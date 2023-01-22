using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBrimstoneElemental")]
    public class LoreBrimstoneElemental : LoreItem
    {
        public override string Lore =>
@"A peculiar being. Until recently, she had laid dormant for ages, posing as the city’s silent matron.
As its economy boomed, traces of brimstone found their way all across the known world.
It was never clear why her slumber ended. At first she stirred. The people were cautiously optimistic.
When she woke, it was horrific. Her inferno billowed through the streets. None were safe.
Fate had a sick sense of humor that day, for Calamitas was there to match her.
Perhaps the two were attuned somehow. They fought to a standstill, fire against fire.
Neither were victorious, and despite her intentions, the city was razed by her flames.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Brimstone Elemental");
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
                AddIngredient<BrimstoneElementalTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
