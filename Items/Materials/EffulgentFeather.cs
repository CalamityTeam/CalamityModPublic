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
            Item.ResearchUnlockCount = 25;
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(3, 11));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
			ItemID.Sets.SortingPriorityMaterials[Type] = 102;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 6, 50, 0);
            Item.rare = ItemRarityID.Purple;
        }
    }
}
