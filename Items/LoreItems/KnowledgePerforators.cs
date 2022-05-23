using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePerforators : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforators and their Hive");
            Tooltip.SetDefault("An abomination of comingled flesh, bone, and organ, infested primarily by blood-slurping worms.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Orange;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<PerforatorTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
