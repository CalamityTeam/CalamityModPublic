using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class MetaTile : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Superdirectional Tile Example");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.MetaTileCrystalExample>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Green;
        }
    }
}
