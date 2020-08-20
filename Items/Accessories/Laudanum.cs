using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
namespace CalamityMod.Items.Accessories
{
	public class Laudanum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laudanum");
            Tooltip.SetDefault("The Horror debuff lasts twice as long,\n" +
                               "but it instead grants various buffs to the player\n" +
                               "Revengeance drop");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
        }
    }
}
