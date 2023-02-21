using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeEaterofWorlds")]
    public class LoreEaterofWorlds : LoreItem
    {
        public override string Lore =>
@"Any powerful being will call forth fable and legend, of both its grandeur and terror. That monstrous worm was no exception.
That measly thing, devouring a planet? Ridiculous. However, ridicule spreads quickly with even an ounce of truth behind it.
One will not need to search long for examples. We are all surrounded by rampant superstition and assumption.
I myself have been subjected to a litany of baseless boasts and accusations in my time.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Eater of Worlds");
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
