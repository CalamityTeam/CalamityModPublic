using System;
using System.Collections.Generic;
using CalamityMod.Items.Accessories;
using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    #region Fraction Struct (thanks Yorai)
    public struct Fraction
    {
        internal readonly int numerator = 1;
        internal readonly int denominator = 1;

        public Fraction(int n, int d)
        {
            numerator = n < 0 ? 0 : n;
            denominator = d <= 0 ? 1 : d;
        }
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

        internal int ChooseQuantity() => Main.rand.Next(minQuantity, maxQuantity + 1);
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

        #region Block Drops
        private static int[] AllLoadedItemIDs = null;

        /// <summary>
        /// Adds the specified items to TML's blockLoot list. Items on the list cannot spawn in the world via any means.<br />
        /// <b>You should only use this function in the following places:</b><br />
        /// - GlobalItem.PreOpenVanillaBag (blocking items from treasure bags, fishing crates, etc.)<br />
        /// - GlobalNPC.PreKill or GlobalNPC.OnKill (blocking items from NPCs based on temporary conditions)<br /><br />
        /// If you want to <b>permanently remove</b> a drop from an NPC, this is not the function you want.<br />
        /// In those cases, use GlobalNPC.ModifyLoot, an if statement for that NPC, and npcLoot.Remove or npcLoot.RemoveWhere.<br />
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
            for (int i = 0; i < Main.npc.Length; ++i)
            {
                NPC n = Main.npc[i];
                if (n != null && n.active && idsToCheck.Contains(n.type))
                {
                    float dist = (n.Center - playerPos).Length();
                    if (dist < minDist)
                    {
                        minDist = dist;
                        r = i;
                    }
                }
            }
            return r;
        }

        /// <summary>
        /// Shorthand for shorthand: Registers an item to drop per-player on the specified condition.<br />
        /// Intended for lore items, but can be used generally for instanced drops.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item ID to drop.</param>
        /// <returns>A LeadingConditionRule which you can attach more PerPlayer or other rules to as you want.</returns>
        public static LeadingConditionRule AddConditionalPerPlayer(this NPCLoot npcLoot, Func<bool> lambda, int itemID)
        {
            LeadingConditionRule lcr = new(If(lambda));
            lcr.Add(PerPlayer(itemID));
            npcLoot.Add(lcr);
            return lcr;
        }

        /// <summary>
        /// Shorthand for shorthand: Registers an item to drop per-player on the specified condition.<br />
        /// Intended for lore items, but can be used generally for instanced drops.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item ID to drop.</param>
        /// <returns>A LeadingConditionRule which you can attach more PerPlayer or other rules to as you want.</returns>
        public static LeadingConditionRule AddConditionalPerPlayer(this NPCLoot npcLoot, Func<DropAttemptInfo, bool> lambda, int itemID)
        {
            LeadingConditionRule lcr = new(If(lambda));
            lcr.Add(PerPlayer(itemID));
            npcLoot.Add(lcr);
            return lcr;
        }

        public static DropBasedOnExpertMode NormalVsExpertQuantity(int itemID, int dropRateInt, int minNormal, int maxNormal, int minExpert, int maxExpert)
        {
            IItemDropRule normalRule = ItemDropRule.Common(itemID, dropRateInt, minNormal, maxNormal);
            IItemDropRule expertRule = ItemDropRule.Common(itemID, dropRateInt, minExpert, maxExpert);
            return new DropBasedOnExpertMode(normalRule, expertRule);
        }

        public static bool DropRevBagAccessories(IEntitySource source, Player p)
        {
            return DropItemFromSetCondition(source, p, CalamityWorld.revenge, 0.05f, ModContent.ItemType<StressPills>(), ModContent.ItemType<Laudanum>(), ModContent.ItemType<HeartofDarkness>());
        }
        #endregion

        //
        // FOLLOWING SECTION: 1.4 LOOT CODE (bestiary compatible)
        //

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
        #endregion

        #region Drop Rule Conditions
        public static IItemDropRuleCondition HallowedBarsCondition = If((info) =>
        {
            // If the Early Hardmode Progression Rework is not enabled, then Hallowed Bars can always drop from Mechanical Bosses.
            if (!CalamityConfig.Instance.EarlyHardmodeProgressionRework)
                return true;

            // If the Early Hardmode Progression Rework is enabled, then all 3 Mechanical Bosses must be defeated for Hallowed Bars to drop.
            return NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3;
        });

        public static IItemDropRuleCondition TarragonSetBonusHeartCondition = If((info) =>
        {
            // Tarragon hearts do not drop from the following:
            // 1 - NPCs spawned from statues
            // 2 - NPCs with no contact damage, unless they are bosses.
            // 3 - Very weak NPCs (for postML), i.e. those with less than 100 max health.
            NPC npc = info.npc;
            if (npc.SpawnedFromStatue || (npc.damage <= 5 && !npc.boss) || npc.lifeMax <= 100)
                return false;

            // If the drop info doesn't have a player, then find the closest player to the NPC and use that player instead.
            Player p = info.player;
            if (p is null || !p.active)
                p = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)];

            // With the player identified, return whether or not they have the full Tarragon Armor set equipped.
            return p.Calamity().tarraSet;
        });

        private static bool CanDropBloodOrbs(DropAttemptInfo info)
        {
            // Blood Orbs do not drop unless it's a Blood Moon.
            if (!Main.bloodMoon)
                return false;

            // If the drop info has a player, then check whether the player is "on the surface".
            bool onSurface = false;
            Player p = info.player;
            if (p != null && p.active)
                onSurface = p.ZoneOverworldHeight || p.ZoneSkyHeight;

            // Also check whether the NPC is considered "on the surface".
            NPC npc = info.npc;
            if (npc.Center.Y <= Main.worldSurface)
                onSurface = true;

            // Blood Orbs do not drop unless either the NPC killed or the player that killed the NPC are on the surface.
            if (!onSurface)
                return false;

            // Blood Orbs do not drop from the following:
            // 1 - NPCs spawned from statues
            // 2 - NPCs with no contact damage, unless they are bosses.
            // 3 - NPCs that are not targeting a player.
            return !npc.SpawnedFromStatue && (npc.damage > 5 || npc.boss) && npc.HasPlayerTarget;
        }

        public static IItemDropRuleCondition BloodOrbBaseCondition = If(CanDropBloodOrbs);
        public static IItemDropRuleCondition BloodOrbBloodflareCondition = If((info) =>
        {
            bool bloodOrbsAvailable = CanDropBloodOrbs(info);
            if (!bloodOrbsAvailable)
                return false;

            // To receive the extra orbs from Bloodflare Armor, you must be wearing Bloodflare Armor.
            Player p = info.player;
            return p != null && p.active && p.Calamity().bloodflareSet;
        });

        internal const float TrasherEatDistance = 96f;
        public static IItemDropRuleCondition AnglerFedToTrasherCondition = If((info) =>
        {
            bool trasherNearby = false;
            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC nearby = Main.npc[i];
                if (nearby is null || !nearby.active || nearby.type != ModContent.NPCType<Trasher>())
                    continue;
                if (info.npc.Distance(nearby.Center) < TrasherEatDistance)
                {
                    trasherNearby = true;
                    break;
                }
            }
            return trasherNearby;
        });
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
        #endregion

        #region NPCLoot Extensions
        /// <summary>
        /// Shorthand to add a simple drop to an NPC.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule Add(this NPCLoot npcLoot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add a simple drop to an NPC using a Fraction drop rate.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule Add(this NPCLoot npcLoot, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(new CommonDrop(itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to an NPC.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this NPCLoot npcLoot, Func<bool> lambda, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.ByCondition(If(lambda), itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to an NPC using a Fraction drop rate.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this NPCLoot npcLoot, Func<bool> lambda, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.ByCondition(If(lambda), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to an NPC.<br />
        /// <b>This version requires a lambda which uses DropAttemptInfo.</b>
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which takes a DropAttemptInfo struct and evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this NPCLoot npcLoot, Func<DropAttemptInfo, bool> lambda, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.ByCondition(If(lambda), itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add an arbitrary conditional drop to an NPC using a Fraction drop rate.<br />
        /// <b>This version requires a lambda which uses DropAttemptInfo.</b>
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which takes a DropAttemptInfo struct and evaluates in real-time to the condition that needs to be checked.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddIf(this NPCLoot npcLoot, Func<DropAttemptInfo, bool> lambda, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.ByCondition(If(lambda), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add a simple normal-only drop to an NPC.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRateInt">The chance that the item will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddNormalOnly(this NPCLoot npcLoot, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), itemID, dropRateInt, minQuantity, maxQuantity));
        }

        /// <summary>
        /// Shorthand to add a simple normal-only drop to an NPC.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The chance that the item will drop as a DropHelper Fraction.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 1.</param>
        /// <returns>The item drop rule registered.</returns>
        public static IItemDropRule AddNormalOnly(this NPCLoot npcLoot, int itemID, Fraction dropRate, int minQuantity = 1, int maxQuantity = 1)
        {
            return npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), itemID, dropRate.denominator, minQuantity, maxQuantity, dropRate.numerator));
        }

        /// <summary>
        /// Shorthand to add an arbitrary drop rule as a normal-only drop to an NPC.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="rule">The IItemDropRule to add.</param>
        public static void AddNormalOnly(this NPCLoot npcLoot, IItemDropRule rule)
        {
            LeadingConditionRule normalOnly = npcLoot.DefineNormalOnlyDropSet();
            normalOnly.Add(rule);
        }

        /// <summary>
        /// Registers a LeadingConditionRule for an NPC and returns it so you can add drops to that rule.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="condition">The condition behind which you want to gate several drop rules.</param>
        /// <returns>The LeadingConditionRule which encapsulates the given condition.</returns>
        public static LeadingConditionRule DefineConditionalDropSet(this NPCLoot npcLoot, IItemDropRuleCondition condition)
        {
            LeadingConditionRule rule = new LeadingConditionRule(condition);
            npcLoot.Add(rule);
            return rule;
        }

        /// <summary>
        /// Shorthand for registering a LeadingConditionRule using DropHelper.If.<br />
        /// This version does <b>NOT</b> use the DropAttemptInfo struct that is available.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <returns>The LeadingConditionRule which encapsulates the given lambda.</returns>
        public static LeadingConditionRule DefineConditionalDropSet(this NPCLoot npcLoot, Func<bool> lambda) => npcLoot.DefineConditionalDropSet(If(lambda));

        /// <summary>
        /// Shorthand for registering a LeadingConditionRule using DropHelper.If.<br />
        /// This version <b>DOES</b> use the DropAttemptInfo struct, and thus the provided lambda requires 1 argument.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="lambda">A lambda which evaluates in real-time to the condition that needs to be checked.</param>
        /// <returns>The LeadingConditionRule which encapsulates the given lambda.</returns>
        public static LeadingConditionRule DefineConditionalDropSet(this NPCLoot npcLoot, Func<DropAttemptInfo, bool> lambda) => npcLoot.DefineConditionalDropSet(If(lambda));

        /// <summary>
        /// Shorthand for shorthand: Registers a Normal Mode only LeadingConditionRule for an NPC and returns it to you.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <returns>A Normal Mode only LeadingConditionRule.</returns>
        public static LeadingConditionRule DefineNormalOnlyDropSet(this NPCLoot npcLoot) => npcLoot.DefineConditionalDropSet(new Conditions.NotExpert());

        /// <summary>
        /// This function does its best to replace all instances of the given item in the given NPC's drops with the specified chance.<br />
        /// It tries to affect as many types of drop rule as possible.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="newNumerator">The new numerator to use.</param>
        /// <param name="newDenominator">The new denominator to use.</param>
        /// <param name="includeGlobalDrops">Whether or not to include global loot rules. Defaults to false. Generally, you should leave this as false.</param>
        /// <returns>The number of changes made.</returns>
        public static int ChangeDropRate(this NPCLoot npcLoot, int itemID, int newNumerator, int newDenominator, bool includeGlobalDrops = false)
        {
            int numChanges = 0;
            var rules = npcLoot.Get(includeGlobalDrops);
            foreach (IItemDropRule rule in rules)
                rule.RecursivelyMutateDropRate(itemID, newNumerator, newDenominator);
            return numChanges;
        }

        /// <summary>
        /// This function does its best to replace all instances of the given item in the given NPC's drops with the specified chance.<br />
        /// It tries to affect as many types of drop rule as possible.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <param name="itemID">The item to drop.</param>
        /// <param name="dropRate">The new drop rate to use, as a DropHelper Fraction.</param>
        /// <param name="includeGlobalDrops">Whether or not to include global loot rules. Defaults to false. Generally, you should leave this as false.</param>
        /// <returns>The number of changes made.</returns>
        public static int ChangeDropRate(this NPCLoot npcLoot, int itemID, Fraction dropRate, bool includeGlobalDrops = false)
        {
            return npcLoot.ChangeDropRate(itemID, dropRate.numerator, dropRate.denominator, includeGlobalDrops);
        }
        #endregion

        #region "Calamity Style" Drop Rule
        /// <summary>
        /// Also known as the "Calamity Style" drop rule.<br />
        /// Every item in the list has the given chance to drop individually.<br />
        /// If no items drop, then one of them is forced to drop, chosen at random.
        /// </summary>
        public class AllOptionsAtOnceWithPityDropRule : OneFromOptionsDropRule
        {
            public AllOptionsAtOnceWithPityDropRule(int chanceNumerator, int chanceDenominator, params int[] itemIDs) : base(chanceDenominator, chanceNumerator, itemIDs)
            { }

            public new ItemDropAttemptResult TryDroppingItem(DropAttemptInfo info)
            {
                bool droppedAnything = false;

                // Roll for each drop individually.
                foreach (int itemID in dropIds)
                {
                    bool rngRoll = info.player.RollLuck(chanceDenominator) < chanceNumerator;
                    droppedAnything |= rngRoll;
                    if (rngRoll)
                        CommonCode.DropItemFromNPC(info.npc, itemID, 1);
                }

                // If everything fails to drop, force drop one item from the set.
                if (!droppedAnything)
                {
                    int itemToDrop = dropIds[info.rng.Next(dropIds.Length)];
                    CommonCode.DropItemFromNPC(info.npc, itemToDrop, 1);
                }

                // Calamity style drops cannot fail. You will always get at least one item.
                ItemDropAttemptResult result = default;
                result.State = ItemDropAttemptResultState.Success;
                return result;
            }

            public new void ReportDroprates(List<DropRateInfo> drops, DropRateInfoChainFeed ratesInfo)
            {
                int numDrops = dropIds.Length;
                float rawDropRate = chanceNumerator / (float)chanceDenominator;
                // Combinatorics:
                // OPTION 1: [The item drops = Raw Drop Rate]
                // +
                // OPTION 2: [ALL items fail to drop = (1-x)^n] * [This item is chosen as pity = 1/n]
                float dropRateWithPityRoll = rawDropRate + (float)(Math.Pow(1f - rawDropRate, numDrops) * (1f / numDrops));
                float dropRateAdjustedForParent = dropRateWithPityRoll * ratesInfo.parentDroprateChance;

                // Report the drop rate of each individual item. This calculation includes the fact that each individual item can be guaranteed as pity.
                foreach (int itemID in dropIds)
                    drops.Add(new DropRateInfo(itemID, 1, 1, dropRateAdjustedForParent, ratesInfo.conditions));

                Chains.ReportDroprates(ChainedRules, rawDropRate, drops, ratesInfo);
            }
        }

        public static IItemDropRule CalamityStyle(int numerator, int denominator, params int[] itemIDs)
        {
            return new AllOptionsAtOnceWithPityDropRule(numerator, denominator, itemIDs);
        }
        public static IItemDropRule CalamityStyle(Fraction dropRateForEachItem, params int[] itemIDs)
        {
            return CalamityStyle(dropRateForEachItem.numerator, dropRateForEachItem.denominator, itemIDs);
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
                    TryDropInternal(info.npc, itemId, stack);
                    result.State = ItemDropAttemptResultState.Success;
                    return result;
                }

                result.State = ItemDropAttemptResultState.FailedRandomRoll;
                return result;
            }

            // The contents of this method are more or less copied from CommonCode.DropItemLocalPerClientAndSetNPCMoneyTo0
            private void TryDropInternal(NPC npc, int itemId, int stack)
            {
                if (itemId <= 0 || itemId >= ItemLoader.ItemCount)
                    return;

                // If server-side, then the item must be spawned for each client individually.
                if (Main.netMode == NetmodeID.Server)
                {
                    int idx = Item.NewItem(npc.GetItemSource_Loot(), npc.Center, itemId, stack, true, -1);
                    Main.timeItemSlotCannotBeReusedFor[idx] = protectionTime;
                    for (int i = 0; i < Main.maxPlayers; ++i)
                        if (Main.player[i].active)
                            NetMessage.SendData(MessageID.InstancedItem, i, -1, null, idx);
                    Main.item[idx].active = false;
                }

                // Otherwise just drop the item.
                else
                    CommonCode.DropItemFromNPC(npc, itemId, stack);
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

        //
        // FOLLOWING SECTION: LEGACY 1.3 LOOT CODE (treasure bags, etc.)
        //

        #region Player Item Spawns
        /// <summary>
        /// Spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItem(IEntitySource source, Player p, int itemID, int minQuantity = 1, int maxQuantity = 0)
        {
            int quantity;

            // If they're equal (or for some reason max is less??) then just drop the minimum amount.
            if (maxQuantity <= minQuantity)
                quantity = minQuantity;

            // Otherwise pick a random amount to drop, inclusive.
            else
                quantity = Main.rand.Next(minQuantity, maxQuantity + 1);

            // If the final quantity is 0 or less, don't bother.
            if (quantity <= 0)
                return 0;

            p.QuickSpawnItem(source, itemID, quantity);
            return quantity;
        }

        /// <summary>
        /// At a chance, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="chance">The chance that the items will spawn. A decimal number <= 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemChance(IEntitySource source, Player p, int itemID, float chance, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return 0;

            return DropItem(source, p, itemID, minQuantity, maxQuantity);
        }

        /// <summary>
        /// At a chance, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemChance(IEntitySource source, Player p, int itemID, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.Next(oneInXChance) != 0)
                return 0;

            return DropItem(source, p, itemID, minQuantity, maxQuantity);
        }
        #endregion

        #region Player Item Spawns Conditional
        /// <summary>
        /// With a condition, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemCondition(IEntitySource source, Player p, int itemID, bool condition, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItem(source, p, itemID, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="chance">The chance that the items will spawn. A decimal number <= 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemCondition(IEntitySource source, Player p, int itemID, bool condition, float chance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(source, p, itemID, chance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemCondition(IEntitySource source, Player p, int itemID, bool condition, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(source, p, itemID, oneInXChance, minQuantity, maxQuantity) : 0;
        }
        #endregion

        #region Player Item Set Spawns
        /// <summary>
        /// Chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSet(IEntitySource source, Player p, params int[] itemIDs)
        {
            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return false;

            // Choose which item to drop.
            int itemID = Main.rand.Next(itemIDs);

            p.QuickSpawnItem(source, itemID);
            return true;
        }

        /// <summary>
        /// At a chance, chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="chance">The chance that the item will spawn. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSetChance(IEntitySource source, Player p, float chance, params int[] itemIDs)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return false;

            return DropItemFromSet(source, p, itemIDs);
        }
        #endregion

        #region Player Item Set Spawns Conditional
        /// <summary>
        /// With a condition, chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSetCondition(IEntitySource source, Player p, bool condition, params int[] itemIDs)
        {
            return condition ? DropItemFromSet(source, p, itemIDs) : false;
        }

        /// <summary>
        /// With a condition and at a chance, chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="chance">The chance that the items will spawn. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSetCondition(IEntitySource source, Player p, bool condition, float chance, params int[] itemIDs)
        {
            return condition ? DropItemFromSetChance(source, p, chance, itemIDs) : false;
        }
        #endregion

        #region Player Entire Set Spawns
        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.
        /// </summary>
        /// <param name="p">The player which should receive the items.</param>
        /// <param name="chance">The chance that an item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(IEntitySource source, Player p, float chance, params int[] itemIDs)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return numDrops;

            // Tally the total number of items dropped as the drop set is iterated through.
            for (int i = 0; i < itemIDs.Length; ++i)
                numDrops += DropItemChance(source, p, itemIDs[i], chance);

            // If nothing at all was dropped, drop one thing at random.
            numDrops += DropItemFromSetCondition(source, p, numDrops <= 0, itemIDs) ? 1 : 0;
            return numDrops;
        }

        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.
        /// </summary>
        /// <param name="p">The player which should receive the items.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(IEntitySource source, Player p, int oneInXChance, params int[] itemIDs)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return numDrops;

            // Tally the total number of items dropped as the drop set is iterated through.
            for (int i = 0; i < itemIDs.Length; ++i)
                numDrops += DropItemChance(source, p, itemIDs[i], oneInXChance);

            // If nothing at all was dropped, drop one thing at random.
            numDrops += DropItemFromSetCondition(source, p, numDrops <= 0, itemIDs) ? 1 : 0;
            return numDrops;
        }
        #endregion

        #region Weighted Item Sets/Stacks Supporting Code
        // int itemID --> WeightedItemStack
        public static WeightedItemStack WeightStack(this int itemID) => WeightStack(itemID, WeightedItemStack.DefaultWeight);
        public static WeightedItemStack WeightStack(this int itemID, float weight) => new WeightedItemStack(itemID, weight);
        public static WeightedItemStack WeightStack(this int itemID, int quantity) => WeightStack(itemID, WeightedItemStack.DefaultWeight, quantity);
        public static WeightedItemStack WeightStack(this int itemID, float weight, int quantity) => new WeightedItemStack(itemID, weight, quantity);
        public static WeightedItemStack WeightStack(this int itemID, int min, int max) => WeightStack(itemID, WeightedItemStack.DefaultWeight, min, max);
        public static WeightedItemStack WeightStack(this int itemID, float weight, int min, int max) => new WeightedItemStack(itemID, weight, min, max);

        // ModItem generic parameter --> WeightedItemStack
        public static WeightedItemStack WeightStack<T>() where T : ModItem => WeightStack<T>(WeightedItemStack.DefaultWeight);
        public static WeightedItemStack WeightStack<T>(float weight) where T : ModItem => WeightStack(ModContent.ItemType<T>(), weight);
        public static WeightedItemStack WeightStack<T>(int quantity) where T : ModItem => WeightStack<T>(WeightedItemStack.DefaultWeight, quantity);
        public static WeightedItemStack WeightStack<T>(float weight, int quantity) where T : ModItem => WeightStack(ModContent.ItemType<T>(), weight, quantity);
        public static WeightedItemStack WeightStack<T>(int min, int max) where T : ModItem => WeightStack<T>(WeightedItemStack.DefaultWeight, min, max);
        public static WeightedItemStack WeightStack<T>(float weight, int min, int max) where T : ModItem => WeightStack(ModContent.ItemType<T>(), weight, min, max);

        // Separated implementation used so weighted random code isn't duplicated in two places.
        public static WeightedItemStack RollWeightedRandom(WeightedItemStack[] stacks)
        {
            int i;
            float[] breakpoints = new float[stacks.Length];
            float totalWeight = 0f;

            // Assign breakpoints based on the cumulative sum of weights thus far.
            // Error check invalid weights by giving them an unbelievably small drop chance.
            for (i = 0; i < stacks.Length; ++i)
            {
                float w = stacks[i].weight;
                if (w <= 0f || float.IsNaN(w) || float.IsInfinity(w))
                    w = WeightedItemStack.MinisiculeWeight;
                breakpoints[i] = totalWeight += w;
            }

            // Iterate through the breakpoints until you find the first one that is surpassed. Drop that item.
            float needle = Main.rand.NextFloat(totalWeight);
            i = 0;
            while (needle > breakpoints[i])
                ++i;
            return stacks[i];
        }
        #endregion

        #region Player Weighted Set Spawns
        /// <summary>
        /// Chooses an item (or stack of items) from an array of drop definitions and spawns it for the given player.<br></br>
        /// Each item is given a certain weight to spawn.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemFromWeightedSet(IEntitySource source, Player p, params WeightedItemStack[] stacks)
        {
            // Can't choose anything from an empty array.
            if (stacks is null || stacks.Length == 0)
                return 0;

            WeightedItemStack stk = RollWeightedRandom(stacks);
            return DropItem(source, p, stk.itemID, stk.minQuantity, stk.maxQuantity);
        }

        /// <summary>
        /// Rolls for each item (or stack of items) in an array of drop definitions to drop at their defined chances.<br></br>
        /// Always drops at least one of the defined stacks.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireWeightedSet(IEntitySource source, Player p, params WeightedItemStack[] stacks)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (stacks is null || stacks.Length == 0)
                return numDrops;

            for (int i = 0; i < stacks.Length; ++i)
            {
                WeightedItemStack stk = stacks[i];
                numDrops += DropItemChance(source, p, stk.itemID, stk.weight, stk.minQuantity, stk.maxQuantity);
            }

            // If nothing at all was dropped, drop one thing at (weighted) random.
            if (numDrops <= 0)
                numDrops += DropItemFromWeightedSet(source, p, stacks);

            return numDrops;
        }
        #endregion
    }
}
