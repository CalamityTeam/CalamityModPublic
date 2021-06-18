using CalamityMod.Events;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.ExoMechs.Thanatos;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.UI
{
    /*
    Heyo! Here's some things you might need to know about this class and where to change things:

    In the "Load" method is where the "OneToMany" dictionary is updated.
    If an NPC is made up of multiple segments (such as EoW, etc.) that all have separate health but would be considered a single boss, you can update the dictionary.
    The boss health bar will automatically keep track of all segments of that type and add up the health.

    The "SetupExclusionList" method allows you to check for if certain mods are loaded and setup the exclusion list based on what mods are loaded.
    For now I've had to load calamity separately, you might want to change that by just passing in Calamity itself etc.

    If you need to add a special kind of health bar, like the EoW one which counts the number of segments left, let me know.
    I may need to dive into the spaghetti to add it.

    There's a few readonly fields at the top of both classes that you can edit should you deem it fit to. Change the animation lengths, etc.
    The bar uses those and shouldn't have any problems dealing with the new values
    (although the flickering in the start animation will always happen in the first 20 frames or so)

    As for toggling, in the Mod File, you can simply add a boolean check and not draw or update the health bar manager.
    There is a "SHOULD_DRAW_SMALLTEXT_HEALTH" field below here which is public, if that's false then the smalltext won't draw.

    That should be it -- ask if you have any questions!
    */

    internal static class BossHealthBarManager
    {
        private static readonly int MAX_BARS = 4;

        public static bool SHOULD_DRAW_SMALLTEXT_HEALTH = true;

        private static List<BossHPUI> Bars;

        private static DynamicSpriteFont HPBarFont;
        private static Texture2D BossMainHPBar;
        private static Texture2D BossComboHPBar;
        private static Texture2D BossSeperatorBar;

        public static Dictionary<int, int[]> OneToMany;
        private static List<int> ExclusionList;
        public static List<int> MinibossHPBarList;

        public static void Load(Mod mod)
        {
            Bars = new List<BossHPUI>();

            if (!Main.dedServ)
            {
                BossMainHPBar = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/BossHPMainBar");
                BossComboHPBar = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/BossHPComboBar");
                BossSeperatorBar = ModContent.GetTexture("CalamityMod/ExtraTextures/UI/BossHPSeperatorBar");

                PlatformID id = Environment.OSVersion.Platform;
                if (id == PlatformID.Win32NT && !Environment.Is64BitProcess)
                {
                    HPBarFont = mod.GetFont("Fonts/HPBarFont");
                }
                else
                {
                    HPBarFont = Main.fontMouseText;
                }
            }

            OneToMany = new Dictionary<int, int[]>();

            int[] EoW = new int[] { NPCID.EaterofWorldsHead, NPCID.EaterofWorldsBody, NPCID.EaterofWorldsTail };
            OneToMany[NPCID.EaterofWorldsHead] = EoW;
            OneToMany[NPCID.EaterofWorldsBody] = EoW;
            OneToMany[NPCID.EaterofWorldsTail] = EoW;

            int[] BoC = new int[] { NPCID.BrainofCthulhu, NPCID.Creeper };
            OneToMany[NPCID.BrainofCthulhu] = BoC;
            OneToMany[NPCID.Creeper] = BoC;

			int[] PerfWorm = new int[] { ModContent.NPCType<PerforatorHeadMedium>(), ModContent.NPCType<PerforatorBodyMedium>(), ModContent.NPCType<PerforatorTailMedium>() };
			OneToMany[ModContent.NPCType<PerforatorHeadMedium>()] = PerfWorm;
			OneToMany[ModContent.NPCType<PerforatorBodyMedium>()] = PerfWorm;
			OneToMany[ModContent.NPCType<PerforatorTailMedium>()] = PerfWorm;

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
            OneToMany[NPCID.MoonLordHead] = MoonLord;
            OneToMany[NPCID.MoonLordHand] = MoonLord;
            OneToMany[NPCID.MoonLordCore] = MoonLord;

            int[] Void = new int[] { ModContent.NPCType<CeaselessVoid>(), ModContent.NPCType<DarkEnergy>() };
            OneToMany[ModContent.NPCType<CeaselessVoid>()] = Void;
            OneToMany[ModContent.NPCType<DarkEnergy>()] = Void;

            int[] Rav = new int[] { ModContent.NPCType<RavagerBody>(), ModContent.NPCType<RavagerClawRight>(), ModContent.NPCType<RavagerClawLeft>(),
                ModContent.NPCType<RavagerLegRight>(), ModContent.NPCType<RavagerLegLeft>(), ModContent.NPCType<RavagerHead>() };
            OneToMany[ModContent.NPCType<RavagerBody>()] = Rav;
            OneToMany[ModContent.NPCType<RavagerClawRight>()] = Rav;
            OneToMany[ModContent.NPCType<RavagerClawLeft>()] = Rav;
            OneToMany[ModContent.NPCType<RavagerLegRight>()] = Rav;
            OneToMany[ModContent.NPCType<RavagerLegLeft>()] = Rav;
            OneToMany[ModContent.NPCType<RavagerHead>()] = Rav;

            int[] Slimes = new int[] { ModContent.NPCType<SlimeGodCore>(), ModContent.NPCType<SlimeGod>(), ModContent.NPCType<SlimeGodSplit>(),
                ModContent.NPCType<SlimeGodRun>(), ModContent.NPCType<SlimeGodRunSplit>() };
            OneToMany[ModContent.NPCType<SlimeGodCore>()] = Slimes;
            OneToMany[ModContent.NPCType<SlimeGod>()] = Slimes;
            OneToMany[ModContent.NPCType<SlimeGodSplit>()] = Slimes;
            OneToMany[ModContent.NPCType<SlimeGodRun>()] = Slimes;
            OneToMany[ModContent.NPCType<SlimeGodRunSplit>()] = Slimes;

            SetupExclusionList();
            SetupMinibossHPBarList();
        }

        private static void SetupExclusionList()
        {
            ExclusionList = new List<int>
            {
                NPCID.MoonLordFreeEye,
                NPCID.MoonLordHead,
                NPCID.MoonLordHand,
                NPCID.MoonLordCore,
                NPCID.WyvernLegs,
                NPCID.WyvernBody,
                NPCID.WyvernBody2,
                NPCID.WyvernBody3,
                NPCID.WyvernTail,
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>(),
                ModContent.NPCType<AstrumDeusBodySpectral>(),
                ModContent.NPCType<AstrumDeusTailSpectral>(),
                ModContent.NPCType<DesertScourgeBody>(),
                ModContent.NPCType<DesertScourgeTail>(),
                ModContent.NPCType<StormWeaverHead>(),
                ModContent.NPCType<StormWeaverBody>(),
                ModContent.NPCType<StormWeaverTail>(),
                ModContent.NPCType<StormWeaverBodyNaked>(),
                ModContent.NPCType<StormWeaverTailNaked>(),
                ModContent.NPCType<DevourerofGodsBody>(),
                ModContent.NPCType<DevourerofGodsTail>(),
                ModContent.NPCType<DevourerofGodsBodyS>(),
                ModContent.NPCType<DevourerofGodsTailS>(),
				ModContent.NPCType<ThanatosBody1>(),
				ModContent.NPCType<ThanatosBody2>(),
				ModContent.NPCType<ThanatosTail>()
			};
        }

        private static void SetupMinibossHPBarList()
        {
            MinibossHPBarList = new List<int>
            {
                //DD2 Event
                NPCID.DD2Betsy,
                NPCID.DD2OgreT2,
                NPCID.DD2OgreT3,
                NPCID.DD2DarkMageT1, //800 HP
                NPCID.DD2DarkMageT3,

                //Prehardmode
                NPCID.DungeonGuardian,

                //Hardmode
                NPCID.GoblinSummoner, //2000 HP
                NPCID.WyvernHead,
                NPCID.Paladin,
                NPCID.IceGolem,
                NPCID.SandElemental,
                NPCID.BigMimicCorruption,
                NPCID.BigMimicCrimson,
                NPCID.BigMimicHallow,

                //Moon Events
                NPCID.MourningWood,
                NPCID.Pumpking,
                NPCID.Everscream,
                NPCID.SantaNK1,
                NPCID.IceQueen,

                //Eclipse
                NPCID.Mothron,

                //Prehardmode
                ModContent.NPCType<GiantClam>(),
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorHeadLarge>(),

                //Hardmode
                ModContent.NPCType<ThiccWaifu>(),
                ModContent.NPCType<Horse>(),
                ModContent.NPCType<GreatSandShark>(),
                ModContent.NPCType<PlaguebringerShade>(),
                ModContent.NPCType<ArmoredDiggerHead>(),
                ModContent.NPCType<CalamitasRun>(), //Clone's brothers
                ModContent.NPCType<CalamitasRun2>(),

                //Abyss
                ModContent.NPCType<EidolonWyrmHeadHuge>(),

                //Post-ML
                ModContent.NPCType<SupremeCataclysm>(),
                ModContent.NPCType<SupremeCatastrophe>(),
                ModContent.NPCType<ProvSpawnDefense>(),
                ModContent.NPCType<ProvSpawnOffense>(),
                ModContent.NPCType<ProvSpawnHealer>()
            };
            MinibossHPBarList.AddRange(AcidRainEvent.AllMinibosses);
        }

        public static void Unload()
        {
            BossMainHPBar = null;
            BossComboHPBar = null;
            BossSeperatorBar = null;
            HPBarFont = null;
            Bars = null;
            ExclusionList = null;
            MinibossHPBarList = null;
            OneToMany = null;
        }

        public static void Update()
        {
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                //Is NPC active
                if (!Main.npc[i].active)
                    continue;

                //Is npc in exclusion list
                int type = Main.npc[i].type;
                if (Main.npc[i].type == NPCID.MoonLordCore)
                {
                    AttemptAddBar(i, NPCID.MoonLordCore);
                }
                if (ExclusionList.Contains(type))
                    continue;

                if (Main.npc[i].type == NPCID.BrainofCthulhu)
                {
                    AttemptAddBar(i, NPCID.BrainofCthulhu);
                }
                else if (Main.npc[i].type == NPCID.SkeletronHead)
                {
                    AttemptAddBar(i, NPCID.SkeletronHead);
                }
                else if (Main.npc[i].type == NPCID.SkeletronPrime)
                {
                    AttemptAddBar(i, NPCID.SkeletronPrime);
                }
                else if (Main.npc[i].type == NPCID.Golem)
                {
                    AttemptAddBar(i, NPCID.Golem);
                }
                else if (Main.npc[i].type == NPCID.MartianSaucerCore)
                {
                    AttemptAddBar(i, NPCID.MartianSaucerCore);
                }
                else if (Main.npc[i].type == NPCID.PirateShip)
                {
                    AttemptAddBar(i, NPCID.PirateShip);
                }
                else if (Main.npc[i].type == ModContent.NPCType<CalamitasRun3>())
                {
                    AttemptAddBar(i, ModContent.NPCType<CalamitasRun3>());
                }
                else if (Main.npc[i].type == ModContent.NPCType<CeaselessVoid>())
                {
                    AttemptAddBar(i, ModContent.NPCType<CeaselessVoid>());
                }
                else if (Main.npc[i].type == ModContent.NPCType<PerforatorHive>())
                {
                    AttemptAddBar(i, ModContent.NPCType<PerforatorHive>());
                }
                else if (Main.npc[i].type == ModContent.NPCType<RavagerBody>())
                {
                    AttemptAddBar(i, ModContent.NPCType<RavagerBody>());
                }
                else if (Main.npc[i].type == ModContent.NPCType<SlimeGodCore>())
                {
                    AttemptAddBar(i, ModContent.NPCType<SlimeGodCore>());
                }
                else if (Main.npc[i].boss || MinibossHPBarList.Contains(type))
                {
                    AttemptAddBar(i);
                }

                // If, specifically, they're the eater of worlds head
                if (Main.npc[i].type == NPCID.EaterofWorldsHead)
                {
                    AttemptAddBar(i, NPCID.EaterofWorldsHead);
                }
				// If, specifically, they're the medium perf head
				if (Main.npc[i].type == ModContent.NPCType<PerforatorHeadMedium>())
				{
					AttemptAddBar(i, ModContent.NPCType<PerforatorHeadMedium>());
				}
			}

            for (int i = 0; i < Bars.Count; i++)
            {
                BossHPUI ui = Bars[i];
                // Update the bar
                ui.Update();
                // Is the NPC the bar is attached to dead?
                if (ui.IsDead())
                {
                    // Begin closing anim of the UI
                    ui.StartClosing();
                }
                if (ui.ShouldClose())
                {
                    // Remove this bar
                    Bars.RemoveAt(i);
                    i--;
                }
            }
        }

        private static void AttemptAddBar(int index, int type = -1)
        {
            // Limit the number of bars.
            if (Bars.Count >= MAX_BARS)
                return;

            bool hasBar = false;

            foreach (BossHPUI ui in Bars)
            {
                int id = ui.GetNPCID();
                if (id == index)
                {
                    hasBar = true;
                    break;
                }

                // Sort out the eater of worlds or medium perf worm splitting into multiple segments and multiple of them being heads
                if (type == NPCID.EaterofWorldsHead || type == ModContent.NPCType<PerforatorHeadMedium>())
                {
                    if (Main.npc[id].type == NPCID.EaterofWorldsHead || Main.npc[id].type == ModContent.NPCType<PerforatorHeadMedium>())
                    {
                        hasBar = true;
                    }
                }
            }

            if (!hasBar)
            {
                BossHPUI newUI = new BossHPUI(index);

                if (type != -1)
                {
                    newUI.SetupForType(type);
                }

                Bars.Add(newUI);
            }
        }

        public static void Draw(SpriteBatch sb)
        {
            int startHeight = 100; //100
            int heightPerOne = 70; //70

            int y = Main.screenHeight - startHeight;
            int x = Main.screenWidth - 420 + ((Main.playerInventory || Main.invasionType > 0 || Main.pumpkinMoon || Main.snowMoon || DD2Event.Ongoing || CalamityWorld.rainingAcid) ? -250 : 0);

            foreach (BossHPUI ui in Bars)
            {
                ui.Draw(sb, x, y);
                y -= heightPerOne;
            }
        }

        // Actual UI handling class
        internal class BossHPUI
        {
            private static readonly Color OrangeColour = new Color(229, 189, 62);
            private static readonly Color OrangeBorderColour = new Color(197, 127, 46);

            private static readonly int MainBarYOffset = 28;
            private static readonly int SepBarYOffset = 18;
            private static readonly int BarMaxWidth = 400;
            private static readonly int OpenAnimTime = 80;
            private static readonly int CloseAnimTime = 120;

            enum SpecialType : byte
            {
                None,
                EaterOfWorlds,
                Creep,
                Skelet,
                SkeletPrime,
                MartSaucer,
                PirShip
            }

            enum SpecialType2 : byte
            {
                None,
                Ceaseless,
                Ravage,
                SlimeCore,
				MediumPerfWorm
            }

            private int _npcLocation;
            private SpecialType _special = SpecialType.None;
            private SpecialType2 _special2 = SpecialType2.None;
            private int[] _specialData = new int[7];
            private int[] _specialData2 = new int[7];
            private float _maxHealth;
            private int _prevLife;
            private bool _inCombo = false;
            private int _comboStartHealth;
            private int _damageCountdown;
            public int EnrageTimer;

            private NPC _npc
            {
                get
                {
                    return Main.npc[_npcLocation];
                }
            }

            private string _lastName = "";

            private bool _oneToMany = false;
            private int[] _arrayOfIds;

            // EDITABLE
            public void SetupForType(int type)
            {
                if (type == ModContent.NPCType<SlimeGodCore>())
                {
                    _special2 = SpecialType2.SlimeCore;
                }
                else if (type == ModContent.NPCType<RavagerBody>())
                {
                    _special2 = SpecialType2.Ravage;
                }
                else if (type == ModContent.NPCType<CeaselessVoid>())
                {
                    _special2 = SpecialType2.Ceaseless;
                }
                else
                {
                    switch (type)
                    {
                        case NPCID.EaterofWorldsHead:
                            _special = SpecialType.EaterOfWorlds;
                            break;
                        case NPCID.BrainofCthulhu:
                            _special = SpecialType.Creep;
                            break;
                        case NPCID.SkeletronHead:
                            _special = SpecialType.Skelet;
                            break;
                        case NPCID.SkeletronPrime:
                            _special = SpecialType.SkeletPrime;
                            break;
                        case NPCID.MartianSaucerCore:
                            _special = SpecialType.MartSaucer;
                            break;
                        case NPCID.PirateShip:
                            _special = SpecialType.PirShip;
                            break;
                    }
                }
            }

            // ANIMATION FIELDS
            private int _openAnimCounter = OpenAnimTime;
            private int _closeAnimCounter;

            public BossHPUI(int id)
            {
                _npcLocation = id;

                _maxHealth = Main.npc[id].lifeMax;

                if (OneToMany.ContainsKey(Main.npc[id].type))
                {
                    _oneToMany = true;
                    _arrayOfIds = OneToMany[Main.npc[id].type];
                }
            }

            public int GetNPCID()
            {
                return _npcLocation;
            }
            public bool ShouldClose()
            {
                return _closeAnimCounter >= CloseAnimTime;
            }
            public void StartClosing()
            {
                if (_closeAnimCounter == 0)
                    _closeAnimCounter = 1;
            }

            public bool IsDead()
            {
                bool dead = false;

                if (_npc == null)
                    dead = true;

                int life = _npc.life;
                if (life < 0 || !_npc.active || _npc.lifeMax < 800)
                    dead = true;

                if (_oneToMany)
                {
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].life > 0 && _arrayOfIds.Contains(Main.npc[i].type))
                        {
                            dead = false;
                        }
                    }
                }

                if (dead && _lastName == "")
                {
                    _lastName = _npc.FullName;
                }

                return dead;
            }

            public void Update()
            {
                if (_closeAnimCounter != 0)
                    return;

                int currentLife = _npc.life;
                bool enraged = _npc.Calamity().CurrentlyEnraged;

                // Calculate current life based all types that are available and considered part of one boss
                if (_oneToMany)
                {
                    currentLife = 0;
                    int maxLife = 0;
                    for (int i = 0; i < Main.maxNPCs; i++)
                    {
                        if (Main.npc[i].active && Main.npc[i].life > 0 && _arrayOfIds.Contains(Main.npc[i].type))
                        {
                            if ((Main.npc[i].type == NPCID.MoonLordHead && (Main.npc[i].ai[0] == -2f || Main.npc[i].ai[0] == -3f || Main.npc[i].Calamity().newAI[0] == 1f)) ||
                                (Main.npc[i].type == NPCID.MoonLordHand && (Main.npc[i].ai[0] == -2f || Main.npc[i].Calamity().newAI[0] == 1f)) ||
                                (Main.npc[i].type == NPCID.MoonLordCore && Main.npc[i].ai[0] == 2f))
                                continue;

                            if (Main.npc[i].Calamity().CurrentlyEnraged)
                                enraged = true;

                            currentLife += Main.npc[i].life;
                            maxLife += Main.npc[i].lifeMax;
                        }
                    }
                    if (maxLife > _maxHealth)
                    {
                        _maxHealth = maxLife;
                    }
                }

                // Make the enrage counter go up/down based on whether the boss is enraged or not.
                EnrageTimer = Utils.Clamp(EnrageTimer + enraged.ToDirectionInt(), 0, 75);

                // Damage countdown
                if (_damageCountdown > 0)
                {
                    _damageCountdown--;
                    if (_damageCountdown == 0)
                    {
                        // This means we need to finish the combo
                        _comboStartHealth = 0;
                        _inCombo = false;
                    }
                }
                // If the current life is not eqaul to the previous frame of life (_prevLife != 0 ensures it's not on it's first frame)
                if (currentLife != _prevLife && _prevLife != 0)
                {
                    // If there's no ongoing combo
                    if (!_inCombo)
                    {
                        // This means we need to start a combo
                        _comboStartHealth = _prevLife;
                        _inCombo = true;
                    }
                    _damageCountdown = 30; //60
                }
                _prevLife = currentLife;

                switch (_special)
                {
                    default:
                        break;
                    case SpecialType.EaterOfWorlds:
                        // Count the segments of the EoW
                        int count = 0;
                        for (int i = 0; i < Main.maxNPCs; i++)
                        {
                            if (IsEoW(i))
                            {
                                count++;
                            }
                        }
                        _specialData[0] = count;
                        break;
                    case SpecialType.Creep:
                        int count2 = NPC.CountNPCS(NPCID.Creeper);
                        _specialData[1] = count2;
                        break;
                    case SpecialType.Skelet:
                        int count3 = NPC.CountNPCS(NPCID.SkeletronHand);
                        _specialData[2] = count3;
                        break;
                    case SpecialType.SkeletPrime:
                        int count4 = NPC.CountNPCS(NPCID.PrimeVice) + NPC.CountNPCS(NPCID.PrimeCannon) +
                            NPC.CountNPCS(NPCID.PrimeSaw) + NPC.CountNPCS(NPCID.PrimeLaser);
                        _specialData[3] = count4;
                        break;
                    case SpecialType.MartSaucer:
                        int count5 = NPC.CountNPCS(NPCID.MartianSaucerTurret) + NPC.CountNPCS(NPCID.MartianSaucerCannon);
                        _specialData[4] = count5;
                        break;
                    case SpecialType.PirShip:
                        int count6 = NPC.CountNPCS(NPCID.PirateShipCannon);
                        _specialData[5] = count6;
                        break;
                }

                switch (_special2)
                {
                    default:
                        break;
                    case SpecialType2.Ceaseless:
                        int count = NPC.CountNPCS(ModContent.NPCType<DarkEnergy>());
                        _specialData2[0] = count;
                        break;
                    case SpecialType2.Ravage:
                        int count2 = NPC.CountNPCS(ModContent.NPCType<RavagerClawRight>()) +
                            NPC.CountNPCS(ModContent.NPCType<RavagerClawLeft>()) +
                            NPC.CountNPCS(ModContent.NPCType<RavagerLegRight>()) +
                            NPC.CountNPCS(ModContent.NPCType<RavagerLegLeft>()) +
                            NPC.CountNPCS(ModContent.NPCType<RavagerHead>());
                        _specialData2[1] = count2;
                        break;
                    case SpecialType2.SlimeCore:
                        int count3 = NPC.CountNPCS(ModContent.NPCType<SlimeGod>()) +
                            NPC.CountNPCS(ModContent.NPCType<SlimeGodSplit>()) +
                            NPC.CountNPCS(ModContent.NPCType<SlimeGodRun>()) +
                            NPC.CountNPCS(ModContent.NPCType<SlimeGodRunSplit>());
                        _specialData2[2] = count3;
                        break;
				}
            }
            public void Draw(SpriteBatch sb, int x, int y)
            {
                // Draw the respective animations.
                // Easier to separate them, even if it does result in more copy pasted code overall.
                if (_openAnimCounter > 0)
                {
                    DrawOpenAnim(sb, x, y);
                    return;
                }

                if (_closeAnimCounter > 0)
                {
                    DrawCloseAnim(sb, x, y);
                    return;
                }

                float percentHealth = _prevLife / _maxHealth;
                int mainBarWidth = (int)(BarMaxWidth * percentHealth);

                if (_inCombo)
                {
                    // DRAW COMBO HEALTH BAR
                    int comboBarWidth = (int)(BarMaxWidth * (_comboStartHealth / _maxHealth)) - mainBarWidth;
                    float alpha = 1f;
                    if (_damageCountdown < 6)
                    {
                        float val = _damageCountdown * 0.166f;
                        alpha *= val;
                        comboBarWidth = (int)(comboBarWidth * val);
                    }

                    sb.Draw(BossComboHPBar, new Rectangle(x + mainBarWidth, y + MainBarYOffset, comboBarWidth, 15), Color.White * alpha);
                }

                // DRAW MAIN HEALTH BAR
                sb.Draw(BossMainHPBar, new Rectangle(x, y + MainBarYOffset, mainBarWidth, 15), Color.White);

                // DRAW WHITE(ISH) LINE
                Color separatorColor = Color.Lerp(new Color(240, 240, 255), Color.Red * 0.5f, EnrageTimer / 75f);
                sb.Draw(BossSeperatorBar, new Rectangle(x, y + SepBarYOffset, BarMaxWidth, 6), separatorColor);

                // DRAW TEXT
                string percentHealthText = (percentHealth * 100).ToString("N1") + "%";
                if (_prevLife == _maxHealth)
                    percentHealthText = "100%";
                Vector2 textSize = HPBarFont.MeasureString(percentHealthText);

                DrawBorderStringEightWay(sb, HPBarFont, percentHealthText, new Vector2(x, y + 22 - textSize.Y), OrangeColour, OrangeBorderColour * 0.25f);

                string name = _npc.FullName;
                Vector2 nameSize = Main.fontMouseText.MeasureString(name);
                if (EnrageTimer > 0)
                {
                    float pulse = (float)Math.Sin(Main.GlobalTime * 4.5f) * 0.5f + 0.5f;
                    float outwardness = EnrageTimer / 75f * 1.5f + pulse * 2f;
                    for (int i = 0; i < 4; i++)
                    {
                        Vector2 drawOffset = (MathHelper.TwoPi * i / 4f).ToRotationVector2() * outwardness;
                        DrawBorderStringEightWay(sb, Main.fontMouseText, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y) + drawOffset, Color.Red * 0.6f, Color.Black * 0.2f);
                    }
                }
                DrawBorderStringEightWay(sb, Main.fontMouseText, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y), Color.White, Color.Black * 0.2f);

                // TODO -- Make small text health a toggle in ModConfig.
                if (SHOULD_DRAW_SMALLTEXT_HEALTH)
                {
                    float textScale = 0.75f;

                    switch (_special)
                    {
                        default:
                            break;
                        case SpecialType.EaterOfWorlds:
                            string count = "(Segments left: " + _specialData[0] + ")";
                            Vector2 countSize = Main.fontItemStack.MeasureString(count) * textScale;
                            float countX = Math.Max(x, x + mainBarWidth - countSize.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count, new Vector2(countX, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType.Creep:
                            string count2 = "(Creepers left: " + _specialData[1] + ")";
                            Vector2 countSize2 = Main.fontItemStack.MeasureString(count2) * textScale;
                            float countX2 = Math.Max(x, x + mainBarWidth - countSize2.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count2, new Vector2(countX2, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType.Skelet:
                            string count3 = "(Hands left: " + _specialData[2] + ")";
                            Vector2 countSize3 = Main.fontItemStack.MeasureString(count3) * textScale;
                            float countX3 = Math.Max(x, x + mainBarWidth - countSize3.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count3, new Vector2(countX3, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType.SkeletPrime:
                            string count4 = "(Arms left: " + _specialData[3] + ")";
                            Vector2 countSize4 = Main.fontItemStack.MeasureString(count4) * textScale;
                            float countX4 = Math.Max(x, x + mainBarWidth - countSize4.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count4, new Vector2(countX4, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType.MartSaucer:
                            string count5 = "(Guns left: " + _specialData[4] + ")";
                            Vector2 countSize5 = Main.fontItemStack.MeasureString(count5) * textScale;
                            float countX5 = Math.Max(x, x + mainBarWidth - countSize5.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count5, new Vector2(countX5, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType.PirShip:
                            string count6 = "(Cannons left: " + _specialData[5] + ")";
                            Vector2 countSize6 = Main.fontItemStack.MeasureString(count6) * textScale;
                            float countX6 = Math.Max(x, x + mainBarWidth - countSize6.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count6, new Vector2(countX6, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                    }

                    switch (_special2)
                    {
                        default:
                            break;
                        case SpecialType2.Ceaseless:
                            string count2 = "(Dark Energy left: " + _specialData2[0] + ")";
                            Vector2 countSize2 = Main.fontItemStack.MeasureString(count2) * textScale;
                            float countX2 = Math.Max(x, x + mainBarWidth - countSize2.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count2, new Vector2(countX2, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType2.Ravage:
                            string count4 = "(Body Parts left: " + _specialData2[1] + ")";
                            Vector2 countSize4 = Main.fontItemStack.MeasureString(count4) * textScale;
                            float countX4 = Math.Max(x, x + mainBarWidth - countSize4.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count4, new Vector2(countX4, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                        case SpecialType2.SlimeCore:
                            string count5 = "(Large Slimes left: " + _specialData2[2] + ")";
                            Vector2 countSize5 = Main.fontItemStack.MeasureString(count5) * textScale;
                            float countX5 = Math.Max(x, x + mainBarWidth - countSize5.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count5, new Vector2(countX5, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                            return;
                    }

                    string actualLife = "(" + _npc.life + " / " + _npc.lifeMax + ")";
                    Vector2 lifeSize = Main.fontItemStack.MeasureString(actualLife) * textScale;
                    float lifeX = Math.Max(x, x + mainBarWidth - lifeSize.X);
                    DrawBorderStringEightWay(sb, Main.fontItemStack, actualLife, new Vector2(lifeX, y + MainBarYOffset + 17), Color.White, Color.Black * 0.24f, textScale);
                }
            }
            public void DrawOpenAnim(SpriteBatch sb, int x, int y)
            {
                float percentThroughAnim = (OpenAnimTime - _openAnimCounter) / (float)OpenAnimTime;
                int mainBarWidth = (int)(BarMaxWidth * MathHelper.SmoothStep(0f, 1f, percentThroughAnim));

                float flickerValue = percentThroughAnim;
                // FLICKER 3 TIMES, QUICK AND DIRTY METHOD
                if (_openAnimCounter == OpenAnimTime - 4 || _openAnimCounter == OpenAnimTime - 8 || _openAnimCounter == OpenAnimTime - 16)
                {
                    flickerValue = Main.rand.NextFloat(0.7f, 0.8f);
                }
                else if (_openAnimCounter == OpenAnimTime - 5 || _openAnimCounter == OpenAnimTime - 9 || _openAnimCounter == OpenAnimTime - 17)
                {
                    flickerValue = Main.rand.NextFloat(0.4f, 0.5f);
                }

                // DRAW MAIN HEALTH BAR
                sb.Draw(BossMainHPBar, new Rectangle(x, y + MainBarYOffset, mainBarWidth, 15), Color.White * flickerValue);

                // DRAW WHITE(ISH) LINE
                sb.Draw(BossSeperatorBar, new Rectangle(x, y + SepBarYOffset, BarMaxWidth, 6), new Color(240, 240, 255) * flickerValue);

                // DRAW TEXT
                string percentHealthText = "100%";
                Vector2 textSize = HPBarFont.MeasureString(percentHealthText);
                DrawBorderStringEightWay(sb, HPBarFont, percentHealthText, new Vector2(x, y + 22 - textSize.Y), OrangeColour * flickerValue, OrangeBorderColour * 0.25f * flickerValue);

                string name = _npc.FullName;
                Vector2 nameSize = Main.fontMouseText.MeasureString(name);
                DrawBorderStringEightWay(sb, Main.fontMouseText, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y), Color.White * flickerValue, Color.Black * 0.2f * flickerValue);

                if (SHOULD_DRAW_SMALLTEXT_HEALTH)
                {
                    float textScale = 0.75f;

                    switch (_special)
                    {
                        default:
                            break;
                        case SpecialType.EaterOfWorlds:
                            string count = "(Segments left: " + _specialData[0] + ")";
                            Vector2 countSize = Main.fontItemStack.MeasureString(count) * textScale;
                            float countX = Math.Max(x, x + mainBarWidth - countSize.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count, new Vector2(countX, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType.Creep:
                            string count2 = "(Creepers left: " + _specialData[1] + ")";
                            Vector2 countSize2 = Main.fontItemStack.MeasureString(count2) * textScale;
                            float countX2 = Math.Max(x, x + mainBarWidth - countSize2.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count2, new Vector2(countX2, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType.Skelet:
                            string count3 = "(Hands left: " + _specialData[2] + ")";
                            Vector2 countSize3 = Main.fontItemStack.MeasureString(count3) * textScale;
                            float countX3 = Math.Max(x, x + mainBarWidth - countSize3.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count3, new Vector2(countX3, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType.SkeletPrime:
                            string count4 = "(Arms left: " + _specialData[3] + ")";
                            Vector2 countSize4 = Main.fontItemStack.MeasureString(count4) * textScale;
                            float countX4 = Math.Max(x, x + mainBarWidth - countSize4.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count4, new Vector2(countX4, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType.MartSaucer:
                            string count5 = "(Guns left: " + _specialData[4] + ")";
                            Vector2 countSize5 = Main.fontItemStack.MeasureString(count5) * textScale;
                            float countX5 = Math.Max(x, x + mainBarWidth - countSize5.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count5, new Vector2(countX5, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType.PirShip:
                            string count6 = "(Cannons left: " + _specialData[5] + ")";
                            Vector2 countSize6 = Main.fontItemStack.MeasureString(count6) * textScale;
                            float countX6 = Math.Max(x, x + mainBarWidth - countSize6.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count6, new Vector2(countX6, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                    }

                    switch (_special2)
                    {
                        default:
                            break;
                        case SpecialType2.Ceaseless:
                            string count2 = "(Dark Energy left: " + _specialData2[0] + ")";
                            Vector2 countSize2 = Main.fontItemStack.MeasureString(count2) * textScale;
                            float countX2 = Math.Max(x, x + mainBarWidth - countSize2.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count2, new Vector2(countX2, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType2.Ravage:
                            string count4 = "(Body Parts left: " + _specialData2[1] + ")";
                            Vector2 countSize4 = Main.fontItemStack.MeasureString(count4) * textScale;
                            float countX4 = Math.Max(x, x + mainBarWidth - countSize4.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count4, new Vector2(countX4, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                        case SpecialType2.SlimeCore:
                            string count5 = "(Large Slimes left: " + _specialData2[2] + ")";
                            Vector2 countSize5 = Main.fontItemStack.MeasureString(count5) * textScale;
                            float countX5 = Math.Max(x, x + mainBarWidth - countSize5.X);
                            DrawBorderStringEightWay(sb, Main.fontItemStack, count5, new Vector2(countX5, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                            _openAnimCounter--;
                            return;
                    }

                    string actualLife = "(" + _npc.life + " / " + _npc.lifeMax + ")";
                    Vector2 lifeSize = Main.fontItemStack.MeasureString(actualLife) * textScale;
                    float lifeX = Math.Max(x, x + mainBarWidth - lifeSize.X);
                    DrawBorderStringEightWay(sb, Main.fontItemStack, actualLife, new Vector2(lifeX, y + MainBarYOffset + 17), Color.White * flickerValue, Color.Black * 0.24f * flickerValue, textScale);
                }

                _openAnimCounter--;
            }
            public void DrawCloseAnim(SpriteBatch sb, int x, int y)
            {
                float percentThroughAnim = _closeAnimCounter / (float)CloseAnimTime;
                float reversePercent = 1f - percentThroughAnim;

                float percentHealth = _prevLife / _maxHealth;
                if (percentHealth < 0)
                    percentHealth = 0;

                int mainBarWidth = (int)(BarMaxWidth * MathHelper.SmoothStep(0f, 1f, reversePercent) * percentHealth);

                // DRAW MAIN HEALTH BAR
                sb.Draw(BossMainHPBar, new Rectangle(x, y + MainBarYOffset, mainBarWidth, 15), Color.White * reversePercent);

                // DRAW WHITE(ISH) LINE
                sb.Draw(BossSeperatorBar, new Rectangle(x, y + SepBarYOffset, BarMaxWidth, 6), new Color(240, 240, 255) * reversePercent);

                // DRAW TEXT
                string percentHealthText = (percentHealth * 100).ToString("N1") + "%";
                if (_prevLife <= 0)
                    percentHealthText = "0%";
                if (_prevLife == _maxHealth)
                    percentHealthText = "100%";

                Vector2 textSize = HPBarFont.MeasureString(percentHealthText);
                DrawBorderStringEightWay(sb, HPBarFont, percentHealthText, new Vector2(x, y + 22 - textSize.Y), OrangeColour * reversePercent, OrangeBorderColour * 0.25f * reversePercent);

                string name = _lastName;
                Vector2 nameSize = Main.fontMouseText.MeasureString(name);
                DrawBorderStringEightWay(sb, Main.fontMouseText, name, new Vector2(x + BarMaxWidth - nameSize.X, y + 23 - nameSize.Y), Color.White * reversePercent, Color.Black * 0.2f * reversePercent);

                _closeAnimCounter++;
            }

            // UTILS
            private void DrawBorderStringEightWay(SpriteBatch sb, DynamicSpriteFont font, string text, Vector2 position, Color main, Color border, float scale = 1f)
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2 pos = position + new Vector2(x, y);
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        DynamicSpriteFontExtensionMethods.DrawString(sb, font, text, pos, border, 0f, default, scale, SpriteEffects.None, 0f);
                    }
                }
                DynamicSpriteFontExtensionMethods.DrawString(sb, font, text, position, main, 0f, default, scale, SpriteEffects.None, 0f);
            }

            private bool IsEoW(int id)
            {
                NPC n = Main.npc[id];

                if (!n.active || n.life <= 0)
                    return false;

                return n.type == NPCID.EaterofWorldsHead ||
                       n.type == NPCID.EaterofWorldsBody ||
                       n.type == NPCID.EaterofWorldsTail;
            }
		}
    }
}
