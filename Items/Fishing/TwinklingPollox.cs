using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class TwinklingPollox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkling Pollox"); //Bass substitute
            Tooltip.SetDefault("The scales gleam like crystals");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 5);
            item.rare = 1;
        }
    }
}
