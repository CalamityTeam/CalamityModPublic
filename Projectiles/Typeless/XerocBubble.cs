using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Typeless
{
	public class XerocBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] > 4f)
            {
                for (int num468 = 0; num468 < 1; num468++)
                {
                    int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 62, 0f, 0f, 100, default, 1f);
                    Main.dust[num469].noGravity = true;
                    Main.dust[num469].velocity *= 0f;
                }
            }
			CalamityGlobalProjectile.HomeInOnNPC(projectile, false, 400f, 8f, 20f);
        }
    }
}
