using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("BarofLife")]
    public class LifeAlloy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Life Alloy");
			ItemID.Sets.SortingPriorityMaterials[Type] = 95; // Stardust Fragment
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Yellow;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CryonicBar>().
                AddIngredient<PerennialBar>().
                AddIngredient<ScoriaBar>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
