using CalamityMod.BiomeManagers;
using CalamityMod.Items.Critters;
using CalamityMod.Items.DraedonMisc;
using CalamityMod.Items.Placeables.Banners;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
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
            get => (BehaviorState)(int)NPC.ai[0];
            set => NPC.ai[0] = (int)value;
        }

        public ref float StuckCount => ref NPC.ai[1];

        public bool Initialized
        {
            get => NPC.ai[2] == 1f;
            set => NPC.ai[2] = value.ToInt();
        }
        public bool WantsToClimbOnSomeWall
        {
            get => NPC.ai[3] == 1f;
            set => NPC.ai[3] = value.ToInt();
        }

        public ref float CurrentFrame => ref NPC.localAI[0];
        public ref float Time => ref NPC.localAI[1];
        public ref float ClimbTime => ref NPC.localAI[2];

        public const float Gravity = 0.4f;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 17;
            Main.npcCatchable[NPC.type] = true;
            NPCID.Sets.CountsAsCritter[NPC.type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
            NPCID.Sets.NormalGoldCritterBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            value.Position.Y += 12;
            value.PortraitPositionYOverride = 32f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = 0;
            NPC.width = 20;
            NPC.height = 22;
            NPC.lifeMax = 80;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = false;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath44;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<RepairUnitBanner>();
            NPC.catchItem = (short)ModContent.ItemType<RepairUnitItem>();
            SpawnModBiomes = new int[1] { ModContent.GetInstance<ArsenalLabBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.RepairUnitCritter")
            });
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

        public override bool? CanBeHitByItem(Player player, Item item) => null;

        public override bool? CanBeHitByProjectile(Projectile projectile) => null;

        public override void AI()
        {
            if (!Initialized)
            {
                NPC.velocity = Vector2.UnitX * Main.rand.NextBool(2).ToDirectionInt() * 1.5f;
                Initialized = true;
            }

            NPC.Calamity().ShouldFallThroughPlatforms = false;
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

            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player is null || !player.active)
                    continue;

                if (NPC.Hitbox.Intersects(player.HitboxForBestiaryNearbyCheck))
                {
                    Main.BestiaryTracker.Kills.RegisterKill(NPC);
                    break;
                }
            }
        }

        public void WalkAroundOnGround()
        {
            // Fall onto the ground if mid-air.

            Tile tileBelow = CalamityUtils.ParanoidTileRetrieval((int)(NPC.Bottom.X / 16f), (int)(NPC.Bottom.Y / 16f));
            bool onSolidGround = WorldGen.SolidTile(tileBelow) || (tileBelow.HasTile && Main.tileSolidTop[tileBelow.TileType]);
            float directionSign = Math.Sign(NPC.velocity.X);
            if (directionSign == 0f)
                directionSign = NPC.spriteDirection;
            if (Math.Abs(NPC.velocity.X) > 0.5f && onSolidGround)
                NPC.velocity.X = MathHelper.Lerp(NPC.velocity.X, directionSign * 5f, 0.05f);

            // Using walking frames if on solid ground.
            // If not on solid ground, use the first frame.
            if (onSolidGround)
                NPC.frameCounter += NPC.velocity.Length() + 0.4f;
            else
                CurrentFrame = 0f;

            // Enforce gravity.
            NPC.velocity.Y = MathHelper.Clamp(NPC.velocity.Y + Gravity, -15f, 15f);

            // Handle directioning.
            if (Math.Abs(NPC.velocity.X) > 0.4f)
                NPC.spriteDirection = (NPC.velocity.X > 0f).ToDirectionInt();

            // Attempt to jump over gaps if they aren't too big.
            Vector2 checkPosition = NPC.Bottom + new Vector2(Math.Sign(NPC.velocity.X) * 24f, 12f);

            float? distanceToAheadBelowTile = CalamityUtils.DistanceToTileCollisionHit(checkPosition, Vector2.UnitX * directionSign);
            if (onSolidGround && distanceToAheadBelowTile.HasValue && distanceToAheadBelowTile.Value >= 2 && distanceToAheadBelowTile.Value < 9 && NPC.velocity.Y == Gravity)
            {
                Vector2 jumpDestination = NPC.Center + Vector2.UnitX * NPC.spriteDirection * (distanceToAheadBelowTile.Value * 16f + 12f);
                NPC.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(NPC.Center, jumpDestination, Gravity, 10f);
            }

            // Attempt to jump over vertical obstacles.
            bool obstacleInWay = !Collision.CanHit(NPC.Center - Vector2.UnitX * directionSign * 8f, 2, 2, NPC.Center + Vector2.UnitX * directionSign * 50f, 8, 8);
            if (onSolidGround && obstacleInWay)
            {
                Vector2 jumpDestination = NPC.Center + Vector2.UnitX * NPC.spriteDirection * 132f;
                float? distanceToObstacle = CalamityUtils.DistanceToTileCollisionHit(NPC.Center, Vector2.UnitX * directionSign);
                float obstacleHeight = 0f;
                for (int i = 0; i < 10; i++)
                {
                    if (WorldGen.SolidTile((int)(NPC.Center.X / 16 + (distanceToObstacle ?? 0) * NPC.spriteDirection), (int)(NPC.Center.Y / 16) - i))
                        obstacleHeight++;
                }

                // Just turn back if the obstacle is ridiculously tall.
                if (obstacleHeight >= 10)
                    NPC.velocity.X = -NPC.spriteDirection * 3f;

                // Otherwise try to jump over it.
                else
                    NPC.velocity = CalamityUtils.GetProjectilePhysicsFiringVelocity(NPC.Center, jumpDestination, Gravity, 9f + StuckCount * 0.7f + obstacleHeight * 0.4f);

                if (MathHelper.Distance(NPC.position.X, NPC.oldPosition.X) < 0.2f)
                {
                    StuckCount++;
                    if (StuckCount > 1f && !WantsToClimbOnSomeWall)
                        WantsToClimbOnSomeWall = true;
                    if (StuckCount > 2f)
                    {
                        NPC.velocity.X *= -0.56f;
                        StuckCount = 0f;
                    }
                    PreviousStuckPosition = NPC.Center;
                }
                NPC.netUpdate = true;
            }

            bool closeObstacleInWay = !Collision.CanHit(NPC.Center, 2, 2, NPC.Center + Vector2.UnitX * directionSign * 34f, 8, 8);
            if (onSolidGround && closeObstacleInWay)
            {
                NPC.velocity.X *= -0.7f;
                NPC.netUpdate = true;
            }

            // Reset the stuck counter if the NPC has become sufficiently far away from the previous stuck postion, indicating that
            // it is in fact, no longer stuck.
            if (!NPC.WithinRange(PreviousStuckPosition, 180f) && StuckCount > 0f)
            {
                StuckCount = 0f;
                NPC.netUpdate = true;
            }

            // Handle walking frames.
            if (NPC.frameCounter >= 11f && Math.Abs(NPC.velocity.Y) < 1f)
            {
                CurrentFrame = (CurrentFrame + 1f) % 8f;
                NPC.frameCounter = 0;
            }

            if (Time % 300f == 299f && !WantsToClimbOnSomeWall)
            {
                WantsToClimbOnSomeWall = true;
                NPC.netUpdate = true;
            }

            // If there are walls to clime on and this NPC wants to climb on some, do so.
            if (WantsToClimbOnSomeWall && CalamityUtils.ParanoidTileRetrieval((int)NPC.Center.X / 16, (int)NPC.Center.Y / 16).WallType > 0)
            {
                CurrentState = BehaviorState.WalkOnWalls;
                StuckCount = 0f;
                WantsToClimbOnSomeWall = false;
                NPC.position.Y -= 10f;
                NPC.netUpdate = true;
            }
        }

        public void WalkOnWalls()
        {
            StuckCount = 0f;
            WantsToClimbOnSomeWall = false;
            NPC.Calamity().ShouldFallThroughPlatforms = true;

            // Generate unusual movement patterns based on sines.
            float offsetAngle = CalamityUtils.AperiodicSin(Time / 360f, 0f, 1f) * 0.006f;

            Vector2 currentDirection = NPC.velocity.SafeNormalize((NPC.rotation - MathHelper.PiOver2).ToRotationVector2());

            // Turn much more quickly if there's no wall ahead.
            Vector2 aheadCheckPosition = NPC.Center + currentDirection * 120f;
            Tile aheadTile = CalamityUtils.ParanoidTileRetrieval((int)(aheadCheckPosition.X / 16), (int)(aheadCheckPosition.Y / 16));
            bool aboutToCollideWithSomething = CalamityUtils.DistanceToTileCollisionHit(NPC.Center - currentDirection * 5f, currentDirection) < 6f;
            bool almostReadyToWalkAgain = ClimbTime > 445f;

            if (((aheadTile.WallType == WallID.None || WorldGen.SolidTile(aheadTile)) && !almostReadyToWalkAgain) || aboutToCollideWithSomething)
            {
                float distanceToCollisionLeft = CalamityUtils.DistanceToTileCollisionHit(NPC.Center - currentDirection.RotatedBy(MathHelper.PiOver2) * 5f, currentDirection.RotatedBy(MathHelper.PiOver2)) ?? 10000f;
                float distanceToCollisionRight = CalamityUtils.DistanceToTileCollisionHit(NPC.Center - currentDirection.RotatedBy(-MathHelper.PiOver2) * 5f, currentDirection.RotatedBy(-MathHelper.PiOver2)) ?? 10000f;
                float steerRotation = distanceToCollisionLeft > distanceToCollisionRight ? MathHelper.PiOver2 : -MathHelper.PiOver2;
                Vector2 idealVelocity = NPC.velocity.RotatedBy(steerRotation);
                NPC.velocity = NPC.velocity.MoveTowards(idealVelocity, 0.15f);
            }
            else
                NPC.velocity = NPC.velocity.RotatedBy(offsetAngle);
            NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 1.56f;

            // Rebound on collision.
            if (NPC.collideX)
            {
                NPC.velocity.X *= -0.8f;
                NPC.Center += NPC.velocity * 2f;
            }
            if (NPC.collideY)
            {
                NPC.velocity.Y *= -0.8f;
                NPC.Center += NPC.velocity * 2f;
            }

            if (NPC.velocity.Length() > 1.4f)
                NPC.rotation = NPC.rotation.AngleLerp(NPC.velocity.ToRotation() + MathHelper.PiOver2, 0.2f);

            if (Collision.SolidCollision(NPC.position + Vector2.One * 24f, NPC.width - 12, NPC.height - 12))
            {
                NPC.velocity *= -1f;
                NPC.position.Y -= 5f;
            }

            // Handle climbing frames.
            NPC.frameCounter++;
            if (NPC.frameCounter >= 4f)
            {
                if (CurrentFrame < 9)
                    CurrentFrame = 9f;
                CurrentFrame++;
                if (CurrentFrame >= Main.npcFrameCount[NPC.type])
                    CurrentFrame = 9f;
                NPC.frameCounter = 0;
            }

            if (CalamityUtils.ParanoidTileRetrieval((int)(NPC.Center.X / 16), (int)(NPC.Center.Y / 16)).WallType == WallID.None || ClimbTime > 620f)
            {
                CurrentState = BehaviorState.WalkAround;
                NPC.velocity = new Vector2(Main.rand.NextBool(2).ToDirectionInt() * 1.2f, 3f);
                NPC.rotation = 0f;
                NPC.netUpdate = true;
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int i = 0; i < 6; i++)
                Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, 226);
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    for (int i = 1; i <= 3; i++)
                        Gore.NewGorePerfect(NPC.GetSource_Death(), NPC.Center, NPC.velocity.RotatedByRandom(0.3f) * Main.rand.NextFloat(0.7f, 1f), Mod.Find<ModGore>($"RepairUnit{i}").Type);
                }
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ModContent.ItemType<DraedonPowerCell>(), 2, 2, 4);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1.8f;
                if (NPC.frameCounter >= 11f)
                {
                    CurrentFrame++;
                    if (CurrentFrame >= 9)
                        CurrentFrame = 1;

                    NPC.frameCounter = 0;
                }
            }

            NPC.frame.Y = (int)(CurrentFrame * frameHeight);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D critterTexture = TextureAssets.Npc[NPC.type].Value;
            Texture2D glowmask = ModContent.Request<Texture2D>("CalamityMod/NPCs/DraedonLabThings/RepairUnitCritterGlowmask").Value;
            Vector2 drawPosition = NPC.Center - screenPos + Vector2.UnitY * NPC.gfxOffY;
            SpriteEffects direction = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(critterTexture, drawPosition, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            spriteBatch.Draw(glowmask, drawPosition, NPC.frame, NPC.GetAlpha(Color.White), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, direction, 0f);
            return false;
        }
    }
}
