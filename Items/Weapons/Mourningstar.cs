using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class Mourningstar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Mourningstar");
			Tooltip.SetDefault("Launches a solar whip sword and a solar particle trail that explodes on enemy hits");
		}

		public override void SetDefaults()
		{
			item.width = 16;
			item.height = 16;
			item.damage = 650;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.channel = true;
			item.autoReuse = true;
			item.melee = true;
			item.useAnimation = 13;
			item.useTime = 13;
			item.useStyle = 5;
			item.knockBack = 2.5f;
			item.UseSound = SoundID.Item116;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.shootSpeed = 24f;
			item.shoot = 611;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "CosmiliteBar", 5);
			recipe.AddIngredient(null, "Phantoplasm", 5);
			recipe.AddIngredient(ItemID.SolarEruption);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	float ai3 = (Main.rand.NextFloat() - 0.75f) * 0.7853982f; //0.5
	    	float ai3X = (Main.rand.NextFloat() - 0.25f) * 0.7853982f; //0.5
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Mourningstar"), damage, knockBack, player.whoAmI, 0.0f, ai3);
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Mourningstar"), damage, knockBack, player.whoAmI, 0.0f, ai3X);
	    	return false;
		}
	}
}
