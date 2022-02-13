using CalamityMod.Items.Weapons.Rogue;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Rogue
{
	public class NanoblackMain : ModProjectile
	{
		public override string Texture => "CalamityMod/Items/Weapons/Rogue/NanoblackReaperRogue";

		private const float RotationIncrement = 0.22f;
		private const int Lifetime = 240;
		private const float ReboundTime = 50f;
		private const int MinBladeTimer = 13;
        private const int MaxBladeTimer = 18;


		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Nanoblack Reaper");
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 7;
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
		}

		public override void SetDefaults()
		{
			projectile.width = 56;
			projectile.height = 56;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.tileCollide = false;
			projectile.penetrate = -1;
			projectile.extraUpdates = 2;
			projectile.timeLeft = Lifetime;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 8;
			projectile.Calamity().rogue = true;
		}

		public override void AI()
		{
			drawOffsetX = -11;
			drawOriginOffsetY = -4;
			drawOriginOffsetX = 0;

			// Initialize the frame counter and random blade delay on the very first frame.
			if (projectile.timeLeft == Lifetime)
				projectile.ai[1] = projectile.Calamity().stealthStrike ? 4f : GetBladeDelay();

			// Produces electricity and green firework sparks constantly while in flight.
			if (Main.rand.NextBool(3))
			{
				int dustType = Main.rand.NextBool(5) ? 226 : 220;
				float scale = 0.8f + Main.rand.NextFloat(0.3f);
				float velocityMult = Main.rand.NextFloat(0.3f, 0.6f);
				int idx = Dust.NewDust(projectile.position, projectile.width, projectile.height, dustType);
				Main.dust[idx].noGravity = true;
				Main.dust[idx].velocity = velocityMult * projectile.velocity;
				Main.dust[idx].scale = scale;
			}

			// ai[0] is a frame counter. ai[1] is a countdown to spawning the next nanoblack energy blade.
			projectile.ai[0] += 1f;
			projectile.ai[1] -= 1f;

			// On the frame the scythe begins returning, send a net update.
			if (projectile.ai[0] == ReboundTime)
				projectile.netUpdate = true;

			// The scythe runs its returning AI if the frame counter is greater than ReboundTime.
			if (projectile.ai[0] >= ReboundTime)
			{
				float returnSpeed = NanoblackReaperRogue.Speed;
				float acceleration = 2.4f;
				Player owner = Main.player[projectile.owner];

				// Delete the scythe if it's excessively far away.
				Vector2 playerCenter = owner.Center;
				float xDist = playerCenter.X - projectile.Center.X;
				float yDist = playerCenter.Y - projectile.Center.Y;
				float dist = (float)Math.Sqrt(xDist * xDist + yDist * yDist);
				if (dist > 3000f)
					projectile.Kill();

				dist = returnSpeed / dist;
				xDist *= dist;
				yDist *= dist;

				// Home back in on the player.
				if (projectile.velocity.X < xDist)
				{
					projectile.velocity.X = projectile.velocity.X + acceleration;
					if (projectile.velocity.X < 0f && xDist > 0f)
						projectile.velocity.X += acceleration;
				}
				else if (projectile.velocity.X > xDist)
				{
					projectile.velocity.X = projectile.velocity.X - acceleration;
					if (projectile.velocity.X > 0f && xDist < 0f)
						projectile.velocity.X -= acceleration;
				}
				if (projectile.velocity.Y < yDist)
				{
					projectile.velocity.Y = projectile.velocity.Y + acceleration;
					if (projectile.velocity.Y < 0f && yDist > 0f)
						projectile.velocity.Y += acceleration;
				}
				else if (projectile.velocity.Y > yDist)
				{
					projectile.velocity.Y = projectile.velocity.Y - acceleration;
					if (projectile.velocity.Y > 0f && yDist < 0f)
						projectile.velocity.Y -= acceleration;
				}

				// Delete the projectile if it touches its owner.
				if (Main.myPlayer == projectile.owner)
					if (projectile.Hitbox.Intersects(owner.Hitbox))
						projectile.Kill();
			}

			// Create nanoblack energy blades at a somewhat-random rate while in flight. (or full-sized scythes afterimages if stealth strike)
			if (projectile.ai[1] <= 0f)
			{
				if (projectile.Calamity().stealthStrike)
				{
					SpawnScytheAfterimage();
					projectile.ai[1] = 4f;
				}
				else
				{
					SpawnEnergyBlade();
					projectile.ai[1] = GetBladeDelay();
				}
			}

			// Rotate the scythe as it flies.
			float spin = projectile.direction <= 0 ? -1f : 1f;
			projectile.rotation += spin * RotationIncrement;
		}

		public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
		{
			CalamityUtils.DrawAfterimagesCentered(projectile, ProjectileID.Sets.TrailingMode[projectile.type], lightColor, 1);
			return false;
		}

		private int GetBladeDelay()
		{
			return Main.rand.Next(MinBladeTimer, MaxBladeTimer + 1);
		}

		private void SpawnEnergyBlade()
		{
			int bladeID = ModContent.ProjectileType<NanoblackSplit>();
			int bladeDamage = projectile.damage / 5;
			float bladeKB = 3f;
			float spin = projectile.direction <= 0 ? -1f : 1f;
			float d = 16f;
			float velocityMult = 0.9f;
			Vector2 directOffset = new Vector2(Main.rand.NextFloat(-d, d), Main.rand.NextFloat(-d, d));
			Vector2 velocityOffset = Main.rand.NextFloat(-velocityMult, velocityMult) * projectile.velocity;
			Vector2 pos = projectile.Center + directOffset + velocityOffset;
			if (projectile.owner == Main.myPlayer)
				Projectile.NewProjectile(pos, Vector2.Zero, bladeID, bladeDamage, bladeKB, projectile.owner, 0f, spin);
		}

		private void SpawnScytheAfterimage()
		{
			int bladeID = ModContent.ProjectileType<NanoblackStealthSplit>();
			int bladeDamage = projectile.damage / 2;
			float bladeKB = 4f;
			float spin = projectile.direction <= 0 ? -1f : 1f;
			float d = 16f;
			float velocityMult = 0.9f;
			if (projectile.owner == Main.myPlayer)
				Projectile.NewProjectile(projectile.Center, Vector2.Zero, bladeID, bladeDamage, bladeKB, projectile.owner, 0f, spin);
		}
	}
}
