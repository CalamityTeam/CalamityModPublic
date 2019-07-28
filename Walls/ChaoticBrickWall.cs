using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class ChaoticBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("ChaoticBrickWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Chaotic Brick Wall");
            AddMapEntry(new Color(255, 0, 0), name);
            dustType = 105;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}
