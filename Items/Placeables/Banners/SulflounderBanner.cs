using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("FlounderBanner")]
    public class SulflounderBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 1;
        public override int BonusNPCID => NPCType<Sulflounder>();
    }
}
