using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class PerennialOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Perennial Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.PerennialOre>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 12;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 18);
            item.rare = 7;
        }
    }
}
