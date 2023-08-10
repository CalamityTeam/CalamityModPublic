using CalamityMod.Events;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.CalClone;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Apollo;
using CalamityMod.NPCs.ExoMechs.Ares;
using CalamityMod.NPCs.ExoMechs.Artemis;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlagueEnemies;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Events;
using Terraria.GameContent.UI.BigProgressBar;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

using static Terraria.ModLoader.ModContent;

namespace CalamityMod.UI
{
    /*
    Heyo! Here's some things you might need to know about this class and where to change things:

    In the "Load" method is where the "OneToMany" dictionary is updated.
    If an NPC is made up of multiple segments (such as EoW, etc.) that all have separate health but would be considered a single boss, you can update the dictionary.
    The boss health bar will automatically keep track of all segments of that type and add up the health.

    The "SetupBossExclusionList" method allows you to check for if certain mods are loaded and setup the exclusion list based on what mods are loaded.
    For now I've had to load calamity separately, you might want to change that by just passing in Calamity itself etc.

    If you need to add a special kind of health bar, like the EoW one which counts the number of segments left, let me know.
    I may need to dive into the spaghetti to add it.

    There's a few readonly fields at the top of both classes that you can edit should you deem it fit to. Change the animation lengths, etc.
    The bar uses those and shouldn't have any problems dealing with the new values
    (although the flickering in the start animation will always happen in the first 20 frames or so)

    As for toggling, in the Mod File, you can simply add a boolean check and not draw or update the health bar manager.
    There is a "CanDrawExtraSmallText" field below here which is public, if that's false then the smalltext won't draw.

    That should be it -- ask if you have any questions!
    */

    public class BossHealthBarManager : ModBossBarStyle
    {
        public struct BossEntityExtension
        {
            public LocalizedText NameOfExtensions;
            public int[] TypesToSearchFor;
            public BossEntityExtension(LocalizedText name, params int[] types)
            {
                NameOfExtensions = name;
                TypesToSearchFor = types;
            }
        }

        public static bool CanDrawExtraSmallText = true;
        public static int MaximumBars = 4;

        public static List<BossHPUI> Bars;

        public static DynamicSpriteFont HPBarFont;
        public static Texture2D BossMainHPBar;
        public static Texture2D BossComboHPBar;
        public static Texture2D BossSeperatorBar;

        public static Dictionary<int, int[]> OneToMany;
        public static List<int> BossExclusionList;
        public static List<int> MinibossHPBarList;
        public static Dictionary<int, BossEntityExtension> EntityExtensionHandler = new Dictionary<int, BossEntityExtension>();
        public static Dictionary<NPCSpecialHPGetRequirement, NPCSpecialHPGetFunction> SpecialHPRequirements = new Dictionary<NPCSpecialHPGetRequirement, NPCSpecialHPGetFunction>();

        public delegate bool NPCSpecialHPGetRequirement(NPC npc);
        public delegate long NPCSpecialHPGetFunction(NPC npc, bool checkingForMaxLife);

