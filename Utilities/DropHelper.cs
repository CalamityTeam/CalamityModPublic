using CalamityMod.Items.Accessories;
using CalamityMod.Items.Ammo.FiniteUse;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod
{
    public static class DropHelper
    {
        #region Global Drop Chances
        /// <summary>
        /// The Defiled Rune boosts various low drop rates to one in this value.
        /// </summary>
        public const int DefiledDropRateInt = 20;

        /// <summary>
        /// The Defiled Rune boosts various low drop rates to this chance (decimal number out of 1.0).
        /// </summary>
        public const float DefiledDropRateFloat = 0.05f;

        /// <summary>
        /// Legendary drops have a 1 in X chance of dropping, where X is this variable.
        /// </summary>
        public const int LegendaryDropRateInt = 100;

        /// <summary>
        /// Legendary weapons have this chance to drop (decimal number out of 1.0).
        /// </summary>
        public const float LegendaryDropRateFloat = 0.01f;

        /// <summary>
        /// Rare Item Variants have a 1 in X chance of dropping, where X is this variable.
        /// </summary>
        public const int RareVariantDropRateInt = 40;

        /// <summary>
        /// Rare Item Variants have this chance to drop (decimal number out of 1.0).
        /// </summary>
        public const float RareVariantDropRateFloat = 0.025f;
        #endregion

        #region Weighted Item Sets
        public const float DefaultWeight = 1f;
        public const float MinisiculeWeight = 1E-6f;

        // TODO -- DropHelper will need to be fully retooled in 1.4 to utilize this struct for all functions.
        public struct WeightedItemStack
        {
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
        }

        public static WeightedItemStack WeightStack(this int itemID) => WeightStack(itemID, DefaultWeight);
        public static WeightedItemStack WeightStack(this int itemID, float weight) => new WeightedItemStack(itemID, weight);
        public static WeightedItemStack WeightStack(this int itemID, int quantity) => WeightStack(itemID, DefaultWeight, quantity);
        public static WeightedItemStack WeightStack(this int itemID, float weight, int quantity) => new WeightedItemStack(itemID, weight, quantity);
        public static WeightedItemStack WeightStack(this int itemID, int min, int max) => WeightStack(itemID, DefaultWeight, min, max);
        public static WeightedItemStack WeightStack(this int itemID, float weight, int min, int max) => new WeightedItemStack(itemID, weight, min, max);

        // Separated implementation used so weighted random code isn't duplicated in two places.
        private static WeightedItemStack RollWeightedRandom(WeightedItemStack[] stacks)
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
                    w = MinisiculeWeight;
                breakpoints[i] = totalWeight += w;
            }

            // Iterate through the breakpoints until you find the first one that is surpassed. Drop that item.
            float needle = Main.rand.NextFloat(totalWeight);
            i = 0;
            while (needle > breakpoints[i])
                ++i;
            return stacks[i];
        }

        /// <summary>
        /// Chooses an item (or stack of items) from an array of drop definitions and drops it from the given NPC.<br></br>
        /// Each item is given a certain weight to spawn. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemFromWeightedSet(NPC npc, bool dropPerPlayer, params WeightedItemStack[] stacks)
        {
            // Can't choose anything from an empty array.
            if (stacks is null || stacks.Length == 0)
                return 0;

            WeightedItemStack stk = RollWeightedRandom(stacks);
            return DropItem(npc, stk.itemID, dropPerPlayer, stk.minQuantity, stk.maxQuantity);
        }

        /// <summary>
        /// Chooses an item (or stack of items) from an array of drop definitions and drops it from the given NPC.<br></br>
        /// Each item is given a certain weight to spawn.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemFromWeightedSet(NPC npc, params WeightedItemStack[] stacks)
        {
            return DropItemFromWeightedSet(npc, false, stacks);
        }

        /// <summary>
        /// Chooses an item (or stack of items) from an array of drop definitions and spawns it for the given player.<br></br>
        /// Each item is given a certain weight to spawn.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemFromWeightedSet(Player p, params WeightedItemStack[] stacks)
        {
            // Can't choose anything from an empty array.
            if (stacks is null || stacks.Length == 0)
                return 0;

            WeightedItemStack stk = RollWeightedRandom(stacks);
            return DropItem(p, stk.itemID, stk.minQuantity, stk.maxQuantity);
        }

        /// <summary>
        /// Rolls for each item (or stack of items) in an array of drop definitions to drop at their defined chances.<br></br>
        /// Always drops at least one of the defined stacks. Optionally spawns one copy of these drops per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the items.</param>
        /// <param name="dropPerPlayer">Whether the drops should be "instanced" (each player gets their own copy).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireWeightedSet(NPC npc, bool dropPerPlayer, params WeightedItemStack[] stacks)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (stacks is null || stacks.Length == 0)
                return numDrops;

            for (int i = 0; i < stacks.Length; ++i)
            {
                WeightedItemStack stk = stacks[i];
                numDrops += DropItemChance(npc, stk.itemID, dropPerPlayer, stk.weight, stk.minQuantity, stk.maxQuantity);
            }

            // If nothing at all was dropped, drop one thing at (weighted) random.
            if (numDrops <= 0)
                numDrops += DropItemFromWeightedSet(npc, dropPerPlayer, stacks);

            return numDrops;
        }

        /// <summary>
        /// Rolls for each item (or stack of items) in an array of drop definitions to drop at their defined chances.<br></br>
        /// Always drops at least one of the defined stacks.
        /// </summary>
        /// <param name="npc">The NPC which should drop the items.</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireWeightedSet(NPC npc, params WeightedItemStack[] stacks)
        {
            return DropEntireWeightedSet(npc, false, stacks);
        }

        /// <summary>
        /// Rolls for each item (or stack of items) in an array of drop definitions to drop at their defined chances.<br></br>
        /// Always drops at least one of the defined stacks.
        /// </summary>
        /// <param name="p">The player which should receive the item(s).</param>
        /// <param name="stacks">The array of drop definitions to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireWeightedSet(Player p, params WeightedItemStack[] stacks)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (stacks is null || stacks.Length == 0)
                return numDrops;

            for (int i = 0; i < stacks.Length; ++i)
            {
                WeightedItemStack stk = stacks[i];
                numDrops += DropItemChance(p, stk.itemID, stk.weight, stk.minQuantity, stk.maxQuantity);
            }

            // If nothing at all was dropped, drop one thing at (weighted) random.
            if (numDrops <= 0)
                numDrops += DropItemFromWeightedSet(p, stacks);

            return numDrops;
        }
        #endregion

        #region Extra Boss Bags
        /// <summary>
        /// The number of extra loot bags bosses drop when Revengeance Mode is active.<br></br>
        /// This is normally zero; Revengenace Mode provides no extra bags.
        /// </summary>
        public static int RevExtraBags = 0;

        /// <summary>
        /// The number of extra loot bags bosses drop when Death Mode is active.<br></br>
        /// This is normally zero; Death Mode provides no extra bags.
        /// </summary>
        public static int DeathExtraBags = 0;

        /// <summary>
        /// The number of extra loot bags bosses drop when the Defiled Rune is active.<br></br>
        /// This is normally zero; Defiled Rune provides no extra bags.
        /// </summary>
        public static int DefiledExtraBags = 0;

        /// <summary>
        /// The number of extra loot bags bosses drop when Armageddon is active.<br></br>
        /// This is normally 5. Bosses drop 5 bags on normal, and 6 on Expert+.
        /// </summary>
        public static int ArmageddonExtraBags = 5;
        #endregion

        #region Boss Bag Drop Helpers
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

            // Armageddon's bonus bags drop even on Normal.
            bagsDropped += DropArmageddonBags(theBoss);

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
            if (CalamityWorld.death)
            {
                for (int i = 0; i < DeathExtraBags; ++i)
                    theBoss.DropBossBags();

                bagsDropped += DeathExtraBags;
            }

            // If Defiled is active, possibly drop extra bags.
            if (CalamityWorld.defiled)
            {
                for (int i = 0; i < DefiledExtraBags; ++i)
                    theBoss.DropBossBags();

                bagsDropped += DefiledExtraBags;
            }

            return bagsDropped;
        }

        /// <summary>
        /// Drops the correct number of boss bags for Armageddon.
        /// </summary>
        /// <param name="theBoss">The NPC to drop boss bags for.</param>
        /// <returns>The number of boss bags dropped.</returns>
        public static int DropArmageddonBags(NPC theBoss)
        {
            if (!CalamityWorld.armageddon)
                return 0;

            for (int i = 0; i < ArmageddonExtraBags; ++i)
                theBoss.DropBossBags();
            return ArmageddonExtraBags;
        }
        #endregion

        #region Specific Drop Helpers
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

        public static bool DropRevBagAccessories(Player p)
        {
            return DropItemFromSetCondition(p, CalamityWorld.revenge, 0.05f, ModContent.ItemType<StressPills>(), ModContent.ItemType<Laudanum>(), ModContent.ItemType<HeartofDarkness>());
        }

        /// <summary>
        /// Drops finite use "Resident Evil" ammunition from the given NPC, if the downed boolean isn't already true.
        /// </summary>
        /// <param name="theBoss">The NPC to drop ammo from.</param>
        /// <param name="alreadyKilled">A downed boolean corresponding to this NPC. Use "false" to always drop ammo.</param>
        /// <param name="magnum">The number of Magnum Rounds to drop.</param>
        /// <param name="bazooka">The number of Grenade Rounds to drop.</param>
        /// <param name="hydra">The number of Explosive Shells to drop.</param>
        /// <returns>The total amount of ammunition dropped.</returns>
        public static int DropResidentEvilAmmo(NPC theBoss, bool alreadyKilled, int magnum, int bazooka, int hydra)
        {
            if (alreadyKilled)
                return 0;

            int dropped = 0;
            dropped += DropItem(theBoss, ModContent.ItemType<MagnumRounds>(), magnum);
            dropped += DropItem(theBoss, ModContent.ItemType<GrenadeRounds>(), bazooka);
            dropped += DropItem(theBoss, ModContent.ItemType<ExplosiveShells>(), hydra);
            return dropped;
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
                Item.NewItem(pos, itemID, stackSize);
                dropped += stackSize;
            }

            return dropped;
        }

        /// <summary>
        /// Drops an item that may instead be replaced by a given Rare Item Variant (RIV).
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="itemID">The ID of the normal item to drop.</param>
        /// <param name="rareID">The ID of the rare item to drop.</param>
        /// <param name="itemChance">The chance that one of the two will drop. A decimal number <= 1.0.</param>
        /// <param name="rareChance">The chance that the RIV will drop. A decimal number <= 1.0.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemRIV(NPC npc, int itemID, int rareID, float itemChance, float rareChance = RareVariantDropRateFloat)
        {
            float f = Main.rand.NextFloat();
            bool replaceWithRare = f <= rareChance; // 1/X chance overall of getting RIV
            if (f <= itemChance) // 1/X chance of getting original OR the RIV replacing it
            {
                DropItemCondition(npc, itemID, !replaceWithRare);
                DropItemCondition(npc, rareID, replaceWithRare);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drops an item that may instead be replaced by a given Rare Item Variant (RIV).
        /// </summary>
        /// <param name="player">The player which should receive the item.</param>
        /// <param name="itemID">The ID of the normal item to drop.</param>
        /// <param name="rareID">The ID of the rare item to drop.</param>
        /// <param name="itemChance">The chance that one of the two will drop. A decimal number <= 1.0.</param>
        /// <param name="rareChance">The chance that the RIV will drop. A decimal number <= 1.0.</param>
        /// <returns>Whether an item was spawned.</returns>
        public static bool DropItemRIV(Player player, int itemID, int rareID, float itemChance, float rareChance = RareVariantDropRateFloat)
        {
            float f = Main.rand.NextFloat();
            bool replaceWithRare = f <= rareChance; // 1/X chance overall of getting RIV
            if (f <= itemChance) // 1/X chance of getting original OR the RIV replacing it
            {
                DropItemCondition(player, itemID, !replaceWithRare);
                DropItemCondition(player, rareID, replaceWithRare);
                return true;
            }
            return false;
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
        public static int DropItem(NPC npc, int itemID, bool dropPerPlayer, int minQuantity = 1, int maxQuantity = 0)
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

            // If the drop is supposed to be instanced, drop it as such.
            if (dropPerPlayer)
            {
                npc.DropItemInstanced(npc.position, npc.Size, itemID, quantity, true);
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
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, bool dropPerPlayer, float chance, int minQuantity = 1, int maxQuantity = 0)
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
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, float chance, int minQuantity = 1, int maxQuantity = 0)
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
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, bool dropPerPlayer, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
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
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemChance(NPC npc, int itemID, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
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
        public static int DropItemCondition(NPC npc, int itemID, bool dropPerPlayer, bool condition, int minQuantity = 1, int maxQuantity = 0)
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
        public static int DropItemCondition(NPC npc, int itemID, bool condition, int minQuantity = 1, int maxQuantity = 0)
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
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool dropPerPlayer, bool condition, float chance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, dropPerPlayer, chance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="chance">The chance that the items will drop. A decimal number <= 1.0.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool condition, float chance, int minQuantity = 1, int maxQuantity = 0)
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
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool dropPerPlayer, bool condition, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, dropPerPlayer, oneInXChance, minQuantity, maxQuantity) : 0;
        }

        /// <summary>
        /// With a condition and at a chance, drops a stack of one or more items from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item(s).</param>
        /// <param name="itemID">The ID of the item(s) to drop.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="oneInXChance">The chance that the items will drop is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="minQuantity">The minimum number of items to drop. Defaults to 1.</param>
        /// <param name="maxQuantity">The maximum number of items to drop. Defaults to 0, meaning the minimum quantity is always used.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropItemCondition(NPC npc, int itemID, bool condition, int oneInXChance, int minQuantity = 1, int maxQuantity = 0)
        {
            return condition ? DropItemChance(npc, itemID, false, oneInXChance, minQuantity, maxQuantity) : 0;
        }
        #endregion

        #region NPC Item Set Drops
        /// <summary>
        /// Chooses an item from an array and drops it from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>Whether an item was dropped.</returns>
        public static bool DropItemFromSet(NPC npc, bool dropPerPlayer, params int[] itemIDs)
        {
            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return false;

            // Choose which item to drop.
            int itemID = Main.rand.Next(itemIDs);

            // If the drop is supposed to be instanced, drop it as such.
            if (dropPerPlayer)
            {
                npc.DropItemInstanced(npc.position, npc.Size, itemID, 1, true);
            }
            else
            {
                Item.NewItem(npc.Hitbox, itemID);
            }

            return true;
        }

        /// <summary>
        /// Chooses an item from an array and drops it from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>Whether an item was dropped.</returns>
        public static bool DropItemFromSet(NPC npc, params int[] itemIDs)
        {
            return DropItemFromSet(npc, false, itemIDs);
        }

        /// <summary>
        /// At a chance, chooses an item from an array and drops it from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="chance">The chance that the item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>Whether an item was dropped.</returns>
        public static bool DropItemFromSetChance(NPC npc, bool dropPerPlayer, float chance, params int[] itemIDs)
        {
            // If you fail the roll to get the drop, stop immediately.
            if (Main.rand.NextFloat() > chance)
                return false;

            return DropItemFromSet(npc, dropPerPlayer, itemIDs);
        }

        /// <summary>
        /// At a chance, chooses an item from an array and drops it from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="chance">The chance that the item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>Whether an item was dropped.</returns>
        public static bool DropItemFromSetChance(NPC npc, float chance, params int[] itemIDs)
        {
            return DropItemFromSetChance(npc, false, chance, itemIDs);
        }
        #endregion

        #region NPC Item Set Drops Conditional
        /// <summary>
        /// With a condition, chooses an item from an array and drops it from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        public static bool DropItemFromSetCondition(NPC npc, bool dropPerPlayer, bool condition, params int[] itemIDs)
        {
            return condition ? DropItemFromSet(npc, dropPerPlayer, itemIDs) : false;
        }

        /// <summary>
        /// With a condition, chooses an item from an array and drops it from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        public static bool DropItemFromSetCondition(NPC npc, bool condition, params int[] itemIDs)
        {
            return condition ? DropItemFromSet(npc, false, itemIDs) : false;
        }

        /// <summary>
        /// With a condition, chooses an item from an array and drops it from the given NPC. Optionally spawns one copy of this drop per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="dropPerPlayer">Whether the drop should be "instanced" (each player gets their own copy).</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="chance">The chance that the item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        public static bool DropItemFromSetCondition(NPC npc, bool dropPerPlayer, bool condition, float chance, params int[] itemIDs)
        {
            return condition ? DropItemFromSetChance(npc, dropPerPlayer, chance, itemIDs) : false;
        }

        /// <summary>
        /// With a condition, chooses an item from an array and drops it from the given NPC.
        /// </summary>
        /// <param name="npc">The NPC which should drop the item.</param>
        /// <param name="condition">Any arbitrary Boolean condition to gate this drop. If false, nothing is dropped.</param>
        /// <param name="chance">The chance that the item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        public static bool DropItemFromSetCondition(NPC npc, bool condition, float chance, params int[] itemIDs)
        {
            return condition ? DropItemFromSetChance(npc, false, chance, itemIDs) : false;
        }
        #endregion

        #region NPC Entire Set Drops
        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.<br></br>
        /// Optionally spawns one copy of these drops per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the items.</param>
        /// <param name="dropPerPlayer">Whether the drops should be "instanced" (each player gets their own copy).</param>
        /// <param name="chance">The chance that an item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(NPC npc, bool dropPerPlayer, float chance, params int[] itemIDs)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return numDrops;

            // Tally the total number of items dropped as the drop set is iterated through.
            for (int i = 0; i < itemIDs.Length; ++i)
                numDrops += DropItemChance(npc, itemIDs[i], dropPerPlayer, chance);

            // If nothing at all was dropped, drop one thing at random.
            numDrops += DropItemFromSetCondition(npc, dropPerPlayer, numDrops <= 0, itemIDs) ? 1 : 0;
            return numDrops;
        }

        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.<br></br>
        /// Optionally spawns one copy of these drops per player.
        /// </summary>
        /// <param name="npc">The NPC which should drop the items.</param>
        /// <param name="dropPerPlayer">Whether the drops should be "instanced" (each player gets their own copy).</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(NPC npc, bool dropPerPlayer, int oneInXChance, params int[] itemIDs)
        {
            int numDrops = 0;

            // Can't choose anything from an empty array.
            if (itemIDs is null || itemIDs.Length == 0)
                return numDrops;

            // Tally the total number of items dropped as the drop set is iterated through.
            for (int i = 0; i < itemIDs.Length; ++i)
                numDrops += DropItemChance(npc, itemIDs[i], dropPerPlayer, oneInXChance);

            // If nothing at all was dropped, drop one thing at random.
            numDrops += DropItemFromSetCondition(npc, dropPerPlayer, numDrops <= 0, itemIDs) ? 1 : 0;
            return numDrops;
        }

        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.
        /// </summary>
        /// <param name="npc">The NPC which should drop the items.</param>
        /// <param name="chance">The chance that an item will drop. A decimal number <= 1.0.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(NPC npc, float chance, params int[] itemIDs)
        {
            return DropEntireSet(npc, false, chance, itemIDs);
        }

        /// <summary>
        /// Rolls for each item in an array to drop at a given chance. Always drops at least one item.
        /// </summary>
        /// <param name="npc">The NPC which should drop the items.</param>
        /// <param name="oneInXChance">The chance that the items will spawn is 1 in this number. For example, 5 gives a 1 in 5 chance.</param>
        /// <param name="itemIDs">The array of items to choose from. If it's null or empty, nothing will be dropped.</param>
        /// <returns>The number of items dropped.</returns>
        public static int DropEntireSet(NPC npc, int oneInXChance, params int[] itemIDs)
        {
            return DropEntireSet(npc, false, oneInXChance, itemIDs);
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
