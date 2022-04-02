using CalamityMod.Buffs.Summon;
using CalamityMod.Projectiles.Melee;
using Microsoft.Xna.Framework;
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
		private int targetIndex = -1;
		private Vector2 idlePos = Vector2.Zero;
        public Player player => Main.player[projectile.owner];

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
			OnSpawn();
			SummonChecks();

			idlePos = player.Center;
			idlePos.X -= (15f + player.width / 2f) * player.direction;
			idlePos.X -= projectile.minionPos * 40f * player.direction;

			FindTarget();
			FlyBackToPlayer();
			AttackTarget();
			GoToTarget();
			IdleBehavior();
		}

		private void OnSpawn()
		{
			// Handle stuff on spawn & variable damage
            if (dust == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int dustAmt = 36;
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int spark = Dust.NewDust(source + dustVel, 0, 0, 132, dustVel.X * 1.1f, dustVel.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[spark].noGravity = true;
                    Main.dust[spark].noLight = true;
                    Main.dust[spark].velocity = dustVel;
                }
                dust += 1f;
            }
		}

		private void SummonChecks()
		{
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue * player.MinionDamage());
                projectile.damage = damage2;
            }

            bool projTypeCheck = projectile.type == ModContent.ProjectileType<StormjawBaby>();
            player.AddBuff(ModContent.BuffType<StormjawBuff>(), 3600);
            if (projTypeCheck)
            {
                if (player.dead)
                {
                    player.Calamity().stormjaw = false;
                }
                if (player.Calamity().stormjaw)
                {
                    projectile.timeLeft = 2;
                }
            }
		}

		private void FindTarget()
		{
			targetIndex = -1;
			float maxDistance = 800f;
			if (projectile.ai[0] == 0f) //find target
			{
				NPC targetedNPC = projectile.OwnerMinionAttackTargetNPC;
				if (targetedNPC != null && targetedNPC.CanBeChasedBy(projectile, false))
				{
					float num1 = (targetedNPC.Center - projectile.Center).Length();
					if (num1 < maxDistance)
					{
						targetIndex = targetedNPC.whoAmI;
						maxDistance = num1;
					}
				}
				if (targetIndex < 0)
				{
					for (int i = 0; i < Main.maxNPCs; ++i)
					{
						NPC npc = Main.npc[i];
						if (npc.CanBeChasedBy(projectile, false))
						{
							float num1 = (npc.Center - projectile.Center).Length();
							if (num1 < maxDistance)
							{
								targetIndex = i;
								maxDistance = num1;
							}
						}
					}
				}
			}
		}

		private void FlyBackToPlayer()
		{
			if (projectile.ai[0] == 1f) //returning to player
			{
				projectile.tileCollide = false;
				Vector2 returnPos = player.Center - projectile.Center;
				returnPos.X -= (float) (40 * player.direction);
				returnPos.X -= (float) (40 * projectile.minionPos * player.direction);
				returnPos.Y -= 60f;
				float playerDist = returnPos.Length();
				float returnSpeed = 12f;
				float acceleration = 0.4f;
				if (returnSpeed < projectile.velocity.Length())
					returnSpeed = projectile.velocity.Length();

				//if close enough to the player and has tile to stand on, return to normal
				if (playerDist < 100f && player.velocity.Y == 0f && (projectile.Bottom.Y <= player.Bottom.Y && !Collision.SolidCollision(projectile.Center, projectile.width, projectile.height)))
				{
					projectile.ai[0] = 0f;
					if (projectile.velocity.Y < -6f)
						projectile.velocity.Y = -6f;
				}
				if (playerDist > 2000f)
				{
					projectile.position = player.Center - projectile.Size / 2f;
					projectile.netUpdate = true;
				}
				if (playerDist < 50f)
				{
					if (projectile.velocity.Length() > 2f)
					{
						projectile.velocity *= 0.99f;
					}
					acceleration = 0.01f;
				}
				else
				{
					if (playerDist < 100f)
					{
						acceleration = 0.1f;
					}
					if (playerDist > 300f)
					{
						acceleration = 1f;
					}
					playerDist = returnSpeed / playerDist;
					returnPos *= playerDist;
				}
				if (projectile.velocity.X < returnPos.X)
				{
					projectile.velocity.X += acceleration;
					if (acceleration > 0.05f && projectile.velocity.X < 0f)
					{
						projectile.velocity.X += acceleration;
					}
				}
				if (projectile.velocity.X > returnPos.X)
				{
					projectile.velocity.X -= acceleration;
					if (acceleration > 0.05f && projectile.velocity.X > 0f)
					{
						projectile.velocity.X -= acceleration;
					}
				}
				if (projectile.velocity.Y < returnPos.Y)
				{
					projectile.velocity.Y += acceleration;
					if (acceleration > 0.05f && projectile.velocity.Y < 0f)
					{
						projectile.velocity.Y += acceleration * 2f;
					}
				}
				if (projectile.velocity.Y > returnPos.Y)
				{
					projectile.velocity.Y -= acceleration;
					if (acceleration > 0.05f && projectile.velocity.Y > 0f)
					{
						projectile.velocity.Y -= acceleration * 2f;
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
				projectile.rotation = projectile.velocity.ToRotation() + projectile.spriteDirection != 1 ? MathHelper.Pi : 0f;
			}
		}

		private void AttackTarget()
		{
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
					else if (Math.Abs(projectile.velocity.X) >= 0.5f)
					{
						projectile.frameCounter += (int)Math.Abs(projectile.velocity.X);
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
				else if (projectile.velocity.Y != 0f)
				{
					projectile.frameCounter = 0;
					projectile.frame = 0;
				}
				projectile.velocity.Y += 0.4f;
				if (projectile.velocity.Y > 10f)
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
							if (spark.WithinBounds(Main.maxProjectiles))
							{
								Main.projectile[spark].Calamity().forceMinion = true;
								Main.projectile[spark].timeLeft = 120;
								Main.projectile[spark].penetrate = 3;
								Main.projectile[spark].usesIDStaticNPCImmunity = true;
								Main.projectile[spark].idStaticNPCHitCooldown = 10;
								Main.projectile[spark].usesLocalNPCImmunity = false;
							}
						}
						sparkCounter = 0;
					}
				}

				projectile.ai[1] -= 1f;
				if (projectile.ai[1] <= 0f)
				{
					projectile.ai[1] = 0f;
					projectile.ai[0] = 0f;
					projectile.netUpdate = true;
					return;
				}
			}
			else
			{
                Rectangle rectangle = new Rectangle((int)(projectile.position.X + projectile.velocity.X * 0.5f - 4f), (int)(projectile.position.Y + projectile.velocity.Y * 0.5f - 4f), projectile.width + 8, projectile.height + 8);
                for (int i = 0; i < Main.maxNPCs; i++)
                {
					NPC npc = Main.npc[i];
                    if (npc.CanBeChasedBy(projectile, false) && npc.immune[projectile.owner] <= 0)
                    {
                        Rectangle rect = npc.getRect();
                        if (rectangle.Intersects(rect) && (npc.noTileCollide || player.CanHit(npc)))
                        {
							sparkCounter += Main.rand.Next(1,3);
							if (sparkCounter >= 20)
							{
								if (Main.myPlayer == projectile.owner)
								{
									for (int j = 0; j < Main.rand.Next(1,4); j++)
									{
										Vector2 sparkS = new Vector2(Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f));
										int spark = Projectile.NewProjectile(projectile.Center, sparkS, ModContent.ProjectileType<Spark>(), projectile.damage, projectile.knockBack, projectile.owner);
										if (spark.WithinBounds(Main.maxProjectiles))
										{
											Main.projectile[spark].Calamity().forceMinion = true;
											Main.projectile[spark].timeLeft = 120;
											Main.projectile[spark].penetrate = 3;
											Main.projectile[spark].usesIDStaticNPCImmunity = true;
											Main.projectile[spark].idStaticNPCHitCooldown = 10;
											Main.projectile[spark].usesLocalNPCImmunity = false;
										}
									}
									sparkCounter = 0;
								}
							}
                        }
                    }
                }
			}
		}

		private void GoToTarget()
		{
			if (targetIndex >= 0) //go to target
			{
				float rangeofSight = 700f;
				float attackZone = 20f;
				if (projectile.position.Y > Main.worldSurface * 16f)
					rangeofSight *= 0.7f;
				NPC npc = Main.npc[targetIndex];
				float targetDist = (npc.Center - projectile.Center).Length();
				Collision.CanHit(projectile.Center, projectile.width, projectile.height, npc.Center, npc.width, npc.height);
				if (targetDist < rangeofSight)
				{
					idlePos = npc.Center;
					if (npc.Center.Y < projectile.Center.Y - 30f && projectile.velocity.Y == 0f)
					{
						float targetYDist = Math.Abs(npc.Center.Y - projectile.Center.Y);
						if (targetYDist < 120f)
							projectile.velocity.Y = -10f;
						else if (targetYDist < 210f)
							projectile.velocity.Y = -13f;
						else if (targetYDist < 270f)
							projectile.velocity.Y = -15f;
						else if (targetYDist < 310f)
							projectile.velocity.Y = -17f;
						else if (targetYDist < 380f)
							projectile.velocity.Y = -18f;
					}
				}
				if (targetDist < attackZone)
				{
					projectile.ai[0] = 2f;
					projectile.ai[1] = 15f;
					projectile.netUpdate = true;
				}
			}
		}

		private void IdleBehavior()
		{
			if (projectile.ai[0] == 0f && targetIndex < 0) //passive AI
			{
				if (sparkCounter > 0)
					sparkCounter--;
				if (sparkCounter < 0)
					sparkCounter = 0;

				float sepAnxietyDist = 500f;
				Vector2 playerDist = player.Center - projectile.Center;
				// Teleport to the player if too far
				if (playerDist.Length() > 2000f)
					projectile.position = player.Center - projectile.Size / 2f;
				// Fly back if too far from the player
				else if (playerDist.Length() > sepAnxietyDist || Math.Abs(playerDist.Y) > 300f)
				{
					projectile.ai[0] = 1f;
					projectile.netUpdate = true;
					if (projectile.velocity.Y > 0f && playerDist.Y < 0f)
						projectile.velocity.Y = 0f;
					if (projectile.velocity.Y < 0f && playerDist.Y > 0f)
						projectile.velocity.Y = 0f;
				}
			}

			if (projectile.ai[0] == 0f)
			{
				projectile.tileCollide = true;
				float accelFast = 0.5f;
				float maxSpeed = 4f;
				float xVel = 4f;
				float accelSlow = 0.1f;
				if (xVel < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
				{
					xVel = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					accelFast = 0.7f;
				}
				int direction = 0;
				bool flag3 = false;
				float idleDist = idlePos.X - projectile.Center.X;
				if (Math.Abs(idleDist) > 5f)
				{
					if (idleDist < 0f)
					{
						direction = -1;
						if (projectile.velocity.X > -maxSpeed)
						{
							projectile.velocity.X -= accelFast;
						}
						else
						{
							projectile.velocity.X -= accelSlow;
						}
					}
					else
					{
						direction = 1;
						if (projectile.velocity.X < maxSpeed)
						{
							projectile.velocity.X += accelFast;
						}
						else
						{
							projectile.velocity.X += accelSlow;
						}
					}
				}
				else
				{
					projectile.velocity.X *= 0.9f;
					if (Math.Abs(projectile.velocity.X) < accelFast * 2f)
						projectile.velocity.X = 0.0f;
				}
				if (direction != 0)
				{
					int xPos = (int)projectile.Center.X / 16;
					int yPos = (int)projectile.position.Y / 16;
					int x = xPos + direction + (int) projectile.velocity.X;
					for (int y = yPos; y < yPos + projectile.height / 16 + 1; ++y)
					{
						if (WorldGen.SolidTile(x, y))
							flag3 = true;
					}
				}
				Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
				if (projectile.velocity.Y == 0f && flag3)
				{
					for (int i = 0; i < 3; ++i)
					{
						int x = (int)projectile.Center.X / 16;
						if (i == 0)
							x = (int)projectile.Left.X / 16;
						if (i == 2)
							x = (int)projectile.Right.X / 16;
						int y = (int)projectile.Bottom.Y / 16;
						Tile tile = Main.tile[x, y];
						if (WorldGen.SolidTile(x, y) || tile.halfBrick() || tile.slope() > 0 || TileID.Sets.Platforms[tile.type] && tile.active() && !tile.inActive())
						{
							try
							{
								int xPos = (int)projectile.Center.X / 16;
								int yPos = (int)projectile.Center.Y / 16;
								int i2 = xPos + direction + (int)projectile.velocity.X;
								if (!WorldGen.SolidTile(i2, yPos - 1) && !WorldGen.SolidTile(i2, yPos - 2))
									projectile.velocity.Y = -5.1f;
								else if (!WorldGen.SolidTile(i2, yPos - 2))
									projectile.velocity.Y = -7.1f;
								else if (WorldGen.SolidTile(i2, yPos - 5))
									projectile.velocity.Y = -11.1f;
								else if (WorldGen.SolidTile(i2, yPos - 4))
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
				if (projectile.velocity.X > xVel)
					projectile.velocity.X = xVel;
				if (projectile.velocity.X < -xVel)
					projectile.velocity.X = -xVel;
				if (projectile.velocity.X < 0f)
					projectile.direction = -1;
				if (projectile.velocity.X > 0f)
					projectile.direction = 1;
				if (projectile.velocity.X > accelFast && direction == 1)
					projectile.direction = 1;
				if (projectile.velocity.X < -accelFast && direction == -1)
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
						xDist -= 40f * player.direction;
						xDist -= 40f * projectile.minionPos * player.direction;
						if (xDist > 0)
							projectile.spriteDirection = projectile.direction;
					}
					else if (Math.Abs(projectile.velocity.X) >= 0.5f)
					{
						projectile.frameCounter += (int)Math.Abs(projectile.velocity.X);
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
				else if (projectile.velocity.Y != 0f)
				{
					projectile.frameCounter = 0;
					projectile.frame = 0;
				}
				projectile.velocity.Y += 0.4f;
				if (projectile.velocity.Y > 10f)
					projectile.velocity.Y = 10f;
			}
		}

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
			if (targetIndex < 0)
				fallThrough = projectile.Bottom.Y < player.Top.Y;
			else
				fallThrough = projectile.Bottom.Y < Main.npc[targetIndex].Top.Y;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void Kill(int timeLeft)
        {
			int index = Gore.NewGore(projectile.Center, new Vector2(0f, 0f), Main.rand.Next(61, 64), projectile.scale);
			Main.gore[index].velocity *= 0.1f;
        }

        public override bool CanDamage() => false;
    }
}
