using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Scavenger
{
    public class FleshTotem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Flesh Totem");
            Tooltip.SetDefault("Halves enemy contact damage\n" +
                "When you take contact damage this effect has a 20 second cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 8;
            item.value = Item.buyPrice(0, 24, 0, 0);
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            modPlayer.fleshTotem = true;
        }
    }
}
