using Terraria;
using Terraria.ID;

namespace CalamityMod.Items.LoreItems
{
    public class LoreQueenSlime : LoreItem
    {
        public override string Lore =>
@"Having fled after your battle, it seems the Slime God fashioned a new guardian from the unleashed essences.
Ensnared in the absorption process of its newfound power, it could not flee again.
Or, perhaps, it was overcome by arrogance or desperation.
A glorious hunt, a fine foe. Now you know you must chase them to the ends of Terraria.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Queen Slime");
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
                AddIngredient(ItemID.QueenSlimeTrophy).
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
