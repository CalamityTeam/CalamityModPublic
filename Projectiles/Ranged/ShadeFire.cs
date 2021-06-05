using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Ranged
{
    public class ShadeFire : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

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
            projectile.penetrate = -1;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 80;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0f / 255f, (255 - projectile.alpha) * 0.35f / 255f);
            if (projectile.timeLeft > 80)
            {
                projectile.timeLeft = 80;
            }
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
                int num297 = 14;
                if (Main.rand.NextBool(2))
                {
                    for (int num298 = 0; num298 < 3; num298++)
                    {
                        int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default, 1f);
                        Dust dust = Main.dust[num299];
                        if (Main.rand.NextBool(3))
                        {
                            dust.noGravity = true;
                            dust.scale *= 1.75f;
                            dust.velocity.X *= 2f;
                            dust.velocity.Y *= 2f;
                        }
                        else
                        {
                            dust.scale *= 0.5f;
                        }
                        dust.velocity.X *= 1.2f;
                        dust.velocity.Y *= 1.2f;
                        dust.scale *= num296;
                        dust.velocity += projectile.velocity;
                        if (!dust.noGravity)
                        {
                            dust.velocity *= 0.5f;
                        }
                    }
                }
            }
            else
            {
                projectile.ai[0] += 1f;
            }
            projectile.rotation += 0.3f * (float)projectile.direction;
        }
    }
}
