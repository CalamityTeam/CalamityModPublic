using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
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
using System.IO;
using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;
/* states:
* 0 = slow drift
* 1 = reelback and teleport after spawn enemy
* 2 = reelback for spin lunge + death legacy
* 3 = spin lunge
* 4 = semicircle spawn arc
* 5 = raindash
* 6 = deceleration
*/

namespace CalamityMod.NPCs.HiveMind
{
    public class HiveMind : ModNPC
    {
        public static int normalIconIndex;
        public static int phase2IconIndex;

        internal static void LoadHeadIcons()
        {
            string normalIconPath = "CalamityMod/NPCs/HiveMind/HiveMind_Head_Boss";
            string phase2IconPath = "CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss";

            CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
            normalIconIndex = ModContent.GetModBossHeadSlot(normalIconPath);

            CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
            phase2IconIndex = ModContent.GetModBossHeadSlot(phase2IconPath);
        }

        // This block of values can be modified in SetDefaults() based on difficulty mode or something
        private int biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;
        private int burrowTimer = 420;
        private int minimumDriftTime = 300;
        private int teleportRadius = 300;
        private int decelerationTime = 30;
        private int reelbackFade = 2; // Divide 255 by this for duration of reelback in ticks
        private float arcTime = 45f; // Ticks needed to complete movement for spawn and rain attacks (DEATH ONLY)
        private float driftSpeed = 1f; // Default speed when slowly floating at player
        private float driftBoost = 1f; // Max speed added as health decreases
        private int lungeDelay = 90; // # of ticks long hive mind spends sliding to a stop before lunging
        private int lungeTime = 33;
        private int lungeFade = 15; // Divide 255 by this for duration of hive mind spin before slowing for lunge
        private double lungeRots = 0.2; // Number of revolutions made while spinning/fading in for lunge
        private bool dashStarted = false;
        private int phase2timer = 360;
        private int rotationDirection;
        private double rotation;
        private double rotationIncrement;
        private int state = 0;
        private int previousState = 0;
        private int nextState = 0;
        private int reelCount = 0;
        private Vector2 deceleration;
        private int frameX = 0;
        private int frameY = 0;
        private const int maxFramesX_Phase2 = 2;
        private const int maxFramesY_Phase2 = 8;
        private const int height_Phase2 = 142;

        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/HiveMindRoar");
        public static readonly SoundStyle FastRoarSound = new("CalamityMod/Sounds/Custom/HiveMindRoarFast");

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 16;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.TrailCacheLength[NPC.type] = NPC.oldPos.Length;
            NPCID.Sets.BossBestiaryPriority.Add(Type); 
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.4f,
                PortraitPositionYOverride = 3f
            };
            value.Position.Y += 3f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
			NPCID.Sets.MPAllowedEnemies[Type] = true;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.npcSlots = 5f;
            NPC.GetNPCDamage();
            NPC.width = 178;
            NPC.height = 122;
            NPC.defense = 8;
            NPC.LifeMaxNERB(8500, 10200, 350000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(0, 15, 0, 0);
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            if (Main.expertMode)
            {
                minimumDriftTime = 120;
                reelbackFade = 4;
            }

            if (CalamityWorld.revenge)
            {
                lungeRots = 0.3;
                minimumDriftTime = 90;
                reelbackFade = 5;
                lungeTime = 28;
                driftSpeed = 2f;
                driftBoost = 2f;
            }

            if (CalamityWorld.death)
            {
                lungeRots = 0.4;
                minimumDriftTime = 60;
                reelbackFade = 6;
                lungeTime = 23;
                driftSpeed = 3f;
                driftBoost = 1f;
            }

            if (BossRushEvent.BossRushActive)
            {
                lungeRots = 0.4;
                minimumDriftTime = 40;
                reelbackFade = 10;
                lungeTime = 16;
                driftSpeed = 6f;
                driftBoost = 1f;
            }

            if (Main.getGoodWorld)
            {
                reelbackFade *= 10;
                arcTime *= 0.5f;
            }

            phase2timer = minimumDriftTime;
            rotationIncrement = 0.0246399424 * lungeRots * lungeFade;
            NPC.Calamity().VulnerableToHeat = true;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundCorruption,
				new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.HiveMind")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            if (NPC.life / (float)NPC.lifeMax < 0.8f)
                index = phase2IconIndex;
            else
                index = normalIconIndex;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(biomeEnrageTimer);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.noTileCollide);
            writer.Write(NPC.noGravity);
            writer.Write(NPC.localAI[0]);
            writer.Write(NPC.localAI[1]);
            writer.Write(NPC.localAI[3]);
            writer.Write(burrowTimer);
            writer.Write(state);
            writer.Write(nextState);
            writer.Write(phase2timer);
            writer.Write(dashStarted);
            writer.Write(rotationDirection);
            writer.Write(rotation);
            writer.Write(previousState);
            writer.Write(reelCount);
            writer.Write(frameX);
            writer.Write(frameY);

