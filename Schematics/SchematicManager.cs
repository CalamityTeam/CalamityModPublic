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
    public static class SchematicManager
    {
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
                ["Workshop"] = CalamitySchematicIO.LoadSchematic("Schematics/Workshop.csch"),
                ["Research Facility"] = CalamitySchematicIO.LoadSchematic("Schematics/ResearchFacility.csch"),
                ["Hell Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/HellLaboratory.csch"),
                ["Sunken Sea Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/SunkenSeaLaboratory.csch"),
                ["Ice Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/IceLaboratory.csch"),
                ["Plague Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/PlagueLaboratory.csch"),
                ["Planetoid Laboratory"] = CalamitySchematicIO.LoadSchematic("Schematics/PlanetoidLaboratory.csch"),

                // Astral world gen structures
                ["Astral Beacon"] = CalamitySchematicIO.LoadSchematic("Schematics/AstralBeacon.csch"),

                // Sulphurous Sea scrap world gen structures
                ["Sulphurous Scrap 1"] = CalamitySchematicIO.LoadSchematic("Schematics/SmallScrapPile1.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 2"] = CalamitySchematicIO.LoadSchematic("Schematics/SmallScrapPile2.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 3"] = CalamitySchematicIO.LoadSchematic("Schematics/SmallScrapPile3.csch").ShaveOffEdge(),
                ["Sulphurous Scrap 4"] = CalamitySchematicIO.LoadSchematic("Schematics/SmallScrapPile4.csch").ShaveOffEdge(),
                ["Large Sulphurous Scrap 1"] = CalamitySchematicIO.LoadSchematic("Schematics/LargeScrapPile1.csch").ShaveOffEdge(),
                ["Large Sulphurous Scrap 2"] = CalamitySchematicIO.LoadSchematic("Schematics/LargeScrapPile2.csch").ShaveOffEdge(),
                ["Large Sulphurous Scrap 3"] = CalamitySchematicIO.LoadSchematic("Schematics/LargeScrapPile3.csch").ShaveOffEdge(),
                ["Large Sulphurous Scrap 4"] = CalamitySchematicIO.LoadSchematic("Schematics/LargeScrapPile4.csch").ShaveOffEdge(),
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
            Tile[,] originalTiles = new Tile[width, height];

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
                    originalTiles[x, y].CopyFrom(Main.tile[x + cornerX, y + cornerY]);
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (originalTiles[x, y].TileType != TileID.Containers)
                        WorldGen.KillTile(x + cornerX, y + cornerY);

            // Lay down the schematic. If the schematic calls for it, bring back tiles that are stored in the old tiles array.
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    SchematicMetaTile smt = schematic[x, y];
                    ref Tile t = ref smt.storedTile;
                    string modChestStr = TileLoader.GetTile(t.TileType)?.ContainerName.GetDefault() ?? "";
                    bool isChest = t.TileType == TileID.Containers || modChestStr != "";

                    // If the determined tile type is a chest and this is its top left corner, define it appropriately.
                    if (isChest && t.TileFrameX % 36 == 0 && t.TileFrameY == 0)
                    {
                        Chest chest = PlaceChest(x + cornerX, y + cornerY, t.TileType);
                        // Use the appropriate chest delegate function to fill the chest.
                        if (chestDelegate is Action<Chest, int, bool>)
                        {
                            (chestDelegate as Action<Chest, int, bool>)?.Invoke(chest, t.TileType, specialCondition);
                            specialCondition = true;
                        }
                        else if (chestDelegate is Action<Chest>)
                            (chestDelegate as Action<Chest>)?.Invoke(chest);
                    }

                    // This is where the meta tile keep booleans are applied.
                    Tile worldTile = Main.tile[x + cornerX, y + cornerY];
                    smt.ApplyTo(ref worldTile, ref originalTiles[x, y]);
                    TryToPlaceTileEntities(x + cornerX, y + cornerY, t);

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

        private static Chest PlaceChest(int x, int y, int chestType)
        {
            int chestIndex = Chest.FindEmptyChest(x, y, chestType);
            Main.chest[chestIndex] = new Chest()
            {
                x = x,
                y = y
            };
            Main.chest[chestIndex].item = new Item[Main.chest[chestIndex].item.Length];
            for (int i = 0; i < Main.chest[chestIndex].item.Length; i++)
            {
                Main.chest[chestIndex].item[i] = new Item();
            }
            return Main.chest[chestIndex];
        }
        #endregion
    }
}
