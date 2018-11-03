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
		}

	    public override void SetDefaults()
	    {
	        item.damage = 200;
	        item.width = 20;
	        item.height = 12;
	        item.useTime = 12;
	        item.useAnimation = 12;
	        item.useStyle = 5;
	        item.knockBack = 3f;
	        item.value = 3000000;
	        item.UseSound = SoundID.Item5;
			item.noMelee = true;
			item.noUseGraphic = true;
			item.ranged = true;
			item.channel = true;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("Phangasm");
	        item.shootSpeed = 20f;
	        item.useAmmo = 40;
	    }
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(43, 96, 222);
	            }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	    	Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("Phangasm"), damage, knockBack, player.whoAmI, 0.0f, 0.0f);
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