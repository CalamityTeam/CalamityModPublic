using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class SunkenSeaBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/SunkenSeaWater");
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/SunkenSeaBGStyle");
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("SunkenSea") ?? MusicID.OceanNight;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/SunkenSeaIcon";
		// Placeholder until we get a dedicated Sunken Sea background
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunken Sea");
        }

        public override bool IsBiomeActive(Player player)
        {
            return BiomeTileCounterSystem.SunkenSeaTiles > 150;
        }
    }
}
