using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.World.Generation;
using Terraria.Graphics.Effects;
using Terraria.GameContent.Generation;
using Terraria.IO;
using Terraria.Localization;
using Terraria.GameContent.Events;
using Terraria.ModLoader.IO;
using CalamityMod.Tiles;
using CalamityMod.NPCs;
using CalamityMod.World;
using CalamityMod.World.Planets;

namespace CalamityMod
{
	public class CalamityWorld : ModWorld
	{
		#region InstanceVars
		//private const int ExpandWorldBy = 200;

		public static int DoGSecondStageCountdown = 0;

		private const int saveVersion = 0;

		//Boss Rush
		public static bool bossRushActive = false; //Whether Boss Rush is active or not
		public static bool deactivateStupidFuckingBullshit = false; //Force Boss Rush to inactive
		public static int bossRushStage = 0; //Boss Rush Stage
		public static int bossRushSpawnCountdown = 180; //Delay before another Boss Rush boss can spawn

		//Death Mode natural boss spawns
		public static int bossSpawnCountdown = 0; //Death Mode natural boss spawn countdown
		public static int bossType = 0; //Death Mmode natural boss spawn type

		//Modes
		public static bool demonMode = false; //Spawn rate boost
		public static bool onionMode = false; //Extra accessory from Moon Lord
		public static bool revenge = false; //Revengeance Mode
		public static bool death = false; //Death Mode
		public static bool defiled = false; //Defiled Mode
		public static bool armageddon = false; //Armageddon Mode
		public static bool ironHeart = false; //Iron Heart Mode

		//Evil Islands
		public static int fehX = 0;
		public static int fehY = 0;

		//Brimstone Crag
		public static int fuhX = 0;
		public static int fuhY = 0;
		public static int calamityTiles = 0;

		//Abyss & Sulphur
		public static int numAbyssIslands = 0;
		public static int[] AbyssIslandX = new int[20];
		public static int[] AbyssIslandY = new int[20];
		public static int[] AbyssItemArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
		public static int sulphurTiles = 0;
		public static int abyssTiles = 0;
		public static bool abyssSide = false;
		public static int abyssChasmBottom = 0;

		//Astral
		public static int astralTiles = 0;
		public static bool spawnAstralMeteor = false;
		public static bool spawnAstralMeteor2 = false;
		public static bool spawnAstralMeteor3 = false;

		//Sunken Sea
		public static int sunkenSeaTiles = 0;
		public static Rectangle SunkenSeaLocation = Rectangle.Empty;

		//Ice Tomb
		public static bool genIcePyramid = true;

		//Shrines
		public static int[] SChestX = new int[10];
		public static int[] SChestY = new int[10];
		public static bool genHellHut = true;
		public static bool genSandHut = true;
		public static bool genEvilHut = true;
		public static bool genPureHut = true; //forest
		public static bool genIceHut = true;
		public static bool genMushroomHut = true;
		public static bool genGraniteHut = true;
		public static bool genMarbleHut = true;
		public static bool genSwordHut = true;
		public static bool genAbyssHut = true;

		#region Downed Bools
		public static bool downedBossAny = false; //Any boss
		public static bool downedDesertScourge = false;
		public static bool downedCrabulon = false;
		public static bool downedHiveMind = false;
		public static bool downedPerforator = false;
		public static bool downedSlimeGod = false;
		public static bool spawnedHardBoss = false; //Hardmode boss spawned
		public static bool downedCryogen = false;
		public static bool downedAquaticScourge = false;
		public static bool downedBrimstoneElemental = false;
		public static bool downedCalamitas = false;
		public static bool downedLeviathan = false;
		public static bool downedAstrageldon = false;
		public static bool downedStarGod = false;
		public static bool downedPlaguebringer = false;
		public static bool downedScavenger = false;
		public static bool downedOldDuke = false;
		public static bool downedGuardians = false;
		public static bool downedProvidence = false;
		public static bool downedSentinel1 = false;
		public static bool downedSentinel2 = false;
		public static bool downedSentinel3 = false;
		public static bool downedPolterghast = false;
		public static bool downedDoG = false;
		public static bool downedBumble = false;
		public static bool buffedEclipse = false;
		public static bool downedBuffedMothron = false;
		public static bool downedYharon = false;
		public static bool downedSCal = false;
		public static bool downedLORDE = false;
		public static bool downedCLAM = false;

		//Vanilla
		public static bool downedWhar = false; //BoC or EoW
		public static bool downedSkullHead = false; //Skeletron
		public static bool downedUgly = false; //Wall of Flesh
		public static bool downedSkeletor = false; //Skeletron Prime
		public static bool downedPlantThing = false; //Plantera
		public static bool downedGolemBaby = false; //Golem
		public static bool downedMoonDude = false; //Moon Lord
		public static bool downedBetsy = false; //Betsy
		#endregion

		#endregion

		#region Initialize
		public override void Initialize()
		{
			if (Config.ExpertPillarEnemyKillCountReduction)
			{
				NPC.LunarShieldPowerExpert = 100;
			}
			CalamityGlobalNPC.holyBoss = -1;
			CalamityGlobalNPC.doughnutBoss = -1;
			CalamityGlobalNPC.voidBoss = -1;
			CalamityGlobalNPC.energyFlame = -1;
			CalamityGlobalNPC.hiveMind = -1;
			CalamityGlobalNPC.hiveMind2 = -1;
			CalamityGlobalNPC.scavenger = -1;
			CalamityGlobalNPC.bobbitWormBottom = -1;
			CalamityGlobalNPC.DoGHead = -1;
			CalamityGlobalNPC.SCal = -1;
			CalamityGlobalNPC.ghostBoss = -1;
			CalamityGlobalNPC.laserEye = -1;
			CalamityGlobalNPC.fireEye = -1;
			CalamityGlobalNPC.lordeBoss = -1;
			CalamityGlobalNPC.brimstoneElemental = -1;
			bossRushStage = 0;
			DoGSecondStageCountdown = 0;
			bossRushActive = false;
			bossRushSpawnCountdown = 180;
			bossSpawnCountdown = 0;
			bossType = 0;
			abyssChasmBottom = 0;
			abyssSide = false;
			downedDesertScourge = false;
			downedAquaticScourge = false;
			downedHiveMind = false;
			downedPerforator = false;
			downedSlimeGod = false;
			downedCryogen = false;
			downedBrimstoneElemental = false;
			downedCalamitas = false;
			downedLeviathan = false;
			downedDoG = false;
			downedPlaguebringer = false;
			downedGuardians = false;
			downedProvidence = false;
			downedSentinel1 = false;
			downedSentinel2 = false;
			downedSentinel3 = false;
			downedYharon = false;
			buffedEclipse = false;
			downedSCal = false;
			downedCLAM = false;
			downedBumble = false;
			downedCrabulon = false;
			downedBetsy = false;
			downedScavenger = false;
			downedWhar = false;
			downedSkullHead = false;
			downedUgly = false;
			downedSkeletor = false;
			downedPlantThing = false;
			downedGolemBaby = false;
			downedMoonDude = false;
			downedBossAny = false;
			spawnedHardBoss = false;
			demonMode = false;
			onionMode = false;
			revenge = false;
			downedStarGod = false;
			downedAstrageldon = false;
			spawnAstralMeteor = false;
			spawnAstralMeteor2 = false;
			spawnAstralMeteor3 = false;
			downedPolterghast = false;
			downedLORDE = false;
			downedBuffedMothron = false;
			downedOldDuke = false;
			death = false;
			defiled = false;
			armageddon = false;
			ironHeart = false;
		}
		#endregion

