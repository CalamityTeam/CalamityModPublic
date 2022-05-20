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
using CalamityMod.NPCs;
using CalamityMod.NPCs.OldDuke;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems;
using Terraria.DataStructures;

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

            // Quest Fish
            if (ZoneSunkenSea && questFish == ModContent.ItemType<EutrophicSandfish>() && Main.rand.NextBool(10))
            {
                itemDrop = ModContent.ItemType<EutrophicSandfish>();
                return;
            }
            if (ZoneSunkenSea && questFish == ModContent.ItemType<SurfClam>() && Main.rand.NextBool(10))
            {
                itemDrop = ModContent.ItemType<SurfClam>();
                return;
            }
            if (ZoneSunkenSea && questFish == ModContent.ItemType<Serpentuna>() && Main.rand.NextBool(10))
            {
                itemDrop = ModContent.ItemType<Serpentuna>();
                return;
            }
            if (ZoneCalamity && questFish == ModContent.ItemType<Brimlish>() && Main.rand.NextBool(10))
            {
                itemDrop = ModContent.ItemType<Brimlish>();
                return;
            }
            if (ZoneCalamity && questFish == ModContent.ItemType<Slurpfish>() && Main.rand.NextBool(10))
            {
                itemDrop = ModContent.ItemType<Slurpfish>();
                return;
            }

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
                        fishList.AddWithCondition<int>(ModContent.ItemType<BrimstoneFish>(), ZoneCalamity);
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
                        fishList.AddWithCondition<int>(ModContent.ItemType<CoralskinFoolfish>(), ZoneSunkenSea);
                        fishList.AddWithCondition<int>(ModContent.ItemType<SunkenSailfish>(), ZoneSunkenSea);
                        fishList.AddWithCondition<int>(ModContent.ItemType<ScarredAngelfish>(), ZoneSunkenSea);
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
                            itemDrop = ItemID.GoldenCrate;
                        }
                        else if (Main.rand.NextBool(chanceForBiomeCrate))
                        {
                            List<int> biomeCrateList = new List<int>();

                            biomeCrateList.AddWithCondition<int>(ModContent.ItemType<AstralCrate>(), ZoneAstral);
                            biomeCrateList.AddWithCondition<int>(ModContent.ItemType<SunkenCrate>(), ZoneSunkenSea);
                            biomeCrateList.AddWithCondition<int>(ModContent.ItemType<AbyssalCrate>(), canSulphurFish);
                            biomeCrateList.AddWithCondition<int>(ItemID.CorruptFishingCrate, Player.ZoneCorrupt);
                            biomeCrateList.AddWithCondition<int>(ItemID.CrimsonFishingCrate, Player.ZoneCrimson);
                            biomeCrateList.AddWithCondition<int>(ItemID.HallowedFishingCrate, Player.ZoneHallow);
                            biomeCrateList.AddWithCondition<int>(ItemID.DungeonFishingCrate, Player.ZoneDungeon);
                            biomeCrateList.AddWithCondition<int>(ItemID.JungleFishingCrate, Player.ZoneJungle);
                            biomeCrateList.AddWithCondition<int>(ItemID.FloatingIslandFishingCrate, Player.ZoneSkyHeight);

                            if (biomeCrateList.Any())
                            {
                                int biomeCrateAmt = biomeCrateList.Count;
                                int caughtBiomeCrate = biomeCrateList[Main.rand.Next(biomeCrateAmt)];
                                itemDrop = caughtBiomeCrate;
                            }
                        }
                        else if (Main.rand.NextBool(chanceForIronCrate))
                        {
                            itemDrop = ItemID.IronCrate;
                        }
                        else
                        {
                            itemDrop = ItemID.WoodenCrate;
                        }
                        return;
                    }
                }
            }

            if (water)
            {
                if (itemDrop == ItemID.WoodenCrate || itemDrop == ItemID.IronCrate || itemDrop == ItemID.GoldenCrate || itemDrop == ItemID.FrogLeg || itemDrop == ItemID.BalloonPufferfish || itemDrop == ItemID.ZephyrFish)
                {
                    return;
                }
                if ((Player.ZoneCrimson || Player.ZoneCorrupt) && Player.ZoneRockLayerHeight && Main.hardMode)
                {
                    if (Main.rand.NextBool(25))
                    {
                        itemDrop = ModContent.ItemType<FishofNight>();
                    }
                }

                if (Player.ZoneHallow && Player.ZoneRockLayerHeight && Main.hardMode)
                {
                    if (Main.rand.NextBool(25))
                    {
                        itemDrop = ModContent.ItemType<FishofLight>();
                    }
                }

                if (Player.ZoneSkyHeight && Main.hardMode)
                {
                    if (Main.rand.NextBool(25))
                    {
                        itemDrop = Main.rand.NextBool() ? ModContent.ItemType<SunbeamFish>() : ModContent.ItemType<FishofFlight>();
                    }
                }

                if (Player.ZoneOverworldHeight && !Main.dayTime)
                {
                    if (Main.rand.NextBool(10))
                    {
                        itemDrop = ModContent.ItemType<EnchantedStarfish>();
                    }
                }

                if (Player.ZoneOverworldHeight && Main.dayTime)
                {
                    if (Main.rand.NextBool(25))
                    {
                        itemDrop = ModContent.ItemType<StuffedFish>();
                    }
                }

                if (Player.ZoneRockLayerHeight)
                {
                    if (Main.rand.NextBool(25))
                    {
                        itemDrop = ModContent.ItemType<GlimmeringGemfish>();
                    }
                }

                if (Player.ZoneSnow)
                {
                    if (Main.rand.NextBool(25) && Main.hardMode)
                    {
                        itemDrop = ModContent.ItemType<FishofEleum>();
                    }
                }

                if (Player.ZoneDirtLayerHeight)
                {
                    if (Main.rand.NextBool(Main.hardMode ? 100 : 40))
                    {
                        itemDrop = ModContent.ItemType<Spadefish>();
                    }
                }

                if (ZoneAstral) // Astral Infection, fishing in water
                {
                    int astralFish = Main.rand.Next(100);
                    if (astralFish >= 85) // 15%
                    {
                        itemDrop = ModContent.ItemType<ProcyonidPrawn>();
                    }
                    else if (astralFish <= 84 && astralFish >= 70) // 15%
                    {
                        itemDrop = ModContent.ItemType<ArcturusAstroidean>();
                    }
                    else if (astralFish <= 69 && astralFish >= 55) // 15%
                    {
                        itemDrop = ModContent.ItemType<AldebaranAlewife>();
                    }
                    else if (Player.cratePotion && astralFish <= 18 && astralFish >= 9) // 10%
                    {
                        itemDrop = ModContent.ItemType<AstralCrate>();
                    }
                    else if (!Player.cratePotion && astralFish <= 13 && astralFish >= 9) // 5%
                    {
                        itemDrop = ModContent.ItemType<AstralCrate>();
                    }
                    else if (astralFish <= 8 && astralFish >= 6) // 3%
                    {
                        itemDrop = ModContent.ItemType<UrsaSergeant>();
                    }
                    else if (astralFish <= 5 && astralFish >= 3) // 3%
                    {
                        itemDrop = ModContent.ItemType<GacruxianMollusk>();
                    }
                    else if (astralFish <= 2 && astralFish >= 0) // 3%
                    {
                        itemDrop = ModContent.ItemType<PolarisParrotfish>();
                    }
                    else // 36% w/o crate pot, 31% w/ crate pot
                    {
                        itemDrop = ModContent.ItemType<TwinklingPollox>();
                        return;
                    }
                }

                if (ZoneSunkenSea) // Sunken Sea, fishing in water
                {
                    int sunkenFish = Main.rand.Next(100);
                    if (sunkenFish >= 85 && Main.hardMode) // 15%
                    {
                        itemDrop = ModContent.ItemType<ScarredAngelfish>();
                    }
                    else if (sunkenFish <= 84 && sunkenFish >= 70) // 15%
                    {
                        itemDrop = ModContent.ItemType<SunkenSailfish>();
                    }
                    else if (sunkenFish <= 69 && sunkenFish >= 55) // 15%
                    {
                        itemDrop = ModContent.ItemType<CoralskinFoolfish>();
                    }
                    else if (Player.cratePotion && sunkenFish <= 18 && sunkenFish >= 9) // 10%
                    {
                        itemDrop = ModContent.ItemType<SunkenCrate>();
                    }
                    else if (!Player.cratePotion && sunkenFish <= 13 && sunkenFish >= 9) // 5%
                    {
                        itemDrop = ModContent.ItemType<SunkenCrate>();
                    }
                    else if (sunkenFish <= 31 && sunkenFish >= 29) // 3%
                    {
                        itemDrop = ModContent.ItemType<GreenwaveLoach>();
                    }
                    else if (sunkenFish <= 8 && sunkenFish >= 6 && Main.hardMode) // 3%
                    {
                        itemDrop = ModContent.ItemType<SerpentsBite>();
                    }
                    else if (sunkenFish <= 5 && sunkenFish >= 3) // 3%
                    {
                        itemDrop = ModContent.ItemType<RustedJingleBell>();
                    }
                    else if (sunkenFish <= 2 && sunkenFish >= 0 && DownedBossSystem.downedDesertScourge) // 3%
                    {
                        itemDrop = ModContent.ItemType<SparklingEmpress>();
                    }
                    else // 38% w/o crate pot, 33% w/ crate pot + 18% if prehardmode
                    {
                        itemDrop = ModContent.ItemType<PrismaticGuppy>();
                        return;
                    }
                }

                if (Player.FindBuffIndex(BuffID.Gills) > -1 && DownedBossSystem.downedCalamitas && Main.rand.NextBool(25))
                {
                    itemDrop = ModContent.ItemType<Floodtide>();
                }

                if (attempt.common)
                {
                    if (canSulphurFish)
                    {
                        itemDrop = ModContent.ItemType<PlantyMush>();
                    }
                    return;
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

                if (canSulphurFish)
                {
                    if (Main.rand.NextBool(15))
                    {
                        itemDrop = ModContent.ItemType<PlantyMush>();
                    }
                    if (Main.rand.NextFloat() < 0.08f)
                    {
                        itemDrop = Utils.SelectRandom(Main.rand, new int[]
                        {
                            ModContent.ItemType<AlluringBait>(),
                            ModContent.ItemType<AbyssalAmulet>()
                        });
                    }
                    if (Player.cratePotion && Main.rand.NextBool(5)) // 20%
                    {
                        itemDrop = ModContent.ItemType<AbyssalCrate>();
                    }
                    else if (!Player.cratePotion && Main.rand.NextBool(10)) // 10%
                    {
                        itemDrop = ModContent.ItemType<AbyssalCrate>();
                    }
                }

                if (Player.ZoneOverworldHeight && Main.bloodMoon)
                {
                    if (Main.rand.NextBool(25))
                    {
                        itemDrop = ModContent.ItemType<Xerocodile>();
                    }
                }
            }

            if (lava)
            {
                if (ZoneCalamity) // Brimstone Crags, fishing in lava
                {
                    int cragFish = Main.rand.Next(100);
                    if (cragFish >= 85) // 15%
                    {
                        itemDrop = ModContent.ItemType<CoastalDemonfish>();
                    }
                    else if (cragFish <= 84 && cragFish >= 70) // 15%
                    {
                        itemDrop = ModContent.ItemType<BrimstoneFish>();
                    }
                    else if (cragFish <= 69 && cragFish >= 55) // 15%
                    {
                        itemDrop = ModContent.ItemType<Shadowfish>();
                    }
                    else if (cragFish <= 54 && cragFish >= 41 && Main.hardMode) // 14%
                    {
                        itemDrop = ModContent.ItemType<ChaoticFish>();
                    }
                    else if (Player.cratePotion && cragFish <= 30 && cragFish >= 21) // 10%
                    {
                        itemDrop = ModContent.ItemType<BrimstoneCrate>();
                    }
                    else if (!Player.cratePotion && cragFish <= 25 && cragFish >= 21) // 5%
                    {
                        itemDrop = ModContent.ItemType<BrimstoneCrate>();
                    }
                    else if (cragFish <= 20 && cragFish >= 11 && DownedBossSystem.downedProvidence) // 10%
                    {
                        itemDrop = ModContent.ItemType<Bloodfin>();
                    }
                    else if (cragFish <= 10 && cragFish >= (Main.hardMode ? 8 : 6)) // 5% (3% hardmode)
                    {
                        itemDrop = ModContent.ItemType<DragoonDrizzlefish>();
                    }
                    else if (cragFish <= 2 && cragFish >= 0) // 3%
                    {
                        itemDrop = ModContent.ItemType<CharredLasher>();
                    }
                    else // 32% w/o crate pot, 27% w/ crate pot, add 10% pre-Prov, add another 12% prehardmode
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
                fishingLevel = (int)(fishingLevel * 1.1f);
            if (Player.ZoneSnow && fishingRod.type == ModContent.ItemType<VerstaltiteFishingRod>())
                fishingLevel = (int)(fishingLevel * 1.1f);
            if (Player.ZoneSkyHeight && fishingRod.type == ModContent.ItemType<HeronRod>())
                fishingLevel = (int)(fishingLevel * 1.1f);

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
            }
        }
        #endregion
    }
}
