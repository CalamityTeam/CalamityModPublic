using CalamityMod.Tiles.DraedonStructures;
using CalamityMod.TileEntities;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static CalamityMod.Schematics.SchematicManager;

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

        public static void PlaceStructure<T>(string mapKey, Point placementPosition, PlacementAnchorType placementAnchor, ref bool specialCondition, T chestInteraction = null) where T : Delegate
        {
            if (chestInteraction != null &&
                !(chestInteraction is Action<Chest>) && 
                !(chestInteraction is Action<Chest, int, bool>))
            {
                throw new ArgumentException("The chest interaction function has invalid parameters.", nameof(chestInteraction));
            }

            // If no structure schematic matching that name was found, cancel.
            if (!TileMaps.ContainsKey(mapKey))
                return;
            PilePlacementMaps.TryGetValue(mapKey, out PilePlacementFunction pilePlacementFunction);
            SchematicMetaTile[,] schematic = TileMaps[mapKey];

            // Make an array for the tiles that used to be where this schematic will be pasted.
            int width = schematic.GetLength(0);
            int height = schematic.GetLength(1);
            Tile[,] oldTiles = new Tile[width, height];

            int cornerX = placementPosition.X;
            int cornerY = placementPosition.Y;

            // TopLeft is the default because anchoring things at their top-left corner is the default Terraria behavior.
            // This is why it does nothing.
            switch (placementAnchor)
            {
                case PlacementAnchorType.TopRight:
                    cornerX += width;
                    break;
                case PlacementAnchorType.Center:
                    cornerX += width / 2;
                    cornerY += height / 2;
                    break;
                case PlacementAnchorType.BottomLeft:
                    cornerY += height;
                    break;
                case PlacementAnchorType.BottomRight:
                    cornerX += width;
                    cornerY += height;
                    break;
                case PlacementAnchorType.TopLeft:
                default:
                    break;
            }

            // Fill the old tiles array with everything that was originally in the target rectangle.
            // Once the old tiles array is filled, destroy everything in the target rectangle (except chests).
            // This two-step process is necessary so that complex tiles are properly removed and replaced instead of getting sliced up.
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    oldTiles[x, y] = (Tile)Main.tile[x + cornerX, y + cornerY].Clone();

            // Attempting to break vanilla chests directly causes infinite recursion, so don't.
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                    if (oldTiles[x, y].type != TileID.Containers)
                        WorldGen.KillTile(x + cornerX, y + cornerY);

            // Lay down the schematic. If the schematic calls for it, bring back tiles that are stored in the old tiles array.
            for (int x = 0; x < width; ++x)
                for (int y = 0; y < height; ++y)
                {
                    if (!WorldGen.InWorld(x + cornerX, y + cornerY))
                        continue;

                    SchematicMetaTile smt = schematic[x, y];
                    ModTile modTile = TileLoader.GetTile(smt.type);
                    bool isChest = smt.type == TileID.Containers || (modTile != null && modTile.chest != "");

                    // If the determined tile type is a chest, define it appropriately.
                    if (isChest)
                    {
                        if (smt.frameX % 36 == 0 && smt.frameY == 0)
                        {
                            Chest chest = PlaceChest(x + cornerX, y + cornerY, smt.type);

                            if (chestInteraction is Action<Chest, int, bool>)
                            {
                                (chestInteraction as Action<Chest, int, bool>)?.Invoke(chest, smt.type, specialCondition);
                                specialCondition = true;
                            }
                            else if (chestInteraction is Action<Chest>)
                                (chestInteraction as Action<Chest>)?.Invoke(chest);
                        }
                    }

                    // Trees and cacti get special treatment and are always imported back in to prevent world corruption.
                    if (smt.type == TileID.Trees || smt.type == TileID.PineTree || smt.type == TileID.Cactus)
                        oldTiles[x, y] = new Tile
                        {
                            wall = oldTiles[x, y].wall
                        };
                    else
                    {
                        // This is where the meta tile keep booleans are applied.
                        smt.ApplyTo(ref Main.tile[x + cornerX, y + cornerY], oldTiles[x, y]);
                        TryToPlaceTileEntities(x + cornerX, y + cornerY);
                    }

                    Rectangle placeInArea = new Rectangle(x, y, width, height);
                    pilePlacementFunction?.Invoke(x + cornerX, y + cornerY, placeInArea);
                }
        }

        private static void TryToPlaceTileEntities(int x, int y)
        {
            Tile t = CalamityUtils.ParanoidTileRetrieval(x, y);
            int tileType = t.type;

            // A tile entity in an empty spot would make no sense.
            if (!t.active())
                return;

            // Ignore tiles that aren't at the top left of the tile.
            // All of Calamity's worldgen-placed tile entities refuse to exist except at the top left corner of their host tile.
            if (t.frameX != 0 || t.frameY != 0)
                return;

            // This cannot be a switch because switch cases must be compile time constants, which ModContent calls are not.
            // Therefore the only option is an if-else ladder.
            else if (tileType == ModContent.TileType<ChargingStation>())
                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEChargingStation>());
            else if (tileType == ModContent.TileType<DraedonLabTurret>())
                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TEDraedonLabTurret>());
            else if (tileType == ModContent.TileType<LabHologramProjector>())
                TileEntity.PlaceEntityNet(x, y, ModContent.TileEntityType<TELabHologramProjector>());
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
