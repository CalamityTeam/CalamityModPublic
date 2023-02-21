using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeQueenBee")]
    public class LoreQueenBee : LoreItem
    {
        public override string Lore =>
@"While of tremendous size, these creatures are docile until aggravated. Their idyllic demeanor is a rarity nowadays, a thing of beauty.
In the past, entire villages would spring up around these grand hives, peacefully harvesting their share of the honey and protecting them from danger.
Though its death is understandable given the circumstances, I do feel pity for these majestic beings.
Fate was cruel to many of their kind.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Queen Bee");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.QueenBeeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
