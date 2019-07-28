using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class Knowledge27 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Aquatic Scourge");
			Tooltip.SetDefault("A horror born of pollution and insatiable hunger; based on size alone this was merely a juvenile.\n" +
                "These scourge creatures are the largest aquatic predators and very rarely do they frequent such shallow waters.\n" +
				"Place in your inventory to gain immunity to the sulphurous waters and increase the stat gains from the Well Fed buff.");
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
			CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
			modPlayer.aquaticScourgeLore = true;
		}
	}
}
