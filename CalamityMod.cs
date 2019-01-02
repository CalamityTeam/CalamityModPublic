using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Initializers;
using Terraria.IO;
using Terraria.Map;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.Localization;
using CalamityMod.NPCs;
using CalamityMod.NPCs.TheDevourerofGods;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Yharon;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.NPCs.Providence;
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Polterghast;
using CalamityMod.Tiles;
using CalamityMod.UI;
using CalamityMod.Skies;

namespace CalamityMod
{
    public class CalamityMod : Mod
    {
        public static ModHotKey AegisHotKey;

        public static ModHotKey TarraHotKey;

        public static ModHotKey RageHotKey;

        public static ModHotKey AdrenalineHotKey;

        public static ModHotKey AstralTeleportHotKey;

        public static ModHotKey AstralArcanumUIHotkey;

        public static ModHotKey BossBarToggleHotKey;

        public static ModHotKey BossBarToggleSmallTextHotKey;

        public static int ghostKillCount = 0;

        public static int sharkKillCount = 0;

        public static Texture2D heartOriginal2;

        public static Texture2D heartOriginal;

        public static Texture2D rainOriginal;

        public static Texture2D manaOriginal;

        public static Texture2D carpetOriginal;

        public static Texture2D AstralCactusTexture;

        public static Texture2D AstralCactusGlowTexture;

        public static Texture2D AstralSky;

        public static Effect CustomShader;

        public static IList<string> donatorList;

        public static List<int> rangedProjectileExceptionList;

        public static List<int> projectileMinionList;

        public static List<int> enemyImmunityList;

        public static List<int> hardModeNerfExceptionList;

        public static List<int> dungeonEnemyBuffList;

        public static List<int> bossScaleList;

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

        public static List<int> lightGreenThrowingRarityList;

        public static List<int> greenThrowingRarityList;

        public static List<int> blueThrowingRarityList;

        public static List<int> purpleThrowingRarityList;

        public static List<int> throwingProjectileList;

        public static List<int> throwingItemList;

        public static List<int> pumpkinMoonBuffList;

        public static List<int> frostMoonBuffList;

        public static List<int> eclipseBuffList;

        public static List<int> eventProjectileBuffList;

        public static List<int> revengeanceEnemyBuffList;

        public static List<int> revengeanceProjectileBuffList;

        public static List<int> trapProjectileList;

        public static CalamityMod Instance;

        public CalamityMod()
    	{
            Instance = this;
        }

