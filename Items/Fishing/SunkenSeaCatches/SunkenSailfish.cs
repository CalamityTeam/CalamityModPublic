using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            Item.width = 58;
            Item.height = 52;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 7);
            Item.rare = ItemRarityID.Green;
        }
    }
}
