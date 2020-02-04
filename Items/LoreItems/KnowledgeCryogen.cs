using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCryogen : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cryogen");
            Tooltip.SetDefault("The archmage's prison.\n" +
                "I am unsure if it has grown weaker over the decades of imprisonment.\n" +
                "Place in your inventory to gain a frost dash that freezes enemies, at the cost of slightly reduced defense due to your brittle body.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 5;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dashMod = 6;
            player.statDefense -= 10;
        }
    }
}
