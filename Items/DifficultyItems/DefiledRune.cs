using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.DifficultyItems
{
	public class DefiledRune : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Defiled Feather");
			Tooltip.SetDefault("It's a long way down from a mountain...\n" +
				"Favorite this item to disable wings.");
			Main.RegisterItemAnimation(item.type, new DrawAnimationVertical(7, 8));
		}

		public override void SetDefaults()
		{
			item.width = 40;
			item.height = 50;
			item.rare = ItemRarityID.Green;
		}

		public override bool CanUseItem(Player player) => false;

		public override void UpdateInventory(Player player)
		{
			if (item.favorited)
				player.Calamity().noWings = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Feather);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
