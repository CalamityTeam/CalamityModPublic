using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgePerforators")]
    public class LorePerforators : LoreItem
    {
        public override string Lore =>
@"These creatures were unique, for they wielded the slain Gods’ power as purely as possible, veins flowing with spilt ichor.
All that exists in the Crimson is truly the divine turned inside out; their gore now glistens with its perverse treachery, for all to bear witness.
The mire reeks of centuries of vile manipulation and callous domination of the hapless.
Judgment is long passed, and extinction is left waiting.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Perforators and Their Hive");
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
                AddIngredient<PerforatorTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
