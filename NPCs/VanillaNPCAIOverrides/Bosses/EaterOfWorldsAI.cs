using System;
using CalamityMod.Events;
using CalamityMod.Packets;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class EaterOfWorldsAI : VanillaAIOverride
    {
        private const float ProjectileTelegraphDuration = 30f;
        private const int TotalDeathModeWorms = 4;
        public const float DRIncreaseTime = 600f;

        // Rev+ exclusive
        public static float HeadDamageMult = 1.25f; // 60 (buffed from 48)
        public static float BodyDamageMult = 1.5f; // 30 (buffed from 20)
        public static float TailDamageMult = 1.5f; // 26 (buffed from 17)
        public static int FireballDamage = 12; // 48; Applies to both Cursed Flames and (Death) Shadowflame fireballs

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Set contact damage
            NPC.damage = (int)Math.Round(NPC.defDamage * (NPC.type == NPCID.EaterofWorldsHead ? HeadDamageMult : NPC.type == NPCID.EaterofWorldsBody ? BodyDamageMult : TailDamageMult));

            // Causes it to split far more in death mode
            if ((((NPC.ai[2] % 2f == 0f && NPC.type == NPCID.EaterofWorldsBody) || NPC.type == NPCID.EaterofWorldsHead) && death) || Main.getGoodWorld)
            {
                calamityGlobalNPC.DR = 0.5f;
                NPC.defense = NPC.defDefense * 2;
            }

            if (Main.getGoodWorld && NPC.type == NPCID.EaterofWorldsHead)
                NPC.reflectsProjectiles = true;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            // Total body segments
            float totalSegments = GetEaterOfWorldsSegmentsCountRevDeath();

            // Count body segments remaining
            float segmentCount = NPC.CountNPCS(NPCID.EaterofWorldsBody);

            // Percent body segments remaining
            float lifeRatio = MathHelper.Clamp(segmentCount / totalSegments, 0f, 1f);

            // Phases

            // Cursed Flame phase
            bool phase2 = lifeRatio < 0.8f || death;

            // Boost velocity by 20% phase
            bool phase3 = lifeRatio < 0.4f || death;

            // Boost velocity by 50% phase
            bool phase4 = lifeRatio < (death ? 0.5f : 0.2f);

            // Go fucking crazy in Death Mode
            bool phase5 = lifeRatio < 0.1f && death;
            bool phase6 = lifeRatio < 0.05f && death;

            // Fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Vile spit
                if (NPC.type == NPCID.EaterofWorldsBody)
                {
                    if (Collision.CanHitLine(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                        NPC.localAI[1] += 1f;
                    else
                        NPC.localAI[1] -= 1f;

                    int vileSpitGateValue = (int)MathHelper.Lerp(death ? 45f : 90f, 900f, lifeRatio);
                    if (Main.getGoodWorld)
                        vileSpitGateValue = (int)(vileSpitGateValue * 0.5f);

                    Vector2 vileSpitShootLocation = NPC.Center + NPC.velocity;
                    if (NPC.localAI[1] >= vileSpitGateValue)
                    {
                        CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                        if (Collision.CanHitLine(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)vileSpitShootLocation.X, (int)vileSpitShootLocation.Y, NPCID.VileSpitEaterOfWorlds, 0, 0f, 1f);

                        NPC.localAI[1] = 0f;
                    }

                    if (NPC.localAI[1] > vileSpitGateValue - ProjectileTelegraphDuration)
                    {
                        Vector2 dustCenter = vileSpitShootLocation + Main.rand.NextVector2CircularEdge(5f, 5f);
                        Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.CorruptGibs, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 80, default, 2f);
                        dust.noGravity = true;
                        dust.velocity *= 0.3f;
                    }
                }

                // Cursed flames (shadowflames in death mode)
                else if (NPC.type == NPCID.EaterofWorldsHead)
                {
                    if (phase2)
                    {
                        float timer = 120f;
                        float shootBoost = lifeRatio * 90f;
                        timer += shootBoost;

                        float showTelegraphGateValue = timer - ProjectileTelegraphDuration;

                        if (Collision.CanHitLine(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1))
                        {
                            if (NPC.justHit && death && calamityGlobalNPC.newAI[0] < showTelegraphGateValue)
                            {
                                calamityGlobalNPC.newAI[0] += 10f;
                                if (calamityGlobalNPC.newAI[0] > showTelegraphGateValue)
                                    calamityGlobalNPC.newAI[0] = showTelegraphGateValue;
                            }
                            else
                                calamityGlobalNPC.newAI[0] += 1f;
                        }
                        else
                            calamityGlobalNPC.newAI[0] -= 1f;

                        if (calamityGlobalNPC.newAI[0] >= timer)
                        {
                            if (Collision.CanHitLine(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1) &&
                                (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation())
                            {
                                calamityGlobalNPC.newAI[0] = 0f;
                                Vector2 cursedFlameDirection = Utils.DirectionTo(NPC.Center, Main.player[NPC.target].Center) * 7f + (NPC.velocity * 0.5f);
                                int type = (death && phase3) ? ModContent.ProjectileType<ShadowflameFireball>() : ProjectileID.CursedFlameHostile;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity, cursedFlameDirection, type, FireballDamage, 0f, Main.myPlayer);
                            }
                        }

                        if (calamityGlobalNPC.newAI[0] > showTelegraphGateValue)
                        {
                            Vector2 dustCenter = NPC.Center + Main.rand.NextVector2CircularEdge(10f, 10f);
                            int dustType = (death && phase3) ? DustID.Shadowflame : DustID.CursedTorch;
                            Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, dustType, 0f, 0f, 0, default, 3f);
                            dust.noGravity = true;
                            dust.velocity *= 0f;
                        }
                    }
                }
            }

            // Despawn
            if (Main.player[NPC.target].dead)
            {
                if (NPC.timeLeft > 300)
                    NPC.timeLeft = 300;
            }

            // All functions that modify the active worm segments are here. This includes spawning the worm originally and splitting effects.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // If this segment is a head or a body without a next-segment defined, then it needs to spawn its own next segment.
                if ((NPC.type == NPCID.EaterofWorldsHead || NPC.type == NPCID.EaterofWorldsBody) && NPC.ai[0] == 0f)
                {
                    int spawnX = (int)NPC.position.X;
                    int spawnY = (int)NPC.position.Y;

                    // A head sets the length variable (npc.ai[2]) and then sets its next segment to a freshly spawned body.
                    if (NPC.type == NPCID.EaterofWorldsHead)
                    {
                        // Amount of segments to spawn.
                        int segmentSpawnAmount = (int)(death ? (totalSegments / TotalDeathModeWorms) : totalSegments);

                        // Spawn additional worms of reduced length in Master Mode.
                        if (death)
                        {
                            Vector2 additionalWormSpawnLocation = new Vector2(spawnX, spawnY);
                            int randomXLimit = 80;
                            int randomYLimit = 80;
                            for (int i = 1; i < TotalDeathModeWorms; i++)
                            {
                                additionalWormSpawnLocation += new Vector2((Main.rand.Next(randomXLimit + 1) + randomXLimit) * (Main.rand.NextBool() ? -1f : 1f), Main.rand.Next(randomYLimit + 1) + randomYLimit);
                                int wormHead = NPC.NewNPC(NPC.GetSource_FromAI(), (int)additionalWormSpawnLocation.X, (int)additionalWormSpawnLocation.Y, NPCID.EaterofWorldsHead, NPC.whoAmI + segmentSpawnAmount * i + 1);
                                Main.npc[wormHead].ai[2] = segmentSpawnAmount;
                                Main.npc[wormHead].ai[0] = NPC.NewNPC(Main.npc[wormHead].GetSource_FromAI(), (int)additionalWormSpawnLocation.X, (int)additionalWormSpawnLocation.Y, NPCID.EaterofWorldsBody, Main.npc[wormHead].whoAmI);
                                Main.npc[(int)Main.npc[wormHead].ai[0]].ai[1] = Main.npc[wormHead].whoAmI;
                                Main.npc[(int)Main.npc[wormHead].ai[0]].ai[2] = Main.npc[wormHead].ai[2] - 1f;
                                Main.npc[wormHead].netUpdate = true;
                            }
                        }

                        // Set head's "length beyond this point" to be the total length of the worm.
                        NPC.ai[2] = segmentSpawnAmount;

                        // Body spawn
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), spawnX, spawnY, NPCID.EaterofWorldsBody, NPC.whoAmI);
                    }

                    // A body with a "length beyond this point" greater than zero just sets its next spawned segment to a freshly spawned body.
                    else if (NPC.type == NPCID.EaterofWorldsBody && NPC.ai[2] > 0f)
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), spawnX, spawnY, NPCID.EaterofWorldsBody, NPC.whoAmI);

                    // If the worm stops here ("length beyond this point" is zero), then spawn a tail instead.
                    else
                        NPC.ai[0] = NPC.NewNPC(NPC.GetSource_FromAI(), spawnX, spawnY, NPCID.EaterofWorldsTail, NPC.whoAmI);

                    // Maintain the linked list of worm segments, and correctly set the "length beyond this point" of this segment.
                    Main.npc[(int)NPC.ai[0]].ai[1] = NPC.whoAmI;
                    Main.npc[(int)NPC.ai[0]].ai[2] = NPC.ai[2] - 1f;
                    NPC.netUpdate = true;
                }

                // Helper function to destroy this Eater of Worlds worm segment.
                void DestroyThisSegment()
                {
                    NPC.life = 0;
                    NPC.HitEffect(0, 10.0);
                    NPC.checkDead();
                }

                // If this segment's previous and next segments are both dead, make it explode instantly. Single segments cannot live.
                if (!Main.npc[(int)NPC.ai[1]].active && !Main.npc[(int)NPC.ai[0]].active)
                    DestroyThisSegment();

                // If this segment is a head and its next segment is dead, make it explode instantly. It's been decapitated.
                if (NPC.type == NPCID.EaterofWorldsHead && !Main.npc[(int)NPC.ai[0]].active)
                    DestroyThisSegment();

                // If this segment is a tail and its previous segment is dead, make it explode instantly. It's been chopped off.
                if (NPC.type == NPCID.EaterofWorldsTail && !Main.npc[(int)NPC.ai[1]].active)
                    DestroyThisSegment();

                // If this segment is a body and its previous segment is dead (or was rendered into a tail), transform into a head.
                if (NPC.type == NPCID.EaterofWorldsBody && (!Main.npc[(int)NPC.ai[1]].active || Main.npc[(int)NPC.ai[1]].aiStyle != NPC.aiStyle))
                {
                    NPC.type = NPCID.EaterofWorldsHead;
                    float segmentLifeRatio = MathHelper.Lerp(0.5f, 1f, NPC.life / (float)NPC.lifeMax);
                    int whoAmI = NPC.whoAmI;
                    float ai0Holdover = NPC.ai[0];
                    float newAI1Holdover = calamityGlobalNPC.newAI[1];

                    // Actually transform the body segment into a head segment.
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * segmentLifeRatio);
                    NPC.whoAmI = whoAmI;
                    NPC.ai[0] = ai0Holdover;
                    // Heads spawned mid fight by splitting do not get reset spawn invincibility.
                    CalamityGlobalNPC newCGN = NPC.Calamity();
                    newCGN.newAI[1] = newAI1Holdover;

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                    NPC.ForceNetUpdate();
                    NPC.alpha = 0;
                }

                // If this segment is a body and its next segment is dead (or was rendered into a head), transform into a tail.
                if (NPC.type == NPCID.EaterofWorldsBody && (!Main.npc[(int)NPC.ai[0]].active || Main.npc[(int)NPC.ai[0]].aiStyle != NPC.aiStyle))
                {
                    NPC.type = NPCID.EaterofWorldsTail;
                    float segmentLifeRatio = MathHelper.Lerp(0.5f, 1f, NPC.life / (float)NPC.lifeMax);
                    int whoAmI = NPC.whoAmI;
                    float ai1Holdover = NPC.ai[1];

                    // Actually transform the body segment into a tail segment.
                    NPC.SetDefaultsKeepPlayerInteraction(NPC.type);
                    NPC.life = (int)(NPC.lifeMax * segmentLifeRatio);
                    NPC.whoAmI = whoAmI;
                    NPC.ai[1] = ai1Holdover;

                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

                    NPC.ForceNetUpdate();
                    NPC.alpha = 0;
                }

                // If for any reason this segment was deleted, send info to clients so they also see it die.
                if (!NPC.active && Main.dedServ)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f);
            }

            // Movement
            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
                tilePositionX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePositionY < 0)
                tilePositionY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            // Fly or not
            bool inTiles = false;
            if (!inTiles)
            {
                for (int i = tilePositionX; i < tileWidthPosX; i++)
                {
                    for (int j = tilePositionY; j < tileWidthPosY; j++)
                    {
                        if (Main.tile[i, j] != null && ((Main.tile[i, j].HasUnactuatedTile && (Main.tileSolid[Main.tile[i, j].TileType] || (Main.tileSolidTop[Main.tile[i, j].TileType] && Main.tile[i, j].TileFrameY == 0))) || Main.tile[i, j].LiquidAmount > 64))
                        {
                            Vector2 vector;
                            vector.X = i * 16;
                            vector.Y = j * 16;
                            if (NPC.position.X + NPC.width > vector.X && NPC.position.X < vector.X + 16f && NPC.position.Y + NPC.height > vector.Y && NPC.position.Y < vector.Y + 16f)
                            {
                                inTiles = true;
                                if (Main.rand.NextBool(100) && Main.tile[i, j].HasUnactuatedTile)
                                    WorldGen.KillTile(i, j, true, true, false);
                            }
                        }
                    }
                }
            }

            if (!inTiles && NPC.type == NPCID.EaterofWorldsHead)
            {
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int noFlyZone = death ? (phase5 ? 400 : 600) : 900;

                bool freeMoveAnyway = true;
                for (int k = 0; k < Main.maxPlayers; k++)
                {
                    if (Main.player[k].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[k].position.X - noFlyZone, (int)Main.player[k].position.Y - noFlyZone, noFlyZone * 2, noFlyZone * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            freeMoveAnyway = false;
                            break;
                        }
                    }
                }

                if (freeMoveAnyway)
                    inTiles = true;
            }

            // Velocity and acceleration
            float velocityScale = death ? 4.8f : 2.4f;
            float velocityBoost = velocityScale * (1f - lifeRatio);
            float accelerationScale = death ? 0.06f : 0.03f;
            float accelerationBoost = accelerationScale * (1f - lifeRatio);
            float segmentVelocity = 12f + velocityBoost;
            float segmentAcceleration = 0.15f + accelerationBoost;

            if (phase6)
            {
                segmentVelocity += 2.4f;
                segmentAcceleration += 0.12f;
            }
            else if (phase5)
            {
                segmentVelocity += 1.8f;
                segmentAcceleration += 0.09f;
            }
            else if (phase4)
            {
                segmentVelocity += 1.2f;
                segmentAcceleration += 0.06f;
            }
            else if (phase3)
            {
                segmentVelocity += 0.6f;
                segmentAcceleration += 0.03f;
            }

            if (death)
            {
                segmentVelocity += (NPC.justHit ? 8f : 2f);
                segmentAcceleration += (NPC.justHit ? 0.16f : 0.04f);
            }

            if (Main.getGoodWorld)
            {
                segmentVelocity += 4f;
                segmentAcceleration += 0.05f;
            }

            Vector2 segmentDirection = NPC.Center;
            Vector2 destination = Main.player[NPC.target].Center + (phase6 ? Main.player[NPC.target].velocity * 20f : Vector2.Zero);
            float targetPosX = destination.X;
            float targetPosY = destination.Y;

            targetPosX = (int)(targetPosX / 16f) * 16;
            targetPosY = (int)(targetPosY / 16f) * 16;
            segmentDirection.X = (int)(segmentDirection.X / 16f) * 16;
            segmentDirection.Y = (int)(segmentDirection.Y / 16f) * 16;
            targetPosX -= segmentDirection.X;
            targetPosY -= segmentDirection.Y;
            float targetDistance = (float)Math.Sqrt(targetPosX * targetPosX + targetPosY * targetPosY);

            // Does this worm segment have a "previous segment" defined?
            if (NPC.ai[1] > 0f && NPC.ai[1] < Main.npc.Length)
            {
                try
                {
                    segmentDirection = NPC.Center;
                    targetPosX = Main.npc[(int)NPC.ai[1]].Center.X - segmentDirection.X;
                    targetPosY = Main.npc[(int)NPC.ai[1]].Center.Y - segmentDirection.Y;
                }
                catch
                {
                }

                NPC.rotation = (float)Math.Atan2(targetPosY, targetPosX) + MathHelper.PiOver2;
                targetDistance = (float)Math.Sqrt(targetPosX * targetPosX + targetPosY * targetPosY);
                int npcWidth = NPC.width;
                npcWidth = (int)(npcWidth * NPC.scale);

                if (Main.getGoodWorld)
                    npcWidth = 62;

                targetDistance = (targetDistance - npcWidth) / targetDistance;
                targetPosX *= targetDistance;
                targetPosY *= targetDistance;
                NPC.velocity = Vector2.Zero;
                NPC.position.X += targetPosX;
                NPC.position.Y += targetPosY;
            }

            // Otherwise this is a head. (Why does this not just check for head NPC type?)
            else
            {
                // Prevent new heads from being slowed when they spawn
                if (calamityGlobalNPC.newAI[2] < 3f)
                {
                    calamityGlobalNPC.newAI[2] += 1f;

                    // Set velocity for when a new head spawns
                    // Only set this if the head is far enough away from the player, to avoid unfair hits
                    if (NPC.Distance(Main.player[NPC.target].Center) > segmentVelocity * 20f)
                        NPC.velocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * (segmentVelocity * (death ? 0.75f : 0.5f));
                }

                if (!inTiles)
                {
                    NPC.velocity.Y += death ? 0.1375f : 0.11f;
                    if (death && NPC.velocity.Y > 0f)
                        NPC.velocity.Y += 0.07f;

                    if (NPC.velocity.Y > segmentVelocity)
                        NPC.velocity.Y = segmentVelocity;

                    // This bool exists to stop the strange wiggle behavior when worms are falling down
                    bool slowXVelocity = Math.Abs(NPC.velocity.X) > segmentAcceleration;
                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.4)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X -= segmentAcceleration * 1.1f;
                        else
                            NPC.velocity.X += segmentAcceleration * 1.1f;
                    }
                    else if (NPC.velocity.Y == segmentVelocity)
                    {
                        if (slowXVelocity)
                        {
                            if (NPC.velocity.X < targetPosX)
                                NPC.velocity.X += segmentAcceleration;
                            else if (NPC.velocity.X > targetPosX)
                                NPC.velocity.X -= segmentAcceleration;
                        }
                        else
                            NPC.velocity.X = 0f;
                    }
                    else if (NPC.velocity.Y > (death ? 5f : 4f))
                    {
                        if (slowXVelocity)
                        {
                            if (NPC.velocity.X < 0f)
                                NPC.velocity.X += segmentAcceleration * 0.9f;
                            else
                                NPC.velocity.X -= segmentAcceleration * 0.9f;
                        }
                        else
                            NPC.velocity.X = 0f;
                    }
                }
                else
                {
                    // Sound
                    if (NPC.soundDelay == 0)
                    {
                        float soundDelay = targetDistance / 40f;
                        if (soundDelay < 10f)
                            soundDelay = 10f;
                        if (soundDelay > 20f)
                            soundDelay = 20f;

                        NPC.soundDelay = (int)soundDelay;
                        SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                    }

                    targetDistance = (float)Math.Sqrt(targetPosX * targetPosX + targetPosY * targetPosY);
                    float absoluteTargetX = Math.Abs(targetPosX);
                    float absoluteTargetY = Math.Abs(targetPosY);
                    float timeToReachTarget = segmentVelocity / targetDistance;
                    targetPosX *= timeToReachTarget;
                    targetPosY *= timeToReachTarget;

                    // Despawn
                    bool shouldDespawn = NPC.type == NPCID.EaterofWorldsHead && (Main.player[NPC.target].dead || !Main.player[NPC.target].ZoneCorrupt || !Main.player[NPC.target].ZoneCrimson) && !BossRushEvent.BossRushActive;
                    if (shouldDespawn)
                    {
                        bool everyoneDead = true;
                        foreach (Player p in Main.ActivePlayers)
                        {
                            if (!p.dead && p.ZoneCorrupt)
                            {
                                everyoneDead = false;
                                break;
                            }
                        }

                        if (everyoneDead)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && (NPC.position.Y / 16f) > (Main.rockLayer + Main.maxTilesY) / 2.0)
                            {
                                NPC.active = false;
                                int segmentAmt = (int)NPC.ai[0];

                                while (segmentAmt > 0 && segmentAmt < Main.maxNPCs && Main.npc[segmentAmt].active && Main.npc[segmentAmt].aiStyle == NPC.aiStyle)
                                {
                                    int attachedSegments = (int)Main.npc[segmentAmt].ai[0];
                                    Main.npc[segmentAmt].active = false;
                                    NPC.life = 0;

                                    if (Main.dedServ)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segmentAmt);

                                    segmentAmt = attachedSegments;
                                }

                                if (Main.dedServ)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, NPC.whoAmI);
                            }
                            targetPosX = 0f;
                            targetPosY = segmentVelocity;
                        }
                    }

                    if ((NPC.velocity.X > 0f && targetPosX > 0f) || (NPC.velocity.X < 0f && targetPosX < 0f) || (NPC.velocity.Y > 0f && targetPosY > 0f) || (NPC.velocity.Y < 0f && targetPosY < 0f))
                    {
                        if (NPC.velocity.X < targetPosX)
                            NPC.velocity.X += segmentAcceleration;
                        else if (NPC.velocity.X > targetPosX)
                            NPC.velocity.X -= segmentAcceleration;
                        if (NPC.velocity.Y < targetPosY)
                            NPC.velocity.Y += segmentAcceleration;
                        else if (NPC.velocity.Y > targetPosY)
                            NPC.velocity.Y -= segmentAcceleration;

                        if (Math.Abs(targetPosY) < segmentVelocity * 0.2 && ((NPC.velocity.X > 0f && targetPosX < 0f) || (NPC.velocity.X < 0f && targetPosX > 0f)))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += segmentAcceleration * 2f;
                            else
                                NPC.velocity.Y -= segmentAcceleration * 2f;
                        }

                        if (Math.Abs(targetPosX) < segmentVelocity * 0.2 && ((NPC.velocity.Y > 0f && targetPosY < 0f) || (NPC.velocity.Y < 0f && targetPosY > 0f)))
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += segmentAcceleration * 2f;
                            else
                                NPC.velocity.X -= segmentAcceleration * 2f;
                        }
                    }
                    else if (absoluteTargetX > absoluteTargetY)
                    {
                        if (NPC.velocity.X < targetPosX)
                            NPC.velocity.X += segmentAcceleration * 1.1f;
                        else if (NPC.velocity.X > targetPosX)
                            NPC.velocity.X -= segmentAcceleration * 1.1f;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.5)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += segmentAcceleration;
                            else
                                NPC.velocity.Y -= segmentAcceleration;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < targetPosY)
                            NPC.velocity.Y += segmentAcceleration * 1.1f;
                        else if (NPC.velocity.Y > targetPosY)
                            NPC.velocity.Y -= segmentAcceleration * 1.1f;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.5)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += segmentAcceleration;
                            else
                                NPC.velocity.X -= segmentAcceleration;
                        }
                    }
                }

                if (death)
                {
                    int numHeads = NPC.CountNPCS(NPC.type);
                    if (numHeads > 0)
                    {
                        // Limit this variable so that the following calculation never goes too low
                        numHeads--;
                        if (numHeads > 7)
                            numHeads = 7;

                        float pushDistanceLowerLimit = 14f - numHeads;
                        float pushDistanceUpperLimit = 140f - numHeads * 10f;
                        float pushDistance = MathHelper.Lerp(pushDistanceLowerLimit, pushDistanceUpperLimit, 1f - lifeRatio) * NPC.scale;
                        float pushVelocity = 0.25f;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (Main.npc[i].active)
                            {
                                if (i != NPC.whoAmI && Main.npc[i].type == NPC.type)
                                {
                                    if (Vector2.Distance(NPC.Center, Main.npc[i].Center) < pushDistance)
                                    {
                                        if (NPC.position.X < Main.npc[i].position.X)
                                            NPC.velocity.X -= pushVelocity;
                                        else
                                            NPC.velocity.X += pushVelocity;

                                        if (NPC.position.Y < Main.npc[i].position.Y)
                                            NPC.velocity.Y -= pushVelocity;
                                        else
                                            NPC.velocity.Y += pushVelocity;
                                    }
                                }
                            }
                        }
                    }
                }

                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                if (NPC.type == NPCID.EaterofWorldsHead)
                {
                    if (inTiles)
                    {
                        if (NPC.localAI[0] != 1f)
                            NPC.netUpdate = true;

                        NPC.localAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.localAI[0] != 0f)
                            NPC.netUpdate = true;

                        NPC.localAI[0] = 0f;
                    }
                    if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                        NPC.netUpdate = true;
                }
            }

            // 10 seconds of resistance to prevent spawn killing
            if (calamityGlobalNPC.newAI[1] < DRIncreaseTime && ((NPC.position - NPC.oldPosition).Length() > 2f || calamityGlobalNPC.newAI[1] > 0f))
                calamityGlobalNPC.newAI[1] += 1f;

            if (NPC.type == NPCID.EaterofWorldsHead || (NPC.type != NPCID.EaterofWorldsHead && Main.npc[(int)NPC.ai[1]].alpha >= 85))
            {
                if (NPC.alpha > 0 && NPC.life > 0)
                {
                    for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                    }
                }

                if ((NPC.position - NPC.oldPosition).Length() > 2f)
                {
                    NPC.alpha -= 42;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;
                }
            }
            else if (NPC.type > NPCID.EaterofWorldsHead && NPC.alpha > 0)
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            // Manually sync newAI because there is no GlobalNPC.SendExtraAI
            if (NPC.active && NPC.netUpdate && Main.dedServ)
            {
                SyncCalamityNPCAIArrayPacket.Send(NPC);
            }

            return false;
        }

        public static int GetEaterOfWorldsSegmentsCountRevDeath()
        {
            return Main.getGoodWorld ? 100 : (CalamityWorld.death || BossRushEvent.BossRushActive) ? 57 : 62;
        }
    }
}
