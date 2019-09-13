using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCrimson : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("The Crimson");
			Tooltip.SetDefault("This bloody hell, spawned from a formless mass of flesh that fell from the stars eons ago.\n" +
                "It is now home to many hideous creatures, spawned from the pumping blood and lurching organs deep within.");
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
	}
}
