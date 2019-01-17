using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons 
{
	public class BurntSienna : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Burnt Sienna");
			Tooltip.SetDefault("Causes enemies to erupt into healing projectiles on death");
		}

		public override void SetDefaults()
		{
			item.width = 42;
			item.damage = 30;
			item.melee = true;
			item.useAnimation = 21;
			item.useTime = 21;
			item.useTurn = true;
			item.useStyle = 1;
			item.knockBack = 5.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 54;
            item.value = Item.buyPrice(0, 4, 0, 0);
            item.rare = 3;
			item.shootSpeed = 5f;
		}
		
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.Amber, 5);
			recipe.AddIngredient(ItemID.AshBlock, 30);
			recipe.AddIngredient(ItemID.Obsidian, 10);
			recipe.AddIngredient(ItemID.MeteoriteBar, 6);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
		
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			float spread = 180f * 0.0174f;
			double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed)- spread/2;
			double deltaAngle = spread/8f;
			double offsetAngle;
			int i;
			if (target.life <= 0)
			{
	    		for (i = 0; i < 1; i++ )
				{
					float randomSpeedX = (float)Main.rand.Next(3);
					float randomSpeedY = (float)Main.rand.Next(3, 5);
				   	offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
				   	int projectile1 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("BurntSienna"), item.damage, knockback, Main.myPlayer);
				    int projectile2 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("BurntSienna"), item.damage, knockback, Main.myPlayer);
					int projectile3 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("BurntSienna"), item.damage, knockback, Main.myPlayer);
				    Main.projectile[projectile1].velocity.X = -randomSpeedX;
				    Main.projectile[projectile1].velocity.Y = -randomSpeedY;
				    Main.projectile[projectile2].velocity.X = randomSpeedX;
				    Main.projectile[projectile2].velocity.Y = -randomSpeedY;
				    Main.projectile[projectile3].velocity.X = 0f;
				    Main.projectile[projectile3].velocity.Y = -randomSpeedY;
				}
			}
		}
	
	    public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(5) == 0)
	        {
	        	int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 246);
	        }
	    }
	}
}
