using System.Collections.Generic;
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
            static bool CryonicAvailable()
            {
                if (!DownedBossSystem.downedCryogen)
                    return false;
                return (NPC.downedMechBoss1.ToInt() + NPC.downedMechBoss2.ToInt() + NPC.downedMechBoss3.ToInt()) >= 2;
            }
            
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

                    // The Celestial Onion only drops if the player hasn't used one.
                    loot.AddIf((info) => !info.player.Calamity().extraAccessoryML, ModContent.ItemType<CelestialOnion>());
                    break;
                #endregion

                #region Fishing Crates
                // TODO -- What is all this shit?
                case ItemID.WoodenCrate:
                case ItemID.WoodenCrateHard:
                    loot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 4, 3, 5); // 25% 3-5 Wulfrum Scrap
                    break;

                case ItemID.IronCrate:
                case ItemID.IronCrateHard:
                    loot.Add(ModContent.ItemType<WulfrumMetalScrap>(), 4, 5, 8); // 25% 5-8 Wulfrum Scrap
                    loot.Add(ModContent.ItemType<AncientBoneDust>(), 4, 5, 8); // 25% 5-8 Ancient Bone Dust
                    break;

                // these drops are not available in hardmode because this crate will stop dropping
                case ItemID.GoldenCrate:
                    loot.Add(ItemID.FlareGun, 10); // 10% Flare Gun
                    loot.Add(ItemID.ShoeSpikes, 10); // 10% Shoe Spikes (BUT NOT CLIMBING CLAWS?)
                    loot.Add(ItemID.BandofRegeneration, 10); // 10% Band of Regeneration
                    break;

                case ItemID.GoldenCrateHard:
                    // Post-Yharon: 15% 30-40 Auric Ore
                    loot.AddIf(() => DownedBossSystem.downedYharon, ModContent.ItemType<AuricOre>(), fifteenPercent, 30, 40);
                    break;

                case ItemID.CorruptFishingCrate:
                case ItemID.CrimsonFishingCrate:
                case ItemID.CorruptFishingCrateHard:
                case ItemID.CrimsonFishingCrateHard:
                    loot.Add(ModContent.ItemType<BlightedGel>(), fifteenPercent, 5, 8); // 15% 5-8 Blighted Gel
                    break;

                case ItemID.HallowedFishingCrate: // WHY
                case ItemID.HallowedFishingCrateHard:
                    var postProv = loot.DefineConditionalDropSet(() => DownedBossSystem.downedProvidence);
                    postProv.Add(ModContent.ItemType<UnholyEssence>(), fifteenPercent, 5, 10); // 15% 5-10 Unholy Essence
                    break;

                case ItemID.DungeonFishingCrate:
                case ItemID.DungeonFishingCrateHard:
                    loot.AddIf(() => NPC.downedPlantBoss, ItemID.Ectoplasm, 10, 1, 5); // 10% 1-5 Ectoplasm
                    loot.AddIf(() => DownedBossSystem.downedPolterghast, ModContent.ItemType<Polterplasm>(), 10, 1, 5); // 10% 1-5 Polterplasm
                    break;

                case ItemID.JungleFishingCrate:
                case ItemID.JungleFishingCrateHard:
                    loot.Add(ModContent.ItemType<MurkyPaste>(), new Fraction(1, 5), 1, 3); // 20% 1-3 Murky Paste
                    var postPlant = loot.DefineConditionalDropSet(() => NPC.downedPlantBoss);
                    postPlant.Add(ModContent.ItemType<PerennialOre>(), 5, 16, 28); // 20% 16-28 Perennial Ore
                    postPlant.Add(ModContent.ItemType<PerennialBar>(), fifteenPercent, 4, 7); // 15% 4-7 Perennial Bar
                    loot.AddIf(() => NPC.downedGolemBoss, ModContent.ItemType<PlagueCellCanister>(), 5, 3, 6); // 20% 3-6 Plague Cell Canister
                    var uelibloom = loot.DefineConditionalDropSet(() => DownedBossSystem.downedProvidence);
                    uelibloom.Add(ModContent.ItemType<UelibloomOre>(), 5, 16, 28); // 20% 16-28 Uelibloom Ore
                    uelibloom.Add(ModContent.ItemType<UelibloomBar>(), fifteenPercent, 4, 7); // 15% 4-7 Uelibloom Bar
                    break;

                case ItemID.FloatingIslandFishingCrate:
                case ItemID.FloatingIslandFishingCrateHard:
                    var evilBossTwo = loot.DefineConditionalDropSet(() => DownedBossSystem.downedHiveMind || DownedBossSystem.downedPerforator);
                    evilBossTwo.Add(ModContent.ItemType<AerialiteOre>(), 5, 16, 28); // 20% 16-28 Aerialite Ore
                    evilBossTwo.Add(ModContent.ItemType<AerialiteBar>(), fifteenPercent, 4, 7); // 15% 4-7 Aerialite Bar
                    loot.AddIf(() => Main.hardMode, ModContent.ItemType<EssenceofSunlight>(), 5, 2, 4); // 20% 2-4 Essence of Sunlight
                    loot.AddIf(() => NPC.downedMoonlord, ModContent.ItemType<ExodiumCluster>(), 5, 16, 28); // 20% 16-28 Exodium Clusters
                    break;

                case ItemID.FrozenCrate:
                case ItemID.FrozenCrateHard:
                    var cryonic = loot.DefineConditionalDropSet(CryonicAvailable);
                    cryonic.Add(ModContent.ItemType<CryonicOre>(), 5, 16, 28); // 20% 16-28 Cryonic Ore
                    cryonic.Add(ModContent.ItemType<CryonicBar>(), fifteenPercent, 4, 7); // 15% 4-7 Cryonic Bar
                    loot.AddIf(() => Main.hardMode, ModContent.ItemType<EssenceofEleum>(), 5, 2, 4); // 20% 2-4 Essence of Eleum
                    break;

                case ItemID.LavaCrate:
                case ItemID.LavaCrateHard:
                    loot.AddIf(() => Main.hardMode, ModContent.ItemType<EssenceofHavoc>(), 5, 2, 4); // 20% 2-4 Essence of Havoc
                    break;

                // Calamity does not touch Oasis Crates yet
                case ItemID.OasisCrate:
                case ItemID.OasisCrateHard:
                    break;

                // Calamity does not touch Ocean Crates yet
                case ItemID.OceanCrate:
                case ItemID.OceanCrateHard:
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
