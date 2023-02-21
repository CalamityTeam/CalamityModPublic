using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCrimson")]
    public class LoreCrimson : LoreItem
    {
        public override string Lore =>
@"The foul air, the morbid fauna, the disgusting terrain… Here lies my first mistake of my crusade.
The essence of a God does not simply vanish when the body dies. It must be properly disposed of or destroyed entirely.
Essence of a pious God could never fester into a mire as dreadful as this.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("The Crimson");
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
