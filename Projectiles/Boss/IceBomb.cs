using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class IceBomb : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Bomb");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 30;
            projectile.height = 30;
            projectile.hostile = true;
            projectile.alpha = 50;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
        	projectile.velocity.X *= 0.985f;
        	projectile.velocity.Y *= 0.985f;
            if (Main.rand.Next(6) == 0)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
            }
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
        	float spread = 60f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
	    	double deltaAngle = spread / 8f;
	    	double offsetAngle;
			int i;
			if (projectile.owner == Main.myPlayer)
			{
		    	for (i = 0; i < 3; i++)
		    	{
		   			offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
		   			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("IceRain"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
		        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("IceRain"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
		    	}
			}
        	for (int k = 0; k < 3; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(mod.BuffType("GlacialState"), 60);
        }
    }
}