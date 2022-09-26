using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GaussPistolShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public float Time
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        public Vector2 OldVelocity;
        public const float MaxChargeRadius = 7f;
        public const float MinEnergyParticleSpeed = 0.25f;
        public const float MaxEnergyParticleSpeed = 4f;
        public const float EnergyChargeRadiusRatio = 1.45f;
        public const float ChargeTime = 30f;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gauss Shot");
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 240;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(OldVelocity);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            OldVelocity = reader.ReadVector2();
        }

        public override void AI()
        {
            Time++;

            if (Time < ChargeTime && OldVelocity == Vector2.Zero)
            {
                OldVelocity = Projectile.velocity;
                Projectile.velocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }
            if (Time >= ChargeTime && OldVelocity != Vector2.Zero)
            {
                Projectile.velocity = OldVelocity;
                OldVelocity = Vector2.Zero;
                Projectile.netUpdate = true;
            }

            float energyRadius = MathHelper.Lerp(MaxChargeRadius * 0.333f, MaxChargeRadius, Utils.GetLerpValue(0f, ChargeTime, Time, true));
            float energySphereSpeedMax = MathHelper.Lerp(MaxEnergyParticleSpeed, MinEnergyParticleSpeed, Utils.GetLerpValue(0f, ChargeTime, Time, true));

            // Charge up dust.
            if (Projectile.velocity.Length() == 0f && !Main.dedServ)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 offset = Main.rand.NextVector2CircularEdge(energyRadius, energyRadius) * 3f;

                    float dustHue = Main.rand.NextFloat(0.15f, 0.3f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 261);
                    dust.velocity = Projectile.DirectionFrom(dust.position) * energyRadius / 3f;
                    dust.color = Main.hslToRgb(dustHue, Main.rand.NextFloat(0.9f, 1f), Main.rand.NextFloat(0.5f, 0.66f));
                    dust.noGravity = true;
                }
            }

            if (!Main.dedServ)
            {
                // To give a bubble effect.
                float xOffsetFactor = MathHelper.Lerp(0.7f, 1.3f, (float)Math.Cos(Time / 15f) * 0.5f + 0.5f);
                float yOffsetFactor = MathHelper.Lerp(0.7f, 1.3f, (float)Math.Sin(Time / 15f) * 0.5f + 0.5f);

                // Idle energy pulse dust.
                for (int i = 0; i < 10; i++)
                {
                    Vector2 offset = Main.rand.NextVector2Circular(energyRadius, energyRadius);
                    offset.X *= xOffsetFactor;
                    offset.Y *= yOffsetFactor;

                    float dustHue = Main.rand.NextFloat(0.15f, 0.3f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 261);
                    dust.velocity = Main.rand.NextVector2Circular(energySphereSpeedMax, energySphereSpeedMax);
                    dust.velocity += Projectile.velocity;
                    dust.color = Main.hslToRgb(dustHue, Main.rand.NextFloat(0.9f, 1f), Main.rand.NextFloat(0.5f, 0.66f));
                    dust.scale = 0.8f;
                    dust.noGravity = true;
                }

                // Ring of dust.
                for (int i = 0; i < 25; i++)
                {
                    float angle = i / 25f * MathHelper.TwoPi;

                    Vector2 offset = angle.ToRotationVector2().RotatedBy(Projectile.velocity.ToRotation()) * energyRadius * 1.5f;
                    offset.X *= -xOffsetFactor;
                    offset.Y *= yOffsetFactor;

                    float dustHue = Main.rand.NextFloat(0.15f, 0.3f);
                    Dust dust = Dust.NewDustPerfect(Projectile.Center + offset, 261);
                    dust.velocity = Vector2.Zero;
                    dust.color = Main.hslToRgb(dustHue, Main.rand.NextFloat(0.9f, 1f), Main.rand.NextFloat(0.5f, 0.66f));
                    dust.scale = 0.5f;
                    dust.noGravity = true;
                }
            }
        }
    }
}
