using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class HeartRapier : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Heart Rapier");
			Tooltip.SetDefault("Heals the player on enemy hits");
		}

		public override void SetDefaults()
		{
			item.width = 44;
			item.damage = 38;
			item.melee = true;
			item.noMelee = true;
			item.useTurn = true;
			item.noUseGraphic = true;
			item.useAnimation = 20;
			item.useStyle = 5;
			item.useTime = 20;
			item.knockBack = 5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = false;
			item.height = 44;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("HeartRapierProjectile");
			item.shootSpeed = 5f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.LifeCrystal, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
