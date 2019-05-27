using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class BalefulHarvesterProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Pumpkin");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.aiStyle = 1;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.alpha = 50;
            projectile.tileCollide = false;
            aiType = 270;
        }

        public override void Kill(int timeLeft)
        {
        	if (projectile.owner == Main.myPlayer)
        	{
		        for (int k = 0; k < 3; k++)
		        {
		        	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 174, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
		          	Projectile.NewProjectile(projectile.position.X, projectile.position.Y, (float)Main.rand.Next(-35, 36) * 0.2f, (float)Main.rand.Next(-35, 36) * 0.2f, mod.ProjectileType("TinyFlare"), 
		           	(int)((double)projectile.damage * 0.7), projectile.knockBack * 0.35f, Main.myPlayer, 0f, 0f);
		        }
        	}
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600);
        }
    }
}