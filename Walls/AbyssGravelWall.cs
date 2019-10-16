using Terraria;
using Terraria.ModLoader; using CalamityMod.Buffs; using CalamityMod.Items; using CalamityMod.NPCs; using CalamityMod.Projectiles; using CalamityMod.Tiles; using CalamityMod.Walls;

namespace CalamityMod.Walls
{
    public class AbyssGravelWall : ModWall
    {
        public override void SetDefaults()
        {
            dustType = 33;
        }

        public override void RandomUpdate(int i, int j)
        {
            if (Main.tile[i, j].liquid <= 0 && j < Main.maxTilesY - 205)
            {
                Main.tile[i, j].liquid = 255;
                Main.tile[i, j].lava(false);
            }
        }

        public override void KillWall(int i, int j, ref bool fail)
        {
            fail = true;
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
