using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeExoMechs : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Exo Mechanical Trio");
            Tooltip.SetDefault("The fruits of masterful craftsmanship and optimization, created only with the objective to destroy.\n" +
                "Yet in the end, they achieved little more than the original designs they were derived from.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.consumable = false;
            Item.Calamity().customRarity = CalamityRarity.Violet;
        }

        public override bool CanUseItem(Player player) => false;


        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<ArtemisTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<ApolloTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<ThanatosTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<AresTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
