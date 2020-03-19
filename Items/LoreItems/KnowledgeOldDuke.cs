using CalamityMod.CalPlayer;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeOldDuke : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Duke");
            Tooltip.SetDefault("Boomer duke moment.\n" +
                "He wanted to eat your face off but then you murdered him.\n" +
                "Place in your inventory for undetermined effects as of now.\n" +
				"However, you're supposed to have negative effects due to acidic radiation.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

		public override void UpdateInventory(Player player)
		{
			CalamityPlayer modPlayer = player.Calamity();
			modPlayer.boomerDukeLore = true;
		}
    }
}
