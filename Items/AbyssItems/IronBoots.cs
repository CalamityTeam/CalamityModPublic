using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.AbyssItems
{
    public class IronBoots : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Iron Boots");
            Tooltip.SetDefault("Allows you to fall faster while in liquids");
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
            modPlayer.ironBoots = true;
        }
    }
}
