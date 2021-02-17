using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class GreenwaveLoach : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Greenwave Loach");
            Tooltip.SetDefault("An endangered fish that is highly prized in the market");
        }

        public override void SetDefaults()
        {
            item.width = 40;
            item.height = 38;
            item.maxStack = 999;
            item.value = Item.sellPrice(gold: 10);
            item.rare = ItemRarityID.Orange;
        }
    }
}
