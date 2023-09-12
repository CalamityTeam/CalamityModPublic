using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class PlagueChargerBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 81;
        public override int BonusNPCID => NPCType<PlagueCharger>();
    }
}
