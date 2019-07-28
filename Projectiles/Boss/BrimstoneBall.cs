using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneBall : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimstone Fireball");
		}

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.hostile = true;
            projectile.penetrate = 6;
            projectile.aiStyle = 14;
            aiType = 277;
            projectile.timeLeft = 300;
        }

		public override void AI()
		{
			Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f);
			for (int num468 = 0; num468 < 5; num468++)
			{
				int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 100, default(Color), 1f);
				Main.dust[num469].noGravity = true;
				Main.dust[num469].velocity *= 0f;
			}
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.velocity *= 0.98f;
            }
            return false;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 180);
        }
    }
}
