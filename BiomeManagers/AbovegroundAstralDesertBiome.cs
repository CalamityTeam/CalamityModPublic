using CalamityMod.Backgrounds;
using CalamityMod.Systems;
using CalamityMod.Waters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbovegroundAstralDesertBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<AstralWater>("CalamityMod/Waters/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<AstralDesertSurfaceBGStyle>("CalamityMod/Backgrounds/AstralDesertSurfaceBGStyle");

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("Astral") ?? MusicID.Space;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Desert Surface");
        }

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && (BiomeTileCounterSystem.AstralTiles > 950 || (player.ZoneSnow && BiomeTileCounterSystem.AstralTiles > 300))
                && player.ZoneDesert && !player.ZoneSnow;
        }
    }
}
