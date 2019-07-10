using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class ChargedBlast : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blast");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 5;
            projectile.height = 5;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.penetrate = 1;
            projectile.extraUpdates = 3;
            projectile.alpha = 255;
            projectile.timeLeft = 600;
        }

        public override void AI()
        {
			if (projectile.alpha > 0)
			{
				projectile.alpha -= 25;
			}
			if (projectile.alpha < 0)
			{
				projectile.alpha = 0;
			}
			Lighting.AddLight((int)projectile.Center.X / 16, (int)projectile.Center.Y / 16, 0f, 0.3f, 0.7f);
			float num55 = 100f;
			float num56 = 3f;
			if (projectile.ai[1] == 0f)
			{
				projectile.localAI[0] += num56;
				if (projectile.localAI[0] > num55)
				{
					projectile.localAI[0] = num55;
				}
			}
			else
			{
				projectile.localAI[0] -= num56;
				if (projectile.localAI[0] <= 0f)
				{
					projectile.Kill();
					return;
				}
			}
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
        	return new Color(100, 100, 255, 0);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        	Microsoft.Xna.Framework.Color color25 = Lighting.GetColor((int)((double)projectile.position.X + (double)projectile.width * 0.5) / 16, (int)(((double)projectile.position.Y + (double)projectile.height * 0.5) / 16.0));
        	int num147 = 0;
			int num148 = 0;
        	float num149 = (float)(Main.projectileTexture[projectile.type].Width - projectile.width) * 0.5f + (float)projectile.width * 0.5f;
        	SpriteEffects spriteEffects = SpriteEffects.None;
			if (projectile.spriteDirection == -1)
			{
				spriteEffects = SpriteEffects.FlipHorizontally;
			}
        	Microsoft.Xna.Framework.Rectangle value6 = new Microsoft.Xna.Framework.Rectangle((int)Main.screenPosition.X - 500, (int)Main.screenPosition.Y - 500, Main.screenWidth + 1000, Main.screenHeight + 1000);
			if (projectile.getRect().Intersects(value6))
			{
				Vector2 value7 = new Vector2(projectile.position.X - Main.screenPosition.X + num149 + (float)num148, projectile.position.Y - Main.screenPosition.Y + (float)(projectile.height / 2) + projectile.gfxOffY);
				float num162 = 100f;
				float scaleFactor = 3f;
				if (projectile.ai[1] == 1f)
				{
					num162 = (float)((int)projectile.localAI[0]);
				}
				for (int num163 = 1; num163 <= (int)projectile.localAI[0]; num163++)
				{
					Vector2 value8 = Vector2.Normalize(projectile.velocity) * (float)num163 * scaleFactor;
					Microsoft.Xna.Framework.Color color29 = projectile.GetAlpha(color25);
					color29 *= (num162 - (float)num163) / num162;
					color29.A = 0;
					Main.spriteBatch.Draw(Main.projectileTexture[projectile.type], value7 - value8, null, color29, projectile.rotation, new Vector2(num149, (float)(projectile.height / 2 + num147)), projectile.scale, spriteEffects, 0f);
				}
			}
			return false;
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 62);
            int projectiles = Main.rand.Next(2, 5);
            if (projectile.owner == Main.myPlayer)
            {
	        	for (int k = 0; k < projectiles; k++)
	            {
	            	Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-10, 11) * 2f, (float)Main.rand.Next(-10, 11) * 2f, mod.ProjectileType("ChargedBlast2"), 
	            	(int)((double)projectile.damage * 0.85), (float)((int)((double)projectile.knockBack * 0.5)), Main.myPlayer, 0f, 0f);
	            }
            }
        }
    }
}
