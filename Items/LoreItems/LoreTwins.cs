using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeTwins")]
    public class LoreTwins : LoreItem
    {
        public override string Lore =>
@"Not all of warfare is open combat. Logistics and intelligence are paramount to decisive victory.
These machines are my finest scouts and agents, reborn in a form that gives them Sight unrivaled.
Archers or snipers, spies or assassins. An enemy is only as safe as you allow him to be.
Draedon understood well that the only fair fight is the one you win.
His assistance was infallible, and his calculus cold and cruel.
Not even the most evasive target stood a chance.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Twins");
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
                AddIngredient(ItemID.RetinazerTrophy).
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient(ItemID.SpazmatismTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
