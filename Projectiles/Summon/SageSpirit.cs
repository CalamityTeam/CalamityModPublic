using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Projectiles.Summon
{
	public class SageSpirit : ModProjectile
	{
		internal Player Owner => Main.player[projectile.owner];
		internal ref float AttackTimer => ref projectile.ai[0];
		internal ref float PlayerFlyTime => ref projectile.ai[1];
		internal ref float SageSpiritIndex => ref projectile.localAI[0];
		internal ref float GeneralTime => ref projectile.localAI[1];
		internal Vector2 PlayerFlyStart;
		internal Vector2 PlayerFlyOffsetAtEnds;
		internal Vector2 PlayerFlyDestination;

		internal const int ShootRate = 40;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sage Spirit");
			Main.projFrames[projectile.type] = 4;
			ProjectileID.Sets.MinionSacrificable[projectile.type] = true;
			ProjectileID.Sets.MinionTargettingFeature[projectile.type] = true;
		}

		public override void SetDefaults()
		{
			projectile.width = 48;
			projectile.height = 72;
			projectile.netImportant = true;
			projectile.friendly = true;
			projectile.ignoreWater = true;
			projectile.minionSlots = 1f;
			projectile.timeLeft = 90000;
			projectile.penetrate = -1;
			projectile.tileCollide = false;
			projectile.minion = true;
		}

		public override void AI()
		{
			GeneralTime++;
			ProvidePlayerMinionBuffs();
			DetermineFrames();
			NPC potentialTarget = projectile.Center.MinionHoming(750f, Owner);
			if (potentialTarget is null)
				FlyNearOwner();
			else
			{
				ResetOwnerFlyValues();
				AttackTarget(potentialTarget);
			}
		}

		internal void ProvidePlayerMinionBuffs()
		{
			Owner.AddBuff(ModContent.BuffType<SageSpiritBuff>(), 3600);

			// Verify player/minion state integrity. The minion cannot stay alive if the
			// owner is dead or if the caller of the AI is invalid.
			if (projectile.type != ModContent.ProjectileType<SageSpirit>())
				return;

			if (Owner.dead)
				Owner.Calamity().sageSpirit = false;
			if (Owner.Calamity().sageSpirit)
				projectile.timeLeft = 2;
		}

		internal void DetermineFrames()
		{
			projectile.frameCounter++;
			if (projectile.frameCounter % 5 == 4)
				projectile.frame = (projectile.frame + 1) % Main.projFrames[projectile.type];
		}

		internal void ResetOwnerFlyValues()
		{
			PlayerFlyTime = 0f;
			PlayerFlyOffsetAtEnds = Vector2.Zero;
			PlayerFlyStart = PlayerFlyDestination = -Vector2.One;
		}

		internal void FlyNearOwner()
		{
			Vector2 destination = Owner.Center + Vector2.UnitX * Owner.width * 1.6f * Owner.direction;

			int totalSageSpirits = Owner.ownedProjectileCounts[projectile.type];
			destination += ((float)(SageSpiritIndex * MathHelper.TwoPi) / totalSageSpirits).ToRotationVector2() * 70f;
			if (PlayerFlyTime >= 1f)
			{
				projectile.Center = PlayerFlyDestination;
				ResetOwnerFlyValues();
				return;
			}

			float distanceFromDestination = projectile.Distance(destination);
			if (float.IsNaN(distanceFromDestination))
				distanceFromDestination = 0f;

			// Determine owner values if they are still in the air.
			if (PlayerFlyTime <= 0f)
			{
				PlayerFlyStart = projectile.Center;
				PlayerFlyOffsetAtEnds = Vector2.UnitY * distanceFromDestination;
				PlayerFlyDestination = destination;
			}

			projectile.position.Y += (float)System.Math.Cos(GeneralTime / 60f * MathHelper.TwoPi + projectile.identity * 1.1f) * 0.5f;

			if (distanceFromDestination < 160f && PlayerFlyTime <= 0f)
				return;

			// A catmull-rom spline attempts to find a smooth arc of two points based on a 0-1 interpolant.
			// Its results are always within the bounds of the second and third term. They are intended to be the "middle" of the spline.
			// The other 2 points at the ends are intended to be used to determine how the resulting curve should arc.
			// In this context, since the offset at the start is down, it'll actually go up because it's starting at the bottom
			// and then going up, bending between the 2 middle points.
			projectile.Center = Vector2.CatmullRom(
				PlayerFlyStart + PlayerFlyOffsetAtEnds,
				PlayerFlyStart,
				PlayerFlyDestination,
				PlayerFlyDestination + PlayerFlyOffsetAtEnds,
				PlayerFlyTime);

			PlayerFlyTime = MathHelper.Clamp(PlayerFlyTime + 0.03f, 0f, 1f);
		}

		internal void AttackTarget(NPC target)
		{
			AttackTimer++;

			int totalSageSpirits = Owner.ownedProjectileCounts[projectile.type];
			Vector2 destinationOffsetFactor = Vector2.Max(target.Size, new Vector2(160f)) * new Vector2(0.6f, 0.37f);
			Vector2 destination = target.Center + (AttackTimer / 12f).ToRotationVector2() * destinationOffsetFactor;
			destination += ((float)(SageSpiritIndex * MathHelper.TwoPi) / totalSageSpirits).ToRotationVector2() * 130f;
			projectile.Center = Vector2.Lerp(projectile.Center, destination, 0.1f);

			// Frequently release a bunch of spikes.
			if (projectile.owner == Main.myPlayer && AttackTimer % ShootRate == ShootRate - 1)
			{
				for (int i = 0; i < 3; i++)
				{
					Vector2 spikeVelocity = -Vector2.UnitY.RotatedBy(MathHelper.Lerp(-0.56f, 0.56f, i / 3f)) * 6f;
					Projectile.NewProjectile(projectile.Top, spikeVelocity, ModContent.ProjectileType<SageNeedle>(), projectile.damage, projectile.knockBack, projectile.owner);
				}
			}
		}

		// The spirit itself should not do direct damage.
		public override bool CanDamage() => false;
	}
}
