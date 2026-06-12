using System;
using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using CalamityMod.Cooldowns;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DaawnlightSpiritOrigin : ModItem, ILocalizedModType, IHoldShiftTooltipItem
    {
        public new string LocalizationCategory => "Items.Accessories";

        public bool HidesNormalTooltip => true;
        public bool HasFlavorTooltip => true;
        public Color? FlavorTooltipColor => new(149, 28, 235); // #951CEB

        // "Despite the seemingly insane numbers here, I think this item might actually be underpowered"
        // hindsight: the item was not underpowered. Ozzatron 05NOV2021
        // 03SEP2024: Old comments kept for record.

        #region Balancing Variables

        /// <summary>
        /// The bullseye's total lifespan while it is not hit.
        /// </summary>
        public const int BullseyeIdleLifetime = 600;

        /// <summary>
        /// The bullseye's lifespan when hit.
        /// </summary>
        public const int BullseyeHitLifetime = 90;

        /// <summary>
        /// The amount of critical strike chance bonus where crit starts decaying faster.<br/>
        /// When the crit bonus reaches this value, the decay rate increases by one tier.
        /// </summary>
        public static readonly int CritDecayThreshold = 50;

        /// <summary>
        /// When the critical strike chance bonus has exceeded <see cref="CritDecayThreshold"/>, it continues to decay faster for every <see cref="CritDecayEchelon"/> more.
        /// </summary>
        public static readonly int CritDecayEchelon = 10;

        /// <summary>
        /// By default, critical strike chance is lost at 15% per second (1% every 4 frames). This is the first value to which decay echelons are applied.<br/>
        /// Once this value reaches 1, the player instead starts losing multiple % of crit every frame.
        /// </summary>
        public static readonly int CritDecayBaseRate = 4;

        // These were very carefully calculated, please don't change them.
        internal const float RegularEnemyBullseyeRadius = 8f;
        internal const float BossBullseyeRadius = 18f;

        // Special search radius for coin ricoshots that only applies to DSO targets.
        public static readonly float RicoshotSearchDistance = 2800f;

        /// <summary>
        /// The maximum amount of extra critical strike chance you can get from this accessory.
        /// </summary>
        public static readonly int CritHardCap = 75;

        #endregion

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 38;
            Item.accessory = true;
            Item.rare = ItemRarityID.Purple;
            Item.value = CalamityGlobalItem.RarityPurpleBuyPrice;
            Item.Calamity().donorItem = true;
        }

        // The pet is purely visual and does not affect the functionality of the item.
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.spiritOrigin = true;

            // If visibility is disabled, despawn the pet.
            if (hideVisual)
            {
                if (player.FindBuffIndex(ModContent.BuffType<ArcherofLunamoon>()) != -1)
                    player.ClearBuff(ModContent.BuffType<ArcherofLunamoon>());
            }

            // If visibility is enabled, spawn the pet.
            else if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<ArcherofLunamoon>()) == -1)
                    player.AddBuff(ModContent.BuffType<ArcherofLunamoon>(), 18000, true);
            }

            // Update the current crit boost.
            int currentCritBoost = modPlayer.spiritOriginCritBoost;

            // Calculate how many tiers / echelons of decay are currently affecting the crit boost.
            int decayRateEchelons = 0;
            if (currentCritBoost >= CritDecayThreshold)
                decayRateEchelons = 1 + ((currentCritBoost - CritDecayThreshold) / CritDecayEchelon);

            // This is the current decay rate. Crit is lost once every this many frames.
            int decayRate = Math.Max(1, CritDecayBaseRate - decayRateEchelons);
            int percentToDecay = 1;

            // If enough echelons have been reached that crit is already draining once per frame,
            // then start removing multiple percent crit per frame as well.
            if (decayRateEchelons >= CritDecayBaseRate)
                percentToDecay += decayRateEchelons - CritDecayBaseRate;

            // Actually decay the crit chance boost.
            if (player.miscCounter % decayRate == 0 && currentCritBoost > 0)
                currentCritBoost -= percentToDecay;

            // Write out the new value to the tracked stat on the player.
            modPlayer.spiritOriginCritBoost = currentCritBoost;

            // Actually give the crit boost as a direct increase to ranged critical strike chance.
            player.GetCritChance<RangedDamageClass>() += Math.Min(modPlayer.spiritOriginCritBoost, CritHardCap);

            // Display the current crit boost on a cooldown.
            if (modPlayer.cooldowns.TryGetValue(DaawnlightSpiritOriginExtraCrit.ID, out var cooldown))
            {
                int displayedCritOnCooldown = Math.Max(0, CritDecayThreshold - currentCritBoost);
                cooldown.timeLeft = displayedCritOnCooldown;
            }
            else
                player.AddCooldown(DaawnlightSpiritOriginExtraCrit.ID, CritDecayThreshold);
        }

        public override void UpdateVanity(Player player)
        {
            // Summon anime girl if it's in vanity slot as the pet is purely vanity
            // It's possible for other "pet" items like Fungal Clump or HotE to summon a passive version of their "pets" with some tweaks though
            player.Calamity().spiritOriginVanity = true;
            if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<ArcherofLunamoon>()) == -1)
                    player.AddBuff(ModContent.BuffType<ArcherofLunamoon>(), 18000, true);
            }
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DeadshotBrooch>().
                AddIngredient<MysteriousCircuitry>(15).
                AddIngredient<DubiousPlating>(15).
                AddIngredient(ItemID.LunarBar, 10).
                AddIngredient(ItemID.FragmentVortex, 5).
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
