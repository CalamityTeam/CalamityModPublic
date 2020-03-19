using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class SulfuricScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sulphuric Scale");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 3);
            item.rare = 1;
        }
    }
}
