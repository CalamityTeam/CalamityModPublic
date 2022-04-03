using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class GrandScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Grand Scale");
            Tooltip.SetDefault("Large scale of an apex predator");
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 4, 50, 0);
            Item.rare = ItemRarityID.Lime;
        }
    }
}
