using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    [LegacyName("VictideBar")]
    public class SeaRemains : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
            DisplayName.SetDefault("Sea Remains");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(silver: 6);
            Item.rare = ItemRarityID.Green;
        }
        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<PearlShard>().
                AddIngredient(ItemID.Coral).
                AddIngredient(ItemID.Starfish).
                AddIngredient(ItemID.Seashell).
                AddTile(TileID.Furnaces).
                Register();
        }
    }
}
