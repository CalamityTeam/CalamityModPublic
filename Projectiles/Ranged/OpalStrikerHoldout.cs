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
    public class OpalStrikerHoldout : ModProjectile
    {
        // Take the name and texture from the weapon
        public override LocalizedText DisplayName => CalamityUtils.GetItemName<OpalStriker>();
        public override string Texture => "CalamityMod/Items/Weapons/Ranged/OpalStriker";

        public static float ChargedDamageMult = 6f;
        public static float ChargedKBMult = 3f;
        public static float BulletSpeed = 12f;

        private Player Owner => Main.player[Projectile.owner];
        public SlotId OpalChargeSlot;

        private ref float CurrentChargingFrames => ref Projectile.ai[0];
        private bool FullyCharged => CurrentChargingFrames >= OpalStriker.FullChargeFrames;

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 30;
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
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.85f;

            if (SoundEngine.TryGetActiveSound(OpalChargeSlot, out var ChargeSound) && ChargeSound.IsPlaying)
                ChargeSound.Position = Projectile.Center;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                if (Projectile.ai[1] != 1f)
                {
                    ChargeSound?.Stop();
                    SoundEngine.PlaySound(FullyCharged ? OpalStriker.ChargedFire : OpalStriker.Fire, Projectile.Center);

                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * BulletSpeed;
                    if (FullyCharged)
                    {
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<OpalChargedStrike>(), (int)(Projectile.damage * ChargedDamageMult), Projectile.knockBack * ChargedKBMult, Projectile.owner);
                        for (int i = 0; i <= 10; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(tipPosition, 162, shootVelocity.RotatedByRandom(MathHelper.ToRadians(20f)) * Main.rand.NextFloat(0.8f, 1.4f), 0, default, Main.rand.NextFloat(1.5f, 2.3f));
                            dust.noGravity = true;
                        }
                    }
                    else
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<OpalStrike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                    Projectile.ai[1] = 1f;
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            else
            {
                // While channeled, keep refreshing the projectile lifespan to the amount when attacking
                Projectile.timeLeft = OpalStriker.AftershotCooldownFrames;
                CurrentChargingFrames++;

                // Sounds
                if (FullyCharged)
                {
                    if ((CurrentChargingFrames - OpalStriker.FullChargeFrames) % OpalStriker.ChargeLoopSoundFrames == 0)
                        OpalChargeSlot = SoundEngine.PlaySound(OpalStriker.ChargeLoop, Projectile.Center);
                }
                else if (CurrentChargingFrames == 10)
                    OpalChargeSlot = SoundEngine.PlaySound(OpalStriker.Charge, Projectile.Center);

                // Charge-up visuals
                if (CurrentChargingFrames >= 10)
                {
                    if (!FullyCharged)
                    {
                        Particle streak = new ManaDrainStreak(Owner, Main.rand.NextFloat(0.06f + (CurrentChargingFrames / 180), 0.08f + (CurrentChargingFrames / 180)), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(0.3f * CurrentChargingFrames, 0.3f * CurrentChargingFrames), 0f, Color.Gold, Color.Orange, 7, tipPosition);
                        GeneralParticleHandler.SpawnParticle(streak);
                    }

                    float orbScale = MathHelper.Clamp(CurrentChargingFrames, 0f, OpalStriker.FullChargeFrames);
                    Particle orb = new GenericBloom(tipPosition, Projectile.velocity, Color.OrangeRed, orbScale / 135f, 2);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(tipPosition, Projectile.velocity, Color.Coral, orbScale / 200f, 2);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }

                // Full charge dusts
                if (CurrentChargingFrames == OpalStriker.FullChargeFrames)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(tipPosition, 162);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 13f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(2f, 2.5f);
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
            Projectile.Center = armPosition + Projectile.velocity * 15 + new Vector2(0, 3);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;

            // Rumble (only while channeling)
            float rumble = MathHelper.Clamp(CurrentChargingFrames, 0f, OpalStriker.FullChargeFrames);
            if (OwnerCanShoot)
                Projectile.position += Main.rand.NextVector2Circular(rumble / 30f, rumble / 30f);
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
            if (SoundEngine.TryGetActiveSound(OpalChargeSlot, out var ChargeSound))
                ChargeSound?.Stop();
        }

        public override bool? CanDamage() => false;
    }
}
