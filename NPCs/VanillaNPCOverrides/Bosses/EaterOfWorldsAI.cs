using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class EaterOfWorldsAI
    {
        public static bool BuffedEaterofWorldsAI(NPC npc, Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = npc.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;

            // Causes it to split far more in death mode
            if ((((npc.ai[2] % 2f == 0f && npc.type == NPCID.EaterofWorldsBody) || npc.type == NPCID.EaterofWorldsHead) && death) || CalamityWorld.LegendaryMode)
            {
                calamityGlobalNPC.DR = 0.5f;
                npc.defense = npc.defDefense * 2;
            }

            if (CalamityWorld.LegendaryMode && npc.type == NPCID.EaterofWorldsHead)
                npc.reflectsProjectiles = true;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles && npc.type == NPCID.EaterofWorldsHead)
                npc.TargetClosest();

            // Fade in.
            npc.Opacity = MathHelper.Clamp(npc.Opacity + 0.08f, 0f, 1f);

            float enrageScale = bossRush ? 1f : 0f;
            if ((npc.position.Y / 16f) < Main.worldSurface || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (!Main.player[npc.target].ZoneCorrupt || bossRush)
            {
                npc.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Total body segments
            float totalSegments = GetEaterOfWorldsSegmentsCountRevDeath();

            // Count segments remaining
            // TODO - This runs for every EoW segment, this is bad because there are three separate loops here running every frame.
            float segmentCount = NPC.CountNPCS(NPCID.EaterofWorldsHead) + NPC.CountNPCS(NPCID.EaterofWorldsBody) + NPC.CountNPCS(NPCID.EaterofWorldsTail);

            // Percent segments remaining, add two to total for head and tail
            float lifeRatio = segmentCount / (totalSegments + 2);

            // 10 seconds of resistance to prevent spawn killing
            if (calamityGlobalNPC.newAI[1] < 600f && bossRush)
                calamityGlobalNPC.newAI[1] += 1f;

            // Phases
            bool phase2 = lifeRatio < 0.9f;
            bool phase3 = lifeRatio < 0.75f;
            bool phase4 = lifeRatio < 0.4f;
            bool phase5 = lifeRatio < 0.1f;

            // Fire projectiles
            if (Main.netMode != NetmodeID.MultiplayerClient && (!phase5 || death || Main.getGoodWorld))
            {
                // Vile spit
                if (npc.type == NPCID.EaterofWorldsBody)
                {
                    if (Main.rand.NextBool(Main.getGoodWorld ? 450 : 900) && phase2)
                    {
                        npc.TargetClosest();
                        if (Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (npc.height / 2) + npc.velocity.Y), NPCID.VileSpitEaterOfWorlds, 0, 0f, 1f, 0f, 0f, 255);
                    }
                }

                // Cursed flames
                else if (npc.type == NPCID.EaterofWorldsHead)
                {
                    calamityGlobalNPC.newAI[0] += 1f;
                    float timer = enrageScale > 0f ? 90f : 120f;
                    float shootBoost = death ? lifeRatio * 90f : lifeRatio * 180f;
                    timer += shootBoost;

                    if (enrageScale >= 2f)
                        timer = 60f;

                    if (calamityGlobalNPC.newAI[0] >= timer && phase3)
                    {
                        calamityGlobalNPC.newAI[0] = 0f;
                        if (Collision.CanHitLine(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                        {
                            Vector2 vector34 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            float num349 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2) - vector34.X;
                            float num350 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2) - vector34.Y;
                            float num418 = 7f + enrageScale * 2f;
                            float num351 = (float)Math.Sqrt(num349 * num349 + num350 * num350);
                            num351 = num418 / num351;
                            num349 *= num351;
                            num350 *= num351;
                            num350 += npc.velocity.Y * 0.5f;
                            num349 += npc.velocity.X * 0.5f;
                            vector34.X -= num349;
                            vector34.Y -= num350;

                            int type = ProjectileID.CursedFlameHostile;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector34.X, vector34.Y, num349, num350, type, npc.GetProjectileDamage(type), 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }

            // Despawn
            if (Main.player[npc.target].dead)
            {
                if (npc.timeLeft > 300)
                    npc.timeLeft = 300;
            }

            // All functions that modify the active worm segments are here. This includes spawning the worm originally and splitting effects.
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // If this segment is a head or a body without a next-segment defined, then it needs to spawn its own next segment.
                if ((npc.type == NPCID.EaterofWorldsHead || npc.type == NPCID.EaterofWorldsBody) && npc.ai[0] == 0f)
                {
                    int spawnX = (int)npc.position.X;
                    int spawnY = (int)npc.position.Y;

                    // A head sets the length variable (npc.ai[2]) and then sets its next segment to a freshly spawned body.
                    if (npc.type == NPCID.EaterofWorldsHead)
                    {
                        // Set head's "length beyond this point" to be the total length of the worm.
                        npc.ai[2] = totalSegments;

                        // Body spawn
                        npc.ai[0] = NPC.NewNPC(npc.GetSource_FromAI(), spawnX, spawnY, NPCID.EaterofWorldsBody, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }

                    // A body with a "length beyond this point" greater than zero just sets its next spawned segment to a freshly spawned body.
                    else if (npc.type == NPCID.EaterofWorldsBody && npc.ai[2] > 0f)
                        npc.ai[0] = NPC.NewNPC(npc.GetSource_FromAI(), spawnX, spawnY, NPCID.EaterofWorldsBody, npc.whoAmI, 0f, 0f, 0f, 0f, 255);

                    // If the worm stops here ("length beyond this point" is zero), then spawn a tail instead.
                    else
                        npc.ai[0] = NPC.NewNPC(npc.GetSource_FromAI(), spawnX, spawnY, NPCID.EaterofWorldsTail, npc.whoAmI, 0f, 0f, 0f, 0f, 255);

                    // Maintain the linked list of worm segments, and correctly set the "length beyond this point" of this segment.
                    Main.npc[(int)npc.ai[0]].ai[1] = npc.whoAmI;
                    Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
                    npc.netUpdate = true;
                }

                // Helper function to destroy this Eater of Worlds worm segment.
                void DestroyThisSegment()
                {
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.checkDead();
                }

                // If this segment's previous and next segments are both dead, make it explode instantly. Single segments cannot live.
                if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active)
                    DestroyThisSegment();

                // If this segment is a head and its next segment is dead, make it explode instantly. It's been decapitated.
                if (npc.type == NPCID.EaterofWorldsHead && !Main.npc[(int)npc.ai[0]].active)
                    DestroyThisSegment();

                // If this segment is a tail and its previous segment is dead, make it explode instantly. It's been chopped off.
                if (npc.type == NPCID.EaterofWorldsTail && !Main.npc[(int)npc.ai[1]].active)
                    DestroyThisSegment();

                // If this segment is a body and its previous segment is dead (or was rendered into a tail), transform into a head.
                if (npc.type == NPCID.EaterofWorldsBody && (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle))
                {
                    npc.type = NPCID.EaterofWorldsHead;
                    float segmentLifeRatio = npc.life / (float)npc.lifeMax;
                    int whoAmI = npc.whoAmI;
                    float ai0Holdover = npc.ai[0];
                    float newAI1Holdover = calamityGlobalNPC.newAI[1];
                    int slowingDebuffResistTimer = calamityGlobalNPC.debuffResistanceTimer;

                    // Actually transform the body segment into a head segment.
                    npc.SetDefaultsKeepPlayerInteraction(npc.type);
                    npc.life = (int)(npc.lifeMax * segmentLifeRatio);
                    npc.whoAmI = whoAmI;
                    npc.ai[0] = ai0Holdover;
                    // Heads spawned mid fight by splitting do not get reset spawn invincibility.
                    CalamityGlobalNPC newCGN = npc.Calamity();
                    newCGN.newAI[1] = newAI1Holdover;
                    newCGN.debuffResistanceTimer = slowingDebuffResistTimer;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                    npc.netSpam = 0;
                }

                // If this segment is a body and its next segment is dead (or was rendered into a head), transform into a tail.
                if (npc.type == NPCID.EaterofWorldsBody && (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle))
                {
                    npc.type = NPCID.EaterofWorldsTail;
                    float segmentLifeRatio = npc.life / (float)npc.lifeMax;
                    int whoAmI = npc.whoAmI;
                    float ai1Holdover = npc.ai[1];
                    int slowingDebuffResistTimer = calamityGlobalNPC.debuffResistanceTimer;

                    // Actually transform the body segment into a tail segment.
                    npc.SetDefaultsKeepPlayerInteraction(npc.type);
                    npc.life = (int)(npc.lifeMax * segmentLifeRatio);
                    npc.whoAmI = whoAmI;
                    npc.ai[1] = ai1Holdover;
                    npc.Calamity().debuffResistanceTimer = slowingDebuffResistTimer;
                    npc.TargetClosest();
                    npc.netUpdate = true;
                    npc.netSpam = 0;
                }

                // If for any reason this segment was deleted, send info to clients so they also see it die.
                if (!npc.active && Main.netMode == NetmodeID.Server)
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f);
            }

            // Movement
            int num29 = (int)(npc.position.X / 16f) - 1;
            int num30 = (int)((npc.position.X + npc.width) / 16f) + 2;
            int num31 = (int)(npc.position.Y / 16f) - 1;
            int num32 = (int)((npc.position.Y + npc.height) / 16f) + 2;
            if (num29 < 0)
                num29 = 0;
            if (num30 > Main.maxTilesX)
                num30 = Main.maxTilesX;
            if (num31 < 0)
                num31 = 0;
            if (num32 > Main.maxTilesY)
                num32 = Main.maxTilesY;

            // Fly or not
            bool inTiles = false;
            if (!inTiles)
            {
                for (int num33 = num29; num33 < num30; num33++)
                {
                    for (int num34 = num31; num34 < num32; num34++)
                    {
                        if (Main.tile[num33, num34] != null && ((Main.tile[num33, num34].HasUnactuatedTile && (Main.tileSolid[Main.tile[num33, num34].TileType] || (Main.tileSolidTop[Main.tile[num33, num34].TileType] && Main.tile[num33, num34].TileFrameY == 0))) || Main.tile[num33, num34].LiquidAmount > 64))
                        {
                            Vector2 vector;
                            vector.X = num33 * 16;
                            vector.Y = num34 * 16;
                            if (npc.position.X + npc.width > vector.X && npc.position.X < vector.X + 16f && npc.position.Y + npc.height > vector.Y && npc.position.Y < vector.Y + 16f)
                            {
                                inTiles = true;
                                if (Main.rand.NextBool(100) && Main.tile[num33, num34].HasUnactuatedTile)
                                {
                                    WorldGen.KillTile(num33, num34, true, true, false);
                                }
                            }
                        }
                    }
                }
            }
            if (!inTiles && npc.type == NPCID.EaterofWorldsHead)
            {
                Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int num35 = death ? 700 : 850;
                num35 -= (int)(enrageScale * 300f);

                if (num35 < 100)
                    num35 = 100;

                bool freeMoveAnyway = true;
                for (int num36 = 0; num36 < Main.maxPlayers; num36++)
                {
                    if (Main.player[num36].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[num36].position.X - num35, (int)Main.player[num36].position.Y - num35, num35 * 2, num35 * 2);
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
            float velocityScale = (death ? 4.8f : 2.4f) * enrageScale;
            float velocityBoost = velocityScale * (1f - lifeRatio);
            float accelerationScale = (death ? 0.06f : 0.03f) * enrageScale;
            float accelerationBoost = accelerationScale * (1f - lifeRatio);
            float num37 = 12f + velocityBoost;
            float num38 = 0.15f + accelerationBoost;

            if (phase5)
            {
                num37 += 2.4f * enrageScale;
                num38 += 0.03f * enrageScale;
            }
            else if (phase4)
            {
                num37 += 1.2f * enrageScale;
                num38 += 0.015f * enrageScale;
            }

            if (Main.getGoodWorld)
            {
                num37 += 4f;
                num38 += 0.05f;
            }

            Vector2 vector2 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num39 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
            float num40 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);

            num39 = (int)(num39 / 16f) * 16;
            num40 = (int)(num40 / 16f) * 16;
            vector2.X = (int)(vector2.X / 16f) * 16;
            vector2.Y = (int)(vector2.Y / 16f) * 16;
            num39 -= vector2.X;
            num40 -= vector2.Y;
            float num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);

            // Does this worm segment have a "previous segment" defined?
            if (npc.ai[1] > 0f && npc.ai[1] < Main.npc.Length)
            {
                try
                {
                    vector2 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                    num39 = Main.npc[(int)npc.ai[1]].position.X + (Main.npc[(int)npc.ai[1]].width / 2) - vector2.X;
                    num40 = Main.npc[(int)npc.ai[1]].position.Y + (Main.npc[(int)npc.ai[1]].height / 2) - vector2.Y;
                }
                catch
                {
                }

                npc.rotation = (float)Math.Atan2(num40, num39) + MathHelper.PiOver2;
                num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
                int num53 = npc.width;
                num53 = (int)(num53 * npc.scale);

                if (Main.getGoodWorld)
                    num53 = 62;

                num52 = (num52 - num53) / num52;
                num39 *= num52;
                num40 *= num52;
                npc.velocity = Vector2.Zero;
                npc.position.X += num39;
                npc.position.Y += num40;
            }

            // Otherwise this is a head. (Why does this not just check for head NPC type?)
            else
            {
                // Prevent new heads from being slowed when they spawn
                if (calamityGlobalNPC.newAI[1] < 3f)
                {
                    calamityGlobalNPC.newAI[1] += 1f;

                    // Set velocity for when a new head spawns
                    npc.velocity = Vector2.Normalize(Main.player[npc.target].Center - npc.Center) * (num37 * (death ? 0.5f : 0.4f));
                }

                if (!inTiles)
                {
                    npc.velocity.Y += death ? 0.15f : 0.11f;
                    if (npc.velocity.Y > num37)
                        npc.velocity.Y = num37;

                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num37 * 0.4)
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X -= num38 * 1.1f;
                        else
                            npc.velocity.X += num38 * 1.1f;
                    }
                    else if (npc.velocity.Y == num37)
                    {
                        if (npc.velocity.X < num39)
                            npc.velocity.X += num38;
                        else if (npc.velocity.X > num39)
                            npc.velocity.X -= num38;
                    }
                    else if (npc.velocity.Y > (death ? 6f : 4f))
                    {
                        if (npc.velocity.X < 0f)
                            npc.velocity.X += num38 * 0.9f;
                        else
                            npc.velocity.X -= num38 * 0.9f;
                    }
                }
                else
                {
                    // Sound
                    if (npc.soundDelay == 0)
                    {
                        float num54 = num52 / 40f;
                        if (num54 < 10f)
                            num54 = 10f;
                        if (num54 > 20f)
                            num54 = 20f;

                        npc.soundDelay = (int)num54;
                        SoundEngine.PlaySound(SoundID.WormDig, npc.position);
                    }

                    num52 = (float)Math.Sqrt(num39 * num39 + num40 * num40);
                    float num55 = Math.Abs(num39);
                    float num56 = Math.Abs(num40);
                    float num57 = num37 / num52;
                    num39 *= num57;
                    num40 *= num57;

                    // Despawn
                    bool shouldDespawn = npc.type == NPCID.EaterofWorldsHead && Main.player[npc.target].dead;
                    if (shouldDespawn && !bossRush)
                    {
                        bool everyoneDead = true;
                        for (int num58 = 0; num58 < Main.maxPlayers; num58++)
                        {
                            if (Main.player[num58].active && !Main.player[num58].dead)
                            {
                                everyoneDead = false;
                                break;
                            }
                        }

                        if (everyoneDead)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && (npc.position.Y / 16f) > (Main.rockLayer + Main.maxTilesY) / 2.0)
                            {
                                npc.active = false;
                                int num59 = (int)npc.ai[0];

                                while (num59 > 0 && num59 < Main.maxNPCs && Main.npc[num59].active && Main.npc[num59].aiStyle == npc.aiStyle)
                                {
                                    int arg_2853_0 = (int)Main.npc[num59].ai[0];
                                    Main.npc[num59].active = false;
                                    npc.life = 0;

                                    if (Main.netMode == NetmodeID.Server)
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num59, 0f, 0f, 0f, 0, 0, 0);

                                    num59 = arg_2853_0;
                                }

                                if (Main.netMode == NetmodeID.Server)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                            num39 = 0f;
                            num40 = num37;
                        }
                    }

                    if ((npc.velocity.X > 0f && num39 > 0f) || (npc.velocity.X < 0f && num39 < 0f) || (npc.velocity.Y > 0f && num40 > 0f) || (npc.velocity.Y < 0f && num40 < 0f))
                    {
                        if (npc.velocity.X < num39)
                            npc.velocity.X += num38;
                        else if (npc.velocity.X > num39)
                            npc.velocity.X -= num38;
                        if (npc.velocity.Y < num40)
                            npc.velocity.Y += num38;
                        else if (npc.velocity.Y > num40)
                            npc.velocity.Y -= num38;

                        if (Math.Abs(num40) < num37 * 0.2 && ((npc.velocity.X > 0f && num39 < 0f) || (npc.velocity.X < 0f && num39 > 0f)))
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num38 * 2f;
                            else
                                npc.velocity.Y -= num38 * 2f;
                        }

                        if (Math.Abs(num39) < num37 * 0.2 && ((npc.velocity.Y > 0f && num40 < 0f) || (npc.velocity.Y < 0f && num40 > 0f)))
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num38 * 2f;
                            else
                                npc.velocity.X -= num38 * 2f;
                        }
                    }
                    else if (num55 > num56)
                    {
                        if (npc.velocity.X < num39)
                            npc.velocity.X += num38 * 1.1f;
                        else if (npc.velocity.X > num39)
                            npc.velocity.X -= num38 * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num37 * 0.5)
                        {
                            if (npc.velocity.Y > 0f)
                                npc.velocity.Y += num38;
                            else
                                npc.velocity.Y -= num38;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y < num40)
                            npc.velocity.Y += num38 * 1.1f;
                        else if (npc.velocity.Y > num40)
                            npc.velocity.Y -= num38 * 1.1f;

                        if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num37 * 0.5)
                        {
                            if (npc.velocity.X > 0f)
                                npc.velocity.X += num38;
                            else
                                npc.velocity.X -= num38;
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;

                if (npc.type == NPCID.EaterofWorldsHead)
                {
                    if (inTiles)
                    {
                        if (npc.localAI[0] != 1f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 1f;
                    }
                    else
                    {
                        if (npc.localAI[0] != 0f)
                            npc.netUpdate = true;

                        npc.localAI[0] = 0f;
                    }
                    if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
                        npc.netUpdate = true;
                }
            }

            // Manually sync newAI because there is no GlobalNPC.SendExtraAI
            if (npc.active && npc.netUpdate && Main.netMode == NetmodeID.Server)
            {
                ModPacket packet = mod.GetPacket();
                packet.Write((byte)CalamityModMessageType.SyncCalamityNPCAIArray);
                packet.Write((byte)npc.whoAmI);
                packet.Write(calamityGlobalNPC.newAI[0]);
                packet.Write(calamityGlobalNPC.newAI[1]);
                packet.Write(calamityGlobalNPC.newAI[2]);
                packet.Write(calamityGlobalNPC.newAI[3]);
                packet.Send(-1, -1);
            }
            return false;
        }

        public static int GetEaterOfWorldsSegmentsCountRevDeath()
        {
            return CalamityWorld.LegendaryMode ? 100 : (CalamityWorld.death || BossRushEvent.BossRushActive) ? 67 : 77;
        }
    }
}
