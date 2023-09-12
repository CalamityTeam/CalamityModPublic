using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class PestilentSlimeBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 79;
        public override int BonusNPCID => NPCType<PestilentSlime>();
    }
}
