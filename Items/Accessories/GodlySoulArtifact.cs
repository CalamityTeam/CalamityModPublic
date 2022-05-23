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
            SacrificeTotal = 1;
            DisplayName.SetDefault("Godly Soul Artifact");
            Tooltip.SetDefault("Loyalty\n" +
                "For each Fiery Draconid you have summoned, you gain 1 minion slot");
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 28;
            Item.accessory = true;
            Item.rare = ItemRarityID.Red;
            Item.Calamity().customRarity = CalamityRarity.Violet;
            Item.value = CalamityGlobalItem.Rarity15BuyPrice;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.gArtifact = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExodiumClusterOre>(25).
                AddIngredient<PlagueContainmentCells>(25).
                AddIngredient<HellcasterFragment>(5).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
