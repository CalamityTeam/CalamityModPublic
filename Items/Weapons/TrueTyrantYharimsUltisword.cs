using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Items.Weapons
{
	public class TrueTyrantYharimsUltisword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("True Tyrant's Ultisword");
			Tooltip.SetDefault("Fires blazing, hyper, and sunlight blades\n" +
				"Gives the player the tyrant's fury buff on enemy hits\n" +
				"This buff increases melee damage by 30% and melee crit chance by 10%");
		}

		public override void SetDefaults()
		{
			item.width = 102;
			item.damage = 185;
			item.melee = true;
			item.useAnimation = 18;
			item.useStyle = 1;
			item.useTime = 18;
			item.useTurn = true;
			item.knockBack = 7.5f;
			item.UseSound = SoundID.Item1;
			item.autoReuse = true;
			item.height = 102;
			item.value = Item.buyPrice(1, 20, 0, 0);
			item.rare = 10;
			item.shoot = mod.ProjectileType("BlazingPhantomBlade");
			item.shootSpeed = 12f;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 12;
		}

		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			switch (Main.rand.Next(3))
			{
				case 0: type = mod.ProjectileType("BlazingPhantomBlade"); break;
				case 1: type = mod.ProjectileType("HyperBlade"); break;
				case 2: type = mod.ProjectileType("SunlightBlade"); break;
				default: break;
			}
			Projectile.NewProjectile(position.X, position.Y, speedX, speedY, type, damage, knockBack, Main.myPlayer);
			return false;
		}

		public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(mod);
			recipe.AddIngredient(null, "TyrantYharimsUltisword");
			recipe.AddIngredient(null, "CoreofCalamity");
			recipe.AddIngredient(null, "UeliaceBar", 15);
			recipe.AddTile(TileID.DemonAltar);
			recipe.SetResult(this);
			recipe.AddRecipe();
		}

		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			if (Main.rand.Next(5) == 0)
			{
				int dust = Dust.NewDust(new Vector2(hitbox.X, hitbox.Y), hitbox.Width, hitbox.Height, 106);
			}
		}

		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			player.AddBuff(mod.BuffType("TyrantsFury"), 300);
			target.AddBuff(BuffID.OnFire, 300);
			target.AddBuff(BuffID.Venom, 300);
			target.AddBuff(BuffID.CursedInferno, 300);
			target.AddBuff(mod.BuffType("BrimstoneFlames"), 300);
			target.AddBuff(mod.BuffType("HolyLight"), 300);
		}
	}
}
