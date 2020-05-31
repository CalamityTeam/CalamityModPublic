using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class AstralClay : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Clay");
        }

        public override void SetDefaults()
        {
            item.createTile = ModContent.TileType<Tiles.Astral.AstralClay>();
            item.useStyle = ItemUseStyleID.SwingThrow;
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
