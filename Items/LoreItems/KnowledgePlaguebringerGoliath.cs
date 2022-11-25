using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.LoreItems
{
    public class KnowledgePlaguebringerGoliath : LoreItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Plaguebringer Goliath");
            Tooltip.SetDefault("A horrific amalgam of steel, flesh, and infection, capable of destroying an entire civilization in just one onslaught.\n" +
                "Its plague nuke barrage can leave an entire area uninhabitable for months. A shame that it came to this but the plague must be contained.");
            SacrificeTotal = 1;
            ItemID.Sets.ItemNoGravity[Item.type] = true;
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.consumable = false;
        }

        public override bool CanUseItem(Player player)
        {
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PlaguebringerGoliathTrophy>().
                AddTile(TileID.Bookcases).
                Register();
        }
    }
}
