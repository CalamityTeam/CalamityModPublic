using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeLunaticCultist : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lunatic Cultist");
            Tooltip.SetDefault("The gifted one that terminated my grand summoning so long ago with his uncanny powers over the arcane.\n" +
                "Someone I once held in such contempt for his actions is now... deceased, his sealing ritual undone... prepare for the end.\n" +
                "Your impending doom approaches...");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Cyan;
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
            r.AddIngredient(ItemID.AncientCultistTrophy);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
