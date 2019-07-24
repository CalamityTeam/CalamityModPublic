using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class SupremeDustProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dust");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 200;
            projectile.height = 200;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            projectile.magic = true;
            projectile.penetrate = 4;
            projectile.extraUpdates = 3;
            projectile.timeLeft = 200;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.95f) / 255f, ((255 - projectile.alpha) * 0.85f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
			if (projectile.ai[0] > 7f)
			{
				float num296 = 1f;
				if (projectile.ai[0] == 8f)
				{
					num296 = 0.25f;
				}
				else if (projectile.ai[0] == 9f)
				{
					num296 = 0.5f;
				}
				else if (projectile.ai[0] == 10f)
				{
					num296 = 0.75f;
				}
				projectile.ai[0] += 1f;
				int num297 = 32;
				int num299 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
				if (Main.rand.Next(3) == 0)
				{
					Main.dust[num299].noGravity = true;
					Main.dust[num299].scale *= 4f;
					Dust expr_DBEF_cp_0 = Main.dust[num299];
					expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 2f;
					Dust expr_DC0F_cp_0 = Main.dust[num299];
					expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 2f;
				}
				else
				{
					Main.dust[num299].scale *= 2.5f;
				}
				Dust expr_DC74_cp_0 = Main.dust[num299];
				expr_DC74_cp_0.velocity.X = expr_DC74_cp_0.velocity.X * 1.2f;
				Dust expr_DC94_cp_0 = Main.dust[num299];
				expr_DC94_cp_0.velocity.Y = expr_DC94_cp_0.velocity.Y * 1.2f;
				Main.dust[num299].scale *= num296;
				int num399 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, num297, projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
				if (Main.rand.Next(3) == 0)
				{
					Main.dust[num399].noGravity = true;
					Main.dust[num399].scale *= 6f;
					Dust expr_DBEF_cp_0 = Main.dust[num399];
					expr_DBEF_cp_0.velocity.X = expr_DBEF_cp_0.velocity.X * 2f;
					Dust expr_DC0F_cp_0 = Main.dust[num399];
					expr_DC0F_cp_0.velocity.Y = expr_DC0F_cp_0.velocity.Y * 2f;
				}
				else
				{
					Main.dust[num399].scale *= 2.5f;
				}
				Dust expr_DC74_cp_1 = Main.dust[num399];
				expr_DC74_cp_1.velocity.X = expr_DC74_cp_1.velocity.X * 1.2f;
				Dust expr_DC94_cp_1 = Main.dust[num399];
				expr_DC94_cp_1.velocity.Y = expr_DC94_cp_1.velocity.Y * 1.2f;
				Main.dust[num399].scale *= num296;
			}
			else
			{
				projectile.ai[0] += 1f;
			}
			projectile.rotation += 0.3f * (float)projectile.direction;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.immune[projectile.owner] = 6;
        	if (projectile.owner == Main.myPlayer)
        	{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("SupremeDustFlakProjectile"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        	}
        }
    }
}