        internal static void Load(Mod mod)
        {
            Bars = new List<BossHPUI>();

            if (!Main.dedServ)
            {
                BossMainHPBar = Request<Texture2D>("CalamityMod/UI/MiscTextures/BossHPMainBar", AssetRequestMode.ImmediateLoad).Value;
                BossComboHPBar = Request<Texture2D>("CalamityMod/UI/MiscTextures/BossHPComboBar", AssetRequestMode.ImmediateLoad).Value;
                BossSeperatorBar = Request<Texture2D>("CalamityMod/UI/MiscTextures/BossHPSeperatorBar", AssetRequestMode.ImmediateLoad).Value;

                PlatformID id = Environment.OSVersion.Platform;
                if (id == PlatformID.Win32NT)
                    HPBarFont = Request<DynamicSpriteFont>("CalamityMod/Fonts/HPBarFont", AssetRequestMode.ImmediateLoad).Value;
                else
                    HPBarFont = FontAssets.MouseText.Value;
            }

            OneToMany = new Dictionary<int, int[]>();

            int[] EoW = new int[] { NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail };
            OneToMany[NPCID.EaterofWorldsHead] = EoW;
            OneToMany[NPCID.EaterofWorldsBody] = EoW;
            OneToMany[NPCID.EaterofWorldsTail] = EoW;

            int[] BoC = new int[] { NPCID.BrainofCthulhu, NPCID.Creeper };
            OneToMany[NPCID.BrainofCthulhu] = BoC;
            OneToMany[NPCID.Creeper] = BoC;

            int[] Skele = new int[] { NPCID.SkeletronHead, NPCID.SkeletronHand };
            OneToMany[NPCID.SkeletronHead] = Skele;
            OneToMany[NPCID.SkeletronHand] = Skele;

            int[] SkelePrime = new int[] { NPCID.SkeletronPrime, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeCannon, NPCID.PrimeLaser };
            OneToMany[NPCID.SkeletronPrime] = SkelePrime;
            OneToMany[NPCID.PrimeSaw] = SkelePrime;
            OneToMany[NPCID.PrimeVice] = SkelePrime;
            OneToMany[NPCID.PrimeCannon] = SkelePrime;
            OneToMany[NPCID.PrimeLaser] = SkelePrime;

            int[] Golem = new int[] { NPCID.Golem, NPCID.GolemFistLeft, NPCID.GolemFistRight, NPCID.GolemHead, NPCID.GolemHeadFree };
            OneToMany[NPCID.Golem] = Golem;
            OneToMany[NPCID.GolemFistLeft] = Golem;
            OneToMany[NPCID.GolemFistRight] = Golem;
            OneToMany[NPCID.GolemHead] = Golem;
            OneToMany[NPCID.GolemHeadFree] = Golem;

            int[] Saucer = new int[] { NPCID.MartianSaucerCore, NPCID.MartianSaucerTurret, NPCID.MartianSaucerCannon };
            OneToMany[NPCID.MartianSaucerCore] = Saucer;
            OneToMany[NPCID.MartianSaucerTurret] = Saucer;
            OneToMany[NPCID.MartianSaucerCannon] = Saucer;

            int[] Ship = new int[] { NPCID.PirateShip, NPCID.PirateShipCannon };
            OneToMany[NPCID.PirateShip] = Ship;
            OneToMany[NPCID.PirateShipCannon] = Ship;

            int[] MoonLord = new int[] { NPCID.MoonLordHead, NPCID.MoonLordHand, NPCID.MoonLordCore };
            OneToMany[NPCID.MoonLordCore] = MoonLord;

            int[] Void = new int[] { NPCType<CeaselessVoid>(), NPCType<DarkEnergy>() };
            OneToMany[NPCType<CeaselessVoid>()] = Void;
            OneToMany[NPCType<DarkEnergy>()] = Void;

            int[] Rav = new int[] { NPCType<RavagerBody>(), NPCType<RavagerClawRight>(), NPCType<RavagerClawLeft>(),
                NPCType<RavagerLegRight>(), NPCType<RavagerLegLeft>(), NPCType<RavagerHead>() };
            OneToMany[NPCType<RavagerBody>()] = Rav;
            OneToMany[NPCType<RavagerClawRight>()] = Rav;
            OneToMany[NPCType<RavagerClawLeft>()] = Rav;
            OneToMany[NPCType<RavagerLegRight>()] = Rav;
            OneToMany[NPCType<RavagerLegLeft>()] = Rav;
            OneToMany[NPCType<RavagerHead>()] = Rav;

            int[] SlimeGod = new int[] { NPCType<EbonianPaladin>(), NPCType<SplitEbonianPaladin>(),
                NPCType<CrimulanPaladin>(), NPCType<SplitCrimulanPaladin>() };
            OneToMany[NPCType<EbonianPaladin>()] = SlimeGod;
            OneToMany[NPCType<CrimulanPaladin>()] = SlimeGod;

            SetupBossExclusionList();
            SetupMinibossHPBarList();
            SetupExtensionHandlerList();
            SetupRequirementsList();
        }

