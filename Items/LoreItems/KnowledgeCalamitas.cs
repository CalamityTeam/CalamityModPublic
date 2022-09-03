using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeCalamitas : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Calamitas");
            Tooltip.SetDefault("The witch unrivaled. Perhaps the only one amongst my cohorts to have ever given me cause for doubt.\n" +
                "Now that you have defeated her your destiny is clear.\n" +
                "Come now, face me.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Violet>();
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<SupremeCalamitasTrophy>()).AddIngredient(ModContent.ItemType<PearlShard>(), 10).Register();
        }
    }
}
