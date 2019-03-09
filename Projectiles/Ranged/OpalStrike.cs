using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class OpalStrike : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Strike");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.scale = 0.5f;
            projectile.alpha = 200;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	if (projectile.scale <= 1.5f)
        	{
        		projectile.scale *= 1.01f;
        	}
        	projectile.alpha -= 2;
        	if (projectile.alpha == 0)
        	{
        		projectile.alpha = 200;
        	}
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}