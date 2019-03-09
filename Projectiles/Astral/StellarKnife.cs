using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Astral
{
    public class StellarKnife : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stellar Knife");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 32;
            projectile.height = 34;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
		}
        
        public override void AI()
        {
			projectile.ai[1] += 0.75f;
        	if (projectile.ai[1] <= 60f)
        	{
				projectile.rotation += 1f;
				projectile.velocity.X *= 0.985f;
				projectile.velocity.Y *= 0.985f;
			}
			else
			{
				projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
				if(projectile.spriteDirection == -1)
				{
					projectile.rotation -= 1.57f;
				}
				float centerX = projectile.Center.X;
				float centerY = projectile.Center.Y;
				float num474 = 600f;
				bool homeIn = false;
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1))
					{
						float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
						float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
						float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
						if (num478 < num474)
						{
							num474 = num478;
							centerX = num476;
							centerY = num477;
							homeIn = true;
						}
					}
				}
				if (homeIn)
				{
					float num483 = 30f;
					Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num484 = centerX - vector35.X;
					float num485 = centerY - vector35.Y;
					float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
					num486 = num483 / num486;
					num484 *= num486;
					num485 *= num486;
					projectile.velocity.X = (projectile.velocity.X * 10f + num484) / 11f;
					projectile.velocity.Y = (projectile.velocity.Y * 10f + num485) / 11f;
					return;
				}
				else
				{
					projectile.velocity.X *= 0.92f;
					projectile.velocity.Y *= 0.92f;
				}
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
        	for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, mod.DustType("AstralBlue"), projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}