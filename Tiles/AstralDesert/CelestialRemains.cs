using CalamityMod.Dusts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace CalamityMod.Tiles.AstralDesert
{
    [LegacyName("AstralFossil")]
    public class CelestialRemains : ModTile
    {
        public byte[,] tileAdjacency;
        public byte[,] secondTileAdjacency;
        public override void SetStaticDefaults()
        {
            Main.tileSolid[Type] = true;
            Main.tileBlockLight[Type] = true;

            CalamityUtils.MergeWithGeneral(Type);
            CalamityUtils.MergeWithDesert(Type);
            CalamityUtils.MergeAstralTiles(Type);

            DustType = ModContent.DustType<AstralBasic>();

            AddMapEntry(new Color(59, 50, 77), CalamityUtils.GetItemName<Items.Placeables.CelestialRemains>());

            TileID.Sets.ForAdvancedCollision.ForSandshark[Type] = true;

            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralSand>(), out tileAdjacency);
            TileFraming.SetUpUniversalMerge(Type, ModContent.TileType<AstralSandstone>(), out secondTileAdjacency);
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            TileFraming.DrawUniversalMergeFrames(i, j, secondTileAdjacency, "CalamityMod/Tiles/Merges/AstralSandstoneMerge");
            TileFraming.DrawUniversalMergeFrames(i, j, tileAdjacency, "CalamityMod/Tiles/Merges/AstralSandMerge");
        }
        public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
        {
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralSand>(), out tileAdjacency[i, j]);
            TileFraming.GetAdjacencyData(i, j, ModContent.TileType<AstralSandstone>(), out secondTileAdjacency[i, j]);
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
