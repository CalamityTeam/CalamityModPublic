using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.StormWeaver
{
    public class StormWeaverHead : ModNPC
    {
        public static int normalIconIndex;
        public static int vulnerableIconIndex;

        internal static void LoadHeadIcons()
        {
            string normalIconPath = "CalamityMod/NPCs/StormWeaver/StormWeaverHead_Head_Boss";
            string vulnerableIconPath = "CalamityMod/NPCs/StormWeaver/StormWeaverHeadNaked_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
            normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

            CalamityMod.Instance.AddBossHeadTexture(vulnerableIconPath, -1);
            vulnerableIconIndex = ModContent.GetModBossHeadSlot(vulnerableIconPath);
        }

        private const float BoltAngleSpread = 280;
        private bool tail = false;

        // Lightning flash variables
        public float lightning = 0f;
        private float lightningDecay = 1f;
        private float lightningSpeed = 0f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Storm Weaver");
            NPCID.Sets.TrailingMode[NPC.type] = 1;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 74;
            NPC.height = 74;

            // 10% of HP is phase one
            bool notDoGFight = CalamityWorld.DoGSecondStageCountdown <= 0 || !DownedBossSystem.downedStormWeaver;
            NPC.lifeMax = notDoGFight ? 825500 : 139750;
            NPC.LifeMaxNERB(NPC.lifeMax, NPC.lifeMax, 475000);

            // If fought alone, Storm Weaver plays its own theme
            if (notDoGFight)
            {
                NPC.value = Item.buyPrice(2, 0, 0, 0);
                Music = CalamityMod.Instance.GetMusicFromMusicMod("Weaver") ?? MusicID.Boss3;
            }
            // If fought as a DoG interlude, keep the DoG music playing
            else
                Music = CalamityMod.Instance.GetMusicFromMusicMod("ScourgeofTheUniverse") ?? MusicID.Boss3;

            // Phase one settings
            CalamityGlobalNPC global = NPC.Calamity();
            NPC.defense = 150;
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;

            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;

            if (CalamityWorld.malice || BossRushEvent.BossRushActive)
                NPC.scale = 1.25f;
            else if (CalamityWorld.death)
                NPC.scale = 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale = 1.15f;
            else if (Main.expertMode)
                NPC.scale = 1.1f;

            NPC.Calamity().VulnerableToElectricity = false;
        }

        public override void BossHeadSlot(ref int index)
        {
            if (NPC.life / (float)NPC.lifeMax < 0.9f)
                index = vulnerableIconIndex;
            else
                index = normalIconIndex;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive && malice;

            if (!Main.raining)
                CalamityUtils.StartRain();

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Shed armor and start charging at the target
            bool phase2 = lifeRatio < 0.9f;

            // Start calling down frost waves from the sky in sheets
            bool phase3 = lifeRatio < 0.7f && expertMode;

            // Lightning strike flash phase and start summoning tornadoes
            bool phase4 = lifeRatio < 0.4f;

            // Become weak and cancel the storm and all other attacks
            bool phase5 = lifeRatio < 0.05f;

            // Update armored settings to naked settings
            if (phase2)
            {
                // Spawn armor gore, roar and set other crucial variables
                if (!NPC.chaseable)
                {
                    NPC.Calamity().VulnerableToHeat = true;
                    NPC.Calamity().VulnerableToCold = true;
                    NPC.Calamity().VulnerableToSickness = true;

                    if (Main.netMode != NetmodeID.Server)
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorHead1").Type, NPC.scale);

                    SoundEngine.PlaySound(SoundID.NPCDeath14, (int)NPC.Center.X, (int)NPC.Center.Y);

                    CalamityGlobalNPC global = NPC.Calamity();
                    NPC.defense = 15;
                    global.DR = 0.1f;
                    global.unbreakableDR = false;
                    NPC.chaseable = true;
                    NPC.HitSound = SoundID.NPCHit13;
                    NPC.DeathSound = SoundID.NPCDeath13;
                    NPC.frame = new Rectangle(0, 0, 62, 86);
                }
            }

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            if (NPC.alpha != 0)
            {
                for (int num934 = 0; num934 < 2; num934++)
                {
                    int num935 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 182, 0f, 0f, 100, default, 2f);
                    Main.dust[num935].noGravity = true;
                    Main.dust[num935].noLight = true;
                }
            }

            NPC.alpha -= 12;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tail && NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    int totalLength = death ? 60 : revenge ? 50 : expertMode ? 40 : 30;
                    for (int num36 = 0; num36 < totalLength; num36++)
                    {
                        int lol;
                        if (num36 >= 0 && num36 < totalLength - 1)
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<StormWeaverBody>(), NPC.whoAmI);
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + (NPC.width / 2), (int)NPC.position.Y + (NPC.height / 2), ModContent.NPCType<StormWeaverTail>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NPC.netUpdate = true;
                        Previous = lol;
                    }

                    tail = true;
                }

                if (expertMode && !phase2)
                {
                    NPC.localAI[0] += malice ? 1.5f : 1f;
                    float spawnOrbGateValue = 360f;
                    if (NPC.localAI[0] >= spawnOrbGateValue)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.netUpdate = true;

                        float orbDistance = 350f;
                        float xPos = Main.rand.NextBool(2) ? Main.player[NPC.target].position.X + orbDistance : Main.player[NPC.target].position.X - orbDistance;
                        float yPos = Main.rand.NextBool(2) ? Main.player[NPC.target].position.Y + orbDistance : Main.player[NPC.target].position.Y - orbDistance;
                        Vector2 spawnPos = new Vector2(xPos, yPos);

                        int type = ProjectileID.CultistBossLightningOrb;
                        int damage = NPC.GetProjectileDamage(type);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPos, Vector2.Zero, type, damage, 0f, Main.myPlayer);
                    }
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            if (Main.player[NPC.target].dead && NPC.life > 0)
            {
                NPC.localAI[1] = 0f;
                calamityGlobalNPC.newAI[0] = 0f;
                NPC.TargetClosest(false);

                NPC.velocity.Y -= 3f;
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                    NPC.velocity.Y -= 3f;

                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    CalamityWorld.DoGSecondStageCountdown = 0;
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = Mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                        netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                        netMessage.Send();
                    }

                    for (int num957 = 0; num957 < Main.maxNPCs; num957++)
                    {
                        if (Main.npc[num957].active && (Main.npc[num957].type == ModContent.NPCType<StormWeaverBody>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverHead>()
                            || Main.npc[num957].type == ModContent.NPCType<StormWeaverTail>()))
                        {
                            Main.npc[num957].active = false;
                        }
                    }
                }
            }

            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 10000f && NPC.life > 0)
            {
                CalamityWorld.DoGSecondStageCountdown = 0;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }

                for (int num957 = 0; num957 < Main.maxNPCs; num957++)
                {
                    if (Main.npc[num957].type == ModContent.NPCType<StormWeaverBody>()
                       || Main.npc[num957].type == ModContent.NPCType<StormWeaverHead>()
                       || Main.npc[num957].type == ModContent.NPCType<StormWeaverTail>())
                    {
                        Main.npc[num957].active = false;
                    }
                }
            }

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;

            Vector2 vector18 = new Vector2(NPC.position.X + NPC.width * 0.5f, NPC.position.Y + NPC.height * 0.5f);
            float num191 = Main.player[NPC.target].position.X + (Main.player[NPC.target].width / 2);
            float num192 = Main.player[NPC.target].position.Y + (Main.player[NPC.target].height / 2);
            float num188 = (phase2 ? 12f : 10f) + (malice ? 3f : revenge ? 1.5f : expertMode ? 1f : 0f);
            float num189 = (phase2 ? 0.24f : 0.2f) + (malice ? 0.12f : revenge ? 0.08f : expertMode ? 0.04f : 0f);

            // Start charging at the player when in phase 2
            if (phase2 && !phase5)
            {
                calamityGlobalNPC.newAI[0] += 1f;
                calamityGlobalNPC.newAI[2] += 1f;

                // Only use tornadoes in phase 4 and swap between using them or the frost waves
                bool useTornadoes = phase4 && calamityGlobalNPC.newAI[3] % 2f == 0f;

                // Gate value that decides when Storm Weaver will charge
                float chargePhaseGateValue = (int)((malice ? 280f : death ? 320f : revenge ? 360f : 400f) - (malice ? 56f : death ? 64f : revenge ? 72f : 80f) * (1f - (lifeRatio / 0.9f)));

                // Divisor that dictates whether frost waves or tornadoes will be fired or not
                float projectileShootDivisor = (int)((malice ? 180f : death ? 220f : revenge ? 260f : 300f) - (malice ? 36f : death ? 44f : revenge ? 42f : 60f) * (1f - (lifeRatio / 0.9f)));

                // Call down frost waves from the sky
                if (phase3 && !useTornadoes)
                {
                    // Let it snow while able to use the frost wave attack
                    if (Main.netMode != NetmodeID.Server)
                    {
                        Vector2 scaledSize = Main.Camera.ScaledSize;
                        Vector2 scaledPosition = Main.Camera.ScaledPosition;
                        if (Main.gamePaused || !(Main.player[NPC.target].position.Y < Main.worldSurface * 16.0))
                            return;

                        float screenWidth = Main.Camera.ScaledSize.X / Main.maxScreenW;
                        int snowDustMax = (int)(500f * screenWidth);
                        snowDustMax = (int)(snowDustMax * 3f);
                        float snowDustAmt = 50f;

                        for (int i = 0; i < snowDustAmt; i++)
                        {
                            try
                            {
                                if (!(Main.snowDust < snowDustMax * (Main.gfxQuality / 2f + 0.5f) + snowDustMax * 0.1f))
                                    return;

                                if (!(Main.rand.NextFloat() < 0.125f))
                                    continue;

                                int snowDustSpawnX = Main.rand.Next((int)scaledSize.X + 1500) - 750;
                                int snowDustSpawnY = (int)scaledPosition.Y - Main.rand.Next(50);
                                if (Main.player[NPC.target].velocity.Y > 0f)
                                    snowDustSpawnY -= (int)Main.player[NPC.target].velocity.Y;

                                if (Main.rand.Next(5) == 0)
                                    snowDustSpawnX = Main.rand.Next(500) - 500;
                                else if (Main.rand.Next(5) == 0)
                                    snowDustSpawnX = Main.rand.Next(500) + (int)scaledSize.X;

                                if (snowDustSpawnX < 0 || snowDustSpawnX > scaledSize.X)
                                    snowDustSpawnY += Main.rand.Next((int)(scaledSize.Y * 0.8)) + (int)(scaledSize.Y * 0.1);

                                snowDustSpawnX += (int)scaledPosition.X;
                                int snowDustSpawnTileX = snowDustSpawnX / 16;
                                int snowDustSpawnTileY = snowDustSpawnY / 16;
                                if (WorldGen.InWorld(snowDustSpawnTileX, snowDustSpawnTileY) && !Main.tile[snowDustSpawnTileX, snowDustSpawnTileY].HasUnactuatedTile)
                                {
                                    int dust = Dust.NewDust(new Vector2(snowDustSpawnX, snowDustSpawnY), 10, 10, 76);
                                    Main.dust[dust].scale += 0.2f;
                                    Main.dust[dust].velocity.Y = 3f + Main.rand.Next(30) * 0.1f;
                                    Main.dust[dust].velocity.Y *= Main.dust[dust].scale;
                                    if (!Main.raining)
                                    {
                                        Main.dust[dust].velocity.X = Main.windSpeedCurrent + Main.rand.Next(-10, 10) * 0.1f;
                                        Main.dust[dust].velocity.X += Main.windSpeedCurrent * 15f;
                                    }
                                    else
                                    {
                                        Main.dust[dust].velocity.X = (float)Math.Sqrt(Math.Abs(Main.windSpeedCurrent)) * Math.Sign(Main.windSpeedCurrent) * 15f + Main.rand.NextFloat() * 0.2f - 0.1f;
                                        Main.dust[dust].velocity.Y *= 0.5f;
                                    }

                                    Main.dust[dust].velocity.Y *= 1.3f;
                                    Main.dust[dust].scale += 0.2f;
                                    Main.dust[dust].velocity *= 1.5f;
                                }

                                continue;
                            }
                            catch
                            {
                            }
                        }
                    }

                    if (calamityGlobalNPC.newAI[2] % projectileShootDivisor == 0f)
                    {
                        // Dictates whether Storm Weaver will use frost or tornadoes
                        calamityGlobalNPC.newAI[3] += 1f;

                        // Play a sound on the player getting frost waves rained on them, as a telegraph
                        SoundEngine.PlaySound(SoundID.Item120, Main.player[NPC.target].Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ProjectileID.FrostWave;
                            int waveDamage = NPC.GetProjectileDamage(type);
                            int totalWaves = phase4 ? 14 : 18;
                            int shotSpacing = phase4 ? 143 : 150;
                            float projectileSpawnX = Main.player[NPC.target].Center.X - totalWaves * shotSpacing * 0.5f;

                            for (int x = 0; x < totalWaves; x++)
                            {
                                float velocityY = phase4 ? 8f : 6f;
                                switch (x)
                                {
                                    case 0:
                                    case 1:
                                        break;
                                    case 2:
                                    case 3:
                                        velocityY -= phase4 ? 1.333f : 1f;
                                        break;
                                    case 4:
                                    case 5:
                                        velocityY -= phase4 ? 2.667f : 2f;
                                        break;
                                    case 6:
                                    case 7:
                                        velocityY -= phase4 ? 4f : 3f;
                                        break;
                                    case 8:
                                    case 9:
                                        velocityY -= phase4 ? 2.667f : 4f;
                                        break;
                                    case 10:
                                    case 11:
                                        velocityY -= phase4 ? 1.333f : 3f;
                                        break;
                                    case 12:
                                    case 13:
                                        velocityY -= phase4 ? 0f : 2f;
                                        break;
                                    case 14:
                                    case 15:
                                        velocityY -= 1f;
                                        break;
                                    case 16:
                                    case 17:
                                        break;
                                }

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnX, Main.player[NPC.target].Center.Y - (phase4 ? 1200f : 1500f), 0f, velocityY * 0.5f, type, waveDamage, 0f, Main.myPlayer, 0f, velocityY);
                                projectileSpawnX += shotSpacing;
                            }
                        }
                    }
                }

                // Summon tornadoes
                if (useTornadoes)
                {
                    if (calamityGlobalNPC.newAI[2] % projectileShootDivisor == 0f)
                    {
                        // Dictates whether Storm Weaver will use frost or tornadoes
                        calamityGlobalNPC.newAI[3] += 1f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int projectileType = ModContent.ProjectileType<StormMarkHostile>();
                            int tornadoDamage = NPC.GetProjectileDamage(projectileType);
                            int totalTornadoes = 3;
                            for (int i = 0; i < totalTornadoes; i++)
                            {
                                float angle = MathHelper.TwoPi / totalTornadoes * i;
                                Vector2 spawnPosition = Main.player[NPC.target].Center + angle.ToRotationVector2() * 750f;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, Vector2.Zero, projectileType, 0, 0f, Main.myPlayer, tornadoDamage, 1f);
                            }
                        }
                    }
                }

                // Charge
                if (calamityGlobalNPC.newAI[0] >= chargePhaseGateValue)
                {
                    // Disable frost waves during charge attack
                    calamityGlobalNPC.newAI[2] = 1f;

                    NPC.localAI[3] = 60f;

                    if (NPC.localAI[1] == 0f)
                        NPC.localAI[1] = 1f;

                    if (calamityGlobalNPC.newAI[0] >= chargePhaseGateValue + 100f)
                    {
                        NPC.TargetClosest();
                        NPC.localAI[1] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    if (revenge)
                    {
                        if (NPC.localAI[1] == 2f)
                        {
                            num188 += Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.01f * (1f - (lifeRatio / 0.9f));
                            num189 += Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.0001f * (1f - (lifeRatio / 0.9f));
                            num188 *= 2f;
                            num189 *= 0.85f;

                            float stopChargeDistance = 800f * NPC.localAI[2];
                            if (stopChargeDistance < 0)
                            {
                                if (NPC.Center.X < Main.player[NPC.target].Center.X + stopChargeDistance)
                                {
                                    NPC.localAI[1] = 0f;
                                    calamityGlobalNPC.newAI[0] = 0f;
                                }
                            }
                            else
                            {
                                if (NPC.Center.X > Main.player[NPC.target].Center.X + stopChargeDistance)
                                {
                                    NPC.localAI[1] = 0f;
                                    calamityGlobalNPC.newAI[0] = 0f;
                                }
                            }
                        }

                        int dustAmt = 5;
                        for (int num1474 = 0; num1474 < dustAmt; num1474++)
                        {
                            Vector2 vector171 = Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f;
                            vector171 = vector171.RotatedBy((num1474 - (dustAmt / 2 - 1)) * (double)MathHelper.Pi / (float)dustAmt) + NPC.Center;
                            Vector2 value18 = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                            int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 206, value18.X, value18.Y, 100, default, 3f);
                            Main.dust[num1475].noGravity = true;
                            Main.dust[num1475].noLight = true;
                            Main.dust[num1475].velocity /= 4f;
                            Main.dust[num1475].velocity -= NPC.velocity;
                        }
                    }
                }
                else if (revenge)
                {
                    if (NPC.localAI[3] > 0f)
                        NPC.localAI[3] -= 1f;
                }
            }

            float num48 = num188 * 1.3f;
            float num49 = num188 * 0.7f;
            float num50 = NPC.velocity.Length();
            if (num50 > 0f)
            {
                if (num50 > num48)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= num48;
                }
                else if (num50 < num49)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= num49;
                }
            }

            if (phase2 && !phase5)
            {
                if (NPC.localAI[1] == 1f)
                {
                    // Play lightning sound on the target if not in phase 3
                    Vector2 soundCenter = Main.player[NPC.target].Center;

                    // Play lightning sound on all nearby players if in phase 3
                    if (phase4)
                    {
                        if (Main.player[Main.myPlayer].active && !Main.player[Main.myPlayer].dead && Vector2.Distance(Main.player[Main.myPlayer].Center, NPC.Center) < 2800f)
                        {
                            soundCenter = Main.player[Main.myPlayer].Center;

                            SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/LightningStrike"), (int)soundCenter.X, (int)soundCenter.Y);

                            if (Main.netMode != NetmodeID.Server)
                            {
                                // Set how quickly the lightning flash dissipates
                                lightningDecay = Main.rand.NextFloat() * 0.05f + 0.008f;

                                // Set how quickly the lightning flash intensifies
                                lightningSpeed = Main.rand.NextFloat() * 0.05f + 0.05f;
                            }
                        }
                    }
                    else
                        SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/LightningStrike"), (int)soundCenter.X, (int)soundCenter.Y);

                    NPC.localAI[1] = 2f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int speed2 = revenge ? 8 : 7;
                        float spawnX2 = NPC.Center.X > Main.player[NPC.target].Center.X ? 1000f : -1000f;
                        float spawnY2 = -1000f + Main.player[NPC.target].Center.Y;
                        Vector2 baseSpawn = new Vector2(spawnX2 + Main.player[NPC.target].Center.X, spawnY2);
                        Vector2 baseVelocity = Main.player[NPC.target].Center - baseSpawn;
                        baseVelocity.Normalize();
                        baseVelocity *= speed2;

                        int boltProjectiles = 3;
                        for (int i = 0; i < boltProjectiles; i++)
                        {
                            Vector2 source = baseSpawn;
                            source.X += i * 30f - (boltProjectiles * 15f);
                            Vector2 velocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-BoltAngleSpread / 2 + (BoltAngleSpread * i / boltProjectiles)));
                            velocity.X = velocity.X + 3f * Main.rand.NextFloat() - 1.5f;
                            Vector2 vector94 = Main.player[NPC.target].Center - source;
                            float ai = Main.rand.Next(100);
                            int type = ProjectileID.CultistBossLightningOrbArc;
                            int damage = NPC.GetProjectileDamage(type);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), source, velocity, type, damage, 0f, Main.myPlayer, vector94.ToRotation(), ai);
                        }
                    }

                    if (revenge)
                        NPC.velocity = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * (num188 + Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.01f * (1f - (lifeRatio / 0.9f))) * 2f;

                    float chargeDirection = 0;
                    if (NPC.velocity.X < 0f)
                        chargeDirection = -1f;
                    else if (NPC.velocity.X > 0f)
                        chargeDirection = 1f;

                    NPC.localAI[2] = chargeDirection;
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

            if ((NPC.velocity.X > 0f && num191 > 0f) || (NPC.velocity.X < 0f && num191 < 0f) || (NPC.velocity.Y > 0f && num192 > 0f) || (NPC.velocity.Y < 0f && num192 < 0f))
            {
                if (NPC.velocity.X < num191)
                {
                    NPC.velocity.X = NPC.velocity.X + num189;
                }
                else
                {
                    if (NPC.velocity.X > num191)
                        NPC.velocity.X = NPC.velocity.X - num189;
                }

                if (NPC.velocity.Y < num192)
                {
                    NPC.velocity.Y = NPC.velocity.Y + num189;
                }
                else
                {
                    if (NPC.velocity.Y > num192)
                        NPC.velocity.Y = NPC.velocity.Y - num189;
                }

                if (Math.Abs(num192) < num188 * 0.2 && ((NPC.velocity.X > 0f && num191 < 0f) || (NPC.velocity.X < 0f && num191 > 0f)))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y = NPC.velocity.Y + num189 * 2f;
                    else
                        NPC.velocity.Y = NPC.velocity.Y - num189 * 2f;
                }

                if (Math.Abs(num191) < num188 * 0.2 && ((NPC.velocity.Y > 0f && num192 < 0f) || (NPC.velocity.Y < 0f && num192 > 0f)))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X = NPC.velocity.X + num189 * 2f;
                    else
                        NPC.velocity.X = NPC.velocity.X - num189 * 2f;
                }
            }
            else
            {
                if (num196 > num197)
                {
                    if (NPC.velocity.X < num191)
                        NPC.velocity.X = NPC.velocity.X + num189 * 1.1f;
                    else if (NPC.velocity.X > num191)
                        NPC.velocity.X = NPC.velocity.X - num189 * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num188 * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = NPC.velocity.Y + num189;
                        else
                            NPC.velocity.Y = NPC.velocity.Y - num189;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < num192)
                        NPC.velocity.Y = NPC.velocity.Y + num189 * 1.1f;
                    else if (NPC.velocity.Y > num192)
                        NPC.velocity.Y = NPC.velocity.Y - num189 * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < num188 * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X = NPC.velocity.X + num189;
                        else
                            NPC.velocity.X = NPC.velocity.X - num189;
                    }
                }
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

            if (phase5)
            {
                // Become weak and stop the storm when in phase 4
                NPC.localAI[1] = 0f;
                calamityGlobalNPC.newAI[0] = 0f;
                CalamityMod.StopRain();
            }
            else if (phase4)
            {
                // Adjust lightning flash variables when in phase 3
                if (Main.netMode != NetmodeID.Server)
                {
                    if (lightningSpeed > 0f)
                    {
                        lightning += lightningSpeed;
                        if (lightning >= 1f)
                        {
                            lightning = 1f;
                            lightningSpeed = 0f;
                        }
                    }
                    else if (lightning > 0f)
                        lightning -= lightningDecay;
                }

                // Start a storm when in third phase
                if (Main.netMode == NetmodeID.MultiplayerClient || (Main.netMode == NetmodeID.SinglePlayer && Main.gameMenu) || calamityGlobalNPC.newAI[1] > 0f)
                    return;

                CalamityUtils.StartRain(true, true);
                calamityGlobalNPC.newAI[1] = 1f;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                NPC.Opacity = 1f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.9f;
            Texture2D texture2D15 = phase2 ? ModContent.Request<Texture2D>("CalamityMod/NPCs/StormWeaver/StormWeaverHeadNaked").Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2(texture2D15.Width / 2, texture2D15.Height / 2);
            Color color36 = Color.White;
            float amount9 = 0.5f;
            int num153 = 5;

            if (CalamityConfig.Instance.Afterimages)
            {
                for (int num155 = 1; num155 < num153; num155 += 2)
                {
                    Color color38 = drawColor;

                    if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
                    {
                        if (NPC.Calamity().newAI[0] > 280f)
                            color38 = Color.Lerp(color38, Color.Cyan, MathHelper.Clamp((NPC.Calamity().newAI[0] - 280f) / 120f, 0f, 1f));
                        else if (NPC.localAI[3] > 0f)
                            color38 = Color.Lerp(color38, Color.Cyan, MathHelper.Clamp(NPC.localAI[3] / 60f, 0f, 1f));
                    }

                    color38 = Color.Lerp(color38, color36, amount9);
                    color38 = NPC.GetAlpha(color38);
                    color38 *= (num153 - num155) / 15f;
                    Vector2 vector41 = NPC.oldPos[num155] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    vector41 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
                    vector41 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, vector41, NPC.frame, color38, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                if (NPC.Calamity().newAI[0] > 280f)
                    color = Color.Lerp(color, Color.Cyan, MathHelper.Clamp((NPC.Calamity().newAI[0] - 280f) / 120f, 0f, 1f));
                else if (NPC.localAI[3] > 0f)
                    color = Color.Lerp(color, Color.Cyan, MathHelper.Clamp(NPC.localAI[3] / 60f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            int buffDuration = NPC.Calamity().newAI[0] >= 400f ? 480 : 240;
            player.AddBuff(BuffID.Electrified, buffDuration, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeHead1").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeHead2").Type, NPC.scale);
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 30;
                NPC.height = 30;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int num621 = 0; num621 < 20; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int num623 = 0; num623 < 40; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }

        public override bool CheckDead()
        {
            for (int num569 = 0; num569 < Main.maxNPCs; num569++)
            {
                if (Main.npc[num569].active && (Main.npc[num569].type == ModContent.NPCType<StormWeaverBody>() || Main.npc[num569].type == ModContent.NPCType<StormWeaverTail>()))
                    Main.npc[num569].life = 0;
            }

            return true;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<SupremeHealingPotion>();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<StormWeaverHead>(),
                ModContent.NPCType<StormWeaverBody>(),
                ModContent.NPCType<StormWeaverTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public static bool AtFullStrength() => !DownedBossSystem.downedSignus || CalamityWorld.DoGSecondStageCountdown <= 0;

        public static bool LastSentinelKilled() => !DownedBossSystem.downedSignus && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedCeaselessVoid;

        public override void OnKill()
        {
            bool fullStrength = AtFullStrength();
            if (fullStrength)
                CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If DoG's fight is active, set the timer for Signus' phase
            if (CalamityWorld.DoGSecondStageCountdown > 7260)
            {
                CalamityWorld.DoGSecondStageCountdown = 7260;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = Mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(CalamityWorld.DoGSecondStageCountdown);
                    netMessage.Send();
                }
            }

            // Mark Ceaseless Void as dead
            if (fullStrength)
            {
                DownedBossSystem.downedStormWeaver = true;
                CalamityNetcode.SyncWorld();
            }
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var fullStrengthDrops = npcLoot.DefineConditionalDropSet(AtFullStrength);
            fullStrengthDrops.Add(ItemDropRule.BossBag(ModContent.ItemType<StormWeaverBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            fullStrengthDrops.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<StormDragoon>(),
                    ModContent.ItemType<TheStorm>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<Thunderstorm>(), 10);

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<ArmoredShell>(), 1, 5, 8));

                // Vanity
                normalOnly.Add(ModContent.ItemType<StormWeaverMask>(), 7);
                normalOnly.Add(ModContent.ItemType<LittleLight>(), 10);
                normalOnly.Add(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerHelm>(), 20).
                    OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerChestplate>())).
                    OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerLeggings>())));
            }

            fullStrengthDrops.Add(ModContent.ItemType<WeaverTrophy>(), 10);

            // Lore
            npcLoot.AddConditionalPerPlayer(LastSentinelKilled, ModContent.ItemType<KnowledgeSentinels>());
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
        }
    }
}
