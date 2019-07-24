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
	public class DeathValley : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Death Valley Duster");
			Tooltip.SetDefault("Casts a large blast of dust");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 97;
	        item.magic = true;
	        item.mana = 9;
	        item.width = 28;
	        item.height = 30;
	        item.useTime = 25;
	        item.useAnimation = 25;
	        item.useStyle = 5;
	        item.noMelee = true;
	        item.knockBack = 5f;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
	        item.UseSound = SoundID.Item20;
	        item.autoReuse = true;
	        item.shoot = mod.ProjectileType("DustProjectile");
	        item.shootSpeed = 5f;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "Tradewinds");
	        recipe.AddIngredient(ItemID.FossilOre, 25);
	        recipe.AddIngredient(ItemID.AncientBattleArmorMaterial);
	        recipe.AddIngredient(null, "DesertFeather", 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}