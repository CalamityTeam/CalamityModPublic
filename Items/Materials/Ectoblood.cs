using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class Ectoblood : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ectoblood");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 32;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = 8;
		}
	}
}
