using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class QuagmireProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Quagmire");
		}
    	
        public override void SetDefaults()
        {
        	projectile.CloneDefaults(ProjectileID.HelFire);
            projectile.width = 16;
            projectile.scale = 1.25f;
            projectile.height = 16;
            projectile.penetrate = 8;
            projectile.melee = true;
            aiType = 553;
        }
        
        public override void AI()
        {
        	if (Main.rand.Next(5) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 44, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        	if (projectile.owner == Main.myPlayer)
        	{
	        	if (Main.rand.Next(10) == 0)
	        	{
	            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.35f, projectile.velocity.Y * 0.35f, 569, (int)((double)projectile.damage * 0.65f), projectile.knockBack, projectile.owner, 0f, 0f);
	        	}
	        	if (Main.rand.Next(30) == 0)
	        	{
	            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.25f, projectile.velocity.Y * 0.25f, 570, (int)((double)projectile.damage * 0.75f), projectile.knockBack, projectile.owner, 0f, 0f);
	        	}
	        	if (Main.rand.Next(50) == 0)
	        	{
	            	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, projectile.velocity.X * 0.15f, projectile.velocity.Y * 0.15f, 571, (int)((double)projectile.damage * 0.85f), projectile.knockBack, projectile.owner, 0f, 0f);
	        	}
        	}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			target.AddBuff(BuffID.Venom, 200);
        }
    }
}