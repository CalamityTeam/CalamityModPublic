using CalamityMod.Buffs.StatDebuffs;
using CalamityMod.CalPlayer;
using CalamityMod.NPCs;
using CalamityMod.NPCs.Abyss;
using CalamityMod.NPCs.AquaticScourge;
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
using CalamityMod.NPCs.SupremeCalamitas;
using CalamityMod.NPCs.Yharon;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.World.Generation;

namespace CalamityMod.World
{
    public class CalamityWorld : ModWorld
    {
        #region Vars
        public static int DoGSecondStageCountdown = 0;
        public static bool dragonScalesBought = false;
        private const int saveVersion = 0;

        //Boss Rush
        public static bool bossRushActive = false; //Whether Boss Rush is active or not
        public static bool deactivateStupidFuckingBullshit = false; //Force Boss Rush to inactive
        public static int bossRushStage = 0; //Boss Rush Stage
        public static int bossRushSpawnCountdown = 180; //Delay before another Boss Rush boss can spawn

        //Death Mode natural boss spawns
        public static int bossSpawnCountdown = 0; //Death Mode natural boss spawn countdown
        public static int bossType = 0; //Death Mode natural boss spawn type

        //Modes
        public static bool demonMode = false; //Spawn rate boost
        public static bool onionMode = false; //Extra accessory from Moon Lord
        public static bool revenge = false; //Revengeance Mode
        public static bool death = false; //Death Mode
        public static bool defiled = false; //Defiled Mode
        public static bool armageddon = false; //Armageddon Mode
        public static bool ironHeart = false; //Iron Heart Mode

		// New Temple Altar
		public static int newAltarX = 0;
		public static int newAltarY = 0;

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

        //Sunken Sea
        public static int sunkenSeaTiles = 0;
        public static Rectangle SunkenSeaLocation = Rectangle.Empty;

        //Shrines
        public static int[] SChestX = new int[10];
        public static int[] SChestY = new int[10];
        public static bool roxShrinePlaced = false;

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
        public static bool downedSentinel1 = false; // Ceaseless Void
        public static bool downedSentinel2 = false; // Storm Weaver
        public static bool downedSentinel3 = false; // Signus, Envoy of the Devourer
        public static bool downedPolterghast = false;
        public static bool downedDoG = false;
        public static bool downedBumble = false;
        public static bool buffedEclipse = false;
        public static bool downedBuffedMothron = false;
        public static bool downedYharon = false;
        public static bool downedSCal = false;
        public static bool downedLORDE = false;
        public static bool downedCLAM = false;
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
            CalamityGlobalNPC.scavenger = -1;
            CalamityGlobalNPC.bobbitWormBottom = -1;
            CalamityGlobalNPC.DoGHead = -1;
            CalamityGlobalNPC.SCal = -1;
            CalamityGlobalNPC.ghostBoss = -1;
            CalamityGlobalNPC.laserEye = -1;
            CalamityGlobalNPC.fireEye = -1;
            CalamityGlobalNPC.brimstoneElemental = -1;
            bossRushStage = 0;
            DoGSecondStageCountdown = 0;
            bossRushActive = false;
            bossRushSpawnCountdown = 180;
            bossSpawnCountdown = 0;
            bossType = 0;
			newAltarX = 0;
			newAltarY = 0;
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
            downedScavenger = false;
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
            downedBossAny = false;
            spawnedHardBoss = false;
            demonMode = false;
            onionMode = false;
            revenge = false;
            downedStarGod = false;
            downedAstrageldon = false;
            downedPolterghast = false;
            downedLORDE = false;
            downedBuffedMothron = false;
            downedOldDuke = false;
            death = false;
            defiled = false;
            armageddon = false;
            ironHeart = false;
            dragonScalesBought = false;
        }
        #endregion

