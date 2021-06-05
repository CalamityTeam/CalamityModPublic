using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class Ectoblood : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ectoblood");
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 32;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 16);
            item.rare = ItemRarityID.Lime;
        }
    }
}
