using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeSlimeGod")]
    public class LoreSlimeGod : LoreItem
    {
        public override string Lore =>
@"An old clan once revered this thing as a paragon of the balance of nature. Now its purity is sullied by freshly absorbed muck and grime.
The gelatinous being neither knows nor cares for the last surviving clansman.
Such tragedy is all too common in worship.
Alas, the Slime God is wise enough to be cowardly, fleeing battles it cannot win when its servants are destroyed.
Perhaps fortune will favor you if you catch it unaware.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Slime God");
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
                AddIngredient<SlimeGodTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
