using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class PermafrostsConcoction : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Permafrost's Concoction");
            Tooltip.SetDefault(@"Increases maximum mana by 50
Increases life regen as life decreases
Increases life regen when afflicted with Poison, On Fire, or Brimstone Flames
You will survive fatal damage and revive with 30% life on a 3 minute cooldown
You are encased in an ice barrier for 5 seconds when revived");
        }

        public override void SetDefaults()
        {
            item.accessory = true;
            item.width = 36;
            item.height = 34;
            item.value = Item.buyPrice(0, 45, 0, 0);
            item.rare = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.statManaMax2 += 50;
            player.Calamity().permafrostsConcoction = true;
        }
    }
}
