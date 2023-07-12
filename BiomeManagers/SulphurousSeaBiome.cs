using System.Security.Policy;
using CalamityMod.CalPlayer;
using CalamityMod.Events;
using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Systems;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class SulphurousSeaBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => Main.zenithWorld ? ModContent.Find<ModWaterStyle>("CalamityMod/PissWater") : ModContent.Find<ModWaterStyle>("CalamityMod/SulphuricWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => Main.zenithWorld ?ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/PissSeaSurfaceBGStyle") : ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/SulphurSeaSurfaceBGStyle");
        public override int BiomeTorchItemType => ModContent.ItemType<SulphurousTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/SulphurousSeaIcon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/SulphurBG";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/SulphurBG";

        public override int Music
        {
            get
            {
                int music = Main.curMusic;
                if (!CalamityPlayer.areThereAnyDamnBosses)
                {
                    bool acidRain = AcidRainEvent.AcidRainEventIsOngoing;

                    // Acid Rain themes
                    if (acidRain)
                    {
                        music = DownedBossSystem.downedPolterghast
                        ? CalamityMod.Instance.GetMusicFromMusicMod("AcidRainTier3") ?? MusicID.Monsoon // Acid Rain Tier 3
                        : CalamityMod.Instance.GetMusicFromMusicMod("AcidRainTier1") ?? MusicID.OldOnesArmy; // Acid Rain Tier 1 + 2
                    }

                    // Regular Sulphur Sea themes, when Acid Rain is not occurring
                    else
                        music = !Main.dayTime
                        ? CalamityMod.Instance.GetMusicFromMusicMod("SulphurousSeaNight") ?? MusicID.Desert // Nighttime
                        : CalamityMod.Instance.GetMusicFromMusicMod("SulphurousSeaDay") ?? MusicID.Desert; // Daytime
                }
                
                return music;
            }
        }

        public override bool IsBiomeActive(Player player)
        {
            Point point = player.Center.ToTileCoordinates();
            bool sulphurPosX = false;

            if (Abyss.AtLeftSideOfWorld)
            {
                if (point.X < 435)
                {
                    sulphurPosX = true;
                }
            }
            else
            {
                if (point.X > Main.maxTilesX - 435)
                {
                    sulphurPosX = true;
                }
            }
            
            if (Main.remixWorld)
                return (BiomeTileCounterSystem.SulphurTiles >= 300 || (point.Y > SulphurousSea.YStart && point.Y < Main.maxTilesY - 200 && sulphurPosX)) && !player.Calamity().ZoneAbyss;

            return (BiomeTileCounterSystem.SulphurTiles >= 300 || (point.Y < (Main.rockLayer - Main.maxTilesY / 13) && sulphurPosX)) && !player.Calamity().ZoneAbyss;
        }
        
        public override void SpecialVisuals(Player player, bool isActive)
        {
            string biomeName = "CalamityMod:SulphurSea";
            if (SkyManager.Instance[biomeName] != null && isActive != SkyManager.Instance[biomeName].IsActive())
            {
                if (isActive)
                {
                    SkyManager.Instance.Activate(biomeName);
                }
                else
                {
                    SkyManager.Instance.Deactivate(biomeName);
                }
            }
        }
    }
}
