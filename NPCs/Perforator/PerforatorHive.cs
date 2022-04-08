using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
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
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.Perforator
{
    [AutoloadBossHead]
    public class PerforatorHive : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool small = false;
        private bool medium = false;
        private bool large = false;

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("The Perforator Hive");
            Main.npcFrameCount[NPC.type] = 10;
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
            Music = CalamityMod.Instance.GetMusicFromMusicMod("Perforators") ?? MusicID.Boss2;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
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
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
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

            bool malice = CalamityWorld.malice || BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Variables for ichor spore phase
            float sporePhaseGateValue = malice ? 450f : 600f;
            bool floatAboveToFireBlobs = NPC.ai[2] >= sporePhaseGateValue - 120f;

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
            if ((!player.ZoneCrimson || (NPC.position.Y / 16f) < Main.worldSurface) && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || malice;

            float enrageScale = BossRushEvent.BossRushActive ? 1f : 0f;
            if (biomeEnraged && (!player.ZoneCrimson || malice))
            {
                NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
                enrageScale += 1f;
            }
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || malice))
            {
                NPC.Calamity().CurrentlyEnraged = !BossRushEvent.BossRushActive;
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

            int wormsAlive = 0;
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>()))
                wormsAlive++;
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadMedium>()))
                wormsAlive++;
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadSmall>()))
                wormsAlive++;

            if (NPC.ai[3] == 0f && NPC.life > 0)
                NPC.ai[3] = NPC.lifeMax;

            bool canSpawnWorms = !small || !medium || !large;
            if (NPC.life > 0 && canSpawnWorms)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int num660 = (int)(NPC.lifeMax * 0.25);
                    if ((NPC.life + num660) < NPC.ai[3])
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
                        NPC.NewNPC(NPC.GetSpawnSourceForNPCFromNPCAI(), (int)NPC.Center.X, (int)NPC.Center.Y, wormType, 1);
                        NPC.TargetClosest();

                        SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.position);

                        for (int num621 = 0; num621 < 16; num621++)
                        {
                            int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 170, 0f, 0f, 100, default, 1f);
                            Main.dust[num622].velocity *= 2f;
                            if (Main.rand.NextBool(2))
                            {
                                Main.dust[num622].scale = 0.25f;
                                Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }

                        for (int num623 = 0; num623 < 32; num623++)
                        {
                            int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1.5f);
                            Main.dust[num624].noGravity = true;
                            Main.dust[num624].velocity *= 3f;
                            num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1f);
                            Main.dust[num624].velocity *= 2f;
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
                if (wormsAlive == 0 || malice || floatAboveToFireBlobs)
                {
                    NPC.ai[2] += 1f;
                    if (NPC.ai[2] >= sporePhaseGateValue)
                    {
                        if (NPC.ai[2] < sporePhaseGateValue + 300f)
                        {
                            if (NPC.velocity.Length() > 0.5f)
                                NPC.velocity *= malice ? 0.94f : 0.96f;
                            else
                                NPC.ai[2] = sporePhaseGateValue + 300f;
                        }
                        else
                        {
                            NPC.ai[2] = 0f;

                            SoundEngine.PlaySound(SoundID.NPCDeath23, NPC.position);

                            for (int num621 = 0; num621 < 32; num621++)
                            {
                                int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 170, 0f, 0f, 100, default, 1f);
                                float dustVelocityYAdd = Math.Abs(Main.dust[num622].velocity.Y) * 0.5f;
                                if (Main.dust[num622].velocity.Y < 0f)
                                    Main.dust[num622].velocity.Y = 2f + dustVelocityYAdd;
                                if (Main.rand.NextBool(2))
                                {
                                    Main.dust[num622].scale = 0.25f;
                                    Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                                }
                            }

                            int numBlobs = expertMode ? 6 : 3;
                            int type = ModContent.ProjectileType<IchorBlob>();
                            int damage = NPC.GetProjectileDamage(type);

                            for (int i = 0; i < numBlobs; i++)
                            {
                                Vector2 blobVelocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                blobVelocity.Normalize();
                                blobVelocity *= Main.rand.Next(200, 401) * (malice ? 0.02f : 0.01f);

                                float sporeVelocityYAdd = Math.Abs(blobVelocity.Y) * 0.5f;
                                if (blobVelocity.Y < 2f)
                                    blobVelocity.Y = 2f + sporeVelocityYAdd;

                                Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), NPC.Center, blobVelocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
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
                int shoot = (revenge ? 6 : 4) - wormsAlive;
                NPC.localAI[0] += Main.rand.Next(shoot);
                if (NPC.localAI[0] >= Main.rand.Next(300, 901) && NPC.position.Y + NPC.height < player.position.Y && Vector2.Distance(player.Center, NPC.Center) > 80f)
                {
                    NPC.localAI[0] = 0f;
                    SoundEngine.PlaySound(SoundID.NPCHit20, NPC.position);

                    for (int num621 = 0; num621 < 8; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 170, 0f, 0f, 100, default, 1f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[num622].scale = 0.25f;
                            Main.dust[num622].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }

                    for (int num623 = 0; num623 < 16; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1.5f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 1f);
                        Main.dust[num624].velocity *= 2f;
                    }

                    int type = Main.rand.NextBool(2) ? ModContent.ProjectileType<IchorShot>() : ModContent.ProjectileType<BloodGeyser>();
                    int damage = NPC.GetProjectileDamage(type);
                    int totalProjectiles = death ? 16 : revenge ? 14 : expertMode ? 12 : 10;
                    float maxVelocity = 8f;
                    float velocityAdjustment = maxVelocity * 1.5f / totalProjectiles;
                    Vector2 start = new Vector2(NPC.Center.X, NPC.Center.Y + 30f);
                    Vector2 destination = wormsAlive > 0 ? new Vector2(Vector2.Normalize(player.Center - start).X, 0f) * maxVelocity * 0.4f : Vector2.Zero;
                    Vector2 velocity = destination + Vector2.UnitY * -maxVelocity;
                    for (int i = 0; i < totalProjectiles + 1; i++)
                    {
                        Projectile.NewProjectile(NPC.GetSpawnSource_ForProjectile(), start, velocity, type, damage, 0f, Main.myPlayer, 0f, player.Center.Y);
                        velocity.X += velocityAdjustment * NPC.direction;
                    }
                }
            }

            if (revenge)
            {
                if (wormsAlive == 1)
                {
                    Movement(player, 4f + velocityEnrageIncrease, 0.1f, 350f);
                }
                else
                {
                    if (large || death)
                        Movement(player, 5.5f + velocityEnrageIncrease, death ? 0.075f : 0.065f, 20f);
                    else if (medium)
                        Movement(player, 5f + velocityEnrageIncrease, death ? 0.07f : 0.06f, 30f);
                    else if (small)
                        Movement(player, 4.5f + velocityEnrageIncrease, death ? 0.065f : 0.055f, 40f);
                    else
                        Movement(player, 4f + velocityEnrageIncrease, death ? 0.06f : 0.05f, 50f);
                }
            }
            else
                Movement(player, 4f + velocityEnrageIncrease, 0.05f, 350f);
        }

        private void Movement(Player target, float velocity, float acceleration, float y)
        {
            // Distance from destination where Perf Hive stops moving
            float movementDistanceGateValue = 100f;

            // This is where Perf Hive should be
            Vector2 destination = new Vector2(target.Center.X, target.Center.Y - y);

            // How far Perf Hive is from where it's supposed to be
            Vector2 distanceFromDestination = destination - NPC.Center;

            // Inverse lerp returns the percentage of progress between A and B
            float lerpValue = Utils.GetLerpValue(movementDistanceGateValue, 2400f, distanceFromDestination.Length(), true);

            // Min velocity
            float minVelocity = distanceFromDestination.Length();
            float minVelocityCap = velocity;
            if (minVelocity > minVelocityCap)
                minVelocity = minVelocityCap;

            // Max velocity
            Vector2 maxVelocity = distanceFromDestination / 24f;
            float maxVelocityCap = minVelocityCap * 3f;
            if (maxVelocity.Length() > maxVelocityCap)
                maxVelocity = distanceFromDestination.SafeNormalize(Vector2.Zero) * maxVelocityCap;

            Vector2 desiredVelocity = Vector2.Lerp(distanceFromDestination.SafeNormalize(Vector2.Zero) * minVelocity, maxVelocity, lerpValue);
            NPC.SimpleFlyMovement(desiredVelocity, acceleration);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[NPC.type].Value;
            Vector2 vector11 = new Vector2((float)(TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));

            Vector2 vector43 = NPC.Center - screenPos;
            vector43 -= new Vector2((float)texture2D15.Width, (float)(texture2D15.Height / Main.npcFrameCount[NPC.type])) * NPC.scale / 2f;
            vector43 += vector11 * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(texture2D15, vector43, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            texture2D15 = ModContent.Request<Texture2D>("CalamityMod/NPCs/Perforator/PerforatorHiveGlow").Value;
            Color color37 = Color.Lerp(Color.White, Color.Yellow, 0.5f);

            spriteBatch.Draw(texture2D15, vector43, NPC.frame, color37, NPC.rotation, vector11, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CheckDead()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<PerforatorHeadLarge>()))
            {
                return false;
            }
            return true;
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * bossLifeScale);
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
                string key = "Mods.CalamityMod.SkyOreText";
                Color messageColor = Color.Cyan;
                CalamityUtils.SpawnOre(ModContent.TileType<AerialiteOre>(), 12E-05, 0.4f, 0.6f, 3, 8);

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark The Perforator Hive as dead
            DownedBossSystem.downedPerforator = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<PerforatorBag>()));

            npcLoot.Add(ModContent.ItemType<PerforatorTrophy>(), 10);
            npcLoot.AddIf(() => !DownedBossSystem.downedPerforator, ModContent.ItemType<KnowledgePerforators>());

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<VeinBurster>(),
                    ModContent.ItemType<BloodyRupture>(),
                    ModContent.ItemType<SausageMaker>(),
                    ModContent.ItemType<Aorta>(),
                    ModContent.ItemType<Eviscerator>(),
                    ModContent.ItemType<BloodBath>(),
                    ModContent.ItemType<BloodClotStaff>(),
                    ModContent.ItemType<BloodstainedGlove>(),
                };
                normalOnly.Add(ItemDropRule.OneFromOptions(DropHelper.NormalWeaponDropRateInt, weapons));
                normalOnly.Add(ModContent.ItemType<ToothBall>(), 1, 30, 50);

                // Materials
                normalOnly.Add(ItemID.CrimtaneBar, 1, 12, 15);
                normalOnly.Add(ItemID.Vertebrae, 1, 12, 15);
                normalOnly.Add(ItemID.CrimsonSeeds, 1, 12, 15);
                normalOnly.Add(ModContent.ItemType<BloodSample>(), 1, 35, 45);
                normalOnly.Add(ItemDropRule.ByCondition(new Conditions.IsHardmode(), ItemID.Ichor, 1, 10, 20));

                // Equipment
                normalOnly.Add(ModContent.ItemType<BloodyWormTooth>());

                // Vanity
                normalOnly.Add(ModContent.ItemType<PerforatorMask>(), 7);
                normalOnly.Add(ModContent.ItemType<BloodyVein>(), 10);
            }
        }

        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(ModContent.BuffType<BurningBlood>(), 180, true);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            for (int k = 0; k < damage / NPC.lifeMax * 100.0; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Hive").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Hive2").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Hive3").Type, 1f);
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.Find<ModGore>("Gores/Hive4").Type, 1f);
                }
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int num621 = 0; num621 < 40; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 70; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 5, 0f, 0f, 100, default, 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
        }
    }
}
