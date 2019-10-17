using Terraria.ModLoader;
using CalamityMod.Tiles;
namespace CalamityMod.Items
{
    public class DevourerofGodsTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Devourer of Gods Trophy");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 30;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 50000;
            item.rare = 1;
            item.createTile = ModContent.TileType<BossTrophy>();
            item.placeStyle = 14;
        }
    }
}
