using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables
{
    public abstract class BaseBanner : ModItem, ILocalizedModType
    {
        // The tile to place (and style if needed)
        public virtual int BannerTileID => 0;
        public virtual int BannerTileStyle => 0;

        // Associated Modded NPC
        public virtual int BonusNPCID => 0;
        public virtual LocalizedText NPCName => NPCLoader.GetNPC(BonusNPCID).DisplayName;

        public new string LocalizationCategory => "Items.Placeables";
        // Override these if there are no NPCs attached to the banner.
        public override LocalizedText DisplayName => CalamityUtils.GetText($"{LocalizationCategory}.FormattedBannerName").WithFormatArgs(NPCName.ToString());
        public override LocalizedText Tooltip => CalamityUtils.GetText($"{LocalizationCategory}.FormattedBannerTooltip").WithFormatArgs(NPCName.ToString());

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(BannerTileID, BannerTileStyle);

            // Banners usually have these values.
            Item.width = 10;
            Item.height = 24;
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(silver: 2);
        }
    }
}
