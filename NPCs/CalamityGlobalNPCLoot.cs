using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.World;
using CalamityMod.CalPlayer;
using CalamityMod.Utilities;

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
				if (npc.type == NPCID.Probe || npc.type == NPCID.ServantofCthulhu)
					return false;
			}

			bool expert = Main.expertMode;
			bool rev = CalamityWorld.revenge;
			bool death = CalamityWorld.death;
			bool defiled = CalamityWorld.defiled;

			// Determine whether this NPC is the second Twin killed in a fight, regardless of which Twin it is.
			bool lastTwinStanding = false;
			if (npc.type == NPCID.Retinazer)
				lastTwinStanding = !NPC.AnyNPCs(NPCID.Spazmatism);
			else if (npc.type == NPCID.Spazmatism)
				lastTwinStanding = !NPC.AnyNPCs(NPCID.Retinazer);

			// Mechanical Bosses' combined lore item
			bool mechLore = !NPC.downedMechBossAny && (lastTwinStanding || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime);
			DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeMechs"), true, mechLore);

			// King Slime
			if (npc.type == NPCID.KingSlime)
			{
				// Drop a huge spray of Gel items
				int minGel = expert ? 90 : 60;
				int maxGel = expert ? 120 : 80;
				DropHelper.DropItemSpray(npc, ItemID.Gel, minGel, maxGel, 2);

				// King Slime doesn't have a lore item yet
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedSlimeKing, 2, 0, 0);
			}

			// Eye of Cthulhu
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeEyeofCthulhu"), true, !NPC.downedBoss1);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss1, 2, 0, 0);
			}

			// Eater of Worlds + Brain of Cthulhu (ignore non-boss segments of EoW)
			else if ((npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)) || npc.type == NPCID.BrainofCthulhu)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeCorruption"), true, !WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeEaterofWorlds"), true, !WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeCrimson"), true, WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeBrainofCthulhu"), true, WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss2, 2, 0, 0);
			}

			// Queen Bee
			else if (npc.type == NPCID.QueenBee)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeQueenBee"), true, !NPC.downedQueenBee);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedQueenBee, 2, 0, 0);
			}

			// Skeletron
			else if (npc.type == NPCID.SkeletronHead)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("ClothiersWrath"), !expert, DropHelper.RareVariantDropRateInt, 1, 1);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeSkeletron"), true, !NPC.downedBoss3);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedBoss3, 3, 1, 0);
			}

			// Wall of Flesh
			else if (npc.type == NPCID.WallofFlesh)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("MLGRune"), !expert); // Demon Trophy
				DropHelper.DropItemCondition(npc, mod.ItemType("Meowthrower"), !expert, 5, 1, 1);
				DropHelper.DropItemCondition(npc, mod.ItemType("RogueEmblem"), !expert, 8, 1, 1);
				DropHelper.DropItemChance(npc, mod.ItemType("IbarakiBox"), !Main.hardMode, Main.hardMode ? 0.1f : 1f); // 100% chance on first kill, 10% chance afterwards
				DropHelper.DropItemFromSetCondition(npc, !expert, 5, ItemID.CorruptionKey, ItemID.CrimsonKey);

				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeUnderworld"), true, !Main.hardMode);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeWallofFlesh"), true, !Main.hardMode);
				DropHelper.DropResidentEvilAmmo(npc, Main.hardMode, 3, 1, 0);

				// First kill text (this is not a loot function)
				if (!Main.hardMode)
				{
					string key2 = "Mods.CalamityMod.UglyBossText";
					Color messageColor2 = Color.Aquamarine;
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key2), messageColor2);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
				}
			}

			// Retinazer OR Spazmatism (whichever is killed last)
			else if (lastTwinStanding)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeTwins"), true, !NPC.downedMechBoss2);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss2, 4, 2, 1);
			}

			// The Destroyer
			else if (npc.type == NPCID.TheDestroyer)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeDestroyer"), true, !NPC.downedMechBoss1);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss1, 4, 2, 1);
			}

			// Skeletron Prime
			else if (npc.type == NPCID.SkeletronPrime)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeSkeletronPrime"), true, !NPC.downedMechBoss3);
				DropHelper.DropItemCondition(npc, mod.ItemType("GoldBurdenBreaker"), true, (npc.ai[1] == 2f && rev));
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMechBoss3, 4, 2, 1);

				// If neither Prime nor Brimmy have been killed, show this text (not a loot function)
				string key = "Mods.CalamityMod.SteelSkullBossText";
				Color messageColor = Color.Crimson;
				if (!NPC.downedMechBoss3 && !CalamityWorld.downedBrimstoneElemental)
				{
					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
			}

			// Plantera
			else if (npc.type == NPCID.Plantera)
			{
				DropHelper.DropItemCondition(npc, ItemID.JungleKey, !expert, 5, 1, 1);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgePlantera"), true, !NPC.downedPlantBoss);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedPlantBoss, 4, 2, 1);

				// Spawn Perennial Ore if Plantera has never been killed
				if (!NPC.downedPlantBoss)
				{
					string key2 = "Mods.CalamityMod.PlantOreText";
					Color messageColor2 = Color.GreenYellow;

					WorldGenerationMethods.SpawnOre(mod.TileType("PerennialOre"), 12E-05, .5f, .7f);

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key2), messageColor2);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key2), messageColor2);
				}

				// Awaken the Abyss if neither Plantera or Calamitas has been killed
				if (!NPC.downedPlantBoss && !CalamityWorld.downedCalamitas)
				{
					string key = "Mods.CalamityMod.PlantBossText";
					Color messageColor = Color.RoyalBlue;

					if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/WyrmScream"), (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
			}

			// Golem
			else if (npc.type == NPCID.Golem)
			{
				DropHelper.DropItemCondition(npc, ItemID.Picksaw, true, !NPC.downedGolemBoss);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeGolem"), true, !NPC.downedGolemBoss);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedGolemBoss, 4, 2, 1);

				// If Golem has never been killed, send messages about PBG and Ravager
				if (!NPC.downedGolemBoss)
				{
					string key = "Mods.CalamityMod.BabyBossText";
					Color messageColor = Color.Lime;
					string key2 = "Mods.CalamityMod.BabyBossText2";
					Color messageColor2 = Color.Yellow;

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

			// Betsy
			else if (npc.type == NPCID.DD2Betsy && !CalamityWorld.downedBetsy)
			{
				DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedBetsy, 4, 2, 1);

				// Mark Betsy as dead (Vanilla does not keep track of her)
				CalamityWorld.downedBetsy = true;
				CalamityMod.UpdateServerBoolean();
			}

			// Duke Fishron
			else if (npc.type == NPCID.DukeFishron)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeDukeFishron"), true, !NPC.downedFishron);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedPlantBoss, 4, 2, 1);
			}

			// Lunatic Cultist
			else if (npc.type == NPCID.CultistBoss)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeLunaticCultist"), true, !NPC.downedAncientCultist);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedAncientCultist, 4, 2, 1);

				// Blood Moon lore item
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeBloodMoon"), true, Main.bloodMoon);

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

			// Moon Lord
			else if (npc.type == NPCID.MoonLordCore)
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("MLGRune2"), true, !expert);
				DropHelper.DropItemCondition(npc, mod.ItemType("GrandDad"), !expert, DropHelper.RareVariantDropRateInt, 1, 1);
				DropHelper.DropItemCondition(npc, mod.ItemType("Infinity"), !expert, DropHelper.RareVariantDropRateInt, 1, 1);
				DropHelper.DropItemCondition(npc, mod.ItemType("KnowledgeMoonLord"), true, !NPC.downedMoonlord);
				DropHelper.DropResidentEvilAmmo(npc, NPC.downedMoonlord, 5, 2, 1);

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

				// Spawn Exodium and send messages about Providence, Bloodstone, Phantoplasm, etc. if ML has not been killed yet
				if (!NPC.downedMoonlord)
				{
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

				CalamityMod.UpdateServerBoolean();
				if (Main.netMode == 2)
				{
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
					CalamityMod.UpdateServerBoolean();
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

			if (NPC.downedPlantBoss && npc.type == NPCID.SandShark && !NPC.AnyNPCs(mod.NPCType("GreatSandShark")))
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

		#region Armor Set Loot
		private void ArmorSetLoot(NPC npc, Mod mod)
		{
			// Tarragon armor set bonus: 20% chance to drop hearts from all valid enemies
			if (Main.player[(int)Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).tarraSet)
			{
				if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && npc.lifeMax > 100)
					DropHelper.DropItemChance(npc, ItemID.Heart, 5);
			}

			// Blood Orb drops: Valid enemy during a blood moon on the Surface
			if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && Main.bloodMoon && npc.HasPlayerTarget && npc.position.Y / 16D < Main.worldSurface)
			{
				if (Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].GetModPlayer<CalamityPlayer>(mod).bloodflareSet)
					DropHelper.DropItemChance(npc, mod.ItemType("BloodOrb"), 2); // 50% chance of 1 orb with Bloodflare

				// 1/12 chance to get a Blood Orb with or without Bloodflare
				DropHelper.DropItemChance(npc, mod.ItemType("BloodOrb"), 12);
			}
		}
		#endregion

		#region Rare Loot
		private void RareLoot(NPC npc, Mod mod)
		{
			bool expert = Main.expertMode;
			bool defiled = CalamityWorld.defiled;

			switch (npc.type)
			{
				default:
					break;
				case NPCID.PossessedArmor:
					int amuletDropRate = defiled ? DropHelper.DefiledDropRateInt : expert ? 150 : 200;
					DropHelper.DropItemChance(npc, mod.ItemType("PsychoticAmulet"), amuletDropRate, 1, 1);
					break;
				case NPCID.SeaSnail:
					DropHelper.DropItemChance(npc, mod.ItemType("SeaShell"), expert ? 2 : 3);
					break;
				case NPCID.GreekSkeleton:
					int gladiatorDropRate = expert ? 15 : 20;
					DropHelper.DropItemChance(npc, ItemID.GladiatorHelmet, gladiatorDropRate);
					DropHelper.DropItemChance(npc, ItemID.GladiatorBreastplate, gladiatorDropRate);
					DropHelper.DropItemChance(npc, ItemID.GladiatorLeggings, gladiatorDropRate);
					break;
				case NPCID.GiantTortoise:
					int tortoiseDropRate = expert ? 5 : 7;
					float shellRoll = Main.rand.NextFloat();
					bool fabledShell = shellRoll < 0.005f; // Exact 1/200 chance for rare regardless of difficulty
					DropHelper.DropItemCondition(npc, mod.ItemType("GiantTortoiseShell"), !fabledShell, tortoiseDropRate, 1, 1);
					DropHelper.DropItemCondition(npc, mod.ItemType("FabledTortoiseShell"), fabledShell, tortoiseDropRate, 1, 1);
					break;
				case NPCID.GiantShelly:
				case NPCID.GiantShelly2:
					DropHelper.DropItemChance(npc, mod.ItemType("GiantShell"), expert ? 5 : 7);
					break;
				case NPCID.AnomuraFungus:
					DropHelper.DropItemChance(npc, mod.ItemType("FungalCarapace"), expert ? 5 : 7);
					break;
				case NPCID.Crawdad:
				case NPCID.Crawdad2:
					DropHelper.DropItemChance(npc, mod.ItemType("CrawCarapace"), expert ? 5 : 7);
					break;
				case NPCID.GreenJellyfish:
					DropHelper.DropItemChance(npc, mod.ItemType("VitalJelly"), expert ? 5 : 7);
					break;
				case NPCID.PinkJellyfish:
					DropHelper.DropItemChance(npc, mod.ItemType("LifeJelly"), expert ? 5 : 7);
					break;
				case NPCID.BlueJellyfish:
					DropHelper.DropItemChance(npc, mod.ItemType("ManaJelly"), expert ? 5 : 7);
					break;
				case NPCID.DarkCaster:
					DropHelper.DropItemChance(npc, mod.ItemType("AncientShiv"), expert ? 20 : 25);
					break;
				case NPCID.BigMimicHallow:
				case NPCID.BigMimicCorruption:
				case NPCID.BigMimicCrimson:
				case NPCID.BigMimicJungle: // arguably unnecessary
					DropHelper.DropItemChance(npc, mod.ItemType("CelestialClaymore"), expert ? 5 : 7);
					break;
				case NPCID.Clinger:
					DropHelper.DropItemChance(npc, mod.ItemType("CursedDagger"), expert ? 20 : 25);
					break;
				case NPCID.Shark:
					DropHelper.DropItemChance(npc, mod.ItemType("DepthBlade"), expert ? 10 : 15);
					DropHelper.DropItemChance(npc, ItemID.SharkToothNecklace, expert ? 20 : 30);
					break;
				case NPCID.PresentMimic:
					DropHelper.DropItemChance(npc, mod.ItemType("HolidayHalberd"), expert ? 5 : 7);
					break;
				case NPCID.IchorSticker:
					int spearDropRate = expert ? 20 : 25;
					float spearRoll = Main.rand.NextFloat();
					bool spearOfDestiny = spearRoll < 0.005f; // Exact 1/200 chance for rare regardless of difficulty
					DropHelper.DropItemCondition(npc, mod.ItemType("IchorSpear"), !spearOfDestiny, spearDropRate, 1, 1);
					DropHelper.DropItemCondition(npc, mod.ItemType("SpearofDestiny"), spearOfDestiny, spearDropRate, 1, 1);
					break;
				case NPCID.Harpy:
					int glazeDropRate = defiled ? 20 : expert ? 60 : 80;
					DropHelper.DropItemCondition(npc, mod.ItemType("SkyGlaze"), NPC.downedBoss1, glazeDropRate, 1, 1);
					break;
				case NPCID.Antlion:
				case NPCID.WalkingAntlion:
				case NPCID.FlyingAntlion:
					DropHelper.DropItemChance(npc, mod.ItemType("MandibleClaws"), expert ? 30 : 40);
					DropHelper.DropItemChance(npc, mod.ItemType("MandibleBow"), expert ? 30 : 40);
					break;
				case NPCID.TombCrawlerHead:
					DropHelper.DropItemChance(npc, mod.ItemType("BurntSienna"), expert ? 15 : 20);
					break;
				case NPCID.DuneSplicerHead:
					DropHelper.DropItemCondition(npc, mod.ItemType("Terracotta"), NPC.downedPlantBoss, expert ? 20 : 30, 1, 1);
					break;
				case NPCID.MartianSaucerCore:
					DropHelper.DropItemChance(npc, mod.ItemType("NullificationRifle"), expert ? 5 : 7);
					break;
				case NPCID.Demon:
					DropHelper.DropItemChance(npc, mod.ItemType("DemonicBoneAsh"), expert ? 2 : 3);
					DropHelper.DropItemChance(npc, mod.ItemType("BladecrestOathsword"), expert ? 20 : 25);
					break;
				case NPCID.BoneSerpentHead:
					DropHelper.DropItemChance(npc, mod.ItemType("DemonicBoneAsh"), expert ? 2 : 3);
					DropHelper.DropItemChance(npc, mod.ItemType("OldLordOathsword"), expert ? 10 : 15);
					break;
				case NPCID.Tim:
					DropHelper.DropItemChance(npc, mod.ItemType("PlasmaRod"), expert ? 2 : 3);
					break;
				case NPCID.GoblinSorcerer:
					DropHelper.DropItemChance(npc, mod.ItemType("PlasmaRod"), expert ? 20 : 25);
					break;
				case NPCID.PirateDeadeye:
					DropHelper.DropItemChance(npc, mod.ItemType("ProporsePistol"), expert ? 20 : 25);
					break;
				case NPCID.PirateCrossbower:
					int crossbowDropRate = expert ? 20 : 25;
					float arbalestRoll = Main.rand.NextFloat();
					bool arbalest = arbalestRoll < 0.005f; // Exact 1/200 chance for rare regardless of difficulty
					DropHelper.DropItemCondition(npc, mod.ItemType("RaidersGlory"), !arbalest, crossbowDropRate, 1, 1);
					DropHelper.DropItemCondition(npc, mod.ItemType("Arbalest"), arbalest, crossbowDropRate, 1, 1);
					break;
				case NPCID.GoblinSummoner:
					DropHelper.DropItemChance(npc, mod.ItemType("TheFirstShadowflame"), expert ? 5 : 7);
					break;
				case NPCID.SandElemental:
					DropHelper.DropItemChance(npc, mod.ItemType("WifeinaBottle"), expert ? 5 : 7);
					DropHelper.DropItemCondition(npc, mod.ItemType("WifeinaBottlewithBoobs"), expert, 20, 1, 1);
					break;
				case NPCID.GoblinWarrior:
					DropHelper.DropItemChance(npc, mod.ItemType("Warblade"), expert ? 15 : 20);
					break;
				case NPCID.MartianWalker:
					DropHelper.DropItemChance(npc, mod.ItemType("Wingman"), expert ? 5 : 7);
					break;
				case NPCID.GiantCursedSkull:
					DropHelper.DropItemChance(npc, mod.ItemType("WrathoftheAncients"), expert ? 20 : 25);
					DropHelper.DropItemCondition(npc, mod.ItemType("Keelhaul"), CalamityWorld.downedLeviathan, 10, 1, 1);
					break;
				case NPCID.Necromancer:
				case NPCID.NecromancerArmored:
					DropHelper.DropItemChance(npc, mod.ItemType("WrathoftheAncients"), expert ? 20 : 25);
					break;
			}

			// Every type of Moss Hornet counts for the Needler
			if (CalamityMod.mossHornetList.Contains(npc.type))
			{
				int needlerDropRate = expert ? 20 : 25;
				DropHelper.DropItemChance(npc, mod.ItemType("Needler"), needlerDropRate);
			}

			// Every type of Skeleton counts for the Waraxe and Ancient Bone Dust
			if (CalamityMod.skeletonList.Contains(npc.type))
			{
				DropHelper.DropItemCondition(npc, mod.ItemType("Waraxe"), !Main.hardMode, expert ? 15 : 20, 1, 1);
				DropHelper.DropItemChance(npc, mod.ItemType("AncientBoneDust"), expert ? 4 : 5);
			}
		}
		#endregion

		#region Common Loot
		private void CommonLoot(NPC npc, Mod mod)
		{
			bool expert = Main.expertMode;
			switch (npc.type)
			{
				default:
					break;
				case NPCID.Vulture:
					DropHelper.DropItemChance(npc, mod.ItemType("DesertFeather"), 2, 1, expert ? 2 : 1);
					break;
				case NPCID.RedDevil:
					DropHelper.DropItemChance(npc, mod.ItemType("EssenceofChaos"), expert ? 1f : 0.5f);
					break;
				case NPCID.WyvernHead:
					DropHelper.DropItem(npc, mod.ItemType("EssenceofCinder"), 1, expert ? 2 : 1);
					break;
				case NPCID.AngryNimbus:
					DropHelper.DropItemChance(npc, mod.ItemType("EssenceofCinder"), expert ? 2 : 3);
					break;
				case NPCID.IcyMerman:
				case NPCID.IceTortoise:
					DropHelper.DropItemChance(npc, mod.ItemType("EssenceofEleum"), expert ? 2 : 3);
					break;
				case NPCID.IceGolem:
					DropHelper.DropItem(npc, mod.ItemType("EssenceofEleum"), 1, 2);
					break;
				case NPCID.Plantera:
					DropHelper.DropItemCondition(npc, mod.ItemType("LivingShard"), !expert, 6, 9);
					break;
				case NPCID.NebulaBrain:
				case NPCID.NebulaSoldier:
				case NPCID.NebulaHeadcrab:
				case NPCID.NebulaBeast:
					DropHelper.DropItemChance(npc, mod.ItemType("MeldBlob"), 4, expert ? 2 : 1, expert ? 3 : 2);
					break;
				case NPCID.DungeonGuardian:
					DropHelper.DropItemCondition(npc, mod.ItemType("GoldBurdenBreaker"), Main.hardMode);
					break;
				case NPCID.CultistBoss:
					DropHelper.DropItemChance(npc, mod.ItemType("StardustStaff"), expert ? 3 : 5);
					DropHelper.DropItemChance(npc, mod.ItemType("ThornBlossom"), DropHelper.RareVariantDropRateInt);
					break;
				case NPCID.EyeofCthulhu:
					DropHelper.DropItemCondition(npc, mod.ItemType("VictoryShard"), !expert, 2, 4);
					DropHelper.DropItemCondition(npc, mod.ItemType("TeardropCleaver"), !expert, 5, 1, 1);
					break;
				case NPCID.DevourerHead:
				case NPCID.SeekerHead:
					DropHelper.DropItemChance(npc, mod.ItemType("FetidEssence"), expert ? 2 : 3);
					break;
				case NPCID.FaceMonster:
				case NPCID.Herpling:
					DropHelper.DropItemChance(npc, mod.ItemType("BloodlettingEssence"), expert ? 4 : 5);
					break;
				case NPCID.ManEater:
					DropHelper.DropItemChance(npc, mod.ItemType("ManeaterBulb"), expert ? 2 : 3);
					break;
				case NPCID.AngryTrapper:
					DropHelper.DropItemChance(npc, mod.ItemType("TrapperBulb"), expert ? 4 : 5);
					break;
				case NPCID.MotherSlime:
				case NPCID.CorruptSlime:
				case NPCID.Crimslime:
					DropHelper.DropItemChance(npc, mod.ItemType("MurkySludge"), expert ? 3 : 4);
					break;
				case NPCID.Moth:
					DropHelper.DropItemChance(npc, mod.ItemType("GypsyPowder"), expert ? 1f : 0.5f);
					break;
				case NPCID.Derpling:
					DropHelper.DropItemChance(npc, mod.ItemType("BeetleJuice"), expert ? 4 : 5);
					break;
				case NPCID.SpikedJungleSlime:
				case NPCID.Arapaima:
					DropHelper.DropItemChance(npc, mod.ItemType("MurkyPaste"), expert ? 4 : 5);
					break;
			}

			// All hardmode dungeon enemies drop Ectoblood
			if (CalamityMod.dungeonEnemyBuffList.Contains(npc.type))
				DropHelper.DropItemChance(npc, mod.ItemType("Ectoblood"), 2, 1, expert ? 3 : 1);
		}
		#endregion

		#region Boss Loot
		private void BossLoot(NPC npc, Mod mod)
		{
			bool expert = Main.expertMode;
			bool rev = CalamityWorld.revenge;
            bool death = CalamityWorld.death;

			// Not really loot code, but NPCLoot is the only death hook
			if (npc.boss && !CalamityWorld.downedBossAny)
			{
				CalamityWorld.downedBossAny = true;
				CalamityMod.UpdateServerBoolean();
			}

            // Nightmare Fuel, Endothermic Energy and Darksun Fragments
            if (npc.type == NPCID.Pumpking)
                DropHelper.DropItemCondition(npc, mod.ItemType("NightmareFuel"), CalamityWorld.downedDoG, 10, 20);
			else if (npc.type == NPCID.IceQueen)
                DropHelper.DropItemCondition(npc, mod.ItemType("EndothermicEnergy"), CalamityWorld.downedDoG, 20, 40);
			else if (npc.type == NPCID.Mothron && CalamityWorld.buffedEclipse)
			{
                DropHelper.DropItem(npc, mod.ItemType("DarksunFragment"), 10, 20);

                // Mark a buffed Mothron as killed (allowing access to Yharon P2)
				CalamityWorld.downedBuffedMothron = true;
				CalamityMod.UpdateServerBoolean();
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
