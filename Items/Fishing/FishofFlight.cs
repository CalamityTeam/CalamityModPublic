using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Fishing
{
    public class FishofFlight : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fish of Flight");
            Tooltip.SetDefault("Right click to extract souls");
        }

        public override void SetDefaults()
        {
            item.maxStack = 999;
            item.consumable = true;
            item.width = 30;
            item.height = 30;
            item.rare = ItemRarityID.Orange;
            item.value = Item.sellPrice(gold: 1);
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            DropHelper.DropItem(player, ItemID.SoulofFlight, 2, 5);
        }
    }
}
