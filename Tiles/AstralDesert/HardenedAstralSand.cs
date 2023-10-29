
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class HardenedAstralSand : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;
        public byte[,] fifthTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.MergeAstralTiles(Type);


            DustType = 108;

            AddMapEntry(new Color(128, 128, 158));

            TileID.Sets.Conversion.HardenedSand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralSand>(), out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralSandstone>(), out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sandstone, out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.HardenedSand, out fourthTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, out fifthTileAdjacency);
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/AstralSandstoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/AstralSandMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/SandstoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, fourthTileAdjacency, "CalamityMod/Tiles/Merges/HardenedSandMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, fifthTileAdjacency, "CalamityMod/Tiles/Merges/SandMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralSand>(), out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralSandstone>(), out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Sandstone, out thirdTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.HardenedSand, out fourthTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, out fifthTileAdjacency[i, j]);
            return true;
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
