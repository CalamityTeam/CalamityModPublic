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
using CalamityMod.Items.CalamityCustomThrowingDamage;

namespace CalamityMod.Projectiles
{
	public class CalamityGlobalProjectile : GlobalProjectile
	{
		#region InstancePerEntity
		public override bool InstancePerEntity
		{
			get
			{
				return true;
			}
		}
		#endregion

		public bool rogue = false;

		public bool setDamageValues = true;

		public float spawnedPlayerMinionDamageValue = 1f;

		public int spawnedPlayerMinionProjectileDamageValue = 0;

		public int counter = 0;

		public int counter2 = 0;

		#region AI
		public override void AI(Projectile projectile)
		{
			if (projectile.modProjectile != null && projectile.modProjectile.mod.Name.Equals("CalamityMod"))
				goto SKIP_CALAMITY;
			if ((projectile.minion || projectile.sentry) && !ProjectileID.Sets.StardustDragon[projectile.type]) //For all other mods and vanilla, exclude dragon due to bugs
			{
				if (setDamageValues)
				{
					spawnedPlayerMinionDamageValue = Main.player[projectile.owner].minionDamage;
					spawnedPlayerMinionProjectileDamageValue = projectile.damage;
					setDamageValues = false;
				}
				if (Main.player[projectile.owner].minionDamage != spawnedPlayerMinionDamageValue)
				{
					int damage2 = (int)(((float)spawnedPlayerMinionProjectileDamageValue / spawnedPlayerMinionDamageValue) * Main.player[projectile.owner].minionDamage);
					projectile.damage = damage2;
				}
			}
			SKIP_CALAMITY:
			if (projectile.type == ProjectileID.HallowStar)
			{
				if (projectile.ai[0] == 1f)
				{
					projectile.ranged = false;
					projectile.melee = true;
				}
				else if (projectile.ai[0] == 2f)
				{
					projectile.ranged = false;
					rogue = true;
				}
			}
			else if (projectile.type == ProjectileID.FallingStar)
			{
				if (projectile.ai[0] == 2f)
				{
					rogue = true;
				}
			}
			else if (projectile.type == ProjectileID.Meteor1 || projectile.type == ProjectileID.Meteor2 || projectile.type == ProjectileID.Meteor3)
			{
				if (projectile.ai[0] == 1f)
				{
					projectile.magic = false;
					projectile.melee = true;
					projectile.tileCollide = false;
				}
			}
			else if (projectile.type == ProjectileID.GoldenShowerFriendly)
			{
				if (projectile.ai[0] == 1f)
				{
					projectile.magic = false;
					projectile.melee = true;
				}
			}
			else if (projectile.type == ProjectileID.MiniSharkron)
			{
				if (projectile.ai[0] == 1f)
					projectile.melee = true;
				else if (projectile.ai[0] == 2f)
					projectile.ranged = true;
			}
			else if (projectile.type == ProjectileID.LostSoulFriendly)
			{
				if (projectile.ai[0] == 1f)
				{
					projectile.magic = false;
					projectile.ranged = true;
				}
			}
			else if (projectile.type == ProjectileID.LunarFlare)
			{
				if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("StellarStriker"))
				{
					projectile.magic = false;
					projectile.melee = true;
				}
			}
			else if (projectile.type == ProjectileID.RubyBolt || projectile.type == ProjectileID.SapphireBolt || projectile.type == ProjectileID.AmethystBolt)
			{
				if (projectile.ai[0] == 1f)
				{
					projectile.magic = false;
					projectile.melee = true;
				}
			}
			else if (projectile.type == ProjectileID.SolarWhipSwordExplosion)
			{
				if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("TruePaladinsHammer") ||
					Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("FlameScythe"))
				{
					projectile.melee = false;
				}
				else if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].ranged)
				{
					projectile.melee = false;
					projectile.ranged = true;
				}
				else if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("PurgeGuzzler") ||
					Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("Lazhar"))
				{
					projectile.melee = false;
					projectile.magic = true;
				}
			}
			else if (projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.Wasp)
			{
				if (projectile.timeLeft > 570) //all of these have a time left of 600 or 660
				{
					if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("PlagueKeeper"))
					{
						projectile.magic = false;
						projectile.melee = true;
					}
					else if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("TheSwarmer"))
					{
						projectile.magic = true;
					}
					else if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("TheHive") ||
						Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("Malevolence") ||
						Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == ItemID.BeesKnees)
					{
						projectile.magic = false;
						projectile.ranged = true;
					}
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).eQuiver && projectile.ranged &&
				projectile.friendly && CalamityMod.rangedProjectileExceptionList.TrueForAll(x => projectile.type != x))
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
						offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
						if (projectile.owner == Main.myPlayer)
						{
							int projectile1 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 8f), (float)(Math.Cos(offsetAngle) * 8f), projectile.type, (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
							int projectile2 = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 8f), (float)(-Math.Cos(offsetAngle) * 8f), projectile.type, (int)((double)projectile.damage * 0.5), projectile.knockBack, projectile.owner, 0f, 0f);
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
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).nanotech && rogue && projectile.friendly)
			{
				counter++;
				if (counter >= 30)
				{
					counter = 0;
					if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[mod.ProjectileType("Nanotech")] < 30)
					{
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("Nanotech"),
							(int)((double)projectile.damage * 0.15), 0f, projectile.owner, 0f, 0f);
					}
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).daedalusSplit && rogue && projectile.friendly)
			{
				counter2++;
				if (counter2 >= 30)
				{
					counter2 = 0;
					if (projectile.owner == Main.myPlayer && Main.player[projectile.owner].ownedProjectileCounts[90] < 30)
					{
						for (int num252 = 0; num252 < 2; num252++)
						{
							Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							while (value15.X == 0f && value15.Y == 0f)
							{
								value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
							}
							value15.Normalize();
							value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
							int shard = Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, 90, (int)((double)projectile.damage * 0.25), 0f, projectile.owner, 0f, 0f);
							Main.projectile[shard].ranged = false;
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
			if (projectile.ranged)
			{
				//BULLETS

				//musket ball: base 100 + 7 = 107
				//silver: base 100 + 9 = 109
				//meteor shot: base 100 + 9 = 109 pierces once
				//acceleration: base 100 + bullet damage 9 = 109 speeds up while travelling
				//flash: base 100 + bullet damage 7 = 107 + (flash damage 107 * 0.25 = 26) = 133 AoE confusion
				//superball: base 100 + bullet damage 6 = 106 bounces a fucking shitload of times and speeds up
				//enhanced nano: base 100 + bullet damage 12 = 112 + (nanobot damage 112 * 0.3 = 33 * 2 nanobots = 66) = 178
				//crystal: base 100 + bullet damage 9 = 109 + (crystal shard damage 109 * 0.6 = 65 * 0.6 = 39 * 3 total shards = 117) = 226
				//frostspark: base 100 + bullet damage 11 = 111 + (explosion damage 111 * 0.5 = 55) = 166
				//icy: base 100 + 10 bullet damage = 110 + (ice shard damage 110 * 0.5 = 55 * 0.6 = 33 * 2 total shards = 66) = 176
				//verium: base 100 + bullet damage 8 = 108 * 0.8 = 86 homing
				//chloro: base 100 + bullet damage 10 = 110 * 0.8 = 88 homing
				//terra: base 100 + bullet damage 9 = 109 + (terra shard damage 109 * 0.5 = 54 * 2 total shards = 106) = 215
				//acid: base 100 + bullet damage 36 = 136 does more based on enemy defense
				//hyperius: base 100 + bullet damage 21 = 121 + (hyperius second bullet damage 121 * 0.8 = 96) = 217
				//holy fire: base 100 + bullet damage 27 = 127 + (explosion damage 127 * 0.85 = 107) = 234

				//ARROWS

				//holy arrow: base 100 + arrow damage 13 = 113 + (star damage 113 * 0.5 = 56 * 0.7 = 39 * 2 total stars = 78) = 191
				//terra arrow: base 100 + arrow damage 9 = 109 + (terra arrow split damage 109 * 0.5 = 54 * 2 total arrows = 106) = 215
				//elysian arrow: base 100 + arrow damage 20 = 120 + meteor damage 120 = 240
				//napalm arrow: base 100 + arrow damage 13 = 113 + (fire shard damage 113 * 0.3 = 33 * 3 total shards = 99) = 212
				//icicle arrow: base 100 + arrow damage 14 = 114 + (ice shard damage 114 * 0.5 = 57 * 0.6 = 34 * 3 total shards = 102) = 216
				//bloodfire arrow: base 100 + arrow damage 40 = 140 but it heals so it's alright
				//arctic arrow: base 100 + arrow damage 16 = 116 but it freezes enemies so it's alright
				//vanquisher arrow: base 100 + arrow damage 33 = 133 + split arrow damage 93 = 226 fine for endgame arrow
				switch (projectile.type)
				{
					case ProjectileID.CrystalShard:
						damage = (int)((double)damage * 0.6);
						break;
					case ProjectileID.ChlorophyteBullet:
						damage = (int)((double)damage * 0.8);
						break;
					case ProjectileID.HallowStar:
						damage = (int)((double)damage * 0.7);
						break;
				}
				if (projectile.type == mod.ProjectileType("VeriumBullet"))
				{
					damage = (int)((double)damage * 0.8);
				}
				else if (projectile.type == mod.ProjectileType("FrostsparkBullet"))
				{
					if (target.buffImmune[mod.BuffType("GlacialState")])
					{
						damage = (int)((double)damage * 1.2);
					}
				}
				else if (projectile.type == mod.ProjectileType("AcidBullet"))
				{
					int defenseAdd = (int)((double)target.defense * 0.1); //100 defense * 0.1 = 10
					damage = damage + defenseAdd;
				}
			}
			if (projectile.owner == Main.myPlayer)
			{
				if (rogue)
				{
					crit = (Main.rand.Next(1, 101) < CalamityCustomThrowingDamagePlayer.ModPlayer(Main.player[projectile.owner]).throwingCrit);
				}
				if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type))
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
						damage = (int)((double)damage * 0.6);
					}
					else
					{
						damage = (int)((double)damage * 1.1);
					}
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).screwdriver)
				{
					if (projectile.penetrate > 1 || projectile.penetrate == -1)
					{
						damage = (int)((double)damage * 1.1);
					}
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).sPower)
				{
					if (projectile.minion || CalamityMod.projectileMinionList.Contains(projectile.type))
					{
						damage = (int)((double)damage * 1.1);
					}
				}
				if (CalamityWorld.revenge)
				{
					bool DHorHoD = (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).draedonsHeart ||
						Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).heartOfDarkness);
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageMode &&
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).adrenalineMode)
					{
						if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type) ||
							projectile.melee || projectile.ranged || projectile.magic || rogue)
						{
							damage = (int)((double)damage *
								(CalamityWorld.death ? (DHorHoD ? 9.9 : 9.0) : (DHorHoD ? 3.3 : 3.0)));
						}
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageMode)
					{
						if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type) ||
							projectile.melee || projectile.ranged || projectile.magic || rogue)
						{
							double rageDamageBoost = 0.0 +
								(Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageBoostOne ? (CalamityWorld.death ? 0.6 : 0.15) : 0.0) + //3.6 or 1.65
								(Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageBoostTwo ? (CalamityWorld.death ? 0.6 : 0.15) : 0.0) + //4.2 or 1.8
								(Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).rageBoostThree ? (CalamityWorld.death ? 0.6 : 0.15) : 0.0); //4.8 or 1.95
							double rageDamage =
								(CalamityWorld.death ? (DHorHoD ? 3.3 : 3.0) : (DHorHoD ? 1.65 : 1.5)) + rageDamageBoost;
							damage = (int)((double)damage * rageDamage);
						}
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).adrenalineMode)
					{
						if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type) ||
							projectile.melee || projectile.ranged || projectile.magic || rogue)
						{
							damage = (int)((double)damage * ((CalamityWorld.death ? 7.0 : 2.5) * Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).adrenalineDmgMult));
						}
					}
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).uberBees &&
					(projectile.type == ProjectileID.GiantBee || projectile.type == ProjectileID.Bee || projectile.type == ProjectileID.Wasp))
				{
					if (Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("TheSwarmer") ||
						Main.player[projectile.owner].inventory[Main.player[projectile.owner].selectedItem].type == mod.ItemType("PlagueKeeper"))
					{
						damage = damage + Main.rand.Next(10, 21);
					}
					else
					{
						damage = damage + Main.rand.Next(70, 101);
						projectile.penetrate = 1;
					}
				}
			}
			if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).astralStarRain && crit &&
				Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).astralStarRainCooldown <= 0)
			{
				if (projectile.owner == Main.myPlayer)
				{
					Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).astralStarRainCooldown = 60;
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
				if (projectile.magic && Main.player[projectile.owner].ghostHeal)
				{
					float num = 0.1f;
					num -= (float)projectile.numHits * 0.05f;
					if (num < 0f)
					{
						num = 0f;
					}
					float num2 = (float)damage * num;
					Main.player[Main.myPlayer].lifeSteal -= num2;
				}
				if (projectile.type == ProjectileID.VampireKnife)
				{
					float num = (float)damage * 0.0375f;
					if (num < 0f)
					{
						num = 0f;
					}
					Main.player[Main.myPlayer].lifeSteal -= num;
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).uberBees &&
					(projectile.type == 566 || projectile.type == 181 || projectile.type == 189))
				{
					target.AddBuff(mod.BuffType("Plague"), 360);
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).alchFlask &&
					(projectile.magic || rogue || projectile.melee || projectile.minion || projectile.ranged) &&
						Main.player[projectile.owner].ownedProjectileCounts[mod.ProjectileType("PlagueSeeker")] < 6)
				{
					int newDamage = (int)((double)projectile.damage * 0.25);
					if (newDamage > 30)
					{
						newDamage = 30;
					}
					int plague = Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("PlagueSeeker"), newDamage, 0f, projectile.owner, 0f, 0f);
					Main.projectile[plague].melee = false;
				}
				if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).reaverBlast && projectile.melee)
				{
					int newDamage = (int)((double)projectile.damage * 0.2);
					if (newDamage > 30)
					{
						newDamage = 30;
					}
					Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ReaverBlast"), newDamage, 0f, projectile.owner, 0f, 0f);
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
					Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
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
					Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
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
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).silvaMage && projectile.penetrate == 1 &&
						Main.rand.Next(0, 100) >= 97)
					{
						Main.PlaySound(29, (int)projectile.position.X, (int)projectile.position.Y, 103);
						projectile.position = projectile.Center;
						projectile.width = (projectile.height = 96);
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
						projectile.damage *= (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).auricSet ? 7 : 4);
						projectile.Damage();
					}
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraMage && target.canGhostHeal)
					{
						if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraMageHealCooldown <= 0)
						{
							Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).tarraMageHealCooldown = 90;
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
							Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
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
							int newDamage = (int)((double)projectile.damage * 0.2);
							if (newDamage > 30)
							{
								newDamage = 30;
							}
							Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, 569 + Main.rand.Next(3), newDamage, 0f, projectile.owner, 0f, 0f);
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
						if (target.canGhostHeal)
						{
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
							Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
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
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("XerocOrb"), (int)((double)num * 1.2), 0f, projectile.owner, (float)num6, 0f);
						if (target.canGhostHeal)
						{
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
							Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
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
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("GodSlayerOrb"),
							(int)((double)num * (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).auricSet ? 2.0 : 1.5)), 0f, projectile.owner, (float)num6, 0f);
						if (target.canGhostHeal)
						{
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
							Main.player[Main.myPlayer].lifeSteal -= num12 * 1.5f;
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
				}
				else if (projectile.melee)
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).ataxiaGeyser &&
						Main.player[projectile.owner].ownedProjectileCounts[mod.ProjectileType("ChaosGeyser")] < 3)
					{
						int newDamage = (int)((double)projectile.damage * 0.15);
						if (newDamage > 35)
						{
							newDamage = 35;
						}
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("ChaosGeyser"), newDamage, 0f, projectile.owner, 0f, 0f);
					}
					else if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet &&
						Main.player[projectile.owner].ownedProjectileCounts[mod.ProjectileType("XerocBlast")] < 3)
					{
						int newDamage = (int)((double)projectile.damage * 0.2);
						if (newDamage > 40)
						{
							newDamage = 40;
						}
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("XerocBlast"), newDamage, 0f, projectile.owner, 0f, 0f);
					}
				}
				else if (projectile.ranged)
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet &&
						Main.player[projectile.owner].ownedProjectileCounts[mod.ProjectileType("XerocFire")] < 3)
					{
						int newDamage = (int)((double)projectile.damage * 0.15);
						if (newDamage > 40)
						{
							newDamage = 40;
						}
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, 0f, 0f, mod.ProjectileType("XerocFire"), newDamage, 0f, projectile.owner, 0f, 0f);
					}
				}
				else if (rogue)
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).xerocSet &&
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
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("XerocStar"), (int)((double)num * 1.6), 0f, projectile.owner, (float)num6, 0f);
					}
				}
				else if (projectile.minion || projectile.sentry || CalamityMod.projectileMinionList.Contains(projectile.type))
				{
					if (Main.player[projectile.owner].GetModPlayer<CalamityPlayer>(mod).pArtifact)
					{
						target.AddBuff(mod.BuffType("HolyLight"), 300);
					}
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
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("GodSlayerPhantom"), (int)((double)num * 2.0), 0f, projectile.owner, 0f, ai1);
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
						Projectile.NewProjectile(projectile.Center.X, projectile.Center.Y, num8, num9, mod.ProjectileType("XerocBubble"), (int)((double)num * 1.2), 0f, projectile.owner, (float)num6, 0f);
					}
				}
			}
		}
		#endregion

		#region Drawing
		public override Color? GetAlpha(Projectile projectile, Color lightColor)
		{
			if (Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).trippy)
				return new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, projectile.alpha);
			return null;
		}

		public override bool PreDraw(Projectile projectile, SpriteBatch spriteBatch, Color lightColor)
		{
			if (Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).trippy)
			{
				Texture2D texture = Main.projectileTexture[projectile.type];
				SpriteEffects spriteEffects = SpriteEffects.None;
				if (projectile.spriteDirection == 1)
				{
					spriteEffects = SpriteEffects.FlipHorizontally;
				}
				float num66 = 0f;
				Vector2 vector11 = new Vector2((float)(texture.Width / 2), (float)(texture.Height / Main.projFrames[projectile.type] / 2));
				Microsoft.Xna.Framework.Color color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
				Microsoft.Xna.Framework.Color alpha15 = projectile.GetAlpha(color9);
				float num212 = 0.99f;
				alpha15.R = (byte)((float)alpha15.R * num212);
				alpha15.G = (byte)((float)alpha15.G * num212);
				alpha15.B = (byte)((float)alpha15.B * num212);
				alpha15.A = (byte)((float)alpha15.A * num212);
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
					position9.X -= (float)(projectile.width / 2);
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
					position9.Y -= (float)(projectile.height / 2);
					Main.spriteBatch.Draw(texture,
						new Vector2(position9.X - Main.screenPosition.X + (float)(projectile.width / 2) - (float)texture.Width * projectile.scale / 2f + vector11.X * projectile.scale, position9.Y - Main.screenPosition.Y + (float)projectile.height - (float)texture.Height * projectile.scale / (float)Main.projFrames[projectile.type] + 4f + vector11.Y * projectile.scale + num66 + projectile.gfxOffY),
						new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(0, y, texture.Width, frames)), alpha15, projectile.rotation, vector11, projectile.scale, spriteEffects, 0f);
				}
			}
			return true;
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
						int newDamage = (int)((double)projectile.damage * 0.33);
						if (newDamage > 65)
						{
							newDamage = 65;
						}
						Projectile.NewProjectile(projectile.oldPosition.X + (float)(projectile.width / 2), projectile.oldPosition.Y + (float)(projectile.height / 2), value15.X, value15.Y, mod.ProjectileType("TarraEnergy"), newDamage, 0f, projectile.owner, 0f, 0f);
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
