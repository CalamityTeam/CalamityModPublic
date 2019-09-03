using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BeetleJuice : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Beetle Juice");
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
