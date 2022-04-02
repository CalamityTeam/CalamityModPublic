using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class PristineFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        private int dust1 = (int)CalamityDusts.ProfanedFire;
        private int dust2 = ModContent.DustType<HolyFireDust>();

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fire");
        }

        public override void SetDefaults()
        {
            projectile.width = 6;
            projectile.height = 6;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 120;
        }

        public override void AI()
        {
            if (projectile.scale <= 1.5f)
            {
                projectile.scale *= 1.01f;
            }
            Lighting.AddLight(projectile.Center, 1f, 1f, 0.25f);
            if (projectile.timeLeft > 90)
            {
                projectile.timeLeft = 120;
            }

            int dustTypeOnTimer = dust1;

            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 5f)
            {
                for (int num447 = 0; num447 < 2; num447++)
                {
                    Vector2 vector33 = projectile.position;
                    vector33 -= projectile.velocity * ((float)num447 * 0.25f);
                    projectile.alpha = 255;
                    int num448 = Dust.NewDust(vector33, 1, 1, dustTypeOnTimer, 0f, 0f, 0, default, 1f);
                    Main.dust[num448].noGravity = true;
                    Main.dust[num448].position = vector33;
                    Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.013f;
                    Main.dust[num448].velocity *= 0.2f;
                    Main.dust[num448].noLight = true;
                    Main.dust[num448].color = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
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
                        int dustType = num41 == 0 ? dust1 : dust2;
                        Vector2 value8 = Vector2.UnitX * -12f;
                        value8 = -Vector2.UnitY.RotatedBy((double)(projectile.ai[0] * 0.1308997f + (float)num41 * 3.14159274f), default) * value7 * 1.5f;
                        int num42 = Dust.NewDust(projectile.Center, 0, 0, dustType, 0f, 0f, 160, default, 1f);
                        Main.dust[num42].scale = 0.75f;
                        Main.dust[num42].noGravity = true;
                        Main.dust[num42].position = projectile.Center + value8;
                        Main.dust[num42].velocity = projectile.velocity;
                        Main.dust[num42].noLight = true;
                        Main.dust[num42].color = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
                    }
                }
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(ModContent.BuffType<HolyFlames>(), 240);
        }

        public override void Kill(int timeLeft)
        {
            int dustType = Utils.SelectRandom(Main.rand, new int[]
            {
                dust1,
                dust2
            });
            int height = 50;
            float num50 = 1.7f;
            float num51 = 0.8f;
            float num52 = 2f;
            Vector2 value3 = (projectile.rotation - MathHelper.PiOver2).ToRotationVector2();
            Vector2 value4 = value3 * projectile.velocity.Length() * (float)projectile.MaxUpdates;
            projectile.position = projectile.Center;
            projectile.width = projectile.height = height;
            projectile.Center = projectile.position;
            projectile.maxPenetrate = -1;
            projectile.penetrate = -1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.Damage();
            for (int num53 = 0; num53 < 40; num53++)
            {
                int num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 200, default, num50);
                Dust dust = Main.dust[num54];
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                dust.noGravity = true;
                dust.velocity *= 3f;
                dust.velocity += value4 * Main.rand.NextFloat();
                dust.color = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
                num54 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 100, default, num51);
                dust.position = projectile.Center + Vector2.UnitY.RotatedByRandom(MathHelper.Pi) * (float)Main.rand.NextDouble() * (float)projectile.width / 2f;
                dust.velocity *= 2f;
                dust.noGravity = true;
                dust.fadeIn = 1f;
                dust.velocity += value4 * Main.rand.NextFloat();
                dust.color = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
            }
            for (int num55 = 0; num55 < 20; num55++)
            {
                int num56 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, dustType, 0f, 0f, 0, default, num52);
                Dust dust = Main.dust[num56];
                dust.position = projectile.Center + Vector2.UnitX.RotatedByRandom(MathHelper.Pi).RotatedBy((double)projectile.velocity.ToRotation(), default) * (float)projectile.width / 3f;
                dust.noGravity = true;
                dust.velocity *= 0.5f;
                dust.velocity += value4 * (0.6f + 0.6f * Main.rand.NextFloat());
                dust.color = CalamityUtils.ColorSwap(new Color(255, 168, 53), new Color(255, 249, 0), 2f);
            }
        }
    }
}
