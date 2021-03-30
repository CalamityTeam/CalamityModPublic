using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class Regenator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Regenator");
            Tooltip.SetDefault("Reduces max HP by 50% but greatly improves life regeneration\n" +
				"Rare Item Variant");
        }

        public override void SetDefaults()
        {
            item.width = 36;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
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
