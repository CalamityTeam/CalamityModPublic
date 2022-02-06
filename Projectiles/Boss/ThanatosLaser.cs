using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Boss
{
	public class ThanatosLaser : ModProjectile
	{
		public float TelegraphDelay
		{
			get => projectile.ai[0];
			set => projectile.ai[0] = value;
		}

		public NPC ThingToAttachTo => Main.npc.IndexInRange((int)projectile.ai[1]) ? Main.npc[(int)projectile.ai[1]] : null;

		public Vector2 Destination;
		public Vector2 Velocity;
		public const float TelegraphTotalTime = 30f;
		public const float TelegraphFadeTime = 15f;
		public const float TelegraphWidth = 4200f;
		public const float LaserVelocity = 10f;

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Exo Pulse Laser");
			Main.projFrames[projectile.type] = 4;
		}

		public override void SetDefaults()
		{
			projectile.Calamity().canBreakPlayerDefense = true;
			projectile.width = 22;
			projectile.height = 22;
			projectile.hostile = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.alpha = 255;
			projectile.penetrate = -1;
			projectile.extraUpdates = 1;
			projectile.timeLeft = 600;
			cooldownSlot = 1;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.WriteVector2(Destination);
			writer.WriteVector2(Velocity);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Destination = reader.ReadVector2();
			Velocity = reader.ReadVector2();
		}

		public override void AI()
		{
			// Determine the relative opacities for each player based on their distance.
			// This has a lower bound of 0.35 to prevent the laser from going completely invisible and players getting hit by cheap shots.
			if (projectile.localAI[0] == 0f)
			{
				projectile.localAI[0] = 1f;
				projectile.netUpdate = true;
			}

			projectile.frameCounter++;
			if (projectile.frameCounter > 12)
			{
				projectile.frame++;
				projectile.frameCounter = 0;
			}
			if (projectile.frame > 3)
				projectile.frame = 0;

			Lighting.AddLight(projectile.Center, 0.6f, 0f, 0f);

			// If there is no NPC to attach to, run this instead.
			if (projectile.ai[1] == -1f)
			{
				// Fade in after telegraphs have faded.
				if (TelegraphDelay > TelegraphTotalTime)
				{
					if (projectile.alpha > 0)
						projectile.alpha -= 25;
					if (projectile.alpha < 0)
						projectile.alpha = 0;

					// If a velocity is in reserve, set the true velocity to it and make it as "taken" by setting it to <0,0>
					if (Velocity != Vector2.Zero)
					{
						projectile.extraUpdates = 3;
						projectile.velocity = Velocity * ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.25f : 1f);
						Velocity = Vector2.Zero;
						projectile.netUpdate = true;
					}

					// Direction and rotation.
					if (projectile.velocity.X < 0f)
					{
						projectile.spriteDirection = -1;
						projectile.rotation = (float)Math.Atan2((double)-(double)projectile.velocity.Y, (double)-(double)projectile.velocity.X);
					}
					else
					{
						projectile.spriteDirection = 1;
						projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
					}
				}
				else if (Velocity == Vector2.Zero)
				{
					Velocity = projectile.velocity;
					projectile.velocity = Vector2.Zero;
					projectile.netUpdate = true;

					// Direction and rotation.
					if (projectile.velocity.X < 0f)
					{
						projectile.spriteDirection = -1;
						projectile.rotation = (float)Math.Atan2((double)-(double)Velocity.Y, (double)-(double)Velocity.X);
					}
					else
					{
						projectile.spriteDirection = 1;
						projectile.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
					}
				}

				TelegraphDelay++;

				return;
			}

			// Die if the thing to attach to disappears.
			if (ThingToAttachTo is null || !ThingToAttachTo.active)
			{
				projectile.Kill();
				return;
			}

			// If the Ares Laser Cannon is the owner.
			bool aresLaserIsOwner = ThingToAttachTo.type == ModContent.NPCType<AresLaserCannon>();

			// Fade in after telegraphs have faded.
			if (TelegraphDelay > TelegraphTotalTime)
			{
				if (projectile.alpha > 0)
					projectile.alpha -= 25;
				if (projectile.alpha < 0)
					projectile.alpha = 0;

				// If a velocity is in reserve, set the true velocity to it and make it as "taken" by setting it to <0,0>
				if (Velocity != Vector2.Zero)
				{
					projectile.extraUpdates = 3;
					projectile.velocity = Velocity * ((CalamityWorld.malice || BossRushEvent.BossRushActive) ? 1.25f : 1f);
					Velocity = Vector2.Zero;
					projectile.netUpdate = true;
				}

				// Direction and rotation.
				if (projectile.velocity.X < 0f)
				{
					projectile.spriteDirection = -1;
					projectile.rotation = (float)Math.Atan2((double)-(double)projectile.velocity.Y, (double)-(double)projectile.velocity.X);
				}
				else
				{
					projectile.spriteDirection = 1;
					projectile.rotation = (float)Math.Atan2((double)projectile.velocity.Y, (double)projectile.velocity.X);
				}
			}
			else if (Destination == Vector2.Zero)
			{
				// Set start of telegraph to the npc center.
				projectile.Center = ThingToAttachTo.Center;

				// Set destination of the laser, the target's center.
				Destination = projectile.velocity;

				if (aresLaserIsOwner)
					projectile.Center += Vector2.Normalize(Destination - ThingToAttachTo.Center) * 70f + Vector2.UnitY * 16f;

				// Calculate and store the velocity that will be used for laser telegraph rotation and beam firing.
				Vector2 projectileDestination = Destination - ThingToAttachTo.Center;
				Velocity = Vector2.Normalize(projectileDestination) * LaserVelocity;

				// Set velocity to zero.
				projectile.velocity = Vector2.Zero;
				projectile.netUpdate = true;

				// Direction and rotation.
				if (projectile.velocity.X < 0f)
				{
					projectile.spriteDirection = -1;
					projectile.rotation = (float)Math.Atan2((double)-(double)Velocity.Y, (double)-(double)Velocity.X);
				}
				else
				{
					projectile.spriteDirection = 1;
					projectile.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
				}
			}
			else
			{
				// Set start of telegraph to the npc center.
				projectile.Center = ThingToAttachTo.Center;

				if (aresLaserIsOwner)
					projectile.Center += Vector2.Normalize(Destination - ThingToAttachTo.Center) * 70f + Vector2.UnitY * 16f;

				// Calculate and store the velocity that will be used for laser telegraph rotation and beam firing.
				Vector2 projectileDestination = Destination - ThingToAttachTo.Center;
				Velocity = Vector2.Normalize(projectileDestination) * LaserVelocity;

				// Direction and rotation.
				if (projectile.velocity.X < 0f)
				{
					projectile.spriteDirection = -1;
					projectile.rotation = (float)Math.Atan2((double)-(double)Velocity.Y, (double)-(double)Velocity.X);
				}
				else
				{
					projectile.spriteDirection = 1;
					projectile.rotation = (float)Math.Atan2((double)Velocity.Y, (double)Velocity.X);
				}
			}

			TelegraphDelay++;
		}

		public override bool CanHitPlayer(Player target) => TelegraphDelay > TelegraphTotalTime;

		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			if (TelegraphDelay > TelegraphTotalTime)
				target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 180);
		}

		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)	
		{
			target.Calamity().lastProjectileHit = projectile;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			return CalamityUtils.CircularHitboxCollision(projHitbox.Center(), projectile.Size.Length() * 0.5f, targetHitbox);
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			if (TelegraphDelay >= TelegraphTotalTime)
			{
				lightColor.R = (byte)(255 * projectile.Opacity);
				lightColor.G = (byte)(255 * projectile.Opacity);
				lightColor.B = (byte)(255 * projectile.Opacity);
				Vector2 drawOffset = projectile.velocity.SafeNormalize(Vector2.Zero) * -30f;
				projectile.Center += drawOffset;
				CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
				projectile.Center -= drawOffset;
				return false;
			}

			Texture2D laserTelegraph = ModContent.GetTexture("CalamityMod/ExtraTextures/LaserWallTelegraphBeam");

			float yScale = 2f;
			if (TelegraphDelay < TelegraphFadeTime)
				yScale = MathHelper.Lerp(0f, 2f, TelegraphDelay / 15f);
			if (TelegraphDelay > TelegraphTotalTime - TelegraphFadeTime)
				yScale = MathHelper.Lerp(2f, 0f, (TelegraphDelay - (TelegraphTotalTime - TelegraphFadeTime)) / 15f);

			Vector2 scaleInner = new Vector2(TelegraphWidth / laserTelegraph.Width, yScale);
			Vector2 origin = laserTelegraph.Size() * new Vector2(0f, 0.5f);
			Vector2 scaleOuter = scaleInner * new Vector2(1f, 2.2f);

			Color colorOuter = Color.Lerp(Color.Red, Color.Crimson, TelegraphDelay / TelegraphTotalTime * 2f % 1f); // Iterate through crimson and red once and then flash.
			Color colorInner = Color.Lerp(colorOuter, Color.White, 0.75f);

			colorOuter *= 0.6f;
			colorInner *= 0.6f;

			spriteBatch.Draw(laserTelegraph, projectile.Center - Main.screenPosition, null, colorInner, Velocity.ToRotation(), origin, scaleInner, SpriteEffects.None, 0f);
			spriteBatch.Draw(laserTelegraph, projectile.Center - Main.screenPosition, null, colorOuter, Velocity.ToRotation(), origin, scaleOuter, SpriteEffects.None, 0f);
			return false;
		}
	}
}
