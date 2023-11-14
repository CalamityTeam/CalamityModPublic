using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalAI
    {
        internal enum MothronAIState
        {
            DespawnYeet = -1,
            NewAISelection = 0,
            FlyTowardsPlayer = 1,
            AccelerateTowardsPlayer = 2,
            ChargeRedirect = 3,
            ChargePreparation = 4,
            DoTheFuckingCharge = 5,
            PickSpotToLayEgg = 6,
            FlyToEggSpot = 7,
            LayEgg = 8
        }

        #region Buffed Mothron AI
        public static bool BuffedMothronAI(NPC npc)
        {
            npc.noTileCollide = false;
            npc.noGravity = true;
            npc.knockBackResist = 0f;
            npc.damage = npc.defDamage;

            ref float aiState = ref npc.ai[0];

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.4f;
            bool phase3 = lifeRatio < 0.1f;

            Player target = Main.player[npc.target];

            // Despawn if no valid target exists or there's no ongoing eclipse.
            if (!Main.eclipse)
                aiState = (int)MothronAIState.DespawnYeet;
            else if (npc.target < 0 || target.dead || !target.active)
            {
                npc.TargetClosest(true);
                if (target.dead)
                {
                    aiState = (int)MothronAIState.DespawnYeet;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        npc.netUpdate = true;
                }
            }

            float flyInertia;
            float chargeSpeed = 32f;
            Vector2 idealFlyVelocity;
            switch ((MothronAIState)(int)aiState)
            {
                case MothronAIState.DespawnYeet:
                    Vector2 idealVelocity = Vector2.UnitY * -34f;
                    npc.velocity = (npc.velocity * 4f + idealVelocity) / 5f;
                    npc.noTileCollide = true;
                    npc.dontTakeDamage = true;
                    return false;

                case MothronAIState.NewAISelection:
                    ref float aiTimer = ref npc.ai[1];

                    npc.TargetClosest(true);

                    if (npc.Center.X < target.Center.X - 2f)
                        npc.direction = 1;
                    if (npc.Center.X > target.Center.X + 2f)
                        npc.direction = -1;

                    npc.spriteDirection = npc.direction;
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;

                    // Rebounding on tile collision.
                    if (npc.collideX)
                    {
                        npc.velocity.X *= -npc.oldVelocity.X * 0.5f;
                        npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -4f, 4f);
                    }
                    if (npc.collideY)
                    {
                        npc.velocity.Y *= -npc.oldVelocity.Y * 0.5f;
                        npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -4f, 4f);
                    }

                    Vector2 destinationAboveTarget = target.Center - Vector2.UnitY * 200f;
                    float distanceFromAboveTarget = npc.Distance(destinationAboveTarget);
                    if (distanceFromAboveTarget > 3000f)
                    {
                        aiState = (int)MothronAIState.FlyTowardsPlayer;
                        aiTimer = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    // Otherwise fly towards the destination if relatively far from it.
                    else if (distanceFromAboveTarget > 600f)
                    {
                        flyInertia = 30f;
                        idealFlyVelocity = npc.SafeDirectionTo(destinationAboveTarget, -Vector2.UnitY) * 15f;
                        npc.velocity = (npc.velocity * (flyInertia - 1f) + idealFlyVelocity) / flyInertia;
                    }

                    // And otherwise, if near the destination, slow down a bit.
                    else if (npc.velocity.Length() > 2f)
                        npc.velocity *= 0.95f;
                    else if (npc.velocity.Length() < 1f)
                        npc.velocity *= 1.05f;

                    aiTimer++;

                    // Select a new AI state after 10 frames.
                    if (aiTimer >= 10f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        aiTimer = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;

                        while ((MothronAIState)(int)aiState == MothronAIState.NewAISelection)
                        {
                            int selection = Main.rand.Next(3);
                            if (phase3)
                                selection = 1;
                            else if (phase2)
                                selection = Main.rand.Next(2);

                            if (selection == 0 && Collision.CanHit(npc.Center, 1, 1, target.Center, 1, 1))
                                aiState = (int)MothronAIState.AccelerateTowardsPlayer;
                            else if (selection == 1)
                                aiState = (int)MothronAIState.ChargeRedirect;
                            else if (selection == 2 && NPC.CountNPCS(NPCID.MothronEgg) + NPC.CountNPCS(NPCID.MothronSpawn) < 2)
                                aiState = (int)MothronAIState.PickSpotToLayEgg;
                        }
                        npc.netUpdate = true;
                        npc.netSpam = 0;
                    }
                    break;

                case MothronAIState.FlyTowardsPlayer:
                    npc.collideX = false;
                    npc.collideY = false;
                    npc.noTileCollide = true;

                    if (npc.target < 0 || !target.active || target.dead)
                        npc.TargetClosest(true);

                    npc.spriteDirection = npc.direction = (npc.velocity.X > 0).ToDirectionInt();
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.02f) / 10f;

                    // Don't bother flying anymore if we're stuck and the target is somewhat close.
                    if (npc.WithinRange(target.Center, 500f) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        aiState = (int)MothronAIState.NewAISelection;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    float flySpeed = 18f + npc.Distance(target.Center) / 100f;
                    flyInertia = 25f;
                    idealFlyVelocity = npc.SafeDirectionTo(target.Center, -Vector2.UnitY) * flySpeed;
                    npc.velocity = (npc.velocity * (flyInertia - 1f) + idealFlyVelocity) / flyInertia;
                    break;

                case MothronAIState.AccelerateTowardsPlayer:
                    aiTimer = ref npc.ai[1];
                    ref float flySpeedAdditive = ref npc.ai[2];

                    // If no valid target exists, try to find a new one and select a new attack.
                    if (npc.target < 0 || !target.active || target.dead)
                    {
                        npc.TargetClosest(true);
                        aiState = (int)MothronAIState.NewAISelection;
                        aiTimer = 0f;
                        flySpeedAdditive = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }

                    npc.spriteDirection = npc.direction = (npc.velocity.X > 0).ToDirectionInt();
                    npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.025f) / 5f;

                    // Rebounding on tile collision.
                    if (npc.collideX)
                    {
                        npc.velocity.X *= -npc.oldVelocity.X * 0.5f;
                        npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -4f, 4f);
                    }
                    if (npc.collideY)
                    {
                        npc.velocity.Y *= -npc.oldVelocity.Y * 0.5f;
                        npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -4f, 4f);
                    }

                    Vector2 destination = target.Center - Vector2.UnitY * 20f;

                    flySpeedAdditive += 0.0222222228f;
                    if (Main.expertMode)
                        flySpeedAdditive += 0.0166666675f;

                    flySpeed = 12f + flySpeedAdditive + npc.Distance(destination) / 120f;
                    flyInertia = 20f;
                    idealFlyVelocity = npc.SafeDirectionTo(destination, -Vector2.UnitY) * flySpeed;
                    npc.velocity = (npc.velocity * (flyInertia - 1f) + idealFlyVelocity) / flyInertia;

                    aiTimer++;
                    // Stop flying if there's an obstacle between the npc and target.
                    if (aiTimer >= 120f || !Collision.CanHit(npc.Center, 1, 1, target.Center, 1, 1))
                    {
                        aiState = (int)MothronAIState.NewAISelection;
                        aiTimer = 0f;
                        flySpeedAdditive = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                    }
                    break;

                case MothronAIState.ChargeRedirect:
                    flySpeedAdditive = ref npc.ai[2];
                    npc.noTileCollide = true;

                    npc.spriteDirection = npc.direction = (npc.velocity.X > 0).ToDirectionInt();
                    npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

                    destination = target.Center;
                    destination -= Vector2.UnitY * 12f;

                    float xOffset = 600f;
                    if (npc.Center.X > target.Center.X)
                        destination.X += xOffset;
                    else
                        destination.X -= xOffset;

                    // If close to the destination beside the player, enter the charge phase.
                    if (Main.netMode != NetmodeID.MultiplayerClient &&
                        Math.Abs(npc.Center.X - target.Center.X) > xOffset - 50f && Math.Abs(npc.Center.Y - target.Center.Y) < 20f)
                    {
                        aiState = (int)MothronAIState.ChargePreparation;
                        flySpeedAdditive = 0f;
                        npc.netUpdate = true;
                        npc.netSpam = 0;
                    }

                    flySpeedAdditive += 0.0333333351f;
                    flySpeed = 24f + flySpeedAdditive;
                    flyInertia = 4f;
                    idealVelocity = npc.SafeDirectionTo(destination, -Vector2.UnitY) * flySpeed;
                    npc.velocity = (npc.velocity * (flyInertia - 1f) + idealVelocity) / flyInertia;
                    break;
                case MothronAIState.ChargePreparation:
                    aiTimer = ref npc.ai[1];
                    ref float chargeDirection = ref npc.ai[2];

                    npc.noTileCollide = true;
                    npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;

                    destination = target.Center - Vector2.UnitY * 12f;

                    float chargePreperationInertia = 8f;
                    Vector2 chargeVelocity = npc.SafeDirectionTo(destination, -Vector2.UnitY) * chargeSpeed;
                    npc.velocity = (npc.velocity * (chargePreperationInertia - 1f) + chargeVelocity) / chargePreperationInertia;
                    npc.spriteDirection = npc.direction = (npc.velocity.X > 0).ToDirectionInt();

                    // Redirect for 10 frames. After that time has been spent, immediately charge as usual.
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        aiTimer++;
                        if (aiTimer > 10f)
                        {
                            npc.velocity = chargeVelocity;

                            if (npc.velocity.X < 0f)
                                npc.direction = -1;
                            else
                                npc.direction = 1;

                            aiState = (int)MothronAIState.DoTheFuckingCharge;
                            chargeDirection = npc.direction;
                            npc.netUpdate = true;
                            npc.netSpam = 0;
                        }
                    }
                    break;

                case MothronAIState.DoTheFuckingCharge:
                    chargeDirection = ref npc.ai[2];
                    flySpeedAdditive = ref npc.ai[3];

                    npc.damage = (int)(npc.defDamage * 1.2);
                    npc.collideX = false;
                    npc.collideY = false;
                    npc.noTileCollide = true;
                    flySpeedAdditive += 0.0333333351f;
                    npc.velocity.X = (chargeSpeed + flySpeedAdditive) * chargeDirection;

                    float chargeDistance = 460f;
                    if (Main.netMode != NetmodeID.MultiplayerClient &&
                        (chargeDirection > 0f && npc.Center.X > target.Center.X + chargeDistance) ||
                        (chargeDirection < 0f && npc.Center.X < target.Center.X - chargeDistance))
                    {
                        // If not stuck, pick a new attack.
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            aiState = (int)MothronAIState.NewAISelection;
                            chargeDirection = 0f;
                            flySpeedAdditive = 0f;
                            npc.netUpdate = true;
                        }

                        // Otherwise, if somewhat horizontally far from the target, go to typical flying by default.
                        else if (Math.Abs(npc.Center.X - target.Center.X) > chargeDistance * 2f - 120f)
                        {
                            aiState = (int)MothronAIState.FlyTowardsPlayer;
                            chargeDirection = 0f;
                            flySpeedAdditive = 0f;
                            npc.netUpdate = true;
                        }
                        npc.netSpam = 0;
                    }
                    npc.rotation = (npc.rotation * 4f + npc.velocity.X * 0.0175f) / 5f;
                    break;

                case MothronAIState.PickSpotToLayEgg:
                    ref float laySpotPositionX = ref npc.ai[2];
                    ref float laySpotPositionY = ref npc.ai[3];
                    // Fallback if the spot selection fails.
                    npc.TargetClosest(true);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        aiState = (int)MothronAIState.NewAISelection;
                        laySpotPositionX = laySpotPositionY = -1f;

                        for (int i = 0; i < 1000; i++)
                        {
                            int potentialSpotX = (int)target.Center.X / 16;
                            int potentialSpotY = (int)target.Center.Y / 16;

                            // Become more open to positions to search the more failed tries that have been accumulated.
                            int checkAreaX = 30 + i / 50;
                            int checkAreaY = 20 + i / 75;

                            potentialSpotX += Main.rand.Next(-checkAreaX, checkAreaX + 1);
                            potentialSpotY += Main.rand.Next(-checkAreaY, checkAreaY + 1);

                            if (!WorldGen.SolidTile(potentialSpotX, potentialSpotY))
                            {
                                // Search downward until a solid tile is reached.
                                // Stop checking if the spot is below the world surface, to prevent potential infinite loops.
                                while (!WorldGen.SolidTile(potentialSpotX, potentialSpotY) && potentialSpotY < Main.worldSurface)
                                    potentialSpotY++;

                                // And ensure that the spot isn't too far away.
                                if (npc.WithinRange(new Vector2(potentialSpotX, potentialSpotY).ToWorldCoordinates(), 1600f))
                                {
                                    aiState = (int)MothronAIState.FlyToEggSpot;
                                    laySpotPositionX = potentialSpotX;
                                    laySpotPositionY = potentialSpotY;
                                    break;
                                }
                            }
                        }
                        npc.netUpdate = true;
                        npc.netSpam = 0;
                    }
                    break;

                case MothronAIState.FlyToEggSpot:
                    npc.spriteDirection = npc.direction = (npc.velocity.X > 0).ToDirectionInt();
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;
                    npc.noTileCollide = true;

                    Vector2 spotToLayEgg = new Vector2(npc.ai[2], npc.ai[3]).ToWorldCoordinates(8f, -20f);
                    float distanceFromSpot = npc.Distance(spotToLayEgg);
                    flySpeed = 12f + distanceFromSpot / 150f;

                    if (flySpeed > 20f)
                        flySpeed = 20f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromSpot < 10f)
                    {
                        aiState = (int)MothronAIState.LayEgg;
                        npc.netUpdate = true;
                    }

                    flyInertia = 10f;
                    npc.velocity = (npc.velocity * (flyInertia - 1f) + npc.SafeDirectionTo(spotToLayEgg, -Vector2.UnitY) * flySpeed) / flyInertia;
                    break;

                case MothronAIState.LayEgg:
                    npc.rotation = (npc.rotation * 9f + npc.velocity.X * 0.025f) / 10f;
                    npc.noTileCollide = false;

                    spotToLayEgg = new Vector2(npc.ai[2], npc.ai[3]).ToWorldCoordinates(8f, -28f);
                    distanceFromSpot = npc.Distance(spotToLayEgg);
                    float hoverSpeed = 4f;
                    float hoverInertia = 2f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && distanceFromSpot < 44f)
                    {
                        ref float attackTimer = ref npc.ai[1];
                        int eggLayTime = 20;
                        if (Main.expertMode)
                            eggLayTime = (int)(eggLayTime * 0.75);
                        int waitTime = eggLayTime;

                        attackTimer++;
                        if (attackTimer == eggLayTime)
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)spotToLayEgg.X, (int)spotToLayEgg.Y + 20, 478, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                        else if (attackTimer == eggLayTime + waitTime)
                        {
                            aiState = (int)MothronAIState.NewAISelection;
                            attackTimer = 0f;
                            npc.ai[2] = npc.ai[3] = 0f;

                            // Try to lay another egg at a 66% chance if the amount of eggs + spawns is not at the limit.
                            if (NPC.CountNPCS(NPCID.MothronEgg) + NPC.CountNPCS(NPCID.MothronSpawn) < 3 && Main.rand.Next(3) != 0)
                                aiState = (int)MothronAIState.PickSpotToLayEgg;
                            else if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                                aiState = (int)MothronAIState.FlyTowardsPlayer;
                            npc.netUpdate = true;
                        }
                    }

                    if (distanceFromSpot < hoverSpeed)
                        hoverSpeed = distanceFromSpot;

                    Vector2 hoverVelocity = npc.SafeDirectionTo(spotToLayEgg) * hoverSpeed;
                    npc.velocity = (npc.velocity * (hoverInertia - 1f) + hoverVelocity) / hoverInertia;
                    if (npc.velocity.HasNaNs())
                        npc.velocity = Vector2.Zero;
                    break;
            }
            return false;
        }
        #endregion

        #region Buffed Pumpking and Pumpking Blade AI
        public static bool BuffedPumpkingAI(NPC npc)
        {
            npc.localAI[0] += 1f;
            if (npc.localAI[0] > 6f)
            {
                npc.localAI[0] = 0f;
                npc.localAI[1] += 1f;

                if (npc.localAI[1] > 4f)
                    npc.localAI[1] = 0f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[2] += 1f;
                if (npc.localAI[2] > 300f)
                {
                    npc.ai[3] = (float)Main.rand.Next(3);
                    npc.localAI[2] = 0f;
                }
                else if (npc.ai[3] == 0f && npc.localAI[2] % 30f == 0f && npc.localAI[2] > 30f)
                {
                    float greekFireSpeed = 10f;
                    Vector2 greekFireSpawnPos = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                    if (!WorldGen.SolidTile((int)greekFireSpawnPos.X / 16, (int)greekFireSpawnPos.Y / 16))
                    {
                        float greekFireTargetX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - greekFireSpawnPos.X;
                        float greekFireTargetY = Main.player[npc.target].position.Y - greekFireSpawnPos.Y;
                        greekFireTargetX += (float)Main.rand.Next(-50, 51);
                        greekFireTargetY += (float)Main.rand.Next(50, 201);
                        greekFireTargetY *= 0.2f;
                        float greekFireTargetDist = (float)Math.Sqrt((double)(greekFireTargetX * greekFireTargetX + greekFireTargetY * greekFireTargetY));
                        greekFireTargetDist = greekFireSpeed / greekFireTargetDist;
                        greekFireTargetX *= greekFireTargetDist;
                        greekFireTargetY *= greekFireTargetDist;
                        greekFireTargetX *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
                        greekFireTargetY *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), greekFireSpawnPos.X, greekFireSpawnPos.Y, greekFireTargetX, greekFireTargetY, ProjectileID.GreekFire1 + Main.rand.Next(3), 60, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.TargetClosest(true);
                npc.ai[0] = 1f;

                int pumpkingBlades = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PumpkingBlade, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[pumpkingBlades].ai[0] = -1f;
                Main.npc[pumpkingBlades].ai[1] = (float)npc.whoAmI;
                Main.npc[pumpkingBlades].target = npc.target;
                Main.npc[pumpkingBlades].netUpdate = true;

                pumpkingBlades = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PumpkingBlade, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[pumpkingBlades].ai[0] = 1f;
                Main.npc[pumpkingBlades].ai[1] = (float)npc.whoAmI;
                Main.npc[pumpkingBlades].ai[3] = 150f;
                Main.npc[pumpkingBlades].target = npc.target;
                Main.npc[pumpkingBlades].netUpdate = true;
            }

            if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 2000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
            {
                npc.TargetClosest(true);

                if (Main.player[npc.target].dead || Math.Abs(npc.position.X - Main.player[npc.target].position.X) > 2000f || Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
                    npc.ai[1] = 2f;
            }

            if (Main.dayTime)
            {
                npc.velocity.Y += 0.3f;
                npc.velocity.X *= 0.9f;
            }
            else if (npc.ai[1] == 0f)
            {
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 300f)
                {
                    if (npc.ai[3] != 1f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else
                    {
                        npc.ai[2] = 0f;
                        npc.ai[1] = 1f;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                    }
                }

                Vector2 aggressivePosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float aggressiveTargetX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - aggressivePosition.X;
                float aggressiveTargetY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - aggressivePosition.Y;
                float aggressiveTargetDist = (float)Math.Sqrt((double)(aggressiveTargetX * aggressiveTargetX + aggressiveTargetY * aggressiveTargetY));
                float aggressiveSpeed = 8f;

                if (npc.ai[3] == 1f)
                {
                    if (aggressiveTargetDist > 900f)
                        aggressiveSpeed = 14f;
                    else if (aggressiveTargetDist > 600f)
                        aggressiveSpeed = 12f;
                    else if (aggressiveTargetDist > 300f)
                        aggressiveSpeed = 10f;
                }

                if (aggressiveTargetDist > 50f)
                {
                    aggressiveTargetDist = aggressiveSpeed / aggressiveTargetDist;
                    npc.velocity.X = (npc.velocity.X * 14f + aggressiveTargetX * aggressiveTargetDist) / 15f;
                    npc.velocity.Y = (npc.velocity.Y * 14f + aggressiveTargetY * aggressiveTargetDist) / 15f;
                }
            }
            else if (npc.ai[1] == 1f)
            {
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 600f || npc.ai[3] != 1f)
                {
                    npc.ai[2] = 0f;
                    npc.ai[1] = 0f;
                }

                Vector2 scytheAttackPosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float scytheAttackTargetX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - scytheAttackPosition.X;
                float scytheAttackTargetY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - scytheAttackPosition.Y;
                float scytheAttackTargetDist = (float)Math.Sqrt((double)(scytheAttackTargetX * scytheAttackTargetX + scytheAttackTargetY * scytheAttackTargetY));
                scytheAttackTargetDist = 20f / scytheAttackTargetDist;

                npc.velocity.X = (npc.velocity.X * 49f + scytheAttackTargetX * scytheAttackTargetDist) / 50f;
                npc.velocity.Y = (npc.velocity.Y * 49f + scytheAttackTargetY * scytheAttackTargetDist) / 50f;
            }
            else if (npc.ai[1] == 2f)
            {
                npc.ai[1] = 3f;
                npc.velocity.Y += 0.1f;

                if (npc.velocity.Y < 0f)
                    npc.velocity.Y *= 0.95f;

                npc.velocity.X *= 0.95f;

                if (npc.timeLeft > 500)
                    npc.timeLeft = 500;
            }
            npc.rotation = npc.velocity.X * -0.02f;

            return false;
        }

        public static bool BuffedPumpkingBladeAI(NPC npc)
        {
            npc.spriteDirection = -(int)npc.ai[0];

            if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != 58)
            {
                npc.ai[2] += 10f;
                if (npc.ai[2] > 50f || Main.netMode != NetmodeID.Server)
                {
                    npc.life = -1;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && Main.npc[(int)npc.ai[1]].ai[3] == 2f)
            {
                npc.localAI[1] += 1f;
                if (npc.localAI[1] > 30f)
                {
                    npc.localAI[1] = 0f;

                    float scytheProjSpeed = 0.01f;
                    Vector2 scytheProjSpawn = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                    float scytheProjTargetX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - scytheProjSpawn.X;
                    float scytheProjTargetY = Main.player[npc.target].position.Y - scytheProjSpawn.Y;
                    float scytheProjTargetDist = (float)Math.Sqrt((double)(scytheProjTargetX * scytheProjTargetX + scytheProjTargetY * scytheProjTargetY));

                    scytheProjTargetDist = scytheProjSpeed / scytheProjTargetDist;
                    scytheProjTargetX *= scytheProjTargetDist;
                    scytheProjTargetY *= scytheProjTargetDist;

                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, scytheProjTargetX, scytheProjTargetY, ProjectileID.FlamingScythe, 70, 0f, Main.myPlayer, npc.rotation, (float)npc.spriteDirection);
                }
            }

            if (Main.dayTime)
            {
                npc.velocity.Y += 0.3f;
                npc.velocity.X *= 0.9f;
                return false;
            }

            if (npc.ai[2] == 0f || npc.ai[2] == 3f)
            {
                if (Main.npc[(int)npc.ai[1]].ai[1] == 3f && npc.timeLeft > 10)
                    npc.timeLeft = 10;

                npc.ai[3] += 1f;
                if (npc.ai[3] >= 180f)
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                Vector2 scytheSwipePosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float scytheSwipeTargetX = (Main.player[npc.target].Center.X + Main.npc[(int)npc.ai[1]].Center.X) / 2f;
                float scytheSwipeTargetY = (Main.player[npc.target].Center.Y + Main.npc[(int)npc.ai[1]].Center.Y) / 2f;
                scytheSwipeTargetX += -170f * npc.ai[0] - scytheSwipePosition.X;
                scytheSwipeTargetY += 90f - scytheSwipePosition.Y;

                float scytheSwipeReelbackDist = Math.Abs(Main.player[npc.target].Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(Main.player[npc.target].Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);
                if (scytheSwipeReelbackDist > 700f)
                {
                    scytheSwipeTargetX = Main.npc[(int)npc.ai[1]].Center.X - 170f * npc.ai[0] - scytheSwipePosition.X;
                    scytheSwipeTargetY = Main.npc[(int)npc.ai[1]].Center.Y + 90f - scytheSwipePosition.Y;
                }

                float scytheSwipeTargetDist = (float)Math.Sqrt((double)(scytheSwipeTargetX * scytheSwipeTargetX + scytheSwipeTargetY * scytheSwipeTargetY));
                float scytheSwipeSpeed = 8f;
                if (scytheSwipeTargetDist > 1000f)
                    scytheSwipeSpeed = 23f;
                else if (scytheSwipeTargetDist > 800f)
                    scytheSwipeSpeed = 20f;
                else if (scytheSwipeTargetDist > 600f)
                    scytheSwipeSpeed = 17f;
                else if (scytheSwipeTargetDist > 400f)
                    scytheSwipeSpeed = 14f;
                else if (scytheSwipeTargetDist > 200f)
                    scytheSwipeSpeed = 11f;

                if (npc.ai[0] < 0f && npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
                    scytheSwipeTargetX -= 4f;
                if (npc.ai[0] > 0f && npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
                    scytheSwipeTargetX += 4f;

                scytheSwipeTargetDist = scytheSwipeSpeed / scytheSwipeTargetDist;
                npc.velocity.X = (npc.velocity.X * 14f + scytheSwipeTargetX * scytheSwipeTargetDist) / 15f;
                npc.velocity.Y = (npc.velocity.Y * 14f + scytheSwipeTargetY * scytheSwipeTargetDist) / 15f;
                scytheSwipeTargetDist = (float)Math.Sqrt((double)(scytheSwipeTargetX * scytheSwipeTargetX + scytheSwipeTargetY * scytheSwipeTargetY));

                if (scytheSwipeTargetDist > 20f)
                    npc.rotation = (float)Math.Atan2((double)scytheSwipeTargetY, (double)scytheSwipeTargetX) + MathHelper.PiOver2;
            }
            else if (npc.ai[2] == 1f)
            {
                Vector2 scytheReturnPosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float scytheReturnTargetX = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - scytheReturnPosition.X;
                float scytheReturnTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - scytheReturnPosition.Y;
                float scytheReturnTargetDist = (float)Math.Sqrt((double)(scytheReturnTargetX * scytheReturnTargetX + scytheReturnTargetY * scytheReturnTargetY));

                npc.rotation = (float)Math.Atan2((double)scytheReturnTargetY, (double)scytheReturnTargetX) + MathHelper.PiOver2;
                npc.velocity.X *= 0.95f;
                npc.velocity.Y -= 0.3f;

                if (npc.velocity.Y < -18f)
                    npc.velocity.Y = -18f;

                if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                {
                    npc.TargetClosest(true);
                    npc.ai[2] = 2f;

                    scytheReturnPosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    scytheReturnTargetX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - scytheReturnPosition.X;
                    scytheReturnTargetY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - scytheReturnPosition.Y;
                    scytheReturnTargetDist = (float)Math.Sqrt((double)(scytheReturnTargetX * scytheReturnTargetX + scytheReturnTargetY * scytheReturnTargetY));
                    scytheReturnTargetDist = 24f / scytheReturnTargetDist;

                    npc.velocity.X = scytheReturnTargetX * scytheReturnTargetDist;
                    npc.velocity.Y = scytheReturnTargetY * scytheReturnTargetDist;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 2f)
            {
                float scytheReturnDestination = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

                if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f || scytheReturnDestination > 800f)
                    npc.ai[2] = 3f;
            }
            else if (npc.ai[2] == 4f)
            {
                Vector2 scytheLesserSwipePos = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float scytheLesserSwipeTargetX = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - scytheLesserSwipePos.X;
                float scytheLesserSwipeTargetY = Main.npc[(int)npc.ai[1]].position.Y + 230f - scytheLesserSwipePos.Y;
                float scytheLesserSwipeTargetDist = (float)Math.Sqrt((double)(scytheLesserSwipeTargetX * scytheLesserSwipeTargetX + scytheLesserSwipeTargetY * scytheLesserSwipeTargetY));

                npc.rotation = (float)Math.Atan2((double)scytheLesserSwipeTargetY, (double)scytheLesserSwipeTargetX) + MathHelper.PiOver2;
                npc.velocity.Y *= 0.95f;
                npc.velocity.X += 0.3f * -npc.ai[0];

                if (npc.velocity.X < -18f)
                    npc.velocity.X = -18f;
                if (npc.velocity.X > 18f)
                    npc.velocity.X = 18f;

                if (npc.position.X + (float)(npc.width / 2) < Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 500f || npc.position.X + (float)(npc.width / 2) > Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) + 500f)
                {
                    npc.TargetClosest(true);
                    npc.ai[2] = 5f;

                    scytheLesserSwipePos = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    scytheLesserSwipeTargetX = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - scytheLesserSwipePos.X;
                    scytheLesserSwipeTargetY = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - scytheLesserSwipePos.Y;
                    scytheLesserSwipeTargetDist = (float)Math.Sqrt((double)(scytheLesserSwipeTargetX * scytheLesserSwipeTargetX + scytheLesserSwipeTargetY * scytheLesserSwipeTargetY));
                    scytheLesserSwipeTargetDist = 17f / scytheLesserSwipeTargetDist;

                    npc.velocity.X = scytheLesserSwipeTargetX * scytheLesserSwipeTargetDist;
                    npc.velocity.Y = scytheLesserSwipeTargetY * scytheLesserSwipeTargetDist;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 5f)
            {
                float scytheLesserSwipeReturnDest = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

                if ((npc.velocity.X > 0f && npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || scytheLesserSwipeReturnDest > 800f)
                    npc.ai[2] = 0f;
            }

            return false;
        }
        #endregion

        #region Buffed Ice Queen AI
        public static bool BuffedIceQueenAI(NPC npc)
        {
            if (Main.dayTime)
            {
                if (npc.velocity.X > 0f)
                    npc.velocity.X += 0.25f;
                else
                    npc.velocity.X -= 0.25f;

                npc.velocity.Y -= 0.1f;
                npc.rotation = npc.velocity.X * 0.05f;
            }
            else if (npc.ai[0] == 0f)
            {
                if (npc.ai[2] == 0f)
                {
                    npc.TargetClosest(true);

                    if (npc.Center.X < Main.player[npc.target].Center.X)
                        npc.ai[2] = 1f;
                    else
                        npc.ai[2] = -1f;
                }

                npc.TargetClosest(true);
                float iceQueenTargetDist = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);

                if (npc.Center.X < Main.player[npc.target].Center.X && npc.ai[2] < 0f && iceQueenTargetDist > 800f)
                    npc.ai[2] = 0f;
                if (npc.Center.X > Main.player[npc.target].Center.X && npc.ai[2] > 0f && iceQueenTargetDist > 800f)
                    npc.ai[2] = 0f;

                float iceQueenAcceleration = 0.6f;
                float iceQueenMaxVelocity = 10f;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                {
                    iceQueenAcceleration = 0.7f;
                    iceQueenMaxVelocity = 12f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                {
                    iceQueenAcceleration = 0.8f;
                    iceQueenMaxVelocity = 14f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                {
                    iceQueenAcceleration = 0.95f;
                    iceQueenMaxVelocity = 16f;
                }

                npc.velocity.X += npc.ai[2] * iceQueenAcceleration;
                if (npc.velocity.X > iceQueenMaxVelocity)
                    npc.velocity.X = iceQueenMaxVelocity;
                if (npc.velocity.X < -iceQueenMaxVelocity)
                    npc.velocity.X = -iceQueenMaxVelocity;

                float iceQueenHoverHeight = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
                if (iceQueenHoverHeight < 150f)
                    npc.velocity.Y -= 0.2f;
                if (iceQueenHoverHeight > 200f)
                    npc.velocity.Y += 0.2f;
                if (npc.velocity.Y > 9f)
                    npc.velocity.Y = 9f;
                if (npc.velocity.Y < -9f)
                    npc.velocity.Y = -9f;

                npc.rotation = npc.velocity.X * 0.05f;

                if ((iceQueenTargetDist < 500f || npc.ai[3] < 0f) && npc.position.Y < Main.player[npc.target].position.Y)
                {
                    npc.ai[3] += 1f;
                    int frostWaveFireDelay = 8;
                    if ((double)npc.life < (double)npc.lifeMax * 0.75)
                        frostWaveFireDelay = 7;
                    if ((double)npc.life < (double)npc.lifeMax * 0.5)
                        frostWaveFireDelay = 6;
                    if ((double)npc.life < (double)npc.lifeMax * 0.25)
                        frostWaveFireDelay = 5;

                    frostWaveFireDelay++;
                    if (npc.ai[3] > (float)frostWaveFireDelay)
                        npc.ai[3] = (float)-(float)frostWaveFireDelay;

                    if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 frostWavePosition = new Vector2(npc.Center.X, npc.Center.Y);
                        frostWavePosition.X += npc.velocity.X * 7f;
                        float frostWaveTargetX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - frostWavePosition.X;
                        float frostWaveTargetY = Main.player[npc.target].Center.Y - frostWavePosition.Y;
                        float frostWaveTargetDist = (float)Math.Sqrt((double)(frostWaveTargetX * frostWaveTargetX + frostWaveTargetY * frostWaveTargetY));

                        float frostWaveSpeed = 8f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.75)
                            frostWaveSpeed = 9f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.5)
                            frostWaveSpeed = 10f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.25)
                            frostWaveSpeed = 11f;

                        frostWaveTargetDist = frostWaveSpeed / frostWaveTargetDist;
                        frostWaveTargetX *= frostWaveTargetDist;
                        frostWaveTargetY *= frostWaveTargetDist;

                        Projectile.NewProjectile(npc.GetSource_FromAI(), frostWavePosition.X, frostWavePosition.Y, frostWaveTargetX, frostWaveTargetY, ProjectileID.FrostWave, 50, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                else if (npc.ai[3] < 0f)
                    npc.ai[3] += 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] += (float)Main.rand.Next(1, 4);

                    if (npc.ai[1] > 600f && iceQueenTargetDist < 600f)
                        npc.ai[0] = -1f;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.TargetClosest(true);

                float icicleAttackAcceleration = 0.2f;
                float icicleAttackMaxVelocity = 10f;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                {
                    icicleAttackAcceleration = 0.24f;
                    icicleAttackMaxVelocity = 12f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                {
                    icicleAttackAcceleration = 0.28f;
                    icicleAttackMaxVelocity = 14f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                {
                    icicleAttackAcceleration = 0.32f;
                    icicleAttackMaxVelocity = 16f;
                }
                icicleAttackAcceleration -= 0.05f;
                icicleAttackMaxVelocity -= 1f;

                if (npc.Center.X < Main.player[npc.target].Center.X)
                {
                    npc.velocity.X += icicleAttackAcceleration;
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                }
                if (npc.Center.X > Main.player[npc.target].Center.X)
                {
                    npc.velocity.X -= icicleAttackAcceleration;
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                }
                if (npc.velocity.X > icicleAttackMaxVelocity || npc.velocity.X < -icicleAttackMaxVelocity)
                    npc.velocity.X *= 0.95f;

                float icicleAttackHoverHeight = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
                if (icicleAttackHoverHeight < 180f)
                    npc.velocity.Y -= 0.1f;
                if (icicleAttackHoverHeight > 200f)
                    npc.velocity.Y += 0.1f;

                if (npc.velocity.Y > 7f)
                    npc.velocity.Y = 7f;
                if (npc.velocity.Y < -7f)
                    npc.velocity.Y = -7f;

                npc.rotation = npc.velocity.X * 0.01f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[3] += 1f;
                    int icicleFireDelay = 10;
                    if ((double)npc.life < (double)npc.lifeMax * 0.75)
                        icicleFireDelay = 8;
                    if ((double)npc.life < (double)npc.lifeMax * 0.5)
                        icicleFireDelay = 6;
                    if ((double)npc.life < (double)npc.lifeMax * 0.25)
                        icicleFireDelay = 4;
                    if ((double)npc.life < (double)npc.lifeMax * 0.1)
                        icicleFireDelay = 2;

                    icicleFireDelay += 3;
                    if (npc.ai[3] >= (float)icicleFireDelay)
                    {
                        npc.ai[3] = 0f;
                        Vector2 icicleSpawnPos = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
                        int i2 = (int)(icicleSpawnPos.X / 16f);
                        int j2 = (int)(icicleSpawnPos.Y / 16f);
                        if (!WorldGen.SolidTile(i2, j2))
                        {
                            float icicleFallSpeed = npc.velocity.Y;

                            if (icicleFallSpeed < 0f)
                                icicleFallSpeed = 0f;

                            icicleFallSpeed += 3f;
                            float speedX2 = npc.velocity.X * 0.25f;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), icicleSpawnPos.X, icicleSpawnPos.Y, speedX2, icicleFallSpeed, ProjectileID.FrostShard, 44, 0f, Main.myPlayer, (float)Main.rand.Next(5), 0f);
                        }
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] += (float)Main.rand.Next(1, 4);

                    if (npc.ai[1] > 450f)
                        npc.ai[0] = -1f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.TargetClosest(true);

                Vector2 iceRainPosition = new Vector2(npc.Center.X, npc.Center.Y - 20f);
                float iceRainXVel = (float)Main.rand.Next(-1000, 1001);
                float iceRainYVel = (float)Main.rand.Next(-1000, 1001);
                float iceRainVelocity = (float)Math.Sqrt((double)(iceRainXVel * iceRainXVel + iceRainYVel * iceRainYVel));
                float iceRainSpeed = 20f;

                npc.velocity *= 0.95f;
                iceRainVelocity = iceRainSpeed / iceRainVelocity;
                iceRainXVel *= iceRainVelocity;
                iceRainYVel *= iceRainVelocity;
                npc.rotation += 0.2f;
                iceRainPosition.X += iceRainXVel * 4f;
                iceRainPosition.Y += iceRainYVel * 4f;

                npc.ai[3] += 1f;
                int iceRainFireDelay = 7;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                    iceRainFireDelay--;
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                    iceRainFireDelay -= 2;
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                    iceRainFireDelay -= 3;
                if ((double)npc.life < (double)npc.lifeMax * 0.1)
                    iceRainFireDelay -= 4;

                if (npc.ai[3] > (float)iceRainFireDelay)
                {
                    npc.ai[3] = 0f;
                    int iceRainAttack = Projectile.NewProjectile(npc.GetSource_FromAI(), iceRainPosition.X, iceRainPosition.Y, iceRainXVel, iceRainYVel, ProjectileID.FrostShard, 40, 0f, Main.myPlayer, 0f, 0f);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] += (float)Main.rand.Next(1, 4);

                    if (npc.ai[1] > 300f)
                        npc.ai[0] = -1f;
                }
            }
            if (npc.ai[0] == -1f)
            {
                int attackPicker = Main.rand.Next(3);
                npc.TargetClosest(true);

                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 1000f)
                    attackPicker = 0;

                npc.ai[0] = (float)attackPicker;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
            }

            return false;
        }
        #endregion
    }
}
