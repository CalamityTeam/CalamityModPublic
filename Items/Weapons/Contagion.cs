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
	public class Contagion : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Contagion");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 300;
	        item.ranged = true;
	        item.width = 22;
	        item.height = 50;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.noUseGraphic = true;
			item.channel = true;
	        item.knockBack = 5f;
	        item.value = 10000000;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Contagion");
	        item.shootSpeed = 20f;
	        item.useAmmo = 40;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(255, 0, 255);
	            }
	        }
	    }
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "ShadowspecBar", 5);
			recipe.AddIngredient(ItemID.Phantasm);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Contagion"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	return false;
		}
	}
}