using CalamityMod.Buffs.Summon;
using CalamityMod.CalPlayer;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
    public class PlantSummon : ModProjectile
    {
		private bool enraged = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plantera");
            Main.projFrames[projectile.type] = 8;
            ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
        }

        public override void SetDefaults()
        {
            projectile.width = 38;
            projectile.height = 38;
            projectile.netImportant = true;
            projectile.friendly = true;
            projectile.ignoreWater = true;
            projectile.minionSlots = 2f;
            projectile.timeLeft = 18000;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft *= 5;
            projectile.minion = true;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = 12;
			projectile.extraUpdates = 0;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
			{
				if (Main.myPlayer == projectile.owner)
				{
					enraged = true;
				}
			}
			else
			{
				enraged = false;
				projectile.extraUpdates = 0;
			}

            projectile.frameCounter++;
            if (projectile.frameCounter > 8)
            {
                projectile.frame++;
                projectile.frameCounter = 0;
            }
			if (!enraged)
			{
				if (projectile.frame >= 4)
				{
					projectile.frame = 0;
				}
			}
			else
			{
				if (projectile.frame >= 8)
				{
					projectile.frame = 4;
				}
			}

            if (projectile.localAI[0] == 0f)
            {
                projectile.Calamity().spawnedPlayerMinionDamageValue = player.MinionDamage();
                projectile.Calamity().spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                int num226 = 36;
                for (int num227 = 0; num227 < num226; num227++)
                {
                    Vector2 vector6 = Vector2.Normalize(projectile.velocity) * new Vector2((float)projectile.width / 2f, (float)projectile.height) * 0.75f;
                    vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default) + projectile.Center;
                    Vector2 vector7 = vector6 - projectile.Center;
                    int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 107, vector7.X * 1.75f, vector7.Y * 1.75f, 100, default, 1.1f);
                    Main.dust[num228].noGravity = true;
                    Main.dust[num228].velocity = vector7;
                }
                projectile.localAI[0] += 1f;
            }
            if (player.MinionDamage() != projectile.Calamity().spawnedPlayerMinionDamageValue)
            {
                int damage2 = (int)((float)projectile.Calamity().spawnedPlayerMinionProjectileDamageValue /
                    projectile.Calamity().spawnedPlayerMinionDamageValue *
                    player.MinionDamage());
                projectile.damage = damage2;
            }

            bool flag64 = projectile.type == ModContent.ProjectileType<PlantSummon>();
            player.AddBuff(ModContent.BuffType<PlantationBuff>(), 3600);
            if (flag64)
            {
                if (player.dead)
                {
                    modPlayer.plantera = false;
                }
                if (modPlayer.plantera)
                {
                    projectile.timeLeft = 2;
                }
            }

            float num633 = 1000f;
            float num634 = 1300f;
            float num635 = 2600f;
            float num636 = 600f;
            float num637 = 0.05f;

			//idle movement
            for (int num638 = 0; num638 < Main.maxProjectiles; num638++)
            {
                bool flag23 = Main.projectile[num638].type == ModContent.ProjectileType<PlantSummon>();
                if (num638 != projectile.whoAmI && Main.projectile[num638].active && Main.projectile[num638].owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - Main.projectile[num638].position.X) + Math.Abs(projectile.position.Y - Main.projectile[num638].position.Y) < (float)projectile.width)
                {
                    if (projectile.position.X < Main.projectile[num638].position.X)
                    {
                        projectile.velocity.X = projectile.velocity.X - num637;
                    }
                    else
                    {
                        projectile.velocity.X = projectile.velocity.X + num637;
                    }
                    if (projectile.position.Y < Main.projectile[num638].position.Y)
                    {
                        projectile.velocity.Y = projectile.velocity.Y - num637;
                    }
                    else
                    {
                        projectile.velocity.Y = projectile.velocity.Y + num637;
                    }
                }
            }

			if (!enraged)
			{
				if (projectile.ai[0] >= 2f)
					projectile.ai[0] = 0f;
				Vector2 vector46 = projectile.position;
				bool flag25 = false;
				Vector2 value = new Vector2(0.5f);
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						Vector2 vector2 = npc.position + npc.Size * value;
						float num646 = Vector2.Distance(npc.Center, projectile.Center);
						if (!flag25 && num646 < num633)
						{
							num633 = num646;
							vector46 = vector2;
							flag25 = true;
						}
					}
				}
				else
				{
					for (int num645 = 0; num645 < Main.maxNPCs; num645++)
					{
						NPC npc = Main.npc[num645];
						if (npc.CanBeChasedBy(projectile, false))
						{
							Vector2 vector2 = npc.position + npc.Size * value;
							float num646 = Vector2.Distance(npc.Center, projectile.Center);
							if (!flag25 && num646 < num633)
							{
								num633 = num646;
								vector46 = vector2;
								flag25 = true;
							}
						}
					}
				}
				float num647 = num634;
				if (flag25)
				{
					num647 = num635;
				}
				if (Vector2.Distance(player.Center, projectile.Center) > num647)
				{
					projectile.ai[0] = 1f;
					projectile.netUpdate = true;
				}
				if (flag25 && projectile.ai[0] == 0f)
				{
					Vector2 vector47 = vector46 - projectile.Center;
					float num648 = vector47.Length();
					vector47.Normalize();
					if (num648 > 200f)
					{
						float scaleFactor2 = 18f; //12
						vector47 *= scaleFactor2;
						projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
					}
					else
					{
						float num649 = 9f;
						vector47 *= -num649;
						projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
					}
				}
				else
				{
					bool flag26 = false;
					if (!flag26)
					{
						flag26 = projectile.ai[0] == 1f;
					}
					float num650 = 12f;
					if (flag26)
					{
						num650 = 30f;
					}
					Vector2 center2 = projectile.Center;
					Vector2 vector48 = player.Center - center2 + new Vector2(0f, -120f);
					float num651 = vector48.Length();
					if (num651 > 200f && num650 < 16f)
					{
						num650 = 16f;
					}
					if (num651 < num636 && flag26)
					{
						projectile.ai[0] = 0f;
						projectile.netUpdate = true;
					}
					if (num651 > 2000f)
					{
						projectile.position.X = player.Center.X - (float)(projectile.width / 2);
						projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
						projectile.netUpdate = true;
					}
					if (num651 > 70f)
					{
						vector48.Normalize();
						vector48 *= num650;
						projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
					}
					else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
				}
				if (flag25)
				{
					projectile.rotation = projectile.rotation.AngleTowards(projectile.AngleTo(vector46) + MathHelper.Pi, 0.1f);
				}
				else
				{
					projectile.rotation = projectile.velocity.ToRotation() + MathHelper.Pi;
				}
				if (projectile.ai[1] > 0f)
				{
					projectile.ai[1] += (float)Main.rand.Next(1, 4);
				}
				if (projectile.ai[1] > 90f)
				{
					projectile.ai[1] = 0f;
					projectile.netUpdate = true;
				}
				if (projectile.ai[0] == 0f)
				{
					float scaleFactor3 = 6f;
					int projType = (Main.rand.NextBool(2) ? ModContent.ProjectileType<PlantSeedGreen>() : ModContent.ProjectileType<PlantSeed>());
					int projDmg = (int)(projectile.damage * 0.7f);
					float speedMult = 1f;
					if (Main.rand.NextBool(4))
					{
						projType = ModContent.ProjectileType<PlantThornBall>();
					}
					if (projType == ModContent.ProjectileType<PlantThornBall>())
					{
						speedMult = 2f;
						projDmg = (int)(projectile.damage * 1.2f);
					}
					if (projectile.ai[1] == 0f && flag25 && num633 < 500f)
					{
						Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
						projectile.ai[1] += 1f;
						if (Main.myPlayer == projectile.owner)
						{
							Vector2 value19 = vector46 - projectile.Center;
							value19.Normalize();
							value19 *= scaleFactor3;
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value19.X * speedMult, value19.Y * speedMult, projType, projDmg, 0f, projectile.owner, 0f, 0f);
							projectile.netUpdate = true;
						}
					}
				}
			}
			else //enraged
			{
				bool flag24 = false;
				if (projectile.ai[0] == 2f)
				{
					projectile.ai[1] += 1f;
					projectile.extraUpdates = 1;
					if (projectile.ai[1] > 30f)
					{
						projectile.ai[1] = 1f;
						projectile.ai[0] = 0f;
						projectile.extraUpdates = 0;
						projectile.numUpdates = 0;
						projectile.netUpdate = true;
					}
					else
					{
						flag24 = true;
					}
				}
				if (flag24)
				{
					return;
				}
				Vector2 vector46 = projectile.position;
				bool flag25 = false;
				Vector2 value = new Vector2(0.5f);
				if (player.HasMinionAttackTargetNPC)
				{
					NPC npc = Main.npc[player.MinionAttackTargetNPC];
					if (npc.CanBeChasedBy(projectile, false))
					{
						Vector2 vector2 = npc.position + npc.Size * value;
						float num646 = Vector2.Distance(npc.Center, projectile.Center);
						if ((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !flag25)
						{
							num633 = num646;
							vector46 = vector2;
							flag25 = true;
						}
					}
				}
				else
				{
					for (int num645 = 0; num645 < Main.maxNPCs; num645++)
					{
						NPC npc = Main.npc[num645];
						if (npc.CanBeChasedBy(projectile, false))
						{
							Vector2 vector2 = npc.position + npc.Size * value;
							float num646 = Vector2.Distance(npc.Center, projectile.Center);
							if ((Vector2.Distance(projectile.Center, vector46) > num646 && num646 < num633) || !flag25)
							{
								num633 = num646;
								vector46 = vector2;
								flag25 = true;
							}
						}
					}
				}
				if (flag25)
				{
					projectile.rotation = (vector46 - projectile.Center).ToRotation() + 3.14159274f;
				}
				else
				{
					projectile.rotation = projectile.velocity.ToRotation() + 3.14159274f;
				}
				float num647 = num634;
				if (flag25)
				{
					num647 = num635;
				}
				if (Vector2.Distance(player.Center, projectile.Center) > num647)
				{
					projectile.ai[0] = 1f;
					projectile.netUpdate = true;
				}
				if (flag25 && projectile.ai[0] == 0f)
				{
					Vector2 vector47 = vector46 - projectile.Center;
					float num648 = vector47.Length();
					vector47.Normalize();
					if (num648 > 200f)
					{
						float scaleFactor2 = 8f;
						vector47 *= scaleFactor2;
						projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
					}
					else
					{
						float num649 = 4f;
						vector47 *= -num649;
						projectile.velocity = (projectile.velocity * 40f + vector47) / 41f;
					}
				}
				else
				{
					bool flag26 = false;
					if (!flag26)
					{
						flag26 = projectile.ai[0] == 1f;
					}
					float num650 = 6f;
					if (flag26)
					{
						num650 = 15f;
					}
					Vector2 center2 = projectile.Center;
					Vector2 vector48 = player.Center - center2 + new Vector2(0f, -60f);
					float num651 = vector48.Length();
					if (num651 > 200f && num650 < 8f)
					{
						num650 = 8f;
					}
					if (num651 < num636 && flag26 && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					{
						projectile.ai[0] = 0f;
						projectile.netUpdate = true;
					}
					if (num651 > 2000f)
					{
						projectile.position.X = player.Center.X - (float)(projectile.width / 2);
						projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
						projectile.netUpdate = true;
					}
					if (num651 > 70f)
					{
						vector48.Normalize();
						vector48 *= num650;
						projectile.velocity = (projectile.velocity * 40f + vector48) / 41f;
					}
					else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
					{
						projectile.velocity.X = -0.15f;
						projectile.velocity.Y = -0.05f;
					}
				}
				if (projectile.ai[1] > 0f)
				{
					projectile.ai[1] += (float)Main.rand.Next(1, 4);
				}
				if (projectile.ai[1] > 40f)
				{
					projectile.ai[1] = 0f;
					projectile.netUpdate = true;
				}
				if (projectile.ai[0] == 0f)
				{
					if (projectile.ai[1] == 0f && flag25 && num633 < 500f)
					{
						projectile.ai[1] += 1f;
						if (Main.myPlayer == projectile.owner)
						{
							projectile.ai[0] = 2f;
							Vector2 value20 = vector46 - projectile.Center;
							value20.Normalize();
							if (Main.rand.NextBool(2))
							{
								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value20.X * 2f, value20.Y * 2f, ModContent.ProjectileType<PlantThornBall>(), (int)(projectile.damage * 1.5f), 0f, Main.myPlayer, 0f, 0f);
							}
							projectile.velocity = value20 * 8f;
							projectile.netUpdate = true;
						}
					}
				}
			}
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.Poisoned, 300);
        }

        public override bool CanDamage()
        {
            return enraged == true;
        }
    }
}
