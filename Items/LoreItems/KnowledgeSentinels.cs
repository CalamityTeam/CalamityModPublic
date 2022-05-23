using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeSentinels : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Sentinels of the Devourer");
            Tooltip.SetDefault("Signus. The Void. The Weaver.\n" +
                "Each represent one of the Devourer’s largest spheres of influence.\n" +
                "Dispatching them has most likely invoked its anger and marked you as a target for destruction.");
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
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<CeaselessVoidTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<WeaverTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<SignusTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
