using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class LivingShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Living Shard");
        }

        public override void SetDefaults()
        {
            item.width = 14;
            item.height = 14;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 80);
            item.rare = 7;
        }
    }
}
