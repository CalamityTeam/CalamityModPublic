using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Fishing
{
    public class FishofEleum : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fish of Eleum");
            Tooltip.SetDefault("Right click to extract essence");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 28;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 10);
            Item.rare = ItemRarityID.Green;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            // IEntitySource my beloathed
            var s = player.GetItemSource_OpenItem(Item.type);
            DropHelper.DropItem(s, player, ModContent.ItemType<EssenceofEleum>(), 5, 10);
        }
    }
}
