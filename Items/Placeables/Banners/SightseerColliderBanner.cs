using CalamityMod.NPCs.Astral;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("SmallSightseerBanner")]
    public class SightseerColliderBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 32;
        public override int BonusNPCID => NPCType<SightseerCollider>();
    }
}
