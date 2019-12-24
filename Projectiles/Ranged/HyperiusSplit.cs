using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class HyperiusSplit : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hyperius Bad Time");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 360;
            projectile.extraUpdates = 1;
            aiType = ProjectileID.Bullet;
        }

        public override void AI()
        {
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.25f / 255f, (255 - projectile.alpha) * 0.01f / 255f, (255 - projectile.alpha) * 0.01f / 255f);
            if (Main.rand.NextBool(2))
            {
                int dustType = Main.rand.Next(3);
                if (dustType == 0)
                {
                    dustType = 235;
                }
                else if (dustType == 1)
                {
                    dustType = 61;
                }
                else
                {
                    dustType = 88;
                }
                int num137 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), 1, 1, dustType, 0f, 0f, 0, default, 0.5f);
                Main.dust[num137].alpha = projectile.alpha;
                Main.dust[num137].velocity *= 0f;
                Main.dust[num137].noGravity = true;
            }

            // HyperiusBulletProj.Visuals(projectile);
        }

        // This projectile is always fullbright.
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(1f, 1f, 1f, 0f);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityGlobalProjectile.DrawCenteredAndAfterimage(projectile, lightColor, ProjectileID.Sets.TrailingMode[projectile.type], 1);
            return false;
        }
    }
}
