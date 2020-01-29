using CalamityMod.Projectiles.Typeless;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneTargetRay : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Target Ray");
        }

        public override void SetDefaults()
        {
            projectile.width = 4;
            projectile.height = 4;
            projectile.hostile = true;
            projectile.penetrate = -1;
            projectile.extraUpdates = 100;
            projectile.timeLeft = 300;
        }

		public override void AI()
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 vector33 = projectile.position;
				vector33 -= projectile.velocity * 0.25f;
				projectile.alpha = 255;
				int num448 = Dust.NewDust(vector33, 1, 1, 235, 0f, 0f, 0, default, 1f);
				Main.dust[num448].position = vector33;
				Main.dust[num448].scale = (float)Main.rand.Next(70, 110) * 0.008f;
				Main.dust[num448].velocity *= 0.2f;
			}
		}
    }
}
