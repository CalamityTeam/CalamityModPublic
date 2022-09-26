using CalamityMod.Tiles.Ores;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.Ores
{
    [LegacyName("ExodiumClusterOre")]
    public class ExodiumCluster : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
            DisplayName.SetDefault("Exodium Cluster");
            Tooltip.SetDefault("A cold cluster from the great unknown.");
			ItemID.Sets.SortingPriorityMaterials[Type] = 101;
        }

        public override void SetDefaults()
        {
            Item.createTile = ModContent.TileType<ExodiumOre>();
            Item.useStyle = ItemUseStyleID.Swing;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.autoReuse = true;
            Item.consumable = true;
            Item.width = 13;
            Item.height = 10;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 1, silver: 60);
            Item.rare = ItemRarityID.Red;
        }

        public override void AddRecipes()
        {
            CreateRecipe(1).AddIngredient(ItemID.LunarOre, 3).AddIngredient(ItemID.FragmentStardust).AddIngredient(ItemID.FragmentSolar).AddIngredient(ItemID.FragmentVortex).AddIngredient(ItemID.FragmentNebula).AddTile(TileID.LunarCraftingStation).Register();
        }
    }
}
