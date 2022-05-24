using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeEyeofCthulhu : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Eye of Cthulhu");
            Tooltip.SetDefault("That eye... how peculiar.\n" +
                "I sensed it watching you more intensely as you grew stronger.");
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
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.EyeofCthulhuTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
