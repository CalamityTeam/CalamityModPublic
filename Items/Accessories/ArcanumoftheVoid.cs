using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class ArcanumoftheVoid : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Arcanum of the Void");
            Tooltip.SetDefault("You have a 5% chance to reflect projectiles when they hit you\n" +
                               "If this effect triggers you get healed for the projectile's damage");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity13BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.projRef = true;
        }
    }
}
