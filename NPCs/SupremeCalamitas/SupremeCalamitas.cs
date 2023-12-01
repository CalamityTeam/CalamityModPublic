using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Items;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.DevPaintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Projectiles.Boss;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent.ItemDropRules;

namespace CalamityMod.NPCs.SupremeCalamitas
{
    public class SupremeCalamitas : ModNPC
    {
        public enum FrameAnimationType
        {
            // The numbering of these values correspond to a frame on the sprite. If this enumeration or the sprite itself
            // is updated, these numbers will need to be too.
            UpwardDraft = 0,
            FasterUpwardDraft = 1,
            Casting = 2,
            BlastCast = 3,
            BlastPunchCast = 4,
            OutwardHandCast = 5,
            PunchHandCast = 6,
            Count = 7
        }

        public const int BulletHellDuration = 900;
        public const int SecondBulletHellEndValue = BulletHellDuration * 2;
        public const int ThirdBulletHellEndValue = BulletHellDuration * 3;
        public const int FourthBulletHellEndValue = BulletHellDuration * 4;
        public const int FifthBulletHellEndValue = BulletHellDuration * 5;
        public const int CirrusPhotonRipperDamage = 3725;
        private const float CirrusPhotonRipperDashVelocity = 6f;
        private const float CirrusPhotonRipperMinDistanceFromTarget = 64f;
        private const float CirrusPhotonRipperDashAcceleration = 0.3f;

        public float bossLife;
        public float uDieLul = 1f;
        public float passedVar = 0f;

        public bool protectionBoost = false;
        public bool canDespawn = false;
        public bool despawnProj = false;
        public bool startText = false;
        public bool startBattle = false; //100%
        public bool hasSummonedSepulcher1 = false; //100%
        public bool startSecondAttack = false; //80%
        public bool startThirdAttack = false; //60%
        public bool halfLife = false; //40%
        public bool startFourthAttack = false; //30%
        public bool secondStage = false; //20%
        public bool startFifthAttack = false; //10%
        public bool gettingTired = false; //8%
        public bool hasSummonedSepulcher2 = false; //8%
        public bool gettingTired2 = false; //6%
        public bool gettingTired3 = false; //4%
        public bool gettingTired4 = false; //2%
        public bool gettingTired5 = false; //1%
        public bool willCharge = false;
        public bool canFireSplitingFireball = true;
        public bool spawnArena = false;
        public bool enteredBrothersPhase = false;
        public bool hasSummonedBrothers = false;
        public bool cirrus = false;

        private const int GiveUpCounterMax = 1200;
        public int giveUpCounter = GiveUpCounterMax;
        public int phaseChange = 0;
        public int spawnX = 0;
        public int spawnX2 = 0;
        public int spawnXReset = 0;
        public int spawnXReset2 = 0;
        public int spawnXAdd = 200;
        public int spawnY = 0;
        public int spawnYReset = 0;
        public int spawnYAdd = 0;
        public int bulletHellCounter = 0;
        public int bulletHellCounter2 = 0;
        public int attackCastDelay = 0;
        public int hitTimer = 0;
        public int alicornFrame = 0;
        public int alicornFrameCounter = 0;

        public float shieldOpacity = 1f;
        public float shieldRotation = 0f;
        public float forcefieldOpacity = 1f;
        public float forcefieldScale = 1;

        public FrameAnimationType FrameType
        {
            get => (FrameAnimationType)(int)NPC.localAI[2];
            set => NPC.localAI[2] = (int)value;
        }

        public bool AttackCloseToBeingOver
        {
            get
            {
                int attackLength = 0;

                // First phase.
                if (NPC.ai[0] == 0f)
                {
                    if (NPC.ai[1] == 0f)
                        attackLength = 300;
                    if (NPC.ai[1] == 2f)
                        attackLength = 70;
                    if (NPC.ai[1] == 3f)
                        attackLength = 480;
                    if (NPC.ai[1] == 4f)
                        attackLength = 300;
                }
                else
                {
                    if (NPC.ai[1] == 0f)
                        attackLength = 240;
                    if (NPC.ai[1] == 2f)
                        attackLength = 70;
                    if (NPC.ai[1] == 3f)
                        attackLength = 300;
                    if (NPC.ai[1] == 4f)
                        attackLength = 240;
                }
                return NPC.ai[2] >= attackLength - 30f;
            }
        }

        public ref float FrameChangeSpeed => ref NPC.localAI[3];

        public Vector2 cataclysmSpawnPosition;
        public Vector2 catastropheSpawnPosition;
        public Vector2 initialRitualPosition;
        public Rectangle safeBox = default;

        public bool IsTargetOutsideOfArena => !Main.player[NPC.target].Hitbox.Intersects(safeBox);

        public static int hoodedHeadIconIndex;
        public static int hoodedHeadIconP2Index;
        public static int hoodlessHeadIconIndex;
        public static int hoodlessHeadIconP2Index;
        public static int cirrusHeadIconIndex;
        public static int cirrusHeadIconP2Index;
        public static float normalDR = 0.25f;
        public static float enragedDR = 0.9999f;

        public static readonly Color textColor = Color.Orange;
        public static readonly Color cirrusTextColor = Color.Pink;
        public const int sepulcherSpawnCastTime = 75;
        public const int brothersSpawnCastTime = 150;
        public const int MaxCirrusAlcohols = 20;
        public const int MaxCirrusAlcoholDebuffDuration = 1500;
        
        // Sounds.
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/SupremeCalamitasSpawn") { Volume = 1.2f }; 
        public static readonly SoundStyle SepulcherSummonSound = new("CalamityMod/Sounds/Custom/SCalSounds/SepulcherSpawn");
        public static readonly SoundStyle BrimstoneShotSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneShoot");
        public static readonly SoundStyle BrimstoneBigShotSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneBigShoot"); // DON'T YOU WANNA BE A [BIG SHOT]
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/SCalSounds/SCalDash");
        public static readonly SoundStyle HellblastSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneHellblastSound");
        public static readonly SoundStyle HurtSound = new("CalamityMod/Sounds/NPCHit/ShieldHit", 3);

        // TODO -- This is cumbersome. Change it to be better in 1.4.
        internal static void LoadHeadIcons()
        {
            string hoodedIconPath = "CalamityMod/NPCs/SupremeCalamitas/HoodedHeadIcon";
            string hoodedIconP2Path = "CalamityMod/NPCs/SupremeCalamitas/HoodedHeadIconP2";
            string hoodlessIconPath = "CalamityMod/NPCs/SupremeCalamitas/HoodlessHeadIcon";
            string hoodlessIconP2Path = "CalamityMod/NPCs/SupremeCalamitas/HoodlessHeadIconP2";
            string cirrusIconPath = "CalamityMod/NPCs/SupremeCalamitas/CirrusHeadIcon";
            string cirrusIconP2Path = "CalamityMod/NPCs/SupremeCalamitas/CirrusHeadIcon2";

            CalamityMod.Instance.AddBossHeadTexture(hoodedIconPath, -1);
            hoodedHeadIconIndex = ModContent.GetModBossHeadSlot(hoodedIconPath);

            CalamityMod.Instance.AddBossHeadTexture(hoodedIconP2Path, -1);
            hoodedHeadIconP2Index = ModContent.GetModBossHeadSlot(hoodedIconP2Path);

            CalamityMod.Instance.AddBossHeadTexture(hoodlessIconPath, -1);
            hoodlessHeadIconIndex = ModContent.GetModBossHeadSlot(hoodlessIconPath);

            CalamityMod.Instance.AddBossHeadTexture(hoodlessIconP2Path, -1);
            hoodlessHeadIconP2Index = ModContent.GetModBossHeadSlot(hoodlessIconP2Path);

            CalamityMod.Instance.AddBossHeadTexture(cirrusIconPath, -1);
            cirrusHeadIconIndex = ModContent.GetModBossHeadSlot(cirrusIconPath);

            CalamityMod.Instance.AddBossHeadTexture(cirrusIconP2Path, -1);
            cirrusHeadIconP2Index = ModContent.GetModBossHeadSlot(cirrusIconP2Path);
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 21;
            NPCID.Sets.TrailingMode[NPC.type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                SpriteDirection = 1
            };
            value.Position.Y += 14f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
        }

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.GetNPCDamage();
            NPC.npcSlots = 50f;
            NPC.width = NPC.height = 44;
            NPC.defense = 100;
            NPC.DR_NERD(normalDR);
            NPC.value = Item.buyPrice(30, 0, 0, 0);
            NPC.LifeMaxNERB(960000, 1150000, 900000);
            double HPBoost = CalamityConfig.Instance.BossHealthBoost * 0.01;
            NPC.lifeMax += (int)(NPC.lifeMax * HPBoost);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = false;
            NPC.chaseable = true;
            NPC.boss = true;
            NPC.canGhostHeal = false;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.Calamity().VulnerableToHeat = false;
            NPC.Calamity().VulnerableToCold = true;
            NPC.Calamity().VulnerableToSickness = true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] 
            {
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SupremeCalamitas")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            bool inPhase2 = NPC.ai[0] == 3f;
            if (cirrus)
            {
                index = inPhase2 ? cirrusHeadIconP2Index : cirrusHeadIconIndex;
            }
            else
            {
                if (!DownedBossSystem.downedCalamitas || BossRushEvent.BossRushActive)
                    index = inPhase2 ? hoodedHeadIconP2Index : hoodedHeadIconIndex;
                else
                    index = inPhase2 ? hoodlessHeadIconP2Index : hoodlessHeadIconIndex;
            }
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (cirrus)
                typeName = CalamityUtils.GetTextValue("NPCs.SupremeCirrus");
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(protectionBoost);
            writer.Write(canDespawn);
            writer.Write(despawnProj);
            writer.Write(startText);
            writer.Write(startBattle);
            writer.Write(hasSummonedSepulcher1);
            writer.Write(startSecondAttack);
            writer.Write(startThirdAttack);
            writer.Write(startFourthAttack);
            writer.Write(startFifthAttack);
            writer.Write(halfLife);
            writer.Write(secondStage);
            writer.Write(hasSummonedSepulcher2);
            writer.Write(gettingTired);
            writer.Write(gettingTired2);
            writer.Write(gettingTired3);
            writer.Write(gettingTired4);
            writer.Write(gettingTired5);
            writer.Write(willCharge);
            writer.Write(canFireSplitingFireball);
            writer.Write(spawnArena);
            writer.Write(hasSummonedBrothers);
            writer.Write(enteredBrothersPhase);
            writer.Write(cirrus);
            writer.Write(NPC.dontTakeDamage);
            writer.Write(NPC.chaseable);

            writer.Write(giveUpCounter);
            writer.Write(phaseChange);
            writer.Write(spawnX);
            writer.Write(spawnX2);
            writer.Write(spawnXReset);
            writer.Write(spawnXReset2);
            writer.Write(spawnXAdd);
            writer.Write(spawnY);
            writer.Write(spawnYReset);
            writer.Write(spawnYAdd);
            writer.Write(bulletHellCounter);
            writer.Write(bulletHellCounter2);
            writer.Write(hitTimer);
            writer.Write(attackCastDelay);

            writer.Write(shieldRotation);

            writer.Write(safeBox.X);
            writer.Write(safeBox.Y);
            writer.Write(safeBox.Width);
            writer.Write(safeBox.Height);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            protectionBoost = reader.ReadBoolean();
            canDespawn = reader.ReadBoolean();
            despawnProj = reader.ReadBoolean();
            startText = reader.ReadBoolean();
            startBattle = reader.ReadBoolean();
            hasSummonedSepulcher1 = reader.ReadBoolean();
            startSecondAttack = reader.ReadBoolean();
            startThirdAttack = reader.ReadBoolean();
            startFourthAttack = reader.ReadBoolean();
            startFifthAttack = reader.ReadBoolean();
            halfLife = reader.ReadBoolean();
            secondStage = reader.ReadBoolean();
            hasSummonedSepulcher2 = reader.ReadBoolean();
            gettingTired = reader.ReadBoolean();
            gettingTired2 = reader.ReadBoolean();
            gettingTired3 = reader.ReadBoolean();
            gettingTired4 = reader.ReadBoolean();
            gettingTired5 = reader.ReadBoolean();
            willCharge = reader.ReadBoolean();
            canFireSplitingFireball = reader.ReadBoolean();
            spawnArena = reader.ReadBoolean();
            hasSummonedBrothers = reader.ReadBoolean();
            enteredBrothersPhase = reader.ReadBoolean();
            cirrus = reader.ReadBoolean();
            NPC.dontTakeDamage = reader.ReadBoolean();
            NPC.chaseable = reader.ReadBoolean();

            giveUpCounter = reader.ReadInt32();
            phaseChange = reader.ReadInt32();
            spawnX = reader.ReadInt32();
            spawnX2 = reader.ReadInt32();
            spawnXReset = reader.ReadInt32();
            spawnXReset2 = reader.ReadInt32();
            spawnXAdd = reader.ReadInt32();
            spawnY = reader.ReadInt32();
            spawnYReset = reader.ReadInt32();
            spawnYAdd = reader.ReadInt32();
            bulletHellCounter = reader.ReadInt32();
            bulletHellCounter2 = reader.ReadInt32();
            hitTimer = reader.ReadInt32();
            attackCastDelay = reader.ReadInt32();

            shieldRotation = reader.ReadSingle();

            safeBox = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
        }

