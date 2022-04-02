using CalamityMod.Events;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.VanillaNPCOverrides.Bosses
{
    public static class KingSlimeAI
    {
        // Master Mode changes
        /* 1 - Rainbow colored
           2 - Spawns a rainbow slime every 5% HP
           3 - Teleports ahead of his target
           4 - Can use a jump and slam like a Big Mimic*/
        public static bool BuffedKingSlimeAI(NPC npc, Mod mod)
        {
            // Variables
            float num234 = 1f;
            bool teleporting = false;
            bool flag9 = false;
            npc.aiAction = 0;

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            npc.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            // Get a target
            if (npc.target < 0 || npc.target == Main.maxPlayers || Main.player[npc.target].dead || !Main.player[npc.target].active)
                npc.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                npc.TargetClosest();

            // Percent life remaining
            float lifeRatio = npc.life / (float)npc.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;

            // Spawn crystal in phase 2
            if (phase3 && npc.Calamity().newAI[0] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.Calamity().newAI[0] = 1f;
                npc.SyncExtraAI();
                Vector2 vector = npc.Center + new Vector2(-40f, -(float)npc.height / 2);
                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(vector, npc.width / 2, npc.height / 2, 90, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 2f;
                    Main.dust[num622].noGravity = true;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                Main.PlaySound(SoundID.Item38, (int)npc.position.X, (int)npc.position.Y);
                NPC.NewNPC((int)vector.X, (int)vector.Y, ModContent.NPCType<KingSlimeJewel>());
            }

            // Set up health value for spawning slimes
            if (npc.ai[3] == 0f && npc.life > 0)
                npc.ai[3] = npc.lifeMax;

            // Spawn with attack delay
            if (npc.localAI[3] == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.ai[0] = -100f;
                npc.localAI[3] = 1f;
                npc.netUpdate = true;
            }

            // Despawn
            int despawnDistance = 500;
            if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > despawnDistance)
            {
                npc.TargetClosest();
                if (Main.player[npc.target].dead || Math.Abs(npc.Center.X - Main.player[npc.target].Center.X) / 16f > despawnDistance)
                {
                    if (npc.timeLeft > 10)
                        npc.timeLeft = 10;

                    //EncourageDespawn(10);

                    if (Main.player[npc.target].Center.X < npc.Center.X)
                        npc.direction = 1;
                    else
                        npc.direction = -1;
                }
            }

            // Faster fall
            if (npc.velocity.Y > 0f)
                npc.velocity.Y += malice ? 0.1f : death ? 0.05f : 0f;

            // Activate teleport
            if (!Main.player[npc.target].dead && npc.ai[2] >= 300f && npc.ai[1] < 5f && npc.velocity.Y == 0f)
            {
                npc.ai[2] = 0f;
                npc.ai[0] = 0f;
                npc.ai[1] = 5f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    GetPlaceToTeleportTo(npc);
            }

            if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0) || Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 160f)
            {
                npc.ai[2] += 1f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                    npc.localAI[0] += 1f;
            }
            else if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                npc.localAI[0] -= 1f;

                if (npc.localAI[0] < 0f)
                    npc.localAI[0] = 0f;
            }

            if (npc.timeLeft < 10 && (npc.ai[0] != 0f || npc.ai[1] != 0f))
            {
                npc.ai[0] = 0f;
                npc.ai[1] = 0f;
                npc.netUpdate = true;
                teleporting = false;
            }

            // Get closer to activating teleport
            if (!Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                npc.ai[2] += death ? 2f : 1f;
            if (Math.Abs(npc.Top.Y - Main.player[npc.target].Bottom.Y) > 320f)
                npc.ai[2] += death ? 2f : 1f;

            // Dust variable
            Dust dust;

            // Teleport
            if (npc.ai[1] == 5f)
            {
                teleporting = true;
                npc.aiAction = 1;
                npc.ai[0] += 1f;
                num234 = MathHelper.Clamp((60f - npc.ai[0]) / 60f, 0f, 1f);
                num234 = 0.5f + num234 * 0.5f;

                if (npc.ai[0] >= 60f)
                    flag9 = true;

                if (npc.ai[0] == 60f)
                    Gore.NewGore(npc.Center + new Vector2(-40f, -(float)npc.height / 2), npc.velocity, 734, 1f);

                if (npc.ai[0] >= 60f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.Bottom = new Vector2(npc.localAI[1], npc.localAI[2]);
                    npc.ai[1] = 6f;
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[0] >= 120f)
                {
                    npc.ai[1] = 6f;
                    npc.ai[0] = 0f;
                }

                if (!flag9)
                {
                    for (int num245 = 0; num245 < 10; num245++)
                    {
                        int num246 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
                        Main.dust[num246].noGravity = true;
                        dust = Main.dust[num246];
                        dust.velocity *= 0.5f;
                    }
                }
            }

            // Post-teleport
            else if (npc.ai[1] == 6f)
            {
                teleporting = true;
                npc.aiAction = 0;
                npc.ai[0] += 1f;
                num234 = MathHelper.Clamp(npc.ai[0] / 30f, 0f, 1f);
                num234 = 0.5f + num234 * 0.5f;

                if (npc.ai[0] >= 30f && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                    npc.netUpdate = true;
                    npc.TargetClosest();
                }

                if (Main.netMode == NetmodeID.MultiplayerClient && npc.ai[0] >= 60f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 0f;
                    npc.TargetClosest();
                }

                for (int num247 = 0; num247 < 10; num247++)
                {
                    int num248 = Dust.NewDust(npc.position + Vector2.UnitX * -20f, npc.width + 40, npc.height, 4, npc.velocity.X, npc.velocity.Y, 150, new Color(78, 136, 255, 80), 2f);
                    Main.dust[num248].noGravity = true;
                    dust = Main.dust[num248];
                    dust.velocity *= 2f;
                }
            }

            // Don't take damage while teleporting
            npc.dontTakeDamage = npc.hide = flag9;

            // Jump
            if (npc.velocity.Y == 0f)
            {
                npc.velocity.X *= 0.8f;
                if (npc.velocity.X > -0.1f && npc.velocity.X < 0.1f)
                    npc.velocity.X = 0f;

                if (!teleporting)
                {
                    npc.ai[0] += 2f;
                    if (malice)
                        npc.ai[0] += 9f;
                    else
                    {
                        if (lifeRatio < 0.8f)
                            npc.ai[0] += 1f;
                        if (lifeRatio < 0.6f)
                            npc.ai[0] += 1f;
                        if (lifeRatio < 0.4f)
                            npc.ai[0] += 2f;
                    }
                    if (lifeRatio < 0.2f)
                        npc.ai[0] += 3f;
                    if (lifeRatio < 0.1f)
                        npc.ai[0] += 4f;

                    if (npc.ai[0] >= 0f)
                    {
                        npc.netUpdate = true;
                        npc.TargetClosest();

                        float distanceBelowTarget = npc.position.Y - (Main.player[npc.target].position.Y + 80f);
                        float speedMult = 1f;
                        if (distanceBelowTarget > 0f)
                            speedMult += distanceBelowTarget * 0.002f;

                        if (speedMult > 2f)
                            speedMult = 2f;

                        // Jump type
                        float jumpSpeedMult = 1.5f;
                        if (npc.ai[1] == 3f)
                        {
                            npc.velocity.Y = -13f * speedMult;
                            npc.velocity.X += (phase2 ? (death ? 5.5f : 4.5f) : 3.5f) * npc.direction;
                            if (malice)
                                npc.velocity.X *= jumpSpeedMult;

                            npc.ai[0] = -200f;
                            npc.ai[1] = 0f;
                        }
                        else if (npc.ai[1] == 2f)
                        {
                            npc.velocity.Y = -6f * speedMult;
                            npc.velocity.X += (phase2 ? (death ? 6.5f : 5.5f) : 4.5f) * npc.direction;
                            if (malice)
                                npc.velocity.X *= jumpSpeedMult;

                            npc.ai[0] = -120f;
                            npc.ai[1] += 1f;
                        }
                        else
                        {
                            npc.velocity.Y = -8f * speedMult;
                            npc.velocity.X += (phase2 ? (death ? 6f : 5f) : 4f) * npc.direction;
                            if (malice)
                                npc.velocity.X *= jumpSpeedMult;

                            npc.ai[0] = -120f;
                            npc.ai[1] += 1f;
                        }
                    }
                    else if (npc.ai[0] >= -30f)
                        npc.aiAction = 1;
                }
            }

            // Change jump velocity
            else if (npc.target < Main.maxPlayers && ((npc.direction == 1 && npc.velocity.X < 3f) || (npc.direction == -1 && npc.velocity.X > -3f)))
            {
                if ((npc.direction == -1 && npc.velocity.X < 0.1) || (npc.direction == 1 && npc.velocity.X > -0.1))
                    npc.velocity.X += (malice ? 0.4f : death ? 0.25f : 0.2f) * npc.direction;
                else
                    npc.velocity.X *= malice ? 0.9f : death ? 0.92f : 0.93f;
            }

            // Spawn dust
            int num249 = Dust.NewDust(npc.position, npc.width, npc.height, 4, npc.velocity.X, npc.velocity.Y, 255, new Color(0, 80, 255, 80), npc.scale * 1.2f);
            Main.dust[num249].noGravity = true;
            dust = Main.dust[num249];
            dust.velocity *= 0.5f;

            if (npc.life <= 0)
                return false;

            // Adjust size based on HP
            float maxScale = death ? 3f : 1.25f;
            float minScale = 0.75f;
            lifeRatio = lifeRatio * (maxScale - minScale) + minScale;
            lifeRatio *= num234;
            if (lifeRatio != npc.scale)
            {
                npc.position.X += npc.width / 2;
                npc.position.Y += npc.height;
                npc.scale = lifeRatio;
                npc.width = (int)(98f * npc.scale);
                npc.height = (int)(92f * npc.scale);
                npc.position.X -= npc.width / 2;
                npc.position.Y -= npc.height;
            }

            // Slime spawning
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                int num251 = (int)(npc.lifeMax * 0.05);
                if (npc.life + num251 < npc.ai[3])
                {
                    npc.ai[3] = npc.life;
                    int num252 = Main.rand.Next(2, 4);
                    for (int num253 = 0; num253 < num252; num253++)
                    {
                        int x = (int)(npc.position.X + Main.rand.Next(npc.width - 32));
                        int y = (int)(npc.position.Y + Main.rand.Next(npc.height - 32));

                        int random = phase2 ? 10 : 8;
                        int npcType = death ? Main.rand.Next(4) + 6 : Main.rand.Next(random);
                        switch (npcType)
                        {
                            case 0:
                                npcType = NPCID.BlueSlime;
                                break;
                            case 1:
                                npcType = NPCID.YellowSlime;
                                break;
                            case 2:
                                npcType = NPCID.RedSlime;
                                break;
                            case 3:
                                npcType = NPCID.PurpleSlime;
                                break;
                            case 4:
                                npcType = NPCID.GreenSlime;
                                break;
                            case 5:
                                npcType = NPCID.IceSlime;
                                break;
                            case 6:
                            case 7:
                            case 8:
                            case 9:
                                npcType = NPCID.SlimeSpiked;
                                break;
                            default:
                                break;
                        }

                        if ((Main.raining || malice) && Main.rand.NextBool(10))
                        {
                            npcType = NPCID.UmbrellaSlime;

                            if (Main.rand.NextBool(5) && Main.hardMode)
                                npcType = NPCID.RainbowSlime;
                        }

                        if (Main.rand.NextBool(250))
                            npcType = NPCID.Pinky;

                        int num255 = NPC.NewNPC(x, y, npcType);
                        Main.npc[num255].SetDefaults(npcType, -1f);
                        Main.npc[num255].velocity.X = Main.rand.Next(-15, 16) * 0.1f;
                        Main.npc[num255].velocity.Y = Main.rand.Next(-30, 1) * 0.1f;
                        Main.npc[num255].ai[0] = -1000 * Main.rand.Next(3);
                        Main.npc[num255].ai[1] = 0f;

                        if (Main.netMode == NetmodeID.Server && num255 < Main.maxNPCs)
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num255, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            return false;
        }

        public static void GetPlaceToTeleportTo(NPC npc)
        {
            npc.TargetClosest(false);
            Point point3 = npc.Center.ToTileCoordinates();
            Point point4 = Main.player[npc.target].Center.ToTileCoordinates();
            Vector2 vector30 = Main.player[npc.target].Center - npc.Center;

            int num235 = 10;
            int num236 = 0;
            int num237 = 7;
            int num238 = 0;

            bool flag10 = false;
            if (npc.localAI[0] >= 360f || vector30.Length() > 2000f)
            {
                if (npc.localAI[0] >= 360f)
                    npc.localAI[0] = 360f;

                flag10 = true;
                num238 = 100;
            }

            while (!flag10 && num238 < 100)
            {
                num238++;
                int num239 = Main.rand.Next(point4.X - num235, point4.X + num235 + 1);
                int num240 = Main.rand.Next(point4.Y - num235, point4.Y + 1);

                if ((num240 < point4.Y - num237 || num240 > point4.Y + num237 || num239 < point4.X - num237 || num239 > point4.X + num237) &&
                    (num240 < point3.Y - num236 || num240 > point3.Y + num236 || num239 < point3.X - num236 || num239 > point3.X + num236) &&
                    !Main.tile[num239, num240].nactive())
                {
                    int num241 = num240;
                    int num242 = 0;

                    if (Main.tile[num239, num241].nactive() && Main.tileSolid[Main.tile[num239, num241].type] && !Main.tileSolidTop[Main.tile[num239, num241].type])
                    {
                        num242 = 1;
                    }
                    else
                    {
                        for (; num242 < 150 && num241 + num242 < Main.maxTilesY; num242++)
                        {
                            int y = num241 + num242;
                            if (Main.tile[num239, y].nactive() && Main.tileSolid[Main.tile[num239, y].type] && !Main.tileSolidTop[Main.tile[num239, y].type])
                            {
                                num242--;
                                break;
                            }
                        }
                    }
                    num240 += num242;

                    bool flag13 = true;
                    if (flag13 && Main.tile[num239, num240].lava())
                        flag13 = false;
                    if (flag13 && !Collision.CanHitLine(npc.Center, 0, 0, Main.player[npc.target].Center, 0, 0))
                        flag13 = false;

                    if (flag13)
                    {
                        npc.localAI[1] = num239 * 16 + 8;
                        npc.localAI[2] = num240 * 16 + 16;
                        break;
                    }
                }
            }

            if (num238 >= 100)
            {
                Vector2 bottom = Main.player[Player.FindClosest(npc.position, npc.width, npc.height)].Bottom;
                npc.localAI[1] = bottom.X;
                npc.localAI[2] = bottom.Y;
            }
        }
    }
}
