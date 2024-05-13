using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Systems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AstralInfectionBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle
        {
            get
            {
                if (Main.LocalPlayer.ZoneSnow) //Snow
                {
                    return ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralSnowSurfaceBGStyle");
                }
                else if (Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow) //Desert
                {
                    return ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralDesertSurfaceBGStyle");
                }
                else //surface
                {
                    return ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralSurfaceBGStyle");
                }
            }
        }

        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle
        {
            get
            {
                if (Main.LocalPlayer.ZoneSnow)
                {
                    return ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/AstralUndergroundBGStyle"); // Could use its own unique background
                }
                return ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/AstralUndergroundBGStyle");
            }
        }

        public override int BiomeTorchItemType => ModContent.ItemType<AstralTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbovegroundAstralBiomeIcon";
        public override string BackgroundPath 
        { 
            get {
                if (Main.LocalPlayer.ZoneDesert && !Main.LocalPlayer.ZoneSnow)
                {
                    //desert
                    return "CalamityMod/Backgrounds/MapBackgrounds/AstralBG"; // Could use its own unique background
                }
                else if (Main.LocalPlayer.ZoneDirtLayerHeight || Main.LocalPlayer.ZoneRockLayerHeight || Main.LocalPlayer.ZoneUnderworldHeight)
                {
                    //underground
                    return "CalamityMod/Backgrounds/MapBackgrounds/AstralBG"; // Could use its own unique background
                }
                else
                {
                    return "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";
                }
            } 
        }
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";

        public override int Music
        {
            get
            {
                if (Main.LocalPlayer.ZoneDirtLayerHeight || Main.LocalPlayer.ZoneRockLayerHeight || Main.LocalPlayer.ZoneUnderworldHeight)
                {
                    return CalamityMod.Instance.GetMusicFromMusicMod("AstralInfectionUnderground") ?? MusicID.Space;
                }
                return CalamityMod.Instance.GetMusicFromMusicMod("AstralInfection") ?? MusicID.Space;
            }
        }

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", isActive);
            if (Main.LocalPlayer.ZoneDirtLayerHeight || Main.LocalPlayer.ZoneRockLayerHeight || Main.LocalPlayer.ZoneUnderworldHeight)
            {
                player.ManageSpecialBiomeVisuals("CalamityMod:Astral", isActive); //underground
            }
        }
    }
}
