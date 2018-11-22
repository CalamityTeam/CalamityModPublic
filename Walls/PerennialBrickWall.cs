using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class PerennialBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("PerennialBrickWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Perennial Brick Wall");
            AddMapEntry(new Color(200, 250, 100), name);
            dustType = mod.DustType("CESparkle");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}