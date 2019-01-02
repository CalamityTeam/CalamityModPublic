using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class CausticEdge : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Caustic Edge");
			Tooltip.SetDefault("Give Sick");
		}

		public override void SetDefaults()
		{
			item.width = 46;
			item.damage = 44;
			item.melee = true;
            item.useTurn = true;
			item.useAnimation = 27;
			item.useStyle = 1;
			item.useTime = 27;
			item.knockBack = 5;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 48;
			item.value = 160000;
			item.rare = 3;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BladeofGrass);
			recipe.AddIngredient(ItemID.LavaBucket);
			recipe.AddIngredient(ItemID.Deathweed, 5);
	        recipe.AddTile(TileID.DemonAltar);	
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 74);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.Poisoned, 480);
		}
	}
}
