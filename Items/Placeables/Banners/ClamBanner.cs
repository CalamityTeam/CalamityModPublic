using CalamityMod.NPCs.SunkenSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class ClamBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 99;
        public override int BonusNPCID => NPCType<Clam>();
    }
}
