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
	public class DarklightGreatsword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Darklight Greatsword");
		}

		public override void SetDefaults()
		{
			item.width = 56;
			item.damage = 55;
			item.melee = true;
			item.useAnimation = 24;
			item.useStyle = 1;
			item.useTime = 24;
			item.useTurn = true;
			item.knockBack = 5;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 60;
			item.value = 300000;
			item.rare = 5;
			item.shoot = mod.ProjectileType("StarCrystal");
			item.shootSpeed = 16f;
		}
		
		public override bool Shoot(Player player, ref Microsoft.Xna.Framework.Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
	    {
	        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, (int)((double)damage * 0.6), knockBack, player.whoAmI, 0.0f, 0.0f);
	        return false;
		}
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "VerstaltiteBar", 12);
			recipe.AddIngredient(ItemID.FallenStar, 5);
			recipe.AddIngredient(ItemID.SoulofNight);
			recipe.AddIngredient(ItemID.SoulofLight);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(3) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 29);
	        }
	    }
	    
	    public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			target.AddBuff(BuffID.Frostburn, 100);
		}
	}
}
