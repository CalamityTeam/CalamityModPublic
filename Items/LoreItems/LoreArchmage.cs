using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCryogen")]
    public class LoreArchmage : LoreItem
    {
        public override string Lore =>
@"He yet lives…?! I thought him slain by Calamitas. It appears she imprisoned the Archmage to spare his life.
I assumed that frigid mass was an old construct of his, running amok without its master to shepherd it.
Permafrost was an old ally of mine, wielding the prestigious title of Archmage and great renown.
His wisdom guided my original conquests, or rather, made much of them possible at all.
As my crusade evolved and my ambitions grew, he expressed vehement disapproval.
Where once he saw justice, there was now tyranny. He departed, and the Witch not long after.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Archmage");
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
                AddIngredient<CryogenTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
