using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing.SunkenSeaCatches
{
    public class PrismaticGuppy : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Prismatic Guppy");
            Tooltip.SetDefault("Throwing these in an aquarium would be insanity");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 7);
            item.rare = 2;
        }
    }
}
