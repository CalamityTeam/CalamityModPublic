using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BrimstoneFireball : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fireball");
		}

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.aiStyle = 1;
            projectile.penetrate = 4;
            aiType = 277;
            projectile.timeLeft = 300;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f, ((255 - projectile.alpha) * 0.05f) / 255f);
        	if (projectile.wet && !projectile.lavaWet)
        	{
        		projectile.Kill();
        		if (projectile.owner == Main.myPlayer)
        		{
	        		int num251 = Main.rand.Next(2, 3);
					for (int num252 = 0; num252 < num251; num252++)
					{
						Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						while (value15.X == 0f && value15.Y == 0f)
						{
							value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
						}
						value15.Normalize();
						value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
						Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, mod.ProjectileType("BrimstoneHomer"), (int)((double)projectile.damage * 0.85), 0f, projectile.owner, 0f, 0f);
					}
        		}
        	}
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num468 = 0; num468 < 5; num468++)
				{
					int num469 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 235, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num469].noGravity = true;
					Main.dust[num469].velocity *= 0f;
				}
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
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
                projectile.velocity *= 0.8f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
        }
    }
}
