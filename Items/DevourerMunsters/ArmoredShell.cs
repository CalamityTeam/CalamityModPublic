using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.DevourerMunsters
{
    public class ArmoredShell : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Armored Shell");
		}

		public override void SetDefaults()
		{
			item.width = 24;
			item.height = 30;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 7, 0, 0);
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 13;
		}
	}
}
