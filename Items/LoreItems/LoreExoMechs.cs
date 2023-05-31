using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeExoMechs")]
    public class LoreExoMechs : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Violet>();
        }


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
