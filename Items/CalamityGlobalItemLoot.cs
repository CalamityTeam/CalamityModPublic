using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class CalamityGlobalItemLoot : GlobalItem
    {
        public override bool InstancePerEntity => false;

        #region Modify Item Loot Main Hook
        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            switch (item.type)
            {
                #region Boss Treasure Bags
                case ItemID.KingSlimeBossBag:
                    loot.Add(new CommonDrop(ModContent.ItemType<CrownJewel>(), 10)); // 10% Crown Jewel
                    break;

                case ItemID.EyeOfCthulhuBossBag:
                    loot.Add(ModContent.ItemType<DeathstareRod>(), DropHelper.BagWeaponDropRateInt); // 33% Deathstare Rod
                    loot.Add(ModContent.ItemType<TeardropCleaver>(), 10); // 10% Teardrop Cleaver
                    break;

                // On Rev+, Eater of Worlds segments don't drop partial loot. As such, the bag needs to drop all materials.
                // This can theoretically be exploited by killing the boss on Expert, then turning on Rev to open the bags.
                // We don't care.
                case ItemID.EaterOfWorldsBossBag:
                    var eowRevLCR = loot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.revenge));
                    eowRevLCR.Add(ItemID.DemoniteOre, 1, 120, 240); // 100% 120-240 Demonite Ore
                    eowRevLCR.Add(ItemID.ShadowScale, 1, 60, 120); // 100% 60-120 Shadow Scale
                    break;


                // On Rev+, Brain of Cthulhu's Creepers don't drop partial loot. As such, the bag needs to drop all materials.
                // This can theoretically be exploited by killing the boss on Expert, then turning on Rev to open the bags.
                // We don't care.
                case ItemID.BrainOfCthulhuBossBag:
                    var bocRevLCR = loot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.revenge));
                    bocRevLCR.Add(ItemID.DemoniteOre, 1, 100, 180); // 100% 100-180 Crimtane Ore
                    bocRevLCR.Add(ItemID.TissueSample, 1, 60, 120); // 100% 60-120 Tissue Sample
                    break;

                case ItemID.DeerclopsBossBag:
                    loot.Remove(FindDeerclopsWeapons(loot));
                    int[] deerclopsWeapons = new int[]
                    {
                        ItemID.LucyTheAxe,
                        ItemID.PewMaticHorn,
                        ItemID.WeatherPain,
                        ItemID.HoundiusShootius
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, deerclopsWeapons));
                    break;

                case ItemID.QueenBeeBossBag:
                    loot.Remove(FindQueenBeeWeapons(loot));
                    int[] queenBeeWeapons = new int[]
                    {
                        ItemID.BeeKeeper,
                        ItemID.BeesKnees,
                        ItemID.BeeGun
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, queenBeeWeapons));
                    loot.Add(ModContent.ItemType<TheBee>(), 10); // 10% The Bee
                    loot.Add(ItemID.Stinger, 1, 8, 12); // 100% 8-12 Stinger
                    loot.Add(ModContent.ItemType<HardenedHoneycomb>(), 1, 50, 75); // 100% 50-75 Hardened Honeycomb
                    break;

                case ItemID.WallOfFleshBossBag:
                    loot.Remove(FindWallOfFleshWeapons(loot));
                    loot.Remove(FindWallOfFleshEmblems(loot));
                    int[] wofWeapons = new int[]
                    {
                        ItemID.BreakerBlade,
                        ItemID.ClockworkAssaultRifle,
                        ModContent.ItemType<Meowthrower>(),
                        ItemID.LaserRifle,
                        ModContent.ItemType<BlackHawkRemote>(),
                        ModContent.ItemType<BlastBarrel>()
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, wofWeapons));
                    loot.Add(ModContent.ItemType<Carnage>(), 10); // 10% Carnage

                    int[] emblems = new int[]
                    {
                        ItemID.WarriorEmblem,
                        ItemID.RangerEmblem,
                        ItemID.SorcererEmblem,
                        ItemID.SummonerEmblem,
                        ModContent.ItemType<RogueEmblem>(),
                    };
                    loot.Add(DropHelper.CalamityStyle(new Fraction(1, 4), emblems)); // Emblems remain 25%
                    break;

                case ItemID.QueenSlimeBossBag:
                    loot.Add(ItemID.SoulofLight, 1, 15, 20); // 100% 15-20 Soul of Light
                    break;

                case ItemID.DestroyerBossBag:
                    loot.Remove(FindHallowedBars(loot));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    break;

                case ItemID.TwinsBossBag:
                    loot.Remove(FindHallowedBars(loot));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    loot.Add(ModContent.ItemType<Arbalest>(), 10); // 10% Arbalest
                    break;

                case ItemID.SkeletronPrimeBossBag:
                    loot.Remove(FindHallowedBars(loot));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    break;

                case ItemID.PlanteraBossBag:
                    loot.Remove(FindPlanteraWeapons(loot));
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
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, planteraWeapons));
                    loot.Add(ModContent.ItemType<BlossomFlux>(), 10); // 10% Blossom Flux
                    loot.Add(ModContent.ItemType<BloomStone>(), DropHelper.BagWeaponDropRateFraction);
                    loot.Add(ModContent.ItemType<LivingShard>(), 1, 30, 35);
                    break;

                case ItemID.GolemBossBag:
                    loot.Remove(FindGolemItems(loot));
                    int[] golemItems = new int[]
                    {
                        ItemID.GolemFist,
                        ItemID.PossessedHatchet,
                        ItemID.Stynger,
                        ItemID.HeatRay,
                        ItemID.StaffofEarth,
                        ItemID.EyeoftheGolem,
                        ItemID.SunStone
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, golemItems));
                    loot.Add(ModContent.ItemType<AegisBlade>(), 10); // 10% Aegis Blade
                    loot.Add(ModContent.ItemType<EssenceofSunlight>(), 1, 8, 13); // 100% 8-13 Essence of Sunlight
                    break;

                case ItemID.BossBagBetsy:
                    loot.Remove(FindBetsyWeapons(loot));
                    int[] betsyWeapons = new int[]
                    {
                        ItemID.DD2SquireBetsySword, // Flying Dragon
                        ItemID.MonkStaffT3, // Sky Dragon's Fury
                        ItemID.DD2BetsyBow, // Aerial Bane
                        ItemID.ApprenticeStaffT3, // Betsy's Wrath
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, betsyWeapons));
                    break;

                case ItemID.FishronBossBag:
                    RemoveDukeRules(loot); // Separately remove Duke's weapons and wings, because they're both included below
                    int[] dukeItems = new int[]
                    {
                        ItemID.Flairon,
                        ItemID.Tsunami,
                        ItemID.BubbleGun,
                        ItemID.RazorbladeTyphoon,
                        ItemID.TempestStaff,
                        ModContent.ItemType<DukesDecapitator>(),
                        ItemID.FishronWings, // Duke's wings have a pathetically low drop rate.
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, dukeItems));
                    loot.Add(ModContent.ItemType<BrinyBaron>(), 10); // 10% Briny Baron
                    break;

                case ItemID.FairyQueenBossBag:
                    RemoveEmpressRules(loot); // Separately remove EoL's weapons and wings, because they're both included below
                    int[] empressItems = new int[]
                    {
                        ItemID.PiercingStarlight, // Starlight
                        ItemID.FairyQueenRangedItem, // Eventide
                        ItemID.FairyQueenMagicItem, // Nightglow
                        ItemID.SparkleGuitar, // Stellar Tune
                        ItemID.EmpressBlade, // Terraprisma
                        ItemID.RainbowWhip, // Kaleidoscope
                        ItemID.RainbowWings, // Empress Wings have a pathetically low drop rate.
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, empressItems));
                    break;

                case ItemID.MoonLordBossBag:
                    loot.Remove(FindMoonLordWeapons(loot));
                    int[] moonLordWeapons = new int[]
                    {
                        ItemID.Meowmere,
                        ItemID.StarWrath,
                        ItemID.Terrarian,
                        ItemID.Celeb2, // Celebration Mk II
                        ItemID.SDMG,
                        ItemID.LastPrism,
                        ItemID.LunarFlareBook,
                        ItemID.MoonlordTurretStaff, // Lunar Portal Staff
                        ItemID.RainbowCrystalStaff,
                        ModContent.ItemType<UtensilPoker>(),
                    };
                    loot.Add(DropHelper.CalamityStyle(DropHelper.BagWeaponDropRateFraction, moonLordWeapons));

                    // The Celestial Onion only drops if the player hasn't used one.
                    loot.AddIf((info) => !info.player.Calamity().extraAccessoryML, ModContent.ItemType<CelestialOnion>());
                    break;
                #endregion

                #region Fishing Crates
                /*
                case ItemID.WoodenCrate:
                case ItemID.WoodenCrateHard:
                    DropHelper.DropItemChance(s, player, ModContent.ItemType<WulfrumMetalScrap>(), 0.25f, 3, 5);
                    break;

                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    DropHelper.DropItemChance(s, player, ModContent.ItemType<WulfrumMetalScrap>(), 0.25f, 5, 8);
                    DropHelper.DropItemChance(s, player, ModContent.ItemType<AncientBoneDust>(), 0.25f, 5, 8);
                    break;

                case ItemID.GoldenCrate:
                    DropHelper.DropItemChance(s, player, ItemID.FlareGun, 0.1f, 1, 1);
                    DropHelper.DropItemChance(s, player, ItemID.ShoeSpikes, 0.1f, 1, 1);
                    DropHelper.DropItemChance(s, player, ItemID.BandofRegeneration, 0.1f, 1, 1);
                    break;

                case ItemID.GoldenCrateHard:
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<AuricOre>(), DownedBossSystem.downedYharon, 0.15f, 30, 40);
                    break;

                case ItemID.CorruptFishingCrate:
                case ItemID.CrimsonFishingCrate:
                case ItemID.CorruptFishingCrateHard:
                case ItemID.CrimsonFishingCrateHard:
                    DropHelper.DropItemChance(s, player, ModContent.ItemType<BlightedGel>(), 0.15f, 5, 8);
                    break;

                case ItemID.HallowedFishingCrate: // WHY
                case ItemID.HallowedFishingCrateHard:
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<UnholyEssence>(), DownedBossSystem.downedProvidence, 0.15f, 5, 10);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<ProfanedRagePotion>(), DownedBossSystem.downedProvidence, 0.15f, 1, 2);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<HolyWrathPotion>(), DownedBossSystem.downedProvidence, 0.15f, 1, 2);
                    break;

                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                    DropHelper.DropItemCondition(s, player, ItemID.Ectoplasm, NPC.downedPlantBoss, 0.1f, 1, 5);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<Phantoplasm>(), DownedBossSystem.downedPolterghast, 0.1f, 1, 5);
                    break;

                case ItemID.JungleFishingCrate:
                case ItemID.JungleFishingCrateHard:
                    DropHelper.DropItemChance(s, player, ModContent.ItemType<MurkyPaste>(), 0.2f, 1, 3);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<BeetleJuice>(), Main.hardMode, 0.2f, 1, 3);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<TrapperBulb>(), Main.hardMode, 0.2f, 1, 3);
                    DropHelper.DropItemCondition(s, player, ItemID.ChlorophyteOre, DownedBossSystem.downedCalamitas || NPC.downedPlantBoss, 0.2f, 16, 28);
                    DropHelper.DropItemCondition(s, player, ItemID.ChlorophyteBar, DownedBossSystem.downedCalamitas || NPC.downedPlantBoss, 0.15f, 4, 7);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<PerennialOre>(), NPC.downedPlantBoss, 0.2f, 16, 28);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<PerennialBar>(), NPC.downedPlantBoss, 0.15f, 4, 7);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<PlagueCellCanister>(), NPC.downedGolemBoss, 0.2f, 3, 6);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<UelibloomOre>(), DownedBossSystem.downedProvidence, 0.2f, 16, 28);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<UelibloomBar>(), DownedBossSystem.downedProvidence, 0.15f, 4, 7);
                    break;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<AerialiteOre>(), DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator, 0.2f, 16, 28);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<AerialiteBar>(), DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator, 0.15f, 4, 7);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<EssenceofSunlight>(), Main.hardMode, 0.2f, 2, 4);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<GalacticaSingularity>(), NPC.downedMoonlord, 0.1f, 1, 3);
                    break;

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    int numMechsDown = NPC.downedMechBoss1.ToInt() + NPC.downedMechBoss2.ToInt() + NPC.downedMechBoss3.ToInt();
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<CryonicOre>(), DownedBossSystem.downedCryogen && numMechsDown >= 2, 0.2f, 16, 28);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<CryonicBar>(), DownedBossSystem.downedCryogen && numMechsDown >= 2, 0.15f, 4, 7);
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<EssenceofEleum>(), Main.hardMode, 0.2f, 2, 4);
                    break;

                case ItemID.LavaCrate:
                case ItemID.LavaCrateHard:
                    DropHelper.DropItemCondition(s, player, ModContent.ItemType<EssenceofChaos>(), Main.hardMode, 0.2f, 2, 4);
                    break;

                // Calamity does not touch Oasis Crates yet
                case ItemID.OasisCrate:
                case ItemID.OasisCrateHard:
                    break;

                // Calamity does not touch Ocean Crates yet
                case ItemID.OceanCrate:
                case ItemID.OceanCrateHard:
                    break;
                */
                #endregion

                #region Miscellaneous
                // Bat Hook is now acquired from Vampires.
                case ItemID.GoodieBag:
                    RemoveBatHookFromGoodieBag(loot);
                    break;
                #endregion
            }
        }
        #endregion

        #region Calamity Style Items From Bosses
        private static IItemDropRule FindDeerclopsWeapons(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.LucyTheAxe)
                            return o;
            return null;
        }

        private static IItemDropRule FindQueenBeeWeapons(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.BeeKeeper)
                            return o;
            return null;
        }

        private static IItemDropRule FindWallOfFleshWeapons(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.BreakerBlade)
                            return o;
            return null;
        }

        private static IItemDropRule FindWallOfFleshEmblems(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.WarriorEmblem)
                            return o;
            return null;
        }

        private static IItemDropRule FindHallowedBars(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is CommonDrop c && c.itemId == ItemID.HallowedBar)
                    return c;
            return null;
        }

        private static IItemDropRule FindPlanteraWeapons(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromRulesRule o)
                    foreach (IItemDropRule r2 in o.options)
                        // Specifically look up the Grenade Launcher because it has unique rules for its ammo.
                        if (r2 is CommonDrop c && c.itemId == ItemID.GrenadeLauncher)
                            return o;
            return null;
        }

        private static IItemDropRule FindGolemItems(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromRulesRule o)
                    foreach (IItemDropRule r2 in o.options)
                        // Specifically look up the Stynger because it has unique rules for its ammo.
                        if (r2 is CommonDrop c && c.itemId == ItemID.Stynger)
                            return o;
            return null;
        }

        private static IItemDropRule FindBetsyWeapons(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.DD2SquireBetsySword) // Flying Dragon
                            return o;
            return null;
        }

        private static void RemoveDukeRules(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            IItemDropRule toRemove = null;

            // Step 1: Remove Fishron Wings loot rule
            foreach (IItemDropRule rule in rules)
                if (rule is CommonDropNotScalingWithLuck c && c.itemId == ItemID.FishronWings)
                    toRemove = c;

            if (toRemove is not null)
                loot.Remove(toRemove);

            // Step 2: Remove Fishron's weapons
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.Flairon)
                            toRemove = o;

            if (toRemove is not null)
                loot.Remove(toRemove);
        }

        private static void RemoveEmpressRules(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            IItemDropRule toRemove = null;

            // Step 1: Remove Empress Wings loot rule
            foreach (IItemDropRule rule in rules)
                if (rule is CommonDropNotScalingWithLuck c && c.itemId == ItemID.RainbowWings)
                    toRemove = c;

            if (toRemove is not null)
                loot.Remove(toRemove);

            // Step 2: Remove Empress of Light's weapons
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.PiercingStarlight)
                            toRemove = o;

            if (toRemove is not null)
                loot.Remove(toRemove);

            // Step 3: Remove Stellar Tune
            foreach (IItemDropRule rule in rules)
                if (rule is CommonDropNotScalingWithLuck c && c.itemId == ItemID.SparkleGuitar)
                    toRemove = c;

            if (toRemove is not null)
                loot.Remove(toRemove);
        }

        private static IItemDropRule FindMoonLordWeapons(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            foreach (IItemDropRule rule in rules)
                if (rule is OneFromOptionsNotScaledWithLuckDropRule o)
                    foreach (int itemID in o.dropIds)
                        if (itemID == ItemID.Terrarian)
                            return o;
            return null;
        }
        #endregion

        #region Goodie Bag Bat Hook
        private static void RemoveBatHookFromGoodieBag(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);
            SequentialRulesNotScalingWithLuckRule rule1 = null;
            foreach (IItemDropRule rule in rules)
                if (rule is SequentialRulesNotScalingWithLuckRule s)
                    rule1 = s;
            if (rule1 is null)
                return;
            foreach (IItemDropRule rule in rule1.rules)
                if (rule is CommonDropNotScalingWithLuck rule2 && rule2.itemId == ItemID.BatHook)
                {
                    rule2.chanceNumerator = 0;
                    rule2.chanceDenominator = 1;
                }
        }
        #endregion
    }
}
