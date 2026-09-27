using CalamityMod.Items.Materials;
using CalamityMod.Tiles.Furniture;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class WulfrumLureItem : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Placeables";
        public static int SignalTime = 30 * 60;
        public static int SpawnIntervals = 4 * 60;
        public static int MaxEnemiesPerWave = 5;
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(SignalTime.FramesToSeconds());

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<WulfrumLure>());
            Item.value = Item.sellPrice(silver: 1);
            Item.rare = ItemRarityID.Blue;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
            AddIngredient<WulfrumMetalScrap>(5).
            AddIngredient<EnergyCore>().
            AddTile(TileID.Anvils).
            Register();
        }
    }
}
