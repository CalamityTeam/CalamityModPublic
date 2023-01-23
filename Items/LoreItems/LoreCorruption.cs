using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCorruption")]
    public class LoreCorruption : LoreItem
    {
        public override string Lore =>
@"To not properly dispose of the essence of a slain God is a fatal mistake. This wasteland stands as proof of such.
Having slain my first Gods, I turned a blind eye as corrupt essence gushed from their rent forms and burrowed into the bowels of Terraria.
The mere existence of this putrid place proves that the Gods of old were beyond redemption.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Corruption");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Green;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EaterofWorldsTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
