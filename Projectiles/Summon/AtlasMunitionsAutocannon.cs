using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityMod.Items.Weapons.Summon;
using Terraria.Audio;
using CalamityMod.Sounds;
using System.IO;
using CalamityMod.Particles;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsAutocannon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public int GeneralTimer;

        public bool CannonIsMounted = true;

        public bool TransitioningFromOverdriveMode;

        public ThanatosSmokeParticleSet SmokeDrawer = new(-1, 3, 0f, 16f, 1.5f);

        public Vector2 CannonCenter => Projectile.Center - Vector2.UnitY * Projectile.scale * 12f;

        public bool InOverdriveMode
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }

        public ref float CannonFrame => ref Projectile.localAI[0];

        public ref float HeatInterpolant => ref Projectile.localAI[1];

        public ref float CannonDirection => ref Projectile.ai[0];

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 5;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 108;
            Projectile.height = 108;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = true;
            Projectile.netImportant = true;
            Projectile.sentry = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 90000;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.Opacity = 0f;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(HeatInterpolant);
            writer.Write(CannonIsMounted);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            HeatInterpolant = reader.ReadSingle();
            CannonIsMounted = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Fade in.
            Projectile.Opacity = Utils.GetLerpValue(0f, 8f, GeneralTimer, true);

            // Handle frames.
            bool frameChangeInterval = Projectile.frameCounter % 6 == 5;
            Projectile.frameCounter++;
            if (frameChangeInterval && Projectile.frame < Main.projFrames[Projectile.type] - 1)
                Projectile.frame++;

            // Handle turret frames.
            bool canShoot = false;
            NPC potentialTarget = Projectile.Center.MinionHoming(AtlasMunitionsBeacon.TargetRange, Owner);
            if (!CannonIsMounted)
                potentialTarget = null;

            // Update the smoke drawer.
            SmokeDrawer.ParticleSpawnRate = HeatInterpolant > 0.7f ? 3 : 9999999;
            SmokeDrawer.BaseMoveRotation = MathHelper.PiOver2 + Projectile.spriteDirection * (Projectile.position.X - Projectile.oldPosition.X) * 0.04f;
            SmokeDrawer.Update();

            if (frameChangeInterval && CannonFrame < 5)
                CannonFrame++;
            else
            {
                // Enter overdrive mode if a target is sufficiently close.
                if (!InOverdriveMode && potentialTarget != null && Projectile.WithinRange(potentialTarget.Center, AtlasMunitionsBeacon.OverdriveModeRange))
                {
                    InOverdriveMode = true;
                    Projectile.netUpdate = true;
                }

                int minFrame = 0;
                int maxFrame = 8;
                int frameToResetToAtLimit = 5;
                canShoot = potentialTarget != null;
                if (!canShoot)
                    maxFrame = 5;

                // Overdrive mode uses frame 9-12 as a transition state and then afterwards loops between 13 and 14 on loop.
                if (InOverdriveMode)
                {
                    minFrame = 9;
                    maxFrame = 16;
                    frameToResetToAtLimit = 14;
                }

                // Transitions from overdrive mode use frames 15-19 as a transition period before moving back to 6 along with disabling overdrive mode and transition flag.
                if (TransitioningFromOverdriveMode)
                {
                    minFrame = 15;
                    maxFrame = 19;
                    frameToResetToAtLimit = 6;
                }

                if (CannonFrame < minFrame)
                    CannonFrame = minFrame;
                if (frameChangeInterval)
                {
                    CannonFrame++;
                    if (CannonFrame >= maxFrame)
                    {
                        CannonFrame = frameToResetToAtLimit;

                        // Disable overdrive mode if done transitioning back to regular frames.
                        if (TransitioningFromOverdriveMode)
                        {
                            InOverdriveMode = false;
                            TransitioningFromOverdriveMode = false;
                            Projectile.netUpdate = true;
                        }
                    }
                }

                // Exit overdrive mode if the target is no longer in sight or too far away.
                bool targetIsInvalidForOverdriveMode = potentialTarget is null || !Projectile.WithinRange(potentialTarget.Center, AtlasMunitionsBeacon.OverdriveModeRange * 1.6f);
                if (InOverdriveMode && targetIsInvalidForOverdriveMode)
                {
                    TransitioningFromOverdriveMode = true;
                    Projectile.netUpdate = true;
                }
            }

            if (canShoot)
            {
                CannonDirection = CannonCenter.AngleTo(potentialTarget.Center);

                // Release lasers rapid-fire at the target.
                float wrappedAttackTimer = GeneralTimer % (InOverdriveMode ? AtlasMunitionsBeacon.TurretShootRateOverdrive : AtlasMunitionsBeacon.TurretShootRate);
                int laserCount = InOverdriveMode ? 3 : 1;
                int laserShootRate = 3;
                if (wrappedAttackTimer == 1f)
                    SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = 0.25f }, CannonCenter);
                
                if (wrappedAttackTimer < laserCount * laserShootRate && wrappedAttackTimer % laserShootRate == 1f)
                {
                    if (Main.myPlayer == Projectile.owner)
                        FireLaserAtTarget(potentialTarget, wrappedAttackTimer / (laserCount * laserShootRate - 1f));
                }
            }
            else
                CannonDirection = CannonDirection.AngleLerp(0f, 0.06f);

            // Have heat dissipate over time.
            if (!InOverdriveMode)
                HeatInterpolant = MathHelper.Clamp(HeatInterpolant - 1f / AtlasMunitionsBeacon.HeatDissipationTime, 0f, 1f);

            // Fall.
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.3f, 0f, 20f);

            // Increment the general timer.
            GeneralTimer++;

            // Be picked up by nearby players if they right click and are holding the appropriate item.
            bool rightClick = Main.mouseRight && Main.mouseRightRelease;
            if (Main.LocalPlayer.WithinRange(Projectile.Center, AtlasMunitionsBeacon.PickupRange) && rightClick && Main.LocalPlayer.ActiveItem().type == ModContent.ItemType<AtlasMunitionsBeacon>() && CannonIsMounted)
            {
                Projectile heldCannon = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Main.LocalPlayer.Center, Vector2.UnitX, ModContent.ProjectileType<AtlasMunitionsAutocannonHeld>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                heldCannon.ModProjectile<AtlasMunitionsAutocannonHeld>().HeatInterpolant = HeatInterpolant * 0.65f;
                heldCannon.originalDamage = Projectile.originalDamage;
                
                CannonIsMounted = false;
                Projectile.netUpdate = true;
                return;
            }

            // Attach any nearby cannons.
            if (!CannonIsMounted)
            {
                int detachedCannonID = ModContent.ProjectileType<AtlasMunitionsAutocannonHeld>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    if (Main.projectile[i].type != detachedCannonID || !Main.projectile[i].active || Main.projectile[i].owner != Projectile.owner)
                        continue;

                    if (!Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox))
                        continue;

                    if (Main.projectile[i].ModProjectile<AtlasMunitionsAutocannonHeld>().BeingHeld)
                        continue;

                    Main.projectile[i].Kill();
                    CannonIsMounted = true;
                    HeatInterpolant = Main.projectile[i].ModProjectile<AtlasMunitionsAutocannonHeld>().HeatInterpolant;
                    Projectile.netUpdate = true;
                    break;
                }
            }

            // If a cannon is not mounted, die if the owner goes very, very far away.
            else if (!Projectile.WithinRange(Owner.Center, 7200f))
            {
                int podID = ModContent.ProjectileType<AtlasMunitionsDropPod>();
                int podUpperID = ModContent.ProjectileType<AtlasMunitionsDropPodUpper>();
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    bool validID = Main.projectile[i].type == podID || Main.projectile[i].type == podUpperID;
                    if (Main.projectile[i].active && validID && Main.projectile[i].owner == Projectile.owner)
                        Main.projectile[i].Kill();
                }

                Projectile.Kill();
            }
        }

        public void FireLaserAtTarget(NPC target, float laserOffsetInterpolant)
        {
            int laserCount = 1;
            int laserDamage = Projectile.damage;
            int laserID = ModContent.ProjectileType<AtlasMunitionsLaser>();
            float offsetAngleMax = 0.0001f;
            if (InOverdriveMode)
            {
                laserCount = 3;
                offsetAngleMax = AtlasMunitionsBeacon.OverdriveProjectileAngularRandomness * 0.5f;
                laserDamage = (int)(laserDamage * AtlasMunitionsBeacon.OverdriveProjectileDamageFactor);
                laserID = ModContent.ProjectileType<AtlasMunitionsLaserOverdrive>();

                // Add heat to the cannon if firing in overdrive mode.
                HeatInterpolant = MathHelper.Clamp(HeatInterpolant + 1f / AtlasMunitionsBeacon.ShotsNeededToReachMaxHeat, 0f, 1f);
                Projectile.netUpdate = true;
            }

            Vector2 laserVelocity = (target.Center - CannonCenter).SafeNormalize(Vector2.UnitY).RotatedByRandom(offsetAngleMax) * 7f;
            Vector2 laserSpawnOffset = CannonDirection.ToRotationVector2() * 66f - (CannonDirection + MathHelper.PiOver2).ToRotationVector2() * Math.Sign(Math.Cos(CannonDirection)) * 10f;
            if (laserCount >= 2)
                laserSpawnOffset += (MathHelper.TwoPi * laserOffsetInterpolant + MathHelper.PiOver2 / laserCount).ToRotationVector2() * 14f;

            Projectile.NewProjectile(Projectile.GetSource_FromAI(), CannonCenter + laserSpawnOffset, laserVelocity, laserID, laserDamage, 0f, Projectile.owner);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsAutocannon").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsAutocannonGlow").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;

            float cannonRotation = CannonDirection;
            SpriteEffects cannonDirection = SpriteEffects.None;
            if (Math.Cos(cannonRotation) < 0f)
            {
                cannonRotation += MathHelper.Pi;
                cannonDirection = SpriteEffects.FlipHorizontally;
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, cannonDirection, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, cannonDirection, 0);

            if (!CannonIsMounted)
                return false;

            // Draw the cannon.
            texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsTurret").Value;
            glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsTurretGlow").Value;
            frame = texture.Frame(1, 20, 0, (int)CannonFrame);
            drawPosition = CannonCenter - Main.screenPosition;
            Vector2 cannonOrigin = new Vector2(0.39f, 0.5f) * frame.Size();
            if (cannonDirection == SpriteEffects.FlipHorizontally)
                cannonOrigin.X = frame.Width - cannonOrigin.X;

            // Draw the smoke.
            SmokeDrawer.DrawSet(drawPosition + Main.screenPosition);

            for (int i = 0; i < 12; i++)
            {
                Vector2 drawOffset = (MathHelper.TwoPi * i / 12f + Main.GlobalTimeWrappedHourly * 2.3f).ToRotationVector2() * (float)Math.Pow(HeatInterpolant, 2.3) * 6f;
                Main.EntitySpriteDraw(texture, drawPosition + drawOffset, frame, AtlasMunitionsBeacon.HeatGlowColor * Projectile.Opacity * 0.5f, cannonRotation, cannonOrigin, Projectile.scale, cannonDirection, 0);
            }
            Main.EntitySpriteDraw(texture, drawPosition, frame, Color.Lerp(lightColor, AtlasMunitionsBeacon.HeatGlowColor, HeatInterpolant * 0.45f) * Projectile.Opacity, cannonRotation, cannonOrigin, Projectile.scale, cannonDirection, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), cannonRotation, cannonOrigin, Projectile.scale, cannonDirection, 0);

            return false;
        }

        // The cannon itself does not do damage- its lasers do.
        public override bool? CanDamage() => false;

        // Don't die on tile collision.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
        
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
    }
}
