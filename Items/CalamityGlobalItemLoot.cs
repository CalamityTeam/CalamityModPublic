using System.Collections.Generic;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Materials;
using CalamityMod.Items.PermanentBoosters;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Ores;
using CalamityMod.Items.Potions;
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
            Fraction fifteenPercent = new Fraction(15, 100);

            switch (item.type)
            {
                #region Boss Treasure Bags
                case ItemID.KingSlimeBossBag:
                    loot.Add(new CommonDrop(ModContent.ItemType<CrownJewel>(), 10)); // 10% Crown Jewel
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.EyeOfCthulhuBossBag:
                    loot.Add(ModContent.ItemType<DeathstareRod>(), DropHelper.BagWeaponDropRateInt); // 33% Deathstare Rod
                    loot.Add(ModContent.ItemType<TeardropCleaver>(), 10); // 10% Teardrop Cleaver
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                // On Rev+, Eater of Worlds segments don't drop partial loot. As such, the bag needs to drop all materials.
                // This can theoretically be exploited by killing the boss on Expert, then turning on Rev to open the bags.
                // We don't care.
                case ItemID.EaterOfWorldsBossBag:
                    var eowRevLCR = loot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.revenge));
                    eowRevLCR.Add(ItemID.DemoniteOre, 1, 120, 240); // 100% 120-240 Demonite Ore
                    eowRevLCR.Add(ItemID.ShadowScale, 1, 60, 120); // 100% 60-120 Shadow Scale
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;


                // On Rev+, Brain of Cthulhu's Creepers don't drop partial loot. As such, the bag needs to drop all materials.
                // This can theoretically be exploited by killing the boss on Expert, then turning on Rev to open the bags.
                // We don't care.
                case ItemID.BrainOfCthulhuBossBag:
                    var bocRevLCR = loot.DefineConditionalDropSet(DropHelper.If(() => CalamityWorld.revenge));
                    bocRevLCR.Add(ItemID.CrimtaneOre, 1, 100, 180); // 100% 100-180 Crimtane Ore
                    bocRevLCR.Add(ItemID.TissueSample, 1, 60, 120); // 100% 60-120 Tissue Sample
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                        ItemID.FireWhip, // Firecracker
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.QueenSlimeBossBag:
                    loot.Add(ItemID.SoulofLight, 1, 15, 20); // 100% 15-20 Soul of Light
                    loot.Add(ItemID.PinkGel, 1, 15, 20); // 100% 15-20 Pink Gel
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.DestroyerBossBag:
                    loot.Remove(FindHallowedBars(loot));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.TwinsBossBag:
                    loot.Remove(FindHallowedBars(loot));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    loot.Add(ModContent.ItemType<Arbalest>(), 10); // 10% Arbalest
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
                    break;

                case ItemID.SkeletronPrimeBossBag:
                    loot.Remove(FindHallowedBars(loot));
                    loot.AddIf(DropHelper.HallowedBarsCondition, ItemID.HallowedBar, 1, 20, 35);
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.Add(ModContent.ItemType<EssenceofSunlight>(), 1, 10, 12); // 100% 10-12 Essence of Sunlight
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
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
                    loot.AddRevBagAccessories();
                    loot.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                    // The Celestial Onion only drops if the player hasn't used one, or if the world is not in Master Mode.
                    loot.AddIf((info) => !info.player.Calamity().extraAccessoryML && !Main.masterMode, ModContent.ItemType<CelestialOnion>());
                    break;
                #endregion

                #region Fishing Crates
                case ItemID.WoodenCrate:
                case ItemID.WoodenCrateHard:
                    // We do not need to pop Hardmode ores out of Pearlwood Crates. They do not drop higher tier ores.
                    break;

                case ItemID.IronCrateHard:
                    RemoveHardmodeOresFromStandardCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Mythril);
                    break;

                // Non-crafted underground Gold Chest loot @ 20%; Individually 5%
                case ItemID.GoldenCrate:
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(5, 1,
                    ItemID.FlareGun,
                    ItemID.Mace,
                    ItemID.BandofRegeneration,
                    ItemID.ShoeSpikes)); // Climbing Claws is in Wooden/Pearlwood (vanilla) in case you're curious
                    break;

                case ItemID.GoldenCrateHard:
                    RemoveHardmodeOresFromStandardCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Titanium);
                    loot.Add(new OneFromOptionsNotScaledWithLuckDropRule(5, 1,
                    ItemID.FlareGun,
                    ItemID.Mace,
                    ItemID.BandofRegeneration,
                    ItemID.ShoeSpikes));
                    break;

                case ItemID.CorruptFishingCrateHard:
                case ItemID.CrimsonFishingCrateHard:
                case ItemID.HallowedFishingCrateHard:
                case ItemID.DungeonFishingCrateHard:
                case ItemID.JungleFishingCrateHard:
                case ItemID.FloatingIslandFishingCrateHard:
                case ItemID.FrozenCrateHard:
                case ItemID.LavaCrateHard:
                case ItemID.OasisCrateHard:
                case ItemID.OceanCrateHard:
                    RemoveHardmodeOresFromBiomeCrates(loot);
                    loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
                    break;
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

        #region Fishing Crate Loot Rule Manipulation
        private static void RemoveHardmodeOresFromStandardCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);

            // This is the primary rule which contains every drop
            AlwaysAtleastOneSuccessDropRule mainRule = null;
            foreach (IItemDropRule rule in rules)
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    mainRule = a;
            if (mainRule is null)
                return;

            // Find ones that are supposed to be for the ore and not the other loot
            foreach (IItemDropRule rule in mainRule.rules)
            {
                // Hardmode ores/bars are both nested within *another* nested rule
                if (rule is SequentialRulesNotScalingWithLuckRule oreRule)
                {
                    // Confirm that this is for the ore/bar then pop the numerator for the big rule
                    foreach (IItemDropRule nestedRule in oreRule.rules)
                    {
                        if (nestedRule is SequentialRulesNotScalingWithLuckRule s)
                        {
                            oreRule.chanceNumerator = 0;
                            return;
                        }
                    }
                }
            }
        }

        private static void RemoveHardmodeOresFromBiomeCrates(ItemLoot loot)
        {
            List<IItemDropRule> rules = loot.Get(false);

            // This is the primary rule which contains every drop
            AlwaysAtleastOneSuccessDropRule mainRule = null;
            foreach (IItemDropRule rule in rules)
                if (rule is AlwaysAtleastOneSuccessDropRule a)
                    mainRule = a;
            if (mainRule is null)
                return;

            // Find ones that are supposed to be for the ore and not the other loot
            foreach (IItemDropRule rule in mainRule.rules)
            {
                // Hardmode ores/bars are both nested within *another* nested rule
                if (rule is SequentialRulesNotScalingWithLuckRule oreRule)
                {
                    // Confirm that this is for the ore/bar then pop the numerator for the big rule
                    foreach (IItemDropRule nestedRule in oreRule.rules)
                    {
                        if (nestedRule is OneFromRulesRule o)
                        {
                            oreRule.chanceNumerator = 0;
                            return;
                        }
                    }
                }
            }
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
