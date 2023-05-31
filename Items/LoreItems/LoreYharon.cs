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
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
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
