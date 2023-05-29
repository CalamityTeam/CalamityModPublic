using CalamityMod.Items.Materials;
using CalamityMod.Items.Accessories;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Tiles.Furniture;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class WulfrumLureItem : ModItem, ILocalizedModType
    {
        public string LocalizationCategory => "Items.Placeables";
        public static int SignalTime = 30 * 60;
        public static int SpawnIntervals = 4 * 60;
        public static int MaxEnemiesPerWave = 3;

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 22;
            Item.maxStack = 9999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 14;
            Item.rare = ItemRarityID.Blue;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createTile = ModContent.TileType<WulfrumLure>();
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).
            AddIngredient(ModContent.ItemType<WulfrumMetalScrap>(), 5).
            AddIngredient(ModContent.ItemType<WulfrumBattery>(), 1).
            AddTile(TileID.Anvils).
            Register();
        }
    }
}
