using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class SoulScythe : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Scythe");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 50;
            projectile.height = 50;
            projectile.aiStyle = 18;
            projectile.alpha = 55;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 420;
            projectile.ignoreWater = true;
            projectile.tileCollide = false;
            aiType = 274;
        }
        
        public override void AI()
        {
        	if (projectile.localAI[0] == 0f)
			{
				projectile.scale -= 0.02f;
				projectile.alpha += 30;
				if (projectile.alpha >= 250)
				{
					projectile.alpha = 255;
					projectile.localAI[0] = 1f;
				}
			}
			else if (projectile.localAI[0] == 1f)
			{
				projectile.scale += 0.02f;
				projectile.alpha -= 30;
				if (projectile.alpha <= 0)
				{
					projectile.alpha = 0;
					projectile.localAI[0] = 0f;
				}
			}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        	if (Main.rand.Next(2) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 75, projectile.velocity.X * 1.5f, projectile.velocity.Y * 1.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.immune[projectile.owner] = 6;
            target.AddBuff(BuffID.OnFire, 200);
            target.AddBuff(mod.BuffType("Plague"), 200);
			if (target.life <= (target.lifeMax * 0.15f))
			{
				Main.PlaySound(2, (int)target.position.X, (int)target.position.Y, 14);
				if (projectile.owner == Main.myPlayer)
				{
					Projectile.NewProjectile(target.Center.X, target.Center.Y, projectile.velocity.X * 0f, projectile.velocity.Y * 0f, mod.ProjectileType("HiveBombExplosion"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
				}
				projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
				projectile.width = 60;
				projectile.height = 60;
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				for (int num621 = 0; num621 < 30; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 50; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 89, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
        }
    }
}