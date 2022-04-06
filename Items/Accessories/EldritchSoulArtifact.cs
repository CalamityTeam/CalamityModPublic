using CalamityMod.CalPlayer;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Plates;
using CalamityMod.Items.Placeables.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Accessories
{
    public class EldritchSoulArtifact : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Eldritch Soul Artifact");
            Tooltip.SetDefault("Knowledge\n" +
                "Boosts melee speed by 10%, ranged velocity by 25%, rogue damage by 15%, max minions by 2, and reduces mana cost by 15%");
        }

        public override void SetDefaults()
        {
            Item.width = 64;
            Item.height = 58;
            Item.accessory = true;
            Item.value = CalamityGlobalItem.Rarity11BuyPrice;
            Item.rare = ItemRarityID.Purple;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            CalamityPlayer modPlayer = player.Calamity();
            modPlayer.eArtifact = true;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ExodiumClusterOre>(25).
                AddIngredient<Navyplate>(25).
                AddIngredient<Phantoplasm>(5).
                AddTile(TileID.DemonAltar).
                Register();
        }
    }
}
