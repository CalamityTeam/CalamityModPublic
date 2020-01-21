using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class GodlySoulArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Godly Soul Artifact");
            Tooltip.SetDefault("Loyalty\n" +
                "Summons two Sons of Yharon to fight for you");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.value = Item.buyPrice(1, 50, 0, 0);
            item.accessory = true;
            item.rare = 10;
            item.Calamity().postMoonLordRarity = 16;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gArtifact = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<Cinderplate>(), 5);
            recipe.AddIngredient(ModContent.ItemType<EssenceofCinder>(), 10);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 10);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 15);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
