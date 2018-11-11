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
using CalamityMod.World.Planets;

namespace CalamityMod
{
	public class CalamityWorld : ModWorld
	{
        #region InstanceVars

        #region NormalVars
        public static bool bossRushActive = false;

        public static int bossRushSpawnCountdown = 180;

        private const int ExpandWorldBy = 200;

        private const int saveVersion = 0;

        private static int fehX = 0;

        private static int fehY = 0;

        private static int fuhX = 0;

        private static int fuhY = 0;

        private static int numAbyssIslands = 0;

        private static int[] AbyssIslandX = new int[20];

        private static int[] AbyssIslandY = new int[20];

        private static int[] AbyssItemArray = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

        private static bool genIcePyramid = true;

        private static int[] SChestX = new int[10];

        private static int[] SChestY = new int[10];

        private static bool genHellHut = true;

        private static bool genSandHut = true;

        private static bool genEvilHut = true;

        private static bool genPureHut = true; //forest

        private static bool genIceHut = true;

        private static bool genMushroomHut = true;

        private static bool genGraniteHut = true;

        private static bool genMarbleHut = true;

        private static bool genSwordHut = true;

        public static int calamityTiles = 0;
		
		public static int astralTiles = 0;

        public static int seaTiles = 0;

        public static int sulphurTiles = 0;

        public static int abyssTiles = 0;

        public static bool abyssSide = false;
		
		public static bool spring = false;
    	
    	public static bool summer = false;
    	
    	public static bool fall = false;
    	
    	public static bool winter = false;
		
		public static bool downedDesertScourge = false;

        public static bool downedAquaticScourge = false;
		
		public static bool downedHiveMind = false;
		
		public static bool downedPerforator = false;
		
		public static bool downedSlimeGod = false;
		
		public static bool downedCryogen = false;
		
		public static bool downedBrimstoneElemental = false;
		
		public static bool downedCalamitas = false;
		
		public static bool downedLeviathan = false;
		
		public static bool downedDoG = false;
		
		public static bool downedPlaguebringer = false;
		
		public static bool downedYharon = false;

        public static bool buffedEclipse = false;
		
		public static bool downedProvidence = false;
		
		public static bool downedGuardians = false;
		
		public static bool downedSentinel1 = false;
		
		public static bool downedSentinel2 = false;
		
		public static bool downedSentinel3 = false;
		
		public static bool downedSCal = false;
		
		public static bool downedBumble = false;
		
		public static bool downedCrabulon = false;
		
		public static bool downedBetsy = false;
		
		public static bool downedScavenger = false;
		
		public static bool downedWhar = false; //boss 2
		
		public static bool downedSkullHead = false; //boss 3
		
		public static bool downedUgly = false; //Wall
		
		public static bool downedSkeletor = false; //Skele of tron Prime
		
		public static bool downedPlantThing = false; //planter
		
		public static bool downedGolemBaby = false; //Baby
		
		public static bool downedMoonDude = false; //Moon guy
		
		public static bool downedBossAny = false; //Any boss
		
		public static bool spawnedHardBoss = false; //Hardmode boss spawned
		
		public static bool demonMode = false;
		
		public static bool onionMode = false;
		
		public static bool revenge = false;

        public static bool death = false;

        public static bool defiled = false;

        public static bool armageddon = false;
		
		public static bool downedStarGod = false;

        public static bool downedAstrageldon = false;

        public static bool spawnAstralMeteor = false;
		
		public static bool spawnAstralMeteor2 = false;
		
		public static bool spawnAstralMeteor3 = false;
		
		public static bool downedPolterghast = false;

        public static bool downedLORDE = false;

        public static bool downedBuffedMothron = false;

        public static bool ironHeart = false;
        #endregion

        #endregion

