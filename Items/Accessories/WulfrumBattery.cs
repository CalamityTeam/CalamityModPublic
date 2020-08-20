using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class WulfrumBattery : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Battery");
            Tooltip.SetDefault("7% increased summon damage");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 22;
            item.value = Item.buyPrice(0, 1, 0, 0);
            item.accessory = true;
            item.rare = 1;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.minionDamage += 0.07f;
        }
    }
}
