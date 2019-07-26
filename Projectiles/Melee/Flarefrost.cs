using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Melee
{
    public class Flarefrost : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Flarefrost");
		}

        public override void SetDefaults()
        {
            projectile.width = 10;
            projectile.height = 10;
            projectile.friendly = true;
            projectile.alpha = 255;
            projectile.penetrate = 1;
            projectile.timeLeft = 180;
            projectile.melee = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.25f) / 255f, ((255 - projectile.alpha) * 0.02f) / 255f, ((255 - projectile.alpha) * 0.01f) / 255f);
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.01f) / 255f, ((255 - projectile.alpha) * 0.15f) / 255f, ((255 - projectile.alpha) * 0.25f) / 255f);
        	for (int num92 = 0; num92 < 3; num92++)
			{
				float num93 = projectile.velocity.X / 3f * (float)num92;
				float num94 = projectile.velocity.Y / 3f * (float)num92;
				int num95 = 4;
				int num96 = Dust.NewDust(new Vector2(projectile.position.X + (float)num95, projectile.position.Y + (float)num95), projectile.width - num95 * 2, projectile.height - num95 * 2, 67, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num96].noGravity = true;
				Main.dust[num96].velocity *= 0.1f;
				Main.dust[num96].velocity += projectile.velocity * 0.1f;
				Dust expr_47FA_cp_0 = Main.dust[num96];
				expr_47FA_cp_0.position.X = expr_47FA_cp_0.position.X - num93;
				Dust expr_4815_cp_0 = Main.dust[num96];
				expr_4815_cp_0.position.Y = expr_4815_cp_0.position.Y - num94;
			}
			if (Main.rand.Next(5) == 0)
			{
				int num97 = 4;
				int num98 = Dust.NewDust(new Vector2(projectile.position.X + (float)num97, projectile.position.Y + (float)num97), projectile.width - num97 * 2, projectile.height - num97 * 2, 67, 0f, 0f, 100, default(Color), 0.6f);
				Main.dust[num98].velocity *= 0.25f;
				Main.dust[num98].velocity += projectile.velocity * 0.5f;
			}
			for (int num105 = 0; num105 < 3; num105++)
			{
				float num99 = projectile.velocity.X / 3f * (float)num105;
				float num100 = projectile.velocity.Y / 3f * (float)num105;
				int num101 = 4;
				int num102 = Dust.NewDust(new Vector2(projectile.position.X + (float)num101, projectile.position.Y + (float)num101), projectile.width - num101 * 2, projectile.height - num101 * 2, 6, 0f, 0f, 100, default(Color), 1.2f);
				Main.dust[num102].noGravity = true;
				Main.dust[num102].velocity *= 0.1f;
				Main.dust[num102].velocity += projectile.velocity * 0.1f;
				Dust expr_47FA_cp_0 = Main.dust[num102];
				expr_47FA_cp_0.position.X = expr_47FA_cp_0.position.X - num99;
				Dust expr_4815_cp_0 = Main.dust[num102];
				expr_4815_cp_0.position.Y = expr_4815_cp_0.position.Y - num100;
			}
			if (Main.rand.Next(5) == 0)
			{
				int num103 = 4;
				int num104 = Dust.NewDust(new Vector2(projectile.position.X + (float)num103, projectile.position.Y + (float)num103), projectile.width - num103 * 2, projectile.height - num103 * 2, 6, 0f, 0f, 100, default(Color), 0.6f);
				Main.dust[num104].velocity *= 0.25f;
				Main.dust[num104].velocity += projectile.velocity * 0.5f;
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
				float num483 = 11f;
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
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 27);
            for (int k = 0; k < 2; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 67, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 6, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
        	target.AddBuff(BuffID.OnFire, 300);
        	target.AddBuff(BuffID.Frostburn, 300);
        }
    }
}
