using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class AmbrosialAmpoule : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = CalamityGlobalItem.Rarity9BuyPrice;
            Item.rare = ItemRarityID.Cyan;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            player.statLifeMax2 += 70;

            // bool left in for abyss light purposes and life regen effects
            modPlayer.aAmpoule = true;

            // Inherits all effects of Honey Dew and Living Dew (except standing regen is not honey exclusive anymore)
            modPlayer.alwaysHoneyRegen = true;
            modPlayer.honeyDewHalveDebuffs = true;
            modPlayer.livingDewHalveDebuffs = true;

            // Add light if the other accessories aren't equipped and visibility is turned on
            if (!(modPlayer.rOoze || modPlayer.purity) && !hideVisual)
                Lighting.AddLight(player.Center, new Vector3(1.2f, 1.2f, 0.72f));
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LivingDew>().
                AddIngredient<RadiantOoze>().
                AddIngredient<LifeAlloy>(3).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
