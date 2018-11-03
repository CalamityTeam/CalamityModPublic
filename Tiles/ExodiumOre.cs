using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles
{
    public class ExodiumOre : ModTile
    {
        public override void SetDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Exodium Ore");
            AddMapEntry(new Color(51, 48, 68), name);

            minPick = 0;
            Main.tileValue[Type] = 450;
            Main.tileSpelunker[Type] = true;

            drop = mod.ItemType("ExodiumClusterOre");
            base.SetDefaults();
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 2 : 4;
        }
    }
}
