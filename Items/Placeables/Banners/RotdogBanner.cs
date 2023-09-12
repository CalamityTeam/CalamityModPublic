using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("PitbullBanner")]
    public class RotdogBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 53;
        public override int BonusNPCID => NPCType<Rotdog>();
    }
}
