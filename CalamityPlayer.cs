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
using Terraria.ModLoader.IO;

namespace CalamityMod
{
	public class CalamityPlayer : ModPlayer
	{
        #region InstanceVars

        #region NormalVars
        public int soulEssence = 0;

        public static int bossRushStage = 0;

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

        public const int hitLimitNormal = 25;

        public const int hitLimitExpert = 20;

        public const int hitLimitRev = 15;

        public const int hitLimitDeath = 10;

        public const int hitLimitArmageddon = 1;

		public float modStealth = 1f;

        public float aquaticBoost = 1f;
		
		public float shieldInvinc = 5f;
		
		public int dashMod;
		
		public int dashTimeMod;

		public const float defEndurance = 0.33f;

        public const int defLifeRegen = 18;

        public const int defLifeRegenTime = 1800;

        public const float defDamage = 1.85f;

        public const int defCrit = 75;

        public bool fab = false;

        public bool drawBossHPBar = true;

        public bool shouldDrawSmallText = true;

        public bool deactivateStupidFuckingBullshit = false;
        #endregion

        #region PetStuff
        public bool leviPet = false;

        public bool sirenPet = false;

        public bool fox = false;

        public bool chibii = false;

        public bool brimling = false;
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

        public bool despawnProj = false;
        #endregion

        #region AdrenalineStuff
        public const int adrenalineMax = 10000;

        public int adrenalineMaxTimer = 300;

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
        public bool dodgeScarf = false;

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

        public int tarraRangedCrits = 0;

        public int tarraRangedCritTimer = 0;

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

        public int bloodflareMageCrits = 0;

        public int bloodflareMageCritTimer = 0;

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
        #endregion

        #region DebuffStuff
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
		
		public bool fungalClump = false;
		
		public bool redDevil = false;
		
		public bool valkyrie = false;
		
		public bool slimeGod = false;
		
		public bool urchin = false;
		
		public bool chaosSpirit = false;
		
		public bool reaverOrb = false;
		
		public bool daedalusCrystal = false;
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
        #endregion
        #endregion

        #region SavingAndLoading
        public override void Initialize()
		{
            deactivateStupidFuckingBullshit = false;
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
                    "bossRushStage", bossRushStage
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
            bossRushStage = tag.GetInt("bossRushStage");
        }

		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			stress = reader.ReadInt32();
            adrenaline = reader.ReadInt32();
            sCalDeathCount = reader.ReadInt32();
            sCalKillCount = reader.ReadInt32();
            bossRushStage = reader.ReadInt32();
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
            leviPet = false;
            sirenPet = false;
            fox = false;
            chibii = false;
            brimling = false;
            fab = false;
            sirenWaterBuff = false;
            abyssalDivingSuitPlates = false;
            abyssalDivingSuitCooldown = false;
            sirenIce = false;
            sirenIceCooldown = false;
            heartOfDarkness = false;
            draedonsHeart = false;
            draedonsStressGain = false;
			afflicted = false;
			affliction = false;
			afflictedBuff = false;
			dodgeScarf = false;
			elysianAegis = false;
			godSlayer = false;
			nCore = false;
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
			ataxiaBolt = false;
			ataxiaGeyser = false;
			shadeRegen = false;
			scarfCooldown = false;
			flamethrowerBoost = false;
			shadowSpeed = false;
			aSpark = false;
			aBulwark = false;
			dAmulet = false;
			fCarapace = false;
			gShell = false;
			absorber = false;
			aAmpoule = false;
			pAmulet = false;
			auricBoost = false;
			fBarrier = false;
			aBrain = false;
			frostFlare = false;
			beeResist = false;
			uberBees = false;
			projRef = false;
			nanotech = false;
			eQuiver = false;
			daedalusReflect = false;
			daedalusSplit = false;
			daedalusAbsorb = false;
			daedalusShard = false;
			reaverSpore = false;
			reaverDoubleTap = false;
			cryogenSoul = false;
			yInsignia = false;
			eGauntlet = false;
			eTalisman = false;
            statisBeltOfCurses = false;
			ataxiaFire = false;
			ataxiaVolley = false;
			ataxiaBlaze = false;
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
            ironBoots = false;
            depthCharm = false;
            anechoicPlating = false;
            jellyfishNecklace = false;
            abyssalAmulet = false;
            lumenousAmulet = false;
            reaperToothNecklace = false;
            aquaticEmblem = false;
            darkSunRing = false;
            calamityRing = false;
            eArtifact = false;
            dArtifact = false;
            gArtifact = false;
			reaverBlast = false;
			reaverBurst = false;
			astralStarRain = false;
			ataxiaMage = false;
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
			dsSetBonus = false;
            NOU = false;
            weakPetrification = false;
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
			rRage = false;
			xRage = false;
			xWrath = false;
			graxDefense = false;
			eGravity = false;
			sMeleeBoost = false;
			tFury = false;
			vHex = false;
			eGrav = false;
			warped = false;
			cadence = false;
			omniscience = false;
			zerg = false;
            zen = false;
            permafrostsConcoction = false;
			yPower = false;
			aWeapon = false;
			tScale = false;
			fabsolVodka = false;
			marked = false;
            cDepth = false;
            fishAlert = false;
            invincible = false;
            shine = false;
            anechoicCoating = false;
			mushy = false;
			molten = false;
            enraged = false;
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
            adrenalineMaxTimer = 300;
            adrenalineDmgDown = 600;
            adrenalineDmgMult = 1f;
			raiderStack = 0;
            fleshTotemCooldown = 0;
            astralStarRainCooldown = 0;
            tarraMageHealCooldown = 0;
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
            revivifyTimer = 0;
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
            tarraRangedCrits = 0;
            tarraRangedCritTimer = 0;
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
            bloodflareMageCrits = 0;
            bloodflareMageCritTimer = 0;
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
                    bossRushStage = 0;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(bossRushStage);
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
            if (CalamityWorld.armageddon && !CalamityGlobalNPC.AnyBossNPCS())
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
            int abyssChasmY = (y - abyssChasmSteps) + 100;
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
            ZoneAbyss = (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir) && 
                (((double)point.Y > (Main.rockLayer - (double)y * 0.05)) && //rocklayer = (0.4 to 0.3) - 0.04 = 0.36 to 0.26
                !player.lavaWet && 
                !player.honeyWet &&
                abyssPosY && 
                abyssPosX));

