using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class StormjawBaby : ModProjectile
    {
        public float dust = 0f;
		private int sparkCounter = 0;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stormjaw Baby");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 10;
        }

        public override void SetDefaults()
        {
            projectile.width = 56;
            projectile.height = 38;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 1f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();
			CalamityGlobalProjectile modProj = projectile.Calamity();
            if (dust == 0f)
            {
                modProj.spawnedPlayerMinionDamageValue = player.MinionDamage();
                modProj.spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int num227 = 0; num227 < dustAmt; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int dusty = Dust.NewDust(vector6 + vector7, 0, 0, 132, vector7.X * 1.1f, vector7.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = vector7;
                }
                dust += 1f;
            }
            if (player.MinionDamage() != modProj.spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)modProj.spawnedPlayerMinionProjectileDamageValue /
                    modProj.spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }
            bool projTypeCheck = projectile.type == ModContent.ProjectileType<StormjawBaby>();
            player.AddBuff(ModContent.BuffType<StormjawBuff>(), 3600);
            if (projTypeCheck)
            {
                if (player.dead)
                {
                    modPlayer.stormjaw = false;
                }
                if (modPlayer.stormjaw)
                {
                    projectile.timeLeft = 2;
                }
            }
			Vector2 vector2_1 = player.Center;
			vector2_1.X = vector2_1.X - (float) ((15 + player.width / 2) * player.direction);
			vector2_1.X = vector2_1.X - (float) (projectile.minionPos * 40 * player.direction);
			int index1 = -1;
			float num4 = 800f;
			int num5 = 15;
			if (projectile.ai[0] == 0f) //find target
			{
				NPC minionAttackTargetNpc = projectile.OwnerMinionAttackTargetNPC;
				if (minionAttackTargetNpc != null && minionAttackTargetNpc.CanBeChasedBy((object) projectile, false))
				{
					float num1 = (minionAttackTargetNpc.Center - projectile.Center).Length();
					if ((double) num1 < (double) num4)
					{
						index1 = minionAttackTargetNpc.whoAmI;
						num4 = num1;
					}
				}
				if (index1 < 0)
				{
					for (int index2 = 0; index2 < Main.maxNPCs; ++index2)
					{
						NPC npc = Main.npc[index2];
						if (npc.CanBeChasedBy((object) projectile, false))
						{
							float num1 = (npc.Center - projectile.Center).Length();
							if ((double) num1 < (double) num4)
							{
								index1 = index2;
								num4 = num1;
							}
						}
					}
				}
			}
			if (projectile.ai[0] == 1f) //returning to player
			{
				projectile.tileCollide = false;
				Vector2 vector2 = new Vector2(projectile.position.X + (float) projectile.width * 0.5f, projectile.position.Y + (float) projectile.height * 0.5f);
				float xDist = player.position.X + (float) (player.width / 2) - vector2.X;
				xDist -= (float) (40 * player.direction);
				xDist -= (float) (40 * projectile.minionPos * player.direction);
				float yDist = player.position.Y + (float) (player.height / 2) - vector2.Y;
				yDist -= 60f;
				float playerDist2 = (float) Math.Sqrt(xDist * xDist + yDist * yDist);
				float num11 = 12f;
				float num12 = playerDist2;
				float conflict2 = 0.4f;
				if (num11 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
					num11 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);

				//if close enough to the player and has tile to stand on, return to normal
				if (playerDist2 < 100f && player.velocity.Y == 0f && (projectile.position.Y + (float) projectile.height <= player.position.Y + (float) player.height && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height)))
				{
					projectile.ai[0] = 0f;
					if (projectile.velocity.Y < -6f)
						projectile.velocity.Y = -6f;
				}
				if (playerDist2 > 2000f)
				{
					projectile.position.X = player.Center.X - (float)(projectile.width / 2);
					projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
					projectile.netUpdate = true;
				}
				if (playerDist2 < 50f)
				{
					if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
					{
						projectile.velocity *= 0.99f;
					}
					conflict2 = 0.01f;
				}
				else
				{
					if (playerDist2 < 100f)
					{
						conflict2 = 0.1f;
					}
					if (playerDist2 > 300f)
					{
						conflict2 = 1f;
					}
					playerDist2 = num12 / playerDist2;
					xDist *= playerDist2;
					yDist *= playerDist2;
				}
				if (projectile.velocity.X < xDist)
				{
					projectile.velocity.X = projectile.velocity.X + conflict2;
					if (conflict2 > 0.05f && projectile.velocity.X < 0f)
					{
						projectile.velocity.X = projectile.velocity.X + conflict2;
					}
				}
				if (projectile.velocity.X > xDist)
				{
					projectile.velocity.X = projectile.velocity.X - conflict2;
					if (conflict2 > 0.05f && projectile.velocity.X > 0f)
					{
						projectile.velocity.X = projectile.velocity.X - conflict2;
					}
				}
				if (projectile.velocity.Y < yDist)
				{
					projectile.velocity.Y = projectile.velocity.Y + conflict2;
					if (conflict2> 0.05f && projectile.velocity.Y < 0f)
					{
						projectile.velocity.Y = projectile.velocity.Y + conflict2 * 2f;
					}
				}
				if (projectile.velocity.Y > yDist)
				{
					projectile.velocity.Y = projectile.velocity.Y - conflict2;
					if (conflict2 > 0.05f && projectile.velocity.Y > 0f)
					{
						projectile.velocity.Y = projectile.velocity.Y - conflict2 * 2f;
					}
				}
				if (projectile.frame < 6 || projectile.frame > 9)
				{
					projectile.frame = 6;
				}
				else
				{
					projectile.frameCounter++;
					if (projectile.frameCounter > 3)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
					}
					if (projectile.frame >= 10)
					{
						projectile.frame = 6;
					}
				}
				if (projectile.velocity.X > 0.5f)
					projectile.spriteDirection = 1;
				else if (projectile.velocity.X < -0.5f)
					projectile.spriteDirection = -1;
				projectile.rotation = projectile.spriteDirection != 1 ? (float) Math.Atan2((double) projectile.velocity.Y, (double) projectile.velocity.X) + 3.14f : (float) Math.Atan2((double) projectile.velocity.Y, (double) projectile.velocity.X);
			}
			if (projectile.ai[0] == 2f) //attack target
			{
				projectile.spriteDirection = -projectile.direction;
				projectile.rotation = 0f;
				if (projectile.velocity.Y == 0f)
				{
					if (projectile.velocity.X == 0f)
					{
						projectile.frame = 0;
						projectile.frameCounter = 0;
					}
					else if ((double) Math.Abs(projectile.velocity.X) >= 0.5)
					{
						projectile.frameCounter += (int) Math.Abs(projectile.velocity.X);
						projectile.frameCounter += 1;
						if (projectile.frameCounter > 10)
						{
							projectile.frame += 1;
							projectile.frameCounter = 0;
						}
						if (projectile.frame >= 6)
							projectile.frame = 0;
					}
					else
					{
						projectile.frame = 0;
						projectile.frameCounter = 0;
					}
				}
				else if ((double) projectile.velocity.Y != 0.0)
				{
					projectile.frameCounter = 0;
					projectile.frame = 0;
				}
				projectile.velocity.Y += 0.4f;
				if ((double) projectile.velocity.Y > 10.0)
					projectile.velocity.Y = 10f;

				sparkCounter += Main.rand.Next(1,4);
				if (sparkCounter >= 20)
				{
					if (Main.myPlayer == projectile.owner)
					{
                        for (int i = 0; i < Main.rand.Next(1,4); i++)
                        {
							Vector2 sparkS = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
							int spark = Projectile.NewProjectile(projectile.Center, sparkS, ModContent.ProjectileType<Spark>(), projectile.damage, projectile.knockBack, projectile.owner);
							Main.projectile[spark].Calamity().forceMinion = true;
							Main.projectile[spark].timeLeft = 120;
							Main.projectile[spark].penetrate = 3;
							ProjectileID.Sets.MinionShot[Main.projectile[spark].type] = true;
							Main.projectile[spark].usesIDStaticNPCImmunity = true;
							Main.projectile[spark].idStaticNPCHitCooldown = 10;
							Main.projectile[spark].usesLocalNPCImmunity = false;
						}
						sparkCounter = 0;
					}
				}

				projectile.ai[1] -= 1f;
				if ((double) projectile.ai[1] <= 0.0)
				{
					projectile.ai[1] = 0f;
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
					return;
				}
			}
			if (index1 >= 0) //go to target
			{
				float num1 = 700f;
				float num2 = 20f;
				if ((double) projectile.position.Y > Main.worldSurface * 16.0)
					num1 *= 0.7f;
				NPC npc = Main.npc[index1];
				Vector2 center = npc.Center;
				float num3 = (center - projectile.Center).Length();
				Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
				if ((double) num3 < (double) num1)
				{
					vector2_1 = center;
					if ((double) center.Y < (double) projectile.Center.Y - 30.0 && (double) projectile.velocity.Y == 0.0)
					{
						float num6 = Math.Abs(center.Y - projectile.Center.Y);
						if ((double) num6 < 120.0)
							projectile.velocity.Y = -10f;
						else if ((double) num6 < 210.0)
							projectile.velocity.Y = -13f;
						else if ((double) num6 < 270.0)
							projectile.velocity.Y = -15f;
						else if ((double) num6 < 310.0)
							projectile.velocity.Y = -17f;
						else if ((double) num6 < 380.0)
							projectile.velocity.Y = -18f;
					}
				}
				if (num3 < num2)
				{
					projectile.ai[0] = 2f;
					projectile.ai[1] = (float) num5;
					projectile.netUpdate = true;
				}
			}
			if (projectile.ai[0] == 0f && index1 < 0) //passive AI
			{
				if (sparkCounter > 0)
					sparkCounter--;
				if (sparkCounter < 0)
					sparkCounter = 0;

				float num1 = 500f;
				Vector2 vector2_2 = player.Center - projectile.Center;
				if ((double) vector2_2.Length() > 2000.0)
					projectile.position = player.Center - new Vector2((float) projectile.width, (float) projectile.height) / 2f;
				else if ((double) vector2_2.Length() > (double) num1 || (double) Math.Abs(vector2_2.Y) > 300.0)
				{
					projectile.ai[0] = 1f;
					projectile.netUpdate = true;
					if (projectile.velocity.Y > 0f && vector2_2.Y < 0f)
						projectile.velocity.Y = 0f;
					if (projectile.velocity.Y < 0f && vector2_2.Y > 0f)
						projectile.velocity.Y = 0f;
				}
			}
			if (projectile.ai[0] == 0f)
			{
				projectile.tileCollide = true;
				float num1 = 0.5f;
				float num2 = 4f;
				float num3 = 4f;
				float num6 = 0.1f;
				if ((double) num3 < (double) Math.Abs(player.velocity.X) + (double) Math.Abs(player.velocity.Y))
				{
					num3 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					num1 = 0.7f;
				}
				int num7 = 0;
				bool flag3 = false;
				float num8 = vector2_1.X - projectile.Center.X;
				if ((double) Math.Abs(num8) > 5.0)
				{
					if ((double) num8 < 0.0)
					{
						num7 = -1;
						if ((double) projectile.velocity.X > -(double) num2)
						{
							projectile.velocity.X -= num1;
						}
						else
						{
							projectile.velocity.X -= num6;
						}
					}
					else
					{
						num7 = 1;
						if ((double) projectile.velocity.X < (double) num2)
						{
							projectile.velocity.X += num1;
						}
						else
						{
							projectile.velocity.X += num6;
						}
					}
				}
				else
				{
					projectile.velocity.X *= 0.9f;
					if ((double) Math.Abs(projectile.velocity.X) < (double) num1 * 2.0)
						projectile.velocity.X = 0.0f;
				}
				if (num7 != 0)
				{
					int num9 = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
					int num10 = (int) projectile.position.Y / 16;
					int i = num9 + num7 + (int) projectile.velocity.X;
					for (int j = num10; j < num10 + projectile.height / 16 + 1; ++j)
					{
						if (WorldGen.SolidTile(i, j))
							flag3 = true;
					}
				}
				Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
				if (projectile.velocity.Y == 0f && flag3)
				{
					for (int index2 = 0; index2 < 3; ++index2)
					{
						int i1 = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
						if (index2 == 0)
							i1 = (int) projectile.position.X / 16;
						if (index2 == 2)
							i1 = (int) ((double) projectile.position.X + (double) projectile.width) / 16;
						int j = (int) ((double) projectile.position.Y + (double) projectile.height) / 16;
						if (WorldGen.SolidTile(i1, j) || Main.tile[i1, j].halfBrick() || (int) Main.tile[i1, j].slope() > 0 || TileID.Sets.Platforms[(int) Main.tile[i1, j].type] && Main.tile[i1, j].active() && !Main.tile[i1, j].inActive())
						{
							try
							{
								int num9 = (int) ((double) projectile.position.X + (double) (projectile.width / 2)) / 16;
								int num10 = (int) ((double) projectile.position.Y + (double) (projectile.height / 2)) / 16;
								int i2 = num9 + num7 + (int) projectile.velocity.X;
								if (!WorldGen.SolidTile(i2, num10 - 1) && !WorldGen.SolidTile(i2, num10 - 2))
									projectile.velocity.Y = -5.1f;
								else if (!WorldGen.SolidTile(i2, num10 - 2))
									projectile.velocity.Y = -7.1f;
								else if (WorldGen.SolidTile(i2, num10 - 5))
									projectile.velocity.Y = -11.1f;
								else if (WorldGen.SolidTile(i2, num10 - 4))
									projectile.velocity.Y = -10.1f;
								else
									projectile.velocity.Y = -9.1f;
							}
							catch
							{
								projectile.velocity.Y = -9.1f;
							}
						}
					}
				}
				if ((double) projectile.velocity.X > (double) num3)
					projectile.velocity.X = num3;
				if ((double) projectile.velocity.X < -(double) num3)
					projectile.velocity.X = -num3;
				if ((double) projectile.velocity.X < 0.0)
					projectile.direction = -1;
				if ((double) projectile.velocity.X > 0.0)
					projectile.direction = 1;
				if ((double) projectile.velocity.X > (double) num1 && num7 == 1)
					projectile.direction = 1;
				if ((double) projectile.velocity.X < -(double) num1 && num7 == -1)
					projectile.direction = -1;
				projectile.spriteDirection = -projectile.direction;
				projectile.rotation = 0f;
				if (projectile.velocity.Y == 0f)
				{
					if (projectile.velocity.X == 0f)
					{
						projectile.frame = 0;
						projectile.frameCounter = 0;
						float xDist = player.Center.X - projectile.Center.X;
						xDist -= (float) (40 * player.direction);
						xDist -= (float) (40 * projectile.minionPos * player.direction);
						if (xDist > 0)
							projectile.spriteDirection = projectile.direction;
					}
					else if ((double) Math.Abs(projectile.velocity.X) >= 0.5)
					{
						projectile.frameCounter += (int) Math.Abs(projectile.velocity.X);
						projectile.frameCounter += 1;
						if (projectile.frameCounter > 10)
						{
							projectile.frame += 1;
							projectile.frameCounter = 0;
						}
						if (projectile.frame >= 6)
							projectile.frame = 0;
					}
					else
					{
						projectile.frame = 0;
						projectile.frameCounter = 0;
					}
				}
				else if ((double) projectile.velocity.Y != 0.0)
				{
					projectile.frameCounter = 0;
					projectile.frame = 0;
				}
				projectile.velocity.Y += 0.4f;
				if ((double) projectile.velocity.Y > 10.0)
					projectile.velocity.Y = 10f;
			}
		}

        public override bool CanDamage()
        {
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            return false;
        }

        public override void Kill(int timeLeft)
        {
			int index = Gore.NewGore(new Vector2(projectile.position.X - (float) (projectile.width / 2), projectile.position.Y - (float) (projectile.height / 2)), new Vector2(0.0f, 0.0f), Main.rand.Next(61, 64), projectile.scale);
			Main.gore[index].velocity *= 0.1f;
        }
    }
}
