using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeHiveMind")]
    public class LoreHiveMind : LoreItem
    {
        public override string Lore =>
@"Some semblance of a God's mind may survive death, like the twitches of a crushed insect.
What little remains attempts to convene, to coalesce in worship, so that its power may yet be restored. How pitiful.
Fortunately for us, the futility of this effort is unmatched. The biomass obeys, but nothing is accomplished.
Far from all divine power flows from faith. A God is forged of its own strength; followers may choose to worship.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Hive Mind");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<HiveMindTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
