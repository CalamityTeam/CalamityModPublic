using CalamityMod.Systems;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class SunkenSeaBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/SunkenSeaWater");
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("SunkenSea") ?? MusicID.Temple;
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/SunkenSeaIcon";

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
