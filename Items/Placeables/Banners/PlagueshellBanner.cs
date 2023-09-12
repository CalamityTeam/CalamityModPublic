using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class PlagueshellBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 80;
        public override int BonusNPCID => NPCType<Plagueshell>();
    }
}
