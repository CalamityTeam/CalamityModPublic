using CalamityMod.CalPlayer;
using CalamityMod.Items.Placeables.FurnitureAbyss;
using CalamityMod.World;
using CalamityMod.Systems;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbyssLayer1Biome : ModBiome
    {
        // Keep this here even though layer one now uses a tile check, cannot be bothered to move it for now
        public static bool MeetsBaseAbyssRequirement(Player player, out int playerYTileCoords)
        {
            Point point = player.Center.ToTileCoordinates();
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int abyssChasmX = Abyss.AtLeftSideOfWorld ? genLimit - (genLimit - 135) + 35 : genLimit + (genLimit - 135) - 35;

            bool abyssPosX = false;
            if (Abyss.AtLeftSideOfWorld)
            {
                if (point.X < abyssChasmX + 140)
                    abyssPosX = true;
            }
            else
            {
                if (point.X > abyssChasmX - 140)
                    abyssPosX = true;
            }

            playerYTileCoords = point.Y;

            if (WeakReferenceSupport.InAnySubworld())
                return false;

            int abyssStartHeight = Main.remixWorld ? SulphurousSea.YStart : ((SulphurousSea.YStart + (int)Main.worldSurface) / 2 + 90);

            if (Main.remixWorld)
                return !player.lavaWet && !player.honeyWet && abyssPosX && playerYTileCoords < abyssStartHeight;

            return !player.lavaWet && !player.honeyWet && abyssPosX && playerYTileCoords >= abyssStartHeight && playerYTileCoords <= Main.maxTilesY - 200;
        }

        // Temporarily use sulphur for now
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/SulphuricDepthsWater");
        public override int BiomeTorchItemType => ModContent.ItemType<AbyssTorch>();
        public override SceneEffectPriority Priority => SceneEffectPriority.Environment;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssLayer1Icon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";

        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return CalamityMod.Instance.GetMusicFromMusicMod("AbyssLayer1") ?? MusicID.Desert;
            }
        }

        public override bool IsBiomeActive(Player player)
        {
            Point point = player.Center.ToTileCoordinates();

            int abyssStartHeight = Main.remixWorld ? SulphurousSea.YStart : ((SulphurousSea.YStart + (int)Main.worldSurface) / 2 + 90);

            if (Main.remixWorld)
            {
                return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && point.Y < abyssStartHeight &&
                BiomeTileCounterSystem.Layer1Tiles >= 200 && !player.Calamity().ZoneAbyssLayer2 && !player.Calamity().ZoneAbyssLayer3 && !player.Calamity().ZoneAbyssLayer4;
            }

            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords2) && point.Y >= abyssStartHeight &&
            BiomeTileCounterSystem.Layer1Tiles >= 200 && !player.Calamity().ZoneAbyssLayer2 && !player.Calamity().ZoneAbyssLayer3 && !player.Calamity().ZoneAbyssLayer4;
        }
    }
}
