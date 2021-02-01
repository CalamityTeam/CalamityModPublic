using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class Tenebris : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tenebris");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Abyss.Tenebris>();
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 8);
            item.rare = ItemRarityID.Lime;
        }
    }
}
