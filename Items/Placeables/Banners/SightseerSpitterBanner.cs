using CalamityMod.NPCs.Astral;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("BigSightseerBanner")]
    public class SightseerSpitterBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 33;
        public override int BonusNPCID => NPCType<SightseerSpitter>();
    }
}
