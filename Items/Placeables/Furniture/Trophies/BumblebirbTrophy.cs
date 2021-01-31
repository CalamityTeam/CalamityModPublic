using CalamityMod.Tiles.Furniture;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture.Trophies
{
    public class BumblebirbTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonfolly Trophy");
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
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.consumable = true;
            item.value = 50000;
            item.rare = ItemRarityID.Blue;
            item.createTile = ModContent.TileType<BossTrophy>();
            item.placeStyle = 18;
        }
    }
}
