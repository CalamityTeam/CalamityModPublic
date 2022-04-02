using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class SupernovaBomb : ModProjectile
    {
        public override string Texture => "CalamityMod/Items/Weapons/Rogue/Supernova";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Supernova Bomb");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 200;
            projectile.tileCollide = true;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            //dust and lighting
            int num298 = Main.rand.NextBool(2) ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                num298 = 269;
            }
            if (Main.rand.NextBool(6))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, num298, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
            Lighting.AddLight(projectile.Center, Main.DiscoR * 0.5f / 255f, Main.DiscoG * 0.5f / 255f, Main.DiscoB * 0.5f / 255f);

            //velocity
            projectile.ai[0] += 1f;
            if (projectile.ai[0] > 10f)
            {
                projectile.ai[0] = 10f;
                if (projectile.velocity.Y == 0f && projectile.velocity.X != 0f)
                {
                    projectile.velocity.X *= 0.97f;
                    if (Math.Abs(projectile.velocity.X) < 0.01f)
                    {
                        projectile.velocity.X = 0f;
                        projectile.netUpdate = true;
                    }
                }
                projectile.velocity.Y += 0.2f;
            }

            //rotation
            projectile.rotation += projectile.velocity.X * 0.1f;

            //stealth strike
            if (projectile.Calamity().stealthStrike && projectile.timeLeft % 8 == 0 && projectile.owner == Main.myPlayer)
                Projectile.NewProjectile(projectile.Center, Vector2.UnitY * 2f, ModContent.ProjectileType<SupernovaHoming>(), (int)(projectile.damage * 0.48), projectile.knockBack, projectile.owner, 0f, 0f);
        }

        public override void Kill(int timeLeft)
        {
            projectile.position = projectile.Center;
            projectile.width = projectile.height = 128;
            projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
            projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
            Main.PlaySound(SoundID.Item14, projectile.position);

            //spawn projectiles
            int projAmt = Main.rand.Next(3, 5);
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SupernovaBoom>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
                for (int i = 0; i < projAmt; i++)
                {
                    Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                    Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<SupernovaSpike>(), (int)(projectile.damage * 0.6), 0f, projectile.owner, 0f, 0f);
                }
                float spread = MathHelper.Pi / 3f;
                double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
                double deltaAngle = spread / 6f;
                double offsetAngle;
                for (int i = 0; i < 3; i++)
                {
                    offsetAngle = startAngle + deltaAngle * (i + i * i) / 2f + 32f * i;
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 2f), (float)(Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<SupernovaHoming>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 2f), (float)(-Math.Cos(offsetAngle) * 2f), ModContent.ProjectileType<SupernovaHoming>(), (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                }
            }

            //dust effects
            int dustType = Main.rand.NextBool(2) ? 107 : 234;
            if (Main.rand.NextBool(4))
            {
                dustType = 269;
            }
            for (int d = 0; d < 5; d++)
            {
                int exo = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType, 0f, 0f, 100, default, 2f);
                Main.dust[exo].velocity *= 3f;
                if (Main.rand.NextBool(2))
                {
                    Main.dust[exo].scale = 0.5f;
                    Main.dust[exo].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                }
            }
            for (int d = 0; d < 9; d++)
            {
                int fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 3f);
                Main.dust[fire].noGravity = true;
                Main.dust[fire].velocity *= 5f;
                fire = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Fire, 0f, 0f, 100, default, 2f);
                Main.dust[fire].velocity *= 2f;
            }

            //gore cloud effects
            CalamityUtils.ExplosionGores(projectile.Center, 3);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.ExoDebuffs();
        }

        public override void OnHitPvp(Player target, int damage, bool crit)
        {
            target.ExoDebuffs();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
