using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    public class BloodSample : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Blood Sample");
			ItemID.Sets.SortingPriorityMaterials[Type] = 68; // Sturdy Fossil
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 34;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
        }    }
}
