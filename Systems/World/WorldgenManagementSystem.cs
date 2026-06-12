using System.Collections.Generic;
using System.Threading;
using CalamityMod.Items.Accessories.Vanity;
using CalamityMod.Items.SummonItems;
using CalamityMod.Items.Weapons.Rogue;
using CalamityMod.Items.Weapons.Summon;
using CalamityMod.World;
using CalamityMod.World.Planets;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Generation;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.WorldBuilding;
using static CalamityMod.World.CalamityWorld;

namespace CalamityMod.Systems
{
    public class WorldgenManagementSystem : ModSystem
    {
        #region StructurePosStoring
        public static Point DungeonArchivePos = Point.Zero;

        public override void SaveWorldData(TagCompound tag)
        {
            tag["DungeonArchivePos"] = DungeonArchivePos;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            if (tag.ContainsKey("DungeonArchivePos"))
                DungeonArchivePos = tag.Get<Point>("DungeonArchivePos");
            else
                DungeonArchivePos = Point.Zero;
        }
        #endregion

        #region PreWorldGen
        public override void PreWorldGen()
        {
            Abyss.TotalPlacedIslandsSoFar = 0;
            DungeonArchivePos = Point.Zero;
            //roxShrinePlaced = false;

            // This will only be applied at world-gen time to new worlds.
            // Old worlds will never receive this marker naturally.
            IsWorldAfterDraedonUpdate = true;
        }
        #endregion

        #region ModifyWorldGenTasks
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
            // Better Underworld structures after the world has been smoothed
            int underworldStructuresIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Smooth World"));
            if (underworldStructuresIndex != -1)
            {
                // Generate the Shimmer Shrine directly above the center of the underground Shimmer lake
                tasks.Insert(underworldStructuresIndex + 2, new PassLegacy("Shimmer Shrine", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.ShimmerShrine").Value;
                    ShimmerShrine.PlaceShimmerShrine(GenVars.structures);
                }));
            }

