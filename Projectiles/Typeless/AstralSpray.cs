using CalamityMod.World;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Typeless
{
	public class AstralSpray : ModProjectile
    {
        public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = -1;
            projectile.extraUpdates = 2;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
        }

        public override bool CanDamage() => false;

        public override bool PreAI()
        {
            if (Main.myPlayer == projectile.owner)
            {
                int x = (int)(projectile.Center.X / 16f);
                int y = (int)(projectile.Center.Y / 16f);

                AstralBiome.ConvertToAstral(x - 1, x + 1, y - 1, y + 1);
            }
            if (projectile.timeLeft > 133)
            {
                projectile.timeLeft = 133;
            }
            if (projectile.ai[0] > 7f)
            {
                float scalar = 1f;
                if (projectile.ai[0] == 8f)
                {
                    scalar = 0.2f;
                }
                else if (projectile.ai[0] == 9f)
                {
                    scalar = 0.4f;
                }
                else if (projectile.ai[0] == 10f)
                {
                    scalar = 0.6f;
                }
                else if (projectile.ai[0] == 11f)
                {
                    scalar = 0.8f;
                }
                projectile.ai[0]++;
                for (int i = 0; i < 1; i++)
                {
                    int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, 118, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].scale *= 1.75f * scalar;
                    Main.dust[d].velocity.X *= 2f;
                    Main.dust[d].velocity.Y *= 2f;
                }
            }
            else
            {
                projectile.ai[0]++;
            }
            projectile.rotation += 0.3f * projectile.direction;
            return false;
        }
    }
}
