using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSulphurSea")]
    public class LoreSulphurSea : LoreItem
    {
        public override string Lore =>
@"This seaside has never been pleasant, though it has seen far better days.
Incessant fumes rising from the industry of Azafure inundate the water with caustic ions.
Yet still, the hardy life adapted. No doubt aided by Silva as she burrowed through to the underworld.
Long considered uninhabitable, its further deterioration led Draedon to designate it as a dumping ground.
Years of careless mass waste disposal has now left the coast’s transformation irreversible.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Sulphur Sea");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<AquaticScourgeTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
