using CalamityMod.CalPlayer;
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
        //keep this here even though layer one now uses a tile check, cannot be bothered to move it for now
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

            return !player.lavaWet && !player.honeyWet && abyssPosX && playerYTileCoords <= Main.maxTilesY - 200;
        }

        //temporarily use sulphur for now
        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/SulphuricDepthsWater");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssLayer1Icon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";
        public override string MapBackground => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";

        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return !Main.dayTime
                ? CalamityMod.Instance.GetMusicFromMusicMod("SulphurousSeaNight") ?? MusicID.Desert // Nighttime
                : CalamityMod.Instance.GetMusicFromMusicMod("SulphurousSeaDay") ?? MusicID.Desert; // Daytime
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Depths");
        }

        public override bool IsBiomeActive(Player player)
        {
            Point point = player.Center.ToTileCoordinates();

            int abyssStartHeight = (SulphurousSea.YStart + (int)Main.worldSurface) / 2 + 90;

            return AbyssLayer1Biome.MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && point.Y >= abyssStartHeight &&
            !player.Calamity().ZoneAbyssLayer2 && !player.Calamity().ZoneAbyssLayer3 && !player.Calamity().ZoneAbyssLayer4;
        }
    }
}
