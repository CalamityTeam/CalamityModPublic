using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousSink : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("Counts as a water, honey, and lava source");
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Sacrilegious Sink");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousSinkTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 6).
				AddIngredient(ItemID.WaterBucket).
				AddIngredient(ItemID.HoneyBucket).
				AddIngredient(ItemID.LavaBucket).
				AddTile(ModContent.TileType<CosmicAnvil>()).
				Register();
        }
    }
}
