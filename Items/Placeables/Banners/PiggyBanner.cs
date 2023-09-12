using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.Tiles;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.Items.Placeables.Banners
{
    public class PiggyBanner : BaseBanner
    {
        public override int BannerTileID => TileType<MonsterBanner>();
        public override int BannerTileStyle => 108;
        public override int BonusNPCID => NPCType<Piggy>();
        public override void SetDefaults()
        {
            Item.Calamity().donorItem = true;
            base.SetDefaults();
        }
    }
}
