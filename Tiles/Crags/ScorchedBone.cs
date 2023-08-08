using CalamityMod.Tiles.Ores;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace CalamityMod.Tiles.Crags
{
    public class ScorchedBone : ModTile
    {
        private int sheetWidth = 450;
        private int sheetHeight = 198;

        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithHell(Type);
            CalamityUtils.SetMerge(Type, ModContent.TileType<BrimstoneSlag>());

            DustType = 155;
            HitSound = SoundID.Dig;
            MinPick = 100;
            AddMapEntry(new Color(87, 62, 67));

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<BrimstoneSlag>(), out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Ash, out secondTileAdjacency);
        }

        public override bool CanExplode(int i, int j)
        {
            return false;
        }

        public override void AnimateIndividualTile(int type, int i, int j, ref int frameXOffset, ref int frameYOffset)
        {
            frameXOffset = i % 3 * sheetWidth;
            frameYOffset = j % 3 * sheetHeight;
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
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
