using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class CounterScarf : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Counter Scarf");
            Tooltip.SetDefault("Melee weapons that don't fire projectiles are granted 20% more damage\n" +
                "Grants the ability to dash; dashing into an attack will cause you to dodge it\n" +
                "After a dodge you will be granted a buff to all damage, melee speed, and all crit chance for a short time\n" +
                "After a successful dodge you must wait 15 seconds before you can dodge again\n" +
                "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = Item.buyPrice(0, 9, 0, 0);
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dodgeScarf = true;
            modPlayer.dashMod = 1;
        }
    }
}
