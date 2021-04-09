using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class TrueShadowScale : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("True Shadow Scale");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 22;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Green;
        }
    }
}
