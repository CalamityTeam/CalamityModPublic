using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreAwakening : LoreItem
    {
        public override string Lore =>
@"The tombs of the Dragons stir. My eyes lift to see ancient dust dancing from high ledges.
These grand wings… how long has it been since I was a hero worthy of their name?
It feels like centuries have passed, yet all I've done is blink.
Look upon my works, as they are… Ruined. None would dare seek me out; tread my path.
Naught awaits them in this cruel world.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Awakening");
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
