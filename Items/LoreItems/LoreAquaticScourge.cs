using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAquaticScourge")]
    public class LoreAquaticScourge : LoreItem
    {
        public override string Lore =>
@"Another once grand sea serpent, well-adapted to its harsh environs.
Unlike the other Scourge, which was half starved and chasing scraps for its next meal, it lived comfortably.
Microorganisms evolve rapidly, so it was able to maintain its filter feeding habits as the sea putrefied.
What a stark contrast to the rest of the ecosystem. Nearly every other creature in the Sulphur Sea is hostile.
A shame that its last bastion of tranquility has fallen.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Aquatic Scourge");
        }

        public override void SetDefaults()
        {
            Item.width = 38;
            Item.height = 26;
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
