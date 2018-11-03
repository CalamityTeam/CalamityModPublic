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
	public class InfernaCutter : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Inferna Cutter");
		}

	    public override void SetDefaults()
	    {
	        item.damage = 85;
	        item.melee = true;
	        item.width = 60;
	        item.height = 46;
	        item.useTime = 16;
	        item.useAnimation = 16;
	        item.useTurn = true;
	        item.axe = 27;
	        item.useStyle = 1;
	        item.knockBack = 7.75f;
	        item.value = 500000;
	        item.rare = 6;
	        item.UseSound = SoundID.Item1;
	        item.autoReuse = true;
	    }
	
	    public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "PurityAxe");
	        recipe.AddIngredient(ItemID.SoulofFright, 8);
	        recipe.AddIngredient(null, "EssenceofChaos", 3);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	    
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(4) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 6);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
	    	target.AddBuff(BuffID.OnFire, 300);
	    	if(Main.rand.Next(2) == 0)
	    	{
	    		target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
	    	}
		}
	}
}