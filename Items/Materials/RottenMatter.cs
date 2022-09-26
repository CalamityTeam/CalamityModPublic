using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Materials
{
    [LegacyName("TrueShadowScale")]
    public class RottenMatter : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Rotten Matter");
			ItemID.Sets.SortingPriorityMaterials[Type] = 65; // Crimtane Ore
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
