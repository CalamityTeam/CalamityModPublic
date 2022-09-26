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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Ink Bomb");
            Tooltip.SetDefault("Throws several ink bombs when hit that explode in a confusing cloud of ink\n" +
                "Gain a lot of stealth when struck\n" +
                "This effect has a 25 second cooldown before it can occur again");
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 50;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.inkBomb = true;
        }
    }
}
