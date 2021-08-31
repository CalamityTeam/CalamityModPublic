using CalamityMod.Tiles.SunkenSea;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables
{
    public class PrismShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prism Shard");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<SeaPrismCrystals>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = ItemRarityID.Green;
        }
    }
}
