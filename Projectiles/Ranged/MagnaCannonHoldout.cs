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
    public class MagnaCannonHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private Player Owner => Main.player[Projectile.owner];
        public SlotId MagnaChargeSlot;
        public SlotId MagnaChargeLoopSlot;
        public SlotId MagnaChargeFirstLoopSlot;

        private float CurrentChargingFrames = 0f;
        public int Fullchargesoundframes = 41;

        private ref float ShotsLoaded => ref Projectile.ai[1]; //arrowsloaded

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        private float storedVelocity = 12f;
        public const float velocityMultiplier = 1.2f;
        public bool Extradamage;
        public int Time = 0;
        public int SoundSpamFix = 0;
        public bool FirstLoop = true;
        public bool FullCharge = false;
        public int Aftershot = 30;

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/MagnaCannon";

        public override void SetDefaults()
        {
            Projectile.width = 58;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead) // destroy the holdout if the player dies
            {
                Projectile.Kill();
                return;
            }

            if (CurrentChargingFrames >= 88)
                Extradamage = true;
            else
                Extradamage = false;

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.9f;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {
                if (Aftershot == 30)
                {
                    if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var MagnaCharge2))
                        MagnaCharge2.Stop();
                    if (SoundEngine.TryGetActiveSound(MagnaChargeLoopSlot, out var MagnaChargeLoop2))
                        MagnaChargeLoop2.Stop();
                    if (SoundEngine.TryGetActiveSound(MagnaChargeLoopSlot, out var MagnaChargeFirstLoop2))
                        MagnaChargeFirstLoop2.Stop();

                    if (SoundSpamFix >= (FullCharge ? 4 : 5)) //Shoot faster if fully charged
                        SoundSpamFix = 0;

                    if (ShotsLoaded <= 0f)
                    {
                        --Aftershot;
                    }
                    Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * storedVelocity;
                    if (SoundSpamFix == 0)
                    {
                        for (int i = 0; i <= 3; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(tipPosition, 187, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f), 0, default, Main.rand.NextFloat(1.5f, 2.3f));
                            dust.noGravity = true;
                        }
                        SoundEngine.PlaySound(MagnaCannon.Fire, Projectile.position);
                        --ShotsLoaded;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity.RotatedByRandom(MathHelper.ToRadians(9f)), ModContent.ProjectileType<MagnaShot>(), Projectile.damage, Projectile.knockBack * (Extradamage ? 3 : 1), Projectile.owner);
                    }

                    SoundSpamFix++;

                    CurrentChargingFrames = 0;
                }
                else
                    --Aftershot;
                
                if (Aftershot == 0)
                    Projectile.Kill();
            }
            else
            {
                if (CurrentChargingFrames == 0)
                    CurrentChargingFrames++;

                if (CurrentChargingFrames % 9 == 0)
                    ShotsLoaded++;

                if (CurrentChargingFrames >= 10)
                {
                    if (CurrentChargingFrames < 136)
                    {
                        Particle streak = new ManaDrainStreak(player, Main.rand.NextFloat(0.06f + (CurrentChargingFrames / 180), 0.08f + (CurrentChargingFrames / 180)), Main.rand.NextVector2CircularEdge(2f, 2f) * Main.rand.NextFloat(0.3f * CurrentChargingFrames, 0.3f * CurrentChargingFrames), 0f, Color.White, Color.Aqua, 7, tipPosition);
                        GeneralParticleHandler.SpawnParticle(streak);
                    }
                    Particle orb = new GenericBloom(tipPosition, Projectile.velocity, Color.DarkBlue, CurrentChargingFrames / 135, 2);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(tipPosition, Projectile.velocity, Color.Aqua, CurrentChargingFrames / 200, 2);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
                if (CurrentChargingFrames == 10)
                {
                    MagnaChargeSlot = SoundEngine.PlaySound(MagnaCannon.ChargeStart, Projectile.position);
                }
                // Start Charging.
                if (CurrentChargingFrames < 138)
                {
                    ++CurrentChargingFrames;
                }

                if (CurrentChargingFrames >= 136f) //126 frames is durration of charge sound
                {
                    Fullchargesoundframes--;
                    Time++;
                    if (CurrentChargingFrames == 136f)
                    {
                        SoundEngine.PlaySound(MagnaCannon.ChargeFull, Projectile.position);
                        ShotsLoaded = 20;
                        FullCharge = true;
                    }
                    if (Fullchargesoundframes <= 0)
                    {
                        if (FirstLoop)
                        {
                            MagnaChargeFirstLoopSlot = MagnaChargeLoopSlot = SoundEngine.PlaySound(MagnaCannon.ChargeLoop, Projectile.position);
                            FirstLoop = false;
                        }
                        if (Time % 154 == 0)
                        {
                            MagnaChargeLoopSlot = SoundEngine.PlaySound(MagnaCannon.ChargeLoop, Projectile.position);
                        }
                    }
                }

                if (CurrentChargingFrames == 136f)
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
            if (SoundEngine.TryGetActiveSound(MagnaChargeLoopSlot, out var MagnaChargeLoop) && MagnaChargeLoop.IsPlaying)
                MagnaChargeLoop.Position = Projectile.Center;
            if (SoundEngine.TryGetActiveSound(MagnaChargeFirstLoopSlot, out var MagnaChargeFirstLoop) && MagnaChargeFirstLoop.IsPlaying)
                MagnaChargeFirstLoop.Position = Projectile.Center;
            if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var MagnaCharge) && MagnaCharge.IsPlaying)
                MagnaCharge.Position = Projectile.Center;
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
            Player player = Main.player[Projectile.owner];
            Projectile.Center = player.RotatedRelativePoint(player.MountedCenter, true) + Projectile.velocity * 20;
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
            if (SoundEngine.TryGetActiveSound(MagnaChargeSlot, out var MagnaCharge))
                MagnaCharge.Stop();
            if (SoundEngine.TryGetActiveSound(MagnaChargeLoopSlot, out var MagnaChargeLoop))
                MagnaChargeLoop.Stop();
            if (SoundEngine.TryGetActiveSound(MagnaChargeLoopSlot, out var MagnaChargeFirstLoop))
                MagnaChargeFirstLoop.Stop();
            CurrentChargingFrames = 0;
        }

        public override bool? CanDamage() => false;
    }
}
