using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class BrimstoneSlag : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Slag");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("BrimstoneSlag");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = 1000;
            item.rare = 3;
        }
    }
}
