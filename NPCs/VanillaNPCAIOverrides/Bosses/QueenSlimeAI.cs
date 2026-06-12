using System;
using System.Collections.Generic;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.Bosses
{
    public class QueenSlimeAI : VanillaAIOverride
    {
        // Vanilla values
        public static int SmallGelDamage = Main.masterMode ? 20 : 17; // 68
        public static int SpikeDamage = Main.masterMode ? 20 : 17; // 68
        public static int LargeGelDamage = 30; // 120
        public static int SlamDamage = 40; // 160

        public override bool AI(Mod mod)
        {
            // Difficulty bools
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float slimeScale = 1f;
            bool teleported = false;
            bool phase2 = lifeRatio <= 0.5f;
            bool phase3 = lifeRatio <= (death ? 0.5f : 0.4f);
            bool phase4 = lifeRatio <= (death ? 0.5f : 0.2f);
            bool phase5 = lifeRatio <= 0.25f && death;

            // Spawn settings
            if (NPC.localAI[0] == 0f)
            {
                NPC.ai[1] = -20f;
                NPC.localAI[0] = NPC.lifeMax;

                CalamityUtils.CalamityTargeting(NPC, default);

                NPC.netUpdate = true;
            }

            // Emit light
            Lighting.AddLight(NPC.Center, 1f, 0.7f, 0.9f);

            // Despawn
            int despawnDistanceInTiles = 500;
            if (Main.player[NPC.target].dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
            {
                CalamityUtils.CalamityTargeting(NPC, default);

                if (Main.player[NPC.target].dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
                {
                    NPC.EncourageDespawn(10);
                    if (Main.player[NPC.target].Center.X < NPC.Center.X)
                        NPC.direction = 1;
                    else
                        NPC.direction = -1;
                }
            }

            // Slow down dramatically while teleporting
            if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
            {
                if (Math.Abs(NPC.velocity.X) > 0.1f)
                {
                    NPC.velocity.X *= 0.8f;
                    if (Math.Abs(NPC.velocity.X) <= 0.1f)
                        NPC.velocity.X = 0f;
                }
            }

            // Teleport
            float teleportGateValue = death ? 300f : 600f;
            if (!Main.player[NPC.target].dead && NPC.timeLeft > 10 && !phase2 && NPC.ai[3] >= teleportGateValue && NPC.ai[0] == 0f && NPC.velocity.Y == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.ai[0] = 2f;
                NPC.ai[1] = 0f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.netUpdate = true;

                    CalamityTargetingParameters options = CalamityTargetingParameters.Defaults;
                    options.faceTarget = false;
                    CalamityUtils.CalamityTargeting(NPC, options);

                    Vector2 vectorAimedAheadOfTarget = Main.player[NPC.target].Center + new Vector2((float)Math.Round(Main.player[NPC.target].velocity.X), 0f).SafeNormalize(Vector2.Zero) * 800f;
                    Point predictiveTeleportPoint = vectorAimedAheadOfTarget.ToTileCoordinates();
                    int randomTeleportOffset = 5;
                    int teleportTries = 0;
                    while (teleportTries < 100)
                    {
                        teleportTries++;
                        int teleportTileX = Main.rand.Next(predictiveTeleportPoint.X - randomTeleportOffset, predictiveTeleportPoint.X + randomTeleportOffset + 1);
                        int teleportTileY = Main.rand.Next(predictiveTeleportPoint.Y - randomTeleportOffset, predictiveTeleportPoint.Y);
                        if (Main.tile[teleportTileX, teleportTileY].HasUnactuatedTile)
                            continue;

                        bool canTeleportToTile = true;
                        if (canTeleportToTile && Main.tile[teleportTileX, teleportTileY].LiquidType == LiquidID.Lava)
                            canTeleportToTile = false;
                        if (canTeleportToTile && !Collision.CanHitLine(NPC.Center, 0, 0, vectorAimedAheadOfTarget, 0, 0))
                            canTeleportToTile = false;

                        if (canTeleportToTile)
                        {
                            NPC.localAI[1] = teleportTileX * 16 + 8;
                            NPC.localAI[2] = teleportTileY * 16 + 16;
                            NPC.ai[3] = 0f;
                            break;
                        }
                    }

                    // Default teleport if the above conditions aren't met in 100 iterations
                    if (teleportTries >= 100)
                    {
                        Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                        NPC.localAI[1] = bottom.X;
                        NPC.localAI[2] = bottom.Y;
                        NPC.ai[3] = 0f;
                    }
                }
            }

            // Get ready to teleport by increasing ai[3]
            if (!phase2)
            {
                if (NPC.ai[3] < teleportGateValue)
                {
                    if (!Collision.CanHitLine(NPC.Center, 0, 0, Main.player[NPC.target].Center, 0, 0) || Math.Abs(NPC.Top.Y - Main.player[NPC.target].Bottom.Y) > 320f)
                        NPC.ai[3] += death ? 3f : 2f;
                    else
                        NPC.ai[3] += 1f;
                }
            }
            else
            {
                float teleportNetUpdate = NPC.ai[3];
                NPC.ai[3] -= 1f;
                if (NPC.ai[3] < 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && teleportNetUpdate > 0f)
                        NPC.netUpdate = true;

                    NPC.ai[3] = 0f;
                }
            }

            // Reset variables if despawning
            if (NPC.timeLeft <= 10 && ((phase2 && NPC.ai[0] != 0f) || (!phase2 && NPC.ai[0] != 3f)))
            {
                if (phase2)
                    NPC.ai[0] = 0f;
                else
                    NPC.ai[0] = 3f;

                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.ai[3] = 0f;
                NPC.netUpdate = true;
            }

            NPC.noTileCollide = false;
            NPC.noGravity = false;

            // Frame data shit I guess?
            if (phase2)
            {
                NPC.localAI[3] += 1f;
                if (NPC.localAI[3] >= 24f)
                    NPC.localAI[3] = 0f;

                if ((NPC.ai[0] == 4f || NPC.ai[0] == 6f) && NPC.ai[2] == 1f)
                    NPC.localAI[3] = 6f;

                if (NPC.ai[0] == 5f && NPC.ai[2] != 1f)
                    NPC.localAI[3] = 7f;
            }

            // Phases
            switch ((int)NPC.ai[0])
            {
                // Phase switch phase
                case 0:

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (phase2)
                    {
                        QueenSlime_FlyMovement(NPC);
                    }
                    else
                    {
                        NPC.noTileCollide = false;
                        NPC.noGravity = false;
                        if (NPC.velocity.Y == 0f)
                        {
                            NPC.velocity.X *= 0.8f;
                            if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                                NPC.velocity.X = 0f;
                        }
                    }

                    if (NPC.timeLeft <= 10 || (!phase2 && NPC.velocity.Y != 0f))
                        break;

                    NPC.ai[1] += 1f;
                    int idleTime = death ? 30 : 40;
                    if (phase2)
                        idleTime = death ? 60 : 80;
                    if (phase4)
                        idleTime /= 2;

                    if (!(NPC.ai[1] > idleTime))
                        break;

                    NPC.ai[1] = 0f;
                    if (phase2)
                    {
                        Player player = Main.player[NPC.target];

                        switch ((int)NPC.Calamity().newAI[0])
                        {
                            default:
                                NPC.ai[0] = Main.rand.NextBool() ? 6f : 5f;
                                break;
                            case 5:
                                NPC.ai[0] = phase4 ? 6f : Main.rand.NextBool() ? 4f : 6f;
                                break;
                            case 6:
                                NPC.ai[0] = phase4 ? 5f : Main.rand.NextBool() ? 5f : 4f;
                                break;
                        }

                        if (NPC.ai[0] == 4f || NPC.ai[0] == 6f)
                        {
                            NPC.ai[2] = 1f;
                            if (player != null && player.active && !player.dead && (player.Bottom.Y < NPC.Bottom.Y || Math.Abs(player.Center.X - NPC.Center.X) > 450f))
                            {
                                NPC.ai[0] = 5f;
                                NPC.ai[2] = 0f;
                            }
                        }
                    }
                    else
                    {
                        switch ((int)NPC.Calamity().newAI[0])
                        {
                            default:
                                NPC.ai[0] = Main.rand.NextBool() ? 5f : 4f;
                                break;
                            case 4:
                                NPC.ai[0] = Main.rand.NextBool() ? 3f : 5f;
                                break;
                            case 5:
                                NPC.ai[0] = Main.rand.NextBool() ? 4f : 3f;
                                break;
                        }
                    }

                    NPC.netUpdate = true;
                    break;

                // Enlarge after teleport
                case 1:

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.rotation = 0f;
                    NPC.ai[1] += 1f;
                    float teleportEndTime = death ? 15f : 20f;
                    slimeScale = MathHelper.Clamp(NPC.ai[1] / teleportEndTime, 0f, 1f);
                    slimeScale = 0.5f + slimeScale * 0.5f;
                    if (NPC.ai[1] >= teleportEndTime && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;

                        CalamityUtils.CalamityTargeting(NPC, default);
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportEndTime * 2f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;

                        CalamityUtils.CalamityTargeting(NPC, default);
                    }

                    // Emit teleport dust
                    Color newColor2 = NPC.AI_121_QueenSlime_GetDustColor();
                    newColor2.A = 150;
                    for (int i = 0; i < 10; i++)
                    {
                        int queenSlimeDust = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, 50, newColor2, 1.5f);
                        Main.dust[queenSlimeDust].noGravity = true;
                        Main.dust[queenSlimeDust].velocity *= 2f;
                    }

                    break;

                // Shrink and spawn teleport gore and dust
                case 2:

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.rotation = 0f;
                    NPC.ai[1] += 1f;
                    float teleportTime = death ? 30f : 40f;
                    slimeScale = MathHelper.Clamp((teleportTime - NPC.ai[1]) / teleportTime, 0f, 1f);
                    slimeScale = 0.5f + slimeScale * 0.5f;

                    if (NPC.ai[1] >= teleportTime)
                        teleported = true;

                    // Spawn crown gore
                    if (NPC.ai[1] == teleportTime)
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-40f, -NPC.height / 2), NPC.velocity, GoreID.QueenSlimeCrown);

                    if (NPC.ai[1] >= teleportTime && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.Bottom = new Vector2(NPC.localAI[1], NPC.localAI[2]);
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }

                    if (Main.netMode == NetmodeID.MultiplayerClient && NPC.ai[1] >= teleportTime * 2f)
                    {
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = 0f;
                    }

                    // Emit teleport dust
                    if (!teleported)
                    {
                        Color newColor = NPC.AI_121_QueenSlime_GetDustColor();
                        newColor.A = 150;
                        for (int n = 0; n < 10; n++)
                        {
                            int queenSlimeDust2 = Dust.NewDust(NPC.position + Vector2.UnitX * -20f, NPC.width + 40, NPC.height, DustID.TintableDust, NPC.velocity.X, NPC.velocity.Y, 50, newColor, 1.5f);
                            Main.dust[queenSlimeDust2].noGravity = true;
                            Main.dust[queenSlimeDust2].velocity *= 0.5f;
                        }
                    }

                    break;

                // She jump
                case 3:

                    // Faster fall
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += death ? 0.05f : 0f;

                    NPC.rotation = 0f;
                    if (NPC.velocity.Y == 0f)
                    {
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        NPC.velocity.X *= 0.8f;
                        if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                            NPC.velocity.X = 0f;

                        float timerIncrement = death ? 6f : 5f;
                        NPC.ai[1] += timerIncrement;
                        if (lifeRatio < 0.85f)
                            NPC.ai[1] += timerIncrement;
                        if (lifeRatio < 0.7f)
                            NPC.ai[1] += timerIncrement;

                        if (!(NPC.ai[1] >= 0f))
                            break;

                        // Set damage
                        NPC.damage = NPC.defDamage;

                        float distanceBelowTarget = NPC.position.Y - (Main.player[NPC.target].position.Y + 80f);
                        float speedMult = 1f;
                        if (distanceBelowTarget > 0f)
                            speedMult += distanceBelowTarget * 0.002f;

                        if (speedMult > 2f)
                            speedMult = 2f;

                        NPC.netUpdate = true;

                        CalamityUtils.CalamityTargeting(NPC, default);

                        if (NPC.ai[2] == 3f)
                        {
                            NPC.velocity.Y = -13f * speedMult;
                            NPC.velocity.X += (death ? 6f : 5.5f) * NPC.direction;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            if (NPC.timeLeft > 10)
                            {
                                NPC.Calamity().newAI[0] = NPC.ai[0];
                                NPC.SyncExtraAI();
                                NPC.ai[0] = 0f;
                            }
                            else
                                NPC.ai[1] = -60f;
                        }
                        else if (NPC.ai[2] == 2f)
                        {
                            NPC.velocity.Y = -(death ? 8f : 6f) * speedMult;
                            NPC.velocity.X += (death ? 7.5f : 7f) * NPC.direction;
                            NPC.ai[1] = -40f;
                            NPC.ai[2] += 1f;
                        }
                        else
                        {
                            NPC.velocity.Y = -(death ? 10f : 8f) * speedMult;
                            NPC.velocity.X += (death ? 6.5f : 6f) * NPC.direction;
                            NPC.ai[1] = -40f;
                            NPC.ai[2] += 1f;
                        }

                        NPC.noTileCollide = true;
                    }
                    else
                    {
                        if (NPC.target >= Main.maxPlayers)
                            break;

                        float jumpVelocity = death ? 7f : 4.5f;
                        if (Main.getGoodWorld)
                            jumpVelocity = 12f;

                        if ((NPC.direction == 1 && NPC.velocity.X < jumpVelocity) || (NPC.direction == -1 && NPC.velocity.X > 0f - jumpVelocity))
                        {
                            if ((NPC.direction == -1 && NPC.velocity.X < 0.1) || (NPC.direction == 1 && NPC.velocity.X > -0.1))
                                NPC.velocity.X += (death ? 0.45f : 0.3f) * NPC.direction;
                            else
                                NPC.velocity.X *= death ? 0.85f : 0.91f;
                        }

                        if (!Main.player[NPC.target].dead)
                        {
                            if (NPC.velocity.Y > 0f && NPC.Bottom.Y > Main.player[NPC.target].Top.Y)
                                NPC.noTileCollide = false;
                            else if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                                NPC.noTileCollide = false;
                            else
                                NPC.noTileCollide = true;
                        }
                    }

                    break;

                // Slam down and create shockwave
                // Create a cascade of crystals while falling down in phase 3 and the case is 4
                // Release a massive eruption of crystals in phase 3 and the case is 6
                case 4:
                case 6:

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.rotation *= 0.9f;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;

                    if (NPC.ai[2] == 1f)
                    {
                        NPC.noTileCollide = false;
                        NPC.noGravity = false;

                        int slamDelay = 30;
                        if (phase2)
                            slamDelay = 10;

                        if (Main.getGoodWorld)
                            slamDelay = 0;

                        if (NPC.velocity.Y == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item167, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ProjectileID.QueenSlimeSmash;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Bottom, Vector2.Zero, type, SlamDamage, 0f, Main.myPlayer);

                                // Line of explosions in Death Mode
                                if (death)
                                {
                                    float expandDelay = 0f;
                                    int maxSmashes = 16;
                                    float maxSmashOffset = maxSmashes * 100f;
                                    Vector2 extraSmashPosition = NPC.Bottom + Vector2.UnitX * maxSmashOffset;
                                    int maxSmashesPerSide = maxSmashes / 2;
                                    float maxExpandDelay = 15f * maxSmashesPerSide;
                                    float smashSpawnDistanceOffset = 200f;
                                    for (int i = 0; i < maxSmashes + 1; i++)
                                    {
                                        expandDelay = MathHelper.Lerp(0f, maxExpandDelay, Math.Abs(i - maxSmashesPerSide) / (float)maxSmashesPerSide);
                                        if (i != maxSmashesPerSide)
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), extraSmashPosition, Vector2.Zero, type, SlamDamage, 0f, Main.myPlayer, -expandDelay);

                                        extraSmashPosition -= Vector2.UnitX * smashSpawnDistanceOffset;
                                    }
                                }

                                // Eruption of crystals in phase 3
                                if (NPC.ai[0] == 6f && phase3)
                                {
                                    float projectileVelocity = 12f;
                                    type = ProjectileID.QueenSlimeMinionBlueSpike;
                                    Vector2 destination = (new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    destination *= projectileVelocity;
                                    int numProj = 20;
                                    float rotation = MathHelper.ToRadians(100);
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, SpikeDamage, 0f, Main.myPlayer, 0f, -2f);
                                    }

                                    if (phase5)
                                    {
                                        numProj = 12;
                                        destination *= 0.65f;
                                        for (int i = 0; i < numProj; i++)
                                        {
                                            Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, SpikeDamage, 0f, Main.myPlayer, 0f, -2f);
                                        }
                                    }
                                }
                            }

                            for (int l = 0; l < 20; l++)
                            {
                                int slamDust = Dust.NewDust(NPC.Bottom - new Vector2(NPC.width / 2, 30f), NPC.width, 30, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 40, NPC.AI_121_QueenSlime_GetDustColor());
                                Main.dust[slamDust].noGravity = true;
                                Main.dust[slamDust].velocity.Y = -5f + Main.rand.NextFloat() * -3f;
                                Main.dust[slamDust].velocity.X *= 7f;
                            }

                            NPC.Calamity().newAI[0] = NPC.ai[0];
                            NPC.SyncExtraAI();
                            NPC.ai[0] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.ai[1] >= slamDelay)
                        {
                            for (int m = 0; m < 4; m++)
                            {
                                Vector2 position = NPC.Bottom - new Vector2(Main.rand.NextFloatDirection() * 16f, Main.rand.Next(8));
                                int slamDust2 = Dust.NewDust(position, 2, 2, DustID.Smoke, NPC.velocity.X, NPC.velocity.Y, 40, NPC.AI_121_QueenSlime_GetDustColor(), 1.4f);
                                Main.dust[slamDust2].position = position;
                                Main.dust[slamDust2].noGravity = true;
                                Main.dust[slamDust2].velocity.Y = NPC.velocity.Y * 0.9f;
                                Main.dust[slamDust2].velocity.X = (Main.rand.NextBool() ? (-10f) : 10f) + Main.rand.NextFloatDirection() * 3f;
                            }
                        }

                        NPC.velocity.X *= 0.8f;
                        float slamNetUpdate = NPC.ai[1];
                        NPC.ai[1] += 1f;
                        if (NPC.ai[1] >= slamDelay)
                        {
                            if (slamNetUpdate < slamDelay)
                                NPC.netUpdate = true;

                            if (phase2 && NPC.ai[1] > (slamDelay + 120))
                            {
                                NPC.Calamity().newAI[0] = NPC.ai[0];
                                NPC.SyncExtraAI();
                                NPC.ai[0] = 0f;
                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                                NPC.velocity.Y *= 0.8f;
                                NPC.netUpdate = true;
                                break;
                            }

                            // Set damage
                            NPC.damage = NPC.defDamage;

                            NPC.velocity.Y += death ? 1.75f : 1.5f;
                            float slamVelocity = death ? 15.5f : 15f;
                            if (Main.getGoodWorld)
                            {
                                NPC.velocity.Y += 1f;
                                slamVelocity = 15.99f;
                            }

                            if (NPC.velocity.Y == 0f)
                                NPC.velocity.Y = 0.01f;

                            if (NPC.velocity.Y >= slamVelocity)
                                NPC.velocity.Y = slamVelocity;

                            // Cascade of crystals in phase 3 or 4 while falling down
                            if (((NPC.ai[0] == 4f && phase3) || phase4) && NPC.ai[1] % 12f == 0f)
                            {
                                SoundEngine.PlaySound(SoundID.Item154, NPC.Center);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 fireFrom = NPC.Center;
                                    int projectileAmt = 2;
                                    int type = ProjectileID.QueenSlimeMinionBlueSpike;
                                    Vector2 velocityIncrease = death ? (Vector2.UnitY * 4f) : Vector2.Zero;
                                    for (int i = 0; i < projectileAmt; i++)
                                    {
                                        int totalProjectiles = 2;
                                        float radians = MathHelper.TwoPi / totalProjectiles;
                                        for (int j = 0; j < totalProjectiles; j++)
                                        {
                                            Vector2 projVelocity = (NPC.velocity + velocityIncrease).RotatedBy(radians * j + MathHelper.PiOver2);
                                            Projectile.NewProjectile(NPC.GetSource_FromAI(), fireFrom, projVelocity, type, SpikeDamage, 0f, Main.myPlayer, 0f, -1f);
                                        }
                                    }
                                }
                            }
                        }
                        else
                            NPC.velocity.Y *= 0.8f;

                        break;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] == 0f)
                    {
                        CalamityUtils.CalamityTargeting(NPC, default);

                        NPC.netUpdate = true;
                    }

                    NPC.ai[1] += 1f;
                    if (!(NPC.ai[1] >= 30f))
                        break;

                    if (NPC.ai[1] >= 60f)
                    {
                        NPC.ai[1] = 60f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 1f;
                            NPC.velocity.Y = -3f;
                            NPC.netUpdate = true;
                        }
                    }

                    Player player3 = Main.player[NPC.target];
                    Vector2 center = NPC.Center;
                    if (!player3.dead && player3.active && Math.Abs(NPC.Center.X - player3.Center.X) / 16f <= despawnDistanceInTiles)
                        center = player3.Center;

                    center.Y -= 384f;
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.velocity = center - NPC.Center;
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero);
                        NPC.velocity *= death ? 27.3f : 24f;
                    }
                    else
                        NPC.velocity.Y *= 0.95f;

                    break;

                // Fire spread of gel projectiles
                case 5:

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.rotation *= 0.9f;
                    NPC.noTileCollide = true;
                    NPC.noGravity = true;

                    if (phase2)
                        NPC.ai[3] = 0f;

                    if (NPC.ai[2] == 1f)
                    {
                        NPC.ai[1] += 1f;
                        if (!(NPC.ai[1] >= 10f))
                            break;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int numGelProjectiles = phase4 ? Main.rand.Next(9, 12) : phase2 ? Main.rand.Next(6, 9) : 12;
                            if (Main.getGoodWorld)
                                numGelProjectiles = 15;

                            float projectileVelocity = death ? 12f : 10.5f;
                            int type = ProjectileID.QueenSlimeGelAttack;
                            if (phase2)
                            {
                                Vector2 destination = (new Vector2(NPC.Center.X, NPC.Center.Y + 100f) - NPC.Center).SafeNormalize(Vector2.UnitY);
                                destination *= projectileVelocity;
                                float rotation = MathHelper.ToRadians(120);
                                for (int i = 0; i < numGelProjectiles; i++)
                                {
                                    if (Main.getGoodWorld)
                                        destination *= Main.rand.NextFloat() + 0.5f;

                                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numGelProjectiles - 1)));
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, LargeGelDamage, 0f, Main.myPlayer, 0f, -2f);
                                    Main.projectile[proj].timeLeft = 900;
                                }
                            }
                            else
                            {
                                for (int j = 0; j < numGelProjectiles; j++)
                                {
                                    Vector2 spinningpoint = new Vector2(projectileVelocity, 0f);

                                    if (Main.getGoodWorld)
                                        spinningpoint *= Main.rand.NextFloat() + 0.5f;

                                    spinningpoint = spinningpoint.RotatedBy((-j) * MathHelper.TwoPi / numGelProjectiles, Vector2.Zero);
                                    int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, spinningpoint, type, LargeGelDamage, 0f, Main.myPlayer, 0f, -2f);
                                    Main.projectile[proj].timeLeft = 900;
                                }
                            }

                            // Fire gel balls directly at players with a max of 3
                            List<int> targets = new List<int>();
                            for (int p = 0; p < Main.maxPlayers; p++)
                            {
                                if (Main.player[p].active && !Main.player[p].dead)
                                    targets.Add(p);

                                if (targets.Count > 2)
                                    break;
                            }
                            foreach (int t in targets)
                            {
                                Vector2 velocity2 = (Main.player[t].Center - NPC.Center).SafeNormalize(Vector2.UnitY) * projectileVelocity;
                                int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, type, LargeGelDamage, 0f, Main.myPlayer, 0f, -2f);
                                Main.projectile[proj].timeLeft = 900;
                            }
                        }

                        SoundEngine.PlaySound(SoundID.Item155, NPC.Center);
                        NPC.Calamity().newAI[0] = NPC.ai[0];
                        NPC.SyncExtraAI();
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.netUpdate = true;
                        break;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] == 0f)
                    {
                        CalamityUtils.CalamityTargeting(NPC, default);

                        NPC.netUpdate = true;
                    }

                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] >= 50f)
                    {
                        NPC.ai[1] = 50f;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 1f;
                            NPC.netUpdate = true;
                        }
                    }

                    float slamDustRadius = 100f;
                    for (int k = 0; k < 4; k++)
                    {
                        Vector2 slamDustArea = NPC.Center + Main.rand.NextVector2CircularEdge(slamDustRadius, slamDustRadius);
                        if (!phase2)
                            slamDustArea += new Vector2(0f, 20f);

                        Vector2 v = slamDustArea - NPC.Center;
                        v = v.SafeNormalize(Vector2.Zero) * -8f;
                        int superSlamDust = Dust.NewDust(slamDustArea, 2, 2, DustID.Smoke, v.X, v.Y, 40, NPC.AI_121_QueenSlime_GetDustColor(), 1.8f);
                        Main.dust[superSlamDust].position = slamDustArea;
                        Main.dust[superSlamDust].noGravity = true;
                        Main.dust[superSlamDust].alpha = 250;
                        Main.dust[superSlamDust].velocity = v;
                        Main.dust[superSlamDust].customData = NPC;
                    }

                    if (phase2)
                        QueenSlime_FlyMovement(NPC);

                    break;
            }

            // Don't take damage while teleporting
            NPC.dontTakeDamage = NPC.hide = teleported;

            // Adjust size with HP
            if (slimeScale != NPC.scale)
            {
                NPC.position.X += NPC.width / 2;
                NPC.position.Y += NPC.height;
                NPC.scale = slimeScale;
                NPC.width = (int)(114f * NPC.scale);
                NPC.height = (int)(100f * NPC.scale);
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height;
            }

            // Spawn small slimes
            // Don't spawn any slimes in final phase
            if (NPC.life <= 0 || (phase4 && !death) || phase5)
                return false;

            if (Main.netMode == NetmodeID.MultiplayerClient)
                return false;

            // Reset numerous variables when phase 2 begins
            if (NPC.localAI[0] >= (NPC.lifeMax / 2) && NPC.life < NPC.lifeMax / 2)
            {
                NPC.localAI[0] = NPC.life;
                NPC.ai[0] = 0f;
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
                NPC.netUpdate = true;
            }

            float slimeSpawnHealthGateValue = Main.zenithWorld ? 0.01f : phase3 ? 0.04f : phase2 ? 0.03f : 0.025f;
            if (death)
                slimeSpawnHealthGateValue *= 0.5f;

            int slimeSpawnThreshold = (int)(NPC.lifeMax * slimeSpawnHealthGateValue);

            if (!((NPC.life + slimeSpawnThreshold) < NPC.localAI[0]))
                return false;

            NPC.localAI[0] = NPC.life;

            int offset = 16;
            int x = (int)(NPC.position.X + offset + Main.rand.Next(NPC.width - offset * 2));
            int y = (int)(NPC.position.Y + offset + Main.rand.Next(NPC.height - offset * 2));

            int random = Main.rand.Next(2);
            if (phase2)
                random += 1;
            if (phase3)
                random = 2;

            int typeToSpawn = NPCID.QueenSlimeMinionBlue;
            switch (random)
            {
                case 0:
                    typeToSpawn = NPCID.QueenSlimeMinionBlue;
                    break;
                case 1:
                    typeToSpawn = NPCID.QueenSlimeMinionPink;
                    break;
                case 2:
                    typeToSpawn = NPCID.QueenSlimeMinionPurple;
                    break;
            }

            int slimeScale2 = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, typeToSpawn);
            Main.npc[slimeScale2].SetDefaults(typeToSpawn);
            Main.npc[slimeScale2].velocity.X = Main.rand.Next(-15, 16) * 0.1f;
            Main.npc[slimeScale2].velocity.Y = Main.rand.Next(-30, 1) * 0.1f;
            Main.npc[slimeScale2].ai[0] = -500 * Main.rand.Next(3);
            Main.npc[slimeScale2].ai[1] = 0f;
            if (Main.dedServ && slimeScale2 < Main.maxNPCs)
                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, slimeScale2);

            return false;
        }

        public static void QueenSlime_FlyMovement(NPC npc)
        {
            // Difficulty bools
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            npc.noTileCollide = true;
            npc.noGravity = true;

            float flyVelocity = death ? 19f : 16f;
            float flyAcceleration = death ? 0.15f : 0.12f;
            float flyDistanceY = 450f;

            Vector2 desiredVelocity = npc.Center;

            if (npc.timeLeft > 10)
            {
                if (!Collision.CanHit(npc, Main.player[npc.target]))
                {
                    bool flyToSolidTilesAboveTarget = false;
                    Vector2 center = Main.player[npc.target].Center;
                    for (int i = 0; i < 16; i++)
                    {
                        float tileDistanceAboveTarget = 16 * i;
                        Point point = (center + new Vector2(0f, 0f - tileDistanceAboveTarget)).ToTileCoordinates();
                        if (WorldGen.SolidOrSlopedTile(point.X, point.Y))
                        {
                            desiredVelocity = center + new Vector2(0f, 0f - tileDistanceAboveTarget + 16f) - npc.Center;
                            flyToSolidTilesAboveTarget = true;
                            break;
                        }
                    }

                    if (!flyToSolidTilesAboveTarget)
                        desiredVelocity = center - npc.Center;
                }
                else
                    desiredVelocity = Main.player[npc.target].Center + new Vector2(0f, -flyDistanceY) - npc.Center;
            }
            else
                desiredVelocity = npc.Center + new Vector2(500f * npc.direction, -flyDistanceY) - npc.Center;

            float distanceFromFlightTarget = desiredVelocity.Length();
            if (Math.Abs(desiredVelocity.X) < 40f)
                desiredVelocity.X = npc.velocity.X;

            if (distanceFromFlightTarget > 100f && ((npc.velocity.X < -12f && desiredVelocity.X > 0f) || (npc.velocity.X > 12f && desiredVelocity.X < 0f)))
                flyAcceleration = 0.2f;

            if (distanceFromFlightTarget < 40f)
            {
                desiredVelocity = npc.velocity;
            }
            else if (distanceFromFlightTarget < 80f)
            {
                desiredVelocity = desiredVelocity.SafeNormalize(Vector2.UnitY);
                desiredVelocity *= flyVelocity * 0.65f;
            }
            else
            {
                desiredVelocity = desiredVelocity.SafeNormalize(Vector2.UnitY);
                desiredVelocity *= flyVelocity;
            }

            npc.SimpleFlyMovement(desiredVelocity, flyAcceleration);
            npc.rotation = npc.velocity.X * 0.1f;
            if (npc.rotation > 0.5f)
                npc.rotation = 0.5f;

            if (npc.rotation < -0.5f)
                npc.rotation = -0.5f;
        }

        public class CrystalSlimeAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                if (NPC.localAI[0] > 0f)
                    NPC.localAI[0] -= 1f;

                if (!NPC.wet && Main.player[NPC.target].active && !Main.player[NPC.target].dead && !Main.player[NPC.target].npcTypeNoAggro[NPC.type])
                {
                    Player obj = Main.player[NPC.target];
                    Vector2 center = NPC.Center;
                    float num19 = obj.Center.X - center.X;
                    float num20 = obj.Center.Y - center.Y;
                    float num21 = (float)Math.Sqrt(num19 * num19 + num20 * num20);
                    int num22 = NPC.CountNPCS(NPCID.QueenSlimeMinionBlue);
                    if (num22 < 5 && Math.Abs(num19) < 500f && Math.Abs(num20) < 550f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && NPC.velocity.Y == 0f)
                    {
                        NPC.ai[0] = -40f;
                        if (NPC.velocity.Y == 0f)
                            NPC.velocity.X *= 0.9f;

                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f)
                        {
                            for (int k = 0; k < 3; k++)
                            {
                                Vector2 vector6 = new Vector2(k - 1, -4f);
                                vector6.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                                vector6.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                                vector6.Normalize();
                                vector6 *= 6f + (float)Main.rand.Next(-50, 51) * 0.01f;
                                if (num21 > 350f)
                                    vector6 *= 2f;
                                else if (num21 > 250f)
                                    vector6 *= 1.5f;

                                int type = ProjectileID.QueenSlimeMinionBlueSpike;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), center, vector6 * (death ? 0.7f : 0.5f), type, SpikeDamage, 0f, Main.myPlayer);
                                NPC.localAI[0] = death ? 50f : 25f;
                                if (num22 > 4)
                                    break;
                            }
                        }
                    }
                    else if (Math.Abs(num19) < 500f && Math.Abs(num20) < 550f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && NPC.velocity.Y == 0f)
                    {
                        float num23 = num21;
                        NPC.ai[0] = -40f;
                        if (NPC.velocity.Y == 0f)
                            NPC.velocity.X *= 0.9f;

                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f)
                        {
                            num20 = Main.player[NPC.target].position.Y - center.Y - (float)Main.rand.Next(0, 200);
                            num21 = (float)Math.Sqrt(num19 * num19 + num20 * num20);
                            num21 = 4.5f / num21;
                            num21 *= 2f;
                            if (num23 > 350f)
                                num21 *= 2f;
                            else if (num23 > 250f)
                                num21 *= 1.5f;

                            num19 *= num21;
                            num20 *= num21;
                            NPC.localAI[0] = death ? 100f : 50f;
                            int type = ProjectileID.QueenSlimeMinionBlueSpike;
                            Vector2 spikeVelocity = new Vector2(num19, num20) * (death ? 0.7f : 0.5f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), center, spikeVelocity, type, SpikeDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                if (NPC.ai[2] > 1f)
                    NPC.ai[2] -= 1f;

                if (NPC.wet)
                {
                    if (NPC.collideY)
                        NPC.velocity.Y = -2f;

                    if (NPC.velocity.Y < 0f && NPC.ai[3] == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        NPC.ai[2] = 200f;
                    }

                    if (NPC.velocity.Y > 0f)
                        NPC.ai[3] = NPC.position.X;

                    if (NPC.velocity.Y > 2f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.Y -= 0.5f;
                    if (NPC.velocity.Y < -4f)
                        NPC.velocity.Y = -4f;

                    if (NPC.ai[2] == 1f)
                        CalamityUtils.CalamityTargeting(NPC, default);
                }

                NPC.aiAction = 0;
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[0] = -100f;
                    NPC.ai[2] = 1f;
                    CalamityUtils.CalamityTargeting(NPC, default);
                }

                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.collideY && NPC.oldVelocity.Y != 0f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.position.X -= NPC.velocity.X + (float)NPC.direction;

                    if (NPC.ai[3] == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        NPC.ai[2] = 200f;
                    }

                    NPC.ai[3] = 0f;
                    NPC.velocity.X *= 0.8f;
                    if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;

                    NPC.ai[0] += death ? 16f : 10f;

                    float num33 = -1000f;

                    int num34 = 0;
                    if (NPC.ai[0] >= 0f)
                        num34 = 1;

                    if (NPC.ai[0] >= num33 && NPC.ai[0] <= num33 * 0.5f)
                        num34 = 2;

                    if (NPC.ai[0] >= num33 * 2f && NPC.ai[0] <= num33 * 1.5f)
                        num34 = 3;

                    if (num34 > 0)
                    {
                        NPC.netUpdate = true;
                        if (NPC.ai[2] == 1f)
                            CalamityUtils.CalamityTargeting(NPC, default);

                        if (num34 == 3)
                        {
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X += (death ? 9 : 6) * NPC.direction;
                            NPC.ai[0] = -200f;
                            NPC.ai[3] = NPC.position.X;
                        }
                        else
                        {
                            NPC.velocity.Y = -6f;
                            NPC.velocity.X += (death ? 6 : 4) * NPC.direction;
                            NPC.ai[0] = -120f;
                            if (num34 == 1)
                                NPC.ai[0] += num33;
                            else
                                NPC.ai[0] += num33 * 2f;
                        }
                    }
                    else if (NPC.ai[0] >= -30f)
                        NPC.aiAction = 1;
                }
                else if (NPC.target < Main.maxPlayers && ((NPC.direction == 1 && NPC.velocity.X < 3f) || (NPC.direction == -1 && NPC.velocity.X > -3f)))
                {
                    if (NPC.collideX && Math.Abs(NPC.velocity.X) == 0.2f)
                        NPC.position.X -= 1.4f * (float)NPC.direction;

                    if (NPC.collideY && NPC.oldVelocity.Y != 0f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.position.X -= NPC.velocity.X + (float)NPC.direction;

                    if ((NPC.direction == -1 && (double)NPC.velocity.X < 0.01) || (NPC.direction == 1 && (double)NPC.velocity.X > -0.01))
                        NPC.velocity.X += 0.2f * (float)NPC.direction;
                    else
                        NPC.velocity.X *= 0.93f;
                }

                return false;
            }
        }

        public class BouncySlimeAI : VanillaAIOverride
        {
            public override bool AI(Mod mod)
            {
                bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

                if (NPC.localAI[0] > 0f)
                    NPC.localAI[0] -= 1f;

                if (!NPC.wet && Main.player[NPC.target].active && !Main.player[NPC.target].dead && !Main.player[NPC.target].npcTypeNoAggro[NPC.type])
                {
                    Player obj2 = Main.player[NPC.target];
                    Vector2 center2 = NPC.Center;
                    float num24 = obj2.Center.X - center2.X;
                    float num25 = obj2.Center.Y - center2.Y;
                    float num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
                    float num27 = num26;
                    if (Math.Abs(num24) < 500f && Math.Abs(num25) < 550f && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) && NPC.velocity.Y == 0f)
                    {
                        NPC.ai[0] = -40f;
                        if (NPC.velocity.Y == 0f)
                            NPC.velocity.X *= 0.9f;

                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] == 0f)
                        {
                            num25 = Main.player[NPC.target].position.Y - center2.Y - (float)Main.rand.Next(0, 200);
                            num26 = (float)Math.Sqrt(num24 * num24 + num25 * num25);
                            num26 = 4.5f / num26;
                            num26 *= 2f;
                            if (num27 > 350f)
                                num26 *= 1.75f;
                            else if (num27 > 250f)
                                num26 *= 1.25f;

                            num24 *= num26;
                            num25 *= num26;
                            NPC.localAI[0] = death ? 60f : 30f;

                            int type = ProjectileID.QueenSlimeMinionPinkBall;
                            Vector2 pinkBallVelocity = new Vector2(num24, num25) * (death ? 0.7f : 0.5f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), center2, pinkBallVelocity, type, SmallGelDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                if (NPC.ai[2] > 1f)
                    NPC.ai[2] -= 1f;

                if (NPC.wet)
                {
                    if (NPC.collideY)
                        NPC.velocity.Y = -2f;

                    if (NPC.velocity.Y < 0f && NPC.ai[3] == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        NPC.ai[2] = 200f;
                    }

                    if (NPC.velocity.Y > 0f)
                        NPC.ai[3] = NPC.position.X;

                    if (NPC.velocity.Y > 2f)
                        NPC.velocity.Y *= 0.9f;

                    NPC.velocity.Y -= 0.5f;
                    if (NPC.velocity.Y < -4f)
                        NPC.velocity.Y = -4f;

                    if (NPC.ai[2] == 1f)
                        CalamityUtils.CalamityTargeting(NPC, default);
                }

                NPC.aiAction = 0;
                if (NPC.ai[2] == 0f)
                {
                    NPC.ai[0] = -100f;
                    NPC.ai[2] = 1f;
                    CalamityUtils.CalamityTargeting(NPC, default);
                }

                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.collideY && NPC.oldVelocity.Y != 0f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.position.X -= NPC.velocity.X + (float)NPC.direction;

                    if (NPC.ai[3] == NPC.position.X)
                    {
                        NPC.direction *= -1;
                        NPC.ai[2] = 200f;
                    }

                    NPC.ai[3] = 0f;
                    NPC.velocity.X *= 0.8f;
                    if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;

                    NPC.ai[0] += death ? 11f : 7f;

                    float num33 = -500f;

                    int num34 = 0;
                    if (NPC.ai[0] >= 0f)
                        num34 = 1;

                    if (NPC.ai[0] >= num33 && NPC.ai[0] <= num33 * 0.5f)
                        num34 = 2;

                    if (NPC.ai[0] >= num33 * 2f && NPC.ai[0] <= num33 * 1.5f)
                        num34 = 3;

                    if (num34 > 0)
                    {
                        NPC.netUpdate = true;
                        if (NPC.ai[2] == 1f)
                            CalamityUtils.CalamityTargeting(NPC, default);

                        if (num34 == 3)
                        {
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X += (death ? 9 : 6) * NPC.direction;
                            NPC.ai[0] = -200f;
                            NPC.ai[3] = NPC.position.X;
                        }
                        else
                        {
                            NPC.velocity.Y = -6f;
                            NPC.velocity.X += (death ? 6 : 4) * NPC.direction;
                            NPC.ai[0] = -120f;
                            if (num34 == 1)
                                NPC.ai[0] += num33;
                            else
                                NPC.ai[0] += num33 * 2f;
                        }

                        NPC.velocity.Y *= 1.6f;
                        NPC.velocity.X *= 1.2f;
                    }
                    else if (NPC.ai[0] >= -30f)
                        NPC.aiAction = 1;
                }
                else if (NPC.target < Main.maxPlayers && ((NPC.direction == 1 && NPC.velocity.X < 3f) || (NPC.direction == -1 && NPC.velocity.X > -3f)))
                {
                    if (NPC.collideX && Math.Abs(NPC.velocity.X) == 0.2f)
                        NPC.position.X -= 1.4f * (float)NPC.direction;

                    if (NPC.collideY && NPC.oldVelocity.Y != 0f && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                        NPC.position.X -= NPC.velocity.X + (float)NPC.direction;

                    if ((NPC.direction == -1 && (double)NPC.velocity.X < 0.01) || (NPC.direction == 1 && (double)NPC.velocity.X > -0.01))
                        NPC.velocity.X += 0.2f * (float)NPC.direction;
                    else
                        NPC.velocity.X *= 0.93f;
                }

                return false;
            }
        }
    }
}
