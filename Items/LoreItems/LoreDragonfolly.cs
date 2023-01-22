using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBumblebirb", "KnowledgeDragonfolly")]
    public class LoreDragonfolly : LoreItem
    {
        public override string Lore =>
@"Near the close of the Draconic Era, there are records of the “impure” Dragon species.
Wyverns, basilisks, Pigrons and the like are documented, though none are sure how exactly they came to be.
To this day, scholars argue over the true names and lineages of these creatures.
Names aside, it is clear the first offshoots are pure enough to retain the great strength of their forebears.
Naturally, this led them to be targeted by cruel divine mandates, and most were hunted to extinction.
It is known that Fishrons, Follies, and the Abyssal Wyrms survived the purging hunts of the Deific Era.
Notably, they now are all reclusive or exceedingly violent. It is tragic how they evolved to be that way.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Dragonfolly");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Purple;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<DragonfollyTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
