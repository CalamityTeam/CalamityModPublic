using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class AldebaranAlewife : ModItem
    {
        public override void SetStaticDefaults()
        {
                        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
            DisplayName.SetDefault("Aldebaran Alewife");
            Tooltip.SetDefault("A star-struck entity in the form of a fish");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 36;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 8);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
