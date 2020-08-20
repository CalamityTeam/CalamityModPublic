using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class SeaShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sea Shell");
            Tooltip.SetDefault("Increased defense and damage reduction when submerged in liquid\n" +
                "Increased movement speed when submerged in liquid");
        }

        public override void SetDefaults()
        {
            item.defense = 2;
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = 2;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.seaShell = true;
        }
    }
}
