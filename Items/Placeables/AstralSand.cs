using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralSand : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Sand");
        }

        public override void SetDefaults()
        {
            item.createTile = mod.TileType("AstralSand");
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 16;
            item.height = 16;
            item.maxStack = 999;
        }
    }
}
