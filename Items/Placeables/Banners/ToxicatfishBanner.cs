using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("CatfishBanner")]
    public class ToxicatfishBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 4;
        public override int BonusNPCID => NPCType<Toxicatfish>();
    }
}
