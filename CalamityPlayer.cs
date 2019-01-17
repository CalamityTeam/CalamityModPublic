using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Localization;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameContent;
using Terraria.GameContent.UI;
using Terraria.GameInput;
using Terraria.Graphics.Capture;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using CalamityMod.NPCs;
using CalamityMod.NPCs.TheDevourerofGods;
using CalamityMod.NPCs.Calamitas;
using CalamityMod.NPCs.PlaguebringerGoliath;
using CalamityMod.NPCs.Yharon;
using CalamityMod.NPCs.Leviathan;
using CalamityMod.Items.Armor;
using CalamityMod.UI;
using CalamityMod.Items.CalamityCustomThrowingDamage;
using Terraria.ModLoader.IO;

namespace CalamityMod
{
	public class CalamityPlayer : ModPlayer
	{
        #region InstanceVars

        #region NormalVars
        public int meleeLevel = 0;

        public int rangedLevel = 0;

        public int magicLevel = 0;

        public int rogueLevel = 0;

        public int summonLevel = 0;

        public int gainLevelCooldown = 120; //120 frames before gaining another stat point, avoiding i-frame nonsense

        public bool shootFireworksLevelUpMelee = true;

        public bool shootFireworksLevelUpRanged = true;

        public bool shootFireworksLevelUpMagic = true;

        public bool shootFireworksLevelUpSummon = true;

        public bool shootFireworksLevelUpRogue = true;

        public int exactMeleeLevel = 0;

        public int exactRangedLevel = 0;

        public int exactMagicLevel = 0;

        public int exactSummonLevel = 0;

        public int exactRogueLevel = 0;

        public int alcoholPoisonLevel = 0;

        public static bool areThereAnyDamnBosses = false;

        private const int saveVersion = 0;
		
		public int modStealthTimer;

        public int hInfernoBoost = 0;

        public int pissWaterBoost = 0;
		
		public int runCheck = 0;
		
		public int packetTimer = 0;

        public int sCalDeathCount = 0;

        public int sCalKillCount = 0;

        public int bloodflareHeartTimer = 180;

        public int bloodflareManaTimer = 180;

		public float modStealth = 1f;

        public float aquaticBoost = 1f;
		
		public float shieldInvinc = 5f;
		
		public int dashMod;
		
		public int dashTimeMod;

		public const float defEndurance = 0.33f;

        public bool playFullRageSound = true;

        public bool playFullAdrenalineSound = true;

        public bool fab = false;

        public bool drawBossHPBar = true;

        public bool shouldDrawSmallText = true;
        #endregion

        #region PetStuff
        public bool leviPet = false;

        public bool sirenPet = false;

        public bool fox = false;

        public bool chibii = false;

        public bool brimling = false;

        public bool bearPet = false;
        #endregion

        #region StressStuff
        public const int stressMax = 10000;

        public int stress;

        public int stressCD;

        public bool heartOfDarkness = false;

        public bool draedonsHeart = false;

        public bool draedonsStressGain = false;
		
		public bool afflicted = false;
		
		public bool affliction = false;
		
		public bool afflictedBuff = false;

        public bool stressPills = false;

        public bool laudanum = false;
        
		public bool stressLevel500 = false;

        public bool rageMode = false;

        //public bool despawnProj = false;
        #endregion

        #region AdrenalineStuff
        public const int adrenalineMax = 10000;

        public int adrenalineMaxTimer = 180;

        public int adrenalineDmgDown = 600;

        public float adrenalineDmgMult = 1f;

        public int adrenaline;

        public int adrenalineCD;

        public bool adrenalineMode = false;
        #endregion

        #region PermanentStuff
        public bool extraAccessoryML = false;
		
		public bool eCore = false;
		
		public bool pHeart = false;
		
		public bool cShard = false;
		
		public bool mFruit = false;
		
		public bool bOrange = false;
		
		public bool eBerry = false;
		
		public bool dFruit = false;

        public bool revJamDrop = false;

        public bool rageBoostOne = false;

        public bool rageBoostTwo = false;

        public bool rageBoostThree = false;

        public bool adrenalineBoostOne = false;

        public bool adrenalineBoostTwo = false;

        public bool adrenalineBoostThree = false;
        #endregion

        #region AccessoryStuff
        public bool fasterMeleeLevel = false;

        public bool fasterRangedLevel = false;

        public bool fasterMagicLevel = false;

        public bool fasterSummonLevel = false;

        public bool fasterRogueLevel = false;

        public bool dodgeScarf = false;

        public bool badgeOfBravery = false;

        public bool scarfCooldown = false;

        public bool cryogenSoul = false;
		
		public bool yInsignia = false;
		
		public bool eGauntlet = false;
		
		public bool eTalisman = false;

        public bool statisBeltOfCurses = false;

        public bool elysianAegis = false;

        public bool elysianGuard = false;

        public bool nCore = false;

        public bool abyssalDivingSuitPlates = false;

        public bool abyssalDivingSuitCooldown = false;

        public int abyssalDivingSuitPlateHits = 0;

        public bool sirenWaterBuff = false;

        public bool sirenIce = false;

        public bool sirenIceCooldown = false;

        public bool aSpark = false;

        public bool aBulwark = false;

        public bool dAmulet = false;

        public bool fCarapace = false;

        public bool gShell = false;

        public bool absorber = false;

        public bool aAmpoule = false;

        public bool pAmulet = false;

        public bool fBarrier = false;

        public bool aBrain = false;

        public bool lol = false;

        public bool raiderTalisman = false;

        public int raiderStack = 0;

        public bool sGenerator = false;

        public bool sDefense = false;

        public bool sPower = false;

        public bool sRegen = false;

        public bool IBoots = false;

        public bool elysianFire = false;

        public bool sTracers = false;

        public bool eTracers = false;

        public bool cTracers = false;

        public bool frostFlare = false;

        public bool beeResist = false;

        public bool uberBees = false;

        public bool projRef = false;

        public bool nanotech = false;

        public bool eQuiver = false;

        public bool shadowMinions = false;

        public bool tearMinions = false;

        public bool alchFlask = false;

        public bool community = false;

        public bool fleshTotem = false;

        public int fleshTotemCooldown = 0;

        public bool bloodPact = false;

        public bool bloodflareCore = false;

        public bool coreOfTheBloodGod = false;

        public bool elementalHeart = false;

        public bool crownJewel = false;

        public bool celestialJewel = false;

        public bool astralArcanum = false;

        public bool harpyRing = false;

        public bool ironBoots = false;

        public bool depthCharm = false;

        public bool anechoicPlating = false;

        public bool jellyfishNecklace = false;

        public bool abyssalAmulet = false;

        public bool lumenousAmulet = false;

        public bool reaperToothNecklace = false;

        public bool aquaticEmblem = false;

        public bool darkSunRing = false;

        public bool calamityRing = false;

        public bool eArtifact = false;

        public bool dArtifact = false;

        public bool gArtifact = false;

        public bool pArtifact = false;
        #endregion

        #region ArmorSetStuff
        public bool victideSet = false;

        public bool aeroSet = false;

        public bool statigelSet = false;
		
		public bool tarraSet = false;

        public bool tarraMelee = false;

        public bool tarraDefense = false;

        public int tarraCooldown = 0;

        public int tarraDefenseTime = 600;

        public bool tarraMage = false;

        public int tarraMageHealCooldown = 0;

        public int tarraCrits = 0;

        public bool tarraRanged = false;

        public bool tarraThrowing = false;

        public int tarraThrowingCrits = 0;

        public int tarraThrowingCritTimer = 0;

        public bool tarraSummon = false;
		
		public bool bloodflareSet = false;

        public bool bloodflareMelee = false;

        public int bloodflareMeleeHits = 0;

        public int bloodflareFrenzyTimer = 0;

        public int bloodflareFrenzyCooldown = 0;

        public bool bloodflareRanged = false;

        public int bloodflareRangedCooldown = 0;

        public bool bloodflareThrowing = false;

        public bool bloodflareMage = false;

        public int bloodflareMageCooldown = 0;

        public bool bloodflareSummon = false;

        public int bloodflareSummonTimer = 0;
		
		public bool godSlayer = false;
		
		public bool godSlayerDamage = false;

        public bool godSlayerMage = false;

        public bool godSlayerRanged = false;

        public bool godSlayerThrowing = false;

        public bool godSlayerSummon = false;

        public float godSlayerDmg;

        public bool godSlayerReflect = false;
		
		public bool godSlayerCooldown = false;
		
		public bool ataxiaBolt = false;
		
		public bool ataxiaFire = false;
		
		public bool ataxiaVolley = false;
		
		public bool ataxiaBlaze = false;
		
		public bool daedalusAbsorb = false;
		
		public bool daedalusShard = false;
		
		public bool reaverSpore = false;
		
		public bool reaverDoubleTap = false;

        public bool flamethrowerBoost = false;

        public bool shadeRegen = false;

        public bool shadowSpeed = false;

        public bool dsSetBonus = false;

        public bool auricBoost = false;

        public bool daedalusReflect = false;

        public bool daedalusSplit = false;

        public bool reaverBlast = false;

        public bool reaverBurst = false;

        public bool astralStarRain = false;

        public int astralStarRainCooldown = 0;

        public float ataxiaDmg;

        public bool ataxiaMage = false;

        public bool ataxiaGeyser = false;

        public float xerocDmg;

        public bool xerocSet = false;

        public bool silvaSet = false;

        public bool silvaMelee = false;

        public bool silvaRanged = false;

        public bool silvaThrowing = false;

        public bool silvaMage = false;

        public bool silvaSummon = false;

        public bool hasSilvaEffect = false;

        public int silvaCountdown = 600;

        public int silvaHitCounter = 0;

        public bool auricSet = false;

        public bool omegaBlueChestplate = false;

        public bool omegaBlueSet = false;

        public bool omegaBlueHentai = false;

        public int omegaBlueCooldown = 0;
        #endregion

        #region DebuffStuff
        public bool aCrunch = false;

        public bool NOU = false;
		
		public bool hAttack = false;
		
		public bool horror = false;
		
		public bool irradiated = false;
		
		public bool bFlames = false;
		
		public bool aFlames = false;
		
		public bool gsInferno = false;
		
		public bool pFlames = false;
		
		public bool hFlames = false;

        public bool hInferno = false;
		
		public bool gState = false;
		
		public bool bBlood = false;

        public bool eGravity = false;

        public bool weakPetrification = false;

        public bool vHex = false;

        public bool eGrav = false;

        public bool warped = false;

        public bool marked = false;

        public bool cDepth = false;

        public bool fishAlert = false;

        public bool bOut = false;
        #endregion

        #region BuffStuff
        public bool rRage = false;
		
		public bool tRegen = false;
		
		public bool xRage = false;
		
		public bool xWrath = false;
		
		public bool graxDefense = false;

        public bool sMeleeBoost = false;

        public bool tFury = false;

        public bool cadence = false;

        public bool omniscience = false;

        public bool zerg = false;

        public bool zen = false;

        public bool yPower = false;

        public bool aWeapon = false;

        public bool tScale = false;

        public bool fabsolVodka = false;

        public bool mushy = false;

        public bool molten = false;

        public bool shellBoost = false;

        public bool cFreeze = false;

        public bool invincible = false;

        public bool shine = false;

        public bool anechoicCoating = false;

        public bool enraged = false;

        public int revivifyTimer = 0;

        public bool permafrostsConcoction;

        public bool vodka = false;

        public bool redWine = false;

        public bool grapeBeer = false;

        public bool moonshine = false;

        public bool rum = false;

        public bool whiskey = false;

        public bool fireball = false;

        public bool everclear = false;

        public bool bloodyMary = false;

        public bool tequila = false;

        public bool caribbeanRum = false;

        public bool cinnamonRoll = false;

        public bool tequilaSunrise = false;

        public bool margarita = false;

        public bool starBeamRye = false;

        public bool screwdriver = false;

        public bool moscowMule = false;

        public bool whiteWine = false;

        public bool evergreenGin = false;
        #endregion

        #region SummonStuff
        public bool mWorm = false;

        public bool iClasper = false;

        public bool herring = false;

        public bool calamari = false;
		
		public bool cEyes = false;
		
		public bool cSlime = false;
		
		public bool cSlime2 = false;
		
		public bool bStar = false;

        public bool aStar = false;
		
		public bool SP = false;
		
		public bool dCreeper = false;
		
		public bool bClot = false;
		
		public bool eAxe = false;
		
		public bool SPG = false;
		
		public bool aChicken = false;
		
		public bool cLamp = false;
		
		public bool pGuy = false;

        public bool gDefense = false;

        public bool gOffense = false;

        public bool gHealer = false;

        public bool cEnergy = false;
		
		public bool sWaifu = false;
		
		public bool dWaifu = false;
		
		public bool cWaifu = false;
		
		public bool bWaifu = false;
		
		public bool slWaifu = false;
		
		public bool fClump = false;
		
		public bool rDevil = false;
		
		public bool aValkyrie = false;

        public bool sCrystal = false;
		
		public bool sGod = false;

        public bool sandnado = false;
		
		public bool vUrchin = false;
		
		public bool cSpirit = false;
		
		public bool rOrb = false;
		
		public bool dCrystal = false;
		
		public bool sandWaifu = false;
		
		public bool sandBoobWaifu = false;
		
		public bool cloudWaifu = false;
		
		public bool brimstoneWaifu = false;
		
		public bool sirenWaifu = false;

        public bool allWaifus = false;
		
		public bool fungalClump = false;
		
		public bool redDevil = false;
		
		public bool valkyrie = false;
		
		public bool slimeGod = false;
		
		public bool urchin = false;
		
		public bool chaosSpirit = false;
		
		public bool reaverOrb = false;
		
		public bool daedalusCrystal = false;

        public int healCounter = 300;
        #endregion

        #region BiomeStuff
        public bool ZoneCalamity = false;
		
		public bool ZoneAstral = false;

        public bool ZoneGreatSea = false;

        public bool ZoneSulphur = false;

        public bool ZoneAbyss = false;

        public bool ZoneAbyssLayer1 = false;

        public bool ZoneAbyssLayer2 = false;

        public bool ZoneAbyssLayer3 = false;

        public bool ZoneAbyssLayer4 = false;

        public int abyssBreathCD;
        #endregion

        #region TransformationStuff
        public bool abyssalDivingSuitPrevious;

        public bool abyssalDivingSuit;

        public bool abyssalDivingSuitHide;

        public bool abyssalDivingSuitForce;

        public bool abyssalDivingSuitPower;

        public bool sirenBoobsPrevious;

        public bool sirenBoobs;

        public bool sirenBoobsHide;

        public bool sirenBoobsForce;

        public bool sirenBoobsPower;

        public bool sirenBoobsAltPrevious;

        public bool sirenBoobsAlt;

        public bool sirenBoobsAltHide;

        public bool sirenBoobsAltForce;

        public bool sirenBoobsAltPower;

        public bool snowmanPrevious;

        public bool snowman;

        public bool snowmanHide;

        public bool snowmanForce;

        public bool snowmanNoseless;

        public bool snowmanPower;
        #endregion
        #endregion

        #region SavingAndLoading
        public override void Initialize()
		{
			extraAccessoryML = false;
			eCore = false;
			mFruit = false;
			bOrange = false;
			eBerry = false;
			dFruit = false;
			pHeart = false;
			cShard = false;
            revJamDrop = false;
            rageBoostOne = false;
            rageBoostTwo = false;
            rageBoostThree = false;
            adrenalineBoostOne = false;
            adrenalineBoostTwo = false;
            adrenalineBoostThree = false;
            drawBossHPBar = true;
            shouldDrawSmallText = true;
		}
		
		public override TagCompound Save()
		{
			var boost = new List<string>();
			if (extraAccessoryML) boost.Add("extraAccessoryML");
			if (eCore) boost.Add("etherealCore");
			if (mFruit) boost.Add("miracleFruit");
			if (bOrange) boost.Add("bloodOrange");
			if (eBerry) boost.Add("elderBerry");
			if (dFruit) boost.Add("dragonFruit");
			if (pHeart) boost.Add("phantomHeart");
			if (cShard) boost.Add("cometShard");
            if (revJamDrop) boost.Add("revJam");
            if (rageBoostOne) boost.Add("rageOne");
            if (rageBoostTwo) boost.Add("rageTwo");
            if (rageBoostThree) boost.Add("rageThree");
            if (adrenalineBoostOne) boost.Add("adrenalineOne");
            if (adrenalineBoostTwo) boost.Add("adrenalineTwo");
            if (adrenalineBoostThree) boost.Add("adrenalineThree");
            if (drawBossHPBar) boost.Add("bossHPBar");
            if (shouldDrawSmallText) boost.Add("drawSmallText");

            return new TagCompound
			{
				{
					"boost", boost
				},
				{
					"stress", stress
				},
                {
                    "adrenaline", adrenaline
                },
                {
                    "sCalDeathCount", sCalDeathCount
                },
                {
                    "sCalKillCount", sCalKillCount
                },
                {
                    "meleeLevel", meleeLevel
                },
                {
                    "exactMeleeLevel", exactMeleeLevel
                },
                {
                    "rangedLevel", rangedLevel
                },
                {
                    "exactRangedLevel", exactRangedLevel
                },
                {
                    "magicLevel", magicLevel
                },
                {
                    "exactMagicLevel", exactMagicLevel
                },
                {
                    "summonLevel", summonLevel
                },
                {
                    "exactSummonLevel", exactSummonLevel
                },
                {
                    "rogueLevel", rogueLevel
                },
                {
                    "exactRogueLevel", exactRogueLevel
                }
			};
		}

		public override void Load(TagCompound tag)
		{
			var boost = tag.GetList<string>("boost");
			extraAccessoryML = boost.Contains("extraAccessoryML");
			eCore = boost.Contains("etherealCore");
			mFruit = boost.Contains("miracleFruit");
			bOrange = boost.Contains("bloodOrange");
			eBerry = boost.Contains("elderBerry");
			dFruit = boost.Contains("dragonFruit");
			pHeart = boost.Contains("phantomHeart");
			cShard = boost.Contains("cometShard");
            revJamDrop = boost.Contains("revJam");
            rageBoostOne = boost.Contains("rageOne");
            rageBoostTwo = boost.Contains("rageTwo");
            rageBoostThree = boost.Contains("rageThree");
            adrenalineBoostOne = boost.Contains("adrenalineOne");
            adrenalineBoostTwo = boost.Contains("adrenalineTwo");
            adrenalineBoostThree = boost.Contains("adrenalineThree");
            drawBossHPBar = boost.Contains("bossHPBar");
            shouldDrawSmallText = boost.Contains("drawSmallText");

            stress = tag.GetInt("stress");
            adrenaline = tag.GetInt("adrenaline");
            sCalDeathCount = tag.GetInt("sCalDeathCount");
            sCalKillCount = tag.GetInt("sCalKillCount");

            meleeLevel = tag.GetInt("meleeLevel");
            rangedLevel = tag.GetInt("rangedLevel");
            magicLevel = tag.GetInt("magicLevel");
            summonLevel = tag.GetInt("summonLevel");
            rogueLevel = tag.GetInt("rogueLevel");
            exactMeleeLevel = tag.GetInt("exactMeleeLevel");
            exactRangedLevel = tag.GetInt("exactRangedLevel");
            exactMagicLevel = tag.GetInt("exactMagicLevel");
            exactSummonLevel = tag.GetInt("exactSummonLevel");
            exactRogueLevel = tag.GetInt("exactRogueLevel");
        }

		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			stress = reader.ReadInt32();
            adrenaline = reader.ReadInt32();
            sCalDeathCount = reader.ReadInt32();
            sCalKillCount = reader.ReadInt32();

            meleeLevel = reader.ReadInt32();
            rangedLevel = reader.ReadInt32();
            magicLevel = reader.ReadInt32();
            summonLevel = reader.ReadInt32();
            rogueLevel = reader.ReadInt32();
            exactMeleeLevel = reader.ReadInt32();
            exactRangedLevel = reader.ReadInt32();
            exactMagicLevel = reader.ReadInt32();
            exactSummonLevel = reader.ReadInt32();
            exactRogueLevel = reader.ReadInt32();

            if (loadVersion == 0)
			{
				BitsByte flags = reader.ReadByte();
				extraAccessoryML = flags[0];
				eCore = flags[1];
				mFruit = flags[2];
				bOrange = flags[3];
				eBerry = flags[4];
				dFruit = flags[5];
				pHeart = flags[6];
				cShard = flags[7];

                BitsByte flags2 = reader.ReadByte();
                revJamDrop = flags2[0];
                rageBoostOne = flags2[1];
                rageBoostTwo = flags2[2];
                rageBoostThree = flags2[3];
                adrenalineBoostOne = flags2[4];
                adrenalineBoostTwo = flags2[5];
                adrenalineBoostThree = flags2[6];
                drawBossHPBar = flags2[7];

                BitsByte flags3 = reader.ReadByte();
                shouldDrawSmallText = flags3[0];
            }
			else
			{
				ErrorLogger.Log("CalamityMod: Unknown loadVersion: " + loadVersion);
			}
		}
        #endregion

        #region ResetEffects
        public override void ResetEffects()
		{
			if (extraAccessoryML && player.extraAccessory && (Main.expertMode || Main.gameMenu))
			{
				player.extraAccessorySlots = 2;
			}

			runCheck = 0;
			dashMod = 0;
            alcoholPoisonLevel = 0;

            leviPet = false;
            sirenPet = false;
            fox = false;
            chibii = false;
            brimling = false;
            bearPet = false;
            fab = false;

            abyssalDivingSuitPlates = false;
            abyssalDivingSuitCooldown = false;

            sirenWaterBuff = false;
            sirenIce = false;
            sirenIceCooldown = false;

            draedonsHeart = false;
            draedonsStressGain = false;

			afflicted = false;
			affliction = false;
			afflictedBuff = false;

            fasterMeleeLevel = false;
            fasterRangedLevel = false;
            fasterMagicLevel = false;
            fasterSummonLevel = false;
            fasterRogueLevel = false;

			dodgeScarf = false;
            scarfCooldown = false;

            elysianAegis = false;

			nCore = false;

            godSlayer = false;
            godSlayerDamage = false;
            godSlayerMage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;
            godSlayerSummon = false;
			godSlayerReflect = false;
			godSlayerCooldown = false;

			silvaSet = false;
            silvaMelee = false;
            silvaRanged = false;
            silvaThrowing = false;
            silvaMage = false;
            silvaSummon = false;

			auricSet = false;
            auricBoost = false;

            omegaBlueChestplate = false;
            omegaBlueSet = false;
            omegaBlueHentai = false;

            ataxiaBolt = false;
			ataxiaGeyser = false;
            ataxiaFire = false;
            ataxiaVolley = false;
            ataxiaBlaze = false;
            ataxiaMage = false;

            shadeRegen = false;
			
			flamethrowerBoost = false;

			shadowSpeed = false;
            dsSetBonus = false;

            badgeOfBravery = false;
            aSpark = false;
			aBulwark = false;
			dAmulet = false;
			fCarapace = false;
			gShell = false;
			absorber = false;
			aAmpoule = false;
			pAmulet = false;
			fBarrier = false;
			aBrain = false;
			frostFlare = false;
			beeResist = false;
			uberBees = false;
			projRef = false;
			nanotech = false;
			eQuiver = false;
            cryogenSoul = false;
            yInsignia = false;
            eGauntlet = false;
            eTalisman = false;
            statisBeltOfCurses = false;
            heartOfDarkness = false;
            shadowMinions = false;
            tearMinions = false;
            alchFlask = false;
            community = false;
            stressPills = false;
            laudanum = false;
            fleshTotem = false;
            bloodPact = false;
            bloodflareCore = false;
            coreOfTheBloodGod = false;
            elementalHeart = false;
            crownJewel = false;
            celestialJewel = false;
            astralArcanum = false;
            harpyRing = false;
            darkSunRing = false;
            calamityRing = false;
            eArtifact = false;
            dArtifact = false;
            gArtifact = false;
            pArtifact = false;
            lol = false;
            raiderTalisman = false;
            sGenerator = false;
            sDefense = false;
            sRegen = false;
            sPower = false;
            IBoots = false;
            elysianFire = false;
            sTracers = false;
            eTracers = false;
            cTracers = false;

            daedalusReflect = false;
			daedalusSplit = false;
			daedalusAbsorb = false;
			daedalusShard = false;

			reaverSpore = false;
			reaverDoubleTap = false;
            reaverBlast = false;
            reaverBurst = false;

            ironBoots = false;
            depthCharm = false;
            anechoicPlating = false;
            jellyfishNecklace = false;
            abyssalAmulet = false;
            lumenousAmulet = false;
            reaperToothNecklace = false;
            aquaticEmblem = false;

			astralStarRain = false;
			
            victideSet = false;

            aeroSet = false;

            statigelSet = false;

			tarraSet = false;
            tarraMelee = false;
            tarraMage = false;
            tarraRanged = false;
            tarraThrowing = false;
            tarraSummon = false;

			bloodflareSet = false;
            bloodflareMelee = false;
            bloodflareRanged = false;
            bloodflareThrowing = false;
            bloodflareMage = false;
            bloodflareSummon = false;

			xerocSet = false;

            NOU = false;
            weakPetrification = false;

            aCrunch = false;
			hAttack = false;
			horror = false;
			irradiated = false;
			bFlames = false;
			aFlames = false;
			gsInferno = false;
			pFlames = false;
			hFlames = false;
            hInferno = false;
			gState = false;
			bBlood = false;
            eGravity = false;
            vHex = false;
            eGrav = false;
            warped = false;
            marked = false;
            cDepth = false;
            fishAlert = false;
            bOut = false;
            enraged = false;
            snowmanNoseless = false;

            rRage = false;
			xRage = false;
			xWrath = false;
			graxDefense = false;
			sMeleeBoost = false;
			tFury = false;
			cadence = false;
			omniscience = false;
			zerg = false;
            zen = false;
            permafrostsConcoction = false;
            vodka = false;
            redWine = false;
            grapeBeer = false;
            moonshine = false;
            rum = false;
            whiskey = false;
            fireball = false;
            everclear = false;
            bloodyMary = false;
            tequila = false;
            caribbeanRum = false;
            cinnamonRoll = false;
            tequilaSunrise = false;
            margarita = false;
            starBeamRye = false;
            screwdriver = false;
            moscowMule = false;
            whiteWine = false;
            evergreenGin = false;
            yPower = false;
			aWeapon = false;
			tScale = false;
			fabsolVodka = false;
            invincible = false;
            shine = false;
            anechoicCoating = false;
			mushy = false;
			molten = false;
			shellBoost = false;
			cFreeze = false;
			tRegen = false;

			mWorm = false;
            iClasper = false;
            herring = false;
            calamari = false;
			cEyes = false;
			cSlime = false;
			cSlime2 = false;
			bStar = false;
            aStar = false;
			SP = false;
			dCreeper = false;
			bClot = false;
			eAxe = false;
			SPG = false;
			aChicken = false;
			cLamp = false;
			pGuy = false;
            cEnergy = false;
            gDefense = false;
            gOffense = false;
            gHealer = false;
			sWaifu = false;
			dWaifu = false;
			cWaifu = false;
			bWaifu = false;
			slWaifu = false;
			fClump = false;
			rDevil = false;
			aValkyrie = false;
            sCrystal = false;
			sGod = false;
            sandnado = false;
			vUrchin = false;
			cSpirit = false;
			rOrb = false;
			dCrystal = false;
			sandWaifu = false;
			sandBoobWaifu = false;
			cloudWaifu = false;
			brimstoneWaifu = false;
			sirenWaifu = false;
            allWaifus = false;
			fungalClump = false;
			redDevil = false;
			valkyrie = false;
			slimeGod = false;
			urchin = false;
			chaosSpirit = false;
			reaverOrb = false;
			daedalusCrystal = false;

            abyssalDivingSuitPrevious = abyssalDivingSuit;
            abyssalDivingSuit = abyssalDivingSuitHide = abyssalDivingSuitForce = abyssalDivingSuitPower = false;

            sirenBoobsPrevious = sirenBoobs;
            sirenBoobs = sirenBoobsHide = sirenBoobsForce = sirenBoobsPower = false;
            sirenBoobsAltPrevious = sirenBoobsAlt;
            sirenBoobsAlt = sirenBoobsAltHide = sirenBoobsAltForce = sirenBoobsAltPower = false;

            snowmanPrevious = snowman;
            snowman = snowmanHide = snowmanForce = snowmanPower = false;

            rageMode = false;
            adrenalineMode = false;
        }
        #endregion