            // Evil Floating Island
            int islandIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Floating Island Houses"));
            if (islandIndex != -1)
            {
                tasks.Insert(islandIndex + 2, new PassLegacy("Evil Island", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister(WorldGen.crimson ? "Mods.CalamityMod.UI.EvilIslandCrimson" : "Mods.CalamityMod.UI.EvilIslandCorrupt").Value;
                    WorldEvilIsland.PlaceEvilIsland();
                }));
            }

            // Generate the Astral Chest right after the dungeon has finished generating
            int dungeonIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            if (dungeonIndex != -1)
            {
                tasks.Insert(dungeonIndex + 1, new PassLegacy("Astral Chest", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.AstralChest").Value;
                    AstralChestGeneration.PlaceAstralChest();
                }));
            }

            // Generate a large Living Mahogany tree on the surface of the jungle (or anywhere in Drunk world)
            int livingTreeIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Living Trees"));
            if (livingTreeIndex != -1)
            {
                tasks.Insert(livingTreeIndex + 1, new PassLegacy("Living Mahogany Tree", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.LivingMahoganyTree").Value;
                    int attempts = 0;
                    while (attempts < 1000)
                    {
                        attempts++;
                        Point origin = WorldGen.RandomWorldPoint((int)Main.worldSurface + 25, 100, Main.maxTilesY - (int)Main.worldSurface - 125, 100);
                        if (GiantHive.GrowLivingJungleTree(origin, GenVars.structures))
                            break;
                    }
                }));
            }

            // Larger Jungle Temple
            int jungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            if (jungleTempleIndex != -1)
            {
                tasks[jungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.BetterJungleTemple").Value;
                    CustomTemple.NewJungleTemple();
                });
            }

            // Improved Golem Arena
            int jungleTempleIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Temple"));
            if (jungleTempleIndex2 != -1)
            {
                tasks[jungleTempleIndex2] = new PassLegacy("Temple", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.BetterJungleTemple").Value;
                    Main.tileSolid[162] = false;
                    Main.tileSolid[226] = true;
                    CustomTemple.NewJungleTemplePart2();
                    Main.tileSolid[232] = false;
                });
            }

            // Better Lihzahrd Altar (consistency?)
            int lihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
            if (lihzahrdAltarIndex != -1)
            {
                tasks[lihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.JungleTempleAltar").Value;
                    CustomTemple.NewJungleTempleLihzahrdAltar();
                });
            }

            // Big Hive
            int giantHiveIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Hives"));
            if (giantHiveIndex != -1)
            {
                tasks.Insert(giantHiveIndex + 1, new PassLegacy("Giant Hive", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.GiantBeehive").Value;
                    int attempts = 0;
                    while (attempts < 1000)
                    {
                        attempts++;
                        Point origin = WorldGen.RandomWorldPoint((int)Main.worldSurface + 25, 100, Main.maxTilesY - (int)Main.UnderworldLayer + 125, 100);
                        if (GiantHive.CanPlaceGiantHive(origin, GenVars.structures))
                            break;
                    }
                }));
            }

            // Move spawn point in Celebrationmk10 to not be in the Sulphurous Sea
            int spawnPointIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Spawn Point"));
            if (spawnPointIndex != -1 && WorldGen.tenthAnniversaryWorldGen && !WorldGen.remixWorldGen)
            {
                tasks.Insert(spawnPointIndex + 1, new PassLegacy("Fix Tenth Anniversary Spawn", (progress, config) =>
                {
                    if ((Main.spawnTileX < Main.maxTilesX / 2 && GenVars.dungeonSide == -1) || (Main.spawnTileX > Main.maxTilesX / 2 && GenVars.dungeonSide == 1))
                    {
                        // Flip the side of the world you spawn on if it's the Dungeon side
                        Main.spawnTileX = Main.maxTilesX - Main.spawnTileX;
                        // Then fix the Y position of the spawn point
                        for (int i = 0; i < Main.maxTilesY; i++)
                        {
                            if (Main.tile[Main.spawnTileX, i].HasTile)
                            {
                                Main.spawnTileY = i;
                                break;
                            }
                        }
                    }

                }));
            }

            // Mechanic Shed
            int mechanicIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Sunflowers"));
            if (mechanicIndex != -1)
            {
                tasks.Insert(mechanicIndex + 1, new PassLegacy("Mechanic Shed", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.MechanicShed").Value;
                    MechanicShed.PlaceMechanicShed(GenVars.structures);
                }));
            }

            // Vernal Pass
            int vernalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Muds Walls In Jungle"));
            if (vernalIndex != -1)
            {
                tasks.Insert(vernalIndex + 1, new PassLegacy("Vernal Pass", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.VernalPass").Value;
                    VernalPass.PlaceVernalPass(GenVars.structures);
                }));
            }

            // Sunken sea
            int SunkenSeaIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Settle Liquids Again"));
            if (SunkenSeaIndex != -1)
            {
                tasks.Insert(SunkenSeaIndex + 1, new PassLegacy("Sunken Sea", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.SunkenSea").Value;

                    Point ssBottomLeft = new Point(GenVars.UndergroundDesertLocation.Left, Main.maxTilesY - 400);
                    SunkenSea.Place(ssBottomLeft);
                }));
            }

            // All further tasks occur right before vanilla worldgen is completed (which includes The Dirtiest Block and final secret seed adjustments)
            int finalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (finalIndex != -1)
            {
                int currentFinalIndex = finalIndex - 1;

                // Reallocate gems so rarity corresponds to depth
                tasks.Insert(++currentFinalIndex, new PassLegacy("Gem Depth Adjustment", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.GemAdjustment").Value;

                    MiscWorldgenRoutines.SmartGemGen();
                }));

                // Forsaken Archive structure in the Dungeon
                tasks.Insert(++currentFinalIndex, new PassLegacy("Forsaken Archive", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.DungeonArchive").Value;

                    DungeonArchive.PlaceArchive();
                }));

                // Planetoids
                tasks.Insert(++currentFinalIndex, new PassLegacy("Planetoids", Planetoid.GenerateAllBasePlanetoids));

                // Sulphurous Sea (Step 1)
                int sulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
                if (sulphurIndex != -1)
                {
                    tasks.Insert(sulphurIndex + 1, new PassLegacy("Sulphur Sea", (progress, config) =>
                    {
                        progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.SulphurSea").Value;

                        SulphurousSea.PlaceSulphurSea();
                    }));
                }

                // Brimstone Crags
                tasks.Insert(++currentFinalIndex, new PassLegacy("Brimstone Crag", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.BrimstoneCrags").Value;
                    BrimstoneCrag.GenAllCragsStuff();
                }));

                // Biome shrines
                tasks.Insert(++currentFinalIndex, new PassLegacy("Special Shrines", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.HiddenShrines").Value;

                    // Checks for what evil the world are put to cut down on worldgen time.
                    // On the drunk seed or getfixedboi, both shrines generate
                    if (WorldGen.crimson || Main.drunkWorld)
                    {
                        progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.CrimsonShrine").Value;
                        UndergroundShrines.PlaceCrimsonShrine(GenVars.structures);
                    }
                    if (!WorldGen.crimson || Main.drunkWorld)
                    {
                        progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.CorruptShrine").Value;
                        UndergroundShrines.PlaceCorruptionShrine(GenVars.structures);
                    }

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.DesertShrine").Value;
                    UndergroundShrines.PlaceDesertShrine(GenVars.structures);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.GraniteShrine").Value;
                    UndergroundShrines.PlaceGraniteShrine(GenVars.structures);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.IceShrine").Value;
                    UndergroundShrines.PlaceIceShrine(GenVars.structures);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.MarbleShrine").Value;
                    UndergroundShrines.PlaceMarbleShrine(GenVars.structures);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.MushroomShrine").Value;
                    UndergroundShrines.PlaceMushroomShrine(GenVars.structures);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.SurfaceShrine").Value;
                    UndergroundShrines.PlaceSurfaceShrine(GenVars.structures);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.Roxcalibur").Value;
                    UndergroundShrines.PlaceRoxShrine(GenVars.structures);
                }));

                // Aerialite
                // This MUST generate after the evil island, otherwise the ores keep getting painted from the evil island gen
                tasks.Insert(++currentFinalIndex, new PassLegacy("Aerialite", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.Aerialite").Value;
                    AerialiteOreGen.Generate();
                }));

                // Draedon Labs
                tasks.Insert(++currentFinalIndex, new PassLegacy("Draedon Structures", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.DraedonLabs").Value;
                    List<Point> workshopPositions = new List<Point>();

                    // Small: 4, Normal: 7, Large: 9
                    // Tries to scale up reasonably for XL worlds
                    int workshopCount = Main.maxTilesX / 900;

                    // Small: 2, Normal: 4, Large: 5
                    // Tries to scale up reasonably for XL worlds
                    int labCount = Main.maxTilesX / 1500;

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.HellLab").Value;
                    DraedonStructures.PlaceHellLab(out Point hellPlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(hellPlacementPosition);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.SunkenLab").Value;
                    DraedonStructures.PlaceSunkenSeaLab(out Point sunkenSeaPlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(sunkenSeaPlacementPosition);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.IceLab").Value;
                    DraedonStructures.PlaceIceLab(out Point icePlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(icePlacementPosition);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.PlagueLab").Value;
                    DraedonStructures.PlacePlagueLab(out Point plaguePlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(plaguePlacementPosition);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.CavernLab").Value;
                    DraedonStructures.PlaceCavernLab(out Point cavernPlacementPosition, workshopPositions, GenVars.structures);
                    workshopPositions.Add(cavernPlacementPosition);

                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.DraedonWorkshop").Value;
                    for (int i = 0; i < workshopCount; i++)
                    {
                        DraedonStructures.PlaceWorkshop(out Point placementPosition, workshopPositions, GenVars.structures);
                        workshopPositions.Add(placementPosition);
                    }
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.DraedonFacility").Value;
                    for (int i = 0; i < labCount; i++)
                    {
                        DraedonStructures.PlaceResearchFacility(out Point placementPosition, workshopPositions, GenVars.structures);
                        workshopPositions.Add(placementPosition);
                    }
                }));

                // Abyss
                tasks.Insert(++currentFinalIndex, new PassLegacy("Abyss", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.Abyss").Value;
                    Abyss.PlaceAbyss();
                    Abyss.AbyssCleanup();
                }));

                // Sulphurous Sea (Part 2, after Abyss)
                tasks.Insert(++currentFinalIndex, new PassLegacy("Sulphur Sea 2", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.SulphurSea2").Value;
                    SulphurousSea.SulphurSeaGenerationAfterAbyss();
                }));

                tasks.Insert(++currentFinalIndex, new PassLegacy("Iron Ball", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.IronBall").Value;
                    MiscWorldgenRoutines.GenerateIronBall();
                }));

                // No Traps/GFB Auric Land Mines
                if (Main.noTrapsWorld)
                {
                    tasks.Insert(++currentFinalIndex, new PassLegacy("Auric Land Mines", (progress, config) =>
                    {
                        progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.AuricLandMines").Value;
                        MiscWorldgenRoutines.GenerateAuricLandMines();
                    }));
                }
            }
        }

        // An Astral Meteor always falls at the beginning of Hardmode.
        // T1 Hardmode Ores always generate after killing Wall of Flesh.
        public override void ModifyHardmodeTasks(List<GenPass> tasks)
        {
            int announceIndex = tasks.FindIndex(match => match.Name == "Hardmode Announcement");

            //
            // EARLY HARDMODE REWORK
            //
            {
                var hardmodeOreT1Pass = new PassLegacy("CalamityMod:EarlyHMRework_HardmodeOreTier1", (progress, config) =>
                {
                    string key = CalamityMod.Instance.GetLocalization("Status.Progression.HardmodeOreTier1Text").Value;
                    Color messageColor = new Color(50, 255, 130);

                    CalamityUtils.SpawnOre(TileID.Cobalt, 12E-05, 0.45f, 0.7f, 3, 8);
                    CalamityUtils.SpawnOre(TileID.Palladium, 12E-05, 0.45f, 0.7f, 3, 8);

                    CalamityUtils.BroadcastLocalizedText(key, messageColor);
                });

                // Disable gen pass if Early Hardmode Rework is disabled.
                // Could just not add/remove gen pass, but that could lead to mod conflicts
                // in case whatever mod targets this specific gen pass.
                if (!CalamityServerConfig.Instance.EarlyHardmodeProgressionRework)
                    hardmodeOreT1Pass.Disable();

                tasks.Insert(announceIndex, hardmodeOreT1Pass);
            }

            // Insert the Astral biome generation right before the final hardmode announcement.
            tasks.Insert(announceIndex, new PassLegacy("AstralMeteor", (progress, config) =>
            {
                //Delaying it a bit so that weaker pcs dont suffer - Shade
                ThreadPool.QueueUserWorkItem(_ => AstralBiome.PlaceAstralMeteor());
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
                    // Checks which sheet a chest belongs to
                    bool isContainer1 = Main.tile[chest.x, chest.y].TileType == TileID.Containers;
                    bool isContainer2 = Main.tile[chest.x, chest.y].TileType == TileID.Containers2;

                    // Pre-1.4 chests
                    bool isBrownChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 0;
                    bool isGoldChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 36 || Main.tile[chest.x, chest.y].TileFrameX == 2 * 36); // Includes Locked Gold Chests
                    bool isMahoganyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 8 * 36;
                    bool isIvyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;
                    bool isIceChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 11 * 36;
                    bool isLihzahrdChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 16 * 36;
                    bool isMushroomChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 32 * 36;
                    bool isMarniteChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 50 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 51 * 36);

                    // 1.4 chests
                    bool isDeadManChest = isContainer2 && Main.tile[chest.x, chest.y].TileFrameX == 4 * 36;
                    bool isSandstoneChest = isContainer2 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;

                    // Replace Suspicious Looking Eyes in Chests with random useful early game potions.
                    if (isBrownChest || isGoldChest || isMahoganyChest || isIvyChest || isIceChest || isLihzahrdChest || isMushroomChest || isMarniteChest || isDeadManChest || isSandstoneChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            if (chest.item[inventoryIndex].type == ItemID.SuspiciousLookingEye)
                            {
                                // For Mushroom Chests, Suspicious Looking Eyes are replaced with Shroomerang instead
                                if (isMushroomChest)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.Shroomerang);
                                    chest.item[inventoryIndex].Prefix(-1);
                                    break;
                                }

                                if (isGoldChest)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<EnchantedKnifeStaff>());
                                    chest.item[inventoryIndex].Prefix(-1);
                                    break;
                                }

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

                    // Replace Step Stool in surface Chests with Kylie
                    // Calamity adds a very easy recipe for this so hopefully this isn't a big deal (The 4 unironic Step Stool lovers will be out for my head)
                    if (isBrownChest)
                    {
                        if (chest.item[0].type == ItemID.PortableStool)
                        {
                            chest.item[0].SetDefaults(ModContent.ItemType<Kylie>());
                            chest.item[0].Prefix(-1);
                        }
                    }

                    // Adds Desert Medallion and The Comb to Sandstone Chests, each at a 20% chance
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
                        else if (rng < 0.4f)
                        {
                            for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                            {
                                if (chest.item[inventoryIndex].IsAir)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ModContent.ItemType<TheComb>());
                                    chest.item[inventoryIndex].stack = 1;
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}
