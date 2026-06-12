using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Graphics.Primitives;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Prefixes;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.BaseProjectiles
{

    public class BaseSwordHoldoutPlayer : ModPlayer
    {
        /// <summary>
        /// Used to detect what swing a weapon is doing.
        /// </summary>
        public int swingNum = 0;

    }
    /// <summary>
    /// Manages all the required settings for custom sword holdouts done using BaseSwordHoldoutProjectile
    /// </summary>
    public abstract class BaseSwordHoldoutItem : ModItem
    {
        public virtual int ProjectileType { get; set; }
        public virtual bool SizeModifiers { get; set; } = true;

        public virtual bool RClickAutoswing { get; set; } = false;

        public override void SetStaticDefaults()
        {
            if (RClickAutoswing) ItemID.Sets.ItemsThatAllowRepeatedRightClick[Type] = true;
        }

        public override bool MeleePrefix()
        {
            return SizeModifiers;
        }
        public override void SetDefaults()
        {
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.shoot = ProjectileType;
            Item.autoReuse = true;
            Item.useTurn = false;
            Item.useStyle = ItemUseStyleID.Shoot;
            if (SizeModifiers) PrefixLegacy.ItemSets.SwordsHammersAxesPicks[Item.type] = true;
        }
        public override bool CanUseItem(Player player)
        {
            if (player.itemTime > 0)
            {
                return false;
            }
            for (int i = 0; i < 1000; i++)
            {
                var proj = Main.projectile[i];
                if (proj.type == ProjectileType && proj.owner == player.whoAmI && proj.active)
                {
                    return false;
                }

            }
            return base.CanUseItem(player);
        }
    }

    //Doze 24apr2025 - Not a child of BaseCustomUseStyleProjectile because this doesn't use any of it's functionality. Comes from my melee rework mod.
    public abstract class BaseSwordHoldoutProjectile : ModProjectile
    {
        #region Overrideable Fields
        /// <summary>
        /// The width, in degrees, of the sword swing.
        /// Defaults to 180
        /// </summary>
        public virtual int swingWidth { get; set; } = 180;
        /// <summary>
        /// How many frames the sword swing should take.
        /// Defaults to 20
        /// </summary>
        public virtual int swingTime { get; set; } = 20;
        /// <summary>
        /// If the swing should alternate directions each use.
        /// Defaults to true
        /// </summary>
        public virtual bool AlternateSwings { get; set; } = true;
        /// <summary>
        /// How far the held sword should be offset from the player
        /// Defaults to 0
        /// </summary>
        public virtual int OffsetDistance { get; set; } = 0;
        /// <summary>
        /// What item this projectile uses as a base.
        /// </summary>
        public virtual Item BaseItem { get; set; }
        /// <summary>
        /// Whether or not this projectile uses a base item at all.
        /// Defaults to true.
        /// </summary>
        public virtual bool UsesBaseItem { get; set; } = true;

        /// <summary>
        /// Length of after-image trail left by the projectile.
        /// Defaults to 0
        /// </summary>
        public virtual int AfterImageLength { get; set; } = 0;

        /// <summary>
        /// Whether or not this should get melee speed bonuses
        /// Defaults to TRUE
        /// </summary>
        public virtual bool useAttackSpeed { get; set; } = true;

        /// <summary>
        /// Whether or not this should get melee size bonuses (Titan Glove)
        /// Defaults to TRUE
        /// </summary>
        public virtual bool useMeleeSize { get; set; } = true;
        /// <summary>
        /// Array of three colors used for the built-in trail drawing code.
        /// Defaults to White, Black, Green.
        /// </summary>
        public virtual Color[] trailColors { get; set; } = [Color.White, Color.Black, Color.Green];

        /// <summary>
        /// Whether or not to draw a trail when swung.
        /// </summary>
        public virtual bool drawSwordTrail { get; set; } = false;
        /// <summary>
        /// How far the trail should be offset from the center of the projectile. Use to draw trail at sword tip
        /// Defaults to 25
        /// </summary>
        public virtual float trailOffset { get; set; } = 25;
        /// <summary>
        /// Length of the trail in frames
        /// Defaults to 25
        /// </summary>
        public virtual int trailLength { get; set; } = 25;

        /// <summary>
        /// How long before the weapon should begin it's actual swing once used
        /// </summary>
        public virtual int StartupTime { get; set; } = 0;
        /// <summary>
        /// How long the weapon should "cool down" after swinging before ending the item use
        /// </summary>
        public virtual int CooldownTime { get; set; } = 0;
        /// <summary>
        /// Speed at which the projectile should rotate to match the mouse angle during StartupTime.
        /// Set to 0 to disable.
        /// Defaults to 0.5f
        /// </summary>
        public virtual float RotateInStartup { get; set; } = 0.5f;

        /// <summary>
        /// Speed at which the projectile should rotate to match the mouse angle during Cooldown.
        /// Set to 0 to disable.
        /// Defaults to 0.5f
        /// </summary>
        public virtual float RotateInCooldown { get; set; } = 0.5f;

        /// <summary>
        /// What sound to use when the sword begins the actual swing (after startup frames)
        /// </summary>
        public virtual SoundStyle? UseSound { get; set; } = null;
        /// <summary>
        /// The length (from the player) of the projectile's line collision.
        /// This helps to prevent blindspots.
        /// Defaults to 0
        /// </summary>
        public virtual float lineCollisionLength { get; set; } = 0;

        public virtual Color AfterImageColor { get; set; } = Color.White;

        #endregion

        #region Fields

        /// <summary>
        /// Angle of the swing. By default, gets set to mouse angle. Can be set in Spawn(IEntitySource) for fixed angles.
        /// </summary>
        public Vector2 angle { get; set; } = Vector2.Zero;
        /// <summary>
        /// The projectile's center based on offset to the player the previous update
        /// Can be used for effects that stay consistent in motion
        /// </summary>
        public Vector2 oldPlayerOffset { get; set; }
        /// <summary>
        /// Internal timer for the projectile's entire lifespan
        /// </summary>
        public int timer { get; set; } = 0;

        /// <summary>
        /// Internal timer for the projectile's swing animation
        /// </summary>
        internal int swingTimer = 0;

        public static Asset<Texture2D> TrailTexture;

        public float baseScale;
        /// <summary>
        /// Old weapon scales used to track for trail drawing
        /// </summary>
        public List<float> oldScale = new List<float>();
        List<float> oldProjectileRot = new List<float> { };
        List<Vector2> oldProjectilePos = new List<Vector2> { };

        internal int ExistsTime = 20;
        public bool inStartup => timer < StartupTime;

        public bool inCooldown => timer > CooldownStartFrame;

        public bool inSwing => !(inStartup || inCooldown);

        public int CooldownStartFrame => swingTime + StartupTime;

        public int CooldownTimer => timer - CooldownStartFrame;

        public float StartupCompletion => timer / (float)StartupTime;
        public float SwingCompletion => swingTimer / (float)swingTime;
        public float CooldownCompletion => CooldownTimer / (float)CooldownTime;

        private bool hasFakedOnSpawn = false;

        #endregion

        #region Overridable Methods  
        /// <summary>
        /// Happens after movement but before timer increases. Use as to not cancel the default AI behavior.
        /// </summary>
        public virtual void AdditionalAI() { }
        /// <summary>
        /// happens at the beginning of AI the first frame.
        /// </summary>
        /// <param name="source"></param>
        public virtual void Spawn() { }
        /// <summary>
        /// happens after SetDefaults. Use as not to cancel default SetDefaults behavior.
        /// </summary>
        public virtual void Defaults() { }
        /// <summary>
        /// Returns the swing offset from the center angle in radians. Automatically will be inverted if AlternateSwings is enabled.
        /// </summary>
        /// <returns></returns>
        public virtual float SwingFunction()
        {
            return MathHelper.ToRadians(MathHelper.SmoothStep(-swingWidth / 2, swingWidth / 2, swingTimer / (float)swingTime));
        }

        /// <summary>
        /// The function for the width of the trail
        /// </summary>
        /// <param name="completion"></param>
        /// <returns></returns>
        public virtual float trailWidth(float completion, Vector2 vertexPos)
        {
            return MathHelper.Lerp(30, 0, completion);
        }

        /// <summary>
        /// The function for the color of the trail
        /// </summary>
        /// <param name="completion"></param>
        /// <returns></returns>
        public virtual Color trailColor(float completion, Vector2 vertexPos)
        {
            return Color.Black;
        }
        #endregion

        #region Overrides
        /// <summary>
        /// DO NOT OVERRIDE IN MOST SITUATIONS
        /// use Defaults() instead.
        /// That will set defaults after everything is set in the base projectile.
        /// </summary>
        public override void SetDefaults()
        {
            Projectile.timeLeft = swingTime * 2;
            if (UsesBaseItem)
            {
                Projectile.width = Projectile.height = Math.Max(BaseItem.height, BaseItem.width);
            }
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.localNPCHitCooldown = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.extraUpdates = 0;
            Projectile.aiStyle = -2;
            Projectile.DamageType = ModLoader.GetMod("CalamityMod").Find<DamageClass>("TrueMeleeDamageClass");
            Projectile.ContinuouslyUpdateDamageStats = true;
            Projectile.tileCollide = false;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 100;
            Defaults();
        }
        /// <summary>
        /// This hook runs at the beginning of AI the first time through.
        /// </summary>
        private void FakeOnSpawn()
        {
            var player = Main.player[Projectile.owner];
            angle = (player.MountedCenter - player.Calamity().mouseWorld).SafeNormalize(Vector2.One);
            Projectile.velocity = Vector2.Zero;
            if (angle.X < 0)
            {
                player.direction = 1;
                Projectile.spriteDirection = 1 * (int)player.gravDir;
            }
            else
            {
                player.direction = -1;
                Projectile.spriteDirection = -1 * (int)player.gravDir;
            }
            if (AlternateSwings && player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 1)
            {
                Projectile.spriteDirection *= -1;
            }
            if (AlternateSwings)
            {
                player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum++;
            }
            swingTime = Main.player[Projectile.owner].HeldItem.useTime;
            Spawn();
            StartupTime *= Projectile.MaxUpdates;
            CooldownTime *= Projectile.MaxUpdates;
            swingTime *= Projectile.MaxUpdates;
            if (useAttackSpeed)
            {
                var speed = Main.player[Projectile.owner].GetTotalAttackSpeed(Projectile.DamageType);
                if (speed > 3f)
                    speed = 3f;

                if (speed != 0f)
                    speed = 1f / speed;

                swingTime = (int)(swingTime * speed);
                if (swingTime < 1)
                {
                    swingTime = 1;
                }
                StartupTime = (int)(StartupTime * speed);
                CooldownTime = (int)(CooldownTime * speed);
            }
            if (useMeleeSize)
            {
                Projectile.scale *= player.GetMeleeScale();
            }
            baseScale = Projectile.scale;
            ExistsTime = swingTime + StartupTime + CooldownTime;
            Projectile.timeLeft = ExistsTime * 2;
            Projectile.netUpdate = true;
        }

        /// <summary>
        /// DO NOT OVERRIDE IN MOST SITUATIONS
        /// use AdditionalAI() instead
        /// That will run AI code at the right time.
        /// </summary>
        public override void AI()
        {
            if (!hasFakedOnSpawn)
            {
                FakeOnSpawn();
                hasFakedOnSpawn = true;
            }
            var player = Main.player[Projectile.owner];
            Projectile.gfxOffY = player.gfxOffY;
            player.Calamity().mouseWorldListener = true;
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            float adust = MathHelper.ToRadians(225);
            if (timer < StartupTime || timer > StartupTime + swingTime)
            {
                if (inStartup)
                    angle = Vector2.Lerp(angle, (player.MountedCenter - player.Calamity().mouseWorld).SafeNormalize(Vector2.One), RotateInStartup);
                if (inCooldown)
                    angle = Vector2.Lerp(angle, (player.MountedCenter - player.Calamity().mouseWorld).SafeNormalize(Vector2.One), RotateInCooldown);
                if (angle.X < 0)
                {
                    player.direction = 1;
                    Projectile.spriteDirection = 1 * (int)player.gravDir;
                }
                else
                {
                    player.direction = -1;
                    Projectile.spriteDirection = -1 * (int)player.gravDir;
                }
                if (AlternateSwings && player.GetModPlayer<BaseSwordHoldoutPlayer>().swingNum % 2 == 1)
                {
                    Projectile.spriteDirection *= -1;
                }
            }
            if (Projectile.spriteDirection == -1)
            {
                adust = MathHelper.ToRadians(-45);
            }
            var armCenter = player.MountedCenter - new Vector2(5 * player.direction, 2);
            if (AfterImageLength > 0)
            {
                oldProjectileRot.Add(Projectile.rotation);
                oldProjectilePos.Add(Projectile.Center + new Vector2(0, Projectile.gfxOffY));
                if (oldProjectileRot.Count > AfterImageLength)
                {
                    oldProjectileRot.RemoveAt(0);
                    oldProjectilePos.RemoveAt(0);
                }
            }
            if (inSwing && swingTimer == 1 && UseSound != null)
            {
                SoundEngine.PlaySound((SoundStyle)UseSound, player.Center);
            }
            var angle2 = (AlternateSwings && modplayer.swingNum % 2 == 1 ? SwingFunction() : SwingFunction());
            Projectile.Center = armCenter - (angle * OffsetDistance * (1 + (Projectile.scale - 1) * 0.75f)).RotatedBy(Projectile.spriteDirection * angle2);
            Projectile.rotation = angle.RotatedBy(Projectile.spriteDirection * angle2).ToRotation() + adust;
            AdditionalAI();
            if (!Projectile.active)
                return;
            oldPlayerOffset = Projectile.Center - player.MountedCenter;
            player.itemTime = ExistsTime + 2 - timer;
            player.itemAnimation = ExistsTime + 2 - timer;
            if (timer > ExistsTime)
            {
                player.itemTime = 0;
                player.itemAnimation = 0;
                Projectile.Kill();
            }
            timer++;
            if (timer >= StartupTime && timer < StartupTime + swingTime)
            {
                swingTimer++;
            }
            var armDir = armCenter - Projectile.Center;
            armDir.Y *= player.gravDir;
            player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, armDir.ToRotation() + MathHelper.ToRadians(90));
            oldScale.Insert(0, Projectile.scale);
        }
        public override bool PreDraw(ref Color lightColor)
        {

            var player = Main.player[Projectile.owner];
            var modplayer = player.GetModPlayer<BaseSwordHoldoutPlayer>();
            if (AfterImageLength > 0)
            {
                Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
                for (int i = 0; i < oldProjectileRot.Count; i++)
                {
                    var col = Projectile.Opacity * (i / (float)AfterImageLength) * 0.1f;
                    if (Projectile.spriteDirection == 1)
                    {
                        Main.EntitySpriteDraw(texture, oldProjectilePos[i] - Main.screenPosition, null, AfterImageColor * col, oldProjectileRot[i], texture.Size() / 2, oldScale[i], SpriteEffects.None, 0);
                    }
                    else
                    {
                        Main.EntitySpriteDraw(texture, oldProjectilePos[i] - Main.screenPosition, null, AfterImageColor * col, oldProjectileRot[i], texture.Size() / 2, oldScale[i], SpriteEffects.FlipHorizontally, 0);
                    }
                }
            }
            if (drawSwordTrail && timer >= StartupTime && timer <= StartupTime + swingTime)
            {
                Main.spriteBatch.EnterShaderRegion();
                if (TrailTexture == null)
                {
                    TrailTexture = ModContent.Request<Texture2D>("CalamityMod/ExtraTextures/GreyscaleGradients/VoronoiShapes", (AssetRequestMode)2);
                }
                Vector2 trailOffset = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() + Projectile.Size * 0.5f;
                GameShaders.Misc["CalamityMod:ExobladeSlash"].SetShaderTexture(TrailTexture);

                GameShaders.Misc["CalamityMod:ExobladeSlash"].UseColor(trailColors[0]);

                GameShaders.Misc["CalamityMod:ExobladeSlash"].UseSecondaryColor(trailColors[1]);

                GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["fireColor"].SetValue(trailColors[2].ToVector3());

                GameShaders.Misc["CalamityMod:ExobladeSlash"].Shader.Parameters["flipped"].SetValue(Projectile.spriteDirection == -1 ? false : true);
                GameShaders.Misc["CalamityMod:ExobladeSlash"].Apply();

                var positionsToUse = Projectile.oldPos.Take((int)MathHelper.Min(trailLength, swingTimer)).ToArray();
                for (var i = 0; i < positionsToUse.Length; i++)
                {
                    if (i >= timer) break;
                    positionsToUse[i] += (Projectile.oldRot[i] - MathHelper.PiOver4 * (Projectile.spriteDirection == -1 ? 3 : 1)).ToRotationVector2() * this.trailOffset * oldScale[i];
                }
                PrimitiveRenderer.RenderTrail(positionsToUse, new(trailWidth, trailColor, (_, _) => trailOffset, shader: GameShaders.Misc["CalamityMod:ExobladeSlash"]), 25);
                Main.spriteBatch.ExitShaderRegion();

                Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            }
            Main.player[Projectile.owner].heldProj = Projectile.whoAmI;
            return true;
        }
        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            var center = hitbox.Center.ToVector2();
            hitbox.Height = (int)(Projectile.height * Projectile.scale);
            hitbox.Width = (int)(Projectile.width * Projectile.scale);
            hitbox.Location = (center - new Vector2(hitbox.Width / 2, hitbox.Height / 2)).ToPoint();

        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (lineCollisionLength > 0)
            {
                var player = Main.player[Projectile.owner];
                var armcenter = player.MountedCenter - new Vector2(5 * player.direction, 2);
                var swordDir = armcenter.DirectionTo(Projectile.Center);
                var collisionline = new Vector2(lineCollisionLength / 2f, 0).RotatedBy(swordDir.ToRotation()) * Projectile.scale;
                bool c = Collision.CheckAABBvLineCollision(targetHitbox.Location.ToVector2(), targetHitbox.Size(), Projectile.Center, Projectile.Center + collisionline);
                if (c && !float.IsNaN(collisionline.X) && !float.IsNaN(collisionline.Y))
                    return true;
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.HitDirectionOverride = ((Main.player[Projectile.owner].DirectionTo(target.Center)).X >= 0 ? 1 : -1);
        }

        public override bool? CanDamage()
        {
            return inSwing;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(angle);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            angle = reader.ReadVector2();
        }
        #endregion

        #region Helper Methods
        /// <summary>
        /// Spawns projectiles from the sword swing, automatically spacing them out on the if an amount is set.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="velocity">Velocity of the projectile. Automatically converted into a Vector2 in the direction of swing.</param>
        /// <param name="damagemod">The modifier from the source projectile's damage for this projectile</param>
        /// <param name="amount">The amount of projectiles for the sword to shoot. If set, it will space out those projectiles evenly. If unset, will force a shot.</param>
        /// <param name="negate"></param>
        /// <returns></returns>
        public void shootCheck(int type = 0, float velocity = 1, float damagemod = 1, int amount = 0, int negate = 0, int ai0 = 0)
        {
            if (negate == 0)
            {
                negate = Projectile.spriteDirection;
            }
            if (amount == 0)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation + negate * MathHelper.ToRadians(45 - negate * 90)).ToRotationVector2() * velocity, type, (int)(Projectile.damage * damagemod), Projectile.knockBack, Projectile.owner, ai0);
                return;
            }
            amount += 1;
            if (swingTimer % (swingTime / amount) == 0 && swingTimer > 0 && swingTimer < swingTime - swingTime / amount / 2)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, (Projectile.rotation + negate * MathHelper.ToRadians(45 - negate * 90)).ToRotationVector2() * velocity, type, (int)(Projectile.damage * damagemod), Projectile.knockBack, Projectile.owner, ai0);
            }
        }

        #endregion
    }

}
