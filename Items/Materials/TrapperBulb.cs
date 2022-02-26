using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class TrapperBulb : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Trapper Bulb");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 16);
            item.rare = ItemRarityID.LightRed;
        }
    }
}
