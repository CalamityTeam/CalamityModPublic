using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class RustyMedallion : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rusty Medallion");
            Tooltip.SetDefault("Causes most ranged weapons to sometimes release acid droplets from the sky");
        }

        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 32;
            item.rare = 1;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.rustyMedal = true;
        }
    }
}
