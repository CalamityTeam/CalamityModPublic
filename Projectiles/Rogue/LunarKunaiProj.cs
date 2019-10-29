using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Rogue
{
    public class LunarKunaiProj : ModProjectile
    {
		bool lunarEnhance = false;
		
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kunai");
			Main.projFrames[projectile.type] = 2;
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 36;
            projectile.friendly = true;
            projectile.penetrate = 1;
            projectile.timeLeft = 300;
            projectile.Calamity().rogue = true;
		}

        public override void AI()
        {
            CalamityPlayer modPlayer = Main.player[Main.myPlayer].Calamity();
        	projectile.ai[0] += 1f;
			if(projectile.ai[0] == 1f && modPlayer.StealthStrikeAvailable())
				lunarEnhance = true;
			else if (projectile.ai[0] >= 50f)
				lunarEnhance = true;
			
			if (lunarEnhance)
				projectile.frame = 1;
			else
				projectile.frame = 0;
			
			projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
        	float num472 = projectile.Center.X;
			float num473 = projectile.Center.Y;
			float num474 = (lunarEnhance ? 1000f : 500f);
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
				float num483 = (lunarEnhance ? 35f : 15f);
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
            if (Main.rand.Next(6) == 0 && lunarEnhance)
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 229, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f);
        }
        
        public override void Kill(int timeLeft)
        {
			if (lunarEnhance)
			{
				Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
				projectile.position = projectile.Center;
				projectile.width = (projectile.height = 32);
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				projectile.damage /= 4;
				for (int num194 = 0; num194 < 10; num194++)
				{
					int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 0, default(Color), 1.5f);
					Main.dust[num195].noGravity = true;
					Main.dust[num195].velocity *= 3f;
					num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 229, 0f, 0f, 100, default(Color), 1f);
					Main.dust[num195].velocity *= 2f;
					Main.dust[num195].noGravity = true;
				}
				projectile.Damage();
			}
			else
			{
				for (int i = 0; i < 5; i++)
				{
					int num304 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 265, 0f, 0f, 100, default(Color), 1f);
					Main.dust[num304].noGravity = true;
					Main.dust[num304].velocity *= 1.2f;
					Main.dust[num304].velocity -= projectile.oldVelocity * 0.3f;
				}
			}
        }
    }
}