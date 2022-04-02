using Terraria;
using Terraria.ModLoader;
using CalamityMod.Items.Materials;
using Terraria.ID;

namespace CalamityMod.Items.Fishing
{
    public class Xerocodile : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xerocodile");
            Tooltip.SetDefault("Right click to extract blood orbs");
        }

        public override void SetDefaults()
        {
            item.width = 30;
            item.height = 28;
            item.maxStack = 999;
            item.value = Item.sellPrice(silver: 10);
            item.rare = ItemRarityID.Green;
        }

        public override bool CanRightClick()
        {
            return true;
        }

        public override void RightClick(Player player)
        {
            DropHelper.DropItem(player, ModContent.ItemType<BloodOrb>(), 5, 15);
        }

        /*public override void RightClick(Player player)
        {
            if (Main.rand.NextBool(3))
                DropHelper.DropItem(player, ModContent.ItemType<TheCamper>(), 1, 1);
            else if (Main.rand.NextBool(2))
                DropHelper.DropItem(player, ModContent.ItemType<CheatTestThing>(), 1, 1);
            else
            {
                DropHelper.DropItem(player, ModContent.ItemType<CheatTestThing>(), 1, 1);
                DropHelper.DropItem(player, ModContent.ItemType<TheCamper>(), 1, 1);
            }
        }*/
    }
}
