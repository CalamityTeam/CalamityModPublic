using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge7 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Underworld");
			Tooltip.SetDefault("These obsidian and hellstone towers were once home to thousands of...'people'.\n" +
                "Unfortunately for them, they were twisted by their inner demons until they were beyond saving.");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.rare = 4;
			item.consumable = false;
		}

		public override bool CanUseItem(Player player)
		{
			return false;
		}
	}
}
