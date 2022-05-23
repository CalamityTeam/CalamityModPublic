using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeOcean : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Ocean");
            Tooltip.SetDefault("Take care to not disturb the deep waters of this world.\n" +
                "You may awaken something more terrifying than death itself.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Lime;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player) => false;

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<LeviathanTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
