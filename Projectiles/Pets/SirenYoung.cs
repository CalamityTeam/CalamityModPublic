using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Pets
{
	public class SirenYoung : ModProjectile
	{
		private bool underwater = false;

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
			projectile.frameCounter++;
			if (projectile.frameCounter > 6)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
			{
				projectile.frame = 0;
			}
			underwater = Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) || modPlayer.leviathanAndSirenLore;
			if (underwater)
			{
				projectile.width = 30;
				projectile.height = 30;
				if (projectile.localAI[0] == 0f)
				{
					Lighting.AddLight(projectile.Center, 2.5f, 2f, 0f); //4.5
				}
				else
				{
					Lighting.AddLight(projectile.Center, 1.65f, 1.32f, 0f); //3
				}
			}
			else
			{
				projectile.width = 54;
				projectile.height = 54;
				Lighting.AddLight(projectile.Center, 0.825f, 0.66f, 0f); //1.5
				Vector2 vector54 = player.Center;
				vector54.X -= (float)((15 + player.width / 2) * player.direction);
				vector54.X -= (float)(40 * player.direction);
				if (projectile.ai[0] == 1f)
				{
					projectile.tileCollide = false;
					float num663 = 0.2f;
					float num664 = 10f;
					int num665 = 200;
					if (num664 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
					{
						num664 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					}
					Vector2 vector58 = vector54 - projectile.Center;
					float num666 = vector58.Length();
					if (num666 > 2000f)
					{
						projectile.position = player.Center - new Vector2((float)projectile.width, (float)projectile.height) / 2f;
					}
					if (num666 < (float)num665 && player.velocity.Y == 0f && projectile.position.Y + (float)projectile.height <= player.position.Y + (float)player.height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					{
						projectile.ai[0] = 0f;
						projectile.netUpdate = true;
						if (projectile.velocity.Y < -6f)
						{
							projectile.velocity.Y = -6f;
						}
					}
					if (num666 >= 60f)
					{
						vector58.Normalize();
						vector58 *= num664;
						if (projectile.velocity.X < vector58.X)
						{
							projectile.velocity.X = projectile.velocity.X + num663;
							if (projectile.velocity.X < 0f)
							{
								projectile.velocity.X = projectile.velocity.X + num663 * 1.5f;
							}
						}
						if (projectile.velocity.X > vector58.X)
						{
							projectile.velocity.X = projectile.velocity.X - num663;
							if (projectile.velocity.X > 0f)
							{
								projectile.velocity.X = projectile.velocity.X - num663 * 1.5f;
							}
						}
						if (projectile.velocity.Y < vector58.Y)
						{
							projectile.velocity.Y = projectile.velocity.Y + num663;
							if (projectile.velocity.Y < 0f)
							{
								projectile.velocity.Y = projectile.velocity.Y + num663 * 1.5f;
							}
						}
						if (projectile.velocity.Y > vector58.Y)
						{
							projectile.velocity.Y = projectile.velocity.Y - num663;
							if (projectile.velocity.Y > 0f)
							{
								projectile.velocity.Y = projectile.velocity.Y - num663 * 1.5f;
							}
						}
					}
					if (projectile.velocity.X != 0f)
					{
						projectile.spriteDirection = Math.Sign(projectile.velocity.X);
					}
				}
				if (projectile.ai[0] == 2f)
				{
					projectile.friendly = true;
					projectile.spriteDirection = projectile.direction;
					projectile.velocity.Y = projectile.velocity.Y + 0.4f;
					if (projectile.velocity.Y > 10f)
					{
						projectile.velocity.Y = 10f;
					}
					float[] var_2_1C896_cp_0 = projectile.ai;
					int var_2_1C896_cp_1 = 1;
					float num73 = var_2_1C896_cp_0[var_2_1C896_cp_1];
					var_2_1C896_cp_0[var_2_1C896_cp_1] = num73 - 1f;
					if (projectile.ai[1] <= 0f)
					{
						projectile.ai[1] = 0f;
						projectile.ai[0] = 0f;
						projectile.friendly = false;
						projectile.netUpdate = true;
						return;
					}
				}
				if (projectile.ai[0] == 0f)
				{
					float num671 = 200f;
					if (player.rocketDelay2 > 0)
					{
						projectile.ai[0] = 1f;
						projectile.netUpdate = true;
					}
					Vector2 vector59 = vector54 - projectile.Center;
					if (vector59.Length() > 2000f)
					{
						projectile.position = vector54 - new Vector2((float)projectile.width, (float)projectile.height) / 2f;
					}
					else if (vector59.Length() > num671 || Math.Abs(vector59.Y) > 300f)
					{
						projectile.ai[0] = 1f;
						projectile.netUpdate = true;
						if (projectile.velocity.Y > 0f && vector59.Y < 0f)
						{
							projectile.velocity.Y = 0f;
						}
						if (projectile.velocity.Y < 0f && vector59.Y > 0f)
						{
							projectile.velocity.Y = 0f;
						}
					}
					projectile.tileCollide = true;
					float num672 = 0.5f;
					float num673 = 4f;
					float num674 = 4f;
					float num675 = 0.1f;
					if (num674 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
					{
						num674 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
						num672 = 0.7f;
					}
					int num676 = 0;
					bool flag29 = false;
					float num677 = vector54.X - projectile.Center.X;
					if (Math.Abs(num677) > 5f)
					{
						if (num677 < 0f)
						{
							num676 = -1;
							if (projectile.velocity.X > -num673)
							{
								projectile.velocity.X = projectile.velocity.X - num672;
							}
							else
							{
								projectile.velocity.X = projectile.velocity.X - num675;
							}
						}
						else
						{
							num676 = 1;
							if (projectile.velocity.X < num673)
							{
								projectile.velocity.X = projectile.velocity.X + num672;
							}
							else
							{
								projectile.velocity.X = projectile.velocity.X + num675;
							}
						}
						flag29 = true;
					}
					else
					{
						projectile.velocity.X = projectile.velocity.X * 0.9f;
						if (Math.Abs(projectile.velocity.X) < num672 * 2f)
						{
							projectile.velocity.X = 0f;
						}
					}
					if (num676 != 0)
					{
						int num678 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
						int num679 = (int)projectile.position.Y / 16;
						num678 += num676;
						num678 += (int)projectile.velocity.X;
						int num3;
						for (int num680 = num679; num680 < num679 + projectile.height / 16 + 1; num680 = num3 + 1)
						{
							if (WorldGen.SolidTile(num678, num680))
							{
								flag29 = true;
							}
							num3 = num680;
						}
					}
					if (projectile.velocity.X != 0f)
					{
						flag29 = true;
					}
					Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
					if (projectile.velocity.Y == 0f && flag29)
					{
						int num3;
						for (int num681 = 0; num681 < 3; num681 = num3 + 1)
						{
							int num682 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
							if (num681 == 0)
							{
								num682 = (int)projectile.position.X / 16;
							}
							if (num681 == 2)
							{
								num682 = (int)(projectile.position.X + (float)projectile.width) / 16;
							}
							int num683 = (int)(projectile.position.Y + (float)projectile.height) / 16;
							if (WorldGen.SolidTile(num682, num683) || Main.tile[num682, num683].halfBrick() || Main.tile[num682, num683].slope() > 0 || (TileID.Sets.Platforms[(int)Main.tile[num682, num683].type] && Main.tile[num682, num683].active() && !Main.tile[num682, num683].inActive()))
							{
								try
								{
									num682 = (int)(projectile.position.X + (float)(projectile.width / 2)) / 16;
									num683 = (int)(projectile.position.Y + (float)(projectile.height / 2)) / 16;
									num682 += num676;
									num682 += (int)projectile.velocity.X;
									if (!WorldGen.SolidTile(num682, num683 - 1) && !WorldGen.SolidTile(num682, num683 - 2))
									{
										projectile.velocity.Y = -5.1f;
									}
									else if (!WorldGen.SolidTile(num682, num683 - 2))
									{
										projectile.velocity.Y = -7.1f;
									}
									else if (WorldGen.SolidTile(num682, num683 - 5))
									{
										projectile.velocity.Y = -11.1f;
									}
									else if (WorldGen.SolidTile(num682, num683 - 4))
									{
										projectile.velocity.Y = -10.1f;
									}
									else
									{
										projectile.velocity.Y = -9.1f;
									}
								}
								catch
								{
									projectile.velocity.Y = -9.1f;
								}
							}
							num3 = num681;
						}
					}
					if (projectile.velocity.X > num674)
					{
						projectile.velocity.X = num674;
					}
					if (projectile.velocity.X < -num674)
					{
						projectile.velocity.X = -num674;
					}
					if (projectile.velocity.X < 0f)
					{
						projectile.direction = -1;
					}
					if (projectile.velocity.X > 0f)
					{
						projectile.direction = 1;
					}
					if (projectile.velocity.X > num672 && num676 == 1)
					{
						projectile.direction = 1;
					}
					if (projectile.velocity.X < -num672 && num676 == -1)
					{
						projectile.direction = -1;
					}
					projectile.spriteDirection = projectile.direction;
					projectile.velocity.Y = projectile.velocity.Y + 0.4f;
					if (projectile.velocity.Y > 10f)
					{
						projectile.velocity.Y = 10f;
					}
				}
				return;
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
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D texture2D13 = (underwater ? Main.projectileTexture[projectile.type] : mod.GetTexture("Projectiles/Pets/SirenFlopping"));
			int num214 = (underwater ? Main.projectileTexture[projectile.type].Height : 256) / Main.projFrames[projectile.type];
			int y6 = num214 * projectile.frame;
			Main.spriteBatch.Draw(texture2D13, projectile.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y6, texture2D13.Width, num214)), projectile.GetAlpha(lightColor), projectile.rotation, new Vector2((float)texture2D13.Width / 2f, (float)num214 / 2f), projectile.scale, SpriteEffects.None, 0f);
			return false;
		}
	}
}