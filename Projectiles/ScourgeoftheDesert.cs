using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class ScourgeoftheDesert : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scourge");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
            projectile.thrown = true;
            projectile.aiStyle = 113;
            aiType = 598;
            projectile.penetrate = 3;
        }
        
        public override void AI()
        {
        	projectile.velocity.X *= 1.025f;
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
        	if (projectile.spriteDirection == -1)
        	{
        		projectile.rotation -= 1.57f;
        	}
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}