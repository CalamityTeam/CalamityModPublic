using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class BelchingCoralBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 121;
        public override int BonusNPCID => NPCType<BelchingCoral>();
    }
}
