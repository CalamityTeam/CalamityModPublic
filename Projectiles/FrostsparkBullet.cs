using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
	public class FrostsparkBullet : ModProjectile
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bullet");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 2;
			ProjectileID.Sets.TrailingMode[projectile.type] = 0;
		}
    	
		public override void SetDefaults()
		{
			projectile.width = 8;
			projectile.height = 8;
			projectile.aiStyle = 1;
			projectile.friendly = true;
			projectile.ranged = true;
			projectile.penetrate = 1;
			projectile.timeLeft = 600;
			projectile.extraUpdates = 1;
			aiType = ProjectileID.Bullet;
		}
		
		public override void AI()
		{
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f);
        	projectile.localAI[0] += 1f;
        	if (projectile.localAI[0] >= 4f)
        	{
				for (int num136 = 0; num136 < 10; num136++)
				{
					float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
					float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
					int dustType = Main.rand.Next(2);
					if (dustType == 0)
					{
						dustType = 67;
					}
					else
					{
						dustType = 6;
					}
					int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, dustType, 0f, 0f, 0, default(Color), 0.5f);
					Main.dust[num137].alpha = projectile.alpha;
					Main.dust[num137].position.X = x2;
					Main.dust[num137].position.Y = y2;
					Main.dust[num137].velocity *= 0f;
					Main.dust[num137].noGravity = true;
				}
        	}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Vector2 drawOrigin = new Vector2(Main.projectileTexture[projectile.type].Width * 0.5f, projectile.height * 0.5f);
			for (int k = 0; k < projectile.oldPos.Length; k++)
			{
				Vector2 drawPos = projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, projectile.gfxOffY);
				Color color = projectile.GetAlpha(lightColor) * ((float)(projectile.oldPos.Length - k) / (float)projectile.oldPos.Length);
				spriteBatch.Draw(Main.projectileTexture[projectile.type], drawPos, null, color, projectile.rotation, drawOrigin, projectile.scale, SpriteEffects.None, 0f);
			}
			return true;
		}
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.OnFire, 240);
        	target.AddBuff(BuffID.Frostburn, 240);
        	target.AddBuff(mod.BuffType("GlacialState"), 120);
        }
		
		public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("BoltExplosion"), (int)((double)projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
        	int num212 = Main.rand.Next(10, 20);
			for (int num213 = 0; num213 < num212; num213++)
			{
				int dustType = Main.rand.Next(2);
				if (dustType == 0)
				{
					dustType = 67;
				}
				else
				{
					dustType = 6;
				}
				int num214 = Dust.NewDust(projectile.Center - projectile.velocity / 2f, 0, 0, dustType, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num214].velocity *= 2f;
				Main.dust[num214].noGravity = true;
			}
        }
	}
}