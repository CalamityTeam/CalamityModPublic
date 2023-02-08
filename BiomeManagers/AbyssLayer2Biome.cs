using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbyssLayer2Biome : ModBiome
    {
        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return CalamityMod.Instance.GetMusicFromMusicMod("Abyss1") ?? MusicID.Hell;
            }
        }

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/UpperAbyssWater");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssLayer2Icon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Upper Abyss");
        }

        public override bool IsBiomeActive(Player player)
        {
            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) &&
            playerYTileCoords > Main.rockLayer - 22 &&
            playerYTileCoords <= Main.rockLayer + Main.maxTilesY * 0.145;
        }
    }
}
