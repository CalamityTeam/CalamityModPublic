using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class BeamingBolt2 : ModProjectile
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
            projectile.penetrate = 1;
            projectile.magic = true;
        }

        public override void AI()
        {
        	for (int dust = 0; dust < 2; dust++)
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
        	float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = 400f;
			bool flag17 = false;
			for (int num475 = 0; num475 < 200; num475++)
			{
				if (Main.npc[num475].CanBeChasedBy(projectile, false) && Collision.CanHit(projectile.Center, 1, 1, Main.npc[num475].Center, 1, 1))
				{
					float num476 = Main.npc[num475].position.X + (float)(Main.npc[num475].width / 2);
					float num477 = Main.npc[num475].position.Y + (float)(Main.npc[num475].height / 2);
					float num478 = Math.Abs(projectile.position.X + (float)(projectile.width / 2) - num476) + Math.Abs(projectile.position.Y + (float)(projectile.height / 2) - num477);
					if (num478 < num474)
					{
						num474 = num478;
						num472 = num476;
						num473 = num477;
						flag17 = true;
					}
				}
			}
			if (flag17)
			{
				float num483 = 20f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
				return;
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
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
        }
    }
}