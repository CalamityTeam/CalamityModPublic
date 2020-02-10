using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SpectralVeil : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Spectral Veil");
            Tooltip.SetDefault("The inside of the cloak is full of teeth...\n" +
                "Press Z to consume 25% of your maximum stealth to perform a short range teleport and render you momentarily invulnerable\n" +
                "If you dodge something while invulnerable, you instantly gain full stealth");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 38;
            item.value = Item.buyPrice(1, 40, 0, 0);
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().spectralVeil = true;
        }
    }
}
