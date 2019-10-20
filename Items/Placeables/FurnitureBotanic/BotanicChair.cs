using Terraria.ModLoader;
namespace CalamityMod.Items.Placeables.FurnitureBotanic
{
    public class BotanicChair : ModItem
    {
        public override void SetStaticDefaults()
        {
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
            item.createTile = ModContent.TileType<Tiles.FurnitureBotanic.BotanicChair>();
        }

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ModContent.ItemType<UelibloomBrick>(), 4);
            recipe.SetResult(this, 1);
            recipe.AddTile(ModContent.TileType<Tiles.FurnitureBotanic.BotanicPlanter>());
            recipe.AddRecipe();
        }
    }
}
