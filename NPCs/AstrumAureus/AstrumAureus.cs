using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Mounts;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.VanillaNPCAIOverrides.RegularEnemies;
using CalamityMod.Projectiles.Boss;
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

namespace CalamityMod.NPCs.AstrumAureus
{
    [AutoloadBossHead]
    [HasPierceResist(singleHitbox: true)]
    public class AstrumAureus : ModNPC
    {
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/AureusHit", 4);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/AureusDeath");
        public static readonly SoundStyle LaserSound = new("CalamityMod/Sounds/Custom/AstrumAureus/AureusShoot");
        public static readonly SoundStyle FlameCrystalSound = new("CalamityMod/Sounds/Custom/AstrumAureus/AureusShootCrystal");
        public static readonly SoundStyle StompSound = new("CalamityMod/Sounds/Custom/AstrumAureus/LegStomp");
        public static readonly SoundStyle JumpSound = new("CalamityMod/Sounds/Custom/AstrumAureus/AureusJump");
        public static readonly SoundStyle TeleportSound = new("CalamityMod/Sounds/Custom/AstrumAureus/AureusTeleport");

        public static Asset<Texture2D> JumpTexture;
        public static Asset<Texture2D> RechargeTexture;
        public static Asset<Texture2D> StompTexture;
        public static Asset<Texture2D> WalkTexture;
        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> JumpTexture_Glow;
        public static Asset<Texture2D> StompTexture_Glow;
        public static Asset<Texture2D> WalkTexture_Glow;

        private bool stomping = false;
        public int slimeProjCounter = 0;
        public int slimePhase = 0;

