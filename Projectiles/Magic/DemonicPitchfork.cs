using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class DemonicPitchfork : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pitchfork");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 27;
            projectile.friendly = true;
            projectile.magic = true;
            projectile.penetrate = 2;
            projectile.timeLeft = 600;
            aiType = 114;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.2f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f, ((255 - projectile.alpha) * 0.2f) / 255f);
            if (projectile.localAI[1] > 7f)
			{
				int num307 = Main.rand.Next(3);
				if (num307 == 0)
				{
					num307 = 14;
				}
				else if (num307 == 1)
				{
					num307 = 27;
				}
				else
				{
					num307 = 173;
				}
				int num308 = Dust.NewDust(new Vector2(projectile.position.X - projectile.velocity.X * 4f + 2f, projectile.position.Y + 2f - projectile.velocity.Y * 4f), 8, 8, num307, 0f, 0f, 100, default(Color), 1.25f);
				Main.dust[num308].velocity *= 0.1f;
			}
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        
        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 2; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 14, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 27, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}
