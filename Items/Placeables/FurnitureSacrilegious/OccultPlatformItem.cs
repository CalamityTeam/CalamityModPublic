using CalamityMod.Rarities;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class OccultPlatformItem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Occult Platform");
            SacrificeTotal = 200;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<OccultPlatformTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(2).AddIngredient(ModContent.ItemType<OccultBrickItem>()).Register();
        }
    }
}
