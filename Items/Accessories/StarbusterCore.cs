using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Accessories
{
    public class StarbusterCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Starbuster Core");
            Tooltip.SetDefault("Summons release an astral explosion on enemy hits\n" +
                               "+1 max minion");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 32;
            item.value = CalamityGlobalItem.Rarity7BuyPrice;
            item.rare = ItemRarityID.Lime;
            item.accessory = true;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Calamity().starbusterCore = true;
        }
    }
}