		#region Save
		public override TagCompound Save()
		{
			var downed = new List<string>();
			if (downedDesertScourge) downed.Add("desertScourge");
			if (downedAquaticScourge) downed.Add("aquaticScourge");
			if (downedHiveMind) downed.Add("hiveMind");
			if (downedPerforator) downed.Add("perforator");
			if (downedSlimeGod) downed.Add("slimeGod");
			if (downedCryogen) downed.Add("cryogen");
			if (downedBrimstoneElemental) downed.Add("brimstoneElemental");
			if (downedCalamitas) downed.Add("calamitas");
			if (downedLeviathan) downed.Add("leviathan");
			if (downedDoG) downed.Add("devourerOfGods");
			if (downedPlaguebringer) downed.Add("plaguebringerGoliath");
			if (downedGuardians) downed.Add("guardians");
			if (downedProvidence) downed.Add("providence");
			if (downedSentinel1) downed.Add("ceaselessVoid");
			if (downedSentinel2) downed.Add("stormWeaver");
			if (downedSentinel3) downed.Add("signus");
			if (downedYharon) downed.Add("yharon");
			if (buffedEclipse) downed.Add("eclipse");
			if (downedSCal) downed.Add("supremeCalamitas");
			if (downedBumble) downed.Add("bumblebirb");
			if (downedCrabulon) downed.Add("crabulon");
			if (downedBetsy) downed.Add("betsy");
			if (downedScavenger) downed.Add("scavenger");
			if (downedWhar) downed.Add("boss2");
			if (downedSkullHead) downed.Add("boss3");
			if (downedUgly) downed.Add("wall");
			if (downedSkeletor) downed.Add("skeletronPrime");
			if (downedPlantThing) downed.Add("planter");
			if (downedGolemBaby) downed.Add("baby");
			if (downedMoonDude) downed.Add("moonDude");
			if (downedBossAny) downed.Add("anyBoss");
			if (demonMode) downed.Add("demonMode");
			if (onionMode) downed.Add("onionMode");
			if (revenge) downed.Add("revenge");
			if (downedStarGod) downed.Add("starGod");
			if (downedAstrageldon) downed.Add("astrageldon");
			if (spawnAstralMeteor) downed.Add("astralMeteor");
			if (spawnAstralMeteor2) downed.Add("astralMeteor2");
			if (spawnAstralMeteor3) downed.Add("astralMeteor3");
			if (spawnedHardBoss) downed.Add("hardBoss");
			if (downedPolterghast) downed.Add("polterghast");
			if (downedLORDE) downed.Add("lorde");
			if (downedBuffedMothron) downed.Add("moth");
			if (downedOldDuke) downed.Add("oldDuke");
			if (death) downed.Add("death");
			if (defiled) downed.Add("defiled");
			if (armageddon) downed.Add("armageddon");
			if (ironHeart) downed.Add("ironHeart");
			if (abyssSide) downed.Add("abyssSide");
			if (bossRushActive) downed.Add("bossRushActive");
			if (downedCLAM) downed.Add("clam");

			return new TagCompound
			{
				{
					"downed", downed
				},
				{
					"abyssChasmBottom", abyssChasmBottom
				}
			};
		}
		#endregion

		#region Load
		public override void Load(TagCompound tag)
		{
			var downed = tag.GetList<string>("downed");
			downedDesertScourge = downed.Contains("desertScourge");
			downedAquaticScourge = downed.Contains("aquaticScourge");
			downedHiveMind = downed.Contains("hiveMind");
			downedPerforator = downed.Contains("perforator");
			downedSlimeGod = downed.Contains("slimeGod");
			downedCryogen = downed.Contains("cryogen");
			downedBrimstoneElemental = downed.Contains("brimstoneElemental");
			downedCalamitas = downed.Contains("calamitas");
			downedLeviathan = downed.Contains("leviathan");
			downedDoG = downed.Contains("devourerOfGods");
			downedPlaguebringer = downed.Contains("plaguebringerGoliath");
			downedGuardians = downed.Contains("guardians");
			downedProvidence = downed.Contains("providence");
			downedSentinel1 = downed.Contains("ceaselessVoid");
			downedSentinel2 = downed.Contains("stormWeaver");
			downedSentinel3 = downed.Contains("signus");
			downedYharon = downed.Contains("yharon");
			buffedEclipse = downed.Contains("eclipse");
			downedSCal = downed.Contains("supremeCalamitas");
			downedBumble = downed.Contains("bumblebirb");
			downedCrabulon = downed.Contains("crabulon");
			downedBetsy = downed.Contains("betsy");
			downedScavenger = downed.Contains("scavenger");
			downedWhar = downed.Contains("boss2");
			downedSkullHead = downed.Contains("boss3");
			downedUgly = downed.Contains("wall");
			downedSkeletor = downed.Contains("skeletronPrime");
			downedPlantThing = downed.Contains("planter");
			downedGolemBaby = downed.Contains("baby");
			downedMoonDude = downed.Contains("moonDude");
			downedBossAny = downed.Contains("anyBoss");
			demonMode = downed.Contains("demonMode");
			onionMode = downed.Contains("onionMode");
			revenge = downed.Contains("revenge");
			downedStarGod = downed.Contains("starGod");
			downedAstrageldon = downed.Contains("astrageldon");
			spawnAstralMeteor = downed.Contains("astralMeteor");
			spawnAstralMeteor2 = downed.Contains("astralMeteor2");
			spawnAstralMeteor3 = downed.Contains("astralMeteor3");
			spawnedHardBoss = downed.Contains("hardBoss");
			downedPolterghast = downed.Contains("polterghast");
			downedLORDE = downed.Contains("lorde");
			downedBuffedMothron = downed.Contains("moth");
			downedOldDuke = downed.Contains("oldDuke");
			death = downed.Contains("death");
			defiled = downed.Contains("defiled");
			armageddon = downed.Contains("armageddon");
			ironHeart = downed.Contains("ironHeart");
			abyssSide = downed.Contains("abyssSide");
			bossRushActive = downed.Contains("bossRushActive");
			downedCLAM = downed.Contains("clam");

			abyssChasmBottom = tag.GetInt("abyssChasmBottom");
		}
		#endregion

		#region LoadLegacy
		public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
			abyssChasmBottom = reader.ReadInt32();

			if (loadVersion == 0)
			{
				BitsByte flags = reader.ReadByte();
				downedDesertScourge = flags[0];
				downedHiveMind = flags[1];
				downedPerforator = flags[2];
				downedSlimeGod = flags[3];
				downedCryogen = flags[4];
				downedBrimstoneElemental = flags[5];
				downedCalamitas = flags[6];
				downedLeviathan = flags[7];

				BitsByte flags2 = reader.ReadByte();
				downedDoG = flags2[0];
				downedPlaguebringer = flags2[1];
				downedGuardians = flags2[2];
				downedProvidence = flags2[3];
				downedSentinel1 = flags2[4];
				downedSentinel2 = flags2[5];
				downedSentinel3 = flags2[6];
				downedYharon = flags2[7];

				BitsByte flags3 = reader.ReadByte();
				downedSCal = flags3[0];
				downedBumble = flags3[1];
				downedCrabulon = flags3[2];
				downedBetsy = flags3[3];
				downedScavenger = flags3[4];
				downedWhar = flags3[5];
				downedSkullHead = flags3[6];
				downedUgly = flags3[7];

				BitsByte flags4 = reader.ReadByte();
				downedSkeletor = flags4[0];
				downedPlantThing = flags4[1];
				downedGolemBaby = flags4[2];
				downedMoonDude = flags4[3];
				downedBossAny = flags4[4];
				demonMode = flags4[5];
				onionMode = flags4[6];
				revenge = flags4[7];

				BitsByte flags5 = reader.ReadByte();
				downedStarGod = flags5[0];
				spawnAstralMeteor = flags5[1];
				spawnAstralMeteor2 = flags5[2];
				spawnAstralMeteor3 = flags5[3];
				spawnedHardBoss = flags5[4];
				downedPolterghast = flags5[5];
				death = flags5[6];
				downedLORDE = flags5[7];

				BitsByte flags6 = reader.ReadByte();
				abyssSide = flags6[0];
				downedAquaticScourge = flags6[1];
				downedAstrageldon = flags6[2];
				buffedEclipse = flags6[3];
				armageddon = flags6[4];
				defiled = flags6[5];
				downedBuffedMothron = flags6[6];
				ironHeart = flags6[7];

				BitsByte flags7 = reader.ReadByte();
				bossRushActive = flags7[0];
				downedOldDuke = flags7[1];
				downedCLAM = flags7[2];
			}
			else
			{
				ErrorLogger.Log("CalamityMod: Unknown loadVersion: " + loadVersion);
			}
		}
		#endregion

		#region NetSend
		public override void NetSend(BinaryWriter writer)
		{
			BitsByte flags = new BitsByte();
			flags[0] = downedDesertScourge;
			flags[1] = downedHiveMind;
			flags[2] = downedPerforator;
			flags[3] = downedSlimeGod;
			flags[4] = downedCryogen;
			flags[5] = downedBrimstoneElemental;
			flags[6] = downedCalamitas;
			flags[7] = downedLeviathan;

			BitsByte flags2 = new BitsByte();
			flags2[0] = downedDoG;
			flags2[1] = downedPlaguebringer;
			flags2[2] = downedGuardians;
			flags2[3] = downedProvidence;
			flags2[4] = downedSentinel1;
			flags2[5] = downedSentinel2;
			flags2[6] = downedSentinel3;
			flags2[7] = downedYharon;

			BitsByte flags3 = new BitsByte();
			flags3[0] = downedSCal;
			flags3[1] = downedBumble;
			flags3[2] = downedCrabulon;
			flags3[3] = downedBetsy;
			flags3[4] = downedScavenger;
			flags3[5] = downedWhar;
			flags3[6] = downedSkullHead;
			flags3[7] = downedUgly;

			BitsByte flags4 = new BitsByte();
			flags4[0] = downedSkeletor;
			flags4[1] = downedPlantThing;
			flags4[2] = downedGolemBaby;
			flags4[3] = downedMoonDude;
			flags4[4] = downedBossAny;
			flags4[5] = demonMode;
			flags4[6] = onionMode;
			flags4[7] = revenge;

			BitsByte flags5 = new BitsByte();
			flags5[0] = downedStarGod;
			flags5[1] = spawnAstralMeteor;
			flags5[2] = spawnAstralMeteor2;
			flags5[3] = spawnAstralMeteor3;
			flags5[4] = spawnedHardBoss;
			flags5[5] = downedPolterghast;
			flags5[6] = death;
			flags5[7] = downedLORDE;

			BitsByte flags6 = new BitsByte();
			flags6[0] = abyssSide;
			flags6[1] = downedAquaticScourge;
			flags6[2] = downedAstrageldon;
			flags6[3] = buffedEclipse;
			flags6[4] = armageddon;
			flags6[5] = defiled;
			flags6[6] = downedBuffedMothron;
			flags6[7] = ironHeart;

			BitsByte flags7 = new BitsByte();
			flags7[0] = bossRushActive;
			flags7[1] = downedOldDuke;
			flags7[2] = downedCLAM;

			writer.Write(flags);
			writer.Write(flags2);
			writer.Write(flags3);
			writer.Write(flags4);
			writer.Write(flags5);
			writer.Write(flags6);
			writer.Write(flags7);
			writer.Write(abyssChasmBottom);
		}
		#endregion

