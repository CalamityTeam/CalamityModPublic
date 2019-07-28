using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class WulfrumShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Wulfrum Shard");
        }

        public override void SetDefaults()
        {
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
			item.value = Item.buyPrice(0, 0, 10, 0);
			item.rare = 1;
        }
    }
}
