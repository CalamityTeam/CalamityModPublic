using CalamityMod.Backgrounds;
using CalamityMod.Systems;
using CalamityMod.Waters;
using CalamityMod.World;
using Terraria;
using Terraria.Graphics.Capture;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbovegroundAstralBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralSurfaceBGStyle");

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("Astral") ?? MusicID.Space;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Surface");
        }

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && (BiomeTileCounterSystem.AstralTiles > 950 || (player.ZoneSnow && BiomeTileCounterSystem.AstralTiles > 300)) && 
                !player.ZoneDesert && !player.ZoneSnow;
        }
    }
}
