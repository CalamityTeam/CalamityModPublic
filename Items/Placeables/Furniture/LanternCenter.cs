using CalamityMod.Tiles.Furniture;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class LanternCenter : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Lantern Center");
            Tooltip.SetDefault("Lights up the night sky with celebratory lanterns");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = Item.buyPrice(0, 20, 0, 0); //Same price as Party Center
            Item.rare = ItemRarityID.Orange;
            Item.createTile = ModContent.TileType<LanternCenterTile>();
        }
    }
}
