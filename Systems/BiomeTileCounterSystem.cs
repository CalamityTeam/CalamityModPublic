using System;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Systems
{
    public class BiomeTileCounterSystem : ModSystem
    {
        public static int BrimstoneCragTiles = 0;
        public static int SulphurTiles = 0;
        public static int AbyssTiles = 0;
        public static int AstralTiles = 0;
        public static int SunkenSeaTiles = 0;
        public static int ArsenalLabTiles = 0;

        public override void ResetNearbyTileEffects()
        {
            BrimstoneCragTiles = 0;
            AstralTiles = 0;
            SunkenSeaTiles = 0;
            SulphurTiles = 0;
            AbyssTiles = 0;
            ArsenalLabTiles = 0;
        }

        public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
        {
            BrimstoneCragTiles = tileCounts[ModContent.TileType<CharredOre>()] + tileCounts[ModContent.TileType<BrimstoneSlag>()];
            SunkenSeaTiles = tileCounts[ModContent.TileType<EutrophicSand>()] + tileCounts[ModContent.TileType<Navystone>()] + tileCounts[ModContent.TileType<SeaPrism>()];
            AbyssTiles = tileCounts[ModContent.TileType<AbyssGravel>()] + tileCounts[ModContent.TileType<Voidstone>()];
            SulphurTiles = tileCounts[ModContent.TileType<SulphurousSand>()] + tileCounts[ModContent.TileType<SulphurousSandstone>()] + tileCounts[ModContent.TileType<HardenedSulphurousSandstone>()];
            ArsenalLabTiles =  tileCounts[ModContent.TileType<LaboratoryPanels>()] + tileCounts[ModContent.TileType<LaboratoryPlating>()] + tileCounts[ModContent.TileType<HazardChevronPanels>()];

            int astralDesertTiles = tileCounts[ModContent.TileType<AstralSand>()] + tileCounts[ModContent.TileType<AstralSandstone>()] + tileCounts[ModContent.TileType<HardenedAstralSand>()] + tileCounts[ModContent.TileType<CelestialRemains>()];
            int astralSnowTiles = tileCounts[ModContent.TileType<AstralIce>()] + tileCounts[ModContent.TileType<AstralSnow>()];

            Main.SceneMetrics.SandTileCount += astralDesertTiles;
            Main.SceneMetrics.SnowTileCount += astralSnowTiles;

            AstralTiles = astralDesertTiles + astralSnowTiles + tileCounts[ModContent.TileType<AstralDirt>()] + tileCounts[ModContent.TileType<AstralStone>()] + tileCounts[ModContent.TileType<AstralGrass>()] + tileCounts[ModContent.TileType<AstralOre>()] + tileCounts[ModContent.TileType<NovaeSlag>()] + tileCounts[ModContent.TileType<AstralClay>()];
        }
    }
}
