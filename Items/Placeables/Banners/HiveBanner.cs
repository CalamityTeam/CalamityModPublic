using CalamityMod.NPCs.Astral;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class HiveBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 40;
        public override int BonusNPCID => NPCType<HiveEnemy>();
    }
}
