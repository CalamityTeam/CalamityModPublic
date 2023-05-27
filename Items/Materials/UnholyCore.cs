using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class UnholyCore : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
			ItemID.Sets.SortingPriorityMaterials[Type] = 90; // Chlorophyte Ore
        }

        public override void SetDefaults()
        {
            Item.width = 15;
            Item.height = 12;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(silver: 80);
            Item.rare = ItemRarityID.Pink;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<InfernalSuevite>(4).
                AddIngredient(ItemID.Hellstone, 4).
                AddTile(TileID.Hellforge).
                Register();
        }
    }
}
