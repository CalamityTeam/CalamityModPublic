using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SCalWormHead : ModNPC
    {
        private const int minLength = 51;
        private const int maxLength = 52;
        private float passedVar = 0f;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sepulcher");
        }

        public override void SetDefaults()
        {
            npc.damage = 0; //150
            npc.npcSlots = 5f;
            npc.width = 62; //324
            npc.height = 64; //216
            npc.defense = 0;
            CalamityGlobalNPC global = npc.Calamity();
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            npc.lifeMax = CalamityWorld.revenge ? 345000 : 300000;
            npc.aiStyle = -1; //new
            aiType = -1; //new
            npc.knockBackResist = 0f;
            npc.scale = Main.expertMode ? 1.35f : 1.2f;
            npc.scale *= 1.25f;

            npc.alpha = 255;
            npc.chaseable = false;
            npc.behindTiles = true;
            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.canGhostHeal = false;
            npc.netAlways = true;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(npc.localAI[0]);
            writer.Write(npc.alpha);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            npc.localAI[0] = reader.ReadSingle();
            npc.alpha = reader.ReadInt32();
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            CalamityGlobalNPC.SCalWorm = npc.whoAmI;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            if (npc.ai[2] > 0f)
                npc.realLife = (int)npc.ai[2];

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned && npc.ai[0] == 0f)
                {
                    float rotationalOffset = 0f;
                    int Previous = npc.whoAmI;
                    for (int i = 0; i < maxLength; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength && i % 2 == 1)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<SCalWormBodyWeak>(), npc.whoAmI);
                            Main.npc[lol].localAI[0] += passedVar;
                            passedVar += 36f;
                        }
                        else if (i >= 0 && i < minLength)
                        {
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<SCalWormBody>(), npc.whoAmI);
                            Main.npc[lol].localAI[3] = i;
                        }
                        else
                            lol = NPC.NewNPC((int)npc.position.X + (npc.width / 2), (int)npc.position.Y + (npc.height / 2), ModContent.NPCType<SCalWormTail>(), npc.whoAmI);
                        
                        // Create arms.
                        if (i >= 3 && i % 4 == 0)
                        {
                            NPC segment = Main.npc[lol];
                            int arm = NPC.NewNPC((int)segment.Center.X, (int)segment.Center.Y, ModContent.NPCType<SCalWormArm>(), lol);
                            if (Main.npc.IndexInRange(arm))
                            {
                                Main.npc[arm].ai[0] = lol;
                                Main.npc[arm].direction = 1;
                                Main.npc[arm].rotation = rotationalOffset;
                            }

                            rotationalOffset += MathHelper.Pi / 6f;

                            arm = NPC.NewNPC((int)segment.Center.X, (int)segment.Center.Y, ModContent.NPCType<SCalWormArm>(), lol);
                            if (Main.npc.IndexInRange(arm))
                            {
                                Main.npc[arm].ai[0] = lol;
                                Main.npc[arm].direction = -1;
                                Main.npc[arm].rotation = rotationalOffset + MathHelper.Pi;
                            }

                            rotationalOffset += MathHelper.Pi / 6f;
                            rotationalOffset = MathHelper.WrapAngle(rotationalOffset);
                        }

                        Main.npc[lol].realLife = npc.whoAmI;
                        Main.npc[lol].ai[2] = npc.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
                if (!npc.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, npc.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }

            if (Main.player[npc.target].dead || !NPC.AnyNPCs(ModContent.NPCType<BrimstoneHeart>()) || CalamityGlobalNPC.SCal < 0 || !Main.npc[CalamityGlobalNPC.SCal].active)
            {
                npc.TargetClosest(false);
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.NPCKilled, "Sounds/NPCKilled/SepulcherDeath"), Main.player[npc.target].Center);
                npc.life = 0;
                npc.HitEffect();
                npc.active = false;
                npc.netUpdate = true;
            }
            else
                npc.Opacity = MathHelper.Clamp(npc.Opacity + 0.165f, 0f, 1f);

            Vector2 vector18 = new Vector2(npc.position.X + npc.width * 0.5f, npc.position.Y + npc.height * 0.5f);
            float num191 = Main.player[npc.target].position.X + (Main.player[npc.target].width / 2);
            float num192 = Main.player[npc.target].position.Y + (Main.player[npc.target].height / 2);
            float num188 = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 12.5f : 10f;
            float num189 = (CalamityWorld.malice || BossRushEvent.BossRushActive) ? 0.125f : 0.1f;

            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = npc.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    npc.velocity.Normalize();
                    npc.velocity *= num49;
                }
            }

            num191 = (int)(num191 / 16f) * 16;
            num192 = (int)(num192 / 16f) * 16;
            vector18.X = (int)(vector18.X / 16f) * 16;
            vector18.Y = (int)(vector18.Y / 16f) * 16;
            num191 -= vector18.X;
            num192 -= vector18.Y;
            float num193 = (float)Math.Sqrt(num191 * num191 + num192 * num192);
            float num196 = Math.Abs(num191);
            float num197 = Math.Abs(num192);
            float num198 = num188 / num193;
            num191 *= num198;
            num192 *= num198;
            if ((npc.velocity.X > 0f && num191 > 0f) || (npc.velocity.X < 0f && num191 < 0f) || (npc.velocity.Y > 0f && num192 > 0f) || (npc.velocity.Y < 0f && num192 < 0f))
            {
                if (npc.velocity.X < num191)
                {
                    npc.velocity.X = npc.velocity.X + num189;
                }
                else
                {
                    if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189;
                    }
                }
                if (npc.velocity.Y < num192)
                {
                    npc.velocity.Y = npc.velocity.Y + num189;
                }
                else
                {
                    if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189;
                    }
                }
                if (Math.Abs(num192) < num188 * 0.2 && ((npc.velocity.X > 0f && num191 < 0f) || (npc.velocity.X < 0f && num191 > 0f)))
                {
                    if (npc.velocity.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 2f;
                    }
                    else
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 2f;
                    }
                }
                if (Math.Abs(num191) < num188 * 0.2 && ((npc.velocity.Y > 0f && num192 < 0f) || (npc.velocity.Y < 0f && num192 > 0f)))
                {
                    if (npc.velocity.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 2f; //changed from 2
                    }
                    else
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 2f; //changed from 2
                    }
                }
            }
            else
            {
                if (num196 > num197)
                {
                    if (npc.velocity.X < num191)
                    {
                        npc.velocity.X = npc.velocity.X + num189 * 1.1f; //changed from 1.1
                    }
                    else if (npc.velocity.X > num191)
                    {
                        npc.velocity.X = npc.velocity.X - num189 * 1.1f; //changed from 1.1
                    }
                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num189;
                        }
                        else
                        {
                            npc.velocity.Y = npc.velocity.Y - num189;
                        }
                    }
                }
                else
                {
                    if (npc.velocity.Y < num192)
                    {
                        npc.velocity.Y = npc.velocity.Y + num189 * 1.1f;
                    }
                    else if (npc.velocity.Y > num192)
                    {
                        npc.velocity.Y = npc.velocity.Y - num189 * 1.1f;
                    }
                    if ((Math.Abs(npc.velocity.X) + Math.Abs(npc.velocity.Y)) < num188 * 0.5)
                    {
                        if (npc.velocity.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num189;
                        }
                        else
                        {
                            npc.velocity.X = npc.velocity.X - num189;
                        }
                    }
                }
            }
            npc.rotation = (float)Math.Atan2(npc.velocity.Y, npc.velocity.X) + MathHelper.PiOver2;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = Main.npcTexture[npc.type];
            Vector2 vector11 = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / 2));

            Vector2 vector43 = npc.Center - Main.screenPosition;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * npc.scale / 2f;
            vector43 += vector11 * npc.scale + new Vector2(0f, npc.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, npc.frame, npc.GetAlpha(lightColor), npc.rotation, vector11, npc.scale, spriteEffects, 0f);

            return false;
        }

        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (CalamityLists.projectileDestroyExceptionList.TrueForAll(x => projectile.type != x))
            {
                if (projectile.penetrate == -1 && !projectile.minion)
                {
                    projectile.penetrate = 1;
                }
                else if (projectile.penetrate >= 1)
                {
                    projectile.penetrate = 1;
                }
            }
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            if (npc.life <= 0)
            {
                for (int i = 1; i <= 3; i++)
                {
                    Vector2 goreSpawnPosition = npc.Center;

                    // Spawn at a slight offset when spawning mandibles.
                    if (i == 2)
                        goreSpawnPosition += npc.velocity.SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver4) * 16f;
                    if (i == 3)
                        goreSpawnPosition += npc.velocity.SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver4) * 16f;
                    Gore.NewGorePerfect(goreSpawnPosition, npc.velocity, mod.GetGoreSlot($"Gores/SupremeCalamitas/SepulcherHead_Gore{i}"), npc.scale);
                }
            }
        }
    }
}
