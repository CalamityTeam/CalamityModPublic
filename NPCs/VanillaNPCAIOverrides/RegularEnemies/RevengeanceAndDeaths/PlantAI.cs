using System;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;

public static partial class RevengeanceAndDeathAI
{
    public class PlantAI : VanillaAIOverride
    {
        public override bool AI(Mod mod)
        {
            if (NPC.ai[0] < 0f || NPC.ai[0] >= (float)Main.maxTilesX || NPC.ai[1] < 0f || NPC.ai[1] >= (float)Main.maxTilesX)
                return false;

            if (!Main.tile[(int)NPC.ai[0], (int)NPC.ai[1]].HasTile)
            {
                NPC.life = -1;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return false;
            }

            FixExploitManEaters.ProtectSpot((int)NPC.ai[0], (int)NPC.ai[1]);

            NPC.TargetClosest();

            float acceleration = 0.035f;
            float minDistance = 250f;
            switch (NPC.type)
            {
                case NPCID.ManEater:
                    minDistance = 350f;
                    break;
                case NPCID.Clinger:
                    minDistance = 225f;
                    break;
                case NPCID.FungiBulb:
                    minDistance = !CalamityWorld.revenge ? 100f : 200f;
                    break;
                case NPCID.AngryTrapper:
                    acceleration = 0.05f;
                    minDistance = 500f;
                    break;
                case NPCID.GiantFungiBulb:
                    acceleration = 0.15f;
                    minDistance = CalamityWorld.revenge ? 450f : 350f;
                    break;
            }

            if (CalamityWorld.death)
            {
                acceleration *= 1.25f;
                minDistance *= 1.25f;
            }

            float maxVelocity = 2f +
                (NPC.type == NPCID.ManEater ? 1f : 0f) +
                (NPC.type == NPCID.AngryTrapper ? 2f : 0f);

            NPC.ai[2] += 1f;
            if (NPC.ai[2] > 300f)
            {
                minDistance *= 1.3f;

                if (CalamityWorld.revenge)
                    maxVelocity += 2f;

                if (NPC.ai[2] > 450f)
                    NPC.ai[2] = 0f;
            }

            Vector2 anchorPosition = new Vector2(NPC.ai[0] * 16f + 8f, NPC.ai[1] * 16f + 8f);
            Vector2 distanceVector = Main.player[NPC.target].Center - anchorPosition;
            float distanceMagnitude = distanceVector.Length();
            if (distanceMagnitude > minDistance)
            {
                float normalizedMagnitude = minDistance / distanceMagnitude;
                distanceVector *= normalizedMagnitude;
            }

            if (NPC.position.X < NPC.ai[0] * 16f + 8f + distanceVector.X)
            {
                NPC.velocity.X += acceleration;
                if (NPC.velocity.X < 0f && distanceVector.X > 0f)
                    NPC.velocity.X += acceleration * 1.5f;
            }
            else if (NPC.position.X > NPC.ai[0] * 16f + 8f + distanceVector.X)
            {
                NPC.velocity.X -= acceleration;
                if (NPC.velocity.X > 0f && distanceVector.X < 0f)
                    NPC.velocity.X -= acceleration * 1.5f;
            }
            if (NPC.position.Y < NPC.ai[1] * 16f + 8f + distanceVector.Y)
            {
                NPC.velocity.Y += acceleration;
                if (NPC.velocity.Y < 0f && distanceVector.Y > 0f)
                    NPC.velocity.Y += acceleration * 1.5f;
            }
            else if (NPC.position.Y > NPC.ai[1] * 16f + 8f + distanceVector.Y)
            {
                NPC.velocity.Y -= acceleration;
                if (NPC.velocity.Y > 0f && distanceVector.Y < 0f)
                    NPC.velocity.Y -= acceleration * 1.5f;
            }

            NPC.velocity = Vector2.Clamp(NPC.velocity, new Vector2(-maxVelocity), new Vector2(maxVelocity));

            if (NPC.type == NPCID.FungiBulb || NPC.type == NPCID.GiantFungiBulb)
            {
                NPC.rotation = NPC.AngleTo(Main.player[NPC.target].Center) + MathHelper.PiOver2;
            }
            else
            {
                NPC.spriteDirection = (distanceVector.X > 0f).ToDirectionInt();
                NPC.rotation = NPC.AngleTo(Main.player[NPC.target].Center) + (distanceVector.X < 0f).ToInt() * MathHelper.Pi;
            }

            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -0.7f;
                if (NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                    NPC.velocity.X = 2f;
                if (NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                    NPC.velocity.X = -2f;
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -0.7f;
                if (NPC.velocity.Y > 0f && NPC.velocity.Y < 2f)
                    NPC.velocity.Y = 2f;
                if (NPC.velocity.Y < 0f && NPC.velocity.Y > -2f)
                    NPC.velocity.Y = -2f;
            }

            if (NPC.type == NPCID.GiantFungiBulb && !Main.player[NPC.target].DeadOrGhost)
            {
                if (NPC.localAI[0] > ((NPC.type == NPCID.GiantFungiBulb ? (CalamityWorld.revenge ? GiantFungiBulbSporeShootGateValue_Rev : GiantFungiBulbSporeShootGateValue) : FungiBulbSporeShootGateValue) - FungiBulbSporeTelegraphTime))
                {
                    Vector2 dustCenter = NPC.Center + NPC.SafeDirectionTo(Main.player[NPC.target].Center, -Vector2.UnitY) * (NPC.type == NPCID.GiantFungiBulb ? 20f : 12f) + Main.rand.NextVector2CircularEdge(5f, 5f);
                    Dust dust = Dust.NewDustDirect(dustCenter, 1, 1, DustID.BlueTorch, 0f, 0f, 100, default, 3f);
                    dust.noGravity = true;
                    dust.velocity *= 0f;
                }

                if (NPC.localAI[0] == (NPC.type == NPCID.GiantFungiBulb ? (CalamityWorld.revenge ? GiantFungiBulbSporeShootGateValue_Rev : GiantFungiBulbSporeShootGateValue) : FungiBulbSporeShootGateValue) - 1)
                    SoundEngine.PlaySound(SoundID.Item17, NPC.Center);
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.type == NPCID.Clinger && !Main.player[NPC.target].DeadOrGhost)
                {
                    if (NPC.justHit)
                        NPC.localAI[0] = 0f;

                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= ClingerShootGateValue_Rev)
                    {
                        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                        {
                            int damage = 17;
                            int type = ProjectileID.CursedFlameHostile;
                            Vector2 flameVelocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center, -Vector2.UnitY) * 12f;

                            int flame = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, flameVelocity, type, damage, 0f, Main.myPlayer);
                            Main.projectile[flame].timeLeft = 180;
                            NPC.localAI[0] = 0f;
                        }
                        else
                            NPC.localAI[0] = ClingerShootGateValue_Rev - 15f;
                    }
                }

