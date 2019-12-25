using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;

namespace CalamityMod.Items.Fishing
{
    public class SunbeamFish : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sunbeam Fish");
            Tooltip.SetDefault("Right click to extract essence");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 10);
            item.rare = 2;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            DropHelper.DropItem(player, ModContent.ItemType<EssenceofCinder>(), 5, 10);
        }
    }
}
