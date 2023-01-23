using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeDukeFishron")]
    public class LoreDukeFishron : LoreItem
    {
        public override string Lore =>
@"Outlandish as they may seem, this species is the single mightiest of the seas.
They are relentless hunters and can easily spend significant time out of the water.
Folklore holds that the Fishrons claim heritage from the true Dragons, countless years back.
While there are many such tales of creatures of draconic descent, this case is factual.
Genetic heritage or not, though, the Fishrons lack Dragonblood, or Auric souls. I would well know.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Duke Fishron");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.DukeFishronTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
