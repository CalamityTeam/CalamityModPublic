using CalamityMod.NPCs.Abyss;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class EidolonWyrmJuvenileBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 27;
        public override int BonusNPCID => NPCType<EidolonWyrmHead>();
    }
}
