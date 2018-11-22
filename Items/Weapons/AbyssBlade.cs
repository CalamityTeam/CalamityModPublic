using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class AbyssBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Abyss Blade");
			Tooltip.SetDefault("Hitting enemies will cause the crush depth debuff\nThe lower the enemies' defense the more damage they take from this debuff");
		}

		public override void SetDefaults()
		{
			item.width = 60;
			item.damage = 80;
			item.melee = true;
			item.useAnimation = 17;
			item.useTime = 17;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
			item.value = 900000;
			item.rare = 8;
			item.shoot = mod.ProjectileType("DepthOrb");
			item.shootSpeed = 19f;
		}
		
		public override void AddRecipes()
	    {
	        ModRecipe recipe = new ModRecipe(mod);
	        recipe.AddIngredient(null, "DepthBlade");
	        recipe.AddIngredient(ItemID.BrokenHeroSword);
            recipe.AddIngredient(null, "DepthCells", 15);
            recipe.AddIngredient(null, "Lumenite", 10);
            recipe.AddIngredient(null, "Tenebris", 5);
            recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
	    }
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 33);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
	    	target.AddBuff(mod.BuffType("CrushDepth"), 300);
		}
	}
}
