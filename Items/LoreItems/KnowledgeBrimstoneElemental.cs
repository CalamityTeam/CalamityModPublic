using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgeBrimstoneElemental : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Brimstone Elemental");
            Tooltip.SetDefault("The most powerful of the elementals, bent on exacting revenge upon the bloody inferno that demolished her home.\n" +
                "Finally put to rest, she will suffer no longer from the grief caused by the deaths of her people.");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Pink;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddTile(TileID.Bookcases).AddIngredient(ModContent.ItemType<BrimstoneElementalTrophy>()).AddIngredient(ModContent.ItemType<VictoryShard>(), 10).Register();
        }
    }
}
