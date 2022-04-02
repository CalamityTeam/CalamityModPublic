using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class VitalJelly : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Vital Jelly");
            Tooltip.SetDefault("10% increased movement and jump speed");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 24;
            item.value = CalamityGlobalItem.Rarity1BuyPrice;
            item.rare = ItemRarityID.Blue;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.1f;
            player.jumpSpeedBoost += 0.5f;
        }
    }
}
