using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class AstralBrickWall : ModWall
    {
        public override void SetDefaults()
        {
            Main.wallHouse[Type] = true;
            drop = mod.ItemType("AstralBrickWall");
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Astral Brick Wall");
            AddMapEntry(new Color(255, 153, 255), name);
            dustType = 173;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 2;
        }
    }
}