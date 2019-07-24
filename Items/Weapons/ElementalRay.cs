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
	public class ElementalRay : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Elemental Ray");
			Tooltip.SetDefault("Casts a rainbow ray that splits when enemies are near it");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 150;
	        item.magic = true;
	        item.mana = 18;
	        item.width = 62;
	        item.height = 62;
	        item.useTime = 16;
	        item.useAnimation = 16;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 7.5f;
            item.value = Item.buyPrice(1, 20, 0, 0);
            item.rare = 10;
            item.UseSound = SoundID.Item60;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("ElementRay");
	        item.shootSpeed = 6f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "GalacticaSingularity", 5);
	        recipe.AddIngredient(ItemID.LunarBar, 5);
	        recipe.AddIngredient(null, "TerraRay");
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}