using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class VitalJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vital Jelly");
            Tooltip.SetDefault("20% increased movement speed\n" +
                "120% increased jump speed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity3BuyPrice;
            item.rare = 3;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.2f;
            player.jumpSpeedBoost += player.autoJump ? 0.3f : 1.2f;
        }
    }
}
