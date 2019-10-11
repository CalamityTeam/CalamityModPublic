using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.AbyssItems
{
    public class AnechoicPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Anechoic Plating");
            Tooltip.SetDefault("Reduces creature's ability to detect you in the abyss\n" +
                "Reduces the defense reduction that the abyss causes");
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
            modPlayer.anechoicPlating = true;
        }
    }
}
