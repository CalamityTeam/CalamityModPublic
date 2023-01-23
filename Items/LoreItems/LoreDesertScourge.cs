using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDesertScourge")]
    public class LoreDesertScourge : LoreItem
    {
        public override string Lore =>
@"Once, it was a majestic sea serpent that threatened none but the microscopic creatures it consumed.
After its home was incinerated, it became familiar with the hunt. To survive, it quickly learned to seek greater prey.
Unfortunately for the scourge, it seems that it too was prey in the end. After all, there is always a bigger fish in the sea.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Desert Scourge");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DesertScourgeTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
