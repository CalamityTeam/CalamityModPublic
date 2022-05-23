using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeProvidence : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Providence, the Profaned Goddess");
            Tooltip.SetDefault("A core surrounded by stone and flame, a simple origin and a simple goal.\n" +
                "What would have become of us had she not been defeated is a frightening concept to consider.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.consumable = false;
            Item.Calamity().customRarity = CalamityRarity.Turquoise;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<ProvidenceTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
