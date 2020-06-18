using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BloodyWormTooth : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloody Worm Tooth");
            Tooltip.SetDefault("5% increased damage reduction and increased melee stats\n" +
                               "10% increased damage reduction and melee stats when below 50% life");
        }

        public override void SetDefaults()
        {
            item.width = 12;
            item.height = 15;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.expert = true;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodyWormTooth = true;
        }
    }
}
