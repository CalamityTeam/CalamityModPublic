using CalamityMod.Tiles.Astral;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.Metadata;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralSnow
{
    public class AstralSnow : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;
            Main.tileBrick[Type] = true;
			TileMaterials.SetForTileId(Type, TileMaterials._materialsByName["Snow"]);

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithSnow(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = 173;

            HitSound = SoundID.Item48;

            AddMapEntry(new Color(189, 211, 221));

            TileID.Sets.Snow[Type] = true;
            TileID.Sets.ChecksForMerge[Type] = true;
            TileID.Sets.CanBeClearedDuringOreRunner[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralDirt>(), out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, TileID.SnowBlock, out secondTileAdjacency);
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/AstralDirtMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/SnowMerge");
        }

        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralDirt>(), out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, TileID.SnowBlock, out secondTileAdjacency[i, j]);
            return true;
        }

        public override bool IsTileBiomeSightable(int i, int j, ref Color sightColor)
        {
            sightColor = Color.Cyan;
            return true;
        }
    }
}
