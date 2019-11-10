using CalamityMod.Buffs.Alcohol;
using CalamityMod.Buffs.DamageOverTime;
using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.Items.Accessories;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.Tools;
using CalamityMod.Items.Weapons.Magic;
using CalamityMod.Items.Weapons.Melee;
using CalamityMod.Items.Weapons.Ranged;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.Items.Materials;
using CalamityMod.Items.Fishing.FishingRods;
using CalamityMod.Localization;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
using CalamityMod.NPCs.Astral;
using CalamityMod.NPCs.AstrumAureus;
using CalamityMod.NPCs.AstrumDeus;
using CalamityMod.NPCs.BrimstoneElemental;
using CalamityMod.NPCs.Bumblebirb;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.CeaselessVoid;
using CalamityMod.NPCs.Crabulon;
using CalamityMod.NPCs.Cryogen;
using CalamityMod.NPCs.DesertScourge;
using CalamityMod.NPCs.DevourerofGods;
using CalamityMod.NPCs.GreatSandShark;
using CalamityMod.NPCs.HiveMind;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.NormalNPCs;
using CalamityMod.NPCs.Perforator;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.NPCs.ProfanedGuardians;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.Ravager;
using CalamityMod.NPCs.Signus;
using CalamityMod.NPCs.SlimeGod;
using CalamityMod.NPCs.StormWeaver;
using CalamityMod.NPCs.SunkenSea;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Projectiles.Boss;
using CalamityMod.Projectiles.Enemy;
using CalamityMod.Projectiles.Ranged;
using CalamityMod.Skies;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Furniture;
using CalamityMod.Tiles.Furniture.CraftingStations;
using CalamityMod.Tiles.FurnitureAbyss;
using CalamityMod.Tiles.FurnitureAncient;
using CalamityMod.Tiles.FurnitureAshen;
using CalamityMod.Tiles.FurnitureBotanic;
using CalamityMod.Tiles.FurnitureCosmilite;
using CalamityMod.Tiles.FurnitureEutrophic;
using CalamityMod.Tiles.FurnitureOccult;
using CalamityMod.Tiles.FurniturePlaguedPlate;
using CalamityMod.Tiles.FurnitureProfaned;
using CalamityMod.Tiles.FurnitureSilva;
using CalamityMod.Tiles.FurnitureStatigel;
using CalamityMod.Tiles.FurnitureStratus;
using CalamityMod.Tiles.FurnitureVoid;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.UI;
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
        // TODO -- I have been advised by Jopo that Mods should never contain static variables
        
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
        public static List<int> boomerangList;
        public static List<int> javelinList;
        public static List<int> daggerList;
        public static List<int> flaskBombList;
        public static List<int> spikyBallList;
        public static List<int> noGravityList;
        public static List<int> lavaFishList;

        public static List<int> zombieList;
        public static List<int> demonEyeList;
        public static List<int> skeletonList;
        public static List<int> angryBonesList;
        public static List<int> hornetList;
        public static List<int> mossHornetList;

        private Mod thorium = null;

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

            thorium = ModLoader.GetMod("ThoriumMod");

            BossHealthBarManager.Load(this);

            Config.Load();

            SetupLists();
            SetupVanillaDR();
            SetupThoriumBossDR(thorium);

            CalamityLocalization.AddLocalizations();
        }

        private void LoadClient()
        {
            AddEquipTexture(new AbyssalDivingSuitHead(), null, EquipType.Head, "AbyssalDivingSuitHead", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Head");
            AddEquipTexture(new AbyssalDivingSuitBody(), null, EquipType.Body, "AbyssalDivingSuitBody", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Body", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Arms");
            AddEquipTexture(new AbyssalDivingSuitLegs(), null, EquipType.Legs, "AbyssalDivingSuitLeg", "CalamityMod/Items/Accessories/AbyssalDivingSuit_Legs");

            AddEquipTexture(new SirenHead(), null, EquipType.Head, "SirenHead", "CalamityMod/Items/Accessories/SirenTrans_Head");
            AddEquipTexture(new SirenBody(), null, EquipType.Body, "SirenBody", "CalamityMod/Items/Accessories/SirenTrans_Body", "CalamityMod/Items/Accessories/SirenTrans_Arms");
            AddEquipTexture(new SirenLegs(), null, EquipType.Legs, "SirenLeg", "CalamityMod/Items/Accessories/SirenTrans_Legs");

            AddEquipTexture(new SirenHeadAlt(), null, EquipType.Head, "SirenHeadAlt", "CalamityMod/Items/Accessories/SirenTransAlt_Head");
            AddEquipTexture(new SirenBodyAlt(), null, EquipType.Body, "SirenBodyAlt", "CalamityMod/Items/Accessories/SirenTransAlt_Body", "CalamityMod/Items/Accessories/SirenTransAlt_Arms");
            AddEquipTexture(new SirenLegsAlt(), null, EquipType.Legs, "SirenLegAlt", "CalamityMod/Items/Accessories/SirenTransAlt_Legs");

            AddEquipTexture(new PopoHead(), null, EquipType.Head, "PopoHead", "CalamityMod/Items/Accessories/Vanity/Popo_Head");
            AddEquipTexture(new PopoNoselessHead(), null, EquipType.Head, "PopoNoselessHead", "CalamityMod/Items/Accessories/Vanity/PopoNoseless_Head");
            AddEquipTexture(new PopoBody(), null, EquipType.Body, "PopoBody", "CalamityMod/Items/Accessories/Vanity/Popo_Body", "CalamityMod/Items/Accessories/Vanity/Popo_Arms");
            AddEquipTexture(new PopoLegs(), null, EquipType.Legs, "PopoLeg", "CalamityMod/Items/Accessories/Vanity/Popo_Legs");

            AstralCactusTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactus");
            AstralCactusGlowTexture = ModContent.GetTexture("CalamityMod/ExtraTextures/Tiles/AstralCactusGlow");
            AstralSky = ModContent.GetTexture("CalamityMod/ExtraTextures/AstralSky");
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
            boomerangList = null;
            javelinList = null;
            daggerList = null;
            flaskBombList = null;
            spikyBallList = null;
            noGravityList = null;
            lavaFishList = null;

            zombieList = null;
            demonEyeList = null;
            skeletonList = null;
            angryBonesList = null;
            hornetList = null;
            mossHornetList = null;

            thorium = null;

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
                ModContent.ProjectileType<PhangasmBow>(),
                ModContent.ProjectileType<ContagionBow>(),
                ModContent.ProjectileType<DaemonsFlameBow>(),
                ModContent.ProjectileType<ExoTornado>(),
                ModContent.ProjectileType<DrataliornusBow>(),
                ModContent.ProjectileType<FlakKrakenGun>(),
                ModContent.ProjectileType<ButcherGun>(),
                ModContent.ProjectileType<StarfleetMK2Gun>(),
                ModContent.ProjectileType<TerraBulletSplit>(),
                ModContent.ProjectileType<TerraArrowSplit>(),
                ModContent.ProjectileType<OMGWTH>(),
                ModContent.ProjectileType<NorfleetCannon>(),
                ModContent.ProjectileType<NorfleetComet>(),
                ModContent.ProjectileType<NorfleetExplosion>()
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
                NPCID.DD2WyvernT1,
                NPCID.DD2WyvernT2,
                NPCID.DD2WyvernT3,
                NPCID.DD2DarkMageT1,
                NPCID.DD2DarkMageT3,
                NPCID.DD2SkeletonT1,
                NPCID.DD2SkeletonT3,
                NPCID.DD2WitherBeastT2,
                NPCID.DD2WitherBeastT3,
                NPCID.DD2DrakinT2,
                NPCID.DD2DrakinT3,
                NPCID.DD2KoboldWalkerT2,
                NPCID.DD2KoboldWalkerT3,
                NPCID.DD2KoboldFlyerT2,
                NPCID.DD2KoboldFlyerT3,
                NPCID.DD2OgreT2,
                NPCID.DD2OgreT3,
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
                ModContent.NPCType<PlaguebringerGoliath>(),
                ModContent.NPCType<PlaguebringerShade>(),
                ModContent.NPCType<PlagueBeeLargeG>(),
                ModContent.NPCType<PlagueBeeLarge>(),
                ModContent.NPCType<PlagueBeeG>(),
                ModContent.NPCType<PlagueBee>()
            };

            beeProjectileList = new List<int>()
            {
                ProjectileID.Stinger,
                ProjectileID.HornetStinger,
                ModContent.ProjectileType<PlagueStingerGoliath>(),
                ModContent.ProjectileType<PlagueStingerGoliathV2>(),
                ModContent.ProjectileType<PlagueExplosion>()
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
                ModContent.BuffType<SulphuricPoisoning>(),
                ModContent.BuffType<Shadowflame>(),
                ModContent.BuffType<BrimstoneFlames>(),
                ModContent.BuffType<BurningBlood>(),
                ModContent.BuffType<GlacialState>(),
                ModContent.BuffType<GodSlayerInferno>(),
                ModContent.BuffType<AstralInfectionDebuff>(),
                ModContent.BuffType<HolyFlames>(),
                ModContent.BuffType<Irradiated>(),
                ModContent.BuffType<Plague>(),
                ModContent.BuffType<AbyssalFlames>(),
                ModContent.BuffType<CrushDepth>(),
                ModContent.BuffType<Horror>(),
                ModContent.BuffType<MarkedforDeath>()
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
                ModContent.ItemType<AegisBlade>(),
                ModContent.ItemType<BalefulHarvester>(),
                ModContent.ItemType<Chaotrix>(),
                ModContent.ItemType<CometQuasher>(),
                ModContent.ItemType<DivineRetribution>(),
                ModContent.ItemType<DraconicDestruction>(),
                ModContent.ItemType<Drataliornus>(),
                ModContent.ItemType<EnergyStaff>(),
                ModContent.ItemType<ExsanguinationLance>(),
                ModContent.ItemType<FirestormCannon>(),
                ModContent.ItemType<FlameburstShortsword>(),
                ModContent.ItemType<FlameScythe>(),
                ModContent.ItemType<FlameScytheMelee>(),
                ModContent.ItemType<FlareBolt>(),
                ModContent.ItemType<FlarefrostBlade>(),
                ModContent.ItemType<FlarewingBow>(),
                ModContent.ItemType<ForbiddenSun>(),
                ModContent.ItemType<FrigidflashBolt>(),
                ModContent.ItemType<GreatbowofTurmoil>(),
                ModContent.ItemType<HarvestStaff>(),
                ModContent.ItemType<Hellborn>(),
                ModContent.ItemType<HellBurst>(),
                ModContent.ItemType<HellfireFlamberge>(),
                ModContent.ItemType<Hellkite>(),
                ModContent.ItemType<HellwingStaff>(),
                ModContent.ItemType<Helstorm>(),
                ModContent.ItemType<HellsSun>(),
                ModContent.ItemType<InfernaCutter>(),
                ModContent.ItemType<Lazhar>(),
                ModContent.ItemType<MeteorFist>(),
                ModContent.ItemType<Mourningstar>(),
                ModContent.ItemType<PhoenixBlade>(),
                ModContent.ItemType<Photoviscerator>(),
                ModContent.ItemType<RedSun>(),
                ModContent.ItemType<SparkSpreader>(),
                ModContent.ItemType<SpectralstormCannon>(),
                ModContent.ItemType<SunGodStaff>(),
                ModContent.ItemType<SunSpiritStaff>(),
                ModContent.ItemType<TearsofHeaven>(),
                ModContent.ItemType<TerraFlameburster>(),
                ModContent.ItemType<TheEmpyrean>(),
                ModContent.ItemType<TheLastMourning>(),
                ModContent.ItemType<TheWand>(),
                ModContent.ItemType<VenusianTrident>(),
                ModContent.ItemType<Vesuvius>(),
                ModContent.ItemType<BlissfulBombardier>(),
                ModContent.ItemType<HolyCollider>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<PurgeGuzzler>(),
                ModContent.ItemType<SolarFlare>(),
                ModContent.ItemType<TelluricGlare>(),
                ModContent.ItemType<AngryChickenStaff>(),
                ModContent.ItemType<ChickenCannon>(),
                ModContent.ItemType<DragonRage>(),
                ModContent.ItemType<DragonsBreath>(),
                ModContent.ItemType<PhoenixFlameBarrage>(),
                ModContent.ItemType<ProfanedTrident>(),
                ModContent.ItemType<TheBurningSky>(),
                ModContent.ItemType<HeliumFlash>()
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
                ModContent.ItemType<DepthBlade>(),
                ModContent.ItemType<AbyssBlade>(),
                ModContent.ItemType<NeptunesBounty>(),
                ModContent.ItemType<AquaticDissolution>(),
                ModContent.ItemType<ArchAmaryllis>(),
                ModContent.ItemType<ThornBlossom>(),
                ModContent.ItemType<BiomeBlade>(),
                ModContent.ItemType<TrueBiomeBlade>(),
                ModContent.ItemType<OmegaBiomeBlade>(),
                ModContent.ItemType<BladedgeGreatbow>(),
                ModContent.ItemType<BlossomFlux>(),
                ModContent.ItemType<EvergladeSpray>(),
                ModContent.ItemType<FeralthornClaymore>(),
                ModContent.ItemType<Floodtide>(),
                ModContent.ItemType<FourSeasonsGalaxia>(),
                ModContent.ItemType<GammaFusillade>(),
                ModContent.ItemType<GleamingMagnolia>(),
                ModContent.ItemType<HarvestStaff>(),
                ModContent.ItemType<HellionFlowerSpear>(),
                ModContent.ItemType<Lazhar>(),
                ModContent.ItemType<LifefruitScythe>(),
                ModContent.ItemType<ManaRose>(),
                ModContent.ItemType<MangroveChakram>(),
                ModContent.ItemType<MangroveChakramMelee>(),
                ModContent.ItemType<MantisClaws>(),
                ModContent.ItemType<Mariana>(),
                ModContent.ItemType<Mistlestorm>(),
                ModContent.ItemType<Monsoon>(),
                ModContent.ItemType<Alluvion>(),
                ModContent.ItemType<Needler>(),
                ModContent.ItemType<NettlelineGreatbow>(),
                ModContent.ItemType<Quagmire>(),
                ModContent.ItemType<Shroomer>(),
                ModContent.ItemType<SolsticeClaymore>(),
                ModContent.ItemType<SporeKnife>(),
                ModContent.ItemType<Spyker>(),
                ModContent.ItemType<StormSaber>(),
                ModContent.ItemType<StormRuler>(),
                ModContent.ItemType<StormSurge>(),
                ModContent.ItemType<TarragonThrowingDart>(),
                ModContent.ItemType<TerraEdge>(),
                ModContent.ItemType<TerraLance>(),
                ModContent.ItemType<TerraRay>(),
                ModContent.ItemType<TerraShiv>(),
                ModContent.ItemType<Terratomere>(),
                ModContent.ItemType<TerraFlameburster>(),
                ModContent.ItemType<TheSwarmer>(),
                ModContent.ItemType<Verdant>(),
                ModContent.ItemType<Barinautical>(),
                ModContent.ItemType<DeepseaStaff>(),
                ModContent.ItemType<Downpour>(),
                ModContent.ItemType<SubmarineShocker>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<Archerfish>(),
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<BlackAnurian>(),
                ModContent.ItemType<CalamarisLament>(),
                ModContent.ItemType<HerringStaff>(),
                ModContent.ItemType<Lionfish>(),
                ModContent.ItemType<ShellfishStaff>(),
                ModContent.ItemType<ClamCrusher>(),
                ModContent.ItemType<ClamorRifle>(),
                ModContent.ItemType<Serpentine>(),
                ModContent.ItemType<UrchinFlail>(),
                ModContent.ItemType<CoralCannon>(),
                ModContent.ItemType<Shellshooter>(),
                ModContent.ItemType<SandDollar>(),
                ModContent.ItemType<MagicalConch>(),
                ModContent.ItemType<SnapClam>(),
                ModContent.ItemType<GacruxianMollusk>(),
                ModContent.ItemType<PolarisParrotfish>(),
                ModContent.ItemType<SparklingEmpress>(),
                ModContent.ItemType<YateveoBloom>()
            };

            alcoholList = new List<int>()
            {
                ModContent.BuffType<BloodyMaryBuff>(),
                ModContent.BuffType<CaribbeanRumBuff>(),
                ModContent.BuffType<CinnamonRollBuff>(),
                ModContent.BuffType<EverclearBuff>(),
                ModContent.BuffType<EvergreenGinBuff>(),
                ModContent.BuffType<FireballBuff>(),
                ModContent.BuffType<GrapeBeerBuff>(),
                ModContent.BuffType<MargaritaBuff>(),
                ModContent.BuffType<MoonshineBuff>(),
                ModContent.BuffType<MoscowMuleBuff>(),
                ModContent.BuffType<RedWineBuff>(),
                ModContent.BuffType<RumBuff>(),
                ModContent.BuffType<ScrewdriverBuff>(),
                ModContent.BuffType<StarBeamRyeBuff>(),
                ModContent.BuffType<TequilaBuff>(),
                ModContent.BuffType<TequilaSunriseBuff>(),
                ModContent.BuffType<VodkaBuff>(),
                ModContent.BuffType<WhiskeyBuff>(),
                ModContent.BuffType<WhiteWineBuff>()
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

                ModContent.NPCType<DesertScourgeHead>(),
                ModContent.NPCType<DesertScourgeHeadSmall>(),
                ModContent.NPCType<DriedSeekerHead>(),

                ModContent.NPCType<CrabulonIdle>(),
                ModContent.NPCType<CrabShroom>(),

                ModContent.NPCType<HiveMind>(),
                ModContent.NPCType<HiveMindP2>(),
                ModContent.NPCType<DankCreeper>(),

                ModContent.NPCType<PerforatorHeadLarge>(),
                ModContent.NPCType<PerforatorHeadMedium>(),
                ModContent.NPCType<PerforatorHeadSmall>(),
                ModContent.NPCType<PerforatorHive>(),

                ModContent.NPCType<SlimeGod>(),
                ModContent.NPCType<SlimeGodRun>(),
                ModContent.NPCType<SlimeGodCore>(),
                ModContent.NPCType<SlimeGodSplit>(),
                ModContent.NPCType<SlimeGodRunSplit>(),
                ModContent.NPCType<SlimeSpawnCorrupt>(),
                ModContent.NPCType<SlimeSpawnCrimson2>(),

                ModContent.NPCType<Cryogen>(),
                ModContent.NPCType<CryogenIce>(),
                ModContent.NPCType<Cryocore>(),
                ModContent.NPCType<Cryocore2>(),
                ModContent.NPCType<IceMass>(),

                ModContent.NPCType<AquaticScourgeHead>(),
                ModContent.NPCType<AquaticScourgeBody>(),
                ModContent.NPCType<AquaticScourgeBodyAlt>(),
                ModContent.NPCType<AquaticScourgeTail>(),

                ModContent.NPCType<BrimstoneElemental>(),

                ModContent.NPCType<Calamitas>(),
                ModContent.NPCType<CalamitasRun3>(),
                ModContent.NPCType<CalamitasRun>(),
                ModContent.NPCType<CalamitasRun2>(),
                ModContent.NPCType<LifeSeeker>(),
                ModContent.NPCType<SoulSeeker>(),

                ModContent.NPCType<Parasea>(),
                ModContent.NPCType<AquaticAberration>(),
                ModContent.NPCType<Leviathan>(),
                ModContent.NPCType<SirenIce>(),
                ModContent.NPCType<Siren>(),

                ModContent.NPCType<AstrumAureus>(),

                ModContent.NPCType<PlaguebringerGoliath>(),
                ModContent.NPCType<PlaguebringerShade>(),
                ModContent.NPCType<PlagueHomingMissile>(),
                ModContent.NPCType<PlagueBeeG>(),
                ModContent.NPCType<PlagueBeeLargeG>(),

                ModContent.NPCType<FlamePillar>(),
                ModContent.NPCType<RockPillar>(),
                ModContent.NPCType<RavagerHead2>(),
                ModContent.NPCType<RavagerBody>(),
                ModContent.NPCType<RavagerClawRight>(),
                ModContent.NPCType<RavagerClawLeft>(),

                ModContent.NPCType<AstrumDeusHead>(),
                ModContent.NPCType<AstrumDeusHeadSpectral>(),

                ModContent.NPCType<ProfanedGuardianBoss>(),
                ModContent.NPCType<ProfanedGuardianBoss2>(),
                ModContent.NPCType<ProfanedGuardianBoss3>(),

                ModContent.NPCType<Bumblefuck>(),
                ModContent.NPCType<Bumblefuck2>(),

                // TODO -- Old Duke isn't added yet.
                // ModContent.NPCType<OldDuke>(),
                // ModContent.NPCType<DukeUrchin>(),

                ModContent.NPCType<ProvSpawnDefense>(),
                ModContent.NPCType<ProvSpawnOffense>(),

                ModContent.NPCType<CeaselessVoid>(),

                ModContent.NPCType<StormWeaverHead>(),
                ModContent.NPCType<StormWeaverHeadNaked>(),

                ModContent.NPCType<Signus>(),
                ModContent.NPCType<CosmicLantern>(),

                ModContent.NPCType<Polterghast>(),
                ModContent.NPCType<PolterPhantom>(),

                ModContent.NPCType<DevourerofGodsHead>(),
                ModContent.NPCType<DevourerofGodsBody>(),
                ModContent.NPCType<DevourerofGodsTail>(),
                ModContent.NPCType<DevourerofGodsHead2>(),
                ModContent.NPCType<DevourerofGodsHeadS>(),
                ModContent.NPCType<DevourerofGodsBodyS>(),
                ModContent.NPCType<DevourerofGodsTailS>(),

                ModContent.NPCType<Yharon>(),
                ModContent.NPCType<DetonatingFlare>(),
                ModContent.NPCType<DetonatingFlare2>(),

                ModContent.NPCType<SupremeCalamitas>(),

                ModContent.NPCType<BobbitWormHead>(),
                ModContent.NPCType<AquaticSeekerHead>(),
                ModContent.NPCType<ColossalSquid>(),
                ModContent.NPCType<EidolonWyrmHead>(),
                ModContent.NPCType<EidolonWyrmHeadHuge>(),
                ModContent.NPCType<GulperEelHead>(),
                ModContent.NPCType<Mauler>(),
                ModContent.NPCType<Reaper>(),

                ModContent.NPCType<Atlas>(),
                ModContent.NPCType<ArmoredDiggerHead>(),
                ModContent.NPCType<Cnidrion>(),
                ModContent.NPCType<GreatSandShark>(),
                ModContent.NPCType<Horse>(),
                ModContent.NPCType<ScornEater>(),

                ModContent.NPCType<PrismTurtle>(),
                ModContent.NPCType<GhostBell>(),
                ModContent.NPCType<EutrophicRay>(),
                ModContent.NPCType<Clam>(),
                ModContent.NPCType<SeaSerpent1>(),
                ModContent.NPCType<BlindedAngler>(),
                ModContent.NPCType<GiantClam>()
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

                ModContent.ProjectileType<SandBlast>(),

                ModContent.ProjectileType<MushBomb>(),
                ModContent.ProjectileType<MushBombFall>(),
                ModContent.ProjectileType<Mushmash>(),

                ModContent.ProjectileType<ShaderainHostile>(),

                ModContent.ProjectileType<BloodGeyser>(),
                ModContent.ProjectileType<IchorShot>(),

                ModContent.ProjectileType<AbyssMine>(),
                ModContent.ProjectileType<AbyssMine2>(),
                ModContent.ProjectileType<AbyssBallVolley>(),
                ModContent.ProjectileType<AbyssBallVolley2>(),

                ModContent.ProjectileType<IceBlast>(),
                ModContent.ProjectileType<IceBomb>(),
                ModContent.ProjectileType<IceRain>(),

                ModContent.ProjectileType<SandPoisonCloud>(),
                ModContent.ProjectileType<SandTooth>(),

                ModContent.ProjectileType<BrimstoneHellfireball>(),
                ModContent.ProjectileType<HellfireExplosion>(),
                ModContent.ProjectileType<BrimstoneBarrage>(),
                ModContent.ProjectileType<BrimstoneHellblast>(),

                ModContent.ProjectileType<BrimstoneLaser>(),
                ModContent.ProjectileType<BrimstoneLaserSplit>(),
                ModContent.ProjectileType<BrimstoneBall>(),
                ModContent.ProjectileType<BrimstoneFire>(),

                ModContent.ProjectileType<LeviathanBomb>(),
                ModContent.ProjectileType<WaterSpear>(),
                ModContent.ProjectileType<FrostMist>(),
                ModContent.ProjectileType<SirenSong>(),

                ModContent.ProjectileType<AstralFlame>(),
                ModContent.ProjectileType<AstralLaser>(),

                ModContent.ProjectileType<PlagueExplosion>(),
                ModContent.ProjectileType<PlagueStingerGoliath>(),
                ModContent.ProjectileType<PlagueStingerGoliathV2>(),
                ModContent.ProjectileType<HiveBombGoliath>(),

                ModContent.ProjectileType<ScavengerNuke>(),

                ModContent.ProjectileType<AstralShot2>(),
                ModContent.ProjectileType<DeusMine>(),

                ModContent.ProjectileType<ProfanedSpear>(),
                ModContent.ProjectileType<FlareDust>(),

                ModContent.ProjectileType<RedLightningFeather>(),

                ModContent.ProjectileType<HolyBlast>(),
                ModContent.ProjectileType<HolyBomb>(),
                ModContent.ProjectileType<HolyFire>(),
                ModContent.ProjectileType<HolyFire2>(),
                ModContent.ProjectileType<HolyFlare>(),
                ModContent.ProjectileType<HolyShot>(),
                ModContent.ProjectileType<HolySpear>(),
                ModContent.ProjectileType<MoltenBlast>(),
                ModContent.ProjectileType<MoltenBlob>(),
                ModContent.ProjectileType<ProvidenceCrystalShard>(),
                ModContent.ProjectileType<ProvidenceHolyRay>(),

                ModContent.ProjectileType<DarkEnergyBall>(),
                ModContent.ProjectileType<DoGBeam>(),

                ModContent.ProjectileType<CosmicFlameBurst>(),
                ModContent.ProjectileType<SignusScythe>(),
                ModContent.ProjectileType<EssenceDust>(),

                ModContent.ProjectileType<PhantomBlast>(),
                ModContent.ProjectileType<PhantomBlast2>(),
                ModContent.ProjectileType<PhantomGhostShot>(),
                ModContent.ProjectileType<PhantomHookShot>(),
                ModContent.ProjectileType<PhantomMine>(),
                ModContent.ProjectileType<PhantomShot>(),
                ModContent.ProjectileType<PhantomShot2>(),

                ModContent.ProjectileType<DoGDeath>(),
                ModContent.ProjectileType<DoGFire>(),
                ModContent.ProjectileType<DoGNebulaShot>(),

                ModContent.ProjectileType<FlareBomb>(),
                ModContent.ProjectileType<FlareDust2>(),
                ModContent.ProjectileType<Flarenado>(),
                ModContent.ProjectileType<Infernado>(),
                ModContent.ProjectileType<Infernado2>(),
                ModContent.ProjectileType<YharonFireball>(),
                ModContent.ProjectileType<YharonFireball2>(),

                ModContent.ProjectileType<BrimstoneFireblast>(),
                ModContent.ProjectileType<BrimstoneGigaBlast>(),
                ModContent.ProjectileType<BrimstoneHellblast2>(),
                ModContent.ProjectileType<BrimstoneMonster>(),
                ModContent.ProjectileType<BrimstoneWave>(),

                ModContent.ProjectileType<GreatSandBlast>(),
                ModContent.ProjectileType<PearlBurst>(),
                ModContent.ProjectileType<PearlRain>()
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
                ModContent.ItemType<AMR>(),
                ModContent.ItemType<HalleysInferno>(),
                ModContent.ItemType<Shroomer>(),
                ModContent.ItemType<SpectreRifle>(),
                ModContent.ItemType<Svantechnical>(),
                ModContent.ItemType<Skullmasher>()
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

                // flails
                ModContent.ItemType<BallOFugu>(),
                ModContent.ItemType<DragonPow>(),

                // spears
                ModContent.ItemType<AmidiasTrident>(),
                ModContent.ItemType<EarthenPike>(),
                ModContent.ItemType<GoldplumeSpear>(),
                ModContent.ItemType<HellionFlowerSpear>(),
                ModContent.ItemType<SpatialLance>(),
                ModContent.ItemType<StarnightLance>(),
                ModContent.ItemType<StreamGouge>(),
                ModContent.ItemType<TerraLance>(),
                ModContent.ItemType<UrchinSpear>(),

                // yoyos
                ModContent.ItemType<AirSpinner>(),
                ModContent.ItemType<Aorta>(),
                ModContent.ItemType<Azathoth>(),
                ModContent.ItemType<Chaotrix>(),
                ModContent.ItemType<Cnidarian>(),
                ModContent.ItemType<Lacerator>(),
                ModContent.ItemType<Oracle>(),
                ModContent.ItemType<Quagmire>(),
                ModContent.ItemType<Shimmerspark>(),
                ModContent.ItemType<SolarFlare>(),
                ModContent.ItemType<TheEyeofCalamitas>(),
                ModContent.ItemType<TheGodsGambit>(),
                ModContent.ItemType<TheObliterator>(),
                ModContent.ItemType<ThePlaguebringer>(),
                ModContent.ItemType<Verdant>(),
                ModContent.ItemType<YinYo>(),

                // other
                ModContent.ItemType<BansheeHook>(),
                ModContent.ItemType<TyphonsGreed>()
            };

            boomerangList = new List<int>()
            {
                ModContent.ItemType<Brimblade>(),
                ModContent.ItemType<BlazingStar>(),
                ModContent.ItemType<Celestus>(),
                ModContent.ItemType<AccretionDisk>(),
                ModContent.ItemType<EnchantedAxe>(),
                ModContent.ItemType<EpidemicShredder>(),
                ModContent.ItemType<Eradicator>(),
                ModContent.ItemType<TruePaladinsHammer>(),
                ModContent.ItemType<FlameScythe>(),
                ModContent.ItemType<GalaxySmasherRogue>(),
                ModContent.ItemType<Glaive>(),
                ModContent.ItemType<GhoulishGouger>(),
                ModContent.ItemType<Icebreaker>(),
                ModContent.ItemType<KelvinCatalyst>(),
                ModContent.ItemType<Kylie>(),
                ModContent.ItemType<MangroveChakram>(),
                ModContent.ItemType<MoltenAmputator>(),
                ModContent.ItemType<NanoblackReaperRogue>(),
                ModContent.ItemType<Pwnagehammer>(),
                ModContent.ItemType<SandDollar>(),
                ModContent.ItemType<SeashellBoomerang>(),
                ModContent.ItemType<StellarContemptRogue>(),
                ModContent.ItemType<TriactisTruePaladinianMageHammerofMight>(),
                ModContent.ItemType<Valediction>()
            };

            javelinList = new List<int>()
            {
                ModContent.ItemType<CrystalPiercer>(),
                ModContent.ItemType<PalladiumJavelin>(),
                ModContent.ItemType<DuneHopper>(),
                ModContent.ItemType<EclipsesFall>(),
                ModContent.ItemType<IchorSpear>(),
                ModContent.ItemType<ProfanedTrident>(),
                ModContent.ItemType<LuminousStriker>(),
                ModContent.ItemType<ScarletDevil>(),
                ModContent.ItemType<ScourgeoftheCosmosThrown>(),
                ModContent.ItemType<ScourgeoftheDesert>(),
                ModContent.ItemType<ScourgeoftheSeas>(),
                ModContent.ItemType<SpearofDestiny>(),
                ModContent.ItemType<SpearofPaleolith>(),
                ModContent.ItemType<XerocPitchfork>(),
            };

            daggerList = new List<int>()
            {
                ModContent.ItemType<CobaltKunai>(),
                ModContent.ItemType<FeatherKnife>(),
                ModContent.ItemType<GelDart>(),
                ModContent.ItemType<MonkeyDarts>(),
                ModContent.ItemType<MythrilKnife>(),
                ModContent.ItemType<OrichalcumSpikedGemstone>(),
                ModContent.ItemType<TarragonThrowingDart>(),
                ModContent.ItemType<WulfrumKnife>(),
                ModContent.ItemType<Cinquedea>(),
                ModContent.ItemType<CosmicKunai>(),
                ModContent.ItemType<CorpusAvertor>(),
                ModContent.ItemType<Crystalline>(),
                ModContent.ItemType<CursedDagger>(),
                ModContent.ItemType<Malachite>(),
                ModContent.ItemType<Mycoroot>(),
                ModContent.ItemType<Prismalline>(),
                ModContent.ItemType<Quasar>(),
                ModContent.ItemType<RadiantStar>(),
                ModContent.ItemType<ShatteredSun>(),
                ModContent.ItemType<StellarKnife>(),
                ModContent.ItemType<StormfrontRazor>(),
                ModContent.ItemType<TimeBolt>(),
                ModContent.ItemType<LunarKunai>()
            };

            flaskBombList = new List<int>()
            {
                ModContent.ItemType<Plaguenade>(),
                ModContent.ItemType<BallisticPoisonBomb>(),
                ModContent.ItemType<BrackishFlask>(),
                ModContent.ItemType<DuststormInABottle>(),
                ModContent.ItemType<SeafoamBomb>(),
                ModContent.ItemType<ConsecratedWater>(),
                ModContent.ItemType<BouncingBetty>(),
                ModContent.ItemType<BlastBarrel>()
            };

            spikyBallList = new List<int>() //There's more to come
            {
                ModContent.ItemType<HellsSun>(),
                ModContent.ItemType<SkyStabber>()
            };

            noGravityList = new List<int>()
            {
                ModContent.ItemType<AuricBar>(),
                ModContent.ItemType<EssenceofChaos>(),
                ModContent.ItemType<EssenceofCinder>(),
                ModContent.ItemType<EssenceofEleum>(),
                ModContent.ItemType<CoreofChaos>(),
                ModContent.ItemType<CoreofCinder>(),
                ModContent.ItemType<CoreofEleum>(),
                ModContent.ItemType<CoreofCalamity>(),
                ModContent.ItemType<CalamitousEssence>(),
                ModContent.ItemType<HellcasterFragment>(),
                ModContent.ItemType<TwistingNether>(),
                ModContent.ItemType<DarkPlasma>(),
                ModContent.ItemType<DarksunFragment>(),
                ModContent.ItemType<UnholyEssence>(),
                ModContent.ItemType<GalacticaSingularity>(),
                ModContent.ItemType<NightmareFuel>(),
                ModContent.ItemType<EndothermicEnergy>()
            };

            lavaFishList = new List<int>()
            {
                ModContent.ItemType<SlurperPole>(),
                ModContent.ItemType<ChaoticSpreadRod>(),
                ModContent.ItemType<TheDevourerofCods>()
            };

            tableList = new List<int>()
            {
                ModContent.TileType<AbyssBath>(),
                ModContent.TileType<AbyssBookcase>(),
                ModContent.TileType<AbyssDresser>(),
                ModContent.TileType<AbyssPiano>(),
                ModContent.TileType<AbyssTable>(),
                ModContent.TileType<AbyssWorkbench>(),
                ModContent.TileType<AncientBath>(),
                ModContent.TileType<AncientBookcase>(),
                ModContent.TileType<AncientDresser>(),
                ModContent.TileType<AncientPiano>(),
                ModContent.TileType<AncientTable>(),
                ModContent.TileType<AncientWorkbench>(),
                ModContent.TileType<AshenBath>(),
                ModContent.TileType<AshenBookcase>(),
                ModContent.TileType<AshenDresser>(),
                ModContent.TileType<AshenPiano>(),
                ModContent.TileType<AshenTable>(),
                ModContent.TileType<AshenWorkbench>(),
                ModContent.TileType<BotanicBathtub>(),
                ModContent.TileType<BotanicBookcase>(),
                ModContent.TileType<BotanicDresser>(),
                ModContent.TileType<BotanicPiano>(),
                ModContent.TileType<BotanicTable>(),
                ModContent.TileType<BotanicWorkBench>(),
                ModContent.TileType<CosmiliteBath>(),
                ModContent.TileType<CosmiliteBookcase>(),
                ModContent.TileType<CosmiliteDresser>(),
                ModContent.TileType<CosmilitePiano>(),
                ModContent.TileType<CosmiliteTable>(),
                ModContent.TileType<CosmiliteWorkbench>(),
                ModContent.TileType<EutrophicBathtub>(),
                ModContent.TileType<EutrophicBookcase>(),
                ModContent.TileType<EutrophicDresser>(),
                ModContent.TileType<EutrophicPiano>(),
                ModContent.TileType<EutrophicTable>(),
                ModContent.TileType<EutrophicWorkBench>(),
                ModContent.TileType<OccultBathtub>(),
                ModContent.TileType<OccultBookcase>(),
                ModContent.TileType<OccultDresser>(),
                ModContent.TileType<OccultPiano>(),
                ModContent.TileType<OccultTable>(),
                ModContent.TileType<OccultWorkBench>(),
                ModContent.TileType<PlaguedPlateBathtub>(),
                ModContent.TileType<PlaguedPlateBookcase>(),
                ModContent.TileType<PlaguedPlateDresser>(),
                ModContent.TileType<PlaguedPlatePiano>(),
                ModContent.TileType<PlaguedPlateTable>(),
                ModContent.TileType<PlaguedPlateWorkbench>(),
                ModContent.TileType<ProfanedBath>(),
                ModContent.TileType<ProfanedBookcase>(),
                ModContent.TileType<ProfanedDresser>(),
                ModContent.TileType<ProfanedPiano>(),
                ModContent.TileType<ProfanedTable>(),
                ModContent.TileType<ProfanedWorkbench>(),
                ModContent.TileType<SilvaBathtub>(),
                ModContent.TileType<SilvaBookcase>(),
                ModContent.TileType<SilvaDresser>(),
                ModContent.TileType<SilvaPiano>(),
                ModContent.TileType<SilvaTable>(),
                ModContent.TileType<SilvaWorkBench>(),
                ModContent.TileType<StatigelBath>(),
                ModContent.TileType<StatigelBookcase>(),
                ModContent.TileType<StatigelDresser>(),
                ModContent.TileType<StatigelPiano>(),
                ModContent.TileType<StatigelTable>(),
                ModContent.TileType<StatigelWorkbench>(),
                ModContent.TileType<StratusBathtub>(),
                ModContent.TileType<StratusBookcase>(),
                ModContent.TileType<StratusDresser>(),
                ModContent.TileType<StratusPiano>(),
                ModContent.TileType<StratusTable>(),
                ModContent.TileType<StratusWorkbench>(),
                ModContent.TileType<VoidBath>(),
                ModContent.TileType<VoidBookcase>(),
                ModContent.TileType<VoidDresser>(),
                ModContent.TileType<VoidPiano>(),
                ModContent.TileType<VoidTable>(),
                ModContent.TileType<VoidWorkbench>()
            };

            chairList = new List<int>()
            {
                ModContent.TileType<AbyssBed>(),
                ModContent.TileType<AbyssChair>(),
                ModContent.TileType<AbyssSofa>(),
                ModContent.TileType<AncientBed>(),
                ModContent.TileType<AncientChair>(),
                ModContent.TileType<AncientSofa>(),
                ModContent.TileType<AshenBed>(),
                ModContent.TileType<AshenChair>(),
                ModContent.TileType<AshenSofa>(),
                ModContent.TileType<BotanicBed>(),
                ModContent.TileType<BotanicBench>(),
                ModContent.TileType<BotanicChair>(),
                ModContent.TileType<CosmiliteBed>(),
                ModContent.TileType<CosmiliteChair>(),
                ModContent.TileType<CosmiliteSofa>(),
                ModContent.TileType<EutrophicBed>(),
                ModContent.TileType<EutrophicBench>(),
                ModContent.TileType<EutrophicChair>(),
                ModContent.TileType<OccultBed>(),
                ModContent.TileType<OccultChair>(),
                ModContent.TileType<OccultSofa>(),
                ModContent.TileType<PlaguedPlateBed>(),
                ModContent.TileType<PlaguedPlateChair>(),
                ModContent.TileType<PlaguedPlateSofa>(),
                ModContent.TileType<ProfanedBed>(),
                ModContent.TileType<ProfanedChair>(),
                ModContent.TileType<ProfanedBench>(),
                ModContent.TileType<SilvaBed>(),
                ModContent.TileType<SilvaChair>(),
                ModContent.TileType<SilvaBench>(),
                ModContent.TileType<StatigelBed>(),
                ModContent.TileType<StatigelChair>(),
                ModContent.TileType<StatigelSofa>(),
                ModContent.TileType<StratusBed>(),
                ModContent.TileType<StratusChair>(),
                ModContent.TileType<StratusSofa>(),
                ModContent.TileType<VoidBed>(),
                ModContent.TileType<VoidChair>(),
                ModContent.TileType<VoidSofa>()
            };

            lightList = new List<int>()
            {
                ModContent.TileType<BlueCandle>(),
                ModContent.TileType<ChaosCandle>(),
                ModContent.TileType<LumenylCrystals>(),
                ModContent.TileType<PinkCandle>(),
                ModContent.TileType<PurpleCandle>(),
                ModContent.TileType<TranquilityCandle>(),
                ModContent.TileType<YellowCandle>(),
                ModContent.TileType<SeaPrismCrystals>(),

                ModContent.TileType<AbyssCandelabra>(),
                ModContent.TileType<AbyssCandle>(),
                ModContent.TileType<AbyssChandelier>(),
                ModContent.TileType<AbyssLamp>(),
                ModContent.TileType<AbyssLantern>(),
                ModContent.TileType<AbyssTorch>(),
                ModContent.TileType<AncientAltar>(),
                ModContent.TileType<AncientBasin>(),
                ModContent.TileType<AncientChandelier>(),
                ModContent.TileType<AncientLamp>(),
                ModContent.TileType<AncientLantern>(),
                ModContent.TileType<AshenAltar>(),
                ModContent.TileType<AshenBasin>(),
                ModContent.TileType<AshenChandelier>(),
                ModContent.TileType<AshenCandle>(),
                ModContent.TileType<AshenCandelabra>(),
                ModContent.TileType<AshenLamp>(),
                ModContent.TileType<AshenLantern>(),
                ModContent.TileType<BotanicCandle>(),
                ModContent.TileType<BotanicCandelabra>(),
                ModContent.TileType<BotanicChandelier>(),
                ModContent.TileType<BotanicLamp>(),
                ModContent.TileType<BotanicLantern>(),
                ModContent.TileType<CosmiliteCandle>(),
                ModContent.TileType<CosmiliteCandelabra>(),
                ModContent.TileType<CosmiliteChandelier>(),
                ModContent.TileType<CosmiliteLamp>(),
                ModContent.TileType<CosmiliteLantern>(),
                ModContent.TileType<CosmiliteSconce>(),
                ModContent.TileType<EutrophicCandle>(),
                ModContent.TileType<EutrophicCandelabra>(),
                ModContent.TileType<EutrophicChandelier>(),
                ModContent.TileType<EutrophicLamp>(),
                ModContent.TileType<EutrophicLantern>(),
                ModContent.TileType<PlaguedPlateBasin>(),
                ModContent.TileType<PlaguedPlateCandle>(),
                ModContent.TileType<PlaguedPlateCandelabra>(),
                ModContent.TileType<PlaguedPlateChandelier>(),
                ModContent.TileType<PlaguedPlateLamp>(),
                ModContent.TileType<PlaguedPlateLantern>(),
                ModContent.TileType<OccultCandle>(),
                ModContent.TileType<OccultCandelabra>(),
                ModContent.TileType<OccultChandelier>(),
                ModContent.TileType<OccultLamp>(),
                ModContent.TileType<OccultLantern>(),
                ModContent.TileType<ProfanedBasin>(),
                ModContent.TileType<ProfanedCandle>(),
                ModContent.TileType<ProfanedCandelabra>(),
                ModContent.TileType<ProfanedChandelier>(),
                ModContent.TileType<ProfanedLamp>(),
                ModContent.TileType<ProfanedLantern>(),
                ModContent.TileType<SilvaBasin>(),
                ModContent.TileType<SilvaCandle>(),
                ModContent.TileType<SilvaCandelabra>(),
                ModContent.TileType<SilvaChandelier>(),
                ModContent.TileType<SilvaLamp>(),
                ModContent.TileType<SilvaLantern>(),
                ModContent.TileType<StatigelChandelier>(),
                ModContent.TileType<StatigelCandle>(),
                ModContent.TileType<StatigelCandelabra>(),
                ModContent.TileType<StatigelLamp>(),
                ModContent.TileType<StatigelLantern>(),
                ModContent.TileType<StratusCandle>(),
                ModContent.TileType<StratusCandelabra>(),
                ModContent.TileType<StratusChandelier>(),
                ModContent.TileType<StratusLantern>(),
                ModContent.TileType<StratusLamp>(),
                ModContent.TileType<VoidCandle>(),
                ModContent.TileType<VoidCandelabra>(),
                ModContent.TileType<VoidChandelier>(),
                ModContent.TileType<VoidLamp>(),
                ModContent.TileType<VoidLantern>()
            };

            doorList = new List<int>()
            {
                ModContent.TileType<AbyssDoorOpen>(),
                ModContent.TileType<AbyssDoorClosed>(),
                ModContent.TileType<AncientDoorOpen>(),
                ModContent.TileType<AncientDoorClosed>(),
                ModContent.TileType<AshenDoorClosed>(),
                ModContent.TileType<AshenDoorOpen>(),
                ModContent.TileType<AshenPlatform>(),
                ModContent.TileType<BotanicDoorOpen>(),
                ModContent.TileType<BotanicDoorClosed>(),
                ModContent.TileType<BotanicPlatform>(),
                ModContent.TileType<CosmiliteDoorOpen>(),
                ModContent.TileType<CosmiliteDoorClosed>(),
                ModContent.TileType<CosmilitePlatform>(),
                ModContent.TileType<EutrophicDoorOpen>(),
                ModContent.TileType<EutrophicDoorClosed>(),
                ModContent.TileType<EutrophicPlatform>(),
                ModContent.TileType<OccultDoorOpen>(),
                ModContent.TileType<OccultDoorClosed>(),
                ModContent.TileType<OccultPlatform>(),
                ModContent.TileType<PlaguedPlateDoorOpen>(),
                ModContent.TileType<PlaguedPlateDoorClosed>(),
                ModContent.TileType<PlaguedPlatePlatform>(),
                ModContent.TileType<ProfanedDoorOpen>(),
                ModContent.TileType<ProfanedDoorClosed>(),
                ModContent.TileType<ProfanedPlatform>(),
                ModContent.TileType<SilvaDoorOpen>(),
                ModContent.TileType<SilvaDoorClosed>(),
                ModContent.TileType<SilvaPlatform>(),
                ModContent.TileType<SmoothAbyssGravelPlatform>(),
                ModContent.TileType<SmoothVoidstonePlatform>(),
                ModContent.TileType<StratusDoorOpen>(),
                ModContent.TileType<StratusDoorClosed>(),
                ModContent.TileType<StatigelPlatform>(),
                ModContent.TileType<StatigelDoorOpen>(),
                ModContent.TileType<StatigelDoorClosed>(),
                ModContent.TileType<StratusPlatform>(),
                ModContent.TileType<VoidDoorOpen>(),
                ModContent.TileType<VoidDoorClosed>()
            };

            zombieList = new List<int>()
            {
                NPCID.Zombie,
                NPCID.ArmedZombie,
                NPCID.BaldZombie,
                NPCID.PincushionZombie,
                NPCID.ArmedZombiePincussion, // what is this spelling
                NPCID.SlimedZombie,
                NPCID.ArmedZombieSlimed,
                NPCID.SwampZombie,
                NPCID.ArmedZombieSwamp,
                NPCID.TwiggyZombie,
                NPCID.ArmedZombieTwiggy,
                NPCID.FemaleZombie,
                NPCID.ArmedZombieCenx,
                NPCID.ZombieRaincoat,
                NPCID.ZombieEskimo,
                NPCID.ArmedZombieEskimo
                // halloween zombies not included because they don't drop shackles or zombie arms
            };

            demonEyeList = new List<int>()
            {
                NPCID.DemonEye,
                NPCID.CataractEye,
                NPCID.SleepyEye,
                NPCID.DialatedEye, // it is spelled "dilated"
                NPCID.GreenEye,
                NPCID.PurpleEye,
                NPCID.DemonEyeOwl,
                NPCID.DemonEyeSpaceship
            };

            skeletonList = new List<int>()
            {
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
                NPCID.AngryBonesBig,
                NPCID.AngryBonesBigMuscle,
                NPCID.AngryBonesBigHelmet
            };

            hornetList = new List<int>()
            {
                NPCID.Hornet,
                NPCID.HornetFatty,
                NPCID.HornetHoney,
                NPCID.HornetLeafy,
                NPCID.HornetSpikey,
                NPCID.HornetStingy
            };

            mossHornetList = new List<int>()
            {
                NPCID.MossHornet,
                NPCID.TinyMossHornet,
                NPCID.LittleMossHornet,
                NPCID.BigMossHornet,
                NPCID.GiantMossHornet
            };

            Mod thorium = ModLoader.GetMod("ThoriumMod");
            if (Config.RevengeanceAndDeathThoriumBossBuff && thorium != null)
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

            void ThoriumDR(string npcName, float dr) {
                int type = thorium.NPCType(npcName);
                if (DRValues.ContainsKey(type))
                    DRValues[type] = dr;
                else
                    DRValues.Add(type, dr);
            };

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
            if (CalamityWorld.revenge && Config.AdrenalineAndRage)
            {
                UIHandler.ModifyInterfaceLayers(ModContent.GetInstance<CalamityMod>(), layers);
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
