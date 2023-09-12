using CalamityMod.NPCs.Crags;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("CultistAssassinBanner")]
    public class RenegadeWarlockBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 95;
        public override int BonusNPCID => NPCType<RenegadeWarlock>();
    }
}
