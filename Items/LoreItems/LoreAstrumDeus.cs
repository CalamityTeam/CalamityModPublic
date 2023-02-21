using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAstrumDeus")]
    public class LoreAstrumDeus : LoreItem
    {
        public override string Lore =>
@"On our world, this being is revered as the God of the night sky. It is said to devour dying stars and birth new ones in turn.
Unlike the many Gods you or I know, it is guiltless. An important distinction, for it was equally as diseased as they.
The infection that tainted its body is from beyond Terraria. Neither I nor Draedon recognize it fully.
With its will subsumed, it hurled a chunk of infested astral matter at our world, then came to guard it.
Thankfully, such a grandiose being that walks amongst the stars is likely not truly dead.
While the land has paid a terrible price, the price of a wrongful conviction is higher still.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Astrum Deus");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Cyan;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AstrumDeusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
