using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class RuinousSoul : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ruinous Soul");
            Tooltip.SetDefault("A shard of the distant past");
            Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(6, 6));
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 999;
            item.value = Item.buyPrice(0, 7, 0, 0);
            item.Calamity().customRarity = CalamityRarity.PureGreen;
        }
    }
}
