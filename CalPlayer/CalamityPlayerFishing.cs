using CalamityMod.Items.Accessories;
using CalamityMod.Events;
using CalamityMod.Items.Fishing;
using CalamityMod.Items.Fishing.AstralCatches;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.Fishing.SulphurCatches;
using CalamityMod.Items.Fishing.SunkenSeaCatches;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.NPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.CalPlayer
{
    public partial class CalamityPlayer : ModPlayer
    {
        #region Catch Fish
        public override void CatchFish(FishingAttempt attempt, ref int itemDrop, ref int npcSpawn, ref AdvancedPopupRequest sonar, ref Vector2 sonarPosition)
        {
            int bait = attempt.playerFishingConditions.BaitItemType;
            int power = attempt.playerFishingConditions.BaitPower + attempt.playerFishingConditions.PolePower;
            int questFish = attempt.questFish;
            int poolSize = attempt.waterTilesCount;
            bool water = !attempt.inHoney && !attempt.inLava;
            bool lava = attempt.inLava;
            bool honey = attempt.inHoney;

			// If vanilla catches an enemy, just cancel
			if (npcSpawn > 0)
				return;

			// Set up for allowing fishing in the Sulphurous Sea
            Point point = Player.Center.ToTileCoordinates();
            bool canSulphurFish = false;
            if (Abyss.AtLeftSideOfWorld)
            {
                if (point.X < 380)
                    canSulphurFish = true;
            }
            else
            {
                if (point.X > Main.maxTilesX - 380)
                    canSulphurFish = true;
            }
            if (ZoneAbyss || ZoneSulphur)
                canSulphurFish = true;

            // Old Duke spawn
            if (canSulphurFish && bait == ModContent.ItemType<BloodwormItem>() && water && !BossRushEvent.BossRushActive)
            {
                CalamityGlobalNPC.OldDukeSpawn(Player.whoAmI, ModContent.NPCType<OldDuke>(), bait);
            }

			// Don't do anything if you can't fish in lava
			if (!attempt.CanFishInLava && lava)
				return;

			// If you caught junk, then ignore all Calamity catches
			if (itemDrop == ItemID.OldShoe || itemDrop == ItemID.FishingSeaweed || itemDrop == ItemID.TinCan)
				return;

            // Handle our modded Quest Fish, You can catch these in any liquid... because I don't care if you can
            if (ZoneSunkenSea && questFish == ModContent.ItemType<EutrophicSandfish>() && attempt.uncommon)
            {
                itemDrop = ModContent.ItemType<EutrophicSandfish>();
                return;
            }
            if (ZoneSunkenSea && questFish == ModContent.ItemType<SurfClam>() && attempt.uncommon)
            {
                itemDrop = ModContent.ItemType<SurfClam>();
                return;
            }
            if (ZoneSunkenSea && questFish == ModContent.ItemType<Serpentuna>() && attempt.uncommon)
            {
                itemDrop = ModContent.ItemType<Serpentuna>();
                return;
            }
            if (ZoneCalamity && questFish == ModContent.ItemType<Brimlish>() && attempt.uncommon)
            {
                itemDrop = ModContent.ItemType<Brimlish>();
                return;
            }
            if (ZoneCalamity && questFish == ModContent.ItemType<Slurpfish>() && attempt.uncommon)
            {
                itemDrop = ModContent.ItemType<Slurpfish>();
                return;
            }

			// Handle Alluring Bait's increased chance for Potion Material fish
            if (alluringBait)
            {
                int chanceForPotionFish = 1000 / power;

                if (chanceForPotionFish < 3)
                    chanceForPotionFish = 3;

                if (Main.rand.NextBool(chanceForPotionFish))
                {
                    List<int> fishList = new List<int>();

                    if (lava)
                    {
                        fishList.AddWithCondition<int>(ItemID.FlarefinKoi, !ZoneCalamity);
                        fishList.AddWithCondition<int>(ItemID.Obsidifish, !ZoneCalamity);
                        fishList.AddWithCondition<int>(ModContent.ItemType<CoastalDemonfish>(), ZoneCalamity);
                        fishList.AddWithCondition<int>(ModContent.ItemType<Shadowfish>(), ZoneCalamity);
                    }
                    else if (water)
                    {
                        if (Player.ZoneDirtLayerHeight || Player.ZoneRockLayerHeight)
                        {
                            fishList.Add(ItemID.ArmoredCavefish);
                            fishList.Add(ItemID.Stinkfish);
                            fishList.Add(ItemID.SpecularFish);
                            fishList.AddWithCondition<int>(ItemID.ChaosFish, Player.ZoneHallow);
                            fishList.AddWithCondition<int>(ItemID.VariegatedLardfish, Player.ZoneJungle);
                        }
                        if (Player.ZoneOverworldHeight || Player.ZoneSkyHeight)
                        {
                            fishList.AddWithCondition<int>(ItemID.DoubleCod, Player.ZoneJungle);
                        }
                        fishList.AddWithCondition<int>(ItemID.FrostMinnow, Player.ZoneSnow);
                        fishList.AddWithCondition<int>(ItemID.Ebonkoi, Player.ZoneCorrupt);
                        fishList.AddWithCondition<int>(ItemID.CrimsonTigerfish, Player.ZoneCrimson);
                        fishList.AddWithCondition<int>(ItemID.Hemopiranha, Player.ZoneCrimson);
                        fishList.AddWithCondition<int>(ItemID.PrincessFish, Player.ZoneHallow);
                        fishList.AddWithCondition<int>(ItemID.Prismite, Player.ZoneHallow);
                        fishList.AddWithCondition<int>(ItemID.Damselfish, Player.ZoneSkyHeight);
                        fishList.AddWithCondition<int>(ModContent.ItemType<AldebaranAlewife>(), ZoneAstral);
                        fishList.AddWithCondition<int>(ModContent.ItemType<SunkenSailfish>(), ZoneSunkenSea);
                        fishList.AddWithCondition<int>(ModContent.ItemType<Shadowfish>(), !Main.dayTime && !Player.ZoneSkyHeight);
                    }

                    if (fishList.Any())
                    {
                        int fishAmt = fishList.Count;
                        int caughtFish = fishList[Main.rand.Next(fishAmt)];
                        itemDrop = caughtFish;
                        return;
                    }
                }
            }

			// Handle the increased chance of crates from Enchanted Pearl and the Supreme Fishing Station
            if (enchantedPearl || fishingStation)
            {
                int chanceForCrates = (enchantedPearl ? 10 : 0) +
                    (fishingStation ? 10 : 0);

                int poolSizeAmt = poolSize / 10;
                if (poolSizeAmt > 100)
                    poolSizeAmt = 100;

                int fishingPowerDivisor = power + poolSizeAmt;

                int chanceForIronCrate = 1000 / fishingPowerDivisor;
                int chanceForBiomeCrate = 2000 / fishingPowerDivisor;
                int chanceForGoldCrate = 3000 / fishingPowerDivisor;
                int chanceForRareItems = 4000 / fishingPowerDivisor;

                if (chanceForIronCrate < 3)
                    chanceForIronCrate = 3;

                if (chanceForBiomeCrate < 4)
                    chanceForBiomeCrate = 4;

                if (chanceForGoldCrate < 5)
                    chanceForGoldCrate = 5;

                if (chanceForRareItems < 6)
                    chanceForRareItems = 6;

                if (lava)
                {
                    if (Main.rand.Next(100) < chanceForCrates)
                    {
                        if (Main.rand.NextBool(chanceForBiomeCrate))
                        {
                            if (ZoneCalamity)
                                itemDrop = ModContent.ItemType<BrimstoneCrate>();
							else
								itemDrop = Main.hardMode ? ItemID.LavaCrateHard : ItemID.LavaCrate;
                        }
                    }
                }

                if (water)
                {
                    if (Main.rand.Next(100) < chanceForCrates)
                    {
                        if (Main.rand.NextBool(chanceForRareItems) && enchantedPearl && fishingStation && Player.cratePotion)
                        {
                            List<int> rareItemList = new List<int>();

                            if (canSulphurFish)
                            {
                                switch (Main.rand.Next(2))
                                {
                                    case 0:
                                        rareItemList.Add(ModContent.ItemType<AlluringBait>());
                                        break;
                                    case 1:
                                        rareItemList.Add(ModContent.ItemType<AbyssalAmulet>());
                                        break;
                                }
                            }
                            if (ZoneAstral)
                            {
                                switch (Main.rand.Next(3))
                                {
                                    case 0:
                                        rareItemList.Add(ModContent.ItemType<GacruxianMollusk>());
                                        break;
                                    case 1:
                                        rareItemList.Add(ModContent.ItemType<PolarisParrotfish>());
                                        break;
                                    case 2:
                                        rareItemList.Add(ModContent.ItemType<UrsaSergeant>());
                                        break;
                                }
                            }
                            if (ZoneSunkenSea)
                            {
                                switch (Main.rand.Next(2))
                                {
                                    case 0:
                                        rareItemList.Add(ModContent.ItemType<SerpentsBite>());
                                        break;
                                    case 1:
                                        rareItemList.Add(ModContent.ItemType<RustedJingleBell>());
                                        break;
                                }
                            }
                            if (Player.ZoneSnow && Player.ZoneRockLayerHeight && (Player.ZoneCorrupt || Player.ZoneCrimson || Player.ZoneHallow))
                            {
                                rareItemList.Add(ItemID.ScalyTruffle);
                            }
                            rareItemList.AddWithCondition<int>(ItemID.Toxikarp, Player.ZoneCorrupt);
                            rareItemList.AddWithCondition<int>(ItemID.Bladetongue, Player.ZoneCrimson);
                            rareItemList.AddWithCondition<int>(ItemID.CrystalSerpent, Player.ZoneHallow);

                            if (rareItemList.Any())
                            {
                                int rareItemAmt = rareItemList.Count;
                                int caughtRareItem = rareItemList[Main.rand.Next(rareItemAmt)];
                                itemDrop = caughtRareItem;
                            }
                        }
                        else if (Main.rand.NextBool(chanceForGoldCrate))
                        {
                            itemDrop = Main.hardMode ? ItemID.GoldenCrateHard : ItemID.GoldenCrate;
                        }
                        else if (Main.rand.NextBool(chanceForBiomeCrate))
                        {
                            List<int> biomeCrateList = new List<int>();

                            biomeCrateList.AddWithCondition<int>(ModContent.ItemType<AstralCrate>(), ZoneAstral);
                            biomeCrateList.AddWithCondition<int>(ModContent.ItemType<SunkenCrate>(), ZoneSunkenSea);
                            biomeCrateList.AddWithCondition<int>(ModContent.ItemType<SulphurousCrate>(), canSulphurFish);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.CorruptFishingCrateHard : ItemID.CorruptFishingCrate, Player.ZoneCorrupt);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.CrimsonFishingCrateHard : ItemID.CrimsonFishingCrate, Player.ZoneCrimson);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.HallowedFishingCrateHard : ItemID.HallowedFishingCrate, Player.ZoneHallow);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.DungeonFishingCrateHard : ItemID.DungeonFishingCrate, Player.ZoneDungeon);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.JungleFishingCrateHard : ItemID.JungleFishingCrate, Player.ZoneJungle);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.FloatingIslandFishingCrateHard : ItemID.FloatingIslandFishingCrate, Player.ZoneSkyHeight);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.FrozenCrateHard : ItemID.FrozenCrate, Player.ZoneSnow);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.OasisCrateHard : ItemID.OasisCrate, Player.ZoneDesert && !Player.ZoneBeach);
                            biomeCrateList.AddWithCondition<int>(Main.hardMode ? ItemID.OceanCrateHard : ItemID.OceanCrate, Player.ZoneBeach);

                            if (biomeCrateList.Any())
                            {
                                int biomeCrateAmt = biomeCrateList.Count;
                                int caughtBiomeCrate = biomeCrateList[Main.rand.Next(biomeCrateAmt)];
                                itemDrop = caughtBiomeCrate;
                            }
                        }
                        else if (Main.rand.NextBool(chanceForIronCrate))
                        {
                            itemDrop = Main.hardMode ? ItemID.IronCrateHard : ItemID.IronCrate;
                        }
                        else
                        {
                            itemDrop = Main.hardMode ? ItemID.WoodenCrateHard : ItemID.WoodenCrate;
                        }
                        return;
                    }
                }
            }

            if (water)
            {
				// Handle Calamity's crates, veryrare and legendary means it's a Golden Crate
				if (attempt.crate)
				{
					if (attempt.rare && !attempt.veryrare && !attempt.legendary)
					{
						if (ZoneAstral)
						{
							itemDrop = ModContent.ItemType<AstralCrate>();
						}
						if (ZoneSunkenSea)
						{
							itemDrop = ModContent.ItemType<SunkenCrate>();
						}
						if (canSulphurFish)
						{
							itemDrop = ModContent.ItemType<SulphurousCrate>();
						}
					}
					return;
				}

				// Don't override the vanilla crates or any special vanilla fishing loot
				List<int> keepCatchList = new List<int>()
				{
					ItemID.WoodenCrate,
					ItemID.WoodenCrateHard,
					ItemID.IronCrate,
					ItemID.IronCrateHard,
					ItemID.GoldenCrate,
					ItemID.GoldenCrateHard,
					ItemID.FrogLeg,
					ItemID.BalloonPufferfish,
					ItemID.ZephyrFish,
					ItemID.CombatBook,
					ItemID.CorruptFishingCrate,
					ItemID.CorruptFishingCrateHard,
					ItemID.CrimsonFishingCrate,
					ItemID.CrimsonFishingCrateHard,
					ItemID.HallowedFishingCrate,
					ItemID.HallowedFishingCrateHard,
					ItemID.DungeonFishingCrate,
					ItemID.DungeonFishingCrateHard,
					ItemID.JungleFishingCrate,
					ItemID.JungleFishingCrateHard,
					ItemID.FloatingIslandFishingCrate,
					ItemID.FloatingIslandFishingCrateHard,
					ItemID.FrozenCrate,
					ItemID.FrozenCrateHard,
					ItemID.OasisCrate,
					ItemID.OasisCrateHard,
					ItemID.OceanCrate,
					ItemID.OceanCrateHard
				};
                if (keepCatchList.Contains(itemDrop))
                {
                    return;
                }

                if ((Player.ZoneCrimson || Player.ZoneCorrupt) && Player.ZoneRockLayerHeight && Main.hardMode)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<FishofNight>();
                    }
                }

                if (Player.ZoneHallow && Player.ZoneRockLayerHeight && Main.hardMode)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<FishofLight>();
                    }
                }

                if (Player.ZoneSkyHeight && Main.hardMode)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = Main.rand.NextBool() ? ModContent.ItemType<SunbeamFish>() : ModContent.ItemType<FishofFlight>();
                    }
                }

				// Increased chance of Enchanted Starfish if you don't have maximum mana
                if (Player.ZoneOverworldHeight && !Main.dayTime)
                {
					int chance = Player.statManaMax < 200 ? 5 : 20;
                    if (attempt.uncommon && Main.rand.NextBool(chance))
                    {
                        itemDrop = ModContent.ItemType<EnchantedStarfish>();
                    }
                }

                if (!Player.ZoneSkyHeight && !Main.dayTime)
                {
                    if (attempt.uncommon && Main.rand.NextBool(10))
                    {
                        itemDrop = ModContent.ItemType<Shadowfish>();
                    }
                }

                if (Player.ZoneOverworldHeight && Main.dayTime)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<StuffedFish>();
                    }
                }

                if (Player.ZoneRockLayerHeight)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<GlimmeringGemfish>();
                    }
                }

                if (Player.ZoneSnow && Main.hardMode)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<FishofEleum>();
                    }
                }

				// Lower chance of Spadefish in Hardmode
                if (Player.ZoneDirtLayerHeight)
                {
					int chance = Main.hardMode ? 10 : 2;
                    if (attempt.veryrare && Main.rand.NextBool(chance))
                    {
                        itemDrop = ModContent.ItemType<Spadefish>();
                    }
                }

                if (Player.FindBuffIndex(BuffID.Gills) > -1 && DownedBossSystem.downedCalamitasClone && (attempt.legendary || (attempt.veryrare && Main.rand.NextBool())))
                {
                    itemDrop = ModContent.ItemType<Floodtide>();
                }

                if (Player.ZoneOverworldHeight && Main.bloodMoon)
                {
                    if (attempt.uncommon && Main.rand.NextBool(7))
                    {
                        itemDrop = ModContent.ItemType<Gorecodile>();
                    }
                }

                if (ZoneAstral) // Astral Infection, fishing in water
                {
					if (attempt.legendary)
                    {
						int legendaryCatch = Utils.SelectRandom(Main.rand, new int[]
						{
							ModContent.ItemType<PolarisParrotfish>(),
							ModContent.ItemType<GacruxianMollusk>(),
							ModContent.ItemType<UrsaSergeant>()
						});
                        itemDrop = legendaryCatch;
                    }
					else if (attempt.uncommon || attempt.rare || attempt.veryrare)
                    {
						int uncommonCatch = Utils.SelectRandom(Main.rand, new int[]
						{
							ModContent.ItemType<ProcyonidPrawn>(),
							ModContent.ItemType<ArcturusAstroidean>(),
							ModContent.ItemType<AldebaranAlewife>()
						});
                        itemDrop = uncommonCatch;
                    }
					else
                    {
                        itemDrop = ModContent.ItemType<TwinklingPollox>();
                        return;
                    }
                }

                if (ZoneSunkenSea) // Sunken Sea, fishing in water
                {
					if (attempt.legendary)
                    {
						List<int> legendaryCatches = new List<int>()
						{
							ModContent.ItemType<RustedJingleBell>(),
							ModContent.ItemType<GreenwaveLoach>()
						};
                        legendaryCatches.AddWithCondition<int>(ModContent.ItemType<SparklingEmpress>(), DownedBossSystem.downedDesertScourge);
                        legendaryCatches.AddWithCondition<int>(ModContent.ItemType<SerpentsBite>(), Main.hardMode);
						itemDrop = legendaryCatches[Main.rand.Next(legendaryCatches.Count)];
                    }
					else if (attempt.uncommon || attempt.rare || attempt.veryrare)
                    {
                        itemDrop = ModContent.ItemType<SunkenSailfish>();
                    }
					else
                    {
                        itemDrop = ModContent.ItemType<PrismaticGuppy>();
                        return;
                    }
                }

				// There is no complete fishing pool here, so most of it is vanilla default
                if (canSulphurFish) // Sulphurous Sea, fishing in water
                {
                    if (attempt.legendary || (attempt.veryrare && Main.rand.NextBool()))
                    {
                        itemDrop = Utils.SelectRandom(Main.rand, new int[]
                        {
                            ModContent.ItemType<AlluringBait>(),
                            ModContent.ItemType<AbyssalAmulet>()
                        });
                    }
                    else if (attempt.common)
                    {
                        itemDrop = ModContent.ItemType<PlantyMush>();
                    }
                }

                /*if (canSulphurFish && (bait.type == ItemID.GoldWorm || bait.type == ItemID.GoldGrasshopper || bait.type == ItemID.GoldButterfly) && power > 150)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        CalamityGlobalNPC.OldDukeSpawn(player.whoAmI, ModContent.NPCType<OldDuke>());
                    }
                    else
                    {
                        NetMessage.SendData(61, -1, -1, null, player.whoAmI, (float)ModContent.NPCType<OldDuke>(), 0f, 0f, 0, 0, 0);
                    }
                    switch (Main.rand.Next(4))
                    {
                        case 0: itemDrop = ModContent.ItemType<IronBoots>(); break; //movement acc
                        case 1: itemDrop = ModContent.ItemType<DepthCharm>(); break; //regen acc
                        case 2: itemDrop = ModContent.ItemType<AnechoicPlating>(); break; //defense acc
                        case 3: itemDrop = ModContent.ItemType<StrangeOrb>(); break; //light pet
                    }
                    return;
                }*/
            }

            if (lava)
            {
                if (ZoneCalamity) // Brimstone Crags, fishing in lava
                {
					if (attempt.crate)
                    {
                        itemDrop = ModContent.ItemType<BrimstoneCrate>();
                    }
					else if (attempt.legendary)
                    {
						List<int> legendaryCatches = new List<int>()
						{
							ModContent.ItemType<CharredLasher>(),
							ModContent.ItemType<DragoonDrizzlefish>()
						};
						itemDrop = legendaryCatches[Main.rand.Next(legendaryCatches.Count)];
                    }
					// Increased chance of Dragoon Drizzlefish in Prehardmode
					else if (attempt.veryrare && !Main.hardMode)
					{
                        itemDrop = ModContent.ItemType<DragoonDrizzlefish>();
					}
					else if ((attempt.rare || attempt.veryrare) && DownedBossSystem.downedProvidence && Main.rand.Next(3) >= 2)
					{
                        itemDrop = ModContent.ItemType<Bloodfin>();
					}
					else if (attempt.uncommon || attempt.rare || attempt.veryrare)
                    {
						List<int> uncommonCatches = new List<int>()
						{
							ModContent.ItemType<CoastalDemonfish>(),
							ModContent.ItemType<Shadowfish>()
						};
                        uncommonCatches.AddWithCondition<int>(ModContent.ItemType<Havocfish>(), Main.hardMode);
						itemDrop = uncommonCatches[Main.rand.Next(uncommonCatches.Count)];
                    }
					else
                    {
                        itemDrop = ModContent.ItemType<CragBullhead>();
                    }
                }
            }
        }
        #endregion

        #region Get Fishing Level
        public override void GetFishingLevel(Item fishingRod, Item bait, ref float fishingLevel)
        {
            if ((ZoneAstral || ZoneAbyss || ZoneSulphur) && bait.type == ModContent.ItemType<ArcturusAstroidean>())
                fishingLevel = fishingLevel * 1.1f;
            if (Player.ZoneSnow && fishingRod.type == ModContent.ItemType<VerstaltiteFishingRod>())
                fishingLevel = fishingLevel * 1.1f;
            if (Player.ZoneSkyHeight && fishingRod.type == ModContent.ItemType<HeronRod>())
                fishingLevel = fishingLevel * 1.1f;

			// Prevent the player from fishing if they have the Bloodworm
            if (bait.type == ModContent.ItemType<BloodwormItem>())
            {
                Point point = Player.Center.ToTileCoordinates();
                bool canSulphurFish = false;
                if (Abyss.AtLeftSideOfWorld)
                {
                    if (point.X < 380)
                        canSulphurFish = true;
                }
                else
                {
                    if (point.X > Main.maxTilesX - 380)
                        canSulphurFish = true;
                }

                if (ZoneAbyss || ZoneSulphur)
                    canSulphurFish = true;

                Item item = Player.ActiveItem();
                if (!canSulphurFish || item.fishingPole <= 0 || item.holdStyle != 1)
                    fishingLevel = -1;

				// If your bait is the Bloodworm, set the Fisherman's Pocket Guide to display Warning!
				// This only happens when a fishing bobber projectile exists
				Player.displayedFishingInfo = Language.GetTextValue("GameUI.FishingWarning");
            }
        }
        #endregion

        #region Get Fishing Level
        public override void ModifyCaughtFish(Item fish)
        {
            if (alluringBait)
            {
				List<int> fishList = new List<int>()
				{
					ItemID.FlarefinKoi,
					ItemID.Obsidifish,
					ItemID.ArmoredCavefish,
					ItemID.Stinkfish,
					ItemID.SpecularFish,
					ItemID.ChaosFish,
					ItemID.VariegatedLardfish,
					ItemID.DoubleCod,
					ItemID.FrostMinnow,
					ItemID.Ebonkoi,
					ItemID.CrimsonTigerfish,
					ItemID.Hemopiranha,
					ItemID.PrincessFish,
					ItemID.Prismite,
					ItemID.Damselfish,
					ModContent.ItemType<CoastalDemonfish>(),
					ModContent.ItemType<Shadowfish>(),
					ModContent.ItemType<AldebaranAlewife>(),
					ModContent.ItemType<SunkenSailfish>(),
				};

				if (fishList.Contains(fish.type))
				{
					// Increase the yield by 1 or 2
					fish.stack += Main.rand.NextBool() ? 1 : 2;
				}
            }
        }
        #endregion
    }
}
