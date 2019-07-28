using Terraria;
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
            item.createTile = mod.TileType("SeaPrismCrystals");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 5, 0);
			item.rare = 2;
        }
    }
}
