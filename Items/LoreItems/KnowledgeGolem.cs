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
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.GolemTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
