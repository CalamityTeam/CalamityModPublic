using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace CalamityMod.Projectiles.Magic
{
    public class StratusSphereProj : ModProjectile
    {
        int roundsGone = 0;
        int dust_nut = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stratus Sphere");
            Main.projFrames[projectile.type] = 6;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.hostile = false;
            projectile.friendly = true;
            projectile.aiStyle = -1;
            projectile.width = 314;
            projectile.height = 198;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.knockBack = 0;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 10;
            projectile.magic = true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(230, 230, 255, projectile.alpha);
        }

        public override void AI()
        {
            projectile.ai[0]++;

            if (projectile.ai[0] < 200)
            {
                if (projectile.ai[0] > 100)
                {
                    projectile.velocity.X = projectile.velocity.X * 40 / 41;
                    projectile.velocity.Y = projectile.velocity.Y * 45 / 46 - 0.005f;
                }
            }
            else
            {
                projectile.velocity.X = projectile.velocity.X * 10 / 11;
                projectile.velocity.Y = projectile.velocity.Y * 10 / 11;
                if (roundsGone <= 4)
                    projectile.ai[1]++;

                int rand = Main.rand.Next(-50, 51);
                int rand2 = Main.rand.Next(-50, 51);
                Vector2 targetDir = projectile.Center + new Vector2(rand, rand2);
                if (projectile.ai[1] > 40)
                    Projectile.NewProjectile(projectile.Center, projectile.SafeDirectionTo(targetDir) * 12f, ModContent.ProjectileType<Crescent>(), projectile.damage / 2, 0.4f, projectile.owner, projectile.whoAmI);

                if (projectile.ai[1] > 46)
                {
                    projectile.ai[1] = 0;
                    roundsGone++;
                }
                if (roundsGone > 4 && Main.player[projectile.owner].ownedProjectileCounts[ModContent.ProjectileType<Crescent>()] == 0)
                    projectile.Kill();
            }

            projectile.frameCounter++;
            if (projectile.frameCounter >= 4)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
                if (projectile.frame >= 6)
                {
                    projectile.frame = 0;
                }
            }

            if (Main.rand.NextFloat() < 1f)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = projectile.Center;
                dust = Main.dust[Terraria.Dust.NewDust(position, 0, 0, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.7236842f)];
            }

            dust_nut++;

            if (dust_nut > 22)
            {
                int num20 = 36;
                for (int i = 0; i < num20; i++)
                {
                    Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f * 0.5f;
                    spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + projectile.Center;
                    Vector2 vector = spinningpoint - projectile.Center;
                    int num21 = Dust.NewDust(spinningpoint + vector, 0, 0, 226, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 0.7236842f);
                    Main.dust[num21].noGravity = true;
                    Main.dust[num21].noLight = true;
                    Main.dust[num21].velocity = Vector2.Normalize(vector) * 3f;
                }
                dust_nut = 0;
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 25; i++)
            {
                Dust dust;
                // You need to set position depending on what you are doing. You may need to subtract width/2 and height/2 as well to center the spawn rectangle.
                Vector2 position = projectile.Center - new Vector2(34, 34);
                dust = Main.dust[Terraria.Dust.NewDust(position, 68, 68, 226, 0f, 0f, 0, new Color(255, 255, 255), 0.7236842f)];
            }

            int num20 = 36;
            for (int i = 0; i < num20; i++)
            {
                Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f * 0.5f;
                spinningpoint = spinningpoint.RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default(Vector2)) + projectile.Center;
                Vector2 vector = spinningpoint - projectile.Center;
                int num21 = Dust.NewDust(spinningpoint + vector, 0, 0, 226, vector.X * 2f, vector.Y * 2f, 0, new Color(255, 255, 255), 0.7236842f);
                Main.dust[num21].noGravity = true;
                Main.dust[num21].noLight = true;
                Main.dust[num21].velocity = Vector2.Normalize(vector) * 3f;
            }
        }
    }
}