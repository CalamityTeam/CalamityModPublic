using System.Collections.Generic;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using CalamityMod.Items.SummonItems;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod.Systems
{
    public class WorldgenManagementSystem : ModSystem
    {
        #region PreWorldGen
        public override void PreWorldGen()
        {
            Abyss.TotalPlacedIslandsSoFar = 0;
            roxShrinePlaced = false;

            // This will only be applied at world-gen time to new worlds.
            // Old worlds will never receive this marker naturally.
            IsWorldAfterDraedonUpdate = true;
        }
        #endregion

        #region ModifyWorldGenTasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref float totalWeight)
        {
            int islandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Island Houses"));
            if (islandIndex != -1)
            {
                tasks.Insert(islandIndex + 2, new PassLegacy("EvilIsland", (progress, config) =>
                {
                    progress.Message = "Corrupting a floating island";
                    WorldEvilIsland.PlaceEvilIsland();
                }));
            }

            int DungeonChestIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (DungeonChestIndex != -1)
            {
                tasks.Insert(DungeonChestIndex + 1, new PassLegacy("CalamityDungeonBiomeChests", MiscWorldgenRoutines.GenerateBiomeChests));
            }

            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
            {
                progress.Message = "Building a bigger jungle temple";
                CustomTemple.NewJungleTemple();
            });

            int JungleTempleIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Temple"));
            tasks[JungleTempleIndex2] = new PassLegacy("Temple", (progress, config) =>
            {
                progress.Message = "Building a bigger jungle temple";
                Main.tileSolid[162] = false;
                Main.tileSolid[226] = true;
                CustomTemple.NewJungleTemplePart2();
                Main.tileSolid[232] = false;
            });

            // Sunken Sea gens after Traps because otherwise boulders spawn in the Sunken Sea :)
            int TrapsIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Traps"));
            if (TrapsIndex != -1)
            {
                tasks.Insert(TrapsIndex + 1, new PassLegacy("SunkenSea", (progress, config) =>
                {
                    progress.Message = "Partially flooding an overblown desert";
                    int sunkenSeaX = WorldGen.UndergroundDesertLocation.Left;
                    int sunkenSeaY = WorldGen.UndergroundDesertLocation.Center.Y;
                    SunkenSea.Place(new Point(sunkenSeaX, sunkenSeaY));
                }));
            }

            int LihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
            tasks[LihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", (progress, config) =>
            {
                progress.Message = "Placing the Lihzahrd altar";
                CustomTemple.NewJungleTempleLihzahrdAltar();
            });

            // TODO -- Most of the below worldgen should be spaced out at better points of generation instead of all crammed at the end

            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                int currentFinalIndex = FinalIndex;
                tasks.Insert(++currentFinalIndex, new PassLegacy("GemDepthAdjustment", (progress, config) =>
                {
                    progress.Message = "Reallocating gem deposits to match cavern depth";
                    MiscWorldgenRoutines.SmartGemGen();
                }));

                tasks.Insert(++currentFinalIndex, new PassLegacy("Planetoids", Planetoid.GenerateAllBasePlanetoids));

                //Not touching this yet because the Crags will be reworked in the future
                #region BrimstoneCrag
                tasks.Insert(++currentFinalIndex, new PassLegacy("BrimstoneCrag", (progress, config) =>
                {
                    progress.Message = "Uncovering the ruins of a fallen empire";

                    int x = Main.maxTilesX;

                    int xUnderworldGen = WorldGen.genRand.Next((int)((double)x * 0.1), (int)((double)x * 0.15));
                    int yUnderworldGen = Main.maxTilesY - 100;

                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen, yUnderworldGen, 180, 201, 120, 136);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 50, yUnderworldGen - 30, 100, 111, 60, 71);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 50, yUnderworldGen - 30, 100, 111, 60, 71);

                    MiscWorldgenRoutines.ChasmGenerator(xUnderworldGen - 110, yUnderworldGen - 10, WorldGen.genRand.Next(150) + 150);
                    MiscWorldgenRoutines.ChasmGenerator(xUnderworldGen + 110, yUnderworldGen - 10, WorldGen.genRand.Next(150) + 150);

                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 180, yUnderworldGen - 20, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 180, yUnderworldGen - 20, 60, 66, 35, 41);

                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen, yUnderworldGen + 30, 1323);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen - 22, yUnderworldGen + 15, 1322);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen + 22, yUnderworldGen + 15, 535);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen - 50, yUnderworldGen - 30, 112);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen + 50, yUnderworldGen - 30, 906);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen - 150, yUnderworldGen - 30, 218);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen + 150, yUnderworldGen - 30, 3019);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen - 180, yUnderworldGen - 20, 274);
                    MiscWorldgenRoutines.UnderworldIslandHouse(xUnderworldGen + 180, yUnderworldGen - 20, 220);
                }));
                #endregion

                int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
                if (SulphurIndex != -1)
                {
                    tasks.Insert(SulphurIndex + 1, new PassLegacy("SulphurSea", (progress, config) =>
                    {
                        progress.Message = "Polluting one of the oceans";
                        SulphurousSea.PlaceSulphurSea();
                    }));
                }

                tasks.Insert(++currentFinalIndex, new PassLegacy("SpecialShrines", (progress, config) =>
                {
                    progress.Message = "Placing hidden shrines";
                    UndergroundShrines.PlaceShrines();
                }));


                tasks.Insert(++currentFinalIndex, new PassLegacy("DraedonStructures", (progress, config) =>
                {
                    progress.Message = "Rust and Dust";
                    List<Point> workshopPositions = new List<Point>();

                    // Small: 4, Normal: 7, Large: 9
                    // Tries to scale up reasonably for XL worlds
                    int workshopCount = Main.maxTilesX / 900;

                    // Small: 2, Normal: 4, Large: 5
                    // Tries to scale up reasonably for XL worlds
                    int labCount = Main.maxTilesX / 1500;

                    DraedonStructures.PlaceHellLab(out Point hellPlacementPosition, workshopPositions, WorldGen.structures);
                    workshopPositions.Add(hellPlacementPosition);

                    DraedonStructures.PlaceSunkenSeaLab(out Point sunkenSeaPlacementPosition, workshopPositions, WorldGen.structures);
                    workshopPositions.Add(sunkenSeaPlacementPosition);

                    DraedonStructures.PlaceIceLab(out Point icePlacementPosition, workshopPositions, WorldGen.structures);
                    workshopPositions.Add(icePlacementPosition);

                    DraedonStructures.PlacePlagueLab(out Point plaguePlacementPosition, workshopPositions, WorldGen.structures);
                    workshopPositions.Add(plaguePlacementPosition);

                    DraedonStructures.PlaceCavernLab(out Point cavernPlacementPosition, workshopPositions, WorldGen.structures);
                    workshopPositions.Add(cavernPlacementPosition);

                    for (int i = 0; i < workshopCount; i++)
                    {
                        DraedonStructures.PlaceWorkshop(out Point placementPosition, workshopPositions, WorldGen.structures);
                        workshopPositions.Add(placementPosition);
                    }
                    for (int i = 0; i < labCount; i++)
                    {
                        DraedonStructures.PlaceResearchFacility(out Point placementPosition, workshopPositions, WorldGen.structures);
                        workshopPositions.Add(placementPosition);
                    }
                }));

                tasks.Insert(++currentFinalIndex, new PassLegacy("Abyss", (progress, config) =>
                {
                    progress.Message = "Discovering the new Challenger Deep";
                    Abyss.PlaceAbyss();
                }));

                tasks.Insert(++currentFinalIndex, new PassLegacy("SulphurSea2", (progress, config) =>
                {
                    progress.Message = "Further polluting one of the oceans";
                    SulphurousSea.SulphurSeaGenerationAfterAbyss();
                }));

                tasks.Insert(++currentFinalIndex, new PassLegacy("Roxcalibur", (progress, config) =>
                {
                    progress.Message = "I Wanna Rock";
                    MiscWorldgenRoutines.PlaceRoxShrine();
                }));
            }
        }

        // An Astral Meteor always falls at the beginning of Hardmode.
        public override void ModifyHardmodeTasks(List<GenPass> tasks)
        {
            int announceIndex = tasks.FindIndex(match => match.Name == "Hardmode Announcement");

            // Insert the Astral biome generation right before the final hardmode announcement.
            tasks.Insert(announceIndex, new PassLegacy("AstralMeteor", (progress, config) =>
            {
                AstralBiome.PlaceAstralMeteor();
            }));
        }
        #endregion

        #region PostWorldGen
        public override void PostWorldGen()
        {
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null)
                {
                    //Checks which sheet a chest belongs to
                    bool isContainer1 = Main.tile[chest.x, chest.y].TileType == TileID.Containers;
                    bool isContainer2 = Main.tile[chest.x, chest.y].TileType == TileID.Containers2;

                    //Pre-1.4 chests
                    bool isBrownChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 0;
                    bool isGoldChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 36 || Main.tile[chest.x, chest.y].TileFrameX == 2*36); //Includes Locked Gold Chests
                    bool isMahoganyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 8 * 36;
                    bool isIvyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;
                    bool isIceChest = isContainer1 &&  Main.tile[chest.x, chest.y].TileFrameX == 11 * 36;
                    bool isMushroomChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 32 * 36;
                    bool isMarniteChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 50 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 51 * 36);

                    //1.4 chests
                    bool isDeadManChest = isContainer2 && Main.tile[chest.x, chest.y].TileFrameX == 4 * 36;
                    bool isSandstoneChest = isContainer2 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;

                    // Replace Suspicious Looking Eyes in Chests with random useful early game potions.
                    if (isBrownChest || isGoldChest || isMahoganyChest || isIvyChest || isIceChest || isMushroomChest || isMarniteChest || isDeadManChest || isSandstoneChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.SuspiciousLookingEye)
                            {
                                // 60% chance of 3-5 Mining Potions
                                // 20% chance of 2-3 Builder's Potions
                                // 20% chance of 5-9 Shine Potions
                                float rng = WorldGen.genRand.NextFloat();
                                if (rng < 0.2f)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.ShinePotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(5, 10);
                                }
                                else if (rng < 0.4f)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.BuilderPotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(2, 4);
                                }
                                else
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.MiningPotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(3, 6);
                                }
                                break;
                            }
                        }
                    }

                    // Adds Desert Medallion to Sandstone Chests at a 20% chance
                    if (isSandstoneChest)
                    {
                        float rng = WorldGen.genRand.NextFloat();
                        if (rng < 0.2f)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (chest.item[inventoryIndex].IsAir)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<DesertMedallion>());
                                    chest.item[inventoryIndex].stack = 1;
                                    break;
                                }
                            }
                        }
                    }

                }
            }

            // Save the set of ores that got generated
            OreTypes[0] = (ushort)WorldGen.copperBar;
            OreTypes[1] = (ushort)WorldGen.ironBar;
            OreTypes[2] = (ushort)WorldGen.silverBar;
            OreTypes[3] = (ushort)WorldGen.goldBar;
        }
        #endregion
    }
}
