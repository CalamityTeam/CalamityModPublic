using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class PurifiedGel : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Type] = 71; // Soul of Light
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
        }    }
}
