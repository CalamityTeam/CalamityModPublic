namespace CalamityMod.Items.Placeables.Banners
{
    public class PiggyBanner : BaseBanner
    {
        public override int BannerTileStyle => 108;
        public override void SetDefaults()
        {
            Item.Calamity().donorItem = true;
            base.SetDefaults();
        }
    }
}
