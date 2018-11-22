using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class CryonicBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("CryonicBrickWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Cryonic Brick Wall");
            AddMapEntry(new Color(0, 0, 150), name);
            dustType = mod.DustType("MSparkle");
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}