using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.Walls
{
    public class BlueBrickWallUnsafe : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 400;
        }

        public override void SetDefaults()
        {
            Item.width = 12;
            Item.height = 12;
            Item.maxStack = 999;
            Item.useTurn = true;
            Item.autoReuse = true;
            Item.useAnimation = 15;
            Item.useTime = 7;
            Item.useStyle = ItemUseStyleID.Swing;
            Item.consumable = true;
            Item.createWall = WallID.BlueDungeonUnsafe;
        }
    }
}
