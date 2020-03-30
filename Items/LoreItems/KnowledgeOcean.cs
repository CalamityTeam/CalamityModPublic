using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeOcean : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Ocean");
            Tooltip.SetDefault("Take care to not disturb the deep waters of this world.\n" +
                "You may awaken something more terrifying than death itself.\n" +
                "Place in your inventory to prevent the mysterious siren lure from spawning.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 7;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.oceanLore = true;
        }
    }
}
