using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeOldDuke")]
    public class LoreOldDuke : LoreItem
    {
        public override string Lore =>
@"That was possibly one of the oldest mundane living beings on the face of the planet.
The first Fishrons were spotted in the middle of the Draconic Era. What exotic prestige…!
Fishrons were one of the original offshoots of pure-blooded Auric Dragons.
They are so old and venerated that many historians are convinced they are the original sea monsters of folklore.
This particular Duke's guile is self-evident; it evaded centuries of hunting, and until now had survived a most thorough poisoning.
Above almost all others, this creature was a living fable. One must wonder what goes through the mind of a fading legend.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Old Duke");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ModContent.RarityType<PureGreen>();
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<OldDukeTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
