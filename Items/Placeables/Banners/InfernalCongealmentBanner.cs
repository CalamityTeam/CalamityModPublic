using CalamityMod.NPCs.Crags;
using CalamityMod.Tiles;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    [LegacyName("CharredSlimeBanner")]
    public class InfernalCongealmentBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 93;
        public override int BonusNPCID => NPCType<InfernalCongealment>();
    }
}
