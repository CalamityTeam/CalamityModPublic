using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
    public class BrimstoneGigaBlast : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Giga Blast");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 36;
            projectile.height = 36;
            projectile.hostile = true;
            projectile.ignoreWater = true;
            projectile.penetrate = 1;
            projectile.alpha = 50;
            projectile.timeLeft = 150;
            cooldownSlot = 1;
        }

        public override void AI()
        {
        	bool revenge = CalamityWorld.revenge;
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.9f) / 255f, ((255 - projectile.alpha) * 0f) / 255f, ((255 - projectile.alpha) * 0f) / 255f);
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	if (projectile.ai[1] == 0f)
			{
				projectile.ai[1] = 1f;
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
			}
        	float num953 = revenge ? 100f : 80f; //100
        	float scaleFactor12 = revenge ? 20f : 15f; //5
			float num954 = 40f;
			int num959 = (int)projectile.ai[0];
			if (num959 >= 0 && Main.player[num959].active && !Main.player[num959].dead) 
			{
				if (projectile.Distance(Main.player[num959].Center) > num954) 
				{
					Vector2 vector102 = projectile.DirectionTo(Main.player[num959].Center);
					if (vector102.HasNaNs()) 
					{
						vector102 = Vector2.UnitY;
					}
					projectile.velocity = (projectile.velocity * (num953 - 1f) + vector102 * scaleFactor12) / num953;
					return;
				}
			} 
			else 
			{
				if (projectile.ai[0] != -1f) 
				{
					projectile.ai[0] = -1f;
					projectile.netUpdate = true;
					return;
				}
			}
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 50, 50, projectile.alpha);
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
        	target.AddBuff(mod.BuffType("AbyssalFlames"), 240);
            target.AddBuff(mod.BuffType("VulnerabilityHex"), 180, true);
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
        	float spread = 45f * 0.0174f;
			double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
	    	double deltaAngle = spread / 8f;
	    	double offsetAngle;
	    	int i;
	    	if (projectile.owner == Main.myPlayer)
	    	{
		    	for (i = 0; i < 8; i++ )
		    	{
		   			offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
		        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 7f ), (float)( Math.Cos(offsetAngle) * 7f ), mod.ProjectileType("BrimstoneBarrage"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
		        	Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 7f ), (float)( -Math.Cos(offsetAngle) * 7f ), mod.ProjectileType("BrimstoneBarrage"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 1f);
		    	}
	    	}
        	for (int dust = 0; dust <= 5; dust++)
        	{
        		Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 235, 0f, 0f);
        	}
        }
    }
}