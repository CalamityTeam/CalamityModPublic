using CalamityMod.Projectiles.Enemy;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
	public class TEDraedonLabTurret : TEBaseTurret
	{
		// Tile which hosts this tile entity
		public override int TileType => ModContent.TileType<DraedonLabTurret>();
		public override int HostTileWidth => DraedonLabTurret.Width;
		public override int HostTileHeight => DraedonLabTurret.Height;

		// Projectile variables
		public override int ProjectileType => ModContent.ProjectileType<DraedonLaser>();
		public override int ProjectileDamage => Main.expertMode ? 14 : 20;
		public override float ProjectileKnockback => 6.5f;
		public override float ShootSpeed => 5f;
		public override int FiringStartupDelay => 10;
		public override int FiringUseTime => 20;

		// Projectile spawn location variables
		public override Vector2 TurretCenterOffset => new Vector2(22f + 4f * Direction, -2f);
		protected override float ShootForwardsOffset => 32f;

		// Targeting variables
		public override float MaxRange => 600f;
		protected override float MaxTargetAngleDeviance => MathHelper.ToRadians(12f);
		protected override float MaxDeltaAnglePerFrame => MathHelper.ToRadians(2f);
		protected override float CloseAimThreshold => MathHelper.ToRadians(12f);
		protected override float CloseAimLerpFactor => 0.08f;

		// Variables specific to this turret subclass
		private int _playerTargetIndex = -1;
		public int PlayerTargetIndex
		{
			get => _playerTargetIndex;
			set
			{
				bool sync = _playerTargetIndex != value;
				_playerTargetIndex = value;
				if (sync)
					SendSyncPacket();
			}
		}
		
		protected override Vector2 ChooseTarget(Vector2 targetingCenter)
		{
			// Store the value that will be set into PlayerTargetIndex here.
			// This local is used because changing the actual field will trigger a netcode auto-sync.
			int indexToSet = PlayerTargetIndex;
			
			// If a player is already targeted, make sure they still exist and aren't dead.
			if (indexToSet != -1)
			{
				Player p = Main.player[PlayerTargetIndex];

				// If they don't exist, or are dead, then unmark them as a target and search for a new one.
				if (!p.active || p.dead)
					indexToSet = -1;
				else
				{
					// If the existing target player is close enough, return their position. Otherwise unmark them as a target.
					Vector2 playerPos = p.Center;
					float distSQ = Vector2.DistanceSquared(targetingCenter, playerPos);
					if (distSQ <= MaxRange * MaxRange)
						return playerPos;
					else
						indexToSet = -1;
				}
			}

			// No valid target currently exists. Loop through players (efficiently!) to look for the closest new target.
			Vector2 ret = InvalidTarget;
			float distSQToBeat = MaxRange * MaxRange;

			for (int i = 0; i < Main.maxPlayers; ++i)
			{
				Player p = Main.player[i];
				if (!p.active || p.dead)
					continue;

				float distSQ = p.DistanceSQ(targetingCenter);
				if (distSQ < distSQToBeat)
				{
					distSQToBeat = distSQ;
					ret = p.Center;
					indexToSet = i;
				}
			}

			// Set PlayerTargetIndex now that we're sure what we're targeting. This triggers a netcode sync if the value is different.
			PlayerTargetIndex = indexToSet;
			return ret;
		}

		// This type of turret syncs extra data: its player target index.
		protected override void SendExtraData(BinaryWriter writer) => writer.Write(_playerTargetIndex);
		protected override void ReceiveExtraData(Mod mod, BinaryReader reader) => _playerTargetIndex = reader.ReadInt32();
	}
}
