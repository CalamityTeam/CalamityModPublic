using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class Shadowfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shadowfish"); //Future potion ingredient
            Tooltip.SetDefault("Darkness spreads");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 8);
            item.rare = 1;
        }
    }
}
