using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DeepDiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Diver");
            Tooltip.SetDefault("15% increased damage and movement speed and +15 defense while underwater\n" +
                                "While underwater you gain the ability to dash great distances");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity2BuyPrice;
            item.rare = 2;
            item.defense = 2;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.IsUnderwater())
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.deepDiver = true;
                modPlayer.dashMod = 5;
            }
        }
    }
}
