using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class ExcaliburShortsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Excalibur Shortsword");
			Tooltip.SetDefault("Don't underestimate the power of shortswords");
		}

		public override void SetDefaults()
		{
			item.useStyle = 3;
			item.useTurn = false;
			item.useAnimation = 10;
			item.useTime = 10;
			item.width = 40;
			item.height = 40;
			item.damage = 55;
			item.melee = true;
			item.knockBack = 5.25f;
			item.UseSound = SoundID.Item1;
			item.useTurn = true;
			item.autoReuse = true;
			item.value = 500000;
			item.rare = 5;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.HallowedBar, 7);
	        recipe.AddTile(TileID.MythrilAnvil);	
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 57);
	        }
	    }
	}
}
