using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using CalamityMod;
using CalamityMod.Items;
using CalamityMod.World;

namespace CalamityMod
{
    public class DropHelper
    {
        /// <summary>
        /// The number of extra loot bags bosses drop when Revengeance Mode is active.
        /// </summary>
        public static int RevExtraBags = 0;

        /// <summary>
        /// The number of extra loot bags bosses drop when Death Mode is active.
        /// </summary>
        public static int DeathExtraBags = 0;

        /// <summary>
        /// The number of extra loot bags bosses drop when the Defiled Rune is active.
        /// </summary>
        public static int DefiledExtraBags = 0;

        /// <summary>
        /// The number of extra loot bags bosses drop when Armageddon is active.
        /// </summary>
        public static int ArmageddonExtraBags = 5;

        /// <summary>
        /// This is the value that Defiled Rune boosts certain drop chances to (a decimal number <= 1.0).
        /// </summary>
        public static float DefiledChanceBoost = 0.05f;

        #region Boss Bag Drop Helper
        /// <summary>
        /// Automatically drops the correct number of boss bags for each difficulty based on constants kept in DropHelper.
        /// </summary>
        /// <param name="theBoss">The NPC to drop boss bags for.</param>
        /// <returns>The number of boss bags dropped.</returns>
        public static int DropBags(NPC theBoss)
        {
            int bagsDropped = 0;

            // Don't drop any bags for an invalid NPC.
            if (theBoss is null)
                return bagsDropped;

            // Armageddon bags drop even on Normal.
            if (CalamityWorld.armageddon)
            {
                for (int i = 0; i < ArmageddonExtraBags; ++i)
                    theBoss.DropBossBags();

                bagsDropped += ArmageddonExtraBags;
            }

            // If the difficulty isn't Expert+, no more bags are dropped.
            if (!Main.expertMode)
                return bagsDropped;

            // Drop the 1 vanilla Expert Mode boss bag.
            theBoss.DropBossBags();
            bagsDropped++;

            // If Rev is active, possibly drop extra bags.
            if (CalamityWorld.revenge)
            {
                for (int i = 0; i < RevExtraBags; ++i)
                    theBoss.DropBossBags();

                bagsDropped += RevExtraBags;
            }

            // If Death is active, possibly drop extra bags.
            if(CalamityWorld.death)
            {
                for (int i = 0; i < DeathExtraBags; ++i)
                    theBoss.DropBossBags();

                bagsDropped += DeathExtraBags;
            }

            // If Defiled is active, possibly drop extra bags.
            if(CalamityWorld.defiled)
            {
                for (int i = 0; i < DefiledExtraBags; ++i)
                    theBoss.DropBossBags();

                bagsDropped += DefiledExtraBags;
            }

            return bagsDropped;
        }
#endregion

        #region NPC Item Drops 100% Chance
        /// <summary>
        /// Drops a stack of one or more items from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItem(NPC npc, int itemID, bool dropPerPlayer = false, int minQuantity = 1, int maxQuantity = 0)
        {
            int quantity;

            // If they're equal (or for some reason max is less??) then just drop the minimum amount.
            if (maxQuantity <= minQuantity)
                quantity = minQuantity;

            // Otherwise pick a random amount to drop, inclusive.
            else
                quantity = Main.rand.Next(minQuantity, maxQuantity + 1);

            // If the drop is supposed to be instanced, drop it as such.
            if (dropPerPlayer)
            {
                npc.DropItemInstanced(npc.position, npc.Size, itemID, quantity);
            }
            else
            {
                Item.NewItem(npc.Hitbox, itemID, quantity);
            }

            return quantity;
        }

        /// <summary>
        /// Drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItem(NPC npc, int itemID, int minQuantity = 1, int maxQuantity = 0)
        {
            return DropItem(npc, itemID, false, minQuantity, maxQuantity);
        }
        #endregion

        #region NPC Item Drops Float Chance
        /// <summary>
        /// At a chance, drops a stack of one or more items from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0. Defaults to 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, bool dropPerPlayer = false, float chance = 1f, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return 0;

            return DropItem(npc, itemID, dropPerPlayer, minQuantity, maxQuantity);
        }

        /// <summary>
        /// At a chance, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0. Defaults to 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, float chance = 1f, int minQuantity = 1, int maxQuantity = 0)
        {
            return DropItemChance(npc, itemID, false, chance, minQuantity, maxQuantity);
        }
        #endregion

        #region NPC Item Drops Int Chance
        /// <summary>
        /// At a chance, drops a stack of one or more items from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance. Defaults to 1.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, bool dropPerPlayer = false, int oneInXChance = 1, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.Next(oneInXChance) != 0)
                return 0;

            return DropItem(npc, itemID, dropPerPlayer, minQuantity, maxQuantity);
        }

        /// <summary>
        /// At a chance, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance. Defaults to 1.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, int oneInXChance = 1, int minQuantity = 1, int maxQuantity = 0)
        {
            return DropItemChance(npc, itemID, false, oneInXChance, minQuantity, maxQuantity);
        }
        #endregion

        #region NPC Item Drops Conditional
        /// <summary>
        /// With a condition, drops a stack of one or more items from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool dropPerPlayer = false, bool condition = true, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItem(npc, itemID, dropPerPlayer, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool condition = true, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItem(npc, itemID, false, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, drops a stack of one or more items from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0. Defaults to 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool dropPerPlayer = false, bool condition = true, float chance = 1f, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, dropPerPlayer, chance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0. Defaults to 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool condition = true, float chance = 1f, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, false, chance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, drops a stack of one or more items from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance. Defaults to 1.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool dropPerPlayer = false, bool condition = true, int oneInXChance = 1, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, dropPerPlayer, oneInXChance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance. Defaults to 1.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool condition = true, int oneInXChance = 1, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, false, oneInXChance, minQuantity, maxQuantity) : 0;
        }
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

            p.QuickSpawnItem(itemID, quantity);
            return quantity;
        }

        /// <summary>
        /// Spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="chance">The chance that the items will spawn. A decimal number <= 1.0. Defaults to 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemChance(Player p, int itemID, float chance = 1f, int minQuantity = 1, int maxQuantity = 0)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return 0;

            return DropItem(p, itemID, minQuantity, maxQuantity);
        }

        /// <summary>
        /// Spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance. Defaults to 1.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemChance(Player p, int itemID, int oneInXChance = 1, int minQuantity = 1, int maxQuantity = 0)
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
        public static int DropItemCondition(Player p, int itemID, bool condition = true, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItem(p, itemID, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="chance">The chance that the items will spawn. A decimal number <= 1.0. Defaults to 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemCondition(Player p, int itemID, bool condition = true, float chance = 1f, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(p, itemID, chance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, spawns a stack of one or more items for the given player.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to spawn.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this spawn. If false, nothing is spawned.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance. Defaults to 1.</param>
        /// <param name="minQuantity">The minimum number of items to spawn. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to spawn. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items spawned.</returns>
        public static int DropItemCondition(Player p, int itemID, bool condition = true, int oneInXChance = 1, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(p, itemID, oneInXChance, minQuantity, maxQuantity) : 0;
        }
        #endregion
    }
}
