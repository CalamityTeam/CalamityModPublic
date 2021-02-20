using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.FurnitureAncient
{
    public class AncientDresser : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 22;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.rare = ItemRarityID.Orange;
            item.consumable = true;
            item.value = 0;
            item.createTile = ModContent.TileType<Tiles.FurnitureAncient.AncientDresser>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BrimstoneSlag>(), 16);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<AncientAltar>());
            recipe.AddRecipe();
        }
    }
}