                if ((NPC.type == NPCID.GiantFungiBulb || NPC.type == NPCID.FungiBulb) && !Main.player[NPC.target].DeadOrGhost)
                {
                    if (NPC.justHit || Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || !Collision.CanHit(NPC, Main.player[NPC.target]))
                        NPC.localAI[0] = 0f;

                    NPC.localAI[0] += 1f;
                    float sporeSpawnGateValue = NPC.type == NPCID.GiantFungiBulb ? (CalamityWorld.revenge ? GiantFungiBulbSporeShootGateValue_Rev : GiantFungiBulbSporeShootGateValue) : FungiBulbSporeShootGateValue;
                    if (NPC.localAI[0] >= sporeSpawnGateValue)
                    {
                        if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && Collision.CanHit(NPC, Main.player[NPC.target]))
                        {
                            float speed = NPC.type == NPCID.GiantFungiBulb ? (CalamityWorld.revenge ? 16f : 14f) : 8f;
                            distanceVector.X = Main.player[NPC.target].Center.X - NPC.Center.X;
                            float absoluteYDistance = Math.Abs(distanceVector.X * 0.1f);
                            if (Main.player[NPC.target].Center.Y - NPC.Center.Y > 0f)
                                absoluteYDistance = 0f;

                            Vector2 velocity = NPC.SafeDirectionTo(Main.player[NPC.target].Center - NPC.Center - Vector2.UnitY * absoluteYDistance, -Vector2.UnitY) * speed;

                            int idx = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.FungiSpore);
                            Main.npc[idx].velocity = velocity;
                            Main.npc[idx].netUpdate = true;
                        }

                        NPC.localAI[0] = 0f;
                    }
                }
            }
            return false;
        }
    }
}
