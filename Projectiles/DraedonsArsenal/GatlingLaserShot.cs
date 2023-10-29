using System;
using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GatlingLaserShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust = 182;
        private Player Owner => Main.player[Projectile.owner];

        private int Time = 0;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Time++;
            Lighting.AddLight(Projectile.Center, 0.2f, 0f, 0f);
            if (Projectile.timeLeft % 11 == 0 && Time > 12 && Time < 460)
            {
                SparkParticle spark = new SparkParticle(Projectile.Center, Projectile.velocity * 0.01f, false, 6, 1.7f, Color.Red);
                GeneralParticleHandler.SpawnParticle(spark);
            }
            if (Projectile.timeLeft % 4 == 0 && Time > 12 && Time < 460)
            {
                SparkParticle spark2 = new SparkParticle(Projectile.Center, Projectile.velocity * 0.01f, false, 6, 0.4f, Color.White);
                GeneralParticleHandler.SpawnParticle(spark2);
            }
        }

        public override void OnKill(int timeLeft)
        {
            int height = 20;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.damage = 0;

            LaserBurst(2.4f, 4.2f); // 60 dusts

            int inc;
            for (int i = 0; i < 100; i = inc + 1)
            {
                int gatlingDust = Dust.NewDust(Projectile.Center, 6, 6, dust, 0f, 0f, 100, default, 1f);
                float dustVelX = Main.dust[gatlingDust].velocity.X;
                float dustVelY = Main.dust[gatlingDust].velocity.Y;

                if (dustVelX == 0f && dustVelY == 0f)
                    dustVelX = 1f;

                float dustVelocity = (float)Math.Sqrt(dustVelX * dustVelX + dustVelY * dustVelY);
                dustVelocity = 4f / dustVelocity;
                dustVelX *= dustVelocity;
                dustVelY *= dustVelocity;

                Dust dust2 = Main.dust[gatlingDust];
                dust2.velocity *= 0.5f;
                dust2.velocity.X = dust2.velocity.X + dustVelX;
                dust2.velocity.Y = dust2.velocity.Y + dustVelY;
                dust2.scale = 0.66f;
                dust2.noGravity = true;

                inc = i;
            }
        }

        private void LaserBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            int inc;
            for (int j = 0; j < 4; j = inc + 1)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int burstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 200, default, 0.9f);
                Main.dust[burstDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[burstDust].noGravity = true;

                Dust dust2 = Main.dust[burstDust];
                dust2.velocity *= 3f;
                dust2 = Main.dust[burstDust];

                burstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 100, default, 0.66f);
                Main.dust[burstDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust2 = Main.dust[burstDust];
                dust2.velocity *= 2f;

                Main.dust[burstDust].noGravity = true;
                Main.dust[burstDust].fadeIn = 1f;
                Main.dust[burstDust].color = Color.Crimson * 0.5f;

                dust2 = Main.dust[burstDust];

                inc = j;
            }
            for (int k = 0; k < 2; k = inc + 1)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int moreBurstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 0, default, 1.2f);
                Main.dust[moreBurstDust].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[moreBurstDust].noGravity = true;

                Dust dust2 = Main.dust[moreBurstDust];
                dust2.velocity *= 0.5f;
                dust2 = Main.dust[moreBurstDust];

                inc = k;
            }
        }
    }
}
