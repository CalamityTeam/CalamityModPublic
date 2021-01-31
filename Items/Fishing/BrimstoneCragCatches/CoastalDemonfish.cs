using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 8);
            item.rare = ItemRarityID.Blue;
        }
    }
}
