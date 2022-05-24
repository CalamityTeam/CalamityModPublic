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
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 26;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 7);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
