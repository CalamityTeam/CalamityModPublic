using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Potions;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.SummonItems
{
    public class Starcore : ModItem
    {
        public override void SetStaticDefaults()
        {
           			ItemID.Sets.SortingPriorityBossSpawns[Type] = 16; // Solar Tablet / Bloody Tear
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 40;
            Item.rare = ItemRarityID.Cyan;
        }

		public override void ModifyResearchSorting(ref ContentSamples.CreativeHelper.ItemGroup itemGroup)
		{
			itemGroup = ContentSamples.CreativeHelper.ItemGroup.BossItem;
		}

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<Stardust>(25).
                AddIngredient<AureusCell>(8).
                AddIngredient<AstralBar>(4).
                AddTile(TileID.LunarCraftingStation).
                Register();
        }
    }
}
