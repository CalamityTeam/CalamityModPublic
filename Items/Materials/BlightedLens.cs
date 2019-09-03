using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BlightedLens : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blighted Lens");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 22;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 3, 0, 0);
			item.rare = 5;
		}
	}
}
