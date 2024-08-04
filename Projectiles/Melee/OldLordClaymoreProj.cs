using System;
using System.IO;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Projectiles.BaseProjectiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class OldLordClaymoreProj : BaseIdleHoldoutProjectile
    {
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<OldLordClaymore>();
        public enum SwingState : byte
        {
            Default,
            Swinging,
            Channeling,
            Spinning
        }

        public int Direction = 1;
        public SwingState CurrentState;
        public ref float ChargeTime => ref Projectile.ai[0];
        public ref float GeneralTime => ref Projectile.ai[1];
        public ref float PostSwingRepositionDelay => ref Projectile.localAI[0];
        public ref bool RMBChannel => ref (Owner.HeldItem.ModItem as OldLordClaymore).RMBchannel;
        public bool LMBUse => Owner.altFunctionUse != 2 && !RMBChannel && Owner.itemAnimation > 0;
        public ref float ChargePower => ref Projectile.localAI[1];

        public const int MaxChargeTime = 90;
        public override string Texture => "CalamityMod/Items/Weapons/Melee/OldLordClaymore";
        public override int AssociatedItemID => ModContent.ItemType<OldLordClaymore>();
        public override int IntendedProjectileType => ModContent.ProjectileType<OldLordClaymoreProj>();
        public override bool? CanDamage() => (CurrentState == SwingState.Default || CurrentState == SwingState.Channeling) ? false : base.CanDamage();

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 70;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.noEnchantmentVisuals = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Direction);
            writer.Write(PostSwingRepositionDelay);
            writer.Write(ChargePower);
            writer.Write((byte)CurrentState);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Direction = reader.ReadInt32();
            PostSwingRepositionDelay = reader.ReadSingle();
            ChargePower = reader.ReadSingle();
            CurrentState = (SwingState)reader.ReadByte();
        }

        public override void SafeAI()
        {
            // Handle right click behaviors.
            if (Main.myPlayer == Projectile.owner)
                Owner.Calamity().rightClickListener = true;

            if (!Owner.Calamity().mouseRight)
                RMBChannel = false;
            else
                RMBChannel = true;

            // Initialize the animation max time if necessary.
            if (Owner.itemAnimationMax == 0)
                Owner.itemAnimationMax = (int)(Owner.ActiveItem().useAnimation * Owner.GetAttackSpeed<MeleeDamageClass>());

            // Decide the current phase state of the blade.
            DecideCurrentState();

            // Glue the sword to its owner.
            Projectile.position = Owner.RotatedRelativePoint(Owner.MountedCenter, true) - Projectile.Size / 2f;

            // Use the direction of the owner when not spinning.
            if (CurrentState != SwingState.Spinning)
                Direction = Owner.direction;

            float swingCompletion = Owner.itemAnimation / (float)Owner.itemAnimationMax;
            float swingSpeedInterpolant = Utils.GetLerpValue(1f, 0.8f, swingCompletion, true) * Utils.GetLerpValue(-0.11f, 0.12f, swingCompletion, true);
            float swingInterpolant = (float)Math.Sin(Math.Pow(swingCompletion, 1.15) * MathHelper.TwoPi);
            if (swingInterpolant < 0f)
            {
                swingInterpolant = -(float)Math.Pow(-swingInterpolant, 0.9);
                swingInterpolant = MathHelper.Lerp(swingInterpolant, -1f, 0.7f);
            }
            else
            {
                swingInterpolant = (float)Math.Pow(swingInterpolant, 0.9);
                swingInterpolant = MathHelper.Lerp(swingInterpolant, 1f, 0.7f);
            }

            float baseRotation = swingInterpolant * Direction * 0.96f;

            // Reset the owner's rotation.
            Owner.fullRotation = 0f;

            // Do a spin dash if a swing is done with full power.
            if (CurrentState == SwingState.Spinning)
            {
                swingSpeedInterpolant = 1f;
                baseRotation = MathHelper.WrapAngle(swingCompletion * Direction * MathHelper.Pi * -3f);
                Owner.fullRotation = baseRotation;
                Owner.fullRotationOrigin = Owner.Center - Owner.position;

                // Emit fire dust.
                for (int i = 0; i < 4; i++)
                {
                    Vector2 dustSpawnPosition = Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(Projectile.width) + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 dustVelocity = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(6f) + Main.rand.NextVector2Circular(2.4f, 2.4f);

                    Dust fire = Dust.NewDustPerfect(dustSpawnPosition, 6, dustVelocity);
                    fire.scale = 1.7f;
                    fire.noGravity = true;
                }

                // Release flames.
                if (Main.myPlayer == Projectile.owner && GeneralTime % 6f == 5f)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(8f, 8f), ModContent.ProjectileType<OathswordFlame>(), Projectile.damage / 2, Projectile.knockBack * 0.5f, Projectile.owner);

                if (Main.myPlayer == Projectile.owner)
                {
                    Owner.velocity = Vector2.Lerp(Owner.velocity, Owner.SafeDirectionTo(Main.MouseWorld) * 16f, 0.125f);
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
            if (CurrentState == SwingState.Default)
            {
                Projectile.Opacity = 0f;

                if (PostSwingRepositionDelay > 0f)
                    PostSwingRepositionDelay--;
                else
                    baseRotation = (MathHelper.PiOver2 + 0.36f) * -Direction;

                // Disable afterimages.
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];
            }
            else
            {
                PostSwingRepositionDelay = PostSwingRepositionDelay == -1f ? 0f : 12f;

                // See the Bladecrest Oathsword Projectile code.
                Projectile.Opacity = 1f;
            }

            // Raise the blade and do charge effects upwards if channeling.
            float horizontalBladeOffset = -4f;
            float chargeInterpolant = Utils.GetLerpValue(0f, 65f, ChargeTime, true);
            if (CurrentState == SwingState.Channeling)
            {
                baseRotation = MathHelper.PiOver2 * -Direction;
                horizontalBladeOffset = MathHelper.Lerp(horizontalBladeOffset, 6f, (float)Math.Pow(chargeInterpolant, 0.3));
                if (Owner.itemAnimation < 2)
                    Owner.itemAnimation = 2;
                PostSwingRepositionDelay = -1f;

                ChargePower = MathHelper.Clamp(ChargePower + 1f, 0f, MaxChargeTime);

                // Disable afterimages.
                Projectile.oldPos = new Vector2[Projectile.oldPos.Length];

                ChargeTime++;
            }
            else
                ChargeTime = 0f;

            float idealRotation = baseRotation;
            Owner.itemRotation = idealRotation;

            idealRotation += MathHelper.PiOver4;
            if (Direction == -1)
                idealRotation += MathHelper.Pi;

            // Define rotation.
            Projectile.rotation = Projectile.rotation.AngleTowards(idealRotation, swingSpeedInterpolant * 0.45f).AngleLerp(idealRotation, swingSpeedInterpolant * 0.2f);

            // Decide how far out the blade should go.
            float bladeOffsetFactor = 0.5f + chargeInterpolant * 0.3f;
            if (chargeInterpolant <= 0f)
                bladeOffsetFactor += Utils.GetLerpValue(0.5f, 0.7f, swingCompletion, true) * Utils.GetLerpValue(1f, 0.7f, swingCompletion, true) * 0.45f;

            // Offset the blade so that the handle is attached to the owner's hand.
            Vector2 bladeOffset = (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Projectile.width * bladeOffsetFactor;
            if (Owner.itemAnimation <= 0 || chargeInterpolant > 0f)
                bladeOffset += new Vector2(Direction * horizontalBladeOffset, 8f).RotatedBy(Owner.fullRotation) * (1f - PostSwingRepositionDelay / 12f);
            Projectile.position += bladeOffset;

            // Create charge dust.
            if (CurrentState == SwingState.Channeling)
            {
                int chargeDustCount = (int)Math.Round(MathHelper.Lerp(1f, 3f, ChargePower / MaxChargeTime));
                if (ChargePower >= MaxChargeTime)
                    chargeDustCount = 0;

                for (int i = 0; i < chargeDustCount; i++)
                {
                    Vector2 dustSpawnPosition = Projectile.Center + (Projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * Main.rand.NextFloat(Projectile.width) + Main.rand.NextVector2Circular(6f, 6f);
                    Vector2 dustVelocity = bladeOffset.SafeNormalize(Vector2.Zero) * Main.rand.NextFloat(6f) + Main.rand.NextVector2Circular(2.4f, 2.4f);

                    Dust dust = Dust.NewDustPerfect(dustSpawnPosition, 267, dustVelocity);
                    dust.color = Main.rand.NextBool(4) ? Color.Purple : Color.Red;
                    dust.scale = Main.rand.NextFloat(0.85f, 1.2f);
                    dust.noGravity = true;
                }

                // Create a burst of charge dust before becoming fully charged.
                if (ChargePower == MaxChargeTime - 1f)
                {
                    SoundEngine.PlaySound(SoundID.Item74, Projectile.Center);
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 dustSpawnPosition = Projectile.Center;
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

        public void DecideCurrentState()
        {
            // Reset the state of the weapon if the player stops using it. This cannot reset a dash.
            if ((Owner.itemAnimation <= 0 || (!RMBChannel && CurrentState == SwingState.Channeling)) && CurrentState != SwingState.Spinning)
            {
                Owner.itemAnimation = 0;
                CurrentState = SwingState.Default;
            }

            // Switch to the swing state as necessary. This cannot happen while channeling or spinning.
            if (CurrentState == 0f && !RMBChannel && Owner.itemAnimation > 0 && ChargePower < MaxChargeTime)
                CurrentState = SwingState.Swinging;

            // Switch to the channel state as necessary. This cannot happen while swinging or spinning.
            if ((CurrentState == SwingState.Default || CurrentState == SwingState.Swinging) && RMBChannel)
                CurrentState = SwingState.Channeling;

            // Switch to the spinning charge state. This can only be done while in the default state.
            if (CurrentState == 0f && Owner.itemAnimation > 0 && ChargePower >= MaxChargeTime)
            {
                Owner.fallStart = (int)(Owner.position.Y / 16f);

                if (CurrentState != SwingState.Spinning)
                {
                    SoundEngine.PlaySound(SoundID.DD2_BookStaffCast, Owner.Center);
                    CurrentState = SwingState.Spinning;
                    Projectile.netUpdate = true;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.Opacity <= 0f)
                return false;

            CalamityUtils.DrawAfterimagesCentered(Projectile, 2, lightColor);

            Main.spriteBatch.EnterShaderRegion();
            GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(0.95f, 0.85f, 0.5f));
            GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0f);
            if (ChargePower >= MaxChargeTime)
                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(0.7f - ((Main.GlobalTimeWrappedHourly * (int)(MaxChargeTime * 0.5f)) % (MaxChargeTime * 0.5f) / MaxChargeTime));
            GameShaders.Misc["CalamityMod:BasicTint"].Apply();

            var texture = ModContent.Request<Texture2D>("CalamityMod/Items/Weapons/Melee/OldLordClaymore").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, Projectile.Size / 2f, Projectile.scale, Projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.ExitShaderRegion();

            return false;
        }

        public override void ModifyDamageHitbox(ref Rectangle hitbox)
        {
            if (CurrentState == SwingState.Spinning)
                hitbox = new Rectangle((int)Owner.position.X - 35, (int)Owner.position.Y - 35, 102, 118);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ItemLoader.OnHitNPC(Owner.ActiveItem(), Owner, target, hit, damageDone);
            NPCLoader.OnHitByItem(target, Owner, Owner.ActiveItem(), hit, damageDone);
            PlayerLoader.OnHitNPC(Owner, target, hit, damageDone);

            if (CurrentState == SwingState.Spinning)
            {
                Owner.immuneNoBlink = true;
                Owner.immune = true;
                Owner.immuneTime = 12;
                for (int k = 0; k < Owner.hurtCooldowns.Length; k++)
                    Owner.hurtCooldowns[k] = Owner.immuneTime;
            }
        }

        public override void OnKill(int timeLeft) => Owner.fullRotation = 0f;
    }
}
