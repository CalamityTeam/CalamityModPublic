using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class EbonianGel : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Blighted Gel");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 14;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 1;
		}
	}
}
