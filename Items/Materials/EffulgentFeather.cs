using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class EffulgentFeather : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Effulgent Feather");
            Tooltip.SetDefault("It vibrates with fluffy golden energy");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(3, 11));
        }

        public override void SetDefaults()
        {
            item.width = 24;
            item.height = 24;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 6, 50, 0);
            item.rare = ItemRarityID.Purple;
        }
    }
}
