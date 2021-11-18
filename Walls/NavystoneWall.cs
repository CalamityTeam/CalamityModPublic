using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class NavystoneWall : ModWall
    {
        public override void SetDefaults()
        {
            dustType = 96;
            AddMapEntry(new Color(0, 50, 50));
        }

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
