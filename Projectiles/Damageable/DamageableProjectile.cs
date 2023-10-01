using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Damageable
{
    [Flags]
    public enum DamageSourceType
    {
        HostileProjectiles = 1 << 1,
        FriendlyProjectiles = 1 << 2, // Does NOT include itself.
        HostileNPCs = 1 << 3
    }
    public abstract class DamageableProjectile : ModProjectile
    {
        #region General Fields
        /// <summary>
        /// Current life of the projectile.
        /// </summary>
        public int Life;
        /// <summary>
        /// Total damage immunity frames. This projectile cannot be hit if this is greater than 0. Descends to 0 every frame.
        /// </summary>
        public int DamageImmunityFrames;
        #endregion

        #region Overridable Fields/Properties

        /// <summary>
        /// Total life of the projectile.
        /// </summary>
        public abstract int LifeMax
        {
            get;
        }

        /// <summary>
        /// The sound the projectile makes on hit.
        /// </summary>
        public abstract SoundStyle HitSound
        {
            get;
        }

        /// <summary>
        /// The sound the projectile makes on death.
        /// </summary>
        public abstract SoundStyle DeathSound
        {
            get;
        }

        /// <summary>
        /// The various possibile damage sources for the projectile.
        /// </summary>
        public abstract DamageSourceType DamageSources
        {
            get;
        }

        /// <summary>
        /// A boolean which determines if the HP bar should be drawn at all.
        /// </summary>
        public virtual bool DrawHPBar
        {
            get;
            set;
        } = true;

        /// <summary>
        /// A boolean which determines if the HP bar should be drawn if <see cref="Life"/> is equal to <see cref="LifeMax"/>
        /// </summary>
        public virtual bool DrawHPBarAtFullHealth
        {
            get;
            set;
        } = false;

        /// <summary>
        /// How many damage immunity frames this projectile should recieve on hit.
        /// </summary>
        public virtual int MaxDamageImmunityFrames
        {
            get;
            set;
        } = 10;

        /// <summary>
        /// List of all enemies this projectile should ignore when checking if it's colliding with an enemy (assuming this is possible).
        /// </summary>
        public virtual List<int> NPCsToIgnore
        {
            get;
            set;
        } = new List<int>();

        /// <summary>
        /// List of all projectiles this projectile should ignore when checking if it's colliding with a projectile (assuming this is possible).
        /// </summary>
        public virtual List<int> ProjectilesToIgnore
        {
            get;
            set;
        } = new List<int>();
        #endregion

        #region Virtual Methods + Sealed Counterparts

        /// <summary>
        /// Copy of <see cref="ModProjectile.SetDefaults"/> with integrations for this custom type.
        /// </summary>
        public virtual void SafeSetDefaults()
        {
        }
        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
            Life = LifeMax;
        }
        public sealed override void PostDraw(Color lightColor)
        {
            DrawHealthBar();
            MouseOverText();
            SafePostDraw(Main.spriteBatch, lightColor);
        }

        /// <summary>
        /// Copy of <see cref="ModProjectile.PostDraw(SpriteBatch, Color)"/> with integrations for this custom type.
        /// </summary>
        public virtual void SafePostDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        }
        public sealed override void AI()
        {
            SafeAI();
            if (DamageImmunityFrames > 0)
            {
                DamageImmunityFrames--;
                return;
            }
            bool wasHit = NPCCollisionCheck();
            if (!wasHit)
            {
                wasHit = ProjectileCollisionCheck();
            }
            if (wasHit && Life > 0)
            {
                if (HitSound != null)
                {
                    SoundEngine.PlaySound(HitSound, Projectile.Center);
                }
            }
            else if (Life <= 0)
            {
                if (DeathSound != null)
                {
                    SoundEngine.PlaySound(DeathSound, Projectile.Center);
                }
                DamageKillEffect();
                Projectile.Kill();
            }
        }
        public virtual bool NPCCollisionCheck()
        {
            Player player = Main.player[Projectile.owner];
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].IsAnEnemy() && DamageSources.HasFlag(DamageSourceType.HostileNPCs))
                {
                    if (Main.npc[i].Hitbox.Intersects(Projectile.Hitbox) && !NPCsToIgnore.Contains(Main.npc[i].type))
                    {
                        int damage = Main.DamageVar(Main.npc[i].damage);
                        int bannerBuffId = Item.NPCtoBanner(Main.npc[i].BannerID());
                        if (bannerBuffId > 0 && player.HasNPCBannerBuff(bannerBuffId))
                        {
                            if (Main.expertMode)
                            {
                                damage = (int)(damage * ItemID.Sets.BannerStrength[Item.BannerToItem(bannerBuffId)].ExpertDamageReceived);
                            }
                            else
                            {
                                damage = (int)(damage * ItemID.Sets.BannerStrength[Item.BannerToItem(bannerBuffId)].NormalDamageReceived);
                            }
                        }
                        CombatText.NewText(new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height), CombatText.DamagedFriendly, damage);
                        Life -= damage;
                        HitEffectNPC(damage, Main.npc[i]);
                        DamageImmunityFrames = MaxDamageImmunityFrames;
                        NetUpdate(true);
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual bool ProjectileCollisionCheck()
        {
            for (int i = 0; i < Main.projectile.Length; i++)
            {
                bool canBeHit = (Main.projectile[i].friendly && DamageSources.HasFlag(DamageSourceType.FriendlyProjectiles) ||
                                 Main.projectile[i].hostile && DamageSources.HasFlag(DamageSourceType.HostileProjectiles)) &&
                                 i != Projectile.whoAmI;
                if (Main.projectile[i].active && canBeHit && Main.projectile[i].damage > 0)
                {
                    if (Main.projectile[i].Colliding(Main.projectile[i].Hitbox, Projectile.Hitbox))
                    {
                        int damage = Main.DamageVar(Main.projectile[i].damage) * 2;
                        damage = Main.expertMode ? (int)(damage * Main.RegisteredGameModes[GameModeID.Expert].EnemyDamageMultiplier) : damage;
                        CombatText.NewText(new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height), CombatText.DamagedFriendly, damage);
                        Life -= damage;
                        HitEffectProjectile(damage, Main.projectile[i]);
                        DamageImmunityFrames = MaxDamageImmunityFrames;
                        if (Projectile.usesIDStaticNPCImmunity)
                            DamageImmunityFrames = Projectile.idStaticNPCHitCooldown;
                        if (Projectile.usesLocalNPCImmunity)
                            DamageImmunityFrames = Projectile.localNPCHitCooldown;
                        NetUpdate(true);
                        return true;
                    }
                }
            }
            return false;
        }
        public virtual void DamageKillEffect()
        {
        }
        /// <summary>
        /// Copy of <see cref="ModProjectile.AI"/> with integrations for this custom type.
        /// </summary>
        public virtual void SafeAI()
        {
        }
        public sealed override void SendExtraAI(BinaryWriter writer)
        {
            SafeSendExtraAI(writer);
            writer.Write(Life);
            writer.Write(DamageImmunityFrames);
        }
        public sealed override void ReceiveExtraAI(BinaryReader reader)
        {
            SafeReceiveExtraAI(reader);
            Life = reader.ReadInt32();
            DamageImmunityFrames = reader.ReadInt32();
        }

        /// <summary>
        /// Copy of <see cref="ModProjectile.SendExtraAI(BinaryWriter)"/> with integrations for this custom type.
        /// <param name="writer"/>The writer used to hold data which will be sent to the server.</param>
        /// </summary>
        public virtual void SafeSendExtraAI(BinaryWriter writer)
        {
        }

        /// <summary>
        /// Copy of <see cref="ModProjectile.ReceiveExtraAI(BinaryReader)"/> with integrations for this custom type.
        /// <param name="reader"/>The reader used to obtain data which will be sent to the server.</param>
        /// </summary>
        public virtual void SafeReceiveExtraAI(BinaryReader reader)
        {
        }

        /// <summary>
        /// Syncs data from this npc to the server, and then to all clients.
        /// </summary>
        /// <param name="force">Whether net spam should be ignored and a net update should be forced.</param>
        /// <param name="forceCondition">The condition required for a forceful sync. Defaults to: <code>Main.netMode != NetmodeID.MultiplayerClient</code></param>
        public virtual void NetUpdate(bool force = false, Func<bool> forceCondition = null)
        {
            if (forceCondition == null)
                forceCondition = () => Main.netMode != NetmodeID.MultiplayerClient;
            if (force)
            {
                if (forceCondition())
                {
                    NetMessage.SendData(MessageID.SyncProjectile, number: Projectile.whoAmI);
                }
            }
            else
                Projectile.netUpdate = true;
        }
        /// <summary>
        /// Does things when the projectile is hit.
        /// </summary>
        /// <param name="damage">The damage taken by the projectile.</param>
        /// <param name="target">The projectile which hit the target.</param>
        public virtual void HitEffectProjectile(int damage, Projectile target)
        {
        }
        /// <summary>
        /// Does things when the projectile is hit.
        /// </summary>
        /// <param name="damage">The damage taken by the projectile.</param>
        /// <param name="target">The npc which hit the target.</param>
        public virtual void HitEffectNPC(int damage, NPC target)
        {
        }
        #endregion

        #region Misc Functions
        /// <summary>
        /// Creates text, much akin to NPCs, when a mouse is over the projectile's hitbox.
        /// </summary>
        public void MouseOverText()
        {
            if (!Main.mouseText)
            {
                Rectangle mouseRectangle = new Rectangle((int)(Main.mouseX + Main.screenPosition.X), (int)(Main.mouseY + Main.screenPosition.Y), 1, 1);
                if (Main.player[Main.myPlayer].gravDir == -1f)
                {
                    mouseRectangle.Y = (int)Main.screenPosition.Y + Main.screenHeight - Main.mouseY;
                }
                if (Projectile.Hitbox.Intersects(mouseRectangle))
                {
                    if (LifeMax > 1)
                    {
                        string lifeDataText = string.Concat(new object[]
                        {
                            Projectile.Name,
                            ": ",
                            Life,
                            "/",
                            LifeMax
                        });
                        Main.spriteBatch.End();
                        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
                        Main.instance.MouseTextHackZoom(lifeDataText);
                    }
                }
            }
        }
        /// <summary>
        /// Draws a health bar below the projectile.
        /// </summary>
        public void DrawHealthBar()
        {
            if (Life == LifeMax && !DrawHPBarAtFullHealth)
                return;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, null, null, null, Main.UIScaleMatrix);
            Main.instance.DrawHealthBar(Projectile.Bottom.X, Projectile.Bottom.Y, Life, LifeMax, 1f, 1f);
        }
        #endregion
    }
}
