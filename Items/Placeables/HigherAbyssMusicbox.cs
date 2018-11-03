using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class HigherAbyssMusicbox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Music Box (Higher Abyss)");
        }

        public override void SetDefaults()
        {
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.createTile = mod.TileType("HigherAbyssMusicbox");
            item.width = 24;
            item.height = 24;
            item.rare = 4;
            item.value = 100000;
            item.accessory = true;
        }
    }
}