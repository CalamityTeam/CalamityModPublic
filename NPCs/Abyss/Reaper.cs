using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables;
using CalamityMod.Items.Placeables.Banners;
using CalamityMod.Items.Potions;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using System;
using System.IO;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Abyss
{
    public class Reaper : ModNPC
    {
        public bool hasBeenHit = false;
        public bool reset = false;
        public bool reset2 = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Reaper Shark");
            Main.npcFrameCount[npc.type] = 4;
        }

        public override void SetDefaults()
        {
            npc.Calamity().canBreakPlayerDefense = true;
            npc.npcSlots = 6f;
            npc.noGravity = true;
            npc.lavaImmune = true;
            npc.damage = 160;
            npc.width = 280;
            npc.height = 150;
            npc.defense = 70;
            npc.lifeMax = 100000; // Previously 190,000
            npc.aiStyle = -1;
            aiType = -1;
            npc.timeLeft = NPC.activeTime * 30;
            npc.value = Item.buyPrice(0, 25, 0, 0);
            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
            npc.knockBackResist = 0f;
            npc.rarity = 2;
            banner = npc.type;
            bannerItem = ModContent.ItemType<ReaperSharkBanner>();
            npc.Calamity().VulnerableToHeat = false;
            npc.Calamity().VulnerableToSickness = true;
            npc.Calamity().VulnerableToElectricity = true;
            npc.Calamity().VulnerableToWater = false;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(reset);
            writer.Write(reset2);
            writer.Write(hasBeenHit);
            writer.Write(npc.localAI[0]);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            reset = reader.ReadBoolean();
            reset2 = reader.ReadBoolean();
            hasBeenHit = reader.ReadBoolean();
            npc.localAI[0] = reader.ReadSingle();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            bool phase1 = npc.life > npc.lifeMax * 0.5;
            bool phase2 = npc.life <= npc.lifeMax * 0.5;
            bool phase3 = npc.life <= npc.lifeMax * 0.1;
            npc.chaseable = hasBeenHit;
            if (npc.soundDelay <= 0)
            {
                npc.soundDelay = 360;
                if (hasBeenHit)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ReaperEnragedRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                else
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ReaperSearchRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
            }
            if (phase3 || phase1)
            {
                if (!reset2 && phase3)
                {
                    npc.damage /= 2;
                    npc.noTileCollide = true;
                    npc.netAlways = true;
                    npc.localAI[0] = 0f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = -16f;
                    npc.ai[3] = 0f;
                    reset2 = true;
                    npc.netUpdate = true;
                }

                npc.spriteDirection = (npc.direction > 0) ? -1 : 1;
                if (npc.ai[2] == 0f)
                {
                    npc.TargetClosest(true);
                    if (!Main.player[npc.target].dead && (Main.player[npc.target].Center - npc.Center).Length() < 170f && Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                    {
                        npc.ai[2] = -16f;
                    }
                    if (npc.justHit || npc.localAI[0] >= 420f)
                    {
                        npc.ai[2] = -16f;
                    }
                    return;
                }

                if (npc.ai[2] < 0f)
                {
                    npc.ai[2] += 1f;
                    if (npc.ai[2] == 0f)
                    {
                        npc.ai[2] = 1f;
                        npc.velocity.X = npc.direction * 2;
                    }
                    return;
                }

                if (npc.ai[2] == 1f)
                {
                    if (npc.direction == 0)
                    {
                        npc.TargetClosest(true);
                    }
                    if (npc.wet || npc.noTileCollide)
                    {
                        bool flag14 = hasBeenHit;
                        npc.TargetClosest(false);
                        if ((!Main.player[npc.target].dead &&
                            Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height) &&
                            //(Main.player[npc.target].Center - npc.Center).Length() < ((Main.player[npc.target].GetCalamityPlayer().anechoicPlating ||
                            //Main.player[npc.target].GetCalamityPlayer().anechoicCoating) ? 300f : 500f) *
                            //(Main.player[npc.target].GetCalamityPlayer().fishAlert ? 3f : 1f))
                            (Main.player[npc.target].Center - npc.Center).Length() < Main.player[npc.target].Calamity().GetAbyssAggro(500f, 300f)) ||
                            npc.justHit)

                        {
                            hasBeenHit = true;
                        }
                        if (!flag14)
                        {
                            if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                            {
                                npc.noTileCollide = false;
                            }
                            if (npc.collideX)
                            {
                                npc.velocity.X = npc.velocity.X * -1f;
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
                        if (flag14)
                        {
                            if (npc.ai[3] > 0f && !Collision.SolidCollision(npc.position, npc.width, npc.height))
                            {
                                if (Collision.CanHit(npc.position, npc.width, npc.height, Main.player[npc.target].position, Main.player[npc.target].width, Main.player[npc.target].height))
                                {
                                    npc.ai[3] = 0f;
                                    npc.ai[1] = 0f;
                                    npc.netUpdate = true;
                                }
                            }
                            else if (npc.ai[3] == 0f)
                            {
                                npc.ai[1] += 1f;
                            }
                            if (npc.ai[1] >= 90f)
                            {
                                npc.ai[3] = 1f;
                                npc.ai[1] = 0f;
                                npc.netUpdate = true;
                            }
                            if (npc.ai[3] == 0f)
                            {
                                npc.noTileCollide = false;
                            }
                            else
                            {
                                npc.noTileCollide = true;
                            }
                            npc.TargetClosest(true);
                            npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.3f;
                            npc.velocity.Y = npc.velocity.Y + (float)npc.directionY * 0.2f;
                            float speedX = phase3 ? 3f : 12f;
                            float speedY = phase3 ? 2.25f : 9f;
                            if (npc.velocity.X > speedX)
                            {
                                npc.velocity.X = speedX;
                            }
                            if (npc.velocity.X < -speedX)
                            {
                                npc.velocity.X = -speedX;
                            }
                            if (npc.velocity.Y > speedY)
                            {
                                npc.velocity.Y = speedY;
                            }
                            if (npc.velocity.Y < -speedY)
                            {
                                npc.velocity.Y = -speedY;
                            }
                        }
                        else
                        {
                            if (!Collision.SolidCollision(npc.position, npc.width, npc.height))
                            {
                                npc.noTileCollide = false;
                            }
                            npc.velocity.X = npc.velocity.X + (float)npc.direction * 0.2f;
                            if (npc.velocity.X < -4f || npc.velocity.X > 4f)
                            {
                                npc.velocity.X = npc.velocity.X * 0.95f;
                            }
                            if (npc.ai[0] == -1f)
                            {
                                npc.velocity.Y = npc.velocity.Y - 0.01f;
                                if ((double)npc.velocity.Y < -0.3)
                                {
                                    npc.ai[0] = 1f;
                                }
                            }
                            else
                            {
                                npc.velocity.Y = npc.velocity.Y + 0.01f;
                                if ((double)npc.velocity.Y > 0.3)
                                {
                                    npc.ai[0] = -1f;
                                }
                            }
                        }
                        int num258 = (int)(npc.position.X + (float)(npc.width / 2)) / 16;
                        int num259 = (int)(npc.position.Y + (float)(npc.height / 2)) / 16;
                        if (Main.tile[num258, num259 - 1] == null)
                        {
                            Main.tile[num258, num259 - 1] = new Tile();
                        }
                        if (Main.tile[num258, num259 + 1] == null)
                        {
                            Main.tile[num258, num259 + 1] = new Tile();
                        }
                        if (Main.tile[num258, num259 + 2] == null)
                        {
                            Main.tile[num258, num259 + 2] = new Tile();
                        }
                        if (Main.tile[num258, num259 - 1].liquid > 128)
                        {
                            if (Main.tile[num258, num259 + 1].active())
                            {
                                npc.ai[0] = -1f;
                            }
                            else if (Main.tile[num258, num259 + 2].active())
                            {
                                npc.ai[0] = -1f;
                            }
                        }
                        if ((double)npc.velocity.Y > 0.4 || (double)npc.velocity.Y < -0.4)
                        {
                            npc.velocity.Y = npc.velocity.Y * 0.95f;
                        }
                    }
                    else
                    {
                        if (npc.velocity.Y == 0f)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                npc.velocity.Y = (float)Main.rand.Next(-250, -180) * 0.1f; //50 20
                                npc.velocity.X = (float)Main.rand.Next(-50, 50) * 0.1f; //20 20
                                npc.netUpdate = true;
                            }
                        }
                        npc.velocity.Y = npc.velocity.Y + 0.4f;
                        if (npc.velocity.Y > 16f)
                        {
                            npc.velocity.Y = 16f;
                        }
                        npc.ai[0] = 1f;
                    }
                    npc.rotation = npc.velocity.Y * (float)npc.direction * 0.1f;
                    if ((double)npc.rotation < -0.2)
                    {
                        npc.rotation = -0.2f;
                    }
                    if ((double)npc.rotation > 0.2)
                    {
                        npc.rotation = 0.2f;
                        return;
                    }
                }
            }
            else if (phase2)
            {
                if (!reset)
                {
                    npc.noTileCollide = true;
                    npc.netAlways = true;
                    npc.localAI[0] = 0f;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    reset = true;
                    npc.netUpdate = true;
                }
                bool expertMode = Main.expertMode;
                int num2 = 30;
                float num3 = expertMode ? 0.4f : 0.35f;
                float scaleFactor = expertMode ? 6f : 5.5f;
                int num4 = expertMode ? 28 : 30;
                float num5 = expertMode ? 12f : 11f;
                int num9 = 90;
                int num16 = 75;
                Vector2 vector = npc.Center;
                Player player = Main.player[npc.target];
                if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
                {
                    npc.TargetClosest(true);
                    player = Main.player[npc.target];
                    npc.netUpdate = true;
                }
                if (player.dead || Vector2.Distance(player.Center, vector) > 5600f)
                {
                    npc.velocity.Y = npc.velocity.Y + 0.4f;
                    if (npc.timeLeft > 10)
                    {
                        npc.timeLeft = 10;
                    }
                    npc.ai[0] = 0f;
                    npc.ai[2] = 0f;
                }
                if (npc.localAI[0] == 0f)
                {
                    npc.localAI[0] = 1f;
                    npc.alpha = 255;
                    npc.rotation = 0f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        npc.ai[0] = -1f;
                        npc.netUpdate = true;
                    }
                }
                float num17 = (float)Math.Atan2((double)(player.Center.Y - vector.Y), (double)(player.Center.X - vector.X));
                if (npc.spriteDirection == 1)
                {
                    num17 += 3.14159274f;
                }
                if (num17 < 0f)
                {
                    num17 += 6.28318548f;
                }
                if (num17 > 6.28318548f)
                {
                    num17 -= 6.28318548f;
                }
                if (npc.ai[0] == -1f)
                {
                    num17 = 0f;
                }
                float num18 = 0.04f;
                if (npc.ai[0] == 1f)
                {
                    num18 = 0f;
                }
                if (npc.rotation < num17)
                {
                    if ((double)(num17 - npc.rotation) > 3.1415926535897931)
                    {
                        npc.rotation -= num18;
                    }
                    else
                    {
                        npc.rotation += num18;
                    }
                }
                if (npc.rotation > num17)
                {
                    if ((double)(npc.rotation - num17) > 3.1415926535897931)
                    {
                        npc.rotation += num18;
                    }
                    else
                    {
                        npc.rotation -= num18;
                    }
                }
                if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
                {
                    npc.rotation = num17;
                }
                if (npc.rotation < 0f)
                {
                    npc.rotation += 6.28318548f;
                }
                if (npc.rotation > 6.28318548f)
                {
                    npc.rotation -= 6.28318548f;
                }
                if (npc.rotation > num17 - num18 && npc.rotation < num17 + num18)
                {
                    npc.rotation = num17;
                }
                if (npc.ai[0] != -1f)
                {
                    if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                    {
                        npc.alpha += 15;
                    }
                    else
                    {
                        npc.alpha -= 15;
                    }
                    if (npc.alpha < 0)
                    {
                        npc.alpha = 0;
                    }
                    if (npc.alpha > 150)
                    {
                        npc.alpha = 150;
                    }
                }
                if (npc.ai[0] == -1f)
                {
                    npc.dontTakeDamage = true;
                    npc.chaseable = false;
                    npc.velocity *= 0.98f;
                    int num19 = Math.Sign(player.Center.X - vector.X);
                    if (num19 != 0)
                    {
                        npc.direction = num19;
                        npc.spriteDirection = -npc.direction;
                    }
                    if (npc.ai[2] > 20f)
                    {
                        npc.velocity.Y = -2f;
                        npc.alpha -= 5;
                        if (Collision.SolidCollision(npc.position, npc.width, npc.height))
                        {
                            npc.alpha += 15;
                        }
                        if (npc.alpha < 0)
                        {
                            npc.alpha = 0;
                        }
                        if (npc.alpha > 150)
                        {
                            npc.alpha = 150;
                        }
                    }
                    if (npc.ai[2] == (float)(num9 - 30))
                    {
                        int num20 = 36;
                        for (int i = 0; i < num20; i++)
                        {
                            Vector2 expr_80F = (Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f * 0.5f).RotatedBy((double)((float)(i - (num20 / 2 - 1)) * 6.28318548f / (float)num20), default) + npc.Center;
                            Vector2 vector2 = expr_80F - npc.Center;
                            int num21 = Dust.NewDust(expr_80F + vector2, 0, 0, 172, vector2.X * 2f, vector2.Y * 2f, 100, default, 1.4f);
                            Main.dust[num21].noGravity = true;
                            Main.dust[num21].noLight = true;
                            Main.dust[num21].velocity = Vector2.Normalize(vector2) * 3f;
                        }
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/ReaperEnragedRoar"), (int)npc.position.X, (int)npc.position.Y);
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= (float)num16)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                        return;
                    }
                }
                else if (npc.ai[0] == 0f && !player.dead)
                {
                    npc.dontTakeDamage = false;
                    npc.chaseable = true;
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = (float)(300 * Math.Sign((vector - player.Center).X));
                    }
                    Vector2 vector3 = Vector2.Normalize(player.Center + new Vector2(npc.ai[1], -200f) - vector - npc.velocity) * scaleFactor;
                    if (npc.velocity.X < vector3.X)
                    {
                        npc.velocity.X = npc.velocity.X + num3;
                        if (npc.velocity.X < 0f && vector3.X > 0f)
                        {
                            npc.velocity.X = npc.velocity.X + num3;
                        }
                    }
                    else if (npc.velocity.X > vector3.X)
                    {
                        npc.velocity.X = npc.velocity.X - num3;
                        if (npc.velocity.X > 0f && vector3.X < 0f)
                        {
                            npc.velocity.X = npc.velocity.X - num3;
                        }
                    }
                    if (npc.velocity.Y < vector3.Y)
                    {
                        npc.velocity.Y = npc.velocity.Y + num3;
                        if (npc.velocity.Y < 0f && vector3.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y + num3;
                        }
                    }
                    else if (npc.velocity.Y > vector3.Y)
                    {
                        npc.velocity.Y = npc.velocity.Y - num3;
                        if (npc.velocity.Y > 0f && vector3.Y < 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y - num3;
                        }
                    }
                    int num22 = Math.Sign(player.Center.X - vector.X);
                    if (num22 != 0)
                    {
                        if (npc.ai[2] == 0f && num22 != npc.direction)
                        {
                            npc.rotation += 3.14159274f;
                        }
                        npc.direction = num22;
                        if (npc.spriteDirection != -npc.direction)
                        {
                            npc.rotation += 3.14159274f;
                        }
                        npc.spriteDirection = -npc.direction;
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= (float)num2)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vector) * num5;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num22 != 0)
                        {
                            npc.direction = num22;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                        npc.netUpdate = true;
                        return;
                    }
                }
                else if (npc.ai[0] == 1f)
                {
                    int num24 = 7;
                    for (int j = 0; j < num24; j++)
                    {
                        Vector2 arg_E1C_0 = (Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f).RotatedBy((double)(j - (num24 / 2 - 1)) * 3.1415926535897931 / (double)(float)num24, default) + vector;
                        Vector2 vector4 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                        int num25 = Dust.NewDust(arg_E1C_0 + vector4, 0, 0, 172, vector4.X * 2f, vector4.Y * 2f, 100, default, 1.4f);
                        Main.dust[num25].noGravity = true;
                        Main.dust[num25].noLight = true;
                        Main.dust[num25].velocity /= 4f;
                        Main.dust[num25].velocity -= npc.velocity;
                    }
                    npc.ai[2] += 1f;
                    if (npc.ai[2] >= (float)num4)
                    {
                        npc.ai[0] = 0f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.netUpdate = true;
                        return;
                    }
                }
            }
            if (Vector2.Distance(Main.player[npc.target].Center, npc.Center) > 6400f)
            {
                npc.active = false;
            }
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
            {
                return hasBeenHit;
            }
            return null;
        }

        public override void FindFrame(int frameHeight)
        {
            npc.frameCounter += hasBeenHit ? 0.15f : 0.075f;
            npc.frameCounter %= Main.npcFrameCount[npc.type];
            int frame = (int)npc.frameCounter;
            npc.frame.Y = frame * frameHeight;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<CrushDepth>(), 300, true);
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.player.Calamity().ZoneAbyssLayer3 && spawnInfo.water && !NPC.AnyNPCs(ModContent.NPCType<Reaper>()) &&
                !NPC.AnyNPCs(ModContent.NPCType<ColossalSquid>()) && !NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHead>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 0.6f;
            }
            if (spawnInfo.player.Calamity().ZoneAbyssLayer4 && spawnInfo.water && !NPC.AnyNPCs(ModContent.NPCType<Reaper>()))
            {
                return SpawnCondition.CaveJellyfish.Chance * 1.2f;
            }
            return 0f;
        }

        public override void NPCLoot()
        {
            DropHelper.DropItem(npc, ModContent.ItemType<Voidstone>(), 40, 50);
            DropHelper.DropItem(npc, ModContent.ItemType<AnechoicCoating>(), 2, 3);
            int minCells = Main.expertMode ? 14 : 10;
            int maxCells = Main.expertMode ? 22 : 17;
            DropHelper.DropItemCondition(npc, ModContent.ItemType<DepthCells>(), CalamityWorld.downedCalamitas, 0.5f, minCells, maxCells);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<ReaperTooth>(), CalamityWorld.downedPolterghast, 1f, 3, 4);
            DropHelper.DropItemCondition(npc, ModContent.ItemType<DeepSeaDumbbell>(), CalamityWorld.downedPolterghast, 3, 1, 1);
            if (CalamityWorld.downedPolterghast)
            {
                DropHelper.DropItemChance(npc, ModContent.ItemType<Valediction>(), 3);
            }
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                for (int k = 0; k < 40; k++)
                {
                    Dust.NewDust(npc.position, npc.width, npc.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
                }
            }
        }
    }
}
