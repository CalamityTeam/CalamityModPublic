using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeEyeofCthulhu : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Eye of Cthulhu");
            Tooltip.SetDefault("That eye...how peculiar.\n" +
                "I sensed it watching you more intensely as you grew stronger.\n" +
                "Place in your inventory for night vision at night.\n" +
				"However, this effect does not trigger while underground.");
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
            if (!Main.dayTime && (player.ZoneOverworldHeight || player.ZoneSkyHeight))
                player.nightVision = true;
        }
    }
}
