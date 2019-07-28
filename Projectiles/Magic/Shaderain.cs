using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class Shaderain : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rain");
		}

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 40;
            projectile.friendly = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 3;
            projectile.ignoreWater = true;
            projectile.timeLeft = 300;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.alpha = 50;
        }

        public override void Kill(int timeLeft)
        {
            int num310 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y + (float)projectile.height - 2f), 2, 2, 14, 0f, 0f, 0, default(Color), 1f);
			Dust expr_A0A0_cp_0 = Main.dust[num310];
			expr_A0A0_cp_0.position.X = expr_A0A0_cp_0.position.X - 2f;
			Main.dust[num310].alpha = 38;
			Main.dust[num310].velocity *= 0.1f;
			Main.dust[num310].velocity += -projectile.oldVelocity * 0.25f;
			Main.dust[num310].scale = 0.95f;
        }
    }
}
