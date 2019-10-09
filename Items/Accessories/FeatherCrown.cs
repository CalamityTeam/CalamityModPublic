using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.CalPlayer;

namespace CalamityMod.Items.Accessories
{
    public class FeatherCrown : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Feather Crown");
			Tooltip.SetDefault("15% increased rogue projectile velocity\n" +
                "Stealth strikes cause feathers to fall from the sky on enemy hits");
		}

		public override void SetDefaults()
		{
			item.width = 20;
			item.height = 20;
            item.value = Item.buyPrice(0, 0, 80, 0);
            item.rare = 4;
			item.accessory = true;
		}

		public override void UpdateAccessory(Player player, bool hideVisual)
		{
            CalamityPlayer modPlayer = player.GetModPlayer<CalamityPlayer>(mod);
            modPlayer.throwingVelocity += 0.15f;
            modPlayer.featherCrown = true;
		}

        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.GoldCrown);
            recipe.AddIngredient(mod.ItemType("AerialiteBar"), 6);
            recipe.AddIngredient(ItemID.Feather, 8);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
            recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.PlatinumCrown);
            recipe.AddIngredient(mod.ItemType("AerialiteBar"), 6);
            recipe.AddIngredient(ItemID.Feather, 8);
            recipe.AddTile(TileID.SkyMill);
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
    }
}