		#region NetReceive
		public override void NetReceive(BinaryReader reader)
		{
			BitsByte flags = reader.ReadByte();
			downedDesertScourge = flags[0];
			downedHiveMind = flags[1];
			downedPerforator = flags[2];
			downedSlimeGod = flags[3];
			downedCryogen = flags[4];
			downedBrimstoneElemental = flags[5];
			downedCalamitas = flags[6];
			downedLeviathan = flags[7];

			BitsByte flags2 = reader.ReadByte();
			downedDoG = flags2[0];
			downedPlaguebringer = flags2[1];
			downedGuardians = flags2[2];
			downedProvidence = flags2[3];
			downedSentinel1 = flags2[4];
			downedSentinel2 = flags2[5];
			downedSentinel3 = flags2[6];
			downedYharon = flags2[7];

			BitsByte flags3 = reader.ReadByte();
			downedSCal = flags3[0];
			downedBumble = flags3[1];
			downedCrabulon = flags3[2];
			downedBetsy = flags3[3];
			downedScavenger = flags3[4];
			downedWhar = flags3[5];
			downedSkullHead = flags3[6];
			downedUgly = flags3[7];

			BitsByte flags4 = reader.ReadByte();
			downedSkeletor = flags4[0];
			downedPlantThing = flags4[1];
			downedGolemBaby = flags4[2];
			downedMoonDude = flags4[3];
			downedBossAny = flags4[4];
			demonMode = flags4[5];
			onionMode = flags4[6];
			revenge = flags4[7];

			BitsByte flags5 = reader.ReadByte();
			downedStarGod = flags5[0];
			spawnAstralMeteor = flags5[1];
			spawnAstralMeteor2 = flags5[2];
			spawnAstralMeteor3 = flags5[3];
			spawnedHardBoss = flags5[4];
			downedPolterghast = flags5[5];
			death = flags5[6];
			downedLORDE = flags5[7];

			BitsByte flags6 = reader.ReadByte();
			abyssSide = flags6[0];
			downedAquaticScourge = flags6[1];
			downedAstrageldon = flags6[2];
			buffedEclipse = flags6[3];
			armageddon = flags6[4];
			defiled = flags6[5];
			downedBuffedMothron = flags6[6];
			ironHeart = flags6[7];

			BitsByte flags7 = reader.ReadByte();
			bossRushActive = flags7[0];
			downedOldDuke = flags7[1];
			downedCLAM = flags7[2];

			abyssChasmBottom = reader.ReadInt32();
		}
		#endregion

		#region Tiles
		public override void ResetNearbyTileEffects()
		{
			calamityTiles = 0;
			astralTiles = 0;
			sunkenSeaTiles = 0;
			sulphurTiles = 0;
			abyssTiles = 0;
		}

		public override void TileCountsAvailable(int[] tileCounts)
		{
			calamityTiles = tileCounts[mod.TileType("CharredOre")] + tileCounts[mod.TileType("BrimstoneSlag")];
			sunkenSeaTiles = tileCounts[mod.TileType("EutrophicSand")] + tileCounts[mod.TileType("Navystone")] + tileCounts[mod.TileType("SeaPrism")];
			abyssTiles = tileCounts[mod.TileType("AbyssGravel")];
			sulphurTiles = tileCounts[mod.TileType("SulphurousSand")];

			#region Astral Stuff
			int astralDesertTiles = tileCounts[mod.TileType("AstralSand")] + tileCounts[mod.TileType("AstralSandstone")] + tileCounts[mod.TileType("HardenedAstralSand")];
			int astralSnowTiles = tileCounts[mod.TileType("AstralIce")];

			Main.sandTiles += astralDesertTiles;
			Main.snowTiles += astralSnowTiles;

			astralTiles = astralDesertTiles + astralSnowTiles + tileCounts[mod.TileType("AstralDirt")] + tileCounts[mod.TileType("AstralStone")] + tileCounts[mod.TileType("AstralGrass")] + tileCounts[mod.TileType("AstralOre")];
			#endregion
		}
		#endregion

		#region PreWorldGen
		public override void PreWorldGen()
		{
			numAbyssIslands = 0;
			genIcePyramid = true;
			genHellHut = true;
			genSandHut = true;
			genEvilHut = true;
			genPureHut = true;
			genIceHut = true;
			genMushroomHut = true;
			genGraniteHut = true;
			genMarbleHut = true;
			genSwordHut = true;
			genAbyssHut = true;
		}
		#endregion

		#region ModifyWorldGenTasks
		public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
			int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			if (ShiniesIndex != -1)
			{
				#region IceTomb
				tasks.Insert(ShiniesIndex + 1, new PassLegacy("IceTomb", delegate (GenerationProgress progress)
				{
					progress.Message = "Ice Tomb";
					int x = Main.maxTilesX;
					int y = Main.maxTilesY;
					int genLimit = x / 2;
					int generateBack = genLimit - 80; //Small = 2020
					int generateForward = genLimit + 80; //Small = 2180

					for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++)
					{
						int tilesX = WorldGen.genRand.Next(0, generateBack);
						int tilesX2 = WorldGen.genRand.Next(generateForward, x);
						int tilesY2 = WorldGen.genRand.Next((int)(y * .4f), (int)(y * .5f));
						if ((Main.tile[tilesX, tilesY2].type == TileID.SnowBlock || Main.tile[tilesX, tilesY2].type == TileID.IceBlock) && genIcePyramid)
						{
							genIcePyramid = false;
							WorldGenerationMethods.IcePyramid(tilesX, tilesY2);
						}
						if ((Main.tile[tilesX2, tilesY2].type == TileID.SnowBlock || Main.tile[tilesX2, tilesY2].type == TileID.IceBlock) && genIcePyramid)
						{
							genIcePyramid = false;
							WorldGenerationMethods.IcePyramid(tilesX2, tilesY2);
						}
					}
				}));
				#endregion

