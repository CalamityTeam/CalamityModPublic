using CalamityMod.Tiles.Furniture;
using CalamityMod.Items.Critters;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class TwinklerInABottle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkler in a Bottle");
        }

        public override void SetDefaults()
        {
            item.width = 34;
            item.height = 32;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<TwinklerInABottleTile>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.Bottle);
            recipe.AddIngredient(ModContent.ItemType<TwinklerItem>());
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
