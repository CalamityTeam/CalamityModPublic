
using Microsoft.Xna.Framework;
using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class AstralGrassWall : ModWall
    {
        /*public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("AstralGrassWallUnsafe", this, texture);
            return base.Autoload(ref name, ref texture);
        }*/

        public override void SetStaticDefaults()
        {
            // TODO -- Change this dust to be one more befitting Astral Grass.
            DustType = DustID.Shadowflame;
            ItemDrop = ModContent.ItemType<Items.Placeables.Walls.AstralGrassWall>();

            WallID.Sets.Conversion.Grass[Type] = true;

            AddMapEntry(new Color(60, 48, 64));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