        #region Load
        public override void Load()
		{
            Main.tile = new Tile[8401, 2601];
            Main.Map = new Terraria.Map.WorldMap(Main.maxTilesX, 2601);
            Main.mapTargetY = 3;
            Main.instance.mapTarget = new RenderTarget2D[Main.mapTargetX, Main.mapTargetY];
            Main.initMap = new bool[Main.mapTargetX, Main.mapTargetY];
            Main.mapWasContentLost = new bool[Main.mapTargetX, Main.mapTargetY];

            heartOriginal2 = Main.heartTexture;
            heartOriginal = Main.heart2Texture;
            rainOriginal = Main.rainTexture;
            manaOriginal = Main.manaTexture;
            carpetOriginal = Main.flyingCarpetTexture;

            RageHotKey = RegisterHotKey("Rage Mode", "V");
            AdrenalineHotKey = RegisterHotKey("Adrenaline Mode", "B");
            AegisHotKey = RegisterHotKey("Elysian Guard", "N");
            TarraHotKey = RegisterHotKey("Armor Set Bonus", "Y");
            AstralTeleportHotKey = RegisterHotKey("Astral Teleport", "P");
            AstralArcanumUIHotkey = RegisterHotKey("Astral Arcanum UI Toggle", "O");
            BossBarToggleHotKey = RegisterHotKey("Boss Health Bar Toggle", "NumPad0");
            BossBarToggleSmallTextHotKey = RegisterHotKey("Boss Health Bar Small Text Toggle", "NumPad1");

            if (!Main.dedServ)
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

                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Cryogen"), ItemType("CryogenMusicbox"), TileType("CryogenMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Calamitas"), ItemType("CalamitasMusicbox"), TileType("CalamitasMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/ScourgeofTheUniverse"), ItemType("DoGMusicbox"), TileType("DoGMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Sulphur"), ItemType("SulphurousMusicbox"), TileType("SulphurousMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/TheAbyss"), ItemType("HigherAbyssMusicbox"), TileType("HigherAbyssMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/PlaguebringerGoliath"), ItemType("PlaguebringerMusicbox"), TileType("PlaguebringerMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/AquaticScourge"), ItemType("AquaticScourgeMusicbox"), TileType("AquaticScourgeMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Astrageldon"), ItemType("AstrageldonMusicbox"), TileType("AstrageldonMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Astral"), ItemType("AstralMusicbox"), TileType("AstralMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/RUIN"), ItemType("PolterghastMusicbox"), TileType("PolterghastMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Signus"), ItemType("SignusMusicbox"), TileType("SignusMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Weaver"), ItemType("StormWeaverMusicbox"), TileType("StormWeaverMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/HiveMind"), ItemType("HiveMindMusicbox"), TileType("HiveMindMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/BloodCoagulant"), ItemType("PerforatorMusicbox"), TileType("PerforatorMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Void"), ItemType("CeaselessVoidMusicbox"), TileType("CeaselessVoidMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/DesertScourge"), ItemType("DesertScourgeMusicbox"), TileType("DesertScourgeMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Crabulon"), ItemType("CrabulonMusicbox"), TileType("CrabulonMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/SlimeGod"), ItemType("SlimeGodMusicbox"), TileType("SlimeGodMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/ProvidenceTheme"), ItemType("ProvidenceMusicbox"), TileType("ProvidenceMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Guardians"), ItemType("ProfanedGuardianMusicbox"), TileType("ProfanedGuardianMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Ravager"), ItemType("RavagerMusicbox"), TileType("RavagerMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/AstrumDeus"), ItemType("AstrumDeusMusicbox"), TileType("AstrumDeusMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse"), ItemType("DoGP2Musicbox"), TileType("DoGP2Musicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/TheDeepAbyss"), ItemType("AbyssLowerMusicbox"), TileType("AbyssLowerMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Calamity"), ItemType("CalamityMusicbox"), TileType("CalamityMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/Crag"), ItemType("CragMusicbox"), TileType("CragMusicbox"));
                AddMusicBox(GetSoundSlot(SoundType.Music, "Sounds/Music/LeftAlone"), ItemType("BrimmyMusicbox"), TileType("BrimmyMusicbox"));

                Filters.Scene["CalamityMod:DevourerofGodsHead"] = new Filter(new DoGScreenShaderData("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:DevourerofGodsHead"] = new DoGSky();

                Filters.Scene["CalamityMod:DevourerofGodsHeadS"] = new Filter(new DoGScreenShaderDataS("FilterMiniTower").UseColor(0.4f, 0.1f, 1.0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
                SkyManager.Instance["CalamityMod:DevourerofGodsHeadS"] = new DoGSkyS();

                Filters.Scene["CalamityMod:CalamitasRun3"] = new Filter(new CalScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.6f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:CalamitasRun3"] = new CalSky();
				
				Filters.Scene["CalamityMod:PlaguebringerGoliath"] = new Filter(new PbGScreenShaderData("FilterMiniTower").UseColor(0.2f, 0.6f, 0.2f).UseOpacity(0.65f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:PlaguebringerGoliath"] = new PbGSky();
				
				Filters.Scene["CalamityMod:Yharon"] = new Filter(new YScreenShaderData("FilterMiniTower").UseColor(1f, 0.4f, 0f).UseOpacity(0.75f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:Yharon"] = new YSky();

                Filters.Scene["CalamityMod:Leviathan"] = new Filter(new LevScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0.5f).UseOpacity(0.5f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:Leviathan"] = new LevSky();
				
				Filters.Scene["CalamityMod:Providence"] = new Filter(new ProvScreenShaderData("FilterMiniTower").UseColor(0.6f, 0.45f, 0f).UseOpacity(0.5f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:Providence"] = new ProvSky();
				
				Filters.Scene["CalamityMod:SupremeCalamitas"] = new Filter(new SCalScreenShaderData("FilterMiniTower").UseColor(1.1f, 0.3f, 0.3f).UseOpacity(0.65f), EffectPriority.VeryHigh);
				SkyManager.Instance["CalamityMod:SupremeCalamitas"] = new SCalSky();

                Filters.Scene["CalamityMod:Astral"] = new Filter(new AstralScreenShaderData(new Ref<Effect>(CustomShader), "AstralPass").UseColor(0.18f, 0.08f, 0.24f), EffectPriority.VeryHigh);
                SkyManager.Instance["CalamityMod:Astral"] = new AstralSky();

                Mod mod = ModLoader.GetMod("CalamityMod");
				UIHandler.OnLoad(mod);

                AstralArcanumUI.Load(this);
                base.Load();
            }

            BossHealthBarManager.Load(this);
            base.Load();

            //Injections.Load();
            //base.Load();

            SetupLists();

            #region Text
            ModTranslation text = CreateTranslation("SkyOreText");
            text.SetDefault("The ground is glittering with cyan light.");
            AddTranslation(text);

            text = CreateTranslation("IceOreText");
            text.SetDefault("The ice caves are crackling with frigid energy.");
            AddTranslation(text);

            text = CreateTranslation("PlantOreText");
            text.SetDefault("Energized plant matter has formed in the underground.");
            AddTranslation(text);

            text = CreateTranslation("TreeOreText");
            text.SetDefault("Fossilized tree bark is bursting through the jungle's mud.");
            AddTranslation(text);

            text = CreateTranslation("AuricOreText");
            text.SetDefault("A godly aura has blessed the world's caverns.");
            AddTranslation(text);

            text = CreateTranslation("FutureOreText");
            text.SetDefault("A cold and dark energy has materialized in space.");
            AddTranslation(text);

            text = CreateTranslation("UglyBossText");
			text.SetDefault("Calamitous creatures now roam free!");
			AddTranslation(text);
			
			text = CreateTranslation("UglyBossText2");
			text.SetDefault("The ancient ice spirits have been unbound!");
			AddTranslation(text);
			
			text = CreateTranslation("SteelSkullBossText");
			text.SetDefault("A blood red inferno lingers in the night...");
			AddTranslation(text);
			
			text = CreateTranslation("PlantBossText");
			text.SetDefault("The ocean depths are trembling.");
			AddTranslation(text);
			
			text = CreateTranslation("BabyBossText");
			text.SetDefault("A plague has befallen the Jungle.");
			AddTranslation(text);
			
			text = CreateTranslation("BabyBossText2");
			text.SetDefault("An ancient automaton roams the land.");
			AddTranslation(text);
			
			text = CreateTranslation("MoonBossText");
			text.SetDefault("The profaned flame blazes fiercely!");
			AddTranslation(text);
			
			text = CreateTranslation("MoonBossText2");
			text.SetDefault("Cosmic terrors are watching...");
			AddTranslation(text);
			
			text = CreateTranslation("MoonBossText3");
			text.SetDefault("The bloody moon beckons...");
			AddTranslation(text);
			
			text = CreateTranslation("PlagueBossText");
			text.SetDefault("PLAGUE NUKE BARRAGE ARMED, PREPARING FOR LAUNCH!!!");
			AddTranslation(text);
			
			text = CreateTranslation("PlagueBossText2");
			text.SetDefault("MISSILES LAUNCHED, TARGETING ROUTINE INITIATED!!!");
			AddTranslation(text);
			
			text = CreateTranslation("ProfanedBossText");
			text.SetDefault("The air is burning...");
			AddTranslation(text);
			
			text = CreateTranslation("ProfanedBossText2");
			text.SetDefault("Shrieks are echoing from the dungeon.");
			AddTranslation(text);
			
			text = CreateTranslation("ProfanedBossText3");
			text.SetDefault("The calamitous beings have been inundated with bloodstone.");
			AddTranslation(text);

            text = CreateTranslation("GhostBossText");
            text.SetDefault("The abyssal spirits have been disturbed.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText3"); //start
            text.SetDefault("Alright, let's get started.  Not sure why you're bothering.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText4"); //75%
            text.SetDefault("You seem so confident...yet painfully ignorant of what has yet to transpire.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText5"); //50%
            text.SetDefault("Couldn't leave well enough alone could you?  Everything was going well until you came along.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText6"); //45%
            text.SetDefault("Brothers, could you assist me for a moment?  Dealing with this fool is growing tiresome.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText"); //40%
            text.SetDefault("Don't worry, I still have plenty of tricks left.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText7"); //30%
            text.SetDefault("Hmm...perhaps I should let the little ones out to play for a while.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText8"); //20%
            text.SetDefault("Impressive...but still not good enough!");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText9"); //10%
            text.SetDefault("I'm just getting started!");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText20"); //8%
            text.SetDefault("What!?  How are you still alive!?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText21"); //6%
            text.SetDefault("Just stop!");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText22"); //4%
            text.SetDefault("Even if you defeat me you still have my lord to contend with!");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText23"); //2%
            text.SetDefault("To this day he has yet to lose a battle!");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText24"); //1%
            text.SetDefault("Not even I could defeat him!  What hope do YOU have!?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText25"); //1% after 10 seconds
            text.SetDefault("He has grown far stronger since we fought...I can't see you defeating him, at least not with your current strength.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText26"); //1% after 15 seconds
            text.SetDefault("Well...I guess this is the end...");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText27"); //1% after first win
            text.SetDefault("Perhaps one of these times I'll change my mind after all these deaths...");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText28"); //not taking enough damage
            text.SetDefault("You aren't hurting as much as I'd like...are you cheating?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText2"); //cheater
            text.SetDefault("Go to hell.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText10"); //end after 20 seconds
            text.SetDefault("At long last I am free...no...wait.  I'll just keep coming back, like you.  Funny how that works...isn't it?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText11"); //rebattle after killing once
            text.SetDefault("Do you enjoy going through hell?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText12"); //rebattle after killing four times
            text.SetDefault("Don't get me wrong, I like pain too, but you're just ridiculous.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText13"); //rebattle five deaths
            text.SetDefault("You must enjoy dying more than most people, huh?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText14"); //rebattle twenty deaths
            text.SetDefault("Do you have a fetish for getting killed or something?");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText15"); //rebattle fifty deaths
            text.SetDefault("Alright, I'm done counting.  You probably died this much just to see what I'd say.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText16"); //die 0 times
            text.SetDefault("You didn't die at all huh?  Welp, you probably cheated then.  Do it again, for real this time...but here's your reward I guess.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText17"); //die 1 time and win
            text.SetDefault("One death?  That's it? ...I guess you earned this then.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText18"); //die 2 times and win
            text.SetDefault("Two deaths.  Good job.  Here's your reward.");
            AddTranslation(text);

            text = CreateTranslation("SupremeBossText19"); //die 3 times and win
            text.SetDefault("Third time's the charm huh?  I guess it is.  Here's a special reward.");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText");
			text.SetDefault("Don't get cocky, kid!");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText2");
			text.SetDefault("You think...you can butcher...ME!?");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText3");
			text.SetDefault("A fatal mistake!");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText4");
			text.SetDefault("Good luck recovering from that!");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText5");
			text.SetDefault("Delicious...");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText6");
			text.SetDefault("Did that hurt?");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText7");
			text.SetDefault("Nothing personal, kid.");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText8");
			text.SetDefault("Are you honestly that bad at dodging?");
			AddTranslation(text);
			
			text = CreateTranslation("EdgyBossText9");
			text.SetDefault("Of all my segments to get hit by...");
			AddTranslation(text);

            text = CreateTranslation("EdgyBossText10");
            text.SetDefault("It's not over yet, kid!");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText11");
            text.SetDefault("A GOD DOES NOT FEAR DEATH!");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText12");
            text.SetDefault("You are no god...but I shall feast upon your essence regardless!");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText13");
            text.SetDefault("SO WEAK!");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText14");
            text.SetDefault("YOU FOOL!");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText15");
            text.SetDefault("DOES IT HURT!?");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText16");
            text.SetDefault("WHY DIDN'T YOU DODGE!?");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText17");
            text.SetDefault("NO!  I AM THE ONE ABOVE ALL!  I WILL CONSUME YOUR UNIVERSE!");
            AddTranslation(text);

            text = CreateTranslation("EdgyBossText18");
            text.SetDefault("YOU CAN'T BUTCHER A GOD!");
            AddTranslation(text);

            text = CreateTranslation("DoGBossText");
			text.SetDefault("The frigid moon shimmers brightly.");
			AddTranslation(text);
			
			text = CreateTranslation("DoGBossText2");
			text.SetDefault("The harvest moon glows eerily.");
			AddTranslation(text);
			
			text = CreateTranslation("AstralText");
			text.SetDefault("A star has fallen from the heavens!");
			AddTranslation(text);
			
			text = CreateTranslation("AstralBossText");
			text.SetDefault("The seal of the stars has been broken!  You can now mine Astral Ore!");
			AddTranslation(text);
			
			text = CreateTranslation("CalamitasBossText");
			text.SetDefault("You underestimate my power...");
			AddTranslation(text);
			
			text = CreateTranslation("CalamitasBossText2");
			text.SetDefault("The brothers have awoken!");
			AddTranslation(text);
			
			text = CreateTranslation("CalamitasBossText3");
			text.SetDefault("Impressive child, most impressive...");
			AddTranslation(text);
			
			text = CreateTranslation("CalamitasBossText4");
			text.SetDefault("The brothers have been reborn!");
			AddTranslation(text);
			
			text = CreateTranslation("CryogenBossText");
			text.SetDefault("Cryogen is derping out!");
			AddTranslation(text);
			
			text = CreateTranslation("BloodMoonText");
			text.SetDefault("The Blood Moon is rising...");
			AddTranslation(text);
			
			text = CreateTranslation("DargonBossText");
			text.SetDefault("The dark sun awaits.");
			AddTranslation(text);

            text = CreateTranslation("DargonBossText2");
            text.SetDefault("My dragon deems you an unworthy opponent.  You must acquire the power of the dark sun to witness his true power.");
            AddTranslation(text);

            text = CreateTranslation("DargonBossText3");
            text.SetDefault("My dragon deems you an unworthy opponent.  Snuff out the profaned flame and destroy the god devourer to witness his true power.");
            AddTranslation(text);

            text = CreateTranslation("DeathText");
            text.SetDefault("Death is active, enjoy the fun.");
            AddTranslation(text);

            text = CreateTranslation("DeathText2");
            text.SetDefault("Death is not active, not fun enough for you?");
            AddTranslation(text);

            text = CreateTranslation("ArmageddonText");
            text.SetDefault("Bosses will now kill you instantly.");
            AddTranslation(text);

            text = CreateTranslation("ArmageddonText2");
            text.SetDefault("Bosses will no longer kill you instantly.");
            AddTranslation(text);

            text = CreateTranslation("DefiledText");
            text.SetDefault("Your soul is mine...");
            AddTranslation(text);

            text = CreateTranslation("DefiledText2");
            text.SetDefault("Your soul is yours once more...");
            AddTranslation(text);

            text = CreateTranslation("FlameText");
            text.SetDefault("The air is getting warmer around you.");
            AddTranslation(text);

            text = CreateTranslation("BossRushStartText");
            text.SetDefault("Hmm?  Ah, another contender.  Very well, may the ritual commence!");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierOneEndText");
            text.SetDefault("Hmm?  Oh, you're still alive.  Unexpected and exhilarating, but don't get complacent just yet.");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierTwoEndText");
            text.SetDefault("Hmm?  Persistent aren't you?  Perhaps you have some hope of prosperity, unlike past challengers.");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierThreeEndText");
            text.SetDefault("Hmm?  Your perseverance is truly a trait to behold.  You've come further than even the demigods in such a short time.");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierThreeEndText2");
            text.SetDefault("May your skills remain sharp for the last challenges.");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierFourEndText");
            text.SetDefault("Hmm?  So you've made it to the final tier, a remarkable feat enviable by even the mightiest of the gods.");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierFourEndText2");
            text.SetDefault("Go forth and conquer 'til the ritual's end!");
            AddTranslation(text);

            text = CreateTranslation("BossRushTierFiveEndText");
            text.SetDefault("Hmm?  You expected a reward beyond this mere pebble?  Patience, the true reward will come apparent in time...");
            AddTranslation(text);

            text = CreateTranslation("MeleeLevelUp");
            text.SetDefault("Melee weapon proficiency level up!");
            AddTranslation(text);

            text = CreateTranslation("MeleeLevelUpFinal");
            text.SetDefault("Melee weapon proficiency maxed out!");
            AddTranslation(text);

            text = CreateTranslation("RangedLevelUp");
            text.SetDefault("Ranged weapon proficiency level up!");
            AddTranslation(text);

            text = CreateTranslation("RangedLevelUpFinal");
            text.SetDefault("Ranged weapon proficiency maxed out!");
            AddTranslation(text);

            text = CreateTranslation("MagicLevelUp");
            text.SetDefault("Magic weapon proficiency level up!");
            AddTranslation(text);

            text = CreateTranslation("MagicLevelUpFinal");
            text.SetDefault("Magic weapon proficiency maxed out!");
            AddTranslation(text);

            text = CreateTranslation("SummonLevelUp");
            text.SetDefault("Summoner weapon proficiency level up!");
            AddTranslation(text);

            text = CreateTranslation("SummonLevelUpFinal");
            text.SetDefault("Summoner weapon proficiency maxed out!");
            AddTranslation(text);

            text = CreateTranslation("RogueLevelUp");
            text.SetDefault("Rogue weapon proficiency level up!");
            AddTranslation(text);

            text = CreateTranslation("RogueLevelUpFinal");
            text.SetDefault("Rogue weapon proficiency maxed out!");
            AddTranslation(text);
            #endregion
        }
        #endregion

        #region Unload
        public override void Unload()
        {
            RageHotKey = null;
            AdrenalineHotKey = null;
            AegisHotKey = null;
            TarraHotKey = null;
            AstralTeleportHotKey = null;
            AstralArcanumUIHotkey = null;
            BossBarToggleHotKey = null;
            BossBarToggleSmallTextHotKey = null;

            AstralCactusTexture = null;
            AstralCactusGlowTexture = null;
            AstralSky = null;

            donatorList = null;

            rangedProjectileExceptionList = null;
            projectileMinionList = null;
            enemyImmunityList = null;
            hardModeNerfExceptionList = null;
            dungeonEnemyBuffList = null;
            bossScaleList = null;
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
            lightGreenThrowingRarityList = null;
            greenThrowingRarityList = null;
            blueThrowingRarityList = null;
            purpleThrowingRarityList = null;
            throwingProjectileList = null;
            throwingItemList = null;
            pumpkinMoonBuffList = null;
            frostMoonBuffList = null;
            eclipseBuffList = null;
            eventProjectileBuffList = null;
            revengeanceEnemyBuffList = null;
            revengeanceProjectileBuffList = null;
            trapProjectileList = null;

            BossHealthBarManager.Unload();
            base.Unload();

            AstralArcanumUI.Unload();
            base.Unload();

            Instance = null;

            //Injections.Unload();
            //base.Unload();

            if (!Main.dedServ)
            {
                Main.music[MusicID.Hell] = Main.soundBank.GetCue("Music_" + MusicID.Hell); //remove in 1.4.1

                Main.heartTexture = heartOriginal2;
                Main.heart2Texture = heartOriginal;
                Main.rainTexture = rainOriginal;
                Main.manaTexture = manaOriginal;
                Main.flyingCarpetTexture = carpetOriginal;
            }
        }
        #endregion

        #region SetupLists
        public static void SetupLists()
        {
            donatorList = new List<string>();

            donatorList.Add("Vorbis");
            donatorList.Add("SoloMael");
            donatorList.Add("Chaotic Reks");
            donatorList.Add("The Buildmonger");
            donatorList.Add("Yuh");
            donatorList.Add("Littlepiggy");
            donatorList.Add("LompL");
            donatorList.Add("Lilith Saintclaire");
            donatorList.Add("Ben Shapiro");
            donatorList.Add("Frederik Henschel");
            donatorList.Add("Faye");
            donatorList.Add("Gibb50");
            donatorList.Add("Braden Hajer");
            donatorList.Add("Hannes Holmlund");
            donatorList.Add("profoundmango69");
            donatorList.Add("Jack M Sargent");
            donatorList.Add("Hans Volter");
            donatorList.Add("Krankwagon");
            donatorList.Add("MishiroUsui");
            donatorList.Add("pixlgray");
            donatorList.Add("Arkhine");
            donatorList.Add("Lodude");

            rangedProjectileExceptionList = new List<int>();

            rangedProjectileExceptionList.Add(ProjectileID.Phantasm);
            rangedProjectileExceptionList.Add(ProjectileID.VortexBeater);
            rangedProjectileExceptionList.Add(ProjectileID.DD2PhoenixBow);

            projectileMinionList = new List<int>();

            projectileMinionList.Add(ProjectileID.PygmySpear);
            projectileMinionList.Add(ProjectileID.UFOMinion);
            projectileMinionList.Add(ProjectileID.UFOLaser);
            projectileMinionList.Add(ProjectileID.StardustCellMinionShot);
            projectileMinionList.Add(ProjectileID.MiniSharkron);
            projectileMinionList.Add(ProjectileID.MiniRetinaLaser);
            projectileMinionList.Add(ProjectileID.ImpFireball);
            projectileMinionList.Add(ProjectileID.HornetStinger);
            projectileMinionList.Add(ProjectileID.DD2FlameBurstTowerT1Shot);
            projectileMinionList.Add(ProjectileID.DD2FlameBurstTowerT2Shot);
            projectileMinionList.Add(ProjectileID.DD2FlameBurstTowerT3Shot);
            projectileMinionList.Add(ProjectileID.DD2BallistraProj);
            projectileMinionList.Add(ProjectileID.DD2ExplosiveTrapT1Explosion);
            projectileMinionList.Add(ProjectileID.DD2ExplosiveTrapT2Explosion);
            projectileMinionList.Add(ProjectileID.DD2ExplosiveTrapT3Explosion);
            projectileMinionList.Add(ProjectileID.SpiderEgg);
            projectileMinionList.Add(ProjectileID.BabySpider);
            projectileMinionList.Add(ProjectileID.FrostBlastFriendly);
            projectileMinionList.Add(ProjectileID.MoonlordTurretLaser);
            projectileMinionList.Add(ProjectileID.RainbowCrystalExplosion);

            enemyImmunityList = new List<int>();

            enemyImmunityList.Add(NPCID.QueenBee);
            enemyImmunityList.Add(NPCID.WallofFlesh);
            enemyImmunityList.Add(NPCID.WallofFleshEye);
            enemyImmunityList.Add(NPCID.Retinazer);
            enemyImmunityList.Add(NPCID.Spazmatism);
            enemyImmunityList.Add(NPCID.SkeletronPrime);
            enemyImmunityList.Add(NPCID.PrimeCannon);
            enemyImmunityList.Add(NPCID.PrimeSaw);
            enemyImmunityList.Add(NPCID.PrimeLaser);
            enemyImmunityList.Add(NPCID.PrimeVice);
            enemyImmunityList.Add(NPCID.Plantera);
            enemyImmunityList.Add(NPCID.IceQueen);
            enemyImmunityList.Add(NPCID.Pumpking);
            enemyImmunityList.Add(NPCID.Mothron);
            enemyImmunityList.Add(NPCID.Golem);
            enemyImmunityList.Add(NPCID.GolemHead);
            enemyImmunityList.Add(NPCID.GolemFistRight);
            enemyImmunityList.Add(NPCID.GolemFistLeft);
            enemyImmunityList.Add(NPCID.DukeFishron);
            enemyImmunityList.Add(NPCID.CultistBoss);
            enemyImmunityList.Add(NPCID.MoonLordHead);
            enemyImmunityList.Add(NPCID.MoonLordHand);
            enemyImmunityList.Add(NPCID.MoonLordCore);
            enemyImmunityList.Add(NPCID.MoonLordFreeEye);

            hardModeNerfExceptionList = new List<int>();

            hardModeNerfExceptionList.Add(NPCID.Probe);
            hardModeNerfExceptionList.Add(NPCID.TheHungry);
            hardModeNerfExceptionList.Add(NPCID.TheHungryII);
            hardModeNerfExceptionList.Add(NPCID.WallofFleshEye);
            hardModeNerfExceptionList.Add(NPCID.Creeper);
            hardModeNerfExceptionList.Add(NPCID.EaterofWorldsHead);
            hardModeNerfExceptionList.Add(NPCID.EaterofWorldsBody);
            hardModeNerfExceptionList.Add(NPCID.EaterofWorldsTail);
            hardModeNerfExceptionList.Add(NPCID.SkeletronHand);

            dungeonEnemyBuffList = new List<int>();

            dungeonEnemyBuffList.Add(NPCID.SkeletonSniper);
            dungeonEnemyBuffList.Add(NPCID.TacticalSkeleton);
            dungeonEnemyBuffList.Add(NPCID.SkeletonCommando);
            dungeonEnemyBuffList.Add(NPCID.Paladin);
            dungeonEnemyBuffList.Add(NPCID.GiantCursedSkull);
            dungeonEnemyBuffList.Add(NPCID.BoneLee);
            dungeonEnemyBuffList.Add(NPCID.DiabolistWhite);
            dungeonEnemyBuffList.Add(NPCID.DiabolistRed);
            dungeonEnemyBuffList.Add(NPCID.NecromancerArmored);
            dungeonEnemyBuffList.Add(NPCID.Necromancer);
            dungeonEnemyBuffList.Add(NPCID.RaggedCasterOpenCoat);
            dungeonEnemyBuffList.Add(NPCID.RaggedCaster);
            dungeonEnemyBuffList.Add(NPCID.HellArmoredBonesSword);
            dungeonEnemyBuffList.Add(NPCID.HellArmoredBonesMace);
            dungeonEnemyBuffList.Add(NPCID.HellArmoredBonesSpikeShield);
            dungeonEnemyBuffList.Add(NPCID.HellArmoredBones);
            dungeonEnemyBuffList.Add(NPCID.BlueArmoredBonesSword);
            dungeonEnemyBuffList.Add(NPCID.BlueArmoredBonesNoPants);
            dungeonEnemyBuffList.Add(NPCID.BlueArmoredBonesMace);
            dungeonEnemyBuffList.Add(NPCID.BlueArmoredBones);
            dungeonEnemyBuffList.Add(NPCID.RustyArmoredBonesSwordNoArmor);
            dungeonEnemyBuffList.Add(NPCID.RustyArmoredBonesSword);
            dungeonEnemyBuffList.Add(NPCID.RustyArmoredBonesFlail);
            dungeonEnemyBuffList.Add(NPCID.RustyArmoredBonesAxe);

            bossScaleList = new List<int>();

            bossScaleList.Add(NPCID.EaterofWorldsHead);
            bossScaleList.Add(NPCID.EaterofWorldsBody);
            bossScaleList.Add(NPCID.EaterofWorldsTail);
            bossScaleList.Add(NPCID.Creeper);
            bossScaleList.Add(NPCID.SkeletronHand);
            bossScaleList.Add(NPCID.WallofFleshEye);
            bossScaleList.Add(NPCID.TheHungry);
            bossScaleList.Add(NPCID.TheHungryII);
            bossScaleList.Add(NPCID.TheDestroyerBody);
            bossScaleList.Add(NPCID.TheDestroyerTail);
            bossScaleList.Add(NPCID.PrimeCannon);
            bossScaleList.Add(NPCID.PrimeVice);
            bossScaleList.Add(NPCID.PrimeSaw);
            bossScaleList.Add(NPCID.PrimeLaser);
            bossScaleList.Add(NPCID.PlanterasTentacle);
            bossScaleList.Add(NPCID.Pumpking);
            bossScaleList.Add(NPCID.IceQueen);
            bossScaleList.Add(NPCID.Mothron);
            bossScaleList.Add(NPCID.GolemHead);

            beeEnemyList = new List<int>();

            beeEnemyList.Add(NPCID.GiantMossHornet);
            beeEnemyList.Add(NPCID.BigMossHornet);
            beeEnemyList.Add(NPCID.LittleMossHornet);
            beeEnemyList.Add(NPCID.TinyMossHornet);
            beeEnemyList.Add(NPCID.MossHornet);
            beeEnemyList.Add(NPCID.VortexHornetQueen);
            beeEnemyList.Add(NPCID.VortexHornet);
            beeEnemyList.Add(NPCID.Bee);
            beeEnemyList.Add(NPCID.BeeSmall);
            beeEnemyList.Add(NPCID.QueenBee);

            beeProjectileList = new List<int>();

            beeProjectileList.Add(ProjectileID.Stinger);
            beeProjectileList.Add(ProjectileID.HornetStinger);

            hardModeNerfList = new List<int>();

            hardModeNerfList.Add(ProjectileID.WebSpit);
            hardModeNerfList.Add(ProjectileID.PinkLaser);
            hardModeNerfList.Add(ProjectileID.FrostBlastHostile);
            hardModeNerfList.Add(ProjectileID.RuneBlast);
            hardModeNerfList.Add(ProjectileID.GoldenShowerHostile);
            hardModeNerfList.Add(ProjectileID.RainNimbus);
            hardModeNerfList.Add(ProjectileID.Stinger);
            hardModeNerfList.Add(ProjectileID.FlamingArrow);
            hardModeNerfList.Add(ProjectileID.BulletDeadeye);
            hardModeNerfList.Add(ProjectileID.CannonballHostile);

            debuffList = new List<int>();

            debuffList.Add(BuffID.Poisoned);
            debuffList.Add(BuffID.Darkness);
            debuffList.Add(BuffID.Cursed);
            debuffList.Add(BuffID.OnFire);
            debuffList.Add(BuffID.Bleeding);
            debuffList.Add(BuffID.Confused);
            debuffList.Add(BuffID.Slow);
            debuffList.Add(BuffID.Weak);
            debuffList.Add(BuffID.Silenced);
            debuffList.Add(BuffID.BrokenArmor);
            debuffList.Add(BuffID.CursedInferno);
            debuffList.Add(BuffID.Frostburn);
            debuffList.Add(BuffID.Chilled);
            debuffList.Add(BuffID.Frozen);
            debuffList.Add(BuffID.Burning);
            debuffList.Add(BuffID.Suffocation);
            debuffList.Add(BuffID.Ichor);
            debuffList.Add(BuffID.Venom);
            debuffList.Add(BuffID.Blackout);
            debuffList.Add(BuffID.Electrified);
            debuffList.Add(BuffID.Rabies);
            debuffList.Add(BuffID.Webbed);
            debuffList.Add(BuffID.Stoned);
            debuffList.Add(BuffID.Dazed);
            debuffList.Add(BuffID.VortexDebuff);
            debuffList.Add(BuffID.WitheredArmor);
            debuffList.Add(BuffID.WitheredWeapon);
            debuffList.Add(BuffID.OgreSpit);
            debuffList.Add(BuffID.BetsysCurse);

            fireWeaponList = new List<int>();

            fireWeaponList.Add(ItemID.FieryGreatsword);
            fireWeaponList.Add(ItemID.DD2SquireDemonSword);
            fireWeaponList.Add(ItemID.TheHorsemansBlade);
            fireWeaponList.Add(ItemID.DD2SquireBetsySword);
            fireWeaponList.Add(ItemID.Cascade);
            fireWeaponList.Add(ItemID.HelFire);
            fireWeaponList.Add(ItemID.MonkStaffT2);
            fireWeaponList.Add(ItemID.Flamarang);
            fireWeaponList.Add(ItemID.MoltenFury);
            fireWeaponList.Add(ItemID.Sunfury);
            fireWeaponList.Add(ItemID.PhoenixBlaster);
            fireWeaponList.Add(ItemID.Flamelash);
            fireWeaponList.Add(ItemID.SolarEruption);
            fireWeaponList.Add(ItemID.DayBreak);
            fireWeaponList.Add(ItemID.MonkStaffT3);
            fireWeaponList.Add(ItemID.HellwingBow);
            fireWeaponList.Add(ItemID.DD2PhoenixBow);
            fireWeaponList.Add(ItemID.DD2BetsyBow);
            fireWeaponList.Add(ItemID.FlareGun);
            fireWeaponList.Add(ItemID.Flamethrower);
            fireWeaponList.Add(ItemID.EldMelter);
            fireWeaponList.Add(ItemID.FlowerofFire);
            fireWeaponList.Add(ItemID.MeteorStaff);
            fireWeaponList.Add(ItemID.ApprenticeStaffT3);
            fireWeaponList.Add(ItemID.InfernoFork);
            fireWeaponList.Add(ItemID.HeatRay);
            fireWeaponList.Add(ItemID.BookofSkulls);
            fireWeaponList.Add(ItemID.ImpStaff);
            fireWeaponList.Add(ItemID.DD2FlameburstTowerT1Popper);
            fireWeaponList.Add(ItemID.DD2FlameburstTowerT2Popper);
            fireWeaponList.Add(ItemID.DD2FlameburstTowerT3Popper);
            fireWeaponList.Add(ItemID.MolotovCocktail);

            natureWeaponList = new List<int>();

            natureWeaponList.Add(ItemID.BladeofGrass);
            natureWeaponList.Add(ItemID.ChlorophyteClaymore);
            natureWeaponList.Add(ItemID.ChlorophyteSaber);
            natureWeaponList.Add(ItemID.ChlorophytePartisan);
            natureWeaponList.Add(ItemID.ChlorophyteShotbow);
            natureWeaponList.Add(ItemID.Seedler);
            natureWeaponList.Add(ItemID.ChristmasTreeSword);
            natureWeaponList.Add(ItemID.TerraBlade);
            natureWeaponList.Add(ItemID.JungleYoyo);
            natureWeaponList.Add(ItemID.Yelets);
            natureWeaponList.Add(ItemID.MushroomSpear);
            natureWeaponList.Add(ItemID.ThornChakram);
            natureWeaponList.Add(ItemID.Bananarang);
            natureWeaponList.Add(ItemID.FlowerPow);
            natureWeaponList.Add(ItemID.BeesKnees);
            natureWeaponList.Add(ItemID.Toxikarp);
            natureWeaponList.Add(ItemID.Bladetongue);
            natureWeaponList.Add(ItemID.PoisonStaff);
            natureWeaponList.Add(ItemID.VenomStaff);
            natureWeaponList.Add(ItemID.StaffofEarth);
            natureWeaponList.Add(ItemID.BeeGun);
            natureWeaponList.Add(ItemID.LeafBlower);
            natureWeaponList.Add(ItemID.WaspGun);
            natureWeaponList.Add(ItemID.CrystalSerpent);
            natureWeaponList.Add(ItemID.Razorpine);
            natureWeaponList.Add(ItemID.HornetStaff);
            natureWeaponList.Add(ItemID.QueenSpiderStaff);
            natureWeaponList.Add(ItemID.SlimeStaff);
            natureWeaponList.Add(ItemID.PygmyStaff);
            natureWeaponList.Add(ItemID.RavenStaff);
            natureWeaponList.Add(ItemID.BatScepter);
            natureWeaponList.Add(ItemID.SpiderStaff);
            natureWeaponList.Add(ItemID.Beenade);
            natureWeaponList.Add(ItemID.FrostDaggerfish);

            alcoholList = new List<int>();

            doubleDamageBuffList = new List<int>();

            doubleDamageBuffList.Add(ItemID.BallOHurt);
            doubleDamageBuffList.Add(ItemID.TheMeatball);
            doubleDamageBuffList.Add(ItemID.BlueMoon);
            doubleDamageBuffList.Add(ItemID.Sunfury);
            doubleDamageBuffList.Add(ItemID.DaoofPow);
            doubleDamageBuffList.Add(ItemID.FlowerPow);
            doubleDamageBuffList.Add(ItemID.Anchor);
            doubleDamageBuffList.Add(ItemID.KOCannon);
            doubleDamageBuffList.Add(ItemID.GolemFist);
            doubleDamageBuffList.Add(ItemID.BreakerBlade);
            doubleDamageBuffList.Add(ItemID.SpectreStaff);
            doubleDamageBuffList.Add(ItemID.MonkStaffT2);
            doubleDamageBuffList.Add(ItemID.ProximityMineLauncher);
            doubleDamageBuffList.Add(ItemID.FireworksLauncher);

            sixtySixDamageBuffList = new List<int>();

            sixtySixDamageBuffList.Add(ItemID.TerraBlade);
            sixtySixDamageBuffList.Add(ItemID.TrueNightsEdge);
            sixtySixDamageBuffList.Add(ItemID.WandofSparking);
            sixtySixDamageBuffList.Add(ItemID.MedusaHead);
            sixtySixDamageBuffList.Add(ItemID.StaffofEarth);
            sixtySixDamageBuffList.Add(ItemID.ChristmasTreeSword);
            sixtySixDamageBuffList.Add(ItemID.MonkStaffT1);
            sixtySixDamageBuffList.Add(ItemID.InfernoFork);
            sixtySixDamageBuffList.Add(ItemID.VenomStaff);

            fiftyDamageBuffList = new List<int>();

            fiftyDamageBuffList.Add(ItemID.NightsEdge);
            fiftyDamageBuffList.Add(ItemID.EldMelter);
            fiftyDamageBuffList.Add(ItemID.Flamethrower);
            fiftyDamageBuffList.Add(ItemID.MoonlordTurretStaff);
            fiftyDamageBuffList.Add(ItemID.WaspGun);
            fiftyDamageBuffList.Add(ItemID.Keybrand);
            fiftyDamageBuffList.Add(ItemID.PulseBow);
            fiftyDamageBuffList.Add(ItemID.PaladinsHammer);

            thirtyThreeDamageBuffList = new List<int>();

            thirtyThreeDamageBuffList.Add(ItemID.CrystalVileShard);
            thirtyThreeDamageBuffList.Add(ItemID.SoulDrain);
            thirtyThreeDamageBuffList.Add(ItemID.ClingerStaff);
            thirtyThreeDamageBuffList.Add(ItemID.ChargedBlasterCannon);
            thirtyThreeDamageBuffList.Add(ItemID.NettleBurst);
            thirtyThreeDamageBuffList.Add(ItemID.Excalibur);
            thirtyThreeDamageBuffList.Add(ItemID.AmberStaff);
            thirtyThreeDamageBuffList.Add(ItemID.BluePhasesaber);
            thirtyThreeDamageBuffList.Add(ItemID.RedPhasesaber);
            thirtyThreeDamageBuffList.Add(ItemID.GreenPhasesaber);
            thirtyThreeDamageBuffList.Add(ItemID.WhitePhasesaber);
            thirtyThreeDamageBuffList.Add(ItemID.YellowPhasesaber);
            thirtyThreeDamageBuffList.Add(ItemID.PurplePhasesaber);
            thirtyThreeDamageBuffList.Add(ItemID.TheRottedFork);
            thirtyThreeDamageBuffList.Add(ItemID.VampireKnives);

            twentyFiveDamageBuffList = new List<int>();

            twentyFiveDamageBuffList.Add(ItemID.Muramasa);
            twentyFiveDamageBuffList.Add(ItemID.StakeLauncher);

            twentyDamageBuffList = new List<int>();

            twentyDamageBuffList.Add(ItemID.ChainGuillotines);
            twentyDamageBuffList.Add(ItemID.FlowerofFrost);
            twentyDamageBuffList.Add(ItemID.PoisonStaff);
            twentyDamageBuffList.Add(ItemID.Gungnir);
            twentyDamageBuffList.Add(ItemID.BookStaff);

            weaponAutoreuseList = new List<int>();

            weaponAutoreuseList.Add(ItemID.NightsEdge);
            weaponAutoreuseList.Add(ItemID.TrueNightsEdge);
            weaponAutoreuseList.Add(ItemID.TrueExcalibur);
            weaponAutoreuseList.Add(ItemID.PhoenixBlaster);
            weaponAutoreuseList.Add(ItemID.VenusMagnum);
            weaponAutoreuseList.Add(ItemID.MagicDagger);
            weaponAutoreuseList.Add(ItemID.BeamSword);
            weaponAutoreuseList.Add(ItemID.MonkStaffT2);
            weaponAutoreuseList.Add(ItemID.PaladinsHammer);

            quarterDamageNerfList = new List<int>();

            quarterDamageNerfList.Add(ItemID.DaedalusStormbow);
            quarterDamageNerfList.Add(ItemID.PhoenixBlaster);
            quarterDamageNerfList.Add(ItemID.VenusMagnum);
            quarterDamageNerfList.Add(ItemID.BlizzardStaff);

            pumpkinMoonBuffList = new List<int>();

            pumpkinMoonBuffList.Add(NPCID.Scarecrow1);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow2);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow3);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow4);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow5);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow6);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow7);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow8);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow9);
            pumpkinMoonBuffList.Add(NPCID.Scarecrow10);
            pumpkinMoonBuffList.Add(NPCID.HeadlessHorseman);
            pumpkinMoonBuffList.Add(NPCID.MourningWood);
            pumpkinMoonBuffList.Add(NPCID.Splinterling);
            pumpkinMoonBuffList.Add(NPCID.Pumpking);
            pumpkinMoonBuffList.Add(NPCID.Hellhound);
            pumpkinMoonBuffList.Add(NPCID.Poltergeist);

            frostMoonBuffList = new List<int>();

            frostMoonBuffList.Add(NPCID.ZombieElf);
            frostMoonBuffList.Add(NPCID.ZombieElfBeard);
            frostMoonBuffList.Add(NPCID.ZombieElfGirl);
            frostMoonBuffList.Add(NPCID.PresentMimic);
            frostMoonBuffList.Add(NPCID.GingerbreadMan);
            frostMoonBuffList.Add(NPCID.Yeti);
            frostMoonBuffList.Add(NPCID.Everscream);
            frostMoonBuffList.Add(NPCID.IceQueen);
            frostMoonBuffList.Add(NPCID.SantaNK1);
            frostMoonBuffList.Add(NPCID.ElfCopter);
            frostMoonBuffList.Add(NPCID.Nutcracker);
            frostMoonBuffList.Add(NPCID.NutcrackerSpinning);
            frostMoonBuffList.Add(NPCID.ElfArcher);
            frostMoonBuffList.Add(NPCID.Krampus);
            frostMoonBuffList.Add(NPCID.Flocko);

            eclipseBuffList = new List<int>();

            eclipseBuffList.Add(NPCID.Eyezor);
            eclipseBuffList.Add(NPCID.Reaper);
            eclipseBuffList.Add(NPCID.Frankenstein);
            eclipseBuffList.Add(NPCID.SwampThing);
            eclipseBuffList.Add(NPCID.Vampire);
            eclipseBuffList.Add(NPCID.VampireBat);
            eclipseBuffList.Add(NPCID.Butcher);
            eclipseBuffList.Add(NPCID.CreatureFromTheDeep);
            eclipseBuffList.Add(NPCID.Fritz);
            eclipseBuffList.Add(NPCID.Nailhead);
            eclipseBuffList.Add(NPCID.Psycho);
            eclipseBuffList.Add(NPCID.DeadlySphere);
            eclipseBuffList.Add(NPCID.DrManFly);
            eclipseBuffList.Add(NPCID.ThePossessed);
            eclipseBuffList.Add(NPCID.Mothron);
            eclipseBuffList.Add(NPCID.MothronEgg);
            eclipseBuffList.Add(NPCID.MothronSpawn);

            eventProjectileBuffList = new List<int>();

            eventProjectileBuffList.Add(ProjectileID.FlamingWood);
            eventProjectileBuffList.Add(ProjectileID.GreekFire1);
            eventProjectileBuffList.Add(ProjectileID.GreekFire2);
            eventProjectileBuffList.Add(ProjectileID.GreekFire3);
            eventProjectileBuffList.Add(ProjectileID.FlamingScythe);
            eventProjectileBuffList.Add(ProjectileID.FlamingArrow);
            eventProjectileBuffList.Add(ProjectileID.PineNeedleHostile);
            eventProjectileBuffList.Add(ProjectileID.OrnamentHostile);
            eventProjectileBuffList.Add(ProjectileID.OrnamentHostileShrapnel);
            eventProjectileBuffList.Add(ProjectileID.FrostWave);
            eventProjectileBuffList.Add(ProjectileID.FrostShard);
            eventProjectileBuffList.Add(ProjectileID.Missile);
            eventProjectileBuffList.Add(ProjectileID.Present);
            eventProjectileBuffList.Add(ProjectileID.Spike);
            eventProjectileBuffList.Add(ProjectileID.BulletDeadeye);
            eventProjectileBuffList.Add(ProjectileID.EyeLaser);
            eventProjectileBuffList.Add(ProjectileID.Nail);
            eventProjectileBuffList.Add(ProjectileID.DrManFlyFlask);

            revengeanceEnemyBuffList = new List<int>();

            revengeanceEnemyBuffList.Add(NPCID.ServantofCthulhu);
            revengeanceEnemyBuffList.Add(NPCID.EyeofCthulhu);
            revengeanceEnemyBuffList.Add(NPCID.EaterofWorldsHead);
            revengeanceEnemyBuffList.Add(NPCID.DevourerHead);
            revengeanceEnemyBuffList.Add(NPCID.GiantWormHead);
            revengeanceEnemyBuffList.Add(NPCID.MeteorHead);
            revengeanceEnemyBuffList.Add(NPCID.SkeletronHead);
            revengeanceEnemyBuffList.Add(NPCID.SkeletronHand);
            revengeanceEnemyBuffList.Add(NPCID.BoneSerpentHead);
            revengeanceEnemyBuffList.Add(NPCID.ManEater);
            revengeanceEnemyBuffList.Add(NPCID.KingSlime);
            revengeanceEnemyBuffList.Add(NPCID.Snatcher);
            revengeanceEnemyBuffList.Add(NPCID.Piranha);
            revengeanceEnemyBuffList.Add(NPCID.Shark);
            revengeanceEnemyBuffList.Add(NPCID.SpikeBall);
            revengeanceEnemyBuffList.Add(NPCID.BlazingWheel);
            revengeanceEnemyBuffList.Add(NPCID.Mimic);
            revengeanceEnemyBuffList.Add(NPCID.WyvernHead);
            revengeanceEnemyBuffList.Add(NPCID.DiggerHead);
            revengeanceEnemyBuffList.Add(NPCID.SeekerHead);
            revengeanceEnemyBuffList.Add(NPCID.AnglerFish);
            revengeanceEnemyBuffList.Add(NPCID.Werewolf);
            revengeanceEnemyBuffList.Add(NPCID.Wraith);
            revengeanceEnemyBuffList.Add(NPCID.WallofFlesh);
            revengeanceEnemyBuffList.Add(NPCID.TheHungry);
            revengeanceEnemyBuffList.Add(NPCID.TheHungryII);
            revengeanceEnemyBuffList.Add(NPCID.LeechHead);
            revengeanceEnemyBuffList.Add(NPCID.Spazmatism);
            revengeanceEnemyBuffList.Add(NPCID.Retinazer);
            revengeanceEnemyBuffList.Add(NPCID.SkeletronPrime);
            revengeanceEnemyBuffList.Add(NPCID.PrimeSaw);
            revengeanceEnemyBuffList.Add(NPCID.PrimeVice);
            revengeanceEnemyBuffList.Add(NPCID.TheDestroyer);
            revengeanceEnemyBuffList.Add(NPCID.Arapaima);
            revengeanceEnemyBuffList.Add(NPCID.BlackRecluse);
            revengeanceEnemyBuffList.Add(NPCID.WallCreeper);
            revengeanceEnemyBuffList.Add(NPCID.WallCreeperWall);
            revengeanceEnemyBuffList.Add(NPCID.BlackRecluseWall);
            revengeanceEnemyBuffList.Add(NPCID.AngryTrapper);
            revengeanceEnemyBuffList.Add(NPCID.Lihzahrd);
            revengeanceEnemyBuffList.Add(NPCID.LihzahrdCrawler);
            revengeanceEnemyBuffList.Add(NPCID.PirateCaptain);
            revengeanceEnemyBuffList.Add(NPCID.QueenBee);
            revengeanceEnemyBuffList.Add(NPCID.FlyingSnake);
            revengeanceEnemyBuffList.Add(NPCID.Golem);
            revengeanceEnemyBuffList.Add(NPCID.GolemFistLeft);
            revengeanceEnemyBuffList.Add(NPCID.GolemFistRight);
            revengeanceEnemyBuffList.Add(NPCID.Reaper);
            revengeanceEnemyBuffList.Add(NPCID.Plantera);
            revengeanceEnemyBuffList.Add(NPCID.PlanterasHook);
            revengeanceEnemyBuffList.Add(NPCID.PlanterasTentacle);
            revengeanceEnemyBuffList.Add(NPCID.BrainofCthulhu);
            revengeanceEnemyBuffList.Add(NPCID.Paladin);
            revengeanceEnemyBuffList.Add(NPCID.BoneLee);
            revengeanceEnemyBuffList.Add(NPCID.MourningWood);
            revengeanceEnemyBuffList.Add(NPCID.Pumpking);
            revengeanceEnemyBuffList.Add(NPCID.PumpkingBlade);
            revengeanceEnemyBuffList.Add(NPCID.PresentMimic);
            revengeanceEnemyBuffList.Add(NPCID.Everscream);
            revengeanceEnemyBuffList.Add(NPCID.IceQueen);
            revengeanceEnemyBuffList.Add(NPCID.SantaNK1);
            revengeanceEnemyBuffList.Add(NPCID.DukeFishron);
            revengeanceEnemyBuffList.Add(NPCID.MoonLordHand);
            revengeanceEnemyBuffList.Add(NPCID.StardustWormHead);
            revengeanceEnemyBuffList.Add(NPCID.SolarCrawltipedeHead);
            revengeanceEnemyBuffList.Add(NPCID.CultistDragonHead);
            revengeanceEnemyBuffList.Add(NPCID.Butcher);
            revengeanceEnemyBuffList.Add(NPCID.Psycho);
            revengeanceEnemyBuffList.Add(NPCID.DeadlySphere);
            revengeanceEnemyBuffList.Add(NPCID.BigMimicCorruption);
            revengeanceEnemyBuffList.Add(NPCID.BigMimicCrimson);
            revengeanceEnemyBuffList.Add(NPCID.BigMimicHallow);
            revengeanceEnemyBuffList.Add(NPCID.Mothron);
            revengeanceEnemyBuffList.Add(NPCID.DuneSplicerHead);
            revengeanceEnemyBuffList.Add(NPCID.SlimeSpiked);
            revengeanceEnemyBuffList.Add(NPCID.SandShark);
            revengeanceEnemyBuffList.Add(NPCID.SandsharkCorrupt);
            revengeanceEnemyBuffList.Add(NPCID.SandsharkCrimson);
            revengeanceEnemyBuffList.Add(NPCID.SandsharkHallow);
            revengeanceEnemyBuffList.Add(NPCID.DD2Betsy);

            revengeanceProjectileBuffList = new List<int>();

            revengeanceProjectileBuffList.Add(ProjectileID.SandBallFalling);
            revengeanceProjectileBuffList.Add(ProjectileID.AshBallFalling);
            revengeanceProjectileBuffList.Add(ProjectileID.DemonSickle);
            revengeanceProjectileBuffList.Add(ProjectileID.EbonsandBallFalling);
            revengeanceProjectileBuffList.Add(ProjectileID.PearlSandBallFalling);
            revengeanceProjectileBuffList.Add(ProjectileID.CursedFlameHostile);
            revengeanceProjectileBuffList.Add(ProjectileID.EyeFire);
            revengeanceProjectileBuffList.Add(ProjectileID.Boulder);
            revengeanceProjectileBuffList.Add(ProjectileID.DeathLaser);
            revengeanceProjectileBuffList.Add(ProjectileID.PoisonDartTrap);
            revengeanceProjectileBuffList.Add(ProjectileID.SpikyBallTrap);
            revengeanceProjectileBuffList.Add(ProjectileID.SpearTrap);
            revengeanceProjectileBuffList.Add(ProjectileID.FlamethrowerTrap);
            revengeanceProjectileBuffList.Add(ProjectileID.FlamesTrap);
            revengeanceProjectileBuffList.Add(ProjectileID.CrimsandBallFalling);
            revengeanceProjectileBuffList.Add(ProjectileID.Fireball);
            revengeanceProjectileBuffList.Add(ProjectileID.EyeBeam);
            revengeanceProjectileBuffList.Add(ProjectileID.PoisonSeedPlantera);
            revengeanceProjectileBuffList.Add(ProjectileID.ThornBall);
            revengeanceProjectileBuffList.Add(ProjectileID.PaladinsHammerHostile);
            revengeanceProjectileBuffList.Add(ProjectileID.RocketSkeleton);
            revengeanceProjectileBuffList.Add(ProjectileID.FlamingWood);
            revengeanceProjectileBuffList.Add(ProjectileID.FlamingScythe);
            revengeanceProjectileBuffList.Add(ProjectileID.FrostWave);
            revengeanceProjectileBuffList.Add(ProjectileID.Present);
            revengeanceProjectileBuffList.Add(ProjectileID.Spike);
            revengeanceProjectileBuffList.Add(ProjectileID.SaucerDeathray);
            revengeanceProjectileBuffList.Add(ProjectileID.PhantasmalEye);
            revengeanceProjectileBuffList.Add(ProjectileID.PhantasmalSphere);
            revengeanceProjectileBuffList.Add(ProjectileID.PhantasmalDeathray);
            revengeanceProjectileBuffList.Add(ProjectileID.CultistBossIceMist);
            revengeanceProjectileBuffList.Add(ProjectileID.CultistBossLightningOrbArc);
            revengeanceProjectileBuffList.Add(ProjectileID.CultistBossFireBall);
            revengeanceProjectileBuffList.Add(ProjectileID.NebulaBolt);
            revengeanceProjectileBuffList.Add(ProjectileID.NebulaSphere);
            revengeanceProjectileBuffList.Add(ProjectileID.NebulaLaser);
            revengeanceProjectileBuffList.Add(ProjectileID.StardustSoldierLaser);
            revengeanceProjectileBuffList.Add(ProjectileID.VortexLaser);
            revengeanceProjectileBuffList.Add(ProjectileID.VortexVortexLightning);
            revengeanceProjectileBuffList.Add(ProjectileID.VortexLightning);
            revengeanceProjectileBuffList.Add(ProjectileID.VortexAcid);
            revengeanceProjectileBuffList.Add(ProjectileID.GeyserTrap);
            revengeanceProjectileBuffList.Add(ProjectileID.SandnadoHostile);
            revengeanceProjectileBuffList.Add(ProjectileID.DD2BetsyFireball);
            revengeanceProjectileBuffList.Add(ProjectileID.DD2BetsyFlameBreath);

            trapProjectileList = new List<int>();

            trapProjectileList.Add(ProjectileID.PoisonDartTrap);
            trapProjectileList.Add(ProjectileID.SpikyBallTrap);
            trapProjectileList.Add(ProjectileID.SpearTrap);
            trapProjectileList.Add(ProjectileID.FlamethrowerTrap);
            trapProjectileList.Add(ProjectileID.FlamesTrap);
            trapProjectileList.Add(ProjectileID.PoisonDart);
            trapProjectileList.Add(ProjectileID.GeyserTrap);

            lightGreenThrowingRarityList = new List<int>();
            greenThrowingRarityList = new List<int>();
            blueThrowingRarityList = new List<int>();
            purpleThrowingRarityList = new List<int>();

            throwingProjectileList = new List<int>();
            throwingItemList = new List<int>();

            Mod calamity = ModLoader.GetMod("CalamityMod");
            if (calamity != null)
            {
                rangedProjectileExceptionList.Add(calamity.ProjectileType("Phangasm"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("Contagion"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("DaemonsFlame"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("ExoTornado"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("Drataliornus"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("FlakKrakenGun"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("Butcher"));
                rangedProjectileExceptionList.Add(calamity.ProjectileType("StarfleetMK2"));

                beeEnemyList.Add(calamity.NPCType("PlaguebringerGoliath"));
                beeEnemyList.Add(calamity.NPCType("PlaguebringerShade"));
                beeEnemyList.Add(calamity.NPCType("PlagueBeeLargeG"));
                beeEnemyList.Add(calamity.NPCType("PlagueBeeLarge"));
                beeEnemyList.Add(calamity.NPCType("PlagueBeeG"));
                beeEnemyList.Add(calamity.NPCType("PlagueBee"));

                beeProjectileList.Add(calamity.ProjectileType("PlagueStingerGoliath"));
                beeProjectileList.Add(calamity.ProjectileType("PlagueStingerGoliathV2"));
                beeProjectileList.Add(calamity.ProjectileType("PlagueExplosion"));

                debuffList.Add(calamity.BuffType("BrimstoneFlames"));
                debuffList.Add(calamity.BuffType("BurningBlood"));
                debuffList.Add(calamity.BuffType("GlacialState"));
                debuffList.Add(calamity.BuffType("GodSlayerInferno"));
                debuffList.Add(calamity.BuffType("HolyLight"));
                debuffList.Add(calamity.BuffType("Irradiated"));
                debuffList.Add(calamity.BuffType("Plague"));
                debuffList.Add(calamity.BuffType("AbyssalFlames"));
                debuffList.Add(calamity.BuffType("CrushDepth"));
                debuffList.Add(calamity.BuffType("Horror"));
                debuffList.Add(calamity.BuffType("MarkedforDeath"));

                fireWeaponList.Add(calamity.ItemType("AegisBlade"));
                fireWeaponList.Add(calamity.ItemType("BalefulHarvester"));
                fireWeaponList.Add(calamity.ItemType("Chaotrix"));
                fireWeaponList.Add(calamity.ItemType("CometQuasher"));
                fireWeaponList.Add(calamity.ItemType("DraconicDestruction"));
                fireWeaponList.Add(calamity.ItemType("Drataliornus"));
                fireWeaponList.Add(calamity.ItemType("EnergyStaff"));
                fireWeaponList.Add(calamity.ItemType("ExsanguinationLance"));
                fireWeaponList.Add(calamity.ItemType("FirestormCannon"));
                fireWeaponList.Add(calamity.ItemType("FlameburstShortsword"));
                fireWeaponList.Add(calamity.ItemType("FlameScythe"));
                fireWeaponList.Add(calamity.ItemType("FlameScytheMelee"));
                fireWeaponList.Add(calamity.ItemType("FlareBolt"));
                fireWeaponList.Add(calamity.ItemType("FlarefrostBlade"));
                fireWeaponList.Add(calamity.ItemType("FlarewingBow"));
                fireWeaponList.Add(calamity.ItemType("ForbiddenSun"));
                fireWeaponList.Add(calamity.ItemType("FrigidflashBolt"));
                fireWeaponList.Add(calamity.ItemType("GreatbowofTurmoil"));
                fireWeaponList.Add(calamity.ItemType("HarvestStaff"));
                fireWeaponList.Add(calamity.ItemType("HellBurst"));
                fireWeaponList.Add(calamity.ItemType("HellfireFlamberge"));
                fireWeaponList.Add(calamity.ItemType("Hellkite"));
                fireWeaponList.Add(calamity.ItemType("HellwingStaff"));
                fireWeaponList.Add(calamity.ItemType("Helstorm"));
                fireWeaponList.Add(calamity.ItemType("InfernaCutter"));
                fireWeaponList.Add(calamity.ItemType("Lazhar"));
                fireWeaponList.Add(calamity.ItemType("MeteorFist"));
                fireWeaponList.Add(calamity.ItemType("Mourningstar"));
                fireWeaponList.Add(calamity.ItemType("PhoenixBlade"));
                fireWeaponList.Add(calamity.ItemType("Photoviscerator"));
                fireWeaponList.Add(calamity.ItemType("RedSun"));
                fireWeaponList.Add(calamity.ItemType("SpectralstormCannon"));
                fireWeaponList.Add(calamity.ItemType("SunGodStaff"));
                fireWeaponList.Add(calamity.ItemType("SunSpiritStaff"));
                fireWeaponList.Add(calamity.ItemType("TearsofHeaven"));
                fireWeaponList.Add(calamity.ItemType("TerraFlameburster"));
                fireWeaponList.Add(calamity.ItemType("TheEmpyrean"));
                fireWeaponList.Add(calamity.ItemType("TheWand"));
                fireWeaponList.Add(calamity.ItemType("VenusianTrident"));
                fireWeaponList.Add(calamity.ItemType("Vesuvius"));
                fireWeaponList.Add(calamity.ItemType("BlissfulBombardier"));
                fireWeaponList.Add(calamity.ItemType("HolyCollider"));
                fireWeaponList.Add(calamity.ItemType("MoltenAmputator"));
                fireWeaponList.Add(calamity.ItemType("PurgeGuzzler"));
                fireWeaponList.Add(calamity.ItemType("SolarFlare"));
                fireWeaponList.Add(calamity.ItemType("TelluricGlare"));
                fireWeaponList.Add(calamity.ItemType("AngryChickenStaff"));
                fireWeaponList.Add(calamity.ItemType("ChickenCannon"));
                fireWeaponList.Add(calamity.ItemType("DragonRage"));
                fireWeaponList.Add(calamity.ItemType("DragonsBreath"));
                fireWeaponList.Add(calamity.ItemType("PhoenixFlameBarrage"));
                fireWeaponList.Add(calamity.ItemType("ProfanedTrident"));
                fireWeaponList.Add(calamity.ItemType("TheBurningSky"));

                natureWeaponList.Add(calamity.ItemType("DepthBlade"));
                natureWeaponList.Add(calamity.ItemType("AbyssBlade"));
                natureWeaponList.Add(calamity.ItemType("NeptunesBounty"));
                natureWeaponList.Add(calamity.ItemType("AquaticDissolution"));
                natureWeaponList.Add(calamity.ItemType("ArchAmaryllis"));
                natureWeaponList.Add(calamity.ItemType("BiomeBlade"));
                natureWeaponList.Add(calamity.ItemType("TrueBiomeBlade"));
                natureWeaponList.Add(calamity.ItemType("OmegaBiomeBlade"));
                natureWeaponList.Add(calamity.ItemType("BladedgeGreatbow"));
                natureWeaponList.Add(calamity.ItemType("BlossomFlux"));
                natureWeaponList.Add(calamity.ItemType("EvergladeSpray"));
                natureWeaponList.Add(calamity.ItemType("FeralthornClaymore"));
                natureWeaponList.Add(calamity.ItemType("Floodtide"));
                natureWeaponList.Add(calamity.ItemType("FourSeasonsGalaxia"));
                natureWeaponList.Add(calamity.ItemType("GammaFusillade"));
                natureWeaponList.Add(calamity.ItemType("GleamingMagnolia"));
                natureWeaponList.Add(calamity.ItemType("HarvestStaff"));
                natureWeaponList.Add(calamity.ItemType("HellionFlowerSpear"));
                natureWeaponList.Add(calamity.ItemType("Lazhar"));
                natureWeaponList.Add(calamity.ItemType("LifefruitScythe"));
                natureWeaponList.Add(calamity.ItemType("ManaRose"));
                natureWeaponList.Add(calamity.ItemType("MangroveChakram"));
                natureWeaponList.Add(calamity.ItemType("MangroveChakramMelee"));
                natureWeaponList.Add(calamity.ItemType("MantisClaws"));
                natureWeaponList.Add(calamity.ItemType("Mariana"));
                natureWeaponList.Add(calamity.ItemType("Mistlestorm"));
                natureWeaponList.Add(calamity.ItemType("Monsoon"));
                natureWeaponList.Add(calamity.ItemType("Alluvion"));
                natureWeaponList.Add(calamity.ItemType("Needler"));
                natureWeaponList.Add(calamity.ItemType("NettlelineGreatbow"));
                natureWeaponList.Add(calamity.ItemType("Quagmire"));
                natureWeaponList.Add(calamity.ItemType("Shroomer"));
                natureWeaponList.Add(calamity.ItemType("SolsticeClaymore"));
                natureWeaponList.Add(calamity.ItemType("SporeKnife"));
                natureWeaponList.Add(calamity.ItemType("Spyker"));
                natureWeaponList.Add(calamity.ItemType("StormSaber"));
                natureWeaponList.Add(calamity.ItemType("StormRuler"));
                natureWeaponList.Add(calamity.ItemType("StormSurge"));
                natureWeaponList.Add(calamity.ItemType("TarragonThrowingDart"));
                natureWeaponList.Add(calamity.ItemType("TerraEdge"));
                natureWeaponList.Add(calamity.ItemType("TerraLance"));
                natureWeaponList.Add(calamity.ItemType("TerraRay"));
                natureWeaponList.Add(calamity.ItemType("TerraShiv"));
                natureWeaponList.Add(calamity.ItemType("Terratomere"));
                natureWeaponList.Add(calamity.ItemType("TerraFlameburster"));
                natureWeaponList.Add(calamity.ItemType("TheSwarmer"));
                natureWeaponList.Add(calamity.ItemType("Verdant"));
                natureWeaponList.Add(calamity.ItemType("Barinautical"));
                natureWeaponList.Add(calamity.ItemType("DeepseaStaff"));
                natureWeaponList.Add(calamity.ItemType("Downpour"));
                natureWeaponList.Add(calamity.ItemType("SubmarineShocker"));
                natureWeaponList.Add(calamity.ItemType("Archerfish"));
                natureWeaponList.Add(calamity.ItemType("BallOFugu"));
                natureWeaponList.Add(calamity.ItemType("BlackAnurian"));
                natureWeaponList.Add(calamity.ItemType("CalamarisLament"));
                natureWeaponList.Add(calamity.ItemType("HerringStaff"));
                natureWeaponList.Add(calamity.ItemType("Lionfish"));

                alcoholList.Add(calamity.BuffType("BloodyMary"));
                alcoholList.Add(calamity.BuffType("CaribbeanRum"));
                alcoholList.Add(calamity.BuffType("CinnamonRoll"));
                alcoholList.Add(calamity.BuffType("Everclear"));
                alcoholList.Add(calamity.BuffType("EvergreenGin"));
                alcoholList.Add(calamity.BuffType("Fireball"));
                alcoholList.Add(calamity.BuffType("GrapeBeer"));
                alcoholList.Add(calamity.BuffType("Margarita"));
                alcoholList.Add(calamity.BuffType("Moonshine"));
                alcoholList.Add(calamity.BuffType("MoscowMule"));
                alcoholList.Add(calamity.BuffType("RedWine"));
                alcoholList.Add(calamity.BuffType("Rum"));
                alcoholList.Add(calamity.BuffType("Screwdriver"));
                alcoholList.Add(calamity.BuffType("StarBeamRye"));
                alcoholList.Add(calamity.BuffType("Tequila"));
                alcoholList.Add(calamity.BuffType("TequilaSunrise"));
                alcoholList.Add(calamity.BuffType("Vodka"));
                alcoholList.Add(calamity.BuffType("Whiskey"));
                alcoholList.Add(calamity.BuffType("WhiteWine"));

                lightGreenThrowingRarityList.Add(calamity.ItemType("TarragonThrowingDart"));
                lightGreenThrowingRarityList.Add(calamity.ItemType("MoltenAmputator"));
                lightGreenThrowingRarityList.Add(calamity.ItemType("AccretionDisk"));

                greenThrowingRarityList.Add(calamity.ItemType("CosmicKunai"));
                greenThrowingRarityList.Add(calamity.ItemType("Eradicator"));
                greenThrowingRarityList.Add(calamity.ItemType("Valediction"));
                greenThrowingRarityList.Add(calamity.ItemType("GhoulishGouger"));

                blueThrowingRarityList.Add(calamity.ItemType("EmpyreanKnivesThrown"));
                blueThrowingRarityList.Add(calamity.ItemType("ExecutionersBlade"));
                blueThrowingRarityList.Add(calamity.ItemType("ProfanedTrident"));
                blueThrowingRarityList.Add(calamity.ItemType("ScourgeoftheCosmosThrown"));

                purpleThrowingRarityList.Add(calamity.ItemType("RoyalKnives"));
                purpleThrowingRarityList.Add(calamity.ItemType("TriactisTruePaladinianMageHammerofMight"));

                throwingProjectileList.Add(calamity.ProjectileType("XerocPitchforkProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("WulfrumKnife"));
                throwingProjectileList.Add(calamity.ProjectileType("Valediction"));
                throwingProjectileList.Add(calamity.ProjectileType("UrchinStinger"));
                throwingProjectileList.Add(calamity.ProjectileType("OPHammer"));
                throwingProjectileList.Add(calamity.ProjectileType("TriactisOPHammer"));
                throwingProjectileList.Add(calamity.ProjectileType("ToothBallProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("TitaniumShurikenProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("TarragonThrowingDartProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("FossilShardThrown"));
                throwingProjectileList.Add(calamity.ProjectileType("SpearofPaleolith"));
                throwingProjectileList.Add(calamity.ProjectileType("SeashellBoomerangProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("ScourgeoftheDesert"));
                throwingProjectileList.Add(calamity.ProjectileType("ScourgeoftheCosmosMini"));
                throwingProjectileList.Add(calamity.ProjectileType("ScourgeoftheCosmos"));
                throwingProjectileList.Add(calamity.ProjectileType("RoyalKnife"));
                throwingProjectileList.Add(calamity.ProjectileType("RotBallProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("Pwnagehammer"));
                throwingProjectileList.Add(calamity.ProjectileType("ProfanedTrident"));
                throwingProjectileList.Add(calamity.ProjectileType("PalladiumJavelinProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("OrichalcumSpikedGemstoneProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("MythrilKnifeProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("Mycoroot"));
                throwingProjectileList.Add(calamity.ProjectileType("MoltenBlobThrown"));
                throwingProjectileList.Add(calamity.ProjectileType("MoltenAmputator"));
                throwingProjectileList.Add(calamity.ProjectileType("MangroveChakramProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("MalachiteBolt"));
                throwingProjectileList.Add(calamity.ProjectileType("Malachite"));
                throwingProjectileList.Add(calamity.ProjectileType("Lionfish"));
                throwingProjectileList.Add(calamity.ProjectileType("IchorSpear"));
                throwingProjectileList.Add(calamity.ProjectileType("IceStarProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("GhoulishGouger"));
                throwingProjectileList.Add(calamity.ProjectileType("GelDartProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("FlameScytheProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("FeatherKnifeProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("ExecutionersBlade"));
                throwingProjectileList.Add(calamity.ProjectileType("EmpyreanKnives"));
                throwingProjectileList.Add(calamity.ProjectileType("EradicatorProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("DuststormCloud"));
                throwingProjectileList.Add(calamity.ProjectileType("DuststormInABottle"));
                throwingProjectileList.Add(calamity.ProjectileType("CursedDagger"));
                throwingProjectileList.Add(calamity.ProjectileType("CrystalPiercerProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("Crystalline2"));
                throwingProjectileList.Add(calamity.ProjectileType("Crystalline"));
                throwingProjectileList.Add(calamity.ProjectileType("Crushax"));
                throwingProjectileList.Add(calamity.ProjectileType("CosmicKunai"));
                throwingProjectileList.Add(calamity.ProjectileType("CobaltKunaiProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("Celestus2"));
                throwingProjectileList.Add(calamity.ProjectileType("Celestus"));
                throwingProjectileList.Add(calamity.ProjectileType("Brimblade2"));
                throwingProjectileList.Add(calamity.ProjectileType("Brimblade"));
                throwingProjectileList.Add(calamity.ProjectileType("BrackishWaterBlast"));
                throwingProjectileList.Add(calamity.ProjectileType("BrackishWater"));
                throwingProjectileList.Add(calamity.ProjectileType("BrackishFlask"));
                throwingProjectileList.Add(calamity.ProjectileType("BallisticPoisonBombSpike"));
                throwingProjectileList.Add(calamity.ProjectileType("BallisticPoisonBomb"));
                throwingProjectileList.Add(calamity.ProjectileType("AccretionDisk2"));
                throwingProjectileList.Add(calamity.ProjectileType("AccretionDisk"));
                throwingProjectileList.Add(calamity.ProjectileType("AdamantiteThrowingAxeProjectile"));
                throwingProjectileList.Add(calamity.ProjectileType("FrostyFlare"));
                throwingProjectileList.Add(calamity.ProjectileType("FrostShardFriendly"));

                throwingItemList.Add(calamity.ItemType("AccretionDisk"));
                throwingItemList.Add(calamity.ItemType("AdamantiteThrowingAxe"));
                throwingItemList.Add(calamity.ItemType("BallisticPoisonBomb"));
                throwingItemList.Add(calamity.ItemType("BrackishFlask"));
                throwingItemList.Add(calamity.ItemType("Brimblade"));
                throwingItemList.Add(calamity.ItemType("Celestus"));
                throwingItemList.Add(calamity.ItemType("CobaltKunai"));
                throwingItemList.Add(calamity.ItemType("CosmicKunai"));
                throwingItemList.Add(calamity.ItemType("CrushsawCrasher"));
                throwingItemList.Add(calamity.ItemType("Crystalline"));
                throwingItemList.Add(calamity.ItemType("CrystalPiercer"));
                throwingItemList.Add(calamity.ItemType("CursedDagger"));
                throwingItemList.Add(calamity.ItemType("DuststormInABottle"));
                throwingItemList.Add(calamity.ItemType("EmpyreanKnivesThrown"));
                throwingItemList.Add(calamity.ItemType("Eradicator"));
                throwingItemList.Add(calamity.ItemType("ExecutionersBlade"));
                throwingItemList.Add(calamity.ItemType("FeatherKnife"));
                throwingItemList.Add(calamity.ItemType("FlameScythe"));
                throwingItemList.Add(calamity.ItemType("GelDart"));
                throwingItemList.Add(calamity.ItemType("GhoulishGouger"));
                throwingItemList.Add(calamity.ItemType("IceStar"));
                throwingItemList.Add(calamity.ItemType("IchorSpear"));
                throwingItemList.Add(calamity.ItemType("Lionfish"));
                throwingItemList.Add(calamity.ItemType("Malachite"));
                throwingItemList.Add(calamity.ItemType("MangroveChakram"));
                throwingItemList.Add(calamity.ItemType("MoltenAmputator"));
                throwingItemList.Add(calamity.ItemType("Mycoroot"));
                throwingItemList.Add(calamity.ItemType("MythrilKnife"));
                throwingItemList.Add(calamity.ItemType("OrichalcumSpikedGemstone"));
                throwingItemList.Add(calamity.ItemType("PalladiumJavelin"));
                throwingItemList.Add(calamity.ItemType("ProfanedTrident"));
                throwingItemList.Add(calamity.ItemType("PwnageHammer"));
                throwingItemList.Add(calamity.ItemType("RotBall"));
                throwingItemList.Add(calamity.ItemType("RoyalKnives"));
                throwingItemList.Add(calamity.ItemType("ScourgeoftheCosmosThrown"));
                throwingItemList.Add(calamity.ItemType("ScourgeoftheDesert"));
                throwingItemList.Add(calamity.ItemType("SeashellBoomerang"));
                throwingItemList.Add(calamity.ItemType("SpearofPaleolith"));
                throwingItemList.Add(calamity.ItemType("TarragonThrowingDart"));
                throwingItemList.Add(calamity.ItemType("TitaniumShuriken"));
                throwingItemList.Add(calamity.ItemType("ToothBall"));
                throwingItemList.Add(calamity.ItemType("TriactisTruePaladinianMageHammerofMight"));
                throwingItemList.Add(calamity.ItemType("TruePaladinsHammer"));
                throwingItemList.Add(calamity.ItemType("UrchinStinger"));
                throwingItemList.Add(calamity.ItemType("Valediction"));
                throwingItemList.Add(calamity.ItemType("WulfrumKnife"));
                throwingItemList.Add(calamity.ItemType("XerocPitchfork"));
                throwingItemList.Add(calamity.ItemType("FrostyFlare"));

                revengeanceEnemyBuffList.Add(calamity.NPCType("Astrageldon"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("AstrumDeusHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("AstrumDeusHeadSpectral"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Bumblefuck"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("CalamitasRun3"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("CosmicWraith"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("CrabulonIdle"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Cryogen"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("DesertScourgeHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("GreatSandShark"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("HiveMindP2"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("DankCreeper"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("AquaticAberration"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Leviathan"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Siren"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PerforatorHeadLarge"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PerforatorHeadMedium"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PerforatorHeadSmall"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PlaguebringerGoliath"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PlagueHomingMissile"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PlagueMine"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Polterghast"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PolterghastHook"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("PolterPhantom"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ProfanedGuardianBoss"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("RockPillar"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ScavengerBody"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ScavengerClawRight"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ScavengerClawLeft"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ProfanedGuardianBoss2"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ProvSpawnDefense"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ProvSpawnOffense"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("SlimeGod"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("SlimeGodRun"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("SlimeGodCore"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("SlimeGodSplit"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("SlimeGodRunSplit"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("StormWeaverHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("StormWeaverHeadNaked"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("SupremeCalamitas"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("DevourerofGodsHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("DevourerofGodsHead2"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("DevourerofGodsHeadS"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Yharon"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("AquaticScourgeHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("BobbitWormHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("AquaticSeekerHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ColossalSquid"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("EidolonWyrmHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("EidolonWyrmHeadHuge"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("GulperEelHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Mauler"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Reaper"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Atlas"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ArmoredDiggerHead"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Cnidrion"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("Horse"));
                revengeanceEnemyBuffList.Add(calamity.NPCType("ScornEater"));

                revengeanceProjectileBuffList.Add(calamity.ProjectileType("AbyssMine"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("AbyssMine2"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("AstralFlame"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BloodGeyser"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneBarrage"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneFireblast"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneGigaBlast"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneHellblast"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneHellblast2"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneHellfireball"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneLaser"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneLaserSplit"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneMonster"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("BrimstoneWave"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("DarkEnergyBall"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("DeusMine"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("DoGDeath"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("DoGFire"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("CosmicFlameBurst"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("FlareBomb"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("Flarenado"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("HiveBombGoliath"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("HolyBlast"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("HolyBomb"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("HolyShot"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("HolySpear"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("IceBomb"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("IchorShot"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("Infernado"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("LeviathanBomb"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("MoltenBlast"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("Mushmash"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("PhantomMine"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("PhantomBlast"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("PhantomBlast2"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("ProfanedSpear"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("ProvidenceCrystalShard"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("ProvidenceHolyRay"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("RedLightningFeather"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("SandPoisonCloud"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("ScavengerNuke"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("SirenSong"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("YharonFireball"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("YharonFireball2"));
                revengeanceProjectileBuffList.Add(calamity.ProjectileType("YharonFireballHoming"));
            }
        }
        #endregion

        #region Music
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
		{
    		Mod mod = ModLoader.GetMod("CalamityMod");
            if (Main.musicVolume != 0)
            {
                if (Main.myPlayer != -1 && !Main.gameMenu && Main.LocalPlayer.active)
                {
                    if (NPC.AnyNPCs(NPCID.CultistBoss))
                    {
                        music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Doomsayer"); priority = MusicPriority.BossMedium;
                    }
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneCalamity)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses) { music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Crag"); priority = MusicPriority.Environment; }
                    }
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneAstral)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses) { music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Astral"); priority = MusicPriority.Environment; }
                    }
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneAbyssLayer1 || Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneAbyssLayer2)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses) { music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TheAbyss"); priority = MusicPriority.BiomeHigh; }
                    }
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneAbyssLayer3)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses) { music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TheDeepAbyss"); priority = MusicPriority.BiomeHigh; }
                    }
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneAbyssLayer4)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses) { music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/TheVoid"); priority = MusicPriority.BiomeHigh; }
                    }
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(this).ZoneSulphur)
                    {
                        if (!CalamityPlayer.areThereAnyDamnBosses) { music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/Sulphur"); priority = MusicPriority.BiomeHigh; }
                    }
                    if (CalamityWorld.DoGSecondStageCountdown <= 540 && CalamityWorld.DoGSecondStageCountdown > 60) //8 seconds before DoG spawns
                    {
                        music = mod.GetSoundSlot(SoundType.Music, "Sounds/Music/UniversalCollapse"); priority = MusicPriority.BossMedium;
                    }
                }
            }
		}
        #endregion

        #region PostSetupContent
        public override void PostSetupContent()
        {
    		Mod mod = ModLoader.GetMod("CalamityMod");
            Mod bossChecklist = ModLoader.GetMod("BossChecklist");

            if (bossChecklist != null)
            {
                #region NormalModes
                // 14 is moonlord, 12 is duke fishron
                bossChecklist.Call("AddBossWithInfo", "Desert Scourge", 1.5f, (Func<bool>)(() => CalamityWorld.downedDesertScourge), "Use a [i:" + mod.ItemType("DriedSeafood") + "] in the Desert Biome"); //1
                bossChecklist.Call("AddBossWithInfo", "Crabulon", 2.5f, (Func<bool>)(() => CalamityWorld.downedCrabulon), "Use a [i:" + mod.ItemType("DecapoditaSprout") + "] in the Mushroom Biome"); //1.5
                bossChecklist.Call("AddBossWithInfo", "Hive Mind / Perforator", 3.5f, (Func<bool>)(() => (CalamityWorld.downedPerforator || CalamityWorld.downedHiveMind)), "By killing a Cyst in the World's Evil Biome"); //2
                bossChecklist.Call("AddBossWithInfo", "Slime God", 5.5f, (Func<bool>)(() => CalamityWorld.downedSlimeGod), "Use an [i:" + mod.ItemType("OverloadedSludge") + "]"); //4
                bossChecklist.Call("AddBossWithInfo", "Cryogen", 6.5f, (Func<bool>)(() => CalamityWorld.downedCryogen), "Use a [i:" + mod.ItemType("CryoKey") + "] in the Snow Biome"); //5
                bossChecklist.Call("AddBossWithInfo", "Brimstone Elemental", 7.5f, (Func<bool>)(() => CalamityWorld.downedBrimstoneElemental), "Use a [i:" + mod.ItemType("CharredIdol") + "] in the Hell Crag"); //6
                bossChecklist.Call("AddBossWithInfo", "Aquatic Scourge", 8.5f, (Func<bool>)(() => CalamityWorld.downedAquaticScourge), "Use a [i:" + mod.ItemType("Seafood") + "] in the Sulphuric Sea or wait for it to spawn in the Sulphuric Sea"); //6
                bossChecklist.Call("AddBossWithInfo", "Calamitas", 9.7f, (Func<bool>)(() => CalamityWorld.downedCalamitas), "Use an [i:" + mod.ItemType("BlightedEyeball") + "] at Night"); //7
                bossChecklist.Call("AddBossWithInfo", "Leviathan", 10.5f, (Func<bool>)(() => CalamityWorld.downedLeviathan), "By killing an unknown entity in the Ocean Biome"); //8
                bossChecklist.Call("AddBossWithInfo", "Astrum Aureus", 10.55f, (Func<bool>)(() => CalamityWorld.downedAstrageldon), "Use an [i:" + mod.ItemType("AstralChunk") + "] at Night"); //8.25
                bossChecklist.Call("AddBossWithInfo", "Astrum Deus", 10.6f, (Func<bool>)(() => CalamityWorld.downedStarGod), "Use a [i:" + mod.ItemType("Starcore") + "] at Night"); //8.5
                bossChecklist.Call("AddBossWithInfo", "Plaguebringer Goliath", 11.5f, (Func<bool>)(() => CalamityWorld.downedPlaguebringer), "Use an [i:" + mod.ItemType("Abomination") + "] in the Jungle Biome"); //9
                bossChecklist.Call("AddBossWithInfo", "Ravager", 12.5f, (Func<bool>)(() => CalamityWorld.downedScavenger), "Use an [i:" + mod.ItemType("AncientMedallion") + "]"); //9.5
                bossChecklist.Call("AddBossWithInfo", "Profaned Guardians", 14.5f, (Func<bool>)(() => CalamityWorld.downedGuardians), "Use a [i:" + mod.ItemType("ProfanedShard") + "] in the Hallow or Underworld Biomes"); //10
                bossChecklist.Call("AddBossWithInfo", "Providence", 15f, (Func<bool>)(() => CalamityWorld.downedProvidence), "Use a [i:" + mod.ItemType("ProfanedCore") + "] in the Hallow or Underworld Biomes"); //11
                bossChecklist.Call("AddBossWithInfo", "Ceaseless Void", 15.1f, (Func<bool>)(() => CalamityWorld.downedSentinel1), "Use a [i:" + mod.ItemType("RuneofCos") + "] in the Dungeon"); //12
                bossChecklist.Call("AddBossWithInfo", "Storm Weaver", 15.2f, (Func<bool>)(() => CalamityWorld.downedSentinel2), "Use a [i:" + mod.ItemType("RuneofCos") + "] in Space"); //13
                bossChecklist.Call("AddBossWithInfo", "Signus", 15.3f, (Func<bool>)(() => CalamityWorld.downedSentinel3), "Use a [i:" + mod.ItemType("RuneofCos") + "] in the Underworld"); //14
                bossChecklist.Call("AddBossWithInfo", "Polterghast", 15.5f, (Func<bool>)(() => CalamityWorld.downedPolterghast), "Use a [i:" + mod.ItemType("NecroplasmicBeacon") + "] in the Dungeon or kill 30 phantom spirits"); //11
                bossChecklist.Call("AddBossWithInfo", "Devourer of Gods", 16f, (Func<bool>)(() => CalamityWorld.downedDoG), "Use a [i:" + mod.ItemType("CosmicWorm") + "]"); //15
                bossChecklist.Call("AddBossWithInfo", "Bumblebirb", 16.5f, (Func<bool>)(() => CalamityWorld.downedBumble), "Use [i:" + mod.ItemType("BirbPheromones") + "] in the Jungle Biome or find it in the Jungle Biome"); //16
                bossChecklist.Call("AddBossWithInfo", "Yharon", 17f, (Func<bool>)(() => CalamityWorld.downedYharon), "Use a [i:" + mod.ItemType("ChickenEgg") + "] in the Jungle Biome"); //17
                bossChecklist.Call("AddBossWithInfo", "Supreme Calamitas", 18f, (Func<bool>)(() => CalamityWorld.downedSCal), "Use an [i:" + mod.ItemType("EyeofExtinction") + "]"); //18
                //bossChecklist.Call("AddBossWithInfo", "MEME GOD", 19f, (Func<bool>)(() => CalamityWorld.downedLORDE), "Use [i:" + mod.ItemType("NO") + "]"); //19
                #endregion
            }
        }
        #endregion

        #region DrawingStuff
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    	{
    		Mod mod = ModLoader.GetMod("CalamityMod");
            if (CalamityWorld.revenge)
			{
			    UIHandler.ModifyInterfaceLayers(mod, layers);
			}
            int index = layers.FindIndex(layer => layer.Name == "Vanilla: Mouse Text");
            if (index != -1)
            {
                layers.Insert(index, new LegacyGameInterfaceLayer("Boss HP Bars", delegate ()
                {
                    if (Main.LocalPlayer.GetModPlayer<CalamityPlayer>(mod).drawBossHPBar)
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
            return npc.GetAlpha(BuffEffects(npc, GetLightColor(position != null ? (Vector2)position : npc.Center), (shadowOverride != 0f ? shadowOverride : 0f), effects, npc.poisoned, npc.onFire, npc.onFire2, Main.player[Main.myPlayer].detectCreature, false, false, false, npc.venom, npc.midas, npc.ichor, npc.onFrostBurn, false, false, npc.dripping, npc.drippingSlime, npc.loveStruck, npc.stinky));
        }
    	
    	public static Color GetLightColor(Vector2 position)
        {
            return Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));
        }
    	
    	public static Color BuffEffects(Entity codable, Color lightColor, float shadow = 0f, bool effects = true, bool poisoned = false, bool onFire = false, bool onFire2 = false, bool hunter = false, bool noItems = false, bool blind = false, bool bleed = false, bool venom = false, bool midas = false, bool ichor = false, bool onFrostBurn = false, bool burned = false, bool honey = false, bool dripping = false, bool drippingSlime = false, bool loveStruck = false, bool stinky = false)
        {
            float cr = 1f; float cg = 1f; float cb = 1f; float ca = 1f;
			if (effects && honey && Main.rand.Next(30) == 0)
			{
				int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 152, 0f, 0f, 150, default(Color), 1f);
				Main.dust[dustID].velocity.Y = 0.3f;
				Main.dust[dustID].velocity.X *= 0.1f;
				Main.dust[dustID].scale += (float)Main.rand.Next(3, 4) * 0.1f;
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
				if (effects && Main.rand.Next(30) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 46, 0f, 0f, 120, default(Color), 0.2f);
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
				if (effects && Main.rand.Next(10) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 171, 0f, 0f, 100, default(Color), 0.5f);
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
					int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default(Color), 2f);
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
						int dustID = Dust.NewDust(new Vector2(codable.position.X - 2f, codable.position.Y - 2f), codable.width + 4, codable.height + 4, 135, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default(Color), 3.5f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].velocity *= 1.8f;
						Main.dust[dustID].velocity.Y -= 0.5f;
						if (Main.rand.Next(4) == 0)
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
						int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 6, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default(Color), 3.5f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].velocity *= 1.8f;
						Main.dust[dustID].velocity.Y -= 0.5f;
						if (Main.rand.Next(4) == 0)
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
				position.X -= 2f; position.Y -= 2f;
				if (Main.rand.Next(2) == 0)
				{
					int dustID = Dust.NewDust(position, codable.width + 4, codable.height + 2, 211, 0f, 0f, 50, default(Color), 0.8f);
					if (Main.rand.Next(2) == 0) 
					{
						Main.dust[dustID].alpha += 25;
					}
					if (Main.rand.Next(2) == 0) 
					{
						Main.dust[dustID].alpha += 25;
					}
					Main.dust[dustID].noLight = true;
					Main.dust[dustID].velocity *= 0.2f;
					Main.dust[dustID].velocity.Y += 0.2f;
					Main.dust[dustID].velocity += codable.velocity;
					if(codable is Player) 
					{
						Main.playerDrawDust.Add(dustID);
					}
				}
				else
				{
					int dustID = Dust.NewDust(position, codable.width + 8, codable.height + 8, 211, 0f, 0f, 50, default(Color), 1.1f);
					if (Main.rand.Next(2) == 0) 
					{
						Main.dust[dustID].alpha += 25;
					}
					if (Main.rand.Next(2) == 0) 
					{
						Main.dust[dustID].alpha += 25;
					}
					Main.dust[dustID].noLight = true;
					Main.dust[dustID].noGravity = true;
					Main.dust[dustID].velocity *= 0.2f;
					Main.dust[dustID].velocity.Y += 1f;
					Main.dust[dustID].velocity += codable.velocity;
					if(codable is Player) 
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
					if (Main.rand.Next(2) == 0)
					{
						Vector2 position2 = codable.position;
						position2.X -= 2f; position2.Y -= 2f;
						int dustID = Dust.NewDust(position2, codable.width + 4, codable.height + 2, 4, 0f, 0f, alpha, newColor, 1.4f);
						if (Main.rand.Next(2) == 0) 
						{
							Main.dust[dustID].alpha += 25;
						}
						if (Main.rand.Next(2) == 0) 
						{
							Main.dust[dustID].alpha += 25;
						}
						Main.dust[dustID].noLight = true;
						Main.dust[dustID].velocity *= 0.2f;
						Main.dust[dustID].velocity.Y += 0.2f;
						Main.dust[dustID].velocity += codable.velocity;
						if(codable is Player) 
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
						int dustID = Dust.NewDust(codable.position - new Vector2(2f, 2f), codable.width + 4, codable.height + 4, 75, codable.velocity.X * 0.4f, codable.velocity.Y * 0.4f, 100, default(Color), 3.5f);
						Main.dust[dustID].noGravity = true;
						Main.dust[dustID].velocity *= 1.8f;
						Main.dust[dustID].velocity.Y -= 0.5f;
						if (Main.rand.Next(4) == 0)
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
				bool dead = (codable is Player ? ((Player)codable).dead : codable is NPC ? ((NPC)codable).life <= 0 : false);
				if (effects && !dead && Main.rand.Next(30) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 5, 0f, 0f, 0, default(Color), 1f);
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
			if (loveStruck && effects && shadow == 0f && Main.instance.IsActive && !Main.gamePaused && Main.rand.Next(5) == 0)
			{
				Vector2 value = new Vector2((float)Main.rand.Next(-10, 11), (float)Main.rand.Next(-10, 11));
				value.Normalize();
				value.X *= 0.66f;
				int goreID = Gore.NewGore(codable.position + new Vector2((float)Main.rand.Next(codable.width + 1), (float)Main.rand.Next(codable.height + 1)), value * (float)Main.rand.Next(3, 6) * 0.33f, 331, (float)Main.rand.Next(40, 121) * 0.01f);
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
				if (effects && Main.rand.Next(5) == 0 && Main.instance.IsActive && !Main.gamePaused)
				{
					Vector2 value2 = new Vector2((float)Main.rand.Next(-10, 11), (float)Main.rand.Next(-10, 11));
					value2.Normalize(); value2.X *= 0.66f; value2.Y = Math.Abs(value2.Y);
					Vector2 vector = value2 * (float)Main.rand.Next(3, 5) * 0.25f;
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 188, vector.X, vector.Y * 0.5f, 100, default(Color), 1.5f);
					Main.dust[dustID].velocity *= 0.1f;
					Main.dust[dustID].velocity.Y -= 0.5f;
					if (codable is Player) 
					{
						Main.playerDrawDust.Add(dustID);
					}
				}
			}
			lightColor.R = (byte)((float)lightColor.R * cr);
			lightColor.G = (byte)((float)lightColor.G * cg);
			lightColor.B = (byte)((float)lightColor.B * cb);
			lightColor.A = (byte)((float)lightColor.A * ca);			
			if (codable is NPC) 
			{
				NPCLoader.DrawEffects((NPC)codable, ref lightColor);
			}
            if (hunter && (codable is NPC ? ((NPC)codable).lifeMax > 1 : true))
            {
				if (effects && !Main.gamePaused && Main.instance.IsActive && Main.rand.Next(50) == 0)
				{
					int dustID = Dust.NewDust(codable.position, codable.width, codable.height, 15, 0f, 0f, 150, default(Color), 0.8f);
					Main.dust[dustID].velocity *= 0.1f;
					Main.dust[dustID].noLight = true;
					if (codable is Player) 
					{
						Main.playerDrawDust.Add(dustID);
					}
				}
				byte colorR = 50, colorG = 255, colorB = 50;
				if(codable is NPC && !(((NPC)codable).friendly || ((NPC)codable).catchItem > 0 || (((NPC)codable).damage == 0 && ((NPC)codable).lifeMax == 5)))
				{
					colorR = 255; colorG = 50;
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
            Color lightColor = (overrideColor != null ? (Color)overrideColor : codable is NPC ? GetNPCColor(((NPC)codable), codable.Center, false) : codable is Projectile ? ((Projectile)codable).GetAlpha(GetLightColor(codable.Center)) : GetLightColor(codable.Center));
            int frameCount = (codable is NPC ? Main.npcFrameCount[((NPC)codable).type] : 1);
            Rectangle frame = (codable is NPC ? ((NPC)codable).frame : new Rectangle(0, 0, texture.Width, texture.Height));
            float scale = (codable is NPC ? ((NPC)codable).scale : ((Projectile)codable).scale);
            float rotation = (codable is NPC ? ((NPC)codable).rotation : ((Projectile)codable).rotation);
            int spriteDirection = (codable is NPC ? ((NPC)codable).spriteDirection : ((Projectile)codable).spriteDirection);
			float offsetY = (codable is NPC ? ((NPC)codable).gfxOffY : 0f);
            DrawTexture(sb, texture, shader, codable.position + new Vector2(0f, offsetY), codable.width, codable.height, scale, rotation, spriteDirection, frameCount, frame, lightColor, drawCentered);
        }
    	
    	public static void DrawTexture(object sb, Texture2D texture, int shader, Vector2 position, int width, int height, float scale, float rotation, int direction, int framecount, Rectangle frame, Color? overrideColor = null, bool drawCentered = false)
        {
            Vector2 origin = new Vector2((float)(texture.Width / 2), (float)(texture.Height / framecount / 2));
            Color lightColor = overrideColor != null ? (Color)overrideColor : GetLightColor(position + new Vector2(width * 0.5f, height * 0.5f));
			if (sb is List<DrawData>)
			{
				DrawData dd = new DrawData(texture, GetDrawPosition(position, origin, width, height, texture.Width, texture.Height, framecount, scale, drawCentered), frame, lightColor, rotation, origin, scale, direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
				dd.shader = shader;
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
				return (position + new Vector2(width * 0.5f, height * 0.5f)) - (texHalf * scale) + (origin * scale) - screenPos;	
			}
			return position - screenPos + new Vector2(width * 0.5f, height) - new Vector2(texWidth * scale / 2f, texHeight * scale / (float)framecount) + (origin * scale) + new Vector2(0f, 5f);
        }
        #endregion

        #region RecipeGroups
        public override void AddRecipeGroups()
		{
		    RecipeGroup group = new RecipeGroup(() => Lang.misc[37] + (" Silt"), new int[]
			{
				424,
				1103
			});
			RecipeGroup.RegisterGroup("SiltGroup", group);

			group = new RecipeGroup(() => Lang.misc[37] + (" Lunar Pickaxe"), new int[]
			{
				2776,
				2781,
				2786,
				3466
			});
			RecipeGroup.RegisterGroup("LunarPickaxe", group);

			group = new RecipeGroup(() => Lang.misc[37] + (" Lunar Axe"), new int[]
			{
				3522,
				3523,
				3524,
				3525
			});
			RecipeGroup.RegisterGroup("LunarAxe", group);

			group = new RecipeGroup(() => Lang.misc[37] + (" Wings"), new int[]
			{
				492,
				493,
				665,
				749,
				761,
				785,
				786,
				821,
				822,
				823,
				948,
				1162,
				1165,
				1515,
				1583,
				1584,
				1585,
				1586,
				1797,
				1830,
                1866,
				1871,
				2280,
				2494,
				2609,
				2770,
				3468,
				3469,
				3470,
				3471,
				3580,
				3582,
				3588,
				3592,
				3883,
                3924,
                3928,
				ItemType("SkylineWings"),
				ItemType("StarlightWings"),
				ItemType("AureateWings"),
				ItemType("DiscordianWings"),
				ItemType("TarragonWings"),
				ItemType("XerocWings")
			});
			RecipeGroup.RegisterGroup("WingsGroup", group);
		}
        #endregion

        #region Recipes
        public override void AddRecipes()
		{
			ModRecipe recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SunplateBlock, 10);
			recipe.AddIngredient(ItemID.Cloud, 5);
			recipe.AddIngredient(ItemID.RainCloud, 3);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.SkyMill);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.IceBlock, 20);
			recipe.AddIngredient(ItemID.Leather, 5);
			recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.IceMachine);
	        recipe.SetResult(ItemID.IceSkates);
	        recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.IceBlock, 20);
            recipe.AddIngredient(ItemID.Glass, 10);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.IceMirror);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.IronBar, 10);
            recipe.AddIngredient(ItemID.Glass, 10);
            recipe.AddIngredient(ItemID.FallenStar, 10);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.MagicMirror);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Lens);
			recipe.AddIngredient(ItemID.BlackDye);
	        recipe.AddTile(TileID.DyeVat);
	        recipe.SetResult(ItemID.BlackLens);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Vertebrae, 5);
	        recipe.AddTile(TileID.WorkBenches);
	        recipe.SetResult(ItemID.Leather);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.IceBlock, 25);
			recipe.AddIngredient(ItemID.SnowBlock, 15);
			recipe.AddIngredient(ItemID.IronBar, 3);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.IceMachine);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.GoldBroadsword);
			recipe.AddIngredient(ItemID.FallenStar, 10);
			recipe.AddIngredient(null, "VictoryShard", 3);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.Starfury);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.PlatinumBroadsword);
			recipe.AddIngredient(ItemID.FallenStar, 10);
			recipe.AddIngredient(null, "VictoryShard", 3);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.Starfury);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Feather, 2);
			recipe.AddIngredient(ItemID.Bottle);
			recipe.AddIngredient(ItemID.Cloud, 25);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.CloudinaBottle);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Feather, 4);
			recipe.AddIngredient(ItemID.Bottle);
			recipe.AddIngredient(ItemID.SnowBlock, 50);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.BlizzardinaBottle);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "DesertFeather", 10);
			recipe.AddIngredient(ItemID.Feather, 6);
			recipe.AddIngredient(ItemID.Bottle);
			recipe.AddIngredient(ItemID.SandBlock, 70);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.SandstorminaBottle);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.WhiteString);
			recipe.AddIngredient(ItemID.Gel, 80);
			recipe.AddIngredient(ItemID.Cloud, 40);
	        recipe.AddTile(TileID.Solidifier);
	        recipe.SetResult(ItemID.ShinyRedBalloon);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.Leather, 5);
			recipe.AddIngredient(ItemID.WaterWalkingPotion, 8);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.WaterWalkingBoots);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.LavaBucket, 5);
			recipe.AddIngredient(ItemID.Obsidian, 25);
			recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.LavaCharm);
	        recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.GoldenKey);
            recipe.AddIngredient(ItemID.Obsidian, 20);
            recipe.AddIngredient(ItemID.Bone, 5);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.ShadowKey);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "CryoBar", 6);
			recipe.AddIngredient(ItemID.FrostCore);
	        recipe.AddTile(TileID.IceMachine);
	        recipe.SetResult(ItemID.FrostHelmet);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "CryoBar", 10);
			recipe.AddIngredient(ItemID.FrostCore);
	        recipe.AddTile(TileID.IceMachine);
	        recipe.SetResult(ItemID.FrostBreastplate);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "CryoBar", 8);
			recipe.AddIngredient(ItemID.FrostCore);
	        recipe.AddTile(TileID.IceMachine);
	        recipe.SetResult(ItemID.FrostLeggings);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.CobaltBar, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.CobaltShield);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.PalladiumBar, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.CobaltShield);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.CobaltBar, 15);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.Muramasa);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.PalladiumBar, 15);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.Muramasa);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "TrueBloodyEdge");
			recipe.AddIngredient(ItemID.TrueExcalibur);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.TerraBlade);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "Ectoblood", 3);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.Ectoplasm);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.EmptyBullet);
			recipe.AddIngredient(ItemID.ExplosivePowder, 3);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.RocketI, 5);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "VictoryShard", 10);
			recipe.AddIngredient(ItemID.SoulofLight, 15);
			recipe.AddIngredient(ItemID.UnicornHorn, 3);
			recipe.AddIngredient(ItemID.LightShard);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.EnchantedSword);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.WormholePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.WarmthPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.CratePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.SonarPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.FishingPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.TeleportationPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.StinkPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.LovePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.WrathPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.InfernoPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.RagePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.EndurancePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.LifeforcePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.AmmoReservationPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.TrapsightPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.SummoningPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.FlipperPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.TitanPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.BuilderPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.CalmingPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.HeartreachPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.MiningPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.GenderChangePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.GravitationPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.HunterPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.ArcheryPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.WaterWalkingPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.ThornsPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.BattlePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.NightOwlPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.ShinePotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.InvisibilityPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.SpelunkerPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.FeatherfallPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.MagicPowerPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.ManaRegenerationPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.IronskinPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.GillsPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.SwiftnessPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.RegenerationPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(null, "BloodOrb", 10);
			recipe.AddIngredient(ItemID.BottledWater);
	        recipe.AddTile(TileID.AlchemyTable);
	        recipe.SetResult(ItemID.ObsidianSkinPotion);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.AncientCloth, 10);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddIngredient(ItemID.SoulofNight, 10);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.FlyingCarpet);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.LihzahrdBrick, 15);
			recipe.AddIngredient(null, "CoreofCinder");
	        recipe.AddTile(TileID.LihzahrdFurnace);
	        recipe.SetResult(ItemID.LihzahrdPowerCell);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.GlowingMushroom, 15);
			recipe.AddIngredient(ItemID.Worm);
	        recipe.AddTile(TileID.Autohammer);
	        recipe.SetResult(ItemID.TruffleWorm);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.Aglet);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.JungleSpores, 15);
			recipe.AddIngredient(ItemID.Cloud, 15);
			recipe.AddIngredient(ItemID.PinkGel, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.AnkletoftheWind);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SunplateBlock, 10);
			recipe.AddIngredient(ItemID.Cloud, 10);
			recipe.AddIngredient(ItemID.GoldBar, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.LuckyHorseshoe);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SunplateBlock, 10);
			recipe.AddIngredient(ItemID.Cloud, 10);
			recipe.AddIngredient(ItemID.PlatinumBar, 5);
	        recipe.AddTile(TileID.Anvils);
	        recipe.SetResult(ItemID.LuckyHorseshoe);
	        recipe.AddRecipe();

	        recipe = new ModRecipe(this);
			recipe.AddIngredient(ItemID.SoulofLight, 30);
			recipe.AddIngredient(ItemID.ChaosFish, 5);
			recipe.AddIngredient(ItemID.PixieDust, 50);
	        recipe.AddTile(TileID.MythrilAnvil);
	        recipe.SetResult(ItemID.RodofDiscord);
	        recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.TurtleShell, 3);
            recipe.AddIngredient(null, "EssenceofEleum", 9);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.FrozenTurtleShell);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(null, "PlantyMush", 10);
            recipe.AddIngredient(null, "LivingShard");
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.LifeFruit);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.FallenStar, 20);
            recipe.AddIngredient(ItemID.SoulofMight, 10);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddIngredient(null, "CryoBar", 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.CelestialMagnet);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 1
            recipe.AddIngredient(ItemID.Silk, 20);
            recipe.AddIngredient(ItemID.SoulofLight, 3);
            recipe.AddIngredient(ItemID.SoulofNight, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.TrifoldMap);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 2
            recipe.AddIngredient(ItemID.Bone, 50);
            recipe.AddIngredient(null, "AncientBoneDust", 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.ArmorPolish);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 3
            recipe.AddIngredient(ItemID.SoulofNight, 20);
            recipe.AddIngredient(ItemID.Lens, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.Nazar);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 4
            recipe.AddIngredient(ItemID.Stinger, 15);
            recipe.AddIngredient(null, "MurkyPaste");
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.Bezoar);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 5
            recipe.AddIngredient(ItemID.BottledWater);
            recipe.AddIngredient(ItemID.Waterleaf, 5);
            recipe.AddIngredient(ItemID.Blinkroot, 5);
            recipe.AddIngredient(ItemID.Daybloom, 5);
            recipe.AddIngredient(null, "BeetleJuice", 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.Vitamins);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 6
            recipe.AddIngredient(ItemID.Silk, 30);
            recipe.AddIngredient(ItemID.SoulofNight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.Blindfold);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 7
            recipe.AddIngredient(ItemID.Timer1Second);
            recipe.AddIngredient(ItemID.PixieDust, 15);
            recipe.AddIngredient(ItemID.SoulofLight, 5);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.FastClock);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 8
            recipe.AddIngredient(ItemID.Wire, 10);
            recipe.AddIngredient(ItemID.HallowedBar, 5);
            recipe.AddIngredient(ItemID.Ruby, 3);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.Megaphone);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //ankh 9
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddIngredient(ItemID.Gel, 50);
            recipe.AddIngredient(ItemID.GreaterHealingPotion);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.AdhesiveBandage);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Frog, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.FrogLeg);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //hermesboots
            recipe.AddIngredient(ItemID.Silk, 10);
            recipe.AddTile(TileID.Loom);
            recipe.SetResult(ItemID.HermesBoots);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);   //radar
            recipe.AddIngredient(ItemID.IronBar, 5);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.Radar);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Bone, 5);
            recipe.AddIngredient(ItemID.PinkGel);
            recipe.AddIngredient(ItemID.HealingPotion);
            recipe.AddIngredient(ItemID.Ruby);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.LifeCrystal);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Wood, 5);
            recipe.AddIngredient(ItemID.Torch, 3);
            recipe.AddIngredient(ItemID.FallenStar);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.WandofSparking);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.IronBar);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.ThrowingKnife, 50);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.IronBar);
            recipe.anyIronBar = true;
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.Shuriken, 50);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Leather, 10);
            recipe.AddTile(TileID.Anvils);
            recipe.SetResult(ItemID.FeralClaws);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.Leather, 5);
            recipe.AddIngredient(null, "BloodOrb", 10);
            recipe.AddTile(TileID.Hellforge);
            recipe.SetResult(ItemID.GuideVoodooDoll);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(ItemID.JungleSpores, 15);
            recipe.AddIngredient(ItemID.RichMahogany, 15);
            recipe.AddIngredient(ItemID.SoulofNight, 15);
            recipe.AddIngredient(ItemID.SoulofLight, 15);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(ItemID.TempleKey);
            recipe.AddRecipe();

            recipe = new ModRecipe(this);
            recipe.AddIngredient(null, "RogueEmblem");
            recipe.AddIngredient(ItemID.SoulofMight, 5);
            recipe.AddIngredient(ItemID.SoulofSight, 5);
            recipe.AddIngredient(ItemID.SoulofFright, 5);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.SetResult(ItemID.AvengerEmblem);
            recipe.AddRecipe();
        }
        #endregion

        #region Packets
        public override void HandlePacket(BinaryReader reader, int whoAmI)
		{
			CalamityModMessageType msgType = (CalamityModMessageType)reader.ReadByte();
            switch (msgType)
			{
                case CalamityModMessageType.MeleeLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleLevels(reader, 0);
                    break;
                case CalamityModMessageType.RangedLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleLevels(reader, 1);
                    break;
                case CalamityModMessageType.MagicLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleLevels(reader, 2);
                    break;
                case CalamityModMessageType.SummonLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleLevels(reader, 3);
                    break;
                case CalamityModMessageType.RogueLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleLevels(reader, 4);
                    break;
                case CalamityModMessageType.ExactMeleeLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleExactLevels(reader, 0);
                    break;
                case CalamityModMessageType.ExactRangedLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleExactLevels(reader, 1);
                    break;
                case CalamityModMessageType.ExactMagicLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleExactLevels(reader, 2);
                    break;
                case CalamityModMessageType.ExactSummonLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleExactLevels(reader, 3);
                    break;
                case CalamityModMessageType.ExactRogueLevelSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleExactLevels(reader, 4);
                    break;
                case CalamityModMessageType.StressSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleStress(reader);
                    break;
                case CalamityModMessageType.Ravager:
                    int ravagerAlive = reader.ReadInt32();
                    CalamityGlobalNPC.scavenger = ravagerAlive;
                    break;
                case CalamityModMessageType.DoG:
                    int DoGAlive = reader.ReadInt32();
                    CalamityGlobalNPC.DoGHead = DoGAlive;
                    break;
                case CalamityModMessageType.Polterghast:
                    int polterAlive = reader.ReadInt32();
                    CalamityGlobalNPC.ghostBoss = polterAlive;
                    break;
                case CalamityModMessageType.LORDE:
                    int lordeAlive = reader.ReadInt32();
                    CalamityGlobalNPC.lordeBoss = lordeAlive;
                    break;
                case CalamityModMessageType.Providence:
                    int provAlive = reader.ReadInt32();
                    CalamityGlobalNPC.holyBoss = provAlive;
                    break;
                case CalamityModMessageType.BossRushStage:
                    int stage = reader.ReadInt32();
                    CalamityWorld.bossRushStage = stage;
                    break;
                case CalamityModMessageType.AdrenalineSync:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleAdrenaline(reader);
                    break;
                case CalamityModMessageType.TeleportPlayer:
                    Main.player[reader.ReadInt32()].GetModPlayer<CalamityPlayer>().HandleTeleport(reader.ReadInt32(), true, whoAmI);
                    break;
                case CalamityModMessageType.SupremeCal:
                    int SCalAlive = reader.ReadInt32();
                    CalamityGlobalNPC.SCal = SCalAlive;
                    break;
                case CalamityModMessageType.DoGCountdownSync:
                    int countdown = reader.ReadInt32();
                    CalamityWorld.DoGSecondStageCountdown = countdown;
                    break;
                default:
					ErrorLogger.Log("CalamityMod: Unknown Message type: " + msgType);
					break;
			}
        }
        #endregion

        public override object Call(params object[] args)
		{
			return ModSupport.Call(args);
		}
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
        Ravager,
        DoG,
        Polterghast,
        LORDE,
        Providence,
        AdrenalineSync,
        TeleportPlayer,
        BossRushStage,
        SupremeCal,
        DoGCountdownSync
    }
}