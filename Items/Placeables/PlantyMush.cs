using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class PlantyMush : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Planty Mush");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Abyss.PlantyMush>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 6);
            item.rare = 3;
        }
    }
}
