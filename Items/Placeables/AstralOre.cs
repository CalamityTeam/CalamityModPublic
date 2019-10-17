using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class AstralOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.AstralOre>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 36);
            item.rare = 9;
        }
    }
}
