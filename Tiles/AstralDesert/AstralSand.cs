using CalamityMod.Projectiles.Typeless;
using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    public class AstralSand : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public byte[,] thirdTileAdjacency;
        public byte[,] fourthTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileSand[Type] = true;
            Main.tileBrick[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Sand"]);

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = 108;

            AddMapEntry(new Color(187, 220, 237));

            TileID.Sets.Suffocate[Type] = true;
            TileID.Sets.CanBeDugByShovel[Type] = true;
            TileID.Sets.Conversion.Sand[Type] = true;
            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;
            TileID.Sets.Falling[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, TileID.Dirt, out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Stone, out secondTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralDirt>(), out thirdTileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.Sand, out fourthTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, fourthTileAdjacency, "CalamityMod/Tiles/Merges/SandMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, thirdTileAdjacency, "CalamityMod/Tiles/Merges/AstralDirtMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/StoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/DirtMerge");
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            if (j < Main.maxTilesY)
            {
                // tile[i, j+1] can still be null if it's on the edge of a chunk
                if (!Main.tile[i, j + 1].HasTile)
                {
                    Main.tile[i, j].Get<TileWallWireStateData>().HasTile = false;
                    Projectile.NewProjectile(new EntitySource_TileBreak(i, j), new Vector2(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<AstralSandBallFalling>(), 15, 0f);
                    WorldGen.SquareTileFrame(i, j);
                    return false;
                }
            }
            TileFraming.GetAdjacencyData(i, j, TileID.Dirt, out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Stone, out secondTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralDirt>(), out thirdTileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.Sand, out fourthTileAdjacency[i, j]);
            return true;
        }

        public override bool HasWalkDust()
        {
            return Main.rand.NextBool(3);
        }

        public override void WalkDust(ref int dustType, ref bool makeDust, ref Color color)
        {
            DustType = 108;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
