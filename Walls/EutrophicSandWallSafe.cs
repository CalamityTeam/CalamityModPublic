using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class EutrophicSandWallSafe : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            dustType = 108;
            drop = mod.ItemType("EutrophicSandWallSafe");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Eutrophic Sand Wall Safe");
            AddMapEntry(new Color(100, 100, 150), name);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
