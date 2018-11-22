using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Voidragon : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blast");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.alpha = 150;
            projectile.friendly = true;
            projectile.ranged = true;
            projectile.extraUpdates = 1;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
            aiType = 503;
        }
        
        public override void AI()
		{
			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 150 + Main.rand.Next(40);
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 92);
			}
            projectile.localAI[0] += 1f;
            if (projectile.localAI[0] == 12f)
            {
                projectile.localAI[0] = 0f;
                for (int l = 0; l < 12; l++)
                {
                    Vector2 vector3 = Vector2.UnitX * (float)(-(float)projectile.width) / 2f;
                    vector3 += -Vector2.UnitY.RotatedBy((double)((float)l * 3.14159274f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector3 = vector3.RotatedBy((double)(projectile.rotation - 1.57079637f), default(Vector2));
                    int num9 = Dust.NewDust(projectile.Center, 0, 0, 173, 0f, 0f, 160, default(Color), 1f);
                    Main.dust[num9].scale = 1.1f;
                    Main.dust[num9].noGravity = true;
                    Main.dust[num9].position = projectile.Center + vector3;
                    Main.dust[num9].velocity = projectile.velocity * 0.1f;
                    Main.dust[num9].velocity = Vector2.Normalize(projectile.Center - projectile.velocity * 3f - Main.dust[num9].position) * 1.25f;
                }
            }
            projectile.alpha -= 15;
			int num58 = 150;
			if (projectile.Center.Y >= projectile.ai[1])
			{
				num58 = 0;
			}
			if (projectile.alpha < num58)
			{
				projectile.alpha = num58;
			}
			projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;
			if (Main.rand.Next(16) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default(Vector2));
				int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				Main.dust[num59].velocity = value3 * 0.66f;
				Main.dust[num59].position = projectile.Center + value3 * 12f;
			}
			if (Main.rand.Next(48) == 0)
			{
				int num60 = Gore.NewGore(projectile.Center, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), 16, 1f);
				Main.gore[num60].velocity *= 0.66f;
				Main.gore[num60].velocity += projectile.velocity * 0.3f;
			}
			if (projectile.ai[1] == 1f)
			{
				projectile.light = 0.9f;
				if (Main.rand.Next(10) == 0)
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				}
				if (Main.rand.Next(20) == 0)
				{
					Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
				}
			}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.1f) / 255f, ((255 - projectile.alpha) * 0.7f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f);
        }
        
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("PlasmaExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
        	}
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 173, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }
    }
}