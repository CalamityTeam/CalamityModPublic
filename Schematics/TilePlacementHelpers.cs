using CalamityMod.TileEntities;
using CalamityMod.Tiles;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Schematics.SchematicLoader;

namespace CalamityMod.Schematics
{
    public static class SchematicPlacementHelpers
    {
        public enum PlacementAnchorType
        {
            TopLeft,
            TopRight,
            Center,
            BottomLeft,
            BottomRight,
        }
        internal static Tile SchematicTileConversion(Tile oldTile, Tile toReplaceWith, Color schematicColorAtPosition)
        {
            float opacity = schematicColorAtPosition.A / 255f;

            Tile newTile = new Tile();
            // Destroy the original tile completely.
            if (opacity == 0f)
                return newTile;

            // Destroy the original tile itself but preserve the wall.
            if (oldTile.wall != 0 && toReplaceWith.wall == 0 && opacity > 0f && opacity <= 0.33f)
                newTile.wall = oldTile.wall;

            // Do nothing to the tile.
            if (opacity > 0.33f && opacity <= 0.66f)
                newTile = (Tile)oldTile.Clone();

            // Completely replace the tile.
            if (opacity > 0.66f)
                newTile = (Tile)toReplaceWith.Clone();
            return newTile;
        }
        public static void PlaceStructure(string mapKey, Point placementPosition, PlacementAnchorType placementAnchor, Action<Chest> chestInteraction = null)
        {
            if (!TileMaps.ContainsKey(mapKey))
                return;
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            ColorTileCombination[,] schematic = TileMaps[mapKey];
            Tile[,] oldTiles = new Tile[schematic.GetLength(0), schematic.GetLength(1)];
            int xOffset = placementPosition.X;
            int yOffset = placementPosition.Y;
            // Top-left is the default for terraria. There is no need to include it in this switch.
            switch (placementAnchor)
            {
                case PlacementAnchorType.TopRight:
                    xOffset += schematic.GetLength(0);
                    break;
                case PlacementAnchorType.Center:
                    xOffset += schematic.GetLength(0) / 2;
                    yOffset += schematic.GetLength(1) / 2;
                    break;
                case PlacementAnchorType.BottomLeft:
                    yOffset += schematic.GetLength(1);
                    break;
                case PlacementAnchorType.BottomRight:
                    xOffset += schematic.GetLength(0);
                    yOffset += schematic.GetLength(1);
                    break;
            }
            for (int x = 0; x < schematic.GetLength(0); x++)
            {
                for (int y = 0; y < schematic.GetLength(1); y++)
                {
                    oldTiles[x, y] = (Tile)Main.tile[x + xOffset, y + yOffset].Clone();

                    // Attempting to break chests causes the game to attempt to infinitely recurse in an attempt to break the tile, resulting in a stack overflow.
                    if (oldTiles[x, y].type == TileID.Containers)
                        continue;
                    WorldGen.KillTile(x + xOffset, y + yOffset);
                }
            }
            for (int x = 0; x < schematic.GetLength(0); x++)
            {
                for (int y = 0; y < schematic.GetLength(1); y++)
                {
                    if (WorldGen.InWorld(x + xOffset, y + yOffset))
                    {
                        Tile tile = schematic[x, y].InternalTile;
                        ModTile modTile = TileLoader.GetTile(tile.type);
                        bool isChest = tile.type == TileID.Containers || (modTile != null && modTile.chest != "");

                        // If the determined tile type is a chest, define it appropriately.
                        if (isChest)
                        {
                            if (tile.frameX % 36 == 0 && tile.frameY == 0)
                            {
                                Chest chest = PlaceChest(x + xOffset, y + yOffset, tile.type);
                                chestInteraction?.Invoke(chest);
                            }
                        }
                        if (tile.type == ModContent.TileType<DraedonItemCharger>() ||
                            tile.type == ModContent.TileType<DraedonTurretTile>() ||
                            tile.type == ModContent.TileType<DraedonFactoryFieldGenerator>())
                        {
                            WorldGen.PlaceTile(x + xOffset, y + yOffset, tile.type);
                        }
                        else if (tile.type == TileID.Trees || tile.type == TileID.PineTree || tile.type == TileID.Cactus)
                        {
                            ushort oldWall = oldTiles[x, y].wall;
                            oldTiles[x, y] = new Tile
                            {
                                wall = oldWall
                            };
                        }
                        else
                        {
                            Main.tile[x + xOffset, y + yOffset] = (Tile)SchematicTileConversion(oldTiles[x, y], tile, schematic[x, y].InternalColor).Clone();
                        }

                        Rectangle placeInArea = new Rectangle(x, y, schematic.GetLength(0), schematic.GetLength(1));

                        // Dictionary.TryGetValue returns null if the specified key is not present.
                        pilePlacementFunction?.Invoke(x + xOffset, y + yOffset, placeInArea);
                    }
                }
            }
        }
        public static void PlaceStructure(string mapKey, Point placementPosition, PlacementAnchorType placementAnchor, ref bool specialCondition, Action<Chest, int, bool> chestInteraction = null)
        {
            LoadEverything(); // Just in case they weren't loaded properly beforehand.

            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            ColorTileCombination[,] schematic = TileMaps[mapKey];
            Tile[,] oldTiles = new Tile[schematic.GetLength(0), schematic.GetLength(1)];
            int xOffset = placementPosition.X;
            int yOffset = placementPosition.Y;
            // Top-left is the default for terraria. There is no need to include it in this switch.
            switch (placementAnchor)
            {
                case PlacementAnchorType.TopRight:
                    xOffset += schematic.GetLength(0);
                    break;
                case PlacementAnchorType.Center:
                    xOffset += schematic.GetLength(0) / 2;
                    yOffset += schematic.GetLength(1) / 2;
                    break;
                case PlacementAnchorType.BottomLeft:
                    yOffset += schematic.GetLength(1);
                    break;
                case PlacementAnchorType.BottomRight:
                    xOffset += schematic.GetLength(0);
                    yOffset += schematic.GetLength(1);
                    break;
            }
            for (int x = 0; x < schematic.GetLength(0); x++)
            {
                for (int y = 0; y < schematic.GetLength(1); y++)
                {
                    oldTiles[x, y] = (Tile)CalamityUtils.ParanoidTileRetrieval(x + xOffset, y + yOffset).Clone();

                    // Attempting to break chests causes the game to attempt to infinitely recurse in an attempt to break the tile, resulting in a stack overflow.
                    if (oldTiles[x, y].type == TileID.Containers)
                        continue;
                    WorldGen.KillTile(x + xOffset, y + yOffset);
                }
            }
            for (int x = 0; x < schematic.GetLength(0); x++)
            {
                for (int y = 0; y < schematic.GetLength(1); y++)
                {
                    if (WorldGen.InWorld(x + xOffset, y + yOffset))
                    {
                        Tile tile = schematic[x, y].InternalTile;
                        ModTile modTile = TileLoader.GetTile(tile.type);
                        bool isChest = tile.type == TileID.Containers || (modTile != null && modTile.chest != "");
                        // If the determined tile type is a chest, define it appropriately.
                        if (isChest)
                        {
                            if (tile.frameX % 36 == 0 && tile.frameY == 0)
                            {
                                Chest chest = PlaceChest(x + xOffset, y + yOffset, tile.type);
                                chestInteraction?.Invoke(chest, tile.type, specialCondition);
                                specialCondition = true;
                            }
                        }
                        if (tile.type == ModContent.TileType<DraedonItemCharger>() ||
                            tile.type == ModContent.TileType<DraedonTurretTile>() ||
                            tile.type == ModContent.TileType<DraedonFactoryFieldGenerator>())
                        {
                            WorldGen.PlaceTile(x + xOffset, y + yOffset, tile.type);
                        }
                        else if (tile.type == TileID.Trees || tile.type == TileID.PineTree || tile.type == TileID.Cactus)
                        {
                            ushort oldWall = oldTiles[x, y].wall;
                            oldTiles[x, y] = new Tile
                            {
                                wall = oldWall
                            };
                        }
                        else
                        {
                            Main.tile[x + xOffset, y + yOffset] = (Tile)SchematicTileConversion(oldTiles[x, y], tile, schematic[x, y].InternalColor).Clone();
                        }

                        Rectangle placeInArea = new Rectangle(x, y, schematic.GetLength(0), schematic.GetLength(1));

                        // Dictionary.TryGetValue returns null if the specified key is not present.
                        pilePlacementFunction?.Invoke(x + xOffset, y + yOffset, placeInArea);
                    }
                }
            }
        }
        public static Chest PlaceChest(int x, int y, int chestType)
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
    }
}
