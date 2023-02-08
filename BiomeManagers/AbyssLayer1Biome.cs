using CalamityMod.CalPlayer;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.BiomeManagers
{
    public class AbyssLayer1Biome : ModBiome
    {
        public static bool MeetsBaseAbyssRequirement(Player player, out int playerYTileCoords)
        {
            Point point = player.Center.ToTileCoordinates();
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int abyssChasmY = y - 250;
            int abyssChasmX = Abyss.AtLeftSideOfWorld ? genLimit - (genLimit - 135) + 35 : genLimit + (genLimit - 135) - 35;

            bool abyssPosX = false;
            bool abyssPosY = point.Y >= (Main.rockLayer - Main.maxTilesY / 13) && point.Y <= Main.maxTilesY - 200;
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

            return !player.lavaWet && !player.honeyWet && abyssPosY && abyssPosX;
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
            return MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && 
            playerYTileCoords >= Main.rockLayer - Main.maxTilesY / 15 &&
            playerYTileCoords <= Main.rockLayer - 22;
        }
    }
}
