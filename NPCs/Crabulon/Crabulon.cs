using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions.Alcohol;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Events;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Crabulon
{
    [AutoloadBossHead]
    public class Crabulon : ModNPC
    {
        private bool stomping = false;
        private const float TelegraphTimeBeforeBigJump = 20f;
        private const float DelayBeforeBigJump = 50f;

        public static Asset<Texture2D> AltTexture;
        public static Asset<Texture2D> AttackTexture;
        public static Asset<Texture2D> Texture_Glow;
        public static Asset<Texture2D> AltTexture_Glow;
        public static Asset<Texture2D> AttackTexture_Glow;

        public static readonly SoundStyle JumpSound = new("CalamityMod/Sounds/Custom/Crabulon/CrabJump");
        public static readonly SoundStyle SlamSound = new("CalamityMod/Sounds/Custom/Crabulon/CrabSlam", 2);
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/CrabulonHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/CrabulonDeath");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 6;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.32f,
                PortraitScale = 0.55f,
                PortraitPositionYOverride = 54f
            };
            value.Position.Y += 80f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                AltTexture = ModContent.Request<Texture2D>(Texture + "Alt", AssetRequestMode.AsyncLoad);
                AttackTexture = ModContent.Request<Texture2D>(Texture + "Attack", AssetRequestMode.AsyncLoad);
                Texture_Glow = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                AltTexture_Glow = ModContent.Request<Texture2D>(Texture + "AltGlow", AssetRequestMode.AsyncLoad);
                AttackTexture_Glow = ModContent.Request<Texture2D>(Texture + "AttackGlow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int MushroomShotDamage = 9; // 36

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 40; // 64 (1.6x expert scaling)
            NPC.npcSlots = 14f;
            NPC.width = 196;
            NPC.height = 196;
            NPC.defense = 8;
            NPC.LifeMaxNERB(3500, 4400, 500000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.noGravity = false;
            NPC.noTileCollide = false;
            NPC.boss = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;

            if (Main.getGoodWorld)
            {
                NPC.scale *= 1.5f;
                NPC.defense += 12;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundMushroom,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Crabulon")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(stomping);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            stomping = reader.ReadBoolean();
        }

        public override void AI()
        {
            Lighting.AddLight(NPC.Center, 0f, 0.3f, 0.7f);

            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;

            NPC.spriteDirection = NPC.direction;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.66f && expertMode;
            bool phase3 = lifeRatio < 0.33f && expertMode;
            bool phase4 = lifeRatio < 0.15f && death;

            int despawnDistanceInTiles = 500;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];
            if (!player.active || player.dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Math.Abs(NPC.Center.X - Main.player[NPC.target].Center.X) / 16f > despawnDistanceInTiles)
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
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (NPC.ai[0] < 2f)
            {
                int mushBombAmt = phase4 ? 6 : phase3 ? 3 : phase2 ? 2 : 1;
                float fireRate = phase4 ? 6f : phase3 ? 3f : phase2 ? 2f : 1f;
                NPC.localAI[3] += fireRate;
                if (NPC.ai[3] == 0f)
                {
                    float shootMushroomsGateValue = revenge ? 120f : expertMode ? 200f : 300f;
                    if (NPC.localAI[3] > shootMushroomsGateValue)
                    {
                        NPC.ai[3] = 1f;
                        NPC.localAI[3] = 0f;
                    }
                }
                else if (NPC.localAI[3] > 30f)
                {
                    NPC.localAI[3] = 0f;
                    NPC.ai[3] += 1f;
                    if (NPC.ai[3] >= mushBombAmt)
                        NPC.ai[3] = 0f;

                    float mushBombSpeed = phase4 ? 16f : phase3 ? 14f : phase2 ? 12f : 10f;
                    int type = ModContent.ProjectileType<MushBomb>();
                    SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        float yVelocity = death ? 1f : expertMode ? 2.5f : 4f;
                        Vector2 projectileVelocity = (Vector2.Normalize(player.Center - NPC.Center) * mushBombSpeed) - Vector2.UnitY * yVelocity;
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, projectileVelocity, type, MushroomShotDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                    }
                }
            }

            if (NPC.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.velocity *= 0.98f;

                NPC.ai[1] += 1f;
                if (phase2)
                    NPC.ai[1] += 1f;
                if (phase3)
                    NPC.ai[1] += 1f;
                if (NPC.Distance(player.Center) < 160f)
                    NPC.ai[1] += death ? 4f : expertMode ? 2f : 1f;

                // Gets tired easily in final phase.
                float idleTime = phase4 ? 480f : death ? 60f : expertMode ? 90f : 120f;
                if (NPC.ai[1] >= idleTime)
                {
                    bool deathModeTripleStomp = Main.rand.NextBool() && death && phase2;
                    NPC.TargetClosest();
                    NPC.noGravity = !deathModeTripleStomp;
                    NPC.noTileCollide = !deathModeTripleStomp;
                    NPC.ai[0] = deathModeTripleStomp ? 5f : 1f;
                    NPC.ai[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                float walkingVelocity = death ? (5f + 1f * (1f - lifeRatio)) : expertMode ? 5f : 3.5f;
                if (phase2)
                    walkingVelocity += 0.5f;
                if (phase3)
                    walkingVelocity += 0.75f;
                if (phase4)
                    walkingVelocity += 1f;
                if (Main.getGoodWorld)
                    walkingVelocity *= 2f;

                bool shouldWalkSlower = false;
                if (Math.Abs(NPC.Center.X - player.Center.X) < 50f)
                    shouldWalkSlower = true;

                if (shouldWalkSlower)
                {
                    NPC.velocity.X *= 0.9f;
                    if (Math.Abs(NPC.velocity.X) < 0.1f)
                        NPC.velocity.X = 0f;
                }
                else
                {
                    float playerLocation = NPC.Center.X - player.Center.X;
                    NPC.direction = playerLocation < 0 ? 1 : -1;

                    float inertia = revenge ? 10f : 20f;
                    NPC.velocity.X = (NPC.velocity.X * inertia + walkingVelocity * NPC.direction) / (inertia + 1f);
                }

                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && player.position.Y <= NPC.position.Y + NPC.height && !NPC.collideX)
                {
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                }
                else
                {
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    Vector2 collisionCheckPosition = new Vector2(NPC.Center.X - 40, NPC.position.Y + NPC.height - 20);

                    bool fallDownOnTopOfTarget = NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width && NPC.position.Y + NPC.height < player.position.Y + player.height - 16f;
                    float acceleration = death ? 0.075f : expertMode ? 0.05f : 0.03f;
                    float acceleration2 = death ? 0.6f : expertMode ? 0.4f : 0.25f;
                    if (fallDownOnTopOfTarget)
                    {
                        float fallSpeed = death ? 1.5f : expertMode ? 1f : 0.5f;
                        NPC.velocity.Y += fallSpeed;
                    }
                    else if (Collision.SolidCollision(collisionCheckPosition, 80, 20))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y = 0f;

                        if (NPC.velocity.Y > -0.2f)
                            NPC.velocity.Y -= acceleration;
                        else
                            NPC.velocity.Y -= acceleration2;

                        float upwardSpeedCap = death ? 9f : expertMode ? 6f : 4f;
                        if (NPC.velocity.Y < -upwardSpeedCap)
                            NPC.velocity.Y = -upwardSpeedCap;
                    }
                    else
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y = 0f;

                        if (NPC.velocity.Y < 0.1f)
                            NPC.velocity.Y += acceleration;
                        else
                            NPC.velocity.Y += 0.5f;
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.Distance(player.Center) < 160f)
                    NPC.ai[1] += death ? 4f : expertMode ? 2f : 1f;

                float stompPhaseGateValue = (revenge ? 150f : expertMode ? 240f : 360f) - (death ? 60f * (1f - lifeRatio) : 0f);
                if (NPC.ai[1] >= stompPhaseGateValue)
                {
                    NPC.noGravity = false;
                    NPC.noTileCollide = false;
                    NPC.ai[0] = (Main.rand.NextBool() && revenge && phase2) ? 4f : 2f;
                    NPC.ai[1] = 0f;
                    NPC.ForceNetUpdate();
                }

                if (NPC.velocity.Y > 10f)
                    NPC.velocity.Y = 10f;
            }
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.noTileCollide = false;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] % 15f == 14f)
                        NPC.netUpdate = true;

                    if (NPC.ai[1] > 0f)
                    {
                        if (revenge)
                        {
                            switch ((int)NPC.ai[3])
                            {
                                case 0:
                                    break;
                                case 1:
                                case 2:
                                    NPC.ai[1] += 2f;
                                    break;
                                case 3:
                                    NPC.ai[1] += 4f;
                                    break;
                                default:
                                    break;
                            }
                        }
                        if (phase2)
                            NPC.ai[1] += !revenge ? 2f : 1f;
                        if (phase3)
                            NPC.ai[1] += !revenge ? 2f : 1f;
                        if (phase4)
                            NPC.ai[1] += 1f;
                    }

                    float jumpGateValue = expertMode ? 60f : 120f;
                    if (NPC.ai[1] >= jumpGateValue)
                    {
                        NPC.ai[1] = -20f;
                    }
                    else if (NPC.ai[1] == -1f)
                    {
                        float maxVelocityXIncrease = death ? 4f : 3f;
                        float maxVelocityYIncrease = death ? 3f : 2f;
                        float velocityX = 6f + (expertMode ? (maxVelocityXIncrease * (1f - lifeRatio)) : 0f);
                        float velocityY = 12f + (expertMode ? (maxVelocityYIncrease * (1f - lifeRatio)) : 0f);

                        float distanceBelowTarget = NPC.position.Y - (player.position.Y + 80f);
                        float speedMult = 1f;

                        if (revenge)
                        {
                            float velocityXAdjustment = velocityX;
                            float velocityYAdjustment = velocityY / 3f;

                            if (lifeRatio < 0.5f && death)
                            {
                                // Crab smash
                                switch ((int)NPC.ai[3])
                                {
                                    case 0: // Normal
                                        velocityX += velocityXAdjustment * 0.5f;
                                        velocityY -= velocityYAdjustment;
                                        break;
                                    case 1: // High
                                        velocityX += velocityXAdjustment * 0.5f;
                                        break;
                                    case 2: // Low
                                        velocityX += velocityXAdjustment * 0.5f;
                                        velocityY -= velocityYAdjustment * 2f;
                                        break;
                                    case 3: // Long and low
                                        velocityX += velocityXAdjustment * 1.5f;
                                        velocityY -= velocityYAdjustment * 2f;
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                switch ((int)NPC.ai[3])
                                {
                                    case 0: // Normal
                                        break;
                                    case 1: // High
                                        velocityY += velocityYAdjustment;
                                        break;
                                    case 2: // Low
                                        velocityY -= velocityYAdjustment;
                                        break;
                                    case 3: // Long and low
                                        velocityX += velocityXAdjustment;
                                        velocityY -= velocityYAdjustment;
                                        break;
                                    default:
                                        break;
                                }
                            }

                            if (distanceBelowTarget > 0f)
                                speedMult += distanceBelowTarget * 0.001f;

                            if (speedMult > 2f)
                                speedMult = 2f;

                            velocityY *= speedMult;
                        }

                        if (expertMode)
                        {
                            if (player.position.Y < NPC.Bottom.Y)
                                NPC.velocity.Y = -velocityY;
                            else
                                NPC.velocity.Y = 1f;

                            NPC.noTileCollide = true;
                        }
                        else
                            NPC.velocity.Y = -velocityY;

                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;

                        NPC.velocity.X = velocityX * NPC.direction;
                        SoundEngine.PlaySound(JumpSound, NPC.Center);

                        NPC.ai[0] = 3f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    SoundEngine.PlaySound(SlamSound, NPC.Center);

                    int type = ModContent.ProjectileType<MushBombFall>();

                    if ((NPC.ai[2] % 2f == 0f || death) && ((phase2 && revenge) || (phase3 && expertMode)))
                    {
                        SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float projectileVelocity = CalamityWorld.death ? 15f : 10f;
                            Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                            destination.Normalize();
                            destination *= projectileVelocity;

                            // Less mushrooms in Death Mode phase 3 because otherwise it's an absolute shitshow.
                            int numProj = phase4 ? 14 : CalamityWorld.death ? (phase3 ? 10 : 16) : 12;
                            float rotation = MathHelper.ToRadians(90);
                            for (int i = 0; i < numProj; i++)
                            {
                                Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                Vector2 randomVelocityVector = new Vector2((Main.rand.NextFloat() - 0.5f) * 4f, (Main.rand.NextFloat() - 0.5f) * 4f);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(perturbedSpeed.X, -projectileVelocity) + randomVelocityVector, type, MushroomShotDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                            }
                        }
                    }

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= (phase2 ? 4f : 3f))
                    {
                        if (revenge && (!phase2 || (phase3 && death)))
                        {
                            SoundEngine.PlaySound(SoundID.Item42, NPC.Center);
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float projectileVelocity = CalamityWorld.death ? 15f : 10f;
                                Vector2 destination = new Vector2(NPC.Center.X, NPC.Center.Y - 100f) - NPC.Center;
                                destination.Normalize();
                                destination *= projectileVelocity;

                                // Less mushrooms in Death Mode phase 3 because otherwise it's an absolute shitshow.
                                int numProj = phase4 ? 8 : (phase3 && death) ? 6 : 8;
                                float rotation = MathHelper.ToRadians(60);
                                for (int i = 0; i < numProj; i++)
                                {
                                    Vector2 perturbedSpeed = destination.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                    Vector2 randomVelocityVector = new Vector2((Main.rand.NextFloat() - 0.5f) * 4f, (Main.rand.NextFloat() - 0.5f) * 4f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(perturbedSpeed.X, -projectileVelocity) + randomVelocityVector, type, MushroomShotDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                                }
                            }
                        }

                        NPC.ai[0] = 0f;
                        NPC.ai[2] = 0f;

                        if (revenge)
                            NPC.ai[3] = 0f;

                        NPC.netUpdate = true;
                    }
                    else
                    {
                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;

                        NPC.ai[0] = 2f;

                        if (revenge)
                            NPC.ai[3] += 1f;

                        NPC.netUpdate = true;
                    }

                    for (int j = (int)NPC.position.X - 20; j < (int)NPC.position.X + NPC.width + 40; j += 20)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y + NPC.height), NPC.width + 20, 4, DustID.BlueFairy, 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }

                        // Destroy tiles with stomps in Zenith seed
                        if (Main.zenithWorld)
                        {
                            int x = j / 16;
                            int y = (int)(NPC.position.Y + NPC.height) / 16;
                            Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                            Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                            if (!walkTile.HasTile && walkTile.LiquidAmount == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                            {
                                walkTile.TileFrameY = 0;
                                walkTile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                                walkTile.Get<TileWallWireStateData>().IsHalfBlock = false;
                                if (groundTile.TileType == TileID.MushroomGrass || groundTile.TileType == TileID.Mud)
                                {
                                    walkTile.Get<TileWallWireStateData>().HasTile = true;
                                    walkTile.TileType = TileID.MushroomPlants;
                                    walkTile.TileFrameX = (short)(Main.rand.Next(5) * 18);

                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendTileSquare(-1, x, y - 1, 1, TileChangeType.None);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (!player.dead && expertMode)
                    {
                        if ((player.position.Y > NPC.Bottom.Y && NPC.velocity.Y > 0f) || (player.position.Y < NPC.Bottom.Y && NPC.velocity.Y < 0f))
                            NPC.noTileCollide = true;
                        else if ((NPC.velocity.Y > 0f && NPC.Bottom.Y > Main.player[NPC.target].Top.Y) || (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height)))
                            NPC.noTileCollide = false;
                    }

                    if (NPC.position.X < player.position.X && NPC.position.X + NPC.width > player.position.X + player.width)
                    {
                        float slowDownMultiplier = death ? 0.9f : expertMode ? 0.93f : 0.96f;
                        NPC.velocity.X *= slowDownMultiplier;

                        float fallSpeedIncrease = phase4 ? 0.2f : death ? 0.15f : expertMode ? 0.12f : 0.09f;
                        NPC.velocity.Y += fallSpeedIncrease;
                    }
                    else
                    {
                        float velocityX = death ? 0.15f : expertMode ? 0.125f : 0.1f;
                        if (NPC.direction < 0)
                            NPC.velocity.X -= velocityX;
                        else if (NPC.direction > 0)
                            NPC.velocity.X += velocityX;

                        float maxVelocityXIncrease = death ? 4f : 3f;
                        float maxVelocityX = 6f + (expertMode ? (maxVelocityXIncrease * (1f - lifeRatio)) : 0f);

                        if (revenge)
                        {
                            float velocityXAdjustment = maxVelocityX;

                            if (lifeRatio < 0.5f && death)
                            {
                                // Mega velocity
                                switch ((int)NPC.ai[3])
                                {
                                    case 0: // Normal
                                    case 1: // High
                                    case 2: // Low
                                        maxVelocityX += velocityXAdjustment * 0.5f;
                                        break;

                                    case 3: // Long and low
                                        maxVelocityX += velocityXAdjustment * 1.5f;
                                        break;

                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                switch ((int)NPC.ai[3])
                                {
                                    case 0: // Normal
                                    case 1: // High
                                    case 2: // Low
                                        break;

                                    case 3: // Long and low
                                        maxVelocityX += velocityXAdjustment;
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }

                        if (Math.Abs(NPC.velocity.X) > maxVelocityX)
                            NPC.velocity.X = NPC.velocity.X < 0f ? -maxVelocityX : maxVelocityX;
                    }
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                if (NPC.velocity.Y == 0f || NPC.ai[2] == 1f)
                    NPC.ai[1] += 1f;

                if (NPC.ai[1] >= DelayBeforeBigJump)
                {
                    if (NPC.ai[1] == DelayBeforeBigJump && NPC.ai[2] == 0f)
                    {
                        Vector2 center = NPC.Center;
                        if (!player.dead && player.active && Math.Abs(NPC.Center.X - player.Center.X) / 16f <= despawnDistanceInTiles)
                            center = player.Center;

                        center.Y -= 320f + Math.Abs(player.Center.Y - NPC.Center.Y);
                        center.X += Math.Abs(player.Center.X - NPC.Center.X) * ((player.Center.X > NPC.Center.X) ? 1 : -1);

                        NPC.ai[2] = 1f;
                        NPC.ai[3] = NPC.Bottom.Y;
                        NPC.noTileCollide = true;

                        float leapVelocity = death ? 18f : 16f;
                        NPC.velocity = center - NPC.Center;
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.Zero);
                        NPC.velocity *= leapVelocity;
                        NPC.velocity.X *= 0.6f;

                        float velocityMinY = -leapVelocity;
                        if (NPC.velocity.Y > velocityMinY)
                            NPC.velocity.Y = velocityMinY;

                        NPC.ForceNetUpdate();
                    }
                    else
                    {
                        float mushroomFireRate = 15f;
                        if (NPC.ai[1] % mushroomFireRate == 0f)
                        {
                            SoundEngine.PlaySound(SoundID.Item42, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = ModContent.ProjectileType<MushBomb>();
                                float yVelocity = death ? 3f : 2f;
                                if (death)
                                {
                                    int numProj = 3;
                                    Vector2 initialVelocity = (NPC.Center + Vector2.UnitY * 10f - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    float rotation = MathHelper.ToRadians(8);
                                    for (int i = 0; i < numProj; i++)
                                    {
                                        Vector2 perturbedSpeed = initialVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, perturbedSpeed, type, MushroomShotDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                                    }
                                }
                                else
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.UnitY * yVelocity, type, MushroomShotDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                            }
                        }

                        // Collide with tiles if falling and beyond a certain threshold
                        if (NPC.velocity.Y >= 0f && NPC.Bottom.Y >= NPC.ai[3] - NPC.height)
                            NPC.noTileCollide = false;

                        // Impact and create lines of mushrooms that spread out along the ground (similar to an old Providence attack)
                        if (NPC.Bottom.Y >= NPC.ai[3] || NPC.velocity.Y == 0f)
                        {
                            SoundEngine.PlaySound(SlamSound, NPC.Center);

                            NPC.ai[0] = 0f;
                            NPC.ai[1] = 0f;
                            NPC.ai[2] = 0f;
                            NPC.ai[3] = 0f;
                            NPC.netUpdate = true;

                            int type = ModContent.ProjectileType<MushBombGround>();

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                float xVelocity = death ? 2f : 1f;
                                int numProj = death ? 5 : 3;
                                Vector2 initialVelocity = Vector2.UnitX * xVelocity;
                                Vector2 initialSpawnLocation = NPC.Bottom - new Vector2(0f, 8f);

                                for (int i = 0; i < numProj; i++)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), initialSpawnLocation + new Vector2(Main.rand.Next(0, 81), Main.rand.Next(-20, 1)), initialVelocity - (i / (float)numProj * initialVelocity), type, MushroomShotDamage, 0f, Main.myPlayer);

                                for (int i = 0; i < numProj; i++)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), initialSpawnLocation + new Vector2(Main.rand.Next(-81, 0), Main.rand.Next(-20, 1)), -(initialVelocity - (i / (float)numProj * initialVelocity)), type, MushroomShotDamage, 0f, Main.myPlayer);
                            }

                            for (int j = (int)NPC.position.X - 20; j < (int)NPC.position.X + NPC.width + 40; j += 20)
                            {
                                for (int k = 0; k < 4; k++)
                                {
                                    int stompDust = Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y + NPC.height), NPC.width + 20, 4, DustID.BlueFairy, 0f, 0f, 100, default, 1.5f);
                                    Main.dust[stompDust].velocity *= 0.2f;
                                }

                                // Destroy tiles with stomps in Zenith seed
                                if (Main.zenithWorld)
                                {
                                    int x = j / 16;
                                    int y = (int)(NPC.position.Y + NPC.height) / 16;
                                    Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                                    Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                                    if (!walkTile.HasTile && walkTile.LiquidAmount == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                                    {
                                        walkTile.TileFrameY = 0;
                                        walkTile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                                        walkTile.Get<TileWallWireStateData>().IsHalfBlock = false;
                                        if (groundTile.TileType == TileID.MushroomGrass || groundTile.TileType == TileID.Mud)
                                        {
                                            walkTile.Get<TileWallWireStateData>().HasTile = true;
                                            walkTile.TileType = TileID.MushroomPlants;
                                            walkTile.TileFrameX = (short)(Main.rand.Next(5) * 18);

                                            if (Main.netMode == NetmodeID.MultiplayerClient)
                                                NetMessage.SendTileSquare(-1, x, y - 1, 1, TileChangeType.None);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                    NPC.velocity.X *= 0.8f;
            }

            // Stomp three times quickly, creating lines of mushrooms and a column of mushrooms that slowly fall down
            else if (NPC.ai[0] == 5f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.noTileCollide = false;
                if (NPC.velocity.Y == 0f)
                {
                    NPC.velocity.X *= 0.8f;
                    NPC.ai[1] += 1f;
                    if (NPC.ai[1] % 15f == 14f)
                        NPC.netUpdate = true;

                    float jumpGateValue = 10f;
                    if (NPC.ai[1] >= jumpGateValue)
                    {
                        NPC.ai[1] = -20f;
                    }
                    else if (NPC.ai[1] == -1f)
                    {
                        NPC.velocity.Y = -(2f + NPC.ai[2]);

                        SoundEngine.PlaySound(JumpSound, NPC.Center);

                        NPC.ai[0] = 6f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }
            }
            else if (NPC.ai[0] == 6f)
            {
                if (NPC.velocity.Y == 0f)
                {
                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    SoundEngine.PlaySound(SlamSound, NPC.Center);

                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= 3f)
                    {
                        // Columns of mushrooms
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<MushBombFall>();
                            int maxColumns = 5;
                            int mushroomsPerColumn = 8;
                            Vector2 initialSpawnLocation = NPC.Bottom - new Vector2(210f, 8f);
                            Vector2 initialVelocity = Vector2.UnitY * 24f;
                            for (int i = 0; i < maxColumns; i++)
                            {
                                // 0 = 0.5, 1 = 0.25, 2 = 0, 3 = 0.25, 4 = 0.5
                                initialVelocity -= Vector2.UnitY * 8f * Math.Abs(0.5f - (i / (float)(maxColumns - 1)));

                                for (int j = 0; j < mushroomsPerColumn; j++)
                                {
                                    initialVelocity += Vector2.UnitX * Main.rand.NextFloat(-0.5f, 0.5f);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), initialSpawnLocation + new Vector2(70f * (i + 1), 0f), -(initialVelocity - ((j / (float)mushroomsPerColumn) * initialVelocity)), type, MushroomShotDamage, 0f, Main.myPlayer, 1f, NPC.Bottom.Y - 16f);
                                }

                                initialVelocity = Vector2.UnitY * 16f;
                            }
                        }

                        NPC.ai[0] = 1f;
                        NPC.ai[2] = 0f;
                        NPC.ai[3] = 0f;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        // Mushrooms along the ground
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<MushBombGround>();
                            float xVelocity = death ? 3f : 1.5f;
                            int numProj = 3;
                            Vector2 initialVelocity = Vector2.UnitX * xVelocity;
                            Vector2 initialSpawnLocation = NPC.Bottom - new Vector2(0f, 8f);

                            for (int i = 0; i < numProj; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), initialSpawnLocation + new Vector2(Main.rand.Next(0, 41), 0f), initialVelocity - ((i / (float)numProj) * initialVelocity), type, MushroomShotDamage, 0f, Main.myPlayer);

                            for (int i = 0; i < numProj; i++)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), initialSpawnLocation - new Vector2(Main.rand.Next(0, 41), 0f), -(initialVelocity - ((i / (float)numProj) * initialVelocity)), type, MushroomShotDamage, 0f, Main.myPlayer);
                        }

                        float playerLocation = NPC.Center.X - player.Center.X;
                        NPC.direction = playerLocation < 0 ? 1 : -1;

                        NPC.ai[0] = 5f;
                        NPC.netUpdate = true;
                    }

                    for (int j = (int)NPC.position.X - 20; j < (int)NPC.position.X + NPC.width + 40; j += 20)
                    {
                        for (int k = 0; k < 4; k++)
                        {
                            int stompDust = Dust.NewDust(new Vector2(NPC.position.X - 20f, NPC.position.Y + NPC.height), NPC.width + 20, 4, DustID.BlueFairy, 0f, 0f, 100, default, 1.5f);
                            Main.dust[stompDust].velocity *= 0.2f;
                        }

                        // Destroy tiles with stomps in Zenith seed (this is going to be so awful with this attack lol)
                        if (Main.zenithWorld)
                        {
                            int x = j / 16;
                            int y = (int)(NPC.position.Y + NPC.height) / 16;
                            Tile groundTile = CalamityUtils.ParanoidTileRetrieval(x, y);
                            Tile walkTile = CalamityUtils.ParanoidTileRetrieval(x, y - 1);
                            if (!walkTile.HasTile && walkTile.LiquidAmount == 0 && groundTile != null && WorldGen.SolidTile(groundTile))
                            {
                                walkTile.TileFrameY = 0;
                                walkTile.Get<TileWallWireStateData>().Slope = SlopeType.Solid;
                                walkTile.Get<TileWallWireStateData>().IsHalfBlock = false;
                                if (groundTile.TileType == TileID.MushroomGrass || groundTile.TileType == TileID.Mud)
                                {
                                    walkTile.Get<TileWallWireStateData>().HasTile = true;
                                    walkTile.TileType = TileID.MushroomPlants;
                                    walkTile.TileFrameX = (short)(Main.rand.Next(5) * 18);

                                    if (Main.netMode == NetmodeID.MultiplayerClient)
                                        NetMessage.SendTileSquare(-1, x, y - 1, 1, TileChangeType.None);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (!player.dead)
                    {
                        if ((player.position.Y > NPC.Bottom.Y && NPC.velocity.Y > 0f) || (player.position.Y < NPC.Bottom.Y && NPC.velocity.Y < 0f))
                            NPC.noTileCollide = true;
                        else if ((NPC.velocity.Y > 0f && NPC.Bottom.Y > Main.player[NPC.target].Top.Y) || (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].Center, 1, 1) && !Collision.SolidCollision(NPC.position, NPC.width, NPC.height)))
                            NPC.noTileCollide = false;
                    }
                }
            }

            if (NPC.localAI[0] == 0f && NPC.life > 0)
                NPC.localAI[0] = NPC.lifeMax;

            if (NPC.life > 0)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int crabShroomSpawnFreq = (int)(NPC.lifeMax * (Main.getGoodWorld ? 0.02 : 0.05));
                    if ((NPC.life + crabShroomSpawnFreq) < NPC.localAI[0])
                    {
                        NPC.localAI[0] = NPC.life;
                        int crabShroomAmt = death ? 4 : expertMode ? 3 : 2;
                        for (int mush = 0; mush < crabShroomAmt; mush++)
                        {
                            int x = (int)(NPC.position.X + Main.rand.Next(NPC.width - 32));
                            int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height - 32));
                            int npcType = ModContent.NPCType<CrabShroom>();
                            int crabShroom = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, npcType);
                            Main.npc[crabShroom].SetDefaults(npcType);
                            Main.npc[crabShroom].velocity.X = Main.rand.Next(-50, 51) * (Main.getGoodWorld ? 0.2f : 0.1f);
                            Main.npc[crabShroom].velocity.Y = Main.rand.Next(-50, -31) * (Main.getGoodWorld ? 0.2f : 0.1f);
                            if (Main.dedServ && crabShroom < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, crabShroom);
                        }
                    }
                }
            }
        }
        public override bool? CanFallThroughPlatforms() => NPC.target >= 0 && Main.player[NPC.target].position.Y > NPC.position.Y + NPC.height;

        // Can only hit the target if within certain distance
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Vector2 npcCenter = NPC.Center;

            // NOTE: Right and left hitboxes are interchangeable, each hitbox is the same size and is located to the right or left of the center hitbox.
            Rectangle leftHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 2f) + 6f * NPC.scale), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);
            Rectangle bodyHitbox = new Rectangle((int)(npcCenter.X - (NPC.width / 4f)), (int)(npcCenter.Y - (NPC.height / 2f)), NPC.width / 2, NPC.height);
            Rectangle rightHitbox = new Rectangle((int)(npcCenter.X + (NPC.width / 4f) - 6f * NPC.scale), (int)(npcCenter.Y - (NPC.height / 4f)), NPC.width / 4, NPC.height / 2);

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

            bool insideLeftHitbox = minLeftDist <= 45f * NPC.scale;

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

            bool insideBodyHitbox = minBodyDist <= 90f * NPC.scale;

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

            bool insideRightHitbox = minRightDist <= 45f * NPC.scale;

            return insideLeftHitbox || insideBodyHitbox || insideRightHitbox;
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.ai[0] > 1f)
            {
                if (NPC.velocity.Y == 0f && ((NPC.ai[1] >= 0f && (NPC.ai[0] == 2f || NPC.ai[0] == 5f)) || (NPC.ai[1] < (DelayBeforeBigJump - TelegraphTimeBeforeBigJump) && NPC.ai[0] == 4f && NPC.ai[2] == 0f))) // Idle just before jump
                {
                    if (stomping)
                        stomping = false;

                    NPC.frameCounter += 0.15;
                    NPC.frameCounter %= Main.npcFrameCount[Type];
                    int frame = (int)NPC.frameCounter;
                    NPC.frame.Y = frame * frameHeight;
                }
                else if (NPC.velocity.Y <= 0f || (NPC.ai[1] < DelayBeforeBigJump && NPC.ai[0] == 4f && NPC.ai[2] == 0f) || NPC.ai[1] < 0f) // Prepare to jump and then jump
                {
                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 12D)
                    {
                        NPC.frame.Y += frameHeight;
                        NPC.frameCounter = 0D;
                    }
                    if (NPC.frame.Y >= frameHeight)
                        NPC.frame.Y = frameHeight;
                }
                else // Stomping
                {
                    if (!stomping)
                    {
                        stomping = true;
                        NPC.frameCounter = 0D;
                    }

                    NPC.frameCounter += 1D;
                    if (NPC.frameCounter > 8D)
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

                NPC.frameCounter += 0.15;
                NPC.frameCounter %= Main.npcFrameCount[Type];
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
        }

        public override Color? GetAlpha(Color drawColor) => Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB, drawColor.A) * NPC.Opacity : null;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D textureIdle = TextureAssets.Npc[Type].Value;
            Texture2D glowIdle = Texture_Glow.Value;
            Texture2D textureWalk = AltTexture.Value;
            Texture2D glowWalk = AltTexture_Glow.Value;
            Texture2D textureAttack = AttackTexture.Value;
            Texture2D glowAttack = AttackTexture_Glow.Value;
            Color colorToShift = Main.zenithWorld ? new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB) : Color.Cyan;
            Color glowColor = Color.Lerp(Color.White, colorToShift, 0.5f);

            int ClonesOnEachSide = Main.zenithWorld ? 2 : 0;
            for (int c = 0 - ClonesOnEachSide; c < 1 + ClonesOnEachSide; c++)
            {
                Vector2 drawOrigin = new Vector2(textureIdle.Width / 2, textureIdle.Height / Main.npcFrameCount[Type] / 2);
                Vector2 drawPos = NPC.Center - screenPos + (Vector2.UnitX * textureIdle.Width * c * 1.6f);

                // Jumping
                if ((NPC.ai[0] > 2f && NPC.ai[0] != 5f) && NPC.velocity.Y != 0f)
                {
                    drawOrigin = new Vector2(textureAttack.Width / 2, textureAttack.Height / 2);
                    drawPos -= new Vector2(textureAttack.Width, textureAttack.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                    spriteBatch.Draw(textureAttack, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                    spriteBatch.Draw(glowAttack, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }

                // Walking
                else if (NPC.ai[0] == 1f)
                {
                    drawOrigin = new Vector2(textureWalk.Width / 2, textureWalk.Height / 2);
                    drawPos -= new Vector2(textureWalk.Width, textureWalk.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                    spriteBatch.Draw(textureWalk, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                    spriteBatch.Draw(glowWalk, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }

                // Standing still
                else
                {
                    drawPos -= new Vector2(textureIdle.Width, textureIdle.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    drawPos += drawOrigin * NPC.scale + new Vector2(0f, NPC.gfxOffY);

                    spriteBatch.Draw(textureIdle, drawPos, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                    spriteBatch.Draw(glowIdle, drawPos, NPC.frame, glowColor, NPC.rotation, drawOrigin, NPC.scale, spriteEffects, 0f);
                }
            }
            return false;
        }

        // GFB removes map icon, hover text, and health bar
        public override void BossHeadSlot(ref int index)
        {
            if (Main.zenithWorld)
                index = -1;
        }

        public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
        {
            if (Main.zenithWorld)
                boundingBox = Rectangle.Empty;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => Main.zenithWorld ? false : base.DrawHealthBar(hbPosition, ref scale, ref position);

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CrabulonBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<MycelialClaws>(),
                    ModContent.ItemType<Fungicide>(),
                    ModContent.ItemType<HyphaeRod>(),
                    ModContent.ItemType<Mycoroot>(),
                    ModContent.ItemType<InfestedClawmerang>(),
                    ModContent.ItemType<PuffShroom>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<FungalClump>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<CrabulonMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<CrabulonTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CrabulonRelic>());

            // GFB Odd Mushroom drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<OddMushroom>(), 1, 1, 9999), true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCrabulon, ModContent.ItemType<LoreCrabulon>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            // Start the Goblin Invasion if the player hasn't gotten one yet (this also gives players more of a reason to fight this boss)
            if (!NPC.downedGoblins && Main.netMode != NetmodeID.MultiplayerClient && !Main.snowMoon && !Main.pumpkinMoon && !DD2Event.Ongoing && !Main.ShouldNormalEventsBeAbleToStart() && Main.invasionType != 1)
                Main.StartInvasion();

            // Mark Crabulon as dead
            DownedBossSystem.downedCrabulon = true;
            CalamityNetcode.SyncWorld();

            if (Main.zenithWorld && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < Main.rand.Next(10, 23); i++)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-NPC.width / 2, NPC.width / 2), (int)NPC.Center.Y + Main.rand.Next(-NPC.height / 2, NPC.height / 2), NPCID.Crab);
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueFairy, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = (int)(200 * NPC.scale);
                NPC.height = (int)(100 * NPC.scale);
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int j = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueFairy, 0f, 0f, 100, default, 2f);
                    Main.dust[j].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[j].scale = 0.5f;
                        Main.dust[j].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int k = 0; k < 70; k++)
                {
                    int stompDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueFairy, 0f, 0f, 100, default, 3f);
                    Main.dust[stompDust].noGravity = true;
                    Main.dust[stompDust].velocity *= 5f;
                    stompDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.BlueFairy, 0f, 0f, 100, default, 2f);
                    Main.dust[stompDust].velocity *= 2f;
                }
                if (!Main.dedServ)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon3").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon4").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon5").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon6").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("Crabulon7").Type, NPC.scale);
                }
            }
        }
    }
}
