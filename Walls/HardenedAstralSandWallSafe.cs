using Terraria;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class HardenedAstralSandWallSafe : ModWall
    {

        public override void SetStaticDefaults()
        {
            // TODO -- Change this dust to be one more befitting Hardened Astral Sand.
            dustType = DustID.Shadowflame;
            drop = ModContent.ItemType<Items.Placeables.Walls.HardenedAstralSandWall>();
            Main.wallHouse[Type] = true;

            WallID.Sets.Conversion.HardenedSand[Type] = true;

            AddMapEntry(new Color(10, 9, 21));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
