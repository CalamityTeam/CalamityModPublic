using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EtherealExtorter : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ethereal Extorter");
            Tooltip.SetDefault(@"Infuses souls into your weapons and body generating different boosts that vary with the environment
Rogue projectiles rarely explode into homing souls
10% increased rogue damage but reduced life regen");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.accessory = true;
            item.value = CalamityGlobalItem.Rarity8BuyPrice;
            item.rare = 8;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.etherealExtorter = true;
            modPlayer.throwingDamage += 0.1f;
			player.lifeRegen -= 1;
			if (Main.moonPhase == 4) // 4 = New Moon
				modPlayer.rogueStealthMax += 0.1f;
        }
    }
}
