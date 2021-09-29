using CalamityMod.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Furniture.Trophies
{
    public class AresTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ares Trophy");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 32;
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
            item.placeStyle = 30;
        }
    }
}
