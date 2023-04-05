using CalamityMod.Projectiles.Turret;
using CalamityMod.Tiles.PlayerTurrets;
using Microsoft.Xna.Framework;
using System.IO;
using Terraria;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public class TEPlayerFireTurret : TEBaseTurret
    {
        // Tile which hosts this tile entity
        public override int TileType => ModContent.TileType<PlayerFireTurret>();
        public override int HostTileWidth => PlayerFireTurret.Width;
        public override int HostTileHeight => PlayerFireTurret.Height;

        // Projectile variables
        public override int ProjectileType => ModContent.ProjectileType<FireShotBuffer>();
        public override int ProjectileDamage => 21;
        public override float ProjectileKnockback => 1f;
        public override float ShootSpeed => 8f;
        public override int FiringStartupDelay => 10;
        public override int FiringUseTime => 6;

        // Projectile spawn location variables
        public override Vector2 TurretCenterOffset => new Vector2(22f + 4f * Direction, -2f);
        protected override float ShootForwardsOffset => 24f;

        // Targeting variables
        public override float MaxRange => 300f;
        protected override float MaxTargetAngleDeviance => MathHelper.ToRadians(36f);
        protected override float MaxDeltaAnglePerFrame => MathHelper.ToRadians(5f);
        protected override float CloseAimThreshold => MathHelper.ToRadians(12f);
        protected override float CloseAimLerpFactor => 0.08f;

        // Variables specific to this turret subclass
        private int _npcTargetIndex = -1;
        public int NPCTargetIndex
        {
            get => _npcTargetIndex;
            set
            {
                bool sync = _npcTargetIndex != value;
                _npcTargetIndex = value;
                if (sync)
                    SendSyncPacket();
            }
        }

        // Lab turrets specifically must be placed on the top left corner of their host tile.
        // This restriction is not shared by general turrets for flexibility reasons.
        public override bool IsTileValidForEntity(int x, int y)
        {
            Tile tile = Main.tile[x, y];
            return tile.HasTile && tile.TileType == TileType && tile.TileFrameX == 0 && tile.TileFrameY == 0;
        }

        protected override Vector2 ChooseTarget(Vector2 targetingCenter)
        {
            if (CalamityUtils.AnyBossNPCS())
            {
                NPCTargetIndex = -1;
                return InvalidTarget;
            }

            int indexToSet = NPCTargetIndex;
            // If a npc is already targeted, make sure they still exist and aren't dead.
            if (indexToSet != -1)
            {
                NPC npc = Main.npc[NPCTargetIndex];

                // If they don't exist, or are dead, then unmark them as a target and search for a new one.
                if (!npc.active)
                    indexToSet = -1;
                else
                {
                    // If the existing target npc is close enough, return their position. Otherwise unmark them as a target.
                    Vector2 npcPos = npc.Center;
                    float distSQ = Vector2.DistanceSquared(targetingCenter, npcPos);
                    if (distSQ <= MaxRange * MaxRange)
                        return npcPos;
                    else
                        indexToSet = -1;
                }
            }

            // No valid target currently exists. Loop through npcs (efficiently!) to look for the closest new target.
            Vector2 ret = InvalidTarget;
            float distSQToBeat = MaxRange * MaxRange;

            for (int i = 0; i < Main.maxNPCs; ++i)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.CountsAsACritter)
                    continue;

                float distSQ = npc.DistanceSQ(targetingCenter);
                if (distSQ < distSQToBeat)
                {
                    distSQToBeat = distSQ;
                    ret = npc.Center;
                    indexToSet = i;
                }
            }

            // Set NPCTargetIndex now that we're sure what we're targeting. This triggers a netcode sync if the value is different.
            NPCTargetIndex = indexToSet;
            return ret;
        }

        // Makes turrets track their targeted player correctly client side.
        // This doesn't improve their actual accuracy, because they only fire server side.
        // However, it makes them rotate much more smoothly.
        // Any targeting and rotation data derived from this update will be overridden by the turret's next sync.
        public override void UpdateClient()
        {
            if (NPCTargetIndex != -1)
            {
                NPC npc = Main.npc[NPCTargetIndex];
                if (npc.active)
                    TargetPos = npc.Center;
            }
        }

        // This type of turret syncs extra data: its player target index.
        // This only takes 2 bytes so a junk array is written for the remainder of the space.
        private const int BytesUsed = 2;
        private static readonly byte[] JunkData = new byte[] { 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0x55, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA, 0xAA };
        protected override void WriteExtraData(BinaryWriter writer)
        {
            writer.Write((short)_npcTargetIndex);
            writer.Write(JunkData);
        }
        protected override void ReadExtraData(Mod mod, BinaryReader reader)
        {
            _npcTargetIndex = reader.ReadInt16();
            _ = reader.ReadBytes(NumExtraBytes - BytesUsed);
        }
    }
}
