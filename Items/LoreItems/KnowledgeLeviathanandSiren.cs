using Terraria;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeLeviathanandSiren : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Leviathan and Siren");
			Tooltip.SetDefault("An odd pair of creatures; one seeking companionship and the other seeking sustenance.\n" +
                "Perhaps two genetic misfits outcast from their homes that found comfort in assisting one another.\n" +
				"Place in your inventory to gain increased max health while wearing the siren heart and treasure detect while wearing the strange orb.\n" +
				"Allows the young siren pet to move normally while outside of liquids.");
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
			modPlayer.leviathanAndSirenLore = true;
		}
	}
}
