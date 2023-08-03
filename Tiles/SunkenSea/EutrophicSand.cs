using CalamityMod.Items.Placeables;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.SunkenSea
{
    public class EutrophicSand : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;

        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type); // Tile blends with sandstone, which it is set to merge with here

            Main.tileShine[Type] = 1800;
            Main.tileShine2[Type] = false;

            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;

            DustType = 108;
            AddMapEntry(new Color(92, 145, 167));

            TileFraming.SetUpUniversalMerge(Type, TileID.Sandstone, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.HardenedSand, out thirdTileAdjacency);
        }

        public override void RandomUpdate(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            Tile up = Main.tile[i, j - 1];
            Tile up2 = Main.tile[i, j - 2];

            // Place SmallCorals
            if (WorldGen.genRand.NextBool(8) && !up.HasTile && !up2.HasTile && up.LiquidAmount > 0 && up2.LiquidAmount > 0 && !tile.LeftSlope && !tile.RightSlope && !tile.IsHalfBlock)
            {
                up.TileType = (ushort)ModContent.TileType<SmallCorals>();
                up.HasTile = true;
                up.TileFrameY = 0;

                // 6 different frames, choose a random one
                up.TileFrameX = (short)(WorldGen.genRand.Next(6) * 18);
                WorldGen.SquareTileFrame(i, j - 1, true);

                if (Main.netMode == NetmodeID.Server)
                    NetMessage.SendTileSquare(-1, i, j - 1, 3, TileChangeType.None);
            }
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/SandstoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/SandMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/HardenedSandMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, TileID.Sandstone, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.HardenedSand, out thirdTileAdjacency[i, j]);
            return TileFraming.BrimstoneFraming(i, j, resetFrame);
        }
    }
}