        #region Save
        public override TagCompound Save()
        {
            var downed = new List<string>();
            if (downedDesertScourge)
                downed.Add("desertScourge");
            if (downedAquaticScourge)
                downed.Add("aquaticScourge");
            if (downedHiveMind)
                downed.Add("hiveMind");
            if (downedPerforator)
                downed.Add("perforator");
            if (downedSlimeGod)
                downed.Add("slimeGod");
            if (downedCryogen)
                downed.Add("cryogen");
            if (downedBrimstoneElemental)
                downed.Add("brimstoneElemental");
            if (downedCalamitas)
                downed.Add("calamitas");
            if (downedLeviathan)
                downed.Add("leviathan");
            if (downedDoG)
                downed.Add("devourerOfGods");
            if (downedPlaguebringer)
                downed.Add("plaguebringerGoliath");
            if (downedGuardians)
                downed.Add("guardians");
            if (downedProvidence)
                downed.Add("providence");
            if (downedSentinel1)
                downed.Add("ceaselessVoid");
            if (downedSentinel2)
                downed.Add("stormWeaver");
            if (downedSentinel3)
                downed.Add("signus");
            if (downedYharon)
                downed.Add("yharon");
            if (buffedEclipse)
                downed.Add("eclipse");
            if (downedSCal)
                downed.Add("supremeCalamitas");
            if (downedBumble)
                downed.Add("bumblebirb");
            if (downedCrabulon)
                downed.Add("crabulon");
            if (downedBetsy)
                downed.Add("betsy");
            if (downedScavenger)
                downed.Add("scavenger");
            if (downedBossAny)
                downed.Add("anyBoss");
            if (demonMode)
                downed.Add("demonMode");
            if (onionMode)
                downed.Add("onionMode");
            if (revenge)
                downed.Add("revenge");
            if (downedStarGod)
                downed.Add("starGod");
            if (downedAstrageldon)
                downed.Add("astrageldon");
            if (spawnedHardBoss)
                downed.Add("hardBoss");
            if (downedPolterghast)
                downed.Add("polterghast");
            if (downedLORDE)
                downed.Add("lorde");
            if (downedBuffedMothron)
                downed.Add("moth");
            if (downedOldDuke)
                downed.Add("oldDuke");
            if (death)
                downed.Add("death");
            if (defiled)
                downed.Add("defiled");
            if (armageddon)
                downed.Add("armageddon");
            if (ironHeart)
                downed.Add("ironHeart");
            if (abyssSide)
                downed.Add("abyssSide");
            if (bossRushActive)
                downed.Add("bossRushActive");
            if (downedCLAM)
                downed.Add("clam");
            if (dragonScalesBought)
                downed.Add("scales");

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
            downedBossAny = downed.Contains("anyBoss");
            demonMode = downed.Contains("demonMode");
            onionMode = downed.Contains("onionMode");
            revenge = downed.Contains("revenge");
            downedStarGod = downed.Contains("starGod");
            downedAstrageldon = downed.Contains("astrageldon");
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
            dragonScalesBought = downed.Contains("scales");

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

                // Explicitly discard the now-unused vanilla boss booleans
                BitsByte flags3 = reader.ReadByte();
                downedSCal = flags3[0];
                downedBumble = flags3[1];
                downedCrabulon = flags3[2];
                downedBetsy = flags3[3];
                downedScavenger = flags3[4];
                _ = flags3[5];
                _ = flags3[6];
                _ = flags3[7];

                BitsByte flags4 = reader.ReadByte();
                _ = flags4[0];
                _ = flags4[1];
                _ = flags4[2];
                _ = flags4[3];
                downedBossAny = flags4[4];
                demonMode = flags4[5];
                onionMode = flags4[6];
                revenge = flags4[7];

                // These 3 bits are currently unused, they used to be astral meteor drops
                BitsByte flags5 = reader.ReadByte();
                downedStarGod = flags5[0];
                _ = flags5[1];
                _ = flags5[2];
                _ = flags5[3];
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
                dragonScalesBought = flags7[3];
            }
            else
            {
                ModContent.GetInstance<CalamityMod>().Logger.Error("Unknown loadVersion: " + loadVersion);
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

            // Don't write meaningful values for the now-unused vanilla boss booleans
            BitsByte flags3 = new BitsByte();
            flags3[0] = downedSCal;
            flags3[1] = downedBumble;
            flags3[2] = downedCrabulon;
            flags3[3] = downedBetsy;
            flags3[4] = downedScavenger;
            flags3[5] = false;
            flags3[6] = false;
            flags3[7] = false;

            BitsByte flags4 = new BitsByte();
            flags4[0] = false;
            flags4[1] = false;
            flags4[2] = false;
            flags4[3] = false;
            flags4[4] = downedBossAny;
            flags4[5] = demonMode;
            flags4[6] = onionMode;
            flags4[7] = revenge;

            // These 3 bits are currently unused, they used to be astral meteor drops
            BitsByte flags5 = new BitsByte();
            flags5[0] = downedStarGod;
            flags5[1] = false;
            flags5[2] = false;
            flags5[3] = false;
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
            flags7[3] = dragonScalesBought;

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

            // Explicitly discard the now-unused vanilla boss booleans
            BitsByte flags3 = reader.ReadByte();
            downedSCal = flags3[0];
            downedBumble = flags3[1];
            downedCrabulon = flags3[2];
            downedBetsy = flags3[3];
            downedScavenger = flags3[4];
            _ = flags3[5];
            _ = flags3[6];
            _ = flags3[7];

            BitsByte flags4 = reader.ReadByte();
            _ = flags4[0];
            _ = flags4[1];
            _ = flags4[2];
            _ = flags4[3];
            downedBossAny = flags4[4];
            demonMode = flags4[5];
            onionMode = flags4[6];
            revenge = flags4[7];

            // These 3 bits are currently unused, they used to be astral meteor drops
            BitsByte flags5 = reader.ReadByte();
            downedStarGod = flags5[0];
            _ = flags5[1];
            _ = flags5[2];
            _ = flags5[3];
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
            dragonScalesBought = flags7[3];

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
            calamityTiles = tileCounts[ModContent.TileType<CharredOre>()] + tileCounts[ModContent.TileType<BrimstoneSlag>()];
            sunkenSeaTiles = tileCounts[ModContent.TileType<EutrophicSand>()] + tileCounts[ModContent.TileType<Navystone>()] + tileCounts[ModContent.TileType<SeaPrism>()];
            abyssTiles = tileCounts[ModContent.TileType<AbyssGravel>()];
            sulphurTiles = tileCounts[ModContent.TileType<SulphurousSand>()];

            #region Astral Stuff
            int astralDesertTiles = tileCounts[ModContent.TileType<AstralSand>()] + tileCounts[ModContent.TileType<AstralSandstone>()] + tileCounts[ModContent.TileType<HardenedAstralSand>()];
            int astralSnowTiles = tileCounts[ModContent.TileType<AstralIce>()];

            Main.sandTiles += astralDesertTiles;
            Main.snowTiles += astralSnowTiles;

            astralTiles = astralDesertTiles + astralSnowTiles + tileCounts[ModContent.TileType<AstralDirt>()] + tileCounts[ModContent.TileType<AstralStone>()] + tileCounts[ModContent.TileType<AstralGrass>()] + tileCounts[ModContent.TileType<AstralOre>()];
            #endregion
        }
        #endregion

