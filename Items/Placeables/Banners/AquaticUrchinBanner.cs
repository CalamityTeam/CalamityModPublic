using CalamityMod.NPCs.SulphurousSea;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class AquaticUrchinBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 7;
        public override int BonusNPCID => NPCType<AquaticUrchin>();
    }
}
