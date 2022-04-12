using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Materials
{
    public class PurifiedGel : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Purified Gel");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
    }
}
