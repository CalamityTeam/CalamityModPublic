using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheBee : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Bee");
            Tooltip.SetDefault("Causes stars to fall and releases bees when injured\n" +
							   "When at full HP, your damage is increased based on your damage reduction\n" +
                               "Damage taken at full HP is halved\n" +
							   "This has a 10 second cooldown");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = 4;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.theBee = true;
        }
    }
}
