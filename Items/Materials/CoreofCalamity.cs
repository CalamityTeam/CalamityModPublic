using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class CoreofCalamity : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Core of Calamity");
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 40;
			item.maxStack = 99;
			item.value = Item.buyPrice(0, 5, 0, 0);
			item.rare = 8;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			maxFallSpeed = 0f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CoreofCinder", 3);
			recipe.AddIngredient(null, "CoreofEleum", 3);
			recipe.AddIngredient(null, "CoreofChaos", 3);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
