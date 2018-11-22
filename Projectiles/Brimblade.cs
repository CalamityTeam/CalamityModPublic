using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Brimblade : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimblade");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 26;
            projectile.height = 26;
            projectile.friendly = true;
            projectile.thrown = true;
            projectile.penetrate = -1;
            projectile.aiStyle = 3;
            projectile.timeLeft = 300;
            aiType = 52;
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(mod.BuffType("BrimstoneFlames"), 100);
			int numProj = 2;
            float rotation = MathHelper.ToRadians(20);
            if (projectile.owner == Main.myPlayer)
            {
	            for (int i = 0; i < numProj + 1; i++)
	            {
	                Vector2 perturbedSpeed = new Vector2(projectile.velocity.X, projectile.velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numProj - 1)));
	                Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, perturbedSpeed.X * 0.25f, perturbedSpeed.Y * 0.25f, mod.ProjectileType("Brimblade2"), (int)((double)projectile.damage * 0.85), projectile.knockBack * 0.5f, projectile.owner, 0f, 0f);
	            }
            }
            for (int k = 0; k < 5; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }
    }
}