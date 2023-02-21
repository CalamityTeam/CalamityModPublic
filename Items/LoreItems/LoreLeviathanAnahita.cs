using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeLeviathanandSiren", "KnowledgeLeviathanAnahita")]
    public class LoreLeviathanAnahita : LoreItem
    {
        public override string Lore =>
@"Although she claims dominion over all the world's oceans, in truth she is a recluse of the deep.
Elementals pose a grave threat to all those around them. Other Elementals are no exception.
Anahita was driven from her home in the Abyss by Silva's encroaching greenery.
Accounts vary as to the majestic beast at her side. Some claim Anahita summoned the Leviathan herself.
Regardless of what you believe, they are inseparable even in death.
Such stalwart loyalty! It reminds me of Yharon.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Leviathan and Anahita");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<LeviathanTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<AnahitaTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
