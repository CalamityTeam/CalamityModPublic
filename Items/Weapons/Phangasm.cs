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
	public class Phangasm : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phangasm");
			Tooltip.SetDefault("Fires a spread of arrows and emits phantom arrows on enemy hits");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 112;
	        item.width = 20;
	        item.height = 12;
	        item.useTime = 12;
	        item.useAnimation = 12;
	        item.useStyle = 5;
	        item.knockBack = 3f;
            item.value = Item.buyPrice(1, 80, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item5;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.ranged = true;
			item.channel = true;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Phangasm");
	        item.shootSpeed = 20f;
	        item.useAmmo = 40;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 14;
		}
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Phangasm"), damage, knockBack, player.whoAmI, 0f, 0f);
	    	return false;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Phantasm);
	        recipe.AddIngredient(null, "CosmiliteBar", 5);
	        recipe.AddIngredient(null, "Phantoplasm", 5);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
