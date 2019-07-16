using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
    public class ScarletDevilBullet : ModProjectile
    {
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gungnir Bullet");
		}
		
        public override void SetDefaults()
        {
            projectile.width = 16;
            projectile.height = 16;
            projectile.friendly = true;
			projectile.timeLeft = 140;
            projectile.tileCollide = false;
            projectile.ignoreWater = true;
			projectile.GetGlobalProjectile<CalamityGlobalProjectile>(mod).rogue = true;
        }

        public override void AI()
        {
            projectile.ai[0] += 1f;
        	if (projectile.ai[0] <= 60f)
        	{
				projectile.velocity.X *= 0.975f;
				projectile.velocity.Y *= 0.975f;
			}
			else
			{
				float centerX = projectile.Center.X;
				float centerY = projectile.Center.Y;
				float num474 = 1000f;
				bool homeIn = false;
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, false))
					{
						float num476 = Main.npc[i].position.X + (float)(Main.npc[i].width / 2);
						float num477 = Main.npc[i].position.Y + (float)(Main.npc[i].height / 2);
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
					float num483 = 30f;
					Vector2 vector35 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
					float num484 = centerX - vector35.X;
					float num485 = centerY - vector35.Y;
					float num486 = (float)Math.Sqrt((double)(num484 * num484 + num485 * num485));
					num486 = num483 / num486;
					num484 *= num486;
					num485 *= num486;
					projectile.velocity.X = (projectile.velocity.X * 10f + num484) / 11f;
					projectile.velocity.Y = (projectile.velocity.Y * 10f + num485) / 11f;
					return;
				}
				else
				{
					projectile.velocity.X = 0f;
					projectile.velocity.Y = 0f;
				}
			}
        }
		
		public override Color? GetAlpha(Color lightColor)
        {
            return new Color(250, 250, 250);
        }
    }
}
