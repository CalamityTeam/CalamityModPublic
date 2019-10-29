using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeTwins : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Twins");
            Tooltip.SetDefault("The bio-mechanical watchers of the night, originally created as security using the souls extracted from human eyes.\n" +
                "These creatures did not belong in this world, it's best to be rid of them.\n" +
                "Place in your inventory to gain invisibility and rogue bonuses at night.");
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
            if (!Main.dayTime)
            {
                player.invis = true;
            }
        }
    }
}
