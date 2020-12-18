using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class TheEvolution : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Evolution");
            Tooltip.SetDefault("You have a 50% chance to reflect projectiles when they hit you back at the enemy for 1000% their original damage\n" +
                                "If this effect triggers you get a health regeneration boost for a short time\n" +
                                "If the same enemy projectile type hits you again you will resist its damage by 15%");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
			item.value = CalamityGlobalItem.Rarity12BuyPrice;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.projRefRare = true;
        }
    }
}
