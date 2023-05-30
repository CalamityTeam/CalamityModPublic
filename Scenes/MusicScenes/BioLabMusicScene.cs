using CalamityMod.Systems;
using CalamityMod.Walls;
using CalamityMod.Walls.DraedonStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Scenes.MusicScenes
{
    public class BioLabMusicScene : ModSceneEffect
    {
        public override int Music => CalamityMod.Instance.GetMusicFromMusicMod("BioLab") ?? -1;
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        // This weight is here to ensure this theme plays over most other songs that use the Environment priority, as it's a smaller area and should take higher precedent
        public override float GetWeight(Player player) => 0.6f;

        public override bool IsSceneEffectActive(Player player)
        {
            Tile backWall = Framing.GetTileSafely((int)(player.Center.X / 16), (int)(player.Center.Y / 16));
            Vector2 playerPosition = player.Center;

            float sunkenSeaLabDistance = Vector2.DistanceSquared(CalamityWorld.SunkenSeaLabCenter, playerPosition);
            float planetoidLabDistance = Vector2.DistanceSquared(CalamityWorld.PlanetoidLabCenter, playerPosition);
            float jungleLabDistance = Vector2.DistanceSquared(CalamityWorld.JungleLabCenter, playerPosition);
            float hellLabDistance = Vector2.DistanceSquared(CalamityWorld.HellLabCenter, playerPosition);
            float iceLabDistance = Vector2.DistanceSquared(CalamityWorld.IceLabCenter, playerPosition);
            float cavernLabDistance = Vector2.DistanceSquared(CalamityWorld.CavernLabCenter, playerPosition);

            // (Tile range * Pixels per tile)^2
            double labRadius = Math.Pow(80f * 16f, 2);

            // Checks if the player is behind any wall that naturally generates in Bio Labs
            bool behindLabWall =
                backWall.WallType == WallID.ObsidianBrick ||
                backWall.WallType == WallID.Glass ||
                backWall.WallType == WallID.SnowWallUnsafe ||
                backWall.WallType == WallID.IceUnsafe ||
                backWall.WallType == WallID.Waterfall ||
                backWall.WallType == WallID.Lavafall ||
                backWall.WallType == WallID.IronBrick ||
                backWall.WallType == ModContent.WallType<AstralIceWall>() ||
                backWall.WallType == ModContent.WallType<AstralSnowWall>() ||
                backWall.WallType == ModContent.WallType<HavocplateWall>() ||
                backWall.WallType == ModContent.WallType<CinderplateWall>() ||
                backWall.WallType == ModContent.WallType<ElumplateWall>() ||
                backWall.WallType == ModContent.WallType<HazardChevronWall>() ||
                backWall.WallType == ModContent.WallType<LaboratoryPanelWall>() ||
                backWall.WallType == ModContent.WallType<LaboratoryPlateBeam>() ||
                backWall.WallType == ModContent.WallType<LaboratoryPlatePillar>() ||
                backWall.WallType == ModContent.WallType<LaboratoryPlatingWall>() ||
                backWall.WallType == ModContent.WallType<NavyplateWall>() ||
                backWall.WallType == ModContent.WallType<PlagueContainmentCellsWall>() ||
                backWall.WallType == ModContent.WallType<PlaguedPlateWall>() ||
                backWall.WallType == ModContent.WallType<RustedPlatePillar>() ||
                backWall.WallType == ModContent.WallType<RustedPlatingWall>();

            // Checks if the player is within a specified range from the center point of any Bio Lab
            bool nearBioLabPoint =
                sunkenSeaLabDistance <= labRadius ||
                planetoidLabDistance <= labRadius ||
                jungleLabDistance <= labRadius ||
                hellLabDistance <= labRadius ||
                iceLabDistance <= labRadius ||
                cavernLabDistance <= labRadius;

            return BiomeTileCounterSystem.ArsenalLabTiles > 150 && behindLabWall && nearBioLabPoint;
        }
    }
}
