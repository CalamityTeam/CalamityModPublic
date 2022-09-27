using CalamityMod.Buffs.Summon;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;

namespace CalamityMod.Projectiles.Summon
{
    public class PuffWarrior : ModProjectile
    {
		public Player Owner => Main.player[Projectile.owner];

		public Tile GroundTile => CalamityUtils.ParanoidTileRetrieval((int)(Projectile.Bottom.X / 16), (int)(Projectile.Bottom.Y / 16) + 1);

		public bool Jumping => !GroundTile.IsTileSolidGround() || Math.Abs(Projectile.velocity.Y) > 5f;

		public ref float JumpCountdown => ref Projectile.localAI[1];

		public const float Gravity = 0.425f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Puff Warrior");
            Main.projFrames[Projectile.type] = 10;
            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.netImportant = true;
            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.minionSlots = 1f;
            Projectile.timeLeft = 90000;
            Projectile.penetrate = -1;
            Projectile.minion = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 18;
			Projectile.tileCollide = true;
        }

        public override void AI()
        {
			ProvidePlayerMinionBuffs();
			DetermineFrames();

			NPC potentialTarget = Projectile.Center.MinionHoming(660f, Owner);
			if (potentialTarget is null)
			{
				HopToOwner(out Vector2 guardSpot);
				TeleportToFarOffDestination(guardSpot);
			}
			else
				HopToTarget(potentialTarget);

			Projectile.spriteDirection = (Projectile.velocity.X > 0).ToDirectionInt();
			Projectile.rotation = Jumping ? Projectile.rotation + MathHelper.Pi / 10f * Projectile.spriteDirection : 0f;

			while (Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
				Projectile.position.Y -= 10f;

			// Enforce gravity.
			if (Projectile.velocity.Y < 16f)
				Projectile.velocity.Y += Gravity;
		}

		internal void ProvidePlayerMinionBuffs()
		{
			Owner.AddBuff(ModContent.BuffType<PuffWarriorBuff>(), 3600);

			// Verify player/minion state integrity. The minion cannot stay alive if the
			// owner is dead or if the caller of the AI is invalid.
			if (Projectile.type != ModContent.ProjectileType<PuffWarrior>())
				return;

			if (Owner.dead)
				Owner.Calamity().puffWarrior = false;
			if (Owner.Calamity().puffWarrior)
				Projectile.timeLeft = 2;
		}

		internal void DetermineFrames()
		{
			int startingFrame = Jumping ? Main.projFrames[Projectile.type] - 3 : 0;
			int endingFrame = Jumping ? Main.projFrames[Projectile.type] - 4 : Main.projFrames[Projectile.type] - 4;

			if (Jumping)
				JumpCountdown = 12f;
			else if (Math.Abs(Projectile.velocity.Y) <= Gravity && JumpCountdown > 0)
			{
				startingFrame = Main.projFrames[Projectile.type] - 1;
				endingFrame = Main.projFrames[Projectile.type] - 1;
				JumpCountdown--;
			}

			if (Projectile.frame < startingFrame)
				Projectile.frame = startingFrame;

			Projectile.frameCounter++;
			if (Projectile.frameCounter % 6 == 5)
				Projectile.frame++;

			if (Projectile.frame >= endingFrame)
				Projectile.frame = startingFrame;
		}

		internal void TeleportToFarOffDestination(Vector2 guardSpot)
		{
			bool obstructionBetweenSpot = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, guardSpot, 4, 4);
			bool outOfRangeOfSpot = !Projectile.WithinRange(guardSpot, obstructionBetweenSpot ? 700f : 1900f);
			if (Main.myPlayer == Projectile.owner && outOfRangeOfSpot)
			{
				Projectile.Center = guardSpot;
				Projectile.netUpdate = true;
			}
		}

		internal void HopToOwner(out Vector2 guardSpot)
		{
			guardSpot = Owner.Center;
			guardSpot.X += Owner.direction * 40f * (Projectile.identity % 14f + Projectile.identity / 14 * 0.2f);

			Vector2 searchPoint = guardSpot - Vector2.UnitY * 420f;

			// Ensure that the search point is not outside of the world (which would result in index problems in the search).
			searchPoint.Y = MathHelper.Clamp(searchPoint.Y, 32f, Main.maxTilesY * 16f - 32f);

			// Adjust the destination to be at the lowest point.
			WorldUtils.Find(searchPoint.ToTileCoordinates(), Searches.Chain(new Searches.Down(Main.maxTilesY + 2), new Conditions.IsSolid()), out Point guardPoint);
			guardSpot = guardPoint.ToWorldCoordinates(8f, 16f);

			if (!Jumping && MathHelper.Distance(guardSpot.X, Projectile.Center.X) > 10f)
			{
                Projectile.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(Projectile.Center, guardSpot, Gravity, 12f);
                Projectile.netUpdate = true;
			}
		}

		internal void HopToTarget(NPC target)
		{
			Vector2 attackPosition = target.Center;
			float attackPositionOffsetDirection = (target.velocity.X > 0).ToDirectionInt() * Math.Sign(Projectile.Center.X - target.Center.X);
			attackPosition.X += attackPositionOffsetDirection * 22f * (Projectile.identity % 14f + Projectile.identity / 14 * 0.2f);

			Vector2 searchPoint = attackPosition - Vector2.UnitY * 360f;

			// Ensure that the search point is not outside of the world (which would result in index problems in the search).
			searchPoint.Y = MathHelper.Clamp(searchPoint.Y, 32f, Main.maxTilesY * 16f - 32f);

			// Adjust the destination to be at the lowest point.
			WorldUtils.Find(searchPoint.ToTileCoordinates(), Searches.Chain(new Searches.Down(Main.maxTilesY + 2), new Conditions.IsSolid()), out Point guardPoint);
			attackPosition = guardPoint.ToWorldCoordinates();

			if (!Jumping)
			{
				Projectile.position += new Vector2(Projectile.spriteDirection * 4f, -4f);
				Projectile.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(Projectile.Center, attackPosition, Gravity, 17f);
				if (Main.myPlayer == Projectile.owner && Projectile.WithinRange(attackPosition, 360f))
				{
					for (int i = 0; i < 3; i++)
					{
						Vector2 shootPosition = Projectile.Top + Vector2.UnitY * (8f + i * 3f);
						Vector2 shootVelocity = (target.Center - shootPosition).SafeNormalize(Vector2.UnitY) * 14f;
						Projectile.NewProjectile(Projectile.GetSource_FromThis(), shootPosition, shootVelocity, ModContent.ProjectileType<PuffCloud>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
					}
				}
				Projectile.netUpdate = true;
			}
		}

		public override bool OnTileCollide(Vector2 oldVelocity)
		{
			if (Projectile.oldVelocity.Length() > 6f)
				Projectile.velocity = oldVelocity * 0.5f;
			else
				Projectile.velocity.X = 0f;
			return false;
		}
	}
}
