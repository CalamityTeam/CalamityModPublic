using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;
using Terraria.GameContent.Creative;

namespace CalamityMod.Items.Fishing
{
    public class Xerocodile : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xerocodile");
            Tooltip.SetDefault("Right click to extract blood orbs");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 10;
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
            var s = player.GetSource_OpenItem(Item.type);
            DropHelper.DropItem(s, player, ModContent.ItemType<BloodOrb>(), 5, 15);
        }
    }
}
