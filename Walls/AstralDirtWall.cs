using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AstralDirtWall : ModWall
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("AstralDirtWallUnsafe", this, texture);
            return base.Autoload(ref name, ref texture);
        }

        public override void SetDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Dirt.
            dustType = DustID.Shadowflame;
            drop = ModContent.ItemType<Items.Placeables.Walls.AstralDirtWall>();
            AddMapEntry(new Color(26, 22, 32));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
