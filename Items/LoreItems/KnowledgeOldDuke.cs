using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeOldDuke : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Old Duke");
            Tooltip.SetDefault("Strange, to find out that the mutant terror of the seas was not alone in its unique biology.\n" +
                "Perhaps I was mistaken to classify the creature from its relation to pigrons alone.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.Calamity().customRarity = CalamityRarity.PureGreen;
            item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            ModRecipe r = new ModRecipe(mod);
            r.SetResult(this);
            r.AddTile(TileID.Bookcases);
            r.AddIngredient(ModContent.ItemType<OldDukeTrophy>());
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
