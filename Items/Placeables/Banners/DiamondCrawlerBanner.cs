using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class DiamondCrawlerBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 71;
        public override int BonusNPCID => NPCType<CrawlerDiamond>();
    }
}
