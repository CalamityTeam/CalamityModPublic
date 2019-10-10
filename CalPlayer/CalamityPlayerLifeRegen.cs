using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.World;
using CalamityMod.Utilities;

namespace CalamityMod.CalPlayer
{
    public class CalamityPlayerLifeRegen
	{
		#region Update Bad Life Regen
		public static void CalamityUpdateBadLifeRegen(Player player, Mod mod)
		{
			Point point = player.Center.ToTileCoordinates();
			CalamityPlayer modPlayer = player.Calamity();

			//Initial Debuffs

			if (modPlayer.shadowflame)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 30;
			}

			if (modPlayer.wDeath)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.statDefense -= 50;
				player.allDamage -= 0.1f;
			}

			if (modPlayer.bFlames || modPlayer.aFlames)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 16;
			}

			if (modPlayer.gsInferno || (modPlayer.ZoneCalamity && player.lavaWet))
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 30;
			}

			if (modPlayer.astralInfection)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 20;
			}

			if (modPlayer.ZoneSulphur && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && !modPlayer.aquaticScourgeLore)
			{
				player.AddBuff(BuffID.Poisoned, 2, true);
				modPlayer.pissWaterBoost++;

				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= modPlayer.pissWaterBoost / 200;

				if (player.lifeRegen < -8)
					player.lifeRegen = -8;
			}
			else
				modPlayer.pissWaterBoost = 0;

			if (modPlayer.hFlames)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 20;
			}

			if (modPlayer.pFlames)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 20;
				player.blind = true;
				player.statDefense -= 8;
				player.moveSpeed -= 0.15f;
			}

			if (modPlayer.bBlood)
			{
				if (player.lifeRegen > 0)
				{
					player.lifeRegen = 0;
				}
				player.lifeRegenTime = 0;
				player.lifeRegen -= 8;
				player.blind = true;
				player.statDefense -= 3;
				player.moveSpeed += 0.2f;
				player.meleeDamage += 0.05f;
				player.rangedDamage -= 0.1f;
				player.magicDamage -= 0.1f;
			}

			if (modPlayer.horror)
			{
				player.blind = true;
				player.statDefense -= 15;
				player.moveSpeed -= 0.15f;
			}

			if (modPlayer.aCrunch)
			{
				player.statDefense /= 3;
				player.endurance *= 0.33f;
			}

			if (modPlayer.vHex)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= 16;
				player.blind = true;
				player.statDefense -= 30;
				player.moveSpeed -= 0.1f;

				if (player.wingTimeMax <= 0)
					player.wingTimeMax = 0;

				player.wingTimeMax /= 2;
			}

			if (modPlayer.cDepth)
			{
				if (player.statDefense > 0)
				{
					int depthDamage = modPlayer.depthCharm ? 9 : 18;
					int subtractDefense = (int)((double)player.statDefense * 0.05); //240 defense = 0 damage taken with depth charm
					int calcDepthDamage = depthDamage - subtractDefense;

					if (calcDepthDamage < 0)
						calcDepthDamage = 0;

					if (player.lifeRegen > 0)
						player.lifeRegen = 0;

					player.lifeRegenTime = 0;
					player.lifeRegen -= calcDepthDamage;
				}
			}

			// Buffs

			if (modPlayer.tRegen)
				player.lifeRegen += 3;

			if (modPlayer.sRegen)
				player.lifeRegen += 2;

			if (modPlayer.tarraSet)
			{
				player.calmed = (modPlayer.tarraMelee ? false : true);
				player.lifeMagnet = true;
			}

			if (modPlayer.aChicken)
			{
				player.lifeRegen += 1;
				player.statDefense += 5;
				player.moveSpeed += 0.1f;
			}

			if (modPlayer.cadence)
			{
				if (player.FindBuffIndex(BuffID.Regeneration) > -1) { player.ClearBuff(BuffID.Regeneration); }
				if (player.FindBuffIndex(BuffID.Lifeforce) > -1) { player.ClearBuff(BuffID.Lifeforce); }
				player.discount = true;
				player.lifeMagnet = true;
				player.calmed = true;
				player.loveStruck = true;
				player.lifeRegen += 5;
				player.statLifeMax2 += player.statLifeMax / 5 / 20 * 25;
			}

			if (modPlayer.omniscience)
			{
				player.detectCreature = true;
				player.dangerSense = true;
				player.findTreasure = true;
			}

			if (modPlayer.aWeapon)
				player.moveSpeed += 0.15f;

			if (modPlayer.mushy)
			{
				player.statDefense += 5;
				player.lifeRegen += 1;
			}

			if (modPlayer.molten)
				player.resistCold = true;

			if (modPlayer.shellBoost)
				player.moveSpeed += 0.9f;

			if (modPlayer.celestialJewel || modPlayer.astralArcanum)
			{
				bool lesserEffect = false;
				for (int l = 0; l < 22; l++)
				{
					int hasBuff = player.buffType[l];
					lesserEffect = CalamityMod.alcoholList.Contains(hasBuff);
				}

				if (lesserEffect)
				{
					player.lifeRegen += 1;
					player.statDefense += 20;
				}
				else
				{
					if (player.lifeRegen < 0)
					{
						if (player.lifeRegenTime < 1800)
							player.lifeRegenTime = 1800;

						player.lifeRegen += 4;
						player.statDefense += 20;
					}
					else
						player.lifeRegen += 2;
				}
			}
			else if (modPlayer.crownJewel)
			{
				bool lesserEffect = false;
				for (int l = 0; l < 22; l++)
				{
					int hasBuff = player.buffType[l];
					lesserEffect = CalamityMod.alcoholList.Contains(hasBuff);
				}

				if (lesserEffect)
					player.statDefense += 10;
				else
				{
					if (player.lifeRegen < 0)
					{
						if (player.lifeRegenTime < 1800)
							player.lifeRegenTime = 1800;

						player.lifeRegen += 2;
						player.statDefense += 10;
					}
					else
						player.lifeRegen += 1;
				}
			}

			if (modPlayer.permafrostsConcoction)
			{
				if (player.statLife < player.statLifeMax2 / 2)
					player.lifeRegen++;
				if (player.statLife < player.statLifeMax2 / 4)
					player.lifeRegen++;
				if (player.statLife < player.statLifeMax2 / 10)
					player.lifeRegen += 2;

				if (player.poisoned || player.onFire || modPlayer.bFlames)
					player.lifeRegen += 4;
			}

			// Last Debuffs

			if (modPlayer.omegaBlueChestplate)
			{
				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;

				if (player.lifeRegenCount > 0)
					player.lifeRegenCount = 0;
			}

			if (Config.LethalLava)
			{
				if (Main.myPlayer == player.whoAmI)
				{
					if (Collision.LavaCollision(player.position, player.width, (player.waterWalk ? (player.height - 6) : player.height)))
					{
						if (player.lavaImmune && !player.immune)
						{
							if (player.lavaTime > 0)
								player.lavaTime--;
						}

						if (player.lavaTime <= 0)
							player.AddBuff(mod.BuffType("LethalLavaBurn"), 2, true);
					}
				}

				if (modPlayer.lethalLavaBurn)
				{
					if (player.lifeRegen > 0)
						player.lifeRegen = 0;

					player.lifeRegenTime = 0;
					int lifeRegenDown = (player.lavaImmune ? 9 : 18);

					if (player.lavaRose)
						lifeRegenDown = 3;

					player.lifeRegen -= lifeRegenDown;
				}
			}

			if (modPlayer.hInferno)
			{
				modPlayer.hInfernoBoost++;

				if (player.lifeRegen > 0)
					player.lifeRegen = 0;

				player.lifeRegenTime = 0;
				player.lifeRegen -= modPlayer.hInfernoBoost;

				if (player.lifeRegen < -200)
					player.lifeRegen = -200;
			}
			else
				modPlayer.hInfernoBoost = 0;

			if (modPlayer.gState)
			{
				player.statDefense /= 2;
				player.velocity.Y = 0f;
				player.velocity.X = 0f;
			}

			if (modPlayer.eGravity)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax /= 4;
			}

			if (modPlayer.eGrav)
			{
				if (player.wingTimeMax < 0)
					player.wingTimeMax = 0;

				if (player.wingTimeMax > 400)
					player.wingTimeMax = 400;

				player.wingTimeMax /= 2;
			}

			if (modPlayer.molluskSet)
				player.velocity.X *= 0.985f;

			if ((modPlayer.warped || modPlayer.caribbeanRum) && !player.slowFall)
				player.velocity.Y *= 1.01f;

			if (modPlayer.weakPetrification || CalamityWorld.bossRushActive)
			{
				if (player.mount.Active)
					player.mount.Dismount(player);
			}

			if (modPlayer.silvaCountdown > 0 && modPlayer.hasSilvaEffect && modPlayer.silvaSet)
			{
				if (player.lifeRegen < 0)
					player.lifeRegen = 0;
			}
		}
		#endregion

		#region Update Life Regen
		public static void CalamityUpdateLifeRegen(Player player, Mod mod)
		{
			CalamityPlayer modPlayer = player.Calamity();

			if (!player.shinyStone)
			{
				int lifeRegenTimeMaxBoost = (CalamityPlayer.areThereAnyDamnBosses ? 450 : 1800);
				int lifeRegenMaxBoost = (CalamityPlayer.areThereAnyDamnBosses ? 1 : 4);
				float lifeRegenLifeRegenTimeMaxBoost = (CalamityPlayer.areThereAnyDamnBosses ? 8f : 30f);

				if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
				{
					if (modPlayer.shadeRegen)
					{
						if (player.lifeRegenTime > 90 && player.lifeRegenTime < lifeRegenTimeMaxBoost)
							player.lifeRegenTime = lifeRegenTimeMaxBoost;

						player.lifeRegenTime += lifeRegenMaxBoost;
						player.lifeRegen += lifeRegenMaxBoost;

						float num3 = (float)((double)player.lifeRegenTime * 2.5); //lifeRegenTime max is 3600
						num3 /= 300f;
						if (num3 > 0f)
						{
							if (num3 > lifeRegenLifeRegenTimeMaxBoost)
								num3 = lifeRegenLifeRegenTimeMaxBoost;

							player.lifeRegen += (int)num3;
						}

						if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
						{
							player.lifeRegenCount++;
							if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30)))
							{
								int num5 = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 200, default, 1f);
								Main.dust[num5].noGravity = true;
								Main.dust[num5].velocity *= 0.75f;
								Main.dust[num5].fadeIn = 1.3f;
								Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
								vector.Normalize();
								vector *= (float)Main.rand.Next(50, 100) * 0.04f;
								Main.dust[num5].velocity = vector;
								vector.Normalize();
								vector *= 34f;
								Main.dust[num5].position = player.Center - vector;
							}
						}
					}
					else if (modPlayer.cFreeze)
					{
						if (player.lifeRegenTime > 90 && player.lifeRegenTime < lifeRegenTimeMaxBoost)
							player.lifeRegenTime = lifeRegenTimeMaxBoost;

						player.lifeRegenTime += lifeRegenMaxBoost;
						player.lifeRegen += lifeRegenMaxBoost;

						float num3 = (float)((double)player.lifeRegenTime * 2.5); //lifeRegenTime max is 3600
						num3 /= 300f;
						if (num3 > 0f)
						{
							if (num3 > lifeRegenLifeRegenTimeMaxBoost)
								num3 = lifeRegenLifeRegenTimeMaxBoost;

							player.lifeRegen += (int)num3;
						}

						if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
						{
							player.lifeRegenCount++;
							if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(30)))
							{
								int num5 = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 200, new Color(150, Main.DiscoG, 255), 0.75f);
								Main.dust[num5].noGravity = true;
								Main.dust[num5].velocity *= 0.75f;
								Main.dust[num5].fadeIn = 1.3f;
								Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
								vector.Normalize();
								vector *= (float)Main.rand.Next(50, 100) * 0.04f;
								Main.dust[num5].velocity = vector;
								vector.Normalize();
								vector *= 34f;
								Main.dust[num5].position = player.Center - vector;
							}
						}
					}
					else if (modPlayer.draedonsHeart)
					{
						if (player.lifeRegenTime > 90 && player.lifeRegenTime < lifeRegenTimeMaxBoost)
							player.lifeRegenTime = lifeRegenTimeMaxBoost;

						player.lifeRegenTime += lifeRegenMaxBoost;
						player.lifeRegen += lifeRegenMaxBoost;

						float num3 = (float)((double)player.lifeRegenTime * 2.5); //lifeRegenTime max is 3600
						num3 /= 300f;
						if (num3 > 0f)
						{
							if (num3 > lifeRegenLifeRegenTimeMaxBoost)
								num3 = lifeRegenLifeRegenTimeMaxBoost;

							player.lifeRegen += (int)num3;
						}

						if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
						{
							player.lifeRegenCount++;
							if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2)))
							{
								int num5 = Dust.NewDust(player.position, player.width, player.height, 107, 0f, 0f, 200, default, 1f);
								Main.dust[num5].noGravity = true;
								Main.dust[num5].velocity *= 0.75f;
								Main.dust[num5].fadeIn = 1.3f;
								Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
								vector.Normalize();
								vector *= (float)Main.rand.Next(50, 100) * 0.04f;
								Main.dust[num5].velocity = vector;
								vector.Normalize();
								vector *= 34f;
								Main.dust[num5].position = player.Center - vector;
							}
						}
					}
					else if (modPlayer.photosynthesis)
					{
						int lifeRegenTimeMaxBoost2 = Main.dayTime ? lifeRegenTimeMaxBoost : (lifeRegenTimeMaxBoost / 5);
						int lifeRegenMaxBoost2 = Main.dayTime ? lifeRegenMaxBoost : (lifeRegenMaxBoost / 5);
						float lifeRegenLifeRegenTimeMaxBoost2 = Main.dayTime ? lifeRegenLifeRegenTimeMaxBoost : (lifeRegenLifeRegenTimeMaxBoost / 5);

						if (player.lifeRegenTime > 90 && player.lifeRegenTime < lifeRegenTimeMaxBoost2)
							player.lifeRegenTime = lifeRegenTimeMaxBoost2;

						player.lifeRegenTime += lifeRegenMaxBoost2;
						player.lifeRegen += lifeRegenMaxBoost2;

						float num3 = (float)((double)player.lifeRegenTime * 2.5); //lifeRegenTime max is 3600
						num3 /= 300f;
						if (num3 > 0f)
						{
							if (num3 > lifeRegenLifeRegenTimeMaxBoost2)
								num3 = lifeRegenLifeRegenTimeMaxBoost2;

							player.lifeRegen += (int)num3;
						}

						if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
						{
							player.lifeRegenCount++;
							if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.NextBool(2)))
							{
								int num5 = Dust.NewDust(player.position, player.width, player.height, 244, 0f, 0f, 200, default, 1f);
								Main.dust[num5].noGravity = true;
								Main.dust[num5].velocity *= 0.75f;
								Main.dust[num5].fadeIn = 1.3f;
								Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
								vector.Normalize();
								vector *= (float)Main.rand.Next(50, 100) * 0.04f;
								Main.dust[num5].velocity = vector;
								vector.Normalize();
								vector *= 34f;
								Main.dust[num5].position = player.Center - vector;
							}
						}
					}
				}
			}
		}
		#endregion
	}
}
