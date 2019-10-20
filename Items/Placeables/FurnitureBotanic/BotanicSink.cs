using Terraria.ID;
using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureBotanic
{
    public class BotanicSink : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Counts as a honey source");
        }

        public override void SetDefaults()
        {
            item.width = 28;
            item.height = 20;
            item.maxStack = 999;
            item.useTurn = true;
            item.autoReuse = true;
            item.useAnimation = 15;
            item.useTime = 10;
            item.useStyle = 1;
            item.value = 0;
            item.consumable = true;
            item.createTile = ModContent.TileType<Tiles.FurnitureBotanic.BotanicSink>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UelibloomBrick>(), 6);
            recipe.AddIngredient(ItemID.HoneyBucket);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<Tiles.FurnitureBotanic.BotanicPlanter>());
            recipe.AddRecipe();
        }
    }
}
