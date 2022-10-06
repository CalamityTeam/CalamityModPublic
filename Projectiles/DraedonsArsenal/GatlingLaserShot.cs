using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class GatlingLaserShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust = 182;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Gatling Laser");
        }

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
            Lighting.AddLight(Projectile.Center, 0.2f, 0f, 0f);

            float createDustVar = 10f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] > createDustVar)
            {
                Vector2 value7 = new Vector2(5f, 10f);
                Vector2 value8 = Vector2.UnitX * -12f;

                switch ((int)Projectile.ai[0])
                {
                    case 0:
                        int num41 = Dust.NewDust(Projectile.Center, 0, 0, dust, 0f, 0f, 160, default, 0.8f);
                        Main.dust[num41].noGravity = true;
                        Main.dust[num41].position = Projectile.Center;
                        Main.dust[num41].velocity = Projectile.velocity;
                        break;
                    case 1:
                        value8 = -Vector2.UnitY.RotatedBy(24f * 0.1308997f + 0f * MathHelper.Pi) * value7 * 0.5f;
                        int num42 = Dust.NewDust(Projectile.Center, 0, 0, dust, 0f, 0f, 160, default, 0.8f);
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = Projectile.Center + value8;
                        Main.dust[num42].velocity = Projectile.velocity;
                        break;
                    case 2:
                        value8 = -Vector2.UnitY.RotatedBy(24f * 0.1308997f + 1f * MathHelper.Pi) * value7 * 0.5f;
                        int num43 = Dust.NewDust(Projectile.Center, 0, 0, dust, 0f, 0f, 160, default, 0.8f);
                        Main.dust[num43].noGravity = true;
                        Main.dust[num43].position = Projectile.Center + value8;
                        Main.dust[num43].velocity = Projectile.velocity;
                        break;
                    default:
                        break;
                }
            }

            if (Projectile.localAI[0] == createDustVar)
                LaserBurst(1.8f, 3f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 90);
        }

        public override void Kill(int timeLeft)
        {
            int height = 20;
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = height;
            Projectile.Center = Projectile.position;
            Projectile.damage = 0;

            LaserBurst(2.4f, 4.2f); // 60 dusts

            int num3;
            for (int num640 = 0; num640 < 100; num640 = num3 + 1)
            {
                float num641 = 4f;

                int num643 = Dust.NewDust(Projectile.Center, 6, 6, dust, 0f, 0f, 100, default, 1f);
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
                dust2.scale = 0.66f;
                dust2.noGravity = true;

                num3 = num640;
            }
        }

        private void LaserBurst(float speed1, float speed2)
        {
            float angleRandom = 0.05f;

            int num3;
            for (int num53 = 0; num53 < 4; num53 = num3 + 1)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 200, default, 0.9f);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;
                Main.dust[num54].noGravity = true;

                Dust dust2 = Main.dust[num54];
                dust2.velocity *= 3f;
                dust2 = Main.dust[num54];

                num54 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 100, default, 0.66f);
                Main.dust[num54].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * Projectile.width / 2f;

                dust2 = Main.dust[num54];
                dust2.velocity *= 2f;

                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                Main.dust[num54].color = Color.Crimson * 0.5f;

                dust2 = Main.dust[num54];

                num3 = num53;
            }
            for (int num55 = 0; num55 < 2; num55 = num3 + 1)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(Projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);

                int num56 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, dust, dustVel.X, dustVel.Y, 0, default, 1.2f);
                Main.dust[num56].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(Projectile.velocity.ToRotation()) * Projectile.width / 3f;
                Main.dust[num56].noGravity = true;

                Dust dust2 = Main.dust[num56];
                dust2.velocity *= 0.5f;
                dust2 = Main.dust[num56];

                num3 = num55;
            }
        }
    }
}
