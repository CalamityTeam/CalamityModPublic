using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeExoMechs")]
    public class LoreExoMechs : LoreItem
    {
        public override string Lore =>
@"What a terrifying marvel of engineering. Draedon's specialty always lay in the machines of war, but these are immaculate.
His bold claim that no God can match his work is, however, incorrect. He is not privy to the Traitor almighty.
Regardless, from steel and wit alone, he has forged engines of destruction that rival Calamitas.
It brings me little comfort to remark that even she, at least, has a heart to speak of.
Draedon is an amoral monster beyond compare. He is entirely devoid of humanity and compassion.
With technology this incomprehensibly advanced, he stands at the precipice of apotheosis.
He can fabricate such dreadful, synthetic nightmares at will. His resources must be nigh unlimited.
Were he to lose his temper, if he even has one, all of life's hopes would be smothered in an instant, silenced by a torrent of silicon.
Though, perhaps you may leverage his unimaginable craft to your advantage, and seek insight from him.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Exo Mechanical Trio");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Violet>();
        }


        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ArtemisTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<ApolloTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<ThanatosTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<AresTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
