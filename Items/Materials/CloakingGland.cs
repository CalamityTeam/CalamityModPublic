using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CloakingGland : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cloaking Gland");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 26;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 2, 0, 0);
            item.rare = 3;
        }
    }
}
