using CalamityMod.Projectiles.Enemy;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.TileEntities
{
    public abstract class TEBaseTurret : ModTileEntity
    {
        public int Time;
        public int Direction;
        public float Rotation;
        public virtual float MaxAngleOffestShootPrompt => MathHelper.ToRadians(35f);
        public abstract int ShootWaitTime { get; }
        public abstract int ShootRate { get; }
        public abstract int ProjectileDamage { get; }
        public abstract float ShootSpeed { get; }
        public abstract float ShootSpawnOffset { get; }
        public abstract float ProjectileKnockback { get; }
        public abstract int TileType { get; }
        public abstract int ProjectileShot { get; }
        public abstract Vector2 ShootCoordsOffset { get; }
        public abstract float TargetDistanceCheck { get; }
        public void SyncTile()
        {
            NetMessage.SendData(MessageID.TileEntitySharing, -1, -1, null, ID, Position.X, Position.Y);
        }
        public virtual int AttemptToLocatePlayer(Vector2 shootTargetCoords)
        {
            int playerIndex = -1;
            for (int i = 0; i < Main.player.Length; i++)
            {
                // Ignore dead/inactive players. They are irrelevant since we're supposed to target a player.
                if (!Main.player[i].active || Main.player[i].dead)
                    continue;
                if (Main.player[i].Distance(shootTargetCoords) < TargetDistanceCheck)
                {
                    playerIndex = i;
                    break;
                }
            }
            return playerIndex;
        }
        public virtual void AttemptToFireAtPlayer(int playerIndex, Vector2 shootTargetCoords)
        {
            Player player = Main.player[playerIndex];
            float angleToTarget = (player.Center - shootTargetCoords).ToRotation();
            float idealAngle = angleToTarget + (Direction == -1).ToInt() * MathHelper.Pi;

            // Adjust the rotation before waiting, and then firing.
            if (Time % ShootRate < ShootRate - ShootWaitTime)
            {
                AimTowardsPlayer(playerIndex, shootTargetCoords, angleToTarget, ref idealAngle);
                SyncTile(); // The stupid rotation/direction doesn't sync otherwise. Don't ask me why.
            }

            // If at the end of the shoot cycle, and the angle offset isn't too far away, fire at the player.
            // Think of the angle offset as how close the turret is to aiming 100% perfectly at the player.
            if (Time % ShootRate == ShootRate - 1 && Math.Abs(MathHelper.WrapAngle(idealAngle - Rotation)) < MaxAngleOffestShootPrompt)
            {
                ShootProjectile(shootTargetCoords);
                SyncTile();
            }
        }
        public virtual void AimTowardsPlayer(int playerIndex, Vector2 shootTargetCoords, float angleToTarget, ref float idealAngle)
        {
            Player player = Main.player[playerIndex];
            int idealDirection = (player.Center.X - shootTargetCoords.X > 0).ToDirectionInt();

            // Adjust the direction if the turret should do so, along with the angle.
            if (Direction != idealDirection)
            {
                Direction = idealDirection;
                idealAngle = angleToTarget + (Direction == -1).ToInt() * MathHelper.Pi;
                Rotation = Rotation.AngleLerp(idealAngle, 0.4f); // Get a head start in rotating towards the ideal angle.
            }
            Rotation = Rotation.AngleLerp(idealAngle, 0.1f);
        }
        public virtual void ShootProjectile(Vector2 shootCoords)
        {
            float adjustedAngle = Rotation - (Direction == -1).ToInt() * MathHelper.Pi;
            Vector2 velocity = adjustedAngle.ToRotationVector2() * ShootSpeed;
            Projectile.NewProjectileDirect(shootCoords + adjustedAngle.ToRotationVector2() * ShootSpawnOffset, velocity, ProjectileShot, ProjectileDamage, ProjectileKnockback);
        }
        public override bool ValidTile(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.active() && tile.type == TileType;
        }
        public override void Update()
        {
            Vector2 positionWorldCoords = Position.ToVector2() * 16f;
            Vector2 shootTargetCoords = positionWorldCoords + ShootCoordsOffset;
            int playerIndex = AttemptToLocatePlayer(shootTargetCoords);
            if (playerIndex != -1)
            {
                AttemptToFireAtPlayer(playerIndex, shootTargetCoords);
                Time++;
            }
            else
            {
                Rotation = Rotation.AngleLerp(0f, 0.25f);
            }
        }
        public override int Hook_AfterPlacement(int i, int j, int type, int style, int direction)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                NetMessage.SendTileSquare(Main.myPlayer, i, j, 3);
                NetMessage.SendData(MessageID.TileEntityPlacement, -1, -1, null, i, j, Type, 0f, 0, 0, 0);
                return -1;
            }
            return Place(i, j);
        }
        public override void NetSend(BinaryWriter writer, bool lightSend)
        {
            writer.Write(Time);
            writer.Write(Direction);
            writer.Write(Rotation);
        }
        public override void NetReceive(BinaryReader reader, bool lightReceive)
        {
            Time = reader.ReadInt32();
            Direction = reader.ReadInt32();
            Rotation = reader.ReadSingle();
        }
    }
}
