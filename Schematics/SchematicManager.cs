using CalamityMod.TileEntities;
using CalamityMod.Tiles.DraedonStructures;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace CalamityMod.Schematics
{
    // TODO -- This can be made into a ModSystem with simple OnModLoad and Unload hooks.
    public static class SchematicManager
    {
        internal const string RustedWorkshopKey = "Rusted Workshop";
        internal const string RustedWorkshopFilename = "Schematics/RustedWorkshop.csch";

        internal const string ResearchOutpostKey = "Research Outpost";
        internal const string ResearchOutpostFilename = "Schematics/ResearchOutpost.csch";

        internal const string SunkenSeaLabKey = "Sunken Sea Laboratory";
        internal const string SunkenSeaLabFilename = "Schematics/DraedonsLab_SunkenSea.csch";

        internal const string PlanetoidLabKey = "Planetoid Laboratory";
        internal const string PlanetoidLabFilename = "Schematics/DraedonsLab_Planetoid.csch";

        internal const string PlagueLabKey = "Plague Laboratory";
        internal const string PlagueLabFilename = "Schematics/DraedonsLab_Plague.csch";

        internal const string HellLabKey = "Hell Laboratory";
        internal const string HellLabFilename = "Schematics/DraedonsLab_Hell.csch";

        internal const string IceLabKey = "Ice Laboratory";
        internal const string IceLabFilename = "Schematics/DraedonsLab_Ice.csch";

        internal const string CavernLabKey = "Cavern Laboratory";
        internal const string CavernLabFilename = "Schematics/DraedonsLab_Cavern.csch";

        internal const string CorruptionShrineKey = "Corruption Shrine";
        internal const string CorruptionShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string CrimsonShrineKey = "Crimson Shrine";
        internal const string CrimsonShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string DesertShrineKey = "Desert Shrine";
        internal const string DesertShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string GraniteShrineKey = "Granite Shrine";
        internal const string GraniteShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string IceShrineKey = "Ice Shrine";
        internal const string IceShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string MarbleShrineKey = "Marble Shrine";
        internal const string MarbleShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string MushroomShrineKey = "Mushroom Shrine";
        internal const string MushroomShrineFilename = "Schematics/Shrine_Desert.csch";

        internal const string SurfaceShrineKey = "Surface Shrine";
        internal const string SurfaceShrineFilename = "Schematics/Shrine_Desert.csch";
        

        internal const string AstralBeaconKey = "Astral Beacon";
        internal const string AstralBeaconFilename = "Schematics/AstralBeacon.csch";
        
        internal static Dictionary<string, SchematicMetaTile[,]> TileMaps;
        internal static Dictionary<string, PilePlacementFunction> PilePlacementMaps;
        public delegate void PilePlacementFunction(int x, int y, Rectangle placeInArea);

        #region Load/Unload
        internal static void Load()
        {
            PilePlacementMaps = new Dictionary<string, PilePlacementFunction>();
            TileMaps = new Dictionary<string, SchematicMetaTile[,]>
            {
                // Draedon's Arsenal world gen structures
                [RustedWorkshopKey] = CalamitySchematicIO.LoadSchematic(RustedWorkshopFilename),
                [ResearchOutpostKey] = CalamitySchematicIO.LoadSchematic(ResearchOutpostFilename),
                [SunkenSeaLabKey] = CalamitySchematicIO.LoadSchematic(SunkenSeaLabFilename),
                [PlanetoidLabKey] = CalamitySchematicIO.LoadSchematic(PlanetoidLabFilename),
                [PlagueLabKey] = CalamitySchematicIO.LoadSchematic(PlagueLabFilename),
                [HellLabKey] = CalamitySchematicIO.LoadSchematic(HellLabFilename),
                [IceLabKey] = CalamitySchematicIO.LoadSchematic(IceLabFilename),
                [CavernLabKey] = CalamitySchematicIO.LoadSchematic(CavernLabFilename),

                // Shrine world gen structures
                [CorruptionShrineKey] = CalamitySchematicIO.LoadSchematic(CorruptionShrineFilename),
                [CrimsonShrineKey] = CalamitySchematicIO.LoadSchematic(CrimsonShrineFilename),
                [DesertShrineKey] = CalamitySchematicIO.LoadSchematic(DesertShrineFilename),
                [GraniteShrineKey] = CalamitySchematicIO.LoadSchematic(GraniteShrineFilename),
                [IceShrineKey] = CalamitySchematicIO.LoadSchematic(IceShrineFilename),
                [MarbleShrineKey] = CalamitySchematicIO.LoadSchematic(MarbleShrineFilename),
                [MushroomShrineKey] = CalamitySchematicIO.LoadSchematic(MushroomShrineFilename),
                [SurfaceShrineKey] = CalamitySchematicIO.LoadSchematic(SurfaceShrineFilename),

                // Astral world gen structures
                [AstralBeaconKey] = CalamitySchematicIO.LoadSchematic(AstralBeaconFilename),

                // Sulphurous Sea scrap world gen structures
                ["Sulphurous Scrap 1"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap1.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 2"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap2.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 3"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap3.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 4"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap4.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 5"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap5.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 6"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap6.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 7"] = CalamitySchematicIO.LoadSchematic("Schematics/SulphurousScrap7.csch").ShaveOffEdge(),
            };
        }
        internal static void Unload()
        {
            TileMaps = null;
            PilePlacementMaps = null;
        }
        #endregion

        #region Get Schematic Area
        public static Vector2? GetSchematicArea(string name)
        {
            // If no schematic exists with this name, simply return null.
            if (!TileMaps.TryGetValue(name, out SchematicMetaTile[,] schematic))
                return null;

            return new Vector2(schematic.GetLength(0), schematic.GetLength(1));
        }
        #endregion Get Schematic Area

        #region Place Schematic
        public static void PlaceSchematic<T>(string name, Point pos, SchematicAnchor anchorType, ref bool specialCondition, T chestDelegate = null) where T : Delegate
        {
            // If no schematic exists with this name, cancel with a helpful log message.
            if (!TileMaps.ContainsKey(name))
            {
                CalamityMod.Instance.Logger.Warn($"Tried to place a schematic with name \"{name}\". No matching schematic file found.");
                return;
            }

            // Invalid chest interaction delegates need to throw an error.
            if (chestDelegate != null &&
                !(chestDelegate is Action<Chest>) &&
                !(chestDelegate is Action<Chest, int, bool>))
            {
                throw new ArgumentException("The chest interaction function has invalid parameters.", nameof(chestDelegate));
            }
            PilePlacementMaps.TryGetValue(name, out PilePlacementFunction pilePlacementFunction);

            // Grab the schematic itself from the dictionary of loaded schematics.
            SchematicMetaTile[,] schematic = TileMaps[name];
            int width = schematic.GetLength(0);
            int height = schematic.GetLength(1);

            // Calculate the appropriate location to start laying down schematic tiles.
            int cornerX = pos.X;
            int cornerY = pos.Y;
            switch (anchorType)
            {
                case SchematicAnchor.TopLeft: // Provided point is top-left corner = No change
                case SchematicAnchor.Default: // This is also default behavior
                default:
                    break;
                case SchematicAnchor.TopCenter: // Provided point is top center = Top-left corner is 1/2 width to the left
                    cornerX -= width / 2;
                    break;
                case SchematicAnchor.TopRight: // Provided point is top-right corner = Top-left corner is 1 width to the left
                    cornerX -= width;
                    break;
                case SchematicAnchor.CenterLeft: // Provided point is left center: Top-left corner is 1/2 height above
                    cornerY -= height / 2;
                    break;
                case SchematicAnchor.Center: // Provided point is center = Top-left corner is 1/2 width and 1/2 height up-left
                    cornerX -= width / 2;
                    cornerY -= height / 2;
                    break;
                case SchematicAnchor.CenterRight: // Provided point is right center: Top-left corner is 1 width and 1/2 height up-left
                    cornerX -= width;
                    cornerY -= height / 2;
                    break;
                case SchematicAnchor.BottomLeft: // Provided point is bottom-left corner = Top-left corner is 1 height above
                    cornerY -= height;
                    break;
                case SchematicAnchor.BottomCenter: // Provided point is bottom center: Top-left corner is 1/2 width and 1 height up-left
                    cornerX -= width / 2;
                    cornerY -= height;
                    break;
                case SchematicAnchor.BottomRight: // Provided point is bottom-right corner = Top-left corner is 1 width to the left and 1 height above
                    cornerX -= width;
                    cornerY -= height;
                    break;
            }

            // Make sure that all four corners of the target area are actually in the world.
            if (!WorldGen.InWorld(cornerX, cornerY) || !WorldGen.InWorld(cornerX + width, cornerY + height))
            {
                CalamityMod.Instance.Logger.Warn("Schematic failed to place: Part of the target location is outside the game world.");
                return;
            }

            // Make an array for the tiles that used to be where this schematic will be pasted.
            SchematicMetaTile[,] originalTiles = new SchematicMetaTile[width, height];

            // Schematic area pre-processing has three steps.
            // Step 1: Kill all trees and cacti specifically. This prevents ugly tree/cactus pieces from being restored later.
            // Step 2: Fill the original tiles array with everything that was originally in the target rectangle.
            // Step 3: Destroy everything in the target rectangle (except chests -- that'll cause infinite recursion).
            // The third step is necessary so that multi tiles on the edge of the region are properly destroyed (e.g. Life Crystals).

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile t = Main.tile[x + cornerX, y + cornerY];
                    if (t.TileType == TileID.Trees || t.TileType == TileID.PineTree || t.TileType == TileID.Cactus)
                        WorldGen.KillTile(x + cornerX, y + cornerY);
                }

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    Tile t = Main.tile[x + cornerX, y + cornerY];
                    originalTiles[x, y] = new SchematicMetaTile(t);
                }

            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (originalTiles[x, y].TileType != TileID.Containers)
                        WorldGen.KillTile(x + cornerX, y + cornerY);

            // Lay down the schematic. If the schematic calls for it, bring back tiles that are stored in the old tiles array.
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    SchematicMetaTile smt = schematic[x, y];
                    smt.ApplyTo(x + cornerX, y + cornerY, originalTiles[x, y]);
                    Tile worldTile = Main.tile[x + cornerX, y + cornerY];

                    // If the determined tile type is a chest and this is its top left corner, define it appropriately.
                    // Skip this step if this schematic position preserves tiles.
                    bool isChest = worldTile.TileType == TileID.Containers || TileID.Sets.BasicChest[worldTile.TileType];
                    if (!smt.keepTile && isChest && worldTile.TileFrameX % 36 == 0 && worldTile.TileFrameY == 0)
                    {
                        // If a chest already exists "near" this position, then the corner was likely already defined.
                        // Do not do anything if a chest was already defined.
                        // FindChestByGuessing checks a 2x2 space starting in the given position, so nudge it up and left by 1.
                        int chestIndex = Chest.FindChestByGuessing(x + cornerX - 1, y + cornerY - 1);
                        if (chestIndex == -1)
                        {
                            chestIndex = Chest.CreateChest(x + cornerX, y + cornerY, -1);
                            Chest chest = Main.chest[chestIndex];
                            // Use the appropriate chest delegate function to fill the chest.
                            if (chestDelegate is Action<Chest, int, bool>)
                            {
                                (chestDelegate as Action<Chest, int, bool>)?.Invoke(chest, worldTile.TileType, specialCondition);
                                specialCondition = true;
                            }
                            else if (chestDelegate is Action<Chest>)
                                (chestDelegate as Action<Chest>)?.Invoke(chest);
                        }
                    }

                    // Now that the tile data is correctly set, place appropriate tile entities.
                    TryToPlaceTileEntities(x + cornerX, y + cornerY, worldTile);

                    // Activate the pile placement function if defined.
                    Rectangle placeInArea = new Rectangle(x, y, width, height);
                    pilePlacementFunction?.Invoke(x + cornerX, y + cornerY, placeInArea);
                }
        }
        #endregion

        #region Place Schematic Helper Methods
        private static void TryToPlaceTileEntities(int x, int y, Tile t)
        {
            // A tile entity in an empty spot would make no sense.
            if (!t.HasTile)
                return;
            // Ignore tiles that aren't at the top left of the tile.
            // All of Calamity's worldgen-placed tile entities refuse to exist except at the top left corner of their host tile.
            if (t.TileFrameX != 0 || t.TileFrameY != 0)
                return;

            // This cannot be a switch because switch cases must be compile time constants, which ModContent calls are not.
            // Therefore the only option is an if-else ladder.
            int tileType = t.TileType;
            if (tileType == ModContent.TileType<ChargingStation>())
                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEChargingStation>());
            else if (tileType == ModContent.TileType<DraedonLabTurret>())
                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEDraedonLabTurret>());
            else if (tileType == ModContent.TileType<LabHologramProjector>())
                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TELabHologramProjector>());
        }
        #endregion
    }
}