        public static void SetupBossExclusionList()
        {
            BossExclusionList = new List<int>
            {
                NPCID.None,
                NPCID.MoonLordFreeEye,
                NPCID.MoonLordHead,
                NPCID.MoonLordHand,
                NPCID.WyvernLegs,
                NPCID.WyvernBody,
                NPCID.WyvernBody2,
                NPCID.WyvernBody3,
                NPCID.WyvernTail,
                NPCType<AquaticScourgeBody>(),
                NPCType<AquaticScourgeBodyAlt>(),
                NPCType<AquaticScourgeTail>(),
                NPCType<AstrumDeusBody>(),
                NPCType<AstrumDeusTail>(),
                NPCType<DesertScourgeBody>(),
                NPCType<DesertScourgeTail>(),
                NPCType<SlimeGodCore>(),
                NPCType<StormWeaverBody>(),
                NPCType<StormWeaverTail>(),
                NPCType<DevourerofGodsBody>(),
                NPCType<DevourerofGodsTail>(),
                NPCType<ThanatosBody1>(),
                NPCType<ThanatosBody2>(),
                NPCType<ThanatosTail>(),
                NPCType<AresGaussNuke>(),
                NPCType<AresLaserCannon>(),
                NPCType<AresPlasmaFlamethrower>(),
                NPCType<AresTeslaCannon>(),
            };
        }

        public static void SetupMinibossHPBarList()
        {
            MinibossHPBarList = new List<int>
            {
                // DD2 Event.
                NPCID.DD2Betsy,
                NPCID.DD2OgreT2,
                NPCID.DD2OgreT3,
                NPCID.DD2DarkMageT1, // 800 HP, T1 variant.
                NPCID.DD2DarkMageT3,

                // Prehardmode.
                NPCID.DungeonGuardian,

                // Hardmode.
                NPCID.GoblinSummoner, // 2000 HP
                NPCID.WyvernHead,
                NPCID.Paladin,
                NPCID.IceGolem,
                NPCID.SandElemental,
                NPCID.BigMimicCorruption,
                NPCID.BigMimicCrimson,
                NPCID.BigMimicHallow,
                NPCID.BloodNautilus,

                // Moon Events.
                NPCID.MourningWood,
                NPCID.Pumpking,
                NPCID.Everscream,
                NPCID.SantaNK1,
                NPCID.IceQueen,

                // Eclipse.
                NPCID.Mothron,

                // Martian Madness.
                NPCID.MartianSaucerCore,

                // Prehardmode Modded.
                NPCType<GiantClam>(),
                NPCType<PerforatorHeadSmall>(),
                NPCType<PerforatorHeadMedium>(),
                NPCType<PerforatorHeadLarge>(),

                // Hardmode Modded.
                NPCType<ThiccWaifu>(),
                NPCType<Horse>(),
                NPCType<GreatSandShark>(),
                NPCType<PlaguebringerMiniboss>(),
                NPCType<ArmoredDiggerHead>(),
                NPCType<Cataclysm>(), //Clone's brothers
                NPCType<Catastrophe>(),

                // Post-ML Modded.
                NPCType<SupremeCataclysm>(),
                NPCType<SupremeCatastrophe>(),
                NPCType<ProvSpawnDefense>(),
                NPCType<ProvSpawnOffense>(),
                NPCType<ProvSpawnHealer>(),
                NPCType<ProfanedGuardianDefender>(),
                NPCType<ProfanedGuardianHealer>()
            };
        }

        public static void SetupExtensionHandlerList()
        {
            EntityExtensionHandler = new Dictionary<int, BossEntityExtension>()
            {
                [NPCID.EaterofWorldsHead] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.Segments"), NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail),
                [NPCID.BrainofCthulhu] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.Creepers"), NPCID.Creeper),
                [NPCID.SkeletronHead] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.Hands"), NPCID.SkeletronHand),
                [NPCID.SkeletronPrime] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.Arms"), NPCID.PrimeCannon, NPCID.PrimeSaw, NPCID.PrimeVice, NPCID.PrimeLaser),
                [NPCID.MartianSaucerCore] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.Guns"), NPCID.MartianSaucerTurret, NPCID.MartianSaucerCannon),
                [NPCID.PirateShip] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.Cannons"), NPCID.PirateShipCannon),
                [NPCType<CeaselessVoid>()] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.DarkEnergy"), NPCType<DarkEnergy>()),
                [NPCType<RavagerBody>()] = new BossEntityExtension(CalamityUtils.GetText("UI.ExtensionName.BodyParts"), NPCType<RavagerClawLeft>(), NPCType<RavagerClawRight>(), NPCType<RavagerLegLeft>(), NPCType<RavagerLegRight>()),
            };
        }

