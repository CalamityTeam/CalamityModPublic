using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class CryonicOre : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryonic Ore");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("CryonicOre");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 5;
        }
    }
}
