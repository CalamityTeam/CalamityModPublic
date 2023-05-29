using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Systems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbovegroundAstralBiomeSurface : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralSurfaceBGStyle");
        public override int BiomeTorchItemType => ModContent.ItemType<AstralTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("AstralInfection") ?? MusicID.Space;

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950 && !player.ZoneSnow && !player.ZoneDesert;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            if (SkyManager.Instance["CalamityMod:AstralSurface"] != null && isActive != SkyManager.Instance["CalamityMod:AstralSurface"].IsActive())
            {
                if (isActive)
                {
                    SkyManager.Instance.Activate("CalamityMod:AstralSurface");
                }
                else
                {
                    SkyManager.Instance.Deactivate("CalamityMod:AstralSurface");
                }
            }
        }

    }
}
