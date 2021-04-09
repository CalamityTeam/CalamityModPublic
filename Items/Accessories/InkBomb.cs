using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class InkBomb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ink Bomb");
            Tooltip.SetDefault("Throws several ink bombs when hit that explode in a confusing cloud of ink\n" +
                "Gain a lot of stealth when struck\n" +
                "This effect has a 20s cooldown before it can occur again");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 50;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = ItemRarityID.Orange;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.inkBomb = true;
        }
    }
}
