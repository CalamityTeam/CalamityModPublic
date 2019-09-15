using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class Stardust : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Stardust");
		}

		public override void SetDefaults()
		{
			item.width = 26;
			item.height = 18;
			item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
			item.rare = 5;
		}
	}
}
