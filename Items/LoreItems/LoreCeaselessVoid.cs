using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSentinels")]
    public class LoreCeaselessVoid : LoreItem
    {
        public override string Lore =>
@"A contained, previously inert portal sealed in the Dungeon. The presiding cult kept it a closely guarded secret.
Upon sighting the Devourer of Gods, their leader hurriedly led me to its chamber to reveal its existence to me.
The portal led to the Devourer's home. It was identical to his, only ancient and perfectly stable.
The serpent claimed it, too, was his creation. Its permanence was a mistake he later rectified.
But this rift was unquestionably far older than he. It dated back to the Golden Age of Dragons.
His lie was thin and forced. Something far more powerful than the Devourer was at hand.
Its eerie persistence gnawed at my mind. It did not just threaten me. It threatened everyone. Everything.
Even faced with such lies and eminent danger, I simply walked away, and did not return.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Ceaseless Void");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CeaselessVoidTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
