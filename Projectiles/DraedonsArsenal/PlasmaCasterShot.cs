using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
    public class PlasmaCasterShot : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = 107; //160
        private int dust2 = 110; //187

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Bolt");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 6;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.6f, 0f);

            int dustTypeOnTimer = dust1;

            float createDustVar = 10f;
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > createDustVar)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = projectile.position;
                    vector33 -= projectile.velocity * (num447 * 0.25f);
                    int num448 = Dust.NewDust(vector33, 1, 1, dustTypeOnTimer, 0f, 0f, 0, default, 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[num448].velocity *= 0.2f;
                }

                projectile.ai[0] += 1f;
                if (projectile.ai[0] == 48f)
                {
                    projectile.ai[0] = 0f;

                    if (dustTypeOnTimer == dust1)
                        dustTypeOnTimer = dust2;
                    else
                        dustTypeOnTimer = dust1;
                }
                else
                {
                    Vector2 value7 = new Vector2(5f, 10f);

                    for (int num41 = 0; num41 < 2; num41++)
                    {
                        Vector2 value8 = Vector2.UnitX * -12f;
                        value8 = -Vector2.UnitY.RotatedBy(projectile.ai[0] * 0.1308997f + num41 * MathHelper.Pi) * value7 * 0.75f;
                        int num42 = Dust.NewDust(projectile.Center, 0, 0, dust1, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 0.6f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = projectile.Center + value8;
                        Main.dust[num42].velocity = projectile.velocity;
                    }

                    for (int num41 = 0; num41 < 2; num41++)
                    {
                        Vector2 value8 = Vector2.UnitX * -12f;
                        value8 = -Vector2.UnitY.RotatedBy(projectile.ai[0] * 0.1308997f + num41 * MathHelper.Pi) * value7 * 1.5f;
                        int num42 = Dust.NewDust(projectile.Center, 0, 0, dust2, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 0.6f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = projectile.Center + value8;
                        Main.dust[num42].velocity = projectile.velocity;
                    }
                }
            }

            if (projectile.localAI[0] == createDustVar)
                PlasmaBurst(1f, 1.6f);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 180);
            target.AddBuff(BuffID.CursedInferno, 90);
        }

        public override void Kill(int timeLeft)
        {
            int height = 90;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = height;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();

            Main.PlaySound(SoundID.Item93, projectile.Center);

            PlasmaBurst(1.8f, 3.6f); // 60 dusts

            int num3;
            for (int num640 = 0; num640 < 400; num640 = num3 + 1)
            {
                float num641 = 16f;
                if (num640 < 300)
                    num641 = 12f;
                if (num640 < 200)
                    num641 = 8f;
                if (num640 < 100)
                    num641 = 4f;

                int num643 = Dust.NewDust(projectile.Center, 6, 6, Main.rand.Next(2) == 0 ? dust1 : dust2, 0f, 0f, 100, default, 1f);
                float num644 = Main.dust[num643].velocity.X;
                float num645 = Main.dust[num643].velocity.Y;

                if (num644 == 0f && num645 == 0f)
                    num644 = 1f;

                float num646 = (float)Math.Sqrt(num644 * num644 + num645 * num645);
                num646 = num641 / num646;
                num644 *= num646;
                num645 *= num646;

                float scale = 1f;
                switch ((int)num641)
                {
                    case 4:
                        scale = 1.2f;
                        break;
                    case 8:
                        scale = 1.1f;
                        break;
                    case 12:
                        scale = 1f;
                        break;
                    case 16:
                        scale = 0.9f;
                        break;
                    default:
                        break;
                }

                Dust dust = Main.dust[num643];
                dust.velocity *= 0.5f;
                dust.velocity.X = dust.velocity.X + num644;
                dust.velocity.Y = dust.velocity.Y + num645;
                dust.scale = scale;
                dust.noGravity = true;

                num3 = num640;
            }
        }

        private void PlasmaBurst(float speed1, float speed2)
        {
            float angleRandom = 0.35f;

            for (int num53 = 0; num53 < 40; num53++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dust1 : dust2;

                int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 200, default, 1.7f);
                Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;
                Main.dust[num54].noGravity = true;

                Dust dust = Main.dust[num54];
                dust.velocity *= 3f;
                dust = Main.dust[num54];

                num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 100, default, 0.8f);
                Main.dust[num54].position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * projectile.width / 2f;

                dust = Main.dust[num54];
                dust.velocity *= 2f;

                Main.dust[num54].noGravity = true;
                Main.dust[num54].fadeIn = 1f;
                Main.dust[num54].color = Color.Green * 0.5f;

                dust = Main.dust[num54];
            }
            for (int num55 = 0; num55 < 20; num55++)
            {
                float dustSpeed = Main.rand.NextFloat(speed1, speed2);
                Vector2 dustVel = new Vector2(dustSpeed, 0.0f).RotatedBy(projectile.velocity.ToRotation());
                dustVel = dustVel.RotatedBy(-angleRandom);
                dustVel = dustVel.RotatedByRandom(2.0f * angleRandom);
                int randomDustType = Main.rand.Next(2) == 0 ? dust1 : dust2;

                int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, randomDustType, dustVel.X, dustVel.Y, 0, default, 2f);
                Main.dust[num56].position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy(projectile.velocity.ToRotation()) * projectile.width / 3f;
                Main.dust[num56].noGravity = true;

                Dust dust = Main.dust[num56];
                dust.velocity *= 0.5f;
                dust = Main.dust[num56];
            }
        }
    }
}