        #region UpdateDead
        public override void UpdateDead()
		{
            #region Debuffs
            stress = 0;
            adrenaline = 0;
            adrenalineMaxTimer = 180;
            adrenalineDmgDown = 600;
            adrenalineDmgMult = 1f;
			raiderStack = 0;
            fleshTotemCooldown = 0;
            astralStarRainCooldown = 0;
            bloodflareMageCooldown = 0;
            tarraMageHealCooldown = 0;
            aCrunch = false;
			hAttack = false;
			horror = false;
			irradiated = false;
			bFlames = false;
			aFlames = false;
			gsInferno = false;
			pFlames = false;
			hFlames = false;
            hInferno = false;
			gState = false;
			bBlood = false;
			eGravity = false;
			vHex = false;
			eGrav = false;
			warped = false;
            marked = false;
            cDepth = false;
            fishAlert = false;
            bOut = false;
            snowmanNoseless = false;
            scarfCooldown = false;
			godSlayerCooldown = false;
            abyssalDivingSuitCooldown = false;
            abyssalDivingSuitPlateHits = 0;
            sirenIceCooldown = false;
            #endregion

            #region Buffs
            sDefense = false;
            sRegen = false;
            sPower = false;
            fab = false;
            abyssalDivingSuitPlates = false;
            sirenWaterBuff = false;
            sirenIce = false;
			afflictedBuff = false;
			rRage = false;
			xRage = false;
			xWrath = false;
			graxDefense = false;
			sMeleeBoost = false;
			tFury = false;
			cadence = false;
			omniscience = false;
			zerg = false;
            zen = false;
			yPower = false;
			aWeapon = false;
			tScale = false;
			fabsolVodka = false;
            invincible = false;
            shine = false;
            anechoicCoating = false;
			mushy = false;
			molten = false;
            enraged = false;
			shellBoost = false;
			cFreeze = false;
			tRegen = false;
            rageMode = false;
            adrenalineMode = false;
            vodka = false;
            redWine = false;
            grapeBeer = false;
            moonshine = false;
            rum = false;
            whiskey = false;
            fireball = false;
            everclear = false;
            bloodyMary = false;
            tequila = false;
            caribbeanRum = false;
            cinnamonRoll = false;
            tequilaSunrise = false;
            margarita = false;
            starBeamRye = false;
            screwdriver = false;
            moscowMule = false;
            whiteWine = false;
            evergreenGin = false;
            revivifyTimer = 0;
            healCounter = 300;
			#endregion
			
			#region Armorbonuses
			flamethrowerBoost = false;
			shadowSpeed = false;
			godSlayer = false;
			godSlayerDamage = false;
            godSlayerMage = false;
            godSlayerRanged = false;
            godSlayerThrowing = false;
            godSlayerSummon = false;
			godSlayerReflect = false;
			auricBoost = false;
			silvaSet = false;
            silvaMelee = false;
            silvaRanged = false;
            silvaThrowing = false;
            silvaMage = false;
            silvaSummon = false;
            hasSilvaEffect = false;
            silvaCountdown = 600;
            silvaHitCounter = 0;
			auricSet = false;
            omegaBlueChestplate = false;
            omegaBlueSet = false;
            omegaBlueCooldown = 0;
            daedalusReflect = false;
			daedalusSplit = false;
			daedalusAbsorb = false;
			daedalusShard = false;
			reaverSpore = false;
			reaverDoubleTap = false;
			shadeRegen = false;
			dsSetBonus = false;
			reaverBlast = false;
			reaverBurst = false;
			astralStarRain = false;
			ataxiaMage = false;
			ataxiaBolt = false;
			ataxiaGeyser = false;
			ataxiaFire = false;
			ataxiaVolley = false;
			ataxiaBlaze = false;
            victideSet = false;
            aeroSet = false;
            statigelSet = false;
			tarraSet = false;
            tarraMelee = false;
            tarraMage = false;
            tarraRanged = false;
            tarraThrowing = false;
            tarraThrowingCrits = 0;
            tarraThrowingCritTimer = 0;
            tarraSummon = false;
			bloodflareSet = false;
            bloodflareMelee = false;
            bloodflareMeleeHits = 0;
            bloodflareFrenzyTimer = 0;
            bloodflareFrenzyCooldown = 0;
            bloodflareRanged = false;
            bloodflareRangedCooldown = 0;
            bloodflareThrowing = false;
            bloodflareMage = false;
            bloodflareSummon = false;
            bloodflareSummonTimer = 0;
			xerocSet = false;
			IBoots = false;
			elysianFire = false;
			elysianAegis = false;
			elysianGuard = false;
            #endregion

            if (CalamityWorld.bossRushActive)
            {
                if (!CalamityGlobalNPC.AnyLivingPlayers())
                {
                    CalamityWorld.bossRushActive = false;
                    CalamityWorld.bossRushStage = 0;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(CalamityWorld.bossRushStage);
                        netMessage.Send();
                    }
                    for (int doom = 0; doom < 200; doom++)
                    {
                        if (Main.npc[doom].active && Main.npc[doom].boss)
                        {
                            Main.npc[doom].active = false;
                            Main.npc[doom].netUpdate = true;
                        }
                    }
                }
            }
            if (CalamityWorld.armageddon && !areThereAnyDamnBosses)
            {
                player.respawnTimer -= 5;
            }
            else if (player.respawnTimer > 300 && Main.expertMode) //600 normal 900 expert
            {
                player.respawnTimer--;
            }
        }
        #endregion

        #region BiomeStuff
        public override void UpdateBiomeVisuals()
		{
            Point point = player.Center.ToTileCoordinates();
            bool aboveGround = point.Y > Main.maxTilesY - 320;
            bool overworld = player.ZoneOverworldHeight && (point.X < 380 || point.X > Main.maxTilesX - 380);
            bool useNebula = NPC.AnyNPCs(mod.NPCType("DevourerofGodsHead"));
			player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHead", useNebula);
            bool useNebulaS = NPC.AnyNPCs(mod.NPCType("DevourerofGodsHeadS"));
            player.ManageSpecialBiomeVisuals("CalamityMod:DevourerofGodsHeadS", useNebulaS);
            bool useBrimstone = NPC.AnyNPCs(mod.NPCType("CalamitasRun3"));
			player.ManageSpecialBiomeVisuals("CalamityMod:CalamitasRun3", useBrimstone);
			bool usePlague = NPC.AnyNPCs(mod.NPCType("PlaguebringerGoliath"));
			player.ManageSpecialBiomeVisuals("CalamityMod:PlaguebringerGoliath", usePlague);
			bool useFire = NPC.AnyNPCs(mod.NPCType("Yharon"));
			player.ManageSpecialBiomeVisuals("CalamityMod:Yharon", useFire);
            player.ManageSpecialBiomeVisuals("HeatDistortion", Main.UseHeatDistortion && (useFire || 
                (aboveGround || ((double)point.Y < Main.worldSurface && player.ZoneDesert && !overworld && !Main.raining && !Filters.Scene["Sandstorm"].IsActive()))));
            bool useWater = NPC.AnyNPCs(mod.NPCType("Leviathan"));
			player.ManageSpecialBiomeVisuals("CalamityMod:Leviathan", useWater);
			bool useHoly = NPC.AnyNPCs(mod.NPCType("Providence"));
			player.ManageSpecialBiomeVisuals("CalamityMod:Providence", useHoly);
			bool useSBrimstone = NPC.AnyNPCs(mod.NPCType("SupremeCalamitas"));
			player.ManageSpecialBiomeVisuals("CalamityMod:SupremeCalamitas", useSBrimstone);
            bool inAstral = ZoneAstral;
            player.ManageSpecialBiomeVisuals("CalamityMod:Astral", inAstral);
        }
		
		public override void UpdateBiomes()
        {
            Point point = player.Center.ToTileCoordinates();
            ZoneCalamity = CalamityWorld.calamityTiles > 50;
            ZoneAstral = CalamityWorld.astralTiles > 950 || (player.ZoneSnow && CalamityWorld.astralTiles > 300);
            //ZoneGreatSea = CalamityWorld.seaTiles > 30;

            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int abyssChasmSteps = y / 4;
            int abyssChasmY = ((y - abyssChasmSteps) + (int)((double)y * 0.055)); //132 = 1932 large
            if (y < 1500)
            {
                abyssChasmY = ((y - abyssChasmSteps) + (int)((double)y * 0.095)); //114 = 1014 small
            }
            else if (y < 2100)
            {
                abyssChasmY = ((y - abyssChasmSteps) + (int)((double)y * 0.0735)); //132 = 1482 medium
            }
            int abyssChasmX = (CalamityWorld.abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135));

            bool abyssPosX = false;
            bool sulphurPosX = false;
            bool abyssPosY = (point.Y <= abyssChasmY);
            if (CalamityWorld.abyssSide)
            {
                if (point.X < 380)
                {
                    sulphurPosX = true;
                }
                if (point.X < abyssChasmX + 80)
                {
                    abyssPosX = true;
                }
            }
            else
            {
                if (point.X > Main.maxTilesX - 380)
                {
                    sulphurPosX = true;
                }
                if (point.X > abyssChasmX - 80)
                {
                    abyssPosX = true;
                }
            }

            //0.4 is the lowest location the rocklayer can be, 0.3 is the highest
            ZoneAbyss = (((double)point.Y > ((Main.rockLayer - (double)y * 0.05) /*+ 35*/)) && //rocklayer = (0.4 to 0.3) - 0.04 = 0.36 to 0.26
                !player.lavaWet && 
                !player.honeyWet &&
                abyssPosY && 
                abyssPosX);

            ZoneAbyssLayer1 = ZoneAbyss &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.03); //(0.36 to 0.26) to (0.436 to 0.336)

            ZoneAbyssLayer2 = ZoneAbyss && 
                (double)point.Y > (Main.rockLayer + (double)y * 0.03) &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.13); //(0.436 to 0.336) to (0.543 to 0.443)

            ZoneAbyssLayer3 = ZoneAbyss && 
                (double)point.Y > (Main.rockLayer + (double)y * 0.13) && 
                (double)point.Y <= (Main.rockLayer + (double)y * 0.23); //(0.543 to 0.443) to (0.662 to 0.562)

            ZoneAbyssLayer4 = ZoneAbyss && 
                point.Y <= Main.maxTilesY - 200 && 
                (double)point.Y > (Main.rockLayer + (double)y * 0.23); //(0.662 to 0.562) to just above underworld

            ZoneSulphur = (CalamityWorld.sulphurTiles > 30 || (player.ZoneOverworldHeight && sulphurPosX)) && !ZoneAbyss;
        }
		
		public override bool CustomBiomesMatch(Player other)
		{
			CalamityPlayer modOther = other.GetModPlayer<CalamityPlayer>(mod);
			return ZoneCalamity == modOther.ZoneCalamity || ZoneAstral == modOther.ZoneAstral || ZoneAbyss == modOther.ZoneAbyss || 
                ZoneAbyssLayer1 == modOther.ZoneAbyssLayer1 || ZoneAbyssLayer2 == modOther.ZoneAbyssLayer2 || 
                ZoneAbyssLayer3 == modOther.ZoneAbyssLayer3 || ZoneAbyssLayer4 == modOther.ZoneAbyssLayer4 || 
                ZoneSulphur == modOther.ZoneSulphur;
		}

		public override void CopyCustomBiomesTo(Player other)
		{
			CalamityPlayer modOther = other.GetModPlayer<CalamityPlayer>(mod);
			modOther.ZoneCalamity = ZoneCalamity;
			modOther.ZoneAstral = ZoneAstral;
            modOther.ZoneSulphur = ZoneSulphur;
            modOther.ZoneAbyss = ZoneAbyss;
            modOther.ZoneAbyssLayer1 = ZoneAbyssLayer1;
            modOther.ZoneAbyssLayer2 = ZoneAbyssLayer2;
            modOther.ZoneAbyssLayer3 = ZoneAbyssLayer3;
            modOther.ZoneAbyssLayer4 = ZoneAbyssLayer4;
        }

		public override void SendCustomBiomes(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = ZoneCalamity;
			flags[1] = ZoneAstral;
            flags[2] = ZoneAbyss;
            flags[3] = ZoneAbyssLayer1;
            flags[4] = ZoneAbyssLayer2;
            flags[5] = ZoneAbyssLayer3;
            flags[6] = ZoneAbyssLayer4;
            flags[7] = ZoneSulphur;
			writer.Write(flags);
		}

		public override void ReceiveCustomBiomes(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			ZoneCalamity = flags[0];
			ZoneAstral = flags[1];
            ZoneAbyss = flags[2];
            ZoneAbyssLayer1 = flags[3];
            ZoneAbyssLayer2 = flags[4];
            ZoneAbyssLayer3 = flags[5];
            ZoneAbyssLayer4 = flags[6];
            ZoneSulphur = flags[7];
        }

        public override Texture2D GetMapBackgroundImage()
        {
            if (ZoneSulphur)
            {
                return mod.GetTexture("Backgrounds/MapBackgrounds/SulphurBG");
            }
            if (ZoneAstral)
            {
                return mod.GetTexture("Backgrounds/MapBackgrounds/AstralBG");
            }
            return null;
        }
        #endregion
        
        #region InventoryStartup
        public override void SetupStartInventory(IList<Item> items, bool mediumcoreDeath)
		{
            if (!mediumcoreDeath)
            {
                player.inventory[9].SetDefaults(mod.ItemType("Revenge"));
                player.inventory[8].SetDefaults(mod.ItemType("IronHeart"));
                player.inventory[7].SetDefaults(mod.ItemType("StarterBag"));
            }
        }
        #endregion

        #region LifeRegen
        public override void UpdateBadLifeRegen()
		{
            #region FirstDebuffs
            if (bFlames || aFlames)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 16;
            }
            if (gsInferno || (ZoneCalamity && player.lavaWet))
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 30;
            }
            if (ZoneSulphur && Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
            {
                player.AddBuff(BuffID.Poisoned, 2, true);
                pissWaterBoost++;
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                if (pissWaterBoost > 1800)
                {
                    player.lifeRegen -= 6;
                }
                else if (pissWaterBoost > 1440)
                {
                    player.lifeRegen -= 4;
                }
                else if (pissWaterBoost > 1080)
                {
                    player.lifeRegen -= 3;
                }
                else if (pissWaterBoost > 720)
                {
                    player.lifeRegen -= 2;
                }
                else if (pissWaterBoost > 360)
                {
                    player.lifeRegen -= 1;
                }
            }
            else
            {
                pissWaterBoost = 0;
            }
            if (hFlames)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 20;
            }
            if (pFlames)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 20;
                player.blind = true;
                player.statDefense -= 8;
                player.moveSpeed -= 0.15f;
            }
            if (bBlood)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 8;
                player.blind = true;
                player.statDefense -= 3;
                player.moveSpeed += 0.2f;
                player.meleeSpeed -= 0.025f;
                player.meleeDamage += 0.05f;
                player.rangedDamage -= 0.1f;
                player.magicDamage -= 0.1f;
            }
            if (horror)
            {
                player.blind = true;
                player.statDefense -= 15;
                player.moveSpeed -= 0.15f;
            }
            if (aCrunch)
            {
                player.statDefense /= 3;
                player.endurance *= 0.33f;
            }
            if (vHex)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 16;
                player.blind = true;
                player.statDefense -= 30;
                player.moveSpeed -= 0.1f;
                player.endurance -= 0.3f;
                if (player.wingTimeMax <= 0)
                {
                    player.wingTimeMax = 0;
                }
                player.wingTimeMax /= 2;
            }
            if (cDepth)
            {
                if (player.statDefense > 0)
                {
                    int depthDamage = depthCharm ? 9 : 18;
                    int subtractDefense = (int)((double)player.statDefense * 0.05); //240 defense = 0 damage taken with depth charm
                    int calcDepthDamage = depthDamage - subtractDefense;
                    if (calcDepthDamage < 0)
                    {
                        calcDepthDamage = 0;
                    }
                    if (player.lifeRegen > 0)
                    {
                        player.lifeRegen = 0;
                    }
                    player.lifeRegenTime = 0;
                    player.lifeRegen -= calcDepthDamage;
                }
            }
            #endregion
            #region Buffs
			if (tRegen)
			{
				player.lifeRegen += 10;
			}
            if (sRegen)
            {
                player.lifeRegen += 2;
            }
            if (tarraSet)
			{
				player.calmed = (tarraMelee ? false : true);
				player.lifeMagnet = true;
			}
			if (aChicken)
			{
				player.lifeRegen += 1;
				player.statDefense += 5;
				player.moveSpeed += 0.1f;
			}
			if (cadence)
			{
				player.discount = true;
				player.lifeMagnet = true;
				player.calmed = true;
				player.loveStruck = true;
				player.lifeRegen += 2;
				player.statLifeMax2 += player.statLifeMax / 5 / 20 * 10;
			}
			if (omniscience)
			{
				player.detectCreature = true;
				player.dangerSense = true;
				player.findTreasure = true;
			}
			if (aWeapon)
			{
				player.moveSpeed += 0.15f;
			}
			if (mushy)
			{
				player.statDefense += 5;
				player.lifeRegen += 5;
			}
			if (molten)
			{
				player.resistCold = true;
			}
			if (shellBoost)
			{
				player.moveSpeed += 0.9f;
			}
            if (celestialJewel || astralArcanum)
            {
                bool lesserEffect = false;
                for (int l = 0; l < 22; l++)
                {
                    int hasBuff = player.buffType[l];
                    bool shouldAffect = CalamityMod.alcoholList.Contains(hasBuff);
                    if (shouldAffect)
                    {
                        lesserEffect = true;
                    }
                }
                if (lesserEffect)
                {
                    player.lifeRegen += 1;
                    player.statDefense += 20;
                }
                else
                {
                    if (player.lifeRegen < 0)
                    {
                        if (player.lifeRegenTime < 1800)
                        {
                            player.lifeRegenTime = 1800;
                        }
                        player.lifeRegen += 4;
                        player.statDefense += 20;
                    }
                    else
                    {
                        player.lifeRegen += 2;
                    }
                }
            }
            else if (crownJewel)
            {
                bool lesserEffect = false;
                for (int l = 0; l < 22; l++)
                {
                    int hasBuff = player.buffType[l];
                    bool shouldAffect = CalamityMod.alcoholList.Contains(hasBuff);
                    if (shouldAffect)
                    {
                        lesserEffect = true;
                    }
                }
                if (lesserEffect)
                {
                    player.statDefense += 10;
                }
                else
                {
                    if (player.lifeRegen < 0)
                    {
                        if (player.lifeRegenTime < 1800)
                        {
                            player.lifeRegenTime = 1800;
                        }
                        player.lifeRegen += 2;
                        player.statDefense += 10;
                    }
                    else
                    {
                        player.lifeRegen += 1;
                    }
                }
            }
            if (permafrostsConcoction)
            {
                if (player.statLife < player.statLifeMax2 / 2)
                    player.lifeRegen++;
                if (player.statLife < player.statLifeMax2 / 4)
                    player.lifeRegen++;
                if (player.statLife < player.statLifeMax2 / 10)
                    player.lifeRegen += 2;
                if (player.poisoned || player.onFire || bFlames)
                    player.lifeRegen += 4;
            }
            #endregion
            #region LastDebuffs
            if (omegaBlueChestplate)
            {
                if (player.lifeRegen > 0)
                    player.lifeRegen = 0;

                player.lifeRegenTime = 0;

                if (player.lifeRegenCount > 0)
                    player.lifeRegenCount = 0;
            }
            if (hInferno)
            {
                hInfernoBoost++;
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                if (hInfernoBoost > 240)
                {
                    player.lifeRegen -= 192;
                }
                else if (hInfernoBoost > 180)
                {
                    player.lifeRegen -= 96;
                }
                else if (hInfernoBoost > 120)
                {
                    player.lifeRegen -= 48;
                }
                else if (hInfernoBoost > 60)
                {
                    player.lifeRegen -= 24;
                }
                else if (hInfernoBoost > 0)
                {
                    player.lifeRegen -= 12;
                }
            }
            else
            {
                hInfernoBoost = 0;
            }
            if (gState)
            {
                player.statDefense /= 2;
                player.velocity.Y = 0f;
                player.velocity.X = 0f;
            }
            if (eGravity)
            {
                player.velocity.X *= 0.99f;
                if (player.wingTimeMax < 0)
                {
                    player.wingTimeMax = 0;
                }
                player.wingTimeMax /= 4;
                if (player.wingTimeMax > 400)
                {
                    player.wingTimeMax = 100;
                }
            }
            if (eGrav)
            {
                if (player.wingTimeMax < 0)
                {
                    player.wingTimeMax = 0;
                }
                player.wingTimeMax /= 3;
                if (player.wingTimeMax > 400)
                {
                    player.wingTimeMax = 200;
                }
            }
            if (warped || caribbeanRum)
            {
                player.velocity.Y *= 1.01f;
            }
            if (weakPetrification || CalamityWorld.bossRushActive)
            {
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
            }
            if (CalamityGlobalNPC.DoGHead > -1)
            {
                player.velocity.X *= 0.99f;
            }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                {
                    player.lifeRegen = 0;
                }
            }
            #endregion
        }

        #region UpdateLifeRegen
        public override void UpdateLifeRegen()
		{
			bool shinyStoned = player.shinyStone;
			float num2 = 0f;
            if (player.lifeRegenTime >= 300)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 600)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 900)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 1200)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 1500)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 1800)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 2400)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 3000)
            {
                num2 += 1f;
            }
            if (player.lifeRegenTime >= 3600)
            {
                num2 += 1f;
            }
            if (player.velocity.X == 0f || player.grappling[0] > 0)
            {
                num2 *= 1.25f;
            }
            else
            {
                num2 *= 0.5f;
            }
            if (!shinyStoned)
            {
                if (draedonsHeart && !shadeRegen && !cFreeze && 
                    (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    if (!areThereAnyDamnBosses)
                    {
                        if (player.lifeRegen < 0)
                        {
                            player.lifeRegen /= 2;
                        }
                        if (player.lifeRegenTime > 90 && player.lifeRegenTime < 1800)
                        {
                            player.lifeRegenTime = 1800;
                        }
                        player.lifeRegenTime += 4;
                        player.lifeRegen += 4;
                        float num3 = (float)(player.lifeRegenTime - 3000);
                        num3 /= 300f;
                        if (num3 > 0f)
                        {
                            if (num3 > 30f)
                            {
                                num3 = 30f;
                            }
                            num2 += num3;
                        }
                    }
                    else
                    {
                        if (player.lifeRegenTime > 90 && player.lifeRegenTime < 360)
                        {
                            player.lifeRegenTime = 360;
                        }
                        player.lifeRegenTime += 2;
                        player.lifeRegen += 2;
                        float num3 = (float)(player.lifeRegenTime - 1800);
                        num3 /= 180f;
                        if (num3 > 0f)
                        {
                            if (num3 > 5f)
                            {
                                num3 = 5f;
                            }
                            num2 += num3;
                        }
                    }
                    float num4 = (float)player.statLifeMax2 / 400f * 0.85f + 0.15f;
                    num2 *= num4;
                    player.lifeRegen += (int)Math.Round((double)num2);
                    player.lifeRegenCount += player.lifeRegen;
                    if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
                    {
                        player.lifeRegenCount++;
                        if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.Next(2) == 0))
                        {
                            int num5 = Dust.NewDust(player.position, player.width, player.height, 107, 0f, 0f, 200, default(Color), 1f);
                            Main.dust[num5].noGravity = true;
                            Main.dust[num5].velocity *= 0.75f;
                            Main.dust[num5].fadeIn = 1.3f;
                            Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            vector.Normalize();
                            vector *= (float)Main.rand.Next(50, 100) * 0.04f;
                            Main.dust[num5].velocity = vector;
                            vector.Normalize();
                            vector *= 34f;
                            Main.dust[num5].position = player.Center - vector;
                        }
                    }
                }
                if (shadeRegen && !draedonsHeart && !cFreeze && 
                    (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    if (player.lifeRegen < 0)
                    {
                        player.lifeRegen /= 2;
                    }
                    if (player.lifeRegenTime > 90 && player.lifeRegenTime < 1800)
                    {
                        player.lifeRegenTime = 1800;
                    }
                    player.lifeRegenTime += 4;
                    player.lifeRegen += 4;
                    float num3 = (float)(player.lifeRegenTime - 3000);
                    num3 /= 300f;
                    if (num3 > 0f)
                    {
                        if (num3 > 30f)
                        {
                            num3 = 30f;
                        }
                        num2 += num3;
                    }
                    float num4 = (float)player.statLifeMax2 / 400f * 0.85f + 0.15f;
                    num2 *= num4;
                    player.lifeRegen += (int)Math.Round((double)num2);
                    player.lifeRegenCount += player.lifeRegen;
                    if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
                    {
                        player.lifeRegenCount++;
                        if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.Next(30) == 0))
                        {
                            int num5 = Dust.NewDust(player.position, player.width, player.height, 173, 0f, 0f, 200, default(Color), 1f);
                            Main.dust[num5].noGravity = true;
                            Main.dust[num5].velocity *= 0.75f;
                            Main.dust[num5].fadeIn = 1.3f;
                            Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            vector.Normalize();
                            vector *= (float)Main.rand.Next(50, 100) * 0.04f;
                            Main.dust[num5].velocity = vector;
                            vector.Normalize();
                            vector *= 34f;
                            Main.dust[num5].position = player.Center - vector;
                        }
                    }
                }
                if (cFreeze && !shadeRegen && !draedonsHeart && 
                    (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    if (player.lifeRegen < 0)
                    {
                        player.lifeRegen /= 2;
                    }
                    if (player.lifeRegenTime > 90 && player.lifeRegenTime < 1800)
                    {
                        player.lifeRegenTime = 1800;
                    }
                    player.lifeRegenTime += 4;
                    player.lifeRegen += 4;
                    float num3 = (float)(player.lifeRegenTime - 3000);
                    num3 /= 300f;
                    if (num3 > 0f)
                    {
                        if (num3 > 30f)
                        {
                            num3 = 30f;
                        }
                        num2 += num3;
                    }
                    float num4 = (float)player.statLifeMax2 / 400f * 0.85f + 0.15f;
                    num2 *= num4;
                    player.lifeRegen += (int)Math.Round((double)num2);
                    player.lifeRegenCount += player.lifeRegen;
                    if (player.lifeRegen > 0 && player.statLife < player.statLifeMax2)
                    {
                        player.lifeRegenCount++;
                        if ((Main.rand.Next(30000) < player.lifeRegenTime || Main.rand.Next(30) == 0))
                        {
                            int num5 = Dust.NewDust(player.position, player.width, player.height, 67, 0f, 0f, 200, new Color(150, Main.DiscoG, 255), 0.75f);
                            Main.dust[num5].noGravity = true;
                            Main.dust[num5].velocity *= 0.75f;
                            Main.dust[num5].fadeIn = 1.3f;
                            Vector2 vector = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                            vector.Normalize();
                            vector *= (float)Main.rand.Next(50, 100) * 0.04f;
                            Main.dust[num5].velocity = vector;
                            vector.Normalize();
                            vector *= 34f;
                            Main.dust[num5].position = player.Center - vector;
                        }
                    }
                }
            }
        }
        #endregion
        #endregion

        #region HotKeys
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (CalamityMod.BossBarToggleHotKey.JustPressed)
            {
                if (drawBossHPBar)
                {
                    drawBossHPBar = false;
                }
                else
                {
                    drawBossHPBar = true;
                }
            }
            if (CalamityMod.BossBarToggleSmallTextHotKey.JustPressed)
            {
                if (shouldDrawSmallText)
                {
                    shouldDrawSmallText = false;
                }
                else
                {
                    shouldDrawSmallText = true;
                }
            }
            if (CalamityMod.TarraHotKey.JustPressed)
            {
                if (tarraMelee && tarraCooldown <= 0)
                {
                    tarraDefense = true;
                }
                if (bloodflareRanged && bloodflareRangedCooldown <= 0)
                {
                    bloodflareRangedCooldown = 1800;
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 60, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 60, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int damage = 800;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            float ai1 = (Main.rand.NextFloat() + 0.5f);
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, mod.ProjectileType("BloodflareSoul"), damage, 0f, player.whoAmI, 0f, ai1);
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, mod.ProjectileType("BloodflareSoul"), damage, 0f, player.whoAmI, 0f, ai1);
                        }
                    }
                }
                if (omegaBlueSet && omegaBlueCooldown <= 0)
                {
                    omegaBlueCooldown = 1800;
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int i = 0; i < 66; i++)
                    {
                        int d = Dust.NewDust(player.position, player.width, player.height, 20, 0, 0, 100, Color.Transparent, 2.6f);
                        Main.dust[d].noGravity = true;
                        Main.dust[d].noLight = true;
                        Main.dust[d].fadeIn = 1f;
                        Main.dust[d].velocity *= 6.6f;
                    }
                }
                if (dsSetBonus)
                {
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 36; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 235, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 235, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    if (player.whoAmI == Main.myPlayer)
                    {
                        player.AddBuff(mod.BuffType("Enraged"), 600, false);
                    }
                    if (Main.netMode != 1)
                    {
                        for (int l = 0; l < 200; l++)
                        {
                            NPC nPC = Main.npc[l];
                            if (nPC.active && !nPC.friendly && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= 3000f)
                            {
                                nPC.AddBuff(mod.BuffType("Enraged"), 600, false);
                            }
                        }
                    }
                }
            }
            if (CalamityMod.AstralArcanumUIHotkey.JustPressed && astralArcanum)
            {
                AstralArcanumUI.Toggle();
            }
            if (CalamityMod.AstralTeleportHotKey.JustPressed)
            {
                if (celestialJewel)
                {
                    if (Main.netMode == 0)
                    {
                        player.TeleportationPotion();
                        Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 6);
                    }
                    else if (Main.netMode == 1 && player.whoAmI == Main.myPlayer)
                    {
                        NetMessage.SendData(73, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
            }
            if (CalamityMod.AegisHotKey.JustPressed)
            {
                if (elysianAegis && !player.mount.Active)
                {
                    elysianGuard = !elysianGuard;
                }
            }
            if (CalamityMod.RageHotKey.JustPressed)
            {
                if (stress == stressMax && !rageMode)
                {
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 235, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 1.15f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 235, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    player.AddBuff(mod.BuffType("RageMode"), 300);
                }
            }
            if (CalamityMod.AdrenalineHotKey.JustPressed)
            {
                if (adrenaline == adrenalineMax && !adrenalineMode)
                {
                    Main.PlaySound(29, (int)player.position.X, (int)player.position.Y, 104);
                    for (int num502 = 0; num502 < 64; num502++)
                    {
                        int dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 16f), player.width, player.height - 16, 206, 0f, 0f, 0, default(Color), 1f);
                        Main.dust[dust].velocity *= 3f;
                        Main.dust[dust].scale *= 2f;
                    }
                    int num226 = 36;
                    for (int num227 = 0; num227 < num226; num227++)
                    {
                        Vector2 vector6 = Vector2.Normalize(player.velocity) * new Vector2((float)player.width / 2f, (float)player.height) * 0.75f;
                        vector6 = vector6.RotatedBy((double)((float)(num227 - (num226 / 2 - 1)) * 6.28318548f / (float)num226), default(Vector2)) + player.Center;
                        Vector2 vector7 = vector6 - player.Center;
                        int num228 = Dust.NewDust(vector6 + vector7, 0, 0, 206, vector7.X * 1.5f, vector7.Y * 1.5f, 100, default(Color), 1.4f);
                        Main.dust[num228].noGravity = true;
                        Main.dust[num228].noLight = true;
                        Main.dust[num228].velocity = vector7;
                    }
                    player.AddBuff(mod.BuffType("AdrenalineMode"), 300);
                }
            }
        }
        #endregion

        #region TeleportMethods
        public void HandleTeleport(int teleportType, bool forceHandle = false, int whoAmI = 0)
        {
            bool syncData = forceHandle || Main.netMode == 0;
            if (syncData)
            {
                TeleportPlayer(teleportType, forceHandle, whoAmI);
            }
            else
            {
                SyncTeleport(teleportType);
            }
        }

        public static void TeleportPlayer(int teleportType, bool syncData = false, int whoAmI = 0)
        {
            Player player;
            if (!syncData)
            {
                player = Main.LocalPlayer;
            }
            else
            {
                player = Main.player[whoAmI];
            }
            switch (teleportType)
            {
                case 0: UnderworldTeleport(player, syncData); break;
                case 1: DungeonTeleport(player, syncData); break;
                case 2: JungleTeleport(player, syncData); break;
                default: break;
            }
        }

        public void SyncTeleport(int teleportType)
        {
            ModPacket netMessage = mod.GetPacket();
            netMessage.Write((byte)CalamityModMessageType.TeleportPlayer);
            netMessage.Write(teleportType);
            netMessage.Send();
        }

        public static void UnderworldTeleport(Player player, bool syncData = false)
        {
            int teleportStartX = 100;
            int teleportRangeX = Main.maxTilesX - 200;
            int teleportStartY = Main.maxTilesY - 200;
            int teleportRangeY = 50;
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int width = player.width;
            Vector2 vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)(-(float)player.height));
            while (!flag && num < 1000)
            {
                num++;
                num2 = teleportStartX + Main.rand.Next(teleportRangeX);
                num3 = teleportStartY + Main.rand.Next(teleportRangeY);
                vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)(-(float)player.height));
                if (!Collision.SolidCollision(vector, width, player.height))
                {
                    if (Main.tile[num2, num3] == null)
                    {
                        Main.tile[num2, num3] = new Tile();
                    }
                    int i = 0;
                    while (i < 100)
                    {
                        if (Main.tile[num2, num3 + i] == null)
                        {
                            Main.tile[num2, num3 + i] = new Tile();
                        }
                        Tile tile = Main.tile[num2, num3 + i];
                        vector = new Vector2((float)num2, (float)(num3 + i)) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)(-(float)player.height));
                        Vector4 vector2 = Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        bool arg_1FF_0 = !Collision.SolidCollision(vector, width, player.height);
                        if (vector2.Z == player.velocity.X && vector2.Y == player.velocity.Y && vector2.X == vector.X)
                        {
                            bool arg_1FE_0 = vector2.Y == vector.Y;
                        }
                        if (arg_1FF_0)
                        {
                            i++;
                        }
                        else
                        {
                            if (tile.active() && !tile.inActive() && Main.tileSolid[(int)tile.type])
                            {
                                break;
                            }
                            i++;
                        }
                    }
                    if (!Collision.LavaCollision(vector, width, player.height) && Collision.HurtTiles(vector, player.velocity, width, player.height, false).Y <= 0f)
                    {
                        Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        if (Collision.SolidCollision(vector, width, player.height) && i < 99)
                        {
                            Vector2 vector3 = Vector2.UnitX * 16f;
                            if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                            {
                                vector3 = -Vector2.UnitX * 16f;
                                if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                {
                                    vector3 = Vector2.UnitY * 16f;
                                    if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                    {
                                        vector3 = -Vector2.UnitY * 16f;
                                        if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                        {
                                            flag = true;
                                            num3 += i;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return;
            }
            ModTeleport(player, vector, syncData, false);
        }

        public static void DungeonTeleport(Player player, bool syncData = false)
        {
            ModTeleport(player, new Vector2(Main.dungeonX, Main.dungeonY), syncData, true);
        }

        public static void JungleTeleport(Player player, bool syncData = false)
        {
            int jungleSide = CalamityWorld.abyssSide ? ((Main.maxTilesX / 2) + (Main.maxTilesX / 4)) : (Main.maxTilesX / 5);
            int teleportStartX = jungleSide;
            int teleportRangeX = 200;
            int teleportStartY = (int)Main.worldSurface;
            int teleportRangeY = 50;
            bool flag = false;
            int num = 0;
            int num2 = 0;
            int num3 = 0;
            int width = player.width;
            Vector2 vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)(-(float)player.height));
            while (!flag && num < 1000)
            {
                num++;
                num2 = teleportStartX + Main.rand.Next(teleportRangeX);
                num3 = teleportStartY + Main.rand.Next(teleportRangeY);
                vector = new Vector2((float)num2, (float)num3) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)(-(float)player.height));
                if (!Collision.SolidCollision(vector, width, player.height))
                {
                    if (Main.tile[num2, num3] == null)
                    {
                        Main.tile[num2, num3] = new Tile();
                    }
                    int i = 0;
                    while (i < 100)
                    {
                        if (Main.tile[num2, num3 + i] == null)
                        {
                            Main.tile[num2, num3 + i] = new Tile();
                        }
                        Tile tile = Main.tile[num2, num3 + i];
                        vector = new Vector2((float)num2, (float)(num3 + i)) * 16f + new Vector2((float)(-(float)width / 2 + 8), (float)(-(float)player.height));
                        Vector4 vector2 = Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        bool arg_1FF_0 = !Collision.SolidCollision(vector, width, player.height);
                        if (vector2.Z == player.velocity.X && vector2.Y == player.velocity.Y && vector2.X == vector.X)
                        {
                            bool arg_1FE_0 = vector2.Y == vector.Y;
                        }
                        if (arg_1FF_0)
                        {
                            i++;
                        }
                        else
                        {
                            if (tile.active() && !tile.inActive() && Main.tileSolid[(int)tile.type])
                            {
                                break;
                            }
                            i++;
                        }
                    }
                    if (!Collision.LavaCollision(vector, width, player.height) && Collision.HurtTiles(vector, player.velocity, width, player.height, false).Y <= 0f)
                    {
                        Collision.SlopeCollision(vector, player.velocity, width, player.height, player.gravDir, false);
                        if (Collision.SolidCollision(vector, width, player.height) && i < 99)
                        {
                            Vector2 vector3 = Vector2.UnitX * 16f;
                            if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                            {
                                vector3 = -Vector2.UnitX * 16f;
                                if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                {
                                    vector3 = Vector2.UnitY * 16f;
                                    if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                    {
                                        vector3 = -Vector2.UnitY * 16f;
                                        if (!(Collision.TileCollision(vector - vector3, vector3, player.width, player.height, false, false, (int)player.gravDir) != vector3))
                                        {
                                            flag = true;
                                            num3 += i;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (!flag)
            {
                return;
            }
            ModTeleport(player, vector, syncData, false);
        }

        public static void ModTeleport(Player player, Vector2 pos, bool syncData = false, bool convertFromTiles = false)
        {
            bool postImmune = player.immune;
            int postImmunteTime = player.immuneTime;
            if (convertFromTiles)
            {
                pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);
            }
            player.grappling[0] = -1;
            player.grapCount = 0;
            for (int index = 0; index < 1000; ++index)
            {
                if (Main.projectile[index].active && Main.projectile[index].owner == player.whoAmI && Main.projectile[index].aiStyle == 7)
                {
                    Main.projectile[index].Kill();
                }
            }
            player.Teleport(pos, 2, 0);
            player.velocity = Vector2.Zero;
            player.immune = postImmune;
            player.immuneTime = postImmunteTime;
            for (int index = 0; index < 100; ++index)
            {
                Main.dust[Dust.NewDust(player.position, player.width, player.height, 164, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 150, Color.Cyan, 1.2f)].velocity *= 0.5f;
            }
            Main.TeleportEffect(player.getRect(), 1);
            Main.TeleportEffect(player.getRect(), 3);
            Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 6);
            if (Main.netMode != 2)
            {
                return;
            }
            if (syncData)
            {
                RemoteClient.CheckSection(player.whoAmI, player.position, 1);
                NetMessage.SendData(65, -1, -1, null, 0, (float)player.whoAmI, pos.X, pos.Y, 3, 0, 0);
            }
        }
        #endregion

        #region UpdateEquips
        public override void UpdateVanityAccessories()
        {
            for (int n = 13; n < 18 + player.extraAccessorySlots; n++)
            {
                Item item = player.armor[n];
                if (item.type == mod.ItemType<Items.Permafrost.Popo>())
                {
                    snowmanHide = false;
                    snowmanForce = true;
                }
                else if (item.type == mod.ItemType<Items.Armor.AbyssalDivingSuit>())
                {
                    abyssalDivingSuitHide = false;
                    abyssalDivingSuitForce = true;
                }
                else if (item.type == mod.ItemType<Items.Armor.SirensHeart>())
                {
                    sirenBoobsHide = false;
                    sirenBoobsForce = true;
                }
                else if (item.type == mod.ItemType<Items.Armor.SirensHeartAlt>())
                {
                    sirenBoobsAltHide = false;
                    sirenBoobsAltForce = true;
                }
            }
        }

        public override void UpdateEquips(ref bool wallSpeedBuff, ref bool tileSpeedBuff, ref bool tileRangeBuff)
        {
            if (snowman)
            {
                if (player.whoAmI == Main.myPlayer && !snowmanNoseless)
                {
                    player.AddBuff(mod.BuffType<Buffs.Permafrost.Popo>(), 60, true);
                }
            }
            if (abyssalDivingSuit)
            {
                player.AddBuff(mod.BuffType<Buffs.AbyssalDivingSuitBuff>(), 60, true);
                if (player.whoAmI == Main.myPlayer)
                {
                    if (abyssalDivingSuitCooldown)
                    {
                        for (int l = 0; l < 22; l++)
                        {
                            int hasBuff = player.buffType[l];
                            if (player.buffTime[l] < 30 && hasBuff == mod.BuffType("AbyssalDivingSuitPlatesBroken"))
                            {
                                abyssalDivingSuitPlateHits = 0;
                                player.DelBuff(l);
                                l = -1;
                            }
                        }
                    }
                    else
                    {
                        player.AddBuff(mod.BuffType("AbyssalDivingSuitPlates"), 2);
                    }
                }
            }
            if (sirenBoobs)
            {
                player.AddBuff(mod.BuffType<Buffs.SirenBobs>(), 60, true);
            }
            else if (sirenBoobsAlt)
            {
                player.AddBuff(mod.BuffType<Buffs.SirenBobsAlt>(), 60, true);
            }
            if ((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3)
            {
                if (player.whoAmI == Main.myPlayer && !sirenIceCooldown)
                {
                    player.AddBuff(mod.BuffType("IceShieldBuff"), 2);
                }
            }
        }
        #endregion

        #region PreUpdateBuffs
        public override void PreUpdateBuffs()
        {
            //Remove the mighty wind buff if the player is in the astral desert.
            if (player.ZoneDesert && ZoneAstral && player.HasBuff(BuffID.WindPushed))
            {
                player.ClearBuff(BuffID.WindPushed);
            }
        }
        #endregion

        #region PostUpdateEffects
        public override void PostUpdateBuffs()
        {
            if (NOU)
                NOULOL();
            if (CalamityWorld.defiled)
                Defiled();
            if (weakPetrification)
                WeakPetrification();
            if (player.mount.Active || player.mount.Cart)
            {
                player.dashDelay = 60;
                dashMod = 0;
            }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }
        }
		
		public override void PostUpdateEquips()
		{
            if (NOU)
                NOULOL();
            if (CalamityWorld.defiled)
                Defiled();
            if (weakPetrification)
                WeakPetrification();
            if (player.mount.Active || player.mount.Cart)
            {
                player.dashDelay = 60;
                dashMod = 0;
            }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                if (player.lifeRegen < 0)
                    player.lifeRegen = 0;
            }
        }

        private void ExactLevelUp(int levelUpType, int level, bool final)
        {
            Color messageColor = Color.Orange;
            switch (levelUpType)
            {
                case 0:
                    exactMeleeLevel = level;
                    if (shootFireworksLevelUpMelee)
                    {
                        string key = (final ? "Mods.CalamityMod.MeleeLevelUpFinal" : "Mods.CalamityMod.MeleeLevelUp");
                        shootFireworksLevelUpMelee = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    break;
                case 1:
                    exactRangedLevel = level;
                    if (shootFireworksLevelUpRanged)
                    {
                        string key = (final ? "Mods.CalamityMod.RangedLevelUpFinal" : "Mods.CalamityMod.RangedLevelUp");
                        messageColor = Color.GreenYellow;
                        shootFireworksLevelUpRanged = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    break;
                case 2:
                    exactMagicLevel = level;
                    if (shootFireworksLevelUpMagic)
                    {
                        string key = (final ? "Mods.CalamityMod.MagicLevelUpFinal" : "Mods.CalamityMod.MagicLevelUp");
                        messageColor = Color.DodgerBlue;
                        shootFireworksLevelUpMagic = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    break;
                case 3:
                    exactSummonLevel = level;
                    if (shootFireworksLevelUpSummon)
                    {
                        string key = (final ? "Mods.CalamityMod.SummonLevelUpFinal" : "Mods.CalamityMod.SummonLevelUp");
                        messageColor = Color.Aquamarine;
                        shootFireworksLevelUpSummon = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    break;
                case 4:
                    exactRogueLevel = level;
                    if (shootFireworksLevelUpRogue)
                    {
                        string key = (final ? "Mods.CalamityMod.RogueLevelUpFinal" : "Mods.CalamityMod.RogueLevelUp");
                        messageColor = Color.Orchid;
                        shootFireworksLevelUpRogue = false;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            if (final)
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworkRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 1f);
                            }
                            else
                            {
                                Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -5f, ProjectileID.RocketFireworksBoxRed + Main.rand.Next(4),
                                    0, 0f, Main.myPlayer, 0f, 0f);
                            }
                        }
                        if (Main.netMode == 0)
                        {
                            Main.NewText(Language.GetTextValue(key), messageColor);
                        }
                        else if (Main.netMode == 2)
                        {
                            NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                        }
                    }
                    break;
            }
            if (Main.netMode == 1)
            {
                ExactLevelPacket(false, levelUpType);
            }
        }

        public override void PostUpdateMiscEffects()
        {
            Main.expertDebuffTime = 1f;
            areThereAnyDamnBosses = CalamityGlobalNPC.AnyBossNPCS();
            #region RevengeanceEffects
            if (CalamityWorld.revenge && player.whoAmI == Main.myPlayer)
            {
                if (player.onHitDodge)
                {
                    for (int l = 0; l < 22; l++)
                    {
                        int hasBuff = player.buffType[l];
                        if (player.buffTime[l] > 360 && hasBuff == BuffID.ShadowDodge)
                        {
                            player.buffTime[l] = 360;
                        }
                    }
                }
                if (player.immuneTime > 120)
                {
                    player.immuneTime = 120;
                }
                int stressGain = 0;
                if (rageMode)
                {
                    stressGain = -2000;
                }
                else
                {
                    if (draedonsHeart)
                    {
                        if (draedonsStressGain)
                        {
                            stressGain += 50;
                        }
                    }
                    else if (heartOfDarkness)
                    {
                        stressGain += 25;
                    }
                }
                stressCD++;
                if (stressCD >= 60)
                {
                    stressCD = 0;
                    stress += stressGain;
                    if (stress < 0)
                    {
                        stress = 0;
                    }
                    if (stress >= stressMax)
                    {
                        if (playFullRageSound)
                        {
                            playFullRageSound = false;
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullRage"), (int)player.position.X, (int)player.position.Y);
                        }
                        stress = stressMax;
                    }
                    else
                    {
                        playFullRageSound = true;
                    }
                }
                stressLevel500 = stress >= 9800;
                if (stressLevel500 && !hAttack)
                {
                    if (Main.rand.Next(10000) == 0)
                    {
                        player.AddBuff(mod.BuffType("HeartAttack"), 18000);
                    }
                }
                if (adrenaline >= adrenalineMax)
                {
                    adrenalineMaxTimer--;
                    if (adrenalineMaxTimer <= 0)
                    {
                        adrenalineDmgDown--;
                        if (adrenalineDmgDown < 0)
                            adrenalineDmgDown = 0;
                        adrenalineMaxTimer = 0;
                    }
                }
                else if (!adrenalineMode && adrenaline <= 0)
                {
                    adrenalineDmgDown = 600;
                    adrenalineMaxTimer = 180;
                    adrenalineDmgMult = 1f;
                }
                adrenalineDmgMult = 0.1f * (float)(adrenalineDmgDown / 60);
                if (adrenalineDmgMult < 0.33f)
                    adrenalineDmgMult = 0.33f;
                int adrenalineGain = 0;
                if (adrenalineMode)
                {
                    adrenalineGain = (CalamityGlobalNPC.SCal > -1 ? -10000 : -2000);
                }
                else
                {
                    if (Main.wof >= 0 && player.position.Y < (float)((Main.maxTilesY - 200) * 16)) // >
                    {
                        adrenaline = 0;
                    }
                    else if (areThereAnyDamnBosses || CalamityWorld.DoGSecondStageCountdown > 0)
                    {
                        int adrenalineTickBoost = 0 +
                            (adrenalineBoostOne ? 63 : 0) + //286
                            (adrenalineBoostTwo ? 115 : 0) + //401
                            (adrenalineBoostThree ? 100 : 0); //501
                        adrenalineGain = 223 + adrenalineTickBoost; //pre-slime god = 45, pre-astrum deus = 35, pre-polterghast = 25, post-polter = 20
                    }
                    else
                    {
                        adrenaline = 0;
                    }
                }
                adrenalineCD++;
                if (adrenalineCD >= (CalamityGlobalNPC.SCal > -1 ? 135 : 60))
                {
                    adrenalineCD = 0;
                    adrenaline += adrenalineGain;
                    if (adrenaline < 0)
                    {
                        adrenaline = 0;
                    }
                    if (adrenaline >= adrenalineMax)
                    {
                        if (playFullAdrenalineSound)
                        {
                            playFullAdrenalineSound = false;
                            Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Custom, "Sounds/Custom/FullAdrenaline"), (int)player.position.X, (int)player.position.Y);
                        }
                        adrenaline = adrenalineMax;
                    }
                    else
                    {
                        playFullAdrenalineSound = true;
                    }
                }
            }
            else if (!CalamityWorld.revenge && player.whoAmI == Main.myPlayer)
            {
                stressCD++;
                if (stressCD >= 60)
                {
                    stressCD = 0;
                    stress += -30;
                    if (stress < 0)
                    {
                        stress = 0;
                    }
                }
                adrenalineCD++;
                if (adrenalineCD >= 60)
                {
                    adrenalineCD = 0;
                    adrenaline += -30;
                    if (adrenaline < 0)
                    {
                        adrenaline = 0;
                    }
                }
            }
            if (player.whoAmI == Main.myPlayer && Main.netMode == 1)
            {
                packetTimer++;
                if (packetTimer == 60)
                {
                    packetTimer = 0;
                    StressPacket(false);
                    AdrenalinePacket(false);
                }
            }
            if (CalamityWorld.revenge)
            {
                if (player.lifeSteal > (CalamityWorld.death ? 30f : 40f))
                {
                    player.lifeSteal = (CalamityWorld.death ? 30f : 40f);
                }
            }
            #endregion
            #region StressTiedEffects
            if (stressPills)
            {
                player.statDefense += 5;
                player.meleeDamage += 0.06f;
                player.magicDamage += 0.06f;
                player.rangedDamage += 0.06f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.06f;
                player.minionDamage += 0.06f;
            }
            if (laudanum)
            {
                player.statDefense += 8;
                player.meleeDamage += 0.06f;
                player.magicDamage += 0.06f;
                player.rangedDamage += 0.06f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.06f;
                player.minionDamage += 0.06f;
            }
            if (!stressLevel500 && player.FindBuffIndex(mod.BuffType("HeartAttack")) > -1) { player.ClearBuff(mod.BuffType("HeartAttack")); }
            if (draedonsHeart && (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
            {
                player.statDefense += 50;
            }
            if (hAttack)
            {
                if (heartOfDarkness || draedonsHeart)
                {
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                    player.rangedDamage += 0.1f;
                    player.meleeDamage += 0.1f;
                    player.magicDamage += 0.1f;
                    player.minionDamage += 0.1f;
                }
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 5;
            }
            if (affliction)
            {
                player.lifeRegen += 1;
                player.endurance += CalamityWorld.revenge ? 0.07f : 0.05f;
                player.statDefense += CalamityWorld.revenge ? 20 : 15;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.rangedDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.meleeDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.magicDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.minionDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.statLifeMax2 += CalamityWorld.revenge ? (player.statLifeMax / 5 / 20 * 10) : (player.statLifeMax / 5 / 20 * 5);
            }
            else if (afflicted)
            {
                player.lifeRegen += 1;
                player.endurance += CalamityWorld.revenge ? 0.07f : 0.05f;
                player.statDefense += CalamityWorld.revenge ? 20 : 15;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.rangedDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.meleeDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.magicDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.minionDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.statLifeMax2 += CalamityWorld.revenge ? (player.statLifeMax / 5 / 20 * 10) : (player.statLifeMax / 5 / 20 * 5);
            }
            if (afflictedBuff) { afflicted = true; }
            #endregion
            #region MaxLifeAndManaBoosts
            player.statLifeMax2 +=
                (mFruit ? 25 : 0) +
                (bOrange ? 25 : 0) +
                (eBerry ? 25 : 0) +
                (dFruit ? 25 : 0);
            if (silvaHitCounter > 0)
            {
                player.statLifeMax2 -= silvaHitCounter * 100;
                if (player.statLifeMax2 < 200)
                {
                    player.statLifeMax2 = 200;
                }
            }
            player.statManaMax2 +=
                (pHeart ? 50 : 0) +
                (eCore ? 50 : 0) +
                (cShard ? 50 : 0) +
                (starBeamRye ? 50 : 0);
            if (Main.netMode != 2 && player.whoAmI == Main.myPlayer)
            {
                Texture2D rain3 = mod.GetTexture("ExtraTextures/Rain3");
                Texture2D rainOriginal = mod.GetTexture("ExtraTextures/RainOriginal");
                Texture2D mana2 = mod.GetTexture("ExtraTextures/Mana2");
                Texture2D mana3 = mod.GetTexture("ExtraTextures/Mana3");
                Texture2D mana4 = mod.GetTexture("ExtraTextures/Mana4");
                Texture2D manaOriginal = mod.GetTexture("ExtraTextures/ManaOriginal");
                Texture2D carpetAuric = mod.GetTexture("ExtraTextures/AuricCarpet");
                Texture2D carpetOriginal = mod.GetTexture("ExtraTextures/Carpet");
                int totalManaBoost =
                    (pHeart ? 1 : 0) +
                    (eCore ? 1 : 0) +
                    (cShard ? 1 : 0);
                switch (totalManaBoost)
                {
                    default:
                        Main.manaTexture = manaOriginal;
                        break;
                    case 3:
                        Main.manaTexture = mana4;
                        break;
                    case 2:
                        Main.manaTexture = mana3;
                        break;
                    case 1:
                        Main.manaTexture = mana2;
                        break;
                }
                if (Main.bloodMoon) { Main.rainTexture = rainOriginal; }
                else if (Main.raining && ZoneSulphur) { Main.rainTexture = rain3; }
                else { Main.rainTexture = rainOriginal; }
                if (auricSet) { Main.flyingCarpetTexture = carpetAuric; }
                else { Main.flyingCarpetTexture = carpetOriginal; }
            }
            #endregion
            #region MiscEffects
            if (gainLevelCooldown > 0)
                gainLevelCooldown--;
            #region MeleeLevels
            switch (meleeLevel)
            {
                case 100:
                    ExactLevelUp(0, 1, false);
                    break;
                case 300:
                    ExactLevelUp(0, 2, false);
                    break;
                case 600:
                    ExactLevelUp(0, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(0, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(0, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(0, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(0, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(0, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(0, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(0, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(0, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(0, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(0, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(0, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(0, 15, true);
                    break;
                default:
                    break;
            }
            #endregion
            #region RangedLevels
            switch (rangedLevel)
            {
                case 100:
                    ExactLevelUp(1, 1, false);
                    break;
                case 300:
                    ExactLevelUp(1, 2, false);
                    break;
                case 600:
                    ExactLevelUp(1, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(1, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(1, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(1, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(1, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(1, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(1, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(1, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(1, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(1, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(1, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(1, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(1, 15, true);
                    break;
                default:
                    break;
            }
            #endregion
            #region MagicLevels
            switch (magicLevel)
            {
                case 100:
                    ExactLevelUp(2, 1, false);
                    break;
                case 300:
                    ExactLevelUp(2, 2, false);
                    break;
                case 600:
                    ExactLevelUp(2, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(2, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(2, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(2, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(2, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(2, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(2, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(2, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(2, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(2, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(2, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(2, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(2, 15, true);
                    break;
                default:
                    break;
            }
            #endregion
            #region SummonLevels
            switch (summonLevel)
            {
                case 100:
                    ExactLevelUp(3, 1, false);
                    break;
                case 300:
                    ExactLevelUp(3, 2, false);
                    break;
                case 600:
                    ExactLevelUp(3, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(3, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(3, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(3, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(3, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(3, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(3, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(3, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(3, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(3, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(3, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(3, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(3, 15, true);
                    break;
                default:
                    break;
            }
            #endregion
            #region RogueLevels
            switch (rogueLevel)
            {
                case 100:
                    ExactLevelUp(4, 1, false);
                    break;
                case 300:
                    ExactLevelUp(4, 2, false);
                    break;
                case 600:
                    ExactLevelUp(4, 3, false);
                    break;
                case 1000:
                    ExactLevelUp(4, 4, false);
                    break;
                case 1500:
                    ExactLevelUp(4, 5, false);
                    break;
                case 2100:
                    ExactLevelUp(4, 6, false);
                    break;
                case 2800:
                    ExactLevelUp(4, 7, false);
                    break;
                case 3600:
                    ExactLevelUp(4, 8, false);
                    break;
                case 4500:
                    ExactLevelUp(4, 9, false);
                    break;
                case 5500:
                    ExactLevelUp(4, 10, false);
                    break;
                case 6600:
                    ExactLevelUp(4, 11, false);
                    break;
                case 7800:
                    ExactLevelUp(4, 12, false);
                    break;
                case 9100:
                    ExactLevelUp(4, 13, false);
                    break;
                case 10500:
                    ExactLevelUp(4, 14, false);
                    break;
                case 12500: //celebration or some shit for final level, yay
                    ExactLevelUp(4, 15, true);
                    break;
                default:
                    break;
            }
            #endregion
            if (Main.myPlayer == player.whoAmI)
            {
                BossHealthBarManager.SHOULD_DRAW_SMALLTEXT_HEALTH = shouldDrawSmallText;
            }
            if (silvaSet || invincible || margarita)
            {
                foreach (int debuff in CalamityMod.debuffList) { player.buffImmune[debuff] = true; }
            }
            if (Main.expertMode && player.ZoneSnow && player.wet && !player.lavaWet && !player.honeyWet && !player.arcticDivingGear)
            {
                player.buffImmune[BuffID.Chilled] = true;
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    if (Main.myPlayer == player.whoAmI && !player.gills && !player.merman)
                    {
                        if (player.breath > 0)
                            player.breath--;
                    }
                }
            }
            if (ZoneAbyss)
            {
                if (abyssalAmulet) { player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * (lumenousAmulet ? 15 : 10); }
                if (Main.myPlayer == player.whoAmI) //4200 total tiles small world
                {
                    int breathLoss = 2;
                    int lightStrength = 0 +
                        ((player.lightOrb || player.crimsonHeart || player.magicLantern) ? 1 : 0) + //1
                        (player.arcticDivingGear ? 1 : 0) + //2
                        (jellyfishNecklace ? 1 : 0) + //3
                        ((player.blueFairy || player.greenFairy || player.redFairy || player.petFlagDD2Ghost) ? 2 : 0) + //5
                        ((shine || lumenousAmulet) ? 2 : 0) + //7
                        ((player.wisp || player.suspiciouslookingTentacle || sirenPet) ? 3 : 0); //10
                    bool lightLevelOne = lightStrength > 0; //1+
                    bool lightLevelTwo = lightStrength > 2; //3+
                    bool lightLevelThree = lightStrength > 4; //5+
                    bool lightLevelFour = lightStrength > 6; //7+
                    if (ZoneAbyssLayer4) //3200 and below
                    {
                        breathLoss = 54;
                        if (!lightLevelFour) { player.blind = true; }
                        if (!lightLevelThree) { player.headcovered = true; }
                        if (!depthCharm) { player.bleed = true; }
                        player.statDefense -= (anechoicPlating ? 40 : 120);
                    }
                    else if (ZoneAbyssLayer3) //2700 to 3200
                    {
                        breathLoss = 18;
                        if (!lightLevelThree) { player.blind = true; }
                        if (!lightLevelTwo) { player.headcovered = true; }
                        if (!depthCharm) { player.bleed = true; }
                        player.statDefense -= (anechoicPlating ? 20 : 60);
                    }
                    else if (ZoneAbyssLayer2) //2100 to 2700
                    {
                        breathLoss = 6;
                        if (!lightLevelTwo) { player.blind = true; }
                        if (!depthCharm) { player.bleed = true; }
                        player.statDefense -= (anechoicPlating ? 10 : 30);
                    }
                    else if (ZoneAbyssLayer1) //1500 to 2100
                    {
                        if (!lightLevelOne) { player.blind = true; }
                        player.statDefense -= (anechoicPlating ? 5 : 15);
                    }
                    double breathLossMult = 1.0 -
                        (player.gills ? 0.2 : 0.0) - //0.8
                        (player.accDivingHelm ? 0.25 : 0.0) - //0.75
                        (player.arcticDivingGear ? 0.25 : 0.0) - //0.75
                        (player.accMerman ? 0.3 : 0.0) - //0.7
                        (victideSet ? 0.2 : 0.0) - //0.85
                        (((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3) ? 0.3 : 0.0) - //0.7
                        (abyssalDivingSuit ? 0.3 : 0.0); //0.7
                    if (breathLossMult < 0.05) { breathLossMult = 0.05; }
                    breathLoss = (int)((double)breathLoss * breathLossMult);
                    int tick = 6;
                    double tickMult = 1.0 +
                        (player.gills ? 4.0 : 0.0) + //5
                        (player.ignoreWater ? 5.0 : 0.0) + //10
                        (player.accDivingHelm ? 10.0 : 0.0) + //20
                        (player.arcticDivingGear ? 10.0 : 0.0) + //30
                        (player.accMerman ? 15.0 : 0.0) + //45
                        (victideSet ? 5.0 : 0.0) + //50
                        (((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3) ? 15.0 : 0.0) + //65
                        (abyssalDivingSuit ? 15.0 : 0.0); //80
                    if (tickMult > 50.0) { tickMult = 50.0; }
                    tick = (int)((double)tick * tickMult);
                    abyssBreathCD++;
                    if (player.gills || player.merman)
                    {
                        if (player.breath > 0)
                            player.breath -= 3;
                    }
                    if (!Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                    {
                        if (player.statLife > 100)
                        {
                            if (player.lifeRegen > 0)
                            {
                                player.lifeRegen = 0;
                            }
                            player.lifeRegenTime = 0;
                            player.lifeRegen -= 160;
                        }
                    }
                    if (abyssBreathCD >= tick)
                    {
                        abyssBreathCD = 0;
                        if (player.breath > 0)
                            player.breath -= breathLoss;
                        if (cDepth)
                        {
                            if (player.breath > 0)
                                player.breath--;
                        }
                        if (player.breath <= 0)
                        {
                            int lifeLoss = 6 +
                                (player.bleed ? 6 : 0) -
                                (depthCharm ? 3 : 0);
                            player.statLife -= lifeLoss;
                            if (player.statLife <= 0)
                            {
                                KillPlayer();
                            }
                        }
                    }
                }
            }
            else
            {
                abyssBreathCD = 0;
            }
            if ((Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && ironBoots) || 
                (aeroSet && !Collision.DrownCollision(player.position, player.width, player.height, player.gravDir)))
            {
                player.maxFallSpeed = 15f;
            }
            if (omegaBlueSet) //should apply after rev caps, actually those are gone so AAAAA
            {
                //add tentacles
                if (player.ownedProjectileCounts[mod.ProjectileType("OmegaBlueTentacle")] < 6)
                {
                    bool[] tentaclesPresent = new bool[6];
                    for (int i = 0; i < 1000; i++)
                    {
                        Projectile projectile = Main.projectile[i];
                        if (projectile.active && projectile.type == mod.ProjectileType("OmegaBlueTentacle") && projectile.owner == Main.myPlayer && projectile.ai[1] >= 0f && projectile.ai[1] < 6f)
                        {
                            tentaclesPresent[(int)projectile.ai[1]] = true;
                        }
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        if (!tentaclesPresent[i])
                        {
                            float modifier = player.meleeDamage + player.magicDamage + player.rangedDamage + 
                                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage + player.minionDamage;
                            modifier /= 5f;
                            int damage = (int)(666 * modifier);
                            Vector2 vel = new Vector2(Main.rand.Next(-13, 14), Main.rand.Next(-13, 14)) * 0.25f;
                            Projectile.NewProjectile(player.Center, vel, mod.ProjectileType("OmegaBlueTentacle"), damage, 8f, Main.myPlayer, Main.rand.Next(120), i);
                        }
                    }
                }
                float damageUp = 0.1f;
                int critUp = 10;
                if (omegaBlueHentai)
                {
                    damageUp *= 2f;
                    critUp *= 2;
                }
                player.meleeDamage += damageUp;
                player.rangedDamage += damageUp;
                player.magicDamage += damageUp;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageUp;
                player.minionDamage += damageUp;
                player.meleeCrit += critUp;
                player.rangedCrit += critUp;
                player.magicCrit += critUp;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += critUp;
            }
            if (!bOut)
            {
                if (gHealer)
                {
                    if (healCounter > 0) { healCounter--; }
                    if (healCounter <= 0)
                    {
                        healCounter = 300;
                        if (player.whoAmI == Main.myPlayer)
                        {
                            int healAmount = 5 +
                                (gDefense ? 5 : 0) +
                                (gOffense ? 5 : 0);
                            player.statLife += healAmount;
                            player.HealEffect(healAmount);
                        }
                    }
                }
                if (gDefense)
                {
                    player.moveSpeed += 0.1f +
                        (gOffense ? 0.1f : 0f);
                    player.endurance += 0.025f +
                        (gOffense ? 0.025f : 0f);
                }
                if (gOffense)
                {
                    player.minionDamage += 0.1f +
                        (gDefense ? 0.05f : 0f);
                    player.maxMinions++;
                }
            }
            if (revivifyTimer > 0) { revivifyTimer--; }
            if (fleshTotemCooldown > 0) { fleshTotemCooldown--; }
            if (astralStarRainCooldown > 0) { astralStarRainCooldown--; }
            if (bloodflareMageCooldown > 0) { bloodflareMageCooldown--; }
            if (tarraMageHealCooldown > 0) { tarraMageHealCooldown--; }
            if (ataxiaDmg > 0) { ataxiaDmg--; }
            if (xerocDmg > 0) { xerocDmg--; }
            if (godSlayerDmg > 0) { godSlayerDmg--; }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
                player.buffImmune[mod.BuffType("VulnerabilityHex")] = true;
                silvaCountdown--;
                for (int j = 0; j < 2; j++)
                {
                    int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
                    Dust expr_A4_cp_0 = Main.dust[num];
                    expr_A4_cp_0.position.X = expr_A4_cp_0.position.X + (float)Main.rand.Next(-20, 21);
                    Dust expr_CB_cp_0 = Main.dust[num];
                    expr_CB_cp_0.position.Y = expr_CB_cp_0.position.Y + (float)Main.rand.Next(-20, 21);
                    Main.dust[num].velocity *= 0.9f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    }
                }
            }
            if (tarraMelee)
            {
                if (tarraDefense)
                {
                    tarraDefenseTime--;
                    if (tarraDefenseTime <= 0)
                    {
                        tarraDefenseTime = 600;
                        tarraCooldown = 1800;
                        tarraDefense = false;
                    }
                    for (int j = 0; j < 2; j++)
                    {
                        int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 157, 0f, 0f, 100, new Color(Main.DiscoR, 203, 103), 2f);
                        Dust expr_A4_cp_0 = Main.dust[num];
                        expr_A4_cp_0.position.X = expr_A4_cp_0.position.X + (float)Main.rand.Next(-20, 21);
                        Dust expr_CB_cp_0 = Main.dust[num];
                        expr_CB_cp_0.position.Y = expr_CB_cp_0.position.Y + (float)Main.rand.Next(-20, 21);
                        Main.dust[num].velocity *= 0.9f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        }
                    }
                }
                if (tarraCooldown > 0) { tarraCooldown--; }
            }
            if (tarraThrowing)
            {
                if (tarraThrowingCritTimer > 0) { tarraThrowingCritTimer--; }
                if (tarraThrowingCrits >= 25)
                {
                    tarraThrowingCrits = 0;
                    tarraThrowingCritTimer = 1800;
                    player.immune = true;
                    player.immuneTime = 300;
                }
                for (int l = 0; l < 22; l++)
                {
                    int hasBuff = player.buffType[l];
                    bool shouldAffect = CalamityMod.debuffList.Contains(hasBuff);
                    if (shouldAffect)
                    {
                        CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                    }
                }
            }
            if (bloodflareSet)
            {
                if (bloodflareHeartTimer > 0) { bloodflareHeartTimer--; }
                if (bloodflareManaTimer > 0) { bloodflareManaTimer--; }
            }
            if (bloodflareMelee)
            {
                if (bloodflareMeleeHits >= 15)
                {
                    bloodflareMeleeHits = 0;
                    bloodflareFrenzyTimer = 300;
                }
                if (bloodflareFrenzyTimer > 0)
                {
                    bloodflareFrenzyTimer--;
                    if (bloodflareFrenzyTimer <= 0)
                    {
                        bloodflareFrenzyCooldown = 1800;
                    }
                    player.meleeCrit += 25;
                    player.meleeDamage += 0.25f;
                    for (int j = 0; j < 2; j++)
                    {
                        int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 5, 0f, 0f, 100, default(Color), 2f);
                        Dust expr_A4_cp_0 = Main.dust[num];
                        expr_A4_cp_0.position.X = expr_A4_cp_0.position.X + (float)Main.rand.Next(-20, 21);
                        Dust expr_CB_cp_0 = Main.dust[num];
                        expr_CB_cp_0.position.Y = expr_CB_cp_0.position.Y + (float)Main.rand.Next(-20, 21);
                        Main.dust[num].velocity *= 0.9f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        }
                    }
                }
                if (bloodflareFrenzyCooldown > 0) { bloodflareFrenzyCooldown--; }
            }
            if (bloodflareRanged)
            {
                if (bloodflareRangedCooldown > 0) { bloodflareRangedCooldown--; }
            }
            if (Main.raining && ZoneSulphur)
            {
                if (player.ZoneOverworldHeight || player.ZoneSkyHeight) { player.AddBuff(mod.BuffType("Irradiated"), 2); }
            }
            if (raiderTalisman) { CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += ((float)raiderStack / 250f) * 0.15f; }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaSummon)
            {
                player.maxMinions += 2;
            }
            if (sDefense)
            {
                player.statDefense += 5;
                player.endurance += 0.05f;
            }
            if (absorber)
            {
                player.moveSpeed += 0.12f;
                player.jumpSpeedBoost += 1.2f;
                player.statLifeMax2 += 20;
                player.thorns = 0.5f;
                player.endurance += 0.05f;
                if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    player.lifeRegen += 2;
                    player.manaRegenBonus += 2;
                }
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    player.statDefense += 5;
                    player.endurance += 0.05f;
                    player.moveSpeed += 0.2f;
                }
            }
            if (coreOfTheBloodGod) { player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 10; }
            if (bloodPact) { player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 100; }
            if (aAmpoule)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1f, 1f, 0.6f);
                player.endurance += 0.05f;
                player.pickSpeed -= 0.5f;
                player.buffImmune[70] = true;
                player.buffImmune[47] = true;
                player.buffImmune[46] = true;
                player.buffImmune[44] = true;
                player.buffImmune[20] = true;
                if (!player.honey && player.lifeRegen < 0)
                {
                    player.lifeRegen += 2;
                    if (player.lifeRegen > 0)
                    {
                        player.lifeRegen = 0;
                    }
                }
                player.lifeRegenTime += 1;
                player.lifeRegen += 2;
            }
            else if (cFreeze)
            {
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0.3f, ((float)Main.DiscoG / 400f), 0.5f);
            }
            else if (sirenIce)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 0.35f, 1f, 1.25f);
            }
            else if (sirenBoobs || sirenBoobsAlt)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1.5f, 1f, 0.1f);
            }
            else if (tarraSummon)
            {
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 3f, 0f);
            }
            if (cFreeze)
            {
                int num = mod.BuffType("GlacialState");
                float num2 = 200f;
                int random = Main.rand.Next(5);
                if (player.whoAmI == Main.myPlayer)
                {
                    if (random == 0)
                    {
                        for (int l = 0; l < 200; l++)
                        {
                            NPC nPC = Main.npc[l];
                            if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && !nPC.buffImmune[num] && Vector2.Distance(player.Center, nPC.Center) <= num2)
                            {
                                if (nPC.FindBuffIndex(num) == -1)
                                {
                                    nPC.AddBuff(num, 120, false);
                                }
                            }
                        }
                    }
                }
            }
            if (invincible || lol)
            {
                player.thorns = 0f;
                player.turtleThorns = false;
            }
            if (player.vortexStealthActive)
            {
                player.rangedDamage -= (1f - player.stealth) * 0.4f; //change 80 to 40
                player.rangedCrit -= (int)((1f - player.stealth) * 5f); //change 20 to 15
            }
            #endregion
            #region StandingStillEffects
            if (aquaticEmblem)
            {
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && !player.mount.Active)
                {
                    if (aquaticBoost > 0f)
                    {
                        aquaticBoost -= 0.0002f; //0.015
                        if ((double)aquaticBoost <= 0.0)
                        {
                            aquaticBoost = 0f;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    aquaticBoost += 0.0002f;
                    if (aquaticBoost > 1f)
                    {
                        aquaticBoost = 1f;
                    }
                    if (player.mount.Active)
                    {
                        aquaticBoost = 1f;
                    }
                }
                player.statDefense += (int)((1f - aquaticBoost) * 40f);
                player.moveSpeed -= (1f - aquaticBoost) * 0.15f;
            }
            if (auricBoost)
            {
                if (player.itemAnimation > 0)
                {
                    modStealthTimer = 5;
                }
                if ((double)player.velocity.X > -0.1 && (double)player.velocity.X < 0.1 && (double)player.velocity.Y > -0.1 && (double)player.velocity.Y < 0.1 && !player.mount.Active)
                {
                    if (modStealthTimer == 0 && modStealth > 0f)
                    {
                        modStealth -= 0.015f;
                        if ((double)modStealth <= 0.0)
                        {
                            modStealth = 0f;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    float num27 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    modStealth += num27 * 0.0075f;
                    if (modStealth > 1f)
                    {
                        modStealth = 1f;
                    }
                    if (player.mount.Active)
                    {
                        modStealth = 1f;
                    }
                }
                player.meleeDamage += (1f - modStealth) * 0.2f;
                player.meleeCrit += (int)((1f - modStealth) * 10f);
                player.rangedDamage += (1f - modStealth) * 0.2f;
                player.rangedCrit += (int)((1f - modStealth) * 10f);
                player.magicDamage += (1f - modStealth) * 0.2f;
                player.magicCrit += (int)((1f - modStealth) * 10f);
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += (1f - modStealth) * 0.2f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += (int)((1f - modStealth) * 10f);
                player.minionDamage += (1f - modStealth) * 0.2f;
                if (modStealthTimer > 0)
                {
                    modStealthTimer--;
                }
            }
            else if (pAmulet)
            {
                if (player.itemAnimation > 0)
                {
                    modStealthTimer = 5;
                }
                if ((double)player.velocity.X > -0.1 && (double)player.velocity.X < 0.1 && (double)player.velocity.Y > -0.1 && (double)player.velocity.Y < 0.1 && !player.mount.Active)
                {
                    if (modStealthTimer == 0 && modStealth > 0f)
                    {
                        modStealth -= 0.015f;
                        if ((double)modStealth <= 0.0)
                        {
                            modStealth = 0f;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                            }
                        }
                    }
                }
                else
                {
                    float num27 = Math.Abs(player.velocity.X) + Math.Abs(player.velocity.Y);
                    modStealth += num27 * 0.0075f;
                    if (modStealth > 1f)
                    {
                        modStealth = 1f;
                    }
                    if (player.mount.Active)
                    {
                        modStealth = 1f;
                    }
                }
                player.meleeDamage += (1f - modStealth) * 0.2f;
                player.meleeCrit += (int)((1f - modStealth) * 10f);
                player.aggro -= (int)((1f - modStealth) * 750f);
                if (modStealthTimer > 0)
                {
                    modStealthTimer--;
                }
            }
            else
            {
                modStealth = 1f;
            }
            #endregion
            #region ElysianAegis
            if (elysianAegis)
            {
                bool flag14 = false;
                if (elysianGuard)
                {
                    float num29 = shieldInvinc;
                    shieldInvinc -= 0.08f;
                    if (shieldInvinc < 0f)
                    {
                        shieldInvinc = 0f;
                    }
                    else
                    {
                        flag14 = true;
                    }
                    if (shieldInvinc == 0f && num29 != shieldInvinc && Main.netMode == 1)
                    {
                        NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                    player.rangedDamage += (5f - shieldInvinc) * 0.02f;
                    player.rangedCrit += (int)((5f - shieldInvinc) * 2f);
                    player.meleeDamage += (5f - shieldInvinc) * 0.02f;
                    player.meleeCrit += (int)((5f - shieldInvinc) * 2f);
                    player.magicDamage += (5f - shieldInvinc) * 0.02f;
                    player.magicCrit += (int)((5f - shieldInvinc) * 2f);
                    player.minionDamage += (5f - shieldInvinc) * 0.02f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += (5f - shieldInvinc) * 0.02f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += (int)((5f - shieldInvinc) * 2f);
                    player.aggro += (int)((5f - shieldInvinc) * 220f);
                    player.statDefense += (int)((5f - shieldInvinc) * 5f);
                    player.moveSpeed *= 0.85f;
                    if (player.mount.Active)
                    {
                        elysianGuard = false;
                    }
                }
                else
                {
                    float num30 = shieldInvinc;
                    shieldInvinc += 0.08f;
                    if (shieldInvinc > 5f)
                    {
                        shieldInvinc = 5f;
                    }
                    else
                    {
                        flag14 = true;
                    }
                    if (shieldInvinc == 5f && num30 != shieldInvinc && Main.netMode == 1)
                    {
                        NetMessage.SendData(84, -1, -1, null, player.whoAmI, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
                if (flag14)
                {
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust = Main.dust[Dust.NewDust(player.Center - vector * 30f, 0, 0, 244, 0f, 0f, 0, default(Color), 1f)];
                        dust.noGravity = true;
                        dust.position = player.Center - vector * (float)Main.rand.Next(5, 11);
                        dust.velocity = vector.RotatedBy(1.5707963705062866, default(Vector2)) * 4f;
                        dust.scale = 0.5f + Main.rand.NextFloat();
                        dust.fadeIn = 0.5f;
                    }
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector2 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust2 = Main.dust[Dust.NewDust(player.Center - vector2 * 30f, 0, 0, 246, 0f, 0f, 0, default(Color), 1f)];
                        dust2.noGravity = true;
                        dust2.position = player.Center - vector2 * 12f;
                        dust2.velocity = vector2.RotatedBy(-1.5707963705062866, default(Vector2)) * 2f;
                        dust2.scale = 0.5f + Main.rand.NextFloat();
                        dust2.fadeIn = 0.5f;
                    }
                }
            }
            else
            {
                elysianGuard = false;
            }
            #endregion
            #region SeasonalChecks
            if (Main.netMode != 2)
            {
                if (Main.time == 0.0)
                {
                    runCheck = 0;
                }
                if (runCheck == 0)
                {
                    CalamityWorld.checkSpring();
                    CalamityWorld.checkSummer();
                    CalamityWorld.checkFall();
                    CalamityWorld.checkWinter();
                    runCheck++;
                }
            }
            #endregion
            #region OtherBuffs
            if (irradiated)
            {
                player.statDefense -= 10;
                player.endurance -= 0.1f;
                player.meleeDamage += 0.05f;
                player.magicDamage += 0.05f;
                player.rangedDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                player.minionDamage += 0.05f;
                player.minionKB += 0.5f;
                player.moveSpeed += 0.05f;
            }
            if (rRage)
            {
                player.meleeDamage += 0.05f;
                player.magicDamage += 0.05f;
                player.rangedDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                player.minionDamage += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.moveSpeed += 0.05f;
            }
            if (xRage)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
            }
            if (xWrath)
            {
                player.meleeCrit += 5;
                player.magicCrit += 5;
                player.rangedCrit += 5;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            }
            if (godSlayerCooldown)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
            }
            if (graxDefense)
            {
                player.statDefense += 15;
                player.endurance += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.meleeDamage += 0.1f;
            }
            if (sMeleeBoost)
            {
                player.meleeSpeed -= 0.05f;
                player.meleeDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
                player.meleeCrit += 5;
                player.magicCrit += 5;
                player.rangedCrit += 5;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            }
            if (tFury)
            {
                player.meleeDamage += 0.3f;
                player.meleeCrit += 10;
            }
            if (yPower)
            {
                player.endurance += 0.05f;
                player.statDefense += 5;
                player.meleeCrit += 2;
                player.meleeDamage += 0.06f;
                player.meleeSpeed -= 0.05f;
                player.magicCrit += 2;
                player.magicDamage += 0.06f;
                player.rangedCrit += 2;
                player.rangedDamage += 0.06f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.06f;
                player.minionDamage += 0.06f;
                player.minionKB += 1f;
                player.moveSpeed += 0.15f;
            }
            if (tScale)
            {
                player.endurance += 0.05f;
                player.statDefense += 5;
                player.kbBuff = true;
            }
            if (darkSunRing)
            {
                player.maxMinions += 1;
                player.lifeRegen += 1;
                player.statDefense += 8;
                player.meleeSpeed -= 0.08f;
                player.meleeDamage += 0.08f;
                player.rangedDamage += 0.08f;
                player.magicDamage += 0.08f;
                player.pickSpeed -= 0.3f;
                player.minionDamage += 0.08f;
                player.minionKB += 1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.08f;
                if (Main.dayTime)
                {
                    player.lifeRegen += 1;
                }
                else
                {
                    player.statDefense += 20;
                }
            }
            if (badgeOfBravery)
            {
                if (player.statLife <= (player.statLifeMax2 * 0.8f) && player.statLife > (player.statLifeMax2 * 0.6f))
                {
                    player.meleeSpeed -= 0.05f;
                }
                else if (player.statLife <= (player.statLifeMax2 * 0.6f) && player.statLife > (player.statLifeMax2 * 0.4f))
                {
                    player.meleeSpeed -= 0.1f;
                }
                else if (player.statLife <= (player.statLifeMax2 * 0.4f) && player.statLife > (player.statLifeMax2 * 0.2f))
                {
                    player.meleeSpeed -= 0.15f;
                }
                else if (player.statLife <= (player.statLifeMax2 * 0.2f))
                {
                    player.meleeSpeed -= 0.2f;
                }
            }
            if (eGauntlet)
            {
                player.longInvince = true;
                player.kbGlove = true;
                player.meleeDamage += 0.15f;
                player.meleeCrit += 5;
                player.meleeSpeed -= 0.15f;
                player.lavaImmune = true;
            }
            if (yInsignia)
            {
                player.longInvince = true;
                player.kbGlove = true;
                player.meleeDamage += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.lavaImmune = true;
                if (player.statLife <= (player.statLifeMax2 * 0.5f))
                {
                    player.meleeDamage += 0.1f;
                    player.magicDamage += 0.1f;
                    player.rangedDamage += 0.1f;
                    player.thrownDamage += 0.1f;
                    player.minionDamage += 0.1f;
                }
                if (player.statLife <= (player.statLifeMax2 * 0.8f) && player.statLife > (player.statLifeMax2 * 0.6f))
                {
                    player.meleeSpeed -= 0.05f;
                }
                else if (player.statLife <= (player.statLifeMax2 * 0.6f) && player.statLife > (player.statLifeMax2 * 0.4f))
                {
                    player.meleeSpeed -= 0.1f;
                }
                else if (player.statLife <= (player.statLifeMax2 * 0.4f) && player.statLife > (player.statLifeMax2 * 0.2f))
                {
                    player.meleeSpeed -= 0.15f;
                }
                else if (player.statLife <= (player.statLifeMax2 * 0.2f))
                {
                    player.meleeSpeed -= 0.2f;
                }
            }
            if (fabsolVodka)
            {
                alcoholPoisonLevel++;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
                player.statDefense -= 20;
            }
            if (vodka)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 2;
                player.statDefense -= 4;
                player.meleeCrit += 2;
                player.meleeDamage += 0.08f;
                player.magicCrit += 2;
                player.magicDamage += 0.08f;
                player.rangedCrit += 2;
                player.rangedDamage += 0.08f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.08f;
                player.minionDamage += 0.08f;
            }
            if (redWine)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 2;
            }
            if (grapeBeer)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 1;
                player.statDefense -= 2;
                player.moveSpeed -= 0.05f;
            }
            if (moonshine)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 2;
                player.statDefense += 10;
                player.endurance += 0.05f;
            }
            if (rum)
            {
                alcoholPoisonLevel++;
                player.lifeRegen += 2;
                player.moveSpeed += 0.1f;
                player.statDefense -= 8;
            }
            if (fireball)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
            }
            if (whiskey)
            {
                alcoholPoisonLevel++;
                player.statDefense -= 8;
                player.meleeCrit += 2;
                player.meleeDamage += 0.04f;
                player.magicCrit += 2;
                player.magicDamage += 0.04f;
                player.rangedCrit += 2;
                player.rangedDamage += 0.04f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.04f;
                player.minionDamage += 0.04f;
            }
            if (everclear)
            {
                alcoholPoisonLevel += 2;
                player.lifeRegen -= 20;
                player.statDefense -= 40;
                player.meleeDamage += 0.25f;
                player.magicDamage += 0.25f;
                player.rangedDamage += 0.25f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.25f;
                player.minionDamage += 0.25f;
            }
            if (bloodyMary)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                if (Main.bloodMoon)
                {
                    player.statDefense -= 6;
                    player.meleeCrit += 7;
                    player.meleeDamage += 0.15f;
                    player.meleeSpeed -= 0.15f;
                    player.magicCrit += 7;
                    player.magicDamage += 0.15f;
                    player.rangedCrit += 7;
                    player.rangedDamage += 0.15f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 7;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
                    player.minionDamage += 0.15f;
                    player.moveSpeed += 0.15f;
                }
            }
            if (tequila)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 2;
                if (Main.dayTime)
                {
                    player.statDefense += 5;
                    player.meleeCrit += 2;
                    player.meleeDamage += 0.04f;
                    player.magicCrit += 2;
                    player.magicDamage += 0.04f;
                    player.rangedCrit += 2;
                    player.rangedDamage += 0.04f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.04f;
                    player.minionDamage += 0.04f;
                    player.endurance += 0.04f;
                }
            }
            if (tequilaSunrise)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                if (Main.dayTime)
                {
                    player.statDefense += 15;
                    player.meleeCrit += 3;
                    player.meleeDamage += 0.09f;
                    player.magicCrit += 3;
                    player.magicDamage += 0.09f;
                    player.rangedCrit += 3;
                    player.rangedDamage += 0.09f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.09f;
                    player.minionDamage += 0.09f;
                    player.endurance += 0.09f;
                }
            }
            if (screwdriver)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
            }
            if (caribbeanRum)
            {
                alcoholPoisonLevel++;
                player.lifeRegen += 2;
                player.moveSpeed += 0.2f;
                player.statDefense -= 12;
            }
            if (cinnamonRoll)
            {
                alcoholPoisonLevel++;
                player.statDefense -= 12;
                player.manaRegenDelay--;
                player.manaRegenBonus += 10;
            }
            if (margarita)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                player.statDefense -= 6;
            }
            if (starBeamRye)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                player.statDefense -= 6;
                player.magicDamage += 0.08f;
                player.manaCost *= 0.9f;
            }
            if (moscowMule)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                player.meleeCrit += 3;
                player.meleeDamage += 0.12f;
                player.magicCrit += 3;
                player.magicDamage += 0.12f;
                player.rangedCrit += 3;
                player.rangedDamage += 0.12f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.12f;
                player.minionDamage += 0.12f;
            }
            if (whiteWine)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                player.statDefense -= 6;
                player.magicDamage += 0.1f;
            }
            if (evergreenGin)
            {
                alcoholPoisonLevel++;
                player.lifeRegen -= 3;
                player.endurance += 0.05f;
            }
            if (alcoholPoisonLevel > 3)
            {
                if (player.lifeRegen > 0)
                {
                    player.lifeRegen = 0;
                }
                player.lifeRegenTime = 0;
                player.lifeRegen -= 3 * alcoholPoisonLevel;
            }
            if (nanotech)
            {
                if (CalamityMod.throwingItemList.Contains(player.inventory[player.selectedItem].type))
                {
                    player.endurance += 0.1f;
                    player.statDefense += 15;
                }
            }
            if (harpyRing)
            {
                if (player.wingTimeMax > 0)
                {
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.25);
                }
                player.moveSpeed += 0.2f;
            }
            if (community)
            {
                float floatTypeBoost = 0.01f +
                    (NPC.downedSlimeKing ? 0.01f : 0f) +
                    (NPC.downedBoss1 ? 0.01f : 0f) +
                    (NPC.downedBoss2 ? 0.01f : 0f) +
                    (NPC.downedQueenBee ? 0.01f : 0f) + //0.05
                    (NPC.downedBoss3 ? 0.01f : 0f) +
                    (Main.hardMode ? 0.01f : 0f) +
                    (NPC.downedMechBossAny ? 0.01f : 0f) +
                    (NPC.downedPlantBoss ? 0.01f : 0f) +
                    (NPC.downedGolemBoss ? 0.01f : 0f) + //0.1
                    (NPC.downedFishron ? 0.01f : 0f) +
                    (NPC.downedAncientCultist ? 0.01f : 0f) +
                    (NPC.downedMoonlord ? 0.01f : 0f) +
                    (CalamityWorld.downedProvidence ? 0.02f : 0f) + //0.15
                    (CalamityWorld.downedDoG ? 0.02f : 0f) + //0.17
                    (CalamityWorld.downedYharon ? 0.03f : 0f); //0.2
                int integerTypeBoost = (int)(floatTypeBoost * 50f);
                int critBoost = integerTypeBoost / 2;
                int regenBoost = 1 + (integerTypeBoost / 5);
                float damageBoost = floatTypeBoost * 0.5f;
                player.endurance += (floatTypeBoost * 0.25f);
                player.statDefense += integerTypeBoost;
                player.meleeCrit += critBoost;
                player.meleeDamage += damageBoost;
                player.meleeSpeed -= (floatTypeBoost * 0.25f);
                player.magicCrit += critBoost;
                player.magicDamage += damageBoost;
                player.rangedCrit += critBoost;
                player.rangedDamage += damageBoost;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += critBoost;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += damageBoost;
                player.minionDamage += damageBoost;
                player.minionKB += floatTypeBoost;
                player.moveSpeed += floatTypeBoost;
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * integerTypeBoost;
                bool lesserEffect = false;
                for (int l = 0; l < 22; l++)
                {
                    int hasBuff = player.buffType[l];
                    bool shouldAffect = CalamityMod.alcoholList.Contains(hasBuff);
                    if (shouldAffect)
                    {
                        lesserEffect = true;
                    }
                }
                if (player.lifeRegen < 0)
                {
                    player.lifeRegen += lesserEffect ? 1 : regenBoost;
                }
                if (player.wingTimeMax > 0)
                {
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.15);
                }
            }
            if (bloodflareThrowing)
            {
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.8))
                {
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
                    player.statDefense += 30;
                }
                else
                {
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                }
            }
            if (bloodflareSummon)
            {
                if (player.statLife >= (int)((double)player.statLifeMax2 * 0.9))
                {
                    player.minionDamage += 0.1f;
                }
                else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.statDefense += 20;
                    player.lifeRegen += 2;
                }
                if (bloodflareSummonTimer > 0) { bloodflareSummonTimer--; }
                if (player.whoAmI == Main.myPlayer && bloodflareSummonTimer <= 0)
                {
                    bloodflareSummonTimer = 900;
                    for (int I = 0; I < 3; I++)
                    {
                        float ai1 = (float)(I * 120);
                        Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(I * 120) * 550), player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
                            mod.ProjectileType("GhostlyMine"), (int)((auricSet ? 15000f : 5000f) * player.minionDamage), 1f, player.whoAmI, ai1, 0f);
                    }
                }
            }
            if (reaperToothNecklace)
            {
                player.meleeDamage += 0.25f;
                player.magicDamage += 0.25f;
                player.rangedDamage += 0.25f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.25f;
                player.minionDamage += 0.25f;
                player.statDefense /= 2;
            }
            if (coreOfTheBloodGod)
            {
                player.endurance += 0.05f;
                player.meleeDamage += 0.07f;
                player.magicDamage += 0.07f;
                player.rangedDamage += 0.07f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.07f;
                player.minionDamage += 0.07f;
                if (player.statDefense < 100)
                {
                    player.meleeDamage += 0.15f;
                    player.magicDamage += 0.15f;
                    player.rangedDamage += 0.15f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
                    player.minionDamage += 0.15f;
                }
            }
            else if (bloodflareCore)
            {
                if (player.statDefense < 100)
                {
                    player.meleeDamage += 0.15f;
                    player.magicDamage += 0.15f;
                    player.rangedDamage += 0.15f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
                    player.minionDamage += 0.15f;
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
                {
                    player.endurance += 0.1f;
                    player.meleeDamage += 0.2f;
                    player.magicDamage += 0.2f;
                    player.rangedDamage += 0.2f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.2f;
                    player.minionDamage += 0.2f;
                }
                else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.endurance += 0.05f;
                    player.meleeDamage += 0.1f;
                    player.magicDamage += 0.1f;
                    player.rangedDamage += 0.1f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                    player.minionDamage += 0.1f;
                }
            }
            if (godSlayerThrowing)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 10;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                    CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.1f;
                }
            }
            if (tarraSummon)
            {
                int lifeCounter = 0;
                float num2 = 300f;
                bool flag = lifeCounter % 60 == 0;
                int num3 = 200;
                if (player.whoAmI == Main.myPlayer)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        NPC nPC = Main.npc[l];
                        if (nPC.active && !nPC.friendly && nPC.damage > 0 && !nPC.dontTakeDamage && Vector2.Distance(player.Center, nPC.Center) <= num2)
                        {
                            if (flag)
                            {
                                nPC.StrikeNPC(num3, 0f, 0, false, false, false);
                                if (Main.netMode != 0)
                                {
                                    NetMessage.SendData(28, -1, -1, null, l, (float)num3, 0f, 0f, 0, 0, 0);
                                }
                            }
                        }
                    }
                }
                lifeCounter++;
                if (lifeCounter >= 180)
                {
                    lifeCounter = 0;
                }
                if (player.statLife >= player.statLifeMax2)
                {
                    player.minionDamage += 0.1f;
                    player.maxMinions += 2;
                }
            }
            #endregion
            #region DRLimitAndOtherShit
            if (auricSet && silvaMelee)
            {
                double multiplier = (double)player.statLife / (double)player.statLifeMax2;
                player.meleeDamage += (float)(multiplier * 0.5); //ranges from 1.5 times to 1 times
            }
            if (player.endurance > defEndurance) //0.33
            {
                float damageReductionAboveCap = player.endurance - defEndurance; //0.6 - 0.33 = 0.27
                player.endurance = defEndurance + (damageReductionAboveCap * 0.1f); //0.33 + (0.27 * 0.1) = 0.357
            }
            #region MeleeLevelBoosts
            if (meleeLevel >= 12500)
            {
                player.meleeDamage += 0.12f;
                player.meleeSpeed -= 0.12f;
                player.meleeCrit += 6;
            }
            else if (meleeLevel >= 10500)
            {
                player.meleeDamage += 0.1f;
                player.meleeSpeed -= 0.1f;
                player.meleeCrit += 5;
            }
            else if (meleeLevel >= 9100)
            {
                player.meleeDamage += 0.09f;
                player.meleeSpeed -= 0.09f;
                player.meleeCrit += 5;
            }
            else if (meleeLevel >= 7800)
            {
                player.meleeDamage += 0.08f;
                player.meleeSpeed -= 0.08f;
                player.meleeCrit += 4;
            }
            else if (meleeLevel >= 6600)
            {
                player.meleeDamage += 0.07f;
                player.meleeSpeed -= 0.07f;
                player.meleeCrit += 4;
            }
            else if (meleeLevel >= 5500) //hm limit
            {
                player.meleeDamage += 0.06f;
                player.meleeSpeed -= 0.06f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 4500)
            {
                player.meleeDamage += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.meleeCrit += 3;
            }
            else if (meleeLevel >= 3600)
            {
                player.meleeDamage += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 2800)
            {
                player.meleeDamage += 0.04f;
                player.meleeSpeed -= 0.04f;
                player.meleeCrit += 2;
            }
            else if (meleeLevel >= 2100)
            {
                player.meleeDamage += 0.04f;
                player.meleeSpeed -= 0.03f;
                player.meleeCrit += 1;
            }
            else if (meleeLevel >= 1500) //prehm limit
            {
                player.meleeDamage += 0.03f;
                player.meleeSpeed -= 0.02f;
                player.meleeCrit += 1;
            }
            else if (meleeLevel >= 1000)
            {
                player.meleeDamage += 0.03f;
                player.meleeSpeed -= 0.01f;
                player.meleeCrit += 1;
            }
            else if (meleeLevel >= 600)
            {
                player.meleeDamage += 0.02f;
                player.meleeSpeed -= 0.01f;
            }
            else if (meleeLevel >= 300)
                player.meleeDamage += 0.02f;
            else if (meleeLevel >= 100)
                player.meleeDamage += 0.01f;
            #endregion
            #region RangedLevelBoosts
            if (rangedLevel >= 12500)
            {
                player.rangedDamage += 0.12f;
                player.moveSpeed += 0.12f;
                player.rangedCrit += 6;
            }
            else if (rangedLevel >= 10500)
            {
                player.rangedDamage += 0.1f;
                player.moveSpeed += 0.1f;
                player.rangedCrit += 5;
            }
            else if (rangedLevel >= 9100)
            {
                player.rangedDamage += 0.09f;
                player.moveSpeed += 0.09f;
                player.rangedCrit += 5;
            }
            else if (rangedLevel >= 7800)
            {
                player.rangedDamage += 0.08f;
                player.moveSpeed += 0.08f;
                player.rangedCrit += 4;
            }
            else if (rangedLevel >= 6600)
            {
                player.rangedDamage += 0.07f;
                player.moveSpeed += 0.07f;
                player.rangedCrit += 4;
            }
            else if (rangedLevel >= 5500)
            {
                player.rangedDamage += 0.06f;
                player.moveSpeed += 0.06f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 4500)
            {
                player.rangedDamage += 0.05f;
                player.moveSpeed += 0.05f;
                player.rangedCrit += 3;
            }
            else if (rangedLevel >= 3600)
            {
                player.rangedDamage += 0.05f;
                player.moveSpeed += 0.05f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 2800)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.04f;
                player.rangedCrit += 2;
            }
            else if (rangedLevel >= 2100)
            {
                player.rangedDamage += 0.04f;
                player.moveSpeed += 0.03f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 1500)
            {
                player.rangedDamage += 0.03f;
                player.moveSpeed += 0.02f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 1000)
            {
                player.rangedDamage += 0.03f;
                player.moveSpeed += 0.01f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 600)
            {
                player.rangedDamage += 0.02f;
                player.rangedCrit += 1;
            }
            else if (rangedLevel >= 300)
                player.rangedDamage += 0.02f;
            else if (rangedLevel >= 100)
                player.rangedDamage += 0.01f;
            #endregion
            #region MagicLevelBoosts
            if (magicLevel >= 12500)
            {
                player.magicDamage += 0.12f;
                player.manaCost *= 0.88f;
                player.magicCrit += 6;
            }
            else if (magicLevel >= 10500)
            {
                player.magicDamage += 0.1f;
                player.manaCost *= 0.9f;
                player.magicCrit += 5;
            }
            else if (magicLevel >= 9100)
            {
                player.magicDamage += 0.09f;
                player.manaCost *= 0.91f;
                player.magicCrit += 5;
            }
            else if (magicLevel >= 7800)
            {
                player.magicDamage += 0.08f;
                player.manaCost *= 0.92f;
                player.magicCrit += 4;
            }
            else if (magicLevel >= 6600)
            {
                player.magicDamage += 0.07f;
                player.manaCost *= 0.93f;
                player.magicCrit += 4;
            }
            else if (magicLevel >= 5500)
            {
                player.magicDamage += 0.06f;
                player.manaCost *= 0.94f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 4500)
            {
                player.magicDamage += 0.05f;
                player.manaCost *= 0.95f;
                player.magicCrit += 3;
            }
            else if (magicLevel >= 3600)
            {
                player.magicDamage += 0.05f;
                player.manaCost *= 0.95f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 2800)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.96f;
                player.magicCrit += 2;
            }
            else if (magicLevel >= 2100)
            {
                player.magicDamage += 0.04f;
                player.manaCost *= 0.97f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 1500)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.98f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 1000)
            {
                player.magicDamage += 0.03f;
                player.manaCost *= 0.99f;
                player.magicCrit += 1;
            }
            else if (magicLevel >= 600)
            {
                player.magicDamage += 0.02f;
                player.manaCost *= 0.99f;
            }
            else if (magicLevel >= 300)
                player.magicDamage += 0.02f;
            else if (magicLevel >= 100)
                player.magicDamage += 0.01f;
            #endregion
            #region SummonLevelBoosts
            if (summonLevel >= 12500)
            {
                player.minionDamage += 0.12f;
                player.minionKB += 3.0f;
                player.maxMinions += 3;
            }
            else if (summonLevel >= 10500)
            {
                player.minionDamage += 0.1f;
                player.minionKB += 3.0f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 9100)
            {
                player.minionDamage += 0.09f;
                player.minionKB += 2.7f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 7800)
            {
                player.minionDamage += 0.08f;
                player.minionKB += 2.4f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 6600)
            {
                player.minionDamage += 0.07f;
                player.minionKB += 2.1f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 5500)
            {
                player.minionDamage += 0.06f;
                player.minionKB += 1.8f;
                player.maxMinions += 2;
            }
            else if (summonLevel >= 4500)
            {
                player.minionDamage += 0.06f;
                player.minionKB += 1.8f;
                player.maxMinions++;
            }
            else if (summonLevel >= 3600)
            {
                player.minionDamage += 0.05f;
                player.minionKB += 1.5f;
                player.maxMinions++;
            }
            else if (summonLevel >= 2800)
            {
                player.minionDamage += 0.04f;
                player.minionKB += 1.2f;
                player.maxMinions++;
            }
            else if (summonLevel >= 2100)
            {
                player.minionDamage += 0.04f;
                player.minionKB += 0.9f;
                player.maxMinions++;
            }
            else if (summonLevel >= 1500)
            {
                player.minionDamage += 0.03f;
                player.minionKB += 0.6f;
            }
            else if (summonLevel >= 1000)
            {
                player.minionDamage += 0.03f;
                player.minionKB += 0.3f;
            }
            else if (summonLevel >= 600)
            {
                player.minionDamage += 0.02f;
                player.minionKB += 0.3f;
            }
            else if (summonLevel >= 300)
                player.minionDamage += 0.02f;
            else if (summonLevel >= 100)
                player.minionDamage += 0.01f;
            #endregion
            #region RogueLevelBoosts
            if (rogueLevel >= 12500)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.12f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.12f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 6;
            }
            else if (rogueLevel >= 10500)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.1f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            }
            else if (rogueLevel >= 9100)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.09f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.09f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 5;
            }
            else if (rogueLevel >= 7800)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.08f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.08f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 4;
            }
            else if (rogueLevel >= 6600)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.07f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.07f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 4;
            }
            else if (rogueLevel >= 5500)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.06f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.06f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            }
            else if (rogueLevel >= 4500)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 3;
            }
            else if (rogueLevel >= 3600)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.05f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            else if (rogueLevel >= 2800)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.04f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.04f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 2;
            }
            else if (rogueLevel >= 2100)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.04f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.03f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 1;
            }
            else if (rogueLevel >= 1500)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.02f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 1;
            }
            else if (rogueLevel >= 1000)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.03f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.01f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingCrit += 1;
            }
            else if (rogueLevel >= 600)
            {
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity += 0.01f;
            }
            else if (rogueLevel >= 300)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.02f;
            else if (rogueLevel >= 100)
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.01f;
            #endregion
            if (dArtifact)
            {
                player.meleeDamage += 0.25f;
                player.magicDamage += 0.25f;
                player.rangedDamage += 0.25f;
                player.minionDamage += 0.25f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.25f;
            }
            if (eArtifact)
            {
                player.meleeSpeed -= 0.1f;
                player.manaCost *= 0.85f;
                CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingDamage += 0.15f;
                player.maxMinions += 2;
            }
            if (gArtifact)
            {
                player.maxMinions += 8;
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(mod.BuffType("AngryChicken")) == -1)
                    {
                        player.AddBuff(mod.BuffType("AngryChicken"), 3600, true);
                    }
                    if (player.ownedProjectileCounts[mod.ProjectileType("AngryChicken")] < 2)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("AngryChicken"), (int)(232f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (pArtifact)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    if (player.FindBuffIndex(mod.BuffType("GuardianHealer")) == -1)
                    {
                        player.AddBuff(mod.BuffType("GuardianHealer"), 3600, true);
                    }
                    if (player.ownedProjectileCounts[mod.ProjectileType("MiniGuardianHealer")] < 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -6f, mod.ProjectileType("MiniGuardianHealer"), 0, 0f, Main.myPlayer, 0f, 0f);
                    }
                    float baseDamage = 150f +
                        (CalamityWorld.downedDoG ? 100f : 0f) +
                        (CalamityWorld.downedYharon ? 100f : 0f);
                    if (player.maxMinions >= 8)
                    {
                        if (player.FindBuffIndex(mod.BuffType("GuardianDefense")) == -1)
                        {
                            player.AddBuff(mod.BuffType("GuardianDefense"), 3600, true);
                        }
                        if (player.ownedProjectileCounts[mod.ProjectileType("MiniGuardianDefense")] < 1)
                        {
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -3f, mod.ProjectileType("MiniGuardianDefense"), (int)(baseDamage * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
                        }
                    }
                    if (tarraSummon || bloodflareSummon || godSlayerSummon || silvaSummon || dsSetBonus)
                    {
                        if (player.FindBuffIndex(mod.BuffType("GuardianOffense")) == -1)
                        {
                            player.AddBuff(mod.BuffType("GuardianOffense"), 3600, true);
                        }
                        if (player.ownedProjectileCounts[mod.ProjectileType("MiniGuardianAttack")] < 1)
                        {
                            Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, -1f, mod.ProjectileType("MiniGuardianAttack"), (int)(baseDamage * player.minionDamage), 1f, Main.myPlayer, 0f, 0f);
                        }
                    }
                }
            }
            if (marked || reaperToothNecklace)
            {
                player.endurance *= 0.5f;
            }
            #endregion
        }

        public override void PostUpdateRunSpeeds()
		{
            #region SpeedBoosts
            if (hAttack)
			{
				if (!Main.dayTime)
				{
					player.runAcceleration *= 1.05f;
					player.maxRunSpeed *= 1.05f;
				}
			}
			if (stressPills || laudanum)
			{
				player.runAcceleration *= 1.05f;
				player.maxRunSpeed *= 1.05f;
			}
            if (abyssalDivingSuit)
            {
                player.runAcceleration *= Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) ? 1.05f : 0.4f;
                player.maxRunSpeed *= Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) ? 1.05f : 0.4f;
            }
            if (sirenWaterBuff)
            {
                player.runAcceleration *= 1.15f;
                player.maxRunSpeed *= 1.15f;
            }
            if (frostFlare && player.statLife < (int)((double)player.statLifeMax2 * 0.25))
            {
                player.runAcceleration *= 1.15f;
                player.maxRunSpeed *= 1.15f;
            }
            if (shadowSpeed)
            {
                player.runAcceleration *= 1.5f;
                player.maxRunSpeed *= 1.5f;
            }
            else if (auricSet)
            {
                player.runAcceleration *= 1.1f;
                player.maxRunSpeed *= 1.1f;
            }
            else if (silvaSet)
			{
				player.runAcceleration *= 1.05f;
				player.maxRunSpeed *= 1.05f;
			}
            if (cTracers)
            {
                player.runAcceleration *= 1.1f;
                player.maxRunSpeed *= 1.1f;
            }
            else if (eTracers)
            {
                player.runAcceleration *= 1.05f;
                player.maxRunSpeed *= 1.05f;
            }
            if (horror)
            {
                player.runAcceleration *= 0.85f;
                player.maxRunSpeed *= 0.85f;
            }
            if (elysianGuard)
            {
                player.runAcceleration *= 0.5f;
                player.maxRunSpeed *= 0.5f;
            }
            #endregion
            #region DashEffects
            if (player.pulley && dashMod > 0)
            {
                ModDashMovement();
            }
            else if (player.grappling[0] == -1 && !player.tongued)
            {
                if (dashMod > 0)
                {
                    ModHorizontalMovement();
                    ModDashMovement();
                }
                if (pAmulet && modStealth < 1f)
                {
                    float num43 = player.maxRunSpeed / 2f * (1f - modStealth);
                    player.maxRunSpeed -= num43;
                    player.accRunSpeed = player.maxRunSpeed;
                }
            }
            #endregion
        }
        #endregion

        #region PreKill
        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
            if (invincible && player.inventory[player.selectedItem].type != mod.ItemType("ColdheartIcicle"))
            {
                if (player.statLife <= 0)
                {
                    player.statLife = 1;
                }
                return false;
            }
            if (hInferno)
            {
                for (int x = 0; x < 200; x++)
                {
                    if (Main.npc[x].active && Main.npc[x].type == mod.NPCType("Providence"))
                    {
                        Main.npc[x].active = false;
                    }
                }
            }
			if (nCore && Main.rand.Next(10) == 0)
			{
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 67);
				for (int j = 0; j < 25; j++)
				{
					int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 173, 0f, 0f, 100, default(Color), 2f);
					Dust expr_A4_cp_0 = Main.dust[num];
					expr_A4_cp_0.position.X = expr_A4_cp_0.position.X + (float)Main.rand.Next(-20, 21);
					Dust expr_CB_cp_0 = Main.dust[num];
					expr_CB_cp_0.position.Y = expr_CB_cp_0.position.Y + (float)Main.rand.Next(-20, 21);
					Main.dust[num].velocity *= 0.9f;
					Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					}
				}
				player.statLife += 100;
    			player.HealEffect(100);
				return false;
			}
            if (godSlayer && !godSlayerCooldown)
			{
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 67);
				for (int j = 0; j < 50; j++)
				{
					int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 173, 0f, 0f, 100, default(Color), 2f);
					Dust expr_A4_cp_0 = Main.dust[num];
					expr_A4_cp_0.position.X = expr_A4_cp_0.position.X + (float)Main.rand.Next(-20, 21);
					Dust expr_CB_cp_0 = Main.dust[num];
					expr_CB_cp_0.position.Y = expr_CB_cp_0.position.Y + (float)Main.rand.Next(-20, 21);
					Main.dust[num].velocity *= 0.9f;
					Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
					if (Main.rand.Next(2) == 0)
					{
						Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
					}
				}
				player.statLife += 150;
    			player.HealEffect(150);
    			player.AddBuff(mod.BuffType("GodSlayerCooldown"), 2700);
				return false;
			}
            if (silvaSet && silvaCountdown > 0)
            {
                if (hasSilvaEffect)
                {
                    silvaHitCounter++;
                }
                if (player.FindBuffIndex(mod.BuffType("SilvaRevival")) == -1)
                    player.AddBuff(mod.BuffType("SilvaRevival"), 600);
                hasSilvaEffect = true;
                if (player.statLife < 1)
                {
                    player.statLife = 1;
                }
                return false;
            }
            if (permafrostsConcoction && player.FindBuffIndex(mod.BuffType("ConcoctionCooldown")) == -1)
            {
                player.AddBuff(mod.BuffType("ConcoctionCooldown"), 10800);
                player.AddBuff(mod.BuffType("Encased"), 300);
                player.statLife = player.statLifeMax2 * 3 / 10;
                Main.PlaySound(SoundID.Item92, player.position);
                for (int i = 0; i < 60; i++)
                {
                    int d = Dust.NewDust(player.position, player.width, player.height, 88, 0f, 0f, 0, default(Color), 2.5f);
                    Main.dust[d].noGravity = true;
                    Main.dust[d].velocity *= 5f;
                }
                return false;
            }
            if (bBlood && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + " became a blood geyser.");
			}
			if ((bFlames || aFlames) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + " was consumed by the black flames.");
			}
			if (pFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s flesh was melted by the plague.");
			}
			if ((hFlames || hInferno) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + " fell prey to their sins.");
			}
            if (CalamityGlobalNPC.SCal > -1)
            {
                if (sCalDeathCount < 51)
                {
                    sCalDeathCount++;
                }
            }
            if (CalamityWorld.ironHeart && areThereAnyDamnBosses)
            {
                KillPlayer();
                return false;
            }
            return true;
		}
        #endregion

        #region UseTimeMult
        public override float UseTimeMultiplier(Item item)
        {
            if (silvaRanged)
            {
                if (item.ranged && !CalamityMod.throwingItemList.Contains(item.type) && item.useTime > 3)
                    return (auricSet ? 1.2f : 1.1f);
            }
            if (silvaThrowing)
            {
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.9) && 
                    CalamityMod.throwingItemList.Contains(item.type) && item.useTime > 3)
                    return 1.1f;
            }
            return 1f;
        }
        #endregion

        #region MeleeSpeedMult
        public override float MeleeSpeedMultiplier(Item item)
        {
            if (player.meleeSpeed > 1.5f)
            {
                return (1.5f / player.meleeSpeed);
            }
            return 1f;
        }
        #endregion

        #region GetWeaponDamageAndKB
        public override void GetWeaponDamage(Item item, ref int damage)
		{
            if (flamethrowerBoost && item.ranged && item.useAmmo == 23)
            {
                damage = (int)((double)damage * 1.25);
            }
            if (dodgeScarf && item.melee && item.shoot == 0)
            {
                damage = (int)((double)damage * 1.2);
            }
            if ((cinnamonRoll && CalamityMod.fireWeaponList.Contains(item.type)) || (evergreenGin && CalamityMod.natureWeaponList.Contains(item.type)))
            {
                damage = (int)((double)damage * 1.15);
            }
            if (fireball && CalamityMod.fireWeaponList.Contains(item.type))
            {
                damage = (int)((double)damage * 1.1);
            }
        }
		
		public override void GetWeaponKnockback(Item item, ref float knockback)
		{
			if (pAmulet && item.melee)
			{
				knockback *= 1f + (1f - modStealth) * 0.5f;
			}
			if (auricBoost)
			{
				knockback *= 1f + (1f - modStealth) * 0.5f;
			}
            if (whiskey)
            {
                knockback *= 1.04f;
            }
            if (tequila && Main.dayTime)
            {
                knockback *= 1.04f;
            }
            if (tequilaSunrise && Main.dayTime)
            {
                knockback *= 1.09f;
            }
            if (moscowMule)
            {
                knockback *= 1.12f;
            }
        }
        #endregion

        #region Shoot
        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
            if (CalamityMod.throwingItemList.Contains(item.type))
            {
                speedX *= CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity;
                speedY *= CalamityCustomThrowingDamagePlayer.ModPlayer(player).throwingVelocity;
            }
            if (eArtifact && item.ranged && !CalamityMod.throwingItemList.Contains(item.type))
            {
                speedX *= 1.25f;
                speedY *= 1.25f;
            }
            if (bloodflareMage) //0 - 99
            {
                if (item.magic && Main.rand.Next(0, 100) >= 95)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("GhostlyBolt"), (int)((double)damage * (auricSet ? 4.2 : 2.6)), 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (bloodflareRanged) //0 - 99
            {
                if (item.ranged && !CalamityMod.throwingItemList.Contains(item.type) && Main.rand.Next(0, 100) >= 98)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("BloodBomb"), (int)((double)damage * (auricSet ? 2.2 : 1.6)), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (tarraMage)
            {
                if (tarraCrits >= 5 && player.whoAmI == Main.myPlayer)
                {
                    tarraCrits = 0;
                    int num106 = 9 + Main.rand.Next(3);
                    for (int num107 = 0; num107 < num106; num107++)
                    {
                        float num110 = 0.025f * (float)num107;
                        float hardar = speedX + (float)Main.rand.Next(-25, 26) * num110;
                        float hordor = speedY + (float)Main.rand.Next(-25, 26) * num110;
                        float num84 = (float)Math.Sqrt((double)(speedX * speedX + speedY * speedY));
                        num84 = item.shootSpeed / num84;
                        hardar *= num84;
                        hordor *= num84;
                        Projectile.NewProjectile(position.X, position.Y, hardar, hordor, 206, (int)((double)damage * 0.2), knockBack, player.whoAmI, 0.0f, 0.0f);
                    }
                }
            }
			if (ataxiaBolt)
			{
                if (item.ranged && !CalamityMod.throwingItemList.Contains(item.type) && Main.rand.Next(2) == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("ChaosFlare"), (int)((double)damage * 0.25), 2f, player.whoAmI, 0f, 0f);
                    }
                }
			}
            if (godSlayerRanged) //0 - 99
            {
                if (item.ranged && !CalamityMod.throwingItemList.Contains(item.type) && Main.rand.Next(0, 100) >= 95)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("GodSlayerShrapnelRound"), (int)((double)damage * (auricSet ? 3.2 : 2.1)), 2f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            if (ataxiaVolley)
			{
                if (CalamityMod.throwingItemList.Contains(item.type) && Main.rand.Next(10) == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 20);
                        float spread = 45f * 0.0174f;
                        double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                        double deltaAngle = spread / 8f;
                        double offsetAngle;
                        int i;
                        for (i = 0; i < 4; i++)
                        {
                            Vector2 vector2 = new Vector2(player.Center.X, player.Center.Y);
                            offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("ChaosFlare2"), (int)((double)damage * 0.5), 1.25f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), mod.ProjectileType("ChaosFlare2"), (int)((double)damage * 0.5), 1.25f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
			}
			if (reaverDoubleTap) //0 - 99
			{
                if (item.ranged && !CalamityMod.throwingItemList.Contains(item.type) && Main.rand.Next(0, 100) >= 90)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("MiniRocket"), (int)((double)damage * 1.3), 2f, player.whoAmI, 0f, 0f);
                    }
                }
			}
            if (victideSet)
            {
                if ((item.ranged || item.melee || item.magic ||
                    CalamityMod.throwingItemList.Contains(item.type) ||
                    item.summon) && Main.rand.Next(10) == 0)
                {
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("Seashell"), damage * 2, 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
			return true;
		}
        #endregion

        #region MeleeEffects
        public override void MeleeEffects(Item item, Rectangle hitbox)
		{
			if (aWeapon && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
		        	int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, mod.DustType("BrimstoneFlame"), player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 0.75f);
		        }
			}
			if (aChicken && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
		        	int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 244, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 0.75f);
		        }
			}
			if (eGauntlet && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
					int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 66, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, new Color(Main.DiscoR, Main.DiscoG, Main.DiscoB), 1.25f);
		        	Main.dust[num280].noGravity = true;
		        }
			}
			if (cryogenSoul && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
		        	int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 67, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 0.75f);
		        }
			}
			if (xerocSet && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
		        	int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 58, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 1.25f);
		        }
			}
			if (reaverBlast && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
		        	int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 74, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 0.75f);
		        }
			}
			if (dsSetBonus && item.melee && !item.noMelee && !item.noUseGraphic)
			{
				if (Main.rand.Next(3) == 0)
		        {
		        	int num280 = Dust.NewDust(new Vector2((float)hitbox.X, (float)hitbox.Y), hitbox.Width, hitbox.Height, 27, player.velocity.X * 0.2f + (float)(player.direction * 3), player.velocity.Y * 0.2f, 100, default(Color), 2.5f);
		        }
			}
		}
        #endregion

        #region OnHitNPC
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            if (omegaBlueChestplate)
                target.AddBuff(mod.BuffType("CrushDepth"), 240);

            if (eGauntlet)
            {
                target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                target.AddBuff(mod.BuffType("Plague"), 120, false);
                target.AddBuff(mod.BuffType("BrimstoneFlames"), 120, false);
                if (Main.rand.Next(5) == 0)
                {
                    target.AddBuff(mod.BuffType("GlacialState"), 120, false);
                }
                target.AddBuff(mod.BuffType("GodSlayerInferno"), 120, false);
                target.AddBuff(BuffID.Poisoned, 120, false);
                target.AddBuff(BuffID.OnFire, 120, false);
                target.AddBuff(BuffID.CursedInferno, 120, false);
                target.AddBuff(BuffID.Frostburn, 120, false);
                target.AddBuff(BuffID.Ichor, 120, false);
                target.AddBuff(BuffID.Venom, 120, false);
            }
            if (aWeapon)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                }
            }
            if (abyssalAmulet)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 120, false);
                }
            }
            if (dsSetBonus)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 120, false);
                }
            }
            if (cryogenSoul || frostFlare)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(44, 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(44, 240, false);
                }
                else
                {
                    target.AddBuff(44, 120, false);
                }
            }
            if (yInsignia)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("HolyLight"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("HolyLight"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                }
            }
            if (ataxiaFire)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 720, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 480, false);
                }
                else
                {
                    target.AddBuff(BuffID.OnFire, 240, false);
                }
            }
            if (alchFlask)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("Plague"), 120, false);
                }
            }
            if (ataxiaGeyser && item.melee && !item.noMelee && !item.noUseGraphic)
            {
                if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[mod.ProjectileType("ChaosGeyser")] < 3)
                {
                    Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("ChaosGeyser"), (int)((double)item.damage * 0.15), 2f, player.whoAmI, 0f, 0f);
                }
            }

            if (target.type == NPCID.TargetDummy)
                return;

            if (bloodflareMelee)
            {
                if (bloodflareMeleeHits < 15 && bloodflareFrenzyTimer <= 0 && bloodflareFrenzyCooldown <= 0)
                {
                    bloodflareMeleeHits++;
                }
                if (player.whoAmI == Main.myPlayer)
                {
                    int healAmount = (Main.rand.Next(3) + 1);
                    player.statLife += healAmount;
                    player.HealEffect(healAmount);
                }
            }
		}
        #endregion

        #region OnHitNPCWithProj
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (omegaBlueChestplate && proj.friendly && !target.friendly)
                target.AddBuff(mod.BuffType("CrushDepth"), 240);

            if (proj.melee && silvaMelee && Main.rand.Next(4) == 0)
                target.AddBuff(mod.BuffType("SilvaStun"), 20);

            if (abyssalAmulet)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 120, false);
                }
            }
            if (dsSetBonus)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("DemonFlames"), 120, false);
                }
            }
            if (alchFlask)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("Plague"), 120, false);
                }
            }
            if (proj.melee)
            {
                if (eGauntlet)
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                    target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                    target.AddBuff(mod.BuffType("Plague"), 120, false);
                    target.AddBuff(mod.BuffType("BrimstoneFlames"), 120, false);
                    if (Main.rand.Next(5) == 0)
                    {
                        target.AddBuff(mod.BuffType("GlacialState"), 120, false);
                    }
                    target.AddBuff(mod.BuffType("GodSlayerInferno"), 120, false);
                    target.AddBuff(BuffID.Poisoned, 120, false);
                    target.AddBuff(BuffID.OnFire, 120, false);
                    target.AddBuff(BuffID.CursedInferno, 120, false);
                    target.AddBuff(BuffID.Frostburn, 120, false);
                    target.AddBuff(BuffID.Ichor, 120, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
                if (aWeapon)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(mod.BuffType("AbyssalFlames"), 360, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(mod.BuffType("AbyssalFlames"), 240, false);
                    }
                    else
                    {
                        target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                    }
                }
                if (cryogenSoul || frostFlare)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(44, 360, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(44, 240, false);
                    }
                    else
                    {
                        target.AddBuff(44, 120, false);
                    }
                }
                if (yInsignia)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(mod.BuffType("HolyLight"), 360, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(mod.BuffType("HolyLight"), 240, false);
                    }
                    else
                    {
                        target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                    }
                }
                if (ataxiaFire)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(BuffID.OnFire, 720, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(BuffID.OnFire, 480, false);
                    }
                    else
                    {
                        target.AddBuff(BuffID.OnFire, 240, false);
                    }
                }
            }
            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                if (gainLevelCooldown <= 0) //max is 12501 to avoid setting off fireworks forever
                {
                    gainLevelCooldown = 120; //2 seconds
                    if (proj.melee && meleeLevel <= 12500)
                    {
                        if (!Main.hardMode && meleeLevel >= 1500)
                        {
                            gainLevelCooldown = 1200; //20 seconds
                        }
                        if (!NPC.downedMoonlord && meleeLevel >= 5500)
                        {
                            gainLevelCooldown = 2400; //40 seconds
                        }
                        meleeLevel++;
                        if (fasterMeleeLevel && meleeLevel % 100 != 0 && Main.rand.Next(10) == 0) //add only to non-multiples of 100
                            meleeLevel++;
                        shootFireworksLevelUpMelee = true;
                        if (Main.netMode == 1)
                        {
                            LevelPacket(false, 0);
                        }
                    }
                    else if (proj.ranged && rangedLevel <= 12500)
                    {
                        if (!Main.hardMode && rangedLevel >= 1500)
                        {
                            gainLevelCooldown = 1200; //20 seconds
                        }
                        if (!NPC.downedMoonlord && rangedLevel >= 5500)
                        {
                            gainLevelCooldown = 2400; //40 seconds
                        }
                        rangedLevel++;
                        if (fasterRangedLevel && rangedLevel % 100 != 0 && Main.rand.Next(10) == 0) //add only to non-multiples of 100
                            rangedLevel++;
                        shootFireworksLevelUpRanged = true;
                        if (Main.netMode == 1)
                        {
                            LevelPacket(false, 1);
                        }
                    }
                    else if (proj.magic && magicLevel <= 12500)
                    {
                        if (!Main.hardMode && magicLevel >= 1500)
                        {
                            gainLevelCooldown = 1200; //20 seconds
                        }
                        if (!NPC.downedMoonlord && magicLevel >= 5500)
                        {
                            gainLevelCooldown = 2400; //40 seconds
                        }
                        magicLevel++;
                        if (fasterMagicLevel && magicLevel % 100 != 0 && Main.rand.Next(10) == 0) //add only to non-multiples of 100
                            magicLevel++;
                        shootFireworksLevelUpMagic = true;
                        if (Main.netMode == 1)
                        {
                            LevelPacket(false, 2);
                        }
                    }
                    else if ((proj.minion || proj.sentry || CalamityMod.projectileMinionList.Contains(proj.type)) && summonLevel <= 12500)
                    {
                        if (!Main.hardMode && summonLevel >= 1500)
                        {
                            gainLevelCooldown = 1200; //20 seconds
                        }
                        if (!NPC.downedMoonlord && summonLevel >= 5500)
                        {
                            gainLevelCooldown = 2400; //40 seconds
                        }
                        summonLevel++;
                        if (fasterSummonLevel && summonLevel % 100 != 0 && Main.rand.Next(10) == 0) //add only to non-multiples of 100
                            summonLevel++;
                        shootFireworksLevelUpSummon = true;
                        if (Main.netMode == 1)
                        {
                            LevelPacket(false, 3);
                        }
                    }
                    else if (CalamityMod.throwingProjectileList.Contains(proj.type) && !proj.melee && rogueLevel <= 12500)
                    {
                        if (!Main.hardMode && rogueLevel >= 1500)
                        {
                            gainLevelCooldown = 1200; //20 seconds
                        }
                        if (!NPC.downedMoonlord && rogueLevel >= 5500)
                        {
                            gainLevelCooldown = 2400; //40 seconds
                        }
                        rogueLevel++;
                        if (fasterRogueLevel && rogueLevel % 100 != 0 && Main.rand.Next(10) == 0) //add only to non-multiples of 100
                            rogueLevel++;
                        shootFireworksLevelUpRogue = true;
                        if (Main.netMode == 1)
                        {
                            LevelPacket(false, 4);
                        }
                    }
                }
                if (raiderTalisman && raiderStack < 250 && CalamityMod.throwingProjectileList.Contains(proj.type) && !proj.melee && crit)
                {
                    raiderStack++;
                }
                if (CalamityWorld.revenge)
                {
                    if ((proj.melee && proj.ownerHitCheck) || proj.aiStyle == 15 || (proj.aiStyle == 75 && proj.melee) ||
                    proj.type == mod.ProjectileType("Nebulash") || proj.type == mod.ProjectileType("CosmicDischarge") ||
                    proj.type == mod.ProjectileType("MourningStar"))
                    {
                        int stressGain = (int)((double)damage * 0.1);
                        int stressMaxGain = 10;
                        if (stressGain < 1)
                        {
                            stressGain = 1;
                        }
                        if (stressGain > stressMaxGain)
                        {
                            stressGain = stressMaxGain;
                        }
                        stress += stressGain;
                        if (stress >= stressMax)
                        {
                            stress = stressMax;
                        }
                    }
                }
            }
        }
        #endregion

        #region PvP
        public override void OnHitPvp(Item item, Player target, int damage, bool crit)
        {
            if (omegaBlueChestplate)
                target.AddBuff(mod.BuffType("CrushDepth"), 240);

            if (eGauntlet)
            {
                target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                target.AddBuff(mod.BuffType("Plague"), 120, false);
                target.AddBuff(mod.BuffType("BrimstoneFlames"), 120, false);
                if (Main.rand.Next(5) == 0)
                {
                    target.AddBuff(mod.BuffType("GlacialState"), 120, false);
                }
                target.AddBuff(mod.BuffType("GodSlayerInferno"), 120, false);
                target.AddBuff(BuffID.Poisoned, 120, false);
                target.AddBuff(BuffID.OnFire, 120, false);
                target.AddBuff(BuffID.CursedInferno, 120, false);
                target.AddBuff(BuffID.Frostburn, 120, false);
                target.AddBuff(BuffID.Ichor, 120, false);
                target.AddBuff(BuffID.Venom, 120, false);
            }
            if (aWeapon)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                }
            }
            if (abyssalAmulet)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 120, false);
                }
            }
            if (cryogenSoul || frostFlare)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(44, 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(44, 240, false);
                }
                else
                {
                    target.AddBuff(44, 120, false);
                }
            }
            if (yInsignia)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("HolyLight"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("HolyLight"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                }
            }
            if (ataxiaFire)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 720, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(BuffID.OnFire, 480, false);
                }
                else
                {
                    target.AddBuff(BuffID.OnFire, 240, false);
                }
            }
            if (alchFlask)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("Plague"), 120, false);
                }
            }
        }

        public override void OnHitPvpWithProj(Projectile proj, Player target, int damage, bool crit)
        {
            if (omegaBlueChestplate)
                target.AddBuff(mod.BuffType("CrushDepth"), 240);

            if (proj.melee && silvaMelee && Main.rand.Next(4) == 0)
                target.AddBuff(mod.BuffType("SilvaStun"), 20);

            if (abyssalAmulet)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("CrushDepth"), 120, false);
                }
            }
            if (alchFlask)
            {
                if (Main.rand.Next(4) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 360, false);
                }
                else if (Main.rand.Next(2) == 0)
                {
                    target.AddBuff(mod.BuffType("Plague"), 240, false);
                }
                else
                {
                    target.AddBuff(mod.BuffType("Plague"), 120, false);
                }
            }
            if (proj.melee)
            {
                if (eGauntlet)
                {
                    target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                    target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                    target.AddBuff(mod.BuffType("Plague"), 120, false);
                    target.AddBuff(mod.BuffType("BrimstoneFlames"), 120, false);
                    if (Main.rand.Next(5) == 0)
                    {
                        target.AddBuff(mod.BuffType("GlacialState"), 120, false);
                    }
                    target.AddBuff(mod.BuffType("GodSlayerInferno"), 120, false);
                    target.AddBuff(BuffID.Poisoned, 120, false);
                    target.AddBuff(BuffID.OnFire, 120, false);
                    target.AddBuff(BuffID.CursedInferno, 120, false);
                    target.AddBuff(BuffID.Frostburn, 120, false);
                    target.AddBuff(BuffID.Ichor, 120, false);
                    target.AddBuff(BuffID.Venom, 120, false);
                }
                if (aWeapon)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(mod.BuffType("AbyssalFlames"), 360, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(mod.BuffType("AbyssalFlames"), 240, false);
                    }
                    else
                    {
                        target.AddBuff(mod.BuffType("AbyssalFlames"), 120, false);
                    }
                }
                if (cryogenSoul || frostFlare)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(44, 360, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(44, 240, false);
                    }
                    else
                    {
                        target.AddBuff(44, 120, false);
                    }
                }
                if (yInsignia)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(mod.BuffType("HolyLight"), 360, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(mod.BuffType("HolyLight"), 240, false);
                    }
                    else
                    {
                        target.AddBuff(mod.BuffType("HolyLight"), 120, false);
                    }
                }
                if (ataxiaFire)
                {
                    if (Main.rand.Next(4) == 0)
                    {
                        target.AddBuff(BuffID.OnFire, 720, false);
                    }
                    else if (Main.rand.Next(2) == 0)
                    {
                        target.AddBuff(BuffID.OnFire, 480, false);
                    }
                    else
                    {
                        target.AddBuff(BuffID.OnFire, 240, false);
                    }
                }
            }
        }
        #endregion

        #region ModifyHitNPC
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (silvaMelee && Main.rand.Next(4) == 0)
            {
                damage *= 5;
            }
            if (CalamityWorld.revenge)
            {
                if (rageMode && adrenalineMode)
                {
                    if (item.melee)
                    {
                        damage = (int)((double)damage * (CalamityWorld.death ? 9.0 : 3.0));
                    }
                }
                else if (rageMode)
                {
                    if (item.melee)
                    {
                        double rageDamageBoost = 0.0 +
                            (rageBoostOne ? (CalamityWorld.death ? 0.6 : 0.15) : 0.0) + //3.6 or 1.65
                            (rageBoostTwo ? (CalamityWorld.death ? 0.6 : 0.15) : 0.0) + //4.2 or 1.8
                            (rageBoostThree ? (CalamityWorld.death ? 0.6 : 0.15) : 0.0); //4.8 or 1.95
                        double rageDamage = (CalamityWorld.death ? 3.0 : 1.5) + rageDamageBoost;
                        damage = (int)((double)damage * rageDamage);
                    }
                }
                else if (adrenalineMode)
                {
                    if (item.melee)
                    {
                        damage = (int)((double)damage * ((CalamityWorld.death ? 7.0 : 2.5) * adrenalineDmgMult));
                    }
                }
            }
            if ((target.damage > 5 || target.boss) && player.whoAmI == Main.myPlayer && !target.SpawnedFromStatue)
            {
                if (gainLevelCooldown <= 0)
                {
                    gainLevelCooldown = 120;
                    if (item.melee && meleeLevel <= 12500)
                    {
                        if (!Main.hardMode && meleeLevel >= 1500)
                        {
                            gainLevelCooldown = 1200; //20 seconds
                        }
                        if (!NPC.downedMoonlord && meleeLevel >= 5500)
                        {
                            gainLevelCooldown = 2400; //40 seconds
                        }
                        meleeLevel++;
                        if (fasterMeleeLevel && meleeLevel % 100 != 0 && Main.rand.Next(10) == 0) //add only to non-multiples of 100
                            meleeLevel++;
                        shootFireworksLevelUpMelee = true;
                        if (Main.netMode == 1)
                        {
                            LevelPacket(false, 0);
                        }
                    }
                }
                if (CalamityWorld.revenge)
                {
                    if (item.shoot == 0 || item.shoot == mod.ProjectileType("NobodyKnows"))
                    {
                        int stressGain = (int)((double)damage * 0.1);
                        int stressMaxGain = 10;
                        if (stressGain < 1)
                        {
                            stressGain = 1;
                        }
                        if (stressGain > stressMaxGain)
                        {
                            stressGain = stressMaxGain;
                        }
                        stress += stressGain;
                        if (stress >= stressMax)
                        {
                            stress = stressMax;
                        }
                    }
                }
            }
            if (astralStarRain && crit && astralStarRainCooldown <= 0)
			{
				if (player.whoAmI == Main.myPlayer)
				{
                    astralStarRainCooldown = 60;
					for (int n = 0; n < 3; n++)
					{
						float x = target.position.X + (float)Main.rand.Next(-400, 400);
						float y = target.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 vector = new Vector2(x, y);
						float num13 = target.position.X + (float)(target.width / 2) - vector.X;
						float num14 = target.position.Y + (float)(target.height / 2) - vector.Y;
						num13 += (float)Main.rand.Next(-100, 101);
						int num15 = 25;
						int projectileType = Main.rand.Next(3);
						if (projectileType == 0)
						{
							projectileType = mod.ProjectileType("AstralStar");
						}
						else if (projectileType == 1)
						{
							projectileType = 92;
						}
						else
						{
							projectileType = 12;
						}
						float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
						num16 = (float)num15 / num16;
						num13 *= num16;
						num14 *= num16;
						int num17 = Projectile.NewProjectile(x, y, num13, num14, projectileType, 75, 5f, player.whoAmI, 0f, 0f);
						Main.projectile[num17].ranged = false;
					}
				}
			}
		}

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (auricSet && silvaThrowing && CalamityMod.throwingProjectileList.Contains(proj.type) && !proj.melee && 
                crit && player.statLife > (int)((double)player.statLifeMax2 * 0.9))
            {
                damage = (int)((double)damage * 3.0);
            }
            if (auricSet && silvaMelee && proj.melee)
            {
                double multiplier = (double)player.statLife / (double)player.statLifeMax2;
                if (multiplier < 0.5)
                {
                    multiplier = 0.5;
                }
                damage = (int)(((double)damage * multiplier) * 2.0); //ranges from 2.0 times to 1 times
            }
            if (godSlayerRanged && crit && proj.ranged)
            {
                int randomChance = 100 - player.rangedCrit; //100 min to 15 max with cap
                if (randomChance < 15)
                {
                    randomChance = 15;
                }
                if (Main.rand.Next(randomChance) == 0)
                {
                    damage *= 2;
                }
            }
            if (tarraMage && crit && proj.magic)
            {
                tarraCrits++;
            }
            if (tarraRanged && crit && proj.ranged && player.whoAmI == Main.myPlayer)
            {
                int num251 = Main.rand.Next(2, 4);
                for (int num252 = 0; num252 < num251; num252++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    int FUCKYOU = Projectile.NewProjectile(target.position.X + (float)(target.width / 2), target.position.Y + (float)(target.height / 2), 
                        value15.X, value15.Y, 206, (int)((double)damage * (auricSet ? 0.4 : 0.25)), 0f, player.whoAmI, 0f, 0f);
                    Main.projectile[FUCKYOU].magic = false;
                    Main.projectile[FUCKYOU].netUpdate = true;
                }
            }
            if (tarraThrowing && tarraThrowingCritTimer <= 0 && tarraThrowingCrits < 25 && crit && 
                CalamityMod.throwingProjectileList.Contains(proj.type) && !proj.melee)
            {
                tarraThrowingCrits++;
            }
            if (bloodflareThrowing && CalamityMod.throwingProjectileList.Contains(proj.type) && !proj.melee && crit && Main.rand.Next(2) == 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    float num11 = 0.03f;
                    num11 -= (float)proj.numHits * 0.015f;
                    if (num11 < 0f)
                    {
                        num11 = 0f;
                    }
                    float num12 = (float)proj.damage * num11;
                    if (num12 < 0f)
                    {
                        num12 = 0f;
                    }
                    if (player.lifeSteal > 0f)
                    {
                        player.statLife += 1;
                        player.HealEffect(1);
                        player.lifeSteal -= num12 * 2f;
                    }
                }
            }
            if (bloodflareMage && bloodflareMageCooldown <= 0 && crit && proj.magic && player.whoAmI == Main.myPlayer)
            {
                bloodflareMageCooldown = 120;
                for (int i = 0; i < 3; i++)
                {
                    Vector2 value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    while (value15.X == 0f && value15.Y == 0f)
                    {
                        value15 = new Vector2((float)Main.rand.Next(-100, 101), (float)Main.rand.Next(-100, 101));
                    }
                    value15.Normalize();
                    value15 *= (float)Main.rand.Next(70, 101) * 0.1f;
                    int fire = Projectile.NewProjectile(target.position.X + (float)(target.width / 2), target.position.Y + (float)(target.height / 2),
                        value15.X, value15.Y, 15, (int)((double)damage * (auricSet ? 11.0 : 6.0)), 0f, player.whoAmI, 0f, 0f);
                    Main.projectile[fire].magic = false;
                    Main.projectile[fire].netUpdate = true;
                }
            }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaRanged && proj.ranged)
            {
                damage = (int)((double)damage * 1.4);
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaThrowing && CalamityMod.throwingProjectileList.Contains(proj.type) && !proj.melee)
            {
                damage = (int)((double)damage * 1.1);
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaMage && proj.magic)
            {
                damage = (int)((double)damage * 1.1);
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaSummon && proj.minion)
            {
                damage = (int)((double)damage * 1.1);
            }
        }
        #endregion

        #region ModifyHitByNPC
        public override void ModifyHitByNPC(NPC npc, ref int damage, ref bool crit)
        {
            if (fleshTotem && fleshTotemCooldown <= 0)
            {
                fleshTotemCooldown = 900; //15 seconds
                damage /= 2;
            }
            if (tarraDefense && tarraMelee)
            {
                damage /= 2;
            }
            if (bloodflareMelee && bloodflareFrenzyTimer > 0)
            {
                damage /= 2;
            }
            if (silvaMelee && silvaCountdown <= 0 && hasSilvaEffect)
            {
                damage /= 2;
            }
            if (CalamityWorld.revenge)
            {
                if (CalamityMod.revengeanceEnemyBuffList.Contains(npc.type))
                    damage = (int)((double)damage * 1.25);
            }
            if (player.whoAmI == Main.myPlayer && !player.immune && !lol && !invincible && !hasSilvaEffect)
            {
                if (CalamityWorld.revenge && !npc.SpawnedFromStatue)
                {
                    int stressGain = (int)((double)damage * 2.5);
                    int stressMaxGain = 2500;
                    if (stressGain < 1)
                    {
                        stressGain = 1;
                    }
                    if (stressGain > stressMaxGain)
                    {
                        stressGain = stressMaxGain;
                    }
                    stress += stressGain;
                    if (stress >= stressMax)
                    {
                        stress = stressMax;
                    }
                }
            }
        }
        #endregion

        #region ModifyHitByProj
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (proj.type == ProjectileID.Nail)
            {
                damage = (int)((double)damage * 0.75);
            }
            if (cEnergy && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                damage = (int)((double)damage * 0.75);
            }
            if (beeResist)
            {
                if (CalamityMod.beeProjectileList.Contains(proj.type))
                    damage = (int)((double)damage * 0.75);
            }
            if (Main.hardMode && !CalamityWorld.spawnedHardBoss && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                if (CalamityMod.hardModeNerfList.Contains(proj.type))
                    damage = (int)((double)damage * 0.75);
            }
            if (CalamityWorld.downedDoG && (Main.pumpkinMoon || Main.snowMoon || Main.eclipse))
            {
                if (CalamityMod.eventProjectileBuffList.Contains(proj.type))
                    damage = (int)((double)damage * 1.5);
            }
            if (CalamityWorld.revenge)
            {
                if (CalamityMod.revengeanceProjectileBuffList.Contains(proj.type))
                    damage = (int)((double)damage * 1.25);
            }
            if (player.whoAmI == Main.myPlayer && !player.immune && !lol && !invincible && !hasSilvaEffect)
            {
                if (CalamityWorld.revenge && !CalamityMod.trapProjectileList.Contains(proj.type))
                {
                    int stressGain = (int)((double)damage * 2.5);
                    int stressMaxGain = 2500;
                    if (stressGain < 1)
                    {
                        stressGain = 1;
                    }
                    if (stressGain > stressMaxGain)
                    {
                        stressGain = stressMaxGain;
                    }
                    stress += stressGain;
                    if (stress >= stressMax)
                    {
                        stress = stressMax;
                    }
                }
            }
        }
        #endregion

        #region OnHit
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (CalamityWorld.revenge)
            {
                if (npc.type == NPCID.SkeletronPrime || npc.type == NPCID.PrimeVice || npc.type == NPCID.PrimeSaw)
                {
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.Spazmatism && npc.ai[0] != 1f && npc.ai[0] != 2f && npc.ai[0] != 0f)
                {
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.Plantera && npc.life < npc.lifeMax / 2)
                {
                    player.AddBuff(BuffID.Poisoned, 180);
                    player.AddBuff(BuffID.Bleeding, 300);
                }
                else if (npc.type == NPCID.PlanterasTentacle)
                {
                    player.AddBuff(BuffID.Poisoned, 90);
                    player.AddBuff(BuffID.Bleeding, 150);
                }
                else if (npc.type == NPCID.Golem || npc.type == NPCID.GolemFistLeft || npc.type == NPCID.GolemFistRight ||
                    npc.type == NPCID.GolemHead || npc.type == NPCID.GolemHeadFree)
                {
                    player.AddBuff(BuffID.BrokenArmor, 180);
                }
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (CalamityWorld.revenge && proj.hostile)
            {
                if (proj.type == ProjectileID.FrostBeam && !player.frozen && !gState)
                {
                    player.AddBuff(mod.BuffType("GlacialState"), 120);
                }
                else if (proj.type == ProjectileID.DeathLaser)
                {
                    player.AddBuff(BuffID.OnFire, 180);
                }
                else if (proj.type == ProjectileID.Skull)
                {
                    player.AddBuff(BuffID.Weak, 120);
                }
                else if (proj.type == ProjectileID.ThornBall)
                {
                    player.AddBuff(BuffID.Poisoned, 180);
                }
                else if (proj.type == ProjectileID.CultistBossIceMist)
                {
                    player.AddBuff(BuffID.Frozen, 60);
                    player.AddBuff(BuffID.Chilled, 120);
                    player.AddBuff(BuffID.Frostburn, 240);
                }
                else if (proj.type == ProjectileID.CultistBossLightningOrbArc)
                {
                    player.AddBuff(BuffID.Electrified, 120);
                }
            }
            if (projRef && proj.active && !proj.friendly && proj.hostile && damage > 0 && Main.rand.Next(20) == 0)
            {
                player.statLife += damage;
                player.HealEffect(damage);
                proj.hostile = false;
                proj.friendly = true;
                proj.velocity.X = -proj.velocity.X;
                proj.velocity.Y = -proj.velocity.Y;
            }
            if (daedalusReflect && proj.active && !proj.friendly && proj.hostile && damage > 0 && Main.rand.Next(3) == 0)
            {
                int healAmt = damage / 5;
                player.statLife += healAmt;
                player.HealEffect(healAmt);
                proj.hostile = false;
                proj.friendly = true;
                proj.velocity.X = -proj.velocity.X;
                proj.velocity.Y = -proj.velocity.Y;
            }
        }
        #endregion

        #region Fishing
        public override void CatchFish(Item fishingRod, Item bait, int power, int liquidType, int poolSize, int worldLayer, int questFish, ref int caughtType, ref bool junk)
		{
            Point point = player.Center.ToTileCoordinates();
            bool abyssPosX = false;
            if (CalamityWorld.abyssSide)
            {
                if (point.X < 380)
                {
                    abyssPosX = true;
                }
            }
            else
            {
                if (point.X > Main.maxTilesX - 380)
                {
                    abyssPosX = true;
                }
            }
            if (junk)
			{
                if (abyssPosX && liquidType == 0 && power < 40)
                {
                    caughtType = mod.ItemType("PlantyMush");
                }
                return;
			}
            /*if (abyssPosX && liquidType == 0 && (bait.type == ItemID.GoldWorm || bait.type == ItemID.GoldGrasshopper || bait.type == ItemID.GoldButterfly) && power > 150)
            {
                if (Main.netMode != 1)
                {
                    CalamityGlobalNPC.OldDukeSpawn(player.whoAmI, mod.NPCType("OldDuke"));
                }
                else
                {
                    NetMessage.SendData(61, -1, -1, null, player.whoAmI, (float)mod.NPCType("OldDuke"), 0f, 0f, 0, 0, 0);
                }
                switch (Main.rand.Next(4))
                {
                    case 0: caughtType = mod.ItemType("IronBoots"); break; //movement acc
                    case 1: caughtType = mod.ItemType("DepthCharm"); break; //regen acc
                    case 2: caughtType = mod.ItemType("AnechoicPlating"); break; //defense acc
                    case 3: caughtType = mod.ItemType("StrangeOrb"); break; //light pet
                }
                return;
            }*/
            if (power >= 20)
            {
                if (power >= 40)
                {
                    if (abyssPosX && liquidType == 0 && Main.rand.Next(15) == 0 && power < 80)
                    {
                        caughtType = mod.ItemType("PlantyMush");
                    }
                    if (power >= 60)
                    {
                        if (player.FindBuffIndex(BuffID.Gills) > -1 && NPC.downedPlantBoss && liquidType == 0 && Main.rand.Next(25) == 0 && power < 160)
                        {
                            caughtType = mod.ItemType("Floodtide");
                        }
                        if (abyssPosX && liquidType == 0 && Main.rand.Next(25) == 0 && power < 160)
                        {
                            caughtType = mod.ItemType("AlluringBait");
                        }
                        if (power >= 80)
                        {
                            if (abyssPosX && Main.hardMode && liquidType == 0 && Main.rand.Next(15) == 0 && power < 210)
                            {
                                switch (Main.rand.Next(4))
                                {
                                    case 0: caughtType = mod.ItemType("IronBoots"); break; //movement acc
                                    case 1: caughtType = mod.ItemType("DepthCharm"); break; //regen acc
                                    case 2: caughtType = mod.ItemType("AnechoicPlating"); break; //defense acc
                                    case 3: caughtType = mod.ItemType("StrangeOrb"); break; //light pet
                                }
                            }
                            if (power >= 110)
                            {
                                if (abyssPosX && liquidType == 0 && Main.rand.Next(25) == 0 && power < 240)
                                {
                                    caughtType = mod.ItemType("AbyssalAmulet");
                                }
                            }
                        }
                    }
                }
            }
		}
        #endregion

        #region FrameEffects
        public override void FrameEffects()
        {
            if ((snowmanPower || snowmanForce) && !snowmanHide)
            {
                player.legs = mod.GetEquipSlot("PopoLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("PopoBody", EquipType.Body);
                player.head = (snowmanNoseless ? mod.GetEquipSlot("PopoNoselessHead", EquipType.Head) : mod.GetEquipSlot("PopoHead", EquipType.Head));
            }
            else if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
            {
                player.legs = mod.GetEquipSlot("AbyssalDivingSuitLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("AbyssalDivingSuitBody", EquipType.Body);
                player.head = mod.GetEquipSlot("AbyssalDivingSuitHead", EquipType.Head);
            }
            else if ((sirenBoobsPower || sirenBoobsForce) && !sirenBoobsHide)
            {
                player.legs = mod.GetEquipSlot("SirenLeg", EquipType.Legs);
                player.body = mod.GetEquipSlot("SirenBody", EquipType.Body);
                player.head = mod.GetEquipSlot("SirenHead", EquipType.Head);
            }
            else if ((sirenBoobsAltPower || sirenBoobsAltForce) && !sirenBoobsAltHide)
            {
                player.legs = mod.GetEquipSlot("SirenLegAlt", EquipType.Legs);
                player.body = mod.GetEquipSlot("SirenBodyAlt", EquipType.Body);
                player.head = mod.GetEquipSlot("SirenHeadAlt", EquipType.Head);
            }
            if (NOU)
                NOULOL();
            if (CalamityWorld.defiled)
                Defiled();
            if (weakPetrification)
                WeakPetrification();
        }
        #endregion

        #region NOULOL
        private void NOULOL()
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
            NOU = true;
        }

        private void WeakPetrification()
        {
            player.doubleJumpCloud = false;
            player.doubleJumpSandstorm = false;
            player.doubleJumpBlizzard = false;
            player.rocketBoots = 0;
            player.jumpBoost = false;
            player.slowFall = false;
            player.gravControl = false;
            player.gravControl2 = false;
            player.jumpSpeedBoost = 0f;
            player.wingTimeMax = (int)((double)player.wingTimeMax * 0.5);
            player.balloon = -1;
            weakPetrification = true;
        }

        private void Defiled()
        {
            player.wingTimeMax = 0;
        }
        #endregion

        #region HurtMethods
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
            if (CalamityWorld.armageddon)
            {
                if (areThereAnyDamnBosses)
                {
                    KillPlayer();
                }
            }
            if (lol || (invincible && player.inventory[player.selectedItem].type != mod.ItemType("ColdheartIcicle")))
            {
                return false;
            }
            if (godSlayerReflect && Main.rand.Next(50) == 0)
            {
                return false;
            }
            if (player.beetleDefense && player.beetleOrbs > 0)
            {
                float newMult = 0.05f * (float)player.beetleOrbs; //.05 .1 .15
                damage = (int)((double)(1f + newMult) * (double)damage); //damage added for one orb : 100 damage = 1.05 * 100 = 105
            }
            if (CalamityGlobalNPC.DraedonMayhem)
            {
                damage = (int)((double)damage * 1.2);
            }
            if (dArtifact)
            {
                damage = (int)((double)damage * 1.25);
            }
            if (CalamityWorld.bossRushActive)
            {
                switch (CalamityWorld.bossRushStage)
                {
                    case 0:
                        damage = (int)((double)damage * 5.0); //Tier 1 Queen Bee values adjusted for a median of 250 damage
                        break;
                    case 1:
                        damage = (int)((double)damage * 4.0); //BoC
                        break;
                    case 2:
                        damage = (int)((double)damage * 4.0); //King Slime
                        break;
                    case 3:
                        damage = (int)((double)damage * 6.0); //EoC
                        break;
                    case 4:
                        damage = (int)((double)damage * 2.5); //Prime
                        break;
                    case 5:
                        damage = (int)((double)damage * 2.5); //Golem
                        break;
                    case 6:
                        damage = (int)((double)damage * 1.75); //Guardians
                        break;
                    case 7:
                        damage = (int)((double)damage * 5.0); //EoW
                        break;
                    case 8:
                        damage = (int)((double)damage * 2.0); //Tier 2 Astrageldon values adjusted for a median of 300 damage
                        break;
                    case 9:
                        damage = (int)((double)damage * 3.0); //Destroyer
                        break;
                    case 10:
                        damage = (int)((double)damage * 3.0); //Twins
                        break;
                    case 11:
                        damage = (int)((double)damage * 1.0); //Birb
                        break;
                    case 12:
                        damage = (int)((double)damage * 4.0); //WoF
                        break;
                    case 13:
                        damage = (int)((double)damage * 4.5); //Hive Mind
                        break;
                    case 14:
                        damage = (int)((double)damage * 4.0); //Skeletron
                        break;
                    case 15:
                        damage = (int)((double)damage * 1.25); //Storm Weaver
                        break;
                    case 16:
                        damage = (int)((double)damage * 3.0); //Tier 3 Aquatic Scourge values adjusted for a median of 350 damage
                        break;
                    case 17:
                        damage = (int)((double)damage * 6.0); //Desert Scourge
                        break;
                    case 18:
                        damage = (int)((double)damage * 3.5); //Cultist
                        break;
                    case 19:
                        damage = (int)((double)damage * 4.5); //Crabulon
                        break;
                    case 20:
                        damage = (int)((double)damage * 3.5); //Plantera
                        break;
                    case 21:
                        damage = (int)((double)damage * 2.0); //Void
                        break;
                    case 22:
                        damage = (int)((double)damage * 5.0); //Perfs
                        break;
                    case 23:
                        damage = (int)((double)damage * 4.0); //Cryogen
                        break;
                    case 24:
                        damage = (int)((double)damage * 4.0); //Tier 4 Brimstone Elemental values adjusted for a median of 400 damage
                        break;
                    case 25:
                        damage = (int)((double)damage * 2.0); //Signus
                        break;
                    case 26:
                        damage = (int)((double)damage * 2.5); //Ravager
                        break;
                    case 27:
                        damage = (int)((double)damage * 2.0); //Fishron
                        break;
                    case 28:
                        damage = (int)((double)damage * 2.5); //Moon Lord
                        break;
                    case 29:
                        damage = (int)((double)damage * 2.75); //Astrum Deus
                        break;
                    case 30:
                        damage = (int)((double)damage * 2.0); //Polter
                        break;
                    case 31:
                        damage = (int)((double)damage * 2.5); //Plaguebringer
                        break;
                    case 32:
                        damage = (int)((double)damage * 4.0); //Tier 5 Calamitas values adjusted for a median of 450 to 500 damage
                        break;
                    case 33:
                        damage = (int)((double)damage * 3.75); //Levi and Siren
                        break;
                    case 34:
                        damage = (int)((double)damage * 5.25); //Slime God
                        break;
                    case 35:
                        damage = (int)((double)damage * 2.5); //Providence
                        break;
                    case 36:
                        damage = (int)((double)damage * 1.0); //SCal
                        break;
                    case 37:
                        damage = (int)((double)damage * 1.1); //Yharon
                        break;
                    case 38:
                        damage = (int)((double)damage * 2.0); //DoG
                        break;
                }
            }
            if (enraged)
            {
                damage = (int)((double)damage * 1.5);
            }
            if (CalamityWorld.revenge)
			{
                if (player.chaosState)
                {
                    damage = (int)((double)damage * 1.25);
                }
                if (player.ichor)
                {
                    damage = (int)((double)damage * 1.25);
                }
                else if (player.onFire2)
                {
                    damage = (int)((double)damage * 1.2);
                }
                customDamage = true;
				float damageMult = CalamityWorld.downedBossAny ? 1f : 0.8f;
                if (CalamityWorld.death)
                {
                    damageMult = 1.25f;
                }
				damage = (int)((double)damage * damageMult);
				double newDamage = (double)damage - (double)player.statDefense * (Main.hardMode ? 0.75 : 0.5);
				double newDamageLimit = 5.0 + (Main.hardMode ? 5.0 : 0.0) + (NPC.downedPlantBoss ? 5.0 : 0.0) + (NPC.downedMoonlord ? 5.0 : 0.0); //5, 10, 15, 20
				if (newDamage < newDamageLimit)
				{
					newDamage = newDamageLimit;
				}
				damage = (int)newDamage;
			}
            if (bloodPact)
            {
                if (Main.rand.Next(4) == 0)
                {
                    damage = (int)((double)damage * 3.0);
                }
            }
            if (godSlayerDamage && damage <= 80)
            {
                damage = 1;
            }
            if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
            {
                playSound = false;
                Main.PlaySound(3, (int)player.position.X, (int)player.position.Y, 4, 1f, 0f); //metal hit noise
            }
            else if (((sirenBoobsPower || sirenBoobsForce) && !sirenBoobsHide) || ((sirenBoobsAltPower || sirenBoobsAltForce) && !sirenBoobsAltHide))
            {
                playSound = false;
                Main.PlaySound(20, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f); //female hit noise
            }
            if (abyssalDivingSuitPlates)
            {
                damage = (int)((double)damage * 0.85);
            }
            if (sirenIce)
            {
                damage = (int)((double)damage * 0.8);
            }
            if (auricSet || silvaSet)
            {
                damage = (int)((double)damage * 0.95);
            }
            if (revivifyTimer > 0)
            {
                int healAmt = damage / 20;
                player.statLife += healAmt;
                player.HealEffect(healAmt);
            }
            if (daedalusAbsorb && Main.rand.Next(10) == 0)
			{
				int healAmt = damage / 2;
				player.statLife += healAmt;
    			player.HealEffect(healAmt);
			}
			if (absorber)
			{
				int healAmt = damage / 20;
				player.statLife += healAmt;
    			player.HealEffect(healAmt);
			}
            if (CalamityGlobalNPC.SCal > -1)
            {
                int newDamageLimit = 300;
                if (damage < newDamageLimit)
                {
                    damage = newDamageLimit;
                }
            }
			return true;
		}
		
		public override void Hurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
			modStealth = 1f;
            if (player.whoAmI == Main.myPlayer && rageMode)
            {
                stress = 0;
                if (player.FindBuffIndex(mod.BuffType("RageMode")) > -1) { player.ClearBuff(mod.BuffType("RageMode")); }
            }
            if (player.whoAmI == Main.myPlayer && !adrenalineMode && adrenaline != adrenalineMax)
            {
                adrenaline = 0;
            }
            if (player.whoAmI == Main.myPlayer && gShell && !player.panic)
			{
				player.AddBuff(mod.BuffType("ShellBoost"), 300);
			}
            if (player.whoAmI == Main.myPlayer && abyssalDivingSuitPlates && damage > 50)
            {
                abyssalDivingSuitPlateHits++;
                if (abyssalDivingSuitPlateHits >= 3)
                {
                    Main.PlaySound(4, (int)player.position.X, (int)player.position.Y, 14);
                    player.AddBuff(mod.BuffType("AbyssalDivingSuitPlatesBroken"), 10830);
                    for (int num621 = 0; num621 < 20; num621++)
                    {
                        int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 31, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num622].velocity *= 3f;
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num622].scale = 0.5f;
                            Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                    for (int num623 = 0; num623 < 35; num623++)
                    {
                        int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 6, 0f, 0f, 100, default(Color), 3f);
                        Main.dust[num624].noGravity = true;
                        Main.dust[num624].velocity *= 5f;
                        num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 6, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num624].velocity *= 2f;
                    }
                    for (int num625 = 0; num625 < 3; num625++)
                    {
                        float scaleFactor10 = 0.33f;
                        if (num625 == 1)
                        {
                            scaleFactor10 = 0.66f;
                        }
                        if (num625 == 2)
                        {
                            scaleFactor10 = 1f;
                        }
                        int num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Main.gore[num626].velocity *= scaleFactor10;
                        Gore expr_13AB6_cp_0 = Main.gore[num626];
                        expr_13AB6_cp_0.velocity.X = expr_13AB6_cp_0.velocity.X + 1f;
                        Gore expr_13AD6_cp_0 = Main.gore[num626];
                        expr_13AD6_cp_0.velocity.Y = expr_13AD6_cp_0.velocity.Y + 1f;
                        num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Main.gore[num626].velocity *= scaleFactor10;
                        Gore expr_13B79_cp_0 = Main.gore[num626];
                        expr_13B79_cp_0.velocity.X = expr_13B79_cp_0.velocity.X - 1f;
                        Gore expr_13B99_cp_0 = Main.gore[num626];
                        expr_13B99_cp_0.velocity.Y = expr_13B99_cp_0.velocity.Y + 1f;
                        num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Main.gore[num626].velocity *= scaleFactor10;
                        Gore expr_13C3C_cp_0 = Main.gore[num626];
                        expr_13C3C_cp_0.velocity.X = expr_13C3C_cp_0.velocity.X + 1f;
                        Gore expr_13C5C_cp_0 = Main.gore[num626];
                        expr_13C5C_cp_0.velocity.Y = expr_13C5C_cp_0.velocity.Y - 1f;
                        num626 = Gore.NewGore(new Vector2(player.position.X + (float)(player.width / 2) - 24f, player.position.Y + (float)(player.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                        Main.gore[num626].velocity *= scaleFactor10;
                        Gore expr_13CFF_cp_0 = Main.gore[num626];
                        expr_13CFF_cp_0.velocity.X = expr_13CFF_cp_0.velocity.X - 1f;
                        Gore expr_13D1F_cp_0 = Main.gore[num626];
                        expr_13D1F_cp_0.velocity.Y = expr_13D1F_cp_0.velocity.Y - 1f;
                    }
                }
            }
            if (player.whoAmI == Main.myPlayer && sirenIce)
            {
                Main.PlaySound(4, (int)player.position.X, (int)player.position.Y, 7);
                player.AddBuff(mod.BuffType("IceShieldBrokenBuff"), 1800);
                for (int num621 = 0; num621 < 10; num621++)
                {
                    int num622 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num622].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num622].scale = 0.5f;
                        Main.dust[num622].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int num623 = 0; num623 < 15; num623++)
                {
                    int num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[num624].noGravity = true;
                    Main.dust[num624].velocity *= 5f;
                    num624 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 67, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num624].velocity *= 2f;
                }
            }
            if (tarraMelee && player.whoAmI == Main.myPlayer)
            {
                if (Main.rand.Next(4) == 0)
                {
                    player.AddBuff(mod.BuffType("TarraLifeRegen"), Main.rand.Next(120, 360));
                }
            }
            if (player.whoAmI == Main.myPlayer && xerocSet)
			{
				player.AddBuff(mod.BuffType("XerocRage"), 240);
        		player.AddBuff(mod.BuffType("XerocWrath"), 240);
			}
			else if (player.whoAmI == Main.myPlayer && reaverBlast)
			{
				player.AddBuff(mod.BuffType("ReaverRage"), 180);
			}
			if ((fBarrier || ((sirenBoobs || sirenBoobsAlt) && NPC.downedBoss3)) && Main.myPlayer == player.whoAmI)
			{
				Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 27);
				for (int m = 0; m < 200; m++)
				{
					if (Main.npc[m].active && !Main.npc[m].friendly)
					{
						float distance = (Main.npc[m].Center - player.Center).Length();
						float num10 = (float)Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
						if (num10 > 500f)
						{
							num10 = 500f + (num10 - 500f) * 0.75f;
						}
						if (num10 > 700f)
						{
							num10 = 700f + (num10 - 700f) * 0.5f;
						}
						if (num10 > 900f)
						{
							num10 = 900f + (num10 - 900f) * 0.25f;
						}
						if (distance < num10)
						{
							float num11 = (float)Main.rand.Next(90 + (int)damage / 3, 240 + (int)damage / 2);
							Main.npc[m].AddBuff(mod.BuffType("GlacialState"), (int)num11, false);
						}
					}
				}
			}
			if (aBrain && Main.myPlayer == player.whoAmI)
			{
				for (int m = 0; m < 200; m++)
				{
					if (Main.npc[m].active && !Main.npc[m].friendly)
					{
						float arg_67A_0 = (Main.npc[m].Center - player.Center).Length();
						float num10 = (float)Main.rand.Next(200 + (int)damage / 2, 301 + (int)damage * 2);
						if (num10 > 500f)
						{
							num10 = 500f + (num10 - 500f) * 0.75f;
						}
						if (num10 > 700f)
						{
							num10 = 700f + (num10 - 700f) * 0.5f;
						}
						if (num10 > 900f)
						{
							num10 = 900f + (num10 - 900f) * 0.25f;
						}
						if (arg_67A_0 < num10)
						{
							float num11 = (float)Main.rand.Next(90 + (int)damage / 3, 300 + (int)damage / 2);
							Main.npc[m].AddBuff(31, (int)num11, false);
						}
					}
				}
				Projectile.NewProjectile(player.Center.X + (float)Main.rand.Next(-40, 40), player.Center.Y - (float)Main.rand.Next(20, 60), player.velocity.X * 0.3f, player.velocity.Y * 0.3f, 565, 0, 0f, player.whoAmI, 0f, 0f);
			}
            if (player.ownedProjectileCounts[mod.ProjectileType("Drataliornus")] != 0)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (Main.projectile[i].active && Main.projectile[i].type == mod.ProjectileType("Drataliornus") && Main.projectile[i].owner == player.whoAmI)
                    {
                        Main.projectile[i].Kill();
                        break;
                    }
                }
                player.AddBuff(mod.BuffType("Backfire"), 360);
                if (player.wingTime > player.wingTimeMax / 2)
                    player.wingTime = player.wingTimeMax / 2;
            }
        }
		
		public override void PostHurt(bool pvp, bool quiet, double damage, int hitDirection, bool crit)
		{
            if (pArtifact)
            {
                player.AddBuff(mod.BuffType("BurntOut"), 300, true);
            }
            if (CalamityGlobalNPC.SCal > -1 && CalamityWorld.revenge)
            {
                if (damage < 100)
                {
                    KillPlayer();
                    string key = "Mods.CalamityMod.SupremeBossText28";
                    Color messageColor = Color.Orange;
                    if (Main.netMode == 0)
                    {
                        Main.NewText(Language.GetTextValue(key), messageColor);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                    }
                }
            }
            bool hardMode = Main.hardMode;
            if (player.whoAmI == Main.myPlayer)
            {
                if (cTracers && damage > 200)
                {
                    player.immuneTime += 60;
                }
                if (godSlayerThrowing && damage > 80)
                {
                    player.immuneTime += 30;
                }
                if (statigelSet && damage > 100)
                {
                    player.immuneTime += 30;
                }
                if (dAmulet)
                {
                    if (damage == 1.0)
                    {
                        player.immuneTime += 10;
                    }
                    else
                    {
                        player.immuneTime += 20;
                    }
                }
                if (fabsolVodka)
                {
                    if (damage == 1.0)
                    {
                        player.immuneTime += 5;
                    }
                    else
                    {
                        player.immuneTime += 10;
                    }
                }
                if (damage > 25)
                {
                    if (aeroSet)
                    {
                        for (int n = 0; n < 4; n++)
                        {
                            float x = player.position.X + (float)Main.rand.Next(-400, 400);
                            float y = player.position.Y - (float)Main.rand.Next(500, 800);
                            Vector2 vector = new Vector2(x, y);
                            float num13 = player.position.X + (float)(player.width / 2) - vector.X;
                            float num14 = player.position.Y + (float)(player.height / 2) - vector.Y;
                            num13 += (float)Main.rand.Next(-100, 101);
                            int num15 = 20;
                            float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                            num16 = (float)num15 / num16;
                            num13 *= num16;
                            num14 *= num16;
                            int num17 = Projectile.NewProjectile(x, y, num13, num14, mod.ProjectileType("StickyFeatherAero"), 20, 1f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
                if (aBulwark)
                {
                    for (int n = 0; n < 4; n++)
                    {
                        float x = player.position.X + (float)Main.rand.Next(-400, 400);
                        float y = player.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num13 = player.position.X + (float)(player.width / 2) - vector.X;
                        float num14 = player.position.Y + (float)(player.height / 2) - vector.Y;
                        num13 += (float)Main.rand.Next(-100, 101);
                        int num15 = 29;
                        float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                        num16 = (float)num15 / num16;
                        num13 *= num16;
                        num14 *= num16;
                        int num17 = Projectile.NewProjectile(x, y, num13, num14, mod.ProjectileType("AstralStar"), 320, 5f, player.whoAmI, 0f, 0f);
                    }
                }
                if (dAmulet)
                {
                    for (int n = 0; n < 3; n++)
                    {
                        float x = player.position.X + (float)Main.rand.Next(-400, 400);
                        float y = player.position.Y - (float)Main.rand.Next(500, 800);
                        Vector2 vector = new Vector2(x, y);
                        float num13 = player.position.X + (float)(player.width / 2) - vector.X;
                        float num14 = player.position.Y + (float)(player.height / 2) - vector.Y;
                        num13 += (float)Main.rand.Next(-100, 101);
                        int num15 = 29;
                        float num16 = (float)Math.Sqrt((double)(num13 * num13 + num14 * num14));
                        num16 = (float)num15 / num16;
                        num13 *= num16;
                        num14 *= num16;
                        int num17 = Projectile.NewProjectile(x, y, num13, num14, 92, 130, 4f, player.whoAmI, 0f, 0f);
                        Main.projectile[num17].usesLocalNPCImmunity = true;
                        Main.projectile[num17].localNPCHitCooldown = 5;
                        Main.projectile[num17].ranged = false;
                    }
                }
            }
			if (fCarapace)
			{
				if (damage > 0)
				{
					Main.PlaySound(3, (int)player.position.X, (int)player.position.Y, 45);
					float spread = 45f * 0.0174f;
					double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
					double deltaAngle = spread / 8f;
					double offsetAngle;
					int i;
					int fDamage = 56;
					if (player.whoAmI == Main.myPlayer)
					{
						for (i = 0; i < 4; i++) 
						{
							float xPos = Main.rand.Next(2) == 0 ? player.Center.X + 100 : player.Center.X - 100;
    						Vector2 vector2 = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
							int spore1 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), 590, fDamage, 1.25f, player.whoAmI, 0f, 0f);
							int spore2 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), 590, fDamage, 1.25f, player.whoAmI, 0f, 0f);
							Main.projectile[spore1].timeLeft = 120;
							Main.projectile[spore2].timeLeft = 120;
						}
					}
				}
			}
			if (aSpark)
			{
				if (damage > 0)
				{
					Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 93);
					float spread = 45f * 0.0174f;
					double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
					double deltaAngle = spread / 8f;
					double offsetAngle;
					int i;
					int sDamage = hardMode ? 36 : 6;
					if (player.whoAmI == Main.myPlayer)
					{
						for (i = 0; i < 4; i++) 
						{
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
							int spark1 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("Spark"), sDamage, 1.25f, player.whoAmI, 0f, 0f);
							int spark2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("Spark"), sDamage, 1.25f, player.whoAmI, 0f, 0f);
							Main.projectile[spark1].timeLeft = 120;
							Main.projectile[spark2].timeLeft = 120;
						}
					}
				}
			}
            if (ataxiaBlaze && Main.rand.Next(5) == 0)
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 74);
                    int eDamage = 100;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("ChaosBlaze"), eDamage, 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            else if (daedalusShard)
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 27);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int sDamage = 27;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 8; i++)
                        {
                            float randomSpeed = (float)Main.rand.Next(1, 7);
                            float randomSpeed2 = (float)Main.rand.Next(1, 7);
                            offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                            int shard = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f) + randomSpeed, 90, sDamage, 1f, player.whoAmI, 0f, 0f);
                            int shard2 = Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f) + randomSpeed2, 90, sDamage, 1f, player.whoAmI, 0f, 0f);
                            Main.projectile[shard].ranged = false;
                            Main.projectile[shard2].ranged = false;
                        }
                    }
                }
            }
            else if (reaverSpore)
            {
                if (damage > 0)
                {
                    Main.PlaySound(3, (int)player.position.X, (int)player.position.Y, 1);
                    float spread = 45f * 0.0174f;
                    double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
                    double deltaAngle = spread / 8f;
                    double offsetAngle;
                    int i;
                    int rDamage = 58;
                    if (player.whoAmI == Main.myPlayer)
                    {
                        for (i = 0; i < 4; i++)
                        {
                            float xPos = Main.rand.Next(2) == 0 ? player.Center.X + 100 : player.Center.X - 100;
                            Vector2 vector2 = new Vector2(xPos, player.Center.Y + Main.rand.Next(-100, 101));
                            offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), 567, rDamage, 2f, player.whoAmI, 0f, 0f);
                            Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), 568, rDamage, 2f, player.whoAmI, 0f, 0f);
                        }
                    }
                }
            }
            else if (godSlayerDamage)
			{
				if (damage > 80)
				{
					Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 73);
					float spread = 45f * 0.0174f;
					double startAngle = Math.Atan2(player.velocity.X, player.velocity.Y) - spread / 2;
					double deltaAngle = spread / 8f;
					double offsetAngle;
					int i;
					if (player.whoAmI == Main.myPlayer)
					{
						for (i = 0; i < 4; i++) 
						{
							offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i;
							Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("GodKiller"), 900, 5f, player.whoAmI, 0f, 0f);
							Projectile.NewProjectile(player.Center.X, player.Center.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("GodKiller"), 900, 5f, player.whoAmI, 0f, 0f);
						}
					}
				}
			}
            else if (godSlayerMage)
            {
                if (damage > 0)
                {
                    Main.PlaySound(2, (int)player.position.X, (int)player.position.Y, 74);
                    if (player.whoAmI == Main.myPlayer)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("GodSlayerBlaze"), (auricSet ? 2400 : 1200), 1f, player.whoAmI, 0f, 0f);
                    }
                }
            }
            else if (dsSetBonus)
			{
				if (player.whoAmI == Main.myPlayer)
        		{
					for (int l = 0; l < 2; l++)
					{
						float x = player.position.X + (float)Main.rand.Next(-400, 400);
						float y = player.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 vector = new Vector2(x, y);
						float num15 = player.position.X + (float)(player.width / 2) - vector.X;
						float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
						num15 += (float)Main.rand.Next(-100, 101);
						int num17 = 22;
						float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
						num18 = (float)num17 / num18;
						num15 *= num18;
						num16 *= num18;
						int num19 = Projectile.NewProjectile(x, y, num15, num16, 294, 3000, 7f, player.whoAmI, 0f, 0f);
						Main.projectile[num19].ai[1] = player.position.Y;
					}
			 	    for (int l = 0; l < 5; l++)
					{
						float x = player.position.X + (float)Main.rand.Next(-400, 400);
						float y = player.position.Y - (float)Main.rand.Next(500, 800);
						Vector2 vector = new Vector2(x, y);
						float num15 = player.position.X + (float)(player.width / 2) - vector.X;
						float num16 = player.position.Y + (float)(player.height / 2) - vector.Y;
						num15 += (float)Main.rand.Next(-100, 101);
						int num17 = 22;
						float num18 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
						num18 = (float)num17 / num18;
						num15 *= num18;
						num16 *= num18;
						int num19 = Projectile.NewProjectile(x, y, num15, num16, 45, 5000, 7f, player.whoAmI, 0f, 0f);
						Main.projectile[num19].ai[1] = player.position.Y;
					}
				}
			}
		}
        #endregion

        #region KillPlayer
        public void KillPlayer()
        {
            player.lastDeathPostion = player.Center;
            player.lastDeathTime = DateTime.Now;
            player.showLastDeath = true;
            bool specialDeath = CalamityWorld.ironHeart && areThereAnyDamnBosses;
            bool flag;
            int coinsOwned = (int)Utils.CoinsCount(out flag, player.inventory, new int[0]);
            if (Main.myPlayer == player.whoAmI)
            {
                player.lostCoins = coinsOwned;
                player.lostCoinString = Main.ValueToCoins(player.lostCoins);
            }
            if (Main.myPlayer == player.whoAmI)
            {
                Main.mapFullscreen = false;
            }
            if (Main.myPlayer == player.whoAmI)
            {
                player.trashItem.SetDefaults(0, false);
                if (specialDeath)
                {
                    player.difficulty = 2;
                    player.DropItems();
                    player.KillMeForGood();
                }
                else if (player.difficulty == 0)
                {
                    for (int i = 0; i < 59; i++)
                    {
                        if (player.inventory[i].stack > 0 && ((player.inventory[i].type >= 1522 && player.inventory[i].type <= 1527) || player.inventory[i].type == 3643))
                        {
                            int num = Item.NewItem((int)player.position.X, (int)player.position.Y, player.width, player.height, player.inventory[i].type, 1, false, 0, false, false);
                            Main.item[num].netDefaults(player.inventory[i].netID);
                            Main.item[num].Prefix((int)player.inventory[i].prefix);
                            Main.item[num].stack = player.inventory[i].stack;
                            Main.item[num].velocity.Y = (float)Main.rand.Next(-20, 1) * 0.2f;
                            Main.item[num].velocity.X = (float)Main.rand.Next(-20, 21) * 0.2f;
                            Main.item[num].noGrabDelay = 100;
                            Main.item[num].favorited = false;
                            Main.item[num].newAndShiny = false;
                            if (Main.netMode == 1)
                            {
                                NetMessage.SendData(21, -1, -1, null, num, 0f, 0f, 0f, 0, 0, 0);
                            }
                            player.inventory[i].SetDefaults(0, false);
                        }
                    }
                }
                else if (player.difficulty == 1)
                {
                    player.DropItems();
                }
                else if (player.difficulty == 2)
                {
                    player.DropItems();
                    player.KillMeForGood();
                }
            }
            if (specialDeath)
            {
                Main.PlaySound(mod.GetLegacySoundSlot(SoundType.Item, "Sounds/Item/IronHeartDeath"), (int)player.position.X, (int)player.position.Y);
            }
            else
            {
                Main.PlaySound(5, (int)player.position.X, (int)player.position.Y, 1, 1f, 0f);
            }
            player.headVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.bodyVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.legVelocity.Y = (float)Main.rand.Next(-40, -10) * 0.1f;
            player.headVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            player.bodyVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            player.legVelocity.X = (float)Main.rand.Next(-20, 21) * 0.1f + (float)(2 * 0);
            if (player.stoned)
            {
                player.headPosition = Vector2.Zero;
                player.bodyPosition = Vector2.Zero;
                player.legPosition = Vector2.Zero;
            }
            for (int j = 0; j < 100; j++)
            {
                Dust.NewDust(player.position, player.width, player.height, (specialDeath ? 91 : 235), (float)(2 * 0), -2f, 0, default(Color), 1f);
            }
            player.mount.Dismount(player);
            player.dead = true;
            player.respawnTimer = 600;
            if (Main.expertMode)
            {
                player.respawnTimer = (int)((double)player.respawnTimer * 1.5);
            }
            player.immuneAlpha = 0;
            player.palladiumRegen = false;
            player.iceBarrier = false;
            player.crystalLeaf = false;
            PlayerDeathReason damageSource = PlayerDeathReason.ByOther(player.Male ? 14 : 15);
            NetworkText deathText = damageSource.GetDeathText(player.name);
            if (Main.netMode == 2)
            {
                NetMessage.BroadcastChatMessage(deathText, new Color(225, 25, 25), -1);
            }
            else if (Main.netMode == 0)
            {
                Main.NewText(deathText.ToString(), 225, 25, 25, false);
            }
            if (Main.netMode == 1 && player.whoAmI == Main.myPlayer)
            {
                NetMessage.SendPlayerDeath(player.whoAmI, damageSource, (int)1000.0, 0, false, -1, -1);
            }
            if (player.whoAmI == Main.myPlayer && player.difficulty == 0)
            {
                player.DropCoins();
            }
            player.DropTombstone(coinsOwned, deathText, 0);
            if (player.whoAmI == Main.myPlayer)
            {
                try
                {
                    WorldGen.saveToonWhilePlaying();
                }
                catch
                {
                }
            }
        }
        #endregion

        #region DashStuff
        public void ModDashMovement()
		{
			if (dashMod == 4 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
			{
				Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
					{
						NPC nPC = Main.npc[i];
						Rectangle rect = nPC.getRect();
						if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
						{
							float num = 1500f * player.meleeDamage;
							float num2 = 15f;
							bool crit = false;
							if (player.kbGlove)
							{
								num2 *= 2f;
							}
							if (player.kbBuff)
							{
								num2 *= 1.5f;
							}
							if (Main.rand.Next(100) < player.meleeCrit)
							{
								crit = true;
							}
							int direction = player.direction;
							if (player.velocity.X < 0f)
							{
								direction = -1;
							}
							if (player.velocity.X > 0f)
							{
								direction = 1;
							}
							if (player.whoAmI == Main.myPlayer)
							{
								player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
								int num6 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("HolyExplosionSupreme"), 1000, 20f, Main.myPlayer, 0f, 0f);
								Main.projectile[num6].Kill();
								Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("HolyEruption"), 780, 5f, Main.myPlayer, 0f, 0f);
							}
							nPC.immune[player.whoAmI] = 6;
                            nPC.AddBuff(mod.BuffType("GodSlayerInferno"), 300);
                            player.immune = true;
							player.immuneNoBlink = true;
							player.immuneTime = 4;
						}
					}
				}
			}
			if (dashMod == 3 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
			{
				Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
					{
						NPC nPC = Main.npc[i];
						Rectangle rect = nPC.getRect();
						if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
						{
							float num = 500f * player.meleeDamage;
							float num2 = 12f;
							bool crit = false;
							if (player.kbGlove)
							{
								num2 *= 2f;
							}
							if (player.kbBuff)
							{
								num2 *= 1.5f;
							}
							if (Main.rand.Next(100) < player.meleeCrit)
							{
								crit = true;
							}
							int direction = player.direction;
							if (player.velocity.X < 0f)
							{
								direction = -1;
							}
							if (player.velocity.X > 0f)
							{
								direction = 1;
							}
							if (player.whoAmI == Main.myPlayer)
							{
								player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
								int num6 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("HolyExplosionSupreme"), 500, 15f, Main.myPlayer, 0f, 0f);
								Main.projectile[num6].Kill();
								Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("HolyEruption"), 380, 5f, Main.myPlayer, 0f, 0f);
							}
							nPC.immune[player.whoAmI] = 6;
							player.immune = true;
							player.immuneNoBlink = true;
							player.immuneTime = 4;
						}
					}
				}
			}
			if (dashMod == 2 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
			{
				Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && Main.npc[i].immune[player.whoAmI] <= 0)
					{
						NPC nPC = Main.npc[i];
						Rectangle rect = nPC.getRect();
						if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
						{
							float num = 100f * player.meleeDamage;
							float num2 = 9f;
							bool crit = false;
							if (player.kbGlove)
							{
								num2 *= 2f;
							}
							if (player.kbBuff)
							{
								num2 *= 1.5f;
							}
							if (Main.rand.Next(100) < player.meleeCrit)
							{
								crit = true;
							}
							int direction = player.direction;
							if (player.velocity.X < 0f)
							{
								direction = -1;
							}
							if (player.velocity.X > 0f)
							{
								direction = 1;
							}
							if (player.whoAmI == Main.myPlayer)
							{
								player.ApplyDamageToNPC(nPC, (int)num, num2, direction, crit);
								int num6 = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("HolyExplosion"), 100, 15f, Main.myPlayer, 0f, 0f);
								Main.projectile[num6].Kill();
							}
							nPC.immune[player.whoAmI] = 6;
							player.immune = true;
							player.immuneNoBlink = true;
							player.immuneTime = 4;
						}
					}
				}
			}
			if (dashMod == 1 && player.dashDelay < 0 && player.whoAmI == Main.myPlayer)
			{
				Rectangle rectangle = new Rectangle((int)((double)player.position.X + (double)player.velocity.X * 0.5 - 4.0), (int)((double)player.position.Y + (double)player.velocity.Y * 0.5 - 4.0), player.width + 8, player.height + 8);
                for (int i = 0; i < 200; i++)
                {
                    if ((Main.npc[i].active && !Main.npc[i].dontTakeDamage && !Main.npc[i].friendly && !Main.npc[i].townNPC && Main.npc[i].immune[player.whoAmI] <= 0 && Main.npc[i].damage > 0))
                    {
                        NPC nPC = Main.npc[i];
                        Rectangle rect = nPC.getRect();
                        if (rectangle.Intersects(rect) && (nPC.noTileCollide || player.CanHit(nPC)))
                        {
                            OnDodge();
                            break;
                        }
                    }
                }
                for (int i = 0; i < 1000; i++)
                {
                    if ((Main.projectile[i].active && !Main.projectile[i].friendly && Main.projectile[i].hostile && Main.projectile[i].damage > 0))
                    {
                        Projectile proj = Main.projectile[i];
                        Rectangle rect = proj.getRect();
                        if (rectangle.Intersects(rect))
                        {
                            OnDodge();
                            break;
                        }
                    }
                }
            }
			if (player.dashDelay > 0)
			{
				return;
			}
			if (player.dashDelay < 0)
			{
				float num7 = 12f;
				float num8 = 0.985f;
				float num9 = Math.Max(player.accRunSpeed, player.maxRunSpeed);
				float num10 = 0.94f;
				int num11 = 20;
				if (dashMod == 1)
				{
					for (int k = 0; k < 2; k++)
					{
						int num12;
						if (player.velocity.Y == 0f)
						{
							num12 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)player.height - 4f), player.width, 8, 235, 0f, 0f, 100, default(Color), 1.4f);
						}
						else
						{
							num12 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + (float)(player.height / 2) - 8f), player.width, 16, 235, 0f, 0f, 100, default(Color), 1.4f);
						}
						Main.dust[num12].velocity *= 0.1f;
						Main.dust[num12].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
				}
				else if (dashMod == 2)
				{
					for (int m = 0; m < 4; m++)
					{
						int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 246, 0f, 0f, 100, default(Color), 2.75f);
						Main.dust[num14].velocity *= 0.1f;
						Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
						Main.dust[num14].noGravity = true;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num14].fadeIn = 0.5f;
						}
					}
				}
				else if (dashMod == 3)
				{
					for (int m = 0; m < 12; m++)
					{
						int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 244, 0f, 0f, 100, default(Color), 2.75f);
						Main.dust[num14].velocity *= 0.1f;
						Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
						Main.dust[num14].noGravity = true;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num14].fadeIn = 0.5f;
						}
					}
                    num7 = 14f; //14
                }
				else if (dashMod == 4)
				{
					for (int m = 0; m < 24; m++)
					{
						int num14 = Dust.NewDust(new Vector2(player.position.X, player.position.Y + 4f), player.width, player.height - 8, 244, 0f, 0f, 100, default(Color), 2.75f);
						Main.dust[num14].velocity *= 0.1f;
						Main.dust[num14].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
						Main.dust[num14].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
						Main.dust[num14].noGravity = true;
						if (Main.rand.Next(2) == 0)
						{
							Main.dust[num14].fadeIn = 0.5f;
						}
					}
                    num7 = 16f; //14
                }
				if (dashMod > 0)
				{
					player.vortexStealthActive = false;
					if (player.velocity.X > num7 || player.velocity.X < -num7)
					{
						player.velocity.X = player.velocity.X * num8;
						return;
					}
					if (player.velocity.X > num9 || player.velocity.X < -num9)
					{
						player.velocity.X = player.velocity.X * num10;
						return;
					}
					player.dashDelay = num11;
					if (player.velocity.X < 0f)
					{
						player.velocity.X = -num9;
						return;
					}
					if (player.velocity.X > 0f)
					{
						player.velocity.X = num9;
						return;
					}
				}
			}
			else if (dashMod > 0 && !player.mount.Active)
			{
				if (dashMod == 1)
				{
					int num16 = 0;
					bool flag = false;
					if (dashTimeMod > 0)
					{
						dashTimeMod--;
					}
					if (dashTimeMod < 0)
					{
						dashTimeMod++;
					}
					if (player.controlRight && player.releaseRight)
					{
						if (dashTimeMod > 0)
						{
							num16 = 1;
							flag = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = 15;
						}
					}
					else if (player.controlLeft && player.releaseLeft)
					{
						if (dashTimeMod < 0)
						{
							num16 = -1;
							flag = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = -15;
						}
					}
					if (flag)
					{
						player.velocity.X = 14.5f * (float)num16; //eoc dash amount
						Point point = (player.Center + new Vector2((float)(num16 * player.width / 2 + 2), player.gravDir * (float)(-(float)player.height) / 2f + player.gravDir * 2f)).ToTileCoordinates();
						Point point2 = (player.Center + new Vector2((float)(num16 * player.width / 2 + 2), 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point.X, point.Y) || WorldGen.SolidOrSlopedTile(point2.X, point2.Y))
						{
							player.velocity.X = player.velocity.X / 2f;
						}
						player.dashDelay = -1;
						for (int num17 = 0; num17 < 20; num17++)
						{
							int num18 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 235, 0f, 0f, 100, default(Color), 2f);
							Dust expr_CDB_cp_0 = Main.dust[num18];
							expr_CDB_cp_0.position.X = expr_CDB_cp_0.position.X + (float)Main.rand.Next(-5, 6);
							Dust expr_D02_cp_0 = Main.dust[num18];
							expr_D02_cp_0.position.Y = expr_D02_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
							Main.dust[num18].velocity *= 0.2f;
							Main.dust[num18].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num18].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
						}
						return;
					}
				}
				else if (dashMod == 2)
				{
					int num23 = 0;
					bool flag3 = false;
					if (dashTimeMod > 0)
					{
						dashTimeMod--;
					}
					if (dashTimeMod < 0)
					{
						dashTimeMod++;
					}
					if (player.controlRight && player.releaseRight)
					{
						if (dashTimeMod > 0)
						{
							num23 = 1;
							flag3 = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = 15;
						}
					}
					else if (player.controlLeft && player.releaseLeft)
					{
						if (dashTimeMod < 0)
						{
							num23 = -1;
							flag3 = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = -15;
						}
					}
					if (flag3)
					{
						player.velocity.X = 16.9f * (float)num23; //tabi dash amount
						Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)(-(float)player.height) / 2f + player.gravDir * 2f)).ToTileCoordinates();
						Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
						{
							player.velocity.X = player.velocity.X / 2f;
						}
						player.dashDelay = -1;
						for (int num24 = 0; num24 < 20; num24++)
						{
							int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 246, 0f, 0f, 100, default(Color), 3f);
							Dust expr_13AF_cp_0 = Main.dust[num25];
							expr_13AF_cp_0.position.X = expr_13AF_cp_0.position.X + (float)Main.rand.Next(-5, 6);
							Dust expr_13D6_cp_0 = Main.dust[num25];
							expr_13D6_cp_0.position.Y = expr_13D6_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
							Main.dust[num25].velocity *= 0.2f;
							Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
							Main.dust[num25].noGravity = true;
							Main.dust[num25].fadeIn = 0.5f;
						}
					}
				}
				else if (dashMod == 3)
				{
					int num23 = 0;
					bool flag3 = false;
					if (dashTimeMod > 0)
					{
						dashTimeMod--;
					}
					if (dashTimeMod < 0)
					{
						dashTimeMod++;
					}
					if (player.controlRight && player.releaseRight)
					{
						if (dashTimeMod > 0)
						{
							num23 = 1;
							flag3 = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = 15;
						}
					}
					else if (player.controlLeft && player.releaseLeft)
					{
						if (dashTimeMod < 0)
						{
							num23 = -1;
							flag3 = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = -15;
						}
					}
					if (flag3)
					{
						player.velocity.X = 21.9f * (float)num23; //solar dash amount
						Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)(-(float)player.height) / 2f + player.gravDir * 2f)).ToTileCoordinates();
						Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
						{
							player.velocity.X = player.velocity.X / 2f;
						}
						player.dashDelay = -1;
						for (int num24 = 0; num24 < 40; num24++)
						{
							int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default(Color), 3f);
							Dust expr_13AF_cp_0 = Main.dust[num25];
							expr_13AF_cp_0.position.X = expr_13AF_cp_0.position.X + (float)Main.rand.Next(-5, 6);
							Dust expr_13D6_cp_0 = Main.dust[num25];
							expr_13D6_cp_0.position.Y = expr_13D6_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
							Main.dust[num25].velocity *= 0.2f;
							Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
							Main.dust[num25].noGravity = true;
							Main.dust[num25].fadeIn = 0.5f;
						}
					}
				}
				else if (dashMod == 4)
				{
					int num23 = 0;
					bool flag3 = false;
					if (dashTimeMod > 0)
					{
						dashTimeMod--;
					}
					if (dashTimeMod < 0)
					{
						dashTimeMod++;
					}
					if (player.controlRight && player.releaseRight)
					{
						if (dashTimeMod > 0)
						{
							num23 = 1;
							flag3 = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = 15;
						}
					}
					else if (player.controlLeft && player.releaseLeft)
					{
						if (dashTimeMod < 0)
						{
							num23 = -1;
							flag3 = true;
							dashTimeMod = 0;
						}
						else
						{
							dashTimeMod = -15;
						}
					}
					if (flag3)
					{
						player.velocity.X = 23.9f * (float)num23; //slighty more powerful solar dash
						Point point5 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), player.gravDir * (float)(-(float)player.height) / 2f + player.gravDir * 2f)).ToTileCoordinates();
						Point point6 = (player.Center + new Vector2((float)(num23 * player.width / 2 + 2), 0f)).ToTileCoordinates();
						if (WorldGen.SolidOrSlopedTile(point5.X, point5.Y) || WorldGen.SolidOrSlopedTile(point6.X, point6.Y))
						{
							player.velocity.X = player.velocity.X / 2f;
						}
						player.dashDelay = -1;
						for (int num24 = 0; num24 < 60; num24++)
						{
							int num25 = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 244, 0f, 0f, 100, default(Color), 3f);
							Dust expr_13AF_cp_0 = Main.dust[num25];
							expr_13AF_cp_0.position.X = expr_13AF_cp_0.position.X + (float)Main.rand.Next(-5, 6);
							Dust expr_13D6_cp_0 = Main.dust[num25];
							expr_13D6_cp_0.position.Y = expr_13D6_cp_0.position.Y + (float)Main.rand.Next(-5, 6);
							Main.dust[num25].velocity *= 0.2f;
							Main.dust[num25].scale *= 1f + (float)Main.rand.Next(20) * 0.01f;
							Main.dust[num25].shader = GameShaders.Armor.GetSecondaryShader(player.ArmorSetDye(), player);
							Main.dust[num25].noGravity = true;
							Main.dust[num25].fadeIn = 0.5f;
						}
					}
				}
			}
		}

        private void OnDodge()
        {
            if (player.whoAmI == Main.myPlayer && dodgeScarf && !scarfCooldown)
            {
                player.AddBuff(mod.BuffType("ScarfMeleeBoost"), Main.rand.Next(480, 561));
                player.AddBuff(mod.BuffType("ScarfCooldown"), (player.chaosState ? 1800 : 900));
                player.immune = true;
                player.immuneTime = 60;
                if (player.longInvince)
                {
                    player.immuneTime += 40;
                }
                for (int k = 0; k < player.hurtCooldowns.Length; k++)
                {
                    player.hurtCooldowns[k] = player.immuneTime;
                }
                for (int j = 0; j < 100; j++)
                {
                    int num = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, 235, 0f, 0f, 100, default(Color), 2f);
                    Dust expr_A4_cp_0 = Main.dust[num];
                    expr_A4_cp_0.position.X = expr_A4_cp_0.position.X + (float)Main.rand.Next(-20, 21);
                    Dust expr_CB_cp_0 = Main.dust[num];
                    expr_CB_cp_0.position.Y = expr_CB_cp_0.position.Y + (float)Main.rand.Next(-20, 21);
                    Main.dust[num].velocity *= 0.4f;
                    Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                    Main.dust[num].shader = GameShaders.Armor.GetSecondaryShader(player.cWaist, player);
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num].scale *= 1f + (float)Main.rand.Next(40) * 0.01f;
                        Main.dust[num].noGravity = true;
                    }
                }
                if (player.whoAmI == Main.myPlayer)
                {
                    NetMessage.SendData(62, -1, -1, null, player.whoAmI, 1f, 0f, 0f, 0, 0, 0);
                }
            }
        }

        public void ModHorizontalMovement()
		{
			float num = (player.accRunSpeed + player.maxRunSpeed) / 2f;
			if (player.controlLeft && player.velocity.X > -player.accRunSpeed && player.dashDelay >= 0)
			{
                if (player.velocity.X < -num && player.velocity.Y == 0f && !player.mount.Active)
				{
					int num3 = 0;
					if (player.gravDir == -1f)
					{
						num3 -= player.height;
					}
					if (dashMod == 1)
					{
						int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 235, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
						Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
						Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
					else if (dashMod == 2)
					{
						int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 246, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 2.5f);
						Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
						Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
						Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
					else if (dashMod == 3)
					{
						int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 3f);
						Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
						Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
						Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
					else if (dashMod == 4)
					{
						int num7 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num3), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 3f);
						Main.dust[num7].velocity.X = Main.dust[num7].velocity.X * 0.2f;
						Main.dust[num7].velocity.Y = Main.dust[num7].velocity.Y * 0.2f;
						Main.dust[num7].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
				}
			}
			else if (player.controlRight && player.velocity.X < player.accRunSpeed && player.dashDelay >= 0)
			{
                if (player.velocity.X > num && player.velocity.Y == 0f && !player.mount.Active)
				{
					int num8 = 0;
					if (player.gravDir == -1f)
					{
						num8 -= player.height;
					}
					if (dashMod == 1)
					{
						int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 235, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 1.5f);
						Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
						Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
						Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
					else if (dashMod == 2)
					{
						int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 246, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 2.5f);
						Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
						Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
						Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
					else if (dashMod == 3)
					{
						int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 3f);
						Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
						Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
						Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
					else if (dashMod == 4)
					{
						int num12 = Dust.NewDust(new Vector2(player.position.X - 4f, player.position.Y + (float)player.height + (float)num8), player.width + 8, 4, 244, -player.velocity.X * 0.5f, player.velocity.Y * 0.5f, 50, default(Color), 3f);
						Main.dust[num12].velocity.X = Main.dust[num12].velocity.X * 0.2f;
						Main.dust[num12].velocity.Y = Main.dust[num12].velocity.Y * 0.2f;
						Main.dust[num12].shader = GameShaders.Armor.GetSecondaryShader(player.cShoe, player);
					}
				}
			}
		}
        #endregion

        #region Drawing
        public static readonly PlayerLayer MiscEffectsBack = new PlayerLayer("CalamityMod", "MiscEffectsBack", PlayerLayer.MiscEffectsBack, delegate (PlayerDrawInfo drawInfo)
        {
            if (drawInfo.shadow != 0f)
            {
                return;
            }
            Player drawPlayer = drawInfo.drawPlayer;
            Mod mod = ModLoader.GetMod("CalamityMod");
            CalamityPlayer modPlayer = drawPlayer.GetModPlayer<CalamityPlayer>(mod);
            if (modPlayer.sirenIce)
            {
                Texture2D texture = mod.GetTexture("ExtraTextures/IceShield");
                int drawX = (int)(drawInfo.position.X + drawPlayer.width / 2f - Main.screenPosition.X);
                int drawY = (int)(drawInfo.position.Y + drawPlayer.height / 2f - Main.screenPosition.Y); //4
                DrawData data = new DrawData(texture, new Vector2(drawX, drawY), null, Color.Cyan, 0f, new Vector2(texture.Width / 2f, texture.Height / 2f), 1f, SpriteEffects.None, 0);
                Main.playerDrawData.Add(data);
            }
        });

        public override void ModifyDrawLayers(List<PlayerLayer> list)
        {
            MiscEffectsBack.visible = true;
            list.Insert(0, MiscEffectsBack);
            if (fab) { AddPlayerLayer(list, clAfterAll, list[list.Count - 1], false); }
        }

        public PlayerLayer clAfterAll = new PlayerLayer("Calamity", "clAfterAll", PlayerLayer.MiscEffectsFront, delegate (PlayerDrawInfo edi)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            Player drawPlayer = edi.drawPlayer;
            if (drawPlayer.mount != null && drawPlayer.GetModPlayer<CalamityPlayer>(mod).fab) { drawPlayer.mount.Draw(Main.playerDrawData, 3, drawPlayer, edi.position, edi.mountColor, edi.spriteEffects, edi.shadow); }
        });

        public static void AddPlayerLayer(List<PlayerLayer> list, PlayerLayer layer, PlayerLayer parent, bool first)
        {
            int insertAt = -1;
            for (int m = 0; m < list.Count; m++)
            {
                PlayerLayer dl = list[m];
                if (dl.Name.Equals(parent.Name)) { insertAt = m; break; }
            }
            if (insertAt == -1) list.Add(layer); else list.Insert(first ? insertAt : insertAt + 1, layer);
        }

        public override void DrawEffects(PlayerDrawInfo drawInfo, ref float r, ref float g, ref float b, ref float a, ref bool fullBright)
		{
            if (CalamityWorld.ironHeart && !Main.gameMenu)
            {
                Texture2D ironHeart = mod.GetTexture("ExtraTextures/IronHeart");
                Main.heartTexture = Main.heart2Texture = ironHeart;
            }
            else
            {
                Texture2D heart3 = mod.GetTexture("ExtraTextures/Heart3");
                Texture2D heart4 = mod.GetTexture("ExtraTextures/Heart4");
                Texture2D heart5 = mod.GetTexture("ExtraTextures/Heart5");
                Texture2D heart6 = mod.GetTexture("ExtraTextures/Heart6");
                Texture2D heartOriginal = mod.GetTexture("ExtraTextures/HeartOriginal");
                Texture2D heartOriginal2 = mod.GetTexture("ExtraTextures/HeartOriginal2");

                int totalFruit =
                    (mFruit ? 1 : 0) +
                    (bOrange ? 1 : 0) +
                    (eBerry ? 1 : 0) +
                    (dFruit ? 1 : 0);
                switch (totalFruit)
                {
                    default:
                        Main.heart2Texture = heartOriginal; Main.heartTexture = heartOriginal2;
                        break;
                    case 4:
                        Main.heart2Texture = heart6; Main.heartTexture = heartOriginal2;
                        break;
                    case 3:
                        Main.heart2Texture = heart5; Main.heartTexture = heartOriginal2;
                        break;
                    case 2:
                        Main.heart2Texture = heart4; Main.heartTexture = heartOriginal2;
                        break;
                    case 1:
                        Main.heart2Texture = heart3; Main.heartTexture = heartOriginal2;
                        break;
                }
            }
            if (revivifyTimer > 0)
            {
                if (Main.rand.Next(2) == 0 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 91, player.velocity.X * 0.2f, player.velocity.Y * 0.2f, 100, default(Color), 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
            }
            if (tRegen)
			{
                if (Main.rand.Next(10) == 0 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 107, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 1f);
                    Main.dust[dust].noGravity = true;
                    Main.dust[dust].velocity *= 0.75f;
                    Main.dust[dust].velocity.Y -= 0.35f;
                    Main.playerDrawDust.Add(dust);
                }
                r *= 0.025f;
				g *= 0.15f;
				b *= 0.035f;
				fullBright = true;
			}
			if (IBoots)
			{
				if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
				{	
					if (Main.rand.Next(2) == 0 && drawInfo.shadow == 0f)
					{
						int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 229, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 1f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.5f;
						Main.playerDrawDust.Add(dust);
					}
					r *= 0.05f;
					g *= 0.05f;
					b *= 0.05f;
					fullBright = true;
				}
			}
			if (elysianFire)
			{
				if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
				{	
					if (Main.rand.Next(2) == 0 && drawInfo.shadow == 0f)
					{
						int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 246, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 1f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.5f;
						Main.playerDrawDust.Add(dust);
					}
					r *= 0.75f;
					g *= 0.55f;
					b *= 0f;
					fullBright = true;
				}
			}
			if (dsSetBonus)
			{
				if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
				{	
					if (Main.rand.Next(2) == 0 && drawInfo.shadow == 0f)
					{
						int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 27, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.5f;
						Main.playerDrawDust.Add(dust);
					}
					r *= 0.15f;
					g *= 0.025f;
					b *= 0.1f;
					fullBright = true;
				}
			}
			if (auricSet)
			{
				if (((double)Math.Abs(player.velocity.X) > 0.05 || (double)Math.Abs(player.velocity.Y) > 0.05) && !player.mount.Active)
				{	
					if (drawInfo.shadow == 0f)
					{
						int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, Main.rand.Next(2) == 0 ? 57 : 244, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 1.5f);
						Main.dust[dust].noGravity = true;
						Main.dust[dust].velocity *= 0.5f;
						Main.playerDrawDust.Add(dust);
					}
					r *= 0.15f;
					g *= 0.025f;
					b *= 0.1f;
					fullBright = true;
				}
			}
			if (bFlames || aFlames || rageMode)
			{
				if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, mod.DustType("BrimstoneFlame"), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.playerDrawDust.Add(dust);
				}
				r *= 0.25f;
				g *= 0.01f;
				b *= 0.01f;
				fullBright = true;
			}
            if (adrenalineMode)
            {
                if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
                {
                    int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 206, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
                    Main.dust[dust].velocity *= 1.8f;
                    Main.dust[dust].velocity.Y -= 0.5f;
                    Main.playerDrawDust.Add(dust);
                }
                r *= 0.01f;
                g *= 0.15f;
                b *= 0.1f;
                fullBright = true;
            }
            if (gsInferno)
			{
				if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 173, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.playerDrawDust.Add(dust);
				}
				r *= 0.25f;
				g *= 0.01f;
				b *= 0.01f;
				fullBright = true;
			}
			if (hFlames || hInferno)
			{
				if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, mod.DustType("HolyFlame"), player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.playerDrawDust.Add(dust);
				}
				r *= 0.25f;
				g *= 0.25f;
				b *= 0.1f;
				fullBright = true;
			}
			if (pFlames)
			{
				if (Main.rand.Next(4) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 89, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.2f;
					Main.dust[dust].velocity.Y -= 0.15f;
					Main.playerDrawDust.Add(dust);
				}
				r *= 0.07f;
				g *= 0.15f;
				b *= 0.01f;
				fullBright = true;
			}
			if (gState || cDepth)
			{
				r *= 0f;
				g *= 0.05f;
				b *= 0.3f;
				fullBright = true;
			}
            if (draedonsHeart && !shadeRegen && !cFreeze && (double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
            {
                r *= 0f;
                g *= 0.5f;
                b *= 0f;
                fullBright = true;
            }
            if (bBlood)
			{
				if (Main.rand.Next(6) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 5, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 3f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 1.8f;
					Main.dust[dust].velocity.Y -= 0.5f;
					Main.playerDrawDust.Add(dust);
				}
				r *= 0.15f;
				g *= 0.01f;
				b *= 0.01f;
				fullBright = true;
			}
			if (mushy)
			{
				if (Main.rand.Next(6) == 0 && drawInfo.shadow == 0f)
				{
					int dust = Dust.NewDust(drawInfo.position - new Vector2(2f, 2f), player.width + 4, player.height + 4, 56, player.velocity.X * 0.4f, player.velocity.Y * 0.4f, 100, default(Color), 2f);
					Main.dust[dust].noGravity = true;
					Main.dust[dust].velocity *= 0.5f;
					Main.dust[dust].velocity.Y -= 0.1f;
					Main.playerDrawDust.Add(dust);
				}
				r *= 0.15f;
				g *= 0.01f;
				b *= 0.01f;
				fullBright = true;
			}
		}
        #endregion

        #region PacketStuff
        private void ExactLevelPacket(bool server, int levelType)
        {
            ModPacket packet = mod.GetPacket(256);
            switch (levelType)
            {
                case 0:
                    packet.Write((byte)CalamityModMessageType.ExactMeleeLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactMeleeLevel);
                    break;
                case 1:
                    packet.Write((byte)CalamityModMessageType.ExactRangedLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactRangedLevel);
                    break;
                case 2:
                    packet.Write((byte)CalamityModMessageType.ExactMagicLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactMagicLevel);
                    break;
                case 3:
                    packet.Write((byte)CalamityModMessageType.ExactSummonLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactSummonLevel);
                    break;
                case 4:
                    packet.Write((byte)CalamityModMessageType.ExactRogueLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(exactRogueLevel);
                    break;
            }
            if (!server)
            {
                packet.Send();
            }
            else
            {
                packet.Send(-1, player.whoAmI);
            }
        }

        private void LevelPacket(bool server, int levelType)
        {
            ModPacket packet = mod.GetPacket(256);
            switch (levelType)
            {
                case 0:
                    packet.Write((byte)CalamityModMessageType.MeleeLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(meleeLevel);
                    break;
                case 1:
                    packet.Write((byte)CalamityModMessageType.RangedLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(rangedLevel);
                    break;
                case 2:
                    packet.Write((byte)CalamityModMessageType.MagicLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(magicLevel);
                    break;
                case 3:
                    packet.Write((byte)CalamityModMessageType.SummonLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(summonLevel);
                    break;
                case 4:
                    packet.Write((byte)CalamityModMessageType.RogueLevelSync);
                    packet.Write(player.whoAmI);
                    packet.Write(rogueLevel);
                    break;
            }
            if (!server)
            {
                packet.Send();
            }
            else
            {
                packet.Send(-1, player.whoAmI);
            }
        }

        private void StressPacket(bool server)
		{
		    ModPacket packet = mod.GetPacket(256);
		    packet.Write((byte)CalamityModMessageType.StressSync);
		    packet.Write(player.whoAmI);
		    packet.Write(stress);
		    if (!server)
		    {
		        packet.Send();
		    }
		    else
		    {
		        packet.Send(-1, player.whoAmI);
		    }
		}

        private void AdrenalinePacket(bool server)
        {
            ModPacket packet = mod.GetPacket(256);
            packet.Write((byte)CalamityModMessageType.AdrenalineSync);
            packet.Write(player.whoAmI);
            packet.Write(adrenaline);
            if (!server)
            {
                packet.Send();
            }
            else
            {
                packet.Send(-1, player.whoAmI);
            }
        }

        internal void HandleExactLevels(BinaryReader reader, int levelType)
        {
            switch (levelType)
            {
                case 0:
                    exactMeleeLevel = reader.ReadInt32();
                    break;
                case 1:
                    exactRangedLevel = reader.ReadInt32();
                    break;
                case 2:
                    exactMagicLevel = reader.ReadInt32();
                    break;
                case 3:
                    exactSummonLevel = reader.ReadInt32();
                    break;
                case 4:
                    exactRogueLevel = reader.ReadInt32();
                    break;
            }
            if (Main.netMode == 2)
            {
                ExactLevelPacket(true, levelType);
            }
        }

        internal void HandleLevels(BinaryReader reader, int levelType)
        {
            switch (levelType)
            {
                case 0:
                    meleeLevel = reader.ReadInt32();
                    break;
                case 1:
                    rangedLevel = reader.ReadInt32();
                    break;
                case 2:
                    magicLevel = reader.ReadInt32();
                    break;
                case 3:
                    summonLevel = reader.ReadInt32();
                    break;
                case 4:
                    rogueLevel = reader.ReadInt32();
                    break;
            }
            if (Main.netMode == 2)
            {
                LevelPacket(true, levelType);
            }
        }

        internal void HandleStress(BinaryReader reader)
		{
		    stress = reader.ReadInt32();
		    if (Main.netMode == 2)
		    {
		        StressPacket(true);
		    }
		}

        internal void HandleAdrenaline(BinaryReader reader)
        {
            adrenaline = reader.ReadInt32();
            if (Main.netMode == 2)
            {
                AdrenalinePacket(true);
            }
        }

        public override void OnEnterWorld(Player player)
		{
		    if (Main.netMode == 1)
		    {
                ExactLevelPacket(false, 0);
                ExactLevelPacket(false, 1);
                ExactLevelPacket(false, 2);
                ExactLevelPacket(false, 3);
                ExactLevelPacket(false, 4);
                LevelPacket(false, 0);
                LevelPacket(false, 1);
                LevelPacket(false, 2);
                LevelPacket(false, 3);
                LevelPacket(false, 4);
                StressPacket(false);
                AdrenalinePacket(false);
		    }
		}
        #endregion
    }
}
