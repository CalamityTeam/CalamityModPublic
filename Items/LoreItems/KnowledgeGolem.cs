using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeGolem : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Golem");
            Tooltip.SetDefault("A primitive construct.\n" +
                "I admire the lihzahrd race for their ingenuity, though finding faith in such a flawed idol would invariably lead to their downfall.");
        }

        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.rare = ItemRarityID.Yellow;
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
            r.AddIngredient(ItemID.GolemTrophy);
            r.AddIngredient(ModContent.ItemType<VictoryShard>(), 10);
            r.AddRecipe();
        }
    }
}
