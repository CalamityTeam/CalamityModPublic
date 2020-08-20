using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
	public class RogueEmblem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rogue Emblem");
            Tooltip.SetDefault("15% increased rogue damage");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity4BuyPrice;
            item.rare = 4;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().throwingDamage += 0.15f;
        }
    }
}
