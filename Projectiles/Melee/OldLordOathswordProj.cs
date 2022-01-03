using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OldLordOathswordProj : BaseIdleHoldoutProjectile
    {
        public int Direction = 1;
        public ref float ChargeTime => ref projectile.ai[0];
        public ref float GeneralTime => ref projectile.ai[1];
        public ref float PostSwingRepositionDelay => ref projectile.localAI[0];
        public ref bool RMBChannel => ref (Owner.HeldItem.modItem as OldLordOathsword).RMBchannel;
        public bool LMBUse => Owner.altFunctionUse != 2 && !RMBChannel && Owner.itemAnimation > 0;
        public ref float ChargePower => ref projectile.localAI[1];
        public float CurrentState; //0 = default, 1 = Swinging, 2 = Channeling, 3 = Spinning
        public const int MaxChargeTime = 60;
        public override string Texture => "CalamityMod/Items/Weapons/Melee/OldLordOathsword";
        public override int AssociatedItemID => ModContent.ItemType<OldLordOathsword>();
        public override int IntendedProjectileType => ModContent.ProjectileType<OldLordOathswordProj>();
        public override bool CanDamage() => CurrentState != 0; //Could also disable the damage during the channel state,

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Lord Oathsword");
            ProjectileID.Sets.TrailingMode[projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            projectile.width = projectile.height = 70;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.timeLeft = 90000;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
            projectile.noEnchantments = true;
            projectile.Calamity().trueMelee = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Direction);
            writer.Write(PostSwingRepositionDelay);
            writer.Write(ChargePower);
            writer.Write(CurrentState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Direction = reader.ReadInt32();
            PostSwingRepositionDelay = reader.ReadSingle();
            ChargePower = reader.ReadSingle();
            CurrentState = reader.ReadSingle();
        }

        public override void SafeAI()
        {
            #region RMB Channel setup
            CalamityPlayer controlsSync = Owner.GetModPlayer<CalamityPlayer>();
            if (Main.myPlayer == projectile.owner)
            {
                controlsSync.rightClickListener = true;
            }

            if (!controlsSync.mouseRight)
                RMBChannel = false;
            else
                RMBChannel = true;
            #endregion


            // Initialize the animation max time if necessary.
            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.meleeSpeed);

            if (((Owner.itemAnimation <= 0) || (!RMBChannel && CurrentState == 2f)) && CurrentState != 3) //Reset the state of the weapon if the player stops using it. This cannot reset a dash
                Owner.itemAnimation = 0;
                CurrentState = 0f;
            if (CurrentState == 0f && !RMBChannel && Owner.itemAnimation > 0 && ChargePower < MaxChargeTime) //Allowed to start swinging from the default state. You cannot swing while rotating or channeling. <- Unless we want to let the player interrupt their dashes with a swing
                CurrentState = 1f;
            if ((CurrentState == 0f || CurrentState == 1f) && RMBChannel) //Allowed to switch from default or swinging state to channeling. You cannot channel from the rotating state
                CurrentState = 2f;
            if(CurrentState == 0f && Owner.itemAnimation > 0 && ChargePower >= MaxChargeTime) //You can only dash if you're not swinging or channeling (Not like you could swap from the Swinging state to the Dashing one anyways)
                CurrentState = 3f;


            // Glue the sword to its owner.
            projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - projectile.Size / 2f + Vector2.UnitY * Owner.gfxOffY;

            if (CurrentState != 3f)
                Direction = Owner.direction;

            float swingCompletion = Owner.itemAnimation / (float)Owner.itemAnimationMax;
            float swingSpeedInterpolant = Utils.InverseLerp(1f, 0.8f, swingCompletion, true) * Utils.InverseLerp(-0.05f, 0.2f, swingCompletion, true);
            float swingInterpolant = (float)Math.Sin(swingCompletion * MathHelper.TwoPi);
            if (swingInterpolant < 0f)
            {
                swingInterpolant = -(float)Math.Pow(-swingInterpolant, 0.9);
                swingInterpolant = MathHelper.Lerp(swingInterpolant, -1f, 0.5f);
            }
            else
            {
                swingInterpolant = (float)Math.Pow(swingInterpolant, 0.9);
                swingInterpolant = MathHelper.Lerp(swingInterpolant, 1f, 0.5f);
            }

            float baseRotation = swingInterpolant * Direction * 0.96f;

            // Reset the owner's rotation.
            Owner.fullRotation = 0f;

            // Do a spin dash if a swing is done with full power.
            if (CurrentState == 3f)
            {
                swingSpeedInterpolant = 1f;
                baseRotation = MathHelper.WrapAngle(swingCompletion * Direction * MathHelper.Pi * -3f);
                Owner.fullRotation = baseRotation;
                Owner.fullRotationOrigin = Owner.Center - Owner.position;
                Owner.immuneNoBlink = true;
                Owner.immune = true;
                Owner.immuneTime = 5;
                for (int k = 0; k < Owner.hurtCooldowns.Length; k++)
                    Owner.hurtCooldowns[k] = Owner.immuneTime;

                // Emit fire dust.
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dustSpawnPosition = projectile.Center + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(projectile.width) + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 dustVelocity = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(6f) + Main.rand.NextVector2Circular(2.4f, 2.4f);

                    Dust fire = Dust.NewDustPerfect(dustSpawnPosition, 6, dustVelocity);
                    fire.scale = 1.7f;
                    fire.noGravity = true;
                }

                // Release flames.
                if (Main.myPlayer == projectile.owner && GeneralTime % 6f == 5f)
                    Projectile.NewProjectile(projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f), ModContent.ProjectileType<OathswordFlame>(), projectile.damage / 2, projectile.knockBack * 0.5f, projectile.owner);

                if (Main.myPlayer == projectile.owner)
                {
                    Owner.velocity = Vector2.Lerp(Owner.velocity, Owner.SafeDirectionTo(Main.MouseWorld) * 23f, 0.125f);
                    NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Main.myPlayer);
                }

                // Stop charging once the item animation has completed.
                if (Owner.itemAnimation <= 1)
                {
                    ChargePower = 0f;
                    CurrentState = 0f;
                }
            }

            // Hold the blade upwards if not firing.
            if (CurrentState == 0f)
            {
                if (PostSwingRepositionDelay > 0f)
                    PostSwingRepositionDelay--;
                else
                    baseRotation = (MathHelper.PiOver2 + 0.36f) * -Direction;

                // Disable afterimages.
                projectile.oldPos = new Vector2[projectile.oldPos.Length];
            }
            else
                PostSwingRepositionDelay = PostSwingRepositionDelay == -1f ? 0f : 12f;

            // Raise the blade and do charge effects upwards if channeling.
            float horizontalBladeOffset = -4f;
            float chargeInterpolant = Utils.InverseLerp(0f, 35f, ChargeTime, true);
            if (CurrentState == 2f)
            {
                baseRotation = MathHelper.PiOver2 * -Direction;
                horizontalBladeOffset = MathHelper.Lerp(horizontalBladeOffset, 10f, (float)Math.Pow(chargeInterpolant, 0.3));
                if (Owner.itemAnimation < 2)
                    Owner.itemAnimation = 2;
                PostSwingRepositionDelay = -1f;

                ChargePower = MathHelper.Clamp(ChargePower + 1f, 0f, MaxChargeTime);

                // Disable afterimages.
                projectile.oldPos = new Vector2[projectile.oldPos.Length];

                ChargeTime++;
            }
            else
                ChargeTime = 0f;

            float idealRotation = baseRotation;

            idealRotation += MathHelper.PiOver4;
            if (Direction == -1)
                idealRotation += MathHelper.Pi;

            // Define rotation.
            projectile.rotation = projectile.rotation.AngleTowards(idealRotation, swingSpeedInterpolant * 0.45f).AngleLerp(idealRotation, swingSpeedInterpolant * 0.2f);

            // Decide how far out the blade should go.
            float bladeOffsetFactor = 0.5f + chargeInterpolant * 0.3f;
            if (chargeInterpolant <= 0f)
                bladeOffsetFactor += Utils.InverseLerp(0.5f, 0.7f, swingCompletion, true) * Utils.InverseLerp(1f, 0.7f, swingCompletion, true) * 0.6f;

            // Offset the blade so that the handle is attached to the owner's hand.
            Vector2 bladeOffset = (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * projectile.width * bladeOffsetFactor;
            if (Owner.itemAnimation <= 0 || chargeInterpolant > 0f)
                bladeOffset += new Vector2(Direction * horizontalBladeOffset, 16f).RotatedBy(Owner.fullRotation) * (1f - PostSwingRepositionDelay / 12f);
            projectile.position += bladeOffset;

            // Create charge dust.
            if (CurrentState == 2f)
            {
                int chargeDustCount = (int)Math.Round(MathHelper.Lerp(1f, 3f, ChargePower / 60f));
                if (ChargePower >= MaxChargeTime)
                    chargeDustCount = 0;

                for (int i = 0; i < chargeDustCount; i++)
                {
                    Vector2 dustSpawnPosition = projectile.Center + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(projectile.width) + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 dustVelocity = bladeOffset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(6f) + Main.rand.NextVector2Circular(2.4f, 2.4f);

                    Dust dust = Dust.NewDustPerfect(dustSpawnPosition, 267, dustVelocity);
                    dust.color = Main.rand.NextBool(4) ? Color.Purple : Color.Red;
                    dust.scale = Main.rand.NextFloat(0.85f, 1.2f);
                    dust.noGravity = true;
                }

                // Create a burst of charge dust before becoming fully charged.
                if (ChargePower == MaxChargeTime - 1f)
                {
                    Main.PlaySound(SoundID.Item74, projectile.Center);
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 dustSpawnPosition = projectile.Center;
                        Vector2 dustVelocity = (MathHelper.TwoPi * i / 30f).ToRotationVector2() * 5f;
                        Dust dust = Dust.NewDustPerfect(dustSpawnPosition, 267, dustVelocity);
                        dust.color = Color.Violet;
                        dust.scale = 1.4f;
                        dust.noGravity = true;
                    }
                }
            }

            GeneralTime++;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, 2, lightColor);

            spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(0.95f, 0.85f, 0.5f));
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0f);
            if (ChargePower >= MaxChargeTime)
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0.7f - ((Main.GlobalTime * 30) % 30f / 60f));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            var texture = ModContent.GetTexture("CalamityMod/Items/Weapons/Melee/OldLordOathsword");

            spriteBatch.Draw(texture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation, projectile.Size / 2f, projectile.scale, projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
            spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, damage, knockback, crit);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), damage, knockback, crit);
            PlayerHooks.OnHitNPC(Owner, Owner.ActiveItem(), target, damage, knockback, crit);
        }

        public override void Kill(int timeLeft) => Owner.fullRotation = 0f;
    }
}
