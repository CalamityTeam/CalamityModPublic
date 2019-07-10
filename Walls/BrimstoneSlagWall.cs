using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class BrimstoneSlagWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("BrimstoneSlagWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Brimstone Slag Wall");
            AddMapEntry(new Color(20, 20, 20), name);
            dustType = 53;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
