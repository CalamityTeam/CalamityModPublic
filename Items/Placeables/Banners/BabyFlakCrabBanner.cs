using CalamityMod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Banners
{
    public class BabyFlakCrabBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            DisplayName.SetDefault("Baby Flak Crab Banner");
            Tooltip.SetDefault("{$CommonItemTooltip.BannerBonus}Baby Flak Crab");
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
            Item.placeStyle = 119;
        }
    }
}
