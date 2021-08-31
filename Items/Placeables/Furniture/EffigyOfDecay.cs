using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class EffigyOfDecay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Effigy of Decay");
            Tooltip.SetDefault("When placed down, nearby players can breathe underwater\n" +
                               "This effect does not work in the abyss\n" +
                               "Nearby players are also immune to the sulphuric poisoning");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = ItemRarityID.Orange;
            item.createTile = ModContent.TileType<EffigyOfDecayPlaceable>();
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<SulfuricScale>(), 20);
            recipe.AddRecipeGroup("IronBar", 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
