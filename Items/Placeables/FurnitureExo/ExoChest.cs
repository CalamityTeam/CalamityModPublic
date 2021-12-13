using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture.CraftingStations;using CalamityMod.Tiles.FurnitureExo;
using Terraria.ModLoader;
using Terraria.ID;
namespace CalamityMod.Items.Placeables.FurnitureExo
{
    public class ExoChest : ModItem
    {
        public override void SetStaticDefaults()
        {
        }

        public override void SetDefaults()
        {
            item.SetNameOverride("Exo Chest");
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<ExoChestTile>();
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExoPrism>(), 8);
            recipe.AddIngredient(ItemID.IronBar, 2);
            recipe.anyIronBar = true;
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.AddRecipe();
        }
    }
}
