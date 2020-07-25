using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class TitanHeart : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Titan Heart");
            Tooltip.SetDefault("Also used at the Astral Beacon");
        }

        public override void SetDefaults()
        {
            item.width = 10;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 1);
            item.rare = 5;
        }
    }
}
