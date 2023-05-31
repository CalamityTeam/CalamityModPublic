using CalamityMod.Items.Materials;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class LoreCynosure : LoreItem
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
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<ShadowspecBar>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
