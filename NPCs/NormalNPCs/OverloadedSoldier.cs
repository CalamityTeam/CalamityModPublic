using System;
using Microsoft.Xna.Framework;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.World;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class OverloadedSoldier : ModNPC
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Overloaded Soldier");
            Main.npcFrameCount[npc.type] = 14;
        }

        public override void SetDefaults()
        {
            npc.aiStyle = -1;
            aiType = -1;
            npc.damage = NPC.downedMoonlord ? 84 : 42;
            npc.width = 18;
            npc.height = 40;
            npc.defense = 18;
            npc.DR_NERD(0.15f);
            npc.lifeMax = NPC.downedMoonlord ? 1350 : 135;
            npc.knockBackResist = 0f;
            npc.value = Item.buyPrice(0, 0, 2, 0);
            npc.HitSound = SoundID.NPCHit2;
            npc.DeathSound = SoundID.NPCDeath2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<OverloadedSoldierBanner>();
            npc.Calamity().VulnerableToSickness = false;
            npc.Calamity().VulnerableToWater = true;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += (double)Math.Abs(npc.velocity.X);
            if (npc.frameCounter > 6.0)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = npc.frame.Y + frameHeight;
            }
            if (npc.velocity.Y == 0f)
            {
                if (npc.direction == 1)
                    npc.spriteDirection = 1;
                if (npc.direction == -1)
                    npc.spriteDirection = -1;
            }
            else
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = 0;
                return;
            }
            if (npc.velocity.X == 0f)
            {
                npc.frameCounter = 0.0;
                npc.frame.Y = 0;
            }
            else
            {
                if (npc.frame.Y < frameHeight)
                    npc.frame.Y = frameHeight;
                if (npc.frame.Y > frameHeight * 13)
                    npc.frame.Y = frameHeight;
            }
        }

        public override void AI()
        {
            Lighting.AddLight((int)((npc.position.X + (float)(npc.width / 2)) / 16f), (int)((npc.position.Y + (float)(npc.height / 2)) / 16f), 0.35f, 0f, 0.15f);
            bool flag3 = false;
            if (npc.velocity.X == 0f)
            {
                flag3 = true;
            }
            if (npc.justHit)
            {
                flag3 = false;
            }
            bool flag4 = false;
            bool flag5 = false;
            int num35 = 60;
            if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
            {
                flag4 = true;
            }
            if ((npc.position.X == npc.oldPosition.X || npc.ai[3] >= (float)num35) | flag4)
            {
                npc.ai[3] += 1f;
            }
            else if ((double)Math.Abs(npc.velocity.X) > 0.9 && npc.ai[3] > 0f)
            {
                npc.ai[3] -= 1f;
            }
            if (npc.ai[3] > (float)(num35 * 10))
            {
                npc.ai[3] = 0f;
            }
            if (npc.justHit)
            {
                npc.ai[3] = 0f;
            }
            if (npc.ai[3] == (float)num35)
            {
                npc.netUpdate = true;
            }
            if (npc.ai[3] < (float)num35)
            {
                npc.TargetClosest(true);
            }
            else if (npc.ai[2] <= 0f)
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
                if (npc.direction == 0)
                {
                    npc.direction = 1;
                }
            }
            float num65 = 1.5f;
            float num66 = 0.1f;
            if (CalamityWorld.death)
            {
                num65 *= 1.5f;
                num66 *= 1.5f;
            }
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) < 400f)
            {
                num65 += (CalamityWorld.death ? 6f : 4f) - ((Main.player[npc.target].Center - npc.Center).Length() * 0.01f);
             }
            if (npc.velocity.X < -num65 || npc.velocity.X > num65)
            {
                if (npc.velocity.Y == 0f)
                    npc.velocity *= 0.7f;
            }
            else if (npc.velocity.X < num65 && npc.direction == 1)
            {
                npc.velocity.X = npc.velocity.X + num66;
                if (npc.velocity.X > num65)
                    npc.velocity.X = num65;
            }
            else if (npc.velocity.X > -num65 && npc.direction == -1)
            {
                npc.velocity.X = npc.velocity.X - num66;
                if (npc.velocity.X < -num65)
                    npc.velocity.X = -num65;
            }
            bool flag22 = false;
            if (npc.velocity.Y == 0f)
            {
                int num161 = (int)(npc.position.Y + (float)npc.height + 7f) / 16;
                int arg_A8FB_0 = (int)npc.position.X / 16;
                int num162 = (int)(npc.position.X + (float)npc.width) / 16;
                for (int num163 = arg_A8FB_0; num163 <= num162; num163++)
                {
                    if (Main.tile[num163, num161] == null)
                    {
                        return;
                    }
                    if (Main.tile[num163, num161].nactive() && Main.tileSolid[(int)Main.tile[num163, num161].type])
                    {
                        flag22 = true;
                        break;
                    }
                }
            }
            if (npc.velocity.Y >= 0f)
            {
                int num164 = 0;
                if (npc.velocity.X < 0f)
                {
                    num164 = -1;
                }
                if (npc.velocity.X > 0f)
                {
                    num164 = 1;
                }
                Vector2 position2 = npc.position;
                position2.X += npc.velocity.X;
                int num165 = (int)((position2.X + (float)(npc.width / 2) + (float)((npc.width / 2 + 1) * num164)) / 16f);
                int num166 = (int)((position2.Y + (float)npc.height - 1f) / 16f);
                if (Main.tile[num165, num166] == null)
                {
                    Main.tile[num165, num166] = new Tile();
                }
                if (Main.tile[num165, num166 - 1] == null)
                {
                    Main.tile[num165, num166 - 1] = new Tile();
                }
                if (Main.tile[num165, num166 - 2] == null)
                {
                    Main.tile[num165, num166 - 2] = new Tile();
                }
                if (Main.tile[num165, num166 - 3] == null)
                {
                    Main.tile[num165, num166 - 3] = new Tile();
                }
                if (Main.tile[num165, num166 + 1] == null)
                {
                    Main.tile[num165, num166 + 1] = new Tile();
                }
                if (Main.tile[num165 - num164, num166 - 3] == null)
                {
                    Main.tile[num165 - num164, num166 - 3] = new Tile();
                }
                if ((float)(num165 * 16) < position2.X + (float)npc.width && (float)(num165 * 16 + 16) > position2.X && ((Main.tile[num165, num166].nactive() && !Main.tile[num165, num166].topSlope() && !Main.tile[num165, num166 - 1].topSlope() && Main.tileSolid[(int)Main.tile[num165, num166].type] && !Main.tileSolidTop[(int)Main.tile[num165, num166].type]) || (Main.tile[num165, num166 - 1].halfBrick() && Main.tile[num165, num166 - 1].nactive())) && (!Main.tile[num165, num166 - 1].nactive() || !Main.tileSolid[(int)Main.tile[num165, num166 - 1].type] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 1].type] || (Main.tile[num165, num166 - 1].halfBrick() && (!Main.tile[num165, num166 - 4].nactive() || !Main.tileSolid[(int)Main.tile[num165, num166 - 4].type] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 4].type]))) && (!Main.tile[num165, num166 - 2].nactive() || !Main.tileSolid[(int)Main.tile[num165, num166 - 2].type] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 2].type]) && (!Main.tile[num165, num166 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num165, num166 - 3].type] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 3].type]) && (!Main.tile[num165 - num164, num166 - 3].nactive() || !Main.tileSolid[(int)Main.tile[num165 - num164, num166 - 3].type]))
                {
                    float num167 = (float)(num166 * 16);
                    if (Main.tile[num165, num166].halfBrick())
                    {
                        num167 += 8f;
                    }
                    if (Main.tile[num165, num166 - 1].halfBrick())
                    {
                        num167 -= 8f;
                    }
                    if (num167 < position2.Y + (float)npc.height)
                    {
                        float num168 = position2.Y + (float)npc.height - num167;
                        float num169 = 16.1f;
                        if (num168 <= num169)
                        {
                            npc.gfxOffY += npc.position.Y + (float)npc.height - num167;
                            npc.position.Y = num167 - (float)npc.height;
                            if (num168 < 9f)
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
            if (flag22)
            {
                int num170 = (int)((npc.position.X + (float)(npc.width / 2) + (float)(15 * npc.direction)) / 16f);
                int num171 = (int)((npc.position.Y + (float)npc.height - 15f) / 16f);
                if (Main.tile[num170, num171] == null)
                {
                    Main.tile[num170, num171] = new Tile();
                }
                if (Main.tile[num170, num171 - 1] == null)
                {
                    Main.tile[num170, num171 - 1] = new Tile();
                }
                if (Main.tile[num170, num171 - 2] == null)
                {
                    Main.tile[num170, num171 - 2] = new Tile();
                }
                if (Main.tile[num170, num171 - 3] == null)
                {
                    Main.tile[num170, num171 - 3] = new Tile();
                }
                if (Main.tile[num170, num171 + 1] == null)
                {
                    Main.tile[num170, num171 + 1] = new Tile();
                }
                if (Main.tile[num170 + npc.direction, num171 - 1] == null)
                {
                    Main.tile[num170 + npc.direction, num171 - 1] = new Tile();
                }
                if (Main.tile[num170 + npc.direction, num171 + 1] == null)
                {
                    Main.tile[num170 + npc.direction, num171 + 1] = new Tile();
                }
                if (Main.tile[num170 - npc.direction, num171 + 1] == null)
                {
                    Main.tile[num170 - npc.direction, num171 + 1] = new Tile();
                }
                Main.tile[num170, num171 + 1].halfBrick();
                if ((Main.tile[num170, num171 - 1].nactive() && (Main.tile[num170, num171 - 1].type == 10 || Main.tile[num170, num171 - 1].type == 388)) & flag5)
                {
                    npc.ai[2] += 1f;
                    npc.ai[3] = 0f;
                    if (npc.ai[2] >= 60f)
                    {
                        npc.velocity.X = 0.5f * (float)-(float)npc.direction;
                        int num172 = 5;
                        if (Main.tile[num170, num171 - 1].type == 388)
                        {
                            num172 = 2;
                        }
                        npc.ai[1] += (float)num172;
                        npc.ai[2] = 0f;
                        bool flag23 = false;
                        if (npc.ai[1] >= 10f)
                        {
                            flag23 = true;
                            npc.ai[1] = 10f;
                        }
                        WorldGen.KillTile(num170, num171 - 1, true, false, false);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !flag23) && flag23 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Main.tile[num170, num171 - 1].type == 10)
                            {
                                bool flag24 = WorldGen.OpenDoor(num170, num171 - 1, npc.direction);
                                if (!flag24)
                                {
                                    npc.ai[3] = (float)num35;
                                    npc.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server & flag24)
                                {
                                    NetMessage.SendData(MessageID.ChangeDoor, -1, -1, null, 0, (float)num170, (float)(num171 - 1), (float)npc.direction, 0, 0, 0);
                                }
                            }
                            if (Main.tile[num170, num171 - 1].type == 388)
                            {
                                bool flag25 = WorldGen.ShiftTallGate(num170, num171 - 1, false);
                                if (!flag25)
                                {
                                    npc.ai[3] = (float)num35;
                                    npc.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server & flag25)
                                {
                                    NetMessage.SendData(MessageID.ChangeDoor, -1, -1, null, 4, (float)num170, (float)(num171 - 1), 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int num173 = npc.spriteDirection;
                    if ((npc.velocity.X < 0f && num173 == -1) || (npc.velocity.X > 0f && num173 == 1))
                    {
                        if (npc.height >= 32 && Main.tile[num170, num171 - 2].nactive() && Main.tileSolid[(int)Main.tile[num170, num171 - 2].type])
                        {
                            if (Main.tile[num170, num171 - 3].nactive() && Main.tileSolid[(int)Main.tile[num170, num171 - 3].type])
                            {
                                npc.velocity.Y = -8f;
                                npc.netUpdate = true;
                            }
                            else
                            {
                                npc.velocity.Y = -7f;
                                npc.netUpdate = true;
                            }
                        }
                        else if (Main.tile[num170, num171 - 1].nactive() && Main.tileSolid[(int)Main.tile[num170, num171 - 1].type])
                        {
                            npc.velocity.Y = -6f;
                            npc.netUpdate = true;
                        }
                        else if (npc.position.Y + (float)npc.height - (float)(num171 * 16) > 20f && Main.tile[num170, num171].nactive() && !Main.tile[num170, num171].topSlope() && Main.tileSolid[(int)Main.tile[num170, num171].type])
                        {
                            npc.velocity.Y = -5f;
                            npc.netUpdate = true;
                        }
                        else if (npc.directionY < 0 && (!Main.tile[num170, num171 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num170, num171 + 1].type]) && (!Main.tile[num170 + npc.direction, num171 + 1].nactive() || !Main.tileSolid[(int)Main.tile[num170 + npc.direction, num171 + 1].type]))
                        {
                            npc.velocity.Y = -8f;
                            npc.velocity.X = npc.velocity.X * 1.5f;
                            npc.netUpdate = true;
                        }
                        else if (flag5)
                        {
                            npc.ai[1] = 0f;
                            npc.ai[2] = 0f;
                        }
                        if ((npc.velocity.Y == 0f & flag3) && npc.ai[3] == 1f)
                        {
                            npc.velocity.Y = -5f;
                        }
                    }
                }
            }
            else if (flag5)
            {
                npc.ai[1] = 0f;
                npc.ai[2] = 0f;
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.playerSafe || !Main.hardMode || spawnInfo.player.Calamity().ZoneAbyss ||
                spawnInfo.player.Calamity().ZoneSunkenSea)
            {
                return 0f;
            }
            return SpawnCondition.Underground.Chance * 0.02f;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<WarCleave>(), 120);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Phantoplasm, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, (int)CalamityDusts.Phantoplasm, hitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<AncientBoneDust>());
            DropHelper.DropItemCondition(npc, ModContent.ItemType<Phantoplasm>(), NPC.downedMoonlord);
        }
    }
}
