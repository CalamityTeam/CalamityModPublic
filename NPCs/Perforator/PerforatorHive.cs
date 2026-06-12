using System;
using System.IO;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Particles;
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

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHive : ModNPC
    {
        public static readonly SoundStyle GeyserShoot = new("CalamityMod/Sounds/Custom/Perforator/PerfHiveShoot", 3);
        public static readonly SoundStyle IchorShoot = new("CalamityMod/Sounds/Custom/Perforator/PerfHiveIchorShoot");
        public static readonly SoundStyle WormSpawn = new("CalamityMod/Sounds/Custom/Perforator/PerfHiveWormSpawn");
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/PerfHiveHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/PerfHiveDeath");

        public static Asset<Texture2D> GlowTexture;
        // Squash n' stretch won't affect actual hitbox size
        private const int Width = 110; 
        private const int Height = 100;

        private bool smallSpawned = false;
        private bool mediumSpawned = false;
        private bool largeSpawned = false;
        private int wormsAlive = 0;

        private float addedStretch;
        private int squashTimer = 0; // Tracks current progress in scaling animation
        private const int squashInterval = 24;
        private const float maxSquash = 0.3f; // Upper intensity
        private float wormSpawnStateTimer = 0f; // For managing worm spawn animation

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 10;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers();
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                GlowTexture = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
            }
        }

        public static int BloodGeyserDamage = 12; // 48
        public static int IchorShotDamage = 12; // 48
        public static int IchorBlobDamage = 12; // 48

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 18f;
            NPC.damage = 30; // 48 (1.6x expert scaling)
            NPC.width = Width;
            NPC.height = Height;
            NPC.defense = 4;
            NPC.LifeMaxNERB(4000, 5750, 270000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCrimson,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.PerforatorHive")
            });
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter += 0.15f;
            NPC.frameCounter %= Main.npcFrameCount[Type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(wormsAlive);
            writer.Write(smallSpawned);
            writer.Write(mediumSpawned);
            writer.Write(largeSpawned);
            writer.Write(NPC.localAI[2]);
            writer.Write(wormSpawnStateTimer); 
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            wormsAlive = reader.ReadInt32();
            smallSpawned = reader.ReadBoolean();
            mediumSpawned = reader.ReadBoolean();
            largeSpawned = reader.ReadBoolean();
            NPC.localAI[2] = reader.ReadSingle();
            wormSpawnStateTimer = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC.perfHive = NPC.whoAmI;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Variables for ichor blob phase
            float blobPhaseGateValue = 600f;
            bool floatAboveToFireBlobs = NPC.ai[2] >= blobPhaseGateValue - 120f;

            Player player = Main.player[NPC.target];

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.7f;

            bool spawnSmall = lifeRatio < 0.75f;
            bool spawnMedium = lifeRatio < 0.50f;
            bool spawnLarge = lifeRatio < 0.25f;

            if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f || !(player.ZoneCrimson || BossRushEvent.BossRushActive))
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f || !(player.ZoneCrimson || BossRushEvent.BossRushActive))
                {
                    NPC.rotation = NPC.velocity.X * 0.04f;

                    if (NPC.velocity.Y < -3f)
                        NPC.velocity.Y = -3f;
                    NPC.velocity.Y += 0.1f;
                    if (NPC.velocity.Y > 12f)
                        NPC.velocity.Y = 12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    return;
                }
            }

            // Natural despawn prevention
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            // GFB seed shenanigans: Behavior during the suck
            if (NPC.localAI[1] >= 6f)
            {
                // Leak projectiles everywhere and start healing
                int type = Main.rand.NextBool() ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                int damage = type == ModContent.ProjectileType<IchorShot>() ? IchorShotDamage : BloodGeyserDamage;
                int spread = Main.rand.Next(-45, 46);
                Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread)), type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);

                // Heals 10 times per second for 0.1% of its health each = 1% per second
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int healAmt = (int)(NPC.lifeMax / 1000);
                    if (healAmt > NPC.lifeMax - NPC.life)
                        healAmt = NPC.lifeMax - NPC.life;

                    if (healAmt > 0)
                    {
                        NPC.life += healAmt;
                        NPC.HealEffect(healAmt, true);
                        NPC.netUpdate = true;
                    }
                }
                NPC.localAI[1] = 0f;
            }

            bool largeWormAlive = NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>());
            bool mediumWormAlive = NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadMedium>());
            bool smallWormAlive = NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadSmall>());
            if (largeWormAlive && mediumWormAlive && smallWormAlive)
                wormsAlive = 3;
            else if ((largeWormAlive && mediumWormAlive) || (largeWormAlive && smallWormAlive) || (mediumWormAlive && smallWormAlive))
                wormsAlive = 2;
            else if (largeWormAlive || mediumWormAlive || smallWormAlive)
                wormsAlive = 1;
            else
                wormsAlive = 0;

            NPC.Calamity().DR = wormsAlive * 0.5f;
            if (wormsAlive >= 1)
                NPC.Calamity().CurrentlyIncreasingDefenseOrDR = true;
            if (NPC.Calamity().DR >= 0.999f)
            {
                NPC.Calamity().DR = 0.999f;
                NPC.Calamity().unbreakableDR = true;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient && wormSpawnStateTimer == 0f)
            {
                if ((!smallSpawned && spawnSmall) ||
                    (!mediumSpawned && spawnMedium) ||
                    (!largeSpawned && spawnLarge))
                {
                    wormSpawnStateTimer = 1f; // Start slowing down to spawn a worm
                    NPC.netUpdate = true;
                }
            }

            if (wormSpawnStateTimer > 0f)
            {    

                NPC.velocity *= 0.94f; // Slow down
                NPC.rotation = NPC.velocity.X * 0.04f; // Update rotation to match velocity

                NPC.damage = 0;

                wormSpawnStateTimer++;

                int slowDownDuration = 20;
                int waitBeforeSpawnDuration = 40; 
                int totalStateDuration = slowDownDuration + waitBeforeSpawnDuration;

                if (wormSpawnStateTimer >= slowDownDuration && wormSpawnStateTimer < totalStateDuration)
                {
                    if (Main.rand.NextBool(7))
                    {
                        int bloodLifetime = Main.rand.Next(20, 45);
                        float bloodScale = Main.rand.NextFloat(0.5f, 1f);
                        Color bloodColor = Color.Lerp(Color.Yellow, Color.DarkRed, Main.rand.NextFloat(0.7f));
                        float randomSpeedMultiplier = Main.rand.NextFloat(0.8f, 1.6f);
                        Vector2 bloodVelocity = Main.rand.NextVector2Unit(5) * 1.5f * randomSpeedMultiplier;
                        bloodVelocity.Y -= 8f;

                        Vector2 randomOffset = Main.rand.NextVector2Unit() * Main.rand.NextFloat(25f, 50f);
                        Vector2 spawnPosition = NPC.Center + randomOffset;

                        BloodParticle blood = new BloodParticle(spawnPosition, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                        GeneralParticleHandler.SpawnParticle(blood);
                    }
                }

                else if (wormSpawnStateTimer >= totalStateDuration)
                {
                    // Blood vars change based on worm spawned
                    Color bloodColor = Color.Lerp(Color.Crimson, Color.DarkRed, Main.rand.NextFloat(0.9f)); 
                    int bloodAmt = 0;
                    float minScale = 1f;
                    float maxScale = 1.8f;
                    int wormType = -1;

                    if (!smallSpawned)
                    {

                        smallSpawned = true;
                        wormType = ModContent.NPCType<PerforatorHeadSmall>();

                        bloodAmt = 12;
                    }
                    else if (!mediumSpawned && spawnMedium)
                    {
                        mediumSpawned = true;
                        wormType = ModContent.NPCType<PerforatorHeadMedium>();

                        bloodColor = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.9f));
                        bloodAmt = 12;
                        minScale = 1.5f;
                        maxScale = 2.4f;
                    }
                    else if (!largeSpawned && spawnLarge)
                    {
                        largeSpawned = true;
                        wormType = ModContent.NPCType<PerforatorHeadLarge>();

                        bloodAmt = 18;
                        minScale = 1.4f;
                        maxScale = 2.2f;
                    }

                    for (int i = 0; i < bloodAmt; ++i)
                    {
                        int bloodLifetime = Main.rand.Next(80, 140);
                        float bloodScale = Main.rand.NextFloat(minScale, maxScale);

                        float randomSpeedMultiplier = Main.rand.NextFloat(1.4f, 2.2f);
                        Vector2 bloodVelocity = Main.rand.NextVector2Unit() * 4 * randomSpeedMultiplier;
                        bloodVelocity.Y -= 5f;
                        BloodParticle blood = new BloodParticle(NPC.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                        GeneralParticleHandler.SpawnParticle(blood);
                    }

                    if (wormType != -1)
                    {
                        squashTimer = squashInterval; // Start scaling animation

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-25, 26), (int)NPC.Center.Y + Main.rand.Next(-25, 26), wormType, 1);

                            // Spawn two small worms in Death
                            if (death)
                            {
                                if (wormType == ModContent.NPCType<PerforatorHeadSmall>())
                                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X + Main.rand.Next(-25, 26), (int)NPC.Center.Y + Main.rand.Next(-25, 26), wormType, 1);
                            }
                        }
                    }

                    NPC.TargetClosest();
                    SoundEngine.PlaySound(WormSpawn, NPC.Center);
                    wormSpawnStateTimer = 0f; // Resets for next time its called
                    NPC.netUpdate = true;
                }
                return; // Don't progress in the AI loop until a worm has finished spawning
            }

            if (squashTimer > 0)
            {
                squashTimer--;
                addedStretch = MathHelper.Lerp(0f, maxSquash, (float) squashTimer / squashInterval);
            }
            else
            {
                addedStretch = 0f;
            }

            if (Math.Abs(NPC.Center.X - player.Center.X) > 10f)
            {
                float playerLocation = NPC.Center.X - player.Center.X;
                NPC.direction = playerLocation < 0f ? 1 : -1;
                NPC.spriteDirection = NPC.direction;
            }

            NPC.rotation = NPC.velocity.X * 0.04f;

            // Emit ichor blobs
            if (phase2)
            {
                if (wormsAlive == 0 || largeSpawned || floatAboveToFireBlobs || Main.getGoodWorld)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= blobPhaseGateValue)
                    {
                        if (NPC.ai[2] < blobPhaseGateValue + 300f)
                        {
                            if (NPC.velocity.Length() > 0.5f)
                                NPC.velocity *= 0.96f;
                            else
                                NPC.ai[2] = blobPhaseGateValue + 300f;

                        }

                        if (NPC.ai[2] < blobPhaseGateValue + 180f)
                        {
                            if (Main.rand.NextBool(4))
                            {
                                int bloodLifetime = Main.rand.Next(25, 35);
                                float bloodScale = Main.rand.NextFloat(0.6f, 0.95f);
                                Color bloodColor = Color.Lerp(Color.Yellow, Color.Orange, Main.rand.NextFloat(0.8f));
                                float randomSpeedMultiplier = Main.rand.NextFloat(1.5f, 2.5f);
                                Vector2 bloodVelocity = Main.rand.NextVector2Unit() * randomSpeedMultiplier;

                                Vector2 spawnPosition = NPC.Center;
                                spawnPosition.Y += 42;

                                BloodParticle blood = new BloodParticle(spawnPosition, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                                GeneralParticleHandler.SpawnParticle(blood);
                            }
                        }

                        else
                        {
                            NPC.ai[2] = 0f;

                            SoundEngine.PlaySound(IchorShoot, NPC.Center);

                            for (int i = 0; i < 32; i++)
                            {
                                int ichorDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Ichor);
                                float dustVelocityYAdd = Math.Abs(Main.dust[ichorDust].velocity.Y) * 0.5f;
                                if (Main.dust[ichorDust].velocity.Y < 0f)
                                    Main.dust[ichorDust].velocity.Y = 2f + dustVelocityYAdd;

                                if (Main.rand.NextBool())
                                    Main.dust[ichorDust].scale = 0.5f;
                            }

                            bool ichorBlobBigWormPhase = wormsAlive > 0 && largeSpawned;
                            int numBlobs = expertMode ? (ichorBlobBigWormPhase ? 4 : 6) : (ichorBlobBigWormPhase ? 2 : 4);
                            if (Main.getGoodWorld)
                                numBlobs *= 2;

                            int type = ModContent.ProjectileType<IchorBlob>();

                            int blobSpread = expertMode ? (ichorBlobBigWormPhase ? 66 : 100) : (ichorBlobBigWormPhase ? 33 : 66);
                            for (int i = 0; i < numBlobs; i++)
                            {
                                Vector2 blobVelocity = new Vector2(Main.rand.Next(-blobSpread, blobSpread + 1), Main.rand.Next(-blobSpread, blobSpread + 1));
                                blobVelocity.Normalize();
                                blobVelocity *= Main.rand.Next(400, 801) * 0.01f;

                                if (Main.getGoodWorld)
                                    blobVelocity *= Main.rand.NextFloat() + 1f;

                                float blobVelocityYAdd = Math.Abs(blobVelocity.Y) * 0.25f;
                                if (blobVelocity.Y < 2f)
                                    blobVelocity.Y = 2f + blobVelocityYAdd;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 50f, blobVelocity, type, IchorBlobDamage, 0f, Main.myPlayer, 0f, player.Center.Y);
                            }
                        }

                        return;
                    }
                }
            }

            // When firing blobs, float above the target and don't call any other projectile firing or movement code
            if (floatAboveToFireBlobs)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (revenge)
                    Movement(player, 9f, 0.3f, 450f);
                else
                    Movement(player, 6f, 0.2f, 450f);

                return;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= (revenge ? 200f : 250f) + wormsAlive * 150f && NPC.position.Y + NPC.height < player.position.Y && Vector2.Distance(player.Center, NPC.Center) > 80f)
                {
                    NPC.localAI[0] = 0f;
                    SoundEngine.PlaySound(GeyserShoot, NPC.Center);

                    int numProj = death ? 16 : revenge ? 14 : expertMode ? 12 : 10;
                    if (Main.getGoodWorld)
                        numProj *= 2;

                    int spread = 75;
                    float velocity = 8f;
                    Vector2 destination = wormsAlive > 0 ? player.Center : NPC.Center - Vector2.UnitY * 100f;
                    Vector2 projectileVelocity = new Vector2(Utils.DirectionTo(NPC.Center, destination).X * velocity, -velocity);
                    float rotation = MathHelper.ToRadians(spread);
                    Vector2 dustSpawnBox = new Vector2(12f, 12f);
                    Vector2 dustSpawnOffset = dustSpawnBox * 0.5f;
                    for (int i = 0; i < numProj; i++)
                    {
                        bool ichor = Main.rand.NextBool();
                        int type = ichor ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                        int damage = ichor ? IchorShotDamage : BloodGeyserDamage;

                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        Vector2 randomVelocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() - 0.5f);
                        Vector2 projectileSpawnLocation = NPC.Center + Vector2.Normalize(perturbedSpeed) * 50f;
                        Vector2 projectileVelocityRandomized = perturbedSpeed + randomVelocity;

                        float dustSpeed = Main.rand.NextFloat(3.0f, 9.0f);
                        float angleRandom = 0.05f;
                        Vector2 dustVelocity = new Vector2(dustSpeed, 0.0f).RotatedBy(projectileVelocityRandomized.ToRotation());
                        dustVelocity = dustVelocity.RotatedBy(-angleRandom);
                        dustVelocity = dustVelocity.RotatedByRandom(2.0f * angleRandom);

                        if (ichor)
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                int ichorDust = Dust.NewDust(projectileSpawnLocation - dustSpawnOffset, (int)dustSpawnBox.X, (int)dustSpawnBox.Y, DustID.Ichor);
                                Main.dust[ichorDust].velocity = dustVelocity;
                            }
                        }
                        else
                        {
                            for (int j = 0; j < 4; j++)
                            {
                                int bloodDust = Dust.NewDust(projectileSpawnLocation - dustSpawnOffset, (int)dustSpawnBox.X, (int)dustSpawnBox.Y, DustID.Blood);
                                Main.dust[bloodDust].velocity = dustVelocity;
                                Main.dust[bloodDust].scale = 2f;
                            }
                        }

                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawnLocation, projectileVelocityRandomized, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                    }
                }
            }

            if (revenge)
            {
                switch (wormsAlive)
                {
                    case 0:

                        // Set damage
                        NPC.damage = NPC.defDamage;

                        if (largeSpawned || death)
                            Movement(player, 13f, death ? 0.115f : 0.1f, 20f);
                        else if (mediumSpawned)
                            Movement(player, 12f, death ? 0.11f : 0.095f, 30f);
                        else if (smallSpawned)
                            Movement(player, 11f, death ? 0.105f : 0.09f, 40f);
                        else
                            Movement(player, 10f, death ? 0.1f : 0.085f, 50f);

                        break;

                    case 1:

                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        Movement(player, 8f, 0.2f, 350f);

                        break;

                    case 2:

                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        Movement(player, 8f, 0.2f, 275f);

                        break;

                    case 3:

                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        Movement(player, 8f, 0.2f, 200f);

                        break;
                }
            }
            else
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                Movement(player, 6f, 0.15f, 350f);
            }
        }

        private void Movement(Player target, float velocity, float acceleration, float y)
        {
            // Distance from destination where Perf Hive stops moving
            float movementDistanceGateValue = 100f;

            // This is where Perf Hive should be
            Vector2 destination = new Vector2(target.Center.X, target.Center.Y - y);

            // How far Perf Hive is from where it's supposed to be
            Vector2 distanceFromDestination = destination - NPC.Center;

            // Set the velocity
            CalamityUtils.SmoothMovement(NPC, movementDistanceGateValue, distanceFromDestination, velocity, acceleration, true);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Texture2D texture = TextureAssets.Npc[Type].Value;
            Color drawColorAlpha = NPC.GetAlpha(drawColor);

            Vector2 scaleStretch = new Vector2(1f - addedStretch, 1f + addedStretch) * NPC.scale;
            float yOffset = addedStretch * 0.5f * NPC.height; // offset from stretching

            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY - yOffset), NPC.frame, drawColorAlpha, NPC.rotation, NPC.frame.Size() * 0.5f, scaleStretch, spriteEffects, 0f);
            texture = GlowTexture.Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            // done again to fix glowmask not matching sprite
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY - yOffset), NPC.frame, glowmaskColor, NPC.rotation, NPC.frame.Size() * 0.5f, scaleStretch, spriteEffects, 0f);

            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!DownedBossSystem.downedHiveMind && !DownedBossSystem.downedPerforator)
            {
                string key = "Mods.CalamityMod.Status.Progression.SkyOreText";
                Color messageColor = Color.Cyan;
                AerialiteOreGen.Enchant();

                CalamityUtils.BroadcastLocalizedText(key, messageColor);
            }

            // Mark The Perforator Hive as dead
            DownedBossSystem.downedPerforator = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PerforatorBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons and such
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, new WeightedItemStack[]
                {
                    ModContent.ItemType<VeinBurster>(),
                    ModContent.ItemType<SausageMaker>(),
                    ModContent.ItemType<Aorta>(),
                    ModContent.ItemType<Eviscerator>(),
                    ModContent.ItemType<BloodBath>(),
                    ModContent.ItemType<FleshOfInfidelity>(),
                    ModContent.ItemType<ToothBall>(),
                }));

                // Materials
                normalOnly.Add(ItemID.CrimtaneBar, 1, 10, 15);
                normalOnly.Add(ItemID.Vertebrae, 1, 10, 15);
                normalOnly.Add(ItemID.CrimsonSeeds, 1, 10, 15);
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.Hardmode(), ItemID.Ichor, 1, 10, 20));

                // Equipment
                normalOnly.Add(ModContent.ItemType<BloodstainedGlove>(), DropHelper.NormalWeaponDropRateFraction);
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<BloodyWormTooth>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<PerforatorMask>(), 7);
                normalOnly.Add(ModContent.ItemType<BloodyVein>(), 10);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<PerforatorTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<PerforatorsRelic>());

            // GFB Bloodfin drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<Bloodfin>(), 1, 1, 9999), true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedPerforator, ModContent.ItemType<LorePerforators>(), desc: DropHelper.FirstKillText);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < hit.Damage / NPC.lifeMax * 100.0; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hive").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hive2").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hive3").Type, 1f);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("Hive4").Type, 1f);
                }
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 12; i++)
                {
                    int ichorDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[ichorDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[ichorDust].scale = 0.5f;
                        Main.dust[ichorDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 20; j++)
                {
                    int bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 3f);
                    Main.dust[bloodDust].noGravity = true;
                    Main.dust[bloodDust].velocity *= 5f;
                    bloodDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, 0f, 0f, 100, default, 2f);
                    Main.dust[bloodDust].velocity *= 2f;
                }
                // Blood burst
                for (int i = 0; i < 24; ++i)
                {
                    int bloodLifetime = Main.rand.Next(120, 200);
                    float bloodScale = Main.rand.NextFloat(1.3f, 2.6f);
                    Color bloodColor = Color.Lerp(Color.Crimson, Color.DarkRed, Main.rand.NextFloat(0.9f));
                    float randomSpeedMultiplier = Main.rand.NextFloat(4.5f, 9f);
                    Vector2 bloodVelocity = Main.rand.NextVector2Unit(6) * 2.5f * randomSpeedMultiplier;
                    bloodVelocity.Y -= 14f;

                    BloodParticle blood = new BloodParticle(NPC.Center, bloodVelocity, bloodLifetime, bloodScale, bloodColor);
                    GeneralParticleHandler.SpawnParticle(blood);
                }
            }
        }
    }
}
