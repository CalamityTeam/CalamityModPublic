using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
	public class BrimlashBuster : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Brimlash Buster");
			Tooltip.SetDefault("50% chance to do triple damage on enemy hits\n" +
				"Fires a brimstone bolt that explodes into more bolts on death");
		}

		public override void SetDefaults()
		{
			item.width = 68;
			item.damage = 100;
			item.melee = true;
			item.useAnimation = 25;
			item.useTime = 25;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 8;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 68;
            item.value = Item.buyPrice(0, 95, 0, 0);
            item.rare = 9;
			item.shoot = mod.ProjectileType("Brimlash");
			item.shootSpeed = 18f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "Brimlash");
	        recipe.AddIngredient(null, "CoreofChaos", 3);
	        recipe.AddIngredient(ItemID.FragmentSolar, 10);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}

	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 235);
	        }
	    }

	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
	    	if (Main.rand.Next(3) == 0)
	    	{
	    		item.damage = 378;
	    	}
	    	else
	    	{
	    		item.damage = 126;
	    	}
		}
	}
}
