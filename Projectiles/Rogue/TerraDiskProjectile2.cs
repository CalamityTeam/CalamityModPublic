using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class TerraDiskProjectile2 : ModProjectile
    {
        private double rotation = 0;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Terra Disk");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 40;
            projectile.height = 40;
            projectile.alpha = 75;
            projectile.ignoreWater = true;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 60;
            aiType = ProjectileID.WoodenBoomerang;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            StealthStrikeAI();
            LightingandDust();
        }

        private void StealthStrikeAI()
        {
            if (projectile.aiStyle == 3)
                return;

            projectile.rotation += 0.4f * projectile.direction;

            Projectile parent = Main.projectile[0];
            bool active = false;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (p.identity == projectile.ai[0] && p.active && p.type == ModContent.ProjectileType<TerraDiskProjectile>())
                {
                    parent = p;
                    active = true;
                    break;
                }
            }

            if (active)
            {
                Vector2 vector = parent.Center - projectile.Center;
                projectile.Center = parent.Center + new Vector2(80, 0).RotatedBy(rotation);
                double rotateAmt = (double)projectile.ai[1];
                rotation += rotateAmt;
                if (rotation >= 360)
                {
                    rotation = 0;
                }
                projectile.velocity.X = (vector.X > 0f) ? -0.000001f : 0f;
            }
            else
            {
                projectile.Kill();
            }

            if (!parent.active)
            {
                projectile.Kill();
            }
        }

        private void LightingandDust()
        {
            Lighting.AddLight(projectile.Center, 0f, 0.75f, 0f);
            if (!Main.rand.NextBool(5))
                return;
            Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 107, projectile.velocity.X, projectile.velocity.Y);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }
    }
}
