using System;
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
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.NPCs.DesertScourge
{
    [AutoloadBossHead]
    [HasPierceResist]
    [LongDistanceNetSync]
    public class DesertScourgeHead : ModNPC
    {
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private bool tailSpawned = false;
        public bool playRoarSound = false;

        public const float SegmentVelocity_Normal = 10f;
        public const float SegmentVelocity_Expert = 12.5f;
        public const float SegmentVelocity_Death = 15f;
        public const float SegmentVelocity_GoodWorld = 21f;
        public const float SegmentVelocity_ZenithSeed = 24f;

        public const float SpitGateValue = 300f;
        public const float SpitGateValue_Death = 180f;

        public const float BurrowTimeGateValue = 600f;
        public const float BurrowResetTimeGateValue = BurrowTimeGateValue + 600f;

        public const float LungeUpwardDistanceOffset = 600f;
        public const float LungeUpwardCutoffDistance = 420f;
        public const float BurrowDistance_Hide = 1080f;
        public const float BurrowDistance = 800f;
        public const float OpenMouthForBiteDistance = 220f;

        private const int OpenMouthStopFrame = 4;

        public static readonly SoundStyle HitSound = new("CalamityMod/Sounds/NPCHit/DesertScourgeHit", 3);
        public static readonly SoundStyle DeathSound = new("CalamityMod/Sounds/NPCKilled/DesertScourgeDeath");
        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/DesertScourge/DesertScourgeRoar");
        public static readonly SoundStyle SandBlastSound = new("CalamityMod/Sounds/Custom/DesertScourge/DesertScourgeSandBlast");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 7;

            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.65f,
                PortraitScale = 0.7f,
                PortraitPositionXOverride = 0,
                PortraitPositionYOverride = 0
            };
            value.Position.X += 20;
            value.Position.Y -= 15;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public static int SpitDamage = 10; // 40

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 40; // 64 (1.6x expert scaling)
            NPC.defense = 4;
            NPC.npcSlots = 12f;
            NPC.width = 104;
            NPC.height = 104;

            NPC.LifeMaxNERB(4000, 5000, 1150000);
            if (Main.getGoodWorld)
                NPC.lifeMax *= 2;
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.value = Item.buyPrice(gold: 1);
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = HitSound;
            NPC.DeathSound = DeathSound;
            NPC.netAlways = true;

            if (Main.getGoodWorld)
                NPC.scale *= 0.4f;
            if (Main.zenithWorld)
                NPC.scale *= 4f;

            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
            NPC.Calamity().VulnerableToWater = true;
        }

        public override void BossHeadSlot(ref int index)
        {
            if ((NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()) || NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHeadYoung>())))
                index = -1;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Desert,
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.DesertScourge")
            });
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.alpha);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(biomeEnrageTimer);
            writer.Write(playRoarSound);
            writer.Write(NPC.localAI[2]);
            writer.Write(NPC.localAI[3]);
            for (int i = 0; i < 4; i++)
                writer.Write(NPC.Calamity().newAI[i]);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.alpha = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            biomeEnrageTimer = reader.ReadInt32();
            playRoarSound = reader.ReadBoolean();
            NPC.localAI[2] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            for (int i = 0; i < 4; i++)
                NPC.Calamity().newAI[i] = reader.ReadSingle();
        }

        public override void AI()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Check for Nuisances
            bool hide = (NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHead>()) || NPC.AnyNPCs(ModContent.NPCType<DesertNuisanceHeadYoung>()));
            if (hide)
            {
                NPC.Calamity().newAI[0] = 0f;
                NPC.Calamity().newAI[1] = 0f;
                NPC.Calamity().newAI[3] = 0f;
                NPC.localAI[3] = 0f;
                playRoarSound = false;
            }

            NPC.dontTakeDamage = hide;
            NPC.canDisplayBuffs = !hide;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            // Enrage
            if (!player.ZoneDesert && !BossRushEvent.BossRushActive)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0;

            float enrageScale = 0f;
            if (biomeEnraged)
            {
                NPC.Calamity().CurrentlyEnraged = true;
                enrageScale += 2f;
            }

            // Percent life remaining.
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phases
            bool phase2 = lifeRatio < 0.5f;

            // Summon the Nuisances.
            if (phase2 && expertMode)
            {
                if (NPC.localAI[2] == 0f)
                {
                    NPC.localAI[2] = 1f;
                    NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<DesertNuisanceHead>());
                    NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<DesertNuisanceHeadYoung>());
                }
            }

            // Only increment the burrow timer if the head is beneath the player, OR if the X distance is greater than a specified value.
            if (NPC.Center.Y > Main.player[NPC.target].Center.Y || Math.Abs(NPC.Center.X - player.Center.X) > 480f)
            {
                if (revenge || lifeRatio < (expertMode ? 0.75f : 0.5f))
                    NPC.Calamity().newAI[0] += 1f;
            }

            bool burrow = NPC.Calamity().newAI[0] >= BurrowTimeGateValue;
            bool resetTime = NPC.Calamity().newAI[0] >= BurrowResetTimeGateValue;
            bool lungeUpward = burrow && NPC.Calamity().newAI[1] == 1f;
            bool quickFall = NPC.Calamity().newAI[1] == 2f;

            float burrowDistance = hide ? BurrowDistance_Hide : BurrowDistance;

            float speed = death ? 0.105f : 0.085f;
            float turnSpeed = death ? 0.21f : 0.17f;

            if (expertMode)
            {
                speed += speed * 0.4f * (1f - lifeRatio);
                turnSpeed += turnSpeed * 0.4f * (1f - lifeRatio);
            }

            if (revenge)
            {
                speed += (death ? 0.03f : 0.02f) * (1f - lifeRatio);
                turnSpeed += (death ? 0.06f : 0.04f) * (1f - lifeRatio);
            }

            speed += 0.085f * enrageScale;
            turnSpeed += 0.17f * enrageScale;

            if (Main.getGoodWorld)
            {
                speed *= 1.1f;
                turnSpeed *= 1.2f;
            }

            // Sand splash
            if (!quickFall)
            {
                if (lungeUpward)
                {
                    if (NPC.localAI[3] == 0f)
                    {
                        Point headTileCenter = NPC.Top.ToTileCoordinates();
                        Tile tileSafely = Framing.GetTileSafely(headTileCenter);
                        bool inSolidTile = tileSafely.HasUnactuatedTile;
                        bool finsInSolidTile = Framing.GetTileSafely(Main.npc[(int)NPC.ai[0]].Center.ToTileCoordinates()).HasUnactuatedTile;
                        if (!inSolidTile && finsInSolidTile && Collision.CanHit(NPC.Top, 1, 1, player.Center, 1, 1))
                        {
                            NPC.localAI[3] = 1f;
                            SoundEngine.PlaySound(SoundID.Item74, NPC.Center);

                            int bestY = headTileCenter.Y;
                            for (int j = 0; j < 20; j++)
                            {
                                if (bestY < 10)
                                    break;

                                if (!WorldGen.SolidTile(headTileCenter.X, bestY))
                                    break;

                                bestY--;
                            }

                            for (int k = 0; k < 20; k++)
                            {
                                if (bestY > Main.maxTilesY - 10)
                                    break;

                                if (WorldGen.ActiveAndWalkableTile(headTileCenter.X, bestY))
                                    break;

                                bestY++;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 sandSplashSpawnPos = new Vector2(headTileCenter.X * 16 + 8, bestY * 16 - 40);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), sandSplashSpawnPos, Vector2.Zero, ModContent.ProjectileType<DesertScourgeDiveSplash>(), 0, 0f, Main.myPlayer);

                                if (death)
                                {
                                    int type = ModContent.ProjectileType<DesertScourgeSpit>();
                                    for (int i = 0; i < 7; i++)
                                    {
                                        Vector2 sandSpitPos = new Vector2((i - 2) * 16f, -Math.Abs((i - 2) * 16f));
                                        Vector2 sandSpitVelocity = ((sandSplashSpawnPos + Vector2.UnitY * 80f) - (sandSplashSpawnPos + sandSpitPos)).SafeNormalize(Vector2.UnitY) * -((Math.Abs(i - 3) + 1) * 3f);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), sandSplashSpawnPos + sandSpitPos, sandSpitVelocity, type, SpitDamage, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                }
                else if (burrow)
                {
                    if (NPC.localAI[3] == 0f)
                    {
                        Point headTileCenter = NPC.Center.ToTileCoordinates();
                        Tile tileSafely = Framing.GetTileSafely(headTileCenter);
                        bool inSolidTile = tileSafely.HasUnactuatedTile;
                        if (inSolidTile && Collision.CanHit(NPC.Top, 1, 1, player.Center, 1, 1))
                        {
                            NPC.localAI[3] = 1f;
                            SoundEngine.PlaySound(SoundID.Item74, NPC.Center);

                            int bestY = headTileCenter.Y;
                            for (int j = 0; j < 20; j++)
                            {
                                if (bestY < 10)
                                    break;

                                if (!WorldGen.SolidTile(headTileCenter.X, bestY))
                                    break;

                                bestY--;
                            }

                            for (int k = 0; k < 20; k++)
                            {
                                if (bestY > Main.maxTilesY - 10)
                                    break;

                                if (WorldGen.ActiveAndWalkableTile(headTileCenter.X, bestY))
                                    break;

                                bestY++;
                            }

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                Vector2 sandSplashSpawnPos = new Vector2(headTileCenter.X * 16 + 8, bestY * 16 - 40);
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), sandSplashSpawnPos, Vector2.Zero, ModContent.ProjectileType<DesertScourgeDiveSplash>(), 0, 0f, Main.myPlayer);

                                if (death)
                                {
                                    int type = ModContent.ProjectileType<DesertScourgeSpit>();
                                    for (int i = 0; i < 7; i++)
                                    {
                                        Vector2 sandSpitPos = new Vector2((i - 2) * 16f, -Math.Abs((i - 2) * 16f));
                                        Vector2 sandSpitVelocity = ((sandSplashSpawnPos + Vector2.UnitY * 80f) - (sandSplashSpawnPos + sandSpitPos)).SafeNormalize(Vector2.UnitY) * -((Math.Abs(i - 3) + 1) * 3f);
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), sandSplashSpawnPos + sandSpitPos, sandSpitVelocity, type, SpitDamage, 0f, Main.myPlayer);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (lungeUpward || burrow)
            {
                speed *= 1.5f;
                turnSpeed *= 1.5f;

                if (NPC.Calamity().newAI[3] == 0f && lungeUpward)
                    NPC.Calamity().newAI[3] = player.Center.Y - LungeUpwardDistanceOffset;
            }

            if (NPC.ai[2] > 0f)
                NPC.realLife = (int)NPC.ai[2];

            if (hide)
            {
                NPC.alpha += 3;
                if (NPC.alpha > 255)
                {
                    NPC.alpha = 255;
                }
                else
                {
                    for (int dustIndex = 0; dustIndex < 2; dustIndex++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.UnusedBrown, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].noLight = true;
                    }
                }
            }
            else
            {
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                    NPC.alpha = 0;
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                // Become moist if in an aquatic biome
                if (Main.zenithWorld && (player.ZoneBeach || player.Calamity().ZoneAbyss || player.Calamity().ZoneSunkenSea || player.Calamity().ZoneSulphur) && NPC.CountNPCS(ModContent.NPCType<AquaticScourgeHead>()) < 1)
                {
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<AquaticScourgeHead>());
                    NPC.active = false;
                }
            }

            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!tailSpawned && NPC.ai[0] == 0f)
                {
                    int previous = NPC.whoAmI;
                    int minLength = death ? 24 : revenge ? 21 : expertMode ? 18 : 15;
                    if (Main.getGoodWorld)
                        minLength *= 3;

                    int bodyTypeAIVariable = 0;
                    for (int i = 0; i < minLength + 1; i++)
                    {
                        int lol;
                        if (i >= 0 && i < minLength)
                        {
                            if (i == 0)
                                bodyTypeAIVariable = 0;
                            else if (i == minLength - 1)
                                bodyTypeAIVariable = 30;
                            else if (i % 2 == 0)
                                bodyTypeAIVariable = 20;
                            else
                                bodyTypeAIVariable = 10;

                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertScourgeBody>(), NPC.whoAmI);
                            Main.npc[lol].ai[3] = bodyTypeAIVariable;
                        }
                        else
                            lol = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DesertScourgeTail>(), NPC.whoAmI);

                        Main.npc[lol].ai[2] = NPC.whoAmI;
                        Main.npc[lol].realLife = NPC.whoAmI;
                        Main.npc[lol].ai[1] = previous;
                        Main.npc[previous].ai[0] = lol;
                        NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, lol, 0f, 0f, 0f, 0);
                        previous = lol;
                    }

                    tailSpawned = true;
                }
            }

            if (NPC.life > Main.npc[(int)NPC.ai[0]].life)
                NPC.life = Main.npc[(int)NPC.ai[0]].life;

            int tilePositionX = (int)(NPC.position.X / 16f) - 1;
            int tileWidthPosX = (int)((NPC.position.X + (float)NPC.width) / 16f) + 2;
            int tilePositionY = (int)(NPC.position.Y / 16f) - 1;
            int tileWidthPosY = (int)((NPC.position.Y + (float)NPC.height) / 16f) + 2;
            if (tilePositionX < 0)
                tilePositionX = 0;
            if (tileWidthPosX > Main.maxTilesX)
                tileWidthPosX = Main.maxTilesX;
            if (tilePositionY < 0)
                tilePositionY = 0;
            if (tileWidthPosY > Main.maxTilesY)
                tileWidthPosY = Main.maxTilesY;

            bool shouldFly = lungeUpward;
            if (!shouldFly)
            {
                for (int k = tilePositionX; k < tileWidthPosX; k++)
                {
                    for (int l = tilePositionY; l < tileWidthPosY; l++)
                    {
                        if (Main.tile[k, l] != null && ((Main.tile[k, l].HasUnactuatedTile && (Main.tileSolid[(int)Main.tile[k, l].TileType] || (Main.tileSolidTop[(int)Main.tile[k, l].TileType] && Main.tile[k, l].TileFrameY == 0))) || Main.tile[k, l].LiquidAmount > 64))
                        {
                            Vector2 vector2;
                            vector2.X = (float)(k * 16);
                            vector2.Y = (float)(l * 16);
                            if (NPC.position.X + (float)NPC.width > vector2.X && NPC.position.X < vector2.X + 16f && NPC.position.Y + (float)NPC.height > vector2.Y && NPC.position.Y < vector2.Y + 16f)
                            {
                                shouldFly = true;
                                break;
                            }
                        }
                    }
                }
            }

            if (!shouldFly)
            {
                NPC.localAI[1] = 1f;
                Rectangle rectangle = new Rectangle((int)NPC.position.X, (int)NPC.position.Y, NPC.width, NPC.height);
                int directChaseDistance = death ? 600 : expertMode ? 800 : 1000;
                if (enrageScale > 0f)
                    directChaseDistance = 100;

                bool shouldDirectlyChase = true;
                if (NPC.position.Y > player.position.Y)
                {
                    int rectWidth = directChaseDistance * 2;
                    int rectHeight = directChaseDistance * 2;
                    foreach (Player plr in Main.ActivePlayers)
                    {
                        int rectX = (int)plr.position.X - directChaseDistance;
                        int rectY = (int)plr.position.Y - directChaseDistance;
                        Rectangle directChaseRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
                        if (rectangle.Intersects(directChaseRect))
                        {
                            shouldDirectlyChase = false;
                            break;
                        }
                    }
                    if (shouldDirectlyChase)
                        shouldFly = true;
                }
            }
            else
                NPC.localAI[1] = 0f;

            if (NPC.velocity.X < 0f)
                NPC.spriteDirection = 1;
            else if (NPC.velocity.X > 0f)
                NPC.spriteDirection = -1;

            float maxChaseSpeed = Main.zenithWorld ? SegmentVelocity_ZenithSeed :
                Main.getGoodWorld ? SegmentVelocity_GoodWorld :
                death ? SegmentVelocity_Death :
                expertMode ? SegmentVelocity_Expert :
                SegmentVelocity_Normal;
            if (burrow || lungeUpward)
                maxChaseSpeed *= 1.5f;
            if (expertMode)
                maxChaseSpeed += maxChaseSpeed * 0.2f * (1f - lifeRatio);

            if (player.dead)
            {
                shouldFly = false;
                NPC.velocity.Y += 1f;
                if ((double)NPC.position.Y > Main.worldSurface * 16D)
                {
                    NPC.velocity.Y += 1f;
                    maxChaseSpeed *= 2f;
                }

                if ((double)NPC.position.Y > Main.rockLayer * 16D)
                {
                    for (int a = 0; a < Main.maxNPCs; a++)
                    {
                        if (Main.npc[a].type == ModContent.NPCType<DesertScourgeHead>() || Main.npc[a].type == ModContent.NPCType<DesertScourgeBody>() || Main.npc[a].type == ModContent.NPCType<DesertScourgeTail>())
                            Main.npc[a].active = false;
                    }
                }
            }

            float burrowTarget = player.Center.Y + burrowDistance;
            float lungeTarget = NPC.Calamity().newAI[3];
            Vector2 npcCenter = NPC.Center;
            float playerX = player.Center.X;
            float targettingPosition = lungeUpward ? lungeTarget : burrow ? burrowTarget : player.Center.Y;
            playerX = (float)((int)(playerX / 16f) * 16);
            targettingPosition = (float)((int)(targettingPosition / 16f) * 16);
            npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
            npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
            playerX -= npcCenter.X;
            targettingPosition -= npcCenter.Y;
            float targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));

            // Lunge up towards target
            if (burrow && NPC.Center.Y >= burrowTarget - 16f && !lungeUpward && !quickFall)
            {
                NPC.Calamity().newAI[1] = 1f;
                NPC.localAI[3] = 0f;
                if (!playRoarSound)
                {
                    SoundEngine.PlaySound(RoarSound, player.Center);
                    playRoarSound = true;
                }
            }

            // Quickly fall back down once above target
            if (lungeUpward && NPC.Center.Y <= NPC.Calamity().newAI[3] + LungeUpwardDistanceOffset - LungeUpwardCutoffDistance && Math.Abs(NPC.Center.X - player.Center.X) < 480f && !quickFall)
            {
                // Spit a huge spread of sand upwards that falls down
                SoundEngine.PlaySound(SandBlastSound, NPC.Center);
                float velocity = Main.getGoodWorld ? 16f : death ? 8.5f : revenge ? 8f : expertMode ? 7.5f : 6f;
                int type = ModContent.ProjectileType<DesertScourgeSpit>();
                Vector2 projectileVelocity = (NPC.Center + NPC.velocity * 10f - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;
                int numProj = death ? 24 : revenge ? 21 : expertMode ? 18 : 12;
                if (Main.getGoodWorld)
                    numProj *= 2;

                int spread = Main.getGoodWorld ? 120 : 90;
                float rotation = MathHelper.ToRadians(spread);
                for (int i = 0; i < numProj; i++)
                {
                    Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(numProj - 1)));

                    for (int k = 0; k < 10; k++)
                    {
                        int dust = Dust.NewDust(NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, 10, 10, (int)CalamityDusts.SulphurousSeaAcid);
                        Main.dust[dust].velocity = perturbedSpeed;
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + Vector2.Normalize(perturbedSpeed) * 5f, perturbedSpeed, type, SpitDamage, 0f, Main.myPlayer);
                }

                NPC.TargetClosest();
                NPC.Calamity().newAI[1] = 2f;
                NPC.localAI[3] = 0f;
                playRoarSound = false;
            }

            // Quickly fall and reset variables once at target's Y position
            if (quickFall)
            {
                NPC.velocity.Y += maxChaseSpeed * 0.02f;
                if (NPC.Center.Y >= NPC.Calamity().newAI[3] + LungeUpwardDistanceOffset)
                {
                    NPC.Calamity().newAI[0] = 0f;
                    NPC.Calamity().newAI[1] = 0f;
                    NPC.Calamity().newAI[3] = 0f;
                    NPC.localAI[3] = 0f;
                    playRoarSound = false;
                }
            }

            // Reset variables if the burrow and lunge attack is taking too long
            if (resetTime)
            {
                NPC.Calamity().newAI[0] = 0f;
                NPC.Calamity().newAI[1] = 0f;
                NPC.Calamity().newAI[3] = 0f;
                NPC.localAI[3] = 0f;
                playRoarSound = false;
            }

            if (hide && !player.dead)
            {
                NPC.SimpleFlyMovement((new Vector2(player.Center.X, burrowTarget) - NPC.Center).SafeNormalize(Vector2.UnitY) * maxChaseSpeed, turnSpeed);
            }
            else if (!shouldFly)
            {
                NPC.velocity.Y += (death ? 0.125f : 0.1f);
                if (NPC.Center.Y - player.Center.Y < -180f)
                {
                    NPC.velocity.Y += 0.05f;
                    if (NPC.velocity.Y > 0f)
                        NPC.velocity.Y += 0.05f;
                }

                if (NPC.velocity.Y > maxChaseSpeed)
                    NPC.velocity.Y = maxChaseSpeed;

                // This bool exists to stop the strange wiggle behavior when worms are falling down
                bool slowXVelocity = Math.Abs(NPC.velocity.X) > speed;
                if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChaseSpeed * 0.4)
                {
                    if (NPC.velocity.X < 0f)
                        NPC.velocity.X -= speed * 1.1f;
                    else
                        NPC.velocity.X += speed * 1.1f;
                }
                else if (NPC.velocity.Y == maxChaseSpeed)
                {
                    if (slowXVelocity)
                    {
                        if (NPC.velocity.X < playerX)
                            NPC.velocity.X += speed;
                        else if (NPC.velocity.X > playerX)
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
                if (NPC.soundDelay == 0)
                {
                    float soundDelay = targetDistance / 40f;
                    if (soundDelay < 10f)
                        soundDelay = 10f;
                    if (soundDelay > 20f)
                        soundDelay = 20f;

                    NPC.soundDelay = (int)soundDelay;
                    SoundEngine.PlaySound(SoundID.WormDig, NPC.Center);
                }

                targetDistance = (float)Math.Sqrt((double)(playerX * playerX + targettingPosition * targettingPosition));
                float absolutePlayerX = Math.Abs(playerX);
                float absoluteTargetPos = Math.Abs(targettingPosition);
                float timeToReachTarget = maxChaseSpeed / targetDistance;
                playerX *= timeToReachTarget;
                targettingPosition *= timeToReachTarget;

                if (((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f)) && ((NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f)))
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X += turnSpeed;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X -= turnSpeed;

                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y += turnSpeed;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y -= turnSpeed;
                }

                if ((NPC.velocity.X > 0f && playerX > 0f) || (NPC.velocity.X < 0f && playerX < 0f) || (NPC.velocity.Y > 0f && targettingPosition > 0f) || (NPC.velocity.Y < 0f && targettingPosition < 0f))
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X += speed;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X -= speed;

                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y += speed;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y -= speed;

                    if ((double)Math.Abs(targettingPosition) < maxChaseSpeed * 0.2 && ((NPC.velocity.X > 0f && playerX < 0f) || (NPC.velocity.X < 0f && playerX > 0f)))
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += speed * 2f;
                        else
                            NPC.velocity.Y -= speed * 2f;
                    }

                    if ((double)Math.Abs(playerX) < maxChaseSpeed * 0.2 && ((NPC.velocity.Y > 0f && targettingPosition < 0f) || (NPC.velocity.Y < 0f && targettingPosition > 0f)))
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += speed * 2f;
                        else
                            NPC.velocity.X -= speed * 2f;
                    }
                }
                else if (absolutePlayerX > absoluteTargetPos)
                {
                    if (NPC.velocity.X < playerX)
                        NPC.velocity.X += speed * 1.1f;
                    else if (NPC.velocity.X > playerX)
                        NPC.velocity.X -= speed * 1.1f;

                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.Y > 0f)
                            NPC.velocity.Y += speed;
                        else
                            NPC.velocity.Y -= speed;
                    }
                }
                else
                {
                    if (NPC.velocity.Y < targettingPosition)
                        NPC.velocity.Y += speed * 1.1f;
                    else if (NPC.velocity.Y > targettingPosition)
                        NPC.velocity.Y -= speed * 1.1f;

                    if ((double)(Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y)) < maxChaseSpeed * 0.5)
                    {
                        if (NPC.velocity.X > 0f)
                            NPC.velocity.X += speed;
                        else
                            NPC.velocity.X -= speed;
                    }
                }
            }

            if ((!burrow || lungeUpward) && !quickFall && !hide)
            {
                Vector2 destination = lungeUpward ? new Vector2(player.Center.X, lungeTarget) : player.Center;
                if (NPC.Distance(destination) > (lungeUpward ? 1000f : 2000f))
                    NPC.velocity += (destination - NPC.Center).SafeNormalize(Vector2.UnitY) * turnSpeed;
            }

            NPC.rotation = (float)Math.Atan2((double)NPC.velocity.Y, (double)NPC.velocity.X) + MathHelper.PiOver2;

            if (shouldFly)
            {
                if (NPC.localAI[0] != 1f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 1f;
            }
            else
            {
                if (NPC.localAI[0] != 0f)
                    NPC.netUpdate = true;

                NPC.localAI[0] = 0f;
            }

            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
                NPC.netUpdate = true;
        }

        public override bool CheckActive() => false;

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

            return minDist <= 60f * NPC.scale && NPC.alpha <= 0;
        }

        public override void FindFrame(int frameHeight)
        {
            // Open mouth to prepare for a nibble ;3
            // Also open mouth if about to spit a projectile spread
            bool burrow = NPC.Calamity().newAI[0] >= BurrowTimeGateValue;
            bool lungeUpward = burrow && NPC.Calamity().newAI[1] == 1f;

            bool aboutToSpitSpread = lungeUpward && NPC.Center.Y <= NPC.Calamity().newAI[3] + LungeUpwardDistanceOffset - LungeUpwardCutoffDistance * 0.25f;
            bool openMouth = NPC.Distance(Main.player[NPC.target].Center) < OpenMouthForBiteDistance &&
                (Main.player[NPC.target].Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation().AngleTowards(NPC.velocity.ToRotation(), MathHelper.PiOver4) == NPC.velocity.ToRotation() &&
                NPC.ai[3] == 0f;

            bool closeMouthBite = NPC.ai[3] == 1f;
            if (closeMouthBite)
            {
                // Force mouth open for a bite if it's not open.
                if (NPC.frame.Y < frameHeight * OpenMouthStopFrame)
                {
                    NPC.frame.Y = frameHeight * OpenMouthStopFrame;
                    NPC.frameCounter = 0D;
                }

                NPC.frameCounter += 1D;
                if (NPC.frameCounter > 4D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                {
                    NPC.ai[3] = 2f;
                    NPC.ForceNetUpdate();
                    NPC.frame.Y = 0;
                }
            }
            else if (openMouth || aboutToSpitSpread)
            {
                NPC.frameCounter += 1D;
                if (NPC.frameCounter > 4D)
                {
                    NPC.frame.Y += frameHeight;
                    NPC.frameCounter = 0D;
                }
                if (NPC.frame.Y >= frameHeight * OpenMouthStopFrame)
                    NPC.frame.Y = frameHeight * OpenMouthStopFrame;
            }

            // Close mouth.
            else
            {
                if (NPC.frame.Y > 0)
                {
                    if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                    {
                        NPC.frame.Y = 0;
                        NPC.ai[3] = 0f;
                        NPC.ForceNetUpdate();
                    }
                    else
                    {
                        NPC.frameCounter += 1D;
                        if (NPC.frameCounter > 4D)
                        {
                            NPC.frame.Y -= frameHeight;
                            NPC.frameCounter = 0D;
                        }
                    }
                }
                else
                {
                    NPC.ai[3] = 0f;
                    NPC.ForceNetUpdate();
                }
            }
        }

        public override void BossLoot(ref int potionType)
        {
            potionType = ItemID.SandBlock;
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            // If Desert Scourge has not been killed yet, notify players that the Sunken Sea is open and Sandstorms can happen.
            if (!DownedBossSystem.downedDesertScourge)
            {
                string key = "Mods.CalamityMod.Status.Progression.OpenSunkenSea";
                Color messageColor = Color.Aquamarine;
                string key2 = "Mods.CalamityMod.Status.Progression.SandstormTrigger";
                Color messageColor2 = Color.PaleGoldenrod;

                CalamityUtils.BroadcastLocalizedText(key, messageColor);
                CalamityUtils.BroadcastLocalizedText(key2, messageColor2);

                if (!Terraria.GameContent.Events.Sandstorm.Happening)
                    CalamityWorld.StartSandstorm();
            }

            // Mark Desert Scourge as dead.
            DownedBossSystem.downedDesertScourge = true;
            CalamityNetcode.SyncWorld();
        }

        public override bool SpecialOnKill()
        {
            int closestSegmentID = DropHelper.FindClosestWormSegment(NPC,
                ModContent.NPCType<DesertScourgeHead>(),
                ModContent.NPCType<DesertScourgeBody>(),
                ModContent.NPCType<DesertScourgeTail>());
            NPC.position = Main.npc[closestSegmentID].position;
            return false;
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            // Boss bag
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<DesertScourgeBag>()));

            // Extraneous potions
            npcLoot.DefineConditionalDropSet(() => true).Add(DropHelper.PerPlayer(ItemID.LesserHealingPotion, 1, 5, 15), hideLootReport: true); // Healing Potions don't show up in the Bestiary.

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons and accessories
                int[] items = new int[]
                {
                    ModContent.ItemType<SaharaSlicers>(),
                    ModContent.ItemType<Barinade>(),
                    ModContent.ItemType<SandstreamScepter>(),
                    ModContent.ItemType<BrittleStarStaff>(),
                    ModContent.ItemType<ScourgeoftheDesert>()
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, items));

                // Vanity
                normalOnly.Add(ModContent.ItemType<DesertScourgeMask>(), 7);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);

                // Materials
                normalOnly.Add(ItemID.Coral, 1, 25, 30);
                normalOnly.Add(ItemID.Seashell, 1, 25, 30);
                normalOnly.Add(ItemID.Starfish, 1, 25, 30);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<PearlShard>(), 1, 25, 30));

                // Equipment
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<OceanCrest>()));
                normalOnly.Add(ModContent.ItemType<SandCloak>(), DropHelper.NormalWeaponDropRateFraction);
            }

            // Trophy (always directly from boss, never in bag)
            npcLoot.Add(ModContent.ItemType<DesertScourgeTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<DesertScourgeRelic>());

            // GFB Sand Shark Tooth Necklace drop
            npcLoot.DefineConditionalDropSet(DropHelper.GFB).Add(DropHelper.PerPlayer(ModContent.ItemType<SandSharkToothNecklace>()), hideLootReport: true);

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedDesertScourge, ModContent.ItemType<LoreDesertScourge>(), desc: DropHelper.FirstKillText);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < 3; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead2").Type, NPC.scale);
                    Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("ScourgeHead3").Type, NPC.scale);
                }

                for (int k = 0; k < 10; k++)
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, hit.HitDirection, -1f, 0, default, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                Texture2D texture = TextureAssets.Npc[NPC.type].Value;
                NPC.Opacity = 1f;
                // Reimplementation of CalamityUtils.DrawAnimatedBestiaryWorm but tweaked due to this entity' animations
                NPC.frame = texture.Frame();
                // Buffers the segment position and rotations
                float offset = -0.2f;
                float startX = 60;
                float startY = 70;
                int segmentSpacing = 50;
                int animationSpeed = 4;
                float wormTimer = NPC.Calamity().bestiaryWormTimer;
                int segCount = 3;
                // Draw the body segments
                for (int i = segCount; i > 0; i--)
                {
                    // The first segment is slightly closer to keep up with the head
                    float bodyOffset = i * segmentSpacing - segmentSpacing * 0.5f;

                    Texture2D toUse = i == 1 ? TextureAssets.Npc[ModContent.NPCType<DesertScourgeBody>()].Value : DesertScourgeBody.BodyTexture2.Value;
                    int frameCount = i == 1 ? 7 : 1;
                    spriteBatch.Draw(toUse, NPC.position + new Vector2(startX + bodyOffset, MathF.Sin((wormTimer + offset * i) * animationSpeed) * 2 + startY), toUse.Frame(1, frameCount, 0, 0), NPC.GetAlpha(drawColor), NPC.rotation - MathHelper.PiOver2 - MathF.Cos((wormTimer + offset * i) * animationSpeed) * MathHelper.PiOver4 * 0.075f, new Vector2(toUse.Width / 2, toUse.Height / (frameCount * 2)), NPC.scale, SpriteEffects.None, 0f);
                }
                // Draw the head
                spriteBatch.Draw(texture, NPC.position + new Vector2(startX + 18, MathF.Sin(wormTimer * animationSpeed) * 2 + startY), texture.Frame(1, 7, 0, 0), NPC.GetAlpha(drawColor), NPC.rotation - MathHelper.PiOver2 - MathF.Cos(wormTimer * animationSpeed) * MathHelper.PiOver4 * 0.075f, new Vector2(texture.Width * 0.5f, texture.Height / 7), NPC.scale, SpriteEffects.None, 0f);

                return false;
            }
            return true;
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                NPC.ai[3] = 1f;
                NPC.ForceNetUpdate();
            }
        }

        public override Color? GetAlpha(Color drawColor)
        {
            if (Main.zenithWorld)
            {
                Color lightColor = Color.MediumBlue * drawColor.A;
                return lightColor * NPC.Opacity;
            }
            else return null;
        }
    }
}
