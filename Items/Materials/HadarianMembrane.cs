using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class HadarianMembrane : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Hadarian Membrane");
            Tooltip.SetDefault("The membrane of an astral creature's wings");
        }

        public override void SetDefaults()
        {
            item.width = 22;
            item.height = 22;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 50);
            item.rare = 7;
        }
    }
}
