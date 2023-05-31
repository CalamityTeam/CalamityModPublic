using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreEmpressofLight : LoreItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.FairyQueenTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
