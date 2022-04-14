using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class TwinklingPollox : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Twinkling Pollox"); //Bass substitute
            Tooltip.SetDefault("The scales gleam like crystals");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 28;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 5);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
