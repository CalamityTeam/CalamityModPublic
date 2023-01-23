using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeRavager")]
    public class LoreRavager : LoreItem
    {
        public override string Lore =>
@"A sickening flesh golem built for the sole purpose of savage, relentless destruction.
The monstrosity was a desperate bid to turn the tides against my God-seeking armies.
I could scarcely believe it myself, but it was born of a ritual of great sacrifice, performed in ardent faith.
The ritual condemned and fused the bodies and souls of their fallen allies into this hideous thing.
When the warlocks pledged their very lives to it as an offering, it awoke and swiftly slew them.
Now caked in fresh blood, it hungered for more, and set off on an aimless rampage.
I suppose its brutality serves as a reminder to be careful what you believe.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Ravager");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<RavagerTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
