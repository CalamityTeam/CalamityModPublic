using CalamityMod.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Banners
{
    public class PiggyBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("{$CommonItemTooltip.BannerBonus}Piggy");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 24;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;

            item.value = Item.buyPrice(silver: 10);
            item.rare = ItemRarityID.Blue;
            item.Calamity().donorItem = true;

            item.createTile = ModContent.TileType<MonsterBanner>();
            item.placeStyle = 108;
        }
    }
}
