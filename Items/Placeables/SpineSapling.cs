using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables
{
    public class SpineSapling : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
            DisplayName.SetDefault("Small Spine");
            Tooltip.SetDefault("Plants Giant Spine saplings on Brimstone Slag");
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<Tiles.Crags.Tree.SpineSapling>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
        }
    }
}
