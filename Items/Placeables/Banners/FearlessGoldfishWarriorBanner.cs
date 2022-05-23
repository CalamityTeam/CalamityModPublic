using CalamityMod.Tiles;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Banners
{
    public class FearlessGoldfishWarriorBanner : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            Tooltip.SetDefault("{$CommonItemTooltip.BannerBonus}Fearless Goldfish Warrior");
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

            Item.value = Item.buyPrice(silver: 10);
            Item.rare = ItemRarityID.Blue;
            Item.Calamity().donorItem = true;

            Item.createTile = ModContent.TileType<MonsterBanner>();
            Item.placeStyle = 109;
        }
    }
}
