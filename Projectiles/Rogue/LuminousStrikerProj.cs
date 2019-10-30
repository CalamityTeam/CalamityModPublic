using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class LuminousStrikerProj : ModProjectile
    {
    	public int shardRainTimer = 3;
		bool stealthStrike = false;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Luminous Striker");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
		}
        
        public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
			if(projectile.ai[0] == 0f && modPlayer.StealthStrikeAvailable())
			{
                projectile.Calamity().stealthStrike = true;
				stealthStrike = true;
				projectile.timeLeft = 600;
				projectile.ai[0] = 1f;
			}
        	
			shardRainTimer--;
        	if (Main.rand.Next(4) == 0)
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 176, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 0.785f;
			projectile.spriteDirection = ((projectile.velocity.X > 0f) ? -1 : 1);
			
        	if (shardRainTimer == 0)
			{
        		if (projectile.owner == Main.myPlayer)
        		{
					if(stealthStrike)
					{
						Projectile.NewProjectile(projectile.Center.X + Main.rand.NextFloat(-15f, 15f), projectile.Center.Y + Main.rand.NextFloat(-15f, 15f), projectile.velocity.X, projectile.velocity.Y, ModContent.ProjectileType<LuminousShard>(), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
					}
					else
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0f, -2f, ModContent.ProjectileType<LuminousShard>(), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
                }
				shardRainTimer = 4;
			}
        }
		
		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
		
		public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.owner == Main.myPlayer)
			{
				for (int i = 0; i < 7; i++)
				{
					Vector2 speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
					while (speed.X == 0f && speed.Y == 0f)
					{
						speed = new Vector2((float)Main.rand.Next(-50, 51), (float)Main.rand.Next(-50, 51));
					}
					speed.Normalize();
					speed *= ((float)Main.rand.Next(30, 61) * 0.1f) * 2f;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, speed.X, speed.Y, mod.ProjectileType("LuminousShard"), (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
		}
        
        public override void Kill(int timeLeft)
        {
        	for (int i = 0; i <= 10; i++)
        	{
        		Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 176, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
        	}
        }
    }
}