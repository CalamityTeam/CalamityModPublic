using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBrimstoneCrag")]
    public class LoreAzafure : LoreItem
    {
        public override string Lore =>
@"Oft called the First City, its tumultuous history stretches back to the Draconic Era.
An odd jewel of civilization, the immense heat of the underworld provided it unlimited potential in defense and industry.
Such was the renown of the forgemasters that when I swayed them to my cause, I was never lacking for arms.
It pains me to say even in hindsight, but their artisanry paved the downfall of the entire city.
For the Witch and I, the air here will forever be laden with regret. There is nothing to be done.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Azafure");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<BrimstoneElementalTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
