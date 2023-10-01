using CalamityMod.Tiles.Furniture;
using CalamityMod.Items.Critters;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class TwinklerInABottle : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<TwinklerInABottleTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.Bottle).
                AddIngredient<TwinklerItem>().
                Register();
        }
    }
}
