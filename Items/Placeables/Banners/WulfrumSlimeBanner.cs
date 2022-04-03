using CalamityMod.Tiles;
using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Banners
{
    public class WulfrumSlimeBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("A decorative banner themed after an extinct slime species");
        }

        public override void SetDefaults()
        {
            Item.width = 10;
            Item.height = 24;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.buyPrice(0, 0, 10, 0);
            Item.createTile = ModContent.TileType<MonsterBanner>();
            Item.placeStyle = 51;
        }
        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.Silk, 3).AddIngredient(ModContent.ItemType<WulfrumShard>(), 3).AddTile(TileID.Loom).Register();
        }
    }
}
