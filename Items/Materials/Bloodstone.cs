using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class Bloodstone : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Bloodstone");
        }

        public override void SetDefaults()
        {
            item.width = 13;
            item.height = 10;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 60);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 13;
        }
    }
}
