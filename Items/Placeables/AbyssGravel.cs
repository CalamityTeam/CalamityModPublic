using Terraria.ModLoader;
namespace CalamityMod.Items
{
    public class AbyssGravel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Abyss Gravel");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.AbyssGravel>();
            item.useStyle = 1;
            item.useTurn = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.autoReuse = true;
            item.consumable = true;
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
        }
    }
}
