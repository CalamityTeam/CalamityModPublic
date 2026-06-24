using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.BrimstoneElemental
{
    [AutoloadBossHead]
    public class BrimstoneElemental : ModNPC
    {
        public enum Elemental
        {
            Brimstone = 0,
            Sand = 1,
            Rare = 2,
            Cloud = 3,
            Water = 4
        }
        public int currentMode = (int)Elemental.Brimstone;

        public static readonly SoundStyle TeleportSound = new("CalamityMod/Sounds/Custom/BrimstoneElemental/Teleport");
        public static readonly SoundStyle HellfireballSound = new("CalamityMod/Sounds/Custom/BrimstoneElemental/Hellfireball", 3);
        public static readonly SoundStyle DartSound = new("CalamityMod/Sounds/Custom/BrimstoneElemental/BrimstoneDartRing", 3);
        public static readonly SoundStyle HideInShellSound = new("CalamityMod/Sounds/Custom/BrimstoneElemental/ShellTransform");
        public static readonly SoundStyle ShellFireSound = new("CalamityMod/Sounds/Custom/BrimstoneElemental/ShellProjectiles", 3);

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 12;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.5f,
                PortraitScale = 0.64f
            };
            value.Position.Y -= 24f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        // These values are all applied alongside their respective GFB attacks (too many to list)
        public static int DartDamage = 19; // 76
        public static int HellblastDamage = 24; // 96
        public static int HellfireballDamage = 24; // 96
        public static int RayDamage = 35; // 140

        public override void SetDefaults()
        {
            NPC.npcSlots = 64f;
            NPC.damage = 56; // 112
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.width = 100;
            NPC.height = 150;
            NPC.defense = 15;
            NPC.value = Item.buyPrice(gold: 12);
            NPC.LifeMaxNERB(30000, 49200, 500000);
            NPC.DR_NERD(0.2f);
            NPC.knockBackResist = 0f;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.netAlways = true;
            NPC.HitSound = SoundID.NPCHit23;
            NPC.DeathSound = SoundID.NPCDeath39;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToWater = true;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BrimstoneCragsBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.BrimstoneElemental")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(currentMode);
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            currentMode = reader.ReadInt32();
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Used for Brimling AI states
            CalamityGlobalNPC.brimstoneElemental = NPC.whoAmI;

            // Emit light
            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1.2f, 0f, 0f);

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool despawnDistance = Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

            if (!player.active || player.dead || despawnDistance)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || despawnDistance)
                {
                    NPC.rotation = NPC.velocity.X * 0.04f;

                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.localAI[0] = 0f;
                        NPC.localAI[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Reset defense
            NPC.defense = NPC.defDefense;
            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = NPC.ai[0] == 4f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Variables for buffing the AI
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool phase2 = lifeRatio < 0.5f && revenge;
            bool phase3 = lifeRatio < 0.33f;

            // Enrage
            if ((!player.ZoneUnderworldHeight || !player.Calamity().ZoneCalamity) && !BossRushEvent.BossRushActive)
            {
                if (calamityGlobalNPC.newAI[3] > 0f)
                    calamityGlobalNPC.newAI[3] -= 1f;
            }
            else
                calamityGlobalNPC.newAI[3] = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = calamityGlobalNPC.newAI[3] <= 0f;

            float enrageScale = 0f;
            if (biomeEnraged && !player.ZoneUnderworldHeight)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 0.5f;
            }
            if (biomeEnraged && !player.Calamity().ZoneCalamity)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 1f;
            }

            NPC.Calamity().DR = NPC.ai[0] == 4f ? 0.6f : 0.2f;

            // Emit dust
            int dustAmt = (NPC.ai[0] == 2f) ? 2 : 1;
            int size = (NPC.ai[0] == 2f) ? 50 : 35;
            if (NPC.ai[0] != 1f)
            {
                for (int i = 0; i < 2; i++)
                {
                    if (Main.rand.Next(3) < dustAmt)
                    {
                        int dust = Dust.NewDust(NPC.Center - new Vector2(size), size * 2, size * 2, DustID.LifeDrain, NPC.velocity.X * 0.5f, NPC.velocity.Y * 0.5f, 90, default, 1.5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0.2f;
                        Main.dust[dust].fadeIn = 1f;
                    }
                }
            }

            // Distance from destination where Brimmy stops moving
            float movementDistanceGateValue = 100f;

            // How fast Brimmy moves to the destination
            float baseVelocity = (death ? 6f : revenge ? 5.5f : expertMode ? 5f : 4.5f) * (NPC.ai[0] == 5f ? 0.05f : NPC.ai[0] == 3f ? 1.5f : 1f);
            baseVelocity += 3f * enrageScale;
            if (expertMode)
                baseVelocity += death ? 3f * (1f - lifeRatio) : 2f * (1f - lifeRatio);

            float baseAcceleration = (death ? 0.12f : 0.1f) * (NPC.ai[0] == 5f ? 0.5f : NPC.ai[0] == 3f ? 1.5f : 1f);
            baseAcceleration += 0.06f * enrageScale;
            if (expertMode)
                baseAcceleration += 0.03f * (1f - lifeRatio);

            // This is where Brimmy should be
            Vector2 destination = NPC.ai[0] != 3f ? player.Center : new Vector2(player.Center.X, player.Center.Y - 300f);

            // How far Brimmy is from where she's supposed to be
            Vector2 distanceFromDestination = destination - NPC.Center;

            // Movement
            if (NPC.ai[0] != 4f)
                CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, baseVelocity, baseAcceleration, true);

            // Rotation and direction
            if (NPC.ai[0] <= 2f || NPC.ai[0] == 5f)
            {
                NPC.rotation = NPC.velocity.X * 0.04f;
                if (NPC.ai[0] != 5 || (NPC.ai[1] < 180f && NPC.ai[0] == 5f))
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0f ? 1 : -1;
                    NPC.spriteDirection = NPC.direction;
                }
            }

            if (Main.zenithWorld) // in gfb, Brimmy channels the power of the other elementals.
            {
                int newMode;
                if (lifeRatio <= 0.8f && lifeRatio > 0.6f)
                {
                    newMode = 1; // Sand
                }
                else if (lifeRatio <= 0.6f && lifeRatio > 0.4f)
                {
                    newMode = 2; // Rare Sand
                }
                else if (lifeRatio <= 0.4f && lifeRatio > 0.2f)
                {
                    newMode = 3; // Cloud
                }
                else if (lifeRatio <= 0.2f)
                {
                    newMode = 4; // Water
                }
                else
                {
                    newMode = 0; // Brimstone, default
                }
                if (newMode != currentMode)
                {
                    SoundEngine.PlaySound(SoundID.Item29, NPC.Center);
                }
                currentMode = newMode;
            }

            if (NPC.ai[0] == -1f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int phase;
                    int random = phase2 ? 6 : 5;
                    do phase = Main.rand.Next(random);
                    while (phase == NPC.ai[1] || (phase == 0 && phase3 && revenge) || phase == 1 || phase == 2 || (phase == 4 && NPC.localAI[3] != 0f));

                    NPC.ai[0] = phase;
                    NPC.ai[1] = 0f;

                    // Cocoon phase cooldown
                    if (NPC.localAI[3] > 0f)
                        NPC.localAI[3] -= 1f;
                    else if (phase == 4)
                    {
                        NPC.localAI[3] = 3f;
                        SoundEngine.PlaySound(HideInShellSound, player.Center);
                    }

                    // Prevent netUpdate from being blocked by the spam counter.
                    // A phase switch sync is a critical operation that must be synced.
                    NPC.ForceNetUpdate(false);
                }
            }

            // Pick a location to teleport to
            else if (NPC.ai[0] == 0f)
            {
                NPC.chaseable = true;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[1] += 1f;

                    if (NPC.localAI[1] >= (death ? 120f : 180f))
                    {
                        NPC.TargetClosest();
                        NPC.localAI[1] = 0f;
                        int timer = 0;
                        int playerPosX;
                        int playerPosY;
                        while (true)
                        {
                            timer++;
                            playerPosX = (int)player.Center.X / 16;
                            playerPosY = (int)player.Center.Y / 16;

                            int min = 12;
                            int max = 16;

                            if (Main.rand.NextBool())
                                playerPosX += Main.rand.Next(min, max);
                            else
                                playerPosX -= Main.rand.Next(min, max);

                            if (Main.rand.NextBool())
                                playerPosY += Main.rand.Next(min, max);
                            else
                                playerPosY -= Main.rand.Next(min, max);

                            if (!WorldGen.SolidTile(playerPosX, playerPosY))
                                break;

                            if (timer > 100)
                                return;
                        }
                        NPC.ai[0] = 1f;
                        NPC.ai[1] = playerPosX;
                        NPC.ai[2] = playerPosY;
                        NPC.netUpdate = true;
                    }
                }
            }

            // Teleport to location
            else if (NPC.ai[0] == 1f)
            {
                // Disable contact damage for some time while fading away
                NPC.damage = 0;

                NPC.chaseable = true;
                Vector2 position = new Vector2(NPC.ai[1] * 16f - (NPC.width / 2), NPC.ai[2] * 16f - (NPC.height / 2));
                for (int m = 0; m < 5; m++)
                {
                    int dust = Dust.NewDust(position, NPC.width, NPC.height, DustID.LifeDrain, 0f, -1f, 90, default, 2f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].fadeIn = 1f;
                }
                NPC.alpha += death ? 5 : revenge ? 4 : expertMode ? 3 : 2;
                if (NPC.alpha >= 255)
                {
                    int spawnType = currentMode == 3 ? NPCID.AngryNimbus : ModContent.NPCType<Brimling>();
                    int enemyCount = currentMode == 3 ? 3 : 1; // 3 angry nimbi if cloud, otherwise 1 brimling
                    if (Main.netMode != NetmodeID.MultiplayerClient && NPC.CountNPCS(spawnType) < (death ? 1 : 2) && revenge && currentMode != 2) // dont spawn anything if gfb rare sand
                    {
                        for (int i = 0; i < enemyCount; i++)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, spawnType);
                    }
                    SoundEngine.PlaySound(TeleportSound, NPC.Center);
                    NPC.alpha = 255;
                    NPC.position = position;
                    for (int n = 0; n < 15; n++)
                    {
                        int warpDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, 0f, -1f, 90, default, 3f);
                        Main.dust[warpDust].noGravity = true;
                    }
                    NPC.ai[0] = 2f;
                    NPC.netUpdate = true;
                }
            }

            // Either teleport again or go to next AI state
            else if (NPC.ai[0] == 2f)
            {
                if (NPC.alpha >= 255)
                {
                    if (Main.zenithWorld)
                    {
                        SoundEngine.PlaySound(SoundID.Item68, NPC.Center);
                        int type = ModContent.ProjectileType<BrimstoneRay>();
                        int damage = RayDamage;
                        Vector2 pos = NPC.Center;
                        for (int i = 0; i < 4; i++)
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.UnitY.RotatedBy(MathHelper.Lerp(0, MathHelper.TwoPi, i / 4f)), type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }
                        }
                        if (currentMode >= 1 && currentMode <= 3)
                        {
                            int tornadoType = currentMode == 3 ? ModContent.ProjectileType<StormMarkHostile>() : ProjectileID.SandnadoHostileMark;
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, Vector2.Zero, tornadoType, damage, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        if (currentMode == 2)
                        {
                            int healAmt = NPC.lifeMax / 25;
                            if (healAmt > 0)
                            {
                                NPC.life += healAmt;
                                NPC.HealEffect(healAmt, true);
                                NPC.netUpdate = true;
                            }
                        }
                    }
                }

                NPC.alpha -= 50;
                if (NPC.alpha <= 0)
                {
                    // Restore contact damage once returned to proper opacity
                    NPC.damage = NPC.defDamage;
                    NPC.chaseable = true;
                    NPC.ai[3] += 1f;
                    NPC.alpha = 0;
                    if (NPC.ai[3] >= 2f || phase2 || Main.getGoodWorld)
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                    }
                    else
                    {
                        NPC.ai[0] = 0f;
                    }
                    NPC.netUpdate = true;
                }
            }

            // Float above target and fire projectiles
            else if (NPC.ai[0] == 3f)
            {
                NPC.chaseable = true;
                NPC.rotation = NPC.velocity.X * 0.04f;

                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                NPC.ai[1] += 1f;
                float divisor = expertMode ? (death ? 80f : revenge ? 45f : 50f) - (float)Math.Ceiling(10f * (1f - lifeRatio)) : 50f;
                divisor -= 3f * enrageScale;
                float divisor2 = divisor * 2f;

                if (NPC.ai[1] % divisor == divisor - 1f)
                {
                    float velocity = (death ? 7f : revenge ? 6f : 5f) + (2f * enrageScale) + (expertMode ? 3f * (1f - lifeRatio) : 0f);
                    int type = ModContent.ProjectileType<BrimstoneHellfireball>();
                    int damage = HellfireballDamage;
                    if (currentMode == 4)
                    {
                        type = ModContent.ProjectileType<FrostMist>();
                        SoundEngine.PlaySound(SoundID.Item30, player.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(HellfireballSound, player.Center);
                    }
                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projectileVelocity.SafeNormalize(Vector2.UnitY) * 5f, projectileVelocity, type, damage, 0f, Main.myPlayer, player.position.X, player.position.Y);
                        Main.projectile[proj].timeLeft = 240;
                    }

                    if (NPC.ai[1] % divisor2 == divisor2 - 1f)
                    {
                        velocity = (death ? 5f : 4f) + 2f * enrageScale;
                        type = ModContent.ProjectileType<BrimstoneBarrage>();
                        damage = DartDamage;
                        if (currentMode == 4)
                        {
                            type = ModContent.ProjectileType<WaterSpear>();
                            SoundEngine.PlaySound(SoundID.Item21, player.Center);
                        }
                        else
                        {
                            SoundEngine.PlaySound(DartSound, player.Center);
                        }
                        projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;
                        int numProj = death ? 8 : 4;
                        int spread = death ? 90 : 45;
                        if (Main.getGoodWorld)
                        {
                            numProj *= 3;
                            spread *= 2;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float rotation = MathHelper.ToRadians(spread);
                            float projectileVelocityToPass = velocity * 3f;
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + perturbedSpeed.SafeNormalize(Vector2.UnitY) * 5f, perturbedSpeed, type, damage, 0f, Main.myPlayer, 1f, 0f, projectileVelocityToPass);
                            }
                        }
                    }
                }

                if (NPC.ai[1] >= divisor * (death ? 5f : 10f))
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 3f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Cocoon bullet hell
            else if (NPC.ai[0] == 4f)
            {
                NPC.defense = NPC.defDefense * 4;

                NPC.chaseable = false;
                NPC.localAI[0] += 1f;
                if (Main.getGoodWorld)
                    NPC.localAI[0] += 2f;
                if (expertMode)
                    NPC.localAI[0] += 1f - lifeRatio;
                NPC.localAI[0] += enrageScale;

                if (NPC.localAI[0] >= 120f)
                {
                    NPC.localAI[0] = 0f;

                    float projectileSpeed = death ? 9f : revenge ? 8f : 6f;
                    projectileSpeed += 2f * enrageScale;

                    Vector2 projectileVelocity = player.Center - NPC.Center;

                    float radialOffset = 0.2f;
                    float diameter = 80f;

                    projectileVelocity = projectileVelocity.SafeNormalize(Vector2.UnitY) * projectileSpeed;

                    Vector2 velocity = projectileVelocity;
                    velocity = velocity.SafeNormalize(Vector2.UnitY);
                    velocity *= diameter;

                    int totalProjectiles = 6;
                    float offsetAngle = MathHelper.Pi * radialOffset;
                    int type = ModContent.ProjectileType<BrimstoneHellblast>();
                    int damage = HellblastDamage;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int j = 0; j < totalProjectiles; j++)
                        {
                            float radians = j - (totalProjectiles - 1f) / 2f;
                            Vector2 offset = velocity.RotatedBy(offsetAngle * radians);
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + offset, projectileVelocity, type, damage, 0f, Main.myPlayer, 1f, 0f);
                            Main.projectile[proj].timeLeft = 300;
                            Main.projectile[proj].tileCollide = false;
                        }
                    }

                    totalProjectiles = 12;
                    float radians2 = MathHelper.TwoPi / totalProjectiles;
                    type = ModContent.ProjectileType<BrimstoneBarrage>();
                    damage = DartDamage;
                    if (currentMode == 4)
                    {
                        type = ModContent.ProjectileType<SirenSong>();
                        SoundEngine.PlaySound(SoundID.Item26, player.Center);
                    }
                    else
                    {
                        SoundEngine.PlaySound(ShellFireSound, player.Center);
                    }
                    double angleA = radians2 * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(projectileSpeed * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = NPC.localAI[2] % 2f == 0f ? new Vector2(0f, -projectileSpeed) : new Vector2(-velocityX, -projectileSpeed);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float projectileVelocityToPass = projectileSpeed * 3f;
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 vector255 = spinningPoint.RotatedBy(radians2 * k);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vector255.SafeNormalize(Vector2.UnitY) * 5f, vector255, type, damage, 0f, Main.myPlayer, 1f, 0f, projectileVelocityToPass);
                        }

                        if (death)
                        {
                            spinningPoint = NPC.localAI[2] % 2f == 0f ? new Vector2(-velocityX, -projectileSpeed) : new Vector2(0f, -projectileSpeed);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians2 * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vector255.SafeNormalize(Vector2.UnitY) * 5f, vector255 * 0.75f, type, damage, 0f, Main.myPlayer, 1f, 0f, projectileVelocityToPass);
                            }
                        }
                    }

                    NPC.localAI[2] += 1f;
                }

                NPC.velocity *= 0.95f;
                NPC.rotation = NPC.velocity.X * 0.04f;
                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (death ? 240f : 300f))
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = -1f;
                    NPC.ai[1] = 4f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.netUpdate = true;
                }
            }

            // Laser beam attack
            else if (NPC.ai[0] == 5f)
            {
                NPC.chaseable = true;

                NPC.defense = NPC.defDefense * 2;

                Vector2 source = new Vector2(NPC.Center.X + (NPC.spriteDirection > 0 ? 34f : -34f), NPC.Center.Y - 74f);
                Vector2 aimAt = player.Center + player.velocity * 20f;
                float aimResponsiveness = (NPC.ai[2] == 1f || death) ? 0.1f : 0.25f;

                Vector2 aimVector = (aimAt - source).SafeNormalize(Vector2.UnitY);
                if (aimVector.HasNaNs())
                    aimVector = -Vector2.UnitY;
                aimVector = Vector2.Lerp(aimVector, NPC.velocity.SafeNormalize(Vector2.UnitY), aimResponsiveness).SafeNormalize(Vector2.UnitY);
                aimVector *= 6f;

                Vector2 laserVelocity = aimVector.SafeNormalize(Vector2.UnitY);
                if (laserVelocity.HasNaNs())
                    laserVelocity = -Vector2.UnitY;

                calamityGlobalNPC.newAI[1] = laserVelocity.X;
                calamityGlobalNPC.newAI[2] = laserVelocity.Y;

                // Rev = 190 + 165 = 355
                // Death = 165

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= 240f)
                {
                    NPC.TargetClosest();
                    NPC.ai[2] += 1f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[1] = 0f;
                    if (NPC.ai[2] >= (death ? 1f : 2f))
                    {
                        NPC.ai[0] = -1f;
                        NPC.ai[1] = 5f;
                        NPC.ai[2] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                    else
                    {
                        NPC.ai[1] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }
                }
                else if (NPC.ai[1] >= 180f)
                {
                    NPC.velocity *= 0.95f;
                    if (NPC.ai[1] == 180f)
                    {
                        SoundEngine.PlaySound(SoundID.Item68, source);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 laserVelocity2 = new Vector2(NPC.localAI[0], NPC.localAI[1]);
                            laserVelocity2 = laserVelocity2.SafeNormalize(Vector2.UnitY);
                            int type = ModContent.ProjectileType<BrimstoneRay>();
                            int damage = RayDamage;

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), source, laserVelocity2, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            if (Main.getGoodWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, -laserVelocity2, type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                            if (Main.zenithWorld)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, new Vector2(-laserVelocity2.X, laserVelocity2.Y), type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, new Vector2(laserVelocity2.X, -laserVelocity2.Y), type, damage, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }
                        }
                    }
                }
                else
                {
                    float playSoundTimer = 30f;
                    if (NPC.ai[1] < 150f)
                    {
                        switch ((int)NPC.ai[2])
                        {
                            case 0:
                                NPC.ai[1] += 0.5f;
                                break;
                            case 1:
                                NPC.ai[1] += 1f;
                                playSoundTimer = 40f;
                                break;
                        }
                        if (death)
                        {
                            NPC.ai[1] += 0.5f;
                            playSoundTimer += 10f;
                        }
                    }

                    if (NPC.ai[1] % playSoundTimer == 0f)
                        SoundEngine.PlaySound(SoundID.Item20, NPC.Center);

                    if (NPC.ai[1] < 150f && calamityGlobalNPC.newAI[0] == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), source, laserVelocity, ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            if (Main.getGoodWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, -laserVelocity, ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);

                            if (Main.zenithWorld)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, new Vector2(-laserVelocity.X, laserVelocity.Y), ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source, new Vector2(laserVelocity.X, -laserVelocity.Y), ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 0f, NPC.whoAmI);
                            }
                        }

                        calamityGlobalNPC.newAI[0] = 1f;
                    }
                    else
                    {
                        if (NPC.ai[1] == 150f)
                        {
                            NPC.localAI[0] = laserVelocity.X;
                            NPC.localAI[1] = laserVelocity.Y;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), source.X, source.Y, NPC.localAI[0], NPC.localAI[1], ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, NPC.whoAmI);
                                if (Main.getGoodWorld)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), source.X, source.Y, -NPC.localAI[0], -NPC.localAI[1], ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, NPC.whoAmI);

                                if (Main.zenithWorld)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), source.X, source.Y, -NPC.localAI[0], NPC.localAI[1], ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, NPC.whoAmI);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), source.X, source.Y, NPC.localAI[0], -NPC.localAI[1], ModContent.ProjectileType<BrimstoneTargetRay>(), 0, 0f, Main.myPlayer, 1f, NPC.whoAmI);
                                }
                            }
                        }
                    }
                }
            }
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;
            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 240);
        }

        public override void FindFrame(int frameHeight) // 9 total frames
        {
            NPC.frameCounter += 1.0;
            if (NPC.ai[0] <= 2f)
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y >= frameHeight * 4)
                {
                    NPC.frame.Y = 0;
                }
            }
            else if (NPC.ai[0] == 3f || NPC.ai[0] == 5f)
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 4)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
                if (NPC.frame.Y >= frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight * 4;
                }
            }
            else
            {
                if (NPC.frameCounter > 12.0)
                {
                    NPC.frame.Y = NPC.frame.Y + frameHeight;
                    NPC.frameCounter = 0.0;
                }
                if (NPC.frame.Y < frameHeight * 8)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
                if (NPC.frame.Y >= frameHeight * 12)
                {
                    NPC.frame.Y = frameHeight * 8;
                }
            }
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<BrimstoneElementalBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Brimlance>(),
                    ModContent.ItemType<SeethingDischarge>(),
                    ModContent.ItemType<DormantBrimseeker>(),
                    ModContent.ItemType<Hellborn>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(ModContent.ItemType<EssenceofHavoc>(), 1, 8, 10);

                // Equipment
                int[] accs = new int[]
                {
                    ModContent.ItemType<RoseStone>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, accs));
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<FlameLickedShell>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<BrimstoneElementalMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

            }

            // Trophy
            npcLoot.Add(ModContent.ItemType<BrimstoneElementalTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<BrimstoneElementalRelic>());

            // GFB Heart of the Elements drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<HeartoftheElements>()), hideLootReport: true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedBrimstoneElemental, ModContent.ItemType<LoreAzafure>(), desc: DropHelper.FirstKillText);
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedBrimstoneElemental, ModContent.ItemType<LoreBrimstoneElemental>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            // Mark brimmy as dead
            DownedBossSystem.downedBrimstoneElemental = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.LifeDrain, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 200;
                NPC.height = 150;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int brimDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[brimDust].scale = 0.5f;
                        Main.dust[brimDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 60; j++)
                {
                    int brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[brimDust2].noGravity = true;
                    Main.dust[brimDust2].velocity *= 5f;
                    brimDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[brimDust2].velocity *= 2f;
                }
                if (!Main.dedServ)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("BrimstoneGore4").Type, 1f);
                }
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
                return true;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;
            Texture2D npcTexture = TextureAssets.Npc[Type].Value;
            Vector2 frameLocation = new Vector2((float)(npcTexture.Width / 2), (float)(npcTexture.Height / Main.npcFrameCount[Type] / 2));
            Vector2 npcOffset = NPC.Center - screenPos;
            npcOffset -= new Vector2((float)npcTexture.Width, (float)(npcTexture.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            npcOffset += frameLocation * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            // Give brimmy an outline based on current elemental mode
            if (Main.zenithWorld)
            {
                Color baseColor = Color.Red;
                switch (currentMode)
                {
                    case 0:
                        baseColor = Color.Red;
                        break;
                    case 1:
                        baseColor = Color.Tan;
                        break;
                    case 2:
                        baseColor = Color.Lime;
                        break;
                    case 3:
                        baseColor = Color.Gray;
                        break;
                    case 4:
                        baseColor = Color.Blue;
                        break;
                }
                CalamityUtils.EnterShaderRegion(spriteBatch);
                Color outlineColor = Color.Lerp(baseColor, Color.White, 0.4f);
                outlineColor *= NPC.Opacity;
                Vector3 outlineHSL = Main.rgbToHsl(outlineColor);
                float outlineThickness = MathHelper.Clamp(2f, 0f, 3f);

                GameShaders.Misc["CalamityMod:BasicTint"].UseOpacity(1f);
                GameShaders.Misc["CalamityMod:BasicTint"].UseColor(Main.hslToRgb(1 - outlineHSL.X, outlineHSL.Y, outlineHSL.Z));
                GameShaders.Misc["CalamityMod:BasicTint"].Apply();

                for (float i = 0; i < 1; i += 0.125f)
                {
                    spriteBatch.Draw(npcTexture, npcOffset + (i * MathHelper.TwoPi).ToRotationVector2() * outlineThickness, NPC.frame, outlineColor, NPC.rotation, frameLocation, NPC.scale, spriteEffects, 0f);
                }
                CalamityUtils.ExitShaderRegion(spriteBatch);
            }
            spriteBatch.Draw(npcTexture, npcOffset, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, frameLocation, NPC.scale, spriteEffects, 0f);

            return false;
        }
    }
}
