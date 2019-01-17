using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class ForbiddenOathblade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Forbidden Oathblade");
			Tooltip.SetDefault("Sword of an ancient demon god");
		}

		public override void SetDefaults()
		{
			item.width = 76;
			item.damage = 61;
			item.melee = true;
			item.useAnimation = 25;
			item.useTime = 25;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 6.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 76;
            item.value = Item.buyPrice(0, 36, 0, 0);
            item.rare = 5;
			item.shoot = mod.ProjectileType("Oathblade");
			item.shootSpeed = 3f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "BladecrestOathsword");
			recipe.AddIngredient(null, "OldLordOathsword");
			recipe.AddIngredient(ItemID.SoulofFright, 5);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 173);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.ShadowFlame, 240);
			target.AddBuff(BuffID.OnFire, 240);
		}
	}
}
