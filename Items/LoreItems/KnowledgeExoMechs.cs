using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
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
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool CanUseItem(Player player) => false;


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArtemisTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<ApolloTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<ThanatosTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<AresTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
