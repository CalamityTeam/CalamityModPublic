using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class DimensionalSoulArtifact : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Dimensional Soul Artifact");
			Tooltip.SetDefault("Power\n" +
                "Boosts all damage by 25%\n" +
                "But at what cost?");
		}

		public override void SetDefaults()
		{
			item.width = 28;
			item.height = 28;
			item.value = Item.buyPrice(1, 50, 0, 0);
			item.accessory = true;
			item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 16;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.GetCalamityPlayer();
            modPlayer.dArtifact = true;
        }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Cinderplate", 5);
            recipe.AddIngredient(null, "EssenceofEleum", 10);
            recipe.AddIngredient(null, "CosmiliteBar", 10);
            recipe.AddIngredient(null, "ExodiumClusterOre", 15);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
        }
	}
}