        #region PreWorldGen
        public override void PreWorldGen()
        {
            numAbyssIslands = 0;
            roxShrinePlaced = false;
        }
        #endregion

        #region ModifyWorldGenTasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int ShiniesIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Shinies"));
            if (ShiniesIndex != -1)
            {
                tasks.Insert(ShiniesIndex + 1, new PassLegacy("IceTomb", delegate (GenerationProgress progress)
                {
                    progress.Message = "Ice Tomb";
                    SmallBiomes.PlaceIceTomb();
                }));

                tasks.Insert(ShiniesIndex + 2, new PassLegacy("EvilIsland", delegate (GenerationProgress progress)
                {
                    progress.Message = "Evil Island";
                    SmallBiomes.PlaceEvilIsland();
                }));
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

			int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
			tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", delegate (GenerationProgress progress)
			{
				progress.Message = "Building a Tyrant's temple";
				WorldGenerationMethods.NewJungleTemple();
			});

			int JungleTempleIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Temple"));
			tasks[JungleTempleIndex2] = new PassLegacy("Temple", delegate (GenerationProgress progress)
			{
				progress.Message = "Building a Tyrant's temple";
				Main.tileSolid[162] = false;
				Main.tileSolid[226] = true;
				WorldGenerationMethods.NewJungleTemplePart2();
				Main.tileSolid[232] = false;
			});

			int LihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
			tasks[LihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", delegate (GenerationProgress progress)
			{
				progress.Message = "Placing a Lihzahrd altar very carefully";
				WorldGenerationMethods.NewJungleTempleLihzahrdAltar();
			});

