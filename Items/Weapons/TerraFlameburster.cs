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
	public class TerraFlameburster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Terra Flameburster");
			Tooltip.SetDefault("80% chance to not consume gel");
		}

	    public override void SetDefaults()
	    {
			item.damage = 56;
			item.ranged = true;
			item.width = 68;
			item.height = 22;
			item.useTime = 3;
			item.useAnimation = 15;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 3.25f;
			item.UseSound = SoundID.Item34;
            item.value = Item.buyPrice(0, 80, 0, 0);
            item.rare = 8;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("TerraFireGreen");
			item.shootSpeed = 7.5f;
			item.useAmmo = 23;
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) < 80)
	    		return false;
	    	return true;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.Flamethrower);
	        recipe.AddIngredient(null, "LivingShard", 7);
	        recipe.AddIngredient(null, "EssenceofCinder", 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
