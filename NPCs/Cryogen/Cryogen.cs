using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Effects;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Wings;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Ores;
using CalamityMod.UI.VanillaBossBars;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.Cryogen
{
    [AutoloadBossHead]
    [LongDistanceNetSync] // Cryogen follows you forever like Queen Bee in vanilla, So we need this to sync it's position on minimap
    public class Cryogen : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private int currentPhase = 1;
        private int teleportLocationX = 0;

        public static Color BackglowColor => new Color(24, 100, 255, 80) * 0.6f;

        public override string Texture => "CalamityMod/NPCs/Cryogen/Cryogen_Phase1";

        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/CryogenHit", 3);
        public static readonly SoundStyle TransitionSound = new("CalamityMod/Sounds/NPCHit/CryogenPhaseTransitionCrack");
        public static readonly SoundStyle ShieldRegenSound = new("CalamityMod/Sounds/Custom/CryogenShieldRegenerate");
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/CryogenDeath");

        public static Asset<Texture2D> Phase2Texture;
        public static Asset<Texture2D> Phase3Texture;
        public static Asset<Texture2D> Phase4Texture;
        public static Asset<Texture2D> Phase5Texture;
        public static Asset<Texture2D> Phase6Texture;

        public FireParticleSet FireDrawer = null;

        public static int cryoIconIndex;
        public static int pyroIconIndex;

        internal static void LoadHeadIcons()
        {
            string cryoIconPath = "CalamityMod/NPCs/Cryogen/Cryogen_Phase1_Head_Boss";
            string pyroIconPath = "CalamityMod/NPCs/Cryogen/Pyrogen_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(cryoIconPath, -1);
            cryoIconIndex = ModContent.GetModBossHeadSlot(cryoIconPath);

            CalamityMod.Instance.AddBossHeadTexture(pyroIconPath, -1);
            pyroIconIndex = ModContent.GetModBossHeadSlot(pyroIconPath);
        }

        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase2", AssetRequestMode.AsyncLoad);
                Phase3Texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase3", AssetRequestMode.AsyncLoad);
                Phase4Texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase4", AssetRequestMode.AsyncLoad);
                Phase5Texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase5", AssetRequestMode.AsyncLoad);
                Phase6Texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/Cryogen/Cryogen_Phase6", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 24f;
            NPC.GetNPCDamage();
            NPC.width = 86;
            NPC.height = 88;
            NPC.defense = 15;
            NPC.DR_NERD(0.3f);
            NPC.LifeMaxNERB(40000, 48000, 300000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 40, 0, 0);
            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<CryogenBossBar>();
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.coldDamage = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;

            if (Main.getGoodWorld)
                NPC.scale *= 0.8f;

            if (Main.zenithWorld)
            {
                NPC.Calamity().VulnerableToHeat = false;
                NPC.Calamity().VulnerableToCold = true;
                NPC.Calamity().VulnerableToWater = true;
            }
            else
            {
                NPC.Calamity().VulnerableToHeat = true;
                NPC.Calamity().VulnerableToCold = false;
                NPC.Calamity().VulnerableToSickness = false;
            }
        }

        public override void BossHeadSlot(ref int index)
        {
            if (Main.zenithWorld)
                index = pyroIconIndex;
            else
                index = cryoIconIndex;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Snow,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.Cryogen")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(teleportLocationX);
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            teleportLocationX = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            Lighting.AddLight((int)((NPC.position.X + (NPC.width / 2)) / 16f), (int)((NPC.position.Y + (NPC.height / 2)) / 16f), 0f, 1f, 1f);

            if (FireDrawer != null)
                FireDrawer.Update();

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Enrage
            if (!player.ZoneSnow && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = death ? 0.5f : 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 2f;
            }

            if (enrageScale > 2f)
                enrageScale = 2f;

            if (bossRush)
                enrageScale = 3f;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < (revenge ? 0.85f : 0.8f) || death;
            bool phase3 = lifeRatio < (death ? 0.8f : revenge ? 0.7f : 0.6f);
            bool phase4 = lifeRatio < (death ? 0.6f : revenge ? 0.55f : 0.4f);
            bool phase5 = lifeRatio < (death ? 0.5f : revenge ? 0.45f : 0.3f);
            bool phase6 = lifeRatio < (death ? 0.35f : 0.25f) && revenge;
            bool phase7 = lifeRatio < (death ? 0.25f : 0.15f) && revenge;

            // Projectile and sound variables
            int iceBlast = Main.zenithWorld ? ModContent.ProjectileType<BrimstoneBarrage>() : ModContent.ProjectileType<IceBlast>();
            int iceBomb = Main.zenithWorld ? ModContent.ProjectileType<SCalBrimstoneFireblast>() : ModContent.ProjectileType<IceBomb>();
            int iceRain = Main.zenithWorld ? ModContent.ProjectileType<BrimstoneBarrage>() : ModContent.ProjectileType<IceRain>();
            int dustType = Main.zenithWorld ? 235 : 67;

            SoundStyle frostSound = Main.zenithWorld ? SoundID.Item20 : SoundID.Item28;
            NPC.HitSound = Main.zenithWorld ? SoundID.NPCHit41 : HitSound;
            NPC.DeathSound = Main.zenithWorld ? SoundID.NPCDeath14 : DeathSound;

            if ((int)NPC.ai[0] + 1 > currentPhase)
                HandlePhaseTransition((int)NPC.ai[0] + 1);

            if (NPC.ai[2] == 0f && NPC.localAI[1] == 0f && Main.netMode != NetmodeID.MultiplayerClient && (NPC.ai[0] < 3f || bossRush || (death && NPC.ai[0] > 3f))) //spawn shield for phase 0 1 2, not 3 4 5
            {
                SoundEngine.PlaySound(ShieldRegenSound, NPC.Center);
                int shieldSpawn = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<CryogenShield>(), NPC.whoAmI);
                NPC.ai[2] = shieldSpawn + 1;
                NPC.localAI[1] = -1f;
                NPC.netUpdate = true;
                Main.npc[shieldSpawn].ai[0] = NPC.whoAmI;
                Main.npc[shieldSpawn].netUpdate = true;
            }

            int shieldTracker = (int)NPC.ai[2] - 1;
            if (shieldTracker != -1 && Main.npc[shieldTracker].active && Main.npc[shieldTracker].type == ModContent.NPCType<CryogenShield>())
            {
                NPC.dontTakeDamage = true;
            }
            else
            {
                NPC.dontTakeDamage = false;
                NPC.ai[2] = 0f;

                if (NPC.localAI[1] == -1f)
                    NPC.localAI[1] = death ? 540f : expertMode ? 720f : 1080f;
                if (NPC.localAI[1] > 0f)
                    NPC.localAI[1] -= 1f;
            }

            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();
            else if (!Main.raining)
                CalamityUtils.StartRain();

            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead)
                {
                    if (NPC.velocity.Y > 3f)
                        NPC.velocity.Y = 3f;
                    NPC.velocity.Y -= 0.1f;
                    if (NPC.velocity.Y < -12f)
                        NPC.velocity.Y = -12f;

                    if (NPC.timeLeft > 60)
                        NPC.timeLeft = 60;

                    if (NPC.ai[1] != 0f)
                    {
                        NPC.ai[1] = 0f;
                        teleportLocationX = 0;
                        calamityGlobalNPC.newAI[2] = 0f;
                        NPC.netUpdate = true;
                    }
                    return;
                }
            }
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
            {
                int spawnType = Main.zenithWorld ? NPCID.RedDevil : NPCID.IceGolem;
                if (!NPC.AnyNPCs(spawnType))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        int enemySpawnX = (int)(NPC.Center.X / 16f) + Main.rand.Next(-50, 51);
                        int enemySpawnY;
                        for (enemySpawnY = (int)(NPC.Center.Y / 16f) + Main.rand.Next(-50, 51); enemySpawnY < Main.maxTilesY - 10 && !WorldGen.SolidTile(enemySpawnX, enemySpawnY); enemySpawnY++)
                        {
                        }

                        enemySpawnY--;
                        if (!WorldGen.SolidTile(enemySpawnX, enemySpawnY))
                        {
                            int legendEnemySpawn = NPC.NewNPC(NPC.GetSource_FromAI(), enemySpawnX * 16 + 8, enemySpawnY * 16, spawnType);
                            if (Main.netMode == NetmodeID.Server && legendEnemySpawn < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, legendEnemySpawn);

                            break;
                        }
                    }
                }
            }

            float chargePhaseGateValue = bossRush ? 240f : 360f;
            float chargeDuration = 60f;
            float chargeTelegraphTime = NPC.ai[0] == 2f ? ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 60f : 80f) : ((CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 90f : 120f);
            float chargeTelegraphMaxRotationIncrement = 1f;
            float chargeTelegraphRotationIncrement = chargeTelegraphMaxRotationIncrement / chargeTelegraphTime;
            float chargeSlowDownTime = 15f;
            float chargeVelocityMin = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 24f : 12f;
            float chargeVelocityMax = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? 42f : 30f;
            if (Main.getGoodWorld)
            {
                chargePhaseGateValue *= 0.7f;
                chargeDuration *= 0.8f;
            }
            float chargeGateValue = chargePhaseGateValue + chargeTelegraphTime;
            float chargeSlownDownPhaseGateValue = chargeGateValue + chargeSlowDownTime;
            bool chargePhase = NPC.ai[1] >= chargePhaseGateValue;

            if (expertMode && (NPC.ai[0] < 5f || !phase6) && !chargePhase)
            {
                calamityGlobalNPC.newAI[3] += 1f;
                if (calamityGlobalNPC.newAI[3] >= (bossRush ? 660f : 900f))
                {
                    calamityGlobalNPC.newAI[3] = 0f;
                    SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int totalProjectiles = 3;
                        float radians = MathHelper.TwoPi / totalProjectiles;
                        int type = iceBomb;
                        int damage = NPC.GetProjectileDamage(type);
                        float velocity = 2f + NPC.ai[0];
                        double angleA = radians * 0.5;
                        double angleB = MathHelper.ToRadians(90f) - angleA;
                        float velocityX = (float)(velocity * Math.Sin(angleA) / Math.Sin(angleB));
                        Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity) : new Vector2(-velocityX, -velocity);
                        for (int k = 0; k < totalProjectiles; k++)
                        {
                            Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer);
                        }
                    }
                }
            }

            if (NPC.ai[0] == 0f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.rotation = NPC.velocity.X * 0.1f;

                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= 120f)
                {
                    NPC.localAI[0] = 0f;
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                    {
                        SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int totalProjectiles = bossRush ? 24 : 16;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = iceBlast;
                            int damage = NPC.GetProjectileDamage(type);
                            float velocity = 9f + enrageScale;
                            float projectileVelocityToPass = 0f;
                            if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                projectileVelocityToPass = velocity * 2f;

                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                            }
                        }
                    }
                }

                Vector2 cryogenCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float playerXDist = player.Center.X - cryogenCenter.X;
                float playerYDist = player.Center.Y - cryogenCenter.Y;
                float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);

                float cryogenSpeed = revenge ? 5f : 4f;
                cryogenSpeed += 4f * enrageScale;

                playerDistance = cryogenSpeed / playerDistance;
                playerXDist *= playerDistance;
                playerYDist *= playerDistance;

                float inertia = 50f;
                if (Main.getGoodWorld)
                    inertia *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia + playerXDist) / (inertia + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia + playerYDist) / (inertia + 1f);

                if (phase2)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 1f;
                    NPC.localAI[0] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 1f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.ai[1] < chargePhaseGateValue)
                {
                    NPC.ai[1] += 1f;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 120f)
                    {
                        NPC.localAI[0] = 0f;
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int totalProjectiles = bossRush ? 18 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = iceBlast;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity2 = 9f + enrageScale;
                                float projectileVelocityToPass = 0f;
                                if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                    projectileVelocityToPass = velocity2 * 2f;

                                Vector2 spinningPoint = new Vector2(0f, -velocity2);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                                }
                            }
                        }
                    }

                    float velocity = revenge ? 3.5f : 4f;
                    float acceleration = 0.15f;
                    velocity -= enrageScale * 0.8f;
                    acceleration += 0.07f * enrageScale;

                    if (NPC.position.Y > player.position.Y - 375f)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y *= 0.98f;

                        NPC.velocity.Y -= acceleration;

                        if (NPC.velocity.Y > velocity)
                            NPC.velocity.Y = velocity;
                    }
                    else if (NPC.position.Y < player.position.Y - 425f)
                    {
                        if (NPC.velocity.Y < 0f)
                            NPC.velocity.Y *= 0.98f;

                        NPC.velocity.Y += acceleration;

                        if (NPC.velocity.Y < -velocity)
                            NPC.velocity.Y = -velocity;
                    }

                    if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + 300f)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X *= 0.98f;

                        NPC.velocity.X -= acceleration;

                        if (NPC.velocity.X > velocity)
                            NPC.velocity.X = velocity;
                    }
                    if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - 300f)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X *= 0.98f;

                        NPC.velocity.X += acceleration;

                        if (NPC.velocity.X < -velocity)
                            NPC.velocity.X = -velocity;
                    }
                }
                else if (NPC.ai[1] < chargeGateValue)
                {
                    NPC.ai[1] += 1f;

                    float totalSpreads = 3f;
                    if ((NPC.ai[1] - chargePhaseGateValue) % (chargeTelegraphTime / totalSpreads) == 0f)
                    {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = iceRain;
                                int damage = NPC.GetProjectileDamage(type);
                                float maxVelocity = 9f + enrageScale;
                                float velocity = maxVelocity - (calamityGlobalNPC.newAI[0] * maxVelocity * 0.5f);
                                int totalProjectiles = 10;
                                int maxTotalProjectileReductionBasedOnRotationSpeed = (int)(totalProjectiles * 0.7f);
                                int totalProjectilesShot = totalProjectiles - (int)Math.Round(calamityGlobalNPC.newAI[0] * maxTotalProjectileReductionBasedOnRotationSpeed);
                                for (int i = 0; i < 2; i++)
                                {
                                    float radians = MathHelper.TwoPi / totalProjectilesShot;
                                    float newVelocity = velocity - (velocity * 0.5f * i);
                                    float projectileVelocityToPass = 0f;
                                    if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                        projectileVelocityToPass = velocity * 2f;

                                    double angleA = radians * 0.5;
                                    double angleB = MathHelper.ToRadians(90f) - angleA;
                                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectilesShot; k++)
                                    {
                                        Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, type == ModContent.ProjectileType<BrimstoneBarrage>() ? 0f : velocity, projectileVelocityToPass);
                                    }
                                }
                            }
                        }
                    }

                    calamityGlobalNPC.newAI[0] += chargeTelegraphRotationIncrement;
                    NPC.rotation += calamityGlobalNPC.newAI[0];
                    NPC.velocity *= 0.98f;
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (NPC.ai[1] == chargeGateValue)
                    {
                        float chargeVelocity = Vector2.Distance(NPC.Center, player.Center) / chargeDuration * 2f;
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (chargeVelocity + enrageScale * 2f);

                        if (NPC.velocity.Length() < chargeVelocityMin)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMin;
                        }

                        if (NPC.velocity.Length() > chargeVelocityMax)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMax;
                        }

                        NPC.ai[1] = chargeGateValue + chargeDuration;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] == chargeGateValue)
                    {
                        NPC.TargetClosest();

                        NPC.ai[1] = 0f;
                        NPC.localAI[0] = 0f;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= chargeSlownDownPhaseGateValue)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;
                }

                if (phase3)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 2f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 2f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (NPC.ai[1] < chargePhaseGateValue)
                {
                    NPC.ai[1] += 1f;

                    NPC.rotation = NPC.velocity.X * 0.1f;

                    NPC.localAI[0] += 1f;
                    if (NPC.localAI[0] >= 120f)
                    {
                        NPC.localAI[0] = 0f;
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int totalProjectiles = bossRush ? 18 : 12;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                int type = iceBlast;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                float projectileVelocityToPass = 0f;
                                if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                    projectileVelocityToPass = velocity * 2f;

                                Vector2 spinningPoint = new Vector2(0f, -velocity);
                                for (int k = 0; k < totalProjectiles; k++)
                                {
                                    Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                                }
                            }
                        }
                    }

                    Vector2 cryogenCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                    float playerXDist = player.Center.X - cryogenCenter.X;
                    float playerYDist = player.Center.Y - cryogenCenter.Y;
                    float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);

                    float cryogenSpeed = revenge ? 7f : 6f;
                    cryogenSpeed += 4f * enrageScale;

                    playerDistance = cryogenSpeed / playerDistance;
                    playerXDist *= playerDistance;
                    playerYDist *= playerDistance;

                    float inertia = 50f;
                    if (Main.getGoodWorld)
                        inertia *= 0.5f;

                    NPC.velocity.X = (NPC.velocity.X * inertia + playerXDist) / (inertia + 1f);
                    NPC.velocity.Y = (NPC.velocity.Y * inertia + playerYDist) / (inertia + 1f);
                }
                else if (NPC.ai[1] < chargeGateValue)
                {
                    NPC.ai[1] += 1f;

                    float totalSpreads = 2f;
                    if ((NPC.ai[1] - chargePhaseGateValue) % (chargeTelegraphTime / totalSpreads) == 0f)
                    {
                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = iceRain;
                                int damage = NPC.GetProjectileDamage(type);
                                float maxVelocity = 9f + enrageScale;
                                float velocity = maxVelocity - (calamityGlobalNPC.newAI[0] * maxVelocity * 0.5f);
                                int totalProjectiles = calamityGlobalNPC.newAI[1] == 0f ? 8 : 4;
                                int maxTotalProjectileReductionBasedOnRotationSpeed = (int)(totalProjectiles * 0.4f);
                                int totalProjectilesShot = totalProjectiles - (int)Math.Round(calamityGlobalNPC.newAI[0] * maxTotalProjectileReductionBasedOnRotationSpeed);
                                for (int i = 0; i < 3; i++)
                                {
                                    float radians = MathHelper.TwoPi / totalProjectilesShot;
                                    float newVelocity = velocity - (velocity * 0.33f * i);
                                    float projectileVelocityToPass = 0f;
                                    if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                        projectileVelocityToPass = velocity * 2f;

                                    double angleA = radians * 0.5;
                                    double angleB = MathHelper.ToRadians(90f) - angleA;
                                    float velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    Vector2 spinningPoint = i == 1 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectilesShot; k++)
                                    {
                                        Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, type == ModContent.ProjectileType<BrimstoneBarrage>() ? 0f : velocity, projectileVelocityToPass);
                                    }
                                }
                            }
                        }
                    }

                    calamityGlobalNPC.newAI[0] += chargeTelegraphRotationIncrement;
                    NPC.rotation += calamityGlobalNPC.newAI[0];
                    NPC.velocity *= 0.98f;
                }
                else
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (NPC.ai[1] == chargeGateValue)
                    {
                        float chargeVelocity = Vector2.Distance(NPC.Center, player.Center) / chargeDuration * 2f;
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (chargeVelocity + enrageScale * 2f);

                        if (NPC.velocity.Length() < chargeVelocityMin)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMin;
                        }

                        if (NPC.velocity.Length() > chargeVelocityMax)
                        {
                            NPC.velocity.Normalize();
                            NPC.velocity *= chargeVelocityMax;
                        }

                        NPC.ai[1] = chargeGateValue + chargeDuration;
                        calamityGlobalNPC.newAI[0] = 0f;
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] == chargeGateValue)
                    {
                        calamityGlobalNPC.newAI[1] += 1f;
                        if (calamityGlobalNPC.newAI[1] > 1f)
                        {
                            NPC.TargetClosest();
                            NPC.ai[1] = 0f;
                            NPC.localAI[0] = 0f;
                            calamityGlobalNPC.newAI[1] = 0f;
                        }
                        else
                            NPC.ai[1] = chargePhaseGateValue;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= chargeSlownDownPhaseGateValue)
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else
                        NPC.rotation += NPC.direction * 0.5f;
                }

                if (phase4)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 3f;
                    NPC.ai[1] = 0f;
                    NPC.localAI[0] = 0f;
                    calamityGlobalNPC.newAI[0] = 0f;
                    calamityGlobalNPC.newAI[1] = 0f;
                    NPC.netUpdate = true;
                }
            }
            else if (NPC.ai[0] == 3f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.rotation = NPC.velocity.X * 0.1f;

                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= 90f && NPC.Opacity == 1f)
                {
                    NPC.localAI[0] = 0f;
                    if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                    {
                        SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int totalProjectiles = bossRush ? 18 : 12;
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            int type = iceBlast;
                            int damage = NPC.GetProjectileDamage(type);
                            float velocity = 10f + enrageScale;
                            float projectileVelocityToPass = 0f;
                            if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                projectileVelocityToPass = velocity * 2f;

                            Vector2 spinningPoint = new Vector2(0f, -velocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, 0f, projectileVelocityToPass);
                            }
                        }
                    }
                }

                Vector2 cryogenCenter = new Vector2(NPC.Center.X, NPC.Center.Y);
                float playerXDist = player.Center.X - cryogenCenter.X;
                float playerYDist = player.Center.Y - cryogenCenter.Y;
                float playerDistance = (float)Math.Sqrt(playerXDist * playerXDist + playerYDist * playerYDist);

                float speed = revenge ? 5.5f : 5f;
                speed += 3f * enrageScale;

                playerDistance = speed / playerDistance;
                playerXDist *= playerDistance;
                playerYDist *= playerDistance;

                float inertia = 50f;
                if (Main.getGoodWorld)
                    inertia *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia + playerXDist) / (inertia + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia + playerYDist) / (inertia + 1f);

                if (NPC.ai[1] == 0f)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.localAI[2] += 1f;
                        if (NPC.localAI[2] >= 180f)
                        {
                            NPC.localAI[2] = 0f;
                            int attackTimer = 0;
                            int playerTileX = (int)player.Center.X / 16;
                            int playerTileY = (int)player.Center.Y / 16;
                            while (attackTimer <= 100)
                            {
                                attackTimer++;

                                int min = 16;
                                int max = 20;

                                if (Main.rand.NextBool())
                                    playerTileX += Main.rand.Next(min, max);
                                else
                                    playerTileX -= Main.rand.Next(min, max);

                                if (Main.rand.NextBool())
                                    playerTileY += Main.rand.Next(min, max);
                                else
                                    playerTileY -= Main.rand.Next(min, max);

                                if (!WorldGen.SolidTile(playerTileX, playerTileY) && Collision.CanHit(new Vector2(playerTileX * 16, playerTileY * 16), 1, 1, player.position, player.width, player.height))
                                    break;

                                playerTileX = (int)player.Center.X / 16;
                                playerTileY = (int)player.Center.Y / 16;
                            }
                            NPC.ai[1] = 1f;
                            teleportLocationX = playerTileX;
                            calamityGlobalNPC.newAI[2] = playerTileY;
                            NPC.netUpdate = true;
                        }
                    }
                }
                else if (NPC.ai[1] == 1f)
                {
                    Vector2 position = new Vector2(teleportLocationX * 16f - (NPC.width / 2), calamityGlobalNPC.newAI[2] * 16f - (NPC.height / 2));
                    for (int m = 0; m < 5; m++)
                    {
                        int dust = Dust.NewDust(position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                    }

                    NPC.Opacity -= 0.008f;
                    if (NPC.Opacity <= 0f)
                    {
                        NPC.Opacity = 0f;
                        NPC.position = position;

                        for (int n = 0; n < 15; n++)
                        {
                            int iceDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dustType, 0f, 0f, 100, default, 3f);
                            Main.dust[iceDust].noGravity = true;
                        }

                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            NPC.localAI[0] = 0f;
                            SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = iceRain;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 9f + enrageScale;
                                for (int i = 0; i < 3; i++)
                                {
                                    int totalProjectiles = bossRush ? 9 : 6;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * 0.33f * i);
                                    float projectileVelocityToPass = 0f;
                                    if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                        projectileVelocityToPass = velocity * 2f;

                                    float velocityX = 0f;
                                    if (i > 0)
                                    {
                                        double angleA = radians * 0.33 * (3 - i);
                                        double angleB = MathHelper.ToRadians(90f) - angleA;
                                        velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    }
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, 0f, type == ModContent.ProjectileType<BrimstoneBarrage>() ? 0f : velocity, projectileVelocityToPass);
                                    }
                                }
                            }
                        }

                        NPC.TargetClosest();
                        NPC.ai[1] = 2f;
                        NPC.netUpdate = true;
                    }
                }
                else if (NPC.ai[1] == 2f)
                {
                    NPC.Opacity += 0.2f;
                    if (NPC.Opacity >= 1f)
                    {
                        NPC.Opacity = 1f;
                        NPC.ai[1] = 0f;
                        NPC.netUpdate = true;
                    }
                }

                if (phase5)
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 150f;
                    NPC.ai[3] = 0f;
                    NPC.localAI[0] = 0f;
                    NPC.localAI[2] = 0f;
                    NPC.Opacity = 1f;
                    teleportLocationX = 0;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;

                    int chance = 100;
                    if (DateTime.Now.Month == 4 && DateTime.Now.Day == 1)
                        chance = 20;
                    if (Main.zenithWorld)
                        chance = 1;

                    if (Main.rand.NextBool(chance))
                    {
                        string key = Main.zenithWorld ? "Mods.CalamityMod.Status.Boss.PyrogenBossText" : "Mods.CalamityMod.Status.Boss.CryogenBossText";
                        Color messageColor = Main.zenithWorld ? Color.Orange : Color.Cyan;
                        CalamityUtils.DisplayLocalizedText(key, messageColor);
                    }
                }
            }
            else if (NPC.ai[0] == 4f)
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                if (phase6)
                {
                    if (NPC.ai[1] == 60f) // Spawn homing ice blasts on charge
                    {
                        NPC.velocity = Vector2.Normalize(player.Center - NPC.Center) * (18f + enrageScale * 2f);

                        if (Collision.CanHit(NPC.position, NPC.width, NPC.height, player.position, player.width, player.height))
                        {
                            SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                int type = iceBlast;
                                int damage = NPC.GetProjectileDamage(type);
                                float velocity = 1.5f + enrageScale * 0.5f;
                                int totalSpreads = phase7 ? 3 : 2;
                                for (int i = 0; i < totalSpreads; i++)
                                {
                                    int totalProjectiles = bossRush ? 3 : 2;
                                    float radians = MathHelper.TwoPi / totalProjectiles;
                                    float newVelocity = velocity - (velocity * (phase7 ? 0.25f : 0.5f) * i);
                                    float projectileVelocityToPass = 0f;
                                    if (type == ModContent.ProjectileType<BrimstoneBarrage>())
                                        projectileVelocityToPass = velocity * 12f;

                                    float velocityX = 0f;
                                    float ai = Main.zenithWorld ? 2f : NPC.target;
                                    if (i > 0)
                                    {
                                        double angleA = radians * (phase7 ? 0.25 : 0.5) * (totalSpreads - i);
                                        double angleB = MathHelper.ToRadians(90f) - angleA;
                                        velocityX = (float)(newVelocity * Math.Sin(angleA) / Math.Sin(angleB));
                                    }
                                    Vector2 spinningPoint = i == 0 ? new Vector2(0f, -newVelocity) : new Vector2(-velocityX, -newVelocity);
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer, ai, type == ModContent.ProjectileType<BrimstoneBarrage>() ? 0f : 1f, projectileVelocityToPass);
                                    }
                                }
                            }
                        }
                    }

                    NPC.ai[1] -= 1f;
                    if (NPC.ai[1] <= 0f) // Set the next charge, or switch back to floating above the player
                    {
                        NPC.ai[3] += 1f;
                        if (NPC.ai[3] > 2f)
                        {
                            NPC.ai[0] = 5f;
                            NPC.ai[1] = 0f;
                            NPC.ai[3] = 0f;
                            calamityGlobalNPC.newAI[3] = 0f;
                        }
                        else
                            NPC.ai[1] = 60f;

                        NPC.rotation = NPC.velocity.X * 0.1f;
                    }
                    else if (NPC.ai[1] <= 15f) // Slow down in preparation for the next charge
                    {
                        NPC.velocity *= 0.95f;
                        NPC.rotation = NPC.velocity.X * 0.15f;
                    }
                    else if (NPC.ai[1] > 60f) // Only used for when phase 6 first starts to prevent insta-charges
                    {
                        NPC.velocity *= 0.98f;
                        NPC.rotation += (150f - NPC.ai[1]) * 0.01f * NPC.direction;
                    }
                    else // Charge
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        NPC.rotation += NPC.direction * 0.5f;
                    }

                    return;
                }

                float chargeVelMult = 18f + enrageScale * 2f;

                Vector2 chargeDirection = new Vector2(NPC.Center.X + (NPC.direction * 20), NPC.Center.Y + 6f);
                float playerchargeXDist = player.position.X + player.width * 0.5f - chargeDirection.X;
                float playerchargeYDist = player.Center.Y - chargeDirection.Y;
                float playerDistance = (float)Math.Sqrt(playerchargeXDist * playerchargeXDist + playerchargeYDist * playerchargeYDist);
                float chargeSpeed = chargeVelMult / playerDistance;
                playerchargeXDist *= chargeSpeed;
                playerchargeYDist *= chargeSpeed;
                calamityGlobalNPC.newAI[2] -= 1f;

                float chargeStartDistance = 300f;
                float chargeCooldown = 30f;

                if (playerDistance < chargeStartDistance || calamityGlobalNPC.newAI[2] > 0f)
                {
                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (playerDistance < chargeStartDistance)
                        calamityGlobalNPC.newAI[2] = chargeCooldown;

                    if (NPC.velocity.Length() < chargeVelMult)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= chargeVelMult;
                    }

                    NPC.rotation += NPC.direction * 0.5f;

                    return;
                }

                float inertia = 30f;
                if (Main.getGoodWorld)
                    inertia *= 0.5f;

                NPC.velocity.X = (NPC.velocity.X * inertia + playerchargeXDist) / (inertia + 1f);
                NPC.velocity.Y = (NPC.velocity.Y * inertia + playerchargeYDist) / (inertia + 1f);
                if (playerDistance < chargeStartDistance + 200f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 9f + playerchargeXDist) / 10f;
                    NPC.velocity.Y = (NPC.velocity.Y * 9f + playerchargeYDist) / 10f;
                }
                if (playerDistance < chargeStartDistance + 100f)
                {
                    NPC.velocity.X = (NPC.velocity.X * 4f + playerchargeXDist) / 5f;
                    NPC.velocity.Y = (NPC.velocity.Y * 4f + playerchargeYDist) / 5f;
                }

                NPC.rotation = NPC.velocity.X * 0.15f;
            }
            else
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                NPC.rotation = NPC.velocity.X * 0.1f;

                calamityGlobalNPC.newAI[3] += 1f;
                if (calamityGlobalNPC.newAI[3] >= (bossRush ? 50f : 75f))
                {
                    calamityGlobalNPC.newAI[3] = 0f;
                    SoundEngine.PlaySound(Main.zenithWorld ? SoundID.NPCHit41 : HitSound, NPC.Center);
                    int totalProjectiles = 2;
                    float radians = MathHelper.TwoPi / totalProjectiles;
                    int type = iceBomb;
                    int damage = NPC.GetProjectileDamage(type);
                    float velocity2 = 6f;
                    double angleA = radians * 0.5;
                    double angleB = MathHelper.ToRadians(90f) - angleA;
                    float velocityX = (float)(velocity2 * Math.Sin(angleA) / Math.Sin(angleB));
                    Vector2 spinningPoint = Main.rand.NextBool() ? new Vector2(0f, -velocity2) : new Vector2(velocityX, -velocity2);
                    for (int k = 0; k < totalProjectiles; k++)
                    {
                        Vector2 projSpreadRotation = spinningPoint.RotatedBy(radians * k);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(projSpreadRotation) * 30f, projSpreadRotation, type, damage, 0f, Main.myPlayer);
                    }
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] >= (bossRush ? 120f : 180f))
                {
                    NPC.TargetClosest();
                    NPC.ai[0] = 4f;
                    NPC.ai[1] = 60f;
                    calamityGlobalNPC.newAI[3] = 0f;
                    calamityGlobalNPC.newAI[2] = 0f;
                    NPC.netUpdate = true;
                }

                float velocity = revenge ? 5f : 6f;
                float acceleration = 0.2f;
                velocity -= enrageScale;
                acceleration += 0.07f * enrageScale;

                if (NPC.position.Y > player.position.Y - 375f)
                {
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y -= acceleration;

                    if (NPC.velocity.Y > velocity)
                        NPC.velocity.Y = velocity;
                }
                else if (NPC.position.Y < player.position.Y - 400f)
                {
                    if (NPC.velocity.Y < 0f)
                        NPC.velocity.Y *= 0.98f;

                    NPC.velocity.Y += acceleration;

                    if (NPC.velocity.Y < -velocity)
                        NPC.velocity.Y = -velocity;
                }

                if (NPC.position.X + (NPC.width / 2) > player.position.X + (player.width / 2) + 350f)
                {
                    if (NPC.velocity.X > 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X -= acceleration;

                    if (NPC.velocity.X > velocity)
                        NPC.velocity.X = velocity;
                }
                if (NPC.position.X + (NPC.width / 2) < player.position.X + (player.width / 2) - 350f)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X *= 0.98f;

                    NPC.velocity.X += acceleration;

                    if (NPC.velocity.X < -velocity)
                        NPC.velocity.X = -velocity;
                }
            }
        }

        private void HandlePhaseTransition(int newPhase)
        {
            SoundStyle sound = Main.zenithWorld ? SoundID.NPCDeath14 : TransitionSound;
            SoundEngine.PlaySound(sound, NPC.Center);
            if (Main.netMode != NetmodeID.Server && !Main.zenithWorld)
            {
                int chipGoreAmount = newPhase >= 5 ? 3 : newPhase >= 3 ? 2 : 1;
                for (int i = 1; i < chipGoreAmount; i++)
                    Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("CryoChipGore" + i).Type, NPC.scale);
            }

            currentPhase = newPhase;

            switch (currentPhase)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    NPC.defense = 13;
                    NPC.Calamity().DR = 0.27f;
                    break;
                case 3:
                    NPC.defense = 10;
                    NPC.Calamity().DR = 0.21f;
                    break;
                case 4:
                    NPC.defense = 6;
                    NPC.Calamity().DR = 0.12f;
                    break;
                case 5:
                case 6:
                    NPC.defense = 0;
                    NPC.Calamity().DR = 0f;
                    break;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.zenithWorld)
            {
                float compactness = NPC.width * 0.6f;
                if (compactness < 10f)
                    compactness = 10f;

                float power = NPC.height / 100f;
                if (power > 2.75f)
                    power = 2.75f;

                if (FireDrawer is null)
                {
                    FireDrawer = new FireParticleSet(int.MaxValue, 1, Color.Red * 1.25f, Color.Red, compactness, power);
                }
                else
                {
                    FireDrawer.DrawSet(NPC.Bottom - Vector2.UnitY * (12f - NPC.gfxOffY));
                }
            }
            else
                FireDrawer = null;


            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            switch (currentPhase)
            {
                case 2:
                    texture = Phase2Texture.Value;
                    break;
                case 3:
                    texture = Phase3Texture.Value;
                    break;
                case 4:
                    texture = Phase4Texture.Value;
                    break;
                case 5:
                    texture = Phase5Texture.Value;
                    break;
                case 6:
                    texture = Phase6Texture.Value;
                    break;
                default:
                    texture = TextureAssets.Npc[NPC.type].Value;
                    break;
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            NPC.DrawBackglow(Main.zenithWorld ? Color.Red : BackglowColor, 4f, spriteEffects, NPC.frame, screenPos);

            Vector2 origin = new Vector2(TextureAssets.Npc[NPC.type].Value.Width / 2, TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2);
            Vector2 drawPos = NPC.Center - screenPos;
            drawPos -= new Vector2(texture.Width, texture.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawPos += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color overlay = Main.zenithWorld ? Color.Red : drawColor;
            spriteBatch.Draw(texture, drawPos, NPC.frame, NPC.GetAlpha(overlay), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (Main.zenithWorld)
            {
                typeName = CalamityUtils.GetTextValue("NPCs.Pyrogen");
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            int dusttype = Main.zenithWorld ? 235 : 67;
            for (int k = 0; k < 3; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 40; i++)
                {
                    int icyDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[icyDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[icyDust].scale = 0.5f;
                        Main.dust[icyDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int icyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 3f);
                    Main.dust[icyDust2].noGravity = true;
                    Main.dust[icyDust2].velocity *= 5f;
                    icyDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, dusttype, 0f, 0f, 100, default, 2f);
                    Main.dust[icyDust2].velocity *= 2f;
                }
                if (Main.netMode != NetmodeID.Server && !Main.zenithWorld)
                {
                    float randomSpread = Main.rand.Next(-200, 201) / 100f;
                    for (int i = 1; i < 4; i++)
                    {
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CryoDeathGore" + i).Type, NPC.scale);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("CryoChipGore" + i).Type, NPC.scale);
                    }
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.GreaterHealingPotion;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CryogenBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Avalanche>(),
                    ModContent.ItemType<HoarfrostBow>(),
                    ModContent.ItemType<SnowstormStaff>(),
                    ModContent.ItemType<Icebreaker>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));
                normalOnly.Add(ModContent.ItemType<GlacialEmbrace>(), 10);

                // Vanity
                normalOnly.Add(ModContent.ItemType<CryogenMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Materials
                normalOnly.Add(ModContent.ItemType<EssenceofEleum>(), 1, 8, 10);

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SoulofCryogen>()));
                normalOnly.Add(ModContent.ItemType<CryoStone>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<FrostFlare>(), DropHelper.NormalWeaponDropRateFraction);
            }

            npcLoot.Add(ItemID.FrozenKey, 3);

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<CryogenTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CryogenRelic>());

            // GFB Bloodflare Core drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<BloodflareCore>(), hideLootReport: true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCryogen, ModContent.ItemType<LoreArchmage>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Spawn Permafrost if he isn't in the world
            int permafrostNPC = NPC.FindFirstNPC(ModContent.NPCType<DILF>());
            if (permafrostNPC == -1 && !BossRushEvent.BossRushActive)
                NPC.NewNPC(NPC.GetSource_Death(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DILF>(), 0, 0f, 0f, 0f, 0f, 255);

            // If Cryogen has not been killed, notify players about Cryonic Ore
            if (!DownedBossSystem.downedCryogen)
            {
                string key = "Mods.CalamityMod.Status.Progression.IceOreText";
                Color messageColor = Color.LightSkyBlue;
                CalamityUtils.SpawnOre(ModContent.TileType<CryonicOre>(), 15E-05, 0.45f, 0.7f, 3, 8, TileID.SnowBlock, TileID.IceBlock, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, ModContent.TileType<AstralSnow>(), ModContent.TileType<AstralIce>());

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark Cryogen as dead
            DownedBossSystem.downedCryogen = true;
            CalamityNetcode.SyncWorld();
        }

        // Can only hit the target if within certain distance
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

            return minDist <= 40f * NPC.scale;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                if (Main.zenithWorld)
                {
                    target.AddBuff(ModContent.BuffType<BrimstoneFlames>(), 120, true);
                    target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 120, true);
                }
                else
                {
                    target.AddBuff(BuffID.Frostburn, 240, true);
                    target.AddBuff(BuffID.Chilled, 120, true);
                }
            }
        }
    }
}
