using System;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class QueenBeeAI : VanillaAIOverride
    {
        // Vanilla values
        public static int StingerDamage = 11; // 44; Also applies to GFB stinger replacements

        public override bool AI(Mod mod)
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool enrage = !BossRushEvent.BossRushActive;
            int targetTileX = (int)Main.player[NPC.target].Center.X / 16;
            int targetTileY = (int)Main.player[NPC.target].Center.Y / 16;

            Tile tile = Framing.GetTileSafely(targetTileX, targetTileY);
            if (tile.WallType == WallID.HiveUnsafe)
                enrage = false;

            float maxEnrageScale = 2f;
            float enrageScale = death ? 0.5f : 0f;
            if ((NPC.position.Y / 16f) < Main.worldSurface && enrage)
            {
                calamityGlobalNPC.CurrentlyEnraged = true;
                enrageScale += 1f;
            }
            if (!Main.player[NPC.target].ZoneJungle && enrage)
            {
                calamityGlobalNPC.CurrentlyEnraged = true;
                enrageScale += 1f;
            }

            if (Main.getGoodWorld)
                enrageScale += 0.5f;

            if (enrageScale > maxEnrageScale)
                enrageScale = maxEnrageScale;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Bee spawn limit
            int beeLimit = death ? 9 : 15;

            // Queen Bee Bee count
            int totalBees = 0;
            bool beeLimitReached = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC bee = Main.npc[i];
                bool isQueenBeeBee = bee.ai[3] == 1f;
                if (bee.active && (bee.type == NPCID.Bee || bee.type == NPCID.BeeSmall) && isQueenBeeBee)
                {
                    totalBees++;
                    if (totalBees >= beeLimit)
                    {
                        beeLimitReached = true;
                        break;
                    }
                }
            }

            // Hornet spawn limit
            int hornetLimit = 2;
            bool hornetLimitReached = false;

            // Only run this when necessary
            if (death)
            {
                // Queen Bee Hornet count
                int totalHornets = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC hornet = Main.npc[i];
                    bool isQueenBeeHornet = hornet.ai[3] == 1f;
                    if (hornet.active && (hornet.type == NPCID.LittleHornetHoney || hornet.type == NPCID.HornetHoney || hornet.type == NPCID.BigHornetHoney) && isQueenBeeHornet)
                    {
                        int hornetCountIncrement = hornet.type == NPCID.BigHornetHoney ? 3 : hornet.type == NPCID.HornetHoney ? 2 : 1;
                        totalHornets += hornetCountIncrement;
                        if (totalHornets >= hornetLimit)
                        {
                            hornetLimitReached = true;
                            break;
                        }
                    }
                }
            }
            else
                hornetLimitReached = true;

            // Phases

            // Become more aggressive and start firing double stingers (triple in death mode) phase
            bool phase2 = lifeRatio < 0.85f;

            // Stingers have slightly higher velocity
            bool phase3 = lifeRatio < 0.7f;

            // Stop spawning bees from ass and start performing stinger arcs + diagonal dashes, spawn bees while charging and become more aggressive phase
            bool phase4 = lifeRatio < 0.5f;

            // Perform many shorter-range dashes and use stinger arc in two possible directions phase
            bool phase5 = lifeRatio < 0.3f;

            // Triple stinger (quintuple in death mode) bombardment phase
            bool phase6 = lifeRatio < 0.1f;

            // Despawn
            float distanceFromTarget = Vector2.Distance(NPC.Center, Main.player[NPC.target].Center);
            if (NPC.ai[0] != 7f)
            {
                if (NPC.timeLeft < 60)
                    NPC.timeLeft = 60;
                if (distanceFromTarget > 3000f)
                    NPC.ai[0] = 4f;
            }
            if (Main.player[NPC.target].dead)
                NPC.ai[0] = 7f;

            // Always start in enemy spawning phase
            if (calamityGlobalNPC.newAI[3] == 0f)
            {
                calamityGlobalNPC.newAI[3] = 1f;
                NPC.ai[0] = 2f;
                NPC.netUpdate = true;
                NPC.SyncExtraAI();
            }

            // Despawn phase
            if (NPC.ai[0] == 7f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.velocity.Y *= 0.98f;

                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;

                if (NPC.position.X < (Main.maxTilesX * 8))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.98f;
                    else
                        NPC.localAI[0] = 1f;

                    NPC.velocity.X -= 0.08f;
                }
                else
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.98f;
                    else
                        NPC.localAI[0] = 1f;

                    NPC.velocity.X += 0.08f;
                }

                if (NPC.timeLeft > 10)
                    NPC.timeLeft = 10;
            }

            // Pick a random phase
            else if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int phase;
                    int maxRandom = phase4 ? (death ? 5 : 4) : 4;
                    do phase = Main.rand.Next(maxRandom);
                    while (phase == NPC.ai[1] || phase == 1 || (phase == 2 && phase4) || (death && phase6 && phase == 3));

                    bool charging = phase == 0;
                    bool stingerArcs = phase == 4;

                    // 5 is stinger arc and charge
                    if (stingerArcs)
                        phase = 5;


                    CalamityUtils.CalamityTargeting(NPC, CalamityTargetingParameters.BossDefaults);
                    NPC.ai[0] = phase;
                    NPC.ai[1] = 0f;

                    // Movement direction for the stinger arcs
                    NPC.ai[2] = (phase == 5 && phase5) ? (Main.rand.NextBool() ? 1f : -1f) : phase == 5 ? 1f : 0f;

                    // Velocity for the charges
                    if (death)
                        NPC.ai[3] = charging ? ((phase6 ? 27f : phase5 ? 16f : phase4 ? 27f : phase2 ? 22f : 17f) + 3f * enrageScale) : 0f;
                    else
                        NPC.ai[3] = charging ? ((phase6 ? 25f : phase5 ? 14f : phase4 ? 25f : phase2 ? 20f : 15f) + 3f * enrageScale) : 0f;

                    // Distance for the charges
                    if (death)
                        calamityGlobalNPC.newAI[1] = charging ? ((phase6 ? 700f : phase5 ? 300f : phase4 ? 600f : phase2 ? 500f : 400f) - 50f * enrageScale) : 0f;
                    else
                        calamityGlobalNPC.newAI[1] = charging ? ((phase6 ? 750f : phase5 ? 350f : phase4 ? 650f : phase2 ? 550f : 450f) - 50f * enrageScale) : 0f;

                    NPC.SyncExtraAI();
                }
            }

            // Charging phase
            else if (NPC.ai[0] == 0f)
            {
                // Charging distance from player
                int chargeDistanceX = (int)calamityGlobalNPC.newAI[1];

                // Number of charges
                int chargeAmt = (int)Math.Ceiling((phase6 ? 2f : phase5 ? 4f : phase4 ? 3f : 2f) + enrageScale);
                if (death)
                    chargeAmt = phase6 ? 1 : phase5 ? 3 : phase4 ? 2 : 1;

                int deathChargeLimit = phase5 ? 3 : 2;
                if (death && chargeAmt > deathChargeLimit)
                    chargeAmt = deathChargeLimit;

                // Switch to a random phase if chargeAmt has been exceeded
                if (NPC.ai[1] > (2 * chargeAmt) && NPC.ai[1] % 2f == 0f)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                    return false;
                }

                // Charge velocity
                float velocity = NPC.ai[3];

                // Line up and initiate charge
                if (NPC.ai[1] % 2f == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    // Initiate charge
                    float chargeDistanceY = phase6 ? 100f : phase4 ? 50f : 20f;
                    chargeDistanceY += 50f * enrageScale;
                    if (death)
                    {
                        chargeDistanceY += MathHelper.Lerp(0f, 100f, 1f - (lifeRatio / 2));
                        chargeDistanceY *= 2f;
                    }

                    float distanceFromTargetX = Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X);
                    float distanceFromTargetY = Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y);
                    if (distanceFromTargetY < chargeDistanceY && distanceFromTargetX >= chargeDistanceX)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        // Set AI variables and speed
                        NPC.localAI[0] = 1f;
                        NPC.ai[1] += 1f;
                        NPC.ai[2] = 0f;

                        // Get target location
                        Vector2 beeLocation = NPC.Center;
                        float targetXDist = Main.player[NPC.target].Center.X - beeLocation.X;
                        float targetYDist = Main.player[NPC.target].Center.Y - beeLocation.Y;
                        float targetDistance = (float)Math.Sqrt(targetXDist * targetXDist + targetYDist * targetYDist);
                        targetDistance = velocity / targetDistance;
                        NPC.velocity.X = targetXDist * targetDistance;
                        NPC.velocity.Y = targetYDist * targetDistance;

                        // Face the correct direction and play charge sound
                        float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.spriteDirection = NPC.direction;

                        SoundEngine.PlaySound(SoundID.Zombie125, NPC.Center);

                        return false;
                    }

                    // Velocity variables
                    NPC.localAI[0] = 0f;
                    float chargeVelocityX = (phase4 ? 24f : phase2 ? 20f : 16f) + 8f * enrageScale;
                    float chargeVelocityY = (phase4 ? 18f : phase2 ? 15f : 12f) + 6f * enrageScale;
                    float chargeAccelerationX = (phase4 ? 0.7f : phase2 ? 0.6f : 0.5f) + 0.25f * enrageScale;
                    float chargeAccelerationY = (phase4 ? 0.35f : phase2 ? 0.3f : 0.25f) + 0.125f * enrageScale;

                    if (death)
                    {
                        chargeVelocityX += 1f;
                        chargeVelocityY += 2f;
                        chargeAccelerationX += 0.1f;
                        chargeAccelerationY += 0.2f;
                    }

                    // Velocity calculations
                    if (NPC.Center.Y < Main.player[NPC.target].Center.Y - chargeDistanceY)
                        NPC.velocity.Y += chargeAccelerationY;
                    else if (NPC.Center.Y > Main.player[NPC.target].Center.Y + chargeDistanceY)
                        NPC.velocity.Y -= chargeAccelerationY;
                    else
                        NPC.velocity.Y *= 0.7f;

                    if (NPC.velocity.Y < -chargeVelocityY)
                        NPC.velocity.Y = -chargeVelocityY;
                    if (NPC.velocity.Y > chargeVelocityY)
                        NPC.velocity.Y = chargeVelocityY;

                    float distanceXMax = 100f;
                    float distanceXMin = 20f;
                    if (distanceFromTargetX > chargeDistanceX + distanceXMax)
                        NPC.velocity.X += chargeAccelerationX * NPC.direction;
                    else if (distanceFromTargetX < chargeDistanceX + distanceXMin)
                        NPC.velocity.X -= chargeAccelerationX * NPC.direction;
                    else
                        NPC.velocity.X *= 0.7f;

                    // Limit velocity
                    if (NPC.velocity.X < -chargeVelocityX)
                        NPC.velocity.X = -chargeVelocityX;
                    if (NPC.velocity.X > chargeVelocityX)
                        NPC.velocity.X = chargeVelocityX;

                    // Face the correct direction
                    float playerLocation2 = NPC.Center.X - Main.player[NPC.target].Center.X;
                    NPC.direction = playerLocation2 < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                    NPC.ForceNetUpdate(false);
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    // Face the correct direction
                    if (NPC.velocity.X < 0f)
                        NPC.direction = -1;
                    else
                        NPC.direction = 1;

                    NPC.spriteDirection = NPC.direction;

                    // Get which side of the player the boss is on
                    int chargeDirection = 1;
                    if (NPC.Center.X < Main.player[NPC.target].Center.X)
                        chargeDirection = -1;

                    // If boss is in correct position, slow down, if not, reset
                    bool shouldCharge = false;
                    if (NPC.direction == chargeDirection && Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) > chargeDistanceX)
                    {
                        NPC.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > chargeDistanceX * 1.5f)
                    {
                        NPC.ai[2] = 1f;
                        shouldCharge = true;
                    }
                    if (enrageScale > 0f && shouldCharge)
                        NPC.velocity *= MathHelper.Lerp(0.5f, death ? 0.9f : 1f, 1f - enrageScale / maxEnrageScale);

                    // Keep moving
                    if (NPC.ai[2] != 1f)
                    {
                        // Velocity fix if Queen Bee is slowed
                        if (NPC.velocity.Length() < velocity)
                            NPC.velocity.X = velocity * NPC.direction;

                        float accelerateGateValue = phase6 ? 30f : phase5 ? 10f : 90f;
                        if (enrageScale > 0f)
                            accelerateGateValue *= 0.75f;

                        calamityGlobalNPC.newAI[0] += 1f;
                        if (calamityGlobalNPC.newAI[0] > accelerateGateValue)
                        {
                            NPC.SyncExtraAI();
                            float velocityXLimit = velocity * 2f;
                            if (Math.Abs(NPC.velocity.X) < velocityXLimit)
                                NPC.velocity.X *= (death ? 1.02f : 1.01f);
                        }

                        // Spawn bees
                        float beeSpawnGateValue = 20f;
                        bool spawnBee = phase4 && calamityGlobalNPC.newAI[0] % beeSpawnGateValue == 0f && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                        if (spawnBee)
                        {
                            SoundEngine.PlaySound(SoundID.NPCHit18, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int spawnType = Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1);
                                if (Main.zenithWorld)
                                {
                                    if (phase3)
                                        spawnType = Main.rand.NextBool(3) ? ModContent.NPCType<PlagueChargerLarge>() : ModContent.NPCType<PlagueCharger>();
                                    else
                                        spawnType = NPCID.Hellbat;
                                }
                                else if (death)
                                {
                                    int random = hornetLimitReached ? 0 : beeLimitReached ? Main.rand.Next(6, 12) : Main.rand.Next(12);
                                    switch (random)
                                    {
                                        default:
                                        case 0:
                                        case 1:
                                        case 2:
                                        case 3:
                                        case 4:
                                        case 5:
                                            break;

                                        case 6:
                                        case 7:
                                        case 8:
                                            spawnType = NPCID.LittleHornetHoney;
                                            break;

                                        case 9:
                                        case 10:
                                            spawnType = NPCID.HornetHoney;
                                            break;

                                        case 11:
                                            spawnType = NPCID.BigHornetHoney;
                                            break;
                                    }
                                }

                                if (!beeLimitReached || !hornetLimitReached)
                                {
                                    int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, spawnType);
                                    Vector2 beeVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    Main.npc[spawn].velocity = beeVelocity;
                                    Main.npc[spawn].velocity *= 5f;
                                    if (!Main.zenithWorld)
                                    {
                                        Main.npc[spawn].ai[2] = enrageScale;
                                        Main.npc[spawn].ai[3] = 1f;
                                    }
                                    Main.npc[spawn].timeLeft = 600;
                                    Main.npc[spawn].netUpdate = true;
                                }
                            }
                        }

                        NPC.localAI[0] = 1f;
                        return false;
                    }

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;

                    // Slow down
                    NPC.localAI[0] = 0f;
                    NPC.velocity *= (death ? 0.8f : 0.9f);

                    float chargeDeceleration = death ? 0.2f : 0.1f;
                    if (phase2)
                    {
                        NPC.velocity *= 0.9f;
                        chargeDeceleration += 0.05f;
                    }
                    if (phase4)
                    {
                        NPC.velocity *= 0.8f;
                        chargeDeceleration += 0.1f;
                    }
                    if (enrageScale > 0f)
                        NPC.velocity *= MathHelper.Lerp(0.7f, 1f, 1f - enrageScale / maxEnrageScale);

                    if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < chargeDeceleration)
                    {
                        NPC.ai[2] = 0f;
                        NPC.ai[1] += 1f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        NPC.SyncExtraAI();
                    }

                    NPC.ForceNetUpdate(false);
                }
            }

            // Fly above target before bee spawning phase
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Direction
                float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                // Get target location
                float beeAttackAccel = death ? 0.48f : 0.24f;
                float beeAttackSpeed = 12f + enrageScale * 3f;

                if (death)
                    beeAttackSpeed *= 1.35f;

                bool canHitTarget = Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                float distanceAboveTarget = !canHitTarget ? 0f : 320f;
                Vector2 hoverDestination = Main.player[NPC.target].Center - Vector2.UnitY * distanceAboveTarget;
                Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * beeAttackSpeed;

                // Go to bee spawn phase
                calamityGlobalNPC.newAI[0] += 1f;
                if ((Vector2.Distance(NPC.Center, hoverDestination) < 400f && canHitTarget) || calamityGlobalNPC.newAI[0] >= (death ? 90f : 180f))
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    NPC.netUpdate = true;
                    NPC.SyncExtraAI();
                    return false;
                }

                NPC.SimpleFlyMovement(idealVelocity, beeAttackAccel);
            }

            // Bee spawn phase
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.localAI[0] = 0f;

                // Get target location and spawn bees from ass
                float beeAttackHoverSpeed = 16f + enrageScale * 4f;
                float beeAttackHoverAccel = death ? 0.6f : 0.3f;

                if (death)
                    beeAttackHoverSpeed *= 1.35f;

                Vector2 beeSpawnLocation = new Vector2(NPC.Center.X + (Main.rand.Next(20) * NPC.direction), NPC.position.Y + NPC.height * 0.8f);
                Vector2 beeSpawnCollisionLocation = new Vector2(beeSpawnLocation.X, beeSpawnLocation.Y - 30f);
                bool canHitTarget = Collision.CanHit(beeSpawnCollisionLocation, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                Vector2 hoverDestination = Main.player[NPC.target].Center - Vector2.UnitY * (!canHitTarget ? 0f : 320f);
                Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * beeAttackHoverSpeed;

                // Bee spawn timer
                NPC.ai[1] += 1f;
                int beeSpawnTimer = 0;
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && !Main.player[i].dead && (NPC.Center - Main.player[i].Center).Length() < 1000f)
                        beeSpawnTimer++;
                }
                NPC.ai[1] += beeSpawnTimer / 2;
                if (phase2)
                    NPC.ai[1] += 1f;

                bool spawnBee = false;
                float beeSpawnCheck = 9 * enrageScale;
                if (NPC.ai[1] > beeSpawnCheck)
                {
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    spawnBee = true;
                }

                // Spawn bees
                if (Collision.CanHit(beeSpawnLocation, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && spawnBee && (!beeLimitReached || !hornetLimitReached))
                {
                    SoundEngine.PlaySound(SoundID.NPCHit18, beeSpawnLocation);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {

                        int spawnType = Main.rand.Next(NPCID.Bee, NPCID.BeeSmall + 1);
                        if (Main.zenithWorld)
                        {
                            if (phase3)
                                spawnType = Main.rand.NextBool(3) ? ModContent.NPCType<PlagueChargerLarge>() : ModContent.NPCType<PlagueCharger>();
                            else
                                spawnType = NPCID.Hellbat;
                        }
                        else if (death)
                        {
                            int random = hornetLimitReached ? 0 : beeLimitReached ? Main.rand.Next(6, 12) : Main.rand.Next(12);
                            switch (random)
                            {
                                default:
                                case 0:
                                case 1:
                                case 2:
                                case 3:
                                case 4:
                                case 5:
                                    break;

                                case 6:
                                case 7:
                                case 8:
                                    spawnType = NPCID.LittleHornetHoney;
                                    break;

                                case 9:
                                case 10:
                                    spawnType = NPCID.HornetHoney;
                                    break;

                                case 11:
                                    spawnType = NPCID.BigHornetHoney;
                                    break;
                            }
                        }

                        int spawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)beeSpawnLocation.X, (int)beeSpawnLocation.Y, spawnType);
                        Vector2 beeVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        Main.npc[spawn].velocity = beeVelocity;
                        Main.npc[spawn].velocity *= 5f;
                        if (!Main.zenithWorld)
                        {
                            Main.npc[spawn].ai[2] = enrageScale;
                            Main.npc[spawn].ai[3] = 1f;
                        }
                        Main.npc[spawn].timeLeft = 600;
                        Main.npc[spawn].netUpdate = true;

                    }
                }

                // Velocity calculations if target is too far away
                if (Vector2.Distance(beeSpawnLocation, hoverDestination) > 400f || !canHitTarget)
                    NPC.SimpleFlyMovement(idealVelocity, beeAttackHoverAccel);
                else
                    NPC.velocity *= (death ? 0.8f : 0.85f);

                // Face the correct direction
                float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                // Go to a random phase
                float numSpawns = death ? 3f : 5f;
                if (NPC.ai[2] > numSpawns || (beeLimitReached && hornetLimitReached))
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 2f;
                    NPC.ai[2] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Stinger phase
            else if (NPC.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Direction
                float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                // Get target location and shoot from ass
                float stingerAttackSpeed = 16f + enrageScale * 4f;
                float stingerAttackAccel = phase6 ? 0.16f : 0.12f;
                if (enrageScale > 0f)
                    stingerAttackAccel = MathHelper.Lerp(phase6 ? 0.3f : 0.24f, phase6 ? 0.6f : 0.48f, enrageScale / maxEnrageScale);

                if (death)
                {
                    stingerAttackSpeed *= 1.08f;
                    stingerAttackAccel *= 1.18f;
                }

                Vector2 stingerSpawnLocation = new Vector2(NPC.Center.X + (Main.rand.Next(20) * NPC.direction), NPC.position.Y + NPC.height * 0.8f);
                bool canHitTarget = Collision.CanHit(new Vector2(stingerSpawnLocation.X, stingerSpawnLocation.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                Vector2 hoverDestination = Main.player[NPC.target].Center - Vector2.UnitY * (!canHitTarget ? 0f : phase4 ? 400f : phase2 ? 360f : 320f);
                Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * stingerAttackSpeed;

                NPC.ai[1] += 1f;
                int stingerAttackTimer = phase6 ? 40 : phase2 ? 30 : 20;
                stingerAttackTimer -= (int)Math.Ceiling((phase6 ? 16f : phase2 ? 12f : 8f) * enrageScale);
                if (stingerAttackTimer < 5)
                    stingerAttackTimer = 5;

                // Fire stingers
                if (NPC.ai[1] % stingerAttackTimer == (stingerAttackTimer - 1) && NPC.Bottom.Y < Main.player[NPC.target].Top.Y && Collision.CanHit(stingerSpawnLocation, 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    SoundEngine.PlaySound(SoundID.Item17, stingerSpawnLocation);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float stingerSpeed = (phase3 ? 6f : 5f) + enrageScale;
                        if (death)
                            stingerSpeed += 1f;

                        float stingerTargetX = Main.player[NPC.target].Center.X - stingerSpawnLocation.X;
                        float stingerTargetY = Main.player[NPC.target].Center.Y - stingerSpawnLocation.Y;
                        float stingerTargetDist = (float)Math.Sqrt(stingerTargetX * stingerTargetX + stingerTargetY * stingerTargetY);
                        stingerTargetDist = stingerSpeed / stingerTargetDist;
                        stingerTargetX *= stingerTargetDist;
                        stingerTargetY *= stingerTargetDist;
                        Vector2 stingerVelocity = new Vector2(stingerTargetX, stingerTargetY);
                        int type = Main.zenithWorld ? (phase3 ? ModContent.ProjectileType<PlagueStingerGoliathV2>() : ProjectileID.FlamingWood) : ProjectileID.QueenBeeStinger;

                        int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), stingerSpawnLocation, stingerVelocity, type, StingerDamage, 0f, Main.myPlayer, 0f, (Main.zenithWorld && phase3) ? Main.player[NPC.target].position.Y : 0f);
                        Main.projectile[projectile].timeLeft = 1200;
                        Main.projectile[projectile].extraUpdates = 1;

                        if (phase2)
                        {
                            int numExtraStingers = death ? (phase6 ? 4 : 2) : (phase6 ? 2 : 1);
                            for (int i = 0; i < numExtraStingers; i++)
                            {
                                projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), stingerSpawnLocation + Main.rand.NextVector2CircularEdge(16f, 16f) * (i + 1), stingerVelocity * MathHelper.Lerp(0.75f, 1f, i / (float)numExtraStingers), type, StingerDamage, 0f, Main.myPlayer, 0f, (Main.zenithWorld && phase3) ? Main.player[NPC.target].position.Y : 0f);
                                Main.projectile[projectile].timeLeft = 1200;
                                Main.projectile[projectile].extraUpdates = 1;
                            }
                        }
                    }
                }

                // Movement calculations
                if (Vector2.Distance(stingerSpawnLocation, hoverDestination) > 40f || !canHitTarget)
                    NPC.SimpleFlyMovement(idealVelocity, stingerAttackAccel);

                // Go to a random phase
                float numStingerShots = phase6 ? 5f : phase2 ? 8f : 15f;
                if (death)
                    numStingerShots = (float)Math.Round(numStingerShots * 0.5f);

                if (NPC.ai[1] > stingerAttackTimer * numStingerShots)
                {
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.netUpdate = true;
                }
            }

            else if (NPC.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.localAI[0] = 1f;
                float despawnVelMult = 14f;

                Vector2 despawnTargetDist = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                despawnTargetDist *= 14f;

                NPC.velocity = (NPC.velocity * despawnVelMult + despawnTargetDist) / (despawnVelMult + 1f);
                if (NPC.velocity.X < 0f)
                    NPC.direction = -1;
                else
                    NPC.direction = 1;

                NPC.spriteDirection = NPC.direction;

                if (distanceFromTarget < 2000f)
                {
                    NPC.ai[0] = -1f;
                    NPC.localAI[0] = 0f;
                }
            }

            // Stinger arcs above the player
            else if (NPC.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Direction
                float playerLocation = NPC.Center.X - Main.player[NPC.target].Center.X;
                NPC.direction = playerLocation < 0 ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                // Get target location and shoot spreads from ass
                float stingerAttackSpeed = 20f + enrageScale * 4f;
                float stingerAttackAccel = phase6 ? 0.7f : 0.5f;
                if (enrageScale > 0f)
                    stingerAttackAccel = MathHelper.Lerp(phase6 ? 0.9f : 0.7f, phase6 ? 2.4f : 1.8f, enrageScale / maxEnrageScale);

                if (death)
                {
                    stingerAttackSpeed *= 1.1f;
                    stingerAttackAccel *= 1.2f;
                }

                int numStingerArcs = phase6 ? 3 : phase5 ? 2 : 1;
                if (death)
                    numStingerArcs++;

                float phaseLimit = phase6 ? 180f : phase5 ? 150f : 120f;
                if (death)
                    phaseLimit *= 1.5f;

                float stingerAttackTimer = (float)Math.Ceiling(phaseLimit / (numStingerArcs + 1));

                float maxDistance = 480f;
                float xLocationScale = MathHelper.Lerp(-maxDistance, maxDistance, NPC.ai[1] / phaseLimit) * NPC.ai[2];
                Vector2 stingerSpawnLocation = new Vector2(NPC.Center.X + (Main.rand.Next(20) * NPC.direction), NPC.position.Y + NPC.height * 0.8f);
                bool canHitTarget = Collision.CanHit(new Vector2(stingerSpawnLocation.X, stingerSpawnLocation.Y - 30f), 1, 1, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height);
                Vector2 hoverDestination = Main.player[NPC.target].Center + Vector2.UnitX * xLocationScale * (death ? 1.35f : 1.25f) - Vector2.UnitY * maxDistance;
                Vector2 idealVelocity = NPC.SafeDirectionTo(hoverDestination) * stingerAttackSpeed;

                // Fire stingers
                bool canFireStingers = stingerSpawnLocation.Y < Main.player[NPC.target].Top.Y - maxDistance * 0.8f || !canHitTarget;
                if (canFireStingers && NPC.ai[1] < phaseLimit)
                {
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] % stingerAttackTimer == 0f && NPC.ai[1] != 0f && NPC.ai[1] != phaseLimit)
                    {
                        SoundEngine.PlaySound(SoundID.Item17, stingerSpawnLocation);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float stingerSpeed = (phase6 ? 5f : 4f) + enrageScale;
                            if (death)
                                stingerSpeed += 1f;

                            Vector2 projectileVelocity = (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * stingerSpeed;
                            int type = Main.zenithWorld ? ModContent.ProjectileType<PlagueStingerGoliathV2>() : ProjectileID.QueenBeeStinger;
                            int numProj = death ? (phase6 ? 7 : phase5 ? 11 : 15) : (phase6 ? 5 : phase5 ? 9 : 13);
                            int spread = phase6 ? 30 : phase5 ? 50 : 60;

                            if (death)
                            {
                                numProj += (phase6 ? 2 : phase5 ? 4 : 6);
                                spread += (phase6 ? 10 : phase5 ? 15 : 20);
                            }

                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                if (i % 2f != 0f)
                                    perturbedSpeed *= 0.8f;

                                int projectile = Projectile.NewProjectile(NPC.GetSource_FromAI(), stingerSpawnLocation + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 10f, perturbedSpeed, type, StingerDamage, 0f, Main.myPlayer, 0f, Main.player[NPC.target].position.Y);
                                Main.projectile[projectile].timeLeft = 1200;
                                Main.projectile[projectile].extraUpdates = 1;

                                if (!Main.zenithWorld)
                                    Main.projectile[projectile].tileCollide = false;
                            }
                        }
                    }
                }

                // Go to a random phase after pausing for a bit
                if (NPC.ai[1] >= phaseLimit)
                {
                    NPC.ai[1] += 1f;

                    if (NPC.Distance(Main.player[NPC.target].Center) > 400f || !canHitTarget)
                    {
                        idealVelocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center) * stingerAttackSpeed;
                        NPC.SimpleFlyMovement(idealVelocity * 0.5f, stingerAttackAccel * 0.5f);
                    }
                    else
                        NPC.velocity *= 0.8f;

                    float idleTime = death ? 140f : 180f;
                    if (NPC.ai[1] >= phaseLimit + idleTime)
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = 4f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                    }
                }
                else
                    NPC.SimpleFlyMovement(idealVelocity, stingerAttackAccel);
            }

            if (Main.dedServ)
                NPC.ForceNetUpdate();

            return false;
        }
    }
}
