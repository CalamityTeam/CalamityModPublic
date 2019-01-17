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
	public class FulgurationHalberd : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Fulguration Halberd");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 53;
			item.melee = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.useTurn = true;
			item.knockBack = 4.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 62;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.CrystalShard, 20);
	        recipe.AddIngredient(ItemID.OrichalcumHalberd);
	        recipe.AddIngredient(ItemID.HellstoneBar, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	        recipe = new ModRecipe(mod);
	        recipe.AddIngredient(ItemID.CrystalShard, 20);
	        recipe.AddIngredient(ItemID.MythrilHalberd);
	        recipe.AddIngredient(ItemID.HellstoneBar, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	}
}
