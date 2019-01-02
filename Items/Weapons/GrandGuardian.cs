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
	public class GrandGuardian : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Grand Guardian");
			Tooltip.SetDefault("Has a chance to lower enemy defense by 30 when striking them\n" +
			           "If enemy defense is 0 or below your attacks will heal you\n" +
			           "Striking enemies causes a large explosion\n" +
			           "Striking enemies that have under half life will make you release rainbow bolts\n" +
			           "Enemies spawn healing orbs on death");
		}

		public override void SetDefaults()
		{
			item.width = 124;
			item.damage = 200;
			item.melee = true;
			item.useAnimation = 22;
			item.useStyle = 1;
			item.useTime = 22;
			item.useTurn = true;
			item.knockBack = 8.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 124;
			item.value = 5000000;
			item.rare = 10;
			item.shootSpeed = 12f;
		}
		
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (target.type == mod.NPCType("DevourerofGodsBodyS") || target.type == mod.NPCType("DevourerofGodsBody"))
			{
				return;
			}
			if (Main.rand.Next(2) == 0)
			{
				target.defense -= 30;
			}
			if (target.defense <= 0)
			{
		    	player.statLife += 6;
		    	player.HealEffect(6);
			}
			Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("RainbowBoom"), (int)((double)((float)item.damage * player.meleeDamage) * 0.5), knockback, Main.myPlayer);
			float spread = 180f * 0.0174f;
			double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
			double deltaAngle = spread / 8f;
			double offsetAngle;
			int i;
			if (target.life <= (target.lifeMax * 0.5f))
			{
				for (i = 0; i < 1; i++ )
				{
					float randomSpeedX = (float)Main.rand.Next(9);
					float randomSpeedY = (float)Main.rand.Next(6, 15);
				   	offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
				   	int projectile1 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("RainBolt"), (int)((double)((float)item.damage * player.meleeDamage) * 0.75), knockback, Main.myPlayer);
				    int projectile2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("RainBolt"), (int)((double)((float)item.damage * player.meleeDamage) * 0.75), knockback, Main.myPlayer);
					int projectile3 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("RainBolt"), (int)((double)((float)item.damage * player.meleeDamage) * 0.75), knockback, Main.myPlayer);
				    Main.projectile[projectile1].velocity.X = -randomSpeedX;
				    Main.projectile[projectile1].velocity.Y = -randomSpeedY;
				    Main.projectile[projectile2].velocity.X = randomSpeedX;
				    Main.projectile[projectile2].velocity.Y = -randomSpeedY;
				    Main.projectile[projectile3].velocity.X = 0f;
				    Main.projectile[projectile3].velocity.Y = -randomSpeedY;
				}
			}
			if (target.life <= 0)
			{
		   		for (i = 0; i < 1; i++ )
				{
					float randomSpeedX = (float)Main.rand.Next(9);
					float randomSpeedY = (float)Main.rand.Next(6, 15);
				   	offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
				   	int projectile1 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("RainHeal"), item.damage, knockback, Main.myPlayer);
				    int projectile2 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("RainHeal"), item.damage, knockback, Main.myPlayer);
					int projectile3 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("RainHeal"), item.damage, knockback, Main.myPlayer);
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
	        if (Main.rand.Next(3) == 0)
	        {
	            int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 66, 0f, 0f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1f);
	            Main.dust[dust].noGravity = true;
	        }
	    }
	
		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "MajesticGuard");
			recipe.AddIngredient(null, "BarofLife", 10);
			recipe.AddIngredient(null, "GalacticaSingularity", 3);
	        recipe.AddTile(TileID.LunarCraftingStation);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
