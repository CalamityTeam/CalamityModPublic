using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureSacrilegious;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureSacrilegious
{
    public class SacrilegiousCandelabra : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.SetNameOverride("Sacrilegious Candelabra");
            Item.width = 28;
            Item.height = 20;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<SacrilegiousCandelabraTile>();
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
				AddIngredient(ModContent.ItemType<OccultBrickItem>(), 5).
				AddIngredient(ModContent.ItemType<AshesofCalamity>(), 3).
				AddTile(ModContent.TileType<CosmicAnvil>()).
				Register();
        }
    }
}
