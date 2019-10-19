using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class OldDie : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Die");
            Tooltip.SetDefault("Increases randomness of attack damage \n" +
                               "'Lucky for you, the curse doesn't affect you. Mostly.'");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 26;
            item.rare = 3;
            item.value = Item.buyPrice(0, 10, 0, 0);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.oldDie = true;
        }
    }
}
