using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
    public class Photoviscerator : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Photoviscerator");
			Tooltip.SetDefault("90% chance to not consume gel\n" +
                "Fires a stream of exo flames that literally melts everything");
		}

	    public override void SetDefaults()
	    {
			item.damage = 250;
			item.ranged = true;
			item.width = 84;
			item.height = 30;
			item.useTime = 2;
			item.useAnimation = 10;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 2f;
			item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(2, 50, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
			item.shoot = mod.ProjectileType("ExoFire");
			item.shootSpeed = 6f;
			item.useAmmo = 23;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 15;
		}

        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-15, 0);
        }

        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
			for (int i = 0; i < 2; i++)
			{
				float SpeedX = speedX + (float)Main.rand.Next(-5, 6) * 0.05f;
				float SpeedY = speedY + (float)Main.rand.Next(-5, 6) * 0.05f;
				Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0f, 0f);
			}
            return false;
        }

        public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 90)
	    		return false;
	    	return true;
	    }

	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "ElementalEruption");
	        recipe.AddIngredient(null, "CleansingBlaze");
	        recipe.AddIngredient(null, "HalleysInferno");
            recipe.AddIngredient(null, "NightmareFuel", 5);
            recipe.AddIngredient(null, "EndothermicEnergy", 5);
            recipe.AddIngredient(null, "CosmiliteBar", 5);
            recipe.AddIngredient(null, "DarksunFragment", 5);
            recipe.AddIngredient(null, "HellcasterFragment", 3);
            recipe.AddIngredient(null, "Phantoplasm", 5);
            recipe.AddIngredient(null, "AuricOre", 25);
            recipe.AddTile(null, "DraedonsForge");
            recipe.SetResult(this);
            recipe.AddRecipe();
        }
	}
}
