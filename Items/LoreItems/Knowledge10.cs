using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge10 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Ocean");
			Tooltip.SetDefault("Take care to not disturb the deep waters of this world.\n" +
                "You may awaken something more terrifying than death itself.");
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
	}
}
