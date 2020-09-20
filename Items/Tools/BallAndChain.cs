using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Tools
{
	public class BallAndChain : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ball and Chain");
			Tooltip.SetDefault("So heavy...\n" +
				"Favorite this item to disable any dashes granted by equipment.");
		}

		public override void SetDefaults()
		{
			item.width = 32;
			item.height = 50;
			item.rare = ItemRarityID.Blue;
		}

		public override bool CanUseItem(Player player) => false;

		public override void UpdateInventory(Player player)
		{
			if (item.favorited)
				player.Calamity().blockAllDashes = true;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.IronBar, 10);
			recipe.AddIngredient(ItemID.Chain, 1);
			recipe.AddTile(TileID.Anvils);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}
	}
}
