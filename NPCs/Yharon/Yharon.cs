using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.Ores;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace CalamityMod.NPCs.Yharon
{
    [AutoloadBossHead]
    public class Yharon : ModNPC
    {
        private Rectangle safeBox = default;
        private bool protectionBoost = false;
        private bool moveCloser = false;
        private bool phaseOneLoot = false;
        private bool dropLoot = false;
        private bool useTornado = true;
        private int healCounter = 0;
        private int secondPhasePhase = 1;
        private int teleportLocation = 0;
        private bool startSecondAI = false;
        private bool spawnArena = false;
        private int invincibilityCounter = 0;

        public static float Phase1_DR = 0.17f;
        public static float Phase2_DR = 0.22f;
        public static float EnragedDR = 0.9f;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Jungle Dragon, Yharon");
            Main.npcFrameCount[npc.type] = 7;
        }

        public override void SetDefaults()
        {
            npc.npcSlots = 50f;
            npc.damage = 330;
            npc.width = 200;
            npc.height = 200;
            npc.defense = 100;
            npc.LifeMaxNERD(2275000, 2525000, 3025000, 3700000, 4000000);
            double HPBoost = Config.BossHealthPercentageBoost * 0.01;
            npc.lifeMax += (int)(npc.lifeMax * HPBoost);
            npc.knockBackResist = 0f;
            npc.aiStyle = -1;
            aiType = -1;
            npc.value = Item.buyPrice(1, 50, 0, 0);
            npc.boss = true;
            NPCID.Sets.TrailCacheLength[npc.type] = 8;
            NPCID.Sets.TrailingMode[npc.type] = 1;

            for (int k = 0; k < npc.buffImmune.Length; k++)
            {
                npc.buffImmune[k] = true;
            }
            npc.buffImmune[BuffID.Ichor] = false;
            npc.buffImmune[BuffID.CursedInferno] = false;
            npc.buffImmune[ModContent.BuffType<DemonFlames>()] = false;
            npc.buffImmune[ModContent.BuffType<Shred>()] = false;

            CalamityGlobalNPC global = npc.Calamity();
            global.DR = Phase1_DR;
            global.customDR = true;
            global.flatDRReductions.Add(BuffID.Ichor, 0.05f);
            global.flatDRReductions.Add(BuffID.CursedInferno, 0.05f);

            npc.noGravity = true;
            npc.noTileCollide = true;
            npc.netAlways = true;
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (calamityModMusic != null)
                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/YHARON");
            else
                music = MusicID.Boss1;
            if (CalamityWorld.downedBuffedMothron || CalamityWorld.bossRushActive)
            {
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/YHARONREBIRTH");
                else
                    music = MusicID.Boss3;
            }
            npc.HitSound = SoundID.NPCHit56;
            npc.DeathSound = SoundID.NPCDeath60;
            bossBag = ModContent.ItemType<YharonBag>();
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(protectionBoost);
            writer.Write(moveCloser);
            writer.Write(phaseOneLoot);
            writer.Write(dropLoot);
            writer.Write(useTornado);
            writer.Write(healCounter);
            writer.Write(secondPhasePhase);
            writer.Write(teleportLocation);
            writer.Write(startSecondAI);
            writer.Write(spawnArena);
            writer.Write(invincibilityCounter);
            writer.Write(npc.dontTakeDamage);
            writer.Write(npc.chaseable);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            protectionBoost = reader.ReadBoolean();
            moveCloser = reader.ReadBoolean();
            phaseOneLoot = reader.ReadBoolean();
            dropLoot = reader.ReadBoolean();
            useTornado = reader.ReadBoolean();
            healCounter = reader.ReadInt32();
            secondPhasePhase = reader.ReadInt32();
            teleportLocation = reader.ReadInt32();
            startSecondAI = reader.ReadBoolean();
            spawnArena = reader.ReadBoolean();
            invincibilityCounter = reader.ReadInt32();
            npc.dontTakeDamage = reader.ReadBoolean();
            npc.chaseable = reader.ReadBoolean();
        }

        public override void AI()
        {
            // Disable loot drop
            dropLoot = (double)npc.life <= (double)npc.lifeMax * 0.1;

            // Stop rain
            CalamityMod.StopRain();

            // Start phase 2 or not
            if (startSecondAI)
            {
                // Despawn and drop phase 1 loot
                if (!CalamityWorld.downedBuffedMothron && !CalamityWorld.bossRushActive)
                {
                    phaseOneLoot = true;

                    npc.DeathSound = null;
                    npc.dontTakeDamage = true;
                    npc.chaseable = false;

                    npc.velocity.Y = npc.velocity.Y - 0.4f;

                    if (npc.alpha < 255)
                    {
                        npc.alpha += 5;
                        if (npc.alpha > 255)
                            npc.alpha = 255;
                    }

                    if (npc.timeLeft > 55)
                        npc.timeLeft = 55;

                    if (npc.timeLeft < 5)
                    {
                        string key = "Mods.CalamityMod.DargonBossText2";
                        Color messageColor = Color.Orange;
                        if (Main.netMode == NetmodeID.SinglePlayer)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == NetmodeID.Server)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }

                        startSecondAI = false;

                        npc.boss = false;
                        npc.life = 0;

                        if (dropLoot)
                            npc.NPCLoot();

                        npc.active = false;
                        npc.netUpdate = true;
                    }

                    return;
                }

                // Don't drop phase 1 loot
                phaseOneLoot = false;

                // Start second AI
                Yharon_AI2();

                return;
            }

            // Phase bools
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool phase2Check = (double)npc.life <= (double)npc.lifeMax * (revenge ? 0.85 : 0.7);
            bool phase3Check = (double)npc.life <= (double)npc.lifeMax * (revenge ? 0.6 : 0.4);
            bool phase4Check = (double)npc.life <= (double)npc.lifeMax * (revenge ? 0.25 : 0.2);
            if (CalamityWorld.death && !CalamityWorld.bossRushActive)
            {
                phase2Check = (double)npc.life <= (double)npc.lifeMax * 0.9;
                phase3Check = (double)npc.life <= (double)npc.lifeMax * 0.8;
                phase4Check = (double)npc.life <= (double)npc.lifeMax * 0.3;
            }
            bool phase5Check = (double)npc.life <= (double)npc.lifeMax * 0.1;
            bool phase2Change = npc.ai[0] > 5f;
            bool phase3Change = npc.ai[0] > 12f;
            bool phase4Change = npc.ai[0] > 20f;
            bool isCharging = npc.ai[3] < 20f;

            // Flare limit
            int flareCount = 3;

            // Velocity and acceleration
            int aiChangeRate = expertMode ? 36 : 38;
            float npcVelocity = expertMode ? 0.7f : 0.69f;
            float scaleFactor = expertMode ? 11f : 10.8f;
            if (phase4Change || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                npcVelocity = 0.95f;
                scaleFactor = 14f;
                aiChangeRate = 25;
            }
            else if (phase3Change)
            {
                npcVelocity = 0.9f;
                scaleFactor = 13f;
                aiChangeRate = 25;
            }
            else if (phase2Change && isCharging)
            {
                npcVelocity = expertMode ? 0.8f : 0.78f;
                scaleFactor = expertMode ? 12.2f : 12f;
                aiChangeRate = expertMode ? 36 : 38;
            }
            else if (isCharging && !phase2Change && !phase3Change && !phase4Change)
            {
                aiChangeRate = 25;
            }

            int chargeTime = expertMode ? 38 : 40;
            int chargeTime2 = expertMode ? 34 : 36;
            float chargeSpeed = expertMode ? 22f : 20.5f;
            float chargeSpeed2 = expertMode ? 37f : 34f;
            if (phase4Change || npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                chargeTime = 28;
                chargeSpeed = 31f;
            }
            else if (phase3Change)
            {
                chargeTime = 32;
                chargeTime2 = 27;
                chargeSpeed = 25f;
                chargeSpeed2 = 41f;
            }
            else if (isCharging && phase2Change)
            {
                chargeTime = expertMode ? 35 : 37;
                chargeTime2 = expertMode ? 31 : 33;
                if (expertMode)
                {
                    chargeSpeed = 24f;
                    chargeSpeed2 = 39f;
                }
            }

            // Variables for charging and etc.
            int xPos = npc.direction == 1 ? 25 : -25;
            int num1454 = 80;
            int num1455 = 4;
            float num1456 = 0.3f;
            float scaleFactor11 = 5f;
            int num1457 = 90;
            int num1458 = 180;
            int num1459 = 180;
            int num1460 = 30;
            int num1461 = 120;
            int num1462 = 4;
            float scaleFactor13 = 20f;
            float num1463 = 6.28318548f / (float)(num1461 / 2);
            int num1464 = 75;

            // Center and target
            Vector2 vectorCenter = npc.Center;
            Player player = Main.player[npc.target];
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(true);
                player = Main.player[npc.target];
                npc.netUpdate = true;
            }

            // Despawn
            if (player.dead)
            {
                npc.velocity.Y = npc.velocity.Y - 0.4f;

                if (npc.timeLeft > 150)
                    npc.timeLeft = 150;

                if (npc.ai[0] > 12f)
                    npc.ai[0] = 13f;
                else if (npc.ai[0] > 5f)
                    npc.ai[0] = 6f;
                else
                    npc.ai[0] = 0f;

                npc.ai[2] = 0f;
            }
            else if (npc.timeLeft < 3600)
                npc.timeLeft = 3600;

            // Create the arena
            if (!spawnArena)
            {
                spawnArena = true;
                safeBox.X = (int)(player.Center.X - (revenge ? 3000f : 3500f));
                safeBox.Y = (int)(player.Center.Y - (revenge ? 9000f : 10500f));
                safeBox.Width = revenge ? 6000 : 7000;
                safeBox.Height = revenge ? 18000 : 21000;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(player.Center.X + (revenge ? 3000f : 3500f), player.Center.Y + 100f, 0f, 0f, ModContent.ProjectileType<SkyFlareRevenge>(), 0, 0f, Main.myPlayer, 0f, 0f);
                    Projectile.NewProjectile(player.Center.X - (revenge ? 3000f : 3500f), player.Center.Y + 100f, 0f, 0f, ModContent.ProjectileType<SkyFlareRevenge>(), 0, 0f, Main.myPlayer, 0f, 0f);
                }
            }

            // Enrage
            if (!Main.player[npc.target].Hitbox.Intersects(safeBox))
            {
                aiChangeRate = 15;
                protectionBoost = true;
                npc.damage = npc.defDamage * 5;
                chargeSpeed += 25f;
            }
            else
            {
                npc.damage = npc.defDamage;
                protectionBoost = false;
            }

            // Set DR based on protection boost (aka enrage)
            npc.Calamity().DR = protectionBoost ? EnragedDR : Phase1_DR;

            // Trigger spawn effects
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

            // Rotation
            float npcRotation = (float)Math.Atan2((double)(player.Center.Y - vectorCenter.Y), (double)(player.Center.X - vectorCenter.X));
            if (npc.spriteDirection == 1)
                npcRotation += 3.14159274f;
            if (npcRotation < 0f)
                npcRotation += 6.28318548f;
            if (npcRotation > 6.28318548f)
                npcRotation -= 6.28318548f;
            if (npc.ai[0] == -1f)
                npcRotation = 0f;
            if (npc.ai[0] == 3f)
                npcRotation = 0f;
            if (npc.ai[0] == 4f)
                npcRotation = 0f;
            if (npc.ai[0] == 9f)
                npcRotation = 0f;
            if (npc.ai[0] == 10f)
                npcRotation = 0f;
            if (npc.ai[0] == 16f)
                npcRotation = 0f;
            if (npc.ai[0] == 20f)
                npcRotation = 0f;

            float npcRotationSpeed = 0.04f;
            if (npc.ai[0] == 1f || npc.ai[0] == 5f || npc.ai[0] == 7f || npc.ai[0] == 11f || npc.ai[0] == 14f || npc.ai[0] == 18f)
                npcRotationSpeed = 0f;
            if (npc.ai[0] == 8f || npc.ai[0] == 12f || npc.ai[0] == 15f || npc.ai[0] == 19f)
                npcRotationSpeed = 0f;
            if (npc.ai[0] == 3f)
                npcRotationSpeed = 0.01f;
            if (npc.ai[0] == 4f)
                npcRotationSpeed = 0.01f;
            if (npc.ai[0] == 9f || npc.ai[0] == 16f || npc.ai[0] == 20f)
                npcRotationSpeed = 0.01f;

            if (npc.rotation < npcRotation)
            {
                if ((double)(npcRotation - npc.rotation) > 3.1415926535897931)
                    npc.rotation -= npcRotationSpeed;
                else
                    npc.rotation += npcRotationSpeed;
            }
            if (npc.rotation > npcRotation)
            {
                if ((double)(npc.rotation - npcRotation) > 3.1415926535897931)
                    npc.rotation += npcRotationSpeed;
                else
                    npc.rotation -= npcRotationSpeed;
            }

            if (npc.rotation > npcRotation - npcRotationSpeed && npc.rotation < npcRotation + npcRotationSpeed)
                npc.rotation = npcRotation;
            if (npc.rotation < 0f)
                npc.rotation += 6.28318548f;
            if (npc.rotation > 6.28318548f)
                npc.rotation -= 6.28318548f;
            if (npc.rotation > npcRotation - npcRotationSpeed && npc.rotation < npcRotation + npcRotationSpeed)
                npc.rotation = npcRotation;

            // Alpha
            if (npc.ai[0] != -1f && npc.ai[0] < 9f)
            {
                bool colliding = Collision.SolidCollision(npc.position, npc.width, npc.height);
                if (colliding)
                    npc.alpha += 15;
                else
                    npc.alpha -= 15;

                if (npc.alpha < 0)
                    npc.alpha = 0;
                if (npc.alpha > 150)
                    npc.alpha = 150;
            }

            // Spawn effects
            if (npc.ai[0] == -1f)
            {
                npc.dontTakeDamage = true;
                npc.chaseable = false;

                npc.velocity *= 0.98f;

                // Direction
                int num1467 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1467 != 0)
                {
                    npc.direction = num1467;
                    npc.spriteDirection = -npc.direction;
                }

                // Alpha
                if (npc.ai[2] > 20f)
                {
                    npc.velocity.Y = -2f;
                    npc.alpha -= 5;

                    bool colliding = Collision.SolidCollision(npc.position, npc.width, npc.height);
                    if (colliding)
                        npc.alpha += 15;
                    if (npc.alpha < 0)
                        npc.alpha = 0;
                    if (npc.alpha > 150)
                        npc.alpha = 150;
                }

                // Dust
                if (npc.ai[2] == (float)(num1457 - 30))
                {
                    int num1468 = 72;
                    for (int num1469 = 0; num1469 < num1468; num1469++)
                    {
                        Vector2 vector169 = Vector2.Normalize(npc.velocity) * new Vector2((float)npc.width / 2f, (float)npc.height) * 0.75f * 0.5f;
                        vector169 = vector169.RotatedBy((double)((float)(num1469 - (num1468 / 2 - 1)) * 6.28318548f / (float)num1468), default) + npc.Center;
                        Vector2 value16 = vector169 - npc.Center;
                        int num1470 = Dust.NewDust(vector169 + value16, 0, 0, 244, value16.X * 2f, value16.Y * 2f, 100, default, 1.4f);
                        Main.dust[num1470].noGravity = true;
                        Main.dust[num1470].noLight = true;
                        Main.dust[num1470].velocity = Vector2.Normalize(value16) * 3f;
                    }
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }

                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1464)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }

            #region Phase1
            else if (npc.ai[0] == 0f && !player.dead)
            {
                npc.dontTakeDamage = false;
                npc.chaseable = true;
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = (float)(300 * Math.Sign((vectorCenter - player.Center).X));
                }
                Vector2 value17 = player.Center + new Vector2(npc.ai[1], -200f) - vectorCenter;
                Vector2 vector170 = Vector2.Normalize(value17 - npc.velocity) * scaleFactor;
                if (npc.velocity.X < vector170.X)
                {
                    npc.velocity.X = npc.velocity.X + npcVelocity;
                    if (npc.velocity.X < 0f && vector170.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + npcVelocity;
                    }
                }
                else if (npc.velocity.X > vector170.X)
                {
                    npc.velocity.X = npc.velocity.X - npcVelocity;
                    if (npc.velocity.X > 0f && vector170.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - npcVelocity;
                    }
                }
                if (npc.velocity.Y < vector170.Y)
                {
                    npc.velocity.Y = npc.velocity.Y + npcVelocity;
                    if (npc.velocity.Y < 0f && vector170.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + npcVelocity;
                    }
                }
                else if (npc.velocity.Y > vector170.Y)
                {
                    npc.velocity.Y = npc.velocity.Y - npcVelocity;
                    if (npc.velocity.Y > 0f && vector170.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - npcVelocity;
                    }
                }
                int num1471 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1471 != 0)
                {
                    if (npc.ai[2] == 0f && num1471 != npc.direction)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.direction = num1471;
                    if (npc.spriteDirection != -npc.direction)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.spriteDirection = -npc.direction;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)aiChangeRate)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3]) //0 2 4 6 1 3 5 7 repeat
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                            aiState = 1; //normal charges
                            break;
                        case 5:
                            aiState = 5; //fast charge
                            break;
                        case 6:
                            npc.ai[3] = 1f;
                            aiState = 2; //fireball attack
                            break;
                        case 7:
                            npc.ai[3] = 0f;
                            aiState = 3; //tornadoes
                            break;
                    }
                    if (phase2Check)
                    {
                        aiState = 4;
                    }
                    if (aiState == 1)
                    {
                        npc.ai[0] = 1f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1471 != 0)
                        {
                            npc.direction = num1471;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        npc.ai[0] = 2f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 3f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 4f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 5)
                    {
                        npc.ai[0] = 5f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed2;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1471 != 0)
                        {
                            npc.direction = num1471;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f) //charge attack
            {
                int num1473 = 7;
                for (int num1474 = 0; num1474 < num1473; num1474++)
                {
                    Vector2 vector171 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector171 = vector171.RotatedBy((double)(num1474 - (num1473 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1473, default) + vectorCenter;
                    Vector2 value18 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 244, value18.X * 2f, value18.Y * 2f, 100, default, 1.4f);
                    Main.dust[num1475].noGravity = true;
                    Main.dust[num1475].noLight = true;
                    Main.dust[num1475].velocity /= 4f;
                    Main.dust[num1475].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 2f) //fireball attack
            {
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = (float)(300 * Math.Sign((vectorCenter - player.Center).X));
                }
                Vector2 value19 = player.Center + new Vector2(npc.ai[1], -200f) - vectorCenter;
                Vector2 vector172 = Vector2.Normalize(value19 - npc.velocity) * scaleFactor11;
                if (npc.velocity.X < vector172.X)
                {
                    npc.velocity.X = npc.velocity.X + num1456;
                    if (npc.velocity.X < 0f && vector172.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + num1456;
                    }
                }
                else if (npc.velocity.X > vector172.X)
                {
                    npc.velocity.X = npc.velocity.X - num1456;
                    if (npc.velocity.X > 0f && vector172.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - num1456;
                    }
                }
                if (npc.velocity.Y < vector172.Y)
                {
                    npc.velocity.Y = npc.velocity.Y + num1456;
                    if (npc.velocity.Y < 0f && vector172.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + num1456;
                    }
                }
                else if (npc.velocity.Y > vector172.Y)
                {
                    npc.velocity.Y = npc.velocity.Y - num1456;
                    if (npc.velocity.Y > 0f && vector172.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - num1456;
                    }
                }
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.ai[2] % (float)num1455 == 0f) //fire flare bombs from mouth
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                        if (NPC.CountNPCS(ModContent.NPCType<DetonatingFlare>()) < flareCount)
                        {
                            NPC.NewNPC((int)vector.X + xPos, (int)vector.Y - 15, ModContent.NPCType<DetonatingFlare>(), 0, 0f, 0f, 0f, 0f, 255);
                        }
                        int damage = expertMode ? 75 : 90;
                        Projectile.NewProjectile((int)vector.X + xPos, (int)vector.Y - 15, 0f, 0f, ModContent.ProjectileType<FlareBomb>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                int num1476 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1476 != 0)
                {
                    npc.direction = num1476;
                    if (npc.spriteDirection != -npc.direction)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.spriteDirection = -npc.direction;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1454)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 3f) //Fire small flares
            {
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1457 - 30))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num1457 - 30))
                {
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, (float)(npc.direction * 2), 8f, ModContent.ProjectileType<Flare>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1)); //changed
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, (float)(-(float)npc.direction * 2), 8f, ModContent.ProjectileType<Flare>(), 0, 0f, Main.myPlayer, 0f, 0f); //changed
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1457)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 4f) //enter phase 2
            {
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1458 - 60))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1458)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 5f)
            {
                int num1473 = 14;
                for (int num1474 = 0; num1474 < num1473; num1474++)
                {
                    Vector2 vector171 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector171 = vector171.RotatedBy((double)(num1474 - (num1473 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1473, default) + vectorCenter;
                    Vector2 value18 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 244, value18.X * 2f, value18.Y * 2f, 100, default, 1.4f); //changed
                    Main.dust[num1475].noGravity = true;
                    Main.dust[num1475].noLight = true;
                    Main.dust[num1475].velocity /= 4f;
                    Main.dust[num1475].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime2)
                {
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            #endregion

            #region Phase2
            else if (npc.ai[0] == 6f && !player.dead) //phase 2
            {
                npc.dontTakeDamage = false;
                npc.chaseable = true;
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = (float)(300 * Math.Sign((vectorCenter - player.Center).X));
                }
                Vector2 value20 = player.Center + new Vector2(npc.ai[1], -200f) - vectorCenter;
                Vector2 vector175 = Vector2.Normalize(value20 - npc.velocity) * scaleFactor;
                if (npc.velocity.X < vector175.X)
                {
                    npc.velocity.X = npc.velocity.X + npcVelocity;
                    if (npc.velocity.X < 0f && vector175.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + npcVelocity;
                    }
                }
                else if (npc.velocity.X > vector175.X)
                {
                    npc.velocity.X = npc.velocity.X - npcVelocity;
                    if (npc.velocity.X > 0f && vector175.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - npcVelocity;
                    }
                }
                if (npc.velocity.Y < vector175.Y)
                {
                    npc.velocity.Y = npc.velocity.Y + npcVelocity;
                    if (npc.velocity.Y < 0f && vector175.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + npcVelocity;
                    }
                }
                else if (npc.velocity.Y > vector175.Y)
                {
                    npc.velocity.Y = npc.velocity.Y - npcVelocity;
                    if (npc.velocity.Y > 0f && vector175.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - npcVelocity;
                    }
                }
                int num1477 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1477 != 0)
                {
                    if (npc.ai[2] == 0f && num1477 != npc.direction)
                    {
                        npc.rotation = 3.14159274f;
                    }
                    npc.direction = num1477;
                    if (npc.spriteDirection != -npc.direction)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.spriteDirection = -npc.direction;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)aiChangeRate)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3]) //0 2 4 6 8 1 3 5 7 9 repeat
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                        case 4:
                        case 5:
                            aiState = 1;
                            break;
                        case 6:
                            aiState = 5;
                            break;
                        case 7:
                            aiState = 6;
                            break;
                        case 8:
                            npc.ai[3] = 1f;
                            aiState = 2;
                            break;
                        case 9:
                            npc.ai[3] = 0f;
                            aiState = 3;
                            break;
                    }
                    if (phase3Check)
                    {
                        aiState = 4;
                    }
                    if (aiState == 1)
                    {
                        npc.ai[0] = 7f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * scaleFactor13;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                        npc.ai[0] = 8f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 9f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 10f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 5)
                    {
                        npc.ai[0] = 11f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed2;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 6)
                    {
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * scaleFactor13;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                        npc.ai[0] = 12f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 7f) //charge
            {
                int num1479 = 7;
                for (int num1480 = 0; num1480 < num1479; num1480++)
                {
                    Vector2 vector176 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector176 = vector176.RotatedBy((double)(num1480 - (num1479 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1479, default) + vectorCenter;
                    Vector2 value21 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1481 = Dust.NewDust(vector176 + value21, 0, 0, 244, value21.X * 2f, value21.Y * 2f, 100, default, 1.4f); //changed
                    Main.dust[num1481].noGravity = true;
                    Main.dust[num1481].noLight = true;
                    Main.dust[num1481].velocity /= 4f;
                    Main.dust[num1481].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 8f)
            {
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.ai[2] % (float)num1462 == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                        if (NPC.CountNPCS(ModContent.NPCType<DetonatingFlare2>()) < flareCount)
                        {
                            NPC.NewNPC((int)vector.X + xPos, (int)vector.Y - 15, ModContent.NPCType<DetonatingFlare2>(), 0, 0f, 0f, 0f, 0f, 255);
                        }
                        Projectile.NewProjectile((int)vector.X + xPos, (int)vector.Y - 15, (float)Main.rand.Next(-400, 401) * 0.13f, (float)Main.rand.Next(-30, 31) * 0.13f, ModContent.ProjectileType<FlareDust>(), 0, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                npc.velocity = npc.velocity.RotatedBy((double)(-(double)num1463 * (float)npc.direction), default);
                npc.rotation -= num1463 * (float)npc.direction;
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1461)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 9f)
            {
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1457 - 30))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num1457 - 30))
                {
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<BigFlare>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1457)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 10f) //start phase 3
            {
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1459 - 60))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1459)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 11f)
            {
                int num1479 = 14;
                for (int num1480 = 0; num1480 < num1479; num1480++)
                {
                    Vector2 vector176 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector176 = vector176.RotatedBy((double)(num1480 - (num1479 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1479, default) + vectorCenter;
                    Vector2 value21 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1481 = Dust.NewDust(vector176 + value21, 0, 0, 244, value21.X * 2f, value21.Y * 2f, 100, default, 1.4f); //changed
                    Main.dust[num1481].noGravity = true;
                    Main.dust[num1481].noLight = true;
                    Main.dust[num1481].velocity /= 4f;
                    Main.dust[num1481].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime2)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 12f)
            {
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.ai[2] % (float)num1462 == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = expertMode ? 75 : 90;
                        Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                        float speed = 0.01f;
                        Vector2 vectorShoot = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                        float playerX = player.position.X + (float)player.width * 0.5f - vectorShoot.X;
                        float playerY = player.position.Y - vectorShoot.Y;
                        float playerXY = (float)Math.Sqrt((double)(playerX * playerX + playerY * playerY));
                        playerXY = speed / playerXY;
                        playerX *= playerXY;
                        playerY *= playerXY;
                        Projectile.NewProjectile((int)vector173.X + xPos, (int)vector173.Y - 15, playerX, playerY, ModContent.ProjectileType<FlareDust2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                npc.velocity = npc.velocity.RotatedBy((double)(-(double)num1463 * (float)npc.direction), default);
                npc.rotation -= num1463 * (float)npc.direction;
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1461)
                {
                    npc.ai[0] = 6f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            #endregion

            #region Phase3
            else if (npc.ai[0] == 13f && !player.dead)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                if (npc.ai[1] == 0f)
                {
                    npc.ai[1] = (float)(300 * Math.Sign((vectorCenter - player.Center).X));
                }
                Vector2 value20 = player.Center + new Vector2(npc.ai[1], -200f) - vectorCenter;
                Vector2 vector175 = Vector2.Normalize(value20 - npc.velocity) * scaleFactor;
                if (npc.velocity.X < vector175.X)
                {
                    npc.velocity.X = npc.velocity.X + npcVelocity;
                    if (npc.velocity.X < 0f && vector175.X > 0f)
                    {
                        npc.velocity.X = npc.velocity.X + npcVelocity;
                    }
                }
                else if (npc.velocity.X > vector175.X)
                {
                    npc.velocity.X = npc.velocity.X - npcVelocity;
                    if (npc.velocity.X > 0f && vector175.X < 0f)
                    {
                        npc.velocity.X = npc.velocity.X - npcVelocity;
                    }
                }
                if (npc.velocity.Y < vector175.Y)
                {
                    npc.velocity.Y = npc.velocity.Y + npcVelocity;
                    if (npc.velocity.Y < 0f && vector175.Y > 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y + npcVelocity;
                    }
                }
                else if (npc.velocity.Y > vector175.Y)
                {
                    npc.velocity.Y = npc.velocity.Y - npcVelocity;
                    if (npc.velocity.Y > 0f && vector175.Y < 0f)
                    {
                        npc.velocity.Y = npc.velocity.Y - npcVelocity;
                    }
                }
                int num1477 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num1477 != 0)
                {
                    if (npc.ai[2] == 0f && num1477 != npc.direction)
                    {
                        npc.rotation = 3.14159274f;
                    }
                    npc.direction = num1477;
                    if (npc.spriteDirection != -npc.direction)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.spriteDirection = -npc.direction;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)aiChangeRate)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3]) //0 2 4 6 8 9 1 3 5 7 10 repeat
                    {
                        case 0:
                        case 1:
                        case 2:
                        case 3:
                            aiState = (CalamityWorld.death && !CalamityWorld.bossRushActive) ? 5 : 1; //normal charges
                            break;
                        case 4:
                        case 5:
                        case 6:
                            aiState = 5; //fast charges
                            break;
                        case 7:
                            aiState = 3; //big tornado
                            break;
                        case 8:
                            aiState = 6; //slow flare bombs
                            break;
                        case 9:
                            npc.ai[3] = 1f;
                            aiState = 7; //small tornado
                            break;
                        case 10:
                            npc.ai[3] = 0f;
                            aiState = 2; //flare circle
                            break;
                    }
                    if (phase4Check)
                    {
                        aiState = 4;
                    }
                    if (aiState == 1)
                    {
                        npc.ai[0] = 14f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * scaleFactor13;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                        npc.ai[0] = 15f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 16f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 17f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 5)
                    {
                        npc.ai[0] = 18f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed2;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 6)
                    {
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * scaleFactor13;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num1477 != 0)
                        {
                            npc.direction = num1477;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                        npc.ai[0] = 19f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 7)
                    {
                        npc.ai[0] = 20f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 14f) //charge
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                int num1479 = 7;
                for (int num1480 = 0; num1480 < num1479; num1480++)
                {
                    Vector2 vector176 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector176 = vector176.RotatedBy((double)(num1480 - (num1479 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1479, default) + vectorCenter;
                    Vector2 value21 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1481 = Dust.NewDust(vector176 + value21, 0, 0, 244, value21.X * 2f, value21.Y * 2f, 100, default, 1.4f);
                    Main.dust[num1481].noGravity = true;
                    Main.dust[num1481].noLight = true;
                    Main.dust[num1481].velocity /= 4f;
                    Main.dust[num1481].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 15f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.ai[2] % (float)num1462 == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                        if (NPC.CountNPCS(ModContent.NPCType<DetonatingFlare2>()) < flareCount && NPC.CountNPCS(ModContent.NPCType<DetonatingFlare>()) < flareCount)
                        {
                            NPC.NewNPC((int)vector.X + xPos, (int)vector.Y - 15, Main.rand.NextBool(2) ? ModContent.NPCType<DetonatingFlare>() : ModContent.NPCType<DetonatingFlare2>(), 0, 0f, 0f, 0f, 0f, 255);
                        }
                        Projectile.NewProjectile((int)vector.X + xPos, (int)vector.Y - 15, (float)Main.rand.Next(-401, 401) * 0.13f, (float)Main.rand.Next(-31, 31) * 0.13f, ModContent.ProjectileType<FlareDust>(), 0, 0f, Main.myPlayer, 0f, 0f); //changed
                        Projectile.NewProjectile((int)vector.X + xPos, (int)vector.Y - 15, (float)Main.rand.Next(-31, 31) * 0.13f, (float)Main.rand.Next(-151, 151) * 0.13f, ModContent.ProjectileType<FlareDust>(), 0, 0f, Main.myPlayer, 0f, 0f); //changed
                    }
                }
                npc.velocity = npc.velocity.RotatedBy((double)(-(double)num1463 * (float)npc.direction), default);
                npc.rotation -= num1463 * (float)npc.direction;
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1461)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 16f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1457 - 30))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num1457 - 30))
                {
                    Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<BigFlare>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1457)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 3f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 17f)
            {
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                if (npc.ai[2] < (float)(num1459 - 90))
                {
                    bool colliding = Collision.SolidCollision(npc.position, npc.width, npc.height);
                    if (colliding)
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
                else if (npc.alpha < 255)
                {
                    npc.alpha += 4;
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                    }
                }
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1459 - 60))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1459)
                {
                    npc.ai[0] = 21f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 18f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                int num1479 = 14;
                for (int num1480 = 0; num1480 < num1479; num1480++)
                {
                    Vector2 vector176 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector176 = vector176.RotatedBy((double)(num1480 - (num1479 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1479, default) + vectorCenter;
                    Vector2 value21 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1481 = Dust.NewDust(vector176 + value21, 0, 0, 244, value21.X * 2f, value21.Y * 2f, 100, default, 1.4f);
                    Main.dust[num1481].noGravity = true;
                    Main.dust[num1481].noLight = true;
                    Main.dust[num1481].velocity /= 4f;
                    Main.dust[num1481].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime2)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 2f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 19f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (npc.ai[2] % (float)num1462 == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int damage = expertMode ? 75 : 90;
                        Vector2 vector173 = Vector2.Normalize(player.Center - vectorCenter) * (float)(npc.width + 20) / 2f + vectorCenter;
                        float speed = 0.01f;
                        Vector2 vectorShoot = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                        float playerX = player.position.X + (float)player.width * 0.5f - vectorShoot.X;
                        float playerY = player.position.Y - vectorShoot.Y;
                        float playerXY = (float)Math.Sqrt((double)(playerX * playerX + playerY * playerY));
                        playerXY = speed / playerXY;
                        playerX *= playerXY;
                        playerY *= playerXY;
                        Projectile.NewProjectile((int)vector173.X + xPos, (int)vector173.Y - 15, playerX, playerY, ModContent.ProjectileType<FlareDust2>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                npc.velocity = npc.velocity.RotatedBy((double)(-(double)num1463 * (float)npc.direction), default);
                npc.rotation -= num1463 * (float)npc.direction;
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1461)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 20f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1457 - 30))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (CalamityWorld.death && !CalamityWorld.bossRushActive)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num1457 - 30))
                    {
                        Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<BigFlare>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num1457 - 30))
                    {
                        Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<Flare>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1457)
                {
                    npc.ai[0] = 13f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            #endregion

            #region Phase4
            else if (npc.ai[0] == 21f && !player.dead)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = false;
                if (npc.alpha < 255)
                {
                    npc.alpha += 25;
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                    }
                }
                if (npc.ai[1] == 0f)
                {
                    teleportLocation = Main.rand.NextBool(2) ? (revenge ? 500 : 600) : (revenge ? -500 : -600);
                    npc.ai[1] = (float)(360 * Math.Sign((vectorCenter - player.Center).X));
                }
                Vector2 value7 = player.Center + new Vector2(npc.ai[1], (float)teleportLocation) - vectorCenter; //teleport distance
                Vector2 desiredVelocity = Vector2.Normalize(value7 - npc.velocity) * scaleFactor;
                npc.SimpleFlyMovement(desiredVelocity, npcVelocity);
                int num32 = Math.Sign(player.Center.X - vectorCenter.X);
                if (num32 != 0)
                {
                    if (npc.ai[2] == 0f && num32 != npc.direction)
                    {
                        npc.rotation = 3.14159274f;
                    }
                    npc.direction = num32;
                    if (npc.spriteDirection != -npc.direction)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.spriteDirection = -npc.direction;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)aiChangeRate)
                {
                    int aiState = 0;
                    switch ((int)npc.ai[3])
                    {
                        case 0: //skip 1
                        case 2:
                        case 3: //skip 4
                        case 5:
                        case 6:
                        case 7: //skip 8
                        case 9:
                        case 10:
                        case 11:
                        case 12: //skip 13
                        case 14:
                        case 15:
                        case 16:
                        case 17:
                        case 18: //skip 19
                            aiState = 1;
                            break;
                        case 1: //+3
                        case 4: //+4
                        case 8: //+5
                        case 13: //+6
                        case 19:
                            aiState = 2;
                            break;
                    }
                    if (phase5Check)
                    {
                        aiState = 4;
                    }
                    if (aiState == 1)
                    {
                        npc.ai[0] = 22f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                        npc.velocity = Vector2.Normalize(player.Center - vectorCenter) * chargeSpeed;
                        npc.rotation = (float)Math.Atan2((double)npc.velocity.Y, (double)npc.velocity.X);
                        if (num32 != 0)
                        {
                            npc.direction = num32;
                            if (npc.spriteDirection == 1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.spriteDirection = -npc.direction;
                        }
                    }
                    else if (aiState == 2)
                    {
                        npc.ai[0] = 23f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 3)
                    {
                        npc.ai[0] = 24f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    else if (aiState == 4)
                    {
                        npc.ai[0] = 25f;
                        npc.ai[1] = 0f;
                        npc.ai[2] = 0f;
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 22f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                npc.alpha -= 25;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
                int num34 = 7;
                for (int m = 0; m < num34; m++)
                {
                    Vector2 vector11 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector11 = vector11.RotatedBy((double)(m - (num34 / 2 - 1)) * 3.1415926535897931 / (double)(float)num34, default) + vectorCenter;
                    Vector2 value8 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num35 = Dust.NewDust(vector11 + value8, 0, 0, 244, value8.X * 2f, value8.Y * 2f, 100, default, 1.4f);
                    Main.dust[num35].noGravity = true;
                    Main.dust[num35].noLight = true;
                    Main.dust[num35].velocity /= 4f;
                    Main.dust[num35].velocity -= npc.velocity;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)chargeTime)
                {
                    npc.ai[0] = 21f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 23f)
            {
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                if (npc.alpha < 255)
                {
                    npc.alpha += 17;
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                    }
                }
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1460 / 2))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == (float)(num1460 / 2))
                {
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = (float)(300 * Math.Sign((vectorCenter - player.Center).X));
                    }
                    Vector2 center = player.Center + new Vector2(-npc.ai[1], teleportLocation); //teleport distance
                    vectorCenter = npc.Center = center;
                    int num36 = Math.Sign(player.Center.X - vectorCenter.X);
                    if (num36 != 0)
                    {
                        if (npc.ai[2] == 0f && num36 != npc.direction)
                        {
                            npc.rotation += 3.14159274f;
                        }
                        npc.direction = num36;
                        if (npc.spriteDirection != -npc.direction)
                        {
                            npc.rotation += 3.14159274f;
                        }
                        npc.spriteDirection = -npc.direction;
                    }
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1460)
                {
                    npc.ai[0] = 21f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    if (npc.ai[3] == 5f && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(vectorCenter.X, vectorCenter.Y, 0f, 0f, ModContent.ProjectileType<BigFlare>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                    }
                    if (npc.ai[3] >= 20f) //14
                    {
                        npc.ai[3] = 0f;
                    }
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 24f)
            {
                npc.dontTakeDamage = phase5Check;
                npc.chaseable = true;
                if (npc.ai[2] == 0f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                npc.velocity = npc.velocity.RotatedBy((double)(-(double)num1463 * (float)npc.direction), default);
                npc.rotation -= num1463 * (float)npc.direction;
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1461)
                {
                    npc.ai[0] = 21f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 25f) //start phase 5
            {
                npc.alpha = 0;
                npc.dontTakeDamage = true;
                npc.chaseable = false;
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == (float)(num1459 - 60))
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= (float)num1459)
                {
                    startSecondAI = true;
                    npc.ai[0] = 0f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            #endregion
        }

        #region AI2
        public void Yharon_AI2()
        {
            bool revenge = CalamityWorld.revenge || CalamityWorld.bossRushActive;
            bool expertMode = Main.expertMode || CalamityWorld.bossRushActive;
            bool phase2 = (double)npc.life <= (double)npc.lifeMax * 0.75;
            bool phase3 = (double)npc.life <= (double)npc.lifeMax * 0.5;
            bool phase4 = (double)npc.life <= (double)npc.lifeMax * 0.05;
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                phase2 = (double)npc.life <= (double)npc.lifeMax * 0.95;
                phase3 = (double)npc.life <= (double)npc.lifeMax * 0.7;
                phase4 = (double)npc.life <= (double)npc.lifeMax * 0.15;
            }
            else if (revenge)
            {
                phase2 = (double)npc.life <= (double)npc.lifeMax * 0.85;
                phase3 = (double)npc.life <= (double)npc.lifeMax * 0.6;
                phase4 = (double)npc.life <= (double)npc.lifeMax * 0.1;
            }
            if (npc.ai[0] != 8f)
            {
                npc.alpha -= 25;
                if (npc.alpha < 0)
                {
                    npc.alpha = 0;
                }
            }
            if (!moveCloser)
            {
                Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
                if (calamityModMusic != null)
                    music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/DragonGod");
                else
                    music = MusicID.LunarBoss;
                moveCloser = true;
                string key = "Mods.CalamityMod.FlameText";
                Color messageColor = Color.Orange;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }
            if (invincibilityCounter < 900)
            {
                phase2 = phase3 = phase4 = false;
                invincibilityCounter += 1;
                int heal = 5; //900 / 5 = 180
                healCounter += 1;
                if (healCounter >= heal)
                {
                    healCounter = 0;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int healAmt = npc.lifeMax / 200;
                        if (healAmt > npc.lifeMax - npc.life)
                        {
                            healAmt = npc.lifeMax - npc.life;
                        }
                        if (healAmt > 0)
                        {
                            npc.life += healAmt;
                            npc.HealEffect(healAmt, true);
                            npc.netUpdate = true;
                        }
                    }
                }
            }
            else
            {
                npc.dontTakeDamage = npc.ai[0] == 9f;
                npc.chaseable = npc.ai[0] < 8f;
            }
            NPCUtils.TargetClosestBetsy(npc, false, null);
            NPCAimedTarget targetData = npc.GetTargetData(true);
            if (!targetData.Hitbox.Intersects(safeBox))
            {
                protectionBoost = true;
                npc.damage = npc.defDamage * 5;
                if (npc.timeLeft > 150)
                {
                    npc.timeLeft = 150;
                }
            }
            else
            {
                npc.damage = npc.defDamage;
                if (phase4)
                {
                    npc.damage = (int)((float)npc.defDamage * 1.25f);
                }
                protectionBoost = false;
                if (npc.timeLeft < 3600)
                {
                    npc.timeLeft = 3600;
                }
            }

            // Set DR based on protection boost (aka enrage)
            npc.Calamity().DR = protectionBoost ? EnragedDR : Phase2_DR;

            int num = -1;
            float num2 = 1f;
            int num4 = expertMode ? 110 : 125;
            if (phase4)
            {
                num4 = (int)((double)num4 * 1.25);
            }
            float num6 = revenge ? 0.6f : 0.55f;
            float scaleFactor = revenge ? 10f : 9f;
            float chargeTime = 34f;
            float chargeTime2 = 30f;
            float chargeSpeed = revenge ? 26f : 25f;
            float chargeSpeed2 = revenge ? 40f : 38f;
            float num11 = 40f;
            float num12 = 80f;
            float num13 = num11 + num12;
            float num15 = 60f;
            float scaleFactor3 = 14f;
            float scaleFactor4 = revenge ? 16f : 15f; //12
            int num16 = 10;
            int num17 = 6 * num16;
            float num18 = 60f;
            float num19 = num15 + (float)num17 + num18;
            float num20 = 60f;
            float num21 = 1f;
            float num22 = 6.28318548f * (num21 / num20);
            float scaleFactor5 = revenge ? 38f : 36.5f; //32
            if (CalamityWorld.death || CalamityWorld.bossRushActive)
            {
                scaleFactor = 10.5f;
                chargeSpeed = 27f;
                chargeSpeed2 = 41f;
                scaleFactor4 = 16.5f;
                scaleFactor5 = 39f;
            }
            if (npc.Calamity().enraged || (Config.BossRushXerocCurse && CalamityWorld.bossRushActive))
            {
                num6 = 0.65f;
                scaleFactor = 11f;
                chargeSpeed = 32f;
                chargeSpeed2 = 45f;
                scaleFactor4 = 18f;
                scaleFactor5 = 40f;
            }
            float num25 = 20f;
            float arg_F9_0 = npc.ai[0];
            float num26;
            if (npc.ai[0] == 0f)
            {
                float[] expr_115_cp_0 = npc.ai;
                int expr_115_cp_1 = 1;
                num26 = expr_115_cp_0[expr_115_cp_1] + 1f;
                expr_115_cp_0[expr_115_cp_1] = num26;
                if (num26 >= 10f)
                {
                    npc.ai[1] = 0f;
                    npc.ai[0] = 1f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 1f)
            {
                if (npc.ai[2] == 0f)
                {
                    npc.ai[2] = (float)((npc.Center.X < targetData.Center.X) ? 1 : -1);
                }
                Vector2 destination = targetData.Center + new Vector2(-npc.ai[2] * 300f, -200f);
                Vector2 desiredVelocity = npc.DirectionTo(destination) * scaleFactor;
                npc.SimpleFlyMovement(desiredVelocity, num6);
                int num27 = (npc.Center.X < targetData.Center.X) ? 1 : -1;
                npc.direction = npc.spriteDirection = num27;
                float[] expr_225_cp_0 = npc.ai;
                int expr_225_cp_1 = 1;
                num26 = expr_225_cp_0[expr_225_cp_1] + 1f;
                expr_225_cp_0[expr_225_cp_1] = num26;
                if (num26 >= 30f)
                {
                    int num28 = 1;
                    if (phase4)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 8; //teleport
                                break;
                            case 1:
                            case 2:
                                num28 = 7; //fast charge
                                break;
                            case 3:
                                num28 = 5; //fire circle + tornado (only once) + fireballs
                                break;
                        }
                    }
                    else if (phase3)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 6; //tornado
                                break;
                            case 1:
                                num28 = 7; //fast charge
                                break;
                            case 2:
                                num28 = 8; //teleport
                                break;
                            case 3:
                                num28 = 7; //fast charge
                                break;
                            case 4:
                                num28 = 5; //fire circle
                                break;
                            case 5:
                                num28 = 4; //fireballs
                                break;
                            case 6:
                                num28 = 7; //fast charge
                                break;
                            case 7:
                                num28 = 8; //teleport
                                break;
                            case 8:
                                num28 = 7; //fast charge
                                break;
                            case 9:
                                num28 = 3; //fireballs
                                break;
                            case 10:
                                num28 = 6; //tornado
                                break;
                            case 11:
                                num28 = 7; //fast charge
                                break;
                            case 12:
                                num28 = 8; //teleport
                                break;
                            case 13:
                                num28 = 7; //fast charge
                                break;
                            case 14:
                                num28 = 5; //fire circle
                                break;
                            case 15:
                                num28 = 4; //fireballs
                                break;
                        }
                    }
                    else if (phase2)
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 6; //tornado
                                break;
                            case 1:
                                num28 = 7; //fast charge
                                break;
                            case 2:
                                num28 = 2; //charge
                                break;
                            case 3:
                                num28 = 5; //fire circle
                                break;
                            case 4:
                                num28 = 4; //fireballs
                                break;
                            case 5:
                                num28 = 7; //fast charge
                                break;
                            case 6:
                                num28 = 2; //charge
                                break;
                            case 7:
                                num28 = 3; //fireballs
                                break;
                            case 8:
                                num28 = 7; //fast charge
                                break;
                            case 9:
                                num28 = 2; //charge
                                break;
                            case 10:
                                num28 = 5; //fire circle
                                break;
                        }
                    }
                    else
                    {
                        switch ((int)npc.ai[3])
                        {
                            case 0:
                                num28 = 6; //tornado
                                break;
                            case 1:
                            case 2:
                                num28 = 2; //charge
                                break;
                            case 3:
                                num28 = 3; //fireballs
                                break;
                            case 4:
                            case 5:
                                num28 = 2; //charge
                                break;
                            case 6:
                                num28 = 4; //fireballs
                                break;
                            case 7:
                            case 8:
                                num28 = 2; //charge
                                break;
                            case 9:
                                num28 = 5; //fire circle
                                break;
                        }
                    }
                    npc.ai[0] = (float)num28;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] += 1f;
                    switch (secondPhasePhase)
                    {
                        case 1:
                            if (phase2)
                            {
                                secondPhasePhase = 2;
                                npc.ai[0] = 9f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                            break;
                        case 2:
                            if (phase3)
                            {
                                secondPhasePhase = 3;
                                npc.ai[0] = 9f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                            break;
                        case 3:
                            if (phase4)
                            {
                                secondPhasePhase = 4;
                                npc.ai[0] = 9f;
                                npc.ai[1] = 0f;
                                npc.ai[2] = 0f;
                                npc.ai[3] = 0f;
                            }
                            break;
                    }
                    npc.netUpdate = true;
                    float aiLimit = 10f;
                    if (phase4)
                    {
                        aiLimit = 4f;
                    }
                    else if (phase3)
                    {
                        aiLimit = 16f;
                    }
                    else if (phase2)
                    {
                        aiLimit = 11f;
                    }
                    if (npc.ai[3] >= aiLimit)
                    {
                        npc.ai[3] = 0f;
                    }
                    switch (num28)
                    {
                        case 2: //charge
                        {
                            Vector2 vector = npc.DirectionTo(targetData.Center);
                            npc.spriteDirection = (vector.X > 0f) ? 1 : -1;
                            npc.rotation = vector.ToRotation();
                            if (npc.spriteDirection == -1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.velocity = vector * chargeSpeed;
                            break;
                        }
                        case 3: //fireballs
                        {
                            Vector2 vector2 = new Vector2((float)((targetData.Center.X > npc.Center.X) ? 1 : -1), 0f);
                            npc.spriteDirection = (vector2.X > 0f) ? 1 : -1;
                            npc.velocity = vector2 * -2f;
                            break;
                        }
                        case 5: //spin move
                        {
                            Vector2 vector3 = npc.DirectionTo(targetData.Center);
                            npc.spriteDirection = (vector3.X > 0f) ? 1 : -1;
                            npc.rotation = vector3.ToRotation();
                            if (npc.spriteDirection == -1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.velocity = vector3 * scaleFactor5;
                            break;
                        }
                        case 7: //fast charge
                        {
                            Vector2 vector = npc.DirectionTo(targetData.Center);
                            npc.spriteDirection = (vector.X > 0f) ? 1 : -1;
                            npc.rotation = vector.ToRotation();
                            if (npc.spriteDirection == -1)
                            {
                                npc.rotation += 3.14159274f;
                            }
                            npc.velocity = vector * chargeSpeed2;
                            break;
                        }
                    }
                }
            }
            else if (npc.ai[0] == 2f)
            {
                if (npc.ai[1] == 1f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                int num1473 = 7;
                for (int num1474 = 0; num1474 < num1473; num1474++)
                {
                    Vector2 vector171 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector171 = vector171.RotatedBy((double)(num1474 - (num1473 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1473, default) + npc.Center;
                    Vector2 value18 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 244, value18.X * 2f, value18.Y * 2f, 100, default, 1.4f);
                    Main.dust[num1475].noGravity = true;
                    Main.dust[num1475].noLight = true;
                    Main.dust[num1475].velocity /= 4f;
                    Main.dust[num1475].velocity -= npc.velocity;
                }
                float[] expr_498_cp_0 = npc.ai;
                int expr_498_cp_1 = 1;
                num26 = expr_498_cp_0[expr_498_cp_1] + 1f;
                expr_498_cp_0[expr_498_cp_1] = num26;
                if (num26 >= chargeTime)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }
            else if (npc.ai[0] == 3f) //fireball spit
            {
                npc.ai[1] += 1f;
                int num29 = (npc.Center.X < targetData.Center.X) ? 1 : -1;
                npc.ai[2] = (float)num29;
                if (npc.ai[1] < num11)
                {
                    Vector2 vector4 = targetData.Center + new Vector2((float)num29 * -600f, -250f);
                    Vector2 value = npc.DirectionTo(vector4) * 12f;
                    if (npc.Distance(vector4) < 12f)
                    {
                        npc.Center = vector4;
                    }
                    else
                    {
                        npc.position += value;
                    }
                    if (Vector2.Distance(vector4, npc.Center) < 16f)
                    {
                        npc.ai[1] = num11 - 1f;
                    }
                    num2 = 1.5f;
                }
                if (npc.ai[1] == num11)
                {
                    int num30 = (targetData.Center.X > npc.Center.X) ? 1 : -1;
                    npc.velocity = new Vector2((float)num30, 0f) * 22f; //10f
                    npc.direction = npc.spriteDirection = num30;
                }
                if (npc.ai[1] >= num11)
                {
                    if (npc.ai[1] % 8 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float num33 = 30f;
                        Vector2 position = npc.Center + new Vector2((110f + num33) * (float)npc.direction, -20f).RotatedBy((double)npc.rotation,
                            default);
                        float speed = 0.01f;
                        Vector2 vectorShoot = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                        float playerX = targetData.Center.X - vectorShoot.X;
                        float playerY = targetData.Center.Y - vectorShoot.Y;
                        float playerXY = (float)Math.Sqrt((double)(playerX * playerX + playerY * playerY));
                        playerXY = speed / playerXY;
                        playerX *= playerXY;
                        playerY *= playerXY;
                        Projectile.NewProjectile(position.X, position.Y, playerX, playerY, ModContent.ProjectileType<FlareDust2>(), num4, 0f, Main.myPlayer, 1f, 0f);
                    }
                    num2 = 1.5f;
                    if (Math.Abs(targetData.Center.X - npc.Center.X) > 550f && Math.Abs(npc.velocity.X) < 20f)
                    {
                        npc.velocity.X = npc.velocity.X + (float)Math.Sign(npc.velocity.X) * 0.5f;
                    }
                }
                if (npc.ai[1] >= num13)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }
            else if (npc.ai[0] == 4f) //fireball spit
            {
                int num31 = (npc.Center.X < targetData.Center.X) ? 1 : -1;
                npc.ai[2] = (float)num31;
                if (npc.ai[1] < num15)
                {
                    Vector2 vector5 = targetData.Center + new Vector2((float)num31 * -1500f, -350f);
                    Vector2 value2 = npc.DirectionTo(vector5) * scaleFactor3;
                    npc.velocity = Vector2.Lerp(npc.velocity, value2, 0.0333333351f);
                    int num32 = (npc.Center.X < targetData.Center.X) ? 1 : -1;
                    npc.direction = npc.spriteDirection = num32;
                    if (Vector2.Distance(vector5, npc.Center) < 16f)
                    {
                        npc.ai[1] = num15 - 1f;
                    }
                    num2 = 1.5f;
                }
                else if (npc.ai[1] == num15)
                {
                    Vector2 vector6 = npc.DirectionTo(targetData.Center);
                    vector6.Y *= 0.25f;
                    vector6 = vector6.SafeNormalize(Vector2.UnitX * (float)npc.direction);
                    npc.spriteDirection = (vector6.X > 0f) ? 1 : -1;
                    npc.rotation = vector6.ToRotation();
                    if (npc.spriteDirection == -1)
                    {
                        npc.rotation += 3.14159274f;
                    }
                    npc.velocity = vector6 * scaleFactor4;
                }
                else
                {
                    npc.position.X = npc.position.X + npc.DirectionTo(targetData.Center).X * 7f;
                    npc.position.Y = npc.position.Y + npc.DirectionTo(targetData.Center + new Vector2(0f, -400f)).Y * 6f;
                    if (npc.ai[1] <= num19 - num18)
                    {
                        num2 = 1.5f;
                    }
                    float num33 = 30f;
                    Vector2 position = npc.Center + new Vector2((110f + num33) * (float)npc.direction, -20f).RotatedBy((double)npc.rotation,
                        default);
                    int num34 = (int)(npc.ai[1] - num15 + 1f);
                    if (num34 <= num17 && num34 % num16 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(position, npc.velocity, ModContent.ProjectileType<YharonFireball>(), num4, 0f, Main.myPlayer, 0f, 0f);
                    }
                }
                if (npc.ai[1] > num19 - num18)
                {
                    npc.velocity.Y = npc.velocity.Y - 0.1f;
                }
                npc.ai[1] += 1f;
                if (npc.ai[1] >= num19)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }
            else if (npc.ai[0] == 5f) //spin 2 win
            {
                npc.velocity = npc.velocity.RotatedBy((double)(-(double)num22 * (float)npc.direction), default);
                npc.position.Y = npc.position.Y - 0.1f;
                npc.position += npc.DirectionTo(targetData.Center) * 10f;
                npc.rotation -= num22 * (float)npc.direction;
                num2 *= 0.7f;
                if (npc.ai[1] == 1f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                float[] expr_B0F_cp_0 = npc.ai;
                int expr_B0F_cp_1 = 1;
                num26 = expr_B0F_cp_0[expr_B0F_cp_1] + 1f;
                expr_B0F_cp_0[expr_B0F_cp_1] = num26;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (npc.ai[1] % 12 == 0)
                    {
                        float num33 = 30f;
                        Vector2 position = npc.Center + new Vector2((110f + num33) * (float)npc.direction, -20f).RotatedBy((double)npc.rotation,
                            default);
                        Projectile.NewProjectile(position, npc.velocity, ModContent.ProjectileType<YharonFireball>(), num4, 0f, Main.myPlayer, 0f, 0f);
                        if (phase4)
                        {
                            float speed = 0.01f;
                            Vector2 vectorShoot = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f + 30f);
                            float playerX = targetData.Center.X - vectorShoot.X;
                            float playerY = targetData.Center.Y - vectorShoot.Y;
                            float playerXY = (float)Math.Sqrt((double)(playerX * playerX + playerY * playerY));
                            playerXY = speed / playerXY;
                            playerX *= playerXY;
                            playerY *= playerXY;
                            Projectile.NewProjectile(position.X, position.Y, playerX, playerY, ModContent.ProjectileType<FlareDust2>(), num4, 0f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    if (npc.ai[1] == 45f && phase4 && useTornado)
                    {
                        useTornado = false;
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, ModContent.ProjectileType<BigFlare2>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                    }
                }
                if (num26 >= num20)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.velocity /= 2f;
                }
            }
            else if (npc.ai[0] == 6f) //flare spawn
            {
                if (npc.ai[1] == 0f)
                {
                    Vector2 destination2 = targetData.Center + new Vector2(0f, -200f);
                    Vector2 desiredVelocity2 = npc.DirectionTo(destination2) * scaleFactor * 2f;
                    npc.SimpleFlyMovement(desiredVelocity2, num6 * 2f);
                    int num35 = (npc.Center.X < targetData.Center.X) ? 1 : -1;
                    npc.direction = npc.spriteDirection = num35;
                    npc.ai[2] += 1f;
                    if (npc.Distance(targetData.Center) < 1000f || npc.ai[2] >= 180f) //450f
                    {
                        npc.ai[1] = 1f;
                        npc.netUpdate = true;
                    }
                }
                else
                {
                    if (npc.ai[1] == 1f)
                    {
                        Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                    }
                    if (npc.ai[1] < num25)
                    {
                        npc.velocity *= 0.95f;
                    }
                    else
                    {
                        npc.velocity *= 0.98f;
                    }
                    if (npc.ai[1] == num25)
                    {
                        if (npc.velocity.Y > 0f)
                        {
                            npc.velocity.Y = npc.velocity.Y / 3f;
                        }
                        npc.velocity.Y = npc.velocity.Y - 3f;
                    }
                    num2 *= 0.85f;
                    bool flag3 = npc.ai[1] == 20f || npc.ai[1] == 45f || npc.ai[1] == 70f;
                    int flareCount = NPC.CountNPCS(ModContent.NPCType<DetonatingFlare>()) + NPC.CountNPCS(ModContent.NPCType<DetonatingFlare2>());
                    if (flareCount > 5)
                    {
                        flag3 = false;
                    }
                    if (flag3 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 vector7 = npc.Center + (6.28318548f * Main.rand.NextFloat()).ToRotationVector2() * new Vector2(2f, 1f) * 100f * (0.6f + Main.rand.NextFloat() * 0.4f);
                        if (Vector2.Distance(vector7, targetData.Center) > 100f)
                        {
                            Point point2 = vector7.ToPoint();
                            NPC.NewNPC(point2.X, point2.Y, ModContent.NPCType<DetonatingFlare>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                            NPC.NewNPC(point2.X, point2.Y, ModContent.NPCType<DetonatingFlare2>(), npc.whoAmI, 0f, 0f, 0f, 0f, 255);
                        }
                        NPC.NewNPC((int)npc.Center.X + (Main.rand.NextBool(2) ? 100 : -100), (int)npc.Center.Y - 100, ModContent.NPCType<DetonatingFlare>(), 0, 0f, 0f, 0f, 0f, 255);
                        NPC.NewNPC((int)npc.Center.X + (Main.rand.NextBool(2) ? 100 : -100), (int)npc.Center.Y - 100, ModContent.NPCType<DetonatingFlare2>(), 0, 0f, 0f, 0f, 0f, 255);
                    }
                    npc.ai[1] += 1f;
                }
                if (npc.ai[1] >= 90f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(npc.Center.X, npc.Center.Y, 0f, 0f, ModContent.ProjectileType<BigFlare2>(), 0, 0f, Main.myPlayer, 1f, (float)(npc.target + 1));
                        Boom(600, num4);
                    }
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }
            else if (npc.ai[0] == 7f) //speedee chargee
            {
                if (npc.ai[1] == 1f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                int num1473 = 7;
                for (int num1474 = 0; num1474 < num1473; num1474++)
                {
                    Vector2 vector171 = Vector2.Normalize(npc.velocity) * new Vector2((float)(npc.width + 50) / 2f, (float)npc.height) * 0.75f;
                    vector171 = vector171.RotatedBy((double)(num1474 - (num1473 / 2 - 1)) * 3.1415926535897931 / (double)(float)num1473, default) + npc.Center;
                    Vector2 value18 = ((float)(Main.rand.NextDouble() * 3.1415927410125732) - 1.57079637f).ToRotationVector2() * (float)Main.rand.Next(3, 8);
                    int num1475 = Dust.NewDust(vector171 + value18, 0, 0, 244, value18.X * 2f, value18.Y * 2f, 100, default, 1.4f);
                    Main.dust[num1475].noGravity = true;
                    Main.dust[num1475].noLight = true;
                    Main.dust[num1475].velocity /= 4f;
                    Main.dust[num1475].velocity -= npc.velocity;
                }
                float[] expr_498_cp_0 = npc.ai;
                int expr_498_cp_1 = 1;
                num26 = expr_498_cp_0[expr_498_cp_1] + 1f;
                expr_498_cp_0[expr_498_cp_1] = num26;
                if (num26 >= chargeTime2)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                }
            }
            else if (npc.ai[0] == 8f) //teleport
            {
                Vector2 npcCenter = npc.Center;
                if (npc.alpha < 255)
                {
                    npc.alpha += 17;
                    if (npc.alpha > 255)
                    {
                        npc.alpha = 255;
                    }
                }
                npc.velocity *= 0.98f;
                npc.velocity.Y = MathHelper.Lerp(npc.velocity.Y, 0f, 0.02f);
                if (npc.ai[2] == 15f)
                {
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoarShort"), (int)npc.position.X, (int)npc.position.Y);
                }
                if (Main.netMode != NetmodeID.MultiplayerClient && npc.ai[2] == 15f)
                {
                    if (npc.ai[1] == 0f)
                    {
                        npc.ai[1] = (float)(300 * Math.Sign((npcCenter - targetData.Center).X));
                    }
                    teleportLocation = Main.rand.NextBool(2) ? (revenge ? 600 : 700) : (revenge ? -600 : -700);
                    Vector2 center = targetData.Center + new Vector2(-npc.ai[1], (float)teleportLocation); //teleport distance
                    npcCenter = npc.Center = center;
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 25f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.netUpdate = true;
                }
            }
            else if (npc.ai[0] == 9f) //enter next phase
            {
                npc.chaseable = false;
                npc.velocity *= 0.95f;
                Vector2 vector = npc.DirectionTo(targetData.Center);
                npc.spriteDirection = (vector.X > 0f) ? 1 : -1;
                npc.rotation = vector.ToRotation();
                if (npc.spriteDirection == -1)
                {
                    npc.rotation += 3.14159274f;
                }
                if (npc.ai[2] == 120f)
                {
                    if (phase4)
                    {
                        int proj;
                        for (int x = 0; x < 1000; x = proj + 1)
                        {
                            Projectile projectile = Main.projectile[x];
                            if (projectile.active)
                            {
                                if (projectile.type == ModContent.ProjectileType<Infernado2>())
                                {
                                    if (projectile.timeLeft >= 300)
                                        projectile.active = false;
                                    else if (projectile.timeLeft > 5)
                                        projectile.timeLeft = (int)(5f * projectile.ai[1]);
                                }
                                else if (projectile.type == ModContent.ProjectileType<BigFlare2>())
                                    projectile.active = false;
                            }
                            proj = x;
                        }
                    }
                    Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/YharonRoar"), (int)npc.position.X, (int)npc.position.Y);
                }
                npc.ai[2] += 1f;
                if (npc.ai[2] >= 180f)
                {
                    npc.ai[0] = 1f;
                    npc.ai[1] = 0f;
                    npc.ai[2] = 0f;
                    npc.ai[3] = 0f;
                    npc.netUpdate = true;
                }
            }
            npc.localAI[0] += num2;
            if (npc.localAI[0] >= 36f)
            {
                npc.localAI[0] = 0f;
            }
            if (num != -1)
            {
                npc.localAI[0] = (float)(num * 4);
            }
            float[] expr_11FC_cp_0 = npc.localAI;
            int expr_11FC_cp_1 = 1;
            num26 = expr_11FC_cp_0[expr_11FC_cp_1] + 1f;
            expr_11FC_cp_0[expr_11FC_cp_1] = num26;
            if (num26 >= 60f)
            {
                npc.localAI[1] = 0f;
            }
            float num42 = npc.DirectionTo(targetData.Center).ToRotation();
            float num43 = 0.04f;
            switch ((int)npc.ai[0])
            {
                case 2:
                case 5:
                case 7:
                case 8:
                case 9:
                    num43 = 0f;
                    break;
                case 3:
                    num43 = 0.01f;
                    num42 = 0f;
                    if (npc.spriteDirection == -1)
                    {
                        num42 -= 3.14159274f;
                    }
                    if (npc.ai[1] >= num11)
                    {
                        num42 += (float)npc.spriteDirection * 3.14159274f / 12f;
                        num43 = 0.05f;
                    }
                    break;
                case 4:
                    num43 = 0.01f;
                    num42 = 3.14159274f;
                    if (npc.spriteDirection == 1)
                    {
                        num42 += 3.14159274f;
                    }
                    break;
                case 6:
                    num43 = 0.02f;
                    num42 = 0f;
                    if (npc.spriteDirection == -1)
                    {
                        num42 -= 3.14159274f;
                    }
                    break;
            }
            if (npc.spriteDirection == -1)
            {
                num42 += 3.14159274f;
            }
            if (num43 != 0f)
            {
                npc.rotation = npc.rotation.AngleTowards(num42, num43);
            }
        }
        #endregion

        #region Drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            Texture2D texture = Main.npcTexture[npc.type];
            Rectangle frame6 = npc.frame;
            Color color9 = Lighting.GetColor((int)((double)npc.position.X + (double)npc.width * 0.5) / 16, (int)(((double)npc.position.Y + (double)npc.height * 0.5) / 16.0));
            int num156 = texture.Height / Main.npcFrameCount[npc.type];
            int y3 = num156 * (int)npc.frameCounter;
            Rectangle rectangle = new Rectangle(0, y3, texture.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            int num157 = 8;
            int num158 = 2;
            int num159 = 1;
            float num160 = 0f;
            int num161 = num159;
            if (startSecondAI && !phaseOneLoot)
            {
                bool drawAfterImage2 = (npc.ai[0] == 2f || npc.ai[0] == 3f || npc.ai[0] == 4f || npc.ai[0] == 5f || npc.ai[0] == 7f) && Lighting.NotRetro;
                SpriteEffects spriteEffects2 = spriteEffects ^ SpriteEffects.FlipHorizontally;
                if (invincibilityCounter < 900)
                {
                    color9 = new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, 0);
                }
                Color alpha16 = npc.GetAlpha(color9);
                while (drawAfterImage2 && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
                {
                    goto IL_6899;
                    IL_6881:
                    num161 += num158;
                    continue;
                    IL_6899:
                    float num164 = (float)(num157 - num161);
                    if (num158 < 0)
                    {
                        num164 = (float)(num159 - num161);
                    }
                    alpha16 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f); //1.5
                    Vector2 value4 = npc.oldPos[num161];
                    float num165 = npc.rotation;
                    Main.spriteBatch.Draw(texture, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), alpha16, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects2.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects2, 0f);
                    goto IL_6881;
                }
                Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), npc.GetAlpha(color9), npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects2, 0);
                return false;
            }
            Color alpha15 = npc.GetAlpha(color9);
            bool drawAfterImage = (npc.ai[0] == 1f || npc.ai[0] == 5f || npc.ai[0] == 7f || npc.ai[0] == 8f || npc.ai[0] == 11f || npc.ai[0] == 12f ||
                npc.ai[0] == 14f || npc.ai[0] == 15f || npc.ai[0] == 18f || npc.ai[0] == 19f || npc.ai[0] == 22f) && Lighting.NotRetro;
            while (drawAfterImage && ((num158 > 0 && num161 < num157) || (num158 < 0 && num161 > num157)))
            {
                goto IL_6899;
                IL_6881:
                num161 += num158;
                continue;
                IL_6899:
                float num164 = (float)(num157 - num161);
                if (num158 < 0)
                {
                    num164 = (float)(num159 - num161);
                }
                alpha15 *= num164 / ((float)NPCID.Sets.TrailCacheLength[npc.type] * 1.5f); //1.5
                Vector2 value4 = npc.oldPos[num161];
                float num165 = npc.rotation;
                Main.spriteBatch.Draw(texture, value4 + npc.Size / 2f - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), alpha15, num165 + npc.rotation * num160 * (float)(num161 - 1) * -(float)spriteEffects.HasFlag(SpriteEffects.FlipHorizontally).ToDirectionInt(), origin2, npc.scale, spriteEffects, 0f);
                goto IL_6881;
            }
            Main.spriteBatch.Draw(texture, npc.Center - Main.screenPosition + new Vector2(0, npc.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(frame6), npc.GetAlpha(color9), npc.rotation, npc.frame.Size() / 2, npc.scale, spriteEffects, 0);
            return false;
        }
        #endregion

        #region Loot
        public override bool SpecialNPCLoot()
        {
            return !dropLoot;
        }

        public override void NPCLoot()
        {
            // If Yharon runs away in phase 1 and the Eclipse isn't buffed yet, notify players of the buffed Solar Eclipse
            if (!startSecondAI && !CalamityWorld.buffedEclipse)
            {
                CalamityWorld.buffedEclipse = true;
                CalamityMod.UpdateServerBoolean();

                string key = "Mods.CalamityMod.DargonBossText";
                Color messageColor = Color.Orange;

                if (Main.netMode == NetmodeID.SinglePlayer)
                    Main.NewText(Language.GetTextValue(key), messageColor);
                else if (Main.netMode == NetmodeID.Server)
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }

            // Bags occur in either phase 1 or 2, as they don't contain phase 2 only drops
            DropHelper.DropBags(npc);

            // Phase 1 drops: Contained in the bag, so they only drop directly on Normal
            if (!Main.expertMode)
            {
                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<DragonRage>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<TheBurningSky>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<DragonsBreath>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ChickenCannon>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<PhoenixFlameBarrage>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<AngryChickenStaff>(), 4);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ProfanedTrident>(), 4);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<YharonMask>(), 7);
                DropHelper.DropItemChance(npc, ModContent.ItemType<ForgottenDragonEgg>(), 10);
            }

            // These drops only occur in Phase 2 (where you actually kill Yharon)
            if (startSecondAI)
            {
                // Materials
                int soulFragMin = Main.expertMode ? 22 : 15;
                int soulFragMax = Main.expertMode ? 28 : 22;
                DropHelper.DropItem(npc, ModContent.ItemType<HellcasterFragment>(), true, soulFragMin, soulFragMax);

                // Equipment
                DropHelper.DropItem(npc, ModContent.ItemType<DrewsWings>());

                // Weapons
                DropHelper.DropItemChance(npc, ModContent.ItemType<VoidVortex>(), DropHelper.RareVariantDropRateInt);
                DropHelper.DropItemChance(npc, ModContent.ItemType<YharimsCrystal>(), DropHelper.LegendaryDropRateInt);

                // Vanity
                DropHelper.DropItemChance(npc, ModContent.ItemType<YharonTrophy>(), 10);

                // Other
                DropHelper.DropItem(npc, ModContent.ItemType<BossRush>());
                DropHelper.DropItemCondition(npc, ModContent.ItemType<KnowledgeYharon>(), true, !CalamityWorld.downedYharon);
                DropHelper.DropResidentEvilAmmo(npc, CalamityWorld.downedYharon, 6, 3, 2);

                // If Yharon has not been killed yet, notify players of Auric Ore
                if (!CalamityWorld.downedYharon)
                {
                    WorldGenerationMethods.SpawnOre(ModContent.TileType<AuricOre>(), 2E-05, .6f, .8f);

                    string key = "Mods.CalamityMod.AuricOreText";
                    Color messageColor = Color.Gold;
                    if (Main.netMode == NetmodeID.SinglePlayer)
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    else if (Main.netMode == NetmodeID.Server)
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }

                // Mark Yharon as dead
                CalamityWorld.downedYharon = true;
                CalamityMod.UpdateServerBoolean();
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }
        #endregion

        #region StrikeNPC
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (CalamityUtils.AntiButcher(npc, ref damage, 0.1f))
                return false;

            // Safeguard to prevent damage which would allow skipping phase 2.
            if (!startSecondAI && dropLoot)
            {
                damage = 0;
                return false;
            }
            return true;
        }
        #endregion

        #region HPBarCooldownSlotandStats
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 2f;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * 0.8f * bossLifeScale);
            npc.damage = (int)(npc.damage * 0.8f);
        }
        #endregion

        #region FindFrame
        public override void FindFrame(int frameHeight)
        {
            if ((!startSecondAI && (npc.ai[0] == 0f || npc.ai[0] == 6f || npc.ai[0] == 13f || npc.ai[0] == 21f)) ||
                (startSecondAI && (npc.ai[0] == 5f || npc.ai[0] < 2f))) //idle
            {
                int num84 = 4; //5
                if (!startSecondAI && (npc.ai[0] == 6f || npc.ai[0] == 13f || npc.ai[0] == 21f)) //Phase ai switch
                {
                    num84 = 3; //4
                }
                npc.frameCounter += 1.0;
                if (npc.frameCounter > (double)num84)
                {
                    npc.frameCounter = 0.0;
                    npc.frame.Y = npc.frame.Y + frameHeight;
                }
                if (npc.frame.Y >= frameHeight * 5) //6
                {
                    npc.frame.Y = 0;
                }
            }
            if ((!startSecondAI && (npc.ai[0] == 1f || npc.ai[0] == 5f || npc.ai[0] == 7f || npc.ai[0] == 11f || npc.ai[0] == 14f || npc.ai[0] == 18f || npc.ai[0] == 22f)) ||
                (startSecondAI && (npc.ai[0] == 6f || npc.ai[0] == 2f || npc.ai[0] == 7f))) //Charging or birb spawn
            {
                npc.frame.Y = frameHeight * 5; //6
            }
            if ((!startSecondAI && (npc.ai[0] == 2f || npc.ai[0] == 8f || npc.ai[0] == 12f || npc.ai[0] == 15f || npc.ai[0] == 19f || npc.ai[0] == 23f)) ||
                (startSecondAI && (npc.ai[0] == 4f || npc.ai[0] == 3f || npc.ai[0] == 8f))) //Fireball spit, teleport, circle, flamethrower
            {
                npc.frame.Y = frameHeight * 5; //6
            }
            if (!startSecondAI && (npc.ai[0] == 3f || npc.ai[0] == 9f || npc.ai[0] == -1f || npc.ai[0] == 16f || npc.ai[0] == 20f || npc.ai[0] == 24f)) //Summon tornadoes
            {
                int num85 = 90;
                if (npc.ai[2] < (float)(num85 - 30) || npc.ai[2] > (float)(num85 - 10))
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 4.0) //5
                    {
                        npc.frameCounter = 0.0;
                        npc.frame.Y = npc.frame.Y + frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 5) //6
                    {
                        npc.frame.Y = 0;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * 5; //6
                    if (npc.ai[2] > (float)(num85 - 20) && npc.ai[2] < (float)(num85 - 15))
                    {
                        npc.frame.Y = frameHeight * 6; //7
                    }
                }
            }
            if ((!startSecondAI && (npc.ai[0] == 4f || npc.ai[0] == 10f || npc.ai[0] == 17f || npc.ai[0] == 25f)) ||
                (startSecondAI && npc.ai[0] == 9f)) //Enter new phase
            {
                int num86 = 180;
                if (npc.ai[2] < (float)(num86 - 60) || npc.ai[2] > (float)(num86 - 20))
                {
                    npc.frameCounter += 1.0;
                    if (npc.frameCounter > 4.0) //5
                    {
                        npc.frameCounter = 0.0;
                        npc.frame.Y = npc.frame.Y + frameHeight;
                    }
                    if (npc.frame.Y >= frameHeight * 5) //6
                    {
                        npc.frame.Y = 0;
                    }
                }
                else
                {
                    npc.frame.Y = frameHeight * 5; //6
                    if (npc.ai[2] > (float)(num86 - 50) && npc.ai[2] < (float)(num86 - 25))
                    {
                        npc.frame.Y = frameHeight * 6; //7
                    }
                }
            }
        }
        #endregion

        #region Boom
        public void Boom(int timeLeft, int damage)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 valueBoom = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                float spreadBoom = 15f * 0.0174f;
                double startAngleBoom = Math.Atan2(npc.velocity.X, npc.velocity.Y) - spreadBoom / 2;
                double deltaAngleBoom = spreadBoom / 8f;
                double offsetAngleBoom;
                int iBoom;
                for (iBoom = 0; iBoom < 25; iBoom++)
                {
                    offsetAngleBoom = startAngleBoom + deltaAngleBoom * (iBoom + iBoom * iBoom) / 2f + 32f * iBoom;
                    int boom1 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(Math.Sin(offsetAngleBoom) * 5f), (float)(Math.Cos(offsetAngleBoom) * 5f), ModContent.ProjectileType<FlareBomb>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    int boom2 = Projectile.NewProjectile(valueBoom.X, valueBoom.Y, (float)(-Math.Sin(offsetAngleBoom) * 5f), (float)(-Math.Cos(offsetAngleBoom) * 5f), ModContent.ProjectileType<FlareBomb>(), damage, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[boom1].timeLeft = timeLeft;
                    Main.projectile[boom2].timeLeft = timeLeft;
                }
            }
        }
        #endregion

        #region HitEffect
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(npc.position, npc.width, npc.height, 5, hitDirection, -1f, 0, default, 1f);
            }
            if (npc.life <= 0)
            {
                Boom(150, 1000);
                npc.position.X = npc.position.X + (float)(npc.width / 2);
                npc.position.Y = npc.position.Y + (float)(npc.height / 2);
                npc.width = 300;
                npc.height = 280;
                npc.position.X = npc.position.X - (float)(npc.width / 2);
                npc.position.Y = npc.position.Y - (float)(npc.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, 244, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
        #endregion
    }
}
