using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class LaserRifleShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust = 127;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laser Rifle");
        }

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
                Vector2 value7 = new Vector2(5f, 10f);
                Vector2 value8 = Vector2.UnitX * -12f;

                for (int i = 0; i < 2; i++)
                {
                    int num41 = Dust.NewDust(Projectile.Center, 0, 0, dust, 0f, 0f, 160, Projectile.ai[0] == 0f ? default : new Color(255, 255, 0), 2f);
                    Main.dust[num41].noGravity = true;
                    Main.dust[num41].position = Projectile.Center;
                    Main.dust[num41].velocity = Projectile.velocity;
                }
            }

            if (Projectile.localAI[0] == createDustVar)
                LaserBurst(1.8f, 3f);
        }

        public override void Kill(int timeLeft)
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

            for (int num640 = 0; num640 < 100; num640++)
            {
                float num641 = 4f;

                int num643 = Dust.NewDust(Projectile.Center, 6, 6, dust, 0f, 0f, 100, default, 2f);
                float num644 = Main.dust[num643].velocity.X;
                float num645 = Main.dust[num643].velocity.Y;

                if (num644 == 0f && num645 == 0f)
                    num644 = 1f;

                float num646 = (float)Math.Sqrt(num644 * num644 + num645 * num645);
                num646 = num641 / num646;
                num644 *= num646;
                num645 *= num646;

                Dust dust2 = Main.dust[num643];
                dust2.velocity *= 0.5f;
                dust2.velocity.X = dust2.velocity.X + num644;
                dust2.velocity.Y = dust2.velocity.Y + num645;
                dust2.noGravity = true;
            }
        }

        private void LaserBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            for (int num53 = 0; num53 < 20; num53++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 200, default, 2.5f);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[num54].noGravity = true;

                Dust dust2 = Main.dust[num54];
                dust2.velocity *= 3f;
                dust2 = Main.dust[num54];

                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 100, default, 1.9f);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust2 = Main.dust[num54];
                dust2.velocity *= 2f;

                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                Main.dust[num54].color = Color.Orange * 0.5f;

                dust2 = Main.dust[num54];
            }
            for (int num55 = 0; num55 < 10; num55++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 0, default, 3.2f);
                Main.dust[num56].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[num56].noGravity = true;

                Dust dust2 = Main.dust[num56];
                dust2.velocity *= 0.5f;
                dust2 = Main.dust[num56];
            }
        }
    }
}
