using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class DesertFeather : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Desert Feather");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.sellPrice(copper: 20);
            item.rare = ItemRarityID.Blue;
        }
    }
}
