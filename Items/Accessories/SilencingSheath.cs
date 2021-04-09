using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SilencingSheath : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Silencing Sheath");
            Tooltip.SetDefault("+20 maximum stealth\n" +
                "Stealth generates 15% faster");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 34;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rogueStealthMax += 0.2f;
            modPlayer.stealthGenStandstill += 0.15f;
            modPlayer.stealthGenMoving += 0.15f;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddRecipeGroup("AnyEvilBar", 8);
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddRecipeGroup("Boss2Material", 3);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
