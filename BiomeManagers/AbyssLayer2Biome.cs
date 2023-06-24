using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.Systems;
using CalamityMod.World;
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
                return CalamityMod.Instance.GetMusicFromMusicMod("AbyssLayer2") ?? MusicID.Hell;
            }
        }

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/UpperAbyssWater");
        public override int BiomeTorchItemType => ModContent.ItemType<AbyssTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssLayer2Icon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer23";

        public override bool IsBiomeActive(Player player)
        {
            if (Main.remixWorld)
            {
                return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && BiomeTileCounterSystem.Layer2Tiles >= 200 &&
                playerYTileCoords <= SulphurousSea.YStart - (int)((Main.maxTilesY - 200) * 0.2f) && playerYTileCoords > SulphurousSea.YStart - (int)((Main.maxTilesY - 200) * 0.4f);
            }

            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords2) && BiomeTileCounterSystem.Layer2Tiles >= 200 && 
            playerYTileCoords2 > Main.rockLayer - 10 && playerYTileCoords2 <= Main.rockLayer + Main.maxTilesY * 0.143;
        }
    }
}
