using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using ReLogic.Utilities;
using CalamityMod.Items.Accessories;
using CalamityMod.Particles;
using System;
using System.Runtime.Intrinsics;

namespace CalamityMod.Projectiles.Ranged
{
    public class OpalStrikerHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private Player Owner => Main.player[Projectile.owner];
        public SlotId OpalChargeSlot;
        public SlotId OpalChargeLoopSlot;

        private float CurrentChargingFrames = 0f;

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        private float storedVelocity = 12f;
        public const float velocityMultiplier = 1.2f;
        public bool Extradamage;
        public int Time = 0;

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/OpalStriker";

        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            if (CurrentChargingFrames >= 88)
                Extradamage = true;
            else
                Extradamage = false;

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.5f;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                int newdamage =  Projectile.damage * (Extradamage ? 7 : 1);
                SoundEngine.PlaySound(OpalStriker.Fire, Projectile.position);
                CurrentChargingFrames = 0;
                Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * storedVelocity;
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<OpalStrike>(), newdamage, Projectile.knockBack * (Extradamage ? 3 : 1), Projectile.owner);
                Projectile.Kill();
            }
            else
            {
                Player player = Main.player[Projectile.owner];
                if (CurrentChargingFrames >= 10)
                {
                    Particle streak = new ManaDrainStreak(player, Main.rand.NextFloat(0.06f + (CurrentChargingFrames / 180), 0.08f + (CurrentChargingFrames / 180)), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(0.3f * CurrentChargingFrames, 0.3f * CurrentChargingFrames), 0f, Color.Gold, Color.Orange, 7, tipPosition);
                    GeneralParticleHandler.SpawnParticle(streak);
                    Particle orb = new GenericBloom(tipPosition, Projectile.velocity, Color.Orange, CurrentChargingFrames / 135, 2);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(tipPosition, Projectile.velocity, Color.Khaki, CurrentChargingFrames / 200, 2);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
                if (CurrentChargingFrames == 10)
                {
                    OpalChargeSlot = SoundEngine.PlaySound(OpalStriker.Charge, Projectile.position);
                }
                // Start Charging.
                if (CurrentChargingFrames < 90)
                    ++CurrentChargingFrames;

                if (CurrentChargingFrames >= 88f) //78 frames is durration of charge sound
                {
                    Time++;
                    if (CurrentChargingFrames == 88f)
                        OpalChargeLoopSlot = SoundEngine.PlaySound(OpalStriker.ChargeLoop, Projectile.position);
                    if (Time % 120 == 0)
                    {
                        OpalChargeLoopSlot = SoundEngine.PlaySound(OpalStriker.ChargeLoop, Projectile.position);
                    }
                    Item heldItem = Owner.ActiveItem();
                }

                if (CurrentChargingFrames == 88f)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(tipPosition, 162);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 18f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1f, 1.5f);
                        chargefull.noGravity = true;
                    }
                }
            }
            if (SoundEngine.TryGetActiveSound(OpalChargeLoopSlot, out var OpalChargeLoop) && OpalChargeLoop.IsPlaying)
                OpalChargeLoop.Position = Projectile.Center;
            if (SoundEngine.TryGetActiveSound(OpalChargeSlot, out var OpalCharge) && OpalCharge.IsPlaying)
                OpalCharge.Position = Projectile.Center;
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
                if (Projectile.velocity != oldVelocity)
                {
                    Projectile.netSpam = 0;
                    Projectile.netUpdate = true;
                }
            }
            Player player = Main.player[Projectile.owner];
            Projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - Projectile.Size / 2f;
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
            Projectile.position += Main.rand.NextVector2Circular(CurrentChargingFrames / 30f, CurrentChargingFrames / 30f); //rumble features
            
        }

        private void ManipulatePlayerVariables()
        {
            Owner.ChangeDir(Projectile.direction);
            Owner.heldProj = Projectile.whoAmI;
            Owner.itemTime = 2;
            Owner.itemAnimation = 2;
            Owner.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
        }
        public override void Kill(int timeLeft)
        {
            if (SoundEngine.TryGetActiveSound(OpalChargeSlot, out var OpalCharge))
                OpalCharge.Stop();
            if (SoundEngine.TryGetActiveSound(OpalChargeLoopSlot, out var OpalChargeLoop))
                OpalChargeLoop.Stop();

            Main.player[Projectile.owner].SetDummyItemTime(18);
        }

        public override bool? CanDamage() => false;
    }
}
