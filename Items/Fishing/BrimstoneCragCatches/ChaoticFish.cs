using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class ChaoticFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Chaotic Fish"); //Future potion ingredient
            Tooltip.SetDefault("The horns lay a curse on those who touch it");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 10);
            item.rare = 2;
        }
    }
}
