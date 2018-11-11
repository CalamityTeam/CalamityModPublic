using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod;
using CalamityMod.NPCs.TheDevourerofGods;
using CalamityMod.Items.Armor;

namespace CalamityMod.Projectiles
{
    public class CalamityGlobalProjectile : GlobalProjectile
    {
        public static float counter = 0;

        #region AI
        public override void AI(Projectile projectile)
		{
            List<int> rangedProjectileExceptionList = new List<int>(6)
            {
                ProjectileID.Phantasm,
                ProjectileID.VortexBeater,
                mod.ProjectileType("Phangasm"),
                mod.ProjectileType("Contagion"),
                mod.ProjectileType("DaemonsFlame"),
                mod.ProjectileType("ExoTornado"),
                mod.ProjectileType("Drataliornus")
            };
            bool shouldAffect = rangedProjectileExceptionList.TrueForAll(x => projectile.type != x);
            if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).eQuiver && projectile.ranged && projectile.friendly && shouldAffect)
			{
				if (Main.rand.Next(200) > 198)
				{
					float spread = 180f * 0.0174f;
					double startAngle = Math.Atan2(projectile.velocity.X, projectile.velocity.Y) - spread / 2;
					double deltaAngle = spread / 8f;
					double offsetAngle;
					int i;
					for (i = 0; i < 1; i++)
					{
					   	offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
					   	if (projectile.owner == Main.myPlayer)
					   	{
						   	int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( Math.Sin(offsetAngle) * 8f ), (float)( Math.Cos(offsetAngle) * 8f ), projectile.type, (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
						    int projectile2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)( -Math.Sin(offsetAngle) * 8f ), (float)( -Math.Cos(offsetAngle) * 8f ), projectile.type, (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
						    Main.projectile[projectile1].ranged = false;
						    Main.projectile[projectile2].ranged = false;
                            Main.projectile[projectile1].timeLeft = 60;
                            Main.projectile[projectile2].timeLeft = 60;
                            Main.projectile[projectile1].noDropItem = true;
                            Main.projectile[projectile2].noDropItem = true;
                        }
					}
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).nanotech && projectile.thrown && projectile.friendly)
			{
				counter += 1f;
				if (counter >= 45f)
				{
					counter = 0f;
					if (projectile.owner == Main.myPlayer)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("Nanotech"), projectile.damage, 0f, projectile.owner, 0f, 0f);
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
                    if (projectile.type == ProjectileID.PureSpray)
                    {
                        CalamityWorld.ConvertFromAstral(i, j, ConvertType.Pure);
                    }
                    if (projectile.type == ProjectileID.CorruptSpray)
                    {
                        CalamityWorld.ConvertFromAstral(i, j, ConvertType.Corrupt);
                    }
                    if (projectile.type == ProjectileID.CrimsonSpray)
                    {
                        CalamityWorld.ConvertFromAstral(i, j, ConvertType.Crimson);
                    }
                    if (projectile.type == ProjectileID.HallowSpray)
                    {
                        CalamityWorld.ConvertFromAstral(i, j, ConvertType.Hallow);
                    }
                }
            }
        }
		#endregion
		
        #region ModifyHitNPC
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
            List<int> projectileMinionList = new List<int>(20)
            {
                195,
                423,
                433,
                614,
                408,
                389,
                376,
                374,
                664,
                666,
                668,
                680,
                694,
                695,
                696,
                378,
                379,
                309,
                642,
                644
            };
            bool shouldAffect = projectileMinionList.Contains(projectile.type);
            if (projectile.owner == Main.myPlayer && CalamityWorld.revenge)
            {
                if ((projectile.minion || shouldAffect))
                {
                    Player player = Main.player[projectile.owner];
                    if (!player.inventory[player.selectedItem].summon &&
                        (player.inventory[player.selectedItem].melee ||
                        player.inventory[player.selectedItem].ranged || 
                        player.inventory[player.selectedItem].magic ||
                        player.inventory[player.selectedItem].thrown) && 
                        player.inventory[player.selectedItem].hammer == 0 &&
                        player.inventory[player.selectedItem].pick == 0 && 
                        player.inventory[player.selectedItem].axe == 0)
                    {
                        if (NPC.downedMoonlord)
                        {
                            damage = (int)((double)damage * 0.33);
                        }
                        else if (Main.hardMode)
                        {
                            damage = (int)((double)damage * 0.66);
                        }
                        else
                        {
                            damage = (int)((double)damage * 0.9);
                        }
                    }
                    else if (!NPC.downedMoonlord)
                    {
                        damage = (int)((double)damage * 1.1);
                    }
                }
            }
            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).sPower)
            {
                if (projectile.minion || shouldAffect)
                {
                    damage = (int)((double)damage * 1.1);
                }
            }
            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageMode &&
                Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).adrenalineMode)
            {
                if (projectile.minion || shouldAffect)
                {
                    damage = (int)((double)damage * (CalamityWorld.death ? 13.0 : 4.0));
                }
            }
            else if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageMode)
            {
                if (projectile.minion || shouldAffect)
                {
                    double rageDamageBoost = 0.0 +
                        (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageBoostOne ? (CalamityWorld.death ? 0.8 : 0.2) : 0.0) + //4.8 or 1.2
                        (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageBoostTwo ? (CalamityWorld.death ? 0.8 : 0.2) : 0.0) + //5.6 or 1.4
                        (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageBoostThree ? (CalamityWorld.death ? 0.8 : 0.2) : 0.0); //6.4 or 1.6
                    double rageDamage = (CalamityWorld.death ? 5.0 : 2.0) + rageDamageBoost;
                    damage = (int)((double)damage * rageDamage);
                }
            }
            else if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).adrenalineMode)
            {
                if (projectile.minion || shouldAffect)
                {
                    damage = (int)((double)damage * ((CalamityWorld.death ? 11.0 : 3.5) * Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).adrenalineDmgMult));
                }
            }
            if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).uberBees && 
                (projectile.type == 566 || projectile.type == 181 || projectile.type == 189))
			{
				if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("TheSwarmer"))
				{
					damage = damage + Main.rand.Next(10, 21);
				}
				else
				{
					damage = damage + Main.rand.Next(70, 101);
					projectile.penetrate = 1;
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).astralStarRain && crit &&
                Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).astralStarRainCooldown <= 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
                    Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).astralStarRainCooldown = 120;
                    for (int n = 0; n < 3; n++)
					{
						float x = target.position.X + (float)Main.rand.Next(-400, 400);
						float y = target.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 vector = new Vector2(x, y);
						float num13 = target.position.X + (float)(target.width / 2) - vector.X;
						float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
						num13 += (float)Main.rand.Next(-100, 101);
						int num15 = 25;
						int projectileType = Main.rand.Next(3);
						if (projectileType == 0)
						{
							projectileType = mod.ProjectileType("AstralStar");
						}
						else if (projectileType == 1)
						{
							projectileType = 92;
						}
						else
						{
							projectileType = 12;
						}
						float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
						num16 = (float)num15 / num16;
						num13 *= num16;
						num14 *= num16;
						int num17 = Projectile.NewProjectile(x, y, num13, num14, projectileType, 75, 5f, projectile.owner, 0f, 0f);
						Main.projectile[num17].ranged = false;
					}
				}
			}
		}
        #endregion

        #region OnHitNPC
        public override void OnHitNPC(Projectile projectile, NPC target, int damage, float knockback, bool crit)
		{
			if (projectile.owner == Main.myPlayer)
			{
				if (target.type == NPCID.TargetDummy)
				{
					return;
				}
                if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).abyssalAmulet)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 180);
                }
                if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).dsSetBonus)
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 180);
                }
                if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).frostFlare)
				{
					target.AddBuff(BuffID.Frostburn, 360);
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).uberBees && 
                    (projectile.type == 566 || projectile.type == 181 || projectile.type == 189))
				{
					target.AddBuff(mod.BuffType("Plague"), 360);
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).alchFlask && 
                    (projectile.magic || projectile.thrown || projectile.melee || projectile.minion || projectile.ranged))
				{
					target.AddBuff(mod.BuffType("Plague"), 120);
					int plague = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("PlagueSeeker"), (int)((double)projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
					Main.projectile[plague].melee = false;
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).reaverBlast && projectile.melee)
				{
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ReaverBlast"), (int)((double)projectile.damage * 0.2), 0f, projectile.owner, 0f, 0f);
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).auricSet && target.canGhostHeal)
				{
					float num11 = 0.05f;
					num11 -= (float)projectile.numHits * 0.025f;
					if (num11 <= 0f)
					{
						return;
					}
					float num12 = (float)projectile.damage * num11;
					if ((int)num12 <= 0)
					{
						return;
					}
					if (Main.player[Main.myPlayer].lifeSteal <= 0f)
					{
						return;
					}
					Main.player[Main.myPlayer].lifeSteal -= num12;
					float num13 = 0f;
					int num14 = projectile.owner;
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
						{
							float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
							if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
							{
								num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
								num14 = i;
							}
						}
					}
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("AuricOrb"), 0, 0f, projectile.owner, (float)num14, num12);
				}
				else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).silvaSet && target.canGhostHeal)
				{
					float num11 = 0.03f;
					num11 -= (float)projectile.numHits * 0.015f;
					if (num11 <= 0f)
					{
						return;
					}
					float num12 = (float)projectile.damage * num11;
					if ((int)num12 <= 0)
					{
						return;
					}
					if (Main.player[Main.myPlayer].lifeSteal <= 0f)
					{
						return;
					}
					Main.player[Main.myPlayer].lifeSteal -= num12;
					float num13 = 0f;
					int num14 = projectile.owner;
					for (int i = 0; i < 255; i++)
					{
						if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
						{
							float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
							if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
							{
								num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
								num14 = i;
							}
						}
					}
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("SilvaOrb"), 0, 0f, projectile.owner, (float)num14, num12);
				}
				if (projectile.magic)
				{
                    if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).silvaMage && projectile.penetrate <= 1 && 
                        Main.rand.Next(0, 100) >= 97)
                    {
                        Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 103);
                        projectile.position = projectile.Center;
                        projectile.width = (projectile.height = projectile.height * 4);
                        projectile.position.X = projectile.position.X - (float)(projectile.width / 2);
                        projectile.position.Y = projectile.position.Y - (float)(projectile.height / 2);
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
                        projectile.damage *= 4;
                        projectile.Damage();
                    }
                    if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraMage)
                    {
                        if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraMageHealCooldown <= 0)
                        {
                            Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraMageHealCooldown = 90;
                            int healAmount = (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).auricSet ? projectile.damage / 100 : projectile.damage / 50);
                            Player player = Main.player[projectile.owner];
                            player.statLife += healAmount;
                            player.HealEffect(healAmount);
                            if (player.statLife > player.statLifeMax2)
                            {
                                player.statLife = player.statLifeMax2;
                            }
                        }
                    }
                    if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).reaverBurst)
					{
						int num251 = Main.rand.Next(2, 5);
						for (int num252 = 0; num252 < num251; num252++)
						{
							Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							while (value15.X == 0f && value15.Y == 0f)
							{
								value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							}
							value15.Normalize();
							value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
							Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, 569 + Main.rand.Next(3), (int)((double)projectile.damage * 0.2), 0f, projectile.owner, 0f, 0f);
						}
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).ataxiaMage &&
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).ataxiaDmg <= 0)
					{
						int num = projectile.damage / 2;
						Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).ataxiaDmg += (float)num;
						int[] array = new int[200];
						int num3 = 0;
						int num4 = 0;
						for (int i = 0; i < 200; i++)
						{
							if (Main.npc[i].CanBeChasedBy(projectile, false))
							{
								float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
								if (num5 < 800f)
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
						float num7 = 20f;
						float num8 = (float)Main.rand.Next(-100, 101);
						float num9 = (float)Main.rand.Next(-100, 101);
						float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
						num10 = num7 / num10;
						num8 *= num10;
						num9 *= num10;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("AtaxiaOrb"), (int)((double)num * 1.5), 0f, projectile.owner, (float)num6, 0f);
						float num11 = 0.1f; //0.2
						num11 -= (float)projectile.numHits * 0.05f; //0.05
						if (num11 <= 0f)
						{
							return;
						}
						float num12 = (float)projectile.damage * num11;
						if ((int)num12 <= 0)
						{
							return;
						}
						if (Main.player[Main.myPlayer].lifeSteal <= 0f)
						{
							return;
						}
						Main.player[Main.myPlayer].lifeSteal -= num12;
						float num13 = 0f;
						int num14 = projectile.owner;
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
							{
								float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
								if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
								{
									num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
									num14 = i;
								}
							}
						}
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("AtaxiaHealOrb"), 0, 0f, projectile.owner, (float)num14, num12);
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet &&
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocDmg <= 0)
					{
						int num = projectile.damage / 2;
						Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocDmg += (float)num;
						int[] array = new int[200];
						int num3 = 0;
						int num4 = 0;
						for (int i = 0; i < 200; i++)
						{
							if (Main.npc[i].CanBeChasedBy(projectile, false))
							{
								float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
								if (num5 < 800f)
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
						float num7 = 30f;
						float num8 = (float)Main.rand.Next(-100, 101);
						float num9 = (float)Main.rand.Next(-100, 101);
						float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
						num10 = num7 / num10;
						num8 *= num10;
						num9 *= num10;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("XerocOrb"), (int)((double)num * 0.8), 0f, projectile.owner, (float)num6, 0f);
						float num11 = 0.1f;
						num11 -= (float)projectile.numHits * 0.05f;
						if (num11 <= 0f)
						{
							return;
						}
						float num12 = (float)projectile.damage * num11;
						if ((int)num12 <= 0)
						{
							return;
						}
						if (Main.player[Main.myPlayer].lifeSteal <= 0f)
						{
							return;
						}
						Main.player[Main.myPlayer].lifeSteal -= num12;
						float num13 = 0f;
						int num14 = projectile.owner;
						for (int i = 0; i < 255; i++)
						{
							if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
							{
								float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
								if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
								{
									num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
									num14 = i;
								}
							}
						}
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("XerocHealOrb"), 0, 0f, projectile.owner, (float)num14, num12);
					}
                    else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).godSlayerMage &&
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).godSlayerDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).godSlayerDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
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
                        float num7 = 20f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("GodSlayerOrb"), (int)((double)num * 1.5), 0f, projectile.owner, (float)num6, 0f);
                        float num11 = (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).auricSet ? 0.03f : 0.06f); //0.2
                        num11 -= (float)projectile.numHits * 0.015f; //0.05
                        if (num11 <= 0f)
                        {
                            return;
                        }
                        float num12 = (float)projectile.damage * num11;
                        if ((int)num12 <= 0)
                        {
                            return;
                        }
                        if (Main.player[Main.myPlayer].lifeSteal <= 0f)
                        {
                            return;
                        }
                        Main.player[Main.myPlayer].lifeSteal -= num12;
                        float num13 = 0f;
                        int num14 = projectile.owner;
                        for (int i = 0; i < 255; i++)
                        {
                            if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[projectile.owner].hostile && !Main.player[i].hostile) || Main.player[projectile.owner].team == Main.player[i].team))
                            {
                                float num15 = Math.Abs(Main.player[i].position.X + (float)(Main.player[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.player[i].position.Y + (float)(Main.player[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num15 < 1200f && (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife) > num13)
                                {
                                    num13 = (float)(Main.player[i].statLifeMax2 - Main.player[i].statLife);
                                    num14 = i;
                                }
                            }
                        }
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("GodSlayerHealOrb"), 0, 0f, projectile.owner, (float)num14, num12);
                    }
                }
				else if (projectile.melee)
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).ataxiaGeyser)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ChaosGeyser"), (int)((double)projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("XerocBlast"), (int)((double)projectile.damage * 0.15), 0f, projectile.owner, 0f, 0f);
					}
				}
				else if (projectile.ranged)
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("XerocFire"), (int)((double)projectile.damage * 0.1), 0f, projectile.owner, 0f, 0f);
					}
				}
				else if (projectile.thrown)
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).daedalusSplit)
					{
						int num251 = Main.rand.Next(2, 5);
						for (int num252 = 0; num252 < num251; num252++)
						{
							Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							while (value15.X == 0f && value15.Y == 0f)
							{
								value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							}
							value15.Normalize();
							value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
							Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, 90, (int)((double)projectile.damage * 0.15), 0.25f, projectile.owner, 0f, 0f);
						}
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet &&
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocDmg <= 0)
					{
						int num = projectile.damage / 2;
						Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocDmg += (float)num;
						int[] array = new int[200];
						int num3 = 0;
						int num4 = 0;
						for (int i = 0; i < 200; i++)
						{
							if (Main.npc[i].CanBeChasedBy(projectile, false))
							{
								float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
								if (num5 < 800f)
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
						float num7 = Main.rand.Next(15, 30);
						float num8 = (float)Main.rand.Next(-100, 101);
						float num9 = (float)Main.rand.Next(-100, 101);
						float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
						num10 = num7 / num10;
						num8 *= num10;
						num9 *= num10;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("XerocStar"), (int)((double)num * 1.3), 0f, projectile.owner, (float)num6, 0f);
					}
				}
				else if (projectile.minion)
				{
                    if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tearMinions)
					{
						target.AddBuff(mod.BuffType("TemporalSadness"), 60);
					}
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).shadowMinions)
					{
						target.AddBuff(BuffID.ShadowFlame, 300);
					}
                    if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).godSlayerSummon && 
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).godSlayerDmg <= 0)
                    {
                        int num = projectile.damage / 2;
                        float ai1 = (Main.rand.NextFloat() + 0.5f);
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).godSlayerDmg += (float)num;
                        int[] array = new int[200];
                        int num3 = 0;
                        int num4 = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            if (Main.npc[i].CanBeChasedBy(projectile, false))
                            {
                                float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
                                if (num5 < 800f)
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
                        float num7 = 15f;
                        float num8 = (float)Main.rand.Next(-100, 101);
                        float num9 = (float)Main.rand.Next(-100, 101);
                        float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
                        num10 = num7 / num10;
                        num8 *= num10;
                        num9 *= num10;
                        Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("GodSlayerPhantom"), (int)((double)num * 1.5), 0f, projectile.owner, 0f, ai1);
                    }
                    else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet &&
                        Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocDmg <= 0)
					{
						int num = projectile.damage / 2;
						Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocDmg += (float)num;
						int[] array = new int[200];
						int num3 = 0;
						int num4 = 0;
						for (int i = 0; i < 200; i++)
						{
							if (Main.npc[i].CanBeChasedBy(projectile, false))
							{
								float num5 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - projectile.position.X + (float)(projectile.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - projectile.position.Y + (float)(projectile.height / 2));
								if (num5 < 800f)
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
						float num7 = 15f;
						float num8 = (float)Main.rand.Next(-100, 101);
						float num9 = (float)Main.rand.Next(-100, 101);
						float num10 = (float)Math.Sqrt((double)(num8 * num8 + num9 * num9));
						num10 = num7 / num10;
						num8 *= num10;
						num9 *= num10;
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("XerocBubble"), (int)((double)num * 1.3), 0f, projectile.owner, (float)num6, 0f);
					}
				}
			}
		}
        #endregion

        #region Kill
        public override void Kill(Projectile projectile, int timeLeft)
        {
            if (projectile.ranged)
            {
                if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraRanged && Main.rand.Next(0, 100) >= 88)
                {
                    int num251 = Main.rand.Next(2, 4);
                    for (int num252 = 0; num252 < num251; num252++)
                    {
                        Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        while (value15.X == 0f && value15.Y == 0f)
                        {
                            value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                        }
                        value15.Normalize();
                        value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                        Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, mod.ProjectileType("TarraEnergy"), (int)((double)projectile.damage * 0.33), 0f, projectile.owner, 0f, 0f);
                    }
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
    }
}
