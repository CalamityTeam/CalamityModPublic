using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class Ectoblood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ectoblood");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 32;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 16);
            item.rare = 8;
        }
    }
}
