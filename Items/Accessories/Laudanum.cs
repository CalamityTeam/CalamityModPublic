using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Laudanum : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
            DisplayName.SetDefault("Laudanum");
            Tooltip.SetDefault("Converts certain debuffs into buffs and extends their durations\n" +
                               "Debuffs affected: Darkness, Blackout, Confused, Slow, Weak, Broken Armor,\n" +
                               "Armor Crunch, War Cleave, Chilled, Ichor and Obstructed\n" +
                               "Revengeance drop");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 26;
            Item.value = CalamityGlobalItem.Rarity3BuyPrice;
            Item.rare = ItemRarityID.Orange;
            Item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.laudanum = true;
        }
    }
}
