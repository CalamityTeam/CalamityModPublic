using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class MurkySludge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Murky Sludge");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 1;
		}
	}
}