            writer.Write(NPC.frame.X);
            writer.Write(NPC.frame.Y);
            writer.Write(NPC.frame.Width);
            writer.Write(NPC.frame.Height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            biomeEnrageTimer = reader.ReadInt32();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.noTileCollide = reader.ReadBoolean();
            NPC.noGravity = reader.ReadBoolean();
            NPC.localAI[0] = reader.ReadSingle();
            NPC.localAI[1] = reader.ReadSingle();
            NPC.localAI[3] = reader.ReadSingle();
            burrowTimer = reader.ReadInt32();
            state = reader.ReadInt32();
            nextState = reader.ReadInt32();
            phase2timer = reader.ReadInt32();
            dashStarted = reader.ReadBoolean();
            rotationDirection = reader.ReadInt32();
            rotation = reader.ReadDouble();
            previousState = reader.ReadInt32();
            reelCount = reader.ReadInt32();
            frameX = reader.ReadInt32();
            frameY = reader.ReadInt32();

            Rectangle frame = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
            if (frame.Width > 0 && frame.Height > 0)
                NPC.frame = frame;
        }

        public override void FindFrame(int frameHeight)
        {
            // When Hive Mind starts flying around
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.8f;

            if (phase2)
            {
                NPC.frameCounter++;
                if (NPC.frameCounter >= 6D)
                {
                    // Reset frame counter
                    NPC.frameCounter = 0D;

                    // Increment the Y frame
                    frameY++;

                    // Reset the Y frame if greater than 8
                    if (frameY == maxFramesY_Phase2)
                    {
                        frameX++;
                        frameY = 0;
                    }

                    // Reset the frames to frame 0
                    if ((frameX * maxFramesY_Phase2) + frameY > 15)
                        frameX = frameY = 0;
                }
            }
            else
            {
                NPC.frameCounter += 1f / 6f;
                NPC.frameCounter %= Main.npcFrameCount[NPC.type];
                int frame = (int)NPC.frameCounter;
                NPC.frame.Y = frame * frameHeight;
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            // When Hive Mind starts flying around
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.8f;

            if (phase2)
            {
                SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                Texture2D texture = ModContent.Request<Texture2D>("CalamityMod/NPCs/HiveMind/HiveMindP2").Value;
                Rectangle frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
                Vector2 vector = new Vector2(NPC.width / 2, NPC.height / 2);
                Color afterimageBaseColor = Color.White;
                int numAfterimages = 5;

                if (CalamityConfig.Instance.Afterimages && state != 0)
                {
                    for (int i = 1; i < numAfterimages; i += 2)
                    {
                        Color afterimageColor = drawColor;
                        afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
                        afterimageColor = NPC.GetAlpha(afterimageColor);
                        afterimageColor *= (numAfterimages - i) / 15f;
                        Vector2 afterimageCenter = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                        afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFramesX_Phase2, maxFramesY_Phase2) * NPC.scale / 2f;
                        afterimageCenter += vector * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                        spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], vector, NPC.scale, spriteEffects, 0f);
                    }
                }

