using System;
using System.Linq;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Packets;
using CalamityMod.World;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Prefixes
{
    public sealed class ReforgeChange : GlobalItem
    {
        public override bool InstancePerEntity => false;

        // Ozzatron 31AUG2022: total rework to the reforge rework for mod compatibility
        // you can now disable the rework with config in case it isn't enough to solve your conflicts
        // removed data saved on items; reforging is now a coalescing flowchart that has no RNG
        private static int storedPrefix = -1;
        public override void PreReforge(Item item)
        {
            storedPrefix = item.prefix;
        }

        public override int ChoosePrefix(Item item, UnifiedRandom rand)
        {
            // Voucher override
            if (VoucherReforgeSystem.ForcedPrefix != -1)
            {
                int forced = VoucherReforgeSystem.ForcedPrefix;
                VoucherReforgeSystem.ForcedPrefix = -1;
                return forced;
            }

            if (storedPrefix == -1 && item.CountsAsClass<RogueDamageClass>() && (item.maxStack == 1 || item.AllowReforgeForStackableItem))
            {
                // Crafting (or first reforge of) a rogue weapon has a 75% chance for a random modifier, this check is done by vanilla
                // Negative modifiers have a 66.66% chance of being voided, Annoying modifier is intentionally ignored by vanilla
                int prefix = CalamityUtils.RandomRoguePrefix();
                bool keepPrefix = !CalamityUtils.NegativeRoguePrefix(prefix) || Main.rand.NextBool(3);
                return keepPrefix ? prefix : 0;
            }

            if (Main.gameMenu)
                return -1;

            if (storedPrefix != -1 && !item.accessory && CalamityServerConfig.Instance.RemoveReforgeRNG)
                return GetReworkedReforge(item, rand, storedPrefix);

            return -1;
        }

        public override void PostReforge(Item item)
        {
            storedPrefix = -1;

            // Bandit steals 20% of the total price of the reforge if she's around.
            if (NPC.AnyNPCs(NPCType<Bandit>()))
            {
                // Calculate the item's reforge cost.
                int value = item.value;
                Player p = Main.LocalPlayer;
                ItemLoader.ReforgePrice(item, ref value, ref p.discountAvailable);

                // Steal 20% of that money.
                int stolen = value / 5;
                CalamityWorld.MoneyStolenByBandit += stolen;

                // Increment the reforge counter to allow the Bandit to refund
                // Also triggers Tinkerer dialogue that hints to the player that money is being stolen
                CalamityWorld.Reforges++;

                if (Main.netMode == NetmodeID.MultiplayerClient)
                {
                    BanditStolenMoneySyncPacket.Send(stolen);
                }
            }
        }

        #region Prefix Definition
        [Obsolete("No longer used with the removal of the Simplify Accessory Reforge config")]
        public static int[] SimplifiedAccessoryPrefixes =
        [
            PrefixID.Warding,
            PrefixID.Menacing,
            PrefixID.Lucky,
            PrefixID.Quick2,
            PrefixID.Violent,
            PrefixID.Arcane,
            PrefixType<Silent>(),
            PrefixType<Invigorating>(),
            PrefixType<Dauntless>()
        ];

        [Obsolete("No longer used with the removal of the Simplify Accessory Reforge config")]
        public static int[][] AccessoryPrefixTiers =
        [
            /* 0 */ [PrefixID.Hard, PrefixID.Jagged, PrefixID.Brisk, PrefixID.Wild],
            /* 1 */ [PrefixID.Guarding, PrefixID.Spiked, PrefixID.Precise, PrefixID.Fleeting, PrefixID.Rash],
            /* 2 */ [PrefixID.Armored, PrefixID.Angry, PrefixID.Hasty2, PrefixID.Intrepid, PrefixID.Arcane],
            /* 3 */ [PrefixID.Warding, PrefixID.Menacing, PrefixID.Lucky, PrefixID.Quick2, PrefixID.Violent, PrefixType<Silent>(), PrefixType<Dauntless>(), PrefixType<Invigorating>()],
        ];

        public static int[][] TerrarianPrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous],
            /* 2 */ [PrefixID.Superior, PrefixID.Demonic, PrefixID.Godly],
            /* 3 */ [PrefixID.Legendary2],
        ];

        public static int[][] MeleePrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Light, PrefixID.Heavy, PrefixID.Light, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky],
            /* 2 */ [PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp],
            /* 3 */ [PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior],
            /* 4 */ [PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly],
            /* 5 */ [PrefixID.Legendary] // for non-tools, Light is a mediocre low-tier reforge
        ];

        public static int[][] MeleeNoSpeedPrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous],
            /* 2 */ [PrefixID.Superior, PrefixID.Demonic],
            /* 3 */ [PrefixID.Godly]
        ];

        // 14MAR2025: Ozzatron: Add support for allowing Ruthless over Godly if the player has more than 100% crit chance of the appropriate class
        public static int[][] MeleeNoSpeedAlwaysCritPrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous],
            /* 2 */ [PrefixID.Superior, PrefixID.Demonic],
            /* 3 */ [PrefixID.Godly, PrefixID.Ruthless]
        ];

        public static int[][] ToolPrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Heavy, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Pointy, PrefixID.Bulky],
            /* 2 */ [PrefixID.Murderous, PrefixID.Agile, PrefixID.Large, PrefixID.Dangerous, PrefixID.Sharp],
            /* 3 */ [PrefixID.Massive, PrefixID.Unpleasant, PrefixID.Savage, PrefixID.Superior],
            /* 4 */ [PrefixID.Demonic, PrefixID.Deadly2, PrefixID.Godly],
            /* 5 */ [PrefixID.Legendary, PrefixID.Light] // for some tools, light is better than legendary. for others, it's equal
        ];

        public static int[][] RangedPrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Powerful, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Intimidating],
            /* 2 */ [PrefixID.Murderous, PrefixID.Agile, PrefixID.Hasty, PrefixID.Staunch, PrefixID.Unpleasant],
            /* 3 */ [PrefixID.Superior, PrefixID.Demonic, PrefixID.Sighted],
            /* 4 */ [PrefixID.Godly, PrefixID.Rapid, /* ranged Deadly */ PrefixID.Deadly, /* universal Deadly */ PrefixID.Deadly2],
            /* 5 */ [PrefixID.Unreal]
        ];

        public static int[][] MagicPrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Furious, PrefixID.Forceful, PrefixID.Strong],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic],
            /* 2 */ [PrefixID.Murderous, PrefixID.Agile, PrefixID.Adept, PrefixID.Celestial, PrefixID.Unpleasant],
            /* 3 */ [PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic],
            /* 4 */ [PrefixID.Godly, PrefixID.Masterful, PrefixID.Deadly2],
            /* 5 */ [PrefixID.Mythical]
        ];

        public static int[][] SummonPrefixTiers =
        [
            /* 0 */ [PrefixID.Nimble, PrefixID.Furious],
            /* 1 */ [PrefixID.Forceful, PrefixID.Strong, PrefixID.Quick, PrefixID.Taboo, PrefixID.Manic],
            /* 2 */ [PrefixID.Hurtful, PrefixID.Adept, PrefixID.Celestial],
            /* 3 */ [PrefixID.Superior, PrefixID.Demonic, PrefixID.Mystic, PrefixID.Deadly2],
            /* 4 */ [PrefixID.Masterful, PrefixID.Godly],
            /* 5 */ [PrefixID.Mythical, PrefixID.Ruthless] // you may want mythical early game for the knockback.
        ];

        // Added appropriate universal reforges to rogue, so they don't ONLY get modded prefixes.
        public static int[][] RoguePrefixTiers =
        [
            /* 0 */ [PrefixID.Keen, PrefixID.Nimble, PrefixID.Nasty, PrefixID.Forceful, PrefixID.Strong, PrefixType<Radical>(), PrefixType<Pointy>()],
            /* 1 */ [PrefixID.Hurtful, PrefixID.Ruthless, PrefixID.Zealous, PrefixID.Quick, PrefixType<Sharp>(), PrefixType<Glorious>()],
            /* 2 */ [PrefixID.Murderous, PrefixID.Agile, PrefixID.Unpleasant, PrefixType<Feathered>(), PrefixType<Sleek>(), PrefixType<Hefty>()],
            /* 3 */ [PrefixID.Superior, PrefixID.Demonic, PrefixType<Mighty>(), PrefixType<Serrated>()],
            /* 4 */ [PrefixID.Godly, PrefixID.Deadly2, PrefixType<Vicious>(), PrefixType<Lethal>()],
            /* 5 */ [PrefixType<Flawless>()]
        ];
        #endregion

        #region Reworked Reforge
        internal static int GetReworkedReforge(Item item, UnifiedRandom rand, int currentPrefix)
        {
            // This is the hardcoded value to "do nothing", and is thus the default choice.
            int prefix = -1;
            bool supportsLegendary = PrefixLegacy.ItemSets.SwordsHammersAxesPicks[item.type] || (item.ModItem != null && item.ModItem.MeleePrefix());

            // MELEE (includes tools and whips)
            // Melee-ranged hybrid weapons prioritize Legendary if available, otherwise go for Unreal
            if ((item.CountsAsClass<MeleeDamageClass>() || item.CountsAsClass<SummonMeleeSpeedDamageClass>() || item.type == ItemType<EvilSmasher>()) && !(item.CountsAsClass<MeleeRangedHybridDamageClass>() && !supportsLegendary) && !(item.type == ItemType<TheBurningSky>()))
            {
                // Terrarian (has its own special "Legendary" for marketing reasons)
                // Other items that want to use Legendary2 are also compatible
                if (PrefixLegacy.ItemSets.ItemsThatCanHaveLegendary2[item.type])
                {
                    prefix = IteratePrefix(rand, TerrarianPrefixTiers, currentPrefix);
                }

                // Swords, Whips, Tools, other items that support the Legendary modifier
                else if (supportsLegendary)
                {
                    var tierListToUse = (item.pick > 0 || item.axe > 0 || item.hammer > 0) ? ToolPrefixTiers : MeleePrefixTiers;
                    prefix = IteratePrefix(rand, tierListToUse, currentPrefix);
                }

                // Yoyos, Flails, Spears, etc.
                // Spears actually work fine with Legendary, but vanilla doesn't give it to them, so we won't either.
                else
                {
                    bool has100Crit = Main.LocalPlayer.GetTotalCritChance(item.DamageType) >= 100;
                    prefix = IteratePrefix(rand, has100Crit ? MeleeNoSpeedAlwaysCritPrefixTiers : MeleeNoSpeedPrefixTiers, currentPrefix);
                }
            }

            // RANGED (and also Burning Sky)
            else if (item.CountsAsClass<RangedDamageClass>() || item.type == ItemType<TheBurningSky>())
            {
                prefix = IteratePrefix(rand, RangedPrefixTiers, currentPrefix);
            }

            // MAGIC
            else if (item.CountsAsClass<MagicDamageClass>() || item.CountsAsClass<MagicSummonHybridDamageClass>())
            {
                prefix = IteratePrefix(rand, MagicPrefixTiers, currentPrefix);
            }

            // SUMMON (not whips)
            else if (item.CountsAsClass<SummonDamageClass>())
            {
                prefix = IteratePrefix(rand, SummonPrefixTiers, currentPrefix);
            }

            // ROGUE (Calamity adds these reforges to all 1.4 TML throwing items)
            else if (item.CountsAsClass<ThrowingDamageClass>())
            {
                prefix = IteratePrefix(rand, RoguePrefixTiers, currentPrefix);
            }

            return prefix;
        }

        private static int GetPrefixTier(int[][] tiers, int currentPrefix)
        {
            for (int checkingTier = 0; checkingTier < tiers.Length; ++checkingTier)
            {
                int[] tierList = tiers[checkingTier];
                for (int i = 0; i < tierList.Length; ++i)
                    if (tierList[i] == currentPrefix)
                        return checkingTier;
            }

            // If an invalid or modded prefix is detected, return -1.
            // This will give a random tier 0 prefix (the "next tier"), starting fresh with a low-tier vanilla or Calamity prefix.
            return -1;
        }

        private static int IteratePrefix(UnifiedRandom rand, int[][] reforgeTiers, int currentPrefix)
        {
            int currentTier = GetPrefixTier(reforgeTiers, currentPrefix);

            // If max tier: give max tier reforges forever
            // Otherwise: go up by 1 tier with every reforge, guaranteed
            int newTier = currentTier == reforgeTiers.Length - 1 ? currentTier : currentTier + 1;
            return rand.Next(reforgeTiers[newTier]);
        }
        #endregion
    }
}
