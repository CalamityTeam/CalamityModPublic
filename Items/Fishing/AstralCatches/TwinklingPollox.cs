using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
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
            item.rare = ItemRarityID.Blue;
        }
    }
}
