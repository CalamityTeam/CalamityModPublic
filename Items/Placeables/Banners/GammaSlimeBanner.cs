using Terraria.ID;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class GammaSlimeBanner : BaseBanner
    {
        public override int BannerTileStyle => 122;
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<IrradiatedSlimeBanner>();
        }
    }
}
