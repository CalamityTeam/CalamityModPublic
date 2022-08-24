using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System;
using CalamityMod.Items.Weapons.Summon;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Summon
{
    public class AtlasMunitionsAutocannon : ModProjectile
    {
        public int GeneralTimer;

        public bool TransitioningFromOverdriveMode;

        public Vector2 CannonCenter => Projectile.Center - Vector2.UnitY * Projectile.scale * 12f;

        public bool InOverdriveMode
        {
            get => Projectile.ai[1] == 1f;
            set => Projectile.ai[1] = value.ToInt();
        }

        public ref float CannonFrame => ref Projectile.localAI[0];

        public ref float CannonDirection => ref Projectile.ai[0];

        public Player Owner => Main.player[Projectile.owner];

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Atlas Munitions Autocannon");
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
            Projectile.timeLeft = Projectile.SentryLifeTime;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.Opacity = 0f;
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
                    maxFrame = 15;
                    frameToResetToAtLimit = 13;
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
                if (GeneralTimer % AtlasMunitionsBeacon.TurretShootRate == AtlasMunitionsBeacon.TurretShootRate - 1)
                {
                    SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = 0.25f }, CannonCenter);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        FireLaserAtTarget(potentialTarget);
                }
            }
            else
                CannonDirection = CannonDirection.AngleLerp(0f, 0.06f);

            // Fall.
            Projectile.velocity.Y = MathHelper.Clamp(Projectile.velocity.Y + 0.3f, 0f, 20f);

            // Increment the general timer.
            GeneralTimer++;
        }

        public void FireLaserAtTarget(NPC target)
        {
            int laserCount = 1;
            int laserDamage = Projectile.damage;
            int laserID = ModContent.ProjectileType<AtlasMunitionsLaser>();
            float offsetAngleMax = 0.0001f;
            if (InOverdriveMode)
            {
                laserCount = 3;
                offsetAngleMax = AtlasMunitionsBeacon.OverdriveProjectileAngularRandomness;
                laserDamage = (int)(laserDamage * AtlasMunitionsBeacon.OverdriveProjectileDamageFactor);
                laserID = ModContent.ProjectileType<AtlasMunitionsLaserOverdrive>();
            }

            Vector2 laserVelocity = (target.Center - CannonCenter).SafeNormalize(Vector2.UnitY) * 9.25f;
            for (int i = 0; i < laserCount; i++)
            {
                Vector2 laserSpawnOffset = CannonDirection.ToRotationVector2() * 92f;
                if (laserCount >= 2)
                    laserSpawnOffset += (MathHelper.TwoPi * i / laserCount + MathHelper.PiOver2 / laserCount).ToRotationVector2() * 10f;

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), CannonCenter + laserSpawnOffset.RotatedByRandom(offsetAngleMax), laserVelocity, laserID, laserDamage, 0f, Projectile.owner);
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsAutocannon").Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsAutocannonGlow").Value;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), Projectile.rotation, frame.Size() * 0.5f, Projectile.scale, 0, 0);

            // Draw the cannon.
            texture = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsTurret").Value;
            glowmask = ModContent.Request<Texture2D>("CalamityMod/Projectiles/Summon/AtlasMunitionsTurretGlow").Value;
            frame = texture.Frame(1, 20, 0, (int)CannonFrame);
            drawPosition = CannonCenter - Main.screenPosition;
            float cannonRotation = CannonDirection;
            Vector2 cannonOrigin = new Vector2(0.39f, 0.5f) * frame.Size();
            SpriteEffects cannonDirection = SpriteEffects.None;
            if (Math.Cos(cannonRotation) < 0f)
            {
                cannonRotation += MathHelper.Pi;
                cannonDirection = SpriteEffects.FlipHorizontally;
                cannonOrigin.X = frame.Width - cannonOrigin.X;
            }

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), cannonRotation, cannonOrigin, Projectile.scale, cannonDirection, 0);
            Main.EntitySpriteDraw(glowmask, drawPosition, frame, Projectile.GetAlpha(Color.White), cannonRotation, cannonOrigin, Projectile.scale, cannonDirection, 0);

            return false;
        }

        // The cannon itself does not do damage- its lasers do.
        public override bool? CanDamage() => false;

        // Don't die on tile collision.
        public override bool OnTileCollide(Vector2 oldVelocity) => false;
    }
}
