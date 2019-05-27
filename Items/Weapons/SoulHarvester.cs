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
	public class SoulHarvester : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Soul Harvester");
			Tooltip.SetDefault("Enemies explode when on low health, spreading the plague");
		}

		public override void SetDefaults()
		{
			item.width = 62;
			item.damage = 98;
			item.melee = true;
			item.useAnimation = 20;
			item.useStyle = 1;
			item.useTime = 20;
			item.useTurn = true;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item71;
			item.autoReuse = true;
			item.height = 64;
			item.value = Item.buyPrice(0, 80, 0, 0);
			item.rare = 8;
			item.shoot = mod.ProjectileType("SoulScythe");
			item.shootSpeed = 18f;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "PlagueCellCluster", 10);
			recipe.AddIngredient(ItemID.CursedFlame, 20);
			recipe.AddIngredient(ItemID.DeathSickle);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.Next(3) == 0)
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 75);
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(mod.BuffType("Plague"), 200);
			target.AddBuff(BuffID.CursedInferno, 200);
			if (target.life <= (target.lifeMax * 0.15f))
			{
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 14);
				Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("HiveBombExplosion"), (int)((float)item.damage * player.meleeDamage), knockback, Main.myPlayer);
				for (int num621 = 0; num621 < 30; num621++)
				{
					int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 89, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num622].velocity *= 3f;
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num622].scale = 0.5f;
						Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
					}
				}
				for (int num623 = 0; num623 < 50; num623++)
				{
					int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 89, 0f, 0f, 100, default(Color), 3f);
					Main.dust[num624].noGravity = true;
					Main.dust[num624].velocity *= 5f;
					num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 89, 0f, 0f, 100, default(Color), 2f);
					Main.dust[num624].velocity *= 2f;
				}
			}
		}
	}
}
