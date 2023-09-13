using System;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ArcNovaDiffuserHoldout : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Ranged";
        private Player Owner => Main.player[Projectile.owner];
        public SlotId NovaChargeStartSlot;
        public SlotId NovaChargeLoopSlot;

        public float CurrentChargingFrames = 0f;

        private ref float ShotsLoaded => ref Projectile.ai[1]; //arrowsloaded

        private bool OwnerCanShoot => Owner.channel && !Owner.noItems && !Owner.CCed;
        private float storedVelocity = 12f;
        public const float velocityMultiplier = 1.2f;
        public int Time = 0;
        public int SoundSpamFix = 0;
        public bool FirstLoop = true;
        public bool ChargeLV1 = false;
        public bool ChargeLV2 = false;
        public float Aftershot = ArcNovaDiffuser.AftershotCooldownFrames;
        public int SoundVariable = 5;
        public bool NovaRing = false;
        public bool ThrowItBack = false;
        public float Recoil = 0;
        public bool PostFireEnergy = true;

        public override string Texture => "CalamityMod/Items/Weapons/Ranged/ArcNovaDiffuser";

        public override void SetDefaults()
        {
            Projectile.width = 72;
            Projectile.height = 38;
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

            Vector2 armPosition = Owner.RotatedRelativePoint(Owner.MountedCenter, true);
            Vector2 tipPosition = armPosition + Projectile.velocity * Projectile.width * 0.95f;

            // Fire if the owner stops channeling or otherwise cannot use the weapon.
            if (!OwnerCanShoot)
            {

                if (Aftershot == ArcNovaDiffuser.AftershotCooldownFrames)
                {
                    if (SoundEngine.TryGetActiveSound(NovaChargeStartSlot, out var NovaCharge2))
                        NovaCharge2.Stop();
                    if (SoundEngine.TryGetActiveSound(NovaChargeLoopSlot, out var NovaChargeLoop2))
                        NovaChargeLoop2.Stop();

                    if (ChargeLV2) // Big Shot Mode
                    {
                        SoundEngine.PlaySound(ArcNovaDiffuser.BigShot, Projectile.position);
                        CurrentChargingFrames = 0;
                        ShotsLoaded = 0;
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * storedVelocity;
                        int charge2Damage = Projectile.damage * 25;
                        float charge2KB = Projectile.knockBack * 3f;
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, shootVelocity, ModContent.ProjectileType<NovaChargeShot>(), charge2Damage, charge2KB, Projectile.owner);
                        Main.player[Projectile.owner].Calamity().GeneralScreenShakePower = 5;
                        for (int i = 0; i <= 20; i++)
                        {
                            Dust dust = Dust.NewDustPerfect(tipPosition, 107, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.2f, 1.2f), 0, default, Main.rand.NextFloat(1.5f, 2.3f));
                            dust.noGravity = true;
                        }
                        ThrowItBack = true;
                        Recoil = 10;
                        Aftershot -= 0.5f;
                    }
                    else //Rapid Fire Mode
                    {
                        if (SoundSpamFix >= (ChargeLV1 ? 7 : 8)) //Shoot faster if fully charged
                            SoundSpamFix = 0;
                        
                        if (SoundSpamFix == (ChargeLV1 ? 6 : 7))//Recoil should be 1 frame lower than above frames
                            ThrowItBack = false;

                        if (ShotsLoaded <= 0f)
                        {
                            --Aftershot;
                        }
                        Vector2 shootVelocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * storedVelocity;
                        if (SoundSpamFix == 0)
                        {
                            for (int i = 0; i <= 3; i++)
                            {
                                Dust dust = Dust.NewDustPerfect(tipPosition, 107, shootVelocity.RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(0.9f, 1.2f), 0, default, Main.rand.NextFloat(1.3f, 1.7f));
                                dust.noGravity = true;
                            }
                            ThrowItBack = true;
                            Recoil = 16;
                            Vector2 fireVec = shootVelocity.RotatedByRandom(MathHelper.ToRadians(2f));
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), tipPosition, fireVec, ModContent.ProjectileType<NovaShot>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                            SoundEngine.PlaySound(ArcNovaDiffuser.SmallShot, Projectile.position);
                            --ShotsLoaded;
                        }
                        Recoil += 1;
                        SoundSpamFix++;

                        CurrentChargingFrames = 0;
                    } 
                }
                else
                {
                    if (ChargeLV2)
                    {
                        if (Recoil >= 30)
                        {
                            ThrowItBack = false;
                        }
                        Recoil += 2;
                        Aftershot -= 0.5f;
                    }
                    else
                        --Aftershot;
                }

                if (ShotsLoaded == 0 && PostFireEnergy)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        SparkParticle spark2 = new SparkParticle((tipPosition - Projectile.velocity * 3) + Main.rand.NextVector2Circular(10, 10), -Projectile.velocity * Main.rand.NextFloat(9.1f, 17.8f), false, Main.rand.Next(9, 12), Main.rand.NextFloat(0.2f, 0.3f), Main.rand.NextBool(4) ? Color.Chartreuse : Color.Lime);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    PostFireEnergy = false;
                }

                if (Aftershot <= 0)
                    Projectile.Kill();
            }
            else
            {
                if (CurrentChargingFrames == 0)
                {
                    CurrentChargingFrames++;
                }

                if (CurrentChargingFrames % 13 == 0 && !ChargeLV1) //every 13 frames get another shot
                    ShotsLoaded++;

                if (CurrentChargingFrames >= 10)
                {
                    for (int i = 0; i < (ChargeLV2 ? 4 : ChargeLV1 ? 3 : 2); i++)
                    {
                        SparkParticle spark2 = new SparkParticle((tipPosition -Projectile.velocity * 4) + Main.rand.NextVector2Circular(12, 12), -Projectile.velocity * Main.rand.NextFloat(16.1f, 30.8f), false, Main.rand.Next(2, 7), Main.rand.NextFloat(CurrentChargingFrames / 350, CurrentChargingFrames / 270), Main.rand.NextBool(4) ? Color.Chartreuse : Color.Lime);
                        GeneralParticleHandler.SpawnParticle(spark2);
                    }
                    Particle orb = new GenericBloom(tipPosition, Projectile.velocity, Color.Lime, CurrentChargingFrames / 270, 2);
                    GeneralParticleHandler.SpawnParticle(orb);
                    Particle orb2 = new GenericBloom(tipPosition, Projectile.velocity, Color.White, CurrentChargingFrames / 400, 2);
                    GeneralParticleHandler.SpawnParticle(orb2);
                }
                if (CurrentChargingFrames == 10)
                {
                    NovaChargeStartSlot = SoundEngine.PlaySound(ArcNovaDiffuser.ChargeStart, Projectile.position);
                }
                // Start Charging.
                if (CurrentChargingFrames < ArcNovaDiffuser.Charge2Frames + 2) //308 is when you reach LV2 charge
                {
                    if (ChargeLV1)
                        CurrentChargingFrames += 2;
                    else
                        CurrentChargingFrames++;
                }

                if (CurrentChargingFrames >= ArcNovaDiffuser.Charge1Frames) //146 frames is durration of charge sound
                {
                    Time++;
                    if (CurrentChargingFrames == ArcNovaDiffuser.Charge1Frames)
                    {
                        SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLV1, Projectile.position);
                        ShotsLoaded = 15;
                        ChargeLV1 = true;
                    }
                    if (FirstLoop)
                    {
                        NovaChargeLoopSlot = SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLoop, Projectile.position);
                        FirstLoop = false;
                    }
                    if (Time % ArcNovaDiffuser.ChargeLoopSoundFrames == 0) //Loop sound is 151 frames long
                    {
                        NovaChargeLoopSlot = SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLoop, Projectile.position);
                    }
                }
                
                if (CurrentChargingFrames >= ArcNovaDiffuser.Charge2Frames) //Stuff for reaching charge LV2
                {
                    if (CurrentChargingFrames == ArcNovaDiffuser.Charge2Frames)
                    {
                        SoundEngine.PlaySound(ArcNovaDiffuser.ChargeLV2, Projectile.position);
                        ChargeLV2 = true;
                    }
                }

                if (CurrentChargingFrames == ArcNovaDiffuser.Charge1Frames)
                {
                    for (int i = 0; i < 36; i++)
                    {
                        Dust chargefull = Dust.NewDustPerfect(tipPosition, 107);
                        chargefull.velocity = (MathHelper.TwoPi * i / 36f).ToRotationVector2() * 8f + Owner.velocity;
                        chargefull.scale = Main.rand.NextFloat(1f, 1.3f);
                        chargefull.noGravity = true;
                    }
                }
                if (CurrentChargingFrames == ArcNovaDiffuser.Charge2Frames)
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
            if (SoundEngine.TryGetActiveSound(NovaChargeLoopSlot, out var NovaChargeLoop) && NovaChargeLoop.IsPlaying)
                NovaChargeLoop.Position = Projectile.Center;
            if (SoundEngine.TryGetActiveSound(NovaChargeStartSlot, out var NovaCharge) && NovaCharge.IsPlaying)
                NovaCharge.Position = Projectile.Center;
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
            Projectile.Center = (player.RotatedRelativePoint(player.MountedCenter, true) + Projectile.velocity * (ThrowItBack ? Recoil : 30)) + new Vector2 (0, 7);
            Projectile.rotation = Projectile.velocity.ToRotation() + (Projectile.spriteDirection == -1 ? MathHelper.Pi : 0f);
            Projectile.spriteDirection = Projectile.direction;
            Projectile.timeLeft = 2;
            player.ChangeDir(Projectile.direction);
            player.heldProj = Projectile.whoAmI;
            player.itemTime = 2;
            player.itemAnimation = 2;
            player.itemRotation = (float)Math.Atan2(Projectile.velocity.Y * Projectile.direction, Projectile.velocity.X * Projectile.direction);
            Projectile.position += Main.rand.NextVector2Circular(CurrentChargingFrames / 70f, CurrentChargingFrames / 70f); //rumble features
            
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
            if (SoundEngine.TryGetActiveSound(NovaChargeStartSlot, out var NovaCharge))
                NovaCharge.Stop();
            if (SoundEngine.TryGetActiveSound(NovaChargeLoopSlot, out var NovaChargeLoop))
                NovaChargeLoop.Stop();
            CurrentChargingFrames = 0;
        }

        public override bool? CanDamage() => false;
    }
}