            ZoneAbyssLayer1 = ZoneAbyss &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.036); //(0.36 to 0.26) to (0.436 to 0.336)

            ZoneAbyssLayer2 = ZoneAbyss && 
                (double)point.Y > (Main.rockLayer + (double)y * 0.036) &&
                (double)point.Y <= (Main.rockLayer + (double)y * 0.143); //(0.436 to 0.336) to (0.543 to 0.443)

            ZoneAbyssLayer3 = ZoneAbyss && 
                (double)point.Y > (Main.rockLayer + (double)y * 0.143) && 
                (double)point.Y <= (Main.rockLayer + (double)y * 0.262); //(0.543 to 0.443) to (0.662 to 0.562)

            ZoneAbyssLayer4 = ZoneAbyss && 
                point.Y <= Main.maxTilesY - 200 && 
                (double)point.Y > (Main.rockLayer + (double)y * 0.262); //(0.662 to 0.562) to just above underworld

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
            /*if (ZoneSulphur)
            {
                return mod.GetTexture("Backgrounds/SulphurBG");
            }*/
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
				player.lifeRegen += 2;
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
                if (player.lifeRegen < 0)
                {
                    if (player.lifeRegenTime < 1800)
                    {
                        player.lifeRegenTime = 1800;
                    }
                    player.lifeRegen += 8;
                    player.statDefense += 20;
                }
                else
                {
                    player.lifeRegen += 2;
                }
            }
            else if (crownJewel)
            {
                if (player.lifeRegen < 0)
                {
                    if (player.lifeRegenTime < 1800)
                    {
                        player.lifeRegenTime = 1800;
                    }
                    player.lifeRegen += 4;
                    player.statDefense += 10;
                }
                else
                {
                    player.lifeRegen += 2;
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
                if (player.poisoned || player.onFire || player.GetModPlayer<CalamityPlayer>().bFlames)
                    player.lifeRegen += 4;
            }
            #endregion
            #region LastDebuffs
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
            if (warped)
            {
                player.velocity.Y *= 1.01f;
            }
            if (weakPetrification)
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
            if (CalamityWorld.revenge)
            {
                if (player.lifeRegenTime > defLifeRegenTime)
                {
                    int lifeRegenAboveCap = player.lifeRegenTime - defLifeRegenTime;
                    player.lifeRegenTime = defLifeRegenTime + (int)((double)lifeRegenAboveCap * 0.25);
                }
                if (player.lifeRegen > defLifeRegen)
                {
                    int lifeRegenAboveCap = player.lifeRegen - defLifeRegen;
                    player.lifeRegen = defLifeRegen + (int)((double)lifeRegenAboveCap * 0.25);
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
                if (shadeRegen || draedonsHeart || cFreeze)
                {
                    num2 *= 1.1f;
                }
                if (draedonsHeart && !shadeRegen && !cFreeze && 
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
                    player.lifeRegenTime += 8; //4
                    player.lifeRegen += 8; //4
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
            if (CalamityWorld.revenge)
            {
                if (player.lifeRegenTime > defLifeRegenTime)
                {
                    int lifeRegenAboveCap = player.lifeRegenTime - defLifeRegenTime;
                    player.lifeRegenTime = defLifeRegenTime + (int)((double)lifeRegenAboveCap * 0.25);
                }
                if (player.lifeRegen > defLifeRegen)
                {
                    int lifeRegenAboveCap = player.lifeRegen - defLifeRegen;
                    player.lifeRegen = defLifeRegen + (int)((double)lifeRegenAboveCap * 0.25);
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
                                if (nPC.FindBuffIndex(mod.BuffType("Enraged")) == -1)
                                {
                                    nPC.AddBuff(mod.BuffType("Enraged"), 600, false);
                                }
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
                if (item.type == mod.ItemType<Items.Armor.AbyssalDivingSuit>())
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
            {
                NOULOL();
            }
            if (weakPetrification)
            {
                WeakPetrification();
            }
        }
		
		public override void PostUpdateEquips()
		{
            if (NOU)
            {
                NOULOL();
            }
            if (weakPetrification)
            {
                WeakPetrification();
            }
		}

        public override void PostUpdateMiscEffects()
        {
            #region BossRush
            if (CalamityWorld.bossRushActive)
            {
                if (!deactivateStupidFuckingBullshit)
                {
                    deactivateStupidFuckingBullshit = true;
                    CalamityWorld.bossRushActive = false;
                    if (Main.netMode == 2)
                    {
                        NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
                    }
                }
                if (NPC.MoonLordCountdown > 0)
                {
                    NPC.MoonLordCountdown = 0;
                }
                if (!CalamityGlobalNPC.AnyBossNPCS())
                {
                    if (CalamityWorld.bossRushSpawnCountdown > 0)
                    {
                        CalamityWorld.bossRushSpawnCountdown--;
                        if (CalamityWorld.bossRushSpawnCountdown == 180 && bossRushStage == 26)
                        {
                            string key = "Mods.CalamityMod.BossRushTierThreeEndText2";
                            Color messageColor = Color.LightCoral;
                            if (Main.netMode == 0)
                            {
                                Main.NewText(Language.GetTextValue(key), messageColor);
                            }
                            else if (Main.netMode == 2)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                            }
                        }
                        if (CalamityWorld.bossRushSpawnCountdown == 210 && bossRushStage == 33)
                        {
                            string key = "Mods.CalamityMod.BossRushTierFourEndText2";
                            Color messageColor = Color.LightCoral;
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
                    if (CalamityWorld.bossRushSpawnCountdown <= 0)
                    {
                        CalamityWorld.bossRushSpawnCountdown = 60;
                        if (bossRushStage > 25)
                        {
                            CalamityWorld.bossRushSpawnCountdown += 120; //3 seconds
                        }
                        if (bossRushStage > 32)
                        {
                            CalamityWorld.bossRushSpawnCountdown += 180; //6 seconds
                        }
                        switch (bossRushStage)
                        {
                            case 9:
                                CalamityWorld.bossRushSpawnCountdown = 240;
                                break;
                            case 18:
                                CalamityWorld.bossRushSpawnCountdown = 300;
                                break;
                            case 25:
                                CalamityWorld.bossRushSpawnCountdown = 360;
                                break;
                            case 32:
                                CalamityWorld.bossRushSpawnCountdown = 420;
                                break;
                            default:
                                break;
                        }
                        if (bossRushStage == 13)
                        {
                            player.Spawn();
                        }
                        else if (bossRushStage == 36)
                        {
                            if (player.FindBuffIndex(mod.BuffType("ExtremeGravity")) > -1)
                            {
                                player.ClearBuff(mod.BuffType("ExtremeGravity"));
                            }
                        }
                        if (Main.netMode != 1)
                        {
                            Main.PlaySound(SoundID.Roar, player.position, 0);
                            switch (bossRushStage)
                            {
                                case 0:
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.QueenBee);
                                    break;
                                case 1:
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.BrainofCthulhu);
                                    break;
                                case 2:
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.KingSlime);
                                    break;
                                case 3:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.EyeofCthulhu);
                                    break;
                                case 4:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.SkeletronPrime);
                                    break;
                                case 5:
                                    NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-50, 51))), (int)(player.position.Y - 150f), NPCID.Golem, 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 6:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("ProfanedGuardianBoss"));
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("ProfanedGuardianBoss2"));
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("ProfanedGuardianBoss3"));
                                    break;
                                case 7:
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.EaterofWorldsHead);
                                    break;
                                case 8:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Astrageldon"));
                                    break;
                                case 9:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.TheDestroyer);
                                    break;
                                case 10:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.Spazmatism);
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.Retinazer);
                                    break;
                                case 11:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Bumblefuck"));
                                    break;
                                case 12:
                                    NPC.SpawnWOF(player.position);
                                    break;
                                case 13:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("HiveMind"));
                                    break;
                                case 14:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.SkeletronHead);
                                    break;
                                case 15:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("StormWeaverHead"));
                                    break;
                                case 16:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("AquaticScourgeHead"));
                                    break;
                                case 17:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertScourgeHead"));
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertScourgeHeadSmall"));
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DesertScourgeHeadSmall"));
                                    break;
                                case 18:
                                    int num1302 = NPC.NewNPC((int)player.Center.X + 30, (int)player.Center.Y - 2, NPCID.CultistBoss, 0, 0f, 0f, 0f, 0f, 255);
                                    Main.npc[num1302].direction = (Main.npc[num1302].spriteDirection = Math.Sign(player.Center.X - (float)player.Center.X - 30f));
                                    break;
                                case 19:
                                    for (int doom = 0; doom < 200; doom++)
                                    {
                                        if (Main.npc[doom].active && (Main.npc[doom].type == 493 || Main.npc[doom].type == 422 || Main.npc[doom].type == 507 ||
                                            Main.npc[doom].type == 517))
                                        {
                                            Main.npc[doom].active = false;
                                            Main.npc[doom].netUpdate = true;
                                        }
                                    }
                                    NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-50, 51))), (int)(player.position.Y - 50f), mod.NPCType("CrabulonIdle"), 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 20:
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.Plantera);
                                    break;
                                case 21:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("CeaselessVoid"));
                                    break;
                                case 22:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("PerforatorHive"));
                                    break;
                                case 23:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Cryogen"));
                                    break;
                                case 24:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("BrimstoneElemental"));
                                    break;
                                case 25:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("CosmicWraith"));
                                    break;
                                case 26:
                                    NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-100, 101))), (int)(player.position.Y - 250f), mod.NPCType("ScavengerBody"), 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 27:
                                    NPC.NewNPC((int)(player.position.X + (float)(Main.rand.Next(-100, 101))), (int)(player.position.Y - 250f), NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 28:
                                    NPC.SpawnOnPlayer(player.whoAmI, NPCID.MoonLordCore);
                                    break;
                                case 29:
                                    ChangeTime(false);
                                    for (int x = 0; x < 10; x++)
                                    {
                                        NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("AstrumDeusHead"));
                                    }
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("AstrumDeusHeadSpectral"));
                                    break;
                                case 30:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Polterghast"));
                                    break;
                                case 31:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("PlaguebringerGoliath"));
                                    break;
                                case 32:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Calamitas"));
                                    break;
                                case 33:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Siren"));
                                    break;
                                case 34:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SlimeGod"));
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SlimeGodRun"));
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SlimeGodCore"));
                                    break;
                                case 35:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Providence"));
                                    break;
                                case 36:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("SupremeCalamitas"));
                                    break;
                                case 37:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("Yharon"));
                                    break;
                                case 38:
                                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("DevourerofGodsHeadS"));
                                    break;
                            }
                        }
                    }
                }
            }
            else
            {
                CalamityWorld.bossRushSpawnCountdown = 180;
                if (bossRushStage != 0)
                {
                    bossRushStage = 0;
                    if (Main.netMode == 2)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(bossRushStage);
                        netMessage.Send();
                    }
                }
            }
            #endregion
            #region LORESTATUR
            soulEssence = 0 +
                (NPC.downedSlimeKing ? 20 : 0) + //20
                (NPC.downedBoss1 ? 30 : 0) + //50
                (NPC.downedBoss2 ? 50 : 0) + //100
                (NPC.downedBoss3 ? 100 : 0) + //200
                (NPC.downedQueenBee ? 50 : 0) + //250
                (NPC.downedMechBoss1 ? 250 : 0) + //500
                (NPC.downedMechBoss2 ? 300 : 0) + //800
                (NPC.downedMechBoss3 ? 350 : 0) + //1150
                (NPC.downedPlantBoss ? 450 : 0) + //1600
                (NPC.downedGolemBoss ? 500 : 0) + //2100
                (NPC.downedAncientCultist ? 600 : 0) + //2700
                (NPC.downedMoonlord ? 1000 : 0) + //3700
                (CalamityWorld.downedDesertScourge ? 50 : 0) + //3750
                (CalamityWorld.downedCrabulon ? 50 : 0) + //3800
                ((CalamityWorld.downedHiveMind || CalamityWorld.downedPerforator) ? 100 : 0) + //3900
                (CalamityWorld.downedSlimeGod ? 200 : 0) + //4100
                (Main.hardMode ? 200 : 0) + //4300
                (CalamityWorld.downedCryogen ? 250 : 0) + //4550
                (CalamityWorld.downedAquaticScourge ? 300 : 0) + //4850
                (CalamityWorld.downedBrimstoneElemental ? 350 : 0) + //5200
                (CalamityWorld.downedCalamitas ? 400 : 0) + //5600
                (CalamityWorld.downedLeviathan ? 800 : 0) + //6400
                (CalamityWorld.downedAstrageldon ? 400 : 0) + //6800
                (CalamityWorld.downedStarGod ? 700 : 0) + //7500
                (CalamityWorld.downedPlaguebringer ? 800 : 0) + //8300
                (CalamityWorld.downedScavenger ? 800 : 0) + //9100
                (NPC.downedFishron ? 800 : 0) + //9900
                (CalamityWorld.downedGuardians ? 900 : 0) + //10800
                (CalamityWorld.downedProvidence ? 2500 : 0) + //13300
                (CalamityWorld.downedSentinel1 ? 1000 : 0) + //14300
                (CalamityWorld.downedSentinel2 ? 1000 : 0) + //15300
                (CalamityWorld.downedSentinel3 ? 2000 : 0) + //17300
                (CalamityWorld.downedPolterghast ? 2700 : 0) + //20000
                (CalamityWorld.downedDoG ? 4000 : 0) + //24000
                (CalamityWorld.downedBumble ? 1000 : 0) + //25000
                (CalamityWorld.downedYharon ? 5000 : 0) + //30000
                (CalamityWorld.downedSCal ? 7500 : 0); //37500
            #endregion
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
                        stressGain += 35;
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
                        stress = stressMax;
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
                    adrenalineMaxTimer = 300;
                    adrenalineDmgMult = 1f;
                }
                adrenalineDmgMult = 0.1f * (float)(adrenalineDmgDown / 120);
                if (adrenalineDmgMult < 0.25f)
                    adrenalineDmgMult = 0.25f;
                int adrenalineGain = 0;
                if (adrenalineMode)
                {
                    adrenalineGain = (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")) ? -10000 : -2000);
                }
                else
                {
                    if (Main.wof >= 0 && player.position.Y < (float)((Main.maxTilesY - 200) * 16)) // >
                    {
                        adrenaline = 0;
                    }
                    else if (CalamityGlobalNPC.AnyBossNPCS() || CalamityGlobalNPC.DoGSecondStageCountdown > 0)
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
                if (adrenalineCD >= (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")) ? 135 : 60))
                {
                    adrenalineCD = 0;
                    adrenaline += adrenalineGain;
                    if (adrenaline < 0)
                    {
                        adrenaline = 0;
                    }
                    if (adrenaline >= adrenalineMax)
                    {
                        adrenaline = adrenalineMax;
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
                player.thrownDamage += 0.05f;
                player.rangedDamage += 0.05f;
                player.meleeDamage += 0.05f;
                player.magicDamage += 0.05f;
                player.minionDamage += 0.05f;
            }
            #endregion
            #region StressTiedEffects
            if (stressPills)
            {
                player.statDefense += 5;
                player.meleeCrit += 5;
                player.meleeDamage += 0.05f;
                player.magicCrit += 5;
                player.magicDamage += 0.05f;
                player.rangedCrit += 5;
                player.rangedDamage += 0.05f;
                player.thrownCrit += 5;
                player.thrownDamage += 0.05f;
                player.minionDamage += 0.05f;
            }
            if (laudanum)
            {
                player.statDefense += 8;
                player.meleeCrit += 6;
                player.meleeDamage += 0.06f;
                player.magicCrit += 6;
                player.magicDamage += 0.06f;
                player.rangedCrit += 6;
                player.rangedDamage += 0.06f;
                player.thrownCrit += 6;
                player.thrownDamage += 0.06f;
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
                    player.thrownDamage += 0.1f;
                    player.rangedDamage += 0.1f;
                    player.meleeDamage += 0.1f;
                    player.magicDamage += 0.1f;
                    player.minionDamage += 0.1f;
                }
                player.lifeRegen += 1;
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * 5;
            }
            if (affliction)
            {
                player.lifeRegen += CalamityWorld.revenge ? 2 : 1;
                player.endurance += CalamityWorld.revenge ? 0.07f : 0.05f;
                player.statDefense += CalamityWorld.revenge ? 20 : 15;
                player.thrownDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.rangedDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.meleeDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.magicDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.minionDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
                player.statLifeMax2 += CalamityWorld.revenge ? (player.statLifeMax / 5 / 20 * 10) : (player.statLifeMax / 5 / 20 * 5);
            }
            else if (afflicted)
            {
                player.lifeRegen += CalamityWorld.revenge ? 2 : 1;
                player.endurance += CalamityWorld.revenge ? 0.07f : 0.05f;
                player.statDefense += CalamityWorld.revenge ? 20 : 15;
                player.thrownDamage += CalamityWorld.revenge ? 0.12f : 0.1f;
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
            if (silvaSet && silvaHitCounter > 0)
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
                (cShard ? 50 : 0);
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
                else if (Main.raining && NPC.downedMoonlord) { Main.rainTexture = rain3; }
                else { Main.rainTexture = rainOriginal; }
                if (auricSet) { Main.flyingCarpetTexture = carpetAuric; }
                else { Main.flyingCarpetTexture = carpetOriginal; }
            }
            #endregion
            #region MiscEffects
            if (Main.myPlayer == player.whoAmI)
            {
                BossHealthBarManager.SHOULD_DRAW_SMALLTEXT_HEALTH = shouldDrawSmallText;
            }
            if (silvaSet || invincible)
            {
                List<int> debuffList = new List<int>(39)
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
                    mod.BuffType("BrimstoneFlames"),
                    mod.BuffType("BurningBlood"),
                    mod.BuffType("GlacialState"),
                    mod.BuffType("GodSlayerInferno"),
                    mod.BuffType("HolyLight"),
                    mod.BuffType("Irradiated"),
                    mod.BuffType("Plague"),
                    mod.BuffType("BrimstoneFlames"),
                    mod.BuffType("AbyssalFlames")
                };
                foreach (int debuff in debuffList) { player.buffImmune[debuff] = true; }
            }
            if (Main.expertMode && player.ZoneSnow && player.wet && !player.lavaWet && !player.honeyWet && !player.arcticDivingGear)
            {
                player.buffImmune[BuffID.Chilled] = true;
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    if (Main.myPlayer == player.whoAmI && !player.gills && !player.merman)
                    {
                        player.breath--;
                    }
                }
            }
            if (ZoneAbyss)
            {
                if (abyssalAmulet) { player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * (lumenousAmulet ? 15 : 10); }
                player.detectCreature = false;
                if (player.chaosState && !NPC.AnyNPCs(mod.NPCType("EidolonWyrmHeadHuge")) && Main.netMode != 1)
                {
                    NPC.SpawnOnPlayer(player.whoAmI, mod.NPCType("EidolonWyrmHeadHuge"));
                }
                if (Main.myPlayer == player.whoAmI) //4200 total tiles small world
                {
                    Point point = player.Center.ToTileCoordinates();
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
                        player.breath -= 3;
                    }
                    if (abyssBreathCD >= tick)
                    {
                        abyssBreathCD = 0;
                        player.breath -= breathLoss;
                        if (cDepth)
                        {
                            player.breath -= 1;
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
            if (revivifyTimer > 0) { revivifyTimer--; }
            if (fleshTotemCooldown > 0) { fleshTotemCooldown--; }
            if (astralStarRainCooldown > 0) { astralStarRainCooldown--; }
            if (tarraMageHealCooldown > 0) { tarraMageHealCooldown--; }
            if (ataxiaDmg > 0) { ataxiaDmg--; }
            if (xerocDmg > 0) { xerocDmg--; }
            if (godSlayerDmg > 0) { godSlayerDmg--; }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaSet)
            {
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
            if (tarraRanged)
            {
                if (tarraRangedCritTimer > 0) { tarraRangedCritTimer--; }
                if (tarraRangedCritTimer <= 0)
                {
                    if (tarraRangedCrits > 0) { tarraRangedCrits -= 10; }
                    if (tarraRangedCrits < 0) { tarraRangedCrits = 0; }
                    tarraRangedCritTimer = 120;
                }
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
                List<int> debuffList = new List<int>(47)
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
                    mod.BuffType("BrimstoneFlames"),
                    mod.BuffType("BurningBlood"),
                    mod.BuffType("GlacialState"),
                    mod.BuffType("GodSlayerInferno"),
                    mod.BuffType("HolyLight"),
                    mod.BuffType("Irradiated"),
                    mod.BuffType("Plague"),
                    mod.BuffType("BrimstoneFlames"),
                    mod.BuffType("AbyssalFlames"),
                    mod.BuffType("CrushDepth"),
                    mod.BuffType("ExtremeGrav"),
                    mod.BuffType("ExtremeGravity"),
                    mod.BuffType("Horror"),
                    mod.BuffType("MarkedforDeath"),
                    mod.BuffType("Warped"),
                    mod.BuffType("WeakPetrification"),
                    mod.BuffType("VulnerabilityHex")
                };
                for (int l = 0; l < 22; l++)
                {
                    int hasBuff = player.buffType[l];
                    bool shouldAffect = debuffList.Contains(hasBuff);
                    if (shouldAffect)
                    {
                        player.thrownDamage += 0.1f;
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
            if (bloodflareMage)
            {
                if (bloodflareMageCritTimer > 0) { bloodflareMageCritTimer--; }
                if (bloodflareMageCritTimer <= 0)
                {
                    if (bloodflareMageCrits > 0) { bloodflareMageCrits -= 10; }
                    if (bloodflareMageCrits < 0) { bloodflareMageCrits = 0; }
                    bloodflareMageCritTimer = 120;
                }
            }
            if (Main.raining && NPC.downedMoonlord)
            {
                if (player.ZoneOverworldHeight || player.ZoneSkyHeight) { player.AddBuff(mod.BuffType("Irradiated"), 2); }
            }
            if (raiderTalisman) { player.thrownDamage += ((float)raiderStack / 50f) * 0.15f; }
            if (tarraRangedCrits > 150) { tarraRangedCrits = 150; }
            if (tarraRanged) { player.rangedDamage += (float)(tarraRangedCrits / 15); }
            if (bloodflareMageCrits > 150) { bloodflareMageCrits = 150; }
            if (bloodflareMage) { player.magicDamage += (float)(bloodflareMageCrits / 10); }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaMage)
            {
                player.magicCrit += 10;
            }
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
                player.endurance += 0.06f;
                if ((double)Math.Abs(player.velocity.X) < 0.05 && (double)Math.Abs(player.velocity.Y) < 0.05 && player.itemAnimation == 0)
                {
                    player.lifeRegen += 3;
                    player.manaRegenBonus += 2;
                }
                if (Collision.DrownCollision(player.position, player.width, player.height, player.gravDir))
                {
                    player.statDefense += 5;
                    player.endurance += 0.07f;
                    player.moveSpeed += 0.2f;
                }
            }
            if (coreOfTheBloodGod) { player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 10; }
            if (bloodPact) { player.statLifeMax2 += player.statLifeMax2 / 5 / 20 * 100; }
            if (aAmpoule)
            {
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 1f, 1f, 0.6f);
                player.endurance += 0.06f;
                player.pickSpeed -= 0.5f;
                player.buffImmune[70] = true;
                player.buffImmune[47] = true;
                player.buffImmune[46] = true;
                player.buffImmune[44] = true;
                player.buffImmune[20] = true;
                if (!player.honey && player.lifeRegen < 0)
                {
                    player.lifeRegen += 4;
                    if (player.lifeRegen > 0)
                    {
                        player.lifeRegen = 0;
                    }
                }
                player.lifeRegenTime += 2;
                player.lifeRegen += 4;
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
                Lighting.AddLight((int)(player.position.X + (float)(player.width / 2)) / 16, (int)(player.position.Y + (float)(player.height / 2)) / 16, 0.55f, 1.5f, 0.55f);
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
                player.meleeCrit += (int)((1f - modStealth) * 20f);
                player.rangedDamage += (1f - modStealth) * 0.2f;
                player.rangedCrit += (int)((1f - modStealth) * 20f);
                player.magicDamage += (1f - modStealth) * 0.2f;
                player.magicCrit += (int)((1f - modStealth) * 20f);
                player.thrownDamage += (1f - modStealth) * 0.2f;
                player.thrownCrit += (int)((1f - modStealth) * 20f);
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
                player.meleeDamage += (1f - modStealth) * 0.25f;
                player.meleeCrit += (int)((1f - modStealth) * 25f);
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
                    player.thrownDamage += (5f - shieldInvinc) * 0.02f;
                    player.thrownCrit += (int)((5f - shieldInvinc) * 2f);
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
                player.statDefense -= 5 +
                    (CalamityWorld.revenge ? 5 : 0);
                player.endurance -= 0.05f +
                    (CalamityWorld.revenge ? 0.05f : 0f);
                player.meleeCrit += 5;
                player.meleeDamage += 0.05f;
                player.magicCrit += 5;
                player.magicDamage += 0.05f;
                player.rangedCrit += 5;
                player.rangedDamage += 0.05f;
                player.thrownCrit += 5;
                player.thrownDamage += 0.05f;
                player.minionDamage += 0.05f;
                player.minionKB += 0.5f;
                player.moveSpeed += 0.05f;
            }
            if (rRage)
            {
                player.meleeCrit += 5;
                player.meleeDamage += 0.05f;
                player.magicCrit += 5;
                player.magicDamage += 0.05f;
                player.rangedCrit += 5;
                player.rangedDamage += 0.05f;
                player.thrownCrit += 5;
                player.thrownDamage += 0.05f;
                player.minionDamage += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.moveSpeed += 0.05f;
            }
            if (xRage)
            {
                player.thrownDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
            }
            if (xWrath)
            {
                player.meleeCrit += 10;
                player.magicCrit += 10;
                player.rangedCrit += 10;
                player.thrownCrit += 10;
            }
            if (godSlayerCooldown)
            {
                player.thrownDamage += 0.1f;
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
                player.meleeCrit += 10;
            }
            if (sMeleeBoost)
            {
                player.meleeSpeed -= 0.05f;
                player.meleeDamage += 0.1f;
                player.meleeCrit += 10;
            }
            if (tFury)
            {
                player.meleeDamage += 0.3f;
                player.meleeCrit += 30;
            }
            if (yPower)
            {
                player.endurance += 0.05f;
                player.statDefense += 5;
                player.meleeCrit += 5;
                player.meleeDamage += 0.05f;
                player.meleeSpeed -= 0.05f;
                player.magicCrit += 5;
                player.magicDamage += 0.05f;
                player.rangedCrit += 5;
                player.rangedDamage += 0.05f;
                player.thrownCrit += 5;
                player.thrownDamage += 0.05f;
                player.minionDamage += 0.05f;
                player.minionKB += 1f;
                player.moveSpeed += 0.15f;
            }
            if (tScale)
            {
                player.endurance += 0.05f;
                player.statDefense += 5;
                player.kbBuff = true;
            }
            if (fabsolVodka)
            {
                player.thrownDamage += 0.1f;
                player.rangedDamage += 0.1f;
                player.meleeDamage += 0.1f;
                player.magicDamage += 0.1f;
                player.minionDamage += 0.1f;
                player.statDefense -= 20;
            }
            if (nanotech && player.inventory[player.selectedItem].thrown)
            {
                player.endurance += 0.1f;
                player.statDefense += 15;
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
                int regenBoost = 1 + (integerTypeBoost / 5);
                player.endurance += (floatTypeBoost * 0.5f);
                player.statDefense += integerTypeBoost;
                player.meleeCrit += integerTypeBoost;
                player.meleeDamage += floatTypeBoost;
                player.meleeSpeed -= (floatTypeBoost * 0.25f);
                player.magicCrit += integerTypeBoost;
                player.magicDamage += floatTypeBoost;
                player.rangedCrit += integerTypeBoost;
                player.rangedDamage += floatTypeBoost;
                player.thrownCrit += integerTypeBoost;
                player.thrownDamage += floatTypeBoost;
                player.minionDamage += floatTypeBoost;
                player.minionKB += floatTypeBoost;
                player.moveSpeed += floatTypeBoost;
                player.statLifeMax2 += player.statLifeMax / 5 / 20 * integerTypeBoost;
                if (player.lifeRegen < 0)
                {
                    player.lifeRegen += regenBoost;
                }
                if (player.wingTimeMax > 0)
                {
                    player.wingTimeMax = (int)((double)player.wingTimeMax * 1.15);
                }
            }
            if (CalamityWorld.defiled)
            {
                player.statLifeMax2 /= 2;
                if (player.statLife > player.statLifeMax2)
                {
                    player.statLife = player.statLifeMax2;
                }
            }
            if (reaperToothNecklace)
            {
                player.meleeCrit += 25;
                player.meleeDamage += 0.25f;
                player.magicCrit += 25;
                player.magicDamage += 0.25f;
                player.rangedCrit += 25;
                player.rangedDamage += 0.25f;
                player.thrownCrit += 25;
                player.thrownDamage += 0.25f;
                player.minionDamage += 0.25f;
            }
            if (coreOfTheBloodGod)
            {
                if (player.whoAmI == Main.myPlayer && Main.netMode != 1)
                {
                    int drain = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, 476, 40, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[drain].usesLocalNPCImmunity = true;
                    Main.projectile[drain].localNPCHitCooldown = 5;
                }
                player.endurance += 0.1f;
                player.meleeCrit += 12;
                player.meleeDamage += 0.12f;
                player.magicCrit += 12;
                player.magicDamage += 0.12f;
                player.rangedCrit += 12;
                player.rangedDamage += 0.12f;
                player.thrownCrit += 12;
                player.thrownDamage += 0.12f;
                player.minionDamage += 0.12f;
                if (player.statDefense < 100)
                {
                    player.meleeDamage += 0.15f;
                    player.magicDamage += 0.15f;
                    player.rangedDamage += 0.15f;
                    player.thrownDamage += 0.15f;
                    player.minionDamage += 0.15f;
                }
            }
            else if (bloodflareCore)
            {
                if (player.whoAmI == Main.myPlayer && Main.netMode != 1)
                {
                    int drain = Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, 476, 40, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[drain].usesLocalNPCImmunity = true;
                    Main.projectile[drain].localNPCHitCooldown = 8;
                }
                if (player.statDefense < 100)
                {
                    player.meleeDamage += 0.15f;
                    player.magicDamage += 0.15f;
                    player.rangedDamage += 0.15f;
                    player.thrownDamage += 0.15f;
                    player.minionDamage += 0.15f;
                }
                if (player.statLife <= (int)((double)player.statLifeMax2 * 0.15))
                {
                    player.endurance += 0.15f;
                    player.meleeCrit += 15;
                    player.meleeDamage += 0.15f;
                    player.magicCrit += 15;
                    player.magicDamage += 0.15f;
                    player.rangedCrit += 15;
                    player.rangedDamage += 0.15f;
                    player.thrownCrit += 15;
                    player.thrownDamage += 0.15f;
                    player.minionDamage += 0.15f;
                }
                else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.endurance += 0.1f;
                    player.meleeCrit += 10;
                    player.meleeDamage += 0.1f;
                    player.magicCrit += 10;
                    player.magicDamage += 0.1f;
                    player.rangedCrit += 10;
                    player.rangedDamage += 0.1f;
                    player.thrownCrit += 10;
                    player.thrownDamage += 0.1f;
                    player.minionDamage += 0.1f;
                }
            }
            if (godSlayerThrowing)
            {
                if (player.statLife >= player.statLifeMax2)
                {
                    player.thrownCrit += 15;
                    player.thrownDamage += 0.15f;
                    player.thrownVelocity += 0.15f;
                }
            }
            if (tarraSummon)
            {
                int lifeCounter = 0;
                Lighting.AddLight((int)(player.Center.X / 16f), (int)(player.Center.Y / 16f), 0f, 3f, 0f);
                float num2 = 300f;
                bool flag = lifeCounter % 60 == 0;
                int num3 = 300;
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
            if (bloodflareThrowing)
            {
                if (player.statLife > (int)((double)player.statLifeMax2 * 0.8))
                {
                    player.thrownCrit += 5;
                    player.statDefense += 30;
                }
                else
                {
                    player.thrownDamage += 0.15f;
                }
            }
            if (bloodflareSummon)
            {
                if (player.statLife >= (int)((double)player.statLifeMax2 * 0.9))
                {
                    player.minionDamage += 0.15f;
                }
                else if (player.statLife <= (int)((double)player.statLifeMax2 * 0.5))
                {
                    player.statDefense += 20;
                    player.lifeRegen += 3;
                }
                if (bloodflareSummonTimer > 0) { bloodflareSummonTimer--; }
                if (Main.netMode != 1 && bloodflareSummonTimer <= 0)
                {
                    bloodflareSummonTimer = 900;
                    for (int I = 0; I < 3; I++)
                    {
                        float ai1 = (float)(I * 120);
                        int mine = Projectile.NewProjectile(player.Center.X + (float)(Math.Sin(I * 120) * 550), player.Center.Y + (float)(Math.Cos(I * 120) * 550), 0f, 0f,
                            mod.ProjectileType("GhostlyMine"), (int)((auricSet ? 15000f : 5000f) * player.minionDamage), 1f, player.whoAmI, ai1, 0f);
                    }
                }
            }
            #endregion
            #region DRAndDamageLimits
            if (CalamityWorld.revenge)
            {
                if (player.lifeRegenTime > defLifeRegenTime)
                {
                    int lifeRegenAboveCap = player.lifeRegenTime - defLifeRegenTime;
                    player.lifeRegenTime = defLifeRegenTime + (int)((double)lifeRegenAboveCap * 0.25);
                }
                if (player.lifeRegen > defLifeRegen)
                {
                    int lifeRegenAboveCap = player.lifeRegen - defLifeRegen;
                    player.lifeRegen = defLifeRegen + (int)((double)lifeRegenAboveCap * 0.25);
                }
                if (player.endurance > defEndurance) //0.33
                {
                    float damageReductionAboveCap = player.endurance - defEndurance; //0.6 - 0.33 = 0.27
                    player.endurance = defEndurance + (damageReductionAboveCap * 0.1f); //0.33 + (0.27 * 0.1) = 0.357
                }
                if (!dsSetBonus)
                {
                    if (player.meleeDamage > defDamage)
                    {
                        float damageAboveCap = player.meleeDamage - defDamage;
                        player.meleeDamage = defDamage + (damageAboveCap * 0.25f);
                    }
                    if (auricSet && silvaMelee)
                    {
                        double multiplier = (double)player.statLife / (double)player.statLifeMax2;
                        player.meleeDamage += (float)(multiplier * 0.5); //ranges from 1.5 times to 1 times
                    }
                    if (player.rangedDamage > defDamage)
                    {
                        float damageAboveCap = player.rangedDamage - defDamage;
                        player.rangedDamage = defDamage + (damageAboveCap * 0.25f);
                    }
                    if (player.magicDamage > defDamage)
                    {
                        float damageAboveCap = player.magicDamage - defDamage;
                        player.magicDamage = defDamage + (damageAboveCap * 0.25f);
                    }
                    if (player.thrownDamage > defDamage)
                    {
                        float damageAboveCap = player.thrownDamage - defDamage;
                        player.thrownDamage = defDamage + (damageAboveCap * 0.25f);
                    }
                    if (!auricSet)
                    {
                        if (player.minionDamage > defDamage)
                        {
                            float damageAboveCap = player.minionDamage - defDamage;
                            player.minionDamage = defDamage + (damageAboveCap * 0.25f);
                        }
                    }
                    if (player.meleeCrit > defCrit)
                    {
                        int critAboveCap = player.meleeCrit - defCrit;
                        player.meleeCrit = defCrit + (int)((double)critAboveCap * 0.1);
                    }
                    if (player.rangedCrit > defCrit)
                    {
                        int critAboveCap = player.rangedCrit - defCrit;
                        player.rangedCrit = defCrit + (int)((double)critAboveCap * 0.1);
                    }
                    if (player.magicCrit > defCrit)
                    {
                        int critAboveCap = player.magicCrit - defCrit;
                        player.magicCrit = defCrit + (int)((double)critAboveCap * 0.1);
                    }
                    if (player.thrownCrit > defCrit)
                    {
                        int critAboveCap = player.thrownCrit - defCrit;
                        player.thrownCrit = defCrit + (int)((double)critAboveCap * 0.1);
                    }
                }
                if (rageMode && adrenalineMode)
                {
                    player.thrownDamage += (CalamityWorld.death ? 12f : 3f);
                    player.rangedDamage += (CalamityWorld.death ? 12f : 3f);
                    player.meleeDamage += (CalamityWorld.death ? 12f : 3f);
                    player.magicDamage += (CalamityWorld.death ? 12f : 3f);
                }
                else if (rageMode)
                {
                    float rageDamageBoost = 0f +
                        (rageBoostOne ? (CalamityWorld.death ? 0.8f : 0.2f) : 0f) + //4.8 or 1.2
                        (rageBoostTwo ? (CalamityWorld.death ? 0.8f : 0.2f) : 0f) + //5.6 or 1.4
                        (rageBoostThree ? (CalamityWorld.death ? 0.8f : 0.2f) : 0f); //6.4 or 1.6
                    float rageDamage = (CalamityWorld.death ? 4f : 1f) + rageDamageBoost;
                    player.thrownDamage += rageDamage;
                    player.rangedDamage += rageDamage;
                    player.meleeDamage += rageDamage;
                    player.magicDamage += rageDamage;
                }
                else if (adrenalineMode)
                {
                    float adrenalineDamageMult = (CalamityWorld.death ? 10f : 2.5f) * adrenalineDmgMult;
                    player.thrownDamage += adrenalineDamageMult;
                    player.rangedDamage += adrenalineDamageMult;
                    player.meleeDamage += adrenalineDamageMult;
                    player.magicDamage += adrenalineDamageMult;
                }
            }
            if (dArtifact)
            {
                player.meleeCrit += 15;
                player.meleeDamage += 0.15f;
                player.magicCrit += 15;
                player.magicDamage += 0.15f;
                player.rangedCrit += 15;
                player.rangedDamage += 0.15f;
                player.thrownCrit += 15;
                player.thrownDamage += 0.15f;
            }
            if (eArtifact)
            {
                player.meleeSpeed -= 0.1f;
                player.manaCost *= 0.75f;
                player.thrownDamage += 0.15f;
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
                    if (player.ownedProjectileCounts[mod.ProjectileType("AngryChicken")] < 2 && Main.netMode != 1)
                    {
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("AngryChicken"), (int)(232f * player.minionDamage), 2f, Main.myPlayer, 0f, 0f);
                    }
                }
            }
            if (marked || reaperToothNecklace)
            {
                player.endurance *= 0.5f;
            }
            if (reaperToothNecklace)
            {
                player.statDefense /= 2;
            }
            #endregion
            #region DespawnProjectilesWhenRageOrAdrenalineStop
            if (player.whoAmI == Main.myPlayer)
            {
                if ((rageMode || adrenalineMode) && !despawnProj)
                {
                    despawnProj = true;
                }
                if (!rageMode && !adrenalineMode && despawnProj)
                {
                    int proj;
                    for (int x = 0; x < 1000; x = proj + 1)
                    {
                        Projectile projectile = Main.projectile[x];
                        if ((projectile.active && projectile.owner == Main.myPlayer && 
                           (projectile.friendly || projectile.timeLeft > 300) && 
                           !projectile.hostile && !projectile.minion && projectile.aiStyle != 7 && !projectile.sentry) ||
                           projectile.aiStyle == 36)
                        {
                            projectile.Kill();
                        }
                        proj = x;
                    }
                    despawnProj = false;
                }
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
            if (invincible)
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
				damageSource = PlayerDeathReason.ByCustomReason(player.name + " became a blood geyser");
			}
			if ((bFlames || aFlames) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + " was consumed by the black flames");
			}
			if (pFlames && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + "'s flesh was melted by the Plague");
			}
			if ((hFlames || hInferno) && damage == 10.0 && hitDirection == 0 && damageSource.SourceOtherIndex == 8)
			{
				damageSource = PlayerDeathReason.ByCustomReason(player.name + " fell prey to their sins");
			}
            if (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
            {
                if (sCalDeathCount < 51)
                {
                    sCalDeathCount++;
                }
            }
            if (CalamityWorld.ironHeart && CalamityGlobalNPC.AnyBossNPCS())
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
            if (silvaRanged && item.ranged && item.useTime > 3)
            {
                return (auricSet ? 1.2f : 1.1f);
            }
            if (player.statLife > (int)((double)player.statLifeMax2 * 0.9) && silvaThrowing && item.thrown && item.useTime > 3)
            {
                return 1.1f;
            }
            return 1f;
        }
        #endregion

        #region MeleeSpeedMult
        public override float MeleeSpeedMultiplier(Item item)
        {
            if (player.meleeSpeed > 1.3f && !eArtifact)
            {
                return (1.3f / player.meleeSpeed);
            }
            return 1f;
        }
        #endregion

        #region GetWeaponDamageAndKB
        public override void GetWeaponDamage(Item item, ref int damage)
		{
			if (dodgeScarf && item.melee && item.shoot == 0)
			{
				damage = (int)((double)damage * 1.2);
			}
			if (flamethrowerBoost && item.ranged && item.useAmmo == 23)
			{
				damage = (int)((double)damage * 1.25);
			}
            if (item.type == ItemID.TerraBlade)
            {
                damage = (int)((double)damage * 1.66);
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
		}
        #endregion

        #region Shoot
        public override bool Shoot(Item item, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
		{
            if (eArtifact && item.ranged)
            {
                speedX *= 1.25f;
                speedY *= 1.25f;
            }
            if (bloodflareMage && item.magic && Main.rand.Next(0, 100) >= 95) //0 - 99
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("GhostlyBolt"), (int)((double)damage * 2.6), 1f, player.whoAmI, 0f, 0f);
                }
            }
            if (bloodflareRanged && item.ranged && Main.rand.Next(0, 100) >= (auricSet ? 98 : 95)) //0 - 99
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX, speedY, mod.ProjectileType("BloodBomb"), (int)((double)damage * 1.6), 2f, player.whoAmI, 0f, 0f);
                }
            }
            if (tarraMage && tarraCrits >= 5)
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
			if (ataxiaBolt && item.ranged && Main.rand.Next(2) == 0)
			{
				if (player.whoAmI == Main.myPlayer)
				{
					Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("ChaosFlare"), (int)((double)damage * 0.25), 2f, player.whoAmI, 0f, 0f);
				}
			}
            if (godSlayerRanged && item.ranged && Main.rand.Next(0, 100) >= 95) //0 - 99
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("GodSlayerShrapnelRound"), (int)((double)damage * 2.1), 2f, player.whoAmI, 0f, 0f);
                }
            }
            if (ataxiaVolley && item.thrown && Main.rand.Next(10) == 0)
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
						Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), mod.ProjectileType("ChaosFlare2"), (int)((double)damage * 0.25), 1.25f, player.whoAmI, 0f, 0f);
						Projectile.NewProjectile(vector2.X, vector2.Y, (float)( -Math.Sin(offsetAngle) * 5f ), (float)( -Math.Cos(offsetAngle) * 5f ), mod.ProjectileType("ChaosFlare2"), (int)((double)damage * 0.25), 1.25f, player.whoAmI, 0f, 0f);
					}
				}
			}
			if (reaverDoubleTap && item.ranged && Main.rand.Next(0, 100) >= 90) //0 - 99
			{
				if (player.whoAmI == Main.myPlayer)
				{
					Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("MiniRocket"), (int)((double)damage * 1.8), 2f, player.whoAmI, 0f, 0f);
				}
			}
            if (victideSet && (item.ranged || item.melee || item.magic || item.thrown || item.summon) && Main.rand.Next(10) == 0)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(position.X, position.Y, speedX * 1.25f, speedY * 1.25f, mod.ProjectileType("Seashell"), damage * 2, 1f, player.whoAmI, 0f, 0f);
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
            if (aChicken)
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
                if (player.whoAmI == Main.myPlayer)
                {
                    Projectile.NewProjectile(target.Center.X, target.Center.Y, 0f, 0f, mod.ProjectileType("ChaosGeyser"), (int)((double)item.damage * 0.25), 2f, player.whoAmI, 0f, 0f);
                }
            }
            if (target.type == NPCID.TargetDummy)
            {
                return;
            }
            if (bloodflareMelee)
            {
                if (bloodflareMeleeHits < 15 && bloodflareFrenzyTimer <= 0 && bloodflareFrenzyCooldown <= 0)
                {
                    bloodflareMeleeHits++;
                }
                if (player.whoAmI == Main.myPlayer)
                {
                    int healAmount = (Main.rand.Next(5) + 1);
                    player.statLife += healAmount;
                    player.HealEffect(healAmount);
                }
            }
		}
        #endregion

        #region OnHitNPCWithProj
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if (proj.melee && silvaMelee && Main.rand.Next(4) == 0)
            {
                target.AddBuff(mod.BuffType("SilvaStun"), 20);
            }
            if (target.type != NPCID.TargetDummy)
            {
                if (player.whoAmI == Main.myPlayer && raiderTalisman && raiderStack < 50 && proj.thrown && crit)
                {
                    raiderStack++;
                }
                if (CalamityWorld.revenge && player.whoAmI == Main.myPlayer)
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

        #region ModifyHitNPC
        public override void ModifyHitNPC(Item item, NPC target, ref int damage, ref float knockback, ref bool crit)
        {
            if (silvaMelee && Main.rand.Next(4) == 0)
            {
                damage *= 5;
            }
            if (target.type != NPCID.TargetDummy)
            {
                if (CalamityWorld.revenge && player.whoAmI == Main.myPlayer)
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
                    astralStarRainCooldown = 120;
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
            if (auricSet && silvaThrowing && proj.thrown && crit && player.statLife > (int)((double)player.statLifeMax2 * 0.9))
            {
                damage = (int)((double)damage * 3.5);
            }
            if (auricSet && silvaMelee && proj.melee)
            {
                double multiplier = (double)player.statLife / (double)player.statLifeMax2;
                if (multiplier < 0.4)
                {
                    multiplier = 0.4;
                }
                damage = (int)(((double)damage * multiplier) * 2.5); //ranges from 2.5 times to 1 times
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
            if (tarraRanged && tarraRangedCrits < 150 && tarraRangedCritTimer <= 105 && crit && proj.ranged)
            {
                tarraRangedCrits++;
                if (tarraRangedCrits > 150) { tarraRangedCrits = 150; }
                tarraRangedCritTimer = 120;
            }
            if (tarraThrowing && tarraThrowingCritTimer <= 0 && tarraThrowingCrits < 25 && crit && proj.thrown)
            {
                tarraThrowingCrits++;
            }
            if (bloodflareThrowing && proj.thrown && crit)
            {
                if (player.whoAmI == Main.myPlayer)
                {
                    player.statLife += 1;
                    player.HealEffect(1);
                }
            }
            if (bloodflareMage && bloodflareMageCrits < 150 && bloodflareMageCritTimer <= 105 && crit && proj.magic)
            {
                bloodflareMageCrits++;
                if (bloodflareMageCrits > 150) { bloodflareMageCrits = 150; }
                bloodflareMageCritTimer = 120;
            }
            if (silvaCountdown > 0 && hasSilvaEffect && silvaRanged && proj.ranged)
            {
                damage = (int)((double)damage * 1.5);
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaThrowing && proj.thrown)
            {
                damage = (int)((double)damage * 1.2);
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaMage && proj.magic)
            {
                damage = (int)((double)damage * 1.1);
            }
            if (silvaCountdown <= 0 && hasSilvaEffect && silvaSummon && proj.minion)
            {
                damage = (int)((double)damage * 1.15);
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
        }
        #endregion

        #region ModifyHitByProj
        public override void ModifyHitByProjectile(Projectile proj, ref int damage, ref bool crit)
        {
            if (cEnergy && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                damage = (int)((double)damage * 0.75);
            }
            if (beeResist)
            {
                int type = proj.type;
                List<int> beeProjectileList = new List<int>(5)
                {
                    ProjectileID.Stinger,
                    ProjectileID.HornetStinger,
                    mod.ProjectileType("PlagueStingerGoliath"),
                    mod.ProjectileType("PlagueStingerGoliathV2"),
                    mod.ProjectileType("PlagueExplosion")
                };
                bool shouldAffect = beeProjectileList.Contains(type);
                if (shouldAffect) { damage = (int)((double)damage * 0.75); }
            }
            if (Main.hardMode && !CalamityWorld.spawnedHardBoss && proj.active && !proj.friendly && proj.hostile && damage > 0)
            {
                List<int> hardModeNerfList = new List<int>(10)
                {
                    472,
                    84,
                    128,
                    129,
                    288,
                    55,
                    264,
                    82,
                    180,
                    240
                };
                bool shouldAffect = hardModeNerfList.Contains(proj.type);
                if (shouldAffect)
                {
                    double multiplier = Main.expertMode ? 0.6 : 0.8;
                    damage = (int)((double)damage * multiplier);
                }
            }
        }
        #endregion

        #region OnHit
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            if (CalamityGlobalNPC.lordeBoss >= 0)
            {
                player.AddBuff(mod.BuffType("NOU"), 15, true);
            }
        }

        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (CalamityGlobalNPC.lordeBoss >= 0)
            {
                player.AddBuff(mod.BuffType("NOU"), 15, true);
            }
            if (proj.type == 257 && proj.hostile && Main.rand.Next(2) == 0 && !player.frozen && !gState)
            {
                player.AddBuff(mod.BuffType("GlacialState"), 120);
            }
            if (projRef && proj.active && !proj.friendly && proj.hostile && damage > 0 && Main.rand.Next(10) == 0)
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
                        if (player.FindBuffIndex(BuffID.Gills) > -1 && NPC.downedPlantBoss && liquidType == 0 && Main.rand.Next(25) == 0 && power < 120)
                        {
                            caughtType = mod.ItemType("Floodtide");
                        }
                        if (abyssPosX && liquidType == 0 && Main.rand.Next(15) == 0 && power < 120)
                        {
                            caughtType = mod.ItemType("AlluringBait");
                        }
                        if (power >= 80)
                        {
                            if (abyssPosX && Main.hardMode && liquidType == 0 && Main.rand.Next(15) == 0 && power < 140)
                            {
                                switch (Main.rand.Next(4))
                                {
                                    case 0: caughtType = mod.ItemType("IronBoots"); break; //movement acc
                                    case 1: caughtType = mod.ItemType("DepthCharm"); break; //regen acc
                                    case 2: caughtType = mod.ItemType("AnechoicPlating"); break; //defense acc
                                    case 3: caughtType = mod.ItemType("StrangeOrb"); break; //light pet
                                }
                            }
                            if (power >= 100)
                            {
                                if (abyssPosX && liquidType == 0 && Main.rand.Next(15) == 0 && power < 160)
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
            if ((abyssalDivingSuitPower || abyssalDivingSuitForce) && !abyssalDivingSuitHide)
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
            {
                NOULOL();
            }
            if (weakPetrification)
            {
                WeakPetrification();
            }
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
        #endregion

        #region HurtMethods
        public override bool PreHurt(bool pvp, bool quiet, ref int damage, ref int hitDirection, ref bool crit, ref bool customDamage, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
		{
            if (CalamityWorld.armageddon)
            {
                if (CalamityGlobalNPC.AnyBossNPCS())
                {
                    KillPlayer();
                }
            }
            if (lol || invincible)
            {
                return false;
            }
            if (godSlayerDamage && damage <= 80)
            {
                return false;
            }
            if (godSlayerReflect && Main.rand.Next(50) == 0)
            {
                return false;
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
                damage = (int)((double)damage * 0.75);
            }
            if (sirenIce)
            {
                damage = (int)((double)damage * 0.75);
            }
            if (auricSet)
			{
				damage = (int)((double)damage * 0.9);
			}
			else if (silvaSet)
			{
				damage = (int)((double)damage * 0.95);
			}
            if (CalamityWorld.revenge)
            {
                if (NPC.golemBoss > 0)
                {
                    damage = (int)((double)damage * CalamityGlobalNPC.golemDamageBonus);
                }
                else if (NPC.AnyNPCs(NPCID.MoonLordCore))
                {
                    damage = (int)((double)damage * 1.1);
                }
                else if (NPC.AnyNPCs(NPCID.CultistBoss))
                {
                    damage = (int)((double)damage * 1.2);
                }
            }
            if (CalamityGlobalNPC.DraedonMayhem == 1)
            {
                damage = (int)((double)damage * 1.2);
            }
            if (Main.pumpkinMoon && CalamityWorld.downedDoG)
			{
				damage = (int)((double)damage * 1.4);
			}
			else if (Main.snowMoon && CalamityWorld.downedDoG)
			{
				damage = (int)((double)damage * 1.3);
			}
			else if (Main.eclipse && CalamityWorld.downedYharon)
			{
				damage = (int)((double)damage * 1.6);
			}
            if (dArtifact)
            {
                damage = (int)((double)damage * 1.25);
            }
            if (CalamityWorld.bossRushActive)
            {
                switch (bossRushStage)
                {
                    case 0:
                        damage = (int)((double)damage * 3.0); //Tier 1 Queen Bee
                        break;
                    case 1:
                        damage = (int)((double)damage * 3.0); //BoC
                        break;
                    case 2:
                        damage = (int)((double)damage * 3.0); //King Slime
                        break;
                    case 3:
                        damage = (int)((double)damage * 4.0); //EoC
                        break;
                    case 4:
                        damage = (int)((double)damage * 2.0); //Prime
                        break;
                    case 5:
                        damage = (int)((double)damage * 1.75); //Golem
                        break;
                    case 6:
                        damage = (int)((double)damage * 1.25); //Guardians
                        break;
                    case 7:
                        damage = (int)((double)damage * 3.5); //EoW
                        break;
                    case 8:
                        damage = (int)((double)damage * 2.0); //Tier 2 Astrageldon
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
                        damage = (int)((double)damage * 3.5); //WoF
                        break;
                    case 13:
                        damage = (int)((double)damage * 4.0); //Hive Mind
                        break;
                    case 14:
                        damage = (int)((double)damage * 4.0); //Skeletron
                        break;
                    case 15:
                        damage = (int)((double)damage * 1.25); //Storm Weaver
                        break;
                    case 16:
                        damage = (int)((double)damage * 2.5); //Tier 3 Aquatic Scourge
                        break;
                    case 17:
                        damage = (int)((double)damage * 4.75); //Desert Scourge
                        break;
                    case 18:
                        damage = (int)((double)damage * 2.5); //Cultist
                        break;
                    case 19:
                        damage = (int)((double)damage * 4.75); //Crabulon
                        break;
                    case 20:
                        damage = (int)((double)damage * 2.5); //Plantera
                        break;
                    case 21:
                        damage = (int)((double)damage * 1.5); //Void
                        break;
                    case 22:
                        damage = (int)((double)damage * 4.75); //Perfs
                        break;
                    case 23:
                        damage = (int)((double)damage * 3.0); //Cryogen
                        break;
                    case 24:
                        damage = (int)((double)damage * 3.25); //Tier 4 Brimstone Elemental
                        break;
                    case 25:
                        damage = (int)((double)damage * 1.75); //Signus
                        break;
                    case 26:
                        damage = (int)((double)damage * 2.5); //Ravager
                        break;
                    case 27:
                        damage = (int)((double)damage * 3.25); //Fishron
                        break;
                    case 28:
                        damage = (int)((double)damage * 2.5); //Moon Lord
                        break;
                    case 29:
                        damage = (int)((double)damage * 2.5); //Astrum Deus
                        break;
                    case 30:
                        damage = (int)((double)damage * 1.75); //Polter
                        break;
                    case 31:
                        damage = (int)((double)damage * 2.5); //Plaguebringer
                        break;
                    case 32:
                        damage = (int)((double)damage * 4.0); //Tier 5 Calamitas
                        break;
                    case 33:
                        damage = (int)((double)damage * 3.5); //Levi and Siren
                        break;
                    case 34:
                        damage = (int)((double)damage * 4.75); //Slime God
                        break;
                    case 35:
                        damage = (int)((double)damage * 2.5); //Providence
                        break;
                    case 36:
                        damage = (int)((double)damage * 1.0); //SCal
                        break;
                    case 37:
                        damage = (int)((double)damage * 1.25); //Yharon
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
                if (player.ichor)
                {
                    damage = (int)((double)damage * 1.25);
                }
                else if (player.onFire2)
                {
                    damage = (int)((double)damage * 1.2);
                }
                customDamage = true;
				float damageMult = CalamityWorld.downedBossAny ? 1.05f : 0.8f;
                if (CalamityWorld.death)
                {
                    damageMult = 1.5f;
                }
				damage = (int)((double)damage * damageMult);
				double newDamage = (double)damage - (double)player.statDefense * 0.625;
				double newDamageLimit = 5.0 + (Main.hardMode ? 5.0 : 0.0) + (NPC.downedPlantBoss ? 5.0 : 0.0) + (NPC.downedMoonlord ? 15.0 : 0.0); //5, 10, 15, 30
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
            if (revivifyTimer > 0)
            {
                int healAmt = damage / 10;
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
				int healAmt = damage / 16;
				player.statLife += healAmt;
    			player.HealEffect(healAmt);
			}
            if (NPC.AnyNPCs(mod.NPCType("SupremeCalamitas")))
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
                for (int i = 0; i < 200; i++)
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
			bool hardMode = Main.hardMode;
            if (player.whoAmI == Main.myPlayer)
            {
                if (CalamityWorld.revenge)
                {
                    int stressGain = (int)((double)damage * 10);
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
					int sDamage = hardMode ? 40 : 10;
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
                            int spore1 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(Math.Sin(offsetAngle) * 5f), (float)(Math.Cos(offsetAngle) * 5f), 567, rDamage, 2f, player.whoAmI, 0f, 0f);
                            int spore2 = Projectile.NewProjectile(vector2.X, vector2.Y, (float)(-Math.Sin(offsetAngle) * 5f), (float)(-Math.Cos(offsetAngle) * 5f), 568, rDamage, 2f, player.whoAmI, 0f, 0f);
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
                        Projectile.NewProjectile(player.Center.X, player.Center.Y, 0f, 0f, mod.ProjectileType("GodSlayerBlaze"), 1200, 1f, player.whoAmI, 0f, 0f);
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
            bool specialDeath = CalamityWorld.ironHeart && CalamityGlobalNPC.AnyBossNPCS();
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
				float num8 = 0.992f;
				float num9 = Math.Max(player.accRunSpeed, player.maxRunSpeed);
				float num10 = 0.96f;
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
					num7 = 14f;
					num8 = 0.985f;
					num10 = 0.94f;
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
                    num7 = 16f; //14
                    num8 = 0.985f;
                    num10 = 0.94f;
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
                    num7 = 18f; //14
                    num8 = 0.985f;
                    num10 = 0.94f;
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
						player.velocity.X = 16.9f * (float)num16; //16.9
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
						player.velocity.X = 21.9f * (float)num23; //21.9 14*1.564
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
						player.velocity.X = 25f * (float)num23; //21.9 16*1.564
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
						player.velocity.X = 28.2f * (float)num23; //18*1.564
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
                player.AddBuff(mod.BuffType("ScarfCooldown"), ((player.chaosState && CalamityWorld.revenge) ? 1800 : 900));
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
		        StressPacket(false);
                AdrenalinePacket(false);
		    }
		}
        #endregion

        #region ChangeTime
        public void ChangeTime(bool day)
        {
            if (day)
            {
                Main.time = 0.0;
                Main.dayTime = true;
            }
            else
            {
                Main.time = 0.0;
                Main.dayTime = false;
            }
            if (Main.netMode == 2)
            {
                NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
            }
        }
        #endregion
    }
}
