using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class DepthCells : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Depth Cells");
            Tooltip.SetDefault("The cells of abyssal creatures");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 80);
            item.rare = ItemRarityID.Lime;
        }
    }
}
