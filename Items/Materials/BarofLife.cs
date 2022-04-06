using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Materials
{
    public class BarofLife : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Life Alloy");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.sellPrice(gold: 3);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes()
        {
            CreateRecipe().
                AddIngredient<VerstaltiteBar>().
                AddIngredient<DraedonBar>().
                AddIngredient<CruptixBar>().
                AddTile(TileID.MythrilAnvil).
                Register();
        }
    }
}
