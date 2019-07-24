using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;

namespace CalamityMod.NPCs
{
	public class CalamityGlobalNPCLoot : GlobalNPC
	{
		#region PreNPCLoot
		public override bool PreNPCLoot(NPC npc)
		{
			if (CalamityWorld.bossRushActive)
				return BossRushLootCancel(npc, mod);

			bool abyssLootCancel = AbyssLootCancel(npc, mod);
			if (abyssLootCancel)
				return false;

			if (CalamityWorld.revenge)
			{
				if (npc.type == NPCID.Probe)
					return false;
			}

			// Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
			bool lastTwinStanding = false;
			if (npc.type == NPCID.Retinazer)
				lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
			else if (npc.type == NPCID.Spazmatism)
				lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);

			// Mechanical Bosses' combined lore item
			bool mechLore = !NPC.downedMechBossAny && (lastTwinStanding || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime);
			DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge20"), true, mechLore);

			// King Slime
			if (npc.type == NPCID.KingSlime)
			{
				// Drop a huge spray of individual Gel items
				int minGel = Main.expertMode ? 90 : 60;
				int maxGel = Main.expertMode ? 120 : 80;
				int amount = Main.rand.Next(minGel, maxGel + 1);
				for (int i = 0; i < amount; ++i)
					DropHelper.DropItem(npc, ItemID.Gel);

				// King Slime doesn't have a lore item yet
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedSlimeKing, 2, 0, 0);
			}

