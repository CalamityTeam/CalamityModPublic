using System;
using System.IO;
using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
using CalamityMod.Sounds;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class StellarTorusSummon : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";

        public Player Owner => Main.player[Projectile.owner];
        public CalamityPlayer ModdedOwner => Owner.Calamity();
        public NPC Target => Projectile.Center.MinionHoming(StellarTorusStaff.EnemyDetectionDistance, Owner);
        public ref float TimerToShoot => ref Projectile.ai[0];
        public ref float RotationInterpolant => ref Projectile.ai[1];
        public ref float AnimationSpeed => ref Projectile.ai[2];
        public int LaserSwingDirection = 0;
        public bool DecidedSwingDirection = false;
        public bool HasReachedSwingRotationStart = false;
        public bool HasShotLaser = false;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 12000;
            ProjectileID.Sets.MinionSacrificable[Type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.minionSlots = 1;
            Projectile.penetrate = -1;

            Projectile.width = Projectile.height = 52;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(LaserSwingDirection);
            writer.Write(DecidedSwingDirection);
            writer.Write(HasReachedSwingRotationStart);
            writer.Write(HasShotLaser);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            LaserSwingDirection = reader.ReadInt32();
            DecidedSwingDirection = reader.ReadBoolean();
            HasReachedSwingRotationStart = reader.ReadBoolean();
            HasShotLaser = reader.ReadBoolean();
        }

        public override void AI()
        {
            CheckMinionExistence();
            DoAnimation();
            Projectile.MinionAntiClump(.1f);

            if (Target is not null)
            {
                TimerToShoot++;

                // When the minion detects a target, before it starts charging it'll line up to the target.
                if (TimerToShoot <= StellarTorusStaff.TimeBeforeCharging)
                {
                    FollowPlayer();
                    Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.DirectionTo(Target.Center).ToRotation(), .05f);
                    AnimationSpeed = 5f;

                    SoundStyle laserCharge = new SoundStyle("CalamityMod/Sounds/Custom/ExoMechs/AresLaserArmCharge") { Volume = .1f, Pitch = .8f, PitchVariance = .2f };
                    SoundEngine.PlaySound(laserCharge, Projectile.Center);
                }

                // When it lines up, the minion will stop following the owner and will stop in place.
                // Now it'll start charging.
                else if ((TimerToShoot > StellarTorusStaff.TimeBeforeCharging) && (TimerToShoot <= (StellarTorusStaff.TimeBeforeCharging + StellarTorusStaff.TimeCharging)))
                {
                    Projectile.velocity *= .95f;
                    AnimationSpeed = Utils.Remap(TimerToShoot, StellarTorusStaff.TimeBeforeCharging, StellarTorusStaff.TimeBeforeCharging + StellarTorusStaff.TimeCharging, 5f, 2f);

                    float scale = Projectile.width * Projectile.scale;
                    float relativeScale = scale / 72f; // The width of the projectile relative to the width of the particle.
                    GenericBloom bloomCharge = new GenericBloom(Projectile.Center,
                        Projectile.velocity,
                        Color.Cyan with { A = 10 },
                        Utils.Remap(TimerToShoot, StellarTorusStaff.TimeBeforeCharging, StellarTorusStaff.TimeBeforeCharging + StellarTorusStaff.TimeCharging, relativeScale * 2f, relativeScale / 2f),
                        (int)StellarTorusStaff  .TimeCharging);
                    GeneralParticleHandler.SpawnParticle(bloomCharge);

                    Vector2 chargeDustSpawn = Projectile.Center + Main.rand.NextVector2Circular(scale * 1.2f, scale * 1.2f);
                    Dust chargeDust = Dust.NewDustPerfect(chargeDustSpawn,
                        307,
                        chargeDustSpawn.DirectionTo(Projectile.Center) * Main.rand.NextFloat(5f, 8f));
                    chargeDust.noGravity = true;
                    chargeDust.scale = chargeDust.velocity.Length() * .15f;
                }

                // And the behaviour just before shooting and while shooting.
                else
                {
                    AnimationSpeed = 2f;

                    // Randomly decided if the minion'll swing the laser "Left -> Right" or "Right -> Left".
                    // ...Or whatever perspective, you get me.
                    if (!DecidedSwingDirection)
                    {
                        LaserSwingDirection = Main.rand.NextBool().ToDirectionInt();
                        DecidedSwingDirection = true;
                        Projectile.netUpdate = true;
                    }

                    // The minion will now point at the direction it has decided.
                    float swingRotStart = Projectile.DirectionTo(Target.Center).ToRotation() + (MathHelper.PiOver4 / 2 * LaserSwingDirection);
                    if (!HasReachedSwingRotationStart)
                    {
                        Projectile.rotation = MathHelper.Lerp(Projectile.rotation, swingRotStart, RotationInterpolant);
                        RotationInterpolant += 1f / 30f;
                        if (RotationInterpolant < 1f)
                            return;

                        HasReachedSwingRotationStart = true;
                        RotationInterpolant = 0f;

                        int sparkAmount = 10;
                        for (int sparkIndex = 0; sparkIndex < sparkAmount; sparkIndex++)
                        {
                            float angle = MathHelper.TwoPi / sparkAmount * sparkIndex;
                            Vector2 velocity = angle.ToRotationVector2().RotatedByRandom(MathHelper.ToRadians(15f)) * Main.rand.NextFloat(10f, 15f);
                            SparkParticle firingSparks = new SparkParticle(Projectile.Center,
                                velocity,
                                true,
                                10,
                                velocity.Length() * .1f,
                                Color.Cyan);
                            GeneralParticleHandler.SpawnParticle(firingSparks);
                        }

                        SoundEngine.PlaySound(CommonCalamitySounds.LaserCannonSound with { Volume = .3f, Pitch = .8f, PitchVariance = .2f }, Projectile.Center);

                        Projectile.netUpdate = true;
                    }

                    // And when it has reached the direction that was decided,
                    // the laser will be shot and the laser will be swong to the reverse rotation that was decided.
                    if (!HasReachedSwingRotationStart)
                        return;

                    ShootLaser();
                    Projectile.rotation = MathHelper.Lerp(swingRotStart, swingRotStart + (MathHelper.PiOver4 * -LaserSwingDirection), RotationInterpolant);
                    RotationInterpolant += 1f / StellarTorusStaff.TimeShooting;

                    Dust shootDust = Dust.NewDustPerfect(Projectile.Center, 307, Main.rand.NextVector2Circular(Main.rand.NextFloat(10f, 15f), Main.rand.NextFloat(10f, 15f)));
                    shootDust.noGravity = true;
                    shootDust.scale = shootDust.velocity.Length() * .2f;

                    // When it completes the full swing: repeat process and reset all targetting variables.
                    if (RotationInterpolant >= 1f)
                        ResetTargettingVariables();
                }
            }
            else
            {
                FollowPlayer();
                Projectile.rotation = (MathF.Sign(Projectile.velocity.X) == 1) ? 0f : MathHelper.Pi;
                AnimationSpeed = 5f;

                ResetTargettingVariables();
            }
        }

        public void CheckMinionExistence()
        {
            Owner.AddBuff(ModContent.BuffType<StellarTorusBuff>(), 2);
            if (Type != ModContent.ProjectileType<StellarTorusSummon>())
                return;

            if (Owner.dead)
                ModdedOwner.StellarTorus = false;
            if (ModdedOwner.StellarTorus)
                Projectile.timeLeft = 2;
        }

        public void DoAnimation()
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter % (1 + (int)AnimationSpeed) == 0)
                Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
        }

        public void FollowPlayer()
        {
            // Teleport to the owner if sufficiently far away.
            if (!Projectile.WithinRange(Owner.Center, StellarTorusStaff.EnemyDetectionDistance))
            {
                Projectile.Center = Owner.Center;
                Projectile.velocity *= 0.3f;
                Projectile.netUpdate = true;
            }

            // If the minion starts to get far, force the minion to go to you.
            else if (!Projectile.WithinRange(Owner.Center, StellarTorusStaff.EnemyDetectionDistance / 3))
            {
                Projectile.velocity = (Owner.Center - Projectile.Center) / 30f;
                Projectile.netUpdate = true;
            }

            // The minion will change directions to you if it's going away from you, meaning it'll just hover around you.
            else if (!Projectile.WithinRange(Owner.Center, StellarTorusStaff.EnemyDetectionDistance / 8))
            {
                Projectile.velocity = (Projectile.velocity * 37f + Projectile.SafeDirectionTo(Owner.Center) * 17f) / 40f;
                Projectile.netUpdate = true;
            }
        }

        public void ShootLaser()
        {
            if (!HasShotLaser && Main.myPlayer == Projectile.owner)
            {
                int laser = Projectile.NewProjectile(Projectile.GetSource_FromThis(),
                    Projectile.Center,
                    Projectile.rotation.ToRotationVector2(),
                    ModContent.ProjectileType<StellarTorusBeam>(),
                    Projectile.damage,
                    Projectile.knockBack,
                    Owner.whoAmI,
                    0f, Projectile.whoAmI, Target.whoAmI);

                if (Main.projectile.IndexInRange(laser))
                    Main.projectile[laser].originalDamage = Projectile.originalDamage;
                
                HasShotLaser = true;
                Projectile.netUpdate = true;
            }
        }

        public void ResetTargettingVariables()
        {
            TimerToShoot = 0f;
            LaserSwingDirection = 0;
            RotationInterpolant = 0f;
            DecidedSwingDirection = false;
            HasReachedSwingRotationStart = false;
            HasShotLaser = false;
            Projectile.netUpdate = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            Rectangle frame = texture.Frame(1, Main.projFrames[Type], 0, Projectile.frame);
            Vector2 origin = frame.Size() * 0.5f;

            Main.EntitySpriteDraw(texture, drawPosition, frame, Projectile.GetAlpha(lightColor), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        public override bool? CanDamage() => false;
    }
}
