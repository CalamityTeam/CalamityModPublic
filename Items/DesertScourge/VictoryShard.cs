using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.DesertScourge
{
    public class VictoryShard : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Victory Shard");
		}

		public override void SetDefaults()
		{
			item.width = 14;
			item.height = 14;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 1;
		}
	}
}
