using CalamityMod.NPCs.AcidRain;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class AcidEelBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 115;
        public override int BonusNPCID => NPCType<AcidEel>();
    }
}
