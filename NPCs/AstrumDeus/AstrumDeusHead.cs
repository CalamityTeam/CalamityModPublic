using System;
using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
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
using CalamityMod.Packets;
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
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.AstrumDeus
{
    [AutoloadBossHead]
    [HasPierceResist]
    [LongDistanceNetSync]
    public class AstrumDeusHead : ModNPC
    {
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusSpawn");
        public static readonly SoundStyle LaserSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusLaser") { Volume = 0.35f };
        public static readonly SoundStyle GodRaySound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusGodRay") { Volume = 0.4f };
        public static readonly SoundStyle MineSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusMine") { Volume = 0.4f };
        public static readonly SoundStyle SplitSound = new("CalamityMod/Sounds/Custom/AstrumDeus/AstrumDeusSplit");
        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/AstrumDeusHit", 2) { Volume = 0.7f };
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/AstrumDeusDeath");

        public static Asset<Texture2D> TextureGlow1;
        public static Asset<Texture2D> TextureGlow2;
        public static Asset<Texture2D> TextureGlow3;
        public static Asset<Texture2D> TextureGlow4;
        public static Asset<Texture2D> TextureFlash2; // There is intentionally no flash texture for the eyes because I thought it looked weird

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.70f,
                PortraitScale = 0.75f
            };
            value.Position.X += 55f;
            value.Position.Y += 23f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                TextureGlow1 = ModContent.Request<Texture2D>(Texture + "Glow", AssetRequestMode.AsyncLoad);
                TextureGlow2 = ModContent.Request<Texture2D>(Texture + "Glow2", AssetRequestMode.AsyncLoad);
                TextureGlow3 = ModContent.Request<Texture2D>(Texture + "Glow3", AssetRequestMode.AsyncLoad);
                TextureGlow4 = ModContent.Request<Texture2D>(Texture + "Glow4", AssetRequestMode.AsyncLoad);
                TextureFlash2 = ModContent.Request<Texture2D>(Texture + "GlowFlash2", AssetRequestMode.AsyncLoad);
            }
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 120; // 240
            NPC.npcSlots = 5f;
            NPC.width = 56;
            NPC.height = 56;
            NPC.defense = 20;
            NPC.DR_NERD(0.1f);
            NPC.LifeMaxNERB(150000, 240000, 650000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;

            if (CalamityWorld.death || BossRushEvent.BossRushActive)
                NPC.scale *= 1.4f;
            else if (CalamityWorld.revenge)
                NPC.scale *= 1.35f;
            else if (Main.expertMode)
                NPC.scale *= 1.2f;

            NPC.boss = true;
            NPC.BossBar = ModContent.GetInstance<AstrumDeusBossBar>();
            NPC.value = Item.buyPrice(gold: 50);
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToSickness = false;
            SpawnModBiomes = new int[1] { ModContent.GetInstance<BiomeManagers.AstralInfectionBiome>().Type };

            if (Main.zenithWorld)
            {
                if (CalamityWorld.death) // killing 10 worms with half of the og's health is ridiculous
                    NPC.lifeMax /= 3;

                NPC.value /= 5;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.AstrumDeus")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.dontTakeDamage);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.dontTakeDamage = reader.ReadBoolean();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            CalamityGlobalNPC calamityGlobalNPC = NPC.Calamity();

            // Difficulty variables
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Deus cannot hit for 3 seconds or while invulnerable
            bool doNotDealDamage = calamityGlobalNPC.newAI[1] < 180f || NPC.dontTakeDamage;
            if (doNotDealDamage)
                NPC.damage = 0;
            else
                NPC.damage = NPC.defDamage;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool increaseSpeed = Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles;
            bool increaseSpeedMore = Vector2.Distance(player.Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance350Tiles;

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (increaseSpeedMore)
                NPC.TargetClosest();

            // Inflict Extreme Gravity to nearby players
            if (revenge)
            {
                if (!Main.dedServ)
                {
                    if (!Main.LocalPlayer.dead && Main.LocalPlayer.active && Vector2.Distance(Main.LocalPlayer.Center, NPC.Center) < CalamityGlobalNPC.CatchUpDistance350Tiles)
                        Main.LocalPlayer.AddBuff(ModContent.BuffType<DoGExtremeGravity>(), 2);
                }
            }

            // Life
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases based on life percentage
            bool halfHealth = lifeRatio < 0.5f;
            bool doubleWormPhase = calamityGlobalNPC.newAI[0] != 0f;
            bool startFlightPhase = lifeRatio < 0.8f || death || doubleWormPhase;
            bool phase2 = lifeRatio < 0.5f && doubleWormPhase && expertMode;
            bool phase3 = lifeRatio < 0.2f && doubleWormPhase && expertMode;
            bool splittingMines = lifeRatio < 0.7f;
            bool movingMines = lifeRatio < 0.3f && doubleWormPhase && expertMode;
            bool deathModeEnragePhase_Head = calamityGlobalNPC.newAI[0] == 3f;
            bool deathModeEnragePhase_BodyAndTail = false;

            // 5 seconds of resistance in phase 2, 10 seconds in phase 1, to prevent spawn killing
            float resistanceTime = doubleWormPhase ? 300f : 600f;

            calamityGlobalNPC.CurrentlyIncreasingDefenseOrDR = calamityGlobalNPC.newAI[1] < resistanceTime;

            // Flight timer
            float aiSwitchTimer = doubleWormPhase ? (Main.getGoodWorld ? 600f : 1200f) : (Main.getGoodWorld ? 900f : 1800f);

            calamityGlobalNPC.newAI[3] += 1f;
            if (calamityGlobalNPC.newAI[3] >= aiSwitchTimer)
                calamityGlobalNPC.newAI[3] = 0f;
            // Sound effect for swapping between attack behaviors in phase 2
            if (doubleWormPhase && calamityGlobalNPC.newAI[3] % aiSwitchTimer == 0f && !(deathModeEnragePhase_Head || deathModeEnragePhase_BodyAndTail))
                SoundEngine.PlaySound(SplitSound with { Pitch = -0.2f, Volume = 0.9f }, player.Center);

            // Phase for flying at the player
            bool flyAtTarget = calamityGlobalNPC.newAI[3] >= (aiSwitchTimer * 0.5f) && startFlightPhase;

            // Length of worms
            int phase1Length = death ? 80 : revenge ? 70 : expertMode ? 60 : 50;
            int phase2Length = death ? 40 : revenge ? 35 : expertMode ? 30 : 25;
            int gfbLength = death ? 8 : revenge ? 7 : expertMode ? 6 : 5;
            int maxLength = Main.zenithWorld && doubleWormPhase ? gfbLength : doubleWormPhase ? phase2Length : phase1Length;

            // Become gradually more pissed as more worms are killed
            int gfbMaxWormCount = 10;
            int gfbWormCount = 0;
            if (Main.zenithWorld)
                gfbWormCount = NPC.CountNPCS(ModContent.NPCType<AstrumDeusHead>());
            if (gfbWormCount > gfbMaxWormCount)
                gfbWormCount = gfbMaxWormCount;

            // Split into two worms
            float splitAnimationTime = 180f;
            bool oneWormAlive = NPC.CountNPCS(ModContent.NPCType<AstrumDeusHead>()) < 2;
            bool deathModeFinalWormEnrage = death && doubleWormPhase && oneWormAlive && calamityGlobalNPC.newAI[1] >= resistanceTime;
            if (deathModeFinalWormEnrage)
            {
                if (calamityGlobalNPC.newAI[0] != 3f)
                {
                    SoundEngine.PlaySound(SplitSound, player.Center);
                    calamityGlobalNPC.newAI[0] = 3f;
                    NPC.defense = 12;
                    calamityGlobalNPC.DR = 0.075f;

                    // Despawns the other deus worm segments
                    int bodyID = ModContent.NPCType<AstrumDeusBody>();
                    int tailID = ModContent.NPCType<AstrumDeusTail>();
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC wormseg = Main.npc[i];
                        if (!wormseg.active)
                            continue;

                        if ((wormseg.type == bodyID || wormseg.type == tailID) && Main.npc[(int)wormseg.ai[2]].Calamity().newAI[0] != 3f)
                        {
                            wormseg.life = 0;
                            wormseg.active = false;
                        }
                    }
                }

                calamityGlobalNPC.newAI[2] += 10f;
                if (calamityGlobalNPC.newAI[2] > 162f)
                    calamityGlobalNPC.newAI[2] = 162f;

                NPC.Opacity = MathHelper.Clamp(1f - (calamityGlobalNPC.newAI[2] / splitAnimationTime), 0f, 1f);
            }

            bool despawnRemainingWorm = doubleWormPhase && oneWormAlive && !death;
            if ((halfHealth && calamityGlobalNPC.newAI[0] == 0f) || despawnRemainingWorm)
            {
                NPC.dontTakeDamage = true;

                calamityGlobalNPC.newAI[2] += despawnRemainingWorm ? 10f : 1f;
                NPC.Opacity = MathHelper.Clamp(1f - (calamityGlobalNPC.newAI[2] / splitAnimationTime), 0f, 1f);

                bool despawning = calamityGlobalNPC.newAI[2] == splitAnimationTime;

                //
                // CODE TWEAKED BY: OZZATRON
                // September 20th, 2020
                // reason: fixing Astrum Deus death mode NPC cap bug
                //

                // Despawn the unsplit Astrum Deus and spawn two half-size Astrum Deus worms.
                if (despawning)
                {
                    // If this is already a split worm (newAI[0] != 0f) then don't do anything. At all.
                    if (doubleWormPhase)
                        return;

                    // Mark all existing body and tail segments as inactive. This instantly frees up their NPC slots for the freshly spawned worms.
                    int bodyID = ModContent.NPCType<AstrumDeusBody>();
                    int tailID = ModContent.NPCType<AstrumDeusTail>();
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        NPC wormseg = Main.npc[i];
                        if (!wormseg.active)
                            continue;
                        if (wormseg.type == bodyID || wormseg.type == tailID)
                        {
                            wormseg.life = 0;
                            wormseg.active = false;
                        }
                    }

                    // The unsplit Astrum Deus worm head will die next frame.
                    NPC.life = 0;

                    // Do not spawn worms client side. The server handles this.
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int wormamt = Main.zenithWorld ? 5 : 1;
                        for (int i = 0; i < wormamt; i++)
                        {
                            // Now that the original worm doesn't exist, startCount can be zero.
                            // int startCount = NPC.whoAmI + phase1Length + 1;
                            int startIndexHeadOne = 1;
                            int headOneID = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPC.type, startIndexHeadOne);
                            Main.npc[headOneID].Calamity().newAI[0] = 1f;
                            Main.npc[headOneID].velocity = Vector2.Normalize(player.Center - Main.npc[headOneID].Center) * 16f;
                            Main.npc[headOneID].timeLeft *= 20;
                            Main.npc[headOneID].ForceNetUpdate();

                            // On server, immediately send the correct extra AI of this head to clients.
                            if (Main.dedServ)
                            {
                                SyncCalamityNPCAIArrayPacket.Send(Main.npc[headOneID]);
                            }

                            // Make sure the second split worm is also contiguous.
                            int startIndexHeadTwo = startIndexHeadOne + phase2Length + 1;
                            int headTwoID = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPC.type, startIndexHeadTwo);
                            Main.npc[headTwoID].Calamity().newAI[0] = 2f;
                            Main.npc[headTwoID].Calamity().newAI[3] = Main.getGoodWorld ? 300f : 600f;
                            Main.npc[headTwoID].velocity = Vector2.Normalize(player.Center - Main.npc[headTwoID].Center) * 16f;
                            Main.npc[headTwoID].timeLeft *= 20;
                            Main.npc[headTwoID].ForceNetUpdate();

                            // On server, immediately send the correct extra AI of this head to clients.
                            if (Main.dedServ)
                            {
                                SyncCalamityNPCAIArrayPacket.Send(Main.npc[headTwoID]);
                            }
                        }

                        SoundEngine.PlaySound(SplitSound, player.Center);
                    }
                    return;
                }
            }

            if (Main.zenithWorld && calamityGlobalNPC.newAI[1] < 10f) // desync the deuses
            {
                float pushForce = 0.25f;
                for (int k = 0; k < Main.maxNPCs; k++)
                {
                    NPC otherDeus = Main.npc[k];
                    // Short circuits to make the loop as fast as possible
                    if (!otherDeus.active || k == NPC.whoAmI)
                        continue;

                    // If the other projectile is indeed the same owned by the same player and they're too close, nudge them away.
                    bool sameProjType = otherDeus.type == NPC.type;
                    float taxicabDist = Vector2.Distance(NPC.Center, otherDeus.Center);
                    float distancegate = 320f;
                    if (sameProjType && taxicabDist < distancegate)
                    {
                        if (NPC.position.X < otherDeus.position.X)
                            NPC.velocity.X -= pushForce;
                        else
                            NPC.velocity.X += pushForce;

                        if (NPC.position.Y < otherDeus.position.Y)
                            NPC.velocity.Y -= pushForce;
                        else
                            NPC.velocity.Y += pushForce;
                    }
                }
            }

            // Set worm variable
            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            // Alpha effects
            NPC.alpha -= 42;
            if (NPC.alpha < 0)
                NPC.alpha = 0;

            // Direction
            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = -1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = 1;

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[0] == 0f)
                {
                    int Previous = NPC.whoAmI;
                    int bodyType = ModContent.NPCType<AstrumDeusBody>();
                    int tailType = ModContent.NPCType<AstrumDeusTail>();
                    for (int segments = 0; segments < maxLength; segments++)
                    {
                        int lol;
                        
                        // Spawn offsets prevent the body segments from spawning bunched on top of each other, which is exploitable
                        if (segments >= 0 && segments < maxLength - 1)
                        {
                            Vector2 bodySpawnOffset = NPC.velocity.SafeNormalize(Vector2.UnitY) * NPC.height * (segments + 1) / 2;
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - bodySpawnOffset.X), (int)(NPC.Center.Y - bodySpawnOffset.Y), bodyType, NPC.whoAmI);
                        }
                        else
                        {
                            Vector2 tailSpawnOffset = NPC.velocity.SafeNormalize(Vector2.UnitY) * NPC.height * maxLength / 2;
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(NPC.Center.X - tailSpawnOffset.X), (int)(NPC.Center.Y - tailSpawnOffset.Y), tailType, NPC.whoAmI);
                        }

                        if (segments % 2 == 0)
                            Main.npc[lol].localAI[3] = 1f;

                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].Calamity().newAI[0] = Main.npc[Previous].Calamity().newAI[0];
                        Main.npc[lol].Calamity().newAI[3] = Main.npc[Previous].Calamity().newAI[3];

                        if (Main.dedServ)
                        {
                            SyncCalamityNPCAIArrayPacket.Send(Main.npc[lol]);
                        }

                        Main.npc[lol].ai[3] = segments + 1;
                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].ai[1] = Previous;
                        Main.npc[Previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol);
                        Previous = lol;
                    }
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            bool hasJustSpawned = calamityGlobalNPC.newAI[1] < resistanceTime * 0.4f && !doubleWormPhase; // Speed boost for the first 4 seconds after spawning
            float segmentVelocity = hasJustSpawned ? 25f : deathModeEnragePhase_Head ? 19f : death ? 17.5f : 16f;

            float segmentVelocityBoost = 5f * (1f - lifeRatio);
            segmentVelocity += segmentVelocityBoost;
            if (gfbWormCount > 0)
                segmentVelocity += (gfbMaxWormCount - gfbWormCount) * 0.444f;

            if (revenge)
            {
                float revMultiplier = 1.1f;
                segmentVelocity *= revMultiplier;
            }

            int headTilePositionX = (int)(NPC.position.X / 16f) - 1;
            int headTileWidthPosX = (int)((NPC.position.X + NPC.width) / 16f) + 2;
            int headTilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int headTileWidthPosY = (int)((NPC.position.Y + NPC.height) / 16f) + 2;

            if (headTilePositionX < 0)
                headTilePositionX = 0;
            if (headTileWidthPosX > Main.maxTilesX)
                headTileWidthPosX = Main.maxTilesX;
            if (headTilePositionY < 0)
                headTilePositionY = 0;
            if (headTileWidthPosY > Main.maxTilesY)
                headTileWidthPosY = Main.maxTilesY;

            // Fly or not
            bool shouldFly = flyAtTarget;
            if (!shouldFly)
            {
                for (int k = headTilePositionX; k < headTileWidthPosX; k++)
                {
                    for (int l = headTilePositionY; l < headTileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[Main.tile[k, l].TileType] || (Main.tileSolidTop[Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = k * 16f;
                            vector2.Y = l * 16f;
                            if (NPC.position.X + NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            // Start flying if target is not within a certain distance
            if (!shouldFly)
            {
                NPC.localAI[1] = 1f;

                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int noFlyZone = 200;
                int heightReduction = death ? 130 : (int)(130f * (1f - lifeRatio));
                int height = 400 - heightReduction;
                bool outsideNoFlyZone = true;

                if (NPC.position.Y > player.position.Y)
                {
                    for (int m = 0; m < Main.maxPlayers; m++)
                    {
                        if (Main.player[m].active)
                        {
                            Rectangle rectangle2 = new Rectangle((int)Main.player[m].position.X - noFlyZone, (int)Main.player[m].position.Y - noFlyZone, noFlyZone * 2, height);
                            if (rectangle.Intersects(rectangle2))
                            {
                                outsideNoFlyZone = false;
                                break;
                            }
                        }
                    }

                    if (outsideNoFlyZone)
                        shouldFly = true;
                }
            }
            else
                NPC.localAI[1] = 0f;

            // Despawn
            if (player.dead)
            {
                shouldFly = false;
                float velocity = 2f;
                NPC.velocity.Y -= velocity;
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    segmentVelocity *= 2f;
                    NPC.velocity.Y -= velocity;
                }

                int headType = ModContent.NPCType<AstrumDeusHead>();
                int bodyType = ModContent.NPCType<AstrumDeusBody>();
                int tailType = ModContent.NPCType<AstrumDeusBody>();
                if ((double)NPC.position.Y < Main.topWorld + 16f)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].type == headType || Main.npc[i].type == bodyType || Main.npc[i].type == tailType)
                        {
                            Main.npc[i].active = false;
                            Main.npc[i].ForceNetUpdate(false);
                        }
                    }
                }
            }

            // Speed and movement
            float speedBoost = death ? (0.1f * (1f - lifeRatio)) : (0.13f * (1f - lifeRatio));
            float turnSpeedBoost = death ? (0.18f * (1f - lifeRatio)) : (0.2f * (1f - lifeRatio));
            float speed = (hasJustSpawned ? 0.26f : deathModeEnragePhase_Head ? 0.2f : death ? 0.18f : 0.13f) + speedBoost;
            float turnSpeed = (hasJustSpawned ? 0.3f : deathModeEnragePhase_Head ? 0.27f : death ? 0.25f : 0.2f) + turnSpeedBoost;
            if (gfbWormCount > 0)
            {
                speed += (gfbMaxWormCount - gfbWormCount) * 0.00555f;
                turnSpeed += (gfbMaxWormCount - gfbWormCount) * 0.00888f;
            }

            if (flyAtTarget)
            {
                float speedMultiplier = deathModeEnragePhase_Head ? 1.25f : doubleWormPhase ? (phase3 ? 1.25f : phase2 ? 1.2f : 1.15f) : 1f;
                speed *= speedMultiplier;
            }

            if (revenge)
            {
                float revMultiplier = 1.1f;
                speed *= revMultiplier;
                turnSpeed *= revMultiplier;
            }

            speed *= increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;
            turnSpeed *= increaseSpeedMore ? 2f : increaseSpeed ? 1.5f : 1f;

            if (Main.getGoodWorld)
            {
                speed *= 1.15f;
                turnSpeed *= 1.15f;
            }

            Vector2 deusCenter = NPC.Center;
            float deusTargetX = player.Center.X;
            float deusTargetY = player.Center.Y;
            deusTargetX = (int)(deusTargetX / 16f) * 16;
            deusTargetY = (int)(deusTargetY / 16f) * 16;
            deusCenter.X = (int)(deusCenter.X / 16f) * 16;
            deusCenter.Y = (int)(deusCenter.Y / 16f) * 16;
            deusTargetX -= deusCenter.X;
            deusTargetY -= deusCenter.Y;

            if (!shouldFly)
            {
                NPC.velocity.Y += 0.15f;
                if (NPC.velocity.Y > segmentVelocity)
                    NPC.velocity.Y = segmentVelocity;

                // This bool exists to stop the strange wiggle behavior when worms are falling down
                bool slowXVelocity = Math.Abs(NPC.velocity.X) > speed;
                if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= speed * 1.1f;
                    else
                        NPC.velocity.X += speed * 1.1f;
                }
                else if (NPC.velocity.Y == segmentVelocity)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < deusTargetX)
                            NPC.velocity.X += speed;
                        else if (NPC.velocity.X > deusTargetX)
                            NPC.velocity.X -= speed;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
                else if (NPC.velocity.Y > 4f)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < 0f)
                            NPC.velocity.X += speed * 0.9f;
                        else
                            NPC.velocity.X -= speed * 0.9f;
                    }
                    else
                        NPC.velocity.X = 0f;
                }
            }
            else
            {
                float deusTargetDist = (float)Math.Sqrt(deusTargetX * deusTargetX + deusTargetY * deusTargetY);
                float deusAbsoluteTargetX = Math.Abs(deusTargetX);
                float deusAbsoluteTargetY = Math.Abs(deusTargetY);
                float deusTimeToReachTarget = segmentVelocity / deusTargetDist;
                deusTargetX *= deusTimeToReachTarget;
                deusTargetY *= deusTimeToReachTarget;

                bool speedUpWhileFlying = false;
                if (flyAtTarget)
                {
                    if (((NPC.velocity.X > 0f && deusTargetX < 0f) || (NPC.velocity.X < 0f && deusTargetX > 0f) || (NPC.velocity.Y > 0f && deusTargetY < 0f) || (NPC.velocity.Y < 0f && deusTargetY > 0f)) && Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) > speed / 2f && deusTargetDist < 400f)
                    {
                        speedUpWhileFlying = true;

                        if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < segmentVelocity)
                            NPC.velocity *= 1.1f;
                    }

                    if (NPC.position.Y > player.position.Y)
                    {
                        speedUpWhileFlying = true;

                        if (Math.Abs(NPC.velocity.X) < segmentVelocity / 2f)
                        {
                            if (NPC.velocity.X == 0f)
                                NPC.velocity.X -= NPC.direction;

                            NPC.velocity.X *= 1.1f;
                        }
                        else if (NPC.velocity.Y > -segmentVelocity)
                            NPC.velocity.Y -= speed;
                    }
                }

                if (!speedUpWhileFlying)
                {
                    if (!flyAtTarget)
                    {
                        if (((NPC.velocity.X > 0f && deusTargetX > 0f) || (NPC.velocity.X < 0f && deusTargetX < 0f)) && ((NPC.velocity.Y > 0f && deusTargetY > 0f) || (NPC.velocity.Y < 0f && deusTargetY < 0f)))
                        {
                            if (NPC.velocity.X < deusTargetX)
                                NPC.velocity.X += turnSpeed;
                            else if (NPC.velocity.X > deusTargetX)
                                NPC.velocity.X -= turnSpeed;

                            if (NPC.velocity.Y < deusTargetY)
                                NPC.velocity.Y += turnSpeed;
                            else if (NPC.velocity.Y > deusTargetY)
                                NPC.velocity.Y -= turnSpeed;
                        }
                    }

                    if ((NPC.velocity.X > 0f && deusTargetX > 0f) || (NPC.velocity.X < 0f && deusTargetX < 0f) || (NPC.velocity.Y > 0f && deusTargetY > 0f) || (NPC.velocity.Y < 0f && deusTargetY < 0f))
                    {
                        if (NPC.velocity.X < deusTargetX)
                            NPC.velocity.X += speed;
                        else if (NPC.velocity.X > deusTargetX)
                            NPC.velocity.X -= speed;

                        if (NPC.velocity.Y < deusTargetY)
                            NPC.velocity.Y += speed;
                        else if (NPC.velocity.Y > deusTargetY)
                            NPC.velocity.Y -= speed;

                        if (Math.Abs(deusTargetY) < segmentVelocity * 0.2 && ((NPC.velocity.X > 0f && deusTargetX < 0f) || (NPC.velocity.X < 0f && deusTargetX > 0f)))
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += speed * 2f;
                            else
                                NPC.velocity.Y -= speed * 2f;
                        }

                        if (Math.Abs(deusTargetX) < segmentVelocity * 0.2 && ((NPC.velocity.Y > 0f && deusTargetY < 0f) || (NPC.velocity.Y < 0f && deusTargetY > 0f)))
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += speed * 2f;
                            else
                                NPC.velocity.X -= speed * 2f;
                        }
                    }
                    else if (deusAbsoluteTargetX > deusAbsoluteTargetY)
                    {
                        if (NPC.velocity.X < deusTargetX)
                            NPC.velocity.X += speed * 1.1f;
                        else if (NPC.velocity.X > deusTargetX)
                            NPC.velocity.X -= speed * 1.1f;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.5)
                        {
                            if (NPC.velocity.Y > 0f)
                                NPC.velocity.Y += speed;
                            else
                                NPC.velocity.Y -= speed;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < deusTargetY)
                            NPC.velocity.Y += speed * 1.1f;
                        else if (NPC.velocity.Y > deusTargetY)
                            NPC.velocity.Y -= speed * 1.1f;

                        if ((Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < segmentVelocity * 0.5)
                        {
                            if (NPC.velocity.X > 0f)
                                NPC.velocity.X += speed;
                            else
                                NPC.velocity.X -= speed;
                        }
                    }
                }
            }

            if (shouldFly)
            {
                if (NPC.localAI[0] != 1f)
                    NPC.ForceNetUpdate(false);

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                    NPC.ForceNetUpdate(false);

                NPC.localAI[0] = 0f;
            }

            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                NPC.ForceNetUpdate(false);

            NPC.rotation = (float)Math.Atan2(NPC.velocity.Y, NPC.velocity.X) + MathHelper.PiOver2;

            // 5 seconds of resistance in phase 2, 10 seconds in phase 1, to prevent spawn killing
            if (calamityGlobalNPC.newAI[1] < resistanceTime && ((NPC.position - NPC.oldPosition).Length() > 2f || calamityGlobalNPC.newAI[1] > 1f))
                calamityGlobalNPC.newAI[1] += 1f;
        }

        public override bool CheckActive()
        {
            return false;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                NPC.Opacity = 1f;
                return CalamityUtils.DrawAnimatedBestiaryWorm(spriteBatch, NPC, drawColor, TextureAssets.Npc[Type].Value, TextureAssets.Npc[ModContent.NPCType<AstrumDeusBody>()].Value, AstrumDeusBody.AltTexture.Value, 7, 26, 0.3f, new Vector2(0, 10), 5, 10, 4);
            }

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            bool deathModeEnragePhase = NPC.Calamity().newAI[0] == 3f;
            bool doubleWormPhase = NPC.Calamity().newAI[0] != 0f && !deathModeEnragePhase;

            float cyanThreshold = Main.getGoodWorld ? 300f : 600f;
            // Head is always the last segment to visually transition
            float transitionStart = cyanThreshold * 0.95f;
            bool drawCyan = NPC.Calamity().newAI[3] >= cyanThreshold;
            bool inColorTrans = doubleWormPhase && NPC.Calamity().newAI[3] % cyanThreshold >= transitionStart;

            Texture2D mainWormTex = TextureAssets.Npc[Type].Value;
            Texture2D secondWormTex = TextureGlow2.Value;
            Texture2D otherMainTex, otherSecondTex;
            Vector2 halfSizeTex = new Vector2(TextureAssets.Npc[Type].Value.Width / 2, TextureAssets.Npc[Type].Value.Height / 2);

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(mainWormTex.Width, mainWormTex.Height) * NPC.scale / 2f;
            drawLocation += halfSizeTex * NPC.scale + new Vector2(0f, NPC.gfxOffY);
            spriteBatch.Draw(mainWormTex, drawLocation, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);

            mainWormTex = TextureGlow1.Value;
            float colorOpacity = Utils.GetLerpValue(transitionStart, cyanThreshold, NPC.Calamity().newAI[3] % cyanThreshold, true);
            Color phaseColor = drawCyan ? Color.Cyan : Color.Orange;
            Color otherPhaseColor = drawCyan ? Color.Orange : Color.Cyan;
            if (doubleWormPhase) // otherMainTex and otherSecondTex contain the opposite texture, and are faded in during the transition
            {
                mainWormTex = drawCyan ? mainWormTex : TextureGlow3.Value;
                otherMainTex = drawCyan ? TextureGlow3.Value : mainWormTex;
                secondWormTex = drawCyan ? TextureGlow4.Value : secondWormTex;
                otherSecondTex = drawCyan ? secondWormTex : TextureGlow4.Value;
            }
            else
            {
                otherMainTex = mainWormTex;
                otherSecondTex = secondWormTex;
            }
            Color mainWormColorLerp = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Cyan, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);
            Color secondWormColorLerp = Color.Lerp(Color.White, doubleWormPhase ? phaseColor : Color.Orange, 0.5f) * (deathModeEnragePhase ? 1f : NPC.Opacity);

            int timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 1 : 2;
            for (int i = 0; i < timesToDraw; i++)
            {
                spriteBatch.Draw(mainWormTex, drawLocation, NPC.frame, mainWormColorLerp * (inColorTrans ? 1f - colorOpacity : 1f), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
                // Controls drawing the new fading in glowmask for the upcoming behavior
                if (inColorTrans)
                    spriteBatch.Draw(otherMainTex, drawLocation, NPC.frame, Color.Lerp(Color.White, otherPhaseColor, 0.5f) * colorOpacity, NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
                // Eyes intentionally do not flash
            }

            timesToDraw = deathModeEnragePhase ? 3 : drawCyan ? 2 : 1;
            for (int i = 0; i < timesToDraw; i++)
            {
                spriteBatch.Draw(secondWormTex, drawLocation, NPC.frame, secondWormColorLerp * (inColorTrans ? 1f - colorOpacity : 1f), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
                // Controls drawing the new fading in glowmask for the upcoming behavior
                if (inColorTrans)
                    spriteBatch.Draw(otherSecondTex, drawLocation, NPC.frame, Color.Lerp(Color.White, otherPhaseColor, 0.5f) * colorOpacity, NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
                // Controls drawing the white flash immediately after swapping behaviors
                if (doubleWormPhase && NPC.Calamity().newAI[3] % cyanThreshold < 25f)
                    spriteBatch.Draw(TextureFlash2.Value, drawLocation, NPC.frame, Color.White * MathHelper.Lerp(1f, 0f, NPC.Calamity().newAI[3] % cyanThreshold / 25f), NPC.rotation, halfSizeTex, NPC.scale, spriteEffects, 0f);
            }
            return false;
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (!Main.zenithWorld && Main.rand.NextBool(5)) // I value people's computers
                {
                    NPC.position.X = NPC.position.X + (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                    NPC.width = 50;
                    NPC.height = 50;
                    NPC.position.X = NPC.position.X - (NPC.width / 2);
                    NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                    for (int i = 0; i < 5; i++)
                    {
                        int purpleDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, (int)CalamityDusts.PurpleCosmilite, 0f, 0f, 100, default, 2f);
                        Main.dust[purpleDust].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[purpleDust].scale = 0.5f;
                            Main.dust[purpleDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int j = 0; j < 10; j++)
                    {
                        int astralDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 3f);
                        Main.dust[astralDust].noGravity = true;
                        Main.dust[astralDust].velocity *= 5f;
                        astralDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<AstralOrange>(), 0f, 0f, 100, default, 2f);
                        Main.dust[astralDust].velocity *= 2f;
                    }
                    if (!Main.dedServ)
                    {
                        float randomSpread = Main.rand.Next(-200, 201) / 100f;
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("AstrumDeusHead1").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("AstrumDeusHead2").Type, 1f);
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity * randomSpread, Mod.Find<ModGore>("AstrumDeusHead3").Type, 1f);
                    }
                }
            }
        }

        public override void BossLoot(ref int potionType) => potionType = ModContent.ItemType<StarblightSoot>();

        public static bool ShouldNotDropThings(NPC NPC) => NPC.Calamity().newAI[0] == 0f || ((CalamityWorld.death || BossRushEvent.BossRushActive) && NPC.Calamity().newAI[0] != 3f);

        public override bool SpecialOnKill()
        {
            if (ShouldNotDropThings(NPC))
                return false;

            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<AstrumDeusHead>(),
                ModContent.NPCType<AstrumDeusBody>(),
                ModContent.NPCType<AstrumDeusTail>());
            NPC.position = Main.npc[closestSegmentID].position;

            return false;
        }

        public override void OnKill()
        {
            if (ShouldNotDropThings(NPC))
                return;

            // Killing ANY split Deus makes all other Deus heads die immediately.
            foreach (NPC otherWormHead in Main.ActiveNPCs)
            {
                if (otherWormHead.type == NPC.type)
                {
                    // Kill the other worm head after setting it to not drop loot.
                    otherWormHead.Calamity().newAI[0] = 0f;
                    otherWormHead.life = 0;
                    otherWormHead.checkDead();
                    otherWormHead.netUpdate = true;
                }
            }

            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Notify players that Astral Ore can be mined if Deus has never been killed yet
            if (!DownedBossSystem.downedAstrumDeus)
            {
                string key = "Mods.CalamityMod.Status.Progression.AstralBossText";
                Color messageColor = Color.Gold;
                CalamityUtils.BroadcastLocalizedText(key, messageColor);
            }

            // Mark Astrum Deus as dead
            DownedBossSystem.downedAstrumDeus = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var lastWorm = npcLoot.DefineConditionalDropSet(info => !ShouldNotDropThings(info.npc));
            lastWorm.Add(ItemDropRule.BossBag(ModContent.ItemType<AstrumDeusBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = new LeadingConditionRule(new Conditions.NotExpert());
            lastWorm.Add(normalOnly);
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<TheMicrowave>(),
                    ModContent.ItemType<StarSputter>(),
                    ModContent.ItemType<StarShower>(),
                    ModContent.ItemType<StarspawnHelixStaff>(),
                    ModContent.ItemType<RegulusRiot>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Vanity
                normalOnly.Add(ModContent.ItemType<AstrumDeusMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<HideofAstrumDeus>()));
                normalOnly.Add(ModContent.ItemType<ChromaticOrb>(), 5);

                // Materials
                normalOnly.Add(ItemID.FallenStar, 1, 25, 40);
                normalOnly.Add(ModContent.ItemType<StarblightSoot>(), 1, 50, 80);
            }

            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.SuperHealingPotion, 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary
            lastWorm.Add(ModContent.ItemType<AstrumDeusTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).AddIf((info) => !ShouldNotDropThings(info.npc), ModContent.ItemType<AstrumDeusRelic>());

            // Fragments
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentSolar, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentVortex, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentNebula, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ItemID.FragmentStardust, 1, 16, 24, 20, 32));
            lastWorm.Add(DropHelper.NormalVsExpertQuantity(ModContent.ItemType<MeldBlob>(), 1, 16, 24, 20, 32));

            // GFB Worm and Spaghetti drop
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.Worm, 1, 1, 9999), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.CanOfWorms, 1, 1, 9999), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.GummyWorm, 1, 1, 9999), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.TruffleWorm, 1, 1, 9999), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.EnchantedNightcrawler, 1, 1, 9999), true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.Spaghetti, 1, 1, 9999), true);
            }

            // Lore
            bool firstDeusKill(DropAttemptInfo info) => !DownedBossSystem.downedAstrumDeus && !ShouldNotDropThings(info.npc);
            npcLoot.AddConditionalPerPlayer(firstDeusKill, ModContent.ItemType<LoreAstrumDeus>(), desc: DropHelper.FirstKillText);
            npcLoot.AddConditionalPerPlayer(firstDeusKill, ModContent.ItemType<LoreAstralInfection>(), desc: DropHelper.FirstKillText);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
                target.AddBuff(ModContent.BuffType<AstralInfectionDebuff>(), 360);
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }
}
