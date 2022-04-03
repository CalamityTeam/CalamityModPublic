using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public abstract class TEBaseTurret : ModTileEntity
    {
        internal static readonly Vector2 InvalidTarget = new Vector2(float.NaN, float.NaN);

        // Fields specific to this instance of the turret tile entity
        public int FiringTime = 0;
        public float Angle = 0f;
        public Vector2 TargetPos = InvalidTarget;

        // Tile which hosts this tile entity, and variables relating to it
        public abstract int TileType { get; }
        public abstract int HostTileWidth { get; }
        public abstract int HostTileHeight { get; }

        // Projectile variables
        public abstract int ProjectileType { get; }
        public abstract int ProjectileDamage { get; }
        public abstract float ProjectileKnockback { get; }
        public abstract float ShootSpeed { get; }
        public abstract int FiringStartupDelay { get; }
        public abstract int FiringUseTime { get; }

        // Projectile spawn location variables
        public abstract Vector2 TurretCenterOffset { get; }
        protected virtual float ShootForwardsOffset => 0f;

        // Targeting variables

        // If the target is more than this distance away, stop tracking them.
        public abstract float MaxRange { get; }
        // Subclasses MUST define their own targeting algorithm.
        protected abstract Vector2 ChooseTarget(Vector2 targetingCenter);
        // If the target is more than this angle away from the turret's current firing direction, don't fire.
        protected virtual float MaxTargetAngleDeviance => MathHelper.ToRadians(16f);
        // How far can the turret swivel every frame while trying to track a fast target?
        protected virtual float MaxDeltaAnglePerFrame => MathHelper.ToRadians(4f);
        // At what angle difference does the turret switch from fixed swiveling to lerp aiming?
        protected virtual float CloseAimThreshold => MathHelper.ToRadians(8f);
        // How effective is the turret's lerp aiming? 1f makes it an aimbot.
        protected virtual float CloseAimLerpFactor => 1f;

        // "Calculator" properties which are calculated on the fly for the code.
        // Some of these are quite recursive, so if you do call them, store them in locals.
        public Vector2 TurretPosition => Position.ToWorldCoordinates(0f, 0f) + TurretCenterOffset;
        protected bool TargetIsInvalid => TargetPos.HasNaNs();
        public int Direction => (Math.Cos(Angle) > 0D).ToDirectionInt();
        public virtual float RestingAngle => Direction == -1 ? MathHelper.Pi : 0f;
        protected float TargetAngle => TargetIsInvalid ? RestingAngle : (TargetPos - TurretPosition).ToRotation();

        // This guarantees that Turret tile entities will not persist if not placed directly on some part of their host tile.
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.TileType == TileType;
        }

        #region Update and Turret Behavior
        public override void Update()
        {
            Vector2 turretPos = TurretPosition;

            // ChooseTarget runs every single frame for flexibility.
            // If you're storing a target e.g. a player or NPC, do that in your subclass implementation of ChooseTarget.
            TargetPos = ChooseTarget(turretPos);
            bool validTarget = !TargetIsInvalid;

            // If there is a defined target, then this frame is spent aiming, and maybe even firing!
            if (validTarget)
            {
                ActiveBehavior(turretPos, TargetPos);
                FiringTime++;
            }

            // Otherwise, reset the running "firing timer" and exhibit passive behavior.
            else
            {
                PassiveBehavior(turretPos);
                FiringTime = 0;
            }

            UpdateAngle();
        }

        // This function does nothing by default, but turrets can use it if they need to update themselves manually on multiplayer clients.
        // Calamity calls this function exactly once per frame.
        public virtual void UpdateClient() { }

        // This function runs on both client and server side. Clients run it manually using Tile Entity Time Handler.
        // This ensures that turrets aim at players visually in multiplayer without causing a packet storm.
        public virtual void UpdateAngle()
        {
            float targetAngle = TargetAngle;

            // WrapAngle clamps things from -pi to pi, not from 0 to 2pi.
            float deltaAngle = MathHelper.WrapAngle(Angle - targetAngle);
            bool usingCloseAiming = Math.Abs(deltaAngle) <= Math.Max(CloseAimThreshold, MaxDeltaAnglePerFrame);

            // When far away from its target angle, the turret swivels at a fixed speed towards its exact target angle.
            // When close in, the turret switches to lerping. The lerp strength is configurable. Set to 1 for an aimbot.
            // Setting the lerp coefficient to be intentionally low makes it easy to dodge shots (albeit narrowly!) without moving super quickly.
            Angle = usingCloseAiming
                ? Utils.AngleLerp(Angle, targetAngle, CloseAimLerpFactor)
                : MathHelper.WrapAngle(Angle - MaxDeltaAnglePerFrame * Math.Sign(deltaAngle));
        }

        // This function does nothing by default, but turrets can use it for flavor behaviors e.g. emitting sparks or beeping.
        protected virtual void PassiveBehavior(Vector2 turretPos) {}

        protected virtual void ActiveBehavior(Vector2 turretPos, Vector2 targetPos)
        {
            // This function won't run without a valid target, so this will always be an angle to a real target.
            float targetAngle = TargetAngle;

            // If the turret is too far off its mark or hasn't been around for long enough, don't fire at all.
            float deltaAngle = MathHelper.WrapAngle(Angle - targetAngle);
            bool angleCloseEnough = Math.Abs(deltaAngle) <= MaxTargetAngleDeviance;
            if (!angleCloseEnough || FiringTime < FiringStartupDelay)
                return;

            // Don't shoot every frame, but sync up the firing cadence to the startup delay.
            if ((FiringTime - FiringStartupDelay) % FiringUseTime == 0)
                Shoot(turretPos);
        }

        // Orders the turret to fire exactly one projectile at its current heading.
        // This function also sends a sync packet. As such, it refuses to run on mutliplayer clients.
        public Projectile Shoot(Vector2 turretMuzzlePos, float ai0 = 0f, float ai1 = 0f)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
                return null;

            SendSyncPacket();

            Vector2 angleVec = Angle.ToRotationVector2();
            Vector2 pos = turretMuzzlePos + angleVec * ShootForwardsOffset;
            Vector2 velocity = angleVec * ShootSpeed;
            return Projectile.NewProjectileDirect(pos, velocity, ProjectileType, ProjectileDamage, ProjectileKnockback, ai0: ai0, ai1: ai1);
        }
        #endregion

        // This code is called as a hook when the player places the host tile so that the turret tile entity may be placed.
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            // If in multiplayer, tell the server to place the tile entity and DO NOT place it yourself. That would mismatch IDs.
            // Also tell the server that you placed tiles in whatever space the host tile occupies.
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileRange(Main.myPlayer, i, j, HostTileWidth, HostTileHeight);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type);
                return -1;
            }

            // If in single player, just place the tile entity, no problems.
            int id = Place(i, j);
            return id;
        }

        // This code is called on dedicated servers only. It is the server-side response to MessageID.TileEntityPlacement.
        // When the server receives such a message from a client, it sends a MessageID.TileEntitySharing to all clients.
        // This will cause them to Place the tile entity locally at that position, all with exactly the same ID.
        public override void OnNetPlace() => NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);

        // Normally, a bunch of other TML hooks would go here, namely:
        //
        // void OnKill()
        // TagCompound Save()
        // void Load(TagCompound tag)
        //
        // Subclasses can override these TML hooks as they please. The base turret does not need to use them.

        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(FiringTime);
            writer.Write(Angle);
            writer.WriteVector2(TargetPos);
        }

        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            FiringTime = reader.ReadInt32();
            Angle = reader.ReadSingle();
            TargetPos = reader.ReadVector2();
        }

        protected internal void SendSyncPacket()
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                return;
            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)CalamityModMessageType.Turret);
            packet.Write(ID);
            packet.Write(FiringTime);
            packet.Write(Angle);
            packet.WriteVector2(TargetPos);
            WriteExtraData(packet);
            packet.Send(-1, -1);
        }

        protected internal static bool ReadSyncPacket(Mod mod, BinaryReader reader)
        {
            int teID = reader.ReadInt32();
            bool exists = ByID.TryGetValue(teID, out TileEntity te);

            // The rest of the packet must be read even if it turns out the turret doesn't exist for whatever reason.
            int firingTime = reader.ReadInt32();
            float angle = reader.ReadSingle();
            Vector2 targetVec = reader.ReadVector2();

            if (exists && te is TEBaseTurret turret)
            {
                turret.FiringTime = firingTime;
                turret.Angle = angle;
                turret.TargetPos = targetVec;
                turret.ReadExtraData(mod, reader);
                return true;
            }
            else
            {
                // Otherwise, discard the fixed extra bytes so the message stream doesn't go haywire.
                _ = reader.ReadBytes(NumExtraBytes);
                return false;
            }
        }

        // Subclasses cannot override SendSyncPacket, but they can override these functions to sync their own extra data.
        // Due to the limitations of TML packets, this data must be exactly 16 bytes in size.
        // The default implementations here write 16 bytes of zeroes and dump all 16 bytes when read.
        public const int NumExtraBytes = 16;
        protected virtual void WriteExtraData(BinaryWriter writer) {
            writer.Write(0Lu);
            writer.Write(0Lu);
        }
        protected virtual void ReadExtraData(Mod mod, BinaryReader reader) => _ = reader.ReadBytes(NumExtraBytes);
    }
}
