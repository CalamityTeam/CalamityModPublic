using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Items.Placeables.FurnitureAshen;
using CalamityMod.Items.Placeables.FurnitureAstral;
using CalamityMod.Items.Placeables.FurnitureBotanic;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Placeables.FurnitureEutrophic;
using CalamityMod.Items.Placeables.FurnitureOccult;
using CalamityMod.Items.Placeables.FurniturePlaguedPlate;
using CalamityMod.Items.Placeables.FurnitureProfaned;
using CalamityMod.Items.Placeables.FurnitureSilva;
using CalamityMod.Items.Placeables.FurnitureStatigel;
using CalamityMod.Items.Placeables.FurnitureStratus;
using CalamityMod.Items.Placeables.FurnitureVoid;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class ThaumaticChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Thaumatic Chair"); // for Deallly#3625 who apparently deleted their account.  how do I contact them??????
            Tooltip.SetDefault("One of Chloe's finest creations"); // who is Chloe?
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 34;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.createTile = ModContent.TileType<ThaumaticChairTile>();
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<AbyssChair>());
            recipe.AddIngredient(ModContent.ItemType<AshenChair>());
            recipe.AddIngredient(ModContent.ItemType<BotanicChair>());
            recipe.AddIngredient(ModContent.ItemType<CosmiliteChair>());
            recipe.AddIngredient(ModContent.ItemType<EutrophicChair>());
            recipe.AddIngredient(ModContent.ItemType<MonolithChair>());
            recipe.AddIngredient(ModContent.ItemType<OccultChair>());
            recipe.AddIngredient(ModContent.ItemType<PlaguedPlateChair>());
            recipe.AddIngredient(ModContent.ItemType<ProfanedChair>());
            recipe.AddIngredient(ModContent.ItemType<SilvaChair>());
            recipe.AddIngredient(ModContent.ItemType<StatigelChair>());
            recipe.AddIngredient(ModContent.ItemType<StratusChair>());
            recipe.AddIngredient(ModContent.ItemType<VoidChair>());
            recipe.AddIngredient(ModContent.ItemType<AuricBar>());
            recipe.SetResult(this);
            recipe.AddTile(ModContent.TileType<CosmicAnvil>());
            recipe.AddRecipe();
        }
    }
}
