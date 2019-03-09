using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class SirenYoung : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Siren");
            Main.projFrames[projectile.type] = 4;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }
    	
        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 30;
            projectile.height = 30;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                if (projectile.localAI[0] == 0f)
                {
                    Lighting.AddLight(projectile.Center, 2.5f, 2f, 0f); //4.5
                }
                else
                {
                    Lighting.AddLight(projectile.Center, 1.65f, 1.32f, 0f); //3
                }
            }
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            if (player.dead)
            {
                modPlayer.sirenPet = false;
            }
            if (modPlayer.sirenPet)
            {
                projectile.timeLeft = 2;
            }
            float num23 = 0.2f;
            float num24 = 5f;
            projectile.tileCollide = false;
            Vector2 vector4 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num25 = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector4.X;
            float num26 = Main.player[projectile.owner].position.Y + Main.player[projectile.owner].gfxOffY + (float)(Main.player[projectile.owner].height / 2) - vector4.Y;
            if (Main.player[projectile.owner].controlLeft)
            {
                num25 -= 120f;
            }
            else if (Main.player[projectile.owner].controlRight)
            {
                num25 += 120f;
            }
            if (Main.player[projectile.owner].controlDown)
            {
                num26 += 120f;
            }
            else
            {
                if (Main.player[projectile.owner].controlUp)
                {
                    num26 -= 120f;
                }
                num26 -= 60f;
            }
            float num27 = (float)Math.Sqrt((double)(num25 * num25 + num26 * num26));
            if (num27 > 1000f)
            {
                projectile.position.X = projectile.position.X + num25;
                projectile.position.Y = projectile.position.Y + num26;
            }
            if (projectile.localAI[0] == 1f)
            {
                if (num27 < 10f && Math.Abs(Main.player[projectile.owner].velocity.X) + Math.Abs(Main.player[projectile.owner].velocity.Y) < num24 && Main.player[projectile.owner].velocity.Y == 0f)
                {
                    projectile.localAI[0] = 0f;
                }
                num24 = 12f;
                if (num27 < num24)
                {
                    projectile.velocity.X = num25;
                    projectile.velocity.Y = num26;
                }
                else
                {
                    num27 = num24 / num27;
                    projectile.velocity.X = num25 * num27;
                    projectile.velocity.Y = num26 * num27;
                }
                if ((double)projectile.velocity.X > 0.5)
                {
                    projectile.direction = -1;
                }
                else if ((double)projectile.velocity.X < -0.5)
                {
                    projectile.direction = 1;
                }
                projectile.spriteDirection = projectile.direction;
                projectile.rotation = projectile.velocity.X * 0.05f;
                projectile.frameCounter++;
                if (projectile.frameCounter > 6)
                {
                    projectile.frame++;
                    projectile.frameCounter = 0;
                }
                if (projectile.frame > 3)
                {
                    projectile.frame = 0;
                    return;
                }
                return;
            }
            if (num27 > 200f)
            {
                projectile.localAI[0] = 1f;
            }
            if ((double)projectile.velocity.X > 0.5)
            {
                projectile.direction = -1;
            }
            else if ((double)projectile.velocity.X < -0.5)
            {
                projectile.direction = 1;
            }
            projectile.spriteDirection = projectile.direction;
            if (num27 < 10f)
            {
                projectile.velocity.X = num25;
                projectile.velocity.Y = num26;
                projectile.rotation = projectile.velocity.X * 0.05f;
                if (num27 < num24)
                {
                    projectile.position += projectile.velocity;
                    projectile.velocity *= 0f;
                    num23 = 0f;
                }
                projectile.direction = -Main.player[projectile.owner].direction;
            }
            num27 = num24 / num27;
            num25 *= num27;
            num26 *= num27;
            if (projectile.velocity.X < num25)
            {
                projectile.velocity.X = projectile.velocity.X + num23;
                if (projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.99f;
                }
            }
            if (projectile.velocity.X > num25)
            {
                projectile.velocity.X = projectile.velocity.X - num23;
                if (projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.99f;
                }
            }
            if (projectile.velocity.Y < num26)
            {
                projectile.velocity.Y = projectile.velocity.Y + num23;
                if (projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.99f;
                }
            }
            if (projectile.velocity.Y > num26)
            {
                projectile.velocity.Y = projectile.velocity.Y - num23;
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.99f;
                }
            }
            if (projectile.velocity.X != 0f || projectile.velocity.Y != 0f)
            {
                projectile.rotation = projectile.velocity.X * 0.05f;
            }
            projectile.frameCounter++;
            if (projectile.frameCounter > 6)
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
    }
}