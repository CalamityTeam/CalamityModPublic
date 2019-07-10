using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee.Spears
{
    public class HeartRapierProjectile : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Rapier");
		}
    	
        public override void SetDefaults()
        {
			projectile.width = 44;  //The width of the .png file in pixels divided by 2.
			projectile.aiStyle = 19;
			projectile.melee = true;  //Dictates whether this is a melee-class weapon.
			projectile.timeLeft = 90;
			projectile.height = 44;  //The height of the .png file in pixels divided by 2.
			projectile.friendly = true;
			projectile.hostile = false;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.penetrate = -1;
			projectile.ownerHitCheck = true;
			projectile.hide = true;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).trueMelee = true;
		}

        public override void AI()
        {
        	Main.player[projectile.owner].direction = projectile.direction;
        	Main.player[projectile.owner].heldProj = projectile.whoAmI;
        	Main.player[projectile.owner].itemTime = Main.player[projectile.owner].itemAnimation;
        	projectile.position.X = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - (float)(projectile.width / 2);
        	projectile.position.Y = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - (float)(projectile.height / 2);
        	projectile.position += projectile.velocity * projectile.ai[0];
        	if (Main.rand.Next(5) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 12, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        	if (projectile.ai[0] == 0f)
        	{
        		projectile.ai[0] = 3f;
        		projectile.netUpdate = true;
        	}
        	if (Main.player[projectile.owner].itemAnimation < Main.player[projectile.owner].itemAnimationMax / 3)
        	{
        		projectile.ai[0] -= 1.1f;
        	}
        	else
        	{
        		projectile.ai[0] += 0.95f;
        	}
        	if (Main.player[projectile.owner].itemAnimation == 0)
        	{
        		projectile.Kill();
        	}
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 2.355f;
        	if (projectile.spriteDirection == -1)
        	{
        		projectile.rotation -= 1.57f;
        	}
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
        	if (target.type == NPCID.TargetDummy || !target.canGhostHeal)
			{
				return;
			}
			Player player = Main.player[projectile.owner];
			player.statLife += 5;
    		player.HealEffect(5);
		}
    }
}
