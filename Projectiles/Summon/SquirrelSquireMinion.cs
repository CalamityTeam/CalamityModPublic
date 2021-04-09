using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class SquirrelSquireMinion : ModProjectile
    {
        public float dust = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Squirrel Squire");
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
            Main.projFrames[projectile.type] = 17;
        }

        public override void SetDefaults()
        {
            projectile.width = 64;
            projectile.height = 64;
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
                for (int d = 0; d < dustAmt; d++)
                {
                    Vector2 source = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    source = source.RotatedBy((double)((float)(d - (dustAmt / 2 - 1)) * MathHelper.TwoPi / (float)dustAmt), default) + projectile.Center;
                    Vector2 dustVel = source - projectile.Center;
                    int dusty = Dust.NewDust(source + dustVel, 0, 0, 7, dustVel.X * 1.1f, dustVel.Y * 1.1f, 100, default, 1.4f);
                    Main.dust[dusty].noGravity = true;
                    Main.dust[dusty].noLight = true;
                    Main.dust[dusty].velocity = dustVel;
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
            bool projTypeCheck = projectile.type == ModContent.ProjectileType<SquirrelSquireMinion>();
            player.AddBuff(ModContent.BuffType<SquirrelSquireBuff>(), 3600);
            if (projTypeCheck)
            {
                if (player.dead)
                {
                    modPlayer.squirrel = false;
                }
                if (modPlayer.squirrel)
                {
                    projectile.timeLeft = 2;
                }
            }
			bool leftofPlayer = false;
			bool rightofPlayer = false;
			bool flag3 = false;
			bool flag4 = false;
			if (projectile.lavaWet)
			{
				projectile.ai[0] = 1f;
				projectile.ai[1] = 0f;
			}
			float minionOffset = 40f * (projectile.minionPos + 1f) * player.direction;
			if (player.Center.X < projectile.Center.X - 10f + minionOffset)
				leftofPlayer = true;
			else if (player.Center.X > projectile.Center.X + 10f + minionOffset)
				rightofPlayer = true;

			if (projectile.ai[1] == 0f)
			{
				float playerDist = (player.Center - projectile.Center).Length();
				if (playerDist > 1000f)
				{
					projectile.ai[0] = 1f;
				}
				if (playerDist > 2000f) //teleport to player if too far
				{
					projectile.position = player.position;
					projectile.netUpdate = true;
				}
			}
			if (projectile.ai[0] != 0f) //flying back to the player
			{
				projectile.tileCollide = false;
				float npcDetectRange = 800f;
				bool npcFound = false;
				int targetIndex = -1;
				for (int index = 0; index < Main.maxNPCs; ++index)
				{
					NPC npc = Main.npc[index];
					if (npc.CanBeChasedBy(projectile, false))
					{
						float npcDist = Vector2.Distance(npc.Center, player.Center);
						if (npcDist < npcDetectRange)
						{
							if (Collision.CanHit(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height))
								targetIndex = index;
							npcFound = true;
							break;
						}
					}
				}

				//return to normal if npc found
				if (npcFound && targetIndex >= 0)
					projectile.ai[0] = 0f;

				Vector2 homeBase = player.Center - projectile.Center;
				homeBase.X -= 40f * player.direction;
				if (!npcFound)
					homeBase.X -= 40f * projectile.minionPos * player.direction;
				homeBase.Y -= 60f;
				float playerDist = homeBase.Length();
				float speed = playerDist;
				float acceleration = 0.4f;

				//if close enough to the player and has tile to stand on, return to normal
				if (playerDist < 100f && player.velocity.Y == 0f && projectile.Bottom.Y <= player.Bottom.Y && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[0] = 0f;
					if (projectile.velocity.Y < -6f)
						projectile.velocity.Y = -6f;
				}
				if (playerDist > 2000f)
				{
					projectile.position = player.position;
					projectile.netUpdate = true;
				}
				if (playerDist < 50f)
				{
					if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
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
					playerDist = speed / playerDist;
					homeBase *= playerDist;
				}
				if (projectile.velocity.X < homeBase.X)
				{
					projectile.velocity.X += acceleration;
					if (acceleration > 0.05f && projectile.velocity.X < 0f)
					{
						projectile.velocity.X += acceleration;
					}
				}
				if (projectile.velocity.X > homeBase.X)
				{
					projectile.velocity.X -= acceleration;
					if (acceleration > 0.05f && projectile.velocity.X > 0f)
					{
						projectile.velocity.X -= acceleration;
					}
				}
				if (projectile.velocity.Y < homeBase.Y)
				{
					projectile.velocity.Y += acceleration;
					if (acceleration > 0.05f && projectile.velocity.Y < 0f)
					{
						projectile.velocity.Y += acceleration * 2f;
					}
				}
				if (projectile.velocity.Y > homeBase.Y)
				{
					projectile.velocity.Y -= acceleration;
					if (acceleration > 0.05f && projectile.velocity.Y > 0f)
					{
						projectile.velocity.Y -= acceleration * 2f;
					}
				}
				if (projectile.frame < 12 || projectile.frame == 16)
				{
					projectile.frame = 12;
				}
				else
				{
					projectile.frameCounter++;
					if (projectile.frameCounter > 3)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
					}
					if (projectile.frame >= 16)
					{
						projectile.frame = 13;
					}
				}
				if (projectile.velocity.X > 0.5f)
					projectile.spriteDirection = 1;
				else if (projectile.velocity.X < -0.5f)
					projectile.spriteDirection = -1;
			}
			else
			{
				float minionOffset2 = (float)(40 * projectile.minionPos);
				float attackCooldown = 30f;
				float directionCooldown = 60f;
				--projectile.localAI[0];
				if (projectile.localAI[0] < 0f)
					projectile.localAI[0] = 0f;
				if (projectile.ai[1] > 0f)
				{
					--projectile.ai[1];
				}
				else
				{
					Vector2 targetPos = projectile.position;
					float range = 100000f;
					float maxDist = range;
					int targetIndex = -1;
					NPC target = projectile.OwnerMinionAttackTargetNPC;
					if (target != null && target.CanBeChasedBy(projectile, false))
					{
						float npcDist = Vector2.Distance(target.Center, projectile.Center);
						if (npcDist < range)
						{
							if (targetIndex == -1 && npcDist <= maxDist)
							{
								maxDist = npcDist;
								targetPos = target.Center;
							}
							if (Collision.CanHit(projectile.Center, projectile.width, projectile.height, target.Center, target.width, target.height))
							{
								range = npcDist;
								targetPos = target.Center;
								targetIndex = target.whoAmI;
							}
						}
					}
					if (targetIndex == -1)
					{
						for (int index = 0; index < Main.maxNPCs; ++index)
						{
							NPC npc = Main.npc[index];
							if (npc.CanBeChasedBy(projectile, false))
							{
								float npcDist = Vector2.Distance(npc.Center, projectile.Center);
								if (npcDist < range)
								{
									if (targetIndex == -1 && npcDist <= maxDist)
									{
										maxDist = npcDist;
										targetPos = npc.Center;
									}
									if (Collision.CanHit(projectile.Center, projectile.width, projectile.height, npc.Center, npc.width, npc.height))
									{
										range = npcDist;
										targetPos = npc.Center;
										targetIndex = index;
									}
								}
							}
						}
					}
					if (targetIndex == -1 && maxDist < range)
						range = maxDist;
					float num13 = 400f;
					if ((double)projectile.position.Y > Main.worldSurface * 16D)
						num13 = 200f;
					if (range < num13 + minionOffset2 && targetIndex == -1)
					{
						float xDist = targetPos.X - projectile.Center.X;
						if (xDist < -5f)
						{
							leftofPlayer = true;
							rightofPlayer = false;
						}
						else if (xDist > 5f)
						{
							rightofPlayer = true;
							leftofPlayer = false;
						}
					}
					else if (targetIndex >= 0 && range < 800f + minionOffset2)
					{
						projectile.localAI[0] = directionCooldown;
						float xDist = targetPos.X - projectile.Center.X;
						if (Math.Abs(xDist) > 300f)
						{
							if (xDist < -50f)
							{
								leftofPlayer = true;
								rightofPlayer = false;
							}
							else if (xDist > 50f)
							{
								rightofPlayer = true;
								leftofPlayer = false;
							}
						}
						else if (projectile.owner == Main.myPlayer)
						{
							projectile.ai[1] = attackCooldown;
							float speed = 12f;
							Vector2 source = projectile.Center - Vector2.UnitY * 8f;
							Vector2 projVel = targetPos - source;
							projVel.X += Main.rand.NextFloat(-10f, 10f);
							projVel.Y += Main.rand.NextFloat(-10f, 10f) - Math.Abs(projVel.X) * Main.rand.NextFloat(0.0001f, 0.01f);
							projVel.Normalize();
							projVel *= speed;
							int damage = projectile.damage;
							int projType = ModContent.ProjectileType<SquirrelSquireAcorn>();
							int index = Projectile.NewProjectile(source, projVel, projType, damage, projectile.knockBack, projectile.owner);
							if (projVel.X < 0f)
								projectile.direction = projectile.spriteDirection = -1;
							else if (projVel.X > 0f)
								projectile.direction = projectile.spriteDirection = 1;
							projectile.netUpdate = true;
						}
					}
				}
				if (projectile.ai[1] != 0f) //If on attack cooldown
				{
					leftofPlayer = false;
					rightofPlayer = false;
				}
				else if (projectile.localAI[0] == 0f)
					projectile.direction = projectile.spriteDirection = player.direction;
				projectile.tileCollide = true;
				float num18 = 0.2f;
				float num19 = 6f;
				if (num19 < Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y))
				{
					num19 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
					num18 = 0.3f;
				}
				if (leftofPlayer)
				{
					if (projectile.velocity.X > -3.5f)
						projectile.velocity.X -= num18;
					else
						projectile.velocity.X -= num18 * 0.25f;
				}
				else if (rightofPlayer)
				{
					if (projectile.velocity.X < 3.5f)
						projectile.velocity.X += num18;
					else
						projectile.velocity.X += num18 * 0.25f;
				}
				else
				{
					projectile.velocity.X *= 0.9f;
					if (projectile.velocity.X >= -num18 && projectile.velocity.X <= num18)
						projectile.velocity.X = 0f;
				}
				if (leftofPlayer | rightofPlayer)
				{
					int i = (int)projectile.Center.X / 16;
					int j = (int)projectile.Center.Y / 16;
					if (leftofPlayer)
						--i;
					if (rightofPlayer)
						++i;
					if (WorldGen.SolidTile(i + (int)projectile.velocity.X, j))
						flag4 = true;
				}
				if (player.position.Y + player.height - 8f > projectile.position.Y + projectile.height)
					flag3 = true;
				Collision.StepUp(ref projectile.position, ref projectile.velocity, projectile.width, projectile.height, ref projectile.stepSpeed, ref projectile.gfxOffY, 1, false, 0);
				if (projectile.velocity.Y == 0f)
				{
					if (!flag3 && (projectile.velocity.X < 0f || projectile.velocity.X > 0f))
					{
						int i = (int)projectile.Center.X / 16;
						int j = (int)projectile.Center.Y / 16 + 1;
						if (leftofPlayer)
							--i;
						if (rightofPlayer)
							++i;
						WorldGen.SolidTile(i, j);
					}
					if (flag4)
					{
						int i = (int)projectile.Center.X / 16;
						int j = (int)projectile.Center.Y / 16 + 1;
						if (WorldGen.SolidTile(i, j) || Main.tile[i, j].halfBrick() || (int)Main.tile[i, j].slope() > 0)
						{
							try
							{
								int i2 = (int)projectile.Center.X / 16;
								int j2 = (int)projectile.Center.Y / 16;
								if (leftofPlayer)
									--i2;
								if (rightofPlayer)
									++i2;
								i2 += (int)projectile.velocity.X;
								if (!WorldGen.SolidTile(i2, j2 - 1) && !WorldGen.SolidTile(i2, j2 - 2))
									projectile.velocity.Y = -5.1f;
								else if (!WorldGen.SolidTile(i2, j2 - 2))
									projectile.velocity.Y = -7.1f;
								else if (WorldGen.SolidTile(i2, j2 - 5))
									projectile.velocity.Y = -11.1f;
								else if (WorldGen.SolidTile(i2, j2 - 4))
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
				if (projectile.velocity.X > num19)
					projectile.velocity.X = num19;
				if (projectile.velocity.X < -num19)
					projectile.velocity.X = -num19;
				if (projectile.velocity.X < -0.05f)
					projectile.direction = projectile.spriteDirection = -1;
				if (projectile.velocity.X > 0.05f)
					projectile.direction = projectile.spriteDirection = 1;
				if (projectile.ai[1] > 0f)
				{
					if (projectile.localAI[1] == 0f)
					{
						projectile.localAI[1] = 1f;
						projectile.frame = 8;
					}
					if (projectile.frame >= 8 && projectile.frame <= 11)
					{
						projectile.frameCounter++;
						if (projectile.frameCounter > 8)
						{
							projectile.frame++;
							projectile.frameCounter = 0;
						}
						if (projectile.frame == 11)
							projectile.frame = 8;
					}
				}
				else if (projectile.velocity.Y == 0f)
				{
					projectile.localAI[1] = 0f;
					if (projectile.velocity.X == 0f)
					{
						projectile.frameCounter++;
						if (projectile.frameCounter > 4)
						{
							projectile.frame++;
							projectile.frameCounter = 0;
						}
						if (projectile.frame >= 4)
						{
							projectile.frame = 0;
						}
					}
					else if (Math.Abs(projectile.velocity.X) > 0.8f)
					{
						projectile.frameCounter += (int)Math.Abs(projectile.velocity.X);
						projectile.frameCounter++;
						if (projectile.frameCounter > 20)
						{
							projectile.frame++;
							projectile.frameCounter = 0;
						}
						if (projectile.frame < 4)
							projectile.frame = 4;
						if (projectile.frame >= 8)
							projectile.frame = 4;
					}
					else
					{
						projectile.frameCounter++;
						if (projectile.frameCounter > 4)
						{
							projectile.frame++;
							projectile.frameCounter = 0;
						}
						if (projectile.frame >= 4)
						{
							projectile.frame = 0;
						}
					}
				}
				else if (projectile.velocity.Y < 0f)
				{
					projectile.frameCounter = 0;
					projectile.frame = 16;
				}
				else if (projectile.velocity.Y > 0f)
				{
					projectile.frameCounter = 0;
					projectile.frame = 16;
				}
				projectile.velocity.Y += 0.4f;
				if (projectile.velocity.Y > 10f)
					projectile.velocity.Y = 10f;
				Vector2 velocity = projectile.velocity;
			}
		}

        public override bool CanDamage() => false;

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough)
        {
            fallThrough = false;
            return true;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) => false;

        public override void Kill(int timeLeft)
        {
			int index = Gore.NewGore(projectile.Center, Vector2.Zero, Main.rand.Next(61, 64), projectile.scale);
			Main.gore[index].velocity *= 0.1f;
        }
    }
}
