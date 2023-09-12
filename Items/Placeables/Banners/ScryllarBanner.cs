using CalamityMod.NPCs.Crags;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class ScryllarBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 45;
        public override int BonusNPCID => NPCType<Scryllar>();
    }
}
