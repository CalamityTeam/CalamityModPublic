using CalamityMod.Walls.DraedonStructures;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DraedonLabThings
{
	public class RepairUnitCritter : ModNPC
    {
        public enum BehaviorState
        {
            WalkAround,
            WalkOnWalls,
            SitAndRecharge
        }

        public Vector2 PreviousStuckPosition = Vector2.Zero;

        public BehaviorState CurrentState
        {
            get => (BehaviorState)(int)npc.ai[0];
            set => npc.ai[0] = (int)value;
        }

        public ref float StuckCount => ref npc.ai[1];

        public bool Initialized
        {
            get => npc.ai[2] == 1f;
            set => npc.ai[2] = value.ToInt();
        }
        public bool WantsToClimbOnSomeWall
        {
            get => npc.ai[3] == 1f;
            set => npc.ai[3] = value.ToInt();
        }

        public ref float CurrentFrame => ref npc.localAI[0];
        public ref float Time => ref npc.localAI[1];
        public ref float ClimbTime => ref npc.localAI[2];

        public const float Gravity = 0.4f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Repair Unit");
            Main.npcFrameCount[npc.type] = 17;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = 0;
            npc.width = 20;
            npc.height = 22;
            npc.lifeMax = 80;
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.HitSound = SoundID.NPCHit4;
            npc.DeathSound = SoundID.NPCDeath44;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(Time);
            writer.Write(ClimbTime);
            writer.WriteVector2(PreviousStuckPosition);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            Time = reader.ReadSingle();
            ClimbTime = reader.ReadSingle();
            PreviousStuckPosition = reader.ReadVector2();
        }

        public override void AI()
        {
            if (!Initialized)
            {
                npc.velocity = Vector2.UnitX * Main.rand.NextBool(2).ToDirectionInt() * 1.5f;
                Initialized = true;
            }

            npc.Calamity().ShouldFallThroughPlatforms = false;
            switch (CurrentState)
            {
                case BehaviorState.WalkAround:
                    WalkAroundOnGround();
                    ClimbTime = 0f;
                    break;
                case BehaviorState.WalkOnWalls:
                    WalkOnWalls();
                    ClimbTime++;
                    break;
            }
            Time++;
        }

        public void WalkAroundOnGround()
        {
            // Fall onto the ground if mid-air.
            npc.frameCounter += npc.velocity.Length() + 0.4f;

            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(npc.Bottom.X / 16), (int)(npc.Bottom.Y / 16) + 1);
            bool onSolidGround = WorldGen.SolidTile(tileBelow) || (tileBelow.active() && Main.tileSolidTop[tileBelow.type]);
            float directionSign = Math.Sign(npc.velocity.X);
            if (directionSign == 0f)
                directionSign = npc.spriteDirection;
            if (Math.Abs(npc.velocity.X) > 0.5f && onSolidGround)
                npc.velocity.X = MathHelper.Lerp(npc.velocity.X, directionSign * 5f, 0.05f);

            // Enforce gravity.
            npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y + Gravity, -15f, 15f);

            // Handle directioning.
            if (Math.Abs(npc.velocity.X) > 0.4f)
                npc.spriteDirection = (npc.velocity.X > 0f).ToDirectionInt();

            // Attempt to jump over gaps if they aren't too big.
            Vector2 checkPosition = npc.Bottom + new Vector2(Math.Sign(npc.velocity.X) * 24f, 12f);
            float? distanceToAheadBelowTile = CalamityUtils.DistanceToTileCollisionHit(checkPosition, Vector2.UnitX * directionSign);
            if (onSolidGround && distanceToAheadBelowTile.HasValue && distanceToAheadBelowTile.Value >= 2 && distanceToAheadBelowTile.Value < 9)
            {
                Vector2 jumpDestination = npc.Center + Vector2.UnitX * npc.spriteDirection * (distanceToAheadBelowTile.Value * 16f + 8f);
                npc.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(npc.Center, jumpDestination, Gravity, 6f);
            }

            // Attempt to jump over vertical obstacles.
            bool obstacleInWay = !Collision.CanHit(npc.Center - Vector2.UnitX * directionSign * 8f, 2, 2, npc.Center + Vector2.UnitX * directionSign * 80f, 8, 8);
            if (onSolidGround && obstacleInWay)
            {
                Vector2 jumpDestination = npc.Center + Vector2.UnitX * npc.spriteDirection * 132f;
                float? distanceToObstacle = CalamityUtils.DistanceToTileCollisionHit(npc.Center, Vector2.UnitX * directionSign);
                float obstacleHeight = 0f;
                for (int i = 0; i < 15; i++)
                {
                    if (WorldGen.SolidTile((int)(npc.Center.X / 16 + (distanceToObstacle ?? 0) * npc.spriteDirection), (int)(npc.Center.Y / 16) - i))
                        obstacleHeight++;
                }
                npc.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(npc.Center, jumpDestination, Gravity, 6f + StuckCount * 0.7f + obstacleHeight * 0.4f);

                if (MathHelper.Distance(npc.position.X, npc.oldPosition.X) < 0.2f)
                {
                    StuckCount++;
                    if (StuckCount > 1f && !WantsToClimbOnSomeWall)
                        WantsToClimbOnSomeWall = true;
                    if (StuckCount > 2f)
                    {
                        npc.velocity.X *= -0.56f;
                        StuckCount = 0f;
                    }
                    PreviousStuckPosition = npc.Center;
                }
                npc.netUpdate = true;
            }

            bool closeObstacleInWay = !Collision.CanHit(npc.Center, 2, 2, npc.Center + Vector2.UnitX * directionSign * 34f, 8, 8);
            if (onSolidGround && closeObstacleInWay)
            {
                npc.velocity.X *= -0.7f;
                npc.netUpdate = true;
            }

            // Reset the stuck counter if the NPC has become sufficiently far away from the previous stuck postion, indicating that
            // it is in fact, no longer stuck.
            if (!npc.WithinRange(PreviousStuckPosition, 180f) && StuckCount > 0f)
            {
                StuckCount = 0f;
                npc.netUpdate = true;
            }

            // Handle walking frames.
            if (npc.frameCounter >= 11f && Math.Abs(npc.velocity.Y) < 1f)
            {
                CurrentFrame = (CurrentFrame + 1f) % 8f;
                npc.frameCounter = 0;
            }

            if (Time % 300f == 299f && !WantsToClimbOnSomeWall)
            {
                WantsToClimbOnSomeWall = true;
                npc.netUpdate = true;
            }

            // If there are walls to clime on and this NPC wants to climb on some, do so.
            if (WantsToClimbOnSomeWall && CalamityUtils.ParanoidTileRetrieval((int)npc.Center.X / 16, (int)npc.Center.Y / 16).wall > 0)
            {
                CurrentState = BehaviorState.WalkOnWalls;
                StuckCount = 0f;
                WantsToClimbOnSomeWall = false;
                npc.position.Y -= 10f;
                npc.netUpdate = true;
            }
        }

        public void WalkOnWalls()
        {
            StuckCount = 0f;
            WantsToClimbOnSomeWall = false;
            npc.Calamity().ShouldFallThroughPlatforms = true;

            // Generate unusual movement patterns based on sines.
            // When two periodic functions are summed, the resulting function is periodic if the
            // ratio of the b/a is rational, given periodic functions f and g:
            // f(a * x) + g(b * x). However, if the ratio is irrational, then the result has no period.
            // This is desirable for somewhat random wavy fluctuations.
            // In this case, pi/1 (or simply pi) is used, which is indeed an irrational number.
            float offsetAngle = (float)(Math.Sin(Time / 360f) + Math.Sin(MathHelper.Pi * Time / 360f)) * 0.003f;

            Vector2 currentDirection = npc.velocity.SafeNormalize((npc.rotation - MathHelper.PiOver2).ToRotationVector2());

            // Turn much more quickly if there's no wall ahead.
            Vector2 aheadCheckPosition = npc.Center + currentDirection * 120f;
            Tile aheadTile = CalamityUtils.ParanoidTileRetrieval((int)(aheadCheckPosition.X / 16), (int)(aheadCheckPosition.Y / 16));
            bool aboutToCollideWithSomething = CalamityUtils.DistanceToTileCollisionHit(npc.Center - currentDirection * 5f, currentDirection) < 6f;
            bool almostReadyToWalkAgain = ClimbTime > 445f;

            if (((aheadTile.wall == WallID.None || WorldGen.SolidTile(aheadTile)) && !almostReadyToWalkAgain) || aboutToCollideWithSomething)
            {
                float distanceToCollisionLeft = CalamityUtils.DistanceToTileCollisionHit(npc.Center - currentDirection.RotatedBy(MathHelper.PiOver2) * 5f, currentDirection.RotatedBy(MathHelper.PiOver2)) ?? 10000f;
                float distanceToCollisionRight = CalamityUtils.DistanceToTileCollisionHit(npc.Center - currentDirection.RotatedBy(-MathHelper.PiOver2) * 5f, currentDirection.RotatedBy(-MathHelper.PiOver2)) ?? 10000f;
                float steerRotation = distanceToCollisionLeft > distanceToCollisionRight ? MathHelper.PiOver2 : -MathHelper.PiOver2;
                Vector2 idealVelocity = npc.velocity.RotatedBy(steerRotation);
                npc.velocity = npc.velocity.MoveTowards(idealVelocity, 0.95f);
            }
            else
                npc.velocity = npc.velocity.RotatedBy(offsetAngle);
            npc.velocity = npc.velocity.SafeNormalize(Vector2.Zero) * 1.56f;

            // Rebound on collision.
            if (npc.collideX)
            {
                npc.velocity.X *= -0.8f;
                npc.Center += npc.velocity * 2f;
            }
            if (npc.collideY)
            {
                npc.velocity.Y *= -0.8f;
                npc.Center += npc.velocity * 2f;
            }

            if (npc.velocity.Length() > 1.4f)
                npc.rotation = npc.rotation.AngleLerp(npc.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);

            if (Collision.SolidCollision(npc.position + Vector2.One * 24f, npc.width - 12, npc.height - 12))
            {
                npc.velocity *= -1f;
                npc.position.Y -= 5f;
            }

            // Handle climbing frames.
            npc.frameCounter++;
            if (npc.frameCounter >= 4f)
            {
                if (CurrentFrame < 9)
                    CurrentFrame = 9f;
                CurrentFrame++;
                if (CurrentFrame >= Main.npcFrameCount[npc.type])
                    CurrentFrame = 9f;
                npc.frameCounter = 0;
            }

            if (CalamityUtils.ParanoidTileRetrieval((int)(npc.Center.X / 16), (int)(npc.Center.Y / 16)).wall == WallID.None || ClimbTime > 620f)
            {
                CurrentState = BehaviorState.WalkAround;
                npc.velocity = new Vector2(Main.rand.NextBool(2).ToDirectionInt() * 1.2f, 3f);
                npc.rotation = 0f;
                npc.netUpdate = true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frame.Y = (int)(CurrentFrame * frameHeight);
        }
    }
}
