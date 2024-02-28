using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.CalamityUtils;

namespace CalamityMod.Projectiles.Rogue
{
    public class FishboneBoomerangProjectile : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Rogue";
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/FishboneBoomerang";

        public static int ChargeupTime = 20;
        public static int Lifetime = 240;
        public float OverallProgress => 1 - Projectile.timeLeft / (float)Lifetime;
        public float ThrowProgress => 1 - Projectile.timeLeft / (float)(Lifetime);
        public float ChargeProgress => 1 - (Projectile.timeLeft - Lifetime) / (float)(ChargeupTime);

        public Player Owner => Main.player[Projectile.owner];
        public ref float Returning => ref Projectile.ai[0];
        public ref float Bouncing => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = Lifetime + ChargeupTime;
            Projectile.DamageType = RogueDamageClass.Instance;
            Projectile.ignoreWater = true;
        }

        public override bool ShouldUpdatePosition()
        {
            return ChargeProgress >= 1;
        }

        public override bool? CanDamage()
        {
            //We don't want the anticipation to deal damage.
            if (ChargeProgress < 1)
                return false;

            return base.CanDamage();
        }

        //Swing animation keys
        public CurveSegment pullback = new CurveSegment(EasingType.PolyOut, 0f, 0f, MathHelper.PiOver4 * -1.2f, 2);
        public CurveSegment throwout = new CurveSegment(EasingType.PolyOut, 0.7f, MathHelper.PiOver4 * -1.2f, MathHelper.PiOver4 * 1.2f + MathHelper.PiOver2, 3);
        internal float ArmAnticipationMovement() => PiecewiseAnimation(ChargeProgress, new CurveSegment[] { pullback, throwout });

        public override void AI()
        {
            //Anticipation animation. Make the player look like theyre holding the fish skeleton
            if (ChargeProgress < 1)
            {
                float armRotation = ArmAnticipationMovement() * Owner.direction;

                Owner.heldProj = Projectile.whoAmI;

                Projectile.Center = Owner.MountedCenter + Vector2.UnitY.RotatedBy(armRotation * Owner.gravDir) * -40f * Owner.gravDir;
                Projectile.rotation = (-MathHelper.PiOver2 + armRotation) * Owner.gravDir;

                Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, MathHelper.Pi + armRotation);

                return;
            }

            //Play the throw sound when the throw ACTUALLY BEGINS.
            //Additionally, make the projectile collide and set its speed and velocity
            if (Projectile.timeLeft == Lifetime)
            {
                SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
                Projectile.Center = Owner.MountedCenter + Projectile.velocity * 12f;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 17.5f;
                Projectile.tileCollide = true;
            }

            //Boomerang spinny sounds
            if (Projectile.soundDelay <= 0)
            {
                SoundEngine.PlaySound(SoundID.Item7, Projectile.Center);
                Projectile.soundDelay = 8;
            }

            Projectile.rotation += (MathHelper.PiOver4 / 4f + MathHelper.PiOver4 / 2f * Math.Clamp(ThrowProgress * 2f, 0, 1)) * Math.Sign(Projectile.velocity.X);

            if (Projectile.velocity.Length() < 2f && Bouncing == 0f)
            {
                Returning = 1f;
                Projectile.numHits = 0;
            }

            if (Returning == 0f && Bouncing == 0f && Projectile.velocity.Length() > 2f)
            {
                Projectile.velocity *= 0.97f;
            }

            if (Returning == 1f && Projectile.velocity.Length() < 17f)
            {
                Projectile.velocity *= 1.1f;
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 dustPos = Projectile.Center + (i * MathHelper.Pi + Projectile.rotation + MathHelper.PiOver2).ToRotationVector2() * 14f;
                Dust dust = Dust.NewDustPerfect(dustPos, 176, (i * MathHelper.Pi + Projectile.rotation * Math.Sign(Projectile.velocity.X)).ToRotationVector2() * 3f);
                dust.noGravity = true;
            }


            if (Returning == 1f)
            {
                Projectile.tileCollide = false;
                //Aim back at the player
                Projectile.velocity = Projectile.velocity.Length() * (Owner.MountedCenter - Projectile.Center).SafeNormalize(Vector2.One);

                if ((Projectile.Center - Owner.MountedCenter).Length() < 24f)
                {
                    Projectile.Kill();
                }

                if (Projectile.numHits >= 5)
                {
                    ImpactEffects();
                    Projectile.Kill();
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            ImpactEffects();

            float streakRotation;
            for (int i = 0; i < 5; i++)
            {
                streakRotation = Main.rand.NextFloat(MathHelper.TwoPi);

                for (int j = 0; j < 4; j++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + streakRotation.ToRotationVector2() * (2f + 0.4f * j), 176, streakRotation.ToRotationVector2() * (0.6f * j + 3f), Scale: 1.4f);
                    dust.noGravity = true;
                }
            }

            if (((Projectile.Calamity().stealthStrike && Projectile.numHits > 2) || !Projectile.Calamity().stealthStrike) && Returning != 1f)
            {
                Projectile.velocity *= 0.3f;
                Returning = 1f;
            }

            else
            {
                //Retarget
                NPC newTarget = null;
                float closestNPCDistance = 10000f;
                float targettingDistance = 400f;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i == target.whoAmI)
                        continue;

                    if (Main.npc[i].CanBeChasedBy(Projectile))
                    {
                        float potentialNewDistance = (Projectile.Center - Main.npc[i].Center).Length();
                        if (potentialNewDistance < targettingDistance && potentialNewDistance < closestNPCDistance)
                        {
                            closestNPCDistance = potentialNewDistance;
                            newTarget = Main.npc[i];
                        }
                    }
                }

                if (newTarget == null)
                {
                    Projectile.velocity *= 0.3f;
                    Returning = 1f;
                    return;
                }

                Bouncing = 1f;
                Projectile.velocity = 15f * (newTarget.Center - Projectile.Center).SafeNormalize(Vector2.One);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            ImpactEffects();
            Projectile.velocity = Projectile.oldVelocity.Length() * 0.3f * (Owner.MountedCenter - Projectile.Center).SafeNormalize(Vector2.One);
            Returning = 1f;
            return false;
        }

        public void ImpactEffects()
        {
            //If the boomerang somehow hits 10 enemies, make it start doing drum sounds
            SoundStyle bonkSound = Projectile.numHits < 10 ? SoundID.DD2_SkeletonHurt with { Volume = SoundID.DD2_SkeletonHurt.Volume * 0.8f, Pitch = SoundID.DD2_SkeletonHurt.Pitch + 0.1f * Projectile.numHits } :
                 Utils.SelectRandom(Main.rand, new SoundStyle[]
                {
                SoundID.DrumClosedHiHat,
                SoundID.DrumCymbal1,
                SoundID.DrumCymbal2,
                SoundID.DrumKick,
                SoundID.DrumTamaSnare,
                SoundID.DrumTomHigh,
                SoundID.DrumHiHat
                });

            SoundEngine.PlaySound(bonkSound, Projectile.Center);
            int goreNumber = Main.rand.Next(4);

            for (int i = 0; i < goreNumber; i++)
            {
                int goreID = Main.rand.NextBool() ? 266 : Main.rand.NextBool() ? 971 : 972;
                Gore bone = Gore.NewGorePerfect(Projectile.GetSource_FromAI(), Projectile.position, Projectile.velocity * 0.2f + Main.rand.NextVector2Circular(5f, 5f), goreID);
                bone.scale = Main.rand.NextFloat(0.6f, 1f) * (goreID == 972 ? 0.7f : 1f); //Shrink the larger bones
                bone.type = goreID; //Gotta do that or else itll spawn gores from the general pool :(
            }
        }
    }
}
