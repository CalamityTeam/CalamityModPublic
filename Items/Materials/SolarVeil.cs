using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            item.width = 46;
            item.height = 32;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 80);
            item.rare = ItemRarityID.Lime;
        }
    }
}
