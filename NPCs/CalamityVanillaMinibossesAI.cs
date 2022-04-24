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
                    float num856 = 10f;
                    Vector2 vector109 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                    if (!WorldGen.SolidTile((int)vector109.X / 16, (int)vector109.Y / 16))
                    {
                        float num857 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector109.X;
                        float num858 = Main.player[npc.target].position.Y - vector109.Y;
                        num857 += (float)Main.rand.Next(-50, 51);
                        num858 += (float)Main.rand.Next(50, 201);
                        num858 *= 0.2f;
                        float num859 = (float)Math.Sqrt((double)(num857 * num857 + num858 * num858));
                        num859 = num856 / num859;
                        num857 *= num859;
                        num858 *= num859;
                        num857 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
                        num858 *= 1f + (float)Main.rand.Next(-30, 31) * 0.01f;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector109.X, vector109.Y, num857, num858, ProjectileID.GreekFire1 + Main.rand.Next(3), 60, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (npc.ai[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.TargetClosest(true);
                npc.ai[0] = 1f;

                int num861 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PumpkingBlade, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[num861].ai[0] = -1f;
                Main.npc[num861].ai[1] = (float)npc.whoAmI;
                Main.npc[num861].target = npc.target;
                Main.npc[num861].netUpdate = true;

                num861 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)npc.position.Y + npc.height / 2, NPCID.PumpkingBlade, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                Main.npc[num861].ai[0] = 1f;
                Main.npc[num861].ai[1] = (float)npc.whoAmI;
                Main.npc[num861].ai[3] = 150f;
                Main.npc[num861].target = npc.target;
                Main.npc[num861].netUpdate = true;
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

                Vector2 vector110 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num862 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector110.X;
                float num863 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - 200f - vector110.Y;
                float num864 = (float)Math.Sqrt((double)(num862 * num862 + num863 * num863));
                float num865 = 8f;

                if (npc.ai[3] == 1f)
                {
                    if (num864 > 900f)
                        num865 = 14f;
                    else if (num864 > 600f)
                        num865 = 12f;
                    else if (num864 > 300f)
                        num865 = 10f;
                }

                if (num864 > 50f)
                {
                    num864 = num865 / num864;
                    npc.velocity.X = (npc.velocity.X * 14f + num862 * num864) / 15f;
                    npc.velocity.Y = (npc.velocity.Y * 14f + num863 * num864) / 15f;
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

                Vector2 vector111 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num866 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector111.X;
                float num867 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector111.Y;
                float num868 = (float)Math.Sqrt((double)(num866 * num866 + num867 * num867));
                num868 = 20f / num868;

                npc.velocity.X = (npc.velocity.X * 49f + num866 * num868) / 50f;
                npc.velocity.Y = (npc.velocity.Y * 49f + num867 * num868) / 50f;
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

                    float num869 = 0.01f;
                    Vector2 vector112 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                    float num870 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector112.X;
                    float num871 = Main.player[npc.target].position.Y - vector112.Y;
                    float num872 = (float)Math.Sqrt((double)(num870 * num870 + num871 * num871));

                    num872 = num869 / num872;
                    num870 *= num872;
                    num871 *= num872;

                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, num870, num871, ProjectileID.FlamingScythe, 70, 0f, Main.myPlayer, npc.rotation, (float)npc.spriteDirection);
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

                Vector2 vector113 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num874 = (Main.player[npc.target].Center.X + Main.npc[(int)npc.ai[1]].Center.X) / 2f;
                float num875 = (Main.player[npc.target].Center.Y + Main.npc[(int)npc.ai[1]].Center.Y) / 2f;
                num874 += -170f * npc.ai[0] - vector113.X;
                num875 += 90f - vector113.Y;

                float num876 = Math.Abs(Main.player[npc.target].Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(Main.player[npc.target].Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);
                if (num876 > 700f)
                {
                    num874 = Main.npc[(int)npc.ai[1]].Center.X - 170f * npc.ai[0] - vector113.X;
                    num875 = Main.npc[(int)npc.ai[1]].Center.Y + 90f - vector113.Y;
                }

                float num877 = (float)Math.Sqrt((double)(num874 * num874 + num875 * num875));
                float num878 = 8f;
                if (num877 > 1000f)
                    num878 = 23f;
                else if (num877 > 800f)
                    num878 = 20f;
                else if (num877 > 600f)
                    num878 = 17f;
                else if (num877 > 400f)
                    num878 = 14f;
                else if (num877 > 200f)
                    num878 = 11f;

                if (npc.ai[0] < 0f && npc.Center.X > Main.npc[(int)npc.ai[1]].Center.X)
                    num874 -= 4f;
                if (npc.ai[0] > 0f && npc.Center.X < Main.npc[(int)npc.ai[1]].Center.X)
                    num874 += 4f;

                num877 = num878 / num877;
                npc.velocity.X = (npc.velocity.X * 14f + num874 * num877) / 15f;
                npc.velocity.Y = (npc.velocity.Y * 14f + num875 * num877) / 15f;
                num877 = (float)Math.Sqrt((double)(num874 * num874 + num875 * num875));

                if (num877 > 20f)
                    npc.rotation = (float)Math.Atan2((double)num875, (double)num874) + MathHelper.PiOver2;
            }
            else if (npc.ai[2] == 1f)
            {
                Vector2 vector114 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num879 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector114.X;
                float num880 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector114.Y;
                float num881 = (float)Math.Sqrt((double)(num879 * num879 + num880 * num880));

                npc.rotation = (float)Math.Atan2((double)num880, (double)num879) + MathHelper.PiOver2;
                npc.velocity.X *= 0.95f;
                npc.velocity.Y -= 0.3f;

                if (npc.velocity.Y < -18f)
                    npc.velocity.Y = -18f;

                if (npc.position.Y < Main.npc[(int)npc.ai[1]].position.Y - 200f)
                {
                    npc.TargetClosest(true);
                    npc.ai[2] = 2f;

                    vector114 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num879 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector114.X;
                    num880 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector114.Y;
                    num881 = (float)Math.Sqrt((double)(num879 * num879 + num880 * num880));
                    num881 = 24f / num881;

                    npc.velocity.X = num879 * num881;
                    npc.velocity.Y = num880 * num881;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 2f)
            {
                float num882 = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

                if (npc.position.Y > Main.player[npc.target].position.Y || npc.velocity.Y < 0f || num882 > 800f)
                    npc.ai[2] = 3f;
            }
            else if (npc.ai[2] == 4f)
            {
                Vector2 vector115 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num883 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - 200f * npc.ai[0] - vector115.X;
                float num884 = Main.npc[(int)npc.ai[1]].position.Y + 230f - vector115.Y;
                float num885 = (float)Math.Sqrt((double)(num883 * num883 + num884 * num884));

                npc.rotation = (float)Math.Atan2((double)num884, (double)num883) + MathHelper.PiOver2;
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

                    vector115 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num883 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector115.X;
                    num884 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector115.Y;
                    num885 = (float)Math.Sqrt((double)(num883 * num883 + num884 * num884));
                    num885 = 17f / num885;

                    npc.velocity.X = num883 * num885;
                    npc.velocity.Y = num884 * num885;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[2] == 5f)
            {
                float num886 = Math.Abs(npc.Center.X - Main.npc[(int)npc.ai[1]].Center.X) + Math.Abs(npc.Center.Y - Main.npc[(int)npc.ai[1]].Center.Y);

                if ((npc.velocity.X > 0f && npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || (npc.velocity.X < 0f && npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)) || num886 > 800f)
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
                int num887 = 800;
                float num888 = Math.Abs(npc.Center.X - Main.player[npc.target].Center.X);

                if (npc.Center.X < Main.player[npc.target].Center.X && npc.ai[2] < 0f && num888 > (float)num887)
                    npc.ai[2] = 0f;
                if (npc.Center.X > Main.player[npc.target].Center.X && npc.ai[2] > 0f && num888 > (float)num887)
                    npc.ai[2] = 0f;

                float num889 = 0.6f;
                float num890 = 10f;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                {
                    num889 = 0.7f;
                    num890 = 12f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                {
                    num889 = 0.8f;
                    num890 = 14f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                {
                    num889 = 0.95f;
                    num890 = 16f;
                }

                npc.velocity.X += npc.ai[2] * num889;
                if (npc.velocity.X > num890)
                    npc.velocity.X = num890;
                if (npc.velocity.X < -num890)
                    npc.velocity.X = -num890;

                float num891 = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
                if (num891 < 150f)
                    npc.velocity.Y -= 0.2f;
                if (num891 > 200f)
                    npc.velocity.Y += 0.2f;
                if (npc.velocity.Y > 9f)
                    npc.velocity.Y = 9f;
                if (npc.velocity.Y < -9f)
                    npc.velocity.Y = -9f;

                npc.rotation = npc.velocity.X * 0.05f;

                if ((num888 < 500f || npc.ai[3] < 0f) && npc.position.Y < Main.player[npc.target].position.Y)
                {
                    npc.ai[3] += 1f;
                    int num892 = 8;
                    if ((double)npc.life < (double)npc.lifeMax * 0.75)
                        num892 = 7;
                    if ((double)npc.life < (double)npc.lifeMax * 0.5)
                        num892 = 6;
                    if ((double)npc.life < (double)npc.lifeMax * 0.25)
                        num892 = 5;

                    num892++;
                    if (npc.ai[3] > (float)num892)
                        npc.ai[3] = (float)-(float)num892;

                    if (npc.ai[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector116 = new Vector2(npc.Center.X, npc.Center.Y);
                        vector116.X += npc.velocity.X * 7f;
                        float num893 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector116.X;
                        float num894 = Main.player[npc.target].Center.Y - vector116.Y;
                        float num895 = (float)Math.Sqrt((double)(num893 * num893 + num894 * num894));

                        float num896 = 8f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.75)
                            num896 = 9f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.5)
                            num896 = 10f;
                        if ((double)npc.life < (double)npc.lifeMax * 0.25)
                            num896 = 11f;

                        num895 = num896 / num895;
                        num893 *= num895;
                        num894 *= num895;

                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector116.X, vector116.Y, num893, num894, ProjectileID.FrostWave, 50, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                else if (npc.ai[3] < 0f)
                    npc.ai[3] += 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] += (float)Main.rand.Next(1, 4);

                    if (npc.ai[1] > 600f && num888 < 600f)
                        npc.ai[0] = -1f;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.TargetClosest(true);

                float num898 = 0.2f;
                float num899 = 10f;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                {
                    num898 = 0.24f;
                    num899 = 12f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                {
                    num898 = 0.28f;
                    num899 = 14f;
                }
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                {
                    num898 = 0.32f;
                    num899 = 16f;
                }
                num898 -= 0.05f;
                num899 -= 1f;

                if (npc.Center.X < Main.player[npc.target].Center.X)
                {
                    npc.velocity.X += num898;
                    if (npc.velocity.X < 0f)
                        npc.velocity.X *= 0.98f;
                }
                if (npc.Center.X > Main.player[npc.target].Center.X)
                {
                    npc.velocity.X -= num898;
                    if (npc.velocity.X > 0f)
                        npc.velocity.X *= 0.98f;
                }
                if (npc.velocity.X > num899 || npc.velocity.X < -num899)
                    npc.velocity.X *= 0.95f;

                float num900 = Main.player[npc.target].position.Y - (npc.position.Y + (float)npc.height);
                if (num900 < 180f)
                    npc.velocity.Y -= 0.1f;
                if (num900 > 200f)
                    npc.velocity.Y += 0.1f;

                if (npc.velocity.Y > 7f)
                    npc.velocity.Y = 7f;
                if (npc.velocity.Y < -7f)
                    npc.velocity.Y = -7f;

                npc.rotation = npc.velocity.X * 0.01f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[3] += 1f;
                    int num901 = 10;
                    if ((double)npc.life < (double)npc.lifeMax * 0.75)
                        num901 = 8;
                    if ((double)npc.life < (double)npc.lifeMax * 0.5)
                        num901 = 6;
                    if ((double)npc.life < (double)npc.lifeMax * 0.25)
                        num901 = 4;
                    if ((double)npc.life < (double)npc.lifeMax * 0.1)
                        num901 = 2;

                    num901 += 3;
                    if (npc.ai[3] >= (float)num901)
                    {
                        npc.ai[3] = 0f;
                        Vector2 vector117 = new Vector2(npc.Center.X, npc.position.Y + (float)npc.height - 14f);
                        int i2 = (int)(vector117.X / 16f);
                        int j2 = (int)(vector117.Y / 16f);
                        if (!WorldGen.SolidTile(i2, j2))
                        {
                            float num902 = npc.velocity.Y;

                            if (num902 < 0f)
                                num902 = 0f;

                            num902 += 3f;
                            float speedX2 = npc.velocity.X * 0.25f;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector117.X, vector117.Y, speedX2, num902, ProjectileID.FrostShard, 44, 0f, Main.myPlayer, (float)Main.rand.Next(5), 0f);
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

                Vector2 vector118 = new Vector2(npc.Center.X, npc.Center.Y - 20f);
                float num904 = (float)Main.rand.Next(-1000, 1001);
                float num905 = (float)Main.rand.Next(-1000, 1001);
                float num906 = (float)Math.Sqrt((double)(num904 * num904 + num905 * num905));
                float num907 = 20f;

                npc.velocity *= 0.95f;
                num906 = num907 / num906;
                num904 *= num906;
                num905 *= num906;
                npc.rotation += 0.2f;
                vector118.X += num904 * 4f;
                vector118.Y += num905 * 4f;

                npc.ai[3] += 1f;
                int num908 = 7;
                if ((double)npc.life < (double)npc.lifeMax * 0.75)
                    num908--;
                if ((double)npc.life < (double)npc.lifeMax * 0.5)
                    num908 -= 2;
                if ((double)npc.life < (double)npc.lifeMax * 0.25)
                    num908 -= 3;
                if ((double)npc.life < (double)npc.lifeMax * 0.1)
                    num908 -= 4;

                if (npc.ai[3] > (float)num908)
                {
                    npc.ai[3] = 0f;
                    int num909 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector118.X, vector118.Y, num904, num905, ProjectileID.FrostShard, 40, 0f, Main.myPlayer, 0f, 0f);
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
                int num910 = Main.rand.Next(3);
                npc.TargetClosest(true);

                if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 1000f)
                    num910 = 0;

                npc.ai[0] = (float)num910;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
            }

            return false;
        }
        #endregion
    }
}
