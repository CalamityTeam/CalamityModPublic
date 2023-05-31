using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreAwakening : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.ArmorStatue).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
