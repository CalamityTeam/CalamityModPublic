using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class GnasherBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 2;
        public override int BonusNPCID => NPCType<Gnasher>();
    }
}
