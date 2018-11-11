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
    public class AstralSandstoneWall : ModWall
    {
        public override bool Autoload(ref string name, ref string texture)
        {
            mod.AddWall("AstralSandstoneWallUnsafe", this, texture);
            return base.Autoload(ref name, ref texture);
        }

        public override void SetDefaults()
        {
            dustType = DustID.Shadowflame; //TODO
            drop = mod.ItemType("AstralSandstoneWall");

            WallID.Sets.Conversion.Sandstone[Type] = true;

            AddMapEntry(new Color(29, 38, 49));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
    }
}
