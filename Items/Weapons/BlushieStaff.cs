using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
	public class BlushieStaff : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Staff of Blushie");
			Tooltip.SetDefault("Hold your mouse, wait, wait, wait, and put your trust in the power of blue magic");
		}

		public override void SetDefaults()
		{
			item.width = 38;
			item.height = 38;
			item.useStyle = 4;
			item.useAnimation = 30;
			item.useTime = 30;
			item.channel = true;
			item.noMelee = true;
			item.damage = 1;
			item.knockBack = 1f;
			item.autoReuse = false;
			item.useTurn = false;
			item.magic = true;
            item.value = Item.buyPrice(5, 0, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item1;
			item.shoot = mod.ProjectileType("BlushieStaffProj");
			item.mana = 200;
			item.shootSpeed = 0f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 19;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Phantoplasm", 100);
	        recipe.AddIngredient(null, "ShadowspecBar", 50);
	        recipe.AddTile(null, "DraedonsForge");
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}