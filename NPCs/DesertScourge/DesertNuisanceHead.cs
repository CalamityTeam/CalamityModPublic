using System;
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
    public class DesertNuisanceHead : ModNPC
    {
        public bool flies = false;
        public float speed = 0.085f;
        public float turnSpeed = 0.125f;
        public int maxLength = 13;
        bool TailSpawned = false;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.8f,
                PortraitScale = 0.8f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/DesertNuisance_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 50;
            value.Position.Y += 35;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.BossBar = Main.BigBossProgressBar.NeverValid;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();

            NPC.defense = 2;
            if (Main.getGoodWorld)
                NPC.defense += 18;

            NPC.width = 60;
            NPC.height = 60;
            NPC.lifeMax = BossRushEvent.BossRushActive ? 35000 : (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 2400 : 800;
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
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!TailSpawned)
                {
                    NPC.ai[2] = (float)NPC.whoAmI;
                    NPC.realLife = NPC.whoAmI;
                    int num2 = NPC.whoAmI;
                    for (int j = 0; j <= maxLength; j++)
                    {
                        int segmentType = ModContent.NPCType<DesertNuisanceBody>();
                        if (j == maxLength)
                        {
                            segmentType = ModContent.NPCType<DesertNuisanceTail>();
                        }
                        int segmentSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.position.X + (float)(NPC.width / 2)), (int)(NPC.position.Y + (float)NPC.height), segmentType, NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        Main.npc[segmentSpawn].ai[2] = (float)NPC.whoAmI;
                        Main.npc[segmentSpawn].realLife = NPC.whoAmI;
                        Main.npc[segmentSpawn].ai[1] = (float)num2;
                        Main.npc[num2].ai[0] = (float)segmentSpawn;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, segmentSpawn, 0f, 0f, 0f, 0, 0, 0);
                        num2 = segmentSpawn;
                    }
                    TailSpawned = true;
                }
            }
            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
            {
                tilePositionX = 0;
            }
            if (tileWidthPosX > Main.maxTilesX)
            {
                tileWidthPosX = Main.maxTilesX;
            }
            if (tilePositionY < 0)
            {
                tilePositionY = 0;
            }
            if (tileWidthPosY > Main.maxTilesY)
            {
                tileWidthPosY = Main.maxTilesY;
            }
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
                int directChaseDistance = 1000;
                bool shouldDirectlyChase = true;
                if (NPC.position.Y > Main.player[NPC.target].position.Y)
                {
                    for (int m = 0; m < 255; m++)
                    {
                        if (Main.player[m].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - directChaseDistance, (int)Main.player[m].position.Y - directChaseDistance, directChaseDistance * 2, directChaseDistance * 2);
                            if (rectangle.Intersects(rectangle2))
                            {
                                shouldDirectlyChase = false;
                                break;
                            }
                        }
                    }
                    if (shouldDirectlyChase)
                    {
                        shouldFly = true;
                    }
                }
            }
            else
            {
                NPC.localAI[1] = 0f;
            }
            float maxChaseSpeed = 16f;
            if (Main.player[NPC.target].dead)
            {
                shouldFly = false;
                NPC.velocity.Y = NPC.velocity.Y + 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16.0)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 1f;
                    maxChaseSpeed = 32f;
                }
                if ((double)NPC.position.Y > Main.rockLayer * 16.0)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DesertNuisanceHead>() || Main.npc[a].type == ModContent.NPCType<DesertNuisanceBody>() ||
                            Main.npc[a].type == ModContent.NPCType<DesertNuisanceTail>())
                        {
                            Main.npc[a].active = false;
                        }
                    }
                }
            }
            float speedCopy = speed;
            float turnSpeedCopy = turnSpeed;
            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
            {
                speedCopy *= 1.5f;
                turnSpeedCopy *= 1.5f;
            }
            Vector2 npcCenter = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float playerX = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float targettingPosition = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            playerX = (float)((int)(playerX / 16f) * 16);
            targettingPosition = (float)((int)(targettingPosition / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerX -= npcCenter.X;
            targettingPosition -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));
            if (!shouldFly)
            {
                NPC.TargetClosest();
                NPC.velocity.Y = NPC.velocity.Y + 0.15f;
                if (NPC.velocity.Y > maxChaseSpeed)
                {
                    NPC.velocity.Y = maxChaseSpeed;
                }
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 1.1f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 1.1f;
                    }
                }
                else if (NPC.velocity.Y == maxChaseSpeed)
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy;
                    }
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (NPC.velocity.X < 0f)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 0.9f;
                    }
                    else
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 0.9f;
                    }
                }
            }
            else
            {
                if (NPC.soundDelay == 0)
                {
                    float soundDelay = targetDistance / 40f;
                    if (soundDelay < 10f)
                    {
                        soundDelay = 10f;
                    }
                    if (soundDelay > 20f)
                    {
                        soundDelay = 20f;
                    }
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
                    {
                        NPC.velocity.X = NPC.velocity.X + turnSpeedCopy;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - turnSpeedCopy;
                    }
                    if (NPC.velocity.Y < targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + turnSpeedCopy;
                    }
                    else if (NPC.velocity.Y > targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - turnSpeedCopy;
                    }
                }
                if ((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f) || (NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f))
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy;
                    }
                    if (NPC.velocity.Y < targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + speedCopy;
                    }
                    else if (NPC.velocity.Y > targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - speedCopy;
                    }
                    if ((double)Math.Abs(targettingPosition) < (double)maxChaseSpeed * 0.2 && ((NPC.velocity.X > 0f && playerX < 0f) || (NPC.velocity.X < 0f && playerX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + speedCopy * 2f;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - speedCopy * 2f;
                        }
                    }
                    if ((double)Math.Abs(playerX) < (double)maxChaseSpeed * 0.2 && ((NPC.velocity.Y > 0f && targettingPosition < 0f) || (NPC.velocity.Y < 0f && targettingPosition > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + speedCopy * 2f;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - speedCopy * 2f;
                        }
                    }
                }
                else if (absolutePlayerX > absoluteTargetPos)
                {
                    if (NPC.velocity.X < playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X + speedCopy * 1.1f;
                    }
                    else if (NPC.velocity.X > playerX)
                    {
                        NPC.velocity.X = NPC.velocity.X - speedCopy * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                        {
                            NPC.velocity.Y = NPC.velocity.Y + speedCopy;
                        }
                        else
                        {
                            NPC.velocity.Y = NPC.velocity.Y - speedCopy;
                        }
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y + speedCopy * 1.1f;
                    }
                    else if (NPC.velocity.Y > targettingPosition)
                    {
                        NPC.velocity.Y = NPC.velocity.Y - speedCopy * 1.1f;
                    }
                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < (double)maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                        {
                            NPC.velocity.X = NPC.velocity.X + speedCopy;
                        }
                        else
                        {
                            NPC.velocity.X = NPC.velocity.X - speedCopy;
                        }
                    }
                }
            }
            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + 1.57f;
            if (shouldFly)
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

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead").Type, 0.65f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead2").Type, 0.65f);
                }
                for (int k = 0; k < 10; k++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.7f * balance);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Bleeding, 180, true);
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
