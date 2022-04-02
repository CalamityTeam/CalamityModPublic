using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class RoverDrive : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rover Drive");
            Tooltip.SetDefault("Activates a protective shield that grants 15 defense for 10 seconds\n" +
            //Actually 10.1 seconds at full power with a dissipation across 0.1666 seconds but whatever
            "The shield then dissipates and recharges for 20 seconds before being reactivated");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 26;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.roverDrive = true;
        }
    }
}
