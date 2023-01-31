using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class LoreStormWeaver : LoreItem
    {
        public override string Lore =>
@"This beast, while of lesser stature than the Devourer, is a great danger in its own right.
They are clearly of the same species. Even this serpent was known to devour Wyverns whole.
Very little is known about the realm or space that the Great Devourer hails from.
Even Draedon and his obsessive research has been unable to discern its true nature.
The Weaver slipped through a rift from this place opened by the Devourer, and he has monitored it since.
In his mind, the lesser serpent's similar powers could lead it to be too threatening for him to let live.
He thinks himself invincible. Little does he know, he has ever stood in a similar position.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Storm Weaver");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<WeaverTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
