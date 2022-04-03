using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

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
            Item.width = Item.height = 22;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(copper: 80);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
