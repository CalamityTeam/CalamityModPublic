using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class TurbulanceWindSlash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wind Slash");
            Main.projFrames[projectile.type] = 4;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 3;
            ProjectileID.Sets.TrailingMode[projectile.type] = 0;
        }

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 0;
            projectile.alpha = 255;
            projectile.ignoreWater = true;
            projectile.Calamity().rogue = true;
            projectile.timeLeft = 180;
        }

        public override bool? CanHitNPC(NPC target) => projectile.timeLeft < 150 && target.CanBeChasedBy(projectile);

        public override void AI()
        {
            projectile.velocity *= 0.99f;
            if (projectile.localAI[0] == 0f)
            {
                projectile.scale += 0.005f;
            }
            if (projectile.alpha > 0)
            {
                projectile.alpha -= 30;
            }
            if (projectile.alpha < 0)
            {
                projectile.alpha = 0;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 1)
            {
                projectile.frameCounter = 0;
                projectile.frame++;
                if (projectile.frame >= 4)
                {
                    projectile.frame = 0;
                }
            }
            if (projectile.ai[0] == 1f) //stealth strike
            {
                projectile.rotation = projectile.velocity.ToRotation() - MathHelper.PiOver2;
            }
            if (projectile.ai[1] == 1f)
            {
                CalamityGlobalProjectile.HomeInOnNPC(projectile, !projectile.tileCollide, projectile.ai[0] == 1f ? 900f : 450f, 8f, 20f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
            return false;
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 10; k++)
            {
                Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 187, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 100, new Color(53, Main.DiscoG, 255));
            }
        }
    }
}
