using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDestroyer")]
    public class LoreDestroyer : LoreItem
    {
        public override string Lore =>
@"The Godseeker Knights of my company were far and away my finest soldiers.
They championed my cause, and I championed them in return.
I bestowed upon them hulking armor and colossal weaponry, so their Might would never falter.
Some days, I would take time to train by their side, inspiring them to new heights of righteous fury.
Draedon understood well, and granted them these massive forms, bristling with weaponry and interlocked armor forged of blessed metal.
While in truth it was repurposed mining equipment, their sheer presence on the battlefield was immense.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Destroyer");
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
                AddIngredient(ItemID.DestroyerTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
