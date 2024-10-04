using System;
using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Sounds;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.StormWeaver
{
    [LongDistanceNetSync]
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

        public static readonly SoundStyle ArmorShedSound = new("CalamityMod/Sounds/Custom/WeaverArmorShed");
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/WeaverDeath");

        // Lightning flash variables
        public float lightning = 0f;
        private float lightningDecay = 1f;
        private float lightningSpeed = 0f;

        public static Asset<Texture2D> Phase2Texture;

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.85f,
                PortraitScale = 0.75f,
                CustomTexturePath = "CalamityMod/ExtraTextures/Bestiary/StormWeaver_Bestiary",
                PortraitPositionXOverride = 40,
                PortraitPositionYOverride = 40
            };
            value.Position.X += 70;
            value.Position.Y += 55;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "Naked", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 5f;
            NPC.width = 74;
            NPC.height = 74;
            NPC.lifeMax = 825000;
            NPC.LifeMaxNERB(NPC.lifeMax, NPC.lifeMax, 500000);
            NPC.value = Item.buyPrice(2, 0, 0, 0);

            // Phase one settings
            CalamityGlobalNPC global = NPC.Calamity();
            NPC.defense = 150;
            global.DR = 0.999999f;
            global.unbreakableDR = true;
            NPC.chaseable = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = DeathSound;

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

            if (BossRushEvent.BossRushActive)
                NPC.scale *= 1.25f;
            else if (CalamityWorld.death)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 0.7f;

            NPC.Calamity().VulnerableToElectricity = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.StormWeaver")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            if (NPC.life / (float)NPC.lifeMax < 0.8f)
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

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();
            else if (!Main.raining && !bossRush)
                CalamityUtils.StartRain();

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Shed armor and start charging at the target
            bool phase2 = lifeRatio < 0.8f;

            // Start calling down frost waves from the sky in sheets and stop firing lightning during the charge
            bool phase3 = lifeRatio < 0.55f;

            // Lightning strike flash phase, stop charging and start summoning tornadoes
            bool phase4 = lifeRatio < 0.3f;

            // Update armored settings to naked settings
            if (phase2 && (!CalamityWorld.LegendaryMode || !revenge))
            {
                // Spawn armor gore, roar and set other crucial variables
                if (!NPC.chaseable)
                {
                    NPC.Calamity().VulnerableToHeat = true;
                    NPC.Calamity().VulnerableToCold = true;
                    NPC.Calamity().VulnerableToSickness = true;

                    if (Main.netMode != NetmodeID.Server)
                        Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWArmorHead1").Type, NPC.scale);

                    SoundEngine.PlaySound(ArmorShedSound, NPC.Center);

                    CalamityGlobalNPC global = NPC.Calamity();
                    NPC.defense = 20;
                    global.DR = 0.2f;
                    global.unbreakableDR = false;
                    NPC.chaseable = true;
                    NPC.HitSound = SoundID.NPCHit13;
                    NPC.frame = new Rectangle(0, 0, 62, 86);
                }
            }

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
                for (int i = 0; i < 2; i++)
                {
                    int redDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.TheDestroyer, 0f, 0f, 100, default, 2f);
                    Main.dust[redDust].noGravity = true;
                    Main.dust[redDust].noLight = true;
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
                    int npcCounts = 0;
                    if (Main.zenithWorld) // use up every remaining npc but 20 for safety in the zenith seed
                    {
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (!Main.npc[i].active)
                                npcCounts++;
                        }

                        totalLength = npcCounts - 20;
                    }

                    for (int segments = 0; segments < totalLength; segments++)
                    {
                        int lol;
                        if (segments >= 0 && segments < totalLength - 1)
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

                // Used for body and tail projectile firing timings in phase 1
                if (!phase2 || (CalamityWorld.LegendaryMode && revenge))
                    NPC.localAI[0] += 1f;
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            if (Main.player[NPC.target].dead && NPC.life > 0)
            {
                NPC.localAI[1] = 0f;
                calamityGlobalNPC.newAI[0] = 0f;
                calamityGlobalNPC.newAI[2] = 0f;
                NPC.TargetClosest(false);

                NPC.velocity.Y -= 3f;
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                    NPC.velocity.Y -= 3f;

                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    foreach (NPC n in Main.ActiveNPCs)
                    {
                        if ((n.type == ModContent.NPCType<StormWeaverBody>()
                            || n.type == ModContent.NPCType<StormWeaverHead>()
                            || n.type == ModContent.NPCType<StormWeaverTail>()))
                        {
                            n.active = false;
                        }
                    }
                }
            }

            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > 10000f && NPC.life > 0)
            {
                foreach (NPC n in Main.ActiveNPCs)
                {
                    if (n.type == ModContent.NPCType<StormWeaverBody>()
                       || n.type == ModContent.NPCType<StormWeaverHead>()
                       || n.type == ModContent.NPCType<StormWeaverTail>())
                    {
                        n.active = false;
                    }
                }
            }

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;

            Vector2 npcCenter = NPC.Center;
            float targetCenterX = Main.player[NPC.target].Center.X;
            float targetCenterY = Main.player[NPC.target].Center.Y;
            float velocity = (phase2 ? 12f : 10f) + (bossRush ? 3f : revenge ? 1.5f : expertMode ? 1f : 0f);
            float acceleration = (phase2 ? 0.24f : 0.2f) + (bossRush ? 0.12f : revenge ? 0.08f : expertMode ? 0.04f : 0f);

            // Start charging at the player when in phase 2
            if (phase2)
            {
                if (!phase4)
                {
                    calamityGlobalNPC.newAI[0] += 1f;
                }
                else
                {
                    NPC.localAI[1] = 0f;
                    if (NPC.localAI[3] > 0f)
                        NPC.localAI[3] -= 1f;

                    calamityGlobalNPC.newAI[0] = 0f;
                }

                calamityGlobalNPC.newAI[2] += 1f;

                // Only use tornadoes in phase 4 and swap between using them or the frost waves
                bool useTornadoes = calamityGlobalNPC.newAI[3] % 2f != 0f;

                // Gate value that decides when Storm Weaver will charge
                float chargePhaseGateValue = bossRush ? 280f : death ? 320f : revenge ? 340f : expertMode ? 360f : 400f;
                if (!phase3)
                    chargePhaseGateValue *= 0.5f;
                if (phase4 && expertMode)
                    chargePhaseGateValue *= 0.9f;

                // Gate value for when Storm Weaver fires projectiles
                float projectileGateValue = (int)(chargePhaseGateValue * 0.25f);

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

                                if (Main.rand.NextBool(5))
                                    snowDustSpawnX = Main.rand.Next(500) - 500;
                                else if (Main.rand.NextBool(5))
                                    snowDustSpawnX = Main.rand.Next(500) + (int)scaledSize.X;

                                if (snowDustSpawnX < 0 || snowDustSpawnX > scaledSize.X)
                                    snowDustSpawnY += Main.rand.Next((int)(scaledSize.Y * 0.8)) + (int)(scaledSize.Y * 0.1);

                                snowDustSpawnX += (int)scaledPosition.X;
                                int snowDustSpawnTileX = snowDustSpawnX / 16;
                                int snowDustSpawnTileY = snowDustSpawnY / 16;
                                if (WorldGen.InWorld(snowDustSpawnTileX, snowDustSpawnTileY) && !Main.tile[snowDustSpawnTileX, snowDustSpawnTileY].HasUnactuatedTile)
                                {
                                    int dust = Dust.NewDust(new Vector2(snowDustSpawnX, snowDustSpawnY), 10, 10, DustID.Snow);
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

                    if (calamityGlobalNPC.newAI[2] >= projectileGateValue)
                    {
                        calamityGlobalNPC.newAI[2] = -projectileGateValue * 4f;

                        // Dictates whether Storm Weaver will use frost or tornadoes
                        if (phase4)
                        {
                            // Lightning strike
                            if (!Main.DisableIntenseVisualEffects)
                            {
                                if (Main.netMode != NetmodeID.Server)
                                {
                                    if (lightningSpeed == 0f)
                                    {
                                        lightningDecay = Main.rand.NextFloat() * 0.05f + 0.008f;
                                        lightningSpeed = Main.rand.NextFloat() * 0.05f + 0.05f;
                                    }
                                }
                            }

                            calamityGlobalNPC.newAI[3] += 1f;
                        }

                        // Play a sound on the player getting frost waves rained on them, as a telegraph
                        SoundEngine.PlaySound(SoundID.Item120, Main.player[NPC.target].Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ProjectileID.FrostWave;
                            int waveDamage = NPC.GetProjectileDamage(type);
                            int totalWaves = death ? (phase4 ? 27 : 25) : (phase4 ? 25 : 23);
                            int shotSpacing = death ? (phase4 ? 185 : 200) : (phase4 ? 200 : 215);
                            float projectileSpawnX = Main.player[NPC.target].Center.X - totalWaves * shotSpacing * 0.5f;

                            // Start fast at index 0, become slower as each projectile spawns and then become faster past the central wave
                            int centralWave = totalWaves / 2;
                            float velocityY = 8f;
                            int wavePatternType = revenge ? Main.rand.Next(3) : expertMode ? Main.rand.Next(2) + 1 : 2;
                            float delayBeforeFiring = -60f;
                            for (int x = 0; x < totalWaves; x++)
                            {
                                switch (wavePatternType)
                                {
                                    // Starts at 8, central point is 6 and the end is 8
                                    case 0:

                                        if (x != 0)
                                        {
                                            if (x <= centralWave)
                                                velocityY -= 1f / 6f;
                                            else
                                                velocityY += 1f / 6f;
                                        }

                                        break;

                                    // Starts at 8 and alternates between 6 and 8
                                    case 1:

                                        if (x != 0)
                                        {
                                            if (x % 2 == 0)
                                                velocityY += 2f;
                                            else
                                                velocityY -= 2f;
                                        }

                                        break;

                                    // Flat line of slower waves
                                    case 2:

                                        velocityY = 7f;

                                        break;
                                }

                                // Telegraph is active for 60 frames
                                // Frost Waves start moving after 30 frames
                                // Frost Waves take 30 frames to reach full velocity
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnX, Main.player[NPC.target].Center.Y - 1600f, 0f, velocityY * 0.5f, ModContent.ProjectileType<StormWeaverFrostWaveTelegraph>(), 0, 0f, Main.myPlayer, 0f, velocityY);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnX, Main.player[NPC.target].Center.Y - 1600f, 0f, velocityY * 0.1f, type, waveDamage, 0f, Main.myPlayer, delayBeforeFiring, velocityY);
                                projectileSpawnX += shotSpacing;
                            }
                        }
                    }
                }

                // Summon tornadoes
                if (useTornadoes)
                {
                    if (calamityGlobalNPC.newAI[2] >= projectileGateValue)
                    {
                        calamityGlobalNPC.newAI[2] = -projectileGateValue * 4f;

                        // Dictates whether Storm Weaver will use frost or tornadoes
                        calamityGlobalNPC.newAI[3] += 1f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int projectileType = ModContent.ProjectileType<StormMarkHostile>();
                            int tornadoDamage = NPC.GetProjectileDamage(projectileType);
                            int totalTornadoes = revenge ? 7 : expertMode ? 5 : 3;
                            float spawnDistance = revenge ? 750f : expertMode ? 900f : 1050f;
                            for (int i = 0; i < totalTornadoes; i++)
                            {
                                Vector2 spawnPosition = Main.player[NPC.target].Center + Vector2.UnitX * spawnDistance * (i - totalTornadoes / 2);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnPosition, Vector2.Zero, projectileType, 0, 0f, Main.myPlayer, tornadoDamage, 1f);
                            }
                        }
                    }
                }

                // Charge
                if (!phase4)
                {
                    if (calamityGlobalNPC.newAI[0] >= chargePhaseGateValue)
                    {
                        NPC.localAI[3] = 60f;

                        if (NPC.localAI[1] == 0f)
                            NPC.localAI[1] = 1f;

                        if (calamityGlobalNPC.newAI[0] >= chargePhaseGateValue + 100f)
                        {
                            NPC.TargetClosest();
                            NPC.localAI[1] = 0f;
                            calamityGlobalNPC.newAI[0] = 0f;
                        }

                        if (NPC.localAI[1] == 2f)
                        {
                            velocity += Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.01f * (1f - (lifeRatio / 0.8f));
                            acceleration += Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.0001f * (1f - (lifeRatio / 0.8f));
                            velocity *= 2f;
                            acceleration *= 0.85f;

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
                        for (int k = 0; k < dustAmt; k++)
                        {
                            Vector2 dustRotation = Vector2.Normalize(NPC.velocity) * new Vector2((NPC.width + 50) / 2f, NPC.height) * 0.75f;
                            dustRotation = dustRotation.RotatedBy((k - (dustAmt / 2 - 1)) * (double)MathHelper.Pi / (float)dustAmt) + NPC.Center;
                            Vector2 randDustMovement = ((float)(Main.rand.NextDouble() * MathHelper.Pi) - MathHelper.PiOver2).ToRotationVector2() * Main.rand.Next(3, 8);
                            int bluishDust = Dust.NewDust(dustRotation + randDustMovement, 0, 0, DustID.UnusedWhiteBluePurple, randDustMovement.X, randDustMovement.Y, 100, default, 3f);
                            Main.dust[bluishDust].noGravity = true;
                            Main.dust[bluishDust].noLight = true;
                            Main.dust[bluishDust].velocity /= 4f;
                            Main.dust[bluishDust].velocity -= NPC.velocity;
                        }
                    }
                    else
                    {
                        if (NPC.localAI[3] > 0f)
                            NPC.localAI[3] -= 1f;
                    }
                }
            }

            if (Main.getGoodWorld)
            {
                velocity *= 1.4f;
                acceleration *= 1.4f;
            }

            float fasterVelMult = velocity * 1.3f;
            float slowerVelMult = velocity * 0.7f;
            float weaverSpeed = NPC.velocity.Length();
            if (weaverSpeed > 0f)
            {
                if (weaverSpeed > fasterVelMult)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= fasterVelMult;
                }
                else if (weaverSpeed < slowerVelMult)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= slowerVelMult;
                }
            }

            if (phase2 && !phase4)
            {
                if (NPC.localAI[1] == 1f)
                {
                    // Play lightning sound on the target
                    Vector2 soundCenter = Main.player[NPC.target].Center;
                    SoundEngine.PlaySound(CommonCalamitySounds.LightningSound, soundCenter);

                    NPC.localAI[1] = 2f;

                    if (!phase3)
                    {
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
                                Vector2 boltVelocity = baseVelocity.RotatedBy(MathHelper.ToRadians(-BoltAngleSpread / 2 + (BoltAngleSpread * i / boltProjectiles)));
                                boltVelocity.X = boltVelocity.X + 3f * Main.rand.NextFloat() - 1.5f;
                                Vector2 aimDirection = Main.player[NPC.target].Center - source;
                                float ai = Main.rand.Next(100);
                                int type = ProjectileID.CultistBossLightningOrbArc;
                                int damage = NPC.GetProjectileDamage(type);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, boltVelocity, type, damage, 0f, Main.myPlayer, aimDirection.ToRotation(), ai);
                            }
                        }
                    }

                    if (revenge)
                        NPC.velocity = Vector2.Normalize(Main.player[NPC.target].Center - NPC.Center) * (velocity + Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) * 0.01f * (1f - (lifeRatio / 0.8f))) * 2f;

                    float chargeDirection = 0;
                    if (NPC.velocity.X < 0f)
                        chargeDirection = -1f;
                    else if (NPC.velocity.X > 0f)
                        chargeDirection = 1f;

                    NPC.localAI[2] = chargeDirection;
                }
            }

            targetCenterX = (int)(targetCenterX / 16f) * 16;
            targetCenterY = (int)(targetCenterY / 16f) * 16;
            npcCenter.X = (int)(npcCenter.X / 16f) * 16;
            npcCenter.Y = (int)(npcCenter.Y / 16f) * 16;
            targetCenterX -= npcCenter.X;
            targetCenterY -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt(targetCenterX * targetCenterX + targetCenterY * targetCenterY);
            float absoluteTargetX = Math.Abs(targetCenterX);
            float absoluteTargetY = Math.Abs(targetCenterY);
            float timeToReachTarget = velocity / targetDistance;
            targetCenterX *= timeToReachTarget;
            targetCenterY *= timeToReachTarget;

            if ((NPC.velocity.X > 0f && targetCenterX > 0f) || (NPC.velocity.X < 0f && targetCenterX < 0f) || (NPC.velocity.Y > 0f && targetCenterY > 0f) || (NPC.velocity.Y < 0f && targetCenterY < 0f))
            {
                if (NPC.velocity.X < targetCenterX)
                {
                    NPC.velocity.X = NPC.velocity.X + acceleration;
                }
                else
                {
                    if (NPC.velocity.X > targetCenterX)
                        NPC.velocity.X = NPC.velocity.X - acceleration;
                }

                if (NPC.velocity.Y < targetCenterY)
                {
                    NPC.velocity.Y = NPC.velocity.Y + acceleration;
                }
                else
                {
                    if (NPC.velocity.Y > targetCenterY)
                        NPC.velocity.Y = NPC.velocity.Y - acceleration;
                }

                if (Math.Abs(targetCenterY) < velocity * 0.2 && ((NPC.velocity.X > 0f && targetCenterX < 0f) || (NPC.velocity.X < 0f && targetCenterX > 0f)))
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y = NPC.velocity.Y + acceleration * 2f;
                    else
                        NPC.velocity.Y = NPC.velocity.Y - acceleration * 2f;
                }

                if (Math.Abs(targetCenterX) < velocity * 0.2 && ((NPC.velocity.Y > 0f && targetCenterY < 0f) || (NPC.velocity.Y < 0f && targetCenterY > 0f)))
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                    else
                        NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                }
            }
            else
            {
                if (absoluteTargetX > absoluteTargetY)
                {
                    if (NPC.velocity.X < targetCenterX)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 1.1f;
                    else if (NPC.velocity.X > targetCenterX)
                        NPC.velocity.X = NPC.velocity.X - acceleration * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < velocity * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = NPC.velocity.Y + acceleration;
                        else
                            NPC.velocity.Y = NPC.velocity.Y - acceleration;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targetCenterY)
                        NPC.velocity.Y = NPC.velocity.Y + acceleration * 1.1f;
                    else if (NPC.velocity.Y > targetCenterY)
                        NPC.velocity.Y = NPC.velocity.Y - acceleration * 1.1f;

                    if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < velocity * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X = NPC.velocity.X + acceleration;
                        else
                            NPC.velocity.X = NPC.velocity.X - acceleration;
                    }
                }
            }

            // Calculate contact damage based on velocity
            float minimalContactDamageVelocity = velocity * 0.25f;
            float minimalDamageVelocity = velocity * 0.5f;
            if (NPC.velocity.Length() <= minimalContactDamageVelocity)
            {
                NPC.damage = (int)Math.Round(NPC.defDamage * 0.5);
            }
            else
            {
                float velocityDamageScalar = MathHelper.Clamp((NPC.velocity.Length() - minimalContactDamageVelocity) / minimalDamageVelocity, 0f, 1f);
                NPC.damage = (int)MathHelper.Lerp((float)Math.Round(NPC.defDamage * 0.5), NPC.defDamage, velocityDamageScalar);
            }

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

            if (phase4)
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

                // Thunder sound
                if (lightning == 1f)
                    SoundEngine.PlaySound(SoundID.Thunder, NPC.Center);

                // Start a storm when in third phase. Don't do this during Boss Rush
                if (Main.netMode == NetmodeID.MultiplayerClient || (Main.netMode == NetmodeID.SinglePlayer && Main.gameMenu) || calamityGlobalNPC.newAI[1] > 0f || bossRush)
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

            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            bool phase2 = lifeRatio < 0.8f && (!CalamityWorld.LegendaryMode || !revenge);
            bool phase3 = lifeRatio < 0.55f;

            // Gate value that decides when Storm Weaver will charge
            float chargePhaseGateValue = bossRush ? 280f : death ? 320f : revenge ? 340f : expertMode ? 360f : 400f;
            if (!phase3)
                chargePhaseGateValue *= 0.5f;

            Texture2D texture = phase2 ? Phase2Texture.Value : TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2(texture.Width / 2, texture.Height / 2);
            float chargeTelegraphTime = 120f;
            float chargeTelegraphGateValue = chargePhaseGateValue - chargeTelegraphTime;

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture.Width, texture.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color drawColorAlpha = NPC.GetAlpha(drawColor);

            if (calamityGlobalNPC.newAI[0] > chargeTelegraphGateValue)
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, MathHelper.Clamp((calamityGlobalNPC.newAI[0] - chargeTelegraphGateValue) / chargeTelegraphTime, 0f, 1f));
            else if (NPC.localAI[3] > 0f)
                drawColorAlpha = Color.Lerp(drawColorAlpha, Color.Cyan, MathHelper.Clamp(NPC.localAI[3] / 60f, 0f, 1f));

            spriteBatch.Draw(texture, drawLocation, NPC.frame, drawColorAlpha, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            bool bossRush = BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool expertMode = Main.expertMode || bossRush;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            bool phase3 = lifeRatio < 0.55f;

            // Gate value that decides when Storm Weaver will charge
            float chargePhaseGateValue = bossRush ? 280f : death ? 320f : revenge ? 340f : expertMode ? 360f : 400f;
            if (!phase3)
                chargePhaseGateValue *= 0.5f;

            int buffDuration = NPC.Calamity().newAI[0] >= chargePhaseGateValue ? 240 : 120;
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.Electrified, buffDuration, true);
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeHead1").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("SWNudeHead2").Type, NPC.scale);
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(50 * NPC.scale);
                NPC.height = (int)(50 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);

                for (int i = 0; i < 20; i++)
                {
                    int cosmiliteDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[cosmiliteDust].scale = 0.5f;
                        Main.dust[cosmiliteDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }

                for (int j = 0; j < 40; j++)
                {
                    int cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[cosmiliteDust2].noGravity = true;
                    Main.dust[cosmiliteDust2].velocity *= 5f;
                    cosmiliteDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[cosmiliteDust2].velocity *= 2f;
                }
            }
        }

        public override bool CheckDead()
        {
            for (int k = 0; k < Main.maxNPCs; k++)
            {
                if (Main.npc[k].active && (Main.npc[k].type == ModContent.NPCType<StormWeaverBody>() || Main.npc[k].type == ModContent.NPCType<StormWeaverTail>()))
                    Main.npc[k].life = 0;
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

        public static bool LastSentinelKilled() => !DownedBossSystem.downedSignus && DownedBossSystem.downedStormWeaver && DownedBossSystem.downedCeaselessVoid;

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);
            DownedBossSystem.downedStormWeaver = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<StormWeaverBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(normalOnly);
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
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<ArmoredShell>(), 1, 5, 7));

                // Vanity
                normalOnly.Add(ModContent.ItemType<StormWeaverMask>(), 7);
                normalOnly.Add(ModContent.ItemType<LittleLight>(), 10);
                var godSlayerVanity = ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerHelm>(), 20);
                godSlayerVanity.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerChestplate>()));
                godSlayerVanity.OnSuccess(ItemDropRule.Common(ModContent.ItemType<AncientGodSlayerLeggings>()));
                normalOnly.Add(godSlayerVanity);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<WeaverTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<StormWeaverRelic>());

            // GFB Elemental Gauntlet and Quiver drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ModContent.ItemType<ElementalGauntlet>(), hideLootReport: true);
                GFBOnly.Add(ModContent.ItemType<ElementalQuiver>(), hideLootReport: true);
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedStormWeaver, ModContent.ItemType<LoreStormWeaver>(), desc: DropHelper.FirstKillText);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }
}
