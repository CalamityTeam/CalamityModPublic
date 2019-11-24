using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class SolarVeil : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Veil");
            Tooltip.SetDefault("Sunlight cannot penetrate the fabric of this cloth");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 3;
        }
    }
}
