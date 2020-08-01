using CalamityMod.Dusts;
using CalamityMod.Items;
using CalamityMod.Items.Weapons.DraedonsArsenal;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.DraedonsArsenal
{
	public class PhaseslayerProjectile : ModProjectile
	{
		// The "average" or "expected" swing speed which the sword's damage balance is based off of.
		// This is rotation EVERY FRAME. The "average" swing speed is 360 degrees in one second, aka pi/30 radians per frame.
		public const float StandardSwingSpeed = MathHelper.Pi / 30f;

		// How quickly the sword's damage updates to reflect its current speed. Higher values make it change damage more quickly.
		public const float DamageUpdateResponsiveness = 0.08f;

		// When the accumuluated "charge exhaustion" meter reaches this value, the Phaseslayer item loses one point of charge.
		public const float ChargeLossBreakpoint = 180f;

		public const int SwordBeamCooldown = 15;
		public const float SwordBeamDamageMultiplier = 0.15f;
		private const float MaximumMouseRange = 360f;
		private const float ProjCenterOffset = 36f;

		public bool IsSmall
		{
			get
			{
				CalamityGlobalItem swordItem = Main.player[projectile.owner].ActiveItem().Calamity();
				float chargeRatio = swordItem.CurrentCharge / (float)swordItem.ChargeMax;
				return chargeRatio < Phaseslayer.SizeChargeThreshold;
			}
		}
		
		// ai[0] wrapper. Stores a rolling lerped average of angular momentum which is used as the swing speed damage multiplier.
		public float AngularDamageFactor
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		// ai[1] wrapper. Stores an accumulating "charge exhaustion" meter. When it reaches the limit, a charge point is lost.
		public float ChargeLossProgress
		{
			get => projectile.ai[1];
			set => projectile.ai[1] = value;
		}

		// localAI[0] wrapper
		public float FadeoutTime
		{
			get => projectile.localAI[0];
			set => projectile.localAI[0] = value;
		}

		public int BladeFrameX => IsSmall ? 1 : projectile.frame / 7;
		public int BladeFrameY => IsSmall ? projectile.frame % 3 : projectile.frame % 7;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Phaseslayer");
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			projectile.width = 46; // Collision logic is done manually.
			projectile.height = 46;
			projectile.friendly = true;
			projectile.melee = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;

			projectile.penetrate = -1;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 7;
		}

		public override void AI()
		{
			Player player = Main.player[projectile.owner];
			// Angles are wrapped to be 0 to 2pi instead of -pi to pi for convenience with absolute values.
			float rotationAdjusted = MathHelper.WrapAngle(projectile.rotation) + MathHelper.Pi;
			float oldRotationAdjusted = MathHelper.WrapAngle(projectile.oldRot[1]) + MathHelper.Pi;
			float deltaAngle = Math.Abs(rotationAdjusted - oldRotationAdjusted);

			// Frame 1 effect: Prevent the sword from instantly firing a sword beam.
			if (projectile.localAI[1] == 0f)
			{
				projectile.localAI[1] = 1f;
				projectile.soundDelay = SwordBeamCooldown;
			}

			ManipulatePlayer(player);
			bool wasBig = !IsSmall;
			UpdateCharge(player);
			bool justShrunk = IsSmall && wasBig;
			if (justShrunk)
				OnShrinkEffects();
			AdjustDamageBasedOnRotation(player, deltaAngle);
			ManipulateFrames();
			HandleSwordBeams(player, deltaAngle);
			HandleFadeout();
		}

		public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
		{
			damage -= target.defense / 4;
		}

		private void ManipulatePlayer(Player player)
		{
			if (Main.myPlayer == player.whoAmI)
			{
				// In addition to typical channel cancellation criteria, the sword fizzles out if it runs out of charge.
				Item playerItem = player.ActiveItem();
				bool hasCharge = playerItem.Calamity().CurrentCharge > 0;
				if (player.channel && !player.noItems && !player.CCed && playerItem.type == ModContent.ItemType<Phaseslayer>() && hasCharge)
				{
					// The distance ratio ranges from 0 (your mouse is directly on the player) to 1 (your mouse is at the max range considered, or any further distance).
					float mouseDistance = projectile.Distance(Main.MouseWorld);
					float distRatio = Utils.InverseLerp(0f, MaximumMouseRange, mouseDistance, true);

					// This formula ensures that the sword has a sudden and extremely harsh responsiveness penalty when the mouse is close to the player.
					// Otherwise it controls perfectly fine.
					float aimResponsiveness = 0.035f + 0.3f * (float)Math.Pow(distRatio, 1D/3);

					// Update the sword's angle with the responsiveness determined by mouse position.
					projectile.rotation = projectile.rotation.AngleLerp(player.AngleTo(Main.MouseWorld), aimResponsiveness);
				}

				// If the player is not wielding the sword, determine whether it should fade out or instantly vanish.
				else if (FadeoutTime == 0f)
				{
					// If the player voluntarily stopped holding left mouse, start the fadeout timer.
					if (!player.channel)
						FadeoutTime = 10f;
					// If the player was killed, frozen, cursed, etc. just delete it immediately.
					else
						projectile.Kill();
				}

				projectile.Center = player.MountedCenter + projectile.rotation.ToRotationVector2() * ProjCenterOffset;
			}

			// These lines ensure the player and their arm are rotated the correct direction to hold the sword.
			projectile.direction = (Math.Cos(projectile.rotation) > 0).ToDirectionInt();
			player.ChangeDir(projectile.direction);
			player.heldProj = projectile.whoAmI;
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = projectile.rotation * projectile.direction;
		}

		private void UpdateCharge(Player player)
		{
			// Having the sword active at all costs some energy.
			ChargeLossProgress += 1f;
			
			// If charge is due to be lost, reset the accumuluated charge exhaustion and update the player's held item.
			if (ChargeLossProgress > ChargeLossBreakpoint)
			{
				ChargeLossProgress -= ChargeLossBreakpoint;

				if (Main.myPlayer == projectile.owner)
				{
					if (Main.mouseItem.active)
						Main.mouseItem.Calamity().CurrentCharge--;
					else
						player.HeldItem.Calamity().CurrentCharge--;
				}
			}
		}

		private void OnShrinkEffects()
		{
			Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/MechGaussRifle"), projectile.Center);
			if (Main.dedServ)
				return;

			for (int i = 0; i < 60; i++)
			{
				Dust dust = Dust.NewDustPerfect(projectile.Center, (int)CalamityDusts.Brimstone);
				dust.velocity = Main.rand.NextVector2Circular(20f, 20f);
				dust.scale = 2.5f;
				dust.fadeIn = 1.2f;
				dust.noGravity = true;
			}
		}

		private void AdjustDamageBasedOnRotation(Player player, float deltaAngle)
		{
			// Update the rolling "blade angular momentum" average by gently lerping in the newest data point.
			AngularDamageFactor = MathHelper.Lerp(AngularDamageFactor, deltaAngle, DamageUpdateResponsiveness);

			// 0x   expected speed gives you  53.5% damage.
			// 1x   expected speed gives you 100.0% damage.
			// 1.5x expected speed gives you 116.6% damage.
			// 2x   expected speed gives you 130.6% damage.
			// 3x   expected speed gives you 153.5% damage.
			// 4x   expected speed gives you 171.7% damage.
			// 5x   expected speed gives you 187.0% damage.
			float speedDamageScalar = 0.166f + (float)Math.Log(AngularDamageFactor / StandardSwingSpeed + 1.5f, 3f);
			float statDamageScalar = player.MeleeDamage() * (IsSmall ? Phaseslayer.SmallDamageMultiplier : 1f);
			projectile.damage = (int)(Phaseslayer.Damage * speedDamageScalar * statDamageScalar);
		}

		private void ManipulateFrames()
		{
			projectile.frame = 0;

			if (IsSmall)
			{
				// Fadeout frames.
				if (FadeoutTime > 5)
					projectile.frame = 1;
				else if (FadeoutTime > 0)
					projectile.frame = 2;
			}
			else
			{
				projectile.frameCounter++;
				int adjustFrameCounter = projectile.frameCounter % 120;

				// Idle lightning frames.
				if (adjustFrameCounter >= 50 && adjustFrameCounter <= 78)
					projectile.frame = (int)MathHelper.Lerp(1, 9, Utils.InverseLerp(50, 75, adjustFrameCounter, true));
				if (adjustFrameCounter >= 90 && adjustFrameCounter <= 120)
					projectile.frame = (int)MathHelper.Lerp(10, 18, Utils.InverseLerp(90, 117, adjustFrameCounter, true));

				// Fadeout frames.
				if (FadeoutTime > 5)
					projectile.frame = 19;
				else if (FadeoutTime > 0)
					projectile.frame = 20;
			}
		}

		private void HandleSwordBeams(Player player, float deltaAngle)
		{
			// Producing a sword beam takes a bit higher of a speed than the "typical" speed the sword is balanced around.
			if (projectile.soundDelay <= 0 && deltaAngle >= 1.3f * StandardSwingSpeed)
			{
				// Sword beams cost a noticeable amount of energy, but deal the blade's current damage. Swing harder to get more damage!
				ChargeLossProgress += 30f;
				if (Main.myPlayer == player.whoAmI)
				{
					Vector2 velocity = projectile.rotation.ToRotationVector2() * 20f;
					Projectile.NewProjectile(projectile.Center, velocity, ModContent.ProjectileType<PhaseslayerBeam>(), (int)(projectile.damage * SwordBeamDamageMultiplier), 0f, player.whoAmI);
				}

				// The sound delay doubles as the sword beam's cooldown.
				projectile.soundDelay = SwordBeamCooldown;
				Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/ELRFire"), projectile.Center);
			}
		}

		private void HandleFadeout()
		{
			if (FadeoutTime > 0)
			{
				FadeoutTime--;
				if (FadeoutTime <= 0f)
					projectile.Kill();
			}
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			Texture2D bladeTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PhaseslayerBlade");
			Texture2D hiltTexture = ModContent.GetTexture(Texture);
			if (IsSmall)
				bladeTexture = ModContent.GetTexture("CalamityMod/Projectiles/DraedonsArsenal/PhaseslayerBladeSmall");
			Vector2 bladeOffset = projectile.rotation.ToRotationVector2() * (IsSmall ? 90f : 132f) * projectile.scale;
			Vector2 origin = bladeTexture.Size() * 0.5f;
			origin /= IsSmall ? new Vector2(1f, 3f) : new Vector2(3f, 7f);
			Rectangle frame = IsSmall ? bladeTexture.Frame(1, 3, 0, BladeFrameY) : bladeTexture.Frame(3, 7, BladeFrameX, BladeFrameY);

			// TODO: Update this afterimage drawcode to be more cool.
			for (int i = 1; i < projectile.oldRot.Length; i++)
			{
				float angleDelta = MathHelper.Clamp(projectile.rotation - projectile.oldRot[i], -0.26f, 0.26f);
				float angle = projectile.rotation + angleDelta;
				angle += MathHelper.PiOver2;
				Color color = Color.White * (float)Math.Pow(1f - i / (float)projectile.oldRot.Length, 3f);
				Rectangle afterimageFrame = IsSmall ? bladeTexture.Frame(1, 3, 0, 2) : bladeTexture.Frame(3, 7, 2, 6);
				spriteBatch.Draw(bladeTexture, projectile.Center + bladeOffset - Main.screenPosition, afterimageFrame, color, angle, origin, projectile.scale, SpriteEffects.None, 0f);
			}

			spriteBatch.Draw(bladeTexture, projectile.Center + bladeOffset - Main.screenPosition, frame, Color.White, projectile.rotation + MathHelper.PiOver2, origin, projectile.scale, SpriteEffects.None, 0f);
			spriteBatch.Draw(hiltTexture, projectile.Center - Main.screenPosition, null, lightColor, projectile.rotation + MathHelper.PiOver2, hiltTexture.Size() * 0.5f, projectile.scale, SpriteEffects.None, 0f);

			return false;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			float _ = 0f;
			Vector2 start = projectile.Center + projectile.rotation.ToRotationVector2() * 28f;
			Vector2 end = start + projectile.rotation.ToRotationVector2() * (IsSmall ? 202f : 254f) * projectile.scale;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 60f * projectile.scale, ref _);
		}
	}
}
