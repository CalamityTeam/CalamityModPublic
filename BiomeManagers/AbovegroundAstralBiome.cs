﻿using CalamityMod.Items.Placeables.Furniture;
using CalamityMod.Systems;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbovegroundAstralBiome : ModBiome
    {
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AstralWater");
        public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.Find<ModSurfaceBackgroundStyle>("CalamityMod/AstralSurfaceFogBGStyle");
        public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("CalamityMod/AstralUndergroundBGStyle");
        public override int BiomeTorchItemType => ModContent.ItemType<AstralTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbovegroundAstralBiomeIcon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AstralBG";

        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("AstralInfection") ?? MusicID.Space;

        public override bool IsBiomeActive(Player player)
        {
            return !player.ZoneDungeon && BiomeTileCounterSystem.AstralTiles > 950;
        }

        public override void SpecialVisuals(Player player, bool isActive)
        {
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", isActive);

        }
    }
}
