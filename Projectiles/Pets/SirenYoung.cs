using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
    public class SirenYoung : ModProjectile
    {
        private bool underwater = false;
		private int sleepyTimer = 0;
		private int lightLevel = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ocean Spirit");
            Main.projFrames[projectile.type] = 17;
            Main.projPet[projectile.type] = true;
            ProjectileID.Sets.LightPet[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.netImportant = true;
            projectile.width = 38;
            projectile.height = 58;
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.timeLeft *= 5;
            projectile.ignoreWater = true;
			projectile.tileCollide = false;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (!player.active)
            {
                projectile.active = false;
                return;
            }
            CalamityPlayer modPlayer = player.Calamity();
            if (player.dead)
            {
                modPlayer.sirenPet = false;
            }
            if (modPlayer.sirenPet)
            {
                projectile.timeLeft = 2;
            }
			if (sleepyTimer < 180)
			{
				projectile.frameCounter++;
				if (projectile.frameCounter > 6)
				{
					projectile.frame++;
					projectile.frameCounter = 0;
				}
			}
			if (underwater)
			{
				if (projectile.frame >= 8)
				{
					projectile.frame = 0;
				}
			}
			else
			{
				if (projectile.frame >= 16)
				{
					projectile.frame = sleepyTimer >= 180 ? 16 : 8;
				}
			}
            underwater = player.IsUnderwater() || modPlayer.leviathanAndSirenLore;
            if (underwater)
            {
				if (projectile.frame == 16)
					projectile.frame = 0;
				if (sleepyTimer > 0)
					sleepyTimer--;
                if (projectile.localAI[0] == 0f)
                {
                    lightLevel = 0;
                }
                else
                {
                    lightLevel = 1;
                }
            }
			else
			{
				if (sleepyTimer < 180)
				{
					sleepyTimer++;
					lightLevel = 1;
				}
				else
				{
					lightLevel = 2;
					projectile.frame = 16;
				}
			}
			switch (lightLevel)
			{
				case 0:
					Lighting.AddLight(projectile.Center, 0f, 2f, 2.5f); //4.5
					break;
				case 1:
					Lighting.AddLight(projectile.Center, 0f, 1.32f, 1.65f); //3
					break;
				case 2:
					Lighting.AddLight(projectile.Center, 0f, 0.5f, 0.7f);
					break;
			}
			
            float num23 = 0.2f;
            float num24 = 5f;
            Vector2 vector4 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
            float num25 = player.position.X + (float)(player.width / 2) - vector4.X;
            float num26 = player.position.Y + player.gfxOffY + (float)(player.height / 2) - vector4.Y;
            if (player.controlLeft && sleepyTimer < 180)
            {
                num25 -= 120f;
            }
            else if (player.controlRight && sleepyTimer < 180)
            {
                num25 += 120f;
            }
            if (player.controlDown && sleepyTimer < 180)
            {
                num26 += 120f;
            }
            else
            {
                if (player.controlUp && sleepyTimer < 180)
                {
                    num26 -= 120f;
                }
                num26 -= 60f;
            }
            float num27 = (float)Math.Sqrt((double)(num25 * num25 + num26 * num26));
            if (num27 > 1000f)
            {
                projectile.position.X += num25;
                projectile.position.Y += num26;
            }
            if (projectile.localAI[0] == 1f)
            {
                if (num27 < 10f && Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y) < num24 && player.velocity.Y == 0f)
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
                if (projectile.velocity.X > 0.5f)
                {
                    projectile.direction = -1;
                }
                else if (projectile.velocity.X < -0.5f)
                {
                    projectile.direction = 1;
                }
                projectile.spriteDirection = projectile.direction;
                projectile.rotation = projectile.velocity.X * 0.05f;
                return;
            }
            if (num27 > 200f)
            {
                projectile.localAI[0] = 1f;
            }
            if (projectile.velocity.X > 0.5f)
            {
                projectile.direction = -1;
            }
            else if (projectile.velocity.X < -0.5f)
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
                projectile.direction = -player.direction;
            }
            num27 = num24 / num27;
            num25 *= num27;
            num26 *= num27;
            if (projectile.velocity.X < num25)
            {
                projectile.velocity.X += num23;
                if (projectile.velocity.X < 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.99f;
                }
            }
            if (projectile.velocity.X > num25)
            {
                projectile.velocity.X -= num23;
                if (projectile.velocity.X > 0f)
                {
                    projectile.velocity.X = projectile.velocity.X * 0.99f;
                }
            }
            if (projectile.velocity.Y < num26)
            {
                projectile.velocity.Y += num23;
                if (projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.99f;
                }
            }
            if (projectile.velocity.Y > num26)
            {
                projectile.velocity.Y -= num23;
                if (projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y = projectile.velocity.Y * 0.99f;
                }
            }
            if (projectile.velocity.X != 0f || projectile.velocity.Y != 0f)
            {
                projectile.rotation = projectile.velocity.X * 0.05f;
            }
		}
    }
}
