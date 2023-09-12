using CalamityMod.NPCs.AcidRain;
using CalamityMod.Tiles;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class GammaSlimeBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 122;
        public override int BonusNPCID => NPCType<GammaSlime>();
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<IrradiatedSlimeBanner>();
        }
    }
}
