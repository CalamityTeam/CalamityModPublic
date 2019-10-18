using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items.Materials
{
    public class FetidEssence : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fetid Essence");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 1);
            item.rare = 1;
        }
    }
}
