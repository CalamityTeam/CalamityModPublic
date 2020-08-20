using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class CrawCarapace : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Craw Carapace");
            Tooltip.SetDefault("5% increased damage reduction\n" +
                "Enemies take damage when they touch you");
        }

        public override void SetDefaults()
        {
            item.defense = 3;
            item.width = 28;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = 1;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.05f;
            player.thorns += 0.25f;
        }
    }
}
