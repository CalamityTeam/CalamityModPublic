using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class SmoothBrimstoneSlagWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("SmoothBrimstoneSlagWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Smooth Brimstone Slag Wall");
            AddMapEntry(new Color(20, 20, 20), name);
            dustType = 53;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}