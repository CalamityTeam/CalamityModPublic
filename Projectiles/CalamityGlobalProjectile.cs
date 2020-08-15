using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Dusts;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Healing;
using CalamityMod.Projectiles.Hybrid;
using CalamityMod.Projectiles.Magic;
using CalamityMod.Projectiles.Melee;
using CalamityMod.Projectiles.Melee.Yoyos;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
using CalamityMod.Projectiles.Typeless.FiniteUse;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Projectiles
{
    public class CalamityGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity
        {
            get
            {
                return true;
            }
        }

		// Class Types
		public bool rogue = false;
        public bool trueMelee = false;

		// Force Class Types
		public bool forceMelee = false;
        public bool forceRanged = false;
        public bool forceMagic = false;
		public bool forceMinion = false;
		public bool forceRogue = false;
		public bool forceTypeless = false;
		public bool forceHostile = false;

        // Damage Adjusters
        private bool setDamageValues = true;
        public float spawnedPlayerMinionDamageValue = 1f;
        public int spawnedPlayerMinionProjectileDamageValue = 0;
        public int defDamage = 0;
		public int defCrit = 0;

		// Rogue Stuff
		public bool stealthStrike = false; //Update all existing rogue weapons with this
        public bool momentumCapacitatorBoost = false; //Constant acceleration

        // Iron Heart
        public int ironHeartDamage = 0;

        // Counters and Timers
        private int counter = 0;
		public int stealthStrikeHitCount = 0;

        public int lineColor = 0; //Note: Although this was intended for fishing line colors, I use this as an AI variable a lot because vanilla only has 4 that sometimes are already in use.  ~Ben
        public bool extorterBoost = false;

		// Organic/Inorganic Boosts
		public bool hasOrganicEnemyHitBoost = false;
		public bool hasInorganicEnemyHitBoost = false;
		public float organicEnemyHitBoost = 0f;
		public float inorganicEnemyHitBoost = 0f;
		public Action<NPC> organicEnemyHitEffect = null;
		public Action<NPC> inorganicEnemyHitEffect = null;

		public bool overridesMinionDamagePrevention = false;

        #region SetDefaults
		public override void SetDefaults(Projectile projectile)
		{
			if (CalamityMod.trueMeleeProjectileList.Contains(projectile.type))
				trueMelee = true;

			switch (projectile.type)
			{
				case ProjectileID.ShadowBeamHostile:
					projectile.timeLeft = 60;
					break;

				case ProjectileID.LostSoulHostile:
					projectile.tileCollide = true;
					break;

				case ProjectileID.NebulaLaser:
					projectile.extraUpdates = 1;
					break;

				case ProjectileID.StarWrath:
					projectile.penetrate = projectile.maxPenetrate = 1;
					break;

				case ProjectileID.Retanimini:
				case ProjectileID.MiniRetinaLaser:
					projectile.localNPCHitCooldown = 10;
					projectile.usesLocalNPCImmunity = true;
					projectile.usesIDStaticNPCImmunity = false;
					break;

				case ProjectileID.Spazmamini:
					projectile.usesIDStaticNPCImmunity = true;
					projectile.idStaticNPCHitCooldown = 12;
					break;
				default:
					break;
			}

			// Disable Lunatic Cultist's homing resistance globally
			ProjectileID.Sets.Homing[projectile.type] = false;
		}
        #endregion

        #region PreAI
        public override bool PreAI(Projectile projectile)
        {
			if (projectile.type == ProjectileID.Starfury)
			{
				if (projectile.timeLeft > 45)
					projectile.timeLeft = 45;

				if (projectile.ai[1] == 0f && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
				{
					projectile.ai[1] = 1f;
					projectile.netUpdate = true;
				}

				if (projectile.soundDelay == 0)
				{
					projectile.soundDelay = 20 + Main.rand.Next(40);
					Main.PlaySound(SoundID.Item9, projectile.position);
				}

				if (projectile.localAI[0] == 0f)
					projectile.localAI[0] = 1f;

				projectile.alpha += (int)(25f * projectile.localAI[0]);
				if (projectile.alpha > 200)
				{
					projectile.alpha = 200;
					projectile.localAI[0] = -1f;
				}
				if (projectile.alpha < 0)
				{
					projectile.alpha = 0;
					projectile.localAI[0] = 1f;
				}

				projectile.rotation += (Math.Abs(projectile.velocity.X) + Math.Abs(projectile.velocity.Y)) * 0.01f * projectile.direction;

				if (projectile.ai[1] == 1f)
				{
					projectile.light = 0.9f;

					if (Main.rand.NextBool(10))
						Dust.NewDust(projectile.position, projectile.width, projectile.height, 58, projectile.velocity.X * 0.5f, projectile.velocity.Y * 0.5f, 150, default, 1.2f);

					if (Main.rand.NextBool(20))
						Gore.NewGore(projectile.position, new Vector2(projectile.velocity.X * 0.2f, projectile.velocity.Y * 0.2f), Main.rand.Next(16, 18), 1f);
				}
				return false;
			}

            if (CalamityWorld.revenge || CalamityWorld.bossRushActive)
            {
				if (projectile.type == ProjectileID.EyeLaser && projectile.ai[0] == 1f)
				{
					projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

					Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.3f / 255f, 0f, (255 - projectile.alpha) * 0.3f / 255f);

					if (projectile.alpha > 0)
						projectile.alpha -= 125;
					if (projectile.alpha < 0)
						projectile.alpha = 0;

					if (projectile.localAI[1] == 0f)
					{
						Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
						projectile.localAI[1] = 1f;
					}

					if (projectile.velocity.Length() < 18f)
						projectile.velocity *= 1.0025f;

					return false;
				}

				else if (projectile.type == ProjectileID.DeathLaser && projectile.ai[0] == 1f)
				{
					projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + MathHelper.PiOver2;

					Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.75f / 255f, 0f, 0f);

					if (projectile.alpha > 0)
						projectile.alpha -= 125;
					if (projectile.alpha < 0)
						projectile.alpha = 0;

					if (projectile.localAI[1] == 0f)
					{
						Main.PlaySound(SoundID.Item33, (int)projectile.position.X, (int)projectile.position.Y);
						projectile.localAI[1] = 1f;
					}

					if (projectile.velocity.Length() < 18f)
						projectile.velocity *= 1.0025f;

					return false;
				}

                else if (projectile.type == ProjectileID.PoisonSeedPlantera)
                {
                    projectile.frameCounter++;
                    if (projectile.frameCounter > 1)
                    {
                        projectile.frameCounter = 0;
                        projectile.frame++;

                        if (projectile.frame > 1)
                            projectile.frame = 0;
                    }

                    if (projectile.ai[1] == 0f)
                    {
                        projectile.ai[1] = 1f;
                        Main.PlaySound(SoundID.Item17, projectile.position);
                    }

                    if (projectile.alpha > 0)
                        projectile.alpha -= 30;
                    if (projectile.alpha < 0)
                        projectile.alpha = 0;

                    projectile.ai[0] += 1f;
                    if (projectile.ai[0] >= 35f)
                    {
                        projectile.ai[0] = 35f;
                        projectile.velocity.Y += 0.01f;
                    }

                    projectile.tileCollide = false;

                    if (projectile.timeLeft > 300)
                        projectile.timeLeft = 300;

                    projectile.rotation = (float)Math.Atan2(projectile.velocity.Y, projectile.velocity.X) + 1.57f;

                    if (projectile.velocity.Y > 16f)
                        projectile.velocity.Y = 16f;

                    return false;
                }

                // Phase 1 sharknado
                else if (projectile.type == ProjectileID.SharknadoBolt)
                {
                    if (projectile.ai[1] < 0f)
                    {
                        float num623 = 0.209439516f;
                        float num624 = -2f;
                        float num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                        projectile.velocity.Y -= num625;

                        projectile.ai[0] += 1f;

                        num625 = (float)(Math.Cos(num623 * projectile.ai[0]) - 0.5) * num624;

                        projectile.velocity.Y += num625;

                        projectile.localAI[0] += 1f;
                        if (projectile.localAI[0] > 10f)
                        {
                            projectile.alpha -= 5;
                            if (projectile.alpha < 100)
                                projectile.alpha = 100;

                            projectile.rotation += projectile.velocity.X * 0.1f;
                            projectile.frame = (int)(projectile.localAI[0] / 3f) % 3;
                        }
                        return false;
                    }
                }

                // Large cthulhunadoes
                else if (projectile.type == ProjectileID.Cthulunado)
                {
                    int num606 = 16;
                    int num607 = 16;
                    float num608 = 2f;
                    int num609 = 150;
                    int num610 = 42;

                    if (projectile.velocity.X != 0f)
                        projectile.direction = projectile.spriteDirection = -Math.Sign(projectile.velocity.X);

                    int num3 = projectile.frameCounter;
                    projectile.frameCounter = num3 + 1;
                    if (projectile.frameCounter > 2)
                    {
                        num3 = projectile.frame;
                        projectile.frame = num3 + 1;
                        projectile.frameCounter = 0;
                    }
                    if (projectile.frame >= 6)
                        projectile.frame = 0;

                    if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
                    {
                        projectile.localAI[0] = 1f;
                        projectile.position.X += projectile.width / 2;
                        projectile.position.Y += projectile.height / 2;
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * num608 / (num607 + num606);
                        projectile.width = (int)(num609 * projectile.scale);
                        projectile.height = (int)(num610 * projectile.scale);
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                        projectile.netUpdate = true;
                    }

                    if (projectile.ai[1] != -1f)
                    {
                        projectile.scale = (num606 + num607 - projectile.ai[1]) * num608 / (num607 + num606);
                        projectile.width = (int)(num609 * projectile.scale);
                        projectile.height = (int)(num610 * projectile.scale);
                    }

                    if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                    {
                        projectile.alpha -= 30;
                        if (projectile.alpha < 60)
                            projectile.alpha = 60;

                        if (projectile.alpha < 100)
                            projectile.alpha = 100;
                    }
                    else
                    {
                        projectile.alpha += 30;
                        if (projectile.alpha > 150)
                            projectile.alpha = 150;
                    }

                    if (projectile.ai[0] > 0f)
                        projectile.ai[0] -= 1f;

                    if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
                    {
                        projectile.netUpdate = true;

                        Vector2 center = projectile.Center;
                        center.Y -= num610 * projectile.scale / 2f;

                        float num611 = (num606 + num607 - projectile.ai[1] + 1f) * num608 / (num607 + num606);
                        center.Y -= num610 * num611 / 2f;
                        center.Y += 2f;

                        Projectile.NewProjectile(center.X, center.Y, projectile.velocity.X, projectile.velocity.Y, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);

                        if ((int)projectile.ai[1] % 3 == 0 && projectile.ai[1] != 0f)
                        {
                            int num614 = NPC.NewNPC((int)center.X, (int)center.Y, NPCID.Sharkron2, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num614].velocity = projectile.velocity;
                            Main.npc[num614].scale = 1.5f;
                            Main.npc[num614].netUpdate = true;
                            Main.npc[num614].ai[2] = projectile.width;
                            Main.npc[num614].ai[3] = -1.5f;
                        }
                    }

                    if (projectile.ai[0] <= 0f)
                    {
                        float num615 = 0.104719758f;
                        float num616 = projectile.width / 5f * 2.5f;
                        float num617 = (float)(Math.Cos(num615 * -(double)projectile.ai[0]) - 0.5) * num616;

                        projectile.position.X -= num617 * -projectile.direction;

                        projectile.ai[0] -= 1f;

                        num617 = (float)(Math.Cos(num615 * -(double)projectile.ai[0]) - 0.5) * num616;
                        projectile.position.X += num617 * -projectile.direction;
                    }
                    return false;
                }

				// Change the stupid homing eyes
				else if (projectile.type == ProjectileID.PhantasmalEye)
				{
					projectile.alpha -= 40;
					if (projectile.alpha < 0)
					{
						projectile.alpha = 0;
					}
					if (projectile.ai[0] == 0f)
					{
						projectile.localAI[0] += 1f;
						if (projectile.localAI[0] >= 45f)
						{
							projectile.localAI[0] = 0f;
							projectile.ai[0] = 1f;
							projectile.ai[1] = 0f - projectile.ai[1];
							projectile.netUpdate = true;
						}
						projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
						projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
						projectile.velocity.Y -= 0.08f;
						if (projectile.velocity.Y > 0f)
						{
							projectile.velocity.Y -= 0.2f;
						}
						if (projectile.velocity.Y < -7f)
						{
							projectile.velocity.Y = -7f;
						}
					}
					else if (projectile.ai[0] == 1f)
					{
						projectile.localAI[0] += 1f;
						if (projectile.localAI[0] >= 90f)
						{
							projectile.localAI[0] = 0f;
							projectile.ai[0] = 2f;
							projectile.ai[1] = Player.FindClosest(projectile.position, projectile.width, projectile.height);
							projectile.netUpdate = true;
						}
						projectile.velocity.X = projectile.velocity.RotatedBy(projectile.ai[1]).X;
						projectile.velocity.X = MathHelper.Clamp(projectile.velocity.X, -6f, 6f);
						projectile.velocity.Y -= 0.08f;
						if (projectile.velocity.Y > 0f)
						{
							projectile.velocity.Y -= 0.2f;
						}
						if (projectile.velocity.Y < -7f)
						{
							projectile.velocity.Y = -7f;
						}
					}
					else if (projectile.ai[0] == 2f)
					{
						projectile.localAI[0] += 1f;
						if (projectile.localAI[0] >= 60f)
						{
							projectile.localAI[0] = 0f;
							projectile.ai[0] = 3f;
							projectile.netUpdate = true;
						}
						Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
						value23.Normalize();
						value23 *= 12f;
						value23 = Vector2.Lerp(projectile.velocity, value23, 0.6f);
						float num675 = 0.4f;
						if (projectile.velocity.X < value23.X)
						{
							projectile.velocity.X += num675;
							if (projectile.velocity.X < 0f && value23.X > 0f)
							{
								projectile.velocity.X += num675;
							}
						}
						else if (projectile.velocity.X > value23.X)
						{
							projectile.velocity.X -= num675;
							if (projectile.velocity.X > 0f && value23.X < 0f)
							{
								projectile.velocity.X -= num675;
							}
						}
						if (projectile.velocity.Y < value23.Y)
						{
							projectile.velocity.Y += num675;
							if (projectile.velocity.Y < 0f && value23.Y > 0f)
							{
								projectile.velocity.Y += num675;
							}
						}
						else if (projectile.velocity.Y > value23.Y)
						{
							projectile.velocity.Y -= num675;
							if (projectile.velocity.Y > 0f && value23.Y < 0f)
							{
								projectile.velocity.Y -= num675;
							}
						}
					}
					else if (projectile.ai[0] == 3f)
					{
						Vector2 value23 = Main.player[(int)projectile.ai[1]].Center - projectile.Center;
						if (value23.Length() < 30f)
						{
							projectile.Kill();
							return false;
						}
						if (projectile.velocity.Length() < 18f)
							projectile.velocity *= 1.01f;
					}
					if (projectile.alpha < 40)
					{
						int num676 = Dust.NewDust(projectile.Center - Vector2.One * 5f, 10, 10, 229, (0f - projectile.velocity.X) / 3f, (0f - projectile.velocity.Y) / 3f, 150, Color.Transparent, 1.2f);
						Main.dust[num676].noGravity = true;
					}
					projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver2;
					return false;
				}

                // Moon Lord Deathray
                else if (projectile.type == ProjectileID.PhantasmalDeathray)
                {
					if (Main.npc[(int)projectile.ai[1]].type == NPCID.MoonLordHead)
					{
						Vector2? vector78 = null;

						if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
						{
							projectile.velocity = -Vector2.UnitY;
						}

						if (Main.npc[(int)projectile.ai[1]].active)
						{
							Vector2 value21 = new Vector2(27f, 59f);
							Vector2 value22 = Utils.Vector2FromElipse(Main.npc[(int)projectile.ai[1]].localAI[0].ToRotationVector2(), value21 * Main.npc[(int)projectile.ai[1]].localAI[1]);
							projectile.position = Main.npc[(int)projectile.ai[1]].Center + value22 - new Vector2(projectile.width, projectile.height) / 2f;
						}

						if (projectile.velocity.HasNaNs() || projectile.velocity == Vector2.Zero)
						{
							projectile.velocity = -Vector2.UnitY;
						}

						if (projectile.localAI[0] == 0f)
						{
							Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
						}

						float num801 = 1f;
						projectile.localAI[0] += 1f;
						if (projectile.localAI[0] >= 180f)
						{
							projectile.Kill();
							return false;
						}

						projectile.scale = (float)Math.Sin(projectile.localAI[0] * 3.14159274f / 180f) * 10f * num801;
						if (projectile.scale > num801)
						{
							projectile.scale = num801;
						}

						float num804 = projectile.velocity.ToRotation();
						num804 += projectile.ai[0];
						projectile.rotation = num804 - 1.57079637f;
						projectile.velocity = num804.ToRotationVector2();

						float num805 = 3f;
						float num806 = projectile.width;

						Vector2 samplingPoint = projectile.Center;
						if (vector78.HasValue)
						{
							samplingPoint = vector78.Value;
						}

						float[] array3 = new float[(int)num805];
						Collision.LaserScan(samplingPoint, projectile.velocity, num806 * projectile.scale, 2400f, array3);
						float num807 = 0f;
						int num3;
						for (int num808 = 0; num808 < array3.Length; num808 = num3 + 1)
						{
							num807 += array3[num808];
							num3 = num808;
						}
						num807 /= num805;

						// Fire laser through walls at max length if target cannot be seen
						if (!Collision.CanHitLine(Main.npc[(int)projectile.ai[1]].Center, 1, 1, Main.player[Main.npc[(int)projectile.ai[1]].target].Center, 1, 1) &&
							Main.npc[(int)projectile.ai[1]].Calamity().newAI[0] == 1f)
						{
							num807 = 2400f;
						}

						float amount = 0.5f;
						projectile.localAI[1] = MathHelper.Lerp(projectile.localAI[1], num807, amount);
						Vector2 vector79 = projectile.Center + projectile.velocity * (projectile.localAI[1] - 14f);
						for (int num809 = 0; num809 < 2; num809 = num3 + 1)
						{
							float num810 = projectile.velocity.ToRotation() + ((Main.rand.Next(2) == 1) ? -1f : 1f) * 1.57079637f;
							float num811 = (float)Main.rand.NextDouble() * 2f + 2f;
							Vector2 vector80 = new Vector2((float)Math.Cos(num810) * num811, (float)Math.Sin(num810) * num811);
							int num812 = Dust.NewDust(vector79, 0, 0, 229, vector80.X, vector80.Y, 0, default, 1f);
							Main.dust[num812].noGravity = true;
							Main.dust[num812].scale = 1.7f;
							num3 = num809;
						}
						if (Main.rand.Next(5) == 0)
						{
							Vector2 value29 = projectile.velocity.RotatedBy(1.5707963705062866, default) * ((float)Main.rand.NextDouble() - 0.5f) * projectile.width;
							int num813 = Dust.NewDust(vector79 + value29 - Vector2.One * 4f, 8, 8, 31, 0f, 0f, 100, default, 1.5f);
							Dust dust = Main.dust[num813];
							dust.velocity *= 0.5f;
							Main.dust[num813].velocity.Y = -Math.Abs(Main.dust[num813].velocity.Y);
						}
						DelegateMethods.v3_1 = new Vector3(0.3f, 0.65f, 0.7f);
						Utils.PlotTileLine(projectile.Center, projectile.Center + projectile.velocity * projectile.localAI[1], projectile.width * projectile.scale, new Utils.PerLinePoint(DelegateMethods.CastLight));

						return false;
					}
                }
            }

			if (CalamityWorld.death && !CalamityPlayer.areThereAnyDamnBosses)
			{
				if (projectile.type == ProjectileID.Sharknado || projectile.type == ProjectileID.Cthulunado)
				{
					int num520 = 10;
					int num521 = 15;
					float num522 = 1f;
					int num523 = 150;
					int num524 = 42;
					if (projectile.type == ProjectileID.Cthulunado)
					{
						num520 = 16;
						num521 = 16;
						num522 = 1.5f;
					}
					if (projectile.velocity.X != 0f)
					{
						projectile.direction = (projectile.spriteDirection = -Math.Sign(projectile.velocity.X));
					}
					projectile.frameCounter++;
					if (projectile.frameCounter > 2)
					{
						projectile.frame++;
						projectile.frameCounter = 0;
					}
					if (projectile.frame >= 6)
					{
						projectile.frame = 0;
					}
					if (projectile.localAI[0] == 0f && Main.myPlayer == projectile.owner)
					{
						projectile.localAI[0] = 1f;
						projectile.position.X += projectile.width / 2;
						projectile.position.Y += projectile.height / 2;
						projectile.scale = ((float)(num520 + num521) - projectile.ai[1]) * num522 / (float)(num521 + num520);
						projectile.width = (int)((float)num523 * projectile.scale);
						projectile.height = (int)((float)num524 * projectile.scale);
						projectile.position.X -= projectile.width / 2;
						projectile.position.Y -= projectile.height / 2;
						projectile.netUpdate = true;
					}
					if (projectile.ai[1] != -1f)
					{
						projectile.scale = ((float)(num520 + num521) - projectile.ai[1]) * num522 / (float)(num521 + num520);
						projectile.width = (int)((float)num523 * projectile.scale);
						projectile.height = (int)((float)num524 * projectile.scale);
					}
					if (!Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
					{
						projectile.alpha -= 30;
						if (projectile.alpha < 60)
						{
							projectile.alpha = 60;
						}
						if (projectile.type == ProjectileID.Cthulunado && projectile.alpha < 100)
						{
							projectile.alpha = 100;
						}
					}
					else
					{
						projectile.alpha += 30;
						if (projectile.alpha > 150)
						{
							projectile.alpha = 150;
						}
					}
					if (projectile.ai[0] > 0f)
					{
						projectile.ai[0] -= 1f;
					}
					if (projectile.ai[0] == 1f && projectile.ai[1] > 0f && projectile.owner == Main.myPlayer)
					{
						projectile.netUpdate = true;
						Vector2 center2 = projectile.Center;
						center2.Y -= (float)num524 * projectile.scale / 2f;
						float num525 = ((float)(num520 + num521) - projectile.ai[1] + 1f) * num522 / (float)(num521 + num520);
						center2.Y -= (float)num524 * num525 / 2f;
						center2.Y += 2f;
						Projectile.NewProjectile(center2, projectile.velocity, projectile.type, projectile.damage, projectile.knockBack, projectile.owner, 10f, projectile.ai[1] - 1f);
					}
					if (projectile.ai[0] <= 0f)
					{
						float num529 = (float)Math.PI / 30f;
						float num530 = (float)projectile.width / 5f;
						if (projectile.type == ProjectileID.Cthulunado)
						{
							num530 *= 2f;
						}
						float num531 = (float)(Math.Cos(num529 * (-projectile.ai[0])) - 0.5) * num530;
						projectile.position.X -= num531 * (float)(-projectile.direction);
						projectile.ai[0] -= 1f;
						num531 = (float)(Math.Cos(num529 * (-projectile.ai[0])) - 0.5) * num530;
						projectile.position.X += num531 * (float)(-projectile.direction);
					}

					return false;
				}
			}

			return true;
        }
        #endregion

        #region AI
        public override void AI(Projectile projectile)
        {
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

            if (defDamage == 0)
                defDamage = projectile.damage;

            if (NPC.downedMoonlord)
            {
                if (CalamityMod.dungeonProjectileBuffList.Contains(projectile.type))
                {
					//Prevents them being buffed in Skeletron, Skeletron Prime, or Golem fights
                    if (((projectile.type == ProjectileID.RocketSkeleton || projectile.type == ProjectileID.Shadowflames) && projectile.ai[1] == 1f) ||
                        (NPC.golemBoss > 0 && (projectile.type == ProjectileID.InfernoHostileBolt || projectile.type == ProjectileID.InfernoHostileBlast)))
                    {
                        projectile.damage = defDamage;
                    }
                    else
                        projectile.damage = defDamage + 60;
                }
            }

            if (CalamityWorld.downedDoG && (Main.pumpkinMoon || Main.snowMoon))
            {
                if (CalamityMod.eventProjectileBuffList.Contains(projectile.type))
                    projectile.damage = defDamage + 70;
            }
            else if (CalamityWorld.buffedEclipse && Main.eclipse)
            {
                if (CalamityMod.eventProjectileBuffList.Contains(projectile.type))
                    projectile.damage = defDamage + 100;
            }

            // Iron Heart damage variable will scale with projectile.damage
            if (CalamityWorld.ironHeart)
            {
                ironHeartDamage = 0;
            }

            if (projectile.modProjectile != null && projectile.modProjectile.mod.Name.Equals("CalamityMod"))
                goto SKIP_CALAMITY;

            if ((projectile.minion || projectile.sentry) && !ProjectileID.Sets.StardustDragon[projectile.type]) //For all other mods and vanilla, exclude dragon due to bugs
            {
                if (setDamageValues)
                {
                    spawnedPlayerMinionDamageValue = player.MinionDamage();
                    spawnedPlayerMinionProjectileDamageValue = projectile.damage;
                    setDamageValues = false;
                }
                if (player.MinionDamage() != spawnedPlayerMinionDamageValue)
                {
                    int damage2 = (int)(spawnedPlayerMinionProjectileDamageValue / spawnedPlayerMinionDamageValue * player.MinionDamage());
                    projectile.damage = damage2;
                }
            }
			SKIP_CALAMITY:

			// If rogue projectiles are not internally throwing while in-flight, they can never critically strike.
			if (rogue)
				projectile.thrown = true;

			if (forceMelee)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = true;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
				projectile.thrown = false;
				rogue = false;
            }
            else if (forceRanged)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = true;
                projectile.magic = false;
                projectile.minion = false;
				projectile.thrown = false;
				rogue = false;
            }
            else if (forceMagic)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = true;
                projectile.minion = false;
				projectile.thrown = false;
				rogue = false;
            }
            else if (forceMinion)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = true;
				projectile.thrown = false;
				rogue = false;
            }
			else if (forceRogue)
			{
				projectile.hostile = false;
				projectile.friendly = true;
				projectile.melee = false;
				projectile.ranged = false;
				projectile.magic = false;
				projectile.minion = false;
				projectile.thrown = true;
				rogue = true;
			}
			else if (forceTypeless)
			{
				projectile.hostile = false;
				projectile.friendly = true;
				projectile.melee = false;
				projectile.ranged = false;
				projectile.magic = false;
				projectile.minion = false;
				projectile.thrown = false;
				rogue = false;
			}
			else if (forceHostile)
            {
                projectile.hostile = true;
                projectile.friendly = false;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
				projectile.thrown = false;
                rogue = false;
            }

            if (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee)
            {
                if (projectile.timeLeft > 570) //all of these have a time left of 600 or 660
                {
                    if (player.ActiveItem().type == ItemID.BeesKnees)
                    {
                        projectile.magic = false;
                        projectile.ranged = true;
                    }
                }
            }
            else if (projectile.type == ProjectileID.SoulDrain)
                projectile.magic = true;

			if (modPlayer.etherealExtorter)
			{
				if (CalamityMod.spikyBallProjList.Contains(projectile.type) && !extorterBoost && Main.moonPhase == 2) //third quarter
				{
					projectile.timeLeft += 300;
					extorterBoost = true;
				}
				if (CalamityMod.javelinProjList.Contains(projectile.type) && !extorterBoost && player.ZoneCrimson)
				{
					projectile.knockBack *= 2;
					extorterBoost = true;
				}
			}

			if (projectile.type == ProjectileID.OrnamentFriendly && lineColor == 1) //spawned by Festive Wings
			{
				Vector2 center = projectile.Center;
				float maxDistance = 460f;
				bool homeIn = false;

				for (int i = 0; i < Main.maxNPCs; i++)
				{
					if (Main.npc[i].CanBeChasedBy(projectile, false))
					{
						float extraDistance = (float)(Main.npc[i].width / 2) + (Main.npc[i].height / 2);

						bool canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

						if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
						{
							center = Main.npc[i].Center;
							homeIn = true;
							break;
						}
					}
				}

				if (homeIn)
				{
					Vector2 homeInVector = projectile.DirectionTo(center);
					if (homeInVector.HasNaNs())
						homeInVector = Vector2.UnitY;

					projectile.velocity = (projectile.velocity * 20f + homeInVector * 15f) / (21f);
				}
			}

            if (!projectile.npcProj && !projectile.trap && projectile.friendly && projectile.damage > 0)
			{
				if (modPlayer.eQuiver && projectile.ranged && CalamityMod.rangedProjectileExceptionList.TrueForAll(x => projectile.type != x))
				{
					if (Main.rand.Next(200) > 198)
					{
						float spread = 180f * 0.0174f;
						double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
						if (projectile.owner == Main.myPlayer)
						{
							int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(startAngle) * 8f), (float)(Math.Cos(startAngle) * 8f), projectile.type, (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
							int projectile2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(startAngle) * 8f), (float)(-Math.Cos(startAngle) * 8f), projectile.type, (int)(projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
							Main.projectile[projectile1].ranged = false;
							Main.projectile[projectile2].ranged = false;
							Main.projectile[projectile1].timeLeft = 60;
							Main.projectile[projectile2].timeLeft = 60;
							Main.projectile[projectile1].noDropItem = true;
							Main.projectile[projectile2].noDropItem = true;
						}
					}
				}

				counter++;
				if (modPlayer.fungalSymbiote && trueMelee)
				{
					if (counter % 6 == 0)
					{
						if (projectile.owner == Main.myPlayer && player.ownedProjectileCounts[ProjectileID.Mushroom] < 30)
						{
							//Note: these don't count as true melee anymore but its useful code to keep around
							if (projectile.type == ProjectileType<NebulashFlail>() || projectile.type == ProjectileType<CosmicDischargeFlail>() ||
								projectile.type == ProjectileType<MourningstarFlail>() || projectile.type == ProjectileID.SolarWhipSword)
							{
								Vector2 vector24 = Main.OffsetsPlayerOnhand[Main.player[projectile.owner].bodyFrame.Y / 56] * 2f;
								if (Main.player[projectile.owner].direction != 1)
								{
									vector24.X = player.bodyFrame.Width - vector24.X;
								}
								if (Main.player[projectile.owner].gravDir != 1f)
								{
									vector24.Y = player.bodyFrame.Height - vector24.Y;
								}
								vector24 -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
								Vector2 newCenter = player.RotatedRelativePoint(player.position + vector24, true) + projectile.velocity;
								Projectile.NewProjectile(newCenter.X, newCenter.Y, 0f, 0f, ProjectileID.Mushroom,
									(int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
							}
							else
							{
								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileID.Mushroom,
									(int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
							}
						}
					}
				}

				if (modPlayer.nanotech && rogue && projectile.type != ProjectileType<MoonSigil>() && projectile.type != ProjectileType<DragonShit>())
				{
					if (counter % 30 == 0)
					{
						if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ProjectileType<Nanotech>()] < 25)
						{
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileType<Nanotech>(),
								(int)(projectile.damage * 0.1), 0f, projectile.owner, 0f, 0f);
						}
					}
				}
				if (modPlayer.dragonScales && rogue && projectile.type != ProjectileType<MoonSigil>() && projectile.type != ProjectileType<DragonShit>())
				{
					if (counter % 50 == 0)
					{
						if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[ProjectileType<DragonShit>()] < 15)
						{
							//spawn a dust that does 1/5th of the original damage
							int projectileID = Projectile.NewProjectile(projectile.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi), ProjectileType<DragonShit>(),
								(int)(projectile.damage * 0.2), 0f, projectile.owner, 0f, 0f);
						}
					}
				}

				if (modPlayer.daedalusSplit && rogue)
				{
					if (counter % 30 == 0)
					{
						if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[90] < 30)
						{
							for (int i = 0; i < 2; i++)
							{
								Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
								int shard = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.CrystalShard, (int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
								Main.projectile[shard].ranged = false;
							}
						}
					}
				}
				//will always be friendly and rogue if it has this boost
				if (modPlayer.momentumCapacitor && momentumCapacitatorBoost)
				{
					if (projectile.velocity.Length() < 26f)
						projectile.velocity *= 1.05f;
				}

				if (modPlayer.theBee && projectile.owner == Main.myPlayer && projectile.damage > 0 && player.statLife >= player.statLifeMax2)
				{
					if (Main.rand.NextBool(5))
					{
						int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, 91, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
						Main.dust[dust].noGravity = true;
					}
				}

				if (modPlayer.providenceLore && projectile.owner == Main.myPlayer && projectile.damage > 0 &&
					(projectile.melee || projectile.ranged || projectile.magic || projectile.thrown || rogue))
				{
					if (Main.rand.NextBool(5))
					{
						int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 0.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].noLight = true;
					}
				}

				if (!projectile.melee && player.meleeEnchant > 0 && !projectile.noEnchantments)
				{
					if (player.meleeEnchant == 7) //flask of party affects all types of weapons
					{
						Vector2 velocity = projectile.velocity;
						if (velocity.Length() > 4.0)
							velocity *= 4f / velocity.Length();
						if (Main.rand.NextBool(20))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, Main.rand.Next(139, 143), velocity.X, velocity.Y, 0, new Color(), 1.2f);
							Main.dust[index].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
							Main.dust[index].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
							Main.dust[index].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
							Main.dust[index].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
							Main.dust[index].scale *= (float)(1.0 + Main.rand.Next(-30, 31) * 0.01);
						}
						if (Main.rand.NextBool(40))
						{
							int Type = Main.rand.Next(276, 283);
							int index = Gore.NewGore(projectile.position, velocity, Type, 1f);
							Main.gore[index].velocity.X *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
							Main.gore[index].velocity.Y *= (float)(1.0 + Main.rand.Next(-50, 51) * 0.01);
							Main.gore[index].scale *= (float)(1.0 + Main.rand.Next(-20, 21) * 0.01);
							Main.gore[index].velocity.X += Main.rand.Next(-50, 51) * 0.05f;
							Main.gore[index].velocity.Y += Main.rand.Next(-50, 51) * 0.05f;
						}
					}
				}

				if (rogue && player.meleeEnchant > 0 && !projectile.noEnchantments)
				{
					if (player.meleeEnchant == 1 && Main.rand.NextBool(3))
					{
						int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 171, 0.0f, 0.0f, 100, new Color(), 1f);
						Main.dust[index].noGravity = true;
						Main.dust[index].fadeIn = 1.5f;
						Main.dust[index].velocity *= 0.25f;
					}
					if (player.meleeEnchant == 1)
					{
						if (Main.rand.NextBool(3))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 171, 0.0f, 0.0f, 100, new Color(), 1f);
							Main.dust[index].noGravity = true;
							Main.dust[index].fadeIn = 1.5f;
							Main.dust[index].velocity *= 0.25f;
						}
					}
					else if (player.meleeEnchant == 2)
					{
						if (Main.rand.NextBool(2))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 75, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
							Main.dust[index].noGravity = true;
							Main.dust[index].velocity *= 0.7f;
							Main.dust[index].velocity.Y -= 0.5f;
						}
					}
					else if (player.meleeEnchant == 3)
					{
						if (Main.rand.NextBool(2))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 6, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 2.5f);
							Main.dust[index].noGravity = true;
							Main.dust[index].velocity *= 0.7f;
							Main.dust[index].velocity.Y -= 0.5f;
						}
					}
					else if (player.meleeEnchant == 4)
					{
						if (Main.rand.NextBool(2))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 57, projectile.velocity.X * 0.2f + (projectile.direction * 3), projectile.velocity.Y * 0.2f, 100, new Color(), 1.1f);
							Main.dust[index].noGravity = true;
							Main.dust[index].velocity.X /= 2f;
							Main.dust[index].velocity.Y /= 2f;
						}
					}
					else if (player.meleeEnchant == 5)
					{
						if (Main.rand.NextBool(2))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 169, 0.0f, 0.0f, 100, new Color(), 1f);
							Main.dust[index].velocity.X += projectile.direction;
							Main.dust[index].velocity.Y += 0.2f;
							Main.dust[index].noGravity = true;
						}
					}
					else if (player.meleeEnchant == 6)
					{
						if (Main.rand.NextBool(2))
						{
							int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 135, 0.0f, 0.0f, 100, new Color(), 1f);
							Main.dust[index].velocity.X += projectile.direction;
							Main.dust[index].velocity.Y += 0.2f;
							Main.dust[index].noGravity = true;
						}
					}
					else if (player.meleeEnchant == 8 && Main.rand.NextBool(4))
					{
						int index = Dust.NewDust(projectile.position, projectile.width, projectile.height, 46, 0.0f, 0.0f, 100, new Color(), 1f);
						Main.dust[index].noGravity = true;
						Main.dust[index].fadeIn = 1.5f;
						Main.dust[index].velocity *= 0.25f;
					}
				}

				if (rogue)
				{
					// Moon Crown gets overridden by Nanotech
					if (modPlayer.moonCrown && !modPlayer.nanotech)
					{
						//Summon moon sigils infrequently
						if (Main.rand.NextBool(300) && projectile.type != ProjectileType<MoonSigil>() && projectile.type != ProjectileType<DragonShit>())
						{
							Projectile.NewProjectile(projectile.position, Vector2.Zero, ProjectileType<MoonSigil>(), (int)(projectile.damage * 0.2), 0, projectile.owner);
						}
					}
				}
			}
		}
        #endregion

        #region PostAI
        public override void PostAI(Projectile projectile)
        {
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

			// Set crit here to avoid issues with projectiles that change class types in PreAI and AI
			if (defCrit == 0 && !projectile.npcProj && !projectile.trap)
			{
				if (projectile.melee)
					defCrit = modPlayer.critStats[0];
				else if (projectile.ranged)
					defCrit = modPlayer.critStats[1];
				else if (projectile.magic)
					defCrit = modPlayer.critStats[2];
				else if (rogue)
					defCrit = modPlayer.critStats[3];
				else if (projectile.IsSummon())
					defCrit = 4;
				else if (player.ActiveItem().crit > 0)
					defCrit = player.ActiveItem().crit;
			}

			if (projectile.owner == Main.myPlayer/* && Main.netMode != NetmodeID.MultiplayerClient*/)
			{
				int x = (int)(projectile.Center.X / 16f);
				int y = (int)(projectile.Center.Y / 16f);
				for (int i = x - 1; i <= x + 1; i++)
				{
					for (int j = y - 1; j <= y + 1; j++)
					{
						if (projectile.type == ProjectileID.PureSpray || projectile.type == ProjectileID.PurificationPowder)
						{
							WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Pure);
						}
						//commented out for Terraria 1.4 when vile/vicious powder spread corruption/crimson
						if (projectile.type == ProjectileID.CorruptSpray)// || projectile.type == ProjectileID.VilePowder)
						{
							WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Corrupt);
						}
						if (projectile.type == ProjectileID.CrimsonSpray)// || projectile.type == ProjectileID.ViciousPowder)
						{
							WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Crimson);
						}
						if (projectile.type == ProjectileID.HallowSpray)
						{
							WorldGenerationMethods.ConvertFromAstral(i, j, ConvertType.Hallow);
						}
					}
				}
			}
        }
        #endregion

		#region ModifyHitNPC
		public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

			// Super dummies have nearly 10 million max HP (which is used in damage calculations).
			// This can very easily cause damage numbers that are unrealistic for the weapon.
			// As a result, they are omitted in this code.
			if (!target.boss && target.type != NPCType<SuperDummyNPC>())
			{
				if (target.Inorganic() && hasInorganicEnemyHitBoost)
				{
					damage += (int)(target.lifeMax * inorganicEnemyHitBoost);
					inorganicEnemyHitEffect?.Invoke(target);
				}
				if (target.Organic() && hasOrganicEnemyHitBoost)
				{
					damage += (int)(target.lifeMax * organicEnemyHitBoost);
					organicEnemyHitEffect?.Invoke(target);
				}
			}

			if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap)
			{
				int critMax = 100;
				int critChance = (int)MathHelper.Clamp(defCrit, 1, critMax);
				crit = Main.rand.Next(1, critMax + 1) <= critChance;

				if ((uint)(projectile.type - ProjectileID.DD2LightningAuraT1) <= 2u)
				{
					if (player.setMonkT3)
						crit = Main.rand.NextBool(4);
					else if (player.setMonkT2)
						crit = Main.rand.NextBool(6);
				}

				if (rogue && stealthStrike && modPlayer.stealthStrikeAlwaysCrits)
					crit = true;

				//Following things need to be done in here.  If done in the projectile file, it's overridden by the thing above
				if (projectile.type == ProjectileType<PwnagehammerProj>() && projectile.ai[0] == 1f)
					crit = true;

				if (projectile.type == ProjectileType<ImpactRound>())
				{
					double damageMult = 1D;
					if (crit)
					{
						damageMult += 0.25;
					}
					if (target.Inorganic())
					{
						damageMult += 0.1;
					}
					damage = (int)(damage * damageMult);
				}

				if (projectile.type == ProjectileType<SphereSpiked>())
				{
					damage = (int)(damage * 1.2);
					if (!crit)
						crit = Main.rand.NextBool(10);
				}

				if (projectile.type == ProjectileType<MagnumRound>())
				{
					if (crit)
					{
						damage = (int)(damage * 1.25);
						knockback *= 1.25f;
					}
				}
			}
		}
		#endregion

        #region OnHitNPC
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
        {
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

            if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap && projectile.friendly)
            {
				if (ProjectileID.Sets.StardustDragon[projectile.type])
				{
					target.immune[projectile.owner] = 10;
				}

				//flask of party affects all types of weapons, !projectile.melee is to prevent double flask effects
                if (!projectile.melee && player.meleeEnchant == 7)
					Projectile.NewProjectile(target.Center, target.velocity, ProjectileID.ConfettiMelee, 0, 0f, projectile.owner, 0f, 0f);

                if (rogue && stealthStrike && modPlayer.dragonScales && CalamityUtils.CountProjectiles(ProjectileType<InfernadoFriendly>()) < 2)
                {
                    int projTileX = (int)(projectile.Center.X / 16f);
                    int projTileY = (int)(projectile.Center.Y / 16f);
                    int distance = 100;
                    if (projTileX < 10)
                    {
                        projTileX = 10;
                    }
                    if (projTileX > Main.maxTilesX - 10)
                    {
                        projTileX = Main.maxTilesX - 10;
                    }
                    if (projTileY < 10)
                    {
                        projTileY = 10;
                    }
                    if (projTileY > Main.maxTilesY - distance - 10)
                    {
                        projTileY = Main.maxTilesY - distance - 10;
                    }
                    for (int x = projTileX; x < projTileX + distance; x++)
                    {
                        Tile tile = Main.tile[projTileX, projTileY];
                        if (tile.active() && (Main.tileSolid[tile.type] || tile.liquid != 0))
                        {
                            projTileX = x;
                            break;
                        }
                    }
                    int projectileIndex = Projectile.NewProjectile(projTileX * 16 + 8, projTileY * 16 - 24, 0f, 0f, ProjectileType<InfernadoFriendly>(), 420, 15f, Main.myPlayer, 16f, 16f);
                    Main.projectile[projectileIndex].Calamity().forceRogue = true;
                    Main.projectile[projectileIndex].netUpdate = true;
                    Main.projectile[projectileIndex].localNPCHitCooldown = 1;
                }

				// Spectre Damage set and Nebula set work on enemies which are "immune to lifesteal"
                if (!target.canGhostHeal)
				{
					if (player.ghostHurt)
					{
						projectile.ghostHurt(damage, target.Center);
					}

					if (player.setNebula && player.nebulaCD == 0 && Main.rand.Next(3) == 0)
					{
						player.nebulaCD = 30;
						int boosterType = Utils.SelectRandom(Main.rand, new int[]
						{
							ItemID.NebulaPickup1,
							ItemID.NebulaPickup2,
							ItemID.NebulaPickup3
						});
						int nebulaBooster = Item.NewItem(target.Center, target.Size, boosterType, 1, false, 0, false, false);
						Main.item[nebulaBooster].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
						Main.item[nebulaBooster].velocity.X = Main.rand.Next(10, 31) * 0.2f * projectile.direction;
						if (Main.netMode == NetmodeID.MultiplayerClient)
						{
							NetMessage.SendData(MessageID.SyncItem, -1, -1, null, nebulaBooster, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}

				if (Main.player[Main.myPlayer].lifeSteal > 0f && target.canGhostHeal && target.type != NPCID.TargetDummy && target.type != NPCType<SuperDummyNPC>() && !player.moonLeech)
				{
					// Increases the degree to which Spectre Healing set contributes to the lifesteal cap
					if (player.ghostHeal)
					{
						float cooldownMult = 0.1f;
						cooldownMult -= projectile.numHits * 0.025f;
						if (cooldownMult < 0f)
							cooldownMult = 0f;

						float cooldown = damage * cooldownMult;
						Main.player[Main.myPlayer].lifeSteal -= cooldown;
					}

					// Increases the degree to which Vampire Knives contribute to the lifesteal cap
					if (projectile.type == ProjectileID.VampireKnife)
					{
						float cooldown = damage * 0.075f;
						if (cooldown < 0f)
							cooldown = 0f;

						Main.player[Main.myPlayer].lifeSteal -= cooldown;
					}

					if (modPlayer.vampiricTalisman && rogue && crit)
					{
						float heal = MathHelper.Clamp(damage * 0.015f, 0f, 6f);
						if ((int)heal > 0)
						{
							SpawnLifeStealProjectile(projectile, player, heal, ProjectileID.VampireHeal, 1200f, 2f);
						}
					}

					if ((modPlayer.bloodyGlove || modPlayer.electricianGlove) && rogue && stealthStrike)
					{
						player.statLife += 1;
						player.HealEffect(1);
					}

					bool otherHealTypes = modPlayer.auricSet || modPlayer.silvaSet || modPlayer.godSlayerMage || modPlayer.tarraMage || modPlayer.ataxiaMage;

					if (projectile.magic && player.ActiveItem().magic)
					{
						if (modPlayer.manaOverloader && otherHealTypes)
						{
							if (Main.rand.NextBool(2))
							{
								float healMult = 0.2f;
								healMult -= projectile.numHits * 0.05f;
								float heal = projectile.damage * healMult * (player.statMana / (float)player.statManaMax2);

								if (heal > 50)
									heal = 50;

								if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
									goto OTHEREFFECTS;

								SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<ManaOverloaderHealOrb>(), 1200f, 2f);
								goto OTHEREFFECTS;
							}
						}
					}

					if (modPlayer.auricSet)
					{
						float healMult = 0.05f;
						healMult -= projectile.numHits * 0.025f;
						float heal = projectile.damage * healMult;

						if (heal > 50)
							heal = 50;

						if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
							goto OTHEREFFECTS;

						SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<AuricOrb>(), 1200f, 3f);
					}
					else if (modPlayer.silvaSet)
					{
						float healMult = 0.03f;
						healMult -= projectile.numHits * 0.015f;
						float heal = projectile.damage * healMult;

						if (heal > 50)
							heal = 50;

						if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
							goto OTHEREFFECTS;

						SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<SilvaOrb>(), 1200f, 3f);
					}
					else if (projectile.magic && player.ActiveItem().magic)
					{
						if (modPlayer.manaOverloader)
						{
							float healMult = 0.2f;
							healMult -= projectile.numHits * 0.05f;
							float heal = projectile.damage * healMult * (player.statMana / (float)player.statManaMax2);

							if (heal > 50)
								heal = 50;

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<ManaOverloaderHealOrb>(), 1200f, 2f);
						}
						if (modPlayer.godSlayerMage)
						{
							float healMult = 0.06f;
							healMult -= projectile.numHits * 0.015f;
							float heal = projectile.damage * healMult;

							if (heal > 50)
								heal = 50;

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<GodSlayerHealOrb>(), 1200f, 2f);
						}
						else if (modPlayer.tarraMage)
						{
							if (modPlayer.tarraMageHealCooldown <= 0)
							{
								modPlayer.tarraMageHealCooldown = 90;

								float healMult = 0.1f;
								healMult -= projectile.numHits * 0.05f;
								float heal = projectile.damage * healMult;

								if (heal > 50)
									heal = 50;

								if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
									goto OTHEREFFECTS;

								Main.player[Main.myPlayer].lifeSteal -= heal * 6f;
								int healAmount = (int)heal;
								player.statLife += healAmount;
								player.HealEffect(healAmount);

								if (player.statLife > player.statLifeMax2)
									player.statLife = player.statLifeMax2;
							}
						}
						else if (modPlayer.ataxiaMage)
						{
							float healMult = 0.1f;
							healMult -= projectile.numHits * 0.05f;
							float heal = projectile.damage * healMult;

							if (heal > 50)
								heal = 50;

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<AtaxiaHealOrb>(), 1200f, 2f);
						}
						else if (modPlayer.manaOverloader)
						{
							float healMult = 0.2f;
							healMult -= projectile.numHits * 0.05f;
							float heal = projectile.damage * healMult * (player.statMana / (float)player.statManaMax2);

							if (heal > 50)
								heal = 50;

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<ManaOverloaderHealOrb>(), 1200f, 2f);
						}
					}
				}

				OTHEREFFECTS:

                if (modPlayer.alchFlask &&
                    (projectile.magic || rogue || projectile.melee || projectile.IsSummon() || projectile.ranged) &&
                    player.ownedProjectileCounts[ProjectileType<PlagueSeeker>()] < 6)
                {
                    int plague = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<PlagueSeeker>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.25, 30), 0f, projectile.owner, 0f, 0f);
                    Main.projectile[plague].Calamity().forceTypeless = true;
                }

                if (modPlayer.reaverBlast && projectile.melee)
                {
                    Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<ReaverBlast>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.2, 30), 0f, projectile.owner, 0f, 0f);
                }

                if (projectile.magic)
                {
                    if (modPlayer.silvaMage && projectile.penetrate == 1 && Main.rand.Next(0, 100) >= 97)
                    {
                        Main.PlaySound(SoundID.Zombie, (int)projectile.position.X, (int)projectile.position.Y, 103);
						ExpandHitboxBy(projectile, 96);
						for (int d = 0; d < 3; d++)
						{
							Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
						}
						for (int d = 0; d < 30; d++)
						{
							int explode = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 2.5f);
							Main.dust[explode].noGravity = true;
							Main.dust[explode].velocity *= 3f;
							explode = Dust.NewDust(projectile.position, projectile.width, projectile.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
							Main.dust[explode].velocity *= 2f;
							Main.dust[explode].noGravity = true;
						}
                        projectile.damage *= modPlayer.auricSet ? 7 : 4;
						projectile.localNPCHitCooldown = 10;
						projectile.usesLocalNPCImmunity = true;
                        projectile.Damage();
                    }

					if (modPlayer.reaverBurst)
					{
						int projAmt = Main.rand.Next(2, 5);
						for (int i = 0; i < projAmt; i++)
						{
							Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
							int proj = Projectile.NewProjectile(projectile.Center, velocity, ProjectileID.SporeGas + Main.rand.Next(3), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 15), 0f, projectile.owner, 0f, 0f);
							Main.projectile[proj].usesLocalNPCImmunity = true;
							Main.projectile[proj].localNPCHitCooldown = 30;
						}
					}
					else if (modPlayer.ataxiaMage && modPlayer.ataxiaDmg <= 0)
					{
						SpawnOrb(projectile, (int)(projectile.damage * 0.625), ProjectileType<AtaxiaOrb>(), 800f, 20f);
						int cooldown = (int)(projectile.damage * 0.5f);
						modPlayer.ataxiaDmg += cooldown;
					}
					else if (modPlayer.godSlayerMage && modPlayer.godSlayerDmg <= 0)
					{
						SpawnOrb(projectile, (int)(projectile.damage * (modPlayer.auricSet ? 1 : 0.75)), ProjectileType<GodSlayerOrb>(), 800f, 20f);
						int cooldown = (int)(projectile.damage * 0.5f);
						modPlayer.godSlayerDmg += cooldown;
					}
                }
                if (projectile.melee)
                {
                    if (modPlayer.ataxiaGeyser && player.ownedProjectileCounts[ProjectileType<ChaosGeyser>()] < 3)
                    {
                        Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<ChaosGeyser>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 35), 0f, projectile.owner, 0f, 0f);
                    }
                }
                if (rogue)
                {
                    if (modPlayer.xerocSet && modPlayer.xerocDmg <= 0 && player.ownedProjectileCounts[ProjectileType<XerocFire>()] < 3 && player.ownedProjectileCounts[ProjectileType<XerocBlast>()] < 3)
                    {
						int cooldown = (int)(projectile.damage * 0.5f);
						switch (Main.rand.Next(5))
						{
							case 0:

								SpawnOrb(projectile, (int)(projectile.damage * 0.8), ProjectileType<XerocStar>(), 800f, Main.rand.Next(15, 30));
								modPlayer.xerocDmg += cooldown;

								break;

							case 1:

								SpawnOrb(projectile, (int)(projectile.damage * 0.625), ProjectileType<XerocOrb>(), 800f, 30f);
								modPlayer.xerocDmg += cooldown;

								if (target.canGhostHeal && Main.player[Main.myPlayer].lifeSteal > 0f)
								{
									float healMult = 0.06f;
									healMult -= projectile.numHits * 0.015f;
									float heal = projectile.damage * healMult;

									if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
										goto SKIPXEROC;

									SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<XerocHealOrb>(), 1200f, 1.5f);
								}

								break;

							case 2:

								Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<XerocFire>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 40), 0f, projectile.owner, 0f, 0f);

								break;

							case 3:

								Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<XerocBlast>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.2, 40), 0f, projectile.owner, 0f, 0f);

								break;

							case 4:

								SpawnOrb(projectile, (int)(projectile.damage * 0.6), ProjectileType<XerocBubble>(), 800f, 15f);
								modPlayer.xerocDmg += cooldown;

								break;

							default:
								break;
						}
					}

					SKIPXEROC:

                    if (modPlayer.featherCrown && stealthStrike && modPlayer.featherCrownCooldown <= 0 && stealthStrikeHitCount < 5)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 source = new Vector2(target.Center.X + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            float speedX = (target.Center.X - source.X) / 30f;
                            float speedY = (target.Center.Y - source.Y) * 8;
							Vector2 velocity = new Vector2(speedX, speedY);
                            int dmg = (int)(15 + (projectile.damage * 0.05f));
                            int feather = Projectile.NewProjectile(source, velocity, ProjectileType<StickyFeather>(), dmg, 3f, projectile.owner);
                            Main.projectile[feather].Calamity().forceRogue = true;
                            modPlayer.featherCrownCooldown = 15;
                        }
                    }

                    if (modPlayer.moonCrown && stealthStrike && modPlayer.moonCrownCooldown <= 0 && stealthStrikeHitCount < 5)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 source = new Vector2(target.Center.X + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            Vector2 velocity = (target.Center - source) / 10f;
                            int dmg = (int)(150 + (projectile.damage * 0.05f));
                            int flare = Projectile.NewProjectile(source, velocity, ProjectileID.LunarFlare, dmg, 3, projectile.owner);
                            Main.projectile[flare].Calamity().forceRogue = true;
                            modPlayer.moonCrownCooldown = 15;
                        }
                    }

                    if (modPlayer.nanotech && stealthStrike && modPlayer.nanoFlareCooldown <= 0 && stealthStrikeHitCount < 5)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 source = new Vector2(target.Center.X + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            Vector2 velocity = (target.Center - source) / 40f;
                            int dmg = (int)(1000 + (projectile.damage * 0.05f));
                            int flare = Projectile.NewProjectile(source, velocity, ProjectileType<NanoFlare>(),dmg, 3f, projectile.owner);
                            Main.projectile[flare].Calamity().rogue = true;
                            modPlayer.nanoFlareCooldown = 15;
                        }
                    }

					if (modPlayer.forbiddenCirclet && stealthStrike && modPlayer.forbiddenCooldown <= 0 && stealthStrikeHitCount < 5)
					{
						for (int index2 = 0; index2 < 6; index2++)
						{
							float xVector = Main.rand.Next(-35, 36) * 0.02f;
							float yVector = Main.rand.Next(-35, 36) * 0.02f;
							xVector *= 10f;
							yVector *= 10f;
                            int dmg = (int)(75 + (projectile.damage * 0.05f));
							Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, xVector, yVector, ProjectileType<ForbiddenCircletEater>(), dmg, projectile.knockBack, projectile.owner, 0f, 0f);
                            modPlayer.forbiddenCooldown = 15;
						}
					}

					if (modPlayer.titanHeartSet && stealthStrike && modPlayer.titanCooldown <= 0 && stealthStrikeHitCount < 5)
					{
						int dmg = (int)(85 + (projectile.damage * 0.05f));
						int boom = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<SabatonBoom>(), dmg, projectile.knockBack, projectile.owner, 0f, 0f);
						Main.projectile[boom].Calamity().forceRogue = true;
						Main.PlaySound(SoundID.Item14, projectile.position);
						for (int dustexplode = 0; dustexplode < 360; dustexplode++)
						{
							Vector2 dustd = new Vector2(17f, 17f).RotatedBy(MathHelper.ToRadians(dustexplode));
							int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, Main.rand.NextBool(2) ? ModContent.DustType<AstralBlue>() : ModContent.DustType<AstralOrange>(), dustd.X, dustd.Y, 100, default, 3f);
							Main.dust[d].noGravity = true;
							Main.dust[d].position = projectile.Center;
						}
						modPlayer.titanCooldown = 15;
					}

					if (modPlayer.corrosiveSpine && projectile.type != ProjectileType<Corrocloud1>() && projectile.type != ProjectileType<Corrocloud2>() && projectile.type != ProjectileType<Corrocloud3>() && stealthStrikeHitCount < 5)
					{
						for (int i = 0; i < 3; i++)
						{
							if (Main.rand.NextBool(2))
							{
								int type = -1;
								switch (Main.rand.Next(15))
								{
									case 0:
										type = ProjectileType<Corrocloud1>();
										break;
									case 1:
										type = ProjectileType<Corrocloud2>();
										break;
									case 2:
										type = ProjectileType<Corrocloud3>();
										break;
								}
								// Should never happen, but just in case-
								if (type != -1)
								{
									float speed = Main.rand.NextFloat(5f, 11f);
									Projectile.NewProjectile(target.Center, Vector2.One.RotatedByRandom(MathHelper.TwoPi) * speed,
										type, (int)(projectile.damage * 0.6), projectile.knockBack, player.whoAmI);
								}
							}
						}
						target.AddBuff(BuffID.Venom, 240);
					}

					if (modPlayer.shadow && modPlayer.shadowPotCooldown <= 0 && stealthStrikeHitCount < 5)
					{
						if (CalamityMod.javelinProjList.Contains(projectile.type))
						{
							int randrot = Main.rand.Next(-30, 391);
							Vector2 SoulSpeed = new Vector2(13f, 13f).RotatedBy(MathHelper.ToRadians(randrot));
							Projectile.NewProjectile(projectile.Center, SoulSpeed, ModContent.ProjectileType<PenumbraSoul>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.1, 60), 3f, projectile.owner, 0f, 0f);
							modPlayer.shadowPotCooldown = 30;
						}
						if (CalamityMod.spikyBallProjList.Contains(projectile.type))
						{
							int scythe = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CosmicScythe>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.05, 60), 3f, projectile.owner, 1f, 0f);
            				Main.projectile[scythe].usesLocalNPCImmunity = true;
            				Main.projectile[scythe].localNPCHitCooldown = 10;
							Main.projectile[scythe].penetrate = 2;
							modPlayer.shadowPotCooldown = 30;
						}
						if (CalamityMod.daggerProjList.Contains(projectile.type))
						{
							Vector2 shardVelocity = new Vector2(Main.rand.NextFloat(-1f, 1f), Main.rand.NextFloat(-1f, 1f));
							shardVelocity.Normalize();
							shardVelocity *= 5f;
							int shard = Projectile.NewProjectile(projectile.Center, shardVelocity, ModContent.ProjectileType<EquanimityDarkShard>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 60), 0f, projectile.owner);
							Main.projectile[shard].timeLeft = 150;
							modPlayer.shadowPotCooldown = 30;
						}
						if (CalamityMod.boomerangProjList.Contains(projectile.type))
						{
							int spiritDamage = CalamityUtils.DamageSoftCap(projectile.damage * 0.2, 60);
							Projectile ghost = SpawnOrb(projectile, spiritDamage, ProjectileID.SpectreWrath, 800f, 4f);
							ghost.Calamity().forceRogue = true;
							ghost.penetrate = 1;
							modPlayer.shadowPotCooldown = 30;
						}
						if (CalamityMod.flaskBombProjList.Contains(projectile.type))
						{
							int blackhole = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<ShadowBlackhole>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.05, 60), 3f, projectile.owner, 0f, 0f);
							Main.projectile[blackhole].Center = projectile.Center;
							modPlayer.shadowPotCooldown = 30;
						}
					}
                }
                if (projectile.IsSummon())
                {
                    if (modPlayer.pArtifact && !modPlayer.profanedCrystal)
                    {
                        target.AddBuff(BuffType<HolyFlames>(), 300);
                    }
					if (modPlayer.profanedCrystalBuffs)
					{
						target.AddBuff(Main.dayTime ? BuffType<HolyFlames>() : BuffType<Nightwither>(), 600);
					}

                    if (modPlayer.tearMinions)
                    {
                        target.AddBuff(BuffType<TemporalSadness>(), 60);
                    }

                    if (modPlayer.shadowMinions)
                    {
                        target.AddBuff(BuffID.ShadowFlame, 300);
                    }

                    if (modPlayer.voltaicJelly)
					{
						//100% chance for Star Tainted Generator or Nucleogenesis
						//20% chance for Voltaic Jelly
						if (Main.rand.NextBool(modPlayer.starTaintedGenerator ? 1 : 5))
						{
							target.AddBuff(BuffID.Electrified, 60);
						}
					}

                    if (modPlayer.starTaintedGenerator)
                    {
                        target.AddBuff(BuffType<AstralInfectionDebuff>(), 180);
                        target.AddBuff(BuffType<Irradiated>(), 180);
                    }

                    // Fearmonger set's colossal life regeneration
                    if(modPlayer.fearmongerSet)
                    {
                        modPlayer.fearmongerRegenFrames += 20;
                        if (modPlayer.fearmongerRegenFrames > 180)
                            modPlayer.fearmongerRegenFrames = 180;
                    }

                    if (modPlayer.godSlayerSummon && modPlayer.godSlayerDmg <= 0)
                    {
						SpawnOrb(projectile, projectile.damage, ProjectileType<GodSlayerPhantom>(), 800f, 15f, true);
						int cooldown = (int)(projectile.damage * 0.5f);
						modPlayer.godSlayerDmg += cooldown;
                    }

					//Priorities: Nucleogenesis => Starbuster Core => Nuclear Rod => Jelly-Charged Battery
					List<int> summonExceptionList = new List<int>()
					{ 
						ProjectileType<EnergyOrb>(),
						ProjectileType<IrradiatedAura>(),
						ProjectileType<SummonAstralExplosion>(),
						ProjectileType<ApparatusExplosion>(),
						ProjectileType<HallowedStarSummon>()
					};

					if (summonExceptionList.TrueForAll(x => projectile.type != x))
					{
						if (modPlayer.jellyDmg <= 0)
						{
							if (modPlayer.nucleogenesis)
							{
								if (Main.rand.NextBool(4))
								{
									Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<ApparatusExplosion>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.25, 90), projectile.knockBack * 0.25f, projectile.owner);
									modPlayer.jellyDmg = 25f;
								}
							}
							else if (modPlayer.starbusterCore)
							{
								if (Main.rand.NextBool(3))
								{
									int cap = modPlayer.starTaintedGenerator ? 75 : 60;
									Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<SummonAstralExplosion>(),
										CalamityUtils.DamageSoftCap(projectile.damage * 0.5, cap), 3f, projectile.owner);
									modPlayer.jellyDmg = 20f;
								}
							}
							else if (modPlayer.nuclearRod)
							{
								if (Main.rand.NextBool(3))
								{
									Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<IrradiatedAura>(),
										CalamityUtils.DamageSoftCap(projectile.damage * 0.25, 40), 0f, projectile.owner);
									modPlayer.jellyDmg = 20f;
								}
							}
							else if (modPlayer.jellyChargedBattery)
							{
								SpawnOrb(projectile, (int)(projectile.damage * 0.525), ProjectileType<EnergyOrb>(), 800f, 15f);
								int cooldown = (int)(projectile.damage * 0.5f);
								modPlayer.jellyDmg += cooldown;
							}
						}

						if (modPlayer.hallowedPower)
						{
							if (Main.rand.NextBool(3) && modPlayer.hallowedRuneCooldown <= 0)
							{
								modPlayer.hallowedRuneCooldown = 60;
								Vector2 spawnPosition = target.Center - new Vector2(0f, 920f).RotatedByRandom(0.3f);
								float speed = Main.rand.NextFloat(17f, 23f);
								Projectile.NewProjectile(spawnPosition, Vector2.Normalize(target.Center - spawnPosition) * speed,
									ProjectileType<HallowedStarSummon>(), projectile.damage / 3, 3f, projectile.owner);
							}
						}
					}
                }

                if (projectile.ranged)
                {
                    if (modPlayer.tarraRanged && Main.rand.Next(0, 100) >= 88 && player.ownedProjectileCounts[ProjectileType<TarraEnergy>()] <= 20 && (projectile.timeLeft <= 2 || projectile.penetrate <= 1))
                    {
                        int projAmt = Main.rand.Next(2, 4);
                        for (int projCount = 0; projCount < projAmt; projCount++)
                        {
							Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                            Projectile.NewProjectile(projectile.Center, velocity, ProjectileType<TarraEnergy>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.33, 65), 0f, projectile.owner, 0f, 0f);
                        }
                    }
                }
            }
        }
        #endregion

        #region CanDamage
        public override bool CanDamage(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.Sharknado:
                    if (projectile.timeLeft > 420)
                        return false;
                    break;

                case ProjectileID.Cthulunado:
                    if (projectile.timeLeft > 720)
                        return false;
                    break;

                default:
                    break;
            }
            return true;
        }
        #endregion

        #region Drawing
        public override Color? GetAlpha(Projectile projectile, Color lightColor)
        {
            if (Main.player[Main.myPlayer].Calamity().trippy)
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);

            if (Main.player[Main.myPlayer].Calamity().omniscience && projectile.hostile)
                return Color.Coral;

            if (projectile.type == ProjectileID.PinkLaser)
            {
                if (projectile.alpha < 200)
                    return new Color(255 - projectile.alpha, 255 - projectile.alpha, 255 - projectile.alpha, 0);

                return Color.Transparent;
            }

            return null;
        }

        public override bool PreDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
        {
            if (Main.player[Main.myPlayer].Calamity().trippy)
            {
                Texture2D texture = Main.projectileTexture[projectile.type];
                SpriteEffects spriteEffects = SpriteEffects.None;
                if (projectile.spriteDirection == -1)
                {
                    spriteEffects = SpriteEffects.FlipHorizontally;
                }
                Vector2 vector11 = new Vector2(texture.Width / 2, texture.Height / Main.projFrames[projectile.type] / 2);
                Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);
                Color alpha15 = projectile.GetAlpha(color9);
                for (int num213 = 0; num213 < 4; num213++)
                {
                    Vector2 position9 = projectile.position;
                    float num214 = Math.Abs(projectile.Center.X - Main.player[Main.myPlayer].Center.X);
                    float num215 = Math.Abs(projectile.Center.Y - Main.player[Main.myPlayer].Center.Y);
                    if (num213 == 0 || num213 == 2)
                    {
                        position9.X = Main.player[Main.myPlayer].Center.X + num214;
                    }
                    else
                    {
                        position9.X = Main.player[Main.myPlayer].Center.X - num214;
                    }
                    position9.X -= projectile.width / 2;
                    if (num213 == 0 || num213 == 1)
                    {
                        position9.Y = Main.player[Main.myPlayer].Center.Y + num215;
                    }
                    else
                    {
                        position9.Y = Main.player[Main.myPlayer].Center.Y - num215;
                    }
                    int frames = texture.Height / Main.projFrames[projectile.type];
                    int y = frames * projectile.frame;
                    position9.Y -= projectile.height / 2;
                    Main.spriteBatch.Draw(texture,
                        new Vector2(position9.X - Main.screenPosition.X + (projectile.width / 2) - texture.Width * projectile.scale / 2f + vector11.X * projectile.scale, position9.Y - Main.screenPosition.Y + projectile.height - texture.Height * projectile.scale / Main.projFrames[projectile.type] + 4f + vector11.Y * projectile.scale + projectile.gfxOffY),
                        new Microsoft.Xna.Framework.Rectangle?(new Rectangle(0, y, texture.Width, frames)), alpha15, projectile.rotation, vector11, projectile.scale, spriteEffects, 0f);
                }
            }
            return true;
        }

		public static void DrawCenteredAndAfterimage(Projectile projectile, Color lightColor, int trailingMode, int afterimageCounter, Texture2D texture = null, bool drawCentered = true)
        {
            if (texture is null)
                texture = Main.projectileTexture[projectile.type];

            int frameHeight = texture.Height / Main.projFrames[projectile.type];
            int frameY = frameHeight * projectile.frame;
			float scale = projectile.scale;
			float rotation = projectile.rotation;

			Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
			Vector2 origin = rectangle.Size() / 2f;

			SpriteEffects spriteEffects = SpriteEffects.None;
            if (projectile.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

			if (CalamityConfig.Instance.Afterimages)
            {
                Vector2 centerOffset = drawCentered ? projectile.Size / 2f : Vector2.Zero;
                switch (trailingMode)
                {
                    case 0:
                        for (int i = 0; i < projectile.oldPos.Length; i++)
                        {
                            Vector2 drawPos = projectile.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY);
                            Color color = projectile.GetAlpha(lightColor) * ((projectile.oldPos.Length - i) / projectile.oldPos.Length);
                            Main.spriteBatch.Draw(texture, drawPos, rectangle, color, rotation, origin, scale, spriteEffects, 0f);
                        }
                        break;

                    case 1:
                        Color color25 = Lighting.GetColor((int)(projectile.Center.X / 16), (int)(projectile.Center.Y / 16));
                        int whyIsThisAlwaysEight = 8;
                        int k = 1;
                        while (k < whyIsThisAlwaysEight)
                        {
                            Color color26 = color25;
                            color26 = projectile.GetAlpha(color26);
                            float num164 = whyIsThisAlwaysEight - k;
                            color26 *= num164 / (ProjectileID.Sets.TrailCacheLength[projectile.type] * 1.5f);
                            Main.spriteBatch.Draw(texture, projectile.oldPos[k] + centerOffset - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, color26, rotation, origin, scale, spriteEffects, 0f);
                            k += afterimageCounter;
                        }
                        break;

                    default:
                        break;
                }
            }

            // Draw the projectile itself
            Vector2 startPos = drawCentered ? projectile.Center : projectile.position;
            Main.spriteBatch.Draw(texture, startPos - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), rectangle, projectile.GetAlpha(lightColor), rotation, origin, scale, spriteEffects, 0f);
        }
        #endregion

        #region Kill
        public override void Kill(Projectile projectile, int timeLeft)
        {
			CalamityPlayer modPlayer = Main.player[projectile.owner].Calamity();
            if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap)
            {
                if (modPlayer.providenceLore && projectile.friendly && projectile.damage > 0 && (projectile.melee || projectile.ranged || projectile.magic || rogue))
                {
                    Main.PlaySound(SoundID.Item20, projectile.Center);
                    for (int dustIndex = 0; dustIndex < 3; dustIndex++)
                    {
                        int fire = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 1f);
                        Main.dust[fire].noGravity = true;
                    }
                }

                if (rogue)
                {
                    if (modPlayer.etherealExtorter && Main.rand.Next(0, 100) >= 95)
                    {
                        for (int i = 0; i < 3; i++)
                        {
							Vector2 velocity = CalamityUtils.RandomVelocity(100f, 70f, 100f);
                            int soul = Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<LostSoulFriendly>(), (int)(projectile.damage * 0.33), 0f, projectile.owner, 0f, 0f);
							Main.projectile[soul].tileCollide = false;
                        }
                    }
					if (modPlayer.scuttlersJewel && CalamityMod.javelinProjList.Contains(projectile.type) && Main.rand.NextBool(3))
					{
						float dmgMult = 1f;
						if (projectile.type == ProjectileType<SpearofDestinyProjectile>())
							dmgMult = 0.5f;

						int spike = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<JewelSpike>(), (int)(projectile.damage * 0.5f * dmgMult), projectile.knockBack, projectile.owner);
						Main.projectile[spike].frame = 4;
					}
                }

				if (projectile.type == ProjectileID.UnholyWater)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 1f);
				}
				if (projectile.type == ProjectileID.BloodWater)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 2f);
				}
				if (projectile.type == ProjectileID.HolyWater)
				{
					Projectile.NewProjectile(projectile.Center, Vector2.Zero, ProjectileType<WaterConvertor>(), 0, 0f, projectile.owner, 3f);
				}
            }
        }
        #endregion

        #region CanHit
        public override bool CanHitPlayer(Projectile projectile, Player target)
        {
            if (projectile.type == ProjectileID.CultistBossLightningOrb)
            {
                return false;
            }
            return true;
        }
		#endregion

		#region LifeSteal
		public static bool CanSpawnLifeStealProjectile(Projectile projectile, float healMultiplier, float healAmount)
		{
			if (healMultiplier <= 0f || (int)healAmount <= 0)
				return false;

			return true;
		}

		public static void SpawnLifeStealProjectile(Projectile projectile, Player player, float healAmount, int healProjectileType, float distanceRequired, float cooldownMultiplier)
		{
			if (Main.player[Main.myPlayer].moonLeech)
				return;
			Main.player[Main.myPlayer].lifeSteal -= healAmount * cooldownMultiplier;
			float lowestHealthCheck = 0f;
			int healTarget = projectile.owner;
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				Player otherPlayer = Main.player[i];
				if (otherPlayer.active && !otherPlayer.dead && ((!player.hostile && !otherPlayer.hostile) || player.team == otherPlayer.team))
				{
					float playerDist = Vector2.Distance(projectile.Center, otherPlayer.Center);
					if (playerDist < distanceRequired && (otherPlayer.statLifeMax2 - otherPlayer.statLife) > lowestHealthCheck)
					{
						lowestHealthCheck = otherPlayer.statLifeMax2 - otherPlayer.statLife;
						healTarget = i;
					}
				}
			}
			Projectile.NewProjectile(projectile.Center, Vector2.Zero, healProjectileType, 0, 0f, projectile.owner, healTarget, healAmount);
		}
		#endregion

		#region AI Shortcuts
		public static Projectile SpawnOrb(Projectile projectile, int damage, int projType, float distanceRequired, float speedMult, bool gsPhantom = false)
        {
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

            float ai1 = Main.rand.NextFloat() + 0.5f;
			int[] array = new int[Main.maxNPCs];
			int targetArrayA = 0;
			int targetArrayB = 0;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				NPC npc = Main.npc[i];
				if (npc.CanBeChasedBy(projectile, false))
				{
					float enemyDist = Vector2.Distance(projectile.Center, npc.Center);
					if (enemyDist < distanceRequired)
					{
						if (Collision.CanHit(projectile.position, 1, 1, npc.position, npc.width, npc.height) && enemyDist > 50f)
						{
							array[targetArrayB] = i;
							targetArrayB++;
						}
						else if (targetArrayB == 0)
						{
							array[targetArrayA] = i;
							targetArrayA++;
						}
					}
				}
			}
			if (targetArrayA == 0 && targetArrayB == 0)
			{
				return Projectile.NewProjectileDirect(projectile.Center, Vector2.Zero, ProjectileType<NobodyKnows>(), 0, 0f, projectile.owner);
			}
			int target = targetArrayB <= 0 ? array[Main.rand.Next(targetArrayA)] : array[Main.rand.Next(targetArrayB)];
			Vector2 velocity = CalamityUtils.RandomVelocity(100f, speedMult, speedMult, 1f);
			Projectile orb = Projectile.NewProjectileDirect(projectile.Center, velocity, projType, damage, 0f, projectile.owner, gsPhantom ? 0f : target, gsPhantom ? ai1 : 0f);
			return orb;
		}

		public static void HomeInOnNPC(Projectile projectile, bool ignoreTiles, float distanceRequired, float homingVelocity, float N)
		{
			Vector2 center = projectile.Center;
			float maxDistance = distanceRequired;
			bool homeIn = false;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].CanBeChasedBy(projectile, false))
				{
					float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

					bool canHit = true;
					if (extraDistance < maxDistance && !ignoreTiles)
						canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

					if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
					{
						center = Main.npc[i].Center;
						homeIn = true;
						break;
					}
				}
			}

            if (!projectile.friendly)
            {
                homeIn = false;
            }

			if (homeIn)
			{
				Vector2 homeInVector = projectile.DirectionTo(center);
				if (homeInVector.HasNaNs())
					homeInVector = Vector2.UnitY;

				projectile.velocity = (projectile.velocity * N + homeInVector * homingVelocity) / (N + 1f);
			}
		}

		public static void MagnetSphereHitscan(Projectile projectile, float distanceRequired, float homingVelocity, float projectileTimer, int maxTargets, int spawnedProjectile, double damageMult = 1D, bool attackMultiple = false)
		{
			float maxDistance = distanceRequired;
			bool homeIn = false;
			int[] targetArray = new int[maxTargets];
			int targetArrayIndex = 0;

			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].CanBeChasedBy(projectile, false))
				{
					float extraDistance = (Main.npc[i].width / 2) + (Main.npc[i].height / 2);

					bool canHit = true;
					if (extraDistance < maxDistance)
						canHit = Collision.CanHit(projectile.Center, 1, 1, Main.npc[i].Center, 1, 1);

					if (Vector2.Distance(Main.npc[i].Center, projectile.Center) < (maxDistance + extraDistance) && canHit)
					{
						if (targetArrayIndex < maxTargets)
						{
							targetArray[targetArrayIndex] = i;
							targetArrayIndex++;
							homeIn = true;
						}
						else
							break;
					}
				}
			}

			if (homeIn)
			{
				int randomTarget = Main.rand.Next(targetArrayIndex);
				randomTarget = targetArray[randomTarget];

				projectile.localAI[0] += 1f;
				if (projectile.localAI[0] > projectileTimer)
				{
					projectile.localAI[0] = 0f;
					Vector2 value = projectile.Center + projectile.velocity * 4f;
					Vector2 velocity = Vector2.Normalize(Main.npc[randomTarget].Center - value) * homingVelocity;

					if (attackMultiple)
					{
						for (int i = 0; i < targetArrayIndex; i++)
						{
							velocity = Vector2.Normalize(Main.npc[targetArray[i]].Center - value) * homingVelocity;

							if (projectile.owner == Main.myPlayer)
							{
								int projectile2 = Projectile.NewProjectile(value.X, value.Y, velocity.X, velocity.Y, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

								if (projectile.type == ProjectileType<EradicatorProjectile>() && projectile.Calamity().rogue)
									Main.projectile[projectile2].Calamity().forceRogue = true;
							}
						}

						return;
					}

					if (projectile.type == ProjectileType<GodsGambitYoyo>())
					{
						velocity.Y += Main.rand.Next(-30, 31) * 0.05f;
						velocity.X += Main.rand.Next(-30, 31) * 0.05f;
					}

					if (projectile.owner == Main.myPlayer)
					{
						int projectile2 = Projectile.NewProjectile(value.X, value.Y, velocity.X, velocity.Y, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

						if (projectile.type == ProjectileType<CnidarianYoyo>() || projectile.type == ProjectileType<GodsGambitYoyo>() ||
							projectile.type == ProjectileType<ShimmersparkYoyo>() || projectile.type == ProjectileType<VerdantYoyo>())
							Main.projectile[projectile2].Calamity().forceMelee = true;
					}
				}
			}
		}

		public static void ExpandHitboxBy(Projectile projectile, int width, int height)
		{
			projectile.position = projectile.Center;
			projectile.width = width;
			projectile.height = height;
			projectile.position -= projectile.Size * 0.5f;
		}
		public static void ExpandHitboxBy(Projectile projectile, int newSize)
		{
			ExpandHitboxBy(projectile, newSize, newSize);
		}
		public static void ExpandHitboxBy(Projectile projectile, float expandRatio)
		{
			ExpandHitboxBy(projectile, (int)(projectile.width * expandRatio), (int)(projectile.height * expandRatio));
		}
		#endregion
	}
}
