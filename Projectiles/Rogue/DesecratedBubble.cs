using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Projectiles.Rogue
{
    public class DesecratedBubble : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bubble");
        }

        public override void SetDefaults()
        {
            projectile.width = 20;
            projectile.height = 20;
            projectile.friendly = true;
            projectile.tileCollide = false;
            projectile.penetrate = 1;
            projectile.alpha = 255;
            projectile.timeLeft = 120;
            projectile.Calamity().rogue = true;
        }

        public override void AI()
        {
            projectile.scale += 0.002f;
            if (projectile.alpha <= 0)
            {
                projectile.alpha = 0;
            }
            else if (projectile.alpha > 50)
            {
                projectile.alpha -= 20;
            }
			if (projectile.timeLeft <= 100)
			{
				projectile.ai[1] = 0f;
			}
			else
			{
				projectile.velocity *= 0.995f;
			}
            if (Main.player[projectile.owner].active && !Main.player[projectile.owner].dead)
            {
				if (projectile.ai[1] == 0f)
				{
					float num472 = projectile.Center.X;
					float num473 = projectile.Center.Y;
					float num474 = 600f;
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
						float num483 = 8f;
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
            }
		}

        public override void Kill(int timeLeft)
        {
            Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 54);
            int num190 = Main.rand.Next(5, 9);
            for (int num191 = 0; num191 < num190; num191++)
            {
                int num192 = Dust.NewDust(projectile.Center, 0, 0, 179, 0f, 0f, 100, default, 1.4f);
                Main.dust[num192].velocity *= 0.8f;
                Main.dust[num192].position = Vector2.Lerp(Main.dust[num192].position, projectile.Center, 0.5f);
                Main.dust[num192].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
			if (projectile.ai[0] == 1f)
			{
				target.AddBuff(BuffID.Ichor, 180);
				target.AddBuff(BuffID.CursedInferno, 180);
			}
        }

        // Cannot deal damage for the first several frames of existence.
        public override bool CanDamage()
        {
            return projectile.timeLeft < 100;
        }
    }
}