using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Patreon
{
    public class LightBead : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Light Bead");
		}
    	
        public override void SetDefaults()
        {
            projectile.width = 8;
            projectile.height = 8;
            projectile.friendly = true;
            projectile.alpha = 50;
            projectile.scale = 1.2f;
            projectile.penetrate = 1;
            projectile.timeLeft = 600;
            projectile.magic = true;
        }

        public override void AI()
        {
        	Lighting.AddLight(projectile.Center, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f, ((255 - projectile.alpha) * 0.5f) / 255f);
			projectile.rotation += projectile.velocity.X * 0.2f;
			projectile.ai[1] += 1f;
			if (Main.rand.Next(6) == 0)
			{
				int num300 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 212, 0f, 0f, 0, default(Color), 1f);
				Main.dust[num300].noGravity = true;
				Main.dust[num300].velocity *= 0.5f;
				Main.dust[num300].scale *= 0.9f;
			}
			if (projectile.ai[1] > 300f)
			{
				projectile.scale -= 0.05f;
				if ((double)projectile.scale <= 0.2)
				{
					projectile.scale = 0.2f;
					projectile.Kill();
					return;
				}
			}
			float centerX = projectile.Center.X;
			float centerY = projectile.Center.Y;
			float num474 = 500f;
			bool homeIn = false;
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
						centerX = num476;
						centerY = num477;
						homeIn = true;
					}
				}
			}
			if (homeIn)
			{
				float num483 = 18f;
				Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float num484 = centerX - vector35.X;
				float num485 = centerY - vector35.Y;
				float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
				num486 = num483 / num486;
				num484 *= num486;
				num485 *= num486;
				projectile.velocity.X = (projectile.velocity.X * 10f + num484) / 11f;
				projectile.velocity.Y = (projectile.velocity.Y * 10f + num485) / 11f;
			}
        }
        
        public override void Kill(int timeLeft)
        {
            for (int k = 0; k < 3; k++)
            {
            	Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 212, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f);
            }
			int num251 = Main.rand.Next(2, 3);
        	if (projectile.owner == Main.myPlayer)
        	{
				for (int num252 = 0; num252 < num251; num252++)
				{
					Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					while (value15.X == 0f && value15.Y == 0f)
					{
						value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
					}
					value15.Normalize();
					value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
					Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, mod.ProjectileType("LightBeadSplit"), (int)((double)projectile.damage * 0.5), 0f, projectile.owner, 0f, 0f);
				}
        	}
        }
    }
}