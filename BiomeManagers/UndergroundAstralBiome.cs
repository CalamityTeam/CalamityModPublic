using CalamityMod.Backgrounds;
using CalamityMod.Systems;
using CalamityMod.Waters;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class UndergroundAstralBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/AstralUndergroundBGStyle");

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("AstralUnderground") ?? MusicID.Space;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Astral Underground");
        }

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && (BiomeTileCounterSystem.AstralTiles > 950 || (player.ZoneSnow && BiomeTileCounterSystem.AstralTiles > 300)) && 
                (player.ZoneDirtLayerHeight || player.ZoneRockLayerHeight || player.ZoneUnderworldHeight);
        }
    }
}
