using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace CalamityMod.Items.Placeables
{
    public class TranquilityCandle : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tranquility Candle");
            Tooltip.SetDefault("The mere presence of this candle calms surrounding enemies drastically");
        }

        public override void SetDefaults()
        {
            item.width = 16;
            item.height = 20;
            item.maxStack = 99;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.consumable = true;
            item.value = 500;
            item.createTile = ModContent.TileType<Tiles.TranquilityCandle>();
        }

        public override void HoldItem(Player player)
        {
            player.Calamity().tranquilityCandle = true;
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PeaceCandle, 3);
            recipe.AddIngredient(ItemID.SoulofLight, 3);
            recipe.AddIngredient(ItemID.PixieDust, 4);
            recipe.AddIngredient(null, "ZenPotion");
            recipe.SetResult(this, 1);
            recipe.AddTile(18);
            recipe.AddRecipe();
        }
    }
}
