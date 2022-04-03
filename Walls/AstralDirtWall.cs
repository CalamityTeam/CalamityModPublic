using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AstralDirtWall : ModWall
    {
        /*public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("AstralDirtWallUnsafe",this, "CalamityMod/Walls/AstralD" );
            return true;
        }*/

        public override void SetStaticDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Dirt.
            dustType = DustID.Shadowflame;
            AddMapEntry(new Color(26, 22, 32));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
