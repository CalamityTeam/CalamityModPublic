using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeAstrumAureus")]
    public class LoreAstrumAureus : LoreItem
    {
        public override string Lore =>
@"Ever pragmatic, Draedon dispatched this machine to locate and analyze the source of the Astral Infection.
While nominally for reconnaissance, the Aureus model is heavily armed and can scale any terrain.
It performed admirably, at least until it was assimilated into the Infection.
Sapient minds have enough willpower to resist the Infection's call indefinitely.
However, even the finest silicon is not beyond its reach. Draedon prefers his creations to serve, after all.
With this experiment concluded, he will certainly be examining you next. Watch yourself.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Astrum Aureus");
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
                AddIngredient<AstrumAureusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
