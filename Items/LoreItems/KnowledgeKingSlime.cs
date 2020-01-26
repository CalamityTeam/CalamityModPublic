using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeKingSlime : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slime");
            Tooltip.SetDefault("Only a fool could be caught by this pitiful excuse for a hunter.\n" +
                "Unfortunately, our world has no shortage of those.\n" +
				"Place in your inventory to gain a slight movement speed and jump boost.\n" +
				"However, your defense will be reduced.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 1;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            player.Calamity().kingSlimeLore = true;
        }
    }
}
