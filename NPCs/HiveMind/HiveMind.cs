using System.IO;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Events;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
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

        public override void Load()
        {
            string normalIconPath = "CalamityMod/NPCs/HiveMind/HiveMind_Head_Boss";
            string phase2IconPath = "CalamityMod/NPCs/HiveMind/HiveMindP2_Head_Boss";

            normalIconIndex = CalamityMod.Instance.AddBossHeadTexture(normalIconPath, -1);
            phase2IconIndex = CalamityMod.Instance.AddBossHeadTexture(phase2IconPath, -1);
        }

        // This block of values can be modified in SetDefaults() based on difficulty mode or something
        private int burrowTimer = 120;
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
        private int lungeAmount = 1;
        private int lungesPerformed = 0;
        private bool performingLungeCombo = false;
        private double lungeRots = 0.2; // Number of revolutions made while spinning/fading in for lunge
        private bool dashStarted = false;
        private int rainDashAmount = 1;
        private int rainDashesPerformed = 0;
        private bool performingRainDashCombo = false;
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
        private int frameWidth = 0;
        private int frameHeight = 0;
        private int maxFrameX = 0;
        private int maxFrameY = 0;

        private bool IsPhaseTwo => (NPC.life / (float)NPC.lifeMax) < 0.8f;

        public static readonly SoundStyle RoarSound = new("CalamityMod/Sounds/Custom/HiveMindRoar");
        public static readonly SoundStyle FastRoarSound = new("CalamityMod/Sounds/Custom/HiveMindRoarFast");

        public static Asset<Texture2D> Phase2Texture;

        // Do not directly use those value in AI
        // Those value should only be used on FindFrame
        // Commented out variables are what you suppose to use commonly:
        // - NPC.height, NPC.width, NPC.frame
        // - frameX, frameY, frameWidth, frameHeight, maxFrameX, maxFrameY
        private const int framesX_P1 = 1;
        private const int framesY_P1 = 16;

        private const int framesX_P2 = 2;
        private const int framesY_P2 = 8;

        private const int frameWidth_P1 = 178;
        private const int frameHeight_P1 = 122;

        private const int frameWidth_P2 = 178;
        private const int frameHeight_P2 = 142;
        //

        public override void SetStaticDefaults()
        {
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.TrailCacheLength[Type] = NPC.oldPos.Length;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Scale = 0.4f,
                PortraitPositionYOverride = 3f
            };
            value.Position.Y += 3f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            NPCID.Sets.MPAllowedEnemies[Type] = true;
            if (!Main.dedServ)
            {
                Phase2Texture = ModContent.Request<Texture2D>(Texture + "P2", AssetRequestMode.AsyncLoad);
            }
        }

        public static int ShaderainDamage = 12; // 48

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 40; // 64 (1.6x expert scaling)
            NPC.npcSlots = 5f;

            // this is the only allowed case for using constant directly
            // as it doesn't have proper frame info yet
            NPC.width = frameWidth_P1;
            NPC.height = frameHeight_P1;

            NPC.defense = 8;
            NPC.LifeMaxNERB(5000, 7000, 350000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 5);
            NPC.boss = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            if (expertMode)
            {
                minimumDriftTime = 120;
                reelbackFade = 4;
            }

            if (revenge)
            {
                lungeRots = 0.3;
                minimumDriftTime = 90;
                reelbackFade = 5;
                lungeTime = 28;
                driftSpeed = 2f;
                driftBoost = 2f;
            }

            if (death)
            {
                lungeRots = 0.4;
                lungeAmount = 3;
                rainDashAmount = 2;
                minimumDriftTime = 60;
                reelbackFade = 6;
                lungeTime = 23;
                driftSpeed = 3.5f;
                driftBoost = 1.5f;
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
            index = IsPhaseTwo ? phase2IconIndex : normalIconIndex;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
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
            writer.Write(lungesPerformed);
            writer.Write(performingLungeCombo);
            writer.Write(rainDashesPerformed);
            writer.Write(performingRainDashCombo);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
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
            lungesPerformed = reader.ReadInt32();
            performingLungeCombo = reader.ReadBoolean();
            rainDashesPerformed = reader.ReadInt32();
            performingRainDashCombo = reader.ReadBoolean();
        }

        public override void FindFrame(int _)
        {
            maxFrameX = framesX_P1;
            maxFrameY = framesY_P1;
            frameWidth = frameWidth_P1;
            frameHeight = frameHeight_P1;

            if (IsPhaseTwo)
            {
                maxFrameX = framesX_P2;
                maxFrameY = framesY_P2;
                frameWidth = frameWidth_P2;
                frameHeight = frameHeight_P2;
            }

            NPC.frameCounter += 1.0; // Update each 6 ticks
            if (NPC.frameCounter >= 6.0)
            {
                NPC.frameCounter = 0.0; // Reset the counter

                // Increment Y frame
                frameY++;
                frameY %= maxFrameY;

                if (frameY == 0) // When reached start of Y -> Increment X
                {
                    // Increment X frame
                    frameX++;
                    frameX %= maxFrameX;
                }
            }

            NPC.frame = new Rectangle(frameWidth * frameX, frameHeight * frameY, NPC.width, NPC.height);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            if (Main.dedServ)
                return true;

            Texture2D texture = IsPhaseTwo ? Phase2Texture.Value : TextureAssets.Npc[Type].Value;
            SpriteEffects spriteEffects = NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = new Vector2(NPC.width / 2, NPC.height);
            Vector2 center = NPC.position - screenPos + origin;

            // AfterImage Effect for Phase Two
            if (CalamityClientConfig.Instance.Afterimages && state != 0 && IsPhaseTwo)
            {
                Color afterimageBaseColor = Color.White;
                int numAfterimages = 5;

                for (int i = 1; i < numAfterimages; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, afterimageBaseColor, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (numAfterimages - i) / 15f;
                    Vector2 afterimageCenter = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimageCenter -= new Vector2(texture.Width, texture.Height) / new Vector2(maxFrameX, maxFrameY) * NPC.scale / 2f;
                    afterimageCenter += origin * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture, afterimageCenter, NPC.frame, afterimageColor, NPC.oldRot[i], origin, NPC.scale, spriteEffects, 0f);
                }
            }

            spriteBatch.Draw(texture, center, NPC.frame, NPC.GetAlpha(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }

        private void SpawnStuff()
        {
            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            int maxSpawns = death ? Main.rand.Next(3, 5) : revenge ? 3 : expertMode ? Main.rand.Next(2, 4) : 2;
            for (int i = 0; i < maxSpawns; i++)
            {
                int type;
                int choice = -1;
                do
                {
                    choice++;
                    switch (choice)
                    {
                        case 0:
                            type = !NPC.AnyNPCs(NPCID.EaterofSouls) ? NPCID.EaterofSouls : 0;
                            break;

                        case 1:
                            type = !NPC.AnyNPCs(NPCID.DevourerHead) ? NPCID.DevourerHead : 0;
                            break;

                        case 2:
                            type = !NPC.AnyNPCs(ModContent.NPCType<DankCreeper>()) ? ModContent.NPCType<DankCreeper>() : 0;
                            break;

                        case 3:
                            type = (!NPC.AnyNPCs(NPCID.Corruptor) && death) ? NPCID.Corruptor : 0;
                            break;

                        default:
                            type = -1;
                            break;
                    }
                }
                while (type == 0);

                if (type > 0)
                    NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + Main.rand.Next(NPC.width), (int)NPC.position.Y + Main.rand.Next(NPC.height), type);
            }

            // Spawn a Hive Cyst
            if (Main.zenithWorld && NPC.CountNPCS(ModContent.NPCType<HiveTumor>()) < 3)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X + Main.rand.Next(NPC.width), (int)NPC.position.Y + Main.rand.Next(NPC.height), ModContent.NPCType<HiveTumor>());
        }

        private void ReelBack()
        {
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;

            NPC.alpha = 0;
            phase2timer = 0;
            deceleration = NPC.velocity / 255f * reelbackFade;

            if (revenge)
            {
                state = 2;
                SoundEngine.PlaySound(FastRoarSound, NPC.Center);
            }
            else
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (NPC.Distance(Main.player[NPC.target].Center) > 80f)
                        SpawnStuff();
                }

                state = nextState;
                nextState = 0;

                if (state == 2)
                    SoundEngine.PlaySound(RoarSound, NPC.Center);
                else
                    SoundEngine.PlaySound(FastRoarSound, NPC.Center);
            }
            NPC.netUpdate = true;
        }

        public override void AI()
        {
            // Update NPC Size according to Phase Changes
            NPC.width = frameWidth;
            NPC.height = frameHeight - 2; //Phase 1 Sprite have 2 pixel margin

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Phase 2 settings
            if (IsPhaseTwo)
            {
                // Spawn gores, play sound and reset every crucial variable at the start
                if (NPC.localAI[1] == 0f)
                {
                    NPC.localAI[1] = 1f;

                    if (!Main.dedServ)
                    {
                        int goreAmount = 7;
                        for (int i = 1; i <= goreAmount; i++)
                            Gore.NewGore(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("HiveMindGore" + i).Type, 1f);
                    }

                    for (int i = 0; i < 20; i++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].velocity *= 3f;
                        if (Main.rand.NextBool())
                        {
                            Main.dust[dust].scale = 0.5f;
                            Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int j = 0; j < 35; j++)
                    {
                        int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 3f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 5f;
                        dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                        Main.dust[dust].velocity *= 2f;
                    }

                    SoundEngine.PlaySound(SoundID.NPCDeath1, NPC.Center);

                    NPC.position = NPC.Center;
                    NPC.position -= NPC.Size * 0.5f;

                    NPC.noGravity = true;
                    NPC.noTileCollide = true;
                    NPC.scale = 1f;
                    NPC.alpha = 0;
                    NPC.dontTakeDamage = false;
                    NPC.damage = 0;
                    NPC.ForceNetUpdate();
                }
            }
            else
            {
                // Avoid cheap bullshit
                NPC.damage = 0;

                CalamityGlobalNPC.hiveMind = NPC.whoAmI;

                if (!player.active || player.dead || !(player.ZoneCorrupt || BossRushEvent.BossRushActive))
                {
                    NPC.TargetClosest(false);
                    player = Main.player[NPC.target];
                    if (!player.active || player.dead || !(player.ZoneCorrupt || BossRushEvent.BossRushActive))
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
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<HiveBlob>(), NPC.whoAmI, ai2: Main.rand.Next(2));
                    }
                }

                if (NPC.ai[3] == 0f && NPC.life > 0)
                    NPC.ai[3] = NPC.lifeMax;

                if (NPC.life > 0)
                {
                    int fivePercentHP = (int)(NPC.lifeMax * 0.05);
                    if ((NPC.life + fivePercentHP) < NPC.ai[3])
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath22, NPC.Center);

                        NPC.ai[3] = NPC.life;

                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 35; j++)
                        {
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 3f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 5f;
                            dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 2f;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int maxSpawns = death ? 5 : revenge ? 4 : expertMode ? Main.rand.Next(3, 5) : Main.rand.Next(2, 4);
                            int maxDankSpawns = death ? Main.rand.Next(2, 4) : revenge ? 2 : expertMode ? Main.rand.Next(1, 3) : 1;

                            for (int i = 0; i < maxSpawns; i++)
                            {
                                int x = (int)(NPC.position.X + Main.rand.Next(NPC.width - 32));
                                int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height - 32));

                                int type = ModContent.NPCType<HiveBlob>();
                                if (NPC.CountNPCS(ModContent.NPCType<DankCreeper>()) < maxDankSpawns)
                                    type = ModContent.NPCType<DankCreeper>();

                                int fivePercentMinions = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, type);
                                Main.npc[fivePercentMinions].SetDefaults(type);
                                if (Main.dedServ && fivePercentMinions < Main.maxNPCs)
                                    NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, fivePercentMinions);
                            }

                            return;
                        }
                    }
                }

                burrowTimer--;
                if (burrowTimer < -120)
                {
                    burrowTimer = death ? 180 : revenge ? 300 : expertMode ? 360 : 420;
                    if (burrowTimer < 30)
                        burrowTimer = 30;

                    NPC.scale = 1f;
                    NPC.alpha = 0;
                    NPC.dontTakeDamage = false;
                }
                else if (burrowTimer < -60)
                {
                    NPC.scale += 0.0165f;
                    NPC.alpha -= 4;
                    if (NPC.alpha < 0)
                        NPC.alpha = 0;

                    int burrowedDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
                    Main.dust[burrowedDust].velocity *= 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[burrowedDust].scale = 0.5f;
                        Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        int burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 3.5f * NPC.scale);
                        Main.dust[burrowedDust2].noGravity = true;
                        Main.dust[burrowedDust2].velocity *= 3.5f;
                        burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
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

                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            NPC hiveBlob = Main.npc[i];
                            if (hiveBlob.active && hiveBlob.type == ModContent.NPCType<HiveBlob>())
                            {
                                hiveBlob.position.X = NPC.position.X;
                                hiveBlob.position.Y = NPC.position.Y;
                            }
                        }
                    }
                    NPC.ForceNetUpdate();
                }
                else if (burrowTimer < 0)
                {
                    NPC.scale -= 0.0165f;
                    NPC.alpha += 4;
                    if (NPC.alpha > 255)
                        NPC.alpha = 255;

                    int burrowedDust = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
                    Main.dust[burrowedDust].velocity *= 2f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[burrowedDust].scale = 0.5f;
                        Main.dust[burrowedDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }

                    for (int i = 0; i < 2; i++)
                    {
                        int burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 3.5f * NPC.scale);
                        Main.dust[burrowedDust2].noGravity = true;
                        Main.dust[burrowedDust2].velocity *= 3.5f;
                        burrowedDust2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.Center.Y), NPC.width, NPC.height / 2, DustID.Demonite, 0f, -3f, 100, default, 2.5f * NPC.scale);
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
                    }
                }

                return;
            }

            switch (state)
            {
                case 0: // Slowdrift

                    // Set damage
                    NPC.damage = NPC.defDamage;

                    if (NPC.alpha > 0)
                    {
                        NPC.alpha -= 3;
                        if (NPC.alpha < 0)
                            NPC.alpha = 0;
                    }

                    if (nextState == 0)
                    {
                        NPC.TargetClosest();
                        if (revenge && lifeRatio < 0.6f)
                        {
                            if (death)
                            {
                                do
                                    nextState = Main.rand.Next(3, 6);
                                while (nextState == previousState);
                                previousState = nextState;
                            }
                            else if (lifeRatio < 0.3f)
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
                                if (expertMode && reelCount == 2)
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

                        NPC.ForceNetUpdate();
                    }

                    if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 8000f || !(player.ZoneCorrupt || BossRushEvent.BossRushActive))
                    {
                        NPC.TargetClosest(false);
                        player = Main.player[NPC.target];
                        if (!player.active || player.dead || Vector2.Distance(NPC.Center, player.Center) > 8000f || !(player.ZoneCorrupt || BossRushEvent.BossRushActive))
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

                    if (death)
                    {
                        // Use an attack sooner if being hit
                        if (NPC.justHit)
                            phase2timer -= 4;
                    }

                    if (phase2timer <= -180) // No stalling drift mode forever
                    {
                        NPC.velocity *= 2f / 255f * reelbackFade;
                        ReelBack();
                        NPC.ForceNetUpdate();
                    }
                    else
                    {
                        NPC.velocity.Normalize();
                        if (expertMode) // Variable velocity in expert and up
                            NPC.velocity *= driftSpeed + driftBoost * lifeRatio;
                        else
                            NPC.velocity *= driftSpeed;
                    }

                    break;

                case 1: // Reelback and teleport

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.alpha += reelbackFade;
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

                        phase2timer = minimumDriftTime + Main.rand.Next(death ? 61 : 121);
                        NPC.ForceNetUpdate();
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
                                NPC.ForceNetUpdate();
                                break;
                            }
                        }
                    }

                    break;

                case 2: // Reelback for lunge + death legacy

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.alpha += reelbackFade;
                    NPC.velocity -= deceleration;

                    if (NPC.alpha >= 255)
                    {
                        NPC.alpha = 255;
                        NPC.velocity = Vector2.Zero;
                        dashStarted = false;

                        if (revenge && lifeRatio < 0.6f && lungesPerformed == 0 && rainDashesPerformed == 0)
                        {
                            state = nextState;
                            nextState = 0;
                            previousState = state;
                        }
                        else
                            state = performingRainDashCombo ? 5 : 3;

                        if (!performingRainDashCombo)
                        {
                            if (player.velocity.X > 0)
                                rotationDirection = 1;
                            else if (player.velocity.X < 0)
                                rotationDirection = -1;
                            else
                                rotationDirection = player.direction;
                        }
                    }

                    break;

                case 3: // Lunge

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.ForceNetUpdate();
                    if (NPC.alpha > 0)
                    {
                        NPC.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);
                        rotation += rotationIncrement * rotationDirection;
                        phase2timer = lungesPerformed > 0 ? (lungeDelay / (lungesPerformed + 1)) : lungeDelay;

                        NPC.alpha -= lungeFade;
                        if (NPC.alpha < 0)
                            NPC.alpha = 0;
                    }
                    else
                    {
                        phase2timer--;
                        if (!dashStarted)
                        {
                            if (phase2timer <= 0)
                            {
                                // Set damage
                                NPC.damage = NPC.defDamage;

                                phase2timer = lungeTime;
                                NPC.velocity = player.Center - NPC.Center;
                                NPC.velocity.Normalize();
                                NPC.velocity *= teleportRadius / lungeTime;
                                dashStarted = true;
                                SoundEngine.PlaySound(RoarSound, NPC.Center);
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.Center = player.Center + new Vector2(teleportRadius, 0).RotatedBy(rotation);
                                rotation += rotationIncrement * rotationDirection * phase2timer / lungeDelay;
                            }
                        }
                        else
                        {
                            // Set damage
                            NPC.damage = NPC.defDamage;

                            if (phase2timer <= 0)
                            {
                                // Avoid cheap bullshit
                                NPC.damage = 0;

                                performingLungeCombo = death;
                                state = 6;
                                phase2timer = 0;
                                deceleration = NPC.velocity / decelerationTime;
                            }
                        }
                    }

                    break;

                case 4: // Enemy spawn arc

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (NPC.alpha > 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.Center = player.Center;
                            NPC.position.Y += teleportRadius;
                        }

                        NPC.alpha -= 5;
                        if (NPC.alpha < 0)
                            NPC.alpha = 0;

                        NPC.ForceNetUpdate();
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            // Set damage
                            NPC.damage = NPC.defDamage;

                            dashStarted = true;
                            SoundEngine.PlaySound(RoarSound, NPC.Center);
                            NPC.velocity.X = MathHelper.Pi * teleportRadius / arcTime;
                            NPC.velocity *= rotationDirection;
                            NPC.ForceNetUpdate();
                        }
                        else
                        {
                            // Set damage
                            NPC.damage = NPC.defDamage;

                            NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);

                            phase2timer++;
                            if (phase2timer == (int)(arcTime / 6f))
                            {
                                phase2timer = 0;
                                NPC.ai[0] += 1f;
                                if (NPC.ai[0] == 6f)
                                {
                                    NPC.velocity = NPC.velocity.RotatedBy(MathHelper.Pi / arcTime * -rotationDirection);

                                    if (NPC.Distance(Main.player[NPC.target].Center) > 80f)
                                        SpawnStuff();

                                    state = 6;
                                    NPC.ai[0] = 0f;
                                    deceleration = NPC.velocity / decelerationTime;
                                }
                            }
                        }
                    }

                    break;

                case 5: // Rain dash

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    if (NPC.alpha > 0)
                    {
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.Center = player.Center;
                            NPC.position.Y -= teleportRadius;
                            NPC.position.X += (death ? (teleportRadius * 1.5f) : teleportRadius) * (rainDashesPerformed == 0 ? rotationDirection : -rotationDirection);
                        }

                        NPC.alpha -= 5;
                        if (NPC.alpha < 0)
                            NPC.alpha = 0;

                        NPC.ForceNetUpdate();
                    }
                    else
                    {
                        if (!dashStarted)
                        {
                            // Set damage
                            NPC.damage = NPC.defDamage;

                            dashStarted = true;
                            SoundEngine.PlaySound(RoarSound, NPC.Center);
                            NPC.velocity.X = teleportRadius / arcTime * 3;
                            NPC.velocity *= (rainDashesPerformed == 0 ? -rotationDirection : rotationDirection);
                            NPC.ForceNetUpdate();
                        }
                        else
                        {
                            // Set damage
                            NPC.damage = NPC.defDamage;

                            phase2timer++;
                            if (phase2timer % 30 == (int)(arcTime / (death ? 15f : 20f)))
                            {
                                phase2timer = 0;
                                NPC.ai[0] += 1f;
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    int type = ModContent.ProjectileType<ShadeNimbusHostile>();
                                    Vector2 cloudSpawnPos = new Vector2(NPC.position.X + Main.rand.Next(NPC.width), NPC.position.Y + Main.rand.Next(NPC.height));
                                    Vector2 randomVelocity = Main.getGoodWorld ? Main.rand.NextVector2CircularEdge(4f, 4f) : Vector2.Zero;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), cloudSpawnPos, randomVelocity, type, ShaderainDamage, 0, Main.myPlayer, 11f);
                                }

                                if (NPC.ai[0] == 10f)
                                {
                                    performingRainDashCombo = death;
                                    state = 6;
                                    NPC.ai[0] = 0f;
                                    deceleration = NPC.velocity / decelerationTime;
                                }
                            }
                        }
                    }

                    break;

                case 6: // Deceleration

                    // Avoid cheap bullshit
                    NPC.damage = 0;

                    NPC.velocity -= deceleration;
                    phase2timer++;
                    if (phase2timer == decelerationTime)
                    {
                        if (performingRainDashCombo)
                        {
                            rainDashesPerformed++;
                            if (rainDashesPerformed < rainDashAmount)
                            {
                                state = 2;
                            }
                            else
                            {
                                phase2timer = minimumDriftTime + Main.rand.Next(death ? 61 : 121);
                                performingRainDashCombo = false;
                                rainDashesPerformed = 0;
                                state = 0;
                            }
                        }
                        else if (performingLungeCombo)
                        {
                            lungesPerformed++;
                            if (lungesPerformed < lungeAmount)
                            {
                                state = 2;
                            }
                            else
                            {
                                phase2timer = minimumDriftTime + Main.rand.Next(death ? 61 : 121);
                                performingLungeCombo = false;
                                lungesPerformed = 0;
                                state = 0;
                            }
                        }
                        else
                        {
                            phase2timer = minimumDriftTime + Main.rand.Next(death ? 61 : 121);
                            state = 0;
                        }

                        NPC.ForceNetUpdate();
                    }

                    break;
            }

            // Dark Heart spawns in phase 2
            if (NPC.life > 0 && expertMode && lifeRatio < 0.5f)
            {
                if (NPC.ai[3] > NPC.lifeMax * 0.6f)
                    NPC.ai[3] = NPC.lifeMax * 0.6f;

                int tenPercentHP = (int)(NPC.lifeMax * 0.1);
                if ((NPC.life + tenPercentHP) < NPC.ai[3])
                {
                    NPC.ai[3] = NPC.life;

                    if (!NPC.AnyNPCs(ModContent.NPCType<DarkHeart>()))
                    {
                        SoundEngine.PlaySound(SoundID.NPCDeath22, NPC.Center);

                        for (int i = 0; i < 20; i++)
                        {
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 3f;
                            if (Main.rand.NextBool())
                            {
                                Main.dust[dust].scale = 0.5f;
                                Main.dust[dust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                            }
                        }
                        for (int j = 0; j < 35; j++)
                        {
                            int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 3f);
                            Main.dust[dust].noGravity = true;
                            Main.dust[dust].velocity *= 5f;
                            dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                            Main.dust[dust].velocity *= 2f;
                        }

                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            int x = (int)(NPC.position.X + Main.rand.Next(NPC.width));
                            int y = (int)(NPC.position.Y + Main.rand.Next(NPC.height));
                            int type = ModContent.NPCType<DarkHeart>();
                            int tenPercentMinions = NPC.NewNPC(NPC.GetSource_FromAI(), x, y, type);
                            Main.npc[tenPercentMinions].SetDefaults(type);
                            if (Main.dedServ && tenPercentMinions < Main.maxNPCs)
                                NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, tenPercentMinions);
                        }
                    }
                }
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

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage < 0)
                return;

            target.AddBuff(ModContent.BuffType<BrainRot>(), 240);
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position) => NPC.scale == 1f; // Only draw HP bar while at full size

        public override void ModifyIncomingHit(ref NPC.HitModifiers modifiers)
        {
            if (phase2timer < 0)
            {
                NPC.velocity *= -4f;
                ReelBack();
                NPC.ForceNetUpdate();
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
            NPC.damage = (int)(NPC.damage * 0.8f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            for (int k = 0; k < hit.Damage / NPC.lifeMax * 100.0; k++)
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, hit.HitDirection, -1f, 0, default, 1f);

            if (!IsPhaseTwo && NPC.Distance(Main.player[NPC.target].Center) > 80f)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (Main.rand.NextBool(30))
                    {
                        if (NPC.CountNPCS(NPCID.EaterofSouls) < 2)
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.EaterofSouls);
                    }

                    if (Main.rand.NextBool(45))
                    {
                        if (!NPC.AnyNPCs(NPCID.DevourerHead))
                            NPC.NewNPC(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.DevourerHead);
                    }
                }
            }

            if (NPC.life <= 0)
            {
                if (!Main.dedServ)
                {
                    int goreAmount = 10;
                    for (int i = 1; i <= goreAmount; i++)
                        Gore.NewGore(NPC.GetSource_Death(), NPC.Center, NPC.velocity, Mod.Find<ModGore>("HiveMindP2Gore" + i).Type, 1f);
                }

                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 200;
                NPC.height = 150;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int killDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                    Main.dust[killDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[killDust].scale = 0.5f;
                        Main.dust[killDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int killDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 3f);
                    Main.dust[killDust2].noGravity = true;
                    Main.dust[killDust2].velocity *= 5f;
                    killDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Demonite, 0f, 0f, 100, default, 2f);
                    Main.dust[killDust2].velocity *= 2f;
                }
            }
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

            // If neither The Hive Mind nor The Perforator Hive have been killed yet, notify players of Aerialite Ore
            if (!DownedBossSystem.downedHiveMind && !DownedBossSystem.downedPerforator)
            {
                string key = "Mods.CalamityMod.Status.Progression.SkyOreText";
                Color messageColor = Color.Cyan;
                AerialiteOreGen.Enchant();

                CalamityUtils.BroadcastLocalizedText(key, messageColor);
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
                    ModContent.ItemType<RotBall>(),
                }));

                // Materials
                normalOnly.Add(ItemID.DemoniteBar, 1, 10, 15);
                normalOnly.Add(ItemID.RottenChunk, 1, 10, 15);
                normalOnly.Add(ItemID.CorruptSeeds, 1, 10, 15);
                normalOnly.Add(ItemDropRule.ByCondition(DropHelper.Hardmode(), ItemID.CursedFlame, 1, 10, 20));

                // Equipment
                normalOnly.Add(ModContent.ItemType<FilthyGlove>(), DropHelper.NormalWeaponDropRateFraction);
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<RottenBrain>()));

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
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.WarriorEmblem), hideLootReport: true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.RangerEmblem), hideLootReport: true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.SorcererEmblem), hideLootReport: true);
                GFBOnly.Add(DropHelper.PerPlayer(ItemID.SummonerEmblem), hideLootReport: true);
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<RogueEmblem>()), hideLootReport: true);
            }

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedHiveMind, ModContent.ItemType<LoreHiveMind>(), desc: DropHelper.FirstKillText);
        }
    }
}
