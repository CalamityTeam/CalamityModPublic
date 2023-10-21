using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class LaserRifleShot : ModProjectile, ILocalizedModType
    {
        public new string LocalizationCategory => "Projectiles.Misc";
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust = 127;

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.penetrate = 1;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0.2f, 0.1f, 0f);

            float createDustVar = 12f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar)
            {
                for (int i = 0; i < 2; i++)
                {
                    int laserDust = Dust.NewDust(Projectile.Center, 0, 0, dust, 0f, 0f, 160, Projectile.ai[0] == 0f ? default : new Color(255, 255, 0), 2f);
                    Main.dust[laserDust].noGravity = true;
                    Main.dust[laserDust].position = Projectile.Center;
                    Main.dust[laserDust].velocity = Projectile.velocity;
                }
            }

            if (Projectile.localAI[0] == createDustVar)
                LaserBurst(1.8f, 3f);
        }

        public override void OnKill(int timeLeft)
        {
            int height = 60;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 10;
            Projectile.Damage();

            LaserBurst(2.4f, 4.2f); // 60 dusts

            for (int i = 0; i < 100; i++)
            {
                int killDust = Dust.NewDust(Projectile.Center, 6, 6, dust, 0f, 0f, 100, default, 2f);
                float killDustX = Main.dust[killDust].velocity.X;
                float killDustY = Main.dust[killDust].velocity.Y;

                if (killDustX == 0f && killDustY == 0f)
                    killDustX = 1f;

                float killDustVelocity = (float)Math.Sqrt(killDustX * killDustX + killDustY * killDustY);
                killDustVelocity = 4f / killDustVelocity;
                killDustX *= killDustVelocity;
                killDustY *= killDustVelocity;

                Dust dust2 = Main.dust[killDust];
                dust2.velocity *= 0.5f;
                dust2.velocity.X = dust2.velocity.X + killDustX;
                dust2.velocity.Y = dust2.velocity.Y + killDustY;
                dust2.noGravity = true;
            }
        }

        private void LaserBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            for (int i = 0; i < 20; i++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int burstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 200, default, 2.5f);
                Main.dust[burstDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[burstDust].noGravity = true;

                Dust dust2 = Main.dust[burstDust];
                dust2.velocity *= 3f;
                dust2 = Main.dust[burstDust];

                burstDust = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 100, default, 1.9f);
                Main.dust[burstDust].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust2 = Main.dust[burstDust];
                dust2.velocity *= 2f;

                Main.dust[burstDust].noGravity = true;
                Main.dust[burstDust].fadeIn = 1f;
                Main.dust[burstDust].color = Color.Orange * 0.5f;

                dust2 = Main.dust[burstDust];
            }
            for (int j = 0; j < 10; j++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int burstDust2 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 0, default, 3.2f);
                Main.dust[burstDust2].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[burstDust2].noGravity = true;

                Dust dust2 = Main.dust[burstDust2];
                dust2.velocity *= 0.5f;
                dust2 = Main.dust[burstDust2];
            }
        }
    }
}
