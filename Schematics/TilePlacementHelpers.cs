using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Schematics.SchematicLoader;

namespace CalamityMod.Schematics
{
    public static class TilePlacementHelpers
    {
        public enum PlacementAnchorType
        {
            TopLeft,
            TopRight,
            Center,
            BottomLeft,
            BottomRight,
        }
        internal static void HandleNullTile(ref Tile tile)
        {
            ushort wall = tile.wall;
            tile = new Tile
            {
                wall = wall
            };
        }
        public static void PlaceDraedonStructure(string mapKey, Point placementPosition, PlacementAnchorType placementAnchor, bool preserveWalls = true)
        {
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            Tile[,] tiles = TileMaps[mapKey];
            int xOffset = placementPosition.X;
            int yOffset = placementPosition.Y;
            // Top-left is the default for terraria. There is no need to include it in this switch.
            switch (placementAnchor)
            {
                case PlacementAnchorType.TopRight:
                    xOffset += tiles.GetLength(0);
                    break;
                case PlacementAnchorType.Center:
                    xOffset += tiles.GetLength(0) / 2;
                    yOffset += tiles.GetLength(1) / 2;
                    break;
                case PlacementAnchorType.BottomLeft:
                    yOffset += tiles.GetLength(1);
                    break;
                case PlacementAnchorType.BottomRight:
                    xOffset += tiles.GetLength(0);
                    yOffset += tiles.GetLength(1);
                    break;
            }
            for (int x = 0; x < tiles.GetLength(0); x++)
            {
                for (int y = 0; y < tiles.GetLength(1); y++)
                {
                    if (WorldGen.InWorld(x + xOffset, y + yOffset))
                    {
                        var modTile = TileLoader.GetTile(tiles[x, y].type);
                        bool isChest = tiles[x, y].type == TileID.Containers || (modTile != null && modTile.chest != "");

                        // If the determined tile type is a chest, define it appropriately.
                        if (isChest)
                        {
                            if (tiles[x, y].frameX == 0 && tiles[x, y].frameY == 0)
                            {
                                PlaceChest(x + xOffset, y + yOffset, tiles[x, y].type);
                            }
                        }

                        ushort oldWall = Main.tile[x + xOffset, y + yOffset].wall;
                        Main.tile[x + xOffset, y + yOffset] = (Tile)tiles[x, y].Clone();

                        // If specified, preserve walls if they're not not being overrided and there's no active tile in its place.
                        if (preserveWalls && Main.tile[x + xOffset, y + yOffset].wall == 0 && !Main.tile[x + xOffset, y + yOffset].active())
                        {
                            Main.tile[x + xOffset, y + yOffset].wall = oldWall;
                        }

                        Rectangle placeInArea = new Rectangle(x, y, tiles.GetLength(0), tiles.GetLength(1));

                        // Dictionary.TryGetValue returns null if the specified key is not present.
                        pilePlacementFunction?.Invoke(x + xOffset, y + yOffset, placeInArea);
                    }
                }
            }
        }
        public static void PlaceChest(int x, int y, int chestType)
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
        }
    }
}
