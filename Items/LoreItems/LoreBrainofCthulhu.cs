using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeBrainofCthulhu")]
    public class LoreBrainofCthulhu : LoreItem
    {
        public override string Lore =>
@"It is true that unspeakable abominations may now be commonplace, largely by my hand.
Though they have always been a product of the folly of the Gods, the same Gods would cull them in equal measure.
My decimation of the falsely divine left many old horrors unconstrained, with new ones birthed every year.
Now, they are your stepping stones.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Brain of Cthulhu");
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
                AddIngredient(ItemID.BrainofCthulhuTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
