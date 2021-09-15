using CalamityMod.Particles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Magic
{
	public class SpiritCongregation : ModProjectile
	{
		public Vector2 HoverOffset = Vector2.Zero;
		public ref float Time => ref projectile.ai[0];
		public ref float BaseDamage => ref projectile.ai[1];
		public ref float DeathCounter => ref projectile.localAI[0];
		public Player Owner => Main.player[projectile.owner];
		public float CurrentPower => (float)Math.Pow(Utils.InverseLerp(15f, 840f, Time, true), 4D);
		public float CongregationDiameter => MathHelper.SmoothStep(54f, 215f, CurrentPower);
		public float MovementSpeed
		{
			get
			{
				float movementSpeed = 9f;

				// Make speed gradually build up over time, with growths at certain points.
				movementSpeed += MathHelper.SmoothStep(0f, 2.2f, Utils.InverseLerp(0.18f, 0.3f, Time, true));
				movementSpeed += MathHelper.SmoothStep(0f, 2f, Utils.InverseLerp(0.4f, 0.52f, Time, true));
				movementSpeed += MathHelper.SmoothStep(0f, 2f, Utils.InverseLerp(0.6f, 0.72f, Time, true));
				movementSpeed += MathHelper.SmoothStep(0f, 2f, Utils.InverseLerp(0.8f, 0.92f, Time, true));

				return movementSpeed;
			}
		}
		public override string Texture => "CalamityMod/Projectiles/InvisibleProj";

		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Spirit Monster");
			ProjectileID.Sets.TrailingMode[projectile.type] = 2;
			ProjectileID.Sets.TrailCacheLength[projectile.type] = 8;
		}

		public override void SetDefaults()
		{
			projectile.width = projectile.height = 60;
			projectile.friendly = true;
			projectile.penetrate = -1;
			projectile.magic = true;
			projectile.tileCollide = false;
			projectile.ignoreWater = true;
			projectile.usesLocalNPCImmunity = true;
			projectile.localNPCHitCooldown = 6;
		}

		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(DeathCounter);
			writer.WriteVector2(HoverOffset);
		}

		public override void ReceiveExtraAI(BinaryReader reader)
		{
			DeathCounter = reader.ReadSingle();
			HoverOffset = reader.ReadVector2();
		}

		public override void AI()
		{
			// If close to dying slow down and do nothing else.
			if (DeathCounter > 0f)
			{
				DeathCounter++;
				if (DeathCounter >= 35f)
					projectile.Kill();
				projectile.velocity *= 0.98f;
				EmitGhostGas();
				return;
			}

			if (Main.myPlayer == projectile.owner)
			{
				// Prepare for death prior to not being channeled.
				if (!Owner.channel)
				{
					DeathCounter = 1f;
					projectile.netUpdate = true;
					return;
				}

				Vector2 previousVelocity = projectile.velocity;

				// If not sufficiently close to the mouse, move towards it.
				if (!projectile.WithinRange(Main.MouseWorld, 80f))
					MoveTowardsMouse();

				// Otherwise slow down to a point.
				else if (projectile.velocity.Length() > 4f)
					projectile.velocity *= 0.95f;

				// Sync velocity if it changed from what it was before.
				if (previousVelocity != projectile.velocity)
				{
					projectile.netSpam = 0;
					projectile.netUpdate = true;
				}
			}

			// Handle dynamic damage.
			if (BaseDamage == 0f)
				BaseDamage = projectile.damage;
			else
			{
				float damageBoostFactor = MathHelper.SmoothStep(1f, 3.3f, CurrentPower);
				projectile.damage = (int)(BaseDamage * damageBoostFactor);
			}

			// Increment the timer on the final extra update.
			if (projectile.FinalExtraUpdate())
				Time++;

			// Set the hover offset to a zero vector after enough time has passed and the projectile is powerful/tame enough.
			bool tame = CurrentPower > 0.97f;
			projectile.hostile = !tame && Time > 75f;
			if (tame)
			{
				if (HoverOffset != Vector2.Zero)
					HoverOffset = Vector2.Zero;
			}

			// Otherwise, if not tame, periodically define a new offset.
			// This determines where to fly to and adds unpredictability at first.
			else if (Main.myPlayer == projectile.owner && Time % 55f == 54f)
			{
				float maxHoverOffset = MathHelper.SmoothStep(460f, 0f, CurrentPower);
				HoverOffset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.6f, 1f) * maxHoverOffset;
				projectile.netUpdate = true;
			}

			// Release spirits that fly towards the target
			if (!tame)
				ReleaseSmallSpirits();

			EmitGhostGas();
		}

		public void MoveTowardsMouse()
		{
			// Make inertia become more significant the more power the congregation has, due to growing size.
			float inertia = MathHelper.Lerp(18f, 40f, CurrentPower);

			Vector2 directionToMouseOffset = projectile.SafeDirectionTo(Main.MouseWorld + HoverOffset);
			Vector2 directionToOwner = projectile.SafeDirectionTo(Owner.Center);
			Vector2 idealVelocity = Vector2.Lerp(directionToMouseOffset, directionToOwner, 0.25f) * MovementSpeed;

			// Approach the ideal velocity.
			projectile.velocity = projectile.velocity.MoveTowards(idealVelocity, MovementSpeed * 0.04f);
			projectile.velocity = (projectile.velocity * (inertia - 1f) + idealVelocity) / inertia;
		}

		public void ReleaseSmallSpirits()
		{
			if (projectile.WithinRange(Owner.Center, 230f) || Time % 45f != 44f)
				return;

			Main.PlaySound(SoundID.DD2_EtherianPortalSpawnEnemy, projectile.Center);
			if (Main.myPlayer == projectile.owner)
			{
				Vector2 spiritVelocity = Main.rand.NextVector2Unit() * Main.rand.NextFloat(7f, 10f);
				Projectile.NewProjectile(projectile.Center, spiritVelocity, ModContent.ProjectileType<SmallSpirit>(), 70, 0f, projectile.owner, projectile.identity);
			}
		}

		public void EmitGhostGas()
		{
			float particleSize = CongregationDiameter;
			if (projectile.oldPosition != projectile.position && Time > 2f)
				particleSize += (projectile.oldPosition - projectile.position).Length() * 4.2f;

			// Place a hard limit on particle sizes.
			if (particleSize > 500f)
				particleSize = 500f;

			// Make particles shrink when dying.
			particleSize *= MathHelper.Lerp(1f, 0.5f, Utils.InverseLerp(0f, 35f, DeathCounter, true));

			int particleSpawnCount = Main.rand.NextBool(3) ? 3 : 1;
			for (int i = 0; i < particleSpawnCount; i++)
			{
				// Summon a base particle.
				Vector2 spawnPosition = projectile.Center + Main.rand.NextVector2Circular(1f, 1f) * particleSize / 26f;
				FusableParticleManager.GetParticleSetByType<GhostlyFusableParticleSet>().SpawnParticle(spawnPosition, particleSize);

				// And an "ahead" particle that spawns based on current movement.
				// This causes the "head" of the overall thing to have bumps when moving.
				spawnPosition += projectile.velocity.RotatedByRandom(1.38f) * particleSize / 65f;
				FusableParticleManager.GetParticleSetByType<GhostlyFusableParticleSet>().SpawnParticle(spawnPosition, particleSize * 0.4f);
			}

			// Release gas projectiles randomly. This does not happen when dying.
			if (Main.myPlayer == projectile.owner && Main.rand.NextBool(11) && DeathCounter <= 0f)
			{
				Vector2 dustVelocity = -projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(1.04f) * 1.5f;
				Projectile.NewProjectile(projectile.Center, dustVelocity, ModContent.ProjectileType<SpiritDust>(), 0, 0f, projectile.owner);
			}
		}

		// Prevent obsence damage when hitting players.
		public override void ModifyHitPlayer(Player target, ref int damage, ref bool crit)
		{
			damage = 92;
		}

		public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
		{
			for (int i = 0; i < projectile.oldPos.Length; i++)
			{
				float hitboxRadius = CongregationDiameter * MathHelper.Lerp(0.45f, 0.17f, i / (float)projectile.oldPos.Length);
				Vector2 hitboxCircle = Vector2.One * hitboxRadius;
				if (CalamityUtils.CircularHitboxCollision(projectile.oldPos[i] + hitboxCircle * 0.5f, hitboxRadius, targetHitbox))
					return true;
			}
			return false;
		}
	}
}
