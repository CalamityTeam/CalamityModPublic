using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class SunkenSailfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunken Sailfish"); //Potion material
            Tooltip.SetDefault("Zooming at 60 miles per hour");
        }

        public override void SetDefaults()
        {
            item.width = 58;
            item.height = 52;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 7);
            item.rare = 2;
        }
    }
}
