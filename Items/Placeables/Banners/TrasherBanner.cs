using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class TrasherBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 3;
        public override int BonusNPCID => NPCType<Trasher>();
    }
}
