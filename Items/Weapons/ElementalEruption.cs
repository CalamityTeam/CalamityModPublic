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
	public class ElementalEruption : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Eruption");
			Tooltip.SetDefault("90% chance to not consume gel");
		}

	    public override void SetDefaults()
	    {
			item.damage = 28;
			item.ranged = true;
			item.width = 64;
			item.height = 34;
			item.useTime = 2;
			item.useAnimation = 10;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3.5f;
			item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.autoReuse = true;
			item.shoot = mod.ProjectileType("TerraFireGreen2");
			item.shootSpeed = 10f;
			item.useAmmo = 23;
		}
	    
	    public override void ModifyTooltips(List<TooltipLine> list)
	    {
	        foreach (TooltipLine line2 in list)
	        {
	            if (line2.mod == "Terraria" && line2.Name == "ItemName")
	            {
	                line2.overrideColor = new Color(0, 255, 200);
	            }
	        }
	    }
	    
	    public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
	        int num6 = Main.rand.Next(3, 6);
	        for (int index = 0; index < num6; ++index)
	        {
	            float SpeedX = speedX + (float) Main.rand.Next(-25, 26) * 0.05f;
	            float SpeedY = speedY + (float) Main.rand.Next(-25, 26) * 0.05f;
	    		switch (Main.rand.Next(3))
				{
	    			case 0: type = mod.ProjectileType("TerraFireGreen2"); break;
	    			case 1: type = mod.ProjectileType("TerraFireRed"); break;
	    			case 2: type = mod.ProjectileType("TerraFireBlue"); break;
	    			default: break;
				}
	            Projectile.NewProjectile(position.X, position.Y, SpeedX, SpeedY, type, damage, knockBack, player.whoAmI, 0.0f, 0.0f);
	    	}
	    	return false;
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) <= 90)
	    		return false;
	    	return true;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "GalacticaSingularity", 5);
	        recipe.AddIngredient(null, "TerraFlameburster");
	        recipe.AddIngredient(null, "Meowthrower");
	        recipe.AddIngredient(null, "MepheticSprayer");
	        recipe.AddIngredient(null, "BrimstoneFlamesprayer");
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}