using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class CoastalDemonfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Coastal Demonfish"); //Hadal Stew ingredient
            Tooltip.SetDefault("The horns lay a curse on those who touch it");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 8);
            Item.rare = ItemRarityID.Blue;
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }
    }
}
