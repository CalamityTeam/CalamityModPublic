using CalamityMod.Tiles.FurnitureAcidwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureAcidwood
{
    public class AcidwoodPlatform : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 200;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Acidwood Platform");
            Item.width = 8;
            Item.height = 10;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<AcidwoodPlatformTile>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<Acidwood>()).Register();
        }
    }
}
