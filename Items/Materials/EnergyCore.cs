using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class EnergyCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Energy Core");
            Tooltip.SetDefault("It pulses with energy");
        }

        public override void SetDefaults()
        {
            item.width = item.height = 22;
            item.maxStack = 999;
            item.value = Item.sellPrice(copper: 80);
            item.rare = 1;
        }
    }
}
