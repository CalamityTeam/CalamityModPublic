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
            int abyssChasmX = Abyss.AtLeftSideOfWorld ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135);

            bool abyssPosX = false;
            bool abyssPosY = point.Y <= abyssChasmY;
            if (Abyss.AtLeftSideOfWorld)
            {
                if (point.X < abyssChasmX + 80)
                    abyssPosX = true;
            }
            else
            {
                if (point.X > abyssChasmX - 80)
                    abyssPosX = true;
            }

            playerYTileCoords = point.Y;

            if (WeakReferenceSupport.InAnySubworld())
                return false;

            return point.Y > (Main.rockLayer - y * 0.05) &&
                !player.lavaWet &&
                !player.honeyWet &&
                abyssPosY &&
                abyssPosX;
        }

        public override ModWaterStyle WaterStyle => ModContent.Find<ModWaterStyle>("CalamityMod/AbyssWater");
        public override SceneEffectPriority Priority => SceneEffectPriority.BiomeHigh;
        public override string BestiaryIcon => "CalamityMod/BiomeManagers/AbyssIcon";
        public override string BackgroundPath => "CalamityMod/Backgrounds/MapBackgrounds/AbyssBGLayer1";

        public override int Music
        {
            get
            {
                if (CalamityPlayer.areThereAnyDamnBosses)
                    return Main.curMusic;
                return CalamityMod.Instance.GetMusicFromMusicMod("Abyss1") ?? MusicID.Hell;
            }
        }

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("First Layer of the Abyss");
        }

        public override bool IsBiomeActive(Player player)
        {
            return MeetsBaseAbyssRequirement(player, out int playerYTileCoords) && 
                playerYTileCoords <= (Main.rockLayer + Main.maxTilesY * 0.03);
        }
    }
}
