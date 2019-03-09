using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Earth : ModProjectile
    {
    	public int noTileHitCounter = 120;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Earth");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 34;
            projectile.height = 34;
            projectile.alpha = 100;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.ignoreWater = true;
        }
        
        public override void AI()
		{
        	int randomToSubtract = Main.rand.Next(1, 4);
        	noTileHitCounter -= randomToSubtract;
        	if (noTileHitCounter == 0)
        	{
        		projectile.tileCollide = true;
        	}
			if (projectile.soundDelay == 0)
			{
				projectile.soundDelay = 20 + Main.rand.Next(40);
				if (Main.rand.Next(5) == 0)
				{
					Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 9);
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
			projectile.localAI[0] += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * (float)projectile.direction;
			projectile.rotation = projectile.velocity.ToRotation() - 1.57079637f;
			int dustChoice = Main.rand.Next(3);
			if (dustChoice == 0)
			{
				dustChoice = 74;
			}
			else if (dustChoice == 1)
			{
				dustChoice = 229;
			}
			else
			{
				dustChoice = 244;
			}
			if (Main.rand.Next(16) == 0)
			{
				Vector2 value3 = Vector2.UnitX.RotatedByRandom(1.5707963705062866).RotatedBy((double)projectile.velocity.ToRotation(), default(Vector2));
				int num59 = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustChoice, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
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
					Dust.NewDust(projectile.position, projectile.width, projectile.height, dustChoice, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default(Color), 1.2f);
				}
				if (Main.rand.Next(20) == 0)
				{
					Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
					return;
				}
			}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f);
        }

        public override void Kill(int timeLeft)
        {
        	float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y)- spread/2;
	    	double deltaAngle = spread/8f;
	    	double offsetAngle;
	    	int i;
	    	for (i = 0; i < 4; i++ )
	    	{
	    		int projChoice = Main.rand.Next(3);
            	if (projChoice == 0)
				{
					projChoice = mod.ProjectileType("Earth2");
				}
				else if (projChoice == 1)
				{
					projChoice = mod.ProjectileType("Earth3");
				}
				else
				{
					projChoice = mod.ProjectileType("Earth4");
				}
	   			offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
	        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), projChoice, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
	        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), projChoice, projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
	    	}
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 10);
            for (int k = 0; k < 15; k++)
            {
            	int dustChoice = Main.rand.Next(3);
            	if (dustChoice == 0)
				{
					dustChoice = 74;
				}
				else if (dustChoice == 1)
				{
					dustChoice = 229;
				}
				else
				{
					dustChoice = 244;
				}
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, dustChoice, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 500);
            target.AddBuff(BuffID.Frostburn, 500);
			target.AddBuff(BuffID.CursedInferno, 500);
        }
    }
}