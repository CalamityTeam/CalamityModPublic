using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Placeables.FurnitureBotanic
{
	public class BotanicCandelabra : ModItem
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
			item.createTile = mod.TileType("BotanicCandelabra");
		}

		public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(null, "UelibloomBrick", 5);
            recipe.AddIngredient(ItemID.JungleSpores, 3);
            recipe.SetResult(this, 1);
            recipe.AddTile(null, "BotanicPlanter");
            recipe.AddRecipe();
        }
	}
}