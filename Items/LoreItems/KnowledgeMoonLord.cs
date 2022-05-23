using CalamityMod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeMoonLord : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moon Lord");
            Tooltip.SetDefault("What a waste.\n" +
                "Had it been fully restored it would have been a force to behold, but what you fought was an empty shell.\n" +
                "However, that doesn't diminish the immense potential locked within it, released upon its death.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Red;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ItemID.MoonLordTrophy).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
