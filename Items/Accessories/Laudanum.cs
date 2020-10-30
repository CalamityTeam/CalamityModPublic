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
            Tooltip.SetDefault("Converts certain debuffs into buffs and extends their durations\n" +
                               "Debuffs affected: Darkness, Blackout, Confused, Slow, Weak, Broken Armor,\n" +
							   "Armor Crunch, War Cleave, Chilled, Ichor and Obstructed\n" +
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
