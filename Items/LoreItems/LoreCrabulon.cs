using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCrabulon")]
    public class LoreCrabulon : LoreItem
    {
        public override string Lore =>
@"Fungus and a sea crab. One sought a host; the other, a new home.
These mushrooms possess a disturbing amount of tenacity. Nothing that lays down to die in their domain is left to rest.
It is this sort of ghastly, forceful exertion of control over the unwilling that led me down my path.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Crabulon");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CrabulonTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
