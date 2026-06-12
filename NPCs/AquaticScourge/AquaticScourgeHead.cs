using System.IO;
using CalamityMod.BiomeManagers;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Placeables.Abyss;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AcidRain;
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
using Terraria.ID;
using Terraria.ModLoader;
using CalamityMod.Systems.Collections;

namespace CalamityMod.NPCs.AquaticScourge
{
    [AutoloadBossHead]
    [HasPierceResist]
    [LongDistanceNetSync]
    public class AquaticScourgeHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.6f,
                PortraitScale = 0.6f
            };
            value.Position.X += 40f;
            value.Position.Y += 20f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public static int MistDamage = 23; // 92
        public static int CloudDamage = 26; // 104; applies to both Sand and Toxic

        public override void SetDefaults()
        {
            NPC.damage = 85; // 170
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.width = 90;
            NPC.height = 90;
            NPC.defense = 10;
            NPC.DR_NERD(0.05f);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.LifeMaxNERB(80000, 96000, 1000000);
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 12);
            NPC.behindTiles = true;
            NPC.chaseable = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.netAlways = true;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.2f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.15f;
            else if (Main.expertMode)
                NPC.scale *= 1.1f;

            if (Main.getGoodWorld)
                NPC.scale *= 1.25f;

            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToSickness = false;
            NPC.Calamity().VulnerableToElectricity = true;
            NPC.Calamity().VulnerableToWater = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<SulphurousSeaBiome>().Type };
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.UIInfoProvider = new CommonEnemyUICollectionInfoProvider(ContentSamples.NpcBestiaryCreditIdsByNpcNetIds[Type], quickUnlock: true);
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new BossBestiaryInfoElement(),
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AquaticScourge")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.chaseable);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            writer.Write(NPC.npcSlots);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.chaseable = reader.ReadBoolean();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            NPC.npcSlots = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            bool getFuckedAI = Main.zenithWorld;
            CalamityGlobalNPC.aquaticScourge = NPC.whoAmI;

            // Adjust hostility and stats
            bool nonHostile = calamityGlobalNPC.newAI[0] == 0f;
            if (NPC.justHit || NPC.life <= NPC.lifeMax * 0.999 || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (nonHostile)
                {
                    // Kiss my motherfucking ass you piece of shit game
                    NPC.timeLeft *= 20;
                    NPC.npcSlots = 16f;
                    NPC.damage = NPC.defDamage;
                    calamityGlobalNPC.KillTime = CalamityNPCSets.BossKillTimes[NPC.type];
                    calamityGlobalNPC.newAI[0] = 1f;
                    nonHostile = false;
                    NPC.boss = true;
                    NPC.chaseable = true;
                    NPC.netUpdate = true;
                }
            }
            else
                NPC.damage = 0;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.75f;
            bool phase3 = lifeRatio < 0.5f;
            bool phase4 = lifeRatio < 0.25f;

            // Set worm variable
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool notOcean = player.position.Y < 300f ||
                player.position.Y > Main.worldSurface * 16.0 ||
                (player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));

            // Check for the flipped Abyss
            if (Main.remixWorld)
            {
                notOcean = player.position.Y < Main.UnderworldLayer * 0.8f || player.position.Y > Main.UnderworldLayer ||
                    (player.position.X > 7680f && player.position.X < (Main.maxTilesX * 16 - 7680));
            }

            // Enrage
            if (notOcean && !player.Calamity().ZoneSulphur && !BossRushEvent.BossRushActive)
            {
                if (NPC.localAI[2] > 0f)
                    NPC.localAI[2] -= 1f;
            }
            else
                NPC.localAI[2] = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = NPC.localAI[2] <= 0f;

            float enrageScale = 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 2f;
            }

            // Circular movement
            float colorFadeTimeAfterSpiral = 90f;
            float spiralGateValue = 480f;
            bool doSpiral = false;
            if (calamityGlobalNPC.newAI[0] == 1f && calamityGlobalNPC.newAI[2] == 1f && (revenge || getFuckedAI))
            {
                doSpiral = calamityGlobalNPC.newAI[1] == 0f && calamityGlobalNPC.newAI[3] >= spiralGateValue;
                if (Vector2.Distance(NPC.Center, player.Center) < (getFuckedAI ? 1600f : 1000f) || doSpiral)
                    calamityGlobalNPC.newAI[3] += 1f;

                if (doSpiral)
                {
                    NPC.localAI[3] = colorFadeTimeAfterSpiral;

                    // Vomit acid mist
                    float acidMistBarfDivisor = getFuckedAI ? 2f : ((float)Math.Floor(death ? 5f : 6f) * (phase3 ? 1.5f : 1f));
                    if (calamityGlobalNPC.newAI[3] % acidMistBarfDivisor == 0f)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            float mistVelocity = death ? 10f : 8f;
                            Vector2 projectileVelocity = (NPC.Center + NPC.velocity * 10f - NPC.Center).SafeNormalize(Vector2.UnitY);
                            int type = ModContent.ProjectileType<SulphuricAcidMist>();
                            int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + projectileVelocity * 5f, projectileVelocity * mistVelocity, type, MistDamage, 0f, Main.myPlayer);
                            Main.projectile[proj].tileCollide = false;
                            Main.projectile[proj].timeLeft = getFuckedAI ? 240 : 600;
                        }
                    }

                    // Vomit circular spreads of acid clouds while in phase 3
                    float toxicCloudBarfDivisor = death ? 30f : 40f;
                    if (calamityGlobalNPC.newAI[3] % toxicCloudBarfDivisor == 0f && phase3)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int type = ModContent.ProjectileType<ToxicCloud>();
                            int totalProjectiles = (phase4 ? 6 : 9) + (getFuckedAI ? Main.rand.Next(-2, 3) : (int)((calamityGlobalNPC.newAI[3] - spiralGateValue) / toxicCloudBarfDivisor) * (phase4 ? 2 : 3));
                            float radians = MathHelper.TwoPi / totalProjectiles;
                            float cloudVelocity = 1f + enrageScale;
                            Vector2 spinningPoint = new Vector2(0f, -cloudVelocity);
                            for (int k = 0; k < totalProjectiles; k++)
                            {
                                Vector2 vector255 = spinningPoint.RotatedBy(radians * k);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + vector255.SafeNormalize(Vector2.UnitY) * 5f, vector255, type, CloudDamage, 0f, Main.myPlayer);
                            }
                        }
                    }

                    // Velocity boost
                    if (calamityGlobalNPC.newAI[3] == spiralGateValue)
                    {
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY);
                        NPC.velocity *= 24f;
                    }

                    // Spin velocity
                    float velocity = (float)(MathHelper.Pi * 2D) / 120f;
                    // In GFB, contracts the radius as the fight progresses
                    if (getFuckedAI)
                        velocity *= phase3 ? 1.5f : phase2 ? 1.25f : 1f;
                    NPC.velocity = NPC.velocity.RotatedBy(-(double)velocity * NPC.localAI[1]);
                    // Speed up even more in GFB for more radius
                    if (getFuckedAI && NPC.velocity.Length() <= 32f)
                        NPC.velocity *= 1.1f;
                    NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

                    // Reset and charge at target
                    // Don't reset in GFB
                    if (!getFuckedAI && calamityGlobalNPC.newAI[3] >= spiralGateValue + 120f)
                    {
                        calamityGlobalNPC.newAI[3] = 0f;
                        NPC.TargetClosest();
                    }
                }
                else
                {
                    if (!Collision.CanHit(NPC.Center, 1, 1, player.position, player.width, player.height) && calamityGlobalNPC.newAI[3] > 300f)
                        calamityGlobalNPC.newAI[3] -= 2f;

                    if (NPC.localAI[3] > 0f)
                        NPC.localAI[3] -= 1f;

                    NPC.localAI[1] = NPC.Center.X - player.Center.X < 0 ? 1f : -1f;
                }
            }

            // Spawn segments
            if (calamityGlobalNPC.newAI[2] == 0f && NPC.ai[0] == 0f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int maxLength = getFuckedAI ? 24 : death ? 80 : revenge ? 40 : expertMode ? 35 : 30;
                    int Previous = NPC.whoAmI;
                    for (int segments = 0; segments < maxLength; segments++)
                    {
                        int lol;
                        if (segments >= 0 && segments < maxLength - 1)
                        {
                            if (segments % 2 == 0)
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AquaticScourgeBodyAlt>(), NPC.whoAmI);
                            else
                                lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AquaticScourgeBody>(), NPC.whoAmI);
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AquaticScourgeTail>(), NPC.whoAmI);

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol);
                        Previous = lol;
                    }
                }

                calamityGlobalNPC.newAI[2] = 1f;
            }

            // Big barf attack
            if (calamityGlobalNPC.newAI[0] == 1f && (!doSpiral && phase2) || (getFuckedAI && !phase3))
            {
                NPC.localAI[0] += 1f;
                if (NPC.localAI[0] >= (revenge ? 360f : 420f))
                {
                    if (Vector2.Distance(player.Center, NPC.Center) > 320f)
                    {
                        NPC.localAI[0] = 0f;
                        NPC.netUpdate = true;
                        SoundEngine.PlaySound(SoundID.NPCDeath13, NPC.Center);

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int totalProjectiles = expertMode ? 8 : 6;
                            if (phase3)
                                totalProjectiles *= 2;

                            int type = ModContent.ProjectileType<SandPoisonCloud>();
                            for (int i = 0; i < totalProjectiles; i++)
                            {
                                Vector2 velocity = new Vector2(Main.rand.Next(-100, 101), Main.rand.Next(-100, 101));
                                velocity = velocity.SafeNormalize(Vector2.UnitY);
                                velocity *= Main.rand.Next(phase3 ? 300 : 100, 401) * 0.01f;

                                float maximumVelocityMult = death ? 0.75f : 0.5f;
                                if (expertMode)
                                    velocity *= 1f + (maximumVelocityMult * (0.5f - lifeRatio));

                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + velocity.SafeNormalize(Vector2.UnitY) * 5f, velocity, type, CloudDamage, 0f, Main.myPlayer);
                            }
                        }
                    }
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            float maxDistance = calamityGlobalNPC.newAI[0] == 1f ? 12800f : 6400f;
            if (player.dead || Vector2.Distance(NPC.Center, player.Center) > maxDistance || (nonHostile && biomeEnraged))
            {
                calamityGlobalNPC.newAI[1] = 1f;
                NPC.TargetClosest(false);
                NPC.velocity.Y += 2f;

                if (NPC.position.Y > Main.worldSurface * 16D)
                    NPC.velocity.Y += 2f;

                if (NPC.position.Y > Main.worldSurface * 16D)
                {
                    for (int a = 0; a < Main.npc.Length; a++)
                    {
                        int type = Main.npc[a].type;
                        if (CalamityNPCTypeSets.AquaticScourge.Contains(type))
                            Main.npc[a].active = false;
                    }
                }
            }
            else
                calamityGlobalNPC.newAI[1] = 0f;

            // Change direction
            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;

            // Alpha changes
            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            Vector2 scourgePosition = NPC.Center;
            Vector2 predictionVector = Main.getGoodWorld ? Main.player[NPC.target].velocity * 20f : Vector2.Zero;
            float scourgeTargetX = player.Center.X + predictionVector.X;
            float scourgeTargetY = player.Center.Y + predictionVector.Y;

            // Velocity and movement
            float scourgeMaxSpeed = 5f;
            float scourgeAcceleration = 0.08f;
            if (calamityGlobalNPC.newAI[0] == 1f)
            {
                scourgeMaxSpeed = revenge ? 14.4f : 12f;
                scourgeAcceleration = revenge ? 0.18f : 0.15f;
                if (expertMode)
                {
                    scourgeMaxSpeed += 2.4f * (1f - lifeRatio);
                    scourgeAcceleration += 0.03f * (1f - lifeRatio);
                }
                scourgeMaxSpeed += 3f * enrageScale;
                scourgeAcceleration += 0.06f * enrageScale;
                if (death || getFuckedAI)
                {
                    scourgeMaxSpeed += 5f;
                    scourgeAcceleration -= getFuckedAI ? 0f : 0.03f;
                    scourgeMaxSpeed += Vector2.Distance(player.Center, NPC.Center) * 0.001f;
                    scourgeAcceleration += Vector2.Distance(player.Center, NPC.Center) * 0.000045f;
                }

                // Increase acceleration after spiral attack
                if (NPC.localAI[3] > 0f)
                {
                    float accelerationMultiplier = MathHelper.Lerp(1f, 2f, NPC.localAI[3] / colorFadeTimeAfterSpiral);
                    scourgeAcceleration *= accelerationMultiplier;
                }

                if (Main.getGoodWorld)
                {
                    scourgeMaxSpeed *= 1.15f;
                    scourgeAcceleration *= 1.15f;
                }
            }

            if (!doSpiral)
            {
                if (calamityGlobalNPC.newAI[0] != 1f)
                {
                    scourgeTargetY += 400;
                    if (Math.Abs(NPC.Center.X - player.Center.X) < 500f)
                    {
                        if (NPC.velocity.X > 0f)
                            scourgeTargetX = player.Center.X + 600f;
                        else
                            scourgeTargetX = player.Center.X - 600f;
                    }
                }

                float scourgeHigherSpeed = scourgeMaxSpeed * 1.3f;
                float scourgeLowerSpeed = scourgeMaxSpeed * 0.7f;
                float scourgeSpeed = NPC.velocity.Length();
                if (scourgeSpeed > 0f)
                {
                    if (scourgeSpeed > scourgeHigherSpeed)
                    {
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY);
                        NPC.velocity *= scourgeHigherSpeed;
                    }
                    else if (scourgeSpeed < scourgeLowerSpeed)
                    {
                        NPC.velocity = NPC.velocity.SafeNormalize(Vector2.UnitY);
                        NPC.velocity *= scourgeLowerSpeed;
                    }
                }
            }

            scourgeTargetX = (int)(scourgeTargetX / 16f) * 16;
            scourgeTargetY = (int)(scourgeTargetY / 16f) * 16;
            scourgePosition.X = (int)(scourgePosition.X / 16f) * 16;
            scourgePosition.Y = (int)(scourgePosition.Y / 16f) * 16;
            scourgeTargetX -= scourgePosition.X;
            scourgeTargetY -= scourgePosition.Y;
            float scourgeTargetDist = (float)Math.Sqrt(scourgeTargetX * scourgeTargetX + scourgeTargetY * scourgeTargetY);

            if (!doSpiral)
            {
                float scourgeAbsoluteTargetX = Math.Abs(scourgeTargetX);
                float scourgeAbsoluteTargetY = Math.Abs(scourgeTargetY);
                float scourgeTimeToReachTarget = scourgeMaxSpeed / scourgeTargetDist;
                scourgeTargetX *= scourgeTimeToReachTarget;
                scourgeTargetY *= scourgeTimeToReachTarget;

                if ((NPC.velocity.X > 0f && scourgeTargetX > 0f) || (NPC.velocity.X < 0f && scourgeTargetX < 0f) || (NPC.velocity.Y > 0f && scourgeTargetY > 0f) || (NPC.velocity.Y < 0f && scourgeTargetY < 0f))
                {
                    if (NPC.velocity.X < scourgeTargetX)
                    {
                        NPC.velocity.X += scourgeAcceleration;
                    }
                    else
                    {
                        if (NPC.velocity.X > scourgeTargetX)
                            NPC.velocity.X -= scourgeAcceleration;
                    }

                    if (NPC.velocity.Y < scourgeTargetY)
                    {
                        NPC.velocity.Y += scourgeAcceleration;
                    }
                    else
                    {
                        if (NPC.velocity.Y > scourgeTargetY)
                            NPC.velocity.Y -= scourgeAcceleration;
                    }

                    if (Math.Abs(scourgeTargetY) < scourgeMaxSpeed * 0.2 && ((NPC.velocity.X > 0f && scourgeTargetX < 0f) || (NPC.velocity.X < 0f && scourgeTargetX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += scourgeAcceleration * 2f;
                        else
                            NPC.velocity.Y -= scourgeAcceleration * 2f;
                    }

                    if (Math.Abs(scourgeTargetX) < scourgeMaxSpeed * 0.2 && ((NPC.velocity.Y > 0f && scourgeTargetY < 0f) || (NPC.velocity.Y < 0f && scourgeTargetY > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += scourgeAcceleration * 2f;
                        else
                            NPC.velocity.X -= scourgeAcceleration * 2f;
                    }
                }
                else
                {
                    if (scourgeAbsoluteTargetX > scourgeAbsoluteTargetY)
                    {
                        if (NPC.velocity.X < scourgeTargetX)
                            NPC.velocity.X += scourgeAcceleration * 1.1f;
                        else if (NPC.velocity.X > scourgeTargetX)
                            NPC.velocity.X -= scourgeAcceleration * 1.1f;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < scourgeMaxSpeed * 0.5)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += scourgeAcceleration;
                            else
                                NPC.velocity.Y -= scourgeAcceleration;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < scourgeTargetY)
                            NPC.velocity.Y += scourgeAcceleration * 1.1f;
                        else if (NPC.velocity.Y > scourgeTargetY)
                            NPC.velocity.Y -= scourgeAcceleration * 1.1f;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < scourgeMaxSpeed * 0.5)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += scourgeAcceleration;
                            else
                                NPC.velocity.X -= scourgeAcceleration;
                        }
                    }
                }

                NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, NPC, drawColor, TextureAssets.Npc[Type].Value, TextureAssets.Npc[ModContent.NPCType<AquaticScourgeBody>()].Value, TextureAssets.Npc[ModContent.NPCType<AquaticScourgeBodyAlt>()].Value, 10, 12, 0.6f, new Vector2(20, 30), 3, 10);
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = TextureAssets.Npc[Type].Value;
            Vector2 scaledDraw = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width, texture2D15.Height) * NPC.scale / 2f;
            drawLocation += scaledDraw * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            Color color = NPC.GetAlpha(drawColor);

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive || Main.zenithWorld)
            {
                if (NPC.Calamity().newAI[3] > 300f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp((NPC.Calamity().newAI[3] - 300f) / 180f, 0f, 1f));
                else if (NPC.localAI[3] > 0f)
                    color = Color.Lerp(color, Color.SandyBrown, MathHelper.Clamp(NPC.localAI[3] / 90f, 0f, 1f));
            }

            spriteBatch.Draw(texture2D15, drawLocation, NPC.frame, color, NPC.rotation, scaledDraw, NPC.scale, spriteEffects, 0f);

            return false;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            Rectangle targetHitbox = target.Hitbox;

            float topLeftHitbox = Vector2.Distance(NPC.Center, targetHitbox.TopLeft());
            float topRightHitbox = Vector2.Distance(NPC.Center, targetHitbox.TopRight());
            float bottomLeftHitbox = Vector2.Distance(NPC.Center, targetHitbox.BottomLeft());
            float bottomRightHitbox = Vector2.Distance(NPC.Center, targetHitbox.BottomRight());

            float minDist = topLeftHitbox;
            if (topRightHitbox < minDist)
                minDist = topRightHitbox;
            if (bottomLeftHitbox < minDist)
                minDist = bottomLeftHitbox;
            if (bottomRightHitbox < minDist)
                minDist = bottomRightHitbox;

            return minDist <= 50f * NPC.scale;
        }

        public override bool? CanBeHitByProjectile(Projectile projectile)
        {
            if (projectile.minion && !projectile.Calamity().overridesMinionDamagePrevention)
                return NPC.Calamity().newAI[0] == 1f;

            return null;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.Calamity().disableNaturalScourgeSpawns)
                return 0f;

            if (spawnInfo.PlayerSafe)
                return 0f;

            if (spawnInfo.Player.Calamity().ZoneSulphur && spawnInfo.Water)
            {
                if (!NPC.AnyNPCs(ModContent.NPCType<AquaticScourgeHead>()))
                    return Main.zenithWorld ? 0.1f : 0.01f;
            }

            return 0f;
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ModContent.ItemType<SulphurousSand>();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<AquaticScourgeBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<SubmarineShocker>(),
                    ModContent.ItemType<Barinautical>(),
                    ModContent.ItemType<Downpour>(),
                    ModContent.ItemType<DeepseaStaff>(),
                    ModContent.ItemType<ScourgeoftheSeas>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Vanity
                normalOnly.Add(ModContent.ItemType<AquaticScourgeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AquaticEmblem>()));
                normalOnly.Add(ModContent.ItemType<CorrosiveSpine>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(ModContent.ItemType<SeasSearing>(), 10);
            }

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.GreaterHealingPotion, 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary
            npcLoot.Add(ModContent.ItemType<AquaticScourgeTrophy>(), 10);
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<AquaticScourgeRelic>());

            // GFB troll drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<SupremeBaitTackleBoxFishingStation>()), hideLootReport: true);

            // Lore
            bool firstASKill() => !DownedBossSystem.downedAquaticScourge;
            npcLoot.AddConditionalPerPlayer(firstASKill, ModContent.ItemType<LoreAquaticScourge>(), desc: DropHelper.FirstKillText);
            npcLoot.AddConditionalPerPlayer(firstASKill, ModContent.ItemType<LoreSulphurSea>(), desc: DropHelper.FirstKillText);
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If Aquatic Scourge has not yet been killed, notify players of buffed Acid Rain
            if (!DownedBossSystem.downedAquaticScourge)
            {
                if (!Main.LocalPlayer.dead && Main.LocalPlayer.active)
                    SoundEngine.PlaySound(Mauler.RoarSound, Main.LocalPlayer.Center);

                string sulfSeaBoostKey = "Mods.CalamityMod.Status.Progression.WetWormBossText";
                Color sulfSeaBoostColor = AcidRainEvent.TextColor;

                CalamityUtils.BroadcastLocalizedText(sulfSeaBoostKey, sulfSeaBoostColor);

                // Set a timer for acid rain to start after 10 seconds
                AcidRainEvent.CountdownUntilForcedAcidRain = 601;
            }

            // Mark Aquatic Scourge as dead
            DownedBossSystem.downedAquaticScourge = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 5; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                for (int k = 0; k < 15; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ASHead").Type, NPC.scale);
                }
            }
        }

        public override bool CheckActive()
        {
            if (NPC.Calamity().newAI[0] == 1f && !Main.player[NPC.target].dead && NPC.Calamity().newAI[1] != 1f)
                return false;

            return true;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<Irradiated>(), 480, true);
        }
    }
}
