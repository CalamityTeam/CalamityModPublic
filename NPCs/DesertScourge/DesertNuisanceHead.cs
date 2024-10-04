using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [AutoloadBossHead]
    [LongDistanceNetSync]
    public class DesertNuisanceHead : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

        public bool flies = false;
        private bool tailSpawned = false;

        public const float SegmentVelocity_Expert = 14f;
        public const float SegmentVelocity_Master = 16.5f;
        public const float SegmentVelocity_GoodWorld = 19f;
        public const float SegmentVelocity_ZenithSeed = 21.5f;

        public const float OpenMouthForBiteDistance = 220f;

        private const int OpenMouthStopFrame = 4;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 7;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitScale = 0.6f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/DesertNuisance_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 45;
            value.Position.Y += 30;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            NPCID.Sets.CantTakeLunchMoney[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();

            NPC.defense = 3;
            if (Main.getGoodWorld)
                NPC.defense += 19;

            NPC.width = 88;
            NPC.height = 88;

            NPC.LifeMaxNERB(1500, 1800, 40000);
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                NPC.lifeMax = 4800;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.Opacity = 0f;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;

            if (Main.zenithWorld)
                NPC.scale *= 2;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            int associatedNPCType = ModContent.NPCType<DesertScourgeHead>();
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[associatedNPCType], quickUnlock: true);

            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DesertNuisance")
            });
        }

        public override void AI()
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool masterMode = Main.masterMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Become angry when the other Nuisance dies.
            bool getMad = (!NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHeadYoung>()) && revenge) || death;

            // Enrage
            if (!Main.player[NPC.target].ZoneDesert && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : getMad ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            // Percent life remaining.
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            float speed = death ? 0.18f : 0.16f;
            float turnSpeed = death ? 0.26f : 0.22f;
            speed += speed * 0.4f * (1f - lifeRatio);
            turnSpeed += turnSpeed * 0.4f * (1f - lifeRatio);
            speed += 0.16f * enrageScale;
            turnSpeed += 0.22f * enrageScale;

            if (Main.getGoodWorld)
            {
                speed *= 1.1f;
                turnSpeed *= 1.2f;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tailSpawned && NPC.ai[0] == 0f)
                {
                    int previous = NPC.whoAmI;
                    int minLength = 8;
                    if (Main.getGoodWorld)
                        minLength *= 2;

                    int bodyTypeAIVariable = 0;
                    for (int i = 0; i < minLength + 1; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength)
                        {
                            if (i == 0)
                                bodyTypeAIVariable = 0;
                            else if (i == minLength - 1)
                                bodyTypeAIVariable = 30;
                            else if (i % 2 == 0)
                                bodyTypeAIVariable = 20;
                            else
                                bodyTypeAIVariable = 10;

                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertNuisanceBody>(), NPC.whoAmI);
                            Main.npc[lol].ai[3] = bodyTypeAIVariable;
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertNuisanceTail>(), NPC.whoAmI);

                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[1] = previous;
                        Main.npc[previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        previous = lol;
                    }

                    tailSpawned = true;
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
                tilePositionX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePositionY < 0)
                tilePositionY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            bool shouldFly = false;
            if (!shouldFly)
            {
                for (int k = tilePositionX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePositionY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!shouldFly)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int directChaseDistance = revenge ? 500 : 1000;
                bool shouldDirectlyChase = true;
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    foreach (Player plr in Main.ActivePlayers)
                    {
                        Rectangle rectangle2 = new Rectangle((int)plr.position.X - directChaseDistance, (int)plr.position.Y - directChaseDistance, directChaseDistance * 2, directChaseDistance * 2);
                        if (rectangle.Intersects(rectangle2))
                        {
                            shouldDirectlyChase = false;
                            break;
                        }
                    }
                    if (shouldDirectlyChase)
                        shouldFly = true;
                }
            }
            else
                NPC.localAI[1] = 0f;

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = 1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = -1;

            float maxChaseSpeed = Main.zenithWorld ? SegmentVelocity_ZenithSeed :
                Main.getGoodWorld ? SegmentVelocity_GoodWorld :
                masterMode ? SegmentVelocity_Master :
                SegmentVelocity_Expert;
            maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);
            if (masterMode)
                maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);

            if (Main.player[NPC.target].dead)
            {
                shouldFly = false;
                NPC.velocity.Y += 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16D)
                {
                    NPC.velocity.Y += 1f;
                    maxChaseSpeed *= 2f;
                }

                if ((double)NPC.position.Y > Main.rockLayer * 16D)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DesertNuisanceHead>() || Main.npc[a].type == ModContent.NPCType<DesertNuisanceBody>() || Main.npc[a].type == ModContent.NPCType<DesertNuisanceTail>())
                            Main.npc[a].active = false;
                    }
                }
            }

            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
            {
                speed *= 1.5f;
                turnSpeed *= 1.5f;
            }

            Vector2 npcCenter = NPC.Center;
            float playerX = Main.player[NPC.target].Center.X;
            float targettingPosition = Main.player[NPC.target].Center.Y;
            playerX = (float)((int)(playerX / 16f) * 16);
            targettingPosition = (float)((int)(targettingPosition / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerX -= npcCenter.X;
            targettingPosition -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));

            if (!shouldFly)
            {
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y > 0f && Math.Abs(NPC.Center.Y - Main.player[NPC.target].Center.Y) > 180f)
                    NPC.velocity.Y += 0.05f;

                if (NPC.velocity.Y > maxChaseSpeed)
                    NPC.velocity.Y = maxChaseSpeed;

                // This bool exists to stop the strange wiggle behavior when worms are falling down
                bool slowXVelocity = Math.Abs(NPC.velocity.X) > speed;
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChaseSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= speed * 1.1f;
                    else
                        NPC.velocity.X += speed * 1.1f;
                }
                else if (NPC.velocity.Y == maxChaseSpeed)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < playerX)
                            NPC.velocity.X += speed;
                        else if (NPC.velocity.X > playerX)
                            NPC.velocity.X -= speed;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X += speed * 0.9f;
                        else
                            NPC.velocity.X -= speed * 0.9f;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
            }
            else
            {
                if (NPC.soundDelay == 0)
                {
                    float soundDelay = targetDistance / 40f;
                    if (soundDelay < 10f)
                        soundDelay = 10f;
                    if (soundDelay > 20f)
                        soundDelay = 20f;

                    NPC.soundDelay = (int)soundDelay;
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }

                targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));
                float absolutePlayerX = Math.Abs(playerX);
                float absoluteTargetPos = Math.Abs(targettingPosition);
                float timeToReachTarget = maxChaseSpeed / targetDistance;
                playerX *= timeToReachTarget;
                targettingPosition *= timeToReachTarget;

                if (((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f)) && ((NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f)))
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X += turnSpeed;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X -= turnSpeed;

                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y += turnSpeed;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y -= turnSpeed;
                }

                if ((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f) || (NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f))
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X += speed;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X -= speed;

                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y += speed;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y -= speed;

                    if ((double)Math.Abs(targettingPosition) < maxChaseSpeed * 0.2 && ((NPC.velocity.X > 0f && playerX < 0f) || (NPC.velocity.X < 0f && playerX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += speed * 2f;
                        else
                            NPC.velocity.Y -= speed * 2f;
                    }

                    if ((double)Math.Abs(playerX) < maxChaseSpeed * 0.2 && ((NPC.velocity.Y > 0f && targettingPosition < 0f) || (NPC.velocity.Y < 0f && targettingPosition > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += speed * 2f;
                        else
                            NPC.velocity.X -= speed * 2f;
                    }
                }
                else if (absolutePlayerX > absoluteTargetPos)
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X += speed * 1.1f;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X -= speed * 1.1f;

                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += speed;
                        else
                            NPC.velocity.Y -= speed;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y += speed * 1.1f;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y -= speed * 1.1f;

                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += speed;
                        else
                            NPC.velocity.X -= speed;
                    }
                }
            }

            Vector2 destination = Main.player[NPC.target].Center;
            if (NPC.Distance(destination) > 1000f)
                NPC.velocity += (destination - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = maxChaseSpeed * 0.25f;
            float minimalDamageVelocity = maxChaseSpeed * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + MathHelper.PiOver2;

            if (shouldFly)
            {
                if (NPC.localAI[0] != 1f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }

            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                NPC.netUpdate = true;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float hitboxTopLeft = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float hitboxTopRight = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float hitboxBotLeft = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float hitboxBotRight = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = hitboxTopLeft;
            if (hitboxTopRight < minDist)
                minDist = hitboxTopRight;
            if (hitboxBotLeft < minDist)
                minDist = hitboxBotLeft;
            if (hitboxBotRight < minDist)
                minDist = hitboxBotRight;

            return minDist <= 50f * NPC.scale;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeNuisanceHead").Type, NPC.scale);

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override void FindFrame(int frameHeight)
        {
            // Open mouth to prepare for a nibble ;3
            bool openMouth = NPC.Distance(Main.player[NPC.target].Center) < OpenMouthForBiteDistance &&
                (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation() &&
                NPC.ai[3] == 0f;

            bool closeMouthBite = NPC.ai[3] == 1f;
            if (closeMouthBite)
            {
                // Force mouth open for a bite if it's not open.
                if (NPC.frame.Y < frameHeight * OpenMouthStopFrame)
                {
                    NPC.frame.Y = frameHeight * OpenMouthStopFrame;
                    NPC.frameCounter = 0D;
                }

                NPC.frameCounter += 1D;
                if (NPC.frameCounter > 4D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                {
                    NPC.ai[3] = 2f;
                    NPC.netUpdate = true;
                    NPC.netSpam = 0;
                    NPC.frame.Y = 0;
                }
            }
            else if (openMouth)
            {
                NPC.frameCounter += 1D;
                if (NPC.frameCounter > 4D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y >= frameHeight * OpenMouthStopFrame)
                    NPC.frame.Y = frameHeight * OpenMouthStopFrame;
            }

            // Close mouth.
            else
            {
                if (NPC.frame.Y > 0)
                {
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
                    {
                        NPC.frame.Y = 0;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    else
                    {
                        NPC.frameCounter += 1D;
                        if (NPC.frameCounter > 4D)
                        {
                            NPC.frame.Y -= frameHeight;
                            NPC.frameCounter = 0D;
                        }
                    }
                }
                else
                {
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                    NPC.netSpam = 0;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(BuffID.Bleeding, 180);
                NPC.ai[3] = 1f;
                NPC.netUpdate = true;
                NPC.netSpam = 0;
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.Orange * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }
    }
}
