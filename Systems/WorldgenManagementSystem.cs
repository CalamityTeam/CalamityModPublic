using System;
using System.Collections.Generic;
using CalamityMod.Tiles.Abyss;
using CalamityMod.Tiles.Astral;
using CalamityMod.Tiles.AstralDesert;
using CalamityMod.Tiles.AstralSnow;
using CalamityMod.Tiles.Crags;
using CalamityMod.Tiles.Ores;
using CalamityMod.Tiles.SunkenSea;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod.Systems
{
    public class WorldgenManagementSystem : ModSystem
    {
        #region PreWorldGen
        public override void PreWorldGen()
        {
            numAbyssIslands = 0;
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
                tasks.Insert(DungeonChestIndex + 1, new PassLegacy("Calamity Mod: Biome Chests", MiscWorldgenRoutines.GenerateBiomeChests));
            }

            int WaterFromSandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Remove Water From Sand"));
            if (WaterFromSandIndex != -1)
            {
                tasks.Insert(WaterFromSandIndex + 1, new PassLegacy("SunkenSea", (progress, config) =>
                {
                    progress.Message = "Making the world more wet";
                    SunkenSea.Place(new Point(WorldGen.UndergroundDesertLocation.Left, WorldGen.UndergroundDesertLocation.Bottom));
                }));
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

            int LihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
            tasks[LihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", (progress, config) =>
            {
                progress.Message = "Placing the Lihzahrd altar";
                CustomTemple.NewJungleTempleLihzahrdAltar();
            });

            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                //Not touching this yet because the Crags will be reworked in the future
                #region BrimstoneCrag
                tasks.Insert(FinalIndex + 1, new PassLegacy("BrimstoneCrag", (progress, config) =>
                {
                    progress.Message = "Uncovering the ruins of a fallen empire";

                    int x = Main.maxTilesX;

                    int xUnderworldGen = WorldGen.genRand.Next((int)((double)x * 0.1), (int)((double)x * 0.15));
                    int yUnderworldGen = Main.maxTilesY - 100;

                    fuhX = xUnderworldGen;
                    fuhY = yUnderworldGen;

                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen, yUnderworldGen, 180, 201, 120, 136);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 50, yUnderworldGen - 30, 100, 111, 60, 71);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 50, yUnderworldGen - 30, 100, 111, 60, 71);

                    MiscWorldgenRoutines.ChasmGenerator(fuhX - 110, fuhY - 10, WorldGen.genRand.Next(150) + 150);
                    MiscWorldgenRoutines.ChasmGenerator(fuhX + 110, fuhY - 10, WorldGen.genRand.Next(150) + 150);

                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 150, yUnderworldGen - 30, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen - 180, yUnderworldGen - 20, 60, 66, 35, 41);
                    MiscWorldgenRoutines.UnderworldIsland(xUnderworldGen + 180, yUnderworldGen - 20, 60, 66, 35, 41);

                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX, fuhY + 30, 1323);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 22, fuhY + 15, 1322);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 22, fuhY + 15, 535);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 50, fuhY - 30, 112);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 50, fuhY - 30, 906);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 150, fuhY - 30, 218);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 150, fuhY - 30, 3019);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX - 180, fuhY - 20, 274);
                    MiscWorldgenRoutines.UnderworldIslandHouse(fuhX + 180, fuhY - 20, 220);
                }));
                #endregion

                int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
                if (SulphurIndex != -1)
                {
                    tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur", (progress, config) =>
                    {
                        progress.Message = "Polluting the ocean";
                        SulphurousSea.PlaceSulphurSea();
                    }));
                }

                tasks.Insert(FinalIndex + 2, new PassLegacy("SpecialShrines", (progress, config) =>
                {
                    progress.Message = "Placing Special Shrines";
                    UndergroundShrines.PlaceShrines();
                }));


                tasks.Insert(FinalIndex + 3, new PassLegacy("Rust and Dust", (progress, config) =>
                {
                    List<Point> workshopPositions = new List<Point>();
                    int workshopCount = Main.maxTilesX / 900;
                    int labCount = Main.maxTilesX / 1500;

                    DraedonStructures.PlaceHellLab(out Point hellPlacementPosition, workshopPositions);
                    workshopPositions.Add(hellPlacementPosition);

                    DraedonStructures.PlaceSunkenSeaLab(out Point sunkenSeaPlacementPosition, workshopPositions);
                    workshopPositions.Add(sunkenSeaPlacementPosition);

                    DraedonStructures.PlaceIceLab(out Point icePlacementPosition, workshopPositions);
                    workshopPositions.Add(icePlacementPosition);

                    DraedonStructures.PlacePlagueLab(out Point plaguePlacementPosition, workshopPositions);
                    workshopPositions.Add(plaguePlacementPosition);

                    for (int i = 0; i < workshopCount; i++)
                    {
                        DraedonStructures.PlaceWorkshop(out Point placementPosition, workshopPositions);
                        workshopPositions.Add(placementPosition);
                    }
                    for (int i = 0; i < labCount; i++)
                    {
                        DraedonStructures.PlaceResearchFacility(out Point placementPosition, workshopPositions);
                        workshopPositions.Add(placementPosition);
                    }
                }));

                tasks.Insert(FinalIndex + 4, new PassLegacy("Abyss", (progress, config) =>
                {
                    progress.Message = "Discovering the new Challenger Deep"; //Putting the Mariana Trench to shame
                    Abyss.PlaceAbyss();
                }));

                tasks.Insert(FinalIndex + 5, new PassLegacy("Sulphur2", (progress, config) =>
                {
                    progress.Message = "Polluting the ocean more";
                    SulphurousSea.FinishGeneratingSulphurSea();
                }));

                tasks.Insert(FinalIndex + 6, new PassLegacy("IWannaRock", (progress, config) =>
                {
                    progress.Message = "I Wanna Rock";
                    MiscWorldgenRoutines.PlaceRoxShrine();
                }));

                tasks.Insert(FinalIndex + 7, new PassLegacy("GoodGameDesignGemGen", (progress, config) =>
                {
                    progress.Message = "Good Game Design Gem Gen";
                    MiscWorldgenRoutines.SmartGemGen();
                }));
            }

            tasks.Add(new PassLegacy("Planetoid Test", Planetoid.GenerateAllBasePlanetoids));
        }

        // An Astral Meteor always falls at the beginning of Hardmode.
        public override void ModifyHardmodeTasks(List<GenPass> tasks)
        {
            // Yes, this internal identifier is misspelled in vanilla.
            int announceIndex = tasks.FindIndex(match => match.Name == "Hardmode Announcment");

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
            // 60% chance of 3-5 Mining Potions
            // 20% chance of 2-3 Builder's Potions
            // 20% chance of 5-9 Shine Potions
            WeightedItemStack[] replacementPotions = new WeightedItemStack[]
            {
                DropHelper.WeightStack(ItemID.MiningPotion, 0.6f, 3, 5),
                DropHelper.WeightStack(ItemID.BuilderPotion, 0.2f, 2, 3),
                DropHelper.WeightStack(ItemID.ShinePotion, 0.2f, 5, 9),
            };

            // Replace Suspicious Looking Eyes in Chests with random useful early game potions.
            for (int chestIndex = 0; chestIndex < Main.maxChests; chestIndex++)
            {
                Chest chest = Main.chest[chestIndex];
                if (chest != null && Main.tile[chest.x, chest.y].TileType == TileID.Containers)
                {
                    bool isGoldChest = Main.tile[chest.x, chest.y].TileFrameX == 36;
                    bool isMahoganyChest = Main.tile[chest.x, chest.y].TileFrameX == 8 * 36;
                    bool isIvyChest = Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;
                    bool isIceChest = Main.tile[chest.x, chest.y].TileFrameX == 11 * 36;
                    if (isGoldChest || isMahoganyChest || isIvyChest || isIceChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.SuspiciousLookingEye)
                            {
                                WeightedItemStack replacement = DropHelper.RollWeightedRandom(replacementPotions);
                                chest.item[inventoryIndex].SetDefaults(replacement.itemID);
                                chest.item[inventoryIndex].stack = replacement.ChooseQuantity();
                                break;
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
