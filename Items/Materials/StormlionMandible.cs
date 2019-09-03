using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class StormlionMandible : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stormlion Mandible");
		}

		public override void SetDefaults()
		{
			item.width = 12;
			item.height = 24;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 1;
		}
	}
}
