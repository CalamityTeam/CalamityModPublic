using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using System;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
	public class GatlingLaserProj : ModProjectile
	{
		private SoundEffectInstance gatlingLaserLoop;
		private bool fireLasers = false;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Gatling Laser");
		}

		public override void SetDefaults()
		{
			projectile.width = 24;
			projectile.height = 58;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.magic = true;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			float num = MathHelper.PiOver2;
			Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
			if (projectile.type == ModContent.ProjectileType<GatlingLaserProj>())
			{
				if (projectile.ai[0] < 5f)
					projectile.ai[0] += 1f;

				if (projectile.ai[0] > 4f)
					projectile.ai[0] = 2f;

				// The Gatling Laser shoots every other frame (effective use time of 2).
				int fireRate = 2;
				projectile.ai[1] += 1f;
				bool shootThisFrame = false;
				if (projectile.ai[1] >= fireRate)
				{
					projectile.ai[1] = 0f;
					shootThisFrame = true;
				}

				if (projectile.soundDelay <= 0)
				{
					projectile.soundDelay = fireRate * 6;
					if (projectile.ai[0] != 1f)
					{
						fireLasers = true;
						projectile.soundDelay *= 6;
						gatlingLaserLoop = Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GatlingLaserFireLoop"), (int)projectile.position.X, (int)projectile.position.Y);
					}
				}
				if (shootThisFrame && Main.myPlayer == projectile.owner && fireLasers)
				{
					Item gatling = player.ActiveItem();

					// This grabs the weapon's current damage, taking current charge level into account.
					int currentDamage = player.GetWeaponDamage(gatling);

					bool stillInUse = player.channel && !player.noItems && !player.CCed;

					// This both checks if the player has sufficient mana and consumes it if they do.
					// If this is false, the Gatling Laser stops functioning.
					bool hasMana = player.CheckMana(gatling, -1, true, false);

					// Checks if the Gatling Laser has sufficient charge to fire. If this is false, it stops functioning.
					CalamityGlobalItem modItem = gatling.Calamity();
					bool hasCharge = modItem.Charge >= GatlingLaser.HoldoutChargeUse;

					if (stillInUse && hasMana && hasCharge)
					{
						float scaleFactor = player.ActiveItem().shootSpeed * projectile.scale;
						Vector2 value2 = vector;
						Vector2 value3 = Main.screenPosition + new Vector2(Main.mouseX, Main.mouseY) - value2;
						if (player.gravDir == -1f)
						{
							value3.Y = Main.screenHeight - Main.mouseY + Main.screenPosition.Y - value2.Y;
						}
						Vector2 vector3 = Vector2.Normalize(value3);
						if (float.IsNaN(vector3.X) || float.IsNaN(vector3.Y))
						{
							vector3 = -Vector2.UnitY;
						}
						vector3 *= scaleFactor;
						if (vector3.X != projectile.velocity.X || vector3.Y != projectile.velocity.Y)
						{
							projectile.netUpdate = true;
						}
						projectile.velocity = vector3;
						int type = ModContent.ProjectileType<GatlingLaserShot>();
						float velocity = 3f;
						value2 = projectile.Center;
						Vector2 spinningpoint = Vector2.Normalize(projectile.velocity) * velocity;

						double spread = Math.PI / 32D;
						spinningpoint = spinningpoint.RotatedBy(Main.rand.NextDouble() * 2D * spread - spread);

						if (float.IsNaN(spinningpoint.X) || float.IsNaN(spinningpoint.Y))
						{
							spinningpoint = -Vector2.UnitY;
						}
						Vector2 velocity2 = new Vector2(spinningpoint.X, spinningpoint.Y);
						if (velocity2.Length() > 5f)
						{
							velocity2.Normalize();
							velocity2 *= 5f;
						}
						float SpeedX = velocity2.X + Main.rand.Next(-1, 2) * 0.005f;
						float SpeedY = velocity2.Y + Main.rand.Next(-1, 2) * 0.005f;
						float ai0 = projectile.ai[0] - 2f; // 0, 1, or 2

						// Use charge when firing a laser.
						modItem.Charge -= GatlingLaser.HoldoutChargeUse;
						Projectile.NewProjectile(value2.X, value2.Y, SpeedX, SpeedY, type, currentDamage, projectile.knockBack, projectile.owner, ai0, 0f);
					}
					else
					{
						if (gatlingLaserLoop != null)
							gatlingLaserLoop.Stop();

						Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/GatlingLaserFireEnd"), (int)projectile.position.X, (int)projectile.position.Y);
						projectile.Kill();
					}
				}
			}
			projectile.position = player.RotatedRelativePoint(player.MountedCenter, true) - projectile.Size / 2f;
			projectile.rotation = projectile.velocity.ToRotation() + num;
			projectile.spriteDirection = projectile.direction;
			projectile.timeLeft = 2;
			player.ChangeDir(projectile.direction);
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = (float)Math.Atan2(projectile.velocity.Y * projectile.direction, projectile.velocity.X * projectile.direction);
		}

		public override bool CanDamage() => false;
	}
}
