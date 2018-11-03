using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class UelibloomBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("UelibloomBrickWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Uelibloom Brick Wall");
            AddMapEntry(new Color(0, 255, 0), name);
            dustType = mod.DustType("TCESparkle");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}