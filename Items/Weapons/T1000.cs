using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons 
{
	public class T1000 : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("T1000");
			Tooltip.SetDefault("Fires a spread of rainbow lasers");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 370;
	        item.magic = true;
	        item.mana = 10;
	        item.width = 20;
	        item.height = 12;
	        item.useTime = 12;
	        item.useAnimation = 12;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.noUseGraphic = true;
			item.channel = true;
	        item.knockBack = 4f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
	        item.shoot = mod.ProjectileType("T1000");
	        item.shootSpeed = 24f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Purge");
			recipe.AddIngredient(null, "Phantoplasm", 5);
	        recipe.AddIngredient(null, "CosmiliteBar", 5);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("T1000"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}