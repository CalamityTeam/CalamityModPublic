using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Rarities;
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
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.consumable = false;
            Item.rare = ModContent.RarityType<Turquoise>();
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<CeaselessVoidTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<WeaverTrophy>().
                AddTile(TileID.Bookcases).
                Register();
            CreateRecipe().
                AddIngredient<SignusTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
