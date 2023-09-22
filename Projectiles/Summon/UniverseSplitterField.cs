using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using CalamityMod.Sounds;

namespace CalamityMod.Projectiles.Summon
{
    public class UniverseSplitterField : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Summon";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public float DustRadius = 0f;
        public const float DustChargeTime = 30f;
        public const float DustMinRadius = 0f;
        public const float DustMaxRadius = 90f;
        public const int SpiralPrecision = 25;
        public const int SpiralRings = 5;
        public const int TimeLeft = 720;

        public const float SmallBeamAngleMax = MathHelper.TwoPi / 15f;

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 200;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.minion = true;
            Projectile.minionSlots = 0f;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = TimeLeft;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(DustRadius);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            DustRadius = reader.ReadSingle();
        }

        public override void AI()
        {
            Timer++;
            if (Timer < DustChargeTime)
            {
                DustRadius = MathHelper.Lerp(DustMinRadius, DustMaxRadius, Timer / DustChargeTime);
            }
            else
            {
                DustRadius = DustMaxRadius + (float)Math.Sin(Timer / 50f) * 16f;
            }
            if (!Main.dedServ)
            {
                GenerateIdleDust();
            }
            SpawnLasers();
        }

        public void GenerateIdleDust()
        {
            // Generate a dust ring that pulsates
            for (int i = 0; i < 80; i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center + (i / 80f * MathHelper.TwoPi).ToRotationVector2() * DustRadius, 247);
                dust.velocity = Vector2.Zero;
                dust.scale = 1.2f;
                dust.noGravity = true;
            }

            // Spirals that spin around
            for (int i = 0; i < SpiralPrecision; i++)
            {
                for (int direction = -1; direction <= 1; direction += 2)
                {
                    for (int j = 0; j < SpiralRings; j++)
                    {
                        Dust dust = Dust.NewDustPerfect(Projectile.Center +
                            Vector2.UnitY.RotatedBy(Timer / SpiralPrecision * direction).RotatedBy(j / (float)SpiralRings * MathHelper.TwoPi).RotatedBy(i / (float)SpiralPrecision * MathHelper.TwoPi / SpiralRings * direction) *
                            DustRadius * i / SpiralPrecision, 261);
                        dust.velocity = Vector2.Zero;
                        dust.scale = 0.7f;
                        dust.noGravity = true;
                    }
                }
            }

            // Outward expansion of dust
            bool firingGiantLaserBeam = Timer > TimeLeft - UniverseSplitterHugeBeam.TimeLeft;
            for (int i = 0; i < (firingGiantLaserBeam ? 30 : 16); i++)
            {
                Dust dust = Dust.NewDustPerfect(Projectile.Center, 247);
                dust.velocity = Main.rand.NextVector2Circular(8f, 8f) * (firingGiantLaserBeam ? 1.6f : 1f);
                dust.noGravity = true;
                dust.scale = 1.25f;
            }

            // Energy dust that appears before the giant beam does and remains
            if (Timer > TimeLeft - UniverseSplitterHugeBeam.TimeLeft - 120f)
            {
                float outwardCircleRadius = MathHelper.Lerp(0f, DustRadius * 1.2f, MathHelper.Clamp((Timer - (TimeLeft - UniverseSplitterHugeBeam.TimeLeft - 120f)) / 40f, 0f, 1f));
                for (int i = 0; i < 95; i++)
                {
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + (i / 95f * MathHelper.TwoPi).ToRotationVector2() * outwardCircleRadius, 247);
                    dust.scale = 1.2f;
                    dust.noGravity = true;
                    dust.velocity = Main.rand.NextBool(7) ? Projectile.DirectionFrom(dust.position) * 6f : Vector2.Zero;
                }
            }
        }

        public void SpawnLasers()
        {
            // Create small beams sometimes
            if (Timer > 120f &&
                Timer < TimeLeft - UniverseSplitterHugeBeam.TimeLeft &&
                Timer % 60f == 0f)
            {
                SoundEngine.PlaySound(CommonCalamitySounds.PlasmaBoltSound, Projectile.Center);
                if (Main.myPlayer == Projectile.owner)
                {
                    Vector2 offset = new(Main.rand.NextFloat(-800f, 800f), -1460f);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                                             Projectile.Center + offset,
                                             -Vector2.Normalize(offset),
                                             ModContent.ProjectileType<UniverseSplitterSmallBeam>(),
                                             Projectile.damage,
                                             Projectile.knockBack,
                                             Projectile.owner,
                                             (-Vector2.Normalize(offset)).ToRotation());
                }
            }
            // Summon a giant beam
            if (Timer == TimeLeft - UniverseSplitterHugeBeam.TimeLeft && Main.myPlayer == Projectile.owner)
            {
                SoundEngine.PlaySound(SoundID.Zombie104, Projectile.Center);
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), 
                                         Projectile.Center + Vector2.UnitY * -UniverseSplitterHugeBeam.MaximumLength / 2f,
                                         Vector2.UnitY,
                                         ModContent.ProjectileType<UniverseSplitterHugeBeam>(),
                                         Projectile.damage,
                                         Projectile.knockBack,
                                         Projectile.owner,
                                         0f);
            }
        }

        public override bool? CanDamage() => false;
    }
}
