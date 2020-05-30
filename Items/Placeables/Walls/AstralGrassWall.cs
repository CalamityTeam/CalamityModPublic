using Terraria.ModLoader;
using WallTiles = CalamityMod.Walls;
using Terraria.ID;

namespace CalamityMod.Items.Placeables.Walls
{
    public class AstralGrassWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Grass Wall");
        }

        public override void SetDefaults()
        {
            item.createWall = ModContent.WallType<WallTiles.AstralGrassWall>();
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
