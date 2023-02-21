using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePlantera")]
    public class LorePlantera : LoreItem
    {
        // Well done, you killed a plant.
        public override string Lore =>
@"This floral aberration is another example of the volatile power of harnessed souls.
Taking their mastery of agriculture to new heights, the Jungle settlers bred a special sprout.
Through ritual blessing of the soil, it was fed legions of souls.
Elders of the village wished to bring forth a new age of botanical prosperity.
Indeed, the plant was awe inspiring. But it was wild and untamed, with a will of its own.
Now that you have slain it, still more disorderly spiritual energies are flooding the lands.
The village's ignorance was shameful in its own right, but this is worse still.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Plantera");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.LightPurple;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.PlanteraTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
