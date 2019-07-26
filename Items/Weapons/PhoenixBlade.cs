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
	public class PhoenixBlade : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phoenix Blade");
			Tooltip.SetDefault("Enemies explode and emit healing flames on death");
		}

		public override void SetDefaults()
		{
			item.width = 106;
			item.damage = 95;
			item.melee = true;
			item.useAnimation = 29;
			item.useStyle = 1;
			item.useTime = 29;
			item.useTurn = true;
			item.knockBack = 8f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 106;
            item.value = Item.buyPrice(0, 48, 0, 0);
            item.rare = 6;
			item.shootSpeed = 12f;
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
	    {
			if (target.life <= 0)
			{
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, 612, damage, knockback, Main.myPlayer);
				float spread = 180f * 0.0174f;
				double startAngle = Math.Atan2(item.shootSpeed, item.shootSpeed) - spread / 2;
				double deltaAngle = spread / 8f;
				double offsetAngle;
				int i;
				for (i = 0; i < 1; i++ )
				{
					float randomSpeedX = (float)Main.rand.Next(5);
					float randomSpeedY = (float)Main.rand.Next(3, 7);
				   	offsetAngle = (startAngle + deltaAngle * ( i + i * i ) / 2f ) + 32f * i;
				   	int projectile1 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( Math.Sin(offsetAngle) * 5f ), (float)( Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("PhoenixHeal"), item.damage, knockback, Main.myPlayer);
				    int projectile2 = Projectile.NewProjectile(target.Center.X, target.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("PhoenixHeal"), item.damage, knockback, Main.myPlayer);
				    Main.projectile[projectile1].velocity.X = -randomSpeedX;
				    Main.projectile[projectile1].velocity.Y = -randomSpeedY;
				    Main.projectile[projectile2].velocity.X = randomSpeedX;
				    Main.projectile[projectile2].velocity.Y = -randomSpeedY;
				}
			}
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
	    {
	        if (Main.rand.Next(4) == 0)
	        {
	            int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 244);
	        }
	    }

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(ItemID.BreakerBlade);
			recipe.AddIngredient(ItemID.HellstoneBar, 10);
			recipe.AddIngredient(null, "EssenceofCinder");
			recipe.AddIngredient(ItemID.SoulofMight, 3);
			recipe.AddIngredient(ItemID.SoulofSight, 3);
			recipe.AddIngredient(ItemID.SoulofFright, 3);
			recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(this);
	        recipe.AddRecipe();
		}
	}
}
