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
using CalamityMod.Projectiles.Rogue;
using CalamityMod.Projectiles.Summon;
using CalamityMod.Projectiles.Typeless;
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
        public bool forceRogue = false;
        public bool forceMinion = false;
        public bool forceHostile = false;
        public bool forceTypeless = false;

        // Damage Adjusters
        private bool setDamageValues = true;
        public float spawnedPlayerMinionDamageValue = 1f;
        public int spawnedPlayerMinionProjectileDamageValue = 0;
        public int defDamage = 0;

        // Rogue Stuff
        public bool stealthStrike = false; //Update all existing rogue weapons with this
        public bool momentumCapacitatorBoost = false; //Constant acceleration

        // Iron Heart
        public int ironHeartDamage = 0;

        // Counters and Timers
        private int counter = 0;

        public int lineColor = 0; //Note: Although this was intended for fishing line colors, I use this as an AI variable a lot because vanilla only has 4 that sometimes are already in use.  ~Ben
        public bool extorterBoost = false;

        public bool overridesMinionDamagePrevention = false;

        #region SetDefaults
        public override void SetDefaults(Projectile projectile)
        {
            switch (projectile.type)
            {
                case ProjectileID.Spear:
                case ProjectileID.Trident:
                case ProjectileID.TheRottedFork:
                case ProjectileID.Swordfish:
                case ProjectileID.Arkhalis:
                case ProjectileID.DarkLance:
                case ProjectileID.CobaltNaginata:
                case ProjectileID.PalladiumPike:
                case ProjectileID.MythrilHalberd:
                case ProjectileID.OrichalcumHalberd:
                case ProjectileID.AdamantiteGlaive:
                case ProjectileID.TitaniumTrident:
                case ProjectileID.MushroomSpear:
                case ProjectileID.Gungnir:
                case ProjectileID.ObsidianSwordfish:
                case ProjectileID.ChlorophytePartisan:
                case ProjectileID.MonkStaffT1:
                case ProjectileID.MonkStaffT2:
                case ProjectileID.MonkStaffT3:
                case ProjectileID.NorthPoleWeapon:

				//tools
                case ProjectileID.CobaltDrill:
                case ProjectileID.MythrilDrill:
                case ProjectileID.AdamantiteDrill:
                case ProjectileID.PalladiumDrill:
                case ProjectileID.OrichalcumDrill:
                case ProjectileID.TitaniumDrill:
                case ProjectileID.ChlorophyteDrill:
                case ProjectileID.CobaltChainsaw:
                case ProjectileID.MythrilChainsaw:
                case ProjectileID.AdamantiteChainsaw:
                case ProjectileID.PalladiumChainsaw:
                case ProjectileID.OrichalcumChainsaw:
                case ProjectileID.TitaniumChainsaw:
                case ProjectileID.ChlorophyteChainsaw:
                case ProjectileID.VortexDrill:
                case ProjectileID.VortexChainsaw:
                case ProjectileID.NebulaDrill:
                case ProjectileID.NebulaChainsaw:
                case ProjectileID.SolarFlareDrill:
                case ProjectileID.SolarFlareChainsaw:
                case ProjectileID.StardustDrill:
                case ProjectileID.StardustChainsaw:
                case ProjectileID.Hamdrax:
                case ProjectileID.ChlorophyteJackhammer:
                case ProjectileID.SawtoothShark:
                case ProjectileID.ButchersChainsaw:
                    trueMelee = true;
                    break;

                case ProjectileID.StarWrath:
                    projectile.penetrate = projectile.maxPenetrate = 1;
                    break;

                case ProjectileID.Spazmamini:
                case ProjectileID.Retanimini:
                case ProjectileID.MiniRetinaLaser:
                    projectile.localNPCHitCooldown = 10;
                    projectile.usesLocalNPCImmunity = true;
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
                if (projectile.type == ProjectileID.PoisonSeedPlantera)
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
							Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 104, 1f, 0f);
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

            if (forceMelee)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = true;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
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
                rogue = true;
            }
            else if (forceMinion)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = true;
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
                rogue = false;
            }
            else if (forceTypeless)
            {
                projectile.hostile = false;
                projectile.friendly = true;
                projectile.melee = false;
                projectile.ranged = false;
                projectile.magic = false;
                projectile.minion = false;
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
							for (int num252 = 0; num252 < 2; num252++)
							{
								Vector2 value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
								while (value15.X == 0f && value15.Y == 0f)
								{
									value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
								}
								value15.Normalize();
								value15 *= Main.rand.Next(70, 101) * 0.1f;
								int shard = Projectile.NewProjectile(projectile.oldPosition.X + (projectile.width / 2), projectile.oldPosition.Y + (projectile.height / 2), value15.X, value15.Y, 90, (int)(projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
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
					(projectile.melee || projectile.ranged || projectile.magic || rogue))
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
        #endregion

		#region ModifyHitNPC
		public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

            if (projectile.owner == Main.myPlayer && !projectile.npcProj && !projectile.trap)
            {
				if (rogue && stealthStrike && modPlayer.stealthStrikeAlwaysCrits)
					crit = true;
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
				if (projectile.type == (ProjectileID.StardustDragon1 | ProjectileID.StardustDragon2 | ProjectileID.StardustDragon3 | ProjectileID.StardustDragon4))
				{
					target.immune[projectile.owner] = 10;
				}

				//flask of party affects all types of weapons, !projectile.melee is to prevent double flask effects
                if (!projectile.melee && player.meleeEnchant == 7)
					Projectile.NewProjectile(target.Center.X, target.Center.Y, target.velocity.X, target.velocity.Y, ProjectileID.ConfettiMelee, 0, 0f, projectile.owner, 0f, 0f);

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
						projectile.ghostHurt(damage, new Vector2(target.Center.X, target.Center.Y));
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
						int nebulaBooster = Item.NewItem((int)target.position.X, (int)target.position.Y, target.width, target.height, boosterType, 1, false, 0, false, false);
						Main.item[nebulaBooster].velocity.Y = Main.rand.Next(-20, 1) * 0.2f;
						Main.item[nebulaBooster].velocity.X = Main.rand.Next(10, 31) * 0.2f * projectile.direction;
						if (Main.netMode == 1)
						{
							NetMessage.SendData(21, -1, -1, null, nebulaBooster, 0f, 0f, 0f, 0, 0, 0);
						}
					}
				}

				if (Main.player[Main.myPlayer].lifeSteal > 0f && target.canGhostHeal && target.type != NPCID.TargetDummy && target.type != NPCType<SuperDummyNPC>() && !player.moonLeech)
				{
					// Increases the degree to which Spectre Healing set contributes to the lifesteal cap
					if (player.ghostHeal)
					{
						float num = 0.1f;
						num -= projectile.numHits * 0.025f;
						if (num < 0f)
							num = 0f;

						float num2 = damage * num;
						Main.player[Main.myPlayer].lifeSteal -= num2;
					}

					// Increases the degree to which Vampire Knives contribute to the lifesteal cap
					if (projectile.type == ProjectileID.VampireKnife)
					{
						float num = damage * 0.0375f;
						if (num < 0f)
							num = 0f;

						Main.player[Main.myPlayer].lifeSteal -= num;
					}

					if (modPlayer.vampiricTalisman && rogue && crit)
					{
						float heal = MathHelper.Clamp(damage * 0.015f, 0f, 6f);
						Main.player[Main.myPlayer].lifeSteal -= heal * 2f;
						Projectile.NewProjectile(target.position.X, target.position.Y, 0f, 0f, ProjectileID.VampireHeal, 0, 0f, projectile.owner, projectile.owner, heal);
					}

					if ((modPlayer.bloodyGlove || modPlayer.electricianGlove) && rogue && stealthStrike)
					{
						player.statLife += 1;
						player.HealEffect(1);
					}

					if (modPlayer.auricSet)
					{
						float healMult = 0.05f;
						healMult -= projectile.numHits * 0.025f;
						float heal = projectile.damage * healMult;

						if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
							goto OTHEREFFECTS;

						SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<AuricOrb>(), 1200f, 1.5f);
					}
					else if (modPlayer.silvaSet)
					{
						float healMult = 0.03f;
						healMult -= projectile.numHits * 0.015f;
						float heal = projectile.damage * healMult;

						if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
							goto OTHEREFFECTS;

						SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<SilvaOrb>(), 1200f, 1.5f);
					}
					else if (projectile.magic)
					{
						if (modPlayer.godSlayerMage)
						{
							float healMult = 0.06f;
							healMult -= projectile.numHits * 0.015f;
							float heal = projectile.damage * healMult;

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<GodSlayerHealOrb>(), 1200f, 1.5f);
						}
						else if (modPlayer.tarraMage)
						{
							if (modPlayer.tarraMageHealCooldown <= 0)
							{
								modPlayer.tarraMageHealCooldown = 90;

								float healMult = 0.1f;
								healMult -= projectile.numHits * 0.05f;
								float heal = projectile.damage * healMult;

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

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<AtaxiaHealOrb>(), 1200f, 1.5f);
						}
						else if (modPlayer.manaOverloader)
						{
							float healMult = 0.2f;
							healMult -= projectile.numHits * 0.05f;
							float heal = projectile.damage * healMult * (player.statMana / (float)player.statManaMax2);

							if (!CanSpawnLifeStealProjectile(projectile, healMult, heal))
								goto OTHEREFFECTS;

							SpawnLifeStealProjectile(projectile, player, heal, ProjectileType<ManaOverloaderHealOrb>(), 1200f, 1.5f);
						}
					}
				}

				OTHEREFFECTS:

                if (modPlayer.alchFlask &&
                    (projectile.magic || rogue || projectile.melee || projectile.minion || projectile.ranged || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type) || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type]) &&
                    player.ownedProjectileCounts[ProjectileType<PlagueSeeker>()] < 6)
                {
                    int plague = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileType<PlagueSeeker>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.25, 30), 0f, projectile.owner, 0f, 0f);
                    Main.projectile[plague].Calamity().forceTypeless = false;
                }

                if (modPlayer.reaverBlast && projectile.melee)
                {
                    Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileType<ReaverBlast>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.2, 30), 0f, projectile.owner, 0f, 0f);
                }

                if (projectile.magic)
                {
                    if (modPlayer.silvaMage && projectile.penetrate == 1 && Main.rand.Next(0, 100) >= 97)
                    {
                        Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 103);
                        projectile.position = projectile.Center;
                        projectile.width = projectile.height = 96;
                        projectile.position.X -= projectile.width / 2;
                        projectile.position.Y -= projectile.height / 2;
                        for (int num193 = 0; num193 < 3; num193++)
                        {
                            Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
                        }
                        for (int num194 = 0; num194 < 30; num194++)
                        {
                            int num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 0, new Color(Main.DiscoR, 203, 103), 2.5f);
                            Main.dust[num195].noGravity = true;
                            Main.dust[num195].velocity *= 3f;
                            num195 = Dust.NewDust(new Vector2(projectile.position.X, projectile.position.Y), projectile.width, projectile.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 1.5f);
                            Main.dust[num195].velocity *= 2f;
                            Main.dust[num195].noGravity = true;
                        }
                        projectile.damage *= modPlayer.auricSet ? 7 : 4;
						projectile.localNPCHitCooldown = 10;
						projectile.usesLocalNPCImmunity = true;
                        projectile.Damage();
                    }

					if (modPlayer.reaverBurst)
					{
						int num251 = Main.rand.Next(2, 5);
						for (int num252 = 0; num252 < num251; num252++)
						{
							Vector2 value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							while (value15.X == 0f && value15.Y == 0f)
							{
								value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
							}
							value15.Normalize();
							value15 *= Main.rand.Next(70, 101) * 0.1f;
							int proj = Projectile.NewProjectile(projectile.oldPosition.X + (projectile.width / 2), projectile.oldPosition.Y + (projectile.height / 2), value15.X, value15.Y, 569 + Main.rand.Next(3), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 15), 0f, projectile.owner, 0f, 0f);
							Main.projectile[proj].usesLocalNPCImmunity = true;
							Main.projectile[proj].localNPCHitCooldown = 30;
						}
					}
					else if (modPlayer.ataxiaMage && modPlayer.ataxiaDmg <= 0)
					{
						SpawnOrb(projectile, 1.25f, ProjectileType<AtaxiaOrb>(), 800f, 20f);
						int num = (int)(projectile.damage * 0.5f);
						modPlayer.ataxiaDmg += num;
					}
					else if (modPlayer.godSlayerMage && modPlayer.godSlayerDmg <= 0)
					{
						SpawnOrb(projectile, modPlayer.auricSet ? 2f : 1.5f, ProjectileType<GodSlayerOrb>(), 800f, 20f);
						int num = (int)(projectile.damage * 0.5f);
						modPlayer.godSlayerDmg += num;
					}
                }
                if (projectile.melee)
                {
                    if (modPlayer.ataxiaGeyser && player.ownedProjectileCounts[ProjectileType<ChaosGeyser>()] < 3)
                    {
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileType<ChaosGeyser>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 35), 0f, projectile.owner, 0f, 0f);
                    }
                }
                if (rogue)
                {
                    if (modPlayer.xerocSet && modPlayer.xerocDmg <= 0 && player.ownedProjectileCounts[ProjectileType<XerocFire>()] < 3 && player.ownedProjectileCounts[ProjectileType<XerocBlast>()] < 3)
                    {
						switch (Main.rand.Next(5))
						{
							case 0:

								SpawnOrb(projectile, 1.6f, ProjectileType<XerocStar>(), 800f, Main.rand.Next(15, 30));
								int num = (int)(projectile.damage * 0.5f);
								modPlayer.xerocDmg += num;

								break;

							case 1:

								SpawnOrb(projectile, 1.25f, ProjectileType<XerocOrb>(), 800f, 30f);
								int num2 = (int)(projectile.damage * 0.5f);
								modPlayer.xerocDmg += num2;

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

								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileType<XerocFire>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.15, 40), 0f, projectile.owner, 0f, 0f);

								break;

							case 3:

								Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileType<XerocBlast>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.2, 40), 0f, projectile.owner, 0f, 0f);

								break;

							case 4:

								SpawnOrb(projectile, 1.2f, ProjectileType<XerocBubble>(), 800f, 15f);
								int num3 = (int)(projectile.damage * 0.5f);
								modPlayer.xerocDmg += num3;

								break;

							default:
								break;
						}
					}

					SKIPXEROC:

                    if (modPlayer.featherCrown && stealthStrike && modPlayer.featherCrownCooldown <= 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 pos = new Vector2(target.Center.X + target.width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            float speedX = (target.Center.X - pos.X) / 30f;
                            float speedY = (target.Center.Y - pos.Y) * 8;
                            int dmg = (int)(15 + (projectile.damage * 0.05f));
                            int feather = Projectile.NewProjectile(pos.X, pos.Y, speedX, speedY, ProjectileType<StickyFeather>(), dmg, 3, projectile.owner, 0f, Main.rand.Next(15));
                            Main.projectile[feather].Calamity().forceRogue = true;
                            modPlayer.featherCrownCooldown = 15;
                        }
                    }

                    if (modPlayer.moonCrown && stealthStrike && modPlayer.moonCrownCooldown <= 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 pos = new Vector2(target.Center.X + target.width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            Vector2 velocity = (target.Center - pos) / 10f;
                            float AI1 = Main.rand.Next(3);
                            int dmg = (int)(150 + (projectile.damage * 0.05f));
                            int flare = Projectile.NewProjectile(pos, velocity, ProjectileID.LunarFlare, dmg, 3, projectile.owner, 0f, AI1);
                            Main.projectile[flare].Calamity().forceRogue = true;
                            modPlayer.moonCrownCooldown = 15;
                        }
                    }

                    if (modPlayer.nanotech && stealthStrike && modPlayer.nanoFlareCooldown <= 0)
                    {
                        for (int i = 0; i < 6; i++)
                        {
                            Vector2 pos = new Vector2(target.Center.X + target.width * 0.5f + Main.rand.Next(-201, 201), Main.screenPosition.Y - 600f - Main.rand.Next(50));
                            Vector2 velocity = (target.Center - pos) / 40f;
                            int dmg = (int)(1000 + (projectile.damage * 0.05f));
                            int flare = Projectile.NewProjectile(pos, velocity, ProjectileType<NanoFlare>(),dmg, 3f, projectile.owner, 0f, 0f);
                            Main.projectile[flare].Calamity().rogue = true;
                            modPlayer.nanoFlareCooldown = 15;
                        }
                    }

					if (modPlayer.forbiddenCirclet && stealthStrike && modPlayer.forbiddenCooldown <= 0)
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

					if (modPlayer.corrosiveSpine && projectile.type != ProjectileType<Corrocloud1>() && projectile.type != ProjectileType<Corrocloud2>() && projectile.type != ProjectileType<Corrocloud3>())
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

					if (modPlayer.shadow && modPlayer.shadowPotCooldown <= 0)
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
							int scythe = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<CosmicScythe>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.05, 60), 3f, projectile.owner, 0f, 0f);
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
							int[] numArray1 = new int[Main.maxNPCs];
							int maxValue1 = 0;
							int maxValue2 = 0;
							for (int index = 0; index < Main.maxNPCs; ++index)
							{
								if (Main.npc[index].CanBeChasedBy((object)projectile, false))
								{
									float num2 = Math.Abs(Main.npc[index].Center.X - projectile.Center.X) + Math.Abs(Main.npc[index].Center.Y - projectile.Center.Y);
									if (num2 < 800f)
									{
										if (Collision.CanHit(projectile.position, 1, 1, Main.npc[index].position, Main.npc[index].width, Main.npc[index].height) && num2 > 50f)
										{
											numArray1[maxValue2] = index;
											++maxValue2;
										}
										else if (maxValue2 == 0)
										{
											numArray1[maxValue1] = index;
											++maxValue1;
										}
									}
								}
							}
							if (maxValue1 == 0 && maxValue2 == 0)
								return;
							int num3 = maxValue2 <= 0 ? numArray1[Main.rand.Next(maxValue1)] : numArray1[Main.rand.Next(maxValue2)];
							double num4 = 4.0;
							float num5 = (float)Main.rand.Next(-100, 101);
							float num6 = (float)Main.rand.Next(-100, 101);
							double num7 = Math.Sqrt((double)num5 * (double)num5 + (double)num6 * (double)num6);
							float num8 = (float)(num4 / num7);
							float SpeedX = num5 * num8;
							float SpeedY = num6 * num8;
							int ghost = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, SpeedX, SpeedY, ProjectileID.SpectreWrath, spiritDamage, 0f, projectile.owner, (float)num3, 0f);
							Main.projectile[ghost].Calamity().forceRogue = true;
							Main.projectile[ghost].penetrate = 1;
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
                if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type) || ProjectileID.Sets.MinionShot[projectile.type] || ProjectileID.Sets.SentryShot[projectile.type])
                {
                    if (modPlayer.profanedCrystalBuffs || (modPlayer.pArtifact && !modPlayer.profanedCrystal))
                    {
                        target.AddBuff(BuffType<HolyFlames>(), modPlayer.profanedCrystalBuffs ? 600 : 300);
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
						SpawnOrb(projectile, 2f, ProjectileType<GodSlayerPhantom>(), 800f, 15f, true);
						int num = (int)(projectile.damage * 0.5f);
						modPlayer.godSlayerDmg += num;
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
								SpawnOrb(projectile, 1.05f, ProjectileType<EnergyOrb>(), 800f, 15f);
								int num = (int)(projectile.damage * 0.5f);
								modPlayer.jellyDmg += num;
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
                        int num251 = Main.rand.Next(2, 4);
                        for (int num252 = 0; num252 < num251; num252++)
                        {
                            Vector2 value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            }
                            value15.Normalize();
                            value15 *= Main.rand.Next(70, 101) * 0.1f;
                            Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ProjectileType<TarraEnergy>(), CalamityUtils.DamageSoftCap(projectile.damage * 0.33, 65), 0f, projectile.owner, 0f, 0f);
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
            if (Main.player[Main.myPlayer].Calamity().omniscience && projectile.hostile)
                return Color.Coral;

            if (Main.player[Main.myPlayer].Calamity().trippy)
                return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);

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
            if (texture == null)
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

			if (CalamityMod.CalamityConfig.Afterimages)
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
                    Main.PlaySound(2, (int)projectile.position.X, (int)projectile.position.Y, 20);
                    for (int dustIndex = 0; dustIndex < 3; dustIndex++)
                    {
                        int dust = Dust.NewDust(projectile.position + projectile.velocity, projectile.width, projectile.height, (int)CalamityDusts.ProfanedFire, projectile.oldVelocity.X * 0.5f, projectile.oldVelocity.Y * 0.5f, 0, default, 1f);
                        Main.dust[dust].noGravity = true;
                    }
                }

                if (rogue)
                {
                    if (modPlayer.etherealExtorter && Main.rand.Next(0, 100) >= 95)
                    {
                        for (int num252 = 0; num252 < 3; num252++)
                        {
                            Vector2 value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            while (value15.X == 0f && value15.Y == 0f)
                            {
                                value15 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            }
                            value15.Normalize();
                            value15 *= Main.rand.Next(70, 101) * 0.1f;
                            int soul = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, value15.X, value15.Y, ModContent.ProjectileType<LostSoulFriendly>(), (int)(projectile.damage * 0.33), 0f, projectile.owner, 0f, 0f);
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
			float num13 = 0f;
			int num14 = projectile.owner;
			for (int i = 0; i < Main.maxPlayers; i++)
			{
				if (Main.player[i].active && !Main.player[i].dead && ((!player.hostile && !Main.player[i].hostile) || player.team == Main.player[i].team))
				{
					float num15 = Math.Abs(Main.player[i].position.X + (Main.player[i].width / 2) - projectile.position.X + (projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (Main.player[i].height / 2) - projectile.position.Y + (projectile.height / 2));
					if (num15 < distanceRequired && (Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
					{
						num13 = Main.player[i].statLifeMax2 - Main.player[i].statLife;
						num14 = i;
					}
				}
			}
			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, healProjectileType, 0, 0f, projectile.owner, num14, healAmount);
		}
		#endregion

		#region AI Shortcuts
		public void SpawnOrb(Projectile projectile, float dmgMult, int projType, float distanceRequired, float N, bool gsPhantom = false)
        {
			Player player = Main.player[projectile.owner];
			CalamityPlayer modPlayer = player.Calamity();

			int num = (int)(projectile.damage * 0.5f);
            float ai1 = Main.rand.NextFloat() + 0.5f;
			int[] array = new int[Main.maxNPCs];
			int num3 = 0;
			int num4 = 0;
			for (int i = 0; i < Main.maxNPCs; i++)
			{
				if (Main.npc[i].CanBeChasedBy(projectile, false))
				{
					float num5 = Math.Abs(Main.npc[i].position.X + (Main.npc[i].width / 2) - projectile.position.X + (projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (Main.npc[i].height / 2) - projectile.position.Y + (projectile.height / 2));
					if (num5 < distanceRequired)
					{
						if (Collision.CanHit(projectile.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num5 > 50f)
						{
							array[num4] = i;
							num4++;
						}
						else if (num4 == 0)
						{
							array[num3] = i;
							num3++;
						}
					}
				}
			}
			if (num3 == 0 && num4 == 0)
			{
				return;
			}
			int num6;
			if (num4 > 0)
			{
				num6 = array[Main.rand.Next(num4)];
			}
			else
			{
				num6 = array[Main.rand.Next(num3)];
			}
			float num7 = N;
			float num8 = Main.rand.Next(-100, 101);
			float num9 = Main.rand.Next(-100, 101);
			float num10 = (float)Math.Sqrt(num8 * num8 + num9 * num9);
			num10 = num7 / num10;
			num8 *= num10;
			num9 *= num10;
			Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, projType, (int)(num * dmgMult), 0f, projectile.owner, gsPhantom ? 0f : num6, gsPhantom ? ai1 : 0f);
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

			if (homeIn)
			{
				Vector2 homeInVector = projectile.DirectionTo(center);
				if (homeInVector.HasNaNs())
					homeInVector = Vector2.UnitY;

				projectile.velocity = (projectile.velocity * N + homeInVector * homingVelocity) / (N + 1f);
			}
		}

		public static void MagnetSphereHitscan(Projectile projectile, float distanceRequired, float homingVelocity, float projectileTimer, int maxTargets, int spawnedProjectile, double damageMult = 1D)
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

					if (projectile.type == ProjectileType<GodsGambitYoyo>())
					{
						velocity.Y += Main.rand.Next(-30, 31) * 0.05f;
						velocity.X += Main.rand.Next(-30, 31) * 0.05f;
					}

					if (projectile.owner == Main.myPlayer)
					{
						int projectile2 = Projectile.NewProjectile(value.X, value.Y, velocity.X, velocity.Y, spawnedProjectile, (int)(projectile.damage * damageMult), projectile.knockBack, projectile.owner, 0f, 0f);

						if (projectile.type == ProjectileType<CnidarianYoyo>() || projectile.type == ProjectileType<GodsGambitYoyo>() ||
							projectile.type == ProjectileType<ShimmersparkYoyo>() || projectile.type == ProjectileType<VerdantYoyo>() || (projectile.type == ProjectileType<EradicatorProjectile>() && projectile.melee))
							Main.projectile[projectile2].Calamity().forceMelee = true;

						if (projectile.type == ProjectileType<EradicatorProjectile>() && projectile.Calamity().rogue)
							Main.projectile[projectile2].Calamity().forceRogue = true;
					}
				}
			}
		}

		public static void HealingProjectile(Projectile projectile, int healing, int playerToHeal, float homingVelocity, float N, bool autoHomes = true, int timeCheck = 120)
		{
			int target = playerToHeal;
			Player player = Main.player[target];
			float homingSpeed = homingVelocity;
			if (player.lifeMagnet)
				homingSpeed *= 1.5f;

			Vector2 projPos = new Vector2(projectile.Center.X, projectile.Center.Y);
			float xDist = player.Center.X - projPos.X;
			float yDist = player.Center.Y - projPos.Y;
			Vector2 playerVector = new Vector2(xDist, yDist);
			float playerDist = playerVector.Length();
			if (playerDist < 50f && projectile.position.X < player.position.X + player.width && projectile.position.X + projectile.width > player.position.X && projectile.position.Y < player.position.Y + player.height && projectile.position.Y + projectile.height > player.position.Y)
			{
				if (projectile.owner == Main.myPlayer && !Main.player[Main.myPlayer].moonLeech)
				{
					int healAmt = healing;
					player.HealEffect(healAmt, false);
					player.statLife += healAmt;
					if (player.statLife > player.statLifeMax2)
					{
						player.statLife = player.statLifeMax2;
					}
					NetMessage.SendData(66, -1, -1, null, target, healAmt, 0f, 0f, 0, 0, 0);
				}
				projectile.Kill();
			}
			if (autoHomes)
			{
				playerDist = homingSpeed / playerDist;
				playerVector.X *= playerDist;
				playerVector.Y *= playerDist;
				projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
				projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
			}
			else if (player.lifeMagnet && projectile.timeLeft < timeCheck)
			{
				playerDist = homingVelocity / playerDist;
				playerVector.X *= playerDist;
				playerVector.Y *= playerDist;
				projectile.velocity.X = (projectile.velocity.X * N + playerVector.X) / (N + 1f);
				projectile.velocity.Y = (projectile.velocity.Y * N + playerVector.Y) / (N + 1f);
			}
		}

		public static void ChargingMinionAI(Projectile projectile, float range, float maxPlayerDist, float extraMaxPlayerDist, float safeDist, int initialUpdates, float chargeDelayTime, float goToSpeed, float goBackSpeed, float chargeCounterMax, float chargeSpeed, bool tileVision, bool ignoreTilesWhenCharging)
		{
            Player player = Main.player[projectile.owner];
            CalamityPlayer modPlayer = player.Calamity();

			//Anti sticky movement to prevent stacking
            float antiStickMvt = 0.05f;
            for (int projIndex = 0; projIndex < Main.maxProjectiles; projIndex++)
            {
				Projectile proj = Main.projectile[projIndex];
                bool typeCheck = proj.type == projectile.type;
                if (projIndex != projectile.whoAmI && proj.active && proj.owner == projectile.owner && typeCheck && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < projectile.width)
                {
                    if (projectile.position.X < proj.position.X)
                    {
                        projectile.velocity.X -= antiStickMvt;
                    }
                    else
                    {
                        projectile.velocity.X += antiStickMvt;
                    }
                    if (projectile.position.Y < proj.position.Y)
                    {
                        projectile.velocity.Y -= antiStickMvt;
                    }
                    else
                    {
                        projectile.velocity.Y += antiStickMvt;
                    }
                }
            }

			//Breather time between charges as like a reset
            bool chargeDelay = false;
            if (projectile.ai[0] == 2f)
            {
                projectile.ai[1] += 1f;
                projectile.extraUpdates = initialUpdates + (projectile.type == ModContent.ProjectileType<CloudElementalMinion>() ? 2 : 1);
                if (projectile.ai[1] > chargeDelayTime)
                {
                    projectile.ai[1] = 1f;
                    projectile.ai[0] = 0f;
                    projectile.extraUpdates = initialUpdates;
                    projectile.numUpdates = 0;
                    projectile.netUpdate = true;
                }
                else
                {
                    chargeDelay = true;
                }
            }
            if (chargeDelay)
            {
                return;
            }

			//Find a target
            float maxDist = range;
            Vector2 targetVec = projectile.position;
            bool foundTarget = false;
            Vector2 half = new Vector2(0.5f);
			//Prioritize the targeted enemy if possible
            if (player.HasMinionAttackTargetNPC)
            {
                NPC npc = Main.npc[player.MinionAttackTargetNPC];
                if (npc.CanBeChasedBy(projectile, false))
                {
					//Check the size of the target to make it easier to hit fat targets like Levi
                    Vector2 sizeCheck = npc.position + npc.Size * half;
                    float targetDist = Vector2.Distance(npc.Center, projectile.Center);
					//Some minions will ignore tiles when choosing a target like Ice Claspers, others will not
					bool canHit = true;
					if (!tileVision)
						canHit = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                    if (!foundTarget && targetDist < maxDist && canHit)
                    {
                        maxDist = targetDist;
                        targetVec = sizeCheck;
                        foundTarget = true;
                    }
                }
            }
			//If no npc is specifically targetted, check through the entire array
            else
            {
                for (int npcIndex = 0; npcIndex < Main.maxNPCs; npcIndex++)
                {
                    NPC npc = Main.npc[npcIndex];
                    if (npc.CanBeChasedBy(projectile, false))
                    {
						Vector2 sizeCheck = npc.position + npc.Size * half;
                        float targetDist = Vector2.Distance(npc.Center, projectile.Center);
						bool canHit = true;
						if (!tileVision)
							canHit = Collision.CanHitLine(projectile.position, projectile.width, projectile.height, npc.position, npc.width, npc.height);
                        if (!foundTarget && targetDist < maxDist && canHit)
                        {
                            maxDist = targetDist;
                            targetVec = sizeCheck;
                            foundTarget = true;
                        }
                    }
                }
            }

			//If the player is too far, return to the player. Range is increased while attacking something.
            float distBeforeForcedReturn = maxPlayerDist;
            if (foundTarget)
            {
                distBeforeForcedReturn = extraMaxPlayerDist;
            }
            if (Vector2.Distance(player.Center, projectile.Center) > distBeforeForcedReturn)
            {
                projectile.ai[0] = 1f;
                projectile.netUpdate = true;
            }

			//Go to the target if you found one
            if (foundTarget && projectile.ai[0] == 0f)
            {
				//Some minions don't ignore tiles while charging like brittle stars
				projectile.tileCollide = !ignoreTilesWhenCharging;
                Vector2 targetSpot = targetVec - projectile.Center;
                float targetDist = targetSpot.Length();
                targetSpot.Normalize();
				//Tries to get the minion in the sweet spot of 200 pixels away but the minion also charges so idk what good it does
                if (targetDist > 200f)
                {
                    float speed = goToSpeed; //8
                    targetSpot *= speed;
                    projectile.velocity = (projectile.velocity * 40f + targetSpot) / 41f;
                }
                else
                {
                    float speed = -goBackSpeed; //-4
                    targetSpot *= speed;
                    projectile.velocity = (projectile.velocity * 40f + targetSpot) / 41f; //41
                }
            }

			//Movement for idle or returning to the player
            else
            {
				//Ignore tiles so they don't get stuck everywhere like Optic Staff
				projectile.tileCollide = false;

                bool returningToPlayer = false;
                if (!returningToPlayer)
                {
                    returningToPlayer = projectile.ai[0] == 1f;
                }

				//Player distance calculations
                Vector2 playerVec = player.Center - projectile.Center + new Vector2(0f, -60f);
                float playerDist = playerVec.Length();

				//If the minion is actively returning, move faster
                float playerHomeSpeed = 6f;
                if (returningToPlayer)
                {
                    playerHomeSpeed = 15f;
                }
				//Move somewhat faster if the player is kinda far~ish
                if (playerDist > 200f && playerHomeSpeed < 8f)
                {
                    playerHomeSpeed = 8f;
                }
				//Return to normal if close enough to the player
                if (playerDist < safeDist && returningToPlayer && !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
                {
                    projectile.ai[0] = 0f;
                    projectile.netUpdate = true;
                }
				//Teleport to the player if abnormally far
                if (playerDist > 2000f)
                {
                    projectile.position.X = player.Center.X - (float)(projectile.width / 2);
                    projectile.position.Y = player.Center.Y - (float)(projectile.height / 2);
                    projectile.netUpdate = true;
                }
				//If more than 70 pixels away, move toward the player
                if (playerDist > 70f)
                {
                    playerVec.Normalize();
                    playerVec *= playerHomeSpeed;
                    projectile.velocity = (projectile.velocity * 40f + playerVec) / 41f;
                }
				//Minions never stay still
                else if (projectile.velocity.X == 0f && projectile.velocity.Y == 0f)
                {
                    projectile.velocity.X = -0.15f;
                    projectile.velocity.Y = -0.05f;
                }
            }

			//Increment attack counter randomly
            if (projectile.ai[1] > 0f)
            {
                projectile.ai[1] += (float)Main.rand.Next(1, 4);
            }
			//If high enough, prepare to attack
            if (projectile.ai[1] > chargeCounterMax)
            {
                projectile.ai[1] = 0f;
                projectile.netUpdate = true;
            }

			//Charge at an enemy if not on cooldown
            if (projectile.ai[0] == 0f)
            {
                if (projectile.ai[1] == 0f && foundTarget && maxDist < 500f)
                {
                    projectile.ai[1] += 1f;
                    if (Main.myPlayer == projectile.owner)
                    {
                        projectile.ai[0] = 2f;
                        Vector2 targetPos = targetVec - projectile.Center;
                        targetPos.Normalize();
                        projectile.velocity = targetPos * chargeSpeed; //8
                        projectile.netUpdate = true;
                    }
                }
            }
        }

		public static void FloatingPetAI(Projectile projectile, bool faceRight, float tiltFloat, bool lightPet = false)
		{
			Player player = Main.player[projectile.owner];

			//anti sticking movement as a failsafe
            float SAImovement = 0.05f;
            for (int index = 0; index < Main.projectile.Length; index++)
            {
				Projectile proj = Main.projectile[index];
                bool flag23 = Main.projPet[proj.type];
                if (index != projectile.whoAmI && proj.active && proj.owner == projectile.owner && flag23 && Math.Abs(projectile.position.X - proj.position.X) + Math.Abs(projectile.position.Y - proj.position.Y) < projectile.width)
                {
                    if (projectile.position.X < proj.position.X)
                    {
                        projectile.velocity.X -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.X += SAImovement;
                    }
                    if (projectile.position.Y < proj.position.Y)
                    {
                        projectile.velocity.Y -= SAImovement;
                    }
                    else
                    {
                        projectile.velocity.Y += SAImovement;
                    }
                }
            }

            float passiveMvtFloat = 0.5f;
            projectile.tileCollide = false;
            float range = 100f;
            Vector2 projPos = new Vector2(projectile.Center.X, projectile.Center.Y);
            float xDist = player.Center.X - projPos.X;
            float yDist = player.Center.Y - projPos.Y;
            yDist += Main.rand.NextFloat(-10, 20);
            xDist += Main.rand.NextFloat(-10, 20);
			//Light pets lead the player, normal pets trail the player
            xDist += 60f * (lightPet ? (float)player.direction : -(float)player.direction);
            yDist -= 60f;
			Vector2 playerVector = new Vector2(xDist, yDist);
            float playerDist = playerVector.Length();
            float returnSpeed = 18f;

			//If player is close enough, resume normal
            if (playerDist < range && player.velocity.Y == 0f &&
                projectile.position.Y + projectile.height <= player.position.Y + player.height &&
                !Collision.SolidCollision(projectile.position, projectile.width, projectile.height))
            {
                if (projectile.velocity.Y < -6f)
                {
                    projectile.velocity.Y = -6f;
                }
            }

			//Teleport to player if too far
            if (playerDist > 2000f)
            {
                projectile.position.X = player.Center.X - projectile.width / 2;
                projectile.position.Y = player.Center.Y - projectile.height / 2;
                projectile.netUpdate = true;
            }

            if (playerDist < 50f)
            {
                if (Math.Abs(projectile.velocity.X) > 2f || Math.Abs(projectile.velocity.Y) > 2f)
                {
                    projectile.velocity *= 0.99f;
                }
                passiveMvtFloat = 0.01f;
            }
            else
            {
                if (playerDist < 100f)
                {
                    passiveMvtFloat = 0.1f;
                }
                if (playerDist > 300f)
                {
                    passiveMvtFloat = 1f;
                }
                playerDist = returnSpeed / playerDist;
                playerVector.X *= playerDist;
                playerVector.Y *= playerDist;
            }
            if (projectile.velocity.X < playerVector.X)
            {
                projectile.velocity.X += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.X < 0f)
                {
                    projectile.velocity.X += passiveMvtFloat;
                }
            }
            if (projectile.velocity.X > playerVector.X)
            {
                projectile.velocity.X -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.X > 0f)
                {
                    projectile.velocity.X -= passiveMvtFloat;
                }
            }
            if (projectile.velocity.Y < playerVector.Y)
            {
                projectile.velocity.Y += passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.Y < 0f)
                {
                    projectile.velocity.Y += passiveMvtFloat * 2f;
                }
            }
            if (projectile.velocity.Y > playerVector.Y)
            {
                projectile.velocity.Y -= passiveMvtFloat;
                if (passiveMvtFloat > 0.05f && projectile.velocity.Y > 0f)
                {
                    projectile.velocity.Y -= passiveMvtFloat * 2f;
                }
            }
			if (projectile.velocity.X >= 0.25f)
			{
				projectile.direction = faceRight ? 1 : -1;
			}
			else if (projectile.velocity.X < -0.25f)
			{
				projectile.direction = faceRight ? -1 : 1;
			}
			//Tilting and change directions
			projectile.spriteDirection = projectile.direction;
			projectile.rotation = projectile.velocity.X * tiltFloat;
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
