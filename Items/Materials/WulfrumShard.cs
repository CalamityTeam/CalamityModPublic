using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class WulfrumShard : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Wulfrum Metal Scrap");
        }

        public override void SetDefaults()
        {
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 80);
            Item.rare = ItemRarityID.Blue;
        }    }
}