        public RevengeanceAndDeathAI.MimicAI ZenithSeedMimicAI;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.27f,
                PortraitScale = 0.45f,
                PortraitPositionYOverride = -24f
            };
            value.Position.Y -= 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Texture_Glow = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                JumpTexture = ModContent.Request<Texture2D>(Texture + "Jump", AssetRequestMode.AsyncLoad);
                RechargeTexture = ModContent.Request<Texture2D>(Texture + "Recharge", AssetRequestMode.AsyncLoad);
                StompTexture = ModContent.Request<Texture2D>(Texture + "Stomp", AssetRequestMode.AsyncLoad);
                WalkTexture = ModContent.Request<Texture2D>(Texture + "Walk", AssetRequestMode.AsyncLoad);
                JumpTexture_Glow = ModContent.Request<Texture2D>(Texture + "JumpGlow", AssetRequestMode.AsyncLoad);
                StompTexture_Glow = ModContent.Request<Texture2D>(Texture + "StompGlow", AssetRequestMode.AsyncLoad);
                WalkTexture_Glow = ModContent.Request<Texture2D>(Texture + "WalkGlow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int LaserDamage = 25; // 100
        public static int CrystalDamage = 30; // 120

        public override void SetDefaults()
        {
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.npcSlots = 15f;
            NPC.damage = 80; // 160
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.width = 374;
            NPC.height = 374;
            NPC.defense = 40;
            NPC.DR_NERD(0.1f);
            NPC.LifeMaxNERB(75000, 120000, 740000); // 30 seconds in boss rush
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 15);
            NPC.boss = true;
            NPC.DeathSound = DeathSound;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BiomeManagers.AstralInfectionBiome>().Type };

            if (Main.getGoodWorld)
                NPC.scale = 0.7f;
            if (Main.zenithWorld)
                NPC.scale = 1.5f;

            ZenithSeedMimicAI = new RevengeanceAndDeathAI.MimicAI();
            ZenithSeedMimicAI.NPC = NPC;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AstrumAureus")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(stomping);
            writer.Write(NPC.alpha);
            writer.Write(slimePhase);
            writer.Write(slimeProjCounter);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            stomping = reader.ReadBoolean();
            NPC.alpha = reader.ReadInt32();
            slimePhase = reader.ReadInt32();
            slimeProjCounter = reader.ReadInt32();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            CalamityGlobalNPC.astrumAureus = NPC.whoAmI;

            // Variables
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.85f : expertMode ? 0.8f : 0.75f);
            bool phase3 = lifeRatio < (revenge ? 0.7f : expertMode ? 0.6f : 0.5f);
            bool phase4 = lifeRatio < (revenge ? 0.5f : 0.4f) && expertMode;
            bool phase5 = lifeRatio < 0.3f && revenge;

            // Exhaustion
            bool exhausted = NPC.ai[2] >= (phase3 ? 2f : 1f);

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Don't fire projectiles and don't increment phase timers for 4 seconds after the teleport phase to avoid cheap bullshit
            float noProjectileOrPhaseIncrementTime = 240f;

            bool dontAttack = NPC.localAI[3] > 0f;
            if (dontAttack)
            {
                NPC.localAI[3] -= 1f;
                if (NPC.Distance(player.Center) < 240f)
                    NPC.localAI[3] -= death ? 4f : expertMode ? 2f : 1f;
            }

            float astralFlameBarrageTimerIncrement = 1f;
            if (expertMode)
                astralFlameBarrageTimerIncrement += death ? (float)Math.Round(3f * (1f - lifeRatio)) : (float)Math.Round(2f * (1f - lifeRatio));

            float walkingVelocity = phase5 ? 7f : 5f;
            if (expertMode)
                walkingVelocity += 1.5f * (1f - lifeRatio);
            if (revenge)
                walkingVelocity += Math.Abs(NPC.Center.X - player.Center.X) * 0.0025f;
            if (Main.getGoodWorld)
                walkingVelocity *= 1.15f;

            float walkingProjectileVelocity = walkingVelocity * 0.8f;

            // Direction
            NPC.spriteDirection = (NPC.direction > 0) ? 1 : -1;

            // Used to reduce Aureus' fall speed
            bool reduceFallSpeed = NPC.velocity.Y > 0f && Collision.SolidCollision(NPC.position + Vector2.UnitY * 1.1f * NPC.velocity.Y, NPC.width, NPC.height) && NPC.ai[0] == 4f;

            // Despawn
            bool despawnDistance = Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;
            if (!player.active || player.dead || despawnDistance)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || despawnDistance)
                {
                    NPC.noTileCollide = true;

                    if (NPC.velocity.Y < -3f)
                        NPC.velocity.Y = -3f;
                    NPC.velocity.Y += 0.1f;
                    if (NPC.velocity.Y > 12f)
                        NPC.velocity.Y = 12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[0] != 0f)
                    {
                        NPC.ai[0] = 0f;
                        NPC.ai[1] = 0f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.localAI[2] = 0f;
                        NPC.localAI[3] = 0f;
                        calamityGlobalNPC.newAI[0] = 0f;
                        calamityGlobalNPC.newAI[1] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            bool geldonPhase1 = lifeRatio > 0.6f && lifeRatio <= 0.7f;
            bool geldonPhase2 = lifeRatio <= 0.1f;
            if (Main.zenithWorld && (geldonPhase1 || geldonPhase2))
            {
                slimeProjCounter++;
                if (slimeProjCounter % 180 == 0)
                {
                    SoundEngine.PlaySound(SoundID.Item33, NPC.Center); // Intentionally keeping the old laser sound in GFB

                    if (slimePhase == 1)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<AstralFlame>();
                            int totalProjectiles = death ? 12 : revenge ? 10 : expertMode ? 8 : 6;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            float velocity = 10f;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, type, CrystalDamage, 0f, Main.myPlayer, 0f, 1f);
                            }
                        }
                        slimePhase = 0;
                    }
                    else
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<AstralLaser>();
                            float aureusLaserSpeed = 7f;
                            float aureusLaserTargetX = player.Center.X - NPC.Center.X;
                            float aureusLaserTargetY = player.Center.Y - NPC.Center.Y;
                            float aureusLaserTargetDist = (float)Math.Sqrt(aureusLaserTargetX * aureusLaserTargetX + aureusLaserTargetY * aureusLaserTargetY);
                            aureusLaserTargetDist = aureusLaserSpeed / aureusLaserTargetDist;
                            aureusLaserTargetX *= aureusLaserTargetDist;
                            aureusLaserTargetY *= aureusLaserTargetDist;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, aureusLaserTargetX, aureusLaserTargetY, type, LaserDamage, 0f, Main.myPlayer);
                            for (int i = 0; i < 4; i++)
                            {
                                Vector2 offset = new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7));
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, aureusLaserTargetX + offset.X, aureusLaserTargetY + offset.Y, type, LaserDamage, 0f, Main.myPlayer);
                            }
                        }
                        slimePhase = 1;
                    }
                }
                ZenithSeedMimicAI.AI(Mod);
                NPC.noGravity = false;
                NPC.noTileCollide = false;
                return;
            }
            else
            {
                NPC.noGravity = true;
            }

            // Emit light when not Idle
            if (NPC.ai[0] != 1f)
                Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 1.3f, 0.5f, 0f);

            // Fire projectiles while walking, teleporting, or falling
            if (NPC.ai[0] == 2f || NPC.ai[0] >= 5f)
            {
                if (!dontAttack)
                    NPC.localAI[0] += NPC.ai[0] == 2f ? 1f : astralFlameBarrageTimerIncrement;

                float astralFlameBarrageGateValue = phase4 ? 30f : 60f;
                if (NPC.localAI[0] >= astralFlameBarrageGateValue)
                {
                    // Fire astral flames while teleporting
                    if (NPC.ai[0] >= 5f && NPC.ai[0] != 7)
                    {
                        NPC.localAI[0] = 0f;
                        SoundEngine.PlaySound(FlameCrystalSound, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float velocity = death ? (8f + NPC.localAI[2] * 0.025f) : 7f;
                            int type = ModContent.ProjectileType<AstralFlame>();
                            float spreadLimit = phase4 ? 100f : 50f;
                            float randomSpread = (Main.rand.NextFloat() - 0.5f) * spreadLimit;
                            Vector2 spawnVector = new Vector2(NPC.Center.X, NPC.Center.Y - 80f * NPC.scale);
                            Vector2 destination = new Vector2(spawnVector.X + randomSpread, spawnVector.Y - 100f * NPC.scale);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector, Vector2.Normalize(destination - spawnVector) * velocity, type, CrystalDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                float laserBarrageGateValue = phase5 ? 160f : phase3 ? 120f : phase2 ? 80f : 60f;
                if (NPC.localAI[0] >= laserBarrageGateValue)
                {
                    // Fire astral lasers while walking
                    if (NPC.ai[0] == 2f)
                    {
                        NPC.localAI[0] = 0f;

                        SoundEngine.PlaySound(LaserSound, NPC.Center);

                        if (calamityGlobalNPC.newAI[2] == 0f)
                        {
                            calamityGlobalNPC.newAI[2] = 1f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int maxProjectiles = !phase2 ? 3 : 5;
                                int spread = !phase2 ? 8 : 10;

                                int type = ModContent.ProjectileType<AstralLaser>();
                                Vector2 projectileVelocity = Vector2.Normalize(player.Center - NPC.Center) * walkingProjectileVelocity;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < maxProjectiles; i++)
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(maxProjectiles - 1)));
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, LaserDamage, 0f, Main.myPlayer, 0f, walkingProjectileVelocity * 2f);
                                }

                                if (phase3)
                                {
                                    float flameVelocity = walkingProjectileVelocity;
                                    maxProjectiles = 2;
                                    spread = 45;

                                    type = ModContent.ProjectileType<AstralFlame>();
                                    projectileVelocity = Vector2.Normalize(player.Center - NPC.Center) * flameVelocity;
                                    rotation = MathHelper.ToRadians(spread);
                                    for (int i = 0; i < maxProjectiles; i++)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(maxProjectiles - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, CrystalDamage, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                        else
                        {
                            calamityGlobalNPC.newAI[2] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int maxProjectiles = !phase3 ? (death ? 11 : 9) : (death ? 17 : 15);
                                int spread = !phase3 ? (death ? 18 : 16) : (death ? 22 : 20);

                                int type = ModContent.ProjectileType<AstralLaser>();
                                int centralLaser = maxProjectiles / 2;
                                int[] lasersToNotFire = new int[6] { centralLaser - 3, centralLaser - 2, centralLaser - 1, centralLaser + 1, centralLaser + 2, centralLaser + 3 };
                                Vector2 projectileVelocity = Vector2.Normalize(player.Center - NPC.Center) * walkingProjectileVelocity;
                                float rotation = MathHelper.ToRadians(spread);
                                for (int i = 0; i < maxProjectiles; i++)
                                {
                                    if (i != lasersToNotFire[0] && i != lasersToNotFire[1] && i != lasersToNotFire[2] && i != lasersToNotFire[3] && i != lasersToNotFire[4] && i != lasersToNotFire[5])
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(maxProjectiles - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, LaserDamage, 0f, Main.myPlayer, 0f, walkingProjectileVelocity * 2f);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else
                NPC.localAI[0] = 0f;

            // Start up
            if (NPC.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.ai[0] = 1f;
                NPC.netUpdate = true;
                CustomGravity();
            }

            // Idle
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Slow down
                NPC.velocity.X *= 0.8f;

                // Stay vulnerable for 3 seconds
                NPC.ai[1] += 1f;
                if (NPC.Distance(player.Center) < 240f)
                    NPC.ai[1] += death ? 4f : expertMode ? 2f : 1f;

                if (NPC.ai[1] >= 180f)
                {
                    // Set AI to random state and reset other AI arrays
                    NPC.TargetClosest();
                    switch (Main.rand.Next(phase3 ? 3 : 2))
                    {
                        case 0:
                            NPC.ai[0] = 2f;
                            break;

                        case 1:
                            NPC.ai[0] = 3f;
                            break;

                        case 2:
                            NPC.ai[0] = 5f;
                            break;

                        default:
                            break;
                    }
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;

                    // Stop colliding with tiles if entering walking phase
                    NPC.noTileCollide = NPC.ai[0] == 2f;

                    NPC.netUpdate = true;
                }
                else
                    CustomGravity();
            }

            // Walk
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Set walking direction
                if (Math.Abs(NPC.Center.X - player.Center.X) < 200f * NPC.scale)
                {
                    NPC.velocity.X *= 0.8f;
                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;

                    if (NPC.direction > 0)
                        NPC.velocity.X = (NPC.velocity.X * 20f + walkingVelocity) / 21f;
                    if (NPC.direction < 0)
                        NPC.velocity.X = (NPC.velocity.X * 20f - walkingVelocity) / 21f;
                }

                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && player.position.Y <= NPC.position.Y + NPC.height && !NPC.collideX)
                {
                    CustomGravity();
                    NPC.noTileCollide = false;
                }
                else
                {
                    NPC.noTileCollide = true;

                    // Walk through tiles if colliding with tiles and player is out of reach
                    int aureusHitboxWidth = 80;
                    int aureusHitboxHeight = 20;
                    Vector2 aureusHitboxTileCollideSize = new Vector2(NPC.Center.X - (aureusHitboxWidth / 2), NPC.position.Y + NPC.height - aureusHitboxHeight);

                    bool nearPlayerWalkingThroughTiles = false;
                    if (NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width && NPC.position.Y + NPC.height < player.position.Y + player.height - 16f)
                        nearPlayerWalkingThroughTiles = true;

                    if (nearPlayerWalkingThroughTiles)
                    {
                        NPC.velocity.Y += 0.5f;
                    }
                    else if (Collision.SolidCollision(aureusHitboxTileCollideSize, aureusHitboxWidth, aureusHitboxHeight))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = 0f;

                        if (NPC.velocity.Y > -0.2)
                            NPC.velocity.Y -= 0.025f;
                        else
                            NPC.velocity.Y -= 0.2f;

                        if (NPC.velocity.Y < -4f)
                            NPC.velocity.Y = -4f;
                    }
                    else
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y = 0f;

                        if (NPC.velocity.Y < 0.1)
                            NPC.velocity.Y += 0.025f;
                        else
                            NPC.velocity.Y += 0.5f;
                    }
                }

                // Walk for a maximum of 6 seconds
                if (!dontAttack)
                {
                    NPC.ai[1] += 1f;
                    if (NPC.Distance(player.Center) < 240f)
                        NPC.ai[1] += death ? 4f : expertMode ? 2f : 1f;
                }

                if (NPC.ai[1] >= (360f - (death ? 90f * (1f - lifeRatio) : 0f)))
                {
                    // Collide with tiles again
                    NPC.noTileCollide = false;

                    // Set AI to next phase (Jump) and reset other AI
                    NPC.TargetClosest();
                    NPC.ai[0] = exhausted ? 1f : 3f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    NPC.netUpdate = true;
                }

                // Limit downward velocity
                if (NPC.velocity.Y > 10f)
                    NPC.velocity.Y = 10f;
            }

            // Jump
            else if (NPC.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.noTileCollide = false;

                if (NPC.velocity.Y == 0f)
                {
                    // Slow down
                    NPC.velocity.X *= 0.8f;

                    // Half second delay before jumping
                    if (!dontAttack)
                        NPC.ai[1] += 1f;

                    if (NPC.ai[1] >= 30f)
                    {
                        NPC.ai[1] = -20f;
                    }
                    else if (NPC.ai[1] == -1f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        // Set jump velocity, reset and set AI to next phase (Stomp)
                        float distanceFromPlayerOnXAxis = NPC.Center.X - player.Center.X;
                        NPC.direction = distanceFromPlayerOnXAxis < 0 ? 1 : -1;
                        calamityGlobalNPC.newAI[3] = NPC.direction;

                        // The limit for how much Aureus can multiply its jump velocity
                        float speedMultLimit = 1f;

                        // Maxes out when the player is a full 2000 pixels away from or above Aureus
                        float multiplier = 1f / 2000f;

                        // Increase Aureus jump velocity X if it's far enough away from the player
                        float distanceAwayFromTarget = Math.Abs(distanceFromPlayerOnXAxis);
                        float distanceGateValue = 400f;
                        bool increaseJumpVelocityX = distanceAwayFromTarget > distanceGateValue && expertMode;
                        if (increaseJumpVelocityX)
                        {
                            calamityGlobalNPC.newAI[0] = (distanceAwayFromTarget - distanceGateValue) * multiplier;
                            if (calamityGlobalNPC.newAI[0] > speedMultLimit)
                                calamityGlobalNPC.newAI[0] = speedMultLimit;
                        }

                        // Increase Aureus jump velocity Y if it's far enough above the player
                        float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                        bool increaseJumpVelocityY = distanceBelowTarget > 0f && revenge;
                        if (increaseJumpVelocityY)
                        {
                            calamityGlobalNPC.newAI[1] = distanceBelowTarget * multiplier;
                            if (calamityGlobalNPC.newAI[1] > speedMultLimit)
                                calamityGlobalNPC.newAI[1] = speedMultLimit;
                        }

                        float velocity = 20f;
                        if (expertMode)
                            velocity += death ? 6f * (1f - lifeRatio) : 4f * (1f - lifeRatio);
                        if (Main.getGoodWorld)
                            velocity *= 1.15f;

                        NPC.velocity = (new Vector2(player.Center.X, player.Center.Y - 500f) - NPC.Center).SafeNormalize(Vector2.Zero) * velocity;
                        NPC.velocity *= new Vector2(calamityGlobalNPC.newAI[0] + 1f, calamityGlobalNPC.newAI[1] + 1f);

                        NPC.noTileCollide = true;

                        NPC.ai[0] = 4f;
                        NPC.ai[1] = 0f;

                        SoundEngine.PlaySound(JumpSound, NPC.Center);

                        NPC.netUpdate = true;
                    }
                }

                // Don't run custom gravity when starting a jump
                if (NPC.ai[0] != 4f)
                    CustomGravity();
            }

            // Stomp
            else if (NPC.ai[0] == 4f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    // Play stomp sound. Gotta specify the filepath to avoid confusion between the namespace and NPC
                    SoundStyle soundToPlay = Main.zenithWorld ? ExoMechs.Ares.AresGaussNuke.NukeExplosionSound : StompSound;
                    SoundEngine.PlaySound(soundToPlay, NPC.Center);

                    if (Main.zenithWorld)
                    {
                        float screenShakePower = 16 * Utils.GetLerpValue(1300f, 0f, NPC.Distance(Main.LocalPlayer.Center), true);
                        Main.LocalPlayer.SetScreenshake(screenShakePower);
                    }

                    // Stomp and jump again, if stomped twice then reset and set AI to next phase (Teleport or Idle)
                    NPC.TargetClosest();
                    NPC.localAI[1] += 1f;
                    float maxStompAmt = phase5 ? 5f : phase3 ? 2f : 3f;
                    if (NPC.localAI[1] >= maxStompAmt)
                    {
                        NPC.ai[0] = exhausted ? 1f : (phase3 ? 5f : 2f);
                        NPC.localAI[1] = 0f;
                        NPC.ai[2] += 1f;
                        NPC.ai[3] = 0f;
                        NPC.noTileCollide = false;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;
                        NPC.ai[0] = 3f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }

                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    calamityGlobalNPC.newAI[3] = 0f;

                    // Spawn dust for visual effect
                    for (int i = (int)NPC.position.X - 20; i < (int)NPC.position.X + NPC.width + 40; i += 20)
                    {
                        for (int j = 0; j < 4; j++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y + NPC.height), NPC.width + 20, 4, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }
                    }

                    // Fire lasers or flames on stomp
                    SoundEngine.PlaySound(LaserSound, NPC.Center);

                    if (Main.zenithWorld)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            bool crystal = Main.rand.NextBool();
                            int type = crystal ? ModContent.ProjectileType<AstralFlame>() : ModContent.ProjectileType<AstralLaser>();
                            int damage = crystal ? CrystalDamage : LaserDamage;
                            int totalProjectiles = death ? 12 : revenge ? 10 : expertMode ? 8 : 6;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            float velocity = 10f;
                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 velocity2 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, velocity2, type, damage, 0f, Main.myPlayer, 0f, 1f);
                            }
                        }
                    }
                    else if (calamityGlobalNPC.newAI[2] == 0f && phase2)
                    {
                        calamityGlobalNPC.newAI[2] = 1f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float flameVelocity = 6f;
                            int maxProjectiles = death ? 3 : 2;
                            int spread = death ? 28 : 20;

                            int type = ModContent.ProjectileType<AstralFlame>();
                            Vector2 spawnVector = new Vector2(NPC.Center.X, NPC.Center.Y - 80f * NPC.scale);
                            Vector2 destination = new Vector2(spawnVector.X, spawnVector.Y + 100f * NPC.scale);
                            Vector2 projectileVelocity = Vector2.Normalize(destination - spawnVector) * flameVelocity;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < maxProjectiles; i++)
                            {
                                Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(maxProjectiles - 1)));
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnVector + Vector2.Normalize(perturbedSpeed) * 100f, perturbedSpeed, type, CrystalDamage, 0f, Main.myPlayer);
                            }
                        }
                    }
                    else
                    {
                        calamityGlobalNPC.newAI[2] = 0f;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float laserVelocity = Main.getGoodWorld ? 7f : death ? 6f : 5f;
                            int maxProjectiles = !phase3 ? (death ? 11 : 9) : (death ? 15 : 13);
                            int spread = !phase3 ? (death ? 18 : 16) : (death ? 22 : 20);

                            int type = ModContent.ProjectileType<AstralLaser>();
                            int[] lasersToNotFire = new int[4] { 1, 3, maxProjectiles - 2, maxProjectiles - 4 };
                            Vector2 projectileVelocity = Vector2.Normalize(player.Center - NPC.Center) * laserVelocity;
                            float rotation = MathHelper.ToRadians(spread);
                            for (int i = 0; i < maxProjectiles; i++)
                            {
                                if (i != lasersToNotFire[0] && i != lasersToNotFire[1] && i != lasersToNotFire[2] && i != lasersToNotFire[3])
                                {
                                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(maxProjectiles - 1)));
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, LaserDamage, 0f, Main.myPlayer, 0f, laserVelocity * 2f);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    // Set velocities while falling, this happens before the stomp
                    // Fall through
                    if (!player.dead)
                    {
                        if ((player.position.Y > NPC.Bottom.Y && NPC.velocity.Y > 0f) || (player.position.Y < NPC.Bottom.Y && NPC.velocity.Y < 0f))
                            NPC.noTileCollide = true;
                        else if ((NPC.velocity.Y > 0f && NPC.Bottom.Y > Main.player[NPC.target].Top.Y) || (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height)))
                            NPC.noTileCollide = false;
                    }

                    if (NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width)
                    {
                        // Make sure Aureus falls quickly when directly on top of or below the player
                        if (NPC.ai[3] < 30f)
                            NPC.ai[3] = 30f;

                        NPC.velocity.X *= 0.8f;

                        if (NPC.Bottom.Y < player.position.Y)
                        {
                            // Make sure Aureus falls rather quickly
                            if (NPC.velocity.Y < -3f)
                                NPC.velocity.Y = -3f;

                            float fallSpeed = 1.2f;
                            if (expertMode)
                                fallSpeed += death ? 0.36f * (1f - lifeRatio) : 0.24f * (1f - lifeRatio);
                            if (Main.getGoodWorld)
                                fallSpeed += 0.5f;

                            if (calamityGlobalNPC.newAI[1] > 0f)
                                fallSpeed *= calamityGlobalNPC.newAI[1] + 1f;

                            NPC.velocity.Y += fallSpeed;
                        }
                    }
                    else
                    {
                        // Push Aureus towards the player on the X axis if he's not directly on top of or below the player
                        float velocityXChange = 0.2f + Math.Abs(NPC.Center.X - player.Center.X) * 0.0001f;

                        if (calamityGlobalNPC.newAI[0] > 0f)
                            velocityXChange *= calamityGlobalNPC.newAI[0] + 1f;

                        if (NPC.direction < 0)
                            NPC.velocity.X -= velocityXChange;
                        else if (NPC.direction > 0)
                            NPC.velocity.X += velocityXChange;

                        float velocityXCap = 12f;
                        if (expertMode)
                            velocityXCap += death ? 3.6f * (1f - lifeRatio) : 2.4f * (1f - lifeRatio);
                        if (Main.getGoodWorld)
                            velocityXCap += 5f;

                        if (calamityGlobalNPC.newAI[0] > 0f)
                            velocityXCap *= calamityGlobalNPC.newAI[0] + 1f;

                        float playerLocation = NPC.Center.X - player.Center.X;
                        int directionRelativeToTarget = playerLocation < 0 ? 1 : -1;
                        bool slowDown = directionRelativeToTarget != calamityGlobalNPC.newAI[3];

                        if (slowDown)
                            velocityXCap *= 0.333f;

                        if (NPC.velocity.X < -velocityXCap)
                            NPC.velocity.X = -velocityXCap;
                        if (NPC.velocity.X > velocityXCap)
                            NPC.velocity.X = velocityXCap;
                    }

                    // Don't start falling quickly until half a second has passed
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] > 30f)
                        CustomGravity();
                }
            }

            // Teleport
            else if (NPC.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Slow down
                NPC.velocity.X *= 0.8f;

                // Start teleport
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[1] += 1f;
                    if (death)
                        NPC.localAI[2] += 1.25f;

                    if (phase4)
                    {
                        NPC.localAI[1] += 1f;
                        if (death)
                            NPC.localAI[2] += 1.25f;
                    }

                    if (NPC.localAI[1] >= (death ? 180f : 240f))
                    {
                        // Reset localAI and find a teleport destination
                        NPC.TargetClosest();
                        NPC.localAI[1] = 0f;

                        Vector2 vectorAimedAheadOfTarget = Main.player[NPC.target].Center + new Vector2((float)Math.Round(Main.player[NPC.target].velocity.X), 0f).SafeNormalize(Vector2.Zero) * 1000f;
                        Point point4 = vectorAimedAheadOfTarget.ToTileCoordinates();
                        int teleportTries = 0;
                        while (teleportTries < 100)
                        {
                            teleportTries++;
                            int teleportTileX = Main.rand.Next(point4.X - 5, point4.X + 6);
                            int teleportTileY = Main.rand.Next(point4.Y - 5, point4.Y);

                            if (!Main.tile[teleportTileX, teleportTileY].HasUnactuatedTile)
                            {
                                NPC.ai[1] = teleportTileX * 16 + 8;
                                NPC.ai[3] = teleportTileY * 16 + 16;
                                break;
                            }
                        }

                        // Default teleport if the above conditions aren't met in 100 iterations
                        if (teleportTries >= 100)
                        {
                            Vector2 bottom = Main.player[Player.FindClosest(NPC.position, NPC.width, NPC.height)].Bottom;
                            NPC.ai[1] = bottom.X;
                            NPC.ai[3] = bottom.Y;
                        }

                        // Set AI to next phase (Mid-teleport)
                        NPC.ai[0] = 6f;
                        NPC.netUpdate = true;
                    }
                }

                CustomGravity();
            }

            // Mid-teleport
            else if (NPC.ai[0] == 6f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (death)
                    NPC.localAI[2] += 1.25f;

                if (phase4)
                {
                    if (death)
                        NPC.localAI[2] += 1.25f;
                }

                // Turn invisible
                NPC.alpha += 10;
                if (NPC.alpha >= 255)
                {
                    // Set position to teleport destination
                    NPC.Bottom = new Vector2(NPC.ai[1], NPC.ai[3]);

                    // Reset alpha and set AI to next phase (End of teleport)
                    NPC.alpha = 255;
                    NPC.ai[0] = 7f;
                    NPC.localAI[2] = 0f;
                    NPC.netUpdate = true;
                }

                // Play sound for cool effect
                if (NPC.soundDelay == 0)
                {
                    NPC.soundDelay = 15;
                    SoundEngine.PlaySound(TeleportSound, NPC.Center);
                }

                // Emit dust to make the teleport pretty
                for (int i = 0; i < 10; i++)
                {
                    int teleportDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), NPC.velocity.X, NPC.velocity.Y, 255, default, 2f);
                    Main.dust[teleportDust].noGravity = true;
                    Main.dust[teleportDust].velocity *= 0.5f;
                }

                CustomGravity();
            }

            // End of teleport
            else if (NPC.ai[0] == 7f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                // Turn visible
                NPC.alpha -= 10;
                if (NPC.alpha <= 0)
                {
                    // Spawn Aureus Spawns
                    bool spawnFlag = expertMode;
                    if (NPC.CountNPCS(ModContent.NPCType<AureusSpawn>()) >= 2)
                        spawnFlag = false;

                    if (spawnFlag && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int aureusSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y - 25f * NPC.scale), ModContent.NPCType<AureusSpawn>());
                        Main.npc[aureusSpawn].velocity.Y = -10f;
                        Main.npc[aureusSpawn].netUpdate = true;
                        if (revenge)
                        {
                            aureusSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)(NPC.Center.Y - 25f * NPC.scale), ModContent.NPCType<AureusSpawn>());
                            Main.npc[aureusSpawn].velocity.Y = -15f;
                            Main.npc[aureusSpawn].netUpdate = true;
                        }

                        if (death)
                        {
                            int damageAmt = NPC.lifeMax / 50;
                            NPC.life -= damageAmt;
                            if (NPC.life < 1)
                                NPC.life = 1;

                            NPC.DamageEffect(damageAmt);
                        }
                    }

                    // Reset alpha and set AI to next phase (Idle)
                    NPC.alpha = 0;
                    NPC.ai[0] = exhausted ? 1f : 2f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] += 1f;
                    NPC.localAI[3] = noProjectileOrPhaseIncrementTime;

                    // Stop colliding with tiles if entering walking phase
                    NPC.noTileCollide = NPC.ai[0] == 2f;

                    NPC.netUpdate = true;
                }

                // Play sound at teleport destination for cool effect
                if (NPC.soundDelay == 0)
                {
                    NPC.soundDelay = 15;
                    SoundEngine.PlaySound(SoundID.Item109, NPC.Center);
                }

                // Emit dust to make the teleport pretty
                for (int i = 0; i < 10; i++)
                {
                    int teleportDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), NPC.velocity.X, NPC.velocity.Y, 255, default, 2f);
                    Main.dust[teleportDust].noGravity = true;
                    Main.dust[teleportDust].velocity *= 0.5f;
                }

                CustomGravity();
            }

            void CustomGravity()
            {
                float gravity = 0.36f;
                float maxFallSpeed = 12f;

                if (calamityGlobalNPC.newAI[1] > 0f && !reduceFallSpeed)
                    maxFallSpeed *= calamityGlobalNPC.newAI[1] + 1f;

                if (Main.getGoodWorld && !reduceFallSpeed)
                {
                    gravity *= 1.15f;
                    maxFallSpeed *= 1.15f;
                }

                NPC.velocity.Y += gravity;
                if (NPC.velocity.Y > maxFallSpeed)
                    NPC.velocity.Y = maxFallSpeed;
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.target >= 0 && Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height;

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[0] == 3f || NPC.ai[0] == 4f)
            {
                if (NPC.velocity.Y == 0f && NPC.ai[1] >= 0f && NPC.ai[0] == 3f) //idle before jump
                {
                    if (stomping)
                        stomping = false;

                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                        NPC.frame.Y = 0;
                }
                else if (NPC.velocity.Y <= 0f || NPC.ai[1] < 0f) //prepare to jump and then jump
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
                else //stomping
                {
                    if (!stomping)
                    {
                        stomping = true;
                        NPC.frameCounter = 0D;
                        NPC.frame.Y = 0;
                    }

                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
            }
            else if (NPC.ai[0] >= 5f)
            {
                if (stomping)
                    stomping = false;

                if (NPC.velocity.Y == 0f) //idle before teleport
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 6)
                        NPC.frame.Y = 0;
                }
                else //in-air
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight * 5)
                        NPC.frame.Y = frameHeight * 5;
                }
            }
            else
            {
                if (stomping)
                    stomping = false;

                NPC.frameCounter += 1D;
                if (NPC.frameCounter > 8D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y >= frameHeight * 6)
                    NPC.frame.Y = 0;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            bool slimePhaseHP = lifeRatio <= 0.1f || (lifeRatio > 0.6f && lifeRatio <= 0.7f);

            Texture2D NPCTexture = TextureAssets.Npc[Type].Value;
            Texture2D GlowMaskTexture = TextureAssets.Npc[Type].Value;
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            if (NPC.ai[0] == 0f || (slimePhaseHP && Main.zenithWorld))
            {
                NPCTexture = TextureAssets.Npc[Type].Value;
                GlowMaskTexture = Texture_Glow.Value;
            }
            else if (NPC.ai[0] == 1f) //nothing special done here
            {
                NPCTexture = RechargeTexture.Value;
            }
            else if (NPC.ai[0] == 2f) //nothing special done here
            {
                NPCTexture = WalkTexture.Value;
                GlowMaskTexture = WalkTexture_Glow.Value;
            }
            else if (NPC.ai[0] == 3f || NPC.ai[0] == 4f) //needs to have an in-air frame
            {
                if (NPC.velocity.Y == 0f && NPC.ai[1] >= 0f && NPC.ai[0] == 3f) //idle before jump
                {
                    NPCTexture = TextureAssets.Npc[Type].Value; //idle frames
                    GlowMaskTexture = Texture_Glow.Value;
                }
                else if (NPC.velocity.Y <= 0f || NPC.ai[1] < 0f) //jump frames if flying upward or if about to jump
                {
                    NPCTexture = JumpTexture.Value;
                    GlowMaskTexture = JumpTexture_Glow.Value;
                }
                else //stomping
                {
                    NPCTexture = StompTexture.Value;
                    GlowMaskTexture = StompTexture_Glow.Value;
                }
            }
            else if (NPC.ai[0] >= 5f) //needs to have an in-air frame
            {
                if (NPC.velocity.Y == 0f) //idle before teleport
                {
                    NPCTexture = TextureAssets.Npc[Type].Value; //idle frames
                    GlowMaskTexture = Texture_Glow.Value;
                }
                else //in-air frames
                {
                    NPCTexture = JumpTexture.Value;
                    GlowMaskTexture = JumpTexture_Glow.Value;
                }
            }

            int frameCount = Main.npcFrameCount[Type];
            Vector2 originalDrawSize = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / frameCount / 2);
            Rectangle frame = NPC.frame;
            float scale = NPC.scale;
            float rotation = NPC.rotation;
            float offsetY = NPC.gfxOffY;
            Color slimeColor = Color.White;
            if (Main.zenithWorld && slimePhaseHP)
            {
                slimeColor = slimePhase == 0 ? Color.Yellow : Color.Violet;
            }
            float colorLerpAmt = 0.5f;
            int afterimageAmt = 7;
            if (NPC.ai[0] == 3f || NPC.ai[0] == 4f)
                afterimageAmt = 10;

            if (CalamityClientConfig.Instance.Afterimages)
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, slimeColor, colorLerpAmt);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
                    afterimagePos += originalDrawSize * scale + new Vector2(0f, 4f + offsetY);
                    spriteBatch.Draw(NPCTexture, afterimagePos, frame, afterimageColor, rotation, originalDrawSize, scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(NPCTexture.Width, NPCTexture.Height / frameCount) * scale / 2f;
            drawLocation += originalDrawSize * scale + new Vector2(0f, 4f + offsetY);
            Color toUse = Main.zenithWorld && slimePhaseHP ? slimeColor : drawColor;
            spriteBatch.Draw(NPCTexture, drawLocation, frame, NPC.GetAlpha(toUse), rotation, originalDrawSize, scale, spriteEffects, 0f);

            if (NPC.ai[0] != 1 || (slimePhaseHP && Main.zenithWorld)) //draw only if not recharging
            {
                Color color = new Color(127 - NPC.alpha, 127 - NPC.alpha, 127 - NPC.alpha, 0).MultiplyRGBA(Color.Gold);
                Color attackingColor = Color.Lerp(Color.White, color, 0.5f);
                if (Main.zenithWorld && slimePhaseHP)
                {
                    attackingColor = slimePhase == 0 ? Color.Violet : Color.Yellow;
                }

                if (CalamityClientConfig.Instance.Afterimages)
                {
                    for (int j = 1; j < afterimageAmt; j++)
                    {
                        Color attackingAfterimageColor = attackingColor;
                        attackingAfterimageColor = Color.Lerp(attackingAfterimageColor, slimeColor, colorLerpAmt);
                        attackingAfterimageColor = NPC.GetAlpha(attackingAfterimageColor);
                        attackingAfterimageColor *= (afterimageAmt - j) / 15f;
                        Vector2 attackAfterimagePos = NPC.oldPos[j] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        attackAfterimagePos -= new Vector2(GlowMaskTexture.Width, GlowMaskTexture.Height / frameCount) * scale / 2f;
                        attackAfterimagePos += originalDrawSize * scale + new Vector2(0f, 4f + offsetY);
                        spriteBatch.Draw(GlowMaskTexture, attackAfterimagePos, frame, attackingAfterimageColor, rotation, originalDrawSize, scale, spriteEffects, 0f);
                    }
                }

                spriteBatch.Draw(GlowMaskTexture, drawLocation, frame, attackingColor, rotation, originalDrawSize, scale, spriteEffects, 0f);
            }

            return false;
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AstrumAureusBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Nebulash>(),
                    ModContent.ItemType<AuroraBlazer>(),
                    ModContent.ItemType<AlulaAustralis>(),
                    ModContent.ItemType<BorealisBomber>(),
                    ModContent.ItemType<AuroradicalThrow>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Vanity
                normalOnly.Add(ModContent.ItemType<AstrumAureusMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<InterstellarStompers>()));

                // Other
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AureusCell>(), 1, 9, 12));
                normalOnly.Add(ModContent.ItemType<LeonidProgenitor>(), 10);
                normalOnly.Add(ModContent.ItemType<SuspiciousLookingJellyBean>());
            }

            npcLoot.Add(ModContent.ItemType<AstrumAureusTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<AstrumAureusRelic>());

            // GFB troll drops: Crab Banners and a random item which uses Luminite Bars
            // Luminite Bar item drop is handled in a separate mod system
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.CrabBanner, 1, 1, 9999), true);
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedAstrumAureus, ModContent.ItemType<LoreAstrumAureus>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If Astrum Aureus has not yet been killed, notify players of new Astral enemy drops
            if (!DownedBossSystem.downedAstrumAureus)
            {
                string key = "Mods.CalamityMod.Status.Progression.AureusBossText";
                string key2 = "Mods.CalamityMod.Status.Progression.AureusBossText2";
                Color messageColor = Color.Gold;

                CalamityUtils.BroadcastLocalizedText(key, messageColor);
                CalamityUtils.BroadcastLocalizedText(key2, messageColor);
            }

            // Drop an Astral Meteor if applicable
            ThreadPool.QueueUserWorkItem(_ => World.AstralBiome.PlaceAstralMeteor());

            // Mark Astrum Aureus as dead
            DownedBossSystem.downedAstrumAureus = true;
            CalamityNetcode.SyncWorld();
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = 16;
                SoundEngine.PlaySound(HitSound, NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(150 * NPC.scale);
                NPC.height = (int)(100 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int r = 0; r < 30; r++)
                {
                    int aureusDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                    Main.dust[aureusDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[aureusDust].scale = 0.5f;
                        Main.dust[aureusDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int s = 0; s < 60; s++)
                {
                    int aureusDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                    Main.dust[aureusDust2].noGravity = true;
                    Main.dust[aureusDust2].velocity *= 5f;
                    aureusDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                    Main.dust[aureusDust2].velocity *= 2f;
                }
                if (!Main.dedServ)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus1").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus4").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus5").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus6").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus7").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus8").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus9").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus10").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Aureus11").Type, 1f);
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
            Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - 92f * NPC.scale), (int)(npcCenter.Y + 28f * NPC.scale), 10, 10);
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f) + 24f * NPC.scale), NPC.width / 2, NPC.height);
            Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + 92f * NPC.scale), (int)(npcCenter.Y + 28f * NPC.scale), 10, 10);

            Vector2 leftHitboxCenter = new Vector2(leftHitbox.X + (leftHitbox.Width / 2), leftHitbox.Y + (leftHitbox.Height / 2));
            Vector2 bodyHitboxCenter = new Vector2(bodyHitbox.X + (bodyHitbox.Width / 2), bodyHitbox.Y + (bodyHitbox.Height / 2));
            Vector2 rightHitboxCenter = new Vector2(rightHitbox.X + (rightHitbox.Width / 2), rightHitbox.Y + (rightHitbox.Height / 2));

            Rectangle targetHitbox = target.Hitbox;

            float leftDist1 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopLeft());
            float leftDist2 = Vector2.Distance(leftHitboxCenter, targetHitbox.TopRight());
            float leftDist3 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomLeft());
            float leftDist4 = Vector2.Distance(leftHitboxCenter, targetHitbox.BottomRight());

            float minLeftDist = leftDist1;
            if (leftDist2 < minLeftDist)
                minLeftDist = leftDist2;
            if (leftDist3 < minLeftDist)
                minLeftDist = leftDist3;
            if (leftDist4 < minLeftDist)
                minLeftDist = leftDist4;

            bool insideLeftHitbox = minLeftDist <= 120f * NPC.scale;

            float bodyDist1 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopLeft());
            float bodyDist2 = Vector2.Distance(bodyHitboxCenter, targetHitbox.TopRight());
            float bodyDist3 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomLeft());
            float bodyDist4 = Vector2.Distance(bodyHitboxCenter, targetHitbox.BottomRight());

            float minBodyDist = bodyDist1;
            if (bodyDist2 < minBodyDist)
                minBodyDist = bodyDist2;
            if (bodyDist3 < minBodyDist)
                minBodyDist = bodyDist3;
            if (bodyDist4 < minBodyDist)
                minBodyDist = bodyDist4;

            bool insideBodyHitbox = minBodyDist <= 160f * NPC.scale;

            float rightDist1 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopLeft());
            float rightDist2 = Vector2.Distance(rightHitboxCenter, targetHitbox.TopRight());
            float rightDist3 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomLeft());
            float rightDist4 = Vector2.Distance(rightHitboxCenter, targetHitbox.BottomRight());

            float minRightDist = rightDist1;
            if (rightDist2 < minRightDist)
                minRightDist = rightDist2;
            if (rightDist3 < minRightDist)
                minRightDist = rightDist3;
            if (rightDist4 < minRightDist)
                minRightDist = rightDist4;

            bool insideRightHitbox = minRightDist <= 120f * NPC.scale;

            return (insideLeftHitbox || insideBodyHitbox || insideRightHitbox) && NPC.alpha == 0 && NPC.ai[0] > 1f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 360);
        }
    }

    // This ModSystem is used to allow Aureus to drop a random item which uses Luminite Bars in its recipe in Get fixed boi.
    // This cannot be placed in ModifyNPCLoot, because ModifyNPCLoot is run before modded recipes are initialized. Thus, placing it there would only drop vanilla items.
    // As a funny side effect, this also allows for dropping items from other mods which use Luminite Bars in their recipes.
    public class AddAureusGFBDrop : ModSystem
    {
        public override void PostSetupRecipes()
        {
            // Iterate through all recipes, and list the items which use Luminite Bars in their recipes.
            List<int> luminiteStuff = [];
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                if (Main.recipe[i].ContainsIngredient(ItemID.LunarBar) && !luminiteStuff.Contains(Main.recipe[i].createItem.type))
                    luminiteStuff.Add(Main.recipe[i].createItem.type);
            }
            // Define the conditional for GFB.
            LeadingConditionRule GFBOnly = new LeadingConditionRule(DropHelper.GFB);
            GFBOnly.OnSuccess(ItemDropRule.OneFromOptionsNotScalingWithLuck(1, luminiteStuff.ToArray()));
            Main.ItemDropsDB.RegisterToNPCNetId(ModContent.NPCType<AstrumAureus>(), GFBOnly);
        }
    }
}
