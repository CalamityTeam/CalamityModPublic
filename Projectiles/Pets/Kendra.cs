using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Pets
{
    public class Kendra : ModProjectile
	{
		private int chosenIdle = 0;
		private int idleTimer = 0;
		private int idleBarkTimer = 0;
		private int playerStill = 0;
		private bool fly = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Kendra");
			Main.projFrames[projectile.type] = 31;
			Main.projPet[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.netImportant = true;
			projectile.width = 46;
			projectile.height = 46;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.timeLeft *= 5;
			projectile.tileCollide = true;
		}

		public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
		{
			Player player = Main.player[projectile.owner];
			Vector2 center2 = projectile.Center;
			Vector2 vector48 = player.Center - center2;
			float playerDistance = vector48.Length();
			fallThrough = playerDistance > 200f;
			return true;
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
				modPlayer.kendra = false;
			}
			if (modPlayer.kendra)
			{
				projectile.timeLeft = 2;
			}
			Vector2 vector46 = projectile.position;
			if (!fly)
			{
				projectile.rotation = 0;
				Vector2 center2 = projectile.Center;
				Vector2 vector48 = player.Center - center2;
				float playerDistance = vector48.Length();
				if (projectile.velocity.Y == 0f && (HoleBelow() || (playerDistance > 110f && projectile.position.X == projectile.oldPosition.X)))
				{
					projectile.velocity.Y = -5f;
				}
				projectile.velocity.Y += 0.2f;
				if (projectile.velocity.Y > 7f)
				{
					projectile.velocity.Y = 7f;
				}
				if (playerDistance > 600f)
				{
					fly = true;
					projectile.velocity.X = 0f;
					projectile.velocity.Y = 0f;
					projectile.tileCollide = false;
				}
				if (playerDistance > 100f)
				{
					if (player.position.X - projectile.position.X > 0f)
					{
						projectile.velocity.X += 0.1f;
						if (projectile.velocity.X > 7f)
						{
							projectile.velocity.X = 7f;
						}
					}
					else
					{
						projectile.velocity.X -= 0.1f;
						if (projectile.velocity.X < -7f)
						{
							projectile.velocity.X = -7f;
						}
					}
				}
				if (playerDistance < 100f)
				{
					if (projectile.velocity.X != 0f)
					{
						if (projectile.velocity.X > 0.5f)
						{
							projectile.velocity.X -= 0.15f;
						}
						else if (projectile.velocity.X < -0.5f)
						{
							projectile.velocity.X += 0.15f;
						}
						else if (projectile.velocity.X < 0.5f && projectile.velocity.X > -0.5f)
						{
							projectile.velocity.X = 0f;
						}
					}
				}
				if (projectile.position.X == projectile.oldPosition.X && projectile.position.Y == projectile.oldPosition.Y && projectile.velocity.X == 0)
				{
					projectile.frameCounter++;
					switch (chosenIdle)
					{
						case 1:
							if (idleTimer == 0)
							{
								projectile.frame = 0;
							}
							idleTimer++;
							if (projectile.frameCounter > 6)
							{
								projectile.frame++;
								projectile.frameCounter = 0;
							}
							if (projectile.frame > 4)
							{
								chosenIdle = 0;
							}
							if (projectile.frame < 1)
							{
								projectile.frame = 1;
							}
							break;
						case 2:
							if (idleTimer == 0)
							{
								projectile.frame = 0;
							}
							idleTimer++;
							if (projectile.frameCounter > 6)
							{
								projectile.frame++;
								projectile.frameCounter = 0;
							}
							if (projectile.frame > 13)
							{
								chosenIdle = 0;
							}
							if (projectile.frame < 5)
							{
								projectile.frame = 5;
							}
							break;
						case 3:
							if (idleTimer == 0)
							{
								projectile.frame = 0;
							}
							idleTimer++;
							if (projectile.frameCounter > 6)
							{
								projectile.frame++;
								projectile.frameCounter = 0;
							}
							if (projectile.frame > 24)
							{
								chosenIdle = 0;
							}
							if (projectile.frame < 19)
							{
								projectile.frame = 19;
							}
							break;
					}
					if (chosenIdle == 0)
					{
						projectile.frame = 0;
						projectile.frameCounter = 6;
						idleTimer++;
						if (idleTimer > 360)
						{
							chosenIdle = Main.rand.Next(1, 3);
							idleTimer = 0;
						}
						idleBarkTimer++;
						if (idleBarkTimer > 1080 && Main.rand.Next(2) == 0)
						{
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/KendraBark"), (int)projectile.position.X, (int)projectile.position.Y);
							chosenIdle = 3;
							idleBarkTimer = 0;
							idleTimer = 0;
						}
					}
				}
				else if (projectile.velocity.Y > 0.3f && projectile.position.Y != projectile.oldPosition.Y)
				{
					projectile.frame = 16;
					projectile.frameCounter = 0;
				}
				else
				{
					projectile.frameCounter++;
					if (projectile.frameCounter > 6)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
					}
					if (projectile.frame > 18)
					{
						projectile.frame = 14;
					}
					if (projectile.frame < 14)
					{
						projectile.frame = 14;
					}
				}
			}
			else if (fly)
			{
				float num16 = 0.3f;
				projectile.tileCollide = false;
				Vector2 vector3 = new Vector2(projectile.position.X + (float)projectile.width * 0.5f, projectile.position.Y + (float)projectile.height * 0.5f);
				float horiPos = Main.player[projectile.owner].position.X + (float)(Main.player[projectile.owner].width / 2) - vector3.X;
				float vertiPos = Main.player[projectile.owner].position.Y + (float)(Main.player[projectile.owner].height / 2) - vector3.Y;
				vertiPos += (float)Main.rand.Next(-10, 21);
				horiPos += (float)Main.rand.Next(-10, 21);
				horiPos += (float)(60 * -(float)Main.player[projectile.owner].direction);
				vertiPos -= 60f;
				float playerDistance = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
				float num21 = 18.5f;
				float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
				if (playerDistance > 2000f)
				{
					projectile.position.X = Main.player[projectile.owner].Center.X - (float)(projectile.width / 2);
					projectile.position.Y = Main.player[projectile.owner].Center.Y - (float)(projectile.height / 2);
					projectile.netUpdate = true;
				}
				if (playerDistance < 100f)
				{
					num16 = 0.1f;
					if (player.velocity.Y == 0f)
					{
						playerStill++;
					}
					else
					{
						playerStill = 0;
					}
					if (playerStill > 60 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					{
						fly = false;
						projectile.tileCollide = true;
					}
				}
				if (playerDistance < 50f)
				{
					if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
					{
						projectile.velocity *= 0.9f;
					}
					num16 = 0.01f;
				}
				else
				{
					if (playerDistance < 100f)
					{
						num16 = 0.1f;
					}
					if (playerDistance > 300f)
					{
						num16 = 1f;
					}
					playerDistance = num21 / playerDistance;
					horiPos *= playerDistance;
					vertiPos *= playerDistance;
				}
				if (projectile.velocity.X <= horiPos)
				{
					projectile.velocity.X = projectile.velocity.X + num16;
					if (num16 > 0.05f && projectile.velocity.X < 0f)
					{
						projectile.velocity.X = projectile.velocity.X + num16;
					}
				}
				if (projectile.velocity.X > horiPos)
				{
					projectile.velocity.X = projectile.velocity.X - num16;
					if (num16 > 0.05f && projectile.velocity.X > 0f)
					{
						projectile.velocity.X = projectile.velocity.X - num16;
					}
				}
				if (projectile.velocity.Y <= vertiPos)
				{
					projectile.velocity.Y = projectile.velocity.Y + num16;
					if (num16 > 0.05f && projectile.velocity.Y < 0f)
					{
						projectile.velocity.Y = projectile.velocity.Y + num16 * 2f;
					}
				}
				if (projectile.velocity.Y > vertiPos)
				{
					projectile.velocity.Y = projectile.velocity.Y - num16;
					if (num16 > 0.05f && projectile.velocity.Y > 0f)
					{
						projectile.velocity.Y = projectile.velocity.Y - num16 * 2f;
					}
				}
				projectile.rotation = projectile.velocity.X * 0.015f;
				projectile.frameCounter++;
				if (projectile.frameCounter > 6)
				{
					projectile.frame++;
					projectile.frameCounter = 0;
				}
				if (projectile.frame > 30)
				{
					projectile.frame = 25;
				}
				if (projectile.frame < 25)
				{
					projectile.frame = 25;
				}
			}
			if (projectile.velocity.X > 0.25f)
			{
				projectile.spriteDirection = -1;
			}
			else if (projectile.velocity.X < -0.25f)
			{
				projectile.spriteDirection = 1;
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			SpriteEffects spriteEffects = (projectile.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None);
			Texture2D texture2D13 = Main.projectileTexture[projectile.type];
			int num214 = Main.projectileTexture[projectile.type].Height / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, spriteEffects, 0f);
			return false;
		}

		private bool HoleBelow()
		{
			int tileWidth = 4;
			int tileX = (int)(projectile.Center.X / 16f) - tileWidth;
			if (projectile.velocity.X > 0)
			{
				tileX += tileWidth;
			}
			int tileY = (int)((projectile.position.Y + projectile.height) / 16f);
			for (int y = tileY; y < tileY + 2; y++)
			{
				for (int x = tileX; x < tileX + tileWidth; x++)
				{
					if (Main.tile[x, y].active())
					{
						return false;
					}
				}
			}
			return true;
		}
	}
}
