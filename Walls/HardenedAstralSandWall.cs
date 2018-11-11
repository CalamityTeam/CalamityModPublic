using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Walls
{
    public class HardenedAstralSandWall : ModWall
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("HardenedAstralSandWallUnsafe", this, texture);
            return base.Autoload(ref name, ref texture);
        }

        public override void SetDefaults()
        {
            dustType = DustID.Shadowflame; //TODO
            drop = mod.ItemType("HardenedAstralSandWall");

            WallID.Sets.Conversion.HardenedSand[Type] = true;
            
            AddMapEntry(new Color(10, 9, 21));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
