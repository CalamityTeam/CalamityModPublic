using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Magic
{
    public class SupremeDustProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dust");
        }

        public override void SetDefaults()
        {
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.95f / 255f, (255 - projectile.alpha) * 0.85f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            if (projectile.ai[0] > 7f)
            {
                float num296 = 1f;
                if (projectile.ai[0] == 8f)
                {
                    num296 = 0.25f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    num296 = 0.5f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    num296 = 0.75f;
                }
                projectile.ai[0] += 1f;
                int num297 = 32;
                int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust = Main.dust[num299];
                if (Main.rand.NextBool(2))
                {
                    dust.noGravity = true;
                    dust.scale *= 4f;
                    dust.velocity.X *= 2f;
                    dust.velocity.Y *= 2f;
                }
                else
                {
                    dust.scale *= 2.5f;
                }
                dust.velocity.X *= 1.2f;
                dust.velocity.Y *= 1.2f;
                dust.scale *= num296;
                int num399 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                Dust dust2 = Main.dust[num399];
                if (Main.rand.NextBool(3))
                {
                    dust2.noGravity = true;
                    dust2.scale *= 6f;
                    dust2.velocity.X *= 2f;
                    dust2.velocity.Y *= 2f;
                }
                else
                {
                    dust2.scale *= 2.5f;
                }
                dust2.velocity.X *= 1.2f;
                dust2.velocity.Y *= 1.2f;
                dust2.scale *= num296;
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.immune[projectile.owner] = 6;
            if (projectile.owner == Main.myPlayer)
            {
                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ModContent.ProjectileType<SupremeDustFlakProjectile>(), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
            }
        }
    }
}
