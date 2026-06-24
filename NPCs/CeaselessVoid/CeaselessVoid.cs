using System.IO;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace CalamityMod.NPCs.CeaselessVoid
{
    [AutoloadBossHead]
    public class CeaselessVoid : ModNPC
    {
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/CeaselessVoidDeath");
        public static readonly SoundStyle BuildupSound = new("CalamityMod/Sounds/Custom/CeaselessVoidDeathBuild");

        public static Asset<Texture2D> GlowTexture;

        public bool playedbuildsound = false;

        public bool madeItToLocation = true;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.55f,
            };
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                GlowTexture = Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int BeamPortalDamage = 60; // 240
        public static int DarkEnergyProjectileDamage = 60; // 240

        public override void SetDefaults()
        {
            NPC.damage = 180; // 360
            NPC.npcSlots = 36f;
            NPC.width = 100;
            NPC.height = 100;
            NPC.defense = 80;
            NPC.Calamity().DR = 0.5f;
            NPC.LifeMaxNERB(50000, 78000, 72000);
            NPC.value = Item.buyPrice(gold: 50);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.boss = true;
            NPC.BossBar = GetInstance<CeaselessVoidBossBar>();
            NPC.DeathSound = DeathSound;
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.Calamity().VulnerableToSickness = false;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheDungeon,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.CeaselessVoid")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            writer.Write(playedbuildsound);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            playedbuildsound = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            CalamityGlobalNPC.voidBoss = NPC.whoAmI;

            // Percent life remaining
            double lifeRatio = NPC.life / (double)NPC.lifeMax;

            // Difficulty modes
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Phases
            bool phase2 = lifeRatio <= 0.7;
            bool phase3 = lifeRatio <= 0.4;
            bool phase4 = lifeRatio <= 0.1;
            bool theBigSucc = NPC.life / (double)NPC.lifeMax <= 0.1;
            bool succSoHardThatYouDie = NPC.life / (double)NPC.lifeMax <= 0.005;

            // Spawn Dark Energies
            int darkEnergyAmt = death ? 6 : revenge ? 5 : expertMode ? 4 : 3;
            if (phase2)
                darkEnergyAmt += 1;
            if (phase3)
                darkEnergyAmt += 1;
            if (phase4)
                darkEnergyAmt += 1;

            if (Main.getGoodWorld)
                darkEnergyAmt *= 2;

            // Spawn a few Dark Energies as soon as the fight starts
            int spacing = 360 / darkEnergyAmt;
            int distance2 = 10;
            if (NPC.ai[2] == 0f)
            {
                NPC.ai[2] = 1f;
                for (int i = 0; i < darkEnergyAmt; i++)
                {
                    for (int j = 0; j < 3; j++)
                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), NPCType<DarkEnergy>(), NPC.whoAmI, i * spacing, j);
                }
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 0.5f);
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 1.5f);
            }

            // If there are any Dark Energies alive, change AI and don't take damage
            bool anyDarkEnergies = NPC.AnyNPCs(NPCType<DarkEnergy>());
            bool movingDuringSuccPhase = NPC.ai[3] == 0f;
            NPC.dontTakeDamage = anyDarkEnergies || theBigSucc || movingDuringSuccPhase;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Despawn
            if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f || (player.position.Y < Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive))
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f || (player.position.Y < Main.worldSurface * 16.0 && !BossRushEvent.BossRushActive))
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // Increase projectile fire rate based on number of nearby active tiles
            float projectileFireRateMultiplier = Main.getGoodWorld ? 0.5f : 1.5f;

            // Decides whether Ceaseless moves closer to its target or not
            float distanceRequiredToMove = Main.getGoodWorld ? 300f : 720f;
            bool move = Vector2.Distance(NPC.Center, player.Center) > distanceRequiredToMove || !Collision.CanHit(NPC.Center, 1, 1, player.Center, 1, 1);

            // Succ attack
            if (!anyDarkEnergies)
            {
                // This is here because it's used in multiple places
                float suckDistance = death ? 1600f : revenge ? 1440f : expertMode ? 1280f : 1040f;

                // Move closer to the target before trying to succ
                if (movingDuringSuccPhase)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (move)
                        Movement(true);
                    else
                        NPC.ai[3] = 1f;
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    // Use this to generate more and more dust in final phase
                    float finalPhaseDustRatio = 1f;
                    if (succSoHardThatYouDie)
                    {
                        finalPhaseDustRatio = 5f;
                    }
                    else if (theBigSucc)
                    {
                        float amount = (10f - (float)(NPC.life / (double)NPC.lifeMax) * 100f) / 10f;
                        finalPhaseDustRatio += MathHelper.Lerp(0f, 2f, amount);
                    }

                    // Slow down
                    if (NPC.velocity.Length() > 0.5f)
                        NPC.velocity *= 0.8f;
                    else
                        NPC.velocity = Vector2.Zero;

                    // Move towards target again if they get outside the succ radius
                    float moveCloserGateValue = suckDistance * 0.8f;
                    if (Vector2.Distance(NPC.Center, player.Center) > moveCloserGateValue)
                        NPC.ai[3] = 0f;

                    // Ceaseless Void sucks in dark energy in different patterns
                    // This attack also sucks in all players that are within reach of the succ
                    for (int h = 0; h < 3; h++)
                    {
                        float distanceDivisor = h + 1f;
                        float dustDistance = suckDistance / distanceDivisor;
                        int numDust = (int)(0.1f * MathHelper.TwoPi * dustDistance);
                        Vector2 dustOffset = Vector2.UnitX.RotatedByRandom(MathHelper.Pi) * dustDistance;

                        int var = (int)(dustDistance / finalPhaseDustRatio);
                        float dustVelocity = 24f / distanceDivisor * finalPhaseDustRatio;
                        for (int i = 0; i < numDust; i++)
                        {
                            if (Main.rand.NextBool(var))
                            {
                                dustOffset = dustOffset.RotatedBy(MathHelper.TwoPi / numDust);
                                Vector2 dustSpawn = NPC.Center + dustOffset;
                                Dust dust = Dust.NewDustPerfect(dustSpawn, DustType<CeaselessDust>(), Utils.DirectionTo(dustSpawn, NPC.Center) * dustVelocity, Scale: 3 - h);
                                dust.fadeIn = 1f;
                            }
                        }
                    }

                    float succPower = 0.125f + finalPhaseDustRatio * 0.125f;
                    for (int i = 0; i < Main.maxPlayers; i++)
                    {
                        float distance = Vector2.Distance(Main.player[i].Center, NPC.Center);
                        if (distance < suckDistance && Main.player[i].grappling[0] == -1)
                        {
                            if (Collision.CanHit(NPC.Center, 1, 1, Main.player[i].Center, 1, 1))
                            {
                                float distanceRatio = distance / suckDistance;
                                float multiplier = 1f - distanceRatio;

                                if (Main.player[i].Center.X < NPC.Center.X)
                                    Main.player[i].velocity.X += succPower * multiplier;
                                else
                                    Main.player[i].velocity.X -= succPower * multiplier;
                            }
                        }
                    }

                    // Slowly die in final phase and then implode
                    // This phase lasts 20 seconds, 60 seconds in GFB
                    if (theBigSucc && calamityGlobalNPC.newAI[1] % 60f == 0f)
                    {
                        int damageIncrement = NPC.lifeMax / (Main.zenithWorld ? 600 : 200);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.life -= damageIncrement;
                            NPC.DamageEffect(damageIncrement);
                        }

                        if (NPC.life <= (damageIncrement * 5) && !playedbuildsound)
                        {
                            SoundEngine.PlaySound(BuildupSound, NPC.Center);
                            playedbuildsound = true;
                        }

                        if (NPC.life <= 0)
                        {
                            NPC.life = 0;
                            NPC.HitEffect();
                            NPC.checkDead();
                        }

                        NPC.netUpdate = true;
                    }

                    // Beam Portals
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (calamityGlobalNPC.newAI[1] == 0f)
                        {
                            int numBeamPortals = revenge ? 3 : 2;
                            float degrees = 360 / numBeamPortals;
                            float beamPortalDistance = death ? 400f : revenge ? 420f : expertMode ? 440f : 480f;
                            int type = ProjectileType<DoGBeamPortal>();
                            for (int i = 0; i < numBeamPortals; i++)
                            {
                                float ai1 = i * degrees;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.Center.X + (float)(Math.Sin(i * degrees) * beamPortalDistance), player.Center.Y + (float)(Math.Cos(i * degrees) * beamPortalDistance), 0f, 0f, type, BeamPortalDamage, 0f, Main.myPlayer, ai1, 0f);
                            }
                        }
                    }

                    // Use this timer to lessen Dark Energy projectile rate of fire while Beam Portals are active
                    float beamPortalTimeLeft = 600f;
                    bool summonLessDarkEnergies = false;
                    if (calamityGlobalNPC.newAI[1] < beamPortalTimeLeft)
                    {
                        calamityGlobalNPC.newAI[1] += 1f;
                        summonLessDarkEnergies = true;
                    }
                    else if (theBigSucc)
                        calamityGlobalNPC.newAI[1] += 1f;

                    // Suck in Dark Energy projectiles from far away
                    calamityGlobalNPC.newAI[3] += 1f;
                    float darkEnergySpiralGateValue = (summonLessDarkEnergies ? 24f : 12f) * projectileFireRateMultiplier;
                    if (calamityGlobalNPC.newAI[3] >= darkEnergySpiralGateValue)
                    {
                        calamityGlobalNPC.newAI[3] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ProjectileType<DarkEnergyBall>();
                            bool normalSpread = NPC.localAI[0] % 2f == 0f;
                            float speed = 0.5f;
                            int totalProjectiles = 4;
                            Vector2 spinningPoint = new Vector2(normalSpread ? 0f : -speed, -speed);
                            float radialOffset = MathHelper.ToRadians(NPC.localAI[1]);
                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 spawnVector = NPC.Center + Vector2.Normalize(spinningPoint.RotatedBy(MathHelper.TwoPi / totalProjectiles * i + radialOffset)) * suckDistance;
                                Vector2 velocity = Vector2.Normalize(NPC.Center - spawnVector) * speed;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, DarkEnergyProjectileDamage, 0f, Main.myPlayer);
                            }
                        }

                        NPC.localAI[1] += 10f;
                    }

                    // Summon some extra projectiles in Expert Mode
                    if (phase2 && expertMode)
                    {
                        NPC.localAI[2] += 1f;
                        if (NPC.localAI[2] >= 60f * projectileFireRateMultiplier)
                        {
                            NPC.localAI[2] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ProjectileType<DarkEnergyBall2>();
                                bool normalSpread = NPC.localAI[0] % 2f != 0f;
                                float speed = 2f;
                                int totalProjectiles = 2;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = new Vector2(normalSpread ? 0f : -velocityX, -speed);
                                float radialOffset = MathHelper.ToRadians(NPC.localAI[1] * 0.25f);
                                for (int i = 0; i < totalProjectiles; i++)
                                {
                                    Vector2 spawnVector = NPC.Center + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
                                    Vector2 velocity = Vector2.Normalize(NPC.Center - spawnVector) * speed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, DarkEnergyProjectileDamage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }

                    // Summon some extra projectiles in Revengeance Mode
                    if (phase4 && revenge)
                    {
                        NPC.localAI[3] += 1f;
                        if (NPC.localAI[3] >= 90f * projectileFireRateMultiplier)
                        {
                            NPC.localAI[3] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ProjectileType<DarkEnergyBall2>();
                                bool normalSpread = NPC.localAI[0] % 2f == 0f;
                                float speed = 4f;
                                int totalProjectiles = 2;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                double angleA = radians * 0.5;
                                double angleB = MathHelper.ToRadians(90f) - angleA;
                                float velocityX = (float)(speed * Math.Sin(angleA) / Math.Sin(angleB));
                                Vector2 spinningPoint = new Vector2(normalSpread ? 0f : -velocityX, -speed);
                                float radialOffset = MathHelper.ToRadians(NPC.localAI[1] * 0.25f);
                                for (int i = 0; i < totalProjectiles; i++)
                                {
                                    Vector2 spawnVector = NPC.Center + Vector2.Normalize(spinningPoint.RotatedBy(radians * i + radialOffset)) * suckDistance;
                                    Vector2 velocity = Vector2.Normalize(NPC.Center - spawnVector) * speed;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, velocity, type, DarkEnergyProjectileDamage, 0f, Main.myPlayer);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                // Avoid cheap bullshit
                NPC.damage = 0;
                if (move)
                {
                    madeItToLocation = false;
                }
                if (!madeItToLocation)
                {
                    Movement(false);
                }
                else
                {
                    // Slow down
                    if (NPC.velocity.Length() > 0.5f)
                        NPC.velocity *= 0.8f;
                    else
                        NPC.velocity = Vector2.Zero;
                }

                // Count up all Dark Energy HP values
                int totalDarkEnergyHP = 0;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC darkEnergy = Main.npc[i];
                    if (darkEnergy.active && darkEnergy.type == NPCType<DarkEnergy>())
                        totalDarkEnergyHP += darkEnergy.life;
                }

                // Destroy all Dark Energies if their total HP is below 20%
                int darkEnergyMaxHP = BossRushEvent.BossRushActive ? DarkEnergy.MaxBossRushHP : DarkEnergy.MaxHP;
                //These are still needed so that CV Dark energy despawn works properly
                double HPBoost = CalamityServerConfig.Instance.BossHealthBoost * 0.01;
                darkEnergyMaxHP += (int)(darkEnergyMaxHP * HPBoost);

                int totalDarkEnergiesSpawned = darkEnergyAmt * 3 + 2;
                int totalDarkEnergyMaxHP = darkEnergyMaxHP * totalDarkEnergiesSpawned;
                int succPhaseGateValue = (int)(totalDarkEnergyMaxHP * 0.2);
                if (totalDarkEnergyHP < succPhaseGateValue)
                {
                    SoundEngine.PlaySound(SoundID.NPCDeath44, NPC.Center);

                    // Kill all Dark Energies
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC darkEnergy = Main.npc[i];
                        if (darkEnergy.active && darkEnergy.type == NPCType<DarkEnergy>())
                        {
                            darkEnergy.HitEffect();
                            darkEnergy.active = false;
                            darkEnergy.netUpdate = true;
                        }
                    }

                    // Generate a dust explosion
                    int dustAmt = 30;
                    int random = 3;
                    for (int j = 0; j < 10; j++)
                    {
                        random += j * 2;
                        for (int d = 0; d < dustAmt; d++)
                        {
                            Vector2 dustVelocity = new Vector2(Main.rand.Next(-random, random), Main.rand.Next(-random, random));
                            dustVelocity = Vector2.Normalize(dustVelocity) * random * 2f;
                            Dust realDust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2CircularEdge(10f, 10f), (int)CalamityDusts.PurpleCosmilite, dustVelocity, 100, default, 5f);
                            realDust.noGravity = true;
                        }
                    }
                }
            }

            // Basic movement towards a location
            void Movement(bool succ)
            {
                float velocity = ((expertMode ? 7.5f : 6f) + (float)(death ? 2f * (1D - lifeRatio) : 0f));
                float acceleration = (death ? 0.2f : expertMode ? 0.16f : 0.12f) + (float)(death ? 0.04f * (1D - lifeRatio) : 0f);

                // Increase speed dramatically in succ phase
                if (succ)
                {
                    velocity *= 2f;
                    acceleration *= 2f;
                }

                if (!madeItToLocation)
                {

                    velocity *= 2f;
                    acceleration *= 5f;
                }

                if (Main.getGoodWorld)
                {
                    velocity *= 1.15f;
                    acceleration *= 1.15f;
                }

                Vector2 destination = player.Center;

                // Move between 8 different positions around the player, in order
                float maxDistance = 320f;
                Vector2 moveToOffset = succ ? Vector2.Zero : Main.getGoodWorld ? new Vector2(0f, -maxDistance) : Vector2.Zero;
                if ((!succ && Main.getGoodWorld) || !madeItToLocation)
                {
                    // Move to a new location every few seconds
                    calamityGlobalNPC.newAI[2] += 1f;
                    float newPositionGateValue = death ? 180f : revenge ? 210f : expertMode ? 240f : 300f;
                    if (calamityGlobalNPC.newAI[2] > newPositionGateValue)
                    {
                        calamityGlobalNPC.newAI[2] = 0f;

                        NPC.ai[0] += 1f;
                        if (NPC.ai[0] > 7f)
                            NPC.ai[0] = 0f;
                    }
                    moveToOffset += new Vector2(maxDistance, 0).RotatedBy(NPC.ai[0] / 8f * MathHelper.TwoPi);
                }

                destination += moveToOffset;

                // How far Ceaseless Void is from where it's supposed to be
                Vector2 distanceFromDestination = destination - NPC.Center;

                // Movement
                if (NPC.Distance(destination) > maxDistance || succ || (!Main.getGoodWorld && !madeItToLocation))
                    CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                if (NPC.Distance(destination) < 80)
                {
                    madeItToLocation = true;
                }
            }

            // Spawn more Dark Energies as the fight progresses
            if (calamityGlobalNPC.newAI[0] == 0f && NPC.life > 0)
                calamityGlobalNPC.newAI[0] = 1f;

            if (NPC.life > 0)
            {
                int healthGateValue = (int)(NPC.lifeMax * 0.3);
                if (((NPC.life + healthGateValue) / (float)NPC.lifeMax) < calamityGlobalNPC.newAI[0])
                {
                    NPC.TargetClosest();
                    calamityGlobalNPC.newAI[0] -= 0.3f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] += 1f;
                    NPC.localAI[1] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.localAI[3] = 0f;

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (phase4)
                        {
                            madeItToLocation = false;
                            for (int i = 0; i < darkEnergyAmt; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), NPCType<DarkEnergy>(), NPC.whoAmI, i * spacing, j * 2f);
                            }
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 1f);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 3f);
                        }
                        else if (phase3)
                        {
                            madeItToLocation = false;
                            for (int i = 0; i < darkEnergyAmt; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), NPCType<DarkEnergy>(), NPC.whoAmI, i * spacing, j * 1.5f);
                            }
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 0.5f);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 2f);
                        }
                        else
                        {
                            madeItToLocation = false;
                            for (int i = 0; i < darkEnergyAmt; i++)
                            {
                                for (int j = 0; j < 3; j++)
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X + (Math.Sin(i * spacing) * distance2)), (int)(NPC.Center.Y + (Math.Cos(i * spacing) * distance2)), NPCType<DarkEnergy>(), NPC.whoAmI, i * spacing, j);
                            }
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 1.5f);
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y + distance2), NPCType<DarkEnergy>(), NPC.whoAmI, 0f, 2.5f);
                        }
                    }

                    // Despawn potentially hazardous projectiles when entering a new phase
                    for (int i = 0; i < Main.maxProjectiles; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active)
                        {
                            if (projectile.type == ProjectileType<DoGBeamPortal>() || projectile.type == ProjectileType<DoGBeam>() ||
                                projectile.type == ProjectileType<DarkEnergyBall>() || projectile.type == ProjectileType<DarkEnergyBall2>())
                            {
                                if (projectile.timeLeft > 30)
                                    projectile.timeLeft = 30;
                            }
                        }
                    }
                }
            }
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

            return minDist <= 50f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(BuffID.VortexDebuff, 60, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[Type].Value.Width / 2), (float)(TextureAssets.Npc[Type].Value.Height / Main.npcFrameCount[Type] / 2));
            int afterimageAmt = 7;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (float)(afterimageAmt - i) / 15f;
                    Vector2 afterimageDrawPos = NPC.oldPos[i] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    afterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    afterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimageDrawPos, NPC.frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = GlowTexture.Value;
            Color cyanLerp = Color.Lerp(Color.White, Color.Cyan, 0.5f);

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int j = 1; j < afterimageAmt; j++)
                {
                    Color extraAfterimageColor = cyanLerp;
                    extraAfterimageColor = Color.Lerp(extraAfterimageColor, Color.White, 0.5f);
                    extraAfterimageColor *= (float)(afterimageAmt - j) / 15f;
                    Vector2 extraAfterimageDrawPos = NPC.oldPos[j] + new Vector2((float)NPC.width, (float)NPC.height) / 2f - screenPos;
                    extraAfterimageDrawPos -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[Type])) * NPC.scale / 2f;
                    extraAfterimageDrawPos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, extraAfterimageDrawPos, NPC.frame, extraAfterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, cyanLerp, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            DownedBossSystem.downedCeaselessVoid = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ItemType<CeaselessVoidBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            LeadingConditionRule normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            npcLoot.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ItemType<MirrorBlade>(),
                    ItemType<VoidConcentrationStaff>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ItemType<DarkPlasma>(), 1, 10, 12));

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ItemType<TheEvolution>()));

                // Vanity
                normalOnly.Add(ItemType<CeaselessVoidMask>(), 7);
                var godSlayerVanity = ItemDropRule.Common(ItemType<AncientGodSlayerHelm>(), 20);
                godSlayerVanity.OnSuccess(ItemDropRule.Common(ItemType<AncientGodSlayerChestplate>()));
                godSlayerVanity.OnSuccess(ItemDropRule.Common(ItemType<AncientGodSlayerLeggings>()));
                normalOnly.Add(godSlayerVanity);
                normalOnly.Add(ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ItemType<CeaselessVoidTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ItemType<CeaselessVoidRelic>());

            // GFB Eclipse Mirror and Nucleogenesis drop
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(DropHelper.PerPlayer(ItemType<EclipseMirror>()), hideLootReport: true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemType<Nucleogenesis>()), hideLootReport: true);
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCeaselessVoid, ItemType<LoreCeaselessVoid>(), desc: DropHelper.FirstKillText);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemType<SupremeHealingPotion>();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0 && NPC.life >= NPC.lifeMax * 0.05f)
            {
                NPC.soundDelay = 8;
                float pitchVar = Main.zenithWorld ? 0.4f : 0;
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCHit/OtherworldlyHit") with { PitchVariance = pitchVar }, NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);
                Main.dust[dust].noGravity = true;
            }

            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int purpleDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[purpleDust].velocity *= 3f;
                    Main.dust[purpleDust].noGravity = true;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[purpleDust].scale = 0.5f;
                        Main.dust[purpleDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int purpleDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 3f);
                    Main.dust[purpleDust2].noGravity = true;
                    Main.dust[purpleDust2].velocity *= 5f;
                    purpleDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[purpleDust2].noGravity = true;
                    Main.dust[purpleDust2].velocity *= 2f;
                }

                if (!Main.dedServ)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CeaselessVoid").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CeaselessVoid2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CeaselessVoid2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CeaselessVoid3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CeaselessVoid3").Type, 1f);
                }
            }
        }
    }
}