        // Collection simplification looks horrendous in the context of delegate creation.
        // Warnings pertaining to it in this section are disabled as a result.
#pragma warning disable IDE0028 // Simplify collection initialization
        public static void SetupRequirementsList()
        {
            SpecialHPRequirements = new Dictionary<NPCSpecialHPGetRequirement, NPCSpecialHPGetFunction>();
            SpecialHPRequirements.Add(npc => npc.Calamity().SplittingWorm, (npc, checkingForMaxLife) =>
            {
                // Go across the entire worm and accumulate life. The expectation is that the boss follows the linked-list-esque standard
                // where ai variables act as a way of accessing previous/next segments. In this case, the intent is to begin at the head and go to the tail.
                long life = 0L;
                NPC currentSegment = npc;

                // Head ... -> Main.npc[(int)currentSegment.ai[0]] -> currentSegment.whoAmI -> ... Tail.

                // What this is doing is checking if the next segment agrees with the fact that the previous segment is what it is attaching to.
                // If it doesn't, that means that we have reached the end of the worm.
                int failsafeCounter = 0;
                while (Main.npc.IndexInRange((int)currentSegment.ai[0]) && Main.npc[(int)currentSegment.ai[0]].ai[1] == currentSegment.whoAmI)
                {
                    // If a segment is not active for some reason, don't go any further.
                    if (!currentSegment.active)
                        break;

                    life += checkingForMaxLife ? currentSegment.lifeMax : currentSegment.life;
                    currentSegment = Main.npc[(int)currentSegment.ai[0]];

                    failsafeCounter++;
                    if (failsafeCounter > Main.maxNPCs)
                        break;
                }

                return life;
            });

            SpecialHPRequirements.Add(npc => npc.type == NPCID.MoonLordCore, (npc, checkingForMaxLife) =>
            {
                long life = checkingForMaxLife ? npc.lifeMax : npc.life;
                if (npc.ai[0] == 2f)
                    life = 0L;

                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    bool isMoonLordPiece = Main.npc[i].type == NPCID.MoonLordHand || Main.npc[i].type == NPCID.MoonLordHead;
                    if (!Main.npc[i].active || !isMoonLordPiece || Main.npc[i].ai[3] != npc.whoAmI)
                        continue;

                    // Don't count HP towards the total if the NPC is in its dead state.
                    if (Main.npc[i].Calamity().newAI[0] == 1f)
                        continue;

                    life += checkingForMaxLife ? Main.npc[i].lifeMax : Main.npc[i].life;
                }

                return life;
            });
        }
#pragma warning restore IDE0028 // Simplify collection initialization

        public override void Unload()
        {
            BossMainHPBar = null;
            BossComboHPBar = null;
            BossSeperatorBar = null;
            HPBarFont = null;
            Bars = null;
            BossExclusionList = null;
            MinibossHPBarList = null;
            OneToMany = null;
            EntityExtensionHandler = null;
            SpecialHPRequirements = null;
        }

        public override void Update(IBigProgressBar currentBar, ref BigProgressBarInfo info)
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                // Ignore inactive NPCs and NPCs that should not be given a bar, even if it meets other criteria.
                if (!Main.npc[i].active || BossExclusionList.Contains(Main.npc[i].type))
                    continue;

