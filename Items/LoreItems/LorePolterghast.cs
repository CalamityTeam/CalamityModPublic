using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePolterghast")]
    public class LorePolterghast : LoreItem
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
            Item.rare = ModContent.RarityType<PureGreen>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PolterghastTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
