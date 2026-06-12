using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CalamityMod.Dusts;
using CalamityMod.Events;
using CalamityMod.Graphics.Metaballs;
using CalamityMod.Items.Armor.Vanity;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Items.LoreItems;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Pets;
using CalamityMod.Items.Placeables.Furniture.BossRelics;
using CalamityMod.Items.Placeables.Furniture.Paintings;
using CalamityMod.Items.Placeables.Furniture.Trophies;
using CalamityMod.Items.Potions;
using CalamityMod.Items.TreasureBags;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Weapons.Typeless;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.TownNPCs;
using CalamityMod.Particles;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Systems.Collections;
using CalamityMod.Systems.Mechanic;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

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

        public static Color CurrentColor
        {
            get
            {
                if (CalamityGlobalNPC.SCal < 0)
                    return AcceptanceColor;

                var npc = Main.npc[CalamityGlobalNPC.SCal];
                if (npc == null || !npc.active)
                    return AcceptanceColor;

                if (npc.ModNPC is not SupremeCalamitas calamitas || calamitas.ArenaBox is null)
                    return AcceptanceColor;

                return calamitas.ArenaBox.borderColor;
            }
        }
        public static Color GriefColor => Color.Crimson;
        public static Color LamentColor => Color.RoyalBlue;
        public static Color EpiphanyColor => new Color(219, 75, 2);
        public static Color AcceptanceColor => Color.Gray;

        public const int BulletHellDuration = 900;
        public const int SecondBulletHellEndValue = BulletHellDuration * 2;
        public const int ThirdBulletHellEndValue = BulletHellDuration * 3;
        public const int FourthBulletHellEndValue = BulletHellDuration * 4;
        public const int FifthBulletHellEndValue = BulletHellDuration * 5;

        public bool FinishedBH1 => bulletHellCounter2 >= BulletHellDuration;
        public bool FinishedBH2 => bulletHellCounter2 >= SecondBulletHellEndValue;
        public bool FinishedBH3 => bulletHellCounter2 >= ThirdBulletHellEndValue;
        public bool FinishedBH4 => bulletHellCounter2 >= FourthBulletHellEndValue;
        public bool FinishedBH5 => bulletHellCounter2 >= FifthBulletHellEndValue;

        public float lifeRatio = 1;

        public bool IsAtHp_BH2 => lifeRatio <= 0.75f;
        public bool IsAtHp_BH3 => lifeRatio <= 0.5f;
        public bool IsAtHp_BH4 => lifeRatio <= 0.3f;
        public bool IsAtHp_BH5 => lifeRatio <= 0.1f;
        public bool IsAtHp_Acceptance => NPC.life < 2;

        public bool IsAtBrothers => IsAtHp_BH3 && FinishedBH3;
        public bool IsAtSeekers => lifeRatio <= 0.2f;
        public bool IsAtSep2 => IsAtHp_BH5 && FinishedBH5;

        public const int PermafrostAbsoluteZeroDamage = 1400;
        private const float PermafrostPhotonRipperDashVelocity = 6f;
        private const float PermafrostPhotonRipperMinDistanceFromTarget = 64f;
        private const float PermafrostPhotonRipperDashAcceleration = 0.3f;

        public float bossLife;
        public float uDieLul = 1f;
        public float passedVar = 0f;

        public bool protectionBoost = false;
        public bool canDespawn = false;
        public bool despawnProj = false;
        public bool startText = false;
        public bool startBattle = false;
        public bool hasSummonedSepulcher1 = false;
        public bool startSecondAttack = false;
        public bool startThirdAttack = false;
        public bool halfLife = false;
        public bool startFourthAttack = false;
        public bool hasSummonedSeekers = false;
        public bool startFifthAttack = false;
        public bool gettingTired = false;
        public bool hasSummonedSepulcher2 = false;
        public bool gettingTired2 = false;
        public bool gettingTired3 = false;
        public bool gettingTired4 = false;
        public bool gettingTired5 = false;
        public bool willCharge = false;
        public bool canFireSplitingFireball = true;
        public bool fireFireblastFirst = true;
        public bool spawnArena = false;
        public bool enteredBrothersPhase = false;
        public bool hasSummonedBrothers = false;
        public bool permafrost = false;
        public bool hasDoneDeathAnim = false;
        public bool postMusicHit = false;

        private const int GiveUpCounterMax = 1200;
        private const int musicSyncCounterMax = 2082; // Used to sync a visual with scal's third music track
        public int giveUpCounter = GiveUpCounterMax;
        public int musicSyncCounter = musicSyncCounterMax;
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
        public int dashVisualCounter = 0;
        public int preventionPause = 15; // Helps prevent telefragging
        public int attackPause = 0;
        public bool respawnBro = true; // Master Mode change
        public bool teleport = false; //Used to teleport SCal to the center of the arena at the start of each bullet hell

        public float shieldOpacity = 1f;
        public float shieldRotation = 0f;
        public float forcefieldOpacity = 1f;
        public float forcefieldScale = 1;
        public float forcefieldPureVisualScale = 1;
        public float rotateToPlayer = 0;
        public float rotateAwayPlayer = 0;

        public float colorCompletion = 1;

        public ArenaWallSystem.Box ArenaBox = null;

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

        public static int hoodedHeadIconIndex;
        public static int hoodedHeadIconP2Index;
        public static int hoodlessHeadIconIndex;
        public static int hoodlessHeadIconP2Index;
        public static int permafrostHeadIconIndex;
        public static float normalDR = 0.25f;
        public static float enragedDR = 0.9999f;

        public static readonly Color textColor = Color.Orange;
        public static readonly Color permafrostTextColor = Color.LightCyan;
        public const int sepulcherSpawnCastTime = 75;
        public const int brothersSpawnCastTime = 150;

        // Sounds.
        public static readonly SoundStyle SpawnSound = new("CalamityMod/Sounds/Custom/SupremeCalamitasSpawn") { Volume = 1.2f };
        public static readonly SoundStyle SepulcherSummonSound = new("CalamityMod/Sounds/Custom/SCalSounds/SepulcherSpawn");
        public static readonly SoundStyle BrimstoneShotSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneShoot");
        public static readonly SoundStyle BrotherHit = new("CalamityMod/Sounds/Custom/SCalSounds/BrothersHurt", 2);
        public static readonly SoundStyle BrotherDeath = new("CalamityMod/Sounds/Custom/SCalSounds/BrothersDeath", 2);
        public static readonly SoundStyle CatastropheSwing = new("CalamityMod/Sounds/Custom/SCalSounds/CatastropheResonanceSlash");
        public static readonly SoundStyle BrimstoneBigShotSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneBigShoot"); // DON'T YOU WANNA BE A [BIG SHOT]
        public static readonly SoundStyle DashSound = new("CalamityMod/Sounds/Custom/SCalSounds/SCalDash");
        public static readonly SoundStyle HellblastSound = new("CalamityMod/Sounds/Custom/SCalSounds/BrimstoneHellblastSound");
        public static readonly SoundStyle HurtSound = new("CalamityMod/Sounds/NPCHit/ShieldHit", 3);
        public static readonly SoundStyle BulletHellSound = new("CalamityMod/Sounds/Custom/SCalSounds/SCalRumble");
        public static readonly SoundStyle BulletHellEndSound = new("CalamityMod/Sounds/Custom/SCalSounds/SCalEndBH");
        public static readonly SoundStyle GiveUpSound = new("CalamityMod/Sounds/Custom/SCalSounds/SupremeCalamitasGiveUp");
        public SlotId BulletHellRumbleSlot;

        public static Asset<Texture2D> HoodedTexture;
        public static Asset<Texture2D> PermafrostTexture;
        public static Asset<Texture2D> ShieldTopTexture;
        public static Asset<Texture2D> ShieldBottomTexture;
        public static Asset<Texture2D> ForcefieldTexture;

        // TODO -- This is cumbersome. Change it to be better in 1.4.
        public override void Load()
        {
            string hoodedIconPath = "CalamityMod/NPCs/SupremeCalamitas/HoodedHeadIcon";
            string hoodlessIconPath = "CalamityMod/NPCs/SupremeCalamitas/HoodlessHeadIcon";
            string permafrostIconPath = "CalamityMod/NPCs/TownNPCs/Archmage_Head";

            hoodedHeadIconIndex = CalamityMod.Instance.AddBossHeadTexture(hoodedIconPath, -1);
            hoodlessHeadIconIndex = CalamityMod.Instance.AddBossHeadTexture(hoodlessIconPath, -1);
            permafrostHeadIconIndex = CalamityMod.Instance.AddBossHeadTexture(permafrostIconPath, -1);
        }

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 21;
            NPCID.Sets.TrailingMode[Type] = 1;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                SpriteDirection = 1
            };
            value.Position.Y += 14f;
            NPCID.Sets.NPCBestiaryDrawOffset[Type] = value;
            if (!Main.dedServ)
            {
                HoodedTexture = ModContent.Request<Texture2D>(Texture + "Hooded", AssetRequestMode.AsyncLoad);
                PermafrostTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremePermafrost", AssetRequestMode.AsyncLoad);
                ShieldTopTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeShieldTop", AssetRequestMode.AsyncLoad);
                ShieldBottomTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/SupremeShieldBottom", AssetRequestMode.AsyncLoad);
                ForcefieldTexture = ModContent.Request<Texture2D>("CalamityMod/NPCs/SupremeCalamitas/ForcefieldTexture", AssetRequestMode.AsyncLoad);
            }
        }

        public static int DartDamage = 80; // 320
        public static int SkullDamage = 85; // 340
        public static int HellblastDamage = 95; // 380
        public static int FireblastDamage = 95; // 380
        public static int GigablastDamage = 105; // 420

        public override void SetDefaults()
        {
            NPC.Calamity().canBreakPlayerDefense = true;
            NPC.damage = 210; // 420
            NPC.npcSlots = 50f;
            NPC.width = NPC.height = 44;
            NPC.defense = 100;
            NPC.DR_NERD(normalDR);
            NPC.value = Item.buyPrice(platinum: 3);
            NPC.LifeMaxNERB(750000, 1150000, 900000);
            NPC.aiStyle = -1;
            AIType = -1;
            NPC.knockBackResist = 0f;
            NPC.dontTakeDamage = false;
            NPC.chaseable = true;
            NPC.boss = true;
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
                new MoonLordPortraitBackgroundProviderBestiaryInfoElement(), // Gives black background
                new FlavorTextBestiaryInfoElement("Mods.CalamityMod.Bestiary.SupremeCalamitas")
            });
        }

        public override void BossHeadSlot(ref int index)
        {
            bool inPhase2 = NPC.ai[0] == 3f;
            if (permafrost)
            {
                index = permafrostHeadIconIndex;
            }
            else
            {
                if (!DownedBossSystem.downedCalamitas || BossRushEvent.BossRushActive)
                    index = hoodedHeadIconIndex;
                else
                    index = hoodlessHeadIconIndex;
            }
        }

        public override void ModifyTypeName(ref string typeName)
        {
            if (permafrost)
                typeName = CalamityUtils.GetTextValue("NPCs.SupremePermafrost");
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
            writer.Write(hasSummonedSeekers);
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
            writer.Write(permafrost);
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
            hasSummonedSeekers = reader.ReadBoolean();
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
            permafrost = reader.ReadBoolean();
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

            if (CalamityServerConfig.Instance.BossesStopWeather)
                CalamityWorld.StopRain();

            bool expertMode = Main.expertMode || BossRushEvent.BossRushActive;
            bool revenge = CalamityWorld.revenge || BossRushEvent.BossRushActive;
            bool death = CalamityWorld.death || BossRushEvent.BossRushActive;

            // permafrost and zenith scal are mutually exclusive
            bool zenithAI = Main.zenithWorld && !permafrost;

            // Percent life remaining
            lifeRatio = NPC.life / (float)NPC.lifeMax;

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
                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalSummonText";
                    if (permafrost)
                        key = "Mods.CalamityMod.Status.Boss.PermafrostSummonText";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";
                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
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

            // Shield effect rotation
            rotateToPlayer = rotateToPlayer.AngleLerp((player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation() + MathHelper.PiOver2, 0.04f);
            rotateAwayPlayer = rotateAwayPlayer.AngleLerp((player.Center - NPC.Center).SafeNormalize(Vector2.UnitY).ToRotation() - MathHelper.PiOver2, 0.04f);

            if (hitTimer > 0)
                hitTimer--;

            if (NPC.dontTakeDamage && !hasDoneDeathAnim) // Dust visuals for shield when immune
            {
                Vector2 sustVel = new Vector2(-78 * Main.rand.NextFloat(0.95f, 1.05f), 0).RotatedBy(rotateToPlayer + MathHelper.PiOver2).RotatedByRandom(1.4);
                Dust sust = Dust.NewDustPerfect(NPC.Center + sustVel, DustID.Sandnado, sustVel * Main.rand.NextFloat(0.001f, 0.03f));
                sust.noGravity = true;
                sust.scale = Main.rand.NextFloat(0.5f, 0.9f);
                sust.alpha = 200;
                sust.color = (Main.rand.NextBool() ? Color.Goldenrod : Color.Red);
            }

            Vector2 hitboxSize = new Vector2(forcefieldScale * 216f / 1.4142f);
            hitboxSize = Vector2.Max(hitboxSize, new Vector2(42, 44));
            if (NPC.Size != hitboxSize)
                NPC.Size = hitboxSize;
            bool shouldNotUseShield = bulletHellCounter2 % BulletHellDuration != 0 || attackCastDelay > 0 ||
                (permafrost ? NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) : (NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()) || NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()))) ||
                NPC.ai[0] == 1f || NPC.ai[0] == 2f;

            // Make the shield and forcefield fade away in SCal's acceptance phase.
            if (IsAtHp_Acceptance && hasDoneDeathAnim)
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
                else if (!permafrost)
                {
                    // Emit dust off the skull at the position of its eye socket.
                    for (float num6 = 1f; num6 < 16f; num6 += 1f)
                    {
                        Dust dust = Dust.NewDustPerfect(NPC.Center, DustID.TheDestroyer);
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
            if (!NPC.dontTakeDamage && willCharge && NPC.ai[1] != 2f)
            {
                Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                velOffset *= Main.rand.NextFloat(2, 13);

            }

            #endregion
            #region ArenaCreation

            // Set the variables on the first frame. This does not run client-side.
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
            Vector4 GetArenaSize(bool brothersActive = false)
            {
                var baseSize = death ? new Vector4(1000) : new Vector4(1250);
                if (wormAlive)
                    baseSize *= new Vector4(1, 1.15f, startFourthAttack ? 0.75f : 0.25f, 1.15f);
                if (NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
                {
                    baseSize *= new Vector4(1.5f, 0.85f, 1.5f, 0.85f);
                }
                if (cataclysmAlive || catastropheAlive)
                    baseSize *= new Vector4(0.85f, 1.33f, 0.85f, 1.33f);
                if (IsAtHp_Acceptance)
                    baseSize = (Main.zenithWorld && !permafrost) ? new Vector4(22f, 22f, 22f, 22f) : new Vector4(400f, 500f, 73f, 500f);
                else if (Main.zenithWorld && IsAtHp_BH5 && !wormAlive && !permafrost && FinishedBH5) // gfb, only after Bh5 and sep both end
                    baseSize *= MathHelper.Lerp(0.22f, 1f, lifeRatio * 12.5f); // Scale down the lower health scal has
                return baseSize;
            }

            Color GetArenaColor(out Color oldColor)
            {
                if (permafrost)
                {
                    oldColor = Color.LightBlue;
                    return Color.LightBlue;
                }
                var c = GriefColor;
                oldColor = GriefColor;
                if (startFifthAttack && gettingTired5 && (giveUpCounter < 1160 || hasDoneDeathAnim))
                {
                    c = AcceptanceColor;
                    oldColor = EpiphanyColor;
                }
                else if (IsAtHp_BH4)
                {
                    c = EpiphanyColor;
                    oldColor = LamentColor;
                }
                else if (IsAtHp_BH3)
                    c = LamentColor;
                return c;
            }

            void UpdateArena(ArenaWallSystem.Box box)
            {
                if (Main.dedServ)
                    return;

                var x = 1f / TextureAssets.MagicPixel.Height();
                var p = ScalArenaMetaball.SpawnParticle((box.TopLeft + box.BottomLeft) * 0.5f - new Vector2(box.borderThickness * 0.5f + 2, 0), Vector2.Zero, 1);
                p.SizeScaling = 0;
                p.TextureToUse = TextureAssets.MagicPixel.Value;
                p.Scale = new Vector2(box.borderThickness - 6, box.borderThickness * 2 + box.Size.Y - 4);
                p.Scale.Y *= x;

                p = ScalArenaMetaball.SpawnParticle((box.TopRight + box.BottomRight) * 0.5f + new Vector2(box.borderThickness * 0.5f + 2, 0), Vector2.Zero, 1);
                p.SizeScaling = 0;
                p.TextureToUse = TextureAssets.MagicPixel.Value;
                p.Scale = new Vector2(box.borderThickness - 6, box.borderThickness * 2 + box.Size.Y - 4);
                p.Scale.Y *= x;

                p = ScalArenaMetaball.SpawnParticle((box.TopRight + box.TopLeft) * 0.5f - new Vector2(0, box.borderThickness * 0.5f + 2), Vector2.Zero, 1);
                p.SizeScaling = 0;
                p.TextureToUse = TextureAssets.MagicPixel.Value;
                p.Scale = new Vector2(box.borderThickness * 2 + box.Size.X - 4, box.borderThickness - 6);
                p.Scale.Y *= x;

                p = ScalArenaMetaball.SpawnParticle((box.BottomLeft + box.BottomRight) * 0.5f + new Vector2(0, box.borderThickness * 0.5f + 2), Vector2.Zero, 1);
                p.SizeScaling = 0;
                p.TextureToUse = TextureAssets.MagicPixel.Value;
                p.Scale = new Vector2(box.borderThickness * 2 + box.Size.X - 4, box.borderThickness - 6);
                p.Scale.Y *= x;

                for (var i2 = 0; i2 < box.Size.Y / 100f; i2++)
                {
                    var point = Vector2.Lerp(box.BottomRight, box.TopRight, Main.rand.NextFloat());

                    p = ScalArenaMetaball.SpawnParticle(point + Vector2.UnitX * 8, Vector2.Zero, 16f);
                    p.SizeScaling = 0.95f;

                    point = Vector2.Lerp(box.TopLeft, box.BottomLeft, Main.rand.NextFloat());
                    p = ScalArenaMetaball.SpawnParticle(point + Vector2.UnitX * -8, Vector2.Zero, 16f);
                    p.SizeScaling = 0.95f;

                }
                for (var i2 = 0; i2 < box.Size.X / 100f; i2++)
                {
                    var point = Vector2.Lerp(box.TopLeft, box.TopRight, Main.rand.NextFloat());
                    p = ScalArenaMetaball.SpawnParticle(point + Vector2.UnitY * -8, Vector2.Zero, 16f);
                    p.SizeScaling = 0.95f;
                    point = Vector2.Lerp(box.BottomRight, box.BottomLeft, Main.rand.NextFloat());
                    p = ScalArenaMetaball.SpawnParticle(point + Vector2.UnitY * 8, Vector2.Zero, 16f);
                    p.SizeScaling = 0.95f;
                }
            }

            void DrawArena(ArenaWallSystem.Box box)
            {

                var color = Color.Black * 0.15f;
                //Inside Fill
                box.DrawBoxWithOffset(box.borderThickness * 0.5f, box.borderThickness, Color.Black * 0.15f);
                //Inner Border
                box.DrawBoxWithOffset(4, 2, box.borderColor * 0.15f);
                //Outer Border
                box.DrawBoxWithOffset(box.borderThickness - 2, 2, box.borderColor);
            }

            if (ArenaBox is null)
            {
                ArenaBox = new()
                {
                    position = new Vector2(spawnX + (death ? 1000 : 1250), spawnY + (death ? 1000 : 1250)),
                    boxDimensions = GetArenaSize() * 2,
                    borderThickness = 2000,
                    RemovalCondition = () => !(Main.npc[NPC.whoAmI].active) || Main.npc[NPC.whoAmI].type != Type,
                    UpdateBox = UpdateArena,
                    DrawBox = DrawArena,
                    DespawnAction = (box) =>
                    {
                        box.borderThickness *= 0.9f;
                        if (box.borderThickness < 4)
                            return true;
                        UpdateArena(box);
                        return false;
                    }
                };
                ArenaWallSystem.ActiveBoxes.Add(ArenaBox);
            }
            ArenaBox.NewDimensions = Vector4.Lerp(ArenaBox.boxDimensions, GetArenaSize(), IsAtHp_Acceptance ? 0.02f : startFourthAttack ? 0.05f : 0.1f);
            var color = GetArenaColor(out Color oldColor);
            if (colorCompletion > 1.1f && color != ArenaBox.borderColor)
                colorCompletion = 0;
            if (colorCompletion < 1)
                ArenaBox.borderColor = Color.Lerp(oldColor, color, colorCompletion);
            else
                ArenaBox.borderColor = color;
            colorCompletion += 0.003f;

            #endregion
            #region Enrage and DR
            if (ArenaWallSystem.ActiveBoxes.Count > 0 && !Collision.CheckAABBvAABBCollision(ArenaBox.TopLeft, ArenaBox.Size, player.position, player.Size))
            {
                float projectileVelocityMultCap = 2f;
                uDieLul = MathHelper.Clamp(uDieLul * 1.01f, 1f, projectileVelocityMultCap);
                protectionBoost = true;
                NPC.Calamity().CurrentlyEnraged = true;
            }
            else
            {
                uDieLul = MathHelper.Clamp(uDieLul * 0.99f, 1f, 2f);
                protectionBoost = false;
                NPC.Calamity().CurrentlyEnraged = false;
            }

            // Permafrost fucks mounts if you exit his arena.
            if (permafrost)
            {
                if (NPC.Calamity().CurrentlyEnraged && player.mount.Active)
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
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var rumbleSound) && rumbleSound.IsPlaying)
                        rumbleSound.Stop();

                    canDespawn = true;
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
                                projectile.ai[2] = 1f;

                                if (projectile.timeLeft > 15)
                                    projectile.timeLeft = 15;
                            }
                        }
                    }

                    NPC.Opacity = MathHelper.Lerp(NPC.Opacity, 0f, 0.065f);
                    NPC.velocity = Vector2.Lerp(Vector2.UnitY * -4f, Vector2.Zero, (float)Math.Sin(MathHelper.Pi * NPC.Opacity));
                    forcefieldOpacity = Utils.GetLerpValue(0.1f, 0.6f, NPC.Opacity, true);
                    if (NPC.alpha >= 230)
                    {
                        if (DownedBossSystem.downedCalamitas && !BossRushEvent.BossRushActive)
                        {
                            // Create a teleport line effect
                            Dust.QuickDustLine(NPC.Center, initialRitualPosition, 500f, permafrost ? Color.Cyan : Color.Red);
                            NPC.Center = initialRitualPosition;

                            // Make the town NPC spawn.
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 12, permafrost ? ModContent.NPCType<Archmage>() : ModContent.NPCType<BrimstoneWitch>());
                        }

                        NPC.active = false;
                        NPC.netUpdate = true;
                    }

                    for (int i = 0; i < MathHelper.Lerp(2f, 6f, 1f - NPC.Opacity); i++)
                    {
                        Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                        brimstoneFire.color = permafrost ? Color.Cyan : Color.Red;
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
            if (!FinishedBH1)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;

                #region BulletHellEndTelegraphBH1
                if (bulletHellCounter2 == (BulletHellDuration - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(BulletHellSound, player.Center);
                if (bulletHellCounter2 > (BulletHellDuration - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                #endregion

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
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        if (bulletHellCounter2 < 300 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 600) // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from above, left, and right
                        {
                            if (!Main.zenithWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 3f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }
                FrameType = FrameAnimationType.Casting;
                return;
            }
            else if (!startBattle)
            {
                attackCastDelay = sepulcherSpawnCastTime;

                Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                GeneralParticleHandler.SpawnParticle(pulse2);
                for (int i = 0; i < 100; i++)
                {
                    Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                    Dust failShotDust = Dust.NewDustPerfect(NPC.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                    failShotDust.noGravity = true;
                    failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                    failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                }

                SoundEngine.PlaySound(BulletHellEndSound, NPC.Center);

                SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal, player.Center);
                startBattle = true;
            }
            #endregion
            #region SecondAttack
            if (!FinishedBH2 && startSecondAttack) // Bullet hell lasts 1800
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;
                //At the start of the bullet hell, teleport Scal to the center of the arena
                if (!teleport)
                {
                    Vector2 goalPos = new Vector2(spawnX + (death ? 1000 : 1250), spawnY + (death ? 1000 : 1250)); // player.Center + new Vector2(0, -175)
                    Dust.QuickDustLine(NPC.Center, goalPos + new Vector2(0, -20), 500f, permafrost ? Color.Cyan : Color.Red);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = goalPos;
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    teleport = true;
                }
                #region BulletHellEndTelegraphBH2
                if (bulletHellCounter2 == (SecondBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(BulletHellSound, player.Center);
                if (bulletHellCounter2 > (SecondBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == SecondBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(NPC.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(BulletHellEndSound, NPC.Center);
                    //Reset the teleport bool for the next bullet hell
                    teleport = false;
                }
                #endregion

                if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Permafrost throws delicious meat that explode into more meat which doesn't split
                    if (permafrost)
                    {
                        if (bulletHellCounter2 % 90 == 0)
                        {
                            float bottleSpeed = 12f;
                            Vector2 bottleVelocity = (player.Center + player.velocity * 20f - NPC.Center).SafeNormalize(Vector2.UnitY) * bottleSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, bottleVelocity * uDieLul, ModContent.ProjectileType<PermafrostMeat>(), 350, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }

                    if (bulletHellCounter2 < 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from top
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }
                    else if (bulletHellCounter2 < 1500 && bulletHellCounter2 > 1200)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from right
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -5f * uDieLul, 0f, fireblast, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }
                    else if (bulletHellCounter2 > 1500)
                    {
                        if (bulletHellCounter2 % 180 == 0) // Blasts from top
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 1)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 1) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 1200 && !Main.zenithWorld) // Blasts from below
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y + 1000f, 0f, -4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 1500) // Blasts from left
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startSecondAttack && IsAtHp_BH2)
            {
                // Bouncy Boulders
                if (permafrost)
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

                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBH2Text";
                    if (permafrost)
                        key = "Mods.CalamityMod.Status.Boss.PermafrostBH2Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                }

                startSecondAttack = true;
                return;
            }
            #endregion
            #region ThirdAttack
            if (!FinishedBH3 && startThirdAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;
                //At the start of the bullet hell, teleport Scal to the center of the arena
                if (!teleport)
                {
                    Vector2 goalPos = new Vector2(spawnX + (death ? 1000 : 1250), spawnY + (death ? 1000 : 1250)); // player.Center + new Vector2(0, -175)
                    Dust.QuickDustLine(NPC.Center, goalPos + new Vector2(0, -20), 500f, permafrost ? Color.Cyan : Color.Red);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = goalPos;
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    teleport = true;
                }
                #region BulletHellEndTelegraphBH3
                if (bulletHellCounter2 == (ThirdBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(BulletHellSound, player.Center);
                if (bulletHellCounter2 > (ThirdBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == ThirdBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(NPC.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(BulletHellEndSound, NPC.Center);
                    //Reset the teleport bool for the next bullet hell
                    teleport = false;
                }
                #endregion

                if (permafrost)
                {
                    Vector2 destination = player.Center;
                    Vector2 distanceFromDestination = destination - NPC.Center;
                    Vector2 desiredVelocity = (distanceFromDestination - NPC.velocity).SafeNormalize(Vector2.UnitY) * PermafrostPhotonRipperDashVelocity;

                    if (Vector2.Distance(NPC.Center, destination) > PermafrostPhotonRipperMinDistanceFromTarget)
                        NPC.SimpleFlyMovement(desiredVelocity * uDieLul, PermafrostPhotonRipperDashAcceleration * uDieLul);
                    else
                        NPC.velocity *= 0.9f;
                }
                else if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Permafrost uses Absolute Zero which acts like Photon Ripper
                    if (bulletHellCounter2 == SecondBulletHellEndValue + 1 && permafrost)
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One, ModContent.ProjectileType<PermafrostAbsoluteZeroProjectile>(), PermafrostAbsoluteZeroDamage, 0f, Main.myPlayer, 0f, 0f, NPC.whoAmI);

                    if (bulletHellCounter2 % 180 == 0) // Blasts from top
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 240 == 0) // Fireblasts from above
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 4)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 4) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 2100 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 2400) // Blasts from right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startThirdAttack && IsAtHp_BH3)
            {
                // Bouncy Boulders
                if (permafrost)
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

                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBH3Text";
                    if (permafrost)
                        key = "Mods.CalamityMod.Status.Boss.PermafrostBH3Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                }

                startThirdAttack = true;
                return;
            }
            #endregion
            #region FourthAttack
            if (IsAtHp_BH4 && musicSyncCounter > -120)
                musicSyncCounter--;
            if (musicSyncCounter <= 126 && musicSyncCounter > 0)
            {
                forcefieldOpacity = MathHelper.Lerp(forcefieldOpacity, 0.4f, 0.06f);

                if (shieldOpacity > 0f)
                    shieldOpacity = MathHelper.Lerp(shieldOpacity, 0f, 0.065f);
            }
            if (musicSyncCounter <= 42 && musicSyncCounter > 0)
            {
                forcefieldPureVisualScale = MathHelper.Lerp(forcefieldPureVisualScale, 0.45f, 0.095f);
            }
            if (musicSyncCounter == 0 && !postMusicHit)
            {
                Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Red, new Vector2(1f, 1f), 0, 0.1f, 4f, 17);
                GeneralParticleHandler.SpawnParticle(pulse);
                Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 3f, 19);
                GeneralParticleHandler.SpawnParticle(pulse2);
                for (int i = 0; i < 30; i++)
                {
                    Vector2 orbvel = new Vector2(25, 25).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.2f);
                    Particle orb = new SparkParticle(NPC.Center + orbvel, orbvel, false, 90, Main.rand.NextFloat(0.95f, 1.85f), Color.Lerp(Color.Red, Color.Magenta, 0.3f));
                    GeneralParticleHandler.SpawnParticle(orb);
                }
                forcefieldOpacity = 0.7f;
                forcefieldPureVisualScale = 1.5f;
                postMusicHit = true;
            }
            if (postMusicHit && !IsAtHp_Acceptance && !canDespawn && NPC.ai[1] != 2f)
            {
                forcefieldPureVisualScale = MathHelper.Lerp(forcefieldPureVisualScale, 1f, 0.095f);
                Vector2 velOffset = new Vector2(56, 56).RotatedByRandom(100) * forcefieldPureVisualScale;
                for (int i = 0; i < 2; i++)
                {
                    Dust fust = Dust.NewDustPerfect(NPC.Center + velOffset, Main.rand.NextBool(3) ? 60 : 114);
                    fust.noGravity = true;
                    fust.velocity = velOffset * Main.rand.NextFloat(0.05f, 0.2f);
                    fust.scale = Main.rand.NextFloat(0.3f, 0.8f);
                    if (Main.rand.NextBool())
                    {
                        Dust sust = Dust.NewDustPerfect(NPC.Center + velOffset, DustID.Sandnado, velOffset * Main.rand.NextFloat(0.01f, 0.1f));
                        sust.noGravity = true;
                        sust.scale = Main.rand.NextFloat(0.2f, 0.6f);
                        sust.alpha = 200;
                        sust.color = (Main.rand.NextBool() ? Color.Goldenrod : Color.Red);
                    }
                }

            }

            if (!FinishedBH4 && startFourthAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;
                //At the start of the bullet hell, teleport Scal to the center of the arena
                if (!teleport)
                {
                    Vector2 goalPos = new Vector2(spawnX + (death ? 1000 : 1250), spawnY + (death ? 1000 : 1250)); // player.Center + new Vector2(0, -175)
                    Dust.QuickDustLine(NPC.Center, goalPos + new Vector2(0, -20), 500f, permafrost ? Color.Cyan : Color.Red);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = goalPos;
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    teleport = true;
                }

                #region BulletHellEndTelegraphBH4
                if (bulletHellCounter2 == (FourthBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(BulletHellSound, player.Center);
                if (bulletHellCounter2 > (FourthBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == FourthBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(NPC.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(BulletHellEndSound, NPC.Center);
                    //Reset the teleport bool for the next bullet hell
                    teleport = false;
                }
                #endregion

                if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient) // More clustered attack
                {
                    // Permafrost throws Delicious Meat that explodes into more meat that splits
                    if (permafrost)
                    {
                        if (bulletHellCounter2 % 90 == 0)
                        {
                            float bottleSpeed = 12f;
                            Vector2 bottleVelocity = (player.Center + player.velocity * 20f - NPC.Center).SafeNormalize(Vector2.UnitY) * bottleSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, bottleVelocity * uDieLul, ModContent.ProjectileType<PermafrostMeat>(), 125, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }

                    if (bulletHellCounter2 % 180 == 0) // Blasts from top
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 240 == 0) // Fireblasts from above
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);

                    int divisor = revenge ? 225 : expertMode ? 450 : 675;

                    Vector2 spawnSpot = safeBox.Center();
                    passedVar += 1f;
                    if (passedVar == 180) // Giant homing fireballs
                    {
                        SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/Custom/SCalAltarSummon") with { Pitch = 0.3f }, player.Center);
                        for (int i = 0; i < 2; i++)
                        {
                            Particle bloom = new BloomParticle(spawnSpot, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.3f), 0f, 1.45f, 240, false);
                            GeneralParticleHandler.SpawnParticle(bloom);
                        }
                        Particle bloom2 = new BloomParticle(spawnSpot, Vector2.Zero, Color.White, 0f, 1.35f, 240, false);
                        GeneralParticleHandler.SpawnParticle(bloom2);
                    }
                    if (passedVar == 420) // Giant homing fireballs
                    {
                        for (int i = 0; i < 90; i++)
                        {
                            Dust spawnDust = Dust.NewDustPerfect(safeBox.Center(), permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone, new Vector2(30, 30).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 1.2f));
                            spawnDust.noGravity = true;
                            spawnDust.scale = Main.rand.NextFloat(1.2f, 2.3f);
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            Vector2 sparkVel = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.1f);
                            GlowOrbParticle orb = new GlowOrbParticle(safeBox.Center() + sparkVel * 2, sparkVel, false, 120, Main.rand.NextFloat(1.55f, 2.75f), permafrost ? Color.DarkCyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), true, true);
                            GeneralParticleHandler.SpawnParticle(orb);
                        }

                        // Whispering Maelstrom passes through normal CanHitPlayer hooks then overrides them. This needs it to have contact damage but the number itself doesn't matter.
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), spawnSpot, Vector2.Zero, ModContent.ProjectileType<BrimstoneMonster>(), 100, 0f, Main.myPlayer, 0f);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 6)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 6) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 3000 && !Main.zenithWorld) // Blasts from below
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y + 1000f, 0f, -4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 3300) // Blasts from left
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startFourthAttack && IsAtHp_BH4)
            {
                // Bouncy Boulders
                if (permafrost)
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

                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBH4Text";
                    if (permafrost)
                        key = "Mods.CalamityMod.Status.Boss.PermafrostBH4Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                }

                startFourthAttack = true;
                return;
            }
            #endregion
            #region FifthAttack
            if (!FinishedBH5 && startFifthAttack)
            {
                despawnProj = true;
                bulletHellCounter2++;
                NPC.damage = 0;
                NPC.chaseable = false;
                NPC.dontTakeDamage = true;
                //At the start of the bullet hell, teleport Scal to the center of the arena
                if (!teleport)
                {
                    Vector2 goalPos = new Vector2(spawnX + (death ? 1000 : 1250), spawnY + (death ? 1000 : 1250)); // player.Center + new Vector2(0, -175)
                    Dust.QuickDustLine(NPC.Center, goalPos + new Vector2(0, -20), 500f, permafrost ? Color.Cyan : Color.Red);
                    NPC.velocity = Vector2.Zero;
                    NPC.Center = goalPos;
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    teleport = true;
                }
                #region BulletHellEndTelegraphBH5
                if (bulletHellCounter2 == (FifthBulletHellEndValue - 360))
                    BulletHellRumbleSlot = SoundEngine.PlaySound(BulletHellSound, player.Center);
                if (bulletHellCounter2 > (FifthBulletHellEndValue - 360))
                {
                    if (SoundEngine.TryGetActiveSound(BulletHellRumbleSlot, out var BHSound) && BHSound.IsPlaying)
                    {
                        BHSound.Position = player.MountedCenter;
                    }
                }
                if (bulletHellCounter2 == FifthBulletHellEndValue)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        Vector2 dustVel = new Vector2(15, 15).RotatedByRandom(100);
                        Dust failShotDust = Dust.NewDustPerfect(NPC.Center + dustVel * 3, Main.rand.NextBool(3) ? 60 : 114);
                        failShotDust.noGravity = true;
                        failShotDust.velocity = dustVel * Main.rand.NextFloat(0.3f, 1.3f);
                        failShotDust.scale = Main.rand.NextFloat(2f, 3.2f);
                    }
                    Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Red, new Vector2(1f, 1f), 0, 0.1f, 5f, 15);
                    GeneralParticleHandler.SpawnParticle(pulse);
                    Particle pulse2 = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.Cyan : Color.Lerp(Color.Red, Color.Magenta, 0.3f), new Vector2(1f, 1f), 0, 0.05f, 4f, 18);
                    GeneralParticleHandler.SpawnParticle(pulse2);

                    SoundEngine.PlaySound(BulletHellEndSound, NPC.Center);
                    //Reset the teleport bool for the next bullet hell
                    //Technically not needed but I wanted to be safe
                    teleport = false;
                }
                #endregion

                if (permafrost)
                {
                    Vector2 destination = player.Center;
                    Vector2 distanceFromDestination = destination - NPC.Center;
                    Vector2 desiredVelocity = (distanceFromDestination - NPC.velocity).SafeNormalize(Vector2.UnitY) * PermafrostPhotonRipperDashVelocity;

                    if (Vector2.Distance(NPC.Center, destination) > PermafrostPhotonRipperMinDistanceFromTarget)
                        NPC.SimpleFlyMovement(desiredVelocity * uDieLul, PermafrostPhotonRipperDashAcceleration * uDieLul);
                    else
                        NPC.velocity *= 0.9f;
                }
                else if (!canDespawn)
                    NPC.velocity *= 0.95f;

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    // Permafrost throws delicious meat that explode into more delicious meat that doesn't split
                    if (permafrost)
                    {
                        // Permafrost uses Absolute Zero similarly to Photon Ripper
                        if (bulletHellCounter2 == FourthBulletHellEndValue + 1)
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.One, ModContent.ProjectileType<PermafrostAbsoluteZeroProjectile>(), PermafrostAbsoluteZeroDamage, 0f, Main.myPlayer, 0f, 0f, NPC.whoAmI);

                        if (bulletHellCounter2 % 90 == 0)
                        {
                            float bottleSpeed = 12f;
                            Vector2 bottleVelocity = (player.Center + player.velocity * 20f - NPC.Center).SafeNormalize(Vector2.UnitY) * bottleSpeed;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, bottleVelocity * uDieLul, ModContent.ProjectileType<PermafrostMeat>(), 125, 0f, Main.myPlayer);
                        }
                    }

                    if (bulletHellCounter2 % 240 == 0) // Blasts from top
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 5f * uDieLul, fireblast, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 360 == 0) // Fireblasts from above
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 10f * uDieLul, gigablast, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);

                    if (bulletHellCounter2 % 30 == 0) // Projectiles that move in wave pattern
                    {
                        int random = Main.rand.Next(-500, 501);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + random, -5f * uDieLul, 0f, wave, SkullDamage, 0f, Main.myPlayer, 0f, 2f);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y - random, 5f * uDieLul, 0f, wave, SkullDamage, 0f, Main.myPlayer, 0f, 2f);
                    }

                    bulletHellCounter++;
                    if (bulletHellCounter >= baseBulletHellProjectileGateValue + 8)
                    {
                        bulletHellCounter = 0;
                        if (bulletHellCounter2 % ((baseBulletHellProjectileGateValue + 8) * 6) == 0)
                        {
                            float distance = Main.rand.NextBool() ? -1000f : 1000f;
                            float velocity = (distance == -1000f ? 4f : -4f) * uDieLul;
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + distance, player.position.Y, velocity, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }

                        if (bulletHellCounter2 < 3900 && !Main.zenithWorld) // Blasts from above
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 4f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else if (bulletHellCounter2 < 4200) // Blasts from left and right
                        {
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3.5f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                        else // Blasts from above, left, and right
                        {
                            if (!Main.zenithWorld)
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + Main.rand.Next(-1000, 1001), player.position.Y - 1000f, 0f, 3f * uDieLul, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);

                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X + 1000f, player.position.Y + Main.rand.Next(-1000, 1001), -3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                            Projectile.NewProjectile(NPC.GetSource_FromAI(), player.position.X - 1000f, player.position.Y + Main.rand.Next(-1000, 1001), 3f * uDieLul, 0f, bulletHellblast, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                        }
                    }
                }

                FrameType = FrameAnimationType.Casting;
                return;
            }

            if (!startFifthAttack && IsAtHp_BH5)
            {
                // Bouncy Boulders
                if (permafrost)
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
                if (permafrost)
                    key = "Mods.CalamityMod.Status.Boss.PermafrostBH5Text";

                if (!BossRushEvent.BossRushActive)
                {
                    if (DownedBossSystem.downedCalamitas && !permafrost)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
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
                    if (permafrost)
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
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), circleOffset, player.Center, ModContent.ProjectileType<PermafrostBlaster>(), 350, 0f, Main.myPlayer, 0f, 1f);
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
                                        int proj = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (rayVelocity).SafeNormalize(Vector2.UnitY) * 16f, rayVelocity, ModContent.ProjectileType<Projectiles.Melee.DarkIceZero>(), 250, 0f, Main.myPlayer);
                                        if (proj.WithinBounds(Main.maxProjectiles))
                                        {
                                            Main.projectile[proj].DamageType = DamageClass.Default;
                                            Main.projectile[proj].friendly = false;
                                            Main.projectile[proj].hostile = true;
                                            Main.projectile[proj].tileCollide = false;
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
                                CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.PermafrostGiveUpText", permafrostTextColor);
                                Dust.QuickDustLine(NPC.Center, initialRitualPosition, 500f, Color.Cyan);
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

                                NPC.NPCLoot(); int cryo = NPC.FindFirstNPC(ModContent.NPCType<Cryogen.Cryogen>());
                                if (cryo > -1)
                                {
                                    Main.npc[cryo].active = false;
                                    Main.npc[cryo].netUpdate = true;
                                }
                            }

                            return;
                        }
                    }
                    else // Scal gives up
                    {
                        // Clear Debuffs DOESNT WORK YET
                        for (int l = 0; l < NPC.maxBuffs; ++l)
                        {
                            int buffID = NPC.buffType[l];

                            bool shouldHalveDuration = CalamityBuffSets.IsDebuff[buffID];

                            if (shouldHalveDuration && NPC.buffTime[l] > 4)
                                NPC.buffTime[l] = 4;
                        }


                        if (!hasDoneDeathAnim && !BossRushEvent.BossRushActive) // Scrapped death animation for Scal
                        {
                            attackPause = 5;
                            Dust.QuickDustLine(NPC.Center, safeBox.Center() + new Vector2(0, -30), 500f, permafrost ? Color.Cyan : Color.Red);
                            NPC.Center = safeBox.Center() + new Vector2(0, -30);
                            NPC.velocity = new Vector2(10 * NPC.spriteDirection, -7);
                            hasDoneDeathAnim = true;
                            NPC.noTileCollide = false;
                            NPC.noGravity = false;
                            NPC.damage = 0;

                            /*
                            // Teleport back to the arena on defeat
                            if (giveUpCounter == GiveUpCounterMax)
                            {
                                Dust.QuickDustLine(NPC.Center, initialRitualPosition + new Vector2(0, -30), 500f, permafrost ? Color.Cyan : Color.Red);
                                NPC.Center = initialRitualPosition + new Vector2(0, -30);
                                NPC.noTileCollide = false;
                                NPC.noGravity = false;
                                NPC.damage = 0;
                                SoundEngine.PlaySound(GiveUpSound with { Volume = 1.7f }, NPC.Center);
                            }
                            if (giveUpCounter == 1010)
                            {
                                Particle pulse2 = new StaticPulseRing(NPC.Center, Vector2.Zero, Color.Red * 0.55f, new Vector2(1f, 1f), 0f, 5f, 0f, 47);
                                GeneralParticleHandler.SpawnParticle(pulse2);
                            }

                            if (shieldOpacity > 0f)
                                shieldOpacity = 0;
                            if (giveUpCounter > 1000)
                                forcefieldScale = MathHelper.Lerp(forcefieldScale, 1.55f, 0.045f);
                            else if (giveUpCounter > 960)
                                forcefieldScale = MathHelper.Lerp(forcefieldScale, 0.4f, 0.085f);

                            if (giveUpCounter > 960)
                            {
                                for (int i = 0; i < 2; i++)
                                {
                                    Vector2 sparkVel = new Vector2(15, 15).RotatedByRandom(100) * Main.rand.NextFloat(0.2f, 1.1f);
                                    GlowOrbParticle orb = new GlowOrbParticle(NPC.Center + sparkVel * (8 * forcefieldScale), sparkVel * Utils.GetLerpValue(1200, 960, giveUpCounter, true), false, 60, Main.rand.NextFloat(0.7f, 1.7f) * Utils.GetLerpValue(1200, 960, giveUpCounter), Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f), true, true);
                                    GeneralParticleHandler.SpawnParticle(orb);
                                }

                                float vibrate = 15 * Utils.GetLerpValue(1200, 960, giveUpCounter, true);
                                NPC.Center = (initialRitualPosition + new Vector2(0, -30)) + Main.rand.NextVector2Circular(vibrate, vibrate);
                            }
                            else
                            {
                                if (giveUpCounter == 960)
                                {
                                    NPC.Center = initialRitualPosition + new Vector2(0, -30);

                                    Vector2 BurstFXDirection = new Vector2(0, 15);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        SparkParticle spark = new SparkParticle(NPC.Center, (BurstFXDirection) * (i + 1), false, 11, (5f - i * 0.6f) * 3, Color.Lerp(Color.Red, Color.Magenta, Utils.GetLerpValue(-3, 3, i)) * 0.8f);
                                        GeneralParticleHandler.SpawnParticle(spark);
                                    }
                                    for (int i = 0; i < 4; i++)
                                    {
                                        SparkParticle spark = new SparkParticle(NPC.Center, (-BurstFXDirection) * (i + 1), false, 11, (5f - i * 0.6f) * 3, Color.Lerp(Color.Red, Color.Magenta, Utils.GetLerpValue(-3, 3, i)) * 0.8f);
                                        GeneralParticleHandler.SpawnParticle(spark);
                                    }

                                    Particle orb = new CustomPulse(NPC.Center, Vector2.Zero, Color.Red, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1.2f, 1.1f, 13);
                                    GeneralParticleHandler.SpawnParticle(orb);
                                    Particle orb2 = new CustomPulse(NPC.Center, Vector2.Zero, Color.Lerp(Color.Red, Color.Magenta, 0.5f), "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 1f, 0.9f, 13);
                                    GeneralParticleHandler.SpawnParticle(orb2);
                                    for (int i = 0; i < 4; i++)
                                    {
                                        Particle orb3 = new CustomPulse(NPC.Center, Vector2.Zero, Color.White, "CalamityMod/Particles/LargeBloom", new Vector2(1, 1), Main.rand.NextFloat(-10, 10), 0.9f, 0.8f, 13);
                                        GeneralParticleHandler.SpawnParticle(orb3);
                                    }

                                    for (int i = 0; i < 30; i++)
                                    {
                                        GlowOrbParticle burst = new GlowOrbParticle(NPC.Center, (new Vector2(30, 30).RotatedByRandom(100) + new Vector2(0, -2)) * Main.rand.NextFloat(0.1f, 1.2f), true, 120, Main.rand.NextFloat(1.7f, 2.7f), Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f), true, true);
                                        GeneralParticleHandler.SpawnParticle(burst);
                                    }

                                    NPC.velocity = new Vector2(18 * NPC.spriteDirection, -12);
                                    giveUpCounter = GiveUpCounterMax;
                                    hasDoneDeathAnim = true;
                                }
                            }
                            */
                        }
                        else
                        {
                            if (BossRushEvent.BossRushActive)
                                NPC.Center = initialRitualPosition + new Vector2(0, -30);

                            if (NPC.velocity.Y < 8f)
                                NPC.velocity.Y += 0.165f;
                            if (!canDespawn)
                                NPC.velocity.X *= 0.965f;

                            if (DownedBossSystem.downedCalamitas || BossRushEvent.BossRushActive)
                            {
                                if (giveUpCounter == 720)
                                {
                                    if (BossRushEvent.BossRushActive)
                                    {
                                        NPC.chaseable = true;
                                        NPC.dontTakeDamage = false;
                                        return;
                                    }

                                    for (int i = 0; i < 24; i++)
                                    {
                                        Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                                        brimstoneFire.color = permafrost ? Color.Cyan : Color.Red;
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
                            else if (giveUpCounter == 900 && !BossRushEvent.BossRushActive)
                            {
                                CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.SCalAcceptanceText1", textColor);
                            }
                            else if (giveUpCounter == 600 && !BossRushEvent.BossRushActive)
                            {
                                CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.SCalAcceptanceText2", textColor);
                            }
                            else if (giveUpCounter == 300 && !BossRushEvent.BossRushActive)
                            {
                                CalamityUtils.BroadcastLocalizedText("Mods.CalamityMod.Status.Boss.SCalAcceptanceText3", textColor);
                            }

                            if (giveUpCounter <= 0)
                            {
                                for (int i = 0; i < 24; i++)
                                {
                                    Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), DustID.Torch);
                                    brimstoneFire.color = permafrost ? Color.Cyan : Color.Red;
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
                    }

                    giveUpCounter--;
                    NPC.chaseable = false;
                    NPC.dontTakeDamage = true;
                    return;
                }

                if (!gettingTired5 && IsAtHp_Acceptance)
                {
                    for (int x = 0; x < Main.maxProjectiles; x++)
                    {
                        Projectile projectile = Main.projectile[x];
                        if (projectile.active)
                        {
                            if (projectile.type == ModContent.ProjectileType<BrimstoneMonster>())
                            {
                                if (projectile.timeLeft > 90)
                                    projectile.timeLeft = 90;
                            }
                            if (projectile.type == bulletHellblast ||
                                projectile.type == barrage ||
                                projectile.type == wave)
                            {
                                if (projectile.timeLeft > 60)
                                    projectile.timeLeft = 60;
                            }
                            else if (projectile.type == fireblast || projectile.type == gigablast)
                            {
                                projectile.ai[2] = 1f;

                                if (projectile.timeLeft > 15)
                                    projectile.timeLeft = 15;
                            }
                        }
                    }

                    if (!BossRushEvent.BossRushActive)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText4";
                        if (permafrost)
                            key = "Mods.CalamityMod.Status.Boss.PermafrostBruhText";
                        else if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                    }

                    gettingTired5 = true;
                    return;
                }
                else if (!gettingTired4 && lifeRatio <= 0.02f)
                {
                    if (!BossRushEvent.BossRushActive && !permafrost)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText3";
                        if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.BroadcastLocalizedText(key, textColor);
                    }

                    gettingTired4 = true;
                    return;
                }
                else if (!gettingTired3 && lifeRatio <= 0.04f)
                {
                    if (!BossRushEvent.BossRushActive && !permafrost)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText2";
                        if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.BroadcastLocalizedText(key, textColor);
                    }

                    gettingTired3 = true;
                    return;
                }
                else if (!gettingTired2 && lifeRatio <= 0.06f)
                {
                    if (!BossRushEvent.BossRushActive)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalDesparationText1";
                        if (permafrost)
                            key = "Mods.CalamityMod.Status.Boss.PermafrostNonchalantText";
                        else if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                    }

                    gettingTired2 = true;
                    return;
                }
                else if (!gettingTired && IsAtSep2)
                {
                    attackCastDelay = sepulcherSpawnCastTime;
                    for (int i = 0; i < 40; i++)
                    {
                        Dust castFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-70f, 70f), permafrost ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
                        castFire.velocity = Vector2.UnitY.RotatedByRandom(0.08f) * -Main.rand.NextFloat(3f, 4.45f);
                        castFire.scale = Main.rand.NextFloat(1.35f, 1.6f);
                        castFire.fadeIn = 1.25f;
                        castFire.noGravity = true;
                    }

                    for (int i = 0; i < 40; i++)
                    {
                        Dust castFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-70f, 70f), permafrost ? (int)CalamityDusts.PurpleCosmilite : (int)CalamityDusts.Brimstone);
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
                            projectile.ai[2] = 1f;

                            if (projectile.timeLeft > 15)
                                projectile.timeLeft = 15;
                        }
                    }
                }
                despawnProj = false;
            }
            #endregion
            #region TransformSeekerandBrotherTriggers
            if (!halfLife && IsAtBrothers && hasSummonedBrothers && (permafrost ? NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) : (NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()) || NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()))) == false)
            {
                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalPhase2Text";
                    if (permafrost)
                        key = "Mods.CalamityMod.Status.Boss.PermafrostPhase2Text";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                }

                halfLife = true;
            }

            if (IsAtSeekers)
            {
                if (!hasSummonedSeekers)
                {
                    if (!BossRushEvent.BossRushActive)
                    {
                        string key = "Mods.CalamityMod.Status.Boss.SCalSeekerRingText";
                        if (permafrost)
                            key = "Mods.CalamityMod.Status.Boss.PermafrostHallowBossSpamText";
                        else if (DownedBossSystem.downedCalamitas)
                            key += "Rematch";

                        CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                    }

                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (permafrost)
                        {
                            // Spawn a buffed Pyrogen
                            int npc = NPC.NewNPC(NPC.GetSource_FromAI(),
                                (int)(vectorCenter.X),
                                (int)(vectorCenter.Y),
                                ModContent.NPCType<Cryogen.Cryogen>(), NPC.whoAmI);
                            Main.npc[npc].timeLeft *= 20;
                            Main.npc[npc].lifeMax = Main.npc[npc].life *= 22;
                            CalamityUtils.BossAwakenMessage(npc);
                        }
                        else
                        {
                            SoundEngine.PlaySound(SoundID.Item74, NPC.Center);
                            int totalSeekers = Main.getGoodWorld ? 20 : 10;
                            int degreesBetweenEachSeeker = 360 / totalSeekers;
                            int distanceFromSCal = Main.getGoodWorld ? 300 : 225;
                            for (int i = 0; i < totalSeekers; i++)
                            {
                                int FireEye = NPC.NewNPC(NPC.GetSource_FromAI(), (int)(vectorCenter.X + (Math.Sin(i * degreesBetweenEachSeeker) * distanceFromSCal)), (int)(vectorCenter.Y + (Math.Cos(i * degreesBetweenEachSeeker) * distanceFromSCal)), ModContent.NPCType<SoulSeekerSupreme>(), NPC.whoAmI, 0, 0, 0, -1);
                                NPC Eye = Main.npc[FireEye];
                                Eye.ai[0] = i * degreesBetweenEachSeeker;
                                Eye.ai[3] = i * degreesBetweenEachSeeker;
                            }
                        }
                    }

                    SoundEngine.PlaySound(SoundID.DD2_DarkMageHealImpact, player.Center);
                    hasSummonedSeekers = true;
                }
            }

            if (bossLife == 0f && NPC.life > 0)
            {
                bossLife = NPC.lifeMax;
            }
            if (NPC.life > 0)
            {
                if (attackPause > 0)
                    attackPause--;

                if (IsAtBrothers && !enteredBrothersPhase)
                {
                    attackPause = 5;
                    if (preventionPause == 0)
                    {
                        enteredBrothersPhase = true;
                        attackCastDelay = brothersSpawnCastTime;
                        NPC.netUpdate = true;
                    }
                    else
                    {
                        preventionPause--;
                        NPC.dontTakeDamage = true;
                        NPC.velocity *= 0.85f;
                        return;
                    }

                }
            }

            #endregion
            #region FirstStage
            if (NPC.ai[0] == 0f)
            {
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
                        attackPause = 5;

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
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        // How fast SCal moves to the destination
                        float velocity = 12f;
                        float acceleration = 0.12f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

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
                            fireFireblastFirst = true;
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

                            if (randomShot == 0 && canFireSplitingFireball && !fireFireblastFirst)
                            {
                                canFireSplitingFireball = false;
                                randomShot = gigablast;
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);

                                for (int i = 0; i < 9; i++) // fireblasts pre laugh
                                {
                                    Vector2 velOffset = NPC.DirectionTo(player.Center).RotatedByRandom(0.6) * Main.rand.NextFloat(5f, 13f);
                                    GlowOrbParticle spark2 = new GlowOrbParticle(projectileSpawn + velOffset * 2f, velOffset * 0.7f, false, 30, Main.rand.NextFloat(0.4f, 0.65f), permafrost ? Color.LightBlue : Main.rand.NextBool(3) ? Color.Lerp(Color.Red, Color.Magenta, 0.3f) : Color.Red);
                                    GeneralParticleHandler.SpawnParticle(spark2);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient && attackPause == 0)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if ((randomShot == 1 && canFireSplitingFireball) || fireFireblastFirst) // Firing gigablast while hovering above pre laugh
                            {
                                canFireSplitingFireball = false;
                                randomShot = fireblast;
                                SoundEngine.PlaySound(BrimstoneShotSound, NPC.Center);

                                for (int i = 0; i < 9; i++)
                                {
                                    Vector2 velOffset = NPC.DirectionTo(player.Center).RotatedByRandom(0.6) * Main.rand.NextFloat(5f, 13f);
                                    GlowOrbParticle spark2 = new GlowOrbParticle(projectileSpawn + velOffset * 2f, velOffset * 0.8f, false, 30, Main.rand.NextFloat(0.4f, 0.65f), permafrost ? Color.LightBlue : Main.rand.NextBool(3) ? Color.Lerp(Color.Red, Color.Magenta, 0.3f) : Color.Red);
                                    GeneralParticleHandler.SpawnParticle(spark2);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient && attackPause == 0)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (!fireFireblastFirst)
                            {
                                canFireSplitingFireball = true;
                                randomShot = barrage;
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                float rotation = MathHelper.ToRadians(20);
                                int numProj = 8;
                                for (int j = 0; j < numProj; j++)
                                {
                                    for (int i = 0; i < 6; i++) // Spread dust for pre laugh floating
                                    {
                                        Vector2 dustVel = (projectileVelocity * 2).RotatedByRandom(0.9) * (Main.rand.NextFloat(0.5f, 1.9f));
                                        GlowOrbParticle orb = new GlowOrbParticle(projectileSpawn, dustVel, false, 15, Main.rand.NextFloat(0.65f, 0.9f), permafrost ? Color.Cyan : Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.3f));
                                        GeneralParticleHandler.SpawnParticle(orb);
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient && attackPause == 0)
                                    {
                                        float projectileVelocityToPass = projectileVelocity.Length() * 1.3f;
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, j / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, perturbedSpeed, randomShot, DartDamage, 0f, Main.myPlayer, 0, 4f, projectileVelocityToPass);
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            fireFireblastFirst = false;
                        }

                        FrameType = FrameAnimationType.FasterUpwardDraft;
                    }
                    else if (NPC.ai[1] == 1f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

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
                            if (permafrost)
                            {
                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    SoundEngine.PlaySound(SoundID.Item60, NPC.Center);
                                    float velocity = 8;
                                    int type = ModContent.ProjectileType<Projectiles.Melee.DarkIceZero>();
                                    int damage = (int)(NPC.damage / 3);
                                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY) * velocity;
                                    float rotation = MathHelper.ToRadians(22);
                                    for (int i = 0; i < 3; i++)
                                    {
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (float)(2)));

                                        int p = Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + (perturbedSpeed).SafeNormalize(Vector2.UnitY) * 3f, perturbedSpeed, type, damage, 0f, Main.myPlayer, 0f, 2f);
                                        if (p.WithinBounds(Main.maxProjectiles))
                                        {
                                            Main.projectile[p].DamageType = DamageClass.Default;
                                            Main.projectile[p].friendly = false;
                                            Main.projectile[p].hostile = true;
                                            Main.projectile[p].tileCollide = true;
                                        }
                                    }
                                }
                            }
                        }

                        NPC.ai[1] = 2f;
                    }
                    else if (NPC.ai[1] == 2f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        NPC.ai[2] += 1f;

                        if (Math.Abs(NPC.velocity.X) > 0.15f)
                            NPC.spriteDirection = (NPC.velocity.X < 0f).ToDirectionInt();

                        if (NPC.ai[2] >= 25f)
                        {
                            // Avoid cheap bullshit
                            NPC.damage = 0;

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
                            // Avoid cheap bullshit
                            NPC.damage = 0;

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
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        int posX = 1;
                        if (NPC.Center.X < player.position.X + player.width)
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

                                // Release a burst of magic dust along with a brimstone hellblast skull. (pre laugh)
                                for (int i = 0; i < 6; i++)
                                {
                                    Vector2 velOffset = NPC.DirectionTo(player.Center).RotatedByRandom(0.6) * Main.rand.NextFloat(5f, 13f);
                                    PointParticle spark2 = new PointParticle(handPosition + velOffset * 2f, velOffset * 1.5f, false, 18, Main.rand.NextFloat(0.4f, 0.65f), permafrost ? Color.Cyan : Main.rand.NextBool(3) ? Color.Lerp(Color.Red, Color.Magenta, 0.3f) : Color.Red);
                                    GeneralParticleHandler.SpawnParticle(spark2);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    Vector2 projectileSpawn = NPC.Center + projectileVelocity * 4f;
                                    projectileVelocity *= 10f * uDieLul;
                                    int projectileType = hellblast;
                                    if (attackPause == 0)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                                }
                            }
                        }

                        if (Main.rand.NextBool()) // Hand spray magic
                        {
                            GlowOrbParticle orb = new GlowOrbParticle(handPosition, new Vector2(0, -6).RotatedByRandom(0.4) * Main.rand.NextFloat(0.8f, 1.4f), false, 15, Main.rand.NextFloat(0.85f, 1.2f), permafrost ? Color.LightBlue : Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.3f), true, true);
                            GeneralParticleHandler.SpawnParticle(orb);
                        }

                        FrameType = FrameAnimationType.OutwardHandCast;
                    }
                    else if (NPC.ai[1] == 4f)
                    {
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        int posX = 1;
                        if (NPC.Center.X < player.position.X + player.width)
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

                        if (NPC.localAI[1] > shootRate) // Firing spam projectiles post laugh
                        {
                            Vector2 handPosition = NPC.Center + new Vector2(NPC.spriteDirection * -22f, 2f);

                            // Release a burst of magic dust when punching (gigablast).
                            for (int i = 0; i < 25; i++)
                            {
                                Vector2 velOffset = NPC.DirectionTo(player.Center).RotatedByRandom(0.6) * Main.rand.NextFloat(5f, 13f);
                                GlowOrbParticle spark2 = new GlowOrbParticle(handPosition + velOffset * 2f, velOffset * 1.5f, false, 9, Main.rand.NextFloat(0.4f, 0.65f), permafrost ? Color.Cyan : Main.rand.NextBool(3) ? Color.Lerp(Color.Red, Color.Magenta, 0.3f) : Color.Red);
                                GeneralParticleHandler.SpawnParticle(spark2);
                            }
                            NPC.localAI[1] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                Vector2 projectileSpawn = NPC.Center + projectileVelocity * 8f;
                                projectileVelocity *= 5f * uDieLul;
                                int projectileType = gigablast;
                                if (attackPause == 0)
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);
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

                // Phase 2
                if (IsAtBrothers && hasSummonedBrothers && (permafrost ? NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHead>()) : (NPC.AnyNPCs(ModContent.NPCType<SupremeCataclysm>()) || NPC.AnyNPCs(ModContent.NPCType<SupremeCatastrophe>()))) == false)
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
                // Avoid cheap bullshit
                NPC.damage = 0;

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
                        for (int i = 0; i < 90; i++)
                        {
                            Dust spawnDust = Dust.NewDustPerfect(NPC.Center, permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone, new Vector2(30, 30).RotatedByRandom(100) * Main.rand.NextFloat(0.05f, 1.2f));
                            spawnDust.noGravity = true;
                            spawnDust.scale = Main.rand.NextFloat(1.2f, 2.3f);
                        }
                        for (int i = 0; i < 40; i++)
                        {
                            Vector2 sparkVel = new Vector2(20, 20).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.1f);
                            GlowOrbParticle orb = new GlowOrbParticle(NPC.Center + sparkVel * 2, sparkVel, false, 120, Main.rand.NextFloat(1.55f, 2.75f), permafrost ? Color.LightBlue : Color.Red, true, true);
                            GeneralParticleHandler.SpawnParticle(orb);
                        }
                        Particle pulse = new DirectionalPulseRing(NPC.Center, Vector2.Zero, permafrost ? Color.LightBlue : Color.Red, new Vector2(2f, 2f), 0, 0f, 1.1f, 25);
                        GeneralParticleHandler.SpawnParticle(pulse);

                        SoundEngine.PlaySound(SpawnSound, NPC.Center);
                    }
                }

                for (int i = 0; i < 4; i++)
                {
                    Dust brimstoneFire = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Square(-24f, 24f), permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone);
                    brimstoneFire.velocity = Vector2.UnitY * -Main.rand.NextFloat(2.75f, 4.25f);
                    brimstoneFire.noGravity = true;
                }

                if (!canDespawn)
                {
                    NPC.velocity *= 0f;

                    if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                        NPC.velocity.X = 0f;
                    if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                        NPC.velocity.Y = 0f;
                }
                FrameType = FrameAnimationType.Casting;
            }
            #endregion
            #region LastStage
            else
            {
                if (wormAlive)
                {
                    NPC.dontTakeDamage = true;
                    NPC.chaseable = false;
                }
                else
                {
                    if (permafrost ? NPC.AnyNPCs(ModContent.NPCType<Providence.Providence>()) : NPC.AnyNPCs(ModContent.NPCType<SoulSeekerSupreme>()))
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
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        // How fast SCal moves to the destination
                        float velocity = 12f;
                        float acceleration = 0.12f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

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
                            fireFireblastFirst = true;
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

                            if (randomShot == 0 && canFireSplitingFireball && !fireFireblastFirst) // fireblast while floating post laugh
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                canFireSplitingFireball = false;
                                randomShot = gigablast;

                                Particle pulse = new DirectionalPulseRing(NPC.Center, projectileVelocity * 1.2f, permafrost ? Color.Cyan : Color.Red, new Vector2(0.5f, 1f), projectileVelocity.ToRotation(), 0.92f, 0f, 55);
                                GeneralParticleHandler.SpawnParticle(pulse);
                                Particle pulse2 = new DirectionalPulseRing(NPC.Center, projectileVelocity * 1f, permafrost ? Color.Cyan : Color.Magenta, new Vector2(0.5f, 1f), projectileVelocity.ToRotation(), 0.95f, 0.4f, 55);
                                GeneralParticleHandler.SpawnParticle(pulse2);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if ((randomShot == 1 && canFireSplitingFireball) || fireFireblastFirst) // Gigablast while floating post laugh
                            {
                                SoundEngine.PlaySound(BrimstoneShotSound, NPC.Center);
                                if (NPC.ai[2] > 1)
                                    canFireSplitingFireball = false;
                                randomShot = fireblast;

                                Particle pulse = new DirectionalPulseRing(NPC.Center, projectileVelocity * 1.2f, permafrost ? Color.Cyan : Color.Red, new Vector2(0.5f, 1f), projectileVelocity.ToRotation(), 0.95f, 0f, 55);
                                GeneralParticleHandler.SpawnParticle(pulse);
                                Particle pulse2 = new DirectionalPulseRing(NPC.Center, projectileVelocity * 1f, permafrost ? Color.Cyan : Color.Magenta, new Vector2(0.5f, 1f), projectileVelocity.ToRotation(), 0.98f, 0.4f, 55);
                                GeneralParticleHandler.SpawnParticle(pulse2);

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, randomShot, FireblastDamage, 0f, Main.myPlayer, 0f, 2f);
                                    NPC.netUpdate = true;
                                }
                            }
                            else if (!fireFireblastFirst)
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                canFireSplitingFireball = true;
                                randomShot = barrage;
                                float rotation = MathHelper.ToRadians(20);
                                int numProj = 8;
                                for (int j = 0; j < numProj; j++)
                                {
                                    for (int i = 0; i < 7; i++) // Spred dust while floating post laugh
                                    {
                                        Vector2 dustVel = (projectileVelocity * 2).RotatedByRandom(0.9) * (Main.rand.NextFloat(0.5f, 1.9f));
                                        GlowOrbParticle orb = new GlowOrbParticle(projectileSpawn, dustVel, false, 15, Main.rand.NextFloat(0.75f, 1f), permafrost ? Color.LightBlue : Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.3f), true, true);
                                        GeneralParticleHandler.SpawnParticle(orb);
                                    }

                                    if (Main.netMode != NetmodeID.MultiplayerClient)
                                    {
                                        float projectileVelocityToPass = projectileVelocity.Length() * 1.3f;
                                        Vector2 perturbedSpeed = projectileVelocity.RotatedBy(MathHelper.Lerp(-rotation, rotation, j / (float)(numProj - 1)));
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, perturbedSpeed, randomShot, DartDamage, 0f, Main.myPlayer, 0f, 4f, projectileVelocityToPass);
                                        NPC.netUpdate = true;
                                    }
                                }
                            }
                            fireFireblastFirst = false;
                        }
                    }
                    else if (NPC.ai[1] == 1f)
                    {
                        // Set damage
                        NPC.damage = NPC.defDamage;

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
                        // Set damage
                        NPC.damage = NPC.defDamage;

                        NPC.ai[2] += 1f;
                        if (NPC.ai[2] >= 25f)
                        {
                            // Avoid cheap bullshit
                            NPC.damage = 0;

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
                            // Avoid cheap bullshit
                            NPC.damage = 0;

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
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        int posX = 1;
                        if (NPC.Center.X < player.position.X + player.width)
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

                                // Release a burst of magic dust along with a brimstone hellblast skull. (post laugh)
                                for (int i = 0; i < 9; i++)
                                {
                                    Vector2 velOffset = NPC.DirectionTo(player.Center).RotatedByRandom(0.6) * Main.rand.NextFloat(5f, 13f);
                                    PointParticle spark2 = new PointParticle(handPosition + velOffset * 2f, velOffset * 1.5f, false, 9, Main.rand.NextFloat(0.5f, 0.75f), permafrost ? Color.Cyan : Main.rand.NextBool(3) ? Color.Lerp(Color.Red, Color.Magenta, 0.3f) : Color.Red);
                                    GeneralParticleHandler.SpawnParticle(spark2);
                                }

                                if (Main.netMode != NetmodeID.MultiplayerClient)
                                {
                                    Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                                    Vector2 projectileSpawn = NPC.Center + projectileVelocity * 4f;
                                    projectileVelocity *= 10f * uDieLul;
                                    int projectileType = hellblast;
                                    if (attackPause == 0)
                                        Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, HellblastDamage, 0f, Main.myPlayer, 0f, 2f);
                                }
                            }
                        }

                        if (Main.rand.NextBool()) // Hand visual post laugh
                        {
                            GlowOrbParticle orb = new GlowOrbParticle(handPosition, new Vector2(0, -6).RotatedByRandom(0.4) * Main.rand.NextFloat(0.8f, 1.4f), false, 15, Main.rand.NextFloat(0.95f, 1.45f), permafrost ? Color.LightBlue : Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f), true, true);
                            GeneralParticleHandler.SpawnParticle(orb);
                        }
                        Dust fust = Dust.NewDustPerfect(handPosition, Main.rand.NextBool(3) ? 60 : 114);
                        fust.noGravity = true;
                        fust.velocity = new Vector2(0, -6).RotatedByRandom(0.4) * Main.rand.NextFloat(0.8f, 1.4f);
                        fust.scale = Main.rand.NextFloat(0.3f, 0.65f);

                        FrameChangeSpeed = 0.245f;
                        FrameType = FrameAnimationType.PunchHandCast;
                    }
                    else if (NPC.ai[1] == 4f)
                    {
                        // Avoid cheap bullshit
                        NPC.damage = 0;

                        // How fast SCal moves to the destination
                        float velocity = 32f;
                        float acceleration = 1.2f;

                        if (Main.getGoodWorld)
                        {
                            velocity *= 1.15f;
                            acceleration *= 1.15f;
                        }

                        int posX = 1;
                        if (NPC.Center.X < player.position.X + player.width)
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

                        if (NPC.localAI[1] > shootRate) // small gigablast post laugh
                        {
                            Vector2 handPosition = NPC.Center + new Vector2(NPC.spriteDirection * -22f, 2f);
                            Vector2 projectileVelocity = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
                            Vector2 projectileSpawn = NPC.Center + projectileVelocity * 8f;

                            // Release a burst of magic dust when punching.
                            Particle pulse = new DirectionalPulseRing(NPC.Center, projectileVelocity * 9, permafrost ? Color.Cyan : Color.Red, new Vector2(0.5f, 1f), projectileVelocity.ToRotation(), 0.9f, 0f, 60);
                            GeneralParticleHandler.SpawnParticle(pulse);
                            Particle pulse2 = new DirectionalPulseRing(NPC.Center, projectileVelocity * 8, permafrost ? Color.Cyan : Color.Magenta, new Vector2(0.5f, 1f), projectileVelocity.ToRotation(), 0.93f, 0.4f, 60);
                            GeneralParticleHandler.SpawnParticle(pulse2);

                            NPC.localAI[1] = 0f;

                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                SoundEngine.PlaySound(BrimstoneBigShotSound, NPC.Center);
                                projectileVelocity *= 5f * uDieLul;
                                int projectileType = gigablast;
                                Projectile.NewProjectile(NPC.GetSource_FromAI(), projectileSpawn, projectileVelocity, projectileType, GigablastDamage, 0f, Main.myPlayer, 0f, 2f);
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
            // Bonus visuals
            if (!canDespawn && !hasDoneDeathAnim && shieldOpacity >= 0.9f)
            {
                NPC.damage = NPC.defDamage;
                if (dashVisualCounter < 9)
                    dashVisualCounter++;
                else
                {
                    // Spawn in a helix-style pattern
                    float sine = (float)Math.Sin(NPC.ai[2] * (0.975f * MathHelper.Clamp(Utils.GetLerpValue(120, 0, dashVisualCounter), 0.5f, 1.1f)) / MathHelper.Pi);

                    Vector2 offset = NPC.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2) * sine * 33f;

                    SparkParticle spark4 = new SparkParticle(NPC.Center + offset - NPC.velocity.SafeNormalize(Vector2.UnitY) * 15f, -NPC.velocity * 0.85f, false, 10, 1.9f * MathHelper.Clamp(Utils.GetLerpValue(120, 0, dashVisualCounter), 0.5f, 1.1f), Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f));
                    GeneralParticleHandler.SpawnParticle(spark4);
                    SparkParticle spark5 = new SparkParticle(NPC.Center - offset - NPC.velocity.SafeNormalize(Vector2.UnitY) * 15f, -NPC.velocity * 0.85f, false, 10, 1.9f * MathHelper.Clamp(Utils.GetLerpValue(120, 0, dashVisualCounter), 0.5f, 1.1f), Main.rand.NextBool() ? Color.Red : Color.Lerp(Color.Red, Color.Magenta, 0.5f));
                    GeneralParticleHandler.SpawnParticle(spark5);

                    Dust dashDust = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(NPC.width, NPC.height), DustID.TheDestroyer);
                    dashDust.noGravity = true;
                    dashDust.velocity = -NPC.velocity.RotatedByRandom(0.15f) * Main.rand.NextFloat(0.9f, 1.2f);
                    dashDust.scale = Main.rand.NextFloat(0.6f, 1.4f);

                    dashVisualCounter++;
                }
            }
            else
            {
                NPC.damage = 0;
                dashVisualCounter = 0;
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

            foreach (Vector2 heartSpawnPosition in heartSpawnPositions)
            {
                Vector2 leftDustPosition = Vector2.CatmullRom(armPosition + Vector2.UnitY * 1000f, armPosition, heartSpawnPosition, heartSpawnPosition + Vector2.UnitY * 1000f, castCompletion);

                Dust castMagicDust = Dust.NewDustPerfect(leftDustPosition, DustID.RainbowMk2);
                castMagicDust.scale = 1.67f;
                castMagicDust.velocity = Main.rand.NextVector2CircularEdge(0.2f, 0.2f);
                castMagicDust.fadeIn = 0.67f;
                castMagicDust.color = permafrost ? Color.Cyan : Color.Red;
                castMagicDust.noGravity = true;
            }

            if (attackCastDelay == 0)
            {
                string key = permafrost ? "Mods.CalamityMod.Status.Boss.PermafrostBirbSwarmText" : "Mods.CalamityMod.Status.Boss.SCalStartText";
                if (NPC.life <= NPC.lifeMax * 0.08)
                    key = permafrost ? "Mods.CalamityMod.Status.Boss.PermafrostSecondBirbSwarmText" : "Mods.CalamityMod.Status.Boss.SCalSepulcher2Text";

                if (!BossRushEvent.BossRushActive)
                {
                    if (DownedBossSystem.downedCalamitas && !permafrost)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                }

                foreach (Vector2 heartSpawnPosition in heartSpawnPositions)
                {
                    // Make the hearts appear in a burst of flame.
                    // Spawn Dragonfollies if Permfroast exists.
                    for (int i = 0; i < 20; i++)
                    {
                        Dust castFire = Dust.NewDustPerfect(heartSpawnPosition + Main.rand.NextVector2Square(-30f, 30f), permafrost ? (int)CalamityDusts.Necroplasm : (int)CalamityDusts.Brimstone);
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
                    if (permafrost)
                    {
                        for (int x = 0; x < 5; x++)
                        {
                            NPC.NewNPC(NPC.GetSource_FromAI(), spawnX + 50, tempSpawnY, ModContent.NPCType<Dragonfolly>());
                            spawnX += spawnXAdd;

                            NPC.NewNPC(NPC.GetSource_FromAI(), spawnX2 - 50, tempSpawnY, ModContent.NPCType<Dragonfolly>());
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
                            NPC.NewNPC(NPC.GetSource_FromAI(), (int)(safeBox.Center().X), (int)(safeBox.Bottom().Y), ModContent.NPCType<SepulcherHead>());
                    }

                    NPC.netUpdate = true;
                }

                SoundEngine.PlaySound(SepulcherSummonSound, target.Center);
            }
        }

        public void DoBrothersSpawningCastAnimation(int bodyWidth, int bodyHeight)
        {
            Player player = Main.player[NPC.target];

            Vector2 leftOfCircle = NPC.Center + Vector2.UnitY * bodyHeight * 0.5f - Vector2.UnitX * bodyWidth * 0.45f;
            Vector2 rightOfCircle = NPC.Center + Vector2.UnitY * bodyHeight * 0.5f + Vector2.UnitX * bodyWidth * 0.45f;

            if (Main.netMode != NetmodeID.MultiplayerClient && catastropheSpawnPosition == Vector2.Zero)
            {
                catastropheSpawnPosition = NPC.Center - Vector2.UnitX * 500f;
                cataclysmSpawnPosition = NPC.Center + Vector2.UnitX * 500f;
                NPC.netUpdate = true;
            }

            // Draw some magic much like the sandstorm elemental cast that approaches where the brothers will spawn.
            if (attackCastDelay < brothersSpawnCastTime - 45f && attackCastDelay >= 60f)
            {
                float castCompletion = Utils.GetLerpValue(brothersSpawnCastTime - 45f, 60f, attackCastDelay);

                Vector2 leftDustPosition = Vector2.CatmullRom(leftOfCircle + Vector2.UnitY * 1000f, leftOfCircle, catastropheSpawnPosition, catastropheSpawnPosition + Vector2.UnitY * 1000f, castCompletion);
                Vector2 rightDustPosition = Vector2.CatmullRom(rightOfCircle + Vector2.UnitY * 1000f, rightOfCircle, cataclysmSpawnPosition, cataclysmSpawnPosition + Vector2.UnitY * 1000f, castCompletion);

                GlowOrbParticle orb = new GlowOrbParticle(leftDustPosition, Vector2.Zero, false, 20, 2.8f - attackCastDelay * 0.01f, permafrost ? Color.Cyan : Color.Red, true, true);
                GeneralParticleHandler.SpawnParticle(orb);
                GlowOrbParticle orb2 = new GlowOrbParticle(rightDustPosition, Vector2.Zero, false, 20, 2.8f - attackCastDelay * 0.01f, permafrost ? Color.Cyan : Color.Red, true, true);
                GeneralParticleHandler.SpawnParticle(orb2);
            }

            // Make some magic effects at where the bros will spawn.
            if (attackCastDelay < 60f)
            {
                if (attackCastDelay == 59)
                {

                    Particle bloom1 = new BloomParticle(catastropheSpawnPosition, Vector2.Zero, Color.DeepSkyBlue, 0f, 1.4f, 59, false);
                    GeneralParticleHandler.SpawnParticle(bloom1);
                    Particle bloom2 = new BloomParticle(catastropheSpawnPosition, Vector2.Zero, Color.DeepSkyBlue, 0f, 1f, 59, false);
                    GeneralParticleHandler.SpawnParticle(bloom2);
                    Particle bloom3 = new BloomParticle(cataclysmSpawnPosition, Vector2.Zero, new Color(121, 21, 77), 0f, 1.4f, 59, false);
                    GeneralParticleHandler.SpawnParticle(bloom3);
                    Particle bloom4 = new BloomParticle(cataclysmSpawnPosition, Vector2.Zero, new Color(121, 21, 77), 0f, 1f, 59, false);
                    GeneralParticleHandler.SpawnParticle(bloom4);
                }
                float burnPower = Utils.GetLerpValue(60f, 20f, attackCastDelay);
                if (attackCastDelay == 0f)
                    burnPower = 4f;

                for (int i = 0; i < MathHelper.Lerp(1, 6, burnPower); i++)
                {
                    Vector2 velOffset = CalamityUtils.RandomVelocity(100f, 70f, 150f, 0.04f);
                    velOffset *= Main.rand.NextFloat(2, 13);
                    VoidSparkParticle spark1 = new VoidSparkParticle(catastropheSpawnPosition + velOffset * 4.5f, -velOffset * 0.25f, false, 9, Main.rand.NextFloat(0.08f, 0.16f) - attackCastDelay * 0.002f, Main.rand.NextBool(5) ? Color.DeepSkyBlue : Color.Red);
                    GeneralParticleHandler.SpawnParticle(spark1);
                    VoidSparkParticle spark2 = new VoidSparkParticle(cataclysmSpawnPosition + velOffset * 4.5f, -velOffset * 0.25f, false, 9, Main.rand.NextFloat(0.08f, 0.16f) - attackCastDelay * 0.002f, Main.rand.NextBool(5) ? new Color(121, 21, 77) : Color.Red);
                    GeneralParticleHandler.SpawnParticle(spark2);
                }
            }

            // And spawn them.
            if (attackCastDelay == 0)
            {
                for (int i = 0; i < 30; i++)
                {
                    SparkParticle spark1 = new SparkParticle(catastropheSpawnPosition, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.5f), false, 35, Main.rand.NextFloat(1.2f, 1.45f), Main.rand.NextBool() ? Color.DeepSkyBlue : Color.Red);
                    GeneralParticleHandler.SpawnParticle(spark1);

                    SparkParticle spark2 = new SparkParticle(cataclysmSpawnPosition, new Vector2(5, 5).RotatedByRandom(100) * Main.rand.NextFloat(0.5f, 1.5f), false, 35, Main.rand.NextFloat(1.2f, 1.45f), Main.rand.NextBool() ? new Color(80, 21, 77) : Color.Red);
                    GeneralParticleHandler.SpawnParticle(spark2);

                    Dust catastrophedust = Dust.NewDustPerfect(catastropheSpawnPosition, DustID.SilverFlame, new Vector2(9, 9).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.5f));
                    catastrophedust.noGravity = true;
                    catastrophedust.scale = Main.rand.NextFloat(1.5f, 1.85f);
                    catastrophedust.color = Color.DeepSkyBlue;

                    Dust cataclysmdust = Dust.NewDustPerfect(cataclysmSpawnPosition, DustID.SilverFlame, new Vector2(9, 9).RotatedByRandom(100) * Main.rand.NextFloat(0.1f, 1.5f));
                    cataclysmdust.noGravity = true;
                    cataclysmdust.scale = Main.rand.NextFloat(1.5f, 1.85f);
                    cataclysmdust.color = new Color(80, 21, 77);

                }

                if (!BossRushEvent.BossRushActive)
                {
                    string key = "Mods.CalamityMod.Status.Boss.SCalBrothersText";
                    if (permafrost)
                        key = "Mods.CalamityMod.Status.Boss.PermafrostDoGText";
                    else if (DownedBossSystem.downedCalamitas)
                        key += "Rematch";

                    CalamityUtils.BroadcastLocalizedText(key, permafrost ? permafrostTextColor : textColor);
                }

                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    bool broDirection = Main.rand.NextBool();
                    CalamityUtils.SpawnBossBetter(catastropheSpawnPosition, permafrost ? ModContent.NPCType<DevourerofGodsHead>() : ModContent.NPCType<SupremeCatastrophe>(), null, broDirection == false ? 1 : -1);
                    if (!permafrost)
                        CalamityUtils.SpawnBossBetter(cataclysmSpawnPosition, ModContent.NPCType<SupremeCataclysm>(), null, broDirection == true ? 1 : -1);
                }

                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/RavagerDeath1") with { Pitch = -0.2f }, cataclysmSpawnPosition);
                SoundEngine.PlaySound(new SoundStyle("CalamityMod/Sounds/NPCKilled/RavagerDeath2") with { Pitch = -0.2f }, catastropheSpawnPosition);
                hasSummonedBrothers = true;
            }

            NPC.velocity *= 0f;
            if (NPC.velocity.X > -0.1 && NPC.velocity.X < 0.1)
                NPC.velocity.X = 0f;
            if (NPC.velocity.Y > -0.1 && NPC.velocity.Y < 0.1)
                NPC.velocity.Y = 0f;

            FrameType = FrameAnimationType.Casting;
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
            CalamityGlobalNPC.SCalGrief = -1;
            CalamityGlobalNPC.SCalLament = -1;
            CalamityGlobalNPC.SCalEpiphany = -1;
            CalamityGlobalNPC.SCalAcceptance = -1;

            if (startFifthAttack && gettingTired5 && (giveUpCounter < 1160 || hasDoneDeathAnim))
                CalamityGlobalNPC.SCalAcceptance = NPC.whoAmI;
            else if (IsAtHp_BH4)
                CalamityGlobalNPC.SCalEpiphany = NPC.whoAmI;
            else if (IsAtHp_BH3)
                CalamityGlobalNPC.SCalLament = NPC.whoAmI;
            else
                CalamityGlobalNPC.SCalGrief = NPC.whoAmI;
        }

        #region Loot
        public override void BossLoot(ref int potionType)
        {
            potionType = ModContent.ItemType<OmegaHealingPotion>();
        }

        public override void OnKill()
        {
            // Don't bother running any of this in Boss Rush.
            if (BossRushEvent.BossRushActive)
                return;

            CalamityGlobalNPC.SetNewBossJustDowned(NPC);

            // Increase the player's SCal kill count
            if (Main.player[NPC.target].Calamity().sCalKillCount < 5)
                Main.player[NPC.target].Calamity().sCalKillCount++;

            // Spawn the SCal NPC directly where the boss was
            if (!BossRushEvent.BossRushActive)
                NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y + 12, permafrost ? ModContent.NPCType<Archmage>() : ModContent.NPCType<BrimstoneWitch>());

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
                // 16NOV2025: Ozzatron: item has been chosen as the "Expert gatekept" item for this Calamity boss
                // normalOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<Calamity>()));

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
                GFBOnly.Add(DropHelper.PerPlayer(ModContent.ItemType<SlurperPole>()), hideLootReport: true);
            }

            // Legendary seed Coldheart Icicle        
            npcLoot.Add(ItemDropRule.ByCondition(DropHelper.If(info => info.npc.type == ModContent.NPCType<SupremeCalamitas>() && info.npc.ModNPC<SupremeCalamitas>().permafrost, false), ModContent.ItemType<ColdheartIcicle>()));

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
            // CIT 16MAY2025: SCal must have contact damage set on the first frame to preserve difficulty mode stat scaling, sometimes leading to people getting hit if on top of her.
            // Thus, also check if the arena has spawned, since that will not be true on the first frame.
            return (collidingWithShield || NPC.Hitbox.Intersects(target.Hitbox)) && spawnArena;
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
        }

        public override Color? GetAlpha(Color drawColor) => willCharge ? Color.Lerp(Color.Red, drawColor, 0.7f) * NPC.Opacity * 0.45f : null;

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            SpriteEffects spriteEffects = SpriteEffects.None;
            if (NPC.spriteDirection == 1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Texture2D texture2D15 = DownedBossSystem.downedCalamitas && !BossRushEvent.BossRushActive ? TextureAssets.Npc[Type].Value : HoodedTexture.Value;
            bool inPhase2 = NPC.ai[0] >= 3f && (NPC.life > NPC.lifeMax * 0.01 || permafrost);

            if (permafrost)
                texture2D15 = PermafrostTexture.Value;

            Vector2 halfSizeTexture = new Vector2(texture2D15.Width / 2f, texture2D15.Height / Main.npcFrameCount[Type] / 2f);
            int afterimageAmt = 7;

            Rectangle frame = texture2D15.Frame(2, Main.npcFrameCount[Type], NPC.frame.Y / Main.npcFrameCount[Type], NPC.frame.Y % Main.npcFrameCount[Type]);

            if (CalamityClientConfig.Instance.Afterimages && !(permafrost && NPC.ai[1] == 2f))
            {
                for (int i = 1; i < afterimageAmt; i += 2)
                {
                    Color afterimageColor = drawColor;
                    afterimageColor = Color.Lerp(afterimageColor, Color.White, 0.5f);
                    afterimageColor = NPC.GetAlpha(afterimageColor);
                    afterimageColor *= (afterimageAmt - i) / 15f;
                    Vector2 afterimagePos = NPC.oldPos[i] + new Vector2(NPC.width, NPC.height) / 2f - screenPos;
                    afterimagePos -= new Vector2(texture2D15.Width / 2f, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
                    afterimagePos += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);
                    spriteBatch.Draw(texture2D15, afterimagePos, frame, afterimageColor, NPC.rotation, halfSizeTexture, NPC.scale, spriteEffects, 0f);
                }
            }

            Vector2 drawLocation = NPC.Center - screenPos;
            drawLocation -= new Vector2(texture2D15.Width / 2f, texture2D15.Height / Main.npcFrameCount[Type]) * NPC.scale / 2f;
            drawLocation += halfSizeTexture * NPC.scale + new Vector2(0f, NPC.gfxOffY);

            if (!(permafrost && NPC.ai[1] == 2f))
            {
                if (inPhase2)
                {
                    // Make the sprite jitter with rage in phase 2. This does not happen in rematches since it would make little sense logically.
                    if (!DownedBossSystem.downedCalamitas)
                        drawLocation += Main.rand.NextVector2Circular(0.25f, 0.7f);

                    // And gain a flaming aura.
                    Color auraColor = NPC.GetAlpha(permafrost ? Color.Cyan : Color.Red) * 0.4f;
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
                DrawShield(spriteBatch);
            }
            return false;
        }

        public void DrawForcefield(SpriteBatch spriteBatch)
        {
            Player player = Main.player[NPC.target];

            float opacity = 1;
            spriteBatch.EnterShaderRegion();

            float intensity = hitTimer / 35f;

            // Shield intensity is always high during invincibility, except during cast animations, so that she can be more easily seen.
            if (NPC.dontTakeDamage && attackCastDelay <= 0)
                intensity = 0.75f + Math.Abs((float)Math.Cos(Main.GlobalTimeWrappedHourly * 1.7f)) * 0.1f;

            // Make the forcefield weaker in the second phase as a means of showing desparation.
            //if (NPC.ai[0] >= 3f)
            //intensity *= 0.6f;


            if (lifeRatio < 0.05f)
                forcefieldOpacity = 0.75f;
            if (lifeRatio <= 0.01f)
                forcefieldOpacity = 0.6f;

            float flickerPower = 0f;
            if (lifeRatio < 0.6f)
                flickerPower += 0.1f;
            if (lifeRatio < 0.3f)
                flickerPower += 0.25f;
            if (postMusicHit)
                flickerPower += 0.61f;
            if (lifeRatio < 0.05f)
                flickerPower += Main.rand.NextFloat(0.7f, 1f);
            if (lifeRatio <= 0.01f)
                flickerPower += 0.08f;
            opacity = forcefieldOpacity;
            opacity *= MathHelper.Lerp(1f, MathHelper.Max(1f - flickerPower, 0.56f), (float)Math.Pow(Math.Cos(Main.GlobalTimeWrappedHourly * MathHelper.Lerp(3f, 5f, flickerPower)), 24D));

            // During/prior to a charge the forcefield is always darker than usual and thus its intensity is also higher.
            if (!NPC.dontTakeDamage && (willCharge || NPC.ai[1] == 2f))
                intensity = 1.1f;

            // Dampen the opacity and intensity slightly, to allow SCal to be more easily visible inside of the forcefield.
            intensity *= musicSyncCounter <= 0 && musicSyncCounter > -30 ? Utils.GetLerpValue(120, 0, musicSyncCounter, true) : 0.75f;
            opacity *= musicSyncCounter <= 0 && musicSyncCounter > -30 ? Utils.GetLerpValue(120, 0, musicSyncCounter, true) : 0.75f;

            Texture2D forcefieldTexture = ForcefieldTexture.Value;
            GameShaders.Misc["CalamityMod:SupremeShield"].UseImage1("Images/Misc/Perlin");

            Color forcefieldColor = Color.DarkViolet;
            Color secondaryForcefieldColor = (permafrost ? Color.Cyan : Color.Red) * 1.4f;

            if (!NPC.dontTakeDamage && willCharge && NPC.ai[1] != 2f)
            {
                forcefieldColor = Color.Magenta;
                secondaryForcefieldColor = Color.Lerp(secondaryForcefieldColor, Color.Lerp(Color.Red, Color.Magenta, 0.3f), 0.7f);
            }
            else
                forcefieldColor = Color.DarkViolet;

            forcefieldColor *= opacity;
            secondaryForcefieldColor *= opacity;

            GameShaders.Misc["CalamityMod:SupremeShield"].UseSecondaryColor(secondaryForcefieldColor);
            GameShaders.Misc["CalamityMod:SupremeShield"].UseColor(forcefieldColor);
            GameShaders.Misc["CalamityMod:SupremeShield"].UseSaturation(1);
            GameShaders.Misc["CalamityMod:SupremeShield"].UseOpacity(0.65f);
            GameShaders.Misc["CalamityMod:SupremeShield"].Apply();

            Texture2D centerTexture = ModContent.Request<Texture2D>("CalamityMod/Particles/CentralGold").Value;

            Texture2D immuneTex = ModContent.Request<Texture2D>("CalamityMod/Particles/SemiCircularSmearVertical").Value;
            if (postMusicHit)
                spriteBatch.Draw(centerTexture, NPC.Center - Main.screenPosition, null, (Color.White with { A = 0 } * opacity) * 2f, rotateAwayPlayer, centerTexture.Size() * 0.5f, forcefieldScale * 0.088f * forcefieldPureVisualScale, SpriteEffects.None, 0f);
            if (!NPC.dontTakeDamage)
                spriteBatch.Draw(forcefieldTexture, NPC.Center - Main.screenPosition, null, Color.White * opacity, postMusicHit ? rotateToPlayer : 0, forcefieldTexture.Size() * 0.5f, forcefieldScale * 3f * forcefieldPureVisualScale, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(immuneTex, NPC.Center - Main.screenPosition, null, Color.White * opacity * 0.3f, rotateToPlayer, immuneTex.Size() * 0.5f, forcefieldScale * 1.35f * forcefieldPureVisualScale, SpriteEffects.None, 0f);
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
            Texture2D shieldSkullTexture = ShieldTopTexture.Value;
            Texture2D shieldJawTexture = ShieldBottomTexture.Value;
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
                Dust.NewDust(NPC.position, NPC.width, NPC.height, permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone, hit.HitDirection, -1f, 0, default, 1f);
            }
            if (NPC.life <= 0)
            {
                NPC.position = NPC.Center;
                NPC.width = NPC.height = 100;
                NPC.position.X = NPC.position.X - (NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (NPC.height / 2);
                for (int i = 0; i < 40; i++)
                {
                    int onHitDust = Dust.NewDust(NPC.position, NPC.width, NPC.height, permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[onHitDust].velocity *= 3f;
                    if (Main.rand.NextBool())
                    {
                        Main.dust[onHitDust].scale = 0.5f;
                        Main.dust[onHitDust].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 70; j++)
                {
                    int onHitDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 3f);
                    Main.dust[onHitDust2].noGravity = true;
                    Main.dust[onHitDust2].velocity *= 5f;
                    onHitDust2 = Dust.NewDust(NPC.position, NPC.width, NPC.height, permafrost ? DustID.IceGolem : (int)CalamityDusts.Brimstone, 0f, 0f, 100, default, 2f);
                    Main.dust[onHitDust2].velocity *= 2f;
                }
            }
        }

        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance * bossAdjustment);
        }
    }
}
