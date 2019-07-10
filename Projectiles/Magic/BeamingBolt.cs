using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
    public class BeamingBolt : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beaming Bolt");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.penetrate = -1;
            projectile.magic = true;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.95f;
        	projectile.velocity.Y *= 0.985f;
        	for (int dust = 0; dust < 3; dust++)
        	{
        		int randomDust = Main.rand.Next(3);
	        	if (randomDust == 0)
	        	{
	        		randomDust = 164;
	        	}
	        	else if (randomDust == 1)
	        	{
	        		randomDust = 58;
	        	}
	        	else
	        	{
	        		randomDust = 204;
	        	}
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        	}
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 6; k++)
            {
            	int randomDust = Main.rand.Next(3);
	        	if (randomDust == 0)
	        	{
	        		randomDust = 164;
	        	}
	        	else if (randomDust == 1)
	        	{
	        		randomDust = 58;
	        	}
	        	else
	        	{
	        		randomDust = 204;
	        	}
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, randomDust, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
            float spread = 90f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y)- spread/2;
			double deltaAngle = spread/8f;
			double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer)
			{
				for (i = 0; i < 4; i++ )
				{
					offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("BeamingBolt2"), (int)((double)projectile.damage * 0.75f), projectile.knockBack, projectile.owner, 0f, 0f);
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("BeamingBolt2"), (int)((double)projectile.damage * 0.75f), projectile.knockBack, projectile.owner, 0f, 0f);
				}
			}
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 105);
        }
        
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.immune[projectile.owner] = 7;
		}
    }
}
