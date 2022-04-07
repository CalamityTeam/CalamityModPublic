
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralIceWall : ModWall
    {
        /*public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("AstralIceWallUnsafe", this, texture);
            return base.Autoload(ref name, ref texture);
        }*/

        public override void SetStaticDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Ice.
            DustType = DustID.Shadowflame;
            AddMapEntry(new Color(83, 76, 92));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
