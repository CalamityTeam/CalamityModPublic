using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items
{
    public class WulfrumHammer : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Wulfrum Hammer");
		}

		public override void SetDefaults()
		{
			item.damage = 7;
			item.melee = true;
			item.width = 56;
			item.height = 56;
			item.useTime = 29;
			item.useAnimation = 29;
			item.useTurn = true;
			item.hammer = 35;
			item.useStyle = 1;
			item.knockBack = 5.5f;
			item.value = Item.buyPrice(0, 1, 0, 0);
			item.rare = 1;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "WulfrumShard", 16);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