				#region EvilIsland
				tasks.Insert(ShiniesIndex + 2, new PassLegacy("EvilIsland", delegate (GenerationProgress progress)
				{
					progress.Message = "Evil Island";
					int x = Main.maxTilesX;
					int xIslandGen = WorldGen.crimson ?
						WorldGen.genRand.Next((int)((double)x * 0.15), (int)((double)x * 0.2)) :
						WorldGen.genRand.Next((int)((double)x * 0.8), (int)((double)x * 0.85));
					int yIslandGen = WorldGen.genRand.Next(90, 151);
					yIslandGen = Math.Min(yIslandGen, (int)WorldGen.worldSurfaceLow - 50);
					int tileXLookup = xIslandGen;
					if (WorldGen.crimson)
					{
						while (Main.tile[tileXLookup, yIslandGen].active())
							tileXLookup++;
					}
					else
					{
						while (Main.tile[tileXLookup, yIslandGen].active())
							tileXLookup--;
					}
					xIslandGen = tileXLookup;
					fehX = xIslandGen;
					fehY = yIslandGen;
					WorldGenerationMethods.EvilIsland(xIslandGen, yIslandGen);
					WorldGenerationMethods.EvilIslandHouse(fehX, fehY);
				}));
				#endregion
			}

			int DungeonChestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
			if (DungeonChestIndex != -1)
			{
				tasks.Insert(DungeonChestIndex + 1, new PassLegacy("Calamity Mod: Biome Chests", WorldGenerationMethods.GenerateBiomeChests));
			}

			int WaterFromSandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Water From Sand"));
			if (WaterFromSandIndex != -1)
			{
				tasks.Insert(WaterFromSandIndex + 1, new PassLegacy("SunkenSea", delegate (GenerationProgress progress)
				{
					progress.Message = "Making the world more wet";
					SunkenSea.Place(new Point(WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Bottom));
				}));
			}

			int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
			if (SulphurIndex != -1)
			{
				#region Sulphur
				tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur", delegate (GenerationProgress progress)
				{
					progress.Message = "Sulphur Beach";
					int x = Main.maxTilesX;
					int y = Main.maxTilesY;
					int genLimit = x / 2;

					if (WorldGen.dungeonX < genLimit)
						abyssSide = true;

					int abyssChasmX = (abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135)); //2100 - 1965 = 135 : 2100 + 1965 = 4065

					if (abyssSide)
					{
						for (int abyssIndexSand = 0; abyssIndexSand < abyssChasmX + 240; abyssIndexSand++)
						{
							for (int abyssIndexSand2 = 0; abyssIndexSand2 < y - 200; abyssIndexSand2++)
							{
								Tile tile = Framing.GetTileSafely(abyssIndexSand, abyssIndexSand2);
								if (abyssIndexSand > abyssChasmX + 225)
								{
									if (tile.active() &&
										tile.type == TileID.Sand &&
										WorldGen.genRand.Next(4) == 0)
									{
										tile.type = (ushort)mod.TileType("SulphurousSand");
									}
									else if (tile.active() &&
											 tile.type == TileID.PalmTree)
									{
										tile.active(false);
									}
								}
								else if (abyssIndexSand > abyssChasmX + 210)
								{
									if (tile.active() &&
										tile.type == TileID.Sand &&
										WorldGen.genRand.Next(2) == 0)
									{
										tile.type = (ushort)mod.TileType("SulphurousSand");
									}
									else if (tile.active() &&
											 tile.type == TileID.PalmTree)
									{
										tile.active(false);
									}
								}
								else
								{
									if (tile.active() &&
										tile.type == TileID.Sand)
									{
										tile.type = (ushort)mod.TileType("SulphurousSand");
									}
									else if (tile.active() &&
											 tile.type == TileID.PalmTree)
									{
										tile.active(false);
									}
								}
							}
						}
					}
					else
					{
						for (int abyssIndexSand = abyssChasmX - 240; abyssIndexSand < x; abyssIndexSand++)
						{
							for (int abyssIndexSand2 = 0; abyssIndexSand2 < y - 200; abyssIndexSand2++)
							{
								Tile tile = Framing.GetTileSafely(abyssIndexSand, abyssIndexSand2);
								if (abyssIndexSand < abyssChasmX - 225)
								{
									if (tile.active() &&
										tile.type == TileID.Sand &&
										WorldGen.genRand.Next(4) == 0)
									{
										tile.type = (ushort)mod.TileType("SulphurousSand");
									}
									else if (tile.active() &&
											 tile.type == TileID.PalmTree)
									{
										tile.active(false);
									}
								}
								else if (abyssIndexSand < abyssChasmX - 210)
								{
									if (tile.active() &&
										tile.type == TileID.Sand &&
										WorldGen.genRand.Next(2) == 0)
									{
										tile.type = (ushort)mod.TileType("SulphurousSand");
									}
									else if (tile.active() &&
											 tile.type == TileID.PalmTree)
									{
										tile.active(false);
									}
								}
								else
								{
									if (tile.active() &&
										tile.type == TileID.Sand)
									{
										tile.type = (ushort)mod.TileType("SulphurousSand");
									}
									else if (tile.active() &&
											 tile.type == TileID.PalmTree)
									{
										tile.active(false);
									}
								}
							}
						}
					}
				}));
				#endregion
			}

			int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (FinalIndex != -1)
			{
				#region BrimstoneCrag
				tasks.Insert(FinalIndex + 1, new PassLegacy("BrimstoneCrag", delegate (GenerationProgress progress)
				{
					progress.Message = "Brimstone Crag";

					int x = Main.maxTilesX;

					int xUnderworldGen = WorldGen.genRand.Next((int)((double)x * 0.1), (int)((double)x * 0.15));
					int yUnderworldGen = Main.maxTilesY - 100;

					fuhX = xUnderworldGen;
					fuhY = yUnderworldGen;

					WorldGenerationMethods.UnderworldIsland(xUnderworldGen, yUnderworldGen, 180, 201, 120, 136);
					WorldGenerationMethods.UnderworldIsland(xUnderworldGen - 50, yUnderworldGen - 30, 100, 111, 60, 71);
					WorldGenerationMethods.UnderworldIsland(xUnderworldGen + 50, yUnderworldGen - 30, 100, 111, 60, 71);

					WorldGenerationMethods.ChasmGenerator(fuhX - 110, fuhY - 10, WorldGen.genRand.Next(150) + 150);
					WorldGenerationMethods.ChasmGenerator(fuhX + 110, fuhY - 10, WorldGen.genRand.Next(150) + 150);

					WorldGenerationMethods.UnderworldIsland(xUnderworldGen - 150, yUnderworldGen - 30, 60, 66, 35, 41);
					WorldGenerationMethods.UnderworldIsland(xUnderworldGen + 150, yUnderworldGen - 30, 60, 66, 35, 41);
					WorldGenerationMethods.UnderworldIsland(xUnderworldGen - 180, yUnderworldGen - 20, 60, 66, 35, 41);
					WorldGenerationMethods.UnderworldIsland(xUnderworldGen + 180, yUnderworldGen - 20, 60, 66, 35, 41);

					WorldGenerationMethods.UnderworldIslandHouse(fuhX, fuhY + 30, 1323);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX - 22, fuhY + 15, 1322);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX + 22, fuhY + 15, 535);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX - 50, fuhY - 30, 112);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX + 50, fuhY - 30, 906);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX - 150, fuhY - 30, 218);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX + 150, fuhY - 30, 3019);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX - 180, fuhY - 20, 274);
					WorldGenerationMethods.UnderworldIslandHouse(fuhX + 180, fuhY - 20, 220);
				}));
				#endregion

				#region SpecialShrines
				tasks.Insert(FinalIndex + 2, new PassLegacy("SpecialShrines", delegate (GenerationProgress progress)
				{
					progress.Message = "Special Shrines";
					int x = Main.maxTilesX;
					int y = Main.maxTilesY;
					int genLimit = x / 2;
					int generateBack = genLimit - 80; //Small = 2020
					int generateForward = genLimit + 80; //Small = 2180

					for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++)
					{
						int tilesX = WorldGen.genRand.Next(0, generateBack);
						int tilesX2 = WorldGen.genRand.Next(generateForward, x);
						int tilesX3 = WorldGen.genRand.Next((int)((double)x * 0.3), generateBack);
						int tilesX4 = WorldGen.genRand.Next(generateForward, (int)((double)x * 0.6));
						int tilesX5 = WorldGen.genRand.Next((int)((double)x * 0.97), (int)((double)x * 0.98));
						int tilesY3 = WorldGen.genRand.Next((int)(y * .3f), (int)(y * .35f));
						int tilesY4 = WorldGen.genRand.Next((int)(y * .35f), (int)(y * .5f));
						int tilesY5 = WorldGen.genRand.Next((int)(y * .55f), (int)(y * .8f));
						int tilesY6 = y - 60;
						if ((Main.tile[tilesX3, tilesY3].type == TileID.Dirt || Main.tile[tilesX3, tilesY3].type == TileID.Stone) && genPureHut)
						{
							genPureHut = false;
							WorldGenerationMethods.SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX3, tilesY3); //red brick, dirt, red brick wall
						}
						if ((Main.tile[tilesX4, tilesY3].type == TileID.Dirt || Main.tile[tilesX4, tilesY3].type == TileID.Stone) && genPureHut)
						{
							genPureHut = false;
							WorldGenerationMethods.SpecialHut(TileID.RedBrick, TileID.Dirt, WallID.RedBrick, 0, tilesX4, tilesY3);
						}
						if (Main.tile[tilesX, tilesY3].type == (WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone) && genEvilHut)
						{
							genEvilHut = false;
							WorldGenerationMethods.SpecialHut(WorldGen.crimson ? TileID.CrimtaneBrick : TileID.DemoniteBrick,
								WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone,
								WorldGen.crimson ? WallID.CrimtaneBrick : WallID.DemoniteBrick, 1, tilesX, tilesY3); //crimstone brick, crimstone, crimstone brick wall
						}
						if (Main.tile[tilesX2, tilesY3].type == (WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone) && genEvilHut)
						{
							genEvilHut = false;
							WorldGenerationMethods.SpecialHut(WorldGen.crimson ? TileID.CrimtaneBrick : TileID.DemoniteBrick,
								WorldGen.crimson ? TileID.Crimstone : TileID.Ebonstone,
								WorldGen.crimson ? WallID.CrimtaneBrick : WallID.DemoniteBrick, 1, tilesX2, tilesY3);
						}
						if (Main.tile[tilesX3, tilesY5].type == TileID.Stone && genHellHut)
						{
							genHellHut = false;
							WorldGenerationMethods.SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, 2, tilesX3, tilesY5); //obsidian brick, obsidian, obsidian brick wall
						}
						if (Main.tile[tilesX4, tilesY5].type == TileID.Stone && genHellHut)
						{
							genHellHut = false;
							WorldGenerationMethods.SpecialHut(TileID.ObsidianBrick, TileID.Obsidian, WallID.ObsidianBrick, 2, tilesX4, tilesY5);
						}
						if (Main.tile[tilesX, tilesY4].type == TileID.IceBlock && genIceHut)
						{
							genIceHut = false;
							WorldGenerationMethods.SpecialHut(TileID.IceBrick, TileID.IceBlock, WallID.IceBrick, 3, tilesX, tilesY4);
						}
						if (Main.tile[tilesX2, tilesY4].type == TileID.IceBlock && genIceHut)
						{
							genIceHut = false;
							WorldGenerationMethods.SpecialHut(TileID.IceBrick, TileID.IceBlock, WallID.IceBrick, 3, tilesX2, tilesY4);
						}
						if (Main.tile[tilesX, tilesY4].type == TileID.DesertFossil && genSandHut)
						{
							genSandHut = false;
							WorldGenerationMethods.SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, 4, tilesX, tilesY4);
						}
						if (Main.tile[tilesX2, tilesY4].type == TileID.DesertFossil && genSandHut)
						{
							genSandHut = false;
							WorldGenerationMethods.SpecialHut(TileID.DesertFossil, TileID.Sandstone, WallID.DesertFossil, 4, tilesX2, tilesY4);
						}
						if (Main.tile[tilesX3, tilesY4].type == TileID.MushroomGrass && genMushroomHut)
						{
							genMushroomHut = false;
							WorldGenerationMethods.SpecialHut(TileID.MushroomBlock, TileID.Mud, WallID.MushroomUnsafe, 5, tilesX3, tilesY4);
						}
						if (Main.tile[tilesX4, tilesY4].type == TileID.MushroomGrass && genMushroomHut)
						{
							genMushroomHut = false;
							WorldGenerationMethods.SpecialHut(TileID.MushroomBlock, TileID.Mud, WallID.MushroomUnsafe, 5, tilesX4, tilesY4);
						}
						if (Main.tile[tilesX, tilesY4].type == TileID.Granite && genGraniteHut)
						{
							genGraniteHut = false;
							WorldGenerationMethods.SpecialHut(TileID.GraniteBlock, TileID.Granite, WallID.GraniteUnsafe, 6, tilesX, tilesY4);
						}
						if (Main.tile[tilesX2, tilesY4].type == TileID.Granite && genGraniteHut)
						{
							genGraniteHut = false;
							WorldGenerationMethods.SpecialHut(TileID.GraniteBlock, TileID.Granite, WallID.GraniteUnsafe, 6, tilesX2, tilesY4);
						}
						if (Main.tile[tilesX, tilesY4].type == TileID.Marble && genMarbleHut)
						{
							genMarbleHut = false;
							WorldGenerationMethods.SpecialHut(TileID.MarbleBlock, TileID.Marble, WallID.MarbleUnsafe, 7, tilesX, tilesY4);
						}
						if (Main.tile[tilesX2, tilesY4].type == TileID.Marble && genMarbleHut)
						{
							genMarbleHut = false;
							WorldGenerationMethods.SpecialHut(TileID.MarbleBlock, TileID.Marble, WallID.MarbleUnsafe, 7, tilesX2, tilesY4);
						}
						if (genSwordHut)
						{
							genSwordHut = false;
							WorldGenerationMethods.SpecialHut(TileID.HellstoneBrick, TileID.Hellstone, WallID.HellstoneBrick, 8, tilesX5, tilesY6);
						}
					}
				}));
				#endregion

				#region Abyss
				tasks.Insert(FinalIndex + 3, new PassLegacy("Abyss", delegate (GenerationProgress progress)
				{
					progress.Message = "The Abyss";
					int x = Main.maxTilesX;
					int y = Main.maxTilesY;
					int genLimit = x / 2;
					int rockLayer = (int)Main.rockLayer;

					int abyssChasmY = y - 250; //Underworld = y - 200
					abyssChasmBottom = abyssChasmY - 100; //850 small 1450 medium 2050 large
					int abyssChasmX = (abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135)); //2100 - 1965 = 135 : 2100 + 1965 = 4065
					bool tenebrisSide = true;
					if (abyssChasmX < genLimit)
					{
						tenebrisSide = false;
					}

					if (abyssSide)
					{
						for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
						{
							for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
							{
								Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);
								bool canConvert = tile.active() &&
									tile.type < TileID.Count &&
									tile.type != TileID.DyePlants &&
									tile.type != TileID.Trees &&
									tile.type != TileID.PalmTree &&
									tile.type != TileID.Sand &&
									tile.type != TileID.Containers &&
									tile.type != TileID.Coral &&
									tile.type != TileID.BeachPiles;
								if (abyssIndex > abyssChasmX + 75)
								{
									if (WorldGen.genRand.Next(4) == 0)
									{
										if (canConvert)
										{
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
										else if (!tile.active() &&
												  abyssIndex2 > rockLayer)
										{
											tile.active(true);
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
									}
								}
								else if (abyssIndex > abyssChasmX + 70)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										if (canConvert)
										{
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
										else if (!tile.active() &&
												  abyssIndex2 > rockLayer)
										{
											tile.active(true);
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
									}
								}
								else
								{
									if (canConvert)
									{
										if (abyssIndex2 > (rockLayer + y * 0.262))
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else
										{
											tile.type = (ushort)mod.TileType("AbyssGravel");
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
										}
									}
									else if (!tile.active() &&
											  abyssIndex2 > rockLayer)
									{
										tile.active(true);
										if (abyssIndex2 > (rockLayer + y * 0.262))
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else
										{
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
									}
								}
							}
						}
					}
					else
					{
						for (int abyssIndex = abyssChasmX - 80; abyssIndex < x; abyssIndex++) //3965
						{
							for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
							{
								Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);
								bool canConvert = tile.active() &&
									tile.type < TileID.Count &&
									tile.type != TileID.DyePlants &&
									tile.type != TileID.Trees &&
									tile.type != TileID.PalmTree &&
									tile.type != TileID.Sand &&
									tile.type != TileID.Containers &&
									tile.type != TileID.Coral &&
									tile.type != TileID.BeachPiles;
								if (abyssIndex < abyssChasmX - 75)
								{
									if (WorldGen.genRand.Next(4) == 0)
									{
										if (canConvert)
										{
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
										else if (!tile.active() &&
												  abyssIndex2 > rockLayer)
										{
											tile.active(true);
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
									}
								}
								else if (abyssIndex < abyssChasmX - 70)
								{
									if (WorldGen.genRand.Next(2) == 0)
									{
										if (canConvert)
										{
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
										else if (!tile.active() &&
												  abyssIndex2 > rockLayer)
										{
											tile.active(true);
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
									}
								}
								else
								{
									if (canConvert)
									{
										if (abyssIndex2 > (rockLayer + y * 0.262))
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else
										{
											tile.type = (ushort)mod.TileType("AbyssGravel");
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
										}
									}
									else if (!tile.active() &&
											  abyssIndex2 > rockLayer)
									{
										tile.active(true);
										if (abyssIndex2 > (rockLayer + y * 0.262))
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
										{
											tile.type = (ushort)mod.TileType("Voidstone");
											tile.wall = (ushort)mod.WallType("VoidstoneWallUnsafe");
										}
										else
										{
											tile.wall = (ushort)mod.WallType("AbyssGravelWall");
											tile.type = (ushort)mod.TileType("AbyssGravel");
										}
									}
								}
							}
						}
					}

					WorldGenerationMethods.ChasmGenerator(abyssChasmX, (int)WorldGen.worldSurfaceLow, abyssChasmBottom, true);

					int maxAbyssIslands = 11; //Small World
					if (y > 2100)
						maxAbyssIslands = 20; //Large World
					else if (y > 1500)
						maxAbyssIslands = 16; //Medium World

					if (genAbyssHut) //sometimes it works, sometimes it doesn't, wtf
					{
						genAbyssHut = false;
						WorldGenerationMethods.SpecialHut((ushort)mod.TileType("SmoothVoidstone"), (ushort)mod.TileType("Voidstone"),
							(ushort)mod.WallType("VoidstoneWallUnsafe"), 9, abyssChasmX, abyssChasmBottom);
					}

					int islandLocationOffset = 30;
					int islandLocationY = rockLayer;
					for (int islands = 0; islands < maxAbyssIslands; islands++)
					{
						int islandLocationX = abyssChasmX;
						int randomIsland = WorldGen.genRand.Next(5); //0 1 2 3 4
						bool hasVoidstone = islandLocationY > (rockLayer + y * 0.143);
						AbyssIslandY[numAbyssIslands] = islandLocationY;
						switch (randomIsland)
						{
							case 0:
								WorldGenerationMethods.AbyssIsland(islandLocationX, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX + 40, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX - 40, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
								break;
							case 1:
								WorldGenerationMethods.AbyssIsland(islandLocationX + 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX - 40, islandLocationY + 10, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
								islandLocationX += 30;
								break;
							case 2:
								WorldGenerationMethods.AbyssIsland(islandLocationX - 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX + 30, islandLocationY + 10, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
								islandLocationX -= 30;
								break;
							case 3:
								WorldGenerationMethods.AbyssIsland(islandLocationX + 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX - 5, islandLocationY + 5, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX - 35, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
								islandLocationX += 25;
								break;
							case 4:
								WorldGenerationMethods.AbyssIsland(islandLocationX - 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX + 5, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
								WorldGenerationMethods.AbyssIsland(islandLocationX + 35, islandLocationY + 5, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
								islandLocationX -= 25;
								break;
						}
						AbyssIslandX[numAbyssIslands] = islandLocationX;
						numAbyssIslands++;
						islandLocationY += islandLocationOffset;
					}

					AbyssItemArray = WorldGenerationMethods.ShuffleArray(AbyssItemArray);
					for (int abyssHouse = 0; abyssHouse < numAbyssIslands; abyssHouse++) //11 15 19
					{
						if (abyssHouse != 20)
							WorldGenerationMethods.AbyssIslandHouse(AbyssIslandX[abyssHouse],
								AbyssIslandY[abyssHouse],
								AbyssItemArray[(abyssHouse > 9 ? (abyssHouse - 10) : abyssHouse)], //10 choices 0 to 9
								AbyssIslandY[abyssHouse] > (rockLayer + y * 0.143));
					}

					if (abyssSide)
					{
						for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
						{
							for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
							{
								if (!Main.tile[abyssIndex, abyssIndex2].active())
								{
									if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
										abyssIndex2 > rockLayer)
									{
										if (WorldGen.genRand.Next(5) == 0)
										{
											int style = WorldGen.genRand.Next(13, 16);
											WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
											WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
										}
									}
									else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
											 abyssIndex2 < (int)Main.worldSurface)
									{
										if (WorldGen.genRand.Next(3) == 0)
										{
											int style = WorldGen.genRand.Next(25, 28);
											WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
											WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
										}
									}
								}
							}
						}
					}
					else
					{
						for (int abyssIndex = abyssChasmX - 80; abyssIndex < x; abyssIndex++) //3965
						{
							for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
							{
								if (!Main.tile[abyssIndex, abyssIndex2].active())
								{
									if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
										abyssIndex2 > rockLayer)
									{
										if (WorldGen.genRand.Next(5) == 0)
										{
											int style = WorldGen.genRand.Next(13, 16);
											WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
											WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
										}
									}
									else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
											 abyssIndex2 < (int)Main.worldSurface)
									{
										if (WorldGen.genRand.Next(3) == 0)
										{
											int style = WorldGen.genRand.Next(25, 28);
											WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
											WorldGenerationMethods.SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
										}
									}
								}
							}
						}
					}
				}));
				#endregion
			}
			
			tasks.Add(new PassLegacy("Planetoid Test", WorldGenerationMethods.Planetoids));

		}
		#endregion

		#region PostUpdate
		public override void PostUpdate()
		{
			SunkenSeaLocation = new Rectangle(WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Bottom,
						WorldGen.UndergroundDesertLocation.Width, WorldGen.UndergroundDesertLocation.Height / 2);
			int closestPlayer = (int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0);
			#region BossRush
			if (!deactivateStupidFuckingBullshit)
			{
				deactivateStupidFuckingBullshit = true;
				bossRushActive = false;
				if (Main.netMode == 2)
				{
					NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
				}
			}
			if (bossRushActive)
			{
				if (NPC.MoonLordCountdown > 0)
				{
					NPC.MoonLordCountdown = 0;
				}
				if (!CalamityPlayer.areThereAnyDamnBosses)
				{
					if (bossRushSpawnCountdown > 0)
					{
						bossRushSpawnCountdown--;
						if (bossRushSpawnCountdown == 180 && bossRushStage == 26)
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
						if (bossRushSpawnCountdown == 210 && bossRushStage == 33)
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
					if (bossRushSpawnCountdown <= 0)
					{
						bossRushSpawnCountdown = 60;
						if (bossRushStage > 25)
						{
							bossRushSpawnCountdown += 120; //3 seconds
						}
						if (bossRushStage > 32)
						{
							bossRushSpawnCountdown += 180; //6 seconds
						}
						switch (bossRushStage)
						{
							case 9:
								bossRushSpawnCountdown = 240;
								break;
							case 18:
								bossRushSpawnCountdown = 300;
								break;
							case 25:
								bossRushSpawnCountdown = 360;
								break;
							case 32:
								bossRushSpawnCountdown = 420;
								break;
							default:
								break;
						}
						if (bossRushStage == 13)
						{
							for (int playerIndex = 0; playerIndex < 255; playerIndex++)
							{
								if (Main.player[playerIndex].active)
								{
									Player player = Main.player[playerIndex];
									player.Spawn();
								}
							}
						}
						else if (bossRushStage == 36)
						{
							for (int playerIndex = 0; playerIndex < 255; playerIndex++)
							{
								if (Main.player[playerIndex].active)
								{
									Player player = Main.player[playerIndex];
									if (player.FindBuffIndex(mod.BuffType("ExtremeGravity")) > -1)
									{
										player.ClearBuff(mod.BuffType("ExtremeGravity"));
									}
								}
							}
						}
						if (Main.netMode != 1)
						{
							Main.PlaySound(SoundID.Roar, Main.player[closestPlayer].position, 0);
							switch (bossRushStage)
							{
								case 0:
									NPC.SpawnOnPlayer(closestPlayer, NPCID.QueenBee);
									break;
								case 1:
									NPC.SpawnOnPlayer(closestPlayer, NPCID.BrainofCthulhu);
									break;
								case 2:
									NPC.SpawnOnPlayer(closestPlayer, NPCID.KingSlime);
									break;
								case 3:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, NPCID.EyeofCthulhu);
									break;
								case 4:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, NPCID.SkeletronPrime);
									break;
								case 5:
									ChangeTime(true);
									NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)(Main.rand.Next(-100, 101))),
										(int)(Main.player[closestPlayer].position.Y - 400f),
										NPCID.Golem, 0, 0f, 0f, 0f, 0f, 255);
									break;
								case 6:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("ProfanedGuardianBoss"));
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("ProfanedGuardianBoss2"));
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("ProfanedGuardianBoss3"));
									break;
								case 7:
									NPC.SpawnOnPlayer(closestPlayer, NPCID.EaterofWorldsHead);
									break;
								case 8:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Astrageldon"));
									break;
								case 9:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, NPCID.TheDestroyer);
									break;
								case 10:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, NPCID.Spazmatism);
									NPC.SpawnOnPlayer(closestPlayer, NPCID.Retinazer);
									break;
								case 11:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Bumblefuck"));
									break;
								case 12:
									NPC.SpawnWOF(Main.player[closestPlayer].position);
									break;
								case 13:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("HiveMind"));
									break;
								case 14:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, NPCID.SkeletronHead);
									break;
								case 15:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("StormWeaverHead"));
									break;
								case 16:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("AquaticScourgeHead"));
									break;
								case 17:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DesertScourgeHead"));
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DesertScourgeHeadSmall"));
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DesertScourgeHeadSmall"));
									break;
								case 18:
									int num1302 = NPC.NewNPC((int)Main.player[closestPlayer].Center.X, (int)Main.player[closestPlayer].Center.Y - 400, NPCID.CultistBoss, 0, 0f, 0f, 0f, 0f, 255);
									Main.npc[num1302].direction = (Main.npc[num1302].spriteDirection = Math.Sign(Main.player[closestPlayer].Center.X - (float)Main.player[closestPlayer].Center.X - 90f));
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
									NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)(Main.rand.Next(-100, 101))), (int)(Main.player[closestPlayer].position.Y - 400f), mod.NPCType("CrabulonIdle"), 0, 0f, 0f, 0f, 0f, 255);
									break;
								case 20:
									NPC.SpawnOnPlayer(closestPlayer, NPCID.Plantera);
									break;
								case 21:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("CeaselessVoid"));
									break;
								case 22:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("PerforatorHive"));
									break;
								case 23:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Cryogen"));
									break;
								case 24:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("BrimstoneElemental"));
									break;
								case 25:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("CosmicWraith"));
									break;
								case 26:
									NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)(Main.rand.Next(-100, 101))), (int)(Main.player[closestPlayer].position.Y - 400f), mod.NPCType("ScavengerBody"), 0, 0f, 0f, 0f, 0f, 255);
									break;
								case 27:
									NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)(Main.rand.Next(-100, 101))), (int)(Main.player[closestPlayer].position.Y - 400f), NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, 255);
									break;
								case 28:
									NPC.SpawnOnPlayer(closestPlayer, NPCID.MoonLordCore);
									break;
								case 29:
									ChangeTime(false);
									for (int x = 0; x < 10; x++)
									{
										NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("AstrumDeusHead"));
									}
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("AstrumDeusHeadSpectral"));
									break;
								case 30:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Polterghast"));
									break;
								case 31:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("PlaguebringerGoliath"));
									break;
								case 32:
									ChangeTime(false);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Calamitas"));
									break;
								case 33:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Siren"));
									break;
								case 34:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("SlimeGod"));
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("SlimeGodRun"));
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("SlimeGodCore"));
									break;
								case 35:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Providence"));
									break;
								case 36:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("SupremeCalamitas"));
									break;
								case 37:
									ChangeTime(true);
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("Yharon"));
									break;
								case 38:
									NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DevourerofGodsHeadS"));
									break;
							}
						}
					}
				}
			}
			else
			{
				bossRushSpawnCountdown = 180;
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
			#region SpawnDoG
			if (DoGSecondStageCountdown > 0) //works
			{
				DoGSecondStageCountdown--;
				if (Main.netMode == 2)
				{
					var netMessage = mod.GetPacket();
					netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
					netMessage.Write(DoGSecondStageCountdown);
					netMessage.Send();
				}
				if (Main.netMode != 1)
				{
					if (DoGSecondStageCountdown == 21540)
					{
						NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("CeaselessVoid"));
					}
					if (DoGSecondStageCountdown == 14340)
					{
						NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("StormWeaverHead"));
					}
					if (DoGSecondStageCountdown == 7140)
					{
						NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("CosmicWraith"));
					}
					if (DoGSecondStageCountdown <= 60)
					{
						if (!NPC.AnyNPCs(mod.NPCType("DevourerofGodsHeadS")))
						{
							NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DevourerofGodsHeadS"));
							string key = "Mods.CalamityMod.EdgyBossText10";
							Color messageColor = Color.Cyan;
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
				}
			}
			#endregion
			if (Main.player[closestPlayer].ZoneDungeon && !NPC.downedBoss3)
			{
				if (!NPC.AnyNPCs(NPCID.DungeonGuardian) && Main.netMode != 1)
					NPC.SpawnOnPlayer(closestPlayer, NPCID.DungeonGuardian); //your hell is as vast as my bonergrin, pray your life ends quickly
			}
			if (Main.player[closestPlayer].ZoneRockLayerHeight &&
				!Main.player[closestPlayer].ZoneUnderworldHeight &&
				!Main.player[closestPlayer].ZoneDungeon &&
				!Main.player[closestPlayer].ZoneJungle &&
				!Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).ZoneSunkenSea &&
				!CalamityPlayer.areThereAnyDamnBosses)
			{
				if (NPC.downedPlantBoss &&
					!Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).ZoneAbyss &&
					Main.player[closestPlayer].townNPCs < 3f)
				{
					if ((Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).zerg && Main.rand.Next(2000) == 0) ||
						(Main.rand.Next(100000) == 0 && !Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).zen))
					{
						if (!NPC.AnyNPCs(mod.NPCType("ArmoredDiggerHead")) && Main.netMode != 1)
							NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("ArmoredDiggerHead"));
					}
				}
			}
			if (Main.dayTime && Main.hardMode)
			{
				if (Main.player[closestPlayer].townNPCs >= 2f)
				{
					if (Main.rand.Next(2000) == 0)
					{
						int steamGril = NPC.FindFirstNPC(NPCID.Steampunker);
						if (steamGril == -1 && Main.netMode != 1)
							NPC.SpawnOnPlayer(closestPlayer, NPCID.Steampunker);
					}
				}
			}
			if (Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).ZoneAbyss)
			{
				if (Main.player[closestPlayer].chaosState)
				{
					if (!NPC.AnyNPCs(mod.NPCType("EidolonWyrmHeadHuge")) && Main.netMode != 1)
						NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("EidolonWyrmHeadHuge"));
				}
			}
			/*if (Main.rand.Next(100000000) == 0)
			{
				string key = "Mods.CalamityMod.AprilFools";
				Color messageColor = Color.Crimson;
				if (Main.netMode == 0)
				{
					Main.NewText(Language.GetTextValue(key), messageColor);
				}
				else if (Main.netMode == 2)
				{
					NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
				}
			}*/
			#region DeathModeBossSpawns
			if (death && !CalamityPlayer.areThereAnyDamnBosses && Main.player[closestPlayer].statLifeMax2 >= 300)
			{
				if (bossSpawnCountdown <= 0) //check for countdown being 0
				{
					if (Main.rand.Next(50000) == 0)
					{
						if (!NPC.downedBoss1 && bossType == 0) //only set countdown and boss type if conditions are met
							if (!Main.dayTime && (Main.player[closestPlayer].ZoneOverworldHeight || Main.player[closestPlayer].ZoneSkyHeight))
							{
								BossText();
								bossType = NPCID.EyeofCthulhu;
								bossSpawnCountdown = 3600; //1 minute
							}

						if (!NPC.downedBoss2 && bossType == 0)
							if (Main.player[closestPlayer].ZoneCorrupt)
							{
								BossText();
								bossType = NPCID.EaterofWorldsHead;
								bossSpawnCountdown = 3600;
							}

						if (!NPC.downedBoss2 && bossType == 0)
							if (Main.player[closestPlayer].ZoneCrimson)
							{
								BossText();
								bossType = NPCID.BrainofCthulhu;
								bossSpawnCountdown = 3600;
							}

						if (!NPC.downedQueenBee && bossType == 0)
							if (Main.player[closestPlayer].ZoneJungle && (Main.player[closestPlayer].ZoneOverworldHeight || Main.player[closestPlayer].ZoneSkyHeight))
							{
								BossText();
								bossType = NPCID.QueenBee;
								bossSpawnCountdown = 3600;
							}

						if (!downedDesertScourge && bossType == 0)
							if (Main.player[closestPlayer].ZoneDesert)
							{
								BossText();
								bossType = mod.NPCType("DesertScourgeHead");
								bossSpawnCountdown = 3600;
							}

						if (!downedPerforator && bossType == 0)
							if (Main.player[closestPlayer].ZoneCrimson)
							{
								BossText();
								bossType = mod.NPCType("PerforatorHive");
								bossSpawnCountdown = 3600;
							}

						if (!downedHiveMind && bossType == 0)
							if (Main.player[closestPlayer].ZoneCorrupt)
							{
								BossText();
								bossType = mod.NPCType("HiveMind");
								bossSpawnCountdown = 3600;
							}

						if (!downedCrabulon && bossType == 0)
							if (Main.player[closestPlayer].ZoneGlowshroom)
							{
								BossText();
								bossType = mod.NPCType("CrabulonIdle");
								bossSpawnCountdown = 3600;
							}

						if (Main.hardMode)
						{
							if (!NPC.downedMechBoss1 && bossType == 0)
								if (!Main.dayTime && (Main.player[closestPlayer].ZoneOverworldHeight || Main.player[closestPlayer].ZoneSkyHeight))
								{
									BossText();
									bossType = NPCID.TheDestroyer;
									bossSpawnCountdown = 3600;
								}

							if (!NPC.downedMechBoss2 && bossType == 0)
								if (!Main.dayTime && (Main.player[closestPlayer].ZoneOverworldHeight || Main.player[closestPlayer].ZoneSkyHeight))
								{
									BossText();
									bossType = NPCID.Spazmatism;
									bossSpawnCountdown = 3600;
								}

							if (!NPC.downedMechBoss3 && bossType == 0)
								if (!Main.dayTime && (Main.player[closestPlayer].ZoneOverworldHeight || Main.player[closestPlayer].ZoneSkyHeight))
								{
									BossText();
									bossType = NPCID.SkeletronPrime;
									bossSpawnCountdown = 3600;
								}

							if (!NPC.downedPlantBoss && bossType == 0)
								if (Main.player[closestPlayer].ZoneJungle && !Main.player[closestPlayer].ZoneOverworldHeight && !Main.player[closestPlayer].ZoneSkyHeight)
								{
									BossText();
									bossType = NPCID.Plantera;
									bossSpawnCountdown = 3600;
								}

							if (!NPC.downedFishron && bossType == 0)
								if (Main.player[closestPlayer].ZoneBeach)
								{
									BossText();
									bossType = NPCID.DukeFishron;
									bossSpawnCountdown = 3600;
								}

							if (!downedCryogen && bossType == 0)
								if (Main.player[closestPlayer].ZoneSnow && Main.player[closestPlayer].ZoneOverworldHeight)
								{
									BossText();
									bossType = mod.NPCType("Cryogen");
									bossSpawnCountdown = 3600;
								}

							if (!downedCalamitas && bossType == 0)
								if (!Main.dayTime && Main.player[closestPlayer].ZoneOverworldHeight)
								{
									BossText();
									bossType = mod.NPCType("Calamitas");
									bossSpawnCountdown = 3600;
								}

							if (!downedAstrageldon && bossType == 0)
								if (Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).ZoneAstral &&
									!Main.dayTime && Main.player[closestPlayer].ZoneOverworldHeight)
								{
									BossText();
									bossType = mod.NPCType("Astrageldon");
									bossSpawnCountdown = 3600;
								}

							if (!downedPlaguebringer && bossType == 0)
								if (Main.player[closestPlayer].ZoneJungle && NPC.downedGolemBoss && Main.player[closestPlayer].ZoneOverworldHeight)
								{
									BossText();
									bossType = mod.NPCType("PlaguebringerGoliath");
									bossSpawnCountdown = 3600;
								}

							if (NPC.downedMoonlord)
							{
								if (!downedGuardians && bossType == 0)
									if (Main.player[closestPlayer].ZoneUnderworldHeight ||
										(Main.player[closestPlayer].ZoneHoly && Main.player[closestPlayer].ZoneOverworldHeight))
									{
										BossText();
										bossType = mod.NPCType("ProfanedGuardianBoss");
										bossSpawnCountdown = 3600;
									}

								if (!downedBumble && bossType == 0)
									if (Main.player[closestPlayer].ZoneJungle && Main.player[closestPlayer].ZoneOverworldHeight)
									{
										BossText();
										bossType = mod.NPCType("Bumblefuck");
										bossSpawnCountdown = 3600;
									}
							}
						}
						if (Main.netMode == 2)
						{
							var netMessage = mod.GetPacket();
							netMessage.Write((byte)CalamityModMessageType.BossSpawnCountdownSync);
							netMessage.Write(bossSpawnCountdown);
							netMessage.Send();
							var netMessage2 = mod.GetPacket();
							netMessage2.Write((byte)CalamityModMessageType.BossTypeSync);
							netMessage2.Write(bossType);
							netMessage2.Send();
						}
					}
				}
				else
				{
					bossSpawnCountdown--;
					if (Main.netMode == 2)
					{
						var netMessage = mod.GetPacket();
						netMessage.Write((byte)CalamityModMessageType.BossSpawnCountdownSync);
						netMessage.Write(bossSpawnCountdown);
						netMessage.Send();
					}
					if (bossSpawnCountdown <= 0)
					{
						bool canSpawn = true;
						switch (bossType)
						{
							case NPCID.EyeofCthulhu:
								if (Main.dayTime || (!Main.player[closestPlayer].ZoneOverworldHeight && !Main.player[closestPlayer].ZoneSkyHeight))
									canSpawn = false;
								break;
							case NPCID.EaterofWorldsHead:
								if (!Main.player[closestPlayer].ZoneCorrupt)
									canSpawn = false;
								break;
							case NPCID.BrainofCthulhu:
								if (!Main.player[closestPlayer].ZoneCrimson)
									canSpawn = false;
								break;
							case NPCID.QueenBee:
								if (!Main.player[closestPlayer].ZoneJungle || (!Main.player[closestPlayer].ZoneOverworldHeight && !Main.player[closestPlayer].ZoneSkyHeight))
									canSpawn = false;
								break;
							case NPCID.TheDestroyer:
								if (Main.dayTime || (!Main.player[closestPlayer].ZoneOverworldHeight && !Main.player[closestPlayer].ZoneSkyHeight))
									canSpawn = false;
								break;
							case NPCID.Spazmatism:
								if (Main.dayTime || (!Main.player[closestPlayer].ZoneOverworldHeight && !Main.player[closestPlayer].ZoneSkyHeight))
									canSpawn = false;
								break;
							case NPCID.SkeletronPrime:
								if (Main.dayTime || (!Main.player[closestPlayer].ZoneOverworldHeight && !Main.player[closestPlayer].ZoneSkyHeight))
									canSpawn = false;
								break;
							case NPCID.Plantera:
								if (!Main.player[closestPlayer].ZoneJungle || Main.player[closestPlayer].ZoneOverworldHeight || Main.player[closestPlayer].ZoneSkyHeight)
									canSpawn = false;
								break;
							case NPCID.DukeFishron:
								if (!Main.player[closestPlayer].ZoneBeach)
									canSpawn = false;
								break;
						}

						if (bossType == mod.NPCType("DesertScourgeHead"))
						{
							if (!Main.player[closestPlayer].ZoneDesert)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("PerforatorHive"))
						{
							if (!Main.player[closestPlayer].ZoneCrimson)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("HiveMind"))
						{
							if (!Main.player[closestPlayer].ZoneCorrupt)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("CrabulonIdle"))
						{
							if (!Main.player[closestPlayer].ZoneGlowshroom)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("Cryogen"))
						{
							if (!Main.player[closestPlayer].ZoneSnow || !Main.player[closestPlayer].ZoneOverworldHeight)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("Calamitas"))
						{
							if (Main.dayTime || !Main.player[closestPlayer].ZoneOverworldHeight)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("Astrageldon"))
						{
							if (!Main.player[closestPlayer].GetModPlayer<CalamityPlayer>(mod).ZoneAstral ||
									Main.dayTime || !Main.player[closestPlayer].ZoneOverworldHeight)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("PlaguebringerGoliath"))
						{
							if (!Main.player[closestPlayer].ZoneJungle || !Main.player[closestPlayer].ZoneOverworldHeight)
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("ProfanedGuardianBoss"))
						{
							if (!Main.player[closestPlayer].ZoneUnderworldHeight &&
										(!Main.player[closestPlayer].ZoneHoly || !Main.player[closestPlayer].ZoneOverworldHeight))
								canSpawn = false;
						}
						else if (bossType == mod.NPCType("Bumblefuck"))
						{
							if (!Main.player[closestPlayer].ZoneJungle || !Main.player[closestPlayer].ZoneOverworldHeight)
								canSpawn = false;
						}

						if (canSpawn && Main.netMode != 1)
						{
							if (bossType == NPCID.Spazmatism)
								NPC.SpawnOnPlayer(closestPlayer, NPCID.Retinazer);
							else if (bossType == mod.NPCType("ProfanedGuardianBoss"))
							{
								NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("ProfanedGuardianBoss2"));
								NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("ProfanedGuardianBoss3"));
							}
							else if (bossType == mod.NPCType("DesertScourgeHead"))
							{
								NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DesertScourgeHeadSmall"));
								NPC.SpawnOnPlayer(closestPlayer, mod.NPCType("DesertScourgeHeadSmall"));
							}
							if (bossType == NPCID.DukeFishron)
								NPC.NewNPC((int)Main.player[closestPlayer].Center.X - 300, (int)Main.player[closestPlayer].Center.Y - 300, bossType);
							else
								NPC.SpawnOnPlayer(closestPlayer, bossType);
						}
						bossType = 0;
						if (Main.netMode == 2)
						{
							var netMessage = mod.GetPacket();
							netMessage.Write((byte)CalamityModMessageType.BossTypeSync);
							netMessage.Write(bossType);
							netMessage.Send();
						}
					}
				}
			}
			#endregion
			if (!NPC.downedBoss3 && revenge)
			{
				if (Main.netMode != 1)
				{
					if (Sandstorm.Happening)
					{
						Sandstorm.Happening = false;
						Sandstorm.TimeLeft = 0;
					}
				}
			}
			if (Main.netMode != 1)
			{
				if (revenge)
				{
					CultistRitual.delay -= Main.dayRate * 10;
					if (CultistRitual.delay < 0)
					{
						CultistRitual.delay = 0;
					}
					CultistRitual.recheck -= Main.dayRate * 10;
					if (CultistRitual.recheck < 0)
					{
						CultistRitual.recheck = 0;
					}
				}
			}
		}
		#endregion

		#region ChangeTime
		public static void ChangeTime(bool day)
		{
			Main.time = 0.0;
			if (day)
			{
				Main.dayTime = true;
			}
			else
			{
				Main.dayTime = false;
			}
			if (Main.netMode == 2)
			{
				NetMessage.SendData(7, -1, -1, null, 0, 0f, 0f, 0f, 0, 0, 0);
			}
		}
		#endregion

		#region BossText
		public void BossText()
		{
			string key = "Mods.CalamityMod.BossSpawnText";
			Color messageColor = Color.Crimson;
			if (Main.netMode == 0)
			{
				Main.NewText(Language.GetTextValue(key), messageColor);
			}
			else if (Main.netMode == 2)
			{
				NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
			}
		}
		#endregion
	}
}
