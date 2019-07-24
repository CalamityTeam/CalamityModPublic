using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
	public class HyperiusBullet : ModProjectile
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
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
        	projectile.localAI[0] += 1f;
        	if (projectile.localAI[0] >= 6f)
        	{
				for (int num136 = 0; num136 < 3; num136++)
				{
					float x2 = projectile.position.X - projectile.velocity.X / 10f * (float)num136;
					float y2 = projectile.position.Y - projectile.velocity.Y / 10f * (float)num136;
					int num137 = Dust.NewDust(new Vector2(x2, y2), 1, 1, 235, 0f, 0f, 0, default(Color), 0.75f);
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
    		float xPos = (Main.rand.Next(2) == 0 ? projectile.position.X + 800 : projectile.position.X - 800);
    		Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-800, 801));
    		float num80 = xPos;
    		float speedX = (float)target.position.X - vector2.X;
    		float speedY = (float)target.position.Y - vector2.Y;
    		float dir= (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
    		dir = 10 / num80;
    		speedX *= dir * 150;
    		speedY *= dir * 150;
            if (speedX > 15f)
            {
                speedX = 15f;
            }
            if (speedX < -15f)
            {
                speedX = -15f;
            }
            if (speedY > 15f)
            {
                speedY = 15f;
            }
            if (speedY < -15f)
            {
                speedY = -15f;
            }
            if (projectile.owner == Main.myPlayer)
    		{
    			Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("OMGWTH"), (int)((double)projectile.damage * 0.8), 1f, projectile.owner);
    		}
		}
	}
}