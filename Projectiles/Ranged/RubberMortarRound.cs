using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Achievements;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Ranged
{
    public class RubberMortarRound : ModProjectile
    {
    	public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Round");
		}

        public override void SetDefaults()
        {
            projectile.width = 14;
            projectile.height = 14;
            projectile.friendly = true;
            projectile.penetrate = 3;
            projectile.timeLeft = 300;
            projectile.ranged = true;
        }

        public override void AI()
        {
        	projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X) + 1.57f;
			if (projectile.owner == Main.myPlayer && projectile.timeLeft <= 3)
			{
				projectile.tileCollide = false;
				projectile.ai[1] = 0f;
				projectile.alpha = 255;
				projectile.position.X = projectile.position.X + (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y + (float)(projectile.height / 2);
				projectile.width = 200;
				projectile.height = 200;
				projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
				projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
				projectile.knockBack = 10f;
			}
			else
			{
				if (Math.Abs(projectile.velocity.X) >= 8f || Math.Abs(projectile.velocity.Y) >= 8f)
				{
					for (int num246 = 0; num246 < 2; num246++)
					{
						float num247 = 0f;
						float num248 = 0f;
						if (num246 == 1)
						{
							num247 = projectile.velocity.X * 0.5f;
							num248 = projectile.velocity.Y * 0.5f;
						}
						int num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 6, 0f, 0f, 100, default(Color), 1f);
						Main.dust[num249].scale *= 2f + (float)Main.rand.Next(10) * 0.1f;
						Main.dust[num249].velocity *= 0.2f;
						Main.dust[num249].noGravity = true;
						num249 = Dust.NewDust(new Vector2(projectile.position.X + 3f + num247, projectile.position.Y + 3f + num248) - projectile.velocity * 0.5f, projectile.width - 8, projectile.height - 8, 31, 0f, 0f, 100, default(Color), 0.5f);
						Main.dust[num249].fadeIn = 1f + (float)Main.rand.Next(5) * 0.1f;
						Main.dust[num249].velocity *= 0.05f;
					}
				}
			}
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            Texture2D tex = Main.projectileTexture[projectile.type];
            spriteBatch.Draw(tex, projectile.Center - Main.screenPosition, null, projectile.GetAlpha(lightColor), projectile.rotation, tex.Size() / 2f, projectile.scale, SpriteEffects.None, 0f);
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			for (int num621 = 0; num621 < 40; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 70; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num624].velocity *= 2f;
			}
			for (int num625 = 0; num625 < 3; num625++)
			{
				float scaleFactor10 = 0.33f;
				if (num625 == 1)
				{
					scaleFactor10 = 0.66f;
				}
				if (num625 == 2)
				{
					scaleFactor10 = 1f;
				}
				int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13AB6_cp_0 = Main.gore[num626];
				expr_13AB6_cp_0.velocity.X = expr_13AB6_cp_0.velocity.X + 1f;
				Gore expr_13AD6_cp_0 = Main.gore[num626];
				expr_13AD6_cp_0.velocity.Y = expr_13AD6_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13B79_cp_0 = Main.gore[num626];
				expr_13B79_cp_0.velocity.X = expr_13B79_cp_0.velocity.X - 1f;
				Gore expr_13B99_cp_0 = Main.gore[num626];
				expr_13B99_cp_0.velocity.Y = expr_13B99_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13C3C_cp_0 = Main.gore[num626];
				expr_13C3C_cp_0.velocity.X = expr_13C3C_cp_0.velocity.X + 1f;
				Gore expr_13C5C_cp_0 = Main.gore[num626];
				expr_13C5C_cp_0.velocity.Y = expr_13C5C_cp_0.velocity.Y - 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13CFF_cp_0 = Main.gore[num626];
				expr_13CFF_cp_0.velocity.X = expr_13CFF_cp_0.velocity.X - 1f;
				Gore expr_13D1F_cp_0 = Main.gore[num626];
				expr_13D1F_cp_0.velocity.Y = expr_13D1F_cp_0.velocity.Y - 1f;
			}
			if (projectile.owner == Main.myPlayer)
			{
				int num814 = 5;
				int num815 = (int)(projectile.position.X / 16f - (float)num814);
				int num816 = (int)(projectile.position.X / 16f + (float)num814);
				int num817 = (int)(projectile.position.Y / 16f - (float)num814);
				int num818 = (int)(projectile.position.Y / 16f + (float)num814);
				if (num815 < 0)
				{
					num815 = 0;
				}
				if (num816 > Main.maxTilesX)
				{
					num816 = Main.maxTilesX;
				}
				if (num817 < 0)
				{
					num817 = 0;
				}
				if (num818 > Main.maxTilesY)
				{
					num818 = Main.maxTilesY;
				}
				bool flag3 = false;
				for (int num819 = num815; num819 <= num816; num819++)
				{
					for (int num820 = num817; num820 <= num818; num820++)
					{
						float num821 = Math.Abs((float)num819 - projectile.position.X / 16f);
						float num822 = Math.Abs((float)num820 - projectile.position.Y / 16f);
						double num823 = Math.Sqrt((double)(num821 * num821 + num822 * num822));
						if (num823 < (double)num814 && Main.tile[num819, num820] != null && Main.tile[num819, num820].wall == 0)
						{
							flag3 = true;
							break;
						}
					}
				}
				AchievementsHelper.CurrentlyMining = true;
				for (int num824 = num815; num824 <= num816; num824++)
				{
					for (int num825 = num817; num825 <= num818; num825++)
					{
						float num826 = Math.Abs((float)num824 - projectile.position.X / 16f);
						float num827 = Math.Abs((float)num825 - projectile.position.Y / 16f);
						double num828 = Math.Sqrt((double)(num826 * num826 + num827 * num827));
						if (num828 < (double)num814)
						{
							if (Main.tile[num824, num825] != null && Main.tile[num824, num825].active() && Main.tile[num824, num825].type != (ushort)mod.TileType("AbyssGravel"))
							{
								WorldGen.KillTile(num824, num825, false, false, false);
								if (!Main.tile[num824, num825].active() && Main.netMode != 0)
								{
									NetMessage.SendData(17, -1, -1, null, 0, (float)num824, (float)num825, 0f, 0, 0, 0);
								}
							}
							for (int num829 = num824 - 1; num829 <= num824 + 1; num829++)
							{
								for (int num830 = num825 - 1; num830 <= num825 + 1; num830++)
								{
									if (Main.tile[num829, num830] != null && Main.tile[num829, num830].wall > 0 && Main.tile[num829, num830].wall != (byte)mod.WallType("AbyssGravelWall") && flag3)
									{
										WorldGen.KillWall(num829, num830, false);
										if (Main.tile[num829, num830].wall == 0 && Main.netMode != 0)
										{
											NetMessage.SendData(17, -1, -1, null, 2, (float)num829, (float)num830, 0f, 0, 0, 0);
										}
									}
								}
							}
						}
					}
				}
				AchievementsHelper.CurrentlyMining = false;
				if (Main.netMode != 0)
				{
					NetMessage.SendData(29, -1, -1, null, projectile.identity, (float)projectile.owner, 0f, 0f, 0, 0, 0);
				}
				if (!projectile.noDropItem)
				{
					int num831 = -1;
					if (Main.netMode == 1 && num831 >= 0)
					{
						NetMessage.SendData(21, -1, -1, null, num831, 1f, 0f, 0f, 0, 0, 0);
					}
				}
			}
            projectile.penetrate--;
            if (projectile.penetrate <= 0)
            {
                projectile.Kill();
                projectile.active = false;
            }
            else
            {
                projectile.ai[0] += 0.1f;
                if (projectile.velocity.X != oldVelocity.X)
                {
                    projectile.velocity.X = -oldVelocity.X;
                }
                if (projectile.velocity.Y != oldVelocity.Y)
                {
                    projectile.velocity.Y = -oldVelocity.Y;
                }
                projectile.velocity *= 1.25f;
            }
            return false;
        }

        public override void Kill(int timeLeft)
        {
        	Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 14);
			for (int num621 = 0; num621 < 40; num621++)
			{
				int num622 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 31, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num622].velocity *= 3f;
				if (Main.rand.Next(2) == 0)
				{
					Main.dust[num622].scale = 0.5f;
					Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
				}
			}
			for (int num623 = 0; num623 < 70; num623++)
			{
				int num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 3f);
				Main.dust[num624].noGravity = true;
				Main.dust[num624].velocity *= 5f;
				num624 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 6, 0f, 0f, 100, default(Color), 2f);
				Main.dust[num624].velocity *= 2f;
			}
			for (int num625 = 0; num625 < 3; num625++)
			{
				float scaleFactor10 = 0.33f;
				if (num625 == 1)
				{
					scaleFactor10 = 0.66f;
				}
				if (num625 == 2)
				{
					scaleFactor10 = 1f;
				}
				int num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13AB6_cp_0 = Main.gore[num626];
				expr_13AB6_cp_0.velocity.X = expr_13AB6_cp_0.velocity.X + 1f;
				Gore expr_13AD6_cp_0 = Main.gore[num626];
				expr_13AD6_cp_0.velocity.Y = expr_13AD6_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13B79_cp_0 = Main.gore[num626];
				expr_13B79_cp_0.velocity.X = expr_13B79_cp_0.velocity.X - 1f;
				Gore expr_13B99_cp_0 = Main.gore[num626];
				expr_13B99_cp_0.velocity.Y = expr_13B99_cp_0.velocity.Y + 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13C3C_cp_0 = Main.gore[num626];
				expr_13C3C_cp_0.velocity.X = expr_13C3C_cp_0.velocity.X + 1f;
				Gore expr_13C5C_cp_0 = Main.gore[num626];
				expr_13C5C_cp_0.velocity.Y = expr_13C5C_cp_0.velocity.Y - 1f;
				num626 = Gore.NewGore(new Vector2(projectile.position.X + (float)(projectile.width / 2) - 24f, projectile.position.Y + (float)(projectile.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
				Main.gore[num626].velocity *= scaleFactor10;
				Gore expr_13CFF_cp_0 = Main.gore[num626];
				expr_13CFF_cp_0.velocity.X = expr_13CFF_cp_0.velocity.X - 1f;
				Gore expr_13D1F_cp_0 = Main.gore[num626];
				expr_13D1F_cp_0.velocity.Y = expr_13D1F_cp_0.velocity.Y - 1f;
			}
			if (projectile.owner == Main.myPlayer)
			{
				int num814 = 5;
				int num815 = (int)(projectile.position.X / 16f - (float)num814);
				int num816 = (int)(projectile.position.X / 16f + (float)num814);
				int num817 = (int)(projectile.position.Y / 16f - (float)num814);
				int num818 = (int)(projectile.position.Y / 16f + (float)num814);
				if (num815 < 0)
				{
					num815 = 0;
				}
				if (num816 > Main.maxTilesX)
				{
					num816 = Main.maxTilesX;
				}
				if (num817 < 0)
				{
					num817 = 0;
				}
				if (num818 > Main.maxTilesY)
				{
					num818 = Main.maxTilesY;
				}
				bool flag3 = false;
				for (int num819 = num815; num819 <= num816; num819++)
				{
					for (int num820 = num817; num820 <= num818; num820++)
					{
						float num821 = Math.Abs((float)num819 - projectile.position.X / 16f);
						float num822 = Math.Abs((float)num820 - projectile.position.Y / 16f);
						double num823 = Math.Sqrt((double)(num821 * num821 + num822 * num822));
						if (num823 < (double)num814 && Main.tile[num819, num820] != null && Main.tile[num819, num820].wall == 0)
						{
							flag3 = true;
							break;
						}
					}
				}
				AchievementsHelper.CurrentlyMining = true;
				for (int num824 = num815; num824 <= num816; num824++)
				{
					for (int num825 = num817; num825 <= num818; num825++)
					{
						float num826 = Math.Abs((float)num824 - projectile.position.X / 16f);
						float num827 = Math.Abs((float)num825 - projectile.position.Y / 16f);
						double num828 = Math.Sqrt((double)(num826 * num826 + num827 * num827));
						if (num828 < (double)num814)
						{
							if (Main.tile[num824, num825] != null && Main.tile[num824, num825].active())
							{
								WorldGen.KillTile(num824, num825, false, false, false);
								if (!Main.tile[num824, num825].active() && Main.netMode != 0)
								{
									NetMessage.SendData(17, -1, -1, null, 0, (float)num824, (float)num825, 0f, 0, 0, 0);
								}
							}
							for (int num829 = num824 - 1; num829 <= num824 + 1; num829++)
							{
								for (int num830 = num825 - 1; num830 <= num825 + 1; num830++)
								{
									if (Main.tile[num829, num830] != null && Main.tile[num829, num830].wall > 0 && flag3)
									{
										WorldGen.KillWall(num829, num830, false);
										if (Main.tile[num829, num830].wall == 0 && Main.netMode != 0)
										{
											NetMessage.SendData(17, -1, -1, null, 2, (float)num829, (float)num830, 0f, 0, 0, 0);
										}
									}
								}
							}
						}
					}
				}
				AchievementsHelper.CurrentlyMining = false;
				if (Main.netMode != 0)
				{
					NetMessage.SendData(29, -1, -1, null, projectile.identity, (float)projectile.owner, 0f, 0f, 0, 0, 0);
				}
				if (!projectile.noDropItem)
				{
					int num831 = -1;
					if (Main.netMode == 1 && num831 >= 0)
					{
						NetMessage.SendData(21, -1, -1, null, num831, 1f, 0f, 0f, 0, 0, 0);
					}
				}
			}
        }
    }
}
