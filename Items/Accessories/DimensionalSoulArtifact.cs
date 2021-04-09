using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class DimensionalSoulArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dimensional Soul Artifact");
            Tooltip.SetDefault("Power\n" +
				"Boosts all damage by 25%, but at what cost?\n" +
                "Increases all damage taken by 15%");
        }

        public override void SetDefaults()
        {
			item.width = 28;
			item.height = 28;
			item.accessory = true;
			item.rare = ItemRarityID.Red;
			item.Calamity().customRarity = CalamityRarity.DarkBlue;
			item.value = CalamityGlobalItem.Rarity14BuyPrice;
		}

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.dArtifact = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 25);
			recipe.AddIngredient(ModContent.ItemType<Elumplate>(), 25);
			recipe.AddIngredient(ModContent.ItemType<CosmiliteBar>(), 5);
			recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
