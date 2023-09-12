using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("WulfrumPylonBanner")]
    public class WulfrumAmplifierBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 126;
        public override int BonusNPCID => NPCType<WulfrumAmplifier>();
    }
}
