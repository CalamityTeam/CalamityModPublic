using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class PurifiedGel : ModItem, ILocalizedModType
    {
        public new string LocalizationCategory => "Items.Materials";
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Type] = 71; // Soul of Light
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 36;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.LightRed;
        }    }
}
