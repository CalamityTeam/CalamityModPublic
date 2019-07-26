using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class ElementBallShiv : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ball");
		}

        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.melee = true;
            projectile.penetrate = 1;
            projectile.tileCollide = false;
            projectile.timeLeft = 60;
        }

        public override void AI()
        {
			projectile.localAI[0] += 1f;
			if (projectile.localAI[0] > 4f)
			{
				for (int num468 = 0; num468 < 3; num468++)
				{
					int num250 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.3f);
					Main.dust[num250].noGravity = true;
					Main.dust[num250].velocity *= 0f;
				}
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
				float num483 = Main.rand.Next(35, 40);
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = num472 - vector35.X;
				float num485 = num473 - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 20f + num484) / 21f;
				projectile.velocity.Y = (projectile.velocity.Y * 20f + num485) / 21f;
			}
        }

        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 7; k++)
            {
            	int num = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 66, (float)(projectile.direction * 2), 0f, 150, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
            	Main.dust[num].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	float xPos = projectile.ai[0] > 0 ? projectile.position.X + 800 : projectile.position.X - 800;
    		Vector2 vector2 = new Vector2(xPos, projectile.position.Y + Main.rand.Next(-800, 801));

    		float num80 = xPos;
    		float speedX = (float)target.position.X - vector2.X;
    		float speedY = (float)target.position.Y - vector2.Y;
    		float dir = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
    		dir = 10 / num80;
    		float random = (float)Main.rand.Next(1, 150);
    		if (projectile.owner == Main.myPlayer)
    		{
	    		for (int i = 0; i <= 3; i++)
	    		{
	    			speedX *= dir * random;
	    			speedY *= dir * random;
	    			Projectile.NewProjectile(vector2.X, vector2.Y, speedX, speedY, mod.ProjectileType("SHIV"), projectile.damage, 1f, projectile.owner);
	    		}
    		}
        	target.AddBuff(mod.BuffType("HolyLight"), 120);
	    	target.AddBuff(mod.BuffType("GlacialState"), 120);
	    	target.AddBuff(mod.BuffType("BrimstoneFlames"), 120);
	    	target.AddBuff(mod.BuffType("Plague"), 120);
        }
    }
}
