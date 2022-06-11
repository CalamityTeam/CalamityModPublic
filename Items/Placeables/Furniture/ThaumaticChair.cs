using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Items.Placeables.FurnitureAshen;
using CalamityMod.Items.Placeables.FurnitureMonolith;
using CalamityMod.Items.Placeables.FurnitureBotanic;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Placeables.FurnitureEutrophic;
using CalamityMod.Items.Placeables.FurnitureOtherworldly;
using CalamityMod.Items.Placeables.FurnitureProfaned;
using CalamityMod.Items.Placeables.FurnitureSilva;
using CalamityMod.Items.Placeables.FurnitureStatigel;
using CalamityMod.Items.Placeables.FurnitureStratus;
using CalamityMod.Items.Placeables.FurnitureVoid;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.FurniturePlagued;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class ThaumaticChair : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Thaumatic Chair"); // for Deallly#3625 who apparently deleted their account.  how do I contact them??????
            Tooltip.SetDefault("One of Chloe's finest creations"); // who is Chloe?
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 34;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ThaumaticChairTile>();
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ModContent.ItemType<AbyssChair>()).AddIngredient(ModContent.ItemType<AshenChair>()).AddIngredient(ModContent.ItemType<BotanicChair>()).AddIngredient(ModContent.ItemType<CosmiliteChair>()).AddIngredient(ModContent.ItemType<EutrophicChair>()).AddIngredient(ModContent.ItemType<MonolithChair>()).AddIngredient(ModContent.ItemType<OtherworldlyChair>()).AddIngredient(ModContent.ItemType<PlaguedPlateChair>()).AddIngredient(ModContent.ItemType<ProfanedChair>()).AddIngredient(ModContent.ItemType<SilvaChair>()).AddIngredient(ModContent.ItemType<StatigelChair>()).AddIngredient(ModContent.ItemType<StratusChair>()).AddIngredient(ModContent.ItemType<VoidChair>()).AddIngredient(ModContent.ItemType<AuricBar>()).AddTile(ModContent.TileType<CosmicAnvil>()).Register();
        }
    }
}
