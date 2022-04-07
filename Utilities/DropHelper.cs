using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static class DropHelper
    {
        #region Global Drop Chances

        /// <summary>
        /// Weapons in Normal Mode typically have a 1 in X chance of dropping, where X is this variable.
        /// </summary>
        public const int NormalWeaponDropRateInt = 4;

        /// <summary>
        /// Weapons in Normal Mode typically have this chance to drop (decimal number out of 1.0).
        /// </summary>
        public const float NormalWeaponDropRateFloat = 0.25f;

        /// <summary>
        /// Weapons in Expert Mode typically have a 1 in X chance of dropping, where X is this variable.
        /// </summary>
        public const int BagWeaponDropRateInt = 3;

        /// <summary>
        /// Weapons in Expert Mode typically have this chance to drop (decimal number out of 1.0).
        /// </summary>
        public const float BagWeaponDropRateFloat = 0.3333333f;
        #endregion

        #region Block Drops
        /// <summary>
        /// Adds the specified items to TML's blockLoot list. Items on the list cannot spawn in the world via any means.<br />
        /// This is used to prevent vanilla loot code from spawning certain items.<br />
        /// <b>You should only use this function inside NPCLoot or bag opening code.</b> TML will clear the list for you when the loot event is over.
        /// </summary>
        /// <param name="itemIDs">The item IDs to prevent from spawning.</param>
        public static void BlockDrops(params int[] itemIDs)
        {
            foreach (int itemID in itemIDs)
                NPCLoader.blockLoot.Add(itemID);
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
        /// Randomly peppers stacks of 1 of the specified item all across the given NPC's hitbox.<br></br>
        /// Makes it appear as though the NPC "explodes" into a cloud of many identical items. Best used with floating items such as Souls.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <param name="stackSize">The number of items to place in each separate stack.</param>
        /// <returns>The number of items dropped. Not always equal to quantity if stack size isn't 1.</returns>
        public static int DropItemSpray(NPC npc, int itemID, int minQuantity = 1, int maxQuantity = 0, int stackSize = 1)
        {
            int quantity;

            // If they're equal (or for some reason max is less??) then just drop the minimum amount.
            if (maxQuantity <= minQuantity)
                quantity = minQuantity;

            // Otherwise pick a random amount to drop, inclusive.
            else
                quantity = Main.rand.Next(minQuantity, maxQuantity + 1);

            int dropped = 0;
            Vector2 pos = Vector2.Zero;
            for (int i = 0; i < quantity; i += stackSize)
            {
                pos.X = Main.rand.NextFloat(npc.Hitbox.Left, npc.Hitbox.Right);
                pos.Y = Main.rand.NextFloat(npc.Hitbox.Top, npc.Hitbox.Bottom);
                Item.NewItem(npc.GetItemSource_Loot(), pos, itemID, stackSize);
                dropped += stackSize;
            }

            return dropped;
        }
        #endregion

        #region Lambda Drop Rule Condition

        // This class serves as a vanilla drop rule condition that is based on an arbitrary bool.
        // Create these using the function DropHelper.If as needed.
        internal class LambdaDropRuleCondition : IItemDropRuleCondition
        {
            private readonly Func<bool> conditionLambda;
            private readonly bool visibleInUI;
            private readonly string description;

            internal LambdaDropRuleCondition(Func<bool> lambda, bool ui = true, string desc = "")
            {
                conditionLambda = lambda;
                visibleInUI = ui;
                description = desc;
            }

            public bool CanDrop(DropAttemptInfo info) => conditionLambda();
            public bool CanShowItemDropInUI() => visibleInUI;
            public string GetConditionDescription() => description;
        }

        public static IItemDropRuleCondition If(Func<bool> lambda) => new LambdaDropRuleCondition(lambda);
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
        /// <returns></returns>
        public static IItemDropRule Add(this LeadingConditionRule mainRule, int itemID, int dropRateInt = 1, int minQuantity = 1, int maxQuantity = 1, bool hideLootReport = false)
        {
            return mainRule.OnSuccess(ItemDropRule.Common(itemID, dropRateInt, minQuantity, maxQuantity), hideLootReport);
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
        /// Shorthand for shorthand: Registers a Normal Mode only LeadingConditionRule for an NPC and returns it to you.
        /// </summary>
        /// <param name="npcLoot">The NPC's NPCLoot object.</param>
        /// <returns>A Normal Mode only LeadingConditionRule.</returns>
        public static LeadingConditionRule DefineNormalOnlyDropSet(this NPCLoot npcLoot) => npcLoot.DefineConditionalDropSet(new Conditions.NotExpert());
        #endregion

        #region Player Item Spawns
        /// <summary>
        /// Spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItem(Player p, int itemID, int minQuantity = 1, int maxQuantity = 0)
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

            p.QuickSpawnItem(itemID, quantity);
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
        public static int DropItemChance(Player p, int itemID, float chance, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return 0;

            return DropItem(p, itemID, minQuantity, maxQuantity);
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
        public static int DropItemChance(Player p, int itemID, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.Next(oneInXChance) != 0)
                return 0;

            return DropItem(p, itemID, minQuantity, maxQuantity);
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
        public static int DropItemCondition(Player p, int itemID, bool condition, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItem(p, itemID, minQuantity, maxQuantity) : 0;
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
        public static int DropItemCondition(Player p, int itemID, bool condition, float chance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(p, itemID, chance, minQuantity, maxQuantity) : 0;
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
        public static int DropItemCondition(Player p, int itemID, bool condition, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(p, itemID, oneInXChance, minQuantity, maxQuantity) : 0;
        }
        #endregion

        #region Player Item Set Spawns
        /// <summary>
        /// Chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSet(Player p, params int[] itemIDs)
        {
            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return false;

            // Choose which item to drop.
            int itemID = Main.rand.Next(itemIDs);

            p.QuickSpawnItem(itemID);
            return true;
        }

        /// <summary>
        /// At a chance, chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="chance">The chance that the item will spawn. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSetChance(Player p, float chance, params int[] itemIDs)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return false;

            return DropItemFromSet(p, itemIDs);
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
        public static bool DropItemFromSetCondition(Player p, bool condition, params int[] itemIDs)
        {
            return condition ? DropItemFromSet(p, itemIDs) : false;
        }

        /// <summary>
        /// With a condition and at a chance, chooses an item from an array and spawns it for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="chance">The chance that the items will spawn. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be spawned.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemFromSetCondition(Player p, bool condition, float chance, params int[] itemIDs)
        {
            return condition ? DropItemFromSetChance(p, chance, itemIDs) : false;
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
        public static int DropEntireSet(Player p, float chance, params int[] itemIDs)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return numDrops;

            // Tally the total number of items dropped as the drop set is iterated through.
            for (int i = 0; i < itemIDs.Length; ++i)
                numDrops += DropItemChance(p, itemIDs[i], chance);

            // If nothing at all was dropped, drop one thing at random.
            numDrops += DropItemFromSetCondition(p, numDrops <= 0, itemIDs) ? 1 : 0;
            return numDrops;
        }

        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.
        /// </summary>
        /// <param name="p">The player which should receive the items.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(Player p, int oneInXChance, params int[] itemIDs)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return numDrops;

            // Tally the total number of items dropped as the drop set is iterated through.
            for (int i = 0; i < itemIDs.Length; ++i)
                numDrops += DropItemChance(p, itemIDs[i], oneInXChance);

            // If nothing at all was dropped, drop one thing at random.
            numDrops += DropItemFromSetCondition(p, numDrops <= 0, itemIDs) ? 1 : 0;
            return numDrops;
        }
        #endregion
    }
}
