using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class CrawCarapace : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Craw Carapace");
            Tooltip.SetDefault("Enemies take damage when they touch you");
        }

        public override void SetDefaults()
        {
            Item.defense = 5;
            Item.width = 28;
            Item.height = 28;
            Item.value = CalamityGlobalItem.Rarity1BuyPrice;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.thorns += 0.5f;
        }
    }
}
