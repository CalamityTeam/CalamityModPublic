using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class UndergroundAstralBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/AstralUndergroundBGStyle");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/UndergroundAstralIcon";
		// Could use its own unique background
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("AstralUnderground") ?? MusicID.Space;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Underground");
        }

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950 && (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight);
        }

        // Just slightly above the above-ground astral biomes.
        public override float GetWeight(Player player) => 0.51f;
    }
}