			// Eye of Cthulhu
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge3"), true, !NPC.downedBoss1);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss1, 2, 0, 0);
			}

			// Queen Bee
			else if (npc.type == NPCID.QueenBee)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge16"), true, !NPC.downedQueenBee);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedQueenBee, 2, 0, 0);
			}

			// The Destroyer
			else if (npc.type == NPCID.TheDestroyer)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge21"), true, !NPC.downedMechBoss1);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss1, 4, 2, 1);
			}

			// Retinazer OR Spazmatism (whichever is killed last)
			else if (lastTwinStanding)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge22"), true, !NPC.downedMechBoss2);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss2, 4, 2, 1);
			}

			// Skeletron Prime
			else if (npc.type == NPCID.SkeletronPrime)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge23"), true, !NPC.downedMechBoss3);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss3, 4, 2, 1);
			}

			// Plantera
			else if (npc.type == NPCID.Plantera)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge25"), true, !NPC.downedPlantBoss);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedPlantBoss, 4, 2, 1);
			}

			// Duke Fishron
			else if (npc.type == NPCID.DukeFishron)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge2"), true, !NPC.downedFishron);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedPlantBoss, 4, 2, 1);
			}

			// Lunatic Cultist
			else if (npc.type == NPCID.CultistBoss)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge4"), true, !NPC.downedAncientCultist);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedAncientCultist, 4, 2, 1);

				// Blood Moon lore item
				DropHelper.DropItemCondition(npc, mod.ItemType("Knowledge34"), true, Main.bloodMoon);

				// Deus text (this is not a loot function)
				if (!NPC.downedAncientCultist)
				{
					string key = "Mods.CalamityMod.DeusText";
					Color messageColor = Color.Gold;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
			}
			return true;
		}
		#endregion

		#region Boss Rush Loot Cancel
		private bool BossRushLootCancel(NPC npc, Mod mod)
		{
			if (npc.type == mod.NPCType("ProfanedGuardianBoss"))
			{
				CalamityWorld.bossRushStage = 7;
				DespawnProj();
			}
			else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
			{
				int count = 0;
				for (int j = 0; j < 200; j++)
				{
					if (Main.npc[j].active && (Main.npc[j].type == NPCID.EaterofWorldsHead || Main.npc[j].type == NPCID.EaterofWorldsBody || Main.npc[j].type == NPCID.EaterofWorldsTail))
					{
						count++;
						break;
					}
				}
				if (count < 4)
				{
					CalamityWorld.bossRushStage = 8;
					DespawnProj();
				}
			}
			else if (npc.type == mod.NPCType("Astrageldon"))
			{
				CalamityWorld.bossRushStage = 9;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("Bumblefuck"))
			{
				CalamityWorld.bossRushStage = 12;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("HiveMindP2"))
			{
				CalamityWorld.bossRushStage = 14;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("StormWeaverHeadNaked"))
			{
				CalamityWorld.bossRushStage = 16;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("AquaticScourgeHead"))
			{
				CalamityWorld.bossRushStage = 17;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("DesertScourgeHead"))
			{
				CalamityWorld.bossRushStage = 18;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("CrabulonIdle"))
			{
				CalamityWorld.bossRushStage = 20;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("CeaselessVoid"))
			{
				CalamityWorld.bossRushStage = 22;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("PerforatorHive"))
			{
				CalamityWorld.bossRushStage = 23;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("Cryogen"))
			{
				CalamityWorld.bossRushStage = 24;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("BrimstoneElemental"))
			{
				CalamityWorld.bossRushStage = 25;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("CosmicWraith"))
			{
				CalamityWorld.bossRushStage = 26;
				DespawnProj();

				string key = "Mods.CalamityMod.BossRushTierThreeEndText";
				Color messageColor = Color.LightCoral;
				if (Main.netMode == 0)
					Main.NewText(Language.GetTextValue(key), messageColor);
				else if (Main.netMode == 2)
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
			}
			else if (npc.type == mod.NPCType("ScavengerBody"))
			{
				CalamityWorld.bossRushStage = 27;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("AstrumDeusHeadSpectral"))
			{
				CalamityWorld.bossRushStage = 30;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("Polterghast"))
			{
				CalamityWorld.bossRushStage = 31;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("PlaguebringerGoliath"))
			{
				CalamityWorld.bossRushStage = 32;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("CalamitasRun3"))
			{
				CalamityWorld.bossRushStage = 33;
				DespawnProj();

				string key = "Mods.CalamityMod.BossRushTierFourEndText";
				Color messageColor = Color.LightCoral;
				if (Main.netMode == 0)
					Main.NewText(Language.GetTextValue(key), messageColor);
				else if (Main.netMode == 2)
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
			}
			else if (npc.type == mod.NPCType("Siren") || npc.type == mod.NPCType("Leviathan"))
			{
				int bossType = (npc.type == mod.NPCType("Siren")) ? mod.NPCType("Leviathan") : mod.NPCType("Siren");
				if (!NPC.AnyNPCs(bossType))
				{
					CalamityWorld.bossRushStage = 34;
					DespawnProj();
				}
			}
			else if (npc.type == mod.NPCType("SlimeGodCore") || npc.type == mod.NPCType("SlimeGodSplit") || npc.type == mod.NPCType("SlimeGodRunSplit"))
			{
				if (npc.type == mod.NPCType("SlimeGodCore") && !NPC.AnyNPCs(mod.NPCType("SlimeGodSplit")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRunSplit")) &&
					!NPC.AnyNPCs(mod.NPCType("SlimeGod")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRun")))
				{
					CalamityWorld.bossRushStage = 35;
					DespawnProj();
				}
				else if (npc.type == mod.NPCType("SlimeGodSplit") && !NPC.AnyNPCs(mod.NPCType("SlimeGodCore")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRunSplit")) &&
					NPC.CountNPCS(mod.NPCType("SlimeGodSplit")) < 2 && !NPC.AnyNPCs(mod.NPCType("SlimeGodRun")))
				{
					CalamityWorld.bossRushStage = 35;
					DespawnProj();
				}
				else if (npc.type == mod.NPCType("SlimeGodRunSplit") && !NPC.AnyNPCs(mod.NPCType("SlimeGodCore")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodSplit")) &&
					NPC.CountNPCS(mod.NPCType("SlimeGodRunSplit")) < 2 && !NPC.AnyNPCs(mod.NPCType("SlimeGod")))
				{
					CalamityWorld.bossRushStage = 35;
					DespawnProj();
				}
			}
			else if (npc.type == mod.NPCType("Providence"))
			{
				CalamityWorld.bossRushStage = 36;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("SupremeCalamitas"))
			{
				CalamityWorld.bossRushStage = 37;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("Yharon"))
			{
				CalamityWorld.bossRushStage = 38;
				DespawnProj();
			}
			else if (npc.type == mod.NPCType("DevourerofGodsHeadS"))
			{
				DropHelper.DropItem(npc, mod.ItemType("Rock"), true);
				CalamityWorld.bossRushStage = 0;
				DespawnProj();
				CalamityWorld.bossRushActive = false;

				if (Main.netMode == 2)
				{
					NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
					var netMessage = mod.GetPacket();
					netMessage.Write((byte)CalamityModMessageType.BossRushStage);
					netMessage.Write(CalamityWorld.bossRushStage);
					netMessage.Send();
				}

				string key = "Mods.CalamityMod.BossRushTierFiveEndText";
				Color messageColor = Color.LightCoral;
				if (Main.netMode == 0)
					Main.NewText(Language.GetTextValue(key), messageColor);
				else if (Main.netMode == 2)
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				return false;
			}

			switch (npc.type)
			{
				case NPCID.QueenBee:
					CalamityWorld.bossRushStage = 1;
					DespawnProj();
					break;
				case NPCID.BrainofCthulhu:
					CalamityWorld.bossRushStage = 2;
					DespawnProj();
					break;
				case NPCID.KingSlime:
					CalamityWorld.bossRushStage = 3;
					DespawnProj();
					break;
				case NPCID.EyeofCthulhu:
					CalamityWorld.bossRushStage = 4;
					DespawnProj();
					break;
				case NPCID.SkeletronPrime:
					CalamityWorld.bossRushStage = 5;
					DespawnProj();
					break;
				case NPCID.Golem:
					CalamityWorld.bossRushStage = 6;
					DespawnProj();
					break;
				case NPCID.TheDestroyer:
					CalamityWorld.bossRushStage = 10;
					DespawnProj();

					string key = "Mods.CalamityMod.BossRushTierOneEndText";
					Color messageColor = Color.LightCoral;
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					break;
				case NPCID.Spazmatism:
					CalamityWorld.bossRushStage = 11;
					DespawnProj();
					break;
				case NPCID.Retinazer:
					CalamityWorld.bossRushStage = 11;
					DespawnProj();
					break;
				case NPCID.WallofFlesh:
					CalamityWorld.bossRushStage = 13;
					DespawnProj();
					break;
				case NPCID.SkeletronHead:
					CalamityWorld.bossRushStage = 15;
					DespawnProj();
					break;
				case NPCID.CultistBoss:
					CalamityWorld.bossRushStage = 19;
					DespawnProj();

					string key2 = "Mods.CalamityMod.BossRushTierTwoEndText";
					Color messageColor2 = Color.LightCoral;
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key2), messageColor2);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
					break;
				case NPCID.Plantera:
					CalamityWorld.bossRushStage = 21;
					DespawnProj();
					break;
				case NPCID.DukeFishron:
					CalamityWorld.bossRushStage = 28;
					DespawnProj();
					break;
				case NPCID.MoonLordCore:
					CalamityWorld.bossRushStage = 29;
					DespawnProj();
					break;
				default:
					break;
			}

			if (Main.netMode == 2)
			{
				var netMessage = mod.GetPacket();
				netMessage.Write((byte)CalamityModMessageType.BossRushStage);
				netMessage.Write(CalamityWorld.bossRushStage);
				netMessage.Send();
			}
			return false;
		}
		#endregion

		#region Abyss Loot Cancel
		private bool AbyssLootCancel(NPC npc, Mod mod)
		{
			int x = Main.maxTilesX;
			int y = Main.maxTilesY;
			int genLimit = x / 2;
			int abyssChasmY = y - 250;
			int abyssChasmX = (CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135));
			bool abyssPosX = false;
			bool abyssPosY = ((double)(npc.position.Y / 16f) <= abyssChasmY);

			if (CalamityWorld.abyssSide)
			{
				if ((double)(npc.position.X / 16f) < abyssChasmX + 80)
					abyssPosX = true;
			}
			else
			{
				if ((double)(npc.position.X / 16f) > abyssChasmX - 80)
					abyssPosX = true;
			}

			bool hurtByAbyss = (npc.wet && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage &&
				((((double)(npc.position.Y / 16f) > (Main.rockLayer - (double)Main.maxTilesY * 0.05)) &&
				abyssPosY && abyssPosX) || CalamityWorld.abyssTiles > 200) && !npc.buffImmune[mod.BuffType("CrushDepth")]);

			return hurtByAbyss;
		}
		#endregion

		#region NPCLoot
		public override void NPCLoot(NPC npc)
		{
			bool revenge = CalamityWorld.revenge;
			if (CalamityGlobalNPC.DraedonMayhem)
			{
				if (!CalamityPlayer.areThereAnyDamnBosses)
				{
					CalamityGlobalNPC.DraedonMayhem = false;
					CalamityGlobalNPC.UpdateServerBoolean();
				}
			}

			RareVariants(npc, mod);

			if (CalamityWorld.defiled)
				DefiledLoot(npc, mod);

			if (CalamityWorld.armageddon)
				ArmageddonLoot(npc, mod);

			if (npc.boss && revenge)
			{
				if (npc.type != mod.NPCType("HiveMind") && npc.type != mod.NPCType("Leviathan") && npc.type != mod.NPCType("Siren") &&
					npc.type != mod.NPCType("StormWeaverHead") && npc.type != mod.NPCType("StormWeaverBody") &&
					npc.type != mod.NPCType("StormWeaverTail") && npc.type != mod.NPCType("DevourerofGodsHead") &&
					npc.type != mod.NPCType("DevourerofGodsBody") && npc.type != mod.NPCType("DevourerofGodsTail"))
				{
					if (Main.netMode != 2)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
							Main.player[Main.myPlayer].GetModPlayer<CalamityPlayer>(mod).adrenaline = 0;
					}
				}
			}

			CheckBossSpawn(npc, mod);

			WormBossLoot(npc, mod);

			ArmorSetLoot(npc, mod);

			RareLoot(npc, mod);

			CommonLoot(npc, mod);

			BossLoot(npc, mod);
		}
		#endregion

		#region Rare Variants
		private void RareVariants(NPC npc, Mod mod)
		{
			switch (npc.type)
			{
				default:
					break;
				case NPCID.BloodZombie:
					DropHelper.DropItemCondition(npc, mod.ItemType("Carnage"), NPC.downedBoss3 && !npc.SpawnedFromStatue, 200, 1, 1);
					break;
				case NPCID.TacticalSkeleton:
					DropHelper.DropItemChance(npc, mod.ItemType("TrueConferenceCall"), 200);
					break;
				case NPCID.DesertBeast:
					DropHelper.DropItemChance(npc, mod.ItemType("EvilSmasher"), 200);
					break;
				case NPCID.DungeonSpirit:
					DropHelper.DropItemChance(npc, mod.ItemType("PearlGod"), 200);
					break;
				case NPCID.RuneWizard:
					DropHelper.DropItemChance(npc, mod.ItemType("EyeofMagnus"), 10);
					break;
				case NPCID.Mimic:
					DropHelper.DropItemCondition(npc, mod.ItemType("TheBee"), !npc.SpawnedFromStatue, 100, 1, 1);
					break;
			}
		}
		#endregion

		#region Defiled Loot
		private void DefiledLoot(NPC npc, Mod mod)
		{
			switch (npc.type)
			{
				default:
					break;
				case NPCID.Werewolf:
					DropHelper.DropItemChance(npc, ItemID.MoonCharm, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.AdhesiveBandage, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.AnglerFish:
					DropHelper.DropItemChance(npc, ItemID.AdhesiveBandage, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.DesertBeast:
					DropHelper.DropItemChance(npc, ItemID.AncientHorn, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.ArmoredSkeleton:
					DropHelper.DropItemChance(npc, ItemID.BeamSword, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.ArmorPolish, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Clown:
					DropHelper.DropItemChance(npc, ItemID.Bananarang, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.ToxicSludge:
					DropHelper.DropItemChance(npc, ItemID.Bezoar, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.EyeofCthulhu:
					DropHelper.DropItemChance(npc, ItemID.Binoculars, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.WanderingEye:
					DropHelper.DropItemChance(npc, ItemID.BlackLens, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.CorruptSlime:
					DropHelper.DropItemChance(npc, ItemID.Blindfold, DropHelper.DefiledDropRateInt);
					break;
				// This is all the random skeletons in the hardmode dungeon
				case 269:
				case 270:
				case 271:
				case 272:
				case 273:
				case 274:
				case 275:
				case 276:
				case 277:
				case 278:
				case 279:
				case 280:
					DropHelper.DropItemChance(npc, ItemID.Keybrand, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.BoneFeather, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.MagnetSphere, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.WispinaBottle, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.UndeadMiner:
					DropHelper.DropItemChance(npc, ItemID.BonePickaxe, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.ScutlixRider:
					DropHelper.DropItemChance(npc, ItemID.BrainScrambler, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Vampire:
					DropHelper.DropItemChance(npc, ItemID.BrokenBatWing, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.MoonStone, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.CaveBat:
					DropHelper.DropItemChance(npc, ItemID.ChainKnife, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.DepthMeter, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.DarkCaster:
					DropHelper.DropItemChance(npc, ItemID.ClothierVoodooDoll, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.PirateCaptain:
					DropHelper.DropItemChance(npc, ItemID.CoinGun, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.DiscountCard, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.Cutlass, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.LuckyCoin, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.PirateStaff, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Reaper:
					DropHelper.DropItemChance(npc, ItemID.DeathSickle, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Demon:
					DropHelper.DropItemChance(npc, ItemID.DemonScythe, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.DesertDjinn:
					DropHelper.DropItemChance(npc, ItemID.DjinnLamp, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.DjinnsCurse, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Shark:
					DropHelper.DropItemChance(npc, ItemID.DivingHelmet, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Pixie:
				case NPCID.Wraith:
				case NPCID.Mummy:
					DropHelper.DropItemChance(npc, ItemID.FastClock, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.RedDevil:
					DropHelper.DropItemChance(npc, ItemID.FireFeather, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.IceElemental:
				case NPCID.IcyMerman:
					DropHelper.DropItemChance(npc, ItemID.IceSickle, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.FrostStaff, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.ArmoredViking:
					DropHelper.DropItemChance(npc, ItemID.IceSickle, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.IceTortoise:
					DropHelper.DropItemChance(npc, ItemID.IceSickle, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.FrozenTurtleShell, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Harpy:
					DropHelper.DropItemCondition(npc, ItemID.GiantHarpyFeather, Main.hardMode, DropHelper.DefiledDropRateFloat);
					break;
				case NPCID.QueenBee:
					DropHelper.DropItemChance(npc, ItemID.HoneyedGoggles, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Piranha:
					DropHelper.DropItemChance(npc, ItemID.Hook, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.DiabolistRed:
				case NPCID.DiabolistWhite:
					DropHelper.DropItemChance(npc, ItemID.InfernoFork, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.PinkJellyfish:
					DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Paladin:
					DropHelper.DropItemChance(npc, ItemID.Kraken, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.PaladinsHammer, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.SkeletonArcher:
					DropHelper.DropItemChance(npc, ItemID.Marrow, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.MagicQuiver, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Lavabat:
					DropHelper.DropItemChance(npc, ItemID.MagmaStone, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.WalkingAntlion:
					DropHelper.DropItemChance(npc, ItemID.AntlionClaw, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.DarkMummy:
					DropHelper.DropItemChance(npc, ItemID.Blindfold, DropHelper.DefiledDropRateInt);
					DropHelper.DropItemChance(npc, ItemID.Megaphone, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.GreenJellyfish:
					DropHelper.DropItemChance(npc, ItemID.Megaphone, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.CursedSkull:
					DropHelper.DropItemChance(npc, ItemID.Nazar, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.FireImp:
					DropHelper.DropItemChance(npc, ItemID.ObsidianRose, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.BlackRecluse:
				case NPCID.BlackRecluseWall:
					DropHelper.DropItemChance(npc, ItemID.PoisonStaff, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.SkeletonSniper:
					DropHelper.DropItemChance(npc, ItemID.RifleScope, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.ChaosElemental:
					DropHelper.DropItemChance(npc, ItemID.RodofDiscord, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Necromancer:
				case NPCID.NecromancerArmored:
					DropHelper.DropItemChance(npc, ItemID.ShadowbeamStaff, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.SnowFlinx:
					DropHelper.DropItemChance(npc, ItemID.SnowballLauncher, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.RaggedCaster:
				case NPCID.RaggedCasterOpenCoat:
					DropHelper.DropItemChance(npc, ItemID.SpectreStaff, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Plantera:
					DropHelper.DropItemChance(npc, ItemID.TheAxe, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.GiantBat:
					DropHelper.DropItemChance(npc, ItemID.TrifoldMap, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.AngryTrapper:
					DropHelper.DropItemChance(npc, ItemID.Uzi, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.Corruptor:
				case NPCID.FloatyGross:
					DropHelper.DropItemChance(npc, ItemID.Vitamins, DropHelper.DefiledDropRateInt);
					break;
				case NPCID.GiantTortoise:
					DropHelper.DropItemCondition(npc, ItemID.Yelets, NPC.downedMechBossAny, DropHelper.DefiledDropRateFloat);
					break;
			}

			// Every type of demon eye counts for Black Lenses
			if (CalamityMod.demonEyeList.Contains(npc.type))
				DropHelper.DropItemChance(npc, ItemID.BlackLens, DropHelper.DefiledDropRateInt);

			// Every type of Skeleton counts for the Bone Sword
			if (CalamityMod.skeletonList.Contains(npc.type))
				DropHelper.DropItemChance(npc, ItemID.BoneSword, DropHelper.DefiledDropRateInt);

			// Every type of Angry Bones counts for the Clothier Voodoo Doll
			if (CalamityMod.angryBonesList.Contains(npc.type))
				DropHelper.DropItemChance(npc, ItemID.ClothierVoodooDoll, DropHelper.DefiledDropRateInt);

			// Every type of hornet AND moss hornet can drop Bezoar
			if (CalamityMod.hornetList.Contains(npc.type) || CalamityMod.mossHornetList.Contains(npc.type))
				DropHelper.DropItemChance(npc, ItemID.Bezoar, DropHelper.DefiledDropRateInt);

			// Every type of moss hornet can drop Tattered Bee Wings
			if (CalamityMod.mossHornetList.Contains(npc.type))
				DropHelper.DropItemChance(npc, ItemID.TatteredBeeWing, DropHelper.DefiledDropRateInt);

			// Because all switch cases must be constant at compile time, modded NPC IDs (which can change) can't be included.
			if (npc.type == mod.NPCType("SunBat"))
				DropHelper.DropItemChance(npc, ItemID.HelFire, DropHelper.DefiledDropRateInt);

			else if (npc.type == mod.NPCType("Cryon"))
				DropHelper.DropItemChance(npc, ItemID.Amarok, DropHelper.DefiledDropRateInt);
		}
		#endregion

		#region Armageddon Loot
		private void ArmageddonLoot(NPC npc, Mod mod)
		{
			switch (npc.type)
			{
				default:
					break;
				case NPCID.KingSlime:
				case NPCID.EyeofCthulhu:
				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
					if (npc.boss) // only drop from the 1 "boss" segment (redcode)
						DropHelper.DropArmageddonBags(npc);
					break;
				case NPCID.BrainofCthulhu:
				case NPCID.QueenBee:
				case NPCID.SkeletronHead:
				case NPCID.WallofFlesh:
				case NPCID.Retinazer: // only drop if spaz is already dead
					if (!NPC.AnyNPCs(NPCID.Spazmatism))
						DropHelper.DropArmageddonBags(npc);
					break;
				case NPCID.Spazmatism: // only drop if ret is already dead
					if (!NPC.AnyNPCs(NPCID.Retinazer))
						DropHelper.DropArmageddonBags(npc);
					break;
				case NPCID.TheDestroyer:
				case NPCID.SkeletronPrime:
				case NPCID.Plantera:
				case NPCID.Golem:
				case NPCID.DD2Betsy:
				case NPCID.DukeFishron:
				case NPCID.MoonLordCore:
					DropHelper.DropArmageddonBags(npc);
					break;
			}
		}
		#endregion

		#region Check Boss Spawn
		// not really drop code
		private void CheckBossSpawn(NPC npc, Mod mod)
		{
			if ((npc.type == mod.NPCType("PhantomSpirit") || npc.type == mod.NPCType("PhantomSpiritS") || npc.type == mod.NPCType("PhantomSpiritM") ||
				npc.type == mod.NPCType("PhantomSpiritL")) && !NPC.AnyNPCs(mod.NPCType("Polterghast")) && !CalamityWorld.downedPolterghast)
			{
				CalamityMod.ghostKillCount++;
				if (CalamityMod.ghostKillCount == 10)
				{
					string key = "Mods.CalamityMod.GhostBossText2";
					Color messageColor = Color.Cyan;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				else if (CalamityMod.ghostKillCount == 20)
				{
					string key = "Mods.CalamityMod.GhostBossText3";
					Color messageColor = Color.Cyan;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				if (CalamityMod.ghostKillCount >= 30 && Main.netMode != 1)
				{
					int lastPlayer = npc.lastInteraction;

					if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
						lastPlayer = npc.FindClosestPlayer();

					if (lastPlayer >= 0)
					{
						NPC.SpawnOnPlayer(lastPlayer, mod.NPCType("Polterghast"));
						CalamityMod.ghostKillCount = 0;
					}
				}
			}

			if ((NPC.downedPlantBoss || CalamityWorld.downedCalamitas) && npc.type == NPCID.SandShark && !NPC.AnyNPCs(mod.NPCType("GreatSandShark")))
			{
				CalamityMod.sharkKillCount++;
				if (CalamityMod.sharkKillCount == 4)
				{
					string key = "Mods.CalamityMod.SandSharkText";
					Color messageColor = Color.Goldenrod;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				else if (CalamityMod.sharkKillCount == 8)
				{
					string key = "Mods.CalamityMod.SandSharkText2";
					Color messageColor = Color.Goldenrod;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				if (CalamityMod.sharkKillCount >= 10 && Main.netMode != 1)
				{
					if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
					{
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MaulerRoar"),
							(int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
					}

					int lastPlayer = npc.lastInteraction;

					if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
						lastPlayer = npc.FindClosestPlayer();

					if (lastPlayer >= 0)
					{
						NPC.SpawnOnPlayer(lastPlayer, mod.NPCType("GreatSandShark"));
						CalamityMod.sharkKillCount = -5;
					}
				}
			}

			if (NPC.downedAncientCultist && !CalamityWorld.downedStarGod && npc.type == mod.NPCType("Atlas") && !NPC.AnyNPCs(mod.NPCType("AstrumDeusHeadSpectral")))
			{
				CalamityMod.astralKillCount++;
				if (CalamityMod.astralKillCount == 1)
				{
					string key = "Mods.CalamityMod.DeusText2";
					Color messageColor = Color.Gold;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				else if (CalamityMod.astralKillCount == 2)
				{
					string key = "Mods.CalamityMod.DeusText3";
					Color messageColor = Color.Gold;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
				if (CalamityMod.astralKillCount >= 3 && Main.netMode != 1)
				{
					int lastPlayer = npc.lastInteraction;

					if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
						lastPlayer = npc.FindClosestPlayer();

					if (lastPlayer >= 0)
					{
						CalamityWorld.ChangeTime(false);

						for (int x = 0; x < 10; x++)
							NPC.SpawnOnPlayer(lastPlayer, mod.NPCType("AstrumDeusHead"));

						NPC.SpawnOnPlayer(lastPlayer, mod.NPCType("AstrumDeusHeadSpectral"));
						CalamityMod.astralKillCount = 0;
					}
				}
			}
		}
		#endregion

		#region Worm Boss Loot
		private void WormBossLoot(NPC npc, Mod mod)
		{
			if (npc.type == mod.NPCType("DesertScourgeHead"))
			{
				Vector2 center = Main.player[npc.target].Center;
				float num2 = 1E+08f;
				Vector2 position2 = npc.position;

				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && (Main.npc[k].type == mod.NPCType("DesertScourgeHead") || Main.npc[k].type == mod.NPCType("DesertScourgeBody") || Main.npc[k].type == mod.NPCType("DesertScourgeTail")))
					{
						float num3 = Math.Abs(Main.npc[k].Center.X - center.X) + Math.Abs(Main.npc[k].Center.Y - center.Y);
						if (num3 < num2)
						{
							num2 = num3;
							position2 = Main.npc[k].position;
						}
					}
				}

				npc.position = position2;

				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.LesserHealingPotion, Main.rand.Next(8, 15));

				if (Main.rand.Next(10) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DesertScourgeTrophy"));

				if (CalamityWorld.armageddon)
				{
					for (int i = 0; i < 5; i++)
						npc.DropBossBags();
				}

				if (Main.expertMode)
					npc.DropBossBags();
				else
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VictoryShard"), Main.rand.Next(7, 15));
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Coral, Main.rand.Next(5, 10));
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Seashell, Main.rand.Next(5, 10));
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfish, Main.rand.Next(5, 10));

					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SeaboundStaff"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Barinade"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StormSpray"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AquaticDischarge"));
					if (Main.rand.Next(4) == 0)
					{
						if (Main.rand.Next(10) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DuneHopper"));
						else
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ScourgeoftheDesert"));
					}
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DeepDiver"));
					if (Main.rand.Next(7) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DesertScourgeMask"));
					if (Main.rand.Next(15) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HighTestFishingLine);
					if (Main.rand.Next(15) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerTackleBag);
					if (Main.rand.Next(15) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TackleBox);
					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerEarring);
					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishermansGuide);
					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.WeatherRadio);
					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Sextant);
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerHat);
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerVest);
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerPants);
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CratePotion, Main.rand.Next(2, 4));
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishingPotion, Main.rand.Next(2, 4));
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SonarPotion, Main.rand.Next(2, 4));
					if (Main.rand.Next(10) == 0)
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("AeroStone"), 1, true);
					if (NPC.downedBoss3)
					{
						if (Main.rand.Next(20) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GoldenBugNet);
					}
				}

				if (!CalamityWorld.downedDesertScourge)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge"), 1, true);
				}

				string key = "Mods.CalamityMod.OpenSunkenSea";
				Color messageColor = Color.Aquamarine;

				if (!CalamityWorld.downedDesertScourge)
				{
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				CalamityWorld.downedDesertScourge = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("AquaticScourgeHead"))
			{
				Vector2 center = Main.player[npc.target].Center;
				float num2 = 1E+08f;
				Vector2 position2 = npc.position;

				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active &&
						(Main.npc[k].type == mod.NPCType("AquaticScourgeHead") ||
						Main.npc[k].type == mod.NPCType("AquaticScourgeBody") ||
						Main.npc[k].type == mod.NPCType("AquaticScourgeBodyAlt") ||
						Main.npc[k].type == mod.NPCType("AquaticScourgeTail")))
					{
						float num3 = Math.Abs(Main.npc[k].Center.X - center.X) + Math.Abs(Main.npc[k].Center.Y - center.Y);
						if (num3 < num2)
						{
							num2 = num3;
							position2 = Main.npc[k].position;
						}
					}
				}

				npc.position = position2;

				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GreaterHealingPotion, Main.rand.Next(8, 15));

				if (Main.hardMode)
				{
					if (CalamityWorld.armageddon)
					{
						for (int i = 0; i < 5; i++)
							npc.DropBossBags();
					}

					if (Main.expertMode)
						npc.DropBossBags();
					else
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VictoryShard"), Main.rand.Next(11, 21));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Coral, Main.rand.Next(5, 10));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Seashell, Main.rand.Next(5, 10));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfish, Main.rand.Next(5, 10));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SoulofSight, Main.rand.Next(20, 41));

						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DeepseaStaff"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Barinautical"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Downpour"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SubmarineShocker"));
						if (Main.rand.Next(12) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HighTestFishingLine);
						if (Main.rand.Next(12) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerTackleBag);
						if (Main.rand.Next(12) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TackleBox);
						if (Main.rand.Next(9) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerEarring);
						if (Main.rand.Next(9) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishermansGuide);
						if (Main.rand.Next(9) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.WeatherRadio);
						if (Main.rand.Next(9) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Sextant);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerHat);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerVest);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerPants);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CratePotion, Main.rand.Next(2, 4));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishingPotion, Main.rand.Next(2, 4));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SonarPotion, Main.rand.Next(2, 4));
						if (Main.rand.Next(9) == 0)
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("AeroStone"), 1, true);
						if (NPC.downedBoss3)
						{
							if (Main.rand.Next(15) == 0)
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GoldenBugNet);
						}
					}
				}
				else
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VictoryShard"), Main.rand.Next(11, 21));
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Coral, Main.rand.Next(5, 10));
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Seashell, Main.rand.Next(5, 10));
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Starfish, Main.rand.Next(5, 10));

					if (Main.rand.Next(12) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HighTestFishingLine);
					if (Main.rand.Next(12) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerTackleBag);
					if (Main.rand.Next(12) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TackleBox);
					if (Main.rand.Next(9) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerEarring);
					if (Main.rand.Next(9) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishermansGuide);
					if (Main.rand.Next(9) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.WeatherRadio);
					if (Main.rand.Next(9) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Sextant);
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerHat);
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerVest);
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AnglerPants);
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CratePotion, Main.rand.Next(2, 4));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FishingPotion, Main.rand.Next(2, 4));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SonarPotion, Main.rand.Next(2, 4));
					if (Main.rand.Next(9) == 0)
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("AeroStone"), 1, true);
					if (NPC.downedBoss3)
					{
						if (Main.rand.Next(15) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GoldenBugNet);
					}
				}

				if (!CalamityWorld.downedAquaticScourge)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge27"), 1, true);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge35"), 1, true);
				}

				CalamityWorld.downedAquaticScourge = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("AstrumDeusHeadSpectral"))
			{
				Vector2 center = Main.player[npc.target].Center;
				float num2 = 1E+08f;
				Vector2 position2 = npc.position;

				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && (Main.npc[k].type == mod.NPCType("AstrumDeusHeadSpectral") || Main.npc[k].type == mod.NPCType("AstrumDeusBodySpectral") || Main.npc[k].type == mod.NPCType("AstrumDeusTailSpectral")))
					{
						float num3 = Math.Abs(Main.npc[k].Center.X - center.X) + Math.Abs(Main.npc[k].Center.Y - center.Y);
						if (num3 < num2)
						{
							num2 = num3;
							position2 = Main.npc[k].position;
						}
					}
				}

				npc.position = position2;

				string key = "Mods.CalamityMod.AstralBossText";
				Color messageColor = Color.Gold;

				if (!CalamityWorld.downedStarGod)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge29"), 1, true);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge36"), 1, true);

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				CalamityWorld.downedStarGod = true;
				CalamityGlobalNPC.UpdateServerBoolean();

				int amount = Main.rand.Next(5, 8);
				if (Main.expertMode)
					amount = (int)((float)amount * 1.5f);

				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3459, Main.rand.Next(1, 4), false, 0, false, false);
				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3458, Main.rand.Next(1, 4), false, 0, false, false);
				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3457, Main.rand.Next(1, 4), false, 0, false, false);
				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, 3456, Main.rand.Next(1, 4), false, 0, false, false);

				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GreaterHealingPotion, Main.rand.Next(8, 15));

				if (Main.rand.Next(10) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AstrumDeusTrophy"));

				if (CalamityWorld.armageddon)
				{
					for (int i = 0; i < 5; i++)
						npc.DropBossBags();
				}

				if (Main.expertMode)
					npc.DropBossBags();
				else
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Stardust"), Main.rand.Next(50, 81));

					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HideofAstrumDeus"));
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Quasar"));
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Starfall"));
					if (Main.rand.Next(7) == 0)
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("AstrumDeusMask"), 1, true);
				}
			}
			else if (npc.type == mod.NPCType("DevourerofGodsHead"))
			{
				CalamityWorld.DoGSecondStageCountdown = CalamityWorld.downedDoG ? 600 : 21600; //10 seconds or 6 minutes

				if (Main.netMode == 2)
				{
					var netMessage = mod.GetPacket();
					netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
					netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
					netMessage.Send();
				}

				for (int playerIndex = 0; playerIndex < 255; playerIndex++)
				{
					if (Main.player[playerIndex].active)
					{
						Player player = Main.player[playerIndex];
						for (int l = 0; l < 22; l++)
						{
							int hasBuff = player.buffType[l];
							if (hasBuff == mod.BuffType("AdrenalineMode"))
							{
								player.DelBuff(l);
								l = -1;
							}
							if (hasBuff == mod.BuffType("RageMode"))
							{
								player.DelBuff(l);
								l = -1;
							}
						}
					}
				}
			}
			else if (npc.type == mod.NPCType("DevourerofGodsHeadS"))
			{
				Vector2 center = Main.player[npc.target].Center;
				float num2 = 1E+08f;
				Vector2 position2 = npc.position;

				for (int k = 0; k < 200; k++)
				{
					if (Main.npc[k].active && (Main.npc[k].type == mod.NPCType("DevourerofGodsHeadS") || Main.npc[k].type == mod.NPCType("DevourerofGodsBodyS") || Main.npc[k].type == mod.NPCType("DevourerofGodsTailS")))
					{
						float num3 = Math.Abs(Main.npc[k].Center.X - center.X) + Math.Abs(Main.npc[k].Center.Y - center.Y);
						if (num3 < num2)
						{
							num2 = num3;
							position2 = Main.npc[k].position;
						}
					}
				}

				npc.position = position2;

				string key = "Mods.CalamityMod.DoGBossText";
				Color messageColor = Color.Cyan;
				string key2 = "Mods.CalamityMod.DoGBossText2";
				Color messageColor2 = Color.Orange;

				if (!CalamityWorld.downedDoG)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge42"), 1, true);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 6);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 3);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"), 2);

					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
						Main.NewText(Language.GetTextValue(key2), messageColor2);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
					}
				}

				CalamityWorld.downedDoG = true;
				CalamityGlobalNPC.UpdateServerBoolean();

				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SupremeHealingPotion"), Main.rand.Next(8, 15));

				if (Main.rand.Next(10) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DevourerofGodsTrophy"));

				if (CalamityWorld.armageddon)
				{
					for (int i = 0; i < 5; i++)
						npc.DropBossBags();
				}

				if (Main.expertMode)
					npc.DropBossBags();
				else
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CosmiliteBar"), Main.rand.Next(25, 35));

					if (Main.rand.Next(7) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DevourerofGodsMask"));
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Norfleet"));
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Skullmasher"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DeathhailStaff"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Excelsus"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TheObliterator"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Eradicator"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EradicatorMelee"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Deathwind"));
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StaffoftheMechworm"));
				}
			}
		}
		#endregion

		#region Armor Set Loot
		private void ArmorSetLoot(NPC npc, Mod mod)
		{
			if (Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).tarraSet)
			{
				if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && npc.lifeMax > 100 && Main.rand.Next(5) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, 58, 1, false, 0, false, false);
			}
			if (Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).bloodflareSet)
			{
				if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && Main.rand.Next(2) == 0 && Main.bloodMoon && npc.HasPlayerTarget && (double)(npc.position.Y / 16f) < Main.worldSurface)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodOrb"));
			}

			if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && Main.rand.Next(12) == 0 && Main.bloodMoon && npc.HasPlayerTarget && (double)(npc.position.Y / 16f) < Main.worldSurface)
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodOrb"));
		}
		#endregion

		#region Rare Loot
		private void RareLoot(NPC npc, Mod mod)
		{
			bool expertMode = Main.expertMode;
			bool defiled = CalamityWorld.defiled;

			if (npc.type == NPCID.PossessedArmor)
			{
				if (expertMode)
				{
					if (Main.rand.Next(150) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PsychoticAmulet"));
				}
				else if (Main.rand.Next(200) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PsychoticAmulet"));

				if (defiled)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PsychoticAmulet"));
				}
			}
			else if (npc.type == NPCID.SeaSnail)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SeaShell"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SeaShell"));
			}
			else if (npc.type == NPCID.GreekSkeleton)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GladiatorHelmet);
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GladiatorHelmet);

				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GladiatorBreastplate);
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GladiatorBreastplate);

				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GladiatorLeggings);
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GladiatorLeggings);
			}
			else if (npc.type == NPCID.GiantTortoise)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
					{
						if (Main.rand.Next(40) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FabledTortoiseShell"));
						else
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GiantTortoiseShell"));
					}
				}
				else if (Main.rand.Next(7) == 0)
				{
					if (Main.rand.Next(29) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FabledTortoiseShell"));
					else
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GiantTortoiseShell"));
				}
			}
			else if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GiantShell"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GiantShell"));
			}
			else if (npc.type == NPCID.AnomuraFungus)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FungalCarapace"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FungalCarapace"));
			}
			else if (npc.type == NPCID.Crawdad || npc.type == NPCID.Crawdad2)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrawCarapace"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrawCarapace"));
			}
			else if (npc.type == NPCID.GreenJellyfish)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VitalJelly"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VitalJelly"));
			}
			else if (npc.type == NPCID.PinkJellyfish)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LifeJelly"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LifeJelly"));
			}
			else if (npc.type == NPCID.BlueJellyfish)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ManaJelly"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ManaJelly"));
			}
			else if (npc.type == NPCID.MossHornet)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Needler"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Needler"));
			}
			else if (npc.type == NPCID.DarkCaster)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AncientShiv"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AncientShiv"));
			}
			else if (npc.type == NPCID.BigMimicCorruption || npc.type == NPCID.BigMimicCrimson || npc.type == NPCID.BigMimicHallow || npc.type == NPCID.BigMimicJungle)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CelestialClaymore"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CelestialClaymore"));
			}
			else if (npc.type == NPCID.Clinger)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CursedDagger"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CursedDagger"));
			}
			else if (npc.type == NPCID.Shark)
			{
				if (expertMode)
				{
					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthBlade"));
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SharkToothNecklace);
				}
				else
				{
					if (Main.rand.Next(15) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DepthBlade"));
					if (Main.rand.Next(30) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SharkToothNecklace);
					if (defiled)
					{
						if (Main.rand.Next(20) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SharkToothNecklace);
					}
				}
			}
			else if (npc.type == NPCID.PresentMimic)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HolidayHalberd"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("HolidayHalberd"));
			}
			else if (npc.type == NPCID.IchorSticker)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
					{
						if (Main.rand.Next(10) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SpearofDestiny"));
						else
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("IchorSpear"));
					}
				}
				else if (Main.rand.Next(25) == 0)
				{
					if (Main.rand.Next(8) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SpearofDestiny"));
					else
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("IchorSpear"));
				}
			}
			else if (npc.type == NPCID.Harpy && NPC.downedBoss1)
			{
				if (expertMode)
				{
					if (Main.rand.Next(30) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SkyGlaze"));
				}
				else if (Main.rand.Next(40) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SkyGlaze"));

				if (defiled)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SkyGlaze"));
				}
			}
			else if (npc.type == NPCID.Antlion || npc.type == NPCID.WalkingAntlion || npc.type == NPCID.FlyingAntlion)
			{
				if (expertMode)
				{
					if (Main.rand.Next(30) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MandibleBow"));
					if (Main.rand.Next(30) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MandibleClaws"));
				}
				else
				{
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MandibleBow"));
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MandibleClaws"));
				}
			}
			else if (npc.type == NPCID.TombCrawlerHead)
			{
				if (expertMode)
				{
					if (Main.rand.Next(15) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BurntSienna"));
				}
				else
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BurntSienna"));
				}
			}
			else if (npc.type == NPCID.DuneSplicerHead && NPC.downedPlantBoss)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Terracotta"));
				}
				else
				{
					if (Main.rand.Next(30) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Terracotta"));
				}
			}
			else if (npc.type == NPCID.MartianSaucerCore)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("NullificationRifle"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("NullificationRifle"));
			}
			else if (npc.type == NPCID.Demon)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BladecrestOathsword"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BladecrestOathsword"));

				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DemonicBoneAsh"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DemonicBoneAsh"));
			}
			else if (npc.type == NPCID.BoneSerpentHead)
			{
				if (expertMode)
				{
					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("OldLordOathsword"));
				}
				else if (Main.rand.Next(15) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("OldLordOathsword"));

				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DemonicBoneAsh"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DemonicBoneAsh"));
			}
			else if (npc.type == NPCID.Tim)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlasmaRod"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlasmaRod"));
			}
			else if (npc.type == NPCID.GoblinSorcerer)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlasmaRod"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PlasmaRod"));
			}
			else if (npc.type == NPCID.PirateDeadeye)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ProporsePistol"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ProporsePistol"));
			}
			else if (npc.type == NPCID.PirateCrossbower)
			{
				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
					{
						if (Main.rand.Next(10) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Arbalest"));
						else
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RaidersGlory"));
					}
				}
				else if (Main.rand.Next(25) == 0)
				{
					if (Main.rand.Next(8) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Arbalest"));
					else
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RaidersGlory"));
				}
			}
			else if (npc.type == NPCID.GoblinSummoner)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TheFirstShadowflame"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TheFirstShadowflame"));
			}
			else if (npc.type == NPCID.SandElemental)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WifeinaBottle"));
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WifeinaBottlewithBoobs"));
				}
				else
				{
					if (Main.rand.Next(7) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WifeinaBottle"));
				}
			}
			else if (CalamityMod.skeletonList.Contains(npc.type))
			{
				if (!Main.hardMode)
				{
					if (expertMode)
					{
						if (Main.rand.Next(15) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Waraxe"));
					}
					else if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Waraxe"));
				}

				if (expertMode)
				{
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AncientBoneDust"));
				}
				else if (Main.rand.Next(5) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AncientBoneDust"));
			}
			else if (npc.type == NPCID.GoblinWarrior)
			{
				if (expertMode)
				{
					if (Main.rand.Next(15) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Warblade"));
				}
				else if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Warblade"));
			}
			else if (npc.type == NPCID.MartianWalker)
			{
				if (expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Wingman"));
				}
				else if (Main.rand.Next(7) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Wingman"));
			}
			else if (npc.type == NPCID.GiantCursedSkull || npc.type == NPCID.NecromancerArmored || npc.type == NPCID.Necromancer)
			{
				if (npc.type == NPCID.GiantCursedSkull)
				{
					if (CalamityWorld.downedLeviathan && Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Keelhaul"));
				}

				if (expertMode)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WrathoftheAncients"));
				}
				else if (Main.rand.Next(25) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("WrathoftheAncients"));
			}
		}
		#endregion

		#region Common Loot
		private void CommonLoot(NPC npc, Mod mod)
		{
			bool expertMode = Main.expertMode;

			if (npc.type == NPCID.Vulture)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DesertFeather"), Main.rand.Next(1, 3));
				}
				else if (Main.rand.Next(2) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DesertFeather"));
			}
			else if (CalamityMod.dungeonEnemyBuffList.Contains(npc.type))
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Ectoblood"), Main.rand.Next(1, 3));
				}
				else if (Main.rand.Next(2) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Ectoblood"));
			}
			else if (npc.type == NPCID.RedDevil)
			{
				if (expertMode)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"));
				else if (Main.rand.Next(2) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofChaos"));
			}
			else if (npc.type == NPCID.WyvernHead)
			{
				if (expertMode)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofCinder"), Main.rand.Next(1, 3));
				else
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofCinder"));
			}
			else if (npc.type == NPCID.AngryNimbus)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofCinder"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofCinder"));
			}
			else if (npc.type == NPCID.IceTortoise || npc.type == NPCID.IcyMerman)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofEleum"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofEleum"));
			}
			else if (npc.type == NPCID.IceGolem)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EssenceofEleum"), Main.rand.Next(1, 3));
			}
			else if (npc.type == NPCID.Plantera)
			{
				if (!expertMode)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("LivingShard"), Main.rand.Next(6, 10));
			}
			else if (npc.type == NPCID.NebulaBrain || npc.type == NPCID.NebulaSoldier || npc.type == NPCID.NebulaHeadcrab || npc.type == NPCID.NebulaBeast)
			{
				if (expertMode)
				{
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MeldBlob"), Main.rand.Next(2, 4));
				}
				else if (Main.rand.Next(4) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MeldBlob"), (Main.rand.Next(1, 3)));
			}
			else if (npc.type == NPCID.CultistBoss)
			{
				if (!expertMode)
				{
					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StardustStaff"));
				}
				else
				{
					if (Main.rand.Next(3) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StardustStaff"));
				}

				if (Main.rand.Next(40) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ThornBlossom"));
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				if (!expertMode)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("VictoryShard"), Main.rand.Next(2, 5));

					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TeardropCleaver"));
				}
			}
			else if (npc.type == NPCID.DevourerHead || npc.type == NPCID.SeekerHead)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FetidEssence"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("FetidEssence"));
			}
			else if (npc.type == NPCID.FaceMonster || npc.type == NPCID.Herpling)
			{
				if (expertMode)
				{
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodlettingEssence"));
				}
				else if (Main.rand.Next(5) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BloodlettingEssence"));
			}
			else if (npc.type == NPCID.ManEater)
			{
				if (expertMode)
				{
					if (Main.rand.Next(2) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ManeaterBulb"));
				}
				else if (Main.rand.Next(3) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ManeaterBulb"));
			}
			else if (npc.type == NPCID.AngryTrapper)
			{
				if (expertMode)
				{
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TrapperBulb"));
				}
				else if (Main.rand.Next(5) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TrapperBulb"));
			}
			else if (npc.type == NPCID.MotherSlime || npc.type == NPCID.Crimslime || npc.type == NPCID.CorruptSlime)
			{
				if (expertMode)
				{
					if (Main.rand.Next(3) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MurkySludge"));
				}
				else if (Main.rand.Next(4) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MurkySludge"));
			}
			else if (npc.type == NPCID.Moth)
			{
				if (expertMode)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GypsyPowder"));
				else if (Main.rand.Next(2) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GypsyPowder"));
			}
			else if (npc.type == NPCID.Derpling)
			{
				if (expertMode)
				{
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BeetleJuice"));
				}
				else if (Main.rand.Next(5) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("BeetleJuice"));
			}
			else if (npc.type == NPCID.SpikedJungleSlime || npc.type == NPCID.Arapaima)
			{
				if (expertMode)
				{
					if (Main.rand.Next(4) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MurkyPaste"));
				}
				else if (Main.rand.Next(5) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MurkyPaste"));
			}
		}
		#endregion

		#region Boss Loot
		private void BossLoot(NPC npc, Mod mod)
		{
			bool expertMode = Main.expertMode;
			bool revenge = CalamityWorld.revenge;
			bool death = CalamityWorld.death;

			if (npc.boss && !CalamityWorld.downedBossAny)
			{
				CalamityWorld.downedBossAny = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}

			if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail || npc.type == NPCID.BrainofCthulhu)
			{
				if (npc.boss)
				{
					bool downedEvil = CalamityWorld.downedWhar;
					CalamityWorld.downedWhar = true;
					CalamityGlobalNPC.UpdateServerBoolean();

					if (!downedEvil)
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);

						if (WorldGen.crimson)
						{
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge8"), 1, true);
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge11"), 1, true);
						}
						else
						{
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge9"), 1, true);
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge12"), 1, true);
						}
					}
				}
			}
			else if (npc.type == NPCID.SkeletronHead)
			{
				if (!expertMode)
				{
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ClothiersWrath"));
				}

				bool downedSkull = CalamityWorld.downedSkullHead;
				CalamityWorld.downedSkullHead = true;
				CalamityGlobalNPC.UpdateServerBoolean();

				if (!downedSkull)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 3);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge17"), 1, true);
				}
			}
			else if (npc.type == NPCID.WallofFlesh)
			{
				if (WorldGenerationMethods.checkAstralMeteor())
				{
					string key = "Mods.CalamityMod.AstralText";
					Color messageColor = Color.Gold;

					if (!CalamityWorld.spawnAstralMeteor)
					{
						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

						CalamityWorld.spawnAstralMeteor = true;
						CalamityGlobalNPC.UpdateServerBoolean();
						WorldGenerationMethods.dropAstralMeteor();
					}
					else if (Main.rand.Next(2) == 0 && !CalamityWorld.spawnAstralMeteor2)
					{
						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

						CalamityWorld.spawnAstralMeteor2 = true;
						CalamityGlobalNPC.UpdateServerBoolean();
						WorldGenerationMethods.dropAstralMeteor();
					}
					else if (Main.rand.Next(4) == 0 && !CalamityWorld.spawnAstralMeteor3)
					{
						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

						CalamityWorld.spawnAstralMeteor3 = true;
						CalamityGlobalNPC.UpdateServerBoolean();
						WorldGenerationMethods.dropAstralMeteor();
					}
				}

				if (!expertMode)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MLGRune"));

					if (Main.rand.Next(5) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Meowthrower"));
					if (Main.rand.Next(8) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("RogueEmblem"));

					if (Main.rand.Next(5) == 0)
					{
						switch (Main.rand.Next(2))
						{
							case 0:
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CrimsonKey);
								break;
							case 1:
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CorruptionKey);
								break;
						}
					}
				}

				bool hardMode = CalamityWorld.downedUgly;
				CalamityWorld.downedUgly = true;
				CalamityGlobalNPC.UpdateServerBoolean();

				string key2 = "Mods.CalamityMod.UglyBossText";
				Color messageColor2 = Color.Aquamarine;

				if (!hardMode)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 3);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge7"), 1, true);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge18"), 1, true);

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key2), messageColor2);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
				}
			}
			else if (npc.type == NPCID.SkeletronPrime || npc.type == mod.NPCType("BrimstoneElemental"))
			{
				bool downedPrime = CalamityWorld.downedSkeletor;
				if (npc.type == NPCID.SkeletronPrime)
				{
					CalamityWorld.downedSkeletor = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}

				string key = "Mods.CalamityMod.SteelSkullBossText";
				Color messageColor = Color.Crimson;

				if (!downedPrime && !CalamityWorld.downedBrimstoneElemental)
				{
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				if (npc.type == mod.NPCType("BrimstoneElemental"))
				{
					if (!CalamityWorld.downedBrimstoneElemental)
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge6"), 1, true);
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge26"), 1, true);
					}

					CalamityWorld.downedBrimstoneElemental = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}
			}
			else if (npc.type == NPCID.Plantera || npc.type == mod.NPCType("CalamitasRun3"))
			{
				bool downedPlant = CalamityWorld.downedPlantThing;

				if (npc.type == NPCID.Plantera)
				{
					if (!expertMode)
					{
						if (Main.rand.Next(5) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.JungleKey);
					}

					CalamityWorld.downedPlantThing = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}

				string key = "Mods.CalamityMod.PlantBossText";
				Color messageColor = Color.RoyalBlue;
				string key2 = "Mods.CalamityMod.PlantOreText";
				Color messageColor2 = Color.GreenYellow;

				if (npc.type == mod.NPCType("CalamitasRun3"))
				{
					if (!CalamityWorld.downedCalamitas && !downedPlant)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}

					npc.DropItemInstanced(npc.position, npc.Size, ItemID.BrokenHeroSword, 1, true);
					if (!CalamityWorld.downedCalamitas)
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge24"), 1, true);
					}

					CalamityWorld.downedCalamitas = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}

				if (npc.type == NPCID.Plantera)
				{
					if (!downedPlant)
					{
						WorldGenerationMethods.SpawnOre(mod.TileType("PerennialOre"), 12E-05, .5f, .7f);

						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key2), messageColor2);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
					}

					if (!downedPlant && !CalamityWorld.downedCalamitas)
					{
						if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
							Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}
				}
			}
			else if (npc.type == NPCID.Golem)
			{
				bool downedIdiot = CalamityWorld.downedGolemBaby;
				CalamityWorld.downedGolemBaby = true;
				CalamityGlobalNPC.UpdateServerBoolean();

				string key = "Mods.CalamityMod.BabyBossText";
				Color messageColor = Color.Lime;
				string key2 = "Mods.CalamityMod.BabyBossText2";
				Color messageColor2 = Color.Yellow;

				if (!downedIdiot)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge31"), 1, true);
					npc.DropItemInstanced(npc.position, npc.Size, ItemID.Picksaw, 1, true);

					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
						Main.NewText(Language.GetTextValue(key2), messageColor2);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
					}
				}
			}
			else if (npc.type == NPCID.MoonLordCore)
			{
				bool downedMoonDude = CalamityWorld.downedMoonDude;
				CalamityWorld.downedMoonDude = true;
				CalamityGlobalNPC.UpdateServerBoolean();

				string key = "Mods.CalamityMod.MoonBossText";
				Color messageColor = Color.Orange;
				string key2 = "Mods.CalamityMod.MoonBossText2";
				Color messageColor2 = Color.Violet;
				string key3 = "Mods.CalamityMod.MoonBossText3";
				Color messageColor3 = Color.Crimson;
				string key4 = "Mods.CalamityMod.ProfanedBossText2";
				Color messageColor4 = Color.Cyan;
				string key5 = "Mods.CalamityMod.FutureOreText";
				Color messageColor5 = Color.LightGray;

				if (!expertMode)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("MLGRune2"), 1, true);

					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Infinity"));
					if (Main.rand.Next(40) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrandDad"));
				}

				if (!downedMoonDude)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge37"), 1, true);
					WorldGenerationMethods.SpawnOre(mod.TileType("ExodiumOre"), 12E-05, .01f, .07f);

					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
						Main.NewText(Language.GetTextValue(key2), messageColor2);
						Main.NewText(Language.GetTextValue(key3), messageColor3);
						Main.NewText(Language.GetTextValue(key4), messageColor4);
						Main.NewText(Language.GetTextValue(key5), messageColor5);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key3), messageColor3);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key4), messageColor4);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key5), messageColor5);
					}
				}
			}
			else if (npc.type == NPCID.DD2Betsy)
			{
				if (!CalamityWorld.downedBetsy)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				}

				CalamityWorld.downedBetsy = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == NPCID.Pumpking && CalamityWorld.downedDoG)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("NightmareFuel"), Main.rand.Next(10, 21));
			}
			else if (npc.type == NPCID.IceQueen && CalamityWorld.downedDoG)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EndothermicEnergy"), Main.rand.Next(20, 41));
			}
			else if (npc.type == NPCID.Mothron && CalamityWorld.buffedEclipse)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("DarksunFragment"), Main.rand.Next(10, 21));

				CalamityWorld.downedBuffedMothron = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("Astrageldon"))
			{
				if (WorldGenerationMethods.checkAstralMeteor())
				{
					string key = "Mods.CalamityMod.AstralText";
					Color messageColor = Color.Gold;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

					WorldGenerationMethods.dropAstralMeteor();
				}

				if (!CalamityWorld.downedAstrageldon)
				{
					string key = "Mods.CalamityMod.AureusBossText";
					string key2 = "Mods.CalamityMod.AureusBossText2";
					Color messageColor = Color.Gold;

					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key), messageColor);
						Main.NewText(Language.GetTextValue(key2), messageColor);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor);
					}

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge30"), 1, true);
				}

				CalamityWorld.downedAstrageldon = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("HiveMindP2"))
			{
				if (!CalamityWorld.downedHiveMind)
				{
					if (!CalamityWorld.downedPerforator)
					{
						string key = "Mods.CalamityMod.SkyOreText";
						Color messageColor = Color.Cyan;
						WorldGenerationMethods.SpawnOre(mod.TileType("AerialiteOre"), 12E-05, .4f, .6f);

						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge14"), 1, true);
				}

				CalamityWorld.downedHiveMind = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("PerforatorHive"))
			{
				if (!CalamityWorld.downedPerforator)
				{
					if (!CalamityWorld.downedHiveMind)
					{
						string key = "Mods.CalamityMod.SkyOreText";
						Color messageColor = Color.Cyan;
						WorldGenerationMethods.SpawnOre(mod.TileType("AerialiteOre"), 12E-05, .4f, .6f);

						if (Main.netMode == 0)
							Main.NewText(Language.GetTextValue(key), messageColor);
						else if (Main.netMode == 2)
							NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
					}

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge13"), 1, true);
				}

				CalamityWorld.downedPerforator = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("SlimeGodCore") || npc.type == mod.NPCType("SlimeGodSplit") || npc.type == mod.NPCType("SlimeGodRunSplit"))
			{
				if (npc.type == mod.NPCType("SlimeGodCore") && !NPC.AnyNPCs(mod.NPCType("SlimeGodSplit")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRunSplit")) && !NPC.AnyNPCs(mod.NPCType("SlimeGod")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRun")))
				{
					if (!CalamityWorld.downedSlimeGod)
					{
						if (revenge && !Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).revJamDrop)
						{
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("PurifiedJam"), Main.rand.Next(6, 9), true);
							Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).revJamDrop = true;
						}

						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 3);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"));
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge15"), 1, true);
					}

					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodTrophy"));

					if (CalamityWorld.armageddon)
					{
						for (int i = 0; i < 5; i++)
							npc.DropBossBags();
					}

					if (Main.expertMode)
						npc.DropBossBags();
					else
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StaticRefiner"));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PurifiedGel"), Main.rand.Next(25, 41));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Gel, Main.rand.Next(180, 251));

						int maskChoice = Main.rand.Next(2);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("OverloadedBlaster"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GelDart"), Main.rand.Next(80, 101));
						if (Main.rand.Next(7) == 0)
						{
							if (maskChoice == 0)
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodMask"));
							else
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodMask2"));
						}
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AbyssalTome"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EldritchTome"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrimslimeStaff"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CorroslimeStaff"));
					}

					CalamityWorld.downedSlimeGod = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}
				else if (npc.type == mod.NPCType("SlimeGodSplit") && !NPC.AnyNPCs(mod.NPCType("SlimeGodCore")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodRunSplit")) && NPC.CountNPCS(mod.NPCType("SlimeGodSplit")) < 2 && !NPC.AnyNPCs(mod.NPCType("SlimeGodRun")))
				{
					if (!CalamityWorld.downedSlimeGod)
					{
						if (revenge && !Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).revJamDrop)
						{
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("PurifiedJam"), Main.rand.Next(6, 9), true);
							Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).revJamDrop = true;
						}

						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 3);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"));
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge15"), 1, true);
					}

					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodTrophy"));

					if (CalamityWorld.armageddon)
					{
						for (int i = 0; i < 5; i++)
							npc.DropBossBags();
					}

					if (Main.expertMode)
						npc.DropBossBags();
					else
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StaticRefiner"));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PurifiedGel"), Main.rand.Next(25, 41));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Gel, Main.rand.Next(180, 251));

						int maskChoice = Main.rand.Next(2);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("OverloadedBlaster"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GelDart"), Main.rand.Next(80, 101));
						if (Main.rand.Next(7) == 0)
						{
							if (maskChoice == 0)
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodMask"));
							else
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodMask2"));
						}
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AbyssalTome"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EldritchTome"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrimslimeStaff"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CorroslimeStaff"));
					}

					CalamityWorld.downedSlimeGod = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}
				else if (npc.type == mod.NPCType("SlimeGodRunSplit") && !NPC.AnyNPCs(mod.NPCType("SlimeGodCore")) && !NPC.AnyNPCs(mod.NPCType("SlimeGodSplit")) && NPC.CountNPCS(mod.NPCType("SlimeGodRunSplit")) < 2 && !NPC.AnyNPCs(mod.NPCType("SlimeGod")))
				{
					if (!CalamityWorld.downedSlimeGod)
					{
						if (revenge && !Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).revJamDrop)
						{
							npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("PurifiedJam"), Main.rand.Next(6, 9), true);
							Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).revJamDrop = true;
						}

						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 3);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"));
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge15"), 1, true);
					}

					if (Main.rand.Next(10) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodTrophy"));

					if (CalamityWorld.armageddon)
					{
						for (int i = 0; i < 5; i++)
							npc.DropBossBags();
					}

					if (Main.expertMode)
						npc.DropBossBags();
					else
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("StaticRefiner"));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PurifiedGel"), Main.rand.Next(25, 41));
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Gel, Main.rand.Next(180, 251));

						int maskChoice = Main.rand.Next(2);
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("OverloadedBlaster"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GelDart"), Main.rand.Next(80, 101));
						if (Main.rand.Next(7) == 0)
						{
							if (maskChoice == 0)
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodMask"));
							else
								Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("SlimeGodMask2"));
						}
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("AbyssalTome"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EldritchTome"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CrimslimeStaff"));
						if (Main.rand.Next(4) == 0)
							Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("CorroslimeStaff"));
					}

					CalamityWorld.downedSlimeGod = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}
			}
			else if (npc.type == mod.NPCType("Cryogen"))
			{
				if (!CalamityWorld.downedCryogen)
				{
					string key = "Mods.CalamityMod.IceOreText";
					Color messageColor = Color.LightSkyBlue;
					WorldGenerationMethods.SpawnOre(mod.TileType("CryonicOre"), 15E-05, .45f, .65f);

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge19"), 1, true);
				}

				CalamityWorld.downedCryogen = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("Siren") || npc.type == mod.NPCType("Leviathan"))
			{
				int bossType = (npc.type == mod.NPCType("Siren")) ? mod.NPCType("Leviathan") : mod.NPCType("Siren");

				if (!NPC.AnyNPCs(bossType))
				{
					if (!CalamityWorld.downedLeviathan)
					{
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge10"), 1, true);
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge28"), 1, true);
					}

					CalamityWorld.downedLeviathan = true;
					CalamityGlobalNPC.UpdateServerBoolean();
				}
			}
			else if (npc.type == mod.NPCType("PlaguebringerGoliath"))
			{
				if (!CalamityWorld.downedPlaguebringer)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge32"), 1, true);
				}

				CalamityWorld.downedPlaguebringer = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("ProfanedGuardianBoss"))
			{
				if (!CalamityWorld.downedGuardians)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge38"), 1, true);
				}

				CalamityWorld.downedGuardians = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("Providence"))
			{
				string key2 = "Mods.CalamityMod.ProfanedBossText3";
				Color messageColor2 = Color.Orange;
				string key3 = "Mods.CalamityMod.TreeOreText";
				Color messageColor3 = Color.LightGreen;

				if (!CalamityWorld.downedProvidence)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge39"), 1, true);
					WorldGenerationMethods.SpawnOre(mod.TileType("UelibloomOre"), 15E-05, .4f, .8f);

					if (Main.netMode == 0)
					{
						Main.NewText(Language.GetTextValue(key2), messageColor2);
						Main.NewText(Language.GetTextValue(key3), messageColor3);
					}
					else if (Main.netMode == 2)
					{
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key3), messageColor3);
					}
				}

				CalamityWorld.downedProvidence = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("CeaselessVoid"))
			{
				if (!CalamityWorld.downedSentinel1)
				{
					if (CalamityWorld.downedSentinel2 && CalamityWorld.downedSentinel3)
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge40"), 1, true);

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				}

				CalamityWorld.downedSentinel1 = true; //21600
				CalamityGlobalNPC.UpdateServerBoolean();

				if (CalamityWorld.DoGSecondStageCountdown > 14460)
				{
					CalamityWorld.DoGSecondStageCountdown = 14460;
					if (Main.netMode == 2)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
						netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
						netMessage.Send();
					}
				}
			}
			else if (npc.type == mod.NPCType("StormWeaverHeadNaked"))
			{
				if (!CalamityWorld.downedSentinel2)
				{
					if (CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel3)
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge40"), 1, true);

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				}

				CalamityWorld.downedSentinel2 = true; //21600
				CalamityGlobalNPC.UpdateServerBoolean();

				if (CalamityWorld.DoGSecondStageCountdown > 7260)
				{
					CalamityWorld.DoGSecondStageCountdown = 7260;
					if (Main.netMode == 2)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
						netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
						netMessage.Send();
					}
				}
			}
			else if (npc.type == mod.NPCType("CosmicWraith"))
			{
				if (!CalamityWorld.downedSentinel3)
				{
					if (CalamityWorld.downedSentinel1 && CalamityWorld.downedSentinel2)
						npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge40"), 1, true);

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				}

				CalamityWorld.downedSentinel3 = true; //21600
				CalamityGlobalNPC.UpdateServerBoolean();

				if (CalamityWorld.DoGSecondStageCountdown > 600)
				{
					CalamityWorld.DoGSecondStageCountdown = 600;
					if (Main.netMode == 2)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
						netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
						netMessage.Send();
					}
				}
			}
			else if (npc.type == mod.NPCType("Bumblefuck"))
			{
				if (!CalamityWorld.downedBumble)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge43"), 1, true);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 5);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				}

				CalamityWorld.downedBumble = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("SupremeCalamitas"))
			{
				if (!CalamityWorld.downedSCal)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge45"), 1, true);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 6);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 3);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"), 2);
				}

				CalamityWorld.downedSCal = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("CrabulonIdle"))
			{
				if (!CalamityWorld.downedCrabulon)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge5"), 1, true);
				}

				CalamityWorld.downedCrabulon = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("ScavengerBody"))
			{
				if (!CalamityWorld.downedScavenger)
				{
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge33"), 1, true);
				}

				CalamityWorld.downedScavenger = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("Polterghast"))
			{
				string key = "Mods.CalamityMod.GhostBossText";
				Color messageColor = Color.RoyalBlue;

				if (!CalamityWorld.downedPolterghast)
				{
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge41"), 1, true);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 6);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 3);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"), 2);

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}

				CalamityWorld.downedPolterghast = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			else if (npc.type == mod.NPCType("GiantClam"))
			{
				CalamityWorld.downedCLAM = true;
				CalamityGlobalNPC.UpdateServerBoolean();
			}
			/*else if (npc.type == mod.NPCType("OldDuke"))
            {
                CalamityWorld.downedOldDuke = true;
            }*/
			if (death)
			{
				if (npc.type == mod.NPCType("SupremeCalamitas"))
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Levi"));
			}
		}
		#endregion

		#region DespawnHostileProjectiles
		public void DespawnProj()
		{
			int proj;
			for (int x = 0; x < 1000; x = proj + 1)
			{
				Projectile projectile = Main.projectile[x];

				if (projectile.active && projectile.hostile && !projectile.friendly && projectile.damage > 0)
					projectile.Kill();

				proj = x;
			}
		}
		#endregion
	}
}