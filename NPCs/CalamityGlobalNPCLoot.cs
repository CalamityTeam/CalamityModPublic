using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.GameContent.Tile_Entities;
using Terraria.GameContent.UI;
using Terraria.ID;
using Terraria.World.Generation;
using Terraria.GameContent.Generation;
using CalamityMod.Tiles;
using CalamityMod.Projectiles;
using CalamityMod;
using CalamityMod.Items;
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

            if (AbyssLootCancel(npc, mod))
                return false;

			if (CalamityWorld.revenge)
			{
				if (npc.type == NPCID.Probe)
					return false;
			}

			if (!NPC.downedMechBossAny && (npc.type == NPCID.Spazmatism || npc.type == NPCID.TheDestroyer || npc.type == NPCID.SkeletronPrime))
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge20"), 1, true);

			if (!NPC.downedSlimeKing && npc.type == NPCID.KingSlime)
			{
				int amount = Main.rand.Next(15, 21);
				if (Main.expertMode)
					amount = (int)((float)amount * 1.5f);
				int type = ItemID.Gel;

				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, type, Main.rand.Next(1, 4), false, 0, false, false);
				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, type, Main.rand.Next(1, 4), false, 0, false, false);
				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, type, Main.rand.Next(1, 4), false, 0, false, false);
				for (int i = 0; i < amount; i++)
					Item.NewItem((int)npc.position.X + Main.rand.Next(npc.width), (int)npc.position.Y + Main.rand.Next(npc.height), 2, 2, type, Main.rand.Next(1, 4), false, 0, false, false);

				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
			}
			else if (!NPC.downedBoss1 && npc.type == NPCID.EyeofCthulhu)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge3"), 1, true);
			}
			else if (!NPC.downedQueenBee && npc.type == NPCID.QueenBee)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 2);
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge16"), 1, true);
			}
			else if (!NPC.downedMechBoss1 && npc.type == NPCID.TheDestroyer)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge21"), 1, true);
			}
			else if (!NPC.downedMechBoss2 && npc.type == NPCID.Spazmatism)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge22"), 1, true);
			}
			else if (!NPC.downedMechBoss3 && npc.type == NPCID.SkeletronPrime)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge23"), 1, true);
			}
			else if (!NPC.downedPlantBoss && npc.type == NPCID.Plantera)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge25"), 1, true);
			}
			else if (!NPC.downedFishron && npc.type == NPCID.DukeFishron)
			{
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
				Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge2"), 1, true);
			}
			else if (npc.type == NPCID.CultistBoss)
			{
				if (!NPC.downedAncientCultist)
				{
					string key = "Mods.CalamityMod.DeusText";
					Color messageColor = Color.Gold;

					if (Main.netMode == 0)
						Main.NewText(Language.GetTextValue(key), messageColor);
					else if (Main.netMode == 2)
						NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);

					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("MagnumRounds"), 4);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("GrenadeRounds"), 2);
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("ExplosiveShells"));
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge4"), 1, true);
				}

				if (Main.bloodMoon)
					npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Knowledge34"), 1, true);
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
				npc.DropItemInstanced(npc.position, npc.Size, mod.ItemType("Rock"), 1, true);
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

			return !hurtByAbyss;
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
			if (npc.type == NPCID.BloodZombie && NPC.downedBoss3 && !npc.SpawnedFromStatue)
			{
				if (Main.rand.Next(200) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Carnage"));
			}
			else if (npc.type == NPCID.TacticalSkeleton)
			{
				if (Main.rand.Next(200) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TrueConferenceCall"));
			}
			else if (npc.type == NPCID.DesertBeast)
			{
				if (Main.rand.Next(200) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EvilSmasher"));
			}
			else if (npc.type == NPCID.DungeonSpirit)
			{
				if (Main.rand.Next(200) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("PearlGod"));
			}
			else if (npc.type == NPCID.RuneWizard)
			{
				if (Main.rand.Next(10) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("EyeofMagnus"));
			}
			else if (npc.type == NPCID.Mimic && !npc.SpawnedFromStatue)
			{
				if (Main.rand.Next(100) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("TheBee"));
			}
		}
		#endregion

		#region Defiled Loot
		private void DefiledLoot(NPC npc, Mod mod)
		{
			if (npc.type == NPCID.AnglerFish || npc.type == NPCID.Werewolf)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AdhesiveBandage);

				if (npc.type == NPCID.Werewolf)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.MoonCharm);
				}
			}
			else if (npc.type == NPCID.DesertBeast)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AncientHorn);
			}
			else if (npc.type == NPCID.ArmoredSkeleton)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BeamSword);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.ArmorPolish);
			}
			else if (npc.type == NPCID.Clown)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Bananarang);
			}
			else if (npc.type == NPCID.Hornet || npc.type == NPCID.MossHornet || npc.type == NPCID.ToxicSludge)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Bezoar);

				if (npc.type == NPCID.MossHornet)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TatteredBeeWing);
				}
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Binoculars);
			}
			else if (npc.type == NPCID.DemonEye || npc.type == NPCID.WanderingEye)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BlackLens);
			}
			else if (npc.type == NPCID.CorruptSlime || npc.type == NPCID.DarkMummy)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Blindfold);
			}
			else if (npc.type >= 269 && npc.type <= 280)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Keybrand);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BoneFeather);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.MagnetSphere);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.WispinaBottle);
			}
			else if (npc.type == NPCID.UndeadMiner)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BonePickaxe);
			}
			else if (npc.type == NPCID.Skeleton)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BoneSword);
			}
			else if (npc.type == NPCID.ScutlixRider)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BrainScrambler);
			}
			else if (npc.type == NPCID.Vampire)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.BrokenBatWing);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.MoonStone);
			}
			else if (npc.type == NPCID.CaveBat)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.ChainKnife);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DepthMeter);
			}
			else if (npc.type == NPCID.DarkCaster || npc.type == NPCID.AngryBones)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.ClothierVoodooDoll);
			}
			else if (npc.type == NPCID.PirateCaptain)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.CoinGun);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DiscountCard);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Cutlass);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.LuckyCoin);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.PirateStaff);
			}
			else if (npc.type == NPCID.Reaper)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DeathSickle);
			}
			else if (npc.type == NPCID.Demon)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DemonScythe);
			}
			else if (npc.type == NPCID.DesertDjinn)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DjinnLamp);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DjinnsCurse);
			}
			else if (npc.type == NPCID.Shark)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.DivingHelmet);
			}
			else if (npc.type == NPCID.Pixie || npc.type == NPCID.Wraith || npc.type == NPCID.Mummy)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FastClock);
			}
			else if (npc.type == NPCID.RedDevil)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FireFeather);
			}
			else if (npc.type == NPCID.IceElemental || npc.type == NPCID.IcyMerman || npc.type == NPCID.ArmoredViking || npc.type == NPCID.IceTortoise)
			{
				if (npc.type == NPCID.IceElemental || npc.type == NPCID.IcyMerman)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FrostStaff);
				}

				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.IceSickle);

				if (npc.type == NPCID.IceTortoise)
				{
					if (Main.rand.Next(20) == 0)
						Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.FrozenTurtleShell);
				}
			}
			else if (npc.type == NPCID.Harpy && Main.hardMode)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.GiantHarpyFeather);
			}
			else if (npc.type == mod.NPCType("SunBat"))
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HelFire);
			}
			else if (npc.type == mod.NPCType("Cryon"))
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Amarok);
			}
			else if (npc.type == NPCID.QueenBee)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.HoneyedGoggles);
			}
			else if (npc.type == NPCID.Piranha)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Hook);
			}
			else if (npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.InfernoFork);
			}
			else if (npc.type == NPCID.PinkJellyfish)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.JellyfishNecklace);
			}
			else if (npc.type == NPCID.Paladin)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Kraken);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.PaladinsHammer);
			}
			else if (npc.type == NPCID.SkeletonArcher)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.MagicQuiver);
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Marrow);
			}
			else if (npc.type == NPCID.Lavabat)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.MagmaStone);
			}
			else if (npc.type == NPCID.WalkingAntlion)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.AntlionClaw);
			}
			else if (npc.type == NPCID.DarkMummy || npc.type == NPCID.GreenJellyfish)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Megaphone);
			}
			else if (npc.type == NPCID.CursedSkull)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Nazar);
			}
			else if (npc.type == NPCID.FireImp)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.ObsidianRose);
			}
			else if (npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.PoisonStaff);
			}
			else if (npc.type == NPCID.SkeletonSniper)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.RifleScope);
			}
			else if (npc.type == NPCID.ChaosElemental)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.RodofDiscord);
			}
			else if (npc.type == NPCID.Necromancer || npc.type == NPCID.NecromancerArmored)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.ShadowbeamStaff);
			}
			else if (npc.type == NPCID.SnowFlinx)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SnowballLauncher);
			}
			else if (npc.type == NPCID.RaggedCaster)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.SpectreStaff);
			}
			else if (npc.type == NPCID.Plantera)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TheAxe);
			}
			else if (npc.type == NPCID.GiantBat)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.TrifoldMap);
			}
			else if (npc.type == NPCID.AngryTrapper)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Uzi);
			}
			else if (npc.type == NPCID.FloatyGross || npc.type == NPCID.Corruptor)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Vitamins);
			}
			else if (NPC.downedMechBossAny && npc.type == NPCID.GiantTortoise)
			{
				if (Main.rand.Next(20) == 0)
					Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.Yelets);
			}
		}
		#endregion

		#region Armageddon Loot
		private void ArmageddonLoot(NPC npc, Mod mod)
		{
			int dropAmt = 5;
			if (npc.type == NPCID.Golem)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.DukeFishron)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.DD2Betsy)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.BrainofCthulhu)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)
			{
				if (npc.boss)
				{
					for (int i = 0; i < dropAmt; i++)
						npc.DropBossBags();
				}
			}
			else if (npc.type == NPCID.QueenBee)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.SkeletronHead)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.WallofFlesh)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.MoonLordCore)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.KingSlime)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.Retinazer || npc.type == NPCID.Spazmatism)
			{
				int num64 = NPCID.Retinazer;
				if (npc.type == NPCID.Retinazer)
					num64 = NPCID.Spazmatism;

				if (!NPC.AnyNPCs(num64))
				{
					for (int i = 0; i < dropAmt; i++)
						npc.DropBossBags();
				}
			}
			else if (npc.type == NPCID.SkeletronPrime)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.TheDestroyer)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
			else if (npc.type == NPCID.Plantera)
			{
				for (int i = 0; i < dropAmt; i++)
					npc.DropBossBags();
			}
		}
		#endregion

		#region Check Boss Spawn
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