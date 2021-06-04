using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SandBlade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[projectile.type] = 1;
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.melee = true;
        }

        public override void AI()
        {
            projectile.rotation += 0.5f;
            projectile.ai[1] += 1f;
            if (projectile.ai[1] <= 30f)
            {
                projectile.velocity.X *= 0.925f;
                projectile.velocity.Y *= 0.925f;
            }
            else if (projectile.ai[1] > 30f && projectile.ai[1] <= 59f)
            {
                projectile.velocity.X *= 1.15f;
                projectile.velocity.Y *= 1.15f;
            }
            else if (projectile.ai[1] == 60f)
            {
                projectile.ai[1] = 0f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 2);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, 0f, 0f);
            }
        }
    }
}
