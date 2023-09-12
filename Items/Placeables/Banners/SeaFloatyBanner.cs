using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class SeaFloatyBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 103;
        public override int BonusNPCID => NPCType<SeaFloaty>();
    }
}
