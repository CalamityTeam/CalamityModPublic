using CalamityMod.Buffs.Pets;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DaawnlightSpiritOrigin : ModItem
    {
        // Despite the seemingly insane numbers here, I think this item might actually be underpowered
        // I left all the numbers easily accessible so it can be quickly rebalanced
        // Regular crits can be intentionally weak; bullseyes should be doing all the work
        // If the item is too weak, increase BullseyeCritRatio (originally 3.8f) until bullseyes are doing enough damage
        // - Ozzatron
        //
        private const float BullseyeCritRatio = 3.8f; // Bullseye crits deal x3.8 damage instead of x2.
        private const float StoredCritConversionRatio = 0.02f; // Add +2% more damage to crits for every 1% critical chance the player would have had.
        private const float MinUseTimeForSlowBonus = 11f;
        private const float MaxSlowBonusUseTime = 72f;
        private const float MaxSlowWeaponBonus = 0.96f; // Up to +96% more damage to crits for slower weapons.

        internal static float GetDamageMultiplier(Player p, CalamityPlayer mp, bool hitBullseye)
        {
            float baseCritMult = 2f; // In vanilla Terraria, crits do +100% damage.
            if (hitBullseye)
                baseCritMult = BullseyeCritRatio; // Replace a "regular crit" with a "bullseye crit".

            // Factor in the critical strike chance the player isn't getting to use.
            float convertedCritBonus = StoredCritConversionRatio * mp.spiritOriginConvertedCrit;

            float useTimeInterpolant = Utils.InverseLerp(MinUseTimeForSlowBonus, MaxSlowBonusUseTime, p.ActiveItem().useTime, true);
            float slowWeaponBonus = MathHelper.Lerp(0f, MaxSlowWeaponBonus, useTimeInterpolant);
            return baseCritMult * (1f + convertedCritBonus + slowWeaponBonus);
        }
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Daawnlight Spirit Origin");
            Tooltip.SetDefault("All nearby enemies and bosses are marked with bullseyes\n" +
                "Ranged attacks that strike a bullseye always critically strike and deal massive damage\n" +
                "When a bullseye is struck, it vanishes and a new one appears elsewhere\n" +
                "Explosions or large projectiles cannot strike bullseyes\n" +
                "Converts all ranged critical strike chance boosts into extra critical strike damage\n" +
                "All ranged weapons will deal even more critical strike damage the slower they are\n" +
                "'A strand of a lost cosmos remains, waiting for its master'");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
            item.accessory = true;
            item.rare = ItemRarityID.Purple;
            item.Calamity().customRarity = CalamityRarity.DarkBlue;
            item.value = CalamityGlobalItem.RarityDarkBlueBuyPrice;
            item.Calamity().donorItem = true;
        }

        // The pet is purely visual and does not affect the functionality of the item.
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().spiritOrigin = true;

            // If visibility is disabled, despawn the pet.
            if (hideVisual)
            {
                if (player.FindBuffIndex(ModContent.BuffType<DaawnlightSpiritOriginBuff>()) != -1)
                    player.ClearBuff(ModContent.BuffType<DaawnlightSpiritOriginBuff>());
            }
            // If visibility is enabled, spawn the pet.
            else if (player.whoAmI == Main.myPlayer)
            {
                if (player.FindBuffIndex(ModContent.BuffType<DaawnlightSpiritOriginBuff>()) == -1)
                    player.AddBuff(ModContent.BuffType<DaawnlightSpiritOriginBuff>(), 18000, true);
            }
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<DaedalusEmblem>());
            recipe.AddIngredient(ModContent.ItemType<LeadCore>(), 3);
            recipe.AddIngredient(ModContent.ItemType<DubiousPlating>(), 15);
            recipe.AddIngredient(ModContent.ItemType<MysteriousCircuitry>(), 15);
            recipe.AddIngredient(ModContent.ItemType<AscendantSpiritEssence>(), 4);
            // TODO -- cosmic anvil
            recipe.AddTile(TileID.LunarCraftingStation);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}