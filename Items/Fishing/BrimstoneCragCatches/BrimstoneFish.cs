using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.BrimstoneCragCatches
{
    public class BrimstoneFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Fish"); //Bass substitute
            Tooltip.SetDefault("Fire is a living being");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 30;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 5);
            item.rare = 1;
        }
    }
}
