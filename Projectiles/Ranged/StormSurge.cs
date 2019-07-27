using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class StormSurge : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Surge");
			Main.projFrames[projectile.type] = 6;
		}

        public override void SetDefaults()
        {
            projectile.width = 66;
            projectile.height = 66;
            projectile.scale = 0.5f;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.penetrate = 2;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, 0f, 1.25f, 1.25f);
        	if (projectile.scale <= 2f)
        	{
        		projectile.scale *= 1.03f;
        	}
        	if (projectile.scale >= 2f)
        	{
        		projectile.Kill();
        	}
        	projectile.frameCounter++;
			if (projectile.frameCounter > 2)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame >= 6)
			{
				projectile.frame = 0;
			}
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
        	Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
        }
    }
}
