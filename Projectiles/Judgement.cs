using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles
{
    public class Judgement : ModProjectile
    {
    	int whiteLightTimer = 5;
    	
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Judgement");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 12;
            projectile.height = 12;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.penetrate = -1;
            projectile.timeLeft = 60;
            projectile.melee = true;
        }

        public override void AI()
        {
        	whiteLightTimer--;
        	if (whiteLightTimer == 0)
        	{
				float spread = 180f * 0.0174f;
				double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y)- spread/2;
				double deltaAngle = spread/8f;
				double offsetAngle;
				int i;
				if (projectile.owner == Main.myPlayer)
				{
					for (i = 0; i < 1; i++ )
					{
					   	offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					   	int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("WhiteOrb"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
					    int projectile2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("WhiteOrb"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
						Main.projectile[projectile1].velocity.X *= 0.1f;
					    Main.projectile[projectile1].velocity.Y *= 0.1f;
					    Main.projectile[projectile2].velocity.X *= 0.1f;
					    Main.projectile[projectile2].velocity.Y *= 0.1f;
					}
				}
				whiteLightTimer = 5;
        	}
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f);
			for (int num457 = 0; num457 < 2; num457++)
			{
				int num458 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 91, 0f, 0f, 100, default(Color), 1.25f);
				Main.dust[num458].noGravity = true;
				Main.dust[num458].velocity *= 0.5f;
				Main.dust[num458].velocity += projectile.velocity * 0.1f;
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
				float num483 = 9f;
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
			return;
        }
        
        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 122);
        	if (projectile.owner == Main.myPlayer)
			{
        		Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y - 100, 0f, 0f, mod.ProjectileType("WhiteBoltAura"), projectile.damage, projectile.knockBack, projectile.owner, 0f, 0f);
        	}
        }
    }
}