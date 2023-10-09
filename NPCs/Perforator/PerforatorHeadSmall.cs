using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHeadSmall : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private const int MsgType = 23;
        private bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.8f,
                PortraitScale = 0.8f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/PerforatorSmall_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 60
            };
            value.Position.X += 60;
            value.Position.Y += 50;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 42;
            NPC.height = 62;
            NPC.LifeMaxNERB(1000, 1200, 50000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<PerforatorHive>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Perforator")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
        }

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Enrage
            if ((!Main.player[NPC.target].ZoneCrimson || (NPC.position.Y / 16f) < Main.worldSurface) && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && (!Main.player[NPC.target].ZoneCrimson || bossRush))
                enrageScale += 1f;
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || bossRush))
                enrageScale += 1f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float speed = 0.15f;
            float turnSpeed = 0.1f;

            if (expertMode)
            {
                float velocityScale = (death ? 0.15f : 0.1f) * enrageScale;
                speed += velocityScale * (1f - lifeRatio);
                float accelerationScale = (death ? 0.12f : 0.1f) * enrageScale;
                turnSpeed += accelerationScale * (1f - lifeRatio);
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    int Previous = NPC.whoAmI;
                    int maxLength = death ? 9 : revenge ? 8 : expertMode ? 7 : 5;
                    for (int num36 = 0; num36 < maxLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < maxLength - 1)
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<PerforatorBodySmall>(), NPC.whoAmI);
                        }
                        else
                        {
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<PerforatorTailSmall>(), NPC.whoAmI);
                        }
                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MsgType, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        Previous = lol;
                    }
                    TailSpawned = true;
                }
            }

            int num12 = (int)(NPC.position.X / 16f) - 1;
            int num13 = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int num14 = (int)(NPC.position.Y / 16f) - 1;
            int num15 = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (num12 < 0)
            {
                num12 = 0;
            }
            if (num13 > Main.maxTilesX)
            {
                num13 = Main.maxTilesX;
            }
            if (num14 < 0)
            {
                num14 = 0;
            }
            if (num15 > Main.maxTilesY)
            {
                num15 = Main.maxTilesY;
            }
            bool flag2 = false;
            if (!flag2)
            {
                for (int k = num12; k < num13; k++)
                {
                    for (int l = num14; l < num15; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                flag2 = true;
                                break;
                            }
                        }
                    }
                }
            }
            if (!flag2)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int num16 = death ? 160 : revenge ? 200 : expertMode ? 240 : 300;
                bool flag3 = true;
                if (NPC.position.Y > player.position.Y)
                {
                    for (int m = 0; m < 255; m++)
                    {
                        if (Main.player[m].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - num16, (int)Main.player[m].position.Y - num16, num16 * 2, num16 * 2);
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
            }
            else
            {
                NPC.localAI[1] = 0f;
            }
            float num17 = 16f;
            if (player.dead || CalamityGlobalNPC.perfHive < 0 || !Main.npc[CalamityGlobalNPC.perfHive].active)
            {
                flag2 = false;
                NPC.velocity.Y += 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16.0)
                {
                    NPC.velocity.Y += 1f;
                    num17 = 32f;
                }
                if ((double)NPC.position.Y > Main.rockLayer * 16.0)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<PerforatorHeadSmall>() || Main.npc[a].type == ModContent.NPCType<PerforatorBodySmall>() ||
                            Main.npc[a].type == ModContent.NPCType<PerforatorTailSmall>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }

            if (Main.zenithWorld && CalamityGlobalNPC.perfHive >= 0)
                NPC.ai[0]++;
            
            //GFB movement
            if (NPC.ai[0] >= 300f && CalamityGlobalNPC.perfHive >= 0)
            {
                flag2 = false;
                
                NPC Hive = Main.npc[CalamityGlobalNPC.perfHive];
                Vector2 targetDistance = Hive.Center - NPC.Center;
                float velocity = 16f;
                float acceleration = 0.15f;
                //Move towards the hive really quickly
                if (targetDistance.Length() > 32f)
                    CalamityUtils.SmoothMovement(NPC, 0f, targetDistance, velocity, acceleration, true);
                else
                {
                    //Go and succ it
                    NPC.Center = Hive.Center + (Vector2.UnitY * 4f).RotatedBy(NPC.rotation);
                    Hive.localAI[1]++;
                }

                if (NPC.ai[0] >= 600f)
                {
                    NPC.ai[0] = 0f;
                    Hive.localAI[1] = 0f;
                }
            }
            //Normal movement
            else
            {
                float num18 = speed;
                float num19 = turnSpeed;
                Vector2 vector3 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
                float num20 = player.position.X + (float)(player.width / 2);
                float num21 = player.position.Y + (float)(player.height / 2);
                num20 = (float)((int)(num20 / 16f) * 16);
                num21 = (float)((int)(num21 / 16f) * 16);
                vector3.X = (float)((int)(vector3.X / 16f) * 16);
                vector3.Y = (float)((int)(vector3.Y / 16f) * 16);
                num20 -= vector3.X;
                num21 -= vector3.Y;
                float num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                if (!flag2)
                {
                    NPC.TargetClosest(true);
                    NPC.velocity.Y = NPC.velocity.Y + 0.15f;
                    if (NPC.velocity.Y > num17)
                    {
                        NPC.velocity.Y = num17;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)num17 * 0.4)
                    {
                        if (NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X - num18 * 1.1f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X + num18 * 1.1f;
                        }
                    }
                    else if (NPC.velocity.Y == num17)
                    {
                        if (NPC.velocity.X < num20)
                        {
                            NPC.velocity.X = NPC.velocity.X + num18;
                        }
                        else if (NPC.velocity.X > num20)
                        {
                            NPC.velocity.X = NPC.velocity.X - num18;
                        }
                    }
                    else if (NPC.velocity.Y > 4f)
                    {
                        if (NPC.velocity.X < 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + num18 * 0.9f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - num18 * 0.9f;
                        }
                    }
                }
                else
                {
                    if (NPC.soundDelay == 0)
                    {
                        float num24 = num22 / 40f;
                        if (num24 < 10f)
                        {
                            num24 = 10f;
                        }
                        if (num24 > 20f)
                        {
                            num24 = 20f;
                        }
                        NPC.soundDelay = (int)num24;
                        SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                    }
                    num22 = (float)Math.Sqrt((double)(num20 * num20 + num21 * num21));
                    float num25 = Math.Abs(num20);
                    float num26 = Math.Abs(num21);
                    float num27 = num17 / num22;
                    num20 *= num27;
                    num21 *= num27;
                    if (((NPC.velocity.X > 0f && num20 > 0f) || (NPC.velocity.X < 0f && num20 < 0f)) && ((NPC.velocity.Y > 0f && num21 > 0f) || (NPC.velocity.Y < 0f && num21 < 0f)))
                    {
                        if (NPC.velocity.X < num20)
                        {
                            NPC.velocity.X = NPC.velocity.X + num19;
                        }
                        else if (NPC.velocity.X > num20)
                        {
                            NPC.velocity.X = NPC.velocity.X - num19;
                        }
                        if (NPC.velocity.Y < num21)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num19;
                        }
                        else if (NPC.velocity.Y > num21)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num19;
                        }
                    }
                    if ((NPC.velocity.X > 0f && num20 > 0f) || (NPC.velocity.X < 0f && num20 < 0f) || (NPC.velocity.Y > 0f && num21 > 0f) || (NPC.velocity.Y < 0f && num21 < 0f))
                    {
                        if (NPC.velocity.X < num20)
                        {
                            NPC.velocity.X = NPC.velocity.X + num18;
                        }
                        else if (NPC.velocity.X > num20)
                        {
                            NPC.velocity.X = NPC.velocity.X - num18;
                        }
                        if (NPC.velocity.Y < num21)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num18;
                        }
                        else if (NPC.velocity.Y > num21)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num18;
                        }
                        if ((double)Math.Abs(num21) < (double)num17 * 0.2 && ((NPC.velocity.X > 0f && num20 < 0f) || (NPC.velocity.X < 0f && num20 > 0f)))
                        {
                            if (NPC.velocity.Y > 0f)
                            {
                                NPC.velocity.Y = NPC.velocity.Y + num18 * 2f;
                            }
                            else
                            {
                                NPC.velocity.Y = NPC.velocity.Y - num18 * 2f;
                            }
                        }
                        if ((double)Math.Abs(num20) < (double)num17 * 0.2 && ((NPC.velocity.Y > 0f && num21 < 0f) || (NPC.velocity.Y < 0f && num21 > 0f)))
                        {
                            if (NPC.velocity.X > 0f)
                            {
                                NPC.velocity.X = NPC.velocity.X + num18 * 2f;
                            }
                            else
                            {
                                NPC.velocity.X = NPC.velocity.X - num18 * 2f;
                            }
                        }
                    }
                    else if (num25 > num26)
                    {
                        if (NPC.velocity.X < num20)
                        {
                            NPC.velocity.X = NPC.velocity.X + num18 * 1.1f;
                        }
                        else if (NPC.velocity.X > num20)
                        {
                            NPC.velocity.X = NPC.velocity.X - num18 * 1.1f;
                        }
                        if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)num17 * 0.5)
                        {
                            if (NPC.velocity.Y > 0f)
                            {
                                NPC.velocity.Y = NPC.velocity.Y + num18;
                            }
                            else
                            {
                                NPC.velocity.Y = NPC.velocity.Y - num18;
                            }
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < num21)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + num18 * 1.1f;
                        }
                        else if (NPC.velocity.Y > num21)
                        {
                            NPC.velocity.Y = NPC.velocity.Y - num18 * 1.1f;
                        }
                        if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)num17 * 0.5)
                        {
                            if (NPC.velocity.X > 0f)
                            {
                                NPC.velocity.X = NPC.velocity.X + num18;
                            }
                            else
                            {
                                NPC.velocity.X = NPC.velocity.X - num18;
                            }
                        }
                    }
                }
            }
            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
            if (flag2)
            {
                if (NPC.localAI[0] != 1f)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                {
                    NPC.netUpdate = true;
                }
                NPC.localAI[0] = 0f;
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / 2));

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height)) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Perforator/PerforatorHeadSmallGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int k = 0; k < 5; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SmallPerf").Type, NPC.scale);
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            name = CalamityUtils.GetTextValue("NPCs.PerforatorSmall");
            potionType = ItemID.HealingPotion;
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorBodySmall>(),
                ModContent.NPCType<PerforatorTailSmall>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void OnKill()
        {
            int heartAmt = Main.rand.Next(3) + 3;
            for (int i = 0; i < heartAmt; i++)
                Item.NewItem(NPC.GetSource_Loot(), (int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height, ItemID.Heart);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 180, true);
        }
    }
}
