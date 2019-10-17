using Terraria;
using Terraria.ModLoader;
using Terraria.DataStructures;

namespace CalamityMod.Items
{
    public class EffulgentFeather : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Effulgent Feather");
            Tooltip.SetDefault("It vibrates with fluffy golden energy");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 12));
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 6, 50, 0);
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 12;
        }
    }
}