                Vector2 center = NPC.Center - screenPos;
                spriteBatch.Draw(texture, center, frame, NPC.GetAlpha(drawColor), NPC.rotation, vector, NPC.scale, spriteEffects, 0f);

                return false;
            }

            return true;
        }

        private void SpawnStuff()
        {
            int maxSpawns = (CalamityWorld.death || BossRushEvent.BossRushActive) ? 5 : CalamityWorld.revenge ? 4 : Main.expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
            for (int i = 0; i < maxSpawns; i++)
            {
                int type = NPCID.EaterofSouls;
                int choice = -1;
                do
                {
                    choice++;
                    switch (choice)
                    {
                        case 0:
                        case 1:
                            type = NPCID.EaterofSouls;
                            break;
                        case 2:
                            type = NPCID.DevourerHead;
                            break;
                        case 3:
                        case 4:
                            type = ModContent.NPCType<DankCreeper>();
                            break;
                        default:
                            break;
                    }
                }
                while (NPC.AnyNPCs(type) && choice < 5);

                if (choice < 5)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + Main.rand.Next(NPC.width), (int)NPC.position.Y + Main.rand.Next(NPC.height), type);
            }

            // Spawn a Hive Cyst
            if (Main.zenithWorld && NPC.CountNPCS(ModContent.NPCType<HiveTumor>()) < 3)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + Main.rand.Next(NPC.width), (int)NPC.position.Y + Main.rand.Next(NPC.height), ModContent.NPCType<HiveTumor>());
        }

        private void ReelBack()
        {
            NPC.alpha = 0;
            phase2timer = 0;
            deceleration = NPC.velocity / 255f * reelbackFade;

            if (CalamityWorld.revenge || BossRushEvent.BossRushActive)
            {
                state = 2;
                SoundEngine.PlaySound(FastRoarSound, NPC.Center);
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                    SpawnStuff();

                state = nextState;
                nextState = 0;

                if (state == 2)
                    SoundEngine.PlaySound(RoarSound, NPC.Center);
                else
                    SoundEngine.PlaySound(FastRoarSound, NPC.Center);
            }
        }

        public override void AI()
        {
            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Enrage
            if ((!player.ZoneCorrupt || (NPC.position.Y / 16f) < Main.worldSurface) && !bossRush)
            {
                if (biomeEnrageTimer > 0)
                    biomeEnrageTimer--;
            }
            else
                biomeEnrageTimer = CalamityGlobalNPC.biomeEnrageTimerMax;

            bool biomeEnraged = biomeEnrageTimer <= 0 || bossRush;

            float enrageScale = bossRush ? 1f : 0f;
            if (biomeEnraged && (!player.ZoneCorrupt || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }
            if (biomeEnraged && ((NPC.position.Y / 16f) < Main.worldSurface || bossRush))
            {
                NPC.Calamity().CurrentlyEnraged = !bossRush;
                enrageScale += 1f;
            }

            // When Hive Mind starts flying around
            bool phase2 = lifeRatio < 0.8f;

            // Phase 2 settings
            if (phase2)
            {
                // Spawn gores, play sound and reset every crucial variable at the start
                if (NPC.localAI[1] == 0f)
                {
                    NPC.localAI[1] = 1f;

                    if (Main.netMode != NetmodeID.Server)
                    {
                        int goreAmount = 7;
                        for (int i = 1; i <= goreAmount; i++)
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.position, NPC.velocity, Mod.Find<ModGore>("HiveMindGore" + i).Type, 1f);
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);

                    NPC.position = NPC.Center;
                    NPC.height = height_Phase2;
                    NPC.position -= NPC.Size * 0.5f;

                    NPC.frame.Y = 0;
                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.scale = 1f;
                    NPC.alpha = 0;
                    NPC.dontTakeDamage = false;
                    NPC.damage = NPC.defDamage;
                    NPC.netSpam = 0;
                    NPC.netUpdate = true;
                }

                NPC.frame = new Rectangle(NPC.width * frameX, NPC.height * frameY, NPC.width, NPC.height);
            }
            else
            {
                CalamityGlobalNPC.hiveMind = NPC.whoAmI;

                if (!player.active || player.dead)
                {
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                    if (!player.active || player.dead)
                    {
                        if (NPC.timeLeft > 60)
                            NPC.timeLeft = 60;

                        if (NPC.localAI[3] < 120f)
                            NPC.localAI[3] += 1f;

                        if (NPC.localAI[3] > 60f)
                        {
                            NPC.velocity.Y += (NPC.localAI[3] - 60f) * 0.5f;

                            NPC.noGravity = true;
                            NPC.noTileCollide = true;

                            if (burrowTimer > 30)
                                burrowTimer = 30;
                        }

                        return;
                    }
                }
                else if (NPC.timeLeft < 1800)
                    NPC.timeLeft = 1800;

                if (NPC.localAI[3] > 0f)
                {
                    NPC.localAI[3] -= 1f;
                    return;
                }

                NPC.noGravity = false;
                NPC.noTileCollide = false;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.localAI[0] == 0f)
                    {
                        NPC.localAI[0] = 1f;
                        int maxBlobs = death ? 15 : revenge ? 7 : expertMode ? 6 : 5;
                        if (Main.getGoodWorld)
                            maxBlobs *= 2;
                        if (Main.zenithWorld)
                            maxBlobs = 50;

                        for (int i = 0; i < maxBlobs; i++)
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<HiveBlob>(), NPC.whoAmI);
                    }
                }

                if (NPC.ai[3] == 0f && NPC.life > 0)
                    NPC.ai[3] = NPC.lifeMax;

                if (NPC.life > 0)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int fivePercentHP = (int)(NPC.lifeMax * 0.05);
                        if ((NPC.life + fivePercentHP) < NPC.ai[3])
                        {
                            NPC.ai[3] = NPC.life;

                            int maxSpawns = bossRush ? 10 : death ? 5 : revenge ? 4 : expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
                            int maxDankSpawns = bossRush ? 4 : death ? Main.rand.Next(2, 4) : revenge ? 2 : expertMode ? Main.rand.Next(1, 3) : 1;

                            for (int i = 0; i < maxSpawns; i++)
                            {
                                int x = (int)(NPC.position.X + Main.rand.Next(NPC.width - 32));
                                int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height - 32));

                                int type = ModContent.NPCType<HiveBlob>();
                                if (NPC.CountNPCS(ModContent.NPCType<DankCreeper>()) < maxDankSpawns)
                                    type = ModContent.NPCType<DankCreeper>();

                                int fivePercentMinions = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, type);
                                Main.npc[fivePercentMinions].SetDefaults(type);
                                if (Main.netMode == NetmodeID.Server && fivePercentMinions < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, fivePercentMinions, 0f, 0f, 0f, 0, 0, 0);
                            }

                            return;
                        }
                    }
                }

                burrowTimer--;
                if (burrowTimer < -120)
                {
                    burrowTimer = (death ? 180 : revenge ? 300 : expertMode ? 360 : 420) - (int)enrageScale * 55;
                    if (burrowTimer < 30)
                        burrowTimer = 30;

                    NPC.scale = 1f;
                    NPC.alpha = 0;
                    NPC.dontTakeDamage = false;
                    NPC.damage = NPC.defDamage;
                }
                else if (burrowTimer < -60)
                {
                    NPC.scale += 0.0165f;
                    NPC.alpha -= 4;

                    int burrowedDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, 14, 0f, -3f, 100, default, 2.5f * NPC.scale);
                    Main.dust[burrowedDust].velocity *= 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[burrowedDust].scale = 0.5f;
                        Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        int burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, 14, 0f, -3f, 100, default, 3.5f * NPC.scale);
                        Main.dust[burrowedDust2].noGravity = true;
                        Main.dust[burrowedDust2].velocity *= 3.5f;
                        burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, 14, 0f, -3f, 100, default, 2.5f * NPC.scale);
                        Main.dust[burrowedDust2].velocity *= 1f;
                    }
                }
                else if (burrowTimer == -60)
                {
                    NPC.scale = 0.01f;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.Center = player.Center;
                        NPC.position.Y = player.position.Y - NPC.height;
                        int tilePosX = (int)NPC.Center.X / 16;
                        int tilePosY = (int)(NPC.position.Y + NPC.height) / 16 + 1;

                        while (!(Main.tile[tilePosX, tilePosY].HasUnactuatedTile && Main.tileSolid[Main.tile[tilePosX, tilePosY].TileType]))
                        {
                            tilePosY++;
                            NPC.position.Y += 16;
                        }
                    }
                    NPC.netUpdate = true;
                    NPC.netSpam = 0;
                }
                else if (burrowTimer < 0)
                {
                    NPC.scale -= 0.0165f;
                    NPC.alpha += 4;

                    int burrowedDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, 14, 0f, -3f, 100, default, 2.5f * NPC.scale);
                    Main.dust[burrowedDust].velocity *= 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[burrowedDust].scale = 0.5f;
                        Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        int burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, 14, 0f, -3f, 100, default, 3.5f * NPC.scale);
                        Main.dust[burrowedDust2].noGravity = true;
                        Main.dust[burrowedDust2].velocity *= 3.5f;
                        burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, 14, 0f, -3f, 100, default, 2.5f * NPC.scale);
                        Main.dust[burrowedDust2].velocity *= 1f;
                    }
                }
                else if (burrowTimer == 0)
                {
                    if (!player.active || player.dead)
                    {
                        burrowTimer = 30;
                    }
                    else
                    {
                        NPC.TargetClosest();
                        NPC.dontTakeDamage = true;
                        NPC.damage = 0;
                    }
                }

                return;
            }

            if (NPC.alpha != 0)
            {
                if (NPC.damage != 0)
                    NPC.damage = 0;
            }
            else
                NPC.damage = NPC.defDamage;

            switch (state)
            {
                case 0: // Slowdrift

                    if (NPC.alpha > 0)
                        NPC.alpha -= 3;

                    if (nextState == 0)
                    {
                        NPC.TargetClosest();
                        if (revenge && lifeRatio < 0.53f)
                        {
                            if (death)
                            {
                                do
                                    nextState = Main.rand.Next(3, 6);
                                while (nextState == previousState);
                                previousState = nextState;
                            }
                            else if (lifeRatio < 0.27f)
                            {
                                do
                                    nextState = Main.rand.Next(3, 6);
                                while (nextState == previousState);
                                previousState = nextState;
                            }
                            else
                            {
                                do
                                    nextState = Main.rand.Next(3, 5);
                                while (nextState == previousState);
                                previousState = nextState;
                            }
                        }
                        else
                        {
                            if (revenge && (Main.rand.NextBool(3) || reelCount == 2))
                            {
                                reelCount = 0;
                                nextState = 2;
                            }
                            else
                            {
                                reelCount++;
                                if (Main.expertMode && reelCount == 2)
                                {
                                    reelCount = 0;
                                    nextState = 2;
                                }
                                else
                                    nextState = 1;

                                NPC.ai[1] = 0f;
                                NPC.ai[2] = 0f;
                            }
                        }

                        if (nextState == 3)
                            rotation = MathHelper.ToRadians(Main.rand.Next(360));

                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }

                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 5000f)
                    {
                        NPC.TargetClosest(false);
                        player = Main.player[NPC.target];
                        if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 5000f)
                        {
                            if (NPC.timeLeft > 60)
                                NPC.timeLeft = 60;

                            if (NPC.localAI[3] < 120f)
                                NPC.localAI[3] += 1f;

                            if (NPC.localAI[3] > 60f)
                                NPC.velocity.Y += (NPC.localAI[3] - 60f) * 0.5f;

                            return;
                        }
                    }
                    else if (NPC.timeLeft < 1800)
                        NPC.timeLeft = 1800;

                    if (NPC.localAI[3] > 0f)
                    {
                        NPC.localAI[3] -= 1f;
                        return;
                    }

                    NPC.velocity = player.Center - NPC.Center;

                    phase2timer--;
                    if (phase2timer <= -180) // No stalling drift mode forever
                    {
                        NPC.velocity *= 2f / 255f * (reelbackFade + 2 * (int)enrageScale);
                        ReelBack();
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    else
                    {
                        NPC.velocity.Normalize();
                        if (expertMode) // Variable velocity in expert and up
                            NPC.velocity *= driftSpeed + enrageScale + driftBoost * lifeRatio;
                        else
                            NPC.velocity *= driftSpeed + enrageScale;
                    }

                    break;

                case 1: // Reelback and teleport

                    NPC.alpha += reelbackFade + 2 * (int)enrageScale;
                    NPC.velocity -= deceleration;

                    if (NPC.alpha >= 255)
                    {
                        NPC.alpha = 255;
                        NPC.velocity = Vector2.Zero;
                        state = 0;

                        if (Main.netMode != NetmodeID.MultiplayerClient && NPC.ai[1] != 0f && NPC.ai[2] != 0f)
                        {
                            NPC.position.X = NPC.ai[1] * 16 - NPC.width / 2;
                            NPC.position.Y = NPC.ai[2] * 16 - NPC.height / 2;
                        }

                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    else if (NPC.ai[1] == 0f && NPC.ai[2] == 0f)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            int posX = (int)player.Center.X / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool() ? -1 : 1);
                            int posY = (int)player.Center.Y / 16 + Main.rand.Next(15, 46) * (Main.rand.NextBool() ? -1 : 1);
                            if (!WorldGen.SolidTile(posX, posY) && Collision.CanHit(new Vector2(posX * 16, posY * 16), 1, 1, player.position, player.width, player.height))
                            {
                                NPC.ai[1] = posX;
                                NPC.ai[2] = posY;
                                NPC.netUpdate = true;
                                NPC.netSpam = 0;
                                break;
                            }
                        }
                    }

                    break;

                case 2: // Reelback for lunge + death legacy

                    NPC.alpha += reelbackFade + 2 * (int)enrageScale;
                    NPC.velocity -= deceleration;

                    if (NPC.alpha >= 255)
                    {
                        NPC.alpha = 255;
                        NPC.velocity = Vector2.Zero;
                        dashStarted = false;

                        if (revenge && lifeRatio < 0.53f)
                        {
                            state = nextState;
                            nextState = 0;
                            previousState = state;
                        }
                        else
                            state = 3;

                        if (player.velocity.X > 0)
                            rotationDirection = 1;
                        else if (player.velocity.X < 0)
                            rotationDirection = -1;
                        else
                            rotationDirection = player.direction;
                    }

                    break;

                case 3: // Lunge

                    NPC.netUpdate = true;
                    NPC.netSpam = 0;
                    if (NPC.alpha > 0)
                    {
                        NPC.alpha -= lungeFade;

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);

                        rotation += rotationIncrement * rotationDirection;
                        phase2timer = lungeDelay;
                    }
                    else
                    {
                        phase2timer--;
                        if (!dashStarted)
                        {
                            if (phase2timer <= 0)
                            {
                                phase2timer = lungeTime - 4 * (int)enrageScale;
                                NPC.velocity = player.Center + (bossRush ? player.velocity * 20f : Vector2.Zero) - NPC.Center;
                                NPC.velocity.Normalize();
                                NPC.velocity *= teleportRadius / (lungeTime - (int)enrageScale);
                                dashStarted = true;
                                SoundEngine.PlaySound(RoarSound, NPC.Center);
                            }
                            else
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    NPC.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);

                                rotation += rotationIncrement * rotationDirection * phase2timer / lungeDelay;
                            }
                        }
                        else
                        {
                            if (phase2timer <= 0)
                            {
                                state = 6;
                                phase2timer = 0;
                                deceleration = NPC.velocity / decelerationTime;
                            }
                        }
                    }

                    break;

                case 4: // Enemy spawn arc

                    if (NPC.alpha > 0)
                    {
                        NPC.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.Center = player.Center;
                            NPC.position.Y += teleportRadius;
                        }
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            SoundEngine.PlaySound(RoarSound, NPC.Center);
                            NPC.velocity.X = MathHelper.Pi * teleportRadius / arcTime;
                            NPC.velocity *= rotationDirection;
                            NPC.netUpdate = true;
                            NPC.netSpam = 0;
                        }
                        else
                        {
                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);

                            phase2timer++;
                            if (phase2timer == (int)arcTime / 6)
                            {
                                phase2timer = 0;
                                NPC.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient && Collision.CanHit(NPC.Center, 1, 1, player.position, player.width, player.height))
                                {
                                    if (NPC.ai[0] == 2 || NPC.ai[0] == 4)
                                    {
                                        if (expertMode && !NPC.AnyNPCs(ModContent.NPCType<DarkHeart>()))
                                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<DarkHeart>());
                                    }
                                    else if (!NPC.AnyNPCs(NPCID.EaterofSouls))
                                        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.EaterofSouls);
                                }

                                if (NPC.ai[0] == 6)
                                {
                                    NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);
                                    SpawnStuff();
                                    state = 6;
                                    NPC.ai[0] = 0;
                                    deceleration = NPC.velocity / decelerationTime;
                                }
                            }
                        }
                    }

                    break;

                case 5: // Rain dash

                    if (NPC.alpha > 0)
                    {
                        NPC.alpha -= 5;
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.Center = player.Center;
                            NPC.position.Y -= teleportRadius;
                            NPC.position.X += teleportRadius * rotationDirection;
                        }
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            dashStarted = true;
                            SoundEngine.PlaySound(RoarSound, NPC.Center);
                            NPC.velocity.X = teleportRadius / arcTime * 3;
                            NPC.velocity *= -rotationDirection;
                            NPC.netUpdate = true;
                            NPC.netSpam = 0;
                        }
                        else
                        {
                            phase2timer++;
                            if (phase2timer == (int)arcTime / 20)
                            {
                                phase2timer = 0;
                                NPC.ai[0]++;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                                    int damage = NPC.GetProjectileDamage(type);
                                    Vector2 cloudSpawnPos = new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height));
                                    Vector2 randomVelocity = (CalamityWorld.LegendaryMode && CalamityWorld.revenge) ? Main.rand.NextVector2CircularEdge(4f, 4f) : Vector2.Zero;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), cloudSpawnPos, randomVelocity, type, damage, 0, Main.myPlayer, 11f);
                                }

                                if (NPC.ai[0] == 10)
                                {
                                    state = 6;
                                    NPC.ai[0] = 0;
                                    deceleration = NPC.velocity / decelerationTime;
                                }
                            }
                        }
                    }

                    break;

                case 6: // Deceleration

                    NPC.velocity -= deceleration;
                    phase2timer++;
                    if (phase2timer == decelerationTime)
                    {
                        phase2timer = minimumDriftTime + Main.rand.Next(121);
                        state = 0;
                        NPC.netUpdate = true;
                        NPC.netSpam = 0;
                    }

                    break;
            }
        }

        public override bool CanHitNPC(NPC target) => NPC.alpha == 0; // Can only be hit while fully visible

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

            return minDist <= 60f && NPC.alpha == 0 && NPC.scale == 1f; // No damage while not fully visible or shrunk
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => NPC.scale == 1f; // Only draw HP bar while at full size

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (phase2timer < 0)
            {
                NPC.velocity *= -4f;
                ReelBack();
                NPC.netUpdate = true;
                NPC.netSpam = 0;
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < hit.Damage / NPC.lifeMax * 100.0; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 14, hit.HitDirection, -1f, 0, default, 1f);

            // When Hive Mind starts flying around
            bool phase2 = NPC.life / (float)NPC.lifeMax < 0.8f;

            if (phase2)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient && Main.zenithWorld ? NPC.CountNPCS(ModContent.NPCType<HiveBlob2>()) < 10 : (Main.rand.NextBool(15) && NPC.CountNPCS(ModContent.NPCType<HiveBlob2>()) < 2))
                {
                    Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                    NPC.NewNPC(NPC.GetSource_FromThis(), (int)spawnAt.X, (int)spawnAt.Y, ModContent.NPCType<HiveBlob2>());
                }
            }
            else
            {
                if (NPC.CountNPCS(NPCID.EaterofSouls) < 3 && NPC.CountNPCS(NPCID.DevourerHead) < 1)
                {
                    if (Main.rand.NextBool(60) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)spawnAt.X, (int)spawnAt.Y, NPCID.EaterofSouls);
                    }

                    if (Main.rand.NextBool(150) && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 spawnAt = NPC.Center + new Vector2(0f, NPC.height / 2f);
                        NPC.NewNPC(NPC.GetSource_FromThis(), (int)spawnAt.X, (int)spawnAt.Y, NPCID.DevourerHead);
                    }
                }
            }

            if (NPC.life <= 0)
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    int goreAmount = 10;
                    for (int i = 1; i <= goreAmount; i++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Mod.Find<ModGore>("HiveMindP2Gore" + i).Type, 1f);
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 200;
                NPC.height = 150;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int killDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[killDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[killDust].scale = 0.5f;
                        Main.dust[killDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int killDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 14, 0f, 0f, 100, default, 3f);
                    Main.dust[killDust2].noGravity = true;
                    Main.dust[killDust2].velocity *= 5f;
                    killDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 14, 0f, 0f, 100, default, 2f);
                    Main.dust[killDust2].velocity *= 2f;
                }
            }
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!DownedBossSystem.downedHiveMind && !DownedBossSystem.downedPerforator)
            {
                string key = "Mods.CalamityMod.Status.Progression.SkyOreText";
                Color messageColor = Color.Cyan;
                AerialiteOreGen.Generate(true);

                CalamityUtils.DisplayLocalizedText(key, messageColor);
            }

            // Mark The Hive Mind as dead
            DownedBossSystem.downedHiveMind = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<HiveMindBag>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons and such
				normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, new WeightedItemStack[]
				{
					ModContent.ItemType<PerfectDark>(),
					ModContent.ItemType<Shadethrower>(),
					ModContent.ItemType<ShaderainStaff>(),
					ModContent.ItemType<DankStaff>(),
					new WeightedItemStack(ModContent.ItemType<RotBall>(), 1f, 30, 50),
				}));

                // Materials
                normalOnly.Add(ItemID.DemoniteBar, 1, 10, 15);
                normalOnly.Add(ItemID.RottenChunk, 1, 10, 15);
                normalOnly.Add(ItemID.CorruptSeeds, 1, 10, 15);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<RottenMatter>(), 1, 25, 30));
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.Hardmode(), ItemID.CursedFlame, 1, 10, 20));

                // Equipment
				normalOnly.Add(ModContent.ItemType<FilthyGlove>(), DropHelper.NormalWeaponDropRateFraction);
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<RottenBrain>()));

                // Vanity
                normalOnly.Add(ModContent.ItemType<HiveMindMask>(), 7);
                normalOnly.Add(ModContent.ItemType<RottingEyeball>(), 10);
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            npcLoot.Add(ModContent.ItemType<HiveMindTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<HiveMindRelic>());

            // GFB class emblem drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ItemID.WarriorEmblem, hideLootReport: true);
                GFBOnly.Add(ItemID.RangerEmblem, hideLootReport: true);
                GFBOnly.Add(ItemID.SorcererEmblem, hideLootReport: true);
                GFBOnly.Add(ItemID.SummonerEmblem, hideLootReport: true);
                GFBOnly.Add(ModContent.ItemType<RogueEmblem>(), hideLootReport: true);
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedHiveMind, ModContent.ItemType<LoreHiveMind>(), desc: DropHelper.FirstKillText);
        }
    }
}
