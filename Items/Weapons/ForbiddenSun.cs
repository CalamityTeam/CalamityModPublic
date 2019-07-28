using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class ForbiddenSun : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Forbidden Sun");
			Tooltip.SetDefault("Casts a fire orb that emits a gigantic explosion on death");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 80;
	        item.magic = true;
	        item.mana = 33;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 30;
	        item.useAnimation = 30;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7f;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ForbiddenSunProjectile");
	        item.shootSpeed = 9f;
	    }

	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CruptixBar", 6);
			recipe.AddIngredient(ItemID.LivingFireBlock, 50);
	        recipe.AddTile(TileID.Bookcases);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
