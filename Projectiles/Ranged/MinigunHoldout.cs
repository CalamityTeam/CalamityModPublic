using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class MinigunHoldout : ModProjectile
    {
        // Take the name and texture from the weapon
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<Minigun>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/Minigun";

        public static int FramesPerLoad = 13;
        public static int MaxLoadableShots = 15;
        public static float BulletSpeed = 1f;

        private Player Owner => Main.player[Projectile.owner];
        public SlotId MinigunRevSlot;

        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private ref float ShotsLoaded => ref Projectile.ai[1];
        private ref float ShootRecoilTimer => ref Projectile.ai[2]; // Dual functions for rapid fire shooting cooldown and recoil
        private bool ChargeLV1 => CurrentChargingFrames >= Minigun.Charge1Frames;
        private bool ChargeLV2 => CurrentChargingFrames >= Minigun.Charge2Frames;

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public override void SetDefaults()
        {
            Projectile.width = 92;
            Projectile.height = 44;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (Owner.dead) // destroy the holdout if the player dies
            {
                Projectile.Kill();
                return;
            }

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.85f + new Vector2 (0, 3.8f);

            if (SoundEngine.TryGetActiveSound(MinigunRevSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                if (ShotsLoaded > 0)
                {
                    // While bullets are remaining, refresh the lifespan; it will not refresh again after bullets run out
                    Projectile.timeLeft = Minigun.AftershotCooldownFrames;

                    // Retract recoil & shoot faster if charged
                    ShootRecoilTimer -= ChargeLV1 ? 2.3f : 2f;

                    if (ShootRecoilTimer <= 0f)
                    {
                        ChargeSound?.Stop();
                        SoundEngine.PlaySound(Minigun.SmallShot, Projectile.position);

                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                        Vector2 fireVec = shootVelocity.RotatedByRandom(MathHelper.ToRadians(2f));
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, fireVec, ModContent.ProjectileType<TitaniumRailgunShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                        for (int i = 0; i <= 3; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(tipPosition, 107, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f), 0, default, Main.rand.NextFloat(1.3f, 1.7f));
                            dust.noGravity = true;
                        }

                        ShotsLoaded--;
                        ShootRecoilTimer = 16f;
                        Projectile.netSpam = 0;
                        Projectile.netUpdate = true;
                    } 
                }
                // Retracting any remaining recoil
                else if (ShootRecoilTimer > 0)
                    ShootRecoilTimer -= 2;

                // Fires a burst of sparks on the last shot
                if (ShotsLoaded == 0)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        SparkParticle spark2 = new SparkParticle((tipPosition - Projectile.velocity * 3) + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity * Main.rand.NextFloat(9.1f, 17.8f), false, Main.rand.Next(9, 12), Main.rand.NextFloat(0.2f, 0.3f), Main.rand.NextBool(4) ? Color.Gold : Color.Yellow);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    ShotsLoaded--;
                }
            }
            else
            {
                // While channeled, keep refreshing the projectile lifespan
                Projectile.timeLeft = 2;

                // Loads shots until maxed out
                if (ShotsLoaded < MaxLoadableShots && CurrentChargingFrames % FramesPerLoad == 0)
                    ShotsLoaded++;

                if (ChargeLV1)
                    CurrentChargingFrames += 2;
                else
                    CurrentChargingFrames++;

                // Sounds
                if (ChargeLV1)
                {
                    // Pulse sounds play independently of the loop
                    if (CurrentChargingFrames == Minigun.Charge2Frames)
                        SoundEngine.PlaySound(Minigun.ChargeLV2, Projectile.Center);
                    else if (CurrentChargingFrames == Minigun.Charge1Frames)
                    {
                        SoundEngine.PlaySound(Minigun.ChargeLV1, Projectile.Center);
                        ShotsLoaded = MaxLoadableShots;
                    }

                    if ((CurrentChargingFrames - Minigun.Charge1Frames) % (Minigun.ChargeLoopSoundFrames * 2) == 0)
                        MinigunRevSlot = SoundEngine.PlaySound(Minigun.ChargeLoop, Projectile.Center);
                }
                else if (CurrentChargingFrames == 10)
                    MinigunRevSlot = SoundEngine.PlaySound(Minigun.ChargeStart, Projectile.Center);

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    float particleScale = MathHelper.Clamp(CurrentChargingFrames, 0f, Minigun.Charge2Frames);
                    for (int i = 0; i < (ChargeLV2 ? 4 : ChargeLV1 ? 3 : 2); i++)
                    {
                        SparkParticle spark2 = new SparkParticle((tipPosition -Projectile.velocity * 4) + Main.rand.NextVector2Circular(12, 12), -Projectile.velocity * Main.rand.NextFloat(16.1f, 30.8f), false, Main.rand.Next(2, 7), Main.rand.NextFloat(particleScale / 350f, particleScale / 270f), Main.rand.NextBool(4) ? Color.Gold : Color.Yellow);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    Particle orb = new GenericBloom(tipPosition, Projectile.velocity, Color.Yellow, particleScale / 270f, 2, false);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(tipPosition, Projectile.velocity, Color.White, particleScale / 400f, 2, false);
                    GeneralParticleHandler.SpawnParticle(orb2);

                    float strength = particleScale / 45f;
                    Vector3 DustLight = new Vector3(0.200f, 0.200f, 0.100f);
                    Lighting.AddLight(tipPosition, DustLight * strength);
                }

                // Full charge dusts
                if (CurrentChargingFrames == Minigun.Charge1Frames)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(tipPosition, 107);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 8f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1f, 1.3f);
                        chargefull.noGravity = true;
                    }
                }
                if (CurrentChargingFrames == Minigun.Charge2Frames)
                {
                    for (int i = 0; i < 45; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(tipPosition, 107);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 12f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1.2f, 1.4f);
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
                float interpolant = Utils.GetLerpValue(5f, 25f, Projectile.Distance(Main.MouseWorld), true);
                Vector2 oldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.SafeDirectionTo(Main.MouseWorld), interpolant);
            }
            Projectile.Center = armPosition + Projectile.velocity * MathHelper.Clamp(30f - ShootRecoilTimer, 0f, 30f) + new Vector2 (0, 7);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;

            // Rumble (only while channeling)
            float rumble = MathHelper.Clamp(CurrentChargingFrames, 0f, Minigun.Charge2Frames);
            if (OwnerCanShoot)
                Projectile.position += Main.rand.NextVector2Circular(rumble / 70f, rumble / 70f);
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
            if (SoundEngine.TryGetActiveSound(MinigunRevSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool? CanDamage() => false;
    }
}
