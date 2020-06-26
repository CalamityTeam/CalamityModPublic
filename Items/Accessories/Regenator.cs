using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class Regenator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regenator");
            Tooltip.SetDefault("Reduces max HP by 50% but greatly improves life regeneration");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity5BuyPrice;
            item.rare = 5;
            item.defense = 6;
            item.accessory = true;
            item.Calamity().customRarity = CalamityRarity.RareVariant;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.regenator = true;
        }
    }
}
