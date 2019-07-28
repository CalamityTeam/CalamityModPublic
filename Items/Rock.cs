using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class Rock : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Rock");
            Tooltip.SetDefault("The first object Xeroc ever created");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
			item.value = Item.buyPrice(0, 0, 0, 1);
		}
    }
}
