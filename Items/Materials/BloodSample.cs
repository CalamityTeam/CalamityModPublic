using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class BloodSample : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blood Sample");
        }

        public override void SetDefaults()
        {
            item.width = 26;
            item.height = 34;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 0, 50, 0);
            item.rare = ItemRarityID.Orange;
        }
    }
}
