using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class ScarredAngelfish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Scarred Angelfish");
            Tooltip.SetDefault("The mark of a fallen angel");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 7);
            item.rare = ItemRarityID.Blue;
        }
    }
}
