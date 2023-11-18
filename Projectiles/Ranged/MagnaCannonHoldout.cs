using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MagnaCannonHoldout : ModProjectile
    {
        // Take the name and texture from the weapon
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<MagnaCannon>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/MagnaCannon";

        public static int FramesPerLoad = 9;
        public static int MaxLoadableShots = 20;
        public static float BulletSpeed = 12f;

        private Player Owner => Main.player[Projectile.owner];
        public SlotId MagnaChargeSlot;

        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float ShotsLoaded => ref Projectile.ai[1];
        private ref float ShootTimer => ref Projectile.ai[2];
        private bool FullyCharged => CurrentChargingFrames >= MagnaCannon.FullChargeFrames;
        public int Time = 0;

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Time++;
            if (Time == 1)
                Projectile.alpha = 255;
            else
                Projectile.alpha = 0;

            if (Owner.dead) // destroy the holdout if the player dies
            {
                Projectile.Kill();
                return;
            }

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.9f;

            if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                if (ShotsLoaded > 0)
                {
                    // While bullets are remaining, refresh the lifespan; it will not refresh again after bullets run out
                    Projectile.timeLeft = MagnaCannon.AftershotCooldownFrames;
                    ShootTimer--;
                }
                    
                if (ShootTimer <= 0f)
                {
                    ChargeSound?.Stop();
                    SoundEngine.PlaySound(MagnaCannon.Fire, Projectile.position);

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(9f)), ModContent.ProjectileType<MagnaShot>(), Projectile.damage, Projectile.knockBack * (FullyCharged ? 3 : 1), Projectile.owner);
                    for (int i = 0; i <= 3; i++)
                    {
                        Dust dust = Dust.NewDustPerfect(tipPosition, 187, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f), 0, default, Main.rand.NextFloat(1.5f, 2.3f));
                        dust.noGravity = true;
                    }

                    ShotsLoaded--;
                    ShootTimer = (FullyCharged ? 4f : 5f);
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;

                // Loads shots until maxed out
                if (ShotsLoaded < MaxLoadableShots && CurrentChargingFrames % FramesPerLoad == 0)
                    ShotsLoaded++;

                CurrentChargingFrames++;

                // Sounds
                if (FullyCharged)
                {
                    ShotsLoaded = MaxLoadableShots;
                    if (CurrentChargingFrames == MagnaCannon.FullChargeFrames)
                        MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeFull, Projectile.Center);
                    else if ((CurrentChargingFrames - MagnaCannon.FullChargeFrames - MagnaCannon.ChargeFullSoundFrames) % MagnaCannon.ChargeLoopSoundFrames == 0)
                        MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeLoop, Projectile.Center);
                }
                else if (CurrentChargingFrames == 10)
                    MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeStart, Projectile.Center);

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    if (!FullyCharged)
                    {
                        Particle streak = new ManaDrainStreak(Owner, Main.rand.NextFloat(0.06f + (CurrentChargingFrames / 180), 0.08f + (CurrentChargingFrames / 180)), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(0.3f * CurrentChargingFrames, 0.3f * CurrentChargingFrames), 0f, Color.White, Color.Aqua, 7, tipPosition);
                        GeneralParticleHandler.SpawnParticle(streak);
                    }
                    float orbScale = MathHelper.Clamp(CurrentChargingFrames, 0f, MagnaCannon.FullChargeFrames);
                    Particle orb = new GenericBloom(tipPosition, Projectile.velocity, Color.DarkBlue, orbScale / 135f, 2);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(tipPosition, Projectile.velocity, Color.Aqua, orbScale / 200f, 2);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }

                // Full charge dusts
                if (CurrentChargingFrames == MagnaCannon.FullChargeFrames)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(tipPosition, 160);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 18f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1f, 1.5f);
                        chargefull.noGravity = true;
                    }
                }
            }
            UpdateProjectileHeldVariables(armPosition);
            ManipulatePlayerVariables();
        }

        private void UpdateProjectileHeldVariables(Vector2 armPosition)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                float interpolant = Utils.GetLerpValue(5f, 40f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
            }
            Projectile.Center = armPosition + Projectile.velocity * 20;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;

            // Rumble (only while channeling)
            float rumble = MathHelper.Clamp(CurrentChargingFrames, 0f, MagnaCannon.FullChargeFrames);
            if (OwnerCanShoot)
                Projectile.position += Main.rand.NextVector2Circular(rumble / 43f, rumble / 43f);
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        public override void OnKill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool? CanDamage() => false;
    }
}
