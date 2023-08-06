using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.Ores
{
    public class HallowedOre : ModTile
    {
        public byte[,] tileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileSolid[Type] = true;
            Main.tileMergeDirt[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileOreFinderPriority[Type] = 690;

            TileID.Sets.Ore[Type] = true;
            TileID.Sets.OreMergesWithMud[Type] = true;

            Main.tileShine[Type] = 2000;
            Main.tileShine2[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            AddMapEntry(new Color(250, 250, 150), CreateMapEntryName());
            MineResist = 2f;
            MinPick = 180;
            HitSound = SoundID.Tink;
            Main.tileSpelunker[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, TileID.Pearlstone, out tileAdjacency);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/PearlstoneMerge");
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Pearlstone, out tileAdjacency[i, j]);
            return true;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 224f / 600f;
            g = 219f / 600f;
            b = 124f / 600f;
        }
    }
}
