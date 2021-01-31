using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class BlightedLens : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blighted Lens");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 22;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 56);
            item.rare = ItemRarityID.Pink;
        }
    }
}
