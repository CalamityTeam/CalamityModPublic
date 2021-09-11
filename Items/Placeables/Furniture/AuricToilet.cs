using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureBotanic;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Placeables.FurnitureSilva;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Furniture
{
    public class AuricToilet : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Auric Toilet");
            Tooltip.SetDefault("This was used by the gods");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 30;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<AuricToiletTile>();
            item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<BotanicChair>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteChair>());
            recipe.AddIngredient(ModContent.ItemType<SilvaChair>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>(), 4);
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<DraedonsForge>());
            recipe.AddRecipe();
        }
    }
}
