using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items
{
    public class BrimstoneKey : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Opens locked ashen chests");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 1;
            item.value = 100;
            item.rare = ItemRarityID.Blue;
        }
    }
}
