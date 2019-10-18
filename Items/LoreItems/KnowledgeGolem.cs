using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeGolem : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golem");
            Tooltip.SetDefault("A primitive construct.\n" +
                "I admire the lihzahrd race for their ingenuity, though finding faith in such a flawed idol would invariably lead to their downfall.\n" +
                "Place in your inventory to gain increased defense while standing still.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 8;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.golemLore = true;
        }
    }
}
