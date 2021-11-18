using Terraria.ModLoader;
namespace CalamityMod.Walls
{
    public class MossyGravelWall : ModWall
    {
        public override void SetDefaults()
        {
            dustType = 2;
        }

        public override void KillWall(int i, int j, ref bool fail) => fail = true;

        public override bool CanExplode(int i, int j) => false;

        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
    }
}
