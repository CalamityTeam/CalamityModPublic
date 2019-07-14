using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Items;

namespace CalamityMod.Items.Patreon
{
	public class Ataraxia : ModItem
	{
		public static int BaseDamage = 3750; // 3120 before scaling was removed; 20.2% buff

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Ataraxia");
			Tooltip.SetDefault("Equanimity");
		}

		public override void SetDefaults()
		{
			item.width = 94;
			item.height = 92;
			item.melee = true;
			item.damage = BaseDamage;
			item.knockBack = 2.5f;
			item.useAnimation = 18;
			item.useTime = 18;
			item.autoReuse = true;
			item.useTurn = true;

			item.useStyle = 1;
			item.UseSound = SoundID.Item1;

			item.rare = 10;
			item.GetGlobalItem<CalamityGlobalItem>(mod).postMoonLordRarity = 21;
			item.value = Item.buyPrice(2, 50, 0, 0);

			item.shoot = mod.ProjectileType("AtaraxiaMain");
			item.shootSpeed = 9f;
		}

		// Fires one large and two small projectiles which stay together in formation.
		public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
			// Play the Terra Blade sound upon firing
			Main.PlaySound(SoundID.Item60, position);

			// Center projectile
			int centerID = mod.ProjectileType("AtaraxiaMain");
			int centerDamage = damage;
			Vector2 centerVec = new Vector2(speedX, speedY);
			int center = Projectile.NewProjectile(position.X, position.Y, speedX, speedY, centerID, centerDamage, knockBack, player.whoAmI, 0.0f, 0.0f);

			// Side projectiles (these deal 75% damage)
			int sideID = mod.ProjectileType("AtaraxiaSide");
			int sideDamage = (int)(0.75f * centerDamage);
			Vector2 speed = new Vector2(speedX, speedY);
			speed.Normalize();
			speed *= 22f;
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
			Vector2 leftOffset = speed.RotatedBy((double)(MathHelper.PiOver4), default(Vector2));
			Vector2 rightOffset = speed.RotatedBy((double)(-MathHelper.PiOver4), default(Vector2));
			leftOffset -= 1.4f * speed;
			rightOffset -= 1.4f * speed;
			Projectile.NewProjectile(rrp.X + leftOffset.X, rrp.Y + leftOffset.Y, speedX, speedY, sideID, sideDamage, knockBack, player.whoAmI, 0.0f, 0.0f);
			Projectile.NewProjectile(rrp.X + rightOffset.X, rrp.Y + rightOffset.Y, speedX, speedY, sideID, sideDamage, knockBack, player.whoAmI, 0.0f, 0.0f);
			return false;
		}

		// On-hit, tosses out five homing projectiles. This is not like Holy Collider.
		public override void OnHitNPC(Player player, NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.ShadowFlame, 480);
			target.AddBuff(BuffID.Ichor, 480);

			// Does not summon extra projectiles versus dummies.
			if (target.type == NPCID.TargetDummy)
				return;

			// Individual true melee homing missiles deal 10% of the weapon's base damage.
			int numSplits = 5;
			int trueMeleeID = mod.ProjectileType("AtaraxiaHoming");
			int trueMeleeDamage = (int)(0.1f * damage);
			float angleVariance = MathHelper.TwoPi / (float)numSplits;
			float spinOffsetAngle = MathHelper.Pi / (2f * numSplits);
			Vector2 posVec = new Vector2(8f, 0f).RotatedByRandom(MathHelper.TwoPi);

			for (int i = 0; i < numSplits; ++i)
			{
				posVec = posVec.RotatedBy(angleVariance);
				Vector2 velocity = new Vector2(posVec.X, posVec.Y).RotatedBy(spinOffsetAngle);
				velocity.Normalize();
				velocity *= 8f;
				Projectile.NewProjectile(target.Center + posVec, velocity, trueMeleeID, trueMeleeDamage, knockback, player.whoAmI, 0.0f, 0.0f);
			}
		}

		// Spawn some fancy dust while swinging
		public override void MeleeEffects(Player player, Rectangle hitbox)
		{
			int dustCount = Main.rand.Next(3, 6);
			Vector2 corner = new Vector2(hitbox.X + hitbox.Width / 4, hitbox.Y + hitbox.Height / 4);
			for (int i = 0; i < dustCount; ++i)
			{
				// Pick a random dust to spawn
				int dustID = -1;
				switch (Main.rand.Next(5))
				{
					case 0:
					case 1: dustID = 70; break;
					case 2: dustID = 71; break;
					default: dustID = 86; break;
				}
				int idx = Dust.NewDust(corner, hitbox.Width / 2, hitbox.Height / 2, dustID);
				Main.dust[idx].noGravity = true;
			}
		}

		public override void AddRecipes()
		{
			ModRecipe r = new ModRecipe(mod);
			r.AddIngredient(ItemID.BrokenHeroSword);
			r.AddIngredient(null, "CosmiliteBar", 25);
			r.AddIngredient(null, "Phantoplasm", 35);
			r.AddIngredient(null, "NightmareFuel", 90);
			r.AddIngredient(null, "EndothermicEnergy", 90);
			r.AddIngredient(null, "DarksunFragment", 65);
			r.AddIngredient(null, "BarofLife", 15);
			r.AddIngredient(null, "CoreofCalamity", 5);
			r.AddIngredient(null, "HellcasterFragment", 10);
			r.AddTile(null, "DraedonsForge");
			r.SetResult(this);
			r.AddRecipe();
		}
	}
}