        public override void AI()
        {
            #region Resets

            // Use an ordinary upward draft by default.
            FrameType = FrameAnimationType.UpwardDraft;
            FrameChangeSpeed = 0.15f;
            #endregion
            #region StartUp

            CalamityGlobalNPC.SCal = NPC.whoAmI;
			HandleMusicVariables();

            bool wormAlive = false;
            if (CalamityGlobalNPC.SCalWorm != -1)
                wormAlive = Main.npc[CalamityGlobalNPC.SCalWorm].active;

            bool cataclysmAlive = false;
            if (CalamityGlobalNPC.SCalCataclysm != -1)
                cataclysmAlive = Main.npc[CalamityGlobalNPC.SCalCataclysm].active;

            bool catastropheAlive = false;
            if (CalamityGlobalNPC.SCalCatastrophe != -1)
                catastropheAlive = Main.npc[CalamityGlobalNPC.SCalCatastrophe].active;

            if (Main.slimeRain)
            {
                Main.StopSlimeRain(true);
                CalamityNetcode.SyncWorld();
            }

            if (CalamityConfig.Instance.BossesStopWeather)
                CalamityMod.StopRain();

            bool bossRush = BossRushEvent.BossRushActive;
            bool expertMode = Main.expertMode || bossRush;
            bool revenge = CalamityWorld.revenge || bossRush;
            bool death = CalamityWorld.death || bossRush;

            // cirrus and zenith scal are mutually exclusive unless it's legendary
            bool zenithAI = Main.zenithWorld && (!cirrus || (CalamityWorld.LegendaryMode && revenge && cirrus));

            // Percent life remaining
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            // Projectile damage values
            int bulletHellblastDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<BrimstoneHellblast2>());
            int barrageDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<BrimstoneBarrage>());
            int gigablastDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<SCalBrimstoneFireblast>());
            int fireblastDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<SCalBrimstoneGigablast>());
            int monsterDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<BrimstoneMonster>());
            int waveDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<BrimstoneWave>());
            int hellblastDamage = NPC.GetProjectileDamage(ModContent.ProjectileType<BrimstoneHellblast>());
			if (bossRush)
			{
				bulletHellblastDamage /= 2;
				barrageDamage /= 2;
				gigablastDamage /= 2;
				fireblastDamage /= 2;
				monsterDamage /= 2;
				waveDamage /= 2;
				hellblastDamage /= 2;
            }

            int bulletHellblast = zenithAI ? ModContent.ProjectileType<BrimstoneWave>() : ModContent.ProjectileType<BrimstoneHellblast2>();
            int barrage = ModContent.ProjectileType<BrimstoneBarrage>();
            int gigablast = zenithAI ? ModContent.ProjectileType<SCalBrimstoneFireblast>() : ModContent.ProjectileType<SCalBrimstoneGigablast>();
            int fireblast = zenithAI ? ModContent.ProjectileType<SCalBrimstoneGigablast>() : ModContent.ProjectileType<SCalBrimstoneFireblast>();
            int wave = zenithAI ? ModContent.ProjectileType<BrimstoneHellblast2>() : ModContent.ProjectileType<BrimstoneWave>();
            int hellblast = zenithAI ? ModContent.ProjectileType<BrimstoneWave>() : ModContent.ProjectileType<BrimstoneHellblast>();

            int bodyWidth = 44;
            int bodyHeight = 42;
            int baseBulletHellProjectileGateValue = revenge ? 8 : expertMode ? 9 : 10;

            if (Main.getGoodWorld)
                baseBulletHellProjectileGateValue -= 2;

            Vector2 vectorCenter = NPC.Center;

            // Get a target
            if (NPC.target < 0 || NPC.target == Main.maxPlayers || Main.player[NPC.target].dead || !Main.player[NPC.target].active)
                NPC.TargetClosest();

            // Despawn safety, make sure to target another player if the current player target is too far away
            if (Vector2.Distance(Main.player[NPC.target].Center, NPC.Center) > CalamityGlobalNPC.CatchUpDistance200Tiles)
                NPC.TargetClosest();

            Player player = Main.player[NPC.target];

            if (!startText)
            {
                if (!bossRush)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalSummonText";
                    if (cirrus)
                        key = "Mods.CalamityMod.Status.Boss.CirrusSummonText";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";
                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }
                startText = true;
            }
            #endregion
            #region Directioning

            bool currentlyCharging = NPC.ai[1] == 2f;
            if (!currentlyCharging && Math.Abs(player.Center.X - NPC.Center.X) > 16f)
                NPC.spriteDirection = (player.Center.X < NPC.Center.X).ToDirectionInt();
            #endregion
            #region Forcefield and Shield Logic

            if (hitTimer > 0)
                hitTimer--;

            Vector2 hitboxSize = new Vector2(forcefieldScale * 216f / 1.4142f);
            hitboxSize = Vector2.Max(hitboxSize, new Vector2(42, 44));
            if (NPC.Size != hitboxSize)
                NPC.Size = hitboxSize;
            bool shouldNotUseShield = bulletHellCounter2 % BulletHellDuration != 0 || attackCastDelay > 0 ||
                (cirrus ? NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) : (NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()) || NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()))) ||
                NPC.ai[0] == 1f || NPC.ai[0] == 2f;

            // Make the shield and forcefield fade away in SCal's acceptance phase.
            if (lifeRatio <= 0.01f)
            {
                shieldOpacity = MathHelper.Lerp(shieldOpacity, 0f, 0.08f);
                forcefieldScale = MathHelper.Lerp(forcefieldScale, 0f, 0.08f);
            }

            // Summon a shield if the next attack will be a charge.
            // Make it go away if certain triggers happen during this, such as a bullet hell starting, however.
            else if (((willCharge && AttackCloseToBeingOver) || NPC.ai[1] == 2f) && !shouldNotUseShield)
            {
                if (NPC.ai[1] != 2f)
                {
                    float idealRotation = NPC.AngleTo(player.Center);
                    float angularOffset = Math.Abs(MathHelper.WrapAngle(shieldRotation - idealRotation));

                    if (angularOffset > 0.04f)
                    {
                        shieldRotation = shieldRotation.AngleLerp(idealRotation, 0.125f);
                        shieldRotation = shieldRotation.AngleTowards(idealRotation, 0.18f);
                    }
                }
                else if (!cirrus)
                {
                    // Emit dust off the skull at the position of its eye socket.
                    for (float num6 = 1f; num6 < 16f; num6 += 1f)
                    {
                        Dust dust = Dust.NewDustPerfect(NPC.Center, 182);
                        dust.position = Vector2.Lerp(NPC.position, NPC.oldPosition, num6 / 16f) + NPC.Size * 0.5f;
                        dust.position += shieldRotation.ToRotationVector2() * 42f;
                        dust.position += (shieldRotation - MathHelper.PiOver2).ToRotationVector2() * (float)Math.Cos(NPC.velocity.ToRotation()) * -4f;
                        dust.noGravity = true;
                        dust.velocity = NPC.velocity;
                        dust.color = Color.Red;
                        dust.scale = MathHelper.Lerp(0.6f, 0.85f, 1f - num6 / 16f);
                    }
                }

                // Shrink the force-field since it looks strange when charging.
                forcefieldScale = MathHelper.Lerp(forcefieldScale, 0.45f, 0.08f);
                shieldOpacity = MathHelper.Lerp(shieldOpacity, 1f, 0.08f);
            }
            // Make the shield disappear if it is no longer relevant and regenerate the forcefield.
            else
            {
                shieldOpacity = MathHelper.Lerp(shieldOpacity, 0f, 0.08f);
                forcefieldScale = MathHelper.Lerp(forcefieldScale, 1f, 0.08f);
            }

            #endregion
            #region ArenaCreation

            // Create the arena on the first frame. This does not run client-side.
            // If this is done on the server, a sync must be performed so that the arena box is
            // known to the clients. Not doing this results in significant desyncs in regards to things like DR.
            if (!spawnArena)
            {
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (death)
                    {
                        safeBox.X = spawnX = spawnXReset = (int)(NPC.Center.X - 1000f);
                        spawnX2 = spawnXReset2 = (int)(NPC.Center.X + 1000f);
                        safeBox.Y = spawnY = spawnYReset = (int)(NPC.Center.Y - 1000f);
                        safeBox.Width = 2000;
                        safeBox.Height = 2000;
                        spawnYAdd = 100;
                    }
                    else
                    {
                        safeBox.X = spawnX = spawnXReset = (int)(NPC.Center.X - 1250f);
                        spawnX2 = spawnXReset2 = (int)(NPC.Center.X + 1250f);
                        safeBox.Y = spawnY = spawnYReset = (int)(NPC.Center.Y - 1250f);
                        safeBox.Width = 2500;
                        safeBox.Height = 2500;
                        spawnYAdd = 125;
                    }

                    int safeBoxTilesX = (int)(safeBox.X + (float)(safeBox.Width / 2)) / 16;
                    int safeBoxTilesY = (int)(safeBox.Y + (float)(safeBox.Height / 2)) / 16;
                    int safeBoxTileWidth = safeBox.Width / 2 / 16 + 1;
                    for (int i = safeBoxTilesX - safeBoxTileWidth; i <= safeBoxTilesX + safeBoxTileWidth; i++)
                    {
                        for (int j = safeBoxTilesY - safeBoxTileWidth; j <= safeBoxTilesY + safeBoxTileWidth; j++)
                        {
                            if (!WorldGen.InWorld(i, j, 2))
                                continue;

                            int xoffset = 0;
                            int yoffset = 0;
                            int maxoffset = 3;
                            if (zenithAI)
                            {
                                xoffset += Main.rand.Next(-maxoffset, maxoffset + 1);
                                yoffset += Main.rand.Next(-maxoffset, maxoffset + 1);
                            }

                            if ((i == safeBoxTilesX - safeBoxTileWidth || i == safeBoxTilesX + safeBoxTileWidth || j == safeBoxTilesY - safeBoxTileWidth || j == safeBoxTilesY + safeBoxTileWidth) && !Main.tile[i + xoffset, j + yoffset].HasTile)
                            {
                                Main.tile[i + xoffset, j + yoffset].TileType = (ushort)ModContent.TileType<Tiles.ArenaTile>();
                                Main.tile[i + xoffset, j + yoffset].Get<TileWallWireStateData>().HasTile = true;
                            }
                            if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.SendTileSquare(-1, i + xoffset, j + yoffset, 1, TileChangeType.None);
                            }
                            else
                            {
                                WorldGen.SquareTileFrame(i + xoffset, j + yoffset, true);
                            }
                        }
                    }

                    if (initialRitualPosition == Vector2.Zero)
                    {
                        initialRitualPosition = NPC.Center + Vector2.UnitY * 24f;
                        NPC.netUpdate = true;
                    }

                    // Sync to update all clients on the state of the arena.
                    // Only after this will enrages be registered.
                    spawnArena = true;
                    NPC.netUpdate = true;
                }
            }
            #endregion
            #region Enrage and DR
            if ((spawnArena && !player.Hitbox.Intersects(safeBox)) || bossRush)
            {
                float projectileVelocityMultCap = (!player.Hitbox.Intersects(safeBox) && spawnArena) ? 2f : 1.5f;
                uDieLul = MathHelper.Clamp(uDieLul * 1.01f, 1f, projectileVelocityMultCap);
                protectionBoost = !bossRush;
                if (!player.Hitbox.Intersects(safeBox))
                    protectionBoost = true;
            }
            else
            {
                uDieLul = MathHelper.Clamp(uDieLul * 0.99f, 1f, 2f);
                protectionBoost = false;
            }
            NPC.Calamity().CurrentlyEnraged = !player.Hitbox.Intersects(safeBox);

            // Cirrus fucks mounts if you exit her arena.
            if (cirrus)
            {
                if (!player.Hitbox.Intersects(safeBox) && player.mount.Active)
                {
                    player.ResetEffects();
                    player.head = -1;
                    player.body = -1;
                    player.legs = -1;
                    player.handon = -1;
                    player.handoff = -1;
                    player.back = -1;
                    player.front = -1;
                    player.shoe = -1;
                    player.waist = -1;
                    player.shield = -1;
                    player.neck = -1;
                    player.face = -1;
                    player.balloon = -1;
                    player.mount.Dismount(player);
                }
            }

            // Set DR to be 99% and unbreakable if enraged. Boost DR during the 5th attack.
            CalamityGlobalNPC global = NPC.Calamity();
            if (protectionBoost && !gettingTired5)
            {
                global.DR = enragedDR;
                global.unbreakableDR = true;
            }
            else
            {
                global.DR = normalDR;
                global.unbreakableDR = false;
                if (startFifthAttack)
                    global.DR *= 1.2f;
            }
            #endregion
            #region Despawn
            if (!player.active || player.dead)
            {
                NPC.TargetClosest(false);
                player = Main.player[NPC.target];

                // Slow down and disappear in a burst of fire if should despawn.
                if (!player.active || player.dead)
                {
                    canDespawn = true;

                    NPC.Opacity = MathHelper.Lerp(NPC.Opacity, 0f, 0.065f);
                    NPC.velocity = Vector2.Lerp(Vector2.UnitY * -4f, Vector2.Zero, (float)Math.Sin(MathHelper.Pi * NPC.Opacity));
                    forcefieldOpacity = Utils.GetLerpValue(0.1f, 0.6f, NPC.Opacity, true);
                    if (NPC.alpha >= 230)
                    {
                        if (DownedBossSystem.downedCalamitas && !bossRush)
                        {
                            // Create a teleport line effect
                            Dust.QuickDustLine(NPC.Center, initialRitualPosition, 500f, cirrus ? Color.Pink : Color.Red);
                            NPC.Center = initialRitualPosition;

                            // Make the town NPC spawn.
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 12, cirrus ? ModContent.NPCType<FAP>() : ModContent.NPCType<WITCH>());
                        }

                        NPC.active = false;
                        NPC.netUpdate = true;
                    }

                    for (int i = 0; i < MathHelper.Lerp(2f, 6f, 1f - NPC.Opacity); i++)
                    {
                        Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                        brimstoneFire.color = cirrus ? Color.Pink : Color.Red;
                        brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 3.25f);
                        brimstoneFire.scale = Main.rand.NextFloat(0.95f, 1.15f);
                        brimstoneFire.noGravity = true;
                    }
                }
            }
            else
                canDespawn = false;
            #endregion
            #region Cast Charge Countdown
            if (attackCastDelay > 0)
            {
                attackCastDelay--;
                NPC.velocity *= 0.94f;
                NPC.damage = 0;
                NPC.dontTakeDamage = true;

                // Make a magic effect over time.
                for (int i = 0; i < (attackCastDelay == 0 ? 16 : 1); i++)
                {
                    Vector2 dustSpawnPosition = NPC.Bottom;
                    float horizontalSpawnOffset = bodyWidth * Main.rand.NextFloat(0.42f, 0.5f);
                    if (Main.rand.NextBool())
                        horizontalSpawnOffset *= -1f;
                    dustSpawnPosition.X += horizontalSpawnOffset;

                    Dust magic = Dust.NewDustPerfect(dustSpawnPosition, 267);
                    magic.color = Color.Lerp(cirrus ? Color.Pink : Color.Red, cirrus ? Color.Violet : Color.Orange, Main.rand.NextFloat(0.8f));
                    magic.noGravity = true;
                    magic.velocity = Vector2.UnitY * -Main.rand.NextFloat(5f, 9f);
                    magic.scale = 1f + NPC.velocity.Y * 0.35f;
                }

                if ((startBattle && !hasSummonedSepulcher1) || (gettingTired && !hasSummonedSepulcher2))
                    DoHeartsSpawningCastAnimation(player, death);

                if (enteredBrothersPhase && !hasSummonedBrothers)
                    DoBrothersSpawningCastAnimation(bodyWidth, bodyHeight);

                if (attackCastDelay == 0)
                {
                    NPC.dontTakeDamage = false;
                    NPC.netUpdate = true;
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }
            #endregion
            #region FirstAttack
            if (bulletHellCounter2 < BulletHellDuration)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;

                if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % (baseBulletHellProjectileGateValue * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        if (bulletHellCounter2 < 300 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else if (bulletHellCounter2 < 600) // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else // Blasts from above, left, and right
                        {
                            if (!Main.zenithWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 3f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                    }
                }
                FrameType = FrameAnimationType.Casting;
                return;
            }
            else if (!startBattle)
            {
                attackCastDelay = sepulcherSpawnCastTime;
                for (int i = 0; i < 40; i++)
                {
                    Dust castFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-70f, 70f), cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                    castFire.velocity = Vector2.UnitY.RotatedByRandom(0.08f) * -Main.rand.NextFloat(3f, 4.45f);
                    castFire.scale = Main.rand.NextFloat(1.35f, 1.6f);
                    castFire.fadeIn = 1.25f;
                    castFire.noGravity = true;
                }

                NPC.Center = safeBox.TopRight() + new Vector2(-120f, 620f);

                for (int i = 0; i < 40; i++)
                {
                    Dust castFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-70f, 70f), cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                    castFire.velocity = Vector2.UnitY.RotatedByRandom(0.08f) * -Main.rand.NextFloat(3f, 4.45f);
                    castFire.scale = Main.rand.NextFloat(1.35f, 1.6f);
                    castFire.fadeIn = 1.25f;
                    castFire.noGravity = true;
                }

                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, player.Center);
                startBattle = true;
            }
            #endregion
            #region SecondAttack
            if (bulletHellCounter2 < SecondBulletHellEndValue && startSecondAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;

                if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Cirrus throws alcohol bottles that explode into Fabstaff Rays
                    if (cirrus)
                    {
                        if (bulletHellCounter2 % 90 == 0)
                        {
                            float bottleSpeed = 12f;
                            Vector2 bottleVelocity = (player.Center + player.velocity * 20f - NPC.Center).SafeNormalize(Vector2.UnitY) * bottleSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, bottleVelocity * uDieLul, ModContent.ProjectileType<CirrusVolatileVodkaBottle>(), 350, 0f, Main.myPlayer);
                        }
                    }

                    if (bulletHellCounter2 < 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from top
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, gigablastDamage, 0f, Main.myPlayer);
                    }
                    else if (bulletHellCounter2 < 1500 && bulletHellCounter2 > 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from right
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -5f * uDieLul, 0f, fireblast, gigablastDamage, 0f, Main.myPlayer);
                    }
                    else if (bulletHellCounter2 > 1500)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from top
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, gigablastDamage, 0f, Main.myPlayer);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 1)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 1) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }

                        if (bulletHellCounter2 < 1200 && !Main.zenithWorld) // Blasts from below
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y + 1000f, 0f, -4f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else if (bulletHellCounter2 < 1500) // Blasts from left
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startSecondAttack && lifeRatio <= 0.75f)
            {
                // Bouncy Boulders
                if (cirrus)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            if (!WorldGen.SolidTile((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)))
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.BouncyBoulder, 300, 10f);
                        }
                    }
                }

                if (!bossRush)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBH2Text";
                    if (cirrus)
                        key = "Mods.CalamityMod.Status.Boss.CirrusBH2Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                startSecondAttack = true;
                return;
            }
            #endregion
            #region ThirdAttack
            if (bulletHellCounter2 < ThirdBulletHellEndValue && startThirdAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;

                if (cirrus)
                {
                    Vector2 destination = player.Center;
                    Vector2 distanceFromDestination = destination - NPC.Center;
                    Vector2 desiredVelocity = (distanceFromDestination - NPC.velocity).SafeNormalize(Vector2.UnitY) * CirrusPhotonRipperDashVelocity;

                    if (Vector2.Distance(NPC.Center, destination) > CirrusPhotonRipperMinDistanceFromTarget)
                        NPC.SimpleFlyMovement(desiredVelocity * uDieLul, CirrusPhotonRipperDashAcceleration * uDieLul);
                    else
                        NPC.velocity *= 0.9f;
                }
                else if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Cirrus uses Photon Ripper
                    if (bulletHellCounter2 == SecondBulletHellEndValue + 1 && cirrus)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One, ModContent.ProjectileType<CirrusPhotonRipperProjectile>(), CirrusPhotonRipperDamage, 0f, Main.myPlayer, 0f, 0f, NPC.whoAmI);

                    if (bulletHellCounter2 % 180 == 0) // Blasts from top
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, gigablastDamage, 0f, Main.myPlayer);

                    if (bulletHellCounter2 % 240 == 0) // Fireblasts from above
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, fireblastDamage, 0f, Main.myPlayer);

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 4)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 4) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }

                        if (bulletHellCounter2 < 2100 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else if (bulletHellCounter2 < 2400) // Blasts from right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startThirdAttack && lifeRatio <= 0.5f)
            {
                // Bouncy Boulders
                if (cirrus)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            if (!WorldGen.SolidTile((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)))
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.BouncyBoulder, 300, 10f);
                        }
                    }
                }

                if (!bossRush)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBH3Text";
                    if (cirrus)
                        key = "Mods.CalamityMod.Status.Boss.CirrusBH3Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                startThirdAttack = true;
                return;
            }
            #endregion
            #region FourthAttack
            if (bulletHellCounter2 < FourthBulletHellEndValue && startFourthAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;

                if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient) // More clustered attack
                {
                    // Cirrus throws alcohol bottles that explode into Fabstaff Rays
                    if (cirrus)
                    {
                        if (bulletHellCounter2 % 90 == 0)
                        {
                            float bottleSpeed = 12f;
                            Vector2 bottleVelocity = (player.Center + player.velocity * 20f - NPC.Center).SafeNormalize(Vector2.UnitY) * bottleSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, bottleVelocity * uDieLul, ModContent.ProjectileType<CirrusVolatileVodkaBottle>(), 125, 0f, Main.myPlayer);
                        }
                    }

                    if (bulletHellCounter2 % 180 == 0) // Blasts from top
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, gigablastDamage, 0f, Main.myPlayer);

                    if (bulletHellCounter2 % 240 == 0) // Fireblasts from above
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, fireblastDamage, 0f, Main.myPlayer);

                    int divisor = revenge ? 225 : expertMode ? 450 : 675;

                    // TODO -- Resprite Brimstone Monsters to be something else.
                    if (bulletHellCounter2 % divisor == 0 && expertMode) // Giant homing fireballs
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 1f * uDieLul, ModContent.ProjectileType<BrimstoneMonster>(), monsterDamage, 0f, Main.myPlayer, 0f, passedVar);
                        passedVar += 1f;
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 6)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 6) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }

                        if (bulletHellCounter2 < 3000 && !Main.zenithWorld) // Blasts from below
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y + 1000f, 0f, -4f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else if (bulletHellCounter2 < 3300) // Blasts from left
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startFourthAttack && lifeRatio <= 0.3f)
            {
                // Bouncy Boulders
                if (cirrus)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 15; i++)
                        {
                            if (!WorldGen.SolidTile((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)))
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.BouncyBoulder, 300, 10f);
                        }
                    }
                }

                if (!bossRush)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBH4Text";
                    if (cirrus)
                        key = "Mods.CalamityMod.Status.Boss.CirrusBH4Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                startFourthAttack = true;
                return;
            }
            #endregion
            #region FifthAttack
            if (bulletHellCounter2 < FifthBulletHellEndValue && startFifthAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;

                if (cirrus)
                {
                    Vector2 destination = player.Center;
                    Vector2 distanceFromDestination = destination - NPC.Center;
                    Vector2 desiredVelocity = (distanceFromDestination - NPC.velocity).SafeNormalize(Vector2.UnitY) * CirrusPhotonRipperDashVelocity;

                    if (Vector2.Distance(NPC.Center, destination) > CirrusPhotonRipperMinDistanceFromTarget)
                        NPC.SimpleFlyMovement(desiredVelocity * uDieLul, CirrusPhotonRipperDashAcceleration * uDieLul);
                    else
                        NPC.velocity *= 0.9f;
                }
                else if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Cirrus throws alcohol bottles that explode into Fabstaff Rays
                    if (cirrus)
                    {
                        // Cirrus uses Photon Ripper
                        if (bulletHellCounter2 == FourthBulletHellEndValue + 1)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One, ModContent.ProjectileType<CirrusPhotonRipperProjectile>(), CirrusPhotonRipperDamage, 0f, Main.myPlayer, 0f, 0f, NPC.whoAmI);

                        if (bulletHellCounter2 % 90 == 0)
                        {
                            float bottleSpeed = 12f;
                            Vector2 bottleVelocity = (player.Center + player.velocity * 20f - NPC.Center).SafeNormalize(Vector2.UnitY) * bottleSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, bottleVelocity * uDieLul, ModContent.ProjectileType<CirrusVolatileVodkaBottle>(), 125, 0f, Main.myPlayer);
                        }
                    }

                    if (bulletHellCounter2 % 240 == 0) // Blasts from top
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, gigablastDamage, 0f, Main.myPlayer);

                    if (bulletHellCounter2 % 360 == 0) // Fireblasts from above
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, fireblastDamage, 0f, Main.myPlayer);

                    if (bulletHellCounter2 % 30 == 0) // Projectiles that move in wave pattern
                    {
                        int random = Main.rand.Next(-500, 501);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + random, -5f * uDieLul, 0f, wave, waveDamage, 0f, Main.myPlayer);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y - random, 5f * uDieLul, 0f, wave, waveDamage, 0f, Main.myPlayer);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 8)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 8) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }

                        if (bulletHellCounter2 < 3900 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else if (bulletHellCounter2 < 4200) // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                        else // Blasts from above, left, and right
                        {
                            if (!Main.zenithWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 3f * uDieLul, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, bulletHellblastDamage, 0f, Main.myPlayer);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startFifthAttack && lifeRatio <= 0.1f)
            {
                // Bouncy Boulders
                if (cirrus)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            if (!WorldGen.SolidTile((int)(NPC.Center.X / 16f), (int)(NPC.Center.Y / 16f)))
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center.X, NPC.Center.Y, (float)Main.rand.Next(-1599, 1600) * 0.01f, (float)Main.rand.Next(-1599, 1) * 0.01f, ProjectileID.BouncyBoulder, 300, 10f);
                        }
                    }
                }

                string key = "Mods.CalamityMod.Status.Boss.SCalBH5Text";
                if (cirrus)
                    key = "Mods.CalamityMod.Status.Boss.CirrusBH5Text";

                if (!bossRush)
                {
                    if (DownedBossSystem.downedCalamitas && !cirrus)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                startFifthAttack = true;
                return;
            }
            #endregion
            #region EndSections
            if (startFifthAttack)
            {
                if (gettingTired5)
                {
                    if (cirrus)
                    {
                        if (giveUpCounter > 1)
                        {
                            // Spin around the target and fire a bunch of beams (Sans) while also firing other projectiles.
                            int blasterTimer = GiveUpCounterMax - giveUpCounter;
                            Vector2 circleOffset = player.Center + (Vector2.UnitY * 640f).RotatedBy(MathHelper.ToRadians(blasterTimer * 3f));
                            NPC.Center = circleOffset;

                            int blasterDivisor = 5;
                            if (blasterTimer % blasterDivisor == 0)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), circleOffset, player.Center, ModContent.ProjectileType<CirrusBlaster>(), 350, 0f, Main.myPlayer, 0f, 1f);
                            }

                            int beamDivisor = 60;
                            if (blasterTimer % beamDivisor == 0)
                            {
                                int totalProjectiles = 5;
                                float radians = MathHelper.TwoPi / totalProjectiles;
                                float velocity = 12f * uDieLul;
                                Vector2 spinningPoint = new Vector2(0f, -velocity);
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    for (int k = 0; k < totalProjectiles; k++)
                                    {
                                        Vector2 rayVelocity = spinningPoint.RotatedBy(radians * k);
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (rayVelocity).SafeNormalize(Vector2.UnitY) * 16f, rayVelocity, ModContent.ProjectileType<Projectiles.Magic.FabRay>(), 250, 0f, Main.myPlayer);
                                        if (proj.WithinBounds(Main.maxProjectiles))
                                        {
                                            Main.projectile[proj].DamageType = DamageClass.Default;
                                            Main.projectile[proj].friendly = false;
                                            Main.projectile[proj].hostile = true;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            NPC.noTileCollide = false;
                            NPC.noGravity = false;
                            NPC.damage = 0;

                            if (giveUpCounter == 1)
                            {
                                NPC.velocity = Vector2.Zero;
                                CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.CirrusGiveUpText", cirrusTextColor);
                                Dust.QuickDustLine(NPC.Center, initialRitualPosition, 500f, Color.Pink);
                                NPC.Center = initialRitualPosition;
                                giveUpCounter--;
                            }
                            else
                            {
                                for (int i = 0; i < 24; i++)
                                {
                                    Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                                    brimstoneFire.color = Color.Pink;
                                    brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 3.25f);
                                    brimstoneFire.scale = Main.rand.NextFloat(0.95f, 1.15f);
                                    brimstoneFire.fadeIn = 1.25f;
                                    brimstoneFire.noGravity = true;
                                }

                                NPC.active = false;
                                NPC.netUpdate = true;
                                NPC.NPCLoot();
                            }

                            return;
                        }
                    }
                    else
                    {
                        if (NPC.velocity.Y < 9f)
                            NPC.velocity.Y += 0.185f;

                        NPC.noTileCollide = false;
                        NPC.noGravity = false;
                        NPC.damage = 0;

                        // Teleport back to the arena on defeat
                        if (giveUpCounter == GiveUpCounterMax)
                        {
                            Dust.QuickDustLine(NPC.Center, initialRitualPosition, 500f, Color.Red);
                            NPC.Center = initialRitualPosition;
                        }

                        if (!canDespawn)
                            NPC.velocity.X *= 0.96f;

                        if (DownedBossSystem.downedCalamitas && !bossRush)
                        {
                            if (giveUpCounter == 720)
                            {
                                for (int i = 0; i < 24; i++)
                                {
                                    Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                                    brimstoneFire.color = Color.Red;
                                    brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 3.25f);
                                    brimstoneFire.scale = Main.rand.NextFloat(0.95f, 1.15f);
                                    brimstoneFire.fadeIn = 1.25f;
                                    brimstoneFire.noGravity = true;
                                }

                                NPC.active = false;
                                NPC.netUpdate = true;
                                NPC.NPCLoot();
                            }
                        }
                        else if (giveUpCounter == 900 && !bossRush)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.SCalAcceptanceText1", textColor);
                        }
                        else if (giveUpCounter == 600 && !bossRush)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.SCalAcceptanceText2", textColor);
                        }
                        else if (giveUpCounter == 300 && !bossRush)
                        {
                            CalamityUtils.DisplayLocalizedText("Mods.CalamityMod.Status.Boss.SCalAcceptanceText3", textColor);
                        }

                        if (giveUpCounter <= 0)
                        {
                            if (bossRush)
                            {
                                NPC.chaseable = true;
                                NPC.dontTakeDamage = false;
                                return;
                            }

                            for (int i = 0; i < 24; i++)
                            {
                                Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                                brimstoneFire.color = Color.Red;
                                brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 3.25f);
                                brimstoneFire.scale = Main.rand.NextFloat(0.95f, 1.15f);
                                brimstoneFire.fadeIn = 1.25f;
                                brimstoneFire.noGravity = true;
                            }

                            NPC.active = false;
                            NPC.netUpdate = true;
                            NPC.NPCLoot();
                            return;
                        }
                    }

                    giveUpCounter--;
                    NPC.chaseable = false;
                    NPC.dontTakeDamage = true;
                    return;
                }

                if (!gettingTired5 && lifeRatio <= 0.01f)
                {
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.active && projectile.type == ModContent.ProjectileType<BrimstoneMonster>())
                        {
                            if (projectile.timeLeft > 90)
                                projectile.timeLeft = 90;
                        }
                    }

                    if (!bossRush)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText4";
                        if (cirrus)
                            key = "Mods.CalamityMod.Status.Boss.CirrusBruhText";
                        else if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                    }

                    gettingTired5 = true;
                    return;
                }
                else if (!gettingTired4 && lifeRatio <= 0.02f)
                {
                    if (!bossRush && !cirrus)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText3";
                        if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.DisplayLocalizedText(key, textColor);
                    }

                    gettingTired4 = true;
                    return;
                }
                else if (!gettingTired3 && lifeRatio <= 0.04f)
                {
                    if (!bossRush && !cirrus)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText2";
                        if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.DisplayLocalizedText(key, textColor);
                    }

                    gettingTired3 = true;
                    return;
                }
                else if (!gettingTired2 && lifeRatio <= 0.06f)
                {
                    if (!bossRush)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText1";
                        if (cirrus)
                            key = "Mods.CalamityMod.Status.Boss.CirrusNonchalantText";
                        else if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                    }

                    gettingTired2 = true;
                    return;
                }
                else if (!gettingTired && lifeRatio <= 0.08f)
                {
                    attackCastDelay = sepulcherSpawnCastTime;
                    for (int i = 0; i < 40; i++)
                    {
                        Dust castFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-70f, 70f), cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                        castFire.velocity = Vector2.UnitY.RotatedByRandom(0.08f) * -Main.rand.NextFloat(3f, 4.45f);
                        castFire.scale = Main.rand.NextFloat(1.35f, 1.6f);
                        castFire.fadeIn = 1.25f;
                        castFire.noGravity = true;
                    }

                    NPC.Center = safeBox.TopRight() + new Vector2(-120f, 620f);

                    for (int i = 0; i < 40; i++)
                    {
                        Dust castFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-70f, 70f), cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                        castFire.velocity = Vector2.UnitY.RotatedByRandom(0.08f) * -Main.rand.NextFloat(3f, 4.45f);
                        castFire.scale = Main.rand.NextFloat(1.35f, 1.6f);
                        castFire.fadeIn = 1.25f;
                        castFire.noGravity = true;
                    }

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, player.Center);
                    gettingTired = true;
                    return;
                }
            }
            #endregion
            #region DespawnProjectiles
            if (bulletHellCounter2 % BulletHellDuration == 0 && despawnProj)
            {
                for (int x = 0; x < Main.maxProjectiles; x++)
                {
                    Projectile projectile = Main.projectile[x];
                    if (projectile.active)
                    {
                        if (projectile.type == bulletHellblast ||
                            projectile.type == barrage ||
                            projectile.type == wave)
                        {
                            if (projectile.timeLeft > 60)
                                projectile.timeLeft = 60;
                        }
                        else if (projectile.type == fireblast || projectile.type == gigablast)
                        {
                            projectile.ai[1] = 1f;

                            if (projectile.timeLeft > 60)
                                projectile.timeLeft = 60;
                        }
                    }
                }
                despawnProj = false;
            }
            #endregion
            #region TransformSeekerandBrotherTriggers
            if (!halfLife && lifeRatio <= 0.4f)
            {
                if (!bossRush)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalPhase2Text";
                    if (cirrus)
                        key = "Mods.CalamityMod.Status.Boss.CirrusPhase2Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                halfLife = true;
            }

            if (lifeRatio <= 0.2f)
            {
                if (!secondStage)
                {
                    if (!bossRush)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalSeekerRingText";
                        if (cirrus)
                            key = "Mods.CalamityMod.Status.Boss.CirrusHallowBossSpamText";
                        else if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (cirrus)
                        {
                            // Spawn 1 Providence, 2 Queen Slimes and 2 Empress of Lights
                            int maximumBullshit = 5;
                            int angleFromCirrus = 360 / maximumBullshit;
                            int distanceFromCirrus = 300;
                            for (int i = 0; i < maximumBullshit; i++)
                            {
                                switch (i)
                                {
                                    case 0:
                                        int npc = NPC.NewNPC(NPC.GetSource_FromAI(),
                                            (int)(vectorCenter.X + (Math.Sin(i * angleFromCirrus) * distanceFromCirrus)),
                                            (int)(vectorCenter.Y + (Math.Cos(i * angleFromCirrus) * distanceFromCirrus)),
                                            ModContent.NPCType<Providence.Providence>(), NPC.whoAmI);
                                        Main.npc[npc].timeLeft *= 20;
                                        CalamityUtils.BossAwakenMessage(npc);
                                        break;

                                    case 1:
                                    case 2:
                                        int npc2 = NPC.NewNPC(NPC.GetSource_FromAI(),
                                            (int)(vectorCenter.X + (Math.Sin(i * angleFromCirrus) * distanceFromCirrus)),
                                            (int)(vectorCenter.Y + (Math.Cos(i * angleFromCirrus) * distanceFromCirrus)),
                                            NPCID.HallowBoss, NPC.whoAmI);
                                        Main.npc[npc2].timeLeft *= 20;
                                        break;

                                    case 3:
                                    case 4:
                                        int npc3 = NPC.NewNPC(NPC.GetSource_FromAI(),
                                            (int)(vectorCenter.X + (Math.Sin(i * angleFromCirrus) * distanceFromCirrus)),
                                            (int)(vectorCenter.Y + (Math.Cos(i * angleFromCirrus) * distanceFromCirrus)),
                                            NPCID.QueenSlimeBoss, NPC.whoAmI);
                                        Main.npc[npc3].timeLeft *= 20;
                                        break;

                                    default:
                                        break;
                                }
                            }
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                            for (int i = 0; i < 20; i++)
                            {
                                int FireEye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(vectorCenter.X + (Math.Sin(i * 18) * 300)), (int)(vectorCenter.Y + (Math.Cos(i * 18) * 300)), ModContent.NPCType<SoulSeekerSupreme>(), NPC.whoAmI, 0, 0, 0, -1);
                                NPC Eye = Main.npc[FireEye];
                                Eye.ai[0] = i * 18;
                                Eye.ai[3] = i * 18;
                            }
                        }
                    }

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, player.Center);
                    secondStage = true;
                }
            }

            if (bossLife == 0f && NPC.life > 0)
            {
                bossLife = NPC.lifeMax;
            }
            if (NPC.life > 0)
            {
                if (lifeRatio < 0.45f && !enteredBrothersPhase)
                {
                    enteredBrothersPhase = true;
                    attackCastDelay = brothersSpawnCastTime;
                    NPC.netUpdate = true;
                }
            }

            #endregion
            #region FirstStage
            if (NPC.ai[0] == 0f)
            {
                NPC.damage = NPC.defDamage;
                if (wormAlive)
                {
                    NPC.dontTakeDamage = true;
                    NPC.chaseable = false;
                }
                else
                {
                    if (cataclysmAlive || catastropheAlive)
                    {
                        NPC.dontTakeDamage = true;
                        NPC.chaseable = false;
                        NPC.damage = 0;

                        if (!canDespawn)
                            NPC.velocity *= 0.95f;

                        return;
                    }
                    else
                    {
                        NPC.dontTakeDamage = false;
                        NPC.chaseable = true;
                    }
                }

                if (NPC.ai[1] == -1f)
                {
                    phaseChange++;
                    if (phaseChange > 23)
                        phaseChange = 0;

                    int phase = 0; //0 = shots above 1 = charge 2 = nothing 3 = hellblasts 4 = fireblasts
                    switch (phaseChange)
                    {
                        case 0:
                            phase = 0;
                            willCharge = false;
                            break; //0341
                        case 1:
                            phase = 3;
                            break;
                        case 2:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 3:
                            phase = 1;
                            break;
                        case 4:
                            phase = 1;
                            break; //1430
                        case 5:
                            phase = 4;
                            willCharge = false;
                            break;
                        case 6:
                            phase = 3;
                            break;
                        case 7:
                            phase = 0;
                            willCharge = true;
                            break;
                        case 8:
                            phase = 1;
                            break; //1034
                        case 9:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 10:
                            phase = 3;
                            break;
                        case 11:
                            phase = 4;
                            break;
                        case 12:
                            phase = 4;
                            break; //4310
                        case 13:
                            phase = 3;
                            willCharge = true;
                            break;
                        case 14:
                            phase = 1;
                            break;
                        case 15:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 16:
                            phase = 4;
                            break; //4411
                        case 17:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 18:
                            phase = 1;
                            break;
                        case 19:
                            phase = 1;
                            break;
                        case 20:
                            phase = 0;
                            break; //0101
                        case 21:
                            phase = 1;
                            break;
                        case 22:
                            phase = 0;
                            break;
                        case 23:
                            phase = 1;
                            break;
                    }

                    NPC.ai[1] = phase;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                }
                else
                {
                    if (NPC.ai[1] == 0f)
                    {
                        // How fast SCal moves to the destination
                        float velocity = 12f;
                        float acceleration = 0.12f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (player.HoldingTrueMeleeWeapon())
                            acceleration *= 0.5f;

                        // This is where SCal should be
                        Vector2 destination = new Vector2(player.Center.X, player.Center.Y - 550f);

                        // How far SCal is from where she's supposed to be
                        Vector2 distanceFromDestination = destination - NPC.Center;

                        // Movement
                        if (!canDespawn)
                        {
                            // Set the velocity
                            CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                        }

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 300f)
                        {
                            NPC.ai[1] = -1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }

                        Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        Vector2 projectileSpawn = NPC.Center + projectileVelocity * 8f;
                        projectileVelocity *= 10f * uDieLul;

                        NPC.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (NPC.localAI[1] > 90f)
                        {
                            NPC.localAI[1] = 0f;

                            int randomShot = Main.rand.Next(6);
                            if (randomShot == 0 && canFireSplitingFireball)
                            {
                                canFireSplitingFireball = false;
                                randomShot = gigablast;
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);

                                for (int i = 0; i < 15; i++)
                                {
                                    Dust magic = Dust.NewDustPerfect(projectileSpawn, 264);
                                    magic.velocity = projectileVelocity.RotatedByRandom(0.36f) * Main.rand.NextFloat(0.9f, 1.1f);
                                    magic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                                    magic.scale = 1.2f;
                                    magic.fadeIn = 0.6f;
                                    magic.noLight = true;
                                    magic.noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, fireblastDamage, 0f, Main.myPlayer);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (randomShot == 1 && canFireSplitingFireball)
                            {
                                canFireSplitingFireball = false;
                                randomShot = fireblast;
                                SoundEngine.PlaySound(BrimstoneShotSound, NPC.Center);

                                for (int i = 0; i < 20; i++)
                                {
                                    Dust magic = Dust.NewDustPerfect(projectileSpawn, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                                    magic.velocity = projectileVelocity.RotatedByRandom(0.36f) * Main.rand.NextFloat(0.9f, 1.1f) * 1.3f;
                                    magic.color = cirrus ? Color.Pink : Color.Red;
                                    magic.scale = 1.425f;
                                    magic.fadeIn = 0.75f;
                                    magic.noLight = true;
                                    magic.noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, gigablastDamage, 0f, Main.myPlayer);
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                canFireSplitingFireball = true;
                                randomShot = barrage;
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                float rotation = MathHelper.ToRadians(20);
                                int numProj = 8;
                                for (int j = 0; j < numProj; j++)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        Vector2 magicDustVelocity = projectileVelocity;
                                        magicDustVelocity *= MathHelper.Lerp(0.3f, 1f, i / 7f);

                                        Dust magic = Dust.NewDustPerfect(projectileSpawn, 264);
                                        magic.velocity = magicDustVelocity;
                                        magic.color = cirrus ? Color.Pink : Color.Red;
                                        magic.scale = MathHelper.Lerp(0.85f, 1.5f, i / 7f);
                                        magic.fadeIn = 0.67f;
                                        magic.noLight = true;
                                        magic.noGravity = true;
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, j / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, perturbedSpeed, randomShot, barrageDamage, 0f, Main.myPlayer);
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                        }

                        FrameType = FrameAnimationType.FasterUpwardDraft;
                    }
                    else if (NPC.ai[1] == 1f)
                    {
                        float chargeVelocity = (wormAlive ? 26f : 30f) + (1f - lifeRatio) * 8f;

                        if (Main.getGoodWorld)
                            chargeVelocity *= 1.15f;

                        if (!canDespawn)
                        {
                            Vector2 vector = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                            NPC.velocity = vector * chargeVelocity;

                            shieldRotation = NPC.velocity.ToRotation();
                            NPC.netUpdate = true;

                            SoundEngine.PlaySound(DashSound, NPC.Center);
                            if (cirrus)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                                    float velocity = 8;
                                    int type = ModContent.ProjectileType<Projectiles.Magic.FabRay>();
                                    int damage = (int)(NPC.damage / 3);
                                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;
                                    float rotation = MathHelper.ToRadians(22);
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(2)));

                                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (perturbedSpeed).SafeNormalize(Vector2.UnitY) * 3f, perturbedSpeed, type, damage, 0f, Main.myPlayer);
                                        if (p.WithinBounds(Main.maxProjectiles))
                                        {
                                             Main.projectile[p].DamageType = DamageClass.Default;
                                             Main.projectile[p].friendly = false;
                                             Main.projectile[p].hostile = true;
                                        }
                                    }
                                }
                            }
                        }

                        NPC.ai[1] = 2f;
                    }
                    else if (NPC.ai[1] == 2f)
                    {
                        NPC.ai[2] += 1f;

                        if (Math.Abs(NPC.velocity.X) > 0.15f)
                            NPC.spriteDirection = (NPC.velocity.X < 0f).ToDirectionInt();

                        if (NPC.ai[2] >= 25f)
                        {
                            if (!canDespawn)
                            {
                                NPC.velocity *= 0.96f;

                                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                                    NPC.velocity.X = 0f;
                                if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                                    NPC.velocity.Y = 0f;
                            }
                        }

                        bool willChargeAgain = NPC.ai[3] + 1 < 2;

                        if (NPC.ai[2] >= 70f)
                        {
                            NPC.ai[3] += 1f;
                            NPC.ai[2] = 0f;
                            NPC.TargetClosest();

                            if (!willChargeAgain)
                                NPC.ai[1] = -1f;
                            else
                                NPC.ai[1] = 1f;
                        }

                        if (willChargeAgain && NPC.ai[2] > 50f)
                        {
                            float idealRotation = NPC.AngleTo(player.Center);
                            shieldRotation = shieldRotation.AngleLerp(idealRotation, 0.125f);
                            shieldRotation = shieldRotation.AngleTowards(idealRotation, 0.18f);
                        }

                        FrameType = FrameAnimationType.FasterUpwardDraft;
                    }
                    else if (NPC.ai[1] == 3f)
                    {
                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (player.HoldingTrueMeleeWeapon())
                            acceleration *= 0.5f;

                        int posX = 1;
                        if (NPC.position.X + (NPC.width / 2) < player.position.X + player.width)
                            posX = -1;

                        // This is where SCal should be
                        Vector2 destination = new Vector2(player.Center.X + posX * 600f, player.Center.Y);

                        // How far SCal is from where she's supposed to be
                        Vector2 distanceFromDestination = destination - NPC.Center;

                        // Movement
                        if (!canDespawn)
                        {
                            // Set the velocity
                            CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                        }

                        Vector2 handPosition = NPC.Center + new Vector2(NPC.spriteDirection * -18f, 2f);

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 480f)
                        {
                            NPC.ai[1] = -1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            if (!player.dead)
                                NPC.ai[3] += wormAlive ? 0.5f : 1f;

                            if (NPC.ai[3] >= 20f)
                            {
                                NPC.ai[3] = 0f;
                                SoundEngine.PlaySound(HellblastSound, NPC.Center);

                                // Release a burst of magic dust along with a brimstone hellblast skull.
                                for (int i = 0; i < 25; i++)
                                {
                                    Dust brimstoneMagic = Dust.NewDustPerfect(handPosition, 264);
                                    brimstoneMagic.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(0.31f) * Main.rand.NextFloat(3f, 5f) + NPC.velocity;
                                    brimstoneMagic.scale = Main.rand.NextFloat(1.25f, 1.35f);
                                    brimstoneMagic.noGravity = true;
                                    brimstoneMagic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                                    brimstoneMagic.fadeIn = 1.5f;
                                    brimstoneMagic.noLight = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    Vector2 projectileSpawn = NPC.Center + projectileVelocity * 4f;
                                    projectileVelocity *= 10f * uDieLul;
                                    int projectileType = hellblast;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, hellblastDamage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (Main.rand.NextBool())
                        {
                            Dust brimstoneMagic = Dust.NewDustPerfect(handPosition, 264);
                            brimstoneMagic.velocity = Vector2.UnitY.RotatedByRandom(0.14f) * Main.rand.NextFloat(-3.5f, -3f) + NPC.velocity;
                            brimstoneMagic.scale = Main.rand.NextFloat(1.25f, 1.35f);
                            brimstoneMagic.noGravity = true;
                            brimstoneMagic.noLight = true;
                        }

                        FrameType = FrameAnimationType.OutwardHandCast;
                    }
                    else if (NPC.ai[1] == 4f)
                    {
                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (player.HoldingTrueMeleeWeapon())
                            acceleration *= 0.5f;

                        int posX = 1;
                        if (NPC.position.X + (NPC.width / 2) < player.position.X + player.width)
                            posX = -1;

                        // This is where SCal should be
                        Vector2 destination = new Vector2(player.Center.X + posX * 750f, player.Center.Y);

                        // How far SCal is from where she's supposed to be
                        Vector2 distanceFromDestination = destination - NPC.Center;

                        // Movement
                        if (!canDespawn)
                        {
                            // Set the velocity
                            CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                        }

                        int shootRate = wormAlive ? 280 : 140;
                        NPC.localAI[1]++;
                        FrameChangeSpeed = 0.175f;
                        FrameType = FrameAnimationType.BlastCast;

                        if (NPC.localAI[1] > shootRate)
                        {
                            Vector2 handPosition = NPC.Center + new Vector2(NPC.spriteDirection * -22f, 2f);

                            // Release a burst of magic dust when punching.
                            for (int i = 0; i < 25; i++)
                            {
                                Dust brimstoneMagic = Dust.NewDustPerfect(handPosition, 264);
                                brimstoneMagic.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(0.24f) * Main.rand.NextFloat(5f, 7f) + NPC.velocity;
                                brimstoneMagic.scale = Main.rand.NextFloat(1.25f, 1.35f);
                                brimstoneMagic.noGravity = true;
                                brimstoneMagic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                                brimstoneMagic.fadeIn = 1.5f;
                                brimstoneMagic.noLight = true;
                            }
                            NPC.localAI[1] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                Vector2 projectileSpawn = NPC.Center + projectileVelocity * 8f;
                                projectileVelocity *= 5f * uDieLul;
                                int projectileType = gigablast;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, fireblastDamage, 0f, Main.myPlayer);
                            }
                        }

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 300f)
                        {
                            NPC.ai[1] = -1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }
                    }
                }

                if (lifeRatio < 0.4f)
                {
                    NPC.ai[0] = 1f;
                    NPC.ai[1] = 0f;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                    NPC.TargetClosest();
                    NPC.netUpdate = true;
                }
            }
            #endregion
            #region Transition

            else if (NPC.ai[0] == 1f || NPC.ai[0] == 2f)
            {
                NPC.dontTakeDamage = true;
                NPC.chaseable = false;

                if (NPC.ai[0] == 1f)
                {
                    NPC.ai[2] += 0.005f;
                    if (NPC.ai[2] > 0.5)
                        NPC.ai[2] = 0.5f;
                }
                else
                {
                    NPC.ai[2] -= 0.005f;
                    if (NPC.ai[2] < 0f)
                        NPC.ai[2] = 0f;
                }

                NPC.ai[1] += 1f;
                if (NPC.ai[1] == 100f)
                {
                    NPC.ai[0] += 1f;
                    NPC.ai[1] = 0f;

                    if (NPC.ai[0] == 3f)
                        NPC.ai[2] = 0f;
                    else
                    {
                        for (int i = 0; i < 50; i++)
                            Dust.NewDust(NPC.position, NPC.width, NPC.height, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone, Main.rand.Next(-30, 31) * 0.2f, Main.rand.Next(-30, 31) * 0.2f, 0, default, 1f);

                        SoundEngine.PlaySound(SpawnSound, NPC.Center);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                    brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2.75f, 4.25f);
                    brimstoneFire.noGravity = true;
                }

                if (!canDespawn)
                {
                    NPC.velocity *= 0.98f;

                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;
                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                        NPC.velocity.Y = 0f;
                }
            }
            #endregion
            #region LastStage
            else
            {
                NPC.damage = NPC.defDamage;
                if (wormAlive)
                {
                    NPC.dontTakeDamage = true;
                    NPC.chaseable = false;
                }
                else
                {
                    if (cirrus ? NPC.AnyNPCs(ModContent.NPCType<Providence.Providence>()) : NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
                    {
                        NPC.dontTakeDamage = true;
                        NPC.chaseable = false;
                    }
                    else
                    {
                        NPC.dontTakeDamage = false;
                        NPC.chaseable = true;
                    }
                }

                if (NPC.ai[1] == -1f)
                {
                    phaseChange++;
                    if (phaseChange > 23)
                        phaseChange = 0;

                    int phase = 0; //0 = shots above 1 = charge 2 = nothing 3 = hellblasts 4 = fireblasts
                    switch (phaseChange)
                    {
                        case 0:
                            phase = 0;
                            willCharge = false;
                            break; //0341
                        case 1:
                            phase = 3;
                            break;
                        case 2:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 3:
                            phase = 1;
                            break;
                        case 4:
                            phase = 1;
                            break; //1430
                        case 5:
                            phase = 4;
                            willCharge = false;
                            break;
                        case 6:
                            phase = 3;
                            break;
                        case 7:
                            phase = 0;
                            willCharge = true;
                            break;
                        case 8:
                            phase = 1;
                            break; //1034
                        case 9:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 10:
                            phase = 3;
                            break;
                        case 11:
                            phase = 4;
                            break;
                        case 12:
                            phase = 4;
                            break; //4310
                        case 13:
                            phase = 3;
                            willCharge = true;
                            break;
                        case 14:
                            phase = 1;
                            break;
                        case 15:
                            phase = 0;
                            willCharge = false;
                            break;
                        case 16:
                            phase = 4;
                            break; //4411
                        case 17:
                            phase = 4;
                            willCharge = true;
                            break;
                        case 18:
                            phase = 1;
                            break;
                        case 19:
                            phase = 1;
                            break;
                        case 20:
                            phase = 0;
                            break; //0101
                        case 21:
                            phase = 1;
                            break;
                        case 22:
                            phase = 0;
                            break;
                        case 23:
                            phase = 1;
                            break;
                    }

                    NPC.ai[1] = phase;
                    NPC.ai[2] = 0f;
                    NPC.ai[3] = 0f;
                }
                else
                {
                    if (NPC.ai[1] == 0f)
                    {
                        // How fast SCal moves to the destination
                        float velocity = 12f;
                        float acceleration = 0.12f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (player.HoldingTrueMeleeWeapon())
                            acceleration *= 0.5f;

                        // This is where SCal should be
                        Vector2 destination = new Vector2(player.Center.X, player.Center.Y - 550f);

                        // How far SCal is from where she's supposed to be
                        Vector2 distanceFromDestination = destination - NPC.Center;

                        // Movement
                        if (!canDespawn)
                        {
                            // Set the velocity
                            CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                        }

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 240f)
                        {
                            NPC.ai[1] = -1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }

                        Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                        Vector2 projectileSpawn = NPC.Center + projectileVelocity * 8f;
                        projectileVelocity *= 10f * uDieLul;

                        NPC.localAI[1] += wormAlive ? 0.5f : 1f;
                        if (NPC.localAI[1] > 60f)
                        {
                            NPC.localAI[1] = 0f;

                            int randomShot = Main.rand.Next(6);
                            if (randomShot == 0 && canFireSplitingFireball)
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                canFireSplitingFireball = false;
                                randomShot = gigablast;

                                for (int i = 0; i < 16; i++)
                                {
                                    Dust magic = Dust.NewDustPerfect(projectileSpawn, 264);
                                    magic.velocity = projectileVelocity.RotatedByRandom(0.36f) * Main.rand.NextFloat(0.9f, 1.1f) * 1.2f;
                                    magic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                                    magic.scale = 1.3f;
                                    magic.fadeIn = 0.6f;
                                    magic.noLight = true;
                                    magic.noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, fireblastDamage, 0f, Main.myPlayer);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (randomShot == 1 && canFireSplitingFireball)
                            {
                                SoundEngine.PlaySound(BrimstoneShotSound, NPC.Center);
                                canFireSplitingFireball = false;
                                randomShot = fireblast;

                                for (int i = 0; i < 24; i++)
                                {
                                    Dust magic = Dust.NewDustPerfect(projectileSpawn, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                                    magic.velocity = projectileVelocity.RotatedByRandom(0.36f) * Main.rand.NextFloat(0.9f, 1.1f) * 1.6f;
                                    magic.color = cirrus ? Color.Pink : Color.Red;
                                    magic.scale = 1.5f;
                                    magic.fadeIn = 0.8f;
                                    magic.noLight = true;
                                    magic.noGravity = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, gigablastDamage, 0f, Main.myPlayer);
                                    NPC.netUpdate = true;
                                }
                            }
                            else
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                canFireSplitingFireball = true;
                                randomShot = barrage;
                                float rotation = MathHelper.ToRadians(20);
                                int numProj = 8;
                                for (int j = 0; j < numProj; j++)
                                {
                                    for (int i = 0; i < 7; i++)
                                    {
                                        Vector2 magicDustVelocity = projectileVelocity;
                                        magicDustVelocity *= MathHelper.Lerp(0.3f, 1f, i / 7f);

                                        Dust magic = Dust.NewDustPerfect(projectileSpawn, 264);
                                        magic.velocity = magicDustVelocity;
                                        magic.color = cirrus ? Color.Pink : Color.Red;
                                        magic.scale = MathHelper.Lerp(0.85f, 1.5f, i / 7f);
                                        magic.fadeIn = 0.67f;
                                        magic.noLight = true;
                                        magic.noGravity = true;
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, j / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, perturbedSpeed, randomShot, barrageDamage, 0f, Main.myPlayer);
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                        }
                    }
                    else if (NPC.ai[1] == 1f)
                    {
                        float chargeVelocity = (wormAlive ? 26f : 30f) + (1f - lifeRatio) * 8f;

                        if (Main.getGoodWorld)
                            chargeVelocity *= 1.15f;

                        if (!canDespawn)
                        {
                            Vector2 vector = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                            NPC.velocity = vector * chargeVelocity;

                            shieldRotation = NPC.velocity.ToRotation();
                            NPC.netUpdate = true;

                            SoundEngine.PlaySound(DashSound, NPC.Center);
                        }

                        NPC.ai[1] = 2f;
                    }
                    else if (NPC.ai[1] == 2f)
                    {
                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 25f)
                        {
                            if (!canDespawn)
                            {
                                NPC.velocity *= 0.96f;

                                if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                                    NPC.velocity.X = 0f;
                                if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                                    NPC.velocity.Y = 0f;
                            }
                        }


                        bool willChargeAgain = NPC.ai[3] + 1 < 1;

                        if (NPC.ai[2] >= 70f)
                        {
                            NPC.ai[3] += 1f;
                            NPC.ai[2] = 0f;
                            NPC.TargetClosest();

                            if (!willChargeAgain)
                                NPC.ai[1] = -1f;
                            else
                                NPC.ai[1] = 1f;
                        }

                        if (willChargeAgain && NPC.ai[2] > 50f)
                        {
                            float idealRotation = NPC.AngleTo(player.Center);
                            shieldRotation = shieldRotation.AngleLerp(idealRotation, 0.125f);
                            shieldRotation = shieldRotation.AngleTowards(idealRotation, 0.18f);
                        }

                        FrameType = FrameAnimationType.FasterUpwardDraft;
                    }
                    else if (NPC.ai[1] == 3f)
                    {
                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (player.HoldingTrueMeleeWeapon())
                            acceleration *= 0.5f;

                        int posX = 1;
                        if (NPC.position.X + (NPC.width / 2) < player.position.X + player.width)
                            posX = -1;

                        // This is where SCal should be
                        Vector2 destination = new Vector2(player.Center.X + posX * 600f, player.Center.Y);

                        // How far SCal is from where she's supposed to be
                        Vector2 distanceFromDestination = destination - NPC.Center;

                        // Movement
                        if (!canDespawn)
                        {
                            // Set the velocity
                            CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                        }

                        Vector2 handPosition = NPC.Center + new Vector2(NPC.spriteDirection * -18f, 2f);

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 300f)
                        {
                            NPC.ai[1] = -1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }
                        else
                        {
                            if (!player.dead)
                                NPC.ai[3] += wormAlive ? 0.5f : 1f;

                            if (NPC.ai[3] >= 24f)
                            {
                                NPC.ai[3] = 0f;
                                SoundEngine.PlaySound(HellblastSound, NPC.Center);

                                // Release a burst of magic dust along with a brimstone hellblast skull.
                                for (int i = 0; i < 25; i++)
                                {
                                    Dust brimstoneMagic = Dust.NewDustPerfect(handPosition, 264);
                                    brimstoneMagic.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(0.31f) * Main.rand.NextFloat(3f, 5f) + NPC.velocity;
                                    brimstoneMagic.scale = Main.rand.NextFloat(1.25f, 1.35f);
                                    brimstoneMagic.noGravity = true;
                                    brimstoneMagic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                                    brimstoneMagic.fadeIn = 1.5f;
                                    brimstoneMagic.noLight = true;
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    Vector2 projectileSpawn = NPC.Center + projectileVelocity * 4f;
                                    projectileVelocity *= 10f * uDieLul;
                                    int projectileType = hellblast;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, hellblastDamage, 0f, Main.myPlayer);
                                }
                            }
                        }

                        if (Main.rand.NextBool())
                        {
                            Dust brimstoneMagic = Dust.NewDustPerfect(handPosition, 264);
                            brimstoneMagic.velocity = Vector2.UnitY.RotatedByRandom(0.14f) * Main.rand.NextFloat(-3.5f, -3f) + NPC.velocity;
                            brimstoneMagic.scale = Main.rand.NextFloat(1.25f, 1.35f);
                            brimstoneMagic.noGravity = true;
                            brimstoneMagic.noLight = true;
                        }

                        FrameChangeSpeed = 0.245f;
                        FrameType = FrameAnimationType.PunchHandCast;
                    }
                    else if (NPC.ai[1] == 4f)
                    {
                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        // Reduce acceleration if target is holding a true melee weapon
                        if (player.HoldingTrueMeleeWeapon())
                            acceleration *= 0.5f;

                        int posX = 1;
                        if (NPC.position.X + (NPC.width / 2) < player.position.X + player.width)
                            posX = -1;

                        // This is where SCal should be
                        Vector2 destination = new Vector2(player.Center.X + posX * 750f, player.Center.Y);

                        // How far SCal is from where she's supposed to be
                        Vector2 distanceFromDestination = destination - NPC.Center;

                        // Movement
                        if (!canDespawn)
                        {
                            // Set the velocity
                            CalamityUtils.SmoothMovement(NPC, 0f, distanceFromDestination, velocity, acceleration, true);
                        }

                        int shootRate = wormAlive ? 200 : 100;
                        NPC.localAI[1]++;
                        if (NPC.ai[2] > 40f && (NPC.localAI[1] > shootRate - 18 || NPC.localAI[1] <= 15f))
                        {
                            FrameChangeSpeed = 0f;
                            FrameType = FrameAnimationType.BlastPunchCast;
                        }

                        if (NPC.localAI[1] > shootRate)
                        {
                            Vector2 handPosition = NPC.Center + new Vector2(NPC.spriteDirection * -22f, 2f);

                            // Release a burst of magic dust when punching.
                            for (int i = 0; i < 25; i++)
                            {
                                Dust brimstoneMagic = Dust.NewDustPerfect(handPosition, 264);
                                brimstoneMagic.velocity = NPC.DirectionTo(player.Center).RotatedByRandom(0.24f) * Main.rand.NextFloat(5f, 7f) + NPC.velocity;
                                brimstoneMagic.scale = Main.rand.NextFloat(1.25f, 1.35f);
                                brimstoneMagic.noGravity = true;
                                brimstoneMagic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                                brimstoneMagic.fadeIn = 1.5f;
                                brimstoneMagic.noLight = true;
                            }
                            NPC.localAI[1] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                Vector2 projectileSpawn = NPC.Center + projectileVelocity * 8f;
                                projectileVelocity *= 5f * uDieLul;
                                int projectileType = gigablast;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, fireblastDamage, 0f, Main.myPlayer);
                            }
                        }

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 240f)
                        {
                            NPC.ai[1] = -1f;
                            NPC.TargetClosest();
                            NPC.netUpdate = true;
                        }
                    }
                }
            }
            #endregion
        }

        public void DoHeartsSpawningCastAnimation(Player target, bool death)
        {
            int tempSpawnY = spawnY;
            tempSpawnY += 250;
            if (death)
                tempSpawnY -= 50;

            List<Vector2> heartSpawnPositions = new List<Vector2>();
            for (int i = 0; i < 5; i++)
            {
                heartSpawnPositions.Add(new Vector2(spawnX + spawnXAdd * i + 50, tempSpawnY + spawnYAdd * i));
                heartSpawnPositions.Add(new Vector2(spawnX2 - spawnXAdd * i - 50, tempSpawnY + spawnYAdd * i));
            }

            float castCompletion = Utils.GetLerpValue(sepulcherSpawnCastTime - 25f, 0f, attackCastDelay, true);
            Vector2 armPosition = NPC.Center + Vector2.UnitX * NPC.spriteDirection * -8f;

            // Emit dust at the arm position as a sort of magic effect.
            Dust magic = Dust.NewDustPerfect(armPosition, 264);
            magic.velocity = Vector2.UnitY.RotatedByRandom(0.17f) * -Main.rand.NextFloat(2.7f, 4.1f);
            magic.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
            magic.noLight = true;
            magic.fadeIn = 0.6f;
            magic.noGravity = true;

            foreach (Vector2 heartSpawnPosition in heartSpawnPositions)
            {
                Vector2 leftDustPosition = Vector2.CatmullRom(armPosition + Vector2.UnitY * 1000f, armPosition, heartSpawnPosition, heartSpawnPosition + Vector2.UnitY * 1000f, castCompletion);

                Dust castMagicDust = Dust.NewDustPerfect(leftDustPosition, 267);
                castMagicDust.scale = 1.67f;
                castMagicDust.velocity = Main.rand.NextVector2CircularEdge(0.2f, 0.2f);
                castMagicDust.fadeIn = 0.67f;
                castMagicDust.color = cirrus ? Color.Pink : Color.Red;
                castMagicDust.noGravity = true;
            }

            if (attackCastDelay == 0)
            {
                string key = cirrus ? "Mods.CalamityMod.Status.Boss.CirrusBirbSwarmText" : "Mods.CalamityMod.Status.Boss.SCalStartText";
                if (NPC.life <= NPC.lifeMax * 0.08)
                    key = cirrus ? "Mods.CalamityMod.Status.Boss.CirrusSecondBirbSwarmText" : "Mods.CalamityMod.Status.Boss.SCalSepulcher2Text";

                if (!BossRushEvent.BossRushActive)
                {
                    if (DownedBossSystem.downedCalamitas && !cirrus)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                foreach (Vector2 heartSpawnPosition in heartSpawnPositions)
                {
                    // Make the hearts appear in a burst of flame.
                    // Spawn Dragonfollies if Cirrus exists.
                    for (int i = 0; i < 20; i++)
                    {
                        Dust castFire = Dust.NewDustPerfect(heartSpawnPosition + Main.rand.NextVector2Square(-30f, 30f), cirrus ? (int)CalamityDusts.Polterplasm : (int)CalamityDusts.Brimstone);
                        castFire.velocity = Vector2.UnitY.RotatedByRandom(0.08f) * -Main.rand.NextFloat(3f, 4.45f);
                        castFire.scale = Main.rand.NextFloat(1.35f, 1.6f);
                        castFire.fadeIn = 1.25f;
                        castFire.noGravity = true;
                    }
                }

                // And play a fire-like sound effect.
                hasSummonedSepulcher1 = true;
                hasSummonedSepulcher2 = NPC.life <= NPC.lifeMax * 0.08;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (cirrus)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), spawnX + 50, tempSpawnY, ModContent.NPCType<Bumblefuck>());
                            spawnX += spawnXAdd;

                            NPC.NewNPC(NPC.GetSource_FromAI(), spawnX2 - 50, tempSpawnY, ModContent.NPCType<Bumblefuck>());
                            spawnX2 -= spawnXAdd;
                            tempSpawnY += spawnYAdd;
                        }

                        spawnX = spawnXReset;
                        spawnX2 = spawnXReset2;
                        spawnY = spawnYReset;
                    }
                    else
                    {
                        List<int> hearts = new List<int>();
                        for (int x = 0; x < 5; x++)
                        {
                            hearts.Add(NPC.NewNPC(NPC.GetSource_FromAI(), spawnX + 50, tempSpawnY, ModContent.NPCType<BrimstoneHeart>()));
                            spawnX += spawnXAdd;

                            hearts.Add(NPC.NewNPC(NPC.GetSource_FromAI(), spawnX2 - 50, tempSpawnY, ModContent.NPCType<BrimstoneHeart>()));
                            spawnX2 -= spawnXAdd;
                            tempSpawnY += spawnYAdd;
                        }

                        ConnectAllBrimstoneHearts(hearts);

                        spawnX = spawnXReset;
                        spawnX2 = spawnXReset2;
                        spawnY = spawnYReset;

                        if (NPC.CountNPCS(ModContent.NPCType<SepulcherHead>()) <= 0) // Check is here for the zenith seed
                            NPC.SpawnOnPlayer(NPC.FindClosestPlayer(), ModContent.NPCType<SepulcherHead>());
                    }

                    NPC.netUpdate = true;
                }

                SoundEngine.PlaySound(SepulcherSummonSound, target.Center);
            }
        }

        public void DoBrothersSpawningCastAnimation(int bodyWidth, int bodyHeight)
        {
            Vector2 leftOfCircle = NPC.Center + Vector2.UnitY * bodyHeight * 0.5f - Vector2.UnitX * bodyWidth * 0.45f;
            Vector2 rightOfCircle = NPC.Center + Vector2.UnitY * bodyHeight * 0.5f + Vector2.UnitX * bodyWidth * 0.45f;

            if (Main.netMode != NetmodeID.MultiplayerClient && catastropheSpawnPosition == Vector2.Zero)
            {
                catastropheSpawnPosition = NPC.Center - Vector2.UnitX * 500f;
                cataclysmSpawnPosition = NPC.Center + Vector2.UnitX * 500f;
                NPC.netUpdate = true;
            }

            // Draw some magic dust much like the sandstorm elemental cast that approaches where the brothers will spawn.
            if (attackCastDelay < brothersSpawnCastTime - 45f && attackCastDelay >= 60f)
            {
                float castCompletion = Utils.GetLerpValue(brothersSpawnCastTime - 45f, 60f, attackCastDelay);

                Vector2 leftDustPosition = Vector2.CatmullRom(leftOfCircle + Vector2.UnitY * 1000f, leftOfCircle, catastropheSpawnPosition, catastropheSpawnPosition + Vector2.UnitY * 1000f, castCompletion);
                Vector2 rightDustPosition = Vector2.CatmullRom(rightOfCircle + Vector2.UnitY * 1000f, rightOfCircle, cataclysmSpawnPosition, cataclysmSpawnPosition + Vector2.UnitY * 1000f, castCompletion);

                Dust castMagicDust = Dust.NewDustPerfect(leftDustPosition, 267);
                castMagicDust.scale = 1.67f;
                castMagicDust.velocity = Main.rand.NextVector2CircularEdge(0.2f, 0.2f);
                castMagicDust.color = cirrus ? Color.Pink : Color.Red;
                castMagicDust.noGravity = true;

                castMagicDust = Dust.CloneDust(castMagicDust);
                castMagicDust.position = rightDustPosition;
            }

            // Make some magic effects at where the bros will spawn.
            if (attackCastDelay < 60f)
            {
                float burnPower = Utils.GetLerpValue(60f, 20f, attackCastDelay);
                if (attackCastDelay == 0f)
                    burnPower = 4f;

                for (int i = 0; i < MathHelper.Lerp(5, 25, burnPower); i++)
                {
                    Dust fire = Dust.NewDustPerfect(catastropheSpawnPosition + Main.rand.NextVector2Circular(60f, 60f), 264);
                    fire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 4f);

                    if (attackCastDelay == 0)
                    {
                        fire.velocity += Main.rand.NextVector2Circular(4f, 4f);
                        fire.fadeIn = 1.6f;
                    }

                    fire.noGravity = true;
                    fire.noLight = true;
                    fire.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                    fire.scale = 1.4f + fire.velocity.Y * 0.16f;

                    fire = Dust.NewDustPerfect(cataclysmSpawnPosition + Main.rand.NextVector2Circular(60f, 60f), 264);
                    fire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2f, 4f);

                    if (attackCastDelay == 0)
                    {
                        fire.velocity += Main.rand.NextVector2Circular(4f, 4f);
                        fire.fadeIn = 1.6f;
                    }

                    fire.noGravity = true;
                    fire.noLight = true;
                    fire.color = cirrus ? Color.BlueViolet : Color.OrangeRed;
                    fire.scale = 1.4f + fire.velocity.Y * 0.16f;
                }
            }

            // And spawn them.
            if (attackCastDelay == 0)
            {
                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBrothersText";
                    if (cirrus)
                        key = "Mods.CalamityMod.Status.Boss.CirrusDoGText";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.DisplayLocalizedText(key, cirrus ? cirrusTextColor : textColor);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    CalamityUtils.SpawnBossBetter(catastropheSpawnPosition, cirrus ? ModContent.NPCType<DevourerofGodsHead>() : ModContent.NPCType<SupremeCatastrophe>());
                    CalamityUtils.SpawnBossBetter(cataclysmSpawnPosition, cirrus ? ModContent.NPCType<DevourerofGodsHead>() : ModContent.NPCType<SupremeCataclysm>());
                }

                SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, NPC.Center);
                hasSummonedBrothers = true;
            }
        }

        public void ConnectAllBrimstoneHearts(List<int> heartIndices)
        {
            int heartType = ModContent.NPCType<BrimstoneHeart>();

            // Ensure that the hearts go in order based on the arena.
            IEnumerable<NPC> hearts = heartIndices.Select(i => Main.npc[i]);
            hearts = hearts.OrderByDescending(heart => Math.Abs(heart.Center.X - safeBox.Left)).ToList();

            int firstHeartIndex = heartIndices.First();
            int lastHeartIndex = heartIndices.Last();

            heartIndices = heartIndices.OrderByDescending(heart => Math.Abs(Main.npc[heart].Center.X - safeBox.Left)).ToList();

            for (int i = 0; i < hearts.Count(); i++)
            {
                NPC heart = hearts.ElementAt(i);

                Vector2 endpoint = safeBox.TopLeft();
                Vector2 oppositePosition = Vector2.Zero;

                for (int j = 0; j < 2; j++)
                {
                    int tries = 0;
                    do
                    {
                        endpoint.X = heart.Center.X + (j == 0).ToDirectionInt() * Main.rand.NextFloat(75f, 250f);
                        tries++;
                        if (tries >= 100)
                            break;
                    }
                    while (Math.Abs(endpoint.X - safeBox.Center.X) > safeBox.Width * 0.48f);

                    if (tries >= 100)
                        endpoint.X = MathHelper.Clamp(endpoint.X, safeBox.Left, safeBox.Right);

                    heart.ModNPC<BrimstoneHeart>().ChainEndpoints.Add(endpoint);
                }

                if (Main.rand.NextBool())
                {
                    endpoint.X = heart.Center.X + Main.rand.NextBool().ToDirectionInt() * Main.rand.NextFloat(45f, 360f);
                    endpoint.X = MathHelper.Clamp(endpoint.X, safeBox.Left, safeBox.Right);
                    heart.ModNPC<BrimstoneHeart>().ChainEndpoints.Add(endpoint);
                }

                heart.netUpdate = true;
            }
        }

        public void HandleMusicVariables()
        {
            float lifeRatio = NPC.life / (float)NPC.lifeMax;

            CalamityGlobalNPC.SCalGrief = -1;
            CalamityGlobalNPC.SCalLament = -1;
            CalamityGlobalNPC.SCalEpiphany = -1;
            CalamityGlobalNPC.SCalAcceptance = -1;

            if (startFifthAttack && gettingTired5)
                CalamityGlobalNPC.SCalAcceptance = NPC.whoAmI;
            else if (lifeRatio <= 0.3f)
                CalamityGlobalNPC.SCalEpiphany = NPC.whoAmI;
            else if (lifeRatio <= 0.5f)
                CalamityGlobalNPC.SCalLament = NPC.whoAmI;
            else
                CalamityGlobalNPC.SCalGrief = NPC.whoAmI;
        }

        #region Loot
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void OnKill()
        {
            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Increase the player's SCal kill count
            if (Main.player[NPC.target].Calamity().sCalKillCount < 5)
                Main.player[NPC.target].Calamity().sCalKillCount++;

            // Spawn the SCal NPC directly where the boss was
            if (!BossRushEvent.BossRushActive)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 12, cirrus ? ModContent.NPCType<FAP>() : ModContent.NPCType<WITCH>());

            // Mark Calamitas as defeated
            DownedBossSystem.downedCalamitas = true;
            CalamityNetcode.SyncWorld();
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<CalamitasCoffer>()));

            // Normal drops: Everything that would otherwise be in the bag
            var normalOnly = npcLoot.DefineNormalOnlyDropSet();
            {
                // Weapons
                int[] weapons = new int[]
                {
                    ModContent.ItemType<Violence>(),
                    ModContent.ItemType<Condemnation>(),
                    ModContent.ItemType<Heresy>(),
                    ModContent.ItemType<Vehemence>(),
                    ModContent.ItemType<Perdition>(),
                    ModContent.ItemType<Vigilance>(),
                    ModContent.ItemType<Sacrifice>(),
                };
                normalOnly.Add(DropHelper.CalamityStyle(DropHelper.NormalWeaponDropRateFraction, weapons));

                // Materials
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<AshesofAnnihilation>(), 1, 25, 30));

                // Equipment
                normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<Calamity>()));

                // SCal vanity set (This drops all at once, or not at all)
				var scalVanitySet = ItemDropRule.Common(ModContent.ItemType<AshenHorns>(), 7);
				scalVanitySet.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SCalMask>()));
				scalVanitySet.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SCalRobes>()));
				scalVanitySet.OnSuccess(ItemDropRule.Common(ModContent.ItemType<SCalBoots>()));
				normalOnly.Add(scalVanitySet);

				// Furniture
                normalOnly.Add(ModContent.ItemType<ThankYouPainting>(), ThankYouPainting.DropInt);
            }

            // One of the only Death-exclusive drops in the mod, as requested by Leviathan: Levi pet and Gael's Greatsword
            npcLoot.AddIf(() => CalamityWorld.death, ModContent.ItemType<Levi>());
            npcLoot.AddIf(() => CalamityWorld.death, ModContent.ItemType<GaelsGreatsword>());

            npcLoot.Add(ModContent.ItemType<SupremeCalamitasTrophy>(), 10);

            // Relic
            npcLoot.DefineConditionalDropSet(DropHelper.RevAndMaster).Add(ModContent.ItemType<CalamitasRelic>());

            // GFB Slurper Pole drops
            var GFBOnly = npcLoot.DefineConditionalDropSet(DropHelper.GFB);
            {
                GFBOnly.Add(ModContent.ItemType<SlurperPole>(), hideLootReport: true);
            }

            // Legendary seed pony on a stick upgrade          
            npcLoot.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<SupremeCalamitas>() && info.npc.ModNPC<SupremeCalamitas>().cirrus, false), ModContent.ItemType<AlicornonaStick>()));

            // Lore
            npcLoot.AddConditionalPerPlayer(() => !DownedBossSystem.downedCalamitas, ModContent.ItemType<LoreCalamitas>(), desc: DropHelper.FirstKillText);

            // Cynosure: If the Exo Mechs have been defeated and this is the first kill of SCal, drop the special lore item
            npcLoot.Add(ItemDropRule.ByCondition(
                DropHelper.If(
                    () => DownedBossSystem.downedExoMechs && !DownedBossSystem.downedCalamitas,
                    desc: DropHelper.CynosureText),
                ModContent.ItemType<LoreCynosure>()
            ));
        }
        #endregion

        // Prevent the player from accidentally killing SCal instead of having her turn into a town NPC.
        public override bool CheckDead()
        {
            if (BossRushEvent.BossRushActive)
                return true;

            NPC.life = 1;
            NPC.active = true;
            NPC.dontTakeDamage = true;
            NPC.netUpdate = true;
            return false;
        }

        public override bool CheckActive()
        {
            return canDespawn;
        }

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            scale = 1.5f;
            return null;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = ImmunityCooldownID.Bosses;

            Vector2 shieldCenter = NPC.Center + shieldRotation.ToRotationVector2() * 24f;
            Vector2 shieldTop = shieldCenter - (shieldRotation + MathHelper.PiOver2).ToRotationVector2() * 61f;
            Vector2 shieldBottom = shieldCenter - (shieldRotation + MathHelper.PiOver2).ToRotationVector2() * 61f;

            float _ = 0f;
            bool collidingWithShield = Collision.CheckAABBvLineCollision(target.TopLeft, target.Size, shieldTop, shieldBottom, 64f, ref _) && shieldOpacity > 0.55f;
            return collidingWithShield || NPC.Hitbox.Intersects(target.Hitbox);
        }

        public override void FindFrame(int frameHeight)
        {
            if (NPC.IsABestiaryIconDummy)
            {
                FrameType = FrameAnimationType.UpwardDraft;
                FrameChangeSpeed = 0.2f;
            }

            bool wormAlive = false;
            if (CalamityGlobalNPC.SCalWorm != -1)
                wormAlive = Main.npc[CalamityGlobalNPC.SCalWorm].active;
            int shootRate = wormAlive ? 200 : 100;

            // Special punch logic for the blast attack.
            if (FrameType == FrameAnimationType.BlastPunchCast && (NPC.localAI[1] > shootRate - 18 || NPC.localAI[1] <= 15f))
            {
                if (NPC.localAI[1] > shootRate - 18)
                    NPC.frame.Y = (int)MathHelper.Lerp(0, 3, Utils.GetLerpValue(shootRate - 18, shootRate, NPC.localAI[1], true));
                else
                    NPC.frame.Y = (int)MathHelper.Lerp(3, 5, Utils.GetLerpValue(0f, 15f, NPC.localAI[1], true));
                NPC.frame.Y += (int)FrameType * 6;
            }
            else
            {
                NPC.frameCounter += FrameChangeSpeed;
                NPC.frameCounter %= 6;
                NPC.frame.Y = (int)NPC.frameCounter + (int)FrameType * 6;
            }
            if (cirrus)
            {
                alicornFrameCounter++;
                if (alicornFrameCounter > 6)
                {
                    alicornFrame ++;
                    alicornFrameCounter = 0;
                }
                if (alicornFrame > 14 || alicornFrame < 9)
                {
                    alicornFrame = 9;
                }
            }
        }

        public override Color? GetAlpha(Color drawColor) => willCharge ? drawColor * NPC.Opacity * 0.45f : null;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = DownedBossSystem.downedCalamitas && !BossRushEvent.BossRushActive ? TextureAssets.Npc[NPC.type].Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeCalamitasHooded").Value;
            Texture2D pony = ModContent.Request<Texture2D>("CalamityMod/Items/Mounts/AlicornMount_Front").Value;
            bool inPhase2 = NPC.ai[0] >= 3f && (NPC.life > NPC.lifeMax * 0.01 || cirrus);

            if (cirrus)
                texture2D15 = inPhase2 ? ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeCirrus_Shimmered").Value : ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeCirrus").Value;

            Vector2 halfSizeTexture = new Vector2(texture2D15.Width / 2f, texture2D15.Height / Main.npcFrameCount[NPC.type] / 2f);
            Vector2 ponyOrigin = new Vector2(pony.Width / 2f, pony.Height / 30f);
            int afterimageAmt = 7;

            Rectangle frame = texture2D15.Frame(2, Main.npcFrameCount[NPC.type], NPC.frame.Y / Main.npcFrameCount[NPC.type], NPC.frame.Y % Main.npcFrameCount[NPC.type]);
            Rectangle ponyFrame = pony.Frame(1, 15, 0, alicornFrame);
            Vector2 ponyPos = NPC.Center - screenPos;
            ponyPos -= new Vector2(pony.Width / 2f, pony.Height / 15) * NPC.scale / 2f;
            ponyPos += ponyOrigin * NPC.scale + new Vector2(-20, NPC.gfxOffY);

            if (CalamityConfig.Instance.Afterimages && !(cirrus && NPC.ai[1] == 2f))
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width / 2f, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width / 2f, texture2D15.Height / Main.npcFrameCount[NPC.type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            if (!(cirrus && NPC.ai[1] == 2f))
            {
                if (inPhase2)
                {
                    // Make the sprite jitter with rage in phase 2. This does not happen in rematches since it would make little sense logically.
                    if (!DownedBossSystem.downedCalamitas)
                        drawLocation += Main.rand.NextVector2Circular(0.25f, 0.7f);

                    // And gain a flaming aura.
                    Color auraColor = NPC.GetAlpha(cirrus ? Color.Pink : Color.Red) * 0.4f;
                    for (int i = 0; i < 7; i++)
                    {
                        Vector2 rotationalDrawOffset = (MathHelper.TwoPi * i / 7f + Main.GlobalTimeWrappedHourly * 4f).ToRotationVector2();
                        rotationalDrawOffset *= MathHelper.Lerp(3f, 4.25f, (float)Math.Cos(Main.GlobalTimeWrappedHourly * 4f) * 0.5f + 0.5f);
                        spriteBatch.Draw(texture2D15, drawLocation + rotationalDrawOffset, frame, auraColor, NPC.rotation, halfSizeTexture, NPC.scale * 1.1f, spriteEffects, 0f);
                    }
                }
                spriteBatch.Draw(texture2D15, drawLocation, frame, NPC.GetAlpha(drawColor), NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
            }

            if (!NPC.IsABestiaryIconDummy)
            {
                DrawForcefield(spriteBatch);
                if (cirrus)
                {
                    if (NPC.ai[1] == 2f)
                    {
                        spriteBatch.Draw(pony, ponyPos, ponyFrame, NPC.GetAlpha(drawColor), NPC.rotation, ponyOrigin, NPC.scale, spriteEffects, 0f);
                    }
                }
                else
                {
                    DrawShield(spriteBatch);
                }
            }
            return false;
        }

        public void DrawForcefield(SpriteBatch spriteBatch)
        {
            spriteBatch.EnterShaderRegion();

            float intensity = hitTimer / 35f;

            // Shield intensity is always high during invincibility, except during cast animations, so that she can be more easily seen.
            if (NPC.dontTakeDamage && attackCastDelay <= 0)
                intensity = 0.75f + Math.Abs((float)Math.Cos(Main.GlobalTimeWrappedHourly * 1.7f)) * 0.1f;

            // Make the forcefield weaker in the second phase as a means of showing desparation.
            if (NPC.ai[0] >= 3f)
                intensity *= 0.6f;

            float lifeRatio = NPC.life / (float)NPC.lifeMax;
            float flickerPower = 0f;
            if (lifeRatio < 0.6f)
                flickerPower += 0.1f;
            if (lifeRatio < 0.3f)
                flickerPower += 0.15f;
            if (lifeRatio < 0.1f)
                flickerPower += 0.2f;
            if (lifeRatio < 0.05f)
                flickerPower += 0.1f;
            float opacity = forcefieldOpacity;
            opacity *= MathHelper.Lerp(1f, MathHelper.Max(1f - flickerPower, 0.56f), (float)Math.Pow(Math.Cos(Main.GlobalTimeWrappedHourly * MathHelper.Lerp(3f, 5f, flickerPower)), 24D));

            // During/prior to a charge the forcefield is always darker than usual and thus its intensity is also higher.
            if (!NPC.dontTakeDamage && (willCharge || NPC.ai[1] == 2f))
                intensity = 1.1f;

            // Dampen the opacity and intensity slightly, to allow SCal to be more easily visible inside of the forcefield.
            intensity *= 0.75f;
            opacity *= 0.75f;

            Texture2D forcefieldTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/ForcefieldTexture").Value;
            GameShaders.Misc["CalamityMod:SupremeShield"].UseImage1("Images/Misc/Perlin");

            Color forcefieldColor = Color.DarkViolet;
            Color secondaryForcefieldColor = (cirrus ? Color.HotPink : Color.Red) * 1.4f;

            if (!NPC.dontTakeDamage && (willCharge || NPC.ai[1] == 2f))
            {
                forcefieldColor *= 0.25f;
                secondaryForcefieldColor = Color.Lerp(secondaryForcefieldColor, Color.Black, 0.7f);
            }

            forcefieldColor *= opacity;
            secondaryForcefieldColor *= opacity;

            GameShaders.Misc["CalamityMod:SupremeShield"].UseSecondaryColor(secondaryForcefieldColor);
            GameShaders.Misc["CalamityMod:SupremeShield"].UseColor(forcefieldColor);
            GameShaders.Misc["CalamityMod:SupremeShield"].UseSaturation(intensity);
            GameShaders.Misc["CalamityMod:SupremeShield"].UseOpacity(opacity);
            GameShaders.Misc["CalamityMod:SupremeShield"].Apply();

            spriteBatch.Draw(forcefieldTexture, NPC.Center - Main.screenPosition, null, Color.White * opacity, 0f, forcefieldTexture.Size() * 0.5f, forcefieldScale * 3f, SpriteEffects.None, 0f);

            spriteBatch.ExitShaderRegion();
        }

        public void DrawShield(SpriteBatch spriteBatch)
        {
            float jawRotation = shieldRotation;
            float jawRotationOffset = 0f;

            // Have an agape mouth when charging.
            if (NPC.ai[1] == 2f)
                jawRotationOffset -= 0.71f;

            // And a laugh right before the charge.
            else if (willCharge && NPC.ai[1] != 2f && AttackCloseToBeingOver)
                jawRotationOffset += MathHelper.Lerp(0.04f, -0.82f, (float)Math.Sin(Main.GlobalTimeWrappedHourly * 17.2f) * 0.5f + 0.5f);

            Color shieldColor = Color.White * shieldOpacity;
            Texture2D shieldSkullTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeShieldTop").Value;
            Texture2D shieldJawTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeShieldBottom").Value;
            Vector2 drawPosition = NPC.Center + shieldRotation.ToRotationVector2() * 24f - Main.screenPosition;
            Vector2 jawDrawPosition = drawPosition;
            SpriteEffects direction = Math.Cos(shieldRotation) > 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
            if (direction == SpriteEffects.FlipVertically)
                jawDrawPosition += (shieldRotation - MathHelper.PiOver2).ToRotationVector2() * 42f;
            else
            {
                jawDrawPosition += (shieldRotation + MathHelper.PiOver2).ToRotationVector2() * 42f;
                jawRotationOffset *= -1f;
            }

            spriteBatch.Draw(shieldJawTexture, jawDrawPosition, null, shieldColor, jawRotation + jawRotationOffset, shieldJawTexture.Size() * 0.5f, 1f, direction, 0f);
            spriteBatch.Draw(shieldSkullTexture, drawPosition, null, shieldColor, shieldRotation, shieldSkullTexture.Size() * 0.5f, 1f, direction, 0f);
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                hitTimer = 35;
                NPC.netUpdate = true;
            }

            // hit sound
            if (NPC.soundDelay == 0)
            {
                NPC.soundDelay = Main.rand.Next(5, 8);
                SoundEngine.PlaySound(HurtSound, NPC.Center);
            }

            for (int k = 0; k < 5; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (NPC.height / 2);
                NPC.width = 100;
                NPC.height = 100;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int onHitDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[onHitDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[onHitDust].scale = 0.5f;
                        Main.dust[onHitDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int onHitDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[onHitDust2].noGravity = true;
                    Main.dust[onHitDust2].velocity *= 5f;
                    onHitDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, cirrus ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[onHitDust2].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
            NPC.damage = (int)(NPC.damage * NPC.GetExpertDamageMultiplier());
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage > 0)
            {
                target.AddBuff(ModContent.BuffType<VulnerabilityHex>(), 600);
                InflictCirrusDebuffs(target);
            }
        }

        public void InflictCirrusDebuffs(Player target)
        {
            if (cirrus)
            {
                switch (Main.rand.Next(MaxCirrusAlcohols))
                {
                    case 0:
                        target.AddBuff(ModContent.BuffType<BloodyMaryBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 1:
                        target.AddBuff(ModContent.BuffType<CaribbeanRumBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 2:
                        target.AddBuff(ModContent.BuffType<CinnamonRollBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 3:
                        target.AddBuff(ModContent.BuffType<EverclearBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 4:
                        target.AddBuff(ModContent.BuffType<EvergreenGinBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 5:
                        target.AddBuff(ModContent.BuffType<FabsolVodkaBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 6:
                        target.AddBuff(ModContent.BuffType<FireballBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 7:
                        target.AddBuff(ModContent.BuffType<GrapeBeerBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 8:
                        target.AddBuff(ModContent.BuffType<MargaritaBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 9:
                        target.AddBuff(ModContent.BuffType<MoonshineBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 10:
                        target.AddBuff(ModContent.BuffType<MoscowMuleBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 11:
                        target.AddBuff(ModContent.BuffType<RedWineBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 12:
                        target.AddBuff(ModContent.BuffType<RumBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 13:
                        target.AddBuff(ModContent.BuffType<ScrewdriverBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 14:
                        target.AddBuff(ModContent.BuffType<StarBeamRyeBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 15:
                        target.AddBuff(ModContent.BuffType<TequilaBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 16:
                        target.AddBuff(ModContent.BuffType<TequilaSunriseBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 17:
                        target.AddBuff(ModContent.BuffType<VodkaBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 18:
                        target.AddBuff(ModContent.BuffType<WhiskeyBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 19:
                        target.AddBuff(ModContent.BuffType<WhiteWineBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                    case 20:
                        target.AddBuff(ModContent.BuffType<OldFashionedBuff>(), MaxCirrusAlcoholDebuffDuration);
                        break;
                }
            }
        }
    }
}
