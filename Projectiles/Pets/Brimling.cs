using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class Brimling : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimling");
            Main.projFrames[projectile.type] = 8;
            Main.projPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 62;
            projectile.height = 60;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (player.dead)
            {
                modPlayer.brimling = false;
            }
            if (modPlayer.brimling)
            {
                projectile.timeLeft = 4;
            }
            float num16 = 0.5f;
            projectile.tileCollide = false;
            int num17 = 100;
            Vector2 vector3 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num18 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector3.X;
            float num19 = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector3.Y;
            num19 += (float)Main.rand.Next(-10, 21);
            num18 += (float)Main.rand.Next(-10, 21);
            num18 += (float)(60 * -(float)Main.player[projectile.owner].direction);
            num19 -= 60f;
            float num20 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            float num21 = 18f;

			//Limites de mouvement ici

			float num27 = (float)Math.Sqrt((double)(num18 * num18 + num19 * num19));
            if (num27 > 1000f)
            {
                projectile.position.X = projectile.position.X + num18;
                projectile.position.Y = projectile.position.Y + num19;
				for (int k = 0; k < 10; k++)
				{
					Dust.NewDust(projectile.position, projectile.width, projectile.height, 235, 0, -1f, 0, default(Color), 1f);
				}
            }

            if (num20 < (float)num17 && Main.player[projectile.owner].velocity.Y == 0f &&
                projectile.position.Y + (float)projectile.height <= Main.player[projectile.owner].position.Y + (float)Main.player[projectile.owner].height &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                projectile.ai[0] = 0f;
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }
            if (num20 < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.90f;
                }
                num16 = 0.01f;
            }
            else
            {
                if (num20 < 100f)
                {
                    num16 = 0.1f;
                }
                if (num20 > 300f)
                {
                    num16 = 1f;
                }
                num20 = num21 / num20;
                num18 *= num20;
                num19 *= num20;
            }

			//Les changements de velocité ici
            if (projectile.velocity.X < num18)
            {
                projectile.velocity.X = projectile.velocity.X + num16;
                if (num16 > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X + num16;
                }
            }
            if (projectile.velocity.X > num18)
            {
                projectile.velocity.X = projectile.velocity.X - num16;
                if (num16 > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X - num16;
                }
            }
            if (projectile.velocity.Y < num19)
            {
                projectile.velocity.Y = projectile.velocity.Y + num16;
                if (num16 > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y + num16 * 2f;
                }
            }
            if (projectile.velocity.Y > num19)
            {
                projectile.velocity.Y = projectile.velocity.Y - num16;
                if (num16 > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y - num16 * 2f;
                }
            }
            if ((double)projectile.velocity.X > 0.25)
            {
                projectile.direction = -1;
            }
            else if ((double)projectile.velocity.X < -0.25)
            {
                projectile.direction = 1;
            }

			//On gère le sprite et les frames ici

			Player projOwner = Main.player[projectile.owner];

            projectile.spriteDirection = -projOwner.direction;
            projectile.rotation = projectile.velocity.X * 0.03f;

			projectile.frameCounter++;

            if (projOwner.statLife >= projOwner.statLifeMax2 / 4)
            {
                if (projectile.frameCounter > 5)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame > 3)
                {
                    projectile.frame = 0;
                    return;
                }
            }
            else
            {
                if (projectile.frameCounter > 5)

                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame > 7)
                {
                    projectile.frame = 4;
                    return;
                }
            }
		}
    }
}
