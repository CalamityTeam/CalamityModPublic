using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Items.Placeables.FurnitureAcidwood;
using CalamityMod.Items.Placeables.FurnitureAncient;
using CalamityMod.Items.Placeables.FurnitureAshen;
using CalamityMod.Items.Placeables.FurnitureMonolith;
using CalamityMod.Items.Placeables.FurnitureBotanic;
using CalamityMod.Items.Placeables.FurnitureCosmilite;
using CalamityMod.Items.Placeables.FurnitureEutrophic;
using CalamityMod.Items.Placeables.FurnitureExo;
using CalamityMod.Items.Placeables.FurnitureSacrilegious;
using CalamityMod.Items.Placeables.FurnitureOtherworldly;
using CalamityMod.Items.Placeables.FurnitureProfaned;
using CalamityMod.Items.Placeables.FurnitureSilva;
using CalamityMod.Items.Placeables.FurnitureStatigel;
using CalamityMod.Items.Placeables.FurnitureStratus;
using CalamityMod.Items.Placeables.FurnitureVoid;
using CalamityMod.Items.Placeables.FurnitureWulfrum;
using CalamityMod.Rarities;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Tiles.Furniture.CraftingStations;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items.Placeables.FurniturePlagued;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class ThaumaticChair : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<ThaumaticChairTile>();
            Item.rare = ModContent.RarityType<Violet>();
            Item.Calamity().donorItem = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient<AbyssChair>().
            AddIngredient<AcidwoodChair>().
            AddIngredient<AncientChair>().
            AddIngredient<AshenChair>().
            AddIngredient<BotanicChair>().
            AddIngredient<CosmiliteChair>().
            AddIngredient<EutrophicChair>().
            AddIngredient<ExoChair>().
            AddIngredient<MonolithChair>().
            AddIngredient<SacrilegiousChair>().
            AddIngredient<OtherworldlyChair>().
            AddIngredient<PlaguedPlateChair>().
            AddIngredient<ProfanedChair>().
            AddIngredient<SilvaChair>().
            AddIngredient<StatigelChair>().
            AddIngredient<StratusChair>().
            AddIngredient<VoidChair>().
            AddIngredient<WulfrumChair>().
            AddIngredient<AuricBar>().
            AddTile<CosmicAnvil>().
            Register();
        }
    }
}
