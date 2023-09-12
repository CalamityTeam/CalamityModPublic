using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class EutrophicRayBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 100;
        public override int BonusNPCID => NPCType<EutrophicRay>();
    }
}
