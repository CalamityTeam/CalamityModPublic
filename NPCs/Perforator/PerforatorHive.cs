using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Fishing.BrimstoneCragCatches;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
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
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
using CalamityMod.Items.Ammo;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHive : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool small = false;
        private bool medium = false;
        private bool large = false;
        private int wormsAlive = 0;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0);
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 18f;
            NPC.GetNPCDamage();
            NPC.width = 110;
            NPC.height = 100;
            NPC.defense = 4;
            NPC.LifeMaxNERB(5000, 6000, 270000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 15, 0, 0);
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit13;
            NPC.DeathSound = SoundID.NPCDeath19;
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
            NPC.frameCounter %= Main.npcFrameCount[NPC.type];
            int frame = (int)NPC.frameCounter;
            NPC.frame.Y = frame * frameHeight;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(wormsAlive);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            wormsAlive = reader.ReadInt32();
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

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Variables for ichor blob phase
            float blobPhaseGateValue = bossRush ? 450f : 600f;
            bool floatAboveToFireBlobs = NPC.ai[2] >= blobPhaseGateValue - 120f;

            // Don't deal damage for 3 seconds after spawning or while firing blobs
            NPC.damage = NPC.defDamage;
            if (NPC.ai[1] < 180f || floatAboveToFireBlobs)
            {
                if (NPC.ai[1] < 180f)
                    NPC.ai[1] += 1f;

                NPC.damage = 0;
            }

            Player player = Main.player[NPC.target];

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases based on life percentage
            bool phase2 = lifeRatio < 0.7f;

            // Enrage
            if ((!player.ZoneCrimson || (NPC.position.Y / 16f) < Main.worldSurface) && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && (!player.ZoneCrimson || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }

            if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];
                if (!player.active || player.dead || Vector2.Distance(player.Center, NPC.Center) > 5600f)
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
            else if (NPC.timeLeft < 1800)
                NPC.timeLeft = 1800;

            //GFB seed shenanigans: Behavior during the suck
            if (NPC.localAI[1] >= 6f)
            {
                //Leak projectiles everywhere and start healing
                int type = Main.rand.NextBool() ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                int damage = NPC.GetProjectileDamage(type);
                int spread = Main.rand.Next(-45, 46);
                Vector2 baseVelocity = Vector2.UnitY * Main.rand.NextFloat(-12.5f, -5f);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, baseVelocity.RotatedBy(MathHelper.ToRadians(spread)), type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);

                //Heals 10 times per second for 0.1% of its health each = 1% per second
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

            NPC.Calamity().DR = wormsAlive * 0.3f;

            if (NPC.ai[3] == 0f && NPC.life > 0)
                NPC.ai[3] = NPC.lifeMax;

            bool canSpawnWorms = !small || !medium || !large || Main.getGoodWorld;
            if (NPC.life > 0 && canSpawnWorms)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int wormSpawnGateValue = (int)(NPC.lifeMax * (Main.getGoodWorld ? 0.15 : 0.25));
                    if ((NPC.life + wormSpawnGateValue) < NPC.ai[3])
                    {
                        NPC.ai[3] = NPC.life;
                        int wormType = ModContent.NPCType<PerforatorHeadSmall>();
                        if (!small)
                        {
                            small = true;
                        }
                        else if (!medium)
                        {
                            medium = true;
                            wormType = ModContent.NPCType<PerforatorHeadMedium>();
                        }
                        else if (!large)
                        {
                            large = true;
                            wormType = ModContent.NPCType<PerforatorHeadLarge>();
                        }

                        if (Main.getGoodWorld && lifeRatio < 0.5f)
                        {
                            if (lifeRatio > 0.35f)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadLarge>(), 1);
                            else if (lifeRatio > 0.2f)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadMedium>(), 1);
                            else if (lifeRatio > 0.05f)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<PerforatorHeadSmall>(), 1);
                        }
                        else
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, wormType, 1);

                        NPC.TargetClosest();

                        SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);

                        for (int i = 0; i < 16; i++)
                        {
                            int ichorDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 170, 0f, 0f, 100, default, 1f);
                            Main.dust[ichorDust].velocity *= 2f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[ichorDust].scale = 0.25f;
                                Main.dust[ichorDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int j = 0; j < 32; j++)
                        {
                            int bloodDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1.5f);
                            Main.dust[bloodDust].noGravity = true;
                            Main.dust[bloodDust].velocity *= 3f;
                            bloodDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1f);
                            Main.dust[bloodDust].velocity *= 2f;
                        }
                    }
                }
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
                if (wormsAlive == 0 || bossRush || floatAboveToFireBlobs || (CalamityWorld.LegendaryMode && CalamityWorld.revenge))
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= blobPhaseGateValue)
                    {
                        if (NPC.ai[2] < blobPhaseGateValue + 300f)
                        {
                            if (NPC.velocity.Length() > 0.5f)
                                NPC.velocity *= bossRush ? 0.94f : 0.96f;
                            else
                                NPC.ai[2] = blobPhaseGateValue + 300f;
                        }
                        else
                        {
                            NPC.ai[2] = 0f;

                            SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.Center);

                            for (int i = 0; i < 32; i++)
                            {
                                int ichorDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 170, 0f, 0f, 100, default, 1f);
                                float dustVelocityYAdd = Math.Abs(Main.dust[ichorDust].velocity.Y) * 0.5f;
                                if (Main.dust[ichorDust].velocity.Y < 0f)
                                    Main.dust[ichorDust].velocity.Y = 2f + dustVelocityYAdd;
                                if (Main.rand.NextBool())
                                {
                                    Main.dust[ichorDust].scale = 0.25f;
                                    Main.dust[ichorDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                }
                            }

                            int numBlobs = expertMode ? 6 : 3;
                            if (Main.getGoodWorld)
                                numBlobs *= 2;

                            int type = ModContent.ProjectileType<IchorBlob>();
                            int damage = NPC.GetProjectileDamage(type);

                            for (int i = 0; i < numBlobs; i++)
                            {
                                Vector2 blobVelocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                blobVelocity.Normalize();
                                blobVelocity *= Main.rand.Next(400, 801) * (bossRush ? 0.02f : 0.01f);

                                if (CalamityWorld.LegendaryMode && CalamityWorld.revenge)
                                    blobVelocity *= Main.rand.NextFloat() + 1f;

                                float blobVelocityYAdd = Math.Abs(blobVelocity.Y) * 0.5f;
                                if (blobVelocity.Y < 2f)
                                    blobVelocity.Y = 2f + blobVelocityYAdd;

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.UnitY * 50f, blobVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                            }
                        }

                        return;
                    }
                }
            }

            // Movement velocities, increased while enraged
            float velocityEnrageIncrease = enrageScale;

            // When firing blobs, float above the target and don't call any other projectile firing or movement code
            if (floatAboveToFireBlobs)
            {
                if (revenge)
                    Movement(player, 6f + velocityEnrageIncrease, 0.3f, 450f);
                else
                    Movement(player, 5f + velocityEnrageIncrease, 0.2f, 450f);

                return;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= (revenge ? 200f : 250f) + wormsAlive * 150f && NPC.position.Y + NPC.height < player.position.Y && Vector2.Distance(player.Center, NPC.Center) > 80f)
                {
                    NPC.localAI[0] = 0f;
                    SoundEngine.PlaySound(SoundID.NPCHit20, NPC.Center);

                    for (int i = 0; i < 8; i++)
                    {
                        int ichorDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 170, 0f, 0f, 100, default, 1f);
                        Main.dust[ichorDust].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[ichorDust].scale = 0.25f;
                            Main.dust[ichorDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    for (int j = 0; j < 16; j++)
                    {
                        int bloodDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1.5f);
                        Main.dust[bloodDust].noGravity = true;
                        Main.dust[bloodDust].velocity *= 5f;
                        bloodDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1f);
                        Main.dust[bloodDust].velocity *= 2f;
                    }

                    int type = Main.rand.NextBool() ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                    int damage = NPC.GetProjectileDamage(type);
                    int numProj = death ? 16 : revenge ? 14 : expertMode ? 12 : 10;
                    if (Main.getGoodWorld)
                        numProj *= 2;

                    int spread = 75;
                    float velocity = 8f;
                    Vector2 destination = wormsAlive > 0 ? player.Center : NPC.Center - Vector2.UnitY * 100f;
                    Vector2 projectileVelocity = new Vector2(Vector2.Normalize(destination - NPC.Center).X * velocity, -velocity);
                    float rotation = MathHelper.ToRadians(spread);
                    for (int i = 0; i < numProj; i++)
                    {
                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));
                        Vector2 randomVelocity = new Vector2(Main.rand.NextFloat() - 0.5f, Main.rand.NextFloat() - 0.5f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 50f, perturbedSpeed + randomVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                    }
                }
            }

            if (revenge)
            {
                switch (wormsAlive)
                {
                    case 0:
                        if (large || death)
                            Movement(player, 11f + velocityEnrageIncrease, death ? 0.1125f : 0.0975f, 20f);
                        else if (medium)
                            Movement(player, 10f + velocityEnrageIncrease, death ? 0.105f : 0.09f, 30f);
                        else if (small)
                            Movement(player, 9f + velocityEnrageIncrease, death ? 0.0975f : 0.0825f, 40f);
                        else
                            Movement(player, 8f + velocityEnrageIncrease, death ? 0.09f : 0.075f, 50f);
                        break;

                    case 1:
                        Movement(player, 6f + velocityEnrageIncrease, 0.15f, 350f);
                        break;

                    case 2:
                        Movement(player, 6f + velocityEnrageIncrease, 0.15f, 275f);
                        break;

                    case 3:
                        Movement(player, 6f + velocityEnrageIncrease, 0.15f, 200f);
                        break;
                }
            }
            else
                Movement(player, 6f + velocityEnrageIncrease, 0.075f, 350f);
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
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 halfSizeTexture = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Perforator/PerforatorHiveGlow").Value;
            Color glowmaskColor = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, glowmaskColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            CalamityGlobalNPC.SetNewShopVariable(new int[] { NPCID.Dryad }, DownedBossSystem.downedPerforator);

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!DownedBossSystem.downedHiveMind && !DownedBossSystem.downedPerforator)
            {
                string key = "Mods.CalamityMod.Status.Progression.SkyOreText";
                Color messageColor = Color.Cyan;
                AerialiteOreGen.Generate(true);

                CalamityUtils.DisplayLocalizedText(key, messageColor);
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
					new WeightedItemStack(ModContent.ItemType<ToothBall>(), 1f, 30, 50),
				}));

                // Materials
                normalOnly.Add(ItemID.CrimtaneBar, 1, 10, 15);
                normalOnly.Add(ItemID.Vertebrae, 1, 10, 15);
                normalOnly.Add(ItemID.CrimsonSeeds, 1, 10, 15);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<BloodSample>(), 1, 25, 30));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.Hardmode(), ItemID.Ichor, 1, 10, 20));

                // Equipment
				normalOnly.Add(ModContent.ItemType<BloodstainedGlove>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<BloodyWormTooth>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<PerforatorMask>(), 7);
                normalOnly.Add(ModContent.ItemType<BloodyVein>(), 10);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<PerforatorTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<PerforatorsRelic>());

            // GFB Bloodfin drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(ModContent.ItemType<Bloodfin>(), 1, 1, 9999, true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedPerforator, ModContent.ItemType<LorePerforators>(), desc: DropHelper.FirstKillText);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<BurningBlood>(), 180, true);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < hit.Damage / NPC.lifeMax * 100.0; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
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
                for (int i = 0; i < 40; i++)
                {
                    int ichorDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[ichorDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[ichorDust].scale = 0.5f;
                        Main.dust[ichorDust].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int bloodDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[bloodDust].noGravity = true;
                    Main.dust[bloodDust].velocity *= 5f;
                    bloodDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[bloodDust].velocity *= 2f;
                }
            }
        }
    }
}
