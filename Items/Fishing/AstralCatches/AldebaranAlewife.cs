using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Fishing.AstralCatches
{
    public class AldebaranAlewife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aldebaran Alewife");
            Tooltip.SetDefault("A star-struck entity in the form of a fish");
        }

        public override void SetDefaults()
        {
            item.width = 38;
            item.height = 36;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 8);
            item.rare = ItemRarityID.Blue;
        }
    }
}
