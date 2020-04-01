using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCorruption : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Corruption");
            Tooltip.SetDefault("The rotten and forever-deteriorating landscape of infected life, brought upon by a deadly microbe long ago.\n" +
                "It is rumored that the microbe was created through experimentation by a long-dead race, predating the Terrarians.\n" +
                "Place in your inventory to prevent hive cysts from spawning.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 2;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.corruptionLore = true;
        }
    }
}
