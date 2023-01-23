using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeEyeofCthulhu")]
    public class LoreEyeofCthulhu : LoreItem
    {
        public override string Lore =>
@"In ages past, heroes made names for themselves facing such monsters.
Now they run rampant, spawning from vile influences left unchecked. They blend well with the horrific injustice of their forebears.
Slaying one merely paves the way for a dozen more. Surely this does not concern you, either.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Eye of Cthulhu");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient(ItemID.EyeofCthulhuTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
