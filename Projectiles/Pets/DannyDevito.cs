using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Projectiles.Pets
{
    public class DannyDevito : ModProjectile
	{
		private int playerStill = 0;
		private bool fly = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Danny Devito");
			Main.projFrames[projectile.type] = 8;
			Main.projPet[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.netImportant = true;
			projectile.width = 42;
			projectile.height = 42;
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
			CalamityPlayer modPlayer = player.GetCalamityPlayer();
			if (player.dead)
			{
				modPlayer.trashMan = false;
			}
			if (modPlayer.trashMan)
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
				if (projectile.velocity.Y == 0 && (HoleBelow() || (playerDistance > 110f && projectile.position.X == projectile.oldPosition.X)))
				{
					projectile.velocity.Y = -5f;
				}
				projectile.velocity.Y += 0.20f;
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
						projectile.velocity.X += 0.10f;
						if (projectile.velocity.X > 7f)
						{
							projectile.velocity.X = 7f;
						}
					}
					else
					{
						projectile.velocity.X -= 0.10f;
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
					projectile.frame = 0;
				}
				else if (projectile.velocity.Y > 0.3f && projectile.position.Y != projectile.oldPosition.Y)
				{
					projectile.frame = 1;
					projectile.frameCounter = 0;
				}
				else
				{
					projectile.frameCounter++;
					if (projectile.frameCounter > 5)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
					}
					if (projectile.frame > 6)
					{
						projectile.frame = 2;
					}
					if (projectile.frame < 2)
					{
						projectile.frame = 2;
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
				float num21 = 18f;
				float num27 = (float)Math.Sqrt((double)(horiPos * horiPos + vertiPos * vertiPos));
				if (playerDistance > 1200f)
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
						++playerStill;
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
						projectile.velocity *= 0.90f;
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
				projectile.rotation = projectile.velocity.X * 0.03f;
				projectile.frame = 7;
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

		/*public override void PostDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (!Main.gamePaused || Main.gameMenu)
			{
				Vector2 vector69 = projectile.position - Main.screenPosition;
				if ((float)Main.mouseX > vector69.X && (float)Main.mouseX < vector69.X + (float)projectile.width && (float)Main.mouseY > vector69.Y && (float)Main.mouseY < vector69.Y + (float)projectile.height)
				{
					int num345 = (int)(Main.player[projectile.owner].Center.X / 16f);
					int num346 = (int)(Main.player[projectile.owner].Center.Y / 16f);
					int num347 = (int)projectile.Center.X / 16;
					int num348 = (int)projectile.Center.Y / 16;
					int lastTileRangeX = Main.player[projectile.owner].lastTileRangeX;
					int lastTileRangeY = Main.player[projectile.owner].lastTileRangeY;
					if (num345 >= num347 - lastTileRangeX && num345 <= num347 + lastTileRangeX + 1 && num346 >= num348 - lastTileRangeY && num346 <= num348 + lastTileRangeY + 1)
					{
						Main.player[projectile.owner].noThrow = 2;
						Main.player[projectile.owner].showItemIcon = true;
						Main.player[projectile.owner].showItemIcon2 = 3213;
						if (PlayerInput.UsingGamepad)
						{
							Main.player[projectile.owner].GamepadEnableGrappleCooldown();
						}
						if (Main.mouseRight && Main.mouseRightRelease && Player.StopMoneyTroughFromWorking == 0)
						{
							Main.mouseRightRelease = false;
							if (Main.player[projectile.owner].chest == -2)
							{
								Main.PlaySound(SoundID.Item11, -1, -1);
								Main.player[projectile.owner].chest = -1;
								Recipe.FindRecipes();
								return;
							}
							Main.player[projectile.owner].GetCalamityPlayer().trashManChest = projectile.type;
							Main.player[projectile.owner].chest = -2;
							Main.player[projectile.owner].chestX = (int)(projectile.Center.X / 16f);
							Main.player[projectile.owner].chestY = (int)(projectile.Center.Y / 16f);
							Main.player[projectile.owner].talkNPC = -1;
							Main.npcShop = 0;
							Main.playerInventory = true;
							Main.PlaySound(SoundID.Item10, -1, -1);
							Recipe.FindRecipes();
						}
					}
				}
			}
		}*/

		private bool HoleBelow() //pretty much the same as the one used in mantis
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
