using CalamityMod.Backgrounds;
using CalamityMod.CalPlayer;
using CalamityMod.Systems;
using CalamityMod.Waters;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class SulphurousSeaBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<SulphuricWater>("CalamityMod/Waters/SulphuricWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<SulphurSeaSurfaceBGStyle>("CalamityMod/Backgrounds/SulphurSeaSurfaceBGStyle");

        public override int Music
        {
            get
            {
                int music = Main.curMusic;
                if (!CalamityPlayer.areThereAnyDamnBosses)
                {
                    bool acidRain = CalamityWorld.rainingAcid;

                    // Acid Rain themes
                    if (acidRain)
                    {
                        music = DownedBossSystem.downedPolterghast
                            ? CalamityMod.Instance.GetMusicFromMusicMod("AcidRain2") ?? MusicID.Eclipse // Acid Rain Tier 3
                            : CalamityMod.Instance.GetMusicFromMusicMod("AcidRain1") ?? MusicID.OldOnesArmy; // Acid Rain Tier 1 + 2
                    }

                    // Regular Sulphur Sea theme, when Acid Rain is not occurring
                    else
                        music = CalamityMod.Instance.GetMusicFromMusicMod("SulphurousSea") ?? MusicID.Desert;
                }
                return music;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulpurous Sea");
        }

        public override bool IsBiomeActive(Player player)
        {
            Point point = player.Center.ToTileCoordinates();
            bool sulphurPosX = false;
            if (CalamityWorld.abyssSide)
            {
                if (point.X < 380)
                    sulphurPosX = true;
            }
            else
            {
                if (point.X > Main.maxTilesX - 380)
                    sulphurPosX = true;
            }
            return BiomeTileCounterSystem.SulphurTiles >= 300 || (player.ZoneOverworldHeight && sulphurPosX);
        }
    }
}
