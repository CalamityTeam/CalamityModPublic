using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class BloodPact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Pact");
            Tooltip.SetDefault("Doubles your max HP\n" +
                "Allows you to be critically hit 25% of the time\n" +
				"After a critical hit, you gain various buffs for ten seconds\n" +
				"Any healing potions consumed during this time period heal 50 percent more health");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.rare = 8;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.bloodPact = true;
        }
    }
}
