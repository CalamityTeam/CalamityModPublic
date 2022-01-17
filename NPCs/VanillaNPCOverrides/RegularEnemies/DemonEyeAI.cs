using CalamityMod.NPCs.AcidRain;
using CalamityMod.NPCs.Crags;
using CalamityMod.NPCs.NormalNPCs;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.RegularEnemies
{
    public static class DemonEyeAI
    {
        // Subtypes of enemies with this AI. Made for programmer convenience.
        public static List<int> NightTimeEnemies => new List<int>()
        {
            NPCID.DemonEye,
            NPCID.WanderingEye,
            NPCID.CataractEye,
            NPCID.SleepyEye,
            NPCID.DialatedEye,
            NPCID.GreenEye,
            NPCID.PurpleEye,
            NPCID.DemonEyeOwl,
            NPCID.DemonEyeSpaceship,
            ModContent.NPCType<BlightedEye>()
        };

        public static List<int> Pigrons => new List<int>()
        {
            NPCID.PigronCorruption,
            NPCID.PigronCrimson,
            NPCID.PigronHallow
        };

        public const int FadeThroughWallsDelay = 300;

        public static void DemonEyeBatMovement(NPC npc, float maxXSpeed = 6f, float maxYSpeed = 3.5f,
            float xAccel = 0.1f, float xAccelBoost1 = 0.06f, float xAccelBoost2 = 0.25f,
            float yAccel = 0.12f, float yAccelBoost1 = 0.07f, float yAccelBoost2 = 0.2f)
        {
            if (npc.direction == -1 && npc.velocity.X > -maxXSpeed)
            {
                npc.velocity.X -= xAccel;
                if (npc.velocity.X > maxXSpeed)
                {
                    npc.velocity.X -= xAccelBoost1;
                }
                else if (npc.velocity.X > 0f)
                {
                    npc.velocity.X -= xAccelBoost2;
                }
                if (npc.velocity.X < -maxXSpeed)
                {
                    npc.velocity.X = -maxXSpeed;
                }
            }
            else if (npc.direction == 1 && npc.velocity.X < maxXSpeed)
            {
                npc.velocity.X += xAccel;
                if (npc.velocity.X < -maxXSpeed)
                {
                    npc.velocity.X += xAccelBoost1;
                }
                else if (npc.velocity.X < 0f)
                {
                    npc.velocity.X += xAccelBoost2;
                }
                if (npc.velocity.X > maxXSpeed)
                {
                    npc.velocity.X = maxXSpeed;
                }
            }
            if (npc.directionY == -1 && npc.velocity.Y > -maxYSpeed)
            {
                npc.velocity.Y -= yAccel;
                if (npc.velocity.Y > maxYSpeed)
                {
                    npc.velocity.Y -= yAccelBoost1;
                }
                else if (npc.velocity.Y > 0f)
                {
                    npc.velocity.Y -= yAccelBoost2;
                }
                if (npc.velocity.Y < -maxYSpeed)
                {
                    npc.velocity.Y = -maxYSpeed;
                }
            }
            else if (npc.directionY == 1 && npc.velocity.Y < maxYSpeed)
            {
                npc.velocity.Y += yAccel;
                if (npc.velocity.Y < -maxYSpeed)
                {
                    npc.velocity.Y += yAccelBoost1;
                }
                else if (npc.velocity.Y < 0f)
                {
                    npc.velocity.Y += yAccelBoost2;
                }
                if (npc.velocity.Y > maxYSpeed)
                {
                    npc.velocity.Y = maxYSpeed;
                }
            }
        }

        public static bool BuffedDemonEyeAI(NPC npc, Mod mod)
        {
            // Randomly play pigron noises if the NPC is a pigron.
            if (Pigrons.Contains(npc.type) && Main.rand.NextBool(1000))
                Main.PlaySound(SoundID.Zombie, (int)npc.position.X, (int)npc.position.Y, 9, 1f, 0f);

            // Disable gravity.
            npc.noGravity = true;

            // Handle tile collision rebounds if applicable.
            if (!npc.noTileCollide)
            {
                // Bounce off of tiles on the X axis.
                if (npc.collideX)
                {
                    npc.velocity.X = npc.oldVelocity.X * -0.5f;
                    if (npc.direction == -1 && npc.velocity.X > 0f && npc.velocity.X < 2f)
                        npc.velocity.X = 2f;

                    if (npc.direction == 1 && npc.velocity.X < 0f && npc.velocity.X > -2f)
                    {
                        npc.velocity.X = -2f;
                    }
                }
                // Bounce off of tiles on the Y axis.
                if (npc.collideY)
                {
                    npc.velocity.Y = npc.oldVelocity.Y * -0.5f;
                    if (npc.velocity.Y > 0f && npc.velocity.Y < 1f)
                        npc.velocity.Y = 1f;
                    if (npc.velocity.Y < 0f && npc.velocity.Y > -1f)
                        npc.velocity.Y = -1f;
                }
            }

            // If the NPC is supposed to only appear during the night, disappear if above-ground and it's daytime.
            if (Main.dayTime && npc.position.Y <= Main.worldSurface * 16.0 && NightTimeEnemies.Contains(npc.type))
            {
                if (npc.timeLeft > 10)
                    npc.timeLeft = 10;

                // Adjust directions
                npc.direction = (npc.velocity.X > 0f).ToDirectionInt();
                npc.directionY = (npc.velocity.X > 0f).ToDirectionInt();
            }

            // Otherwise constantly search for the closest target.
            else
                npc.TargetClosest();

            Player target = Main.player[npc.target];
            ref float fadeThroughWallsTimer = ref npc.ai[0];
            ref float fadeThroughWallsFlag = ref npc.ai[1];

            // Fade away and go through walls if the NPC is a pigron or Calamity eye.
            if (Pigrons.Contains(npc.type) || npc.type == ModContent.NPCType<CalamityEye>() || npc.type == ModContent.NPCType<BlightedEye>())
            {
                // Stop going through walls if the target can be reached.
                if (Collision.CanHit(npc.position, npc.width, npc.height, target.position, target.width, target.height))
                {
                    if (fadeThroughWallsFlag != 0f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        fadeThroughWallsFlag = 0f;
                        fadeThroughWallsTimer = 0f;
                        npc.netUpdate = true;
                    }
                }
                else if (fadeThroughWallsFlag == 0f)
                    fadeThroughWallsTimer++;

                if (fadeThroughWallsTimer >= FadeThroughWallsDelay)
                {
                    fadeThroughWallsFlag = 1f;
                    fadeThroughWallsTimer = 0f;
                    npc.netUpdate = true;
                }
                if (fadeThroughWallsFlag == 0f)
                {
                    npc.alpha = 0;
                    npc.noTileCollide = false;
                }
                else
                {
                    npc.wet = false;
                    npc.alpha = 200;
                    npc.noTileCollide = true;
                }
                npc.rotation = npc.velocity.Y * 0.1f * npc.direction;
                npc.TargetClosest(true);

                DemonEyeBatMovement(npc);
            }
            else if (npc.type == NPCID.TheHungryII)
            {
                npc.TargetClosest(true);

                // Emit light for some reason.
                Lighting.AddLight((int)npc.Center.X / 16, (int)npc.Center.Y / 16, 0.3f, 0.2f, 0.1f);

                DemonEyeBatMovement(npc, 8f, 3.5f, 0.12f, 0.12f, 0.25f, 0.06f, 0.07f, 0.2f);
                if (Main.rand.NextBool(40))
                {
                    Vector2 dustSpawnTopLeft = new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f);
                    Dust blood = Dust.NewDustDirect(dustSpawnTopLeft, npc.width, npc.height / 2, DustID.Blood, npc.velocity.X, 2f, 0, default, 1f);
                    blood.velocity.X *= 0.5f;
                    blood.velocity.Y *= 0.1f;
                }
            }
            else if (npc.type == NPCID.WanderingEye)
            {
                // Move faster when close to dying.
                if (npc.life < npc.lifeMax * 0.5)
                    DemonEyeBatMovement(npc, 8f, 6f, 0.12f, 0.12f, 0.07f, 0.12f, 0.12f, 0.07f);
                else
                    DemonEyeBatMovement(npc, 6f, 2.5f, 0.12f, 0.12f, 0.07f, 0.06f, 0.07f, 0.05f);
            }
            else
            {
                float maxSpeedX = 6f;
                float maxSpeedY = 2.5f;
                maxSpeedX *= 1f + (1f - npc.scale);
                maxSpeedY *= 1f + (1f - npc.scale);
                DemonEyeBatMovement(npc, maxSpeedX, maxSpeedY, 0.08f, 0.08f, 0.03f, 0.02f, 0.03f, 0.015f);
            }

            // Make actual eyes emit blood randomly
            if (npc.type == NPCID.DemonEye ||
                 npc.type == NPCID.WanderingEye ||
                 npc.type == ModContent.NPCType<CalamityEye>() ||
                 npc.type == ModContent.NPCType<BlightedEye>() ||
                 (npc.type >= NPCID.CataractEye && npc.type <= NPCID.PurpleEye))
            {
                if (Main.rand.NextBool(40))
                {
                    Vector2 dustSpawnTopLeft = new Vector2(npc.position.X, npc.position.Y + npc.height * 0.25f);
                    Dust blood = Dust.NewDustDirect(dustSpawnTopLeft, npc.width, npc.height / 2, DustID.Blood, npc.velocity.X, 2f, 0, default, 1f);
                    blood.velocity.X *= 0.5f;
                    blood.velocity.Y *= 0.1f;
                }
            }

            // Avoid entering water. This does not apply to pigrons.
            if (npc.wet && !Pigrons.Contains(npc.type))
            {
                if (npc.velocity.Y > 0f)
                    npc.velocity.Y *= 0.95f;

                npc.velocity.Y -= 0.7f;
                if (npc.velocity.Y < -6f)
                    npc.velocity.Y = -6f;

                npc.TargetClosest(true);
            }
            return false;
        }
    }
}
