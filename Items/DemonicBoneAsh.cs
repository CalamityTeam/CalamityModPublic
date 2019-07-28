using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class DemonicBoneAsh : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Demonic Bone Ash");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
			item.maxStack = 999;
			item.value = Item.buyPrice(0, 2, 0, 0);
			item.rare = 3;
		}
	}
}
