using CalamityMod.Tiles.Crags;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles.Ores
{
    [LegacyName("CharredOre")]
    public class InfernalSuevite : ModTile
    {
        private int sheetWidth = 234;
        private int sheetHeight = 90;

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileLighted[Type] = true;
            Main.tileOreFinderPriority[Type] = 675;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);

            TileID.Sets.Ore[Type] = true;

            AddMapEntry(new Color(17, 16, 26), CreateMapEntryName());
            MineResist = 2f;
            MinPick = 150;
            HitSound = SoundID.Tink;
            DustType = 235;
            Main.tileSpelunker[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<BrimstoneSlag>(), out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Ash, out secondTileAdjacency);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 2 * sheetWidth;
            frameYOffset = j % 2 * sheetHeight;
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.5f;
            g = 0f;
            b = 0f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/AshMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/BrimstoneSlagMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<BrimstoneSlag>(), out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Ash, out secondTileAdjacency[i, j]);
            return true;
        }
    }
}
