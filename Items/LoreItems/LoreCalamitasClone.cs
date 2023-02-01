using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    [LegacyName("KnowledgeCalamitasClone")]
    public class LoreCalamitasClone : LoreItem
    {
        public override string Lore =>
@"I had seen this monster roaming the night in the past, and thought nothing of it.
With its technology, it was certainly one of Draedon’s creations.
But, to think it was housing a clone of the Witch… Detestable.
Surely Calamitas would want nothing to do with such a project.
I know not how it wields her brimstone magic. Perhaps some day one of us may find answers.";

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            DisplayName.SetDefault("Calamitas Clone");
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
                AddIngredient<CalamitasCloneTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