			int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
            if (SulphurIndex != -1)
            {
                tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur", delegate (GenerationProgress progress)
                {
                    progress.Message = "Sulphur Sea";
                    Abyss.PlaceSulphurSea();
                }));
            }

            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                //Not touching this yet because the Crags will be reworked in the future
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

                tasks.Insert(FinalIndex + 2, new PassLegacy("SpecialShrines", delegate (GenerationProgress progress)
                {
                    progress.Message = "Special Shrines";
                    SmallBiomes.PlaceShrines();
                }));

                tasks.Insert(FinalIndex + 3, new PassLegacy("Abyss", delegate (GenerationProgress progress)
                {
                    progress.Message = "The Abyss";
                    Abyss.PlaceAbyss();
                }));

                tasks.Insert(FinalIndex + 4, new PassLegacy("IWannaRock", delegate (GenerationProgress progress)
                {
                    progress.Message = "I Wanna Rock";
                    WorldGenerationMethods.PlaceRoxShrine();
                }));
            }

            tasks.Add(new PassLegacy("Planetoid Test", WorldGenerationMethods.Planetoids));
        }

        // An Astral Meteor always falls at the beginning of Hardmode.
        public override void ModifyHardmodeTasks(List<GenPass> tasks)
        {
            // Yes, this internal identifier is misspelled in vanilla.
            int announceIndex = tasks.FindIndex(match => match.Name == "Hardmode Announcment");

            // Insert the Astral biome generation right before the final hardmode announcement.
            tasks.Insert(announceIndex, new PassLegacy("AstralMeteor", delegate (GenerationProgress progress)
            {
                WorldGenerationMethods.PlaceAstralMeteor();
            }));
        }
        #endregion

        #region PostUpdate
        public override void PostUpdate()
        {
            // Sunken Sea Location...duh
            SunkenSeaLocation = new Rectangle(WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Bottom,
                        WorldGen.UndergroundDesertLocation.Width, WorldGen.UndergroundDesertLocation.Height / 2);

            // Player variable, always finds the closest player relative to the center of the map
            int closestPlayer = (int)Player.FindClosest(new Vector2((float)(Main.maxTilesX / 2), (float)Main.worldSurface / 2f) * 16f, 0, 0);

            // Force boss rush to off
            if (!deactivateStupidFuckingBullshit)
            {
                deactivateStupidFuckingBullshit = true;
                bossRushActive = false;
                CalamityMod.UpdateServerBoolean();
            }

            // Boss Rush shit
            if (bossRushActive)
            {
                // Prevent Moon Lord from spawning naturally
                if (NPC.MoonLordCountdown > 0)
                {
                    NPC.MoonLordCountdown = 0;
                }

                // Do boss rush countdown and shit if no boss is alive
                if (!CalamityPlayer.areThereAnyDamnBosses)
                {
                    // Stage text
                    if (bossRushSpawnCountdown > 0)
                    {
                        bossRushSpawnCountdown--;
                        if (bossRushSpawnCountdown == 180)
                        {
                            // After Fishron is dead
                            if (bossRushStage == 28)
                            {
                                string key = "Mods.CalamityMod.BossRushTierThreeEndText2";
                                Color messageColor = Color.LightCoral;
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    Main.NewText(Language.GetTextValue(key), messageColor);
                                }
                                else if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                                }
                            }

                            // After Providence is dead
                            else if (bossRushStage == 36)
                            {
                                string key = "Mods.CalamityMod.BossRushTierFourEndText2";
                                Color messageColor = Color.LightCoral;
                                if (Main.netMode == NetmodeID.SinglePlayer)
                                {
                                    Main.NewText(Language.GetTextValue(key), messageColor);
                                }
                                else if (Main.netMode == NetmodeID.Server)
                                {
                                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                                }
                            }
                        }
                    }

                    // Cooldown and boss spawn
                    if (bossRushSpawnCountdown <= 0)
                    {
                        // Cooldown before next boss spawns
                        bossRushSpawnCountdown = 60;

                        // Increase cooldown post-Fishron
                        if (bossRushStage >= 27)
                        {
                            bossRushSpawnCountdown += 300;
                        }

                        // Change cooldown based on stage
                        switch (bossRushStage)
                        {
                            // When Destroyer or Cultist dies, increase time to show text
                            case 9:
                            case 18:
                                bossRushSpawnCountdown = 300;
                                break;

                            // When Signus dies, increase time to give players a moment to get in a good spot for Ravager
                            case 25:
                                bossRushSpawnCountdown = 360;
                                break;

                            // When Calamitas Clone dies, increase time to give players a moment to relax
                            case 32:
                                bossRushSpawnCountdown = 420;
                                break;
                            default:
                                break;
                        }

                        // Post-Wall of Flesh teleport back to spawn
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

                        // Remove Providence debuff for next boss fight
                        else if (bossRushStage == 36)
                        {
                            for (int playerIndex = 0; playerIndex < 255; playerIndex++)
                            {
                                if (Main.player[playerIndex].active)
                                {
                                    Player player = Main.player[playerIndex];
                                    if (player.FindBuffIndex(ModContent.BuffType<ExtremeGravity>()) > -1)
                                    {
                                        player.ClearBuff(ModContent.BuffType<ExtremeGravity>());
                                    }
                                }
                            }
                        }

                        // Spawn bosses
                        if (Main.netMode != NetmodeID.MultiplayerClient)
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
                                    NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)Main.rand.Next(-100, 101)),
                                        (int)(Main.player[closestPlayer].position.Y - 400f),
                                        NPCID.Golem, 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 6:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<ProfanedGuardianBoss>());
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<ProfanedGuardianBoss2>());
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<ProfanedGuardianBoss3>());
                                    break;
                                case 7:
                                    NPC.SpawnOnPlayer(closestPlayer, NPCID.EaterofWorldsHead);
                                    break;
                                case 8:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<AstrumAureus>());
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
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Bumblefuck>());
                                    break;
                                case 12:
                                    NPC.SpawnWOF(Main.player[closestPlayer].position);
                                    break;
                                case 13:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<HiveMind>());
                                    break;
                                case 14:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(closestPlayer, NPCID.SkeletronHead);
                                    break;
                                case 15:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<StormWeaverHead>());
                                    break;
                                case 16:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<AquaticScourgeHead>());
                                    break;
                                case 17:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DesertScourgeHead>());
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DesertScourgeHeadSmall>());
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DesertScourgeHeadSmall>());
                                    break;
                                case 18:
                                    int num1302 = NPC.NewNPC((int)Main.player[closestPlayer].Center.X, (int)Main.player[closestPlayer].Center.Y - 400, NPCID.CultistBoss, 0, 0f, 0f, 0f, 0f, 255);
                                    Main.npc[num1302].direction = Main.npc[num1302].spriteDirection = Math.Sign(Main.player[closestPlayer].Center.X - (float)Main.player[closestPlayer].Center.X - 90f);
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
                                    NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)Main.rand.Next(-100, 101)), (int)(Main.player[closestPlayer].position.Y - 400f), ModContent.NPCType<CrabulonIdle>(), 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 20:
                                    NPC.SpawnOnPlayer(closestPlayer, NPCID.Plantera);
                                    break;
                                case 21:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<CeaselessVoid>());
                                    break;
                                case 22:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<PerforatorHive>());
                                    break;
                                case 23:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Cryogen>());
                                    break;
                                case 24:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<BrimstoneElemental>());
                                    break;
                                case 25:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Signus>());
                                    break;
                                case 26:
                                    NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)Main.rand.Next(-100, 101)), (int)(Main.player[closestPlayer].position.Y - 400f), ModContent.NPCType<RavagerBody>(), 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 27:
                                    NPC.NewNPC((int)(Main.player[closestPlayer].position.X + (float)Main.rand.Next(-100, 101)), (int)(Main.player[closestPlayer].position.Y - 400f), NPCID.DukeFishron, 0, 0f, 0f, 0f, 0f, 255);
                                    break;
                                case 28:
                                    NPC.SpawnOnPlayer(closestPlayer, NPCID.MoonLordCore);
                                    break;
                                case 29:
                                    ChangeTime(false);
                                    for (int x = 0; x < 10; x++)
                                    {
                                        NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<AstrumDeusHead>());
                                    }
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<AstrumDeusHeadSpectral>());
                                    break;
                                case 30:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Polterghast>());
                                    break;
                                case 31:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<PlaguebringerGoliath>());
                                    break;
                                case 32:
                                    ChangeTime(false);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Calamitas>());
                                    break;
                                case 33:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Siren>());
                                    break;
                                case 34:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<SlimeGod>());
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<SlimeGodRun>());
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<SlimeGodCore>());
                                    break;
                                case 35:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Providence>());
                                    break;
                                case 36:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<SupremeCalamitas>());
                                    break;
                                case 37:
                                    ChangeTime(true);
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Yharon>());
                                    break;
                                case 38:
                                    NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DevourerofGodsHeadS>());
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
                    if (Main.netMode == NetmodeID.Server)
                    {
                        var netMessage = mod.GetPacket();
                        netMessage.Write((byte)CalamityModMessageType.BossRushStage);
                        netMessage.Write(bossRushStage);
                        netMessage.Send();
                    }
                }
            }

            if (DoGSecondStageCountdown > 0)
            {
                DoGSecondStageCountdown--;
                if (Main.netMode == NetmodeID.Server)
                {
                    var netMessage = mod.GetPacket();
                    netMessage.Write((byte)CalamityModMessageType.DoGCountdownSync);
                    netMessage.Write(DoGSecondStageCountdown);
                    netMessage.Send();
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    if (DoGSecondStageCountdown == 21540)
                    {
                        NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<CeaselessVoid>());
                    }
                    if (DoGSecondStageCountdown == 14340)
                    {
                        NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<StormWeaverHead>());
                    }
                    if (DoGSecondStageCountdown == 7140)
                    {
                        NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<Signus>());
                    }
                    if (DoGSecondStageCountdown <= 60)
                    {
                        if (!NPC.AnyNPCs(ModContent.NPCType<DevourerofGodsHeadS>()))
                        {
                            NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DevourerofGodsHeadS>());
                            string key = "Mods.CalamityMod.EdgyBossText10";
                            Color messageColor = Color.Cyan;
                            if (Main.netMode == NetmodeID.SinglePlayer)
                            {
                                Main.NewText(Language.GetTextValue(key), messageColor);
                            }
                            else if (Main.netMode == NetmodeID.Server)
                            {
                                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                            }
                        }
                    }
                }
            }

            if (Main.player[closestPlayer].ZoneDungeon && !NPC.downedBoss3)
            {
                if (!NPC.AnyNPCs(NPCID.DungeonGuardian) && Main.netMode != NetmodeID.MultiplayerClient)
                    NPC.SpawnOnPlayer(closestPlayer, NPCID.DungeonGuardian); //your hell is as vast as my bonergrin, pray your life ends quickly
            }

            if (Main.player[closestPlayer].ZoneRockLayerHeight &&
                !Main.player[closestPlayer].ZoneUnderworldHeight &&
                !Main.player[closestPlayer].ZoneDungeon &&
                !Main.player[closestPlayer].ZoneJungle &&
                !Main.player[closestPlayer].Calamity().ZoneSunkenSea &&
                !CalamityPlayer.areThereAnyDamnBosses)
            {
                if (NPC.downedPlantBoss &&
                    !Main.player[closestPlayer].Calamity().ZoneAbyss &&
                    Main.player[closestPlayer].townNPCs < 3f)
                {
                    double spawnRate = 100000D;

                    if (death)
                        spawnRate *= 0.75D;
                    else if (revenge)
                        spawnRate *= 0.85D;

                    if (demonMode)
                        spawnRate *= 0.75D;

                    if (Main.player[closestPlayer].Calamity().zerg && Main.player[closestPlayer].Calamity().chaosCandle)
                        spawnRate *= 0.005D;
                    else if (Main.player[closestPlayer].Calamity().zerg)
                        spawnRate *= 0.01D;
                    else if (Main.player[closestPlayer].Calamity().chaosCandle)
                        spawnRate *= 0.02D;

                    if (Main.player[closestPlayer].Calamity().zen && Main.player[closestPlayer].Calamity().tranquilityCandle)
                        spawnRate *= 75D;
                    else if (Main.player[closestPlayer].Calamity().zen)
                        spawnRate *= 50D;
                    else if (Main.player[closestPlayer].Calamity().tranquilityCandle)
                        spawnRate *= 25D;

                    int chance = (int)spawnRate;
                    if (Main.rand.Next(chance) == 0)
                    {
                        if (!NPC.AnyNPCs(ModContent.NPCType<ArmoredDiggerHead>()) && Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<ArmoredDiggerHead>());
                    }
                }
            }

            if (Main.dayTime && Main.hardMode)
            {
                if (Main.player[closestPlayer].townNPCs >= 2f)
                {
                    if (Main.rand.NextBool(2000))
                    {
                        int steamGril = NPC.FindFirstNPC(NPCID.Steampunker);
                        if (steamGril == -1 && Main.netMode != NetmodeID.MultiplayerClient)
                            NPC.SpawnOnPlayer(closestPlayer, NPCID.Steampunker);
                    }
                }
            }

            if (Main.player[closestPlayer].Calamity().ZoneAbyss)
            {
                if (Main.player[closestPlayer].chaosState)
                {
                    if (!NPC.AnyNPCs(ModContent.NPCType<EidolonWyrmHeadHuge>()) && Main.netMode != NetmodeID.MultiplayerClient)
                        NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<EidolonWyrmHeadHuge>());
                }
            }

            // TODO -- remove random lorde spawn code
            /*if (Main.rand.NextBool(100000000))
            {
                string key = "Mods.CalamityMod.AprilFools";
                Color messageColor = Color.Crimson;
                if (Main.netMode == NetmodeID.SinglePlayer)
                {
                    Main.NewText(Language.GetTextValue(key), messageColor);
                }
                else if (Main.netMode == NetmodeID.Server)
                {
                    NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
                }
            }*/

            if (death && !CalamityPlayer.areThereAnyDamnBosses && Main.player[closestPlayer].statLifeMax2 >= 300)
            {
                if (bossSpawnCountdown <= 0) //check for countdown being 0
                {
                    if (Main.rand.NextBool(50000))
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
                                bossType = ModContent.NPCType<DesertScourgeHead>();
                                bossSpawnCountdown = 3600;
                            }

                        if (!downedPerforator && bossType == 0)
                            if (Main.player[closestPlayer].ZoneCrimson)
                            {
                                BossText();
                                bossType = ModContent.NPCType<PerforatorHive>();
                                bossSpawnCountdown = 3600;
                            }

                        if (!downedHiveMind && bossType == 0)
                            if (Main.player[closestPlayer].ZoneCorrupt)
                            {
                                BossText();
                                bossType = ModContent.NPCType<HiveMind>();
                                bossSpawnCountdown = 3600;
                            }

                        if (!downedCrabulon && bossType == 0)
                            if (Main.player[closestPlayer].ZoneGlowshroom)
                            {
                                BossText();
                                bossType = ModContent.NPCType<CrabulonIdle>();
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
                                if (Main.player[closestPlayer].ZoneBeach && !Main.player[closestPlayer].Calamity().ZoneSulphur)
                                {
                                    BossText();
                                    bossType = NPCID.DukeFishron;
                                    bossSpawnCountdown = 3600;
                                }

                            if (!downedCryogen && bossType == 0)
                                if (Main.player[closestPlayer].ZoneSnow && Main.player[closestPlayer].ZoneOverworldHeight)
                                {
                                    BossText();
                                    bossType = ModContent.NPCType<Cryogen>();
                                    bossSpawnCountdown = 3600;
                                }

                            if (!downedCalamitas && bossType == 0)
                                if (!Main.dayTime && Main.player[closestPlayer].ZoneOverworldHeight)
                                {
                                    BossText();
                                    bossType = ModContent.NPCType<Calamitas>();
                                    bossSpawnCountdown = 3600;
                                }

                            if (!downedAstrageldon && bossType == 0)
                                if (Main.player[closestPlayer].Calamity().ZoneAstral &&
                                    !Main.dayTime && Main.player[closestPlayer].ZoneOverworldHeight)
                                {
                                    BossText();
                                    bossType = ModContent.NPCType<AstrumAureus>();
                                    bossSpawnCountdown = 3600;
                                }

                            if (!downedPlaguebringer && bossType == 0)
                                if (Main.player[closestPlayer].ZoneJungle && NPC.downedGolemBoss && Main.player[closestPlayer].ZoneOverworldHeight)
                                {
                                    BossText();
                                    bossType = ModContent.NPCType<PlaguebringerGoliath>();
                                    bossSpawnCountdown = 3600;
                                }

                            if (NPC.downedMoonlord)
                            {
                                if (!downedGuardians && bossType == 0)
                                    if (Main.dayTime && (Main.player[closestPlayer].ZoneUnderworldHeight ||
                                        (Main.player[closestPlayer].ZoneHoly && Main.player[closestPlayer].ZoneOverworldHeight)))
                                    {
                                        BossText();
                                        bossType = ModContent.NPCType<ProfanedGuardianBoss>();
                                        bossSpawnCountdown = 3600;
                                    }

                                if (!downedBumble && bossType == 0)
                                    if (Main.player[closestPlayer].ZoneJungle && Main.player[closestPlayer].ZoneOverworldHeight)
                                    {
                                        BossText();
                                        bossType = ModContent.NPCType<Bumblefuck>();
                                        bossSpawnCountdown = 3600;
                                    }
                            }
                        }
                        if (Main.netMode == NetmodeID.Server)
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
                    if (Main.netMode == NetmodeID.Server)
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

                        if (bossType == ModContent.NPCType<DesertScourgeHead>())
                        {
                            if (!Main.player[closestPlayer].ZoneDesert)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<PerforatorHive>())
                        {
                            if (!Main.player[closestPlayer].ZoneCrimson)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<HiveMind>())
                        {
                            if (!Main.player[closestPlayer].ZoneCorrupt)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<CrabulonIdle>())
                        {
                            if (!Main.player[closestPlayer].ZoneGlowshroom)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<Cryogen>())
                        {
                            if (!Main.player[closestPlayer].ZoneSnow || !Main.player[closestPlayer].ZoneOverworldHeight)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<Calamitas>())
                        {
                            if (Main.dayTime || !Main.player[closestPlayer].ZoneOverworldHeight)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<AstrumAureus>())
                        {
                            if (!Main.player[closestPlayer].Calamity().ZoneAstral ||
                                    Main.dayTime || !Main.player[closestPlayer].ZoneOverworldHeight)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<PlaguebringerGoliath>())
                        {
                            if (!Main.player[closestPlayer].ZoneJungle || !Main.player[closestPlayer].ZoneOverworldHeight)
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<ProfanedGuardianBoss>())
                        {
                            if (!Main.dayTime || (!Main.player[closestPlayer].ZoneUnderworldHeight &&
                                        (!Main.player[closestPlayer].ZoneHoly || !Main.player[closestPlayer].ZoneOverworldHeight)))
                                canSpawn = false;
                        }
                        else if (bossType == ModContent.NPCType<Bumblefuck>())
                        {
                            if (!Main.player[closestPlayer].ZoneJungle || !Main.player[closestPlayer].ZoneOverworldHeight)
                                canSpawn = false;
                        }

                        if (canSpawn && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            if (bossType == NPCID.Spazmatism)
                                NPC.SpawnOnPlayer(closestPlayer, NPCID.Retinazer);
                            else if (bossType == ModContent.NPCType<ProfanedGuardianBoss>())
                            {
                                NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<ProfanedGuardianBoss2>());
                                NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<ProfanedGuardianBoss3>());
                            }
                            else if (bossType == ModContent.NPCType<DesertScourgeHead>())
                            {
                                NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DesertScourgeHeadSmall>());
                                NPC.SpawnOnPlayer(closestPlayer, ModContent.NPCType<DesertScourgeHeadSmall>());
                            }
                            if (bossType == NPCID.DukeFishron)
                                NPC.NewNPC((int)Main.player[closestPlayer].Center.X - 300, (int)Main.player[closestPlayer].Center.Y - 300, bossType);
                            else
                                NPC.SpawnOnPlayer(closestPlayer, bossType);
                        }
                        bossType = 0;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            var netMessage = mod.GetPacket();
                            netMessage.Write((byte)CalamityModMessageType.BossTypeSync);
                            netMessage.Write(bossType);
                            netMessage.Send();
                        }
                    }
                }
            }

            if (!downedDesertScourge && Main.netMode != NetmodeID.MultiplayerClient)
                CalamityUtils.StopSandstorm();

            if (Main.netMode != NetmodeID.MultiplayerClient)
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
        #endregion

        #region ChangeTime
        public static void ChangeTime(bool day)
        {
            Main.time = 0.0;
            Main.dayTime = day;
            CalamityMod.UpdateServerBoolean();
        }
        #endregion

        #region BossText
        public void BossText()
        {
            string key = "Mods.CalamityMod.BossSpawnText";
            Color messageColor = Color.Crimson;
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                Main.NewText(Language.GetTextValue(key), messageColor);
            }
            else if (Main.netMode == NetmodeID.Server)
            {
                NetMessage.BroadcastChatMessage(NetworkText.FromKey(key), messageColor);
            }
        }
        #endregion
    }
}
