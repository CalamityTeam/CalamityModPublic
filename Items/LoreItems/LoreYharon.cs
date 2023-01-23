using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeYharon")]
    public class LoreYharon : LoreItem
    {
        public override string Lore =>
@"The return of the Age of Dragons, dashed. Just like that, it is but ashes in the wind…
Yharon was the last of the Auric Dragons. As a phoenix, his domain of power includes rebirth.
The Gods thought him culled with the rest of his kind, but he returned as an egg, hidden on the Aerie.
I was destined to consume his Auric soul when he hatched, and rule forever as God-King.

[c/FCA92B:Destiny is for the weak.]

I rejected their whims, and upended their scheme. I was sentenced to execution for treason.
Their meek, ingratiated swine cast both Yharon’s egg and I into the magma of Hell.
The intense heat hideously scarred me, but birthed Yharon anew. He rose, wreathed in fire, and saved my life.
From that day, our souls were one. He shared with me the tale of Zeratros, and the genocide of his kind.
I promised him I would have justice. So the war began, Yharon rallying all as a beacon of hope.
Now, that hope is long withered. I am but a husk of the hero I once was, and this is the ultimate proof.
Yharon may yet return, as he does, but he… he has bade me farewell.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Yharon, Resplendent Phoenix");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<DarkBlue>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<YharonTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
