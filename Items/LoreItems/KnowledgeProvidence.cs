using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeProvidence : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Providence, the Profaned Goddess");
            Tooltip.SetDefault("A core surrounded by stone and flame, a simple origin and a simple goal.\n" +
                "What would have become of us had she not been defeated is a frightening concept to consider.\n" +
                "Place in your inventory to imbue all projectiles with profaned flames, causing them to inflict extra damage.\n" +
				"However, your max life is decreased due to a part of your soul being required to fuel the profaned flames.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.consumable = false;
            item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void UpdateInventory(Player player)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.providenceLore = true;
        }
    }
}
