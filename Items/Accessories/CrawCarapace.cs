using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class CrawCarapace : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Accessories";
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.crawCarapace = true;
            player.thorns += 0.25f;
        }
    }
}
