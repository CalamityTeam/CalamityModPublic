using CalamityMod.CalPlayer;
using CalamityMod.Localization;
using CalamityMod.MiscModSupport;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.TheDevourerofGods;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Skies;
using CalamityMod.UI;
using CalamityMod.Utilities;
using CalamityMod.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace CalamityMod
{
    public class CalamityMod : Mod
    {
        // Hotkeys
        public static ModHotKey NormalityRelocatorHotKey;
        public static ModHotKey AegisHotKey;
        public static ModHotKey TarraHotKey;
        public static ModHotKey RageHotKey;
        public static ModHotKey AdrenalineHotKey;
        public static ModHotKey AstralTeleportHotKey;
        public static ModHotKey AstralArcanumUIHotkey;
        public static ModHotKey BossBarToggleHotKey;
        public static ModHotKey BossBarToggleSmallTextHotKey;
        public static ModHotKey MomentumCapacitatorHotkey;

        // Boss Spawners
        public static int ghostKillCount = 0;
        public static int sharkKillCount = 0;
        public static int astralKillCount = 0;

        // Textures & Shaders
        public static Texture2D heartOriginal2;
        public static Texture2D heartOriginal;
        public static Texture2D rainOriginal;
        public static Texture2D manaOriginal;
        public static Texture2D carpetOriginal;
        public static Texture2D AstralCactusTexture;
        public static Texture2D AstralCactusGlowTexture;
        public static Texture2D AstralSky;
        public static Effect CustomShader;

        // DR data structure
        public static SortedDictionary<int, float> DRValues;

        // Lists
        public static IList<string> donatorList;
        public static List<int> rangedProjectileExceptionList;
        public static List<int> projectileMinionList;
        public static List<int> enemyImmunityList;
        public static List<int> dungeonEnemyBuffList;
        public static List<int> dungeonProjectileBuffList;
        public static List<int> bossScaleList;
        public static List<int> bossHPScaleList;
        public static List<int> beeEnemyList;
        public static List<int> beeProjectileList;
        public static List<int> hardModeNerfList;
        public static List<int> debuffList;
        public static List<int> fireWeaponList;
        public static List<int> natureWeaponList;
        public static List<int> alcoholList;
        public static List<int> doubleDamageBuffList; //100% buff
        public static List<int> sixtySixDamageBuffList; //66% buff
        public static List<int> fiftyDamageBuffList; //50% buff
        public static List<int> thirtyThreeDamageBuffList; //33% buff
        public static List<int> twentyFiveDamageBuffList; //25% buff
        public static List<int> twentyDamageBuffList; //20% buff
        public static List<int> weaponAutoreuseList;
        public static List<int> quarterDamageNerfList; //25% nerf
        public static List<int> pumpkinMoonBuffList;
        public static List<int> frostMoonBuffList;
        public static List<int> eclipseBuffList;
        public static List<int> eventProjectileBuffList;
        public static List<int> revengeanceEnemyBuffList;
        public static List<int> revengeanceProjectileBuffList;
        public static List<int> revengeanceLifeStealExceptionList;
        public static List<int> movementImpairImmuneList;
        public static List<int> trapProjectileList;
        public static List<int> scopedWeaponList;
        public static List<int> trueMeleeBoostExceptionList;
        public static List<int> tableList;
        public static List<int> chairList;
        public static List<int> lightList;
        public static List<int> doorList;

        public static List<int> zombieList;
        public static List<int> demonEyeList;
        public static List<int> skeletonList;
        public static List<int> angryBonesList;
        public static List<int> hornetList;
        public static List<int> mossHornetList;

        #region Load
        public override void Load()
        {
            heartOriginal2 = Main.heartTexture;
            heartOriginal = Main.heart2Texture;
            rainOriginal = Main.rainTexture;
            manaOriginal = Main.manaTexture;
            carpetOriginal = Main.flyingCarpetTexture;

            NormalityRelocatorHotKey = RegisterHotKey("Normality Relocator", "Z");
            RageHotKey = RegisterHotKey("Rage Mode", "V");
            AdrenalineHotKey = RegisterHotKey("Adrenaline Mode", "B");
            AegisHotKey = RegisterHotKey("Elysian Guard", "N");
            TarraHotKey = RegisterHotKey("Armor Set Bonus", "Y");
            AstralTeleportHotKey = RegisterHotKey("Astral Teleport", "P");
            AstralArcanumUIHotkey = RegisterHotKey("Astral Arcanum UI Toggle", "O");
            MomentumCapacitatorHotkey = RegisterHotKey("Momentom Capacitater Effect", "U");
            BossBarToggleHotKey = RegisterHotKey("Boss Health Bar Toggle", "NumPad0");
            BossBarToggleSmallTextHotKey = RegisterHotKey("Boss Health Bar Small Text Toggle", "NumPad1");

            if (!Main.dedServ)
            {
                LoadClient();
            }

            BossHealthBarManager.Load(this);

            Config.Load();

            SetupLists();
            SetupVanillaDR();
            SetupThoriumBossDR(ModLoader.GetMod("ThoriumMod"));

            CalamityLocalization.AddLocalizations();
        }

        private void LoadClient()
        {
            AddEquipTexture(new Items.Armor.AbyssalDivingSuitHead(), null, EquipType.Head, "AbyssalDivingSuitHead", "CalamityMod/Items/Armor/AbyssalDivingSuit_Head");
            AddEquipTexture(new Items.Armor.AbyssalDivingSuitBody(), null, EquipType.Body, "AbyssalDivingSuitBody", "CalamityMod/Items/Armor/AbyssalDivingSuit_Body", "CalamityMod/Items/Armor/AbyssalDivingSuit_Arms");
            AddEquipTexture(new Items.Armor.AbyssalDivingSuitLegs(), null, EquipType.Legs, "AbyssalDivingSuitLeg", "CalamityMod/Items/Armor/AbyssalDivingSuit_Legs");

            AddEquipTexture(new Items.Armor.SirenHead(), null, EquipType.Head, "SirenHead", "CalamityMod/Items/Armor/SirenTrans_Head");
            AddEquipTexture(new Items.Armor.SirenBody(), null, EquipType.Body, "SirenBody", "CalamityMod/Items/Armor/SirenTrans_Body", "CalamityMod/Items/Armor/SirenTrans_Arms");
            AddEquipTexture(new Items.Armor.SirenLegs(), null, EquipType.Legs, "SirenLeg", "CalamityMod/Items/Armor/SirenTrans_Legs");

            AddEquipTexture(new Items.Armor.SirenHeadAlt(), null, EquipType.Head, "SirenHeadAlt", "CalamityMod/Items/Armor/SirenTransAlt_Head");
            AddEquipTexture(new Items.Armor.SirenBodyAlt(), null, EquipType.Body, "SirenBodyAlt", "CalamityMod/Items/Armor/SirenTransAlt_Body", "CalamityMod/Items/Armor/SirenTransAlt_Arms");
            AddEquipTexture(new Items.Armor.SirenLegsAlt(), null, EquipType.Legs, "SirenLegAlt", "CalamityMod/Items/Armor/SirenTransAlt_Legs");

            AddEquipTexture(new Items.Permafrost.PopoHead(), null, EquipType.Head, "PopoHead", "CalamityMod/Items/Permafrost/Popo_Head");
            AddEquipTexture(new Items.Permafrost.PopoNoselessHead(), null, EquipType.Head, "PopoNoselessHead", "CalamityMod/Items/Permafrost/PopoNoseless_Head");
            AddEquipTexture(new Items.Permafrost.PopoBody(), null, EquipType.Body, "PopoBody", "CalamityMod/Items/Permafrost/Popo_Body", "CalamityMod/Items/Permafrost/Popo_Arms");
            AddEquipTexture(new Items.Permafrost.PopoLegs(), null, EquipType.Legs, "PopoLeg", "CalamityMod/Items/Permafrost/Popo_Legs");

            AstralCactusTexture = GetTexture("ExtraTextures/Tiles/AstralCactus");
            AstralCactusGlowTexture = GetTexture("ExtraTextures/Tiles/AstralCactusGlow");
            AstralSky = GetTexture("ExtraTextures/AstralSky");
            CustomShader = GetEffect("Effects/CustomShader");

            Filters.Scene["CalamityMod:DevourerofGodsHead"] = new Filter(new DoGScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:DevourerofGodsHead"] = new DoGSky();

            Filters.Scene["CalamityMod:DevourerofGodsHeadS"] = new Filter(new DoGScreenShaderDataS("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:DevourerofGodsHeadS"] = new DoGSkyS();

            Filters.Scene["CalamityMod:CalamitasRun3"] = new Filter(new CalScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.6f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:CalamitasRun3"] = new CalSky();

            Filters.Scene["CalamityMod:PlaguebringerGoliath"] = new Filter(new PbGScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.6f, 0.2f).UseOpacity(0.35f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:PlaguebringerGoliath"] = new PbGSky();

            Filters.Scene["CalamityMod:Yharon"] = new Filter(new YScreenShaderData("FilterMiniTower").UseColor(1f, 0.4f, 0f).UseOpacity(0.75f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Yharon"] = new YSky();

            Filters.Scene["CalamityMod:Leviathan"] = new Filter(new LevScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0.5f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Leviathan"] = new LevSky();

            Filters.Scene["CalamityMod:Providence"] = new Filter(new ProvScreenShaderData("FilterMiniTower").UseColor(0.45f, 0.4f, 0.2f).UseOpacity(0.5f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Providence"] = new ProvSky();

            Filters.Scene["CalamityMod:SupremeCalamitas"] = new Filter(new SCalScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.65f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:SupremeCalamitas"] = new SCalSky();

            Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(CustomShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);
            SkyManager.Instance["CalamityMod:Astral"] = new AstralSky();

            UIHandler.OnLoad(this);
            AstralArcanumUI.Load(this);
        }
        #endregion

        #region Unload
        public override void Unload()
        {
            NormalityRelocatorHotKey = null;
            RageHotKey = null;
            AdrenalineHotKey = null;
            AegisHotKey = null;
            TarraHotKey = null;
            AstralTeleportHotKey = null;
            AstralArcanumUIHotkey = null;
            MomentumCapacitatorHotkey = null;
            BossBarToggleHotKey = null;
            BossBarToggleSmallTextHotKey = null;

            AstralCactusTexture = null;
            AstralCactusGlowTexture = null;
            AstralSky = null;

            DRValues?.Clear();
            DRValues = null;

            donatorList = null;
            rangedProjectileExceptionList = null;
            projectileMinionList = null;
            enemyImmunityList = null;
            dungeonEnemyBuffList = null;
            dungeonProjectileBuffList = null;
            bossScaleList = null;
            bossHPScaleList = null;
            beeEnemyList = null;
            beeProjectileList = null;
            hardModeNerfList = null;
            debuffList = null;
            fireWeaponList = null;
            natureWeaponList = null;
            alcoholList = null;
            doubleDamageBuffList = null;
            sixtySixDamageBuffList = null;
            fiftyDamageBuffList = null;
            thirtyThreeDamageBuffList = null;
            twentyFiveDamageBuffList = null;
            weaponAutoreuseList = null;
            quarterDamageNerfList = null;
            pumpkinMoonBuffList = null;
            frostMoonBuffList = null;
            eclipseBuffList = null;
            eventProjectileBuffList = null;
            revengeanceEnemyBuffList = null;
            revengeanceProjectileBuffList = null;
            revengeanceLifeStealExceptionList = null;
            movementImpairImmuneList = null;
            trapProjectileList = null;
            scopedWeaponList = null;
            trueMeleeBoostExceptionList = null;
            tableList = null;
            chairList = null;
            lightList = null;
            doorList = null;

            zombieList = null;
            demonEyeList = null;
            skeletonList = null;
            angryBonesList = null;
            hornetList = null;
            mossHornetList = null;

            BossHealthBarManager.Unload();
            base.Unload();

            AstralArcanumUI.Unload();
            base.Unload();

            if (!Main.dedServ)
            {
                Main.heartTexture = heartOriginal2;
                Main.heart2Texture = heartOriginal;
                Main.rainTexture = rainOriginal;
                Main.manaTexture = manaOriginal;
                Main.flyingCarpetTexture = carpetOriginal;
            }

            heartOriginal2 = null;
            heartOriginal = null;
            rainOriginal = null;
            manaOriginal = null;
            carpetOriginal = null;
        }
        #endregion

        #region SetupLists
        public static void SetupLists()
        {
            Mod calamity = ModLoader.GetMod("CalamityMod");
            Mod thorium = ModLoader.GetMod("ThoriumMod");
            if (calamity != null)
            {
                donatorList = new List<string>()
                {
                    "Vorbis",
                    "SoloMael",
                    "Chaotic Reks",
                    "The Buildmonger",
                    "Yuh",
                    "Littlepiggy",
                    "LompL",
                    "Lilith Saintclaire",
                    "Ben Shapiro",
                    "Frederik Henschel",
                    "Faye",
                    "Gibb50",
                    "Braden Hajer",
                    "Hannes Holmlund",
                    "profoundmango69",
                    "Jack M Sargent",
                    "Hans Volter",
                    "Krankwagon",
                    "MishiroUsui",
                    "pixlgray",
                    "Arkhine",
                    "Lodude",
                    "DevAesthetic",
                    "Mister Winchester",
                    "Zacky",
                    "Veine",
                    "Javyz",
                    "Shifter",
                    "Crysthamyr",
                    "Elfinlocks",
                    "Ein",
                    "2Larry2",
                    "Jenonen",
                    "Dodu",
                    "Arti",
                    "Tervastator",
                    "Luis Arguello",
                    "Alexander Davis",
                    "BakaQing",
                    "Laura Coonrod",
                    "Xaphlactus",
                    "MajinBagel",
                    "Bendy",
                    "Rando Calrissian",
                    "Tails the Fox 92",
                    "Bread",
                    "Minty Candy",
                    "Preston Card",
                    "MovingTarget_086",
                    "Shiro",
                    "Chip",
                    "Taylor Riverpaw",
                    "ShotgunAngel",
                    "Sandblast",
                    "ThomasThePencil",
                    "Aero (Aero#4599)",
                    "GlitchOut",
                    "Daawnz",
                    "CrabBar",
                    "Yatagarasu",
                    "Jarod Isaac Gordon",
                    "Zombieh",
                    "MingWhy",
                    "Random Weeb",
                    "Ahmed Fahad Zamel Al Sharif",
                    "Eragon3942",
                    "TheBlackHand",
                    "william",
                    "Samuel Foreman",
                    "Christopher Pham",
                    "DemoN K!ng",
                    "Malik Ciaramella",
                    "Ryan Baker-Ortiz",
                    "Aleksanders Denisovs",
                    "TheSilverGhost",
                    "Lucazii",
                    "Shay",
                    "Prism",
                    "BobIsNotMyRealName",
                    "Guwahavel",
                    "Azura",
                    "Joshua Miranda",
                    "Doveda",
                    "William Chang",
                    "Arche",
                    "DevilSunrise",
                    "Yanmei",
                    "Chaos",
                    "Ryan Tucker",
                    "Fish Repairs",
                    "Melvin Brouwers",
                    "Vroomy Has -3,000 IQ",
                    "The Goliath",
                    "DaPyRo"
                };

                rangedProjectileExceptionList = new List<int>()
                {
                    ProjectileID.Phantasm,
                    ProjectileID.VortexBeater,
                    ProjectileID.DD2PhoenixBow,
                    ProjectileID.IchorDart,
                    ProjectileID.PhantasmArrow,
                    calamity.ProjectileType("Phangasm"),
                    calamity.ProjectileType("Contagion"),
                    calamity.ProjectileType("DaemonsFlame"),
                    calamity.ProjectileType("ExoTornado"),
                    calamity.ProjectileType("Drataliornus"),
                    calamity.ProjectileType("FlakKrakenGun"),
                    calamity.ProjectileType("Butcher"),
                    calamity.ProjectileType("StarfleetMK2"),
                    calamity.ProjectileType("TerraBulletSplit"),
                    calamity.ProjectileType("TerraArrow2"),
                    calamity.ProjectileType("OMGWTH"),
                    calamity.ProjectileType("Norfleet"),
                    calamity.ProjectileType("NorfleetComet"),
                    calamity.ProjectileType("NorfleetExplosion")
                };

                projectileMinionList = new List<int>()
                {
                    ProjectileID.PygmySpear,
                    ProjectileID.UFOMinion,
                    ProjectileID.UFOLaser,
                    ProjectileID.StardustCellMinionShot,
                    ProjectileID.MiniSharkron,
                    ProjectileID.MiniRetinaLaser,
                    ProjectileID.ImpFireball,
                    ProjectileID.HornetStinger,
                    ProjectileID.DD2FlameBurstTowerT1Shot,
                    ProjectileID.DD2FlameBurstTowerT2Shot,
                    ProjectileID.DD2FlameBurstTowerT3Shot,
                    ProjectileID.DD2BallistraProj,
                    ProjectileID.DD2ExplosiveTrapT1Explosion,
                    ProjectileID.DD2ExplosiveTrapT2Explosion,
                    ProjectileID.DD2ExplosiveTrapT3Explosion,
                    ProjectileID.SpiderEgg,
                    ProjectileID.BabySpider,
                    ProjectileID.FrostBlastFriendly,
                    ProjectileID.MoonlordTurretLaser,
                    ProjectileID.RainbowCrystalExplosion
                };

                enemyImmunityList = new List<int>()
                {
                    NPCID.KingSlime,
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.BrainofCthulhu,
                    NPCID.Creeper,
                    NPCID.EyeofCthulhu,
                    NPCID.QueenBee,
                    NPCID.SkeletronHead,
                    NPCID.SkeletronHand,
                    NPCID.WallofFlesh,
                    NPCID.WallofFleshEye,
                    NPCID.Retinazer,
                    NPCID.Spazmatism,
                    NPCID.SkeletronPrime,
                    NPCID.PrimeCannon,
                    NPCID.PrimeSaw,
                    NPCID.PrimeLaser,
                    NPCID.PrimeVice,
                    NPCID.Plantera,
                    NPCID.IceQueen,
                    NPCID.Pumpking,
                    NPCID.Mothron,
                    NPCID.Golem,
                    NPCID.GolemHead,
                    NPCID.GolemFistRight,
                    NPCID.GolemFistLeft,
                    NPCID.DukeFishron,
                    NPCID.CultistBoss,
                    NPCID.MoonLordHead,
                    NPCID.MoonLordHand,
                    NPCID.MoonLordCore,
                    NPCID.MoonLordFreeEye,
                    NPCID.DD2Betsy
                };

                dungeonEnemyBuffList = new List<int>()
                {
                    NPCID.SkeletonSniper,
                    NPCID.TacticalSkeleton,
                    NPCID.SkeletonCommando,
                    NPCID.Paladin,
                    NPCID.GiantCursedSkull,
                    NPCID.BoneLee,
                    NPCID.DiabolistWhite,
                    NPCID.DiabolistRed,
                    NPCID.NecromancerArmored,
                    NPCID.Necromancer,
                    NPCID.RaggedCasterOpenCoat,
                    NPCID.RaggedCaster,
                    NPCID.HellArmoredBonesSword,
                    NPCID.HellArmoredBonesMace,
                    NPCID.HellArmoredBonesSpikeShield,
                    NPCID.HellArmoredBones,
                    NPCID.BlueArmoredBonesSword,
                    NPCID.BlueArmoredBonesNoPants,
                    NPCID.BlueArmoredBonesMace,
                    NPCID.BlueArmoredBones,
                    NPCID.RustyArmoredBonesSwordNoArmor,
                    NPCID.RustyArmoredBonesSword,
                    NPCID.RustyArmoredBonesFlail,
                    NPCID.RustyArmoredBonesAxe
                };

                dungeonProjectileBuffList = new List<int>()
                {
                    ProjectileID.PaladinsHammerHostile,
                    ProjectileID.ShadowBeamHostile,
                    ProjectileID.InfernoHostileBolt,
                    ProjectileID.InfernoHostileBlast,
                    ProjectileID.LostSoulHostile,
                    ProjectileID.SniperBullet,
                    ProjectileID.RocketSkeleton,
                    ProjectileID.BulletDeadeye,
                    ProjectileID.Shadowflames
                };

                bossScaleList = new List<int>()
                {
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.Creeper,
                    NPCID.SkeletronHand,
                    NPCID.WallofFleshEye,
                    NPCID.TheHungry,
                    NPCID.TheHungryII,
                    NPCID.TheDestroyerBody,
                    NPCID.TheDestroyerTail,
                    NPCID.PrimeCannon,
                    NPCID.PrimeVice,
                    NPCID.PrimeSaw,
                    NPCID.PrimeLaser,
                    NPCID.PlanterasTentacle,
                    NPCID.Pumpking,
                    NPCID.IceQueen,
                    NPCID.Mothron,
                    NPCID.GolemHead
                };

                bossHPScaleList = new List<int>()
                {
                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.SkeletronHand,
                    NPCID.WallofFleshEye,
                    NPCID.TheDestroyerBody,
                    NPCID.TheDestroyerTail,
                    NPCID.PrimeCannon,
                    NPCID.PrimeLaser,
                    NPCID.PrimeVice,
                    NPCID.PrimeSaw,
                    NPCID.GolemHead,
                    NPCID.GolemHeadFree,
                    NPCID.GolemFistRight,
                    NPCID.GolemFistLeft,
                    NPCID.MoonLordHead,
                    NPCID.MoonLordHand
                };

                beeEnemyList = new List<int>()
                {
                    NPCID.GiantMossHornet,
                    NPCID.BigMossHornet,
                    NPCID.LittleMossHornet,
                    NPCID.TinyMossHornet,
                    NPCID.MossHornet,
                    NPCID.VortexHornetQueen,
                    NPCID.VortexHornet,
                    NPCID.Bee,
                    NPCID.BeeSmall,
                    NPCID.QueenBee,
                    calamity.NPCType("PlaguebringerGoliath"),
                    calamity.NPCType("PlaguebringerShade"),
                    calamity.NPCType("PlagueBeeLargeG"),
                    calamity.NPCType("PlagueBeeLarge"),
                    calamity.NPCType("PlagueBeeG"),
                    calamity.NPCType("PlagueBee")
                };

                beeProjectileList = new List<int>()
                {
                    ProjectileID.Stinger,
                    ProjectileID.HornetStinger,
                    calamity.ProjectileType("PlagueStingerGoliath"),
                    calamity.ProjectileType("PlagueStingerGoliathV2"),
                    calamity.ProjectileType("PlagueExplosion")
                };

                hardModeNerfList = new List<int>()
                {
                    ProjectileID.WebSpit,
                    ProjectileID.PinkLaser,
                    ProjectileID.FrostBlastHostile,
                    ProjectileID.RuneBlast,
                    ProjectileID.GoldenShowerHostile,
                    ProjectileID.RainNimbus,
                    ProjectileID.Stinger,
                    ProjectileID.FlamingArrow,
                    ProjectileID.BulletDeadeye,
                    ProjectileID.CannonballHostile
                };

                debuffList = new List<int>()
                {
                    BuffID.Poisoned,
                    BuffID.Darkness,
                    BuffID.Cursed,
                    BuffID.OnFire,
                    BuffID.Bleeding,
                    BuffID.Confused,
                    BuffID.Slow,
                    BuffID.Weak,
                    BuffID.Silenced,
                    BuffID.BrokenArmor,
                    BuffID.CursedInferno,
                    BuffID.Frostburn,
                    BuffID.Chilled,
                    BuffID.Frozen,
                    BuffID.Burning,
                    BuffID.Suffocation,
                    BuffID.Ichor,
                    BuffID.Venom,
                    BuffID.Blackout,
                    BuffID.Electrified,
                    BuffID.Rabies,
                    BuffID.Webbed,
                    BuffID.Stoned,
                    BuffID.Dazed,
                    BuffID.VortexDebuff,
                    BuffID.WitheredArmor,
                    BuffID.WitheredWeapon,
                    BuffID.OgreSpit,
                    BuffID.BetsysCurse,
                    calamity.BuffType("Shadowflame"),
                    calamity.BuffType("BrimstoneFlames"),
                    calamity.BuffType("BurningBlood"),
                    calamity.BuffType("GlacialState"),
                    calamity.BuffType("GodSlayerInferno"),
                    calamity.BuffType("AstralInfectionDebuff"),
                    calamity.BuffType("HolyLight"),
                    calamity.BuffType("Irradiated"),
                    calamity.BuffType("Plague"),
                    calamity.BuffType("AbyssalFlames"),
                    calamity.BuffType("CrushDepth"),
                    calamity.BuffType("Horror"),
                    calamity.BuffType("MarkedforDeath")
                };

                fireWeaponList = new List<int>()
                {
                    ItemID.FieryGreatsword,
                    ItemID.DD2SquireDemonSword,
                    ItemID.TheHorsemansBlade,
                    ItemID.DD2SquireBetsySword,
                    ItemID.Cascade,
                    ItemID.HelFire,
                    ItemID.MonkStaffT2,
                    ItemID.Flamarang,
                    ItemID.MoltenFury,
                    ItemID.Sunfury,
                    ItemID.PhoenixBlaster,
                    ItemID.Flamelash,
                    ItemID.SolarEruption,
                    ItemID.DayBreak,
                    ItemID.MonkStaffT3,
                    ItemID.HellwingBow,
                    ItemID.DD2PhoenixBow,
                    ItemID.DD2BetsyBow,
                    ItemID.FlareGun,
                    ItemID.Flamethrower,
                    ItemID.EldMelter,
                    ItemID.FlowerofFire,
                    ItemID.MeteorStaff,
                    ItemID.ApprenticeStaffT3,
                    ItemID.InfernoFork,
                    ItemID.HeatRay,
                    ItemID.BookofSkulls,
                    ItemID.ImpStaff,
                    ItemID.DD2FlameburstTowerT1Popper,
                    ItemID.DD2FlameburstTowerT2Popper,
                    ItemID.DD2FlameburstTowerT3Popper,
                    ItemID.MolotovCocktail,
                    calamity.ItemType("AegisBlade"),
                    calamity.ItemType("BalefulHarvester"),
                    calamity.ItemType("Chaotrix"),
                    calamity.ItemType("CometQuasher"),
                    calamity.ItemType("DraconicDestruction"),
                    calamity.ItemType("Drataliornus"),
                    calamity.ItemType("EnergyStaff"),
                    calamity.ItemType("ExsanguinationLance"),
                    calamity.ItemType("FirestormCannon"),
                    calamity.ItemType("FlameburstShortsword"),
                    calamity.ItemType("FlameScythe"),
                    calamity.ItemType("FlameScytheMelee"),
                    calamity.ItemType("FlareBolt"),
                    calamity.ItemType("FlarefrostBlade"),
                    calamity.ItemType("FlarewingBow"),
                    calamity.ItemType("ForbiddenSun"),
                    calamity.ItemType("FrigidflashBolt"),
                    calamity.ItemType("GreatbowofTurmoil"),
                    calamity.ItemType("HarvestStaff"),
                    calamity.ItemType("Hellborn"),
                    calamity.ItemType("HellBurst"),
                    calamity.ItemType("HellfireFlamberge"),
                    calamity.ItemType("Hellkite"),
                    calamity.ItemType("HellwingStaff"),
                    calamity.ItemType("Helstorm"),
                    calamity.ItemType("InfernaCutter"),
                    calamity.ItemType("Lazhar"),
                    calamity.ItemType("MeteorFist"),
                    calamity.ItemType("Mourningstar"),
                    calamity.ItemType("PhoenixBlade"),
                    calamity.ItemType("Photoviscerator"),
                    calamity.ItemType("RedSun"),
                    calamity.ItemType("SpectralstormCannon"),
                    calamity.ItemType("SunGodStaff"),
                    calamity.ItemType("SunSpiritStaff"),
                    calamity.ItemType("TearsofHeaven"),
                    calamity.ItemType("TerraFlameburster"),
                    calamity.ItemType("TheEmpyrean"),
                    calamity.ItemType("TheLastMourning"),
                    calamity.ItemType("TheWand"),
                    calamity.ItemType("VenusianTrident"),
                    calamity.ItemType("Vesuvius"),
                    calamity.ItemType("BlissfulBombardier"),
                    calamity.ItemType("HolyCollider"),
                    calamity.ItemType("MoltenAmputator"),
                    calamity.ItemType("PurgeGuzzler"),
                    calamity.ItemType("SolarFlare"),
                    calamity.ItemType("TelluricGlare"),
                    calamity.ItemType("AngryChickenStaff"),
                    calamity.ItemType("ChickenCannon"),
                    calamity.ItemType("DragonRage"),
                    calamity.ItemType("DragonsBreath"),
                    calamity.ItemType("PhoenixFlameBarrage"),
                    calamity.ItemType("ProfanedTrident"),
                    calamity.ItemType("TheBurningSky"),
                    calamity.ItemType("HeliumFlash")
                };

                natureWeaponList = new List<int>()
                {
                    ItemID.BladeofGrass,
                    ItemID.ChlorophyteClaymore,
                    ItemID.ChlorophyteSaber,
                    ItemID.ChlorophytePartisan,
                    ItemID.ChlorophyteShotbow,
                    ItemID.Seedler,
                    ItemID.ChristmasTreeSword,
                    ItemID.TerraBlade,
                    ItemID.JungleYoyo,
                    ItemID.Yelets,
                    ItemID.MushroomSpear,
                    ItemID.ThornChakram,
                    ItemID.Bananarang,
                    ItemID.FlowerPow,
                    ItemID.BeesKnees,
                    ItemID.Toxikarp,
                    ItemID.Bladetongue,
                    ItemID.PoisonStaff,
                    ItemID.VenomStaff,
                    ItemID.StaffofEarth,
                    ItemID.BeeGun,
                    ItemID.LeafBlower,
                    ItemID.WaspGun,
                    ItemID.CrystalSerpent,
                    ItemID.Razorpine,
                    ItemID.HornetStaff,
                    ItemID.QueenSpiderStaff,
                    ItemID.SlimeStaff,
                    ItemID.PygmyStaff,
                    ItemID.RavenStaff,
                    ItemID.BatScepter,
                    ItemID.SpiderStaff,
                    ItemID.Beenade,
                    ItemID.FrostDaggerfish,
                    calamity.ItemType("DepthBlade"),
                    calamity.ItemType("AbyssBlade"),
                    calamity.ItemType("NeptunesBounty"),
                    calamity.ItemType("AquaticDissolution"),
                    calamity.ItemType("ArchAmaryllis"),
                    calamity.ItemType("ThornBlossom"),
                    calamity.ItemType("BiomeBlade"),
                    calamity.ItemType("TrueBiomeBlade"),
                    calamity.ItemType("OmegaBiomeBlade"),
                    calamity.ItemType("BladedgeGreatbow"),
                    calamity.ItemType("BlossomFlux"),
                    calamity.ItemType("EvergladeSpray"),
                    calamity.ItemType("FeralthornClaymore"),
                    calamity.ItemType("Floodtide"),
                    calamity.ItemType("FourSeasonsGalaxia"),
                    calamity.ItemType("GammaFusillade"),
                    calamity.ItemType("GleamingMagnolia"),
                    calamity.ItemType("HarvestStaff"),
                    calamity.ItemType("HellionFlowerSpear"),
                    calamity.ItemType("Lazhar"),
                    calamity.ItemType("LifefruitScythe"),
                    calamity.ItemType("ManaRose"),
                    calamity.ItemType("MangroveChakram"),
                    calamity.ItemType("MangroveChakramMelee"),
                    calamity.ItemType("MantisClaws"),
                    calamity.ItemType("Mariana"),
                    calamity.ItemType("Mistlestorm"),
                    calamity.ItemType("Monsoon"),
                    calamity.ItemType("Alluvion"),
                    calamity.ItemType("Needler"),
                    calamity.ItemType("NettlelineGreatbow"),
                    calamity.ItemType("Quagmire"),
                    calamity.ItemType("Shroomer"),
                    calamity.ItemType("SolsticeClaymore"),
                    calamity.ItemType("SporeKnife"),
                    calamity.ItemType("Spyker"),
                    calamity.ItemType("StormSaber"),
                    calamity.ItemType("StormRuler"),
                    calamity.ItemType("StormSurge"),
                    calamity.ItemType("TarragonThrowingDart"),
                    calamity.ItemType("TerraEdge"),
                    calamity.ItemType("TerraLance"),
                    calamity.ItemType("TerraRay"),
                    calamity.ItemType("TerraShiv"),
                    calamity.ItemType("Terratomere"),
                    calamity.ItemType("TerraFlameburster"),
                    calamity.ItemType("TheSwarmer"),
                    calamity.ItemType("Verdant"),
                    calamity.ItemType("Barinautical"),
                    calamity.ItemType("DeepseaStaff"),
                    calamity.ItemType("Downpour"),
                    calamity.ItemType("SubmarineShocker"),
                    calamity.ItemType("Archerfish"),
                    calamity.ItemType("BallOFugu"),
                    calamity.ItemType("BlackAnurian"),
                    calamity.ItemType("CalamarisLament"),
                    calamity.ItemType("HerringStaff"),
                    calamity.ItemType("Lionfish")
                };

                alcoholList = new List<int>()
                {
                    calamity.BuffType("BloodyMary"),
                    calamity.BuffType("CaribbeanRum"),
                    calamity.BuffType("CinnamonRoll"),
                    calamity.BuffType("Everclear"),
                    calamity.BuffType("EvergreenGin"),
                    calamity.BuffType("Fireball"),
                    calamity.BuffType("GrapeBeer"),
                    calamity.BuffType("Margarita"),
                    calamity.BuffType("Moonshine"),
                    calamity.BuffType("MoscowMule"),
                    calamity.BuffType("RedWine"),
                    calamity.BuffType("Rum"),
                    calamity.BuffType("Screwdriver"),
                    calamity.BuffType("StarBeamRye"),
                    calamity.BuffType("Tequila"),
                    calamity.BuffType("TequilaSunrise"),
                    calamity.BuffType("Vodka"),
                    calamity.BuffType("Whiskey"),
                    calamity.BuffType("WhiteWine")
                };

                doubleDamageBuffList = new List<int>()
                {
                    ItemID.BallOHurt,
                    ItemID.TheMeatball,
                    ItemID.BlueMoon,
                    ItemID.Sunfury,
                    ItemID.DaoofPow,
                    ItemID.FlowerPow,
                    ItemID.Anchor,
                    ItemID.KOCannon,
                    ItemID.GolemFist,
                    ItemID.BreakerBlade,
                    ItemID.MonkStaffT2,
                    ItemID.ProximityMineLauncher,
                    ItemID.FireworksLauncher
                };

                sixtySixDamageBuffList = new List<int>()
                {
                    ItemID.TrueNightsEdge,
                    ItemID.WandofSparking,
                    ItemID.MedusaHead,
                    ItemID.StaffofEarth,
                    ItemID.ChristmasTreeSword,
                    ItemID.MonkStaffT1,
                    ItemID.InfernoFork,
                    ItemID.VenomStaff
                };

                fiftyDamageBuffList = new List<int>()
                {
                    ItemID.NightsEdge,
                    ItemID.EldMelter,
                    ItemID.Flamethrower,
                    ItemID.MoonlordTurretStaff,
                    ItemID.WaspGun,
                    ItemID.Keybrand,
                    ItemID.PulseBow,
                    ItemID.PaladinsHammer
                };

                thirtyThreeDamageBuffList = new List<int>()
                {
                    ItemID.CrystalVileShard,
                    ItemID.SoulDrain,
                    ItemID.ClingerStaff,
                    ItemID.ChargedBlasterCannon,
                    ItemID.NettleBurst,
                    ItemID.Excalibur,
                    ItemID.AmberStaff,
                    ItemID.BluePhasesaber,
                    ItemID.RedPhasesaber,
                    ItemID.GreenPhasesaber,
                    ItemID.WhitePhasesaber,
                    ItemID.YellowPhasesaber,
                    ItemID.PurplePhasesaber,
                    ItemID.TheRottedFork,
                    ItemID.VampireKnives,
                    ItemID.Cascade
                };

                twentyFiveDamageBuffList = new List<int>()
                {
                    ItemID.Muramasa,
                    ItemID.StakeLauncher,
                    ItemID.BookStaff
                };

                twentyDamageBuffList = new List<int>()
                {
                    ItemID.TerraBlade,
                    ItemID.ChainGuillotines,
                    ItemID.FlowerofFrost,
                    ItemID.PoisonStaff,
                    ItemID.Gungnir
                };

                weaponAutoreuseList = new List<int>()
                {
                    ItemID.NightsEdge,
                    ItemID.TrueNightsEdge,
                    ItemID.TrueExcalibur,
                    ItemID.PhoenixBlaster,
                    ItemID.VenusMagnum,
                    ItemID.MagicDagger,
                    ItemID.BeamSword,
                    ItemID.MonkStaffT2,
                    ItemID.PaladinsHammer
                };

                quarterDamageNerfList = new List<int>()
                {
                    ItemID.DaedalusStormbow,
                    ItemID.PhoenixBlaster,
                    ItemID.VenusMagnum,
                    ItemID.BlizzardStaff,
                    ItemID.Phantasm
                };

                pumpkinMoonBuffList = new List<int>()
                {
                    NPCID.Scarecrow1,
                    NPCID.Scarecrow2,
                    NPCID.Scarecrow3,
                    NPCID.Scarecrow4,
                    NPCID.Scarecrow5,
                    NPCID.Scarecrow6,
                    NPCID.Scarecrow7,
                    NPCID.Scarecrow8,
                    NPCID.Scarecrow9,
                    NPCID.Scarecrow10,
                    NPCID.HeadlessHorseman,
                    NPCID.MourningWood,
                    NPCID.Splinterling,
                    NPCID.Pumpking,
                    NPCID.PumpkingBlade,
                    NPCID.Hellhound,
                    NPCID.Poltergeist
                };

                frostMoonBuffList = new List<int>()
                {
                    NPCID.ZombieElf,
                    NPCID.ZombieElfBeard,
                    NPCID.ZombieElfGirl,
                    NPCID.PresentMimic,
                    NPCID.GingerbreadMan,
                    NPCID.Yeti,
                    NPCID.Everscream,
                    NPCID.IceQueen,
                    NPCID.SantaNK1,
                    NPCID.ElfCopter,
                    NPCID.Nutcracker,
                    NPCID.NutcrackerSpinning,
                    NPCID.ElfArcher,
                    NPCID.Krampus,
                    NPCID.Flocko
                };

                eclipseBuffList = new List<int>()
                {
                    NPCID.Eyezor,
                    NPCID.Reaper,
                    NPCID.Frankenstein,
                    NPCID.SwampThing,
                    NPCID.Vampire,
                    NPCID.VampireBat,
                    NPCID.Butcher,
                    NPCID.CreatureFromTheDeep,
                    NPCID.Fritz,
                    NPCID.Nailhead,
                    NPCID.Psycho,
                    NPCID.DeadlySphere,
                    NPCID.DrManFly,
                    NPCID.ThePossessed,
                    NPCID.Mothron,
                    NPCID.MothronEgg,
                    NPCID.MothronSpawn
                };

                eventProjectileBuffList = new List<int>()
                {
                    ProjectileID.FlamingWood,
                    ProjectileID.GreekFire1,
                    ProjectileID.GreekFire2,
                    ProjectileID.GreekFire3,
                    ProjectileID.FlamingScythe,
                    ProjectileID.FlamingArrow,
                    ProjectileID.PineNeedleHostile,
                    ProjectileID.OrnamentHostile,
                    ProjectileID.OrnamentHostileShrapnel,
                    ProjectileID.FrostWave,
                    ProjectileID.FrostShard,
                    ProjectileID.Missile,
                    ProjectileID.Present,
                    ProjectileID.Spike,
                    ProjectileID.BulletDeadeye,
                    ProjectileID.EyeLaser,
                    ProjectileID.Nail,
                    ProjectileID.DrManFlyFlask
                };

                revengeanceEnemyBuffList = new List<int>()
                {
                    NPCID.KingSlime,

                    NPCID.ServantofCthulhu,
                    NPCID.EyeofCthulhu,

                    NPCID.EaterofWorldsHead,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,

                    NPCID.BrainofCthulhu,
                    NPCID.Creeper,

                    NPCID.QueenBee,
                    NPCID.Bee,
                    NPCID.BeeSmall,

                    NPCID.SkeletronHead,
                    NPCID.SkeletronHand,

                    NPCID.WallofFlesh,
                    NPCID.TheHungryII,
                    NPCID.LeechHead,

                    NPCID.Spazmatism,
                    NPCID.Retinazer,

                    NPCID.SkeletronPrime,
                    NPCID.PrimeSaw,
                    NPCID.PrimeVice,
                    NPCID.PrimeLaser,
                    NPCID.PrimeCannon,

                    NPCID.TheDestroyer,
                    NPCID.TheDestroyerBody,
                    NPCID.TheDestroyerTail,
                    NPCID.Probe,

                    NPCID.Plantera,
                    NPCID.PlanterasTentacle,
                    NPCID.Spore,

                    NPCID.Golem,
                    NPCID.GolemHead,
                    NPCID.GolemHeadFree,
                    NPCID.GolemFistLeft,
                    NPCID.GolemFistRight,

                    NPCID.DukeFishron,
                    NPCID.DetonatingBubble,
                    NPCID.Sharkron,
                    NPCID.Sharkron2,

                    NPCID.DD2Betsy,

                    NPCID.CultistDragonHead,
                    NPCID.AncientCultistSquidhead,
                    NPCID.AncientLight,

                    NPCID.MoonLordHand,

                    NPCID.DevourerHead,
                    NPCID.GiantWormHead,
                    NPCID.MeteorHead,
                    NPCID.BoneSerpentHead,
                    NPCID.ManEater,
                    NPCID.Snatcher,
                    NPCID.Piranha,
                    NPCID.Shark,
                    NPCID.SpikeBall,
                    NPCID.BlazingWheel,
                    NPCID.Mimic,
                    NPCID.WyvernHead,
                    NPCID.DiggerHead,
                    NPCID.SeekerHead,
                    NPCID.AnglerFish,
                    NPCID.Werewolf,
                    NPCID.Wraith,
                    NPCID.Arapaima,
                    NPCID.BlackRecluse,
                    NPCID.WallCreeper,
                    NPCID.WallCreeperWall,
                    NPCID.BlackRecluseWall,
                    NPCID.AngryTrapper,
                    NPCID.Lihzahrd,
                    NPCID.LihzahrdCrawler,
                    NPCID.PirateCaptain,
                    NPCID.FlyingSnake,
                    NPCID.Reaper,
                    NPCID.Paladin,
                    NPCID.BoneLee,
                    NPCID.MourningWood,
                    NPCID.Pumpking,
                    NPCID.PumpkingBlade,
                    NPCID.PresentMimic,
                    NPCID.Everscream,
                    NPCID.IceQueen,
                    NPCID.SantaNK1,
                    NPCID.StardustWormHead,
                    NPCID.SolarCrawltipedeHead,
                    NPCID.Butcher,
                    NPCID.Psycho,
                    NPCID.DeadlySphere,
                    NPCID.BigMimicCorruption,
                    NPCID.BigMimicCrimson,
                    NPCID.BigMimicHallow,
                    NPCID.Mothron,
                    NPCID.DuneSplicerHead,
                    NPCID.SlimeSpiked,
                    NPCID.SandShark,
                    NPCID.SandsharkCorrupt,
                    NPCID.SandsharkCrimson,
                    NPCID.SandsharkHallow,

                    calamity.NPCType("DesertScourgeHead"),
                    calamity.NPCType("DesertScourgeHeadSmall"),
                    calamity.NPCType("DriedSeekerHead"),

                    calamity.NPCType("CrabulonIdle"),
                    calamity.NPCType("CrabShroom"),

                    calamity.NPCType("HiveMind"),
                    calamity.NPCType("HiveMindP2"),
                    calamity.NPCType("DankCreeper"),

                    calamity.NPCType("PerforatorHeadLarge"),
                    calamity.NPCType("PerforatorHeadMedium"),
                    calamity.NPCType("PerforatorHeadSmall"),
                    calamity.NPCType("PerforatorHive"),

                    calamity.NPCType("SlimeGod"),
                    calamity.NPCType("SlimeGodRun"),
                    calamity.NPCType("SlimeGodCore"),
                    calamity.NPCType("SlimeGodSplit"),
                    calamity.NPCType("SlimeGodRunSplit"),
                    calamity.NPCType("SlimeSpawnCorrupt"),
                    calamity.NPCType("SlimeSpawnCrimson2"),

                    calamity.NPCType("Cryogen"),
                    calamity.NPCType("CryogenIce"),
                    calamity.NPCType("Cryocore"),
                    calamity.NPCType("Cryocore2"),
                    calamity.NPCType("IceMass"),

                    calamity.NPCType("AquaticScourgeHead"),
                    calamity.NPCType("AquaticScourgeBody"),
                    calamity.NPCType("AquaticScourgeBodyAlt"),
                    calamity.NPCType("AquaticScourgeTail"),

                    calamity.NPCType("BrimstoneElemental"),

                    calamity.NPCType("Calamitas"),
                    calamity.NPCType("CalamitasRun3"),
                    calamity.NPCType("CalamitasRun"),
                    calamity.NPCType("CalamitasRun2"),
                    calamity.NPCType("LifeSeeker"),
                    calamity.NPCType("SoulSeeker"),

                    calamity.NPCType("Parasea"),
                    calamity.NPCType("AquaticAberration"),
                    calamity.NPCType("Leviathan"),
                    calamity.NPCType("SirenIce"),
                    calamity.NPCType("Siren"),

                    calamity.NPCType("Astrageldon"),

                    calamity.NPCType("PlaguebringerGoliath"),
                    calamity.NPCType("PlaguebringerShade"),
                    calamity.NPCType("PlagueHomingMissile"),
                    calamity.NPCType("PlagueBeeG"),
                    calamity.NPCType("PlagueBeeLargeG"),

                    calamity.NPCType("FlamePillar"),
                    calamity.NPCType("RockPillar"),
                    calamity.NPCType("ScavengerHead2"),
                    calamity.NPCType("ScavengerBody"),
                    calamity.NPCType("ScavengerClawRight"),
                    calamity.NPCType("ScavengerClawLeft"),

                    calamity.NPCType("AstrumDeusHead"),
                    calamity.NPCType("AstrumDeusHeadSpectral"),

                    calamity.NPCType("ProfanedGuardianBoss"),
                    calamity.NPCType("ProfanedGuardianBoss2"),
                    calamity.NPCType("ProfanedGuardianBoss3"),

                    calamity.NPCType("Bumblefuck"),
                    calamity.NPCType("Bumblefuck2"),

                    calamity.NPCType("OldDuke"),
                    calamity.NPCType("DukeUrchin"),

                    calamity.NPCType("ProvSpawnDefense"),
                    calamity.NPCType("ProvSpawnOffense"),

                    calamity.NPCType("CeaselessVoid"),

                    calamity.NPCType("StormWeaverHead"),
                    calamity.NPCType("StormWeaverHeadNaked"),

                    calamity.NPCType("CosmicWraith"),
                    calamity.NPCType("CosmicLantern"),

                    calamity.NPCType("Polterghast"),
                    calamity.NPCType("PolterPhantom"),

                    calamity.NPCType("DevourerofGodsHead"),
                    calamity.NPCType("DevourerofGodsBody"),
                    calamity.NPCType("DevourerofGodsTail"),
                    calamity.NPCType("DevourerofGodsHead2"),
                    calamity.NPCType("DevourerofGodsHeadS"),
                    calamity.NPCType("DevourerofGodsBodyS"),
                    calamity.NPCType("DevourerofGodsTailS"),

                    calamity.NPCType("Yharon"),
                    calamity.NPCType("DetonatingFlare"),
                    calamity.NPCType("DetonatingFlare2"),

                    calamity.NPCType("SupremeCalamitas"),

                    calamity.NPCType("BobbitWormHead"),
                    calamity.NPCType("AquaticSeekerHead"),
                    calamity.NPCType("ColossalSquid"),
                    calamity.NPCType("EidolonWyrmHead"),
                    calamity.NPCType("EidolonWyrmHeadHuge"),
                    calamity.NPCType("GulperEelHead"),
                    calamity.NPCType("Mauler"),
                    calamity.NPCType("Reaper"),

                    calamity.NPCType("Atlas"),
                    calamity.NPCType("ArmoredDiggerHead"),
                    calamity.NPCType("Cnidrion"),
                    calamity.NPCType("GreatSandShark"),
                    calamity.NPCType("Horse"),
                    calamity.NPCType("ScornEater"),

                    calamity.NPCType("PrismTurtle"),
                    calamity.NPCType("GhostBell"),
                    calamity.NPCType("EutrophicRay"),
                    calamity.NPCType("Clam"),
                    calamity.NPCType("SeaSerpent1"),
                    calamity.NPCType("BlindedAngler"),
                    calamity.NPCType("GiantClam")
                };

                revengeanceProjectileBuffList = new List<int>()
                {
                    ProjectileID.Sharknado,
                    ProjectileID.Cthulunado,
                    ProjectileID.DD2BetsyFireball,
                    ProjectileID.DD2BetsyFlameBreath,

                    ProjectileID.SandBallFalling,
                    ProjectileID.AshBallFalling,
                    ProjectileID.DemonSickle,
                    ProjectileID.EbonsandBallFalling,
                    ProjectileID.PearlSandBallFalling,
                    ProjectileID.Boulder,
                    ProjectileID.PoisonDartTrap,
                    ProjectileID.SpikyBallTrap,
                    ProjectileID.SpearTrap,
                    ProjectileID.FlamethrowerTrap,
                    ProjectileID.FlamesTrap,
                    ProjectileID.CrimsandBallFalling,
                    ProjectileID.Fireball,
                    ProjectileID.PaladinsHammerHostile,
                    ProjectileID.FlamingWood,
                    ProjectileID.FlamingScythe,
                    ProjectileID.FrostWave,
                    ProjectileID.Present,
                    ProjectileID.Spike,
                    ProjectileID.SaucerDeathray,
                    ProjectileID.NebulaBolt,
                    ProjectileID.NebulaSphere,
                    ProjectileID.NebulaLaser,
                    ProjectileID.StardustSoldierLaser,
                    ProjectileID.VortexLaser,
                    ProjectileID.VortexVortexLightning,
                    ProjectileID.VortexLightning,
                    ProjectileID.VortexAcid,
                    ProjectileID.GeyserTrap,
                    ProjectileID.SandnadoHostile,

                    calamity.ProjectileType("SandBlast"),

                    calamity.ProjectileType("MushBomb"),
                    calamity.ProjectileType("MushBombFall"),
                    calamity.ProjectileType("Mushmash"),

                    calamity.ProjectileType("ShaderainHostile"),

                    calamity.ProjectileType("BloodGeyser"),
                    calamity.ProjectileType("IchorShot"),

                    calamity.ProjectileType("AbyssMine"),
                    calamity.ProjectileType("AbyssMine2"),
                    calamity.ProjectileType("AbyssBallVolley"),
                    calamity.ProjectileType("AbyssBallVolley2"),

                    calamity.ProjectileType("IceBlast"),
                    calamity.ProjectileType("IceBomb"),
                    calamity.ProjectileType("IceRain"),

                    calamity.ProjectileType("SandPoisonCloud"),
                    calamity.ProjectileType("SandTooth"),

                    calamity.ProjectileType("BrimstoneHellfireball"),
                    calamity.ProjectileType("HellfireExplosion"),
                    calamity.ProjectileType("BrimstoneBarrage"),
                    calamity.ProjectileType("BrimstoneHellblast"),

                    calamity.ProjectileType("BrimstoneLaser"),
                    calamity.ProjectileType("BrimstoneLaserSplit"),
                    calamity.ProjectileType("BrimstoneBall"),
                    calamity.ProjectileType("BrimstoneFire"),

                    calamity.ProjectileType("LeviathanBomb"),
                    calamity.ProjectileType("WaterSpear"),
                    calamity.ProjectileType("FrostMist"),
                    calamity.ProjectileType("SirenSong"),

                    calamity.ProjectileType("AstralFlame"),
                    calamity.ProjectileType("AstralLaser"),

                    calamity.ProjectileType("PlagueExplosion"),
                    calamity.ProjectileType("PlagueStingerGoliath"),
                    calamity.ProjectileType("PlagueStingerGoliathV2"),
                    calamity.ProjectileType("HiveBombGoliath"),

                    calamity.ProjectileType("ScavengerNuke"),

                    calamity.ProjectileType("AstralShot2"),
                    calamity.ProjectileType("DeusMine"),

                    calamity.ProjectileType("ProfanedSpear"),
                    calamity.ProjectileType("FlareDust"),

                    calamity.ProjectileType("RedLightningFeather"),

                    calamity.ProjectileType("HolyBlast"),
                    calamity.ProjectileType("HolyBomb"),
                    calamity.ProjectileType("HolyFire"),
                    calamity.ProjectileType("HolyFire2"),
                    calamity.ProjectileType("HolyFlare"),
                    calamity.ProjectileType("HolyShot"),
                    calamity.ProjectileType("HolySpear"),
                    calamity.ProjectileType("MoltenBlast"),
                    calamity.ProjectileType("MoltenBlob"),
                    calamity.ProjectileType("ProvidenceCrystalShard"),
                    calamity.ProjectileType("ProvidenceHolyRay"),

                    calamity.ProjectileType("DarkEnergyBall"),
                    calamity.ProjectileType("DoGBeam"),

                    calamity.ProjectileType("CosmicFlameBurst"),
                    calamity.ProjectileType("SignusScythe"),
                    calamity.ProjectileType("EssenceDust"),

                    calamity.ProjectileType("PhantomBlast"),
                    calamity.ProjectileType("PhantomBlast2"),
                    calamity.ProjectileType("PhantomGhostShot"),
                    calamity.ProjectileType("PhantomHookShot"),
                    calamity.ProjectileType("PhantomMine"),
                    calamity.ProjectileType("PhantomShot"),
                    calamity.ProjectileType("PhantomShot2"),

                    calamity.ProjectileType("DoGDeath"),
                    calamity.ProjectileType("DoGFire"),
                    calamity.ProjectileType("DoGNebulaShot"),

                    calamity.ProjectileType("FlareBomb"),
                    calamity.ProjectileType("FlareDust2"),
                    calamity.ProjectileType("Flarenado"),
                    calamity.ProjectileType("Infernado"),
                    calamity.ProjectileType("Infernado2"),
                    calamity.ProjectileType("YharonFireball"),
                    calamity.ProjectileType("YharonFireball2"),

                    calamity.ProjectileType("BrimstoneFireblast"),
                    calamity.ProjectileType("BrimstoneGigaBlast"),
                    calamity.ProjectileType("BrimstoneHellblast2"),
                    calamity.ProjectileType("BrimstoneMonster"),
                    calamity.ProjectileType("BrimstoneWave"),

                    calamity.ProjectileType("GreatSandBlast"),
                    calamity.ProjectileType("PearlBurst"),
                    calamity.ProjectileType("PearlRain")
                };

                revengeanceLifeStealExceptionList = new List<int>()
                {
                    NPCID.Probe,
                    NPCID.MoonLordFreeEye,
                    NPCID.CultistDragonHead,
                    NPCID.CultistDragonBody1,
                    NPCID.CultistDragonBody2,
                    NPCID.CultistDragonBody3,
                    NPCID.CultistDragonBody4,
                    NPCID.CultistDragonTail,
                    NPCID.AncientCultistSquidhead,
                    NPCID.AncientLight,
                    NPCID.Sharkron,
                    NPCID.Sharkron2,
                    NPCID.PlanterasTentacle,
                    NPCID.Spore,
                    NPCID.TheHungryII,
                    NPCID.LeechHead,
                    NPCID.LeechBody,
                    NPCID.LeechTail,
                    NPCID.TheDestroyerBody,
                    NPCID.TheDestroyerTail,
                    NPCID.EaterofWorldsBody,
                    NPCID.EaterofWorldsTail,
                    NPCID.GolemHead,
                    NPCID.GolemHeadFree,
                    NPCID.GolemFistRight,
                    NPCID.GolemFistLeft,
                    NPCID.MoonLordCore
                };

                movementImpairImmuneList = new List<int>()
                {
                    NPCID.QueenBee,
                };

                trapProjectileList = new List<int>()
                {
                    ProjectileID.PoisonDartTrap,
                    ProjectileID.SpikyBallTrap,
                    ProjectileID.SpearTrap,
                    ProjectileID.FlamethrowerTrap,
                    ProjectileID.FlamesTrap,
                    ProjectileID.PoisonDart,
                    ProjectileID.GeyserTrap
                };

                scopedWeaponList = new List<int>()
                {
                    calamity.ItemType("AMR"),
                    calamity.ItemType("HalleysInferno"),
                    calamity.ItemType("Shroomer"),
                    calamity.ItemType("SpectreRifle"),
                    calamity.ItemType("Svantechnical"),
                    calamity.ItemType("Skullmasher")
                };

                trueMeleeBoostExceptionList = new List<int>()
                {
                    ItemID.FlowerPow,
                    ItemID.Flairon,
                    ItemID.ChlorophytePartisan,
                    ItemID.MushroomSpear,
                    ItemID.NorthPole,
                    ItemID.WoodYoyo,
                    ItemID.CorruptYoyo,
                    ItemID.CrimsonYoyo,
                    ItemID.JungleYoyo,
                    ItemID.Cascade,
                    ItemID.Chik,
                    ItemID.Code2,
                    ItemID.Rally,
                    ItemID.Yelets,
                    ItemID.RedsYoyo,
                    ItemID.ValkyrieYoyo,
                    ItemID.Amarok,
                    ItemID.HelFire,
                    ItemID.Kraken,
                    ItemID.TheEyeOfCthulhu,
                    ItemID.FormatC,
                    ItemID.Gradient,
                    ItemID.Valor,
                    ItemID.Terrarian,
                    calamity.ItemType("BallOFugu"),
                    calamity.ItemType("TyphonsGreed"),
                    calamity.ItemType("UrchinSpear"),
                    calamity.ItemType("AmidiasTrident"),
                    calamity.ItemType("GoldplumeSpear"),
                    calamity.ItemType("EarthenPike"),
                    calamity.ItemType("HellionFlowerSpear"),
                    calamity.ItemType("StarnightLance"),
                    calamity.ItemType("TerraLance"),
                    calamity.ItemType("BansheeHook"),
                    calamity.ItemType("SpatialLance"),
                    calamity.ItemType("StreamGouge"),
                    calamity.ItemType("AirSpinner"),
                    calamity.ItemType("Aorta"),
                    calamity.ItemType("Azathoth"),
                    calamity.ItemType("Chaotrix"),
                    calamity.ItemType("Cnidarian"),
                    calamity.ItemType("Lacerator"),
                    calamity.ItemType("Quagmire"),
                    calamity.ItemType("Shimmerspark"),
                    calamity.ItemType("SolarFlare"),
                    calamity.ItemType("TheEyeofCalamitas"),
                    calamity.ItemType("TheGodsGambit"),
                    calamity.ItemType("TheObliterator"),
                    calamity.ItemType("ThePlaguebringer"),
                    calamity.ItemType("Verdant"),
                    calamity.ItemType("YinYo")
                };

                tableList = new List<int>()
                {
                    calamity.TileType("AbyssBath"),
                    calamity.TileType("AbyssBookcase"),
                    calamity.TileType("AbyssDresser"),
                    calamity.TileType("AbyssPiano"),
                    calamity.TileType("AbyssTable"),
                    calamity.TileType("AbyssWorkbench"),
                    calamity.TileType("AncientBath"),
                    calamity.TileType("AncientBookcase"),
                    calamity.TileType("AncientDresser"),
                    calamity.TileType("AncientPiano"),
                    calamity.TileType("AncientTable"),
                    calamity.TileType("AncientWorkbench"),
                    calamity.TileType("AshenBath"),
                    calamity.TileType("AshenBookcase"),
                    calamity.TileType("AshenDresser"),
                    calamity.TileType("AshenPiano"),
                    calamity.TileType("AshenTable"),
                    calamity.TileType("AshenWorkbench"),
                    calamity.TileType("BotanicBathtub"),
                    calamity.TileType("BotanicBookcase"),
                    calamity.TileType("BotanicDresser"),
                    calamity.TileType("BotanicPiano"),
                    calamity.TileType("BotanicTable"),
                    calamity.TileType("BotanicWorkBench"),
                    calamity.TileType("CosmiliteBath"),
                    calamity.TileType("CosmiliteBookcase"),
                    calamity.TileType("CosmiliteDresser"),
                    calamity.TileType("CosmilitePiano"),
                    calamity.TileType("CosmiliteTable"),
                    calamity.TileType("CosmiliteWorkbench"),
                    calamity.TileType("EutrophicBathtub"),
                    calamity.TileType("EutrophicBookcase"),
                    calamity.TileType("EutrophicDresser"),
                    calamity.TileType("EutrophicPiano"),
                    calamity.TileType("EutrophicTable"),
                    calamity.TileType("EutrophicWorkBench"),
                    calamity.TileType("OccultBathtub"),
                    calamity.TileType("OccultBookcase"),
                    calamity.TileType("OccultDresser"),
                    calamity.TileType("OccultPiano"),
                    calamity.TileType("OccultTable"),
                    calamity.TileType("OccultWorkBench"),
                    calamity.TileType("PlaguedPlateBathtub"),
                    calamity.TileType("PlaguedPlateBookcase"),
                    calamity.TileType("PlaguedPlateDresser"),
                    calamity.TileType("PlaguedPlatePiano"),
                    calamity.TileType("PlaguedPlateTable"),
                    calamity.TileType("PlaguedPlateWorkbench"),
                    calamity.TileType("ProfanedBath"),
                    calamity.TileType("ProfanedBookcase"),
                    calamity.TileType("ProfanedDresser"),
                    calamity.TileType("ProfanedPiano"),
                    calamity.TileType("ProfanedTable"),
                    calamity.TileType("ProfanedWorkbench"),
                    calamity.TileType("SilvaBathtub"),
                    calamity.TileType("SilvaBookcase"),
                    calamity.TileType("SilvaDresser"),
                    calamity.TileType("SilvaPiano"),
                    calamity.TileType("SilvaTable"),
                    calamity.TileType("SilvaWorkBench"),
                    calamity.TileType("StatigelBath"),
                    calamity.TileType("StatigelBookcase"),
                    calamity.TileType("StatigelDresser"),
                    calamity.TileType("StatigelPiano"),
                    calamity.TileType("StatigelTable"),
                    calamity.TileType("StatigelWorkbench"),
                    calamity.TileType("StratusBathtub"),
                    calamity.TileType("StratusBookcase"),
                    calamity.TileType("StratusDresser"),
                    calamity.TileType("StratusPiano"),
                    calamity.TileType("StratusTable"),
                    calamity.TileType("StratusWorkbench"),
                    calamity.TileType("VoidBath"),
                    calamity.TileType("VoidBookcase"),
                    calamity.TileType("VoidDresser"),
                    calamity.TileType("VoidPiano"),
                    calamity.TileType("VoidTable"),
                    calamity.TileType("VoidWorkbench")
                };

                chairList = new List<int>()
                {
                    calamity.TileType("AbyssBed"),
                    calamity.TileType("AbyssChair"),
                    calamity.TileType("AbyssSofa"),
                    calamity.TileType("AncientBed"),
                    calamity.TileType("AncientChair"),
                    calamity.TileType("AncientSofa"),
                    calamity.TileType("AshenBed"),
                    calamity.TileType("AshenChair"),
                    calamity.TileType("AshenSofa"),
                    calamity.TileType("BotanicBed"),
                    calamity.TileType("BotanicBench"),
                    calamity.TileType("BotanicChair"),
                    calamity.TileType("CosmiliteBed"),
                    calamity.TileType("CosmiliteChair"),
                    calamity.TileType("CosmiliteSofa"),
                    calamity.TileType("EutrophicBed"),
                    calamity.TileType("EutrophicBench"),
                    calamity.TileType("EutrophicChair"),
                    calamity.TileType("OccultBed"),
                    calamity.TileType("OccultChair"),
                    calamity.TileType("OccultSofa"),
                    calamity.TileType("PlaguedPlateBed"),
                    calamity.TileType("PlaguedPlateChair"),
                    calamity.TileType("PlaguedPlateSofa"),
                    calamity.TileType("ProfanedBed"),
                    calamity.TileType("ProfanedChair"),
                    calamity.TileType("ProfanedBench"),
                    calamity.TileType("SilvaBed"),
                    calamity.TileType("SilvaChair"),
                    calamity.TileType("SilvaBench"),
                    calamity.TileType("StatigelBed"),
                    calamity.TileType("StatigelChair"),
                    calamity.TileType("StatigelSofa"),
                    calamity.TileType("StratusBed"),
                    calamity.TileType("StratusChair"),
                    calamity.TileType("StratusSofa"),
                    calamity.TileType("VoidBed"),
                    calamity.TileType("VoidChair"),
                    calamity.TileType("VoidSofa")
                };

                lightList = new List<int>()
                {
                    calamity.TileType("BlueCandle"),
                    calamity.TileType("ChaosCandle"),
                    calamity.TileType("LumenylCrystals"),
                    calamity.TileType("PinkCandle"),
                    calamity.TileType("PurpleCandle"),
                    calamity.TileType("TranquilityCandle"),
                    calamity.TileType("YellowCandle"),
                    calamity.TileType("SeaPrismCrystals"),

                    calamity.TileType("AbyssCandelabra"),
                    calamity.TileType("AbyssCandle"),
                    calamity.TileType("AbyssChandelier"),
                    calamity.TileType("AbyssLamp"),
                    calamity.TileType("AbyssLantern"),
                    calamity.TileType("AbyssTorch"),
                    calamity.TileType("AncientAltar"),
                    calamity.TileType("AncientBasin"),
                    calamity.TileType("AncientChandelier"),
                    calamity.TileType("AncientLamp"),
                    calamity.TileType("AncientLantern"),
                    calamity.TileType("AshenAltar"),
                    calamity.TileType("AshenBasin"),
                    calamity.TileType("AshenChandelier"),
                    calamity.TileType("AshenCandle"),
                    calamity.TileType("AshenCandelabra"),
                    calamity.TileType("AshenLamp"),
                    calamity.TileType("AshenLantern"),
                    calamity.TileType("BotanicCandle"),
                    calamity.TileType("BotanicCandelabra"),
                    calamity.TileType("BotanicChandelier"),
                    calamity.TileType("BotanicLamp"),
                    calamity.TileType("BotanicLantern"),
                    calamity.TileType("CosmiliteCandle"),
                    calamity.TileType("CosmiliteCandelabra"),
                    calamity.TileType("CosmiliteChandelier"),
                    calamity.TileType("CosmiliteLamp"),
                    calamity.TileType("CosmiliteLantern"),
                    calamity.TileType("CosmiliteSconce"),
                    calamity.TileType("EutrophicCandle"),
                    calamity.TileType("EutrophicCandelabra"),
                    calamity.TileType("EutrophicChandelier"),
                    calamity.TileType("EutrophicLamp"),
                    calamity.TileType("EutrophicLantern"),
                    calamity.TileType("PlaguedPlateBasin"),
                    calamity.TileType("PlaguedPlateCandle"),
                    calamity.TileType("PlaguedPlateCandelabra"),
                    calamity.TileType("PlaguedPlateChandelier"),
                    calamity.TileType("PlaguedPlateLamp"),
                    calamity.TileType("PlaguedPlateLantern"),
                    calamity.TileType("OccultCandle"),
                    calamity.TileType("OccultCandelabra"),
                    calamity.TileType("OccultChandelier"),
                    calamity.TileType("OccultLamp"),
                    calamity.TileType("OccultLantern"),
                    calamity.TileType("ProfanedBasin"),
                    calamity.TileType("ProfanedCandle"),
                    calamity.TileType("ProfanedCandelabra"),
                    calamity.TileType("ProfanedChandelier"),
                    calamity.TileType("ProfanedLamp"),
                    calamity.TileType("ProfanedLantern"),
                    calamity.TileType("SilvaBasin"),
                    calamity.TileType("SilvaCandle"),
                    calamity.TileType("SilvaCandelabra"),
                    calamity.TileType("SilvaChandelier"),
                    calamity.TileType("SilvaLamp"),
                    calamity.TileType("SilvaLantern"),
                    calamity.TileType("StatigelChandelier"),
                    calamity.TileType("StatigelCandle"),
                    calamity.TileType("StatigelCandelabra"),
                    calamity.TileType("StatigelLamp"),
                    calamity.TileType("StatigelLantern"),
                    calamity.TileType("StratusCandle"),
                    calamity.TileType("StratusCandelabra"),
                    calamity.TileType("StratusChandelier"),
                    calamity.TileType("StratusLantern"),
                    calamity.TileType("StratusLamp"),
                    calamity.TileType("VoidCandle"),
                    calamity.TileType("VoidCandelabra"),
                    calamity.TileType("VoidChandelier"),
                    calamity.TileType("VoidLamp"),
                    calamity.TileType("VoidLantern")
                };

                doorList = new List<int>()
                {
                    calamity.TileType("AbyssDoorOpen"),
                    calamity.TileType("AbyssDoorClosed"),
                    calamity.TileType("AncientDoorOpen"),
                    calamity.TileType("AncientDoorClosed"),
                    calamity.TileType("AshenDoorClosed"),
                    calamity.TileType("AshenDoorOpen"),
                    calamity.TileType("AshenPlatform"),
                    calamity.TileType("BotanicDoorOpen"),
                    calamity.TileType("BotanicDoorClosed"),
                    calamity.TileType("BotanicPlatform"),
                    calamity.TileType("CosmiliteDoorOpen"),
                    calamity.TileType("CosmiliteDoorClosed"),
                    calamity.TileType("CosmilitePlatform"),
                    calamity.TileType("EutrophicDoorOpen"),
                    calamity.TileType("EutrophicDoorClosed"),
                    calamity.TileType("EutrophicPlatform"),
                    calamity.TileType("OccultDoorOpen"),
                    calamity.TileType("OccultDoorClosed"),
                    calamity.TileType("OccultPlatform"),
                    calamity.TileType("PlaguedPlateDoorOpen"),
                    calamity.TileType("PlaguedPlateDoorClosed"),
                    calamity.TileType("PlaguedPlatePlatform"),
                    calamity.TileType("ProfanedDoorOpen"),
                    calamity.TileType("ProfanedDoorClosed"),
                    calamity.TileType("ProfanedPlatform"),
                    calamity.TileType("SilvaDoorOpen"),
                    calamity.TileType("SilvaDoorClosed"),
                    calamity.TileType("SilvaPlatform"),
                    calamity.TileType("SmoothAbyssGravelPlatform"),
                    calamity.TileType("SmoothVoidstonePlatform"),
                    calamity.TileType("StratusDoorOpen"),
                    calamity.TileType("StratusDoorClosed"),
                    calamity.TileType("StatigelPlatform"),
                    calamity.TileType("StratusPlatform"),
                    calamity.TileType("VoidDoorOpen"),
                    calamity.TileType("VoidDoorClosed")
                };

                zombieList = new List<int>()
                {
                    NPCID.Zombie,
                    NPCID.SmallZombie,
                    NPCID.BigZombie,
                    NPCID.ArmedZombie,
                    NPCID.BaldZombie,
                    NPCID.SmallBaldZombie,
                    NPCID.BigBaldZombie,
                    NPCID.PincushionZombie,
                    NPCID.SmallPincushionZombie,
                    NPCID.BigPincushionZombie,
                    NPCID.ArmedZombiePincussion, // what is this spelling
                    NPCID.SlimedZombie,
                    NPCID.SmallSlimedZombie,
                    NPCID.BigSlimedZombie,
                    NPCID.ArmedZombieSlimed,
                    NPCID.SwampZombie,
                    NPCID.SmallSwampZombie,
                    NPCID.BigSwampZombie,
                    NPCID.ArmedZombieSwamp,
                    NPCID.TwiggyZombie,
                    NPCID.SmallTwiggyZombie,
                    NPCID.BigTwiggyZombie,
                    NPCID.ArmedZombieTwiggy,
                    NPCID.FemaleZombie,
                    NPCID.SmallFemaleZombie,
                    NPCID.BigFemaleZombie,
                    NPCID.ArmedZombieCenx,
                    NPCID.ZombieRaincoat,
                    NPCID.SmallRainZombie,
                    NPCID.BigRainZombie,
                    NPCID.ZombieEskimo,
                    NPCID.ArmedZombieEskimo
                    // halloween zombies not included because they don't drop shackles or zombie arms
                };

                demonEyeList = new List<int>()
                {
                    NPCID.DemonEye,
                    NPCID.DemonEye2,
                    NPCID.CataractEye,
                    NPCID.CataractEye2,
                    NPCID.SleepyEye,
                    NPCID.SleepyEye2,
                    NPCID.DialatedEye, // it is spelled "dilated"
                    NPCID.DialatedEye2, // yep
                    NPCID.GreenEye,
                    NPCID.GreenEye2,
                    NPCID.PurpleEye,
                    NPCID.PurpleEye2,
                    NPCID.DemonEyeOwl,
                    NPCID.DemonEyeSpaceship
                };

                skeletonList = new List<int>()
                {
                    NPCID.BigPantlessSkeleton,
                    NPCID.SmallPantlessSkeleton,
                    NPCID.BigMisassembledSkeleton,
                    NPCID.SmallMisassembledSkeleton,
                    NPCID.BigHeadacheSkeleton,
                    NPCID.SmallHeadacheSkeleton,
                    NPCID.BigSkeleton,
                    NPCID.SmallSkeleton,
                    NPCID.HeavySkeleton,
                    NPCID.Skeleton,
                    NPCID.ArmoredSkeleton,
                    NPCID.SkeletonArcher,
                    NPCID.HeadacheSkeleton,
                    NPCID.MisassembledSkeleton,
                    NPCID.PantlessSkeleton,
                    NPCID.SkeletonTopHat,
                    NPCID.SkeletonAstonaut,
                    NPCID.SkeletonAlien,
                    NPCID.BoneThrowingSkeleton,
                    NPCID.BoneThrowingSkeleton2,
                    NPCID.BoneThrowingSkeleton3,
                    NPCID.BoneThrowingSkeleton4,
                    NPCID.GreekSkeleton
                };

                angryBonesList = new List<int>()
                {
                    NPCID.AngryBones,
                    NPCID.ShortBones,
                    NPCID.BigBoned,
                    NPCID.AngryBonesBig,
                    NPCID.AngryBonesBigMuscle,
                    NPCID.AngryBonesBigHelmet
                };

                hornetList = new List<int>()
                {
                    NPCID.Hornet,
                    NPCID.LittleStinger,
                    NPCID.BigStinger,
                    NPCID.HornetFatty,
                    NPCID.LittleHornetFatty,
                    NPCID.BigHornetFatty,
                    NPCID.HornetHoney,
                    NPCID.LittleHornetHoney,
                    NPCID.BigHornetHoney,
                    NPCID.HornetLeafy,
                    NPCID.LittleHornetLeafy,
                    NPCID.BigHornetLeafy,
                    NPCID.HornetSpikey,
                    NPCID.LittleHornetSpikey,
                    NPCID.BigHornetSpikey,
                    NPCID.HornetStingy,
                    NPCID.LittleHornetStingy,
                    NPCID.BigHornetStingy
                };

                mossHornetList = new List<int>()
                {
                    NPCID.MossHornet,
                    NPCID.TinyMossHornet,
                    NPCID.LittleMossHornet,
                    NPCID.BigMossHornet,
                    NPCID.GiantMossHornet
                };
            }

            if (Config.RevengeanceAndDeathThoriumBossBuff)
            {
                if (thorium != null)
                {
                    enemyImmunityList.Add(thorium.NPCType("TheGrandThunderBirdv2"));
                    enemyImmunityList.Add(thorium.NPCType("QueenJelly"));
                    enemyImmunityList.Add(thorium.NPCType("Viscount"));
                    enemyImmunityList.Add(thorium.NPCType("GraniteEnergyStorm"));
                    enemyImmunityList.Add(thorium.NPCType("TheBuriedWarrior"));
                    enemyImmunityList.Add(thorium.NPCType("ThePrimeScouter"));
                    enemyImmunityList.Add(thorium.NPCType("BoreanStrider"));
                    enemyImmunityList.Add(thorium.NPCType("BoreanStriderPopped"));
                    enemyImmunityList.Add(thorium.NPCType("FallenDeathBeholder"));
                    enemyImmunityList.Add(thorium.NPCType("FallenDeathBeholder2"));
                    enemyImmunityList.Add(thorium.NPCType("Lich"));
                    enemyImmunityList.Add(thorium.NPCType("LichHeadless"));
                    enemyImmunityList.Add(thorium.NPCType("Abyssion"));
                    enemyImmunityList.Add(thorium.NPCType("AbyssionCracked"));
                    enemyImmunityList.Add(thorium.NPCType("AbyssionReleased"));
                    enemyImmunityList.Add(thorium.NPCType("SlagFury"));
                    enemyImmunityList.Add(thorium.NPCType("Omnicide"));
                    enemyImmunityList.Add(thorium.NPCType("RealityBreaker"));
                    enemyImmunityList.Add(thorium.NPCType("Aquaius"));
                    enemyImmunityList.Add(thorium.NPCType("Aquaius2"));

                    revengeanceEnemyBuffList.Add(thorium.NPCType("TheGrandThunderBirdv2"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("QueenJelly"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("Viscount"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("GraniteEnergyStorm"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("TheBuriedWarrior"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("ThePrimeScouter"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("BoreanStrider"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("BoreanStriderPopped"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("FallenDeathBeholder"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("FallenDeathBeholder2"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("Lich"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("LichHeadless"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("Abyssion"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("AbyssionCracked"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("AbyssionReleased"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("SlagFury"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("Omnicide"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("RealityBreaker"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("Aquaius"));
                    revengeanceEnemyBuffList.Add(thorium.NPCType("Aquaius2"));

                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("GrandThunderBirdZap"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ThunderGust"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BubbleBomb"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("QueenJellyArm"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("QueenTorrent"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ViscountRipple"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ViscountRipple2"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ViscountBlood"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ViscountStomp"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ViscountStomp2"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("ViscountRockFall"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("GraniteCharge"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedShock"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedDagger"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedArrow"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedArrow2"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedArrowF"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedArrowP"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedArrowC"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedMagic"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BuriedMagicPop"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("MainBeamOuter"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("MainBeam"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("MainBeamCheese"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("VaporizeBlast"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("GravitonSurge"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("Vaporize"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("GravitonCharge"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("GravitySpark"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("DoomBeholderBeam"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("VoidLaserPro"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BeholderBeam"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BlizzardBarrage"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FrostSurge"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FrostSurgeR"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BlizzardCascade"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BlizzardBoom"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("BlizzardFang"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FrostMytePro"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("IceAnomaly"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichGaze"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichGazeB"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichFlareSpawn"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichFlare"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichPulse"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichMatter"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("SoulRenderLich"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichFlareDeathD"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("LichFlareDeathU"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("Whirlpool"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("AbyssionSpit"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("AbyssionSpit2"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("AquaRipple"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("AbyssalStrike2"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("OldGodSpit"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("OldGodSpit2"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("WaterPulse"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("TyphoonBlastHostile"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("TyphoonBlastHostileSmaller"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("AquaBarrage"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("DeathRaySpawnR"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("DeathRaySpawnL"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("DeathRaySpawn"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("OmniDeath"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("OmniSphereOrb"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FlameLash"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FlamePulse"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FlamePulseTorn"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("FlameNova"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("MoltenFury"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("RealityFury"));
                    revengeanceProjectileBuffList.Add(thorium.ProjectileType("UFOBlast"));
                }
            }
        }
        #endregion

        #region Vanilla Enemy DR
        private void SetupVanillaDR()
        {
            DRValues = new SortedDictionary<int, float> {
                { NPCID.AngryBonesBig, 0.2f },
                { NPCID.AngryBonesBigHelmet, 0.2f },
                { NPCID.AngryBonesBigMuscle, 0.2f },
                { NPCID.AnomuraFungus, 0.1f },
                { NPCID.Antlion, 0.1f },
                { NPCID.Arapaima, 0.1f },
                { NPCID.ArmoredSkeleton, 0.15f },
                { NPCID.ArmoredViking, 0.1f },
                { NPCID.BigMimicCorruption, 0.3f },
                { NPCID.BigMimicCrimson, 0.3f },
                { NPCID.BigMimicHallow, 0.3f },
                { NPCID.BigMimicJungle, 0.3f }, // unused vanilla enemy
                { NPCID.BlueArmoredBones, 0.2f },
                { NPCID.BlueArmoredBonesMace, 0.2f },
                { NPCID.BlueArmoredBonesNoPants, 0.2f },
                { NPCID.BlueArmoredBonesSword, 0.2f },
                { NPCID.BoneLee, 0.2f },
                { NPCID.Crab, 0.05f },
                { NPCID.Crawdad, 0.2f },
                { NPCID.Crawdad2, 0.2f },
                { NPCID.CultistBoss, 0.05f },
                { NPCID.DD2Betsy, 0.1f },
                { NPCID.DD2OgreT2, 0.1f },
                { NPCID.DD2OgreT3, 0.15f },
                { NPCID.DeadlySphere, 0.4f },
                { NPCID.DiabolistRed, 0.2f },
                { NPCID.DiabolistWhite, 0.2f },
                { NPCID.DungeonGuardian, 0.999999f },
                { NPCID.DungeonSpirit, 0.2f },
                { NPCID.ElfCopter, 0.15f },
                { NPCID.Everscream, 0.1f },
                { NPCID.FlyingAntlion, 0.05f },
                { NPCID.GiantCursedSkull, 0.2f },
                { NPCID.GiantShelly, 0.2f },
                { NPCID.GiantShelly2, 0.2f },
                { NPCID.GiantTortoise, 0.35f },
                { NPCID.Golem, 0.25f },
                { NPCID.GolemFistLeft, 0.25f },
                { NPCID.GolemFistRight, 0.25f },
                { NPCID.GolemHead, 0.25f },
                { NPCID.GolemHeadFree, 0.25f },
                { NPCID.GraniteFlyer, 0.1f },
                { NPCID.GraniteGolem, 0.15f },
                { NPCID.GreekSkeleton, 0.1f },
                { NPCID.HellArmoredBones, 0.2f },
                { NPCID.HellArmoredBonesMace, 0.2f },
                { NPCID.HellArmoredBonesSpikeShield, 0.2f },
                { NPCID.HellArmoredBonesSword, 0.2f },
                { NPCID.IceGolem, 0.1f },
                { NPCID.IceQueen, 0.1f },
                { NPCID.IceTortoise, 0.35f },
                { NPCID.HeadlessHorseman, 0.05f },
                { NPCID.MartianDrone, 0.2f },
                { NPCID.MartianSaucer, 0.2f },
                { NPCID.MartianSaucerCannon, 0.2f },
                { NPCID.MartianSaucerCore, 0.2f },
                { NPCID.MartianSaucerTurret, 0.2f },
                { NPCID.MartianTurret, 0.2f },
                { NPCID.MartianWalker, 0.35f },
                { NPCID.Mimic, 0.3f },
                { NPCID.MoonLordCore, 0.05f },
                { NPCID.MoonLordFreeEye, 0.05f },
                { NPCID.MoonLordHand, 0.05f },
                { NPCID.MoonLordHead, 0.05f },
                { NPCID.Mothron, 0.2f },
                { NPCID.MothronEgg, 0.5f },
                { NPCID.MourningWood, 0.1f },
                { NPCID.Necromancer, 0.2f },
                { NPCID.NecromancerArmored, 0.2f },
                { NPCID.Paladin, 0.45f },
                { NPCID.PirateCaptain, 0.05f },
                { NPCID.PirateShipCannon, 0.15f },
                { NPCID.PossessedArmor, 0.25f },
                { NPCID.PresentMimic, 0.3f },
                { NPCID.PrimeCannon, 0.2f },
                { NPCID.PrimeLaser, 0.2f },
                { NPCID.PrimeSaw, 0.25f },
                { NPCID.PrimeVice, 0.25f },
                { NPCID.Probe, 0.25f },
                { NPCID.Pumpking, 0.1f },
                { NPCID.QueenBee, 0.05f },
                { NPCID.RaggedCaster, 0.2f },
                { NPCID.RaggedCasterOpenCoat, 0.2f },
                { NPCID.Retinazer, 0.2f },
                { NPCID.RustyArmoredBonesAxe, 0.2f },
                { NPCID.RustyArmoredBonesFlail, 0.2f },
                { NPCID.RustyArmoredBonesSword, 0.2f },
                { NPCID.RustyArmoredBonesSwordNoArmor, 0.2f },
                { NPCID.SandElemental, 0.1f },
                { NPCID.SantaNK1, 0.35f },
                { NPCID.SeaSnail, 0.05f },
                { NPCID.SkeletonArcher, 0.1f },
                { NPCID.SkeletonCommando, 0.2f },
                { NPCID.SkeletonSniper, 0.2f },
                { NPCID.SkeletronHand, 0.05f },
                { NPCID.SkeletronHead, 0.05f },
                { NPCID.SkeletronPrime, 0.25f },
                { NPCID.Spazmatism, 0.2f },
                { NPCID.TacticalSkeleton, 0.2f },
                { NPCID.TheDestroyer, 0.1f },
                { NPCID.TheDestroyerBody, 0.2f },
                { NPCID.TheDestroyerTail, 0.35f },
                { NPCID.TheHungry, 0.1f },
                { NPCID.UndeadViking, 0.1f },
                { NPCID.WalkingAntlion, 0.1f },
                { NPCID.WallofFlesh, 0.5f },
            };
        }
        #endregion

        #region Thorium Boss DR
        private void SetupThoriumBossDR(Mod thorium)
        {
            if (thorium is null || !Config.RevengeanceAndDeathThoriumBossBuff)
                return;

            void ThoriumDR(string npcName, float dr) => DRValues.Add(thorium.NPCType(npcName), dr);
            ThoriumDR("Viscount", 0.05f);
            ThoriumDR("BoreanStrider", 0.05f);
            ThoriumDR("FallenDeathBeholder", 0.05f);
            ThoriumDR("Lich", 0.05f);
            ThoriumDR("AbyssionReleased", 0.05f);
            ThoriumDR("CryoCore", 0.1f);
            ThoriumDR("BioCore", 0.1f);
            ThoriumDR("PyroCore", 0.1f);
            ThoriumDR("Aquaius", 0.1f);
            ThoriumDR("ThePrimeScouter", 0.2f);
            ThoriumDR("FallenDeathBeholder2", 0.2f);
            ThoriumDR("SlagFury", 0.2f);
            ThoriumDR("Aquaius2", 0.2f);
            ThoriumDR("GraniteEnergyStorm", 0.2f);
            ThoriumDR("TheBuriedWarrior", 0.25f);
            ThoriumDR("TheBuriedWarrior1", 0.25f);
            ThoriumDR("TheBuriedWarrior2", 0.25f);
            ThoriumDR("LichHeadless", 0.25f);
            ThoriumDR("AbyssionCracked", 0.25f);
            ThoriumDR("Omnicide", 0.3f);
            ThoriumDR("Abyssion", 0.35f);
        }
        #endregion

        #region Music
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            Mod calamityModMusic = ModLoader.GetMod("CalamityModMusic");
            if (Main.musicVolume != 0)
            {
                if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
                {
                    Player p = Main.LocalPlayer;
                    if (p.InCalamity())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Crag");
                            else
                                music = MusicID.Eerie;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InSunkenSea())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/SunkenSea");
                            else
                                music = MusicID.Temple;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAstral())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Astral");
                            else
                                music = MusicID.Space;
                            priority = MusicPriority.Environment;
                        }
                    }
                    if (p.InAbyss(1) || p.InAbyss(2))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/TheAbyss");
                            else
                                music = MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InAbyss(3))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/TheDeepAbyss");
                            else
                                music = MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InAbyss(4))
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/TheVoid");
                            else
                                music = MusicID.Hell;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (p.InSulphur())
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/Sulphur");
                            else
                                music = MusicID.Desert;
                            priority = MusicPriority.BiomeHigh;
                        }
                    }
                    if (CalamityWorld.DoGSecondStageCountdown <= 540 && CalamityWorld.DoGSecondStageCountdown > 60) //8 seconds before DoG spawns
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses)
                        {
                            if (calamityModMusic != null)
                                music = calamityModMusic.GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse");
                            else
                                music = MusicID.LunarBoss;
                            priority = MusicPriority.BossMedium;
                        }
                    }
                }
            }
        }
        #endregion

        #region ModSupport
        public override void PostSetupContent() => WeakReferenceSupport.Setup();

        public override object Call(params object[] args) => ModCalls.Call(args);
        #endregion

        #region DrawingStuff
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            if (CalamityWorld.revenge && Config.AdrenalineAndRage)
            {
                UIHandler.ModifyInterfaceLayers(mod, layers);
            }
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("Boss HP Bars", delegate ()
                {
                    if (Main.LocalPlayer.Calamity().drawBossHPBar)
                    {
                        BossHealthBarManager.Update();
                        BossHealthBarManager.Draw(Main.spriteBatch);
                    }
                    return true;
                }, InterfaceScaleType.None));
            }
            base.ModifyInterfaceLayers(layers);
            layers.Insert(index, new LegacyGameInterfaceLayer("Astral Arcanum UI", delegate ()
            {
                AstralArcanumUI.UpdateAndDraw(Main.spriteBatch);
                return true;
            }, InterfaceScaleType.None));
        }

        public static Color GetNPCColor(NPC npc, Vector2? position = null, bool effects = true, float shadowOverride = 0f)
        {
            return npc.GetAlpha(BuffEffects(
                npc, GetLightColor(position != null ? (Vector2)position : npc.Center),
                shadowOverride != 0f ? shadowOverride : 0f, effects, npc.poisoned, npc.onFire, npc.onFire2,
                Main.player[Main.myPlayer].detectCreature, false, false, false, npc.venom, npc.midas, npc.ichor,
                npc.onFrostBurn, false, false, npc.dripping, npc.drippingSlime, npc.loveStruck, npc.stinky)
            );
        }

        public static Color GetLightColor(Vector2 position) => Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));

        public static Color BuffEffects(Entity codable, Color lightColor, float shadow = 0f, bool effects = true,
            bool poisoned = false, bool onFire = false, bool onFire2 = false, bool hunter = false, bool noItems = false,
            bool blind = false, bool bleed = false, bool venom = false, bool midas = false, bool ichor = false,
            bool onFrostBurn = false, bool burned = false, bool honey = false, bool dripping = false,
            bool drippingSlime = false, bool loveStruck = false, bool stinky = false)
        {
            float cr = 1f;
            float cg = 1f;
            float cb = 1f;
            float ca = 1f;
            if (effects && honey && Main.rand.NextBool(30))
            {
                int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 152, 0f, 0f, 150, default, 1f);
                Main.dust[dustID].velocity.Y = 0.3f;
                Main.dust[dustID].velocity.X *= 0.1f;
                Main.dust[dustID].scale += Main.rand.Next(3, 4) * 0.1f;
                Main.dust[dustID].alpha = 100;
                Main.dust[dustID].noGravity = true;
                Main.dust[dustID].velocity += codable.velocity * 0.1f;
                if (codable is Player)
                {
                    Main.playerDrawDust.Add(dustID);
                }
            }
            if (poisoned)
            {
                if (effects && Main.rand.NextBool(30))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 46, 0f, 0f, 120, default, 0.2f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].fadeIn = 1.9f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                cr *= 0.65f;
                cb *= 0.75f;
            }
            if (venom)
            {
                if (effects && Main.rand.NextBool(10))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 171, 0f, 0f, 100, default, 0.5f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].fadeIn = 1.5f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                cg *= 0.45f;
                cr *= 0.75f;
            }
            if (midas)
            {
                cb *= 0.3f;
                cr *= 0.85f;
            }
            if (ichor)
            {
                if (codable is NPC)
                {
                    lightColor = new Color(255, 255, 0, 255);
                }
                else
                {
                    cb = 0f;
                }
            }
            if (burned)
            {
                if (effects)
                {
                    int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 2f);
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].velocity *= 1.8f;
                    Main.dust[dustID].velocity.Y -= 0.75f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                if (codable is Player)
                {
                    cr = 1f;
                    cb *= 0.6f;
                    cg *= 0.7f;
                }
            }
            if (onFrostBurn)
            {
                if (effects)
                {
                    if (Main.rand.Next(4) < 3)
                    {
                        int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 135, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
                        Main.dust[dustID].noGravity = true;
                        Main.dust[dustID].velocity *= 1.8f;
                        Main.dust[dustID].velocity.Y -= 0.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale *= 0.5f;
                        }
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                    Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 0.1f, 0.6f, 1f);
                }
                if (codable is Player)
                {
                    cr *= 0.5f;
                    cg *= 0.7f;
                }
            }
            if (onFire)
            {
                if (effects)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
                        Main.dust[dustID].noGravity = true;
                        Main.dust[dustID].velocity *= 1.8f;
                        Main.dust[dustID].velocity.Y -= 0.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale *= 0.5f;
                        }
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                    Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
                }
                if (codable is Player)
                {
                    cb *= 0.6f;
                    cg *= 0.7f;
                }
            }
            if (dripping && shadow == 0f && Main.rand.Next(4) != 0)
            {
                Vector2 position = codable.position;
                position.X -= 2f;
                position.Y -= 2f;
                if (Main.rand.NextBool(2))
                {
                    int dustID = Dust.NewDust(position, codable.width + 4, codable.height + 2, 211, 0f, 0f, 50, default, 0.8f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    Main.dust[dustID].noLight = true;
                    Main.dust[dustID].velocity *= 0.2f;
                    Main.dust[dustID].velocity.Y += 0.2f;
                    Main.dust[dustID].velocity += codable.velocity;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                else
                {
                    int dustID = Dust.NewDust(position, codable.width + 8, codable.height + 8, 211, 0f, 0f, 50, default, 1.1f);
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    if (Main.rand.NextBool(2))
                    {
                        Main.dust[dustID].alpha += 25;
                    }
                    Main.dust[dustID].noLight = true;
                    Main.dust[dustID].noGravity = true;
                    Main.dust[dustID].velocity *= 0.2f;
                    Main.dust[dustID].velocity.Y += 1f;
                    Main.dust[dustID].velocity += codable.velocity;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
            }
            if (drippingSlime && shadow == 0f)
            {
                int alpha = 175;
                Color newColor = new Color(0, 80, 255, 100);
                if (Main.rand.Next(4) != 0)
                {
                    if (Main.rand.NextBool(2))
                    {
                        Vector2 position2 = codable.position;
                        position2.X -= 2f;
                        position2.Y -= 2f;
                        int dustID = Dust.NewDust(position2, codable.width + 4, codable.height + 2, 4, 0f, 0f, alpha, newColor, 1.4f);
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[dustID].alpha += 25;
                        }
                        if (Main.rand.NextBool(2))
                        {
                            Main.dust[dustID].alpha += 25;
                        }
                        Main.dust[dustID].noLight = true;
                        Main.dust[dustID].velocity *= 0.2f;
                        Main.dust[dustID].velocity.Y += 0.2f;
                        Main.dust[dustID].velocity += codable.velocity;
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                }
                cr *= 0.8f;
                cg *= 0.8f;
            }
            if (onFire2)
            {
                if (effects)
                {
                    if (Main.rand.Next(4) != 0)
                    {
                        int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 75, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default, 3.5f);
                        Main.dust[dustID].noGravity = true;
                        Main.dust[dustID].velocity *= 1.8f;
                        Main.dust[dustID].velocity.Y -= 0.5f;
                        if (Main.rand.NextBool(4))
                        {
                            Main.dust[dustID].noGravity = false;
                            Main.dust[dustID].scale *= 0.5f;
                        }
                        if (codable is Player)
                        {
                            Main.playerDrawDust.Add(dustID);
                        }
                    }
                    Lighting.AddLight((int)(codable.position.X / 16f), (int)(codable.position.Y / 16f + 1f), 1f, 0.3f, 0.1f);
                }
                if (codable is Player)
                {
                    cb *= 0.6f;
                    cg *= 0.7f;
                }
            }
            if (noItems)
            {
                cr *= 0.65f;
                cg *= 0.8f;
            }
            if (blind)
            {
                cr *= 0.7f;
                cg *= 0.65f;
            }
            if (bleed)
            {
                bool dead = codable is Player ? ((Player)codable).dead : codable is NPC ? ((NPC)codable).life <= 0 : false;
                if (effects && !dead && Main.rand.NextBool(30))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 5, 0f, 0f, 0, default, 1f);
                    Main.dust[dustID].velocity.Y += 0.5f;
                    Main.dust[dustID].velocity *= 0.25f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                cg *= 0.9f;
                cb *= 0.9f;
            }
            if (loveStruck && effects && shadow == 0f && Main.instance.IsActive && !Main.gamePaused && Main.rand.NextBool(5))
            {
                Vector2 value = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                value.Normalize();
                value.X *= 0.66f;
                int goreID = Gore.NewGore(codable.position + new Vector2(Main.rand.Next(codable.width + 1), Main.rand.Next(codable.height + 1)), value * Main.rand.Next(3, 6) * 0.33f, 331, Main.rand.Next(40, 121) * 0.01f);
                Main.gore[goreID].sticky = false;
                Main.gore[goreID].velocity *= 0.4f;
                Main.gore[goreID].velocity.Y -= 0.6f;
                if (codable is Player)
                {
                    Main.playerDrawGore.Add(goreID);
                }
            }
            if (stinky && shadow == 0f)
            {
                cr *= 0.7f;
                cb *= 0.55f;
                if (effects && Main.rand.NextBool(5) && Main.instance.IsActive && !Main.gamePaused)
                {
                    Vector2 value2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                    value2.Normalize();
                    value2.X *= 0.66f;
                    value2.Y = Math.Abs(value2.Y);
                    Vector2 vector = value2 * Main.rand.Next(3, 5) * 0.25f;
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 188, vector.X, vector.Y * 0.5f, 100, default, 1.5f);
                    Main.dust[dustID].velocity *= 0.1f;
                    Main.dust[dustID].velocity.Y -= 0.5f;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
            }
            lightColor.R = (byte)(lightColor.R * cr);
            lightColor.G = (byte)(lightColor.G * cg);
            lightColor.B = (byte)(lightColor.B * cb);
            lightColor.A = (byte)(lightColor.A * ca);
            if (codable is NPC)
            {
                NPCLoader.DrawEffects((NPC)codable, ref lightColor);
            }
            if (hunter && (codable is NPC ? ((NPC)codable).lifeMax > 1 : true))
            {
                if (effects && !Main.gamePaused && Main.instance.IsActive && Main.rand.NextBool(50))
                {
                    int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 15, 0f, 0f, 150, default, 0.8f);
                    Main.dust[dustID].velocity *= 0.1f;
                    Main.dust[dustID].noLight = true;
                    if (codable is Player)
                    {
                        Main.playerDrawDust.Add(dustID);
                    }
                }
                byte colorR = 50, colorG = 255, colorB = 50;
                if (codable is NPC && !(((NPC)codable).friendly || ((NPC)codable).catchItem > 0 || (((NPC)codable).damage == 0 && ((NPC)codable).lifeMax == 5)))
                {
                    colorR = 255;
                    colorG = 50;
                }
                if (!(codable is NPC) && lightColor.R < 150)
                {
                    lightColor.A = Main.mouseTextColor;
                }
                if (lightColor.R < colorR)
                {
                    lightColor.R = colorR;
                }
                if (lightColor.G < colorG)
                {
                    lightColor.G = colorG;
                }
                if (lightColor.B < colorB)
                {
                    lightColor.B = colorB;
                }
            }
            return lightColor;
        }

        public static void DrawTexture(object sb, Texture2D texture, int shader, Entity codable, Color? overrideColor = null, bool drawCentered = false)
        {
            Color lightColor = overrideColor != null ? (Color)overrideColor : codable is NPC ? GetNPCColor((NPC)codable, codable.Center, false) : codable is Projectile ? ((Projectile)codable).GetAlpha(GetLightColor(codable.Center)) : GetLightColor(codable.Center);
            int frameCount = codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1;
            Rectangle frame = codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height);
            float scale = codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale;
            float rotation = codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation;
            int spriteDirection = codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection;
            float offsetY = codable is NPC ? ((NPC)codable).gfxOffY : 0f;
            DrawTexture(sb, texture, shader, codable.position + new Vector2(0f, offsetY), codable.width, codable.height, scale, rotation, spriteDirection, frameCount, frame, lightColor, drawCentered);
        }

        public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, Rectangle frame, Color? overrideColor = null, bool drawCentered = false)
        {
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / framecount / 2);
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
            if (sb is List<DrawData>)
            {
                DrawData dd = new DrawData(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, framecount, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
                {
                    shader = shader
                };
                ((List<DrawData>)sb).Add(dd);
            }
            else if (sb is SpriteBatch)
            {
                bool applyDye = shader > 0;
                if (applyDye)
                {
                    ((SpriteBatch)sb).End();
                    ((SpriteBatch)sb).Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    GameShaders.Armor.ApplySecondary(shader, Main.player[Main.myPlayer], null);
                }
                ((SpriteBatch)sb).Draw(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, framecount, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                if (applyDye)
                {
                    ((SpriteBatch)sb).End();
                    ((SpriteBatch)sb).Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                }
            }
        }

        public static Vector2 GetDrawPosition(Vector2 position, Vector2 origin, int width, int height, int texWidth, int texHeight, int framecount, float scale, bool drawCentered = false)
        {
            Vector2 screenPos = new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y);
            if (drawCentered)
            {
                Vector2 texHalf = new Vector2(texWidth / 2, texHeight / framecount / 2);
                return position + new Vector2(width * 0.5f, height * 0.5f) - (texHalf * scale) + (origin * scale) - screenPos;
            }
            return position - screenPos + new Vector2(width * 0.5f, height) - new Vector2(texWidth * scale / 2f, texHeight * scale / framecount) + (origin * scale) + new Vector2(0f, 5f);
        }
        #endregion

        #region Recipes
        public override void AddRecipeGroups() => CalamityRecipes.AddRecipeGroups();

        public override void AddRecipes() => CalamityRecipes.AddRecipes();
        #endregion

        #region Seasons
        public static Season CurrentSeason
        {
            get
            {
                DateTime date = DateTime.Now;
                int day = date.DayOfYear - Convert.ToInt32(DateTime.IsLeapYear(date.Year) && date.DayOfYear > 59);

                if (day < 80 || day >= 355)
                {
                    return Season.Winter;
                }

                else if (day >= 80 && day < 172)
                {
                    return Season.Spring;
                }

                else if (day >= 172 && day < 266)
                {
                    return Season.Summer;
                }

                else
                {
                    return Season.Fall;
                }
            }
        }
        #endregion

        #region Packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
        {
            CalamityModMessageType msgType = (CalamityModMessageType)reader.ReadByte();
            switch (msgType)
            {
                case CalamityModMessageType.MeleeLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 0);
                    break;
                case CalamityModMessageType.RangedLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 1);
                    break;
                case CalamityModMessageType.MagicLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 2);
                    break;
                case CalamityModMessageType.SummonLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 3);
                    break;
                case CalamityModMessageType.RogueLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleLevels(reader, 4);
                    break;
                case CalamityModMessageType.ExactMeleeLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 0);
                    break;
                case CalamityModMessageType.ExactRangedLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 1);
                    break;
                case CalamityModMessageType.ExactMagicLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 2);
                    break;
                case CalamityModMessageType.ExactSummonLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 3);
                    break;
                case CalamityModMessageType.ExactRogueLevelSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleExactLevels(reader, 4);
                    break;
                case CalamityModMessageType.StressSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleStress(reader);
                    break;
                case CalamityModMessageType.BossRushStage:
                    int stage = reader.ReadInt32();
                    CalamityWorld.bossRushStage = stage;
                    break;
                case CalamityModMessageType.AdrenalineSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleAdrenaline(reader);
                    break;
                case CalamityModMessageType.RadiationSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleRadiation(reader);
                    break;
                /*case CalamityModMessageType.DistanceFromBossSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleDistanceFromBoss(reader);
                    break;*/
                case CalamityModMessageType.TeleportPlayer:
                    Main.player[reader.ReadInt32()].Calamity().HandleTeleport(reader.ReadInt32(), true, whoAmI);
                    break;
                case CalamityModMessageType.DoGCountdownSync:
                    int countdown = reader.ReadInt32();
                    CalamityWorld.DoGSecondStageCountdown = countdown;
                    break;
                case CalamityModMessageType.BossSpawnCountdownSync:
                    int countdown2 = reader.ReadInt32();
                    CalamityWorld.bossSpawnCountdown = countdown2;
                    break;
                case CalamityModMessageType.BossTypeSync:
                    int type = reader.ReadInt32();
                    CalamityWorld.bossType = type;
                    break;
                case CalamityModMessageType.DeathCountSync:
                    Main.player[reader.ReadInt32()].Calamity().HandleDeathCount(reader);
                    break;
                default:
                    Logger.Warn("Unknown Message type: " + msgType);
                    break;
            }
        }
        #endregion

        #region Stop Rain
        public static void StopRain()
        {
            if (!Main.raining)
                return;
            Main.raining = false;
            UpdateServerBoolean();
        }
        #endregion

        #region Update Server Boolean
        public static void UpdateServerBoolean()
        {
            if (Main.netMode == NetmodeID.Server)
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
        }
        #endregion
    }

    public enum Season : byte
    {
        Winter,
        Spring,
        Summer,
        Fall
    }

    enum CalamityModMessageType : byte
    {
        MeleeLevelSync,
        RangedLevelSync,
        MagicLevelSync,
        SummonLevelSync,
        RogueLevelSync,
        ExactMeleeLevelSync,
        ExactRangedLevelSync,
        ExactMagicLevelSync,
        ExactSummonLevelSync,
        ExactRogueLevelSync,
        StressSync,
        AdrenalineSync,
        TeleportPlayer,
        BossRushStage,
        DoGCountdownSync,
        BossSpawnCountdownSync,
        BossTypeSync,
        DeathCountSync,
        RadiationSync
        //DistanceFromBossSync
    }
}
