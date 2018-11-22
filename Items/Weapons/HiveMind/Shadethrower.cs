using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Weapons.HiveMind
{
	public class Shadethrower : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadethrower");
			Tooltip.SetDefault("33% chance to not consume gel");
		}

	    public override void SetDefaults()
	    {
			item.damage = 16;
			item.ranged = true;
			item.width = 54;
			item.height = 14;
			item.useTime = 10;
			item.useAnimation = 30;
			item.useStyle = 5;
			item.noMelee = true;
			item.knockBack = 1.5f;
			item.UseSound = SoundID.Item34;
			item.value = 50000;
			item.rare = 3;
			item.autoReuse = true;
			item.shoot = mod.ProjectileType("ShadeFire");
			item.shootSpeed = 5.5f;
			item.useAmmo = 23;
		}
	    
	    public override Vector2? HoldoutOffset()
		{
			return new Vector2(-5, 0);
		}
	    
	    public override bool ConsumeAmmo(Player player)
	    {
	    	if (Main.rand.Next(0, 100) <= 33)
	    		return false;
	    	return true;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.RottenChunk, 3);
	        recipe.AddIngredient(ItemID.DemoniteBar, 7);
	        recipe.AddIngredient(null, "TrueShadowScale", 10);
	        recipe.AddTile(TileID.DemonAltar);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}