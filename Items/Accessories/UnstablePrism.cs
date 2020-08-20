using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class UnstablePrism : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Unstable Prism");
            Tooltip.SetDefault("Three sparks are released on critical hits");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 38;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.unstablePrism = true;
        }
    }
}
