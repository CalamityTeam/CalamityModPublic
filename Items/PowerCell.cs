using CalamityMod.UI;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class PowerCell : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Draedon Power Cell");
            Tooltip.SetDefault("Used at a charger to charge special weapons");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 14;
            item.rare = 10;
            item.maxStack = 999;
        }
    }
}
