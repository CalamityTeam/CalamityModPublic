using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class ForbiddenAxeBlade : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blade");
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
            projectile.alpha -= 3;
            projectile.rotation += 0.75f;

            projectile.ai[1] += 1f;
            if (projectile.ai[1] <= 20f)
            {
                projectile.velocity *= 0.85f;
            }
            else if (projectile.ai[1] > 20f && projectile.ai[1] <= 39f)
            {
                projectile.velocity *= 1.25f;
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, 300f, 10f, 20f);
            }
            else if (projectile.ai[1] == 40f)
            {
                projectile.ai[1] = 0f;
            }

            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, projectile.velocity.X * 0.1f, projectile.velocity.Y * 0.1f);
            }
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 159, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }
    }
}
