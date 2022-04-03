
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class HardenedAstralSandWall : ModWall
    {
        /*public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("HardenedAstralSandWallUnsafe", this, texture);
            return base.Autoload(ref name, ref texture);
        }*/

        public override void SetStaticDefaults()
        {
            // TODO -- Change this dust to be one more befitting Hardened Astral Sand.
            dustType = DustID.Shadowflame;

            WallID.Sets.Conversion.HardenedSand[Type] = true;

            AddMapEntry(new Color(10, 9, 21));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
