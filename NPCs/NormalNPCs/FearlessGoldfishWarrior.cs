using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using CalamityMod.Items.Placeables.Banners;

namespace CalamityMod.NPCs.NormalNPCs
{
    public class FearlessGoldfishWarrior : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
        }

        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.damage = Main.hardMode ? 100 : 30;
            NPC.width = 36;
            NPC.height = 32;
            NPC.defense = Main.hardMode ? 10 : 2;
            NPC.lifeMax = Main.hardMode ? 150 : 50;
            NPC.knockBackResist = Main.hardMode ? 0.2f : 0.5f;
            NPC.value = Item.buyPrice(0, 0, 1, 0);
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            Banner = NPC.type;
            BannerItem = ModContent.ItemType<FearlessGoldfishWarriorBanner>();
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events.Rain,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.FearlessGoldfishWarrior")
            });
        }

        public override void AI()
        {
            bool flag3 = false;
            if (NPC.velocity.X == 0f)
            {
                flag3 = true;
            }
            if (NPC.justHit)
            {
                flag3 = false;
            }
            bool flag4 = false;
            bool flag5 = false;
            int num35 = 60;
            if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
            {
                flag4 = true;
            }
            if ((NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= (float)num35) | flag4)
            {
                NPC.ai[3] += 1f;
            }
            else if ((double)Math.Abs(NPC.velocity.X) > 0.9 && NPC.ai[3] > 0f)
            {
                NPC.ai[3] -= 1f;
            }
            if (NPC.ai[3] > (float)(num35 * 10))
            {
                NPC.ai[3] = 0f;
            }
            if (NPC.justHit)
            {
                NPC.ai[3] = 0f;
            }
            if (NPC.ai[3] == (float)num35)
            {
                NPC.netUpdate = true;
            }
            if (NPC.ai[3] < (float)num35)
            {
                NPC.TargetClosest(true);
            }
            else if (NPC.ai[2] <= 0f)
            {
                if (NPC.velocity.X == 0f)
                {
                    if (NPC.velocity.Y == 0f)
                    {
                        NPC.ai[0] += 1f;
                        if (NPC.ai[0] >= 2f)
                        {
                            NPC.direction *= -1;
                            NPC.spriteDirection = NPC.direction;
                            NPC.ai[0] = 0f;
                        }
                    }
                }
                else
                {
                    NPC.ai[0] = 0f;
                }
                if (NPC.direction == 0)
                {
                    NPC.direction = 1;
                }
            }
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 18f)
            {
                NPC.ai[3] = 0f;
                NPC.velocity.X = NPC.velocity.X * 0.9f;
                if ((double)NPC.velocity.X > -0.1 && (double)NPC.velocity.X < 0.1)
                    NPC.velocity.X = 0f;
                return;
            }
            float num65 = CalamityWorld.death ? 3f : CalamityWorld.revenge ? 2f : 1f;
            float num66 = CalamityWorld.death ? 0.28f : CalamityWorld.revenge ? 0.18f : 0.08f;
            num65 += (1f - (float)NPC.life / (float)NPC.lifeMax) * 2f;
            num66 += (1f - (float)NPC.life / (float)NPC.lifeMax) * 0.2f;
            if (NPC.velocity.X < -num65 || NPC.velocity.X > num65)
            {
                if (NPC.velocity.Y == 0f)
                    NPC.velocity *= 0.7f;
            }
            else if (NPC.velocity.X < num65 && NPC.direction == 1)
            {
                NPC.velocity.X = NPC.velocity.X + num66;
                if (NPC.velocity.X > num65)
                    NPC.velocity.X = num65;
            }
            else if (NPC.velocity.X > -num65 && NPC.direction == -1)
            {
                NPC.velocity.X = NPC.velocity.X - num66;
                if (NPC.velocity.X < -num65)
                    NPC.velocity.X = -num65;
            }
            bool flag22 = false;
            if (NPC.velocity.Y == 0f)
            {
                int num161 = (int)(NPC.position.Y + (float)NPC.height + 7f) / 16;
                int arg_A8FB_0 = (int)NPC.position.X / 16;
                int num162 = (int)(NPC.position.X + (float)NPC.width) / 16;
                for (int num163 = arg_A8FB_0; num163 <= num162; num163++)
                {
                    if (Main.tile[num163, num161] == null)
                    {
                        return;
                    }
                    if (Main.tile[num163, num161].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num163, num161].TileType])
                    {
                        flag22 = true;
                        break;
                    }
                }
            }
            if (NPC.velocity.Y >= 0f)
            {
                int num164 = 0;
                if (NPC.velocity.X < 0f)
                {
                    num164 = -1;
                }
                if (NPC.velocity.X > 0f)
                {
                    num164 = 1;
                }
                Vector2 position2 = NPC.position;
                position2.X += NPC.velocity.X;
                int num165 = (int)((position2.X + (float)(NPC.width / 2) + (float)((NPC.width / 2 + 1) * num164)) / 16f);
                int num166 = (int)((position2.Y + (float)NPC.height - 1f) / 16f);
                if ((float)(num165 * 16) < position2.X + (float)NPC.width && (float)(num165 * 16 + 16) > position2.X && ((Main.tile[num165, num166].HasUnactuatedTile && !Main.tile[num165, num166].TopSlope && !Main.tile[num165, num166 - 1].TopSlope && Main.tileSolid[(int)Main.tile[num165, num166].TileType] && !Main.tileSolidTop[(int)Main.tile[num165, num166].TileType]) || (Main.tile[num165, num166 - 1].IsHalfBlock && Main.tile[num165, num166 - 1].HasUnactuatedTile)) && (!Main.tile[num165, num166 - 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num165, num166 - 1].TileType] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 1].TileType] || (Main.tile[num165, num166 - 1].IsHalfBlock && (!Main.tile[num165, num166 - 4].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num165, num166 - 4].TileType] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 4].TileType]))) && (!Main.tile[num165, num166 - 2].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num165, num166 - 2].TileType] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 2].TileType]) && (!Main.tile[num165, num166 - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num165, num166 - 3].TileType] || Main.tileSolidTop[(int)Main.tile[num165, num166 - 3].TileType]) && (!Main.tile[num165 - num164, num166 - 3].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num165 - num164, num166 - 3].TileType]))
                {
                    float num167 = (float)(num166 * 16);
                    if (Main.tile[num165, num166].IsHalfBlock)
                    {
                        num167 += 8f;
                    }
                    if (Main.tile[num165, num166 - 1].IsHalfBlock)
                    {
                        num167 -= 8f;
                    }
                    if (num167 < position2.Y + (float)NPC.height)
                    {
                        float num168 = position2.Y + (float)NPC.height - num167;
                        float num169 = 16.1f;
                        if (num168 <= num169)
                        {
                            NPC.gfxOffY += NPC.position.Y + (float)NPC.height - num167;
                            NPC.position.Y = num167 - (float)NPC.height;
                            if (num168 < 9f)
                            {
                                NPC.stepSpeed = 1f;
                            }
                            else
                            {
                                NPC.stepSpeed = 2f;
                            }
                        }
                    }
                }
            }
            if (flag22)
            {
                int num170 = (int)((NPC.position.X + (float)(NPC.width / 2) + (float)(15 * NPC.direction)) / 16f);
                int num171 = (int)((NPC.position.Y + (float)NPC.height - 15f) / 16f);
                if ((Main.tile[num170, num171 - 1].HasUnactuatedTile && (Main.tile[num170, num171 - 1].TileType == 10 || Main.tile[num170, num171 - 1].TileType == 388)) & flag5)
                {
                    NPC.ai[2] += 1f;
                    NPC.ai[3] = 0f;
                    if (NPC.ai[2] >= 60f)
                    {
                        NPC.velocity.X = 0.5f * (float)-(float)NPC.direction;
                        int num172 = 5;
                        if (Main.tile[num170, num171 - 1].TileType == 388)
                        {
                            num172 = 2;
                        }
                        NPC.ai[1] += (float)num172;
                        NPC.ai[2] = 0f;
                        bool flag23 = false;
                        if (NPC.ai[1] >= 10f)
                        {
                            flag23 = true;
                            NPC.ai[1] = 10f;
                        }
                        WorldGen.KillTile(num170, num171 - 1, true, false, false);
                        if ((Main.netMode != NetmodeID.MultiplayerClient || !flag23) && flag23 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (Main.tile[num170, num171 - 1].TileType == 10)
                            {
                                bool flag24 = WorldGen.OpenDoor(num170, num171 - 1, NPC.direction);
                                if (!flag24)
                                {
                                    NPC.ai[3] = (float)num35;
                                    NPC.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server & flag24)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, (float)num170, (float)(num171 - 1), (float)NPC.direction, 0, 0, 0);
                                }
                            }
                            if (Main.tile[num170, num171 - 1].TileType == 388)
                            {
                                bool flag25 = WorldGen.ShiftTallGate(num170, num171 - 1, false);
                                if (!flag25)
                                {
                                    NPC.ai[3] = (float)num35;
                                    NPC.netUpdate = true;
                                }
                                if (Main.netMode == NetmodeID.Server & flag25)
                                {
                                    NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, (float)num170, (float)(num171 - 1), 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                else
                {
                    int num173 = NPC.spriteDirection;
                    if ((NPC.velocity.X < 0f && num173 == -1) || (NPC.velocity.X > 0f && num173 == 1))
                    {
                        if (NPC.height >= 32 && Main.tile[num170, num171 - 2].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num170, num171 - 2].TileType])
                        {
                            if (Main.tile[num170, num171 - 3].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num170, num171 - 3].TileType])
                            {
                                NPC.velocity.Y = -8f;
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.velocity.Y = -7f;
                                NPC.netUpdate = true;
                            }
                        }
                        else if (Main.tile[num170, num171 - 1].HasUnactuatedTile && Main.tileSolid[(int)Main.tile[num170, num171 - 1].TileType])
                        {
                            NPC.velocity.Y = -6f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.position.Y + (float)NPC.height - (float)(num171 * 16) > 20f && Main.tile[num170, num171].HasUnactuatedTile && !Main.tile[num170, num171].TopSlope && Main.tileSolid[(int)Main.tile[num170, num171].TileType])
                        {
                            NPC.velocity.Y = -5f;
                            NPC.netUpdate = true;
                        }
                        else if (NPC.directionY < 0 && (!Main.tile[num170, num171 + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num170, num171 + 1].TileType]) && (!Main.tile[num170 + NPC.direction, num171 + 1].HasUnactuatedTile || !Main.tileSolid[(int)Main.tile[num170 + NPC.direction, num171 + 1].TileType]))
                        {
                            NPC.velocity.Y = -8f;
                            NPC.velocity.X = NPC.velocity.X * 1.5f;
                            NPC.netUpdate = true;
                        }
                        else if (flag5)
                        {
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                        }
                        if ((NPC.velocity.Y == 0f & flag3) && NPC.ai[3] == 1f)
                        {
                            NPC.velocity.Y = -5f;
                        }
                    }
                }
            }
            else if (flag5)
            {
                NPC.ai[1] = 0f;
                NPC.ai[2] = 0f;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) < 18f || NPC.IsABestiaryIconDummy)
            {
                NPC.frameCounter += 1.0;
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y = frameHeight * 5;
                if (NPC.frame.Y > frameHeight * 9)
                    NPC.frame.Y = frameHeight * 5;
            }
            else
            {
                NPC.frameCounter += (double)Math.Abs(NPC.velocity.X);
                if (NPC.frameCounter > 6.0)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                }
                if (NPC.velocity.Y == 0f)
                {
                    if (NPC.direction == 1)
                        NPC.spriteDirection = 1;
                    if (NPC.direction == -1)
                        NPC.spriteDirection = -1;
                }
                else
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = frameHeight;
                    return;
                }
                if (NPC.velocity.X == 0f)
                {
                    NPC.frameCounter = 0.0;
                    NPC.frame.Y = 0;
                }
                else
                {
                    if (NPC.frame.Y > frameHeight * 4)
                        NPC.frame.Y = 0;
                }
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.PlayerSafe || !spawnInfo.Player.ZoneRain || spawnInfo.Player.ZoneDesert || spawnInfo.Player.Calamity().ZoneSulphur)
            {
                return 0f;
            }
            return SpawnCondition.OverworldDayRain.Chance * 0.05f;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            bool instakill = false;
            List<string> metarexNames = new List<string> { "LordMetarex", "Metarex" };
            foreach (string s in metarexNames)
                if (s.ToLower() == target.name.ToLower())
                {
                    instakill = true;
                    break;
                }

            if (instakill)
            {
                target.KillMe(PlayerDeathReason.ByCustomReason(CalamityUtils.GetText("Status.Death.Goldfish").Format(target.name)), 1000.0, 0, false);
                modifiers.FinalDamage *= target.statLifeMax2 * Main.rand.NextFloat(2.0f, 3.5f);
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemID.TinHelmet);
            npcLoot.AddIf(() => Main.hardMode, ItemID.MagicDagger);
        }
    }
}
