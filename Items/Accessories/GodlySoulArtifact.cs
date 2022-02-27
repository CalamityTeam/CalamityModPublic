using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
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
                "For each Fiery Draconid you have summoned, you gain 1 minion slot");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 28;
            item.accessory = true;
            item.rare = ItemRarityID.Red;
            item.Calamity().customRarity = CalamityRarity.Violet;
            item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gArtifact = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<ExodiumClusterOre>(), 25);
            recipe.AddIngredient(ModContent.ItemType<PlagueContainmentCells>(), 25);
            recipe.AddIngredient(ModContent.ItemType<HellcasterFragment>(), 5);
            recipe.AddTile(TileID.DemonAltar);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