        #region Initialize
        public override void Initialize()
		{
            CalamityPlayer.bossRushStage = 0;
            CalamityGlobalNPC.golemDamageBonus = 1.0;
            CalamityGlobalNPC.DoGSecondStageCountdown = 0;
            CalamityGlobalNPC.holyBoss = -1;
            CalamityGlobalNPC.doughnutBoss = -1;
            CalamityGlobalNPC.voidBoss = -1;
            CalamityGlobalNPC.energyFlame = -1;
            CalamityGlobalNPC.hiveMind = -1;
            CalamityGlobalNPC.hiveMind2 = -1;
            CalamityGlobalNPC.scavenger = -1;
            CalamityGlobalNPC.DoGHead = -1;
            CalamityGlobalNPC.ghostBoss = -1;
            CalamityGlobalNPC.laserEye = -1;
            CalamityGlobalNPC.fireEye = -1;
            CalamityGlobalNPC.brimstoneElemental = -1;
            bossRushActive = false;
            bossRushSpawnCountdown = 180;
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
            if (death) downed.Add("death");
            if (defiled) downed.Add("defiled");
            if (armageddon) downed.Add("armageddon");
            if (ironHeart) downed.Add("ironHeart");
            if (abyssSide) downed.Add("abyssSide");
            if (bossRushActive) downed.Add("bossRushActive");

			return new TagCompound
			{
				{
					"downed", downed
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
            death = downed.Contains("death");
            defiled = downed.Contains("defiled");
            armageddon = downed.Contains("armageddon");
            ironHeart = downed.Contains("ironHeart");
            abyssSide = downed.Contains("abyssSide");
            bossRushActive = downed.Contains("bossRushActive");
        }
        #endregion

        #region LoadLegacy
        public override void LoadLegacy(BinaryReader reader)
		{
			int loadVersion = reader.ReadInt32();
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

            writer.Write(flags);
			writer.Write(flags2);
			writer.Write(flags3);
			writer.Write(flags4);
			writer.Write(flags5);
            writer.Write(flags6);
            writer.Write(flags7);
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
        }
        #endregion

        #region Tiles
        public override void ResetNearbyTileEffects()
		{
			calamityTiles = 0;
			astralTiles = 0;
            seaTiles = 0;
            sulphurTiles = 0;
            abyssTiles = 0;
		}
		
		public override void TileCountsAvailable(int[] tileCounts)
		{
			calamityTiles = tileCounts[mod.TileType("CharredOre")] + tileCounts[mod.TileType("BrimstoneSlag")];
            seaTiles = tileCounts[mod.TileType("EutrophicSand")] + tileCounts[mod.TileType("SeaPrism")];
            abyssTiles = tileCounts[mod.TileType("AbyssGravel")];
            sulphurTiles = tileCounts[mod.TileType("SulphurousSand")];
			
			#region Astral Stuff
            int astralDesertTiles = tileCounts[mod.TileType("AstralSand")] + tileCounts[mod.TileType("AstralSandstone")] + tileCounts[mod.TileType("HardenedAstralSand")];
            int astralSnowTiles = tileCounts[mod.TileType("AstralIce")];

            Main.sandTiles += astralDesertTiles;
            Main.snowTiles += astralSnowTiles;

            astralTiles = astralDesertTiles + astralSnowTiles + tileCounts[mod.TileType("AstralDirt")] + tileCounts[mod.TileType("AstralStone")] + tileCounts[mod.TileType("AstralGrass")]+ tileCounts[mod.TileType("AstralOre")];
			#endregion
		}
        #endregion

        #region PreWorldGen
        public override void PreWorldGen()
        {
            CalamityWorld.numAbyssIslands = 0;
            CalamityWorld.genIcePyramid = true;
            CalamityWorld.genHellHut = true;
            CalamityWorld.genSandHut = true;
            CalamityWorld.genEvilHut = true;
            CalamityWorld.genPureHut = true;
            CalamityWorld.genIceHut = true;
            CalamityWorld.genMushroomHut = true;
            CalamityWorld.genGraniteHut = true;
            CalamityWorld.genMarbleHut = true;
        }
        #endregion

        #region ModifyWorldGenTasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
		{
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
			if (ShiniesIndex != -1)
			{
				tasks.Insert(ShiniesIndex + 1, new PassLegacy("CalamityOres", delegate (GenerationProgress progress)
                {
                    progress.Message = "Ice Tomb";
                    int x = Main.maxTilesX;
                    int y = Main.maxTilesY;
                    int genLimit = x / 2;
                    int generateBack = genLimit - 80; //Small = 2020
                    int generateForward = genLimit + 80; //Small = 2180
                    #region IceTomb
                    for (int k = 0; k < (int)((double)(x * y) * 50E-05); k++)
                    {
                        int tilesX = WorldGen.genRand.Next(0, generateBack);
                        int tilesX2 = WorldGen.genRand.Next(generateForward, x);
                        int tilesY2 = WorldGen.genRand.Next((int)(y * .4f), (int)(y * .5f));
                        if ((Main.tile[tilesX, tilesY2].type == 147 || Main.tile[tilesX, tilesY2].type == 161) && CalamityWorld.genIcePyramid)
                        {
                            CalamityWorld.genIcePyramid = false;
                            IcePyramid(tilesX, tilesY2);
                        }
                        if ((Main.tile[tilesX2, tilesY2].type == 147 || Main.tile[tilesX2, tilesY2].type == 161) && CalamityWorld.genIcePyramid)
                        {
                            CalamityWorld.genIcePyramid = false;
                            IcePyramid(tilesX2, tilesY2);
                        }
                    }
                    #endregion
                    #region EvilIslandGen
                    int xIslandGen = WorldGen.crimson ?
                        WorldGen.genRand.Next((int)((double)x * 0.1), (int)((double)x * 0.4)) :
                        WorldGen.genRand.Next((int)((double)x * 0.6), (int)((double)x * 0.9));
                    int yIslandGen = WorldGen.genRand.Next(90, 151);
                    yIslandGen = Math.Min(yIslandGen, (int)WorldGen.worldSurfaceLow - 50);
                    int tileXLookup = xIslandGen;
                    if (WorldGen.crimson)
                    {
                        while (Main.tile[tileXLookup, yIslandGen].active())
                        {
                            tileXLookup++;
                        }
                    }
                    else
                    {
                        while (Main.tile[tileXLookup, yIslandGen].active())
                        {
                            tileXLookup--;
                        }
                    }
                    xIslandGen = tileXLookup;
                    CalamityWorld.fehX = xIslandGen;
                    CalamityWorld.fehY = yIslandGen;
                    EvilIsland(xIslandGen, yIslandGen);
                    EvilIslandHouse(CalamityWorld.fehX, CalamityWorld.fehY);
                    #endregion
                }));
			}
            int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (SulphurIndex != -1)
            {
                tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur", delegate (GenerationProgress progress)
                {
                    progress.Message = "Sulphur Beach";
                    int x = Main.maxTilesX;
                    int y = Main.maxTilesY;
                    int genLimit = x / 2;
                    if (WorldGen.dungeonX < genLimit) { abyssSide = true; }
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
                                        tile.type == 53 &&
                                        WorldGen.genRand.Next(4) == 0)
                                    {
                                        tile.type = (ushort)mod.TileType("SulphurousSand");
                                    }
                                    else if (tile.active() &&
                                             tile.type == 323)
                                    {
                                        tile.active(false);
                                    }
                                }
                                else if (abyssIndexSand > abyssChasmX + 210)
                                {
                                    if (tile.active() &&
                                        tile.type == 53 && 
                                        WorldGen.genRand.Next(2) == 0)
                                    {
                                        tile.type = (ushort)mod.TileType("SulphurousSand");
                                    }
                                    else if (tile.active() &&
                                             tile.type == 323)
                                    {
                                        tile.active(false);
                                    }
                                }
                                else
                                {
                                    if (tile.active() &&
                                        tile.type == 53)
                                    {
                                        tile.type = (ushort)mod.TileType("SulphurousSand");
                                    }
                                    else if (tile.active() &&
                                             tile.type == 323)
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
                                        tile.type == 53 &&
                                        WorldGen.genRand.Next(4) == 0)
                                    {
                                        tile.type = (ushort)mod.TileType("SulphurousSand");
                                    }
                                    else if (tile.active() &&
                                             tile.type == 323)
                                    {
                                        tile.active(false);
                                    }
                                }
                                else if (abyssIndexSand < abyssChasmX - 210)
                                {
                                    if (tile.active() &&
                                        tile.type == 53 &&
                                        WorldGen.genRand.Next(2) == 0)
                                    {
                                        tile.type = (ushort)mod.TileType("SulphurousSand");
                                    }
                                    else if (tile.active() &&
                                             tile.type == 323)
                                    {
                                        tile.active(false);
                                    }
                                }
                                else
                                {
                                    if (tile.active() &&
                                        tile.type == 53)
                                    {
                                        tile.type = (ushort)mod.TileType("SulphurousSand");
                                    }
                                    else if (tile.active() &&
                                             tile.type == 323)
                                    {
                                        tile.active(false);
                                    }
                                }
                            }
                        }
                    }
                }));
            }
            int CragIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
			if (CragIndex != -1) 
			{
                tasks.Insert(CragIndex + 1, new PassLegacy("HellCrag", delegate(GenerationProgress progress)
				{
					progress.Message = "Brimstone Crag, Abyss, and Structures";
                    int x = Main.maxTilesX;
                    int y = Main.maxTilesY;
                    int genLimit = x / 2;
                    int generateBack = genLimit - 80; //Small = 2020
                    int generateForward = genLimit + 80; //Small = 2180
                    int rockLayer = (int)Main.rockLayer;
                    #region AbyssGen
                    int abyssChasmSteps = y / 4;
                    int abyssChasmY = (y - abyssChasmSteps) + 100; // (y / 12)
                    int abyssChasmX = (abyssSide ? genLimit - (genLimit - 135) : genLimit + (genLimit - 135)); //2100 - 1965 = 135 : 2100 + 1965 = 4065
                    bool tenebrisSide = true;
                    if (abyssChasmX < genLimit) { tenebrisSide = false; }
                    if (abyssSide)
                    {
                        for (int abyssIndex = 0; abyssIndex < abyssChasmX + 80; abyssIndex++) //235
                        {
                            for (int abyssIndex2 = 0; abyssIndex2 < abyssChasmY; abyssIndex2++)
                            {
                                Tile tile = Framing.GetTileSafely(abyssIndex, abyssIndex2);
                                if (abyssIndex > abyssChasmX + 75)
                                {
                                    if (WorldGen.genRand.Next(4) == 0)
                                    {
                                        if (tile.active() &&
                                            tile.type != 227 && //dye plants
                                            tile.type != 323 && //trees
                                            tile.type != 53 && //sand
                                            tile.type != 21 && //chests
                                            tile.type != 81 && //coral
                                            tile.type != 324 && //seashells and starfish
                                            tile.type != (ushort)mod.TileType("SulphurousSand"))
                                        {
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                        else if (!tile.active() &&
                                                  abyssIndex2 > rockLayer)
                                        {
                                            tile.active(true);
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                    }
                                }
                                else if (abyssIndex > abyssChasmX + 70)
                                {
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        if (tile.active() &&
                                            tile.type != 227 && //dye plants
                                            tile.type != 323 && //trees
                                            tile.type != 53 && //sand
                                            tile.type != 21 && //chests
                                            tile.type != 81 && //coral
                                            tile.type != 324 && //seashells and starfish
                                            tile.type != (ushort)mod.TileType("SulphurousSand"))
                                        {
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                        else if (!tile.active() &&
                                                  abyssIndex2 > rockLayer)
                                        {
                                            tile.active(true);
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                    }
                                }
                                else
                                {
                                    if (tile.active() &&
                                        tile.type != 227 && //dye plants
                                        tile.type != 323 && //trees
                                        tile.type != 53 && //sand
                                        tile.type != 21 && //chests
                                        tile.type != 81 && //coral
                                        tile.type != 324 && //seashells and starfish
                                        tile.type != (ushort)mod.TileType("SulphurousSand"))
                                    {
                                        if (abyssIndex2 > (rockLayer + y * 0.262))
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else
                                        {
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                        }
                                    }
                                    else if (!tile.active() &&
                                              abyssIndex2 > rockLayer)
                                    {
                                        tile.active(true);
                                        if (abyssIndex2 > (rockLayer + y * 0.262))
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else
                                        {
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
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
                                if (abyssIndex < abyssChasmX - 75)
                                {
                                    if (WorldGen.genRand.Next(4) == 0)
                                    {
                                        if (tile.active() &&
                                            tile.type != 227 && //dye plants
                                            tile.type != 323 && //trees
                                            tile.type != 53 && //sand
                                            tile.type != 21 && //chests
                                            tile.type != 81 && //coral
                                            tile.type != 324 && //seashells and starfish
                                            tile.type != (ushort)mod.TileType("SulphurousSand"))
                                        {
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                        else if (!tile.active() &&
                                                  abyssIndex2 > rockLayer)
                                        {
                                            tile.active(true);
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                    }
                                }
                                else if (abyssIndex < abyssChasmX - 70)
                                {
                                    if (WorldGen.genRand.Next(2) == 0)
                                    {
                                        if (tile.active() &&
                                            tile.type != 227 && //dye plants
                                            tile.type != 323 && //trees
                                            tile.type != 53 && //sand
                                            tile.type != 21 && //chests
                                            tile.type != 81 && //coral
                                            tile.type != 324 && //seashells and starfish
                                            tile.type != (ushort)mod.TileType("SulphurousSand"))
                                        {
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                        else if (!tile.active() &&
                                                  abyssIndex2 > rockLayer)
                                        {
                                            tile.active(true);
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                    }
                                }
                                else
                                {
                                    if (tile.active() &&
                                        tile.type != 227 && //dye plants
                                        tile.type != 323 && //trees
                                        tile.type != 53 && //sand
                                        tile.type != 21 && //chests
                                        tile.type != 81 && //coral
                                        tile.type != 324 && //seashells and starfish
                                        tile.type != (ushort)mod.TileType("SulphurousSand"))
                                    {
                                        if (abyssIndex2 > (rockLayer + y * 0.262))
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else
                                        {
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                        }
                                    }
                                    else if (!tile.active() &&
                                              abyssIndex2 > rockLayer)
                                    {
                                        tile.active(true);
                                        if (abyssIndex2 > (rockLayer + y * 0.262))
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else if (abyssIndex2 > (rockLayer + y * 0.143) && WorldGen.genRand.Next(3) == 0)
                                        {
                                            tile.type = (ushort)mod.TileType("Voidstone");
                                            tile.wall = (byte)mod.WallType("VoidstoneWallUnsafe");
                                        }
                                        else
                                        {
                                            tile.wall = (byte)mod.WallType("AbyssGravelWall");
                                            tile.type = (ushort)mod.TileType("AbyssGravel");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    ChasmGenerator(abyssChasmX, (int)WorldGen.worldSurfaceLow, abyssChasmSteps, true);
                    int maxAbyssIslands = 11;
                    if (y > 1500) { maxAbyssIslands = 15; if (y > 2100) { maxAbyssIslands = 19; } }
                    int islandLocationOffset = 30;
                    int islandLocationY = rockLayer;
                    for (int islands = 0; islands < maxAbyssIslands; islands++)
                    {
                        int islandLocationX = abyssChasmX;
                        int randomIsland = WorldGen.genRand.Next(5); //0 1 2 3 4
                        bool hasVoidstone = islandLocationY > (rockLayer + y * 0.143);
                        CalamityWorld.AbyssIslandY[CalamityWorld.numAbyssIslands] = islandLocationY;
                        switch (randomIsland)
                        {
                            case 0:
                                AbyssIsland(islandLocationX, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                                AbyssIsland(islandLocationX + 40, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                                AbyssIsland(islandLocationX - 40, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                                break;
                            case 1:
                                AbyssIsland(islandLocationX + 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                                AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                                AbyssIsland(islandLocationX - 40, islandLocationY + 10, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                                islandLocationX += 30;
                                break;
                            case 2:
                                AbyssIsland(islandLocationX - 30, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                                AbyssIsland(islandLocationX + 30, islandLocationY + 10, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                                AbyssIsland(islandLocationX, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                                islandLocationX -= 30;
                                break;
                            case 3:
                                AbyssIsland(islandLocationX + 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                                AbyssIsland(islandLocationX - 5, islandLocationY + 5, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                                AbyssIsland(islandLocationX - 35, islandLocationY + 15, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                                islandLocationX += 25;
                                break;
                            case 4:
                                AbyssIsland(islandLocationX - 25, islandLocationY, 60, 66, 30, 36, true, false, hasVoidstone);
                                AbyssIsland(islandLocationX + 5, islandLocationY + 15, 60, 66, 30, 36, false, tenebrisSide, hasVoidstone);
                                AbyssIsland(islandLocationX + 35, islandLocationY + 5, 60, 66, 30, 36, false, !tenebrisSide, hasVoidstone);
                                islandLocationX -= 25;
                                break;
                        }
                        CalamityWorld.AbyssIslandX[CalamityWorld.numAbyssIslands] = islandLocationX;
                        CalamityWorld.numAbyssIslands++;
                        islandLocationY += islandLocationOffset;
                    }
                    CalamityWorld.AbyssItemArray = ShuffleArray(CalamityWorld.AbyssItemArray);
                    for (int abyssHouse = 0; abyssHouse < CalamityWorld.numAbyssIslands; abyssHouse++) //11 15 19
                    {
                        AbyssIslandHouse(CalamityWorld.AbyssIslandX[abyssHouse], 
                            CalamityWorld.AbyssIslandY[abyssHouse], 
                            CalamityWorld.AbyssItemArray[(abyssHouse > 9 ? (abyssHouse - 10) : abyssHouse)], //10 choices 0 to 9
                            CalamityWorld.AbyssIslandY[abyssHouse] > (rockLayer + y * 0.143));
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
                                            SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                        }
                                    }
                                    else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                             abyssIndex2 < (int)Main.worldSurface)
                                    {
                                        if (WorldGen.genRand.Next(3) == 0)
                                        {
                                            int style = WorldGen.genRand.Next(25, 28);
                                            WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                            SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
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
                                            SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                        }
                                    }
                                    else if (WorldGen.SolidTile(abyssIndex, abyssIndex2 + 1) &&
                                             abyssIndex2 < (int)Main.worldSurface)
                                    {
                                        if (WorldGen.genRand.Next(3) == 0)
                                        {
                                            int style = WorldGen.genRand.Next(25, 28);
                                            WorldGen.PlacePot(abyssIndex, abyssIndex2, 28, style);
                                            SafeSquareTileFrame(abyssIndex, abyssIndex2, true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                    #region Shrines
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
                        int tilesY6 = y - 50;
                        if ((Main.tile[tilesX3, tilesY3].type == 0 || Main.tile[tilesX3, tilesY3].type == 1) && CalamityWorld.genPureHut)
                        {
                            CalamityWorld.genPureHut = false;
                            SpecialHut((ushort)39, (ushort)0, (byte)6, 0, tilesX3, tilesY3); //red brick, dirt, red brick wall
                        }
                        if ((Main.tile[tilesX4, tilesY3].type == 0 || Main.tile[tilesX4, tilesY3].type == 1) && CalamityWorld.genPureHut)
                        {
                            CalamityWorld.genPureHut = false;
                            SpecialHut((ushort)39, (ushort)0, (byte)6, 0, tilesX4, tilesY3);
                        }
                        if (Main.tile[tilesX, tilesY3].type == (ushort)(WorldGen.crimson ? 203 : 25) && CalamityWorld.genEvilHut)
                        {
                            CalamityWorld.genEvilHut = false;
                            SpecialHut((ushort)(WorldGen.crimson ? 347 : 140), (ushort)(WorldGen.crimson ? 203 : 25), (byte)(WorldGen.crimson ? 174 : 33), 1, tilesX, tilesY3); //crimstone brick, crimstone, crimstone brick wall
                        }
                        if (Main.tile[tilesX2, tilesY3].type == (ushort)(WorldGen.crimson ? 203 : 25) && CalamityWorld.genEvilHut)
                        {
                            CalamityWorld.genEvilHut = false;
                            SpecialHut((ushort)(WorldGen.crimson ? 347 : 140), (ushort)(WorldGen.crimson ? 203 : 25), (byte)(WorldGen.crimson ? 174 : 33), 1, tilesX2, tilesY3);
                        }
                        if (Main.tile[tilesX3, tilesY5].type == 1 && CalamityWorld.genHellHut)
                        {
                            CalamityWorld.genHellHut = false;
                            SpecialHut((ushort)75, (ushort)56, (byte)20, 2, tilesX3, tilesY5); //obsidian brick, obsidian, obsidian brick wall
                        }
                        if (Main.tile[tilesX4, tilesY5].type == 1 && CalamityWorld.genHellHut)
                        {
                            CalamityWorld.genHellHut = false;
                            SpecialHut((ushort)75, (ushort)56, (byte)20, 2, tilesX4, tilesY5);
                        }
                        if (Main.tile[tilesX, tilesY4].type == 161 && CalamityWorld.genIceHut)
                        {
                            CalamityWorld.genIceHut = false;
                            SpecialHut((ushort)206, (ushort)161, (byte)84, 3, tilesX, tilesY4); //ice brick, ice, ice brick wall
                        }
                        if (Main.tile[tilesX2, tilesY4].type == 161 && CalamityWorld.genIceHut)
                        {
                            CalamityWorld.genIceHut = false;
                            SpecialHut((ushort)206, (ushort)161, (byte)84, 3, tilesX2, tilesY4);
                        }
                        if (Main.tile[tilesX, tilesY4].type == 404 && CalamityWorld.genSandHut)
                        {
                            CalamityWorld.genSandHut = false;
                            SpecialHut((ushort)404, (ushort)396, (byte)223, 4, tilesX, tilesY4); //fossil, sand, fossil wall
                        }
                        if (Main.tile[tilesX2, tilesY4].type == 404 && CalamityWorld.genSandHut)
                        {
                            CalamityWorld.genSandHut = false;
                            SpecialHut((ushort)404, (ushort)396, (byte)223, 4, tilesX2, tilesY4);
                        }
                        if (Main.tile[tilesX3, tilesY4].type == 59 && CalamityWorld.genMushroomHut)
                        {
                            CalamityWorld.genMushroomHut = false;
                            SpecialHut((ushort)190, (ushort)59, (byte)74, 5, tilesX3, tilesY4); //mushroom block, mud, mushroom wall
                        }
                        if (Main.tile[tilesX4, tilesY4].type == 59 && CalamityWorld.genMushroomHut)
                        {
                            CalamityWorld.genMushroomHut = false;
                            SpecialHut((ushort)190, (ushort)59, (byte)74, 5, tilesX4, tilesY4);
                        }
                        if (Main.tile[tilesX, tilesY4].type == 368 && CalamityWorld.genGraniteHut)
                        {
                            CalamityWorld.genGraniteHut = false;
                            SpecialHut((ushort)369, (ushort)368, (byte)181, 6, tilesX, tilesY4); //granite brick, granite, granite brick wall
                        }
                        if (Main.tile[tilesX2, tilesY4].type == 368 && CalamityWorld.genGraniteHut)
                        {
                            CalamityWorld.genGraniteHut = false;
                            SpecialHut((ushort)369, (ushort)368, (byte)181, 6, tilesX2, tilesY4);
                        }
                        if (Main.tile[tilesX, tilesY4].type == 367 && CalamityWorld.genMarbleHut)
                        {
                            CalamityWorld.genMarbleHut = false;
                            SpecialHut((ushort)357, (ushort)367, (byte)179, 7, tilesX, tilesY4); //marble brick, marble, marble brick wall
                        }
                        if (Main.tile[tilesX2, tilesY4].type == 367 && CalamityWorld.genMarbleHut)
                        {
                            CalamityWorld.genMarbleHut = false;
                            SpecialHut((ushort)357, (ushort)367, (byte)179, 7, tilesX2, tilesY4);
                        }
                        if (CalamityWorld.genSwordHut)
                        {
                            CalamityWorld.genSwordHut = false;
                            SpecialHut((ushort)76, (ushort)58, (byte)177, 8, tilesX5, tilesY6);
                        }
                    }
                    #endregion
                    #region Crag
                    int xUnderworldGen = WorldGen.genRand.Next((int)((double)x * 0.1), (int)((double)x * 0.15));
                    int yUnderworldGen = y - 100;
                    CalamityWorld.fuhX = xUnderworldGen;
                    CalamityWorld.fuhY = yUnderworldGen;
                    UnderworldIsland(xUnderworldGen, yUnderworldGen, 180, 201, 120, 136);
                    UnderworldIsland(xUnderworldGen - 50, yUnderworldGen - 30, 100, 111, 60, 71);
                    UnderworldIsland(xUnderworldGen + 50, yUnderworldGen - 30, 100, 111, 60, 71);
                    ChasmGenerator(CalamityWorld.fuhX - 110, CalamityWorld.fuhY - 10, WorldGen.genRand.Next(150) + 150);
                    ChasmGenerator(CalamityWorld.fuhX + 110, CalamityWorld.fuhY - 10, WorldGen.genRand.Next(150) + 150);
                    UnderworldIsland(xUnderworldGen - 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    UnderworldIsland(xUnderworldGen + 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    UnderworldIsland(xUnderworldGen - 180, yUnderworldGen - 20, 60, 66, 35, 41);
                    UnderworldIsland(xUnderworldGen + 180, yUnderworldGen - 20, 60, 66, 35, 41);
                    UnderworldIslandHouse(CalamityWorld.fuhX, CalamityWorld.fuhY + 30, 1323);
                    UnderworldIslandHouse(CalamityWorld.fuhX - 22, CalamityWorld.fuhY + 15, 1322);
                    UnderworldIslandHouse(CalamityWorld.fuhX + 22, CalamityWorld.fuhY + 15, 535);
                    UnderworldIslandHouse(CalamityWorld.fuhX - 50, CalamityWorld.fuhY - 30, 112);
                    UnderworldIslandHouse(CalamityWorld.fuhX + 50, CalamityWorld.fuhY - 30, 906);
                    UnderworldIslandHouse(CalamityWorld.fuhX - 150, CalamityWorld.fuhY - 30, 218);
                    UnderworldIslandHouse(CalamityWorld.fuhX + 150, CalamityWorld.fuhY - 30, 3019);
                    UnderworldIslandHouse(CalamityWorld.fuhX - 180, CalamityWorld.fuhY - 20, 274);
                    UnderworldIslandHouse(CalamityWorld.fuhX + 180, CalamityWorld.fuhY - 20, 220);
                    #endregion
                }));
			}
            //RE-ENABLE IF YOU WANT QUICK TESTING FOR WORLD GEN C:
            //RemoveMostTasks(tasks);

            tasks.Add(new PassLegacy("Expand World", ExpandWorld));
            //EVERYTHING THAT WILL SPAWN IN EXPANDED HEIGHT AREA MUST BE DONE AFTER THIS SO:
            tasks.Add(new PassLegacy("Planetoid Test", Planetoids));

            //FIX THE POTS BEING A PROBLEM USING A CUSTOM FIX
            int pots = tasks.FindIndex(task => task.Name == "Pots");
            if (pots != -1)
            {
                tasks.RemoveAt(pots);
                tasks.Insert(pots, new PassLegacy("Pots", PotFix));
            }
        }
        #endregion

        #region SafeTileFrame
        public static void SafeSquareTileFrame(int i, int j, bool resetFrame = true)
        {
            for (int x = i - 1; x <= i + 1; x++)
            {
                for (int y = j - 1; y <= j + 1; y++)
                {
                    if (x < 0 || y < 0 || x >= Main.maxTilesX || y >= Main.maxTilesY) continue;
                    if (x == i && y == j) WorldGen.TileFrame(i, j, resetFrame, false);
                    else WorldGen.TileFrame(x, y, false, false);
                }
            }
        }
        #endregion

        #region ShuffleArray
        public static int[] ShuffleArray(int[] array)
        {
            Random random = new Random();
            for (int index = array.Length; index > 0; index--)
            {
                int j = random.Next(index);
                int k = array[j];
                array[j] = array[index - 1];
                array[index - 1] = k;
            }
            return array;
        }
        #endregion

        #region Seasons
        public static void checkSpring()
		{
			DateTime now = DateTime.Now;
			int day = now.Day;
			int month = now.Month;
			if (month >= 3 && month <= 6)
			{
				if (month == 3 && day >= 20)
				{
					spring = true;
					return;
				}
				if (month == 4)
				{
					spring = true;
					return;
				}
				if (month == 5)
				{
					spring = true;
					return;
				}
				if (month == 6 && day <= 20)
				{
					spring = true;
					return;
				}
			}
			spring = false;
		}
    	
    	public static void checkSummer()
		{
			DateTime now = DateTime.Now;
			int day = now.Day;
			int month = now.Month;
			if (month >= 6 && month <= 9)
			{
				if (month == 6 && day >= 21)
				{
					summer = true;
					return;
				}
				if (month == 7)
				{
					summer = true;
					return;
				}
				if (month == 8)
				{
					summer = true;
					return;
				}
				if (month == 9 && day <= 21)
				{
					summer = true;
					return;
				}
			}
			summer = false;
		}
    	
    	public static void checkFall()
		{
			DateTime now = DateTime.Now;
			int day = now.Day;
			int month = now.Month;
			if (month >= 9 && month <= 12)
			{
				if (month == 9 && day >= 22)
				{
					fall = true;
					return;
				}
				if (month == 10)
				{
					fall = true;
					return;
				}
				if (month == 11)
				{
					fall = true;
					return;
				}
				if (month == 12 && day <= 20)
				{
					fall = true;
					return;
				}
			}
			fall = false;
		}
    	
    	public static void checkWinter()
		{
			DateTime now = DateTime.Now;
			int day = now.Day;
			int month = now.Month;
			if (month == 12 || (month >= 1 && month <= 3))
			{
				if (month == 12 && day >= 21)
				{
					winter = true;
					return;
				}
				if (month == 1)
				{
					winter = true;
					return;
				}
				if (month == 2)
				{
					winter = true;
					return;
				}
				if (month == 3 && day <= 19)
				{
					winter = true;
					return;
				}
			}
			winter = false;
		}
        #endregion

        #region OreSpawn
        public static void spawnOre(int type, double frequency, float depth, float depthLimit)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            bool genCenter = false;
            int x = Main.maxTilesX;
            int y = Main.maxTilesY;
            int genLimit = x / 2;
            int generateBack = genLimit - 80; //Small = 2020
            int generateForward = genLimit + 80; //Small = 2180
            if (type == mod.TileType("ExodiumOre"))
            {
                genCenter = true;
                depthLimit = 0.14f;
                if (y > 1500) { depthLimit = 0.1f; if (y > 2100) { depthLimit = 0.07f; } }
            }
            if (Main.netMode != 1)
            {
                for (int k = 0; k < (int)((double)(x * y) * frequency); k++)
                {
                    int tilesX = WorldGen.genRand.Next(0, (genCenter ? genLimit : generateBack));
                    int tilesX2 = WorldGen.genRand.Next((genCenter ? genLimit : generateForward), x);
                    int tilesY = WorldGen.genRand.Next((int)(y * depth), (int)(y * depthLimit));
                    if (type == mod.TileType("AuricOre"))
                    {
                        WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(12, 18), WorldGen.genRand.Next(12, 18), (ushort)type);
                        WorldGen.OreRunner(tilesX2, tilesY, (double)WorldGen.genRand.Next(12, 18), WorldGen.genRand.Next(12, 18), (ushort)type);
                    }
                    else if (type == mod.TileType("UelibloomOre"))
                    {
                        if (Main.tile[tilesX, tilesY].type == 59)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                        if (Main.tile[tilesX2, tilesY].type == 59)
                        {
                            WorldGen.OreRunner(tilesX2, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else if (type == mod.TileType("PerennialOre"))
                    {
                        if (Main.tile[tilesX, tilesY].type == 0 || Main.tile[tilesX, tilesY].type == 1)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                        if (Main.tile[tilesX2, tilesY].type == 0 || Main.tile[tilesX2, tilesY].type == 1)
                        {
                            WorldGen.OreRunner(tilesX2, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else if (type == mod.TileType("CryonicOre"))
                    {
                        if (Main.tile[tilesX, tilesY].type == 147 || Main.tile[tilesX, tilesY].type == 161 || Main.tile[tilesX, tilesY].type == 163 || Main.tile[tilesX, tilesY].type == 164 || Main.tile[tilesX, tilesY].type == 200)
                        {
                            WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                        if (Main.tile[tilesX2, tilesY].type == 147 || Main.tile[tilesX2, tilesY].type == 161 || Main.tile[tilesX2, tilesY].type == 163 || Main.tile[tilesX2, tilesY].type == 164 || Main.tile[tilesX2, tilesY].type == 200)
                        {
                            WorldGen.OreRunner(tilesX2, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        }
                    }
                    else
                    {
                        WorldGen.OreRunner(tilesX, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                        WorldGen.OreRunner(tilesX2, tilesY, (double)WorldGen.genRand.Next(3, 8), WorldGen.genRand.Next(3, 8), (ushort)type);
                    }
                }
            }
        }
        #endregion

        #region AstralMeteor
        public static bool checkAstralMeteor()
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int num = 0;
            float num2 = (float)(Main.maxTilesX / 4200);
            int num3 = (int)(200f * num2); //Small = 201 Medium = 305 Large = 401
            for (int j = 5; j < Main.maxTilesX - 5; j++)
            {
                int num4 = 5;
                while ((double)num4 < Main.worldSurface)
                {
                    if (Main.tile[j, num4].active() && Main.tile[j, num4].type == mod.TileType("AstralOre"))
                    {
                        num++;
                        if (num > num3)
                        {
                            return false;
                        }
                    }
                    num4++;
                }
            }
            return true;
        }

        public static void dropAstralMeteor()
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			bool flag = true;
			if (Main.netMode == 1)
			{
				return;
			}
			for (int i = 0; i < 255; i++)
			{
				if (Main.player[i].active)
				{
					flag = false;
					break;
				}
			}
            if (!checkAstralMeteor())
            {
                return;
            }
			float num5 = 600f;
			while (!flag)
			{
				float num6 = (float)Main.maxTilesX * 0.08f;
				int num7 = Main.rand.Next(150, Main.maxTilesX - 150);
				while ((float)num7 > (float)Main.spawnTileX - num6 && (float)num7 < (float)Main.spawnTileX + num6)
				{
					num7 = Main.rand.Next(150, Main.maxTilesX - 150);
				}
				int k = (int)(Main.worldSurface * 0.3);
				while (k < Main.maxTilesY)
				{
					if (Main.tile[num7, k].active() && Main.tileSolid[(int)Main.tile[num7, k].type])
					{
						int num8 = 0;
						int num9 = 15;
						for (int l = num7 - num9; l < num7 + num9; l++)
						{
							for (int m = k - num9; m < k + num9; m++)
							{
								if (WorldGen.SolidTile(l, m))
								{
									num8++;
									if (Main.tile[l, m].type == 189 || Main.tile[l, m].type == 202)
									{
										num8 -= 100;
									}
								}
								else if (Main.tile[l, m].liquid > 0)
								{
									num8--;
								}
							}
						}
						if ((float)num8 < num5)
						{
							num5 -= 0.5f;
							break;
						}
						flag = CalamityWorld.astralMeteor(num7, k);
						if (flag)
						{
							break;
						}
						break;
					}
					else
					{
						k++;
					}
				}
				if (num5 < 100f)
				{
					return;
				}
			}
		}
		
		public static bool astralMeteor(int i, int j)
		{
			Mod mod = ModLoader.GetMod("CalamityMod");
			if (i < 50 || i > Main.maxTilesX - 50)
			{
				return false;
			}
			if (j < 50 || j > Main.maxTilesY - 50)
			{
				return false;
			}
			int num = 35;
			Rectangle rectangle = new Rectangle((i - num) * 16, (j - num) * 16, num * 2 * 16, num * 2 * 16);
			for (int k = 0; k < 255; k++)
			{
				if (Main.player[k].active)
				{
					Rectangle value = new Rectangle((int)(Main.player[k].position.X + (float)(Main.player[k].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[k].position.Y + (float)(Main.player[k].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
					if (rectangle.Intersects(value))
					{
						return false;
					}
				}
			}
			for (int l = 0; l < 200; l++)
			{
				if (Main.npc[l].active)
				{
					Rectangle value2 = new Rectangle((int)Main.npc[l].position.X, (int)Main.npc[l].position.Y, Main.npc[l].width, Main.npc[l].height);
					if (rectangle.Intersects(value2))
					{
						return false;
					}
				}
			}
			for (int m = i - num; m < i + num; m++)
			{
				for (int n = j - num; n < j + num; n++)
				{
					if (Main.tile[m, n].active() && Main.tile[m, n].type == 21)
					{
						return false;
					}
				}
			}
			num = WorldGen.genRand.Next(17, 23);
			for (int num2 = i - num; num2 < i + num; num2++)
			{
				for (int num3 = j - num; num3 < j + num; num3++)
				{
					if (num3 > j + Main.rand.Next(-2, 3) - 5)
					{
						float num4 = (float)Math.Abs(i - num2);
						float num5 = (float)Math.Abs(j - num3);
						float num6 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
						if ((double)num6 < (double)num * 0.9 + (double)Main.rand.Next(-4, 5))
						{
							if (!Main.tileSolid[(int)Main.tile[num2, num3].type])
							{
								Main.tile[num2, num3].active(false);
							}
							Main.tile[num2, num3].type = (ushort)mod.TileType("AstralOre");
						}
					}
				}
			}
			num = WorldGen.genRand.Next(8, 14);
			for (int num7 = i - num; num7 < i + num; num7++)
			{
				for (int num8 = j - num; num8 < j + num; num8++)
				{
					if (num8 > j + Main.rand.Next(-2, 3) - 4)
					{
						float num9 = (float)Math.Abs(i - num7);
						float num10 = (float)Math.Abs(j - num8);
						float num11 = (float)Math.Sqrt((double)(num9 * num9 + num10 * num10));
						if ((double)num11 < (double)num * 0.8 + (double)Main.rand.Next(-3, 4))
						{
							Main.tile[num7, num8].active(false);
						}
					}
				}
			}
			num = WorldGen.genRand.Next(25, 35);
			for (int num12 = i - num; num12 < i + num; num12++)
			{
				for (int num13 = j - num; num13 < j + num; num13++)
				{
					float num14 = (float)Math.Abs(i - num12);
					float num15 = (float)Math.Abs(j - num13);
					float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
					if ((double)num16 < (double)num * 0.7)
					{
						if (Main.tile[num12, num13].type == 5 || Main.tile[num12, num13].type == 32 || Main.tile[num12, num13].type == 352)
						{
							WorldGen.KillTile(num12, num13, false, false, false);
						}
						Main.tile[num12, num13].liquid = 0;
					}
					if (Main.tile[num12, num13].type == (ushort)mod.TileType("AstralOre"))
					{
						if (!WorldGen.SolidTile(num12 - 1, num13) && !WorldGen.SolidTile(num12 + 1, num13) && !WorldGen.SolidTile(num12, num13 - 1) && !WorldGen.SolidTile(num12, num13 + 1))
						{
							Main.tile[num12, num13].active(false);
						}
						else if ((Main.tile[num12, num13].halfBrick() || Main.tile[num12 - 1, num13].topSlope()) && !WorldGen.SolidTile(num12, num13 + 1))
						{
							Main.tile[num12, num13].active(false);
						}
					}
					WorldGen.SquareTileFrame(num12, num13, true);
					WorldGen.SquareWallFrame(num12, num13, true);
				}
			}
			num = WorldGen.genRand.Next(23, 32);
			for (int num17 = i - num; num17 < i + num; num17++)
			{
				for (int num18 = j - num; num18 < j + num; num18++)
				{
					if (num18 > j + WorldGen.genRand.Next(-3, 4) - 3 && Main.tile[num17, num18].active() && Main.rand.Next(10) == 0)
					{
						float num19 = (float)Math.Abs(i - num17);
						float num20 = (float)Math.Abs(j - num18);
						float num21 = (float)Math.Sqrt((double)(num19 * num19 + num20 * num20));
						if ((double)num21 < (double)num * 0.8)
						{
							if (Main.tile[num17, num18].type == 5 || Main.tile[num17, num18].type == 32 || Main.tile[num17, num18].type == 352)
							{
								WorldGen.KillTile(num17, num18, false, false, false);
							}
							Main.tile[num17, num18].type = (ushort)mod.TileType("AstralOre");
							WorldGen.SquareTileFrame(num17, num18, true);
						}
					}
				}
			}
			num = WorldGen.genRand.Next(30, 38);
			for (int num22 = i - num; num22 < i + num; num22++)
			{
				for (int num23 = j - num; num23 < j + num; num23++)
				{
					if (num23 > j + WorldGen.genRand.Next(-2, 3) && Main.tile[num22, num23].active() && Main.rand.Next(20) == 0)
					{
						float num24 = (float)Math.Abs(i - num22);
						float num25 = (float)Math.Abs(j - num23);
						float num26 = (float)Math.Sqrt((double)(num24 * num24 + num25 * num25));
						if ((double)num26 < (double)num * 0.85)
						{
							if (Main.tile[num22, num23].type == 5 || Main.tile[num22, num23].type == 32 || Main.tile[num22, num23].type == 352)
							{
								WorldGen.KillTile(num22, num23, false, false, false);
							}
							Main.tile[num22, num23].type = (ushort)mod.TileType("AstralOre");
							WorldGen.SquareTileFrame(num22, num23, true);
						}
					}
				}
			}
			if (Main.netMode != 1)
			{
				NetMessage.SendTileSquare(-1, i, j, 40, TileChangeType.None);
				
                DoAstralConversion(new Point(i, j));
			}
			return true;
		}
        
		

        public static void DoAstralConversion(object obj)
        {
            //Pre-calculate all variables necessary for elliptical area checking
            Point origin = (Point)obj;
            Vector2 center = origin.ToVector2() * 16f + new Vector2(8f);

            float angle = MathHelper.Pi * 0.15f;
            float otherAngle = MathHelper.PiOver2 - angle;

            int distanceInTiles = 150 + ((Main.maxTilesX - 4200) / 4200) * 200;
            float distance = distanceInTiles * 16f;
            float constant = (distance * 2f) / (float)Math.Sin(angle);

            float fociSpacing = ((distance * (float)Math.Sin(otherAngle)) / (float)Math.Sin(angle));
            int verticalRadius = (int)(constant / 16f);

            Vector2 fociOffset = Vector2.UnitY * fociSpacing;
            Vector2 topFoci = center - fociOffset;
            Vector2 bottomFoci = center + fociOffset;
            
            for (int x = origin.X - distanceInTiles - 2; x <= origin.X + distanceInTiles + 2; x++)
            {
                for (int y = (int)(origin.Y - verticalRadius * 0.4f) - 3; y <= origin.Y + verticalRadius + 3; y++)
                {
                    float dist;
                    if (CheckInEllipse(new Point(x, y), topFoci, bottomFoci, constant, center, out dist, y < origin.Y))
                    {
                        //If we're in the outer blurPercent% of the ellipse
                        float percent = dist / constant;
                        float blurPercent = 0.98f;
                        if (percent > blurPercent)
                        {
                            float outerEdgePercent = (percent - blurPercent) / (1f - blurPercent);
                            if (Main.rand.NextFloat(1f) > outerEdgePercent)
                            {
                                ConvertToAstral(x, y);
                            }
                        }
                        else
                        {
                            ConvertToAstral(x, y);
                        }
                    }
                }
            }
        }

        public static void ConvertToAstral(int startX, int endX, int startY, int endY)
        {
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    ConvertToAstral(x, y);
                }
            }
        }

        public static void ConvertToAstral(int x, int y, bool tileframe = true)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            if (WorldGen.InWorld(x, y, 1))
            {
                int type = Main.tile[x, y].type;
                int wallType = Main.tile[x, y].wall;

                if (WallID.Sets.Conversion.Grass[wallType])
                {
                    Main.tile[x, y].wall = (ushort)mod.WallType("AstralGrassWallUnsafe");
                }
                else if (WallID.Sets.Conversion.HardenedSand[wallType])
                {
                    Main.tile[x, y].wall = (ushort)mod.WallType("HardenedAstralSandWallUnsafe");
                }
                else if (WallID.Sets.Conversion.Sandstone[wallType])
                {
                    Main.tile[x, y].wall = (ushort)mod.WallType("AstralSandstoneWallUnsafe");
                }
                else if (WallID.Sets.Conversion.Stone[wallType])
                {
                    Main.tile[x, y].wall = (ushort)mod.WallType("AstralStoneWallUnsafe");
                }
                else
                {
                    switch (wallType)
                    {
                        case WallID.DirtUnsafe:
                        case WallID.DirtUnsafe1:
                        case WallID.DirtUnsafe2:
                        case WallID.DirtUnsafe3:
                        case WallID.DirtUnsafe4:
                        case WallID.Cave6Unsafe:
                        case WallID.Dirt:
                            Main.tile[x, y].wall = (ushort)mod.WallType("AstralDirtWallUnsafe");
                            break;
                        case WallID.IceUnsafe:
                            Main.tile[x, y].wall = (ushort)mod.WallType("AstralIceWallUnsafe");
                            break;
                    }
                }

                if (TileID.Sets.Conversion.Grass[type] && !TileID.Sets.GrassSpecial[type])
                {
                    Main.tile[x, y].type = (ushort)mod.TileType("AstralGrass");
                }
                else if (TileID.Sets.Conversion.Stone[type] || Main.tileMoss[type])
                {
                    Main.tile[x, y].type = (ushort)mod.TileType("AstralStone");
                }
                else if (TileID.Sets.Conversion.Sand[type])
                {
                    Main.tile[x, y].type = (ushort)mod.TileType("AstralSand");
                }
                else if (TileID.Sets.Conversion.HardenedSand[type])
                {
                    Main.tile[x, y].type = (ushort)mod.TileType("HardenedAstralSand");
                }
                else if (TileID.Sets.Conversion.Sandstone[type])
                {
                    Main.tile[x, y].type = (ushort)mod.TileType("AstralSandstone");
                }
                else if (TileID.Sets.Conversion.Ice[type])
                {
                    Main.tile[x, y].type = (ushort)mod.TileType("AstralIce");
                }
                else
                {
                    Tile tile = Main.tile[x, y];
                    switch(type)
                    {
                        case TileID.Dirt:
                            Main.tile[x, y].type = (ushort)mod.TileType("AstralDirt");
                            break;
                        case TileID.Vines:
                            Main.tile[x, y].type = (ushort)mod.TileType("AstralVines");
                            break;
                        case TileID.LargePiles:
                            if (tile.frameX <= 1170)
                            {
                                RecursiveReplaceToAstral(TileID.LargePiles, (ushort)mod.TileType("AstralNormalLargePiles"), x, y, 324, 0, 1170, 0, 18);
                            }
                            if (tile.frameX >= 1728)
                            {
                                RecursiveReplaceToAstral(TileID.LargePiles, (ushort)mod.TileType("AstralNormalLargePiles"), x, y, 324, 1728, 1872, 0, 18);
                            }
                            if (tile.frameX >= 1404 && tile.frameX <= 1710)
                            {
                                RecursiveReplaceToAstral(TileID.LargePiles, (ushort)mod.TileType("AstralIceLargePiles"), x, y, 324, 1404, 1710, 0, 18);
                            }
                            break;
                        case TileID.LargePiles2:
                            if (tile.frameX >= 1566 && tile.frameY < 36)
                            {
                                RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)mod.TileType("AstralDesertLargePiles"), x, y, 324, 1566, 1872, 0, 18);
                            }
                            if (tile.frameX >= 756 && tile.frameX <= 900)
                            {
                                RecursiveReplaceToAstral(TileID.LargePiles2, (ushort)mod.TileType("AstralNormalLargePiles"), x, y, 324, 756, 900, 0, 18);
                            }
                            break;
                        case TileID.SmallPiles:
                            if (tile.frameY == 18)
                            {
                                ushort newType = 9999;
                                if (tile.frameX >= 1476 && tile.frameX <= 1674)
                                {
                                    newType = (ushort)mod.TileType("AstralDesertMediumPiles");
                                }
                                else if (tile.frameX <= 558 || (tile.frameX >= 1368 && tile.frameX <= 1458))
                                {
                                    newType = (ushort)mod.TileType("AstralNormalMediumPiles");
                                }
                                else if (tile.frameX >= 900 && tile.frameX <= 1098)
                                {
                                    newType = (ushort)mod.TileType("AstralIceMediumPiles");
                                }
                                else
                                {
                                    break;
                                }
                                int leftMost = x;
                                if (tile.frameX % 36 != 0) //this means it's the right tile of the two
                                {
                                    leftMost--;
                                }
                                Main.tile[leftMost, y].type = newType;
                                Main.tile[leftMost + 1, y].type = newType;
                                while (Main.tile[leftMost, y].frameX >= 216)
                                {
                                    Main.tile[leftMost, y].frameX -= 216;
                                    Main.tile[leftMost + 1, y].frameX -= 216;
                                }
                            }
                            else if (tile.frameY == 0)
                            {
                                ushort newType3 = 9999;
                                if (tile.frameX >= 972 && tile.frameX <= 1062)
                                {
                                    newType3 = (ushort)mod.TileType("AstralDesertSmallPiles");
                                }
                                else if (tile.frameX <= 486)
                                {
                                    newType3 = (ushort)mod.TileType("AstralNormalSmallPiles");
                                }
                                else if (tile.frameX >= 648 && tile.frameX <= 846)
                                {
                                    newType3 = (ushort)mod.TileType("AstralIceSmallPiles");
                                }
                                else
                                {
                                    break;
                                }
                                Main.tile[x, y].type = newType3;
                                while (Main.tile[x, y].frameX >= 108) //REFRAME IT
                                {
                                    Main.tile[x, y].frameX -= 108;
                                }
                            }
                            break;
                        case TileID.Stalactite:
                            int topMost = tile.frameY <= 54 ? (tile.frameY % 36 == 0 ? y : y - 1) : y;
                            bool twoTall = tile.frameY <= 54;
                            bool hanging = tile.frameY <= 18 || tile.frameY == 72;
                            ushort newType2 = 9999;
                            if (tile.frameX >= 378 && tile.frameX <= 414) //DESERT
                            {
                                newType2 = (ushort)mod.TileType("AstralDesertStalactite");
                            }
                            else if ((tile.frameX >= 54 && tile.frameX <= 90) || (tile.frameX >= 216 && tile.frameX <= 360))
                            {
                                newType2 = (ushort)mod.TileType("AstralNormalStalactite");
                            }
                            else if (tile.frameX <= 36)
                            {
                                newType2 = (ushort)mod.TileType("AstralIceStalactite");
                            }
                            else
                            {
                                break;
                            }

                            //Set types
                            Main.tile[x, topMost].type = newType2;
                            if (twoTall)
                                Main.tile[x, topMost + 1].type = newType2;

                            //Fix frames
                            while (Main.tile[x, topMost].frameX >= 54)
                            {
                                Main.tile[x, topMost].frameX -= 54;
                                if (twoTall)
                                    Main.tile[x, topMost + 1].frameX -= 54;
                            }

                            if (hanging)
                            {
                                ConvertToAstral(x, topMost - 1);
                                break;
                            }
                            else
                            {
                                if (twoTall)
                                {
                                    ConvertToAstral(x, topMost + 2);
                                    break;
                                }
                                ConvertToAstral(x, topMost + 1);
                                break;
                            }
                    }
                }
                if (tileframe)
                {
                    if (Main.netMode == 0)
                    {
                        WorldGen.SquareTileFrame(x, y, true);
                    }
                    else if (Main.netMode == 2)
                    {
                        NetMessage.SendTileSquare(-1, x, y, 1);
                    }
                }
            }
        }

        public static void ConvertFromAstral(int x, int y, ConvertType convert)
        {
            Tile tile = Main.tile[x, y];
            int type = tile.type;
            int wallType = tile.wall;
            Mod mod = CalamityMod.Instance;

            if (WorldGen.InWorld(x, y, 1))
            {
                #region WALL
                if (wallType == mod.WallType("AstralDirtWall"))
                {
                    Main.tile[x, y].wall = WallID.DirtUnsafe;
                }
                else if (wallType == mod.WallType("AstralGrassWall"))
                {
                    switch (convert)
                    {
                        case ConvertType.Corrupt:
                            Main.tile[x, y].wall = WallID.CorruptGrassUnsafe;
                            break;
                        case ConvertType.Crimson:
                            Main.tile[x, y].wall = WallID.CrimsonGrassUnsafe;
                            break;
                        case ConvertType.Hallow:
                            Main.tile[x, y].wall = WallID.HallowedGrassUnsafe;
                            break;
                        case ConvertType.Pure:
                            Main.tile[x, y].wall = WallID.GrassUnsafe;
                            break;
                    }
                }
                else if (wallType == mod.WallType("AstralIceWall"))
                {
                    Main.tile[x, y].wall = WallID.IceUnsafe;
                }
                else if (wallType == mod.WallType("AstralStoneWall"))
                {
                    switch (convert)
                    {
                        case ConvertType.Corrupt:
                            Main.tile[x, y].wall = WallID.EbonstoneUnsafe;
                            break;
                        case ConvertType.Crimson:
                            Main.tile[x, y].wall = WallID.CrimstoneUnsafe;
                            break;
                        case ConvertType.Hallow:
                            Main.tile[x, y].wall = WallID.PearlstoneBrickUnsafe;
                            break;
                        case ConvertType.Pure:
                            Main.tile[x, y].wall = WallID.Stone;
                            break;
                    }
                }
                #endregion

                #region TILE
                if (type == mod.TileType("AstralDirt"))
                {
                    tile.type = TileID.Dirt;
                }
                else if (type == mod.TileType("AstralGrass"))
                {
                    SetTileFromConvert(x, y, convert, TileID.CorruptGrass, TileID.FleshGrass, TileID.HallowedGrass, TileID.Grass);
                }
                else if (type == mod.TileType("AstralStone"))
                {
                    SetTileFromConvert(x, y, convert, TileID.Ebonstone, TileID.Crimstone, TileID.Pearlstone, TileID.Stone);
                }
                else if (type == mod.TileType("AstralSand"))
                {
                    SetTileFromConvert(x, y, convert, TileID.Ebonsand, TileID.Crimsand, TileID.Pearlsand, TileID.Sand);
                }
                else if (type == mod.TileType("AstralSandstone"))
                {
                    SetTileFromConvert(x, y, convert, TileID.CorruptSandstone, TileID.CrimsonSandstone, TileID.HallowSandstone, TileID.Sandstone);
                }
                else if (type == mod.TileType("HardenedAstralSand"))
                {
                    SetTileFromConvert(x, y, convert, TileID.CorruptHardenedSand, TileID.CrimsonHardenedSand, TileID.HallowHardenedSand, TileID.HardenedSand);
                }
                else if (type == mod.TileType("AstralIce"))
                {
                    SetTileFromConvert(x, y, convert, TileID.CorruptIce, TileID.FleshIce, TileID.HallowedIce, TileID.IceBlock);
                }
                else if (type == mod.TileType("AstralVines"))
                {
                    SetTileFromConvert(x, y, convert, ushort.MaxValue, TileID.CrimsonVines, TileID.HallowedVines, TileID.Vines);
                }
                else if (type == mod.TileType("AstralShortPlants"))
                {
                    SetTileFromConvert(x, y, convert, TileID.CorruptPlants, ushort.MaxValue, TileID.HallowedPlants, TileID.Plants);
                }
                else if (type == mod.TileType("AstralTallPlants"))
                {
                    SetTileFromConvert(x, y, convert, ushort.MaxValue, ushort.MaxValue, TileID.HallowedPlants2, TileID.Plants2);
                }
                else if (type == mod.TileType("AstralNormalLargePiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 378, 0);
                }
                else if (type == mod.TileType("AstralNormalMediumPiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 18);
                }
                else if (type == mod.TileType("AstralNormalSmallPiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 0, 0);
                }
                else if (type == mod.TileType("AstralDesertLargePiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles2, x, y, 1566, 0);
                }
                else if (type == mod.TileType("AstralDesertMediumPiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 1476, 18);
                }
                else if (type == mod.TileType("AstralDesertSmallPiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 972, 0);
                }
                else if (type == mod.TileType("AstralIceLargePiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.LargePiles, x, y, 1404, 0);
                }
                else if (type == mod.TileType("AstralIceMediumPiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 900, 18);
                }
                else if (type == mod.TileType("AstralIceSmallPiles"))
                {
                    RecursiveReplaceFromAstral((ushort)type, TileID.SmallPiles, x, y, 648, 0);
                }
                else if (type == mod.TileType("AstralNormalStalactite"))
                {
                    ushort originType = TileID.Stone;
                    int frameXAdd = 54;
                    switch (convert)
                    {
                        case ConvertType.Corrupt:
                            originType = TileID.Ebonstone;
                            frameXAdd = 324;
                            break;
                        case ConvertType.Crimson:
                            originType = TileID.Crimstone;
                            frameXAdd = 270;
                            break;
                        case ConvertType.Hallow:
                            originType = TileID.Pearlstone;
                            frameXAdd = 216;
                            break;
                    }
                    ReplaceAstralStalactite((ushort)type, TileID.Stalactite, originType, x, y, frameXAdd, 0);
                }
                else if (type == mod.TileType("AstralDesertStalactite"))
                {
                    ushort originType = TileID.Sandstone;
                    int frameXAdd = 378;
                    switch (convert)
                    {
                        case ConvertType.Corrupt:
                            originType = TileID.CorruptSandstone;
                            frameXAdd = 324;
                            break;
                        case ConvertType.Crimson:
                            originType = TileID.CrimsonSandstone;
                            frameXAdd = 270;
                            break;
                        case ConvertType.Hallow:
                            originType = TileID.HallowSandstone;
                            frameXAdd = 216;
                            break;
                    }
                    ReplaceAstralStalactite((ushort)type, TileID.Stalactite, originType, x, y, frameXAdd, 0);
                }
                else if (type == mod.TileType("AstralIceStalactite"))
                {
                    ReplaceAstralStalactite((ushort)type, TileID.Stalactite, TileID.IceBlock, x, y, 0, 0);
                }
                if (TileID.Sets.Conversion.Grass[type] || type == TileID.Dirt)
                {
                    WorldGen.SquareTileFrame(x, y);
                }
                #endregion
            }
        }

        private static void SetTileFromConvert(int x, int y, ConvertType convert, ushort corrupt, ushort crimson, ushort hallow, ushort pure)
        {
            switch (convert)
            {
                case ConvertType.Corrupt:
                    if (corrupt != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = corrupt;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Crimson:
                    if (crimson != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = crimson;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Hallow:
                    if (hallow != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = hallow;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
                case ConvertType.Pure:
                    if (pure != ushort.MaxValue)
                    {
                        Main.tile[x, y].type = pure;
                        WorldGen.SquareTileFrame(x, y);
                    }
                    break;
            }
        }

        private static void RecursiveReplaceToAstral(ushort checkType, ushort replaceType, int x, int y, int replaceTextureWidth, int minFrameX = 0, int maxFrameX = int.MaxValue, int minFrameY = 0, int maxFrameY = int.MaxValue)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active() || tile.type != checkType || tile.frameX < minFrameX || tile.frameX > maxFrameX || tile.frameY < minFrameY || tile.frameY > maxFrameY)
                return;

            Main.tile[x, y].type = replaceType;
            while (Main.tile[x, y].frameX >= replaceTextureWidth)
            {
                Main.tile[x, y].frameX -= (short)replaceTextureWidth;
            }

            RecursiveReplaceToAstral(checkType, replaceType, x - 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            RecursiveReplaceToAstral(checkType, replaceType, x + 1, y, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            RecursiveReplaceToAstral(checkType, replaceType, x, y - 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
            RecursiveReplaceToAstral(checkType, replaceType, x, y + 1, replaceTextureWidth, minFrameX, maxFrameX, minFrameY, maxFrameY);
        }

        private static void RecursiveReplaceFromAstral(ushort checkType, ushort replaceType, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];
            if (tile == null || !tile.active() || tile.type != checkType)
                return;

            Main.tile[x, y].type = replaceType;
            Main.tile[x, y].frameX += (short)addFrameX;
            Main.tile[x, y].frameY += (short)addFrameY;

            RecursiveReplaceFromAstral(checkType, replaceType, x - 1, y, addFrameX, addFrameY);
            RecursiveReplaceFromAstral(checkType, replaceType, x + 1, y, addFrameX, addFrameY);
            RecursiveReplaceFromAstral(checkType, replaceType, x, y - 1, addFrameX, addFrameY);
            RecursiveReplaceFromAstral(checkType, replaceType, x, y + 1, addFrameX, addFrameY);
        }

        private static void ReplaceAstralStalactite(ushort checkType, ushort replaceType, ushort replaceOriginTile, int x, int y, int addFrameX, int addFrameY)
        {
            Tile tile = Main.tile[x, y];

            int topMost = tile.frameY <= 54 ? (tile.frameY % 36 == 0 ? y : y - 1) : y;
            bool twoTall = tile.frameY <= 54;
            bool hanging = tile.frameY <= 18 || tile.frameY == 72;

            int yOriginTile = (hanging ? topMost - 1 : (twoTall ? topMost + 2 : y + 1));

            Main.tile[x, topMost++].type = replaceType;
            if (twoTall)
            {
                Main.tile[x, topMost].type = replaceType;
            }
            Main.tile[x, yOriginTile].type = replaceOriginTile;
        }

        private static bool CheckInEllipse(Point tile, Vector2 focus1, Vector2 focus2, float distanceConstant, Vector2 center, out float distance, bool collapse = false)
        {
            Vector2 point = tile.ToVector2() * 16f + new Vector2(8f);
            if (collapse) //Collapse ensures the ellipse is shrunk down a lot in terms of distance.
            {
                float distY = center.Y - point.Y;
                point.Y -= distY * 8f;
            }
            float distance1 = Vector2.Distance(point, focus1);
            float distance2 = Vector2.Distance(point, focus2);
            distance = distance1 + distance2;
            return distance <= distanceConstant;
        }
		
		#endregion

        #region EvilIsland
        public static void EvilIsland(int i, int j)
        {
            double num = (double)WorldGen.genRand.Next(100, 150); //100 150
            float num2 = (float)WorldGen.genRand.Next(20, 30); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(WorldGen.crimson ? 400 : 401); //ebonstone or crimstone
                                WorldGen.SquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = (WorldGen.crimson ? 400 : 401);
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = (WorldGen.crimson ? 398 : 399);
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(80, 95);
            num2 = (float)WorldGen.genRand.Next(10, 15);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = num5 - 1;
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(80, 120) * 0.01;
                float num19 = vector.Y + 1f;
                for (int num20 = num7; num20 < num8; num20++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num19 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num19 < vector.Y)
                    {
                        num19 = vector.Y;
                    }
                    if (num19 > vector.Y + 2f)
                    {
                        num19 = vector.Y + 2f;
                    }
                    for (int num21 = num9; num21 < num10; num21++)
                    {
                        if ((float)num21 > num19)
                        {
                            float arg_69E_0 = Math.Abs((float)num20 - vector.X);
                            float num22 = Math.Abs((float)num21 - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_69E_0 * arg_69E_0 + num22 * num22)) < num11 * 0.4 && 
                                Main.tile[num20, num21].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num20, num21].type = (ushort)(WorldGen.crimson ? 22 : 204); //ore
                                WorldGen.SquareTileFrame(num20, num21, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = (WorldGen.crimson ? 400 : 401);
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (ushort)(WorldGen.crimson ? 220 : 221);
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (WorldGen.crimson ? 400 : 401))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
            int num38 = WorldGen.genRand.Next(4);
            for (int num39 = 0; num39 <= num38; num39++)
            {
                int num40 = WorldGen.genRand.Next(num3 - 5, num4 + 5);
                int num41 = num5 - WorldGen.genRand.Next(20, 40);
                int num42 = WorldGen.genRand.Next(4, 8);
                int num43 = (WorldGen.crimson ? 400 : 401);
                if (WorldGen.genRand.Next(2) == 0)
                {
                    num43 = (WorldGen.crimson ? 398 : 399);
                }
                for (int num44 = num40 - num42; num44 <= num40 + num42; num44++)
                {
                    for (int num45 = num41 - num42; num45 <= num41 + num42; num45++)
                    {
                        float arg_C74_0 = (float)Math.Abs(num44 - num40);
                        float num46 = (float)(Math.Abs(num45 - num41) * 2);
                        if (Math.Sqrt((double)(arg_C74_0 * arg_C74_0 + num46 * num46)) < (double)(num42 + WorldGen.genRand.Next(-1, 2)))
                        {
                            Main.tile[num44, num45].active(true);
                            Main.tile[num44, num45].type = (ushort)num43;
                            WorldGen.SquareTileFrame(num44, num45, true);
                        }
                    }
                }
                for (int num47 = num40 - num42 + 2; num47 <= num40 + num42 - 2; num47++)
                {
                    int num48 = num41 - num42;
                    while (!Main.tile[num47, num48].active())
                    {
                        num48++;
                    }
                    Main.tile[num47, num48].active(false);
                    Main.tile[num47, num48].liquid = 255;
                    WorldGen.SquareTileFrame(num47, num48, true);
                }
            }
        }
        #endregion

        #region EvilIslandHouse
        public static void EvilIslandHouse(int i, int j)
        {
            ushort type = (ushort)(WorldGen.crimson ? 152 : 347); //tile
            byte wall = (byte)(WorldGen.crimson ? 35 : 174); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, (WorldGen.crimson ? 1 : 10)); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            int contain = 0;
            if (WorldGen.crimson)
            {
                contain = 1571; //scourge
            }
            else
            {
                contain = 1569; //vampire
            }
            WorldGen.AddBuriedChest(i, num10 - 3, contain, false, (WorldGen.crimson ? 19 : 20)); //chest
            int num14 = i - num2 / 2 + 1;
            int num15 = i + num2 / 2 - 1;
            int num16 = 1;
            if (num2 > 10)
            {
                num16 = 2;
            }
            int num17 = (num6 + num7) / 2 - 1;
            for (int num18 = num14 - num16; num18 <= num14 + num16; num18++)
            {
                for (int num19 = num17 - 1; num19 <= num17 + 1; num19++)
                {
                    Main.tile[num18, num19].wall = 21; //glass
                }
            }
            for (int num20 = num15 - num16; num20 <= num15 + num16; num20++)
            {
                for (int num21 = num17 - 1; num21 <= num17 + 1; num21++)
                {
                    Main.tile[num20, num21].wall = 21; //glass
                }
            }
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, 14, true, false, -1, (WorldGen.crimson ? 1 : 8)); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, 15, true, false, 0, (WorldGen.crimson ? 2 : 11)); //chair
            Tile expr_510 = Main.tile[num22 - 2, num7 - 1];
            expr_510.frameX += 18;
            Tile expr_531 = Main.tile[num22 - 2, num7 - 2];
            expr_531.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, 15, true, false, 0, (WorldGen.crimson ? 2 : 11)); //chair
        }
        #endregion

        #region UnderworldIsland
        public static void UnderworldIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMin / 5, sizeMax / 5); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)mod.TileType("BrimstoneSlag");
                                WorldGen.SquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = mod.TileType("BrimstoneSlag");
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = mod.TileType("CharredOre");
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                WorldGen.SquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMin2 / 8, sizeMax2 / 8);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = mod.TileType("BrimstoneSlag");
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    WorldGen.SquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (ushort)mod.TileType("BrimstoneSlag"))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (ushort)mod.TileType("BrimstoneSlag"))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                WorldGen.SquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (ushort)mod.TileType("BrimstoneSlag"))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (ushort)mod.TileType("BrimstoneSlag"))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                WorldGen.SquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    WorldGen.SquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region UnderworldIslandHouse
        public static void UnderworldIslandHouse(int i, int j, int item)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            ushort type = (ushort)mod.TileType("BrimstoneSlag"); //tile
            byte wall = (byte)14; //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(7, 12);
            int num3 = WorldGen.genRand.Next(5, 7);
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].liquid = 0;
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = 0;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == 0)
                    {
                        Main.tile[n, num8].active(false);
                        Main.tile[n, num8].wall = wall;
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            WorldGen.PlaceTile(num9, num10, 10, true, false, -1, 19); //door
            num9 = i + (num2 + 1) * -num - num;
            for (int num12 = num6; num12 <= num7 + 1; num12++)
            {
                Main.tile[num9, num12].active(true);
                Main.tile[num9, num12].liquid = 0;
                Main.tile[num9, num12].type = type;
                Main.tile[num9, num12].wall = 0;
                Main.tile[num9, num12].halfBrick(false);
                Main.tile[num9, num12].slope(0);
            }
            WorldGen.AddBuriedChest(i, num10 - 3, item, false, 4); //chest
            int num22 = i + (num2 / 2 + 1) * -num;
            WorldGen.PlaceTile(num22, num7 - 1, 14, true, false, -1, 13); //table
            WorldGen.PlaceTile(num22 - 2, num7 - 1, 15, true, false, 0, 16); //chair
            Tile expr_510 = Main.tile[num22 - 2, num7 - 1];
            expr_510.frameX += 18;
            Tile expr_531 = Main.tile[num22 - 2, num7 - 2];
            expr_531.frameX += 18;
            WorldGen.PlaceTile(num22 + 2, num7 - 1, 15, true, false, 0, 16); //chair
        }
        #endregion

        #region AbyssIsland
        public static void AbyssIsland(int i, int j, int sizeMin, int sizeMax, int sizeMin2, int sizeMax2, bool hasChest, bool hasTenebris, bool isVoid)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int sizeMinSmall = sizeMin / 5;
            int sizeMaxSmall = sizeMax / 5;
            double num = (double)WorldGen.genRand.Next(sizeMin, sizeMax); //100 150
            float num2 = (float)WorldGen.genRand.Next(sizeMinSmall, sizeMaxSmall); //20 30
            int num3 = i;
            int num4 = i;
            int num5 = i;
            int num6 = j;
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                int num7 = (int)((double)vector.X - num * 0.5);
                int num8 = (int)((double)vector.X + num * 0.5);
                int num9 = (int)((double)vector.Y - num * 0.5);
                int num10 = (int)((double)vector.Y + num * 0.5);
                if (num7 < 0)
                {
                    num7 = 0;
                }
                if (num8 > Main.maxTilesX)
                {
                    num8 = Main.maxTilesX;
                }
                if (num9 < 0)
                {
                    num9 = 0;
                }
                if (num10 > Main.maxTilesY)
                {
                    num10 = Main.maxTilesY;
                }
                double num11 = num * (double)WorldGen.genRand.Next(sizeMin, sizeMax) * 0.01; //80 120
                float num12 = vector.Y + 1f;
                for (int k = num7; k < num8; k++)
                {
                    if (WorldGen.genRand.Next(2) == 0)
                    {
                        num12 += (float)WorldGen.genRand.Next(-1, 2);
                    }
                    if (num12 < vector.Y)
                    {
                        num12 = vector.Y;
                    }
                    if (num12 > vector.Y + 2f)
                    {
                        num12 = vector.Y + 2f;
                    }
                    for (int l = num9; l < num10; l++)
                    {
                        if ((float)l > num12)
                        {
                            float arg_218_0 = Math.Abs((float)k - vector.X);
                            float num13 = Math.Abs((float)l - vector.Y) * 3f;
                            if (Math.Sqrt((double)(arg_218_0 * arg_218_0 + num13 * num13)) < num11 * 0.4)
                            {
                                if (k < num3)
                                {
                                    num3 = k;
                                }
                                if (k > num4)
                                {
                                    num4 = k;
                                }
                                if (l < num5)
                                {
                                    num5 = l;
                                }
                                if (l > num6)
                                {
                                    num6 = l;
                                }
                                Main.tile[k, l].active(true);
                                Main.tile[k, l].type = (ushort)(isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel"));
                                SafeSquareTileFrame(k, l, true);
                            }
                        }
                    }
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int m = num3;
            int num15;
            for (m += WorldGen.genRand.Next(5); m < num4; m += WorldGen.genRand.Next(num15, (int)((double)num15 * 1.5)))
            {
                int num14 = num6;
                while (!Main.tile[m, num14].active())
                {
                    num14--;
                }
                num14 += WorldGen.genRand.Next(-3, 4);
                num15 = WorldGen.genRand.Next(4, 8);
                int num16 = (isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel"));
                if (WorldGen.genRand.Next(4) == 0)
                {
                    num16 = (hasChest ? mod.TileType("ChaoticOre") : mod.TileType("PlantyMush"));
                }
                for (int n = m - num15; n <= m + num15; n++)
                {
                    for (int num17 = num14 - num15; num17 <= num14 + num15; num17++)
                    {
                        if (num17 > num5)
                        {
                            float arg_409_0 = (float)Math.Abs(n - m);
                            float num18 = (float)(Math.Abs(num17 - num14) * 2);
                            if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num15 + WorldGen.genRand.Next(2)))
                            {
                                Main.tile[n, num17].active(true);
                                Main.tile[n, num17].type = (ushort)num16;
                                SafeSquareTileFrame(n, num17, true);
                            }
                        }
                    }
                }
            }
            if (hasTenebris)
            {
                int p = num3;
                int num150;
                for (p += WorldGen.genRand.Next(5); p < num4; p += WorldGen.genRand.Next(num150, (int)((double)num150 * 1.5)))
                {
                    int num14 = num6;
                    while (!Main.tile[p, num14].active())
                    {
                        num14--;
                    }
                    num14 += WorldGen.genRand.Next(-3, 4); //-3 4
                    num150 = 1; //4 8
                    int num16 = mod.TileType("Tenebris");
                    for (int n = p - num150; n <= p + num150; n++)
                    {
                        for (int num17 = num14 - num150; num17 <= num14 + num150; num17++)
                        {
                            if (num17 > num5)
                            {
                                float arg_409_0 = (float)Math.Abs(n - p);
                                float num18 = (float)(Math.Abs(num17 - num14) * 2);
                                if (Math.Sqrt((double)(arg_409_0 * arg_409_0 + num18 * num18)) < (double)(num150 + WorldGen.genRand.Next(2)))
                                {
                                    Main.tile[n, num17].active(true);
                                    Main.tile[n, num17].type = (ushort)num16;
                                    SafeSquareTileFrame(n, num17, true);
                                }
                            }
                        }
                    }
                }
            }
            int sizeMinSmall2 = sizeMin2 / 8;
            int sizeMaxSmall2 = sizeMax2 / 8;
            num = (double)WorldGen.genRand.Next(sizeMin2, sizeMax2);
            num2 = (float)WorldGen.genRand.Next(sizeMinSmall2, sizeMaxSmall2);
            vector.X = (float)i;
            vector.Y = (float)num5;
            vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            while (vector2.X > -2f && vector2.X < 2f)
            {
                vector2.X = (float)WorldGen.genRand.Next(-20, 21) * 0.2f;
            }
            vector2.Y = (float)WorldGen.genRand.Next(-20, -10) * 0.02f;
            while (num > 0.0 && num2 > 0f)
            {
                num -= (double)WorldGen.genRand.Next(4);
                num2 -= 1f;
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-20, 21) * 0.05f;
                if (vector2.X > 1f)
                {
                    vector2.X = 1f;
                }
                if (vector2.X < -1f)
                {
                    vector2.X = -1f;
                }
                if ((double)vector2.Y > 0.2)
                {
                    vector2.Y = -0.2f;
                }
                if ((double)vector2.Y < -0.2)
                {
                    vector2.Y = -0.2f;
                }
            }
            int num23 = num3;
            num23 += WorldGen.genRand.Next(5);
            while (num23 < num4)
            {
                int num24 = num6;
                while ((!Main.tile[num23, num24].active() || Main.tile[num23, num24].type != 0) && num23 < num4)
                {
                    num24--;
                    if (num24 < num5)
                    {
                        num24 = num6;
                        num23 += WorldGen.genRand.Next(1, 4);
                    }
                }
                if (num23 < num4)
                {
                    num24 += WorldGen.genRand.Next(0, 4);
                    int num25 = WorldGen.genRand.Next(2, 5);
                    int num26 = (isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel"));
                    for (int num27 = num23 - num25; num27 <= num23 + num25; num27++)
                    {
                        for (int num28 = num24 - num25; num28 <= num24 + num25; num28++)
                        {
                            if (num28 > num5)
                            {
                                float arg_890_0 = (float)Math.Abs(num27 - num23);
                                float num29 = (float)(Math.Abs(num28 - num24) * 2);
                                if (Math.Sqrt((double)(arg_890_0 * arg_890_0 + num29 * num29)) < (double)num25)
                                {
                                    Main.tile[num27, num28].type = (ushort)num26;
                                    SafeSquareTileFrame(num27, num28, true);
                                }
                            }
                        }
                    }
                    num23 += WorldGen.genRand.Next(num25, (int)((double)num25 * 1.5));
                }
            }
            for (int num30 = num3 - 20; num30 <= num4 + 20; num30++)
            {
                for (int num31 = num5 - 20; num31 <= num6 + 20; num31++)
                {
                    bool flag = true;
                    for (int num32 = num30 - 1; num32 <= num30 + 1; num32++)
                    {
                        for (int num33 = num31 - 1; num33 <= num31 + 1; num33++)
                        {
                            if (!Main.tile[num32, num33].active())
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[num30, num31].wall = (byte)(isVoid ? mod.WallType("VoidstoneWallUnsafe") : mod.WallType("AbyssGravelWall"));
                        WorldGen.SquareWallFrame(num30, num31, true);
                    }
                }
            }
            for (int num34 = num3; num34 <= num4; num34++)
            {
                int num35 = num5 - 10;
                while (!Main.tile[num34, num35 + 1].active())
                {
                    num35++;
                }
                if (num35 < num6 && Main.tile[num34, num35 + 1].type == (ushort)(isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel")))
                {
                    if (WorldGen.genRand.Next(10) == 0)
                    {
                        int num36 = WorldGen.genRand.Next(1, 3);
                        for (int num37 = num34 - num36; num37 <= num34 + num36; num37++)
                        {
                            if (Main.tile[num37, num35].type == (ushort)(isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel")))
                            {
                                Main.tile[num37, num35].active(false);
                                Main.tile[num37, num35].liquid = 255;
                                Main.tile[num37, num35].lava(false);
                                SafeSquareTileFrame(num34, num35, true);
                            }
                            if (Main.tile[num37, num35 + 1].type == (ushort)(isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel")))
                            {
                                Main.tile[num37, num35 + 1].active(false);
                                Main.tile[num37, num35 + 1].liquid = 255;
                                Main.tile[num37, num35 + 1].lava(false);
                                SafeSquareTileFrame(num34, num35 + 1, true);
                            }
                            if (num37 > num34 - num36 && num37 < num34 + 2 && Main.tile[num37, num35 + 2].type == (ushort)(isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel")))
                            {
                                Main.tile[num37, num35 + 2].active(false);
                                Main.tile[num37, num35 + 2].liquid = 255;
                                Main.tile[num37, num35 + 2].lava(false);
                                SafeSquareTileFrame(num34, num35 + 2, true);
                            }
                        }
                    }
                    if (WorldGen.genRand.Next(5) == 0)
                    {
                        Main.tile[num34, num35].liquid = 255;
                    }
                    Main.tile[num34, num35].lava(false);
                    SafeSquareTileFrame(num34, num35, true);
                }
            }
        }
        #endregion

        #region AbyssIslandHouse
        public static void AbyssIslandHouse(int i, int j, int itemChoice, bool isVoid)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            ushort type = (ushort)(isVoid ? mod.TileType("Voidstone") : mod.TileType("AbyssGravel")); //tile
            byte wall = (byte)(isVoid ? mod.WallType("VoidstoneWallUnsafe") : mod.WallType("AbyssGravelWall")); //wall
            Vector2 vector = new Vector2((float)i, (float)j);
            int num = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num = -1;
            }
            int num2 = WorldGen.genRand.Next(5, 9);
            int num3 = 3;
            vector.X = (float)(i + (num2 + 2) * num);
            for (int k = j - 15; k < j + 30; k++)
            {
                if (Main.tile[(int)vector.X, k].active())
                {
                    vector.Y = (float)(k - 1);
                    break;
                }
            }
            vector.X = (float)i;
            int num4 = (int)(vector.X - (float)num2 - 1f);
            int num5 = (int)(vector.X + (float)num2 + 1f);
            int num6 = (int)(vector.Y - (float)num3 - 1f);
            int num7 = (int)(vector.Y + 2f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int l = num4; l <= num5; l++)
            {
                for (int m = num6 - 1; m < num7 + 1; m++)
                {
                    if (m != num6 - 1 || (l != num4 && l != num5))
                    {
                        Main.tile[l, m].active(true);
                        Main.tile[l, m].type = type;
                        Main.tile[l, m].wall = wall;
                        Main.tile[l, m].halfBrick(false);
                        Main.tile[l, m].slope(0);
                    }
                }
            }
            num4 = (int)(vector.X - (float)num2);
            num5 = (int)(vector.X + (float)num2);
            num6 = (int)(vector.Y - (float)num3);
            num7 = (int)(vector.Y + 1f);
            if (num4 < 0)
            {
                num4 = 0;
            }
            if (num5 > Main.maxTilesX)
            {
                num5 = Main.maxTilesX;
            }
            if (num6 < 0)
            {
                num6 = 0;
            }
            if (num7 > Main.maxTilesY)
            {
                num7 = Main.maxTilesY;
            }
            for (int n = num4; n <= num5; n++)
            {
                for (int num8 = num6; num8 < num7; num8++)
                {
                    if ((num8 != num6 || (n != num4 && n != num5)) && Main.tile[n, num8].wall == wall)
                    {
                        Main.tile[n, num8].active(false);
                    }
                }
            }
            int num9 = i + (num2 + 1) * num;
            int num10 = (int)vector.Y;
            for (int num11 = num9 - 2; num11 <= num9 + 2; num11++)
            {
                Main.tile[num11, num10].active(false);
                Main.tile[num11, num10 - 1].active(false);
                Main.tile[num11, num10 - 2].active(false);
            }
            switch (itemChoice)
            {
                case 0: itemChoice = mod.ItemType("TorrentialTear"); break; //rain item
                case 1: itemChoice = mod.ItemType("IronBoots"); break; //movement acc
                case 2: itemChoice = mod.ItemType("DepthCharm"); break; //regen acc
                case 3: itemChoice = mod.ItemType("Archerfish"); break; //ranged
                case 4: itemChoice = mod.ItemType("AnechoicPlating"); break; //defense acc
                case 5: itemChoice = mod.ItemType("BallOFugu"); break; //melee
                case 6: itemChoice = mod.ItemType("StrangeOrb"); break; //light pet
                case 7: itemChoice = mod.ItemType("HerringStaff"); break; //summon
                case 8: itemChoice = mod.ItemType("BlackAnurian"); break; //magic
                case 9: itemChoice = mod.ItemType("Lionfish"); break; //throwing
                default: itemChoice = 497; break;
            }
            WorldGen.AddBuriedChest(i, num10 - 3, itemChoice, false, 4); //chest
        }
        #endregion

        #region IcePyramid
        public static bool IcePyramid(int i, int j)
        {
            ushort num = 161;
            int arg_36_0 = j - WorldGen.genRand.Next(0, 7);
            int num2 = WorldGen.genRand.Next(9, 13);
            int num3 = 1;
            int num4 = j + WorldGen.genRand.Next(75, 125); //75 125
            for (int k = arg_36_0; k < num4; k++)
            {
                for (int l = i - num3; l < i + num3 - 1; l++)
                {
                    Main.tile[l, k].type = num;
                    Main.tile[l, k].active(true);
                    Main.tile[l, k].halfBrick(false);
                    Main.tile[l, k].slope(0);
                }
                num3++;
            }
            for (int m = i - num3 - 5; m <= i + num3 + 5; m++)
            {
                for (int n = j - 1; n <= num4 + 1; n++)
                {
                    bool flag = true;
                    for (int num5 = m - 1; num5 <= m + 1; num5++)
                    {
                        for (int num6 = n - 1; num6 <= n + 1; num6++)
                        {
                            if (Main.tile[num5, num6].type != num)
                            {
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        Main.tile[m, n].wall = 71;
                        WorldGen.SquareWallFrame(m, n, true);
                    }
                }
            }
            int num7 = 1;
            if (WorldGen.genRand.Next(2) == 0)
            {
                num7 = -1;
            }
            int num8 = i - num2 * num7;
            int num9 = j + num2;
            int num10 = WorldGen.genRand.Next(5, 8);
            bool flag2 = true;
            int num11 = WorldGen.genRand.Next(20, 30);
            while (flag2)
            {
                flag2 = false;
                bool flag3 = false;
                for (int num12 = num9; num12 <= num9 + num10; num12++)
                {
                    int num13 = num8;
                    if (Main.tile[num13, num12 - 1].type == 161)
                    {
                        flag3 = true;
                    }
                    if (Main.tile[num13, num12].type == num)
                    {
                        Main.tile[num13, num12 + 1].wall = 71;
                        Main.tile[num13 + num7, num12].wall = 71;
                        Main.tile[num13, num12].active(false);
                        flag2 = true;
                    }
                    if (flag3)
                    {
                        Main.tile[num13, num12].type = 161;
                        Main.tile[num13, num12].active(true);
                        Main.tile[num13, num12].halfBrick(false);
                        Main.tile[num13, num12].slope(0);
                    }
                }
                num8 -= num7;
            }
            num8 = i - num2 * num7;
            bool flag4 = true;
            bool flag5 = false;
            flag2 = true;
            while (flag2)
            {
                for (int num14 = num9; num14 <= num9 + num10; num14++)
                {
                    int num15 = num8;
                    Main.tile[num15, num14].active(false);
                }
                num8 += num7;
                num9++;
                num11--;
                if (num9 >= num4 - num10 * 2)
                {
                    num11 = 10;
                }
                if (num11 <= 0)
                {
                    bool flag6 = false;
                    if (!flag4 && !flag5)
                    {
                        flag5 = true;
                        flag6 = true;
                        int num16 = WorldGen.genRand.Next(7, 13);
                        int num17 = WorldGen.genRand.Next(23, 28);
                        int num18 = num17;
                        int num19 = num8;
                        while (num17 > 0)
                        {
                            for (int num20 = num9 - num16 + num10; num20 <= num9 + num10; num20++)
                            {
                                if (num17 == num18 || num17 == 1)
                                {
                                    if (num20 >= num9 - num16 + num10 + 2)
                                    {
                                        Main.tile[num8, num20].active(false);
                                    }
                                }
                                else if (num17 == num18 - 1 || num17 == 2 || num17 == num18 - 2 || num17 == 3)
                                {
                                    if (num20 >= num9 - num16 + num10 + 1)
                                    {
                                        Main.tile[num8, num20].active(false);
                                    }
                                }
                                else
                                {
                                    Main.tile[num8, num20].active(false);
                                }
                            }
                            num17--;
                            num8 += num7;
                        }
                        int num21 = num8 - num7;
                        int num22 = num21;
                        int num23 = num19;
                        if (num21 > num19)
                        {
                            num22 = num19;
                            num23 = num21;
                        }
                        int num24 = WorldGen.genRand.Next(3);
                        if (num24 == 0)
                        {
                            num24 = 1861; //diving gear
                        }
                        else if (num24 == 1)
                        {
                            num24 = 1163; //balloon
                        }
                        else if (num24 == 2)
                        {
                            num24 = 1253; //shell
                        }
                        WorldGen.AddBuriedChest((num22 + num23) / 2, num9, num24, false, 22);
                        int num25 = WorldGen.genRand.Next(1, 10);
                        for (int num26 = 0; num26 < num25; num26++)
                        {
                            int arg_49B_0 = WorldGen.genRand.Next(num22, num23);
                            int j2 = num9 + num10;
                            WorldGen.PlaceSmallPile(arg_49B_0, j2, WorldGen.genRand.Next(16, 19), 1, 185);
                        }
                        for (int num27 = num22; num27 <= num23; num27++)
                        {
                            WorldGen.PlacePot(num27, num9 + num10, 28, WorldGen.genRand.Next(4, 7));
                        }
                    }
                    if (flag4)
                    {
                        flag4 = false;
                        num7 *= -1;
                        num11 = WorldGen.genRand.Next(15, 20);
                    }
                    else if (flag6)
                    {
                        num11 = WorldGen.genRand.Next(10, 15);
                    }
                    else
                    {
                        num7 *= -1;
                        num11 = WorldGen.genRand.Next(20, 40);
                    }
                }
                if (num9 >= num4 - num10)
                {
                    flag2 = false;
                }
            }
            int num28 = WorldGen.genRand.Next(20, 40); //100 200
            int num29 = WorldGen.genRand.Next(100, 160); //500 800
            flag2 = true;
            int num30 = num10;
            num11 = WorldGen.genRand.Next(10, 50);
            if (num7 == 1)
            {
                num8 -= num30;
            }
            int num31 = WorldGen.genRand.Next(5, 10);
            while (flag2)
            {
                num28--;
                num29--;
                num11--;
                for (int num32 = num8 - num31 - WorldGen.genRand.Next(0, 2); num32 <= num8 + num30 + num31 + WorldGen.genRand.Next(0, 2); num32++)
                {
                    int num33 = num9;
                    if (num32 >= num8 && num32 <= num8 + num30)
                    {
                        Main.tile[num32, num33].active(false);
                    }
                    else
                    {
                        Main.tile[num32, num33].type = num;
                        Main.tile[num32, num33].active(true);
                        Main.tile[num32, num33].halfBrick(false);
                        Main.tile[num32, num33].slope(0);
                    }
                    if (num32 >= num8 - 1 && num32 <= num8 + 1 + num30)
                    {
                        Main.tile[num32, num33].wall = 71;
                    }
                }
                num9++;
                num8 += num7;
                if (num28 <= 0)
                {
                    flag2 = false;
                    for (int num34 = num8 + 1; num34 <= num8 + num30 - 1; num34++)
                    {
                        if (Main.tile[num34, num9].active())
                        {
                            flag2 = true;
                        }
                    }
                }
                if (num11 < 0)
                {
                    num11 = WorldGen.genRand.Next(10, 50);
                    num7 *= -1;
                }
                if (num29 <= 0)
                {
                    flag2 = false;
                }
            }
            return true;
        }
        #endregion

        #region SpecialHut
        public static void SpecialHut(ushort tile, ushort tile2, byte wall, int hutType, int chestX, int chestY)
        {
            int m = WorldGen.genRand.Next(2, 4);
            int num6 = WorldGen.genRand.Next(2, 4);
            for (int n = chestX - m - 1; n <= chestX + m + 1; n++)
            {
                for (int num8 = chestY - num6 - 1; num8 <= chestY + num6 + 1; num8++)
                {
                    Main.tile[n, num8].active(true);
                    Main.tile[n, num8].type = tile; //change
                    Main.tile[n, num8].liquid = 0;
                    Main.tile[n, num8].lava(false);
                }
            }
            for (int num9 = chestX - m; num9 <= chestX + m; num9++)
            {
                for (int num10 = chestY - num6; num10 <= chestY + num6; num10++)
                {
                    Main.tile[num9, num10].active(false);
                    Main.tile[num9, num10].wall = wall; //change
                }
            }
            for (int num14 = chestX - m - 1; num14 <= chestX + m + 1; num14++)
            {
                for (int num15 = chestY + num6 - 2; num15 <= chestY + num6; num15++)
                {
                    Main.tile[num14, num15].active(false);
                }
            }
            for (int num16 = chestX - m - 1; num16 <= chestX + m + 1; num16++)
            {
                for (int num17 = chestY + num6 - 2; num17 <= chestY + num6 - 1; num17++)
                {
                    Main.tile[num16, num17].active(false);
                }
            }
            for (int num18 = chestX - m - 1; num18 <= chestX + m + 1; num18++)
            {
                int num19 = 4;
                int num20 = chestY + num6 + 2;
                while (!Main.tile[num18, num20].active() && num20 < Main.maxTilesY && num19 > 0)
                {
                    Main.tile[num18, num20].active(true);
                    Main.tile[num18, num20].type = tile2; //change
                    num20++;
                    num19--;
                }
            }
            m -= WorldGen.genRand.Next(1, 3);
            int num21 = chestY - num6 - 2;
            while (m > -1)
            {
                for (int num22 = chestX - m - 1; num22 <= chestX + m + 1; num22++)
                {
                    Main.tile[num22, num21].active(true);
                    Main.tile[num22, num21].type = tile; //change
                }
                m -= WorldGen.genRand.Next(1, 3);
                num21--;
            }
            CalamityWorld.SChestX[hutType] = chestX;
            CalamityWorld.SChestY[hutType] = chestY;
            SpecialChest(hutType);
        }
        #endregion

        #region SpecialChest
        public static void SpecialChest(int itemChoice)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            int item = 0;
            int chestType = 0;
            switch (itemChoice) //0 to 7
            {
                case 0: item = 976; break; //climbing gear
                case 1: item = (WorldGen.crimson ? 3016 : 3015); chestType = (WorldGen.crimson ? 43 : 3); break; //flesh knuckles or putrid scent
                case 2: item = 908; chestType = 44; break; //lava waders
                case 3: item = 1595; chestType = 47; break; //cuffs
                case 4: item = 899; chestType = 30; break; //sun stone
                case 5: item = 1864; chestType = 32; break; //scarab
                case 6: item = 532; chestType = 50; break; //fuck you, star cloak
                case 7: item = 554; chestType = 51; break; //fuck you, cross necklace
                case 8: item = mod.ItemType("Murasama"); chestType = 44; break;
            }
            for (int j = CalamityWorld.SChestX[itemChoice] - 1; j <= CalamityWorld.SChestX[itemChoice] + 1; j++)
            {
                for (int k = CalamityWorld.SChestY[itemChoice]; k <= CalamityWorld.SChestY[itemChoice] + 2; k++)
                {
                    WorldGen.KillTile(j, k, false, false, false);
                }
            }
            for (int l = CalamityWorld.SChestX[itemChoice] - 1; l <= CalamityWorld.SChestX[itemChoice] + 1; l++)
            {
                for (int m = CalamityWorld.SChestY[itemChoice]; m <= CalamityWorld.SChestY[itemChoice] + 3; m++)
                {
                    if (m < Main.maxTilesY)
                    {
                        Main.tile[l, m].slope(0);
                        Main.tile[l, m].halfBrick(false);
                    }
                }
            }
            WorldGen.AddBuriedChest(CalamityWorld.SChestX[itemChoice], CalamityWorld.SChestY[itemChoice], item, false, chestType);
        }
        #endregion

        #region ChasmGenerator
        public static void ChasmGenerator(int i, int j, int steps, bool ocean = false)
        {
            Mod mod = ModLoader.GetMod("CalamityMod");
            float num = (float)steps; //y / 4
            if (ocean)
            {
                int tileYLookup = j;
                if (abyssSide)
                {
                    while (!Main.tile[i + 125, tileYLookup].active())
                    {
                        tileYLookup++;
                    }
                }
                else
                {
                    while (!Main.tile[i - 125, tileYLookup].active())
                    {
                        tileYLookup++;
                    }
                }
                j = tileYLookup;
            }
            Vector2 vector;
            vector.X = (float)i;
            vector.Y = (float)j;
            Vector2 vector2;
            vector2.X = (float)WorldGen.genRand.Next(-1, 2) * 0.1f;
            vector2.Y = (float)WorldGen.genRand.Next(3, 8) * 0.2f + 0.5f;
            int num2 = 5;
            double num3 = (double)(WorldGen.genRand.Next(5, 7) + 40); //start width
            while (num3 > 0.0)
            {
                if (num > 0f)
                {
                    num3 += (double)WorldGen.genRand.Next(2);
                    num3 -= (double)WorldGen.genRand.Next(2);
                    if (ocean && num > (float)((double)(Main.maxTilesY / 4) * 0.75)) //dig small hole
                    {
                        if (num3 < 4.0) //min width
                        {
                            num3 = 4.0; //min width
                        }
                        if (num3 > 6.0) //max width
                        {
                            num3 = 6.0; //max width
                        }
                    }
                    else //dig large hole
                    {
                        if (num3 < (ocean ? 42.0 : 14.0)) //min width
                        {
                            num3 = (ocean ? 42.0 : 14.0); //min width
                        }
                        if (num3 > (ocean ? 60.0 : 40.0)) //max width
                        {
                            num3 = (ocean ? 60.0 : 40.0); //max width
                        }
                        if (num == 1f && num3 < (ocean ? 50.0 : 20.0))
                        {
                            num3 = (ocean ? 50.0 : 20.0);
                        }
                    }
                }
                else if ((double)vector.Y > (Main.rockLayer + (double)Main.maxTilesY * 0.312)) //.362
                {
                    num3 -= (double)(WorldGen.genRand.Next(5) + 8); //4
                }
                if (((double)vector.Y > (Main.rockLayer + (double)Main.maxTilesY * 0.3) && num > 0f && ocean) ||
                    ((double)vector.Y >= (double)Main.maxTilesY && num > 0f && !ocean))
                {
                    num = 0f;
                }
                num -= 1f;
                int num4;
                int num5;
                int num6;
                int num7;
                if (num > (float)num2)
                {
                    num4 = (int)((double)vector.X - num3 * 0.5);
                    num5 = (int)((double)vector.X + num3 * 0.5);
                    num6 = (int)((double)vector.Y - num3 * 0.5);
                    num7 = (int)((double)vector.Y + num3 * 0.5);
                    if (num4 < 0)
                    {
                        num4 = 0;
                    }
                    if (num5 > Main.maxTilesX - 1)
                    {
                        num5 = Main.maxTilesX - 1;
                    }
                    if (num6 < 0)
                    {
                        num6 = 0;
                    }
                    if (num7 > Main.maxTilesY)
                    {
                        num7 = Main.maxTilesY;
                    }
                    for (int k = num4; k < num5; k++)
                    {
                        for (int l = num6; l < num7; l++)
                        {
                            if ((double)(Math.Abs((float)k - vector.X) + Math.Abs((float)l - vector.Y)) < num3 * 0.5 * (1.0 + (double)WorldGen.genRand.Next(-5, 6) * 0.015))
                            {
                                if (ocean)
                                {
                                    Main.tile[k, l].active(false);
                                    Main.tile[k, l].liquid = 255;
                                    Main.tile[k, l].lava(false);
                                }
                                else
                                {
                                    Main.tile[k, l].active(false);
                                    Main.tile[k, l].liquid = 255;
                                    Main.tile[k, l].lava(true);
                                }
                            }
                        }
                    }
                }
                if (num <= 2f && (double)vector.Y < (Main.rockLayer + (double)Main.maxTilesY * 0.312)) //1260
                {
                    num = 2f;
                }
                vector += vector2;
                vector2.X += (float)WorldGen.genRand.Next(-1, 2) * 0.01f;
                if ((double)vector2.X > 0.02)
                {
                    vector2.X = 0.02f;
                }
                if ((double)vector2.X < -0.02)
                {
                    vector2.X = -0.02f;
                }
                num4 = (int)((double)vector.X - num3 * 1.1);
                num5 = (int)((double)vector.X + num3 * 1.1);
                num6 = (int)((double)vector.Y - num3 * 1.1);
                num7 = (int)((double)vector.Y + num3 * 1.1);
                if (num4 < 1)
                {
                    num4 = 1;
                }
                if (num5 > Main.maxTilesX - 1)
                {
                    num5 = Main.maxTilesX - 1;
                }
                if (num6 < 0)
                {
                    num6 = 0;
                }
                if (num7 > Main.maxTilesY)
                {
                    num7 = Main.maxTilesY;
                }
                for (int m = num4; m < num5; m++)
                {
                    for (int n = num6; n < num7; n++)
                    {
                        if ((double)(Math.Abs((float)m - vector.X) + Math.Abs((float)n - vector.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (n > j + WorldGen.genRand.Next(7, 16))
                            {
                                Main.tile[m, n].active(false);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[m, n].active(false);
                            }
                            if (ocean)
                            {
                                Main.tile[m, n].liquid = 255;
                                Main.tile[m, n].lava(false);
                            }
                            else
                            {
                                Main.tile[m, n].liquid = 255;
                                Main.tile[m, n].lava(true);
                            }
                        }
                    }
                }
                for (int num11 = num4; num11 < num5; num11++)
                {
                    for (int num12 = num6; num12 < num7; num12++)
                    {
                        if ((double)(Math.Abs((float)num11 - vector.X) + Math.Abs((float)num12 - vector.Y)) < num3 * 1.1 * (1.0 + (double)WorldGen.genRand.Next(-5, 6) * 0.015))
                        {
                            if (ocean)
                            {
                                Main.tile[num11, num12].liquid = 255;
                                Main.tile[num11, num12].lava(false);
                            }
                            else
                            {
                                Main.tile[num11, num12].liquid = 255;
                                Main.tile[num11, num12].lava(true);
                            }
                            if (steps <= num2)
                            {
                                Main.tile[num11, num12].active(false);
                            }
                        }
                    }
                }
            }
        }
        #endregion

        #region PlanetoidMethods
        //WORLD GENERATION PASSES
        private static void ExpandWorld(GenerationProgress progress)
        {
            int newHeight = Main.maxTilesY + ExpandWorldBy;

            //---------------------
            //MISC. FIXES
            //---------------------

            //TILES
            FieldInfo worldSizeY = typeof(WorldFileData).GetField("WorldSizeY", BindingFlags.Instance | BindingFlags.Public);
            worldSizeY.SetValue(Main.ActiveWorldFileData, newHeight);

            FieldInfo lastMaxTilesY = typeof(WorldGen).GetField("lastMaxTilesY", BindingFlags.Static | BindingFlags.NonPublic);
            lastMaxTilesY.SetValue(null, newHeight);

            Main.maxTilesY = newHeight;

            Main.bottomWorld = newHeight * 16;
            Main.maxSectionsY = Main.maxTilesY / 150;
            //

            //MAP
            MethodInfo methodInfo = typeof(Main).GetMethod("InitMap", BindingFlags.NonPublic | BindingFlags.Instance);
            methodInfo.Invoke(Main.instance, null);
            //

            //MOVING THE TILES DOWN
            Tile[,] newTiles = new Tile[8401, 2401 + ExpandWorldBy];
            for (int x = 0; x < Main.maxTilesX; x++)
            {
                for (int y = 0; y < Main.maxTilesY; y++)
                {
                    newTiles[x, y] = new Tile();
                    if (y >= ExpandWorldBy)
                    {
                        newTiles[x, y].CopyFrom(Main.tile[x, y - ExpandWorldBy]);
                        Main.tile[x, y - ExpandWorldBy] = null;
                    }
                }
            }

            Main.tile = newTiles;

            for (int i = 0; i < Main.chest.Length; i++)
            {
                if (Main.chest[i] != null)
                    Main.chest[i].y += ExpandWorldBy;
            }
            //

            //FIXING VARIABLES
            Main.worldSurface += ExpandWorldBy;
            Main.rockLayer += ExpandWorldBy;
            WorldGen.worldSurface += ExpandWorldBy;
            WorldGen.worldSurfaceHigh += ExpandWorldBy;
            WorldGen.worldSurfaceLow += ExpandWorldBy;
            WorldGen.rockLayer += ExpandWorldBy;
            WorldGen.rockLayerHigh += ExpandWorldBy;
            WorldGen.rockLayerLow += ExpandWorldBy;
            Main.spawnTileY += ExpandWorldBy;
            Main.dungeonY += ExpandWorldBy;
            //

            //FIXING THE GUIDE'S CURRENT POSITION
            for (int i = 0; i < Main.npc.Length; i++)
            {
                if (Main.npc[i].type == NPCID.Guide)
                {
                    Main.npc[i].homeTileY = Main.spawnTileY;
                    Main.npc[i].position.Y += ExpandWorldBy * 16f;
                    break;
                }
            }
            //
        }

        private static void Planetoids(GenerationProgress progress)
        {
            progress.Message = "Generating Planetoids...";

            int GrassPlanetoidCount = Main.maxTilesX / 1100;
            int LCPlanetoidCount = Main.maxTilesX / 800;
            int MudPlanetoidCount = Main.maxTilesX / 1100;

            //It might seem scary but fuck making an extra boolean
            while (true)
            {
                if (Biomes<MainPlanet>.Place(new Point(WorldGen.genRand.Next(Main.maxTilesX / 2 - 100, Main.maxTilesX / 2 + 100), WorldGen.genRand.Next(128, 134)), WorldGen.structures))
                {
                    break;
                }
            }
            while (LCPlanetoidCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.2), (int)(Main.maxTilesX * 0.8));
                int y = WorldGen.genRand.Next(70, ExpandWorldBy);

                bool placed = Biomes<HeartPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    LCPlanetoidCount--;
            }
            while (GrassPlanetoidCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.333), (int)(Main.maxTilesX * 0.666));
                int y = WorldGen.genRand.Next(100, ExpandWorldBy);


                bool placed = Biomes<GrassPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    GrassPlanetoidCount--;
            }
            while (MudPlanetoidCount > 0)
            {
                int x = WorldGen.genRand.Next((int)(Main.maxTilesX * 0.3f), (int)(Main.maxTilesX * 0.7f));
                int y = WorldGen.genRand.Next(100, ExpandWorldBy);

                bool placed = Biomes<MudPlanet>.Place(x, y, WorldGen.structures);

                if (placed)
                    MudPlanetoidCount--;
            }
        }

        private static void PotFix(GenerationProgress progress)
        {
            Main.tileSolid[137] = true;
            Main.tileSolid[130] = true;
            progress.Message = Lang.gen[35].Value;
            for (int i = 0; i < (int)((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008); i++)
            {
                float num = (float)((double)i / ((double)(Main.maxTilesX * Main.maxTilesY) * 0.0008));
                progress.Set(num);
                bool flag = false;
                int num2 = 0;
                while (!flag)
                {
                    int num3 = WorldGen.genRand.Next((int)WorldGen.worldSurfaceHigh, Main.maxTilesY - 10);
                    if ((double)num > 0.93)
                    {
                        num3 = Main.maxTilesY - 150;
                    }
                    else if ((double)num > 0.75)
                    {
                        num3 = (int)WorldGen.worldSurfaceLow;
                    }
                    int num4 = WorldGen.genRand.Next(1, Main.maxTilesX - 1);
                    bool flag2 = false;
                    for (int j = num3; j < Main.maxTilesY; j++)
                    {
                        if (!flag2)
                        {
                            if (Main.tile[num4, j].active() && Main.tileSolid[(int)Main.tile[num4, j].type] && !Main.tile[num4, j - 1].lava())
                            {
                                flag2 = true;
                            }
                        }
                        else
                        {
                            int style = WorldGen.genRand.Next(0, 4);
                            int num5 = 0;
                            if (j < Main.maxTilesY - 5)
                            {
                                num5 = (int)Main.tile[num4, j + 1].type;
                            }
                            if (num5 == 147 || num5 == 161 || num5 == 162)
                            {
                                style = WorldGen.genRand.Next(4, 7);
                            }
                            if (num5 == 60)
                            {
                                style = WorldGen.genRand.Next(7, 10);
                            }
                            if (Main.wallDungeon[(int)Main.tile[num4, j].wall])
                            {
                                style = WorldGen.genRand.Next(10, 13);
                            }
                            if (num5 == 41 || num5 == 43 || num5 == 44)
                            {
                                style = WorldGen.genRand.Next(10, 13);
                            }
                            if (num5 == 22 || num5 == 23 || num5 == 25)
                            {
                                style = WorldGen.genRand.Next(16, 19);
                            }
                            if (num5 == 199 || num5 == 203 || num5 == 204 || num5 == 200)
                            {
                                style = WorldGen.genRand.Next(22, 25);
                            }
                            if (num5 == 367)
                            {
                                style = WorldGen.genRand.Next(31, 34);
                            }
                            if (num5 == 226)
                            {
                                style = WorldGen.genRand.Next(28, 31);
                            }
                            if (j > Main.maxTilesY - 200)
                            {
                                style = WorldGen.genRand.Next(13, 16);
                            }
                            if (WorldGen.PlacePot(num4, j, 28, style))
                            {
                                flag = true;
                                break;
                            }
                            num2++;
                            if (num2 >= 10000)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private static void RemoveMostTasks(List<GenPass> tasks)
        {
            int value = 15;
            while (tasks.Count > value)
            {
                if (tasks[value].Name == "Spawn Point")
                {
                    value++;
                }
                tasks.RemoveAt(value);
            }
        }
        #endregion

        #region PostUpdate
        public override void PostUpdate()
		{
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
            if (CalamityGlobalNPC.holyBoss >= 0 && !Main.npc[CalamityGlobalNPC.holyBoss].active)
            {
                CalamityGlobalNPC.holyBoss = -1;
                if (Main.netMode == 2)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.Providence);
                    netMessage.Write(CalamityGlobalNPC.holyBoss);
                    netMessage.Send();
                }
            }
            if (CalamityGlobalNPC.doughnutBoss >= 0 && !Main.npc[CalamityGlobalNPC.doughnutBoss].active)
            {
                CalamityGlobalNPC.doughnutBoss = -1;
            }
            if (CalamityGlobalNPC.voidBoss >= 0 && !Main.npc[CalamityGlobalNPC.voidBoss].active)
            {
                CalamityGlobalNPC.voidBoss = -1;
            }
            if (CalamityGlobalNPC.energyFlame >= 0 && !Main.npc[CalamityGlobalNPC.energyFlame].active)
            {
                CalamityGlobalNPC.energyFlame = -1;
            }
            if (CalamityGlobalNPC.hiveMind >= 0 && !Main.npc[CalamityGlobalNPC.hiveMind].active)
            {
                CalamityGlobalNPC.hiveMind = -1;
            }
            if (CalamityGlobalNPC.hiveMind2 >= 0 && !Main.npc[CalamityGlobalNPC.hiveMind2].active)
            {
                CalamityGlobalNPC.hiveMind2 = -1;
            }
            if (CalamityGlobalNPC.scavenger >= 0 && !Main.npc[CalamityGlobalNPC.scavenger].active)
            {
                CalamityGlobalNPC.scavenger = -1;
                if (Main.netMode == 2)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.Ravager);
                    netMessage.Write(CalamityGlobalNPC.scavenger);
                    netMessage.Send();
                }
            }
            if (CalamityGlobalNPC.DoGHead >= 0 && !Main.npc[CalamityGlobalNPC.DoGHead].active)
            {
                CalamityGlobalNPC.DoGHead = -1;
                if (Main.netMode == 2)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoG);
                    netMessage.Write(CalamityGlobalNPC.DoGHead);
                    netMessage.Send();
                }
            }
            if (CalamityGlobalNPC.ghostBoss >= 0 && !Main.npc[CalamityGlobalNPC.ghostBoss].active)
            {
                CalamityGlobalNPC.ghostBoss = -1;
                if (Main.netMode == 2)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.Polterghast);
                    netMessage.Write(CalamityGlobalNPC.ghostBoss);
                    netMessage.Send();
                }
            }
            if (CalamityGlobalNPC.laserEye >= 0 && !Main.npc[CalamityGlobalNPC.laserEye].active)
            {
                CalamityGlobalNPC.laserEye = -1;
            }
            if (CalamityGlobalNPC.fireEye >= 0 && !Main.npc[CalamityGlobalNPC.fireEye].active)
            {
                CalamityGlobalNPC.fireEye = -1;
            }
            if (CalamityGlobalNPC.lordeBoss >= 0 && !Main.npc[CalamityGlobalNPC.lordeBoss].active)
            {
                CalamityGlobalNPC.lordeBoss = -1;
                if (Main.netMode == 2)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.LORDE);
                    netMessage.Write(CalamityGlobalNPC.lordeBoss);
                    netMessage.Send();
                }
            }
            #region SpawnDoG
            if (CalamityGlobalNPC.DoGSecondStageCountdown > 0)
            {
                CalamityGlobalNPC.DoGSecondStageCountdown--;
                if (Main.netMode != 1)
                {
                    if (CalamityGlobalNPC.DoGSecondStageCountdown == 21540)
                    {
                        NPC.SpawnOnPlayer((int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0), mod.NPCType("CeaselessVoid"));
                    }
                    if (CalamityGlobalNPC.DoGSecondStageCountdown == 14340)
                    {
                        NPC.SpawnOnPlayer((int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0), mod.NPCType("StormWeaverHead"));
                    }
                    if (CalamityGlobalNPC.DoGSecondStageCountdown == 7140)
                    {
                        NPC.SpawnOnPlayer((int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0), mod.NPCType("CosmicWraith"));
                    }
                    if (CalamityGlobalNPC.DoGSecondStageCountdown <= 60)
                    {
                        if (!NPC.AnyNPCs(mod.NPCType("DevourerofGodsHeadS")))
                        {
                            NPC.SpawnOnPlayer((int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0), mod.NPCType("DevourerofGodsHeadS"));
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
    }
}
