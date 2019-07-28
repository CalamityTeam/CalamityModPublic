using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class TrapperBulb : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Trapper Bulb");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 5;
		}
	}
}
