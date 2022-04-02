using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class DeepDiver : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Deep Diver");
            Tooltip.SetDefault("15% increased damage, movement speed and +15 defense while underwater\n" +
                                "While underwater you gain the ability to dash great distances");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 28;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = ItemRarityID.Pink;
            item.defense = 8;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.IsUnderwater())
            {
                CalamityPlayer modPlayer = player.Calamity();
                modPlayer.deepDiver = true;
                modPlayer.dashMod = 5;
                player.dash = 0;
            }
        }
    }
}