                bool isEoWSegment = Main.npc[i].type == NPCID.EaterofWorldsBody || Main.npc[i].type == NPCID.EaterofWorldsTail;
                if ((Main.npc[i].IsABoss() && !isEoWSegment) || MinibossHPBarList.Contains(Main.npc[i].type) || Main.npc[i].Calamity().CanHaveBossHealthBar)
                    AttemptToAddBar(i);
            }

            for (int i = 0; i < Bars.Count; i++)
            {
                BossHPUI ui = Bars[i];

                // Update the bar.
                ui.Update();

                // Remove the bar if it's done with its closing animation.
                if (ui.CloseAnimationTimer >= BossHPUI.CloseAnimationTime)
                {
                    Bars.RemoveAt(i);
                    i--;
                }
            }
        }

        public static void AttemptToAddBar(int index)
        {
            // Limit the number of bars.
            if (Bars.Count >= MaximumBars)
                return;

            // Do not attempt to create a new bar for an NPC that's already in the list of bars.
            NPC npc = Main.npc[index];
            bool canAddBar = npc.active && npc.life > 0 && Bars.All(b => b.NPCIndex != index) && !npc.Calamity().ShouldCloseHPBar;

            // SPECIAL CASE: Artemis and Apollo should be registered as one boss, as they share HP.
            // Apollo is the only NPC to recieve a bar, with a special overriding name.
            string overridingName = null;
            if (npc.type == NPCType<Artemis>())
                canAddBar = false;
            if (npc.type == NPCType<Apollo>())
                overridingName = CalamityUtils.GetTextValue("UI.ExoTwinsName" + (npc.ModNPC<Apollo>().exoMechdusa ? "Hekate" : "Normal"));

            if (canAddBar)
                Bars.Add(new BossHPUI(index, overridingName));
        }

        public override bool PreventDraw => true;

        public override void Draw(SpriteBatch spriteBatch, IBigProgressBar currentBar, BigProgressBarInfo info)
        {
            int startHeight = 100;
            int x = Main.screenWidth - 420;
            int y = Main.screenHeight - startHeight;
            if (Main.playerInventory || Main.invasionType > 0 || Main.pumpkinMoon || Main.snowMoon || DD2Event.Ongoing || AcidRainEvent.AcidRainEventIsOngoing)
                x -= 250;

            foreach (BossHPUI ui in Bars)
            {
                ui.Draw(spriteBatch, x, y);
                y -= BossHPUI.VerticalOffsetPerBar;
            }
        }

        public class BossHPUI
        {
            public int NPCIndex = -1;
            public int IntendedNPCType = -1;
            public int OpenAnimationTimer;
            public int CloseAnimationTimer;
            public int EnrageTimer;
            public int IncreasingDefenseOrDRTimer;
            public int ComboDamageCountdown;
            public long PreviousLife;
            public long HealthAtStartOfCombo;
            public long InitialMaxLife;
            public string OverridingName = null;
            public NPC AssociatedNPC => Main.npc.IndexInRange(NPCIndex) ? Main.npc[NPCIndex] : null;
            public int NPCType => AssociatedNPC?.type ?? -1;
            public long CombinedNPCLife
            {
                get
                {
                    if (AssociatedNPC is null || !AssociatedNPC.active)
                        return 0L;

                    long life = AssociatedNPC.life;

                    // Immediately check for special logged edge-cases. If one is hit, go with what it says.
                    foreach (KeyValuePair<NPCSpecialHPGetRequirement, NPCSpecialHPGetFunction> requirement in SpecialHPRequirements)
                    {
                        if (requirement.Key(AssociatedNPC))
                            return requirement.Value(AssociatedNPC, false);
                    }

                    // Don't check any further if the NPC is not a part of any one-to-many relationship.
                    if (!OneToMany.ContainsKey(NPCType))
                        return life;

                    // Otherwise, check if any of said relationship NPCs are enraged.
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (!Main.npc[i].active || Main.npc[i].life <= 0 || !OneToMany[NPCType].Contains(Main.npc[i].type))
                            continue;

                        life += Main.npc[i].life;
                    }

                    return life;
                }
            }
            public long CombinedNPCMaxLife
            {
                get
                {
                    if (AssociatedNPC is null || !AssociatedNPC.active)
                        return 0L;

                    long maxLife = AssociatedNPC.lifeMax;

                    // Immediately check for special logged edge-cases. If one is hit, go with what it says.
                    foreach (KeyValuePair<NPCSpecialHPGetRequirement, NPCSpecialHPGetFunction> requirement in SpecialHPRequirements)
                    {
                        if (requirement.Key(AssociatedNPC))
                            return requirement.Value(AssociatedNPC, true);
                    }

                    // Don't check any further if the NPC is not a part of any one-to-many relationship.
                    if (!OneToMany.ContainsKey(NPCType))
                        return maxLife;

                    // Otherwise, check if any of said relationship NPCs are enraged.
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (!Main.npc[i].active || Main.npc[i].life <= 0 || !OneToMany[NPCType].Contains(Main.npc[i].type))
                            continue;

                        maxLife += Main.npc[i].lifeMax;
                    }

                    return maxLife;
                }
            }
            public bool NPCIsEnraged
            {
                get
                {
                    if (AssociatedNPC is null || !AssociatedNPC.active)
                        return false;

                    if (AssociatedNPC.Calamity().CurrentlyEnraged)
                        return true;

                    // Don't check any further if the NPC is not a part of any one-to-many relationship.
                    if (!OneToMany.ContainsKey(NPCType))
                        return false;

                    // Otherwise, check if any of said relationship NPCs are enraged.
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (!Main.npc[i].active || Main.npc[i].life <= 0 || !OneToMany[NPCType].Contains(Main.npc[i].type))
                            continue;

                        if (Main.npc[i].Calamity().CurrentlyEnraged)
                            return true;
                    }
                    return false;
                }
            }
            public bool NPCIsIncreasingDefenseOrDR
            {
                get
                {
                    if (AssociatedNPC is null || !AssociatedNPC.active)
                        return false;

                    if (AssociatedNPC.Calamity().CurrentlyIncreasingDefenseOrDR)
                        return true;

                    // Don't check any further if the NPC is not a part of any one-to-many relationship.
                    if (!OneToMany.ContainsKey(NPCType))
                        return false;

                    // Otherwise, check if any of said relationship NPCs are increasing their defense or DR.
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (!Main.npc[i].active || Main.npc[i].life <= 0 || !OneToMany[NPCType].Contains(Main.npc[i].type))
                            continue;

                        if (Main.npc[i].Calamity().CurrentlyIncreasingDefenseOrDR)
                            return true;
                    }
                    return false;
                }
            }
            public float NPCLifeRatio
            {
                get
                {
                    float lifeRatio = CombinedNPCLife / (float)InitialMaxLife;

                    // Handle division by 0 edge-cases.
                    if (float.IsNaN(lifeRatio) || float.IsInfinity(lifeRatio))
                        return 0f;

                    return lifeRatio;
                }
            }

            public const int MainBarYOffset = 28;
            public const int SeparatorBarYOffset = 18;
            public const int BarMaxWidth = 400;
            public const int OpenAnimationTime = 80;
            public const int CloseAnimationTime = 120;
            public const int EnrageAnimationTime = 120;
            public const int IncreasedDefenseOrDRAnimationTime = 120;
            public const int VerticalOffsetPerBar = 70;
            public const float SmallTextScale = 0.75f;
            public static Color MainColor = new Color(229, 189, 62);
            public static Color MainBorderColour = new Color(197, 127, 46);

            public BossHPUI(int index, string overridingName = null)
            {
                NPCIndex = index;

                // Store the intended NPC type for this bar.
                // This is necessary because it is possible for the NPC slot to become occupied by something else when the NPC becomes
                // inactive. If this happens and the bar isn't done closing, it will simply think that the new occupant is what is should attach to, regardless
                // of if it's actually supposed to have a bar in the first place. By verifying NPC type, we can be sure that this does not happen.
                // And if an NPC of the same type does this, it doesn't matter, because that means it was valid to begin with.
                if (AssociatedNPC != null && AssociatedNPC.active)
                {
                    IntendedNPCType = AssociatedNPC.type;
                    PreviousLife = CombinedNPCLife;
                }
                OverridingName = overridingName;
            }

            public void Update()
            {
                // Handle combos.
                if (CombinedNPCLife != PreviousLife && PreviousLife != 0)
                {
                    // If there's no ongoing combo, begin one.
                    if (ComboDamageCountdown <= 0)
                        HealthAtStartOfCombo = CombinedNPCLife;

                    ComboDamageCountdown = 30;
                }
                PreviousLife = CombinedNPCLife;

                if (ComboDamageCountdown > 0)
                    ComboDamageCountdown--;

                // Update timers as necessary.
                if (AssociatedNPC is null || !AssociatedNPC.active || NPCType != IntendedNPCType || AssociatedNPC.Calamity().ShouldCloseHPBar)
                {
                    EnrageTimer = Utils.Clamp(EnrageTimer - 4, 0, EnrageAnimationTime);
                    IncreasingDefenseOrDRTimer = Utils.Clamp(IncreasingDefenseOrDRTimer - 4, 0, IncreasedDefenseOrDRAnimationTime);
                    CloseAnimationTimer = Utils.Clamp(CloseAnimationTimer + 1, 0, CloseAnimationTime);
                    return;
                }

                OpenAnimationTimer = Utils.Clamp(OpenAnimationTimer + 1, 0, OpenAnimationTime);
                EnrageTimer = Utils.Clamp(EnrageTimer + NPCIsEnraged.ToDirectionInt(), 0, EnrageAnimationTime);
                IncreasingDefenseOrDRTimer = Utils.Clamp(IncreasingDefenseOrDRTimer + NPCIsIncreasingDefenseOrDR.ToDirectionInt(), 0, IncreasedDefenseOrDRAnimationTime);

                if (CombinedNPCMaxLife != 0 && (InitialMaxLife == 0 || InitialMaxLife < CombinedNPCMaxLife))
                    InitialMaxLife = CombinedNPCMaxLife;
            }

            public void Draw(SpriteBatch sb, int x, int y)
            {
                float animationCompletionRatio = MathHelper.Clamp(OpenAnimationTimer / (float)OpenAnimationTime, 0f, 1f);
                if (CloseAnimationTimer > 0)
                    animationCompletionRatio = 1f - MathHelper.Clamp(CloseAnimationTimer / (float)CloseAnimationTime, 0f, 1f);

                float openAnimationFlicker = animationCompletionRatio;
                if (OpenAnimationTimer == 4 || OpenAnimationTimer == 8 || OpenAnimationTimer == 16)
                    openAnimationFlicker = Main.rand.NextFloat(0.7f, 0.8f);
                if (OpenAnimationTimer == 3 || OpenAnimationTimer == 7 || OpenAnimationTimer == 15)
                    openAnimationFlicker = Main.rand.NextFloat(0.4f, 0.5f);

                // Draw the main health bar.
                int mainBarWidth = (int)MathHelper.Min(BarMaxWidth * animationCompletionRatio, BarMaxWidth * NPCLifeRatio);
                sb.Draw(BossMainHPBar, new Rectangle(x, y + MainBarYOffset, mainBarWidth, BossMainHPBar.Height), Color.White);

                // Draw a red damage health bar if performing a conbo.
                if (ComboDamageCountdown > 0)
                {
                    int comboBarWidth = (int)(BarMaxWidth * HealthAtStartOfCombo / (float)InitialMaxLife) - mainBarWidth;
                    float alpha = 1f;

                    // Shrink the bar on the last 6 frames of the damage countdown.
                    if (ComboDamageCountdown < 6)
                        comboBarWidth = (int)(comboBarWidth * ComboDamageCountdown / 6f);

                    sb.Draw(BossComboHPBar, new Rectangle(x + mainBarWidth, y + MainBarYOffset, comboBarWidth, BossComboHPBar.Height), Color.White * alpha);
                }

                // Draw a white separator bar.
                // Enrage bar color takes priority over defense or DR increase bar color, because it's more important to display the enrage.
                Color separatorColor = new Color(240, 240, 255) * animationCompletionRatio;
                if (NPCIsEnraged)
                    separatorColor = Color.Lerp(new Color(240, 240, 255), Color.Red * 0.5f, EnrageTimer / (float)EnrageAnimationTime) * animationCompletionRatio;
                else if (NPCIsIncreasingDefenseOrDR)
                    separatorColor = Color.Lerp(new Color(240, 240, 255), Color.LightGray * 0.5f, IncreasingDefenseOrDRTimer / (float)IncreasedDefenseOrDRAnimationTime) * animationCompletionRatio;

                // Draw the bar.
                sb.Draw(BossSeperatorBar, new Rectangle(x, y + SeparatorBarYOffset, BarMaxWidth, 6), separatorColor);

                // Draw the text.
                string percentHealthText = (NPCLifeRatio * 100).ToString("N1") + "%";
                if (NPCLifeRatio == 0f)
                    percentHealthText = "0%";
                Vector2 textSize = HPBarFont.MeasureString(percentHealthText);

                CalamityUtils.DrawBorderStringEightWay(sb, HPBarFont, percentHealthText, new Vector2(x, y + 22 - textSize.Y), MainColor, MainBorderColour * 0.25f);

                // Draw a red back-glow of the text if the NPC is enraged or a gray back-glow if the NPC is increasing defense or DR.
                string name = OverridingName ?? AssociatedNPC.FullName;

                Vector2 nameSize = FontAssets.MouseText.Value.MeasureString(name);
                if (NPCIsEnraged)
                {
                    if (EnrageTimer > 0)
                    {
                        float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f) * 0.5f + 0.5f;
                        float outwardness = EnrageTimer / (float)EnrageAnimationTime * 1.5f + pulse * 2f;
                        Color color1 = Color.Red * 0.6f;
                        Color color2 = Color.Black * 0.2f;
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 drawOffset = (MathHelper.TwoPi * i / 4f).ToRotationVector2() * outwardness;
                            CalamityUtils.DrawBorderStringEightWay(sb, FontAssets.MouseText.Value, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y) + drawOffset, color1, color2);
                        }
                    }
                }
                else if (NPCIsIncreasingDefenseOrDR)
                {
                    if (IncreasingDefenseOrDRTimer > 0)
                    {
                        float pulse = (float)Math.Sin(Main.GlobalTimeWrappedHourly * 4.5f) * 0.5f + 0.5f;
                        float outwardness = IncreasingDefenseOrDRTimer / (float)IncreasedDefenseOrDRAnimationTime * 1.5f + pulse * 2f;
                        Color color1 = Color.LightGray * 0.6f;
                        Color color2 = Color.Black * 0.2f;
                        for (int i = 0; i < 4; i++)
                        {
                            Vector2 drawOffset = (MathHelper.TwoPi * i / 4f).ToRotationVector2() * outwardness;
                            CalamityUtils.DrawBorderStringEightWay(sb, FontAssets.MouseText.Value, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y) + drawOffset, color1, color2);
                        }
                    }
                }

                // And draw the text to indicate the name of the boss.
                CalamityUtils.DrawBorderStringEightWay(sb, FontAssets.MouseText.Value, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y), Color.White, Color.Black * 0.2f);

                if (CanDrawExtraSmallText)
                {
                    // Draw a smaller bar below for indications of secondary entities, such as servants or appendages.
                    if (EntityExtensionHandler.TryGetValue(NPCType, out BossEntityExtension extraEntityData))
                    {
                        int totalExtraEntities = CalamityUtils.CountNPCsBetter(extraEntityData.TypesToSearchFor);

                        string extensionName = extraEntityData.NameOfExtensions.ToString();
                        string text = CalamityUtils.GetText("UI.ExtensionDisplay").Format(extensionName, totalExtraEntities);
                        Vector2 textAreaSize = FontAssets.ItemStack.Value.MeasureString(text) * SmallTextScale;
                        float horizontalDrawPosition = Math.Max(x, x + mainBarWidth - textAreaSize.X);
                        float verticalDrawPosition = y + MainBarYOffset + 17;
                        Vector2 smallBarDrawPosition = new Vector2(horizontalDrawPosition, verticalDrawPosition);
                        CalamityUtils.DrawBorderStringEightWay(sb, FontAssets.ItemStack.Value, text, smallBarDrawPosition, Color.White * openAnimationFlicker, Color.Black * openAnimationFlicker * 0.24f, SmallTextScale);
                    }

                    // If that isn't necessary, simply display the precise amount of remaining life for the boss.
                    else
                    {
                        // Draw the precise life.
                        string actualLifeText = $"({CombinedNPCLife} / {InitialMaxLife})";
                        Vector2 textAreaSize = FontAssets.ItemStack.Value.MeasureString(actualLifeText) * SmallTextScale;
                        float horizontalDrawPosition = Math.Max(x, x + mainBarWidth - textAreaSize.X);
                        float verticalDrawPosition = y + MainBarYOffset + 17;
                        Vector2 smallBarDrawPosition = new Vector2(horizontalDrawPosition, verticalDrawPosition);
                        CalamityUtils.DrawBorderStringEightWay(sb, FontAssets.ItemStack.Value, actualLifeText, smallBarDrawPosition, Color.White * openAnimationFlicker, Color.Black * openAnimationFlicker * 0.24f, SmallTextScale);
                    }
                }
            }
        }
    }
}
