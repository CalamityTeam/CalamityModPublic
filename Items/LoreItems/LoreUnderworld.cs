using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeUnderworld")]
    public class LoreUnderworld : LoreItem
    {
        public override string Lore =>
@"The hellish reputation the underworld gets is rather a modern thing.
The layers of ash choking the previously great cities are still warm.
The more domineering of Gods wished for me to champion their cause, and rule their society from here.
Yet, surrounded by magma as it were, Azafure simply burned when their wishes were not met.
Such is the unfortunate price of war, though I have no regrets fighting for my people.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Underworld");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightRed;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.WallofFleshTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
