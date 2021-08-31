using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class MolluskHusk : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Mollusk Husk");
            Tooltip.SetDefault("The remains of a mollusk");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 3, 0, 0);
            item.rare = ItemRarityID.Pink;
        }
    }
}
