using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.AbyssItems
{
    public class DepthCharm : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depths Charm");
            Tooltip.SetDefault("Reduces the damage caused by the pressure of the abyss while out of breath\n" +
                "Removes the bleed effect caused by the upper layers of the abyss");
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
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            modPlayer.depthCharm = true;
        }
    }
}
