using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Systems;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbyssLayer3Biome : ModBiome
    {
        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return CalamityMod.Instance.GetMusicFromMusicMod("AbyssLayer3") ?? MusicID.Hell;
            }
        }

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/MiddleAbyssWater");
        public override int BiomeTorchItemType => ModContent.ItemType<AbyssTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssLayer3Icon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";

        public override bool IsBiomeActive(Player player)
        {
            if (Main.remixWorld)
            {
                return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && BiomeTileCounterSystem.Layer3Tiles >= 200 &&
                playerYTileCoords <= SulphurousSea.YStart - (int)((Main.maxTilesY - 200) * 0.4f) && playerYTileCoords > SulphurousSea.YStart - (int)((Main.maxTilesY - 200) * 0.6f);
            }

            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords2) && BiomeTileCounterSystem.Layer3Tiles >= 200 &&
            playerYTileCoords2 > Main.rockLayer + Main.maxTilesY * 0.143 && playerYTileCoords2 <= Main.rockLayer + Main.maxTilesY * 0.268;
        }
    }
}
