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
using Terraria.Localization;

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
        public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight)
        {
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

            // Replace the entire fucking Dungeon generation pass because nothing else will work as intended
            int DungeonIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Dungeon"));
            tasks[DungeonIndex] = new PassLegacy("Dungeon", (progress, config) =>
            {
                progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.BetterDungeon").Value;
                CustomDungeon.NewDungeon();
            });

            // Larger Jungle Temple
            int JungleTempleIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Jungle Temple"));
            tasks[JungleTempleIndex] = new PassLegacy("Jungle Temple", (progress, config) =>
            {
                progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.BetterJungleTemple").Value;
                CustomTemple.NewJungleTemple();
            });

            // Improved Golem arena
            int JungleTempleIndex2 = tasks.FindIndex(genpass => genpass.Name.Equals("Temple"));
            tasks[JungleTempleIndex2] = new PassLegacy("Temple", (progress, config) =>
            {
                progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.BetterJungleTemple").Value;
                Main.tileSolid[162] = false;
                Main.tileSolid[226] = true;
                CustomTemple.NewJungleTemplePart2();
                Main.tileSolid[232] = false;
            });

            // Better Lihzahrd altar (consistency?)
            int LihzahrdAltarIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Lihzahrd Altars"));
            tasks[LihzahrdAltarIndex] = new PassLegacy("Lihzahrd Altars", (progress, config) =>
            {
                progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.JungleTempleAltar").Value;
                CustomTemple.NewJungleTempleLihzahrdAltar();
            });

            // Giant beehive
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
                        Point origin = WorldGen.RandomWorldPoint((int)Main.worldSurface + 25, 20, Main.maxTilesY - (int)Main.worldSurface - 125, 20);
                        if (GiantHive.CanPlaceGiantHive(origin, GenVars.structures))
                            break;
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

            // Vernal pass
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

                    int sunkenSeaX = GenVars.UndergroundDesertLocation.Left;
                    int sunkenSeaY = Main.maxTilesY - 400;

                    SunkenSea.Place(new Point(sunkenSeaX, sunkenSeaY));
                }));
            }

            // All further tasks occur after vanilla worldgen is completed
            int FinalIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Final Cleanup"));
            if (FinalIndex != -1)
            {
                // Reallocate gems so rarity corresponds to depth
                int currentFinalIndex = FinalIndex;
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
                int SulphurIndex = tasks.FindIndex(genpass => genpass.Name.Equals("Micro Biomes"));
                if (SulphurIndex != -1)
                {
                    tasks.Insert(SulphurIndex + 1, new PassLegacy("Sulphur Sea", (progress, config) =>
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
                }));

                // Aerialite
                // This MUST generate after the evil island, otherwise the ores keep getting painted from the evil island gen
                tasks.Insert(++currentFinalIndex, new PassLegacy("Aerialite", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.Aerialite").Value;
                    AerialiteOreGen.Generate(false);
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
                }));

                // Sulphurous Sea (Part 2, after Abyss)
                tasks.Insert(++currentFinalIndex, new PassLegacy("Sulphur Sea 2", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.SulphurSea2").Value;
                    SulphurousSea.SulphurSeaGenerationAfterAbyss();
                }));

                // Roxcalibur
                tasks.Insert(++currentFinalIndex, new PassLegacy("Roxcalibur", (progress, config) =>
                {
                    progress.Message = Language.GetOrRegister("Mods.CalamityMod.UI.Roxcalibur").Value;
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
                    // Checks which sheet a chest belongs to
                    bool isContainer1 = Main.tile[chest.x, chest.y].TileType == TileID.Containers;
                    bool isContainer2 = Main.tile[chest.x, chest.y].TileType == TileID.Containers2;

                    // Pre-1.4 chests
                    bool isBrownChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 0;
                    bool isGoldChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 36 || Main.tile[chest.x, chest.y].TileFrameX == 2 * 36); // Includes Locked Gold Chests
                    bool isMahoganyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 8 * 36;
                    bool isIvyChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 10 * 36;
                    bool isIceChest = isContainer1 &&  Main.tile[chest.x, chest.y].TileFrameX == 11 * 36;
                    bool isMushroomChest = isContainer1 && Main.tile[chest.x, chest.y].TileFrameX == 32 * 36;
                    bool isMarniteChest = isContainer1 && (Main.tile[chest.x, chest.y].TileFrameX == 50 * 36 || Main.tile[chest.x, chest.y].TileFrameX == 51 * 36);

                    // 1.4 chests
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

                    // Give Dead Man's Chests better loot.
                    if (isDeadManChest)
                    {
                        for (int inventoryIndex = 0; inventoryIndex < 40; inventoryIndex++)
                        {
                            // Cavern Dead Man's Chests
                            if (chest.y > GenVars.lavaLine || (chest.y <= GenVars.lavaLine && Main.remixWorld))
                            {
                                if (chest.item[inventoryIndex].type == ItemID.Dynamite)
                                    chest.item[inventoryIndex].SetDefaults(ItemID.StickyDynamite);

                                if (chest.item[inventoryIndex].type == ItemID.JestersArrow)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.HolyArrow);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(25, 51);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.SilverBar ||
                                    chest.item[inventoryIndex].type == ItemID.TungstenBar ||
                                    chest.item[inventoryIndex].type == ItemID.GoldBar ||
                                    chest.item[inventoryIndex].type == ItemID.PlatinumBar)
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(6, 16);

                                if (chest.item[inventoryIndex].type == ItemID.FlamingArrow)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.HellfireArrow);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(25, 51);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.ThrowingKnife)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.PoisonedKnife);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(25, 51);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.HealingPotion)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.RestorationPotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(3, 6);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.RecallPotion)
                                    chest.item[inventoryIndex].SetDefaults(ItemID.PotionOfReturn);

                                if (chest.item[inventoryIndex].type == ItemID.Torch)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.UltrabrightTorch);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(15, 30);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.Glowstick)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.SpelunkerGlowstick);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(15, 30);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.GoldCoin)
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(2, 4);
                            }

                            // Underground Dead Man's Chests
                            else
                            {
                                if (chest.item[inventoryIndex].type == ItemID.Bomb)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.StickyBomb);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(10, 20);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.Rope)
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(100, 201);

                                if (chest.item[inventoryIndex].type == ItemID.IronBar ||
                                    chest.item[inventoryIndex].type == ItemID.LeadBar ||
                                    chest.item[inventoryIndex].type == ItemID.SilverBar ||
                                    chest.item[inventoryIndex].type == ItemID.TungstenBar)
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(10, 20);

                                if (chest.item[inventoryIndex].type == ItemID.WoodenArrow)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.FlamingArrow);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(25, 50);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.Shuriken)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.ThrowingKnife);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(25, 50);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.LesserHealingPotion)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.HealingPotion);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(3, 6);
                                }

                                if (chest.item[inventoryIndex].type == ItemID.SilverCoin)
                                {
                                    chest.item[inventoryIndex].SetDefaults(ItemID.GoldCoin);
                                    chest.item[inventoryIndex].stack = WorldGen.genRand.Next(1, 3);
                                }
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
            OreTypes[0] = (ushort)GenVars.copperBar;
            OreTypes[1] = (ushort)GenVars.ironBar;
            OreTypes[2] = (ushort)GenVars.silverBar;
            OreTypes[3] = (ushort)GenVars.goldBar;
        }
        #endregion
    }
}
