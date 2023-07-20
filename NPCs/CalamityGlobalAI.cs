using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.VanillaNPCOverrides.RegularEnemies;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs
{
    public partial class CalamityGlobalAI
    {
        #region Death Mode NPC AI

        #region Fighter AI

        // Methods for AIs that are always terminated via a return in the Fighter AI code. More specialized and general
        // functions are given summaries of their specifics
        public static void BuffedPsychoAI(NPC npc)
        {
            int psychoAlphaMax = 200;
            // Standing still
            if (npc.ai[2] == 0f)
            {
                npc.alpha = psychoAlphaMax;
                npc.TargetClosest(true);
                if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f)
                {
                    npc.ai[2] = -16f;
                }
                if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 2f || npc.justHit)
                {
                    npc.ai[2] = -16f;
                }
            }
            // Active
            if (npc.ai[2] < 0f)
            {
                if (npc.alpha > 0)
                {
                    npc.alpha -= psychoAlphaMax / 16;
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
                    npc.ai[2] = 1f;
                    npc.velocity.X = (float)(npc.direction * 2);
                }
            }
        }
        public static void BuffedSwampThingAI(NPC npc)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(240))
            {
                npc.ai[2] = (float)Main.rand.Next(-480, -60);
                npc.netUpdate = true;
            }
            if (npc.ai[2] < 0f)
            {
                npc.TargetClosest(true);
                if (npc.justHit)
                {
                    npc.ai[2] = 0f;
                }
                if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[2] = 0f;
                }
            }
            if (npc.ai[2] < 0f)
            {
                npc.velocity.X *= 0.9f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                {
                    npc.velocity.X = 0f;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] == 0f)
                {
                    npc.velocity.X = (float)npc.direction * 0.1f;
                }
            }
        }
        // Misc effects used more than once in the Fighter AI
        public static void MedusaHeadDustEffect(NPC npc, float time)
        {
            Vector2 headPosition = npc.Top + new Vector2((float)(npc.spriteDirection * 6), 6f);
            float rotationVectorMult = MathHelper.Lerp(20f, 30f, (time * 3f + 50f) / 182f);
            Main.rand.NextFloat();
            for (float num17 = 0f; num17 < 2f; num17 += 1f)
            {
                Vector2 rotationVector = Vector2.UnitY.RotatedByRandom(Math.PI * 2.0) * (Main.rand.NextFloat() * 0.5f + 0.5f);
                Dust dust = Dust.NewDustDirect(headPosition, 0, 0, 228, 0f, 0f, 0, default, 1f);
                dust.position = headPosition + rotationVector * rotationVectorMult;
                dust.noGravity = true;
                dust.velocity = rotationVector * 2f;
                dust.scale = 0.5f + Main.rand.NextFloat() * 0.5f;
            }
        }

        /// <summary>
        /// Causes an NPC to run on the X axis until it hits the maximum speed and decclerates as needed.
        /// Works best with fighter AI based NPCs.
        /// </summary>
        /// <param name="npc">The NPC to manipulate</param>
        /// <param name="velocityMax">The max speed to run at.</param>
        /// <param name="acceleration">The rate at which the X velocity changes with time.</param>
        /// <param name="turnDeceleration">The X velocity deceleration multiplier when the NPC will turn to another direction.</param>
        /// <param name="extraDeceleration">Causes the NPC to slow down faster when not jumping for direction turning if true.</param>
        /// <param name="turnDeceleration">The X velocity deceleration multiplier when the NPC will turn to another direction.</param>
        /// <param name="extraDecelerationFactor">The X velocity deceleration multiplier used by <paramref name="turnDeceleration"/>.</param>
        public static void FighterRunningAI(NPC npc, float velocityMax, float acceleration, float turnDeceleration,
            bool extraDeceleration = false, float extraDecelerationFactor = 0.99f)
        {
            if (npc.velocity.X < -velocityMax || npc.velocity.X > velocityMax)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity *= turnDeceleration;
                }
            }
            else if (npc.velocity.X < velocityMax && npc.direction == 1)
            {
                npc.velocity.X += acceleration;

                if (extraDeceleration && npc.velocity.Y == 0f && npc.velocity.X < 0f)
                    npc.velocity.X *= extraDecelerationFactor;

                if (npc.velocity.X > velocityMax)
                {
                    npc.velocity.X = velocityMax;
                }
            }
            else if (npc.velocity.X > -velocityMax && npc.direction == -1)
            {
                if (extraDeceleration && npc.velocity.Y == 0f && npc.velocity.X > 0f)
                    npc.velocity.X *= extraDecelerationFactor;
                npc.velocity.X -= acceleration;
                if (npc.velocity.X < -velocityMax)
                {
                    npc.velocity.X = -velocityMax;
                }
            }
        }

        public static void TryConvertToWallClimber(NPC npc)
        {
            // Note: The Possessed rely on an AI shift rather than a transformation NPC
            // As a result, they are not included in this method

            List<int> spiders = new List<int>()
            {
                NPCID.BlackRecluse,
                NPCID.BloodCrawler,
                NPCID.DesertScorpionWalk,
                NPCID.JungleCreeper,
                NPCID.WallCreeper,
                ModContent.NPCType<AstralachneaGround>()
            };
            // These checks are not required if the npc is not a real spider
            if (!spiders.Contains(npc.type))
                return;
            int tileCoordsX = (int)npc.Center.X / 16;
            int tileCoordsY = (int)npc.Center.Y / 16;
            bool climbWalls = false;
            for (int x = tileCoordsX - 1; x <= tileCoordsX + 1; x++)
            {
                for (int y = tileCoordsY - 1; y <= tileCoordsY + 1; y++)
                {
                    if (Main.tile[x, y].WallType > 0)
                    {
                        climbWalls = true;
                    }
                }
            }
            int transformType = -1;
            if (climbWalls)
            {
                if (npc.type == ModContent.NPCType<AstralachneaGround>())
                {
                    transformType = ModContent.NPCType<AstralachneaWall>();
                }
                else
                {
                    switch (npc.type)
                    {
                        case NPCID.BlackRecluse:
                            transformType = NPCID.BlackRecluseWall;
                            break;
                        case NPCID.BloodCrawler:
                            transformType = NPCID.BloodCrawlerWall;
                            break;
                        case NPCID.DesertScorpionWalk:
                            transformType = NPCID.DesertScorpionWall;
                            break;
                        case NPCID.JungleCreeper:
                            transformType = NPCID.JungleCreeperWall;
                            break;
                        case NPCID.WallCreeper:
                            transformType = NPCID.WallCreeperWall;
                            break;
                    }
                }
                if (transformType != -1)
                {
                    npc.Transform(transformType);
                }
            }
        }

        public static bool BuffedFighterAI(NPC npc, Mod mod)
        {
            int npcType = npc.type;
            if (npc.ModNPC != null)
            {
                if (npc.ModNPC.AIType != 0)
                    npcType = npc.ModNPC.AIType;
            }

            if (npcType == NPCID.Psycho)
            {
                BuffedPsychoAI(npc);
            }

            if (npcType == NPCID.SwampThing)
            {
                BuffedSwampThingAI(npc);
            }

            if (npcType == NPCID.CreatureFromTheDeep)
            {
                // Swimming
                if (npc.wet)
                {
                    npc.knockBackResist = 0f;
                    npc.ai[3] = -0.10101f;
                    npc.noGravity = true;
                    npc.width = 34;
                    npc.height = 24;
                    npc.position = npc.Center - npc.Size / 2f;
                    npc.TargetClosest(true);
                    if (npc.collideX)
                    {
                        npc.velocity.X = -npc.oldVelocity.X;
                    }
                    if (npc.velocity.X < 0f)
                    {
                        npc.direction = -1;
                    }
                    if (npc.velocity.X > 0f)
                    {
                        npc.direction = 1;
                    }

                    // If there's nothing in the way of the player, swim towards them
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.velocity = (npc.velocity * 19f + npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * 10f) / 20f;
                        return false;
                    }

                    float velocityMultiplier = 10f;
                    if (npc.velocity.Y > 0f)
                    {
                        velocityMultiplier = 6f;
                    }
                    if (npc.velocity.Y < 0f)
                    {
                        velocityMultiplier = 16f;
                    }
                    Vector2 directionVectorNormalized = new Vector2((float)npc.direction, -1f);
                    directionVectorNormalized.Normalize();
                    directionVectorNormalized *= velocityMultiplier;

                    // If the speed is low, make the turn speed higher
                    if (velocityMultiplier < 5f)
                    {
                        npc.velocity = (npc.velocity * 24f + directionVectorNormalized) / 25f;
                        return false;
                    }
                    npc.velocity = (npc.velocity * 9f + directionVectorNormalized) / 10f;
                    return false;
                }
                else
                {
                    npc.knockBackResist = CalamityWorld.death ? 0.1f : 0.2f;
                    npc.noGravity = false;
                    npc.width = 18;
                    npc.height = 40;
                    npc.position = npc.Center - npc.Size / 2f;

                    // If was just swimming, set values to return to land
                    if (npc.ai[3] == -0.10101f)
                    {
                        npc.ai[3] = 0f;
                        // Adjust velocity from the one the NPC had when swimming
                        float velocityMagnitude = npc.velocity.Length();
                        velocityMagnitude *= 2f;
                        if (velocityMagnitude > 12f)
                        {
                            velocityMagnitude = 12f;
                        }
                        npc.velocity.Normalize();
                        npc.velocity *= velocityMagnitude;
                        npc.direction = npc.spriteDirection = (npc.velocity.X > 0).ToDirectionInt();
                    }
                }
            }

            if (npcType == NPCID.CultistArcherBlue || npcType == NPCID.CultistArcherWhite)
            {
                // Pissed off
                if (npc.ai[3] < 0f)
                {
                    npc.damage = 0;
                    npc.velocity.X *= 0.93f;
                    if (npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                    {
                        npc.velocity.X = 0f;
                    }
                    int targetNPC = (int)(-npc.ai[3] - 1f);
                    int directionToTarget = Math.Sign(Main.npc[targetNPC].Center.X - npc.Center.X);
                    if (directionToTarget != npc.direction)
                    {
                        npc.velocity.X = 0f;
                        npc.direction = directionToTarget;
                        npc.netUpdate = true;
                    }
                    if (npc.justHit && Main.netMode != NetmodeID.MultiplayerClient && Main.npc[targetNPC].localAI[0] == 0f)
                    {
                        Main.npc[targetNPC].localAI[0] = 1f;
                    }
                    if (npc.ai[0] < 1000f)
                    {
                        npc.ai[0] = 1000f;
                    }
                    npc.ai[0] += 1f;
                    if (npc.ai[0] >= 1300f)
                    {
                        npc.ai[0] = 1000f;
                        npc.netUpdate = true;
                    }
                    return false;
                }
                // Not pissed off
                if (npc.ai[0] >= 1000f)
                {
                    npc.ai[0] = 0f;
                }
                npc.damage = npc.defDamage;
            }

            if (npcType == NPCID.MartianOfficer && npc.ai[2] == 0f && npc.localAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                // If Martian Officer is ready to generate shield, generate it.
                int shield = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.ForceBubble, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                npc.ai[2] = (float)(shield + 1);
                npc.localAI[0] = -1f;
                npc.netUpdate = true;
                Main.npc[shield].ai[0] = (float)npc.whoAmI;
                Main.npc[shield].netUpdate = true;
            }

            if (npcType == NPCID.MartianOfficer)
            {
                int shield = (int)npc.ai[2] - 1;
                if (shield != -1 && Main.npc[shield].active && Main.npc[shield].type == NPCID.ForceBubble)
                {
                    npc.dontTakeDamage = true;
                }
                else
                {
                    npc.dontTakeDamage = false;
                    npc.ai[2] = 0f;
                    if (npc.localAI[0] == -1f)
                    {
                        npc.localAI[0] = CalamityWorld.death ? 60f : 120f;
                    }
                    if (npc.localAI[0] > 0f)
                    {
                        npc.localAI[0] -= 1f;
                    }
                }
            }

            if (npcType == NPCID.GraniteGolem)
            {
                int activeTime = 300;
                int defendTime = 120;
                npc.defense = npc.defDefense;
                if (npc.ai[2] < 0f)
                {
                    npc.defense = npc.defDefense + 15;
                    npc.ai[2] += 1f;
                    npc.velocity.X *= 0.9f;
                    if (Math.Abs(npc.velocity.X) < 0.001)
                    {
                        npc.velocity.X = 0.001f * (float)npc.direction;
                    }
                    if (Math.Abs(npc.velocity.Y) > 1f)
                    {
                        npc.ai[2] += 10f;
                    }
                    if (npc.ai[2] >= 0f)
                    {
                        npc.netUpdate = true;
                        npc.velocity.X += (float)npc.direction * 0.3f;
                    }
                    return false;
                }
                if (npc.ai[2] < activeTime)
                {
                    if (npc.justHit)
                    {
                        npc.ai[2] += 15f;
                    }
                    npc.ai[2] += 1f;
                }
                else if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] = defendTime * -1f;
                    npc.netUpdate = true;
                }
            }

            if (npcType == NPCID.Medusa)
            {
                int afterHitTime = 90;
                int afterWaitTime = 210;
                int maxTime = 270;
                int debuffTime = (CalamityWorld.death ? 4 : 2) * 60; // 2 seconds in Rev, 4 seconds in Death
                int turnToStoneTime = 20;
                float mesudaActiveDistance = CalamityWorld.death ? 1500f : 900f;
                float medusaEffectDistance = CalamityWorld.death ? 1600f : 1000f;
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                else if (npc.ai[2] == 0f)
                {
                    if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && npc.velocity.Y == 0f && npc.Distance(Main.player[npc.target].Center) < mesudaActiveDistance && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = (float)(maxTime * -1f - turnToStoneTime);
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (npc.ai[2] < 0f && npc.ai[2] < maxTime * -1f)
                    {
                        npc.velocity.X *= 0.9f;
                        if (npc.velocity.Y < -2f || npc.velocity.Y > 4f || npc.justHit)
                        {
                            npc.ai[2] = afterHitTime;
                        }
                        else
                        {
                            npc.ai[2] += 1f;
                            if (npc.ai[2] == 0f)
                            {
                                npc.ai[2] = afterWaitTime;
                            }
                        }
                        float time = npc.ai[2] + maxTime + turnToStoneTime;
                        if (time == 1f)
                        {
                            SoundEngine.PlaySound(SoundID.NPCDeath17, npc.position);
                        }
                        if (time < turnToStoneTime)
                        {
                            MedusaHeadDustEffect(npc, time);
                        }
                        Lighting.AddLight(npc.Center, 0.9f, 0.75f, 0.1f);
                        return false;
                    }
                    if (npc.ai[2] < 0f && npc.ai[2] >= maxTime * -1f)
                    {
                        Lighting.AddLight(npc.Center, 0.9f, 0.75f, 0.1f);
                        npc.velocity.X *= 0.9f;
                        if (npc.velocity.Y < -2f || npc.velocity.Y > 4f || npc.justHit)
                        {
                            npc.ai[2] = afterHitTime;
                        }
                        else
                        {
                            npc.ai[2] += 1f;
                            if (npc.ai[2] == 0f)
                            {
                                npc.ai[2] = afterWaitTime;
                            }
                        }
                        float time = npc.ai[2] + maxTime;
                        if (time < 180f && (Main.rand.NextBool(3) || npc.ai[2] % 3f == 0f))
                        {
                            MedusaHeadDustEffect(npc, time);
                        }
                        if (Main.netMode != NetmodeID.Server)
                        {
                            Player player = Main.player[Main.myPlayer];
                            if (!player.dead && player.active && player.FindBuffIndex(BuffID.Stoned) == -1)
                            {
                                if (npc.Distance(player.Center) < medusaEffectDistance)
                                {
                                    bool canTurnPlayerToStone = npc.Distance(player.Center) < 30f;
                                    if (!canTurnPlayerToStone)
                                    {
                                        // Used to be "float x = 0.7853982f.ToRotationVector2().X;"
                                        // cos(pi/4) should do the job too, though. If you want/need to revert this to the
                                        // code above, do so.
                                        float x = (float)Math.Cos(MathHelper.PiOver4);
                                        Vector2 vector6 = Vector2.Normalize(player.Center - npc.Center);
                                        if (vector6.X > x || vector6.X < -x)
                                        {
                                            canTurnPlayerToStone = true;
                                        }
                                    }
                                    if (((player.Center.X < npc.Center.X && npc.direction < 0 && player.direction > 0) || (player.Center.X > npc.Center.X && npc.direction > 0 && player.direction < 0)) && canTurnPlayerToStone && (Collision.CanHitLine(npc.Center, 1, 1, player.Center, 1, 1) || Collision.CanHitLine(npc.Center - Vector2.UnitY * 16f, 1, 1, player.Center, 1, 1) || Collision.CanHitLine(npc.Center + Vector2.UnitY * 8f, 1, 1, player.Center, 1, 1)))
                                    {
                                        player.AddBuff(BuffID.Stoned, debuffTime + (int)npc.ai[2] * -1, true);
                                    }
                                }
                            }
                        }
                        return false;
                    }
                }
            }

            if (npcType == NPCID.GoblinSummoner)
            {
                // Shit out minion things
                if (npc.ai[3] < 0f)
                {
                    npc.knockBackResist = 0f;
                    npc.defense = (int)(npc.defDefense * 1.3);
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    npc.direction = (npc.velocity.X > 0).ToDirectionInt();
                    npc.rotation = npc.velocity.X * 0.1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.localAI[3] += 1f;
                        if (npc.localAI[3] > (float)Main.rand.Next(20, CalamityWorld.death ? 40 : 120))
                        {
                            npc.localAI[3] = 0f;
                            Vector2 spawnPosition = npc.Center;
                            spawnPosition += npc.velocity;
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)spawnPosition.X, (int)spawnPosition.Y, NPCID.ChaosBall);
                        }
                    }
                }
                else
                {
                    npc.localAI[3] = 0f;
                    npc.knockBackResist = 0.2f;
                    npc.rotation *= 0.9f;
                    npc.defense = npc.defDefense;
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                }
                if (npc.ai[3] == 1f)
                {
                    npc.knockBackResist = 0f;
                    npc.defense += 10;
                }
                if (npc.ai[3] == -1f)
                {
                    npc.TargetClosest(true);
                    float velocityMultiplier = 10f;
                    float turnValue = 40f;
                    Vector2 value3 = Main.player[npc.target].Center - npc.Center;
                    float playerDistance = value3.Length();
                    velocityMultiplier += playerDistance / 200f;
                    value3.Normalize();
                    value3 *= velocityMultiplier;
                    npc.velocity = (npc.velocity * (turnValue - 1f) + value3) / turnValue;
                    if (playerDistance < 500f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        // Go back to normal AI
                        npc.ai[3] = 0f;
                        npc.ai[2] = 0f;
                    }
                    return false;
                }
                // Go up on the Y axis and slow down on the X axis
                if (npc.ai[3] == -2f)
                {
                    npc.velocity.Y -= 0.2f;
                    if (npc.velocity.Y < -12f)
                    {
                        npc.velocity.Y = -12f;
                    }
                    if (Main.player[npc.target].Center.Y - npc.Center.Y > 200f)
                    {
                        npc.TargetClosest(true);
                        npc.ai[3] = -3f;
                        if (Main.player[npc.target].Center.X > npc.Center.X)
                        {
                            npc.ai[2] = 1f;
                        }
                        else
                        {
                            npc.ai[2] = -1f;
                        }
                    }
                    npc.velocity.X *= 0.99f;
                    return false;
                }
                // Similar to above, but more quick
                if (npc.ai[3] == -3f)
                {
                    if (npc.direction == 0)
                    {
                        npc.TargetClosest(true);
                    }
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[2] = (float)npc.direction;
                    }
                    npc.velocity.Y *= 0.9f;
                    npc.velocity.X += npc.ai[2] * 0.3f;
                    if (npc.velocity.X > 10f)
                    {
                        npc.velocity.X = 10f;
                    }
                    if (npc.velocity.X < -10f)
                    {
                        npc.velocity.X = -10f;
                    }
                    float playerDistance = Main.player[npc.target].Center.X - npc.Center.X;
                    if ((npc.ai[2] < 0f && playerDistance > 300f) || (npc.ai[2] > 0f && playerDistance < -300f))
                    {
                        npc.ai[3] = -4f;
                        npc.ai[2] = 0f;
                        return false;
                    }
                    if (Math.Abs(Main.player[npc.target].Center.X - npc.Center.X) > 800f)
                    {
                        npc.ai[3] = -1f;
                        npc.ai[2] = 0f;
                    }
                    return false;
                }
                else
                {
                    if (npc.ai[3] == -4f)
                    {
                        npc.ai[2] += 1f;
                        npc.velocity.Y += 0.1f;
                        // Don't go very fast
                        if (npc.velocity.Length() > 4f)
                        {
                            npc.velocity *= 0.9f;
                        }
                        int tileAtCenterX = (int)npc.Center.X / 16;
                        int tileAtBottom = (int)(npc.position.Y + (float)npc.height + 12f) / 16;
                        bool ableToRestart = false;
                        for (int i = tileAtCenterX - 1; i <= tileAtCenterX + 1; i++)
                        {
                            if (Main.tile[i, tileAtBottom].HasTile && Main.tileSolid[(int)Main.tile[i, tileAtBottom].TileType])
                            {
                                ableToRestart = true;
                            }
                        }
                        if (ableToRestart && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.ai[3] = 0f;
                            npc.ai[2] = 0f;
                        }
                        else if (npc.ai[2] > 300f || npc.Center.Y > Main.player[npc.target].Center.Y + 200f)
                        {
                            npc.ai[3] = -1f;
                            npc.ai[2] = 0f;
                        }
                    }
                    // Barf out the shadowflame skull things
                    else
                    {
                        if (npc.ai[3] == 1f)
                        {
                            Vector2 spawnPosiion = npc.Center;
                            spawnPosiion.Y -= 70f;
                            npc.velocity.X *= 0.8f;
                            npc.ai[2] += 1f;
                            if (npc.ai[2] == (CalamityWorld.death ? 15f : 30f))
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    NPC.NewNPC(npc.GetSource_FromAI(), (int)spawnPosiion.X, (int)spawnPosiion.Y + 18, NPCID.ShadowFlameApparition, 0, 0f, 0f, 0f, 0f, 255);
                                }
                            }
                            else if (npc.ai[2] >= 90f)
                            {
                                npc.ai[3] = -2f;
                                npc.ai[2] = 0f;
                            }
                            for (int j = 0; j < 2; j++)
                            {
                                // Randomize and normalize. You know the drill
                                Vector2 dustVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                                dustVelocity *= (float)Main.rand.Next(0, 100) * 0.1f;
                                dustVelocity.Normalize();
                                dustVelocity *= (float)Main.rand.Next(50, 90) * 0.1f;
                                int dustIdx = Dust.NewDust(spawnPosiion, 1, 1, 27, 0f, 0f, 0, default, 1f);
                                Main.dust[dustIdx].velocity = -dustVelocity * 0.3f;
                                Main.dust[dustIdx].alpha = 100;
                                if (Main.rand.NextBool(2))
                                {
                                    Main.dust[dustIdx].noGravity = true;
                                    Main.dust[dustIdx].scale += 0.3f;
                                }
                            }
                            return false;
                        }
                        npc.ai[2] += 1f;
                        int maxSkullCount = 10;
                        if (npc.velocity.Y == 0f && NPC.CountNPCS(NPCID.ShadowFlameApparition) < maxSkullCount)
                        {
                            if (npc.ai[2] >= 180f)
                            {
                                npc.ai[2] = 0f;
                                npc.ai[3] = 1f;
                            }
                        }
                        else
                        {
                            if (NPC.CountNPCS(NPCID.ShadowFlameApparition) >= maxSkullCount)
                            {
                                npc.ai[2] += 1f;
                            }
                            if (npc.ai[2] >= 360f)
                            {
                                npc.ai[2] = 0f;
                                npc.ai[3] = -2f;
                                npc.velocity.Y -= 3f;
                            }
                        }
                        if (npc.target >= 0 && !Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() > 800f)
                        {
                            npc.ai[3] = -1f;
                            npc.ai[2] = 0f;
                        }
                    }
                    if (Main.player[npc.target].dead)
                    {
                        npc.TargetClosest(true);
                        if (Main.player[npc.target].dead && npc.timeLeft > 1)
                        {
                            npc.timeLeft = 1;
                        }
                    }
                }
            }

            if (npcType == NPCID.SolarSolenian)
            {
                npc.reflectsProjectiles = false;
                npc.takenDamageMultiplier = 1f;
                int chargeTime = 6;
                int yFlyTime = 10;
                float velocityMultiplier = 20f;
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                if (npc.ai[2] == 0f)
                {
                    if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = -1f;
                        npc.netUpdate = true;
                        npc.TargetClosest(true);
                    }
                }
                else
                {
                    if (npc.ai[2] < 0f && npc.ai[2] > chargeTime * -1f)
                    {
                        npc.ai[2] -= 1f;
                        npc.velocity.X *= 0.9f;
                        return false;
                    }
                    if (npc.ai[2] == chargeTime * -1f)
                    {
                        npc.ai[2] -= 1f;
                        npc.TargetClosest(true);
                        Vector2 vectorToPlayer = npc.SafeDirectionTo(Main.player[npc.target].Top - Vector2.UnitY * 30f, Vector2.Normalize(new Vector2(npc.spriteDirection, -1f)));

                        npc.velocity = vectorToPlayer * velocityMultiplier;
                        npc.netUpdate = true;
                        return false;
                    }
                    if (npc.ai[2] < chargeTime * -1f)
                    {
                        npc.ai[2] -= 1f;
                        if (npc.velocity.Y == 0f)
                        {
                            npc.ai[2] = CalamityWorld.death ? 60f : 90f;
                        }
                        else if (npc.ai[2] < (float)(chargeTime * -1f - yFlyTime))
                        {
                            npc.velocity.Y += 0.15f;
                            if (npc.velocity.Y > 24f)
                            {
                                npc.velocity.Y = 24f;
                            }
                        }
                        npc.reflectsProjectiles = true;
                        npc.takenDamageMultiplier = CalamityWorld.death ? 2f : 3f;
                        if (npc.justHit)
                        {
                            npc.ai[2] = CalamityWorld.death ? 60f : 90f;
                            npc.netUpdate = true;
                        }
                        return false;
                    }
                }
            }

            if (npcType == NPCID.SolarDrakomire)
            {
                int timeToReset = 42;
                int timeToBreathFire = 18;
                if (npc.justHit)
                {
                    npc.ai[2] = CalamityWorld.death ? 30f : 60f;
                    npc.netUpdate = true;
                }
                if (npc.ai[2] > 0f)
                {
                    npc.ai[2] -= 1f;
                }
                if (npc.ai[2] == 0f)
                {
                    int solarFlareCount = 0;
                    int maxFlareCount = 6;
                    for (int k = 0; k < Main.maxNPCs; k++)
                    {
                        if (Main.npc[k].active && Main.npc[k].type == NPCID.SolarFlare)
                        {
                            solarFlareCount++;
                        }
                    }
                    if (solarFlareCount > maxFlareCount)
                    {
                        npc.ai[2] = CalamityWorld.death ? 30f : 60f;
                    }
                    else if (((Main.player[npc.target].Center.X < npc.Center.X && npc.direction < 0) || (Main.player[npc.target].Center.X > npc.Center.X && npc.direction > 0)) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    {
                        npc.ai[2] = -1f;
                        npc.netUpdate = true;
                        npc.TargetClosest(true);
                    }
                }
                else if (npc.ai[2] < 0f && npc.ai[2] > (float)(-(float)timeToReset))
                {
                    npc.ai[2] -= 1f;
                    if (npc.ai[2] == (float)(-(float)timeToReset))
                    {
                        npc.ai[2] = (float)(180 + 10 * Main.rand.Next(10));
                    }
                    npc.velocity.X *= 0.8f;
                    if (npc.ai[2] == (float)(-(float)timeToBreathFire) || npc.ai[2] == (float)(-(float)timeToBreathFire - 8) || npc.ai[2] == (float)(-(float)timeToBreathFire - 16))
                    {
                        for (int l = 0; l < 20; l++)
                        {
                            Vector2 spawnPosition = npc.Center + Vector2.UnitX * (float)npc.spriteDirection * 40f;
                            Dust dust = Main.dust[Dust.NewDust(spawnPosition, 0, 0, 259, 0f, 0f, 0, default, 1f)];
                            Vector2 velocity = Vector2.UnitY.RotatedByRandom(Math.PI * 2.0);
                            dust.position = spawnPosition + velocity * 4f;
                            dust.velocity = velocity * 2f + Vector2.UnitX * Main.rand.NextFloat() * (float)npc.spriteDirection * 3f;
                            dust.scale = 0.3f + velocity.X * (float)(-(float)npc.spriteDirection);
                            dust.fadeIn = 0.7f;
                            dust.noGravity = true;
                        }
                        if (npc.velocity.X > -0.5f && npc.velocity.X < 0.5f)
                        {
                            npc.velocity.X = 0f;
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X + npc.spriteDirection * 45, (int)npc.Center.Y + 8, NPCID.SolarFlare, 0, 0f, 0f, 0f, 0f, npc.target);
                        }
                    }
                    return false;
                }
            }

            if (npcType == NPCID.VortexLarva)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= (CalamityWorld.death ? 90f : 150f))
                {
                    int centerTileX = (int)npc.Center.X / 16 - 1;
                    int centerTileY = (int)npc.Center.Y / 16 - 1;
                    if (!Collision.SolidTiles(centerTileX, centerTileX + 2, centerTileY, centerTileY + 1) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.Transform(NPCID.VortexHornet);
                        npc.life = npc.lifeMax;
                        npc.localAI[0] = 0f;
                        return false;
                    }
                }
                int maxValue;
                if (npc.localAI[0] < 30f)
                {
                    maxValue = 16;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 45f : 60f))
                {
                    maxValue = 8;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 60f : 90f))
                {
                    maxValue = 4;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 75f : 120f))
                {
                    maxValue = 2;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 90f : 150f))
                {
                    maxValue = 1;
                }
                else
                {
                    maxValue = 1;
                }
                if (Main.rand.NextBool(maxValue))
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default, 1f)];
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.noLight = true;
                    dust.velocity = npc.DirectionFrom(dust.position) * dust.velocity.Length();
                    dust.position -= dust.velocity * 5f;
                    dust.position.X += (float)(npc.direction * 6);
                    dust.position.Y += 4f;
                }
            }

            if (npcType == NPCID.VortexHornet)
            {
                npc.localAI[0] += 1f;
                npc.localAI[0] += Math.Abs(npc.velocity.X) / 2f;
                if (npc.localAI[0] >= (CalamityWorld.death ? 300f : 600f) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int centerTileX = (int)npc.Center.X / 16 - 2;
                    int centerTileY = (int)npc.Center.Y / 16 - 3;
                    if (!Collision.SolidTiles(centerTileX, centerTileX + 4, centerTileY, centerTileY + 4))
                    {
                        npc.Transform(NPCID.VortexHornetQueen);
                        npc.life = npc.lifeMax;
                        npc.localAI[0] = 0f;
                        return false;
                    }
                }
                int maxValue2;
                if (npc.localAI[0] < (CalamityWorld.death ? 60f : 120f))
                {
                    maxValue2 = 32;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 120f : 240f))
                {
                    maxValue2 = 16;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 180f : 360f))
                {
                    maxValue2 = 6;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 240f : 480f))
                {
                    maxValue2 = 2;
                }
                else if (npc.localAI[0] < (CalamityWorld.death ? 300f : 600f))
                {
                    maxValue2 = 1;
                }
                else
                {
                    maxValue2 = 1;
                }
                if (Main.rand.NextBool(maxValue2))
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.position, npc.width, npc.height, 229, 0f, 0f, 0, default, 1f)];
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.noLight = true;
                }
            }

            bool jump = false;
            if (npc.velocity.X == 0f)
            {
                jump = true;
            }
            if (npc.justHit)
            {
                jump = false;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && npcType == NPCID.Lihzahrd && (double)npc.life <= (double)npc.lifeMax * 0.9)
            {
                npc.Transform(NPCID.LihzahrdCrawler);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && npcType == NPCID.Nutcracker && (double)npc.life <= (double)npc.lifeMax * 0.9)
            {
                npc.Transform(NPCID.NutcrackerSpinning);
            }

            // This variable seems to have a lot of purposes.
            // I wasn't sure what I could name it that isn't very vague
            int aiGateValue = 60;
            if (npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>())
            {
                aiGateValue = 180;
                if (npc.ai[3] == -30f)
                {
                    npc.velocity *= 0f;
                    npc.ai[3] = 0f;
                    SoundEngine.PlaySound(SoundID.Item8, npc.position);
                    float distX = npc.oldPos[2].X + (float)npc.width * 0.5f - npc.Center.X;
                    float distY = npc.oldPos[2].Y + (float)npc.height * 0.5f - npc.Center.Y;
                    float num38 = (float)Math.Sqrt((distX * distX + distY * distY));
                    num38 = 2f / num38;
                    distX *= num38;
                    distY *= num38;
                    for (int m = 0; m < 20; m++)
                    {
                        int dustIdx = Dust.NewDust(npc.position, npc.width, npc.height, 71, distX, distY, 200, default, 2f);
                        Main.dust[dustIdx].noGravity = true;
                        Dust dust = Main.dust[dustIdx];
                        dust.velocity.X *= 2f;
                    }
                    for (int n = 0; n < 20; n++)
                    {
                        int dustIdx = Dust.NewDust(npc.oldPos[2], npc.width, npc.height, 71, -distX, -distY, 200, default, 2f);
                        Main.dust[dustIdx].noGravity = true;
                        Dust dust = Main.dust[dustIdx];
                        dust.velocity.X *= 2f;
                    }
                }
            }

            bool canIncrementAI3 = false;
            bool reset = true;
            if (npcType == NPCID.Yeti || npcType == NPCID.CorruptBunny || npcType == NPCID.Crab || npcType == NPCID.Clown || npcType == NPCID.SkeletonArcher || npcType == NPCID.GoblinArcher || npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>()
                || npcType == NPCID.BlackRecluse || npcType == NPCID.WallCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.CorruptPenguin || npcType == NPCID.LihzahrdCrawler || npcType == NPCID.IcyMerman || npcType == NPCID.PirateDeadeye
                || npcType == NPCID.PirateCrossbower || npcType == NPCID.PirateCaptain || npcType == NPCID.CochinealBeetle || npcType == NPCID.CyanBeetle || npcType == NPCID.LacBeetle || npcType == NPCID.SeaSnail || npcType == NPCID.FlyingSnake
                || npcType == NPCID.IceGolem || npcType == NPCID.Eyezor || npcType == NPCID.AnomuraFungus || npcType == NPCID.MushiLadybug || npcType == NPCID.Paladin || npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton
                || npcType == NPCID.SkeletonCommando || npcType == NPCID.Scarecrow1 || npcType == NPCID.Scarecrow2 || npcType == NPCID.Scarecrow3 || npcType == NPCID.Scarecrow4 || npcType == NPCID.Scarecrow5 || npcType == NPCID.Nutcracker
                || npcType == NPCID.NutcrackerSpinning || npcType == NPCID.ElfArcher || npcType == NPCID.Krampus || npcType == NPCID.CultistArcherBlue || (npcType >= 430 && npcType <= 436)
                || (npcType == NPCID.CultistArcherWhite || npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianOfficer || npcType == NPCID.MartianEngineer || npcType == NPCID.Scutlix
                || (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4)) || (npcType == NPCID.Psycho || npcType == NPCID.CrimsonBunny || npcType == NPCID.SwampThing || npcType == NPCID.ThePossessed || npcType == NPCID.DrManFly
                || npcType == NPCID.GoblinSummoner || npcType == NPCID.CrimsonPenguin || npcType == NPCID.Medusa || npcType == NPCID.GreekSkeleton || npcType == NPCID.GraniteGolem || npcType == NPCID.StardustSoldier || npcType == NPCID.NebulaSoldier
                || npcType == NPCID.StardustSpiderBig || (npcType >= 494 && npcType <= 506)) || (npcType == NPCID.VortexRifleman || npcType == NPCID.VortexHornet || npcType == NPCID.VortexHornetQueen || npcType == NPCID.VortexLarva
                || npcType == NPCID.WalkingAntlion || npcType == NPCID.SolarDrakomire || npcType == NPCID.SolarSolenian || npcType == NPCID.MartianWalker || (npcType >= 524 && npcType <= 527)) || npcType == NPCID.DesertLamiaLight
                || npcType == NPCID.DesertLamiaDark || npcType == NPCID.DesertScorpionWalk || npcType == NPCID.DesertBeast)
            {
                reset = false;
            }

            bool ableToAlterAI3 = false;
            if (npcType == NPCID.VortexRifleman || npcType == NPCID.GoblinSummoner)
            {
                ableToAlterAI3 = true;
            }

            // This variable means so many fucking things for different NPCs
            // Such as npc spawn timer, projectile timer, ect
            // From what I can gather, the ai2 value's max for spawning things also varies
            // As a result, I'll just leave this as flag7. If anyone has
            // Ideas for a better variable name, please change it
            bool flag7 = npc.ai[2] <= 0f;
            if (npcType <= NPCID.RayGunner)
            {
                if (npcType <= NPCID.PirateCaptain)
                {
                    if (npcType - 110 > 1 && npcType != NPCID.IcyMerman && npcType - NPCID.PirateDeadeye > 2)
                    {
                        goto PrepareToShoot;
                    }
                }
                else if (npcType - NPCID.SkeletonSniper > 2 && npcType != NPCID.ElfArcher && npcType - NPCID.CultistArcherBlue > 3)
                {
                    goto PrepareToShoot;
                }
            }
            else if (npcType <= NPCID.NebulaSoldier)
            {
                if (npcType != NPCID.StardustSpiderSmall && npcType != NPCID.StardustSoldier && npcType != NPCID.NebulaSoldier)
                {
                    goto PrepareToShoot;
                }
            }
            else if (npcType <= NPCID.Psycho)
            {
                if (npcType != NPCID.VortexHornetQueen && npcType != NPCID.Psycho)
                {
                    goto PrepareToShoot;
                }
            }
            else if (npcType - 498 > 8 && npcType != NPCID.MartianWalker)
            {
                goto PrepareToShoot;
            }

            // If anyone can give me an explanation of the real noticable differences between
            //& and && (same with | and ||) with booleans,
            // I'd greatly appreciate it
            PrepareToShoot:
            if (!ableToAlterAI3 & flag7)
            {
                if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
                {
                    canIncrementAI3 = true;
                }
                if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)aiGateValue) | canIncrementAI3)
                {
                    npc.ai[3] += 1f;
                }
                else if (Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
                {
                    npc.ai[3] -= 1f;
                }
                if (npc.ai[3] > (float)(aiGateValue * 10))
                {
                    npc.ai[3] = 0f;
                }
                if (npc.justHit)
                {
                    npc.ai[3] = 0f;
                }
                if (npc.ai[3] == (float)aiGateValue)
                {
                    npc.netUpdate = true;
                }
            }

            if (npcType == NPCID.Nailhead && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.localAI[3] > 0f)
                {
                    npc.localAI[3] -= 1f;
                }
                if (npc.justHit && npc.localAI[3] <= 0f && Main.rand.NextBool())
                {
                    npc.localAI[3] = 30f;
                    int nailCount = Main.rand.Next(3, 6);
                    int[] players = new int[nailCount];
                    int i = 0;
                    for (int j = 0; j < Main.player.Length; j++)
                    {
                        if (Main.player[j].active && !Main.player[j].dead && Collision.CanHitLine(npc.position, npc.width, npc.height, Main.player[j].position, Main.player[j].width, Main.player[j].height))
                        {
                            players[i] = j;
                            i++;
                            if (i == nailCount)
                            {
                                break;
                            }
                        }
                    }
                    if (i > 1)
                    {
                        for (int j = 0; j < 100; j++)
                        {
                            int rand = Main.rand.Next(i);
                            int rand2;
                            for (rand2 = rand; rand2 == rand; rand2 = Main.rand.Next(i))
                            {
                            }
                            Utils.Swap(ref players[rand], ref players[rand2]);
                        }
                    }
                    Vector2 velocityDelta = -Vector2.One;
                    for (int j = 0; j < i; j++)
                        velocityDelta += npc.SafeDirectionTo(Main.npc[players[j]].Center);

                    velocityDelta.Normalize();
                    for (int j = 0; j < nailCount; j++)
                    {
                        float velocityMultiplier = Main.rand.Next(8, 13);
                        Vector2 nailVelocity = Vector2.UnitY.RotatedByRandom(Math.PI * 2.0);
                        if (i > 0)
                        {
                            nailVelocity += velocityDelta;
                            nailVelocity.Normalize();
                        }
                        nailVelocity *= velocityMultiplier;
                        if (i > 0)
                        {
                            i--;
                            nailVelocity = npc.SafeDirectionTo(Main.npc[players[i]].Center, -Vector2.UnitY) * velocityMultiplier;
                        }

                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center + Vector2.UnitY * (npc.width / 4), nailVelocity, ProjectileID.Nail, (int)(npc.damage * 0.15), 1f, 255, 0f, 0f);
                    }
                }
            }

            if (npcType == NPCID.Butcher)
            {
                if (npc.velocity.Y < -0.3f || npc.velocity.Y > 0.3f)
                {
                    npc.knockBackResist = 0f;
                }
                else
                {
                    npc.knockBackResist = 0.1f;
                }
            }

            if (npcType == NPCID.ThePossessed)
            {
                npc.knockBackResist = 0.25f;
                if (npc.ai[2] == 1f)
                {
                    npc.knockBackResist = 0f;
                }
                bool spiderAI = false;
                int centerTileX = (int)npc.Center.X / 16;
                int centerTileY = (int)npc.Center.Y / 16;
                for (int x = centerTileX - 1; x <= centerTileX + 1; x++)
                {
                    for (int y = centerTileY - 1; y <= centerTileY + 1; y++)
                    {
                        if (Main.tile[x, y] != null && Main.tile[x, y].WallType > 0)
                        {
                            spiderAI = true;
                            break;
                        }
                    }
                    if (spiderAI)
                    {
                        break;
                    }
                }
                if (npc.ai[2] == 0f & spiderAI)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.Y = -4.6f;
                        npc.velocity.X *= 1.5f;
                    }
                    else if (npc.velocity.Y > 0f)
                    {
                        npc.ai[2] = 1f;
                    }
                }
                if (spiderAI && npc.ai[2] == 1f && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    Vector2 distanceVector = Main.player[npc.target].Center - npc.Center;
                    float distanceMagnitude = distanceVector.Length();
                    distanceVector.Normalize();
                    distanceVector *= 6f + distanceMagnitude / 300f;
                    npc.velocity = (npc.velocity * 29f + distanceVector) / 30f;
                    npc.noGravity = true;
                    npc.ai[2] = 1f;
                    return false;
                }
                npc.noGravity = false;
                npc.ai[2] = 0f;
            }

            if (npcType == NPCID.Fritz && npc.velocity.Y == 0f && (Main.player[npc.target].Center - npc.Center).Length() < 150f && Math.Abs(npc.velocity.X) > 3f && ((npc.velocity.X < 0f && npc.Center.X > Main.player[npc.target].Center.X) || (npc.velocity.X > 0f && npc.Center.X < Main.player[npc.target].Center.X)))
            {
                npc.velocity.X *= 2f;
                npc.velocity.Y -= 4.5f;
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 20f)
                {
                    npc.velocity.Y -= 0.5f;
                }
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 40f)
                {
                    npc.velocity.Y -= 1f;
                }
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 80f)
                {
                    npc.velocity.Y -= 1.5f;
                }
                if (npc.Center.Y - Main.player[npc.target].Center.Y > 100f)
                {
                    npc.velocity.Y -= 1.5f;
                }
                if (Math.Abs(npc.velocity.X) > 9f)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = -9f;
                    }
                    else
                    {
                        npc.velocity.X = 9f;
                    }
                }
            }

            if (npc.ai[3] < (float)aiGateValue && (Main.eclipse || !Main.dayTime || (double)npc.position.Y > Main.worldSurface * 16.0 || (Main.invasionType == InvasionID.GoblinArmy && (npcType == NPCID.Yeti || npcType == NPCID.ElfArcher)) || (Main.invasionType == InvasionID.GoblinArmy && (npcType == NPCID.GoblinPeon || npcType == NPCID.GoblinThief || npcType == NPCID.GoblinWarrior || npcType == NPCID.GoblinArcher || npcType == NPCID.GoblinSummoner)) || (npcType == NPCID.GoblinScout || (Main.invasionType == InvasionID.PirateInvasion && npcType >= 212 && npcType <= 216)) || (Main.invasionType == InvasionID.MartianMadness && (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianOfficer || npcType == NPCID.GrayGrunt || npcType == NPCID.MartianEngineer || npcType == NPCID.GigaZapper || npcType == NPCID.Scutlix || npcType == NPCID.MartianWalker)) || (npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.CorruptBunny || npcType == NPCID.Crab || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.Mummy || npcType == NPCID.DarkMummy || npcType == NPCID.LightMummy || npcType == NPCID.SkeletonArcher || npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>() || npcType == NPCID.CorruptPenguin || npcType == NPCID.FaceMonster || npcType == NPCID.SnowFlinx || npcType == NPCID.Lihzahrd || npcType == NPCID.LihzahrdCrawler || npcType == NPCID.IcyMerman || npcType == NPCID.CochinealBeetle || npcType == NPCID.CyanBeetle || npcType == NPCID.LacBeetle || npcType == NPCID.SeaSnail || npcType == NPCID.BloodCrawler || npcType == NPCID.IceGolem || npcType == NPCID.ZombieMushroom || npcType == NPCID.ZombieMushroomHat || npcType == NPCID.AnomuraFungus || npcType == NPCID.MushiLadybug || npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton || npcType == NPCID.SkeletonCommando || npcType == NPCID.CultistArcherBlue || npcType == NPCID.CultistArcherWhite || npcType == NPCID.CrimsonBunny || npcType == NPCID.CrimsonPenguin || npcType == NPCID.NebulaSoldier || (npcType == NPCID.StardustSoldier && (npc.ai[1] >= 180f || npc.ai[1] < 90f))) || (npcType == NPCID.StardustSpiderBig || npcType == NPCID.VortexRifleman || npcType == NPCID.VortexSoldier || npcType == NPCID.VortexHornet || npcType == NPCID.VortexLarva || npcType == NPCID.WalkingAntlion || npcType == NPCID.SolarDrakomire || npcType == NPCID.SolarSolenian || (npcType >= 524 && npcType <= 527)) || npcType == NPCID.DesertLamiaLight || npcType == NPCID.DesertLamiaDark || npcType == NPCID.DesertScorpionWalk || npcType == NPCID.DesertBeast))
            {
                if ((npcType == NPCID.Zombie || npcType == NPCID.ZombieXmas || npcType == NPCID.ZombieSweater || npcType == NPCID.Skeleton || (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4) || npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.SkeletonArcher || npcType == NPCID.BaldZombie || npcType == NPCID.UndeadViking || npcType == NPCID.ZombieEskimo || npcType == NPCID.Frankenstein || npcType == NPCID.PincushionZombie || npcType == NPCID.SlimedZombie || npcType == NPCID.SwampZombie || npcType == NPCID.TwiggyZombie || npcType == NPCID.ArmoredViking || npcType == NPCID.FemaleZombie || npcType == NPCID.HeadacheSkeleton || npcType == NPCID.MisassembledSkeleton || npcType == NPCID.PantlessSkeleton || npcType == NPCID.ZombieRaincoat || npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton || npcType == NPCID.SkeletonCommando || npcType == NPCID.ZombieSuperman || npcType == NPCID.ZombiePixie || npcType == NPCID.ZombieDoctor || npcType == NPCID.GreekSkeleton) && Main.rand.NextBool(1000))
                {
                    SoundEngine.PlaySound(SoundID.ZombieMoan, npc.position);
                }
                if (npcType == NPCID.BloodZombie && Main.rand.NextBool(800))
                {
                    SoundEngine.PlaySound(SoundID.ZombieMoan, npc.position); //There was a npcType thing afterwards but its not really useable now. Hilarious, frankly
                }
                if ((npcType == NPCID.Mummy || npcType == NPCID.DarkMummy || npcType == NPCID.LightMummy) && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Mummy, npc.position);
                }
                if (npcType == NPCID.Vampire && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Zombie7, npc.position);
                }
                if (npcType == NPCID.Frankenstein && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Zombie6, npc.position);
                }
                if (npcType == NPCID.FaceMonster && Main.rand.NextBool(500))
                {
                    SoundEngine.PlaySound(SoundID.Zombie8, npc.position);
                }
                if (npcType >= 269 && npcType <= 280 && Main.rand.NextBool(1000))
                {
                    SoundEngine.PlaySound(SoundID.ZombieMoan, npc.position);
                }
                npc.TargetClosest(true);
            }
            else if (npc.ai[2] <= 0f ||
                (npcType != NPCID.SkeletonArcher &&
                npcType != NPCID.GoblinArcher &&
                npcType != NPCID.IcyMerman &&
                npcType != NPCID.PirateDeadeye &&
                npcType != NPCID.PirateCrossbower &&
                npcType != NPCID.PirateCaptain &&
                npcType != NPCID.SkeletonSniper &&
                npcType != NPCID.TacticalSkeleton &&
                npcType != NPCID.SkeletonCommando &&
                npcType != NPCID.ElfArcher &&
                npcType != NPCID.BrainScrambler &&
                npcType != NPCID.RayGunner &&
                npcType != NPCID.MartianOfficer &&
                npcType != NPCID.GrayGrunt &&
                npcType != NPCID.MartianEngineer &&
                npcType != NPCID.GigaZapper &&
                npcType != NPCID.Scutlix &&
                npcType != NPCID.ThePossessed &&
                npcType != NPCID.SwampThing &&
                npcType != NPCID.Psycho &&
                npcType != NPCID.GoblinSummoner &&
                npcType != NPCID.StardustSoldier &&
                npcType != NPCID.StardustSpiderSmall &&
                npcType != NPCID.NebulaSoldier &&
                npcType != NPCID.VortexRifleman &&
                npcType != NPCID.VortexHornetQueen &&
                npcType != NPCID.SolarDrakomire &&
                npcType != NPCID.SolarSolenian &&
                npcType != NPCID.MartianWalker))
            {
                if (Main.dayTime && (double)(npc.position.Y / 16f) < Main.worldSurface && npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;

                            npc.spriteDirection = npc.direction;

                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }

            if (npcType == NPCID.Vampire || npcType == NPCID.NutcrackerSpinning)
            {
                if (npcType == NPCID.Vampire && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
                {
                    npc.velocity.X *= 0.95f;
                }
                if (npc.velocity.X < -8f || npc.velocity.X > 8f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < 8f && npc.direction == 1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.99f;
                    }
                    npc.velocity.X += 0.09f;
                    if (npc.velocity.X > 8f)
                    {
                        npc.velocity.X = 8f;
                    }
                }
                else if (npc.velocity.X > -8f && npc.direction == -1)
                {
                    if (npc.velocity.Y == 0f && npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.99f;
                    }
                    npc.velocity.X -= 0.09f;
                    if (npc.velocity.X < -8f)
                    {
                        npc.velocity.X = -8f;
                    }
                }
            }
            else if (npcType == NPCID.LihzahrdCrawler)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 8f : 6f, 0.12f, 0.8f, true, 0.8f);
            }
            else if (npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>() || npcType == NPCID.SwampThing || npcType == NPCID.PirateCorsair || npcType == NPCID.MushiLadybug || npcType == NPCID.DesertLamiaLight || npcType == NPCID.DesertLamiaDark)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 6f : 4f, 0.09f, 0.8f, true, 0.8f);
            }
            else if (npcType == NPCID.CreatureFromTheDeep || npcType == NPCID.GoblinThief || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.Werewolf || npcType == NPCID.BlackRecluse || npcType == NPCID.Frankenstein || npcType == NPCID.Nymph || npcType == NPCID.ArmoredViking || npcType == NPCID.PirateDeckhand || npcType == NPCID.AnomuraFungus || npcType == NPCID.Splinterling || npcType == NPCID.Yeti || npcType == NPCID.Nutcracker || npcType == NPCID.Krampus || (npcType >= 524 && npcType <= 527) || npcType == NPCID.DesertScorpionWalk)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 3f : 2.5f, 0.09f, 0.8f);
            }
            else if (npcType == NPCID.Clown)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 5f : 4f, 0.06f, 0.8f);
            }
            else if (npcType == NPCID.Skeleton || npcType == NPCID.SporeSkeleton || npcType == NPCID.GoblinPeon || npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.CorruptBunny || npcType == NPCID.GoblinScout || npcType == NPCID.PossessedArmor || npcType == NPCID.WallCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.UndeadViking || npcType == NPCID.CorruptPenguin || npcType == NPCID.SnowFlinx || npcType == NPCID.Lihzahrd || npcType == NPCID.HeadacheSkeleton || npcType == NPCID.MisassembledSkeleton || npcType == NPCID.PantlessSkeleton || npcType == NPCID.CochinealBeetle || npcType == NPCID.CyanBeetle || npcType == NPCID.LacBeetle || npcType == NPCID.FlyingSnake || npcType == NPCID.FaceMonster || npcType == NPCID.ZombieMushroom || npcType == NPCID.ZombieElf || npcType == NPCID.ZombieElfBeard || npcType == NPCID.ZombieElfGirl || npcType == NPCID.GingerbreadMan || npcType == NPCID.GrayGrunt || npcType == NPCID.GigaZapper || npcType == NPCID.Fritz || npcType == NPCID.Nailhead || npcType == NPCID.Psycho || npcType == NPCID.CrimsonBunny || npcType == NPCID.ThePossessed || npcType == NPCID.CrimsonPenguin || npcType == NPCID.Medusa || npcType == NPCID.GraniteGolem || npcType == NPCID.VortexRifleman || npcType == NPCID.VortexSoldier)
            {
                float maxVelocity = 1.5f;
                if (npcType == NPCID.AngryBonesBig)
                {
                    maxVelocity = 2f;
                }
                else if (npcType == NPCID.AngryBonesBigMuscle)
                {
                    maxVelocity = 1.75f;
                }
                else if (npcType == NPCID.AngryBonesBigHelmet)
                {
                    maxVelocity = 1.25f;
                }
                else if (npcType == NPCID.HeadacheSkeleton)
                {
                    maxVelocity = 1.1f;
                }
                else if (npcType == NPCID.MisassembledSkeleton)
                {
                    maxVelocity = 0.9f;
                }
                else if (npcType == NPCID.PantlessSkeleton)
                {
                    maxVelocity = 1.2f;
                }
                else if (npcType == NPCID.ZombieElf)
                {
                    maxVelocity = 1.75f;
                }
                else if (npcType == NPCID.ZombieElfBeard)
                {
                    maxVelocity = 1.25f;
                }
                else if (npcType == NPCID.ZombieElfGirl)
                {
                    maxVelocity = 2f;
                }
                else if (npcType == NPCID.GrayGrunt)
                {
                    maxVelocity = 1.8f;
                }
                else if (npcType == NPCID.GigaZapper)
                {
                    maxVelocity = 2.25f;
                }
                else if (npcType == NPCID.Fritz)
                {
                    maxVelocity = 4f;
                }
                else if (npcType == NPCID.Nailhead)
                {
                    maxVelocity = 0.75f;
                }
                else if (npcType == NPCID.Psycho)
                {
                    maxVelocity = 3.75f;
                }
                else if (npcType == NPCID.ThePossessed)
                {
                    maxVelocity = 3.25f;
                }
                else if (npcType == NPCID.Medusa)
                {
                    maxVelocity = 1.5f + (1f - (float)npc.life / (float)npc.lifeMax) * 2f;
                }
                else if (npcType == NPCID.VortexRifleman)
                {
                    maxVelocity = 6f;
                }
                else if (npcType == NPCID.VortexSoldier)
                {
                    maxVelocity = 4f;
                }
                if (npcType == NPCID.Skeleton || npcType == NPCID.HeadacheSkeleton || npcType == NPCID.MisassembledSkeleton || npcType == NPCID.PantlessSkeleton || npcType == NPCID.GingerbreadMan)
                {
                    maxVelocity *= 1f + (1f - npc.scale);
                }
                maxVelocity *= 1.25f;
                if (CalamityWorld.death)
                    maxVelocity *= 1.25f;

                bool extraSlowdown = npc.velocity.Y == 0f && npcType == NPCID.Fritz && ((npc.direction > 0 && npc.velocity.X < 0f) || (npc.direction < 0 && npc.velocity.X > 0f));
                FighterRunningAI(npc, maxVelocity, 0.09f, 0.9f, extraSlowdown, 0.9f);
            }
            else if (npcType >= NPCID.RustyArmoredBonesAxe && npcType <= NPCID.HellArmoredBonesSword)
            {
                float maxVelocity = 1.5f;
                if (npcType == NPCID.RustyArmoredBonesAxe)
                {
                    maxVelocity = 2f;
                }
                if (npcType == NPCID.RustyArmoredBonesFlail)
                {
                    maxVelocity = 1f;
                }
                if (npcType == NPCID.RustyArmoredBonesSword)
                {
                    maxVelocity = 1.5f;
                }
                if (npcType == NPCID.RustyArmoredBonesSwordNoArmor)
                {
                    maxVelocity = 3f;
                }
                if (npcType == NPCID.BlueArmoredBones)
                {
                    maxVelocity = 1.25f;
                }
                if (npcType == NPCID.BlueArmoredBonesMace)
                {
                    maxVelocity = 3f;
                }
                if (npcType == NPCID.BlueArmoredBonesNoPants)
                {
                    maxVelocity = 3.25f;
                }
                if (npcType == NPCID.BlueArmoredBonesSword)
                {
                    maxVelocity = 2f;
                }
                if (npcType == NPCID.HellArmoredBones)
                {
                    maxVelocity = 2.75f;
                }
                if (npcType == NPCID.HellArmoredBonesSpikeShield)
                {
                    maxVelocity = 1.8f;
                }
                if (npcType == NPCID.HellArmoredBonesMace)
                {
                    maxVelocity = 1.3f;
                }
                if (npcType == NPCID.HellArmoredBonesSword)
                {
                    maxVelocity = 2.5f;
                }
                maxVelocity *= 1f + (1f - npc.scale);
                maxVelocity *= 1.25f;
                if (CalamityWorld.death)
                    maxVelocity *= 1.25f;

                FighterRunningAI(npc, maxVelocity, 0.09f, 0.8f, false);
            }
            else if (npcType >= 305 && npcType <= 314)
            {
                float maxVelocity = 1.5f;
                if (npcType == NPCID.Scarecrow1 || npcType == NPCID.Scarecrow6)
                {
                    maxVelocity = 2f;
                }
                if (npcType == NPCID.Scarecrow2 || npcType == NPCID.Scarecrow7)
                {
                    maxVelocity = 1.25f;
                }
                if (npcType == NPCID.Scarecrow3 || npcType == NPCID.Scarecrow8)
                {
                    maxVelocity = 2.25f;
                }
                if (npcType == NPCID.Scarecrow4 || npcType == NPCID.Scarecrow9)
                {
                    maxVelocity = 1.5f;
                }
                if (npcType == NPCID.Scarecrow5 || npcType == NPCID.Scarecrow10)
                {
                    maxVelocity = 1f;
                }
                maxVelocity *= 1.25f;
                if (CalamityWorld.death)
                    maxVelocity *= 1.25f;

                if (npcType < 310) //Pogo stick Scarecrows
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X *= 0.85f;
                        if (npc.velocity.X > -0.3f && npc.velocity.X < 0.3f)
                        {
                            npc.velocity.Y = -9f; //-7f normally
                            npc.velocity.X = maxVelocity * (float)npc.direction;
                        }
                    }
                    else if (npc.spriteDirection == npc.direction)
                    {
                        npc.velocity.X = (npc.velocity.X * 10f + maxVelocity * npc.direction) / 11f;
                    }
                }
                else
                {
                    FighterRunningAI(npc, maxVelocity, 0.09f, 0.8f, false);
                }
            }
            else if (npcType == NPCID.Crab || npcType == NPCID.SeaSnail || npcType == NPCID.VortexLarva)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 4f : 1f, 0.06f, 0.7f);
            }
            else if (npcType == NPCID.Mummy || npcType == NPCID.DarkMummy || npcType == NPCID.LightMummy)
            {
                float maxVelocity = 3f;
                float acceleration = 0.15f;
                if (npcType == NPCID.DarkMummy)
                {
                    maxVelocity *= 1.5f;
                }
                if (CalamityWorld.death)
                {
                    maxVelocity *= 1.25f;
                }
                FighterRunningAI(npc, maxVelocity, acceleration, 0.7f);
            }
            else if (npcType == NPCID.BoneLee)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 10f : 7f, 0.3f, 0.7f);
            }
            else if (npcType == NPCID.IceGolem)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 4.5f : 3f, 0.3f, 0.7f);
            }
            else if (npcType == NPCID.Eyezor)
            {
                FighterRunningAI(npc, CalamityWorld.death ? 5f : 3.5f, 0.3f, 0.8f);
            }
            else if (npcType == NPCID.MartianEngineer)
            {
                if (npc.ai[2] > 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                }
                else
                {
                    FighterRunningAI(npc, CalamityWorld.death ? 5f : 3.5f, 0.2f, 0.8f);
                }
            }
            else if (npcType == NPCID.Butcher)
            {
                float acceleration = 0.2f;
                if (Math.Abs(npc.velocity.X) > 2f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 2.5)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 3f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 3.5)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 4f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 4.5f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 5f)
                {
                    acceleration *= 0.8f;
                }
                if (Math.Abs(npc.velocity.X) > 5.5)
                {
                    acceleration *= 0.8f;
                }
                FighterRunningAI(npc, CalamityWorld.death ? 10f : 7f, acceleration, 0.8f);
            }
            else if (npcType == NPCID.GiantWalkingAntlion || npcType == NPCID.WalkingAntlion || npcType == NPCID.LarvaeAntlion)
            {
                float xAdditive = CalamityWorld.death ? 3.5f : 3f;
                float turnValue = 90f;
                float absoluteVelocityX = Math.Abs(npc.velocity.X);
                if (absoluteVelocityX > 2.75f)
                {
                    xAdditive = CalamityWorld.death ? 7f : 6f;
                    turnValue += 100f;
                }
                else if (absoluteVelocityX > 2.25)
                {
                    xAdditive = CalamityWorld.death ? 5f : 4.25f;
                    turnValue += 80f;
                }
                if (Math.Abs(npc.velocity.Y) < 0.5)
                {
                    if (npc.velocity.X > 0f && npc.direction < 0)
                    {
                        npc.velocity *= 0.9f;
                    }
                    if (npc.velocity.X < 0f && npc.direction > 0)
                    {
                        npc.velocity *= 0.9f;
                    }
                }
                if (Math.Abs(npc.velocity.Y) > 0.3f)
                {
                    turnValue *= 3f;
                }
                if (npc.velocity.X <= 0f && npc.direction < 0)
                {
                    npc.velocity.X = (npc.velocity.X * turnValue - xAdditive) / (turnValue + 1f);
                }
                else if (npc.velocity.X >= 0f && npc.direction > 0)
                {
                    npc.velocity.X = (npc.velocity.X * turnValue + xAdditive) / (turnValue + 1f);
                }
                else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) > 20f && Math.Abs(npc.velocity.Y) <= 0.3f)
                {
                    npc.velocity.X *= 0.99f;
                    npc.velocity.X += npc.direction * 0.025f;
                }
            }
            else if (npcType == NPCID.Scutlix || npcType == NPCID.VortexHornet || npcType == NPCID.SolarDrakomire || npcType == NPCID.SolarSolenian || npcType == NPCID.SolarSpearman || npcType == NPCID.DesertBeast)
            {
                float maxVelocity = 5f;
                float acceleration = 0.25f;
                float turnMultiplier = 0.7f;
                if (npcType == NPCID.VortexHornet)
                {
                    maxVelocity = 6f;
                    acceleration = 0.2f;
                    turnMultiplier = 0.8f;
                }
                else if (npcType == NPCID.SolarDrakomire)
                {
                    maxVelocity = 4f;
                    acceleration = 0.1f;
                    turnMultiplier = 0.95f;
                }
                else if (npcType == NPCID.SolarSolenian)
                {
                    maxVelocity = 6f;
                    acceleration = 0.15f;
                    turnMultiplier = 0.85f;
                }
                else if (npcType == NPCID.SolarSpearman)
                {
                    maxVelocity = 5f;
                    acceleration = 0.1f;
                    turnMultiplier = 0.95f;
                }
                else if (npcType == NPCID.DesertBeast)
                {
                    maxVelocity = 5f;
                    acceleration = 0.15f;
                    turnMultiplier = 0.98f;
                }
                maxVelocity *= 1.25f;
                acceleration *= 1.25f;
                if (CalamityWorld.death)
                {
                    maxVelocity *= 1.25f;
                    acceleration *= 1.25f;
                }
                FighterRunningAI(npc, maxVelocity, acceleration, turnMultiplier);
            }
            else if ((npcType >= NPCID.ArmedZombie && npcType <= NPCID.ArmedZombieCenx) || npcType == NPCID.Crawdad || npcType == NPCID.Crawdad2)
            {
                if (npc.ai[2] == 0f)
                {
                    npc.damage = npc.defDamage;
                    float maxVelocity = CalamityWorld.death ? 2.5f : 1.5f;
                    maxVelocity *= 1f + (1f - npc.scale);
                    FighterRunningAI(npc, maxVelocity, 0.09f, 0.8f, true, 0.8f);
                    if (npc.velocity.Y == 0f && (!Main.dayTime || (double)npc.position.Y > Main.worldSurface * 16.0) && !Main.player[npc.target].dead)
                    {
                        Vector2 playerDistance = npc.Center - Main.player[npc.target].Center;
                        int slowdownDistance = 50;
                        if (npcType >= NPCID.Crawdad && npcType <= NPCID.Crawdad2)
                        {
                            slowdownDistance = 42;
                        }
                        if (playerDistance.Length() < slowdownDistance && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                        {
                            npc.velocity.X *= 0.7f;
                            npc.ai[2] = 1f;
                        }
                    }
                }
                else
                {
                    npc.damage = npc.defDamage * 2;
                    npc.ai[3] = 1f;
                    npc.velocity.X *= 0.9f;
                    if (Math.Abs(npc.velocity.X) < 0.1f)
                    {
                        npc.velocity.X = 0f;
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= 20f || npc.velocity.Y != 0f || (Main.dayTime && (double)npc.position.Y < Main.worldSurface * 16.0))
                    {
                        npc.ai[2] = 0f;
                    }
                }
            }
            else if (npcType != NPCID.SkeletonArcher &&
                npcType != NPCID.GoblinArcher &&
                npcType != NPCID.IcyMerman &&
                npcType != NPCID.PirateDeadeye &&
                npcType != NPCID.PirateCrossbower &&
                npcType != NPCID.PirateCaptain &&
                npcType != NPCID.Paladin &&
                npcType != NPCID.SkeletonSniper &&
                npcType != NPCID.TacticalSkeleton &&
                npcType != NPCID.SkeletonCommando &&
                npcType != NPCID.ElfArcher &&
                npcType != NPCID.CultistArcherBlue &&
                npcType != NPCID.CultistArcherWhite &&
                npcType != NPCID.BrainScrambler &&
                npcType != NPCID.RayGunner &&
                (npcType < NPCID.BoneThrowingSkeleton || npcType > NPCID.BoneThrowingSkeleton4) &&
                npcType != NPCID.DrManFly &&
                npcType != NPCID.GreekSkeleton &&
                npcType != NPCID.StardustSoldier &&
                npcType != NPCID.StardustSpiderSmall &&
                (npcType < NPCID.Salamander || npcType > NPCID.Salamander9) &&
                npcType != NPCID.NebulaSoldier &&
                npcType != NPCID.VortexSoldier &&
                npcType != NPCID.MartianWalker)
            {
                float velocityMax = 1f;
                if (npcType == NPCID.PincushionZombie)
                {
                    velocityMax = 1.1f;
                }
                if (npcType == NPCID.SlimedZombie)
                {
                    velocityMax = 0.9f;
                }
                if (npcType == NPCID.SwampZombie)
                {
                    velocityMax = 1.2f;
                }
                if (npcType == NPCID.TwiggyZombie)
                {
                    velocityMax = 0.8f;
                }
                if (npcType == NPCID.BaldZombie)
                {
                    velocityMax = 0.95f;
                }
                if (npcType == NPCID.FemaleZombie)
                {
                    velocityMax = 0.87f;
                }
                if (npcType == NPCID.ZombieRaincoat)
                {
                    velocityMax = 1.05f;
                }
                if (npcType == NPCID.BloodZombie)
                {
                    float playerDistance = (Main.player[npc.target].Center - npc.Center).Length();
                    playerDistance *= 0.0025f;
                    if (playerDistance > 1.5)
                    {
                        playerDistance = 1.5f;
                    }
                    if (Main.expertMode)
                    {
                        velocityMax = 3f - playerDistance;
                    }
                    else
                    {
                        velocityMax = 2.5f - playerDistance;
                    }
                    velocityMax *= 0.8f;
                }
                if (npcType == NPCID.BloodZombie || npcType == NPCID.Zombie || npcType == NPCID.BaldZombie || npcType == NPCID.PincushionZombie || npcType == NPCID.SlimedZombie || npcType == NPCID.SwampZombie || npcType == NPCID.TwiggyZombie || npcType == NPCID.FemaleZombie || npcType == NPCID.ZombieRaincoat || npcType == NPCID.ZombieXmas || npcType == NPCID.ZombieSweater)
                {
                    velocityMax *= 1f + (1f - npc.scale);
                }
                velocityMax *= 1.25f;
                if (CalamityWorld.death)
                    velocityMax *= 1.25f;
                FighterRunningAI(npc, velocityMax, 0.09f, 0.8f, true, 0.8f);
            }

            if (npcType >= 277 && npcType <= 280)
            {
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.2f, 0.1f, 0f);
            }
            else if (npcType == NPCID.MartianWalker)
            {
                Lighting.AddLight(npc.Top + new Vector2(0f, 20f), 0.3f, 0.3f, 0.7f);
            }
            else if (npcType == NPCID.DesertGhoulCorruption)
            {
                Vector3 rgb = new Vector3(0.7f, 1f, 0.2f) * 0.5f;
                Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb);
            }
            else if (npcType == NPCID.DesertGhoulCrimson)
            {
                Vector3 rgb2 = new Vector3(1f, 1f, 0.5f) * 0.4f;
                Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb2);
            }
            else if (npcType == NPCID.DesertGhoulHallow)
            {
                Vector3 rgb3 = new Vector3(0.6f, 0.3f, 1f) * 0.4f;
                Lighting.AddLight(npc.Top + new Vector2(0f, 15f), rgb3);
            }
            else if (npcType == NPCID.SolarDrakomire)
            {
                npc.hide = false;
                // I'd assume the Drakomire is drawn by the rider if it's present
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == NPCID.SolarDrakomireRider && Main.npc[i].ai[0] == (float)npc.whoAmI)
                    {
                        npc.hide = true;
                        break;
                    }
                }
            }
            else if (npcType == NPCID.MushiLadybug)
            {
                if (npc.velocity.Y != 0f)
                {
                    npc.TargetClosest(true);
                    npc.spriteDirection = npc.direction;
                    if (Main.player[npc.target].Center.X < npc.position.X && npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    else if (Main.player[npc.target].Center.X > npc.position.X + (float)npc.width && npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.95f;
                    }
                    if (Main.player[npc.target].Center.X < npc.position.X && npc.velocity.X > -5f)
                    {
                        npc.velocity.X -= 0.1f;
                    }
                    else if (Main.player[npc.target].Center.X > npc.position.X + (float)npc.width && npc.velocity.X < 5f)
                    {
                        npc.velocity.X += 0.1f;
                    }
                }
                else if (Main.player[npc.target].Center.Y + 50f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -9f;
                }
            }
            else if (npcType == NPCID.VortexRifleman)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] = 0f;
                }
                if (npc.velocity.Y != 0f && npc.ai[2] == 1f)
                {
                    npc.TargetClosest(true);
                    npc.spriteDirection = -npc.direction;
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        float idealLocationX = Main.player[npc.target].Center.X - (float)(npc.direction * 600) - npc.Center.X;
                        float idealLocationY = Main.player[npc.target].Bottom.Y - npc.Bottom.Y;
                        if (idealLocationX < 0f && npc.velocity.X > 0f)
                        {
                            npc.velocity.X *= 0.9f;
                        }
                        else if (idealLocationX > 0f && npc.velocity.X < 0f)
                        {
                            npc.velocity.X *= 0.9f;
                        }
                        if (idealLocationX < 0f && npc.velocity.X > -7f)
                        {
                            npc.velocity.X -= 0.12f;
                        }
                        else if (idealLocationX > 0f && npc.velocity.X < 7f)
                        {
                            npc.velocity.X += 0.12f;
                        }
                        if (npc.velocity.X > 8f)
                        {
                            npc.velocity.X = 8f;
                        }
                        if (npc.velocity.X < -8f)
                        {
                            npc.velocity.X = -8f;
                        }
                        if (idealLocationY < -20f && npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y *= 0.8f;
                        }
                        else if (idealLocationY > 20f && npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y *= 0.8f;
                        }
                        if (idealLocationY < -20f && npc.velocity.Y > -7f)
                        {
                            npc.velocity.Y -= 0.35f;
                        }
                        else if (idealLocationY > 20f && npc.velocity.Y < 7f)
                        {
                            npc.velocity.Y += 0.35f;
                        }
                    }
                    if (Main.rand.NextBool(3))
                    {
                        Vector2 position = npc.Center + new Vector2((float)(npc.direction * -14), -8f) - Vector2.One * 4f;
                        Vector2 velocity = new Vector2((float)(npc.direction * -6), 12f) * 0.2f + Utils.RandomVector2(Main.rand, -1f, 1f) * 0.1f;
                        Dust dust = Main.dust[Dust.NewDust(position, 8, 8, 229, velocity.X, velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 0.5f)];
                        dust.noGravity = true;
                        dust.velocity = velocity;
                        dust.customData = npc;
                    }
                    // Adjust velocity based on the other storm drivers who want to pump your face full of bullets
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npcType && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
                        {
                            if (npc.position.X < Main.npc[i].position.X)
                            {
                                npc.velocity.X -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.X += 0.05f;
                            }
                            if (npc.position.Y < Main.npc[i].position.Y)
                            {
                                npc.velocity.Y -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.Y += 0.05f;
                            }
                        }
                    }
                }
                else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -7f;
                    npc.ai[2] = 1f;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.localAI[2]++;
                    bool closeToPlayer = npc.Distance(Main.player[npc.target].Center) < 600f && Math.Abs(npc.SafeDirectionTo(Main.player[npc.target].Center).Y) < 0.5f;
                    if (npc.localAI[2] >= Main.rand.Next(240, CalamityWorld.death ? 300 : 480) && closeToPlayer && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        npc.localAI[2] = 0f;
                        Vector2 spawnPosition = npc.Center + new Vector2(npc.direction * 30f, 2f);
                        Vector2 baseLaserVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center, Vector2.UnitX * npc.direction) * 9f;

                        int damage = Main.expertMode ? 50 : 75;
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 randomizedVelocity = baseLaserVelocity + Utils.RandomVector2(Main.rand, -0.8f, 0.8f);
                            Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition, randomizedVelocity, ProjectileID.VortexLaser, damage, 1f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            else if (npcType == NPCID.VortexHornet)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.ai[2] = 0f;
                    npc.rotation = 0f;
                }
                else
                {
                    npc.rotation = npc.velocity.X * 0.1f;
                }
                if (npc.velocity.Y != 0f && npc.ai[2] == 1f)
                {
                    npc.TargetClosest(true);
                    npc.spriteDirection = -npc.direction;
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        float playerDistX = Main.player[npc.target].Center.X - npc.Center.X;
                        float playerDistY = Main.player[npc.target].Center.Y - npc.Center.Y;
                        if (playerDistX < 0f && npc.velocity.X > 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        else if (playerDistX > 0f && npc.velocity.X < 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        if (playerDistX < -20f && npc.velocity.X > -8f)
                        {
                            npc.velocity.X -= 0.025f;
                        }
                        else if (playerDistX > 20f && npc.velocity.X < 8f)
                        {
                            npc.velocity.X += 0.025f;
                        }
                        if (npc.velocity.X > 8f)
                        {
                            npc.velocity.X = 8f;
                        }
                        if (npc.velocity.X < -8f)
                        {
                            npc.velocity.X = -8f;
                        }
                        if (playerDistY < -20f && npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y *= 0.98f;
                        }
                        else if (playerDistY > 20f && npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y *= 0.98f;
                        }
                        if (playerDistY < -20f && npc.velocity.Y > -8f)
                        {
                            npc.velocity.Y -= 0.25f;
                        }
                        else if (playerDistY > 20f && npc.velocity.Y < 8f)
                        {
                            npc.velocity.Y += 0.25f;
                        }
                    }
                    for (int i = 0; i < Main.npc.Length; i++)
                    {
                        if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npcType && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
                        {
                            if (npc.position.X < Main.npc[i].position.X)
                            {
                                npc.velocity.X -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.X += 0.05f;
                            }
                            if (npc.position.Y < Main.npc[i].position.Y)
                            {
                                npc.velocity.Y -= 0.05f;
                            }
                            else
                            {
                                npc.velocity.Y += 0.05f;
                            }
                        }
                    }
                }
                else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -7f;
                    npc.ai[2] = 1f;
                }
            }
            else if (npcType == NPCID.VortexHornetQueen)
            {
                if (npc.ai[1] > 0f && npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.85f;
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity.Y = -0.4f;
                    }
                }
                if (npc.velocity.Y != 0f)
                {
                    npc.TargetClosest(true);
                    npc.spriteDirection = npc.direction;
                    if (Collision.CanHit(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                    {
                        float distanceToLocationX = Main.player[npc.target].Center.X - (float)(npc.direction * 450) - npc.Center.X;
                        if (distanceToLocationX < 40f && npc.velocity.X > 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        else if (distanceToLocationX > 40f && npc.velocity.X < 0f)
                        {
                            npc.velocity.X *= 0.98f;
                        }
                        if (distanceToLocationX < 40f && npc.velocity.X > -7f)
                        {
                            npc.velocity.X -= 0.25f;
                        }
                        else if (distanceToLocationX > 40f && npc.velocity.X < 7f)
                        {
                            npc.velocity.X += 0.25f;
                        }
                        if (npc.velocity.X > 8f)
                        {
                            npc.velocity.X = 8f;
                        }
                        if (npc.velocity.X < -8f)
                        {
                            npc.velocity.X = -8f;
                        }
                    }
                }
                else if (Main.player[npc.target].Center.Y + 100f < npc.position.Y && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.velocity.Y = -8f;
                }
                for (int i = 0; i < Main.npc.Length; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npcType && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
                    {
                        if (npc.position.X < Main.npc[i].position.X)
                        {
                            npc.velocity.X -= 0.1f;
                        }
                        else
                        {
                            npc.velocity.X += 0.1f;
                        }
                        if (npc.position.Y < Main.npc[i].position.Y)
                        {
                            npc.velocity.Y -= 0.1f;
                        }
                        else
                        {
                            npc.velocity.Y += 0.1f;
                        }
                    }
                }
                if (Main.rand.NextBool(6) && npc.ai[1] <= 20f)
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.Center + new Vector2((float)((npc.spriteDirection == 1) ? 8 : -20), -20f), 8, 8, 229, npc.velocity.X, npc.velocity.Y, 100, default, 1f)];
                    dust.velocity = dust.velocity / 4f + npc.velocity / 2f;
                    dust.scale = 0.6f;
                    dust.noLight = true;
                }
                if (npc.ai[1] >= 57f)
                {
                    int dustType = Utils.SelectRandom<int>(Main.rand, new int[]
                    {
                        161,
                        229
                    });
                    Dust dust = Main.dust[Dust.NewDust(npc.Center + new Vector2((float)((npc.spriteDirection == 1) ? 8 : -20), -20f), 8, 8, dustType, npc.velocity.X, npc.velocity.Y, 100, default, 1f)];
                    dust.velocity = dust.velocity / 4f + npc.SafeDirectionTo(Main.player[npc.target].Top);
                    dust.scale = 1.2f;
                    dust.noLight = true;
                }
                if (Main.rand.NextBool(6))
                {
                    Dust dust = Main.dust[Dust.NewDust(npc.Center, 2, 2, 229, 0f, 0f, 0, default, 1f)];
                    dust.position = npc.Center + new Vector2((float)((npc.spriteDirection == 1) ? 26 : -26), 24f);
                    dust.velocity.X = 0f;
                    if (dust.velocity.Y < 0f)
                    {
                        dust.velocity.Y = 0f;
                    }
                    dust.noGravity = true;
                    dust.scale = 1f;
                    dust.noLight = true;
                }
            }
            else if (npcType == NPCID.SnowFlinx)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.rotation = 0f;
                    npc.localAI[0] = 0f;
                }
                else if (npc.localAI[0] == 1f)
                {
                    npc.rotation += npc.velocity.X * 0.05f;
                }
            }
            else if (npcType == NPCID.VortexLarva)
            {
                if (npc.velocity.Y == 0f)
                {
                    npc.rotation = 0f;
                }
                else
                {
                    npc.rotation += npc.velocity.X * 0.08f;
                }
            }

            // Turn into a bat
            if (npcType == NPCID.Vampire && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.Distance(Main.player[npc.target].Center) > 300f)
                {
                    npc.Transform(NPCID.VampireBat);
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f)
                TryConvertToWallClimber(npc);

            bool prehardmodeSpiders = npc.type == NPCID.WallCreeper || npc.type == NPCID.WallCreeperWall || npc.type == NPCID.BloodCrawler || npc.type == NPCID.BloodCrawlerWall;
            if (Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode && npc.target >= 0 && (npcType == NPCID.BlackRecluse || npcType == NPCID.BlackRecluseWall || npc.type == NPCID.JungleCreeper || npc.type == NPCID.JungleCreeperWall || prehardmodeSpiders) && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
            {
                npc.localAI[0] += 1f;
                if (npc.justHit)
                    npc.localAI[0] = 0f;

                float webSpitGateValue = CalamityWorld.death ? 240f : 390f;
                if (npc.localAI[0] >= webSpitGateValue)
                {
                    npc.localAI[0] = 0f;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * (prehardmodeSpiders ? 6f : 10f), ProjectileID.WebSpit, 18, 0f, Main.myPlayer, 0f, 0f);
                }
            }

            if (npcType == NPCID.IceGolem)
            {
                if (npc.justHit)
                    npc.ai[2] = 0f;

                if (npc.confused)
                    npc.ai[2] = 0f;

                npc.ai[2] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= (CalamityWorld.death ? 60f : 90f) && npc.velocity.Y == 0f && !Main.player[npc.target].dead && !Main.player[npc.target].frozen && ((npc.direction > 0 && npc.Center.X < Main.player[npc.target].Center.X) || (npc.direction < 0 && npc.Center.X > Main.player[npc.target].Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.netUpdate = true;
                    Vector2 velocity = npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * 15;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center - Vector2.UnitY * 28 + velocity * 2f, velocity, ProjectileID.FrostBeam, 32, 0f, Main.myPlayer, 0f, 0f);
                    npc.ai[2] = 0f;
                }
            }

            if (npcType == NPCID.Eyezor)
            {
                if (npc.justHit)
                    npc.ai[2] = 0f;

                if (npc.confused)
                    npc.ai[2] = 0f;

                npc.ai[2] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= (CalamityWorld.death ? 60f : 90f) && npc.velocity.Y == 0f && !Main.player[npc.target].dead && !Main.player[npc.target].frozen && ((npc.direction > 0 && npc.Center.X < Main.player[npc.target].Center.X) || (npc.direction < 0 && npc.Center.X > Main.player[npc.target].Center.X)) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    float speed = 12f;
                    Vector2 spawnPosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + 12f);
                    Vector2 velocity = Vector2.Normalize(Main.player[npc.target].Center - spawnPosition) * speed;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition, velocity, ProjectileID.EyeLaser, 40, 0f, Main.myPlayer, 0f, 0f);
                    npc.ai[2] = 0f;
                }
            }

            if (npcType == NPCID.MartianEngineer)
            {
                if (npc.confused)
                {
                    npc.ai[2] = -40f;
                }
                else
                {
                    if (npc.ai[2] < 40f)
                    {
                        npc.ai[2] += 1f;
                    }
                    if (npc.ai[2] > 0f && NPC.CountNPCS(NPCID.MartianTurret) >= 4 * NPC.CountNPCS(NPCID.MartianEngineer))
                    {
                        npc.ai[2] = 0f;
                    }
                    if (npc.justHit)
                    {
                        npc.ai[2] = -20f;
                    }
                    if (npc.ai[2] == 20f)
                    {
                        int centerTileX = (int)npc.position.X / 16;
                        int centerTileY = (int)npc.position.Y / 16;
                        int centerTileX2 = (int)npc.position.X / 16;
                        int centerTileY2 = (int)npc.position.Y / 16;
                        int maxTurretDistX = 5;
                        int maxTurretDistanceY = 2;
                        int tryCounter = 0;
                        bool createdTurret = false;
                        while (!createdTurret && tryCounter < 100)
                        {
                            tryCounter++;
                            int turretSpawnX = Main.rand.Next(centerTileX - maxTurretDistX, centerTileX + maxTurretDistX);
                            for (int y = Main.rand.Next(centerTileY - maxTurretDistX, centerTileY + maxTurretDistX); y < centerTileY + maxTurretDistX; y++)
                            {
                                if ((y < centerTileY - maxTurretDistanceY || y > centerTileY + maxTurretDistanceY || turretSpawnX < centerTileX - maxTurretDistanceY || turretSpawnX > centerTileX + maxTurretDistanceY) && (y < centerTileY2 || y > centerTileY2 || turretSpawnX < centerTileX2 || turretSpawnX > centerTileX2) && Main.tile[turretSpawnX, y].HasUnactuatedTile)
                                {
                                    bool notLava = true;
                                    if (Main.tile[turretSpawnX, y - 1].LiquidType == LiquidID.Lava)
                                    {
                                        notLava = false;
                                    }
                                    if (notLava && Main.tileSolid[(int)Main.tile[turretSpawnX, y].TileType] && !Collision.SolidTiles(turretSpawnX - 1, turretSpawnX + 1, y - 4, y - 1))
                                    {
                                        int turretIdx = NPC.NewNPC(npc.GetSource_FromAI(), turretSpawnX * 16 - npc.width / 2, y * 16, NPCID.MartianTurret, 0, 0f, 0f, 0f, 0f, 255);
                                        Main.npc[turretIdx].position.Y = (float)(y * 16 - Main.npc[turretIdx].height);
                                        createdTurret = true;
                                        npc.netUpdate = true;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    if (npc.ai[2] == 40f)
                    {
                        npc.ai[2] = -90f;
                    }
                }
            }

            if (npcType == NPCID.GigaZapper)
            {
                if (npc.confused)
                {
                    npc.ai[2] = -40f;
                }
                else
                {
                    if (npc.ai[2] < 20f)
                    {
                        npc.ai[2] += 1f;
                    }
                    if (npc.justHit)
                    {
                        npc.ai[2] = -20f;
                    }
                    if (npc.ai[2] == 20f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[2] = (float)(-10 + Main.rand.Next(3) * -10);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y + 8f, (float)(npc.direction * 8), 0f, ProjectileID.GigaZapperSpear, 25, 1f, Main.myPlayer, 0f, 0f);
                    }
                }
            }

            if (npcType == NPCID.SkeletonArcher ||
                npcType == NPCID.GoblinArcher ||
                npcType == NPCID.IcyMerman ||
                npcType == NPCID.PirateDeadeye ||
                npcType == NPCID.PirateCrossbower ||
                npcType == NPCID.PirateCaptain ||
                npcType == NPCID.Paladin ||
                npcType == NPCID.SkeletonSniper ||
                npcType == NPCID.TacticalSkeleton ||
                npcType == NPCID.SkeletonCommando ||
                npcType == NPCID.ElfArcher ||
                npcType == NPCID.CultistArcherBlue ||
                npcType == NPCID.CultistArcherWhite ||
                npcType == NPCID.BrainScrambler ||
                npcType == NPCID.RayGunner ||
                (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4) ||
                (npcType == NPCID.DrManFly ||
                npcType == NPCID.GreekSkeleton ||
                npcType == NPCID.StardustSoldier ||
                npcType == NPCID.StardustSpiderBig ||
                (npcType >= NPCID.Salamander && npcType <= NPCID.Salamander9)) ||
                npcType == NPCID.NebulaSoldier ||
                npcType == NPCID.VortexHornetQueen ||
                npcType == NPCID.MartianWalker)
            {
                bool npcAllowedToShoot = npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianWalker;
                bool isAlienQueen = npcType == NPCID.VortexHornetQueen;
                bool flag18 = true;
                int num136 = -1;
                int num137 = -1;
                if (npcType == NPCID.StardustSoldier)
                {
                    npcAllowedToShoot = true;
                    num136 = 90;
                    num137 = 90;
                    if (npc.ai[1] <= 150f)
                    {
                        flag18 = false;
                    }
                }
                if (npc.confused)
                {
                    npc.ai[2] = 0f;
                }
                else
                {
                    if (npc.ai[1] > 0f)
                    {
                        npc.ai[1] -= 1f;
                    }
                    if (npc.justHit)
                    {
                        npc.ai[1] = 30f;
                        npc.ai[2] = 0f;
                    }
                    int attackTimeMax = 70;
                    if (npcType == NPCID.CultistArcherBlue || npcType == NPCID.CultistArcherWhite)
                    {
                        attackTimeMax = 80;
                    }
                    if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner)
                    {
                        attackTimeMax = 80;
                    }
                    if (npcType == NPCID.MartianWalker)
                    {
                        attackTimeMax = 15;
                    }
                    if (npcType == NPCID.ElfArcher)
                    {
                        attackTimeMax = 110;
                    }
                    if (npcType == NPCID.SkeletonSniper)
                    {
                        attackTimeMax = 200;
                    }
                    if (npcType == NPCID.TacticalSkeleton)
                    {
                        attackTimeMax = 120;
                    }
                    if (npcType == NPCID.SkeletonCommando)
                    {
                        attackTimeMax = 90;
                    }
                    if (npcType == NPCID.GoblinArcher)
                    {
                        attackTimeMax = 180;
                    }
                    if (npcType == NPCID.IcyMerman)
                    {
                        attackTimeMax = 50;
                    }
                    if (npcType == NPCID.GreekSkeleton)
                    {
                        attackTimeMax = 100;
                    }
                    if (npcType == NPCID.PirateDeadeye)
                    {
                        attackTimeMax = 40;
                    }
                    if (npcType == NPCID.PirateCrossbower)
                    {
                        attackTimeMax = 80;
                    }
                    if (npcType == NPCID.Paladin)
                    {
                        attackTimeMax = 30;
                    }
                    if (npcType == NPCID.StardustSoldier)
                    {
                        attackTimeMax = 300;
                    }
                    if (npcType == NPCID.StardustSpiderBig)
                    {
                        attackTimeMax = 60;
                    }
                    if (npcType == NPCID.NebulaSoldier)
                    {
                        attackTimeMax = 180;
                    }
                    if (npcType == NPCID.VortexHornetQueen)
                    {
                        attackTimeMax = 60;
                    }
                    bool priateCaptainBoost = false;
                    if (npcType == NPCID.PirateCaptain)
                    {
                        if (npc.localAI[2] >= 20f)
                        {
                            priateCaptainBoost = true;
                        }
                        if (priateCaptainBoost)
                        {
                            attackTimeMax = 60;
                        }
                        else
                        {
                            attackTimeMax = 8;
                        }
                    }
                    attackTimeMax = (int)(attackTimeMax * 0.75);
                    int num139 = attackTimeMax / 2;
                    if (npcType == NPCID.NebulaSoldier)
                    {
                        num139 = attackTimeMax - 1;
                    }
                    if (npcType == NPCID.VortexHornetQueen)
                    {
                        num139 = attackTimeMax - 1;
                    }
                    if (npc.ai[2] > 0f)
                    {
                        if (flag18)
                        {
                            npc.TargetClosest(true);
                        }
                        if (npc.ai[1] == (float)num139)
                        {
                            if (npcType == NPCID.PirateCaptain)
                            {
                                npc.localAI[2] += 1f;
                            }
                            float num140 = CalamityWorld.death ? 6f : 11f;
                            if (npcType == NPCID.GoblinArcher)
                            {
                                num140 = CalamityWorld.death ? 5f : 9f;
                            }
                            if (npcType == NPCID.IcyMerman)
                            {
                                num140 = CalamityWorld.death ? 4f : 7f;
                            }
                            if (npcType == NPCID.Paladin)
                            {
                                num140 = CalamityWorld.death ? 5f : 9f;
                            }
                            if (npcType == NPCID.SkeletonCommando)
                            {
                                num140 = CalamityWorld.death ? 2.5f : 4f;
                            }
                            if (npcType == NPCID.PirateDeadeye)
                            {
                                num140 = CalamityWorld.death ? 8f : 14f;
                            }
                            if (npcType == NPCID.PirateCrossbower)
                            {
                                num140 = CalamityWorld.death ? 9f : 16f;
                            }
                            if (npcType == NPCID.RayGunner)
                            {
                                num140 = CalamityWorld.death ? 4f : 7f;
                            }
                            if (npcType == NPCID.MartianWalker)
                            {
                                num140 = CalamityWorld.death ? 5f : 8f;
                            }
                            if (npcType == NPCID.StardustSpiderBig)
                            {
                                num140 = 4f;
                            }
                            if (npcType >= 449 && npcType <= 452)
                            {
                                num140 = CalamityWorld.death ? 4f : 7f;
                            }
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                num140 = CalamityWorld.death ? 5f : 8f;
                            }
                            if (npcType == NPCID.DrManFly)
                            {
                                num140 = CalamityWorld.death ? 4.5f : 7.5f;
                            }
                            if (npcType == NPCID.StardustSoldier)
                            {
                                num140 = 1f;
                            }
                            if (npcType >= 498 && npcType <= 506)
                            {
                                num140 = CalamityWorld.death ? 4f : 7f;
                            }
                            num140 *= 1.25f;
                            Vector2 spawnPosition = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                spawnPosition.Y -= 14f;
                            }
                            if (npcType == NPCID.IcyMerman)
                            {
                                spawnPosition.Y -= 10f;
                            }
                            if (npcType == NPCID.Paladin)
                            {
                                spawnPosition.Y -= 10f;
                            }
                            if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner)
                            {
                                spawnPosition.Y += 6f;
                            }
                            if (npcType == NPCID.MartianWalker)
                            {
                                spawnPosition.Y = npc.position.Y + 20f;
                            }
                            if (npcType >= 498 && npcType <= 506)
                            {
                                spawnPosition.Y -= 8f;
                            }
                            if (npcType == NPCID.VortexHornetQueen)
                            {
                                spawnPosition += new Vector2((float)(npc.spriteDirection * 2), -12f);
                            }
                            float distX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - spawnPosition.X;
                            float num142 = Math.Abs(distX) * 0.1f;
                            if (npcType == NPCID.SkeletonSniper || npcType == NPCID.TacticalSkeleton)
                            {
                                num142 = 0f;
                            }
                            if (npcType == NPCID.PirateCrossbower)
                            {
                                num142 = Math.Abs(distX) * 0.08f;
                            }
                            if (npcType == NPCID.PirateDeadeye || (npcType == NPCID.PirateCaptain && !priateCaptainBoost))
                            {
                                num142 = 0f;
                            }
                            if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner || npcType == NPCID.MartianWalker)
                            {
                                num142 = 0f;
                            }
                            if (npcType >= 449 && npcType <= 452)
                            {
                                num142 = Math.Abs(distX) * (float)Main.rand.Next(10, 50) * 0.01f;
                            }
                            if (npcType == NPCID.DrManFly)
                            {
                                num142 = Math.Abs(distX) * (float)Main.rand.Next(10, 50) * 0.01f;
                            }
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                num142 = Math.Abs(distX) * (float)Main.rand.Next(-10, 11) * 0.0035f;
                            }
                            if (npcType >= 498 && npcType <= 506)
                            {
                                num142 = Math.Abs(distX) * (float)Main.rand.Next(1, 11) * 0.0025f;
                            }
                            float distY = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - spawnPosition.Y - num142;
                            float magnitude = (float)Math.Sqrt((distX * distX + distY * distY));
                            npc.netUpdate = true;
                            magnitude = num140 / magnitude;
                            distX *= magnitude;
                            distY *= magnitude;
                            int damage = 35;
                            int projectileType = ProjectileID.FlamingArrow;
                            if (npcType == NPCID.ElfArcher)
                            {
                                damage = 45;
                            }
                            if (npcType == NPCID.GoblinArcher)
                            {
                                projectileType = ProjectileID.WoodenArrowHostile;
                                damage = 11;
                            }
                            if (npcType == NPCID.CultistArcherBlue || npcType == NPCID.CultistArcherWhite)
                            {
                                projectileType = ProjectileID.WoodenArrowHostile;
                                damage = 40;
                            }
                            if (npcType == NPCID.BrainScrambler)
                            {
                                projectileType = ProjectileID.BrainScramblerBolt;
                                damage = 24;
                            }
                            if (npcType == NPCID.RayGunner)
                            {
                                projectileType = ProjectileID.RayGunnerLaser;
                                damage = 30;
                            }
                            if (npcType == NPCID.MartianWalker)
                            {
                                projectileType = ProjectileID.MartianWalkerLaser;
                                damage = 35;
                            }
                            if (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4)
                            {
                                projectileType = ProjectileID.SkeletonBone;
                                damage = 20;
                            }
                            if (npcType >= NPCID.Salamander && npcType <= NPCID.Salamander9)
                            {
                                projectileType = ProjectileID.SalamanderSpit;
                                damage = 14;
                            }
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                projectileType = ProjectileID.JavelinHostile;
                                damage = 18;
                            }
                            if (npcType == NPCID.IcyMerman)
                            {
                                projectileType = ProjectileID.IcewaterSpit;
                                damage = 37;
                            }
                            if (npcType == NPCID.DrManFly)
                            {
                                projectileType = ProjectileID.DrManFlyFlask;
                                damage = 50;
                            }
                            if (npcType == NPCID.StardustSoldier)
                            {
                                projectileType = ProjectileID.StardustSoldierLaser;
                                damage = (Main.expertMode ? 45 : 60);
                            }
                            if (npcType == NPCID.NebulaSoldier)
                            {
                                projectileType = ProjectileID.NebulaBolt;
                                damage = (Main.expertMode ? 45 : 60);
                            }
                            if (npcType == NPCID.VortexHornetQueen)
                            {
                                projectileType = ProjectileID.VortexAcid;
                                damage = (Main.expertMode ? 45 : 60);
                            }
                            if (npcType == NPCID.SkeletonSniper)
                            {
                                projectileType = ProjectileID.SniperBullet;
                                damage = 100;
                            }
                            if (npcType == NPCID.Paladin)
                            {
                                projectileType = ProjectileID.PaladinsHammerHostile;
                                damage = 60;
                            }
                            if (npcType == NPCID.SkeletonCommando)
                            {
                                projectileType = ProjectileID.RocketSkeleton;
                                damage = 60;
                            }
                            if (npcType == NPCID.PirateDeadeye)
                            {
                                projectileType = ProjectileID.BulletDeadeye;
                                damage = 25;
                            }
                            if (npcType == NPCID.PirateCrossbower)
                            {
                                projectileType = ProjectileID.FlamingArrow;
                                damage = 40;
                            }
                            if (npcType == NPCID.TacticalSkeleton)
                            {
                                damage = 50;
                                projectileType = ProjectileID.BulletDeadeye;
                            }
                            if (npcType == NPCID.PirateCaptain)
                            {
                                projectileType = ProjectileID.BulletDeadeye;
                                damage = 30;
                                if (priateCaptainBoost)
                                {
                                    damage = 100;
                                    projectileType = ProjectileID.CannonballHostile;
                                    npc.localAI[2] = 0f;
                                }
                            }
                            spawnPosition.X += distX;
                            spawnPosition.Y += distY;
                            if (Main.expertMode && npcType == NPCID.Paladin)
                            {
                                damage = (int)(damage * 0.75);
                            }
                            if (Main.expertMode && npcType >= 381 && npcType <= 392)
                            {
                                damage = (int)(damage * 0.8);
                            }
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                if (npcType == NPCID.TacticalSkeleton)
                                {
                                    for (int num147 = 0; num147 < 4; num147++)
                                    {
                                        distX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - spawnPosition.X;
                                        distY = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - spawnPosition.Y;
                                        magnitude = (float)Math.Sqrt((distX * distX + distY * distY));
                                        magnitude = (CalamityWorld.death ? 8f : 12f) / magnitude;
                                        distX += (float)Main.rand.Next(-40, 41);
                                        distY += (float)Main.rand.Next(-40, 41);
                                        distX *= magnitude;
                                        distY *= magnitude;
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, distX, distY, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                                        if (CalamityWorld.death)
                                        {
                                            Main.projectile[proj].extraUpdates += 1;
                                            Main.projectile[proj].timeLeft = 1200;
                                        }
                                    }
                                }
                                else if (npcType == NPCID.StardustSoldier)
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition.X, spawnPosition.Y, distX, distY, projectileType, damage, 0f, Main.myPlayer, 0f, (float)npc.whoAmI);
                                    if (CalamityWorld.death)
                                    {
                                        Main.projectile[proj].extraUpdates += 1;
                                        Main.projectile[proj].timeLeft = 480;
                                    }
                                }
                                else if (npcType == NPCID.NebulaSoldier)
                                {
                                    for (int i = 0; i < 4; i++)
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X - (float)(npc.spriteDirection * 4), npc.Center.Y + 6f, (float)(-3 + 2 * i) * 0.15f, (float)(-(float)Main.rand.Next(0, 3)) * 0.2f - 0.1f, projectileType, damage, 0f, Main.myPlayer, 0f, (float)npc.whoAmI);
                                        if (CalamityWorld.death)
                                        {
                                            Main.projectile[proj].extraUpdates += 1;
                                            Main.projectile[proj].timeLeft = 1200;
                                        }
                                    }
                                }
                                else if (npcType == NPCID.StardustSpiderBig)
                                {
                                    int idx = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.StardustSpiderSmall, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                                    Main.npc[idx].velocity = new Vector2(distX, -6f + distY);
                                }
                                else
                                {
                                    int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition.X, spawnPosition.Y, distX, distY, projectileType, damage, 0f, Main.myPlayer, 0f, 0f);
                                    if (CalamityWorld.death)
                                    {
                                        Main.projectile[proj].extraUpdates += 1;
                                        Main.projectile[proj].timeLeft = 1200;
                                    }
                                }
                            }
                            if (Math.Abs(distY) > Math.Abs(distX) * 2f)
                            {
                                if (distY > 0f)
                                {
                                    npc.ai[2] = 1f;
                                }
                                else
                                {
                                    npc.ai[2] = 5f;
                                }
                            }
                            else if (Math.Abs(distX) > Math.Abs(distY) * 2f)
                            {
                                npc.ai[2] = 3f;
                            }
                            else if (distY > 0f)
                            {
                                npc.ai[2] = 2f;
                            }
                            else
                            {
                                npc.ai[2] = 4f;
                            }
                        }
                        if ((npc.velocity.Y != 0f && !isAlienQueen) || npc.ai[1] <= 0f)
                        {
                            npc.ai[2] = 0f;
                            npc.ai[1] = 0f;
                        }
                        else if (!npcAllowedToShoot || (num136 != -1 && npc.ai[1] >= (float)num136 && npc.ai[1] < (float)(num136 + num137) && (!isAlienQueen || npc.velocity.Y == 0f)))
                        {
                            npc.velocity.X *= 0.9f;
                            npc.spriteDirection = npc.direction;
                        }
                    }

                    if (npcType == NPCID.DrManFly && !Main.eclipse)
                    {
                        npcAllowedToShoot = true;
                    }
                    else if ((npc.ai[2] <= 0f | npcAllowedToShoot) && (npc.velocity.Y == 0f | isAlienQueen) && npc.ai[1] <= 0f && !Main.player[npc.target].dead)
                    {
                        bool canAttack = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        if (npcType == NPCID.MartianWalker)
                        {
                            canAttack = Collision.CanHitLine(npc.Top + new Vector2(0f, 20f), 0, 0, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        }
                        if (Main.player[npc.target].stealth == 0f && Main.player[npc.target].itemAnimation == 0)
                        {
                            canAttack = false;
                        }
                        if (canAttack)
                        {
                            Vector2 vector23 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float distX = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector23.X;
                            float distXAbsolute = Math.Abs(distX) * 0.1f;
                            float distY = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector23.Y - distXAbsolute;
                            distX += (float)Main.rand.Next(-40, 41);
                            distY += (float)Main.rand.Next(-40, 41);
                            float playerDistance = (float)Math.Sqrt(distX * distX + distY * distY);
                            float maxAttackDistance = 700f;
                            if (npcType == NPCID.PirateDeadeye)
                            {
                                maxAttackDistance = 550f;
                            }
                            if (npcType == NPCID.PirateCrossbower)
                            {
                                maxAttackDistance = 800f;
                            }
                            if (npcType >= NPCID.Salamander && npcType <= NPCID.Salamander9)
                            {
                                maxAttackDistance = 190f;
                            }
                            if (npcType >= NPCID.BoneThrowingSkeleton && npcType <= NPCID.BoneThrowingSkeleton4)
                            {
                                maxAttackDistance = 200f;
                            }
                            if (npcType == NPCID.GreekSkeleton)
                            {
                                maxAttackDistance = 400f;
                            }
                            if (npcType == NPCID.DrManFly)
                            {
                                maxAttackDistance = 400f;
                            }
                            if (CalamityWorld.death)
                            {
                                maxAttackDistance *= 1.25f;
                            }
                            if (playerDistance < maxAttackDistance)
                            {
                                npc.netUpdate = true;
                                npc.velocity.X *= 0.5f;
                                playerDistance = 10f / playerDistance;
                                distX *= playerDistance;
                                distY *= playerDistance;
                                npc.ai[2] = 3f;
                                npc.ai[1] = (float)attackTimeMax;
                                if (Math.Abs(distY) > Math.Abs(distX) * 2f)
                                {
                                    if (distY > 0f)
                                    {
                                        npc.ai[2] = 1f;
                                    }
                                    else
                                    {
                                        npc.ai[2] = 5f;
                                    }
                                }
                                else if (Math.Abs(distX) > Math.Abs(distY) * 2f)
                                {
                                    npc.ai[2] = 3f;
                                }
                                else if (distY > 0f)
                                {
                                    npc.ai[2] = 2f;
                                }
                                else
                                {
                                    npc.ai[2] = 4f;
                                }
                            }
                        }
                    }

                    if (npc.ai[2] <= 0f || (npcAllowedToShoot && (num136 == -1 || npc.ai[1] < (float)num136 || npc.ai[1] >= (float)(num136 + num137))))
                    {
                        float maxVelocity = 1f;
                        float acceleration = 0.07f;
                        float decelerationFactor = 0.8f;
                        if (npcType == NPCID.PirateDeadeye)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.09f;
                        }
                        else if (npcType == NPCID.PirateCrossbower)
                        {
                            maxVelocity = 1.5f;
                            acceleration = 0.08f;
                        }
                        else if (npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.5f;
                        }
                        else if (npcType == NPCID.MartianWalker)
                        {
                            maxVelocity = 4f;
                            acceleration = 1f;
                            decelerationFactor = 0.7f;
                        }
                        else if (npcType == NPCID.StardustSoldier)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.5f;
                        }
                        else if (npcType == NPCID.StardustSpiderBig)
                        {
                            maxVelocity = 2f;
                            acceleration = 0.5f;
                        }
                        maxVelocity *= 1.5f;
                        acceleration *= 1.5f;
                        bool forceDeceleration = false;
                        if ((npcType == NPCID.BrainScrambler || npcType == NPCID.RayGunner) && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 300f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            forceDeceleration = true;
                            npc.ai[3] = 0f;
                        }
                        if (npcType == NPCID.MartianWalker && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 400f && Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        {
                            forceDeceleration = true;
                            npc.ai[3] = 0f;
                        }
                        // The extra OR conditional is the reason the special method I created above isn't used.
                        // It has enough parameters. Another one for 1 specific purpose isn't worth it anymore
                        if ((npc.velocity.X < -maxVelocity || npc.velocity.X > maxVelocity) | forceDeceleration)
                        {
                            if (npc.velocity.Y == 0f)
                            {
                                npc.velocity *= decelerationFactor;
                            }
                        }
                        else if (npc.velocity.X < maxVelocity && npc.direction == 1)
                        {
                            npc.velocity.X += acceleration;
                            if (npc.velocity.X > maxVelocity)
                            {
                                npc.velocity.X = maxVelocity;
                            }
                        }
                        else if (npc.velocity.X > -maxVelocity && npc.direction == -1)
                        {
                            npc.velocity.X -= acceleration;
                            if (npc.velocity.X < -maxVelocity)
                            {
                                npc.velocity.X = -maxVelocity;
                            }
                        }
                    }

                    if (npcType == NPCID.MartianWalker)
                    {
                        npc.localAI[2] += 1f;
                        if (npc.localAI[2] >= 6f)
                        {
                            npc.localAI[2] = 0f;
                            npc.localAI[3] = Main.player[npc.target].DirectionFrom(npc.Top + new Vector2(0f, 20f)).ToRotation();
                        }
                    }
                }
            }

            if (npcType == NPCID.Clown && Main.netMode != NetmodeID.MultiplayerClient && !Main.player[npc.target].dead)
            {
                if (npc.justHit)
                    npc.ai[2] = 0f;

                npc.ai[2] += 1f;
                if (npc.ai[2] > (CalamityWorld.death ? 60f : 180f))
                {
                    Vector2 spawnPosition = new Vector2(npc.position.X + (float)npc.width * 0.5f - (float)(npc.direction * 24), npc.position.Y + 4f);
                    int velocityX = 3 * npc.direction;
                    int velocityY = -5;
                    int num160 = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition.X, spawnPosition.Y, velocityX, velocityY, ProjectileID.HappyBomb, 0, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[num160].timeLeft = 300;
                    if (CalamityWorld.death)
                    {
                        Main.projectile[num160].extraUpdates += 1;
                        Main.projectile[num160].timeLeft = 600;
                    }
                    npc.ai[2] = 0f;
                }
            }

            bool canOpenDoors = false;
            if (npc.velocity.Y == 0f)
            {
                int j = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
                int npcLeft = (int)npc.position.X / 16;
                int npcRight = (int)(npc.position.X + (float)npc.width) / 16;
                for (int i = npcLeft; i <= npcRight; i++)
                {
                    if (Main.tile[i, j].HasUnactuatedTile && Main.tileSolid[Main.tile[i, j].TileType])
                    {
                        canOpenDoors = true;
                        break;
                    }
                }
            }

            if (npcType == NPCID.VortexLarva)
            {
                canOpenDoors = false;
            }

            if (npc.velocity.Y >= 0f)
            {
                int velocitySign = 0;
                if (npc.velocity.X < 0f)
                {
                    velocitySign = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    velocitySign = 1;
                }
                Vector2 positionDelta = npc.position;
                positionDelta.X += npc.velocity.X;
                int x = (int)((positionDelta.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * velocitySign)) / 16f);
                int y = (int)((positionDelta.Y + (float)npc.height - 1f) / 16f);
                if (x * 16 < positionDelta.X + (float)npc.width &&
                    (x + 1) * 16 > positionDelta.X && ((Main.tile[x, y].HasUnactuatedTile &&
                    !Main.tile[x, y].TopSlope && !Main.tile[x, y - 1].TopSlope &&
                    Main.tileSolid[(int)Main.tile[x, y].TileType] &&
                    !Main.tileSolidTop[(int)Main.tile[x, y].TileType]) ||
                    (Main.tile[x, y - 1].IsHalfBlock && Main.tile[x, y - 1].HasUnactuatedTile)) &&
                    (!Main.tile[x, y - 1].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 1].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 1].TileType] ||
                    (Main.tile[x, y - 1].IsHalfBlock &&
                    (!Main.tile[x, y - 4].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 4].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 4].TileType]))) &&
                    (!Main.tile[x, y - 2].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 2].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 2].TileType]) &&
                    (!Main.tile[x, y - 3].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x, y - 3].TileType] ||
                    Main.tileSolidTop[(int)Main.tile[x, y - 3].TileType]) &&
                    (!Main.tile[x - velocitySign, y - 3].HasUnactuatedTile ||
                    !Main.tileSolid[(int)Main.tile[x - velocitySign, y - 3].TileType]))
                {
                    float yAdjust = y * 16f;
                    if (Main.tile[x, y].IsHalfBlock)
                    {
                        yAdjust += 8f;
                    }
                    if (Main.tile[x, y - 1].IsHalfBlock)
                    {
                        yAdjust -= 8f;
                    }
                    if (yAdjust < positionDelta.Y + (float)npc.height)
                    {
                        float gfxOffRelativeToDelta = positionDelta.Y + (float)npc.height - yAdjust;
                        float yOffsetFloor = 16.1f;
                        if (npcType == NPCID.BlackRecluse || npcType == NPCID.WallCreeper || npcType == NPCID.JungleCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.DesertScorpionWalk)
                        {
                            yOffsetFloor += 8f;
                        }
                        if (gfxOffRelativeToDelta <= yOffsetFloor)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - yAdjust;
                            npc.position.Y = yAdjust - (float)npc.height;
                            if (gfxOffRelativeToDelta < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }

            if (canOpenDoors)
            {
                int x = (int)((npc.Center.X + (float)(15 * npc.direction)) / 16f);
                int y = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (npcType == NPCID.Clown || npcType == NPCID.BlackRecluse || npcType == NPCID.WallCreeper || npcType == NPCID.LihzahrdCrawler || npcType == NPCID.JungleCreeper || npcType == NPCID.BloodCrawler || npcType == NPCID.AnomuraFungus || npcType == NPCID.MushiLadybug || npcType == NPCID.Paladin || npcType == NPCID.Scutlix || npcType == NPCID.VortexRifleman || npcType == NPCID.VortexHornet || npcType == NPCID.VortexHornetQueen || npcType == NPCID.WalkingAntlion || npcType == NPCID.SolarDrakomire || npcType == NPCID.DesertScorpionWalk || npcType == NPCID.DesertBeast)
                {
                    x = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 16) * npc.direction)) / 16f);
                }
                if ((Main.tile[x, y - 1].HasUnactuatedTile &&
                    (Main.tile[x, y - 1].TileType == TileID.ClosedDoor ||
                    Main.tile[x, y - 1].TileType == TileID.TallGateClosed)) & reset)
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        npc.velocity.X = -0.5f * npc.direction;
                        int timerIncrement = 5;
                        if (Main.tile[x, y - 1].TileType == TileID.TallGateClosed)
                        {
                            timerIncrement = 2;
                        }
                        npc.ai[1] += timerIncrement;
                        // Special increments
                        if (npcType == NPCID.GoblinThief)
                        {
                            npc.ai[1] += 1f;
                        }
                        if (npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet)
                        {
                            npc.ai[1] += 6f;
                        }
                        npc.ai[2] = 0f;
                        bool readyToOpenDoor = false;
                        if (npc.ai[1] >= 10f)
                        {
                            readyToOpenDoor = true;
                            npc.ai[1] = 10f;
                        }
                        if (npcType == NPCID.Butcher)
                        {
                            readyToOpenDoor = true;
                        }
                        WorldGen.KillTile(x, y - 1, true, false, false);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !readyToOpenDoor) && readyToOpenDoor && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npcType == NPCID.GoblinPeon)
                            {
                                WorldGen.KillTile(x, y - 1, false, false, false);
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, (float)x, (float)(y - 1), 0f, 0, 0, 0);
                                }
                            }
                            else
                            {
                                if (Main.tile[x, y - 1].TileType == TileID.ClosedDoor)
                                {
                                    bool flag24 = WorldGen.OpenDoor(x, y - 1, npc.direction);
                                    if (!flag24)
                                    {
                                        npc.ai[3] = (float)aiGateValue;
                                        npc.netUpdate = true;
                                    }
                                    if (Main.netMode == NetmodeID.Server & flag24)
                                    {
                                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, (float)x, (float)(y - 1), (float)npc.direction, 0, 0, 0);
                                    }
                                }
                                if (Main.tile[x, y - 1].TileType == TileID.TallGateClosed)
                                {
                                    bool flag25 = WorldGen.ShiftTallGate(x, y - 1, false);
                                    if (!flag25)
                                    {
                                        npc.ai[3] = (float)aiGateValue;
                                        npc.netUpdate = true;
                                    }
                                    if (Main.netMode == NetmodeID.Server & flag25)
                                    {
                                        NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, (float)x, (float)(y - 1), 0f, 0, 0, 0);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    int alteredDirection = npc.spriteDirection;
                    if (npcType == NPCID.VortexRifleman)
                    {
                        alteredDirection *= -1;
                    }
                    if ((npc.velocity.X < 0f && alteredDirection == -1) || (npc.velocity.X > 0f && alteredDirection == 1))
                    {
                        if (npc.height >= 32 && Main.tile[x, y - 2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y - 2].TileType])
                        {
                            if (Main.tile[x, y - 3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y - 3].TileType])
                            {
                                npc.velocity.Y = -9f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.velocity.Y = -8f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (Main.tile[x, y - 1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[x, y - 1].TileType])
                        {
                            npc.velocity.Y = -7f;
                            npc.netUpdate = true;
                        }
                        else if (npc.position.Y + (float)npc.height - (float)(y * 16) > 20f && Main.tile[x, y].HasUnactuatedTile && !Main.tile[x, y].TopSlope && Main.tileSolid[(int)Main.tile[x, y].TileType])
                        {
                            npc.velocity.Y = -6f;
                            npc.netUpdate = true;
                        }
                        else if (npc.directionY < 0 && npcType != 67 && (!Main.tile[x, y + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[x, y + 1].TileType]) && (!Main.tile[x + npc.direction, y + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[x + npc.direction, y + 1].TileType]))
                        {
                            npc.velocity.Y = -9f;
                            npc.velocity.X *= 2f;
                            npc.netUpdate = true;
                        }
                        else if (reset)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                        if ((npc.velocity.Y == 0f & jump) && npc.ai[3] == 1f)
                        {
                            npc.velocity.Y = -6f;
                        }
                    }
                    if ((npcType == NPCID.AngryBones || npcType == NPCID.AngryBonesBig || npcType == NPCID.AngryBonesBigMuscle || npcType == NPCID.AngryBonesBigHelmet || npcType == NPCID.CorruptBunny || npcType == NPCID.ArmoredSkeleton || npcType == NPCID.Werewolf || npcType == NPCID.CorruptPenguin || npcType == NPCID.Nymph || npcType == NPCID.GrayGrunt || npcType == NPCID.GigaZapper || npcType == NPCID.CrimsonBunny || npcType == NPCID.CrimsonPenguin || (npcType >= 524 && npcType <= 527)) && npc.velocity.Y == 0f && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 100f && Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 50f && ((npc.direction > 0 && npc.velocity.X >= 1f) || (npc.direction < 0 && npc.velocity.X <= -1f)))
                    {
                        npc.velocity.X *= 3f;
                        if (npc.velocity.X > 4f)
                        {
                            npc.velocity.X = 4f;
                        }
                        if (npc.velocity.X < -4f)
                        {
                            npc.velocity.X = -4f;
                        }
                        npc.velocity.Y = -5f;
                        npc.netUpdate = true;
                    }
                    if ((npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>()) && npc.velocity.Y < 0f)
                    {
                        npc.velocity.Y *= 1.1f;
                    }
                    if (npcType == NPCID.BoneLee && npc.velocity.Y == 0f && Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2))) < 150f && Math.Abs(npc.position.Y + (float)(npc.height / 2) - (Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2))) < 50f && ((npc.direction > 0 && npc.velocity.X >= 1f) || (npc.direction < 0 && npc.velocity.X <= -1f)))
                    {
                        npc.velocity.X = (float)(9 * npc.direction);
                        npc.velocity.Y = -5f;
                        npc.netUpdate = true;
                    }
                    if (npcType == NPCID.BoneLee && npc.velocity.Y < 0f)
                    {
                        npc.velocity.X *= 1.25f;
                        npc.velocity.Y *= 1.15f;
                    }
                    if (npcType == NPCID.Butcher && npc.velocity.Y < 0f)
                    {
                        npc.velocity.X *= 1.35f;
                        npc.velocity.Y *= 1.15f;
                    }
                }
            }
            else if (reset)
            {
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }

            // Teleport (Chaos elementals)
            if (Main.netMode != NetmodeID.MultiplayerClient && (npcType == NPCID.ChaosElemental || npcType == ModContent.NPCType<RenegadeWarlock>()) && npc.ai[3] >= (float)aiGateValue)
            {
                int tileCoordsX = (int)Main.player[npc.target].position.X / 16;
                int tileCoordsY = (int)Main.player[npc.target].position.Y / 16;
                int tileCoordsX2 = (int)npc.position.X / 16;
                int tileCoordsY2 = (int)npc.position.Y / 16;
                int maxDistanceX = 20;
                int tryCount = 0;
                bool tooFarFromPlayer = false;
                if (Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
                {
                    tryCount = 100;
                    tooFarFromPlayer = true;
                }
                while (!tooFarFromPlayer && tryCount < 100)
                {
                    tryCount++;
                    int randX = Main.rand.Next(tileCoordsX - maxDistanceX, tileCoordsX + maxDistanceX);
                    for (int num181 = Main.rand.Next(tileCoordsY - maxDistanceX, tileCoordsY + maxDistanceX); num181 < tileCoordsY + maxDistanceX; num181++)
                    {
                        if ((num181 < tileCoordsY - 4 || num181 > tileCoordsY + 4 || randX < tileCoordsX - 4 || randX > tileCoordsX + 4) && (num181 < tileCoordsY2 - 1 || num181 > tileCoordsY2 + 1 || randX < tileCoordsX2 - 1 || randX > tileCoordsX2 + 1) && Main.tile[randX, num181].HasUnactuatedTile)
                        {
                            bool foundGoodTeleport = true;
                            // I don't understand why the hell this exists if it's only for Chaos Elementals, but I suppose I'll
                            // Leave it here
                            if (npcType == NPCID.DarkCaster && Main.tile[randX, num181 - 1].WallType == WallID.None)
                            {
                                foundGoodTeleport = false;
                            }
                            else if (Main.tile[randX, num181 - 1].LiquidType == LiquidID.Lava)
                            {
                                foundGoodTeleport = false;
                            }
                            if (foundGoodTeleport && Main.tileSolid[(int)Main.tile[randX, num181].TileType] && !Collision.SolidTiles(randX - 1, randX + 1, num181 - 4, num181 - 1))
                            {
                                npc.position.X = (float)(randX * 16 - npc.width / 2);
                                npc.position.Y = (float)(num181 * 16 - npc.height);
                                npc.netUpdate = true;
                                npc.ai[3] = -30f;
                            }
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Flying AI
        public static bool BuffedFlyingAI(NPC npc, Mod mod)
        {
            if (npc.target < 0 || npc.target <= Main.maxPlayers || Main.player[npc.target].dead)
                npc.TargetClosest();

            if (npc.type == NPCID.BloodSquid)
            {
                if (Main.dayTime)
                {
                    npc.velocity.Y -= 0.3f;
                    npc.EncourageDespawn(60);
                }

                npc.position += npc.netOffset;
                if (npc.alpha == 255)
                {
                    npc.spriteDirection = npc.direction;
                    npc.velocity.Y = -6f;
                    for (int i = 0; i < 35; i++)
                    {
                        Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                        dust.velocity *= 1f;
                        dust.scale = 1f + Main.rand.NextFloat() * 0.5f;
                        dust.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                        dust.velocity += npc.velocity * 0.5f;
                    }
                }

                npc.alpha -= 15;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                if (npc.alpha != 0)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                        dust2.velocity *= 1f;
                        dust2.scale = 1f + Main.rand.NextFloat() * 0.5f;
                        dust2.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                        dust2.velocity += npc.velocity * 0.3f;
                    }
                }

                npc.position -= npc.netOffset;
            }

            NPCAimedTarget targetData = npc.GetTargetData();
            bool targetDead = false;
            if (targetData.Type == NPCTargetType.Player)
                targetDead = Main.player[npc.target].dead;

            float num = 6f;
            float num2 = 0.05f;
            if (npc.type == NPCID.EaterofSouls || npc.type == NPCID.Crimera)
            {
                num = 4f;
                num2 = 0.035f;
            }
            else if (npc.type == NPCID.Corruptor)
            {
                num = 4.2f;
                num2 = 0.022f;
            }
            else if (npc.type == NPCID.BloodSquid)
            {
                num = 6f;
                num2 = 0.1f;
            }
            else if (npc.type == NPCID.Hornet || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy))
            {
                num = 3.5f;
                num2 = 0.021f;
                if (npc.type == NPCID.HornetFatty)
                {
                    num = 3f;
                    num2 = 0.017f;
                }

                num *= 1f - npc.scale;
                num2 *= 1f - npc.scale;
                if ((double)(npc.position.Y / 16f) < Main.worldSurface)
                {
                    if (Main.player[npc.target].position.Y - npc.position.Y > 300f && npc.velocity.Y < 0f)
                        npc.velocity.Y *= 0.97f;

                    if (Main.player[npc.target].position.Y - npc.position.Y < 80f && npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.97f;
                }
            }
            else if (npc.type == NPCID.MossHornet)
            {
                num = 4f;
                num2 = 0.017f;
            }
            else if (npc.type == NPCID.Parrot)
            {
                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    num = 6f;
                    num2 = 0.1f;
                }
                else
                {
                    num2 = 0.01f;
                    num = 2f;
                }
            }
            else if (npc.type == NPCID.Moth)
            {
                num = 7f;
                num2 = 0.06f;
            }
            else if (npc.type == NPCID.MeteorHead)
            {
                num = 2f;
                num2 = 0.05f;
            }
            else if (npc.type == NPCID.ServantofCthulhu)
            {
                num = 5f;
                num2 = 0.03f;
            }
            else if (npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall)
            {
                npc.ai[1] += 1f;
                float originalFlyAwayTime = 60f;
                float flyAwayTime = CalamityWorld.death ? 30f : 40f;
                float originalFlyAwayVelocity = 6f;
                float flyAwayDistance = originalFlyAwayTime * originalFlyAwayVelocity;
                float flyAwayVelocity = flyAwayDistance / flyAwayTime;
                float num3 = (npc.ai[1] - flyAwayTime) / flyAwayTime;
                if (num3 > 1f)
                {
                    num3 = 1f;
                }
                else
                {
                    if (npc.velocity.X > flyAwayVelocity)
                        npc.velocity.X = flyAwayVelocity;

                    if (npc.velocity.X < -flyAwayVelocity)
                        npc.velocity.X = -flyAwayVelocity;

                    if (npc.velocity.Y > flyAwayVelocity)
                        npc.velocity.Y = flyAwayVelocity;

                    if (npc.velocity.Y < -flyAwayVelocity)
                        npc.velocity.Y = -flyAwayVelocity;
                }

                num = 5f;
                num2 = 0.1f;
                num2 *= num3;
            }
            num *= 1.25f;
            num2 *= 1.25f;
            if (CalamityWorld.death)
            {
                num *= 1.25f;
                num2 *= 1.25f;
            }
            Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num4 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num5 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);
            num4 = (float)((int)(num4 / 8f) * 8);
            num5 = (float)((int)(num5 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num4 -= vector.X;
            num5 -= vector.Y;
            float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
            float num7 = num6;
            bool flag = false;
            if (num6 > 600f)
            {
                flag = true;
            }
            if (num6 == 0f)
            {
                num4 = npc.velocity.X;
                num5 = npc.velocity.Y;
            }
            else
            {
                num6 = num / num6;
                num4 *= num6;
                num5 *= num6;
            }
            if (npc.type == NPCID.Hornet || npc.type == NPCID.MossHornet || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy) || npc.type == NPCID.EaterofSouls || npc.type == NPCID.Corruptor || npc.type == NPCID.Probe || npc.type == NPCID.Crimera || npc.type == NPCID.Moth || npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall || npc.type == NPCID.BloodSquid)
            {
                if (num7 > 100f || npc.type == NPCID.Corruptor || npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall || npc.type == NPCID.BloodSquid || npc.type == NPCID.Hornet || npc.type == NPCID.MossHornet || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy))
                {
                    npc.ai[0] += 1f;
                    if (npc.ai[0] > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + 0.03f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.03f;
                    }
                    if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                    {
                        npc.velocity.X = npc.velocity.X + 0.03f;
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - 0.03f;
                    }
                    if (npc.ai[0] > 200f)
                    {
                        npc.ai[0] = -200f;
                    }
                }
                if (num7 < 150f && (npc.type == NPCID.EaterofSouls || npc.type == NPCID.Corruptor || npc.type == NPCID.Crimera || npc.type == NPCID.BloodSquid))
                {
                    npc.velocity.X = npc.velocity.X + num4 * 0.009f;
                    npc.velocity.Y = npc.velocity.Y + num5 * 0.009f;
                }
            }
            if (targetDead)
            {
                num4 = (float)npc.direction * num / 2f;
                num5 = -num / 2f;
            }
            if (npc.velocity.X < num4)
            {
                npc.velocity.X = npc.velocity.X + num2;
                if (npc.type != NPCID.Crimera && npc.type != NPCID.EaterofSouls && npc.type != NPCID.Corruptor && npc.type != NPCID.Probe && npc.type != NPCID.BloodSquid && npc.velocity.X < 0f && num4 > 0f)
                {
                    npc.velocity.X = npc.velocity.X + num2;
                }
            }
            else if (npc.velocity.X > num4)
            {
                npc.velocity.X = npc.velocity.X - num2;
                if (npc.type != NPCID.Crimera && npc.type != NPCID.EaterofSouls && npc.type != NPCID.Corruptor && npc.type != NPCID.Probe && npc.type != NPCID.BloodSquid && npc.velocity.X > 0f && num4 < 0f)
                {
                    npc.velocity.X = npc.velocity.X - num2;
                }
            }
            if (npc.velocity.Y < num5)
            {
                npc.velocity.Y = npc.velocity.Y + num2;
                if (npc.type != NPCID.Crimera && npc.type != NPCID.EaterofSouls && npc.type != NPCID.Corruptor && npc.type != NPCID.Probe && npc.type != NPCID.BloodSquid && npc.velocity.Y < 0f && num5 > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num2;
                }
            }
            else if (npc.velocity.Y > num5)
            {
                npc.velocity.Y = npc.velocity.Y - num2;
                if (npc.type != NPCID.Crimera && npc.type != NPCID.EaterofSouls && npc.type != NPCID.Corruptor && npc.type != NPCID.Probe && npc.type != NPCID.BloodSquid && npc.velocity.Y > 0f && num5 < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num2;
                }
            }
            if (npc.type == NPCID.MeteorHead)
            {
                if (num4 > 0f)
                {
                    npc.spriteDirection = 1;
                    npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
                }
                else if (num4 < 0f)
                {
                    npc.spriteDirection = -1;
                    npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + MathHelper.Pi;
                }
            }
            else if (npc.type == NPCID.Probe)
            {
                if (npc.justHit)
                    npc.localAI[0] = 0f;

                npc.localAI[0] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.localAI[0] >= 120f)
                {
                    npc.localAI[0] = 0f;
                    if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        int num8 = 22;
                        int num9 = 84;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), vector.X, vector.Y, num4, num5, num9, num8, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                int arg_A81_0 = (int)npc.position.X + npc.width / 2;
                int num10 = (int)npc.position.Y + npc.height / 2;
                int arg_A8B_0 = arg_A81_0 / 16;
                num10 /= 16;
                if (!WorldGen.SolidTile(arg_A8B_0, num10))
                {
                    Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.3f, 0.1f, 0.05f);
                }
                if (num4 > 0f)
                {
                    npc.spriteDirection = 1;
                    npc.rotation = (float)Math.Atan2((double)num5, (double)num4);
                }
                if (num4 < 0f)
                {
                    npc.spriteDirection = -1;
                    npc.rotation = (float)Math.Atan2((double)num5, (double)num4) + MathHelper.Pi;
                }
            }
            else if (npc.type == NPCID.EaterofSouls || npc.type == NPCID.Corruptor || npc.type == NPCID.Crimera || npc.type == NPCID.BloodSquid)
            {
                npc.rotation = (float)Math.Atan2((double)num5, (double)num4) - MathHelper.PiOver2;
            }
            else if (npc.type == NPCID.Moth || npc.type == NPCID.Hornet || npc.type == NPCID.MossHornet || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy))
            {
                if (npc.velocity.X > 0f)
                {
                    npc.spriteDirection = 1;
                }
                if (npc.velocity.X < 0f)
                {
                    npc.spriteDirection = -1;
                }
                npc.rotation = npc.velocity.X * 0.1f;
            }
            else
            {
                npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) - MathHelper.PiOver2;
            }
            if (npc.type == NPCID.Hornet || npc.type == NPCID.MossHornet || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy) || npc.type == NPCID.EaterofSouls || npc.type == NPCID.MeteorHead || npc.type == NPCID.Corruptor || npc.type == NPCID.Probe || npc.type == NPCID.Crimera || npc.type == NPCID.Moth || npc.type == NPCID.Bee || npc.type == NPCID.BeeSmall || npc.type == NPCID.BloodSquid)
            {
                float num11 = 0.7f;
                if (npc.type == NPCID.EaterofSouls || npc.type == NPCID.Crimera)
                {
                    num11 = 0.4f;
                }
                if (npc.collideX)
                {
                    npc.netUpdate = true;
                    npc.velocity.X = npc.oldVelocity.X * -num11;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                    {
                        npc.velocity.X = 2f;
                    }
                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                if (npc.collideY)
                {
                    npc.netUpdate = true;
                    npc.velocity.Y = npc.oldVelocity.Y * -num11;
                    if (npc.velocity.Y > 0f && (double)npc.velocity.Y < 1.5)
                    {
                        npc.velocity.Y = 2f;
                    }
                    if (npc.velocity.Y < 0f && (double)npc.velocity.Y > -1.5)
                    {
                        npc.velocity.Y = -2f;
                    }
                }
                if (npc.type == NPCID.BloodSquid)
                {
                    int num13 = Dust.NewDust(npc.position, npc.width, npc.height, 5, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100);
                    Main.dust[num13].velocity *= 0.5f;
                }
                else if (npc.type == NPCID.MeteorHead)
                {
                    int num12 = Dust.NewDust(new Vector2(npc.position.X - npc.velocity.X, npc.position.Y - npc.velocity.Y), npc.width, npc.height, 6, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 2f);
                    Dust dust = Main.dust[num12];
                    dust.noGravity = true;
                    dust.velocity.X *= 0.3f;
                    dust.velocity.Y *= 0.3f;
                }
                else if (npc.type != NPCID.Probe && npc.type != NPCID.Moth && npc.type != NPCID.Parrot && npc.type != NPCID.Bee && npc.type != NPCID.BeeSmall && Main.rand.NextBool(20))
                {
                    int num13 = 18;
                    if (npc.type == NPCID.Crimera)
                    {
                        num13 = 5;
                    }
                    int num14 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), num13, npc.velocity.X, 2f, 75, npc.color, npc.scale);
                    Dust dust = Main.dust[num14];
                    dust.velocity.X *= 0.5f;
                    dust.velocity.Y *= 0.1f;
                }
            }
            else if (npc.type != NPCID.Parrot && Main.rand.NextBool(40))
            {
                int num15 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + (float)npc.height * 0.25f), npc.width, (int)((float)npc.height * 0.5f), 5, npc.velocity.X, 2f, 0, default(Color), 1f);
                Dust dust = Main.dust[num15];
                dust.velocity.X *= 0.5f;
                dust.velocity.Y *= 0.1f;
            }
            if ((npc.type == NPCID.EaterofSouls || npc.type == NPCID.Corruptor || npc.type == NPCID.Crimera || npc.type == NPCID.BloodSquid) && npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
                npc.velocity.Y = npc.velocity.Y - 0.4f;
                if (npc.velocity.Y < -3f)
                {
                    npc.velocity.Y = -3f;
                }
            }
            if (npc.type == NPCID.Moth && npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
                npc.velocity.Y = npc.velocity.Y - 0.7f;
                if (npc.velocity.Y < -6f)
                {
                    npc.velocity.Y = -6f;
                }
                npc.TargetClosest(true);
            }
            if (npc.type == NPCID.Hornet || npc.type == NPCID.MossHornet || (npc.type >= NPCID.HornetFatty && npc.type <= NPCID.HornetStingy))
            {
                if (npc.wet)
                {
                    if (npc.velocity.Y > 0f)
                        npc.velocity.Y *= 0.95f;

                    npc.velocity.Y -= 0.5f;
                    if (npc.velocity.Y < -4f)
                        npc.velocity.Y = -4f;

                    npc.TargetClosest();
                }

                if (npc.ai[1] == 101f)
                {
                    SoundEngine.PlaySound(SoundID.Item17, npc.position);
                    npc.ai[1] = 0f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] += (npc.type == NPCID.MossHornet ? 2f : 1f) * 0.1f * npc.scale;
                    if (npc.justHit)
                        npc.ai[1] = 0f;

                    if (targetData.Type == NPCTargetType.Player)
                    {
                        Player player = Main.player[npc.target];
                        if (player != null && player.stealth == 0f && player.itemAnimation == 0)
                            npc.ai[1] = 0f;
                    }

                    if (npc.ai[1] >= 240f)
                    {
                        if (targetData.Type != 0 && Collision.CanHit(npc, targetData))
                        {
                            float num17 = (CalamityWorld.death || Main.hardMode) ? 5f : 8f;
                            Vector2 vector2 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height / 2);
                            float num18 = targetData.Center.X - vector2.X;
                            float num19 = targetData.Center.Y - vector2.Y;
                            if ((num18 < 0f && npc.velocity.X < 0f) || (num18 > 0f && npc.velocity.X > 0f))
                            {
                                float num20 = (float)Math.Sqrt(num18 * num18 + num19 * num19);
                                num20 = num17 / num20;
                                num18 *= num20;
                                num19 *= num20;
                                int num21 = (int)(10f * npc.scale);
                                if (npc.type == NPCID.MossHornet)
                                    num21 = (int)(30f * npc.scale);

                                int num22 = ProjectileID.Stinger;
                                int num23 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector2.X, vector2.Y, num18, num19, num22, num21, 0f, Main.myPlayer);
                                Main.projectile[num23].timeLeft = (CalamityWorld.death || Main.hardMode) ? 600 : 300;
                                Main.projectile[num23].extraUpdates += (CalamityWorld.death || Main.hardMode) ? 1 : 0;
                                npc.ai[1] = 101f;
                                npc.netUpdate = true;
                            }
                            else
                                npc.ai[1] = 0f;
                        }
                        else
                            npc.ai[1] = 0f;
                    }
                }
            }
            if (npc.type == NPCID.Probe & flag)
            {
                if ((npc.velocity.X > 0f && num4 > 0f) || (npc.velocity.X < 0f && num4 < 0f))
                {
                    if (Math.Abs(npc.velocity.X) < 12f)
                    {
                        npc.velocity.X = npc.velocity.X * 1.05f;
                    }
                }
                else
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && !targetDead)
            {
                if (Main.getGoodWorld && npc.type == NPCID.EaterofSouls)
                {
                    if (NPC.AnyNPCs(NPCID.EaterofWorldsHead))
                    {
                        if (npc.justHit)
                            npc.localAI[0] = 0f;

                        npc.localAI[0] += 1f;
                        if (npc.localAI[0] == 60f)
                        {
                            if (targetData.Type != 0 && Collision.CanHit(npc, targetData))
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), NPCID.VileSpitEaterOfWorlds);

                            npc.localAI[0] = 0f;
                        }
                    }
                }

                if (npc.type == NPCID.Corruptor)
                {
                    if (npc.justHit)
                        npc.localAI[0] = 0f;

                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] == 180f)
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2) + npc.velocity.X), (int)(npc.position.Y + (float)(npc.height / 2) + npc.velocity.Y), 112, 0, 0f, 0f, 0f, 0f, 255);
                        }
                        npc.localAI[0] = 0f;
                    }
                }

                if (npc.type == NPCID.BloodSquid)
                {
                    if (npc.justHit)
                        npc.localAI[0] = 0f;

                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        if (targetData.Type != 0 && Collision.CanHit(npc, targetData))
                        {
                            if ((npc.Center - targetData.Center).Length() < 400f)
                            {
                                Vector2 vector3 = npc.DirectionTo(new Vector2(targetData.Center.X, targetData.Position.Y));
                                npc.velocity = -vector3 * 5f;
                                npc.netUpdate = true;
                                npc.localAI[0] = 0f;
                                vector3 = npc.DirectionTo(new Vector2(targetData.Center.X, targetData.Position.Y));
                                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vector3 * (CalamityWorld.death ? 6f : 10f), ProjectileID.BloodShot, 50, 1f, Main.myPlayer);
                                if (CalamityWorld.death)
                                {
                                    Main.projectile[proj].extraUpdates += 1;
                                    Main.projectile[proj].timeLeft = 1200;
                                }
                            }
                            else
                            {
                                npc.localAI[0] = 50f;
                            }
                        }
                        else
                        {
                            npc.localAI[0] = 50f;
                        }
                    }
                }
            }
            if ((Main.dayTime && npc.type != NPCID.Crimera && npc.type != NPCID.EaterofSouls && npc.type != NPCID.MeteorHead && npc.type != NPCID.Bee && npc.type != NPCID.BeeSmall && npc.type != NPCID.Corruptor && npc.type != NPCID.Moth && npc.type != NPCID.Parrot && npc.type != NPCID.BloodSquid) || Main.player[npc.target].dead)
            {
                npc.velocity.Y = npc.velocity.Y - num2 * 2f;
                if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
            }
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }
            return false;
        }
        #endregion

        #region Worm AI
        public static bool BuffedWormAI(NPC npc, Mod mod)
        {
            if (npc.type == NPCID.LeechHead && npc.localAI[1] == 0f)
            {
                npc.localAI[1] = 1f;
                SoundEngine.PlaySound(SoundID.NPCDeath13, npc.position);
                int num = 1;
                if (npc.velocity.X < 0f)
                {
                    num = -1;
                }
                for (int i = 0; i < 20; i++)
                {
                    Dust.NewDust(new Vector2(npc.position.X - 20f, npc.position.Y - 20f), npc.width + 40, npc.height + 40, 5, (float)(num * 8), -1f, 0, default(Color), 1f);
                }
            }

            if (npc.type >= NPCID.BloodEelHead && npc.type <= NPCID.BloodEelTail)
            {
                npc.position += npc.netOffset;
                npc.dontTakeDamage = (npc.alpha > 0);
                if (npc.type == NPCID.BloodEelHead || (npc.type != NPCID.BloodEelHead && Main.npc[(int)npc.ai[1]].alpha < 85))
                {
                    if (npc.dontTakeDamage)
                    {
                        for (int k = 0; k < 2; k++)
                        {
                            Dust.NewDust(npc.position, npc.width, npc.height, 5, 0f, 0f, 100);
                        }
                    }

                    npc.alpha -= 42;
                    if (npc.alpha < 0)
                        npc.alpha = 0;
                }

                if (npc.alpha == 0 && Main.rand.NextBool(5))
                    Dust.NewDust(npc.position, npc.width, npc.height, 5, 0f, 0f, 100);

                npc.position -= npc.netOffset;
            }
            else if (npc.type == NPCID.StardustWormHead && npc.ai[1] == 0f)
            {
                npc.ai[1] = Main.rand.Next(-2, 0);
                npc.netUpdate = true;
            }

            bool flag = npc.type == NPCID.GiantWormHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.DiggerHead || npc.type == NPCID.LeechHead || npc.type == NPCID.DuneSplicerHead || (!Main.player[npc.target].ZoneUndergroundDesert && npc.type == NPCID.TombCrawlerHead);
            float num4 = npc.type == NPCID.TombCrawlerHead ? 0.1f : 0.2f;

            bool isSplittingNPC = (npc.type >= NPCID.DiggerHead && npc.type <= NPCID.DiggerTail) || (npc.type >= NPCID.SeekerHead && npc.type <= NPCID.SeekerTail) || (npc.type >= NPCID.DuneSplicerHead && npc.type <= NPCID.DuneSplicerTail);

            bool isSplittingNPCHead = npc.type == NPCID.DiggerHead || npc.type == NPCID.SeekerHead || npc.type == NPCID.DuneSplicerHead;

            bool isSplittingNPCBody = npc.type == NPCID.DiggerBody || npc.type == NPCID.SeekerBody || npc.type == NPCID.DuneSplicerBody;

            bool isSplittingNPCTail = npc.type == NPCID.DiggerTail || npc.type == NPCID.SeekerTail || npc.type == NPCID.DuneSplicerTail;

            if (isSplittingNPC)
            {
                npc.realLife = -1;
            }
            else
            {
                npc.defense = (int)(npc.defDefense * 1.3);

                if (npc.ai[3] > 0f)
                    npc.realLife = (int)npc.ai[3];
            }

            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || (flag && (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0))
            {
                npc.TargetClosest(true);
            }

            if (Main.player[npc.target].dead || (flag && (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0))
            {
                if (npc.timeLeft > 300)
                {
                    npc.timeLeft = 300;
                }
                if (flag)
                {
                    npc.velocity.Y = npc.velocity.Y + num4;
                }
            }

            if (npc.type == NPCID.BloodEelHead && Main.dayTime)
            {
                npc.EncourageDespawn(60);
                npc.velocity.Y += 1f;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.type == NPCID.WyvernHead && npc.ai[0] == 0f)
                {
                    int maxParts = CalamityWorld.death ? 30 : 21;
                    npc.ai[3] = (float)npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int num6 = npc.whoAmI;
                    for (int k = 0; k < maxParts; k++)
                    {
                        int num7 = NPCID.WyvernBody;
                        if (k == 1 || k == 12)
                        {
                            num7 = NPCID.WyvernLegs;
                        }
                        else if (k == maxParts - 3)
                        {
                            num7 = NPCID.WyvernBody2;
                        }
                        else if (k == maxParts - 2)
                        {
                            num7 = NPCID.WyvernBody3;
                        }
                        else if (k == maxParts - 1)
                        {
                            num7 = NPCID.WyvernTail;
                        }
                        int num8 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num7, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num8].ai[3] = (float)npc.whoAmI;
                        Main.npc[num8].realLife = npc.whoAmI;
                        Main.npc[num8].ai[1] = (float)num6;
                        Main.npc[num6].ai[0] = (float)num8;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num8, 0f, 0f, 0f, 0, 0, 0);
                        num6 = num8;
                    }
                }

                if (npc.type == NPCID.TombCrawlerHead && npc.ai[0] == 0f)
                {
                    npc.ai[3] = (float)npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int num12 = npc.whoAmI;
                    int num13 = Main.rand.Next(11, CalamityWorld.death ? 25 : 15);
                    for (int m = 0; m < num13; m++)
                    {
                        int num14 = NPCID.TombCrawlerBody;
                        if (m == num13 - 1)
                        {
                            num14 = NPCID.TombCrawlerTail;
                        }
                        int num15 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num14, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num15].ai[3] = (float)npc.whoAmI;
                        Main.npc[num15].realLife = npc.whoAmI;
                        Main.npc[num15].ai[1] = (float)num12;
                        Main.npc[num12].ai[0] = (float)num15;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num15, 0f, 0f, 0f, 0, 0, 0);
                        num12 = num15;
                    }
                }

                if (npc.type == NPCID.SolarCrawltipedeHead && npc.ai[0] == 0f)
                {
                    npc.ai[3] = (float)npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int num20 = npc.whoAmI;
                    int num21 = CalamityWorld.death ? 70 : 50;
                    for (int num22 = 0; num22 < num21; num22++)
                    {
                        int num23 = NPCID.SolarCrawltipedeBody;
                        if (num22 == num21 - 1)
                        {
                            num23 = NPCID.SolarCrawltipedeTail;
                        }
                        int num24 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num23, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[num24].ai[3] = (float)npc.whoAmI;
                        Main.npc[num24].realLife = npc.whoAmI;
                        Main.npc[num24].ai[1] = (float)num20;
                        Main.npc[num20].ai[0] = (float)num24;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num24, 0f, 0f, 0f, 0, 0, 0);
                        num20 = num24;
                    }
                }

                if (npc.type == NPCID.BloodEelHead && npc.ai[0] == 0f)
                {
                    npc.ai[3] = npc.whoAmI;
                    npc.realLife = npc.whoAmI;
                    int num22 = 0;
                    int num23 = npc.whoAmI;
                    int num24 = CalamityWorld.death ? 44 : 34;
                    for (int num25 = 0; num25 < num24; num25++)
                    {
                        int num26 = NPCID.BloodEelBody;
                        if (num25 == num24 - 1)
                            num26 = NPCID.BloodEelTail;

                        num22 = NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), num26, npc.whoAmI);
                        Main.npc[num22].ai[3] = npc.whoAmI;
                        Main.npc[num22].realLife = npc.whoAmI;
                        Main.npc[num22].ai[1] = num23;
                        Main.npc[num22].CopyInteractions(npc);
                        Main.npc[num23].ai[0] = num22;
                        NetMessage.SendData(23, -1, -1, null, num22);
                        num23 = num22;
                    }
                }
                else if ((isSplittingNPCHead || isSplittingNPCBody || npc.type == NPCID.GiantWormHead || npc.type == NPCID.GiantWormBody || npc.type == NPCID.DevourerHead || npc.type == NPCID.DevourerBody || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.BoneSerpentBody || npc.type == NPCID.LeechHead || npc.type == NPCID.LeechBody) && npc.ai[0] == 0f)
                {
                    if (isSplittingNPCHead || npc.type == NPCID.GiantWormHead || npc.type == NPCID.DevourerHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.LeechHead)
                    {
                        if (!isSplittingNPCHead)
                        {
                            npc.ai[3] = (float)npc.whoAmI;
                            npc.realLife = npc.whoAmI;
                        }

                        switch (npc.type)
                        {
                            case NPCID.DevourerHead:
                                npc.ai[2] = (float)Main.rand.Next(13, CalamityWorld.death ? 30 : 19);
                                break;
                            case NPCID.GiantWormHead:
                                npc.ai[2] = (float)Main.rand.Next(25, CalamityWorld.death ? 50 : 31);
                                break;
                            case NPCID.BoneSerpentHead:
                                npc.ai[2] = (float)Main.rand.Next(16, CalamityWorld.death ? 33 : 23);
                                break;
                            case NPCID.DiggerHead:
                                npc.ai[2] = (float)Main.rand.Next(12, CalamityWorld.death ? 27 : 18);
                                break;
                            case NPCID.SeekerHead:
                                npc.ai[2] = (float)Main.rand.Next(27, CalamityWorld.death ? 45 : 33);
                                break;
                            case NPCID.LeechHead:
                                npc.ai[2] = (float)Main.rand.Next(CalamityWorld.death ? 3 : 5, CalamityWorld.death ? 5 : 8);
                                break;
                            case NPCID.DuneSplicerHead:
                                npc.ai[2] = (float)Main.rand.Next(15, CalamityWorld.death ? 35 : 24);
                                break;
                        }

                        npc.ai[0] = (float)NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), npc.type + 1, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }
                    else if ((isSplittingNPCBody || npc.type == NPCID.GiantWormBody || npc.type == NPCID.DevourerBody || npc.type == NPCID.BoneSerpentBody || npc.type == NPCID.LeechBody) && npc.ai[2] > 0f)
                    {
                        npc.ai[0] = (float)NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), npc.type, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }
                    else
                    {
                        npc.ai[0] = (float)NPC.NewNPC(npc.GetSource_FromAI(), (int)(npc.position.X + (float)(npc.width / 2)), (int)(npc.position.Y + (float)npc.height), npc.type + 1, npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                    }

                    if (!isSplittingNPC)
                    {
                        Main.npc[(int)npc.ai[0]].ai[3] = npc.ai[3];
                        Main.npc[(int)npc.ai[0]].realLife = npc.realLife;
                    }
                    Main.npc[(int)npc.ai[0]].ai[1] = (float)npc.whoAmI;
                    Main.npc[(int)npc.ai[0]].ai[2] = npc.ai[2] - 1f;
                    npc.netUpdate = true;
                }

                if (!isSplittingNPC)
                {
                    if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
                    {
                        if (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle)
                        {
                            npc.life = 0;
                            npc.HitEffect(0, 10.0);
                            npc.active = false;
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                        }
                    }
                    if (npc.ai[0] > 0f && npc.ai[0] < (float)Main.npc.Length)
                    {
                        if (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle)
                        {
                            npc.life = 0;
                            npc.HitEffect(0, 10.0);
                            npc.active = false;
                            NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
                else
                {
                    if (!Main.npc[(int)npc.ai[1]].active && !Main.npc[(int)npc.ai[0]].active)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);
                        npc.checkDead();
                        npc.active = false;
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                    }
                    if (isSplittingNPCHead && !Main.npc[(int)npc.ai[0]].active)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);
                        npc.checkDead();
                        npc.active = false;
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                    }
                    if (isSplittingNPCTail && !Main.npc[(int)npc.ai[1]].active)
                    {
                        npc.life = 0;
                        npc.HitEffect(0, 10.0);
                        npc.checkDead();
                        npc.active = false;
                        NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                    }
                    if (isSplittingNPCBody && (!Main.npc[(int)npc.ai[1]].active || Main.npc[(int)npc.ai[1]].aiStyle != npc.aiStyle))
                    {
                        npc.type = npc.type - 1;
                        int whoAmI = npc.whoAmI;
                        float num25 = (float)npc.life / (float)npc.lifeMax;
                        float num26 = npc.ai[0];
                        npc.SetDefaultsKeepPlayerInteraction(npc.type);
                        npc.life = (int)((float)npc.lifeMax * num25);
                        npc.ai[0] = num26;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        npc.whoAmI = whoAmI;
                    }
                    if (isSplittingNPCBody && (!Main.npc[(int)npc.ai[0]].active || Main.npc[(int)npc.ai[0]].aiStyle != npc.aiStyle))
                    {
                        int whoAmI2 = npc.whoAmI;
                        float num27 = (float)npc.life / (float)npc.lifeMax;
                        float num28 = npc.ai[1];
                        npc.SetDefaultsKeepPlayerInteraction(npc.type);
                        npc.life = (int)((float)npc.lifeMax * num27);
                        npc.ai[1] = num28;
                        npc.TargetClosest(true);
                        npc.netUpdate = true;
                        npc.whoAmI = whoAmI2;
                    }
                }

                if (!npc.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            int num29 = (int)(npc.position.X / 16f) - 1;
            int num30 = (int)((npc.position.X + (float)npc.width) / 16f) + 2;
            int num31 = (int)(npc.position.Y / 16f) - 1;
            int num32 = (int)((npc.position.Y + (float)npc.height) / 16f) + 2;
            if (num29 < 0)
            {
                num29 = 0;
            }
            if (num30 > Main.maxTilesX)
            {
                num30 = Main.maxTilesX;
            }
            if (num31 < 0)
            {
                num31 = 0;
            }
            if (num32 > Main.maxTilesY)
            {
                num32 = Main.maxTilesY;
            }

            bool flag2 = false;
            if (npc.type >= NPCID.WyvernHead && npc.type <= NPCID.WyvernTail)
            {
                flag2 = true;
            }
            if (npc.type == NPCID.StardustWormHead && npc.ai[1] == -1f)
            {
                flag2 = true;
            }
            if (npc.type >= NPCID.SolarCrawltipedeHead && npc.type <= NPCID.SolarCrawltipedeTail)
            {
                flag2 = true;
            }
            if (npc.type >= NPCID.BloodEelHead && npc.type <= NPCID.BloodEelTail)
            {
                flag2 = true;
            }
            if (!flag2)
            {
                for (int num33 = num29; num33 < num30; num33++)
                {
                    for (int num34 = num31; num34 < num32; num34++)
                    {
                        if (Main.tile[num33, num34] != null && ((Main.tile[num33, num34].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[num33, num34].TileType] || (Main.tileSolidTop[(int)Main.tile[num33, num34].TileType] && Main.tile[num33, num34].TileFrameY == 0))) || Main.tile[num33, num34].LiquidAmount > 64))
                        {
                            Vector2 vector;
                            vector.X = (float)(num33 * 16);
                            vector.Y = (float)(num34 * 16);
                            if (npc.position.X + (float)npc.width > vector.X && npc.position.X < vector.X + 16f && npc.position.Y + (float)npc.height > vector.Y && npc.position.Y < vector.Y + 16f)
                            {
                                flag2 = true;
                                if (Main.rand.NextBool(100) && npc.type != NPCID.LeechHead && Main.tile[num33, num34].HasUnactuatedTile)
                                {
                                    WorldGen.KillTile(num33, num34, true, true, false);
                                }
                            }
                        }
                    }
                }
            }

            if (!flag2 && (isSplittingNPCHead || npc.type == NPCID.GiantWormHead || npc.type == NPCID.DevourerHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.LeechHead || npc.type == NPCID.TombCrawlerHead))
            {
                Rectangle rectangle = new Rectangle((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height);
                int num35 = 1000;
                bool flag3 = true;
                for (int num36 = 0; num36 < Main.maxPlayers; num36++)
                {
                    if (Main.player[num36].active)
                    {
                        Rectangle rectangle2 = new Rectangle((int)Main.player[num36].position.X - num35, (int)Main.player[num36].position.Y - num35, num35 * 2, num35 * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            flag3 = false;
                            break;
                        }
                    }
                }
                if (flag3)
                {
                    flag2 = true;
                }
            }

            if ((npc.type >= NPCID.WyvernHead && npc.type <= NPCID.WyvernTail) || (npc.type >= NPCID.BloodEelHead && npc.type <= NPCID.BloodEelTail))
            {
                if (npc.velocity.X < 0f)
                {
                    npc.spriteDirection = 1;
                }
                else if (npc.velocity.X > 0f)
                {
                    npc.spriteDirection = -1;
                }
            }

            if (npc.type == NPCID.SolarCrawltipedeTail)
            {
                if (npc.justHit)
                {
                    npc.localAI[3] = 3f;
                }
                if (npc.localAI[2] > 0f)
                {
                    npc.localAI[2] -= 16f;
                    if (npc.localAI[2] == 0f)
                    {
                        npc.localAI[2] = -128f;
                    }
                }
                else if (npc.localAI[2] < 0f)
                {
                    npc.localAI[2] += 16f;
                }
                else if (npc.localAI[3] > 0f)
                {
                    npc.localAI[2] = 128f;
                    npc.localAI[3] -= 1f;
                }
            }

            if (npc.type == NPCID.SolarCrawltipedeHead)
            {
                Vector2 value = npc.Center + (npc.rotation - MathHelper.PiOver2).ToRotationVector2() * 8f;
                Vector2 value2 = npc.rotation.ToRotationVector2() * 16f;
                Dust expr_1AAB = Main.dust[Dust.NewDust(value + value2, 0, 0, 6, npc.velocity.X, npc.velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 3f)];
                expr_1AAB.noGravity = true;
                expr_1AAB.noLight = true;
                expr_1AAB.position -= new Vector2(4f);
                expr_1AAB.fadeIn = 1f;
                expr_1AAB.velocity = Vector2.Zero;
                Dust expr_1B33 = Main.dust[Dust.NewDust(value - value2, 0, 0, 6, npc.velocity.X, npc.velocity.Y, 100, Color.Transparent, 1f + Main.rand.NextFloat() * 3f)];
                expr_1B33.noGravity = true;
                expr_1B33.noLight = true;
                expr_1B33.position -= new Vector2(4f);
                expr_1B33.fadeIn = 1f;
                expr_1B33.velocity = Vector2.Zero;
            }

            float num37 = 10f;
            float num38 = 0.09f;
            if (npc.type == NPCID.DiggerHead)
            {
                num37 = 6.5f;
                num38 = 0.05f;
            }
            if (npc.type == NPCID.GiantWormHead)
            {
                num37 = 7.5f;
                num38 = 0.06f;
            }
            if (npc.type == NPCID.TombCrawlerHead)
            {
                num37 = 8f;
                num38 = 0.13f;
            }
            if (npc.type == NPCID.DuneSplicerHead)
            {
                if (!Main.player[npc.target].dead && Main.player[npc.target].ZoneSandstorm)
                {
                    num37 = 16f;
                    num38 = 0.35f;
                }
                else
                {
                    num38 = 0.25f;
                }
            }
            if (npc.type == NPCID.WyvernHead)
            {
                num37 = 11f;
                num38 = 0.3f;
            }
            if (npc.type == NPCID.StardustWormHead)
            {
                num37 = 9f;
                num38 = 0.25f;
            }
            if (npc.type == NPCID.LeechHead && Main.wofNPCIndex >= 0)
            {
                float lifeRatio = (float)Main.npc[Main.wofNPCIndex].life / (float)Main.npc[Main.wofNPCIndex].lifeMax;
                if (lifeRatio < 0.75f)
                {
                    num37 += 1f;
                    num38 += 0.1f;
                }
                if (lifeRatio < 0.5f)
                {
                    num37 += 1f;
                    num38 += 0.1f;
                }
                if (lifeRatio < 0.25f)
                {
                    num37 += 2f;
                    num38 += 0.1f;
                }
            }
            if (npc.type == NPCID.BloodEelHead)
            {
                num37 = 18f;
                num38 = 0.6f;
            }

            if (CalamityWorld.death)
            {
                num37 *= 1.25f;
                num38 *= 1.25f;
            }

            Vector2 vector2 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float num39 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2);
            float num40 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2);

            if (npc.type == NPCID.SolarCrawltipedeHead)
            {
                num37 = 12f;
                num38 = 0.32f;
                int num41 = -1;
                int num42 = (int)(Main.player[npc.target].Center.X / 16f);
                int num43 = (int)(Main.player[npc.target].Center.Y / 16f);
                for (int num44 = num42 - 2; num44 <= num42 + 2; num44++)
                {
                    for (int num45 = num43; num45 <= num43 + 15; num45++)
                    {
                        if (WorldGen.SolidTile2(num44, num45))
                        {
                            num41 = num45;
                            break;
                        }
                    }
                    if (num41 > 0)
                    {
                        break;
                    }
                }
                if (num41 > 0)
                {
                    num41 *= 16;
                    float num46 = (float)(num41 - 800);
                    if (Main.player[npc.target].position.Y > num46)
                    {
                        num40 = num46;
                        if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 500f)
                        {
                            if (npc.velocity.X > 0f)
                            {
                                num39 = Main.player[npc.target].Center.X + 600f;
                            }
                            else
                            {
                                num39 = Main.player[npc.target].Center.X - 600f;
                            }
                        }
                    }
                }
                else
                {
                    num37 = 28f;
                    num38 = 0.8f;
                }
                float num47 = num37 * 1.3f;
                float num48 = num37 * 0.7f;
                float num49 = npc.velocity.Length();
                if (num49 > 0f)
                {
                    if (num49 > num47)
                    {
                        npc.velocity.Normalize();
                        npc.velocity *= num47;
                    }
                    else if (num49 < num48)
                    {
                        npc.velocity.Normalize();
                        npc.velocity *= num48;
                    }
                }
                if (num41 > 0)
                {
                    for (int num50 = 0; num50 < Main.maxNPCs; num50++)
                    {
                        if (Main.npc[num50].active && Main.npc[num50].type == npc.type && num50 != npc.whoAmI)
                        {
                            Vector2 vector3 = Main.npc[num50].Center - npc.Center;
                            if (vector3.Length() < 400f)
                            {
                                vector3.Normalize();
                                vector3 *= 1000f;
                                num39 -= vector3.X;
                                num40 -= vector3.Y;
                            }
                        }
                    }
                }
                else
                {
                    for (int num51 = 0; num51 < Main.maxNPCs; num51++)
                    {
                        if (Main.npc[num51].active && Main.npc[num51].type == npc.type && num51 != npc.whoAmI)
                        {
                            Vector2 vector4 = Main.npc[num51].Center - npc.Center;
                            if (vector4.Length() < 60f)
                            {
                                vector4.Normalize();
                                vector4 *= 200f;
                                num39 -= vector4.X;
                                num40 -= vector4.Y;
                            }
                        }
                    }
                }
            }

            num39 = (float)((int)(num39 / 16f) * 16);
            num40 = (float)((int)(num40 / 16f) * 16);
            vector2.X = (float)((int)(vector2.X / 16f) * 16);
            vector2.Y = (float)((int)(vector2.Y / 16f) * 16);
            num39 -= vector2.X;
            num40 -= vector2.Y;
            float num52 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));

            if (npc.ai[1] > 0f && npc.ai[1] < (float)Main.npc.Length)
            {
                try
                {
                    vector2 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                    num39 = Main.npc[(int)npc.ai[1]].position.X + (float)(Main.npc[(int)npc.ai[1]].width / 2) - vector2.X;
                    num40 = Main.npc[(int)npc.ai[1]].position.Y + (float)(Main.npc[(int)npc.ai[1]].height / 2) - vector2.Y;
                }
                catch
                {
                }
                npc.rotation = (float)Math.Atan2((double)num40, (double)num39) + MathHelper.PiOver2;
                num52 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));
                int num53 = npc.width;
                if (npc.type >= NPCID.WyvernHead && npc.type <= NPCID.WyvernTail)
                {
                    num53 = 42;
                }
                if (npc.type >= NPCID.SolarCrawltipedeHead && npc.type <= NPCID.SolarCrawltipedeTail)
                {
                    num53 += 6;
                }
                if (npc.type >= NPCID.BloodEelHead && npc.type <= NPCID.BloodEelTail)
                {
                    num53 = 24;
                }
                num52 = (num52 - (float)num53) / num52;
                num39 *= num52;
                num40 *= num52;
                npc.velocity = Vector2.Zero;
                npc.position.X = npc.position.X + num39;
                npc.position.Y = npc.position.Y + num40;
                if ((npc.type >= NPCID.WyvernHead && npc.type <= NPCID.WyvernTail) || (npc.type >= NPCID.BloodEelHead && npc.type <= NPCID.BloodEelTail))
                {
                    if (num39 < 0f)
                    {
                        npc.spriteDirection = 1;
                    }
                    else if (num39 > 0f)
                    {
                        npc.spriteDirection = -1;
                    }
                }
            }
            else
            {
                if (!flag2)
                {
                    npc.TargetClosest(true);
                    npc.velocity.Y = npc.velocity.Y + 0.11f;
                    if (npc.velocity.Y > num37)
                    {
                        npc.velocity.Y = num37;
                    }
                    if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num37 * 0.4)
                    {
                        if (npc.velocity.X < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num38 * 1.1f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X + num38 * 1.1f;
                        }
                    }
                    else if (npc.velocity.Y == num37)
                    {
                        if (npc.velocity.X < num39)
                        {
                            npc.velocity.X = npc.velocity.X + num38;
                        }
                        else if (npc.velocity.X > num39)
                        {
                            npc.velocity.X = npc.velocity.X - num38;
                        }
                    }
                    else if (npc.velocity.Y > 4f)
                    {
                        if (npc.velocity.X < 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num38 * 0.9f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num38 * 0.9f;
                        }
                    }
                }
                else
                {
                    if (npc.type != NPCID.WyvernHead && npc.type != NPCID.LeechHead && npc.type != NPCID.SolarCrawltipedeHead && npc.type != NPCID.BloodEelHead && npc.soundDelay == 0)
                    {
                        float num54 = num52 / 40f;
                        if (num54 < 10f)
                        {
                            num54 = 10f;
                        }
                        if (num54 > 20f)
                        {
                            num54 = 20f;
                        }
                        npc.soundDelay = (int)num54;
                        SoundEngine.PlaySound(SoundID.WormDig, npc.position);
                    }

                    num52 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));
                    float num55 = Math.Abs(num39);
                    float num56 = Math.Abs(num40);
                    float num57 = num37 / num52;
                    num39 *= num57;
                    num40 *= num57;

                    bool flag4 = false;
                    if (npc.type == NPCID.DevourerHead && ((!Main.player[npc.target].ZoneCorrupt && !Main.player[npc.target].ZoneCrimson) || Main.player[npc.target].dead))
                    {
                        flag4 = true;
                    }
                    if ((npc.type == NPCID.TombCrawlerHead && (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0 && !Main.player[npc.target].ZoneSandstorm && !Main.player[npc.target].ZoneUndergroundDesert) || Main.player[npc.target].dead)
                    {
                        flag4 = true;
                    }
                    if ((npc.type == NPCID.DuneSplicerHead && (double)Main.player[npc.target].position.Y < Main.worldSurface * 16.0 && !Main.player[npc.target].ZoneSandstorm && !Main.player[npc.target].ZoneUndergroundDesert) || Main.player[npc.target].dead)
                    {
                        flag4 = true;
                    }
                    if (flag4)
                    {
                        bool flag5 = true;
                        for (int num58 = 0; num58 < Main.maxPlayers; num58++)
                        {
                            if (Main.player[num58].active && !Main.player[num58].dead && Main.player[num58].ZoneCorrupt)
                            {
                                flag5 = false;
                            }
                        }
                        if (flag5)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient && (double)(npc.position.Y / 16f) > (Main.rockLayer + (double)Main.maxTilesY) / 2.0)
                            {
                                npc.active = false;
                                int num59 = (int)npc.ai[0];
                                while (num59 > 0 && num59 < Main.maxNPCs && Main.npc[num59].active && Main.npc[num59].aiStyle == npc.aiStyle)
                                {
                                    int arg_2853_0 = (int)Main.npc[num59].ai[0];
                                    Main.npc[num59].active = false;
                                    npc.life = 0;
                                    if (Main.netMode == NetmodeID.Server)
                                    {
                                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num59, 0f, 0f, 0f, 0, 0, 0);
                                    }
                                    num59 = arg_2853_0;
                                }
                                if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, npc.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                                }
                            }
                            num39 = 0f;
                            num40 = num37;
                        }
                    }

                    bool flag6 = false;
                    if (npc.type == NPCID.WyvernHead)
                    {
                        if (((npc.velocity.X > 0f && num39 < 0f) || (npc.velocity.X < 0f && num39 > 0f) || (npc.velocity.Y > 0f && num40 < 0f) || (npc.velocity.Y < 0f && num40 > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > num38 / 2f && num52 < 300f)
                        {
                            flag6 = true;

                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num37)
                                npc.velocity *= 1.1f;
                        }
                        if (npc.position.Y > Main.player[npc.target].position.Y || (double)(Main.player[npc.target].position.Y / 16f) > Main.worldSurface || Main.player[npc.target].dead)
                        {
                            flag6 = true;

                            if (Math.Abs(npc.velocity.X) < num37 / 2f)
                            {
                                if (npc.velocity.X == 0f)
                                    npc.velocity.X = npc.velocity.X - npc.direction;

                                npc.velocity.X = npc.velocity.X * 1.1f;
                            }
                            else if (npc.velocity.Y > -num37)
                                npc.velocity.Y = npc.velocity.Y - num38;
                        }
                    }

                    if (npc.type == NPCID.BloodEelHead)
                    {
                        float num69 = 120f;
                        if (((npc.velocity.X > 0f && num39 < 0f) || (npc.velocity.X < 0f && num39 > 0f) || (npc.velocity.Y > 0f && num40 < 0f) || (npc.velocity.Y < 0f && num40 > 0f)) && Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) > num38 / 2f && num52 < num69)
                        {
                            flag6 = true;
                            if (Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y) < num37)
                                npc.velocity *= 1.1f;
                        }
                        if (npc.position.Y > Main.player[npc.target].position.Y || Main.player[npc.target].dead)
                        {
                            flag6 = true;
                            if (Math.Abs(npc.velocity.X) < num37 / 2f)
                            {
                                if (npc.velocity.X == 0f)
                                    npc.velocity.X -= npc.direction;

                                npc.velocity.X *= 1.1f;
                            }
                            else if (npc.velocity.Y > 0f - num37)
                                npc.velocity.Y -= num38;
                        }
                    }

                    if (!flag6)
                    {
                        if ((npc.velocity.X > 0f && num39 > 0f) || (npc.velocity.X < 0f && num39 < 0f) || (npc.velocity.Y > 0f && num40 > 0f) || (npc.velocity.Y < 0f && num40 < 0f))
                        {
                            if (npc.velocity.X < num39)
                                npc.velocity.X = npc.velocity.X + num38;
                            else if (npc.velocity.X > num39)
                                npc.velocity.X = npc.velocity.X - num38;

                            if (npc.velocity.Y < num40)
                                npc.velocity.Y = npc.velocity.Y + num38;
                            else if (npc.velocity.Y > num40)
                                npc.velocity.Y = npc.velocity.Y - num38;

                            if ((double)Math.Abs(num40) < (double)num37 * 0.2 && ((npc.velocity.X > 0f && num39 < 0f) || (npc.velocity.X < 0f && num39 > 0f)))
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y = npc.velocity.Y + num38 * 2f;
                                else
                                    npc.velocity.Y = npc.velocity.Y - num38 * 2f;
                            }

                            if ((double)Math.Abs(num39) < (double)num37 * 0.2 && ((npc.velocity.Y > 0f && num40 < 0f) || (npc.velocity.Y < 0f && num40 > 0f)))
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X = npc.velocity.X + num38 * 2f;
                                else
                                    npc.velocity.X = npc.velocity.X - num38 * 2f;
                            }
                        }
                        else if (num55 > num56)
                        {
                            if (npc.velocity.X < num39)
                                npc.velocity.X = npc.velocity.X + num38 * 1.1f;
                            else if (npc.velocity.X > num39)
                                npc.velocity.X = npc.velocity.X - num38 * 1.1f;

                            if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num37 * 0.5)
                            {
                                if (npc.velocity.Y > 0f)
                                    npc.velocity.Y = npc.velocity.Y + num38;
                                else
                                    npc.velocity.Y = npc.velocity.Y - num38;
                            }
                        }
                        else
                        {
                            if (npc.velocity.Y < num40)
                                npc.velocity.Y = npc.velocity.Y + num38 * 1.1f;
                            else if (npc.velocity.Y > num40)
                                npc.velocity.Y = npc.velocity.Y - num38 * 1.1f;

                            if ((double)(Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < (double)num37 * 0.5)
                            {
                                if (npc.velocity.X > 0f)
                                    npc.velocity.X = npc.velocity.X + num38;
                                else
                                    npc.velocity.X = npc.velocity.X - num38;
                            }
                        }
                    }
                }

                npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X) + MathHelper.PiOver2;
                if (npc.type == NPCID.DevourerHead || npc.type == NPCID.GiantWormHead || npc.type == NPCID.BoneSerpentHead || npc.type == NPCID.DiggerHead || npc.type == NPCID.SeekerHead || npc.type == NPCID.LeechHead || npc.type == NPCID.DuneSplicerHead || npc.type == NPCID.TombCrawlerHead)
                {
                    if (flag2)
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
                    {
                        npc.netUpdate = true;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Caster AI
        public static bool BuffedCasterAI(NPC npc, Mod mod)
        {
            npc.TargetClosest(true);
            npc.velocity.X = npc.velocity.X * 0.93f;
            if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
            {
                npc.velocity.X = 0f;
            }
            if (npc.ai[0] == 0f)
            {
                npc.ai[0] = 500f;
            }
            if (npc.type == NPCID.RuneWizard)
            {
                if (npc.alpha < 255)
                {
                    npc.alpha++;
                }
                if (npc.justHit)
                {
                    npc.alpha = 0;
                }
            }
            if (npc.ai[2] != 0f && npc.ai[3] != 0f)
            {
                if (npc.type == NPCID.RuneWizard)
                {
                    npc.alpha = 255;
                }
                SoundEngine.PlaySound(SoundID.Item8, npc.position);
                int num;
                for (int num68 = 0; num68 < 50; num68 = num + 1)
                {
                    if (npc.type == NPCID.GoblinSorcerer || npc.type == NPCID.Tim)
                    {
                        int num69 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 100, default(Color), (float)Main.rand.Next(1, 3));
                        Dust dust = Main.dust[num69];
                        dust.velocity *= 3f;
                        if (Main.dust[num69].scale > 1f)
                        {
                            Main.dust[num69].noGravity = true;
                        }
                    }
                    else if (npc.type == NPCID.DarkCaster)
                    {
                        int num70 = Dust.NewDust(npc.position, npc.width, npc.height, 172, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[num70];
                        dust.velocity *= 3f;
                        Main.dust[num70].noGravity = true;
                    }
                    else if (npc.type == NPCID.Necromancer || npc.type == NPCID.NecromancerArmored)
                    {
                        int num71 = Dust.NewDust(npc.position, npc.width, npc.height, 173, 0f, 0f, 0, default(Color), 1f);
                        Dust dust = Main.dust[num71];
                        dust.velocity *= 2f;
                        Main.dust[num71].scale = 1.4f;
                    }
                    else if (npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
                    {
                        int num72 = Dust.NewDust(npc.position, npc.width, npc.height, 174, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[num72];
                        dust.velocity *= 3f;
                        Main.dust[num72].noGravity = true;
                    }
                    else if (npc.type == NPCID.RaggedCaster || npc.type == NPCID.RaggedCasterOpenCoat)
                    {
                        int num73 = Dust.NewDust(npc.position, npc.width, npc.height, 175, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[num73];
                        dust.velocity *= 3f;
                        Main.dust[num73].noGravity = true;
                    }
                    else if (npc.type == NPCID.RuneWizard)
                    {
                        int num74 = Dust.NewDust(npc.position, npc.width, npc.height, 106, 0f, 0f, 100, default(Color), 2.5f);
                        Dust dust = Main.dust[num74];
                        dust.velocity *= 3f;
                        Main.dust[num74].noGravity = true;
                    }
                    else if (npc.type == NPCID.DesertDjinn)
                    {
                        int num75 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 100, default(Color), 2.5f);
                        Dust dust = Main.dust[num75];
                        dust.velocity *= 3f;
                        Main.dust[num75].noGravity = true;
                    }
                    else
                    {
                        int num76 = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 2.5f);
                        Dust dust = Main.dust[num76];
                        dust.velocity *= 3f;
                        Main.dust[num76].noGravity = true;
                    }
                    num = num68;
                }
                npc.position.X = npc.ai[2] * 16f - (float)(npc.width / 2) + 8f;
                npc.position.Y = npc.ai[3] * 16f - (float)npc.height;
                npc.velocity.X = 0f;
                npc.velocity.Y = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                SoundEngine.PlaySound(SoundID.Item8, npc.position);
                for (int num77 = 0; num77 < 50; num77 = num + 1)
                {
                    if (npc.type == NPCID.GoblinSorcerer || npc.type == NPCID.Tim)
                    {
                        int num78 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 100, default(Color), (float)Main.rand.Next(1, 3));
                        Dust dust = Main.dust[num78];
                        dust.velocity *= 3f;
                        if (Main.dust[num78].scale > 1f)
                        {
                            Main.dust[num78].noGravity = true;
                        }
                    }
                    else if (npc.type == NPCID.DarkCaster)
                    {
                        int num79 = Dust.NewDust(npc.position, npc.width, npc.height, 172, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[num79];
                        dust.velocity *= 3f;
                        Main.dust[num79].noGravity = true;
                    }
                    else if (npc.type == NPCID.RuneWizard)
                    {
                        int num80 = Dust.NewDust(npc.position, npc.width, npc.height, 106, 0f, 0f, 100, default(Color), 2.5f);
                        Dust dust = Main.dust[num80];
                        dust.velocity *= 3f;
                        Main.dust[num80].noGravity = true;
                    }
                    else if (npc.type == NPCID.Necromancer || npc.type == NPCID.NecromancerArmored)
                    {
                        int num81 = Dust.NewDust(npc.position, npc.width, npc.height, 173, 0f, 0f, 0, default(Color), 1f);
                        Dust dust = Main.dust[num81];
                        dust.velocity *= 2f;
                        Main.dust[num81].scale = 1.4f;
                    }
                    else if (npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
                    {
                        int num82 = Dust.NewDust(npc.position, npc.width, npc.height, 174, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[num82];
                        dust.velocity *= 3f;
                        Main.dust[num82].noGravity = true;
                    }
                    else if (npc.type == NPCID.RaggedCaster || npc.type == NPCID.RaggedCasterOpenCoat)
                    {
                        int num83 = Dust.NewDust(npc.position, npc.width, npc.height, 175, 0f, 0f, 100, default(Color), 1.5f);
                        Dust dust = Main.dust[num83];
                        dust.velocity *= 3f;
                        Main.dust[num83].noGravity = true;
                    }
                    else if (npc.type == NPCID.DesertDjinn)
                    {
                        int num84 = Dust.NewDust(npc.position, npc.width, npc.height, 27, 0f, 0f, 100, default(Color), 2.5f);
                        Dust dust = Main.dust[num84];
                        dust.velocity *= 3f;
                        Main.dust[num84].noGravity = true;
                    }
                    else
                    {
                        int num85 = Dust.NewDust(npc.position, npc.width, npc.height, 6, 0f, 0f, 100, default(Color), 2.5f);
                        Dust dust = Main.dust[num85];
                        dust.velocity *= 3f;
                        Main.dust[num85].noGravity = true;
                    }
                    num = num77;
                }
            }

            if (npc.justHit)
                npc.ai[0] = (npc.type == NPCID.RuneWizard && Main.zenithWorld) ? 5f : 2f;

            npc.ai[0] += (npc.type == NPCID.RuneWizard && Main.zenithWorld) ? 5f : 2f;

            if (npc.type == NPCID.Necromancer || npc.type == NPCID.NecromancerArmored)
            {
                if (npc.ai[0] % 50f == 0f && npc.ai[0] <= 250f)
                {
                    npc.ai[1] = 30f;
                    npc.netUpdate = true;
                }
                if (npc.ai[0] >= 400f)
                {
                    npc.ai[0] = 700f;
                }
            }
            else if (npc.type == NPCID.RuneWizard)
            {
                if (npc.ai[0] == 80f || npc.ai[0] == 150f || npc.ai[0] == 230f || npc.ai[0] == 300f || npc.ai[0] == 380f || npc.ai[0] == 450f)
                {
                    npc.ai[1] = 30f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.type == NPCID.DesertDjinn)
            {
                if (npc.ai[0] == 180f)
                {
                    npc.ai[1] = 181f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.type == NPCID.RaggedCaster || npc.type == NPCID.RaggedCasterOpenCoat)
            {
                if (npc.ai[0] == 20f || npc.ai[0] == 40f || npc.ai[0] == 60f || npc.ai[0] == 120f || npc.ai[0] == 140f || npc.ai[0] == 160f || npc.ai[0] == 220f || npc.ai[0] == 240f || npc.ai[0] == 260f)
                {
                    npc.ai[1] = 30f;
                    npc.netUpdate = true;
                }
                if (npc.ai[0] >= 460f)
                {
                    npc.ai[0] = 700f;
                }
            }
            else if (npc.ai[0] % 100f == 0f && npc.ai[0] <= 300f)
            {
                npc.ai[1] = 30f;
                npc.netUpdate = true;
            }
            if ((npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite) && npc.ai[0] > 400f)
            {
                npc.ai[0] = 650f;
            }
            if (npc.type == NPCID.DesertDjinn && npc.ai[0] >= 360f)
            {
                npc.ai[0] = 650f;
            }
            if (npc.ai[0] >= 650f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = 2f;
                int num86 = (int)Main.player[npc.target].position.X / 16;
                int num87 = (int)Main.player[npc.target].position.Y / 16;
                int num88 = (int)npc.position.X / 16;
                int num89 = (int)npc.position.Y / 16;
                int num90 = 20;
                int num91 = 0;
                bool flag4 = false;
                if (Math.Abs(npc.position.X - Main.player[npc.target].position.X) + Math.Abs(npc.position.Y - Main.player[npc.target].position.Y) > 2000f)
                {
                    num91 = 100;
                    flag4 = true;
                }
                while (!flag4 && num91 < 100)
                {
                    num91++;
                    int num92 = Main.rand.Next(num86 - num90, num86 + num90);
                    int num93 = Main.rand.Next(num87 - num90, num87 + num90);
                    int num;
                    for (int num94 = num93; num94 < num87 + num90; num94 = num + 1)
                    {
                        if ((num94 < num87 - 4 || num94 > num87 + 4 || num92 < num86 - 4 || num92 > num86 + 4) && (num94 < num89 - 1 || num94 > num89 + 1 || num92 < num88 - 1 || num92 > num88 + 1) && Main.tile[num92, num94].HasUnactuatedTile)
                        {
                            bool flag5 = true;
                            if ((npc.type == NPCID.DarkCaster || (npc.type >= NPCID.RaggedCaster && npc.type <= NPCID.DiabolistWhite)) && !Main.wallDungeon[(int)Main.tile[num92, num94 - 1].WallType])
                            {
                                flag5 = false;
                            }
                            else if (Main.tile[num92, num94 - 1].LiquidType == LiquidID.Lava)
                            {
                                flag5 = false;
                            }
                            if (flag5 && Main.tileSolid[(int)Main.tile[num92, num94].TileType] && !Collision.SolidTiles(num92 - 1, num92 + 1, num94 - 4, num94 - 1))
                            {
                                npc.ai[1] = 20f;
                                npc.ai[2] = (float)num92;
                                npc.ai[3] = (float)num94;
                                flag4 = true;
                                break;
                            }
                        }
                        num = num94;
                    }
                }
                npc.netUpdate = true;
            }
            if (npc.ai[1] > 0f)
            {
                npc.ai[1] -= 1f;
                if (npc.type == NPCID.DesertDjinn)
                {
                    if (npc.ai[1] % 30f == 0f && npc.ai[1] / 30f < 5f)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, npc.position);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Point point = npc.Center.ToTileCoordinates();
                            Point point2 = Main.player[npc.target].Center.ToTileCoordinates();
                            Vector2 vector12 = Main.player[npc.target].Center - npc.Center;
                            int num95 = 6;
                            int num96 = 6;
                            int num97 = 0;
                            int num98 = 2;
                            int num99 = 0;
                            bool flag6 = false;
                            if (vector12.Length() > 2000f)
                            {
                                flag6 = true;
                            }
                            while (!flag6)
                            {
                                if (num99 >= 50)
                                {
                                    break;
                                }
                                num99++;
                                int num100 = Main.rand.Next(point2.X - num95, point2.X + num95 + 1);
                                int num101 = Main.rand.Next(point2.Y - num95, point2.Y + num95 + 1);
                                if ((num101 < point2.Y - num97 || num101 > point2.Y + num97 || num100 < point2.X - num97 || num100 > point2.X + num97) && (num101 < point.Y - num96 || num101 > point.Y + num96 || num100 < point.X - num96 || num100 > point.X + num96) && !Main.tile[num100, num101].HasUnactuatedTile)
                                {
                                    bool flag7 = true;
                                    if (flag7 && Main.tile[num100, num101].LiquidType == LiquidID.Lava)
                                    {
                                        flag7 = false;
                                    }
                                    if (flag7 && Collision.SolidTiles(num100 - num98, num100 + num98, num101 - num98, num101 + num98))
                                    {
                                        flag7 = false;
                                    }
                                    if (flag7)
                                    {
                                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), (float)(num100 * 16 + 8), (float)(num101 * 16 + 8), 0f, 0f, ProjectileID.DesertDjinnCurse, 0, 1f, Main.myPlayer, (float)npc.target, 0f);
                                        if (CalamityWorld.death)
                                            Main.projectile[proj].extraUpdates += 1;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
                else if (npc.ai[1] == 25f)
                {
                    if (npc.type >= NPCID.RaggedCaster && npc.type <= NPCID.DiabolistWhite)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float num102 = CalamityWorld.death ? 8f : 6f;
                            if (npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
                            {
                                num102 = CalamityWorld.death ? 10f : 8f;
                            }
                            if (npc.type == NPCID.RaggedCaster || npc.type == NPCID.RaggedCasterOpenCoat)
                            {
                                num102 = CalamityWorld.death ? 5f : 4f;
                            }
                            Vector2 vector13 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y);
                            float num103 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector13.X;
                            float num104 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector13.Y;
                            float num105 = (float)Math.Sqrt((double)(num103 * num103 + num104 * num104));
                            num105 = num102 / num105;
                            num103 *= num105;
                            num104 *= num105;
                            int damage = 16;
                            int projType = ProjectileID.ShadowBeamHostile;
                            if (npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
                            {
                                projType = ProjectileID.InfernoHostileBolt;
                                damage = 32;
                            }
                            if (npc.type == NPCID.RaggedCaster || npc.type == NPCID.RaggedCasterOpenCoat)
                            {
                                projType = ProjectileID.LostSoulHostile;
                                damage = 32;
                            }
                            int num108 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector13.X, vector13.Y, num103, num104, projType, damage, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[num108].timeLeft = 300;
                            if (projType == ProjectileID.InfernoHostileBolt)
                            {
                                Main.projectile[num108].ai[0] = Main.player[npc.target].Center.X;
                                Main.projectile[num108].ai[1] = Main.player[npc.target].Center.Y;
                                Main.projectile[num108].netUpdate = true;
                            }
                            npc.localAI[0] = 0f;
                        }
                    }
                    else
                    {
                        if (npc.type != NPCID.RuneWizard)
                        {
                            SoundEngine.PlaySound(SoundID.Item8, npc.position);
                        }
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (npc.type == NPCID.GoblinSorcerer || npc.type == NPCID.Tim)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X + npc.width / 2, (int)npc.position.Y - 8, NPCID.ChaosBall, 0, 0f, 0f, 0f, 0f, 255);
                            }
                            else if (npc.type == NPCID.DarkCaster)
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X + npc.width / 2, (int)npc.position.Y - 8, NPCID.WaterSphere, 0, 0f, 0f, 0f, 0f, 255);
                            }
                            else if (npc.type == NPCID.RuneWizard)
                            {
                                float num109 = CalamityWorld.death ? 12f : 10f;
                                Vector2 vector14 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                                float num110 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector14.X;
                                float num111 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector14.Y;
                                float num112 = (float)Math.Sqrt((double)(num110 * num110 + num111 * num111));
                                num112 = num109 / num112;
                                num110 *= num112;
                                num111 *= num112;
                                int num113 = 40;
                                int num114 = ProjectileID.RuneBlast;
                                int num115 = Projectile.NewProjectile(npc.GetSource_FromAI(), vector14.X, vector14.Y, num110, num111, num114, num113, 0f, Main.myPlayer, 0f, 0f);
                                Main.projectile[num115].timeLeft = 300;
                                npc.localAI[0] = 0f;
                            }
                            else
                            {
                                NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.position.X + npc.width / 2 + npc.direction * 8, (int)npc.position.Y + 20, NPCID.BurningSphere, 0, 0f, 0f, 0f, 0f, 255);
                            }
                        }
                    }
                }
            }
            if (npc.type == NPCID.GoblinSorcerer || npc.type == NPCID.Tim)
            {
                if (Main.rand.NextBool(5))
                {
                    int num116 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 27, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 1.5f);
                    Dust dust = Main.dust[num116];
                    dust.noGravity = true;
                    dust.velocity.X *= 0.5f;
                    dust.velocity.Y = -2f;
                }
            }
            else if (npc.type == NPCID.DarkCaster)
            {
                if (Main.rand.Next(3) != 0)
                {
                    int num117 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 172, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 0.9f);
                    Dust dust = Main.dust[num117];
                    dust.noGravity = true;
                    dust.velocity.X *= 0.3f;
                    dust.velocity.Y *= 0.2f;
                    dust.velocity.Y -= 1f;
                }
            }
            else
            {
                if (npc.type == NPCID.RuneWizard)
                {
                    int num118 = 1;
                    if (npc.alpha == 255)
                    {
                        num118 = 2;
                    }
                    int num;
                    for (int num119 = 0; num119 < num118; num119 = num + 1)
                    {
                        if (Main.rand.Next(255) > 255 - npc.alpha)
                        {
                            int num120 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 106, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 1.2f);
                            Dust dust = Main.dust[num120];
                            dust.noGravity = true;
                            dust.velocity.X *= (0.1f + (float)Main.rand.Next(30) * 0.01f);
                            dust.velocity.Y *= (0.1f + (float)Main.rand.Next(30) * 0.01f);
                            dust.scale *= 1f + (float)Main.rand.Next(6) * 0.1f;
                        }
                        num = num119;
                    }
                    return false;
                }
                if (npc.type == NPCID.Necromancer || npc.type == NPCID.NecromancerArmored)
                {
                    if (Main.rand.NextBool(2))
                    {
                        int num121 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 173, 0f, 0f, 0, default(Color), 1f);
                        Dust dust = Main.dust[num121];
                        dust.velocity.X *= 0.5f;
                        dust.velocity.Y *= 0.5f;
                    }
                }
                else if (npc.type == NPCID.DiabolistRed || npc.type == NPCID.DiabolistWhite)
                {
                    if (Main.rand.NextBool(2))
                    {
                        int num122 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 174, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 1f);
                        Dust dust = Main.dust[num122];
                        dust.noGravity = true;
                        dust.velocity *= 0.4f;
                        dust.velocity.Y -= 0.7f;
                        return false;
                    }
                }
                else if (npc.type == NPCID.RaggedCaster || npc.type == NPCID.RaggedCasterOpenCoat)
                {
                    if (Main.rand.NextBool(2))
                    {
                        int num123 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 175, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 0.1f);
                        Dust dust = Main.dust[num123];
                        dust.noGravity = true;
                        dust.velocity *= 0.5f;
                        dust.fadeIn = 1.2f;
                    }
                }
                else
                {
                    if (npc.type == NPCID.DesertDjinn)
                    {
                        Lighting.AddLight(npc.Top, 0.6f, 0.6f, 0.3f);
                        return false;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        int num124 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y + 2f), npc.width, npc.height, 6, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 2f);
                        Dust dust = Main.dust[num124];
                        dust.noGravity = true;
                        dust.velocity.X *= 1f;
                        dust.velocity.Y *= 1f;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Plant AI
        public static bool BuffedPlantAI(NPC npc, Mod mod)
        {
            if (npc.ai[0] < 0f || npc.ai[0] >= (float)Main.maxTilesX || npc.ai[1] < 0f || npc.ai[1] >= (float)Main.maxTilesX)
            {
                return false;
            }

            if (!Main.tile[(int)npc.ai[0], (int)npc.ai[1]].HasTile)
            {
                npc.life = -1;
                npc.HitEffect(0, 10.0);
                npc.active = false;
                return false;
            }

            FixExploitManEaters.ProtectSpot((int)npc.ai[0], (int)npc.ai[1]);

            npc.TargetClosest(true);

            float acceleration = 0.035f;
            float minDistance = 250f;
            switch (npc.type)
            {
                case NPCID.ManEater:
                    minDistance = 350f;
                    break;
                case NPCID.Clinger:
                    minDistance = 225f;
                    break;
                case NPCID.FungiBulb:
                    minDistance = 200f;
                    break;
                case NPCID.AngryTrapper:
                    acceleration = 0.05f;
                    minDistance = 500f;
                    break;
                case NPCID.GiantFungiBulb:
                    acceleration = 0.15f;
                    minDistance = 450f;
                    break;
            }

            if (CalamityWorld.death)
            {
                acceleration *= 1.25f;
                minDistance *= 1.25f;
            }

            float maxVelocity = 2f +
                (npc.type == NPCID.ManEater ? 1f : 0f) +
                (npc.type == NPCID.AngryTrapper ? 2f : 0f);

            npc.ai[2] += 1f;
            if (npc.ai[2] > 300f)
            {
                minDistance *= 1.3f;
                maxVelocity += 2f;
                if (npc.ai[2] > 450f)
                {
                    npc.ai[2] = 0f;
                }
            }

            Vector2 anchorPosition = new Vector2(npc.ai[0] * 16f + 8f, npc.ai[1] * 16f + 8f);
            Vector2 distanceVector = Main.player[npc.target].Center - anchorPosition;
            float distanceMagnitude = distanceVector.Length();
            if (distanceMagnitude > minDistance)
            {
                float normalizedMagnitude = minDistance / distanceMagnitude;
                distanceVector *= normalizedMagnitude;
            }
            if (npc.position.X < npc.ai[0] * 16f + 8f + distanceVector.X)
            {
                npc.velocity.X += acceleration;
                if (npc.velocity.X < 0f && distanceVector.X > 0f)
                {
                    npc.velocity.X += acceleration * 1.5f;
                }
            }
            else if (npc.position.X > npc.ai[0] * 16f + 8f + distanceVector.X)
            {
                npc.velocity.X -= acceleration;
                if (npc.velocity.X > 0f && distanceVector.X < 0f)
                {
                    npc.velocity.X -= acceleration * 1.5f;
                }
            }
            if (npc.position.Y < npc.ai[1] * 16f + 8f + distanceVector.Y)
            {
                npc.velocity.Y += acceleration;
                if (npc.velocity.Y < 0f && distanceVector.Y > 0f)
                {
                    npc.velocity.Y += acceleration * 1.5f;
                }
            }
            else if (npc.position.Y > npc.ai[1] * 16f + 8f + distanceVector.Y)
            {
                npc.velocity.Y -= acceleration;
                if (npc.velocity.Y > 0f && distanceVector.Y < 0f)
                {
                    npc.velocity.Y -= acceleration * 1.5f;
                }
            }

            npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-maxVelocity), new Vector2(maxVelocity));

            if (npc.type == NPCID.FungiBulb || npc.type == NPCID.GiantFungiBulb)
            {
                npc.rotation = npc.AngleTo(Main.player[npc.target].Center) + MathHelper.PiOver2;
            }
            else
            {
                npc.spriteDirection = (distanceVector.X > 0f).ToDirectionInt();
                npc.rotation = npc.AngleTo(Main.player[npc.target].Center) + (distanceVector.X < 0f).ToInt() * MathHelper.Pi;
            }

            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -0.7f;
                if (npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -0.7f;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 2f)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -2f)
                {
                    npc.velocity.Y = -2f;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.type == NPCID.Clinger && !Main.player[npc.target].dead)
                {
                    if (npc.justHit)
                        npc.localAI[0] = 0f;

                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 90f)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            int damage = 17;
                            int type = ProjectileID.CursedFlameHostile;
                            Vector2 flameVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * 12f;

                            int flame = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, flameVelocity, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[flame].timeLeft = 180;
                            npc.localAI[0] = 0f;
                        }
                        else
                        {
                            npc.localAI[0] = 75f;
                        }
                    }
                }
                if (npc.type == NPCID.GiantFungiBulb && !Main.player[npc.target].dead)
                {
                    if (npc.justHit)
                        npc.localAI[0] = 0f;

                    npc.localAI[0] += 1f;
                    if (npc.localAI[0] >= 120f)
                    {
                        if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            float speed = 16f;
                            distanceVector.X = Main.player[npc.target].Center.X - npc.Center.X;
                            float absoluteYDistance = Math.Abs(distanceVector.X * 0.1f);
                            if (Main.player[npc.target].Center.Y - npc.Center.Y > 0f)
                            {
                                absoluteYDistance = 0f;
                            }

                            Vector2 velocity = npc.SafeDirectionTo(Main.player[npc.target].Center - npc.Center - Vector2.UnitY * absoluteYDistance, -Vector2.UnitY) * speed;

                            int idx = NPC.NewNPC(npc.GetSource_FromAI(), (int)npc.Center.X, (int)npc.Center.Y, NPCID.FungiSpore, 0, 0f, 0f, 0f, 0f, 255);
                            Main.npc[idx].velocity = velocity;
                            Main.npc[idx].netUpdate = true;
                            npc.localAI[0] = 0f;
                            return false;
                        }
                        npc.localAI[0] = 120f;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Bat AI
        public static bool BuffedBatAI(NPC npc, Mod mod)
        {
            if (npc.type == NPCID.Hellbat || npc.type == NPCID.Lavabat)
            {
                int num203 = Dust.NewDust(npc.position, npc.width, npc.height, 6, npc.velocity.X * 0.2f, npc.velocity.Y * 0.2f, 100, default(Color), 2f);
                Main.dust[num203].noGravity = true;
            }
            if (npc.type == NPCID.IceBat && Main.rand.NextBool(10))
            {
                int num204 = Dust.NewDust(npc.position, npc.width, npc.height, 67, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                Main.dust[num204].noGravity = true;
                Dust dust = Main.dust[num204];
                dust.velocity *= 0.2f;
                Main.dust[num204].noLight = true;
            }
            npc.noGravity = true;
            // Collision on the X axis.
            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.5f;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            // Collision on Y axis.
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                {
                    npc.velocity.Y = 1f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                {
                    npc.velocity.Y = -1f;
                }
            }
            if (npc.type == NPCID.FlyingSnake)
            {
                int direction = 1;
                int directionY = 1;
                if (npc.velocity.X < 0f)
                {
                    direction = -1;
                }
                if (npc.velocity.Y < 0f)
                {
                    directionY = -1;
                }
                npc.TargetClosest(true);
                if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.direction = direction;
                    npc.directionY = directionY;
                }
            }
            else
            {
                npc.TargetClosest(true);
            }
            float maxSpeedX = CalamityWorld.death ? 6f : 5f;
            float maxSpeedY = CalamityWorld.death ? 2.5f : 2f;

            float xAccel = 0.12f;
            float xAccelBoost1 = 0.12f;
            float xAccelBoost2 = 0.06f;

            float yAccel = 0.06f;
            float yAccelBoost1 = 0.07f;
            float yAccelBoost2 = 0.05f;

            if (npc.type == NPCID.VampireBat)
            {
                if (npc.position.Y < Main.worldSurface * 16.0 && Main.dayTime && !Main.eclipse)
                {
                    npc.directionY = -1;
                    npc.direction *= -1;
                }
                maxSpeedX = maxSpeedY = CalamityWorld.death ? 11f : 9f;
                xAccel = yAccel = 0.3f;
                xAccelBoost1 = yAccelBoost1 = 0.12f;
                xAccelBoost2 = yAccelBoost2 = 0.07f;
            }
            else if (npc.type == NPCID.FlyingSnake)
            {
                maxSpeedX = CalamityWorld.death ? 9f : 6f;
                maxSpeedY = CalamityWorld.death ? 5f : 3.5f;

                xAccel = 0.3f;
                xAccelBoost1 = 0.12f;
                xAccelBoost2 = 0.07f;

                yAccel = 0.12f;
                yAccelBoost1 = 0.07f;
                yAccelBoost2 = 0.05f;
            }
            DemonEyeAI.DemonEyeBatMovement(npc, maxSpeedX, maxSpeedY, xAccel, xAccelBoost1, xAccelBoost2, yAccel, yAccelBoost1, yAccelBoost2);
            if (npc.type == NPCID.CaveBat ||
                npc.type == NPCID.JungleBat ||
                npc.type == NPCID.Hellbat ||
                npc.type == NPCID.Demon ||
                npc.type == NPCID.VoodooDemon ||
                npc.type == NPCID.GiantBat ||
                npc.type == NPCID.IlluminantBat ||
                npc.type == NPCID.IceBat ||
                npc.type == NPCID.Lavabat ||
                npc.type == NPCID.GiantFlyingFox ||
                npc.type == ModContent.NPCType<Melter>())
            {
                maxSpeedX = CalamityWorld.death ? 6f : 5f;
                maxSpeedY = CalamityWorld.death ? 2.5f : 2f;
                if (npc.wet)
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y *= 0.95f;
                    }
                    npc.velocity.Y -= 0.6f;
                    if (npc.velocity.Y < -5f)
                    {
                        npc.velocity.Y = -5f;
                    }
                    npc.TargetClosest(true);
                }
                if (npc.type == NPCID.Hellbat)
                {
                    xAccel = 0.12f;
                    xAccelBoost1 = 0.09f;
                    xAccelBoost2 = 0.05f;

                    yAccel = 0.06f;
                    yAccelBoost1 = 0.05f;
                    yAccelBoost2 = 0.03f;
                }
                else
                {
                    xAccel = 0.12f;
                    xAccelBoost1 = 0.12f;
                    xAccelBoost2 = 0.07f;

                    yAccel = 0.06f;
                    yAccelBoost1 = 0.07f;
                    yAccelBoost2 = 0.05f;
                }
                DemonEyeAI.DemonEyeBatMovement(npc, maxSpeedX, maxSpeedY, xAccel, xAccelBoost1, xAccelBoost2, yAccel, yAccelBoost1, yAccelBoost2);
            }
            if (npc.type == NPCID.Harpy && npc.wet)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y *= 0.95f;
                }
                npc.velocity.Y -= 0.6f;
                if (npc.velocity.Y < -5f)
                {
                    npc.velocity.Y = -5f;
                }
                npc.TargetClosest(true);
            }
            // Turn back into a walking bat when possible
            if (npc.type == NPCID.VampireBat && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.Distance(Main.player[npc.target].Center) < 200f &&
                    npc.Center.Y < Main.player[npc.target].Center.Y &&
                    Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.Transform(NPCID.Vampire);
                }
            }
            npc.ai[1] += 2f;
            if (npc.type == NPCID.VampireBat)
            {
                npc.ai[1] += 1f;
            }
            if (npc.ai[1] > 200f)
            {
                if (!Main.player[npc.target].wet && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                {
                    npc.ai[1] = 0f;
                }
                xAccel = 0.22f;
                yAccel = 0.11f;
                float maxVelocityX = 4.4f;
                float maxVelocityY = 1.6f;
                if (npc.ai[1] > 1000f)
                {
                    npc.ai[1] = 0f;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] > 0f)
                {
                    if (npc.velocity.Y < maxVelocityY)
                    {
                        npc.velocity.Y += yAccel;
                    }
                }
                else if (npc.velocity.Y > -maxVelocityY)
                {
                    npc.velocity.Y -= yAccel;
                }
                if (npc.ai[2] < -150f || npc.ai[2] > 150f)
                {
                    if (npc.velocity.X < maxVelocityX)
                    {
                        npc.velocity.X += xAccel;
                    }
                }
                else if (npc.velocity.X > -maxVelocityX)
                {
                    npc.velocity.X -= xAccel;
                }
                if (npc.ai[2] > 300f)
                {
                    npc.ai[2] = -300f;
                }
            }
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.justHit)
                    npc.ai[0] = 0f;

                if (npc.type == NPCID.Harpy)
                {
                    npc.ai[0] += 1f;
                    if (npc.ai[0] % 30f == 0f && npc.ai[0] <= 120f)
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            int damage = 15;
                            int type = ProjectileID.HarpyFeather;
                            Vector2 featherVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * (CalamityWorld.death ? 4f : 6f);

                            int feather = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, featherVelocity, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[feather].timeLeft = 300;
                            if (CalamityWorld.death)
                            {
                                Main.projectile[feather].extraUpdates += 1;
                                Main.projectile[feather].timeLeft = 600;
                            }
                        }
                    }
                    else if (npc.ai[0] >= 400f + Main.rand.Next(200))
                    {
                        npc.ai[0] = 0f;
                    }
                }
                if (npc.type == NPCID.Demon || npc.type == NPCID.VoodooDemon)
                {
                    npc.ai[0] += 1f;
                    if (npc.ai[0] % 20f == 0f && npc.ai[0] <= 100f)
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            int damage = 21;
                            int type = ProjectileID.DemonSickle;
                            Vector2 sickleVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * (CalamityWorld.death ? 0.15f : 0.2f);

                            int sickle = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, sickleVelocity, type, damage, 0f, Main.myPlayer, 0f, 0f);
                            Main.projectile[sickle].timeLeft = 300;
                            if (CalamityWorld.death)
                            {
                                Main.projectile[sickle].extraUpdates += 1;
                                Main.projectile[sickle].timeLeft = 600;
                            }
                        }
                    }
                    else if (npc.ai[0] >= 300f + Main.rand.Next(200))
                    {
                        npc.ai[0] = 0f;
                    }
                }
                if (npc.type == NPCID.RedDevil)
                {
                    npc.ai[0] += 1f;
                    if (npc.ai[0] % 20f == 0f && npc.ai[0] <= 100f)
                    {
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                        {
                            Vector2 spawnPosition = npc.Center;

                            float tridentSpeed = CalamityWorld.death ? 0.15f : 0.2f;
                            Vector2 tridentVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center) * tridentSpeed;
                            spawnPosition += npc.velocity * 5f;

                            int damage = 80;
                            int type = ProjectileID.UnholyTridentHostile;
                            int trident = Projectile.NewProjectile(npc.GetSource_FromAI(), spawnPosition + tridentVelocity * 100f, tridentVelocity, type, damage, 3f, Main.myPlayer, 0f, 0f);
                            Main.projectile[trident].timeLeft = 300;
                            if (CalamityWorld.death)
                            {
                                Main.projectile[trident].extraUpdates += 1;
                                Main.projectile[trident].timeLeft = 600;
                            }
                        }
                    }
                    else if (npc.ai[0] >= 250f + Main.rand.Next(200))
                    {
                        npc.ai[0] = 0f;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Swimming AI
        public static bool BuffedSwimmingAI(NPC npc, Mod mod)
        {
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (npc.wet)
            {
                bool noWetTargets = false;
                npc.TargetClosest(false);
                if ((Main.player[npc.target].wet || (CalamityWorld.death && npc.Distance(Main.player[npc.target].Center) < 400f)) && !Main.player[npc.target].dead)
                {
                    noWetTargets = true;
                }
                if (!noWetTargets)
                {
                    if (npc.collideX)
                    {
                        npc.velocity.X *= -1f;
                        npc.direction *= -1;
                        npc.netUpdate = true;
                    }
                    if (npc.collideY)
                    {
                        npc.netUpdate = true;
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                            npc.directionY = -1;
                            npc.ai[0] = -1f;
                        }
                        else if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = Math.Abs(npc.velocity.Y);
                            npc.directionY = 1;
                            npc.ai[0] = 1f;
                        }
                    }
                }
                if (npc.type == NPCID.AnglerFish)
                {
                    Lighting.AddLight((int)(npc.position.X + (float)(npc.width / 2) + (float)(npc.direction * (npc.width + 8))) / 16, (int)(npc.position.Y + 2f) / 16, 0.07f, 0.04f, 0.025f);
                }
                if (noWetTargets)
                {
                    npc.TargetClosest(true);
                    if (npc.type == NPCID.Arapaima)
                    {
                        // Check if the direction value signs match
                        if ((npc.velocity.X > 0).ToDirectionInt() != (npc.velocity.X > 0).ToDirectionInt())
                        {
                            npc.velocity.X *= 0.95f;
                        }
                        npc.velocity.X += npc.direction * 0.5f;
                        npc.velocity.Y += npc.directionY * 0.4f;

                        // I don't really understand why a boundary break penalty of 2 is used here, but just to be safe, I'll leave it alone.
                        if (npc.velocity.X > 16f)
                        {
                            npc.velocity.X = 14f;
                        }
                        if (npc.velocity.X < -16f)
                        {
                            npc.velocity.X = -14f;
                        }
                        if (npc.velocity.Y > 10f)
                        {
                            npc.velocity.Y = 8f;
                        }
                        if (npc.velocity.Y < -10f)
                        {
                            npc.velocity.Y = -8f;
                        }
                    }
                    else if (npc.type == NPCID.Shark || npc.type == NPCID.AnglerFish)
                    {
                        npc.velocity.X += npc.direction * 0.3f;
                        npc.velocity.Y += npc.directionY * 0.3f;
                        if (npc.velocity.X > 10f)
                        {
                            npc.velocity.X = 10f;
                        }
                        if (npc.velocity.X < -10f)
                        {
                            npc.velocity.X = -10f;
                        }
                        if (npc.velocity.Y > 6f)
                        {
                            npc.velocity.Y = 6f;
                        }
                        if (npc.velocity.Y < -6f)
                        {
                            npc.velocity.Y = -6f;
                        }
                        npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-10f, -6f), new Vector2(10f, 6f));
                    }
                    else
                    {
                        npc.velocity.X += npc.direction * 0.2f;
                        npc.velocity.Y += npc.directionY * 0.2f;
                        npc.velocity = Vector2.Clamp(npc.velocity, new Vector2(-6f, -4f), new Vector2(6f, 4f));
                    }
                }
                else
                {
                    if (npc.type == NPCID.Arapaima)
                    {
                        npc.directionY = (Main.player[npc.target].position.Y > npc.position.Y).ToDirectionInt();
                        npc.velocity.X += npc.direction * 0.2f;
                        if (npc.velocity.X < -2f || npc.velocity.X > 2f)
                        {
                            npc.velocity.X *= 0.95f;
                        }
                        // Bob up and down in the water
                        if (npc.ai[0] == -1f)
                        {
                            float yVelocityMin = -0.6f;
                            if (npc.directionY < 0)
                            {
                                yVelocityMin = -1f;
                            }
                            if (npc.directionY > 0)
                            {
                                yVelocityMin = -0.2f;
                            }
                            npc.velocity.Y -= 0.02f;
                            if (npc.velocity.Y < yVelocityMin)
                            {
                                npc.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            float yVelocityMin = 0.6f;
                            if (npc.directionY < 0)
                            {
                                yVelocityMin = 0.2f;
                            }
                            if (npc.directionY > 0)
                            {
                                yVelocityMin = 1f;
                            }
                            npc.velocity.Y += 0.02f;
                            if (npc.velocity.Y > yVelocityMin)
                            {
                                npc.ai[0] = -1f;
                            }
                        }
                    }
                    else
                    {
                        npc.velocity.X += npc.direction * 0.1f;
                        if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                        {
                            npc.velocity.X *= 0.95f;
                        }
                        if (npc.ai[0] == -1f)
                        {
                            npc.velocity.Y -= 0.01f;
                            if (npc.velocity.Y < -0.3f)
                            {
                                npc.ai[0] = 1f;
                            }
                        }
                        else
                        {
                            npc.velocity.Y += 0.01f;
                            if (npc.velocity.Y > 0.3)
                            {
                                npc.ai[0] = -1f;
                            }
                        }
                    }
                    int x = (int)npc.Center.X / 16;
                    int y = (int)npc.Center.Y / 16;
                    if (Main.tile[x, y - 1].LiquidAmount > 128)
                    {
                        if (Main.tile[x, y + 1].HasTile)
                        {
                            npc.ai[0] = -1f;
                        }
                        else if (Main.tile[x, y + 2].HasTile)
                        {
                            npc.ai[0] = -1f;
                        }
                    }
                    if (npc.type != NPCID.Arapaima && Math.Abs(npc.velocity.Y) < 0.4f)
                    {
                        npc.velocity.Y *= 0.95f;
                    }
                }
            }
            else
            {
                if (npc.velocity.Y == 0f)
                {
                    // Sit helplessly on land and do absolutely nothing.
                    if (npc.type == NPCID.Shark)
                    {
                        npc.velocity.X *= 0.94f;
                        if (Math.Abs(npc.velocity.X) < 0.2)
                        {
                            npc.velocity.X = 0f;
                        }
                    }
                    // Flop around
                    else if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.velocity.Y = Main.rand.NextFloat(-5f, -2f);
                        npc.velocity.X = Main.rand.NextFloat(-2f, -2f);
                        npc.netUpdate = true;
                    }
                }
                npc.velocity.Y += 0.3f;
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
                npc.ai[0] = 1f;
            }
            npc.rotation = npc.velocity.Y * npc.direction * 0.1f;
            npc.rotation = MathHelper.Clamp(npc.rotation, -0.2f, 0.2f);
            return false;
        }
        #endregion

        #region Jellyfish AI
        public static bool BuffedJellyfishAI(NPC npc, Mod mod)
        {
            // Stop moving because we're emitting electricity and don't take damage
            bool endEarly = false;
            if (npc.wet && npc.ai[1] == 1f)
            {
                endEarly = true;
            }
            else
            {
                npc.dontTakeDamage = false;
            }
            if (npc.type == NPCID.BlueJellyfish || npc.type == NPCID.PinkJellyfish || npc.type == NPCID.GreenJellyfish || npc.type == NPCID.BloodJelly)
            {
                if (npc.wet)
                {
                    if (npc.target >= 0 && Main.player[npc.target].wet && !Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 200f)
                    {
                        if (npc.ai[1] == 0f)
                        {
                            npc.ai[2] += 2f;
                        }
                        else
                        {
                            npc.ai[2] -= 0.25f;
                        }
                    }
                    if (endEarly)
                    {
                        npc.dontTakeDamage = true;
                        npc.ai[2] += 1f;
                        if (npc.ai[2] >= 90f)
                        {
                            npc.ai[1] = 0f;
                        }
                    }
                    else
                    {
                        npc.ai[2] += 1f;
                        if (npc.ai[2] >= 300f)
                        {
                            npc.ai[1] = 1f;
                            npc.ai[2] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }
            float lightIntensity = 1f;
            if (endEarly)
            {
                lightIntensity += 0.5f;
            }
            if (npc.type == NPCID.BlueJellyfish)
            {
                Lighting.AddLight((int)(npc.Center.X) / 16, (int)(npc.Center.Y) / 16, 0.05f * lightIntensity, 0.15f * lightIntensity, 0.4f * lightIntensity);
            }
            else if (npc.type == NPCID.GreenJellyfish)
            {
                Lighting.AddLight((int)(npc.Center.X) / 16, (int)(npc.Center.Y) / 16, 0.05f * lightIntensity, 0.45f * lightIntensity, 0.1f * lightIntensity);
            }
            else if (npc.type != NPCID.Squid && npc.type != NPCID.BloodJelly)
            {
                Lighting.AddLight((int)(npc.Center.X) / 16, (int)(npc.Center.Y) / 16, 0.35f * lightIntensity, 0.05f * lightIntensity, 0.2f * lightIntensity);
            }
            if (npc.direction == 0)
            {
                npc.TargetClosest(true);
            }
            if (endEarly)
            {
                return false;
            }
            if (!npc.wet)
            {
                npc.rotation += npc.velocity.X * 0.1f;
                if (npc.velocity.Y == 0f)
                {
                    npc.velocity.X *= 0.98f;
                    if (Math.Abs(npc.velocity.X) < 0.01f)
                    {
                        npc.velocity.X = 0f;
                    }
                }
                npc.velocity.Y += 0.2f;
                if (npc.velocity.Y > 10f)
                {
                    npc.velocity.Y = 10f;
                }
                npc.ai[0] = 1f;
                return false;
            }
            // Collision
            // Turn around on X collision
            if (npc.collideX)
            {
                npc.velocity.X *= 1f;
                npc.direction *= -1;
            }
            // Manipulate the sign of the Y velocity
            if (npc.collideY)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = Math.Abs(npc.velocity.Y) * -1f;
                    npc.directionY = -1;
                    npc.ai[0] = -1f;
                }
                else if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = Math.Abs(npc.velocity.Y);
                    npc.directionY = 1;
                    npc.ai[0] = 1f;
                }
            }
            bool targetInWater = false;
            if (!npc.friendly)
            {
                npc.TargetClosest(false);
                if ((Main.player[npc.target].wet || (CalamityWorld.death && npc.Distance(Main.player[npc.target].Center) < 400f)) && !Main.player[npc.target].dead)
                {
                    targetInWater = true;
                }
            }
            // Slow down. When slow enough, charge again.
            if (targetInWater)
            {
                npc.localAI[2] = 1f;
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver2;
                npc.velocity *= 0.96f;
                float minimumSpeed = 0.2f;
                if (npc.type == NPCID.GreenJellyfish)
                {
                    npc.velocity *= 0.98f;
                    minimumSpeed = 0.6f;
                }
                if (npc.type == NPCID.Squid)
                {
                    npc.velocity *= 0.99f;
                    minimumSpeed = 1f;
                }
                if (npc.type == NPCID.BloodJelly)
                {
                    npc.velocity *= 0.995f;
                    minimumSpeed = 3f;
                }
                minimumSpeed *= 0.8f;
                if (npc.velocity.Length() < minimumSpeed)
                {
                    if (npc.type == NPCID.Squid)
                    {
                        npc.localAI[0] = 1f;
                    }
                    npc.TargetClosest(true);

                    float lungeSpeed = npc.type == NPCID.GreenJellyfish ? 18f : 14f;
                    npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * lungeSpeed;
                }
            }
            // General floating around.
            else
            {
                npc.localAI[2] = 0f;
                npc.velocity.X += npc.direction * 0.02f;
                npc.rotation = npc.velocity.X * 0.4f;
                if (npc.velocity.X < -1f || npc.velocity.X > 1f)
                {
                    npc.velocity.X *= 0.95f;
                }
                if (npc.ai[0] == -1f)
                {
                    npc.velocity.Y -= 0.01f;
                    if (npc.velocity.Y < -1f)
                    {
                        npc.ai[0] = 1f;
                    }
                }
                else
                {
                    npc.velocity.Y += 0.01f;
                    if (npc.velocity.Y > 1f)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                int x = (int)npc.Center.X / 16;
                int y = (int)npc.Center.Y / 16;
                if (Main.tile[x, y - 1].LiquidAmount > 128)
                {
                    if (Main.tile[x, y + 1].HasTile)
                    {
                        npc.ai[0] = -1f;
                    }
                    else if (Main.tile[x, y + 2].HasTile)
                    {
                        npc.ai[0] = -1f;
                    }
                }
                else
                {
                    npc.ai[0] = 1f;
                }
                if (Math.Abs(npc.velocity.Y) > 1.2)
                {
                    npc.velocity.Y *= 0.99f;
                }
            }
            return false;
        }
        #endregion

        #region Antlion AI
        public static bool BuffedAntlionAI(NPC npc, Mod mod)
        {
            npc.TargetClosest(true);

            //Calculate speed and velocity of the sand balls
            float speed = 12f;
            float xVel = Main.player[npc.target].Center.X - npc.Center.X;
            float yVel = Main.player[npc.target].position.Y - npc.Center.Y;
            Vector2 velocity = new Vector2(xVel, yVel);
            float targetDist = velocity.Length();

            targetDist = speed / targetDist;
            velocity.X *= targetDist;
            velocity.Y *= targetDist;

            //Adjust rotation and velocity
            bool canShoot = false;
            if (npc.directionY < 0)
            {
                //Set rotation based on the target location
                npc.rotation = velocity.ToRotation() + MathHelper.PiOver2;
                //Antlions can only shoot if rotated between a certain cone of spread based on the target location
                canShoot = Math.Abs(npc.rotation) <= 1.2f;

                //Hardcap rotation so it doesn't look weird, but since the above calculation happens first, it ignores this cap
                if (npc.rotation < -0.8f)
                {
                    npc.rotation = -0.8f;
                }
                else if (npc.rotation > 0.8f)
                {
                    npc.rotation = 0.8f;
                }

                //Antlions generally don't move horizontally so prevent that as needed
                if (npc.velocity.X != 0f)
                {
                    npc.velocity.X *= 0.9f;
                    if (Math.Abs(npc.velocity.X) < 0.1f)
                    {
                        npc.netUpdate = true;
                        npc.velocity.X = 0f;
                    }
                }
            }

            if (npc.justHit)
                npc.ai[0] = 199f;

            //Decrement the firing cooldown, play a sound if at full meaning it just fired
            if (npc.ai[0] > 0f)
            {
                if (npc.ai[0] == 200f)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath13, npc.position);
                }
                npc.ai[0] -= 1f;
            }

            //Antlions should only fire if the target is in the shooting cone and has a line of sight as well as not being on cooldown.
            bool lineofSight = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
            if (Main.netMode != NetmodeID.MultiplayerClient && canShoot && npc.ai[0] == 0f && lineofSight)
            {
                //Reset the firing cooldown to 3.3333 seconds
                npc.ai[0] = 200f;

                //With the Rev and Death damage calculations, this becomes 56 damage.
                int damage = 10;

                int projType = ProjectileID.SandBallFalling;

                // 2 to 3 in Rev, 3 to 5 in Death, if FTW is also enabled, 8 to 13 (random chance for 10x the amount)
                int projAmt = Main.rand.Next(CalamityWorld.death ? 3 : 2, CalamityWorld.death ? 6 : 4);
                if (Main.getGoodWorld)
                {
                    projAmt = Main.rand.Next(8, 14);
                    if (Main.rand.NextBool(1000) || Main.zenithWorld)
                        projAmt = Main.rand.Next(80, 131);
                }

                for (int i = 0; i < projAmt; i++)
                {
                    //Adjust the velocity to make it a shotgun-like spread
                    velocity.X += (float)Main.rand.Next(-30, 31) * 0.05f;
                    velocity.Y += (float)Main.rand.Next(-30, 31) * 0.05f;

                    int sandBall = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, projType, damage, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[sandBall].ai[0] = 2f;
                    Main.projectile[sandBall].timeLeft = 300;
                    Main.projectile[sandBall].friendly = false;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, sandBall, 0f, 0f, 0f, 0, 0, 0);
                }
                npc.netUpdate = true;
            }

            try
            {
                //This tile checking behavior is used for when Antlions cover themselves in sand and need to rise upward to get to the surface
                int xLeft = (int)npc.position.X / 16;
                int xCenter = (int)npc.Center.X / 16;
                int xRight = (int)(npc.position.X + (float)npc.width) / 16;
                int y = (int)(npc.position.Y + (float)npc.height) / 16;
                bool tileClimbing = false;
                if ((Main.tile[xLeft, y].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[xLeft, y].TileType]) || (Main.tile[xCenter, y].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[xCenter, y].TileType]) || (Main.tile[xRight, y].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[xRight, y].TileType]))
                {
                    tileClimbing = true;
                }
                if (tileClimbing)
                {
                    npc.noGravity = true;
                    npc.noTileCollide = true;
                    npc.velocity.Y = -0.2f;
                }

                //If not rising up through tiles, occasionally spawn some dust
                else
                {
                    npc.noGravity = false;
                    npc.noTileCollide = false;
                    if (Main.rand.NextBool(2))
                    {
                        int sand = Dust.NewDust(new Vector2(npc.position.X - 4f, npc.position.Y + (float)npc.height - 8f), npc.width + 8, 24, 32, 0f, npc.velocity.Y / 2f, 0, default(Color), 1f);
                        Dust dust = Main.dust[sand];
                        dust.velocity.X *= 0.4f;
                        dust.velocity.Y *= -1f;
                        if (Main.rand.NextBool(2))
                        {
                            dust.noGravity = true;
                            dust.scale += 0.2f;
                        }
                    }
                }
            }
            catch
            {
            }
            return false;
        }
        #endregion

        #region Spike Ball AI
        public static bool BuffedSpikeBallAI(NPC npc, Mod mod)
        {
            if (npc.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.TargetClosest(true);
                    npc.direction *= -1;
                    npc.directionY *= -1;
                    npc.position.Y = npc.position.Y + (float)(npc.height / 2 + 8);
                    npc.ai[1] = npc.position.X + (float)(npc.width / 2);
                    npc.ai[2] = npc.position.Y + (float)(npc.height / 2);
                    if (npc.direction == 0)
                    {
                        npc.direction = 1;
                    }
                    if (npc.directionY == 0)
                    {
                        npc.directionY = 1;
                    }
                    npc.ai[3] = 1f + (float)Main.rand.Next(15) * 0.1f;
                    npc.velocity.Y = (float)(npc.directionY * 6) * npc.ai[3];
                    npc.ai[0] += 1f;
                    npc.netUpdate = true;
                    return false;
                }
                npc.ai[1] = npc.position.X + (float)(npc.width / 2);
                npc.ai[2] = npc.position.Y + (float)(npc.height / 2);
            }
            else
            {
                float num282 = (CalamityWorld.death ? 12f : 9f) * npc.ai[3];
                float num283 = (CalamityWorld.death ? 0.4f : 0.3f) * npc.ai[3];
                float num284 = num282 / num283 / 2f;
                if (npc.ai[0] >= 1f && npc.ai[0] < (float)((int)num284))
                {
                    npc.velocity.Y = (float)npc.directionY * num282;
                    npc.ai[0] += 1f;
                    return false;
                }
                if (npc.ai[0] >= (float)((int)num284))
                {
                    npc.velocity.Y = 0f;
                    npc.directionY *= -1;
                    npc.velocity.X = num282 * (float)npc.direction;
                    npc.ai[0] = -1f;
                    return false;
                }
                if (npc.directionY > 0)
                {
                    if (npc.velocity.Y >= num282)
                    {
                        npc.directionY *= -1;
                        npc.velocity.Y = num282;
                    }
                }
                else if (npc.directionY < 0 && npc.velocity.Y <= -num282)
                {
                    npc.directionY *= -1;
                    npc.velocity.Y = -num282;
                }
                if (npc.direction > 0)
                {
                    if (npc.velocity.X >= num282)
                    {
                        npc.direction *= -1;
                        npc.velocity.X = num282;
                    }
                }
                else if (npc.direction < 0 && npc.velocity.X <= -num282)
                {
                    npc.direction *= -1;
                    npc.velocity.X = -num282;
                }
                npc.velocity.X = npc.velocity.X + num283 * (float)npc.direction;
                npc.velocity.Y = npc.velocity.Y + num283 * (float)npc.directionY;
            }
            return false;
        }
        #endregion

        #region Blazing Wheel AI
        public static bool BuffedBlazingWheelAI(NPC npc, Mod mod)
        {
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                npc.directionY = 1;
                npc.ai[0] = 1f;
            }
            int num285 = CalamityWorld.death ? 9 : 6;
            if (npc.ai[1] == 0f)
            {
                npc.rotation += (float)(npc.direction * npc.directionY) * 0.13f;
                if (npc.collideY)
                {
                    npc.ai[0] = 2f;
                }
                if (!npc.collideY && npc.ai[0] == 2f)
                {
                    npc.direction = -npc.direction;
                    npc.ai[1] = 1f;
                    npc.ai[0] = 1f;
                }
                if (npc.collideX)
                {
                    npc.directionY = -npc.directionY;
                    npc.ai[1] = 1f;
                }
            }
            else
            {
                npc.rotation -= (float)(npc.direction * npc.directionY) * 0.13f;
                if (npc.collideX)
                {
                    npc.ai[0] = 2f;
                }
                if (!npc.collideX && npc.ai[0] == 2f)
                {
                    npc.directionY = -npc.directionY;
                    npc.ai[1] = 0f;
                    npc.ai[0] = 1f;
                }
                if (npc.collideY)
                {
                    npc.direction = -npc.direction;
                    npc.ai[1] = 0f;
                }
            }
            npc.velocity.X = (float)(num285 * npc.direction);
            npc.velocity.Y = (float)(num285 * npc.directionY);
            float num286 = (float)(270 - (int)Main.mouseTextColor) / 400f;
            Lighting.AddLight((int)(npc.position.X + (float)(npc.width / 2)) / 16, (int)(npc.position.Y + (float)(npc.height / 2)) / 16, 0.9f, 0.3f + num286, 0.2f);
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= (CalamityWorld.death ? 90f : 120f))
                {
                    npc.localAI[0] = 0f;
                    for (int num1501 = 0; num1501 < 4; num1501++)
                    {
                        Vector2 vector255 = new Vector2(0f, -5f).RotatedBy((double)(MathHelper.PiOver2 * (float)num1501));
                        int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, vector255.X, vector255.Y, ProjectileID.FlamesTrap, 20, 0f, Main.myPlayer, 0f, 0f);
                        Main.projectile[proj].tileCollide = false;
                        Main.projectile[proj].friendly = false;
                        Main.projectile[proj].trap = false;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Hovering AI
        public static bool BuffedHoveringAI(NPC npc, Mod mod)
        {
            bool flag19 = false;
            bool flag20 = npc.type == NPCID.Poltergeist && !Main.pumpkinMoon;

            if (npc.type == NPCID.Reaper && !Main.eclipse)
            {
                flag20 = true;
            }
            if (npc.type == NPCID.Drippler && Main.dayTime)
            {
                flag20 = true;
            }

            if (!flag20)
            {
                if (npc.ai[2] >= 0f)
                {
                    int num287 = 16;
                    bool flag21 = false;
                    bool flag22 = false;
                    if (npc.position.X > npc.ai[0] - (float)num287 && npc.position.X < npc.ai[0] + (float)num287)
                    {
                        flag21 = true;
                    }
                    else if ((npc.velocity.X < 0f && npc.direction > 0) || (npc.velocity.X > 0f && npc.direction < 0))
                    {
                        flag21 = true;
                    }
                    num287 += 24;
                    if (npc.position.Y > npc.ai[1] - (float)num287 && npc.position.Y < npc.ai[1] + (float)num287)
                    {
                        flag22 = true;
                    }
                    if (flag21 & flag22)
                    {
                        npc.ai[2] += 1f;
                        if (npc.ai[2] >= 40f)
                        {
                            npc.ai[2] = -200f;
                            npc.direction *= -1;
                            npc.velocity.X = npc.velocity.X * -1f;
                            npc.collideX = false;
                        }
                    }
                    else
                    {
                        npc.ai[0] = npc.position.X;
                        npc.ai[1] = npc.position.Y;
                        npc.ai[2] = 0f;
                    }
                    npc.TargetClosest(true);
                }
                else if (npc.type == NPCID.Reaper)
                {
                    npc.TargetClosest(true);
                    npc.ai[2] += 30f;
                }
                else
                {
                    if (npc.type == NPCID.Poltergeist)
                    {
                        npc.ai[2] += 5f;
                    }
                    else
                    {
                        npc.ai[2] += 15f;
                    }
                    if (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) > npc.position.X + (float)(npc.width / 2))
                    {
                        npc.direction = -1;
                    }
                    else
                    {
                        npc.direction = 1;
                    }
                }
            }

            int num288 = (int)((npc.position.X + (float)(npc.width / 2)) / 16f) + npc.direction * 2;
            int num289 = (int)((npc.position.Y + (float)npc.height) / 16f);
            bool flag23 = true;
            bool flag24 = false;
            int num290 = 6;

            if (npc.type == NPCID.Gastropod)
            {
                float num291 = 6f;
                Vector2 vector33 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num292 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector33.X;
                float num293 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector33.Y;
                float num294 = (float)Math.Sqrt((double)(num292 * num292 + num293 * num293));

                num294 = num291 / num294;
                num292 *= num294;
                num293 *= num294;

                if (npc.justHit)
                {
                    npc.localAI[1] = 0f;
                    npc.ai[3] = 0f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] == 32f && !Main.player[npc.target].npcTypeNoAggro[npc.type])
                {
                    int damage = 25;
                    int projType = ProjectileID.PinkLaser;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), vector33.X, vector33.Y, num292, num293, projType, damage, 0f, Main.myPlayer, 0f, 0f);
                }

                num290 = 12;

                if (npc.ai[3] > 0f)
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= 64f)
                    {
                        npc.ai[3] = 0f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] == 0f)
                {
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] > (CalamityWorld.death ? 60f : 120f) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) && !Main.player[npc.target].npcTypeNoAggro[npc.type])
                    {
                        npc.localAI[1] = 0f;
                        npc.ai[3] = 1f;
                        npc.netUpdate = true;
                    }
                }
            }
            else if (npc.type == NPCID.Pixie)
            {
                num290 = 8;

                if (Main.rand.NextBool(6))
                {
                    int num298 = Dust.NewDust(npc.position, npc.width, npc.height, 55, 0f, 0f, 200, npc.color, 1f);
                    Dust dust = Main.dust[num298];
                    dust.velocity *= 0.3f;
                }

                if (Main.rand.NextBool(40))
                {
                    SoundEngine.PlaySound(SoundID.Pixie, npc.position);
                }
            }
            else if (npc.type == NPCID.IceElemental)
            {
                Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0f, 0.6f, 0.75f);

                npc.alpha = 30;

                if (Main.rand.NextBool(3))
                {
                    int num299 = Dust.NewDust(npc.position, npc.width, npc.height, 92, 0f, 0f, 200, default(Color), 1f);
                    Dust dust = Main.dust[num299];
                    dust.velocity *= 0.3f;
                    Main.dust[num299].noGravity = true;
                }

                float num300 = 6f;
                Vector2 vector34 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num301 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector34.X;
                float num302 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector34.Y;
                float num303 = (float)Math.Sqrt((double)(num301 * num301 + num302 * num302));

                num303 = num300 / num303;
                num301 *= num303;
                num302 *= num303;

                if (num301 > 0f)
                {
                    npc.direction = 1;
                }
                else
                {
                    npc.direction = -1;
                }

                npc.spriteDirection = npc.direction;

                if (npc.direction < 0)
                {
                    npc.rotation = (float)Math.Atan2((double)(-(double)num302), (double)(-(double)num301));
                }
                else
                {
                    npc.rotation = (float)Math.Atan2((double)num302, (double)num301);
                }

                if (npc.justHit)
                {
                    npc.localAI[1] = 0f;
                    npc.ai[3] = 0f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] == 16f)
                {
                    int dmg = 45;
                    int projType = ProjectileID.FrostBlastHostile;
                    Projectile.NewProjectile(npc.GetSource_FromAI(), vector34.X, vector34.Y, num301, num302, projType, dmg, 0f, Main.myPlayer, 0f, 0f);
                }

                num290 = 15;

                if (npc.ai[3] > 0f)
                {
                    npc.ai[3] += 1f;
                    if (npc.ai[3] >= 64f)
                    {
                        npc.ai[3] = 0f;
                    }
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] == 0f)
                {
                    npc.localAI[1] += 1f;
                    if (npc.localAI[1] > (CalamityWorld.death ? 60f : 120f) && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.localAI[1] = 0f;
                        npc.ai[3] = 1f;
                        npc.netUpdate = true;
                    }
                }
            }
            else if (npc.type == NPCID.IchorSticker)
            {
                npc.rotation = npc.velocity.X * 0.1f;

                if (Main.player[npc.target].Center.Y < npc.Center.Y)
                {
                    num290 = 18;
                }
                else
                {
                    num290 = 9;
                }

                if (npc.justHit)
                    npc.ai[3] = 0f;

                if (Main.netMode != NetmodeID.MultiplayerClient && !npc.confused)
                {
                    npc.ai[3] += 1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[3] >= (CalamityWorld.death ? 60f : 120f))
                    {
                        npc.ai[3] = 0f;
                        if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].head))
                        {
                            float num307 = 10f;
                            Vector2 vector35 = new Vector2(npc.position.X + (float)npc.width * 0.5f - 4f, npc.position.Y + (float)npc.height * 0.7f);
                            float num308 = Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) - vector35.X;
                            float num309 = Math.Abs(num308) * 0.1f;
                            float num310 = Main.player[npc.target].position.Y + (float)(Main.player[npc.target].height / 2) - vector35.Y - num309;
                            float num311 = (float)Math.Sqrt((double)(num308 * num308 + num310 * num310));
                            num311 = num307 / num311;
                            num308 *= num311;
                            num310 *= num311;
                            int dmg = 40;
                            int projType = ProjectileID.GoldenShowerHostile;
                            Projectile.NewProjectile(npc.GetSource_FromAI(), vector35.X, vector35.Y, num308, num310, projType, dmg, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }

            if (npc.type == NPCID.Drippler)
            {
                num290 = 8;

                if (npc.target >= 0)
                {
                    float num315 = (Main.player[npc.target].Center - npc.Center).Length();
                    num315 /= 70f;
                    if (num315 > 8f)
                    {
                        num315 = 8f;
                    }
                    num290 += (int)num315;
                }
            }

            for (int num316 = num289; num316 < num289 + num290; num316++)
            {
                if ((Main.tile[num288, num316].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num288, num316].TileType]) || Main.tile[num288, num316].LiquidAmount > 0)
                {
                    if (num316 <= num289 + 1)
                    {
                        flag24 = true;
                    }
                    flag23 = false;
                    break;
                }
            }

            if (Main.player[npc.target].npcTypeNoAggro[npc.type])
            {
                bool flag25 = false;
                for (int num317 = num289; num317 < num289 + num290 - 2; num317++)
                {
                    if ((Main.tile[num288, num317].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num288, num317].TileType]) || Main.tile[num288, num317].LiquidAmount > 0)
                    {
                        flag25 = true;
                        break;
                    }
                }
                npc.directionY = (!flag25).ToDirectionInt();
            }

            if (npc.type == NPCID.IceElemental || npc.type == NPCID.IchorSticker)
            {
                for (int num318 = num289 - 3; num318 < num289; num318++)
                {
                    if ((Main.tile[num288, num318].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num288, num318].TileType]) || Main.tile[num288, num318].LiquidAmount > 0)
                    {
                        flag24 = false;
                        flag19 = true;
                        break;
                    }
                }
            }

            if (flag19)
            {
                flag23 = true;
                if (npc.type == NPCID.IchorSticker)
                {
                    npc.velocity.Y = npc.velocity.Y + 3f;
                }
            }

            if (flag23)
            {
                if (npc.type == NPCID.Pixie || npc.type == NPCID.IceElemental)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.3f;
                    if (npc.velocity.Y > 3f)
                    {
                        npc.velocity.Y = 3f;
                    }
                }
                else if (npc.type == NPCID.Drippler)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.05f;
                    if (npc.velocity.Y > 1f)
                    {
                        npc.velocity.Y = 1f;
                    }
                }
                else
                {
                    npc.velocity.Y = npc.velocity.Y + 0.15f;
                    if (npc.velocity.Y > 4f)
                    {
                        npc.velocity.Y = 4f;
                    }
                }
            }
            else
            {
                if (npc.type == NPCID.Pixie || npc.type == NPCID.IceElemental)
                {
                    if ((npc.directionY < 0 && npc.velocity.Y > 0f) | flag24)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.3f;
                    }
                }
                else if (npc.type == NPCID.Drippler)
                {
                    if ((npc.directionY < 0 && npc.velocity.Y > 0f) | flag24)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.1f;
                    }
                    if (npc.velocity.Y < -1f)
                    {
                        npc.velocity.Y = -1f;
                    }
                }
                else if (npc.directionY < 0 && npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.15f;
                }

                if (npc.velocity.Y < -5.5f)
                {
                    npc.velocity.Y = -5.5f;
                }
            }

            if (npc.type == NPCID.Pixie && npc.wet)
            {
                npc.velocity.Y = npc.velocity.Y - 0.3f;
                if (npc.velocity.Y < -3f)
                {
                    npc.velocity.Y = -3f;
                }
            }

            if (npc.collideX)
            {
                npc.velocity.X = npc.oldVelocity.X * -0.4f;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 1f)
                {
                    npc.velocity.X = 1f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -1f)
                {
                    npc.velocity.X = -1f;
                }
            }
            if (npc.collideY)
            {
                npc.velocity.Y = npc.oldVelocity.Y * -0.25f;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                {
                    npc.velocity.Y = 1f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                {
                    npc.velocity.Y = -1f;
                }
            }

            float num319 = 4f;
            if (npc.type == NPCID.Pixie)
            {
                num319 = 5f;
            }
            if (npc.type == NPCID.Reaper)
            {
                num319 = 6f;
            }
            if (npc.type == NPCID.Drippler)
            {
                num319 = 2.5f;
            }
            if (CalamityWorld.death)
            {
                num319 *= 1.25f;
            }

            if (npc.type == NPCID.Poltergeist)
            {
                npc.alpha = 0;
                num319 = 6f;
                if (!flag20)
                {
                    npc.TargetClosest(true);
                }
                else if (npc.timeLeft > 10)
                {
                    npc.timeLeft = 10;
                }
                if (npc.direction < 0 && npc.velocity.X > 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
                if (npc.direction > 0 && npc.velocity.X < 0f)
                {
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
            }

            if (npc.direction == -1 && npc.velocity.X > -num319)
            {
                npc.velocity.X = npc.velocity.X - 0.15f;
                if (npc.velocity.X > num319)
                {
                    npc.velocity.X = npc.velocity.X - 0.15f;
                }
                else if (npc.velocity.X > 0f)
                {
                    npc.velocity.X = npc.velocity.X + 0.1f;
                }
                if (npc.velocity.X < -num319)
                {
                    npc.velocity.X = -num319;
                }
            }
            else if (npc.direction == 1 && npc.velocity.X < num319)
            {
                npc.velocity.X = npc.velocity.X + 0.15f;
                if (npc.velocity.X < -num319)
                {
                    npc.velocity.X = npc.velocity.X + 0.15f;
                }
                else if (npc.velocity.X < 0f)
                {
                    npc.velocity.X = npc.velocity.X - 0.1f;
                }
                if (npc.velocity.X > num319)
                {
                    npc.velocity.X = num319;
                }
            }

            if (npc.type == NPCID.Drippler)
            {
                num319 = 1.5f;
            }
            else
            {
                num319 = 2.5f;
            }
            if (CalamityWorld.death)
            {
                num319 *= 1.25f;
            }

            if (npc.directionY == -1 && npc.velocity.Y > -num319)
            {
                npc.velocity.Y = npc.velocity.Y - 0.06f;
                if (npc.velocity.Y > num319)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.1f;
                }
                else if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.05f;
                }
                if (npc.velocity.Y < -num319)
                {
                    npc.velocity.Y = -num319;
                }
            }
            else if (npc.directionY == 1 && npc.velocity.Y < num319)
            {
                npc.velocity.Y = npc.velocity.Y + 0.06f;
                if (npc.velocity.Y < -num319)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.1f;
                }
                else if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.05f;
                }
                if (npc.velocity.Y > num319)
                {
                    npc.velocity.Y = num319;
                }
            }

            if (npc.type == NPCID.Gastropod)
            {
                Lighting.AddLight((int)npc.position.X / 16, (int)npc.position.Y / 16, 0.4f, 0f, 0.25f);
            }

            return false;
        }
        #endregion

        #region Flying Weapon AI
        public static bool BuffedFlyingWeaponAI(NPC npc, Mod mod)
        {
            npc.knockBackResist = 0f;
            npc.noGravity = true;
            npc.noTileCollide = true;
            if (npc.type == NPCID.EnchantedSword)
            {
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.2f, 0.05f, 0.3f);
            }
            else if (npc.type == NPCID.CrimsonAxe)
            {
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.3f, 0.15f, 0.05f);
            }
            else
            {
                Lighting.AddLight((int)(npc.Center.X / 16f), (int)(npc.Center.Y / 16f), 0.05f, 0.2f, 0.3f);
            }
            // Adjust target if we don't have one.
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }
            // Charge
            if (npc.ai[0] == 0f)
            {
                float chargeSpeed = CalamityWorld.death ? 16f : 12f;
                npc.velocity = npc.SafeDirectionTo(Main.player[npc.target].Center, -Vector2.UnitY) * chargeSpeed;
                npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;

                // Slow down
                npc.ai[0] = 1f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
                return false;
            }
            // Slow down
            if (npc.ai[0] == 1f)
            {
                npc.velocity *= CalamityWorld.death ? 0.98f : 0.99f;
                npc.ai[1] += 1f;
                // Get ready to spin and then charge again.
                if (npc.ai[1] >= (CalamityWorld.death ? 50f : 100f))
                {
                    npc.netUpdate = true;
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.velocity = Vector2.Zero;
                }
            }
            // Spin
            else
            {
                npc.velocity *= CalamityWorld.death ? 0.94f : 0.96f;
                npc.ai[1] += 1f;

                float anglularSpeed = npc.ai[1] / (CalamityWorld.death ? 90f : 150f);
                anglularSpeed = 0.1f + anglularSpeed * 0.4f;
                npc.rotation += anglularSpeed * npc.direction;

                // Charge
                if (npc.ai[1] >= (CalamityWorld.death ? 90f : 150f))
                {
                    npc.netUpdate = true;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                }
            }
            return false;
        }
        #endregion

        #region Mimic AI
        public static bool BuffedMimicAI(NPC npc, Mod mod)
        {
            bool isLostHoppingPresent = npc.type == NPCID.PresentMimic && !Main.snowMoon;

            if (npc.ai[3] == 0f)
            {
                npc.position.X += 8f;
                if (npc.position.Y / 16f > Main.maxTilesY - 200f)
                {
                    npc.ai[3] = 3f;
                }
                else if (npc.position.Y / 16f > Main.worldSurface)
                {
                    npc.TargetClosest(true);
                    npc.ai[3] = 2f;
                }
                else
                {
                    npc.ai[3] = 1f;
                }
            }

            // Never wait. Go straight for the player.
            if (npc.type == NPCID.PresentMimic || npc.type == NPCID.IceMimic)
            {
                npc.ai[3] = 1f;
            }

            npc.dontTakeDamage = npc.ai[0] == 0f;

            // Sitting around, waiting for a potential player
            if (npc.ai[0] == 0f)
            {
                if (!isLostHoppingPresent)
                {
                    npc.TargetClosest(true);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.velocity.X != 0f || npc.velocity.Y < 0f || npc.velocity.Y > 0.3f)
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                        return false;
                    }

                    Rectangle detectionZone = new Rectangle((int)npc.position.X - 80, (int)npc.position.Y - 80, npc.width + 160, npc.height + 160);
                    // If a player is nearby, become active.
                    if (detectionZone.Intersects(Main.player[npc.target].Hitbox) || npc.life < npc.lifeMax)
                    {
                        npc.ai[0] = 1f;
                        npc.netUpdate = true;
                    }
                }
            }
            else if (npc.velocity.Y == 0f)
            {
                npc.ai[2] += 1f;

                int timeSpentStopping = 20;
                if (npc.ai[1] == 0f)
                {
                    timeSpentStopping = 12;
                }
                if (npc.ai[2] < timeSpentStopping)
                {
                    npc.velocity.X *= 0.9f;
                    return false;
                }

                npc.ai[2] = 0f;

                if (!isLostHoppingPresent)
                {
                    npc.TargetClosest(true);
                }
                if (npc.direction == 0)
                {
                    npc.direction = -1;
                }

                npc.spriteDirection = npc.direction;

                npc.ai[1] += 1f;
                // Hop
                if (npc.ai[1] == 2f)
                {
                    npc.velocity.X = npc.direction * 4f;
                    npc.velocity.Y = -8f;
                    npc.ai[1] = 0f;
                }
                else
                {
                    npc.velocity.X = npc.direction * 5.5f;
                    npc.velocity.Y = -4f;
                }

                npc.netUpdate = true;
            }
            else
            {
                if (npc.direction == 1 && npc.velocity.X < 1f)
                {
                    npc.velocity.X += 0.1f;
                    return false;
                }

                if (npc.direction == -1 && npc.velocity.X > -1f)
                {
                    npc.velocity.X -= 0.1f;
                }
            }
            return false;
        }
        #endregion

        #region Unicorn AI
        public static bool BuffedUnicornAI(NPC npc, Mod mod)
        {
            int num = 30;
            int num2 = 8;

            bool flag = false;
            bool flag2 = false;
            bool flag3 = false;

            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
            {
                flag2 = true;
                npc.ai[3] += 1f;
            }

            if (npc.type == NPCID.Tumbleweed)
            {
                num2 = 3;
                bool flag4 = npc.velocity.Y == 0f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (i != npc.whoAmI && Main.npc[i].active && Main.npc[i].type == npc.type && Math.Abs(npc.position.X - Main.npc[i].position.X) + Math.Abs(npc.position.Y - Main.npc[i].position.Y) < (float)npc.width)
                    {
                        if (npc.position.X < Main.npc[i].position.X)
                        {
                            npc.velocity.X -= 0.05f;
                        }
                        else
                        {
                            npc.velocity.X += 0.05f;
                        }
                        if (npc.position.Y < Main.npc[i].position.Y)
                        {
                            npc.velocity.Y -= 0.05f;
                        }
                        else
                        {
                            npc.velocity.Y += 0.05f;
                        }
                    }
                }
                if (flag4)
                {
                    npc.velocity.Y = 0f;
                }
            }

            if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num) | flag2)
            {
                npc.ai[3] += 1f;
                flag3 = true;
            }
            else if (npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }

            if (npc.ai[3] > (float)(num * num2))
            {
                npc.ai[3] = 0f;
            }

            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }

            if (npc.ai[3] == (float)num)
            {
                npc.netUpdate = true;
            }

            Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
            float arg_31B_0 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector.X;
            float num3 = Main.player[npc.target].position.Y - vector.Y;
            float num4 = (float)Math.Sqrt((double)(arg_31B_0 * arg_31B_0 + num3 * num3));

            if (num4 < 200f && !flag3)
            {
                npc.ai[3] = 0f;
            }

            if (npc.type == NPCID.StardustSpiderSmall)
            {
                npc.ai[1] += 1f;
                bool flag5 = npc.ai[1] >= (CalamityWorld.death ? 60f : 120f);
                if (!flag5 && npc.velocity.Y == 0f)
                {
                    for (int j = 0; j < Main.maxPlayers; j++)
                    {
                        if (Main.player[j].active && !Main.player[j].dead && Main.player[j].Distance(npc.Center) < 800f && Main.player[j].Center.Y < npc.Center.Y && Math.Abs(Main.player[j].Center.X - npc.Center.X) < 20f)
                        {
                            flag5 = true;
                            break;
                        }
                    }
                }

                if (flag5 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    for (int k = 0; k < 3; k++)
                    {
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X, npc.Center.Y, (Main.rand.NextFloat() - 0.5f) * 2f, -4f - 10f * Main.rand.NextFloat(), ProjectileID.Twinkle, 50, 0f, Main.myPlayer, 0f, 0f);
                    }
                    npc.HitEffect(9999, 10.0);
                    npc.active = false;
                    return false;
                }
            }
            else if (npc.type == NPCID.NebulaBeast)
            {
                if (npc.ai[2] == 1f)
                {
                    npc.ai[1] += 1f;
                    npc.velocity.X = npc.velocity.X * 0.7f;
                    if (npc.ai[1] < 30f)
                    {
                        Vector2 vector2 = npc.Center + Vector2.UnitX * (float)npc.spriteDirection * -20f;
                        Dust arg_5A4_0 = Main.dust[Dust.NewDust(vector2, 0, 0, 242, 0f, 0f, 0, default(Color), 1f)];
                        Vector2 vector3 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                        arg_5A4_0.position = vector2 + vector3 * 20f;
                        arg_5A4_0.velocity = -vector3 * 2f;
                        arg_5A4_0.scale = 0.5f + vector3.X * (float)(-(float)npc.spriteDirection);
                        arg_5A4_0.fadeIn = 1f;
                        arg_5A4_0.noGravity = true;
                    }
                    else if (npc.ai[1] == 30f)
                    {
                        for (int l = 0; l < 20; l++)
                        {
                            Vector2 vector4 = npc.Center + Vector2.UnitX * (float)npc.spriteDirection * -20f;
                            Dust arg_68B_0 = Main.dust[Dust.NewDust(vector4, 0, 0, 242, 0f, 0f, 0, default(Color), 1f)];
                            Vector2 vector5 = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                            arg_68B_0.position = vector4 + vector5 * 4f;
                            arg_68B_0.velocity = vector5 * 4f + Vector2.UnitX * Main.rand.NextFloat() * (float)npc.spriteDirection * -5f;
                            arg_68B_0.scale = 0.5f + vector5.X * (float)(-(float)npc.spriteDirection);
                            arg_68B_0.fadeIn = 1f;
                            arg_68B_0.noGravity = true;
                        }
                    }

                    if (npc.velocity.X > -0.5f && npc.velocity.X < 0.5f)
                    {
                        npc.velocity.X = 0f;
                    }

                    if (npc.ai[1] == 30f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int num5 = Main.expertMode ? 35 : 50;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center.X + (float)(npc.spriteDirection * -20), npc.Center.Y, (float)(npc.spriteDirection * -7), 0f, ProjectileID.NebulaSphere, num5, 0f, Main.myPlayer, (float)npc.target, 0f);
                    }

                    if (npc.ai[1] >= 60f)
                    {
                        npc.ai[1] = (float)(-(float)Main.rand.Next(320, CalamityWorld.death ? 361 : 601));
                        npc.ai[2] = 0f;
                    }
                }
                else
                {
                    npc.ai[1] += 1f;
                    if (npc.ai[1] >= 180f && num4 < 500f && npc.velocity.Y == 0f)
                    {
                        flag = true;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 1f;
                        npc.netUpdate = true;
                    }
                    else if (npc.velocity.Y == 0f && num4 < 100f && Math.Abs(npc.velocity.X) > 3f && ((npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f) || (npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f)))
                    {
                        npc.velocity.Y = npc.velocity.Y - 6f;
                    }
                }
            }
            else if (npc.type == NPCID.Wolf || npc.type == NPCID.Hellhound || npc.type == ModContent.NPCType<Rotdog>())
            {
                if (npc.velocity.Y == 0f && num4 < 100f && Math.Abs(npc.velocity.X) > 3f && ((npc.position.X + (float)(npc.width / 2) < Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) && npc.velocity.X > 0f) || (npc.position.X + (float)(npc.width / 2) > Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2) && npc.velocity.X < 0f)))
                {
                    npc.velocity.Y -= 6f;
                }
            }
            else if (npc.type == NPCID.Tumbleweed && npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) > 3f && ((npc.Center.X < Main.player[npc.target].Center.X && npc.velocity.X > 0f) || (npc.Center.X > Main.player[npc.target].Center.X && npc.velocity.X < 0f)))
            {
                npc.velocity.Y -= 6f;
                SoundEngine.PlaySound(SoundID.NPCHit11, npc.Center);
            }

            if (npc.ai[3] < (float)num)
            {
                if ((npc.type == NPCID.Hellhound || npc.type == NPCID.HeadlessHorseman) && !Main.pumpkinMoon)
                {
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                }
                else
                {
                    npc.TargetClosest(true);
                }
            }
            else
            {
                if (npc.velocity.X == 0f)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.ai[0] += 1f;
                        if (npc.ai[0] >= 2f)
                        {
                            npc.direction *= -1;
                            npc.spriteDirection = npc.direction;
                            npc.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    npc.ai[0] = 0f;
                }
                npc.directionY = -1;
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }

            float num6 = 9f;
            float num7 = 0.1f;
            if (CalamityWorld.death)
            {
                num6 *= 1.25f;
                num7 *= 1.25f;
            }

            if (!flag && (npc.velocity.Y == 0f || npc.wet || (npc.velocity.X <= 0f && npc.direction < 0) || (npc.velocity.X >= 0f && npc.direction > 0)))
            {
                if (npc.type == NPCID.Wolf || npc.type == ModContent.NPCType<Rotdog>())
                {
                    if (npc.velocity.X > 0f && npc.direction < 0)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                    if (npc.velocity.X < 0f && npc.direction > 0)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                }
                else if (npc.type == NPCID.Hellhound)
                {
                    if (npc.velocity.X > 0f && npc.direction < 0)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                    if (npc.velocity.X < 0f && npc.direction > 0)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                    if (npc.direction > 0 && npc.velocity.X < 3f)
                    {
                        npc.velocity.X += 0.15f;
                    }
                    if (npc.direction < 0 && npc.velocity.X > -3f)
                    {
                        npc.velocity.X -= 0.15f;
                    }
                }
                else if (npc.type == NPCID.HeadlessHorseman)
                {
                    if (npc.velocity.X > 0f && npc.direction < 0)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                    if (npc.velocity.X < 0f && npc.direction > 0)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                    if (npc.velocity.X < -num6 || npc.velocity.X > num6)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity *= 0.8f;
                        }
                    }
                    else if (npc.velocity.X < num6 && npc.direction == 1)
                    {
                        npc.velocity.X += 0.1f;
                        if (npc.velocity.X > num6)
                        {
                            npc.velocity.X = num6;
                        }
                    }
                    else if (npc.velocity.X > -num6 && npc.direction == -1)
                    {
                        npc.velocity.X -= 0.1f;
                        if (npc.velocity.X < -num6)
                        {
                            npc.velocity.X = -num6;
                        }
                    }
                }
                else if (npc.type == NPCID.StardustSpiderSmall)
                {
                    if (Math.Sign(npc.velocity.X) != npc.direction)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                    num7 = 0.2f;
                }
                else if (npc.type == NPCID.NebulaBeast)
                {
                    if (Math.Sign(npc.velocity.X) != npc.direction)
                    {
                        npc.velocity.X *= 0.8f;
                    }
                    num6 = 12f;
                    num7 = 0.2f;
                }
                else if (npc.type == NPCID.Tumbleweed)
                {
                    if (Math.Sign(npc.velocity.X) != npc.direction)
                    {
                        npc.velocity.X *= 0.9f;
                    }
                    float num8 = MathHelper.Lerp(0.6f, 1f, Math.Abs(Main.windSpeedCurrent)) * (float)Math.Sign(Main.windSpeedCurrent);
                    if (!Main.player[npc.target].ZoneSandstorm)
                    {
                        num8 = 0f;
                    }
                    num6 = 6f + num8 * (float)npc.direction * 4f;
                    num7 = 0.2f;
                }
                if (CalamityWorld.death)
                {
                    num6 *= 1.25f;
                    num7 *= 1.25f;
                }
                if (npc.velocity.X < -num6 || npc.velocity.X > num6)
                {
                    if (npc.velocity.Y == 0f)
                    {
                        npc.velocity *= 0.8f;
                    }
                }
                else if (npc.velocity.X < num6 && npc.direction == 1)
                {
                    npc.velocity.X = npc.velocity.X + num7;
                    if (npc.velocity.X > num6)
                    {
                        npc.velocity.X = num6;
                    }
                }
                else if (npc.velocity.X > -num6 && npc.direction == -1)
                {
                    npc.velocity.X = npc.velocity.X - num7;
                    if (npc.velocity.X < -num6)
                    {
                        npc.velocity.X = -num6;
                    }
                }
            }

            if (npc.velocity.Y >= 0f)
            {
                int num9 = 0;
                if (npc.velocity.X < 0f)
                {
                    num9 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num9 = 1;
                }

                Vector2 position = npc.position;
                position.X += npc.velocity.X;
                int num10 = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num9)) / 16f);
                int num11 = (int)((position.Y + (float)npc.height - 1f) / 16f);

                if ((float)(num10 * 16) < position.X + (float)npc.width && (float)(num10 * 16 + 16) > position.X && ((Main.tile[num10, num11].HasUnactuatedTile && !Main.tile[num10, num11].TopSlope && !Main.tile[num10, num11 - 1].TopSlope && Main.tileSolid[(int)Main.tile[num10, num11].TileType] && !Main.tileSolidTop[(int)Main.tile[num10, num11].TileType]) || (Main.tile[num10, num11 - 1].IsHalfBlock && Main.tile[num10, num11 - 1].HasUnactuatedTile)) && (!Main.tile[num10, num11 - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num10, num11 - 1].TileType] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 1].TileType] || (Main.tile[num10, num11 - 1].IsHalfBlock && (!Main.tile[num10, num11 - 4].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num10, num11 - 4].TileType] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 4].TileType]))) && (!Main.tile[num10, num11 - 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num10, num11 - 2].TileType] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 2].TileType]) && (!Main.tile[num10, num11 - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num10, num11 - 3].TileType] || Main.tileSolidTop[(int)Main.tile[num10, num11 - 3].TileType]) && (!Main.tile[num10 - num9, num11 - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num10 - num9, num11 - 3].TileType]))
                {
                    float num12 = (float)(num11 * 16);
                    if (Main.tile[num10, num11].IsHalfBlock)
                    {
                        num12 += 8f;
                    }
                    if (Main.tile[num10, num11 - 1].IsHalfBlock)
                    {
                        num12 -= 8f;
                    }
                    if (num12 < position.Y + (float)npc.height)
                    {
                        float num13 = position.Y + (float)npc.height - num12;
                        if ((double)num13 <= 16.1)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num12;
                            npc.position.Y = num12 - (float)npc.height;
                            if (num13 < 9f)
                            {
                                npc.stepSpeed = 1f;
                            }
                            else
                            {
                                npc.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }

            if (npc.velocity.Y == 0f)
            {
                int num14 = (int)((npc.position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 2) * npc.direction) + npc.velocity.X * 5f) / 16f);
                int num15 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);

                int num16 = npc.spriteDirection;
                if (npc.type == NPCID.NebulaBeast || npc.type == NPCID.StardustSpiderSmall || npc.type == NPCID.Tumbleweed)
                {
                    num16 *= -1;
                }

                if ((npc.velocity.X < 0f && num16 == -1) || (npc.velocity.X > 0f && num16 == 1))
                {
                    bool flag6 = npc.type == NPCID.StardustSpiderSmall || npc.type == NPCID.NebulaBeast;
                    float num17 = 3f;

                    if (Main.tile[num14, num15 - 2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num14, num15 - 2].TileType])
                    {
                        if (Main.tile[num14, num15 - 3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num14, num15 - 3].TileType])
                        {
                            npc.velocity.Y = -10.5f;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            npc.velocity.Y = -9.5f;
                            npc.netUpdate = true;
                        }
                    }
                    else if (Main.tile[num14, num15 - 1].HasUnactuatedTile && !Main.tile[num14, num15 - 1].TopSlope && Main.tileSolid[(int)Main.tile[num14, num15 - 1].TileType])
                    {
                        npc.velocity.Y = -9f;
                        npc.netUpdate = true;
                    }
                    else if (npc.position.Y + (float)npc.height - (float)(num15 * 16) > 20f && Main.tile[num14, num15].HasUnactuatedTile && !Main.tile[num14, num15].TopSlope && Main.tileSolid[(int)Main.tile[num14, num15].TileType])
                    {
                        npc.velocity.Y = -7f;
                        npc.netUpdate = true;
                    }
                    else if ((npc.directionY < 0 || Math.Abs(npc.velocity.X) > num17) && (!flag6 || !Main.tile[num14, num15 + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num14, num15 + 1].TileType]) && (!Main.tile[num14, num15 + 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num14, num15 + 2].TileType]) && (!Main.tile[num14 + npc.direction, num15 + 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num14 + npc.direction, num15 + 3].TileType]))
                    {
                        npc.velocity.Y = -10f;
                        npc.netUpdate = true;
                    }
                }
            }

            if (npc.type == NPCID.NebulaBeast && Math.Abs(npc.velocity.X) >= num6 * 0.95f)
            {
                Rectangle hitbox = npc.Hitbox;
                for (int m = 0; m < 2; m++)
                {
                    if (Main.rand.NextBool(3))
                    {
                        Dust expr_1B5F = Main.dust[Dust.NewDust(hitbox.TopLeft(), hitbox.Width, hitbox.Height, 242, 0f, 0f, 0, default(Color), 1f)];
                        expr_1B5F.velocity = Vector2.Zero;
                        expr_1B5F.noGravity = true;
                        expr_1B5F.fadeIn = 1f;
                        expr_1B5F.scale = 0.5f + Main.rand.NextFloat();
                    }
                }
            }

            if (npc.type == NPCID.Tumbleweed)
            {
                npc.rotation += npc.velocity.X * 0.05f;
                npc.spriteDirection = -npc.direction;
            }

            return false;
        }
        #endregion

        #region Tortoise AI
        public static bool BuffedTortoiseAI(NPC npc, Mod mod)
        {
            if (npc.target < 0 || Main.player[npc.target].dead || npc.direction == 0)
            {
                npc.TargetClosest(true);
            }

            bool flag31 = true;

            int num532 = 0;
            if (npc.velocity.X < 0f)
            {
                num532 = -1;
            }
            if (npc.velocity.X > 0f)
            {
                num532 = 1;
            }

            Vector2 position = npc.position;
            position.X += npc.velocity.X;
            int num533 = (int)((position.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num532)) / 16f);
            int num534 = (int)((position.Y + (float)npc.height - 1f) / 16f);

            if ((float)(num533 * 16) < position.X + (float)npc.width && (float)(num533 * 16 + 16) > position.X && ((Main.tile[num533, num534].HasUnactuatedTile && !Main.tile[num533, num534].TopSlope && !Main.tile[num533, num534 - 1].TopSlope && ((Main.tileSolid[(int)Main.tile[num533, num534].TileType] && !Main.tileSolidTop[(int)Main.tile[num533, num534].TileType]) || (flag31 && Main.tileSolidTop[(int)Main.tile[num533, num534].TileType] && (!Main.tileSolid[(int)Main.tile[num533, num534 - 1].TileType] || !Main.tile[num533, num534 - 1].HasUnactuatedTile) && Main.tile[num533, num534].TileType != 16 && Main.tile[num533, num534].TileType != 18 && Main.tile[num533, num534].TileType != 134))) || (Main.tile[num533, num534 - 1].IsHalfBlock && Main.tile[num533, num534 - 1].HasUnactuatedTile)) && (!Main.tile[num533, num534 - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num533, num534 - 1].TileType] || Main.tileSolidTop[(int)Main.tile[num533, num534 - 1].TileType] || (Main.tile[num533, num534 - 1].IsHalfBlock && (!Main.tile[num533, num534 - 4].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num533, num534 - 4].TileType] || Main.tileSolidTop[(int)Main.tile[num533, num534 - 4].TileType]))) && (!Main.tile[num533, num534 - 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num533, num534 - 2].TileType] || Main.tileSolidTop[(int)Main.tile[num533, num534 - 2].TileType]) && (!Main.tile[num533, num534 - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num533, num534 - 3].TileType] || Main.tileSolidTop[(int)Main.tile[num533, num534 - 3].TileType]) && (!Main.tile[num533 - num532, num534 - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num533 - num532, num534 - 3].TileType] || Main.tileSolidTop[(int)Main.tile[num533 - num532, num534 - 3].TileType]))
            {
                float num535 = (float)(num534 * 16);
                if (Main.tile[num533, num534].IsHalfBlock)
                {
                    num535 += 8f;
                }
                if (Main.tile[num533, num534 - 1].IsHalfBlock)
                {
                    num535 -= 8f;
                }

                if (num535 < position.Y + (float)npc.height)
                {
                    float num536 = position.Y + (float)npc.height - num535;
                    if ((double)num536 <= 16.1)
                    {
                        npc.gfxOffY += npc.position.Y + (float)npc.height - num535;
                        npc.position.Y = num535 - (float)npc.height;
                        if (num536 < 9f)
                        {
                            npc.stepSpeed = 0.75f;
                        }
                        else
                        {
                            npc.stepSpeed = 1.5f;
                        }
                    }
                }
            }

            if (npc.type == NPCID.IceTortoise && Main.rand.NextBool(10))
            {
                int num537 = Dust.NewDust(npc.position, npc.width, npc.height, 67, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                Main.dust[num537].noGravity = true;
                Dust dust = Main.dust[num537];
                dust.velocity *= 0.2f;
            }

            if (npc.ai[0] == 0f)
            {
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else if (npc.velocity.X > 0f)
                {
                    npc.direction = 1;
                }

                npc.spriteDirection = npc.direction;

                Vector2 vector66 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num538 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector66.X;
                float num539 = Main.player[npc.target].position.Y - vector66.Y;
                float num540 = (float)Math.Sqrt((double)(num538 * num538 + num539 * num539));

                bool flag32 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                {
                    if (num540 > 200f & flag32)
                    {
                        npc.ai[1] += 3f;
                    }
                    if (num540 > 600f && (flag32 || npc.position.Y + (float)npc.height > Main.player[npc.target].position.Y - 200f))
                    {
                        npc.ai[1] += 6f;
                    }
                }
                else
                {
                    if (num540 > 200f & flag32)
                    {
                        npc.ai[1] += 6f;
                    }
                    if (num540 > 600f && (flag32 || npc.position.Y + (float)npc.height > Main.player[npc.target].position.Y - 200f))
                    {
                        npc.ai[1] += 15f;
                    }
                }

                if (npc.wet)
                {
                    npc.ai[1] = 1000f;
                }

                npc.defense = npc.defDefense;
                npc.damage = npc.defDamage;

                if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                {
                    npc.knockBackResist = 0.5f;
                }
                else
                {
                    npc.knockBackResist = 0.15f;
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= (CalamityWorld.death ? 400f : 500f))
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 1f;
                }

                if (!npc.justHit && npc.velocity.X != npc.oldVelocity.X)
                {
                    npc.direction *= -1;
                }

                if (npc.velocity.Y == 0f && Main.player[npc.target].position.Y < npc.position.Y + (float)npc.height)
                {
                    int num541;
                    int num542;
                    if (npc.direction > 0)
                    {
                        num541 = (int)(((double)npc.position.X + (double)npc.width * 0.5) / 16.0);
                        num542 = num541 + 3;
                    }
                    else
                    {
                        num542 = (int)(((double)npc.position.X + (double)npc.width * 0.5) / 16.0);
                        num541 = num542 - 3;
                    }

                    int num543 = (int)((npc.position.Y + (float)npc.height + 2f) / 16f) - 1;
                    int num544 = num543 + 4;
                    bool flag33 = false;
                    for (int num545 = num541; num545 <= num542; num545++)
                    {
                        for (int num546 = num543; num546 <= num544; num546++)
                        {
                            if (Main.tile[num545, num546] != null && Main.tile[num545, num546].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num545, num546].TileType])
                            {
                                flag33 = true;
                            }
                        }
                    }

                    if (!flag33)
                    {
                        npc.direction *= -1;
                        npc.velocity.X = 0.1f * (float)npc.direction;
                    }
                }

                if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                {
                    float num547 = 1f;
                    if (npc.velocity.X < -num547 || npc.velocity.X > num547)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity *= 0.8f;
                        }
                    }
                    else if (npc.velocity.X < num547 && npc.direction == 1)
                    {
                        npc.velocity.X = npc.velocity.X + 0.1f;
                        if (npc.velocity.X > num547)
                        {
                            npc.velocity.X = num547;
                        }
                    }
                    else if (npc.velocity.X > -num547 && npc.direction == -1)
                    {
                        npc.velocity.X = npc.velocity.X - 0.1f;
                        if (npc.velocity.X < -num547)
                        {
                            npc.velocity.X = -num547;
                        }
                    }
                }
                else
                {
                    float num548 = 2f;
                    if (num540 < 400f)
                    {
                        if (npc.velocity.X < -num548 || npc.velocity.X > num548)
                        {
                            if (npc.velocity.Y == 0f)
                            {
                                npc.velocity *= 0.8f;
                            }
                        }
                        else if (npc.velocity.X < num548 && npc.direction == 1)
                        {
                            npc.velocity.X = npc.velocity.X + 0.1f;
                            if (npc.velocity.X > num548)
                            {
                                npc.velocity.X = num548;
                            }
                        }
                        else if (npc.velocity.X > -num548 && npc.direction == -1)
                        {
                            npc.velocity.X = npc.velocity.X - 0.1f;
                            if (npc.velocity.X < -num548)
                            {
                                npc.velocity.X = -num548;
                            }
                        }
                    }
                    else if (npc.velocity.X < -3f || npc.velocity.X > 3f)
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            npc.velocity *= 0.8f;
                        }
                    }
                    else if (npc.velocity.X < 3f && npc.direction == 1)
                    {
                        npc.velocity.X = npc.velocity.X + 0.1f;
                        if (npc.velocity.X > 3f)
                        {
                            npc.velocity.X = 3f;
                        }
                    }
                    else if (npc.velocity.X > -3f && npc.direction == -1)
                    {
                        npc.velocity.X = npc.velocity.X - 0.1f;
                        if (npc.velocity.X < -3f)
                        {
                            npc.velocity.X = -3f;
                        }
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.velocity.X = npc.velocity.X * 0.5f;

                if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                {
                    npc.ai[1] += 1f;
                }
                else
                {
                    npc.ai[1] += 2f;
                }

                if (npc.ai[1] >= 30f)
                {
                    npc.netUpdate = true;
                    npc.TargetClosest(true);
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[0] = 3f;

                    if (npc.type == NPCID.SolarSroller)
                    {
                        npc.ai[0] = 6f;
                        npc.ai[2] = (float)Main.rand.Next(2, 5);
                    }
                }
            }
            else
            {
                if (npc.ai[0] == 3f)
                {
                    if (npc.type == NPCID.IceTortoise && Main.rand.Next(3) < 2)
                    {
                        int num549 = Dust.NewDust(npc.position, npc.width, npc.height, 67, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                        Main.dust[num549].noGravity = true;
                        Dust dust = Main.dust[num549];
                        dust.velocity *= 0.2f;
                    }

                    if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                    {
                        npc.damage = (int)(npc.defDamage * 1.35);
                    }
                    else
                    {
                        npc.damage = (int)(npc.defDamage * 1.8);
                    }

                    npc.defense = npc.defDefense * 2;

                    npc.ai[1] += 1f;
                    if (npc.ai[1] == 1f)
                    {
                        npc.netUpdate = true;
                        npc.TargetClosest(true);

                        npc.ai[2] += 0.3f;
                        npc.rotation += npc.ai[2] * (float)npc.direction;
                        npc.ai[1] += 1f;

                        bool flag34 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                        float num550 = 15f;
                        if (!flag34)
                        {
                            num550 = 6f;
                        }
                        if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                        {
                            num550 *= 0.75f;
                        }

                        Vector2 vector67 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float num551 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector67.X;
                        float num552 = Math.Abs(num551) * 0.2f;
                        if (npc.directionY > 0)
                        {
                            num552 = 0f;
                        }
                        float num553 = Main.player[npc.target].position.Y - vector67.Y - num552;
                        float num554 = (float)Math.Sqrt((double)(num551 * num551 + num553 * num553));
                        npc.netUpdate = true;

                        num554 = num550 / num554;
                        num551 *= num554;
                        num553 *= num554;

                        if (!flag34)
                        {
                            num553 = -10f;
                        }

                        npc.velocity.X = num551;
                        npc.velocity.Y = num553;
                        npc.ai[3] = npc.velocity.X;
                    }
                    else
                    {
                        if (npc.position.X + (float)npc.width > Main.player[npc.target].position.X && npc.position.X < Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height)
                        {
                            npc.velocity.X = npc.velocity.X * 0.8f;
                            npc.ai[3] = 0f;
                            if (npc.velocity.Y < 0f)
                            {
                                npc.velocity.Y = npc.velocity.Y + 0.3f;
                            }
                        }

                        if (npc.ai[3] != 0f)
                        {
                            npc.velocity.X = npc.ai[3];
                            npc.velocity.Y = npc.velocity.Y - 0.33f;
                        }

                        if (npc.ai[1] >= 90f)
                        {
                            npc.noGravity = false;
                            npc.ai[1] = 0f;
                            npc.ai[0] = 4f;
                        }
                    }

                    if (npc.wet && npc.directionY < 0)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.45f;
                    }

                    npc.rotation += npc.ai[2] * (float)npc.direction;

                    return false;
                }

                if (npc.ai[0] == 4f)
                {
                    if (npc.wet && npc.directionY < 0)
                    {
                        npc.velocity.Y = npc.velocity.Y - 0.45f;
                    }

                    npc.velocity.X = npc.velocity.X * 0.95f;

                    if (npc.ai[2] > 0f)
                    {
                        npc.ai[2] -= 0.01f;
                        npc.rotation += npc.ai[2] * (float)npc.direction;
                    }
                    else if (npc.velocity.Y >= 0f)
                    {
                        npc.rotation = 0f;
                    }

                    if (npc.ai[2] <= 0f && (npc.velocity.Y == 0f || npc.wet))
                    {
                        npc.netUpdate = true;
                        npc.rotation = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[0] = 5f;
                    }
                }
                else
                {
                    if (npc.ai[0] == 6f)
                    {
                        npc.damage = (int)(npc.defDamage * 1.4);
                        npc.defense = npc.defDefense * 2;
                        npc.knockBackResist = 0f;

                        if (Main.rand.Next(3) < 2)
                        {
                            int num555 = Dust.NewDust(npc.Center - new Vector2(30f), 60, 60, 6, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default(Color), 1.5f);
                            Dust dust = Main.dust[num555];
                            dust.noGravity = true;
                            dust.velocity *= 0.2f;
                            dust.fadeIn = 1f;
                        }

                        npc.ai[1] += 1f;
                        if (npc.ai[3] > 0f)
                        {
                            if (npc.ai[3] == 1f)
                            {
                                Vector2 vector68 = npc.Center - new Vector2(50f);
                                for (int num556 = 0; num556 < 32; num556++)
                                {
                                    int num557 = Dust.NewDust(vector68, 100, 100, 6, 0f, 0f, 100, default(Color), 2.5f);
                                    Dust dust = Main.dust[num557];
                                    dust.noGravity = true;
                                    dust.velocity *= 3f;
                                    num557 = Dust.NewDust(vector68, 100, 100, 6, 0f, 0f, 100, default(Color), 1.5f);
                                    dust.velocity *= 2f;
                                    dust.noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.Server)
                                {
                                    for (int num558 = 0; num558 < 4; num558++)
                                    {
                                        int num559 = Gore.NewGore(npc.GetSource_FromAI(), vector68 + new Vector2((float)(50 * Main.rand.Next(100)) / 100f, (float)(50 * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), 1f);
                                        Gore gore = Main.gore[num559];
                                        gore.velocity *= 0.3f;
                                        gore.velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                                        gore.velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
                                    }
                                }
                            }

                            for (int num560 = 0; num560 < 5; num560++)
                            {
                                int num561 = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default(Color), 1.5f);
                                Main.dust[num561].velocity *= Main.rand.NextFloat();
                            }

                            npc.ai[3] += 1f;
                            if (npc.ai[3] >= 10f)
                            {
                                npc.ai[3] = 0f;
                            }
                        }

                        if (npc.ai[1] == 1f)
                        {
                            npc.netUpdate = true;
                            npc.TargetClosest(true);

                            bool flag35 = Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height);
                            float num562 = 24f;
                            if (!flag35)
                            {
                                num562 = 10f;
                            }

                            Vector2 vector69 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                            float num563 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector69.X;
                            float num564 = Math.Abs(num563) * 0.2f;
                            if (npc.directionY > 0)
                            {
                                num564 = 0f;
                            }
                            float num565 = Main.player[npc.target].position.Y - vector69.Y - num564;
                            float num566 = (float)Math.Sqrt((double)(num563 * num563 + num565 * num565));
                            npc.netUpdate = true;

                            num566 = num562 / num566;
                            num563 *= num566;
                            num565 *= num566;

                            if (!flag35)
                            {
                                num565 = -12f;
                            }

                            npc.velocity.X = num563;
                            npc.velocity.Y = num565;
                        }
                        else
                        {
                            if (npc.position.X + (float)npc.width > Main.player[npc.target].position.X && npc.position.X < Main.player[npc.target].position.X + (float)Main.player[npc.target].width && npc.position.Y < Main.player[npc.target].position.Y + (float)Main.player[npc.target].height)
                            {
                                npc.velocity.X = npc.velocity.X * 0.9f;
                                if (npc.velocity.Y < 0f)
                                {
                                    npc.velocity.Y = npc.velocity.Y + 0.3f;
                                }
                            }

                            if (npc.ai[2] == 0f || npc.ai[1] >= 1200f)
                            {
                                npc.ai[1] = 0f;
                                npc.ai[0] = 5f;
                            }
                        }

                        if (npc.wet && npc.directionY < 0)
                        {
                            npc.velocity.Y = npc.velocity.Y - 0.45f;
                        }

                        npc.rotation += MathHelper.Clamp(npc.velocity.X / 10f * (float)npc.direction, -0.314159274f, 0.314159274f);

                        return false;
                    }

                    if (npc.ai[0] == 5f)
                    {
                        npc.rotation = 0f;
                        npc.velocity.X = 0f;

                        if (npc.type == NPCID.GiantShelly || npc.type == NPCID.GiantShelly2)
                        {
                            npc.ai[1] += 1f;
                        }
                        else
                        {
                            npc.ai[1] += 2f;
                        }

                        if (npc.ai[1] >= 30f)
                        {
                            npc.TargetClosest(true);
                            npc.netUpdate = true;
                            npc.ai[1] = 0f;
                            npc.ai[0] = 0f;
                        }

                        if (npc.wet)
                        {
                            npc.ai[0] = 3f;
                            npc.ai[1] = 0f;
                        }
                    }
                }
            }

            return false;
        }
        #endregion

        #region Spider AI
        public static bool BuffedSpiderAI(NPC npc, Mod mod)
        {
            //Find a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead)
            {
                npc.TargetClosest(true);
            }

            float speed = 2.5f;
            float mvtAdjust = 0.1f;
            if (npc.type == NPCID.DesertScorpionWall)
            {
                speed = 5f;
                mvtAdjust = 0.2f;
            }
            if (CalamityWorld.death)
            {
                speed *= 1.25f;
                mvtAdjust *= 1.25f;
            }

            //Calculate how to reach the target
            Vector2 npcPos = npc.Center;
            Vector2 targetPos = Main.player[npc.target].Center;
            targetPos.X = (float)((int)(targetPos.X / 8f) * 8);
            targetPos.Y = (float)((int)(targetPos.Y / 8f) * 8);
            npcPos.X = (float)((int)(npcPos.X / 8f) * 8);
            npcPos.Y = (float)((int)(npcPos.Y / 8f) * 8);
            targetPos.X -= npcPos.X;
            targetPos.Y -= npcPos.Y;
            float targetDist = targetPos.Length();
            if (targetDist == 0f)
            {
                targetPos.X = npc.velocity.X;
                targetPos.Y = npc.velocity.Y;
            }
            else
            {
                targetDist = speed / targetDist;
                targetPos.X *= targetDist;
                targetPos.Y *= targetDist;
            }

            //If the target is dead, nobody cares
            if (Main.player[npc.target].dead)
            {
                targetPos.X = (float)npc.direction * speed / 2f;
                targetPos.Y = -speed / 2f;
            }

            //Sprite direction
            npc.spriteDirection = -1;

            //If you can't see the target, wander around
            if (!Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
            {
                npc.ai[0] += 1f;
                if (npc.ai[0] > 0f)
                {
                    npc.velocity.Y += 0.023f;
                }
                else
                {
                    npc.velocity.Y -= 0.023f;
                }
                if (npc.ai[0] < -100f || npc.ai[0] > 100f)
                {
                    npc.velocity.X += 0.023f;
                }
                else
                {
                    npc.velocity.X -= 0.023f;
                }
                if (npc.ai[0] > 200f)
                {
                    npc.ai[0] = -200f;
                }
                npc.velocity.X += targetPos.X * 0.009f;
                npc.velocity.Y += targetPos.Y * 0.009f;
                npc.rotation = npc.velocity.ToRotation();
                if (npc.velocity.X > 2.5f)
                {
                    npc.velocity.X *= 0.9f;
                }
                if (npc.velocity.X < -2.5f)
                {
                    npc.velocity.X *= 0.9f;
                }
                if (npc.velocity.Y > 2.5f)
                {
                    npc.velocity.Y *= 0.9f;
                }
                if (npc.velocity.Y < -2.5f)
                {
                    npc.velocity.Y *= 0.9f;
                }
                npc.velocity.X = MathHelper.Clamp(npc.velocity.X, -4f, 4f);
                npc.velocity.Y = MathHelper.Clamp(npc.velocity.Y, -4f, 4f);
            }
            //If target is in sight, move toward target
            else
            {
                if (npc.velocity.X < targetPos.X)
                {
                    npc.velocity.X += mvtAdjust;
                    if (npc.velocity.X < 0f && targetPos.X > 0f)
                    {
                        npc.velocity.X += mvtAdjust;
                    }
                }
                else if (npc.velocity.X > targetPos.X)
                {
                    npc.velocity.X -= mvtAdjust;
                    if (npc.velocity.X > 0f && targetPos.X < 0f)
                    {
                        npc.velocity.X -= mvtAdjust;
                    }
                }
                if (npc.velocity.Y < targetPos.Y)
                {
                    npc.velocity.Y += mvtAdjust;
                    if (npc.velocity.Y < 0f && targetPos.Y > 0f)
                    {
                        npc.velocity.Y += mvtAdjust;
                    }
                }
                else if (npc.velocity.Y > targetPos.Y)
                {
                    npc.velocity.Y -= mvtAdjust;
                    if (npc.velocity.Y > 0f && targetPos.Y < 0f)
                    {
                        npc.velocity.Y -= mvtAdjust;
                    }
                }
                npc.rotation = targetPos.ToRotation();
            }

            //Desert Scorpion has a different sprite orientation
            if (npc.type == NPCID.DesertScorpionWall)
            {
                npc.rotation += MathHelper.PiOver2;
            }

            //Wall collision behavior?
            float half = 0.5f;
            if (npc.collideX)
            {
                npc.netUpdate = true;
                npc.velocity.X = npc.oldVelocity.X * -half;
                if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                {
                    npc.velocity.X = 2f;
                }
                if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                {
                    npc.velocity.X = -2f;
                }
            }
            if (npc.collideY)
            {
                npc.netUpdate = true;
                npc.velocity.Y = npc.oldVelocity.Y * -half;
                if (npc.velocity.Y > 0f && npc.velocity.Y < 1.5f)
                {
                    npc.velocity.Y = 2f;
                }
                if (npc.velocity.Y < 0f && npc.velocity.Y > -1.5f)
                {
                    npc.velocity.Y = -2f;
                }
            }

            //Net update for changing directions
            if (((npc.velocity.X > 0f && npc.oldVelocity.X < 0f) || (npc.velocity.X < 0f && npc.oldVelocity.X > 0f) || (npc.velocity.Y > 0f && npc.oldVelocity.Y < 0f) || (npc.velocity.Y < 0f && npc.oldVelocity.Y > 0f)) && !npc.justHit)
            {
                npc.netUpdate = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                bool prehardmodeSpiders = npc.type == NPCID.WallCreeper || npc.type == NPCID.WallCreeperWall || npc.type == NPCID.BloodCrawler || npc.type == NPCID.BloodCrawlerWall;
                if (Main.netMode != NetmodeID.MultiplayerClient &&
                    npc.target >= 0 &&
                    (npc.type == NPCID.BlackRecluse || npc.type == NPCID.BlackRecluseWall || npc.type == NPCID.JungleCreeper || npc.type == NPCID.JungleCreeperWall || prehardmodeSpiders) &&
                    Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.localAI[0] += 1f;
                    if (npc.justHit)
                        npc.localAI[0] = 0f;

                    float webSpitGateValue = CalamityWorld.death ? 240f : 390f;
                    if (npc.localAI[0] >= webSpitGateValue)
                    {
                        npc.localAI[0] = 0f;
                        Vector2 velocity = Main.player[npc.target].Center - npc.Center;
                        velocity.Normalize();
                        velocity *= prehardmodeSpiders ? 5f : 8f;
                        Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, velocity, ProjectileID.WebSpit, 18, 0f, Main.myPlayer, 0f, 0f);
                    }
                }

                //Check for walls
                int npcX = (int)npc.Center.X / 16;
                int npcY = (int)npc.Center.Y / 16;
                bool climbingWall = false;
                for (int i = npcX - 1; i <= npcX + 1; i++)
                {
                    for (int j = npcY - 1; j <= npcY + 1; j++)
                    {
                        if (Main.tile[i, j].WallType > 0)
                        {
                            climbingWall = true;
                        }
                    }
                }
                //If not on a wall, transform to fighter form
                if (!climbingWall)
                {
                    if (npc.type == NPCID.JungleCreeperWall)
                    {
                        npc.Transform(NPCID.JungleCreeper);
                        return false;
                    }
                    if (npc.type == NPCID.BlackRecluseWall)
                    {
                        npc.Transform(NPCID.BlackRecluse);
                        return false;
                    }
                    if (npc.type == NPCID.BloodCrawlerWall)
                    {
                        npc.Transform(NPCID.BloodCrawler);
                        return false;
                    }
                    if (npc.type == NPCID.DesertScorpionWall)
                    {
                        npc.Transform(NPCID.DesertScorpionWalk);
                        return false;
                    }
                    npc.Transform(NPCID.WallCreeper);
                }
            }
            return false;
        }
        #endregion

        #region Herpling AI
        public static bool BuffedHerplingAI(NPC npc, Mod mod)
        {
            if (npc.ai[2] > 1f)
            {
                npc.ai[2] -= 1f;
            }

            if (npc.ai[2] == 0f)
            {
                npc.ai[0] = -100f;
                npc.ai[2] = 1f;
                npc.TargetClosest(true);
                npc.spriteDirection = npc.direction;
            }

            if (npc.wet && npc.type != NPCID.Derpling)
            {
                if (npc.collideX)
                {
                    npc.direction *= -npc.direction;
                    npc.spriteDirection = npc.direction;
                }

                if (npc.collideY)
                {
                    npc.TargetClosest(true);

                    if (npc.oldVelocity.Y < 0f)
                    {
                        npc.velocity.Y = 5f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - 2f;
                    }

                    npc.spriteDirection = npc.direction;
                }

                if (npc.velocity.Y > 4f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.9f;
                }

                npc.velocity.Y = npc.velocity.Y - 0.45f;

                if (npc.velocity.Y < -6f)
                {
                    npc.velocity.Y = -6f;
                }
            }

            if (npc.velocity.Y == 0f)
            {
                if (npc.ai[3] == npc.position.X)
                {
                    npc.direction *= -1;
                    npc.ai[2] = 300f;
                }

                npc.ai[3] = 0f;

                npc.velocity.X = npc.velocity.X * 0.8f;
                if ((double)npc.velocity.X > -0.1 && (double)npc.velocity.X < 0.1)
                {
                    npc.velocity.X = 0f;
                }

                if (npc.type == NPCID.Derpling)
                {
                    npc.ai[0] += 3f;
                }
                else
                {
                    npc.ai[0] += 10f;
                }

                Vector2 vector72 = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float num585 = Main.player[npc.target].position.X + (float)Main.player[npc.target].width * 0.5f - vector72.X;
                float num586 = Main.player[npc.target].position.Y + (float)Main.player[npc.target].height * 0.5f - vector72.Y;
                float num587 = (float)Math.Sqrt((double)(num585 * num585 + num586 * num586));
                float num588 = 400f / num587;

                if (npc.type == NPCID.Derpling)
                {
                    num588 *= 5f;
                }
                else
                {
                    num588 *= 10f;
                }

                if (num588 > 30f)
                {
                    num588 = 30f;
                }

                npc.ai[0] += (float)((int)num588);

                if (npc.ai[0] >= 0f)
                {
                    npc.netUpdate = true;

                    if (npc.ai[2] == 1f)
                    {
                        npc.TargetClosest(true);
                    }

                    if (npc.type == NPCID.Derpling)
                    {
                        if (npc.ai[1] == 2f)
                        {
                            npc.velocity.Y = -14f;
                            npc.velocity.X = npc.velocity.X + 3f * (float)npc.direction;
                            if (num587 < 350f && num587 > 200f)
                            {
                                npc.velocity.X = npc.velocity.X + (float)npc.direction;
                            }
                            npc.ai[0] = CalamityWorld.death ? -100f : -200f;
                            npc.ai[1] = 0f;
                            npc.ai[3] = npc.position.X;
                        }
                        else
                        {
                            npc.velocity.Y = -10f;
                            npc.velocity.X = npc.velocity.X + (float)(5 * npc.direction);
                            if (num587 < 350f && num587 > 200f)
                            {
                                npc.velocity.X = npc.velocity.X + (float)npc.direction;
                            }
                            npc.ai[0] = CalamityWorld.death ? -60f : -120f;
                            npc.ai[1] += 1f;
                        }
                    }
                    else if (npc.ai[1] == 3f)
                    {
                        npc.velocity.Y = -9f;
                        npc.velocity.X = npc.velocity.X + (float)(2 * npc.direction);
                        if (num587 < 350f && num587 > 200f)
                        {
                            npc.velocity.X = npc.velocity.X + (float)npc.direction;
                        }
                        npc.ai[0] = CalamityWorld.death ? -100f : -200f;
                        npc.ai[1] = 0f;
                        npc.ai[3] = npc.position.X;
                    }
                    else
                    {
                        npc.velocity.Y = -5f;
                        npc.velocity.X = npc.velocity.X + (float)(4 * npc.direction);
                        if (num587 < 350f && num587 > 200f)
                        {
                            npc.velocity.X = npc.velocity.X + (float)npc.direction;
                        }
                        npc.ai[0] = CalamityWorld.death ? -60f : -120f;
                        npc.ai[1] += 1f;
                    }
                }
                else if (npc.ai[0] >= -30f)
                {
                    npc.aiAction = 1;
                }

                npc.spriteDirection = npc.direction;

                return false;
            }

            if (npc.target < Main.maxPlayers)
            {
                if (npc.type == NPCID.Derpling)
                {
                    bool flag37 = false;
                    if (npc.position.Y + (float)npc.height < Main.player[npc.target].position.Y && npc.position.X + (float)npc.width > Main.player[npc.target].position.X && npc.position.X < Main.player[npc.target].position.X + (float)Main.player[npc.target].width)
                    {
                        flag37 = true;
                        npc.velocity.X = npc.velocity.X * 0.9f;
                        if (npc.velocity.Y < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y * 0.9f;
                            npc.velocity.Y = npc.velocity.Y + 0.15f;
                        }
                    }

                    if (!flag37 && ((npc.direction == 1 && npc.velocity.X < 4f) || (npc.direction == -1 && npc.velocity.X > -4f)))
                    {
                        if ((npc.direction == -1 && (double)npc.velocity.X < 0.1) || (npc.direction == 1 && (double)npc.velocity.X > -0.1))
                        {
                            npc.velocity.X = npc.velocity.X + 0.3f * (float)npc.direction;
                            return false;
                        }
                        npc.velocity.X = npc.velocity.X * 0.9f;
                    }
                }
                else if ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f))
                {
                    if ((npc.direction == -1 && (double)npc.velocity.X < 0.1) || (npc.direction == 1 && (double)npc.velocity.X > -0.1))
                    {
                        npc.velocity.X = npc.velocity.X + 0.3f * (float)npc.direction;
                        return false;
                    }
                    npc.velocity.X = npc.velocity.X * 0.9f;
                }
            }

            return false;
        }
        #endregion

        #region Flying Fish AI
        public static bool BuffedFlyingFishAI(NPC npc, Mod mod)
        {
            npc.noGravity = true;
            if (npc.collideX)
            {
                if (npc.oldVelocity.X > 0f)
                {
                    npc.direction = -1;
                }
                else
                {
                    npc.direction = 1;
                }
                npc.velocity.X = (float)npc.direction;
            }
            if (npc.collideY)
            {
                if (npc.oldVelocity.Y > 0f)
                {
                    npc.directionY = -1;
                }
                else
                {
                    npc.directionY = 1;
                }
                npc.velocity.Y = (float)npc.directionY;
            }
            if (npc.type == NPCID.EyeballFlyingFish)
            {
                npc.position += npc.netOffset;
                if (npc.alpha == 255)
                {
                    npc.velocity.Y = -6f;
                    npc.netUpdate = true;
                    for (int num662 = 0; num662 < 15; num662++)
                    {
                        Dust dust2 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                        Dust dust = dust2;
                        dust.velocity *= 0.5f;
                        dust2.scale = 1f + Main.rand.NextFloat() * 0.5f;
                        dust2.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                        dust = dust2;
                        dust.velocity += npc.velocity * 0.5f;
                    }
                }

                npc.alpha -= 15;
                if (npc.alpha < 0)
                    npc.alpha = 0;

                if (npc.alpha != 0)
                {
                    for (int num663 = 0; num663 < 2; num663++)
                    {
                        Dust dust3 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                        Dust dust = dust3;
                        dust.velocity *= 1f;
                        dust3.scale = 1f + Main.rand.NextFloat() * 0.5f;
                        dust3.fadeIn = 1.5f + Main.rand.NextFloat() * 0.5f;
                        dust = dust3;
                        dust.velocity += npc.velocity * 0.3f;
                    }
                }

                if (Main.rand.NextBool(3))
                {
                    Dust dust4 = Dust.NewDustDirect(npc.position, npc.width, npc.height, 5);
                    Dust dust = dust4;
                    dust.velocity *= 0f;
                    dust4.alpha = 120;
                    dust4.scale = 0.7f + Main.rand.NextFloat() * 0.5f;
                    dust = dust4;
                    dust.velocity += npc.velocity * 0.3f;
                }

                npc.position -= npc.netOffset;
            }
            int num631 = npc.target;
            int direction2 = npc.direction;
            if (npc.target == Main.maxPlayers || (Main.player[npc.target].wet && npc.type != NPCID.EyeballFlyingFish) || Main.player[npc.target].dead || Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
            {
                npc.ai[0] = 90f;
                npc.TargetClosest(true);
            }
            else if (npc.ai[0] > 0f)
            {
                npc.ai[0] -= 1f;
                npc.TargetClosest(true);
            }
            if (npc.netUpdate && num631 == npc.target && direction2 == npc.direction)
            {
                npc.netUpdate = false;
            }
            float num632 = 0.05f;
            float num633 = 0.01f;
            float num634 = 6f;
            float num635 = 3f;
            float num636 = 30f;
            float num637 = 100f;
            float num638 = Math.Abs(npc.position.X + (float)(npc.width / 2) - (Main.player[npc.target].position.X + (float)(Main.player[npc.target].width / 2)));
            float num639 = Main.player[npc.target].position.Y - (float)(npc.height / 2);
            if (npc.type == NPCID.FlyingAntlion || npc.type == NPCID.GiantFlyingAntlion)
            {
                num632 = 0.09f;
                num633 = 0.03f;
                num634 = 9f;
                num635 = 6f;
                num636 = 40f;
                num637 = 150f;
                num639 = Main.player[npc.target].Center.Y - (float)(npc.height / 2);
                npc.rotation = npc.velocity.X * 0.1f;
                int num;
                for (int num640 = 0; num640 < Main.maxNPCs; num640 = num + 1)
                {
                    if (num640 != npc.whoAmI && Main.npc[num640].active && Main.npc[num640].type == npc.type && Math.Abs(npc.position.X - Main.npc[num640].position.X) + Math.Abs(npc.position.Y - Main.npc[num640].position.Y) < (float)npc.width)
                    {
                        if (npc.position.X < Main.npc[num640].position.X)
                        {
                            npc.velocity.X = npc.velocity.X - 0.05f;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X + 0.05f;
                        }
                        if (npc.position.Y < Main.npc[num640].position.Y)
                        {
                            npc.velocity.Y = npc.velocity.Y - 0.05f;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y + 0.05f;
                        }
                    }
                    num = num640;
                }
            }
            else if (npc.type == NPCID.EyeballFlyingFish)
            {
                num632 = 0.16f;
                num633 = 0.12f;
                num634 = 9f;
                num635 = 5f;
                num636 = 0f;
                num637 = 250f;
                num639 = Main.player[npc.target].position.Y;
                if (Main.dayTime)
                {
                    num639 = 0f;
                    npc.direction *= -1;
                }
            }
            if (CalamityWorld.death)
            {
                num634 *= 1.25f;
                num632 *= 1.25f;
            }
            if (npc.ai[0] <= 0f)
            {
                num634 *= 0.8f;
                num632 *= 0.7f;
                num639 = npc.Center.Y + (float)(npc.directionY * 1000);
                if (npc.velocity.X < 0f)
                {
                    npc.direction = -1;
                }
                else if (npc.velocity.X > 0f || npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            if (num638 > num636)
            {
                if (npc.direction == -1 && npc.velocity.X > -num634)
                {
                    npc.velocity.X = npc.velocity.X - num632;
                    if (npc.velocity.X > num634)
                    {
                        npc.velocity.X = npc.velocity.X - num632;
                    }
                    else if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num632 / 2f;
                    }
                    if (npc.velocity.X < -num634)
                    {
                        npc.velocity.X = -num634;
                    }
                }
                else if (npc.direction == 1 && npc.velocity.X < num634)
                {
                    npc.velocity.X = npc.velocity.X + num632;
                    if (npc.velocity.X < -num634)
                    {
                        npc.velocity.X = npc.velocity.X + num632;
                    }
                    else if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num632 / 2f;
                    }
                    if (npc.velocity.X > num634)
                    {
                        npc.velocity.X = num634;
                    }
                }
            }
            if (num638 > num637)
            {
                num639 -= num637 / 2f;
            }
            if (npc.position.Y < num639)
            {
                npc.velocity.Y = npc.velocity.Y + num633;
                if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y = npc.velocity.Y + num633;
                }
            }
            else
            {
                npc.velocity.Y = npc.velocity.Y - num633;
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y - num633;
                }
            }
            if (npc.velocity.Y < -num635)
            {
                npc.velocity.Y = -num635;
            }
            if (npc.velocity.Y > num635)
            {
                npc.velocity.Y = num635;
            }
            if (npc.wet && npc.type != NPCID.EyeballFlyingFish)
            {
                if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y = npc.velocity.Y * 0.95f;
                }
                npc.velocity.Y = npc.velocity.Y - 0.7f;
                if (npc.velocity.Y < -6f)
                {
                    npc.velocity.Y = -6f;
                }
            }
            return false;
        }
        #endregion

        #region Angry Nimbus AI
        public static bool BuffedAngryNimbusAI(NPC npc, Mod mod)
        {
            npc.noGravity = true;
            npc.TargetClosest(true);
            float speed = CalamityWorld.death ? 9f : 6f;
            float acceleration = CalamityWorld.death ? 0.3f : 0.25f;

            Vector2 idealVelocity = npc.SafeDirectionTo(Main.player[npc.target].Center - 300f * Vector2.UnitY) * speed;
            float playerDistance = npc.Distance(Main.player[npc.target].Center);
            if (playerDistance < 20f)
                idealVelocity = npc.velocity;

            // Yes, I understand that npc.SimpleFlyMovement exists. However, the "acceleration * 2f" is not a part of that method.
            // It is not identical to what is being achieved here.
            if (npc.velocity.X < idealVelocity.X)
            {
                npc.velocity.X += acceleration;
                if (npc.velocity.X < 0f && idealVelocity.X > 0f)
                {
                    npc.velocity.X = npc.velocity.X + acceleration * 2f;
                }
            }
            else if (npc.velocity.X > idealVelocity.X)
            {
                npc.velocity.X -= acceleration;
                if (npc.velocity.X > 0f && idealVelocity.X < 0f)
                {
                    npc.velocity.X -= acceleration * 2f;
                }
            }
            if (npc.velocity.Y < idealVelocity.Y)
            {
                npc.velocity.Y += acceleration;
                if (npc.velocity.Y < 0f && idealVelocity.Y > 0f)
                {
                    npc.velocity.Y += acceleration * 2f;
                }
            }
            else if (npc.velocity.Y > idealVelocity.Y)
            {
                npc.velocity.Y -= acceleration;
                if (npc.velocity.Y > 0f && idealVelocity.Y < 0f)
                {
                    npc.velocity.Y -= acceleration * 2f;
                }
            }

            // Make it rain
            float minXRainDistance = CalamityWorld.death ? 200f : 150f;
            if (npc.Center.X > Main.player[npc.target].position.X - minXRainDistance &&
                npc.position.X < Main.player[npc.target].Center.X + minXRainDistance &&
                npc.Center.Y < Main.player[npc.target].position.Y &&
                Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
                Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (npc.justHit)
                    npc.ai[0] = 0f;

                npc.ai[0] += 1f;
                if (npc.ai[0] % 8f == 0f)
                {
                    Vector2 rainSpawnPosition = npc.position + new Vector2(10f + Main.rand.Next(npc.width - 20), npc.height + 4f);

                    Projectile.NewProjectile(npc.GetSource_FromAI(), rainSpawnPosition, Vector2.UnitY * 5f, ProjectileID.RainNimbus, 20, 0f, Main.myPlayer, 0f, 0f);
                    if (npc.ai[0] % 16f == 0f)
                    {
                        float speedX = (float)Main.rand.NextFloat(CalamityWorld.death ? -6f : -3f, CalamityWorld.death ? 6f : 3f) * (Main.rand.NextFloat() - 0.5f);
                        Projectile.NewProjectile(npc.GetSource_FromAI(), rainSpawnPosition, new Vector2(speedX, 5f), ProjectileID.FrostShard, 20, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                if (npc.ai[0] >= 607f)
                    npc.ai[0] = 0f;
            }
            return false;
        }
        #endregion

        #region Spore AI
        public static bool BuffedSporeAI(NPC npc, Mod mod)
        {
            if (npc.type == NPCID.Spore)
                Lighting.AddLight(npc.Center, 0.2f, 0.1f, 0.2f);

            if (npc.timeLeft > 5)
                npc.timeLeft = 5;

            npc.noTileCollide = true;
            npc.velocity.Y += 0.02f;

            // Ensure slow fall
            if (npc.velocity.Y > 1f)
                npc.velocity.Y = 1f;

            // Use regular AI if not spawned by a Giant Plantera Bulb
            if (npc.ai[0] != -1f)
            {
                npc.TargetClosest(true);
                float acceleration = Main.expertMode ? 0.2f : 0.1f;
                float velocity = Main.expertMode ? 5f : 3f;
                if (CalamityWorld.death)
                {
                    acceleration *= 1.25f;
                    velocity *= 1.25f;
                }

                // Simple movement AI. You shouldn't need any help from comments to parse this.
                if (npc.Center.X < Main.player[npc.target].position.X)
                {
                    if (npc.velocity.X < 0f)
                    {
                        npc.velocity.X *= 0.96f;
                    }
                    npc.velocity.X += acceleration;
                }
                else if (npc.position.X > Main.player[npc.target].Center.X)
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X *= 0.96f;
                    }
                    npc.velocity.X -= acceleration;
                }
                if (npc.velocity.X > velocity || npc.velocity.X < -velocity)
                {
                    npc.velocity.X *= 0.97f;
                }
            }
            else
            {
                npc.velocity.X *= 0.98f;
                npc.damage = npc.defDamage = 0;
            }

            npc.rotation = npc.velocity.X * 0.2f;

            return false;
        }
        #endregion

        #region Tesla Turret AI
        public static bool BuffedTeslaTurretAI(NPC npc, Mod mod)
        {
            npc.TargetClosest(false);

            npc.spriteDirection = npc.direction;

            npc.velocity.X *= 0.93f;
            if (Math.Abs(npc.velocity.X) < 0.1f)
            {
                npc.velocity.X = 0f;
            }

            float appearTime = 120f;
            float alphaFadeinTime = 60f;

            // Spend the first "appearTime" frames sitting around and spawning.
            if (npc.ai[1] < appearTime)
            {
                npc.ai[1] += 1f;
                if (npc.ai[1] > appearTime - alphaFadeinTime)
                {
                    float alphaRatio = (npc.ai[1] - alphaFadeinTime) / (appearTime - alphaFadeinTime);
                    npc.alpha = (int)((1f - alphaRatio) * 255f);
                }
                else
                {
                    npc.alpha = 255;
                }

                npc.dontTakeDamage = true;

                npc.frameCounter = 0.0;
                npc.frame.Y = 0;

                // Circular dust
                float angularRatio = npc.ai[1] / alphaFadeinTime;
                Vector2 spinningpoint = new Vector2(0f, -30f).RotatedBy(angularRatio * 1.5f * MathHelper.TwoPi) * new Vector2(1f, 0.4f);

                for (int i = 0; i < 4; i++)
                {
                    Vector2 dustSpawnDelta = Vector2.Zero;
                    float scaleFactor2 = 1f;
                    switch (i)
                    {
                        case 0:
                            dustSpawnDelta = Vector2.UnitY * -15f;
                            scaleFactor2 = 0.15f;
                            break;
                        case 1:
                            dustSpawnDelta = Vector2.UnitY * -5f;
                            scaleFactor2 = 0.3f;
                            break;
                        case 2:
                            dustSpawnDelta = Vector2.UnitY * 5f;
                            scaleFactor2 = 0.6f;
                            break;
                        case 3:
                            dustSpawnDelta = Vector2.UnitY * 20f;
                            scaleFactor2 = 0.45f;
                            break;
                    }

                    int idx = Dust.NewDust(npc.Center, 0, 0, 226, 0f, 0f, 100, default, 0.5f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = npc.Center + spinningpoint * scaleFactor2 + dustSpawnDelta;
                    Main.dust[idx].velocity = Vector2.Zero;
                    spinningpoint *= -1f;

                    idx = Dust.NewDust(npc.Center, 0, 0, 226, 0f, 0f, 100, default, 0.5f);
                    Main.dust[idx].noGravity = true;
                    Main.dust[idx].position = npc.Center + spinningpoint * scaleFactor2 + dustSpawnDelta;
                    Main.dust[idx].velocity = Vector2.Zero;
                }

                Lighting.AddLight((int)npc.Center.X / 16, (int)(npc.Center.Y - 10f) / 16, 0.1f * angularRatio, 0.5f * angularRatio, 0.7f * angularRatio);

                return false;
            }

            Lighting.AddLight((int)npc.Center.X / 16, (int)(npc.Center.Y - 10f) / 16, 0.1f, 0.5f, 0.7f);
            npc.dontTakeDamage = false;

            if (npc.ai[0] < 60f)
            {
                npc.ai[0] += 1f;
            }

            // Reset laser shoot counter
            if (npc.justHit)
            {
                npc.ai[0] = 0f;
            }

            // Shoot laser
            if (npc.ai[0] == 60f)
            {
                npc.ai[0] = CalamityWorld.death ? -60f : -120f;

                // The "+ Main.player[npc.target].velocity * 20f" part ensures the turret will aim ahead of the player
                Vector2 distanceVector = Main.player[npc.target].Center + Main.player[npc.target].velocity * 20f - (npc.Center - Vector2.UnitY * 10f);

                if (distanceVector.HasNaNs())
                {
                    distanceVector = -Vector2.UnitY;
                }
                Vector2 velocity = Vector2.Normalize(distanceVector) * 14f;

                Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center - Vector2.UnitY * 10f, velocity, ProjectileID.MartianTurretBolt, 28, 0f, Main.myPlayer, 0f, 0f);
            }

            return false;
        }
        #endregion

        #region Corite AI
        public static bool BuffedCoriteAI(NPC npc, Mod mod)
        {
            npc.TargetClosest(false);

            npc.rotation = npc.velocity.ToRotation();

            if (Math.Sign(npc.velocity.X) != 0)
            {
                npc.spriteDirection = -Math.Sign(npc.velocity.X);
            }
            if (npc.rotation < -MathHelper.PiOver2)
            {
                npc.rotation += MathHelper.Pi;
            }
            if (npc.rotation > MathHelper.PiOver2)
            {
                npc.rotation -= MathHelper.Pi;
            }

            if (npc.type == NPCID.SolarCorite)
            {
                npc.spriteDirection = Math.Sign(npc.velocity.X);
            }

            float npcKBResist = 0.4f;

            float upwardChargeSpeed = 12f;
            float idealUpwardDelta = 200f;
            float maximumDistanceBeforeCharge = 900f;
            float upwardMovementIntertia = 30f;

            float chargePhaseWait = 30f;
            float chargeWaitSlowdownMult = 0.95f;
            int chargeRandomness = 50;
            float chargeSpeed = 14f;
            float maximumChargeTime = 30f;
            float chargeDistanceCheck = 100f;
            float chargeIntertia = 20f;
            float chargeAcceleration = 0f;

            // Stops charging if speed is less than this when charging.
            float minimumChargeSpeed = 7f;

            bool hasCoolDustPhase = true;

            if (npc.type == NPCID.SolarCorite)
            {
                npcKBResist = 0.3f;
                upwardChargeSpeed = 10f;
                idealUpwardDelta = 300f;
                maximumDistanceBeforeCharge = 1000f;
                upwardMovementIntertia = 60f;
                chargePhaseWait = 5f;
                chargeWaitSlowdownMult = 0.8f;
                chargeRandomness = 0;
                chargeSpeed = 10f;
                chargeDistanceCheck = 150f;
                chargeIntertia = 60f;
                chargeAcceleration = 0.333333343f;
                minimumChargeSpeed = 8f;
                hasCoolDustPhase = false;
            }

            chargeAcceleration *= chargeIntertia;

            if (CalamityWorld.death)
            {
                upwardChargeSpeed *= 1.25f;
                chargeSpeed *= 1.25f;
            }

            // Drone dust
            if (npc.type == NPCID.MartianDrone && npc.ai[0] != 3f)
            {
                int idx = Dust.NewDust(npc.position, npc.width, npc.height, 226, 0f, 0f, 100, default, 0.5f);
                Main.dust[idx].noGravity = true;
                Main.dust[idx].velocity = npc.velocity / 5f;
                Vector2 rotationVector = new Vector2(-10f, 10f);

                if (npc.spriteDirection == 1)
                {
                    rotationVector.X *= -1f;
                }

                rotationVector = rotationVector.RotatedBy(npc.rotation);
                Main.dust[idx].position = npc.Center + rotationVector;
            }

            if (npc.type == NPCID.SolarCorite)
            {
                int dustSpawnChance = (npc.ai[0] == 2f) ? 2 : 1;
                int dustSpawnAreaSize = (npc.ai[0] == 2f) ? 30 : 20;
                for (int num1055 = 0; num1055 < 2; num1055++)
                {
                    if (Main.rand.Next(3) < dustSpawnChance)
                    {
                        int idx = Dust.NewDust(npc.Center - new Vector2(dustSpawnAreaSize), dustSpawnAreaSize * 2, dustSpawnAreaSize * 2, 6, npc.velocity.X * 0.5f, npc.velocity.Y * 0.5f, 90, default, 1.5f);
                        Main.dust[idx].noGravity = true;
                        Dust dust = Main.dust[idx];
                        dust.velocity *= 0.2f;
                        Main.dust[idx].fadeIn = 1f;
                    }
                }
            }

            // Move upward.
            if (npc.ai[0] == 0f)
            {
                npc.knockBackResist = npcKBResist;
                Vector2 playerDistanceNorm = Main.player[npc.target].Center - npc.Center;
                Vector2 upwardVelocity = playerDistanceNorm - Vector2.UnitY * idealUpwardDelta;
                float playerDistance = npc.Distance(Main.player[npc.target].Center);
                playerDistanceNorm = Vector2.Normalize(playerDistanceNorm) * upwardChargeSpeed;
                upwardVelocity = Vector2.Normalize(upwardVelocity) * upwardChargeSpeed;
                bool closeAngleDistance = Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1);

                if (npc.ai[3] >= 120f)
                {
                    closeAngleDistance = true;
                }

                // In simpler terms, this means that we incoporate a bool which states:
                //    The angular distance between this and the player is greater than pi/8 or
                //    less than pi - pi/8.
                // into the already existing boolean.
                closeAngleDistance &= npc.AngleTo(Main.player[npc.target].Center) > (MathHelper.Pi / 8f) &&
                    npc.AngleTo(Main.player[npc.target].Center) < (MathHelper.Pi - MathHelper.Pi / 8f);

                // If in a relatively close area of the player, or we meet the angle criteria above, prepare for charge.
                if (playerDistance < maximumDistanceBeforeCharge || closeAngleDistance)
                {
                    npc.ai[0] = 1f;
                    npc.ai[2] = playerDistanceNorm.X;
                    npc.ai[3] = playerDistanceNorm.Y;
                    npc.netUpdate = true;
                }
                // Otherwise, resume upward movement.
                else
                {
                    npc.velocity = (npc.velocity * (upwardMovementIntertia - 1f) + upwardVelocity) / upwardMovementIntertia;

                    if (!closeAngleDistance)
                    {
                        npc.ai[3] += 1f;
                        if (npc.ai[3] == 120f)
                        {
                            npc.netUpdate = true;
                        }
                    }
                    else
                    {
                        npc.ai[3] = 0f;
                    }
                }
            }
            // Slow down and prepare for charge.
            else if (npc.ai[0] == 1f)
            {
                npc.knockBackResist = 0f;
                npc.velocity *= chargeWaitSlowdownMult;

                npc.ai[1] += 1f;
                // If enough time has passed, charge.
                if (npc.ai[1] >= chargePhaseWait)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                    Vector2 velocity = new Vector2(npc.ai[2], npc.ai[3]) +
                        new Vector2(Main.rand.Next(-chargeRandomness, chargeRandomness + 1), Main.rand.Next(-chargeRandomness, chargeRandomness + 1)) * 0.04f;
                    velocity.Normalize();
                    velocity *= chargeSpeed;
                    npc.velocity = velocity;
                }

                // Spawn some cool dust.
                if (npc.type == NPCID.MartianDrone && Main.rand.NextBool(4))
                {
                    int idx = Dust.NewDust(npc.position, npc.width, npc.height, 226, 0f, 0f, 100, default, 0.5f);
                    Main.dust[idx].noGravity = true;
                    Dust dust = Main.dust[idx];
                    dust.velocity *= 2f;
                    Main.dust[idx].velocity = Main.dust[idx].velocity / 2f + Vector2.Normalize(Main.dust[idx].position - npc.Center);
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.knockBackResist = 0f;

                npc.ai[1] += 1f;
                bool aboveAndFar = Vector2.Distance(npc.Center, Main.player[npc.target].Center) > chargeDistanceCheck && npc.Center.Y > Main.player[npc.target].Center.Y;
                // If time is up and the player is (relatively) far, or the velocity is (relatively) low, reset.
                if ((npc.ai[1] >= maximumChargeTime & aboveAndFar) || npc.velocity.Length() < minimumChargeSpeed)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.velocity /= 2f;
                    npc.netUpdate = true;

                    if (npc.type == NPCID.SolarCorite)
                    {
                        npc.ai[1] = 45f;
                        npc.ai[0] = 4f;
                    }
                }
                else
                {
                    Vector2 distanceNormalized = Vector2.Normalize(Main.player[npc.target].Center - npc.Center);

                    if (distanceNormalized.HasNaNs())
                    {
                        distanceNormalized = new Vector2(npc.direction, 0f);
                    }

                    npc.velocity = (npc.velocity * (chargeIntertia - 1f) + distanceNormalized * (npc.velocity.Length() + chargeAcceleration)) / chargeIntertia;
                }

                if (hasCoolDustPhase && Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                npc.ai[1] -= 3f;
                if (npc.ai[1] <= 0f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                npc.velocity *= CalamityWorld.death ? 0.9f : 0.95f;
            }

            if (hasCoolDustPhase && npc.ai[0] != 3f && Vector2.Distance(npc.Center, Main.player[npc.target].Center) < 64f)
            {
                npc.ai[0] = 3f;
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
                npc.ai[3] = 0f;
                npc.netUpdate = true;
            }

            // Dust
            if (npc.ai[0] == 3f)
            {
                npc.position = npc.Center;
                npc.width = npc.height = CalamityWorld.death ? 360 : 240;
                npc.position -= npc.Size;

                npc.velocity = Vector2.Zero;

                npc.damage = 160;

                npc.alpha = 255;
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.2f, 0.7f, 1.1f);

                for (int i = 0; i < 10; i++)
                {
                    int idx = Dust.NewDust(npc.position, npc.width, npc.height, 31, 0f, 0f, 100, default, 1.5f);
                    Dust dust = Main.dust[idx];
                    dust.velocity *= 1.4f;
                    Main.dust[idx].position = ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * ((float)Main.rand.NextDouble() * 96f) + npc.Center;
                }

                for (int i = 0; i < 40; i++)
                {
                    int idx = Dust.NewDust(npc.position, npc.width, npc.height, 226, 0f, 0f, 100, default, 0.5f);
                    Main.dust[idx].noGravity = true;
                    Dust dust = Main.dust[idx];
                    dust.velocity *= 2f;
                    Main.dust[idx].position = ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * ((float)Main.rand.NextDouble() * 96f) + npc.Center;
                    Main.dust[idx].velocity = Main.dust[idx].velocity / 2f + Vector2.Normalize(Main.dust[idx].position - npc.Center);

                    if (Main.rand.NextBool())
                    {
                        idx = Dust.NewDust(npc.position, npc.width, npc.height, 226, 0f, 0f, 100, default, 0.9f);
                        Main.dust[idx].noGravity = true;
                        dust = Main.dust[idx];
                        dust.velocity *= 1.2f;
                        Main.dust[idx].position = ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * ((float)Main.rand.NextDouble() * 96f) + npc.Center;
                        Main.dust[idx].velocity = Main.dust[idx].velocity / 2f + Vector2.Normalize(Main.dust[idx].position - npc.Center);
                    }

                    if (Main.rand.NextBool(4))
                    {
                        idx = Dust.NewDust(npc.position, npc.width, npc.height, 226, 0f, 0f, 100, default, 0.7f);
                        dust = Main.dust[idx];
                        dust.velocity *= 1.2f;
                        Main.dust[idx].position = ((float)Main.rand.NextDouble() * MathHelper.TwoPi).ToRotationVector2() * ((float)Main.rand.NextDouble() * 96f) + npc.Center;
                        Main.dust[idx].velocity = Main.dust[idx].velocity / 2f + Vector2.Normalize(Main.dust[idx].position - npc.Center);
                    }
                }

                npc.ai[1] += 1f;
                if (npc.ai[1] >= 3f)
                {
                    SoundEngine.PlaySound(SoundID.Item14, npc.position);
                    npc.life = 0;
                    npc.HitEffect(0, 10.0);
                    npc.active = false;
                }
            }

            return false;
        }
        #endregion

        #region Martian Probe AI
        public static bool BuffedMartianProbeAI(NPC npc, Mod mod)
        {
            // Float around
            if (npc.ai[0] == 0f)
            {
                if (npc.direction == 0)
                {
                    npc.TargetClosest(true);
                    npc.netUpdate = true;
                }

                if (npc.collideX)
                {
                    npc.direction = -npc.direction;
                    npc.netUpdate = true;
                }

                npc.velocity.X = 6f * (float)npc.direction;
                Point centerTileCoords = npc.Center.ToTileCoordinates();
                int distanceFromGround = 30;

                if (WorldGen.InWorld(centerTileCoords.X, centerTileCoords.Y, 30))
                {
                    for (int y = 0; y < 30; y++)
                    {
                        if (WorldGen.SolidTile(centerTileCoords.X, centerTileCoords.Y + y))
                        {
                            distanceFromGround = y;
                            break;
                        }
                    }
                }

                if (distanceFromGround < 15)
                {
                    npc.velocity.Y = Math.Max(npc.velocity.Y - 0.05f, -3.5f);
                }
                else if (distanceFromGround < 20)
                {
                    npc.velocity.Y *= 0.95f;
                }
                else
                {
                    npc.velocity.Y = Math.Min(npc.velocity.Y + 0.05f, 1.5f);
                }

                int playerIndex = npc.FindClosestPlayer(out float distanceFromPlayer);
                if (playerIndex == -1 || Main.player[playerIndex].dead)
                {
                    return false;
                }

                // If a player is below and nearby the probe, become active
                if (distanceFromPlayer < 440f && Main.player[playerIndex].Center.Y > npc.Center.Y)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            // Wait
            else if (npc.ai[0] == 1f)
            {
                npc.ai[1] += 1f;

                npc.velocity *= 0.93f;

                if (npc.ai[1] >= (CalamityWorld.death ? 5f : 45f))
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 2f;

                    int closetPlayer = npc.FindClosestPlayer();
                    // Update the X acceleration
                    npc.ai[3] = closetPlayer != -1 ? (Main.player[closetPlayer].Center.X < npc.Center.X).ToDirectionInt() : 1f;

                    npc.netUpdate = true;
                }
            }
            // And fly away
            else if (npc.ai[0] == 2f)
            {
                npc.noTileCollide = true;

                npc.ai[1] += 1f;

                npc.velocity.Y = Math.Max(npc.velocity.Y - 0.2f, -12f);
                npc.velocity.X = Math.Min(npc.velocity.X + npc.ai[3] * 0.1f, 6f);

                // If above the world or enough time has passed, summon the naked grey gods.
                if ((npc.position.Y < -npc.height || npc.ai[1] >= 135f) && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Main.StartInvasion(InvasionID.MartianMadness);
                    npc.active = false;
                    npc.netUpdate = true;
                }
            }

            Vector3 lightColor = Color.SkyBlue.ToVector3();
            if (npc.ai[0] == 2f)
            {
                lightColor = Color.Red.ToVector3();
            }
            lightColor *= 0.65f;

            Lighting.AddLight(npc.Center, lightColor);

            return false;
        }
        #endregion

        #region Mothron Egg AI
        public static bool BuffedMothronEggAI(NPC npc, Mod mod)
        {
            // Define what exactly is going to be shit out of this egg when it's ready.
            if (npc.ai[1] == 0f)
            {
                npc.ai[1] = (Main.rand.NextBool(10) && NPC.CountNPCS(NPCID.Mothron) < 2) ? NPCID.Mothron : NPCID.MothronSpawn;

                if ((int)npc.ai[1] == NPCID.Mothron)
                {
                    npc.defense = (int)(npc.defDefense * 1.5);
                    npc.scale *= 2f;
                    npc.width = npc.height = (int)(34f * npc.scale);
                    npc.netUpdate = true;
                }
            }

            // Fall to the side like a sack of potatoes
            if (npc.velocity.Y == 0f)
            {
                npc.velocity.X *= 0.9f;
                npc.rotation += npc.velocity.X * 0.02f;
            }
            else
            {
                npc.velocity.X *= 0.99f;
                npc.rotation += npc.velocity.X * 0.04f;
            }

            // How much time is needed before the egg hatches
            float hatchTimer = ((int)npc.ai[1] == NPCID.Mothron ? 480f : 240f);
            if (CalamityWorld.death)
                hatchTimer *= 0.5f;

            npc.ai[0] += 1f;
            if (npc.ai[0] >= hatchTimer)
            {
                int hatchType = NPC.CountNPCS(NPCID.Mothron) < 2 ? (int)npc.ai[1] : NPCID.MothronSpawn;
                npc.Transform(hatchType);
            }

            // Jump around sometimes
            if (Main.netMode != NetmodeID.MultiplayerClient && npc.velocity.Y == 0f && Math.Abs(npc.velocity.X) < 0.2f && npc.ai[0] >= hatchTimer * 0.75f)
            {
                float hatchCompleteness = npc.ai[0] - hatchTimer * 0.75f;
                hatchCompleteness /= hatchTimer * 0.25f;
                if (Main.rand.Next(-10, 120) < hatchCompleteness * 100f)
                {
                    npc.velocity.Y -= Main.rand.Next(20, 40) * 0.025f;
                    npc.velocity.X += Main.rand.Next(-20, 20) * 0.025f;
                    npc.velocity *= 1f + hatchCompleteness * 2f;
                    npc.netUpdate = true;
                }
            }

            return false;
        }
        #endregion

        #region Granite Elemental AI
        public static bool BuffedGraniteElementalAI(NPC npc, Mod mod)
        {
            npc.noGravity = true;
            npc.noTileCollide = false;
            npc.defense = npc.defDefense;
            if (npc.justHit && Main.netMode != NetmodeID.MultiplayerClient && Main.rand.NextBool(10))
            {
                npc.netUpdate = true;
                npc.ai[0] = -1f;
                npc.ai[1] = 0f;
            }
            if (npc.ai[0] == -1f)
            {
                npc.defense = npc.defDefense + 10;
                npc.noGravity = false;
                npc.velocity.X *= 0.98f;
                npc.ai[1] += 1f;
                if (npc.ai[1] >= 120f)
                {
                    npc.ai[0] = (npc.ai[1] = (npc.ai[2] = (npc.ai[3] = 0f)));
                }
            }
            else if (npc.ai[0] == 0f)
            {
                npc.TargetClosest(true);
                if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[0] = 1f;
                    return false;
                }
                Vector2 value46 = Main.player[npc.target].Center - npc.Center;
                value46.Y -= (float)(Main.player[npc.target].height / 4);
                float attackTimeMax1 = value46.Length();
                if (attackTimeMax1 > (CalamityWorld.death ? 400f : 800f))
                {
                    npc.ai[0] = 2f;
                    return false;
                }
                Vector2 center30 = npc.Center;
                center30.X = Main.player[npc.target].Center.X;
                Vector2 vector243 = center30 - npc.Center;
                if (vector243.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center30, 1, 1))
                {
                    npc.ai[0] = 3f;
                    npc.ai[1] = center30.X;
                    npc.ai[2] = center30.Y;
                    Vector2 center31 = npc.Center;
                    center31.Y = Main.player[npc.target].Center.Y;
                    if (vector243.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center31, 1, 1) && Collision.CanHit(center31, 1, 1, Main.player[npc.target].position, 1, 1))
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = center31.X;
                        npc.ai[2] = center31.Y;
                    }
                }
                else
                {
                    center30 = npc.Center;
                    center30.Y = Main.player[npc.target].Center.Y;
                    if ((center30 - npc.Center).Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center30, 1, 1))
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = center30.X;
                        npc.ai[2] = center30.Y;
                    }
                }
                if (npc.ai[0] == 0f)
                {
                    npc.localAI[0] = 0f;
                    value46.Normalize();
                    value46 *= 0.5f;
                    npc.velocity += value46;
                    npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                Vector2 value47 = Main.player[npc.target].Center - npc.Center;
                float attackTimeMax2 = value47.Length();
                float attackTimeMax3 = 2f;
                attackTimeMax3 += attackTimeMax2 / (CalamityWorld.death ? 160f : 180f);
                int attackTimeMax4 = 50;
                value47.Normalize();
                value47 *= attackTimeMax3;
                npc.velocity = (npc.velocity * (float)(attackTimeMax4 - 1) + value47) / (float)attackTimeMax4;
                if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.noTileCollide = true;
                Vector2 value48 = Main.player[npc.target].Center - npc.Center;
                float attackTimeMax5 = value48.Length();
                float scaleFactor23 = CalamityWorld.death ? 3f : 2.5f;
                int attackTimeMax6 = 4;
                value48.Normalize();
                value48 *= scaleFactor23;
                npc.velocity = (npc.velocity * (float)(attackTimeMax6 - 1) + value48) / (float)attackTimeMax6;
                if (attackTimeMax5 < 600f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                Vector2 value49 = new Vector2(npc.ai[1], npc.ai[2]);
                Vector2 value50 = value49 - npc.Center;
                float attackTimeMax7 = value50.Length();
                float attackTimeMax8 = 2f;
                float attackTimeMax9 = 3f;
                value50.Normalize();
                value50 *= attackTimeMax8;
                npc.velocity = (npc.velocity * (attackTimeMax9 - 1f) + value50) / attackTimeMax9;
                if (npc.collideX || npc.collideY)
                {
                    npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
                }
                if (attackTimeMax7 < attackTimeMax8 || attackTimeMax7 > 800f || Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[0] = 0f;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                if (npc.collideX)
                {
                    npc.velocity.X = npc.velocity.X * -0.8f;
                }
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.velocity.Y * -0.8f;
                }
                Vector2 value51;
                if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
                {
                    value51 = Main.player[npc.target].Center - npc.Center;
                    value51.Y -= (float)(Main.player[npc.target].height / 4);
                    value51.Normalize();
                    npc.velocity = value51 * 0.1f;
                }
                float scaleFactor24 = CalamityWorld.death ? 2.5f : 2f;
                float num1390 = 20f;
                value51 = npc.velocity;
                value51.Normalize();
                value51 *= scaleFactor24;
                npc.velocity = (npc.velocity * (num1390 - 1f) + value51) / num1390;
                npc.ai[1] += 1f;
                if (npc.ai[1] > 180f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                }
                if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[0] = 0f;
                }
                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 5f && !Collision.SolidCollision(npc.position - new Vector2(10f, 10f), npc.width + 20, npc.height + 20))
                {
                    npc.localAI[0] = 0f;
                    Vector2 center32 = npc.Center;
                    center32.X = Main.player[npc.target].Center.X;
                    if (Collision.CanHit(npc.Center, 1, 1, center32, 1, 1) && Collision.CanHit(npc.Center, 1, 1, center32, 1, 1) && Collision.CanHit(Main.player[npc.target].Center, 1, 1, center32, 1, 1))
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = center32.X;
                        npc.ai[2] = center32.Y;
                        return false;
                    }
                    center32 = npc.Center;
                    center32.Y = Main.player[npc.target].Center.Y;
                    if (Collision.CanHit(npc.Center, 1, 1, center32, 1, 1) && Collision.CanHit(Main.player[npc.target].Center, 1, 1, center32, 1, 1))
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = center32.X;
                        npc.ai[2] = center32.Y;
                    }
                }
            }
            return false;
        }
        #endregion

        #region Star Cell AI
        public static bool BuffedStarCellAI(NPC npc, Mod mod)
        {
            npc.noTileCollide = false;
            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest();
                if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[0] = 1f;
                }
                else
                {
                    Vector2 vector204 = Main.player[npc.target].Center - npc.Center;
                    vector204.Y -= Main.player[npc.target].height / 4;
                    float num1324 = vector204.Length();
                    if (num1324 > 800f)
                    {
                        npc.ai[0] = 2f;
                    }
                    else
                    {
                        Vector2 center31 = npc.Center;
                        center31.X = Main.player[npc.target].Center.X;
                        Vector2 vector205 = center31 - npc.Center;
                        if (vector205.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center31, 1, 1))
                        {
                            npc.ai[0] = 3f;
                            npc.ai[1] = center31.X;
                            npc.ai[2] = center31.Y;
                            Vector2 center32 = npc.Center;
                            center32.Y = Main.player[npc.target].Center.Y;
                            if (vector205.Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center32, 1, 1) && Collision.CanHit(center32, 1, 1, Main.player[npc.target].position, 1, 1))
                            {
                                npc.ai[0] = 3f;
                                npc.ai[1] = center32.X;
                                npc.ai[2] = center32.Y;
                            }
                        }
                        else
                        {
                            center31 = npc.Center;
                            center31.Y = Main.player[npc.target].Center.Y;
                            if ((center31 - npc.Center).Length() > 8f && Collision.CanHit(npc.Center, 1, 1, center31, 1, 1))
                            {
                                npc.ai[0] = 3f;
                                npc.ai[1] = center31.X;
                                npc.ai[2] = center31.Y;
                            }
                        }

                        if (npc.ai[0] == 0f)
                        {
                            npc.localAI[0] = 0f;
                            vector204.Normalize();
                            vector204 *= 0.5f;
                            npc.velocity += vector204;
                            npc.ai[0] = 4f;
                            npc.ai[1] = 0f;
                        }
                    }
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.rotation += npc.direction * 0.3f;
                Vector2 value35 = Main.player[npc.target].Center - npc.Center;
                if (npc.type == NPCID.NebulaHeadcrab)
                    value35 = Main.player[npc.target].Top - npc.Center;

                float num1325 = value35.Length();
                float num1326 = CalamityWorld.death ? 9f : 7.5f;
                num1326 += num1325 / 100f;
                int num1327 = CalamityWorld.death ? 40 : 45;
                value35.Normalize();
                value35 *= num1326;
                npc.velocity = (npc.velocity * (num1327 - 1) + value35) / num1327;
                if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                }

                if (npc.type == NPCID.NebulaHeadcrab && num1325 < 40f && Main.player[npc.target].active && !Main.player[npc.target].dead)
                {
                    bool flag84 = true;
                    for (int num1328 = 0; num1328 < Main.maxNPCs; num1328++)
                    {
                        NPC nPC7 = Main.npc[num1328];
                        if (nPC7.active && nPC7.type == npc.type && nPC7.ai[0] == 5f && nPC7.target == npc.target)
                        {
                            flag84 = false;
                            break;
                        }
                    }

                    if (flag84)
                    {
                        npc.Center = Main.player[npc.target].Top;
                        npc.velocity = Vector2.Zero;
                        npc.ai[0] = 5f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                }
            }
            else if (npc.ai[0] == 2f)
            {
                npc.rotation = npc.velocity.X * 0.1f;
                npc.noTileCollide = true;
                Vector2 value36 = Main.player[npc.target].Center - npc.Center;
                float num1329 = value36.Length();
                float num1330 = CalamityWorld.death ? 6f : 4.5f;
                int num1331 = 2;
                value36.Normalize();
                value36 *= num1330;
                npc.velocity = (npc.velocity * (num1331 - 1) + value36) / num1331;
                if (num1329 < 600f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.ai[0] = 0f;
            }
            else if (npc.ai[0] == 3f)
            {
                npc.rotation = npc.velocity.X * 0.1f;
                Vector2 value37 = new Vector2(npc.ai[1], npc.ai[2]);
                Vector2 value38 = value37 - npc.Center;
                float num1332 = value38.Length();
                float num1333 = CalamityWorld.death ? 4f : 3f;
                float num1334 = 2f;
                value38.Normalize();
                value38 *= num1333;
                npc.velocity = (npc.velocity * (num1334 - 1f) + value38) / num1334;
                if (npc.collideX || npc.collideY)
                {
                    npc.ai[0] = 4f;
                    npc.ai[1] = 0f;
                }

                if (num1332 < num1333 || num1332 > 800f || Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    npc.ai[0] = 0f;
            }
            else if (npc.ai[0] == 4f)
            {
                npc.rotation = npc.velocity.X * 0.1f;
                if (npc.collideX)
                    npc.velocity.X *= -0.8f;

                if (npc.collideY)
                    npc.velocity.Y *= -0.8f;

                Vector2 value39;
                if (npc.velocity.X == 0f && npc.velocity.Y == 0f)
                {
                    value39 = Main.player[npc.target].Center - npc.Center;
                    value39.Y -= Main.player[npc.target].height / 4;
                    value39.Normalize();
                    npc.velocity = value39 * 0.1f;
                }

                float num1335 = CalamityWorld.death ? 4f : 3f;
                float num1336 = CalamityWorld.death ? 16f : 18f;
                value39 = npc.velocity;
                value39.Normalize();
                value39 *= num1335;
                npc.velocity = (npc.velocity * (num1336 - 1f) + value39) / num1336;
                npc.ai[1] += 1f;
                if (npc.ai[1] > 180f)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                }

                if (Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                    npc.ai[0] = 0f;

                npc.localAI[0] += 1f;
                if (npc.localAI[0] >= 5f && !Collision.SolidCollision(npc.position - new Vector2(10f, 10f), npc.width + 20, npc.height + 20))
                {
                    npc.localAI[0] = 0f;
                    Vector2 center33 = npc.Center;
                    center33.X = Main.player[npc.target].Center.X;
                    if (Collision.CanHit(npc.Center, 1, 1, center33, 1, 1) && Collision.CanHit(npc.Center, 1, 1, center33, 1, 1) && Collision.CanHit(Main.player[npc.target].Center, 1, 1, center33, 1, 1))
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = center33.X;
                        npc.ai[2] = center33.Y;
                    }
                    else
                    {
                        center33 = npc.Center;
                        center33.Y = Main.player[npc.target].Center.Y;
                        if (Collision.CanHit(npc.Center, 1, 1, center33, 1, 1) && Collision.CanHit(Main.player[npc.target].Center, 1, 1, center33, 1, 1))
                        {
                            npc.ai[0] = 3f;
                            npc.ai[1] = center33.X;
                            npc.ai[2] = center33.Y;
                        }
                    }
                }
            }
            else if (npc.ai[0] == 5f)
            {
                Player player8 = Main.player[npc.target];
                if (!player8.active || player8.dead)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
                else
                {
                    npc.Center = ((player8.gravDir == 1f) ? player8.Top : player8.Bottom) + new Vector2(player8.direction * 4, 0f);
                    npc.gfxOffY = player8.gfxOffY;
                    npc.velocity = Vector2.Zero;
                    if (!player8.creativeGodMode)
                        player8.AddBuff(BuffID.Obstructed, 59);
                }
            }

            if (npc.type == NPCID.StardustCellBig)
            {
                npc.rotation = 0f;
                for (int num1337 = 0; num1337 < Main.maxNPCs; num1337++)
                {
                    if (num1337 != npc.whoAmI && Main.npc[num1337].active && Main.npc[num1337].type == npc.type && Math.Abs(npc.position.X - Main.npc[num1337].position.X) + Math.Abs(npc.position.Y - Main.npc[num1337].position.Y) < npc.width)
                    {
                        if (npc.position.X < Main.npc[num1337].position.X)
                            npc.velocity.X -= 0.05f;
                        else
                            npc.velocity.X += 0.05f;

                        if (npc.position.Y < Main.npc[num1337].position.Y)
                            npc.velocity.Y -= 0.05f;
                        else
                            npc.velocity.Y += 0.05f;
                    }
                }
            }
            else
            {
                if (npc.type != NPCID.NebulaHeadcrab)
                    return false;

                npc.hide = npc.ai[0] == 5f;
                npc.rotation = npc.velocity.X * 0.1f;
                for (int num1338 = 0; num1338 < Main.maxNPCs; num1338++)
                {
                    if (num1338 != npc.whoAmI && Main.npc[num1338].active && Main.npc[num1338].type == npc.type && Math.Abs(npc.position.X - Main.npc[num1338].position.X) + Math.Abs(npc.position.Y - Main.npc[num1338].position.Y) < npc.width)
                    {
                        if (npc.position.X < Main.npc[num1338].position.X)
                            npc.velocity.X -= 0.05f;
                        else
                            npc.velocity.X += 0.05f;

                        if (npc.position.Y < Main.npc[num1338].position.Y)
                            npc.velocity.Y -= 0.05f;
                        else
                            npc.velocity.Y += 0.05f;
                    }
                }
            }

            return false;
        }
        #endregion

        #region Ancient Vision AI
        public static bool BuffedAncientVisionAI(NPC npc, Mod mod)
        {
            if (npc.alpha > 0)
            {
                npc.alpha -= 30;
                if (npc.alpha < 0)
                    npc.alpha = 0;
            }

            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.knockBackResist = 0f;

            for (int num1339 = 0; num1339 < Main.maxNPCs; num1339++)
            {
                if (num1339 == npc.whoAmI || !Main.npc[num1339].active || Main.npc[num1339].type != npc.type)
                    continue;

                Vector2 vector206 = Main.npc[num1339].Center - npc.Center;
                if (!(vector206.Length() < 50f))
                    continue;

                vector206.Normalize();
                if (vector206.X == 0f && vector206.Y == 0f)
                {
                    if (num1339 > npc.whoAmI)
                        vector206.X = 1f;
                    else
                        vector206.X = -1f;
                }

                vector206 *= 0.4f;
                npc.velocity -= vector206;
                NPC nPC = Main.npc[num1339];
                nPC.velocity += vector206;
            }

            if (npc.type == NPCID.ShadowFlameApparition)
            {
                float num1340 = 120f;
                if (npc.localAI[0] < num1340)
                {
                    if (npc.localAI[0] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                        npc.TargetClosest();
                        if (npc.direction > 0)
                            npc.velocity.X += 2f;
                        else
                            npc.velocity.X -= 2f;

                        npc.position += npc.netOffset;
                        for (int num1341 = 0; num1341 < 20; num1341++)
                        {
                            Vector2 center34 = npc.Center;
                            center34.Y -= 18f;
                            Vector2 vector207 = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                            vector207.Normalize();
                            vector207 *= Main.rand.Next(0, 100) * 0.1f;
                            center34 += vector207;
                            vector207.Normalize();
                            vector207 *= Main.rand.Next(50, 90) * 0.2f;
                            int num1342 = Dust.NewDust(center34, 1, 1, 27);
                            Main.dust[num1342].velocity = -vector207 * 0.3f;
                            Main.dust[num1342].alpha = 100;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[num1342].noGravity = true;
                                Dust dust = Main.dust[num1342];
                                dust.scale += 0.3f;
                            }
                        }

                        npc.position -= npc.netOffset;
                    }

                    npc.localAI[0] += 1f;
                    float num1343 = 1f - npc.localAI[0] / num1340;
                    float num1344 = num1343 * 20f;
                    for (int num1345 = 0; num1345 < num1344; num1345++)
                    {
                        if (Main.rand.NextBool(5))
                        {
                            npc.position += npc.netOffset;
                            int num1346 = Dust.NewDust(npc.position, npc.width, npc.height, 27);
                            Main.dust[num1346].alpha = 100;
                            Dust dust = Main.dust[num1346];
                            dust.velocity *= 0.3f;
                            dust = Main.dust[num1346];
                            dust.velocity += npc.velocity * 0.75f;
                            Main.dust[num1346].noGravity = true;
                            npc.position -= npc.netOffset;
                        }
                    }
                }
            }

            if (npc.type == NPCID.AncientCultistSquidhead)
            {
                float num1347 = 120f;
                if (npc.localAI[0] < num1347)
                {
                    if (npc.localAI[0] == 0f)
                    {
                        SoundEngine.PlaySound(SoundID.Item8, npc.Center);
                        npc.TargetClosest();
                        if (npc.direction > 0)
                            npc.velocity.X += 2f;
                        else
                            npc.velocity.X -= 2f;
                    }

                    npc.localAI[0] += 1f;
                    int num1348 = 10;
                    for (int num1349 = 0; num1349 < 2; num1349++)
                    {
                        npc.position += npc.netOffset;
                        int num1350 = Dust.NewDust(npc.position - new Vector2(num1348), npc.width + num1348 * 2, npc.height + num1348 * 2, 228, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num1350].noGravity = true;
                        Main.dust[num1350].noLight = true;
                        npc.position -= npc.netOffset;
                    }
                }
            }

            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest();
                npc.ai[0] = 1f;
                npc.ai[1] = npc.direction;
            }
            else if (npc.ai[0] == 1f)
            {
                npc.TargetClosest();
                float num1351 = 0.5f;
                float num1352 = 10f;
                float num1353 = 4f;
                float num1354 = 550f;
                float num1355 = 3f;
                if (npc.type == NPCID.AncientCultistSquidhead)
                {
                    num1351 = 0.8f;
                    num1352 = 16f;
                    num1354 = 440f;
                    num1353 = 6f;
                }
                if (CalamityWorld.death)
                {
                    num1351 *= 1.25f;
                    num1352 *= 1.25f;
                    num1354 *= 0.9f;
                    num1355 -= 1f;
                }

                npc.velocity.X += npc.ai[1] * num1351;
                if (npc.velocity.X > num1352)
                    npc.velocity.X = num1352;

                if (npc.velocity.X < 0f - num1352)
                    npc.velocity.X = 0f - num1352;

                float num1356 = Main.player[npc.target].Center.Y - npc.Center.Y;
                if (Math.Abs(num1356) > num1353)
                    num1355 = CalamityWorld.death ? 10f : 12f;

                if (num1356 > num1353)
                    num1356 = num1353;
                else if (num1356 < 0f - num1353)
                    num1356 = 0f - num1353;

                npc.velocity.Y = (npc.velocity.Y * (num1355 - 1f) + num1356) / num1355;
                if ((npc.ai[1] > 0f && Main.player[npc.target].Center.X - npc.Center.X < 0f - num1354) || (npc.ai[1] < 0f && Main.player[npc.target].Center.X - npc.Center.X > num1354))
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    if (npc.Center.Y + 20f > Main.player[npc.target].Center.Y)
                        npc.ai[1] = -1f;
                    else
                        npc.ai[1] = 1f;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                float num1357 = 0.6f;
                float num1358 = 0.93f;
                float num1359 = 7f;
                if (npc.type == NPCID.AncientCultistSquidhead)
                {
                    num1357 = 0.45f;
                    num1359 = 10f;
                    num1358 = 0.87f;
                }
                if (CalamityWorld.death)
                {
                    num1357 *= 1.25f;
                    num1358 *= 0.9f;
                    num1359 *= 1.25f;
                }

                npc.velocity.Y += npc.ai[1] * num1357;
                if (npc.velocity.Length() > num1359)
                    npc.velocity *= num1358;

                if (npc.velocity.X > -1f && npc.velocity.X < 1f)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 3f;
                    npc.ai[1] = npc.direction;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                float num1360 = 0.6f;
                float num1361 = 0.3f;
                float num1362 = 7f;
                float num1363 = 0.93f;
                if (npc.type == NPCID.AncientCultistSquidhead)
                {
                    num1360 = 0.8f;
                    num1361 = 0.45f;
                    num1362 = 9f;
                    num1363 = 0.87f;
                }
                if (CalamityWorld.death)
                {
                    num1360 *= 1.25f;
                    num1361 *= 1.25f;
                    num1362 *= 1.25f;
                    num1363 *= 0.9f;
                }

                npc.velocity.X += npc.ai[1] * num1360;
                if (npc.Center.Y > Main.player[npc.target].Center.Y)
                    npc.velocity.Y -= num1361;
                else
                    npc.velocity.Y += num1361;

                if (npc.velocity.Length() > num1362)
                    npc.velocity *= num1363;

                if (npc.velocity.Y > -1f && npc.velocity.Y < 1f)
                {
                    npc.TargetClosest();
                    npc.ai[0] = 0f;
                    npc.ai[1] = npc.direction;
                }
            }

            if (npc.type == NPCID.AncientCultistSquidhead)
            {
                int num1364 = 10;
                npc.position += npc.netOffset;

                int num1366 = Dust.NewDust(npc.position - new Vector2(num1364), npc.width + num1364 * 2, npc.height + num1364 * 2, 228, 0f, 0f, 100, default(Color), 2f);
                Main.dust[num1366].noGravity = true;
                Main.dust[num1366].noLight = true;

                npc.position -= npc.netOffset;
            }

            return false;
        }
        #endregion

        #region Big Mimic AI
        public static bool BuffedBigMimicAI(NPC npc, Mod mod)
        {
            npc.knockBackResist = 0.2f * Main.GameModeInfo.KnockbackToEnemiesMultiplier;
            npc.dontTakeDamage = false;
            npc.noTileCollide = false;
            npc.noGravity = false;
            npc.reflectsProjectiles = false;
            if (npc.ai[0] != 7f && Main.player[npc.target].dead)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead)
                {
                    npc.ai[0] = 7f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }

            if (npc.ai[0] == 0f)
            {
                npc.TargetClosest();
                Vector2 vector208 = Main.player[npc.target].Center - npc.Center;
                if (Main.netMode != NetmodeID.MultiplayerClient && (npc.velocity.X != 0f || npc.velocity.Y > 100f || npc.justHit || vector208.Length() < 80f))
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                npc.ai[1] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] > 36f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f)
            {
                Vector2 vector209 = Main.player[npc.target].Center - npc.Center;
                if (Main.netMode != NetmodeID.MultiplayerClient && vector209.Length() > 600f)
                {
                    npc.ai[0] = 5f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                if (npc.velocity.Y == 0f)
                {
                    npc.TargetClosest();
                    npc.velocity.X *= 0.85f;
                    npc.ai[1] += 1f;
                    float num1367 = 10f + (CalamityWorld.death ? 10f : 20f) * (npc.life / (float)npc.lifeMax);
                    float num1368 = 5f + (CalamityWorld.death ? 7f : 5f) * (1f - npc.life / (float)npc.lifeMax);
                    float num1369 = CalamityWorld.death ? 7f : 5f;
                    if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                        num1369 += 2f;

                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] > num1367)
                    {
                        npc.ai[3] += 1f;
                        if (npc.ai[3] >= 3f)
                        {
                            npc.ai[3] = 0f;
                            num1369 *= 2f;
                            num1368 /= 2f;
                        }

                        npc.ai[1] = 0f;
                        npc.velocity.Y -= num1369;
                        npc.velocity.X = num1368 * npc.direction;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.knockBackResist = 0f;
                    npc.velocity.X *= 0.99f;
                    if (npc.direction < 0 && npc.velocity.X > -1f)
                        npc.velocity.X = -1f;

                    if (npc.direction > 0 && npc.velocity.X < 1f)
                        npc.velocity.X = 1f;
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] > (CalamityWorld.death ? 130f : 170f) && npc.velocity.Y == 0f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    switch (Main.rand.Next(3))
                    {
                        case 0:
                            npc.ai[0] = 3f;
                            break;
                        case 1:
                            npc.ai[0] = 4f;
                            npc.noTileCollide = true;
                            npc.velocity.Y = CalamityWorld.death ? -12f : -10f;
                            break;
                        case 2:
                            npc.ai[0] = 6f;
                            break;
                        default:
                            npc.ai[0] = 2f;
                            break;
                    }

                    if (Main.tenthAnniversaryWorld && npc.type == NPCID.BigMimicJungle && npc.ai[0] == 3f && Main.rand.NextBool())
                        npc.ai[0] = 8f;

                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f)
            {
                npc.velocity.X *= 0.85f;
                npc.dontTakeDamage = true;
                npc.ai[1] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] >= (CalamityWorld.death ? 60f : 120f))
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.netUpdate = true;
                }

                if (Main.expertMode)
                {
                    npc.ReflectProjectiles(npc.Hitbox);
                    npc.reflectsProjectiles = true;
                }
            }
            else if (npc.ai[0] == 4f)
            {
                npc.noTileCollide = true;
                npc.noGravity = true;
                npc.knockBackResist = 0f;
                if (npc.velocity.X < 0f)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                npc.spriteDirection = npc.direction;
                npc.TargetClosest();
                Vector2 center35 = Main.player[npc.target].Center;
                center35.Y -= 350f;
                Vector2 value40 = center35 - npc.Center;
                if (npc.ai[2] == 1f)
                {
                    npc.ai[1] += 1f;
                    value40 = Main.player[npc.target].Center - npc.Center;
                    value40.Normalize();
                    value40 *= CalamityWorld.death ? 12f : 10f;
                    npc.velocity = (npc.velocity * 4f + value40) / 5f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] > 6f)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[0] = 4.1f;
                        npc.ai[2] = 0f;
                        npc.velocity = value40;
                        npc.netUpdate = true;
                    }
                }
                else if (Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) < 40f && npc.Center.Y < Main.player[npc.target].Center.Y - 300f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[1] = 0f;
                        npc.ai[2] = 1f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    value40.Normalize();
                    value40 *= CalamityWorld.death ? 16f : 14f;
                    npc.velocity = (npc.velocity * 5f + value40) / 6f;
                }
            }
            else if (npc.ai[0] == 4.1f)
            {
                npc.knockBackResist = 0f;
                if (npc.ai[2] == 0f && Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1) && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    npc.ai[2] = 1f;

                if (npc.position.Y + npc.height >= Main.player[npc.target].position.Y || npc.velocity.Y <= 0f)
                {
                    npc.ai[1] += 1f;
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[1] > 10f)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.ai[3] = 0f;
                        npc.netUpdate = true;
                        if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                            npc.ai[0] = 5f;
                    }
                }
                else if (npc.ai[2] == 0f)
                {
                    npc.noTileCollide = true;
                    npc.noGravity = true;
                    npc.knockBackResist = 0f;
                }

                npc.velocity.Y += CalamityWorld.death ? 0.3f : 0.25f;
                if (npc.velocity.Y > (CalamityWorld.death ? 24f : 20f))
                    npc.velocity.Y = CalamityWorld.death ? 24f : 20f;
            }
            else if (npc.ai[0] == 5f)
            {
                if (npc.velocity.X > 0f)
                    npc.direction = 1;
                else
                    npc.direction = -1;

                npc.spriteDirection = npc.direction;
                npc.noTileCollide = true;
                npc.noGravity = true;
                npc.knockBackResist = 0f;
                Vector2 value41 = Main.player[npc.target].Center - npc.Center;
                value41.Y -= 4f;
                if (Main.netMode != NetmodeID.MultiplayerClient && value41.Length() < 200f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }

                if (value41.Length() > 10f)
                {
                    value41.Normalize();
                    value41 *= CalamityWorld.death ? 15f : 12.5f;
                }

                npc.velocity = (npc.velocity * 4f + value41) / 5f;
            }
            else if (npc.ai[0] == 6f)
            {
                npc.knockBackResist = 0f;
                if (npc.velocity.Y == 0f)
                {
                    npc.TargetClosest();
                    npc.velocity.X *= 0.8f;
                    npc.ai[1] += 1f;
                    if (npc.ai[1] > 5f)
                    {
                        npc.ai[1] = 0f;
                        npc.velocity.Y -= 4f;
                        if (Main.player[npc.target].position.Y + Main.player[npc.target].height < npc.Center.Y)
                            npc.velocity.Y -= 1.25f;

                        if (Main.player[npc.target].position.Y + Main.player[npc.target].height < npc.Center.Y - 40f)
                            npc.velocity.Y -= 1.5f;

                        if (Main.player[npc.target].position.Y + Main.player[npc.target].height < npc.Center.Y - 80f)
                            npc.velocity.Y -= 1.75f;

                        if (Main.player[npc.target].position.Y + Main.player[npc.target].height < npc.Center.Y - 120f)
                            npc.velocity.Y -= 2f;

                        if (Main.player[npc.target].position.Y + Main.player[npc.target].height < npc.Center.Y - 160f)
                            npc.velocity.Y -= 2.25f;

                        if (Main.player[npc.target].position.Y + Main.player[npc.target].height < npc.Center.Y - 200f)
                            npc.velocity.Y -= 2.5f;

                        if (!Collision.CanHit(npc.Center, 1, 1, Main.player[npc.target].Center, 1, 1))
                            npc.velocity.Y -= 2f;

                        npc.velocity.X = (CalamityWorld.death ? 16 : 14) * npc.direction;
                        npc.ai[2] += 1f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    npc.velocity.X *= 0.98f;
                    if (npc.direction < 0 && npc.velocity.X > -8f)
                        npc.velocity.X = -8f;

                    if (npc.direction > 0 && npc.velocity.X < 8f)
                        npc.velocity.X = 8f;
                }

                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] >= 3f && npc.velocity.Y == 0f)
                {
                    npc.ai[0] = 2f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 7f)
            {
                npc.damage = 0;
                npc.life = npc.lifeMax;
                npc.defense = 9999;
                npc.noTileCollide = true;
                npc.alpha += 7;
                if (npc.alpha > 255)
                    npc.alpha = 255;

                npc.velocity.X *= 0.98f;
            }
            else
            {
                if (npc.ai[0] != 8f)
                    return false;

                npc.velocity.X *= 0.85f;
                npc.ai[1] += 1f;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (!Main.tenthAnniversaryWorld || npc.ai[1] >= 180f)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.netUpdate = true;
                    }
                    else if (npc.ai[1] % 20f == 0f)
                    {
                        int num = 10;
                        for (int i = 0; i < num; i++)
                        {
                            int itemID = ItemID.Sets.ItemsForStuffCannon[Main.rand.Next(ItemID.Sets.ItemsForStuffCannon.Length)];
                            int item = Item.NewItem(npc.GetSource_Loot(), (int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, itemID, 1, noBroadcast: false, -1, noGrabDelay: true);
                            float num2 = Main.rand.Next(10, 26);
                            Vector2 vector = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
                            Vector2 vector2 = Main.player[npc.target].Center - new Vector2(0f, 120f);
                            float num3 = vector2.X - vector.X;
                            float num4 = vector2.Y - vector.Y;
                            num3 += Main.rand.Next(-50, 51) * 0.1f;
                            num4 += Main.rand.Next(-50, 51) * 0.1f;
                            float num5 = (float)Math.Sqrt(num3 * num3 + num4 * num4);
                            num5 = num2 / num5;
                            num3 *= num5;
                            num4 *= num5;
                            num3 += Main.rand.Next(-50, 51) * 0.1f;
                            num4 += Main.rand.Next(-50, 51) * 0.1f;
                            Main.item[item].velocity.X = num3;
                            Main.item[item].velocity.Y = num4;
                            Main.item[item].noGrabDelay = 100;
                            if (Main.netMode != NetmodeID.SinglePlayer)
                                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, item);
                        }
                    }
                }
            }
            return false;
        }
        #endregion

        #region Small Star Cell AI
        public static bool BuffedSmallStarCellAI(NPC npc, Mod mod)
        {
            float num1470 = CalamityWorld.death ? 100f : 200f;
            if (npc.velocity.Length() > 4f)
                npc.velocity *= 0.95f;

            npc.velocity *= 0.99f;
            npc.ai[0]++;
            float num1471 = MathHelper.Clamp(npc.ai[0] / num1470, 0f, 1f);
            npc.scale = 1f + 0.3f * num1471;
            if (npc.ai[0] >= num1470)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.Transform(NPCID.StardustCellBig);
                    npc.netUpdate = true;
                }

                return false;
            }

            npc.rotation += npc.velocity.X * 0.1f;
            if (!(npc.ai[0] > 20f))
                return false;

            Vector2 center39 = npc.Center;
            int num1472 = (int)(npc.ai[0] / (num1470 / 2f));
            for (int num1473 = 0; num1473 < num1472 + 1; num1473++)
            {
                if (Main.rand.NextBool())
                {
                    int num1474 = 226;
                    float num1475 = 0.4f;
                    if (num1473 % 2 == 1)
                    {
                        num1474 = 226;
                        num1475 = 0.65f;
                    }

                    Vector2 vector222 = center39 + ((float)Main.rand.NextDouble() * ((float)Math.PI * 2f)).ToRotationVector2() * (12f - num1472 * 2);
                    int num1476 = Dust.NewDust(vector222 - Vector2.One * 12f, 24, 24, num1474, npc.velocity.X / 2f, npc.velocity.Y / 2f);
                    Dust dust = Main.dust[num1476];
                    dust.position -= new Vector2(2f);
                    Main.dust[num1476].velocity = Vector2.Normalize(center39 - vector222) * 1.5f * (10f - num1472 * 2f) / 10f;
                    Main.dust[num1476].noGravity = true;
                    Main.dust[num1476].scale = num1475;
                    Main.dust[num1476].customData = npc;
                }
            }

            return false;
        }
        #endregion

        #region Flow Invader AI
        public static bool BuffedFlowInvaderAI(NPC npc, Mod mod)
        {
            float num1477 = CalamityWorld.death ? 8f : 6.5f;
            float moveSpeed = CalamityWorld.death ? 0.25f : 0.2f;
            npc.TargetClosest();
            Vector2 desiredVelocity3 = Main.player[npc.target].Center - npc.Center + new Vector2(0f, -300f);
            float num1478 = desiredVelocity3.Length();
            if (num1478 < 20f)
            {
                desiredVelocity3 = npc.velocity;
            }
            else if (num1478 < 40f)
            {
                desiredVelocity3.Normalize();
                desiredVelocity3 *= num1477 * 0.35f;
            }
            else if (num1478 < 80f)
            {
                desiredVelocity3.Normalize();
                desiredVelocity3 *= num1477 * 0.65f;
            }
            else
            {
                desiredVelocity3.Normalize();
                desiredVelocity3 *= num1477;
            }

            npc.SimpleFlyMovement(desiredVelocity3, moveSpeed);
            npc.rotation = npc.velocity.X * 0.1f;
            if (!((npc.ai[0] += 1f) >= (CalamityWorld.death ? 30f : 50f)))
                return false;

            npc.ai[0] = 0f;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 vector223 = Vector2.Zero;
                while (Math.Abs(vector223.X) < 1.5f)
                    vector223 = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver2) * new Vector2(5f, 3f);

                int proj = Projectile.NewProjectile(npc.GetSource_FromAI(), npc.Center, vector223, ProjectileID.StardustJellyfishSmall, 60, 0f, Main.myPlayer, 0f, npc.whoAmI);
                Main.projectile[proj].extraUpdates += 1;
            }

            return false;
        }
        #endregion

        #endregion
    }
}
