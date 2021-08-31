using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class CragBullhead : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Crag Bullhead"); //Bass substitute
            Tooltip.SetDefault("Its scales are scorching hot");
        }

        public override void SetDefaults()
        {
            item.width = 32;
            item.height = 36;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 5);
            item.rare = ItemRarityID.Blue;
        }
    }
}
