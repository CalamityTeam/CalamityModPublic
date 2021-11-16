using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.DifficultyItems;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using System.Threading;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs
{
    public class CalamityGlobalNPCLoot : GlobalNPC
    {
        #region Instance Per Entity
        public override bool InstancePerEntity => false;
        public override bool CloneNewInstances => false;
        #endregion

        #region PreNPCLoot
        public override bool PreNPCLoot(NPC npc)
        {
            // No bosses drop loot in Boss Rush. Progress the event instead.
            if (BossRushEvent.BossRushActive)
            {
                BossRushEvent.OnBossKill(npc, mod);
                return false;
            }

            if (AbyssLootCancel(npc, mod))
                return false;

            if (CalamityWorld.death && !SplittingWormLootBlockWrapper(npc, mod))
                return false;

            // Do not provide free hearts for certain boss NPCs in Rev+.
            if ((CalamityWorld.revenge || CalamityWorld.malice) && CalamityLists.heartDropBlockList.Contains(npc.type) && CalamityPlayer.areThereAnyDamnBosses)
				DropHelper.BlockDrops(ItemID.Heart);

            //
            // Ozzatron 17FEB2021: A NOTE about PreNPCLoot vs NPCLoot
            // PreNPCLoot runs before the boss is marked as dead. This means it is required for lore items and Resident Evil ammo.
            // Because we already have clauses here for every boss, it is more convenient to drop everything here than it is.
            // to iterate through all the bosses twice.
            //

            // Since Calamity makes it spawn in pre-hardmode, don't want to cause other mods to freak out if they use it as a tier gate (like a new weapon or something)
            if (npc.type == NPCID.GreenJellyfish && !Main.hardMode)
            {
                DropHelper.DropItem(npc, ItemID.Glowstick, 1, 4);
                DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, 0.01f);
                DropHelper.DropItemChance(npc, ModContent.ItemType<VitalJelly>(), Main.expertMode ? 5 : 7);
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
            DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeMechs>(), true, mechLore);

			if (CalamityWorld.armageddon)
				ArmageddonLoot(npc);

			if (npc.type == NPCID.KingSlime)
			{
                // Drop a huge spray of Gel items
                // More gel is not dropped on Expert because he has more minions, which increases the amount of gel provided.
                int minGel = 72;
                int maxGel = 100;
				DropHelper.DropItemSpray(npc, ItemID.Gel, minGel, maxGel, 4);

				// Legendary drop for King Slime
				DropHelper.DropItemCondition(npc, ModContent.ItemType<CrownJewel>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeKingSlime>(), true, !NPC.downedSlimeKing);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, NPC.downedSlimeKing);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);
			}
			else if (npc.type == NPCID.EyeofCthulhu)
			{
				// Legendary drops for Eye of Cthulhu
				DropHelper.DropItemCondition(npc, ModContent.ItemType<TeardropCleaver>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<CounterScarf>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<DeathstareRod>(), !Main.expertMode, DropHelper.NormalWeaponDropRateFloat, 1, 1);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeEyeofCthulhu>(), true, !NPC.downedBoss1);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss1);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);
			}
			else if ((npc.boss && (npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody || npc.type == NPCID.EaterofWorldsTail)) || npc.type == NPCID.BrainofCthulhu)
			{
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCorruption>(), true, !WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeEaterofWorlds>(), true, !WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeCrimson>(), true, WorldGen.crimson && !NPC.downedBoss2);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBrainofCthulhu>(), true, WorldGen.crimson && !NPC.downedBoss2);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss2);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);
			}
			else if (npc.type == NPCID.QueenBee)
			{
				// Drop weapons Calamity style instead of mutually exclusive.
				if (!Main.expertMode)
				{
					int[] queenBeeWeapons = new int[]
					{
						ItemID.BeeKeeper,
						ItemID.BeesKnees,
						ItemID.BeeGun,
					};
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, queenBeeWeapons);
					DropHelper.BlockDrops(queenBeeWeapons);
				}

				DropHelper.DropItemCondition(npc, ItemID.Stinger, !Main.expertMode, 5, 10); // Extra stingers
				DropHelper.DropItemCondition(npc, ModContent.ItemType<HardenedHoneycomb>(), !Main.expertMode, 30, 50);

				// Legendary drop for Queen Bee
				DropHelper.DropItemCondition(npc, ModContent.ItemType<TheBee>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeQueenBee>(), true, !NPC.downedQueenBee);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Dryad }, NPC.downedQueenBee);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);
			}
			else if (npc.type == NPCID.SkeletronHead)
			{
				DropHelper.DropItemSpray(npc, ItemID.Bone, 70, 100, 5);

				// Legendary drop for Skeletron
				DropHelper.DropItemCondition(npc, ModContent.ItemType<ClothiersWrath>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSkeletron>(), true, !NPC.downedBoss3);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.Dryad, NPCID.Demolitionist }, NPC.downedBoss3);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);
			}
			else if (npc.type == NPCID.WallofFlesh)
			{
				if (!Main.expertMode)
				{
					// Drop weapons Calamity style instead of mutually exclusive -- this includes Calamity weapons.
					int[] wofWeapons = new int[]
					{
						ItemID.BreakerBlade,
						ItemID.ClockworkAssaultRifle,
						ModContent.ItemType<Meowthrower>(),
						ItemID.LaserRifle,
						ModContent.ItemType<BlackHawkRemote>(),
						ModContent.ItemType<BlastBarrel>(),
					};
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, wofWeapons);
					DropHelper.BlockDrops(wofWeapons);

					// Drop emblems Calamity style instead of mutually exclusive -- this includes the Rogue Emblem.
					int[] emblems = new int[]
					{
						ItemID.WarriorEmblem,
						ItemID.RangerEmblem,
						ItemID.SorcererEmblem,
						ItemID.SummonerEmblem,
						ModContent.ItemType<RogueEmblem>(),
					};
					DropHelper.DropEntireSet(npc, 0.25f, emblems);
					DropHelper.BlockDrops(emblems);
				}

				// Drop Demon Trophy directly if it hasn't been used yet and Expert Mode is not active.
				DropHelper.DropItemCondition(npc, ModContent.ItemType<MLGRune>(), !Main.expertMode && !CalamityWorld.demonMode);

				// Drop Hermit's Box directly for EACH player, regardles of Expert or not. 100% chance on first kill, 10% chance afterwards.
				float hermitBoxChance = Main.hardMode ? 1f : 0.1f;
				DropHelper.DropItemChance(npc, ModContent.ItemType<IbarakiBox>(), true, hermitBoxChance);

				DropHelper.DropItemFromSetCondition(npc, !Main.expertMode, 0.2f, ItemID.CorruptionKey, ItemID.CrimsonKey);

				// Legendary drop for Wall of Flesh
				DropHelper.DropItemCondition(npc, ModContent.ItemType<EvilSmasher>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeUnderworld>(), true, !Main.hardMode);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeWallofFlesh>(), true, !Main.hardMode);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Merchant, NPCID.ArmsDealer, NPCID.Dryad, NPCID.Painter, NPCID.WitchDoctor, NPCID.Stylist, NPCID.DyeTrader, NPCID.Demolitionist, NPCID.PartyGirl, NPCID.Clothier, NPCID.SkeletonMerchant, ModContent.NPCType<THIEF>() }, Main.hardMode);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				// First kill text (this is not a loot function)
				if (!Main.hardMode)
				{
					// Increase altar count to allow natural mech boss spawning.
					if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
						WorldGen.altarCount++;

					string key2 = "Mods.CalamityMod.UglyBossText";
					Color messageColor2 = Color.Aquamarine;
					CalamityUtils.DisplayLocalizedText(key2, messageColor2);

					if (CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					{
						string key3 = "Mods.CalamityMod.HardmodeOreTier1Text";
						Color messageColor3 = new Color(50, 255, 130);
                        CalamityUtils.SpawnOre(TileID.Cobalt, 12E-05, 0.4f, 0.6f, 3, 8);
						CalamityUtils.SpawnOre(TileID.Palladium, 12E-05, 0.4f, 0.6f, 3, 8);
						CalamityUtils.DisplayLocalizedText(key3, messageColor3);
					}
				}
			}
			else if (lastTwinStanding)
			{
				// Only drop hallowed bars after all mechs are down
				if ((!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					DropHelper.BlockDrops(ItemID.HallowedBar);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<MysteriousCircuitry>(), Main.expertMode, CalamityGlobalNPC.DraedonMayhem, 8, 16);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<DubiousPlating>(), Main.expertMode, CalamityGlobalNPC.DraedonMayhem, 8, 16);

				// Legendary drop for Twins
				DropHelper.DropItemCondition(npc, ModContent.ItemType<Arbalest>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeTwins>(), true, !NPC.downedMechBoss2);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, !NPC.downedMechBoss1 || NPC.downedMechBoss2 || !NPC.downedMechBoss3);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				if (!NPC.downedMechBoss2 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					SpawnMechBossHardmodeOres();
			}
			else if (npc.type == NPCID.TheDestroyer)
			{
				// Only drop hallowed bars after all mechs are down
				if ((!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					DropHelper.BlockDrops(ItemID.HallowedBar);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<MysteriousCircuitry>(), Main.expertMode, CalamityGlobalNPC.DraedonMayhem, 8, 16);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<DubiousPlating>(), Main.expertMode, CalamityGlobalNPC.DraedonMayhem, 8, 16);

				// Legendary drop for Destroyer
				DropHelper.DropItemCondition(npc, ModContent.ItemType<SHPC>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDestroyer>(), true, !NPC.downedMechBoss1);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				if (!NPC.downedMechBoss1 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					SpawnMechBossHardmodeOres();
			}
			else if (npc.type == NPCID.SkeletronPrime)
			{
				// Only drop hallowed bars after all mechs are down
				if ((!NPC.downedMechBoss1 || !NPC.downedMechBoss2 || !NPC.downedMechBoss3) && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					DropHelper.BlockDrops(ItemID.HallowedBar);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<MysteriousCircuitry>(), Main.expertMode, CalamityGlobalNPC.DraedonMayhem, 8, 16);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<DubiousPlating>(), Main.expertMode, CalamityGlobalNPC.DraedonMayhem, 8, 16);

				// Legendary drop for Skeletron Prime
				DropHelper.DropItemCondition(npc, ModContent.ItemType<GoldBurdenBreaker>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<SpearofDestiny>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeSkeletronPrime>(), true, !NPC.downedMechBoss3);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.DD2Bartender, NPCID.Stylist, NPCID.Truffle, ModContent.NPCType<THIEF>() }, NPC.downedMechBossAny);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Stylist, ModContent.NPCType<DILF>(), ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, !NPC.downedMechBoss1 || !NPC.downedMechBoss2 || NPC.downedMechBoss3);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				if (!NPC.downedMechBoss3 && CalamityConfig.Instance.EarlyHardmodeProgressionRework)
					SpawnMechBossHardmodeOres();
			}
			else if (npc.type == NPCID.Plantera)
			{
				// Drop weapons Calamity style instead of mutually exclusive.
				if (!Main.expertMode)
				{
					int[] planteraWeapons = new int[]
					{
						ItemID.FlowerPow,
						ItemID.Seedler,
						ItemID.GrenadeLauncher,
						ItemID.VenusMagnum,
						ItemID.LeafBlower,
						ItemID.NettleBurst,
						ItemID.WaspGun
					};
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, planteraWeapons);
					DropHelper.BlockDrops(planteraWeapons);

                    // Equipment
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BloomStone>(), 5);
                }

                DropHelper.DropItemCondition(npc, ModContent.ItemType<LivingShard>(), !Main.expertMode, 12, 18);
				DropHelper.DropItemCondition(npc, ItemID.JungleKey, !Main.expertMode, 5, 1, 1);

				// Legendary drop for Plantera
				DropHelper.DropItemCondition(npc, ModContent.ItemType<BlossomFlux>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<ThornBlossom>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgePlantera>(), true, !NPC.downedPlantBoss);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.WitchDoctor, NPCID.Truffle, ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedPlantBoss);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				// Spawn Perennial Ore if Plantera has never been killed
				if (!NPC.downedPlantBoss)
				{
					string key2 = "Mods.CalamityMod.PlantOreText";
					Color messageColor2 = Color.GreenYellow;
					string key3 = "Mods.CalamityMod.SandSharkText3";
					Color messageColor3 = Color.Goldenrod;

					CalamityUtils.SpawnOre(ModContent.TileType<PerennialOre>(), 12E-05, 0.5f, 0.7f, 3, 8, TileID.Dirt, TileID.Stone);

					CalamityUtils.DisplayLocalizedText(key2, messageColor2);
					CalamityUtils.DisplayLocalizedText(key3, messageColor3);
				}
			}

			// These event enemies set shop variables and since those depend on downed bools they must be done in PreNPCLoot.
			else if (npc.type == NPCID.Pumpking)
			{
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Clothier }, NPC.downedHalloweenKing);
			}
			else if (npc.type == NPCID.Everscream)
			{
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, NPC.downedChristmasTree || !NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
			}
			else if (npc.type == NPCID.SantaNK1)
			{
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, !NPC.downedChristmasTree || NPC.downedChristmasSantank || !NPC.downedChristmasIceQueen);
			}
			else if (npc.type == NPCID.IceQueen)
			{
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.Clothier }, NPC.downedChristmasIceQueen);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<DILF>() }, !NPC.downedChristmasTree || !NPC.downedChristmasSantank || NPC.downedChristmasIceQueen);
			}

			else if (npc.type == NPCID.Golem)
			{
				// Drop loot Calamity style instead of mutually exclusive.
				if (!Main.expertMode)
				{
					int[] golemItems = new int[]
					{
						ItemID.GolemFist,
						ItemID.PossessedHatchet,
						ItemID.Stynger,
						ItemID.HeatRay,
						ItemID.StaffofEarth,
						ItemID.EyeoftheGolem,
						ItemID.SunStone,
					};
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, golemItems);
					DropHelper.BlockDrops(golemItems);
				}

				// If Golem has never been killed, provide a Picksaw to all players. This only applies in Normal Mode.
				// The Golem Treasure Bag is guaranteed to provide a Picksaw if one is not yet in the inventory.
				DropHelper.DropItemCondition(npc, ItemID.Picksaw, true, !Main.expertMode && !NPC.downedGolemBoss);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofCinder>(), !Main.expertMode, 5, 10);

				// Legendary drop for Golem
				DropHelper.DropItemCondition(npc, ModContent.ItemType<AegisBlade>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<LeadWizard>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeGolem>(), true, !NPC.downedGolemBoss);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { NPCID.ArmsDealer, NPCID.Cyborg, NPCID.Steampunker, NPCID.Wizard, NPCID.WitchDoctor, NPCID.DD2Bartender, ModContent.NPCType<FAP>(), ModContent.NPCType<THIEF>() }, NPC.downedGolemBoss);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				// If Golem has never been killed, send a message about the Plague.
				if (!NPC.downedGolemBoss)
				{
					string key = "Mods.CalamityMod.BabyBossText";
					Color messageColor = Color.Lime;

					CalamityUtils.DisplayLocalizedText(key, messageColor);
				}
			}
			else if (npc.type == NPCID.DD2Betsy && !CalamityWorld.downedBetsy)
			{
				// Drop weapons Calamity style instead of mutually exclusive.
				if (!Main.expertMode)
				{
					int[] betsyWeapons = new int[]
					{
						ItemID.DD2SquireBetsySword, // Flying Dragon
                        ItemID.MonkStaffT3, // Sky Dragon's Fury
                        ItemID.DD2BetsyBow, // Aerial Bane
                        ItemID.ApprenticeStaffT3, // Betsy's Wrath
                    };
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, betsyWeapons);
					DropHelper.BlockDrops(betsyWeapons);
				}

				// Mark Betsy as dead (Vanilla does not keep track of her)
				CalamityWorld.downedBetsy = true;
				CalamityNetcode.SyncWorld();
			}
			else if (npc.type == NPCID.DukeFishron)
			{
				// Drop weapons Calamity style instead of mutually exclusive -- this includes Calamity weapons.
				if (!Main.expertMode)
				{
					int[] dukeWeapons = new int[]
					{
						ItemID.Flairon,
						ItemID.Tsunami,
						ItemID.BubbleGun,
						ItemID.RazorbladeTyphoon,
						ItemID.TempestStaff,
						ModContent.ItemType<DukesDecapitator>(),
					};
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, dukeWeapons);
					DropHelper.BlockDrops(dukeWeapons);
				}

				// Legendary drop for Duke Fishron
				DropHelper.DropItemCondition(npc, ModContent.ItemType<BrinyBaron>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeDukeFishron>(), true, !NPC.downedFishron);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);
			}
			else if (npc.type == NPCID.CultistBoss)
			{
				// Legendary drops for Cultist
				DropHelper.DropItemCondition(npc, ModContent.ItemType<EyeofMagnus>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<StardustStaff>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeLunaticCultist>(), true, !NPC.downedAncientCultist);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeBloodMoon>(), true, Main.bloodMoon);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

				// Deus text (this is not a loot function)
				if (!NPC.downedAncientCultist)
				{
					string key = "Mods.CalamityMod.DeusText";
					Color messageColor = Color.Gold;

					CalamityUtils.DisplayLocalizedText(key, messageColor);
				}
			}
			else if (npc.type == NPCID.MoonLordCore)
			{
				// Drop weapons Calamity style instead of mutually exclusive -- this includes Calamity weapons.
				if (!Main.expertMode)
				{
					int[] moonLordWeapons = new int[]
					{
						ItemID.Meowmere,
						ItemID.StarWrath,
						ItemID.Terrarian,
						ItemID.FireworksLauncher, // Celebration
                        // ItemID.CelebrationMK2,
                        ItemID.SDMG,
						ItemID.LastPrism,
						ItemID.LunarFlareBook,
						ItemID.MoonlordTurretStaff, // Lunar Portal Staff
                        ItemID.RainbowCrystalStaff,
						ModContent.ItemType<UtensilPoker>(),
					};
					DropHelper.DropEntireSet(npc, DropHelper.NormalWeaponDropRateFloat, moonLordWeapons);
					DropHelper.BlockDrops(moonLordWeapons);
				}

				// Gravity Globe is available to Normal players as well
				DropHelper.DropItemCondition(npc, ItemID.GravityGlobe, !Main.expertMode);

				// One Celestial Onion is given to each player individually
				DropHelper.DropItemCondition(npc, ModContent.ItemType<MLGRune2>(), true, !Main.expertMode);

				// Legendary drops for Moon Lord
				DropHelper.DropItemCondition(npc, ModContent.ItemType<GrandDad>(), true, CalamityWorld.malice);
				DropHelper.DropItemCondition(npc, ModContent.ItemType<Infinity>(), true, CalamityWorld.malice);

				DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeMoonLord>(), true, !NPC.downedMoonlord);
				CalamityGlobalTownNPC.SetNewShopVariable(new int[] { ModContent.NPCType<THIEF>() }, NPC.downedMoonlord);
				CalamityGlobalNPC.SetNewBossJustDowned(npc);

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

                if (!CalamityWorld.HasGeneratedLuminitePlanetoids)
                {
                    // Generate luminite planetoids.
                    // This operation is done on a separate thread to lighten the load on servers so that they
                    // can focus on more critical operations asychronously and ideally avoid a time-out crash.
                    // Very few operations in Terraria utilize the pool, so it is highly unlikely that no threads will remain in it.
                    ThreadPool.QueueUserWorkItem(_ => LuminitePlanet.GenerateLuminitePlanetoids());

                    CalamityWorld.HasGeneratedLuminitePlanetoids = true;

                    // If the moon lord is already marked as dead, an associated world sync packet will not be sent automatically
                    // Send one manually.
                    if (NPC.downedMoonlord)
                        CalamityNetcode.SyncWorld();
                }

                // Spawn Exodium planetoids and send messages about Providence, Bloodstone, Phantoplasm, etc. if ML has not been killed yet
                if (!NPC.downedMoonlord)
				{
					CalamityUtils.DisplayLocalizedText(key, messageColor);
					CalamityUtils.DisplayLocalizedText(key2, messageColor2);
					CalamityUtils.DisplayLocalizedText(key3, messageColor3);
					CalamityUtils.DisplayLocalizedText(key4, messageColor4);
					CalamityUtils.DisplayLocalizedText(key5, messageColor5);
				}
			}
			else if (npc.type == NPCID.RedDevil)
			{
				DropHelper.DropItemChance(npc, ItemID.FireFeather, 0.1f);
				DropHelper.BlockDrops(ItemID.FireFeather);
			}
			else if (npc.type == NPCID.Vampire || npc.type == NPCID.VampireBat)
			{
				DropHelper.DropItemChance(npc, ItemID.MoonStone, 0.15f);
				DropHelper.BlockDrops(ItemID.MoonStone);
			}
			else if (npc.type == NPCID.Werewolf)
			{
				DropHelper.DropItemChance(npc, ItemID.MoonCharm, 0.05f);
				DropHelper.BlockDrops(ItemID.MoonCharm);
			}
			else if (npc.type == NPCID.Mimic)
			{
				float w = DropHelper.BagWeaponDropRateFloat;
				DropHelper.DropEntireWeightedSet(npc,
					DropHelper.WeightStack(ItemID.StarCloak, w),
					DropHelper.WeightStack(ItemID.CrossNecklace, w),
					DropHelper.WeightStack(ItemID.TitanGlove, w),
					DropHelper.WeightStack(ItemID.DualHook, w),
					DropHelper.WeightStack(ItemID.MagicDagger, w),
					DropHelper.WeightStack(ItemID.Compass, w),
					DropHelper.WeightStack(ItemID.PhilosophersStone, w)
				);

				int[] mimicDrops = new int[]
				{
					ItemID.StarCloak,
					ItemID.CrossNecklace,
					ItemID.TitanGlove,
					ItemID.DualHook,
                    ItemID.MagicDagger,
					ItemID.Compass,
					ItemID.PhilosophersStone
				};
				DropHelper.BlockDrops(mimicDrops);
			}
			else if (npc.type == NPCID.Moth)
			{
				DropHelper.DropItem(npc, ItemID.ButterflyDust);
				DropHelper.BlockDrops(ItemID.ButterflyDust);
			}
			else if (npc.type >= NPCID.RustyArmoredBonesAxe && npc.type <= NPCID.HellArmoredBonesSword)
			{
				DropHelper.DropItemChance(npc, ItemID.WispinaBottle, 0.005f);
				DropHelper.BlockDrops(ItemID.WispinaBottle);
			}
			else if (npc.type == NPCID.Paladin)
			{
				DropHelper.DropItemChance(npc, ItemID.PaladinsHammer, 0.15f);
				DropHelper.DropItemChance(npc, ItemID.PaladinsShield, 0.2f);

				int[] paladinDrops = new int[]
				{
					ItemID.PaladinsHammer,
					ItemID.PaladinsShield
				};
				DropHelper.BlockDrops(paladinDrops);
			}
			else if (npc.type == NPCID.BoneLee)
			{
				DropHelper.DropItemChance(npc, ItemID.BlackBelt, 0.25f);
				DropHelper.DropItemChance(npc, ItemID.Tabi, 0.25f);

				int[] boneLeeDrops = new int[]
				{
					ItemID.BlackBelt,
					ItemID.Tabi
				};
				DropHelper.BlockDrops(boneLeeDrops);
			}

			return true;
        }
		#endregion

		#region Spawn Mech Boss Hardmode Ores
		private void SpawnMechBossHardmodeOres()
		{
			if (!NPC.downedMechBossAny)
			{
				string key = "Mods.CalamityMod.HardmodeOreTier2Text";
				Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Mythril, 12E-05, 0.5f, 0.7f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Orichalcum, 12E-05, 0.5f, 0.7f, 3, 8);
				CalamityUtils.DisplayLocalizedText(key, messageColor);
			}
			else if ((!NPC.downedMechBoss1 && !NPC.downedMechBoss2) || (!NPC.downedMechBoss2 && !NPC.downedMechBoss3) || (!NPC.downedMechBoss3 && !NPC.downedMechBoss1))
			{
				string key = "Mods.CalamityMod.HardmodeOreTier3Text";
				Color messageColor = new Color(50, 255, 130);
                CalamityUtils.SpawnOre(TileID.Adamantite, 12E-05, 0.6f, 0.8f, 3, 8);
                CalamityUtils.SpawnOre(TileID.Titanium, 12E-05, 0.6f, 0.8f, 3, 8);
				CalamityUtils.DisplayLocalizedText(key, messageColor);
			}
			else
			{
				string key = "Mods.CalamityMod.HardmodeOreTier4Text";
				Color messageColor = new Color(50, 255, 130);
				CalamityUtils.SpawnOre(ModContent.TileType<HallowedOre>(), 12E-05, 0.45f, 0.8f, 3, 8, TileID.Pearlstone, TileID.HallowHardenedSand, TileID.HallowSandstone, TileID.HallowedIce);
				CalamityUtils.DisplayLocalizedText(key, messageColor);
			}
		}
		#endregion

		#region Abyss Loot Cancel
		private bool AbyssLootCancel(NPC npc, Mod mod)
        {
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int abyssChasmY = y - 250;
            int abyssChasmX = CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);
            bool abyssPosX = false;
            bool abyssPosY = (double)(npc.position.Y / 16f) <= abyssChasmY;

            if (CalamityWorld.abyssSide)
            {
                if ((double)(npc.position.X / 16f) < abyssChasmX + 80)
                {
                    abyssPosX = true;
                }
            }
            else
            {
                if ((double)(npc.position.X / 16f) > abyssChasmX - 80)
                {
                    abyssPosX = true;
                }
            }

            bool hurtByAbyss = npc.wet && npc.damage > 0 && !npc.boss && !npc.friendly && !npc.dontTakeDamage &&
                (npc.position.Y / 16f > (Main.rockLayer - Main.maxTilesY * 0.05)) &&
                abyssPosY && abyssPosX && !npc.buffImmune[ModContent.BuffType<CrushDepth>()];

            return hurtByAbyss;
        }
        #endregion

        #region Splitting Worm Loot
        internal static bool SplittingWormLootBlockWrapper(NPC npc, Mod mod)
        {
            if (!CalamityWorld.death)
                return true;

            switch (npc.type)
            {
                case NPCID.DiggerHead:
                case NPCID.DiggerBody:
                case NPCID.DiggerTail:
                    return SplittingWormLoot(npc, mod, 0);
                case NPCID.SeekerHead:
                case NPCID.SeekerBody:
                case NPCID.SeekerTail:
                    return SplittingWormLoot(npc, mod, 1);
                case NPCID.DuneSplicerHead:
                case NPCID.DuneSplicerBody:
                case NPCID.DuneSplicerTail:
                    return SplittingWormLoot(npc, mod, 2);
                default:
                    return true;
            }
        }

        internal static bool SplittingWormLoot(NPC npc, Mod mod, int wormType)
        {
            switch (wormType)
            {
                case 0: return CheckSegments(NPCID.DiggerHead, NPCID.DiggerBody, NPCID.DiggerTail);
                case 1: return CheckSegments(NPCID.SeekerHead, NPCID.SeekerBody, NPCID.SeekerTail);
                case 2: return CheckSegments(NPCID.DuneSplicerHead, NPCID.DuneSplicerBody, NPCID.DuneSplicerTail);
                default:
                    break;
            }

            bool CheckSegments(int head, int body, int tail)
            {
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && (Main.npc[i].type == head || Main.npc[i].type == body || Main.npc[i].type == tail))
                    {
                        return false;
                    }
                }
                return true;
            }

            return true;
        }
		#endregion

		#region Armageddon Loot
		private void ArmageddonLoot(NPC npc)
		{
			switch (npc.type)
			{
				case NPCID.EaterofWorldsHead:
				case NPCID.EaterofWorldsBody:
				case NPCID.EaterofWorldsTail:
					if (npc.boss) // only drop from the 1 "boss" segment (redcode)
						DropHelper.DropArmageddonBags(npc);
					break;

				case NPCID.Retinazer: // only drop if spaz is already dead
					if (!NPC.AnyNPCs(NPCID.Spazmatism))
						DropHelper.DropArmageddonBags(npc);
					break;

				case NPCID.Spazmatism: // only drop if ret is already dead
					if (!NPC.AnyNPCs(NPCID.Retinazer))
						DropHelper.DropArmageddonBags(npc);
					break;

				case NPCID.KingSlime:
				case NPCID.EyeofCthulhu:
				case NPCID.BrainofCthulhu:
				case NPCID.QueenBee:
				case NPCID.SkeletronHead:
				case NPCID.WallofFlesh:
				case NPCID.TheDestroyer:
				case NPCID.SkeletronPrime:
				case NPCID.Plantera:
				case NPCID.Golem:
				case NPCID.DD2Betsy:
				case NPCID.DukeFishron:
				case NPCID.MoonLordCore:
					DropHelper.DropArmageddonBags(npc);
					break;

				default:
					break;
			}
		}
		#endregion

		#region NPCLoot
		public override void NPCLoot(NPC npc)
        {
            // LATER -- keeping bosses alive lets draedon mayhem continue even after killing mechs
            // Reset Draedon Mayhem to false if no bosses are alive
            if (CalamityGlobalNPC.DraedonMayhem)
            {
                if (!CalamityPlayer.areThereAnyDamnBosses)
                {
                    CalamityGlobalNPC.DraedonMayhem = false;
                    CalamityNetcode.SyncWorld();
                }
            }

            // Miscellaneous on-enemy-kill effects.
            CheckBossSpawn(npc);
            if (CalamityWorld.rainingAcid)
                AcidRainEvent.OnEnemyKill(npc);

            ArmorSetLoot(npc);
            RareLoot(npc);
            CommonLoot(npc);
            TownNPCLoot(npc);
            EventLoot(npc, Main.pumpkinMoon, Main.snowMoon, Main.eclipse);
        }
        #endregion

        #region Check Boss Spawn
        // not really drop code
        private void CheckBossSpawn(NPC npc)
        {
            if ((npc.type == ModContent.NPCType<PhantomSpirit>() || npc.type == ModContent.NPCType<PhantomSpiritS>() || npc.type == ModContent.NPCType<PhantomSpiritM>() ||
                npc.type == ModContent.NPCType<PhantomSpiritL>()) && !NPC.AnyNPCs(ModContent.NPCType<Polterghast.Polterghast>()) && !CalamityWorld.downedPolterghast)
            {
                CalamityMod.ghostKillCount++;
                if (CalamityMod.ghostKillCount == 10)
                {
                    string key = "Mods.CalamityMod.GhostBossText2";
                    Color messageColor = Color.Cyan;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                else if (CalamityMod.ghostKillCount == 20)
                {
                    string key = "Mods.CalamityMod.GhostBossText3";
                    Color messageColor = Color.Cyan;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }

                if (CalamityMod.ghostKillCount >= 30 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<Polterghast.Polterghast>());
                        CalamityMod.ghostKillCount = 0;
                    }
                }
            }

            if (NPC.downedPlantBoss && (npc.type == NPCID.SandShark || npc.type == NPCID.SandsharkHallow || npc.type == NPCID.SandsharkCorrupt || npc.type == NPCID.SandsharkCrimson) && !NPC.AnyNPCs(ModContent.NPCType<GreatSandShark.GreatSandShark>()))
            {
                CalamityMod.sharkKillCount++;
                if (CalamityMod.sharkKillCount == 4)
                {
                    string key = "Mods.CalamityMod.SandSharkText";
                    Color messageColor = Color.Goldenrod;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                else if (CalamityMod.sharkKillCount == 8)
                {
                    string key = "Mods.CalamityMod.SandSharkText2";
                    Color messageColor = Color.Goldenrod;

                    CalamityUtils.DisplayLocalizedText(key, messageColor);
                }
                if (CalamityMod.sharkKillCount >= 10 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!Main.player[Main.myPlayer].dead && Main.player[Main.myPlayer].active)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/MaulerRoar"),
                            (int)Main.player[Main.myPlayer].position.X, (int)Main.player[Main.myPlayer].position.Y);
                    }

                    int lastPlayer = npc.lastInteraction;

                    if (!Main.player[lastPlayer].active || Main.player[lastPlayer].dead)
                    {
                        lastPlayer = npc.FindClosestPlayer();
                    }

                    if (lastPlayer >= 0)
                    {
                        NPC.SpawnOnPlayer(lastPlayer, ModContent.NPCType<GreatSandShark.GreatSandShark>());
                        CalamityMod.sharkKillCount = -5;
                    }
                }
            }
        }
        #endregion

        #region Armor Set Loot
        private void ArmorSetLoot(NPC npc)
        {
            // Tarragon armor set bonus: 20% chance to drop hearts from all valid enemies
            if (Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Calamity().tarraSet)
            {
                if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && npc.lifeMax > 100)
                {
                    DropHelper.DropItemChance(npc, ItemID.Heart, 5);
                }
            }

            // Blood Orb drops: Valid enemy during a blood moon on the Surface
            if (!npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && Main.bloodMoon && npc.HasPlayerTarget && npc.position.Y / 16D < Main.worldSurface)
            {
                if (Main.player[Player.FindClosest(npc.Center, npc.width, npc.height)].Calamity().bloodflareSet)
                {
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BloodOrb>(), 2); // 50% chance of 1 orb with Bloodflare
                }

                // 20% chance to get a Blood Orb with or without Bloodflare
                DropHelper.DropItemChance(npc, ModContent.ItemType<BloodOrb>(), 5);
            }
        }
        #endregion

        #region Rare Loot
        private void RareLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Drippler:
                    float bouncingEyeballChance = Main.expertMode ? 0.05f : 0.025f;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BouncingEyeball>(), bouncingEyeballChance);
                    break;

                case NPCID.FireImp:
                    float stalactiteDropRate = Main.expertMode ? 0.16f : 0.1f;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<AshenStalactite>(), stalactiteDropRate);
                    break;

                case NPCID.PossessedArmor:
                    float amuletDropRate = Main.expertMode ? 0.05f : 0.025f;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PsychoticAmulet>(), amuletDropRate);
                    break;

                case NPCID.VampireBat:
                case NPCID.Vampire:
                    float batHookDropRate = Main.expertMode ? 0.05f : 0.025f;
                    DropHelper.DropItemChance(npc, ItemID.BatHook, batHookDropRate);
                    break;

                case NPCID.SeaSnail:
                    DropHelper.DropItem(npc, ModContent.ItemType<SeaShell>());
                    break;

                case NPCID.GreekSkeleton:
                    // Calamity increases the drop chance of the Gladiator's set because it's actually a set
                    float gladiatorDropRate = Main.expertMode ? 0.16f : 0.1f;
                    int[] gladiatorArmor = new int[]
                    {
                        ItemID.GladiatorHelmet,
                        ItemID.GladiatorBreastplate,
                        ItemID.GladiatorLeggings,
                    };
                    DropHelper.DropItemFromSetChance(npc, gladiatorDropRate, gladiatorArmor);
                    DropHelper.BlockDrops(gladiatorArmor);
                    break;

                case NPCID.GiantTortoise:
                    float shellChance = Main.expertMode ? 0.25f : 0.15f;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<GiantTortoiseShell>(), shellChance);
                    break;

                case NPCID.GiantShelly:
                case NPCID.GiantShelly2:
					float shellDropRate = Main.expertMode ? 0.25f : 0.15f;
					DropHelper.DropItemChance(npc, ModContent.ItemType<GiantShell>(), shellDropRate);
                    break;

                case NPCID.AnomuraFungus:
					float fungalShellDropRate = Main.expertMode ? 0.25f : 0.15f;
					DropHelper.DropItemChance(npc, ModContent.ItemType<FungalCarapace>(), fungalShellDropRate);
                    break;

                case NPCID.Crawdad:
                case NPCID.Crawdad2:
					float carapaceDropRate = Main.expertMode ? 0.25f : 0.15f;
					DropHelper.DropItemChance(npc, ModContent.ItemType<CrawCarapace>(), carapaceDropRate);
                    break;

                case NPCID.GreenJellyfish:
					float vitalJellyDropRate = Main.expertMode ? 0.25f : 0.15f;
					DropHelper.DropItemChance(npc, ModContent.ItemType<VitalJelly>(), vitalJellyDropRate);
                    break;

                case NPCID.PinkJellyfish:
					float lifeJellyDropRate = Main.expertMode ? 0.125f : 0.075f;
					DropHelper.DropItemChance(npc, ModContent.ItemType<LifeJelly>(), lifeJellyDropRate);
                    break;

                case NPCID.BlueJellyfish:
					float manaJellyDropRate = Main.expertMode ? 0.25f : 0.15f;
					DropHelper.DropItemChance(npc, ModContent.ItemType<ManaJelly>(), manaJellyDropRate);
                    break;

                case NPCID.DarkCaster:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<AncientShiv>(), Main.expertMode ? 15 : 25);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ShinobiBlade>(), Main.expertMode ? 15 : 25);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<StaffOfNecrosteocytes>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.BigMimicHallow:
                case NPCID.BigMimicCorruption:
                case NPCID.BigMimicCrimson:
                case NPCID.BigMimicJungle: // arguably unnecessary
                    DropHelper.DropItemChance(npc, ModContent.ItemType<CelestialClaymore>(), Main.expertMode ? 4 : 7);
                    break;

                case NPCID.Clinger:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<CursedDagger>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.Shark:
                    DropHelper.DropItemChance(npc, ItemID.SharkToothNecklace, Main.expertMode ? 15 : 25);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<JoyfulHeart>(), Main.expertMode ? 15 : 25);
                    break;
                    
                case NPCID.IchorSticker:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<IchorSpear>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.Harpy:
                    int glazeDropRate = Main.expertMode ? 30 : 50;
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<SkyGlaze>(), NPC.downedBoss1, glazeDropRate, 1, 1);
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<EssenceofCinder>(), Main.hardMode && !npc.SpawnedFromStatue, Main.expertMode ? 2 : 3, 1, 1);
                    break;

                case NPCID.Antlion:
                case NPCID.WalkingAntlion:
                case NPCID.FlyingAntlion:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MandibleClaws>(), Main.expertMode ? 30 : 50);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MandibleBow>(), Main.expertMode ? 30 : 50);
                    break;

                case NPCID.TombCrawlerHead:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BurntSienna>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.DuneSplicerHead:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Terracotta>(), NPC.downedPlantBoss, Main.expertMode ? 15 : 25, 1, 1);
                    break;

                case NPCID.MartianSaucerCore:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<NullificationRifle>(), Main.expertMode ? 4 : 7);
                    break;

                case NPCID.Demon:
                case NPCID.VoodooDemon:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DemonicBoneAsh>(), Main.expertMode ? 2 : 3);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BladecrestOathsword>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.BoneSerpentHead:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DemonicBoneAsh>(), Main.expertMode ? 2 : 3);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<OldLordOathsword>(), Main.expertMode ? 7 : 12);
                    break;

                case NPCID.Tim:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PlasmaRod>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.GoblinSorcerer:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PlasmaRod>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.PirateDeadeye:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ProporsePistol>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.PirateCrossbower:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<RaidersGlory>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.GoblinSummoner:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<TheFirstShadowflame>(), Main.expertMode ? 3 : 5);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BurningStrife>(), Main.expertMode ? 3 : 5);
                    break;

                case NPCID.SandElemental:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<WifeinaBottle>(), Main.expertMode ? 3 : 5);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<WifeinaBottlewithBoobs>(), Main.expertMode ? 6 : 10);
                    break;

                case NPCID.GoblinWarrior:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<Warblade>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.MartianWalker:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<Wingman>(), Main.expertMode ? 4 : 7);
                    break;

                case NPCID.GiantCursedSkull:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<Keelhaul>(), CalamityWorld.downedLeviathan, Main.expertMode ? 15 : 25, 1, 1);
                    break;

                case NPCID.Necromancer:
                case NPCID.NecromancerArmored:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<WrathoftheAncients>(), Main.expertMode ? 15 : 25);
                    break;

                case NPCID.DeadlySphere:
                    float defectiveDropRate = Main.expertMode ? 0.33f : 0.2f;
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DefectiveSphere>(), defectiveDropRate);
                    break;

                case NPCID.BloodJelly:
                case NPCID.FungoFish:
                    DropHelper.DropItemChance(npc, ItemID.JellyfishNecklace, 0.01f);
                    break;

                case NPCID.Goldfish:
                case NPCID.GoldfishWalker:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<PineapplePet>(), 0.002f);
                    break;

                default:
                    break;
            }

            // Every type of Moss Hornet counts for the Needler
            if (CalamityLists.mossHornetList.Contains(npc.type))
            {
                int needlerDropRate = Main.expertMode ? 15 : 25;
                DropHelper.DropItemChance(npc, ModContent.ItemType<Needler>(), needlerDropRate);
            }

            // Every type of Skeleton counts for the Waraxe and Ancient Bone Dust
            if (CalamityLists.skeletonList.Contains(npc.type))
            {
                DropHelper.DropItemCondition(npc, ModContent.ItemType<Waraxe>(), !Main.hardMode, Main.expertMode ? 15 : 25, 1, 1);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AncientBoneDust>(), Main.expertMode ? 3 : 5);
            }
        }
        #endregion

        #region Common Loot
        private void CommonLoot(NPC npc)
        {
            switch (npc.type)
            {
                case NPCID.Vulture:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DesertFeather>(), 2, 1, Main.expertMode ? 2 : 1);
                    break;

                case NPCID.RedDevil:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<DemonicBoneAsh>(), Main.expertMode ? 2 : 3);
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofChaos>(), Main.expertMode ? 1f : 0.5f);
                    break;

                case NPCID.WyvernHead:
                    DropHelper.DropItem(npc, ModContent.ItemType<EssenceofCinder>(), 1, Main.expertMode ? 2 : 1);
                    break;

                case NPCID.AngryNimbus:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofCinder>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.IcyMerman:
                case NPCID.IceTortoise:
                case NPCID.IceElemental:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<EssenceofEleum>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.IceGolem:
                    DropHelper.DropItem(npc, ModContent.ItemType<EssenceofEleum>(), 1, 2);
                    break;

                case NPCID.SolarSpearman: //Drakanian
                case NPCID.SolarSolenian: //Selenian
                case NPCID.SolarCorite:
                case NPCID.SolarSroller:
                case NPCID.SolarDrakomireRider:
                case NPCID.SolarDrakomire:
                case NPCID.SolarCrawltipedeHead:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MeldBlob>(), Main.expertMode ? 4 : 5);
                    DropHelper.DropItemChance(npc, ItemID.FragmentSolar, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.VortexSoldier: //Vortexian
                case NPCID.VortexLarva: //Alien Larva
                case NPCID.VortexHornet: //Alien Hornet
                case NPCID.VortexHornetQueen: //Alien Queen
                case NPCID.VortexRifleman: //Storm Diver
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MeldBlob>(), Main.expertMode ? 4 : 5);
                    DropHelper.DropItemChance(npc, ItemID.FragmentVortex, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.NebulaBrain: //Nebula Floater
                case NPCID.NebulaSoldier: //Predictor
                case NPCID.NebulaHeadcrab: //Brain Suckler
                case NPCID.NebulaBeast: //Evolution Beast
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MeldBlob>(), Main.expertMode ? 4 : 5);
                    DropHelper.DropItemChance(npc, ItemID.FragmentNebula, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.StardustSoldier: //Stargazer
                case NPCID.StardustSpiderBig: //Twinkle Popper
                case NPCID.StardustJellyfishBig: //Flow Invader
                case NPCID.StardustCellBig: //Star Cell
                case NPCID.StardustWormHead: //Milkyway Weaver
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MeldBlob>(), Main.expertMode ? 4 : 5);
                    DropHelper.DropItemChance(npc, ItemID.FragmentStardust, Main.expertMode ? 4 : 5);
                    break;

                case NPCID.AngryTrapper:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<TrapperBulb>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.MotherSlime:
                case NPCID.CorruptSlime:
                case NPCID.Crimslime:
                case NPCID.BigCrimslime:
                case NPCID.LittleCrimslime:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MurkySludge>(), Main.expertMode ? 2 : 3);
                    break;

                case NPCID.Derpling:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<BeetleJuice>(), Main.expertMode ? 3 : 4);
                    break;

                case NPCID.SpikedJungleSlime:
                case NPCID.Arapaima:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<MurkyPaste>(), Main.expertMode ? 3 : 4);
                    break;

                case NPCID.Reaper:
                case NPCID.Psycho:
                    DropHelper.DropItemCondition(npc, ModContent.ItemType<SolarVeil>(), CalamityWorld.downedCalamitas || NPC.downedPlantBoss, Main.expertMode ? 0.75f : 0.5f, 1, 4);
                    break;

                case NPCID.MartianOfficer:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ShockGrenade>(), Main.expertMode ? 3 : 4, 3, 8);
                    break;
                case NPCID.BrainScrambler:
                case NPCID.GrayGrunt:
                case NPCID.GigaZapper:
                case NPCID.MartianEngineer:
                case NPCID.RayGunner:
                case NPCID.ScutlixRider:
                    DropHelper.DropItemChance(npc, ModContent.ItemType<ShockGrenade>(), Main.expertMode ? 4 : 5, 1, 4);
                    break;

                case NPCID.Gastropod:
                    DropHelper.DropItem(npc, ItemID.PinkGel, 5, 10);
                    break;

                default:
                    break;
            }

            // All hardmode dungeon enemies drop Ectoblood
            if (CalamityLists.dungeonEnemyBuffList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ModContent.ItemType<Ectoblood>(), 2, 1, Main.expertMode ? 3 : 1);
            }

            // Every type of moss hornet can drop stingers
            if (CalamityLists.mossHornetList.Contains(npc.type))
            {
                DropHelper.DropItemChance(npc, ItemID.Stinger, Main.expertMode ? 1f : 0.6666f);
            }
        }
        #endregion

        #region Town NPC Loot
        private void TownNPCLoot(NPC npc)
        {
            const float TrasherEatDistance = 48f;

            if (npc.type == NPCID.Angler)
            {
                bool fedToTrasher = false;
                for(int i = 0; i < Main.maxNPCs; ++i)
                {
                    NPC nearby = Main.npc[i];
                    if (!nearby.active || nearby.type != ModContent.NPCType<Trasher>())
                        continue;
                    if (npc.Distance(nearby.Center) < TrasherEatDistance)
                    {
                        fedToTrasher = true;
                        break;
                    }
                }

                if (fedToTrasher)
                    DropHelper.DropItemCondition(npc, ItemID.GoldenFishingRod, Main.hardMode);
                else
                    DropHelper.DropItemCondition(npc, ItemID.GoldenFishingRod, Main.hardMode, 12, 1, 1);
            }
        }
        #endregion

        #region Event Loot
        private void EventLoot(NPC npc, bool pumpkin, bool frost, bool eclipse)
        {
            // Nightmare Fuel, Endothermic Energy and Darksun Fragments
            if (!CalamityWorld.downedDoG)
            {
                return;
            }

            if (frost)
            {
                switch (npc.type)
                {
                    case NPCID.Nutcracker:
                    case NPCID.NutcrackerSpinning:
                    case NPCID.ElfCopter:
                    case NPCID.Flocko:
                        DropHelper.DropItemChance(npc, ModContent.ItemType<EndothermicEnergy>(), 2);
                        break;
                    case NPCID.Krampus:
                    case NPCID.Yeti:
                    case NPCID.PresentMimic:
                        DropHelper.DropItemChance(npc, ModContent.ItemType<EndothermicEnergy>(), 2, 1, 2);
                        break;
                    case NPCID.Everscream:
                        DropHelper.DropItem(npc, ModContent.ItemType<EndothermicEnergy>(), 3, 5);
                        break;
                    case NPCID.SantaNK1:
                        DropHelper.DropItem(npc, ModContent.ItemType<EndothermicEnergy>(), 5, 10);
                        break;
                    case NPCID.IceQueen:
                        DropHelper.DropItem(npc, ModContent.ItemType<EndothermicEnergy>(), 10, 20);
                        break;
                }
            }
            else if (pumpkin)
            {
                switch (npc.type)
                {
                    case NPCID.Splinterling:
                        DropHelper.DropItemChance(npc, ModContent.ItemType<NightmareFuel>(), 2);
                        break;
                    case NPCID.Hellhound:
                    case NPCID.Poltergeist:
                        DropHelper.DropItemChance(npc, ModContent.ItemType<NightmareFuel>(), 2, 1, 2);
                        break;
                    case NPCID.HeadlessHorseman:
                        DropHelper.DropItem(npc, ModContent.ItemType<NightmareFuel>(), 3, 5);
                        break;
                    case NPCID.MourningWood:
                        DropHelper.DropItem(npc, ModContent.ItemType<NightmareFuel>(), 5, 10);
                        break;
                    case NPCID.Pumpking:
                        DropHelper.DropItem(npc, ModContent.ItemType<NightmareFuel>(), 10, 20);
                        break;
                }
            }

            if (eclipse)
            {
                switch (npc.type)
                {
                    case NPCID.Frankenstein:
                    case NPCID.SwampThing:
                    case NPCID.Fritz:
                    case NPCID.CreatureFromTheDeep:
                        DropHelper.DropItemChance(npc, ModContent.ItemType<DarksunFragment>(), 10);
                        break;
                    case NPCID.Reaper:
                    case NPCID.Psycho:
                    case NPCID.Vampire:
                    case NPCID.VampireBat:
                    case NPCID.ThePossessed:
                    case NPCID.Butcher:
                    case NPCID.DeadlySphere:
                    case NPCID.DrManFly:
                        DropHelper.DropItemChance(npc, ModContent.ItemType<DarksunFragment>(), 2);
                        break;
                    case NPCID.Eyezor:
                        DropHelper.DropItem(npc, ModContent.ItemType<DarksunFragment>(), 1, 2);
                        break;
                    case NPCID.Nailhead:
                        DropHelper.DropItem(npc, ModContent.ItemType<DarksunFragment>(), 3, 5);
                        break;
                    case NPCID.Mothron:
                        DropHelper.DropItem(npc, ModContent.ItemType<DarksunFragment>(), 20, 30);
                        break;
                }
            }
        }
		#endregion
	}
}
