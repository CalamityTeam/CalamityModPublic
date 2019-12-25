using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCalamitas : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitas");
            Tooltip.SetDefault("The witch unrivaled. Perhaps the only one amongst my cohorts to have ever given me cause for doubt.\n" +
                "Now that you have defeated her your destiny is clear.\n" +
                "Come now, face me.\n" +
                "Place in your inventory to die instantly from every hit.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.consumable = false;
            item.Calamity().postMoonLordRarity = 15;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.SCalLore = true;
        }
    }
}
