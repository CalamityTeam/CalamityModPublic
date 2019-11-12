using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class CragBullhead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crag Bullhead"); //Future potion ingredient
            Tooltip.SetDefault("Its scales are scorching hot");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 36;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 8);
            item.rare = 1;
        }
    }
}
