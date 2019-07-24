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
	public class DemonicPitchfork : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Demonic Pitchfork");
			Tooltip.SetDefault("Fires a demonic pitchfork");
			Item.staff[item.type] = true;
		}

	    public override void SetDefaults()
	    {
	        item.damage = 67;
	        item.magic = true;
	        item.mana = 22;
	        item.width = 56;
	        item.height = 56;
	        item.useTime = 24;
	        item.useAnimation = 24;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 6;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
	        item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("DemonicPitchfork");
	        item.shootSpeed = 13f;
	    }
	    
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "TrueShadowScale", 15);
	        recipe.AddIngredient(ItemID.DarkLance);
	        recipe.AddIngredient(ItemID.Obsidian, 20);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}