using System;
using System.Collections.Generic;
using CalamityMod.Enums;
using CalamityMod.Items.Accessories;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod
{
    #region Fraction Struct (thanks Yorai)
    public struct Fraction
    {
        internal readonly int numerator;
        internal readonly int denominator;

        public Fraction(int n, int d)
        {
            numerator = n < 0 ? 0 : n;
            denominator = d <= 0 ? 1 : d;
        }

        public static implicit operator float(Fraction f) => f.numerator / (float)f.denominator;
    }
    #endregion

    #region Weighted Item Stack Struct
    public struct WeightedItemStack
    {
        public const float DefaultWeight = 1f;
        public const float MinisiculeWeight = 1E-6f;

        internal int itemID;
        internal float weight;
        internal int minQuantity;
        internal int maxQuantity;

        internal WeightedItemStack(int id, float w)
        {
            itemID = id;
            weight = w;
            minQuantity = 1;
            maxQuantity = 1;
        }

        internal WeightedItemStack(int id, float w, int quantity)
        {
            itemID = id;
            weight = w;
            minQuantity = quantity;
            maxQuantity = quantity;
        }

        internal WeightedItemStack(int id, float w, int min, int max)
        {
            itemID = id;
            weight = w;
            minQuantity = min;
            maxQuantity = max;
        }

        internal int ChooseQuantity(UnifiedRandom rng) => rng.Next(minQuantity, maxQuantity + 1);

        // Allow for implicitly casting integer item IDs into weighted item stacks.
        // Stack size is assumed to be 1. Weight is assumed to be default.
        public static implicit operator WeightedItemStack(int id)
        {
            return new WeightedItemStack(id, DefaultWeight, 1);
        }
    }
    #endregion

    public static class DropHelper
    {
        #region Global Drop Chances
        /// <summary>
        /// Weapons in Normal Mode typically have a 1 in X chance of dropping, where X is this variable.
        /// </summary>
        public const int NormalWeaponDropRateInt = 4;

        /// <summary>
        /// Weapons in Normal Mode typically have this chance to drop, measured out of 1.0.
        /// </summary>
        public const float NormalWeaponDropRateFloat = 0.25f;

        /// <summary>
        /// Weapons in Normal Mode typically have this chance to drop (as a DropHelper Fraction).
        /// </summary>
        public static readonly Fraction NormalWeaponDropRateFraction = new(1, NormalWeaponDropRateInt);

        /// <summary>
        /// Weapons in Expert Mode typically have a 1 in X chance of dropping, where X is this variable.
        /// </summary>
        public const int BagWeaponDropRateInt = 3;

        /// <summary>
        /// Weapons in Expert Mode typically have this chance to drop, measured out of 1.0.
        /// </summary>
        public const float BagWeaponDropRateFloat = 0.3333333f;

        /// <summary>
        /// Weapons in Expert Mode typically have this chance to drop (as a DropHelper Fraction).
        /// </summary>
        public static readonly Fraction BagWeaponDropRateFraction = new(1, BagWeaponDropRateInt);
        #endregion

        #region Bestiary Text
        public static string FirstKillText = CalamityUtils.GetTextValue("Condition.Drops.FirstKill");
        public static string MechBossText = CalamityUtils.GetTextValue("Condition.Drops.MechBoss");
        public static string CataclysmKilledLast = CalamityUtils.GetTextValue("Condition.Drops.CataclysmKilledLast");
        public static string CatastropheKilledLast = CalamityUtils.GetTextValue("Condition.Drops.CatastropheKilledLast");
        public static string CynosureText = CalamityUtils.GetTextValue("Condition.Drops.Cynosure");

        public static string ProvidenceEnragedText = CalamityUtils.GetTextValue("Condition.Drops.ProvidenceEnraged");
        public static string ProvidenceChallengeText = CalamityUtils.GetTextValue("Condition.Drops.ProvidenceChallenge");

        #endregion

        #region Block Drops
        private static int[] AllLoadedItemIDs = null;

        /// <summary>
        /// Adds the specified items to TML's blockLoot list. Items on the list cannot spawn in the world via any means.<br />
        /// <b>You should only use this function in the following places:</b><br />
        /// - ModNPC.PreKill and GlobalNPC.PreKill<br />
        /// - ModNPC.OnKill and GlobalNPC.OnKill<br /><br />
        /// This function is intended to block items from dropping from NPCs based on <b>TEMPORARY CONDITIONS.</b><br />
        /// If you want to <b>permanently remove</b> a drop from an NPC, this is not the function you want.<br />
        /// In those cases, use GlobalNPC.ModifyLoot, an if statement for that NPC, and loot.Remove or loot.RemoveWhere.<br />
        /// This will ensure that the drops are removed from the bestiary as well.
        /// </summary>
        /// <param name="itemIDs">The item IDs to prevent from spawning.</param>
        public static void BlockDrops(params int[] itemIDs)
        {
            foreach (int itemID in itemIDs)
                NPCLoader.blockLoot.Add(itemID);
        }

        /// <summary>
        /// Blocks every possible item in the game from dropping. This is the extreme version of BlockDrops.<br />
        /// <b>Please read the usage notes on BlockDrops.</b><br />
        /// This function intentionally still allows hearts and mana stars to drop. If you also want to block those, block them separately.
        /// </summary>
        /// <param name="exceptions">The item IDs to still allow to drop.</param>
        public static void BlockEverything(params int[] exceptions)
        {
            // This solution is legitimately brain damaged but it works for now
            // At least it's cached...
            if (AllLoadedItemIDs is null)
            {
                AllLoadedItemIDs = new int[ItemLoader.ItemCount];
                for (int i = 0; i < ItemLoader.ItemCount; ++i)
                    AllLoadedItemIDs[i] = i;
            }

            // Apply exceptions
            int[] withSomeExceptions = new int[ItemLoader.ItemCount];
            AllLoadedItemIDs.CopyTo(withSomeExceptions, 0);
            withSomeExceptions[ItemID.Heart] = ItemID.RedPotion;
            withSomeExceptions[ItemID.Star] = ItemID.RedPotion;
            foreach (int itemID in exceptions)
                withSomeExceptions[itemID] = ItemID.RedPotion;

            BlockDrops(withSomeExceptions);
        }
        #endregion

        #region Specific Drop Helpers
        // Code copied from Player.QuickSpawnClonedItem, which was added by TML.
        /// <summary>
        /// Clones the given item and spawns it into the world at the given position. You can also customize stack count as necessary.<br></br>
        /// The default stack count of -1 makes it copy the stack count of the given item.
        /// </summary>
        /// <param name="item">The item to clone and spawn.</param>
        /// <param name="position">Where the item should be spawned.</param>
        /// <param name="stack">The stack count to use. Leave at -1 to use the stack of the <b>item</b> parameter.</param>
        /// <returns>The spawned clone of the item. <b>NEVER</b> equal to the input item.</returns>
        public static Item DropItemClone(IEntitySource src, Item item, Vector2 position, int stack = -1)
        {
            int index = Item.NewItem(src, position, item.type, stack, false, -1, false, false);
            Item theClone = Main.item[index] = item.Clone();
            theClone.whoAmI = index;
            theClone.position = position;
            if (stack != -1)
                theClone.stack = stack;

            // If in multiplayer, broadcast that this item was spawned.
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, index, 1f);

            return theClone;
        }

        /// <summary>
        /// Finds the worm segment nearest to an NPC's target by combing the NPC array for the closest NPC that is one of the specified types.<br></br>
        /// Return the specified NPC's index if no matching worm segment was found.
        /// </summary>
        /// <param name="wormHead">The NPC whose target is used for distance comparisons.</param>
        /// <param name="wormSegmentIDs">An array (or multiple parameters) of NPC IDs which are the worm segments to look for.</param>
        /// <returns>An index in the NPC array of the closest worm segment, or the specified NPC's index.</returns>
        public static int FindClosestWormSegment(NPC wormHead, params int[] wormSegmentIDs)
        {
            List<int> idsToCheck = new List<int>(wormSegmentIDs);
            Vector2 playerPos = Main.player[wormHead.target].Center;

            int r = wormHead.whoAmI;
            float minDist = 1E+06f;
            foreach (NPC n in Main.ActiveNPCs)
            {
                if (idsToCheck.Contains(n.type))
                {
                    float dist = (n.Center - playerPos).Length();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        r = n.whoAmI;
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// Shorthand for shorthand: Registers an item to drop per-player on the specified condition.<br />
        /// Intended for lore items, but can be used generally for instanced drops.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item ID to drop.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>A LeadingConditionRule which you can attach more PerPlayer or other rules to as you want.</returns>
        public static LeadingConditionRule AddConditionalPerPlayer(this ILoot loot, Func<bool> lambda, int itemID, bool ui = true, string desc = null)
        {
            LeadingConditionRule lcr = new(If(lambda, ui, desc));
            lcr.Add(PerPlayer(itemID));
            loot.Add(lcr);
            return lcr;
        }

        /// <summary>
        /// Shorthand for shorthand: Registers an item to drop per-player on the specified condition.<br />
        /// Intended for lore items, but can be used generally for instanced drops.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item ID to drop.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>A LeadingConditionRule which you can attach more PerPlayer or other rules to as you want.</returns>
        public static LeadingConditionRule AddConditionalPerPlayer(this ILoot loot, Func<DropAttemptInfo, bool> lambda, int itemID, bool ui = true, string desc = null)
        {
            LeadingConditionRule lcr = new(If(lambda, ui, desc));
            lcr.Add(PerPlayer(itemID));
            loot.Add(lcr);
            return lcr;
        }

        public static DropBasedOnExpertMode NormalVsExpertQuantity(int itemID, int dropRateInt, int minNormal, int maxNormal, int minExpert, int maxExpert)
        {
            IItemDropRule normalRule = ItemDropRule.Common(itemID, dropRateInt, minNormal, maxNormal);
            IItemDropRule expertRule = ItemDropRule.Common(itemID, dropRateInt, minExpert, maxExpert);
            return new DropBasedOnExpertMode(normalRule, expertRule);
        }

        /// <summary>
        /// Adds the Revengeance Mode bag accessories to the given loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        public static void AddRevBagAccessories(this ILoot loot)
        {
            var lcr = new LeadingConditionRule(If(() => CalamityWorld.revenge));
            lcr.Add(new OneFromOptionsDropRule(20, 1, ModContent.ItemType<HeartofDarkness>(), ModContent.ItemType<StressPills>()));
            loot.Add(lcr);
        }

        /// <summary>
        /// Adds all the common drops for Biome Crates.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="hardMode">Whether or not the crate is considered a hardmode crate.</param>
        public static void AddBiomeCrateLootRules(this ILoot loot, bool hardMode = true)
        {
            if (hardMode)
                loot.AddHardmodeOresToCrates(HardmodeCrateType.Biome);
            else
            {
                // Pre-Hardmode Ore/Bar loot pools
                // 20-35 Ores @ 14.29%; Individually 1.79%
                IItemDropRule[] phmOres = new IItemDropRule[8]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CopperOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.TinOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.IronOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.LeadOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.GoldOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.PlatinumOre, 1, 20, 35)
                };
                loot.Add(new OneFromRulesRule(7, phmOres));

                // 6-16 Bars @ 25%; Individually 4.17%
                IItemDropRule[] phmBars = new IItemDropRule[6]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.IronBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.LeadBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.GoldBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.PlatinumBar, 1, 6, 16)
                };
                loot.Add(new OneFromRulesRule(4, phmBars));
            }

            // 2-4 Buff Potions @ 25%; Individually 4.17%
            loot.Add(new OneFromRulesRule(4, new IItemDropRule[6]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.ObsidianSkinPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.SpelunkerPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.HunterPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.GravitationPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.MiningPotion, 1, 2, 4),
                ItemDropRule.NotScalingWithLuck(ItemID.HeartreachPotion, 1, 2, 4)
            }));

            // 5-17 (Regular) Recovery Potions @ 50%; 25% Healing 25% Mana
            loot.Add(new OneFromRulesRule(2, new IItemDropRule[2]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.HealingPotion, 1, 5, 17),
                ItemDropRule.NotScalingWithLuck(ItemID.ManaPotion, 1, 5, 17)
            }));

            // 2-6 Bait @ 50%; 25% Master 25% Journeyman
            loot.Add(new OneFromRulesRule(2, new IItemDropRule[2]
            {
                ItemDropRule.NotScalingWithLuck(ItemID.MasterBait, 1, 2, 6),
                ItemDropRule.NotScalingWithLuck(ItemID.JourneymanBait, 1, 2, 6)
            }));

            // 5-12 Gold Coin @ 25%
            loot.Add(ItemID.GoldCoin, 4, 5, 12);
        }

        /// <summary>
        /// Adds (or re-adds) Hardmode Ores to crates with respect to Hardmode Progression rework<br/>
        /// Drop rules replicate vanilla's rules.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="type">Type of crate: Mythril, Titanium, Biome</param>
        public static void AddHardmodeOresToCrates(this ILoot loot, HardmodeCrateType type)
        {
            var adamantiteLCR = loot.DefineConditionalDropSet(AdamantiteCondition);
            var mythrilLCR = new LeadingConditionRule(MythrilCondition);
            if (type == HardmodeCrateType.Biome)
            {
                // Pre-Hardmode loot pools
                // 20-35 Ores @ 7.14%; Individually 0.89%
                IItemDropRule[] phmOres = new IItemDropRule[8]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CopperOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.TinOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.IronOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.LeadOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.GoldOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.PlatinumOre, 1, 20, 35)
                };
                // 6-16 Bars @ 8.33%; Individually 1.39%
                IItemDropRule[] phmBars = new IItemDropRule[6]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.IronBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.LeadBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.GoldBar, 1, 6, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.PlatinumBar, 1, 6, 16)
                };

                // Tier 3 loot pools
                // 20-35 Ores @ 7.14%; Individually 1.19%
                IItemDropRule[] hmOresThree = new IItemDropRule[6]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.AdamantiteOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.TitaniumOre, 1, 20, 35)
                };
                // 5-16 Bars @ 16.67%; Individually 2.78%
                IItemDropRule[] hmBarsThree = new IItemDropRule[6]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.AdamantiteBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.TitaniumBar, 1, 5, 16)
                };
                var adamantiteDropsOres = new SequentialRulesNotScalingWithLuckRule(7, new OneFromRulesRule(2, hmOresThree), new OneFromRulesRule(1, phmOres));
                var adamantiteDropsBars = new SequentialRulesNotScalingWithLuckRule(4, new OneFromRulesRule(3, 2, hmBarsThree), new OneFromRulesRule(1, phmBars));

                // Tier 2 loot pools
                // 20-35 Ores @ 7.14%; Individually 1.79%
                IItemDropRule[] hmOresTwo = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 20, 35)
                };
                // 5-16 Bars @ 16.67%; Individually 4.17%
                IItemDropRule[] hmBarsTwo = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 5, 16)
                };
                var mythrilDropsOres = new SequentialRulesNotScalingWithLuckRule(7, new OneFromRulesRule(2, hmOresTwo), new OneFromRulesRule(1, phmOres));
                var mythrilDropsBars = new SequentialRulesNotScalingWithLuckRule(4, new OneFromRulesRule(3, 2, hmBarsTwo), new OneFromRulesRule(1, phmBars));

                // Tier 1 loot pools
                // 20-35 Ores @ 7.14%; Individually 3.57%
                IItemDropRule[] hmOresOne = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 20, 35),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 20, 35)
                };
                // 5-16 Bars @ 16.67%; Individually 8.33%
                IItemDropRule[] hmBarsOne = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 5, 16),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 5, 16)
                };
                var cobaltDropsOres = new SequentialRulesNotScalingWithLuckRule(7, new OneFromRulesRule(2, hmOresOne), new OneFromRulesRule(1, phmOres));
                var cobaltDropsBars = new SequentialRulesNotScalingWithLuckRule(4, new OneFromRulesRule(3, 2, hmBarsOne), new OneFromRulesRule(1, phmBars));

                adamantiteLCR.Add(adamantiteDropsOres);
                adamantiteLCR.Add(adamantiteDropsBars);
                adamantiteLCR.OnFailedConditions(mythrilLCR);
                mythrilLCR.Add(mythrilDropsOres);
                mythrilLCR.Add(mythrilDropsBars);
                mythrilLCR.OnFailedConditions(cobaltDropsOres);
                mythrilLCR.OnFailedConditions(cobaltDropsBars);
            }
            else if (type == HardmodeCrateType.Mythril)
            {
                // Pre-Hardmode loot pools
                // 12-21 Ores @ 8.33%; Individually 1.39%
                IItemDropRule[] phmOres = new IItemDropRule[6]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CopperOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.TinOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.IronOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.LeadOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenOre, 1, 12, 21),
                };
                // 4-7 Bars @ 6.94%; Individually 1.16%
                IItemDropRule[] phmBars = new IItemDropRule[6]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CopperBar, 1, 4, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.TinBar, 1, 4, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.IronBar, 1, 4, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.LeadBar, 1, 4, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverBar, 1, 4, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenBar, 1, 4, 7),
                };

                // Tier 2 loot pools
                // 12-21 Ores @ 8.33%; Individually 2.08%
                IItemDropRule[] hmOresTwo = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 12, 21)
                };
                // 3-7 Bars @ 13.89%; Individually 3.47%
                IItemDropRule[] hmBarsTwo = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 3, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 3, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 3, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 3, 7)
                };
                var mythrilDropsOres = new SequentialRulesNotScalingWithLuckRule(6, new OneFromRulesRule(2, hmOresTwo), new OneFromRulesRule(1, phmOres));
                var mythrilDropsBars = new SequentialRulesNotScalingWithLuckRule(4, new OneFromRulesRule(3, 2, hmBarsTwo), new OneFromRulesRule(1, phmBars));
                var mythrilDrops = new SequentialRulesNotScalingWithLuckRule(1, mythrilDropsOres, mythrilDropsBars);

                // Tier 1 loot pools
                // 12-21 Ores @ 8.33%; Individually 4.17%
                IItemDropRule[] hmOresOne = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 12, 21),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 12, 21)
                };
                // 3-7 Bars @ 13.89%; Individually 6.94%
                IItemDropRule[] hmBarsOne = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 3, 7),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 3, 7)
                };
                var cobaltDropsOres = new SequentialRulesNotScalingWithLuckRule(6, new OneFromRulesRule(2, hmOresOne), new OneFromRulesRule(1, phmOres));
                var cobaltDropsBars = new SequentialRulesNotScalingWithLuckRule(4, new OneFromRulesRule(3, 2, hmBarsOne), new OneFromRulesRule(1, phmBars));
                var cobaltDrops = new SequentialRulesNotScalingWithLuckRule(1, cobaltDropsOres, cobaltDropsBars);

                mythrilLCR.Add(mythrilDrops);
                mythrilLCR.OnFailedConditions(cobaltDrops);
                loot.Add(mythrilLCR); // No Adamantite LCR here so we need to add this in
            }
            else if (type == HardmodeCrateType.Titanium)
            {
                // Pre-Hardmode loot pools
                // 25-34 Ores @ 10%; Individually 2.5%
                IItemDropRule[] phmOres = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.GoldOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.PlatinumOre, 1, 25, 34)
                };
                // 8-11 Bars @ 8.89%; Individually 2.22%
                IItemDropRule[] phmBars = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.SilverBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.TungstenBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.GoldBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.PlatinumBar, 1, 8, 11)
                };

                // Tier 3 loot pools
                // 25-34 Ores @ 10%; Individually 2.5%
                IItemDropRule[] hmOresThree = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.AdamantiteOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.TitaniumOre, 1, 25, 34)
                };
                // 8-11 Bars @ 17.78%; Individually 4.44%
                IItemDropRule[] hmBarsThree = new IItemDropRule[4]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.AdamantiteBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.TitaniumBar, 1, 8, 11)
                };
                var adamantiteDropsOres = new SequentialRulesNotScalingWithLuckRule(5, new OneFromRulesRule(2, hmOresThree), new OneFromRulesRule(1, phmOres));
                var adamantiteDropsBars = new SequentialRulesNotScalingWithLuckRule(3, new OneFromRulesRule(3, 2, hmBarsThree), new OneFromRulesRule(1, phmBars));
                var adamantiteDrops = new SequentialRulesNotScalingWithLuckRule(1, adamantiteDropsOres, adamantiteDropsBars);

                // Tier 2 loot pools
                // 25-34 Ores @ 10%; Individually 5%
                IItemDropRule[] hmOresTwo = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumOre, 1, 25, 34)
                };
                // 8-11 Bars @ 17.78%; Individually 8.89%
                IItemDropRule[] hmBarsTwo = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.MythrilBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.OrichalcumBar, 1, 8, 11)
                };
                var mythrilDropsOres = new SequentialRulesNotScalingWithLuckRule(5, new OneFromRulesRule(2, hmOresTwo), new OneFromRulesRule(1, phmOres));
                var mythrilDropsBars = new SequentialRulesNotScalingWithLuckRule(3, new OneFromRulesRule(3, 2, hmBarsTwo), new OneFromRulesRule(1, phmBars));
                var mythrilDrops = new SequentialRulesNotScalingWithLuckRule(1, mythrilDropsOres, mythrilDropsBars);

                // Tier 1 loot pools
                // EXCEPTION: Titanium Crates do not drop Cobalt. This is added to make it not entirely useless to get early.
                // 25-34 Ores @ 10%; Individually 5%
                IItemDropRule[] hmOresOne = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltOre, 1, 25, 34),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumOre, 1, 25, 34)
                };
                // 8-11 Bars @ 17.78%; Individually 8.89%
                IItemDropRule[] hmBarsOne = new IItemDropRule[2]
                {
                    ItemDropRule.NotScalingWithLuck(ItemID.CobaltBar, 1, 8, 11),
                    ItemDropRule.NotScalingWithLuck(ItemID.PalladiumBar, 1, 8, 11)
                };
                var cobaltDropsOres = new SequentialRulesNotScalingWithLuckRule(5, new OneFromRulesRule(2, hmOresOne), new OneFromRulesRule(1, phmOres));
                var cobaltDropsBars = new SequentialRulesNotScalingWithLuckRule(3, new OneFromRulesRule(3, 2, hmBarsOne), new OneFromRulesRule(1, phmBars));
                var cobaltDrops = new SequentialRulesNotScalingWithLuckRule(1, cobaltDropsOres, cobaltDropsBars);

                adamantiteLCR.Add(adamantiteDrops);
                adamantiteLCR.OnFailedConditions(mythrilLCR);
                mythrilLCR.Add(mythrilDrops);
                mythrilLCR.OnFailedConditions(cobaltDrops);
            }
        }
        #endregion

        #region Recursive Drop Rate Mutator
        private static int RecursivelyMutateDropRate(this IItemDropRule rule, int itemID, int newNumerator, int newDenominator)
        {
            if (rule is CommonDrop drop && drop.itemId == itemID)
            {
                drop.chanceNumerator = newNumerator;
                drop.chanceDenominator = newDenominator;
                return 1;
            }
            else if (rule is ItemDropWithConditionRule conditionalDrop && conditionalDrop.itemId == itemID)
            {
                conditionalDrop.chanceNumerator = newNumerator;
                conditionalDrop.chanceDenominator = newDenominator;
                return 1;
            }
            else if (rule is DropBasedOnExpertMode expertDrop)
            {
                int normalChanges = RecursivelyMutateDropRate(expertDrop.ruleForNormalMode, itemID, newNumerator, newDenominator);
                int expertChanges = RecursivelyMutateDropRate(expertDrop.ruleForExpertMode, itemID, newNumerator, newDenominator);
                return normalChanges + expertChanges;
            }
            else if (rule is DropBasedOnMasterMode masterDrop)
            {
                int defaultChanges = RecursivelyMutateDropRate(masterDrop.ruleForDefault, itemID, newNumerator, newDenominator);
                int masterChanges = RecursivelyMutateDropRate(masterDrop.ruleForMasterMode, itemID, newNumerator, newDenominator);
                return defaultChanges + masterChanges;
            }
            return 0;
        }
        #endregion

        #region Lambda Drop Rule Condition
        // This class serves as a vanilla drop rule condition that is based on completely arbitrary code.
        // Create these using the function DropHelper.If as needed.
        internal class LambdaDropRuleCondition : IItemDropRuleCondition
        {
            private readonly Func<DropAttemptInfo, bool> conditionLambda;
            private readonly bool visibleInUI;
            private readonly string description;

            internal LambdaDropRuleCondition(Func<DropAttemptInfo, bool> lambda, bool ui = true, string desc = null)
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda(info);
            public bool CanShowItemDropInUI() => visibleInUI;
            public string GetConditionDescription() => description;
        }

        internal class LambdaDropRuleCondition2 : IItemDropRuleCondition
        {
            private readonly Func<DropAttemptInfo, bool> conditionLambda;
            private readonly Func<bool> visibleInUI;
            private readonly string description;

            internal LambdaDropRuleCondition2(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, string desc = null)
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda(info);
            public bool CanShowItemDropInUI() => visibleInUI();
            public string GetConditionDescription() => description;
        }

        internal class LambdaDropRuleCondition3 : IItemDropRuleCondition
        {
            private readonly Func<DropAttemptInfo, bool> conditionLambda;
            private readonly Func<bool> visibleInUI;
            private readonly Func<string> description;

            internal LambdaDropRuleCondition3(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, Func<string> desc)
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda(info);
            public bool CanShowItemDropInUI() => visibleInUI();
            public string GetConditionDescription() => description();
        }

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" does <b>NOT</b> use the DropAttemptInfo struct that is available.<br />
        /// This lets you write simpler lambdas that do not need the context, e.g. just checking if a boss is dead.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>() => {CodeHere}</code></param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<bool> lambda) => new LambdaDropRuleCondition((_) => lambda());

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" does <b>NOT</b> use the DropAttemptInfo struct that is available.<br />
        /// This lets you write simpler lambdas that do not need the context, e.g. just checking if a boss is dead.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>() => {CodeHere}</code></param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<bool> lambda, bool ui = true, string desc = null)
        {
            bool LambdaInfoWrapper(DropAttemptInfo _) => lambda();
            return new LambdaDropRuleCondition(LambdaInfoWrapper, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<bool> lambda, Func<bool> ui, string desc = null)
        {
            bool LambdaInfoWrapper(DropAttemptInfo _) => lambda();
            return new LambdaDropRuleCondition2(LambdaInfoWrapper, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<bool> lambda, Func<bool> ui, Func<string> desc)
        {
            bool LambdaInfoWrapper(DropAttemptInfo _) => lambda();
            return new LambdaDropRuleCondition3(LambdaInfoWrapper, ui, desc);
        }

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" <b>DOES</b> use the DropAttemptInfo struct, and thus the provided lambda requires 1 argument.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>(info) => {CodeHere}</code></param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda) => new LambdaDropRuleCondition(lambda);

        /// <summary>
        /// Creates a new LambdaDropRuleCondition which executes the code of your choosing to decide whether this item drop should occur.<br />
        /// This version of "If" <b>DOES</b> use the DropAttemptInfo struct, and thus the provided lambda requires 1 argument.
        /// </summary>
        /// <param name="lambda">Lambda function which evaluates to true or false, deciding whether the item should drop. <code>(info) => {CodeHere}</code></param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The LambdaDropRuleCondition produced.</returns>
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda, bool ui = true, string desc = null)
        {
            return new LambdaDropRuleCondition(lambda, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, string desc = null)
        {
            return new LambdaDropRuleCondition2(lambda, ui, desc);
        }
        public static IItemDropRuleCondition If(Func<DropAttemptInfo, bool> lambda, Func<bool> ui, Func<string> desc)
        {
            return new LambdaDropRuleCondition3(lambda, ui, desc);
        }
        #endregion

        #region Drop Rule Conditions
        public static IItemDropRuleCondition MythrilCondition = If((info) =>
        {
            // If the Early Hardmode Progression Rework is not enabled, then Mythril/Orichalcum can always drop.
            if (!CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                return true;

            // If the Early Hardmode Progression Rework is enabled, then any Mechanical Boss must be defeated for Mythril/Orichalcum to drop.
            return NPC.downedMechBossAny;
        });
        public static IItemDropRuleCondition AdamantiteCondition = If((info) =>
        {
            // If the Early Hardmode Progression Rework is not enabled, then Adamantite/Titanium can always drop.
            if (!CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                return true;

            // If the Early Hardmode Progression Rework is enabled, then 2 of the Mechanical Bosses must be defeated for Adamantite/Titanium to drop.
            return (NPC.downedMechBoss1 && NPC.downedMechBoss2) || (NPC.downedMechBoss2 && NPC.downedMechBoss3) || (NPC.downedMechBoss1 && NPC.downedMechBoss3);
        });

        public static IItemDropRuleCondition HallowedBarsCondition = If((info) =>
        {
            // If the Early Hardmode Progression Rework is not enabled, then Hallowed Bars can always drop from Mechanical Bosses.
            if (!CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                return true;

            // If the Early Hardmode Progression Rework is enabled, then all 3 Mechanical Bosses must be defeated for Hallowed Bars to drop.
            return NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
        });

        public static IItemDropRuleCondition GoldSetBonusGoldCondition = If((info) =>
        {
            NPC npc = info.npc;
            if (npc.IsAnEnemy(false))
                return false;

            // If the drop info doesn't have a player, then find the closest player to the NPC and use that player instead.
            Player p = info.player;
            if (p is null || !p.active)
                p = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];

            // With the player identified, return whether or not they have the full Gold Armor set equipped.
            return p.Calamity().goldArmorGoldDrops;
        });

        public static IItemDropRuleCondition GoldSetBonusBossCondition = If((info) =>
        {
            // Gold coins "from from bosses"
            NPC npc = info.npc;
            if (!npc.boss)
                return false;

            // If the drop info doesn't have a player, then find the closest player to the NPC and use that player instead.
            Player p = info.player;
            if (p is null || !p.active)
                p = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];

            // With the player identified, return whether or not they have the full Gold Armor set equipped.
            return p.Calamity().goldArmorGoldDrops;
        });

        public static IItemDropRuleCondition TarragonSetBonusHeartCondition = If((info) =>
        {
            NPC npc = info.npc;
            if (npc.IsAnEnemy(false))
                return false;

            // If the drop info doesn't have a player, then find the closest player to the NPC and use that player instead.
            Player p = info.player;
            if (p is null || !p.active)
                p = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];

            // With the player identified, return whether or not they have the full Tarragon Armor set equipped.
            return p.Calamity().tarraSet;
        });

        // Remix seed drop rules
        public static IItemDropRuleCondition Remix => Condition.RemixWorld.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied);
        public static IItemDropRuleCondition NotRemix => Condition.NotRemixWorld.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied);

        // Get Fixed Boi seed drop rule
        public static IItemDropRuleCondition GFB => Condition.ZenithWorld.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied);

        public static IItemDropRuleCondition RevNoMaster => CalamityConditions.InRevengeanceModeNotMasterMode.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied);
        public static IItemDropRuleCondition RevAndMaster => CalamityConditions.InRevengeanceModeOrMasterMode.ToDropCondition(ShowItemDropInUI.WhenConditionSatisfied);

        #region Boss Defeat Conditionals
        public static IItemDropRuleCondition PostKS(bool ui = true) => Condition.DownedKingSlime.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostDS(bool ui = true) => CalamityConditions.DownedDesertScourge.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEoC(bool ui = true) => Condition.DownedEyeOfCthulhu.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostCrab(bool ui = true) => CalamityConditions.DownedCrabulon.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEvil1(bool ui = true) => Condition.DownedEowOrBoc.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostHM(bool ui = true) => CalamityConditions.DownedHiveMind.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostPerfs(bool ui = true) => CalamityConditions.DownedPerforator.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEvil2(bool ui = true) => CalamityConditions.DownedHiveMindOrPerforator.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostQB(bool ui = true) => Condition.DownedQueenBee.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostDeer(bool ui = true) => Condition.DownedDeerclops.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSkele(bool ui = true) => Condition.DownedSkeletron.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSG(bool ui = true) => CalamityConditions.DownedSlimeGod.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition Hardmode(bool ui = true) => Condition.Hardmode.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostQS(bool ui = true) => Condition.DownedQueenSlime.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostCryo(bool ui = true) => CalamityConditions.DownedCryogen.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostAS(bool ui = true) => CalamityConditions.DownedAquaticScourge.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostBrim(bool ui = true) => CalamityConditions.DownedBrimstoneElemental.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostDest(bool ui = true) => Condition.DownedDestroyer.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostTwins(bool ui = true) => Condition.DownedTwins.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSP(bool ui = true) => Condition.DownedSkeletronPrime.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition Post1Mech(bool ui = true) => Condition.DownedMechBossAny.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition Post3Mechs(bool ui = true) => Condition.DownedMechBossAll.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostCal(bool ui = true) => CalamityConditions.DownedCalamitasClone.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostPlant(bool ui = true) => Condition.DownedPlantera.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostCalPlant(bool ui = true) => CalamityConditions.DownedCalamitasCloneOrPlantera.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostLevi(bool ui = true) => CalamityConditions.DownedLeviathan.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostAureus(bool ui = true) => CalamityConditions.DownedAstrumAureus.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostGolem(bool ui = true) => Condition.DownedGolem.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostPBG(bool ui = true) => CalamityConditions.DownedPlaguebringer.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostEoL(bool ui = true) => Condition.DownedEmpressOfLight.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostFish(bool ui = true) => Condition.DownedDukeFishron.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostRav(bool ui = true) => CalamityConditions.DownedRavager.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostLC(bool ui = true) => Condition.DownedCultist.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostAD(bool ui = true) => CalamityConditions.DownedAstrumDeus.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostML(bool ui = true) => Condition.DownedMoonLord.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostGuard(bool ui = true) => CalamityConditions.DownedGuardians.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostBirb(bool ui = true) => CalamityConditions.DownedBumblebird.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostProv(bool ui = true) => CalamityConditions.DownedProvidence.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSig(bool ui = true) => CalamityConditions.DownedSignus.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSW(bool ui = true) => CalamityConditions.DownedStormWeaver.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostCV(bool ui = true) => CalamityConditions.DownedCeaselessVoid.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostPolter(bool ui = true) => CalamityConditions.DownedPolterghast.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostOD(bool ui = true) => CalamityConditions.DownedOldDuke.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostDoG(bool ui = true) => CalamityConditions.DownedDevourerOfGods.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostYharon(bool ui = true) => CalamityConditions.DownedYharon.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostExos(bool ui = true) => CalamityConditions.DownedExoMechs.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostSCal(bool ui = true) => CalamityConditions.DownedSupremeCalamitas.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostAEW(bool ui = true) => CalamityConditions.DownedPrimordialWyrm.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostHorribleHog(bool ui = true) => CalamityConditions.DownedHorribleHog.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostClam(bool ui = true) => CalamityConditions.DownedClam.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostClamHM(bool ui = true) => CalamityConditions.DownedBuffedClam.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostGSS(bool ui = true) => CalamityConditions.DownedGreatSandShark.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostBetsy(bool ui = true) => CalamityConditions.DownedBetsy.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostT1AR(bool ui = true) => CalamityConditions.DownedAcidRainT1.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        public static IItemDropRuleCondition PostT2AR(bool ui = true) => CalamityConditions.DownedAcidRainT2.ToDropCondition(ui ? ShowItemDropInUI.Always : ShowItemDropInUI.Never);
        #endregion
        #endregion

        #region Leading Condition Rule Extensions
        /// <summary>
        /// Adds any given drop rule as a chained rule to the given LeadingConditionRule.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should have another drop rule registered as one of its chains.</param>
        /// <param name="chainedRule">The drop rule which should occur given this leading condition.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <returns>The LeadingConditionRule (first parameter).</returns>
        public static IItemDropRule Add(this LeadingConditionRule mainRule, IItemDropRule chainedRule, bool hideLootReport = false)
        {
            return mainRule.OnSuccess(chainedRule, hideLootReport);
        }

        /// <summary>
        /// Shorthand to add a simple drop to the given LeadingConditionRule.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <returns>The LeadingConditionRule (first parameter).</returns>
        public static IItemDropRule Add(this LeadingConditionRule mainRule, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false)
        {
            return mainRule.OnSuccess(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity), hideLootReport);
        }

        /// <summary>
        /// Shorthand to add a simple drop to the given LeadingConditionRule using a Fraction drop rate.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <returns>The LeadingConditionRule (first parameter).</returns>
        public static IItemDropRule Add(this LeadingConditionRule mainRule, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false)
        {
            return mainRule.OnSuccess(new CommonDrop(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator), hideLootReport);
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to the given LeadingConditionRule.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this LeadingConditionRule mainRule, Func<bool> lambda, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false, string desc = null)
        {
            return mainRule.OnSuccess(ItemDropRule.ByCondition(If(lambda, true, desc), itemID, dropRateInt, minQuantity, maxQuantity), hideLootReport);
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to the given LeadingConditionRule using a Fraction drop rate.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this LeadingConditionRule mainRule, Func<bool> lambda, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false, string desc = null)
        {
            return mainRule.OnSuccess(ItemDropRule.ByCondition(If(lambda, true, desc), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator), hideLootReport);
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to the given LeadingConditionRule.<br />
        /// <b>This version requires a lambda which uses DropAttemptInfo.</b>
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="lambda">A lambda which takes a DropAttemptInfo struct and evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this LeadingConditionRule mainRule, Func<DropAttemptInfo, bool> lambda, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false, string desc = null)
        {
            return mainRule.OnSuccess(ItemDropRule.ByCondition(If(lambda, true, desc), itemID, dropRateInt, minQuantity, maxQuantity), hideLootReport);
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to the given LeadingConditionRule using a Fraction drop rate.<br />
        /// <b>This version requires a lambda which uses DropAttemptInfo.</b>
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="lambda">A lambda which takes a DropAttemptInfo struct and evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this LeadingConditionRule mainRule, Func<DropAttemptInfo, bool> lambda, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false, string desc = null)
        {
            return mainRule.OnSuccess(ItemDropRule.ByCondition(If(lambda, true, desc), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator), hideLootReport);
        }

        /// <summary>
        /// Adds any given drop rule as a chained rule to the given LeadingConditionRule.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should have another drop rule registered as one of its chains.</param>
        /// <param name="chainedRule">The drop rule which should occur given this leading condition.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <returns>The LeadingConditionRule (first parameter).</returns>
        public static IItemDropRule AddFail(this LeadingConditionRule mainRule, IItemDropRule chainedRule, bool hideLootReport = false)
        {
            return mainRule.OnFailedConditions(chainedRule, hideLootReport);
        }

        /// <summary>
        /// Shorthand to add a simple drop to the given LeadingConditionRule.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <returns>The LeadingConditionRule (first parameter).</returns>
        public static IItemDropRule AddFail(this LeadingConditionRule mainRule, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false)
        {
            return mainRule.OnFailedConditions(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity), hideLootReport);
        }

        /// <summary>
        /// Shorthand to add a simple drop to the given LeadingConditionRule using a Fraction drop rate.
        /// </summary>
        /// <param name="mainRule">The LeadingConditionRule which should drop this item as one of its chains.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="hideLootReport">Set to true for this drop to not appear in the Bestiary.</param>
        /// <returns>The LeadingConditionRule (first parameter).</returns>
        public static IItemDropRule AddFail(this LeadingConditionRule mainRule, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false)
        {
            return mainRule.OnFailedConditions(new CommonDrop(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator), hideLootReport);
        }
        #endregion

        #region ILoot Extensions
        /// <summary>
        /// Shorthand to add a simple drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule Add(this ILoot loot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add a simple drop to a loot table using a Fraction drop rate.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule Add(this ILoot loot, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(new CommonDrop(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="cond">An IItemDropRuleCondition which encapsulates the condition which needs to be checked in real-time.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this ILoot loot, IItemDropRuleCondition cond, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.ByCondition(cond, itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to a loot table using a Fraction drop rate.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="cond">An IItemDropRuleCondition which encapsulates the condition which needs to be checked in real-time.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this ILoot loot, IItemDropRuleCondition cond, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.ByCondition(cond, itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this ILoot loot, Func<bool> lambda, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool ui = true, string desc = null)
        {
            return loot.Add(ItemDropRule.ByCondition(If(lambda, ui, desc), itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to a loot table using a Fraction drop rate.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this ILoot loot, Func<bool> lambda, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1, bool ui = true, string desc = null)
        {
            return loot.Add(ItemDropRule.ByCondition(If(lambda, ui, desc), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to a loot table.<br />
        /// <b>This version requires a lambda which uses DropAttemptInfo.</b>
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which takes a DropAttemptInfo struct and evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this ILoot loot, Func<DropAttemptInfo, bool> lambda, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool ui = true, string desc = null)
        {
            return loot.Add(ItemDropRule.ByCondition(If(lambda, ui, desc), itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to a loot table using a Fraction drop rate.<br />
        /// <b>This version requires a lambda which uses DropAttemptInfo.</b>
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which takes a DropAttemptInfo struct and evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <param name="ui">Whether drops registered with this condition appear in the Bestiary. Defaults to true.</param>
        /// <param name="desc">The description of this condition in the Bestiary. Defaults to null.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this ILoot loot, Func<DropAttemptInfo, bool> lambda, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1, bool ui = true, string desc = null)
        {
            return loot.Add(ItemDropRule.ByCondition(If(lambda, ui, desc), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add a simple normal-only drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddNormalOnly(this ILoot loot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add a simple normal-only drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddNormalOnly(this ILoot loot, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return loot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary drop rule as a normal-only drop to a loot table.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="rule">The IItemDropRule to add.</param>
        public static void AddNormalOnly(this ILoot loot, IItemDropRule rule)
        {
            LeadingConditionRule normalOnly = loot.DefineNormalOnlyDropSet();
            normalOnly.Add(rule);
        }

        /// <summary>
        /// Registers a LeadingConditionRule for a loot table and returns it so you can add drops to that rule.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="condition">The condition behind which you want to gate several drop rules.</param>
        /// <returns>The LeadingConditionRule which encapsulates the given condition.</returns>
        public static LeadingConditionRule DefineConditionalDropSet(this ILoot loot, IItemDropRuleCondition condition)
        {
            LeadingConditionRule rule = new LeadingConditionRule(condition);
            loot.Add(rule);
            return rule;
        }

        /// <summary>
        /// Shorthand for registering a LeadingConditionRule using DropHelper.If.<br />
        /// This version does <b>NOT</b> use the DropAttemptInfo struct that is available.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <returns>The LeadingConditionRule which encapsulates the given lambda.</returns>
        public static LeadingConditionRule DefineConditionalDropSet(this ILoot loot, Func<bool> lambda) => loot.DefineConditionalDropSet(If(lambda));

        /// <summary>
        /// Shorthand for registering a LeadingConditionRule using DropHelper.If.<br />
        /// This version <b>DOES</b> use the DropAttemptInfo struct, and thus the provided lambda requires 1 argument.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <returns>The LeadingConditionRule which encapsulates the given lambda.</returns>
        public static LeadingConditionRule DefineConditionalDropSet(this ILoot loot, Func<DropAttemptInfo, bool> lambda) => loot.DefineConditionalDropSet(If(lambda));

        /// <summary>
        /// Shorthand for shorthand: Registers a Normal Mode only LeadingConditionRule for a loot table and returns it to you.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <returns>A Normal Mode only LeadingConditionRule.</returns>
        public static LeadingConditionRule DefineNormalOnlyDropSet(this ILoot loot) => loot.DefineConditionalDropSet(new Conditions.NotExpert());

        /// <summary>
        /// This function does its best to replace all instances of the given item in the given loot table's entries with the specified chance.<br />
        /// It tries to affect as many types of drop rule as possible.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="newNumerator">The new numerator to use.</param>
        /// <param name="newDenominator">The new denominator to use.</param>
        /// <param name="includeGlobalDrops">Whether or not to include global loot rules. Defaults to false. Generally, you should leave this as false.</param>
        /// <returns>The number of changes made.</returns>
        public static int ChangeDropRate(this ILoot loot, int itemID, int newNumerator, int newDenominator, bool includeGlobalDrops = false)
        {
            int numChanges = 0;
            var rules = loot.Get(includeGlobalDrops);
            foreach (IItemDropRule rule in rules)
                rule.RecursivelyMutateDropRate(itemID, newNumerator, newDenominator);
            return numChanges;
        }

        /// <summary>
        /// This function does its best to replace all instances of the given item in the given loot table's entries with the specified chance.<br />
        /// It tries to affect as many types of drop rule as possible.
        /// </summary>
        /// <param name="loot">The ILoot interface for the loot table.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The new drop rate to use, as a DropHelper Fraction.</param>
        /// <param name="includeGlobalDrops">Whether or not to include global loot rules. Defaults to false. Generally, you should leave this as false.</param>
        /// <returns>The number of changes made.</returns>
        public static int ChangeDropRate(this ILoot loot, int itemID, Fraction dropRate, bool includeGlobalDrops = false)
        {
            return loot.ChangeDropRate(itemID, dropRate.numerator, dropRate.denominator, includeGlobalDrops);
        }
        #endregion

        #region "Calamity Style" Drop Rule
        /// <summary>
        /// Also known as the "Calamity Style" drop rule.<br />
        /// Every item in the list has the given chance to drop individually.<br />
        /// If no items drop, then one of them is forced to drop, chosen at random.
        /// </summary>
        public class AllOptionsAtOnceWithPityDropRule : IItemDropRule
        {
            public WeightedItemStack[] stacks;
            public Fraction dropRate;
            public bool usesLuck;
            public List<IItemDropRuleChainAttempt> ChainedRules { get; set; }

            public AllOptionsAtOnceWithPityDropRule(Fraction dropRate, bool luck, params WeightedItemStack[] stacks)
            {
                this.dropRate = dropRate;
                this.stacks = stacks;
                usesLuck = luck;
                ChainedRules = new List<IItemDropRuleChainAttempt>();
            }

            public AllOptionsAtOnceWithPityDropRule(Fraction dropRate, bool luck, params int[] itemIDs)
            {
                this.dropRate = dropRate;
                stacks = new WeightedItemStack[itemIDs.Length];
                for (int i = 0; i < stacks.Length; ++i)
                    stacks[i] = itemIDs[i]; // implicit conversion operator
                usesLuck = luck;
                ChainedRules = new List<IItemDropRuleChainAttempt>();
            }

            public bool CanDrop(DropAttemptInfo info) => true;

            public ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            {
                bool droppedAnything = false;

                // Roll for each drop individually.
                foreach (WeightedItemStack stack in stacks)
                {
                    bool rngRoll = usesLuck ? info.player.RollLuck(dropRate.denominator) < dropRate.numerator : info.rng.NextFloat() < dropRate;
                    droppedAnything |= rngRoll;
                    if (rngRoll)
                        CommonCode.DropItem(info, stack.itemID, stack.ChooseQuantity(info.rng));
                }

                // If everything fails to drop, force drop one item from the set.
                if (!droppedAnything)
                {
                    WeightedItemStack stack = info.rng.NextFromList(stacks);
                    CommonCode.DropItem(info, stack.itemID, stack.ChooseQuantity(info.rng));
                }

                // Calamity style drops cannot fail. You will always get at least one item.
                ItemDropAttemptResult result = default;
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            public void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
            {
                int numDrops = stacks.Length;
                float rawDropRate = dropRate;
                // Combinatorics:
                // OPTION 1: [The item drops = Raw Drop Rate]
                // +
                // OPTION 2: [ALL items fail to drop = (1-x)^n] * [This item is chosen as pity = 1/n]
                float dropRateWithPityRoll = rawDropRate + (float)(Math.Pow(1f - rawDropRate, numDrops) * (1f / numDrops));
                float dropRateAdjustedForParent = dropRateWithPityRoll * ratesInfo.parentDroprateChance;

                // Report the drop rate of each individual item. This calculation includes the fact that each individual item can be guaranteed as pity.
                foreach (WeightedItemStack stack in stacks)
                    drops.Add(new DropRateInfo(stack.itemID, stack.minQuantity, stack.maxQuantity, dropRateAdjustedForParent, ratesInfo.conditions));

                Chains.ReportDroprates(ChainedRules, rawDropRate, drops, ratesInfo);
            }
        }

        public static IItemDropRule CalamityStyle(Fraction dropRateForEachItem, params WeightedItemStack[] stacks) => CalamityStyle(dropRateForEachItem, true, stacks);
        public static IItemDropRule CalamityStyle(Fraction dropRateForEachItem, bool luck, params WeightedItemStack[] stacks)
        {
            return new AllOptionsAtOnceWithPityDropRule(dropRateForEachItem, luck, stacks);
        }
        public static IItemDropRule CalamityStyle(Fraction dropRateForEachItem, params int[] itemIDs) => CalamityStyle(dropRateForEachItem, true, itemIDs);
        public static IItemDropRule CalamityStyle(Fraction dropRateForEachItem, bool luck, params int[] itemIDs)
        {
            return new AllOptionsAtOnceWithPityDropRule(dropRateForEachItem, luck, itemIDs);
        }
        #endregion

        #region Per Player Drop Rule
        public class PerPlayerDropRule : CommonDrop
        {
            // Default instanced drops are protected for 15 minutes, because they are used for boss bags.
            // You can customize this duration as you see fit. Calamity defaults it to 5 minutes.
            private const int DefaultDropProtectionTime = 18000; // 5 minutes
            private int protectionTime;

            public PerPlayerDropRule(int itemID, int denominator, int minQuantity = 1, int maxQuantity = 1, int numerator = 1, int protectFrames = DefaultDropProtectionTime)
                : base(itemID, denominator, minQuantity, maxQuantity, numerator)
            {
                protectionTime = protectFrames;
            }

            public PerPlayerDropRule(int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
                : base(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator)
            {
                protectionTime = DefaultDropProtectionTime;
            }

            // Overriding CanDrop is unnecessary. This drop rule has no condition.
            // If you want to use a condition with PerPlayerDropRule, use DropHelper.If

            public override ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            {
                ItemDropAttemptResult result = default;
                if (info.rng.Next(chanceDenominator) < chanceNumerator)
                {
                    int stack = info.rng.Next(amountDroppedMinimum, amountDroppedMaximum + 1);
                    TryDropInternal(info, itemId, stack);
                    result.State = ItemDropAttemptResultState.Success;
                    return result;
                }

                result.State = ItemDropAttemptResultState.FailedRandomRoll;
                return result;
            }

            // The contents of this method are more or less copied from CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0
            private void TryDropInternal(DropAttemptInfo info, int itemId, int stack)
            {
                if (itemId <= 0 || itemId >= ItemLoader.ItemCount)
                    return;

                // If server-side, then the item must be spawned for each client individually.
                if (Main.dedServ)
                {
                    NPC npc = info.npc;
                    int idx = Item.NewItem(npc.GetSource_Loot(), npc.Center, itemId, stack, true, -1);
                    if (idx < Main.maxItems)
                    {
                        Main.timeItemSlotCannotBeReusedFor[idx] = protectionTime;
                        foreach (Player player in Main.ActivePlayers)
                            NetMessage.SendData(MessageID.InstancedItem, player.whoAmI, -1, null, idx);
                        Main.item[idx].active = false;
                    }
                }

                // Otherwise just drop the item.
                else
                    CommonCode.DropItem(info, itemId, stack);
            }
        }

        public static IItemDropRule PerPlayer(int itemID, int denominator = 1, int minQuantity = 1, int maxQuantity = 1, int numerator = 1)
        {
            return new PerPlayerDropRule(itemID, denominator, minQuantity, maxQuantity, numerator);
        }
        public static IItemDropRule PerPlayer(int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return PerPlayer(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator);
        }
        #endregion
    }
}
