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
	public class Azathoth : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Azathoth");
			Tooltip.SetDefault("Destroy the universe in the blink of an eye\nFires cosmic orbs that blast nearby enemies with lasers");
		}

	    public override void SetDefaults()
	    {
	    	item.CloneDefaults(ItemID.Kraken);
	        item.damage = 240;
	        item.useTime = 20;
	        item.useAnimation = 20;
	        item.useStyle = 5;
	        item.channel = true;
	        item.melee = true;
	        item.knockBack = 6f;
	        item.value = 10000000;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("AzathothProjectile");
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
	    
	    public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage), knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}
	    
	    public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Terrarian);
	        recipe.AddIngredient(null, "ShadowspecBar", 5);
	        recipe.AddIngredient(null, "CoreofCalamity", 3);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}