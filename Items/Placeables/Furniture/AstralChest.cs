using CalamityMod.Tiles.Astral;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Furniture
{
    public class AstralChest : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 22;
            Item.maxStack = 99;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.value = 500;
            Item.createTile = ModContent.TileType<AstralChestLocked>();
        }
    }
}
