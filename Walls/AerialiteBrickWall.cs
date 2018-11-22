using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AerialiteBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("AerialiteBrickWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Aerialite Brick Wall");
            AddMapEntry(new Color(0, 255, 255), name);
            dustType = mod.DustType("AHSparkle");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}