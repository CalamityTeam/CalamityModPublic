using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class GypsyPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gypsy Powder");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
            item.value = Item.sellPrice(silver: 30);
			item.rare = 5;
		}
	}
}
