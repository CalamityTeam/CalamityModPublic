using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeKingSlime : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slime");
            Tooltip.SetDefault("Only a fool could be caught by this pitiful excuse for a hunter.\n" +
                "Unfortunately, our world has no shortage of those.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.KingSlimeTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
